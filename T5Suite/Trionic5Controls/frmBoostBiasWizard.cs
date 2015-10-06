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
    public partial class frmBoostBiasWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmBoostBiasWizard()
        {
            InitializeComponent();
        }

        public int RangeStep
        {
            get
            {
                return Convert.ToInt32(spinEdit1.Value);
            }
            set
            {
                spinEdit1.Value = value;
                CalcRanges();
            }
        }

        private void CalcRanges()
        {
            int _rangeUpto = 2500 + (Convert.ToInt32(spinEdit1.EditValue) * 30 * 10);
            RangeUpto = _rangeUpto;
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

        public int RangeFrom
        {
            get
            {
                return Convert.ToInt32(spinEdit4.Value);
            }
            set
            {
                spinEdit4.Value = value;
            }
        }

        public int RangeUpto
        {
            get
            {
                return Convert.ToInt32(spinEdit3.Value);
            }
            set
            {
                spinEdit3.Value = value;
            }
        }

        private void spinEdit1_ValueChanged(object sender, EventArgs e)
        {
            CalcRanges();
        }
    }
}