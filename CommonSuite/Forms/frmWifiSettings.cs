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
    public partial class frmWifiSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmWifiSettings()
        {
            InitializeComponent();
        }

        public int Port
        {
            get
            {
                return (int)spinEdit1.Value;
            }
            set
            {
                spinEdit1.Value = (decimal)value;
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