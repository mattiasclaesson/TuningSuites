using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO.Ports;

namespace CommonSuite
{
    public partial class frmComportSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmComportSettings()
        {
            InitializeComponent();
        }

        public int Baudrate
        {
            get
            {
                int retval = 38400;
                switch (comboBoxEdit2.SelectedIndex)
                {
                    case 0:
                        retval = 9600;
                        break;
                    case 1:
                        retval = 38400;
                        break;
                    case 2:
                        retval = 115200;
                        break;
                    case 3:
                        retval = 230400;
                        break;
                    case 4 :
                        retval = 2000000;
                        break;
                    default:
                        retval = 38400;
                        break;
                }
                return retval;
            }
            set
            {
                if (value == 9600) comboBoxEdit2.SelectedIndex = 0;
                else if (value == 115200) comboBoxEdit2.SelectedIndex = 2;
                else if (value == 230400) comboBoxEdit2.SelectedIndex = 3;
                else if (value == 2000000) comboBoxEdit2.SelectedIndex = 4;
                else comboBoxEdit2.SelectedIndex = 1;
            }
        }

        public bool ELM327KLine
        {
            get
            {
                return cbELM327KLine.Checked;
            }
            set
            {
                cbELM327KLine.Checked = value;
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
    }
}