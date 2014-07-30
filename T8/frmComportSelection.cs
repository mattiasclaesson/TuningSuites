using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO.Ports;

namespace T8SuitePro
{
    public partial class frmComportSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmComportSelection()
        {
            InitializeComponent();
            string[] portNames = SerialPort.GetPortNames();
            comboBoxEdit1.Properties.Items.Add("Autodetect");
            foreach (string port in portNames)
            {
                if (port.StartsWith("COM"))
                {
                    comboBoxEdit1.Properties.Items.Add(port);
                }
            }
            comboBoxEdit1.SelectedIndex = 0;
        }

        public string PortName
        {
            get
            {
                return comboBoxEdit1.SelectedItem.ToString();
            }
            set
            {
                comboBoxEdit1.SelectedItem = value;
            }
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