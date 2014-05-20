using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Trionic5Controls
{
    public partial class CompareResultSelector : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void NotifySelectFile(object sender, SelectFileEventArgs e);
        public event CompareResultSelector.NotifySelectFile onFileSelect;


        public CompareResultSelector()
        {
            InitializeComponent();
        }

        public void SetData(DataTable dt)
        {
            gridControl1.DataSource = dt;
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int[] selectedrows = gridView1.GetSelectedRows();
            if (selectedrows.Length > 0)
            {
                string filename = (string)gridView1.GetRowCellValue(gridView1.FocusedRowHandle, gcFullFilename);
                // start a symbolcompare window for this symbol
                CastSelectEvent(filename);
            }
        }

        private void CastSelectEvent(string filename)
        {
            if (onFileSelect != null)
            {
                // haal eerst de data uit de tabel van de gridview
                onFileSelect(this, new SelectFileEventArgs(filename));
            }
        }
        public class SelectFileEventArgs : System.EventArgs
        {
            private string _filename;

            public string Filename
            {
                get
                {
                    return _filename;
                }
            }

            public SelectFileEventArgs(string filename)
            {
                this._filename = filename;
            }
        }
    }
}
