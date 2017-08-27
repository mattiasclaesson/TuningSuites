using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Controls
{
    public partial class CompareResults : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void NotifySelectSymbol(object sender, SelectSymbolEventArgs e);
        public event CompareResults.NotifySelectSymbol onSymbolSelect;
        private AddressLookupCollection m_compareAddressLookupCollection = new AddressLookupCollection();

        public AddressLookupCollection CompareAddressLookupCollection
        {
            get { return m_compareAddressLookupCollection; }
            set { m_compareAddressLookupCollection = value; }
        }

        private SymbolCollection m_compareSymbolCollection = new SymbolCollection();

        public SymbolCollection CompareSymbolCollection
        {
            get { return m_compareSymbolCollection; }
            set { m_compareSymbolCollection = value; }
        }

        private Trionic5File m_ComparedTrionic5File = new Trionic5File();

        public Trionic5File ComparedTrionic5File
        {
            get { return m_ComparedTrionic5File; }
            set { m_ComparedTrionic5File = value; }
        }

        private Trionic5FileInformation m_ComparedTrionic5FileInformation = new Trionic5FileInformation();

        public Trionic5FileInformation ComparedTrionic5FileInformation
        {
            get { return m_ComparedTrionic5FileInformation; }
            set { m_ComparedTrionic5FileInformation = value; }
        }

        private string m_filename = "";

        public string Filename
        {
            get { return m_filename; }
            set { m_filename = value; }
        }

        public CompareResults()
        {
            InitializeComponent();
        }

        public void SetGridWidth()
        {
            gridView1.BestFitColumns();
        }

        private void CastSelectEvent(int m_map_address, int m_map_length, string m_map_name)
        {
            if (onSymbolSelect != null)
            {
                // haal eerst de data uit de tabel van de gridview
                onSymbolSelect(this, new SelectSymbolEventArgs(m_map_address, m_map_length, m_map_name, m_filename, false, m_compareSymbolCollection, m_compareAddressLookupCollection, m_ComparedTrionic5File, m_ComparedTrionic5FileInformation));
            }
        }

        private void CastDifferenceEvent(int m_map_address, int m_map_length, string m_map_name)
        {
            if (onSymbolSelect != null)
            {
                // haal eerst de data uit de tabel van de gridview
                onSymbolSelect(this, new SelectSymbolEventArgs(m_map_address, m_map_length, m_map_name, m_filename, true, m_compareSymbolCollection, m_compareAddressLookupCollection, m_ComparedTrionic5File, m_ComparedTrionic5FileInformation));
            }
        }

        public void OpenGridViewGroups(GridControl ctrl, int groupleveltoexpand)
        {
            // open grouplevel 0 (if available)
            ctrl.BeginUpdate();
            try
            {
                GridView view = (GridView)ctrl.DefaultView;
                //view.ExpandAllGroups();
                view.MoveFirst();
                while (!view.IsLastRow)
                {
                    int rowhandle = view.FocusedRowHandle;
                    if (view.IsGroupRow(rowhandle))
                    {
                        int grouplevel = view.GetRowLevel(rowhandle);
                        if (grouplevel <= groupleveltoexpand)
                        {
                            view.ExpandGroupRow(rowhandle);
                        }
                    }
                    view.MoveNext();
                }
                view.MoveFirst();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            ctrl.EndUpdate();
        }

        private void StartTableViewer()
        {
            if (gridView1.SelectedRowsCount > 0)
            {
                int[] selrows = gridView1.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    DataRowView dr = (DataRowView)gridView1.GetRow((int)selrows.GetValue(0));
                    string Map_name = dr.Row["SYMBOLNAME"].ToString();
                    int address = Convert.ToInt32(dr.Row["FLASHADDRESS"].ToString());
                    int length = Convert.ToInt32(dr.Row["LENGTHBYTES"].ToString());
                    CastSelectEvent(address, length, Map_name);
                }
            }

        }


        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int[] selectedrows = gridView1.GetSelectedRows();
            if (selectedrows.Length > 0)
            {
                int grouplevel = gridView1.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridView1.GroupCount)
                {
                    StartTableViewer();
                }
            }
        }

        public class SelectSymbolEventArgs : System.EventArgs
        {
            private int _address;
            private int _length;
            private string _mapname; 
            private string _filename;
            private bool _showdiffmap;
            private SymbolCollection _symbols;

            public SymbolCollection Symbols
            {
                get { return _symbols; }
                set { _symbols = value; }
            }
            private AddressLookupCollection _addresses;

            public AddressLookupCollection Addresses
            {
                get { return _addresses; }
                set { _addresses = value; }
            }

            private Trionic5File m_CompTrionic5File;

            public Trionic5File CompTrionic5File
            {
                get { return m_CompTrionic5File; }
                set { m_CompTrionic5File = value; }
            }

            private Trionic5FileInformation m_CompTrionic5FileInformation;

            public Trionic5FileInformation CompTrionic5FileInformation
            {
                get { return m_CompTrionic5FileInformation; }
                set { m_CompTrionic5FileInformation = value; }
            }

            public bool ShowDiffMap
            {
                get
                {
                    return _showdiffmap;
                }
            }

            public int SymbolAddress
            {
                get
                {
                    return _address;
                }
            }

            public int SymbolLength
            {
                get
                {
                    return _length;
                }
            }

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }

            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public SelectSymbolEventArgs(int address, int length, string mapname, string filename, bool showdiffmap, SymbolCollection symColl, AddressLookupCollection adrColl, Trionic5File _file, Trionic5FileInformation _fileinfo)
            {
                this._address = address;
                this._length = length;
                this._mapname = mapname;
                this._filename = filename;
                this._showdiffmap = showdiffmap;
                this._symbols = symColl;
                this._addresses = adrColl;
                this.m_CompTrionic5File = _file;
                this.m_CompTrionic5FileInformation = _fileinfo;
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartTableViewer();
                e.Handled = true;
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == gridColumn6.Name)
            {
                object o = gridView1.GetRowCellValue(e.RowHandle, "XDFCATEGORY");
                Color c = Color.White;
                if (o != DBNull.Value)
                {
                    if (Convert.ToInt32(o) == (int)XDFCategories.Fuel)
                    {
                        c = Color.LightSteelBlue;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Ignition)
                    {
                        c = Color.LightGreen;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Boost_control)
                    {
                        c = Color.OrangeRed;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Misc)
                    {
                        c = Color.LightGray;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Sensor)
                    {
                        c = Color.Yellow;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Correction)
                    {
                        c = Color.LightPink;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Idle)
                    {
                        c = Color.BurlyWood;
                    }
                }
                if (c != Color.White)
                {
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, c, Color.White, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(gb, e.Bounds);
                }

            }

        }

        private void showDifferenceMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // trek de ene map van de andere af en toon het resultaat in een mapviewer!
            if (gridView1.SelectedRowsCount > 0)
            {
                int[] selrows = gridView1.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    DataRowView dr = (DataRowView)gridView1.GetRow((int)selrows.GetValue(0));
                    string Map_name = dr.Row["SYMBOLNAME"].ToString();
                    int address = Convert.ToInt32(dr.Row["FLASHADDRESS"].ToString());
                    int length = Convert.ToInt32(dr.Row["LENGTHBYTES"].ToString());
                    CastDifferenceEvent(address, length, Map_name);
                }
            }
        }


    }
}
