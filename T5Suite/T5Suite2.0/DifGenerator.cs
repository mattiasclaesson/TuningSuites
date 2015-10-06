using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Trionic5Tools;


namespace T5Suite2
{
    class DifGenerator
    {
        public delegate void ExportProgress(object sender, ProgressEventArgs e);
        public event DifGenerator.ExportProgress onExportProgress;

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }

        LogFilterCollection _filters = new LogFilterCollection();

        public void SetFilters(LogFilterCollection filters)
        {
            _filters = filters;
        }

        private float FetchPreviousValueFromLine(string previousline, string varnametofetch)
        {
            float retval = -1000000;
            char[] sep = new char[1];
            char[] sep2 = new char[1];
            sep.SetValue('|', 0);
            sep2.SetValue('=', 0);
             string[] values = previousline.Split(sep);
             if (values.Length > 0)
             {

                 for (int t = 1; t < values.Length; t++)
                 {
                     string subvalue = (string)values.GetValue(t);
                     string[] subvals = subvalue.Split(sep2);
                     if (subvals.Length == 2)
                     {
                         string varname = (string)subvals.GetValue(0);
                         if (varname == varnametofetch)
                         {
                             retval = (float) ConvertToDouble((string)subvals.GetValue(1));
                             if (varname == "AD_sond")
                             {
                                 retval = (float)ConvertToLambda(retval);
                             }
                             if (varname == _widebandSymbol)
                             {
                                 if (retval >= 25)
                                 {
                                     retval = (float)ConvertToWidebandLambda(retval);
                                 }
                             }
                             if (varname == "P_fak" || varname == "P gain")
                             {
                                 if (retval > 32000) retval = -(65535 - retval);
                             }
                             if (varname == "I_fak" || varname == "I gain")
                             {
                                 if (retval > 32000) retval = -(65535 - retval);
                             }
                             if (varname == "D_fak" || varname == "D gain")
                             {
                                 if (retval > 32000) retval = -(65535 - retval);
                             }
                         }
                     }
                 }
             }
            return retval;
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
//            Console.WriteLine("ConvertToWidebandLambda original: " + value.ToString());
            double retval = value;
            double voltage = ((value) / 255) * (_maximumVoltageWideband / 1000 - _minimumVoltageWideband / 1000);
            //Console.WriteLine("ConvertToWidebandLambda voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < _minimumVoltageWideband / 1000) voltage = _minimumVoltageWideband / 1000;
            if (voltage > _maximumVoltageWideband / 1000) voltage = _maximumVoltageWideband / 1000;
            ///Console.WriteLine("ConvertToWidebandLambda voltage after bounds: " + voltage.ToString());
            double steepness = ((_highAFR / 1000) - (_lowAFR / 1000)) / ((_maximumVoltageWideband / 1000) - (_minimumVoltageWideband / 1000));
            //Console.WriteLine("ConvertToWidebandLambda steepness: " + steepness.ToString());
            //Console.WriteLine("ConvertToWidebandLambda lowAFR: " + _lowAFR.ToString() + " highAFR: " + _highAFR.ToString() + " lowVolt: " + _minimumVoltageWideband.ToString() + " lowAFR: " + " highVolt: "+ _maximumVoltageWideband.ToString());
            retval = (_lowAFR / 1000) + (steepness * (voltage - (_minimumVoltageWideband / 1000)));
            //Console.WriteLine("ConvertToWidebandLambda retval: " + retval.ToString());
            return retval * 1000;
        }

        private double ConvertToWidebandLambda_OLD(double value)
        {
            double retval = value;
            if (_useWidebandInput)
            {
                // convert to AFR value using wideband lambda sensor settings
                // ranges 0 - 255 will be default for 0-5 volt
                double voltage = (value * 5) / 255;
                if (voltage < _minimumVoltageWideband) voltage = _minimumVoltageWideband;
                if (voltage > _maximumVoltageWideband) voltage = _maximumVoltageWideband;
                double steepness = (_highAFR - _lowAFR) / (_maximumVoltageWideband - _minimumVoltageWideband);
                retval = _lowAFR + (steepness * (voltage - _minimumVoltageWideband));
            }
            return retval;
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


        public bool ConvertFileToDif(string filename, SymbolCollection sc, DateTime startDate, DateTime endDate, bool RescaleTimeScaleToLogWorksInterval, bool DoInterpolationOfPoints)
        {
            bool retval = false;
            string newfilename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".dif";
            bool _calculateregkonmat = false;
            bool pwmut10present = false;
            bool pfakpresent = false;
            bool ifakpresent = false;
            bool dfakpresent = false;
           
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname == "Pgm_status")
                {
                    //sc.Remove(sh);
                    break;
                }
            }
            int colorindex = 1;
            foreach (SymbolHelper sh in sc)
            {
                if (System.Drawing.ColorTranslator.ToWin32(sh.Color) == 0)
                {
                    sh.Color = System.Drawing.Color.FromArgb(0, colorindex, colorindex, colorindex);
                    colorindex++;
                    //sc.Remove(sh);
                    //symbolremoved = true;
                    //break;
                }
            }
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname == "PWM_ut10") pwmut10present = true;
                if (sh.Varname == "P_fak") pfakpresent = true;
                if (sh.Varname == "I_fak") ifakpresent = true;
                if (sh.Varname == "D_fak") dfakpresent = true;
            }
            if (pwmut10present && pfakpresent && ifakpresent && dfakpresent)
            {
                //                    _calculateregkonmat = true;
            }
            //_calculateregkonmat = false; /// TEST

            CastExportProgressEvent(1);
            if (File.Exists(newfilename)) File.Delete(newfilename);
            using (StreamWriter sw = new StreamWriter(newfilename, false))
            {
                // first write the dif header
                sw.WriteLine("TABLE");
                sw.WriteLine("0,1");
                sw.WriteLine("\"EXCEL\"");
                sw.WriteLine("VECTORS");
                sw.WriteLine("0, 31280");
                sw.WriteLine("\"LMTR\"");
                sw.WriteLine("TUPLES");
                int count = sc.Count + 1;
                if (_calculateregkonmat) count++;
                sw.WriteLine("0," + count.ToString());
                sw.WriteLine("\"1222200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\"");
                sw.WriteLine("DATA");
                sw.WriteLine("0,0");
                sw.WriteLine("\"\"");
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Input Description\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0"); // 1 extra for regkonmat
                    sw.WriteLine("\"\"");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"From device:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"LM-1 (LM-1:" + t.ToString() + ")\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"LM-1 (LM-1:" + sc.Count.ToString() + ")\"");
                }

                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Name:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"" + sc[t].Varname + "\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"Regkonmat\"");
                }

                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Unit:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"" + ConvertToUnits(sc[t].Varname) + "\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"%\"");
                }

                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Range:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("0," + ConvertToRangeFrom(sc[t].Varname) /*+ "," + COnvertToRangeUpto(sc[t].Varname)*/);
                    sw.WriteLine("V");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("0,0");
                    sw.WriteLine("V");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"equiv(Sample):\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("0,0");
                    sw.WriteLine("V");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("0,0");
                    sw.WriteLine("V");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"to:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("0," + COnvertToRangeUpto(sc[t].Varname) + "");
                    sw.WriteLine("V");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("0,100");
                    sw.WriteLine("V");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"equiv(Sample):\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("0,4096");
                    sw.WriteLine("V");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("0,4096");
                    sw.WriteLine("V");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Color:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    Int32 color = sc[t].Color.R * 256 * 256;
                    color += sc[t].Color.G * 256;
                    color += sc[t].Color.B;
                    sw.WriteLine("0," + /*System.Drawing.ColorTranslator.ToWin32(sc[t].Color).ToString()*/ color.ToString());//ConvertToColor(sc[t].Varname));
                    sw.WriteLine("V");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("0," + System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Pink).ToString());
                    sw.WriteLine("V");
                }

                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"-End-\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"\"");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Session 1\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"\"");
                }
                sw.WriteLine("-1,0");
                sw.WriteLine("BOT");
                sw.WriteLine("1,0");
                sw.WriteLine("\"Name:\"");
                for (int t = 0; t < sc.Count; t++)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"" + sc[t].Varname + "\"");
                }
                if (_calculateregkonmat)
                {
                    sw.WriteLine("1,0");
                    sw.WriteLine("\"Regkonmat\"");
                }
                CastExportProgressEvent(10);
                FileInfo fi = new FileInfo(filename);
                string[] alllines = File.ReadAllLines(filename);

                //using (StreamReader sr = new StreamReader(filename))
                {
                    long rest_ticks = 0;
                    //string line = string.Empty;
                    string previousline = string.Empty;
                    char[] sep = new char[1];
                    char[] sep2 = new char[1];
                    int linecount = 0;
                    sep.SetValue('|', 0);
                    sep2.SetValue('=', 0);
                    DateTime dtpreviousline = DateTime.MaxValue;
                    //while ((line = sr.ReadLine()) != null)
                    foreach(string line in alllines)
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
                                    int numberofrepeats = 1;

                                    if (RescaleTimeScaleToLogWorksInterval)
                                    {
                                        dtpreviousline = dtpreviousline.AddTicks(-rest_ticks);
                                        if (dtpreviousline <= dt)
                                        {
                                            TimeSpan tsrepeats = new TimeSpan(dt.Ticks - dtpreviousline.Ticks);
                                            // tel vorige rest erbij
                                            //tsrepeats = tsrepeats.Add(new TimeSpan(rest_ticks));
                                            numberofrepeats = (int)(tsrepeats.TotalMilliseconds / 83.33F/*82.0F*/);
                                            //if (numberofrepeats > 10) numberofrepeats = 10;
                                            if (numberofrepeats <= 0) numberofrepeats = 1;

                                            // rest_ticks opnieuw uitrekenen
                                            DateTime exported_to = dtpreviousline.AddMilliseconds((double)numberofrepeats * 83.33F);
                                            rest_ticks = Math.Abs(dt.Ticks - exported_to.Ticks);
                                            if (rest_ticks > TimeSpan.TicksPerMinute) rest_ticks = 0;
                                            long rest_ms = rest_ticks / TimeSpan.TicksPerMillisecond;
                                          //  Console.WriteLine("From timespan: " + dtpreviousline.Minute.ToString("D2") + ":" + dtpreviousline.Second.ToString("D2") + "." + dtpreviousline.Millisecond.ToString("D3") + " upto " + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2") + "." + dt.Millisecond.ToString("D3") + " export_to " + exported_to.Minute.ToString("D2") + ":" + exported_to.Second.ToString("D2") + "." + exported_to.Millisecond.ToString("D3") + " results in " + numberofrepeats.ToString() + " and rest = " + rest_ticks.ToString() + " is " + rest_ms.ToString() + " ms");
                                            dtpreviousline = dt;
                                        }
                                        else
                                        {
                                            dtpreviousline = dt; // sync
                                            rest_ticks = 0;
                                        }
                                    }
                                    if (numberofrepeats > 500) numberofrepeats = 1; // prevent long pauses in the file <GS-19102010>
                                    for (int repeatcount = 1; repeatcount <= numberofrepeats; repeatcount++)
                                    {
                                        sw.WriteLine("-1,0");
                                        sw.WriteLine("BOT");
                                        TimeSpan ts = new TimeSpan(dt.Ticks - startDate.Ticks);
                                        float millisecs = (float)(ts.TotalMilliseconds) / 1000F;
                                        sw.WriteLine("0," + millisecs.ToString("F4").Replace(',', '.'));
                                        sw.WriteLine("V");
                                        double ifak = -1000000;
                                        double pfak = -1000000;
                                        double dfak = -1000000;
                                        double pwmut = -1000000;
                                        bool symbolfound = false;
                                        foreach (SymbolHelper sh in sc)
                                        {
                                            symbolfound = false;
                                            for (int t = 1; t < values.Length; t++)
                                            {
                                                string subvalue = (string)values.GetValue(t);
                                                string[] subvals = subvalue.Split(sep2);
                                                if (subvals.Length == 2)
                                                {
                                                    string varname = (string)subvals.GetValue(0);

                                                    //foreach (SymbolHelper sh in sc)
                                                    //{
                                                    if (sh.Varname == varname)
                                                    {
                                                        symbolfound = true;
                                                        
                                                        if ((varname != "Pgm_mod!") && (varname != "Pgm_status"))
                                                        {
                                                            double value = ConvertToDouble((string)subvals.GetValue(1));
                                                            if (varname == "AD_sond")
                                                            {
                                                                value = ConvertToLambda(value);
                                                            }
                                                            if (varname == _widebandSymbol)
                                                            {
                                                                if (value >= 25)
                                                                {
                                                                    value = ConvertToWidebandLambda(value);
                                                                }
                                                            }
                                                            if (varname == "P_fak" || varname == "P gain")
                                                            {
                                                                if (value > 32000) value = -(65535 - value);
                                                                pfak = value;
                                                            }

                                                            if (varname == "I_fak" || varname == "I gain")
                                                            {
                                                                if (value > 32000) value = -(65535 - value);
                                                                ifak = value;
                                                            }
                                                            if (varname == "D_fak" || varname == "D gain")
                                                            {
                                                                if (value > 32000) value = -(65535 - value);
                                                                dfak = value;
                                                            }
                                                            if (varname == "PWM_ut10" || varname == "PWM APC")
                                                            {
                                                                //value /= 10;
                                                                pwmut = value;
                                                            }
                                                            if (DoInterpolationOfPoints)
                                                            {
                                                                if (previousline != string.Empty)
                                                                {
                                                                    if (numberofrepeats > 1)
                                                                    {
                                                                        double previousvalue = FetchPreviousValueFromLine(previousline, varname);
                                                                        if (previousvalue > -1000000)
                                                                        {
                                                                            // interpolate between current and 
                                                                            float difference = (float)value - (float)previousvalue;
                                                                            float newvalue = (float)value - ((((float)(numberofrepeats)-(float)repeatcount) / (float)numberofrepeats) * difference);
                                                                            if (varname == "P_fak" || varname == "P gain")
                                                                            {
                                                                                if (newvalue > 32000) newvalue = -(65535 - newvalue);
                                                                                pfak = newvalue;
                                                                            }

                                                                            if (varname == "I_fak" || varname == "I gain")
                                                                            {
                                                                                if (newvalue > 32000) newvalue = -(65535 - newvalue);
                                                                                ifak = newvalue;
                                                                            }
                                                                            if (varname == "D_fak" || varname == "D gain")
                                                                            {
                                                                                if (newvalue > 32000) newvalue = -(65535 - newvalue);
                                                                                dfak = newvalue;
                                                                            }
                                                                            if (varname == "PWM_ut10" || varname == "PWM APC")
                                                                            {
                                                                                //value /= 10;
                                                                                pwmut = newvalue;
                                                                            }
                                                                            value = newvalue;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (varname == "Rpm" || varname == "Lufttemp" || varname == "Bil_hast" || varname == "EGT")
                                                            {
                                                                sw.WriteLine("0," + value.ToString().Replace(',', '.'));
                                                            }
                                                            else if (varname == "Ign_angle" || varname == "TQ" || varname == "InjectorDC" || varname == "Power" || varname == "Torque")
                                                            {
                                                                sw.WriteLine("0," + value.ToString("F1").Replace(',', '.'));
                                                            }
                                                            else
                                                            {
                                                                sw.WriteLine("0," + value.ToString("F2").Replace(',', '.'));
                                                            }
                                                            sw.WriteLine("V");

                                                        }
                                                        /*else if (varname == "Pgm_status")
                                                        {
                                                            // indicate what fuelmap is used
                                                            Int64 value = Convert.ToInt64((string)subvals.GetValue(1));
                                                            if ((value & 0x00000200) > 0)
                                                            {
                                                                Console.WriteLine("Knock map used on: " + dt.ToString());
                                                            }
                                                            

                                                        }*/
                                                        break;
                                                    }
                                                    //}
                                                }
                                            }
                                            if (!symbolfound)
                                            {
                                                // dan 0 schrijven
                                                sw.WriteLine("0,0");
                                                sw.WriteLine("V");
                                            }
                                            if (_calculateregkonmat)
                                            {
                                                if (pfak != -1000000 && ifak != -1000000 && dfak != -1000000 && pwmut != -1000000)
                                                {

                                                    double regkonmat = (pwmut * 10) - pfak - ifak - dfak;
                                                    regkonmat /= 10;
                                                    sw.WriteLine("0," + regkonmat.ToString("F3").Replace(',', '.'));
                                                    sw.WriteLine("V");
                                                }
                                                else
                                                {
                                                    sw.WriteLine("0,0");
                                                    sw.WriteLine("V");
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                        previousline = line;
                    }
                    sw.WriteLine("-1,0");
                    sw.WriteLine("EOD");

                }
                CastExportProgressEvent(100);
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
                                if ((varname != "Pgm_mod!") && (varname != "Pgm_status"))
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
                                }
                            }
                        
                    }
                }
            }
            return retval;
        }

        private string COnvertToRangeUpto(string symbolname)
        {
            string retval = "1000";
            switch (symbolname)
            {
                case "KnockInfo":
                case "Idle":
                case "Warmup":
                case "ClosedLoop":
                    retval = "2";
                    break;
                case "RPM":
                    retval = "8000";
                    break;
                case "TPSAccCyl1":
                case "TPSAccCyl2":
                case "TPSAccCyl3":
                case "TPSAccCyl4":
                case "LoadAccCyl1":
                case "LoadAccCyl2":
                case "LoadAccCyl3":
                case "LoadAccCyl4":
                case "TPSRetCyl1":
                case "TPSRetCyl2":
                case "TPSRetCyl3":
                case "TPSRetCyl4":
                case "LoadRetCyl1":
                case "LoadRetCyl2":
                case "LoadRetCyl3":
                case "LoadRetCyl4":
                    retval = "100";
                    break;
                case "TORQUE":
                case "Torque":
                    retval = "800";
                    break;
                case "TQ":
                    retval = "800";
                    break;
                case "POWER":
                case "Power":
                    retval = "700";
                    break;
                case "IGNADV":
                    retval = "40";
                    break;
                case "Ign_angle":
                case "Ign_angle_byte":
                case "Ignition angle":
                    retval = "45";
                    break;
                case "Reg_kon_apc":
                    retval = "100";
                    break;
                case "REGKONMAT":
                    retval = "100";
                    break;
                case "IMPORTANTLINE":
                case "ImportantLine":
                    retval = "2";
                    break;
                case "AFR":
                case "WB Lambda":
                    retval = "23.0";
                    break;
                case "P_Manifold10":
                case "P_Manifold":
                case "P_Medel":
                case "P_medel":
                case "Boost":
                    retval = "2.5";
                    break;
                case "Lufttemp":
                case "IAT":
                    retval = "120";
                    break;
                case "Kyl_temp":
                case "Coolant":
                    retval = "120";
                    break;
                case "Lambdaint":
                    retval = "255";
                    break;
                case "Knock_average_limit":
                    retval = "500";
                    break;
                case "Knock_ref_level":
                    retval = "500";
                    break;
                case "Knock_lim":
                    retval = "500";
                    break;
                case "Knock_level":
                    retval = "500";
                    break;
                case "Knock_average":
                    retval = "500";
                    break;
                case "Knock_map_lim":
                    retval = "500";
                    break;
                case "Rpm":
                    retval = "8000";
                    break;
                case "Regl_tryck":
                case "Target boost":
                    retval = "2.5";
                    break;
                case "Max_tryck":
                case "Boost request":
                case "Boost error":
                    retval = "2.5";
                    break;
                case "InjectorDC":
                    retval = "120";
                    break;
                case "Injection duration":
                    retval = "35";
                    break;
                case "Boost reduction":
                    retval = "0.5";
                    break;
                case "Insptid_ms10":
                    retval = "35";
                    break;
                case "PWM_ut10":
                case "PWM APC":
                    retval = "100";
                    break;
                case "Medeltrot":
                case "TPS":
                    retval = "155";
                    break;
                case "Bil_hast":
                case "Speed":
                    retval = "300";
                    break;
                case "Gear":
                    retval = "6";
                    break;
                case "P_fak":
                case "P gain":
                    retval = "2000";
                    break;
                case "I_fak":
                case "I gain":
                    retval = "2000";
                    break;
                case "D_fak":
                case "D gain":
                    retval = "2000";
                    break;
                case "AD_sond":
                    retval = "2";
                    break;
                case "U_lambda_cat":
                    retval = "255";
                    break;
                default:
                    if (symbolname == _widebandSymbol)
                    {
                        retval = "24";
                    }
                    break;
            }
            return retval;
        }

        private string ConvertToRangeFrom(string symbolname)
        {
            string retval = "0";
            switch (symbolname)
            {
                case "KnockInfo":
                case "Idle":
                case "Warmup":
                case "ClosedLoop":
                    retval = "0";
                    break;
                case "RPM":
                    retval = "0";
                    break;
                case "TQ":
                    retval = "0";
                    break;
                case "TORQUE":
                    retval = "0";
                    break;
                case "POWER":
                    retval = "0";
                    break;
                case "Ign_angle":
                case "Ign_angle_byte":
                case "Ignition angle":
                    retval = "-10";
                    break;
                case "Reg_kon_apc":
                    retval = "0";
                    break;
                case "IGNADV":
                    retval = "-10" ;
                    break;
                case "IMPORTANTLINE":
                case "ImportantLine":
                    retval = "0";
                    break;
                case "REGKONMAT":
                    retval = "0";
                    break;
                case "P_Manifold10":
                case "P_Manifold":
                case "P_Medel":
                case "P_medel":
                case "Boost":
                    retval = "-1";
                    break;
                case "AFR":
                case "WB Lambda":
                    retval = "7.0";
                    break;
                case "Lufttemp":
                case "IAT":
                    retval = "-40";
                    break;
                case "Kyl_temp":
                case "Coolant":
                    retval = "-40";
                    break;
                case "Lambdaint":
                    retval = "0";
                    break;
                case "Knock_average_limit":
                    retval = "0";
                    break;
                case "Knock_ref_level":
                    retval = "0";
                    break;
                case "Knock_lim":
                    retval = "0";
                    break;
                case "Knock_level":
                    retval = "0";
                    break;
                case "Knock_average":
                    retval = "0";
                    break;
                case "Knock_map_lim":
                    retval = "0";
                    break;
                case "Rpm":
                    retval = "0";
                    break;
                case "Regl_tryck":
                case "Target boost":
                    retval = "-1";
                    break;
                case "Max_tryck":
                case "Boost request":
                    retval = "-1";
                    break;
                case "Boost error":
                    retval = "0";
                    break;
                case "Injection duration":
                    retval = "0";
                    break;
                case "InjectorDC":
                    retval = "0";
                    break;
                case "Boost reduction":
                    retval = "0";
                    break;
                case "Insptid_ms10":
                    retval = "0";
                    break;
                case "PWM_ut10":
                case "PWM APC":
                    retval = "0";
                    break;
                case "Medeltrot":
                case "TPS":
                    retval = "0";
                    break;
                case "Bil_hast":
                case "Speed":
                    retval = "0";
                    break;
                case "Gear":
                    retval = "0";
                    break;
                case "P_fak":
                case "P gain":
                    retval = "-2000";
                    break;
                case "I_fak":
                case "I gain":
                    retval = "-2000";
                    break;
                case "D_fak":
                case "D gain":
                    retval = "-2000";
                    break;
                case "AD_sond":
                    retval = "0";
                    break;
                
                case "U_lambda_cat":
                    retval = "0";
                    break;
                default:
                    if (symbolname == _widebandSymbol)
                    {
                        retval = "7";
                    }
                    break;
            }
            return retval;
        }

        private string ConvertToUnits(string symbolname)
        {
            string retval = symbolname;
            switch (symbolname)
            {
                case "KnockInfo":
                    retval = "Knock condition";
                    break;
                case "Idle":
                    retval = "Idle condition";
                    break;
                case "Warmup":
                    retval = "Warmup condition";
                    break;
                case "ClosedLoop":
                    retval = "Closed loop";
                    break;

                case "TPSAccCyl1":
                    retval = "TPS accel enrichment #1";
                    break;
                case "TPSAccCyl2":
                    retval = "TPS accel enrichment #2";
                    break;
                case "TPSAccCyl3":
                    retval = "TPS accel enrichment #3";
                    break;
                case "TPSAccCyl4":
                    retval = "TPS accel enrichment #4";
                    break;
                case "LoadAccCyl1":
                    retval = "Load accel enrichment #1";
                    break;
                case "LoadAccCyl2":
                    retval = "Load accel enrichment #2";
                    break;
                case "LoadAccCyl3":
                    retval = "Load accel enrichment #3";
                    break;
                case "LoadAccCyl4":
                    retval = "Load accel enrichment #4";
                    break;
                case "TPSRetCyl1":
                    retval = "TPS retard enrichment #1";
                    break;
                case "TPSRetCyl2":
                    retval = "TPS retard enrichment #2";
                    break;
                case "TPSRetCyl3":
                    retval = "TPS retard enrichment #3";
                    break;
                case "TPSRetCyl4":
                    retval = "TPS retard enrichment #4";
                    break;
                case "LoadRetCyl1":
                    retval = "Load retard enrichment #1";
                    break;
                case "LoadRetCyl2":
                    retval = "Load retard enrichment #2";
                    break;
                case "LoadRetCyl3":
                    retval = "Load retard enrichment #3";
                    break;
                case "LoadRetCyl4":
                    retval = "Load retard enrichment #4";
                    break;

                case "RPM":
                    retval = "rpm";
                    break;
                case "TQ":
                    retval = "Nm";
                    break;
                case "TORQUE":
                    retval = "Nm";
                    break;
                case "POWER":
                    retval = "hp";
                    break;
                case "U_lambda_cat":
                    retval = "lambda (2)";
                    break;
                case "Ign_angle":
                case "Ign_angle_byte":
                case "Ignition angle":
                    retval = "d BTDC";
                    break;
                case "Reg_kon_apc":
                    retval = "%";
                    break;
                case "IMPORTANTLINE":
                case "ImportantLine":
                    retval = "NOTE THIS [F6]";
                    break;
                case "IGNADV":
                    retval = "degrees";
                    break;
                case "REGKONMAT":
                    retval = "% rkm";
                    break;
                case "P_Manifold10":
                case "P_Manifold":
                case "P_Medel":
                case "P_medel":
                case "Boost":
                    retval = "bar";
                    break;
                case "AFR":
                case "WB Lambda":
                    retval = "AFR";
                    break;
                case "Lufttemp":
                case "IAT":
                    retval = "C";
                    break;
                case "Kyl_temp":
                case "Coolant":
                    retval = "C";
                    break;
                case "Lambdaint":
                    retval = "Lambda_int";
                    break;
                case "Knock_average_limit":
                    retval = "Knock_average_limit";
                    break;
                case "Knock_ref_level":
                    retval = "Knock_ref_level";
                    break;
                case "Knock_lim":
                    retval = "Knock_lim";
                    break;
                case "Knock_level":
                    retval = "Knock level";
                    break;
                case "Knock_average":
                    retval = "Knock average";
                    break;
                case "Knock_map_lim":
                    retval = "Knock limit";
                    break;
                case "Rpm":
                    retval = "rpm";
                    break;
                case "Regl_tryck":
                case "Target boost":
                    retval = "bar";
                    break;
                case "Max_tryck":
                case "Boost request":
                case "Boost error":
                    retval = "bar";
                    break;
                case "InjectorDC":
                    retval = "%";
                    break;
                case "Injection duration":
                    retval = "ms";
                    break;
                case "Boost reduction":
                    retval = "bar";
                    break;
                case "Insptid_ms10":
                    retval = "ms";
                    break;
                case "PWM_ut10":
                case "PWM APC":
                    retval = "%";
                    break;
                case "Medeltrot":
                case "TPS":
                    retval = "%";
                    break;
                case "Bil_hast":
                case "Speed":
                    retval = "km/h";
                    break;
                case "Gear":
                    retval = "gear";
                    break;
                case "P_fak":
                case "P gain":
                case "I_fak":
                case "I gain":
                case "D_fak":
                case "D gain":
                case "I_fak_max":
                    retval = "pnts";
                    break;
                case "AD_sond":
                    retval = "Lambda";
                    break;
                default:
                    if (symbolname == _widebandSymbol)
                    {
                        retval = "WB Lambda";
                    }
                    break;
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
