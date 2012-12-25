using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using T7.CAN;

namespace T7.KWP
{
    /// <summary>
    /// KWPCANListener is used by the KWPCANDevice for listening for CAN messages.
    /// </summary>
    class KWPCANListener : ICANListener
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

       // private bool _logData = false;
        public CANMessage waitMessage(int a_timeout)
        {          
            CANMessage retMsg;
           // _logData = true;
            m_resetEvent.WaitOne(a_timeout, true);
            lock (m_canMessage)
            {
                retMsg = m_canMessage;
            }
           // _logData = false;
            return retMsg;
        }

/*        public CANMessage waitForMessage(uint a_canID, int a_timeout)
        {
            CANMessage retMsg;
            m_canMessage.setID(0);  // init so we cannot receive the same frame twice <GS-10022010>
            lock (m_canMessage)
            {
                m_waitMsgID = a_canID;
            }
            m_resetEvent.WaitOne(a_timeout, true);
            lock (m_canMessage)
            {
                retMsg = m_canMessage;
            }

            return retMsg;
        }
*/
        override public void handleMessage(CANMessage a_message)
        {
            lock (m_canMessage)
            {
                /*if (_logData)
                {
                    Console.WriteLine("KWPListener: " + a_message.getID().ToString("X4") + " " + a_message.getData().ToString("X16"));
                }*/
                if (a_message.getID() == m_waitMsgID)
                {
                    m_canMessage.setData(a_message.getData());
                    m_canMessage.setFlags(a_message.getFlags());
                    m_canMessage.setID(a_message.getID());
                    m_canMessage.setLength(a_message.getLength());
                    m_canMessage.setTimeStamp(a_message.getTimeStamp());
                    m_resetEvent.Set();
                }
            }
        }
    }
}
