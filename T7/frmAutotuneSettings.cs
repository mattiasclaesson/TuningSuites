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