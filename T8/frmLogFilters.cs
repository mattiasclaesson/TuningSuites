using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NLog;

namespace CommonSuite
{
    public partial class frmLogFilters : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public frmLogFilters()
        {
            InitializeComponent();
            repositoryItemComboBox1.Items.Add(LogFilter.MathType.GreaterThan);
            repositoryItemComboBox1.Items.Add(LogFilter.MathType.SmallerThan);
            repositoryItemComboBox1.Items.Add(LogFilter.MathType.Equals);
        }

        public void SetSymbols(SymbolCollection sc)
        {
            repositoryItemLookUpEdit1.DataSource = sc;
        }

        public void SetFilters(LogFilterCollection filters)
        {
            if (filters == null) logger.Debug("setting filters as null");
            gridControl1.DataSource = filters;
            gridView1.BestFitColumns();
        }

        public LogFilterCollection GetFilters()
        {
            return (LogFilterCollection)gridControl1.DataSource;
        }


        private void simpleButton4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            LogFilterCollection filters = (LogFilterCollection)gridControl1.DataSource;
            if (filters == null) filters = new LogFilterCollection();
            LogFilter newfilter = new LogFilter();
            newfilter.Index = filters.Count;
            filters.Add(newfilter);
            gridControl1.DataSource = filters;
            gridView1.RefreshData();
            gridView1.BestFitColumns();
            //gridView1.AddNewRow();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle >= 0)
            {
                gridView1.DeleteRow(gridView1.FocusedRowHandle);
            }
        }
    }
}