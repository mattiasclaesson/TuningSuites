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
            comboBoxEdit1.Enabled = checkBox1.Checked;
        }
    }
}