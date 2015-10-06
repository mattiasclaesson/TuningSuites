using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Trionic5Tools
{
    public class LogFilters
    {
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
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            // als "T5Suite2" niet bestaat, eerst inlezen uit "T5SuitePro" en dan meteen weer opslaan
            try
            {
                RegistryKey testKey = TempKey.OpenSubKey("T5Suite2");
                using (RegistryKey Settings = TempKey.CreateSubKey("T5Suite2\\LogFilters"))
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
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception E2)
            {
                Console.WriteLine(E2.Message);
            }
            return filters;
        }


        private void SaveFilter(LogFilter filter)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            if (filter.Symbol != "")
            {
                using (RegistryKey saveSettings = TempKey.CreateSubKey("T5Suite2\\LogFilters\\" + filter.Index.ToString()))
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
            RegistryKey TempKey = null;
            if (index != "")
            {
                TempKey = Registry.CurrentUser.CreateSubKey("Software");
                using (RegistryKey Settings = TempKey.CreateSubKey("T5Suite2\\LogFilters\\" + index))
                {
                    try
                    {
                        filter.Index = Convert.ToInt32(index);
                        filter.Active = Convert.ToBoolean(Settings.GetValue("active").ToString());
                        filter.Type = (Trionic5Tools.LogFilter.MathType)Convert.ToInt32(Settings.GetValue("type"));
                        filter.Symbol = Settings.GetValue("symbol").ToString();
                        filter.Value = (float)ConvertToDouble(Settings.GetValue("value").ToString());
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
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
