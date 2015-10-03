using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using CommonSuite;
using NLog;

namespace T7
{
    public partial class frmFuelMapAccept : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void UpdateFuelMap(object sender, UpdateFuelMapEventArgs e);
        public event frmFuelMapAccept.UpdateFuelMap onUpdateFuelMap;

        public delegate void SyncDates(object sender, EventArgs e);
        public event frmFuelMapAccept.SyncDates onSyncDates;

        private int[] x_axisvalues;

        public int[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }
        private int[] y_axisvalues;

        public int[] Y_axisvalues
        {
            get { return y_axisvalues; }
            set { y_axisvalues = value; }
        }

        public bool AutoSizeColumns
        {
            set
            {
                gridView1.OptionsView.ColumnAutoWidth = value;
            }
        }

        private int m_textheight = 12;

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
                    //logger.Debug("Writing x: " + x.ToString() + " y: " + y.ToString() + " percentage: " + value.ToString());
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
                        logger.Debug(E.Message);
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

            if (!gridView1.OptionsView.ColumnAutoWidth)
            {
                for (int c = 0; c < gridView1.Columns.Count; c++)
                {
                    gridView1.Columns[c].Width = 40;
                }
            }

            // set y axis size
            int indicatorwidth = -1;
            for (int i = 0; i < y_axisvalues.Length; i++)
            {
                string yval = Convert.ToInt32(y_axisvalues.GetValue(i)).ToString();
                Graphics g = gridControl1.CreateGraphics();
                SizeF size = g.MeasureString(yval, this.Font);
                if (size.Width > indicatorwidth) indicatorwidth = (int)size.Width;
                m_textheight = (int)size.Height;
                g.Dispose();

            }
            if (indicatorwidth > 0)
            {
                gridView1.IndicatorWidth = indicatorwidth + 6; // keep margin
            }

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

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                try
                {
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            string yvalue = y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle).ToString();
                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (x_axisvalues.Length > 0)
                {
                    if (e.Column != null)
                    {
                        if (x_axisvalues.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = x_axisvalues.GetValue(e.Column.VisibleIndex).ToString();
                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }
    }
}