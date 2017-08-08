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
    public partial class frmBoostAdaption : DevExpress.XtraEditors.XtraForm
    {
        public frmBoostAdaption()
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

        private void spinEdit5_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}