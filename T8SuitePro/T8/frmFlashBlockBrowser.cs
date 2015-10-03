using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T8SuitePro
{
    public partial class frmFlashBlockBrowser : DevExpress.XtraEditors.XtraForm
    {
        public frmFlashBlockBrowser()
        {
            InitializeComponent();
        }

        internal void SetData(DataTable dt)
        {
            gridControl1.DataSource = dt;
            Application.DoEvents();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}