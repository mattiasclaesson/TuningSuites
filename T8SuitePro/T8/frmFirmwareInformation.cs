using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CommonSuite;

namespace T8SuitePro
{
    public partial class frmFirmwareInformation : DevExpress.XtraEditors.XtraForm
    {
        private string m_filename = string.Empty;

        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        public frmFirmwareInformation()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        public string EngineTypeBySoftwareVersion
        {
            get
            {
                while (textEdit1.Text.Length < textEdit1.Properties.MaxLength) textEdit1.Text += " ";

                return textEdit1.Text;
            }
            set
            {
                textEdit1.Text = value;
                textEdit1.Properties.MaxLength = value.Length;
            }
        }
        public string EngineTypeByVIN
        {
            get
            {
                while (textEdit13.Text.Length < textEdit13.Properties.MaxLength) textEdit13.Text += " ";

                return textEdit13.Text;
            }
            set
            {
                textEdit13.Text = value;
                textEdit13.Properties.MaxLength = value.Length;
            }
        }

        public string SoftwareID
        {
            get
            {
                while (textEdit2.Text.Length < textEdit2.Properties.MaxLength) textEdit2.Text += " ";

                return textEdit2.Text;
            }
            set
            {
                textEdit2.Text = value;
                textEdit2.Properties.MaxLength = value.Length;
            }
        }


        public string ImmoID
        {
            get
            {
                while (textEdit3.Text.Length < textEdit3.Properties.MaxLength) textEdit3.Text += " ";
                return textEdit3.Text;
            }
            set
            {
                textEdit3.Text = value;
                textEdit3.Properties.MaxLength = value.Length;
            }
        }
        public string ChassisID
        {
            get
            {
                while (textEdit4.Text.Length < textEdit4.Properties.MaxLength) textEdit4.Text += " ";
                return textEdit4.Text;
            }
            set
            {
                textEdit4.Text = value;
                textEdit4.Properties.MaxLength = value.Length;
            }
        }

        public string ReleaseDate
        {
            get
            {
                while (textEdit8.Text.Length < textEdit8.Properties.MaxLength) textEdit8.Text += " ";
                return textEdit8.Text;
            }
            set
            {
                textEdit8.Text = value;
                textEdit8.Properties.MaxLength = value.Length;
            }
        }

        public string PartNumber
        {
            get
            {
                while (textEdit5.Text.Length < textEdit5.Properties.MaxLength) textEdit5.Text += " ";
                return textEdit5.Text;
            }
            set
            {
                textEdit5.Text = value;
                textEdit5.Properties.MaxLength = value.Length;
            }
        }

        public string ProgrammingDevice
        {
            get
            {
                while (textEdit6.Text.Length < textEdit6.Properties.MaxLength) textEdit6.Text += " ";
                return textEdit6.Text;
            }
            set
            {
                textEdit6.Text = value;
                textEdit6.Properties.MaxLength = value.Length;
            }
        }

        public string ProgrammerName
        {
            get
            {
                while (textEdit7.Text.Length < textEdit7.Properties.MaxLength) textEdit7.Text += " ";
                return textEdit7.Text;
            }
            set
            {
                textEdit7.Text = value;
                textEdit7.Properties.MaxLength = value.Length;
            }
        }

        public string HardwareID
        {
            get
            {
                while (textEdit9.Text.Length < textEdit9.Properties.MaxLength) textEdit9.Text += " ";
                return textEdit9.Text;
            }
            set
            {
                textEdit9.Text = value;
                textEdit9.Properties.MaxLength = value.Length;
            }
        }

        public string HardwareType
        {
            get
            {
                while (textEdit10.Text.Length < textEdit10.Properties.MaxLength) textEdit10.Text += " ";
                return textEdit10.Text;
            }
            set
            {
                textEdit10.Text = value;
                textEdit10.Properties.MaxLength = value.Length;
            }
        }

        public string InterfaceDevice
        {
            get
            {
                while (textEdit11.Text.Length < textEdit11.Properties.MaxLength) textEdit11.Text += " ";
                return textEdit11.Text;
            }
            set
            {
                textEdit11.Text = value;
                textEdit11.Properties.MaxLength = value.Length;
            }
        }

        public string ECUDescription
        {
            get
            {
                while (textEdit12.Text.Length < textEdit12.Properties.MaxLength) textEdit12.Text += " ";
                return textEdit12.Text;
            }
            set
            {
                textEdit12.Text = value;
                textEdit12.Properties.MaxLength = value.Length;
            }
        }

        public string NumberOfFlashBlocks
        {
            get
            {
                return buttonEdit1.Text;
            }
            set
            {
                buttonEdit1.Text = value;
            }
        }

       

        private void EnableVinAndImmo()
        {
            if (!textEdit3.Enabled)
            {
                textEdit3.Enabled = true;
                textEdit4.Enabled = true;
                simpleButton3.Enabled = true;
                frmInfoBox info = new frmInfoBox("Warning: T8Suite only has experimental support for changing VIN and immobilizer codes!!!");
            }
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            EnableVinAndImmo();
        }

        private bool _changeVINAndImmo = false;

        public bool ChangeVINAndImmo
        {
            get { return _changeVINAndImmo; }
            set { _changeVINAndImmo = value; }
        }

        private void labelControl4_DoubleClick(object sender, EventArgs e)
        {
            EnableVinAndImmo();
            _changeVINAndImmo = true;

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // clear all VIN numbers in the flashblocks
            T8Header t8header = new T8Header();
            t8header.init(m_filename);
            t8header.ClearVIN();
            t8header.init(m_filename);
            textEdit4.Text = t8header.ChassisID;
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // show the flashblock browser
            DataTable dt = new DataTable();
            dt.Columns.Add("Blocknumber", Type.GetType("System.Int32"));
            dt.Columns.Add("Blocktype");
            dt.Columns.Add("Address");
            dt.Columns.Add("VIN");
            dt.Columns.Add("ECU type");
            dt.Columns.Add("Interface");
            dt.Columns.Add("SecretCode");
            T8Header t8header = new T8Header();
            t8header.init(m_filename);

            foreach (FlashBlock fb in t8header.FlashBlocks)
            {
                string vin = string.Empty;
                string ecudescr = string.Empty;
                string interfacetype = string.Empty;
                string secretcode = string.Empty;
                fb.DecodeBlock(out vin, out ecudescr, out interfacetype, out secretcode);
                dt.Rows.Add(fb.BlockNumber, fb.BlockType.ToString(), fb.BlockAddress.ToString("X8"), vin, ecudescr, interfacetype, secretcode);
            }
            frmFlashBlockBrowser browser = new frmFlashBlockBrowser();
            browser.SetData(dt);
            browser.ShowDialog();
        }


    }
}