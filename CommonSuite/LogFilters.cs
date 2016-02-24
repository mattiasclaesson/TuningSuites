using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using NLog;

namespace CommonSuite
{
    public class LogFilters
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        SuiteRegistry _suiteRegistry;

        public LogFilters(SuiteRegistry suiteRegistry)
        {
            _suiteRegistry = suiteRegistry;
        }

        public void SaveFiltersToRegistry(LogFilterCollection filters)
        {
            foreach (LogFilter filter in filters)
            {
                SaveFilter(filter);
            }
        }

        public LogFilterCollection GetFiltersFromRegistry()
        {
            LogFilterCollection filters = new LogFilterCollection();
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());
            try
            {
                using (RegistryKey Settings = SuiteKey.CreateSubKey("LogFilters"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetSubKeyNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                LogFilter filter = LoadFilter(a);
                                filters.Add(filter);
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception E2)
            {
                logger.Debug(E2.Message);
            }
            return filters;
        }


        private void SaveFilter(LogFilter filter)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            if (filter.Symbol != "")
            {
                using (RegistryKey saveSettings = SuiteKey.CreateSubKey("LogFilters\\" + filter.Index.ToString()))
                {
                    saveSettings.SetValue("value", filter.Value.ToString());
                    saveSettings.SetValue("type", (int)filter.Type);
                    saveSettings.SetValue("index", filter.Index);
                    saveSettings.SetValue("symbol", filter.Symbol);
                    saveSettings.SetValue("active", filter.Active);
                }
            }
        }


        private LogFilter LoadFilter(string index)
        {
            LogFilter filter = new LogFilter();
            if (index != "")
            {
                RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
                RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
                RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());
                using (RegistryKey Settings = SuiteKey.CreateSubKey("LogFilters\\" + index))
                {
                    try
                    {
                        filter.Index = Convert.ToInt32(index);
                        filter.Active = Convert.ToBoolean(Settings.GetValue("active").ToString());
                        filter.Type = (LogFilter.MathType)Convert.ToInt32(Settings.GetValue("type"));
                        filter.Symbol = Settings.GetValue("symbol").ToString();
                        filter.Value = (float)ConvertToDouble(Settings.GetValue("value").ToString());
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                }
            }
            return filter;

        }


        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }
    }
}
