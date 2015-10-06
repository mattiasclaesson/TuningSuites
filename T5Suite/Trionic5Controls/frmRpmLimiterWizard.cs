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
    public partial class frmRpmLimiterWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmRpmLimiterWizard()
        {
            InitializeComponent();
        }

        public int SoftwareRPMLimit
        {
            get
            {
                return Convert.ToInt32(spinEdit1.Value);
            }
            set
            {
                spinEdit1.Value = value;
            }
        }

        public int HardcodedRPMLimit
        {
            get
            {
                return Convert.ToInt32(spinEdit2.Value);
            }
            set
            {
                spinEdit2.Value = value;
            }
        }

    }
}