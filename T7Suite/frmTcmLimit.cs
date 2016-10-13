using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO.Ports;

namespace T7
{
    public partial class frmTcmLimit : DevExpress.XtraEditors.XtraForm
    {
        public frmTcmLimit()
        {
            InitializeComponent();
        }

        public bool Modify
        {
            get 
            {
                return checkBox1.Checked;
            }
            set
            {
                checkBox1.Checked = value;
                comboBoxEdit1.Enabled = value;
            }
        }

        public bool ModifyLowGearSpecific
        {
            get
            {
                return checkBox2.Checked;
            }
            set
            {
                checkBox2.Checked = value;
                comboBoxEdit2.Enabled = value;
                comboBoxEdit3.Enabled = value;
            }
        }

        public int TorqueLimit
        {
            get
            {
                int value;
                switch(comboBoxEdit1.SelectedIndex)
                {
                    case 0 :
                        value = 0;
                        break;
                    case 1 :
                        value = 285;
                        break;
                    case 2 :
                        value = 295;
                        break;
                    case 3 :
                        value = 305;
                        break;
                    case 4 :
                        value = 325;
                        break;
                    case 5 :
                    default :
                        value = 345;
                        break;
                }
                return value;
            }
            set
            {
                switch(value)
                {
                    case 0 :
                        comboBoxEdit1.SelectedIndex = 0;
                        break;
                    case 285 :
                        comboBoxEdit1.SelectedIndex = 1;
                        break;
                    case 295 :
                        comboBoxEdit1.SelectedIndex = 2;
                        break;
                    case 305 :
                        comboBoxEdit1.SelectedIndex = 3;
                        break;
                    case 325:
                        comboBoxEdit1.SelectedIndex = 4;
                        break;
                    case 345:
                    default:
                        comboBoxEdit1.SelectedIndex = 5;
                        break;
                }
            }
        }

        public int LowGearModify
        {
            get
            {
                int value;
                switch (comboBoxEdit2.SelectedIndex)
                {
                    case 0:
                        value = 2; // Reverse
                        break;
                    case 1:
                        value = 3; // Neutral
                        break;
                    case 2:
                        value = 5; // Gear 1
                        break;
                    case 3:
                        value = 6; // Gear 2
                        break;
                    case 4:
                        value = 7; // Gear 3
                        break;
                    case 5:
                        value = 8; // Gear 4
                        break;
                    case 6:
                        value = 9; // Gear 5
                        break;
                    default:
                        value = 5;
                        break;
                }
                return value;
            }
            set
            {
                switch (value)
                {
                    case 2:
                        comboBoxEdit2.SelectedIndex = 0;
                        break;
                    case 3:
                        comboBoxEdit2.SelectedIndex = 1;
                        break;
                    case 5:
                        comboBoxEdit2.SelectedIndex = 2;
                        break;
                    case 6:
                        comboBoxEdit2.SelectedIndex = 3;
                        break;
                    case 7:
                        comboBoxEdit2.SelectedIndex = 4;
                        break;
                    case 8:
                        comboBoxEdit2.SelectedIndex = 5;
                        break;
                    case 9:
                        comboBoxEdit2.SelectedIndex = 6;
                        break;
                    default:
                        comboBoxEdit2.SelectedIndex = 2;
                        break;
                }
            }
        }

        public int LowGearTorqueLimit
        {
            get
            {
                int value;
                switch (comboBoxEdit3.SelectedIndex)
                {
                    case 0:
                        value = 0x10E;
                        break;
                    case 1:
                        value = 0x11F;
                        break;
                    case 2:
                        value = 0x12C;
                        break;
                    case 3:
                        value = 0x13F;
                        break;
                    case 4:
                        value = 0x14E;
                        break;
                    case 5:
                        value = 0x15E;
                        break;
                    case 6:
                        value = 0x16D;
                        break;
                    case 7:
                        value = 0x17E;
                        break;
                    case 8:
                        value = 0x18E;
                        break;
                    default:
                        value = 0x12C;
                        break;
                }
                return value;
            }
            set
            {
                switch (value)
                {
                    case 0x10E:
                        comboBoxEdit3.SelectedIndex = 0;
                        break;
                    case 0x11F:
                        comboBoxEdit3.SelectedIndex = 1;
                        break;
                    case 0x12C:
                        comboBoxEdit3.SelectedIndex = 2;
                        break;
                    case 0x13F:
                        comboBoxEdit3.SelectedIndex = 3;
                        break;
                    case 0x14E:
                        comboBoxEdit3.SelectedIndex = 4;
                        break;
                    case 0x15E:
                        comboBoxEdit3.SelectedIndex = 5;
                        break;
                    case 0x16D:
                        comboBoxEdit3.SelectedIndex = 6;
                        break;
                    case 0x17E:
                        comboBoxEdit3.SelectedIndex = 7;
                        break;
                    case 0x18E:
                        comboBoxEdit3.SelectedIndex = 8;
                        break;
                    default:
                        comboBoxEdit3.SelectedIndex = 2;
                        break;
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBoxEdit1.Enabled = true;
                comboBoxEdit2.Enabled = false;
                comboBoxEdit3.Enabled = false;
                checkBox2.Checked = false;
            }
            else
            {
                comboBoxEdit1.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                comboBoxEdit1.Enabled = false;
                comboBoxEdit2.Enabled = true;
                comboBoxEdit3.Enabled = true;
                checkBox1.Checked = false;
            }
            else
            {
                comboBoxEdit2.Enabled = false;
                comboBoxEdit3.Enabled = false;
            }
        }
    }
}