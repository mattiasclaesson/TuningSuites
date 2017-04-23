using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using NLog;
using System.Drawing;

namespace CommonSuite
{
    public class SymbolColors
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        SuiteRegistry _suiteRegistry;

        public SymbolColors(SuiteRegistry suiteRegistry)
        {
            _suiteRegistry = suiteRegistry;

            CheckDefaultSymbolColors();
        }

        public void SaveColorToRegistry(string symbolname, Color c)
        {
            int win32color = System.Drawing.ColorTranslator.ToWin32(c);
            if (win32color != 0)
            {
                SaveRegistrySetting(symbolname, win32color);
            }
        }

        public void AddColorsFromRegistry(SymbolCollection sc)
        {
            foreach (SymbolHelper sh in sc)
            {
                sh.Color = GetColorFromRegistry(sh.SmartVarname);
            }
        }

        public Color GetColorFromRegistry(string symbolname)
        {
            Color c = Color.Black;
            Int32 win32color = GetValueFromRegistry(symbolname);
            c = System.Drawing.ColorTranslator.FromWin32(win32color);
            return c;
        }

        private void CheckDefaultSymbolColors()
        {
            if (GetValueFromRegistry("ActualIn.n_Engine") == 0) SaveColorToRegistry("ActualIn.n_Engine", Color.LightCyan);
            if (GetValueFromRegistry("ActualIn.T_Engine") == 0) SaveColorToRegistry("ActualIn.T_Engine", Color.LightSalmon);
            if (GetValueFromRegistry("ActualIn.T_AirInlet") == 0) SaveColorToRegistry("ActualIn.T_AirInlet", Color.LightBlue);
            if (GetValueFromRegistry("ECMStat.ST_ActiveAirDem") == 0) SaveColorToRegistry("ECMStat.ST_ActiveAirDem", Color.Lime);
            if (GetValueFromRegistry("Lambda.Status") == 0) SaveColorToRegistry("Lambda.Status", Color.Azure);

            if (GetValueFromRegistry("IgnMastProt.fi_Offset") == 0) SaveColorToRegistry("IgnMastProt.fi_Offset", Color.Pink); // T8
            if (GetValueFromRegistry("IgnProt.fi_Offset") == 0) SaveColorToRegistry("IgnProt.fi_Offset", Color.Pink); //T7

            if (GetValueFromRegistry("AirMassMast.m_Request") == 0) SaveColorToRegistry("AirMassMast.m_Request", Color.Orange); // T8
            if (GetValueFromRegistry("m_Request") == 0) SaveColorToRegistry("m_Request", Color.Orange); // T7

            if (GetValueFromRegistry("Out.M_EngTrqAct") == 0) SaveColorToRegistry("Out.M_EngTrqAct", Color.DarkGray); // T8
            if (GetValueFromRegistry("Out.M_Engine") == 0) SaveColorToRegistry("Out.M_Engine", Color.DarkGray); //T7

            if (GetValueFromRegistry("ECMStat.P_Engine") == 0) SaveColorToRegistry("ECMStat.P_Engine", Color.LightSlateGray);

            if (GetValueFromRegistry("In.p_AirInlet") == 0) SaveColorToRegistry("In.p_AirInlet", Color.Red); // T8

            if (GetValueFromRegistry("ECMStat.p_Diff") == 0) SaveColorToRegistry("ECMStat.p_Diff", Color.Red);
            if (GetValueFromRegistry("Out.PWM_BoostCntrl") == 0) SaveColorToRegistry("Out.PWM_BoostCntrl", Color.Wheat);
            if (GetValueFromRegistry("Out.fi_Ignition") == 0) SaveColorToRegistry("Out.fi_Ignition", Color.LightGoldenrodYellow);
            if (GetValueFromRegistry("Out.X_AccPos") == 0) SaveColorToRegistry("Out.X_AccPos", Color.Crimson);
            if (GetValueFromRegistry("MAF.m_AirInlet") == 0) SaveColorToRegistry("MAF.m_AirInlet", Color.OrangeRed);
            if (GetValueFromRegistry("In.v_Vehicle") == 0) SaveColorToRegistry("In.v_Vehicle", Color.Green);
            if (GetValueFromRegistry("Exhaust.T_Calc") == 0) SaveColorToRegistry("Exhaust.T_Calc", Color.Goldenrod);
            if (GetValueFromRegistry("BFuelProt.CurrentFuelCon") == 0) SaveColorToRegistry("BFuelProt.CurrentFuelCon", Color.White);
        }

        private void SaveRegistrySetting(string key, int value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            using (RegistryKey saveSettings = SuiteKey.CreateSubKey("SymbolColors"))
            {
                saveSettings.SetValue(key, value.ToString(), RegistryValueKind.String);
            }
        }

        private Int32 GetValueFromRegistry(string symbolname)
        {
            Int32 win32color = 0;
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            using (RegistryKey Settings = SuiteKey.CreateSubKey("SymbolColors"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == symbolname)
                            {
                                string value = Settings.GetValue(a).ToString();
                                win32color = Convert.ToInt32(value);
                            }
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
            }
            return win32color;
        }
    }
}
