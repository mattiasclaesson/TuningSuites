//------------------------------------------------------------------------------
//  TITLE :- CANdo.dll import wrapper - CANdoImport.cs
//  AUTHOR :- Martyn Brown
//  DATE :- 19/02/06
//
//  DESCRIPTION :- 'C#' class to wrap up the properties & methods exported by
//  CANdo.dll.
//
//  UPDATES :-
//	19/02/06 Created
//
//  (c) 2006 Netronics Ltd. All rights reserved.
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CANdoCSharp
{
  class CANdoImport
  {
    // CANdo constants
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
  }
}
