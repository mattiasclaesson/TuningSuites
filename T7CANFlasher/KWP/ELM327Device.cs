using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace T7.KWP
{
    class ELM327Device : IKWPDevice
    {

        bool m_deviceIsOpen = false;
        SerialPort m_serialPort = new SerialPort();

        /// <summary>
        /// Constructor for ELM327Device.
        /// </summary>
        public ELM327Device()
        {
          
        }

        /// <summary>
        /// This method starts a new KWP session. It must be called before the sendRequest
        /// method can be called.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool startSession()
        {
            string str = "";
            m_serialPort.Write("AT IIA 11\r"); //Set ISO init address to 0x11
            m_serialPort.ReadTo(">");

            m_serialPort.Write("AT SH CF A1 F1\r");    //Set header
            str = m_serialPort.ReadTo(">");
            return true;
        }

        /// <summary>
        /// This method sends a KWP request and returns a KWPReply. The method returns
        /// when a reply has been received, after a failure or after a timeout.
        /// The open and startSession methods must be called and returned possitive result
        /// before this method is used.
        /// </summary>
        /// <param name="a_request">The KWPRequest.</param>
        /// <param name="r_reply">The reply to the KWPRequest.</param>
        /// <returns>RequestResult.</returns>
        public RequestResult sendRequest(KWPRequest a_request, out KWPReply r_reply)
        {
            string sendString = "";
            string receiveString = "";
            for (int i = 1; i < a_request.getData().Length; i++)
                sendString += a_request.getData()[i].ToString("X") + " ";
            sendString += "\r";
            m_serialPort.Write(sendString);
            //receiveString = "49 01 01 00 00 00 31 \n\r49 02 02 44 34 47 50 \n\r49 02 03 30 30 52 35 \n\r49 02 04 25 42";// m_serialPort.ReadTo(">");
            receiveString = m_serialPort.ReadTo(">");
            char[] chrArray = receiveString.ToCharArray();
            byte[] reply = new byte[0xFF];
            int insertPos = 1;
            int index = 0;
            string subString = "";
            while(receiveString.Length > 4)
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
                    reply[insertPos] = (byte)Convert.ToInt16("0x"+subString, 16);
                    insertPos++;
                    receiveString = receiveString.Remove(0, index + 1);
                }

            }

            reply[0] = (byte)insertPos; //Length

            r_reply = new KWPReply(reply, a_request.getNrOfPID());
            return RequestResult.NoError;
        }

        /// <summary>
        /// This method opens a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool open()
        {
            //Automatically find port with ELM327
            
            //Detect all serial ports.
            string[] serialPortNames = SerialPort.GetPortNames();
            m_serialPort.BaudRate = 9600;
            m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 3000;
            bool readException = false;

            //Check if a ELM327 v1.2 is connected to any port.
            foreach (string port in serialPortNames)
            {
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
                    return false;
                }
                    
                m_serialPort.Write("ATZ\r");    //Reset all
                Thread.Sleep(3000);

                //Try to set up ELM327
                try
                {
                    m_serialPort.ReadTo(">");
                }
                catch(Exception)
                {
                    readException = true;
                }
                if (readException)
                    continue;
                m_serialPort.Write("ATL1\r");   //Linefeeds On
                m_serialPort.ReadTo(">");       
                m_serialPort.Write("ATE0\r");   //Echo off
                m_serialPort.ReadTo(">");
                m_serialPort.Write("ATI\r");    //Print version
                string answer = m_serialPort.ReadTo(">");
                if (answer.StartsWith("ELM327 v1.2"))
                {
                    m_deviceIsOpen = true;
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// This method closes a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public bool close()
        {
            m_serialPort.Close();
            return true;
        }

        /// <summary>
        /// This method checks if the IKWPDevice is opened or not.
        /// </summary>
        /// <returns>true if device is open, otherwise false.</returns>
        public bool isOpen()
        {
            return m_deviceIsOpen;
        }
    }
}
