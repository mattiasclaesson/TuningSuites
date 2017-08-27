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

namespace T5Suite2
{
    public partial class frmMatrixSelection : DevExpress.XtraEditors.XtraForm
    {
        SymbolCollection _symbols;

        public frmMatrixSelection()
        {
            InitializeComponent();
        }

        public void SetSymbolList(SymbolCollection sc)
        {
            lookUpEdit1.Properties.DisplayMember = "Varname";
            lookUpEdit1.Properties.ValueMember = "Varname";
            lookUpEdit2.Properties.DisplayMember = "Varname";
            lookUpEdit2.Properties.ValueMember = "Varname";
            lookUpEdit3.Properties.DisplayMember = "Varname";
            lookUpEdit3.Properties.ValueMember = "Varname";
            lookUpEdit1.Properties.DataSource = sc;
            lookUpEdit2.Properties.DataSource = sc;
            lookUpEdit3.Properties.DataSource = sc;
            _symbols = sc;
        }

        public int GetViewType()
        {
            if (comboBoxEdit1.SelectedIndex < 0) comboBoxEdit1.SelectedIndex = 0;
            return comboBoxEdit1.SelectedIndex;
        }

        public string GetXAxisSymbol()
        {
            return (string)lookUpEdit1.EditValue;
        }
        public string GetYAxisSymbol()
        {
            return (string)lookUpEdit2.EditValue;
        }
        public string GetZAxisSymbol()
        {
            return (string)lookUpEdit3.EditValue;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        internal void SetXSelection(string xaxis)
        {
            try
            {
                if (xaxis != "")
                {
                    if (SymbolCollectionContains(xaxis)) lookUpEdit1.EditValue = xaxis;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private bool SymbolCollectionContains(string symbolname)
        {
            if (_symbols == null) return false;
            foreach (SymbolHelper sh in _symbols)
            {
                if (sh.Varname == symbolname) return true;
            }
            return false;
        }

        internal void SetYSelection(string yaxis)
        {
            try
            {
                if (yaxis != "")
                {
                    if (SymbolCollectionContains(yaxis)) lookUpEdit2.EditValue = yaxis;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        internal void SetZSelection(string zaxis)
        {
            try
            {
                if (zaxis != "")
                {
                    if (SymbolCollectionContains(zaxis)) lookUpEdit3.EditValue = zaxis;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}