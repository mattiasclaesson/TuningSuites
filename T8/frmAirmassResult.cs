using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using T8SuitePro;
using System.IO;
using DevExpress.XtraCharts;


namespace T8SuitePro
{
    public enum limitType : int
    {
        None,
        TorqueLimiterEngine,
        TorqueLimiterEngineE85,
        TorqueLimiterGear,
        AirmassLimiter,
        TurboSpeedLimiter,
        FuelCutLimiter,
        OverBoostLimiter
    }

    public partial class frmAirmassResult : DevExpress.XtraEditors.XtraForm
    {
        int rows;
        int columns;
        private int[] y_axisvalues;
        private int[] x_axisvalues;
        private int[] limitermap;
        private string m_currentfile = string.Empty;
        private int m_MaxValueInTable;

        int m_injectorConstant;
        byte[] fuelVEMap;
        int[] EGTMap;
        int[] fuelVExaxis;
        int[] fuelVEyaxis;


        public string Currentfile
        {
            get { return m_currentfile; }
            set { m_currentfile = value; }
        }

        private SymbolCollection m_symbols;

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }

        public delegate void StartTableViewer(object sender, StartTableViewerEventArgs e);
        public event frmAirmassResult.StartTableViewer onStartTableViewer;

        public frmAirmassResult()
        {
            InitializeComponent();
        }

        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Userdescription == symbolname || sh.Varname == symbolname)
                {
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }
        private int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Userdescription == symbolname || sh.Varname == symbolname)
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
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private int[] readIntdatafromfile(string filename, int address, int length)
        {
            int[] retval = new int[length/2];
            try
            {
                int j = 0;
                for (int i = 0; i < length; i+=2)
                {
                    byte b1 = entirefile[address + i];
                    byte b2 = entirefile[address + i + 1];
                    int value = Convert.ToInt32(b1) * 256 + Convert.ToInt32(b2);
                    retval.SetValue(value, j++);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private bool IsOverboostEnabled()
        {
            bool retval = false;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "TrqLimCal.EnableOverBoost" || sh.Userdescription == "TrqLimCal.EnableOverBoost")
                {
                    // read data from file and verify if the value is > 0
                    byte[] overboostenable = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.EnableOverBoost"), (int)GetSymbolLength(m_symbols, "TrqLimCal.EnableOverBoost"));
                    foreach (byte b in overboostenable)
                    {
                        if (b != 0x00) retval = true;
                    }
                }
            }
            return retval;

        }

        private bool IsBinaryBiopower()
        {
           foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "FFTrqCal.FFTrq_MaxEngineTab1" || sh.Userdescription == "FFTrqCal.FFTrq_MaxEngineTab1" ||
                    sh.Varname == "FFTrqCal.FFTrq_MaxEngineTab2" || sh.Userdescription == "FFTrqCal.FFTrq_MaxEngineTab2")
                {
                    return true;
                }
            }
            return false;
        }

        private int TorqueToAirmass(int torque, int rpm, bool E85)
        {
            // should be done through the torque->airmass map
            if (torque > 32000) torque = -(65535 - torque);

            double airmass = GetInterpolatedTableValue(airtorquemap, xairtorque, yaxis, rpm, torque);
            return Convert.ToInt32(airmass);
        }

        private int peakAirmass = 0;

        public int PeakAirmass
        {
            get { return peakAirmass; }
            set { peakAirmass = value; }
        }

        int[] airtorquemap;
        int[] xairtorque;
        int[] yaxis;
        int[] bstknk;
        int[] bstknkau;
        int[] turbospeed;
        int[] nominaltorque;
        int[] nominaltorquexaxis;
        int[] nominaltorqueyaxis;
        byte[] entirefile;
        int[] fuelcutlimit;
        int[] ffmaxairmass;

        public void Calculate()
        {
            // do the math!
            entirefile = File.ReadAllBytes(m_currentfile);
            try
            {
                if (IsBinaryBiopower())
                {
                    checkEdit2.Enabled = true;
                }
                else
                {
                    checkEdit2.Checked = false;
                    checkEdit2.Enabled = false;
                }
                if (IsOverboostEnabled())
                {
                    checkEdit4.Enabled = true;
                }
                else
                {
                    checkEdit4.Enabled = false;
                }
                int[] pedalrequestmap = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.Trq_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.Trq_RequestMap"));
                limitermap = new int[pedalrequestmap.Length];
                int[] resulttable = new int[pedalrequestmap.Length]; // result 
                rows = GetSymbolLength(m_symbols, "PedalMapCal.n_EngineMap") / 2;
                columns = GetSymbolLength(m_symbols, "PedalMapCal.X_PedalMap") / 2;
                int[] pedalXAxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.n_EngineMap"), GetSymbolLength(m_symbols, "PedalMapCal.n_EngineMap"));
                int[] pedalYAxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.X_PedalMap"), GetSymbolLength(m_symbols, "PedalMapCal.X_PedalMap"));
                y_axisvalues = pedalYAxis;
                x_axisvalues = pedalXAxis;

                airtorquemap = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirTorqMap"));
                xairtorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_EngXSP"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_EngXSP"));
                yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.n_EngineYSP"), GetSymbolLength(m_symbols, "TrqMastCal.n_EngineYSP"));
                bstknk = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"));
                if (SymbolExists("BstKnkCal.MaxAirmassAu"))
                {
                    bstknkau = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"));
                }
                else
                {
                    bstknkau = bstknk;
                }
                turbospeed = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "LimEngCal.TurboSpeedTab"), GetSymbolLength(m_symbols, "LimEngCal.TurboSpeedTab"));
                nominaltorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"));
                nominaltorquexaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"));
                nominaltorqueyaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.n_EngineYSP"), GetSymbolLength(m_symbols, "TrqMastCal.n_EngineYSP"));
                fuelcutlimit = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"));
                
                if(SymbolExists("FFAirCal.m_maxAirmass"))
                {
                    ffmaxairmass = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FFAirCal.m_maxAirmass"), GetSymbolLength(m_symbols, "FFAirCal.m_maxAirmass"));
                }

                try
                {
                    int[] injConstant = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "InjCorrCal.InjectorConst"), GetSymbolLength(m_symbols, "InjCorrCal.InjectorConst"));
                    m_injectorConstant = Convert.ToInt32(injConstant.GetValue(0));
                    if (checkEdit2.Checked)
                    {
                        fuelVEMap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FFFuelCal.TempEnrichFacMAP"), GetSymbolLength(m_symbols, "FFFuelCal.TempEnrichFacMAP"));
                    }
                    else
                    {
                        fuelVEMap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BFuelCal.TempEnrichFacMap"), GetSymbolLength(m_symbols, "BFuelCal.TempEnrichFacMap"));
                    }
                    fuelVExaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BFuelCal.AirXSP"), GetSymbolLength(m_symbols, "BFuelCal.AirXSP"));
                    fuelVEyaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BFuelCal.RpmYSP"), GetSymbolLength(m_symbols, "BFuelCal.RpmYSP"));

                    if (SymbolExists("ExhaustCal.T_Lambda1Map"))
                    {
                        EGTMap = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "ExhaustCal.T_Lambda1Map"), GetSymbolLength(m_symbols, "ExhaustCal.T_Lambda1Map"));
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }

                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominaltorque.SetValue(val, a);
                }

                // also get the axis for pedalrequestmap
                // x = PedalMapCal.n_EngineMap (1x) rpm
                // y = PedalMapCal.X_PedalMap (10x) pedalposition (%)
                for (int colcount = 0; colcount < columns; colcount++)
                {
                    for (int rowcount = 0; rowcount < rows; rowcount++)
                    {
                        // get the current value from the request map
                        int rpm = (int)pedalXAxis.GetValue(rowcount);
                        int requestedtorque = (int)pedalrequestmap.GetValue((colcount * rows) + rowcount);
                        int airmassrequestforcell = TorqueToAirmass(requestedtorque, rpm, false);
                        
                        limitType limiterType = limitType.None;
                        int resultingAirMass = CalculateMaxAirmassforcell(/*pedalpos*/((int)pedalYAxis.GetValue(colcount) / 10), /* rpm */(int)pedalXAxis.GetValue(rowcount), airmassrequestforcell, checkEdit1.Checked, checkEdit2.Checked, checkEdit4.Checked, out limiterType);
                        if (peakAirmass < resultingAirMass) peakAirmass = resultingAirMass;
                        resulttable.SetValue(resultingAirMass, (colcount * rows) + rowcount);
                        limitermap.SetValue(limiterType, (colcount * rows) + rowcount);
                    }
                }
                // now show resulttable
                DataTable dt = new DataTable();
                foreach (int xvalue in pedalXAxis)
                {
                    dt.Columns.Add(xvalue.ToString());
                }
                // now fill the table rows
                m_MaxValueInTable = 0;
                for (int r = pedalYAxis.Length - 1; r >= 0; r--)
                {
                    object[] values = new object[columns];
                    for (int t = 0; t < pedalXAxis.Length; t++)
                    {
                        int currValue = (int)resulttable.GetValue((r * columns) + t);
                        if (currValue > m_MaxValueInTable) m_MaxValueInTable = currValue;
                        values.SetValue(resulttable.GetValue((r * columns) + t), t);
                    }
                    dt.Rows.Add(values);
                }
                gridControl1.DataSource = dt;
                if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
                {
                    LoadGraphWithDetails();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to calculate for file : " + m_currentfile);
            }

        }

        private int CalculateMaxAirmassforcell(int pedalposition, int rpm, int requestairmass, bool autogearbox, bool E85, bool OverboostEnabled, out limitType limiterType)
        {
            // calculate the restricted airmass for the current point
            int retval = requestairmass;
            limiterType = limitType.None;

            Console.WriteLine("Pedalpos: " + pedalposition.ToString() + " Rpm: " + rpm.ToString() + " requests: " + requestairmass.ToString() + " mg/c");
            
            // first check against torquelimiters
            limitType TrqLimiterType = limitType.None;
            retval = CheckAgainstTorqueLimiters(rpm, requestairmass, E85, autogearbox, OverboostEnabled, out TrqLimiterType);
            if (retval < requestairmass)
            {
                limiterType = TrqLimiterType;
            }
            
            // secondly check against airmasslimiters
            limitType AirmassLimiterType = limitType.None;
            int TorqueLimitedAirmass = retval;
            retval = CheckAgainstAirmassLimiters(rpm, retval, autogearbox, E85, ref AirmassLimiterType);
            retval = CheckAgainstFuelcutLimiter(retval, ref AirmassLimiterType);
            if (retval < TorqueLimitedAirmass)
            {
                limiterType = AirmassLimiterType;
            }

            return retval;
        }

        private bool SymbolExists(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname) return true;
            }
            return false;
        }

        private int CheckAgainstTorqueLimiters(int rpm, int requestedairmass, bool E85, bool Automatic, bool OverboostEnabled, out limitType TrqLimiter)
        {
            // first convert airmass torque to torque using TorqueCal.M_NominalMap
            // axis are 
            // x = TorqueCal.m_AirXSP (airmass)
            // y = TorqueCal.n_EngYSP (rpm)
            TrqLimiter = limitType.None;
            int LimitedAirMass = requestedairmass;
            int torque = Convert.ToInt32(GetInterpolatedTableValue(nominaltorque, nominaltorquexaxis, nominaltorqueyaxis, rpm, requestedairmass));

            int[] enginetorquelim;
            if (E85)
            {
                enginetorquelim = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FFTrqCal.FFTrq_MaxEngineTab1"), GetSymbolLength(m_symbols, "FFTrqCal.FFTrq_MaxEngineTab1"));
                // TODO: Add support for FFTrqCal.FFTrq_MaxEngineTab2 // 150 hp
            }
            else
            {
                // Select old style TrqLimCal.Trq_MaxEngineManTab1 or TrqLimCal.Trq_MaxEngineAutTab1
                string engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineManTab1";
                if (Automatic)
                {
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineAutTab1";
                }
                if (!SymbolExists(engineTorqueLimiter))
                {
                    // If it does not exist, default to the newer style TrqLimCal.Trq_MaxEngineTab1
                    engineTorqueLimiter = "TrqLimCal.Trq_MaxEngineTab1";
                    // TODO: Add support for TrqLimCal.Trq_MaxEngineTab2"; // 150 hp
                }
                enginetorquelim = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, engineTorqueLimiter), GetSymbolLength(m_symbols, engineTorqueLimiter));
            }
            if (OverboostEnabled)
            {
                enginetorquelim = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_OverBoostTab"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_OverBoostTab"));
            }

            int[] xdummy = new int[1];
            xdummy.SetValue(0, 0);
            int torquelimit1 = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
            if (torque > torquelimit1)
            {
                //Console.WriteLine("Torque is limited from " + torque.ToString() + " to " + torquelimit1.ToString() + " at " + rpm.ToString() + " rpm");
                torque = torquelimit1;
                if (E85)
                {
                    TrqLimiter = limitType.TorqueLimiterEngineE85;
                }
                else if (OverboostEnabled)
                {
                    TrqLimiter = limitType.OverBoostLimiter;
                }
                else
                {
                    TrqLimiter = limitType.TorqueLimiterEngine;
                }
            }

            // Trq_ManGear = only manual gearbox
            if (!checkEdit1.Checked)
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

                enginetorquelim = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_ManGear"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_ManGear"));
                //
                int torquelimitManual = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, gears, comboBoxEdit1.SelectedIndex, 0));
                if (torque > torquelimitManual)
                {
                    torque = torquelimitManual;
                    TrqLimiter = limitType.TorqueLimiterGear;
                    Console.WriteLine("Manual gear torque limit hit");
                }
            }
            else
            { 
                // Newer style automatic torque limits:
                // TMCCal.Trq_MaxEngineTab 170 hp because we use petrol: TrqLimCal.Trq_MaxEngineTab1 and E85: FFTrqCal.FFTrq_MaxEngineTab1
                // TODO: Add support for TMCCal.Trq_MaxEngineLowTab 150 hp

                enginetorquelim = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TMCCal.Trq_MaxEngineTab"), GetSymbolLength(m_symbols, "TMCCal.Trq_MaxEngineTab"));
                int torquelimitAutomatic = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
                if (torque > torquelimitAutomatic)
                {
                    torque = torquelimitAutomatic;
                    TrqLimiter = limitType.TorqueLimiterGear;
                    Console.WriteLine("Automatic gear torque limit hit");
                }
            }

            // else ???
            if (TrqLimiter != limitType.None)
            {
                LimitedAirMass = Convert.ToInt32(GetInterpolatedTableValue(airtorquemap, xairtorque, yaxis, rpm, torque));
            }

            return LimitedAirMass;
        }

        private int CheckAgainstFuelcutLimiter(int requestedairmass, ref limitType AirmassLimiter)
        {
            int retval = requestedairmass;
            if ((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                if (fuelcutlimit.Length > 0)
                {
                    if (Convert.ToInt32(fuelcutlimit.GetValue(0)) < requestedairmass)
                    {
                        retval = Convert.ToInt32(fuelcutlimit.GetValue(0));
                        AirmassLimiter = limitType.FuelCutLimiter;
                    }
                }
            }
            return retval;
            
        }

        private int CheckAgainstAirmassLimiters(int rpm, int requestedairmass, bool autogearbox, bool E85, ref limitType AirmassLimiter)
        {
            //AirmassLimiter = limitType.None;
            // check against BstKnkCal.MaxAirmass
            string xaxisstr = "BstKnkCal.OffsetXSP";
            if (!SymbolExists(xaxisstr)) xaxisstr = "BstKnkCal.fi_offsetXSP";
            // only the right-most column (no knock)

            // BstKnkCal.fi_offsetXSP in case of flexfuel!
            int[] xaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, xaxisstr), GetSymbolLength(m_symbols, xaxisstr));
            for (int a = 0; a < xaxis.Length; a++)
            {
                int val = (int)xaxis.GetValue(a);
                if (val > 32000) val = -(65536 - val);
                xaxis.SetValue(val, a);
            }
            int[] yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.n_EngYSP"), GetSymbolLength(m_symbols, "BstKnkCal.n_EngYSP"));
            int airmasslimit = requestedairmass;

            // E85 flexfuel air limit
            int[] xaxisFFAir = new int[1]; // TODO: Move all axis out in Calculate
            if (E85)
            {
                xaxisFFAir = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FFAirCal.fi_offsetXSP"), GetSymbolLength(m_symbols, "FFAirCal.fi_offsetXSP"));
                for (int a = 0; a < xaxisFFAir.Length; a++)
                {
                    int val = (int)xaxisFFAir.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    xaxisFFAir.SetValue(val, a);
                }      
            }
            
            if (autogearbox) airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknkau, xaxis, yaxis, rpm, 0));
            else if (E85) airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(ffmaxairmass, xaxisFFAir, yaxis, rpm, 0)); // zero degree ignition offset
            else airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknk, xaxis, yaxis, rpm, 0));
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = limitType.AirmassLimiter;
                Console.WriteLine("Reduced airmass because of BstKnkCal.MaxAirmass/FFAirCal.m_maxAirmass: " + requestedairmass.ToString() + " rpm: " + rpm.ToString() + " E85: " + E85.ToString());
            }

            return requestedairmass;
        }


        static private double GetInterpolatedTableValue(int[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue)
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
                //AddDebugLog("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
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
                //AddDebugLog("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
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
                
                //AddDebugLog("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //AddDebugLog("result = " + result.ToString());
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to interpolate: " + E.Message);
            }
            return result;


        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
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
                        //limitermap
                        int row = rows - (e.RowHandle+1);
                        limitType curLimit = (limitType)limitermap.GetValue((row * columns) + e.Column.AbsoluteIndex);
                        Point[] pnts = new Point[4];
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 0);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width - (e.Bounds.Height / 2), e.Bounds.Y), 1);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y + (e.Bounds.Height / 2)), 2);
                        pnts.SetValue(new Point(e.Bounds.X + e.Bounds.Width, e.Bounds.Y), 3);
                        if (curLimit == limitType.AirmassLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Blue, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TorqueLimiterEngineE85)
                        {
                            e.Graphics.FillPolygon(Brushes.Purple, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TorqueLimiterEngine)
                        {
                            e.Graphics.FillPolygon(Brushes.Yellow, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TurboSpeedLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.Black, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.TorqueLimiterGear)
                        {
                            e.Graphics.FillPolygon(Brushes.SaddleBrown, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.FuelCutLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.DarkGray, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        else if (curLimit == limitType.OverBoostLimiter)
                        {
                            e.Graphics.FillPolygon(Brushes.CornflowerBlue, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        if (comboBoxEdit2.SelectedIndex == 1)
                        {
                            // convert airmass to torque
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int torque;
                            if (!checkEdit6.Checked)
                            {
                                torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, checkEdit7.Checked);
                            }
                            else
                            {
                                torque = AirmassToTorqueLbft(Convert.ToInt32(e.CellValue), rpm, checkEdit7.Checked);
                            }
                            e.DisplayText = torque.ToString();
                        }
                        else if (comboBoxEdit2.SelectedIndex == 2)
                        {
                            //convert airmass to horsepower
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, checkEdit7.Checked);
                            int horsepower;
                            if (!checkEdit5.Checked)
                            {
                                horsepower = TorqueToPower(torque, rpm);
                            }
                            else
                            {
                                horsepower = TorqueToPowerkW(torque, rpm);
                            }
                            e.DisplayText = horsepower.ToString();
                        }
                        else if (comboBoxEdit2.SelectedIndex == 3) //injector DC
                        {
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int injDC = CalculateInjectorDC(Convert.ToInt32(e.CellValue), rpm);
                            e.DisplayText = injDC.ToString();
                        }
                        else if (comboBoxEdit2.SelectedIndex == 4) //target lambda
                        {
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int targetLambda = CalculateTargetLambda(Convert.ToInt32(e.CellValue), rpm);
                            float dtarget = (float)targetLambda;
                            dtarget /= 100;
                            e.DisplayText = dtarget.ToString("F2");

                        }
                        else if (comboBoxEdit2.SelectedIndex == 5) //target AFR
                        {
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int targetLambda = CalculateTargetLambda(Convert.ToInt32(e.CellValue), rpm);
                            float dtarget = (float)targetLambda;
                            dtarget /= 100;
                            if (checkEdit2.Checked)
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
                        else if (comboBoxEdit2.SelectedIndex == 6)
                        {
                            // convert to estimated EGT
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int airmass = Convert.ToInt32(e.CellValue);
                            int egt = CalculateEstimateEGT(airmass, rpm);
                            e.DisplayText = egt.ToString();
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
                Console.WriteLine(E.Message);
            }
        }

        static private int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7024;
           
            return Convert.ToInt32(power);
        }

        private int AirmassToTorque(int airmass, int rpm)
        {
            double tq = Convert.ToDouble(airmass) / 3.1;
            if (checkEdit2.Checked)
            {
                tq *= 1.07;
            }
            double correction = 1;
            if (rpm >= 6000) correction = 0.97;
            else if (rpm > 5800) correction = 0.98;
            else if (rpm > 5400) correction = 0.985;
            else if (rpm > 5000) correction = 0.99;
            else if (rpm > 4600) correction = 0.995;
            tq *= correction;
            return Convert.ToInt32(tq);
        }


        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {

                //  e.Painter.DrawCaption(new DevExpress.Utils.Drawing.ObjectInfoArgs(new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics)), "As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, null);
                // e.Cache.DrawString("As waarde", this.Font, Brushes.MidnightBlue, e.Bounds, new StringFormat());
                try
                {
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            int value = (int)y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle) ;
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
                    Console.WriteLine(E.Message);
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridControl1.Refresh();
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void labelControl8_DoubleClick(object sender, EventArgs e)
        {
            // start turbospeed limiter
            //CastStartViewerEvent("");
        }

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            // start airmass limiter viewer
            // if automatic
            if (checkEdit1.Checked && SymbolExists("BstKnkCal.MaxAirmassAu"))
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
            CastStartViewerEvent("FFTrqCal.FFTrq_MaxEngineTab1");
            CastStartViewerEvent("FFTrqCal.FFTrq_MaxEngineTab2");
        }

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            // start engine torque limiter
            if (checkEdit1.Checked)
            {
                CastStartViewerEvent("TrqLimCal.Trq_MaxEngineAutTab1");
                CastStartViewerEvent("TrqLimCal.Trq_MaxEngineAutTab2");
            }
            else
            {
                CastStartViewerEvent("TrqLimCal.Trq_MaxEngineManTab1");
                CastStartViewerEvent("TrqLimCal.Trq_MaxEngineManTab2");
            }
        }

        private void CastStartViewerEvent(string mapname)
        {
            if (onStartTableViewer != null)
            {
                onStartTableViewer(this, new StartTableViewerEventArgs(mapname));
            }
        }
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

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void labelControl12_DoubleClick(object sender, EventArgs e)
        {
            // gear torque limiters 
            CastStartViewerEvent("TrqLimCal.Trq_ManGear"); 
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

        private void checkEdit4_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void checkEdit8_CheckedChanged(object sender, EventArgs e)
        {
            UpdateGraphVisibility();
        }

        int powerSeries = -1;
        int powerCompareSeries = -1;
        int torqueCompareSeries = -1;
        int torqueSeries = -1;
        int injectorDCSeries = -1;
        int lambdaSeries = -1;
        int EGTSeries = -1;

        private void LoadExtraGraphFromCompareBin(DataTable dt, string filename)
        {
            Console.WriteLine("Loading additional data for: " + filename);
            string powerLabel = "Power (bhp) " + Path.GetFileNameWithoutExtension(filename);
            if (checkEdit5.Checked) powerLabel = "Power (kW) " + Path.GetFileNameWithoutExtension(filename);
            string torqueLabel = "Torque (Nm) " + Path.GetFileNameWithoutExtension(filename);
            if (checkEdit6.Checked) torqueLabel = "Torque (lbft) " + Path.GetFileNameWithoutExtension(filename);
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
                int rpm = Convert.ToInt32(x_axisvalues.GetValue(i));
                int torque = AirmassToTorque(Convert.ToInt32(o), rpm, checkEdit7.Checked);
                int horsepower = TorqueToPower(torque, rpm);
                if (checkEdit5.Checked) horsepower = TorqueToPowerkW(torque, rpm);
                if (checkEdit6.Checked) torque = AirmassToTorqueLbft(Convert.ToInt32(o), rpm, checkEdit7.Checked);

                double[] dvals = new double[1];
                dvals.SetValue(Convert.ToDouble(horsepower), 0);
                chartControl1.Series[powerCompareSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvals));

                double[] dvalstorq = new double[1];
                dvalstorq.SetValue(Convert.ToDouble(torque), 0);
                chartControl1.Series[torqueCompareSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalstorq));


            }
        }

        private int TorqueToPowerkW(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            // convert to kW in stead of horsepower
            power *= 0.73549875;
            return Convert.ToInt32(power);
        }


        private int AirmassToTorqueLbft(int airmass, int rpm, bool TrionicStyle)
        {
            double tq = Convert.ToDouble(airmass) / 3.1;
            if (TrionicStyle)
            {
                int[] nominaltorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"));
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominaltorque.SetValue(val, a);
                }
                int[] xaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"));
                int[] yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.n_EngineYSP"), GetSymbolLength(m_symbols, "TrqMastCal.n_EngineYSP"));
                tq = GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, airmass);
                if (checkEdit2.Checked)
                {
                    tq *= 1.07;
                }
                tq /= 1.3558; //<GS-22032010> bugfix
            }
            else
            {
                if (checkEdit2.Checked)
                {
                    tq *= 1.07;
                }
                double correction = GetCorrectionFactorForRpm(rpm);
                tq *= correction;
                tq /= 1.3558; //<GS-22032010> bugfix
            }
            return Convert.ToInt32(tq);
        }

        private double GetCorrectionFactorForRpm(int rpm)
        {
            double correction = 1;
            /*if (rpm >= 6000) correction = 0.97;
            else if (rpm > 5800) correction = 0.98;
            else if (rpm > 5400) correction = 0.985;
            else if (rpm > 5000) correction = 0.99;
            else if (rpm > 4600) correction = 0.995;*/
            if (rpm >= 6000) correction = 0.85;
            else if (rpm >= 5820) correction = 0.94;
            else if (rpm >= 5440) correction = 0.95;
            else if (rpm >= 5060) correction = 0.99;
            else if (rpm >= 4680) correction = 1.00;//1.03;
            else if (rpm >= 4300) correction = 1.00;//1.05;
            else if (rpm >= 3920) correction = 1.00;//1.06;
            else if (rpm >= 3540) correction = 1.00;//1.06;
            else if (rpm >= 3160) correction = 1.00;//1.07;
            else if (rpm >= 2780) correction = 1.00;//1.07;
            else if (rpm >= 2400) correction = 1.00;//1.07;
            else if (rpm >= 2020) correction = 1.00;//1.06;
            else if (rpm >= 1640) correction = 1.00;
            else if (rpm >= 1260) correction = 1.00;
            else correction = 1.00;
            return correction;

        }


        private int AirmassToTorque(int airmass, int rpm, bool TrionicStyle)
        {
            double tq = Convert.ToDouble(airmass) / 3.1;
            if (TrionicStyle)
            {
                int[] nominaltorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"));
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    val /= 10; // in tenth of Nms
                    nominaltorque.SetValue(val, a);
                }
                int[] xaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"));
                int[] yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.n_EngineYSP"), GetSymbolLength(m_symbols, "TrqMastCal.n_EngineYSP"));
                tq = GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, airmass);
            }
            else
            {
                if (checkEdit2.Checked)
                {
                    tq *= 1.07;
                }
                double correction = GetCorrectionFactorForRpm(rpm);
                tq *= correction;
            }
            return Convert.ToInt32(tq);
        }


        private void LoadGraphWithDetails()
        {
            if (gridControl1.DataSource != null)
            {
                DataTable dt = (DataTable)gridControl1.DataSource;
                // get only the WOT cells, the last 16 integers
                // and the columns which hold the rpm stages
                chartControl1.Series.Clear();
                string powerLabel = "Power (bhp)";
                if (checkEdit5.Checked) powerLabel = "Power (kW)";
                string torqueLabel = "Torque (Nm)";
                if (checkEdit6.Checked) torqueLabel = "Torque (lbft)";

                string injectorDCLabel = "Injector DC";
                string targetLambdaLabel = "Target lambda";

                powerSeries = chartControl1.Series.Add(powerLabel, DevExpress.XtraCharts.ViewType.Spline);
                torqueSeries = chartControl1.Series.Add(torqueLabel, DevExpress.XtraCharts.ViewType.Spline);
                injectorDCSeries = chartControl1.Series.Add(injectorDCLabel, DevExpress.XtraCharts.ViewType.Spline);
                lambdaSeries = chartControl1.Series.Add(targetLambdaLabel, DevExpress.XtraCharts.ViewType.Spline);
                EGTSeries = chartControl1.Series.Add("EGT estimate", DevExpress.XtraCharts.ViewType.Spline);
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


                chartControl1.Series[powerSeries].View.Color = Color.Red;
                chartControl1.Series[torqueSeries].View.Color = Color.Blue;
                chartControl1.Series[injectorDCSeries].View.Color = Color.GreenYellow;
                chartControl1.Series[lambdaSeries].View.Color = Color.DarkGreen;
                chartControl1.Series[EGTSeries].View.Color = Color.Plum;
                chartControl1.Series[powerSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[powerSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[torqueSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[torqueSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[injectorDCSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[injectorDCSeries].ValueScaleType = ScaleType.Numerical;
                chartControl1.Series[lambdaSeries].ArgumentScaleType = ScaleType.Qualitative;
                chartControl1.Series[lambdaSeries].ValueScaleType = ScaleType.Numerical;
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

                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    double o = Convert.ToDouble(dt.Rows[0].ItemArray.GetValue(i));
                    // convert to hp
                    int rpm = Convert.ToInt32(x_axisvalues.GetValue(i));
                    int torque = AirmassToTorque(Convert.ToInt32(o), rpm, checkEdit7.Checked);
                    int horsepower = TorqueToPower(torque, rpm);
                    int injDC = CalculateInjectorDC(Convert.ToInt32(o), rpm);
                    int TargetLambda = CalculateTargetLambda(Convert.ToInt32(o), rpm);
                    int EstimateEGT = CalculateEstimateEGT(Convert.ToInt32(o), rpm);
                    if (checkEdit5.Checked) horsepower = TorqueToPowerkW(torque, rpm);
                    if (checkEdit6.Checked) torque = AirmassToTorqueLbft(Convert.ToInt32(o), rpm, checkEdit7.Checked);

                    double[] dvals = new double[1];
                    dvals.SetValue(Convert.ToDouble(horsepower), 0);
                    chartControl1.Series[powerSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvals));

                    double[] dvalstorq = new double[1];
                    dvalstorq.SetValue(Convert.ToDouble(torque), 0);
                    chartControl1.Series[torqueSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalstorq));

                    //<GS-21062010>
                    double[] dvalsinjDC = new double[1];
                    dvalsinjDC.SetValue(Convert.ToDouble(injDC), 0);
                    chartControl1.Series[injectorDCSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsinjDC));

                    //<GS-24062010>
                    double[] dvalsLambda = new double[1];
                    dvalsLambda.SetValue(Convert.ToDouble(TargetLambda), 0);
                    chartControl1.Series[lambdaSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsLambda));

                    //<GS-03082010>
                    double[] dvalsEGT = new double[1];
                    dvalsEGT.SetValue(Convert.ToDouble(EstimateEGT), 0);
                    chartControl1.Series[EGTSeries].Points.Add(new SeriesPoint(Convert.ToDouble(rpm), dvalsEGT));
                }

                /*if ((XYDiagram)chartControl1.Diagram != null)
                {
                    //((XYDiagram)chartControl1.Diagram).AxisX.Arg
                }*/


            }
            /*if (m_current_comparefilename != string.Empty)
            {
                DataTable dt2 = CalculateDataTable(m_current_comparefilename, Compare_symbol_collection);
                LoadExtraGraphFromCompareBin(dt2, m_current_comparefilename);
            }*/
            // load the graph with the current details from the airmass result viewer
            /*
                int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                int torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm);
                int horsepower = TorqueToPower(torque, rpm);
                e.DisplayText = horsepower.ToString();             
            */

        }


        /*private int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            return Convert.ToInt32(power);
        }*/


        private int CalculateEstimateEGT(int airmass, int rpm)
        {
            int retval = 0;
            // first calulcate simple
            if (EGTMap != null)
            {
                double egtvalue = GetInterpolatedTableValue(EGTMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                if (fuelVEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
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
            if (retval > 50 && checkEdit2.Checked) retval -= 50; // correction for E85 fuel, 50 degrees off!
            return retval;
        }

        //DONE: Switch to InjectorDC in table view
        //DONE: Add extra line in dyno to show inverted VE value (lambda)
        //DONE: Count E85 checkbox into equasion

        private int CalculateTargetLambda(int airmass, int rpm)
        {
            int retval = 0;
            // first calulcate simple
            if (fuelVEMap != null)
            {
                double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                vecorr /= 128;
                vecorr = 1 / vecorr;
                vecorr *= 100; // range correction
                retval = Convert.ToInt32(vecorr);

            }
            return retval;
        }

        private int CalculateInjectorDC(int airmass, int rpm)
        {
            // calculate injector DC
            // needs injector constant, ve map, battery correction map
            int retval = 0;
            // first calulcate simple
            if (m_injectorConstant > 0 && fuelVEMap != null)
            {
                float frpm = (float)rpm;
                frpm /= 2; // injection once every 2 rounds in 4 stroke engine
                float fairmass = (float)airmass;
                float airmassperminute = fairmass * frpm;
                airmassperminute /= 1000; // from mg/c to g/c

                float m_requiredFuelForLambda = airmassperminute / 14.7F;
                if (checkEdit2.Checked)
                {
                    // running E85, different target lambda
                    m_requiredFuelForLambda = airmassperminute / 9.76F;
                }

                // get correct data from fuelVEmap ... by airmass and rpm
                double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                vecorr /= 128; //table is a byte value, with range 0.00-2.55 , 1.00 is 128
                m_requiredFuelForLambda *= (float)vecorr;

                float injectorDC = m_requiredFuelForLambda / (float)m_injectorConstant;

                injectorDC *= 100;
                retval = Convert.ToInt32(injectorDC);

            }
            return retval;
        }

        private void UpdateGraphVisibility()
        {
            if (powerSeries >= 0) chartControl1.Series[powerSeries].Visible = checkEdit8.Checked;
            if (torqueSeries >= 0) chartControl1.Series[torqueSeries].Visible = checkEdit9.Checked;
            if (injectorDCSeries >= 0) chartControl1.Series[injectorDCSeries].Visible = checkEdit10.Checked;
            if (lambdaSeries >= 0) chartControl1.Series[lambdaSeries].Visible = checkEdit11.Checked;
            if (EGTSeries >= 0) chartControl1.Series[EGTSeries].Visible = checkEdit12.Checked;
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
        {
            // refresh
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
        }

        private void checkEdit6_CheckedChanged(object sender, EventArgs e)
        {
            // refresh
            gridControl1.Refresh();
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            {
                LoadGraphWithDetails();
            }
        }

        private void checkEdit7_CheckedChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /*private double GetInterpolatedTableValue(int[] table, int[] xaxis, int[] yaxis, int yvalue, int xvalue)
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
                //AddDebugLog("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
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
                //AddDebugLog("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
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

                //AddDebugLog("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //AddDebugLog("result = " + result.ToString());
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to interpolate: " + E.Message);
            }
            return result;


        }*/

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
                //AddDebugLog("RPMindex = " + m_rpmindex + " Percentage = " + m_rpmpercentage.ToString() + " MAPindex = " + m_mapindex.ToString() + " Percentage = " + m_mappercentage.ToString());
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
                //AddDebugLog("a1 = " + a1.ToString() + " a2 = " + a2.ToString() + " b1 = " + b1.ToString() + " b2 = " + b2.ToString());
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

                //AddDebugLog("aval = " + aval.ToString() + " bval = " + bval.ToString());
                double abdiff = Math.Abs(aval - bval);
                if (aval > bval) result = aval - (m_ypercentage * abdiff);
                else result = aval + (m_ypercentage * abdiff);
                //AddDebugLog("result = " + result.ToString());
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to interpolate: " + E.Message);
            }
            return result;


        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage1.Name)
            {
                // in table view
            }
            else
            {
                // in graph view
                LoadGraphWithDetails();
            }
        }

    }
}