//-----------------------------------------------------------------------------
//  Just4Trionic CAN<->USB interface driver
//  $Id$
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO.Ports;
using T5CANLib.WMI;

namespace T5CANLib.CAN
{

//-----------------------------------------------------------------------------
/**
    Wrapper for Just4Trionic (see http://www.mbed.org/Just4pLeisure/notebook/just4trionic).
*/

    public class Just4TrionicDevice : ICANDevice, IDisposable
{

    bool m_deviceIsOpen = false;
    readonly SerialPort m_serialPort = new SerialPort();
    private Thread m_readThread;
    readonly Object m_synchObject = new Object();
    bool m_endThread = false;

    private const char ESC = '\x1B';
    private const int m_forcedBaudrate = 115200;
    //private int m_forcedBaudrate = 921600;

    // internal state
    //private Thread read_thread;                     ///< reader thread
    //private bool term_requested = false;            ///< thread termination flag
    //private Object term_mutex = new Object();       ///< mutex for termination flag

    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /**
        Destructor.
    */
    ~Just4TrionicDevice()
    {
        lock (m_synchObject)
        {
            m_endThread = true;
        }
        close();
    }

    //-------------------------------------------------------------------------
    /**
        Releases the object.
    */
    public override void Delete()
    {
        // empty
    }

    //-------------------------------------------------------------------------
    /**
        Disposes of the object.
    */
    public void Dispose()
    {
        m_serialPort.Dispose();
    }

    override public void EnableLogging(string path2log)
    {
    }

    override public void DisableLogging()
    {
    }

    public override float GetADCValue(uint channel)
    {
        return 0F;
    }

    // not supported by lawicel
    public override float GetThermoValue()
    {
        return 0F;
    }
    override public int GetNumberOfAdapters()
    {
        return 1;
    }


    public override void setPortNumber(string portnumber)
    {
        //nothing to do
    }
    override public void clearTransmitBuffer()
    {
        // implement clearTransmitBuffer
    }

    override public void clearReceiveBuffer()
    {
        // implement clearReceiveBuffer
    }

    //-------------------------------------------------------------------------
    /**
        Opens a connection to CAN interface. 

        @return             result 
    */
    public override OpenResult open()
    {
        //Automatically find port with Just4Trionic
        string port = UseWMIForCOMPortByFriendlyName("mbed Serial Port");

        m_serialPort.BaudRate = m_forcedBaudrate;
        m_serialPort.Handshake = Handshake.None;
        //m_serialPort.ReadBufferSize = 0x10000;
        m_serialPort.ReadTimeout = 100;
        if (port != null)
        {
            // only check this comport
            Console.WriteLine("Opening com: " + port);

            if (m_serialPort.IsOpen)
                m_serialPort.Close();
            m_serialPort.PortName = port;

            try
            {
                m_serialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                return OpenResult.OpenError;
            }

            m_deviceIsOpen = true;

            m_serialPort.BreakState = true;     //Reset mbed / Just4trionic
            m_serialPort.BreakState = false;     //
            Thread.Sleep(1000);
            m_serialPort.Write("o\r");          // 'open' Just4trionic CAN interface
            Thread.Sleep(10);
            
            m_serialPort.Write("s2\r");         // Set Just4trionic CAN speed to 615,000 bits (T5-SFI)
            Thread.Sleep(10);
            Flush();                            // Flush 'junk' in serial port buffers

            try
            {
                //m_serialPort.ReadLine();
                Console.WriteLine("Connected to CAN at 615,000 speed");
                //CastInformationEvent("Connected to CAN T5-SFI using " + port);

                //if (m_readThread != null)
                //{
                //    Console.WriteLine(m_readThread.ThreadState.ToString());
                //}
                m_readThread = new Thread(readMessages);
                m_endThread = false; // reset for next tries :)
                if (m_readThread.ThreadState == ThreadState.Unstarted)
                    m_readThread.Start();
                m_serialPort.Write("f5\r");         // Set Just4trionic filter to allow only Trionic 5 messages
                Thread.Sleep(10);
                Flush();                            // Flush 'junk' in serial port buffers
                return OpenResult.OK;
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to connect to the T5 SFI");
            }

            //CastInformationEvent("Oh dear :-( Just4Trionic cannot connect to a CAN bus.");
            close();
            return OpenResult.OpenError;
        }
        //CastInformationEvent("Oh dear :-( Just4Trionic doesn't seem to be connected to your computer.");
        close();
        return OpenResult.OpenError;
    }

    private static string UseWMIForCOMPortByFriendlyName(string strFriendlyName)
    {
        foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
        {
            if (comPort.Description.StartsWith(strFriendlyName))
                return comPort.Name;
        }
        return null;
    }


    private void Flush()
    //public override void Flush()
    {
        if (m_deviceIsOpen)
        {
            m_serialPort.DiscardInBuffer();
            m_serialPort.DiscardOutBuffer();
        }
    }
    
    //-------------------------------------------------------------------------
    /**
        Determines if connection to CAN device is open.
    
        return          open (true/false)
    */
    public override bool isOpen()
    {
        return m_deviceIsOpen;
    }

    //-------------------------------------------------------------------------
    /**
        Closes the connection to CAN interface.
     
        return          success (true/false)
    */
    public override CloseResult close()
    {
        if (m_deviceIsOpen)
        {
            m_serialPort.Write("\x1B");         // mbed ESCape CAN interface
            m_serialPort.BreakState = true;     // Reset mbed / Just4trionic
            m_serialPort.BreakState = false;    //
        }
        // terminate thread?
        m_endThread = true;
        m_serialPort.Close();
        m_deviceIsOpen = false;

        return CloseResult.OK;
    }

    //-------------------------------------------------------------------------
    /**
        Sends a 11 bit CAN data frame.
     
        @param      message     CAN message
      
        @return                 success (true/false) 
    */
    public override bool sendMessage(CANMessage a_message)
    {
        string sendString = "t";
        sendString += a_message.getID().ToString("X3");
        sendString += a_message.getLength().ToString("X1");
        for (uint i = 0; i < a_message.getLength(); i++) // leave out the length field, the ELM chip assigns that for us
        {
            sendString += a_message.getCanData(i).ToString("X2");
        }
        sendString += "\r";
        if (m_serialPort.IsOpen)
        {
            //AddToCanTrace("TX: " + a_message.getID().ToString("X3") + " " + a_message.getLength().ToString("X1") + " " + a_message.getData().ToString("X16"));
            m_serialPort.Write(sendString);
            //Console.WriteLine("TX: " + sendString);
        }

        // bitrate = 38400bps -> 3840 bytes per second
        // sending each byte will take 0.2 ms approx
        //Thread.Sleep(a_message.getLength()); // sleep length ms
        //            Thread.Sleep(10);
        Thread.Sleep(1);

        return true; // remove after implementation
    }

    //-------------------------------------------------------------------------
    /**    
        Handles incoming messages.
    */
    private void readMessages()
    {
        CANMessage canMessage = new CANMessage();
        string rxMessage = string.Empty;

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

            try
            {
                if (m_serialPort.IsOpen)
                {
                    do
                    {
                        rxMessage = m_serialPort.ReadLine();
                        rxMessage = rxMessage.Replace("\r", ""); // remove prompt characters... we don't need that stuff
                        rxMessage = rxMessage.Replace("\n", ""); // remove prompt characters... we don't need that stuff
                    } while (rxMessage.StartsWith("w") == false);

                    uint id = Convert.ToUInt32(rxMessage.Substring(1, 3), 16);
                    canMessage.setID(id);
                    canMessage.setLength(8);
                    canMessage.setData(0x0000000000000000);
                    for (uint i = 0; i < 8; i++)
                        canMessage.setCanData(Convert.ToByte(rxMessage.Substring(5 + (2 * (int)i), 2), 16), i);

                    lock (m_listeners)
                    {
                        //AddToCanTrace("RX: " + canMessage.getID().ToString("X3") + " " + canMessage.getLength().ToString("X1") + " " + canMessage.getData().ToString("X16"));
                        //Console.WriteLine("MSG: " + rxMessage);
                        foreach (ICANListener listener in m_listeners)
                        {
                            listener.handleMessage(canMessage);
                        }
                        //CastInformationEvent(canMessage); // <GS-05042011> re-activated this function
                    }
                    // give up CPU for a moment
                    Thread.Sleep(1);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("MSG: " + rxMessage);
            }
        }
    }
};

}   // end namespace
//-----------------------------------------------------------------------------
//  EOF
//-----------------------------------------------------------------------------
