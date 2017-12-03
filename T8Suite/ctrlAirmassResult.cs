using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraCharts;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public partial class ctrlAirmassResult : DevExpress.XtraEditors.XtraUserControl
    {
        private const int TORQUE_LIMIT_350NM = 3500;
        private const int TORQUE_LIMIT_400NM = 4000;

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int pedal_Rows;
        private int pedal_Columns;
        private int[] pedal_Yaxis;
        private int[] pedal_Xaxis;

        private int injectorConstant = 0;
        private int[] injectorBatteryCorrection;
        private int[] injectorBatteryCorrection_axis;

        private byte[] fuelVEMap;
        private int[] fuelVE_Xaxis;
        private int[] fuelVE_Yaxis;

        private int[] EGTMap;
        
        private int[] airTorqueMap;
        private int[] airTorqueMap_Xaxis;
        private int[] airTorqueMap_Yaxis;
        
        private int[] bstknkMaxAirmassMap;
        private int[] bstknkMaxAirmassMap_Xaxis;
        private int[] bstknkMaxAirmassMap_Yaxis;

        private int[] bstknkMaxAirmassAuMap;

        private int[] ffMaxAirmassMap;
        private int[] ffMaxAirmassMap_Xaxis;

        private int[] nominalTorqueMap;
        private int[] nominalTorqueMap_Xaxis;
        private int[] nominalTorqueMap_Yaxis;

        private byte[] entirefile;

        private int fuelcutAirInletLimit;

        private int[] enginetorquelimOverboost;
        private int[] enginetorquelim;
        private int[] enginetorquelimE85;
        private int[] enginetorquelimGear;
        private int[] enginetorquelimAuto;

        private AirmassLimitType[] limiterResult;

        private int m_MaxValueInTable = 0;

        private string m_currentfilename = string.Empty;
        public string Currentfile
        {
            get { return m_currentfilename; }
            set { m_currentfilename = value; }
        }

        private SymbolCollection m_symbols;
        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }

        int[] xdummy = new int[1];

        int powerSeries = -1;
        int powerCompareSeries = -1;
        int torqueCompareSeries = -1;
        int torqueSeries = -1;
        int injectorDCSeries = -1;
        int lambdaSeries = -1;
        int EGTSeries = -1;
        int FlowSeries = -1;

        public delegate void StartTableViewer(object sender, StartTableViewerEventArgs e);
        public event ctrlAirmassResult.StartTableViewer onStartTableViewer;

        public class StartTableViewerEventArgs : System.EventArgs
        {
            private string _mapname;

            public string SymbolName
            {
                get
                {
                    return _mapname;
                }
            }

            public StartTableViewerEventArgs(string mapname)
            {
                this._mapname = mapname;
            }
        }

        public ctrlAirmassResult()
        {
            InitializeComponent();
        }

        private static int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    return sh.Length;
                }
            }
            return 0;
        }

        private byte[] readdatafromfile(SymbolCollection curSymbolCollection, string tablename)
        {
            int address = 0;
            int length = 0;
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == tablename)
                {
                    address = (int)sh.Flash_start_address;
                    length = sh.Length;
                    break;
                }
            }

            byte[] retval = new byte[length];
            try
            {
                for (int i = 0; i < length; i++)
                {
                    retval.SetValue(entirefile[address + i], i);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }
            return retval;
        }

        private int[] readIntdatafromfile(SymbolCollection curSymbolCollection, string tablename)
        {
            int address = 0;
            int length = 0;
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == tablename)
                {
                    address = (int)sh.Flash_start_address;
                    length = sh.Length;
                    break;
                }
            }

            int[] retval = new int[length / 2];
            try
            {
                int j = 0;
                for (int i = 0; i < length; i += 2)
                {
                    byte b1 = entirefile[address + i];
                    byte b2 = entirefile[address + i + 1];
                    int value = Convert.ToInt32(b1) * 256 + Convert.ToInt32(b2);
                    retval.SetValue(value, j++);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }
            return retval;
        }

        private bool IsOverboostEnabled(SymbolCollection symbols)
        {
            bool retval = false;
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TrqLimCal.EnableOverBoost")
                {
                    // read data from file and verify if the value is > 0
                    byte[] overboostenable = readdatafromfile(symbols, "TrqLimCal.EnableOverBoost");
                    foreach (byte b in overboostenable)
                    {
                        if (b != 0x00) retval = true;
                    }
                }
            }
            return retval;

        }

        private static bool IsBinaryBiopower(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "FFTrqCal.FFTrq_MaxEngineTab1" || sh.SmartVarname == "FFTrqCal.FFTrq_MaxEngineTab2")
                {
                    return true;
                }
            }
            return false;
        }

        private int TorqueToAirmass(int torque, int rpm)
        {
            // should be done through the torque->airmass map
            if (torque > 32000)
            {
                torque = -(65535 - torque);
            }
            double airmass = GetInterpolatedTableValue(airTorqueMap, airTorqueMap_Xaxis, airTorqueMap_Yaxis, rpm, torque);
            return Convert.ToInt32(airmass);
        }

        public void Calculate()
        {
            DataTable dt = CalculateDataTable(m_currentfilename, m_symbols, out limiterResult);
            gridControl1.DataSource = dt;
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == tabCompressormap.Name)
            {
                LoadCompressorMapWithDetails();
            }
        }

        private static bool SymbolExists(string symbolname, SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == symbolname) return true;
            }
            return false;
        }

        private int CalculateMaxAirmassforcell(int rpm, int requestairmass, out AirmassLimitType limiterType)
        {
            int restrictedairmass = requestairmass;
            limiterType = AirmassLimitType.None;

            // first check against torquelimiters
            AirmassLimitType TrqLimiterType = AirmassLimitType.None;
            restrictedairmass = CheckAgainstTorqueLimiters(rpm, requestairmass, out TrqLimiterType);
            if (restrictedairmass < requestairmass)
            {
                limiterType = TrqLimiterType;
            }

            // secondly check against airmasslimiters
            AirmassLimitType AirmassLimiterType = AirmassLimitType.None;
            int TorqueLimitedAirmass = restrictedairmass;
            restrictedairmass = CheckAgainstAirmassLimiters(rpm, restrictedairmass, ref AirmassLimiterType);

            // finally check agains fuelcut limiter
            restrictedairmass = CheckAgainstFuelcutLimiter(restrictedairmass, ref AirmassLimiterType);
            if (restrictedairmass < TorqueLimitedAirmass)
            {
                limiterType = AirmassLimiterType;
            }

            return restrictedairmass;
        }

        private int CheckAgainstTorqueLimiters(int rpm, int requestedairmass, out AirmassLimitType TrqLimiter)
        {
            TrqLimiter = AirmassLimitType.TorqueLimiterGear;
            int LimitedAirMass = requestedairmass;
            int torque;
            if (isCarAutomatic.Checked)
            {
                torque = TORQUE_LIMIT_350NM;
            }
            else
            {
                torque = TORQUE_LIMIT_400NM; // Basefile hardcoded limiter
            }

            if (isFuelE85.Checked)
            {
                int torquelimitE85 = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimE85, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (torque > torquelimitE85)
                {
                    logger.Debug(String.Format("Torque E85limit is limited from {0} to {1} at {2} rpm", torque, torquelimitE85, rpm));
                    torque = torquelimitE85;
                    TrqLimiter = AirmassLimitType.TorqueLimiterEngineE85;
                }
            }
            else
            {
                int torquelimitOverboost;
                if (isOverboostActive.Checked)
                {
                    torquelimitOverboost = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimOverboost, xdummy, airTorqueMap_Yaxis, rpm, 0));
                }
                else
                {
                    torquelimitOverboost = 0;
                }

                int torquelimitPetrol = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (isOverboostActive.Checked && torque > torquelimitOverboost)
                {
                    logger.Debug(String.Format("Torque OverBoostLimit is limited from {0} to {1} at {2} rpm", torque, torquelimitOverboost, rpm));
                    torque = torquelimitOverboost;
                    TrqLimiter = AirmassLimitType.OverBoostLimiter;
                }
                else if (isOverboostActive.Checked && torque < torquelimitOverboost && torque > torquelimitPetrol)
                {
                    logger.Debug(String.Format("Torque OverBoostLimit replaced Petrol limit {0} with {1} at {2} rpm", torquelimitPetrol, torquelimitOverboost, rpm));
                    torque = torquelimitOverboost;
                    TrqLimiter = AirmassLimitType.OverBoostLimiter;
                }
                else if (torque > torquelimitPetrol)
                {
                    logger.Debug(String.Format("Torque Petrol is limited from {0} to {1} at {2} rpm", torque, torquelimitPetrol, rpm));
                    torque = torquelimitPetrol;
                    TrqLimiter = AirmassLimitType.TorqueLimiterEngine;
                }
            }

            if (isCarAutomatic.Checked)
            {
                int torquelimitAutomatic = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimAuto, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (torque > torquelimitAutomatic)
                {
                    logger.Debug(String.Format("Automatic gear torque limited from {0} to {1} at {2} rpm", torque, torquelimitAutomatic, rpm));
                    torque = torquelimitAutomatic;
                    TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                }
            }
            else
            {
                // Gear values have the same meaning as in ECMStat.ManualGear
                // Actual gear (manual gearbox).
                int[] gears = new int[8];
                gears.SetValue(0, 0); // Undefined gear  0
                gears.SetValue(1, 1); // First gear      1
                gears.SetValue(2, 2); // Second gear     2
                gears.SetValue(3, 3); // Third gear      3
                gears.SetValue(4, 4); // Fourth gear     4
                gears.SetValue(5, 5); // Fifth gear      5
                gears.SetValue(6, 6); // Sixth gear      6
                gears.SetValue(7, 7); // Reverse gear    7

                int torquelimitManual = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimGear, xdummy, gears, cbGearSelection.SelectedIndex, 0));
                if (torque > torquelimitManual)
                {
                    logger.Debug(String.Format("Manual gear torque limited from {0} to {1} at {2} rpm", torque, torquelimitManual, rpm));
                    torque = torquelimitManual;
                    TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                }
            }

            int TestLimitedAirmass = Convert.ToInt32(GetInterpolatedTableValue(airTorqueMap, airTorqueMap_Xaxis, airTorqueMap_Yaxis, rpm, torque));
            if (TestLimitedAirmass < LimitedAirMass)
            {
                LimitedAirMass = TestLimitedAirmass;
                if (TrqLimiter == AirmassLimitType.None)
                {
                    TrqLimiter = AirmassLimitType.AirTorqueCalibration;
                }
            }

            if (TrqLimiter == AirmassLimitType.None)
            {
                LimitedAirMass = requestedairmass;
            }
            return LimitedAirMass;
        }

        private int CheckAgainstFuelcutLimiter(int requestedairmass, ref AirmassLimitType AirmassLimiter)
        {
            int retval = requestedairmass;

            if (fuelcutAirInletLimit < requestedairmass)
            {
                retval = fuelcutAirInletLimit;
                AirmassLimiter = AirmassLimitType.FuelCutLimiter;
                logger.Debug("Reduced airmass because of FuelCutLimiter: " + requestedairmass);
            }
            return retval;
        }

        private int CheckAgainstAirmassLimiters(int rpm, int requestedairmass, ref AirmassLimitType AirmassLimiter)
        {            
            int airmasslimit = requestedairmass;
            string message;

            if (isCarAutomatic.Checked)
            {
                airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknkMaxAirmassAuMap, bstknkMaxAirmassMap_Xaxis, bstknkMaxAirmassMap_Yaxis, rpm, 0));
                message = String.Format("Reduced airmass because of BstKnkCal.MaxAirmassAu: {0} rpm: {1}", requestedairmass, rpm);
            }
            else if (isFuelE85.Checked)
            {
                airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(ffMaxAirmassMap, ffMaxAirmassMap_Xaxis, bstknkMaxAirmassMap_Yaxis, rpm, 0)); // zero degree ignition offset
                message = String.Format("Reduced airmass because of FFAirCal.m_maxAirmass: {0} rpm: {1}", requestedairmass, rpm);
            }
            else
            {
                airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknkMaxAirmassMap, bstknkMaxAirmassMap_Xaxis, bstknkMaxAirmassMap_Yaxis, rpm, 0));
                message = String.Format("Reduced airmass because of BstKnkCal.MaxAirmass: {0} rpm: {1}", requestedairmass, rpm);
            }

            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = AirmassLimitType.AirmassLimiter;
                logger.Debug(message);
            }

            return requestedairmass;
        }

        private double GetInterpolatedTableValue(int[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue)
        {
            int m_yindex = 0;
            int m_xindex = 0;
            double result = 0;
            double m_ydiff = 0;
            double m_xdiff = 0;
            double m_ypercentage = 0;
            double m_xpercentage = 0;
            try
            {
                for (int yindex = 0; yindex < yaxis.Length; yindex++)
                {
                    if (yvalue > Convert.ToInt32(yaxis.GetValue(yindex)))
                    {
                        m_yindex = yindex;
                        m_ydiff = Math.Abs(Convert.ToDouble(yaxis.GetValue(yindex)) - yvalue);
                        if (m_yindex < yaxis.Length - 1)
                        {
                            m_ypercentage = (double)m_ydiff / (Convert.ToInt32(yaxis.GetValue(yindex + 1)) - Convert.ToInt32(yaxis.GetValue(yindex)));
                        }
                        else
                        {
                            m_ypercentage = 0;
                        }
                        // break;
                    }
                }
                for (int xindex = 0; xindex < xaxis.Length; xindex++)
                {
                    if (xvalue > Convert.ToDouble(xaxis.GetValue(xindex)))
                    {
                        m_xindex = xindex;
                        m_xdiff = Math.Abs(Convert.ToDouble(xaxis.GetValue(xindex)) - xvalue);
                        if (m_xindex < xaxis.Length - 1)
                        {
                            m_xpercentage = m_xdiff / (Convert.ToInt32(xaxis.GetValue(xindex + 1)) - Convert.ToInt32(xaxis.GetValue(xindex)));
                        }
                        else
                        {
                            m_xpercentage = 0;
                        }
                        // break;
                    }
                }
                //logger.Debug("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage + " MAPindex = " + m_mapindex + " Percentage = " + m_mappercentage);
                // now we found the indexes of the smaller values
                int a1 = 0;
                int a2 = 0;
                int b1 = 0;
                int b2 = 0;
                if (m_yindex == (yaxis.Length - 1) && (m_xindex < xaxis.Length - 1))
                {
                    // last row in table, extend with the same values
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = a1;
                    b2 = a2;
                }
                else if (m_yindex == (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = a1;
                    b2 = a1;
                    return Convert.ToDouble(a1);
                }
                else if (m_yindex <= (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = b1;
                }
                else
                {
                    a1 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (int)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = (int)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex + 1);
                }
                //logger.Debug("a1 = " + a1 + " a2 = " + a2 + " b1 = " + b1 + " b2 = " + b2);
                
                // now interpolate the values found
                double aval = 0;
                double bval = 0;
                double adiff = Math.Abs((double)a1 - (double)a2);
                if (a1 > a2) aval = a1 - (m_xpercentage * adiff);
                else aval = a1 + (m_xpercentage * adiff);
                double bdiff = Math.Abs((double)b1 - (double)b2);
                if (b1 > b2) bval = b1 - (m_xpercentage * bdiff);
                else bval = b1 + (m_xpercentage * bdiff);
                // now interpolate vertically (RPM axis)

                //logger.Debug("aval = " + aval + " bval = " + bval);
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval)
                {
                    result = aval - (m_ypercentage * abdiff);
                }
                else
                {
                    result = aval + (m_ypercentage * abdiff);
                }
                //logger.Debug("result = " + result);
            }
            catch (Exception E)
            {
                logger.Debug(E, "Failed to interpolate");
            }
            return result;
        }

        private double GetInterpolatedTableValue(byte[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue) 
        {
            int m_yindex = 0;
            int m_xindex = 0;
            double result = 0;
            double m_ydiff = 0;
            double m_xdiff = 0;
            double m_ypercentage = 0;
            double m_xpercentage = 0;
            try
            {
                for (int yindex = 0; yindex < yaxis.Length; yindex++)
                {
                    if (yvalue > Convert.ToInt32(yaxis.GetValue(yindex)))
                    {
                        m_yindex = yindex;
                        m_ydiff = Math.Abs(Convert.ToDouble(yaxis.GetValue(yindex)) - yvalue);
                        if (m_yindex < yaxis.Length - 1)
                        {
                            m_ypercentage = (double)m_ydiff / (Convert.ToInt32(yaxis.GetValue(yindex + 1)) - Convert.ToInt32(yaxis.GetValue(yindex)));
                        }
                        else
                        {
                            m_ypercentage = 0;
                        }
                        // break;
                    }
                }
                for (int xindex = 0; xindex < xaxis.Length; xindex++)
                {
                    if (xvalue > Convert.ToDouble(xaxis.GetValue(xindex)))
                    {
                        m_xindex = xindex;
                        m_xdiff = Math.Abs(Convert.ToDouble(xaxis.GetValue(xindex)) - xvalue);
                        if (m_xindex < xaxis.Length - 1)
                        {
                            m_xpercentage = m_xdiff / (Convert.ToInt32(xaxis.GetValue(xindex + 1)) - Convert.ToInt32(xaxis.GetValue(xindex)));
                        }
                        else
                        {
                            m_xpercentage = 0;
                        }
                        // break;
                    }
                }
                //logger.Debug("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage + " MAPindex = " + m_mapindex + " Percentage = " + m_mappercentage);
                // now we found the indexes of the smaller values
                byte a1 = 0;
                byte a2 = 0;
                byte b1 = 0;
                byte b2 = 0;
                if (m_yindex == (yaxis.Length - 1) && (m_xindex < xaxis.Length - 1))
                {
                    // last row in table, extend with the same values
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = a1;
                    b2 = a2;
                }
                else if (m_yindex == (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = a1;
                    b2 = a1;
                    return Convert.ToDouble(a1);
                }
                else if (m_yindex <= (yaxis.Length - 1) && (m_xindex == xaxis.Length - 1))
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = a1;
                    b1 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = b1;
                }
                else
                {
                    a1 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex);
                    a2 = (byte)table.GetValue((m_yindex * xaxis.Length) + m_xindex + 1);
                    b1 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex);
                    b2 = (byte)table.GetValue(((m_yindex + 1) * xaxis.Length) + m_xindex + 1);
                }
                //logger.Debug("a1 = " + a1 + " a2 = " + a2 + " b1 = " + b1 + " b2 = " + b2);
                // now interpolate the values found
                double aval = 0;
                double bval = 0;
                double adiff = Math.Abs((double)a1 - (double)a2);
                if (a1 > a2) aval = a1 - (m_xpercentage * adiff);
                else aval = a1 + (m_xpercentage * adiff);
                double bdiff = Math.Abs((double)b1 - (double)b2);
                if (b1 > b2) bval = b1 - (m_xpercentage * bdiff);
                else bval = b1 + (m_xpercentage * bdiff);
                // now interpolate vertically (RPM axis)

                //logger.Debug("aval = " + aval + " bval = " + bval);
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval)
                {
                    result = aval - (m_ypercentage * abdiff);
                }
                else
                {
                    result = aval + (m_ypercentage * abdiff);
                }
                //logger.Debug("result = " + result);
            }
            catch (Exception E)
            {
                logger.Debug(E, "Failed to interpolate");
            }
            return result;
        }

        private static int TorqueToPowerkW(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            // convert to kW in stead of horsepower
            power *= 0.73549875;
            return Convert.ToInt32(power);
        }

        private static int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            return Convert.ToInt32(power);
        }

        private static int TorqueToTorqueLbft(int torque)
        {
            double tq = torque;
            tq /= 1.3558;
            return Convert.ToInt32(tq);
        }

        private static double GetCorrectionFactorForRpm(int rpm)
        {
            double correction = 1;
            if (rpm >= 6000) correction = 0.85;
            else if (rpm >= 5820) correction = 0.94;
            else if (rpm >= 5440) correction = 0.95;
            else if (rpm >= 5060) correction = 0.99;
            else if (rpm >= 4680) correction = 1.00;
            else if (rpm >= 4300) correction = 1.00;
            else if (rpm >= 3920) correction = 1.00;
            else if (rpm >= 3540) correction = 1.00;
            else if (rpm >= 3160) correction = 1.00;
            else if (rpm >= 2780) correction = 1.00;
            else if (rpm >= 2400) correction = 1.00;
            else if (rpm >= 2020) correction = 1.00;
            else if (rpm >= 1640) correction = 1.00;
            else if (rpm >= 1260) correction = 1.00;
            else correction = 1.00;

            return correction;
        }

        private int AirmassToTorque(SymbolCollection symbols, int airmass, int rpm)
        {
            double tq;
            if (useTrionicCalculationForTorque.Checked)
            {
                // first convert airmass torque to torque using TrqMastCal.Trq_NominalMap
                // axis are 
                // x = TrqMastCal.m_AirXSP (airmass)
                // y = TrqMastCal.n_EngineYSP (rpm)
                int[] nominaltorque = readIntdatafromfile(symbols, "TrqMastCal.Trq_NominalMap");
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    val /= 10; // in tenth of Nms
                    nominaltorque.SetValue(val, a);
                }
                int[] xaxis = readIntdatafromfile(symbols, "TrqMastCal.m_AirXSP");
                int[] yaxis = readIntdatafromfile(symbols, "TrqMastCal.n_EngineYSP");
                tq = GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, airmass);
            }
            else
            {
                tq = Convert.ToDouble(airmass) / 3.1;
                if (isFuelE85.Checked)
                {
                    tq *= 1.07;
                }
                double correction = GetCorrectionFactorForRpm(rpm);
                tq *= correction;
            }
            return Convert.ToInt32(tq);
        }

        private void CastStartViewerEvent(string mapname)
        {
            if (onStartTableViewer != null)
            {
                onStartTableViewer(this, new StartTableViewerEventArgs(mapname));
            }
        }

        private static string MaximizeFileLength(string filename)
        {
            string retval = filename;
            if (retval.Length > 16) retval = retval.Substring(0, 14) + "..";
            return retval;
        }

        private void LoadExtraGraphFromCompareBin(DataTable dt, string filename, SymbolCollection symbols)
        {
            logger.Debug("Loading additional data for: " + filename);
            string powerLabel;
            if (displayPowerInkW.Checked)
            {
                powerLabel = "Power (kW) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            }
            else
            {
                powerLabel = "Power (bhp) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            }
            string torqueLabel;
            if (displayTorqueInLBFT.Checked)
            {
                torqueLabel = "Torque (lbft) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            }
            else
            {
                torqueLabel = "Torque (Nm) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            }
            powerCompareSeries = chartControl1.Series.Add(powerLabel, DevExpress.XtraCharts.ViewType.Spline);
            torqueCompareSeries = chartControl1.Series.Add(torqueLabel, DevExpress.XtraCharts.ViewType.Spline);
            // set line colors
            chartControl1.Series[powerCompareSeries].Label.Visible = false;
            chartControl1.Series[torqueCompareSeries].Label.Visible = false;
            chartControl1.Series[powerCompareSeries].View.Color = Color.Orange;
            chartControl1.Series[torqueCompareSeries].View.Color = Color.LightBlue;
            chartControl1.Series[powerCompareSeries].ArgumentScaleType = ScaleType.Qualitative;
            chartControl1.Series[powerCompareSeries].ValueScaleType = ScaleType.Numerical;
            chartControl1.Series[torqueCompareSeries].ArgumentScaleType = ScaleType.Qualitative;
            chartControl1.Series[torqueCompareSeries].ValueScaleType = ScaleType.Numerical;
            SplineSeriesView sv = (SplineSeriesView)chartControl1.Series[powerCompareSeries].View;
            sv.LineMarkerOptions.Visible = false;
            sv = (SplineSeriesView)chartControl1.Series[torqueCompareSeries].View;
            sv.LineMarkerOptions.Visible = false;
            for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
            {
                double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                // convert to hp
                int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(i));
                int torque = AirmassToTorque(symbols, Convert.ToInt32(o), rpm);
                int horsepower;
                if (displayPowerInkW.Checked)
                {
                    horsepower = TorqueToPowerkW(torque, rpm);
                }
                else
                {
                    horsepower = TorqueToPower(torque, rpm);
                }

                if (displayTorqueInLBFT.Checked)
                {
                    torque = TorqueToTorqueLbft(torque);
                }

                double[] dvals = new double[1];
                dvals.SetValue(Convert.ToDouble(horsepower), 0);
                chartControl1.Series[powerCompareSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvals));

                double[] dvalstorq = new double[1];
                dvalstorq.SetValue(Convert.ToDouble(torque), 0);
                chartControl1.Series[torqueCompareSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalstorq));
            }
        }

        private void LoadGraphWithDetails()
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                // get only the WOT cells, the last 16 integers
                // and the columns which hold the rpm stages
                chartControl1.Series.Clear();
                string powerLabel;
                if (displayPowerInkW.Checked)
                {
                    powerLabel = "Power (kW)";
                }
                else
                {
                    powerLabel = "Power (bhp)";
                }
                string torqueLabel;
                if (displayTorqueInLBFT.Checked)
                {
                    torqueLabel = "Torque (lbft)";
                }
                else
                {
                    torqueLabel = "Torque (Nm)";
                }

                const string injectorDCLabel = "Injector DC (%)";
                const string targetLambdaLabel = "Target lambda (*100)";

                powerSeries = chartControl1.Series.Add(powerLabel, DevExpress.XtraCharts.ViewType.Spline);
                torqueSeries = chartControl1.Series.Add(torqueLabel, DevExpress.XtraCharts.ViewType.Spline);
                injectorDCSeries = chartControl1.Series.Add(injectorDCLabel, DevExpress.XtraCharts.ViewType.Spline);
                lambdaSeries = chartControl1.Series.Add(targetLambdaLabel, DevExpress.XtraCharts.ViewType.Spline);
                EGTSeries = chartControl1.Series.Add("EGT estimate (°C)", DevExpress.XtraCharts.ViewType.Spline);
                FlowSeries = chartControl1.Series.Add("Fuel flow (l/h)", DevExpress.XtraCharts.ViewType.Spline);
                // set line colors
                chartControl1.Series[powerSeries].Label.Border.Visible = false;
                chartControl1.Series[torqueSeries].Label.Border.Visible = false;
                chartControl1.Series[EGTSeries].Label.Border.Visible = false;
                chartControl1.Series[powerSeries].Label.Shadow.Visible = true;
                chartControl1.Series[torqueSeries].Label.Shadow.Visible = true;
                chartControl1.Series[EGTSeries].Label.Shadow.Visible = true;
                chartControl1.Series[injectorDCSeries].Label.Border.Visible = false;
                chartControl1.Series[injectorDCSeries].Label.Shadow.Visible = true;
                chartControl1.Series[lambdaSeries].Label.Border.Visible = false;
                chartControl1.Series[lambdaSeries].Label.Shadow.Visible = true;
                chartControl1.Series[FlowSeries].Label.Border.Visible = false;
                chartControl1.Series[FlowSeries].Label.Shadow.Visible = true;


                chartControl1.Series[powerSeries].View.Color = Color.Red;
                chartControl1.Series[torqueSeries].View.Color = Color.Blue;
                chartControl1.Series[injectorDCSeries].View.Color = Color.GreenYellow;
                chartControl1.Series[lambdaSeries].View.Color = Color.DarkGreen;
                chartControl1.Series[EGTSeries].View.Color = Color.Plum;
                chartControl1.Series[FlowSeries].View.Color = Color.Black;

                chartControl1.Series[powerSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[powerSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[torqueSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[torqueSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[injectorDCSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[injectorDCSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[lambdaSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[lambdaSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[FlowSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[FlowSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[FlowSeries].Visible = false; // default not visible
                chartControl1.Series[EGTSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[EGTSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[EGTSeries].Visible = false; // default not visible

                SplineSeriesView sv = (SplineSeriesView)chartControl1.Series[powerSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[torqueSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[injectorDCSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[lambdaSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[EGTSeries].View;
                sv.LineMarkerOptions.Visible = false;
                sv = (SplineSeriesView)chartControl1.Series[FlowSeries].View;
                sv.LineMarkerOptions.Visible = false;

                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    // convert to hp
                    int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(i));
                    int torque = AirmassToTorque(m_symbols, Convert.ToInt32(o), rpm);
                    
                    int horsepower;
                    if (displayPowerInkW.Checked)
                    {
                        horsepower = TorqueToPowerkW(torque, rpm);
                    }
                    else
                    {
                        horsepower = TorqueToPower(torque, rpm);
                    }

                    if (displayTorqueInLBFT.Checked)
                    {
                        torque = TorqueToTorqueLbft(torque);
                    }

                    int injDC = CalculateInjectorDCusingPulseWidth(Convert.ToInt32(o), rpm);
                    int TargetLambda = CalculateTargetLambda(Convert.ToInt32(o), rpm, injDC);
                    if (injDC > 100)
                    {
                        injDC = 100;
                    }
                    int EstimateEGT = CalculateEstimateEGT(Convert.ToInt32(o), rpm);

                    int EstimateFlow = CalculateFuelFlow(Convert.ToInt32(o), rpm, TargetLambda);
                    
                    double[] dvals = new double[1];
                    dvals.SetValue(Convert.ToDouble(horsepower), 0);
                    chartControl1.Series[powerSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvals));

                    double[] dvalstorq = new double[1];
                    dvalstorq.SetValue(Convert.ToDouble(torque), 0);
                    chartControl1.Series[torqueSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalstorq));

                    double[] dvalsinjDC = new double[1];
                    dvalsinjDC.SetValue(Convert.ToDouble(injDC), 0);
                    chartControl1.Series[injectorDCSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsinjDC));

                    double[] dvalsLambda = new double[1];
                    dvalsLambda.SetValue(Convert.ToDouble(TargetLambda), 0);
                    chartControl1.Series[lambdaSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsLambda));

                    double[] dvalsEGT = new double[1];
                    dvalsEGT.SetValue(Convert.ToDouble(EstimateEGT), 0);
                    chartControl1.Series[EGTSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsEGT));

                    double[] dvalsFlow = new double[1];
                    dvalsFlow.SetValue(Convert.ToDouble(EstimateFlow), 0);
                    chartControl1.Series[FlowSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsFlow));
                }
            }
            if (m_current_comparefilename != string.Empty)
            {
                AirmassLimitType[] diffBinaryLimitResult;
                DataTable dt2 = CalculateDataTable(m_current_comparefilename, Compare_symbol_collection, out diffBinaryLimitResult);
                LoadExtraGraphFromCompareBin(dt2, m_current_comparefilename, Compare_symbol_collection);
            }

            UpdateGraphVisibility();
        }

        private int CalculateEstimateEGT(int airmass, int rpm)
        {
            int retval = 0;
            
            if (EGTMap != null)
            {
                double egtvalue = GetInterpolatedTableValue(EGTMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                if (fuelVEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                    vecorr /= 128;
                    vecorr = 1 / vecorr;
                    int[] nullvalue = new int[1];
                    nullvalue.SetValue(0, 0);
                    /*double closedLoopLimit = GetInterpolatedTableValue(open_loop, nullvalue, open_loopyaxis, rpm, 0);
                    if (airmass < closedLoopLimit)
                    {
                        vecorr = 1;
                    }*/
                    // if in closed loop correction factor should be ignored

                    egtvalue *= vecorr;
                }
                retval = Convert.ToInt32(egtvalue);
            }
            else
            {
                checkEdit12.Checked = false;
            }
            if (retval > 50 && isFuelE85.Checked) retval -= 50; // correction for E85 fuel, 50 degrees off!
            return retval;
        }

        private int CalculateTargetLambda(int airmass, int rpm, int injDC)
        {
            int retval = 0;
            if (fuelVEMap != null)
            {
                double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                vecorr /= 128;
                vecorr = 1 / vecorr;
                vecorr *= 100; // range correction
				if (injDC > 100)
                {
                    vecorr *= injDC;
                    vecorr /= 100;
                }
                retval = Convert.ToInt32(vecorr);
            }
            return retval;
        }

		private int CalculateFuelFlow(int airmass, int rpm, int lambda)
        {
            int retval = 0;
            // First we calculate how much fuel needs to be injected. Milligram is converted to gram.
            double fuelToInjectPerCycle; // mg/c *1000 = g/c
            if (isFuelE85.Checked)
            {
                // running E85, different target lambda
                fuelToInjectPerCycle = (double)airmass / (9.84F * 1000); // mg/c *1000 = g/c
            }
            else
            {
                fuelToInjectPerCycle = (double)airmass / (14.65F * 1000);
            }
            double dtarget = (double)lambda;
            dtarget /= 100;
            fuelToInjectPerCycle /= dtarget;

            double combustionsPerSecond = rpm * 2 / 60;
            double fuelFlow = fuelToInjectPerCycle * combustionsPerSecond; // g/s

            // convert fuel flow from g/s to l/h
            if (isFuelE85.Checked)
            {
                fuelFlow *= 3600F / (1000F * 0.775F);
                
            }
            else
            {
                fuelFlow *= 3600F / (1000F * 0.742F);
            }
            retval = Convert.ToInt32(fuelFlow);
            return retval;
        }

        private void LoadCompressorMapWithDetails()
        {
            // we should get the top line like for the chart

            if (gridControl1.DataSource != null)
            {
                /*
                TurboType tt = TurboType.TD0415T;
                if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.T25_55 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.T25_60 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT17)
                {
                    tt = TurboType.Stock;
                }
                else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD04 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0416T)
                {
                    tt = TurboType.TD0415T;
                }
                else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0418T || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0419T)
                {
                    tt = TurboType.TD0419T;
                }
                else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT2871R || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT28RS)
                {
                    tt = TurboType.GT28RS;
                }
                else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT3071R86 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT30R)
                {
                    tt = TurboType.GT3071R;
                }
                else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT40R || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.HX40W)
                {
                    tt = TurboType.HX40w;
                }
                */
                DataTable dt = (DataTable)gridControl1.DataSource;
                double[] boost_req = new double[dt.Columns.Count];
                /*PressureToTorque ptt = new PressureToTorque();

                 <GS-09032011> changed to airmass calculation 
                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    // we get the airmass from this.. now convert to boost pressure
                    double trq = AirmassToTorque(Convert.ToInt32(o), Convert.ToInt32(x_axisvalues.GetValue(i)), checkEdit7.Checked);
                    double val = ptt.CalculatePressureFromTorque(trq, tt );
                    boost_req.SetValue(val, i);
                }*/

                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    boost_req.SetValue(o, i);
                }
                ctrlCompressorMap1.Boost_request = boost_req;
                // set rpm range
                ctrlCompressorMap1.Rpm_points = pedal_Xaxis;

                if (!ctrlCompressorMap1.IsInitiallyLoaded)
                {
                    T8Header header = new T8Header();
                    header.init(m_currentfilename);
                    // Partnumber
                    //PartNumberConverter pnc = new PartNumberConverter();
                    //ECUInformation ecuinfo = pnc.GetECUInfo(header.PartNumber.Trim(), "");
                    //ecuinfo.Turbomodel;
                    // VIN
                    VINCarInfo carinfo = VINDecoder.DecodeVINNumber(header.ChassisID);
                    if (carinfo.TurboModel == VINTurboModel.MitsubishiTD04L_14T)
                    {
                        ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.TD04);
                    }
                    else
                    {
                        ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.GT17);
                    }
                    
                    ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter2;
                }

                ctrlCompressorMap1.IsInitiallyLoaded = true;
                ctrlCompressorMap1.Redraw();
            }
        }

        private DataTable CalculateDataTable(string filename, SymbolCollection symbols, out AirmassLimitType[] limitResult)
        {
            limitResult = null;
            // do the math!
            entirefile = File.ReadAllBytes(filename);
            try
            {
                if (IsBinaryBiopower(symbols))
                {
                    isFuelE85.Enabled = true;
                }
                else
                {
                    isFuelE85.Checked = false;
                    isFuelE85.Enabled = false;
                }
                if (isCarAutomatic.Checked)
                {
                    isOverboostActive.Enabled = false;
                }
                else
                {
                    if (IsOverboostEnabled(symbols))
                    {
                        isOverboostActive.Enabled = true;
                    }
                    else
                    {
                        isOverboostActive.Enabled = false;
                    }
                }
                
                // TODO: isCarHighOutput, Get the high output/low output from the loaded binary.
                xdummy.SetValue(0, 0);
                int[] pedalrequestmap = readIntdatafromfile(symbols, "PedalMapCal.Trq_RequestMap");
                limitResult = new AirmassLimitType[pedalrequestmap.Length];
                int[] resulttable = new int[pedalrequestmap.Length]; // result 
                pedal_Rows = GetSymbolLength(symbols, "PedalMapCal.n_EngineMap") / 2;
                pedal_Columns = GetSymbolLength(symbols, "PedalMapCal.X_PedalMap") / 2;
                pedal_Xaxis = readIntdatafromfile(symbols, "PedalMapCal.n_EngineMap");
                pedal_Yaxis = readIntdatafromfile(symbols, "PedalMapCal.X_PedalMap");

                airTorqueMap = readIntdatafromfile(symbols, "TrqMastCal.m_AirTorqMap");
                airTorqueMap_Xaxis = readIntdatafromfile(symbols, "TrqMastCal.Trq_EngXSP");
                airTorqueMap_Yaxis = readIntdatafromfile(symbols, "TrqMastCal.n_EngineYSP");

                bstknkMaxAirmassMap = readIntdatafromfile(symbols, "BstKnkCal.MaxAirmass");
                string bstknkMaxAirmassMap_XaxisName = "BstKnkCal.OffsetXSP";
                if (!SymbolExists(bstknkMaxAirmassMap_XaxisName, symbols))
                {
                    bstknkMaxAirmassMap_XaxisName = "BstKnkCal.fi_offsetXSP"; // in case of flexifuel binary
                }
                bstknkMaxAirmassMap_Xaxis = readIntdatafromfile(symbols, bstknkMaxAirmassMap_XaxisName);
                for (int a = 0; a < bstknkMaxAirmassMap_Xaxis.Length; a++)
                {
                    int val = (int)bstknkMaxAirmassMap_Xaxis.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    bstknkMaxAirmassMap_Xaxis.SetValue(val, a);
                }
                bstknkMaxAirmassMap_Yaxis = readIntdatafromfile(symbols, "BstKnkCal.n_EngYSP");

                if (SymbolExists("BstKnkCal.MaxAirmassAu", symbols))
                {
                    bstknkMaxAirmassAuMap = readIntdatafromfile(symbols, "BstKnkCal.MaxAirmassAu");
                }
                else 
                {
                    bstknkMaxAirmassAuMap = readIntdatafromfile(symbols, "BstKnkCal.MaxAirmass");
                }

                nominalTorqueMap = readIntdatafromfile(symbols, "TrqMastCal.Trq_NominalMap");
                nominalTorqueMap_Xaxis = readIntdatafromfile(symbols, "TrqMastCal.m_AirXSP");
                nominalTorqueMap_Yaxis = readIntdatafromfile(symbols, "TrqMastCal.n_EngineYSP");
                for (int a = 0; a < nominalTorqueMap.Length; a++)
                {
                    int val = (int)nominalTorqueMap.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominalTorqueMap.SetValue(val, a);
                }

                fuelcutAirInletLimit = Convert.ToInt32(readIntdatafromfile(symbols, "FCutCal.m_AirInletLimit").GetValue(0));

                if (isFuelE85.Checked)
                {
                    ffMaxAirmassMap = readIntdatafromfile(symbols, "FFAirCal.m_maxAirmass");
                    ffMaxAirmassMap_Xaxis = readIntdatafromfile(symbols, "FFAirCal.fi_offsetXSP");
                    for (int a = 0; a < ffMaxAirmassMap_Xaxis.Length; a++)
                    {
                        int val = (int)ffMaxAirmassMap_Xaxis.GetValue(a);
                        if (val > 32000) val = -(65536 - val);
                        ffMaxAirmassMap_Xaxis.SetValue(val, a);
                    }
                }

                // Try old style TrqLimCal.Trq_MaxEngineManTab1/2 or TrqLimCal.Trq_MaxEngineAutTab1/2
                string engineTorqueLimiter;
                if (isCarAutomatic.Checked)
                {
                    if (isCarHighOutput.Checked)
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineAutTab1";
                    }
                    else
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineAutTab2";
                    }
                }
                else
                {
                    if (isCarHighOutput.Checked)
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineManTab1";
                    }
                    else
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineManTab2";
                    }
                }

                if (!SymbolExists(engineTorqueLimiter, symbols))
                {
                    // If the old style symbol does not exist, default to the newer style TrqLimCal.Trq_MaxEngineTab1/2
                    if (isCarHighOutput.Checked)
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineTab1";
                    }
                    else
                    {
                        engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineTab2";
                    }
                }
                enginetorquelim = readIntdatafromfile(symbols, engineTorqueLimiter);
                if (isFuelE85.Checked)
                {
                    if (isCarHighOutput.Checked)
                    {
                        enginetorquelimE85 = readIntdatafromfile(symbols, "FFTrqCal.FFTrq_MaxEngineTab1");
                    }
                    else
                    {
                        enginetorquelimE85 = readIntdatafromfile(symbols, "FFTrqCal.FFTrq_MaxEngineTab2");
                    }
                }
                enginetorquelimOverboost = readIntdatafromfile(symbols, "TrqLimCal.Trq_OverBoostTab");

                enginetorquelimGear = readIntdatafromfile(symbols, "TrqLimCal.Trq_ManGear");
                
                // Newer style automatic torque limits
                if (isCarHighOutput.Checked)
                {
                    enginetorquelimAuto = readIntdatafromfile(symbols, "TMCCal.Trq_MaxEngineTab");
                }
                else
                {
                    enginetorquelimAuto = readIntdatafromfile(symbols, "TMCCal.Trq_MaxEngineLowTab");
                }

                try
                {
                    int[] injConstant = readIntdatafromfile(symbols, "InjCorrCal.InjectorConst");
                    injectorConstant = Convert.ToInt32(injConstant.GetValue(0));
                    injectorBatteryCorrection = readIntdatafromfile(symbols, "InjCorrCal.BattCorrTab");
                    injectorBatteryCorrection_axis = readIntdatafromfile(symbols, "InjCorrCal.BattCorrSP");
                    if (isFuelE85.Checked)
                    {
                        fuelVEMap = readdatafromfile(symbols, "FFFuelCal.TempEnrichFacMAP");
                    }
                    else
                    {
                        fuelVEMap = readdatafromfile(symbols, "BFuelCal.TempEnrichFacMap");
                    }
                    fuelVE_Xaxis = readIntdatafromfile(symbols, "BFuelCal.AirXSP");
                    fuelVE_Yaxis = readIntdatafromfile(symbols, "BFuelCal.RpmYSP");

                    if (SymbolExists("ExhaustCal.T_Lambda1Map", symbols))
                    {
                        EGTMap = readIntdatafromfile(symbols, "ExhaustCal.T_Lambda1Map");
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E);
                }

                // step thru the complete pedal and rpm range
                for (int colcount = 0; colcount < pedal_Columns; colcount++)
                {
                    for (int rowcount = 0; rowcount < pedal_Rows; rowcount++)
                    {
                        // get the current value from the request map
                        int rpm = (int)pedal_Xaxis.GetValue(rowcount);
                        int requestedtorque = (int)pedalrequestmap.GetValue((colcount * pedal_Rows) + rowcount);
                        int airmassrequestforcell = TorqueToAirmass(requestedtorque, rpm);

                        AirmassLimitType limiterType = AirmassLimitType.None;
                        int resultingAirMass = CalculateMaxAirmassforcell(rpm, airmassrequestforcell, out limiterType);
                        resulttable.SetValue(resultingAirMass, (colcount * pedal_Rows) + rowcount);
                        limitResult.SetValue(limiterType, (colcount * pedal_Rows) + rowcount);
                    }
                }
                // now show resulttable
                DataTable dt = new DataTable();
                foreach (int xvalue in pedal_Xaxis)
                {
                    dt.Columns.Add(xvalue.ToString());
                }
                // now fill the table rows
                m_MaxValueInTable = 0;
                for (int r = pedal_Yaxis.Length - 1; r >= 0; r--)
                {
                    object[] values = new object[pedal_Columns];
                    for (int t = 0; t < pedal_Xaxis.Length; t++)
                    {
                        int currValue = (int)resulttable.GetValue((r * pedal_Columns) + t);
                        if (currValue > m_MaxValueInTable) m_MaxValueInTable = currValue;
                        values.SetValue(resulttable.GetValue((r * pedal_Columns) + t), t);
                    }
                    dt.Rows.Add(values);
                }
                return dt;
            }
            catch (Exception)
            {
                logger.Debug("Failed to calculate for file : " + filename);
            }
            return null;
        }

        private void UpdateGraphVisibility()
        {
            if (powerSeries >= 0)
            {
                chartControl1.Series[powerSeries].Visible = checkEdit8.Checked;
            }
            if (powerCompareSeries >= 0)
            {
                chartControl1.Series[powerCompareSeries].Visible = checkEdit8.Checked;
            }
            if (torqueSeries >= 0)
            {
                chartControl1.Series[torqueSeries].Visible = checkEdit9.Checked;
            }
            if (torqueCompareSeries >= 0)
            {
                chartControl1.Series[torqueCompareSeries].Visible = checkEdit9.Checked;
            }
            if (injectorDCSeries >= 0)
            {
                chartControl1.Series[injectorDCSeries].Visible = checkEdit10.Checked;
            }
            if (lambdaSeries >= 0)
            {
                chartControl1.Series[lambdaSeries].Visible = checkEdit11.Checked;
            }
            if (EGTSeries >= 0)
            {
                chartControl1.Series[EGTSeries].Visible = checkEdit12.Checked;
            }
            if (FlowSeries >= 0)
            {
                chartControl1.Series[FlowSeries].Visible = showFuelFlow.Checked;
            }
        }

        string m_current_comparefilename = string.Empty;
        SymbolCollection Compare_symbol_collection = new SymbolCollection();

        private int CalculateInjectorDCusingPulseWidth(int airmass, int rpm)
        {
            // calculate injector DC
            // needs injector constant, ve map, battery correction map
            int retval = 0;
            // first calculcate simple
            if (injectorConstant > 0 && fuelVEMap != null)
            {
                // First we calculate how much fuel needs to be injected. Milligram is converted to gram.
                double fuelToInjectPerCycle = (double)airmass / (14.65F * 1000); // mg/c *1000 = g/c
                if (isFuelE85.Checked)
                {
                    // running E85, different target lambda
                    fuelToInjectPerCycle = (double)airmass / (9.84F * 1000); // mg/c *1000 = g/c
                }

                // Calculate how much fuel do we get for every millisecond of injection
                double injectorFuelPerMs = (double)injectorConstant / (60 * 1000);

                double baseFuelPulseWidth = fuelToInjectPerCycle / injectorFuelPerMs;

                // get correct data from fuelVEmap ... by airmass and rpm
                double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                vecorr /= 128; //table is a byte value, with range 0.00-2.55 , 1.00 is 128
                baseFuelPulseWidth *= (double)vecorr;

                // Calculate DC from pulse width
                double injectorDCbase = (double)(baseFuelPulseWidth * rpm) / 1200;
                retval = Convert.ToInt32(Math.Round(injectorDCbase, 0));

                if (retval < 100)
                {
                    // Add battery correction only if DC has not reached 100%
                    int[] nullvalue = new int[1];
                    nullvalue.SetValue(0, 0);
                    double batteryCorrectionValue = GetInterpolatedTableValue(injectorBatteryCorrection, nullvalue, injectorBatteryCorrection_axis, 130, 0); // 13.0V
                    batteryCorrectionValue /= 1000; // ms
                    double finalFuelPulseWidth = baseFuelPulseWidth + batteryCorrectionValue;

                    // Calculate DC from pulse width plus batterycorrection contribution
                    double injectorDCbattcorr = (double)(finalFuelPulseWidth * rpm) / 1200;
                    retval = Convert.ToInt32(Math.Round(injectorDCbattcorr, 0));
                }
            }
            return retval;
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Binary files|*.bin" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_current_comparefilename = ofd.FileName;
                Compare_symbol_collection = new SymbolCollection();

                if (!Trionic8File.ValidateTrionic8File(m_current_comparefilename))
                {
                    m_current_comparefilename = string.Empty;
                    return;
                }

                Trionic8File.TryToExtractPackedBinary(m_current_comparefilename, out Compare_symbol_collection);
                // try to load additional symboltranslations that the user entered
                Trionic8File.TryToLoadAdditionalBinSymbols(m_current_comparefilename, Compare_symbol_collection);

                // so... now determine the max values for the compare file
                // show the dynograph
                if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
                {
                    LoadGraphWithDetails();
                }
                else
                {
                    xtraTabControl1.SelectedTabPage = xtraTabPage2;
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            CastCloseEvent();
        }

        public delegate void ViewerClose(object sender, EventArgs e);
        public event ctrlAirmassResult.ViewerClose onClose;

        private void CastCloseEvent()
        {
            if (onClose != null)
            {
                onClose(this, EventArgs.Empty);
            }
        }

        private void isCarAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void isOverboostActive_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void isCarHighOutput_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == tabCompressormap.Name)
            {
                LoadCompressorMapWithDetails();
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void useTrionicCalculationForTorque_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void displayPowerInkW_CheckedChanged(object sender, EventArgs e)
        {
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == tabCompressormap.Name)
            {
                LoadCompressorMapWithDetails();
            }
        }

        private void displayTorqueInLBFT_CheckedChanged(object sender, EventArgs e)
        {
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
            else if (xtraTabControl1.SelectedTabPage.Name == tabCompressormap.Name)
            {
                LoadCompressorMapWithDetails();
            }
        }

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            // start airmass limiter viewer
            if (isCarAutomatic.Checked)
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmassAu");
            }
            else if (isFuelE85.Checked)
            {
                CastStartViewerEvent("FFAirCal.m_maxAirmass");
            }
            else
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmass");
            }
        }

        private void labelControl2_DoubleClick(object sender, EventArgs e)
        {
            // start E85 torque limiter
            if (isCarHighOutput.Checked)
            {
                CastStartViewerEvent("FFTrqCal.FFTrq_MaxEngineTab1"); // high output 175/200hp
            }
            else
            {
                CastStartViewerEvent("FFTrqCal.FFTrq_MaxEngineTab2"); // low output 150hp
            }
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            // start engine torque limiter
            // Try old style TrqLimCal.Trq_MaxEngineManTab1/2 or TrqLimCal.Trq_MaxEngineAutTab1/2
            string engineTorqueLimiter;
            if (isCarAutomatic.Checked)
            {
                if (isCarHighOutput.Checked)
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineAutTab1";
                }
                else
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineAutTab2";
                }
            }
            else
            {
                if (isCarHighOutput.Checked)
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineManTab1";
                }
                else
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineManTab2";
                }
            }

            if (!SymbolExists(engineTorqueLimiter, m_symbols))
            {
                // If the old style symbol does not exist, default to the newer style TrqLimCal.Trq_MaxEngineTab1/2
                if (isCarHighOutput.Checked)
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineTab1";
                }
                else
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineTab2";
                }
            }
            CastStartViewerEvent(engineTorqueLimiter);
        }

        private void labelControl12_DoubleClick(object sender, EventArgs e)
        {
            // gear torque limiters 
            if (isCarAutomatic.Checked)
            {
                // Newer style automatic torque limits
                if (isCarHighOutput.Checked)
                {
                    CastStartViewerEvent("TMCCal.Trq_MaxEngineTab");
                }
                else
                {
                    CastStartViewerEvent("TMCCal.Trq_MaxEngineLowTab");
                }
            }
            else
            {
                CastStartViewerEvent("TrqLimCal.Trq_ManGear");
            }
        }

        private void labelControl14_DoubleClick(object sender, EventArgs e)
        {
            CastStartViewerEvent("FCutCal.m_AirInletLimit");
        }

        private void labelControl16_DoubleClick(object sender, EventArgs e)
        {
            // show overboost tab
            CastStartViewerEvent("TrqLimCal.Trq_OverBoostTab");
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage1.Name)
            {
                // in table view
            }
            else if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                // in graph view
                LoadGraphWithDetails();
            }
            else
            {
                // compressor map plotter
                LoadCompressorMapWithDetails();
            }
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        int b = 0;
                        int cellvalue = 0;
                        //if (m_isHexMode)
                        b = Convert.ToInt32(e.CellValue.ToString());
                        cellvalue = b;
                        b *= 255;
                        if (m_MaxValueInTable != 0)
                        {
                            b /= m_MaxValueInTable;
                        }
                        int red = 128;
                        int green = 128;
                        int blue = 128;
                        Color c = Color.White;
                        red = b;
                        if (red < 0) red = 0;
                        if (red > 255) red = 255;
                        if (b > 255) b = 255;
                        blue = 0;
                        green = 255 - red;
                        c = Color.FromArgb(red, green, blue);
                        SolidBrush sb = new SolidBrush(c);
                        e.Graphics.FillRectangle(sb, e.Bounds);

                        // check limiter type
                        int row = pedal_Rows - (e.RowHandle + 1);
                        AirmassLimitType curLimit = (AirmassLimitType)limiterResult.GetValue((row * pedal_Columns) + e.Column.AbsoluteIndex);
                        Point[] pnts = new Point[4];
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 0);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width - (e.Bounds.Height / 2), e.Bounds.Y), 1);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 3);
                        if (curLimit == AirmassLimitType.AirmassLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Blue, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == AirmassLimitType.TorqueLimiterEngineE85)
                        {
                            e.Graphics.FillPolygon(Brushes.Purple, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == AirmassLimitType.TorqueLimiterEngine)
                        {
                            e.Graphics.FillPolygon(Brushes.Yellow, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == AirmassLimitType.TorqueLimiterGear)
                        {
                            e.Graphics.FillPolygon(Brushes.SaddleBrown, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == AirmassLimitType.FuelCutLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.DarkGray, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == AirmassLimitType.OverBoostLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.CornflowerBlue, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        if (cbTableSelectionEdit.SelectedIndex == 1)
                        {
                            // convert airmass to torque
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int torque;
                            if (displayTorqueInLBFT.Checked)
                            {
                                torque = TorqueToTorqueLbft(AirmassToTorque(m_symbols, Convert.ToInt32(e.CellValue), rpm));
                            }
                            else
                            {
                                torque = AirmassToTorque(m_symbols, Convert.ToInt32(e.CellValue), rpm);
                            }
                            e.DisplayText = torque.ToString();
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 2)
                        {
                            //convert airmass to horsepower
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int torque = AirmassToTorque(m_symbols, Convert.ToInt32(e.CellValue), rpm);
                            int horsepower;
                            if (displayPowerInkW.Checked)
                            {
                                horsepower = TorqueToPowerkW(torque, rpm);
                            }
                            else
                            {
                                horsepower = TorqueToPower(torque, rpm);
                            }
                            e.DisplayText = horsepower.ToString();
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 3) //injector DC
                        {
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int injDC = CalculateInjectorDCusingPulseWidth(Convert.ToInt32(e.CellValue), rpm);
                            if (injDC > 100)
                            {
                                injDC = 100;
                            }
                            e.DisplayText = injDC.ToString();
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 4) //target lambda
                        {
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int injDC = CalculateInjectorDCusingPulseWidth(Convert.ToInt32(e.CellValue), rpm);
                            int targetLambda = CalculateTargetLambda(Convert.ToInt32(e.CellValue), rpm, injDC);
                            float dtarget = (float)targetLambda;
                            dtarget /= 100;
                            e.DisplayText = dtarget.ToString("F2");

                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 5) //target AFR
                        {
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int injDC = CalculateInjectorDCusingPulseWidth(Convert.ToInt32(e.CellValue), rpm);
                            int targetLambda = CalculateTargetLambda(Convert.ToInt32(e.CellValue), rpm, injDC);
                            float dtarget = (float)targetLambda;
                            dtarget /= 100;
                            if (isFuelE85.Checked)
                            {
                                // E85
                                dtarget *= 9.76F;
                            }
                            else
                            {
                                dtarget *= 14.7F;
                            }
                            e.DisplayText = dtarget.ToString("F2");
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 6)
                        {
                            // convert to estimated EGT
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int airmass = Convert.ToInt32(e.CellValue);
                            int egt = CalculateEstimateEGT(airmass, rpm);
                            e.DisplayText = egt.ToString();
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 7)
                        {
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int injDC = CalculateInjectorDCusingPulseWidth(Convert.ToInt32(e.CellValue), rpm);
                            int targetLambda = CalculateTargetLambda(Convert.ToInt32(e.CellValue), rpm, injDC);
                            int fuelflow = CalculateFuelFlow(Convert.ToInt32(e.CellValue), rpm, targetLambda);
                            e.DisplayText = fuelflow.ToString("F2");
                        }
                        else
                        {
                            int airmass = Convert.ToInt32(e.CellValue);
                            e.DisplayText = airmass.ToString();
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {

                //  e.Painter.DrawCaption(new DevExpress.Utils.Drawing.ObjectInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics)), "As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, null);
                // e.Cache.DrawString("As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, new StringFormat());
                try
                {
                    if (pedal_Yaxis.Length > 0)
                    {
                        if (pedal_Yaxis.Length > e.RowHandle)
                        {
                            int value = (int)pedal_Yaxis.GetValue((pedal_Yaxis.Length - 1) - e.RowHandle);
                            value /= 10;
                            string yvalue = value.ToString();
                            /*if (!m_isUpsideDown)
                            {
                                // dan andere waarde nemen
                                yvalue = y_axisvalues.GetValue(e.RowHandle).ToString();
                            }
                            if (m_y_axis_name == "MAP")
                            {
                                if (m_viewtype == ViewType.Easy3Bar || m_viewtype == ViewType.Decimal3Bar)
                                {
                                    int tempval = Convert.ToInt32(y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle));
                                    if (!m_isUpsideDown)
                                    {
                                        tempval = Convert.ToInt32(y_axisvalues.GetValue(e.RowHandle));
                                    }
                                    tempval *= 120;
                                    tempval /= 100;
                                    yvalue = tempval.ToString();
                                }
                            }*/

                            Rectangle r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
                            e.Graphics.DrawRectangle(Pens.LightSteelBlue, r);
                            System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, e.Appearance.BackColor2, e.Appearance.BackColor2, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(gb, e.Bounds);
                            e.Graphics.DrawString(yvalue, this.Font, Brushes.MidnightBlue, new PointF(e.Bounds.X + 4, e.Bounds.Y + 1 + (e.Bounds.Height - 12) / 2));
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E);
                }
            }
        }

        private void ctrlCompressorMap1_onRefreshData(object sender, EventArgs e)
        {
            if (sender is ctrlCompressorMap)
            {
                // refresh the entire airmass viewer for data in the binary might have changed
                Calculate();
                if (gridControl1.DataSource != null)
                {
                    /*
                    TurboType tt = TurboType.TD0415T;
                    if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.T25_55 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.T25_60 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT17)
                    {
                        tt = TurboType.Stock;
                    }
                    else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD04 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0416T)
                    {
                        tt = TurboType.TD0415T;
                    }
                    else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0418T || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.TD0419T)
                    {
                        tt = TurboType.TD0419T;
                    }
                    else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT2871R || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT28RS)
                    {
                        tt = TurboType.GT28RS;
                    }
                    else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT3071R86 || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT30R)
                    {
                        tt = TurboType.GT3071R;
                    }
                    else if (ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.GT40R || ctrlCompressorMap1.Compressor == ctrlCompressorMap.CompressorMap.HX40W)
                    {
                        tt = TurboType.HX40w;
                    }
                    */
                    DataTable dt = (DataTable)gridControl1.DataSource;

                    double[] boost_req = new double[dt.Columns.Count];
                    /*PressureToTorque ptt = new PressureToTorque();
                    for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                        // we get the airmass from this.. now convert to boost pressure
                        double trq = AirmassToTorque(Convert.ToInt32(o), Convert.ToInt32(x_axisvalues.GetValue(i)), checkEdit7.Checked);
                        double val = ptt.CalculatePressureFromTorque(trq, tt);
                        boost_req.SetValue(val, i);
                    }*/
                    for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                    {
                        double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                        boost_req.SetValue(o, i);
                    }
                    ctrlCompressorMap1.Boost_request = boost_req;

                    // set rpm range
                    ctrlCompressorMap1.Rpm_points = pedal_Xaxis;
                    //PartNumberConverter pnc = new PartNumberConverter();
                    //
                    /*T7FileHeader header = new T7FileHeader();
                    header.init(m_currentfile, false);
                    ECUInformation ecuinfo = pnc.GetECUInfo(header.getPartNumber().Trim(), "");
                    if (ecuinfo.Is2point3liter) ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter23;
                    else ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter2;*/
                    //ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.TD04); //TODO: always for now, later rewrite and add the GT17 map
                    ctrlCompressorMap1.Redraw();
                }
            }
        }

        private void checkEdit8_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraphVisibility();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (chartControl1.IsPrintingAvailable)
            {
                printToolStripMenuItem.Enabled = true;
            }
            else
            {
                printToolStripMenuItem.Enabled = false;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "JPEG images|*.jpg" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    chartControl1.ExportToImage(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;
            chartControl1.ShowPrintPreview();
        }
    }
}
