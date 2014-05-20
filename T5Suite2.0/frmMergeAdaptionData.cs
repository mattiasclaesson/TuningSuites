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
    public partial class frmMergeAdaptionData : DevExpress.XtraEditors.XtraForm
    {
        public frmMergeAdaptionData()
        {
            InitializeComponent();
        }

        public bool AllowSpotAdaption
        {
            get
            {
                return checkEdit1.Checked;
            }
        }

        public bool AllowLongTermFuelTrimAdaption
        {
            get
            {
                return checkEdit2.Checked;
            }
        }

        public bool AllowIdleFuelTrimAdaption
        {
            get
            {
                return checkEdit3.Checked;
            }
        }

        public bool AllowIgnitionAdaptionBasedOnKnockInformation
        {
            get
            {
                return checkEdit4.Checked;
            }
        }

        public bool AllowCylinderCorrectionBasedOnKnockInformation
        {
            get
            {
                return checkEdit5.Checked;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }



    }
}