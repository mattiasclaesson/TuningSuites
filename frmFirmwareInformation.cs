using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T7
{
    public partial class frmFirmwareInformation : DevExpress.XtraEditors.XtraForm
    {
        public frmFirmwareInformation()
        {
            InitializeComponent();
            dateEdit1.DateTime = DateTime.Now;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public DateTime ProgrammingDateTime
        {
            get
            {
                return dateEdit1.DateTime;
            }
            set
            {
                dateEdit1.DateTime = value;
            }
        }

        public bool ChecksumEnabled
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

        public bool CompressedSymboltable
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


        public bool MissingSymbolTable
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

        public string Partnumber
        {
            set
            {
                textEdit7.Text = value;
            }
        }

        public bool BioPowerSoftware
        {
            get
            {
                return checkEdit2.Enabled;
            }
            set
            {
                checkEdit2.Enabled = value;
            }
        }

        public bool BioPowerEnabled
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

        public bool SoftwareIsOpen
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

        public void EnableSIDAdvancedOptions(bool enabled)
        {
            // only for ET02U01C and EU0AF01C base code files
            groupControl3.Enabled = enabled;
            checkEdit12.Enabled = enabled;
            checkEdit13.Enabled = enabled;
        }

        public bool SIDDisableStartScreen
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

        public bool SIDDisableAdaptionMessages
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



        public bool CatalystLightOff
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

        public bool SecondLambdaEnabled
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

        public bool OBDIIPresent
        {
            set
            {
                if (value == true)
                {
                    checkEdit8.Enabled = true;
                }
                else
                {
                    checkEdit8.Enabled = false;
                }
            }
            get
            {
                return checkEdit8.Enabled;
            }
        }

        public bool FastThrottleResponsePresent
        {
            set
            {
                if (value == true)
                {
                    checkEdit9.Enabled = true;
                    checkEdit11.Enabled = true;
                }
                else
                {
                    checkEdit9.Enabled = false;
                    checkEdit11.Enabled = false;
                }
            }
        }

        public bool CatalystLightoffPresent
        {
            set
            {
                if (value == true)
                {
                    checkEdit10.Enabled = true;
                }
                else
                {
                    checkEdit10.Enabled = false;
                }
            }
        }




        public bool SecondLambdaPresent
        {
            set
            {
                if (value == true)
                {
                    checkEdit7.Enabled = true;
                }
                else
                {
                    checkEdit7.Enabled = false;
                    checkEdit7.Checked = false;
                }
            }
        }


        public bool FastThrottleReponse
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

        public bool ExtraFastThrottleReponse
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

        public bool OBDIIEnabled
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

        public bool TorqueLimitersPresent
        {
            get
            {
                return checkEdit6.Enabled;
            }
            set
            {
                checkEdit6.Enabled = value;
                if (!checkEdit6.Enabled)
                {
                    checkEdit6.Checked = false;
                }
            }
        }

        public bool TorqueLimitersEnabled
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

        public string OriginalCarType
        {
            get
            {
                return textEdit5.Text;
            }
            set
            {
                textEdit5.Text = value;
            }
        }

        public string SIDDate
        {
            get
            {
                return textEdit4.Text;
            }
            set
            {
                textEdit4.Properties.MaxLength = value.Length;
                textEdit4.Text = value;
            }
        }

        public string OriginalEngineType
        {
            get
            {
                return textEdit6.Text;
            }
            set
            {
                textEdit6.Text = value;
            }
        }

        public string EngineType
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
                while (buttonEdit1.Text.Length < buttonEdit1.Properties.MaxLength) buttonEdit1.Text += " ";
                return buttonEdit1.Text;
            }
            set
            {
                buttonEdit1.Text = value;
                buttonEdit1.Properties.MaxLength = value.Length;
            }
        }



        internal void DisableTimeStamping()
        {
            dateEdit1.Enabled = false;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkEdit11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit11.Checked) checkEdit9.Checked = false;
        }

        private void checkEdit9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit9.Checked) checkEdit11.Checked = false;
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked && this.Visible)
            {
                frmInfoBox info = new frmInfoBox("You can edit the SID parameters in T7Suite by starting the SID editor via Actions -> SID information");
            }
        }

        private string _previousVIN = string.Empty;
        private string _previousImmo = string.Empty;

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            ofd.Title = "Select a file to extract VIN and Immobilizer code from";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (_previousVIN == string.Empty) _previousVIN = ChassisID;
                if (_previousImmo == string.Empty) _previousImmo = ImmoID;
                T7FileHeader t7InfoHeader = new T7FileHeader();

                t7InfoHeader.init(ofd.FileName, false);
                // fetch the immo & VIN from another binary file
                ChassisID = t7InfoHeader.getChassisID();
                ImmoID = t7InfoHeader.getImmobilizerID();
                simpleButton4.Enabled = true;
                Application.DoEvents();
            }

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            simpleButton4.Enabled = false;
            ChassisID = _previousVIN;
            ImmoID = _previousImmo;
            Application.DoEvents();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            //VINDecoder
            frmDecodeVIN decode = new frmDecodeVIN();
            decode.SetVinNumber(buttonEdit1.Text);
            decode.ShowDialog();
        }
    }
}