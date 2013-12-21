using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Plot3D
{
    public class Surface3DRenderer
    {
        double screenDistance, sf, cf, st, ct, R, A, B, C, D; //transformations coeficients
        double density = 0.5f;
        Color penColor = Color.Black;
        PointF startPoint = new PointF(-10, -10);
        PointF endPoint = new PointF(10, 10);
        RendererFunction function = defaultFunction;
        ColorSchema colorSchema = ColorSchema.Autumn;
        private double m_maxdata_element = 0;
        private double m_mindata_element = 0;
        private double m_correction_percentage = 1;
        private bool m_isRedWhite = false;
        int m_highlighted_x_value = -1;
        int m_highlighted_y_value = -1;
        public bool IsRedWhite
        {
            get { return m_isRedWhite; }
            set { m_isRedWhite = value; }
        }

        public double Correction_percentage
        {
            get { return m_correction_percentage; }
            set { m_correction_percentage = value; }
        }

        int[] x_axisvalues;

        public int[] X_axisvalues
        {
            get { return x_axisvalues; }
            set { x_axisvalues = value; }
        }
        int[] y_axisvalues;

        public int[] Y_axisvalues
        {
            get { return y_axisvalues; }
            set { y_axisvalues = value; }
        }

        string m_xaxis_descr = "x-axis";

        public string Xaxis_descr
        {
            get { return m_xaxis_descr; }
            set { m_xaxis_descr = value; }
        }
        string m_yaxis_descr = "y-axis";

        public string Yaxis_descr
        {
            get { return m_yaxis_descr; }
            set { m_yaxis_descr = value; }
        }
        string m_zaxis_descr = "z-axis";

        public string Zaxis_descr
        {
            get { return m_zaxis_descr; }
            set { m_zaxis_descr = value; }
        }



        #region Properties

        /// <summary>
        /// Surface spanning net density
        /// </summary>
        public double Density
        {
            get { return density; }
            set { density = value; }
        }

        /// <summary>
        /// Quadrilateral pen color
        /// </summary>
        public Color PenColor
        {
            get { return penColor; }
            set { penColor = value; }
        }

        public PointF StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public PointF EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public RendererFunction Function
        {
            get { return function; }
            set { function = value; }
        }

        public ColorSchema ColorSchema
        {
            get { return colorSchema; }
            set { colorSchema = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Surface3DRenderer"/> class. Calculates transformations coeficients.
        /// </summary>
        /// <param name="obsX">Observator's X position</param>
        /// <param name="obsY">Observator's Y position</param>
        /// <param name="obsZ">Observator's Z position</param>
        /// <param name="xs0">X coordinate of screen</param>
        /// <param name="ys0">Y coordinate of screen</param>
        /// <param name="screenWidth">Drawing area width in pixels.</param>
        /// <param name="screenHeight">Drawing area height in pixels.</param>
        /// <param name="screenDistance">The screen distance.</param>
        /// <param name="screenWidthPhys">Width of the screen in meters.</param>
        /// <param name="screenHeightPhys">Height of the screen in meters.</param>
        public Surface3DRenderer(double obsX, double obsY, double obsZ, int xs0, int ys0, int screenWidth, int screenHeight, double screenDistance, double screenWidthPhys, double screenHeightPhys)
        {
            ReCalculateTransformationsCoeficients(obsX, obsY, obsZ, xs0, ys0, screenWidth, screenHeight, screenDistance, screenWidthPhys, screenHeightPhys);
        }

        public void ReCalculateTransformationsCoeficients(double obsX, double obsY, double obsZ, int xs0, int ys0, int screenWidth, int screenHeight, double screenDistance, double screenWidthPhys, double screenHeightPhys)
        {
            double r1, a;

            if (screenWidthPhys <= 0)//when screen dimensions are not specified
                screenWidthPhys = screenWidth * 0.0257 / 72.0;        //0.0257 m = 1 inch. Screen has 72 px/inch
            if (screenHeightPhys <= 0)
                screenHeightPhys = screenHeight * 0.0257 / 72.0;

            r1 = obsX * obsX + obsY * obsY;
            a = Math.Sqrt(r1);//distance in XY plane
            R = Math.Sqrt(r1 + obsZ * obsZ);//distance from observator to center
            if (a != 0) //rotation matrix coeficients calculation
            {
                sf = obsY / a;//sin( fi)
                cf = obsX / a;//cos( fi)
            }
            else
            {
                sf = 0;
                cf = 1;
            }
            st = a / R;//sin( teta)
            ct = obsZ / R;//cos( teta)

            //linear tranfrormation coeficients
            A = screenWidth / screenWidthPhys;
            B = xs0 + A * screenWidthPhys / 2.0;
            C = -(double)screenHeight / screenHeightPhys;
            D = ys0 - C * screenHeightPhys / 2.0;

            this.screenDistance = screenDistance;
        }

        /// <summary>
        /// Performs projection. Calculates screen coordinates for 3D point.
        /// </summary>
        /// <param name="x">Point's x coordinate.</param>
        /// <param name="y">Point's y coordinate.</param>
        /// <param name="z">Point's z coordinate.</param>
        /// <returns>Point in 2D space of the screen.</returns>
        public PointF Project(double x, double y, double z)
        {
            double xn, yn, zn;//point coordinates in computer's frame of reference

            //transformations
            xn = -sf * x + cf * y;
            yn = -cf * ct * x - sf * ct * y + st * z;
            zn = -cf * st * x - sf * st * y - ct * z + R;

            if (zn == 0) zn = 0.01;

            //Tales' theorem
            return new PointF((float)(A * xn * screenDistance / zn + B), (float)(C * yn * screenDistance / zn + D));
        }

        private System.Data.DataTable m_mapdata = new System.Data.DataTable();

        public System.Data.DataTable Mapdata
        {
          get {
              return m_mapdata; 
          }
          set
          {
              m_mapdata = value;
              foreach (System.Data.DataRow dr in m_mapdata.Rows)
              {
                  foreach (object cell in dr.ItemArray)
                  {
                      try
                      {
                          double d = Convert.ToDouble(cell);
                          if (d > m_maxdata_element) m_maxdata_element = d;
                          if (d < m_mindata_element) m_mindata_element = d;

                      }
                      catch (Exception E)
                      {
                          Console.WriteLine("Set m_mapdata: "  + E.Message);
                      }
                  }
              }
          }
        }

        private System.Data.DataTable m_mapcomparedata = new System.Data.DataTable();

        public System.Data.DataTable Mapcomparedata
        {
            get
            {
                return m_mapcomparedata;
            }
            set
            {
                m_mapcomparedata = value;
                foreach (System.Data.DataRow dr in m_mapdata.Rows)
                {
                    foreach (object cell in dr.ItemArray)
                    {
                        try
                        {
                            double d = Convert.ToDouble(cell);
                            if (d > m_maxdata_element) m_maxdata_element = d;

                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Set m_mapdata: " + E.Message);
                        }
                    }
                }
            }
        }

        private System.Data.DataTable m_maporiginaldata = new System.Data.DataTable();

        public System.Data.DataTable Maporiginaldata
        {
            get
            {
                return m_maporiginaldata;
            }
            set
            {
                m_maporiginaldata = value;
                foreach (System.Data.DataRow dr in m_mapdata.Rows)
                {
                    foreach (object cell in dr.ItemArray)
                    {
                        try
                        {
                            double d = Convert.ToDouble(cell);
                            if (d > m_maxdata_element) m_maxdata_element = d;

                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Set m_mapdata: " + E.Message);
                        }
                    }
                }
            }
        }


        public void DrawXAxis(string description, double[] values)
        {

        }

        public double GetDataFromTable(double xi, double yi)
        {
            double retval = 0;
            //Console.WriteLine("xi A = " + xi.ToString() + " yi = "  + yi.ToString()); 
            xi = (m_mapdata.Columns.Count-1) - xi;
            //Console.WriteLine("xi B = " + xi.ToString());
            if (xi < m_mapdata.Columns.Count && yi < m_mapdata.Rows.Count)
            {
                try
                {
                    if (m_mapdata.Rows[(int)yi][(int)xi] != DBNull.Value)
                    {
                        
                        retval = Convert.ToDouble(m_mapdata.Rows[(int)yi][(int)xi]);
                        if (retval > 0xF000)
                        {
                            retval = 0x10000 - retval;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                Console.WriteLine("xi C = " + xi.ToString() + " yi C = " + yi.ToString());
            }
            return retval;
        }
        public double GetDataFromOriginalTable(double xi, double yi)
        {
            double retval = 0;
            //Console.WriteLine("xi A = " + xi.ToString() + " yi = "  + yi.ToString()); 
            xi = (m_maporiginaldata.Columns.Count - 1) - xi;
            //Console.WriteLine("xi B = " + xi.ToString());
            if (xi < m_maporiginaldata.Columns.Count && yi < m_maporiginaldata.Rows.Count)
            {
                try
                {
                    if (m_maporiginaldata.Rows[(int)yi][(int)xi] != DBNull.Value)
                    {

                        retval = Convert.ToDouble(m_maporiginaldata.Rows[(int)yi][(int)xi]);
                        if (retval > 0xF000)
                        {
                            retval = 0x10000 - retval;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                //Console.WriteLine("xi C = " + xi.ToString() + " yi C = " + yi.ToString());
            }
            return retval;
        }

        public double GetDataFromCompareTable(double xi, double yi)
        {
            double retval = 0;
            //Console.WriteLine("xi A = " + xi.ToString() + " yi = "  + yi.ToString()); 
            xi = (m_mapcomparedata.Columns.Count - 1) - xi;
            //Console.WriteLine("xi B = " + xi.ToString());
            if (xi < m_mapcomparedata.Columns.Count && yi < m_mapcomparedata.Rows.Count)
            {
                try
                {
                    if (m_mapcomparedata.Rows[(int)yi][(int)xi] != DBNull.Value)
                    {

                        retval = Convert.ToDouble(m_mapcomparedata.Rows[(int)yi][(int)xi]);
                        if (retval > 0xF000)
                        {
                            retval = 0x10000 - retval;
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
            else
            {
                //Console.WriteLine("xi C = " + xi.ToString() + " yi C = " + yi.ToString());
            }
            return retval;
        }
        private SolidBrush GetDrawingColor(double b)
        {
            //return new SolidBrush(Color.LightBlue);
            b *= 255;
            //b /= m_MaxValueInTable;
            if (m_maxdata_element != 0)
            {
                b /= m_maxdata_element;
            }
            int green = 128;
            int blue = 128;
            Color c = Color.White;
            if (b < 0) b = 0;
            if (b > 255) b = 255;
            if (Double.IsNaN(b)) b = 0;
            if (!m_isRedWhite)
            {
                blue = 0;
                green = 255 - (int)b;
                c = Color.FromArgb((int)b, green, blue);
            }
            else
            {
                if (b < 0) b = 0;
                c = Color.FromArgb((int)b, Color.Red);
            }
            SolidBrush sb = new SolidBrush(c);
            return sb;
        }

        public void RenderSurface(Graphics graphics, PointF LastMouseHoverPoint)
        {
            //double tempi = 0;
            PointF[] axis_bounds = new PointF[7];

            SolidBrush[] brushes = new SolidBrush[colorSchema.Length];
            for (int i = 0; i < brushes.Length; i++)
                brushes[i] = new SolidBrush(colorSchema[i]);

            double z1, z2;
            PointF[] polygon = new PointF[4];
            PointF[] polygonoriginal = new PointF[4];
            PointF[] polygoncompare = new PointF[4];

            double xi = startPoint.X, yi, minZ = double.PositiveInfinity, maxZ = double.NegativeInfinity;
            double[,] mesh = new double[(int)((endPoint.X - startPoint.X) / density + 2), (int)((endPoint.Y - startPoint.Y) / density + 2)];
            double[,] meshCompare = new double[(int)((endPoint.X - startPoint.X) / density + 2), (int)((endPoint.Y - startPoint.Y) / density + 2)];
            double[,] meshOriginal = new double[(int)((endPoint.X - startPoint.X) / density + 2), (int)((endPoint.Y - startPoint.Y) / density + 2)];
            PointF[,] meshF = new PointF[mesh.GetLength(0), mesh.GetLength(1)];
            PointF[,] meshFCompare = new PointF[mesh.GetLength(0), mesh.GetLength(1)];
            PointF[,] meshFOriginal = new PointF[mesh.GetLength(0), mesh.GetLength(1)];

            // assen tekenen
            PointF[] axispolygon = new PointF[4];

            for (int z = (int)Math.Floor(m_mindata_element + 1)/*1*/; z <= (int)Math.Ceiling(m_maxdata_element); z++)
            {
                string zvalue = z.ToString();
                try
                {
                    float dblzvalue = z * (1F/(float)m_correction_percentage);
                    zvalue = dblzvalue.ToString("F0");
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                PointF p = Project(mesh.GetLength(0)+1, 0, z);
                graphics.DrawString(zvalue, new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, p);
            }

            for (int t = 0; t < mesh.GetLength(0); t++)
            {
                for (int yt = 0; yt < (int)Math.Ceiling(m_maxdata_element); yt++)
                {
                    try
                    {
                        axispolygon[1] = Project(t, 0, yt);
                        axispolygon[0] = Project(t + 1, 0, yt);
                        axispolygon[2] = Project(t, 0, yt + 1);
                        axispolygon[3] = Project(t + 1, 0, yt + 1);
                        graphics.DrawPolygon(Pens.DarkGray, axispolygon);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
             
            }
            // assen tekenen
            for (int t = 0; t < mesh.GetLength(1) ; t++)
            {
                for (int yt = 0; yt < (int)Math.Ceiling(m_maxdata_element); yt++)
                {
                    try
                    {
                        axispolygon[1] = Project(0, t, yt);
                        axispolygon[0] = Project(0, t + 1, yt);
                        axispolygon[2] = Project(0, t, yt + 1);
                        axispolygon[3] = Project(0, t + 1, yt + 1);
                        graphics.DrawPolygon(Pens.DarkGray, axispolygon);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
                string yaxis = t.ToString();
                if (y_axisvalues != null)
                {
                    yaxis = "";
                    if (y_axisvalues.Length > t)
                    {
                        yaxis = y_axisvalues.GetValue((y_axisvalues.Length - 1) - (t)).ToString();
                    }
                }
                
                PointF p = Project( mesh.GetLength(0) + 1,t + 1, 0);
                graphics.DrawString(yaxis, new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, p);
            }
            for (int t = 0; t < mesh.GetLength(0) ; t++)
            {
                for (int yt = 0; yt < mesh.GetLength(1); yt++)
                {
                    try
                    {
                        axispolygon[1] = Project(t, yt, 0);
                        axispolygon[0] = Project(t, yt + 1, 0);
                        axispolygon[2] = Project(t + 1, yt, 0);
                        axispolygon[3] = Project(t + 1, yt + 1, 0);
                        graphics.DrawPolygon(Pens.DarkGray, axispolygon);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
                string xaxis = t.ToString();
                if (x_axisvalues != null)
                {
                    xaxis = "";
                    if (x_axisvalues.Length > t)
                    {
                        xaxis = x_axisvalues.GetValue((x_axisvalues.Length - 1) - t).ToString(); 
                    }
                }
                PointF p = Project(t + 1, mesh.GetLength(1) + 1 ,0);
                graphics.DrawString(xaxis, new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, p);
            }


            for (int x = 0; x < mesh.GetLength(0); x++)
            {
                yi = startPoint.Y;
                for (int y = 0; y < mesh.GetLength(1); y++)
                {
                    double zz = 0;
                    double zzorig = 0;
                    double zzcompare = 0;

                    if (x == (mesh.GetLength(0) - 1) && y == (mesh.GetLength(1) - 1))
                    {
                        zz = GetDataFromTable(xi - 1, yi - 1);// ;//function(xi, yi);
                        zzorig = GetDataFromOriginalTable(xi - 1, yi - 1);// ;//function(xi, yi);
                        zzcompare = GetDataFromCompareTable(xi - 1, yi - 1);// ;//function(xi, yi);
                    }
                    else if (x == (mesh.GetLength(0) - 1))
                    {
                        zz = GetDataFromTable(xi - 1, yi);// ;//function(xi, yi);
                        zzorig = GetDataFromOriginalTable(xi - 1, yi);// ;//function(xi, yi);
                        zzcompare = GetDataFromCompareTable(xi - 1, yi);// ;//function(xi, yi);
                    }
                    else if (y == (mesh.GetLength(1) - 1))
                    {
                        zz = GetDataFromTable(xi, yi - 1);// ;//function(xi, yi);
                        zzorig = GetDataFromOriginalTable(xi, yi - 1);// ;//function(xi, yi);
                        zzcompare = GetDataFromCompareTable(xi, yi - 1);// ;//function(xi, yi);
                    }
                    else
                    {
                        zz = GetDataFromTable(xi, yi);// ;//function(xi, yi);
                        zzorig = GetDataFromOriginalTable(xi, yi);// ;//function(xi, yi);
                        zzcompare = GetDataFromCompareTable(xi, yi);// ;//function(xi, yi);
                    }

//                    tempi += 0.01;
                    mesh[x, y] = zz;
                    meshF[x, y] = Project(xi, yi, zz);
                    meshCompare[x, y] = zzcompare;
                    meshFCompare[x, y] = Project(xi, yi, zzcompare);
                    meshOriginal[x, y] = zzorig;
                    meshFOriginal[x, y] = Project(xi, yi, zzorig);

                    yi += density;

                    if (minZ > zz) minZ = zz;
                    if (maxZ < zz) maxZ = zz;
                }
                xi += density;
            }

            double cc = (maxZ - minZ) / (brushes.Length - 1.0);
            SolidBrush sborig = new SolidBrush(Color.FromArgb(40, Color.Blue));
            Pen penorig = new Pen(Color.FromArgb(30, Color.Blue));
            SolidBrush sbcomp = new SolidBrush(Color.FromArgb(50, Color.DimGray));
            Pen pencomp = new Pen(Color.FromArgb(40, Color.DimGray));

            using (Pen pen = new Pen(penColor))
                for (int x = 0; x < mesh.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < mesh.GetLength(1) - 1; y++)
                    {
                        z1 = mesh[x, y];
                        z2 = mesh[x, y + 1];
                        //z2 = mesh[x + 1, y + 1];
                        polygon[0] = meshF[x, y];
                        polygon[1] = meshF[x, y + 1];
                        polygon[2] = meshF[x + 1, y + 1];
                        polygon[3] = meshF[x + 1, y];
                        polygonoriginal[0] = meshFOriginal[x, y];
                        polygonoriginal[1] = meshFOriginal[x, y + 1];
                        polygonoriginal[2] = meshFOriginal[x + 1, y + 1];
                        polygonoriginal[3] = meshFOriginal[x + 1, y];

                        polygoncompare[0] = meshFCompare[x, y];
                        polygoncompare[1] = meshFCompare[x, y + 1];
                        polygoncompare[2] = meshFCompare[x + 1, y + 1];
                        polygoncompare[3] = meshFCompare[x + 1, y];
                        /*
                        if (x == mesh.GetLength(0) - 1)
                        {
                            if (y == mesh.GetLength(1) - 1)
                            {
                                z1 = mesh[x, y];
                                z2 = mesh[x, y];

                            }
                            else
                            {
                                z1 = mesh[x, y];
                                z2 = mesh[x, y + 1];

                                polygon[0] = meshF[x, y];
                                //polygon[0].Y = 0;
                                
                                //polygon[0].Y += meshF[x, y].Y - meshF[x - 1, y].Y;
                                polygon[1] = meshF[x, y + 1];
                                //polygon[1].Y += meshF[x, y].Y - meshF[x - 1, y].Y;
                                polygon[2] = meshF[x, y + 1];
                                polygon[2].X += meshF[x, y].X - meshF[x - 1, y].X;
                                polygon[2].Y += meshF[x, y + 1].Y - meshF[x , y + 1].Y;

                                //polygon[2].Y += meshF[x, y].Y -meshF[x - 1, y].Y;
                                polygon[3] = meshF[x, y];
                                polygon[3].X += meshF[x, y].X - meshF[x - 1, y ].X;
                                polygon[3].Y += meshF[x, y].Y - meshF[x , y ].Y;
                                //polygon[3].Y += meshF[x, y].Y - meshF[x- 1, y ].Y;
                            }
                           
                        }
                        else if (y == mesh.GetLength(1) - 1)
                        {
                            if (x == mesh.GetLength(0) - 1)
                            {
                                z1 = mesh[x, y];
                                z2 = mesh[x, y];

                            }
                            else
                            {
                                z1 = mesh[x, y];
                                z2 = mesh[x, y];

                                polygon[0] = meshF[x, y];
                                polygon[1] = meshF[x, y];
//                                polygon[1].Y += 10;// meshF[x, y].Y - meshF[x, y - 1].Y;
                                polygon[1].X += meshF[x, y].X - meshF[x, y - 1].X;
                                polygon[2] = meshF[x + 1, y];
//                                polygon[2].Y += 10;// meshF[x, y].Y - meshF[x, y - 1].Y;
                                polygon[2].X += meshF[x, y].X - meshF[x, y - 1].X;
                                polygon[3] = meshF[x + 1, y];

                                // moet uitbreiden naar einde
                            }
                        }
                        else
                        {
                            z1 = mesh[x, y];
                            z2 = mesh[x, y + 1];
                            //z2 = mesh[x + 1, y + 1];
                            /*
                            polygon[0] = meshF[x, y];
                            polygon[1] = meshF[x, y + 1];
                            polygon[2] = meshF[x + 1, y + 1];
                            polygon[3] = meshF[x + 1, y];
                        }*/

                        /*if (x == 0 && y == 0)
                        {
                            axis_bounds[0] = polygon[0];
                        }
                        else if (x == (mesh.GetLength(0) - 2) && y == 0)
                        {
                            axis_bounds[1] = polygon[0];
                        }
                        else if (x == 0 && y == (mesh.GetLength(1) - 2))
                        {
                            //                            graphics.DrawString("YMAX", new Font(FontFamily.GenericSerif, 8), Brushes.Black, polygon[2]);
                            axis_bounds[2] = polygon[0];

                        }
                        else if (x == (mesh.GetLength(0) - 2) && y == (mesh.GetLength(1) - 2))
                        {
                            //                            graphics.DrawString("MAX", new Font(FontFamily.GenericSerif, 8), Brushes.Black, polygon[3]);
                            axis_bounds[3] = polygon[0];
                        }*/


                        //graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        //graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

                        if ((int)(((z1 + z2) / 2.0 - minZ) / cc) < brushes.Length)
                        {
                            try
                            {
                                int brushnumber = (brushes.Length - 1) - (int)(((z1 + z2) / 2.0 - minZ) / cc);
                                //brushes[brushnumber].Color.A = 
                                //int red = GetDrawingColor(z2);
                                
                                //Console.WriteLine(z1.ToString() + " = red: " + red.ToString());
                                //graphics.FillPolygon(brushes[brushnumber], polygon);
                                if (!Double.IsNaN(z1))
                                {
                                    if (x == m_highlighted_x_value && y == m_highlighted_y_value)
                                    {
                                        graphics.FillPolygon(Brushes.Aqua, polygon);
                                    }
                                    else
                                    {
                                        graphics.FillPolygon(GetDrawingColor(z1), polygon);
                                    }
                                    if (m_maporiginaldata.Rows.Count > 0)
                                    {
                                        graphics.FillPolygon(sborig, polygonoriginal);
                                        graphics.DrawPolygon(penorig, polygonoriginal);

                                    }
                                    if (m_mapcomparedata.Rows.Count > 0)
                                    {
                                        graphics.FillPolygon(sbcomp, polygoncompare);
                                        graphics.DrawPolygon(pencomp, polygoncompare);
                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                        else
                        {
                            graphics.FillPolygon(Brushes.Blue, polygon);
                        }
                        graphics.DrawPolygon(pen, polygon);
                    }
                }
            for (int i = 0; i < brushes.Length; i++)
                brushes[i].Dispose();

            // axis labels
            PointF xaxispoint = Project(mesh.GetLength(0) / 2, mesh.GetLength(1) + 4, 0);
            graphics.DrawString(m_xaxis_descr, new Font(FontFamily.GenericSansSerif, 8), Brushes.MidnightBlue, xaxispoint);
            
            /*float width = graphics.MeasureString(m_xaxis_descr, new Font(FontFamily.GenericSansSerif,8)).Width;
            float height = graphics.MeasureString(m_xaxis_descr, new Font(FontFamily.GenericSansSerif, 8)).Height;
            graphics.TranslateTransform( width, 0);
            graphics.RotateTransform(45);
            graphics.DrawString(m_xaxis_descr, new Font(FontFamily.GenericSansSerif, 8), Brushes.MidnightBlue, xaxispoint);*/
            PointF yaxispoint = Project(mesh.GetLength(0) + 4, mesh.GetLength(1) / 2, 0);
            graphics.DrawString(m_yaxis_descr, new Font(FontFamily.GenericSansSerif, 8), Brushes.MidnightBlue, yaxispoint);

            PointF zaxispoint = Project(mesh.GetLength(0) + 4, 0, m_maxdata_element/2);
            graphics.DrawString(m_zaxis_descr, new Font(FontFamily.GenericSansSerif, 8), Brushes.MidnightBlue, zaxispoint);


            /*if (LastMouseHoverPoint.X != 0 && LastMouseHoverPoint.Y != 0)
            {
                graphics.FillEllipse(Brushes.YellowGreen, LastMouseHoverPoint.X, LastMouseHoverPoint.Y, 5, 5);
            }*/
      
        }

        public static RendererFunction GetFunctionHandle(string formula)
        {
            CompiledFunction fn = FunctionCompiler.Compile(2, formula);
            return new RendererFunction(delegate(double x, double y)
            {
                return fn(x, y);
            });
        }

        public void SetFunction(string formula)
        {
            function = GetFunctionHandle(formula);
        }

        private static double defaultFunction(double a, double b)
        {
            double an = a, bn = b, anPlus1;
            short iter = 0;
            do
            {
                anPlus1 = (an + bn) / 2.0;
                bn = Math.Sqrt(an * bn);
                an = anPlus1;
                if (iter++ > 1000) return an;
            } while (Math.Abs(an - bn)<0.1);
            return an;
        }

        internal bool GetMousePoint(int x, int y, out PointF tableposition, out double  val)
        {
            PointF p;
            tableposition = new Point();
            int rowcount = 0;
            val = 0;

            foreach (System.Data.DataRow dr in m_mapdata.Rows)
            {
                int colcount = 0;
                foreach (object o in dr.ItemArray)
                {
                    try
                    {
                        p = Project((double)colcount, (double)rowcount, Convert.ToDouble(o));
                       // Console.WriteLine(rowcount.ToString() + " / " + colcount.ToString() + " = " + Convert.ToDouble(o).ToString() + " x = " + p.X.ToString() + " y = " + p.Y.ToString());
                        int xdiff = (int)Math.Abs(x - p.X);
                        int ydiff = (int)Math.Abs(y - p.Y);
                        if (xdiff < 5 && ydiff < 5)
                        {
                            //Console.WriteLine(rowcount.ToString() + " / " + colcount.ToString() + " = " + Convert.ToDouble(o).ToString() + " x = " + p.X.ToString() + " y = " + p.Y.ToString());
                            tableposition = new PointF((float)rowcount, (float)colcount);
                            val = Convert.ToDouble(o) * (1/m_correction_percentage);
                            return true;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("GetMousePoint: :" + E.Message);
                    }
                    colcount++;
                }
                rowcount++;
            }
            return false;
        }

        internal void RenderGraph(Graphics graphics)
        {
            //double tempi = 0;
            //double z1, z2;
            PointF[] polygon = new PointF[4];

            for (int z = 1; z <= (int)Math.Ceiling(m_maxdata_element); z++)
            {
                string zvalue = z.ToString();
                try
                {
                    float dblzvalue = z * (1F / (float)m_correction_percentage);
                    zvalue = dblzvalue.ToString("F0");
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                PointF p = Project(0, 0, z);
                graphics.DrawString(zvalue, new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, p);
            }
            graphics.DrawString("0", new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, Project(0,0,0));
            for (int y = 1; y <= m_mapdata.Rows.Count; y++)
            {
                string yvalue = y.ToString();
                try
                {
                    yvalue = y_axisvalues.GetValue((y_axisvalues.Length - 1) - (y-1)).ToString();
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                PointF p = Project(0, y, -1);
                graphics.DrawString(yvalue, new Font(FontFamily.GenericSansSerif, 6), Brushes.Black, p);
            }
            PointF prev_point = new PointF(0, 0);
            for (int r = 0; r < m_mapdata.Rows.Count; r++)
            {
                try
                {
                    if (prev_point.X != 0 && prev_point.Y != 0)
                    {
                        graphics.DrawLine(Pens.MidnightBlue, prev_point, Project(0, (r + 1), Convert.ToDouble(m_mapdata.Rows[r][0])));
                        prev_point = Project(0, (r + 1), Convert.ToDouble(m_mapdata.Rows[r][0]));
                    }
                    else
                    {
                        prev_point = Project(0, (r + 1), Convert.ToDouble(m_mapdata.Rows[r][0]));
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }
        }

        internal void ClearHighlightInGraph()
        {
            m_highlighted_x_value = -1;
            m_highlighted_y_value = -1;
        }


        internal void HighlightInGraph(int x_index, int y_index)
        {
            m_highlighted_x_value = (x_axisvalues.Length - 1) - x_index;
            m_highlighted_y_value = (y_axisvalues.Length - 1) - y_index;
        }
    }

    public delegate double RendererFunction(double x, double y);

    public struct Point3D
    {
        public double x, y, z;

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}