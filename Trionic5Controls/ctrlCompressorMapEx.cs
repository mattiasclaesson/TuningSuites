using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Microsoft.Win32;

namespace Trionic5Controls
{
    public partial class ctrlCompressorMapEx : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void RefreshData(object sender, EventArgs e);
        public event ctrlCompressorMapEx.RefreshData onRefreshData;


        public enum CompressorMap : int
        {
            T25_55,
            T25_60,
            TD04,
            TD0416T, //nieuw
            TD0418T,
            TD0419T,
            TD0620G, //nieuw
            GT2871R, //nieuw
            GT28RS,
            GT3071R86,
            GT30R,
            GT40R,
            HX40W,
            S400SX371,
            GT17,
            T25_NG900
        }

        private CompressorMap _compressor = CompressorMap.T25_55;

        public ctrlCompressorMapEx()
        {
            InitializeComponent();
        }

        ~ctrlCompressorMapEx()
        {
            scalablePictureBox1.Picture = null;
        }

        public void ReleaseResources()
        {
            Console.WriteLine("Disposing of scaleablepicturebox");
            scalablePictureBox1.Picture = null; // release image
            scalablePictureBox1.Dispose();
            Console.WriteLine("Disposed of scaleablepicturebox");
        }

        private void t25ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load T25 map
            SetCompressorType(CompressorMap.T25_55);
        }

        private void tD0415GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load TD04 map
            SetCompressorType(CompressorMap.TD04);
        }

        private void gT28RSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load GT28RS map
            SetCompressorType(CompressorMap.GT28RS);
        }

        private void gT30RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load GT30R map
            SetCompressorType(CompressorMap.GT30R);
        }

        private double x_offset = 0;
        private double y_offset = 0;
        private double x_multiplier = 1;
        private double y_multiplier = 1;
        private int ori_width = 0;
        private int ori_height = 0;

        private double[] boost_request = new double[16] { 0.20, 0.50, 1.05, 1.08, 1.03, 1.00, 1.00, 0.98, 0.94, 0.91, 0.94, 0.88, 0.79, 0.68, 0.58, 0.50};

        public double[] Boost_request
        {
            get { return boost_request; }
            set { boost_request = value; }
        }
        private int[] rpm_points = new int[16] { 1000, 1500, 1750, 2000, 2250, 2500, 2750, 3000, 3250, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };

        public int[] Rpm_points
        {
            get { return rpm_points; }
            set
            {
                rpm_points = value;
                toolStripTextBox3.ToolTipText = "VE at " + rpm_points[0].ToString() + " rpm";
                toolStripTextBox4.ToolTipText = "VE at " + rpm_points[1].ToString() + " rpm";
                toolStripTextBox5.ToolTipText = "VE at " + rpm_points[2].ToString() + " rpm";
                toolStripTextBox6.ToolTipText = "VE at " + rpm_points[3].ToString() + " rpm";
                toolStripTextBox7.ToolTipText = "VE at " + rpm_points[4].ToString() + " rpm";
                toolStripTextBox8.ToolTipText = "VE at " + rpm_points[5].ToString() + " rpm";
                toolStripTextBox9.ToolTipText = "VE at " + rpm_points[6].ToString() + " rpm";
                toolStripTextBox10.ToolTipText = "VE at " + rpm_points[7].ToString() + " rpm";
                toolStripTextBox11.ToolTipText = "VE at " + rpm_points[8].ToString() + " rpm";
                toolStripTextBox12.ToolTipText = "VE at " + rpm_points[9].ToString() + " rpm";
                toolStripTextBox13.ToolTipText = "VE at " + rpm_points[10].ToString() + " rpm";
                toolStripTextBox14.ToolTipText = "VE at " + rpm_points[11].ToString() + " rpm";
                toolStripTextBox15.ToolTipText = "VE at " + rpm_points[12].ToString() + " rpm";
                toolStripTextBox16.ToolTipText = "VE at " + rpm_points[13].ToString() + " rpm";
                toolStripTextBox17.ToolTipText = "VE at " + rpm_points[14].ToString() + " rpm";
                toolStripTextBox18.ToolTipText = "VE at " + rpm_points[15].ToString() + " rpm";
                LoadRPMEfficiencies();
            }
        }

        public enum EngineType : int
        {
            Liter2,
            Liter23
        }

        private EngineType current_engineType = EngineType.Liter23;

        public EngineType Current_engineType
        {
            get { return current_engineType; }
            set { current_engineType = value; }
        }


        private double ConvertToFahrenheit(double celcius)
        {
            //F =  C × 1.8 + 32
            double retval = (celcius * 1.8) + 32;
            return retval;
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

        private double CalculateXCorrection()
        {
            double retval = 1;

            //retval = (double)scalablePictureBox1.Width / (double)ori_width;
            return retval;
        }
        private double CalculateYCorrection()
        {
            double retval = 1;
            //retval = (double)scalablePictureBox1.Height / (double)ori_height;
            return retval;
        }

        private double CalculateIntakeLoss(int rpm)
        {
            double retval = 0.08;
            if (rpm < 880) retval = 0.08;
            else if (rpm < 1260) retval = 0.10;
            else if (rpm < 1640) retval = 0.17;
            else if (rpm < 2020) retval = 0.28;
            else if (rpm < 2400) retval = 0.42;
            else if (rpm < 2780) retval = 0.50;
            else if (rpm < 3160) retval = 0.58;
            else if (rpm < 3540) retval = 0.65;
            else if (rpm < 3920) retval = 0.74;
            else if (rpm < 4300) retval = 0.82;
            else if (rpm < 4680) retval = 0.92;
            else if (rpm < 5060) retval = 1.03;
            else if (rpm < 5440) retval = 1.07;
            else if (rpm < 5820) retval = 1.10;
            else if (rpm < 6000) retval = 1.08;
            else retval = 1.43;
            return retval;
        }

        private double GetEngineEfficiency(int rpm)
        {
            // efficiency drops with increasing rpm

            double retval = 1.00;
            retval -= ((double)rpm / 100000) * 4;
            /*if (rpm >= 7000) retval = 0.65;
            else if (rpm >= 6000) retval = 0.70;
            else if (rpm >= 5000) retval = 0.75;
            else if (rpm >= 4000) retval = 0.80;
            else if (rpm >= 3000) retval = 0.85;
            else if (rpm >= 2000) retval = 0.90;
            else if (rpm >= 1000) retval = 0.85;*/
            return retval;
        }

        public void SetCompressorType(CompressorMap compressorMap)
        {
            string imagename = "Trionic5Controls.Compressormaps.t25_55_saab.JPG";
            switch (compressorMap)
            {
                case CompressorMap.T25_55:
                    imagename = "Trionic5Controls.Compressormaps.t25_55_saab.JPG";
                    toolStripComboBox1.SelectedIndex = 0;
                    break;
                case CompressorMap.T25_60:
                    imagename = "Trionic5Controls.Compressormaps.t25-60trim.JPG";
                    toolStripComboBox1.SelectedIndex = 1;
                    break;
                case CompressorMap.TD04:
                    imagename = "Trionic5Controls.Compressormaps.td04-15g-cfm.JPG";
                    toolStripComboBox1.SelectedIndex = 2;
                    break;
                case CompressorMap.TD0416T:
                    imagename = "Trionic5Controls.Compressormaps.td04h-16t-cfm.JPG";
                    toolStripComboBox1.SelectedIndex = 3;
                    break;
                case CompressorMap.TD0418T:
                    imagename = "Trionic5Controls.Compressormaps.td04h-18t-cfm.JPG";
                    toolStripComboBox1.SelectedIndex = 4;
                    break;
                case CompressorMap.TD0419T:
                    imagename = "Trionic5Controls.Compressormaps.td04h-19t-cfm.JPG";
                    toolStripComboBox1.SelectedIndex = 5;
                    break;
                case CompressorMap.TD0620G:
                    imagename = "Trionic5Controls.Compressormaps.td06h-20g-cfm.JPG";
                    toolStripComboBox1.SelectedIndex = 6;
                    break;
                case CompressorMap.GT2871R:
                    imagename = "Trionic5Controls.Compressormaps.gt2871r-48.jpg";
                    toolStripComboBox1.SelectedIndex = 7;
                    break;
                case CompressorMap.GT28RS:
                    imagename = "Trionic5Controls.Compressormaps.gt28rscompress.JPG";
                    toolStripComboBox1.SelectedIndex = 8;
                    break;
                case CompressorMap.GT3071R86:
                    imagename = "Trionic5Controls.Compressormaps.GT3071R86.jpg";
                    toolStripComboBox1.SelectedIndex = 9;
                    break;
                case CompressorMap.GT30R:
                    imagename = "Trionic5Controls.Compressormaps.gt30rcompress.JPG";
                    toolStripComboBox1.SelectedIndex = 10;
                    break;
                case CompressorMap.GT40R:
                    imagename = "Trionic5Controls.Compressormaps.gt40rcompress.JPG";
                    toolStripComboBox1.SelectedIndex = 11;
                    break;
                case CompressorMap.HX40W:
                    toolStripComboBox1.SelectedIndex = 12;
                    imagename = "Trionic5Controls.Compressormaps.hx40w.jpg";
                    break;
                case CompressorMap.S400SX371:
                    toolStripComboBox1.SelectedIndex = 13;
                    imagename = "Trionic5Controls.Compressormaps.S400SX3-71.jpg";
                    break;
                case CompressorMap.GT17:
                    toolStripComboBox1.SelectedIndex = 14;
                    imagename = "Trionic5Controls.Compressormaps.GT17.jpg";
                    break;
                case CompressorMap.T25_NG900:
                    toolStripComboBox1.SelectedIndex = 15;
                    imagename = "Trionic5Controls.Compressormaps.T25_54mm_60trim_Map.jpg";
                    break;
            }
            _compressor = compressorMap;

            Bitmap bmp = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(imagename));
            ori_width = bmp.Width;
            ori_height = bmp.Height;
            DrawLineonImage(bmp);
            scalablePictureBox1.Picture = null;
            string tempImagePath = Path.GetTempPath() + "\\T5CMImage.bmp";
            if (File.Exists(tempImagePath)) File.Delete(tempImagePath);
            bmp.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Bmp);
            Bitmap bmp2 = new Bitmap(tempImagePath);
            scalablePictureBox1.Picture = bmp2;
        }

        private void DrawLineonImage(Bitmap bmp)
        {
            Graphics graphic = System.Drawing.Graphics.FromImage(bmp);
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            float prev_x_location = 0;
            float prev_y_location = 0;
            Pen p = new Pen(Color.Red, 3);

            try
            {
                switch (_compressor)
                {
                    case CompressorMap.T25_55:
                        // set coordinates
                        x_offset = 64 * CalculateXCorrection();
                        y_offset = 865 * CalculateYCorrection();
                        x_multiplier = 20 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 396 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.T25_60:
                        x_offset = 60 * CalculateXCorrection();
                        y_offset = 867 * CalculateYCorrection();
                        x_multiplier = 17.28 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 398 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT2871R:
                        x_offset = 50 * CalculateXCorrection();
                        y_offset = 595 * CalculateYCorrection();
                        x_multiplier = 9.56 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 276.5 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT28RS:
                        // set coordinates
                        x_offset = 55 * CalculateXCorrection();
                        y_offset = 460 * CalculateYCorrection();
                        x_multiplier = 8 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 211 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT30R:
                        // set coordinates
                        x_offset = 50 * CalculateXCorrection();
                        y_offset = 463 * CalculateYCorrection();
                        x_multiplier = 6.4 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 158 * CalculateYCorrection(); // per bar
                        break;
                    // 100 cfm = 6.91 lbs/m
                    case CompressorMap.TD04:
                        x_offset = 66 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        x_multiplier = 10.45 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234.5 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.TD0416T:
                        x_offset = 64 * CalculateXCorrection();
                        y_offset = 573 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 233 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.TD0418T:
                        x_offset = 65 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234 * CalculateYCorrection(); // per bar
                        /*                        x_offset = 126 * CalculateXCorrection();
                                                y_offset = 940 * CalculateYCorrection();
                                                // 648 - 126 = 0.3 m3/s = 635 cfm = 44 lbs/m
                                                // 940 - 256 = 2 bar 
                                                x_multiplier = 11.86 * CalculateXCorrection(); // per lbs/m
                                                y_multiplier = 342 * CalculateYCorrection(); // per bar*/
                        break;
                    case CompressorMap.TD0419T:
                        x_offset = 65 * CalculateXCorrection();
                        y_offset = 576 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.27 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 234 * CalculateYCorrection(); // per bar
                        /*x_offset = 228 * CalculateXCorrection();
                        y_offset = 1627 * CalculateYCorrection();
                        // 1102 - 228 = 0.3 m3/s = 635 cfm = 44 lbs/m
                        // 1627 - 468 = 2 bar 
                        x_multiplier = 19.86 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 579.5 * CalculateYCorrection(); // per bar*/
                        break;
                    case CompressorMap.TD0620G:
                        //TODO
                        x_offset = 58 * CalculateXCorrection();
                        y_offset = 577 * CalculateYCorrection();
                        //408 = 600 cfm = 41.46 lbs/m
                        x_multiplier = 8.30 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 235 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT3071R86:
                        x_offset = 42 * CalculateXCorrection();
                        y_offset = 556 * CalculateYCorrection();
                        x_multiplier = 6.67 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 171 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.GT40R:
                        x_offset = 54 * CalculateXCorrection();
                        y_offset = 482 * CalculateYCorrection();
                        x_multiplier = 5.31 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 171 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.HX40W:
                        x_offset = 35 * CalculateXCorrection();
                        y_offset = 762 * CalculateYCorrection();
                        x_multiplier = 5.03 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 167 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.S400SX371:
                        x_offset = 45 * CalculateXCorrection();
                        y_offset = 484 * CalculateYCorrection();
                        x_multiplier = 6.713 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 102 * CalculateYCorrection(); // per bar
                        //753 - 124 = 0.72-0.08 m3/s = 0.64 m3/s =  1356.1 cfm = 93.7 lbs/m
                        // 629 = 93.7 lbs/m
                        //1 lb/min = 14.4718 cfm
                        //1 m3/s = 2118.88 cfm
                        // 648 - 126 = 0.3 m3/s = 635 cfm = 44 lbs/m
                        // 940 - 256 = 2 bar 
                        break;
                    case CompressorMap.GT17:
                        x_offset = 42 * CalculateXCorrection();
                        y_offset = 539 * CalculateYCorrection();
                        x_multiplier = 10.67 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 166 * CalculateYCorrection(); // per bar
                        break;
                    case CompressorMap.T25_NG900:
                        x_offset = 45 * CalculateXCorrection();
                        y_offset = 622 * CalculateYCorrection();
                        x_multiplier = 17.25 * CalculateXCorrection(); // per lbs/m
                        y_multiplier = 258.5 * CalculateYCorrection(); // per bar
                        break;
                }
                PointF[] pnts = new PointF[16];

                for (int i = 0; i < 16; i++)
                {
                    int rpm = rpm_points[i];
                    double boost_req = boost_request[i];
                    //to plot the point in the compressor map, first we need to calculate the flow of the engine

                    boost_req += CalculateIntakeLoss(rpm) / 14.7; // intake loss is returned in PSI

                    double displacement = 140;
                    switch (current_engineType)
                    {
                        case EngineType.Liter2:
                            displacement = 122;
                            break;
                        case EngineType.Liter23:
                            displacement = 140;
                            break;
                    }
                    double EVF = (displacement / 1728) * (rpm / 2);

                    //TODO: <GS-09032011> request to have an option to fill in efficiency per RPM range

                    double efficiency = GetEngineEfficiency(rpm);

                    try
                    {
                        //double temp_eff = ConvertToDouble(toolStripTextBox2.Text);
                        double temp_eff = 0;//ConvertToDouble(toolStripTextBox2.Text);
                        if (i == 0) temp_eff = ConvertToDouble(toolStripTextBox3.Text);
                        else if (i == 1) temp_eff = ConvertToDouble(toolStripTextBox4.Text);
                        else if (i == 2) temp_eff = ConvertToDouble(toolStripTextBox5.Text);
                        else if (i == 3) temp_eff = ConvertToDouble(toolStripTextBox6.Text);
                        else if (i == 4) temp_eff = ConvertToDouble(toolStripTextBox7.Text);
                        else if (i == 5) temp_eff = ConvertToDouble(toolStripTextBox8.Text);
                        else if (i == 6) temp_eff = ConvertToDouble(toolStripTextBox9.Text);
                        else if (i == 7) temp_eff = ConvertToDouble(toolStripTextBox10.Text);
                        else if (i == 8) temp_eff = ConvertToDouble(toolStripTextBox11.Text);
                        else if (i == 9) temp_eff = ConvertToDouble(toolStripTextBox12.Text);
                        else if (i == 10) temp_eff = ConvertToDouble(toolStripTextBox13.Text);
                        else if (i == 11) temp_eff = ConvertToDouble(toolStripTextBox14.Text);
                        else if (i == 12) temp_eff = ConvertToDouble(toolStripTextBox15.Text);
                        else if (i == 13) temp_eff = ConvertToDouble(toolStripTextBox16.Text);
                        else if (i == 14) temp_eff = ConvertToDouble(toolStripTextBox17.Text);
                        else if (i == 15) temp_eff = ConvertToDouble(toolStripTextBox18.Text);
                        if (temp_eff != 0)
                        {
                            efficiency = temp_eff / 100;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    double temperature = 20;

                    try
                    {
                        temperature = ConvertToDouble(toolStripTextBox1.Text);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    temperature = ConvertToFahrenheit(temperature);
                    temperature = 460 + temperature; // to rankin
                    double airflowlbsm = (((14.5 + (boost_req * 14.5)) * EVF * 29) / (10.73 * temperature)) * efficiency;
                    // now we have airflow and pressure, we can draw after we calculate the points
                    Console.WriteLine("airflow is " + airflowlbsm.ToString() + " at " + rpm.ToString() + " rpm and " + boost_req.ToString() + " bar with VE: " + efficiency.ToString());
                    float x_location = (float)(x_offset + airflowlbsm * x_multiplier);
                    float y_location = (float)(y_offset - boost_req * y_multiplier);
                    pnts[i].X = x_location;
                    pnts[i].Y = y_location;
                    if (i > 0)
                    {
                        // draw line
                        graphic.DrawLine(p, new PointF(prev_x_location, prev_y_location), new PointF(x_location, y_location));
                    }
                    // draw dot on the point itself
                    graphic.FillEllipse(Brushes.OrangeRed, x_location - 4, y_location - 4, 8, 8);
                    int rpm10 = rpm / 10;
                    graphic.DrawString(rpm10.ToString("D3"), this.Font, Brushes.OrangeRed, x_location, y_location);
                    prev_x_location = x_location;
                    prev_y_location = y_location;
                }
                graphic.Save();
                p.Dispose();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void tD0418TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.TD0418T);
        }

        private void tD0419TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.TD0419T);
        }

        private void gT3071r86ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.GT3071R86);
        }

        private void gT40RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.GT40R);
        }

        private void hX40wToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.HX40W);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CompressorMap map = (CompressorMap)toolStripComboBox1.SelectedIndex;
                if (map != _compressor)
                {
                    SetCompressorType(map);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // cast refresh event
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }

        }

        public void Redraw()
        {
            SetCompressorType(_compressor);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG images|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                scalablePictureBox1.PictureBox.Image.Save(sfd.FileName);
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Redraw();
            }
        }

        private void scalablePictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("Painted");
        }

        private void LoadRPMEfficiencies()
        {
            toolStripTextBox3.Text = LoadRPMEfficiency(0);
            toolStripTextBox4.Text = LoadRPMEfficiency(1);
            toolStripTextBox5.Text = LoadRPMEfficiency(2);
            toolStripTextBox6.Text = LoadRPMEfficiency(3);
            toolStripTextBox7.Text = LoadRPMEfficiency(4);
            toolStripTextBox8.Text = LoadRPMEfficiency(5);
            toolStripTextBox9.Text = LoadRPMEfficiency(6);
            toolStripTextBox10.Text = LoadRPMEfficiency(7);
            toolStripTextBox11.Text = LoadRPMEfficiency(8);
            toolStripTextBox12.Text = LoadRPMEfficiency(9);
            toolStripTextBox13.Text = LoadRPMEfficiency(10);
            toolStripTextBox14.Text = LoadRPMEfficiency(11);
            toolStripTextBox15.Text = LoadRPMEfficiency(12);
            toolStripTextBox16.Text = LoadRPMEfficiency(13);
            toolStripTextBox17.Text = LoadRPMEfficiency(14);
            toolStripTextBox18.Text = LoadRPMEfficiency(15);
        }

        private void toolStripDropDownButton2_DropDownClosed(object sender, EventArgs e)
        {
            // save the data into the registry
            SaveRPMEfficiency(0, toolStripTextBox3.Text.Replace(',', '.'));
            SaveRPMEfficiency(1, toolStripTextBox4.Text.Replace(',', '.'));
            SaveRPMEfficiency(2, toolStripTextBox5.Text.Replace(',', '.'));
            SaveRPMEfficiency(3, toolStripTextBox6.Text.Replace(',', '.'));
            SaveRPMEfficiency(4, toolStripTextBox7.Text.Replace(',', '.'));
            SaveRPMEfficiency(5, toolStripTextBox8.Text.Replace(',', '.'));
            SaveRPMEfficiency(6, toolStripTextBox9.Text.Replace(',', '.'));
            SaveRPMEfficiency(7, toolStripTextBox10.Text.Replace(',', '.'));
            SaveRPMEfficiency(8, toolStripTextBox11.Text.Replace(',', '.'));
            SaveRPMEfficiency(9, toolStripTextBox12.Text.Replace(',', '.'));
            SaveRPMEfficiency(10, toolStripTextBox13.Text.Replace(',', '.'));
            SaveRPMEfficiency(11, toolStripTextBox14.Text.Replace(',', '.'));
            SaveRPMEfficiency(12, toolStripTextBox15.Text.Replace(',', '.'));
            SaveRPMEfficiency(13, toolStripTextBox16.Text.Replace(',', '.'));
            SaveRPMEfficiency(14, toolStripTextBox17.Text.Replace(',', '.'));
            SaveRPMEfficiency(15, toolStripTextBox18.Text.Replace(',', '.'));
            if (onRefreshData != null)
            {
                onRefreshData(this, EventArgs.Empty);
            }
        }

        private void SaveRPMEfficiency(int index, string value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T5Suite2"))
            {
                string key = "RPMEFF-" + index.ToString();
                saveSettings.SetValue(key, value);
            }
        }

        private string LoadRPMEfficiency(int index)
        {
            string returnvalue = "90";
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T5Suite2"))
            {
                object o = Settings.GetValue("RPMEFF-" + index.ToString());
                if (o != null)
                {
                    try
                    {
                        returnvalue = o.ToString();
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
            }
            return returnvalue;
        }

        private void s400SX371ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCompressorType(CompressorMap.S400SX371);
        }
    }
}
