using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Trionic5Controls
{
    public partial class frmDTCCodes : DevExpress.XtraEditors.XtraForm
    {
        public delegate void ClearErrorCodes(object sender, EventArgs e);
        public event frmDTCCodes.ClearErrorCodes onClearErrorCodes;

        public frmDTCCodes()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            simpleButton2.Enabled = false;
            Application.DoEvents();
            CastClearCodesEvent();
        }

        public void SetDataSet(DataTable dt)
        {
            gridControl1.DataSource = dt;
            simpleButton2.Enabled = true;
            Application.DoEvents();
        }

        private void CastClearCodesEvent()
        {
            if (onClearErrorCodes != null)
            {
                onClearErrorCodes(this, EventArgs.Empty);
                Console.WriteLine("Event cast");
            }
        }
    }
}