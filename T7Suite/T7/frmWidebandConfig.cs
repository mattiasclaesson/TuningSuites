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
    public partial class frmWidebandConfig : DevExpress.XtraEditors.XtraForm
    {
        public frmWidebandConfig()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }


        public double WidebandLambdaLowVoltage
        {
            get
            {
                return ConvertToDouble(txtLowVoltage.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;

                txtLowVoltage.Text = realval.ToString();
            }
        }

        public double WidebandLambdaHighVoltage
        {
            get
            {
                return ConvertToDouble(txtHighVoltage.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighVoltage.Text = realval.ToString();
            }
        }

        public double WidebandLambdaLowAFR
        {
            get
            {
                return ConvertToDouble(txtLowAFR.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtLowAFR.Text = realval.ToString();
            }
        }

        public double WidebandLambdaHighAFR
        {
            get
            {
                return (ConvertToDouble(txtHighAFR.Text)) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighAFR.Text = realval.ToString();
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}