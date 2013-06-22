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