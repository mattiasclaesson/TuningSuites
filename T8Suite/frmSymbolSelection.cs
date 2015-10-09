using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace CommonSuite
{
    public partial class frmSymbolSelection : DevExpress.XtraEditors.XtraForm
    {
        

        SymbolCollection _sc = new SymbolCollection();

        public SymbolCollection Sc
        {
            get { return _sc; }
            set { _sc = value; }
        }
        public frmSymbolSelection()
        {
            InitializeComponent();
        }

        

        public void SetSymbolCollection(SymbolCollection sc)
        {
            //checkedListBoxControl1.Items.Add(varname);
            _sc = sc;
            gridControl1.DataSource = sc;
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

        public void SelectAllSymbols()
        {
            gridView1.SelectAll();
        }

        public SymbolCollection GetSelectedSymbolCollection()
        {
            // build a new collection based on the selected rows
            SymbolCollection scSelected = new SymbolCollection();
            if (gridControl1.DataSource != null)
            {
                //DataTable dt = (DataTable)gridControl1.DataSource;


                int[] selrows = gridView1.GetSelectedRows();
                foreach (int rowhandle in selrows)
                {
                    SymbolHelper sh = (SymbolHelper)gridView1.GetRow(rowhandle);
                    if (sh != null)
                    {
                        scSelected.Add(sh);
                    }
                }
            }
            return scSelected;

        }
    }
}