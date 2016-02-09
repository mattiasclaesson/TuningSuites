using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace CommonSuite
{
    public class CSVGenerator
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void ExportProgress(object sender, ProgressEventArgs e);
        public event CSVGenerator.ExportProgress onExportProgress;

        private AppSettings m_appSettings;

        public AppSettings AppSettings
        {
            get { return m_appSettings; }
            set { m_appSettings = value; }
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

        private bool _useWidebandInput = false;

        public bool UseWidebandInput
        {
            get { return _useWidebandInput; }
            set { _useWidebandInput = value; }
        }

        private double _maximumVoltageWideband = 5;

        public double MaximumVoltageWideband
        {
            get { return _maximumVoltageWideband; }
            set { _maximumVoltageWideband = value; }
        }

        private double _minimumVoltageWideband = 0;

        public double MinimumVoltageWideband
        {
            get { return _minimumVoltageWideband; }
            set { _minimumVoltageWideband = value; }
        }

        private string _widebandSymbol = "AD_EGR";

        public string WidebandSymbol
        {
            get { return _widebandSymbol; }
            set { _widebandSymbol = value; }
        }

        private double _highAFR = 20;

        public double HighAFR
        {
            get { return _highAFR; }
            set { _highAFR = value; }
        }
        private double _lowAFR = 10;

        public double LowAFR
        {
            get { return _lowAFR; }
            set { _lowAFR = value; }
        }

        private double ConvertToWidebandLambda(double value)
        {
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double retval = 0;
            double voltage = ((value) / 1023) * (m_appSettings.WidebandHighVoltage / 1000 - m_appSettings.WidebandLowVoltage / 1000);
            //logger.Debug("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_appSettings.WidebandLowVoltage / 1000) voltage = m_appSettings.WidebandLowVoltage / 1000;
            if (voltage > m_appSettings.WidebandHighVoltage / 1000) voltage = m_appSettings.WidebandHighVoltage / 1000;
            //logger.Debug("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_appSettings.WidebandHighAFR / 1000) - (m_appSettings.WidebandLowAFR / 1000)) / ((m_appSettings.WidebandHighVoltage / 1000) - (m_appSettings.WidebandLowVoltage / 1000));
            //logger.Debug("Steepness: " + steepness.ToString());
            retval = (m_appSettings.WidebandLowAFR / 1000) + (steepness * (voltage - (m_appSettings.WidebandLowVoltage / 1000)));
            //logger.Debug("retval: " + retval.ToString());
            return retval;

        }

        LogFilterCollection _filters = new LogFilterCollection();

        public void SetFilters(LogFilterCollection filters)
        {
            _filters = filters;
        }


        private double ConvertToLambda(double value)
        {
            // converteer naar lambda waarden
            // 23 = lambda = 1
            // daarboven = rijk (lambda<1)
            // daaronder = arm  (lambda>1)
            value = Math.Abs(value - 125); // value - 125 means range -125 to -75 
            // abs value means range 125 to 75 in which 125 = lean and 75 = rich
            value /= 100;
            return (value); 

        }


        public bool ConvertFileToCSV(string filename, SymbolCollection sc, DateTime startDate, DateTime endDate)
        {
            bool retval = false;
            string newfilename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".csv";

            CastExportProgressEvent(1);
            if (File.Exists(newfilename)) File.Delete(newfilename);
            using (StreamWriter sw = new StreamWriter(newfilename, false))
            {
                FileInfo fi = new FileInfo(filename);
                string[] alllines = File.ReadAllLines(filename);
                List<string> header = new List<string>(50);
                HashSet<string> headerHash = new HashSet<string>();
                sw.Write("Time");
                {
                    string previousline = string.Empty;
                    char[] sep = new char[1];
                    char[] sep2 = new char[1];
                    int linecount = 0;
                    sep.SetValue('|', 0);
                    sep2.SetValue('=', 0);
                    DateTime dtpreviousline = DateTime.MaxValue;

                    // find all symbolnames to write header 
                    foreach (string line in alllines)
                    {
                        string[] values = line.Split(sep);
                        if (values.Length > 0 && CheckValuesAgainstFilters(values))
                        {
                            for (int t = 1; t < values.Length; t++)
                            {
                                string subvalue = (string)values.GetValue(t);
                                string[] subvals = subvalue.Split(sep2);
                                if (subvals.Length == 2)
                                {
                                    if (headerHash.Add(subvals[0]))
                                    {
                                        header.Add(subvals[0]);
                                        sw.Write(string.Format(",{0}",subvals[0]));
                                    }
                                }
                            }
                        }
                    }
                    sw.WriteLine();
                    
                    // find all values
                    foreach (string line in alllines) 
                    {
                        linecount+= line.Length;
                        int percentage = (linecount * 90) / (int)fi.Length;
                        CastExportProgressEvent(10 + percentage);

                        string[] values = line.Split(sep);
                        if (values.Length > 0 && CheckValuesAgainstFilters(values))
                        {
                            try
                            {
                                retval = true;
                                string dtstring = (string)values.GetValue(0);
                                DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                if (dtstring.Length > 20)
                                {
                                    dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)), Convert.ToInt32(dtstring.Substring(20, 3)));
                                }
                                if (dt >= startDate && dt <= endDate)
                                {
                                    TimeSpan ts = new TimeSpan(dt.Ticks - startDate.Ticks);
                                    float millisecs = (float)(ts.TotalMilliseconds) / 1000F;
                                    sw.Write(millisecs.ToString("F4").Replace(',', '.'));
                                    
                                    foreach (string column in header)
                                    {
                                        for (int t = 1; t < values.Length; t++)
                                        {
                                            string subvalue = (string)values.GetValue(t);
                                            string[] subvals = subvalue.Split(sep2);
                                            if (subvals.Length == 2)
                                            {
                                                string varname = (string)subvals.GetValue(0);

                                                if (column == varname)
                                                {
                                                    if (varname == "AD_sond")
                                                    {
                                                        double value = ConvertToDouble(subvals[1]);
                                                        value = ConvertToLambda(value);
                                                        sw.Write(string.Format(",{0}", value));
                                                    }
                                                    else if (varname == _widebandSymbol || varname == "DisplProt.AD_Scanner")
                                                    {
                                                        double value = ConvertToDouble(subvals[1]);
                                                        value = ConvertToWidebandLambda(value);
                                                        sw.Write(string.Format(",{0}", value));
                                                    }
                                                    else
                                                    {
                                                        sw.Write(string.Format(",{0}", subvals[1].Replace(",",".")));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    sw.WriteLine();
                                }                                
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                        previousline = line;
                    }
                }
            }
            return retval;
        }

        private bool CheckValuesAgainstFilters(string[] values)
        {
            bool retval = true;
            if (_filters == null) return true;
            if (_filters.Count == 0) return true;
            char[] sep2 = new char[1];
            sep2.SetValue('=', 0);

            for (int t = 1; t < values.Length; t++)
            {
                string subvalue = (string)values.GetValue(t);
                string[] subvals = subvalue.Split(sep2);
                if (subvals.Length == 2)
                {
                    string varname = (string)subvals.GetValue(0);
                    foreach (LogFilter filter in _filters)
                    {
                        if (filter.Symbol == varname && filter.Active)
                        {
                            float value = (float)ConvertToDouble((string)subvals.GetValue(1));
                            if (filter.Type == LogFilter.MathType.Equals)
                            {
                                if (value != filter.Value) retval = false;
                            }
                            else if (filter.Type == LogFilter.MathType.GreaterThan)
                            {
                                if (value < filter.Value) retval = false;
                            }
                            else if (filter.Type == LogFilter.MathType.SmallerThan)
                            {
                                if (value > filter.Value) retval = false;
                            }
                            //logger.Debug("value = " + value.ToString() + " retval = " + retval.ToString());
                        }
                        
                    }
                }
            }
            return retval;
        }

        private int ConvertToColor(string symbolname)
        {
            int retval = 0;

            
            for (int i = 0; i < symbolname.Length; i++)
            {
                retval += Convert.ToInt32(symbolname[i]);
            }
            retval *= symbolname.Length * 256;
            return retval;
        }

        private void CastExportProgressEvent(int percentage)
        {
            if (onExportProgress != null)
            {
                onExportProgress(this, new ProgressEventArgs(percentage));
            }
        }


        public class ProgressEventArgs : System.EventArgs
        {
            private int _percentage;

            public int Percentage
            {
                get { return _percentage; }
                set { _percentage = value; }
            }

            public ProgressEventArgs(int percentage)
            {
                this._percentage = percentage;
            }
        }
    }
}
