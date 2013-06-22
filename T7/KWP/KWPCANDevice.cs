using System;
using System.Collections.Generic;
using System.Text;
using T7.CAN;
using System.IO;

namespace T7.KWP
{
    /// <summary>
    /// KWPCANDevice implements KWP2000 messages (KWP) over CAN for Trionic 7 (it might work
    /// with other ECUs as well).
    /// A KWP request consists of a 3-4 byte header and data: [length, Mode, PID, data] or
    /// [length, Mode, PIDHigh, PIDLow, data]. The first byte, length, represents the length of
    /// [Mode, PID(s), data]. Mode is a KWP "function" and the PIDs represents function parameters
    /// that modifies the behaviour of the function. The data can be "any" number of bytes
    /// and could for example represent an address.
    /// KWP request that are so long that they wont fit in a CAN message are divided into several
    /// CAN messages - called rows.
    /// 
    /// The response to a KWP request is a KWP reply. They have the same format as the request:
    /// [length, Mode, PID(s), data]. The length is the number of bytes in the reply exluding the
    /// length byte. The Mode and PID(s) are copied from the request. The data is the answer 
    /// from the KWP request.
    /// 
    /// When the KWP messages are divided into CAN messages there is a CAN header added to the data.
    /// For a request [0x4n,0xA1] is added where n represents the number of rows to send. For
    /// a reply [0xCn,0xBF] or [0x8n,0xBF] is added. The first variant is added for the first
    /// reply and the second for the rest of the replys. n means the row number.
    /// 
    /// Each reply row needs to be acknowledged by a CAN message befor the ECU will send 
    /// the next reply row. The format of the acknowledgement message is [0x40,0xA1,0x3F,0x8n]
    /// where n is the reply row number to acknowledge.
    /// 
    /// For this implementation the request rows are sent with CAN ID 0x240. Reply rows are received
    /// with CAN ID 0x258. Acknowledgement rows are sent with CAN ID 0x266.
    /// 
    /// 
    /// Example of a KWP request asking for the VIN:
    /// 
    /// 240h [40 A1 02 1A 90 00 00 00]
    /// 258h [C3 BF 13 5A 90 59 53 33] YS3
    /// 266h [40 A1 3F 83 00 00 00 00]
    /// 258h [82 BF 45 46 35 38 43 39] EF58C9
    /// 266h [40 A1 3F 82 00 00 00 00]
    /// 258h [81 BF 59 31 32 33 34 35] Y12345
    /// 266h [40 A1 3F 81 00 00 00 00]
    /// 258h [80 BF 36 37 00 00 00 00] 67
    /// 266h [40 A1 3F 80 00 00 00 00]
    /// 
    /// First line: The KWP request is [02,1A,90] (length=2, Mode=1A, PID=90). The KWP request is 
    /// wrapped in a CAN message with the header [40,A1] (40=row 0). The message is sent with
    /// CAN ID x240.
    /// 
    /// Second line: The request results in a multi row reply and this is the first row.
    /// [13,5A,90] is the KWP reply header (13=length, 5A=request Mode+0x40, 90=PID]. [C3,BF] is the 
    /// CAN header for the wrapped KWP reply (C3=first message, fourth row (3+1)). [59,53,33]
    /// is the start of the result (VIN).
    /// The message is sent from the ECU with CAN ID 0x258.
    /// 
    /// Third line: Acknowledgement of the second line. [40,A1,3F,83] (3 is the row number).
    /// The message is sent with CAN ID 0x266.
    /// 
    /// KWPCANDevice works with any CAN adapter/device that implements the ICANDevice interface.
    /// This is done to make it very easy to use this KWP over CAN implementation on any 
    /// CAN interface.
    /// 
    /// </summary>
    class KWPCANDevice : IKWPDevice
    {
        private Object m_lockObject = new Object();
        private ICANDevice m_canDevice;
        KWPCANListener m_kwpCanListener = new KWPCANListener();
        const int timeoutPeriod = 1000; // if timeout <GS-11022010> changed from 1000 to 250 to not intefere with the keepalive timer

        private bool m_EnableCanLog = false;

        public override bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set { m_EnableCanLog = value; }
        }

        /// <summary>
        /// Set the CAN device to be used by this class.
        /// </summary>
        /// <param name="a_canDevice">A ICANDevice.</param>
        public override void setCANDevice(ICANDevice a_canDevice)
        {
            if (m_canDevice == null)
            {
                lock (m_lockObject)
                {
                    Console.WriteLine("******* KWPCANDevice: m_CanDevice set");

                    m_canDevice = a_canDevice;
                }
            }
            else
            {
                Console.WriteLine("KWPCANDevice, candevice was already set");
            }
        }

        /// <summary>
        /// Open the CAN device.
        /// </summary>
        /// <returns>True if the device was opened, otherwise false.</returns>
        public override bool open()
        {
            Console.WriteLine("******* KWPCANDevice: Opening KWPCANDevice");

            bool retVal = false;
            Console.WriteLine("Opening m_canDevice");
            lock (m_lockObject)
            {
                Console.WriteLine("Lock passed: Opening m_canDevice");
                if (m_canDevice.open() == OpenResult.OK)
                {
                    Console.WriteLine("Adding listener");
                    m_canDevice.addListener(m_kwpCanListener);
                    retVal = true;
                }
                else
                    retVal = false;
            }
            Console.WriteLine("return value = " + retVal.ToString());
            return retVal;
        }

        /// <summary>
        /// Check if the CAN device is opened.
        /// </summary>
        /// <returns>True if the device is open, otherwise false.</returns>
        public override bool isOpen()
        {
            bool retVal = false;
            //lock (m_lockObject)
            {
                if (m_canDevice.isOpen())
                    retVal = true;
                else
                    retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Close the CAN device.
        /// </summary>
        /// <returns>True if the device was closed, otherwise false.</returns>
        public override bool close()
        {
            Console.WriteLine("******* KWPCANDevice: Closing KWPCANDevice");

            bool retVal = false;
            lock (m_lockObject)
            {
                if (m_canDevice.close() == CloseResult.OK)
                    retVal = true;
                else
                    retVal = false;
                m_canDevice.removeListener(m_kwpCanListener);
            }
            return retVal;
        }
        private void AddToCanTrace(string line)
        {
            Console.WriteLine("KWPCANDevice: " + line);
            DateTime dtnow = DateTime.Now;
            if (m_EnableCanLog)
            {
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\CanTraceKWPCANDevice.txt", true))
                {
                    sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
                }
            }
        }


        /// <summary>
        /// Start a KWP session.
        /// </summary>
        /// <remarks>
        /// A KWP session must be started before any requests can be sent.
        /// </remarks>
        /// <returns>True if the session was started, otherwise false.</returns>
        public override bool startSession()
        {
            CANMessage msg = new CANMessage(0x220, 0, 8);
            msg.setData(0x000040021100813F);
            AddToCanTrace("Sending 0x000040021100813F message");

            m_kwpCanListener.setupWaitMessage(0x238);
            
            if (!m_canDevice.sendMessage(msg))
            {
                AddToCanTrace("Unable to send 0x000040021100813F message");
                return false;
            }
            Console.WriteLine("Init msg sent");
            if (m_kwpCanListener.waitMessage(timeoutPeriod).getID() == 0x238)
            {
                AddToCanTrace("Successfully sent 0x000040021100813F message and received reply 0x238");
                return true;
            }
            else
            {
                AddToCanTrace("Didn't receive 0x238 message as reply on 0x000040021100813F message");
                return false;
            }

/*          if (!m_canDevice.sendMessage(msg)) 
            {
                AddToCanTrace("Unable to send 0x000040021100813F message");
                return false;
            }
            Console.WriteLine("Init msg sent");
            if (m_kwpCanListener.waitForMessage(0x238, timeoutPeriod).getID() == 0x238)
            {
                AddToCanTrace("Successfully sent 0x000040021100813F message and received reply 0x238");
                return true;
            }
            else
            {
                AddToCanTrace("Didn't receive 0x238 message as reply on 0x000040021100813F message");
                return false;
            }
*/
        }

        /// <summary>
        /// Send a KWP request.
        /// </summary>
        /// <param name="a_request">A KWP request.</param>
        /// <param name="r_reply">A KWP reply.</param>
        /// <returns>The status of the request.</returns>
        public override RequestResult sendRequest(KWPRequest a_request, out KWPReply r_reply)
        {
            CANMessage msg = new CANMessage(0x240, 0, 8);
            uint row = nrOfRowsToSend(a_request.getData());

            m_kwpCanListener.setupWaitMessage(0x258);

            // Send one or several request messages.
            for (; row > 0; row--)
            {
                msg.setData(createCanMessage(a_request.getData(), row - 1));
                if (!m_canDevice.sendMessage(msg))
                {
                    r_reply = new KWPReply();
                    return RequestResult.ErrorSending;
                }
            }

            msg = m_kwpCanListener.waitMessage(timeoutPeriod);          
 //         msg = m_kwpCanListener.waitForMessage(0x258, timeoutPeriod);    
            
            // Receive one or several replys and send an ack for each reply.
            if (msg.getID() == 0x258)
            {
                uint nrOfRows = (uint)(msg.getCanData(0) & 0x3F)+ 1;
                row = 0;
                if (nrOfRows == 0)
                    throw new Exception("Wrong nr of rows");
                //Assume that no KWP reply contains more than 0x200 bytes
                byte[] reply = new byte[0x200];
                reply = collectReply(reply, msg.getData(), row);
                sendAck(nrOfRows - 1);
                nrOfRows--;

                m_kwpCanListener.setupWaitMessage(0x258);

                while (nrOfRows > 0)
                {
//                    msg = m_kwpCanListener.waitForMessage(0x258, timeoutPeriod);
                    msg = m_kwpCanListener.waitMessage(timeoutPeriod);
                    if (msg.getID() == 0x258)
                    {
                        row++;
                        reply = collectReply(reply, msg.getData(), row);
                        sendAck(nrOfRows - 1);
                        nrOfRows--;
                    }
                    else
                    {
                        r_reply = new KWPReply();
                        return RequestResult.Timeout;
                    }

                }
                r_reply = new KWPReply(reply, a_request.getNrOfPID());
                return RequestResult.NoError;
            }
            else
            {
                r_reply = new KWPReply();
                return RequestResult.Timeout;
            }
        }

        /// <summary>
        /// Calculates the number of CAN messages to send for a KWP request.
        /// </summary>
        /// <param name="a_data">A byte array representing the KWP request.</param>
        /// <returns>The number of CAN messages (rows) to divide the request into.</returns>
        private uint nrOfRowsToSend(byte[] a_data)
        {
            return (uint)(a_data[0] / 6) + 1;
        }

        /// <summary>
        /// Get the number of rows to read in a reply.
        /// </summary>
        /// <remarks>
        /// The number of rows is given in the first byte of the CAN message.
        /// </remarks>
        /// <param name="a_canMsg">Represents a 8 byte CAN message.</param>
        /// <returns>The number of rows in the reply.</returns>
        private uint nrOfRowsToRead(ulong a_canMsg)
        {
            uint nrOfRows = 0;
            nrOfRows = (uint)a_canMsg & 0x0F;
            nrOfRows++;
            return nrOfRows;
        }

        /// <summary>
        /// Create a CAN message for a KWP request.
        /// </summary>
        /// <remarks>
        /// A KWP request may result in multi row CAN messages. This method
        /// creates one CAN message for a row [0..[
        /// </remarks>
        /// <param name="a_data">The data to insert into the CAN message.</param>
        /// <param name="a_row">The row.</param>
        /// <returns>A CAN message represented by a ulong.</returns>
        private ulong createCanMessage(byte[] a_data, uint a_row)
        {
            if (a_row > nrOfRowsToSend(a_data))
                throw new Exception("Message nr out of index");
            ulong result = 0;
            uint i = 0;
            if(nrOfRowsToSend(a_data) - a_row - 1 == 0)
                result = setCanData(result, (byte)(0x40 | a_row), i++);
            else
                result = setCanData(result, (byte)a_row, i++);
            result = setCanData(result, (byte)0xA1, i++);
            uint j;
            if (nrOfRowsToSend(a_data) - a_row - 1 == 0)
                j = 0;
            else
                j = ((nrOfRowsToSend(a_data) - a_row  - 1)* 6);
            int nrOfBytesToCopy;
            if (a_data.Length - j < 6)
                nrOfBytesToCopy = (int)(a_data.Length - j);
            else
                nrOfBytesToCopy = 6;
            for (int k = 0; k < nrOfBytesToCopy; i++, j++, k++)
                result = setCanData(result, (byte)a_data[j], i);
            return result;
        }

        /// <summary>
        /// Set a byte in a CAN message.
        /// </summary>
        /// <param name="a_canData">Represents the 8 bytes of a CAN message.</param>
        /// <param name="a_byte">The byte to set in the CAN message.</param>
        /// <param name="a_index">Index position to set the byte [0..7].</param>
        /// <returns>A CAN message with a_byte set in position a_index.</returns>
        private ulong setCanData(ulong a_canData, byte a_byte, uint a_index)
        {
            if (a_index > 7)
                throw new Exception("Index out of range");
            ulong tmp = (ulong)a_byte;
            tmp = tmp << (int)(a_index * 8);
            return a_canData | tmp;
        }

        /// <summary>
        /// Get a byte from a CAN message.
        /// </summary>
        /// <param name="a_canmsg">Represents the 8 bytes of a CAN message.</param>
        /// <param name="a_nr">The byte [0..7] to get.</param>
        /// <returns>The byte from index a_nt in a_canmsg.</returns>
        private byte getCanData(ulong a_canmsg, uint a_nr)
        {
            return (byte)(a_canmsg >> (int)(a_nr * 8));
        }


        /// <summary>
        /// Collect CAN message into a byte array.
        /// </summary>
        /// <param name="a_data">The array the data should be put into.</param>
        /// <param name="a_canMsg">The CAN message to take the data from.</param>
        /// <param name="a_row">The KWP row number.</param>
        /// <returns>The data from a_canMsg is inserted into a_data and returned.</returns>
        private byte[] collectReply(byte[] a_data, ulong a_canMsg, uint a_row)
        {
            uint j = a_row * 6;
            for (uint i = 2; i < 8; i++, j++)
                a_data[j] = getCanData(a_canMsg, i);
            return a_data;
        }

        /// <summary>
        /// Send an acknowledgement message.
        /// </summary>
        /// <param name="a_rowNr">The row number that should be acknowledged.</param>
        private void sendAck(uint a_rowNr)
        {
            CANMessage msg = new CANMessage(0x266,0,8);
            uint i = 0;
            ulong data = 0;
            data = setCanData(data, (byte)0x40, i++);
            data = setCanData(data, (byte)0xA1, i++);
            data = setCanData(data, (byte)0x3F, i++);
            data = setCanData(data, (byte)(0x80 | (int)(a_rowNr)), i++);
            msg.setData(data);
            if (!m_canDevice.sendMessage(msg))
                Console.WriteLine("Error sending ack");

        }
    }
}
