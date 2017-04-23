using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using NLog;

namespace CommonSuite
{
    public partial class frmPlotSelection : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DateTime _startdate;

        public DateTime Startdate
        {
            get { return _startdate; }
            set
            {
                _startdate = value;
                dateEditFrom.EditValue = _startdate;
            }
        }
        private DateTime _enddate;

        public DateTime Enddate
        {
            get { return _enddate; }
            set
            {
                _enddate = value;
                dateEditTo.EditValue = _enddate;
            }
        }

        SymbolCollection _sc = new SymbolCollection();

        public SymbolCollection Sc
        {
            get { return _sc; }
            set { _sc = value; }
        }

        SuiteRegistry _suiteRegistry;
        SymbolColors _symbolColors;

        public frmPlotSelection(SuiteRegistry suiteRegistry)
        {
            InitializeComponent();
            _suiteRegistry = suiteRegistry;
            _symbolColors = new SymbolColors(suiteRegistry);
        }

        public void AddItemToList(string varname)
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                dt.Rows.Add(varname, _symbolColors.GetColorFromRegistry(varname).ToArgb());
            }
            else
            {
                DataTable dt = new DataTable();
                dt.TableName = "SymbolColors";
                dt.Columns.Add("SYMBOLNAME");
                
                dt.Columns.Add("COLOR", Type.GetType("System.Int32"));
                dt.Rows.Add(varname, _symbolColors.GetColorFromRegistry(varname).ToArgb());
                gridControl1.DataSource = dt;
            }
            UpdateColors();
        }

        public void UpdateColors()
        {
            gridControl1.Update();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
           
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;

                foreach (DataRow dr in dt.Rows)
                {
                    _symbolColors.SaveColorToRegistry(dr["SYMBOLNAME"].ToString(), Color.FromArgb(Convert.ToInt32(dr["COLOR"])));
                }

                int[] selrows = gridView1.GetSelectedRows();
                foreach (int rowhandle in selrows)
                {
                    DataRowView dv = (DataRowView)gridView1.GetRow(rowhandle);
                    if (dv != null)
                    {
                        SymbolHelper sh = new SymbolHelper();
                        sh.Varname = dv.Row["SYMBOLNAME"].ToString();
                        sh.Color = Color.FromArgb(Convert.ToInt32(dv.Row["COLOR"]));
                        _sc.Add(sh);
                    }
                }
                
            }
            this.Close();
        }

        private void dateEditFrom_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEditFrom.EditValue != null)
            {
                if (dateEditFrom.EditValue != DBNull.Value)
                {
                    _startdate = Convert.ToDateTime(dateEditFrom.EditValue);
                }
            }
        }

        private void dateEditTo_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEditTo.EditValue != null)
            {
                if (dateEditTo.EditValue != DBNull.Value)
                {
                    _enddate = Convert.ToDateTime(dateEditTo.EditValue);
                }
            }

        }

        public void SelectAllSymbols()
        {
            gridView1.SelectAll();
        }

        private void btnFilters_Click(object sender, EventArgs e)
        {
            // setup the export filters
            LogFilters filterhelper = new LogFilters(_suiteRegistry);
            frmLogFilters frmfilters = new frmLogFilters();
            LogFilterCollection filters = filterhelper.GetFiltersFromRegistry();
            logger.Debug("filters: " + filters.Count);
            frmfilters.SetFilters(filters);
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;

                int[] selrows = gridView1.GetSelectedRows();
                foreach (int rowhandle in selrows)
                {
                    DataRowView dv = (DataRowView)gridView1.GetRow(rowhandle);
                    if (dv != null)
                    {
                        SymbolHelper sh = new SymbolHelper();
                        sh.Varname = dv.Row["SYMBOLNAME"].ToString();
                        sh.Color = Color.FromArgb(Convert.ToInt32(dv.Row["COLOR"]));
                        _sc.Add(sh);
                    }
                }

            }
            frmfilters.SetSymbols(_sc);
            if (frmfilters.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                filterhelper.SaveFiltersToRegistry(frmfilters.GetFilters());
            }
        }
    }
}