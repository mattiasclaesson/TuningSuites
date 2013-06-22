using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using T7.CAN;
using Microsoft.Win32;

namespace T7.KWP
{
    class CANELM327Device : ICANDevice
    {
        bool m_deviceIsOpen = false;
        SerialPort m_serialPort = new SerialPort();
        Thread m_readThread;
        Object m_synchObject = new Object();
        bool m_endThread = false;
        private uint _senderID = 0x220;

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

        public CANELM327Device()
        {
          
        }

        ~CANELM327Device()
        {
            lock (m_synchObject)
            {
                m_endThread = true;
            }
            close();
        }

        public void readMessages()
        {
            CANMessage canMessage = new CANMessage();
            byte[] receiveBuffer = new byte[1024]; // circular buffer for reception of data
            string receiveString = string.Empty;
            

            Console.WriteLine("readMessages started");
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                    {
                        Console.WriteLine("readMessages ended");
                        return;
                    }
                }
                if (m_serialPort.IsOpen)
                {
                    if (m_serialPort.BytesToRead > 0)
                    {
                        receiveString += m_serialPort.ReadExisting();
                        //Console.WriteLine("BUF1: " + receiveString);
                        receiveString = receiveString.Replace(">", ""); // remove prompt characters... we don't need that stuff
                        receiveString = receiveString.Replace("NO DATA", ""); // remove prompt characters... we don't need that stuff
                        while (receiveString.StartsWith("\n") || receiveString.StartsWith("\r"))
                        {
                            receiveString = receiveString.Substring(1, receiveString.Length - 1);
                        }

                        while (receiveString.Contains('\r'))
                        {
                            // process the line
                            int idx = receiveString.IndexOf('\r');
                            string rxMessage = receiveString.Substring(0, idx);
                            receiveString = receiveString.Substring(idx + 1, receiveString.Length - idx - 1);
                            while (receiveString.StartsWith("\n") || receiveString.StartsWith("\r"))
                            {
                                receiveString = receiveString.Substring(1, receiveString.Length - 1);
                            }
                            Console.WriteLine("BUF2: " + receiveString);
                            // is it a valid line
                            if (rxMessage.Length >= 6)
                            {
                                try
                                {
                                    uint id = Convert.ToUInt32(rxMessage.Substring(0, 3), 16);
                                    if (MessageContainsInformationForRealtime(id))
                                    {
                                        canMessage.setID(id);
                                        canMessage.setLength(8); // TODO: alter to match data
                                        canMessage.setData(0x0000000000000000); // reset message content
                                        byte b1 = Convert.ToByte(rxMessage.Substring(4, 2), 16);
                                        if (b1 < 7)
                                        {
                                            canMessage.setCanData(b1, 0);
                                            //Console.WriteLine("Byte 1: " + Convert.ToByte(rxMessage.Substring(4, 2), 16).ToString("X2"));
                                            if (b1 >= 1) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(7, 2), 16), 1);
                                            if (b1 >= 2) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(10, 2), 16), 2);
                                            if (b1 >= 3) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(13, 2), 16), 3);
                                            if (b1 >= 4) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(16, 2), 16), 4);
                                            if (b1 >= 5) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(19, 2), 16), 5);
                                            if (b1 >= 6) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(22, 2), 16), 6);
                                            if (b1 >= 7) canMessage.setCanData(Convert.ToByte(rxMessage.Substring(25, 2), 16), 7);
                                        }
                                        else
                                        {
                                            canMessage.setCanData(b1, 0);
                                            //Console.WriteLine("Byte 1: " + Convert.ToByte(rxMessage.Substring(4, 2), 16).ToString("X2"));
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(7, 2), 16), 1);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(10, 2), 16), 2);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(13, 2), 16), 3);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(16, 2), 16), 4);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(19, 2), 16), 5);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(22, 2), 16), 6);
                                            canMessage.setCanData(Convert.ToByte(rxMessage.Substring(25, 2), 16), 7);
                                        }

                                        lock (m_listeners)
                                        {
                                            AddToCanTrace("RX: " + canMessage.getData().ToString("X16"));
                                            //Console.WriteLine("MSG: " + rxMessage);
                                            foreach (ICANListener listener in m_listeners)
                                            {
                                                listener.handleMessage(canMessage);
                                            }
                                        }
                                    }
                                    
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("MSG: " + rxMessage);
                                }
                            }
                        }
                    }
                    else 
                    {
                        Thread.Sleep(1); // give others some air
                    }
                }

            }
            // parse the receive string 
           

            /*int readResult = 0;
            LAWICEL.CANMsg r_canMsg = new LAWICEL.CANMsg();
            CANMessage canMessage = new CANMessage();
            Console.WriteLine("readMessages started");
            while (true)
            {
                lock (m_synchObject)
                {
                    if (m_endThread)
                    {
                        Console.WriteLine("readMessages ended");
                        return;
                    }
                }
                readResult = LAWICEL.canusb_Read(m_deviceHandle, out r_canMsg);
                if (readResult == LAWICEL.ERROR_CANUSB_OK)
                {
                    //Console.WriteLine(r_canMsg.id.ToString("X3") + " " + r_canMsg.data.ToString("X8"));
                    if (MessageContainsInformationForRealtime(r_canMsg.id))
                    {
                        canMessage.setID(r_canMsg.id);
                        canMessage.setLength(r_canMsg.len);
                        canMessage.setTimeStamp(r_canMsg.timestamp);
                        canMessage.setFlags(r_canMsg.flags);
                        canMessage.setData(r_canMsg.data);
                        lock (m_listeners)
                        {
                            AddToCanTrace("RX: " + r_canMsg.data.ToString("X16"));
                            foreach (ICANListener listener in m_listeners)
                            {
                                //while (listener.messagePending()) ; // dirty, make this better
                                listener.handleMessage(canMessage);
                            }
                            //CastInformationEvent(canMessage); // <GS-05042011> re-activated this function
                        }
                        //Thread.Sleep(1);
                    }

                    // cast event to application to process message
                    //if (MessageContainsInformationForRealtime(r_canMsg.id))
                    //{
                    //TODO: process all other known msg id's into the realtime view
                    //  CastInformationEvent(canMessage); // <GS-05042011> re-activated this function
                    //}
                }
                else if (readResult == LAWICEL.ERROR_CANUSB_NO_MESSAGE)
                {
                    Thread.Sleep(1);
                }
            }
            */
        }

        public override uint waitForMessage(uint a_canID, uint timeout, out CANMessage canMsg)
        {
            canMsg = new CANMessage();
            return 0;
        }


        public override bool sendMessage(CANMessage a_message)
        {
            string sendString = "  ";
            if (a_message.getID() != _senderID)
            {
                _senderID = a_message.getID();
                // 
                m_serialPort.Write("ATSH " + _senderID.ToString("X3") + "\r");    //Set header to XX = T7 ECU
                string answer = m_serialPort.ReadTo(">");
                Console.WriteLine("ATSH " + _senderID.ToString("X3") + " response: " + answer);

            }

            for (uint i = 0; i < a_message.getLength(); i++) // leave out the length field, the ELM chip assigns that for us
            {
                //if (i <= 7)
                {
                    sendString += a_message.getCanData(i).ToString("X2");
                }
                /*else
                {
                    sendString += "00"; // fill with zeros
                }*/
            }

            sendString += "\r";
            if (m_serialPort.IsOpen)
            {

                m_serialPort.Write(sendString);
                Console.WriteLine("TX: " + sendString);
            }

            // bitrate = 38400bps -> 3840 bytes per second
            // sending each byte will take 0.2 ms approx
            Thread.Sleep(a_message.getLength()); // sleep length ms

            //            Thread.Sleep(10);
            //receiveString = "49 01 01 00 00 00 31 \n\r49 02 02 44 34 47 50 \n\r49 02 03 30 30 52 35 \n\r49 02 04 25 42";// m_serialPort.ReadTo(">");
            /*receiveString = m_serialPort.ReadTo(">");
            char[] chrArray = receiveString.ToCharArray();
            byte[] reply = new byte[0xFF];
            int insertPos = 1;
            int index = 0;
            string subString = "";
            while (receiveString.Length > 4)
            {

                //Remove first three bytes

                //TODO. Remove Mode and PIDs
                for (int i = 0; i < 3; i++)
                {
                    index = receiveString.IndexOf(" ");
                    receiveString = receiveString.Remove(0, index + 1);
                }
                //Read data for the rest of the row.
                for (int i = 0; i < 4; i++)
                {
                    index = receiveString.IndexOf(" ");
                    if (index == 0) //Last row not 4 bytes of data.
                    {
                        continue;
                    }
                    subString = receiveString.Substring(0, index);
                    reply[insertPos] = (byte)Convert.ToInt16("0x" + subString, 16);
                    insertPos++;
                    receiveString = receiveString.Remove(0, index + 1);
                }

            }

            reply[0] = (byte)insertPos; //Length

            r_reply = new KWPReply(reply, a_request.getNrOfPID());
            return RequestResult.NoError;*/
            return true; // remove after implementation
        }

        public override float GetThermoValue()
        {
            return 0F;
        }

        public override float GetADCValue(uint channel)
        {
            return 0F;
        }

        public override bool isOpen()
        {
            return m_deviceIsOpen;
        }



        public override OpenResult open()
        {
            //Automatically find port with ELM327

            //Detect all serial ports.
            string[] serialPortNames = SerialPort.GetPortNames();
            m_serialPort.BaudRate = 38400;
            m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 3000;
            bool readException = false;
            if (m_forcedComport != string.Empty && m_forcedComport != "Autodetect")
            {
                // only check this comport
                Console.WriteLine("Opening com: " + m_forcedComport);

                readException = false;
                if (m_serialPort.IsOpen)
                    m_serialPort.Close();
                m_serialPort.PortName = m_forcedComport;

                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    return OpenResult.OpenError;
                }

                m_serialPort.Write("ATZ\r");    //Reset all
                Thread.Sleep(1000);

                //Try to set up ELM327
                try
                {
                    m_serialPort.ReadTo(">");
                }
                catch (Exception)
                {
                    readException = true;
                }
                if (readException)
                {
                    m_serialPort.Close();
                    return OpenResult.OpenError;
                }
                m_serialPort.Write("ATL1\r");   //Linefeeds On //<GS-18052011> turned off for now
                m_serialPort.ReadTo(">");
                m_serialPort.Write("ATE0\r");   //Echo off
                m_serialPort.ReadTo(">");



                m_serialPort.Write("ATI\r");    //Print version
                string answer = m_serialPort.ReadTo(">");
                Console.WriteLine("Version ELM: " + answer);
                if (answer.StartsWith("ELM327 v1.2") || answer.StartsWith("ELM327 v1.3"))
                {
                    CastInformationEvent("Connected on " + m_forcedComport);

                    m_serialPort.Write("ATSP6\r");    //Set protocol type ISO 15765-4 CAN (11 bit ID, 500kb/s)
                    answer = m_serialPort.ReadTo(">");
                    Console.WriteLine("Protocol select response: " + answer);
                    if (answer.StartsWith("OK"))
                    {
                        m_deviceIsOpen = true;

                        m_serialPort.Write("ATH1\r");    //ATH1 = Headers ON, so we can see who's talking
                        answer = m_serialPort.ReadTo(">");
                        Console.WriteLine("ATH1 response: " + answer);
                        string idString = "ATSH " + _senderID.ToString("X3");
                        m_serialPort.Write(idString + "\r");    //Set header to XX = T7 ECU
                        answer = m_serialPort.ReadTo(">");
                        Console.WriteLine(idString + answer);
                        //m_serialPort.Write("ATAL\r");    //Allow messages with length > 7
                        //Console.WriteLine("ATAL response: " + answer);
                        //answer = m_serialPort.ReadTo(">");

                        m_serialPort.Write("ATCAF0\r");   //Can formatting OFF (don't automatically send repsonse codes, we will do this!)
                        Console.WriteLine("ATCAF0:" + m_serialPort.ReadTo(">"));
                        //m_serialPort.Write("ATR0\r");     //Don't wait for response from the ECU
                        //m_serialPort.ReadTo(">");
                        if (m_readThread != null)
                        {
                            Console.WriteLine(m_readThread.ThreadState.ToString());
                        }
                        m_readThread = new Thread(readMessages);
                        m_endThread = false; // reset for next tries :)
                        if (m_readThread.ThreadState == ThreadState.Unstarted)
                            m_readThread.Start();
                        return OpenResult.OK;
                    }
                    m_serialPort.Close();
                }
            }
            //else
            {
                //Check if a ELM327 v1.2 is connected to any port.
                foreach (string port in serialPortNames)
                {
                    m_serialPort.ReadTimeout = 500;

                    CastInformationEvent("Scanning " + port);
                    Console.WriteLine("Trying port: " + port);
                    if (!port.StartsWith("COM")) continue;
                    readException = false;
                    if (m_serialPort.IsOpen)
                        m_serialPort.Close();
                    m_serialPort.PortName = port;

                    try
                    {
                        m_serialPort.Open();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //return OpenResult.OpenError;
                        continue;
                    }

                    m_serialPort.Write("ATZ\r");    //Reset all
                    Thread.Sleep(1000);

                    //Try to set up ELM327
                    try
                    {
                        m_serialPort.ReadTo(">");
                    }
                    catch (Exception)
                    {
                        readException = true;
                    }
                    if (readException)
                    {
                        m_serialPort.Close();
                        //return OpenResult.OpenError;
                        continue;
                    }
                    m_serialPort.Write("ATL1\r");   //Linefeeds On //<GS-18052011> turned off for now
                    m_serialPort.ReadTo(">");
                    m_serialPort.Write("ATE0\r");   //Echo off
                    m_serialPort.ReadTo(">");



                    m_serialPort.Write("ATI\r");    //Print version
                    string answer = m_serialPort.ReadTo(">");
                    Console.WriteLine("Version ELM: " + answer);
                    if (answer.StartsWith("ELM327 v1.2") || answer.StartsWith("ELM327 v1.3"))
                    {
                        m_serialPort.ReadTimeout = 3000;
                        CastInformationEvent("Connected on " + port);

                        SaveRegistrySetting("ELM327Port", port);


                        // save COMPORT number in registry to use this as preffered comport the next time
                        m_serialPort.Write("ATSP6\r");    //Set protocol type ISO 15765-4 CAR (11 bit ID, 500kb/s)
                        answer = m_serialPort.ReadTo(">");
                        Console.WriteLine("Protocol select response: " + answer);
                        if (answer.StartsWith("OK"))
                        {
                            m_deviceIsOpen = true;

                            m_serialPort.Write("ATH1\r");    //ATH1 = Headers ON, so we can see who's talking
                            answer = m_serialPort.ReadTo(">");
                            Console.WriteLine("ATH1 response: " + answer);
                            m_serialPort.Write("ATSH 7E0\r");    //Set header to 7E0 = ECU
                            answer = m_serialPort.ReadTo(">");
                            Console.WriteLine("ATSH 7E0 response: " + answer);
                            //m_serialPort.Write("ATAL\r");    //Allow messages with length > 7
                            //Console.WriteLine("ATAL response: " + answer);
                            //answer = m_serialPort.ReadTo(">");

                            m_serialPort.Write("ATCAF0\r");   //Can formatting OFF (don't automatically send repsonse codes, we will do this!)
                            Console.WriteLine("ATCAF0:" + m_serialPort.ReadTo(">"));
                            //m_serialPort.Write("ATR0\r");     //Don't wait for response from the ECU
                            //m_serialPort.ReadTo(">");
                            if (m_readThread != null)
                            {
                                Console.WriteLine(m_readThread.ThreadState.ToString());
                            }
                            m_readThread = new Thread(readMessages);
                            m_endThread = false; // reset for next tries :)
                            if (m_readThread.ThreadState == ThreadState.Unstarted)
                                m_readThread.Start();
                            return OpenResult.OK;
                        }
                        m_serialPort.Close();
                    }

                }
            }
            return OpenResult.OpenError;
        }

        private void SaveRegistrySetting(string key, string value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T7SuitePro"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        public override void Flush()
        {
            if (m_deviceIsOpen)
            {
                m_serialPort.DiscardInBuffer();
                m_serialPort.DiscardOutBuffer();
            }
        }

        public override CloseResult close()
        {
            m_serialPort.Close();
            m_endThread = true;
            m_deviceIsOpen = false;
            
            return CloseResult.OK;
        }
    }
}
