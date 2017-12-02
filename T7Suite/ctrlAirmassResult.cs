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
using TrionicCANLib.Checksum;

namespace T7
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
        private byte[] E85VEMap;
        private int[] open_loop;       
        private int[] open_loopyaxis;

        private int[] EGTMap;
        
        private int[] airTorqueMap;
        private int[] airTorqueMap_Xaxis;
        private int[] airTorqueMap_Yaxis;
        
        private int[] bstknkMaxAirmassMap;
        private int[] bstknkMaxAirmassMap_Xaxis;
        private int[] bstknkMaxAirmassMap_Yaxis;
        private int[] bstknkMaxAirmassAuMap;

        private int[] turbospeed;
        private int[] turbospeed_Yaxis;
        private int[] turbospeed2;
        private int[] turbospeed2_Yaxis;

        private int[] nominalTorqueMap;
        private int[] nominalTorqueMap_Xaxis;
        private int[] nominalTorqueMap_Yaxis;

        private byte[] entirefile;

        private int fuelcutAirInletLimit;

        private int[] enginetorquelimOverboost;
        private int[] enginetorquelim;
        private int[] enginetorquelimE85;
        private int[] enginetorquelimE85Auto;
        private int[] enginetorquelimGear;
        private int[] enginetorquelimAuto;
        private int[] enginetorquelimConvertible;
        private int[] enginetorque_Yaxis;
        private int[] enginetorquelimReverse;
        private int[] enginetorquelimReverse_Yaxis;
        private int[] enginetorquelim1st;
        private int[] enginetorquelim1st_Yaxis;
        private int[] enginetorquelim5th;
        private int[] enginetorquelim5th_Yaxis;

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

        private bool _softwareIsOpen = false;
        private bool _softwareIsOpenDetermined = false;

        private bool IsSoftwareOpen()
        {
            bool retval = false;
            if (_softwareIsOpenDetermined) return _softwareIsOpen;

            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Flash_start_address > m_currentfile_size && sh.Length > 0x100 && sh.Length < 0x400)
                {
                    if (sh.Varname == "BFuelCal.Map" || sh.Varname == "IgnNormCal.Map" || sh.Varname == "AirCtrlCal.map"
                        || sh.Userdescription == "BFuelCal.Map" || sh.Userdescription == "IgnNormCal.Map")
                    {
                        retval = true; // found maps > 0x100 in size in sram
                        _softwareIsOpen = true;
                        //                        logger.Debug("Software is open because of symbol: " + sh.Varname);
                    }
                }
            }
            _softwareIsOpenDetermined = true;
            return retval;
        }

        private int m_currentSramOffsett = 0;

        public int CurrentSramOffsett
        {
            get { return m_currentSramOffsett; }
            set { m_currentSramOffsett = value; }
        }

        private int GetOpenFileOffset()
        {
            // try to find a KNOWN table (which is always more or less similar
            if (m_currentSramOffsett > 0)
            {
                //logger.Debug("Working with: " + m_currentSramOffsett.ToString("X8"));
                return m_currentSramOffsett;
            }
            //     34FCEF00
            // 48 bytes ernaast (0x30)
            //return 0xEFFC34; // autodetect the offset!!!
            return 0xEFFC04; // autodetect the offset!!!

        }

        private int m_currentfile_size = 0x80000;

        public int Currentfile_size
        {
            get { return m_currentfile_size; }
            set { m_currentfile_size = value; }
        }

        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    if (IsSoftwareOpen() /*&& sh.Length >= 0x02 */&& sh.Length < 0x400 && sh.Flash_start_address > m_currentfile_size) // <GS-09082010>
                    {
                        return sh.Flash_start_address - GetOpenFileOffset();
                    }
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }
        private int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
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

        private byte[] readdatafromfile(string filename, int address, int length)
        {
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

        private int[] readIntdatafromfile(string filename, int address, int length)
        {
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

        private bool IsOverboostEnabled(string filename, SymbolCollection symbols)
        {
            bool retval = false;
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TorqueCal.EnableOverBoost")
                {
                    // read data from file and verify if the value is > 0
                    byte[] overboostenable = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.EnableOverBoost"), (int)GetSymbolLength(symbols, "TorqueCal.EnableOverBoost"));
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
                if (sh.SmartVarname == "TorqueCal.M_EngMaxE85Tab")
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsBinaryConvertable(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TorqueCal.M_CabGearLim")
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsBinaryBiopowerAuto(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TorqueCal.M_EngMaxE85TabAut")
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Has1stGearLimit(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TorqueCal.M_1GearTab")
                {
                    return true;
                }
            }
            return false;
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
                if (sh.SmartVarname == symbolname)
                    return true;
            }
            return false;
        }

        private int CalculateMaxAirmassforcell(int rpm, int requestairmass, bool autogearbox, bool E85, bool Convertable, bool OverboostEnabled, bool E85Automatic, bool TorqueLimitEnabled, bool Gear1stLimitAvailable, out AirmassLimitType limiterType)
        {
            int restrictedairmass = requestairmass;
            //logger.Debug("Pedalpos: " + pedalposition + " Rpm: " + rpm + " requests: " + requestairmass + " mg/c");
            //calculate the restricted airmass for the current point
            
            limiterType = AirmassLimitType.None;

            if (TorqueLimitEnabled)
            {
                AirmassLimitType TrqLimiterType = AirmassLimitType.None;
                restrictedairmass = CheckAgainstTorqueLimiters(rpm, requestairmass, E85, Convertable, autogearbox, OverboostEnabled, E85Automatic, Gear1stLimitAvailable, out TrqLimiterType);
                if (restrictedairmass < requestairmass)
                {
                    limiterType = TrqLimiterType;
                }
            }

            AirmassLimitType AirmassLimiterType = AirmassLimitType.None;
            int TorqueLimitedAirmass = restrictedairmass;

            restrictedairmass = CheckAgainstAirmassLimiters(rpm, restrictedairmass, autogearbox, ref AirmassLimiterType);

            restrictedairmass = CheckAgainstTurboSpeedLimiter(rpm, restrictedairmass, ref AirmassLimiterType);

            // finally check agains fuelcut limiter
            restrictedairmass = CheckAgainstFuelcutLimiter(restrictedairmass, ref AirmassLimiterType);
            if (restrictedairmass < TorqueLimitedAirmass)
            {
                limiterType = AirmassLimiterType;
            }
            return restrictedairmass;
        }

        private bool HasBinaryTorqueLimiterEnabled(SymbolCollection symbols, string filename)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == "TorqueCal.ST_Loop")
                {

                    byte[] torquelimdata = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.ST_Loop"), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)torquelimdata.GetValue(0) == 0x00)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private int CheckAgainstTorqueLimiters(int rpm, int requestedairmass, bool E85, bool Convertable, bool Automatic, bool OverboostEnabled, bool E85Automatic, bool Gear1stLimitAvailable, out AirmassLimitType TrqLimiter)
        {
            TrqLimiter = AirmassLimitType.None;
            int LimitedAirMass = requestedairmass;
            int torque = TORQUE_LIMIT_400NM; // Basefile hardcoded limiter
                
            if (E85Automatic && E85 && Automatic)
            {
                int torquelimitE85Auto = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimE85Auto, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (torque > torquelimitE85Auto)
                {
                    logger.Debug(String.Format("Torque E85Autolimit is limited from {0} to {1} at {2} rpm", torque, torquelimitE85Auto, rpm));
                    torque = torquelimitE85Auto;
                    TrqLimiter = AirmassLimitType.TorqueLimiterEngineE85Auto;
                }
            }
            else if (E85)
            {
                int torquelimitE85 = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimE85, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (torque > torquelimitE85)
                {
                    logger.Debug(String.Format("Torque E85limit is limited from {0} to {1} at {2} rpm", torque, torquelimitE85, rpm));
                    torque = torquelimitE85;
                    TrqLimiter = AirmassLimitType.TorqueLimiterEngineE85;
                }
            }
            else if (Automatic)
            {
                int torquelimitAuto = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimAuto, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (torque > torquelimitAuto)
                {
                    logger.Debug(String.Format("Torque Autolimit is limited from {0} to {1} at {2} rpm", torque, torquelimitAuto, rpm));
                    torque = torquelimitAuto;
                    TrqLimiter = AirmassLimitType.TorqueLimiterEngine;
                }
            }
            else
            {
                int torquelimitOverboost;
                if (OverboostEnabled)
                    torquelimitOverboost = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimOverboost, xdummy, airTorqueMap_Yaxis, rpm, 0));
                else
                    torquelimitOverboost = 0;

                int torquelimitPetrol = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, airTorqueMap_Yaxis, rpm, 0));
                if (OverboostEnabled && torque > torquelimitOverboost)
                {
                    logger.Debug(String.Format("Torque OverBoostLimit is limited from {0} to {1} at {2} rpm", torque, torquelimitOverboost, rpm));
                    torque = torquelimitOverboost;
                    TrqLimiter = AirmassLimitType.OverBoostLimiter;
                }
                else if (OverboostEnabled && torque < torquelimitOverboost && torque > torquelimitPetrol)
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

            if (!isCarAutomatic.Checked)
            {
                int[] gears = new int[6];
                gears.SetValue(0, 0);
                gears.SetValue(1, 1);
                gears.SetValue(2, 2);
                gears.SetValue(3, 3);
                gears.SetValue(4, 4);
                gears.SetValue(5, 5);

                if (Convertable)
                {
                    int torquelimitConvertable = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimConvertible, xdummy, gears, comboBoxEdit1.SelectedIndex, 0));
                    if (torque > torquelimitConvertable)
                    {
                        torque = torquelimitConvertable;
                        TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                        logger.Debug("Convertable gear torque limit hit");
                    }
                }
                
                int torquelimitManual = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelimGear, xdummy, gears, comboBoxEdit1.SelectedIndex, 0));
                if (torque > torquelimitManual)
                {
                    torque = torquelimitManual;
                    logger.Debug(String.Format("Manual gear torque limited from {0} to {1} at {2} rpm", torque, torquelimitManual, rpm));
                    TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                }
                
                // and check 5th gear limiter as well!!! (if checkbox is 5)
                if (comboBoxEdit1.SelectedIndex == 5)
                {
                    int torquelimit5th = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim5th, xdummy, enginetorquelim5th_Yaxis, rpm, 0));
                    if (torque > torquelimit5th)
                    {
                        torque = torquelimit5th;
                        TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                        logger.Debug("Fifth gear torque limit hit");
                    }
                }
            }
            else
            {
                if (torque > TORQUE_LIMIT_350NM)
                {
                    logger.Debug(String.Format("Automatic gear torque limited from {0} to {1} at {2} rpm", torque, TORQUE_LIMIT_350NM, rpm));
                    torque = TORQUE_LIMIT_350NM;
                    TrqLimiter = AirmassLimitType.TorqueLimiterGear;
                }
            }
            
            int TestLimitedAirmass = Convert.ToInt32(GetInterpolatedTableValue(airTorqueMap, airTorqueMap_Xaxis, enginetorque_Yaxis, rpm, torque));
            if (TestLimitedAirmass < LimitedAirMass)
            {
                LimitedAirMass = TestLimitedAirmass;
                if(TrqLimiter == AirmassLimitType.None)
                    TrqLimiter = AirmassLimitType.AirTorqueCalibration;
            }

            if (TrqLimiter == AirmassLimitType.None)
                LimitedAirMass = requestedairmass;

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

        private int CheckAgainstTurboSpeedLimiter(int rpm, int requestedairmass, ref AirmassLimitType AirmassLimiter)
        {
            int ambientpressure = Convert.ToInt32(spinEdit1.EditValue) * 10; // 100.0 kPa = 1000 as table value unit is 0.1 kPa
            int airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(turbospeed, xdummy, turbospeed_Yaxis, ambientpressure, 0));

            // Second limitation is based on engine rpm to prevent the turbo from overspeeding at high rpm.
            // Interpolated correction factor applied to the calculated airmass value from main turbospeed table LimEngCal.TurboSpeedTab.
            int correctionfactor = Convert.ToInt32(GetInterpolatedTableValue(turbospeed2, xdummy, turbospeed2_Yaxis, rpm, 0));
            airmasslimit = airmasslimit * correctionfactor / 1000; // correction factor value unit is 0.001
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = AirmassLimitType.TurboSpeedLimiter;
            }
            return requestedairmass;
        }

        private int CheckAgainstAirmassLimiters(int rpm, int requestedairmass, bool autogearbox, ref AirmassLimitType AirmassLimiter)
        {
            int airmasslimit = requestedairmass;
            string message;

            if (autogearbox)
            {
                airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknkMaxAirmassAuMap, bstknkMaxAirmassMap_Xaxis, bstknkMaxAirmassMap_Yaxis, rpm, 0));
                message = String.Format("Reduced airmass because of BstKnkCal.MaxAirmassAu: {0} rpm: {1}", requestedairmass, rpm);
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
                //logger.Debug("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
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
                //logger.Debug("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
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

                //logger.Debug("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //logger.Debug("result = " + result.ToString());
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
                //logger.Debug("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
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
                //logger.Debug("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
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

                //logger.Debug("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //logger.Debug("result = " + result.ToString());
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
        
        private int AirmassToTorque(int airmass, int rpm, bool TrionicStyle)
        {
            double tq;
            if (TrionicStyle)
            {
                tq = GetInterpolatedTableValue(nominalTorqueMap, nominalTorqueMap_Xaxis, nominalTorqueMap_Yaxis, rpm, airmass);
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
                powerLabel = "Power (kW) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            else
                powerLabel = "Power (bhp) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            string torqueLabel;
            if (displayTorqueInLBFT.Checked)
                torqueLabel = "Torque (lbft) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            else
                torqueLabel = "Torque (Nm) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
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
                int torque = AirmassToTorque(Convert.ToInt32(o), rpm, useTrionicCalculationForTorque.Checked);
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
                    powerLabel = "Power (kW)";
                else
                    powerLabel = "Power (bhp)";
                string torqueLabel;
                if (displayTorqueInLBFT.Checked)
                    torqueLabel = "Torque (lbft)";
                else
                    torqueLabel = "Torque (Nm)";

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
                    int torque = AirmassToTorque(Convert.ToInt32(o), rpm, useTrionicCalculationForTorque.Checked);
                    
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
                if (isFuelE85.Checked)
                {
                    if (E85VEMap != null)
                    {
                        double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                        vecorr /= 100;
                        vecorr = 1 / vecorr;
                        int[] nullvalue = new int[1];
                        nullvalue.SetValue(0, 0);
                        double closedLoopLimit = GetInterpolatedTableValue(open_loop, nullvalue, open_loopyaxis, rpm, 0);
                        if (airmass < closedLoopLimit)
                        {
                            vecorr = 1;
                        }
                        // if in closed loop correction factor should be ignored

                        egtvalue *= vecorr;
                    }
                }
                else
                {
                    if (fuelVEMap != null)
                    {
                        double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                        vecorr /= 100;
                        vecorr = 1 / vecorr;
                        int[] nullvalue = new int[1];
                        nullvalue.SetValue(0, 0);
                        double closedLoopLimit = GetInterpolatedTableValue(open_loop, nullvalue, open_loopyaxis, rpm, 0);
                        if (airmass < closedLoopLimit)
                        {
                            vecorr = 1;
                        }
                        // if in closed loop correction factor should be ignored

                        egtvalue *= vecorr;
                    }
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

            if (isFuelE85.Checked)
            {
                if (E85VEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                    vecorr /= 100;
                    vecorr = 1 / vecorr;
                    vecorr *= 100; // range correction
                    if (injDC > 100)
                    {
                        vecorr *= injDC;
                        vecorr /= 100;
                    }
                    retval = Convert.ToInt32(vecorr);
                }
            }
            else
            {
                if (fuelVEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                    vecorr /= 100;
                    vecorr = 1 / vecorr;
                    vecorr *= 100; // range correction
                    if (injDC > 100)
                    {
                        vecorr *= injDC;
                        vecorr /= 100;
                    }
                    retval = Convert.ToInt32(vecorr);
                }
            }
            return retval;
        }

        private int CalculateFuelFlow(int airmass, int rpm, int lambda)
        {
            int retval = 0;
            // First we calculate how much fuel needs to be injected. Milligram is converted to gram.
            double fuelToInjectPerCycle; // mg/c *1000 = g/c
            if (isFuelE85.Checked)
                // running E85, different target lambda
                fuelToInjectPerCycle = (double)airmass / (9.84F * 1000); // mg/c *1000 = g/c
            else
                fuelToInjectPerCycle = (double)airmass / (14.65F * 1000);

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
					PartNumberConverter pnc = new PartNumberConverter();
	                //
    	            T7FileHeader header = new T7FileHeader();
    	            header.init(m_currentfilename, false);
    	            ECUInformation ecuinfo = pnc.GetECUInfo(header.getPartNumber().Trim(), "");
                    if (ecuinfo.Is2point3liter) ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter23;
                    else ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter2;
                    if (ecuinfo.Isaero) ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.TD04);
                    else ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.GT17);
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
		        if (IsBinaryConvertable(symbols))
		        {
		            isCarConvertible.Enabled = true;
		        }
		        else
		        {
		            isCarConvertible.Checked = false;
		            isCarConvertible.Enabled = false;
		        }
		        if (IsOverboostEnabled(filename, symbols))
		        {
		            isOverboostActive.Enabled = true;
		        }
		        else
		        {
		            isOverboostActive.Enabled = false;
		        }

		        bool e85automatic = IsBinaryBiopowerAuto(symbols);

                bool torqueLimitEnabled = HasBinaryTorqueLimiterEnabled(symbols, filename);

                bool gear1stLimitAvailable = Has1stGearLimit(symbols);
			
				xdummy.SetValue(0, 0);
		        int[] pedalrequestmap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(symbols, "PedalMapCal.m_RequestMap"));
		        limitResult = new AirmassLimitType[pedalrequestmap.Length];
		        int[] resulttable = new int[pedalrequestmap.Length]; // result 
		        pedal_Rows = GetSymbolLength(symbols, "PedalMapCal.n_EngineMap") / 2;
		        pedal_Columns = GetSymbolLength(symbols, "PedalMapCal.X_PedalMap") / 2;
		        pedal_Xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.n_EngineMap"), GetSymbolLength(symbols, "PedalMapCal.n_EngineMap"));
		        pedal_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.X_PedalMap"), GetSymbolLength(symbols, "PedalMapCal.X_PedalMap"));

                airTorqueMap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(symbols, "TorqueCal.m_AirTorqMap"));
                airTorqueMap_Xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(symbols, "TorqueCal.M_EngXSP"));
                airTorqueMap_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(symbols, "TorqueCal.n_EngYSP"));

                bstknkMaxAirmassMap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmass"));
                bstknkMaxAirmassMap_Xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.OffsetXSP"), GetSymbolLength(symbols, "BstKnkCal.OffsetXSP"));
                for (int a = 0; a < bstknkMaxAirmassMap_Xaxis.Length; a++)
                {
                    int val = (int)bstknkMaxAirmassMap_Xaxis.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    bstknkMaxAirmassMap_Xaxis.SetValue(val, a);
                }
                bstknkMaxAirmassMap_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.n_EngYSP"), GetSymbolLength(symbols, "BstKnkCal.n_EngYSP"));

                if (SymbolExists("BstKnkCal.MaxAirmassAu", symbols))
                {
                    bstknkMaxAirmassAuMap = readIntdatafromfile(m_currentfilename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmassAu"));
                }

                turbospeed = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.TurboSpeedTab"), GetSymbolLength(symbols, "LimEngCal.TurboSpeedTab"));
                turbospeed_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.p_AirSP"), GetSymbolLength(symbols, "LimEngCal.p_AirSP"));
                turbospeed2 = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.TurboSpeedTab2"), GetSymbolLength(symbols, "LimEngCal.TurboSpeedTab2"));
                turbospeed2_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.n_EngSP"), GetSymbolLength(symbols, "LimEngCal.n_EngSP"));

                nominalTorqueMap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(symbols, "TorqueCal.M_NominalMap"));
                nominalTorqueMap_Xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(symbols, "TorqueCal.m_AirXSP"));
                nominalTorqueMap_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(symbols, "TorqueCal.n_EngYSP"));
                for (int a = 0; a < nominalTorqueMap.Length; a++)
                {
                    int val = (int)nominalTorqueMap.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominalTorqueMap.SetValue(val, a);
                }

                fuelcutAirInletLimit = Convert.ToInt32(readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(symbols, "FCutCal.m_AirInletLimit")).GetValue(0));

                enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxTab"));
                enginetorquelimE85Auto = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxE85TabAut"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxE85TabAut"));
                enginetorquelimAuto = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxAutTab"));
                enginetorquelimE85 = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxE85Tab"));
                enginetorque_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(symbols, "TorqueCal.n_EngYSP"));
                enginetorquelimConvertible = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_CabGearLim"), GetSymbolLength(symbols, "TorqueCal.M_CabGearLim"));
                enginetorquelimGear = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(symbols, "TorqueCal.M_ManGearLim"));
                enginetorquelim5th = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(symbols, "TorqueCal.M_5GearLimTab"));
                enginetorquelim5th_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_Eng5GearSP"), GetSymbolLength(symbols, "TorqueCal.n_Eng5GearSP"));
                enginetorquelimReverse = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_ReverseTab"), GetSymbolLength(symbols, "TorqueCal.M_ReverseTab"));
                enginetorquelimReverse_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngSP"), GetSymbolLength(symbols, "TorqueCal.n_EngSP"));
                enginetorquelim1st = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_1GearTab"), GetSymbolLength(symbols, "TorqueCal.M_1GearTab"));
                enginetorquelim1st_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_Eng1GearSP"), GetSymbolLength(symbols, "TorqueCal.n_Eng1GearSP"));
                enginetorquelimOverboost = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_OverBoostTab"), GetSymbolLength(symbols, "TorqueCal.M_OverBoostTab"));

                try
                {
                    int[] injConstant = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "InjCorrCal.InjectorConst"), GetSymbolLength(symbols, "InjCorrCal.InjectorConst"));
                    injectorConstant = Convert.ToInt32(injConstant.GetValue(0));
                    injectorBatteryCorrection = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "InjCorrCal.BattCorrTab"), GetSymbolLength(symbols, "InjCorrCal.BattCorrTab"));
                    injectorBatteryCorrection_axis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "InjCorrCal.BattCorrSP"), GetSymbolLength(symbols, "InjCorrCal.BattCorrSP"));
                    fuelVEMap = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.Map"), GetSymbolLength(symbols, "BFuelCal.Map"));
                    E85VEMap = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.E85Map"), GetSymbolLength(symbols, "BFuelCal.E85Map"));
                    fuelVE_Xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.AirXSP"), GetSymbolLength(symbols, "BFuelCal.AirXSP"));
                    fuelVE_Yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.RpmYSP"), GetSymbolLength(symbols, "BFuelCal.RpmYSP"));
                    open_loop = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.MaxLoadNormTab"), GetSymbolLength(symbols, "LambdaCal.MaxLoadNormTab"));
                    if (isFuelE85.Checked)
                    {
                        open_loop = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.MaxLoadE85Tab"), GetSymbolLength(symbols, "LambdaCal.MaxLoadE85Tab"));//
                    }
                    open_loopyaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.RpmSp"), GetSymbolLength(symbols, "LambdaCal.RpmSp"));

                    if (SymbolExists("ExhaustCal.T_Lambda1Map", symbols))
                    {
                        EGTMap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "ExhaustCal.T_Lambda1Map"), GetSymbolLength(symbols, "ExhaustCal.T_Lambda1Map"));
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
                        int pedalpos = ((int)pedal_Yaxis.GetValue(colcount) / 10);
                        int airmassrequestforcell = (int)pedalrequestmap.GetValue((colcount * pedal_Rows) + rowcount);
                        logger.Debug(String.Format("Current request = {0} mg/c", airmassrequestforcell));
                        AirmassLimitType limiterType = AirmassLimitType.None;

                        int resultingAirMass = CalculateMaxAirmassforcell(rpm, airmassrequestforcell, isCarAutomatic.Checked, isFuelE85.Checked, isCarConvertible.Checked, isOverboostActive.Checked, e85automatic, torqueLimitEnabled, gear1stLimitAvailable, out limiterType);
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
            if (powerSeries >= 0) chartControl1.Series[powerSeries].Visible = checkEdit8.Checked;
            if (powerCompareSeries >= 0) chartControl1.Series[powerCompareSeries].Visible = checkEdit8.Checked;
            if (torqueSeries >= 0) chartControl1.Series[torqueSeries].Visible = checkEdit9.Checked;
            if (torqueCompareSeries >= 0) chartControl1.Series[torqueCompareSeries].Visible = checkEdit9.Checked;
            if (injectorDCSeries >= 0) chartControl1.Series[injectorDCSeries].Visible = checkEdit10.Checked;
            if (lambdaSeries >= 0) chartControl1.Series[lambdaSeries].Visible = checkEdit11.Checked;
            if (EGTSeries >= 0) chartControl1.Series[EGTSeries].Visible = checkEdit12.Checked;
            if (FlowSeries >= 0) chartControl1.Series[FlowSeries].Visible = showFuelFlow.Checked;
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
                if (isFuelE85.Checked)
                {
                    double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                    vecorr /= 100;
                    baseFuelPulseWidth *= (double)vecorr;
                }
                else
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVE_Xaxis, fuelVE_Yaxis, rpm, airmass);
                    vecorr /= 100;
                    baseFuelPulseWidth *= (double)vecorr;
                }

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
                string softwareversion = string.Empty;

                try
                {
                    T7FileHeader t7InfoHeader = new T7FileHeader();
                    if (t7InfoHeader.init(m_current_comparefilename, false))
                    {
                        softwareversion = t7InfoHeader.getSoftwareVersion();
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E);
                }
                Trionic7File t7file = new Trionic7File();
                
                Compare_symbol_collection = t7file.ExtractFile(m_current_comparefilename, 44, softwareversion);
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

        private void isCarConvertible_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void isOverboostActive_CheckedChanged(object sender, EventArgs e)
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
            // refresh
            //Calculate();
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
            // refresh
            //Calculate();
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

        private void labelControl8_DoubleClick(object sender, EventArgs e)
        {
            // start turbospeed limiter viewer!
            CastStartViewerEvent("LimEngCal.TurboSpeedTab");
            CastStartViewerEvent("LimEngCal.TurboSpeedTab2");
        }

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            // start airmass limiter viewer
            // if aut
            if (isCarAutomatic.Checked)
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmassAu");
            }
            else
            {
                CastStartViewerEvent("BstKnkCal.MaxAirmass");
            }
        }

        private void labelControl2_DoubleClick(object sender, EventArgs e)
        {
            // start E85 torque limiter
            CastStartViewerEvent("TorqueCal.M_EngMaxE85Tab");
        }

        private void labelControl18_Click(object sender, EventArgs e)
        {
            // start E85Auto torque limiter
            CastStartViewerEvent("TorqueCal.M_EngMaxE85TabAut");
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            // start engine torque limiter
            if (isCarAutomatic.Checked)
            {
                CastStartViewerEvent("TorqueCal.M_EngMaxAutTab");
            }
            else
            {
                CastStartViewerEvent("TorqueCal.M_EngMaxTab");
            }
        }

        private void labelControl12_DoubleClick(object sender, EventArgs e)
        {
            // gear torque limiters 
            /*
            TorqueCal.M_1GearTab		TorqueCal.n_Eng1GearSP 	(ONLY AUT)
            TorqueCal.M_ReverseTab		TorqueCal.n_EngSP	(ONLY AUT)
            TorqueCal.M_5GearTab		TorqueCal.n_Eng5GearSP
            TorqueCal.M_CabGearLim		TorqueCal.n_Eng5GearSP
             * */
            if (isCarAutomatic.Checked)
            {
                // automatic gearbox
                // CastStartViewerEvent("TorqueCal.M_EngMaxAutTab");??
                if (comboBoxEdit1.SelectedIndex == 0)
                {
                    // reverse
                    CastStartViewerEvent("TorqueCal.M_ReverseTab");
                }
                else if (comboBoxEdit1.SelectedIndex == 1)
                {
                    CastStartViewerEvent("TorqueCal.M_1GearTab");
                }

            }
            else if (isCarConvertible.Checked)
            {
                // cabrio
                CastStartViewerEvent("TorqueCal.M_CabGearLim");
                if (comboBoxEdit1.SelectedIndex == 5)
                {
                    CastStartViewerEvent("TorqueCal.M_5GearLimTab");
                }
            }
            else
            {
                CastStartViewerEvent("TorqueCal.M_ManGearLim");
                if (comboBoxEdit1.SelectedIndex == 5)
                {
                    CastStartViewerEvent("TorqueCal.M_5GearLimTab");
                }

            }
        }

        private void labelControl14_DoubleClick(object sender, EventArgs e)
        {
            CastStartViewerEvent("FCutCal.m_AirInletLimit");
        }

        private void labelControl16_DoubleClick(object sender, EventArgs e)
        {
            // show overboost tab
            CastStartViewerEvent("TorqueCal.M_OverBoostTab");
        }

        private void labelControl12_Click(object sender, EventArgs e)
        {
            if (!isCarAutomatic.Checked)
            {
                if (isCarConvertible.Checked)
                {
                    CastStartViewerEvent("TorqueCal.M_CabGearLim");
                }

                if (comboBoxEdit1.SelectedIndex == 5)
                {
                    CastStartViewerEvent("TorqueCal.M_5GearLimTab");
                }
            }
            else
            {
                // automatic!
                if (comboBoxEdit1.SelectedIndex == 0)
                {
                    CastStartViewerEvent("TorqueCal.M_ReverseTab");
                }

                if (comboBoxEdit1.SelectedIndex == 1)
                {
                    if(SymbolExists("TorqueCal.M_1GearTab",m_symbols))
                    {
                        CastStartViewerEvent("TorqueCal.M_1GearTab");
                    }
                }
            }
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
                        else if (curLimit == AirmassLimitType.TurboSpeedLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Black, pnts, System.Drawing.Drawing2D.FillMode.Winding);
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
                        else if (curLimit == AirmassLimitType.TorqueLimiterEngineE85Auto)
                        {
                            e.Graphics.FillPolygon(Brushes.White, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        if (cbTableSelectionEdit.SelectedIndex == 1)
                        {
                            // convert airmass to torque
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int torque;
                            if (displayTorqueInLBFT.Checked)
                                torque = TorqueToTorqueLbft(AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, useTrionicCalculationForTorque.Checked));
                            else
                                torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, useTrionicCalculationForTorque.Checked);
                            e.DisplayText = torque.ToString();
                        }
                        else if (cbTableSelectionEdit.SelectedIndex == 2)
                        {
                            //convert airmass to horsepower
                            int rpm = Convert.ToInt32(pedal_Xaxis.GetValue(e.Column.AbsoluteIndex));
                            int torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, useTrionicCalculationForTorque.Checked);
                            int horsepower;
                            if (displayPowerInkW.Checked)
                                horsepower = TorqueToPowerkW(torque, rpm);
                            else
                                horsepower = TorqueToPower(torque, rpm);
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
            // save graph as image
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
            // print graph
            // set paper to landscape
            chartControl1.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;
            //DevExpress.XtraPrinting.PrintingSystem ps = new DevExpress.XtraPrinting.PrintingSystem();
            //ps.PageSettings.Landscape = true;

            chartControl1.ShowPrintPreview();
        }
    }
}
