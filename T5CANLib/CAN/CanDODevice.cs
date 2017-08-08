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
public class CanDODevice : ICANDevice
{
    public const byte CLOSED = 0;  // USB channel closed
    public const byte OPEN = 1;  // USB channel open
    public const byte STOP = 0;  // Stop Rx/Tx of CAN messages
    public const byte RUN = 1;  // Start Rx/Tx of CAN messages
    public const byte CLEAR = 0;  // Status message flag clear
    public const byte SET = 1;  // Status message flag set
    public const byte NORMAL_MODE = 0;  // Rx/Tx CAN mode
    public const byte LISTEN_ONLY_MODE = 1;  // Rx only mode, no ACKs
    public const byte LOOPBACK_MODE = 2;  // Tx internally looped back to Rx

    // CAN message constants
    public const byte ID_11_BIT = 0;  // Standard 11 bit ID
    public const byte ID_29_BIT = 1;  // Extended 29 bit ID
    public const byte DATA_FRAME = 0;  // CAN data frame
    public const byte REMOTE_FRAME = 1;  // CAN remote frame

    // CAN receive cyclic buffer size
    public const int CAN_BUFFER_LENGTH = 2048;

    // Function return values
    public const int CANDO_SUCCESS = 0x0000;  // All OK
    public const int CANDO_USB_DLL_ERROR = 0x0001;  // Error loading USB DLL
    public const int CANDO_NOT_FOUND = 0x0002;  // CANdo not found
    public const int CANDO_IO_FAILED = 0x0004;  // Failed to initialise USB parameters
    public const int CANDO_CLOSED = 0x0008;  // No CANdo channel open
    public const int CANDO_READ_ERROR = 0x0010;  // USB read error
    public const int CANDO_WRITE_ERROR = 0x0020;  // USB write error
    public const int CANDO_WRITE_INCOMPLETE = 0x0040;  // Not all requested bytes written to CANdo
    public const int CANDO_BUFFER_OVERFLOW = 0x0080;  // Overflow in cyclic buffer
    public const int CANDO_RX_OVERRUN = 0x0100;  // Message received greater than max. message size
    public const int CANDO_RX_TYPE_UNKNOWN = 0x0200;  // Unknown message type received
    public const int CANDO_RX_CRC_ERROR = 0x0400;  // CRC mismatch
    public const int CANDO_RX_DECODE_ERROR = 0x0800;  // Error decoding message
    public const int CANDO_ERROR = 0x8000;  // Non specific error

    // Structure used to store USB info. for CANdo
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class TCANdoUSB
    {
      public const int CANDO_STRING_LENGTH = 256;
      public int TotalNo;  // Total no. of CANdo on USB bus
      public int No;  // No. of this CANdo
      public byte OpenFlag;  // USB communications channel state
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CANDO_STRING_LENGTH)]
      public string Description;  // USB decriptor string for CANdo
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CANDO_STRING_LENGTH)]
      public string SerialNo;  // USB S/N for this CANdo
      public IntPtr Handle;  // Handle to connected CANdo
    }

    // Structure used to store a CAN message
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TCANdoCAN
    {
      public const byte CAN_DATA_LENGTH = 8;
      public byte IDE;
      public byte RTR;
      public uint ID;
      public byte DLC;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = CAN_DATA_LENGTH)]
      public byte[] Data;
      public byte BusState;
      public uint TimeStamp;
    }

    // Structure used to store cyclic buffer control parameters for CAN messages received from CANdo
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TCANdoCANBufferControl
    {
      public int WriteIndex;
      public int ReadIndex;
      public byte FullFlag;
    }

    // Structure used to store status information received from CANdo
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class TCANdoStatus
    {
      public byte HardwareVersion;
      public byte SoftwareVersion;
      public byte Status;
      public byte BusState;
      public int TimeStamp;
      public byte NewFlag;
    }

    // Functions imported from 'CANdo.dll'
    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoOpen(
      [OutAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer);

    [DllImportAttribute("CANdo.dll")]
    public static extern void CANdoClose(
      [OutAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoFlushBuffers(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoSetBaudRate(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      byte SJW, byte BRP, byte PHSEG1, byte PHSEG2, byte PROPSEG, byte SAM);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoSetMode(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      byte Mode);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoSetFilters(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      uint Rx1Mask,
      byte Rx1IDE1, uint Rx1Filter1,
      byte Rx1IDE2, uint Rx1Filter2,
      uint Rx2Mask,
      byte Rx2IDE1, uint Rx2Filter1,
      byte Rx2IDE2, uint Rx2Filter2,
      byte Rx2IDE3, uint Rx2Filter3,
      byte Rx2IDE4, uint Rx2Filter4);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoSetState(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      byte State);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoReceive(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      IntPtr CANdoCANBufferPointer,
      [OutAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoStatus CANdoStatusPointer);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoTransmit(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer,
      byte IDExtended, uint ID, byte RTR, byte DLC,
      [InAttribute, MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]
			byte[] Data,
      byte BufferNo, byte RepeatTime);

    [DllImportAttribute("CANdo.dll")]
    public static extern int CANdoRequestStatus(
      [InAttribute, MarshalAs(UnmanagedType.LPStruct)]
			TCANdoUSB CANdoUSBPointer);

    // internal state
    private Thread read_thread;                     ///< reader thread
    private bool term_requested = false;            ///< thread termination flag
    private Object term_mutex = new Object();       ///< mutex for termination flag
    private TCANdoUSB CANdoUSB;
    private TCANdoStatus CANdoStatus;
    int CANDataSize, CANBufferControlSize, CANBufferSize;
    IntPtr CANBufferPointer, CANDataPointer, CANBufferControlPointer;
    TCANdoCAN[] CANDataBuffer;  // Cyclic buffer store for CAN receive messages
    TCANdoCANBufferControl CANBufferControl;  // Cyclic buffer control
    Type CANDataType, CANBufferControlType;


    //-------------------------------------------------------------------------
    /**
        Default constructor.
    */
    public CanDODevice()
    {
        // create adapter object
        CANdoUSB = new TCANdoUSB();
        CANdoUSB.OpenFlag = CLOSED;

        // Create a store for CAN receive messages
        CANDataBuffer = new TCANdoCAN[CAN_BUFFER_LENGTH];  // Cyclic buffer data
        CANBufferControl = new TCANdoCANBufferControl();  // Cyclic buffer control

        CANDataType = typeof(TCANdoCAN);
        CANDataSize = Marshal.SizeOf(CANDataType);

        CANBufferControlType = typeof(TCANdoCANBufferControl);
        CANBufferControlSize = Marshal.SizeOf(CANBufferControlType);

        CANBufferSize = CANDataBuffer.Length * CANDataSize + CANBufferControlSize;
        CANBufferPointer = Marshal.AllocHGlobal(CANBufferSize);  // Allocate unmanaged memory for the CAN receive message cyclic buffer
        CANBufferControlPointer = (IntPtr)(((int)CANBufferPointer) + CANDataBuffer.Length * CANDataSize);
        CANBufferControl.WriteIndex = 0;  // Reset cyclic buffer
        CANBufferControl.ReadIndex = 0;
        CANBufferControl.FullFlag = 0;
        Marshal.StructureToPtr(CANBufferControl, CANBufferControlPointer, true);  // Update unmanaged memory

        // Create a store for status message
        CANdoStatus = new TCANdoStatus();

        // create reader thread
        this.read_thread = new Thread(this.read_messages);
        Debug.Assert(read_thread != null);
    }

    //-------------------------------------------------------------------------
    /**
        Destructor.
    */
    ~CanDODevice()
    {
        //MctAdapter_Release();
    }

    override public void EnableLogging(string path2log)
    {
    }

    override public void DisableLogging()
    {
     
    }

    public override void Delete()
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
    override public void clearTransmitBuffer()
    {
        //  implement clearTransmitBuffer
    }

    override public void clearReceiveBuffer()
    {
        // implement clearReceiveBuffer
    }

    public override void setPortNumber(string portnumber)
    {
        //nothing to do
    }


    //-------------------------------------------------------------------------
    /**
        Opens a connection to CAN interface. 

        @return             result 
    */

    private bool SetBaudRate()
    {
        /*byte BRP, PHSEG1, PHSEG2, PROPSEG;

        // Set CANdo baud rate
        switch (comboBox1.SelectedIndex)
        {
            case 0:
                // 62.5k
                BRP = 7;
                PHSEG1 = 7;
                PHSEG2 = 7;
                PROPSEG = 2;
                break;

            case 1:
                // 125k
                BRP = 3;
                PHSEG1 = 7;
                PHSEG2 = 7;
                PROPSEG = 2;
                break;

            case 2:
                // 250k
                BRP = 1;
                PHSEG1 = 7;
                PHSEG2 = 7;
                PROPSEG = 2;
                break;

            case 3:
                // 500k
                BRP = 0;
                PHSEG1 = 7;
                PHSEG2 = 7;
                PROPSEG = 2;
                break;

            case 4:
                // 1M
                BRP = 0;
                PHSEG1 = 2;
                PHSEG2 = 2;
                PROPSEG = 2;
                break;

            default:
                // 250k
                BRP = 1;
                PHSEG1 = 7;
                PHSEG2 = 7;
                PROPSEG = 2;
                break;
        }

        if (CANdoSetBaudRate(CANdoUSB, 0, BRP, PHSEG1, PHSEG2, PROPSEG, 0) == CANDO_SUCCESS)
        {
            Thread.Sleep(100);  // Wait 100ms to allow CANdo to store settings in EEPROM if changed
            return true;  // Baud rate set
        }
        else
            return false;  // Error*/
        return true;
    }
    public override OpenResult open()
    {
        OpenResult result = OpenResult.OpenError;
        if (CANdoUSB.OpenFlag == CLOSED)
        {
            try
            {
                if (CANdoOpen(CANdoUSB) == CANDO_SUCCESS)
                {
                    result = OpenResult.OK;
                    SetBaudRate();

                }
                else
                {
                    CANdoClose(CANdoUSB);  // Unload CANdo.dll
                    result = OpenResult.OpenError;
                }
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Dll not found");
            }
        }

        if (result == OpenResult.OK)
        {
            // start reader thread
            Debug.Assert(this.read_thread != null);
            if (this.read_thread.ThreadState != System.Threading.ThreadState.Running)
            {
                this.read_thread.Start();
            }
        }

        return result;
    }

    //-------------------------------------------------------------------------
    /**
        Determines if connection to CAN device is open.
    
        return          open (true/false)
    */
    public override bool isOpen()
    {
        if (CANdoUSB.OpenFlag == OPEN)
        {
            return true;
        }

        return false;
    }

    //-------------------------------------------------------------------------
    /**
        Closes the connection to CAN interface.
     
        return          success (true/false)
    */
    public override CloseResult close()
    {
        //return MctAdapter_Close() ? CloseResult.OK : CloseResult.CloseError;
        if (CANdoUSB.OpenFlag == OPEN)
        {
             CANdoClose(CANdoUSB);
             return CloseResult.OK;
        }
        return CloseResult.CloseError;
    
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
        //CANdoTransmit(CANdoUSB, ID_11_BIT, message.getID(), DATA_FRAME, 0, message.getData(), 
        //return MctAdapter_SendMessage(message.getID(), message.getLength(),
            //message.getData());
        return true;
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
        return 0;
       /* msg = new CANMessage();
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
        */
    }

    //-------------------------------------------------------------------------
    /**    
        Handles incoming messages.
    */
    private void read_messages()
    {
        /*uint id;
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
        }*/
    }
};

}   // end namespace
//-----------------------------------------------------------------------------
//  EOF
//-----------------------------------------------------------------------------
