using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace T8SuitePro
{
    public partial class frmTransferSelectionSymbolsSelection : DevExpress.XtraEditors.XtraForm
    {
        private SymbolCollection m_symbols;

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set
            {
                m_symbols = value;
                //TODO: select the values that are in the registry
                LoadAndSelectRegistryValues();
                checkedListBoxControl1.DisplayMember = "Varname";
                checkedListBoxControl1.ValueMember = "Selected";
                m_symbols.SortColumn = "Varname";
                m_symbols.SortingOrder = GenericComparer.SortOrder.Ascending;
                m_symbols.Sort();
                checkedListBoxControl1.DataSource = m_symbols;
                int idx = 0;
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Selected)
                    {
                        checkedListBoxControl1.SetItemChecked(idx, true);
                    }
                    idx++;
                }
            }
        }

        private void LoadAndSelectRegistryValues()
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");


            using (RegistryKey Settings = TempKey.CreateSubKey("T8SuitePro\\TransferSettings"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            SelectValueInList(a);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
        }

        private void SelectValueInList(string symbol)
        {
            //TODO: Implement
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbol)
                {
                    sh.Selected = true;
                    //Console.WriteLine("Symbol: " + symbol + " was selected");
                }
            }
        }

        private void ClearRegistrySettings()
        {
            //TODO: Implement
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            TempKey.DeleteSubKeyTree("T8SuitePro\\TransferSettings");
        }

        public frmTransferSelectionSymbolsSelection()
        {
            InitializeComponent();

        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            // select all
            int idx = 0;
            foreach (SymbolHelper sh in m_symbols)
            {
                sh.Selected = true;
                checkedListBoxControl1.SetItemChecked(idx, true);
                idx++;
            }
            checkedListBoxControl1.Refresh();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            // select none
            int idx = 0;
            foreach (SymbolHelper sh in m_symbols)
            {
                sh.Selected = false;
                checkedListBoxControl1.SetItemChecked(idx, false);
                idx++;
            }
            
            checkedListBoxControl1.Refresh();
        }

        private bool GetSelectedFromList(string varname)
        {
            foreach (object map in checkedListBoxControl1.CheckedItems)
            {
                if (map is SymbolHelper)
                {
                    SymbolHelper sh = (SymbolHelper)map;
                    /*if (sh.Selected)
                    {
                        Console.WriteLine("Checking: " + sh.Varname + " " + sh.Selected.ToString());
                    }*/
                    if (sh.Varname == varname)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            // selected to false
            //SymbolCollection new_symbols = (SymbolCollection)checkedListBoxControl1.DataSource;
            //m_symbols = new_symbols;
            ClearRegistrySettings();
            //TODO: clear registry of selected symbols
            foreach (SymbolHelper sh in m_symbols)
            {

                sh.Selected = GetSelectedFromList(sh.Varname);
                if (sh.Selected)
                {
                    //TODO: write to registry
                    SaveRegistrySetting(sh.Varname, true);
                }
            }

            this.Close();
        }

       

        private void SaveRegistrySetting(string key, bool value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T8SuitePro\\TransferSettings"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}