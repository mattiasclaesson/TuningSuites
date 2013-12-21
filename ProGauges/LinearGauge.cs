using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;

namespace ProGauges
{
    public partial class LinearGauge : UserControl, ISupportInitialize
    {
        private PrivateFontCollection m_pfc = new PrivateFontCollection();

        private Color m_BackGroundColor = Color.FromArgb(24,24,24);
        private bool m_bIsInitializing;
        private Rectangle rcentre;
        private int m_NumberOfDecimals = 0;

        public int NumberOfDecimals
        {
            get { return m_NumberOfDecimals; }
            set { m_NumberOfDecimals = value; }
        }

        private float m_MaxPeakHoldValue = float.MinValue;

        private bool m_ShowRanges = true;

        private System.Timers.Timer peakholdtimer = new System.Timers.Timer();

        [Browsable(true), Category("LinearGauge"), Description("Toggles ranges display on/off"), DefaultValue(typeof(bool), "true")]
        public bool ShowRanges
        {
            get { return m_ShowRanges; }
            set
            {
                if (m_ShowRanges != value)
                {
                    m_ShowRanges = value;
                    base.Invalidate();
                }
            }
        }


        private bool m_ShowValue = true;

        [Browsable(true), Category("LinearGauge"), Description("Toggles value display on/off"), DefaultValue(typeof(bool), "true")]
        public bool ShowValue
        {
            get { return m_ShowValue; }
            set
            {
                if (m_ShowValue != value)
                {
                    m_ShowValue = value;
                    base.Invalidate();
                }
            }
        }
        private bool m_ShowValueInPercentage = false;

        [Browsable(true), Category("LinearGauge"), Description("Toggles value display from actual values to percentage"), DefaultValue(typeof(bool), "false")]
        public bool ShowValueInPercentage
        {
            get { return m_ShowValueInPercentage; }
            set
            {
                if (m_ShowValueInPercentage != value)
                {
                    m_ShowValueInPercentage = value;
                    base.Invalidate();
                }
            }
        }
        private string m_gaugeText = "Linear gauge";

        [Browsable(true), Category("LinearGauge"), Description("Sets the text for the gauge"), DefaultValue(typeof(bool), "Linear gauge")]
        public string GaugeText
        {
            get { return m_gaugeText; }
            set
            {
                if (m_gaugeText != value)
                {
                    m_gaugeText = value;
                    base.Invalidate();
                }
            }

        }

        private string m_gaugeUnits = "units";
        [Browsable(true), Category("LinearGauge"), Description("Sets the units for the gauge"), DefaultValue(typeof(bool), "units")]
        public string GaugeUnits
        {
            get { return m_gaugeUnits; }
            set
            {
                if (m_gaugeUnits != value)
                {
                    m_gaugeUnits = value;
                    base.Invalidate();
                }
            }
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
        private int m_peakholdOpaque = 0;

        private float m_value = 0;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge value"), DefaultValue(typeof(float), "0")]
        public float Value
        {
            get { return m_value; }
            set
            {
                if (m_value != value)
                {
                    m_value = value;
                    base.Invalidate();
                }
                if (m_value > m_MaxPeakHoldValue)
                {
                    m_MaxPeakHoldValue = m_value;
                    m_peakholdOpaque = 255;
                }
            }
        }

        private float m_recommendedValue = 25;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge recommended value"), DefaultValue(typeof(float), "25")]
        public float RecommendedValue
        {
            get { return m_recommendedValue; }
            set
            {
                if (m_recommendedValue != value)
                {
                    m_recommendedValue = value;
                    base.Invalidate();
                }
            }
        }

        private int m_recommendedPercentage = 10;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge recommended percentage"), DefaultValue(typeof(int), "10")]
        public int RecommendedPercentage
        {
            get { return m_recommendedPercentage; }
            set
            {
                if (m_recommendedPercentage != value)
                {
                    m_recommendedPercentage = value;
                    base.Invalidate();
                }
            }
        }

        private float m_thresholdValue = 90;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge threshold value"), DefaultValue(typeof(float), "90")]
        public float ThresholdValue
        {
            get { return m_thresholdValue; }
            set
            {
                if (m_thresholdValue != value)
                {
                    m_thresholdValue = value;
                    base.Invalidate();
                }
            }
        }

        private float m_maxValue = 100;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge maximum scale value"), DefaultValue(typeof(float), "100")]
        public float MaxValue
        {
            get { return m_maxValue; }
            set
            {
                if (m_maxValue != value)
                {
                    m_maxValue = value;
                    base.Invalidate();
                }
            }
        }
        private float m_minValue = 0;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge minimum scale value"), DefaultValue(typeof(float), "0")]
        public float MinValue
        {
            get { return m_minValue; }
            set
            {
                if (m_minValue != value)
                {
                    m_minValue = value;
                    base.Invalidate();
                }
            }
        }


        private void LoadFont()
        {
            Stream fontStream = this.GetType().Assembly.GetManifestResourceStream("ProGauges.Eurostile.ttf");
            if (fontStream != null)
            {
                byte[] fontdata = new byte[fontStream.Length];
                fontStream.Read(fontdata, 0, (int)fontStream.Length);
                fontStream.Close();
                unsafe
                {
                    fixed (byte* pFontData = fontdata)
                    {
                        m_pfc.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
                    }
                }
            }
            else
            {
                throw new Exception("Font could not be found");
            }
        }

        public LinearGauge()
        {
            InitializeComponent();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            peakholdtimer.Interval = 50;
            peakholdtimer.Elapsed += new System.Timers.ElapsedEventHandler(peakholdtimer_Elapsed);
            peakholdtimer.Start();
            GetCenterRectangle();
            try
            {
                LoadFont();
                System.Drawing.Font fn;
                foreach (FontFamily ff in m_pfc.Families)
                {
                    this.Font = fn = new Font(ff, 12, FontStyle.Bold);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        void peakholdtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_peakholdOpaque > 0) m_peakholdOpaque--;
            else
            {
                m_MaxPeakHoldValue = float.MinValue;
            }
        }

        void ISupportInitialize.BeginInit()
        {
            this.m_bIsInitializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            this.m_bIsInitializing = false;
            base.Invalidate();
        }


        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (!this.IsDisposed)
            {
                base.OnPaintBackground(pevent);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!this.IsDisposed)
            {
                base.OnSizeChanged(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.IsDisposed)
            {

                Graphics g = e.Graphics;
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                this.DrawBackground(g);
                if ((base.ClientRectangle.Height >= 85) && (base.ClientRectangle.Width >= 85))
                {
                    this.DrawNumbers(g);
                    this.DrawText(g);
                }
                this.DrawScale(g);
                this.DrawRanges(g);
                this.DrawHighlight(g);
            }            //g.Dispose();
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
                    //if (!this.m_bIsInitializing)
                    //{
                        base.Invalidate();
                    //}
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
                    //if (!this.m_bIsInitializing)
                    //{
                    base.Invalidate();
                    //}
                }
            }
        }

        private void DrawHighlight(Graphics g)
        {
            if (this.m_bShowHighlight)
            {
                Rectangle clientRectangle = base.ClientRectangle;
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


        private Color m_recommendedRangeColor = Color.LawnGreen;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge recommended range color"), DefaultValue(typeof(Color), "System.Drawing.Color.LawnGreen")]
        public Color RecommendedRangeColor
        {
            get { return m_recommendedRangeColor; }
            set
            {
                if (m_recommendedRangeColor != value)
                {
                    m_recommendedRangeColor = value;
                    base.Invalidate();
                }
            }
        }
        private Color m_thresholdColor = Color.Firebrick;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge recommended range color"), DefaultValue(typeof(Color), "System.Drawing.Color.Firebrick")]
        public Color ThresholdColor
        {
            get { return m_thresholdColor; }
            set
            {
                if (m_thresholdColor != value)
                {
                    m_thresholdColor = value;
                    base.Invalidate();
                }
            }
        }


        private Color m_startColor = Color.GreenYellow;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge start color"), DefaultValue(typeof(Color), "System.Drawing.Color.GreenYellow")]
        public Color StartColor
        {
            get { return m_startColor; }
            set
            {
                if (m_startColor != value)
                {
                    m_startColor = value;
                    base.Invalidate();
                }
            }
        }

        private Color m_endColor = Color.OrangeRed;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge end color"), DefaultValue(typeof(Color), "System.Drawing.Color.OrangeRed")]
        public Color EndColor
        {
            get { return m_endColor; }
            set
            {
                if (m_endColor != value)
                {
                    m_endColor = value;
                    base.Invalidate();
                }
            }
        }

        private int m_alphaForGaugeColors = 180;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge alpha value for the gauge colored bar"), DefaultValue(typeof(int), "180")]
        public int AlphaForGaugeColors
        {
            get { return (m_alphaForGaugeColors*100)/255; }
            set
            {
                int realvalue = value * 255 / 100;
                if (m_alphaForGaugeColors != realvalue)
                {
                    m_alphaForGaugeColors = realvalue;
                    base.Invalidate();
                }
            }
        }

        

        /// <summary>
        /// Draws the recommended and threshold ranges on the gauge
        /// </summary>
        /// <param name="g"></param>
        private void DrawRanges(Graphics g)
        {
            if (m_ShowRanges)
            {
                // draw recommended range
                Pen p = new Pen(Color.FromArgb(m_alphaForGaugeColors, m_BevelLineColor));
                float range = m_maxValue - m_minValue;
                Rectangle scalerect = new Rectangle(rcentre.X, rcentre.Y + rcentre.Height + 1, rcentre.Width, 6);
                //scalerect.Inflate(-1, -1);

                if (m_recommendedValue >= m_minValue && m_recommendedValue < m_maxValue)
                {
                    // calculate range based on percentage
                    // percentage = percentage of entire scale!
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(scalerect, m_BackGroundColor, m_recommendedRangeColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                    float centerpercentage = (m_recommendedValue - m_minValue) / range; 
                    float recommendedstartpercentage = centerpercentage;
                    recommendedstartpercentage -= (float)m_recommendedPercentage / 200;
                    float recommendedendpercentage = centerpercentage;
                    recommendedendpercentage += (float)m_recommendedPercentage / 200;
                    float startx = scalerect.Width * recommendedstartpercentage;
                    float endx = scalerect.Width * recommendedendpercentage;
                    float centerx = scalerect.Width * centerpercentage;
                    Rectangle startfillrect = new Rectangle(scalerect.X + (int)startx, scalerect.Y, (int)centerx - (int)startx, scalerect.Height);
                    Rectangle startcolorrect = startfillrect;
                    startcolorrect.Inflate(1, 0);

                    System.Drawing.Drawing2D.LinearGradientBrush gb1 = new System.Drawing.Drawing2D.LinearGradientBrush(startcolorrect, Color.Transparent, m_recommendedRangeColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    g.FillRectangle(gb1, startfillrect);
                    Rectangle endfillrect = new Rectangle(scalerect.X + (int)centerx, scalerect.Y, (int)endx - (int)centerx, scalerect.Height);
                    Rectangle endcolorrect = endfillrect;
                    endcolorrect.Inflate(1, 0);
                    System.Drawing.Drawing2D.LinearGradientBrush gb2 = new System.Drawing.Drawing2D.LinearGradientBrush(endcolorrect, m_recommendedRangeColor, Color.Transparent, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    g.FillRectangle(gb2, endfillrect);
                    g.DrawRectangle(p, startfillrect.X, startfillrect.Y, startfillrect.Width + endfillrect.Width, startfillrect.Height);
                    gb.Dispose();
                    gb1.Dispose();
                    gb2.Dispose();
                    

                }
                
                // draw threshold
                if (m_thresholdValue >= m_minValue && m_thresholdValue < m_maxValue)
                {
                    // percentage
                    float percentage = (m_thresholdValue - m_minValue) / range;
                    if (percentage > 1) percentage = 1;
                    if (percentage < 0) percentage = 0;
                    float startx = scalerect.Width * percentage;
                    Rectangle fillrect = new Rectangle(scalerect.X + (int)startx, scalerect.Y, scalerect.Width-(int)startx, scalerect.Height);
                    Rectangle fillcolorrect = fillrect;
                    fillcolorrect.Inflate(1, 0);
                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(fillcolorrect, Color.Transparent, m_thresholdColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    g.FillRectangle(gb, fillrect);
                    // nog een rectangle erom heen?
                    g.DrawRectangle(p, fillrect);
                    gb.Dispose();

                }
                p.Dispose();
            }
        }


        /// <summary>
        /// Draws the actual scale on the gauge
        /// </summary>
        /// <param name="g"></param>
        private void DrawScale(Graphics g)
        {
            Rectangle scalerect = rcentre;
            scalerect.Inflate(-2, -2);
            Color realstart = Color.FromArgb(m_alphaForGaugeColors, m_startColor);
            Color realend = Color.FromArgb(m_alphaForGaugeColors, m_endColor);
            scalerect = new Rectangle(scalerect.X + 1, scalerect.Y + 1, scalerect.Width, scalerect.Height);
            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(rcentre, realstart, realend, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            // percentage calulation
            float range = m_maxValue - m_minValue;
            float percentage = (m_value - m_minValue) / range;
            //float percentage = (m_value) / (m_maxValue - m_minValue);
            if (percentage > 1) percentage = 1;
            float width = scalerect.Width * percentage;
            Rectangle fillrect = new Rectangle(scalerect.X-1, scalerect.Y-1,(int)width , scalerect.Height+1);
            g.FillRectangle(gb, fillrect);

            // draw peak & hold?

            if (m_MaxPeakHoldValue > float.MinValue && m_peakholdOpaque > 0)
            {
                Color peakholdcolor = Color.FromArgb(m_peakholdOpaque, Color.Red);
                percentage = (m_MaxPeakHoldValue - m_minValue) / range;
                if (percentage > 1) percentage = 1;
                width = scalerect.Width * percentage;
                g.DrawLine(new Pen(peakholdcolor, 3), new Point(scalerect.X - 1 + (int)width, scalerect.Y - 1), new Point(scalerect.X - 1 + (int)width, scalerect.Y + scalerect.Height));
            }

            gb.Dispose();
        }

        private int m_numberOfDivisions = 5;

        [Browsable(true), Category("LinearGauge"), Description("Sets number of divisions that should be drawn"), DefaultValue(typeof(int), "5")]
        public int NumberOfDivisions
        {
            get { return m_numberOfDivisions; }
            set
            {
                if (m_numberOfDivisions != value)
                {
                    m_numberOfDivisions = value;
                    base.Invalidate();
                }
            }
        }

        private Color m_TickColor = Color.Gray;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge tick color"), DefaultValue(typeof(Color), "System.Drawing.Color.Gray")]
        public Color TickColor
        {
            get { return m_TickColor; }
            set
            {
                if (m_TickColor != value)
                {
                    m_TickColor = value;
                    base.Invalidate();
                }

            }
        }

        private int m_subTickCount = 4;

        [Browsable(true), Category("LinearGauge"), Description("Sets number of sub divisions that should be drawn"), DefaultValue(typeof(int), "4")]
        public int SubTickCount
        {
            get { return m_subTickCount; }
            set
            {
                if (m_subTickCount != value)
                {
                    m_subTickCount = value;
                    base.Invalidate();
                }
            }
        }

        private Color m_TextColor = Color.Silver;

        [Browsable(true), Category("LinearGauge"), Description("Set the gauge text color"), DefaultValue(typeof(Color), "System.Drawing.Color.Silver")]
        public Color TextColor
        {
            get { return m_TextColor; }
            set
            {
                if (m_TextColor != value)
                {
                    m_TextColor = value;
                    base.Invalidate();
                }

            }
        }

        /// <summary>
        /// Draws the numbers above the center retangle
        /// </summary>
        /// <param name="g"></param>
        private void DrawNumbers(Graphics g)
        {
            int y_offset = rcentre.Y - 20;
            int x_offset = rcentre.X;
            for (int t = 0; t < m_numberOfDivisions+1; t++)
            {
                int tickWidth = rcentre.Width / (m_numberOfDivisions);
                int xPos = x_offset + t * tickWidth;
                float fval = m_minValue + (t * ((m_maxValue-m_minValue)/m_numberOfDivisions));
                string outstr = fval.ToString("F" + m_NumberOfDecimals.ToString());

//                string outstr = fval.ToString("F0");
                if (fval < 10 && fval > -10 && fval != 0) outstr = fval.ToString("F1");
                if (fval < 1 && fval > -1 && fval != 0) outstr = fval.ToString("F2");
                SizeF textSize = g.MeasureString(outstr, this.Font);
                Pen p = new Pen(Color.FromArgb(80, m_TickColor));
                g.DrawRectangle(p, new Rectangle(xPos, rcentre.Y + 1, 3, rcentre.Height - 2));

                // subticks
                SolidBrush sb = new SolidBrush(Color.FromArgb(80, m_TickColor));
                if (t < m_numberOfDivisions)
                {
                    for (int subt = 0; subt < m_subTickCount; subt++)
                    {
                        int subTickWidth = tickWidth / (m_subTickCount + 1);
                        int xPosSub = xPos + (subt+1) * subTickWidth;
                        g.FillEllipse(sb, xPosSub, rcentre.Y + (rcentre.Height / 2), 3, 3);
                    }
                }
                xPos -= (int)textSize.Width / 2;
                SolidBrush sbtxt = new SolidBrush(m_TextColor);
                g.DrawString(outstr, this.Font, sbtxt, new PointF((float)xPos, (float)y_offset));
                p.Dispose();
                sb.Dispose();
                sbtxt.Dispose();
            }
        }


        /// <summary>
        /// Draws the text under the center retangle
        /// </summary>
        /// <param name="g"></param>
        private void DrawText(Graphics g)
        {
            string text2display = m_gaugeText ;
            if (m_ShowValue)
            {
                
                if (m_ShowValueInPercentage)
                {
                    // add percentage to text
                    float range = m_maxValue - m_minValue;
                    float percentage = (m_value - m_minValue) / range;
                    percentage *= 100;
                    // and percentage sign
                    text2display += " " + percentage.ToString("F0") + " %";
                }
                else
                {
                    // add value to text
                    //string strval = m_value.ToString("F0");
                    string strval = m_value.ToString("F" + m_NumberOfDecimals.ToString());
                    if (m_value > -10 && m_value < 10 && m_value != 0) strval = m_value.ToString("F1");
                    if (m_value > -1 && m_value < 1 && m_value != 0) strval = m_value.ToString("F2");
                    text2display += " " + strval;
                    // and units
                    text2display += " " + m_gaugeUnits;
                }
            }
            SizeF textsize = g.MeasureString(text2display, this.Font);
            SolidBrush sbtxt = new SolidBrush(m_TextColor);
            int xPos = this.ClientRectangle.X + (this.ClientRectangle.Width /2) - ((int)textsize.Width/2);
            g.DrawString(text2display, this.Font, sbtxt, new PointF((float)xPos, rcentre.Y + rcentre.Height + 10));
            sbtxt.Dispose();
        }

        /// <summary>
        /// Draws the background image for that gauge
        /// </summary>
        /// <param name="g"></param>
        private void DrawBackground(Graphics g)
        {
            SolidBrush b = new SolidBrush(m_BackGroundColor);
            g.FillRectangle(b, this.ClientRectangle);
            RectangleF r = this.ClientRectangle;
            r.Inflate(-3, -3);
            Pen p = new Pen(m_BevelLineColor, 2);
            g.DrawRectangle(p, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
            g.DrawRectangle(Pens.DimGray, rcentre);
            b.Dispose();
            p.Dispose();
        }

        private void GetCenterRectangle()
        {
            rcentre = new Rectangle(this.ClientRectangle.X + this.ClientRectangle.Width / 8, this.ClientRectangle.Y + (this.ClientRectangle.Height * 3) / 8, (this.ClientRectangle.Width * 6) / 8, (this.ClientRectangle.Height * 2) / 8);
        }

        private void LinearGauge_Resize(object sender, EventArgs e)
        {
            GetCenterRectangle();            
            base.Invalidate();
        }

    }
}