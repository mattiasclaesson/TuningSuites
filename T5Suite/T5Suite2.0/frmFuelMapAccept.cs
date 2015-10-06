using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;

namespace T5Suite2
{
    public partial class frmFuelMapAccept : DevExpress.XtraEditors.XtraForm
    {
        public delegate void UpdateFuelMap(object sender, UpdateFuelMapEventArgs e);
        public event frmFuelMapAccept.UpdateFuelMap onUpdateFuelMap;

        public delegate void SyncDates(object sender, EventArgs e);
        public event frmFuelMapAccept.SyncDates onSyncDates;

        public class UpdateFuelMapEventArgs : System.EventArgs
        {
            private int _x;

            public int X
            {
                get { return _x; }
                set { _x = value; }
            }
            private int _y;

            public int Y
            {
                get { return _y; }
                set { _y = value; }
            }
            private double _value;

            public double Value
            {
                get { return _value; }
                set { _value = value; }
            }

            private bool _dosync;

            public bool doSync
            {
                get { return _dosync; }
                set { _dosync = value; }
            }


            public UpdateFuelMapEventArgs(int x, int y, double value, bool dosync)
            {
                this._x = x;
                this._y = y;
                this._value = value;
                this._dosync = dosync;
            }
        }

        public frmFuelMapAccept()
        {
            InitializeComponent();
            /*if (Screen.PrimaryScreen.WorkingArea.Height <= 1024)
            {
                this.WindowState = FormWindowState.Maximized;
            }*/
            Application.DoEvents();
            ResizeGridControls();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.CellValue != null)
            {
                if (e.CellValue != DBNull.Value)
                {
                    float v = (float)Convert.ToDouble(e.CellValue);
                    
                    e.DisplayText = v.ToString("F2");
                    // color values
                    if (v != 0)
                    {
                        SolidBrush sb = new SolidBrush(Color.Orange);
                        e.Graphics.FillRectangle(sb, e.Bounds);
                        sb.Dispose();
                    }
                }
            }
        }

        

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            GridCell[] cells = gridView1.GetSelectedCells();
            bool doSync = false;
            foreach (GridCell cell in cells)
            {
                if (gridView1.GetRowCellValue(cell.RowHandle, cell.Column) != null)
                {
                    doSync = true;
                    CastUpdateFuelMapEvent(cell.Column.AbsoluteIndex, cell.RowHandle, Convert.ToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column)), false);
                }
            }
            if (cells.Length > 0)
            {
                if (doSync) CastSyncEvent();
            }
            this.Close();
            // and write to ECU
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CastSyncEvent()
        {
            if (onSyncDates != null)
            {
                onSyncDates(this, EventArgs.Empty);
            }
        }

        private void CastUpdateFuelMapEvent(int x, int y, double value, bool doSync)
        {
            
            //
            if (onUpdateFuelMap != null)
            {
                if (value != 0)
                {
                    //Console.WriteLine("Writing x: " + x.ToString() + " y: " + y.ToString() + " percentage: " + value.ToString());
                    onUpdateFuelMap(this, new UpdateFuelMapEventArgs(x, y, value, doSync));
                }
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            simpleButton3.Enabled = false;
            simpleButton3.Text = "Please wait";
            Application.DoEvents();
            gridView1.SelectAll();
            GridCell[] cells = gridView1.GetSelectedCells();
            bool doSync = false;
            foreach (GridCell cell in cells)
            {
                if (gridView1.GetRowCellValue(cell.RowHandle, cell.Column) != null)
                {
                    try
                    {
                        doSync = true;
                        double value = Convert.ToDouble(gridView1.GetRowCellValue(cell.RowHandle, cell.Column));
                       // if (value == 0) value = 5; // <GS-09082010> straks weghalen!!!
                        CastUpdateFuelMapEvent(cell.Column.AbsoluteIndex, cell.RowHandle, value , doSync);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
                
                
            }
            if (cells.Length > 0)
            {
                if (doSync) CastSyncEvent();
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        public void SetDataTable(DataTable dt)
        {
            gridControl1.DataSource = dt;
            ResizeGridControls();
        }

        private void gridControl1_Resize(object sender, EventArgs e)
        {
            ResizeGridControls();
        }

        
            // check font settings depeding on size of the control (font should increase when size increases for better readability)
        private void ResizeGridControls()
        {
            // check font settings depeding on size of the control (font should increase when size increases for better readability)
            gridView1.RowHeight = gridControl1.Height / 18;
            gridView1.Appearance.Row.Font = new Font("Tahoma", gridView1.RowHeight / 3, FontStyle.Bold);
        }

    }
}