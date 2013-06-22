using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace T7.KWP
{
    /// <summary>
    /// KWPResult represents the result returned by the request methods.
    /// </summary>
    public enum KWPResult
    {
        OK,
        NOK,
        Timeout,
        DeviceNotConnected
    }

    /// <summary>
    /// KWPHandler implements messages for the KWP2000 (Key Word Protocol 2000) protocol (also called
    /// ISO 14230-4). Not all messages are implemented.
    /// </summary>
    public class KWPHandler
    {
        /// <summary>
        /// IKWPDevice to be used by KWPHandler.
        /// </summary>
        private static IKWPDevice m_kwpDevice;
        private bool gotSequrityAccess = false;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a_kwpDevice">IKWPDevice to be used by KWPHandler.</param>
        public static void setKWPDevice(IKWPDevice a_kwpDevice)
        {
            Console.WriteLine("******* KWPHandler: KWP device set");
            if (m_kwpDevice == null)
            {
                m_kwpDevice = a_kwpDevice;
            }
            else
            {
                Console.WriteLine("KWPHandler KWPDevice was already set");
            }

        }

        public static KWPHandler getInstance()
        {
            if (m_kwpDevice == null)
                Console.WriteLine("KWPDevice not set.");
            if (m_instance == null)
                m_instance = new KWPHandler();
            return m_instance;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private KWPHandler()
        {
            timerDelegate = new TimerCallback(this.sendKeepAlive);
        }

        private bool m_suspendAlivePolling = false;

        public void SuspendAlivePolling()
        {
            m_suspendAlivePolling = true;
        }

        public void ResumeAlivePolling()
        {
            m_suspendAlivePolling = false;
        }
        
        /// <summary>
        /// TODO: we should ONLY send keep alives if theres been no data on the bus for
        /// at least x seconds... otherwise, skip sending the KA because for some reason it 
        /// distorts our realtime information
        /// </summary>
        /// <param name="stateInfo"></param>
        public void sendKeepAlive(Object stateInfo)
        {
            if (m_kwpDevice.isOpen() && !m_suspendAlivePolling)
            {
                sendUnknownRequest();
            }
          //  Console.WriteLine("Sending keep alive");
        }


        /// <summary>
        /// This method starts a KWP session. It must be called before any request can be made.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool startSession()
        {
            return m_kwpDevice.startSession();
        }

        /// <summary>
        /// This method opens the IKWPDevice used for communication.
        /// Device must be opened before any requests can be made.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool openDevice()
        {
            Console.WriteLine("******* KWPHandler: Opening kwpDevice");

            return m_kwpDevice.open();
        }

        /// <summary>
        /// This method closes the IKWPDevice used for communication.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool closeDevice()
        {
            Console.WriteLine("******* KWPHandler: Closing kwpDevice");

            if(m_kwpDevice != null)
                return m_kwpDevice.close();
            return false;
        }

        /// <summary>
        /// Send a request for sequrity access.
        /// This is needed to access protected functions (flash reading and writing).
        /// </summary>
        /// <returns>True on success, otherwise false.</returns>
        public bool requestSequrityAccess(bool ForceRequest)
        {
            if (gotSequrityAccess && !ForceRequest) return true;
            //Try method 1
            if (requestSequrityAccessLevel(1))
            {
                gotSequrityAccess = true;
                return true;
            }
            if (requestSequrityAccessLevel(2))
            {
                gotSequrityAccess = true;
                return true;
            }
            else
                return false;
        }

        private void LogEntry(string entry)
        {
            try
            {
                
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\kwplog.txt", true))
                {
                    sw.WriteLine(entry);
                    //Console.WriteLine(entry);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void LogDataString(string entry)
        {
            if (m_logginEnabled)
            {
                if (entry != "")
                {
                    LogEntry(DateTime.Now.ToString("HH:mm:ss:fff") + " - " + entry);
//                    m_logFileStream.WriteLine(DateTime.Now.ToString("HH:mm:ss:fff") + " - " + entry);
                }
                else
                {
                    //m_logFileStream.WriteLine();
                    LogEntry("");
                }
//                m_logFileStream.Flush();

            }
        }

       

        public bool ResetECU()
        {
            LogDataString("ResetECU");
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[1];
            data[0] = (byte)0x00;
            KWPRequest req = new KWPRequest(0x11, 0x01, data);
            Console.WriteLine(req.ToString());
            result = sendRequest(req, out reply);
            Console.WriteLine(reply.ToString());
            return true;
        }

        public bool ReadFreezeFrameData(uint frameNumber)
        {
            LogDataString("ReadFreezeFrameData");
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[1];
            data[0] = (byte)0x00;
            //data[1] = (byte)0x00;
            //data[2] = (byte)0x00;
            KWPRequest req = new KWPRequest(0x12, Convert.ToByte(frameNumber), data);
            Console.WriteLine(req.ToString());
            result = sendRequest(req, out reply);
            Console.WriteLine(reply.ToString());
            return true;
        }

        public bool ReadDTCCodes()
        {
            LogDataString("ReadDTCCodes");
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[2];
            data[0] = (byte)0xFF;
            data[1] = (byte)0xFF;
            //data[2] = (byte)0x00;
            KWPRequest req = new KWPRequest(0x18, 0x02/*, data*/);
            Console.WriteLine(req.ToString());
            result = sendRequest(req, out reply);
            Console.WriteLine(reply.ToString());
            return true;
        }

        public bool ClearDTCCode(int dtccode)
        {
            LogDataString("ClearDTCCode: " + dtccode.ToString("X4"));
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[1];
            data[0] = (byte)(dtccode & 0x00FF);
            //data[1] = (byte)0xFF;
            //data[2] = (byte)0x00;
            KWPRequest req = new KWPRequest(0x14, (byte)(dtccode >> 8), data);
            Console.WriteLine(req.ToString());
            result = sendRequest(req, out reply);
            Console.WriteLine(reply.ToString());
            return true;
        }

        public bool ClearDTCCodes()
        {
            LogDataString("ClearDTCCodes");
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] data = new byte[1];
            data[0] = (byte)0xFF;
            //data[1] = (byte)0xFF;
            //data[2] = (byte)0x00;
            KWPRequest req = new KWPRequest(0x14, 0xFF, data);
            Console.WriteLine(req.ToString());
            result = sendRequest(req, out reply);
            Console.WriteLine(reply.ToString());
            return true;
        }

        /// <summary>
        /// Send a request for a sequrity access with one out of two methods to 
        /// calculate the key.
        /// </summary>
        /// <param name="a_method">Key calculation method [1,2]</param>
        /// <returns>true if sequrity access was granted, otherwise false</returns>
        private bool requestSequrityAccessLevel(uint a_method)
        {
            LogDataString("requestSequrityAccessLevel: " + a_method.ToString());
            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] seed = new byte[2];
            byte[] key = new byte[2];
            // Send a seed request.
            KWPRequest requestForKey = new KWPRequest(0x27, 0x05);
            Console.WriteLine("requestSequrityAccessLevel " + a_method.ToString() +  " request for key: " + requestForKey.ToString());
            result = sendRequest(requestForKey, out reply);
            Console.WriteLine("requestSequrityAccessLevel " + a_method.ToString() + " request for key result: " + reply.ToString());

            if (result != KWPResult.OK)
                return false;
            if (reply.getData().Length < 2)
                return false;
            seed[0] = reply.getData()[0];
            seed[1] = reply.getData()[1];
            if (a_method == 1)
                key = calculateKey(seed, 0);
            else
                key = calculateKey(seed, 1);
            // Send key reply.
            KWPRequest sendKeyRequest = new KWPRequest(0x27, 0x06, key);
            Console.WriteLine("requestSequrityAccessLevel " + a_method.ToString() +  " send Key request: " + sendKeyRequest.ToString());
            result = sendRequest(sendKeyRequest, out reply);
            Console.WriteLine("requestSequrityAccessLevel " + a_method.ToString() + " send Key reply: " + reply.ToString());
            if (result != KWPResult.OK)
            {
                Console.WriteLine("Security access request was not send");
                return false;
            }

            //Check if sequrity was granted.
            Console.WriteLine("Mode: " + reply.getMode().ToString("X2"));
            Console.WriteLine("Data: " + reply.getData()[0].ToString("X2"));
            if ((reply.getMode() == 0x67) && (reply.getData()[0] == 0x34)) // WAS [0]
            {
                Console.WriteLine("Security access granted: " + a_method.ToString());
                return true;
            }

            Console.WriteLine("Security access was not granted: " + reply.ToString());
            return false;
        }

        /// <summary>
        /// This method sends a request for the VIN (Vehicle ID Number).
        /// </summary>
        /// <param name="r_vin">The requested VIN.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getVIN(out string r_vin)
        {
            LogDataString("getVIN");

            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x1A,0x90), out reply);
            if (result == KWPResult.OK)
            {
                r_vin = getString(reply);
                return KWPResult.OK;
            }
            else
            {
                r_vin = "";
                return KWPResult.Timeout;
            }
        }

        /// <summary>
        /// This method sends a request for the immobilizer ID.
        /// </summary>
        /// <param name="r_immo">The requested immo ID.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getImmo(out string r_immo)
        {
            LogDataString("getImmo");

            KWPReply reply = new KWPReply();
            KWPResult result;
            r_immo = "";
            result = sendRequest(new KWPRequest(0x1A, 0x92), out reply);
            if (result != KWPResult.OK)
                return result;
            r_immo = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request to get the offset of the symbol table.
        /// </summary>
        /// <param name="r_immo">Start address of the symbol table.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSymbolTableOffset(out UInt16 r_offset)
        {
            LogDataString("getSymbolTableOffset");

            KWPReply reply = new KWPReply();
            KWPResult result;
            r_offset = 0;
            result = sendRequest(new KWPRequest(0x1A, 0x9B), out reply);
            if (result != KWPResult.OK)
                return result;
            r_offset = getUint16(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software part number.
        /// </summary>
        /// <param name="r_swPartNo">The requested sofware part number.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwPartNumber(out string r_swPartNo)
        {
            LogDataString("getSwPartNumber");

            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x1A, 0x94), out reply);
            r_swPartNo = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software version.
        /// </summary>
        /// <param name="r_swVersion">The requested software version.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwVersion(out string r_swVersion)
        {
            LogDataString("getSwVersion");

            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x1A, 0x95), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the software version using diagnostic routine 0x51.
        /// </summary>
        /// <param name="r_swVersion">The requested software version.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getSwVersionFromDR51(out string r_swVersion)
        {
            LogDataString("getSwVersionFromDR51");

            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x31, 0x51), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        /// <summary>
        /// This method sends a request for the engine type description.
        /// </summary>
        /// <param name="r_swVersion">The requested engine type description.</param>
        /// <returns>KWPResult</returns>
        public KWPResult getEngineType(out string r_swVersion)
        {
            LogDataString("getEngineType");

            KWPReply reply = new KWPReply();
            KWPResult result;
            r_swVersion = "";
            result = sendRequest(new KWPRequest(0x1A, 0x97), out reply);
            if (result != KWPResult.OK)
                return result;
            r_swVersion = getString(reply);
            return result;
        }

        public KWPResult sendEraseMemorySpaceRequest(Int32 StartAddress, Int32 Length)
        {
            /*02 FLASH memory erasure 
            1 start address (high byte)
            2 start address (middle byte)
            3 start address (low byte)
            4 stop address (high byte)
            5 stop address (middle byte)
            6 stop address (low byte)
            */
            Int32 StopAddress = StartAddress + Length;
            LogDataString("sendEraseRequest");

            KWPReply reply = new KWPReply();
            KWPReply reply2 = new KWPReply();
            KWPResult result = KWPResult.Timeout;
            int i = 0;

            //First erase message. Up to 5 retries.
            //Mode = 0x31
            //PID = 0x51
            //Expected result is 0x71
            byte[] a_data = new byte[6];
            int bt = 0;
            //a_data[0] = (byte)(StartAddress >> 24);
            a_data[bt++] = (byte)(StartAddress >> 16);
            a_data[bt++] = (byte)(StartAddress >> 8);
            a_data[bt++] = (byte)(StartAddress);
            //a_data[4] = (byte)(Length >> 24);
            a_data[bt++] = (byte)(StopAddress >> 16);
            a_data[bt++] = (byte)(StopAddress >> 8);
            a_data[bt++] = (byte)(StopAddress);

            requestSequrityAccess(true);
            KWPRequest req = new KWPRequest(0x31, 0x50, a_data);
            
            result = sendRequest(req, out reply);
            Console.WriteLine("Erase(1) " + reply.ToString());
            /*if (result != KWPResult.OK)
                return result;
            System.Threading.Thread.Sleep(10000);
            result = sendRequest(new KWPRequest(0x31, 0x53, a_data), out reply);
            Console.WriteLine("Erase(2:" + i.ToString() + ") " + reply.ToString());
            */
            if (result != KWPResult.OK)
                return result;

            result = sendRequest(new KWPRequest(0x3E, 0x50), out reply2); // tester present???
            Console.WriteLine("reply on exit " + reply2.ToString());

            return result;
        }

        /// <summary>
        /// sendEraseRequest sends an erase request to the ECU.
        /// This method must be called before the ECU can be flashed.
        /// </summary>
        /// <returns>KWPResult</returns>
        public KWPResult sendEraseRequest()
        {
            LogDataString("sendEraseRequest");

            KWPReply reply = new KWPReply();
            KWPReply reply2 = new KWPReply();
            KWPResult result = KWPResult.Timeout;
            int i = 0;

            //First erase message. Up to 5 retries.
            //Mode = 0x31
            //PID = 0x52
            //Expected result is 0x71
            result = sendRequest(new KWPRequest(0x31, 0x52), out reply);
            Console.WriteLine("Erase(1) " + reply.ToString());
            if (result != KWPResult.OK)
                return result;
            while (reply.getMode() != 0x71) 
            {
                System.Threading.Thread.Sleep(1000);
                result = sendRequest(new KWPRequest(0x31, 0x52), out reply);
                Console.WriteLine("Erase(2:" + i.ToString() + ") " + reply.ToString());
                if (i++ > 15) return KWPResult.Timeout;
            }
            if (result != KWPResult.OK) 
                return result;

            //Second erase message. Up to 10 retries.
            //Mode = 0x31
            //PID = 0x53
            //Expected result is 0x71
            i = 0;
            result = sendRequest(new KWPRequest(0x31, 0x53), out reply2);
            Console.WriteLine("Erase(3) " + reply2.ToString());
            if (result != KWPResult.OK)
                return result;
            while (reply2.getMode() != 0x71)
            {
                System.Threading.Thread.Sleep(1000);
                result = sendRequest(new KWPRequest(0x31, 0x53), out reply2);
                Console.WriteLine("Erase(4:" + i.ToString() + ") " + reply2.ToString());
                if (i++ > 20) return KWPResult.Timeout;
            }

            //Erase confirm message
            //Mode = 0x3E
            //Expected result is 0x7E
            result = sendRequest(new KWPRequest(0x3E, 0x53), out reply2);
            Console.WriteLine("Erase(5) " + reply2.ToString());

            return result;
        }

        /// <summary>
        /// This method sets up the address and length for writing to flash. It must be called before
        /// the sendWriteDataRequest method is called.
        /// </summary>
        /// <param name="a_address">The addres to start writing to.</param>
        /// <param name="a_length">The length to write</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendWriteRequest(uint a_address, uint a_length)
        {
            LogDataString("sendWriteRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] addressAndLength = new byte[7];
            //set address (byte 0 to 2)
            addressAndLength[0] = (byte)(a_address >> 16);
            addressAndLength[1] = (byte)(a_address >> 8);
            addressAndLength[2] = (byte)(a_address);
            //set length (byte 3 to 6);
            addressAndLength[3] = (byte)(a_length >> 24);
            addressAndLength[4] = (byte)(a_length >> 16);
            addressAndLength[5] = (byte)(a_length >> 8);
            addressAndLength[6] = (byte)(a_length);

            //Send request
            //Mode = 0x34
            //PID = no PID used by this request
            //Data = aaallll (aaa = address, llll = length)
            //Expected result = 0x74
            result = sendRequest(new KWPRequest(0x34, addressAndLength), out reply);
            if (result != KWPResult.OK)
                return result;
            if (reply.getMode() != 0x74)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send data to be written to flash. sendWriteRequest must be called before this method.
        /// </summary>
        /// <param name="a_data">The data to be written.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendWriteDataRequest(byte[] a_data)
        {
            LogDataString("sendWriteDataRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x36
            //PID = no PID used by this request
            //Data = data to be flashed
            //Expected result = 0x76
            result = sendRequest(new KWPRequest(0x36, a_data), out reply);
            if (reply.getMode() != 0x76)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method requests data to be transmitted.
        /// </summary>
        /// <param name="a_data">The data to be transmitted.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendDataTransferRequest(out byte[] a_data)
        {
            LogDataString("sendDataTransferRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;
          

            //Send request
            //Mode = 0x36
            //PID = no PID used by this request
            //Data = no data
            //Expected result = 0x76
            result = sendRequest(new KWPRequest(0x36), out reply);
            a_data = reply.getData();
            if (reply.getMode() != 0x76)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// Send unknown request
        /// </summary>
        /// <returns>KWPResult</returns>
        public KWPResult sendUnknownRequest()
        {
            LogDataString("sendUnknownRequest");
            //Console.WriteLine("sendUnknownRequest");
            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x3E
            //PID = no PID used by this request
            //Expected result = 0x7E
            result = sendRequest(new KWPRequest(0x3E), out reply);
            if (reply.getMode() != 0x7E)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send a request to download the symbol map.
        /// After this request has been made data can be fetched with a data transfer request
        /// </summary>
        /// <param name="a_data">The data to be written.</param>
        /// <returns>KWPResult</returns>
        public KWPResult sendReadSymbolMapRequest()
        {
            LogDataString("sendReadSymbolMapRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;

            //Send request
            //Mode = 0x31
            //PID = 0x50
            //Data = data to be flashed
            //Expected result = 0x71
            result = sendRequest(new KWPRequest(0x31, 0x50), out reply);
            if (reply.getMode() != 0x71)
                return KWPResult.NOK;
            else
                return result;
        }

        /// <summary>
        /// This method send a request for reading from ECU memory (both RAM and flash). 
        /// It sets up start address and the length to read.
        /// </summary>
        /// <param name="a_address">The address to start reading from.</param>
        /// <param name="a_length">The total length to read.</param>
        /// <returns>true on success, otherwise false</returns>
        public bool sendReadRequest(uint a_address, uint a_length)
        {
            LogDataString("sendReadRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] lengthAndAddress = new byte[5];
            //set length (byte 0 and 1)
            lengthAndAddress[0] = (byte)(a_length >> 8);
            lengthAndAddress[1] = (byte)(a_length);
            //set address (byte 2 to 4);
            lengthAndAddress[2] = (byte)(a_address >> 16);
            lengthAndAddress[3] = (byte)(a_address >> 8);
            lengthAndAddress[4] = (byte)(a_address);
            result = sendRequest(new KWPRequest(0x2C, 0xF0, 0x03, lengthAndAddress), out reply);
            if (result == KWPResult.OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method send a request for reading a symbol. 
        /// </summary>
        /// <param name="a_address">The symbol number to read [0..0xFFFF-1].</param>
        /// <returns></returns>
        public bool setSymbolRequest(uint a_symbolNumber)
        {
            LogDataString("setSymbolRequest");

            KWPReply reply = new KWPReply();
            KWPResult result = KWPResult.Timeout;
            byte[] symbolNumber = new byte[5];
            //First two bytes should be zero
            symbolNumber[0] = 0;
            symbolNumber[1] = 0;
            symbolNumber[2] = 0x80;
            //set symbol number (byte 2 to 3);
            symbolNumber[3] = (byte)(a_symbolNumber >> 8);
            symbolNumber[4] = (byte)(a_symbolNumber);
            //<GS-11022010>
            // check result .. it should be 2 bytes long
            int _retryCount = 0;
            while (_retryCount++ < 3 && result != KWPResult.OK)
            {
                //result = sendRequest(new KWPRequest(0x2C, 0xF0, 0x03, symbolNumber), out reply);
                result = sendRequest(new KWPRequest(0x2C, 0xF0, 0x03, symbolNumber), out reply/*, 2*/);
                if (reply.getLength() != 2)
                {
                    result = KWPResult.Timeout;
                    LogDataString("Got wrong response on sendRequest in setSymbolRequest, len = " + reply.getLength().ToString("D2"));
                    Console.WriteLine("Got wrong response on sendRequest in setSymbolRequest, len = " + reply.getLength().ToString("D2"));
                }
            }
            if (result == KWPResult.OK)
            {
                return true;
            }
            else
            {
                LogDataString("setSymbolRequest timed out");
                Console.WriteLine("setSymbolRequest timed out");
                return false;
            }
        }

        /// <summary>
        /// This method writes to a symbol in RAM.
        /// The ECU must not be write protected for this to work.
        /// </summary>
        /// <param name="a_symbolNumber">Symbol number to write to.</param>
        /// <param name="a_data">Data to write.</param> 
        /// <returns></returns>
        public bool writeSymbolRequestAddress(uint a_address, byte[] a_data)
        {
            LogDataString("writeSymbolRequest: " + a_address.ToString("X8") + "len: " + a_data.Length.ToString("X4"));

            KWPReply reply = new KWPReply();
            KWPResult result;
            byte[] symbolNumberAndData = new byte[4 + a_data.Length];
            //symbolNumberAndData[0] = (byte)(a_address >> 24);
            symbolNumberAndData[0] = (byte)(a_address >> 16);
            symbolNumberAndData[1] = (byte)(a_address >> 8);
            symbolNumberAndData[2] = (byte)(a_address);
            symbolNumberAndData[3] = (byte)(a_data.Length);
            // len len 
            // adr adr adr

            for (int i = 0; i < a_data.Length; i++)
                symbolNumberAndData[i + 4] = a_data[i];

            string requestString = "RequestString: ";
            foreach (byte b in symbolNumberAndData)
            {
                requestString += b.ToString("X2") + " ";
            }
            Console.WriteLine(requestString);
            // end dump to console
            Console.WriteLine("SymbolNumberAndData length: " + symbolNumberAndData.Length.ToString("X8"));
            KWPRequest t_request = new KWPRequest(0x3D, /*0x81, */symbolNumberAndData);
            Console.WriteLine(t_request.ToString());
            result = sendRequest(t_request, out reply);
            if (result != KWPResult.OK)
            {
                Console.WriteLine("Result != KWPResult.OK");
                return false;
            }
            Console.WriteLine("Result = " + reply.getData()[0].ToString("X2"));
            Console.WriteLine("Result-total = " + reply.ToString());
            if (reply.getData()[0] == 0x7D)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method writes to a symbol in RAM.
        /// The ECU must not be write protected for this to work.
        /// </summary>
        /// <param name="a_symbolNumber">Symbol number to write to.</param>
        /// <param name="a_data">Data to write.</param> 
        /// <returns></returns>
        public bool writeSymbolRequest(uint a_symbolNumber, byte[] a_data)
        {
            LogDataString("writeSymbolRequest: " + a_symbolNumber.ToString());

            KWPReply reply = new KWPReply();
            KWPResult result;
            int LengthToTx = a_data.Length;
            if (LengthToTx > 0x41) LengthToTx = 0x41; // FA
            byte[] symbolNumberAndData = new byte[3 + /*a_data.Length*/ LengthToTx];
            //First two bytes should be the symbol number
            symbolNumberAndData[0] = (byte)(a_symbolNumber >> 8);
            symbolNumberAndData[1] = (byte)(a_symbolNumber);
            symbolNumberAndData[2] = (byte)(0);
            // len len 
            // adr adr adr

            for (int i = 0; i < /*a_data.Length*/ LengthToTx; i++)
                symbolNumberAndData[i + 3] = a_data[i];

            string requestString = "RequestString: ";
            foreach (byte b in symbolNumberAndData)
            {
                requestString += b.ToString("X2") + " ";
            }
            Console.WriteLine(requestString);
            // end dump to console
            Console.WriteLine("SymbolNumberAndData length: " + symbolNumberAndData.Length.ToString("X8"));
            KWPRequest t_request = new KWPRequest(0x3D, 0x80, symbolNumberAndData);
            Console.WriteLine(t_request.ToString());
            result = sendRequest(t_request, out reply);
            if (result != KWPResult.OK)
            {
                Console.WriteLine("Result != KWPResult.OK");
                return false;
            }
            Console.WriteLine("Result = " + reply.getData()[0].ToString("X2"));
            Console.WriteLine("Resulttotal = " + reply.ToString());
            if (reply.getData()[0] == 0x7D)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method writes to a symbol in RAM.
        /// The ECU must not be write protected for this to work.
        /// </summary>
        /// <param name="a_symbolNumber">Symbol number to write to.</param>
        /// <param name="a_data">Data to write.</param> 
        /// <returns></returns>
        public bool writeSymbolRequestTest(uint a_symbolNumber, byte[] a_data, int idx)
        {
            LogDataString("writeSymbolRequest: " + a_symbolNumber.ToString());

            KWPReply reply = new KWPReply();
            KWPResult result;
            int LengthToTx = a_data.Length;
            if (LengthToTx > 0x41) LengthToTx = 0x41; // FA
            byte[] symbolNumberAndData = new byte[3 + /*a_data.Length*/ LengthToTx];
            //First two bytes should be the symbol number
            symbolNumberAndData[0] = (byte)(a_symbolNumber >> 8);
            symbolNumberAndData[1] = (byte)(a_symbolNumber);
            symbolNumberAndData[2] = (byte)(0);
            // len len 
            // adr adr adr

            for (int i = 0; i < /*a_data.Length*/ LengthToTx; i++)
                symbolNumberAndData[i + 3] = a_data[i];

            string requestString = "RequestString: ";
            foreach (byte b in symbolNumberAndData)
            {
                requestString += b.ToString("X2") + " ";
            }
            Console.WriteLine(requestString);
            // end dump to console
            Console.WriteLine("SymbolNumberAndData length: " + symbolNumberAndData.Length.ToString("X8"));
            KWPRequest t_request = new KWPRequest(0x3D, 0x80, symbolNumberAndData);
            Console.WriteLine(t_request.ToString());
            result = sendRequest(t_request, out reply);
            if (result != KWPResult.OK)
            {
                Console.WriteLine("Result != KWPResult.OK");
                return false;
            }
            Console.WriteLine("Result = " + reply.getData()[0].ToString("X2"));
            if (reply.getData()[0] == 0x7D)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method sends a request to exit data transfer exit. It should be called when a 
        /// read session has been finished.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool sendDataTransferExitRequest()
        {
            LogDataString("sendDataTransferExitRequest");

            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x82, 0x00), out reply);
            if (result == KWPResult.OK)
                return true;
            else 
                return false;
        }

        /// <summary>
        /// This method send a request to receive data from flash. The sendReadRequest
        /// method must be called before this.
        /// </summary>
        /// <param name="r_data">The requested data.</param>
        /// <returns></returns>
        public bool sendRequestDataByOffset(out byte[] r_data)
        {
            LogDataString("sendRequestDataByOffset");

            KWPReply reply = new KWPReply();
            KWPResult result;
            result = sendRequest(new KWPRequest(0x21, 0xF0), out reply);
            r_data = reply.getData();
            if (result == KWPResult.OK)
                return true;
            else 
                return false;
        }

        public static void startLogging()
        {
            m_logginEnabled = true;
            /*DateTime dateTime = DateTime.Now;
            String fileName = "kwplog.txt";
            if (!File.Exists(fileName))
                File.Create(fileName);
            try
            {
                m_logFileStream = new StreamWriter(fileName);
                m_logFileStream.WriteLine("New logging started: " + dateTime);
                m_logFileStream.WriteLine();
                m_logginEnabled = true;
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to enable logging");
            }*/
            
        }

        public static void stopLogging()
        {
            m_logginEnabled = false;
            /*if(m_logFileStream != null)
                m_logFileStream.Close();*/

        }

        /// <summary>
        /// This method sends a KWPRequest and returns a KWPReply.
        /// </summary>
        /// <param name="a_request">The request.</param>
        /// <param name="a_reply">The reply.</param>
        /// <returns>KWPResult</returns>
        private KWPResult sendRequest(KWPRequest a_request, out KWPReply a_reply)
        {
            LogDataString("sendRequest");

            KWPReply reply = new KWPReply();
            RequestResult result;
            a_reply = new KWPReply();
            //<GS-11012010> was allemaal 1000
            int keepAliveTimeout = 1000;
            //Console.WriteLine("Checking KWP device open");
            if (!m_kwpDevice.isOpen())
                return KWPResult.DeviceNotConnected;

            // reset the timer for keep alive (set to 1 seconds now)
            if (stateTimer == null)
                stateTimer = new System.Threading.Timer(sendKeepAlive, new Object(), keepAliveTimeout, keepAliveTimeout);
            stateTimer.Change(keepAliveTimeout, keepAliveTimeout);

            m_requestMutex.WaitOne();

            LogDataString(a_request.ToString());

            result = m_kwpDevice.sendRequest(a_request, out reply);
            a_reply = reply;
            if (result == RequestResult.NoError)
            {
                LogDataString(reply.ToString());
                LogDataString(""); // empty line

                m_requestMutex.ReleaseMutex();
                return KWPResult.OK;
            }
            else
            {
                LogDataString("Timeout in KWPHandler::sendRequest: " + result.ToString());
                Console.WriteLine("Timeout in KWPHandler::sendRequest: " + result.ToString());
                m_requestMutex.ReleaseMutex();
                return KWPResult.Timeout;
            }
        }
        /// <summary>
        /// This method sends a KWPRequest and returns a KWPReply.
        /// </summary>
        /// <param name="a_request">The request.</param>
        /// <param name="a_reply">The reply.</param>
        /// <returns>KWPResult</returns>
        private KWPResult sendRequest(KWPRequest a_request, out KWPReply a_reply, int expectedLength)
        {
            int _maxSendRetries = 3;
            KWPResult _kwpResult = KWPResult.Timeout;
            LogDataString("sendRequest");

            KWPReply reply = new KWPReply();
            RequestResult result;
            a_reply = new KWPReply();
            //<GS-11012010> was allemaal 1000
            int keepAliveTimeout = 1000;
            //Console.WriteLine("Checking KWP device open");
            if (!m_kwpDevice.isOpen())
                return KWPResult.DeviceNotConnected;

            // reset the timer for keep alive (set to 1 seconds now)
            if (stateTimer == null)
                stateTimer = new System.Threading.Timer(sendKeepAlive, new Object(), keepAliveTimeout, keepAliveTimeout);
            stateTimer.Change(keepAliveTimeout, keepAliveTimeout);

            m_requestMutex.WaitOne();
            int _retryCount = 0;
            result = RequestResult.Unknown; // <GS-11022010>
            while (_retryCount < _maxSendRetries && result != RequestResult.NoError)
            {
                LogDataString(a_request.ToString());
                result = m_kwpDevice.sendRequest(a_request, out reply);
                if ((int)reply.getLength() != expectedLength)
                {
                    result = RequestResult.InvalidLength;
                    
                }
                if (result == RequestResult.NoError)
                {
                    a_reply = reply;
                    LogDataString(reply.ToString());
                    LogDataString(""); // empty line
                    m_requestMutex.ReleaseMutex();
                    //return KWPResult.OK;
                    _kwpResult = KWPResult.OK;
                }
                else
                {
                    LogDataString("Timeout in KWPHandler::sendRequest");
                    m_requestMutex.ReleaseMutex();
                    //return KWPResult.Timeout;
                    _kwpResult = KWPResult.Timeout;
                }
            }
            return _kwpResult;
        }

        /// <summary>
        /// Helper method for transforming the information in a KWPReply to a string.
        /// </summary>
        /// <param name="a_reply">The KWPReply.</param>
        /// <returns>A string representing the information in the a_reply.</returns>
        private string getString(KWPReply a_reply)
        {
            if (a_reply.getData().Length == 0)
                return "";
            Encoding ascii = Encoding.ASCII;
            ascii.GetChars(a_reply.getData(), 0, a_reply.getData().Length);
            return ascii.GetString(a_reply.getData(), 0, a_reply.getData().Length);
        }

        private UInt16 getUint16(KWPReply a_reply)
        {
            UInt16 uinteger;
            uinteger = (UInt16)((a_reply.getData()[1] << 8) | (a_reply.getData()[0]));
            return uinteger;
        }

        /// <summary>
        /// Calculate key for a seed.
        /// </summary>
        /// <param name="a_seed">Byte array with two bytes representing the seed.</param>
        /// <param name="a_method">Type of method to use for calculation [0,1].</param>
        /// <returns>Byte array with two bytes representing the key.</returns>
        private byte[] calculateKey(byte[] a_seed, uint a_method)
        {
            int key;
            byte[] returnKey = new byte[2];
            int seed = a_seed[0] << 8 | a_seed[1];
    
            key = seed << 2;
            key &= 0xFFFF;
            key ^= (a_method == 1 ? 0x4081 : 0x8142);
            key -= (a_method == 1 ? 0x1F6F : 0x2356);
            key &= 0xFFFF;

            returnKey[0] = (byte)((key >> 8) & 0xFF);
            returnKey[1] = (byte)(key & 0xFF);

            return returnKey;
        }

        private static bool m_logginEnabled = false;
        //private static StreamWriter m_logFileStream;
        private static KWPHandler m_instance;
        private Mutex m_requestMutex = new Mutex();
        private TimerCallback timerDelegate;
        private System.Threading.Timer stateTimer;

    }
}
