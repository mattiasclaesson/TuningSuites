using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CommonSuite;
using NLog;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;


namespace T8SuitePro
{
    public partial class frmTEM : DevExpress.XtraEditors.XtraForm
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        // Check this one some more. It must be disposed of
        private RepositoryItemLookUpEdit UsableSymbols = new RepositoryItemLookUpEdit();
        public PidCollection tems = null;

        public frmTEM()
        {
            InitializeComponent();

            this.tems = Form1.m_tems.CopyOf();
            gridControl1.DataSource = this.tems;
            PopulateSymbols();

            this.cSymbol.ColumnEdit = this.UsableSymbols;

            // Set cell formatting
            this.cSymbolAddress.DisplayFormat.FormatType = FormatType.Numeric;
            this.cSymbolAddress.DisplayFormat.FormatString = "X6";
            this.cSymbolSize.DisplayFormat.FormatType = FormatType.Numeric;
            this.cSymbolSize.DisplayFormat.FormatString = "0";
            this.cSymbolType.DisplayFormat.FormatType = FormatType.Numeric;
            this.cSymbolType.DisplayFormat.FormatString = "X2";

            // Register event handlers
            this.cSymbol.View.CellValueChanged += this.onSymbolChange;
            this.gridView1.ValidatingEditor += this.onEditValidation;
            this.gridView1.CustomDrawCell += this.onDrawCell;
            this.gridView1.ShowingEditor += this.onEditAttempt;

            // Experiment: Let go of editor window if scrolling outside of it
            this.gridControl1.MouseWheel += onMouseScroll;

            // Change window title to reflect number of items in table
            this.Text = "TEM editor (" + this.tems.Count.ToString() + " items)";
        }

        // Copy tems to a local collection
        private void PopulateSymbols()
        {
            DataTable dt = new DataTable("IDValues");
            dt.Columns.Add("#");
            dt.Columns.Add("Symbol");

            // Copy most junk as long as it has an address and is not of type typedef
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if ((sh.Symbol_type != 0x20 || sh.Length > 0) && sh.Internal_address > 0)
                {
                    dt.Rows.Add(sh.Symbol_number_ECU.ToString(), sh.Varname);
                }
            }

            // Copy broken symbols that are already present in the tem table
            foreach (PidHelper ph in this.tems)
            {
                // Prevent "OFF" from getting a symbol name
                if (ph.IsProtected == false)
                {
                    SymbolHelper sh = Form1.m_symbols.SymbolWithIndex(ph.SymbolIndex);
                    if ((sh.Symbol_type == 0x20 && sh.Length == 0) || sh.Internal_address  == 0)
                    {
                        dt.Rows.Add(sh.Symbol_number_ECU.ToString(), sh.Varname);
                    }
                }
            }

            this.UsableSymbols.ValueMember = "#";
            this.UsableSymbols.DisplayMember = "Symbol";
            this.UsableSymbols.DataSource = dt;
            this.UsableSymbols.PopupWidth = 400;

            // It has a "wonderful" behaviour where you're forced to change cell or push enter for it to actually perform an update after a symbol has changed. Fix!
            // (why is it ignoring most settings?)
            this.UsableSymbols.ValidateOnEnterKey = false;
            // this.UsableSymbols.AutoHeight = true;
            this.UsableSymbols.AllowMouseWheel = true;
            this.UsableSymbols.DropDownRows = 20;
            this.UsableSymbols.PopulateColumns();

            // Hackjob AB!
            string desc;
            int address, size, type;
            foreach (PidHelper ph in this.tems)
            {
                if (ph.IsProtected == false)
                {
                    GetSymbolData(ph.SymbolIndex, out desc, out address, out size, out type);
                    ph.SymbolDescription = desc;
                    ph.SymbolAddress = address;
                    ph.SymbolSize = size;
                    ph.SymbolType = type;
                }
                else
                {
                    ph.SymbolDescription = "This message is displayed in idle mode (No data is read)";
                }
            }
        }

        void onMouseScroll(object sender, MouseEventArgs e)
        {
            GridControl grid = sender as GridControl;
            grid.FocusedView.CloseEditor();
        }  

        /// <summary>
        /// Prevent user from editing protected items!
        /// Not the most elegant solution but it is what it is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEditAttempt(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int row = gridView1.FocusedRowHandle;
            object o = gridView1.GetRowCellValue(row, cIdx);
            if (o == null || o == DBNull.Value)
            {
                e.Cancel = true;
                logger.Debug("On edit attempt with null ref");
            }
            else
            {
                int val = (int)o;
                if (val < 0 || val >= tems.Count)
                {
                    e.Cancel = true;
                    logger.Debug("On edit attempt out of TEM bounds");
                }
                else
                {
                    PidHelper ph = tems[val];
                    if (ph.IsProtected)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        /// <summary>
        /// Validate cell input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onEditValidation(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.FocusedColumn.Name == cTEM.Name)
            {
                try
                {
                    string val = e.Value.ToString();
                    if (val.Length == 0 || val.Length > 4)
                    {
                        e.ErrorText = "Enter a name of length 1 to 4!";
                        e.Valid = false;
                    }
                    else
                    {
                        for (int i = 0; i < val.Length; i++)
                        {
                            char ch = val[i];
                            if (ch < 0x20 || ch >= 0x7f)
                            {
                                e.ErrorText = "No special characters!";
                                e.Valid = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug("Parse string exception: " + E);
                    e.ErrorText = "Parse exception! (" + E + ")";
                    e.Valid = false;
                }
            }
        }

        /// <summary>
        /// Update address, size and description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSymbolChange(object sender, CellValueChangedEventArgs e)
        {
            int rowIndex = e.RowHandle;
            if (rowIndex < 0) return;

            if (e.Column.Caption == "Symbol")
            {
                string desc;
                int address, size, type;
                GetSymbolData(gridView1.ActiveEditor.Text, out desc, out address, out size, out type);
                gridView1.SetRowCellValue(rowIndex, cSymbolAddress, address);
                gridView1.SetRowCellValue(rowIndex, cSymbolSize, size);
                gridView1.SetRowCellValue(rowIndex, cSymbolType, type);
                gridView1.SetRowCellValue(rowIndex, cDescription, desc);
                
            }
        }

        /// <summary>
        /// Colourise cells that are incorrect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            bool flagFault = false;
            bool flagWarn = false;
            bool isProtected = false;
            PidHelper ph = null;
            object op = gridView1.GetRowCellValue(e.RowHandle, cIdx);
            if (op == null || op == DBNull.Value)
            {
                logger.Debug("On edit attempt with null ref");
            }
            else
            {
                int val = (int)op;
                if (val < 0 || val >= tems.Count)
                {
                    logger.Debug("On edit attempt out of TEM bounds");
                }
                else
                {
                    ph = tems[val];
                    isProtected = ph.IsProtected;
                }
            }

            // The only protected symbol is the first one, "OFF", and it should/can not be edited in any way so it's just annoying to see a red blaring marker with no way to "fix" it
            if (isProtected == false)
            {
                // Mark TEM cell if it's incorrect
                if (e.Column.Name == cTEM.Name)
                {
                    object o = gridView1.GetRowCellValue(e.RowHandle, cTEM);
                    if (o == null || o == DBNull.Value)
                    {
                        flagFault = true;
                    }
                    else
                    {
                        string val = (string)o;
                        if (val.Length == 0 || val.Length > 4)
                        {
                            flagFault = true;
                        }
                    }
                }

                // Indicate that only parts of this symbol will be shown
                if (e.Column.Name == cSymbolSize.Name ||
                    e.Column.Name == cSymbolType.Name ||
                    e.Column.Name == cSymbol.Name)
                {
                    object os = gridView1.GetRowCellValue(e.RowHandle, cSymbolSize);
                    object ot = gridView1.GetRowCellValue(e.RowHandle, cSymbolType);
                    if (os == null || os == DBNull.Value ||
                        ot == null || ot == DBNull.Value)
                    {
                        flagFault = true;
                    }
                    else
                    {
                        int size = (int)os;
                        int type = (int)ot;
                        if (size < 1)
                        {
                            flagFault = true;
                        }
                        else
                        {
                            // Read type but remove some flags that are of no interest to this code
                            type = type & (~0x23);
                            if (((type & 0x04) > 0 && size != 1) || // byte and length is not 1
                                ((type & 0x48) > 0 && size != 4) || // long or other long and length is not 4
                                ((type & 0x10) > 0 && size > 2) || // bitfield and length is above 2
                                ((type == 0) && size != 2) || // word
                                ((type & 0x80) > 0)) // Unknown flag
                            {
                                flagWarn = true;
                            }
                        }
                    }
                }

                // Mark symbol name as invalid if the ECU index is believed to be out of bounds
                if (e.Column.Name == cSymbol.Name)
                {
                    object no = gridView1.GetRowCellValue(e.RowHandle, cSymbol);
                    if (no == null || no == DBNull.Value ||
                        (int)no < 1 || (int)no > 20000)
                    {
                        flagFault = true;
                    }
                }
            }

            if (flagFault)
            {
                e.Graphics.FillRectangle(Brushes.OrangeRed, e.Bounds);
            }
            else if (flagWarn)
            {
                e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);
            }
        }

        private static void GetSymbolData(int index, out string desc, out int address, out int size, out int type)
        {
            address = 0;
            size = 0;
            desc = "";
            type = 0;
            if (Form1.m_symbols == null)  return;
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if (sh.Symbol_number_ECU == index)
                {
                    desc = sh.Description;
                    address = sh.Internal_address;
                    size = sh.Length;
                    type = sh.Symbol_type;
                    return;
                }
            }
        }

        private static void GetSymbolData(string name, out string desc, out int address, out int size, out int type)
        {
            address = 0;
            size = 0;
            type = 0;
            desc = "";
            if (Form1.m_symbols == null)  return;
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if (sh.Varname == name)
                {
                    desc = sh.Description;
                    address = sh.Internal_address;
                    size = sh.Length;
                    type = sh.Symbol_type;
                    return;
                }
            }
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