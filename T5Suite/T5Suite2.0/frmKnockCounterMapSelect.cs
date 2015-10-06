using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmKnockCounterMapSelect : DevExpress.XtraEditors.XtraForm
    {
        public frmKnockCounterMapSelect()
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            // selected row = ok
            if (gridView1.SelectedRowsCount > 0)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public void SetDataSource(KnockMapInfoCollection dt)
        {
            if (dt != null)
            {
                gridControl1.DataSource = dt;
            }
        }

        public string GetKnockMapFilename()
        {
            string retval = string.Empty;
            if (gridView1.SelectedRowsCount > 0)
            {
                int[] rows = gridView1.GetSelectedRows();
                if (rows.Length > 0)
                {
                    Trionic5Tools.KnockMapInfo dv = (Trionic5Tools.KnockMapInfo)gridView1.GetRow(Convert.ToInt32(rows.GetValue(0)));
                    if (dv != null)
                    {
                        //sh.Varname = dv.Row["SYMBOLNAME"].ToString();
                        if (dv.FileName != string.Empty)
                        {
                            retval = dv.FileName;
                        }
                    }
                }
            }
            return retval;
        }

        public string GetKnockMapFilenameCompare()
        {
            string retval = string.Empty;
            if (gridView1.SelectedRowsCount > 0)
            {
                int[] rows = gridView1.GetSelectedRows();
                if (rows.Length > 1)
                {
                    Trionic5Tools.KnockMapInfo dv = (Trionic5Tools.KnockMapInfo)gridView1.GetRow(Convert.ToInt32(rows.GetValue(1)));
                    if (dv != null)
                    {
                        //sh.Varname = dv.Row["SYMBOLNAME"].ToString();
                        if (dv.FileName != string.Empty)
                        {
                            retval = dv.FileName;
                        }
                    }
                }
            }
            return retval;
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (gridView1.GetSelectedRows().Length == 2)
            {
                btnCompareKnockMaps.Enabled = true;
            }
            else
            {
                btnCompareKnockMaps.Enabled = false;
            }
        }

        private void btnCompareKnockMaps_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}