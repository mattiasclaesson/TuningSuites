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
    public partial class frmBoostAdaptionWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmBoostAdaptionWizard()
        {
            InitializeComponent();
        }

        public int ManualRPMLow
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

        public int ManualRPMHigh
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

        public int AutomaticRPMLow
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

        public int AutomaticRPMHigh
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

        public int BoostError
        {
            get
            {
                return (int)(Convert.ToDouble(spinEdit5.Value) * 100);
            }
            set
            {
                double temp = value;
                temp /= 100;
                spinEdit5.Value = (decimal)temp;
            }
        }
    }
}