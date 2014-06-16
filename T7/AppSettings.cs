using System;
using System.Collections.Generic;
using System.Text;
//using System.Security.Permissions;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

//[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,ViewAndModify = "HKEY_CURRENT_USER")]

namespace T7
{
    public class AppSettings
    {
        private string _ELM327Port = "Autodetect";

        public string ELM327Port
        {
            get { return _ELM327Port; }
            set
            {
                _ELM327Port = value;
                SaveRegistrySetting("ELM327Port", _ELM327Port);
            }
        }

        private string _LastXAxisFromMatrix = string.Empty;

        public string LastXAxisFromMatrix
        {
            get { return _LastXAxisFromMatrix; }
            set
            {
                _LastXAxisFromMatrix = value;
                SaveRegistrySetting("LastXAxisFromMatrix", _LastXAxisFromMatrix);
            }
        }
        private string _LastYAxisFromMatrix = string.Empty;

        public string LastYAxisFromMatrix
        {
            get { return _LastYAxisFromMatrix; }
            set
            {
                _LastYAxisFromMatrix = value;
                SaveRegistrySetting("LastYAxisFromMatrix", _LastYAxisFromMatrix);

            }

        }
        private string _LastZAxisFromMatrix = string.Empty;

        public string LastZAxisFromMatrix
        {
            get { return _LastZAxisFromMatrix; }
            set
            {
                _LastZAxisFromMatrix = value;
                SaveRegistrySetting("LastZAxisFromMatrix", _LastZAxisFromMatrix);

            }
        }

        private T7.frmMain.PanelMode _panelmode = frmMain.PanelMode.Day;

        public T7.frmMain.PanelMode Panelmode
        {
            get { return _panelmode; }
            set
            {
                _panelmode = value;
                SaveRegistrySetting("PanelMode", (int)_panelmode);
            }
        }

        #region auto logging

        private string m_autoLogTriggerStartSymbol = string.Empty;

        public string AutoLogTriggerStartSymbol
        {
            get { return m_autoLogTriggerStartSymbol; }
            set
            {
                m_autoLogTriggerStartSymbol = value;
                SaveRegistrySetting("autoLogTriggerStartSymbol", m_autoLogTriggerStartSymbol);
            }
        }
        private string m_autoLogTriggerStopSymbol = string.Empty;

        public string AutoLogTriggerStopSymbol
        {
            get { return m_autoLogTriggerStopSymbol; }
            set
            {
                m_autoLogTriggerStopSymbol = value;
                SaveRegistrySetting("autoLogTriggerStopSymbol", m_autoLogTriggerStopSymbol);
            }
        }

        private int m_autoLogStartSign = 0;

        public int AutoLogStartSign
        {
            get { return m_autoLogStartSign; }
            set
            {
                m_autoLogStartSign = value;
                SaveRegistrySetting("autoLogStartSign", m_autoLogStartSign);
            }
        }
        private int m_autoLogStopSign = 0;

        public int AutoLogStopSign
        {
            get { return m_autoLogStopSign; }
            set
            {
                m_autoLogStopSign = value;
                SaveRegistrySetting("autoLogStopSign", m_autoLogStopSign);
            }
        }

        private double m_autoLogStartValue = 0;

        public double AutoLogStartValue
        {
            get { return m_autoLogStartValue; }
            set
            {
                m_autoLogStartValue = value;
                SaveRegistrySetting("autoLogStartValue", m_autoLogStartValue.ToString());
            }
        }
        private double m_autoLogStopValue = 0;

        public double AutoLogStopValue
        {
            get { return m_autoLogStopValue; }
            set
            {
                m_autoLogStopValue = value;
                SaveRegistrySetting("autoLogStopValue", m_autoLogStopValue.ToString());
            }
        }

        private bool m_dosBoxInstalled = false;

        public bool DosBoxInstalled
        {
            get { return m_dosBoxInstalled; }
            set
            {
                m_dosBoxInstalled = value;
                SaveRegistrySetting("dosBoxInstalled", m_dosBoxInstalled);
            }
        }


        private bool m_autoLoggingEnabled = false;

        public bool AutoLoggingEnabled
        {
            get { return m_autoLoggingEnabled; }
            set
            {
                m_autoLoggingEnabled = value;
                SaveRegistrySetting("autoLoggingEnabled", m_autoLoggingEnabled);
            }
        }

        #endregion

        #region autotune

        private bool m_AllowIdleAutoTune = false;

        public bool AllowIdleAutoTune
        {
            get { return m_AllowIdleAutoTune; }
            set
            {
                m_AllowIdleAutoTune = value;
                SaveRegistrySetting("AllowIdleAutoTune", m_AllowIdleAutoTune);
            }
        }

        private bool m_PlayCellProcessedSound = false;

        public bool PlayCellProcessedSound
        {
            get { return m_PlayCellProcessedSound; }
            set
            {
                m_PlayCellProcessedSound = value;
                SaveRegistrySetting("PlayCellProcessedSound", m_PlayCellProcessedSound);
            }
        }

        private int m_MinimumAFRMeasurements = 25;

        public int MinimumAFRMeasurements
        {
            get { return m_MinimumAFRMeasurements; }
            set
            {
                m_MinimumAFRMeasurements = value;
                SaveRegistrySetting("MinimumAFRMeasurements", m_MinimumAFRMeasurements);
            }
        }
        private int m_MaximumAFRDeviance = 2;

        public int MaximumAFRDeviance
        {
            get { return m_MaximumAFRDeviance; }
            set
            {
                m_MaximumAFRDeviance = value;
                SaveRegistrySetting("MaximumAFRDeviance", m_MaximumAFRDeviance);
            }
        }

        private int m_CellStableTime_ms = 1000;

        public int CellStableTime_ms
        {
            get { return m_CellStableTime_ms; }
            set
            {
                m_CellStableTime_ms = value;
                SaveRegistrySetting("CellStableTime_ms", m_CellStableTime_ms);
            }
        }


        private int m_CorrectionPercentage = 50;

        public int CorrectionPercentage
        {
            get { return m_CorrectionPercentage; }
            set
            {
                m_CorrectionPercentage = value;
                SaveRegistrySetting("CorrectionPercentage", m_CorrectionPercentage);
            }
        }


        private int m_AreaCorrectionPercentage = 0;

        public int AreaCorrectionPercentage
        {
            get { return m_AreaCorrectionPercentage; }
            set
            {
                m_AreaCorrectionPercentage = value;
                SaveRegistrySetting("AreaCorrectionPercentage", m_AreaCorrectionPercentage);
            }
        }


        private int m_AcceptableTargetErrorPercentage = 2;

        public int AcceptableTargetErrorPercentage
        {
            get { return m_AcceptableTargetErrorPercentage; }
            set
            {
                m_AcceptableTargetErrorPercentage = value;
                SaveRegistrySetting("AcceptableTargetErrorPercentage", m_AcceptableTargetErrorPercentage);
            }
        }


        private int m_MaximumAdjustmentPerCyclePercentage = 10;

        public int MaximumAdjustmentPerCyclePercentage
        {
            get { return m_MaximumAdjustmentPerCyclePercentage; }
            set
            {
                m_MaximumAdjustmentPerCyclePercentage = value;
                SaveRegistrySetting("MaximumAdjustmentPerCyclePercentage", m_MaximumAdjustmentPerCyclePercentage);
            }
        }


        private int m_EnrichmentFilter = 3;

        public int EnrichmentFilter
        {
            get { return m_EnrichmentFilter; }
            set
            {
                m_EnrichmentFilter = value;
                SaveRegistrySetting("EnrichmentFilter", m_EnrichmentFilter);
            }
        }


        private int m_FuelCutDecayTime_ms = 100;

        public int FuelCutDecayTime_ms
        {
            get { return m_FuelCutDecayTime_ms; }
            set
            {
                m_FuelCutDecayTime_ms = value;
                SaveRegistrySetting("FuelCutDecayTime_ms", m_FuelCutDecayTime_ms);
            }
        }


        private bool m_DisableClosedLoopOnStartAutotune = true;

        public bool DisableClosedLoopOnStartAutotune
        {
            get { return m_DisableClosedLoopOnStartAutotune; }
            set
            {
                m_DisableClosedLoopOnStartAutotune = value;
                SaveRegistrySetting("DisableClosedLoopOnStartAutotune", m_DisableClosedLoopOnStartAutotune);
            }
        }

        private bool m_DiscardFuelcutMeasurements = true;

        public bool DiscardFuelcutMeasurements
        {
            get { return m_DiscardFuelcutMeasurements; }
            set
            {
                m_DiscardFuelcutMeasurements = value;
                SaveRegistrySetting("DiscardFuelcutMeasurements", m_DiscardFuelcutMeasurements);
            }
        }

        private bool m_DiscardClosedThrottleMeasurements = true;

        public bool DiscardClosedThrottleMeasurements
        {
            get { return m_DiscardClosedThrottleMeasurements; }
            set
            {
                m_DiscardClosedThrottleMeasurements = value;
                SaveRegistrySetting("DiscardClosedThrottleMeasurements", m_DiscardClosedThrottleMeasurements);
            }
        }

        private bool m_AutoUpdateFuelMap = false;

        public bool AutoUpdateFuelMap
        {
            get { return m_AutoUpdateFuelMap; }
            set
            {
                m_AutoUpdateFuelMap = value;
                SaveRegistrySetting("AutoUpdateFuelMap", m_AutoUpdateFuelMap);
            }
        }

        #endregion

        private bool _notification1Active = false;

        public bool Notification1Active
        {
            get { return _notification1Active; }
            set
            {
                _notification1Active = value;
                SaveRegistrySetting("notification1Active", _notification1Active);
            }
        }
        private bool _notification2Active = false;

        public bool Notification2Active
        {
            get { return _notification2Active; }
            set
            {
                _notification2Active = value;
                SaveRegistrySetting("notification2Active", _notification2Active);
            }
        }
        private bool _notification3Active = false;

        public bool Notification3Active
        {
            get { return _notification3Active; }
            set
            {
                _notification3Active = value;
                SaveRegistrySetting("notification3Active", _notification3Active);
            }
        }

        private string _notification1symbol = "Exhaust.T_Calc"; // only initial

        public string Notification1symbol
        {
            get { return _notification1symbol; }
            set
            {
                _notification1symbol = value;
                SaveRegistrySetting("notification1symbol", _notification1symbol);
            }
        }
        private string _notification2symbol = string.Empty;

        public string Notification2symbol
        {
            get { return _notification2symbol; }
            set
            {
                _notification2symbol = value;
                SaveRegistrySetting("notification2symbol", _notification2symbol);
            }
        }
        private string _notification3symbol = string.Empty;

        public string Notification3symbol
        {
            get { return _notification3symbol; }
            set
            {
                _notification3symbol = value;
                SaveRegistrySetting("notification3symbol", _notification3symbol);
            }
        }
        private int _notification1condition = 1;

        public int Notification1condition
        {
            get { return _notification1condition; }
            set
            {
                _notification1condition = value;
                SaveRegistrySetting("notification1condition", _notification1condition);
            }
        }
        private int _notification2condition = 1;

        public int Notification2condition
        {
            get { return _notification2condition; }
            set
            {
                _notification2condition = value;
                SaveRegistrySetting("notification2condition", _notification2condition);
            }
        }
        private int _notification3condition = 1;

        public int Notification3condition
        {
            get { return _notification3condition; }
            set
            {
                _notification3condition = value;
                SaveRegistrySetting("notification3condition", _notification3condition);
            }
        }
        private double _notification1value = 950;

        public double Notification1value
        {
            get { return _notification1value; }
            set
            {
                _notification1value = value;
                SaveRegistrySetting("notification1value", _notification1value.ToString());
            }
        }
        private double _notification2value = 0;

        public double Notification2value
        {
            get { return _notification2value; }
            set
            {
                _notification2value = value;
                SaveRegistrySetting("notification2value", _notification2value.ToString());
            }
        }
        private double _notification3value = 0;

        public double Notification3value
        {
            get { return _notification3value; }
            set
            {
                _notification3value = value;
                SaveRegistrySetting("notification3value", _notification3value.ToString());
            }
        }
        private string _notification1sound = "knock.wav";

        public string Notification1sound
        {
            get { return _notification1sound; }
            set
            {
                _notification1sound = value;
                SaveRegistrySetting("notification1sound", _notification1sound);
            }
        }
        private string _notification2sound = string.Empty;

        public string Notification2sound
        {
            get { return _notification2sound; }
            set
            {
                _notification2sound = value;
                SaveRegistrySetting("notification2sound", _notification2sound);
            }
        }
        private string _notification3sound = string.Empty;

        public string Notification3sound
        {
            get { return _notification3sound; }
            set
            {
                _notification3sound = value;
                SaveRegistrySetting("notification3sound", _notification3sound);
            }
        }

        private int m_StandardFill = 0;

        public int StandardFill
        {
            get { return m_StandardFill; }
            set
            {
                m_StandardFill = value;
                SaveRegistrySetting("StandardFill", m_StandardFill);
            }
        }

        private double _adc1lowvalue = 0;

        public double Adc1lowvalue
        {
            get { return _adc1lowvalue; }
            set
            {
                _adc1lowvalue = value;
                SaveRegistrySetting("adc1lowvalue", _adc1lowvalue.ToString());
            }
        }
        private double _adc1highvalue = 100000;

        public double Adc1highvalue
        {
            get { return _adc1highvalue; }
            set
            {
                _adc1highvalue = value;
                SaveRegistrySetting("adc1highvalue", _adc1highvalue.ToString());
            }
        }
        private double _adc2lowvalue = 0;

        public double Adc2lowvalue
        {
            get { return _adc2lowvalue; }
            set
            {
                _adc2lowvalue = value;
                SaveRegistrySetting("adc2lowvalue", _adc2lowvalue.ToString());
            }
        }
        private double _adc2highvalue = 100000;

        public double Adc2highvalue
        {
            get { return _adc2highvalue; }
            set
            {
                _adc2highvalue = value;
                SaveRegistrySetting("adc2highvalue", _adc2highvalue.ToString());
            }
        }
        private double _adc3lowvalue = 0;

        public double Adc3lowvalue
        {
            get { return _adc3lowvalue; }
            set
            {
                _adc3lowvalue = value;
                SaveRegistrySetting("adc3lowvalue", _adc3lowvalue.ToString());
            }
        }
        private double _adc3highvalue = 100000;

        public double Adc3highvalue
        {
            get { return _adc3highvalue; }
            set
            {
                _adc3highvalue = value;
                SaveRegistrySetting("adc3highvalue", _adc3highvalue.ToString());
            }
        }
        private double _adc4lowvalue = 0;

        public double Adc4lowvalue
        {
            get { return _adc4lowvalue; }
            set
            {
                _adc4lowvalue = value;
                SaveRegistrySetting("adc4lowvalue", _adc4lowvalue.ToString());
            }
        }
        private double _adc4highvalue = 100000;

        public double Adc4highvalue
        {
            get { return _adc4highvalue; }
            set
            {
                _adc4highvalue = value;
                SaveRegistrySetting("adc4highvalue", _adc4highvalue.ToString());
            }
        }
        private double _adc5lowvalue = 0;

        public double Adc5lowvalue
        {
            get { return _adc5lowvalue; }
            set
            {
                _adc5lowvalue = value;
                SaveRegistrySetting("adc5lowvalue", _adc5lowvalue.ToString());
            }
        }
        private double _adc5highvalue = 100000;

        public double Adc5highvalue
        {
            get { return _adc5highvalue; }
            set
            {
                _adc5highvalue = value;
                SaveRegistrySetting("adc5highvalue", _adc5highvalue.ToString());
            }
        }

        private double _adc1lowvoltage = 0;

        public double Adc1lowvoltage
        {
            get { return _adc1lowvoltage; }
            set
            {
                _adc1lowvoltage = value;
                SaveRegistrySetting("adc1lowvoltage", _adc1lowvoltage.ToString());
            }
        }
        private double _adc2lowvoltage = 0;

        public double Adc2lowvoltage
        {
            get { return _adc2lowvoltage; }
            set
            {
                _adc2lowvoltage = value;
                SaveRegistrySetting("adc2lowvoltage", _adc2lowvoltage.ToString());
            }
        }
        private double _adc3lowvoltage = 0;

        public double Adc3lowvoltage
        {
            get { return _adc3lowvoltage; }
            set
            {
                _adc3lowvoltage = value;
                SaveRegistrySetting("adc3lowvoltage", _adc3lowvoltage.ToString());
            }
        }
        private double _adc4lowvoltage = 0;

        public double Adc4lowvoltage
        {
            get { return _adc4lowvoltage; }
            set
            {
                _adc4lowvoltage = value;
                SaveRegistrySetting("adc4lowvoltage", _adc4lowvoltage.ToString());
            }
        }
        private double _adc5lowvoltage = 0;

        public double Adc5lowvoltage
        {
            get { return _adc5lowvoltage; }
            set
            {
                _adc5lowvoltage = value;
                SaveRegistrySetting("adc5lowvoltage", _adc5lowvoltage.ToString());
            }
        }
        private double _adc1highvoltage = 5000;

        public double Adc1highvoltage
        {
            get { return _adc1highvoltage; }
            set
            {
                _adc1highvoltage = value;
                SaveRegistrySetting("adc1highvoltage", _adc1highvoltage.ToString());
            }
        }
        private double _adc2highvoltage = 5000;

        public double Adc2highvoltage
        {
            get { return _adc2highvoltage; }
            set
            {
                _adc2highvoltage = value;
                SaveRegistrySetting("adc2highvoltage", _adc2highvoltage.ToString());
            }
        }
        private double _adc3highvoltage = 5000;

        public double Adc3highvoltage
        {
            get { return _adc3highvoltage; }
            set
            {
                _adc3highvoltage = value;
                SaveRegistrySetting("adc3highvoltage", _adc3highvoltage.ToString());
            }
        }
        private double _adc4highvoltage = 5000;

        public double Adc4highvoltage
        {
            get { return _adc4highvoltage; }
            set
            {
                _adc4highvoltage = value;
                SaveRegistrySetting("adc4highvoltage", _adc4highvoltage.ToString());
            }
        }
        private double _adc5highvoltage = 5000;

        public double Adc5highvoltage
        {
            get { return _adc5highvoltage; }
            set
            {
                _adc5highvoltage = value;
                SaveRegistrySetting("adc5highvoltage", _adc5highvoltage.ToString());
            }
        }
        private string _adc1channelname = "Channel1";

        public string Adc1channelname
        {
            get { return _adc1channelname; }
            set
            {
                _adc1channelname = value;
                SaveRegistrySetting("adc1channelname", _adc1channelname);
            }
        }
        private string _adc2channelname = "Channel2";

        public string Adc2channelname
        {
            get { return _adc2channelname; }
            set
            {
                _adc2channelname = value;
                SaveRegistrySetting("adc2channelname", _adc2channelname);
            }
        }
        private string _adc3channelname = "Channel3";

        public string Adc3channelname
        {
            get { return _adc3channelname; }
            set
            {
                _adc3channelname = value;
                SaveRegistrySetting("adc3channelname", _adc3channelname);
            }
        }
        private string _adc4channelname = "Channel4";

        public string Adc4channelname
        {
            get { return _adc4channelname; }
            set
            {
                _adc4channelname = value;
                SaveRegistrySetting("adc4channelname", _adc4channelname);
            }
        }
        private string _adc5channelname = "Channel5";

        public string Adc5channelname
        {
            get { return _adc5channelname; }
            set
            {
                _adc5channelname = value;
                SaveRegistrySetting("adc5channelname", _adc5channelname);
            }
        }
        private bool _useadc1 = false;

        public bool Useadc1
        {
            get { return _useadc1; }
            set
            {
                _useadc1 = value;
                SaveRegistrySetting("useadc1", _useadc1);
            }
        }
        private bool _useadc2 = false;

        public bool Useadc2
        {
            get { return _useadc2; }
            set
            {
                _useadc2 = value;
                SaveRegistrySetting("useadc2", _useadc2);
            }
        }
        private bool _useadc3 = false;

        public bool Useadc3
        {
            get { return _useadc3; }
            set
            {
                _useadc3 = value;
                SaveRegistrySetting("useadc3", _useadc3);
            }
        }
        private bool _useadc4 = false;

        public bool Useadc4
        {
            get { return _useadc4; }
            set
            {
                _useadc4 = value;
                SaveRegistrySetting("useadc4", _useadc4);
            }
        }
        private bool _useadc5 = false;

        public bool Useadc5
        {
            get { return _useadc5; }
            set
            {
                _useadc5 = value;
                SaveRegistrySetting("useadc5", _useadc5);
            }
        }
        private bool _usethermo = false;

        public bool Usethermo
        {
            get { return _usethermo; }
            set
            {
                _usethermo = value;
                SaveRegistrySetting("usethermo", _usethermo);
            }
        }
        private string _thermochannelname = "EGT";

        public string Thermochannelname
        {
            get { return _thermochannelname; }
            set
            {
                _thermochannelname = value;
                SaveRegistrySetting("thermochannelname", _thermochannelname);
            }
        }


        private CANBusAdapter m_CANBusAdapterType = CANBusAdapter.Lawicel;

        public CANBusAdapter CANBusAdapterType
        {
            get { return m_CANBusAdapterType; }
            set
            {
                m_CANBusAdapterType = value;
                SaveRegistrySetting("CANBusAdapterType", (int)m_CANBusAdapterType);
            }
        }

        private bool m_RequestProjectNotes = false;

        public bool RequestProjectNotes
        {
            get { return m_RequestProjectNotes; }
            set
            {
                m_RequestProjectNotes = value;
                SaveRegistrySetting("RequestProjectNotes", m_RequestProjectNotes);
            }
        }

        private string m_ProjectFolder = Application.StartupPath + "\\Projects";

        public string ProjectFolder
        {
            get { return m_ProjectFolder; }
            set
            {
                m_ProjectFolder = value;
                SaveRegistrySetting("ProjectFolder", m_ProjectFolder);
            }
        }

        private int m_LastOpenedType = 0; // 0 = file, 1 = project

        public int LastOpenedType
        {
            get { return m_LastOpenedType; }
            set
            {
                m_LastOpenedType = value;
                SaveRegistrySetting("LastOpenedType", m_LastOpenedType);
            }
        }

        private string m_lastprojectname = "";
        public string Lastprojectname
        {
            get { return m_lastprojectname; }
            set
            {
                if (m_lastprojectname != value)
                {
                    m_lastprojectname = value;
                    SaveRegistrySetting("LastProjectname", m_lastprojectname);
                }
            }
        }

        private Font m_RealtimeFont = new Font(FontFamily.GenericSansSerif, 8F, FontStyle.Regular);

        public Font RealtimeFont
        {
            get { return m_RealtimeFont; }
            set
            {
                m_RealtimeFont = value;
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
                string fontString = tc.ConvertToString(m_RealtimeFont);
                SaveRegistrySetting("RealtimeFont", fontString);
            }
        }

        private bool m_UseNewMapViewer = true;

        public bool UseNewMapViewer
        {
            get { return m_UseNewMapViewer; }
            set
            {
                m_UseNewMapViewer = value;
                SaveRegistrySetting("UseNewMapViewer", m_UseNewMapViewer);
            }
        }

        private bool m_AutoCreateAFRMaps = true;

        public bool AutoCreateAFRMaps
        {
            get { return m_AutoCreateAFRMaps; }
            set
            {
                m_AutoCreateAFRMaps = value;
                SaveRegistrySetting("AutoCreateAFRMaps", m_AutoCreateAFRMaps);
            }
        }

        private string m_WideBandSymbol = "DisplProt.AD_Scanner";

        public string WideBandSymbol
        {
            get { return m_WideBandSymbol; }
            set
            {
                m_WideBandSymbol = value;
                SaveRegistrySetting("WideBandSymbol", m_WideBandSymbol);
            }
        }

        private double m_WidebandLowVoltage = 0;

        public double WidebandLowVoltage
        {
            get { return m_WidebandLowVoltage; }
            set
            {
                m_WidebandLowVoltage = value;
                SaveRegistrySetting("WidebandLowVoltage", m_WidebandLowVoltage.ToString());
            }
        }
        private double m_WidebandHighVoltage = 5000;

        public double WidebandHighVoltage
        {
            get { return m_WidebandHighVoltage; }
            set
            {
                m_WidebandHighVoltage = value;
                SaveRegistrySetting("WidebandHighVoltage", m_WidebandHighVoltage.ToString());
            }
        }
        private double m_WidebandLowAFR = 7390;

        public double WidebandLowAFR
        {
            get { return m_WidebandLowAFR; }
            set
            {
                m_WidebandLowAFR = value;
                SaveRegistrySetting("WidebandLowAFR", m_WidebandLowAFR.ToString());
            }
        }
        private double m_WidebandHighAFR = 22300;

        public double WidebandHighAFR
        {
            get { return m_WidebandHighAFR; }
            set
            {
                m_WidebandHighAFR = value;
                SaveRegistrySetting("WidebandHighAFR", m_WidebandHighAFR.ToString());
            }
        }

        private bool m_UseWidebandLambda = false;

        public bool UseWidebandLambda
        {
            get { return m_UseWidebandLambda; }
            set
            {
                m_UseWidebandLambda = value;
                SaveRegistrySetting("UseWidebandLambda", m_UseWidebandLambda);
            }
        }

        private bool m_ResetRealtimeSymbolOnTabPageSwitch = true;

        public bool ResetRealtimeSymbolOnTabPageSwitch
        {
            get { return m_ResetRealtimeSymbolOnTabPageSwitch; }
            set
            {
                m_ResetRealtimeSymbolOnTabPageSwitch = value;
                SaveRegistrySetting("ResetRealtimeSymbolOnTabPageSwitch", m_ResetRealtimeSymbolOnTabPageSwitch);
            }
        }


        private bool m_UseAdditionalCanbusFrames = false;

        public bool UseAdditionalCanbusFrames
        {
            get { return false; }
            set
            {
                m_UseAdditionalCanbusFrames = false;
                SaveRegistrySetting("UseAdditionalCanbusFrames", m_UseAdditionalCanbusFrames);
            }
        }

        private bool m_AutoUpdateSRAMViewers = false;

        public bool AutoUpdateSRAMViewers
        {
            get { return m_AutoUpdateSRAMViewers; }
            set
            {
                m_AutoUpdateSRAMViewers = value;
                SaveRegistrySetting("AutoUpdateSRAMViewers", m_AutoUpdateSRAMViewers);
            }
        }

        private bool m_InterpolateLogWorksTimescale = false;

        public bool InterpolateLogWorksTimescale
        {
            get { return m_InterpolateLogWorksTimescale; }
            set
            {
                m_InterpolateLogWorksTimescale = value;
                SaveRegistrySetting("InterpolateLogWorksTimescale", m_InterpolateLogWorksTimescale);
            }
        }

        private int m_AutoUpdateInterval = 20;

        public int AutoUpdateInterval
        {
            get { return m_AutoUpdateInterval; }
            set
            {
                m_AutoUpdateInterval = value;
                SaveRegistrySetting("AutoUpdateInterval", m_AutoUpdateInterval.ToString());
            }
        }

        private int m_ApplicationLanguage = 44;

        public int ApplicationLanguage
        {
            get { return m_ApplicationLanguage; }
            set
            {
                m_ApplicationLanguage = value;
                SaveRegistrySetting("ApplicationLanguage", m_ApplicationLanguage.ToString());
            }
        }

        private string m_skinname = string.Empty;

        public string Skinname
        {
            get { return m_skinname; }
            set
            {
                m_skinname = value;
                SaveRegistrySetting("Skinname", m_skinname);
            }
        }

        private bool m_OnlyPBus = false;

        public bool OnlyPBus
        {
            get { return m_OnlyPBus; }
            set
            {
                m_OnlyPBus = value;
                SaveRegistrySetting("OnlyPBus", m_OnlyPBus);
            }
        }

        private bool m_EnableCanLog = false;

        public bool EnableCanLog
        {
            get { return m_EnableCanLog; }
            set
            {
                m_EnableCanLog = value;
                m_EnableCanLog = false;
                SaveRegistrySetting("EnableCanLog", m_EnableCanLog);
            }
        }

        private bool m_AutoFixFooter = false;

        public bool AutoFixFooter
        {
            get { return m_AutoFixFooter; }
            set
            {
                m_AutoFixFooter = value;
                SaveRegistrySetting("AutoFixFooter", m_AutoFixFooter);
            }
        }


        private bool m_ShowMenu = false;

        public bool ShowMenu
        {
            get { return m_ShowMenu; }
            set
            {
                m_ShowMenu = value;
                SaveRegistrySetting("ShowMenu", m_ShowMenu);
            }
        }

        private bool m_WriteTimestampInBinary = true;

        public bool WriteTimestampInBinary
        {
            get { return m_WriteTimestampInBinary; }
            set
            {
                m_WriteTimestampInBinary = value;
                SaveRegistrySetting("WriteTimestampInBinary", m_WriteTimestampInBinary);
            }
        }

        private bool m_MeasureAFRInLambda = false;

        public bool MeasureAFRInLambda
        {
            get { return m_MeasureAFRInLambda; }
            set
            {
                m_MeasureAFRInLambda = value;
                SaveRegistrySetting("MeasureAFRInLambda", m_MeasureAFRInLambda);
            }
        }

        private bool m_ShowTablesUpsideDown = false;

        public bool ShowTablesUpsideDown
        {
            get { return m_ShowTablesUpsideDown; }
            set
            {
                m_ShowTablesUpsideDown = value;
                SaveRegistrySetting("ShowTablesUpsideDown", m_ShowTablesUpsideDown);
            }
        }

        private bool m_AllowAskForPartnumber = true;

        public bool AllowAskForPartnumber
        {
            get { return m_AllowAskForPartnumber; }
            set
            {
                    m_AllowAskForPartnumber = value;
                    SaveRegistrySetting("AllowAskForPartnumber", m_AllowAskForPartnumber);
            }
        }

        private bool m_SynchronizeMapviewers = true;

        public bool SynchronizeMapviewers
        {
            get { return m_SynchronizeMapviewers; }
            set
            {
                m_SynchronizeMapviewers = value;
                SaveRegistrySetting("SynchronizeMapviewers", m_SynchronizeMapviewers);
            }
        }

        private bool m_FancyDocking = true;

        public bool FancyDocking
        {
            get { return m_FancyDocking; }
            set
            {
                m_FancyDocking = value;
                SaveRegistrySetting("FancyDocking", m_FancyDocking);
            }
        }

        private bool m_AutoLoadLastFile = true;

        public bool AutoLoadLastFile
        {
            get { return m_AutoLoadLastFile; }
            set
            {
                m_AutoLoadLastFile = value;
                SaveRegistrySetting("AutoLoadLastFile", m_AutoLoadLastFile);
            }
        }

        private bool m_AlwaysRecreateRepositoryItems = false;

        public bool AlwaysRecreateRepositoryItems
        {
            get { return m_AlwaysRecreateRepositoryItems; }
            set
            {
                m_AlwaysRecreateRepositoryItems = value;
                SaveRegistrySetting("AlwaysRecreateRepositoryItems", m_AlwaysRecreateRepositoryItems);
            }
        }

        private ViewType m_DefaultViewType = ViewType.Easy;

        public ViewType DefaultViewType
        {
            get { return m_DefaultViewType; }
            set
            {
                m_DefaultViewType = value;
                SaveRegistrySetting("DefaultViewType", (int)m_DefaultViewType);
            }
        }


        private ViewSize m_DefaultViewSize = ViewSize.NormalView;

        public ViewSize DefaultViewSize
        {
            get { return m_DefaultViewSize; }
            set
            {
                m_DefaultViewSize = value;
                SaveRegistrySetting("DefaultViewSize", (int)m_DefaultViewSize);
            }
        }

        private bool m_NewPanelsFloating = false;

        public bool NewPanelsFloating
        {
            get { return m_NewPanelsFloating; }
            set
            {
                m_NewPanelsFloating = value;
                SaveRegistrySetting("NewPanelsFloating", m_NewPanelsFloating);
            }
        }
        private bool m_ShowViewerInWindows = false;

        public bool ShowViewerInWindows
        {
            get { return m_ShowViewerInWindows; }
            set
            {
                m_ShowViewerInWindows = false; // WAS VALUE
                SaveRegistrySetting("ShowViewerInWindows", m_ShowViewerInWindows);
            }
        }


        private bool m_DisableMapviewerColors = false;

        public bool DisableMapviewerColors
        {
            get { return m_DisableMapviewerColors; }
            set
            {
                m_DisableMapviewerColors = value;
                SaveRegistrySetting("DisableMapviewerColors", m_DisableMapviewerColors);
            }
        }

        private bool m_AutoDockSameFile = false;

        public bool AutoDockSameFile
        {
            get { return m_AutoDockSameFile; }
            set
            {
                m_AutoDockSameFile = value;
                SaveRegistrySetting("AutoDockSameFile", m_AutoDockSameFile);
            }
        }


        private bool m_AutoDockSameSymbol = true;

        public bool AutoDockSameSymbol
        {
            get { return m_AutoDockSameSymbol; }
            set
            {
                m_AutoDockSameSymbol = value;
                SaveRegistrySetting("AutoDockSameSymbol", m_AutoDockSameSymbol);
            }
        }


        private bool m_AutoSizeNewWindows = true;

        public bool AutoSizeNewWindows
        {
            get { return m_AutoSizeNewWindows; }
            set
            {
                m_AutoSizeNewWindows = value;
                SaveRegistrySetting("AutoSizeNewWindows", m_AutoSizeNewWindows);
            }
        }

        private bool m_AutoSizeColumnsInWindows = true;

        public bool AutoSizeColumnsInWindows
        {
            get { return m_AutoSizeColumnsInWindows; }
            set
            {
                m_AutoSizeColumnsInWindows = value;
                SaveRegistrySetting("AutoSizeColumnsInWindows", m_AutoSizeColumnsInWindows);
            }
        }


        private bool m_ShowGraphs = true;

        public bool ShowGraphs
        {
            get { return m_ShowGraphs; }
            set
            {
                m_ShowGraphs = value;
                SaveRegistrySetting("ShowGraphs", m_ShowGraphs);
            }
        }

        private bool m_HideSymbolTable = false;

        public bool HideSymbolTable
        {
            get { return m_HideSymbolTable; }
            set
            {
                m_HideSymbolTable = value;
                SaveRegistrySetting("HideSymbolTable", m_HideSymbolTable);
            }
        }

        private bool m_ShowAddressesInHex = true;

        public bool ShowAddressesInHex
        {
            get { return m_ShowAddressesInHex; }
            set
            {
                m_ShowAddressesInHex = value;
                SaveRegistrySetting("ShowAddressesInHex", m_ShowAddressesInHex);
            }
        }

        private bool m_AutoChecksum = true;

        public bool AutoChecksum
        {
            get { return m_AutoChecksum; }
            set
            {
                m_AutoChecksum = value;
                SaveRegistrySetting("AutoChecksum", m_AutoChecksum);
            }
        }

        private string m_TargetECUReadFile = string.Empty;

        public string TargetECUReadFile
        {
            get { return m_TargetECUReadFile; }
            set
            {
                m_TargetECUReadFile = value;
                SaveRegistrySetting("TargetECUReadFile", m_TargetECUReadFile);
            }
        }

        private string m_write_ecubatchfile = string.Empty;

        public string Write_ecubatchfile
        {
            get { return m_write_ecubatchfile; }
            set 
            {
                if (m_write_ecubatchfile != value)
                {
                    m_write_ecubatchfile = value;
                    SaveRegistrySetting("WriteECUBatchfile", m_write_ecubatchfile);
                }
            }
        }
        private string m_read_ecubatchfile = string.Empty;

        public string Read_ecubatchfile
        {
            get { return m_read_ecubatchfile; }
            set {
                if (m_read_ecubatchfile != value)
                {
                    m_read_ecubatchfile = value;
                    SaveRegistrySetting("ReadECUBatchfile", m_read_ecubatchfile);
                }
            }
        }

        private string m_lastfilename = "";

        private bool m_ShowRedWhite = false;

        public bool ShowRedWhite
        {
            get { return m_ShowRedWhite; }
            set
            {
                if (m_ShowRedWhite != value)
                {
                    m_ShowRedWhite = value;
                    SaveRegistrySetting("ShowRedWhite", m_ShowRedWhite);
                }
            }
        }


        private bool m_AutoExtractSymbols = true;

        public bool AutoExtractSymbols
        {
            get { return m_AutoExtractSymbols; }
            set 
            {
                if(m_AutoExtractSymbols != value)
                {
                    m_AutoExtractSymbols = value;
                    SaveRegistrySetting("AutoExtractSymbols", m_AutoExtractSymbols);
                }
            }
        }



        public string Lastfilename
        {
            get { return m_lastfilename; }
            set {
                if (m_lastfilename != value)
                {
                    m_lastfilename = value;
                    SaveRegistrySetting("LastFilename", m_lastfilename);
                }
            }
        }
        private bool m_viewinhex = false;

        public bool Viewinhex
        {
            get { return m_viewinhex; }
            set 
            {
                if (m_viewinhex != value)
                {
                    m_viewinhex = value;
                    SaveRegistrySetting("ViewInHex", m_viewinhex);
                }
            }
        }

        private bool m_debugmode = false;

        public bool DebugMode
        {
            get { return m_debugmode; }
        }

        private bool m_adminmode = false;

        public bool AdminMode
        {
            get { return m_adminmode; }
        }



        private void SaveRegistrySetting(string key, string value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");

            using (RegistryKey saveSettings = ManufacturerKey.CreateSubKey("T7SuitePro"))
            {
                saveSettings.SetValue(key, value);
            }
        }
        private void SaveRegistrySetting(string key, Int32 value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");

            using (RegistryKey saveSettings = ManufacturerKey.CreateSubKey("T7SuitePro"))
            {
                saveSettings.SetValue(key, value);
            }
        }
        private void SaveRegistrySetting(string key, bool value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");

            using (RegistryKey saveSettings = ManufacturerKey.CreateSubKey("T7SuitePro"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        public void SaveSettings()
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");

            using (RegistryKey saveSettings = ManufacturerKey.CreateSubKey("T7SuitePro"))
            {
                saveSettings.SetValue("PanelMode", (int)_panelmode);
                saveSettings.SetValue("PlayCellProcessedSound", m_PlayCellProcessedSound);
                saveSettings.SetValue("AllowIdleAutoTune", m_AllowIdleAutoTune);

                saveSettings.SetValue("AcceptableTargetErrorPercentage", m_AcceptableTargetErrorPercentage);
                saveSettings.SetValue("AreaCorrectionPercentage", m_AreaCorrectionPercentage);
                saveSettings.SetValue("AutoUpdateFuelMap", m_AutoUpdateFuelMap);
                saveSettings.SetValue("CellStableTime_ms", m_CellStableTime_ms);
                saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);
                saveSettings.SetValue("CorrectionPercentage", m_CorrectionPercentage);
                saveSettings.SetValue("DiscardClosedThrottleMeasurements", m_DiscardClosedThrottleMeasurements);
                saveSettings.SetValue("DiscardFuelcutMeasurements", m_DiscardFuelcutMeasurements);
                saveSettings.SetValue("DisableClosedLoopOnStartAutotune", m_DisableClosedLoopOnStartAutotune);
                saveSettings.SetValue("EnrichmentFilter", m_EnrichmentFilter);
                saveSettings.SetValue("FuelCutDecayTime_ms", m_FuelCutDecayTime_ms);
                saveSettings.SetValue("MaximumAdjustmentPerCyclePercentage", m_MaximumAdjustmentPerCyclePercentage);
                saveSettings.SetValue("MinimumAFRMeasurements", m_MinimumAFRMeasurements);
                saveSettings.SetValue("MaximumAFRDeviance", m_MaximumAFRDeviance);

                saveSettings.SetValue("autoLoggingEnabled", m_autoLoggingEnabled);
                saveSettings.SetValue("autoLogStartSign", m_autoLogStartSign);
                saveSettings.SetValue("autoLogStartValue", m_autoLogStartValue.ToString());
                saveSettings.SetValue("autoLogStopSign", m_autoLogStopSign);
                saveSettings.SetValue("autoLogStopValue", m_autoLogStopValue.ToString());
                saveSettings.SetValue("autoLogTriggerStartSymbol", m_autoLogTriggerStartSymbol);
                saveSettings.SetValue("autoLogTriggerStopSymbol", m_autoLogTriggerStopSymbol);

                saveSettings.SetValue("ELM327Port", _ELM327Port);

                saveSettings.SetValue("LastXAxisFromMatrix", _LastXAxisFromMatrix);
                saveSettings.SetValue("LastYAxisFromMatrix", _LastYAxisFromMatrix);
                saveSettings.SetValue("LastZAxisFromMatrix", _LastZAxisFromMatrix);


                saveSettings.SetValue("notification1Active", _notification1Active);
                saveSettings.SetValue("notification2Active", _notification2Active);
                saveSettings.SetValue("notification3Active", _notification3Active);
                saveSettings.SetValue("notification1condition", _notification1condition);
                saveSettings.SetValue("notification2condition", _notification2condition);
                saveSettings.SetValue("notification3condition", _notification3condition);
                saveSettings.SetValue("notification1sound", _notification1sound);
                saveSettings.SetValue("notification2sound", _notification2sound);
                saveSettings.SetValue("notification3sound", _notification3sound);
                saveSettings.SetValue("notification1symbol", _notification1symbol);
                saveSettings.SetValue("notification2symbol", _notification2symbol);
                saveSettings.SetValue("notification3symbol", _notification3symbol);
                saveSettings.SetValue("notification1value", _notification1value.ToString());
                saveSettings.SetValue("notification2value", _notification2value.ToString());
                saveSettings.SetValue("notification3value", _notification3value.ToString());

                saveSettings.SetValue("StandardFill", m_StandardFill);
                saveSettings.SetValue("adc1channelname", _adc1channelname);
                saveSettings.SetValue("adc2channelname", _adc2channelname);
                saveSettings.SetValue("adc3channelname", _adc3channelname);
                saveSettings.SetValue("adc4channelname", _adc4channelname);
                saveSettings.SetValue("adc5channelname", _adc5channelname);
                saveSettings.SetValue("adc1highvalue", _adc1highvalue.ToString());
                saveSettings.SetValue("adc2highvalue", _adc2highvalue.ToString());
                saveSettings.SetValue("adc3highvalue", _adc3highvalue.ToString());
                saveSettings.SetValue("adc4highvalue", _adc4highvalue.ToString());
                saveSettings.SetValue("adc5highvalue", _adc5highvalue.ToString());
                saveSettings.SetValue("adc1lowvalue", _adc1lowvalue.ToString());
                saveSettings.SetValue("adc2lowvalue", _adc2lowvalue.ToString());
                saveSettings.SetValue("adc3lowvalue", _adc3lowvalue.ToString());
                saveSettings.SetValue("adc4lowvalue", _adc4lowvalue.ToString());
                saveSettings.SetValue("adc5lowvalue", _adc5lowvalue.ToString());
                saveSettings.SetValue("adc1lowvoltage", _adc1lowvoltage.ToString());
                saveSettings.SetValue("adc2lowvoltage", _adc2lowvoltage.ToString());
                saveSettings.SetValue("adc3lowvoltage", _adc3lowvoltage.ToString());
                saveSettings.SetValue("adc4lowvoltage", _adc4lowvoltage.ToString());
                saveSettings.SetValue("adc5lowvoltage", _adc5lowvoltage.ToString());
                saveSettings.SetValue("adc1highvoltage", _adc1highvoltage.ToString());
                saveSettings.SetValue("adc2highvoltage", _adc2highvoltage.ToString());
                saveSettings.SetValue("adc3highvoltage", _adc3highvoltage.ToString());
                saveSettings.SetValue("adc4highvoltage", _adc4highvoltage.ToString());
                saveSettings.SetValue("adc5highvoltage", _adc5highvoltage.ToString());
                saveSettings.SetValue("useadc1", _useadc1);
                saveSettings.SetValue("useadc2", _useadc2);
                saveSettings.SetValue("useadc3", _useadc3);
                saveSettings.SetValue("useadc4", _useadc4);
                saveSettings.SetValue("useadc5", _useadc5);
                saveSettings.SetValue("usethermo", _usethermo);
                saveSettings.SetValue("thermochannelname", _thermochannelname);
                saveSettings.SetValue("WidebandLowVoltage", m_WidebandLowVoltage.ToString());
                saveSettings.SetValue("WidebandHighVoltage", m_WidebandHighVoltage.ToString());
                saveSettings.SetValue("WidebandLowAFR", m_WidebandLowAFR.ToString());
                saveSettings.SetValue("WidebandHighAFR", m_WidebandHighAFR.ToString());

                saveSettings.SetValue("ViewInHex", m_viewinhex);
                saveSettings.SetValue("LastFilename", m_lastfilename);
                saveSettings.SetValue("AutoExtractSymbols", m_AutoExtractSymbols);
                saveSettings.SetValue("LastProjectname", m_lastprojectname);
                saveSettings.SetValue("WriteECUBatchfile", m_write_ecubatchfile);
                saveSettings.SetValue("LastOpenedType", m_LastOpenedType);
                saveSettings.SetValue("CANBusAdapterType", (int)m_CANBusAdapterType);
                saveSettings.SetValue("ReadECUBatchfile", m_read_ecubatchfile);
                saveSettings.SetValue("ShowRedWhite", m_ShowRedWhite);
                saveSettings.SetValue("TargetECUReadFile", m_TargetECUReadFile);
                saveSettings.SetValue("AutoChecksum", m_AutoChecksum);
                saveSettings.SetValue("ShowAddressesInHex", m_ShowAddressesInHex);
                saveSettings.SetValue("ShowGraphs", m_ShowGraphs);
                saveSettings.SetValue("HideSymbolTable", m_HideSymbolTable);
                saveSettings.SetValue("AutoSizeNewWindows", m_AutoSizeNewWindows);
                saveSettings.SetValue("AutoSizeColumnsInWindows", m_AutoSizeColumnsInWindows);
                saveSettings.SetValue("DisableMapviewerColors", m_DisableMapviewerColors);
                saveSettings.SetValue("AutoDockSameFile", m_AutoDockSameFile);
                saveSettings.SetValue("AutoDockSameSymbol", m_AutoDockSameSymbol);
                saveSettings.SetValue("ShowViewerInWindows", m_ShowViewerInWindows);
                saveSettings.SetValue("NewPanelsFloating", m_NewPanelsFloating);
                saveSettings.SetValue("Skinname", m_skinname);
                saveSettings.SetValue("DefaultViewType", (int)m_DefaultViewType);
                saveSettings.SetValue("DefaultViewSize", (int)m_DefaultViewSize);
                saveSettings.SetValue("AutoLoadLastFile", m_AutoLoadLastFile);
                saveSettings.SetValue("FancyDocking", m_FancyDocking);
                saveSettings.SetValue("AlwaysRecreateRepositoryItems", m_AlwaysRecreateRepositoryItems);
                saveSettings.SetValue("SynchronizeMapviewers", m_SynchronizeMapviewers);
                saveSettings.SetValue("AllowAskForPartnumber", m_AllowAskForPartnumber);
                saveSettings.SetValue("ShowTablesUpsideDown", m_ShowTablesUpsideDown);
                saveSettings.SetValue("MeasureAFRInLambda", m_MeasureAFRInLambda);
                saveSettings.SetValue("WriteTimestampInBinary", m_WriteTimestampInBinary);
                saveSettings.SetValue("ShowMenu", m_ShowMenu);
                saveSettings.SetValue("AutoFixFooter", m_AutoFixFooter);
                saveSettings.SetValue("EnableCanLog", m_EnableCanLog);
                saveSettings.SetValue("OnlyPBus", m_OnlyPBus);
                saveSettings.SetValue("InterpolateLogWorksTimescale", m_InterpolateLogWorksTimescale);
                saveSettings.SetValue("AutoUpdateSRAMViewers", m_AutoUpdateSRAMViewers);
                saveSettings.SetValue("UseAdditionalCanbusFrames", m_UseAdditionalCanbusFrames);
                saveSettings.SetValue("ResetRealtimeSymbolOnTabPageSwitch", m_ResetRealtimeSymbolOnTabPageSwitch);
                saveSettings.SetValue("UseWidebandLambda", m_UseWidebandLambda);
                saveSettings.SetValue("WideBandSymbol", m_WideBandSymbol);
                saveSettings.SetValue("AutoCreateAFRMaps", m_AutoCreateAFRMaps);
                saveSettings.SetValue("UseNewMapViewer", m_UseNewMapViewer);
                saveSettings.SetValue("ApplicationLanguage", m_ApplicationLanguage.ToString());
                saveSettings.SetValue("AutoUpdateInterval", m_AutoUpdateInterval.ToString());

                saveSettings.SetValue("ProjectFolder", m_ProjectFolder);
                saveSettings.SetValue("RequestProjectNotes", m_RequestProjectNotes);

                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
                string fontString = tc.ConvertToString(m_RealtimeFont);
                SaveRegistrySetting("RealtimeFont", fontString);

            }
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


        public AppSettings()
        {
            // laad alle waarden uit het register
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");

            using (RegistryKey Settings = ManufacturerKey.CreateSubKey("T7SuitePro"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == "ViewInHex")
                            {
                                m_viewinhex = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            if (a == "DebugMode")
                            {
                                m_debugmode = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            if (a == "AdminMode")
                            {
                                m_adminmode = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "LastFilename")
                            {
                                m_lastfilename = Settings.GetValue(a).ToString();
                            }
                            else if (a == "AutoExtractSymbols")
                            {
                                m_AutoExtractSymbols = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "LastOpenedType")
                            {
                                m_LastOpenedType = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "CANBusAdapterType")
                            {
                                m_CANBusAdapterType = (CANBusAdapter)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "PanelMode")
                            {
                                _panelmode = (frmMain.PanelMode)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification1Active")
                            {
                                _notification1Active = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification2Active")
                            {
                                _notification2Active = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification3Active")
                            {
                                _notification3Active = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification1condition")
                            {
                                _notification1condition = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification2condition")
                            {
                                _notification2condition = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification3condition")
                            {
                                _notification3condition = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification1sound")
                            {
                                _notification1sound = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification2sound")
                            {
                                _notification2sound = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification3sound")
                            {
                                _notification3sound = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification1symbol")
                            {
                                _notification1symbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification2symbol")
                            {
                                _notification2symbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification3symbol")
                            {
                                _notification3symbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "notification1value")
                            {
                                _notification1value = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification2value")
                            {
                                _notification2value = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "notification3value")
                            {
                                _notification3value = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "StandardFill")
                            {
                                m_StandardFill = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "thermochannelname")
                            {
                                _thermochannelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "adc1channelname")
                            {
                                _adc1channelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "adc2channelname")
                            {
                                _adc2channelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "adc3channelname")
                            {
                                _adc3channelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "adc4channelname")
                            {
                                _adc4channelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "adc5channelname")
                            {
                                _adc5channelname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "usethermo")
                            {
                                _usethermo = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "useadc1")
                            {
                                _useadc1 = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "useadc2")
                            {
                                _useadc2 = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "useadc3")
                            {
                                _useadc3 = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "useadc4")
                            {
                                _useadc4 = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "useadc5")
                            {
                                _useadc5 = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc1highvalue")
                            {
                                _adc1highvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc2highvalue")
                            {
                                _adc2highvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc3highvalue")
                            {
                                _adc3highvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc4highvalue")
                            {
                                _adc4highvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc5highvalue")
                            {
                                _adc5highvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc1lowvalue")
                            {
                                _adc1lowvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc2lowvalue")
                            {
                                _adc2lowvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc3lowvalue")
                            {
                                _adc3lowvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc4lowvalue")
                            {
                                _adc4lowvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc5lowvalue")
                            {
                                _adc5lowvalue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc1lowvoltage")
                            {
                                _adc1lowvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc2lowvoltage")
                            {
                                _adc2lowvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc3lowvoltage")
                            {
                                _adc3lowvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc4lowvoltage")
                            {
                                _adc4lowvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc5lowvoltage")
                            {
                                _adc5lowvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc1highvoltage")
                            {
                                _adc1highvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc2highvoltage")
                            {
                                _adc2highvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc3highvoltage")
                            {
                                _adc3highvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc4highvoltage")
                            {
                                _adc4highvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "adc5highvoltage")
                            {
                                _adc5highvoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }

                            else if (a == "WidebandLowVoltage")
                            {
                                m_WidebandLowVoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "WidebandHighVoltage")
                            {
                                m_WidebandHighVoltage = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "WidebandLowAFR")
                            {
                                m_WidebandLowAFR = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "WidebandHighAFR")
                            {
                                m_WidebandHighAFR = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "LastProjectname")
                            {
                                m_lastprojectname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "WriteECUBatchfile")
                            {
                                m_write_ecubatchfile = Settings.GetValue(a).ToString();
                            }
                            else if (a == "ProjectFolder")
                            {
                                m_ProjectFolder = Settings.GetValue(a).ToString();
                            }
                            else if (a == "TargetECUReadFile")
                            {
                                m_TargetECUReadFile = Settings.GetValue(a).ToString();
                            }
                            else if (a == "AutoChecksum")
                            {
                                m_AutoChecksum = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "RequestProjectNotes")
                            {
                                m_RequestProjectNotes = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowAddressesInHex")
                            {
                                m_ShowAddressesInHex = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "HideSymbolTable")
                            {
                                m_HideSymbolTable = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowGraphs")
                            {
                                m_ShowGraphs = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoSizeNewWindows")
                            {
                                m_AutoSizeNewWindows = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoSizeColumnsInWindows")
                            {
                                m_AutoSizeColumnsInWindows = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }

                            else if (a == "ReadECUBatchfile")
                            {
                                m_read_ecubatchfile = Settings.GetValue(a).ToString();
                            }
                            else if (a == "ShowRedWhite")
                            {
                                m_ShowRedWhite = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DisableMapviewerColors")
                            {
                                m_DisableMapviewerColors = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoDockSameFile")
                            {
                                m_AutoDockSameFile = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoDockSameSymbol")
                            {
                                m_AutoDockSameSymbol = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowViewerInWindows")
                            {
                                m_ShowViewerInWindows = false; //Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "NewPanelsFloating")
                            {
                                m_NewPanelsFloating = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AlwaysRecreateRepositoryItems")
                            {
                                m_AlwaysRecreateRepositoryItems = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoLoadLastFile")
                            {
                                m_AutoLoadLastFile = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "FancyDocking")
                            {
                                m_FancyDocking = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DefaultViewType")
                            {
                                m_DefaultViewType = (ViewType)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DefaultViewSize")
                            {
                                m_DefaultViewSize = (ViewSize)Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "SynchronizeMapviewers")
                            {
                                m_SynchronizeMapviewers = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AllowAskForPartnumber")
                            {
                                m_AllowAskForPartnumber = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowTablesUpsideDown")
                            {
                                m_ShowTablesUpsideDown = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "MeasureAFRInLambda")
                            {
                                m_MeasureAFRInLambda = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "WriteTimestampInBinary")
                            {
                                m_WriteTimestampInBinary = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ShowMenu")
                            {
                                m_ShowMenu = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoFixFooter")
                            {
                                m_AutoFixFooter = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "EnableCanLog")
                            {
                                //m_EnableCanLog = Convert.ToBoolean(Settings.GetValue(a).ToString());
                                m_EnableCanLog = false;// Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "OnlyPBus")
                            {
                                m_OnlyPBus = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "InterpolateLogWorksTimescale")
                            {
                                m_InterpolateLogWorksTimescale = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoUpdateSRAMViewers")
                            {
                                m_AutoUpdateSRAMViewers = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "UseAdditionalCanbusFrames")
                            {
                                m_UseAdditionalCanbusFrames = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "ResetRealtimeSymbolOnTabPageSwitch")
                            {
                                m_ResetRealtimeSymbolOnTabPageSwitch = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "UseWidebandLambda")
                            {
                                m_UseWidebandLambda = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "WideBandSymbol")
                            {
                                m_WideBandSymbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "AutoCreateAFRMaps")
                            {
                                m_AutoCreateAFRMaps = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "UseNewMapViewer")
                            {
                                m_UseNewMapViewer = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "Skinname")
                            {
                                m_skinname = Settings.GetValue(a).ToString();
                            }
                            else if (a == "RealtimeFont")
                            {
                                //m_RealtimeFont = new Font(Settings.GetValue(a).ToString(), 10F);
                                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Font));
                                //string fontString = tc.ConvertToString(font);
                                //Console.WriteLine("Font as string: {0}", fontString);

                                m_RealtimeFont = (Font)tc.ConvertFromString(Settings.GetValue(a).ToString());


                            }
                            else if (a == "ApplicationLanguage")
                            {
                                m_ApplicationLanguage = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoUpdateInterval")
                            {
                                m_AutoUpdateInterval = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }

                            else if (a == "PlayCellProcessedSound")
                            {
                                m_PlayCellProcessedSound = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AllowIdleAutoTune")
                            {
                                m_AllowIdleAutoTune = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AcceptableTargetErrorPercentage")
                            {
                                m_AcceptableTargetErrorPercentage = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AreaCorrectionPercentage")
                            {
                                m_AreaCorrectionPercentage = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "AutoUpdateFuelMap")
                            {
                                m_AutoUpdateFuelMap = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "CellStableTime_ms")
                            {
                                m_CellStableTime_ms = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "CorrectionPercentage")
                            {
                                m_CorrectionPercentage = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DiscardClosedThrottleMeasurements")
                            {
                                m_DiscardClosedThrottleMeasurements = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DiscardFuelcutMeasurements")
                            {
                                m_DiscardFuelcutMeasurements = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "DisableClosedLoopOnStartAutotune")
                            {
                                m_DisableClosedLoopOnStartAutotune = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "EnrichmentFilter")
                            {
                                m_EnrichmentFilter = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "FuelCutDecayTime_ms")
                            {
                                m_FuelCutDecayTime_ms = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "MaximumAdjustmentPerCyclePercentage")
                            {
                                m_MaximumAdjustmentPerCyclePercentage = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "MaximumAFRDeviance")
                            {
                                m_MaximumAFRDeviance = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "MinimumAFRMeasurements")
                            {
                                m_MinimumAFRMeasurements = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLoggingEnabled")
                            {
                                m_autoLoggingEnabled = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "dosBoxInstalled")
                            {
                                m_dosBoxInstalled = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLogStartSign")
                            {
                                m_autoLogStartSign = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLogStopSign")
                            {
                                m_autoLogStopSign = Convert.ToInt32(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLogStartValue")
                            {
                                m_autoLogStartValue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLogStopValue")
                            {
                                m_autoLogStopValue = ConvertToDouble(Settings.GetValue(a).ToString());
                            }
                            else if (a == "autoLogTriggerStartSymbol")
                            {
                                m_autoLogTriggerStartSymbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "autoLogTriggerStopSymbol")
                            {
                                m_autoLogTriggerStopSymbol = Settings.GetValue(a).ToString();
                            }
                            else if (a == "ELM327Port")
                            {
                                _ELM327Port = Settings.GetValue(a).ToString();
                            }
                            else if (a == "LastXAxisFromMatrix")
                            {
                                _LastXAxisFromMatrix = Settings.GetValue(a).ToString();
                            }
                            else if (a == "LastYAxisFromMatrix")
                            {
                                _LastYAxisFromMatrix = Settings.GetValue(a).ToString();
                            }
                            else if (a == "LastZAxisFromMatrix")
                            {
                                _LastZAxisFromMatrix = Settings.GetValue(a).ToString();
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("error retrieving registry settings: " + E.Message);
                        }

                    }
                }
            }

        }
    }
}
