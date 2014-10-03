using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using T7.CAN;

namespace T7.KWP
{
    class ELM327Device : IKWPDevice
    {

        bool m_deviceIsOpen = false;
        SerialPort m_serialPort = new SerialPort();

        private int m_baseBaudrate = 38400;
        public int BaseBaudrate
        {
            get
            {
                return m_baseBaudrate;
            }
            set
            {
                m_baseBaudrate = value;
            }
        }

        /// <summary>
        /// Constructor for ELM327Device.
        /// </summary>
        public ELM327Device()
        {
          
        }

        private bool m_EnableLog = false;

        public override bool EnableLog
        {
            get { return m_EnableLog; }
            set { m_EnableLog = value; }
        }

        private int m_forcedBaudrate = 38400;

        public override int ForcedBaudrate
        {
            get
            {
                return m_forcedBaudrate;
            }
            set
            {
                m_forcedBaudrate = value;
            }
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

        /// <summary>
        /// Set the CAN device to be used by this class.
        /// </summary>
        /// <param name="a_canDevice">A ICANDevice.</param>
        public override void setCANDevice(ICANDevice a_canDevice)
        {
           
        }

        /// <summary>
        /// This method starts a new KWP session. It must be called before the sendRequest
        /// method can be called.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public override bool startSession()
        {
            string str = "";
            try
            {
                m_serialPort.Write("ATSP5\r");
                str = m_serialPort.ReadTo(">");

                m_serialPort.Write("ATAL\r");
                str = m_serialPort.ReadTo(">");

                m_serialPort.Write("ATSH8011F1\r");    //Set header
                str = m_serialPort.ReadTo(">");
                m_serialPort.Write("1A90\r");             //Read VIN. This is only done to initiate the bus.
                str = m_serialPort.ReadTo(">");
                if (str.StartsWith("BUS INIT: ERROR"))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
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
        public override RequestResult sendRequest(KWPRequest a_request, out KWPReply r_reply)
        {
            string sendString = "";
            string receiveString = "";
            byte[] reply = new byte[0xFF];
            try
            {
                for (int i = 1; i < a_request.getData().Length; i++)
                {
                    string tmpStr = a_request.getData()[i].ToString("X");
                    if (tmpStr.Length == 1)
                        sendString += "0" + tmpStr + " ";
                    else
                        sendString += tmpStr + " ";
                }
                sendString += "\r";

                m_serialPort.Write(sendString);
                //receiveString = "5A 90 59 53 33 45 46 35 39 45 32 33 33 30 32 30 38 32 37 \r\n\r\n";// m_serialPort.ReadTo(">");
                receiveString = m_serialPort.ReadTo(">");
                string tmpString = receiveString;

                int insertPos = 1;
                string subString = "";

                while (receiveString.Length > 4)
                {
                    int index = receiveString.IndexOf(" ");
                    subString = receiveString.Substring(0, index);
                    reply[insertPos] = (byte)Convert.ToInt16("0x" + subString, 16);
                    insertPos++;
                    receiveString = receiveString.Remove(0, index + 1);
                }
                insertPos--;

                reply[0] = (byte)insertPos; //Length

                r_reply = new KWPReply(reply, a_request.getNrOfPID());
                return RequestResult.NoError;
            }
            catch (Exception)
            {
                r_reply = new KWPReply(reply, a_request.getNrOfPID());
                return RequestResult.ErrorSending;
            }
        }

        /// <summary>
        /// This method opens a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public override bool open()
        {
            var detectedRate = DetectInitialPortSpeedAndReset();
            if (detectedRate != 0)
                BaseBaudrate = detectedRate;

            m_serialPort.BaudRate = BaseBaudrate;
            m_serialPort.Handshake = Handshake.None;
            //m_serialPort.ReadTimeout = 100;
            m_serialPort.WriteTimeout = 1000;
            m_serialPort.ReadBufferSize = 1024;
            m_serialPort.WriteBufferSize = 1024;

            //m_serialPort.BaudRate = m_portSpeed;
            //m_serialPort.Handshake = Handshake.None;
            m_serialPort.ReadTimeout = 3000;
            //m_serialPort.Parity = Parity.None;
            //m_serialPort.StopBits = StopBits.One;
            //m_serialPort.DtrEnable = true;
            //m_serialPort.RtsEnable = true;
            //bool readException = false;
            string str;

            if (m_forcedComport != string.Empty)
            {
                //readException = false;
                if (m_serialPort.IsOpen)
                    m_serialPort.Close();
                m_serialPort.PortName = m_forcedComport;

                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }

                //if (m_portSpeed == 38400)
                //{
                //    m_serialPort.BaudRate = 57600;
                //    m_serialPort.Write("ATZ\r");    //Reset all
                //    Thread.Sleep(3000);
                //    m_serialPort.BaudRate = 38400;
                //}
                //else if (m_portSpeed == 115200)
                //{
                //    m_serialPort.BaudRate = 2000000;
                //    m_serialPort.Write("ATZ\r");    //Reset all
                //    Thread.Sleep(3000);
                //    m_serialPort.BaudRate = 115200;
                //}
                //m_serialPort.Write("ATZ\r");    //Reset all
                //Thread.Sleep(3000);

                ////Try to set up ELM327
                //try
                //{
                //    str = m_serialPort.ReadTo(">");
                //}
                //catch (Exception)
                //{
                //    //readException = true;
                //}





                string answer;
                try
                {
                    m_serialPort.Write("ATL1\r");   //Linefeeds On
                    str = m_serialPort.ReadTo(">");
                    m_serialPort.Write("ATE0\r");   //Echo off
                    str = m_serialPort.ReadTo(">");
                    m_serialPort.Write("ATAT2\r");   //Automatic timing
                    str = m_serialPort.ReadTo(">");
                    string localStr = "";
                    if (detectedRate == 115200)  //Try setting the speed to 2Mbit
                    {
                        m_serialPort.Write("ATBRT28\r"); //Set baudrate timeout 200 ms
                        m_serialPort.ReadTo(">");

                        m_serialPort.Write("AT BRD 02\r");   //Automaic timing
                        str = m_serialPort.ReadLine();
                        if (str.StartsWith("OK"))
                        {
                            try
                            {
                                m_serialPort.Close();
                                m_serialPort.BaudRate = 2000000;
                                m_serialPort.Open();
                            }
                            catch (UnauthorizedAccessException e)
                            {
                                //AddToSerialTrace("exception" + e.ToString());
                                return false;
                            }

                            try
                            {
                                //m_serialPort.BaudRate = 2000000;
                                bool gotVersion = false;
                                int tries = 20;
                                while (!gotVersion && tries-- > 0)
                                {
                                    string elmVersion = m_serialPort.ReadExisting();
                                    //AddToSerialTrace("elmVersion:" + elmVersion);
                                    Console.WriteLine("elmVersion: " + elmVersion);
                                    if (elmVersion.Length > 5)
                                    {
                                        gotVersion = true;
                                    }
                                    Thread.Sleep(10);
                                    m_serialPort.Write("\r");
                                }

                                if (!gotVersion)
                                    return false;

                                m_serialPort.Write("\r");
                                answer = m_serialPort.ReadTo(">");
                            }
                            catch (Exception e)
                            {
                                //Could not connect at 2Mbit
                                m_serialPort.BaudRate = 115200;
                            }
                        }
                    }
                    m_serialPort.Write("ATI\r");    //Print version
                    answer = m_serialPort.ReadTo(">");

                }
                catch (Exception)
                {
                    return false;
                }
                if (answer.StartsWith("ELM327"))
                {
                    m_deviceIsOpen = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Detects the port speed, resets the interface, then detects the speed again
        /// </summary>
        /// <returns></returns>
        private int DetectInitialPortSpeedAndReset()
        {
            int[] speeds = new int[] { 9600, 38400, 115200, 230400, 285714, 500000, 1000000, 2000000 }; ///*2000000, 1000000, 500000, 230400,*/ 115200, 57600, 38400, 19200, 9600 };

            for (int i = 0; i < 2; i++)
            {
                foreach (var speed in speeds)
                {
                    Console.Out.WriteLine("Try speed:" + speed);
                    //if (m_serialPort.IsOpen)
                    m_serialPort.Close();
                    m_serialPort.BaudRate = speed;
                    m_serialPort.PortName = m_forcedComport;
                    m_serialPort.ReadTimeout = 1000;
                    m_serialPort.Open();
                    try
                    {
                        m_serialPort.DiscardInBuffer();
                        WriteToSerialWithTrace("ATI\r");
                        Thread.Sleep(50);
                        WriteToSerialWithTrace("ATI\r"); //need to send 2 times for some reason...
                        Thread.Sleep(50);
                        string reply = m_serialPort.ReadExisting();
                        Console.Out.WriteLine("Result:" + reply);
                        bool success = !string.IsNullOrEmpty(reply) && reply.Contains("ELM327");
                        if (success)
                        {
                            if (i == 0)
                            {
                                WriteToSerialWithTrace("ATZ\r");//do reset
                                Thread.Sleep(2000);//wait for it to transfer data 
                                m_serialPort.Close();
                                break;
                            }
                            else
                            {
                                m_serialPort.Close();
                                return speed;
                            }
                        }
                        else
                        {
                            Console.Out.WriteLine("Failed");
                            m_serialPort.Close();
                        }
                    }
                    catch (Exception x)
                    {
                        //AddToDeviceTrace("ELM372Device DetectInitialPortSpeedAndReset" + x.Message);
                        m_serialPort.Close();
                    }
                }
            }
            return 0;
        }

        protected void WriteToSerialWithTrace(string line)
        {
            m_serialPort.Write(line);
            //AddToDeviceTrace("SERTX: " + line);
        }

        /// <summary>
        /// This method closes a KWP device for usage.
        /// </summary>
        /// <returns>true on success, otherwise false.</returns>
        public override bool close()
        {
            m_serialPort.Close();
            return true;
        }

        /// <summary>
        /// This method checks if the IKWPDevice is opened or not.
        /// </summary>
        /// <returns>true if device is open, otherwise false.</returns>
        public override bool isOpen()
        {
            return m_deviceIsOpen;
        }
    }
}
