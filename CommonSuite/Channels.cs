using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Microsoft.Win32;

namespace CommonSuite
{
    public class Channels
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        SuiteRegistry _suiteRegistry;

        public Channels(SuiteRegistry suiteRegistry)
        {
            _suiteRegistry = suiteRegistry;
        }

        public bool GetChannelFromRegistry(string key)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());
            bool retval = true;

            using (RegistryKey Settings = SuiteKey.CreateSubKey("Channels"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == key.ToUpper())
                            {
                                retval = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            return retval;
        }


        public void SaveChannelToRegistry(string channelName, bool value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            using (RegistryKey saveSettings = SuiteKey.CreateSubKey("Channels"))
            {
                saveSettings.SetValue(channelName.ToUpper(), value);
            }
        }

        public int GetChannelResolution(string symbolname)
        {
            int numberOfDecimals = 0;
            switch (symbolname)
            {
                    //T7
                case "In.p_AirInlet":
                case "Out.fi_Ignition":
                    //T5
                case "P_Manifold10":
                case "Boost":
                case "Ign_angle":
                case "Boost request":
                case "Target boost":
                case "Boost error":
                case "Boost reduction":
                    //Common?
                case "AFR":
                case "InjectorDC":
                    numberOfDecimals = 2;
                    break;

            }
            return numberOfDecimals;
        }
    }
}
