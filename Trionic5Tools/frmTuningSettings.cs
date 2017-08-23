using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Tools
{
    public partial class frmTuningSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmTuningSettings()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // options to set and get the parameters in the selection screen

        public TurboType Turbo
        {
            get
            {
                return (TurboType)comboBoxEdit1.SelectedIndex;
            }
            set
            {
                comboBoxEdit1.SelectedIndex = Convert.ToInt32(value);
            }
        }

        public InjectorType Injectors
        {
            get
            {
                return (InjectorType)comboBoxEdit2.SelectedIndex;
            }
            set
            {
                comboBoxEdit2.SelectedIndex = Convert.ToInt32(value);
            }
        }

        public MapSensorType MapSensor
        {
            get
            {
                return (MapSensorType)comboBoxEdit3.SelectedIndex;
            }
            set
            {
                comboBoxEdit3.SelectedIndex = Convert.ToInt32(value);
            }
        }


        public double PeakBoost
        {
            get
            {
                return Convert.ToDouble(spinEdit1.Value);
            }
            set
            {
                spinEdit1.Value = (decimal)value;
            }
        }

        public double BoostFirstGear
        {
            get
            {
                return Convert.ToDouble(spinEdit2.Value);
            }
            set
            {
                spinEdit2.Value = (decimal)value;
            }
        }

        public double BoostSecondGear
        {
            get
            {
                return Convert.ToDouble(spinEdit3.Value);
            }
            set
            {
                spinEdit3.Value = (decimal)value;
            }
        }

        public double BoostFuelcut
        {
            get
            {
                return Convert.ToDouble(spinEdit4.Value);
            }
            set
            {
                spinEdit4.Value = (decimal)value;
            }
        }
    }
}