using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using NLog;

namespace CommonSuite
{
    public class LogWorks
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetLogWorksPathFromRegistry()
        {
            RegistryKey TempKey = null;
            string foundvalue = string.Empty;
            TempKey = Registry.LocalMachine;

            using (RegistryKey Settings = TempKey.OpenSubKey("SOFTWARE\\Classes\\d32FileHandler\\Shell\\Open\\Command"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            foundvalue = Settings.GetValue(a).ToString();
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
            }

            if (foundvalue == string.Empty)
            {
                using (RegistryKey Settings = TempKey.OpenSubKey("SOFTWARE\\Classes\\Applications\\LogWorks2.exe\\shell\\Open\\Command"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                foundvalue = Settings.GetValue(a).ToString();
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                }
            }

            if (foundvalue == string.Empty)
            {
                using (RegistryKey Settings = TempKey.OpenSubKey("SOFTWARE\\Classes\\d32.File\\shell\\open\\command"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        try
                        {
                            foundvalue = Settings.GetValue(vals[0]).ToString();
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
            }

            if (foundvalue == string.Empty)
            {
                using (RegistryKey Settings = TempKey.OpenSubKey("HKEY_CLASSES_ROOT\\Applications\\LogWorks3.exe\\shell\\open\\command"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        try
                        {
                            foundvalue = Settings.GetValue(vals[0]).ToString();
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
            }

            if (foundvalue != string.Empty)
            {
                foundvalue = foundvalue.Substring(1, foundvalue.Length - 1);
                int idx = foundvalue.IndexOf('\"');
                if (idx > 0)
                {
                    foundvalue = foundvalue.Substring(0, idx);
                    return foundvalue;
                }
            }

            return foundvalue;
        }
    }
}
