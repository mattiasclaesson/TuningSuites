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
    public partial class frmAutotuneSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmAutotuneSettings()
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

        public int CellStableTime_ms
        {
            get
            {
                return (int)spinEdit1.Value;
            }
            set
            {
                spinEdit1.Value = value;
            }
        }

        public int IgnitionCellStableTime_ms
        {
            get
            {
                return (int)spinEdit11.Value;
            }
            set
            {
                spinEdit11.Value = value;
            }
        }

        public int MinimumEngineSpeedForIgnitionTuning
        {
            get
            {
                return (int)spinEdit12.Value;
            }
            set
            {
                spinEdit12.Value = value;
            }
        }

        public double MaximumIgnitionAdvancePerSession
        {
            get
            {
                return Convert.ToDouble(spinEdit10.Value);
            }
            set
            {
                spinEdit10.Value = Convert.ToDecimal(value);
            }
        }

        public double IgnitionAdvancePerCycle
        {
            get
            {
                return Convert.ToDouble(spinEdit13.Value);
            }
            set
            {
                spinEdit13.Value = Convert.ToDecimal(value);
            }
        }

        public double IgnitionRetardFirstKnock
        {
            get
            {
                return Convert.ToDouble(spinEdit14.Value);
            }
            set
            {
                spinEdit14.Value = Convert.ToDecimal(value);
            }
        }

        public double IgnitionRetardFurtherKnocks
        {
            get
            {
                return Convert.ToDouble(spinEdit15.Value);
            }
            set
            {
                spinEdit15.Value = Convert.ToDecimal(value);
            }
        }

        public double GlobalMaximumIgnitionAdvance
        {
            get
            {
                return Convert.ToDouble(spinEdit16.Value);
            }
            set
            {
                spinEdit16.Value = Convert.ToDecimal(value);
            }
        }


        public int CorrectionPercentage
        {
            get
            {
                return (int)spinEdit3.Value;
            }
            set
            {
                spinEdit3.Value = value;
            }
        }

        public int AreaCorrectionPercentage
        {
            get
            {
                return (int)spinEdit4.Value;
            }
            set
            {
                spinEdit4.Value = value;
            }
        }

        public int AcceptableTargetErrorPercentage
        {
            get
            {
                return (int)spinEdit6.Value;
            }
            set
            {
                spinEdit6.Value = value;
            }
        }

        public int MaximumAdjustmentPerCyclePercentage
        {
            get
            {
                return (int)spinEdit7.Value;
            }
            set
            {
                spinEdit7.Value = value;
            }
        }

        public int EnrichmentFilter
        {
            get
            {
                return (int)spinEdit5.Value;
            }
            set
            {
                spinEdit5.Value = value;
            }
        }

        public int FuelCutDecayTime_ms
        {
            get
            {
                return (int)spinEdit2.Value;
            }
            set
            {
                spinEdit2.Value = value;
            }
        }

        public bool DiscardFuelcutMeasurements
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

        public bool DisableClosedLoopOnStartAutotune
        {
            get
            {
                return checkEdit4.Checked;
            }
            set
            {
                checkEdit4.Checked = value;
            }
        }

        public bool PlayCellProcessedSound
        {
            get
            {
                return checkEdit5.Checked;
            }
            set
            {
                checkEdit5.Checked = value;
            }
        }

        public bool CapIgnitionMap
        {
            get
            {
                return checkEdit8.Checked;
            }
            set
            {
                checkEdit8.Checked = value;
            }
        }


        public bool ResetFuelTrims
        {
            get
            {
                return checkEdit7.Checked;
            }
            set
            {
                checkEdit7.Checked = value;
            }
        }


        public bool AllowIdleAutoTune
        {
            get
            {
                return checkEdit6.Checked;
            }
            set
            {
                checkEdit6.Checked = value;
            }
        }

        
        public bool DiscardClosedThrottleMeasurements
        {
            get
            {
                return checkEdit2.Checked;
            }
            set
            {
                checkEdit2.Checked = value;
            }
        }

        public bool AutoUpdateFuelMap
        {
            get
            {
                return checkEdit3.Checked;
            }
            set
            {
                checkEdit3.Checked = value;
            }
        }

        public int MinimumAFRMeasurements
        {
            get
            {
                return (int)spinEdit8.Value;
            }
            set
            {
                spinEdit8.Value = value;
            }
        }

        public int MaximumAFRDeviance
        {
            get
            {
                return (int)spinEdit9.Value;
            }
            set
            {
                spinEdit9.Value = value;
            }
        }

    }
}