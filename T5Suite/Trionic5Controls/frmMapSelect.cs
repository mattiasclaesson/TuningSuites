using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace Trionic5Controls
{
    public partial class frmMapSelect : DevExpress.XtraEditors.XtraForm
    {
        private string _selectedMap = "Close";

        public string SelectedMap
        {
            get { return _selectedMap; }
            set { _selectedMap = value; }
        }
        public frmMapSelect()
        {
            InitializeComponent();
            
        }

        private void listBoxControl1_Click(object sender, EventArgs e)
        {
            // check if an item is selected
            if (listBoxControl1.SelectedValue != null)
            {
                _selectedMap = listBoxControl1.SelectedValue.ToString();
                this.Close();
            }
        }

        public void ClearList()
        {
            listBoxControl1.Items.Clear();
        }

        public void SetSymbolCollection(SymbolCollection sc)
        {
            listBoxControl1.ValueMember = "Varname";
            listBoxControl1.DisplayMember = "Helptext";
            listBoxControl1.DataSource = sc;
        }

        private void frmMapSelect_Shown(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _selectedMap = "Close";
            this.Close();
        }

        private void listBoxControl1_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            timer1.Enabled = false;
            timer1.Enabled = true;
        }
    }
}