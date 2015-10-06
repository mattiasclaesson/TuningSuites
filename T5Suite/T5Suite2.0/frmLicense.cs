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
    public partial class frmLicense : DevExpress.XtraEditors.XtraForm
    {
        public frmLicense()
        {
            InitializeComponent();
        }

        private bool immoValid = false;

        public bool ImmoValid
        {
            get { return immoValid; }
            set
            {
                immoValid = value;
                if (immoValid)
                {
                    textEdit2.Enabled = false;
                    textEdit2.Text = "••••••••••••••••••••••••••••••••";
                    this.Text = "License code status: VALID";
                }
            }
        }

        public string GetLicenseCode()
        {
            return textEdit2.Text;
        }

        public void SetHWID(string ID)
        {
            textEdit1.Text = ID;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textEdit1.Text);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}