using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class PressureToTorque
    {

        /*
0.2 bar = 197,5 Nm
0.4 bar = 247,5 Nm
0.6 bar = 287,5 Nm
0.8 bar = 340 Nm
1.0 bar = 377,5 Nm
1.2 bar = 430 Nm
1.4 bar = 472,5 Nm
1.6 bar = 533,3333333 Nm
1.8 bar = 600 Nm
 * */

        public double CalculateTorqueFromPressure(double pressure, TurboType turbo)
        {
            double tq = 0;
            double[] matrix = new double[30] { 197.5, 247.5, 287.5, 340, 365, 410, 460, 533.3, 600, 640,
                                               197.5, 247.5, 287.5, 340, 365, 410, 460, 533.3, 600, 640,
                                               197.5, 247.5, 287.5, 340, 365, 410, 460, 533.3, 600, 640};
            double[] x_axis = new double[10] { 0.2, 0.4, 0.6, 0.8, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0 };
            double[] y_axis = new double[3] { 0, 1, 2 };

            if (turbo == TurboType.Stock)
            {
                matrix = new double[30] {      160, 210, 240, 270, 300, 360, 410, 440, 450, 460,
                                               160, 210, 240, 270, 300, 360, 410, 440, 450, 460,
                                               160, 210, 240, 270, 300, 360, 410, 440, 450, 460};
            }
            else if (turbo == TurboType.TD0415T)
            {
                matrix = new double[30] {      170, 220, 260, 310, 340, 400, 450, 490, 540, 560,
                                               170, 220, 260, 310, 340, 400, 450, 490, 540, 560,
                                               170, 220, 260, 310, 340, 400, 450, 490, 540, 560};
            }
            else if (turbo == TurboType.TD0419T || turbo == TurboType.GT28BB || turbo == TurboType.GT28RS)
            {
                matrix = new double[30] {      210, 260, 300, 360, 400, 450, 490, 540, 590, 620,
                                               210, 260, 300, 360, 400, 450, 490, 540, 590, 620,
                                               210, 260, 300, 360, 400, 450, 490, 540, 590, 620};
            }
            else if (turbo == TurboType.GT3071R || turbo == TurboType.HX35w)
            {
                matrix = new double[30] {      250, 300, 340, 390, 440, 480, 520, 570, 610, 650,
                                               250, 300, 340, 390, 440, 480, 520, 570, 610, 650,
                                               250, 300, 340, 390, 440, 480, 520, 570, 610, 650};
            }
            else if (turbo == TurboType.HX40w || turbo == TurboType.S400SX371)
            {
                matrix = new double[30] {      270, 320, 360, 410, 460, 500, 540, 580, 620, 670,
                                               270, 320, 360, 410, 460, 500, 540, 580, 620, 670,
                                               270, 320, 360, 410, 460, 500, 540, 580, 620, 670};
            }

            tq = Handle_tables(1, pressure, 3, 10, matrix, y_axis, x_axis);
            return tq;
        }


        public double CalculatePressureFromTorque(double torque, TurboType turbo)
        {
            double pressure = 0;
            double[] matrix = new double[30] { 0.2, 0.4, 0.6, 0.8, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0,
                                               0.2, 0.4, 0.6, 0.8, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0,
                                               0.2, 0.4, 0.6, 0.8, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0};
            double[] x_axis = new double[10] { 197.5, 247.5, 287.5, 340, 365, 410, 460, 533.3, 600, 640 };
            if (turbo == TurboType.Stock)
            {
                x_axis = new double[10] { 160, 210, 250, 300, 330, 390, 430, 440, 450, 460 };
            }
            else if (turbo == TurboType.TD0415T)
            {
                x_axis = new double[10] { 170, 220, 260, 310, 340, 400, 450, 490, 540, 560 };
            }
            else if (turbo == TurboType.TD0419T || turbo == TurboType.GT28BB || turbo == TurboType.GT28RS)
            {
                x_axis = new double[10] { 210, 260, 300, 360, 400, 450, 490, 540, 590, 620 };
            }
            else if (turbo == TurboType.GT3071R || turbo == TurboType.HX35w)
            {
                x_axis = new double[10] { 250, 300, 340, 390, 440, 480, 520, 570, 610, 650 };
            }
            else if (turbo == TurboType.HX40w || turbo == TurboType.S400SX371)
            {
                x_axis = new double[10] { 270, 320, 360, 410, 460, 500, 540, 580, 620, 670 };
            }
            double[] y_axis = new double[3] { 0, 1, 2 };
            pressure = Handle_tables(1, torque, 3, 10, matrix, y_axis, x_axis);
            return pressure;
        }

        private double Handle_tables(double y_value, double x_value, int y_count, int x_count, double[] matrix, double[] y_axis, double[] x_axis)
        {
            double value;
            double tmp1;
            double tmp2;
            double tmp3, vx, vy;
            int x_indx;
            int y_indx;
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
            value = vx;// ((vy + vx) / 2);
            // value=interpolate2(,tmp1);

            return value;
        }


        private double interpolate2(double tmp1, double tmp2, double tmp3, double tmp4)
        {
            double retval = 0;
            try
            {
                if (tmp2 != 0)
                {
                    retval = (((tmp1 * tmp3) / tmp2) + tmp4);
                }
                else
                {
                    retval = tmp4;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;

        }
    }
}
