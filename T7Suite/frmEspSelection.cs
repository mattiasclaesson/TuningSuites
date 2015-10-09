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
    public partial class frmEspSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmEspSelection()
        {
            InitializeComponent();
        }

        public byte Esp
        {
            get
            {
                byte value;
                switch(comboBoxEdit1.SelectedIndex)
                {
                    case 0 :
                        value = 0x82;
                        break;
                    case 1 :
                        value = 0x90;
                        break;
                    case 2 :
                        value = 0x91;
                        break;
                    case 3 :
                        value = 0x92;
                        break;
                    default :
                        value = 0;
                        break;
                }
                return value;
            }
            set
            {
                switch(value)
                {
                    case 0x82 :
                        comboBoxEdit1.SelectedIndex = 0;
                        break;
                    case 0x90 :
                        comboBoxEdit1.SelectedIndex = 1;
                        break;
                    case 0x91 :
                        comboBoxEdit1.SelectedIndex = 2;
                        break;
                    case 0x92 :
                        comboBoxEdit1.SelectedIndex = 3;
                        break;
                    default :
                        comboBoxEdit1.SelectedIndex = -1;
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
    }
}