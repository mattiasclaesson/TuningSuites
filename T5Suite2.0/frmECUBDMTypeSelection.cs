using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using T5CANLib;

namespace T5Suite2
{
    public partial class frmECUBDMTypeSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmECUBDMTypeSelection()
        {
            InitializeComponent();
        }

        private void frmECUTypeSelection_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 4;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 4)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        public ecu_t GetECUType()
        {
            return (ecu_t)comboBox1.SelectedIndex;
        }
    }
}