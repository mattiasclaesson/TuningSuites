using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace T7
{
    public partial class frmTuningWizard : DevExpress.XtraEditors.XtraForm
    {
        public frmTuningWizard()
        {
            InitializeComponent();
        }

        public string ExtraInfo
        {
            set
            {
                memoEdit4.Text = "Details " + Environment.NewLine + Environment.NewLine + value;
            }
        }

        public int DesiredHP
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


        public int PeakTorque
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

        public bool CarRunsE85
        {
            get
            {
                return checkEdit1.Checked;
            }
            set
            {
                checkEdit1.Checked = value;
            }
        }

        private void wizardPage1_PageCommit(object sender, EventArgs e)
        {
            memoEdit5.Text = "Settings to review" + Environment.NewLine + Environment.NewLine;
            memoEdit5.Text += "Peak torque: " + PeakTorque.ToString()+ " Nm" + Environment.NewLine;
            memoEdit5.Text += "Peak power: " + DesiredHP.ToString() +" bhp" + Environment.NewLine;
            if (CarRunsE85) memoEdit5.Text += "Fuel: Ethanol (E85)";
            else memoEdit5.Text += "Fuel: Petrol";
        }
    }
}