// Lawicel_canusb.cs
//
// C# Declarations for LAWICEL CANUSB DLL Driver
//
// http://www.canusb.com
// (c) 2005 Lawicel HB, Sweden
// Rev. 0.0.1, 2005-06-09
//
// This C# Software is NO freeware/shareware!
//
// You are only permitted to use & modify this software if you
// own a CANUSB from LAWICEL HB, Sweden.
//
// Do not use this C# Demo software or parts of it to communicate
// with other CAN hardware other than the LAWICEL CANUSB Hardware.
//

using System;
using System.Text;
using System.Runtime.InteropServices;

class LAWICEL
{
	// CAN message
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct CANMsg
	{
		public uint id;			// 11/29 bit Identifier
		public uint timestamp;  // Hardware Timestamp (0-9999mS)
		public byte flags;		// Message Flags
		public byte len;		// Number of data bytes 0-8
		public ulong data;		// Data Bytes 0..7
	}

	#region Bitrate Codes
	// BTR0+BTR1 register
	// Register values for BTR0/BTR1
	public const string CAN_BAUD_BTR_1M = "0x00:0x14";     //   1 MBit/sec
	public const string CAN_BAUD_BTR_500K = "0x00:0x1C";   // 500 KBit/sec
	public const string CAN_BAUD_BTR_250K = "0x01:0x1C";   // 250 KBit/sec
	public const string CAN_BAUD_BTR_125K = "0x03:0x1C";   // 125 KBit/sec
	public const string CAN_BAUD_BTR_100K = "0x43:0x2F";   // 100 KBit/sec
	public const string CAN_BAUD_BTR_50K = "0x47:0x2F";    //  50 KBit/sec
	public const string CAN_BAUD_BTR_20K = "0x53:0x2F";    //  20 KBit/sec
	public const string CAN_BAUD_BTR_10K = "0x67:0x2F";    //  10 KBit/sec
	public const string CAN_BAUD_BTR_5K = "0x7F:0x7F";     //   5 KBit/sec
	
	// Baudrate can also be set with "real" value if set to one of the
	// values below
	public const string CAN_BAUD_1M = "1000";				//   1 MBit / s
	public const string CAN_BAUD_800K = "800";				// 800 kBit / s
	public const string CAN_BAUD_500K = "500";				// 500 kBit / s
	public const string CAN_BAUD_250K = "250";				// 250 kBit / s
	public const string CAN_BAUD_125K = "125";				// 125 kBit / s
	public const string CAN_BAUD_100K = "100";				// 100 kBit / s
	public const string CAN_BAUD_50K = "50";				//  50 kBit / s
	public const string CAN_BAUD_20K = "20";				//  20 kBit / s
	public const string CAN_BAUD_10K = "10";				//  10 kBit / s
	#endregion

	#region Error Codes
	//  error return codes
	public const int ERROR_CANUSB_OK = 1;                  // All is OK
	public const int ERROR_CANUSB_OPEN_SUBSYSTEM = -2;     // Problems with driver subsystem
	public const int ERROR_CANUSB_COMMAND_SUBSYSTEM = -3;  // Unable to send command to adapter
	public const int ERROR_CANUSB_NOT_OPEN = -4;           // Channel not open
	public const int ERROR_CANUSB_TX_FIFO_FULL = -5;       // Transmit fifo full
	public const int ERROR_CANUSB_INVALID_PARAM = -6;      // Invalid parameter
	public const int ERROR_CANUSB_NO_MESSAGE = -7;         // No message available
	#endregion

	// Msg Type:
	public const byte CANMSG_EXTENDED = 0x80;				// Extended Frame
	public const byte CANMSG_RTR = 0x40;					// RTR Frame

	// Open flags
	public const byte CANUSB_FLAG_TIMESTAMP = 0x00000001;

	// Filter mask settings
	// Use codes below to receive all frames
	public const uint CANUSB_ACCEPTANCE_CODE_ALL = 0x00000000;
	public const uint CANUSB_ACCEPTANCE_MASK_ALL = 0xFFFFFFFF;

	///////////////////////////////////////////////////////////////////////////////
	// cansub_Open
	//
	//
	// Open CANUSB interface to device
	//
	// Returs handle to device if open was successfull or zero on falure.
	//
	//
	// szID
	// ====
	// USB Serial number for adapter or NULL to open the first found.
	// Serial number is not CANUSB HW#, it is USB chip #
	//
	//
	// szBitrate
	// =========
	// "10" for 10kbps
	// "20" for 20kbps
	// "50" for 50kbps
	// "100" for 100kbps
	// "250" for 250kbps
	// "500" for 500kbps
	// "800" for 800kbps
	// "1000" for 1Mbps
	//
	// or
	//
	// btr0:btr1 pair  ex. "0x03:0x1c" or 3:28
	//
	// acceptance_code
	// ===============
	// Set to CANUSB_ACCEPTANCE_CODE_ALL to  get all messages.
	//
	// acceptance_mask
	// ===============
	// Set to CANUSB_ACCEPTANCE_MASk_ALL to  get all messages.
	//
	// flags
	// =====
	// CANUSB_FLAG_TIMESTAMP - Timestamp will be set by adapter.
	//

	[DllImport("canusbdrv.dll", EntryPoint="canusb_Open")]
	public static extern uint canusb_Open( string szID, string szBitrate, uint acceptance_code, uint acceptance_mask, uint flags );

	// Version of the above to be able to pass a null pointer
	// IntPtr.Zero is passed for szID to open first available driver
	[DllImport("canusbdrv.dll", EntryPoint="canusb_Open")]
	public static extern uint canusb_Open( IntPtr szID, string szBitrate, uint acceptance_code, uint acceptance_mask, uint flags );

	///////////////////////////////////////////////////////////////////////////////
	// canusb_Close
	//
	// Close CANUSB interface with handle h.
	//
	// Returns <= 0 on failure. > 0 on success.

	[DllImport("canusbdrv.dll", EntryPoint="canusb_Close")]
	public static extern int canusb_Close( uint handle );

	
	///////////////////////////////////////////////////////////////////////////////
	// canusb_Read
	//
	// Read message from channel with handle h.
	//
	// Returns <= 0 on failure. >0 on success.
	//

	[DllImport("canusbdrv.dll", EntryPoint="canusb_Read")]
	public static extern int canusb_Read( uint handle, out CANMsg msg );

	
	///////////////////////////////////////////////////////////////////////////////
	// canusb_Write
	//
	// Write message to channel with handle h.
	//
	// Returns <= 0 on failure. >0 on success.
	//

	[DllImport("canusbdrv.dll", EntryPoint="canusb_Write")]
	public static extern int canusb_Write( uint handle, ref CANMsg msg );


	///////////////////////////////////////////////////////////////////////////////
	// canusb_Status
	//
	// Get CANUSB hardware status for channel with handle h.

	[DllImport("canusbdrv.dll", EntryPoint="canusb_Status")]
	public static extern int canusb_Status( uint handle );


	///////////////////////////////////////////////////////////////////////////////
	// canusb_VersionInfo
	//
	// Get hardware/firmware and driver version for channel with handle h.
	//
	// Returns <= 0 on failure. > 0 on success.
	//
	// Format
	//  "Hardware_Major.Hardware_Minor;Firmware_Major.Firmware_Minor;Driver_Major.Driver_Minor"
	//

	[DllImport("canusbdrv.dll", EntryPoint="canusb_VersionInfo")]
	public static extern int canusb_VersionInfo( uint handle, StringBuilder verinfo );

}