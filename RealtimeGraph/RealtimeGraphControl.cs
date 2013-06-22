/*
 * TODO: 
 * DONE Zoom op datum click weghalen
 * DONE Initieel maximaal 5 minuten tonen indien de log langer is (vanaf begin van de log)
 * "Automatisch" bestanden splitsen (gebruiker selectie laten maken indien er gaten zijn van > 5 minuten, overige gaten aanvullen met inteval = 1 seconde)
 * DONE BETER MAKEN Info over huidige muis "tijd" tonen in de legenda (actuele geselecteerde waarde)
 * Channel selectie uitschakelen (selectie vooraf zoals in de export naar LogWorks)
 * Wellicht optie popup menu bij rechtsklik in legenda op kanaal of in grafiek (detectie grafiek op basis van huidige detectie waarde)
 * */

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
using System.Diagnostics;


namespace RealtimeGraph
{
    public enum PanType : int
    {
        PanLeft,
        PanRight
    }

    public partial class RealtimeGraphControl : UserControl
    {
        public delegate void GraphPainted(object sender, EventArgs e);

        private GraphLineCollection _lines = new GraphLineCollection();
        public event GraphPainted onGraphPainted;

        private string _contextDescription = string.Empty;
        private string _selectedSymbol = string.Empty;
        private bool _panning = false;
        private int _LegendLeftOffset = 75;

        public bool Panning
        {
            get { return _panning; }
            set { _panning = value; }
        }

        private float _maxInitialMinutes = 3;

        private int _maxMinutesToShowDetails = 5;

        public int MaxMinutesToShowDetails
        {
            get { return _maxMinutesToShowDetails; }
            set { _maxMinutesToShowDetails = value; }
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
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;

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
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;

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

        private int _maxZoomFactor = 100;

        public int MaxZoomFactor
        {
            get { return _maxZoomFactor; }
            set { _maxZoomFactor = value; }
        }

        private bool m_allowconfig = false;

        public bool Allowconfig
        {
            get { return m_allowconfig; }
            set { m_allowconfig = value; }
        }

        public RealtimeGraphControl()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(RealtimeGraphControl_MouseWheel);
        }

        void RealtimeGraphControl_MouseWheel(object sender, MouseEventArgs e)
        {
            float _oriZoomFactor = _zoomfactor;
            _zoomfactor += (float)e.Delta/40;
            if (_zoomfactor < 1) _zoomfactor = 1;
            if (_zoomfactor > _maxZoomFactor) _zoomfactor = _maxZoomFactor;
            if (_oriZoomFactor != _zoomfactor)
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _centerDateTime = GetCenterDateTimeByMouseClick(e.X, e.Y);
                Cursor.Position = GetCenterOfControl();
                Invalidate();
            }
        }

        /*LogFilterCollection _filters = new LogFilterCollection();

        public void SetFilters(LogFilterCollection filters)
        {
            _filters = filters;
        }*/


        private Point GetCenterOfControl()
        {
            Point p = new Point();
            p.X = this.Left + this.Width / 2;
            p.Y = this.Top + this.Height / 2;
            return PointToScreen(p);
        }

        public void AddMeasurement(string Graphname, string SymbolName, DateTime Timestamp, float value, float minrange, float maxrange, Color linecolor)
        {
            bool _linefound = false;
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
                _newline.NumberOfDecimals = GetChannelResolution(SymbolName);
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

            using (RegistryKey Settings = TempKey.CreateSubKey("T7SuitePro\\Channels"))
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

        /*private void WriteLastestValuesToLog()
        {
            DateTime datet = DateTime.Now;

            string logline = datet.ToString("dd/MM/yyyy HH:mm:ss") + "." + datet.Millisecond.ToString("D3") + "|";
            foreach (GraphLine line in _lines)
            {
                logline += line.Symbol + "=" + line.Measurements[line.Measurements.Count - 1].Value.ToString("F2") + "|";
            }
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "-"+ _contextDescription + ".t5l", true))
            {
                sw.WriteLine(logline);
            }
        }*/

        public void ForceRepaint()
        {
            _selectionDetermined = false;
            _minDateTimeDone = false;
            _maxDateTimeDone = false;

            this.Invalidate();
            // all new values, write them to a logfile (5tl)
           // WriteLastestValuesToLog();
        }

        private void RealtimeGraphControl_Paint(object sender, PaintEventArgs e)
        {
            // paint the graphs in the control
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

//            Console.WriteLine("this: " + this.Bounds.ToString() + " cliprect: " + e.ClipRectangle.ToString() + " client: " + this.ClientRectangle.ToString());

            this.DrawBackground(this.ClientRectangle, e.Graphics);
            this.DrawYScale(this.ClientRectangle, e.Graphics);
            this.DrawYLines(this.ClientRectangle, e.Graphics);
            this.DrawXScale(this.ClientRectangle, e.Graphics);
            this.DrawLines(this.ClientRectangle, e.Graphics);
            this.DrawLabels(this.ClientRectangle, e.Graphics);
            this.DrawInfo(this.ClientRectangle, e.Graphics);
            // e.Clipractangle
            if (onGraphPainted != null)
            {
                onGraphPainted(this, EventArgs.Empty);
            }
            //Console.WriteLine("Zoomfactor: " + _zoomfactor.ToString());
            //Console.WriteLine("Centerdate: " + _centerDateTime.ToString());
            //Console.WriteLine("Timespan: " + GetDurationOfGraph().TotalMilliseconds.ToString());
        }

        private void DrawGrid(Rectangle r, Graphics graphics)
        {
            int numberofsections = 20;
            int numberofscales = 10;
            Color c = Color.FromArgb(175, Color.DimGray);
            Pen p = new Pen(c);
            float sectionwidth = (float)(r.Width - 175) / numberofsections;
            float scaleheight = (float)(r.Height - 100) / numberofscales;
            for (int xcount = 1; xcount < numberofsections; xcount++)
            {
                graphics.DrawLine(p, xcount * sectionwidth + 100, 0, xcount * sectionwidth + 100, r.Height- 100);
            }
            for (int ycount = 1; ycount < numberofscales; ycount++)
            {
                graphics.DrawLine(p, 100, ycount * scaleheight, r.Width - 75, ycount * scaleheight);
            }
            p.Dispose();
        }

        private bool _minDateTimeDone = false;
        private DateTime _minDateTime;

        private DateTime GetMinDateTime()
        {
            if (_minDateTimeDone) return _minDateTime;

            DateTime retval = DateTime.Now.AddDays(1);
            _minDateTime = retval;
            foreach (GraphLine _line in _lines)
            {
                foreach (GraphMeasurement gm in _line.Measurements)
                {
                    if (gm.Timestamp < retval) retval = gm.Timestamp;
                }
            }
            _minDateTime = retval;
            _minDateTimeDone = true;
            return retval;
        }

        private bool _maxDateTimeDone = false;
        private DateTime _maxDateTime;
        private DateTime GetMaxDateTime()
        {
            if (_maxDateTimeDone) return _maxDateTime;
            DateTime retval = DateTime.MinValue;
            _maxDateTime = retval;
            foreach (GraphLine _line in _lines)
            {
                foreach (GraphMeasurement gm in _line.Measurements)
                {
                    if (gm.Timestamp > retval) retval = gm.Timestamp;
                }
            }
            if (retval == DateTime.MinValue) retval = DateTime.Now;
            _maxDateTime = retval;
            _maxDateTimeDone = true;
            return retval;
        }

        private TimeSpan GetDurationOfGraph()
        {
            TimeSpan ts = new TimeSpan(_maxdt.Ticks - _mindt.Ticks);
            return ts;
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
                if (_mindt < GetMinDateTime()) _mindt = GetMinDateTime();
                if (_maxdt > GetMaxDateTime()) _maxdt = GetMaxDateTime();
            }
            ts = new TimeSpan(_maxdt.Ticks - _mindt.Ticks);
            Font f = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
            Pen pen =  new Pen(Color.FromArgb(100, Color.Wheat));
            // total number of labels depends on with of the drawing surface
            SizeF stringsize = graphics.MeasureString("00/00 00:00:00", f);
            int numberoflabels = (r.Width - 100)/ ((int)stringsize.Width + 10);
            //Console.WriteLine("dates: " + _mindt.ToString("dd/MM HH:mm:ss") + " - " + _maxdt.ToString("dd/MM HH:mm:ss") + " #labels: " + numberoflabels.ToString());
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

        private void RedrawLabels()
        {
            Graphics g = this.CreateGraphics();
            ClearLabels(this.Bounds, g);
            DrawLabels(this.Bounds, g);
        }

        private void ClearLabels(Rectangle r, Graphics graphics)
        {
            graphics.FillRectangle(Brushes.Black, r.Width - _LegendLeftOffset, 0, _LegendLeftOffset, r.Height - 100);
        }


        private void DrawInfo(Rectangle r, Graphics graphics)
        {

        }

        private void DrawLabels(Rectangle r, Graphics graphics)
        {
            SolidBrush brsh = new SolidBrush(Color.Red);
            Font f = new Font("Tahoma", 7);
            int cnt = 0;

            foreach (GraphLine line in _lines)
            {
                if (line.LineVisible)
                {
                    brsh.Color = line.LineColor;
                    PointF pntF = new PointF((float)(r.Width - _LegendLeftOffset), (float)((cnt + 1) * 20));
                    if (line.CurrentlySelectedValue != float.MinValue)
                    {
                        graphics.DrawString(line.ChannelName + " " + line.CurrentlySelectedValue.ToString("F" + line.NumberOfDecimals.ToString()), f, brsh, pntF);
                    }
                    else
                    {
                        graphics.DrawString(line.ChannelName, f, brsh, pntF);
                    }
                    cnt++;
                }
            }
            // determine scales in Y axis
            f.Dispose();
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
                _newline.NumberOfDecimals = GetChannelResolution(SymbolName);
                _newline.ChannelName = Graphname;
                _newline.Clear();
                coll.Add(_newline);
                _newline.AddPoint(value, Timestamp, minrange, maxrange, linecolor);
                _newline.LineVisible = GetRegistryValue(Graphname);
            }
        }

        private int GetChannelResolution(string symbolname)
        {
            int numberOfDecimals = 0;
            switch (symbolname)
            {
                case "In.p_AirInlet":
                case "InjectorDC":
                case "Out.fi_Ignition":
                case "AFR":
                    numberOfDecimals = 2;
                    break;

            }
            //Console.WriteLine(symbolname);
            return numberOfDecimals;
        }

        GraphLineCollection selcoll = new GraphLineCollection();
        private bool _selectionDetermined = false;

        private GraphLineCollection GetLinesInSelection()
        {
            //GraphLineCollection coll = new GraphLineCollection();
            //Console.WriteLine("date selection: " + _mindt.ToString("dd/MM HH:mm:ss") + " - " + _maxdt.ToString("dd/MM HH:mm:ss"));
            if (_selectionDetermined) return selcoll;
            selcoll = new GraphLineCollection();

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
                            AddMeasurementToCollection(selcoll, _line.ChannelName, _line.Symbol, gm.Timestamp, gm.Value, _line.Minrange, _line.Maxrange, _line.LineColor);
                        }
                    }
                }
                if (!_measurementfound)
                {
                    if(!(PanToLeft())) _measurementfound = true;
                }
            }
            _selectionDetermined = true;
            return selcoll;
        }


        private void DrawLines(Rectangle r, Graphics graphics)
        {
            Pen p = new Pen(Color.Red);
            SolidBrush sb = new SolidBrush(Color.Red);
            GraphLineCollection displaylines = GetLinesInSelection();
            foreach (GraphLine line in /*_lines*/ displaylines)
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
                            float sectionwidth = (float)(r.Width - 175) / (float)line.Maxpoints;
                            //Console.WriteLine("Sectionwidth: " + sectionwidth.ToString());
                            //float x1 = (float)(cnt - 1) * sectionwidth + 100;
                            //float x2 = (float)cnt * sectionwidth + 100;
                            float totalticks = _maxdt.Ticks - _mindt.Ticks;
                            float x1 = ((line.Measurements[cnt - 1].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;
                            float x2 = ((line.Measurements[cnt].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;

                            float y1 = (r.Height - 100) - (line.Measurements[cnt - 1].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
                            float y2 = (r.Height - 100) - (line.Measurements[cnt].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
//                            Console.WriteLine("Line with value from : " + line.Measurements[cnt -1].Value.ToString() + " and to: " + line.Measurements[cnt].Value.ToString() + " x1: " + x1.ToString() + " y1: " + y1.ToString() + " x2: " + x2.ToString() + " y2: " + y2.ToString());
                            p.Color = line.LineColor;
                            sb.Color = line.LineColor;
                            if (float.IsInfinity(y1)) y1 = 0;
                            if (float.IsNegativeInfinity(y1)) y1 = 0;
                            if (float.IsInfinity(y2)) y2 = 0;
                            if (float.IsNegativeInfinity(y2)) y2 = 0;
                            graphics.DrawLine(p, x1, y1, x2, y2);

                            //graphics.FillEllipse(sb, x2-2, y2-2, 4, 4);
                            //graphics.FillEllipse(sb, x1 - 2, y1 - 2, 4, 4); 
                        }
                        cnt++;
                    }
                }
            }
            p.Dispose();
            sb.Dispose();
        }

        private void DrawBackground(Rectangle r, Graphics graphics)
        {
            SolidBrush sbback = new SolidBrush(this.BackColor);
            graphics.FillRectangle(sbback, r.X + 100, r.Y, r.Width - 100, r.Height);
            sbback.Dispose();
        }

        private void RealtimeGraphControl_Click(object sender, EventArgs e)
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

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T7SuitePro\\Channels"))
            {
                saveSettings.SetValue(key.ToUpper(), value);
            }
        }


        private void RealtimeGraphControl_MouseClick(object sender, MouseEventArgs e)
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
                    _selectionDetermined = false;
                    _minDateTimeDone = false;
                    _maxDateTimeDone = false;
                    _centerDateTime = GetCenterDateTimeByMouseClick(e.Location.X, e.Location.Y);
                    Invalidate(); // <GS-21012010> 
                    //<GS-21012010>  TryToZoomIn();
                }
                else
                {
                    // pan left or right depending on position on graph
                    /*if (e.Location.X > this.ClientRectangle.Width / 2)
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
                    }*/
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // uitzoomen
//                Console.WriteLine("Zooming out");
                //<GS-21012010> _centerDateTime = GetCenterDateTimeByMouseClick(e.Location.X, e.Location.Y);
                //<GS-21012010> TryToZoomOut();
            }
        }

        private DateTime GetCenterDateTimeByMouseClick(int x, int y)
        {
            // relative y position in selected daterange
            _selectionDetermined = false;
            _minDateTimeDone = false;
            _maxDateTimeDone = false;

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
            if (_zoomfactor == 1)
            {
                // kan niet pannen nie
            }
            else
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _centerDateTime = GetPreviousCenterDateTime(out _panned);
                Invalidate();
            }
            return _panned;
        }

        private DateTime GetNextCenterDateTime(out bool _panned)
        {
            _panned = true;
            DateTime _oriDateTime = _centerDateTime;
            // add a percentage depending on zoomfactor
            DateTime maxdt = GetMaxDateTime();
            DateTime mindt = GetMinDateTime();
            TimeSpan ts = new TimeSpan(maxdt.Ticks - mindt.Ticks);
            _centerDateTime = _centerDateTime.AddTicks((long)(ts.Ticks / (_zoomfactor * 10)));
            // also determine new _mindt and _maxdt
            

            if (_centerDateTime > maxdt.AddTicks(-(GetDurationOfGraph().Ticks/2)))
            {
                _centerDateTime = _oriDateTime;//maxdt.AddTicks(-(long)((ts.Ticks / 50)));
                _panned = false;
            }
            return _centerDateTime;
        }

        private bool PanToRight()
        {
            bool _panned = false;
            if (_zoomfactor == 1)
            {
                // kan niet pannen nie
            }
            else
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _centerDateTime = GetNextCenterDateTime(out _panned);
                Invalidate();
            }
            return _panned;
        }

        private DateTime GetPreviousCenterDateTime(out bool _panned)
        {
            // substract a percentage depending on zoomfactor
            _panned = true;
            DateTime _oriDatetime = _centerDateTime;
            DateTime maxdt = GetMaxDateTime();
            DateTime mindt = GetMinDateTime();

            TimeSpan ts = new TimeSpan(maxdt.Ticks - mindt.Ticks);
            _centerDateTime = _centerDateTime.AddTicks(-(long)(ts.Ticks / (_zoomfactor * 10)));
            if (_centerDateTime < mindt.AddTicks(GetDurationOfGraph().Ticks/2))
            {
                _centerDateTime = _oriDatetime;
                //_centerDateTime = mindt.AddTicks((long)ts.Ticks / 50);
                _panned = false;
            }
            return _centerDateTime;

        }

        private void TryToZoomOut()
        {
            if (_zoomfactor == 1)
            {
                
            }
            else if (_zoomfactor <= 1)
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _zoomfactor = 1;
                Invalidate();
            }
            else
            {
                // zoomfactor > 1
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _zoomfactor -= 5;
                Invalidate();
            }

        }

        private void TryToZoomIn()
        {
            if (_zoomfactor == _maxZoomFactor)
            {
                
            }
            else if (_zoomfactor > _maxZoomFactor)
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;

                _zoomfactor = _maxZoomFactor;
                Invalidate();
            }
            else
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;
                _zoomfactor += 5;
                Invalidate();
            }
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

        private Int32 GetValueFromRegistry(string symbolname)
        {
            RegistryKey TempKey = null;
            Int32 win32color = 0;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T7SuitePro\\SymbolColors"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == symbolname)
                            {
                                string value = Settings.GetValue(a).ToString();
                                win32color = Convert.ToInt32(value);
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            return win32color;
        }

        RealtimeSymbolCollection _colorCollection = new RealtimeSymbolCollection();

        private Color GetGraphColor(string symbolname)
        {
            Color retval = Color.White;
            if (_colorCollection.Count == 0)
            {
                RegistryKey TempKey = null;
                TempKey = Registry.CurrentUser.CreateSubKey("Software");
                using (RegistryKey Settings = TempKey.CreateSubKey("T7SuitePro\\SymbolColors"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                // a = symbolname
                                
                                RealtimeSymbolHelper ch = new RealtimeSymbolHelper();
                                ch.Color = ColorTranslator.FromWin32(Convert.ToInt32(Settings.GetValue(a)));
                                ch.Varname = a;
                                _colorCollection.Add(ch);
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine("error retrieving registry settings: " + E.Message);
                            }

                        }
                    }
                }
            }

            foreach (RealtimeSymbolHelper ch in _colorCollection)
            {
                if (ch.Varname == symbolname)
                {
                    return ch.Color;
                }
            }
            // load graphcolors into table and lookup from there

            /*symbolname = symbolname.ToUpper();
            if (symbolname == "In.v_Vehicle") retval = Color.LightGreen;
            else if (symbolname == "In.p_AirInlet") retval = Color.Red;
            else if (symbolname == "P_MANIFOLD10") retval = Color.Red;
            else if (symbolname == "ActualIn.T_AirInlet") retval = Color.LightBlue;
            else if (symbolname == "ActualIn.T_Engine") retval = Color.LightGray;
            else if (symbolname == "AD_SOND") retval = Color.Yellow;
            else if (symbolname == "AD_EGR") retval = Color.GreenYellow;
            else if (symbolname == "ActualIn.n_Engine") retval = Color.Gold;
            else if (symbolname == "INSPTID_MS10") retval = Color.Firebrick;
            else if (symbolname == "GEAR") retval = Color.Purple;
            else if (symbolname == "APC_DECRESE") retval = Color.LightPink;
            else if (symbolname == "Out.fi_Ignition") retval = Color.LightSeaGreen;
            else if (symbolname == "P_FAK") retval = Color.LightYellow;
            else if (symbolname == "I_FAK") retval = Color.LightSteelBlue;
            else if (symbolname == "D_FAK") retval = Color.AntiqueWhite;
            else if (symbolname == "REGL_TRYCK") retval = Color.RosyBrown;
            else if (symbolname == "MAX_TRYCK") retval = Color.Pink;
            else if (symbolname == "Out.PWM_BoostCntrl") retval = Color.PaleGreen;
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
            else if (symbolname == "KNOCK_ANG_DEC!") retval = Color.DarkGoldenrod;*/
            return retval;
        }

        private string GetGraphName(string symbolname)
        {
            string retval = symbolname;
            symbolname = symbolname.ToUpper();
            if (symbolname == "IN.V_VEHICLE") retval = "Speed";
            else if (symbolname == "ACTUALIN.N_ENGINE") retval = "Rpm";
            else if (symbolname == "IN.P_AIRINLET") retval = "Boost";
            else if (symbolname == "ACTUALIN.T_ENGINE") retval = "Coolant";
            else if (symbolname == "ACTUALIN.T_AIRINLET") retval = "IAT";
            else if (symbolname == "ECMSTAT.ST_ACTIVEAIRDEM") retval = "LIMITER";
            else if (symbolname == "IGNPROT.FI_OFFSET") retval = "IOFF";
            else if (symbolname == "M_REQUEST") retval = "Request";
            else if (symbolname == "OUT.M_ENGINE") retval = "Torque";
            else if (symbolname == "ECMSTAT.P_ENGINE") retval = "Power";
            else if (symbolname == "OUT.PWM_BOOSTCNTRL") retval = "APC PWM";
            else if (symbolname == "OUT.FI_IGNITION") retval = "Ign.angle";
            else if (symbolname == "OUT.X_ACCPEDAL") retval = "TPS";
            else if (symbolname == "MAF.M_AIRINLET") retval = "Airmass";
            else if (symbolname == "EXHAUST.T_CALC") retval = "EGT";
            else if (symbolname == "DisplProt.LambdaScanner") retval = "WBLambda";
            else if (symbolname == "Lambda.LambdaInt") retval = "NBLambda";
            return retval;
        }

        public void ImportT5Logfile(string filename)
        {
            frmRealtimeProgress progress = new frmRealtimeProgress();
            progress.Show();
            Application.DoEvents();
            DateTime _dtFrom = DateTime.MinValue;
            DateTime _dtUpto = DateTime.MinValue;
            if(File.Exists(filename))
            {
                //analyse file first
                LogSectionCollection logsections = AnalyseFile(filename);
                foreach (LogSection sc in logsections)
                {
                    Console.WriteLine("section: " + sc.StartDateTime.ToString() + " - " + sc.EndDateTime.ToString());
                }
                if (logsections.Count > 1)
                {
                    // have the user select the desired section
                    progress.Hide();
                    Application.DoEvents();
                    frmSectionSelection secsel = new frmSectionSelection();
                    secsel.LogSections = logsections;
                    if (secsel.ShowDialog() == DialogResult.OK && secsel.Valid)
                    {
                        // dummy
                        _dtFrom = secsel.SelectedStart;
                        _dtUpto = secsel.SelectedEnd;
                    }
                    else
                    {
                        progress.Close();
                        return;
                    }
                    progress.Show();
                    Application.DoEvents();
                }
                else
                {
                    _dtFrom = logsections[0].StartDateTime;
                    _dtUpto = logsections[0].EndDateTime;
                }
                _lines.Clear();
                _zoomfactor = 1;
                FileInfo fi = new FileInfo(filename);
                long bytesread = 0;
                string[] alllines = File.ReadAllLines(filename);
                //using (StreamReader sr = new StreamReader(filename))
                {
                    //string line = string.Empty;
                    //while ((line = sr.ReadLine()) != null)
                    foreach(string line in alllines)
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
                                    string dtstring = (string)values.GetValue(0);
                                    DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                    if (dtstring.Length > 20)
                                    {
                                        dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)), Convert.ToInt32(dtstring.Substring(20, 3)));
                                    }
                                    if (dt >= _dtFrom && dt <= _dtUpto)
                                    {
                                        //DateTime dt = Convert.ToDateTime(values.GetValue(0));
                                        //if (_previousDateTime == DateTime.MinValue) _previousDateTime = dt; // sync
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
            //<GS-
            //_mindt = GetMinDateTime();
            //_mindt = GetMinDateTime();
            //_maxdt = _mindt.AddSeconds(GetDurationOfGraph().TotalSeconds / 10);
            //_maxdt = GetMinDateTime().AddSeconds(
            _mindt = GetMinDateTime();
            _maxdt = GetMaxDateTime();
            if (GetDurationOfGraph().TotalMinutes > _maxInitialMinutes)
            {
                _centerDateTime = GetMinDateTime().AddMinutes(_maxInitialMinutes/2);
                _zoomfactor = CalculateZoomFactorForMinutes(_maxInitialMinutes);
            }
            progress.Close();
            Invalidate();
            
        }

        private LogSectionCollection AnalyseFile(string filename)
        {
            LogSectionCollection lsc = new LogSectionCollection();
            DateTime _previousDateTime = DateTime.MinValue;
            DateTime _sectionStart = DateTime.MinValue;
            DateTime _sectionEnd = DateTime.MinValue;
            string[] alllines = File.ReadAllLines(filename);
            //using (StreamReader sr = new StreamReader(filename))
            {
                //string line = string.Empty;
                //while ((line = sr.ReadLine()) != null)
                foreach(string line in alllines)
                {
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
                                string dtstring = (string)values.GetValue(0);
                                DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                if (dtstring.Length > 20)
                                {
                                    dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)), Convert.ToInt32(dtstring.Substring(20, 3)));
                                }

                                //DateTime dt = Convert.ToDateTime(values.GetValue(0));
                                if (_previousDateTime == DateTime.MinValue)
                                {
                                    _previousDateTime = dt; // sync
                                }

                                TimeSpan ts = new TimeSpan(dt.Ticks - _previousDateTime.Ticks);
                                if (_sectionStart == DateTime.MinValue)
                                {
                                   // at least one section
                                    _sectionStart = dt;
                                    _sectionEnd = dt; // initial

                                }
                                if (ts.TotalSeconds < 10)
                                {
                                    _sectionEnd = dt;
                                }
                                else
                                {
                                    LogSection ls = new LogSection();
                                    ls.StartDateTime = _sectionStart;
                                    ls.EndDateTime = _sectionEnd;
                                    ls.Duration = new TimeSpan(ls.EndDateTime.Ticks - ls.StartDateTime.Ticks);
                                    lsc.Add(ls);
                                    // reset probes
                                    //_previousDateTime = _sectionEnd;
                                    _previousDateTime = DateTime.MinValue;
                                    _sectionStart = DateTime.MinValue;
                                    _sectionEnd = DateTime.MinValue;
                                }
                                _previousDateTime = dt;

                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }

                        }
                    }
                }
            }
            if (_sectionStart != DateTime.MinValue && _sectionEnd != DateTime.MinValue)
            {
                LogSection ls = new LogSection();
                ls.StartDateTime = _sectionStart;
                ls.EndDateTime = _sectionEnd;
                ls.Duration = new TimeSpan(ls.EndDateTime.Ticks - ls.StartDateTime.Ticks);
                lsc.Add(ls);
            }
            return lsc;
        }

        private float CalculateZoomFactorForMinutes(float NumberOfMinutes)
        {
            float zoom_factor = 1;
            long ticksFromMinutes = (long)NumberOfMinutes * 60 * 10000000;
            // total timespan for graph
            long ticksInGraph = GetMaxDateTime().Ticks - GetMinDateTime().Ticks;
            zoom_factor = ticksInGraph / ticksFromMinutes;
            return zoom_factor;
        }

        private void DetermineRange(GraphLine _line, float _min, float _max)
        {
            _line.Maxrange = _max * 1.05F;
            _line.Minrange = _min * 1.05F;
            switch (_line.ChannelName)
            {
                case "DisplProt.LambdaScanner": // AFR through wideband?
                case "Lambda.LambdaInt": // AFR through narrowband?
                    _line.Minrange = 0.5F;
                    _line.Maxrange = 1.5F;
                    break;
                case "ActualIn.n_Engine":
                    _line.Minrange = 0;
                    _line.Maxrange = 7000;
                    break;
                case "In.v_Vehicle":
                    _line.Minrange = 0;
                    _line.Maxrange = 300;
                    break;
                case "In.p_AirInlet":
                    _line.Minrange = -1;
                    _line.Maxrange = 2;
                    break;
                case "ActualIn.T_AirInlet":
                    _line.Minrange = -30;
                    _line.Maxrange = 120;
                    break;
                case "ActualIn.T_Engine":
                    _line.Minrange = -30;
                    _line.Maxrange = 120;
                    break;
                case "Out.fi_Ignition":
                    _line.Minrange = -10;
                    _line.Maxrange = 50;
                    break;
                case "Out.PWM_BoostCntrl":
                case "REG_KON_APC":
                    _line.Minrange = 0;
                    _line.Maxrange = 100;
                    break;
                case "Out.X_AccPedal":
                    _line.Minrange = 0;
                    _line.Maxrange = 100;
                    break;
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            // paint the graphs in the control
            

            //this.DrawBackground(e.ClipRectangle, e.Graphics);
            //this.DrawGrid(e.ClipRectangle, e.Graphics);
            //this.DrawLines(e.ClipRectangle, e.Graphics);
            //Console.WriteLine("Done painting panel2");
            //this.DrawLabels(e.Graphics);
            /*foreach(GraphLine line in _lines)
            {
                Console.WriteLine(line.Symbol + " contains: " + line.Numberofmeasurements.ToString());
                foreach (GraphMeasurement measurement in line.Measurements)
                {
                    Console.WriteLine("  measurement " + measurement.Timestamp.ToString("HH:mm:ss") + " " + measurement.Value.ToString("F2"));
                }
            }*/
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
                            //float x1 = (float)(cnt - 1) * sectionwidth + 100;
                            //float x2 = (float)cnt * sectionwidth + 100;
                            float totalticks = _maxdt.Ticks - _mindt.Ticks;
                            float x1 = ((line.Measurements[cnt - 1].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;
                            float x2 = ((line.Measurements[cnt].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;

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

        private void DetermineGraphValuesByPosition(Rectangle r, float x, float y)
        {

            GraphLineCollection linesinselection = GetLinesInSelection();
            foreach (GraphLine line in linesinselection)
            {
                float _max_dev = r.Width;
                float value = float.MinValue;
                if (line.LineVisible)
                {
                    int cnt = 0;
                    foreach (GraphMeasurement measurement in line.Measurements)
                    {
                        if (cnt > 0)
                        {
                            //Console.WriteLine("  measurement " + measurement.Timestamp.ToString("HH:mm:ss") + " " + measurement.Value.ToString("F2"));
                            float sectionwidth = (float)(r.Width - 175) / (float)line.Maxpoints;
                            //float x1 = (float)(cnt - 1) * sectionwidth + 100;
                            //float x2 = (float)cnt * sectionwidth + 100;
                            float totalticks = _maxdt.Ticks - _mindt.Ticks;
                            float x1 = ((line.Measurements[cnt - 1].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;
                            float x2 = ((line.Measurements[cnt].Timestamp.Ticks - _mindt.Ticks) * ((float)r.Width - 175) / totalticks) + 100;

                            float y1 = (r.Height - 100) - (line.Measurements[cnt - 1].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
                            float y2 = (r.Height - 100) - (line.Measurements[cnt].Value - line.Minrange) / (line.Maxrange - line.Minrange) * (r.Height - 100);
                            if (Math.Abs(x1 - x) <= _max_dev /*&& Math.Abs(y1 - y) <= _max_dev*/)
                            {
                                _max_dev = Math.Abs(x1 - x);
                                value = line.Measurements[cnt - 1].Value;
                            }
                            if (Math.Abs(x2 - x) <= _max_dev /*&& Math.Abs(y2 - y) <= _max_dev*/)
                            {
                                _max_dev = Math.Abs(x2 - x);
                                value = measurement.Value;
                            }
                        }
                        cnt++;
                    }
                }
                foreach (GraphLine gl in _lines)
                {
                    if (gl.ChannelName == line.ChannelName)
                    {
                        gl.CurrentlySelectedValue = value;
                        break;
                    }
                }
            }
        }

        private void RealtimeGraphControl_MouseMove(object sender, MouseEventArgs e)
        {
            // check whether we're close to one of the lines in the graph
            // or over one of the labels in the legenda

            if (_panning)
            {
                _selectionDetermined = false;
                _minDateTimeDone = false;
                _maxDateTimeDone = false;

                //Console.WriteLine("Dragging for pan");
                int deltaX = _MouseX - e.X;
                _MouseX = e.X;
                
                // calculate time per pixel
                int graphWidth = (this.Bounds.Width - 175);
                double milliSeconds = GetDurationOfGraph().TotalMilliseconds;
                double milliSecondsPerPixel = milliSeconds / (double)graphWidth;
                double milliSecondsToShift = deltaX * milliSecondsPerPixel;
                //Console.WriteLine("Delta: " + deltaX.ToString() + " " + milliSecondsToShift.ToString() + " ms");
                _centerDateTime = _centerDateTime.AddMilliseconds(milliSecondsToShift);

                int deltaY = _MouseY - e.Y;
                if (Math.Abs(deltaY) > 30)
                {
                    _MouseY = e.Y;
                    _zoomfactor += deltaY/10;
                    if (_zoomfactor < 1) _zoomfactor = 1;
                    if (_zoomfactor > _maxZoomFactor) _zoomfactor = _maxZoomFactor;
                }
                

                Invalidate();
            }
            else
            {
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
                    label1.Text = _selectedSymbol + "=" + value.ToString("F2") + " at " + measurement.ToString("dd/MM/yyyy HH:mm:ss:fff");
                }
                else
                {
                    label1.Text = "";
                }
                // <GS-21012010> add function to display ALL values in the legend
                if (GetDurationOfGraph().TotalMinutes < _maxMinutesToShowDetails)
                {
                    DetermineGraphValuesByPosition(this.Bounds, (float)e.X, (float)e.Y);
                    /*DateTime _dtMousePosition = GetCenterDateTimeByMouseClick(e.X, e.Y);
                    GraphLineCollection displaylines = GetLinesInSelection();
                    foreach (GraphLine line in displaylines)
                    {
                        if (line.LineVisible)
                        {
                            long _diff = long.MaxValue;
                            float _currValue = float.MinValue;
                            foreach (GraphMeasurement curmeasurement in line.Measurements)
                            {
                                // get closest measurement to dtmouseposition
                                if (Math.Abs(curmeasurement.Timestamp.Ticks - _dtMousePosition.Ticks) < _diff)
                                {
                                    _diff = Math.Abs(curmeasurement.Timestamp.Ticks - _dtMousePosition.Ticks);
                                    _currValue = curmeasurement.Value;
                                }

                            }
                            foreach (GraphLine gl in _lines)
                            {
                                if (gl.ChannelName == line.ChannelName)
                                {
                                    gl.CurrentlySelectedValue = _currValue;
                                    break;
                                }
                            }
                        }
                    }
                    */
                    RedrawLabels();
                }

            }
//            Invalidate();
            
            
        }

        private int _MouseX;
        private int _MouseY;

        private void RealtimeGraphControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void RealtimeGraphControl_MouseDown(object sender, MouseEventArgs e)
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
                    //Cursor = new Cursor(
                    Cursor = Cursors.Hand;
                    _panning = true;
                    _MouseX = e.X;
                    _MouseY = e.Y;
                    /*
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
                    }*/
                }
            }
        }

        private void RealtimeGraphControl_MouseUp(object sender, MouseEventArgs e)
        {
            _panning = false;
            Cursor = Cursors.Default;
        }

        private void RealtimeGraphControl_MouseLeave(object sender, EventArgs e)
        {
            _panning = false;
            Cursor = Cursors.Default;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*if (_panning)
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
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
                // pan left
            PanToLeft();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PanToRight();
        }

        
    }
}