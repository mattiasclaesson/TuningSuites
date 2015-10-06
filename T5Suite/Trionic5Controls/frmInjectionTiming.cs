using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace Trionic5Controls
{
    public enum InjectionType : int
    {
        Normal = 0,
        Knocking,
        Idle
    }
    public partial class frmInjectionTiming : DevExpress.XtraEditors.XtraForm
    {
        private InjectionType m_viewtype = InjectionType.Normal;

        private MapSensorType m_mapSensor = MapSensorType.MapSensor25;

        public MapSensorType MapSensor
        {
            get { return m_mapSensor; }
            set { m_mapSensor = value; }
        }

        private int batt_volt = 13;
        
        private int min_tid = 0;


        public int Min_tid
        {
            get { return min_tid; }
            set { min_tid = value; }
        }

        private int[] batt_korr_tab;

        public int[] Batt_korr_tab
        {
            get { return batt_korr_tab; }
            set { batt_korr_tab = value; }
        }


        private int[] temp_steg;

        public int[] Temp_steg
        {
            get { return temp_steg; }
            set { temp_steg = value; }
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
        private int[] lufttemp_steg;

        public int[] Lufttemp_steg
        {
            get { return lufttemp_steg; }
            set { lufttemp_steg = value; }
        }

        private int[] luft_kompfak;

        public int[] Luft_kompfak
        {
            get { return luft_kompfak; }
            set { luft_kompfak = value; }
        }


        private int[] lufttemp_tab;

        public int[] Lufttemp_tab
        {
            get { return lufttemp_tab; }
            set { lufttemp_tab = value; }
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
        private int[] fuel_knock_map_x_axis;

        public int[] Fuel_knock_map_x_axis
        {
            get { return fuel_knock_map_x_axis; }
            set { fuel_knock_map_x_axis = value; }
        }
        private int[] fuel_knock_map_y_axis;

        public int[] Fuel_knock_map_y_axis
        {
            get { return fuel_knock_map_y_axis; }
            set { fuel_knock_map_y_axis = value; }
        }

        private byte[] insp_mat;

        public byte[] Insp_mat
        {
            get { return insp_mat; }
            set { insp_mat = value; }
        }
        private byte[] fuel_knock_mat;

        public byte[] Fuel_knock_mat
        {
            get { return fuel_knock_mat; }
            set { fuel_knock_mat = value; }
        }
        private byte[] idle_fuel_map;

        public byte[] Idle_fuel_map
        {
            get { return idle_fuel_map; }
            set { idle_fuel_map = value; }
        }
        private int[] idle_fuel_y_axis;

        public int[] Idle_fuel_y_axis
        {
            get { return idle_fuel_y_axis; }
            set { idle_fuel_y_axis = value; }
        }


        private int[] idle_fuel_x_axis;

        public int[] Idle_fuel_x_axis
        {
            get { return idle_fuel_x_axis; }
            set { idle_fuel_x_axis = value; }
        }

        private int inj_konst = 21;

        public int Inj_konst
        {
            get { return inj_konst; }
            set
            {
                inj_konst = value;
                spinEdit1.EditValue = inj_konst;
            }
        }

        private int m_iat = 46; // 60 degrees

        public frmInjectionTiming()
        {
            InitializeComponent();
            gridView1.IndicatorWidth = 40;
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
            Console.WriteLine("looked for : " + axisvalue.ToString() + " found idx in kyltemp_steg : " + index.ToString() + " value in kyltemp_tab:  " + retval.ToString());
            return retval;

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


        public void CalculateInjectionTiming(InjectionType type)
        {
            gridView1.Columns.Clear();
            switch (type)
            {
                case InjectionType.Normal:
                    if (fuel_map_x_axis.Length > 0 && fuel_map_y_axis.Length > 0 && insp_mat.Length > 0)
                    {
                        // fill table
                        DataTable dt = new DataTable();
                        object[] vals = new object[fuel_map_x_axis.Length];
                        int idx = 0;
                        foreach (int pressure in fuel_map_x_axis)
                        {
                            dt.Columns.Add(pressure.ToString());
                        }
                        int rpm_index = 0;
                        for(int i = fuel_map_y_axis.Length -1; i >= 0; i --)
                        //foreach (int rpm in fuel_map_y_axis)
                        {
                            int rpm = fuel_map_y_axis[i];
                            int mapindex = 0;
                            foreach (int pressureit in fuel_map_x_axis)
                            {
                                int pressure = pressureit;
                                if (m_mapSensor == MapSensorType.MapSensor30)
                                {
                                    pressure *= 120;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor35)
                                {
                                    pressure *= 140;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor40)
                                {
                                    pressure *= 160;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor50)
                                {
                                    pressure *= 200;
                                    pressure /= 100;
                                }
                                int Lufttemp_faktor = GetLuftTempFactor(m_iat);
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
                                float dutycycle = (rpm * inj_dur_ms) / 1200;

                                // for fun, take Fload_tab into account
                                //Console.WriteLine("InjTiming Converting pressure: " + pressure.ToString() + " rpm: " + rpm.ToString() + " dc: " + dutycycle.ToString() + " ms: " + inj_dur_ms.ToString());

                                vals.SetValue(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2"), mapindex);
                                //Console.WriteLine(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2") + " mapidx: " + mapindex.ToString());
                                mapindex++;
                            }
                            dt.Rows.Add(vals);
                            rpm_index++;
                        }
                        gridControl1.DataSource = dt;
                    }
                    break;
                case InjectionType.Knocking:
                    if (fuel_knock_map_x_axis.Length > 0 && fuel_knock_map_y_axis.Length > 0 && fuel_knock_mat.Length > 0)
                    {
                        // fill table
                        DataTable dt = new DataTable();
                        object[] vals = new object[fuel_knock_map_x_axis.Length];
                        int idx = 0;
                        foreach (int pressure in fuel_knock_map_x_axis)
                        {
                            dt.Columns.Add(pressure.ToString());
                        }
                        int rpm_index = 0;
                        for (int i = fuel_knock_map_y_axis.Length - 1; i >= 0; i--)
                        //foreach (int rpm in fuel_map_y_axis)
                        {
                            int rpm = fuel_knock_map_y_axis[i];
                            int mapindex = 0;
                            foreach (int pressureit in fuel_knock_map_x_axis)
                            {
                                int pressure = pressureit;
                                if (m_mapSensor == MapSensorType.MapSensor30)
                                {
                                    pressure *= 120;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor35)
                                {
                                    pressure *= 140;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor40)
                                {
                                    pressure *= 160;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor50)
                                {
                                    pressure *= 200;
                                    pressure /= 100;
                                }
                                int Lufttemp_faktor = GetLuftTempFactor(m_iat);
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
                                int cellvalue = (int)Handle_tables(rpm, (byte)Medellast, fuel_knock_map_y_axis.Length, fuel_knock_map_x_axis.Length, fuel_knock_mat, fuel_knock_map_y_axis, fuel_knock_map_x_axis);
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
                                //float dutycycle = ((float)(Injection_timems10) / temp) * 100;

                                inj_dur_ms /= 250;
                                float dutycycle = (rpm * inj_dur_ms) / 1200;
                                vals.SetValue(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2"), mapindex);
                                //Console.WriteLine(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2") + " mapidx: " + mapindex.ToString());

                                mapindex++;
                            }
                            dt.Rows.Add(vals);
                            rpm_index++;
                        }
                        gridControl1.DataSource = dt;
                    }

                    break;
                case InjectionType.Idle:

                    if (idle_fuel_x_axis.Length > 0 && idle_fuel_y_axis.Length > 0 && idle_fuel_map.Length > 0)
                    {
                        // fill table
                        DataTable dt = new DataTable();
                        object[] vals = new object[idle_fuel_x_axis.Length];
                        int idx = 0;
                        foreach (int pressure in idle_fuel_x_axis)
                        {
                            dt.Columns.Add(pressure.ToString());
                        }
                        int rpm_index = 0;
                        for (int i = idle_fuel_y_axis.Length - 1; i >= 0; i--)
                        //foreach (int rpm in fuel_map_y_axis)
                        {
                            int rpm = idle_fuel_y_axis[i];
                            int mapindex = 0;
                            foreach (int pressureit in idle_fuel_x_axis)
                            {
                                int pressure = pressureit;
                                if (m_mapSensor == MapSensorType.MapSensor30)
                                {
                                    pressure *= 120;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor35)
                                {
                                    pressure *= 140;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor40)
                                {
                                    pressure *= 160;
                                    pressure /= 100;
                                }
                                else if (m_mapSensor == MapSensorType.MapSensor50)
                                {
                                    pressure *= 200;
                                    pressure /= 100;
                                }
                                int Lufttemp_faktor = GetLuftTempFactor(m_iat);
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
                                int cellvalue = (int)Handle_tables(rpm, (byte)Medellast, idle_fuel_y_axis.Length, idle_fuel_x_axis.Length, idle_fuel_map, idle_fuel_y_axis, idle_fuel_x_axis);
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
                                //float dutycycle = ((float)(Injection_timems10) / temp) * 100;

                                inj_dur_ms /= 250;
                                float dutycycle = (rpm * inj_dur_ms) / 1200;
                                vals.SetValue(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2"), mapindex);
                                //Console.WriteLine(inj_dur_ms.ToString("F2") + "/" + dutycycle.ToString("F2") + " mapidx: " + mapindex.ToString());

                                mapindex++;
                            }
                            dt.Rows.Add(vals);
                            rpm_index++;
                        }
                        gridControl1.DataSource = dt;
                    }
                    break;
            }
        }

        private int CalculateInjectionTime(int pressure, int m_iat)
        {
            //pressure *= 10; // must be P_manifold10

            int grund_tid = inj_konst * ((GetLuftTempFactor(m_iat)  + 384) * pressure) / 512;
            return grund_tid;
            //(Insp_mat+128)/256

        }

        private int GetLuftTempFactor(int m_iat)
        {
            //P_manifold_LTF = ((Lufttemp_faktor + 384) * P_manifold10) / 512;
            //return 100;

            int Lufttemp_faktor = Handle_temp_tables(m_iat, luft_kompfak,lufttemp_steg);
            return Lufttemp_faktor;
            //int Lufttemp_faktor = LookupAirTemperature(m_iat);
            //return Lufttemp_faktor;
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
            if (x_indx < x_count-1 && y_indx < y_count-1)
            {
                vx = interpolate2((tmp3 - tmp1), (x_axis[x_indx + 1] - x_axis[x_indx]), (x_value - x_axis[x_indx]), tmp1);
                vy = interpolate2((tmp2 - tmp1), (y_axis[y_indx + 1] - y_axis[y_indx]), (y_value - y_axis[y_indx]), tmp1);
            }
            else
            {
                vx = interpolate2((tmp3 - tmp1), (x_axis[x_indx ] - x_axis[x_indx]), (x_value - x_axis[x_indx]), tmp1);
                vy = interpolate2((tmp2 - tmp1), (y_axis[y_indx ] - y_axis[y_indx]), (y_value - y_axis[y_indx]), tmp1);
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



        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RecalculateValues(InjectionType type)
        {
            CalculateInjectionTiming(type);
        }

        private void comboBoxEdit1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            switch (comboBoxEdit1.SelectedIndex)
            {
                case 0: //-30
                    m_iat = 230;
                    break;
                case 1: //-10
                    m_iat = 199;
                    break;
                case 2: // 20
                    m_iat = 163;
                    break;
                case 3: // 40
                    m_iat = 77;
                    break;
                case 4: // 60
                    m_iat = 46;
                    break;
                case 5: // 80
                    m_iat = 36;
                    break;
            }
            RecalculateValues(m_viewtype);
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxEdit2.SelectedIndex)
            {
                case 0: // normal
                    m_viewtype = InjectionType.Normal;
                    break;
                case 2: // knock
                    m_viewtype = InjectionType.Knocking;
                    break;
                case 1: // idle
                    m_viewtype = InjectionType.Idle;
                    break;

            }
            RecalculateValues(m_viewtype);
        }

        private void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                batt_volt = Convert.ToInt32(comboBoxEdit3.Text);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            comboBoxEdit3.Text = batt_volt.ToString();
            RecalculateValues(m_viewtype);
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                string str = (string)e.CellValue;
                char[] sep = new char[1];
                sep.SetValue('/', 0);
                string[] vals = str.Split(sep);
                double v = Convert.ToDouble(vals.GetValue(0));
                int b = (int)v;
                e.DisplayText = v.ToString();
                double dc = Convert.ToDouble(vals.GetValue(1));
                if (comboBoxEdit4.SelectedIndex == 1)
                {
                    e.DisplayText = dc.ToString();
                }

                b *= 8;

                if (b < 0) b = -b;
                if (b > 255) b = 255;
                Color c = Color.FromArgb(b, Color.Red);
                int width = (int)(e.Bounds.Width * dc) / 100;

                //Color c = Color.FromArgb(r, 128, 128);
                SolidBrush sb = new SolidBrush(c);
                e.Graphics.FillRectangle(sb, e.Bounds /* new Rectangle(e.Bounds.X, e.Bounds.Y, width, e.Bounds.Height)*/);

                // draw DC in bottom of cell
                System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Green, Color.Red, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(gb, new Rectangle(e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 3, width, 4));

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            bool m_isUpsideDown = true;
            if (e.RowHandle >= 0)
            {
                try
                {
                    switch (m_viewtype)
                    {
                        case InjectionType.Normal:

                            if (fuel_map_y_axis.Length > 0)
                            {
                                if (fuel_map_y_axis.Length > e.RowHandle)
                                {

                                    string yvalue = fuel_map_y_axis.GetValue((fuel_map_y_axis.Length - 1) - e.RowHandle).ToString();
                                    if (!m_isUpsideDown)
                                    {
                                        // dan andere waarde nemen
                                        yvalue = fuel_map_y_axis.GetValue(e.RowHandle).ToString();
                                    }

                                    Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                    e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                                    e.Graphics.FillRectangle(gb, e.Bounds);
                                    e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height- 12) / 2));
                                    e.Handled = true;
                                }
                            }
                            break;
                        case InjectionType.Knocking:

                            if (fuel_knock_map_y_axis.Length > 0)
                            {
                                if (fuel_knock_map_y_axis.Length > e.RowHandle)
                                {

                                    string yvalue = fuel_knock_map_y_axis.GetValue((fuel_knock_map_y_axis.Length - 1) - e.RowHandle).ToString();
                                    if (!m_isUpsideDown)
                                    {
                                        // dan andere waarde nemen
                                        yvalue = fuel_knock_map_y_axis.GetValue(e.RowHandle).ToString();
                                    }

                                    Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                    e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                                    e.Graphics.FillRectangle(gb, e.Bounds);
                                    e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                                    e.Handled = true;
                                }
                            }
                            break;
                        case InjectionType.Idle:

                            if (idle_fuel_y_axis.Length > 0)
                            {
                                if (idle_fuel_y_axis.Length > e.RowHandle)
                                {

                                    string yvalue = idle_fuel_y_axis.GetValue((idle_fuel_y_axis.Length - 1) - e.RowHandle).ToString();
                                    if (!m_isUpsideDown)
                                    {
                                        // dan andere waarde nemen
                                        yvalue = idle_fuel_y_axis.GetValue(e.RowHandle).ToString();
                                    }

                                    Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                                    e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                                    System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                                    e.Graphics.FillRectangle(gb, e.Bounds);
                                    e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                                    e.Handled = true;
                                }
                            }
                            break;
                    }

                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }

        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {

                switch (m_viewtype)
                {
                    case InjectionType.Normal:

                        if (fuel_map_x_axis.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = fuel_map_x_axis.GetValue(e.Column.VisibleIndex).ToString();

                            try
                            {
                                float v = (float)Convert.ToDouble(xvalue);
                                switch (m_mapSensor)
                                {
                                    case MapSensorType.MapSensor30:
                                        v *= 1.2F;
                                        break;
                                    case MapSensorType.MapSensor35:
                                        v *= 1.4F;
                                        break;
                                    case MapSensorType.MapSensor40:
                                        v *= 1.6F;
                                        break;
                                    case MapSensorType.MapSensor50:
                                        v *= 2.0F;
                                        break;
                                }

                                v *= (float)0.01F;
                                v -= 1;
                                xvalue = v.ToString("F2");
                            }
                            catch (Exception cE)
                            {
                                Console.WriteLine(cE.Message);
                            }

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height- 12) / 2));
                            e.Handled = true;
                        }
                        break;
                    case InjectionType.Knocking:

                        if (fuel_knock_map_x_axis.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = fuel_knock_map_x_axis.GetValue(e.Column.VisibleIndex).ToString();

                            try
                            {
                                float v = (float)Convert.ToDouble(xvalue);
                                switch (m_mapSensor)
                                {
                                    case MapSensorType.MapSensor30:
                                        v *= 1.2F;
                                        break;
                                    case MapSensorType.MapSensor35:
                                        v *= 1.4F;
                                        break;
                                    case MapSensorType.MapSensor40:
                                        v *= 1.6F;
                                        break;
                                    case MapSensorType.MapSensor50:
                                        v *= 2.0F;
                                        break;
                                } 
                                v *= (float)0.01F;
                                v -= 1;
                                xvalue = v.ToString("F2");
                            }
                            catch (Exception cE)
                            {
                                Console.WriteLine(cE.Message);
                            }

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                            e.Handled = true;
                        }
                        break;
                    case InjectionType.Idle:

                        if (idle_fuel_x_axis.Length > e.Column.VisibleIndex)
                        {
                            string xvalue = idle_fuel_x_axis.GetValue(e.Column.VisibleIndex).ToString();

                            try
                            {
                                float v = (float)Convert.ToDouble(xvalue);
                                switch (m_mapSensor)
                                {
                                    case MapSensorType.MapSensor30:
                                        v *= 1.2F;
                                        break;
                                    case MapSensorType.MapSensor35:
                                        v *= 1.4F;
                                        break;
                                    case MapSensorType.MapSensor40:
                                        v *= 1.6F;
                                        break;
                                    case MapSensorType.MapSensor50:
                                        v *= 2.0F;
                                        break;
                                } 
                                v *= (float)0.01F;
                                v -= 1;
                                xvalue = v.ToString("F2");
                            }
                            catch (Exception cE)
                            {
                                Console.WriteLine(cE.Message);
                            }

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            //e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, r);
                            e.Graphics.DrawString(xvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 3, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                            e.Handled = true;
                        }
                        break;
                }


            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            inj_konst = Convert.ToInt32(spinEdit1.EditValue);
            RecalculateValues(m_viewtype);
        }

        private void comboBoxEdit4_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalculateValues(m_viewtype);
        }
    }
}