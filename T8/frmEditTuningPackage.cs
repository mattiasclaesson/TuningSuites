using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public partial class frmEditTuningPackage : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public frmEditTuningPackage()
        {
            InitializeComponent();
        }

        public void SetDataTable(DataTable dt)
        {
            gridControl1.DataSource = dt;
        }

        public DataTable GetDataTable()
        {
            return (DataTable)gridControl1.DataSource;
        }

        private void gridControl1_DragOver(object sender, DragEventArgs e)
        {
            // kijken of het mag
            if (e.Data is System.Windows.Forms.DataObject)
            {

                object o = e.Data.GetData("T8SuitePro.SymbolHelper");
                if (o is SymbolHelper)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }

        }

        private void gridControl1_DragDrop(object sender, DragEventArgs e)
        {
            // what symbol
            // kijken of het mag
            if (e.Data is System.Windows.Forms.DataObject)
            {

                object o = e.Data.GetData("T8SuitePro.SymbolHelper");
                if (o is SymbolHelper)
                {
                    SymbolHelper sh = (SymbolHelper)o;
                    logger.Debug("Dropped: " + sh.Varname);
                    AddSymbolToTuningPackage(sh);
                }
            }
        }

        private string ConvertToByteArray(byte[] data)
        {
            string retval = string.Empty;
            foreach (byte b in data)
            {
                retval += b.ToString("X2") + ",";
            }
            return retval;
        }

        private void AddSymbolToTuningPackage(SymbolHelper sh)
        {
            // check whether already exists
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                bool _found = false;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Map"] != DBNull.Value)
                    {
                        if (dr["Map"].ToString() == sh.Varname)
                        {
                            _found = true;
                            dr["Data"] = ConvertToByteArray(sh.Currentdata); // update ?
                        }
                    }
                }
                if (!_found)
                {
                    dt.Rows.Add(sh.Varname, sh.Length, ConvertToByteArray(sh.Currentdata));
                }
            }
            else
            {
                // nieuwe maken
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("Map");
                dt.Columns.Add("Length");
                dt.Columns.Add("Data");
                dt.Rows.Add(sh.Varname, sh.Length, ConvertToByteArray(sh.Currentdata));
                gridControl1.DataSource = dt;
            }
        }

        private void DeleteSymbolFromTuningPackage(string symbolname)
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Map"] != DBNull.Value)
                    {
                        if (dr["Map"].ToString() == symbolname)
                        {
                            dt.Rows.Remove(dr);
                            break;
                        }
                    }
                }
            }
        }


        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool _writeData = false;

        public bool WriteData
        {
            get { return _writeData; }
            set { _writeData = value; }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _writeData = true;
            this.Close();
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // remove selected symbols
                if (gridView1.SelectedRowsCount > 0)
                {
                    int[] selrows = gridView1.GetSelectedRows();
                    if (selrows.Length > 0)
                    {
                        foreach (int i in selrows)
                        {
                            if (i >= 0)
                            {
                                
                                DataRowView drv = (DataRowView)gridView1.GetRow(i);
                                DeleteSymbolFromTuningPackage(drv.Row["Map"].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}