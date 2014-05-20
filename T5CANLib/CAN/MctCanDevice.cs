//-----------------------------------------------------------------------------
//  Mictronics CAN<->USB interface driver
//  $Id$
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace T5CANLib.CAN
{

//-----------------------------------------------------------------------------
/**
    Wrapper for Mictronics CAN<->USB interface (see http://www.mictronics.de).
*/
public class MctCanDevice : ICANDevice
{
    // driver imports
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_Create")]
    static extern void MctAdapter_Create();
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_Release")]
    static extern void MctAdapter_Release();
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_Open")]
    static extern bool MctAdapter_Open(string bitrate);
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_IsOpen")]
    static extern bool MctAdapter_IsOpen();
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_SendMessage")]
    static extern bool MctAdapter_SendMessage(uint id, byte length, ulong data);
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_ReceiveMessage")]
    static extern bool MctAdapter_ReceiveMessage(out uint id, out byte length, 
        out ulong data);
    [DllImport("mct_can.dll", EntryPoint = "MctAdapter_Close")]
    static extern bool MctAdapter_Close();

    // internal state
    private Thread read_thread;                     ///< reader thread
    private bool term_requested = false;            ///< thread termination flag
    private Object term_mutex = new Object();       ///< mutex for termination flag

    //-------------------------------------------------------------------------
    /**
        Default constructor.
    */
    public MctCanDevice()
    {
        // create adapter object
        MctAdapter_Create();

        // create reader thread
        this.read_thread = new Thread(this.read_messages);
        Debug.Assert(read_thread != null);
    }

    //-------------------------------------------------------------------------
    /**
        Destructor.
    */
    ~MctCanDevice()
    {
        MctAdapter_Release();
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

    public override void Delete()
    {
        
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
        // connect to bus
        if (!MctAdapter_Open("4037"))
        {
            return OpenResult.OpenError;
        }

        // start reader thread
        Debug.Assert(this.read_thread != null);
        if (this.read_thread.ThreadState != System.Threading.ThreadState.Running)
        {
            this.read_thread.Start();
        }

        return OpenResult.OK;
    }

    //-------------------------------------------------------------------------
    /**
        Determines if connection to CAN device is open.
    
        return          open (true/false)
    */
    public override bool isOpen()
    {
        return MctAdapter_IsOpen();
    }

    //-------------------------------------------------------------------------
    /**
        Closes the connection to CAN interface.
     
        return          success (true/false)
    */
    public override CloseResult close()
    {
        return MctAdapter_Close() ? CloseResult.OK : CloseResult.CloseError;
    
        // terminate thread?
    }

    //-------------------------------------------------------------------------
    /**
        Sends a 11 bit CAN data frame.
     
        @param      message     CAN message
      
        @return                 success (true/false) 
    */
    public override bool sendMessage(CANMessage message)
    {
        return MctAdapter_SendMessage(message.getID(), message.getLength(),
            message.getData());
    }

    //-------------------------------------------------------------------------
    /**
        Waits for any message to be received.
      
        @param      timeout     timeout
        @param      msg         message
      
        @return                 message ID if received, 0 otherwise 
    */
    private uint wait_message(uint timeout, out CANMessage msg)
    {
        msg = new CANMessage();
        Debug.Assert(msg != null);

        int wait_cnt = 0;
        uint id;
        byte length;
        ulong data;
        while (wait_cnt < timeout)
        {
            if (MctAdapter_ReceiveMessage(out id, out length, out data))
            {
                // message received
                msg.setID(id);
                msg.setLength(length);
                msg.setData(data);
                
                return id;
            }

            // wait a bit
            Thread.Sleep(1);
            ++wait_cnt;
        }

        // nothing was received
        return 0;
    }

    //-------------------------------------------------------------------------
    /**    
        Handles incoming messages.
    */
    private void read_messages()
    {
        uint id;
        byte length;
        ulong data;

        CANMessage msg = new CANMessage();
        Debug.Assert(msg != null);

        // main loop
        while (true)
        {
            // check tor thread termination request
            Debug.Assert(this.term_mutex != null);
            lock (this.term_mutex)
            {
                if (this.term_requested)
                {
                    return;
                }
            }

            // receive messages
            while (MctAdapter_ReceiveMessage(out id, out length, out data))
            {
                // convert message
                msg.setID(id);
                msg.setLength(length);
                msg.setData(data);

                // pass message to listeners
                lock (this.m_listeners)
                {
                    foreach (ICANListener listener in this.m_listeners)
                    {
                        listener.handleMessage(msg);
                    }
                }
            }

            // give up CPU for a moment
            Thread.Sleep(1);          
        }
    }
};

}   // end namespace
//-----------------------------------------------------------------------------
//  EOF
//-----------------------------------------------------------------------------
