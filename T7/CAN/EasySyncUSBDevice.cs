using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using T7.CAN;
using System.IO;

namespace T7.KWP
{
    /// <summary>
    /// EasySyncUSBDevice is an implementation of ICANDevice for the Lawicel CANUSB device
    /// (www.canusb.com). 
    /// In this implementation the open method autmatically detects if the device is connected
    /// to a T7 I-bus or P-bus. The autodetection is primarily done by listening for the 0x280
    /// message (sent on both busses) but if the device is started after an interrupted flashing
    /// session there is no such message available on the bus. There fore the open method sends
    /// a message to set address and length for flashing. If there is a reply there is connection.
    /// 
    /// All incomming messages are published to registered ICANListeners.
    /// </summary>
    class EasySyncUSBDevice : ICANDevice
    {

        static uint m_deviceHandle = 0;
        Thread m_readThread;
        Object m_synchObject = new Object();
        bool m_endThread = false;

        /*private bool _useOnlyPBus = true;

        public bool UseOnlyPBus
        {
            get { return _useOnlyPBus; }
            set { _useOnlyPBus = value; }
        }

        private bool m_EnableCanLog = false;

        public bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set { m_EnableCanLog = value; }
        }*/
        /// <summary>
        /// Constructor for EasySyncUSBDevice.
        /// </summary>
        public EasySyncUSBDevice()
        {
            m_readThread = new Thread(readMessages);
        }

        private string m_forcedComport = string.Empty;

        public override string ForcedComport
        {
            get
            {
                return m_forcedComport;
            }
            set
            {
                m_forcedComport = value;
            }
        }
        // not supported by easysync
        public override float GetADCValue(uint channel)
        {
            return 0F;
        }

        // not supported by easysync
        public override float GetThermoValue()
        {
            return 0F;
        }
        /// <summary>
        /// Destructor for EasySyncUSBDevice.
        /// </summary>
        ~EasySyncUSBDevice()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            close();
        }

        public override void Flush()
        {
            EASYSYNC.canusb_Flush(m_deviceHandle);
        }

        /// <summary>
        /// readMessages is the "run" method of this class. It reads all incomming messages
        /// and publishes them to registered ICANListeners.
        /// </summary>
        public void readMessages()
        {
            int readResult = 0;
            EASYSYNC.CANMsg r_canMsg = new EASYSYNC.CANMsg();
            CANMessage canMessage = new CANMessage();
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                        return;
                }
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == EASYSYNC.ERROR_CANUSB_OK)
                {
                    canMessage.setID(r_canMsg.id);
                    canMessage.setLength(r_canMsg.len);
                    canMessage.setTimeStamp(r_canMsg.timestamp);
                    canMessage.setFlags(r_canMsg.flags);
                    canMessage.setData(r_canMsg.data);
                    lock (m_listeners)
                    {
                        foreach (ICANListener listener in m_listeners)
                        {
                            listener.handleMessage(canMessage);
                        }
                    }
                    if (this.MessageContainsInformationForRealtime(canMessage.getID()))
                    {
                        CastInformationEvent(canMessage);
                    }
                }
                else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// The open method tries to connect to both busses to see if one of them is connected and
        /// active. The first strategy is to listen for any CAN message. If this fails there is a
        /// check to see if the application is started after an interrupted flash session. This is
        /// done by sending a message to set address and length (only for P-bus).
        /// </summary>
        /// <returns>OpenResult.OK is returned on success. Otherwise OpenResult.OpenError is
        /// returned.</returns>
        override public OpenResult open()
        {
            Console.WriteLine("Opening Easysync device");
            //EASYSYNC.CANMsg msg = new EASYSYNC.CANMsg();
            //Check if I bus is connected
            close();
            /*m_deviceHandle = EASYSYNC.canusb_Open(IntPtr.Zero,
                "0xcb:0x9a",                        // Slow
                EASYSYNC.CANUSB_ACCEPTANCE_CODE_ALL,
                EASYSYNC.CANUSB_ACCEPTANCE_MASK_ALL,
                EASYSYNC.CANUSB_FLAG_TIMESTAMP);
            if (waitAnyMessage(1000, out msg) != 0)
            {
                if (m_readThread.ThreadState == ThreadState.Unstarted)
                    m_readThread.Start();
                return OpenResult.OK;
            }
            close();
            */
            //I bus wasn't connected.
            //Check if P bus is connected
            Console.WriteLine("Calling canusb_Open");
            m_deviceHandle = EASYSYNC.canusb_Open(/*IntPtr.Zero,*/ null,
            EASYSYNC.CAN_BAUD_500K,              //500Kb/s
            null,//EASYSYNC.CANUSB_ACCEPTANCE_CODE_ALL,
            null,//EASYSYNC.CANUSB_ACCEPTANCE_MASK_ALL,
            EASYSYNC.CANUSB_FLAG_TIMESTAMP);
            Console.WriteLine("Device handle: " + m_deviceHandle.ToString());
            if (m_deviceHandle == 4294967295) return OpenResult.OpenError; // no valid handle
            if (boxIsThere())
            {
                Console.WriteLine("Box is there");
                if (m_readThread.ThreadState == ThreadState.Unstarted)
                    m_readThread.Start();
                return OpenResult.OK;
            }
            Console.WriteLine("Box is not there");
            close();
            return OpenResult.OpenError;
        }

        /// <summary>
        /// The close method closes the CANUSB device.
        /// </summary>
        /// <returns>CloseResult.OK on success, otherwise CloseResult.CloseError.</returns>
        override public CloseResult close()
        {
            int res = 0;
            try
            {
                res = EASYSYNC.canusb_Close(m_deviceHandle);
            }
            catch(DllNotFoundException e)
            {
                Console.WriteLine("EASYSYNC.canusb_Close: " + e.Message);
                return CloseResult.CloseError;
            }

            m_deviceHandle = 0;
            if (EASYSYNC.ERROR_CANUSB_OK == res)
            {
                return CloseResult.OK;
            }
            else
            {
                return CloseResult.CloseError;
            }
        }

        /// <summary>
        /// isOpen checks if the device is open.
        /// </summary>
        /// <returns>true if the device is open, otherwise false.</returns>
        override public bool isOpen()
        {
            if (m_deviceHandle > 0)
                return true;
            else
                return false;
        }

        /*private void AddToCanTrace(string line)
        {
            if (m_EnableCanLog)
            {
                DateTime dtnow = DateTime.Now;
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\CanTraceEasySyncUSBDevice.txt", true))
                {
                    sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
                }
            }
        }*/
        /// <summary>
        /// sendMessage send a CANMessage.
        /// </summary>
        /// <param name="a_message">A CANMessage.</param>
        /// <returns>true on success, othewise false.</returns>
        override public bool sendMessage(CANMessage a_message)
        {
            EASYSYNC.CANMsg msg = new EASYSYNC.CANMsg();
            msg.id = a_message.getID();
            msg.len = a_message.getLength();
            msg.flags = a_message.getFlags();
            msg.data = a_message.getData();
            int writeResult;
            AddToCanTrace("Sending message");
            writeResult = EASYSYNC.canusb_Write(m_deviceHandle, ref msg);
            if (writeResult == EASYSYNC.ERROR_CANUSB_OK)
            {
                AddToCanTrace("Message sent successfully");
                return true;
            }
            else
            {
                switch (writeResult)
                {
                    case EASYSYNC.ERROR_CANUSB_COMMAND_SUBSYSTEM:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_COMMAND_SUBSYSTEM");
                        break;
                    case EASYSYNC.ERROR_CANUSB_INVALID_PARAM:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_INVALID_PARAM");
                        break;
                    case EASYSYNC.ERROR_CANUSB_NO_MESSAGE:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_NO_MESSAGE");
                        break;
                    case EASYSYNC.ERROR_CANUSB_NOT_OPEN:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_NOT_OPEN");
                        break;
                    case EASYSYNC.ERROR_CANUSB_OPEN_SUBSYSTEM:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_OPEN_SUBSYSTEM");
                        break;
                    case EASYSYNC.ERROR_CANUSB_TX_FIFO_FULL:
                        AddToCanTrace("Message failed to send: ERROR_CANUSB_TX_FIFO_FULL");
                        break;
                    default:
                        AddToCanTrace("Message failed to send: " + writeResult.ToString());
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// waitForMessage waits for a specific CAN message give by a CAN id.
        /// </summary>
        /// <param name="a_canID">The CAN id to listen for</param>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message with a_canID that we where listening for.</param>
        /// <returns>The CAN id for the message we where listening for, otherwise 0.</returns>
        public override uint waitForMessage(uint a_canID, uint timeout, out CANMessage canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            canMsg = new CANMessage();
            while (nrOfWait < timeout)
            {
                EASYSYNC.CANMsg r_canMsg = new EASYSYNC.CANMsg();
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == EASYSYNC.ERROR_CANUSB_OK)
                {
                    if (r_canMsg.id == a_canID)
                    {
                        canMsg.setID(r_canMsg.id);
                        canMsg.setData(r_canMsg.data);
                        canMsg.setFlags(r_canMsg.flags);
                        return (uint)r_canMsg.id;
                    }
                }
                Thread.Sleep(1);
                nrOfWait++;
            }
            return 0;
        }
       

        /// <summary>
        /// waitForMessage waits for a specific CAN message give by a CAN id.
        /// </summary>
        /// <param name="a_canID">The CAN id to listen for</param>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message with a_canID that we where listening for.</param>
        /// <returns>The CAN id for the message we where listening for, otherwise 0.</returns>
        private uint waitForMessage(uint a_canID, uint timeout, out EASYSYNC.CANMsg r_canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == EASYSYNC.ERROR_CANUSB_OK)
                {
                    if (r_canMsg.id == 0x00)
                    {
                        nrOfWait++;
                    }
                    else if (r_canMsg.id != a_canID)
                        continue;
                    return (uint)r_canMsg.id; 
                }
                else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            r_canMsg = new EASYSYNC.CANMsg(); 
            return 0;
        }

        /// <summary>
        /// waitAnyMessage waits for any message to be received.
        /// </summary>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message that was first received</param>
        /// <returns>The CAN id for the message received, otherwise 0.</returns>
        private uint waitAnyMessage(uint timeout, out EASYSYNC.CANMsg r_canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == EASYSYNC.ERROR_CANUSB_OK)
                {
                    return (uint)r_canMsg.id;
                }
                else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            r_canMsg = new EASYSYNC.CANMsg();
            return 0;
        }

        /// <summary>
        /// Check if there is connection with a CAN bus.
        /// </summary>
        /// <returns>true on connection, otherwise false</returns>
        private bool boxIsThere()
        {
            EASYSYNC.CANMsg msg = new EASYSYNC.CANMsg();
            if (waitAnyMessage(2000, out msg) != 0)
            {
                Console.WriteLine("Seen a message");
                return true;
            }
            Console.WriteLine("Sending session request");
            if (sendSessionRequest())
            {
                Console.WriteLine("Session request issued");
                return true;
            }
            Console.WriteLine("No box detected");

            return false;
        }

        /// <summary>
        /// Send a message that starts a session. This is used to test if there is 
        /// a connection.
        /// </summary>
        /// <returns></returns>
        private bool sendSessionRequest()
        {
            CANMessage msg1 = new CANMessage(0x220, 0, 8);
            EASYSYNC.CANMsg msg = new EASYSYNC.CANMsg();
            msg1.setData(0x000040021100813f);

            if (!sendMessage(msg1))
                return false;
            if (waitForMessage(0x238, 1000, out msg) == 0x238)
            {
                //Ok, there seems to be a ECU somewhere out there.
                //Now, sleep for 10 seconds to get a session timeout. This is needed for
                //applications on higher level. Otherwise there will be no reply when the
                //higher level application tries to start a session.
                Thread.Sleep(10000);
                return true;
            }
            return false;
        }

       
    }


           
}
