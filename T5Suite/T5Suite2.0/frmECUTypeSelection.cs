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
    public partial class frmECUTypeSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmECUTypeSelection()
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
            DialogResult = DialogResult.OK;
        }

        public ECUType GetECUType()
        {
            return (ECUType)comboBox1.SelectedIndex;
        }
    }
}