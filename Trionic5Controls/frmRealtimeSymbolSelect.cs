using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CommonSuite;

namespace Trionic5Controls
{
    public partial class frmRealtimeSymbolSelect : DevExpress.XtraEditors.XtraForm
    {
        public frmRealtimeSymbolSelect()
        {
            InitializeComponent();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public string GetSelectedSymbol()
        {
            string retval = string.Empty;
            if (lookUpEdit1.EditValue is string)
            {
                retval = (string)lookUpEdit1.EditValue;
            }
            return retval;
        }

        public void SetCorrectionFactor(double factor)
        {
            spinEdit1.EditValue = factor;
        }

        public void SetCorrectionOffset(double offset)
        {
            spinEdit2.EditValue = offset;
        }

        public void SetSelectedSymbol(string symbolname)
        {
            lookUpEdit1.EditValue = symbolname;
            lookUpEdit1.Enabled = false;
        }

        public double GetSelectedCorrectionFactor()
        {
            double retval = 1;
            try
            {
                retval = Convert.ToDouble(spinEdit1.EditValue);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }
        public double GetSelectedCorrectionOffset()
        {
            double retval = 1;
            try
            {
                retval = Convert.ToDouble(spinEdit2.EditValue);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }


        internal void SetCollection(SymbolCollection m_RealtimeSymbolCollection)
        {
            lookUpEdit1.Properties.DataSource = m_RealtimeSymbolCollection;
        }
    }
}