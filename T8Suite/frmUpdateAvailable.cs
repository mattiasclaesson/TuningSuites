using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace T8SuitePro
{
    public partial class frmUpdateAvailable : DevExpress.XtraEditors.XtraForm
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        
        public frmUpdateAvailable()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
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

        public void SetVersionNumber(string version)
        {
            labelControl2.Text = "Available version: " + version;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://develop.trionictuning.com/T8Suite/Notes.xml" );
        }

        private void frmUpdateAvailable_Load(object sender, EventArgs e)
        {

        }
    }
}