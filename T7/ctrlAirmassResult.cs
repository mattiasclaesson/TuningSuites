using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraCharts;
using CommonSuite;

namespace T7
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
        OverBoostLimiter,
        AirTorqueCalibration,
        TorqueLimiterEngineE85Auto
    }

    public partial class ctrlAirmassResult : DevExpress.XtraEditors.XtraUserControl
    {
        int rows;
        int columns;

        int m_injectorConstant = 0;

        private int[] y_axisvalues;
        private int[] x_axisvalues;
        private int[] limitermap;
        byte[] fuelVEMap;
        byte[] E85VEMap;

        int[] EGTMap;
        int[] open_loop;

        int[] fuelVExaxis;
        int[] fuelVEyaxis;
        int[] open_loopyaxis;


        private string m_currentfile = string.Empty;
        private int m_MaxValueInTable = 0;
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
                        //                        LogHelper.Log("Software is open because of symbol: " + sh.Varname);
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
                //LogHelper.Log("Working with: " + m_currentSramOffsett.ToString("X8"));
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
                if (sh.Userdescription == symbolname || sh.Varname == symbolname)
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
                FileStream fsi1 = File.OpenRead(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                string temp = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    retval.SetValue(br1.ReadByte(), i);
                }
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                LogHelper.Log(E.Message);
            }
            return retval;
        }

        private int[] readIntdatafromfile(string filename, int address, int length)
        {
            int[] retval = new int[length / 2];
            try
            {
                FileStream fsi1 = File.OpenRead(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                string temp = string.Empty;
                int j = 0;
                for (int i = 0; i < length; i += 2)
                {
                    byte b1 = br1.ReadByte();
                    byte b2 = br1.ReadByte();
                    int value = Convert.ToInt32(b1) * 256 + Convert.ToInt32(b2);
                    retval.SetValue(value, j++);
                }
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                LogHelper.Log(E.Message);
            }
            return retval;
        }

        private bool IsOverboostEnabled(string filename, SymbolCollection symbols)
        {
            bool retval = false;
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Varname == "TorqueCal.EnableOverBoost" || sh.Userdescription == "TorqueCal.EnableOverBoost")
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

        private bool IsBinaryBiopower(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Varname == "TorqueCal.M_EngMaxE85Tab" || sh.Userdescription == "TorqueCal.M_EngMaxE85Tab")
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBinaryConvertable(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Varname == "TorqueCal.M_CabGearLim" || sh.Userdescription == "TorqueCal.M_CabGearLim")
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBinaryBiopowerAuto(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Varname == "TorqueCal.M_EngMaxE85TabAut" || sh.Userdescription == "TorqueCal.M_EngMaxE85TabAut")
                {
                    return true;
                }
            }
            return false;
        }

        public void Calculate(string filename, SymbolCollection symbols)
        {
            // do the math!

            if (!IsBinaryBiopower(symbols))
            {
                checkEdit2.Checked = false;
                checkEdit2.Enabled = false;
            }
            else
            {
                checkEdit2.Enabled = true;
            }
            if (IsBinaryConvertable(symbols))
            {
                checkEdit3.Enabled = true;
            }
            else
            {
                checkEdit3.Checked = false;
                checkEdit3.Enabled = false;
            }
            if (IsOverboostEnabled(filename, symbols))
            {
                checkEdit4.Enabled = true;
            }
            else
            {
                checkEdit4.Enabled = false;
            }

            bool e85automatic = IsBinaryBiopowerAuto(symbols);
            
            int[] pedalrequestmap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(symbols, "PedalMapCal.m_RequestMap"));
            try
            {
                int[] injConstant = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "InjCorrCal.InjectorConst"), GetSymbolLength(symbols, "InjCorrCal.InjectorConst"));
                m_injectorConstant = Convert.ToInt32(injConstant.GetValue(0));
                fuelVEMap = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.Map"), GetSymbolLength(symbols, "BFuelCal.Map"));
                E85VEMap = readdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.E85Map"), GetSymbolLength(symbols, "BFuelCal.E85Map"));
                fuelVExaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.AirXSP"), GetSymbolLength(symbols, "BFuelCal.AirXSP"));
                fuelVEyaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BFuelCal.RpmYSP"), GetSymbolLength(symbols, "BFuelCal.RpmYSP"));
                open_loop = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.MaxLoadNormTab"), GetSymbolLength(symbols, "LambdaCal.MaxLoadNormTab"));
                if (checkEdit2.Checked)
                {
                    open_loop = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.MaxLoadE85Tab"), GetSymbolLength(symbols, "LambdaCal.MaxLoadE85Tab"));//
                }
                open_loopyaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LambdaCal.RpmSp"), GetSymbolLength(symbols, "LambdaCal.RpmSp"));

                if (SymbolExists("ExhaustCal.T_Lambda1Map"))
                {
                    EGTMap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "ExhaustCal.T_Lambda1Map"), GetSymbolLength(symbols, "ExhaustCal.T_Lambda1Map"));
                }
            }
            catch (Exception E)
            {
                LogHelper.Log(E.Message);
            }
            limitermap = new int[pedalrequestmap.Length];
            int[] resulttable = new int[pedalrequestmap.Length]; // result 
            rows = GetSymbolLength(symbols, "PedalMapCal.n_EngineMap") / 2;
            columns = GetSymbolLength(symbols, "PedalMapCal.X_PedalMap") / 2;
            int[] pedalXAxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.n_EngineMap"), GetSymbolLength(symbols, "PedalMapCal.n_EngineMap"));
            int[] pedalYAxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.X_PedalMap"), GetSymbolLength(symbols, "PedalMapCal.X_PedalMap"));
            y_axisvalues = pedalYAxis;
            x_axisvalues = pedalXAxis;
            // also get the axis for pedalrequestmap
            // x = PedalMapCal.n_EngineMap (1x) rpm
            // y = PedalMapCal.X_PedalMap (10x) pedalposition (%)
            for (int colcount = 0; colcount < columns; colcount++)
            {
                for (int rowcount = 0; rowcount < rows; rowcount++)
                {
                    // get the current value from the request map
                    int airmassrequestforcell = (int)pedalrequestmap.GetValue((colcount * rows) + rowcount);
                    limitType limiterType = limitType.None;
                    int resultingAirMass = CalculateMaxAirmassforcell(symbols, filename, ((int)pedalYAxis.GetValue(colcount) / 10), /* rpm */(int)pedalXAxis.GetValue(rowcount), airmassrequestforcell, checkEdit1.Checked, checkEdit2.Checked, checkEdit3.Checked, checkEdit4.Checked, e85automatic, out limiterType);
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
            else if (xtraTabControl1.SelectedTabPage.Name == tabCompressormap.Name)
            {
                LoadCompressorMapWithDetails();
            }
        }

        private bool SymbolExists(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname) return true;
            }
            return false;
        }

        private int CalculateMaxAirmassforcell(SymbolCollection symbols, string filename, int pedalposition, int rpm, int requestairmass, bool autogearbox, bool E85, bool Convertable, bool OverboostEnabled, bool E85Automatic, out limitType limiterType)
        {
            int retval = requestairmass;
            //LogHelper.Log("Pedalpos: " + pedalposition.ToString() + " Rpm: " + rpm.ToString() + " requests: " + requestairmass.ToString() + " mg/c");
            //calculate the restricted airmass for the current point
            // first, check the airmasslimiter
            //            retval = CheckAgainstAirmassLimiters(rpm, requestairmass, autogearbox);
            // then check against torquelimiters
            limiterType = limitType.None;

            limitType TrqLimiterType = limitType.None;
            retval = CheckAgainstTorqueLimiters(symbols, filename, rpm, requestairmass, E85, Convertable, autogearbox, OverboostEnabled, E85Automatic, out TrqLimiterType);
            if (retval < requestairmass) limiterType = TrqLimiterType;
            // finally check agains fuelcut limiter???
            limitType AirmassLimiterType = limitType.None;
            int TorqueLimitedAirmass = retval;

            retval = CheckAgainstAirmassLimiters(symbols, filename, rpm, retval, autogearbox, ref AirmassLimiterType);

            retval = CheckAgainstTurboSpeedLimiter(symbols, filename, rpm, retval, ref AirmassLimiterType);

            retval = CheckAgainstFuelcutLimiter(symbols, filename, retval, ref AirmassLimiterType);

            if (retval < TorqueLimitedAirmass) limiterType = AirmassLimiterType;


            return retval;
        }

        private bool HasBinaryTorqueLimiterEnabled(SymbolCollection symbols, string filename)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Varname == "TorqueCal.ST_Loop" || sh.Userdescription == "TorqueCal.ST_Loop")
                {

                    byte[] toqruelimdata = readdatafromfile(filename, (int)GetSymbolAddress(m_symbols, "TorqueCal.ST_Loop"), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) == 0x00)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private int CheckAgainstTorqueLimiters(SymbolCollection symbols, string filename, int rpm, int requestedairmass, bool E85, bool Convertable, bool Automatic, bool OverboostEnabled, bool E85Automatic, out limitType TrqLimiter)
        {
            //only if torquelimiters are enabled
            // first convert airmass torque to torque using TorqueCal.M_NominalMap
            // axis are 
            // x = TorqueCal.m_AirXSP (airmass)
            // y = TorqueCal.n_EngYSP (rpm)
            int torque = 0;
            TrqLimiter = limitType.None;
            int LimitedAirMass = requestedairmass;
            /*if (requestedairmass == 1100 && rpm == 2400)
            {
                LogHelper.Log("1100");
            }*/
            if (HasBinaryTorqueLimiterEnabled(symbols, filename))
            {
                /*int[] nominaltorque = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(symbols, "TorqueCal.M_NominalMap"));
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominaltorque.SetValue(val, a);
                }*/
                int[] xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(symbols, "TorqueCal.m_AirXSP"));
                int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(symbols, "TorqueCal.n_EngYSP"));
                //torque = Convert.ToInt32(GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, requestedairmass));
                torque = AirmassToTorque(requestedairmass, rpm, checkEdit7.Checked);
                
                // check against TorqueCal.M_EngMaxTab, TorqueCal.M_ManGearLim and TorqueCal.M_5GearLimTab
                int[] enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxTab"));
                if (E85Automatic && E85 && Automatic)
                {
                    enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxE85TabAut"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxE85TabAut"));
                }
                else
                {
                    // Old style binary where there are no dedicated TorqueCal.M_EngMaxE85TabAut
                    if (Automatic)
                    {
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxAutTab"));
                    }
                    if (E85)
                    {
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(symbols, "TorqueCal.M_EngMaxE85Tab"));
                    }
                }
                if (OverboostEnabled)
                {
                    enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_OverBoostTab"), GetSymbolLength(symbols, "TorqueCal.M_OverBoostTab"));
                }

                int[] xdummy = new int[1];
                xdummy.SetValue(0, 0);
                int torquelimit1 = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
                //requestedairmass
                if (torque > torquelimit1)
                {
                    //LogHelper.Log("Torque is limited from " + torque.ToString() + " to " + torquelimit1.ToString() + " at " + rpm.ToString() + " rpm");
                    torque = torquelimit1;
                    if (E85Automatic && E85 && Automatic)
                    {
                        TrqLimiter = limitType.TorqueLimiterEngineE85Auto;
                    }
                    else if (E85)
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
                if (!Automatic)
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
                        // extra check
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_CabGearLim"), GetSymbolLength(symbols, "TorqueCal.M_CabGearLim"));
                        //
                        int torquelimitConvertable = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, gears, comboBoxEdit1.SelectedIndex, 0));
                        if (torque > torquelimitConvertable)
                        {
                            torque = torquelimitConvertable;
                            TrqLimiter = limitType.TorqueLimiterGear;
                            //LogHelper.Log("Convertable gear torque limit hit");
                        }
                    }
                    else
                    {
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(symbols, "TorqueCal.M_ManGearLim"));
                        //
                        int torquelimitManual = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, gears, comboBoxEdit1.SelectedIndex, 0));
                        if (torque > torquelimitManual)
                        {
                            torque = torquelimitManual;
                            TrqLimiter = limitType.TorqueLimiterGear;
                            //LogHelper.Log("Manual gear torque limit hit");
                        }
                    }

                    // and check 5th gear limiter as well!!! (if checkbox is 5)
                    //TorqueCal.M_5GearTab		TorqueCal.n_Eng5GearSP
                    if (comboBoxEdit1.SelectedIndex == 5)
                    {
                        //LogHelper.Log("Checking fifth gear!");
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(symbols, "TorqueCal.M_5GearLimTab"));
                        yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_Eng5GearSP"), GetSymbolLength(symbols, "TorqueCal.n_Eng5GearSP"));
                        int torquelimit5th = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
                        if (torque > torquelimit5th)
                        {
                            torque = torquelimit5th;
                            TrqLimiter = limitType.TorqueLimiterGear;
                            //LogHelper.Log("Fifth gear torque limit hit");
                        }
                    }
                }
                else
                {
                    // automatic!
                    if (comboBoxEdit1.SelectedIndex == 0)
                    {
                        //LogHelper.Log("Checking reverse gear!");
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_ReverseTab"), GetSymbolLength(symbols, "TorqueCal.M_ReverseTab"));
                        yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_EngSP"), GetSymbolLength(symbols, "TorqueCal.n_EngSP"));
                        int torquelimitreverse = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
                        if (torque > torquelimitreverse)
                        {
                            torque = torquelimitreverse;
                            TrqLimiter = limitType.TorqueLimiterGear;
                            //LogHelper.Log("Reverse gear torque limit hit");
                        }
                    }

                    // first gear
                    //TorqueCal.M_1GearTab		TorqueCal.n_Eng1GearSP
                    else if (comboBoxEdit1.SelectedIndex == 1)
                    {
                        //LogHelper.Log("Checking first gear!");
                        enginetorquelim = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_1GearTab"), GetSymbolLength(symbols, "TorqueCal.M_1GearTab"));
                        yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.n_Eng1GearSP"), GetSymbolLength(symbols, "TorqueCal.n_Eng1GearSP"));
                        int torquelimit1st = Convert.ToInt32(GetInterpolatedTableValue(enginetorquelim, xdummy, yaxis, rpm, 0));
                        if (torque > torquelimit1st)
                        {
                            torque = torquelimit1st;
                            TrqLimiter = limitType.TorqueLimiterGear;
                            //LogHelper.Log("First gear torque limit hit");
                        }
                    }
                }

                int[] airtorquemap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(symbols, "TorqueCal.m_AirTorqMap"));
                int[] xairtorque = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(symbols, "TorqueCal.M_EngXSP"));
                int TestLimitedAirmass = Convert.ToInt32(GetInterpolatedTableValue(airtorquemap, xairtorque, yaxis, rpm, torque));
                if (TestLimitedAirmass < LimitedAirMass)
                {
                    LimitedAirMass = TestLimitedAirmass;
                    if(TrqLimiter == limitType.None) TrqLimiter = limitType.AirTorqueCalibration;
                }
            }
//            LogHelper.Log("1. Torque is " + torque.ToString() + " at " + rpm.ToString() + " rpm and airmass " + requestedairmass.ToString() + " res: " + LimitedAirMass.ToString() + " type: " + TrqLimiter.ToString());
            if (TrqLimiter == limitType.None) LimitedAirMass = requestedairmass; // bugfix for if no limiter is active
//            LogHelper.Log("2. Torque is " + torque.ToString() + " at " + rpm.ToString() + " rpm and airmass " + requestedairmass.ToString() + " res: " + LimitedAirMass.ToString() + " type: " + TrqLimiter.ToString());

            return LimitedAirMass;
        }

        private int CheckAgainstFuelcutLimiter(SymbolCollection symbols, string filename, int requestedairmass, ref limitType AirmassLimiter)
        {
            int retval = requestedairmass;
            if ((int)GetSymbolAddress(symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                int[] fuelcutlimit = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(symbols, "FCutCal.m_AirInletLimit"));
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

        private int CheckAgainstTurboSpeedLimiter(SymbolCollection symbols, string filename, int rpm, int requestedairmass, ref limitType AirmassLimiter)
        {
            int[] turbospeed = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.TurboSpeedTab"), GetSymbolLength(symbols, "LimEngCal.TurboSpeedTab"));
            int[] nullvalue = new int[1];
            nullvalue.SetValue(0, 0);
            int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.p_AirSP"), GetSymbolLength(symbols, "LimEngCal.p_AirSP"));
            int ambientpressure = Convert.ToInt32(spinEdit1.EditValue) * 10; // 100.0 kPa = 1000 as table value unit is 0.1 kPa
            int airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(turbospeed, nullvalue, yaxis, ambientpressure, 0));

            // Second limitation is based on engine rpm to prevent the turbo from overspeeding at high rpm.
            // Interpolated correction factor applied to the calculated airmass value from main turbospeed table LimEngCal.TurboSpeedTab.
            int[] turbospeed2 = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.TurboSpeedTab2"), GetSymbolLength(symbols, "LimEngCal.TurboSpeedTab2"));
            int[] yaxis2 = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "LimEngCal.n_EngSP"), GetSymbolLength(symbols, "LimEngCal.n_EngSP"));
            int correctionfactor = Convert.ToInt32(GetInterpolatedTableValue(turbospeed2, nullvalue, yaxis2, rpm, 0));
            airmasslimit = airmasslimit * correctionfactor / 1000; // correction factor value unit is 0.001
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = limitType.TurboSpeedLimiter;
            }
            return requestedairmass;
        }

        private int CheckAgainstAirmassLimiters(SymbolCollection symbols, string filename, int rpm, int requestedairmass, bool autogearbox, ref limitType AirmassLimiter)
        {
            //AirmassLimiter = limitType.None;
            //Y axis = BstKnkCal.n_EngYSP needed for interpolation (16)
            //X axis = BstKnkCal.OffsetXSP needed for length (16)
            // check against BstKnkCal.MaxAirmass
            int cols = GetSymbolLength(symbols, "BstKnkCal.OffsetXSP") / 2;
            int rows = GetSymbolLength(symbols, "BstKnkCal.n_EngYSP") / 2;
            // only the right-most column (no knock)
            int[] bstknk = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmass"));
            if (autogearbox)
            {
                bstknk = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(symbols, "BstKnkCal.MaxAirmassAu"));
            }
            int[] xaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.OffsetXSP"), GetSymbolLength(symbols, "BstKnkCal.OffsetXSP"));
            for (int a = 0; a < xaxis.Length; a++)
            {
                int val = (int)xaxis.GetValue(a);
                if (val > 32000) val = -(65536 - val);
                xaxis.SetValue(val, a);
            }
            int[] yaxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "BstKnkCal.n_EngYSP"), GetSymbolLength(symbols, "BstKnkCal.n_EngYSP"));
            int airmasslimit = Convert.ToInt32(GetInterpolatedTableValue(bstknk, xaxis, yaxis, rpm, 0));
            if (airmasslimit < requestedairmass)
            {
                requestedairmass = airmasslimit;
                AirmassLimiter = limitType.AirmassLimiter;
                //LogHelper.Log("Reduced airmass because of BstKnkCal.MaxAirmass: " + requestedairmass.ToString() + " rpm: " + rpm.ToString());
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
                LogHelper.Log("Failed to interpolate: " + E.Message);
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
                LogHelper.Log("Failed to interpolate: " + E.Message);
            }
            return result;


        }
        private int TorqueToPowerkW(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            // convert to kW in stead of horsepower
            power *= 0.73549875;
            return Convert.ToInt32(power);
        }

        private int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
            return Convert.ToInt32(power);
        }

        private int AirmassToTorqueLbft(int airmass, int rpm, bool TrionicStyle)
        {
            double tq = Convert.ToDouble(airmass) / 3.1;
            if (TrionicStyle)
            {
                int[] nominaltorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(m_symbols, "TorqueCal.M_NominalMap"));
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominaltorque.SetValue(val, a);
                }
                int[] xaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(m_symbols, "TorqueCal.m_AirXSP"));
                int[] yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(m_symbols, "TorqueCal.n_EngYSP"));
                //int torque = Convert.ToInt32(GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, airmass));
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
                //tq /= 1.56; // to lbft
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
                int[] nominaltorque = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(m_symbols, "TorqueCal.M_NominalMap"));
                for (int a = 0; a < nominaltorque.Length; a++)
                {
                    int val = (int)nominaltorque.GetValue(a);
                    if (val > 32000) val = -(65536 - val);
                    nominaltorque.SetValue(val, a);
                }
                int[] xaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(m_symbols, "TorqueCal.m_AirXSP"));
                int[] yaxis = readIntdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.n_EngYSP"), GetSymbolLength(m_symbols, "TorqueCal.n_EngYSP"));
                //int torque = Convert.ToInt32(GetInterpolatedTableValue(nominaltorque, xaxis, yaxis, rpm, airmass));
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
        private void CastStartViewerEvent(string mapname)
        {
            if (onStartTableViewer != null)
            {
                onStartTableViewer(this, new StartTableViewerEventArgs(mapname));
            }
        }
        
        int powerSeries = -1;
        int powerCompareSeries = -1;
        int torqueCompareSeries = -1;
        int torqueSeries = -1;
        int injectorDCSeries = -1;
        int lambdaSeries = -1;
        int EGTSeries = -1;

        private string MaximizeFileLength(string filename)
        {
            string retval = filename;
            if (retval.Length > 16) retval = retval.Substring(0, 14) + "..";
            return retval;
        }



        private void LoadExtraGraphFromCompareBin(DataTable dt, string filename)
        {
            LogHelper.Log("Loading additional data for: " + filename);
            string powerLabel = "Power (bhp) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            if (checkEdit5.Checked) powerLabel = "Power (kW) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            string torqueLabel = "Torque (Nm) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
            if (checkEdit6.Checked) torqueLabel = "Torque (lbft) " + MaximizeFileLength(Path.GetFileNameWithoutExtension(filename));
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
            if (m_current_comparefilename != string.Empty)
            {
                DataTable dt2 = CalculateDataTable(m_current_comparefilename, Compare_symbol_collection);
                LoadExtraGraphFromCompareBin(dt2, m_current_comparefilename);
            }
            // load the graph with the current details from the airmass result viewer
            /*
                int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                int torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm);
                int horsepower = TorqueToPower(torque, rpm);
                e.DisplayText = horsepower.ToString();             
            */

        }

        private int CalculateEstimateEGT(int airmass, int rpm)
        {
            int retval = 0;
            // first calulcate simple
            if (EGTMap != null)
            {
                double egtvalue = GetInterpolatedTableValue(EGTMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                if (checkEdit2.Checked)
                {
                    if (E85VEMap != null)
                    {
                        double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
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
                        double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
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

            if (checkEdit2.Checked)
            {
                if (E85VEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                    vecorr /= 100;
                    vecorr = 1 / vecorr;
                    vecorr *= 100; // range correction
                    retval = Convert.ToInt32(vecorr);
                }
            }
            else
            {
                if (fuelVEMap != null)
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                    vecorr /= 100;
                    vecorr = 1 / vecorr;
                    vecorr *= 100; // range correction
                    retval = Convert.ToInt32(vecorr);
                }
            }
            return retval;
        }
        private void LoadCompressorMapWithDetails()
        {
            // we should get the top line like for the chart

            if (gridControl1.DataSource != null)
            {
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
                ctrlCompressorMap1.Rpm_points = x_axisvalues;
                PartNumberConverter pnc = new PartNumberConverter();
                //
                T7FileHeader header = new T7FileHeader();
                header.init(m_currentfile, false);
                ECUInformation ecuinfo = pnc.GetECUInfo(header.getPartNumber().Trim(), "");
                if (!ctrlCompressorMap1.IsInitiallyLoaded)
                {
                    if (ecuinfo.Is2point3liter) ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter23;
                    else ctrlCompressorMap1.Current_engineType = ctrlCompressorMap.EngineType.Liter2;
                    if (ecuinfo.Isaero) ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.TD04);
                    else ctrlCompressorMap1.SetCompressorType(ctrlCompressorMap.CompressorMap.GT17);
                }
                ctrlCompressorMap1.IsInitiallyLoaded = true;
                ctrlCompressorMap1.Redraw();
            }

        }
        private DataTable CalculateDataTable(string filename, SymbolCollection symbols)
        {
            // do the math!
            if (IsBinaryBiopower(symbols))
            {
                checkEdit2.Enabled = true;
            }
            else
            {
                checkEdit2.Checked = false;
                checkEdit2.Enabled = false;
            }
            if (IsBinaryConvertable(symbols))
            {
                checkEdit3.Enabled = true;
            }
            else
            {
                checkEdit3.Checked = false;
                checkEdit3.Enabled = false;
            }
            if (IsOverboostEnabled(filename, symbols))
            {
                checkEdit4.Enabled = true;
            }
            else
            {
                checkEdit4.Enabled = false;
            }

            bool e85automatic = IsBinaryBiopowerAuto(symbols);

            int[] pedalrequestmap = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(symbols, "PedalMapCal.m_RequestMap"));
            //limitermap = new int[pedalrequestmap.Length];
            int[] resulttable = new int[pedalrequestmap.Length]; // result 
            rows = GetSymbolLength(symbols, "PedalMapCal.n_EngineMap") / 2;
            columns = GetSymbolLength(symbols, "PedalMapCal.X_PedalMap") / 2;
            int[] pedalXAxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.n_EngineMap"), GetSymbolLength(symbols, "PedalMapCal.n_EngineMap"));
            int[] pedalYAxis = readIntdatafromfile(filename, (int)GetSymbolAddress(symbols, "PedalMapCal.X_PedalMap"), GetSymbolLength(symbols, "PedalMapCal.X_PedalMap"));
            y_axisvalues = pedalYAxis;
            x_axisvalues = pedalXAxis;
            // also get the axis for pedalrequestmap
            // x = PedalMapCal.n_EngineMap (1x) rpm
            // y = PedalMapCal.X_PedalMap (10x) pedalposition (%)
            for (int colcount = 0; colcount < columns; colcount++)
            {
                for (int rowcount = 0; rowcount < rows; rowcount++)
                {
                    // get the current value from the request map
                    int airmassrequestforcell = (int)pedalrequestmap.GetValue((colcount * rows) + rowcount);
                    //LogHelper.Log("Current request = " + airmassrequestforcell.ToString() + " mg/c");
                    limitType limiterType = limitType.None;
                    int resultingAirMass = CalculateMaxAirmassforcell(symbols, filename, ((int)pedalYAxis.GetValue(colcount) / 10), /* rpm */(int)pedalXAxis.GetValue(rowcount), airmassrequestforcell, checkEdit1.Checked, checkEdit2.Checked, checkEdit3.Checked, checkEdit4.Checked, e85automatic, out limiterType);
                    resulttable.SetValue(resultingAirMass, (colcount * rows) + rowcount);
                    //limitermap.SetValue(limiterType, (colcount * rows) + rowcount);
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
            //gridControl1.DataSource = dt;
            //if (xtraTabControl1.SelectedTabPage.Name == xtraTabPage2.Name)
            //{
            //LoadGraphWithDetails();
            //}

            return dt;
        }

        private void UpdateGraphVisibility()
        {
            if (powerSeries >= 0) chartControl1.Series[powerSeries].Visible = checkEdit8.Checked;
            if (torqueSeries >= 0) chartControl1.Series[torqueSeries].Visible = checkEdit9.Checked;
            if (injectorDCSeries >= 0) chartControl1.Series[injectorDCSeries].Visible = checkEdit10.Checked;
            if (lambdaSeries >= 0) chartControl1.Series[lambdaSeries].Visible = checkEdit11.Checked;
            if (EGTSeries >= 0) chartControl1.Series[EGTSeries].Visible = checkEdit12.Checked;
        }
        string m_current_softwareversion = string.Empty;
        string m_current_comparefilename = string.Empty;
        SymbolCollection Compare_symbol_collection = new SymbolCollection();

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
                if (checkEdit2.Checked)
                {
                    double vecorr = GetInterpolatedTableValue(E85VEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                    vecorr /= 100;
                    m_requiredFuelForLambda *= (float)vecorr;
                    float injectorDC = m_requiredFuelForLambda / (float)m_injectorConstant;
                    injectorDC *= 100;
                    retval = Convert.ToInt32(injectorDC);

                }
                else
                {
                    double vecorr = GetInterpolatedTableValue(fuelVEMap, fuelVExaxis, fuelVEyaxis, rpm, airmass);
                    vecorr /= 100;
                    m_requiredFuelForLambda *= (float)vecorr;
                    float injectorDC = m_requiredFuelForLambda / (float)m_injectorConstant;
                    injectorDC *= 100;
                    retval = Convert.ToInt32(injectorDC);
                }
                

            }
            return retval;
        }


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_current_comparefilename = ofd.FileName;
                SymbolTranslator translator = new SymbolTranslator();
                string help = string.Empty;
                FileInfo fi = new FileInfo(m_current_comparefilename);
                fi.IsReadOnly = false;

                try
                {
                    T7FileHeader t7InfoHeader = new T7FileHeader();
                    if (t7InfoHeader.init(m_current_comparefilename, false))
                    {
                        m_current_softwareversion = t7InfoHeader.getSoftwareVersion();
                    }
                    else
                    {
                        m_current_softwareversion = "";
                    }
                }
                catch (Exception E2)
                {
                    LogHelper.Log(E2.Message);
                }
                Trionic7File t7file = new Trionic7File();
                
                Compare_symbol_collection = t7file.ExtractFile(m_current_comparefilename, 44, m_current_softwareversion);
                // so... now determine the max values for the compare file
                // show the dynograph
                xtraTabControl1.SelectedTabPage = xtraTabPage2;
                LoadGraphWithDetails(); // initial values from original bin
                //DataTable dt = CalculateDataTable(m_current_comparefilename, Compare_symbol_collection);
                //LoadExtraGraphFromCompareBin(dt, m_current_comparefilename);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
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

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit3_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit4_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
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
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit7_CheckedChanged(object sender, EventArgs e)
        {
            Calculate(m_currentfile, m_symbols);
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
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

        private void checkEdit6_CheckedChanged(object sender, EventArgs e)
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
        }

        private void labelControl1_DoubleClick(object sender, EventArgs e)
        {
            // start airmass limiter viewer
            // if aut
            if (checkEdit1.Checked)
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

        private void labelControl3_DoubleClick(object sender, EventArgs e)
        {
            // start engine torque limiter
            if (checkEdit1.Checked)
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
            if (checkEdit1.Checked)
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
            else if (checkEdit3.Checked)
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
                        //limitermap
                        int row = rows - (e.RowHandle + 1);
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
                        else if (curLimit == limitType.TorqueLimiterEngineE85Auto)
                        {
                            e.Graphics.FillPolygon(Brushes.White, pnts, System.Drawing.Drawing2D.FillMode.Winding);
                        }
                        if (comboBoxEdit2.SelectedIndex == 1)
                        {
                            // convert airmass to torque
                            int rpm = Convert.ToInt32(x_axisvalues.GetValue(e.Column.AbsoluteIndex));
                            int torque = AirmassToTorque(Convert.ToInt32(e.CellValue), rpm, checkEdit7.Checked);
                            if (checkEdit6.Checked)
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
                            int horsepower = TorqueToPower(torque, rpm);
                            if (checkEdit5.Checked)
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
                LogHelper.Log(E.Message);
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
                    if (y_axisvalues.Length > 0)
                    {
                        if (y_axisvalues.Length > e.RowHandle)
                        {
                            int value = (int)y_axisvalues.GetValue((y_axisvalues.Length - 1) - e.RowHandle);
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
                    LogHelper.Log(E.Message);
                }
            }
        }

        private void ctrlCompressorMap1_onRefreshData(object sender, EventArgs e)
        {
            if (sender is ctrlCompressorMap)
            {
                // refresh the entire airmass viewer for data in the binary might have changed
                Calculate(m_currentfile, m_symbols);
                if (gridControl1.DataSource != null)
                {
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
                    ctrlCompressorMap1.Rpm_points = x_axisvalues;
                    PartNumberConverter pnc = new PartNumberConverter();
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
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG images|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                chartControl1.ExportToImage(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
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
