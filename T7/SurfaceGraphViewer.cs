using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Plot3D;
using System.Runtime.InteropServices;
using CommonSuite;
using NLog;

namespace T7
{
    public partial class SurfaceGraphViewer : DevExpress.XtraEditors.XtraUserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        [DllImport("user32.dll", EntryPoint = "CreateIconIndirect")]
        private static extern IntPtr CreateIconIndirect(IntPtr iconInfo);

        private struct IconInfo
        {
            public bool fIcon;
            public Int32 xHotspot;
            public Int32 yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        Surface3DRenderer sr;
        private bool m_IsDragging = false;
        private Point m_p = new Point();
/*        private int pov_x = -20;
        private int pov_y = 35;
        private int pov_z = 40;
        private int pan_x = 0;
        private int pan_y = 0;*/
        PointF m_lastMouseHoverPoint = new PointF();
        private int pov_x = 30;

        public int Pov_x
        {
            get { return pov_x; }
            set { pov_x = value; }
        }
        private int pov_y = 56;

        public int Pov_y
        {
            get { return pov_y; }
            set { pov_y = value; }
        }
        private int pov_z = 21;

        public int Pov_z
        {
            get { return pov_z; }
            set { pov_z = value; }
        }
        private int pan_x = 45;

        public int Pan_x
        {
            get { return pan_x; }
            set { pan_x = value; }
        }
        private int pan_y = 77;

        public int Pan_y
        {
            get { return pan_y; }
            set { pan_y = value; }
        }
        private double pov_d = 0.6;
        private bool m_isRedWhite = false;

        public bool IsRedWhite
        {
            get { return m_isRedWhite; }
            set
            {
                m_isRedWhite = value;
                sr.IsRedWhite = m_isRedWhite;
            }
        }

        public double Pov_d
        {
            get { return pov_d; }
            set { pov_d = value; }
        }

        private int m_map_length = 0;

        public int Map_length
        {
            get { return m_map_length; }
            set { m_map_length = value; }
        }


        private string m_map_name = string.Empty;

        public string Map_name
        {
            get { return m_map_name; }
            set
            {
                m_map_name = value;
            }
        }

        private bool m_ShowinBlue = false;

        public bool ShowinBlue
        {
            get { return m_ShowinBlue; }
            set
            {
                m_ShowinBlue = value;
                sr.ShowinBlue = value;
            }
        }

        private int m_numberOfColumns = 8;

        public int NumberOfColumns
        {
            get { return m_numberOfColumns; }
            set
            {
                m_numberOfColumns = value;
                if (m_numberOfColumns == 1)
                {
                    pan_x = 0;
                    pan_y = 10;
                    pov_x = 70;
                    pov_y = 0;
                    pov_z = 0;
                    pov_d = 0.2;
                }
            }
        }

        private bool m_isSixteenbit = false;

        public bool IsSixteenbit
        {
            get { return m_isSixteenbit; }
            set { m_isSixteenbit = value; }
        }

        private int[] x_axis;

        public int[] X_axis
        {
            get { return x_axis; }
            set { x_axis = value; }
        }

        private int[] y_axis;

        public int[] Y_axis
        {
            get { return y_axis; }
            set { y_axis = value; }
        }

        private int[] z_axis;

        public int[] Z_axis
        {
            get { return z_axis; }
            set { z_axis = value; }
        }

        private string x_axis_descr = string.Empty;

        public string X_axis_descr
        {
            get { return x_axis_descr; }
            set { x_axis_descr = value; }
        }
        private string y_axis_descr = string.Empty;

        public string Y_axis_descr
        {
            get { return y_axis_descr; }
            set { y_axis_descr = value; }
        }
        private string z_axis_descr = string.Empty;

        public string Z_axis_descr
        {
            get { return z_axis_descr; }
            set { z_axis_descr = value; }
        }

        private byte[] m_map_content;

        public byte[] Map_content
        {
            get { return m_map_content; }
            set
            {
                m_map_content = value;
                
            }
        }

        private byte[] m_map_original_content;

        public byte[] Map_original_content
        {
            get { return m_map_original_content; }
            set
            {
                m_map_original_content = value;

            }
        }


        private byte[] m_map_compare_content;

        public byte[] Map_compare_content
        {
            get { return m_map_compare_content; }
            set
            {
                m_map_compare_content = value;

            }
        }

        private bool m_isUpsideDown = false;

        public bool IsUpsideDown
        {
            get { return m_isUpsideDown; }
            set { m_isUpsideDown = value; }
        }

        public void NormalizeData()
        {
            // omzetten naar datatable
            // wat is de maximale waarde in de tabel?

            int byteoffset = 0;
            int maxvalue = 0;
            if (m_isSixteenbit)
            {
                for (int tt = 0; tt < m_map_content.Length; tt += 2)
                {
                    int valtot = 0;
                    valtot = Convert.ToInt32((byte)m_map_content.GetValue(byteoffset++)) * 256;
                    valtot += Convert.ToInt32((byte)m_map_content.GetValue(byteoffset++));
                    //logger.Debug("Value: :"+ valtot.ToString("X4"));
                    if (valtot > 0xF000)
                    {
                        valtot ^= 0xFFFF;
                        valtot = -valtot;
                    }
                    // TEST!!!
                    /*else if ((valtot & 0x8000) > 0)
                    {
                        //valtot = 0x10000 - valtot;
                        valtot &= 0x7FFF;
                        valtot = -valtot;
                    }*/
                    /*
                    if ( (valtot & 0x8000)>0)
                    {
                        //valtot = 0x10000 - valtot;
                        valtot &= 0x7FFF;
                        valtot = -valtot;
                    }*/
                    if (valtot > maxvalue) maxvalue = valtot;
                }
            }
            else
            {
                for (int tt = 0; tt < m_map_content.Length; tt++)
                {
                    int valtot = Convert.ToInt32((byte)m_map_content.GetValue(byteoffset++));
                    if (valtot > maxvalue) maxvalue = valtot;
                }
            }
            byteoffset = 0;
            if (m_map_compare_content != null)
            {
                if (m_isSixteenbit)
                {

                    for (int tt = 0; tt < m_map_compare_content.Length; tt += 2)
                    {
                        int valtot = 0;
                        valtot = Convert.ToInt32((byte)m_map_compare_content.GetValue(byteoffset++)) * 256;
                        valtot += Convert.ToInt32((byte)m_map_compare_content.GetValue(byteoffset++));
                        if (valtot > 0xF000)
                        {
                            valtot = 0x10000 - valtot;
                            valtot = -valtot;
                        }
                        if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                        {
                            if (valtot > 200)
                            {
                                valtot = 256 - valtot;
                                valtot = -valtot;
                            }
                        }
                        if (valtot > maxvalue) maxvalue = valtot;
                    }
                }
                else
                {
                    for (int tt = 0; tt < m_map_compare_content.Length; tt++)
                    {
                        int valtot = Convert.ToInt32((byte)m_map_compare_content.GetValue(byteoffset++));
                        if (valtot > maxvalue) maxvalue = valtot;
                    }
                }
            }
            if (m_map_original_content != null)
            {
                byteoffset = 0;
                if (m_isSixteenbit)
                {

                    for (int tt = 0; tt < m_map_original_content.Length; tt += 2)
                    {
                        int valtot = 0;
                        valtot = Convert.ToInt32((byte)m_map_original_content.GetValue(byteoffset++)) * 256;
                        valtot += Convert.ToInt32((byte)m_map_original_content.GetValue(byteoffset++));
                        if (valtot > 0xF000)
                        {
                            valtot = 0x10000 - valtot;
                            valtot = -valtot;
                        }
                        if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                        {
                            if (valtot > 200)
                            {
                                valtot = 256 - valtot;
                                valtot = -valtot;
                            }
                        }
                        if (valtot > maxvalue) maxvalue = valtot;
                    }
                }
                else
                {
                    for (int tt = 0; tt < m_map_original_content.Length; tt++)
                    {
                        int valtot = Convert.ToInt32((byte)m_map_original_content.GetValue(byteoffset++));
                        if (valtot > maxvalue) maxvalue = valtot;
                    }
                }
            }
            byteoffset = 0;
            if (maxvalue == 0) maxvalue = 256;
            float percentagetoscale = (float)((double)m_numberOfColumns / (double)maxvalue);
            
            sr.Correction_percentage = percentagetoscale;

            m_mapdata = new DataTable();
            m_mapcomparedata = new DataTable();
            m_maporiginaldata = new DataTable();

            for (int i = 0; i < m_numberOfColumns; i++)
            {
                m_mapdata.Columns.Add(i.ToString(), Type.GetType("System.Double"));
                m_mapcomparedata.Columns.Add(i.ToString(), Type.GetType("System.Double"));
                m_maporiginaldata.Columns.Add(i.ToString(), Type.GetType("System.Double"));
            }
            int numberofrows = m_map_length / m_numberOfColumns;
            if (m_isSixteenbit) numberofrows /= 2;
            byteoffset = 0;
            for (int j = 0; j < numberofrows; j++)
            {
                object[] arr = new object[m_numberOfColumns];
                for (int cc = 0; cc < m_numberOfColumns; cc++)
                {
                    if (m_isSixteenbit)
                    {
                        double valtot = 0;
                        valtot = Convert.ToInt32((byte)m_map_content.GetValue(byteoffset++)) * 256;
                        valtot += Convert.ToInt32((byte)m_map_content.GetValue(byteoffset++));
                        if ((int)valtot > 0xF000)
                        {
                            int ival = (int)valtot;
                            ival ^= 0xFFFF;
                            valtot = -ival;
                        }
                        // TEST !!!
                        /*else if (((int)valtot & 0x8000) > 0)
                        {
                            //valtot = 0x10000 - valtot;
                            int ival = (int) valtot;
                            ival &= 0x7FFF;
                            valtot = -ival;
                        }*/

                        valtot *= percentagetoscale;
                        arr.SetValue((double)valtot, cc);
                    }
                    else
                    {
                        try
                        {
                            double valtot = 0;
                            valtot = Convert.ToDouble((byte)m_map_content.GetValue(byteoffset++));
                            valtot *= percentagetoscale;
                            arr.SetValue(valtot, cc);
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                    }
                }
                if (!m_isUpsideDown)
                {
                    m_mapdata.Rows.Add(arr);
                }
                else
                {
                    System.Data.DataRow r = m_mapdata.NewRow();
                    r.ItemArray = arr;
                    m_mapdata.Rows.InsertAt(r, 0);
                }

                // moet insert op pos 0 zijn om te flippen!
/*                if (m_isUpsideDown)
                {
                    System.Data.DataRow r = dt.NewRow();
                    r.ItemArray = objarr;
                    dt.Rows.InsertAt(r, 0);
                }
                else
                {
                    dt.Rows.Add(objarr);
                }*/

            }

            // for original data
            if (m_map_original_content != null)
            {
                byteoffset = 0;
                for (int j = 0; j < numberofrows; j++)
                {
                    object[] arr = new object[m_numberOfColumns];
                    for (int cc = 0; cc < m_numberOfColumns; cc++)
                    {
                        if (m_isSixteenbit)
                        {
                            double valtot = 0;
                            valtot = Convert.ToInt32((byte)m_map_original_content.GetValue(byteoffset++)) * 256;
                            valtot += Convert.ToInt32((byte)m_map_original_content.GetValue(byteoffset++));
                            if (valtot > 0xF000)
                            {
                                valtot = 0x10000 - valtot;
                                valtot = -valtot;
                            }
                            if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                            {
                                if (valtot > 200)
                                {
                                    valtot = 256 - valtot;
                                    valtot = -valtot;
                                }
                            }
                            valtot *= percentagetoscale;
                            arr.SetValue((double)valtot, cc);
                        }
                        else
                        {
                            try
                            {
                                double valtot = 0;
                                valtot = Convert.ToDouble((byte)m_map_original_content.GetValue(byteoffset++));
                                valtot *= percentagetoscale;
                                arr.SetValue(valtot, cc);
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                    if (!m_isUpsideDown)
                    {
                        m_maporiginaldata.Rows.Add(arr);
                    }
                    else
                    {
                        System.Data.DataRow r = m_maporiginaldata.NewRow();
                        r.ItemArray = arr;
                        m_maporiginaldata.Rows.InsertAt(r, 0);
                    }
                }
            }

            // for compare data
            if (m_map_compare_content != null)
            {
                byteoffset = 0;
                for (int j = 0; j < numberofrows; j++)
                {
                    object[] arr = new object[m_numberOfColumns];
                    for (int cc = 0; cc < m_numberOfColumns; cc++)
                    {
                        if (m_isSixteenbit)
                        {
                            double valtot = 0;
                            valtot = Convert.ToInt32((byte)m_map_compare_content.GetValue(byteoffset++)) * 256;
                            valtot += Convert.ToInt32((byte)m_map_compare_content.GetValue(byteoffset++));
                            if (valtot > 0xF000)
                            {
                                valtot = 0x10000 - valtot;
                                valtot = -valtot;
                            }
                            if (m_map_name == "TargetAFR" || m_map_name == "FeedbackAFR" || m_map_name == "FeedbackvsTargetAFR")
                            {
                                if (valtot > 200)
                                {
                                    valtot = 256 - valtot;
                                    valtot = -valtot;
                                }
                            }
                            valtot *= percentagetoscale;
                            arr.SetValue((double)valtot, cc);
                        }
                        else
                        {
                            try
                            {
                                double valtot = 0;
                                valtot = Convert.ToDouble((byte)m_map_compare_content.GetValue(byteoffset++));
                                valtot *= percentagetoscale;
                                arr.SetValue(valtot, cc);
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                    if (!m_isUpsideDown)
                    {
                        m_mapcomparedata.Rows.Add(arr);
                    }
                    else
                    {
                        System.Data.DataRow r = m_mapcomparedata.NewRow();
                        r.ItemArray = arr;
                        m_mapcomparedata.Rows.InsertAt(r, 0);
                    }
                }
            }

            sr.Mapdata = m_mapdata;
            sr.Mapcomparedata = m_mapcomparedata;
            sr.Maporiginaldata = m_maporiginaldata;

            sr.StartPoint = new PointF(0, 0);
            sr.EndPoint = new PointF(m_mapdata.Columns.Count - 1, m_mapdata.Rows.Count - 1);
            sr.X_axisvalues = x_axis;
            sr.Y_axisvalues = y_axis;
            sr.Xaxis_descr = x_axis_descr;
            sr.Yaxis_descr = Y_axis_descr;
            sr.Zaxis_descr = z_axis_descr;
            //frmTableDebugger tabdebugger = new frmTableDebugger();
            //tabdebugger.ShowTable(m_mapdata);
            //tabdebugger.Show();

            Invalidate();
        }

        private DataTable m_mapdata = new DataTable();
        private DataTable m_mapcomparedata = new DataTable();
        private DataTable m_maporiginaldata = new DataTable();

        public DataTable Mapdata
        {
            get { return m_mapdata; }
            set
            {
                m_mapdata = value;
                sr.Mapdata = m_mapdata;
                sr.StartPoint = new PointF(0, 0);
                sr.EndPoint = new PointF(m_mapdata.Columns.Count - 1, m_mapdata.Rows.Count - 1);
                Invalidate();
            }
        }

        public delegate void GraphChangedEvent(object sender, GraphChangedEventArgs e);
        public event SurfaceGraphViewer.GraphChangedEvent onGraphChanged;


        private void CastGraphChangedEvent()
        {
            //pov_x, pov_y, pov_z, pan_x, pan_y,pov_d
            if (onGraphChanged != null)
            {
                if (this.Visible)
                {
                    onGraphChanged(this, new GraphChangedEventArgs(pov_x, pov_y, pov_z, pan_x, pan_y, pov_d));
                }
            }
        }

        public class GraphChangedEventArgs : System.EventArgs
        {
            //pov_x, pov_y, pov_z, pan_x, pan_y,pov_d
            private int _pov_x;

            public int Pov_x
            {
                get { return _pov_x; }
                set { _pov_x = value; }
            }
            private int _pov_y;

            public int Pov_y
            {
                get { return _pov_y; }
                set { _pov_y = value; }
            }
            private int _pov_z;

            public int Pov_z
            {
                get { return _pov_z; }
                set { _pov_z = value; }
            }
            private int _pan_x;

            public int Pan_x
            {
                get { return _pan_x; }
                set { _pan_x = value; }
            }
            private int _pan_y;

            public int Pan_y
            {
                get { return _pan_y; }
                set { _pan_y = value; }
            }
            private double _pov_d;

            public double Pov_d
            {
                get { return _pov_d; }
                set { _pov_d = value; }
            }

            public GraphChangedEventArgs(int povx, int povy, int povz, int panx, int pany, double povd)
            {
                this._pan_x = panx;
                this._pan_y = pany;
                this._pov_d = povd;
                this._pov_x = povx;
                this._pov_y = povy;
                this._pov_z = povz;
            }
        }


        public void SetView(int povx, int povy, int povz, int panx, int pany, double povd)
        {
            this.pov_x = povx;
            this.pov_y = povy;
            this.pov_z = povz;
            this.pan_x = panx;
            this.pan_y = pany;
            this.pov_d = povd;
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            Invalidate();
        }

        private void SurfaceGraphMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int delta = e.Delta;
            //if((uint)e.Delta > 0xff000000) delta = (int)(0x100000000- (long)delta);
            logger.Debug("e.Delta: " + e.Delta.ToString());
            logger.Debug("Delta: " + delta.ToString());
            pov_d += (delta * 0.0001);
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
        }

        public SurfaceGraphViewer()
        {
            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.SurfaceGraphMouseWheel);

            sr = new Surface3DRenderer(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            sr.ColorSchema = new ColorSchema(0);
            sr.Density = 1; 
            ResizeRedraw = true;
            DoubleBuffered = true;
        }

        private void frm3DSurfaceGraphViewer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            if (sr != null)
            {
                if (m_numberOfColumns == 1)
                {
                    sr.RenderGraph(e.Graphics);
                }
                else
                {
                    sr.RenderSurface(e.Graphics, m_lastMouseHoverPoint);
                }
            }
        }

        private void frm3DSurfaceGraphViewer_Resize(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (sr != null)
                {
                    sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                    CastGraphChangedEvent();
                }
            }
        }

        private void frm3DSurfaceGraphViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // rotate
                this.Cursor = GenerateCursor("Turn");
            }
            else if (e.Button == MouseButtons.Right)
            {
                // pan
                this.Cursor = GenerateCursor("Pan");
            }
            m_IsDragging = true;
            m_p = e.Location;
            //Invalidate();
        }

        private void frm3DSurfaceGraphViewer_MouseUp(object sender, MouseEventArgs e)
        {
            m_IsDragging = false;
            this.Cursor = Cursors.Default;
        }

        private void frm3DSurfaceGraphViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsDragging)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int deltay = m_p.X - e.Location.X;
                    int deltax = m_p.Y - e.Location.Y;
                    //PointF f = sr.Project(0, 0, 0);
                    
                    pov_y += deltay / 2;
                    pov_x -= deltay / 2;
                    //pov_z -= deltay / 2;
                   // pov_y += deltax / 2;
                    //int deltaz = m_p.Y - e.Location.Y;
                    pov_z -= deltax / 2;
                }
               /* else if (e.Button == MouseButtons.Right)
                {
                    int deltaz = m_p.Y - e.Location.Y;
                    pov_z -= deltaz / 2;
                }*/
                else if (e.Button == MouseButtons.Right)
                {
                    int deltax = m_p.X - e.Location.X;
                    int deltay = m_p.Y - e.Location.Y;
                    pan_x -= deltax / 2;
                    pan_y -= deltay / 2;
                }

                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
                m_p = e.Location;
            }
            else
            {
                // waarden van punten weergeven als in de buurt
                /*PointF tableposition = new Point(0,0);
                double val = 0;
                if (sr.GetMousePoint(e.X, e.Y, out tableposition, out val))
                {
                    //logger.Debug("Position = " + e.X.ToString() + ":" + e.Y.ToString() + " tablepos = " + tableposition.X.ToString() + ":" + tableposition.Y.ToString() + " value = " + val.ToString());
                    m_lastMouseHoverPoint = new PointF((float)e.X, (float)e.Y);
                    toolTipController1.ShowHint("Mouse hit : " + m_lastMouseHoverPoint.X.ToString() + ":" + m_lastMouseHoverPoint.Y.ToString(), PointToClient(e.Location));
                }*/
            }
        }

        private void SurfaceGraphViewer_DoubleClick(object sender, EventArgs e)
        {
            
            int hlpr = pov_x;
            pov_x = pov_y;
            pov_y =-hlpr;
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, this.ClientRectangle.Width, this.ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
            //frmTableDebugger tabdebugger = new frmTableDebugger();
            //tabdebugger.ShowTable(m_mapdata);
            //tabdebugger.Show();
        }

        private void SurfaceGraphViewer_Load(object sender, EventArgs e)
        {

        }

        private void spinEdit1_ValueChanged(object sender, EventArgs e)
        {
           /* pov_x = Convert.ToInt32( spinEdit1.EditValue);
            pov_y = Convert.ToInt32(spinEdit2.EditValue);
            pov_z = Convert.ToInt32(spinEdit3.EditValue);
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, this.ClientRectangle.Width, this.ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();*/
        }

        private void spinEdit4_ValueChanged(object sender, EventArgs e)
        {
         /*   pov_d = Convert.ToDouble(spinEdit4.EditValue);
          sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, this.ClientRectangle.Width, this.ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();*/
        }

        private void spinEdit5_EditValueChanged(object sender, EventArgs e)
        {
           /* sr.ColorSchema = new ColorSchema(Convert.ToDouble(spinEdit5.EditValue));
            Invalidate();*/
        }

        private void SurfaceGraphViewer_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private Cursor GenerateCursor(string text)
        {
            Bitmap imgColor = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Color color = Color.MidnightBlue;
            SolidBrush brush = new SolidBrush(color);
            SolidBrush brushMask = new SolidBrush(Color.White);
            string s = text;
            System.Drawing.Font fnt = new Font(FontFamily.GenericSerif, 10, FontStyle.Bold);
            using (Graphics g = Graphics.FromImage(imgColor))
            {
                SizeF size = g.MeasureString(s, fnt);
                if (size.Width > 32) size.Width = 32;
                if (size.Height > 32) size.Height = 32;
                g.FillRectangle(brushMask, 0, 0, 32, 32);
                g.DrawString(s, fnt, brush, (32 - size.Width) / 2, (32 - size.Height) / 2);
                g.Flush();
                //g.DrawRectangle(Pens.OrangeRed, new Rectangle(1, 1, 30, 30));
            }
            imgColor.MakeTransparent(Color.White);
            IconInfo ii = new IconInfo();
            ii.fIcon = false;
            ii.xHotspot = 16;
            ii.yHotspot = 16;
            ii.hbmMask = imgColor.GetHbitmap();
            ii.hbmColor = imgColor.GetHbitmap();
            unsafe
            {
                
                IntPtr iiPtr = new IntPtr(&ii);
                IntPtr curPtr = CreateIconIndirect(iiPtr);
                return new Cursor(curPtr);
            }
        }

        private void SurfaceGraphViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Add)
            {
                pov_d += 0.01;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();

            }
            else if (e.KeyCode == Keys.Subtract)
            {
                pov_d -= 0.01;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if ((int)e.KeyCode == 104)
            {
                pan_y -= 10;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if ((int)e.KeyCode == 98)
            {
                pan_y += 10;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if ((int)e.KeyCode == 102)
            {
                pan_x += 10;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if ((int)e.KeyCode == 100)
            {
                pan_x -= 10;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if (e.KeyCode == Keys.Left)
            {
                pov_x -= 1;
                pov_y += 1;

                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if (e.KeyCode == Keys.Right)
            {
                pov_x += 1;
                pov_y -= 1;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if (e.KeyCode == Keys.Up)
            {
                pov_z -= 1;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
            else if (e.KeyCode == Keys.Down)
            {
                pov_z += 1;
                sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
                CastGraphChangedEvent();
                Invalidate();
            }
        }

        private void SurfaceGraphViewer_KeyPress(object sender, KeyPressEventArgs e)
        {
            

        }

        public void ZoomIn()
        {
            pov_d += 0.01;
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
        }

        public void ZoomOut()
        {
            pov_d -= 0.01;
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
        }

        public void TurnLeft()
        {
            pov_x += 1;
            pov_y -= 1;

            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
        }

        public void TurnRight()
        {
            pov_x -= 1;
            pov_y += 1;

            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            CastGraphChangedEvent();
            Invalidate();
        }

        internal void RefreshView()
        {
            sr.ReCalculateTransformationsCoeficients(pov_x, pov_y, pov_z, pan_x, pan_y, ClientRectangle.Width, ClientRectangle.Height, pov_d, 0, 0);
            Invalidate();
        }

        private int m_tpsindex = -1;
        private int m_rpmindex = -1;

        internal void HighlightInGraph(int tpsindex, int rpmindex)
        {
            if (tpsindex < 0 || rpmindex < 0)
            {
                sr.ClearHighlightInGraph();
                m_tpsindex = -1;
                m_rpmindex = -1;
            }
            if (tpsindex != m_tpsindex || rpmindex != m_rpmindex)
            {
                m_rpmindex = rpmindex;
                m_tpsindex = tpsindex;
                sr.HighlightInGraph(tpsindex, rpmindex);
                RefreshView();
            }
        }
    }
}
