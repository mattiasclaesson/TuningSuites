using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using T7;
using System.IO;

namespace T7
{
    public partial class frmSIDInformation : DevExpress.XtraEditors.XtraForm
    {
        private int m_ApplicationLanguage = 44;

        public int ApplicationLanguage
        {
            get { return m_ApplicationLanguage; }
            set { m_ApplicationLanguage = value; }
        }

        private bool _ShowAddressesInHex = true;

        public bool ShowAddressesInHex
        {
            get { return _ShowAddressesInHex; }
            set
            {
                _ShowAddressesInHex = value;
                SetFilterMode();
            }
        }

        private void SetFilterMode()
        {
            if (_ShowAddressesInHex)
            {

                gcSIDAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSIDAddress.DisplayFormat.FormatString = "X6";
                gcSIDAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            else
            {
                gcSIDAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSIDAddress.DisplayFormat.FormatString = "";
                gcSIDAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
            }

        }

        public frmSIDInformation()
        {
            InitializeComponent();
            m_sidcollection = new SIDICollection();
        }

        private T7.SymbolCollection _symbols = new T7.SymbolCollection();

        public T7.SymbolCollection Symbols
        {
            get { return _symbols; }
            set
            {
                _symbols = value;
                SymbolCollection sdnew = new SymbolCollection();
                foreach (SymbolHelper sh in _symbols)
                {                                   
                    if (sh.Flash_start_address >= 0xF0000) //??
                    {
                        
                        sdnew.Add(sh);
                        if (sh.Varname.StartsWith("Symbolnumber") && sh.Userdescription != "")
                        {
                            sdnew[sdnew.Count - 1].Varname = sh.Userdescription;
                        }
                    }
                    
                }
                repositoryItemLookUpEdit2.DataSource = sdnew;
            }
        }

        private SIDICollection m_entiresidcollection;

        internal SIDICollection Entiresidcollection
        {
            get { return m_entiresidcollection; }
            set
            {
                m_entiresidcollection = value;
                repositoryItemLookUpEdit1.DataSource = m_entiresidcollection;
            }
        }

        private SIDICollection m_sidcollection;

        internal SIDICollection Sidcollection
        {
            get { return m_sidcollection; }
            set
            {
                m_sidcollection = value;

                SymbolTranslator strans = new SymbolTranslator();
                string ht = string.Empty;
                XDFCategories cat = XDFCategories.Undocumented;
                XDFSubCategory subcat = XDFSubCategory.Undocumented;
                // try to fill in the empty spaces first
                foreach (SIDIHelper sh in m_sidcollection)
                {
                   
                    if (sh.T7Symbol == string.Empty)
                    {
                        sh.T7Symbol = sh.FoundT7Symbol;
                        // and get the info
                        sh.Info = strans.TranslateSymbolToHelpText(sh.T7Symbol, out ht, out cat, out subcat, m_ApplicationLanguage);
                    }
                }

                gridControl1.DataSource = m_sidcollection;
                gridView1.ExpandAllGroups();
            }
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

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            bool m_color = false;
            if (e.Column.Name == gcSIDFoundSymbol.Name)
            {
                object o = gridView1.GetRowCellValue(e.RowHandle, gcSIDT7Symbol);
                if (o == null)
                {
                    // dan sowieso rood maken
                    m_color = true;
                }
                else if (o == DBNull.Value)
                {
                    m_color = true;
                }
                else
                {
                    string val = (string)o;
                    if (val != e.DisplayText)
                    {
                        m_color = true;
                    }
                }
                if (m_color)
                {
                    e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                }
            }
            else if (e.Column.Name == gcSIDMode.Name)
            {
                try
                {
                    if (e.CellValue != null)
                    {
                        int i = Convert.ToInt32(e.CellValue);
                        string name = i.ToString();
                        switch (i)
                        {
/*
0 = Adaptation
1 = Exhaust
2 = TCM FUEL/DTI
3 = TCM GSI/CSLU
4 = TCM TRQ/SPEED
5 = ESP
6 = Purge
 */
                            case 0:
                                name = "Adaptation";
                                break;
                            case 1:
                                name = "Exhaust";
                                break;
                            case 2:
                                name = "TCM FUEL/DTI";
                                break;
                            case 3:
                                name = "TCM GSI/CSLU";
                                break;
                            case 4:
                                name = "TCM TRQ/SPEED";
                                break;
                            case 5:
                                name = "ESP";
                                break;
                            case 6:
                                name = "Purge";
                                break;
                            case 99:
                                name = "Generic";
                                break;
                        }
                        e.DisplayText = name;
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }

        }

        private void repositoryItemLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //repositoryItemLookUpEdit1.editva
            
            /*if (repositoryItemLookUpEdit1 is LookUpEdit)
            {
                LookUpEdit lue = (LookUpEdit)repositoryItemLookUpEdit1;
                Console.WriteLine("Changed to: " + lue.EditValue.ToString());
            }*/
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            
        }

        private void gridView1_HiddenEditor(object sender, EventArgs e)
        {
            

        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            // nu dan de rest aanvullen
            // so, the user selected a different symbol from the list, adjust the address, ID (HOW?) T7Symbol name and FoundT7Symbol name.
            // ID = 00 if length = 2 bytes
            // ID = 04 if length = 1 bytes
            /*if (gridView1.FocusedRowHandle == 0)
            {
                e.Valid = false;
                return;
            }*/
            //Console.WriteLine(sender.ToString());
            if (gridView1.FocusedColumn.Name == gcSIDSymbol.Name)
            {
                /*
                Int32 NewSIDAddress = 0x00;
                string newSymbol = string.Empty;
                string newInfo = string.Empty;
                string newFoundT7Symbol = string.Empty;
                string newValue = "00";
                Int32 newSymbolLength = 0;
                // match the T7Symbol to it
                foreach (SIDIHelper sidh in m_entiresidcollection)
                {
                    if (sidh.Symbol == e.Value.ToString())
                    {
                        newSymbol = sidh.T7Symbol;
                    }
                }
                foreach (SymbolHelper sh in _symbols)
                {
                    if (sh.Varname == newSymbol)
                    {
                        newSymbolLength = sh.Length;
                        newFoundT7Symbol = sh.Varname;
                        newInfo = sh.Description;
                        NewSIDAddress = (int)sh.Flash_start_address;
                    }
                }
                
                if (newSymbolLength == 1) newValue = "04";
                else if (newSymbolLength == 2) newValue = "01";
                else if (newSymbolLength == 4) newValue = "00";

                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDAddress, NewSIDAddress);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDT7Symbol, newSymbol);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDAddress, NewSIDAddress);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDInfo, newInfo);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDFoundSymbol, newFoundT7Symbol);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDValue, newValue);*/

                // als lengte > 4 dan afkappen! 
                string val = e.Value.ToString();
                if (val.Length > 4)
                {
                    e.ErrorText = "Maximum symbolname length = 4!";
                    e.Valid = false;
                }
                else if (val.Length < 4)
                {
                    val.PadRight(4, ' ');
                }
            }
            else if (gridView1.FocusedColumn.Name == gcSIDModeDescr.Name)
            {
                // nothing yet
            }
            else if (gridView1.FocusedColumn.Name == gcSIDT7Symbol.Name)
            {
                // symbol changed, also change shortname into a new value
                // also change address
                Int32 NewSIDAddress = 0x00;
                string newSymbol = string.Empty;
                string newInfo = string.Empty;
                string newFoundT7Symbol = string.Empty;
                string newValue = "00";
                Int32 newSymbolLength = 0;
                // match the T7Symbol to it
                foreach (SIDIHelper sidh in m_entiresidcollection)
                {
                    if (sidh.T7Symbol == e.Value.ToString())
                    {
                        newSymbol = sidh.Symbol;
                    }
                }
                if (newSymbol == string.Empty)
                {
                    newSymbol = e.Value.ToString().Substring(0, 4);
                }

                foreach (SymbolHelper sh in _symbols)
                {
                    if (sh.Varname == e.Value.ToString())
                    {
                        newSymbolLength = sh.Length;
                        newFoundT7Symbol = sh.Varname;
                        newInfo = sh.Description;
                        NewSIDAddress = (int)sh.Flash_start_address;
                    }
                }
                /*
    Lengte 1 -> Value 04
    Lengte 2 -> Value 01
    Lengte 4 OF 2 -> Value 00
    BOOL len = 1 -> Value = 05
                 * */
                if (newSymbolLength == 1) newValue = "04";
                else if (newSymbolLength == 2) newValue = "01";
                else if (newSymbolLength == 4) newValue = "00";

                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDAddress, NewSIDAddress);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDSymbol, newSymbol);
                //gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDAddress, NewSIDAddress);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDInfo, newInfo);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDFoundSymbol, newFoundT7Symbol);
                gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gcSIDValue, newValue);
            }
            else if (gridView1.FocusedColumn.Name == gcSIDValue.Name)
            {
                // value changed (ID of symbol)... just change this one
            }
            
        }


        internal int Get99ModeCount()
        {
            int cnt = 0;
            foreach (SIDIHelper sh in m_sidcollection)
            {
                if (sh.Mode == 99) cnt++;
            }
            return cnt;
        }

        internal int GetNewModeCount()
        {
            int cnt = 0;
            foreach (SIDIHelper sh in m_sidcollection)
            {
                if (sh.Mode != 99) cnt++;
            }
            return cnt;
        }

        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            object o = gridView1.GetFocusedRow();
            if (o is SIDIHelper)
            {
                SIDIHelper sh = (SIDIHelper)o;
                if (sh.IsReadOnly)
                {
                    e.Cancel = true;
                }
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            
        }

        private void frmSIDInformation_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("IDValues");
            dt.Columns.Add("ID");
            dt.Columns.Add("DESCR");
            dt.Rows.Add("00", "Unsigned integer");
            dt.Rows.Add("01", "Signed integer");
            dt.Rows.Add("02", "02");
            dt.Rows.Add("03", "03");
            dt.Rows.Add("04", "Unsigned byte");
            dt.Rows.Add("05", "Signed byte");
            repositoryItemLookUpEdit3.DisplayMember = "DESCR";
            repositoryItemLookUpEdit3.ValueMember = "ID";
            repositoryItemLookUpEdit3.DataSource = dt;
            /*repositoryItemComboBox1.Items.Add("00");
            repositoryItemComboBox1.Items.Add("01");
            repositoryItemComboBox1.Items.Add("02");
            repositoryItemComboBox1.Items.Add("03");
            repositoryItemComboBox1.Items.Add("04");
            repositoryItemComboBox1.Items.Add("05");*/
        }

        private int GetSymbolAddressSRAM(string symbol)
        {
            int retval = 0;
            foreach (SymbolHelper sh in Symbols)
            {
                if (sh.Varname == symbol || sh.Userdescription == symbol)
                {
                    retval = (int)sh.Start_address;
                    break;
                }
            }
            return retval;
            
        }

        private void exportSIDiSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save symbol collection to .sid file
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "SIDi files|*.sid";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    
                    SIDICollection _coll = (SIDICollection)gridControl1.DataSource;
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                    {
                        foreach (SIDIHelper sidhelper in _coll)
                        {
                            int oriAddress = GetSymbolAddressSRAM(sidhelper.T7Symbol);
                            if (sidhelper.T7Symbol == string.Empty)
                            {
                                sidhelper.T7Symbol = GetDescriptionFromCollection(Entiresidcollection, sidhelper.Symbol);
                            }
                            sw.WriteLine(sidhelper.FoundT7Symbol + "|" + sidhelper.Info + "|" + sidhelper.Mode.ToString() + "|" + sidhelper.ModeDescr + "|" + sidhelper.Symbol + "|" + sidhelper.T7Symbol + "|" + sidhelper.Value + "|" + sidhelper.AddressSRAM.ToString() + "|" + oriAddress.ToString() + "|" + sidhelper.IsReadOnly.ToString());
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        private void importSIDiSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // import symbols from .sid file (must be same length)
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                SIDICollection _current = (SIDICollection)gridControl1.DataSource;
                SIDICollection _importedCollection = new SIDICollection();
                ofd.Filter = "SIDi files|*.sid";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string Line = string.Empty;
                    char[] sep = new char[1];
                    sep.SetValue('|', 0);
                    char[] sep2 = new char[1];
                    sep2.SetValue('+', 0);
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    if (lines.Length != _current.Count)
                    {
                        MessageBox.Show("Unable to import SIDi settings with a different length!");
                    }
                    else
                    {

                        using (StreamReader sr = new StreamReader(ofd.FileName))
                        {
                            // read all lines in the file
                            while ((Line = sr.ReadLine()) != null)
                            {
                                try
                                {
                                    string[] values = Line.Split(sep);
                                    SIDIHelper sh = new SIDIHelper();
                                    
                                    sh.FoundT7Symbol = (string)values.GetValue(0);
                                    sh.Info = (string)values.GetValue(1);
                                    sh.Mode = Convert.ToInt32((string)values.GetValue(2));
                                    sh.ModeDescr = (string)values.GetValue(3);
                                    sh.Symbol = (string)values.GetValue(4);
                                    sh.T7Symbol = (string)values.GetValue(5);
                                    if (sh.T7Symbol == sh.Symbol) sh.T7Symbol = sh.FoundT7Symbol;
                                    sh.Value = (string)values.GetValue(6);
                                    
                                    sh.AddressSRAM = Convert.ToInt32((string)values.GetValue(7));
                                    int oriAddress = Convert.ToInt32((string)values.GetValue(8));
                                    sh.IsReadOnly = Convert.ToBoolean((string)values.GetValue(9));
                                    int sramdiff = 0;
                                    if (oriAddress != 0)
                                    {
                                        sramdiff = sh.AddressSRAM - oriAddress;
                                    }
                                    else
                                    {
                                        //Console.WriteLine("Ori address = 0 " + sh.Symbol + " " + sh.T7Symbol);
                                    }
                                    if (sh.T7Symbol == string.Empty)
                                    {
                                        sh.T7Symbol = GetDescriptionFromCollection(Entiresidcollection, sh.Symbol);
                                    }
                                    // split by +
                                    string[] symbolvalues = sh.T7Symbol.Split(sep2);
                                    if (symbolvalues.Length > 1)
                                    {
                                        // dan corrigeren
                                        try
                                        {
                                            sramdiff += Convert.ToInt32(symbolvalues.GetValue(1));
                                            sh.AddressSRAM = GetSymbolAddressSRAM(symbolvalues.GetValue(0).ToString()) + sramdiff;
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }
                                    else
                                    {
                                        sh.AddressSRAM = GetSymbolAddressSRAM(sh.T7Symbol) + sramdiff;
                                    }
                                    
                                    _importedCollection.Add(sh);
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }

                            }
                        }
                        gridControl1.DataSource = _importedCollection;
                        m_sidcollection = (SIDICollection)gridControl1.DataSource;
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private string GetDescriptionFromCollection(SIDICollection _entireCollection, string symbol)
        {
            string retval = string.Empty;
            foreach (SIDIHelper sh in _entireCollection)
            {
                if (sh.Symbol == symbol)
                {
                    retval = sh.T7Symbol;
                    break;
                }
            }
            return retval;
        }

    }
}