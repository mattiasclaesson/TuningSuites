using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;

namespace RealtimeGraph
{
    public partial class frmLineselection : Form
    {
        public frmLineselection()
        {
            InitializeComponent();
        }

        public void SetDataSource(GraphLineCollection lines)
        {
            //checkedListBoxControl1.DataSource = lines;
            foreach (GraphLine line in lines)
            {
                if (line.LineVisible)
                {
                    checkedListBoxControl1.Items.Add(line.Symbol, line.ChannelName, CheckState.Checked, true);
                }
                else
                {
                    checkedListBoxControl1.Items.Add(line.Symbol, line.ChannelName, CheckState.Unchecked, true);
                }
            }
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

     
    }
}