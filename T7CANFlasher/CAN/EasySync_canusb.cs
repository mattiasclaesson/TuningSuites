using System;
using System.Text;
using System.Runtime.InteropServices;

class EASYSYNC
{
	// CAN message
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CANMsg
    {
        public uint id;
        public uint timestamp;
        public byte flags;
        public byte len;
        public ulong data;
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
    public const int ERROR_CANUSB_FAIL = -1;                  // Failed
	public const int ERROR_CANUSB_OPEN_SUBSYSTEM = -2;     // Problems with driver subsystem
	public const int ERROR_CANUSB_COMMAND_SUBSYSTEM = -3;  // Unable to send command to adapter
	public const int ERROR_CANUSB_NOT_OPEN = -4;           // Channel not open
	public const int ERROR_CANUSB_TX_FIFO_FULL = -5;       // Transmit fifo full
	public const int ERROR_CANUSB_INVALID_PARAM = -6;      // Invalid parameter
	public const int ERROR_CANUSB_NO_MESSAGE = -7;         // No message available
    public const int ERROR_CANUSB_MEMORY_ERROR = -8;
    public const int ERROR_CANUSB_NO_DEVICE = -9;
    public const int ERROR_CANUSB_TIMEOUT = -10;
    public const int ERROR_CANUSB_INVALID_HARDWARE = -11;
    #endregion

	// Msg Type:
	public const byte CANMSG_EXTENDED = 0x80;				// Extended Frame
	public const byte CANMSG_RTR = 0x40;					// RTR Frame

	// Open flags
    public const uint CANUSB_FLAG_TIMESTAMP = 0x10000000;

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

    [DllImport("USBCanPlusDllF.dll", EntryPoint="canplus_Open")]
    public static extern uint canusb_Open( string szID, string szBitrate, string acceptance_code, string acceptance_mask, uint flags );
    
    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Open")]
    public static extern int canusb_Open(IntPtr szID, string szBitrate, IntPtr acceptance_code, IntPtr acceptance_mask, uint flags);

	// Version of the above to be able to pass a null pointer
	// IntPtr.Zero is passed for szID to open first available driver
    /*[DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Open")]
	public static extern uint canusb_Open( IntPtr szID, string szBitrate, uint acceptance_code, uint acceptance_mask, uint flags );*/

	///////////////////////////////////////////////////////////////////////////////
	// canusb_Close
	//
	// Close CANUSB interface with handle h.
	//
	// Returns <= 0 on failure. > 0 on success.

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Close")]
	public static extern int canusb_Close( int handle );

	
	///////////////////////////////////////////////////////////////////////////////
	// canusb_Read
	//
	// Read message from channel with handle h.
	//
	// Returns <= 0 on failure. >0 on success.
	//

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Read")]
	public static extern int canusb_Read( int handle, out CANMsg msg );

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Flush")]
    public static extern int canusb_Flush(int handle);

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Reset")]
    public static extern int canusb_Reset(int handle);

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Listen")]
    public static extern int canusb_Listen(int handle);

	
	///////////////////////////////////////////////////////////////////////////////
	// canusb_Write
	//
	// Write message to channel with handle h.
	//
	// Returns <= 0 on failure. >0 on success.
	//

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Write")]
    public static extern int canusb_Write(int handle, ref CANMsg msg);


	///////////////////////////////////////////////////////////////////////////////
	// canusb_Status
	//
	// Get CANUSB hardware status for channel with handle h.

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_Status")]
	public static extern int canusb_Status( int handle );


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

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_VersionInfo")]
    public static extern int canusb_VersionInfo(int handle, StringBuilder verinfo);

    [DllImport("USBCanPlusDllF.dll", EntryPoint = "canplus_getFirstAdapter", CharSet = CharSet.Ansi)]
    public static extern int canusb_getFirstAdapter(StringBuilder adapter, int size);

}