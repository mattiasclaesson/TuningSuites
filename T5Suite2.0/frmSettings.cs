using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
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


        public double WidebandLambdaLowVoltage
        {
            get
            {
                return ConvertToDouble(txtLowVoltage.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;

                txtLowVoltage.Text = realval.ToString();
            }
        }

        public double WidebandLambdaHighVoltage
        {
            get
            {
                return ConvertToDouble(txtHighVoltage.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighVoltage.Text = realval.ToString();
            }
        }

        public double WidebandLambdaLowAFR
        {
            get
            {
                return ConvertToDouble(txtLowAFR.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtLowAFR.Text = realval.ToString();
            }
        }

        public double WidebandLambdaHighAFR
        {
            get
            {
                return (ConvertToDouble(txtHighAFR.Text)) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighAFR.Text = realval.ToString();
            }
        }

        public bool AlwaysCreateAFRMaps
        {
            get
            {
                return checkEdit31.Checked;
            }
            set
            {
                checkEdit31.Checked = value;
            }
        }

        public bool KnockCounterSnapshot
        {
            get
            {
                return checkEdit20.Checked;
            }
            set
            {
                checkEdit20.Checked = value;
            }
        }

        public bool FancyDocking
        {
            get
            {
                return checkEdit16.Checked;
            }
            set
            {
                checkEdit16.Checked = value;
            }
        }



        public bool SynchronizeMapviewers
        {
            get
            {
                return checkEdit15.Checked;
            }
            set
            {
                checkEdit15.Checked = value;
            }
        }

        public ViewType DefaultViewType
        {
            get
            {
                return (ViewType)comboBoxEdit1.SelectedIndex;
            }
            set
            {
                comboBoxEdit1.SelectedIndex = (int)value;
            }
        }


        public ViewSize DefaultViewSize
        {
            get
            {
                return (ViewSize)comboBoxEdit2.SelectedIndex;
            }
            set
            {
                comboBoxEdit2.SelectedIndex = (int)value;
            }
        }




        public bool AutoLoadLastFile
        {
            get
            {
                return checkEdit14.Checked;
            }
            set
            {
                checkEdit14.Checked = value;
            }
        }



        public bool NewPanelsFloating
        {
            get
            {
                return checkEdit12.Checked;
            }
            set
            {
                checkEdit12.Checked = value;
            }
        }

        public bool EnableAdvancedMode
        {
            get
            {
                return checkEdit11.Checked;
            }
            set
            {
                checkEdit11.Checked = value;
            }
        }

        public bool DisableMapviewerColors
        {
            get
            {
                return checkEdit8.Checked;
            }
            set
            {
                checkEdit8.Checked = value;
            }
        }
        public bool AutoDockSameFile
        {
            get
            {
                return checkEdit9.Checked;
            }
            set
            {
                checkEdit9.Checked = value;
            }
        }

        public bool AutoDockSameSymbol
        {
            get
            {
                return checkEdit10.Checked;
            }
            set
            {
                checkEdit10.Checked = value;
            }
        }


        public bool AutoSizeNewWindows
        {
            get
            {
                return checkEdit1.Checked;
            }
            set
            {
                checkEdit1.Checked = value;
            }
        }

        public bool UseRedAndWhiteMaps
        {
            get
            {
                return checkEdit2.Checked;
            }
            set
            {
                checkEdit2.Checked = value;
            }
        }

        public bool ViewTablesInHex
        {
            get
            {
                return checkEdit4.Checked;
            }
            set
            {
                checkEdit4.Checked = value;
            }
        }

        public bool ShowGraphsInMapViewer
        {
            get
            {
                return checkEdit5.Checked;
            }
            set
            {
                checkEdit5.Checked = value;
            }
        }

        public bool HideSymbolWindow
        {
            get
            {
                return checkEdit6.Checked;
            }
            set
            {
                checkEdit6.Checked = value;
            }
        }

        public bool AutoSizeColumnsInViewer
        {
            get
            {
                return checkEdit7.Checked;
            }
            set
            {
                checkEdit7.Checked = value;
            }
        }

        public bool AutoUpdateChecksum
        {
            get
            {
                return checkEdit3.Checked;
            }
            set
            {
                checkEdit3.Checked = value;
            }
        }

        public bool TemperatureInFahrenheit
        {
            get
            {
                return checkEdit22.Checked;
            }
            set
            {
                checkEdit22.Checked = value;
            }
        }


        public bool AutoGenerateLogWorksFile
        {
            get
            {
                return checkEdit18.Checked;
            }
            set
            {
                checkEdit18.Checked = value;
            }
        }

        public bool InterpolateLogWorksTimescale
        {
            get
            {
                return checkEdit19.Checked;
            }
            set
            {
                checkEdit19.Checked = value;
            }
        }

        private string GetWideBandSymbol(string editvalue)
        {
            string retval = string.Empty;
            int index = editvalue.IndexOf('(');
            if (index > 0)
            {
                retval = editvalue.Substring(0, index - 1);
            }
            return retval;
        }

        private string SetWideBandSymbol(string editvalue)
        {
            string retval = string.Empty;
            if (editvalue == "AD_cat") retval = "AD_cat (pin 70)";
            if (editvalue == "AD_sond") retval = "AD_sond (pin 23)";
            if (editvalue == "AD_EGR") retval = "AD_EGR (pin 69)";
            return retval;
        }

        public string WideBandLambdaSymbol
        {
            get
            {
                return GetWideBandSymbol((string)comboBoxEdit3.EditValue);
            }
            set
            {
                comboBoxEdit3.EditValue = SetWideBandSymbol(value);
            }
        }

        public string CanUSBDevice
        {
            get
            {
                return (string)comboBoxEdit4.EditValue;
            }
            set
            {
                string tvalue = value;
                if (tvalue == "Multiadapter") tvalue = "CombiAdapter";
                comboBoxEdit4.EditValue = value;
            }
        }


        public bool UseWidebandLambdaThroughSymbol
        {
            get
            {
                return checkEdit23.Checked;
            }
            set
            {
                checkEdit23.Checked = value;
            }
        }

        public bool ShowAddressesInHex
        {
            get
            {
                return checkEdit24.Checked;
            }
            set
            {
                checkEdit24.Checked = value;
            }
        }

        public bool AutoHighlightSelectedMap
        {
            get
            {
                return checkEdit13.Checked;
            }
            set
            {
                checkEdit13.Checked = value;
            }
        }

        public bool ShowAdditionalSymbolInformation
        {
            get
            {
                return checkEdit17.Checked;
            }
            set
            {
                checkEdit17.Checked = value;
            }
        }

        public bool RequestProjectNotes
        {
            get
            {
                return checkEdit26.Checked;
            }
            set
            {
                checkEdit26.Checked = value;
            }
        }

        public bool AutoDetectMapsensorType
        {
            get
            {
                return checkEdit28.Checked;
            }
            set
            {
                checkEdit28.Checked = value;
            }
        }


        public MapviewerType MapViewerType
        {
            get
            {
                return (MapviewerType)comboBoxEdit5.SelectedIndex;
            }
            set
            {
                comboBoxEdit5.SelectedIndex = (int)value;
            }

        }

        public bool UseEasyTrionicOptions
        {
            get
            {
                return checkEdit27.Checked;
            }
            set
            {
                checkEdit27.Checked = value;
            }
        }

        public bool DirectSRAMWriteOnSymbolChange
        {
            get
            {
                return checkEdit21.Checked;
            }
            set
            {
                checkEdit21.Checked = value;
            }
        }


        public bool PlayKnockSound
        {
            get
            {
                return checkEdit29.Checked;
            }
            set
            {
                checkEdit29.Checked = value;
            }
        }

        public bool AutoOpenLogFile
        {
            get
            {
                return checkEdit30.Checked;
            }
            set
            {
                checkEdit30.Checked = value;
            }
        }

        public bool OneLogPerTypePerDay
        {
            get
            {
                return checkEdit32.Checked;
            }
            set
            {
                checkEdit32.Checked = value;
            }
        }

        public bool OneLogForAllTypes
        {
            get
            {
                return checkEdit33.Checked;
            }
            set
            {
                checkEdit33.Checked = value;
            }
        }


        public bool EnableCanLogging
        {
            get
            {
                return checkEdit25.Checked;
            }
            set
            {
                checkEdit25.Checked = value;
            }
        }


        private void frmSettings_Load(object sender, EventArgs e)
        {

        }

        private int m_MinimumAFRMeasurements = 25;

        public int MinimumAFRMeasurements
        {
            get { return m_MinimumAFRMeasurements; }
            set
            {
                m_MinimumAFRMeasurements = value;
            }
        }
        private int m_MaximumAFRDeviance = 2;

        public int MaximumAFRDeviance
        {
            get { return m_MaximumAFRDeviance; }
            set
            {
                m_MaximumAFRDeviance = value;
            }
        }

        private double m_IgnitionAdvancePerCycle = 0.1;

        public double IgnitionAdvancePerCycle
        {
            get { return m_IgnitionAdvancePerCycle; }
            set { m_IgnitionAdvancePerCycle = value; }
        }

        private double m_IgnitionRetardFirstKnock = 1.0;

        public double IgnitionRetardFirstKnock
        {
            get { return m_IgnitionRetardFirstKnock; }
            set { m_IgnitionRetardFirstKnock = value; }
        }

        private double m_GlobalMaximumIgnitionAdvance = 35.0;

        public double GlobalMaximumIgnitionAdvance
        {
            get { return m_GlobalMaximumIgnitionAdvance; }
            set { m_GlobalMaximumIgnitionAdvance = value; }
        }

        private double m_IgnitionRetardFurtherKnocks = 0.5;

        public double IgnitionRetardFurtherKnocks
        {
            get { return m_IgnitionRetardFurtherKnocks; }
            set { m_IgnitionRetardFurtherKnocks = value; }
        }


        private double m_MaximumIgnitionAdvancePerSession = 2;

        public double MaximumIgnitionAdvancePerSession
        {
            get { return m_MaximumIgnitionAdvancePerSession; }
            set { m_MaximumIgnitionAdvancePerSession = value; }
        }

        private int m_MinimumEngineSpeedForIgnitionTuning = 1200;

        public int MinimumEngineSpeedForIgnitionTuning
        {
            get { return m_MinimumEngineSpeedForIgnitionTuning; }
            set { m_MinimumEngineSpeedForIgnitionTuning = value; }
        }

        private int m_IgnitionCellStableTime_ms = 500;

        public int IgnitionCellStableTime_ms
        {
            get { return m_IgnitionCellStableTime_ms; }
            set { m_IgnitionCellStableTime_ms = value; }
        }

        private int m_CellStableTime_ms = 1000;

        public int CellStableTime_ms
        {
            get { return m_CellStableTime_ms; }
            set
            {
                m_CellStableTime_ms = value;
            }
        }


        private int m_CorrectionPercentage = 50;

        public int CorrectionPercentage
        {
            get { return m_CorrectionPercentage; }
            set
            {
                m_CorrectionPercentage = value;
            }
        }


        private int m_AreaCorrectionPercentage = 0;

        public int AreaCorrectionPercentage
        {
            get { return m_AreaCorrectionPercentage; }
            set
            {
                m_AreaCorrectionPercentage = value;
            }
        }


        private int m_AcceptableTargetErrorPercentage = 2;

        public int AcceptableTargetErrorPercentage
        {
            get { return m_AcceptableTargetErrorPercentage; }
            set
            {
                m_AcceptableTargetErrorPercentage = value;
            }
        }


        private int m_MaximumAdjustmentPerCyclePercentage = 10;

        public int MaximumAdjustmentPerCyclePercentage
        {
            get { return m_MaximumAdjustmentPerCyclePercentage; }
            set
            {
                m_MaximumAdjustmentPerCyclePercentage = value;
            }
        }


        private int m_EnrichmentFilter = 3;

        public int EnrichmentFilter
        {
            get { return m_EnrichmentFilter; }
            set
            {
                m_EnrichmentFilter = value;
            }
        }


        private int m_FuelCutDecayTime_ms = 100;

        public int FuelCutDecayTime_ms
        {
            get { return m_FuelCutDecayTime_ms; }
            set
            {
                m_FuelCutDecayTime_ms = value;
            }
        }

        private bool m_CapIgnitionMap = true;

        public bool CapIgnitionMap
        {
            get { return m_CapIgnitionMap; }
            set { m_CapIgnitionMap = value; }
        }

        private bool m_allowIdleAutoTune = false;

        public bool AllowIdleAutoTune
        {
            get { return m_allowIdleAutoTune; }
            set { m_allowIdleAutoTune = value; }
        }

        private bool m_ResetFuelTrims = false;

        public bool ResetFuelTrims
        {
            get { return m_ResetFuelTrims; }
            set { m_ResetFuelTrims = value; }
        }

        private bool m_PlayCellProcessedSound = false;

        public bool PlayCellProcessedSound
        {
            get { return m_PlayCellProcessedSound; }
            set { m_PlayCellProcessedSound = value; }
        }

        private bool m_DisableClosedLoopOnStartAutotune = true;

        public bool DisableClosedLoopOnStartAutotune
        {
            get { return m_DisableClosedLoopOnStartAutotune; }
            set { m_DisableClosedLoopOnStartAutotune = value; }
        }

        private bool m_DiscardFuelcutMeasurements = true;

        public bool DiscardFuelcutMeasurements
        {
            get { return m_DiscardFuelcutMeasurements; }
            set
            {
                m_DiscardFuelcutMeasurements = value;
            }
        }


        private bool m_DiscardClosedThrottleMeasurements = true;

        public bool DiscardClosedThrottleMeasurements
        {
            get { return m_DiscardClosedThrottleMeasurements; }
            set
            {
                m_DiscardClosedThrottleMeasurements = value;
            }
        }

        private bool m_AutoUpdateFuelMap = false;

        public bool AutoUpdateFuelMap
        {
            get { return m_AutoUpdateFuelMap; }
            set
            {
                m_AutoUpdateFuelMap = value;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // show autotune settings screen
            frmAutotuneSettings autotunesettings = new frmAutotuneSettings();
            autotunesettings.AcceptableTargetErrorPercentage = m_AcceptableTargetErrorPercentage;
            autotunesettings.AreaCorrectionPercentage = m_AreaCorrectionPercentage;
            autotunesettings.AutoUpdateFuelMap = m_AutoUpdateFuelMap;
            autotunesettings.CellStableTime_ms = m_CellStableTime_ms;
            autotunesettings.IgnitionCellStableTime_ms = m_IgnitionCellStableTime_ms;
            autotunesettings.MinimumEngineSpeedForIgnitionTuning = m_MinimumEngineSpeedForIgnitionTuning;
            autotunesettings.MaximumIgnitionAdvancePerSession = m_MaximumIgnitionAdvancePerSession;
            autotunesettings.IgnitionAdvancePerCycle = m_IgnitionAdvancePerCycle;
            autotunesettings.IgnitionRetardFirstKnock = m_IgnitionRetardFirstKnock;
            autotunesettings.IgnitionRetardFurtherKnocks = m_IgnitionRetardFurtherKnocks;
            autotunesettings.GlobalMaximumIgnitionAdvance = m_GlobalMaximumIgnitionAdvance;
            autotunesettings.CorrectionPercentage = m_CorrectionPercentage;
            autotunesettings.DiscardClosedThrottleMeasurements = m_DiscardClosedThrottleMeasurements;
            autotunesettings.DiscardFuelcutMeasurements = m_DiscardFuelcutMeasurements;
            autotunesettings.DisableClosedLoopOnStartAutotune = m_DisableClosedLoopOnStartAutotune;
            autotunesettings.PlayCellProcessedSound = m_PlayCellProcessedSound;
            autotunesettings.CapIgnitionMap = m_CapIgnitionMap;
            autotunesettings.ResetFuelTrims = m_ResetFuelTrims;
            autotunesettings.AllowIdleAutoTune = m_allowIdleAutoTune;
            autotunesettings.EnrichmentFilter = m_EnrichmentFilter;
            autotunesettings.FuelCutDecayTime_ms = m_FuelCutDecayTime_ms;
            autotunesettings.MaximumAdjustmentPerCyclePercentage = m_MaximumAdjustmentPerCyclePercentage;
            autotunesettings.MaximumAFRDeviance = m_MaximumAFRDeviance;
            autotunesettings.MinimumAFRMeasurements = m_MinimumAFRMeasurements;
            if (autotunesettings.ShowDialog() == DialogResult.OK)
            {
                m_AcceptableTargetErrorPercentage = autotunesettings.AcceptableTargetErrorPercentage;
                m_AreaCorrectionPercentage = autotunesettings.AreaCorrectionPercentage;
                m_AutoUpdateFuelMap = autotunesettings.AutoUpdateFuelMap;
                m_CellStableTime_ms = autotunesettings.CellStableTime_ms;
                m_IgnitionCellStableTime_ms = autotunesettings.IgnitionCellStableTime_ms;
                m_MinimumEngineSpeedForIgnitionTuning = autotunesettings.MinimumEngineSpeedForIgnitionTuning;
                m_MaximumIgnitionAdvancePerSession = autotunesettings.MaximumIgnitionAdvancePerSession;
                m_IgnitionAdvancePerCycle = autotunesettings.IgnitionAdvancePerCycle;
                m_IgnitionRetardFirstKnock = autotunesettings.IgnitionRetardFirstKnock;
                m_IgnitionRetardFurtherKnocks = autotunesettings.IgnitionRetardFurtherKnocks;
                m_GlobalMaximumIgnitionAdvance = autotunesettings.GlobalMaximumIgnitionAdvance;
                m_CorrectionPercentage = autotunesettings.CorrectionPercentage;
                m_DisableClosedLoopOnStartAutotune = autotunesettings.DisableClosedLoopOnStartAutotune;
                m_DiscardClosedThrottleMeasurements = autotunesettings.DiscardClosedThrottleMeasurements;
                m_DiscardFuelcutMeasurements = autotunesettings.DiscardFuelcutMeasurements;
                m_EnrichmentFilter = autotunesettings.EnrichmentFilter;
                m_FuelCutDecayTime_ms = autotunesettings.FuelCutDecayTime_ms;
                m_MaximumAdjustmentPerCyclePercentage = autotunesettings.MaximumAdjustmentPerCyclePercentage;
                m_MaximumAFRDeviance = autotunesettings.MaximumAFRDeviance;
                m_MinimumAFRMeasurements = autotunesettings.MinimumAFRMeasurements;
                m_allowIdleAutoTune = autotunesettings.AllowIdleAutoTune;
                m_ResetFuelTrims = autotunesettings.ResetFuelTrims;
                m_CapIgnitionMap = autotunesettings.CapIgnitionMap;
                m_PlayCellProcessedSound = autotunesettings.PlayCellProcessedSound;

            }
            DialogResult = DialogResult.None;
        }

        private SymbolCollection m_symbols;

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }

        private string m_autoLogTriggerStartSymbol = string.Empty;

        public string AutoLogTriggerStartSymbol
        {
            get { return m_autoLogTriggerStartSymbol; }
            set { m_autoLogTriggerStartSymbol = value; }
        }
        private string m_autoLogTriggerStopSymbol = string.Empty;

        public string AutoLogTriggerStopSymbol
        {
            get { return m_autoLogTriggerStopSymbol; }
            set { m_autoLogTriggerStopSymbol = value; }
        }

        private int m_autoLogStartSign = 0;

        public int AutoLogStartSign
        {
            get { return m_autoLogStartSign; }
            set { m_autoLogStartSign = value; }
        }
        private int m_autoLogStopSign = 0;

        public int AutoLogStopSign
        {
            get { return m_autoLogStopSign; }
            set { m_autoLogStopSign = value; }
        }

        private double m_autoLogStartValue = 0;

        public double AutoLogStartValue
        {
            get { return m_autoLogStartValue; }
            set { m_autoLogStartValue = value; }
        }
        private double m_autoLogStopValue = 0;

        public double AutoLogStopValue
        {
            get { return m_autoLogStopValue; }
            set { m_autoLogStopValue = value; }
        }

        private bool m_autoLoggingEnabled = false;

        public bool AutoLoggingEnabled
        {
            get { return m_autoLoggingEnabled; }
            set { m_autoLoggingEnabled = value; }
        }

        public string ProjectFolder
        {
            get
            {
                return buttonEdit1.Text;
            }
            set
            {
                buttonEdit1.Text = value;
                if (buttonEdit1.Text == "")
                {
                    buttonEdit1.Text = Application.StartupPath + "\\Projects";
                }
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            frmAutoLoggingSettings autologsettings = new frmAutoLoggingSettings();
            autologsettings.AutoLoggingEnabled = m_autoLoggingEnabled;
            autologsettings.TriggerStartSymbol = m_autoLogTriggerStartSymbol;
            autologsettings.TriggerStopSymbol = m_autoLogTriggerStopSymbol;
            autologsettings.TriggerStartValue = m_autoLogStartValue;
            autologsettings.TriggerStopValue = m_autoLogStopValue;
            autologsettings.TriggerStartSign = (AutoStartSign)m_autoLogStartSign;
            autologsettings.TriggerStopSign = (AutoStartSign)m_autoLogStopSign;
            autologsettings.Symbols = m_symbols;
            if (autologsettings.ShowDialog() == DialogResult.OK)
            {
                m_autoLoggingEnabled = autologsettings.AutoLoggingEnabled;
                m_autoLogTriggerStartSymbol = autologsettings.TriggerStartSymbol;
                m_autoLogTriggerStopSymbol = autologsettings.TriggerStopSymbol;
                m_autoLogStartValue = autologsettings.TriggerStartValue;
                m_autoLogStopValue = autologsettings.TriggerStopValue;
                m_autoLogStartSign = (int)autologsettings.TriggerStartSign;
                m_autoLogStopSign = (int)autologsettings.TriggerStopSign;
            }
            DialogResult = DialogResult.None;
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = "Choose a project folder";
            fbd.SelectedPath = buttonEdit1.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                buttonEdit1.Text = fbd.SelectedPath;
            }
        }

        private void comboBoxEdit4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("A restart of the application is needed after changing the type of canbus adapter!");
            if (comboBoxEdit4.SelectedIndex == 2) simpleButton5.Enabled = true;
            else simpleButton5.Enabled = false;
        }

        private T5AppSettings m_appSettings;

        public T5AppSettings AppSettings
        {
            get { return m_appSettings; }
            set { m_appSettings = value; }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            // open the config screen for additional configuration of the adapter
            // which ADC channels mean what
            // use ADC 1-5 & assign symbolname, max & min value
            // use thermo & assign symbolname, max & min value
            frmMultiAdapterConfig multiconfig = new frmMultiAdapterConfig();
            multiconfig.AppSettings = m_appSettings;
            if (multiconfig.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                // nothing really.. all is saved in appsettings already ... 
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            frmNotifications notifications = new frmNotifications();
            notifications.AppSettings = m_appSettings;
            notifications.Symbols = m_symbols;
            if (notifications.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                // nothing really.. all is saved in appsettings already ... 
            }
        }




    }
}