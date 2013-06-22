using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace T7
{
    public partial class frmSplash : DevExpress.XtraEditors.XtraForm
    {
        public frmSplash()
        {
            InitializeComponent();
            label1.Text = "Version " + Application.ProductVersion.ToString();
        }

        public void SetVersion(string version)
        {
        }

    }
}