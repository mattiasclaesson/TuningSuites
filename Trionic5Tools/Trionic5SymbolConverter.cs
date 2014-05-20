using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public enum MapSensorType : int 
    {
        MapSensor25,
        MapSensor30,
        MapSensor35,
        MapSensor40,
        MapSensor50
    }

    public class Trionic5SymbolConverter
    {
        /// <summary>
        /// Take care of threebar conversion as well!
        /// </summary>
        /// <param name="symbolname"></param>
        /// <param name="ecudata"></param>
        /// <returns></returns>
        public double ConvertSymbol(string symbolname, byte[] ecudata,  MapSensorType _mapsensor, double _userCorrectionFactor, double _userCorrectionOffset, bool UseUserCorrection)
        {
            double retval = 0;

            if (UseUserCorrection)
            {
                retval = ConvertByteStringToDouble(ecudata);
                retval *= _userCorrectionFactor;
                retval += _userCorrectionOffset;
            }
            else
            {

                //convert data depending on symbolname
                double _correctionForMapsensor = 1;
                switch (_mapsensor)
                {
                    case MapSensorType.MapSensor25:
                        _correctionForMapsensor = 1;
                        break;
                    case MapSensorType.MapSensor30:
                        _correctionForMapsensor = 1.2;
                        break;
                    case MapSensorType.MapSensor35:
                        _correctionForMapsensor = 1.4;
                        break;
                    case MapSensorType.MapSensor40:
                        _correctionForMapsensor = 1.6;
                        break;
                    case MapSensorType.MapSensor50:
                        _correctionForMapsensor = 2.0;
                        break;
                }

                switch (symbolname)
                {
                    case "P_medel":
                    case "P_Manifold10":
                    case "P_Manifold":
                    case "Max_tryck":
                    case "Regl_tryck":
                        // inlet manifold pressure
                        retval = ConvertByteStringToDouble(ecudata);
                        retval *= _correctionForMapsensor;
                        retval *= 0.01F;
                        retval -= 1;
                        break;
                    case "Lufttemp":
                        retval = ConvertByteStringToDouble(ecudata);
                        if (retval > 128) retval = -(256 - retval);
                        break;
                    case "Kyl_temp":
                        retval = ConvertByteStringToDouble(ecudata);
                        if (retval > 128) retval = -(256 - retval);
                        break;
                    case "Rpm":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval *= 10; // factor 10
                        break;
                    case "AD_sond":
                        // should average, no the realtime panel does that
                        retval = ConvertByteStringToDouble(ecudata);
                        retval = ConvertToAFR(retval);
                        break;
                    case "AD_EGR":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval = ConvertToWidebandAFR(retval);
                        //retval = ConvertToAFR(retval);
                        break;
                    case "Pgm_status":
                        // now what, just pass it on in a seperate structure
                        retval = ConvertByteStringToDoubleStatus(ecudata);
                        break;
                    case "Insptid_ms10":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval /= 10;
                        break;
                    case "Lacc_mangd":
                    case "Acc_mangd":
                    case "Lret_mangd":
                    case "Ret_mangd":
                        retval = ConvertByteStringToDouble(ecudata);
                        // 4 values in one variable, one for each cylinder
                        break;
                    case "Ign_angle":
                        retval = ConvertByteStringToDouble(ecudata);
                        if (retval > 32000) retval = -(65536 - retval);
                        retval /= 10;
                        break;
                    case "Knock_offset1":
                    case "Knock_offset2":
                    case "Knock_offset3":
                    case "Knock_offset4":
                    case "Knock_offset1234":
                        retval = ConvertByteStringToDouble(ecudata);
                        if (retval > 32000) retval = -(65536 - retval);
                        retval /= 10;
                        break;
                    case "Medeltrot":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval -= 34;
                        //TODO: should substract trot_min from this value?
                        break;
                    case "Apc_decrese":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval *= _correctionForMapsensor;
                        retval *= 0.01F; // to bar!
                        break;
                    case "P_fak":
                    case "I_fak":
                    case "D_fak":
                        retval = ConvertByteStringToDouble(ecudata);
                        if (retval > 32000) retval = -(65535 - retval);
                        break;
                    case "PWM_ut10":
                        retval = ConvertByteStringToDouble(ecudata);
                        break;
                    case "Knock_count_cyl1":
                    case "Knock_count_cyl2":
                    case "Knock_count_cyl3":
                    case "Knock_count_cyl4":
                        retval = ConvertByteStringToDouble(ecudata);
                        break;
                    case "Knock_average":
                        retval = ConvertByteStringToDouble(ecudata);
                        break;
                    case "Bil_hast":
                        retval = ConvertByteStringToDouble(ecudata);
                        break;
                    case "TQ":
                        retval = ConvertByteStringToDouble(ecudata);
                        retval *= _correctionForMapsensor;
                        break;
                    default:
                        retval = ConvertByteStringToDouble(ecudata);
                        break;
                }
            }
            
            return retval;
        }

        private double ConvertByteStringToDoubleStatus(byte[] ecudata)
        {
            double retval = 0;
            if (ecudata.Length == 4)
            {
                retval = Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(0));
            }
            else if (ecudata.Length == 5)
            {
                retval = Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(0));
            }
            else if (ecudata.Length == 6)
            {
                retval = Convert.ToDouble(ecudata.GetValue(5)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(0));
            }
            else if (ecudata.Length == 7)
            {
                retval = Convert.ToDouble(ecudata.GetValue(6)) * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(5)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(0));
            }
            else if (ecudata.Length == 8)
            {
                retval = Convert.ToDouble(ecudata.GetValue(7)) * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(6)) * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(5)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(0));
            }
            return retval;

        }

        private double ConvertByteStringToDouble(byte[] ecudata)
        {
            double retval = 0;
            if (ecudata.Length == 1)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0));
            }
            else if (ecudata.Length == 2)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1));
            }
            else if (ecudata.Length == 4)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3));
            }
            else if (ecudata.Length == 5)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4));
            }
            else if (ecudata.Length == 6)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(5));
            }
            else if (ecudata.Length == 7)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(5)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(6));
            }
            else if (ecudata.Length == 8)
            {
                retval = Convert.ToDouble(ecudata.GetValue(0)) * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(1)) * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(2)) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(3)) * 256 * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(4)) * 256 * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(5)) * 256 * 256;
                retval += Convert.ToDouble(ecudata.GetValue(6)) * 256;
                retval += Convert.ToDouble(ecudata.GetValue(7));
            }
            return retval;
        }

        private double m_WidebandHighVoltage = 5000;

        public double WidebandHighVoltage
        {
            get { return m_WidebandHighVoltage; }
            set { m_WidebandHighVoltage = value; }
        }
        private double m_WidebandLowVoltage = 0;

        public double WidebandLowVoltage
        {
            get { return m_WidebandLowVoltage; }
            set { m_WidebandLowVoltage = value; }
        }
        private double m_WidebandHighAFR = 22.7F;

        public double WidebandHighAFR
        {
            get { return m_WidebandHighAFR; }
            set { m_WidebandHighAFR = value; }
        }
        private double m_WidebandLowAFR = 7F;

        public double WidebandLowAFR
        {
            get { return m_WidebandLowAFR; }
            set { m_WidebandLowAFR = value; }
        }

        private double ConvertToWidebandAFR(double value)
        {
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double retval = 0;
            double voltage = ((value) / 255) * (m_WidebandHighVoltage / 1000 - m_WidebandLowVoltage / 1000);
            //Console.WriteLine("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_WidebandLowVoltage / 1000) voltage = m_WidebandLowVoltage / 1000;
            if (voltage > m_WidebandHighVoltage / 1000) voltage = m_WidebandHighVoltage / 1000;
            //Console.WriteLine("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_WidebandHighAFR / 1000) - (m_WidebandLowAFR / 1000)) / ((m_WidebandHighVoltage / 1000) - (m_WidebandLowVoltage / 1000));
            //Console.WriteLine("Steepness: " + steepness.ToString());
            retval = (m_WidebandLowAFR / 1000) + (steepness * (voltage - (m_WidebandLowVoltage / 1000)));
            //Console.WriteLine("retval: " + retval.ToString());
            return retval;

        }


        private double ConvertToAFR(double value)
        {
            // converteer naar lambda waarden
            // 23 = lambda = 1
            // daarboven = rijk (lambda<1)
            // daaronder = arm  (lambda>1)
            value = Math.Abs(value - 125); // value - 125 means range -125 to -75 
            // abs value means range 125 to 75 in which 125 = lean and 75 = rich
            value /= 100;
            value *= 14.7;
            return (value);

        }
    }
}
