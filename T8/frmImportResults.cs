using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CommonSuite
{
    public partial class frmImportResults : DevExpress.XtraEditors.XtraForm
    {
        public frmImportResults()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SetDataTable(DataTable dt)
        {
            gridControl1.DataSource = dt;
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == gridColumn2.Name)
            {
                // draw
                Color c = Color.Red;
                if (e.DisplayText == "Success")
                {
                    c = Color.Green;
                } else if (e.DisplayText == "Skipped")
                {
                    c = Color.Blue;
                }
                System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, c, Color.White, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(gb, e.Bounds);

            }
        }

    }
}