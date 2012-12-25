using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using T7.CAN;
using System.IO;
using System.IO.Ports;
namespace T7.KWP
{
    /// <summary>
    /// CANUSBDevice is an implementation of ICANDevice for the mitronics CANUSB device
    /// ([url]www.canusb.com[/url]).
    /// In this implementation the open method autmatically detects if the device is connected
    /// to a T7 I-bus or P-bus. The autodetection is primarily done by listening for the 0x280
    /// message (sent on both busses) but if the device is started after an interrupted flashing
    /// session there is no such message available on the bus. There fore the open method sends
    /// a message to set address and length for flashing. If there is a reply there is connection.
    ///
    /// All incomming messages are published to registered ICANListeners.
    /// </summary>
    class Mictronics : ICANDevice
    {

        SerialPort serialPort = new SerialPort();
        static uint m_deviceHandle = 0;
        Thread m_readThread;
        Object m_synchObject = new Object();
        bool m_endThread = false;
        private bool m_EnableCanLog = false;
        System.Collections.Queue canBusMessages = new System.Collections.Queue();

        public override float GetThermoValue()
        {
            return 0F;
        }

        public override float GetADCValue(uint channel)
        {
            return 0F;
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



        public override void Flush()
        {
            if (serialPort.IsOpen)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
            }
        }

        public bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set { m_EnableCanLog = value; }
        }
        /// <summary>
        /// Constructor for Mitronics.
        /// </summary>
        public Mictronics()
        {
            m_readThread = new Thread(readMessages);
        }

        /// <summary>
        /// Destructor for Mitronics.
        /// </summary>
        ~Mictronics()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            close();
        }

        public void DecodeCan232Data(string msg)
        {
            int i = 0;

            if (msg.Substring(0, 1) == "t" && msg.Length >= 21)
            {
                int len = Convert.ToByte(msg.Substring(4, 1));
                int id = Convert.ToInt32(msg.Substring(1, 3), 16);
                CANMessage buf = new CANMessage((uint)id, 0, (byte)len);
                if (len > 8) len = 8;
                for (i = 0; i <= len - 1; i++)
                {
                    buf.setCanData(Convert.ToByte(msg.Substring(5 + i * 2, 2), 16), (uint)i);
                }
                byte[] b = new byte[2];
                int p1 = 5 + 8 * 2;
                int p2 = 5 + 9 * 2;
                if (len > p1 && len > p2)
                {
                    b[0] = Convert.ToByte(msg.Substring(p1, 2), 16);
                    b[1] = Convert.ToByte(msg.Substring(p2, 2), 16);
                }
                buf.setTimeStamp(BitConverter.ToUInt16(b, 0));
                lock (canBusMessages)
                {
                    canBusMessages.Enqueue(buf);
                }
                /*lock (m_listeners)
                {
                    foreach (ICANListener listener in m_listeners)
                    {
                        listener.handleMessage(buf);
                    }
                }*/
            }
        }
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data1 = serialPort.ReadExisting();
            string[] dataLines = data1.Split('\r');
            foreach (string s in dataLines)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    //System.Diagnostics.Debug.WriteLine("RECV:" + s);
                    DecodeCan232Data(s);
                }
            }
        }

        static string Message(string s)
        {
            return s + "\r";
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

            serialPort.BaudRate = 57600;
            serialPort.PortName = "COM3";
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.ReadTimeout = serialPort.WriteTimeout = 500;
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.Open();

            // Reset mode

            serialPort.Write(Message("C"));

            // Set CAN BUS Speed
            serialPort.Write(Message("Z"));

            serialPort.Write(Message("sCB9A"));

            // Open CanBUS
            serialPort.Write(Message("O"));
            CANMessage msg;
            if (waitAnyMessage(1000, out msg) != 0)
            {
                if (m_readThread.ThreadState == ThreadState.Unstarted)
                    m_readThread.Start();
                return OpenResult.OK;
            }

            //I bus wasn't connected.
            //Check if P bus is connected
            serialPort.BaudRate = 500000;
            serialPort.PortName = "COM4"; //???
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.ReadTimeout = serialPort.WriteTimeout = 500;
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.Open();

            // Reset mode

            serialPort.Write(Message("C"));

            serialPort.Write(Message("s001C"));

            // Open CanBUS
            serialPort.Write(Message("O"));

            if (boxIsThere())
            {
                return OpenResult.OK;
            }

            serialPort.Close();
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
                serialPort.Close();
            }
            catch (DllNotFoundException e)
            {
                return CloseResult.CloseError;
            }

            m_deviceHandle = 0;
            return CloseResult.OK;

        }

        /// <summary>
        /// isOpen checks if the device is open.
        /// </summary>
        /// <returns>true if the device is open, otherwise false.</returns>
        override public bool isOpen()
        {
            if (serialPort.IsOpen)
                return true;
            else
                return false;
        }

        private void AddToCanTrace(string line)
        {
            if (m_EnableCanLog)
            {
                DateTime dtnow = DateTime.Now;
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\CanTraceCANUSBDevice.txt", true))
                {
                    sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
                }
            }
        }
        static public string CodeCan11Data(CANMessage msg)
        {
            string buf = "t";
            byte[] b = new byte[8];
            //b[0] = (byte)msg.getID();
            //b[1] = (byte)msg.getLength();
            for (uint i = 0; i < msg.getLength(); i++)
            {
                b[i] = msg.getCanData(i);
            }
            buf += String.Format("{0:X}", msg.getID()) + String.Format("{0:X}", msg.getLength());
            buf += BitConverter.ToString(b).Replace("-", string.Empty);
            return buf;
        }
        /// <summary>
        /// sendMessage send a CANMessage.
        /// </summary>
        /// <param name="a_message">A CANMessage.</param>
        /// <returns>true on success, othewise false.</returns>
        override public bool sendMessage(CANMessage a_message)
        {
            AddToCanTrace("Sending message");
            serialPort.Write(Message(CodeCan11Data(a_message)));

            AddToCanTrace("Message sent successfully Maybe");
            return true;
        }



        /// <summary>
        /// waitForMessage waits for a specific CAN message give by a CAN id.
        /// </summary>
        /// <param name="a_canID">The CAN id to listen for</param>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message with a_canID that we where listening for.</param>
        /// <returns>The CAN id for the message we where listening for, otherwise 0.</returns>
        public override uint waitForMessage(uint a_canID, uint timeout, out CANMessage canMessage)
        {
            int readResult = 0;
            int nrOfWait = 0;
            CANMessage tmpcanMessage = null;
            while (nrOfWait < timeout)
            {

                bool read = false;
                lock (canBusMessages)
                {
                    if (canBusMessages.Count > 0)
                    {
                        tmpcanMessage = (CANMessage)canBusMessages.Dequeue();
                        read = true;
                    }
                }
                if (read && tmpcanMessage != null)
                {
                    canMessage = tmpcanMessage;
                    if (canMessage.getID() == 0x00)
                    {
                        nrOfWait++;
                    }
                    else if (canMessage.getID() != a_canID)
                        continue;
                    return (uint)canMessage.getID();
                }
                else
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            canMessage = new CANMessage();
            return 0;
        }

        /// <summary>
        /// waitAnyMessage waits for any message to be received.
        /// </summary>
        /// <param name="timeout">Listen timeout</param>
        /// <param name="r_canMsg">The CAN message that was first received</param>
        /// <returns>The CAN id for the message received, otherwise 0.</returns>
        private uint waitAnyMessage(uint timeout, out CANMessage canMessage)
        {
            CANMessage tmpCanMessage = null;
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                bool read = false;
                lock (canBusMessages)
                {
                    if (canBusMessages.Count > 0)
                    {
                        tmpCanMessage = (CANMessage)canBusMessages.Dequeue();
                        read = true;
                    }
                }
                if (read && tmpCanMessage != null)
                {
                    canMessage = tmpCanMessage;
                    return (uint)canMessage.getID();
                }
                else
                {
                    Thread.Sleep(1);
                    nrOfWait++;
                }
            }
            canMessage = new CANMessage();
            return 0;
        }

        /// <summary>
        /// Check if there is connection with a CAN bus.
        /// </summary>
        /// <returns>true on connection, otherwise false</returns>
        private bool boxIsThere()
        {
            CANMessage msg;
            msg = new CANMessage();
            if (waitAnyMessage(2000, out msg) != 0)
                return true;
            if (sendSessionRequest())
                return true;

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
            CANMessage msgout = new CANMessage();
            msg1.setData(0x000040021100813f);

            if (!sendMessage(msg1))
                return false;
            if (waitForMessage(0x238, 1000, out msgout) == 0x238)
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

        /// <summary>
        /// readMessages is the "run" method of this class. It reads all incomming messages
        /// and publishes them to registered ICANListeners.
        /// </summary>
        public void readMessages()
        {
            int readResult = 0;
            CANMessage canMessage = new CANMessage();
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                        return;
                }
                bool read = false;
                lock (canBusMessages)
                {
                    if (canBusMessages.Count > 0)
                    {
                        canMessage = (CANMessage)canBusMessages.Dequeue();
                        read = true;
                    }
                }
                if (read)
                {
                    lock (m_listeners)
                    {
                        foreach (ICANListener listener in m_listeners)
                        {
                            listener.handleMessage(canMessage);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

    }



}