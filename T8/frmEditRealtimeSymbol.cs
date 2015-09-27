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
    public partial class frmEditRealtimeSymbol : DevExpress.XtraEditors.XtraForm
    {
        private string _varname;

        public string Varname
        {
            get { return _varname; }
            set { _varname = value; }
        }

        private int _symbolnumber;

        public int Symbolnumber
        {
            get { return _symbolnumber; }
            set { _symbolnumber = value; }
        }


        public double MinimumValue
        {
            get { return Convert.ToDouble(spinEdit1.EditValue); }
            set { spinEdit1.EditValue = value; }
        }
        public double MaximumValue
        {
            get { return Convert.ToDouble(spinEdit2.EditValue); }
            set { spinEdit2.EditValue = value; }
        }
        public double OffsetValue
        {
            get { return Convert.ToDouble(spinEdit3.EditValue); }
            set { spinEdit3.EditValue = value; }
        }
        public double CorrectionValue
        {
            get { return Convert.ToDouble(spinEdit4.EditValue); }
            set { spinEdit4.EditValue = value; }
        }

        public string Symbolname
        {
            get { return lookUpEdit1.EditValue.ToString(); }
            set { lookUpEdit1.EditValue = value; }
        }


        public string Description
        {
            get { return textEdit1.Text; }
            set { textEdit1.Text = value; }
        }

        public frmEditRealtimeSymbol()
        {
            InitializeComponent();
        }

        private SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }

        private void frmEditRealtimeSymbol_Load(object sender, EventArgs e)
        {
            lookUpEdit1.Properties.DataSource = m_symbols;
            lookUpEdit1.Properties.DisplayMember = "Varname";
            lookUpEdit1.Properties.ValueMember = "Varname";
        }

        private void lookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string varname = lookUpEdit1.EditValue.ToString();
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == varname)
                {
                    _varname = varname;
                    _symbolnumber = sh.Symbol_number;
                }
                float correction = GetSymbolCorrectionFactor(varname);
                float offset = GetSymbolOffset(varname);
                if (offset != 0)
                {
                    spinEdit3.EditValue = offset;
                }
                if(correction != 1)
                {
                    spinEdit4.EditValue = correction;
                }
            }
        }

        private float GetSymbolCorrectionFactor(string varname)
        {
            float returnvalue = 1;
            switch (varname)
            {
                case "P_Manifold":
                case "Regl_tryck":
                case "Max_tryck":
                    returnvalue = 0.01F;
                    break;
                case "P_Manifold10":
                    returnvalue = 0.001F;
                    break;
                case "Rpm":
                    returnvalue = 10;
                    break;
                case "Insptid_ms10":
                    returnvalue = 0.1F;
                    break;
            }
            return returnvalue;
        }

        private float GetSymbolOffset(string varname)
        {
            float returnvalue = 0;
            switch (varname)
            {
                case "P_Manifold":
                case "Regl_tryck":
                case "Max_tryck":
                case "P_Manifold10":
                    returnvalue = -1;
                    break;
            }
            return returnvalue;
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
    }
}