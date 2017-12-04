using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using DevExpress.XtraEditors.Controls;

namespace OnlineGraph
{
    public enum PanType : int
    {
        PanLeft,
        PanRight
    }

    public partial class OnlineGraphControl : UserControl
    {
        public delegate void GraphPainted(object sender, EventArgs e);

        private GraphLineCollection _lines = new GraphLineCollection();
        public event GraphPainted onGraphPainted;

        private string _contextDescription = string.Empty;
        private string _selectedSymbol = string.Empty;
        private bool _panning = false;

        public bool Panning
        {
            get { return _panning; }
            set { _panning = value; }
        }
        private PanType _pantype = PanType.PanLeft;

        public PanType Pantype
        {
            get { return _pantype; }
            set { _pantype = value; }
        }
        private float _zoomfactor = 1;

        public float Zoomfactor
        {
            get { return _zoomfactor; }
            set
            {
                _zoomfactor = value;
                Invalidate();
            }
        }
        private DateTime _centerDateTime = DateTime.Now;

        public DateTime CenterDateTime
        {
            get { return _centerDateTime; }
            set
            {
                _centerDateTime = value;
                Invalidate();
            }
        }
        private DateTime _mindt = DateTime.Now;

        public DateTime Mindt
        {
            get { return _mindt; }
            set { _mindt = value; }
        }
        private DateTime _maxdt = DateTime.Now;

        public DateTime Maxdt
        {
            get { return _maxdt; }
            set { _maxdt = value; }
        }

        public string ContextDescription
        {
            get { return _contextDescription; }
            set { _contextDescription = value; }
        }


        private bool m_allowconfig = true;

        public bool Allowconfig
        {
            get { return m_allowconfig; }
            set { m_allowconfig = value; }
        }

        public OnlineGraphControl()
        {
            InitializeComponent();
        }

        public void AddMeasurement(string Graphname, string SymbolName, DateTime Timestamp, float value, float minrange, float maxrange, Color linecolor)
        {
            bool _linefound = false;
            //Console.WriteLine(Graphname + " " + SymbolName + " " + value.ToString());
            foreach (GraphLine line in _lines)
            {
                if (line.Symbol == SymbolName)
                {
                    _linefound = true;
//                    if (value < minrange) minrange = value - 5;
//                    if (value > maxrange) maxrange = value + 5;

                    line.AddPoint(value, Timestamp, minrange, maxrange, linecolor);
                    break;
                }
            }
            if (!_linefound)
            {
                GraphLine _newline = new GraphLine();
                _newline.Symbol = SymbolName;
                _newline.ChannelName = Graphname;
                _newline.Clear();
                _lines.Add(_newline);
//                if (value < minrange) minrange = value;
//                if (value > maxrange) maxrange = value;
                _newline.AddPoint(value, Timestamp, minrange, maxrange, linecolor);
                // set visible or invisible according to registry setting
                _newline.LineVisible = GetRegistryValue(Graphname);
            }
        }

        private bool GetRegistryValue(string key)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            bool retval = true;

            using (RegistryKey Settings = TempKey.CreateSubKey("T5Suite\\Channels"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == key.ToUpper())
                            {
                                retval = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            return retval;
        }

        private void WriteLastestValuesToLog(int m_knock)
        {
            DateTime datet = DateTime.Now;

            string logline = datet.ToString("dd/MM/yyyy HH:mm:ss") + "." + datet.Millisecond.ToString("D3") + "|";
            foreach (GraphLine line in _lines)
            {
                logline += line.Symbol + "=" + line.Measurements[line.Measurements.Count - 1].Value.ToString("F2") + "|";
            }
            logline += "KnockInfo=" + m_knock.ToString() + "|";
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "-"+ _contextDescription + ".t5l", true))
            {
                sw.WriteLine(logline);
            }
        }

        public void ForceRepaint(int m_knock)
        {
            this.Invalidate();
            // all new values, write them to a logfile (5tl)
            WriteLastestValuesToLog(m_knock);
        }

        private void OnlineGraphControl_Paint(object sender, PaintEventArgs e)
        {
            // paint the graphs in the control
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.DrawBackground(this.ClientRectangle, e.Graphics);
            //this.DrawYScale(this.ClientRectangle, e.Graphics);
            //this.DrawYLines(this.ClientRectangle, e.Graphics);
            //this.DrawXScale(this.ClientRectangle, e.Graphics);
            this.DrawGrid(this.ClientRectangle, e.Graphics);
            this.DrawLines(this.ClientRectangle, e.Graphics);
            this.DrawLabels(this.ClientRectangle, e.Graphics);

            // e.Clipractangle
            if (onGraphPainted != null)
            {
                onGraphPainted(this, EventArgs.Empty);
            }
        }

        private void DrawGrid(Rectangle r, Graphics graphics)
        {
            int numberofsections = 20;
            int numberofscales = 10;
            Color c = Color.FromArgb(175, Color.DimGray);
            Pen p = new Pen(c);
            float sectionwidth = (float)(r.Width - 75) / numberofsections;
            float scaleheight = (float)(r.Height) / numberofscales;
            for (int xcount = 1; xcount < numberofsections; xcount++)
            {
                graphics.DrawLine(p, xcount * sectionwidth, 0, xcount * sectionwidth , r.Height);
            }
            for (int ycount = 1; ycount < numberofscales; ycount++)
            {
                graphics.DrawLine(p, 0, ycount * scaleheight, r.Width - 75, ycount * scaleheight);
            }
            p.Dispose();
        }

        private DateTime GetMinDateTime()
        {
            DateTime retval = DateTime.Now.AddDays(1);
            foreach (GraphLine _line in _lines)
            {
                foreach (GraphMeasurement gm in _line.Measurements)
                {
                    if (gm.Timestamp < retval) retval = gm.Timestamp;
                }
            }
            return retval;
        }

        private DateTime GetMaxDateTime()
        {
            DateTime retval = DateTime.MinValue;
            foreach (GraphLine _line in _lines)
            {
                foreach (GraphMeasurement gm in _line.Measurements)
                {
                    if (gm.Timestamp > retval) retval = gm.Timestamp;
                }
            }
            if (retval == DateTime.MinValue) retval = DateTime.Now;
            return retval;
        }


        private void DrawXScale(Rectangle r, Graphics graphics)
        {
            _mindt = GetMinDateTime();
            _maxdt = GetMaxDateTime();

            TimeSpan ts = new TimeSpan(_maxdt.Ticks - _mindt.Ticks);

             if (_zoomfactor == 1)
            {
                _centerDateTime = new DateTime(_mindt.Ticks + ((_maxdt.Ticks - _mindt.Ticks) / 2));
            }
            else
            {
                // dan moet er iets gebeuren
                _mindt = _centerDateTime.AddTicks(-(long)((ts.Ticks / 2) / _zoomfactor /** 10*/));
                _maxdt = _centerDateTime.AddTicks((long)((ts.Ticks / 2) / _zoomfactor /** 10*/));
            }
            ts = new TimeSpan(_maxdt.Ticks - _mindt.Ticks);
            Font f = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
            Pen pen =  new Pen(Color.FromArgb(100, Color.Wheat));
            // total number of labels depends on with of the drawing surface
            SizeF stringsize = graphics.MeasureString("00/00 00:00:00", f);
            int numberoflabels = (r.Width - 100)/ ((int)stringsize.Width + 10);
            //Console.WriteLine("dates: " + mindt.ToString("dd/MM HH:mm:ss") + " - " + maxdt.ToString("dd/MM HH:mm:ss") + " #labels: " + numberoflabels.ToString());
            for (int xtel = 0; xtel < numberoflabels; xtel++)
            {
                DateTime xvalue = _mindt.AddTicks(ts.Ticks / numberoflabels * xtel);
                PointF p = new PointF(100 + (xtel * ((float)stringsize.Width + 10)) , r.Top + r.Height - 80);
                graphics.DrawString(xvalue.ToString("dd/MM HH:mm:ss"), f, Brushes.White, p);
                graphics.DrawLine(pen, (int)p.X , r.Top + 10, (int)p.X , r.Top + r.Height - 98);
            }
            pen.Dispose();
        }

        private void DrawYScale(Rectangle r, Graphics graphics)
        {
            float _maxscale = 0;
            float _minscale = 0;
            bool drawYscale = false;
            Color lineColor = Color.White;
            if (_lines.Count == 1)
            {
                _selectedSymbol = _lines[0].Symbol;
            }
            if (_selectedSymbol != string.Empty)
            {
                foreach (GraphLine line in _lines)
                {
                    if (line.Symbol == _selectedSymbol)
                    {
                        _maxscale = line.Maxrange;
                        _minscale = line.Minrange;
                        lineColor = line.LineColor;
                        drawYscale = true;
                        //Console.WriteLine("sym: " + line.Symbol + " max = " + _maxscale.ToString() + " min = " + _minscale.ToString());
                        break;
                    }
                }
            }
            

            if (drawYscale)
            {
                // number to draw is determined by the height of the control and the selected font
                SolidBrush sbback = new SolidBrush(this.BackColor);
                graphics.FillRectangle(sbback, 0, 0, 100, r.Height);
                Pen pen = new Pen(Color.FromArgb(100, Color.Wheat));
                SolidBrush sb = new SolidBrush(lineColor);
                SizeF stringsize = graphics.MeasureString("000", this.Font);
                int numberoflabels = (r.Height - 100) / ((int)stringsize.Height + 20);
                //int numberoflabels = 30;
                float divisionvalue = (_maxscale - _minscale) / (float)(numberoflabels);
                float divisionpixels = (r.Height - 100) / (float)(numberoflabels );
                for (int ytel = numberoflabels -1; ytel > 0  ; ytel--)
                {
                        float yvalue = _maxscale - (ytel * divisionvalue);
                        //PointF p = new PointF(50 - (stringsize.Width / 2), ytel * ((float)stringsize.Height + 20) - (stringsize.Height / 2));
                        PointF p = new PointF(50 - (stringsize.Width / 2), ytel * divisionpixels);
                        graphics.DrawString(yvalue.ToString("F2"), this.Font, sb, p);
                   // graphics.DrawLine(pen, 95, p.Y + stringsize.Height / 2, r.Width - 100, p.Y + stringsize.Height / 2);
                }
                sbback.Dispose();
                sb.Dispose();
                pen.Dispose();
            }
        }

        private void DrawYLines(Rectangle r, Graphics graphics)
        {

            Pen pen = new Pen(Color.FromArgb(100, Color.Wheat));
            SizeF stringsize = graphics.MeasureString("000", this.Font);
            int numberoflabels = /*30;*/ (r.Height - 100) / ((int)stringsize.Height + 20);
            float divisionpixels = (r.Height - 100) / (float)(numberoflabels );
            for (int ytel = numberoflabels - 1; ytel > 0; ytel--)
            //for (int ytel = 0; ytel < numberoflabels; ytel++)
            {
                PointF p = new PointF(5,  (ytel * divisionpixels)/* ytel * ((float)stringsize.Height + 20)*/ /*- stringsize.Height/2*/);
                graphics.DrawLine(pen, 100, p.Y, r.Width - 100, p.Y );
            }
            pen.Dispose();

        }

        private void DrawLabels(Rectangle r, Graphics graphics)
        {
            SolidBrush brsh = new SolidBrush(Color.Red);
                
            int cnt = 0;

            foreach (GraphLine line in _lines)
            {
                if (line.LineVisible)
                {
                    brsh.Color = line.LineColor;
                    PointF pntF = new PointF((float)(r.Width - 70), (float)((cnt + 1) * 20));
                    graphics.DrawString(line.ChannelName, this.Font, brsh, pntF);
                    cnt++;
                }
            }
            // determine scales in Y axis
            
            brsh.Dispose();
        }

        public void AddMeasurementToCollection(GraphLineCollection coll, string Graphname, string SymbolName, DateTime Timestamp, float value, float minrange, float maxrange, Color linecolor)
        {
            bool _linefound = false;
            foreach (GraphLine line in coll)
            {
                if (line.Symbol == SymbolName)
                {
                    _linefound = true;
                    //                    if (value < minrange) minrange = value - 5;
                    //                    if (value > maxrange) maxrange = value + 5;

                    line.AddPoint(value, Timestamp, minrange, maxrange, linecolor);
                    break;
                }
            }
            if (!_linefound)
            {
                GraphLine _newline = new GraphLine();
                _newline.Symbol = SymbolName;
                _newline.ChannelName = Graphname;
                _newline.Clear();
                coll.Add(_newline);
                _newline.AddPoint(value, Timestamp, minrange, maxrange, linecolor);
                _newline.LineVisible = GetRegistryValue(Graphname);
            }
        }


        private GraphLineCollection GetLinesInSelection()
        {
            GraphLineCollection coll = new GraphLineCollection();
            //Console.WriteLine("date selection: " + _mindt.ToString("dd/MM HH:mm:ss") + " - " + _maxdt.ToString("dd/MM HH:mm:ss"));
            bool _measurementfound = false;
            while (!_measurementfound)
            {
                foreach (GraphLine _line in _lines)
                {
                    foreach (GraphMeasurement gm in _line.Measurements)
                    {
                        if (gm.Timestamp >= _mindt && gm.Timestamp <= _maxdt)
                        {
                            _measurementfound = true;
                            AddMeasurementToCollection(coll, _line.ChannelName, _line.Symbol, gm.Timestamp, gm.Value, _line.Minrange, _line.Maxrange, _line.LineColor);
                        }
                    }
                }
                if (!_measurementfound)
                {
                    if(!(PanToLeft())) _measurementfound = true;
                }
            }
            return coll;

        }

        private void DrawLines(Rectangle r, Graphics graphics)
        {
            Pen p = new Pen(Color.Red);
            //SolidBrush sb = new SolidBrush(Color.Red);
            foreach (GraphLine line in _lines)
            {
                if (line.LineVisible)
                {
                    //Console.WriteLine("Visible line: " + line.ChannelName);
                    if (line.Maxrange == 0) line.Maxrange = 1;
                    //Console.WriteLine(line.Symbol + " contains: " + line.Numberofmeasurements.ToString());
                    int cnt = 0;
                    foreach (GraphMeasurement measurement in line.Measurements)
                    {
                        //Console.WriteLine("  measurement " + measurement.Timestamp.ToString("HH:mm:ss") + " " + measurement.Value.ToString("F2"));
                        if (cnt > 0)
                        {
                            float sectionwidth = (float)(r.Width - 75) / (float)line.Maxpoints;
                            float x1 = (float)(cnt - 1) * sectionwidth ;
                            float x2 = (float)cnt * sectionwidth ;
                            float y1 = (r.Height ) - (line.Measurements[cnt - 1].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height );
                            float y2 = (r.Height ) - (line.Measurements[cnt].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height );
//                            Console.WriteLine("Line with value from : " + line.Measurements[cnt -1].Value.ToString() + " and to: " + line.Measurements[cnt].Value.ToString() + " x1: " + x1.ToString() + " y1: " + y1.ToString() + " x2: " + x2.ToString() + " y2: " + y2.ToString());
                            p.Color = line.LineColor;
                            //sb.Color = line.LineColor;
                            graphics.DrawLine(p, x1, y1, x2, y2);
                            //graphics.FillEllipse(sb, x2-2, y2-2, 4, 4);
                            //graphics.FillEllipse(sb, x1 - 2, y1 - 2, 4, 4); 
                        }
                        cnt++;
                    }
                }
            }
            p.Dispose();
            //    sb.Dispose();
        }

        private void DrawBackground(Rectangle r, Graphics graphics)
        {
            SolidBrush sbback = new SolidBrush(this.BackColor);
            graphics.FillRectangle(sbback, 0 , 0, r.Width, r.Height);
            sbback.Dispose();
        }

        private void OnlineGraphControl_Click(object sender, EventArgs e)
        {
            
        }

        private void DumpLineVisiblity()
        {
            foreach (GraphLine line in _lines)
            {
                Console.WriteLine("Channel: " + line.ChannelName + " symbol: " + line.Symbol + " visible: " + line.LineVisible.ToString());
            }
        }

      
        private void DoConfig()
        {
            frmLineselection linesel = new frmLineselection();
            linesel.TopMost = true;
            //linesel.checkedListBoxControl1.DataSource = _lines;
            linesel.SetDataSource(_lines);
            //DumpLineVisiblity();
            if (linesel.ShowDialog() == DialogResult.OK)
            {
                // save values to file/registry
                foreach (CheckedListBoxItem item in linesel.checkedListBoxControl1.Items)
                {
                    foreach (GraphLine line in _lines)
                    {
                        if (line.ChannelName == (string)item.Description)
                        {
                            if (item.CheckState == CheckState.Unchecked || item.CheckState == CheckState.Indeterminate)
                            {
                                line.LineVisible = false;
                            }
                            else
                            {
                                line.LineVisible = true;
                            }
                            break;
                        }
                    }
                }
               
                SaveConfig();
                Invalidate();
            }
            //DumpLineVisiblity();
        }

        private void SaveConfig()
        {
            foreach (GraphLine line in _lines)
            {
                SaveRegistrySetting(line.ChannelName, line.LineVisible);
            }
        }

        private void SaveRegistrySetting(string key, bool value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T5Suite\\Channels"))
            {
                saveSettings.SetValue(key.ToUpper(), value);
            }
        }


        private void OnlineGraphControl_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = PointToClient(e.Location);
//            Console.WriteLine(e.Location.X.ToString() + " " + p.X.ToString());
            if (e.Button == MouseButtons.Left)
            {
                if (e.Location.X > this.ClientRectangle.Width - 75)
                {
                    if (m_allowconfig)
                    {
                        DoConfig();
                    }
                }
                // anders misschien inzoomen
                else if (e.Location.Y > this.ClientRectangle.Height - 100)
                {
//                    Console.WriteLine("Zooming in");
                    // determine centerdatetime by the point the user clicked
                    _centerDateTime = GetCenterDateTimeByMouseClick(e.Location.X, e.Location.Y);
                    TryToZoomIn();
                }
                else
                {
                    // pan left or right depending on position on graph
                    if (e.Location.X > this.ClientRectangle.Width / 2)
                    {
                         // pan right
//                        Console.WriteLine("Panning right");
                        PanToRight();
                    }
                    else
                    {
                        // pan left
//                        Console.WriteLine("Panning left");
                        PanToLeft();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // uitzoomen
//                Console.WriteLine("Zooming out");
                _centerDateTime = GetCenterDateTimeByMouseClick(e.Location.X, e.Location.Y);
                TryToZoomOut();
            }
        }

        private DateTime GetCenterDateTimeByMouseClick(int x, int y)
        {
            // relative y position in selected daterange
            DateTime returnvalue = _centerDateTime;
            int graphwidth = this.ClientRectangle.Width - 175;
            int positioningraph = x - 100;
            float percentage = (float)positioningraph / graphwidth;
            TimeSpan ts = new TimeSpan(_maxdt.Ticks - _mindt.Ticks);
            returnvalue = _mindt.AddTicks((long)(percentage * ts.Ticks));
            return returnvalue;

        }

        private bool PanToLeft()
        {
            bool _panned = false;
            /*if (_zoomfactor == 1)
            {
                // kan niet pannen nie
            }
            else
            {
                _centerDateTime = GetPreviousCenterDateTime(out _panned);
                Invalidate();
            }*/
            return _panned;
        }

        private DateTime GetNextCenterDateTime(out bool _panned)
        {
            _panned = true;
            // add a percentage depending on zoomfactor
            DateTime maxdt = GetMaxDateTime();
            DateTime mindt = GetMinDateTime();
            TimeSpan ts = new TimeSpan(maxdt.Ticks - mindt.Ticks);
            _centerDateTime = _centerDateTime.AddTicks((long)(ts.Ticks / (_zoomfactor * 10)));
            if (_centerDateTime > maxdt)
            {
                _centerDateTime = maxdt.AddTicks(-(long)((ts.Ticks / 50)));
                _panned = false;
            }
            return _centerDateTime;
        }

        private bool PanToRight()
        {
            bool _panned = false;
            /*if (_zoomfactor == 1)
            {
                // kan niet pannen nie
            }
            else
            {
                _centerDateTime = GetNextCenterDateTime(out _panned);
                Invalidate();
            }*/
            return _panned;
        }

        private DateTime GetPreviousCenterDateTime(out bool _panned)
        {
            // substract a percentage depending on zoomfactor
            _panned = true;
            DateTime maxdt = GetMaxDateTime();
            DateTime mindt = GetMinDateTime();

            TimeSpan ts = new TimeSpan(maxdt.Ticks - mindt.Ticks);
            _centerDateTime = _centerDateTime.AddTicks(-(long)(ts.Ticks / (_zoomfactor * 10)));
            if (_centerDateTime < mindt)
            {
                _centerDateTime = mindt.AddTicks((long)ts.Ticks / 50);
                _panned = false;
            }
            return _centerDateTime;

        }

        private void TryToZoomOut()
        {
           /* if (_zoomfactor == 1)
            {
                
            }
            else if (_zoomfactor <= 1)
            {
                _zoomfactor = 1;
                Invalidate();
            }
            else
            {
                // zoomfactor > 1
                _zoomfactor-=2;
                Invalidate();
            }*/

        }

        private void TryToZoomIn()
        {
            /*if (_zoomfactor == 40)
            {
                
            }
            else if (_zoomfactor > 40)
            {
                _zoomfactor = 40;
                Invalidate();
            }
            else
            {
                _zoomfactor+=2;
                Invalidate();
            }*/
        }
        
        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }

        private Color GetGraphColor(string symbolname)
        {
            Color retval = Color.White;
            symbolname = symbolname.ToUpper();
            if (symbolname == "BIL_HAST") retval = Color.LightGreen;
            else if (symbolname == "P_MANIFOLD") retval = Color.Red;
            else if (symbolname == "P_MANIFOLD10") retval = Color.Red;
            else if (symbolname == "LUFTTEMP") retval = Color.LightBlue;
            else if (symbolname == "KYL_TEMP") retval = Color.LightGray;
            else if (symbolname == "AD_SOND") retval = Color.Yellow;
            else if (symbolname == "AD_EGR") retval = Color.GreenYellow;
            else if (symbolname == "RPM") retval = Color.Gold;
            else if (symbolname == "INSPTID_MS10") retval = Color.Firebrick;
            else if (symbolname == "GEAR") retval = Color.Purple;
            else if (symbolname == "APC_DECRESE") retval = Color.LightPink;
            else if (symbolname == "IGN_ANGLE") retval = Color.LightSeaGreen;
            else if (symbolname == "P_FAK") retval = Color.LightYellow;
            else if (symbolname == "I_FAK") retval = Color.LightSteelBlue;
            else if (symbolname == "D_FAK") retval = Color.AntiqueWhite;
            else if (symbolname == "REGL_TRYCK") retval = Color.RosyBrown;
            else if (symbolname == "MAX_TRYCK") retval = Color.Pink;
            else if (symbolname == "PWM_UT10") retval = Color.PaleGreen;
            else if (symbolname == "REG_KON_APC") retval = Color.PapayaWhip;
            else if (symbolname == "MEDELTROT") retval = Color.SpringGreen;
            else if (symbolname == "TORT_MIN") retval = Color.Silver;
            else if (symbolname == "KNOCK_MAP_OFFSET") retval = Color.DarkTurquoise;
            else if (symbolname == "KNOCK_OFFSETT1234") retval = Color.Aqua;
            else if (symbolname == "KNOCK_AVERAGE") retval = Color.Orange;
            else if (symbolname == "KNOCK_AVERAGE_LIMIT") retval = Color.OliveDrab;
            else if (symbolname == "KNOCK_LEVEL") retval = Color.OrangeRed;
            else if (symbolname == "KNOCK_MAP_LIM") retval = Color.Navy;
            else if (symbolname == "KNOCK_LIM") retval = Color.Moccasin;
            else if (symbolname == "KNOCK_REF_LEVEL") retval = Color.SeaShell;
            else if (symbolname == "LKNOCK_OREF_LEVEL") retval = Color.MistyRose;
            else if (symbolname == "SPIK_COUNT") retval = Color.Brown;
            else if (symbolname == "KNOCK_DIAG_LEVEL") retval = Color.Chartreuse;// (groter maken)
            else if (symbolname == "KNOCK_ANG_DEC!") retval = Color.DarkGoldenrod;
            return retval;
        }

        private string GetGraphName(string symbolname)
        {
            string retval = symbolname;
            symbolname = symbolname.ToUpper();
            if (symbolname == "BIL_HAST") retval = "Speed";
            else if (symbolname == "P_MANIFOLD") retval = "Boost";
            else if (symbolname == "P_MANIFOLD10") retval = "Boost";
            else if (symbolname == "LUFTTEMP") retval = "IAT";
            else if (symbolname == "KYL_TEMP") retval = "Coolant";
            else if (symbolname == "AD_SOND") retval = "Lambda A/D";
            else if (symbolname == "RPM") retval = "Rpm";
            else if (symbolname == "INSPTID_MS10") retval = "Inj.dur";
            else if (symbolname == "GEAR") retval = "Gear";
            else if (symbolname == "APC_DECRESE") retval = "APCD";
            else if (symbolname == "IGN_ANGLE") retval = "Ign.angle";
            else if (symbolname == "P_FAK") retval = "P factor";
            else if (symbolname == "I_FAK") retval = "I factor";
            else if (symbolname == "D_FAK") retval = "D factor";
            else if (symbolname == "REGL_TRYCK") retval = "Target boost";
            else if (symbolname == "MAX_TRYCK") retval = "Max boost";
            else if (symbolname == "PWM_UT10") retval = "APC PWM";
            else if (symbolname == "REG_KON_APC") retval = "Reg. value";
            else if (symbolname == "MEDELTROT") retval = "TPS";
            else if (symbolname == "TROT_MIN") retval = "TPS offset";
            else if (symbolname == "KNOCK_MAP_OFFSET") retval = "Map offset";
            else if (symbolname == "KNOCK_OFFSET1234") retval = "Offset1234";
            else if (symbolname == "KNOCK_AVERAGE") retval = "Average";
            else if (symbolname == "KNOCK_AVERAGE_LIMIT") retval = "Average limit";
            else if (symbolname == "KNOCK_LEVEL") retval = "Level";
            else if (symbolname == "KNOCK_MAP_LIM") retval = "Ign.map limit";
            else if (symbolname == "KNOCK_LIM") retval = "Map limit";
            else if (symbolname == "KNOCK_REF_LEVEL") retval = "Ref. level";
            else if (symbolname == "LKNOCK_OREF_LEVEL") retval = "ORef level";
            else if (symbolname == "SPIK_COUNT") retval = "Spik";
            else if (symbolname == "KNOCK_DIAG_LEVEL") retval = "Diag level";// (groter maken)
            else if (symbolname == "KNOCK_ANG_DEC!") retval = "Ign. decrease";
            return retval;
        }

        public void ImportT5Logfile(string filename)
        {
            frmProgress progress = new frmProgress();
            progress.Show();
            Application.DoEvents();
            if(File.Exists(filename))
            {
                _lines.Clear();
                _zoomfactor = 1;
                FileInfo fi = new FileInfo(filename);
                long bytesread = 0;
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        bytesread += line.Length + 2;
                        progress.SetProgressPercentage((int)(bytesread * 100 / fi.Length));

                        char[] sep = new char[1];
                        sep.SetValue('|', 0);
                        char[] sepequals = new char[1];
                        sepequals.SetValue('=', 0);
                        if (line.Length > 0)
                        {
                            //10/03/2009 07:33:06.187|P_Manifold10=-0.64|Lufttemp=8.00|Rpm=1660.00|Bil_hast=15.20|Ign_angle=28.50|AD_EGR=127.00|AFR=17.86|InjectorDC=2.49|Power=0.00|Torque=0.00|
                            // fetch all values from the line including the timestamp
                            string[] values = line.Split(sep);
                            if (values.Length > 1)
                            {
                                try
                                {
                                    DateTime dt = Convert.ToDateTime(values.GetValue(0));
                                    for (int tel = 1; tel < values.Length; tel++)
                                    {
                                        string valuepart = (string)values.GetValue(tel);
                                        string[] valueparts = valuepart.Split(sepequals);
                                        if (valueparts.Length == 2)
                                        {
                                            string symbol = (string)valueparts.GetValue(0);
                                            string strvalue = (string)valueparts.GetValue(1);
                                            double dval = ConvertToDouble(strvalue);
                                            this.AddMeasurement(GetGraphName(symbol), symbol, dt, (float)dval, 0, 1, GetGraphColor(symbol));
                                        }
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine("Failed to process line: " + line + " : " + E.Message);
                                }
                            }
                        }
                    }
                    foreach (GraphLine _line in _lines)
                    {
                        float _min = 0;
                        float _max = -10000;
                        foreach (GraphMeasurement gm in _line.Measurements)
                        {
                            if (gm.Value > _max) _max = gm.Value;
                            if (gm.Value < _min) _min = gm.Value;
                        }
                        DetermineRange(_line, _min, _max);
                    }
                }
            }
            progress.Close();
        }

        private void DetermineRange(GraphLine _line, float _min, float _max)
        {
            _line.Maxrange = _max * 1.05F;
            _line.Minrange = _min * 1.05F;
            switch (_line.Symbol.ToUpper())
            {
                case "RPM":
                    _line.Minrange = 0;
                    _line.Maxrange = 7000;
                    break;
                case "BIL_HAST":
                    _line.Minrange = 0;
                    _line.Maxrange = 300;
                    break;
                case "P_MANIFOLD":
                case "P_MANIFOLD10":
                case "REGL_TRYCK":
                case "MAX_TRYCK":
                    _line.Minrange = -1;
                    _line.Maxrange = 2;
                    break;
                case "LUFTTEMP":
                    _line.Minrange = -30;
                    _line.Maxrange = 120;
                    break;
                case "KYL_TEMP":
                    _line.Minrange = -30;
                    _line.Maxrange = 120;
                    break;
                case "INSPTID_MS10":
                    _line.Minrange = 0;
                    _line.Maxrange = 50;
                    break;
                case "GEAR":
                    _line.Minrange = -1;
                    _line.Maxrange = 5;
                    break;
                case "APC_DECRESE":
                case "APC_DECREASE":
                    _line.Minrange = 0;
                    _line.Maxrange = 200;
                    break;
                case "IGN_ANGLE":
                    _line.Minrange = -10;
                    _line.Maxrange = 50;
                    break;
                case "PWM_UT10":
                case "REG_KON_APC":
                    _line.Minrange = 0;
                    _line.Maxrange = 100;
                    break;
                case "MODELTROT":
                case "TROT_MIN":
                    _line.Minrange = 0;
                    _line.Maxrange = 255;
                    break;
                case "AFR":
                case "wAFR":
                case "AD_EGR":
                case "AD_SOND":
                    _line.Minrange = 7;
                    _line.Maxrange = 23;
                    break;
            }
        }

        private string DetermineGraphNameByLinesPosition(Rectangle r, float x, float y,out DateTime measurementdt, out float value, out bool _valid)
        {
            
            float _max_dev = 3;
            _valid = false;
            measurementdt = DateTime.Now;
            value = 0;
            GraphLineCollection linesinselection = GetLinesInSelection();
            foreach (GraphLine line in linesinselection)
            {
                if (line.LineVisible)
                {
                    int cnt = 0;
                    foreach (GraphMeasurement measurement in line.Measurements)
                    {
                        if (cnt > 0)
                        {
                            //Console.WriteLine("  measurement " + measurement.Timestamp.ToString("HH:mm:ss") + " " + measurement.Value.ToString("F2"));
                            float sectionwidth = (float)(r.Width - 175) / (float)line.Maxpoints;
                            float x1 = (float)(cnt - 1) * sectionwidth + 100;
                            float x2 = (float)cnt * sectionwidth + 100;
                            float y1 = (r.Height - 100) - (line.Measurements[cnt - 1].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
                            float y2 = (r.Height - 100) - (line.Measurements[cnt].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
                            if (Math.Abs(x1 - x) <= _max_dev && Math.Abs(y1 - y) <= _max_dev)
                            {

                                value = line.Measurements[cnt - 1].Value;
                                measurementdt = line.Measurements[cnt - 1].Timestamp;
                                //Console.WriteLine("Match: " + value.ToString() + " dt: " + measurementdt.ToString("dd/MM HH:mm:ss") + " x1: " + x1.ToString() + " y1: " + y1.ToString() + " x2: " + x2.ToString() + " y2: " + y2.ToString());
                                _valid = true;
                                return line.Symbol;
                            }
                            else if (Math.Abs(x2 - x) <= _max_dev && Math.Abs(y2 - y) <= _max_dev)
                            {
                                value = measurement.Value;
                                measurementdt = measurement.Timestamp;
                                _valid = true;
                                //Console.WriteLine("Match: " + value.ToString() + " dt: " + measurementdt.ToString("dd/MM HH:mm:ss") + " x1: " + x1.ToString() + " y1: " + y1.ToString() + " x2: " + x2.ToString() + " y2: " + y2.ToString());
                                return line.Symbol;
                            }
                        }
                        cnt++;
                    }
                }
            }
            return string.Empty;
        }

        private void OnlineGraphControl_MouseMove(object sender, MouseEventArgs e)
        {
            // check whether we're close to one of the lines in the graph
            // or over one of the labels in the legenda
            DateTime measurement = DateTime.Now;
            float value = 0;
            bool _valid = false;
            //Point p = PointToClient(e.Location);
            _selectedSymbol = DetermineGraphNameByLinesPosition(this.Bounds, (float)e.X, (float)e.Y, out measurement, out value, out _valid);
            // only redraw the yaxis
            this.DrawYScale(this.ClientRectangle, this.CreateGraphics());
            if (_selectedSymbol != string.Empty && _valid)
            {

                //toolTip1.Show(_selectedSymbol + "=" + value.ToString("F2") + " at " + measurement.ToString("dd/MM/yyyy HH:mm:ss"), this);
                // update the labels that reflect the current values for all lines
                label1.Text = _selectedSymbol + "=" + value.ToString("F2") + " at " + measurement.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
            {
                label1.Text = "";
            }
//            Invalidate();
            
            
        }

        private void OnlineGraphControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnlineGraphControl_MouseDown(object sender, MouseEventArgs e)
        {
            _panning = true;
            Point p = PointToClient(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                if (e.Location.X > this.ClientRectangle.Width - 75)
                {
                }
                // anders misschien inzoomen
                else if (e.Location.Y > this.ClientRectangle.Height - 100)
                {
                }
                else
                {
                    // pan left or right depending on position on graph
                    if (e.Location.X > this.ClientRectangle.Width / 2)
                    {
                        // pan right
                        _pantype = PanType.PanRight;
                        PanToRight();
                    }
                    else
                    {
                        // pan left
                        _pantype = PanType.PanLeft;
                        PanToLeft();
                    }
                }
            }
        }

        private void OnlineGraphControl_MouseUp(object sender, MouseEventArgs e)
        {
            _panning = false;
        }

        private void OnlineGraphControl_MouseLeave(object sender, EventArgs e)
        {
            _panning = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_panning)
            {
                if (_pantype == PanType.PanLeft)
                {
                    if (!PanToLeft())
                    {
                        _panning = false;
                    }
                }
                else
                {
                    if (!PanToRight())
                    {
                        _panning = false;
                    }
                }
            }
        }

        
    }
}