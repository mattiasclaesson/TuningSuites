using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CommonSuite;
using TrionicCANLib;
using System.IO.Ports;
using TrionicCANLib.API;
using DevExpress.XtraEditors.Controls;

namespace T7
{
    public partial class frmSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmSettings()
        {
            InitializeComponent();
            string[] portNames = SerialPort.GetPortNames();
            foreach (string port in portNames)
            {
                if (port.StartsWith("COM"))
                {
                    cbWidebandComPort.Properties.Items.Add(port);
                }
            }
            cbWidebandComPort.SelectedIndex = 0;
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        public int StandardFill
        {
            get
            {
                return comboBoxEdit4.SelectedIndex;
            }
            set
            {
                comboBoxEdit4.SelectedIndex = value;
            }
        }

        public CANBusAdapter CANBusAdapterType
        {
            get
            {
                if (cbAdapterType.SelectedIndex == -1) cbAdapterType.SelectedIndex = 0;
                return (CANBusAdapter)cbAdapterType.SelectedIndex;
            }
            set
            {
                try
                {
                    cbAdapterType.SelectedIndex = (int)value;
                }
                catch (Exception)
                {
                    cbAdapterType.SelectedIndex = 0;
                }
            }
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

        public bool ShowTablesUpsideDown
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

        public bool MeasureAFRInLambda
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

        public bool WriteTimestampInBinary
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
                int editValue = (int)value;
                if (editValue > 2) editValue = 2;
                comboBoxEdit1.SelectedIndex = editValue;
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


        public bool AutoFixFooter
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

        public bool EnableCanLog
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

        public bool AlwaysRecreateRepositoryItems
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

        public bool ShowMapViewersInWindows
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


        public bool ShowAddressesInHex
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

        public int AutoUpdateInterval
        {
            get
            {
                return Convert.ToInt32(spinEdit1.Value);
            }
            set
            {
                spinEdit1.Value = value;
            }
        }

        public bool AutoUpdateSRAMViewers
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

        public bool ResetRealtimeSymbolOnTabPageSwitch
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

        public bool UseAdditionalCanbusFrames
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

        public string WideBandSymbol
        {
            get
            {
                return (string)comboBoxEdit5.EditValue;
            }
            set
            {
                comboBoxEdit5.EditValue = value;
            }
        }

        public bool UseWidebandLambda
        {
            get
            {
                return ceWidebandTrionic.Checked;
            }
            set
            {
                ceWidebandTrionic.Checked = value;
            }
        }

        public bool OnlyPBus
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

        public bool DisableConnectionCheck
        {
            get
            {
                return ceDisableConnectionCheck.Checked;
            }
            set
            {
                ceDisableConnectionCheck.Checked = value;
            }
        }

        public bool AutoCreateAFRMaps
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

        public bool UseNewMapViewer
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


        private void frmSettings_Load(object sender, EventArgs e)
        {

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

        public bool RequestProjectNotes
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

        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAdapterType.SelectedIndex == (int)CANBusAdapter.COMBI || 
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.ELM327 ||
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.JUST4TRIONIC)
            {
                btnAdapterConfiguration.Enabled = true;
            }
            else
            {
                btnAdapterConfiguration.Enabled = false;
            }

            if (cbAdapterType.SelectedIndex != -1)
            {
                string[] adapters = ITrionic.GetAdapterNames((CANBusAdapter)cbAdapterType.SelectedIndex);
                ComboBoxItemCollection collection = cbAdapter.Properties.Items;
                collection.BeginUpdate();
                collection.Clear();
                foreach (string adapter in adapters)
                {
                    collection.Add(adapter);
                }
                collection.EndUpdate();

                if (adapters.Length > 0)
                {
                    cbAdapter.SelectedIndex = 0;
                    cbAdapter.Enabled = true;
                }
                else
                {
                    cbAdapter.SelectedIndex = -1;
                    cbAdapter.Enabled = false;
                }
            }
        }

        private AppSettings m_appSettings;

        public AppSettings AppSettings
        {
            get { return m_appSettings; }
            set { m_appSettings = value; }
        }

        private void btnAdapterConfiguration_Click(object sender, EventArgs e)
        {
            if (cbAdapterType.SelectedIndex == (int)CANBusAdapter.COMBI)
            {
                // open the config screen for additional configuration of the adapter
                // which ADC channels mean what
                // use ADC 1-5 & assign symbolname, max & min value
                // use thermo & assign symbolname, max & min value
                frmCombiAdapterConfig combiconfig = new frmCombiAdapterConfig();
                combiconfig.AppSettings = m_appSettings;
                if (combiconfig.ShowDialog() == DialogResult.OK)
                {
                    DialogResult = DialogResult.None;
                    // nothing really.. all is saved in appsettings already ... 
                }
            }
            else if (cbAdapterType.SelectedIndex == (int)CANBusAdapter.ELM327 ||
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.JUST4TRIONIC)
            {
                frmComportSettings comportSel = new frmComportSettings();
                comportSel.Baudrate = m_appSettings.Baudrate;
                comportSel.ELM327KLine = m_appSettings.ELM327Kline;
                if (comportSel.ShowDialog() == DialogResult.OK)
                {
                    m_appSettings.Baudrate = comportSel.Baudrate;
                    m_appSettings.ELM327Kline = comportSel.ELM327KLine;
                }
                DialogResult = DialogResult.None;
            }
        }

        private SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }


        private void btnNotifications_Click(object sender, EventArgs e)
        {
            frmNotifications notifications = new frmNotifications();
            notifications.AppSettings = m_appSettings;
            notifications.Symbols = m_symbols;
            if (notifications.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
            }

        }

        private void checkEdit26_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxEdit5.Enabled = ceWidebandTrionic.Checked;
            btnWidebandConfiguration.Enabled = ceWidebandTrionic.Checked;

            if (ceWidebandTrionic.Checked)
            {
                ceWidebandComPort.Checked = false;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            frmWidebandConfig config = new frmWidebandConfig();
            config.WidebandLambdaHighAFR = m_appSettings.WidebandHighAFR;
            config.WidebandLambdaHighVoltage = m_appSettings.WidebandHighVoltage;
            config.WidebandLambdaLowAFR = m_appSettings.WidebandLowAFR;
            config.WidebandLambdaLowVoltage = m_appSettings.WidebandLowVoltage;
            if (config.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.WidebandHighAFR = config.WidebandLambdaHighAFR;
                m_appSettings.WidebandHighVoltage = config.WidebandLambdaHighVoltage;
                m_appSettings.WidebandLowAFR = config.WidebandLambdaLowAFR;
                m_appSettings.WidebandLowVoltage = config.WidebandLambdaLowVoltage;
            }
            DialogResult = DialogResult.None;

        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            if (comboBoxEdit5.SelectedIndex == 0) btnWidebandConfiguration.Enabled = true;
            else btnWidebandConfiguration.Enabled = false;

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

        private bool m_allowIdleAutoTune = false;

        public bool AllowIdleAutoTune
        {
            get { return m_allowIdleAutoTune; }
            set { m_allowIdleAutoTune = value; }
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

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            // show autotune settings screen
            frmAutotuneSettings autotunesettings = new frmAutotuneSettings();
            autotunesettings.AcceptableTargetErrorPercentage = m_AcceptableTargetErrorPercentage;
            autotunesettings.AreaCorrectionPercentage = m_AreaCorrectionPercentage;
            autotunesettings.AutoUpdateFuelMap = m_AutoUpdateFuelMap;
            autotunesettings.CellStableTime_ms = m_CellStableTime_ms;
            autotunesettings.CorrectionPercentage = m_CorrectionPercentage;
            autotunesettings.DiscardClosedThrottleMeasurements = m_DiscardClosedThrottleMeasurements;
            autotunesettings.DiscardFuelcutMeasurements = m_DiscardFuelcutMeasurements;
            autotunesettings.DisableClosedLoopOnStartAutotune = m_DisableClosedLoopOnStartAutotune;
            autotunesettings.PlayCellProcessedSound = m_PlayCellProcessedSound;
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
                m_PlayCellProcessedSound = autotunesettings.PlayCellProcessedSound;

            }
            DialogResult = DialogResult.None;
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

        private void checkEdit33_CheckedChanged(object sender, EventArgs e)
        {
            cbWidebandDevice.Enabled = ceWidebandComPort.Checked;
            cbWidebandComPort.Enabled = ceWidebandComPort.Checked;
            if (ceWidebandComPort.Checked)
            {
                ceWidebandTrionic.Checked = false;
            }
        }

        public bool UseDigitalWidebandLambda
        {
            get
            {
                return ceWidebandComPort.Checked;
            }
            set
            {
                ceWidebandComPort.Checked = value;
            }
        }

        public string WidebandComPort
        {
            get
            {
                return cbWidebandComPort.SelectedItem != null ? cbWidebandComPort.SelectedItem.ToString() : String.Empty; 
            }
            set
            {
                cbWidebandComPort.SelectedItem = value;
            }
        }

        public string WidebandDevice
        {
            get
            {
                return cbWidebandDevice.SelectedItem.ToString();
            }
            set
            {
                cbWidebandDevice.SelectedItem = value;
            }
        }

        public string AdapterType
        {
            get
            {
                if (cbAdapterType.SelectedIndex == -1)
                    cbAdapterType.SelectedIndex = 1;
                return cbAdapterType.SelectedItem.ToString();
            }
            set
            {
                cbAdapterType.SelectedItem = value;
            }
        }

        public string Adapter
        {
            get
            {
                return cbAdapter.SelectedItem != null ? cbAdapter.SelectedItem.ToString() : string.Empty;
            }
            set
            {
                cbAdapter.SelectedItem = value;
            }
        }
    }
}