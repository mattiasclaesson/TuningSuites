using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace T5CANLib.CAN
{
    /// <summary>
    /// CANListener is used by the CANDevice for listening for CAN messages.
    /// </summary>
    class CANListener : ICANListener
    {
        private CANMessage m_canMessage = new CANMessage();
        private uint m_waitMsgID = 0;
        private AutoResetEvent m_resetEvent = new AutoResetEvent(false);

        //---------------------------------------------------------------------
        /**
        */
        public void setupWaitMessage(uint can_id)
        {
            this.m_canMessage.setID(0);
            lock (this.m_canMessage)
            {
                this.m_waitMsgID = can_id;
            }
        }

        //---------------------------------------------------------------------
        /**
        */
        public CANMessage waitMessage(int a_timeout)
        {
            CANMessage retMsg;

            m_resetEvent.WaitOne(a_timeout, true);
            lock (m_canMessage)
            {
                retMsg = m_canMessage;
            }

            return retMsg;
        }

/*      public CANMessage waitForMessage(uint a_canID, int a_timeout)
        {
            CANMessage retMsg;
            lock (m_canMessage)
            {
                m_waitMsgID = a_canID;
            }
            m_resetEvent.WaitOne(a_timeout, false);
            lock (m_canMessage)
            {
                retMsg = m_canMessage;
            }

            return retMsg;
        }
*/

        private void CheckRxMessage()
        {
            ulong data = m_canMessage.getData();
            if ((data & 0x00000000000000FF) == 0xA6)
            {
                // yes, write to designated file, 7 bytes at a time
                if (File.Exists(@"c:\dump.bin"))
                {
                    FileStream fs = new FileStream(@"c:\dump.bin", FileMode.Append);
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            byte b = (byte)((ulong)(data >> i * 8) & 0x00000000000000FF);
                            bw.Write(b);
                        }
                    }
                    fs.Close();
                    fs.Dispose();
                }
                else
                {
                    FileStream fs = new FileStream(@"c:\dump.bin", FileMode.Create);
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            byte b = (byte)((ulong)(data >> i * 8) & 0x00000000000000FF);
                            bw.Write(b);
                        }
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }

        }

        override public void handleMessage(CANMessage a_message)
        {
            bool messageReceived = false;
            lock (m_canMessage)
            {
                //Console.WriteLine("Received message in message handler");

                if (a_message.getID() == m_waitMsgID)
                {
                    m_canMessage = a_message;
                    messageReceived = true;
//                    CheckRxMsg();
                }
                // if it is a A6 command we're downloading the flash content as fast as possible.
                // just signal that the bytes we're received
                else
                {
//                    CheckRxMsg();
                }
            }
            if (messageReceived)
            {
//                CheckRxMessage();
                m_resetEvent.Set();
            }
        }
    }
}
