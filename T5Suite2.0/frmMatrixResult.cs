using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Controls;
using Nevron.Chart;
using Nevron.Chart.WinForm;
using Nevron.GraphicsCore;


//TODO: rewrite control so it can use the new "mapviewer" as well.
namespace T5Suite2
{
   

    public partial class frmMatrixResult : DevExpress.XtraEditors.XtraForm
    {
        private NChartControl nChartControl1 = null;
        public frmMatrixResult()
        {
            InitializeComponent();
            //surfaceGraphViewer1.onGraphChanged += new SurfaceGraphViewer.GraphChangedEvent(surfaceGraphViewer1_onGraphChanged);
        }

        private double m_MaxValue = 0;

        public double MaxValue
        {
            get { return m_MaxValue; }
            set { m_MaxValue = value; }
        }
        private double m_MinValue = 0;

        public double MinValue
        {
            get { return m_MinValue; }
            set { m_MinValue = value; }
        }

        private int m_textheight = 12;

        private double[] y_axisvalues;

        private double[] x_axisvalues;

        public double[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }

        public void SetYAxis(double[] y_axis)
        {
            y_axisvalues = y_axis;
        }

        public void SetXAxis(double[] x_axis)
        {
            x_axisvalues = x_axis;
        }

        private string X_axis_name = string.Empty;

        public void SetXAxisName(string name)
        {
            X_axis_name = name;
        }
        private string Y_axis_name = string.Empty;

        public void SetYAxisName(string name)
        {
            Y_axis_name = name;
        }
        private string Z_axis_name = string.Empty;

        public void SetZAxisName(string name)
        {
            Z_axis_name = name;
        }

        private int[] ConvertToIntArray(double[] dblarr)
        {
            int[] retval = new int[dblarr.Length];
            for (int i = 0; i < dblarr.Length; i++)
            {
                retval[i] = 0;
                try
                {
                    retval[i] = Convert.ToInt32(dblarr[i] * 100);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
            return retval;
        }

        private bool m_useNewMapViewer = false;

        public bool UseNewMapViewer
        {
            get { return m_useNewMapViewer; }
            set { m_useNewMapViewer = value; }
        }

        public void SetTable(DataTable dt)
        {
            gridControl1.DataSource = dt;
            // also set parameters for the SurfaceGraphViewer
            byte[] m_map_content = new byte[16 * 16 * 2];
            int idx = 0;
            for (int i = 15; i >= 0; i--)
            {
                foreach (object o in dt.Rows[i].ItemArray)
                {
                    double value = Convert.ToDouble(o);
                    if (m_realMinValue > value) m_realMinValue = value;
                    if (m_realMaxValue < value) m_realMaxValue = value;
                    // now convert to int * 100
                    Int32 ivalue = 0;
                    try
                    {
                        ivalue = Convert.ToInt32(value * 100);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to convert to integer value: " + value.ToString());
                    }
                    byte b1 = 0;
                    byte b2 = 0;
                    try
                    {
                        b1 = Convert.ToByte(ivalue / 256);
                        b2 = Convert.ToByte(ivalue - (int)b1 * 256);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to convert to byte value + " + ivalue.ToString());
                    }
                    
                    m_map_content[idx++] = b1;
                    m_map_content[idx++] = b2;
                }
            }
            if (!m_useNewMapViewer)
            {
                BuildNewSurfaceGraph(m_map_content);
            }
            else
            {
                BuildNewNChart(m_map_content);
            }
        }

        private double m_realMinValue = Double.MaxValue;
        private double m_realMaxValue = Double.MinValue;

        private void BuildNewNChart(byte[] m_map_content)
        {
            try
            {
                DataTable dt = (DataTable)gridControl1.DataSource;

                nChartControl1 = new NChartControl();
                nChartControl1.Settings.ShapeRenderingMode = ShapeRenderingMode.HighSpeed;
                nChartControl1.Controller.Tools.Add(new NSelectorTool());
                nChartControl1.Controller.Tools.Add(new NTrackballTool());
                nChartControl1.MouseWheel += new MouseEventHandler(nChartControl1_MouseWheel);
                nChartControl1.MouseDown += new MouseEventHandler(nChartControl1_MouseDown);
                nChartControl1.MouseUp += new MouseEventHandler(nChartControl1_MouseUp);

                nChartControl1.Dock = DockStyle.Fill;
                xtraTabPage2.Controls.Add(nChartControl1);
                //nChartControl1.Charts.Clear();
                
                NChart chart = nChartControl1.Charts[0];
                nChartControl1.Legends.Clear();
                chart.Enable3D = true;
                chart.Width = 60.0f;
                chart.Depth = 60.0f;
                chart.Height = 35.0f;
                chart.Projection.SetPredefinedProjection(PredefinedProjection.PerspectiveTilted);
                chart.LightModel.SetPredefinedLightModel(PredefinedLightModel.ShinyTopLeft);

                NStandardScaleConfigurator scaleConfiguratorX = (NStandardScaleConfigurator)chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator;
                scaleConfiguratorX.MaxTickCount = dt.Rows.Count;
                scaleConfiguratorX.MajorTickMode = MajorTickMode.AutoMaxCount;
                //scaleConfiguratorX.AutoLabels = true;
                NScaleTitleStyle titleStyleX = (NScaleTitleStyle)scaleConfiguratorX.Title;
                titleStyleX.Text = Y_axis_name;
                //<GS-08032010> as waarden nog omzetten indien noodzakelijk (MAP etc)
                scaleConfiguratorX.AutoLabels = false;
                scaleConfiguratorX.Labels.Clear();

                for (int t = y_axisvalues.Length - 1; t >= 0; t--)
                {
                    string yvalue = y_axisvalues.GetValue(t).ToString();
                    //if (Y_axis_name == "MAP" || Y_axis_name == "Pressure error (bar)")
                    {
                        try
                        {
                            float v = (float)Convert.ToDouble(yvalue);
                            yvalue = v.ToString("F2");
                        }
                        catch (Exception cE)
                        {
                            Console.WriteLine(cE.Message);
                        }
                    }
                    scaleConfiguratorX.Labels.Add(yvalue);

                }
                NStandardScaleConfigurator scaleConfiguratorY = (NStandardScaleConfigurator)chart.Axis(StandardAxis.Depth).ScaleConfigurator;
                scaleConfiguratorY.MajorTickMode = MajorTickMode.AutoMaxCount;
                scaleConfiguratorY.MaxTickCount = dt.Columns.Count;
                //scaleConfiguratorY.AutoLabels = true;
                NScaleTitleStyle titleStyleY = (NScaleTitleStyle)scaleConfiguratorY.Title;
                titleStyleY.Text = X_axis_name;
                scaleConfiguratorY.AutoLabels = false;
                scaleConfiguratorY.Labels.Clear();
                for (int t = 0; t < x_axisvalues.Length; t++)
                {
                    string xvalue = x_axisvalues.GetValue(t).ToString();
                    //if (X_axis_name == "MAP" || X_axis_name == "Pressure error (bar)")
                    {
                        try
                        {
                            float v = (float)Convert.ToDouble(xvalue);
                            xvalue = v.ToString("F2");
                        }
                        catch (Exception cE)
                        {
                            Console.WriteLine(cE.Message);
                        }
                    }
                    scaleConfiguratorY.Labels.Add(xvalue);
                }

                NStandardScaleConfigurator scaleConfiguratorZ = (NStandardScaleConfigurator)chart.Axis(StandardAxis.PrimaryY).ScaleConfigurator;
                scaleConfiguratorZ.MajorTickMode = MajorTickMode.AutoMaxCount;
                NScaleTitleStyle titleStyleZ = (NScaleTitleStyle)scaleConfiguratorZ.Title;
                titleStyleZ.Text = Z_axis_name;
                scaleConfiguratorZ.AutoLabels = true;
                chart.Wall(ChartWallType.Back).Visible = false;
                chart.Wall(ChartWallType.Left).Visible = false;
                chart.Wall(ChartWallType.Right).Visible = false;
                chart.Wall(ChartWallType.Floor).Visible = false;
                NMeshSurfaceSeries surface = null;
                chart.Series.Clear();
                if (chart.Series.Count == 0)
                {
                    surface = (NMeshSurfaceSeries)chart.Series.Add(SeriesType.MeshSurface);
                }
                else
                {
                    surface = (NMeshSurfaceSeries)chart.Series[0];
                }
                surface.Name = "Surface";
                surface.PositionValue = 10.0;

                surface.Palette.Clear();
                surface.Data.SetGridSize(dt.Columns.Count, dt.Rows.Count);
                surface.ValueFormatter.FormatSpecifier = "0.00";
                surface.FillMode = SurfaceFillMode.Zone; // <GS-08032010>
                surface.SmoothPalette = true;
                surface.FrameColorMode = SurfaceFrameColorMode.Uniform;
                surface.FillStyle.SetTransparencyPercent(25);
                surface.FrameMode = SurfaceFrameMode.MeshContour;

                double diff = m_realMaxValue - m_realMinValue;
                
                surface.Palette.Add(m_realMinValue, Color.Green);
                surface.Palette.Add(m_realMinValue + 0.25 * diff, Color.Yellow);
                surface.Palette.Add(m_realMinValue + 0.50 * diff, Color.Orange);
                surface.Palette.Add(m_realMinValue + 0.75 * diff, Color.OrangeRed);
                surface.Palette.Add(m_realMinValue + diff, Color.Red);

                surface.PaletteSteps = 4;
                surface.AutomaticPalette = false;

                FillData(surface, dt);
                // hier
                nChartControl1.Refresh();
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to refresh mesh chart: " + E.Message);
            }
        }

        void nChartControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                nChartControl1.Controller.Tools.Clear();
                NDragTool dragTool = null;
                dragTool = new NTrackballTool();
                nChartControl1.Controller.Tools.Add(dragTool);
            }
            //<GS-07062010>
            //CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);


        }

        void nChartControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                nChartControl1.Controller.Tools.Clear();
                NDragTool dragTool = null;
                dragTool = new NOffsetTool();
                nChartControl1.Controller.Tools.Add(dragTool);
                //dragTool.BeginDragMouseCommand.MouseButton = MouseButtons.Right;
                //dragTool.EndDragMouseCommand.MouseButton = MouseButtons.Right;
            }

        }

        void nChartControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            // wieltje wordt zoomin/out
            if (e.Delta > 0)
            {
                nChartControl1.Charts[0].Projection.Zoom += 5;
                nChartControl1.Refresh();
            }
            else
            {
                nChartControl1.Charts[0].Projection.Zoom -= 5;
                nChartControl1.Refresh();
            }
            //<GS-07062010>
            //CastSurfaceGraphChangedEventEx(nChartControl1.Charts[0].Projection.XDepth, nChartControl1.Charts[0].Projection.YDepth, nChartControl1.Charts[0].Projection.Zoom, nChartControl1.Charts[0].Projection.Rotation, nChartControl1.Charts[0].Projection.Elevation);
        }

        private void FillData(NMeshSurfaceSeries surface, DataTable dt)
        {
            int rowcount = 0;


            //surface.Data.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                for (int t = 0; t < dt.Columns.Count; t++)
                {
                    double value = Convert.ToInt32(dr[t]);
                    surface.Data.SetValue(rowcount, t, value, rowcount, t);
                }
                rowcount++;
            }
        }

        private void BuildNewSurfaceGraph(byte[] m_map_content)
        {
            SurfaceGraphViewer surfaceGraphViewer1 = new SurfaceGraphViewer();


            surfaceGraphViewer1.Dock = DockStyle.Fill;
            xtraTabPage2.Controls.Add(surfaceGraphViewer1);

            surfaceGraphViewer1.Pan_x = 45;
            surfaceGraphViewer1.Pan_y = 77;
            surfaceGraphViewer1.Pov_d = 0.6;
            surfaceGraphViewer1.Pov_x = 30;
            surfaceGraphViewer1.Pov_y = 56;
            surfaceGraphViewer1.Pov_z = 21;

            surfaceGraphViewer1.ShowinBlue = false;
            surfaceGraphViewer1.IsRedWhite = false;
            surfaceGraphViewer1.Map_name = "Matrix";
            surfaceGraphViewer1.X_axis = ConvertToIntArray(x_axisvalues);
            surfaceGraphViewer1.Y_axis = ConvertToIntArray(y_axisvalues);
            surfaceGraphViewer1.X_axis_descr = X_axis_name;
            surfaceGraphViewer1.Y_axis_descr = Y_axis_name;
            surfaceGraphViewer1.Z_axis_descr = Z_axis_name;
            surfaceGraphViewer1.Map_length = 16 * 16 * 2;
            surfaceGraphViewer1.Map_content = m_map_content;
            surfaceGraphViewer1.NumberOfColumns = 16;
            surfaceGraphViewer1.IsSixteenbit = true;
            surfaceGraphViewer1.Pov_d = 0.3;
            surfaceGraphViewer1.IsUpsideDown = true;
            /*if (false)
            {
                surfaceGraphViewer1.Pov_d = 0.25;
                surfaceGraphViewer1.Pan_y = 20;
                surfaceGraphViewer1.Pan_x = 20;
            }*/
            surfaceGraphViewer1.NormalizeData();
            surfaceGraphViewer1.RefreshView();

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.CellValue != null)
                {
                    float value = (float)Convert.ToDouble(e.CellValue);
                    if (value == 0)
                    {
                        e.DisplayText = "";
                    }
                    else
                    {
                        e.DisplayText = value.ToString("F2");
                        // color
                        Double b = value * 255;
                        if (m_MaxValue != 0)
                        {
                            b /= m_MaxValue;
                        }
                        int red = 128;
                        int green = 128;
                        int blue = 128;
                        red = Convert.ToInt32(b);
                        if (red < 0) red = 0;
                        if (red > 255) red = 255;
                        if (b > 255) b = 255;
                        blue = 0;
                        green = 255 - red;
                        Color c = Color.FromArgb(red, green, blue);
                        SolidBrush sb = new SolidBrush(c);
                        e.Graphics.FillRectangle(sb, e.Bounds);
                        sb.Dispose();
                    }
                }
            }
            catch (Exception E)
            {
                // nothing
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {

                //  e.Painter.DrawCaption(new DevExpress.Utils.Drawing.ObjectInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics)), "As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, null);
                // e.Cache.DrawString("As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, new StringFormat());
                try
                {
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            float val = (float) Convert.ToDouble(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                            string yvalue = val.ToString("F2");
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
                    Console.WriteLine(E.Message);
                }
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null)
                {
                    float xvalue = (float)Convert.ToDouble(e.Column.Caption);
                    Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                    e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(gb, e.Bounds);
                    //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                    e.Graphics.DrawString(xvalue.ToString("F2"), this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - m_textheight) / 2));
                    e.Handled = true;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void frmMatrixResult_Resize(object sender, EventArgs e)
        {
            Console.WriteLine(this.Height.ToString() + " " + this.Width.ToString());
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            foreach (Control c in xtraTabPage2.Controls)
            {
                if (c is SurfaceGraphViewer)
                {
                    SurfaceGraphViewer surfaceGraphViewer1 = (SurfaceGraphViewer)c;
                    surfaceGraphViewer1.RefreshView();
                }
                else if (c is NChartControl)
                {
                    //NChartControl nChartControl1 = (NChartControl)c;
                    nChartControl1.Refresh();

                }
            }
        }

        internal void SetViewType(int type)
        {
            
        }
    }
}