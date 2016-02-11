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
                dateEdit1.EditValue = _startdate;
            }
        }
        private DateTime _enddate;

        public DateTime Enddate
        {
            get { return _enddate; }
            set
            {
                _enddate = value;
                dateEdit2.EditValue = _enddate;
            }
        }

        SymbolCollection _sc = new SymbolCollection();

        public SymbolCollection Sc
        {
            get { return _sc; }
            set { _sc = value; }
        }

        SuiteRegistry _suiteRegistry;

        public frmPlotSelection(SuiteRegistry suiteRegistry)
        {
            InitializeComponent();
            _suiteRegistry = suiteRegistry;
        }

        private void SaveRegistrySetting(string key, int value)
        {
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            using (RegistryKey saveSettings = SuiteKey.CreateSubKey("SymbolColors"))
            {
                saveSettings.SetValue(key, value.ToString(), RegistryValueKind.String);
            }
        }

        private Int32 GetValueFromRegistry(string symbolname)
        {
            Int32 win32color = 0;
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey(_suiteRegistry.getRegistryPath());

            using (RegistryKey Settings = SuiteKey.CreateSubKey("SymbolColors"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == symbolname)
                            {
                                string value = Settings.GetValue(a).ToString();
                                win32color = Convert.ToInt32(value);
                            }
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
            }
            return win32color;
        }

        private void SaveColorToRegistry(string symbolname, Color c)
        {
            int win32color = c.ToArgb();// System.Drawing.ColorTranslator.ToAr(c);
            SaveRegistrySetting(symbolname, win32color);
        }

        private Color GetColorFromRegistry(string symbolname)
        {
            Color c = Color.Black;
            Int32 win32color = GetValueFromRegistry(symbolname);
            c = Color.FromArgb((int)win32color);
             //c = System.Drawing.ColorTranslator.FromWin32(win32color);
            return c;
        }

        public void AddItemToList(string varname)
        {
            //checkedListBoxControl1.Items.Add(varname);
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                dt.Rows.Add(varname, GetColorFromRegistry(varname).ToArgb());
                //logger.Debug(varname + " got color: " + GetColorFromRegistry(varname).ToArgb().ToString());
                //dt.Rows.Add(varname, Color.Red.ToArgb());
            }
            else
            {
                DataTable dt = new DataTable();
                dt.TableName = "SymbolColors";
                dt.Columns.Add("SYMBOLNAME");
                
                dt.Columns.Add("COLOR", Type.GetType("System.Int32"));
                dt.Rows.Add(varname, GetColorFromRegistry(varname).ToArgb());
                //logger.Debug(varname + " got color: " + GetColorFromRegistry(varname).ToArgb().ToString());
                gridControl1.DataSource = dt;
            }
            UpdateColors();
        }

        public void UpdateColors()
        {
            
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
                    SaveColorToRegistry(dr["SYMBOLNAME"].ToString(), Color.FromArgb(Convert.ToInt32(dr["COLOR"])));
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

            /*
            foreach (DevExpress.XtraEditors.Controls.CheckedListBoxItem item in checkedListBoxControl1.Items)
            {
                if (item.CheckState == CheckState.Checked)
                {
                    SymbolHelper sh = new SymbolHelper();
                    sh.Varname = item.Value.ToString();
                    _sc.Add(sh);
                }
            }*/
            this.Close();
        }

        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEdit1.EditValue != null)
            {
                if (dateEdit1.EditValue != DBNull.Value)
                {
                    _startdate = Convert.ToDateTime(dateEdit1.EditValue);
                }
            }
        }

        private void dateEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (dateEdit2.EditValue != null)
            {
                if (dateEdit2.EditValue != DBNull.Value)
                {
                    _enddate = Convert.ToDateTime(dateEdit2.EditValue);
                }
            }

        }


        public void SelectAllSymbols()
        {
            gridView1.SelectAll();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // setup the export filters
            LogFilters filterhelper = new LogFilters() { SuiteRegistry = _suiteRegistry };
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