using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Nevron.GraphicsCore;
using Nevron.Chart;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmDynoChart : DevExpress.XtraEditors.XtraForm
    {
        /*** values for injdc ****/

        private int inj_konst = 21;

        public int Inj_konst
        {
            get { return inj_konst; }
            set { inj_konst = value; }
        }
        private int m_iat = 163; // go for 20 degrees

        private int[] luft_kompfak;

        public int[] Luft_kompfak
        {
            get { return luft_kompfak; }
            set { luft_kompfak = value; }
        }
        private int[] lufttemp_steg;

        public int[] Lufttemp_steg
        {
            get { return lufttemp_steg; }
            set { lufttemp_steg = value; }
        }
        private int[] lufttemp_tab;

        public int[] Lufttemp_tab
        {
            get { return lufttemp_tab; }
            set { lufttemp_tab = value; }
        }
        private int[] kyltemp_steg;
        public int[] Kyltemp_steg
        {
            get { return kyltemp_steg; }
            set { kyltemp_steg = value; }
        }
        private int[] kyltemp_tab;

        public int[] Kyltemp_tab
        {
            get { return kyltemp_tab; }
            set { kyltemp_tab = value; }
        }

        private int batt_volt = 13;
        private int[] batt_korr_tab;

        public int[] Batt_korr_tab
        {
            get { return batt_korr_tab; }
            set { batt_korr_tab = value; }
        }

        private int min_tid = 0;


        public int Min_tid
        {
            get { return min_tid; }
            set { min_tid = value; }
        }
        private int[] fuel_map_x_axis;

        public int[] Fuel_map_x_axis
        {
            get { return fuel_map_x_axis; }
            set { fuel_map_x_axis = value; }
        }
        private int[] fuel_map_y_axis;

        public int[] Fuel_map_y_axis
        {
            get { return fuel_map_y_axis; }
            set { fuel_map_y_axis = value; }
        }

        private byte[] insp_mat;

        public byte[] Insp_mat
        {
            get { return insp_mat; }
            set { insp_mat = value; }
        }

        /*** values for injDC ***/ 


        private int[] x_axisvalues;

        public int[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }

        public frmDynoChart()
        {
            InitializeComponent();
        }

        private void frmDynoChart_Load(object sender, EventArgs e)
        {
            
        }

        public void FillGraph(double[] values, double[] injvalues, TurboType turbo)
        {
            DataTable chartdt = new DataTable();
            chartdt.Columns.Add("X", Type.GetType("System.Double"));
            chartdt.Columns.Add("Y", Type.GetType("System.Double"));
            chartdt.Columns.Add("INJDC", Type.GetType("System.Double"));
            for (int i = 0; i < values.Length; i++)
            {
                chartdt.Rows.Add(Convert.ToDouble(x_axisvalues.GetValue(i)), values.GetValue(i), injvalues.GetValue(i));
            }
            NChart chart = nChartControl1.Charts[0];
            //NSeries series = (NSeries)chart.Series[0];
            NSmoothLineSeries line = null;
            NSmoothLineSeries line2 = null;
            NSmoothLineSeries line3 = null;
            if (chart.Series.Count == 0)
            {
                line = (NSmoothLineSeries)chart.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line = (NSmoothLineSeries)chart.Series[0];
            }
            if (chart.Series.Count == 1)
            {
                line2 = (NSmoothLineSeries)chart.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line2 = (NSmoothLineSeries)chart.Series[1];
            }
            if (chart.Series.Count == 2)
            {
                line3 = (NSmoothLineSeries)chart.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line3 = (NSmoothLineSeries)chart.Series[2];
            }
            line.ClearDataPoints();
            line2.ClearDataPoints();
            line3.ClearDataPoints();
            PressureToTorque ptt = new PressureToTorque();
            foreach (DataRow dr in chartdt.Rows)
            {
                double power = 0;
                double torque = 0;
                torque = ptt.CalculateTorqueFromPressure(Convert.ToDouble(dr["Y"]), turbo);
                power = TorqueToPower(torque, Convert.ToDouble(dr["X"]));
                //power = 
                line.AddDataPoint(new NDataPoint(Convert.ToDouble(dr["X"]), torque));
                line2.AddDataPoint(new NDataPoint(Convert.ToDouble(dr["X"]), power));
                line3.AddDataPoint(new NDataPoint(Convert.ToDouble(dr["X"]), Convert.ToDouble(dr["INJDC"])));
            }
            nChartControl1.Refresh();
        }

        private double TorqueToPower(double torque, double rpm)
        {
            double power = (torque * rpm) / 7121;
            return power;
        }

        private void Init2dGraph()
        {
            nChartControl1.Settings.ShapeRenderingMode = ShapeRenderingMode.HighQuality;
            nChartControl1.Legends.Clear(); // no legend
            NChart chart2d = nChartControl1.Charts[0];

            NSmoothLineSeries surface = null;
            if (chart2d.Series.Count == 0)
            {
                surface = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                surface = (NSmoothLineSeries)chart2d.Series[0];
            }

            chart2d.BoundsMode = BoundsMode.Stretch;
            NLinearScaleConfigurator linearScale = (NLinearScaleConfigurator)chart2d.Axis(StandardAxis.PrimaryY).ScaleConfigurator;
            linearScale.MajorGridStyle.LineStyle.Pattern = LinePattern.Dot;
            linearScale.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
            NScaleStripStyle stripStyle = new NScaleStripStyle(new NColorFillStyle(Color.Beige), null, true, 0, 0, 1, 1);
            stripStyle.Interlaced = true;
            stripStyle.SetShowAtWall(ChartWallType.Back, true);
            stripStyle.SetShowAtWall(ChartWallType.Left, true);
            linearScale.StripStyles.Add(stripStyle);
            NSmoothLineSeries line = null;
            if (chart2d.Series.Count == 0)
            {
                line = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line = (NSmoothLineSeries)chart2d.Series[0];
            }
            line.Name = "Power (bhp)";
            line.Legend.Mode = SeriesLegendMode.Series;
            line.UseXValues = true;
            line.UseZValues = false;
            line.DataLabelStyle.Visible = true;
            line.Values.ValueFormatter = new Nevron.Dom.NNumericValueFormatter("0");
            line.MarkerStyle.Visible = true;
            line.MarkerStyle.PointShape = PointShape.Sphere;
            line.MarkerStyle.AutoDepth = true;
            line.MarkerStyle.Width = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line.MarkerStyle.Height = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line.MarkerStyle.Depth = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            //line.HorizontalAxes = y_axisvalues;

            NSmoothLineSeries line2 = null;
            if (chart2d.Series.Count == 1)
            {
                line2 = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line2 = (NSmoothLineSeries)chart2d.Series[1];
            }
            line2.Name = "Torque (Nm)";
            line2.Legend.Mode = SeriesLegendMode.Series;
            line2.UseXValues = true;
            line2.UseZValues = false;
            line2.Values.ValueFormatter = new Nevron.Dom.NNumericValueFormatter("0");
            line2.MarkerStyle.Visible = true;
            line2.MarkerStyle.PointShape = PointShape.Sphere;
            line2.MarkerStyle.AutoDepth = true;
            line2.MarkerStyle.Width = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line2.MarkerStyle.Height = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line2.MarkerStyle.Depth = new NLength(1.4f, NRelativeUnit.ParentPercentage);

            NSmoothLineSeries line3 = null;
            if (chart2d.Series.Count == 2)
            {
                line3 = (NSmoothLineSeries)chart2d.Series.Add(SeriesType.SmoothLine);
            }
            else
            {
                line3 = (NSmoothLineSeries)chart2d.Series[2];
            }
            line3.Name = "Injector DC";
            line3.Legend.Mode = SeriesLegendMode.Series;
            line3.UseXValues = true;
            line3.UseZValues = false;
            line3.DataLabelStyle.Visible = true;
            line3.Values.ValueFormatter = new Nevron.Dom.NNumericValueFormatter("0.0");
            line3.MarkerStyle.Visible = true;
            line3.MarkerStyle.PointShape = PointShape.Sphere;
            line3.MarkerStyle.AutoDepth = true;
            line3.MarkerStyle.Width = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line3.MarkerStyle.Height = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            line3.MarkerStyle.Depth = new NLength(1.4f, NRelativeUnit.ParentPercentage);
            
            surface.Name = "Surface";
            //surface.Legend.Mode = SeriesLegendMode.SeriesLogic;
            //surface.PositionValue = 10.0;
            for (int i = 0; i < x_axisvalues.Length; i++)
            {
                surface.XValues.Add(x_axisvalues.GetValue(i));
            }
            NStyleSheet styleSheet = NStyleSheet.CreatePredefinedStyleSheet(PredefinedStyleSheet.Nevron);
            styleSheet.Apply(nChartControl1.Document);
        }

        private void frmDynoChart_Shown(object sender, EventArgs e)
        {
            Init2dGraph();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG images|*.png";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                nChartControl1.ImageExporter.SaveToFile(sfd.FileName, new NPngImageFormat());
            }
        }


        private IECUFile _trionicFile;

        public IECUFile TrionicFile
        {
            get { return _trionicFile; }
            set { _trionicFile = value; }
        }

        private T5AppSettings _appSettings;

        public T5AppSettings AppSettings
        {
            get { return _appSettings; }
            set { _appSettings = value; }
        }

        

        private int LookupCoolantTemperature(int axisvalue)
        {
            // find index in Kyltemp_steg
            int index = -1;
            int retval = -1;
            int smallestdiff = 256;
            int idx = 0;
            int secondvalue = -1;
            try
            {
                foreach (int i in Kyltemp_steg)
                {
                    if (Math.Abs(i - axisvalue) < smallestdiff)
                    {
                        index = idx;
                        smallestdiff = (int)Math.Abs(i - axisvalue);
                        if (i < axisvalue)
                        {
                            secondvalue = (int)Kyltemp_steg.GetValue(idx + 1);
                        }
                        else
                        {
                            secondvalue = (int)Kyltemp_steg.GetValue(idx - 1);
                        }
                    }
                    idx++;
                }
                if (index >= 0 && index < Kyltemp_tab.Length)
                {
                    // get value from Kyltemp_tab
                    retval = (int)Kyltemp_tab.GetValue(index);
                    int firstvalue = (int)Kyltemp_steg.GetValue(index);
                    int sval = -1000;
                    int diff = (int)Math.Abs(secondvalue - firstvalue);
                    int diff2 = axisvalue - firstvalue;
                    double percentage = (double)diff2 / (double)diff;
                    if (secondvalue > firstvalue)
                    {
                        // dan moeten we de volgende uit kyltemp_tab ook hebben
                        sval = (int)Kyltemp_tab.GetValue(index + 1);
                        percentage = (double)diff2 / (double)diff;
                    }
                    else
                    {
                        sval = (int)Kyltemp_tab.GetValue(index - 1);
                        percentage = (double)diff2 / (double)diff;
                        percentage = -percentage;

                    }
                    // hoeveel interpoleren?

                    retval += (int)((double)percentage * (double)(sval - retval));
                    retval -= 40;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            //Console.WriteLine("looked for : " + axisvalue.ToString() + " found idx in kyltemp_steg : " + index.ToString() + " value in kyltemp_tab:  " + retval.ToString());
            return retval;

        }
        

        private int Handle_temp_tables(int AD_value, int[] tab, int[] steg)
        {
            int steg_index = 0;
            int steg_value;
            int ret_value;


            /*find steg index*/
            while (steg[steg_index] != 255)
            {
                if (steg[steg_index] >= AD_value)
                {
                    break;
                }
                steg_index++;
            }



            if (steg_index == 0)
            {
                ret_value = tab[0];
            }
            else
            {
                steg_index--;
                if (tab[steg_index + 1] > tab[steg_index])
                {
                    //check
                    ret_value = tab[steg_index] + (((tab[steg_index + 1] - tab[steg_index]) * (AD_value - steg[steg_index])) / (steg[steg_index + 1] - steg[steg_index]));
                }
                else
                {
                    ret_value = tab[steg_index] - (((tab[steg_index] - tab[steg_index + 1]) * (AD_value - steg[steg_index])) / (steg[steg_index + 1] - steg[steg_index]));
                }
            }/*Handle temp tables*/
            return ret_value;
        }

        

        private int GetLuftTempFactor(int m_iat)
        {
            //P_manifold_LTF = ((Lufttemp_faktor + 384) * P_manifold10) / 512;
            //return 100;

            int Lufttemp_faktor = Handle_temp_tables(m_iat, luft_kompfak, lufttemp_steg);
            return Lufttemp_faktor;
            //int Lufttemp_faktor = LookupAirTemperature(m_iat);
            //return Lufttemp_faktor;
        }


        private byte Handle_tables(int y_value, byte x_value, int y_count, int x_count, byte[] matrix, int[] y_axis, int[] x_axis)
        {
            byte value;
            byte tmp1;
            byte tmp2;
            byte tmp3, vx, vy;
            byte x_indx;
            byte y_indx;
            int y_delta;
            byte x_delta;
            byte x_interpol;
            byte y_interpol;

            /*find y-index*/
            y_indx = 0;

            while (y_indx <= y_count - 1)
            {
                if (y_axis[y_indx] > y_value)
                {
                    break;
                }
                y_indx++;
            }

            /*find x-index*/
            x_indx = 0;

            while (x_indx <= x_count - 1)
            {
                if (x_axis[x_indx] > x_value)
                {
                    break;
                }
                x_indx++;
            }
            if (x_indx > 0)
                x_indx--;
            if (y_indx > 0)
                y_indx--;


            tmp1 = matrix[(y_indx * x_count) + x_indx];

            if (y_indx < y_count - 1)
            {
                tmp2 = matrix[(y_indx + 1) * x_count + x_indx];
            }
            else
            {
                tmp2 = tmp1;
            }
            if (x_indx < x_count - 1)
            {
                tmp3 = matrix[(y_indx * x_count) + (x_indx + 1)];
            }
            else
            {
                tmp3 = tmp1;
            }



            //trionic style x&y
            if (x_indx < x_count - 1 && y_indx < y_count - 1)
            {
                vx = interpolate2((tmp3 - tmp1), (x_axis[x_indx + 1] - x_axis[x_indx]), (x_value - x_axis[x_indx]), tmp1);
                vy = interpolate2((tmp2 - tmp1), (y_axis[y_indx + 1] - y_axis[y_indx]), (y_value - y_axis[y_indx]), tmp1);
            }
            else
            {
                vx = interpolate2((tmp3 - tmp1), (x_axis[x_indx] - x_axis[x_indx]), (x_value - x_axis[x_indx]), tmp1);
                vy = interpolate2((tmp2 - tmp1), (y_axis[y_indx] - y_axis[y_indx]), (y_value - y_axis[y_indx]), tmp1);
            }
            //printf("vx=0x%x vy=0x%x\n",vx,vy);
            //todo: interpolate vx and vy
            value = (byte)((vy + vx) / 2);
            // value=interpolate2(,tmp1);

            return value;
        }


        private byte interpolate2(int tmp1, int tmp2, int tmp3, int tmp4)
        {
            byte retval = 0;
            try
            {
                if (tmp2 != 0)
                {
                    retval = (byte)(((tmp1 * tmp3) / tmp2) + tmp4);
                }
                else
                {
                    retval = (byte)tmp4;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;

        }

        

        private int CalculateInjectionTime(int pressure, int m_iat)
        {
            //pressure *= 10; // must be P_manifold10

            int grund_tid = inj_konst * ((GetLuftTempFactor(m_iat) + 384) * pressure) / 512;
            return grund_tid;
            //(Insp_mat+128)/256

        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pressure">in decimal form, 200 = 200kpa = 2 bar</param>
        /// <param name="rpm"></param>
        /// <returns></returns>
        private double BoostRpmToInjectorDuration(int pressure, int rpm)
        {
            
            int Lufttemp_faktor = GetLuftTempFactor(m_iat);
            //rpm /= 10;
            int P_manifold10 = pressure * 10;
            int Last = 0;
            int Medellast = 0;
            if ((((Lufttemp_faktor + 384) * (P_manifold10 / 10)) / 512) < 255)
            {
                Last = ((Lufttemp_faktor + 384) * (P_manifold10 / 10)) / 512;
            }
            else
            {
                Last = 255;
            }
            Medellast = Last;
            //Console.WriteLine(pressure.ToString() + " gives medellast: " + Medellast.ToString());
            int cellvalue = (int)Handle_tables(rpm, (byte)Medellast, fuel_map_y_axis.Length, fuel_map_x_axis.Length, insp_mat, fuel_map_y_axis, fuel_map_x_axis);
            int injection_duration = CalculateInjectionTime(pressure, m_iat);
            float inj_dur_ms = injection_duration * (((float)cellvalue + 128) / 256);

            if (batt_volt >= 14)
            {
                batt_volt = 14;
            }
            else if (batt_volt < 5)
            {
                batt_volt = 4;
            }
            int Batt_korr = Batt_korr_tab[14 - batt_volt];

            inj_dur_ms += Batt_korr;


            if (inj_dur_ms >= 32500)
            {
                inj_dur_ms = 32500;
            }

            if (inj_dur_ms <= min_tid)
            {
                inj_dur_ms = min_tid;
            }
            float Injection_timems10 = inj_dur_ms / 25;
            float temp = 6000000 / (float)rpm;
            //float dutycycle = ((Injection_timems10) / temp) * 100;
            
            inj_dur_ms /= 250;
            float dutycycle = (rpm * inj_dur_ms) / 1200 ;

//            Console.WriteLine("Converting pressure: " + pressure.ToString() + " rpm: " + rpm.ToString() + " dc: " + dutycycle.ToString() + " ms: " + inj_dur_ms.ToString());
            return dutycycle;
            // for fun, take Fload_tab into account

        }

        private int LookupAirTemperature(int axisvalue)
        {
            // find index in Lufttemp_steg
            int index = -1;
            int retval = -1;
            int smallestdiff = 256;
            int idx = 0;
            int secondvalue = -1;
            try
            {
                foreach (int i in Lufttemp_steg)
                {
                    if (Math.Abs(i - axisvalue) < smallestdiff)
                    {
                        index = idx;
                        smallestdiff = (int)Math.Abs(i - axisvalue);
                        try
                        {
                            if (i < axisvalue)
                            {
                                secondvalue = (int)Lufttemp_steg.GetValue(idx + 1);
                            }
                            else
                            {
                                secondvalue = (int)Lufttemp_steg.GetValue(idx - 1);
                            }
                        }
                        catch (Exception sE)
                        {
                            Console.WriteLine(sE.Message);
                        }
                    }
                    idx++;
                }
                if (index >= 0 && index < Lufttemp_tab.Length)
                {
                    // get value from Lufttemp_tab
                    retval = (int)Lufttemp_tab.GetValue(index);
                    int firstvalue = (int)Lufttemp_steg.GetValue(index);
                    int sval = -1000;
                    int diff = (int)Math.Abs(secondvalue - firstvalue);
                    int diff2 = axisvalue - firstvalue;
                    double percentage = (double)diff2 / (double)diff;
                    if (secondvalue >= 0)
                    {
                        if (secondvalue > firstvalue)
                        {
                            // dan moeten we de volgende uit Lufttemp_tab ook hebben
                            sval = (int)Lufttemp_tab.GetValue(index + 1);
                            percentage = (double)diff2 / (double)diff;
                        }
                        else
                        {
                            sval = (int)Lufttemp_tab.GetValue(index - 1);
                            percentage = (double)diff2 / (double)diff;
                            percentage = -percentage;

                        }
                        // hoeveel interpoleren?
                        retval += (int)((double)percentage * (double)(sval - retval));
                    }
                    retval -= 40;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;

        }

        internal void BuildGraph()
        {
            if (_trionicFile != null)
            {
                Trionic5FileInformation m_trionicFileInformation = _trionicFile.GetFileInfo();
                X_axisvalues = _trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetBoostRequestMap()); // injection map Y = rpm
                Trionic5Properties props = _trionicFile.GetTrionicProperties();
                byte[] tryck_mat = _trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetBoostRequestMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostRequestMap()));
                if (props.AutomaticTransmission)
                {
                    tryck_mat = _trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetBoostRequestMapAUT()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostRequestMapAUT()));
                }
                /* new */

                Insp_mat = _trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                //Fuel_knock_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionKnockMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionKnockMap()));
                //injectiontiming.Idle_fuel_map = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIdleFuelMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()));
                Batt_korr_tab = _trionicFile.GetSymbolAsIntArray(m_trionicFileInformation.GetBatteryCorrectionMap());
                Min_tid = _trionicFile.GetSymbolAsInt("Min_tid!");

                Fuel_map_x_axis = _trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, _trionicFile.GetFileInfo().GetInjectionMap());
                Fuel_map_y_axis = _trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, _trionicFile.GetFileInfo().GetInjectionMap());
                //injectiontiming.Fuel_knock_map_x_axis = m_trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, "Fuel_knock_mat!");
                //injectiontiming.Fuel_knock_map_y_axis = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, "Fuel_knock_mat!");
                //injectiontiming.Idle_fuel_x_axis = m_trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, "Idle_fuel_korr!");
                //injectiontiming.Idle_fuel_y_axis = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, "Idle_fuel_korr!");
                Luft_kompfak = _trionicFile.Luft_kompfak_array;
                //Temp_steg = _trionicFile.Temp_steg_array;
                Kyltemp_steg = _trionicFile.Kyltemp_steg_array;
                Kyltemp_tab = _trionicFile.Kyltemp_tab_array;
                Lufttemp_steg = _trionicFile.Lufttemp_steg_array;
                Lufttemp_tab = _trionicFile.Lufttemp_tab_array;
                byte[] data = _trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectorConstant()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectorConstant()));
                int inj_konst = Convert.ToInt32(data.GetValue(0));
                Inj_konst = inj_konst;
                /* new */
                // get every 8th byte
                double[] values = new double[16];
                MapSensorType mst = _trionicFile.GetMapSensorType(_appSettings.AutoDetectMapsensorType);
                double[] injdc = new double[16]; // fill with injector DC in max values
                for (int i = 0; i < 16; i++)
                {
                    double val = Convert.ToDouble(tryck_mat[i * 8 + 7]);
                    
                    if (mst == MapSensorType.MapSensor30)
                    {
                        val *= 1.2;
                    }
                    else if (mst == MapSensorType.MapSensor35)
                    {
                        val *= 1.4;
                    }
                    else if (mst == MapSensorType.MapSensor40)
                    {
                        val *= 1.6;
                    }
                    else if (mst == MapSensorType.MapSensor50)
                    {
                        val *= 2.0;
                    }
                    injdc.SetValue(BoostRpmToInjectorDuration(Convert.ToInt32(val), Convert.ToInt32(X_axisvalues.GetValue(i))), i);
                    val -= 100;
                    val /= 100;
                    values.SetValue(val, i);
                }
                FillGraph(values, injdc, props.TurboType);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            BuildGraph();
        }
    }
}