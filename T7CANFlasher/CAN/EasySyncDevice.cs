using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
//using System.Diagnostics;

namespace T5CANLib.CAN
{
    /// <summary>
    /// CANUSBDevice is an implementation of ICANDevice for the EasySync CANUSB device
    /// (www.canusb.com). 
    /// 
    /// All incomming messages are published to registered ICANListeners.
    /// </summary>
    public class EASYSYNCDevice : ICANDevice, IDisposable
    {

        static uint m_deviceHandle = 0;
        Thread m_readThread;
        Object m_synchObject = new Object();
        bool m_endThread = false;
        private bool m_DoLogging = false;
        private string m_startuppath = @"C:\Program files\Dilemma\CarPCControl";

        public string Startuppath
        {
            get { return m_startuppath; }
            set { m_startuppath = value; }
        }
        public bool DoLogging
        {
            get { return m_DoLogging; }
            set { m_DoLogging = value; }
        }
        /// <summary>
        /// Constructor for CANUSBDevice.
        /// </summary>
        public EASYSYNCDevice()
        {
            /*m_readThread = new Thread(readMessages);
            try
            {
                m_readThread.Priority = ThreadPriority.Normal; // realtime enough
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }*/
        }

        /// <summary>
        /// Destructor for CANUSBDevice.
        /// </summary>
        ~EASYSYNCDevice()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            close();
        }

        public enum FlushFlags : byte
        {
            FLUSH_WAIT = 0,
            FLUSH_DONTWAIT = 1,
            FLUSH_EMPTY_INQUEUE = 2
        }

        public override void Delete()
        {

        }

        public void Dispose()
        {

        }
        public override void setPortNumber(int portnumber)
        {
            //nothing to do
        }

        override public void EnableLogging(string path2log)
        {
            m_DoLogging = true;
            m_startuppath = path2log;
        }

        override public void DisableLogging()
        {
            m_DoLogging = false;
        }

        override public void clearReceiveBuffer()
        {
            EASYSYNC.canusb_Flush(m_deviceHandle);
        }

        override public void clearTransmitBuffer()
        {
            EASYSYNC.canusb_Flush(m_deviceHandle);
        }
        int thrdcnt = 0;
        /// <summary>
        /// readMessages is the "run" method of this class. It reads all incomming messages
        /// and publishes them to registered ICANListeners.
        /// </summary>
        public void readMessages()
        {
            int readResult = 0;
            EASYSYNC.CANMsg r_canMsg = new EASYSYNC.CANMsg();
            CANMessage canMessage = new CANMessage();

            /* Stopwatch
             while (true)
             {
                 lock (m_synchObject)
                 {
                     if (m_endThread)
                         return;
                 }
                 readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                 if (readResult > 0)
                 {
                     canMessage.setID(r_canMsg.id);
                     canMessage.setLength(r_canMsg.len);
                     canMessage.setTimeStamp(r_canMsg.timestamp);
                     canMessage.setFlags(r_canMsg.flags);
                     canMessage.setData(r_canMsg.data);
                     if (m_DoLogging)
                     {
                         DumpCanMsg(r_canMsg, false);
                     }
                     lock (m_listeners)
                     {
                         foreach (ICANListener listener in m_listeners)
                         {
                             listener.handleMessage(canMessage);
                         }
                     }
                 }
                 else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                 {
                     Thread.Sleep(1);
                 }
             }*/

            while (true)
            {
                /*if ((thrdcnt++ % 1000) == 0)
                {
                    Console.WriteLine("Reading messages");
                }*/
                lock (m_synchObject)
                {
                    if (m_endThread)
                    {
                        m_endThread = false;
                        return;
                    }
                }
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult > 0)
                {
                    canMessage.setID((uint)r_canMsg.id);
                    canMessage.setLength(r_canMsg.len);
                    canMessage.setTimeStamp((uint)r_canMsg.timestamp);
                    canMessage.setFlags(r_canMsg.flags);
                    //TODO: data hier nog vullen canMessage.setData(r_canMsg.data);
                    if (m_DoLogging)
                    {
                        DumpCanMsg(r_canMsg, false);
                    }
                    lock (m_listeners)
                    {
                        foreach (ICANListener listener in m_listeners)
                        {
                            listener.handleMessage(canMessage);
                        }
                    }
                }
                else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1); // changed to 0 to see performance impact
                }
            }
        }

        private string GetCharString(int value)
        {
            char c = Convert.ToChar(value);
            string charstr = c.ToString();
            if (c == 0x0d) charstr = "<CR>";
            else if (c == 0x0a) charstr = "<LF>";
            else if (c == 0x00) charstr = "<NULL>";
            else if (c == 0x01) charstr = "<SOH>";
            else if (c == 0x02) charstr = "<STX>";
            else if (c == 0x03) charstr = "<ETX>";
            else if (c == 0x04) charstr = "<EOT>";
            else if (c == 0x05) charstr = "<ENQ>";
            else if (c == 0x06) charstr = "<ACK>";
            else if (c == 0x07) charstr = "<BEL>";
            else if (c == 0x08) charstr = "<BS>";
            else if (c == 0x09) charstr = "<TAB>";
            else if (c == 0x0B) charstr = "<VT>";
            else if (c == 0x0C) charstr = "<FF>";
            else if (c == 0x0E) charstr = "<SO>";
            else if (c == 0x0F) charstr = "<SI>";
            else if (c == 0x10) charstr = "<DLE>";
            else if (c == 0x11) charstr = "<DC1>";
            else if (c == 0x12) charstr = "<DC2>";
            else if (c == 0x13) charstr = "<DC3>";
            else if (c == 0x14) charstr = "<DC4>";
            else if (c == 0x15) charstr = "<NACK>";
            else if (c == 0x16) charstr = "<SYN>";
            else if (c == 0x17) charstr = "<ETB>";
            else if (c == 0x18) charstr = "<CAN>";
            else if (c == 0x19) charstr = "<EM>";
            else if (c == 0x1A) charstr = "<SUB>";
            else if (c == 0x1B) charstr = "<ESC>";
            else if (c == 0x1C) charstr = "<FS>";
            else if (c == 0x1D) charstr = "<GS>";
            else if (c == 0x1E) charstr = "<RS>";
            else if (c == 0x1F) charstr = "<US>";
            else if (c == 0x7F) charstr = "<DEL>";
            return charstr;
        }

        private void DumpCanMsg(EASYSYNC.CANMsg r_canMsg, bool IsTransmit)
        {
            /*DateTime dt = DateTime.Now;
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(m_startuppath, dt.Year.ToString("D4") + dt.Month.ToString("D2") + dt.Day.ToString("D2") + "-CanTrace.log"), true))
                {
                    if (IsTransmit)
                    {
                        // get the byte transmitted
                        int transmitvalue = (int)(r_canMsg.data & 0x000000000000FF00);
                        transmitvalue /= 256;

                        sw.WriteLine(dt.ToString("dd/MM/yyyy HH:mm:ss") + " TX: id=" + r_canMsg.id.ToString("D2") + " len= " + r_canMsg.len.ToString("X8") + " data=" + r_canMsg.data.ToString("X16") + " " + r_canMsg.flags.ToString("X2") + " character = " + GetCharString(transmitvalue) + "\t ts: " + r_canMsg.timestamp.ToString("X16") + " flags: " + r_canMsg.flags.ToString("X2"));
                    }
                    else
                    {
                        // get the byte received
                        int receivevalue = (int)(r_canMsg.data & 0x0000000000FF0000);
                        receivevalue /= (256 * 256);
                        sw.WriteLine(dt.ToString("dd/MM/yyyy HH:mm:ss") + " RX: id=" + r_canMsg.id.ToString("D2") + " len= " + r_canMsg.len.ToString("X8") + " data=" + r_canMsg.data.ToString("X16") + " " + r_canMsg.flags.ToString("X2") + " character = " + GetCharString(receivevalue) + "\t ts: " + r_canMsg.timestamp.ToString("X16") + " flags: " + r_canMsg.flags.ToString("X2"));
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to write to logfile: " + E.Message);
            }*/
        }

        override public int GetNumberOfAdapters()
        {
            //char[] buff = new char[32];
            StringBuilder adapter = new StringBuilder(10);
            return EASYSYNC.canusb_getFirstAdapter(adapter, 32);
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
            if (m_deviceHandle != 0)
            {
                close();
            }
            // get first adapter
            //char[] adapter = new char[10];
            StringBuilder adapter = new StringBuilder(10);
            
            int result = EASYSYNC.canusb_getFirstAdapter(adapter, 10);
            if (result > 0)
            {
                Console.WriteLine("Number of adapters found: " + result.ToString("X8"));
                Console.WriteLine("Adapter serialnumber: " + adapter);
                m_deviceHandle = EASYSYNC.canusb_Open(null,
                    //"0x00:0xB9:0x07",
                    //"00:B9:07",
                    "00B907",
                    //"600",
                    //null,
                    null,//EASYSYNC.CANUSB_ACCEPTANCE_CODE_ALL,
                    null,//EASYSYNC.CANUSB_ACCEPTANCE_MASK_ALL,
                    EASYSYNC.CANUSB_FLAG_TIMESTAMP);
                Console.WriteLine("Handle: " + m_deviceHandle.ToString("X8"));
                if (m_deviceHandle > 0 && m_deviceHandle != 0xFFFFFFFD)
                {
                    EASYSYNC.canusb_Flush(m_deviceHandle);
                    Console.WriteLine("Creating new reader thread");

                    StringBuilder sb = new StringBuilder(100);
                    EASYSYNC.canusb_VersionInfo(m_deviceHandle, sb);
                    Console.WriteLine("Versionifo: " + sb.ToString());

                    Console.WriteLine("Status: " + EASYSYNC.canusb_Status(m_deviceHandle).ToString("X8"));

                    m_readThread = new Thread(readMessages);
                    try
                    {
                        m_readThread.Priority = ThreadPriority.Normal; // realtime enough
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    if (m_readThread.ThreadState == ThreadState.Unstarted)
                        m_readThread.Start();
                    return OpenResult.OK;
                }
                else
                {
                    // second try after unload of dlls?

                    //close();
                    return OpenResult.OpenError;

                }
            }
            return OpenResult.OpenError;
        }

        /// <summary>
        /// The close method closes the CANUSB device.
        /// </summary>
        /// <returns>CloseResult.OK on success, otherwise CloseResult.CloseError.</returns>
        override public CloseResult close()
        {
            Console.WriteLine("Close called in CANUSBDevice");
            if (m_readThread != null)
            {
                if (m_readThread.ThreadState != ThreadState.Stopped && m_readThread.ThreadState != ThreadState.StopRequested)
                {
                    lock (m_synchObject)
                    {
                        m_endThread = true;
                    }
                    // m_readThread.Abort();
                }
            }
            int res = EASYSYNC.canusb_Close(m_deviceHandle);
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

        private byte GetDataMSBADCII(byte total)
        {
            string temp = total.ToString("X2");
            return (byte)temp[0];
        }

        private byte GetDataLSBADCII(byte total)
        {
            string temp = total.ToString("X2");
            return (byte)temp[1];
        }


        /// <summary>
        /// sendMessage send a CANMessage.
        /// </summary>
        /// <param name="a_message">A CANMessage.</param>
        /// <returns>true on success, othewise false.</returns>
        override public bool sendMessage(CANMessage a_message)
        {
            EASYSYNC.CANMsg msg = new EASYSYNC.CANMsg();
            msg.id = (ushort)a_message.getID();
            msg.len = a_message.getLength();
            msg.flags = a_message.getFlags();
            ulong msgdata = a_message.getData();
            // store in data (ulong)
            /*byte databyte = a_message.getCanData(7);
            msg.data_1 = GetDataMSBADCII(databyte);
            msg.data_2 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(6);
            msg.data_3 = GetDataMSBADCII(databyte);
            msg.data_4 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(5);
            msg.data_5 = GetDataMSBADCII(databyte);
            msg.data_6 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(4);
            msg.data_7 = GetDataMSBADCII(databyte);
            msg.data_8 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(3);
            msg.data_9 = GetDataMSBADCII(databyte);
            msg.data_10 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(2);
            msg.data_11 = GetDataMSBADCII(databyte);
            msg.data_12 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(1);
            msg.data_13 = GetDataMSBADCII(databyte);
            msg.data_14 = GetDataLSBADCII(databyte);
            databyte = a_message.getCanData(0);
            msg.data_15 = GetDataMSBADCII(databyte);
            msg.data_16 = GetDataLSBADCII(databyte);
            */

            msg.data = a_message.getData(); // this data should be in ascii: unsigned char data[16];			// Databytes 0...7
            // example:
            /*
            msg.data[0]='A';
			msg.data[1]='1';

			msg.data[2]='B';
			msg.data[3]='2';

			msg.data[4]='C';
			msg.data[5]='3';

			msg.data[6]='D';
			msg.data[7]='4';

			msg.data[8]='E';
			msg.data[9]='5';

			msg.data[10]='F';
			msg.data[11]='6';

			msg.data[12]='1';
			msg.data[13]='2';

			msg.data[14]='3';
			msg.data[15]='4'; * */
            if (m_DoLogging)
            {
                DumpCanMsg(msg, true);
            }

            int writeResult;
            Console.WriteLine("Writing to handle: " + m_deviceHandle.ToString("X8"));
            writeResult = EASYSYNC.canusb_Write(m_deviceHandle, ref msg);

            if (writeResult == EASYSYNC.ERROR_CANUSB_OK)
                return true;
            else
            {
                //EASYSYNC.canusb_Flush(m_deviceHandle);
                Console.WriteLine("Failed to send message: " + writeResult.ToString("X8"));
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
        private uint waitForMessage(uint a_canID, uint timeout, out EASYSYNC.CANMsg r_canMsg)
        {
            int readResult = 0;
            int nrOfWait = 0;
            while (nrOfWait < timeout)
            {
                readResult = EASYSYNC.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == EASYSYNC.ERROR_CANUSB_OK)
                {
                    if (r_canMsg.id != a_canID)
                        continue;
                    return (uint)r_canMsg.id;
                }
                else if (readResult == EASYSYNC.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1); // changed to 0 to see performance impact
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
                    Thread.Sleep(1); // changed to 0 to see performance impact
                    nrOfWait++;
                }
            }
            r_canMsg = new EASYSYNC.CANMsg();
            return 0;
        }


    }



}
