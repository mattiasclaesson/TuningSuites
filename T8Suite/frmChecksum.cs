using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace T8SuitePro
{
    public partial class frmChecksum : DevExpress.XtraEditors.XtraForm
    {
        public frmChecksum()
        {
            InitializeComponent();
        }

        public string Layer
        {
            set
            {
                groupControl1.Text = value;
            }
        }

        public string FileChecksum
        {
            set
            {
                textEdit1.Text = value;
            }
        }

        public string RealChecksum
        {
            set
            {
                textEdit2.Text = value;
            }
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

    }
}