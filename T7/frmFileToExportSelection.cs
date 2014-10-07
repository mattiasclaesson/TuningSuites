using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace CommonSuite
{
    public partial class frmFileToExportSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmFileToExportSelection()
        {
            InitializeComponent();
        }

        public void SetOriginalFileName(string name)
        {
            simpleButton1.Text = Path.GetFileNameWithoutExtension(name);
        }

        public void SetCompareFileName(string name)
        {
            simpleButton2.Text = Path.GetFileNameWithoutExtension(name);
        }

        private bool _useOriginalFile = false;

        public bool UseOriginalFile
        {
            get { return _useOriginalFile; }
            set { _useOriginalFile = value; }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _useOriginalFile = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            _useOriginalFile = false;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}