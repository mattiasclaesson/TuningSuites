using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ProCharts
{
    public partial class LineChart : UserControl, ISupportInitialize
    {
        private Color m_BackGroundColor = Color.FromArgb(24, 24, 24);
        private Rectangle rcentre;


        public LineChart()
        {
            InitializeComponent();
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            base.Invalidate();
        }

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge background color"), DefaultValue(typeof(Color), "System.Drawing.Color.Black")]
        public Color BackGroundColor
        {
            get { return m_BackGroundColor; }
            set
            {
                m_BackGroundColor = value;
                base.Invalidate();
            }
        }

        private Color m_BevelLineColor = Color.Gray;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge bevel color"), DefaultValue(typeof(Color), "System.Drawing.Color.Gray")]
        public Color BevelLineColor
        {
            get { return m_BevelLineColor; }
            set
            {
                m_BevelLineColor = value;
                base.Invalidate();
            }
        }

        private ChannelCollection m_channelCollection = new ChannelCollection();

        public void AddPointCollection(PointCollection pc)
        {
            foreach (PointHelper p in pc)
            {
                bool foundchannel = false;
                foreach (ChannelHelper ch in m_channelCollection)
                {
                    if (ch.ChannelName == p.Channelname)
                    {
                        ch.PointCollection.Add(p);
                        if (p.DateTimeValue > m_MaxDateTime) m_MaxDateTime = p.DateTimeValue;
                        if (p.DateTimeValue < m_MinDateTime) m_MinDateTime = p.DateTimeValue;
                        if (p.Pointvalue < ch.MinValue) ch.MinValue = p.Pointvalue;
                        if (p.Pointvalue > ch.MaxValue) ch.MaxValue = p.Pointvalue;
                        foundchannel = true;
                    }
                }
                if (!foundchannel)
                {
                    ChannelHelper nch = new ChannelHelper();
                    nch.ChannelName = p.Channelname;
                    nch.PointCollection.Add(p);
                    nch.ChannelColor = Color.FromArgb(m_channelCollection.Count * 10, 255 - (m_channelCollection.Count * 10), 128);
                    if (p.DateTimeValue > m_MaxDateTime) m_MaxDateTime = p.DateTimeValue;
                    if (p.DateTimeValue < m_MinDateTime) m_MinDateTime = p.DateTimeValue;
                    if (p.Pointvalue < nch.MinValue) nch.MinValue = p.Pointvalue;
                    if (p.Pointvalue > nch.MaxValue) nch.MaxValue = p.Pointvalue;
                    m_channelCollection.Add(nch);
                }
            }
            Console.WriteLine("mindate: "+ m_MinDateTime.ToString() + " maxdate: " + m_MaxDateTime.ToString());
            this.Invalidate(true);
        }

        private DateTime m_MinDateTime = DateTime.MaxValue;
        private DateTime m_MaxDateTime = DateTime.MinValue;

        private void DrawLines(Graphics g)
        {
            // draw each line in the linecollection
            // use the x_axis scale (time!)
            TimeSpan ts = new TimeSpan(m_MaxDateTime.Ticks - m_MinDateTime.Ticks);
            double totalmilliseconds = ts.TotalMilliseconds;

            foreach (ChannelHelper ch in m_channelCollection)
            {
                // First sort the points in the collection!!! 
                Pen p = new Pen(ch.ChannelColor);

                //ch.PointCollection.SortColumn =;
                //ch.PointCollection.Sort();

                // then plot them
                bool first = true;
                int previous_x = 0;
                int previous_y = 0;
                foreach (PointHelper ph in ch.PointCollection)
                {

                    if (ph.DateTimeValue >= m_MinDateTime && ph.DateTimeValue <= m_MaxDateTime)
                    {

                        // determine where to plot
                        TimeSpan tsp = new TimeSpan(ph.DateTimeValue.Ticks - m_MinDateTime.Ticks);
                        int x_offset = (int)((((float)tsp.TotalMilliseconds / (float)totalmilliseconds)) * (float)splitContainer2.Panel1.ClientRectangle.Width);
                        int y_offset = (int)((ph.Pointvalue - ch.MinValue) * splitContainer2.Panel1.ClientRectangle.Height / (ch.MaxValue - ch.MinValue));
                        int x = x_offset;
                        int y = splitContainer2.Panel1.ClientRectangle.Height - y_offset;
                        //g.DrawEllipse(p , x , y -10, 3, 3);
                        if (!first)
                        {
                            g.DrawLine(p, previous_x, previous_y, x, y);
                        }
                        previous_x = x;
                        previous_y = y;
                    }
                    
                    first = false;
                }
                p.Dispose();
            }
        }

        private void DrawScale(Graphics g)
        {
            // draw x scale and y scale(s)
            TimeSpan ts = new TimeSpan(m_MaxDateTime.Ticks - m_MinDateTime.Ticks);
            for (int t = 0; t < 10; t++)
            {
            }

        }

        private void DrawText(Graphics g)
        {
            
        }

        private void DrawNumbers(Graphics g)
        {
            
        }

       
        private bool m_bShowHighlight = true;

        [Browsable(true), Category("LinearGauge"), Description("Switches highlighting of the gauge on and off"), DefaultValue(typeof(bool), "true")]
        public bool ShowHighlight
        {
            get { return m_bShowHighlight; }
            set
            {
                if (m_bShowHighlight != value)
                {
                    m_bShowHighlight = value;
                    base.Invalidate();
                }
            }
        }

        private byte m_nHighlightOpaqueEnd = 30;


        [DefaultValue(50), Browsable(true), Category("LinearGauge"), Description("Set the opaque value of the highlight")]
        public byte HighlightOpaqueEnd
        {
            get
            {
                return this.m_nHighlightOpaqueEnd;
            }
            set
            {
                if (value > 100)
                {
                    throw new ArgumentException("This value should be between 0 and 50");
                }
                if (this.m_nHighlightOpaqueEnd != value)
                {
                    this.m_nHighlightOpaqueEnd = value;
                    base.Invalidate();
                }
            }
        }


        private byte m_nHighlightOpaqueStart = 100;


        [DefaultValue(100), Browsable(true), Category("LinearGauge"), Description("Set the opaque start value of the highlight")]
        public byte HighlightOpaqueStart
        {
            get
            {
                return this.m_nHighlightOpaqueStart;
            }
            set
            {
                if (value > 255)
                {
                    throw new ArgumentException("This value should be between 0 and 50");
                }
                if (this.m_nHighlightOpaqueStart != value)
                {
                    this.m_nHighlightOpaqueStart = value;
                    base.Invalidate();
                }
            }
        }

        private void DrawHighlight(Graphics g)
        {
            if (this.m_bShowHighlight)
            {
                Rectangle clientRectangle = splitContainer2.Panel1.ClientRectangle;
                clientRectangle.Height = clientRectangle.Height >> 1;
                clientRectangle.Inflate(-2, -2);
                Color color = Color.FromArgb(this.m_nHighlightOpaqueStart, 0xff, 0xff, 0xff);
                Color color2 = Color.FromArgb(this.m_nHighlightOpaqueEnd, 0xff, 0xff, 0xff);
                this.DrawRoundRect(g, clientRectangle, /*((this.m_nCornerRadius - 1) > 1) ? ((float)(this.m_nCornerRadius - 1)) :*/ ((float)1), color, color2, Color.Empty, 0, true, false);
            }
            else
            {
                /*Rectangle clientRectangle = base.ClientRectangle;
                clientRectangle.Height = clientRectangle.Height >> 1;
                clientRectangle.Inflate(-2, -2);
                Color color = Color.FromArgb(100, 0xff, 0xff, 0xff);
                Color color2 = Color.FromArgb(this.m_nHighlightOpaque, 0xff, 0xff, 0xff);
                Brush backGroundBrush = new SolidBrush(Color.FromArgb(120, Color.Silver));
                g.FillEllipse(backGroundBrush, clientRectangle);*/
            }
        }


        private void DrawRoundRect(Graphics g, Rectangle rect, float radius, Color col1, Color col2, Color colBorder, int nBorderWidth, bool bGradient, bool bDrawBorder)
        {
            GraphicsPath path = new GraphicsPath();
            float width = radius + radius;
            RectangleF ef = new RectangleF(0f, 0f, width, width);
            Brush brush = null;
            ef.X = rect.Left;
            ef.Y = rect.Top;
            path.AddArc(ef, 180f, 90f);
            ef.X = (rect.Right - 1) - width;
            path.AddArc(ef, 270f, 90f);
            ef.Y = (rect.Bottom - 1) - width;
            path.AddArc(ef, 0f, 90f);
            ef.X = rect.Left;
            path.AddArc(ef, 90f, 90f);
            path.CloseFigure();
            if (bGradient)
            {
                brush = new LinearGradientBrush(rect, col1, col2, 90f, false);
            }
            else
            {
                brush = new SolidBrush(col1);
            }
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(brush, path);
            if (/*bDrawBorder*/ true)
            {
                Pen pen = new Pen(colBorder);
                pen.Width = nBorderWidth;
                g.DrawPath(pen, path);
                pen.Dispose();
            }
            g.SmoothingMode = SmoothingMode.None;
            brush.Dispose();
            path.Dispose();
        }

        private void LineChart_Resize(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!this.IsDisposed)
            {

                Graphics g = e.Graphics;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                this.DrawBackground(g);
                if ((base.ClientRectangle.Height >= 85) && (base.ClientRectangle.Width >= 85))
                {
                    this.DrawNumbers(g);
                    this.DrawText(g);
                }
                this.DrawScale(g);
                this.DrawLines(g);
                this.DrawHighlight(g);
            }            //g.Dispose();
        }

        private void GetCenterRectangle()
        {

            rcentre = new Rectangle(splitContainer2.Panel1.ClientRectangle.X + splitContainer2.Panel1.ClientRectangle.Width / 8, splitContainer2.Panel1.ClientRectangle.Y + (splitContainer2.Panel1.ClientRectangle.Height * 3) / 8, (splitContainer2.Panel1.ClientRectangle.Width * 6) / 8, (splitContainer2.Panel1.ClientRectangle.Height * 2) / 8);
        }

        private void DrawBackground(Graphics g)
        {
            SolidBrush b = new SolidBrush(m_BackGroundColor);
            g.FillRectangle(b, splitContainer2.Panel1.ClientRectangle);
            RectangleF r = splitContainer2.Panel1.ClientRectangle;
            r.Inflate(-3, -3);
            Pen p = new Pen(m_BevelLineColor, 2);
            g.DrawRectangle(p, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
            //g.DrawRectangle(Pens.DimGray, rcentre);
            b.Dispose();
            p.Dispose();
        }


        private void splitContainer2_Panel1_Resize(object sender, EventArgs e)
        {
            GetCenterRectangle();

        }

        private int m_numberOfDivisions = 10;
/*        private void GetCenterRectangle()
        {
            rcentre = new Rectangle(this.ClientRectangle.X + this.ClientRectangle.Width / 8, this.ClientRectangle.Y + (this.ClientRectangle.Height * 3) / 8, (this.ClientRectangle.Width * 6) / 8, (this.ClientRectangle.Height * 2) / 8);
        }*/

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush b = new SolidBrush(m_BackGroundColor);
            e.Graphics.FillRectangle(b, splitContainer2.Panel2.ClientRectangle);
            b.Dispose();
            // paint all x axis stuff
            if (m_channelCollection.Count > 0)
            {
                // background
                // values
                TimeSpan ts = new TimeSpan(m_MaxDateTime.Ticks - m_MinDateTime.Ticks);
                double totalmilliseconds = ts.TotalMilliseconds;
                int sizex = splitContainer2.Panel2.ClientRectangle.Width;
                float sizeperdivision = (float)sizex / (float)m_numberOfDivisions;
                for (int xscale = 1; xscale < m_numberOfDivisions; xscale++)
                {
                    float xpos = (float)xscale * sizeperdivision;
                    float doutstr = (float)totalmilliseconds * ((float)xscale/ (float)m_numberOfDivisions);
                    
                    string outstr = doutstr.ToString("F1");

                    SizeF textSize = e.Graphics.MeasureString(outstr, this.Font);
                    float xpostext = xpos - textSize.Width / 2;
                    e.Graphics.DrawString(outstr, this.Font, Brushes.Wheat, xpostext, 10F);
                    e.Graphics.DrawLine(Pens.Wheat, xpos, 0, xpos, 8);

                }
            }
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            SolidBrush b = new SolidBrush(m_BackGroundColor);
            e.Graphics.FillRectangle(b, splitContainer1.Panel1.ClientRectangle);
            b.Dispose();
            // draw y scales
            // one scale for every channel
            int channelnumber = 0;
            foreach (ChannelHelper ch in m_channelCollection)
            {
                // determine x position for text
                SolidBrush sb = new SolidBrush(ch.ChannelColor);
                double totalscale = ch.MaxValue - ch.MinValue;
                int sizey = splitContainer2.Panel1.ClientRectangle.Height;
                float sizeperdivision = (float)sizey / (float)m_numberOfDivisions;
                for (int yscale = 1; yscale < m_numberOfDivisions; yscale++)
                {
                    float ypos = (float)yscale * sizeperdivision;
                    float doutstr = (float)totalscale * ((float)(m_numberOfDivisions - yscale) / (float)m_numberOfDivisions) + ch.MinValue;
                    string outstr = doutstr.ToString("F1");
                    SizeF textSize = e.Graphics.MeasureString(outstr, this.Font);
                    float ypostext = ypos - textSize.Width / 2;
                    
                    e.Graphics.DrawString(outstr, this.Font, sb, channelnumber * (65), ypostext);
                }

                string text = ch.ChannelName;
                float width = e.Graphics.MeasureString(text, this.Font).Width;
                float height = e.Graphics.MeasureString(text, this.Font).Height;
                //double angle =  Math.PI/2;
                float rotationAngle = -90;
                e.Graphics.TranslateTransform((channelnumber * 65) + 10, splitContainer1.Panel1.ClientRectangle.Height - 100);
                    //(splitContainer1.Panel1.ClientRectangle.Width + (float)(height * Math.Sin(angle)) - (float)(width * Math.Cos(angle))) / 2,
                    //(splitContainer1.Panel1.ClientRectangle.Height - (float)(height * Math.Cos(angle)) - (float)(width * Math.Sin(angle))) / 6);
                e.Graphics.RotateTransform((float)rotationAngle);
                e.Graphics.DrawString(text, this.Font, sb, 0, 0);
                e.Graphics.ResetTransform();


                sb.Dispose();
                channelnumber++;
            }
        }
    }
}