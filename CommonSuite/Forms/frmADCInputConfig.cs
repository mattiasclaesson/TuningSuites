using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CommonSuite
{
    public partial class frmADCInputConfig : DevExpress.XtraEditors.XtraForm
    {
        public frmADCInputConfig()
        {
            InitializeComponent();
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

        public string ChannelName
        {
            get
            {
                return textEdit1.Text;
            }
            set
            {
                textEdit1.Text = value;
            }
        }

        public double LowVoltage
        {
            get
            {
                return ConvertToDouble(txtLowVoltage.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtLowVoltage.Text = realval.ToString("F2");
            }
        }

        public double HighVoltage
        {
            get
            {
                return ConvertToDouble(txtHighVoltage.Text)* 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighVoltage.Text = realval.ToString("F2");
            }
        }

        public double LowValue
        {
            get
            {
                return ConvertToDouble(txtLowValue.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtLowValue.Text = realval.ToString("F2");
            }
        }

        public double HighValue
        {
            get
            {
                return ConvertToDouble(txtHighValue.Text) * 1000;
            }
            set
            {
                double realval = value / 1000;
                txtHighValue.Text = realval.ToString("F2");
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}