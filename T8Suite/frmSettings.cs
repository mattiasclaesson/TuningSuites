using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CommonSuite;
using System.IO.Ports;
using TrionicCANLib.API;
using DevExpress.XtraEditors.Controls;

namespace T8SuitePro
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

        public SuiteViewType DefaultViewType
        {
            get
            {
                return (SuiteViewType)comboBoxEdit1.SelectedIndex;
            }
            set
            {
                int selval = (int)value;
                if (selval > 2) selval = 2;
                comboBoxEdit1.SelectedIndex = selval;
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



        public bool ShowMapPreviewPopup
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

        public bool AutoMapDetectionActive
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
                if (comportSel.ShowDialog() == DialogResult.OK)
                {
                    m_appSettings.Baudrate = comportSel.Baudrate;
                }
                DialogResult = DialogResult.None;
            }
            else if (cbAdapterType.SelectedIndex == (int)CANBusAdapter.MXWIFI)
            {
                frmWifiSettings comportSel = new frmWifiSettings();
                comportSel.Port = m_appSettings.WifiPort;
                if (comportSel.ShowDialog() == DialogResult.OK)
                {
                    m_appSettings.WifiPort = comportSel.Port;
                }
                DialogResult = DialogResult.None;
            }
        }

        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAdapterType.SelectedIndex == (int)CANBusAdapter.COMBI || 
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.ELM327 ||
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.JUST4TRIONIC ||
                cbAdapterType.SelectedIndex == (int)CANBusAdapter.MXWIFI)
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

        private void ceWidebandComPort_CheckedChanged(object sender, EventArgs e)
        {
            cbWidebandDevice.Enabled = ceWidebandComPort.Checked;
            cbWidebandComPort.Enabled = ceWidebandComPort.Checked;
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