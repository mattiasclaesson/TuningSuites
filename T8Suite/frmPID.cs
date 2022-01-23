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
    public partial class frmPID : DevExpress.XtraEditors.XtraForm
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        // Check this one some more. It must be disposed of
        private RepositoryItemLookUpEdit UsableSymbols = new RepositoryItemLookUpEdit();
        public PidCollection pids = null;

        public frmPID()
        {
            InitializeComponent();

            this.pids = Form1.m_pids.CopyOf();
            gridControl1.DataSource = this.pids;
            PopulateSymbols();

            this.cReadFlag.ColumnEdit = AddFlagTable();
            this.cWriteFlag.ColumnEdit = AddFlagTable();
            this.cSymbol.ColumnEdit = this.UsableSymbols;

            // Set cell formatting
            this.cSymbolAddress.DisplayFormat.FormatType = FormatType.Numeric;
            this.cSymbolAddress.DisplayFormat.FormatString = "X6";
            this.cSymbolSize.DisplayFormat.FormatType = FormatType.Numeric;
            this.cSymbolSize.DisplayFormat.FormatString = "0";
            this.cPID.DisplayFormat.FormatType = FormatType.Numeric;
            this.cPID.DisplayFormat.FormatString = "X4";

            // Register event handlers
            this.cSymbol.View.CellValueChanged += this.onSymbolChange;
            this.gridView1.ValidatingEditor += this.onEditValidation;
            this.gridView1.CustomDrawCell += this.onDrawCell;
            this.gridView1.ShowingEditor += this.onEditAttempt;
            // this.cSymbol.View.CellValueChanging += this.onSymbolChange;

            // Experiment: Let go of editor window if scrolling outside of it
            this.gridControl1.MouseWheel += onMouseScroll;

            // Change window title to reflect number of pids in table -Change tab title once tabs are working
            this.Text = "PID editor (" + this.pids.Count.ToString() + " items)";
        }
        
        // Copy pids to a local collection
        private void PopulateSymbols()
        {
            DataTable dt = new DataTable("IDValues");
            dt.Columns.Add("#");
            dt.Columns.Add("Symbol");

            Dictionary<int, string> dataItems = new Dictionary<int, string>();

            // Add USABLE symbols
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if (sh.Length > 0 && sh.Length < 8)
                {
                    dt.Rows.Add(sh.Symbol_number_ECU.ToString(), sh.Varname);
                }
            }

            // Add UNUSABLE symbols (if any) that are already present in the pid table
            foreach (PidHelper ph in this.pids)
            {
                SymbolHelper sh = Form1.m_symbols.SymbolWithIndex(ph.SymbolIndex);
                if (sh.Length == 0 || sh.Length >= 8)
                {
                    dt.Rows.Add(sh.Symbol_number_ECU.ToString(), sh.Varname);
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
            int address, size;
            foreach (PidHelper ph in this.pids)
            {
                GetSymbolData(ph.SymbolIndex, out desc, out address, out size);
                ph.SymbolDescription = desc;
                ph.SymbolAddress = address;
                ph.SymbolSize = size;
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
                if (val < 0 || val >= pids.Count)
                {
                    e.Cancel = true;
                    logger.Debug("On edit attempt out of PID bounds");
                }
                else
                {
                    PidHelper ph = pids[val];
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
            if (gridView1.FocusedColumn.Name == cPID.Name)
            {
                try
                {
                    string val = e.Value.ToString();
                    int value = Int32.Parse(val, System.Globalization.NumberStyles.HexNumber);
                    if (value < 0 || value > 65535)
                    {
                        e.ErrorText = "16-bit hex!";
                        e.Valid = false;
                    }
                    else
                    {
                        e.Value = value.ToString("X4");
                    }
                }
                catch (Exception E)
                {
                    logger.Debug("Parse string exception: " + E);
                    e.ErrorText = "16-bit hex! (" + E + ")";
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
                int address, size;
                GetSymbolData(gridView1.ActiveEditor.Text, out desc, out address, out size);
                gridView1.SetRowCellValue(rowIndex, cSymbolAddress, address);
                gridView1.SetRowCellValue(rowIndex, cSymbolSize, size);
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

            // Mark PID cell if it's incorrect
            if (e.Column.Name == cPID.Name)
            {
                object o = gridView1.GetRowCellValue(e.RowHandle, cPID);
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

            // Mark symbol name and size if size is out of bounds
            if (e.Column.Name == cSymbolSize.Name ||
                e.Column.Name == cSymbol.Name)
            {
                object o = gridView1.GetRowCellValue(e.RowHandle, cSymbolSize);
                if (o == null || o == DBNull.Value)
                {
                    flagFault = true;
                }
                else
                {
                    int val = (int)o;
                    if (val > 7 || val < 1)
                    {
                        flagFault = true;
                    }
                }
            }

            // Mark write flag and address if address is within flash and flag is not set to disabled
            if (e.Column.Name == cWriteFlag.Name ||
                e.Column.Name == cSymbolAddress.Name)
            {
                object ado = gridView1.GetRowCellValue(e.RowHandle, cSymbolAddress);
                object wfo = gridView1.GetRowCellValue(e.RowHandle, cWriteFlag);
                if (ado == null || ado == DBNull.Value ||
                    wfo == null || wfo == DBNull.Value)
                {
                    flagFault = true;
                }
                else
                {
                    int val = (int)ado;
                    if (val < 0x100000 && (int)wfo != 0)
                    {
                        flagFault = true;
                    }
                }
            }

            // Check if either flag is out of bounds or not set
            if (e.Column.Name == cReadFlag.Name)
            {
                object rfo = gridView1.GetRowCellValue(e.RowHandle, cReadFlag);
                if (rfo == null || rfo == DBNull.Value ||
                    (int)rfo < 0 || (int)rfo > 2)
                {
                    flagFault = true;
                }
            }
            else if (e.Column.Name == cWriteFlag.Name)
            {
                object wfo = gridView1.GetRowCellValue(e.RowHandle, cWriteFlag);
                if (wfo == null || wfo == DBNull.Value ||
                    (int)wfo < 0 || (int)wfo > 2)
                {
                    flagFault = true;
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

            if (flagFault)
            {
                e.Graphics.FillRectangle(Brushes.OrangeRed, e.Bounds);
            }
        }

        /// <summary>
        /// Add flag table to columns
        /// </summary>
        /// <returns></returns>
        private static RepositoryItemLookUpEdit AddFlagTable()
        {
            RepositoryItemLookUpEdit retval = new RepositoryItemLookUpEdit();
            DataTable dt = new DataTable("IDValues");
            dt.Columns.Add("Value");
            dt.Columns.Add("Flag");
            dt.Rows.Add("0", "Disabled");
            dt.Rows.Add("1", "Enabled");
            dt.Rows.Add("2", "Secured");
            dt.Rows.Add("3", "Unknown");
            retval.ValueMember = "Value";
            retval.DisplayMember = "Flag";
            retval.DataSource = dt;
            return retval;
        }

        private static void GetSymbolData(int index, out string desc, out int address, out int size)
        {
            address = 0;
            size = 0;
            desc = "";
            if (Form1.m_symbols == null)  return;
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if (sh.Symbol_number_ECU == index)
                {
                    desc = sh.Description;
                    address = sh.Internal_address;
                    size = sh.Length;
                    return;
                }
            }
        }

        private static void GetSymbolData(string name, out string desc, out int address, out int size)
        {
            address = 0;
            size = 0;
            desc = "";
            if (Form1.m_symbols == null)  return;
            foreach (SymbolHelper sh in Form1.m_symbols)
            {
                if (sh.Varname == name)
                {
                    desc = sh.Description;
                    address = sh.Internal_address;
                    size = sh.Length;
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