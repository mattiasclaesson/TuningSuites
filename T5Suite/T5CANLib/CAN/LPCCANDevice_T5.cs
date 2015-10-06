//-----------------------------------------------------------------------------
//	Universal Trionic adapter library
//	(C) Janis Silins, 2010
//  $Id$
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Combi;

namespace T5CANLib.CAN
{

//-----------------------------------------------------------------------------
/**
    CAN library driver for LPC17xx based devices.
*/
public class LPCCANDevice_T5 : ICANDevice, IDisposable
{
    // dynamic state
    private Thread read_thread;                     ///< reader thread
    private bool term_requested = false;            ///< thread termination flag
    private Object term_mutex = new Object();       ///< mutex for termination flag
    private bool logging_enabled = false;           ///< logging flag
    private string startup_path =
        @"C:\Program Files\Dilemma\CarPCControl";   ///< startup path   

    private caCombiAdapter combi;               ///< adapter object
    private CANMessage in_msg =
        new CANMessage();                       ///< incoming message

    //-------------------------------------------------------------------------
    /**
        Default constructor.
    */
    public LPCCANDevice_T5()
    {
        // create adapter
        this.combi = new caCombiAdapter();
        Debug.Assert(this.combi != null);

        // create reader thread
        this.read_thread = new Thread(this.read_messages);
        Debug.Assert(read_thread != null);
    }

    //-------------------------------------------------------------------------
    /**
        Destructor.
    */
    ~LPCCANDevice_T5()
    {
        // release adapter
        this.close();
        this.combi = null;
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
        // empty
    }

    //-----------------------------------------------------------------------------
    /**
	    Checks if ADC low-pass filter is active.

	    @param		channel		A/D channel number [0...4]

	    @return					active (yes / no)
    */

    public override void setPortNumber(string portnumber)
    {
        //nothing to do
    }

    /*
    public override bool GetADCFiltering(uint channel)
    {
        Debug.Assert(this.combi != null);
        return this.combi.GetADCFiltering(channel);
    }*/

    //-----------------------------------------------------------------------------
    /**
	    Enables / disables low-pass filtering for all ADC channels and stores
	    the setting in EEPROM.

	    @param			channel			A/D channel number [0...4]
	    @param			enable			filtering enabled (yes / no)
    */
    /*public override void SetADCFiltering(uint channel, bool enable)
    {
        Debug.Assert(this.combi != null);
        this.combi.SetADCFiltering(channel, enable);
    }*/

    //-----------------------------------------------------------------------------
    /**
        Returns momentary voltage from A/D converter; works in all modes.

        @param		channel		A/D channel number [0...4]

        @return					analog value, V					
    */
    public override float GetADCValue(uint channel)
    {
        Debug.Assert(this.combi != null);
        return this.combi.GetADCValue(channel);
    }

    //-----------------------------------------------------------------------------
    /**
        Returns current temperature from K-type thermocouple.

        @param		value		temperature, DegC			
    */
    public override float GetThermoValue()
    {
        Debug.Assert(this.combi != null);
        float value = this.combi.GetThermoValue();
        //value += DateTime.Now.Millisecond;
        return value;
    }

    //-------------------------------------------------------------------------
    /**
        Connects to adapter over USB.
      
        @return             succ / fail 
    */
    public bool connect()
    {
        try
        {
            // connect to adapter
            this.combi.Open();
            uint fw_ver = this.combi.GetFirmwareVersion();

            return true;
        }

        catch 
        {
            return false;
        }
    }

    //-------------------------------------------------------------------------
    /**
        Disconnects from adapter.
      
        @return             succ / fail 
    */
    public void disconnect()
    {
        this.combi.Close();
    }

    //-------------------------------------------------------------------------
    /**
        Opens a connection to CAN interface. 

        @return             result 
    */
    public override OpenResult open()
    {
        try
        {
            // connect to adapter
            this.connect();

            // open CAN channel
            this.combi.CAN_SetBitrate(615000);
            this.combi.CAN_Open(true);

            // start reader thread
            this.read_thread.Start();
            return OpenResult.OK;
        }

        catch
        {
            // cleanup
            this.close();

            // adapter not present
            return OpenResult.OpenError;
        }
    }

    //-------------------------------------------------------------------------
    /**
        Determines if connection to CAN device is open.
    
        return          open (true/false)
    */
    public override bool isOpen()
    {
        return this.combi.IsOpen();
    }

    //-------------------------------------------------------------------------
    /**
        Closes the connection to CAN interface.
     
        return          success (true/false)
    */
    public override CloseResult close()
    {
        try
        {
            // terminate worker thread
            Debug.Assert(this.term_mutex != null);
            lock (this.term_mutex)
            {
                this.term_requested = true;
            }

            // close connection
            this.disconnect();
            return CloseResult.OK;
        }

        catch
        {
            // ignore errors
            return CloseResult.OK;
        }
    }

    //-------------------------------------------------------------------------
    /**
        Clears incoming data buffer.
    */
    public override void clearReceiveBuffer()
    {
        // TODO
    }

    //-------------------------------------------------------------------------
    /**
        Clears outgoing data buffer.
    */
    public override void clearTransmitBuffer()
    {
        // TODO
    }

    //-------------------------------------------------------------------------
    /**
    */

    //-------------------------------------------------------------------------
    /**
        Returns the number of connected adapters.
      
        @return             number of adapters
    */
    public override int GetNumberOfAdapters()
    {
        return 1;
    }

    //-------------------------------------------------------------------------
    /**
        Enables logging.
     
        @param      path2log        log file location
    */
    public override void EnableLogging(string path2log)
    {
        this.logging_enabled = true;
        this.startup_path = path2log;
    }

    //-------------------------------------------------------------------------
    /**
        Disables logging.
    */
    public override void DisableLogging()
    {
        this.logging_enabled = false;
    }

    //-------------------------------------------------------------------------
    /**
        Writes a CAN message to log file.
     
        @param      r_canMsg        message
        @param      IsTransmit      ???
    */
    private void DumpCanMsg(CANMessage r_canMsg, bool IsTransmit)
    {
        DateTime dt = DateTime.Now;

        try
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(this.startup_path, 
                dt.Year.ToString("D4") + dt.Month.ToString("D2") + 
                dt.Day.ToString("D2") + "-CanTrace.log"), true))
            {
                if (IsTransmit)
                {
                    // get the byte transmitted
                    int transmitvalue = (int)(r_canMsg.getData() & 0x000000000000FF00);
                    transmitvalue /= 256;

                    sw.WriteLine(dt.ToString("dd/MM/yyyy HH:mm:ss") + 
                        " TX: id=" + r_canMsg.getID().ToString("D2") + 
                        " len= " + r_canMsg.getLength().ToString("X8") + 
                        " data=" + r_canMsg.getData().ToString("X16") + 
                        " " + r_canMsg.getFlags().ToString("X2") + 
                        " character = " + GetCharString(transmitvalue) + 
                        "\t ts: " + r_canMsg.getTimeStamp().ToString("X16") + 
                        " flags: " + r_canMsg.getFlags().ToString("X2"));
                }
                else
                {
                    // get the byte received
                    int receivevalue = (int)(r_canMsg.getData() & 0x0000000000FF0000);
                    receivevalue /= (256 * 256);
                    sw.WriteLine(dt.ToString("dd/MM/yyyy HH:mm:ss") + 
                        " RX: id=" + r_canMsg.getID().ToString("D2") + 
                        " len= " + r_canMsg.getLength().ToString("X8") + 
                        " data=" + r_canMsg.getData().ToString("X16") + 
                        " " + r_canMsg.getFlags().ToString("X2") + 
                        " character = " + GetCharString(receivevalue) + 
                        "\t ts: " + r_canMsg.getTimeStamp().ToString("X16") + 
                        " flags: " + r_canMsg.getFlags().ToString("X2"));
                }
            }
        }
        catch (Exception E)
        {
            Console.WriteLine("Failed to write to logfile: " + E.Message);
        }
    }

    //-------------------------------------------------------------------------
    /**
        Converts a special character to string.
      
        @param      value       character
     
        @return                 string 
    */ 
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

    //-------------------------------------------------------------------------
    /**
        Sends a 11 bit CAN data frame.
     
        @param      msg         CAN message
      
        @return                 success (true/false) 
    */
    public override bool sendMessage(CANMessage msg)
    {
        if (this.logging_enabled)
        {
            this.DumpCanMsg(msg, true);
        }

        try
        {
            Combi.caCombiAdapter.caCANFrame frame;
            frame.id = msg.getID();
            frame.length = msg.getLength();
            frame.data = msg.getData();
            frame.is_extended = 0;
            frame.is_remote = 0;

            this.combi.CAN_SendMessage(ref frame);
            return true;
        }

        catch (Exception e)
        {
            return false;
        }
    }

    //-------------------------------------------------------------------------
    /**    
        Handles incoming messages.
    */
    private void read_messages()
    {
        caCombiAdapter.caCANFrame frame = new caCombiAdapter.caCANFrame();

        // main loop
        while (true)
        {
            // check tor thread termination request
            Debug.Assert(this.term_mutex != null);
            lock (this.term_mutex)
            {
                if (this.term_requested)
                {
                    // exit
                    return;
                }
            }

            // receive messages
            if (this.combi.CAN_GetMessage(ref frame, 1000))
            {
                // convert message
                this.in_msg.setID(frame.id);
                this.in_msg.setLength(frame.length);
                this.in_msg.setData(frame.data);

                // pass message to listeners
                lock (this.m_listeners)
                {
                    foreach (ICANListener listener in this.m_listeners)
                    {
                        listener.handleMessage(this.in_msg);
                    }
                }
            }
        }
    }
};

}   // end namespace
//-----------------------------------------------------------------------------
//  EOF
//-----------------------------------------------------------------------------
