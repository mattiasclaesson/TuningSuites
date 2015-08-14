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

namespace T7
{
    public partial class frmEditTuningPackage : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string _tuningPackageFilename = string.Empty;

        public string TuningPackageFilename
        {
            get { return _tuningPackageFilename; }
            set { _tuningPackageFilename = value; }
        }

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

                object o = e.Data.GetData("T7.SymbolHelper");
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

                object o = e.Data.GetData("T7.SymbolHelper");
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

        public delegate void MapSelected(object sender, MapSelectedEventArgs e);
        public event frmEditTuningPackage.MapSelected onMapSelected;


        private void CastOpenMapViewersEvent(string symbolname, string data, string filename)
        {
            if (onMapSelected != null)
            {
                onMapSelected(this, new MapSelectedEventArgs(symbolname, data, filename));
            }

        }

        public class MapSelectedEventArgs : System.EventArgs
        {
            private string _data;

            public string Data
            {
                get { return _data; }
                set { _data = value; }
            }

            private string _filename;

            public string Filename
            {
                get { return _filename; }
                set { _filename = value; }
            }


            private string _mapname;

            public string Mapname
            {
                get { return _mapname; }
                set { _mapname = value; }
            }

            public MapSelectedEventArgs(string mapname, string data, string filename)
            {
                this._mapname = mapname;
                this._data = data;
                this._filename = filename;
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            // show two mapviewers: 1 from the currently opened binary file and one from the tuning package content
            System.Windows.Forms.Application.DoEvents();
            //this.LayoutMdi(MdiLayout.Cascade);
            int[] selectedrows = gridView1.GetSelectedRows();

            if (selectedrows.Length > 0)
            {
                int grouplevel = gridView1.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridView1.GroupCount)
                {
                    int[] selrows = gridView1.GetSelectedRows();
                    if (selrows.Length > 0)
                    {
                        //dt.Columns.Add("Map");
                        //dt.Columns.Add("Length");
                        //dt.Columns.Add("Data");
                        DataRow dr = gridView1.GetDataRow((int)selrows.GetValue(0));
                        CastOpenMapViewersEvent(dr["Map"].ToString(), dr["Data"].ToString(), _tuningPackageFilename);
                    }
                }
            }
        }

        internal void SetFilename(string filename)
        {
            _tuningPackageFilename = filename;
        }

        internal void SetDataForSymbol(string symbolName, string symbolData)
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Map"].ToString() == symbolName)
                    {
                        dr["Data"] = symbolData;
                    }
                }
            }
            Application.DoEvents();
        }
    }
}