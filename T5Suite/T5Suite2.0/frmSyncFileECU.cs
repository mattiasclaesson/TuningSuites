using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T5Suite2
{
    public partial class frmSyncFileECU : DevExpress.XtraEditors.XtraForm
    {
        public frmSyncFileECU()
        {
            InitializeComponent();
        }

        public void SetInformation(string text)
        {
            labelControl5.Text = text;
        }

        public void SetTimeStamps(DateTime ts_file, DateTime ts_ecu)
        {
            labelControl4.Text = ts_file.ToString("dd/MM/yyyy HH:mm:ss");
            labelControl3.Text = ts_ecu.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            this.Close();
        }
    }
}