using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Trionic5Tools
{
    public enum AdapterType : int
    {
        USB_BDM,
        LAWICEL
    }

    public class FTDIAdapterMonitor
    {
        public delegate void AdapterPresent(object sender, AdapterEventArgs e);
        public event FTDIAdapterMonitor.AdapterPresent onAdapterPresent;

        public class AdapterEventArgs : System.EventArgs
        {
            private AdapterType _type;

            public AdapterType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            private bool _present;

            public bool Present
            {
                get { return _present; }
                set { _present = value; }
            }
            public AdapterEventArgs(bool present, AdapterType type)
            {
                this._present = present;
                this._type = type;
            }
        }

        private System.Timers.Timer tmr = new System.Timers.Timer(1000);

        private bool _adapterPresent = false; // assume no adapter
        private bool _BDMadapterPresent = false; // assume no adapter

        public FTDIAdapterMonitor()
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

        private void CastAdapterPresentEvent(bool adapterPresent, AdapterType type)
        {
            if (onAdapterPresent != null)
            {
                onAdapterPresent(this, new AdapterEventArgs(adapterPresent, type));
            }
        }

        void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                tmr.Enabled = false;

                if (CheckDevice(USB_BDM_LOCATION))
                {
                    if (!_BDMadapterPresent)
                    {
                        _BDMadapterPresent = true;
                        CastAdapterPresentEvent(_BDMadapterPresent, AdapterType.USB_BDM);
                    }
                }
                else
                {
                    if (_BDMadapterPresent)
                    {
                        _BDMadapterPresent = false;
                        CastAdapterPresentEvent(_BDMadapterPresent, AdapterType.USB_BDM);
                    }
                }
                if (CheckDevice(LAWICEL_LOCATION))
                {
                    if (!_adapterPresent)
                    {
                        _adapterPresent = true;
                        CastAdapterPresentEvent(_adapterPresent, AdapterType.LAWICEL);
                    }
                }
                else
                {
                    if (_adapterPresent)
                    {
                        _adapterPresent = false;
                        CastAdapterPresentEvent(_adapterPresent, AdapterType.LAWICEL);
                    }
                }
                tmr.Enabled = true;
            }
            catch (Exception)
            {

            }
        }
        public static string USB_BDM_LOCATION = @"{219d0508-57a8-4ff5-97a1-bd86587c6c7e}\##?#USB#Vid_0403&Pid_6001#A600csOc#{219d0508-57a8-4ff5-97a1-bd86587c6c7e}\Control";
        public static string LAWICEL_LOCATION = @"{219d0508-57a8-4ff5-97a1-bd86587c6c7e}\##?#USB#Vid_0403&Pid_ffa8#LWSZ26UZ#{219d0508-57a8-4ff5-97a1-bd86587c6c7e}\Control";

        bool CheckDevice(string subkeyName)
        {
            // search registry
            bool retval = false;
            RegistryKey TempKey = null;
            TempKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\ControlSet001\Control\DeviceClasses");
            try
            {
                using (RegistryKey Settings = TempKey.CreateSubKey(subkeyName))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                if (a == "ReferenceCount")
                                {
                                    int testvalue = Convert.ToInt32(Settings.GetValue(a).ToString());
                                    if (testvalue > 0) retval = true;
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;

        }
    }

}
