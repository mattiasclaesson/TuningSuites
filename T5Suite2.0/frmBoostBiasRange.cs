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
    public partial class frmBoostBiasRange : DevExpress.XtraEditors.XtraForm
    {
        public frmBoostBiasRange()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
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