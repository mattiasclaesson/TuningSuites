using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Trionic5Tools
{
    public class CombiAdapterMonitor
    {
        public delegate void AdapterPresent(object sender, AdapterEventArgs e);
        public event CombiAdapterMonitor.AdapterPresent onAdapterPresent;

        public class AdapterEventArgs : System.EventArgs
        {
            private bool _present;

            public bool Present
            {
                get { return _present; }
                set { _present = value; }
            }
            public AdapterEventArgs(bool present)
            {
                this._present = present;
            }
        }

        private System.Timers.Timer tmr = new System.Timers.Timer(1000);

        private bool _adapterPresent = false; // assume no adapter

        public CombiAdapterMonitor()
        {
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
        }

        public void Start()
        {
            tmr.Start();
        }

        public void Stop()
        {
            tmr.Stop();
        }

        private void CastAdapterPresentEvent(bool adapterPresent)
        {
            if (onAdapterPresent != null)
            {
                onAdapterPresent(this, new AdapterEventArgs(adapterPresent));   
            }
        }

        void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (CheckDevice(GUID_COMBIADAPTER))
                {
                    if (!_adapterPresent)
                    {
                        _adapterPresent = true;
                        CastAdapterPresentEvent(_adapterPresent);
                    }
                }
                else
                {
                    if (_adapterPresent)
                    {
                        _adapterPresent = false;
                        CastAdapterPresentEvent(_adapterPresent);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        //219D0508-57A8-4ff5-97A1-BD86587C6C7E
        public static Guid GUID_FTDI_BASE =    new Guid("219D0508-57A8-4ff5-97A1-BD86587C6C7E"); 

        public static Guid GUID_COMBIADAPTER = new Guid("78A1C341-4539-11D3-B88D-00C04FAD5171");

        bool CheckDevice(Guid GuidToCheck)
        {
            IntPtr devinfo = Win32.SetupDiGetClassDevs(ref GuidToCheck, IntPtr.Zero, IntPtr.Zero, Win32.DIGCF_PRESENT);
            Win32.SP_DEVINFO_DATA devInfoSet = new Win32.SP_DEVINFO_DATA();
            devInfoSet.cbSize = Marshal.SizeOf(typeof(Win32.SP_DEVINFO_DATA));
            if (Win32.SetupDiEnumDeviceInfo(devinfo, 0, ref devInfoSet)) return true;
            return false;
        }
    }

    class Win32
    {
        public const int
        WM_DEVICECHANGE = 0x0219;
        public const int
        DBT_DEVICEARRIVAL = 0x8000,
        DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int
        DEVICE_NOTIFY_WINDOW_HANDLE = 0,
        DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int
        DBT_DEVTYP_DEVICEINTERFACE = 5;
        public static Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");//HID
        public static Guid GUID_DEVINTERFACE_ALL = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");//All devices
        public static Guid GUID_DEVCLASS_MOUSE = new Guid("4D36E96F-E325-11CE-BFC1-08002BE10318");
        public static Guid GUID_DEVCLASS_KEYBOARD = new Guid("4D36E96B-E325-11CE-BFC1-08002BE10318");

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            public short dbcc_name;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(
        IntPtr hRecipient,
        IntPtr NotificationFilter,
        Int32 Flags);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();


        public const int DIGCF_PRESENT = 2;


        [DllImport("setupapi.dll")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hWndParent, int Flags);

        [DllImport("setupapi.dll")]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int Supplies, ref SP_DEVINFO_DATA DeviceInfoData);

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public int Reserved;
        }
    }
}
