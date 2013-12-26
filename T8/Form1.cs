/*
ECUIDCal.ApplicationFileName

FA5B_C_FMEP_46_FIE_82d
<swversion>_<variant>_<programmerid>_<calibrationversion>_<calibrator_id>_<enginevariantcode>

<softwareversion>
1st digit
F=T8/L850T
G=T9/SVC
H=T9/SCC
2nd digit (major variant or model year)
digit3&4 is updated every release

<variant>
C = closed (calibration data in ROM)
O = Open (calibration data in RAM)

<programmer id>
FMEP = official release
Other = test version = f.e. 1EPA = Torkel Wahlberg (FMEPA)

<calibration version>
increasing number - hex

<calibrator id>
EBD = Peder Nordenback (FMEBD)
FME = Official release

<engine variant code>
?
  
Saab

FC0J_C_FMEP_63_FIE_80f = MY2008 1.8t 150bhp manual
FC0J_C_FMEP_63_FIE_82s = MY07 Aero automatic
FC0J_C_FMEP_63_FIE_81j = MY2007 Aero ?
FC0J_C_FMEP_63_FIE_82s = Aero

FA5I_C_FME2_65_FIE_82h = MY05 Aero
FA5B_C_FME4_52_FIE_81b = 1.8t
FC0U_C_FME1_14_FIE_828 = MY10 Aero
FA5B_C_FME6_54_FIE_82b = 2.0T Aero 210bhp
FA5B_C_FME4_52_FIE_82b = 2.0T Aero 210bhp Hirsch modified
FA5I_C_FME2_72_FIE_81d = B207L MY2004
 * 
 * 80 -> 1.8t (150bhp)
 * 81 -> 2.0t (175bhp)
 * 82 -> 2.0T Aero (210bhp)

Opel

FA5B_C_FME3_4M_FME_PI_83f = Opel Signum 2.0t


Opel files only have one engine limiter TrqLimCal.Trq_MaxEngineTab1 en TrqLimCal.Trq_MaxEngineTab2
No difference in Aut/Man
 * 
 * 
 * 
 * TODO: Check flashblocks for changing VIN, ALTERNATE, Use marry process in realtime
 * TODO: Check flashblocks for information that can be written using the 0x3B (writeDataByLocalID) function
 * TODO: Figure out a way to determine which flashblock is the active one (need disassembly for it)
 * TODO: implement additional canbus functions:
 *          Set top speed
 *          Set transmission type
 *          Set ... 
 * TODO: Bitmask display in mapviewer somehow (check EnableDiagCal.*)
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using T8SuitePro;
using System.Diagnostics;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using PSTaskDialog;
using Microsoft.Win32;
using DevExpress.XtraBars.Docking;
using System.Threading;
using RealtimeGraph;
using T8CANLib;
using DevExpress.Skins;

namespace T8SuitePro
{
    public delegate void DelegateShowChangeLog(Version v);
    public delegate void DelegateStartReleaseNotePanel(string filename, string version);
    public delegate void DelegateUpdateRealTimeValue(string symbolname, float value);

    public enum PanelMode : int
    {
        Day,
        Night
    }

    public enum CANBusAdapter : int
    {
        Lawicel,
        MultiAdapter,
        ELM327,
        //EasySync,
        Just4Trionic
    }

    public enum MonitorType : int
    {
        Default,
        Dashboard
    }

    public enum AFRViewType : int
    {
        AFRMode,
        LambdaMode
    }

    public partial class Form1 : Form
    {
        private string m_CurrentWorkingProject = string.Empty;
        private TrionicProjectLog m_ProjectLog = new TrionicProjectLog();
        public DelegateUpdateRealTimeValue m_DelegateUpdateRealTimeValue;
        private bool m_WriteLogMarker = false;
        private string m_currentsramfile = string.Empty;

        private T8CANLib.T8Can t8can = new T8CANLib.T8Can();
        System.Data.DataTable m_realtimeAddresses;
        private Stopwatch _sw = new Stopwatch();
        private EngineStatus _currentEngineStatus = new EngineStatus();
        private AFRViewType AfrViewMode = AFRViewType.AFRMode;

        string m_currentfile = string.Empty;
        AppSettings m_appSettings = new AppSettings();
        SymbolCollection m_symbols = new SymbolCollection();
        int m_currentMapHelperRowHandle = -1;
        private frmMapHelper m_mapHelper = new frmMapHelper();
        private string m_currentMapname = string.Empty;
        private Microsoft.Office.Interop.Excel.Application xla;
        msiupdater m_msiUpdater;
        public DelegateShowChangeLog m_DelegateShowChangeLog;
        private bool _isFullScreenEnabled = false;
        private FormWindowState _oldWindowState;
        private System.Drawing.Rectangle _oldDesktopBounds;
        private System.Drawing.Size _oldClientSize;
        System.Data.DataTable resumeTuning = new System.Data.DataTable();
        private bool m_startFromCommandLine = false;
        private string m_commandLineFile = string.Empty;
        public DelegateStartReleaseNotePanel m_DelegateStartReleaseNotePanel;
        private frmProgress frmProgressLogWorks;


        public Form1(string[] args)
        {
            frmSplash splash = new frmSplash();
            splash.Show();
            System.Windows.Forms.Application.DoEvents();
            InitializeComponent();

            try
            {
                m_DelegateShowChangeLog = new DelegateShowChangeLog(this.ShowChangeLog);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

            try
            {
                RegistryKey TempKeyCM = null;
                TempKeyCM = Registry.ClassesRoot.CreateSubKey(@"SystemFileAssociations\.bin\shell\Edit in T8 Suite\command");
                string StartKey = System.Windows.Forms.Application.ExecutablePath + " \"%1\"";
                TempKeyCM.SetValue("", StartKey);
                TempKeyCM.Close();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            try
            {
                RegistryKey TempKeyCM = null;
                TempKeyCM = Registry.ClassesRoot.CreateSubKey(@"SystemFileAssociations\.bin\shell\Auto detect Trionic file type\command");
                string StartKey = System.Windows.Forms.Application.StartupPath + "\\SuiteLauncher.exe" + " \"%1\"";
                TempKeyCM.SetValue("", StartKey);
                TempKeyCM.Close();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            if (args.Length > 0)
            {
                if (args[0].ToString().ToUpper().EndsWith(".BIN"))
                {
                    if (File.Exists(args[0].ToString()))
                    {
                        m_startFromCommandLine = true;
                        m_commandLineFile = args[0].ToString();
                    }
                }
            }
            try
            {
                m_DelegateStartReleaseNotePanel = new DelegateStartReleaseNotePanel(this.StartReleaseNotesViewer);
                m_DelegateUpdateRealTimeValue = new DelegateUpdateRealTimeValue(this.UpdateRealtimeInformationValue);

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

            try
            {
                sndplayer = new System.Media.SoundPlayer();
                t8can.onReadProgress += new T8CANLib.T8Can.ReadProgress(t8can_onReadProgress);
                t8can.onWriteProgress += new T8CANLib.T8Can.WriteProgress(t8can_onWriteProgress);
                t8can.onCanInfo += new T8CANLib.T8Can.CanInfo(t8can_onCanInfo);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }


            splash.Close();
        }

        void t8can_onWriteProgress(object sender, T8CANLib.T8Can.WriteProgressEventArgs e)
        {
            SetProgress("Sending " + e.Percentage.ToString("F2") + " %");
            SetProgressPercentage(Convert.ToInt32(e.Percentage));
        }


        void t8can_onCanInfo(object sender, T8CANLib.T8Can.CanInfoEventArgs e)
        {
            SetProgress(e.Info);
        }

        void t8can_onReadProgress(object sender, T8CANLib.T8Can.ReadProgressEventArgs e)
        {
            SetProgress("Downloading " + e.Percentage.ToString("F2") + " %");
            SetProgressPercentage(Convert.ToInt32(e.Percentage));
        }

        private void UpdateRealtimeInformationValue(string symbolname, float value)
        {
            switch (symbolname)
            {
                case "ActualIn.n_Engine": // rpm
                    //if (m_appSettings.DebugMode) value = 2500; //TODO: <GS-25012011> REMOVE AFTER TESTING
                    digitalDisplayControl1.DigitText = value.ToString("F0");
                    _currentEngineStatus.CurrentRPM = value;
                    break;
                case "ActualIn.T_Engine": // engine temp
                    //if (m_appSettings.DebugMode) value = 85; //TODO: <GS-25012011> REMOVE AFTER TESTING
                    digitalDisplayControl2.DigitText = value.ToString("F0");
                    _currentEngineStatus.CurrentEngineTemp = value;
                    break;
                case "ActualIn.T_AirInlet": // IAT temp
                    digitalDisplayControl3.DigitText = value.ToString("F0");
                    _currentEngineStatus.CurrentIAT = value;
                    //_currentIAT = value;
                    break;
                case "ECMStat.ST_ActiveAirDem": // Airmass limiter ID
                    digitalDisplayControl5.DigitText = value.ToString("F0");
                    //TODO: Adjust for Trionic 8
                    labelControl9.Text = ConvertActiveAirDemand(Convert.ToInt32(value));
                    _currentEngineStatus.CurrentAirmassLimiterID = Convert.ToInt32(value);
                    break;
                case "Lambda.Status": // Lambda status
                    //digitalDisplayControl5.DigitText = value.ToString("F0");
                    labelControl10.Text = ConvertLambdaStatus(Convert.ToInt32(value));
                    //TODO: Adjust for Trionic 8
                    _currentEngineStatus.CurrentLambdaStatus = Convert.ToInt32(value);
                    break;
                case "FCut.CutStatus": // Fuelcut status
                    labelControl12.Text = ConvertFuelcutStatus(Convert.ToInt32(value));
                    //TODO: Adjust for Trionic 8
                    _currentEngineStatus.CurrentFuelcutStatus = Convert.ToInt32(value);
                    break;
                case "IgnMastProt.fi_Offset": // Ioff
                    measurementIgnitionOffset.Value = value;
                    _currentEngineStatus.CurrentIgnitionOffset = value;
                    //TODO: <GS-18102010> ignition offset als knock indicator gebruiken!?
                    UpdateKnockIndicator(value);

                    break;
                case "AirMassMast.m_Request": // drivers airmass request
                    measurementAirmassRequest.Value = value;
                    _currentEngineStatus.CurrentAirmassRequest = value;
                    break;
                case "Out.M_EngTrqAct": // calc. torque
                    measurementCalculatedTorque.Value = value;
                    _currentEngineStatus.CurrentEngineTorque = value;
                    // update Power as well <GS-05042011>
                    //Power (HP) = (Torque (Nm) * rpm) / 7121
                    _currentEngineStatus.CurrentEnginePower = _currentEngineStatus.CurrentRPM * _currentEngineStatus.CurrentEngineTorque / 7121;
                    measurementCalculatedPower.Value = _currentEngineStatus.CurrentEnginePower;
                    break;
                case "ECMStat.P_Engine": // calc. power
                    measurementCalculatedPower.Value = value;
                    _currentEngineStatus.CurrentEnginePower = value;
                    break;
                case "ECMStat.p_Diff": // pressure diff = boost
                case "In.p_AirInlet": // pressure diff = boost
                    measurementBoost.Value = value;
                    _currentEngineStatus.CurrentBoostPressure = value;
                    break;
                case "Out.PWM_BoostCntrl": // duty cycle BCV
                    measurementDutyCycleBCV.Value = value;
                    _currentEngineStatus.CurrentBoostValvePWM = value;
                    break;
                case "Out.fi_Ignition": // Ignition advance
                    measurementIgnitionAdvance.Value = value;
                    _currentEngineStatus.CurrentIgnitionAdvance = value;
                    break;
                case "Out.X_AccPos": // TPS
                    measurementPedalPosition.Value = value;
                    _currentEngineStatus.CurrentThrottlePosition = value;
                    break;
                case "MAF.m_AirInlet": // actual airmass
                    //if (m_appSettings.DebugMode) value = 500; //TODO: <GS-25012011> REMOVE AFTER TESTING
                    linearGauge1.Value = value;
                    _currentEngineStatus.CurrentAirmassPerCombustion = value;
                    break;
                case "DisplProt.LambdaScanner": // AFR through wideband?
                case "DisplProt.AD_Scanner": // AFR through wideband?
                    if (m_appSettings.UseWidebandLambda)
                    {
                        if (symbolname == "DisplProt.AD_Scanner")
                        {
                            // first convert the value to AFR
                            // value to volt first
                            //value = value / 204.6F;
                            //Console.WriteLine("1: " + value.ToString());
                            value = (float)ConvertToWidebandAFR(value);
                            //if (m_appSettings.DebugMode) value = 13; //TODO: <GS-25012011> REMOVE AFTER TESTING
                            //Console.WriteLine("2: " + value.ToString());
                        }

                        float valTextLambda = value / 14.7F;
                        digitalDisplayControl6.DigitText = value.ToString("F1");
                        if (AfrViewMode == AFRViewType.AFRMode)
                        {
                            // value must have been divided by 100 already, so 0.75-1.25 here
                            //value *= 14.7F;
                            linearGauge2.Value = value;
                        }
                        else
                        {
                            // lambda mode
                            linearGauge2.Value = valTextLambda;
                        }
                        if (m_appSettings.MeasureAFRInLambda)
                        {
                            //LogWidebandAFR(value / 14.7F, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        }
                        else
                        {
                            //LogWidebandAFR(value, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        }
                        //ProcessAutoTuning(value, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                    }
                    //_currentEngineStatus.CurrentAFR = value;
                    break;
                case "Lambda.LambdaInt": // AFR through narrowband?
                    if (!m_appSettings.UseWidebandLambda)
                    {
                        float valText = value * 14.7F;
                        digitalDisplayControl6.DigitText = value.ToString("F1");
                        if (AfrViewMode == AFRViewType.AFRMode)
                        {
                            // value must have been divided by 100 already, so 0.75-1.25 here
                            value *= 14.7F;
                            linearGauge2.Value = value;
                        }
                        else
                        {
                            // lambda mode
                            linearGauge2.Value = value;
                        }
                    }
                    //_currentEngineStatus.CurrentAFR = value;
                    break;
                case "In.v_Vehicle":
                    measurementSpeed.Value = value;
                    _currentEngineStatus.CurrentVehicleSpeed = value;
                    break;
                case "Exhaust.T_Calc":
                    measurementEGT.DigitText = value.ToString("F0");
                    _currentEngineStatus.CurrentEGT = value;
                    break;
                case "BFuelProt.CurrentFuelCon":
                    digitalDisplayControl4.DigitText = value.ToString("F1");
                    _currentEngineStatus.CurrentConsumption = value;
                    break;
                case "FPSCounter":
                    //label1.Text = value.ToString("F1") + " fps";
                    dockRealtime.Text = "Realtime panel [" + value.ToString("F1") + " fps]";
                    break;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private double ConvertToWidebandAFR(float value)
        {
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double retval = 0;
            double voltage = ((value) / 1023) * (m_appSettings.WidebandHighVoltage / 1000 - m_appSettings.WidebandLowVoltage / 1000);
            //Console.WriteLine("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_appSettings.WidebandLowVoltage / 1000) voltage = m_appSettings.WidebandLowVoltage / 1000;
            if (voltage > m_appSettings.WidebandHighVoltage / 1000) voltage = m_appSettings.WidebandHighVoltage / 1000;
            //Console.WriteLine("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_appSettings.WidebandHighAFR / 1000) - (m_appSettings.WidebandLowAFR / 1000)) / ((m_appSettings.WidebandHighVoltage / 1000) - (m_appSettings.WidebandLowVoltage / 1000));
            //Console.WriteLine("Steepness: " + steepness.ToString());
            retval = (m_appSettings.WidebandLowAFR / 1000) + (steepness * (voltage - (m_appSettings.WidebandLowVoltage / 1000)));
            //Console.WriteLine("retval: " + retval.ToString());
            return retval;

        }

        private void UpdateKnockIndicator(float value)
        {
            try
            {
                int ivalue = Convert.ToInt32(value);
                // range -20 / +20
                // multiply with 12
                ivalue *= 12;
                if (ivalue < -255) ivalue = -255;
                if (ivalue > 255) ivalue = 255;
                Color c;
                if (ivalue < 0) // red
                {
                    ivalue = -ivalue;
                    c = Color.FromArgb(ivalue, 0, 0);
                }
                else
                {
                    c = Color.Black;
                    if (m_appSettings.Panelmode == PanelMode.Night) c = Color.FromArgb(234, 77, 0);
                    //c = Color.FromArgb(0, ivalue, 0);
                }
                // set the indicator
                measurementIgnitionOffset.SetDigitColor = c;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }


        private void StartReleaseNotesViewer(string xmlfilename, string version)
        {
            dockManager1.BeginUpdate();
            DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
            dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
            dp.Tag = xmlfilename;
            ctrlReleaseNotes mv = new ctrlReleaseNotes();
            mv.LoadXML(xmlfilename);
            mv.Dock = DockStyle.Fill;
            dp.Width = 500;
            dp.Text = "Release notes: " + version;
            dp.Controls.Add(mv);
            dockManager1.EndUpdate();
        }

        void dockPanel_ClosedPanel(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        {
            // force close of the file that the hexviewer had open!
            if (sender is DockPanel)
            {
                DockPanel pnl = (DockPanel)sender;

                foreach (Control c in pnl.Controls)
                {
                    if (c is HexViewer)
                    {
                        HexViewer vwr = (HexViewer)c;
                        vwr.CloseFile();
                        //UpdateChecksum(m_currentfile);
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is HexViewer)
                            {
                                HexViewer vwr2 = (HexViewer)c2;
                                vwr2.CloseFile();
                                //UpdateChecksum(m_currentfile);
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                    {
                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is HexViewer)
                            {
                                HexViewer vwr3 = (HexViewer)c3;
                                vwr3.CloseFile();
                                //UpdateChecksum(m_currentfile);
                            }
                        }
                    }
                }
                // remove the panel from the dockmanager
                dockManager1.RemovePanel(pnl);
            }
        }

        private void LoadLayoutFiles()
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\SymbolViewLayout.xml"))
                {
                    gridViewSymbols.RestoreLayoutFromXml(System.Windows.Forms.Application.StartupPath + "\\SymbolViewLayout.xml");
                }
            }
            catch (Exception E1)
            {
                Console.WriteLine(E1.Message);
            }
        }

        private void SaveLayoutFiles()
        {
            try
            {
                gridViewSymbols.SaveLayoutToXml(System.Windows.Forms.Application.StartupPath + "\\SymbolViewLayout.xml");
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void TryToByteFlipFile(string filename)
        {
            // read the first few bytes and decide!
            // normally 00 10 0C 00... if it is 10 00 00 0C flip the file
            byte b1 = 0;
            byte b2 = 0;
            byte b3 = 0;
            byte b4 = 0;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fs))
            {
                b1 = br.ReadByte();
                b2 = br.ReadByte();
                b3 = br.ReadByte();
                b4 = br.ReadByte();
            }
            fs.Close();
            if (b1 == 0x10 && b2 == 0x00 && b3 == 0x00 && b4 == 0x0C)
            {
                Console.WriteLine("Flipping file");
                string filenamenew = filename + ".tmp";
                FileStream fromfile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                FileStream tofile = new FileStream(filenamenew, FileMode.CreateNew);
                using (BinaryReader brori = new BinaryReader(fromfile))
                {
                    using (BinaryWriter brnew = new BinaryWriter(tofile))
                    {
                        for (int t = 0; t < 0x100000; t += 2)
                        {
                            byte bfirst = brori.ReadByte();
                            byte bsecond = brori.ReadByte();
                            brnew.Write(bsecond);
                            brnew.Write(bfirst);
                        }
                    }
                }
                fromfile.Close();
                tofile.Close();
                CreateBinaryBackup(filename, false);
                File.Delete(filename);
                File.Copy(filenamenew, filename);
                File.Delete(filenamenew);
            }
        }

        private bool ValidateTrionic8File(string filename)
        {
            byte[] testdata = readdatafromfile(filename, 0, 0x10);
            if (testdata[0] == 0x00 && (testdata[1] == 0x10 || testdata[1] == 0x00) && testdata[2] == 0x0C && testdata[3] == 0x00)
            {
                return true;
            }
            return false;
        }

        private void TryToOpenFile(string filename, out SymbolCollection symbol_collection, int filename_size)
        {
            SymbolTranslator translator = new SymbolTranslator();
            string help = string.Empty;
            bool symbolsLoaded = false;
            m_currentsramfile = string.Empty; // geen sramfile erbij
            barStaticItem1.Caption = "";
            barFilenameText.Caption = "";
            bool _hideRealtime = false;
            //XDFCategories category = XDFCategories.Undocumented;
            //XDFSubCategory subcat = XDFSubCategory.Undocumented;
            symbol_collection = new SymbolCollection();
            // check file for real only attributes
            if (filename == string.Empty) return;
            FileInfo fi = new FileInfo(filename);
            fi.IsReadOnly = false;
            if (fi.Length != 0x100000)
            {
                m_currentfile = string.Empty;
                MessageBox.Show("File has incorrect length: " + Path.GetFileName(filename));
                return;
            }
            if (!ValidateTrionic8File(filename))
            {
                TryToByteFlipFile(filename);
                if (!ValidateTrionic8File(filename))
                {
                    m_currentfile = string.Empty;
                    MessageBox.Show("File does not seem to be a Trionic 8 file: " + Path.GetFileName(filename));
                    return;
                }
            }

            try
            {
                //TODO: if the file is byte-swapped, fix that first!
                TryToByteFlipFile(filename);

                SetProgress("Opening " + Path.GetFileName(filename));
                SetProgressPercentage(0);

                System.Windows.Forms.Application.DoEvents();

                int sym_count = 0;
                if (filename != string.Empty)
                {
                    if (File.Exists(filename))
                    {
                        TryToExtractPackedBinary(filename, sym_count, filename_size, out symbol_collection);
                    }
                    // try to load additional symboltranslations that the user entered
                    symbolsLoaded = TryToLoadAdditionalSymbols(filename, true, symbol_collection);

                }
            }
            catch (Exception E)
            {
                Console.WriteLine("TryOpenFile filed: " + filename + " err: " + E.Message);
            }
            foreach (SymbolHelper sh in /*m_symbols*/ symbol_collection)
            {
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        Console.WriteLine("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
                if (sh.Varname.StartsWith("Symbolnumber"))
                {
                    // do not allow realtime interaction with this type of files loaded (no symboltable)
                    _hideRealtime = true;
                }
            }
            try
            {
                if (m_appSettings.MapDetectionActive && !symbolsLoaded)
                {
                    SymbolFiller sf = new SymbolFiller();
                    sf.CheckAndFillCollection(/*m_symbols*/ symbol_collection);
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

            _hideRealtime = false;

            if (_hideRealtime)
            {
                btnToggleRealtime.Enabled = false;
                btnSRAMSnapshot.Enabled = false;
                btnGetECUInfo.Enabled = false;
                btnGetFlashContent.Enabled = false;
                btnWriteLogMarker.Enabled = false;
            }
            else
            {
                btnToggleRealtime.Enabled = true;
                btnSRAMSnapshot.Enabled = true;
                btnGetECUInfo.Enabled = true;
                btnGetFlashContent.Enabled = true;
                btnWriteLogMarker.Enabled = true;
            }
            SetProgress("Loading data into view... ");
            SetProgressPercentage(95);
            System.Windows.Forms.Application.DoEvents();

            //this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
            //barFilenameText.Caption = Path.GetFileName(m_currentfile);
            //gridControlSymbols.DataSource = m_symbols;
            //m_appSettings.Lastfilename = m_currentfile;

            //gridViewSymbols.BestFitColumns();

            SetDefaultFilters();

            SetProgressIdle();
            //UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);
            if (m_currentfile != string.Empty) LoadRealtimeTable();
        }

        private void SetProgressIdle()
        {
            SetProgress("Idle");
            SetProgressPercentage(0);
        }
        private void SetProgressPercentage(int perc)
        {
            try
            {
                if (Convert.ToInt32(barProgress.EditValue) != perc)
                {
                    barProgress.EditValue = perc;
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void SetProgress(string descr)
        {
            if (barProgress.Caption != descr)
            {
                barProgress.Caption = descr;
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void SetDefaultFilters()
        {
            DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(@"([Flash_start_address] > 0 AND [Flash_start_address] < 1048576)", "Only symbols within binary");
            gridViewSymbols.ActiveFilter.Clear();
            gridViewSymbols.ActiveFilter.Add(gcSymbolsAddress, fltr);
            /*** set filter ***/
            gridViewSymbols.ActiveFilterEnabled = true;
        }

        private int GetAddrTableOffset(string filename)
        {
            int addrtaboffset = GetStartOfAddressTableOffset(filename);
            Console.WriteLine("addrtaboffset: " + addrtaboffset.ToString("X8"));
            //int NqNqNqOffset = GetNqNqNqStringFromOffset(addrtaboffset - 0x100, filename);
            int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
            Console.WriteLine("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            Console.WriteLine("symbtaboffset: " + symbtaboffset.ToString("X8"));
            //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
            //                    symbtaboffset = NqNqNqOffset;
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
            int retval = NqNqNqOffset + 21;
            Console.WriteLine("symbtablength: " + symbtablength.ToString("X8"));
            return retval;
        }

        private int GetAddrTableOffsetBySymbolTable(string filename)
        {
            int addrtaboffset = GetEndOfSymbolTable(filename);
            Console.WriteLine("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
            //int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
            int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
            Console.WriteLine("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            Console.WriteLine("symbtaboffset: " + symbtaboffset.ToString("X8"));
            //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
            //                    symbtaboffset = NqNqNqOffset;
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
            int retval = NqNqNqOffset + 21;
            Console.WriteLine("symbtablength: " + symbtablength.ToString("X8"));
            return retval;
        }


        private bool TryToExtractPackedBinary(string filename, int sym_count, int filename_size, out SymbolCollection symbol_collection)
        {
            bool retval = true;
            //Test 15092009
            //int RealAddressTableOffset = GetAddrTableOffset(filename) + 7; // was 17
            int RealAddressTableOffset = GetAddrTableOffsetBySymbolTable(filename) + 7; // was 17 // <GS-22032010>

            //Test 15092009
            Console.WriteLine("Real symboltable offset: " + RealAddressTableOffset.ToString("X8"));

            int symboltableoffset = 0;
            symbol_collection = new SymbolCollection();
            SetProgress("Unpacking file... ");
            SetProgressPercentage(5);
            System.Windows.Forms.Application.DoEvents();

            bool compr_created = UnpackFileUsingDecode(filename, out symboltableoffset);
            sym_count = 0;
            SetProgress("Finding address table... ");
            SetProgressPercentage(15);
            System.Windows.Forms.Application.DoEvents();
            byte[] searchsequence = new byte[9];
            searchsequence.SetValue((byte)0x00, 0);
            searchsequence.SetValue((byte)0x00, 1);
            searchsequence.SetValue((byte)0x00, 2);
            searchsequence.SetValue((byte)0x00, 3);
            searchsequence.SetValue((byte)0x00, 4);
            searchsequence.SetValue((byte)0x00, 5);
            searchsequence.SetValue((byte)0x00, 6);
            searchsequence.SetValue((byte)0x00, 7);
            searchsequence.SetValue((byte)0x20, 8);
            int AddressTableOffset = 0;//GetAddressTableOffset(searchsequence);
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < filename_size) && (AddressTableOffset == 0))
                {
                    byte adrb = br.ReadByte();
                    switch (adr_state)
                    {
                        case 0:
                            if (adrb == (byte)searchsequence.GetValue(0))
                            {

                                adr_state++;
                            }
                            break;
                        case 1:
                            if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                            }
                            break;
                        case 8:
                            if (adrb == (byte)searchsequence.GetValue(8))
                            {
                                // found it
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                            }
                            break;
                    }
                }
                if (AddressTableOffset > 0)
                {
                    AddressTableOffset = RealAddressTableOffset; // TEST 15092009
                    SetProgress("Reading address table... ");
                    SetProgressPercentage(25);
                    System.Windows.Forms.Application.DoEvents();

                    fsread.Seek(AddressTableOffset - 17, SeekOrigin.Begin);
                    bool endoftable = false;
                    Int64 internal_address = 0;
                    int sramaddress = 0;
                    int symbollength = 0;
                    int bitmask = 0;
                    int symb_count = 0;
                    symbol_collection = new SymbolCollection();
                    while (!endoftable)
                    {
                        // steeds 10 karaketers
                        try
                        {
                            byte[] bytes = br.ReadBytes(10);
                            if (bytes.Length == 10)
                            {
                                // DumpBytesToConsole(bytes);
                                if ((Convert.ToInt32(bytes.GetValue(9)) != 0x00) /*|| (Convert.ToInt32(bytes.GetValue(6)) != 0x00)*/)
                                {
                                    endoftable = true;
                                    //MessageBox.Show("EOT: " + fsread.Position.ToString("X6"));
                                    Console.WriteLine("EOT: " + fsread.Position.ToString("X6"));
                                }
                                else
                                {
                                    //DumpBytesToConsole(bytes);

                                    internal_address = Convert.ToInt64(bytes.GetValue(0)) * 256 * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(1)) * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(2));

                                    /* if (bytes[1] == 0x7A && bytes[2] == 0xEE)
                                     {
                                         Console.WriteLine("suspicious");

                                         if (internal_address == 0x7AEE)
                                         {
                                             Console.WriteLine("break: " + fsread.Position.ToString("X8"));
                                         }
                                     }*/
                                    symbollength = Convert.ToInt32(bytes.GetValue(3)) * 256;
                                    symbollength += Convert.ToInt32(bytes.GetValue(4));

                                    bitmask = Convert.ToInt32(bytes.GetValue(5)) * 256;
                                    bitmask += Convert.ToInt32(bytes.GetValue(6));


                                    //                                                sramaddress = Convert.ToInt32(bytes.GetValue(7)) * 256 * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(8)) * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(9));
                                    SymbolHelper sh = new SymbolHelper();
                                    sh.Symbol_type = Convert.ToInt32(bytes.GetValue(7));
                                    sh.Varname = "Symbolnumber " + symbol_collection.Count.ToString();
                                    sh.Symbol_number = symbol_collection.Count;
                                    sh.Symbol_number_ECU = symbol_collection.Count;
                                    sh.Flash_start_address = internal_address;
                                    sh.Start_address = internal_address;
                                    sh.Length = symbollength;
                                    sh.BitMask = bitmask;
                                    /*if (internal_address == 0x0AE956)
                                    {
                                        DumpBytesToConsole(bytes);
                                    }*/
                                    //DumpToSymbolFile(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));

                                    symbol_collection.Add(sh);
                                    symb_count++;
                                    if (symb_count % 500 == 0)
                                    {
                                        SetProgress("Symbol: " + sh.Varname);
                                        SetProgressPercentage(5);
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }
                            }
                            else
                            {
                                endoftable = true;
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                            retval = false;
                        }

                    }
                    if (compr_created)
                    {
                        Console.WriteLine("Dumping addresstable");
                        SetProgress("Creating symbol list... ");
                        SetProgressPercentage(50);
                        System.Windows.Forms.Application.DoEvents();

                        DumpSymbolAddressTable(symbol_collection);
                        Console.WriteLine("Decoding packed symbol table");
                        // run decode.exe

                        if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR.TXT");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\COMPR"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\table.tmp"))
                        {
                            File.Delete(Path.GetTempPath() + "\\table.tmp");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\T8SuitePro.decode.exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\T8SuitePro.decode.exe");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe"))
                        {
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe", Path.GetTempPath() + "\\T8SuitePro.decode.exe");
                        }

                        // <GS-18062012> support for x64
                        bool x64 = Detectx64Architecture();
                        if (x64)
                        {
                            mRecreateAllExecutableResources();

                            // rename T7.decode.exe to decode.exe
                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\decode.exe"))
                            {
                                File.Delete(System.Windows.Forms.Application.StartupPath + "\\decode.exe");
                            }
                            File.Move(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe", System.Windows.Forms.Application.StartupPath + "\\decode.exe");

                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\decode.exe"))
                            {
                                if (File.Exists(Path.GetTempPath() + "\\decode.exe"))
                                {
                                    File.Delete(Path.GetTempPath() + "\\decode.exe");
                                }
                                File.Copy(System.Windows.Forms.Application.StartupPath + "\\decode.exe", Path.GetTempPath() + "\\decode.exe");
                            }
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\COMPR", Path.GetTempPath() + "\\COMPR");
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\table.tmp", Path.GetTempPath() + "\\table.tmp");

                            string DosBoxPath = Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "DOSBox-0.74");  
                            // write a dosbox.conf first
                            string confFile = Path.Combine(DosBoxPath, "t8dosb.conf");
                            if (!File.Exists(confFile))
                            {
                                using (StreamWriter sw = new StreamWriter(confFile, false))
                                {
                                    sw.WriteLine("[autoexec]");
                                    sw.WriteLine("cycles=max");
                                    sw.WriteLine("mount c \"" + Path.GetTempPath() + "\"");
                                    sw.WriteLine("c:");
                                    sw.WriteLine("decode.exe");
                                    sw.WriteLine("exit");
                                }
                            }

                            string Exename = Path.Combine(DosBoxPath, "DOSBox.exe");
                            ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
                            startinfo.CreateNoWindow = true; // TRUE
                            startinfo.WindowStyle = ProcessWindowStyle.Hidden; // hidden
                            startinfo.Arguments = "-noconsole -conf t8dosb.conf";
                            startinfo.WorkingDirectory = DosBoxPath;
                            System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                            conv_proc.WaitForExit(30000);

                            if (!conv_proc.HasExited)
                            {
                                conv_proc.Kill();
                                retval = false;
                            }
                            else
                            {
                                // nu door compr.txt lopen
                                Console.WriteLine("Adding names to symbols");
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                                {
                                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                                }
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP"))
                                {
                                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP");
                                }
                                if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                                {
                                    File.Copy(Path.GetTempPath() + "\\COMPR.TXT", System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                                }
                                if (File.Exists(Path.GetTempPath() + "\\XTABLE.TMP"))
                                {
                                    File.Copy(Path.GetTempPath() + "\\XTABLE.TMP", System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP");
                                }
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                                {
                                    AddNamesToSymbolsFromTableTmp(symbol_collection);
                                }
                            }
                        }
                        else
                        {
                            mRecreateAllExecutableResources();
                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe"))
                            {
                                if (File.Exists(Path.GetTempPath() + "\\T8SuitePro.decode.exe")) File.Delete(Path.GetTempPath() + "\\T8SuitePro.decode.exe");
                                File.Copy(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe", Path.GetTempPath() + "\\T8SuitePro.decode.exe");
                            }
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\COMPR", Path.GetTempPath() + "\\COMPR");
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\table.tmp", Path.GetTempPath() + "\\table.tmp");
                            string Exename = Path.Combine(Path.GetTempPath(), "T8SuitePro.decode.exe");

                            ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
                            startinfo.CreateNoWindow = true;
                            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                            startinfo.WorkingDirectory = Path.GetTempPath();
                            System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                            conv_proc.WaitForExit(10000); // wait for 10 seconds max
                            if (!conv_proc.HasExited)
                            {
                                conv_proc.Kill();
                                retval = false;
                            }
                            else
                            {
                                // nu door compr.txt lopen
                                SetProgress("Arranging symbol list... ");
                                SetProgressPercentage(70);
                                System.Windows.Forms.Application.DoEvents();

                                Console.WriteLine("Adding names to symbols");
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                                {
                                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                                }
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP"))
                                {
                                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP");
                                }
                                if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                                {
                                    File.Copy(Path.GetTempPath() + "\\COMPR.TXT", System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                                }
                                if (File.Exists(Path.GetTempPath() + "\\XTABLE.TMP"))
                                {
                                    File.Copy(Path.GetTempPath() + "\\XTABLE.TMP", System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP");
                                }
                                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                                {
                                    SetProgress("Finalizing list... ");
                                    SetProgressPercentage(85);
                                    System.Windows.Forms.Application.DoEvents();
                                    AddNamesToSymbolsFromTableTmp(symbol_collection);
                                }
                            }
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\T8SuitePro.decode.exe");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\T8SuitePro.decode.exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\T8SuitePro.decode.exe");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not find address table!");
                    retval = false;
                }
            }
            return retval;
        }

        private bool Detectx64Architecture()
        {
            try
            {
                // if x64, install dosbox if not already done
                if (Wow.Is64BitOperatingSystem)
                {
                    // 64 bit detected... 
                    AppSettings m_appSettings = new AppSettings();
                    if (!m_appSettings.DosBoxInstalled)
                    {
                        //DOSBox0.74-win32-installer.exe /S
                        string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Dosbox", "DOSBox0.74-win32-installer.exe");
                        ProcessStartInfo startinfo = new ProcessStartInfo(fileName);
                        startinfo.CreateNoWindow = true;
                        startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startinfo.Arguments = "/S";
                        startinfo.WorkingDirectory = /*Application.StartupPath*/Path.GetTempPath();
                        System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                        conv_proc.WaitForExit(); // wait for 10 seconds max
                        m_appSettings.DosBoxInstalled = true;
                    }
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        private void DumpToSymbolFile(string p)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Documents and Settings\Guido.MOBICOACH\My Documents\Prive\SAAB\Trionic 8\Dev\SymbolAddressIssue\Addresses.txt", true))
            {
                sw.WriteLine(p);
            }
        }

        //======================================================
        //Recreate all executable resources
        //======================================================
        private void mRecreateAllExecutableResources()
        {
            // Get Current Assembly refrence
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all imbedded resources
            string[] arrResources = currentAssembly.GetManifestResourceNames();

            foreach (string resourceName in arrResources)
            {
                if (resourceName.EndsWith(".exe"))
                { //or other extension desired
                    //Name of the file saved on disk
                    string saveAsName = resourceName;
                    FileInfo fileInfoOutputFile = new FileInfo(System.Windows.Forms.Application.StartupPath + "\\" + saveAsName);
                    //CHECK IF FILE EXISTS AND DO SOMETHING DEPENDING ON YOUR NEEDS
                    if (fileInfoOutputFile.Exists)
                    {
                        //overwrite if desired  (depending on your needs)
                        //fileInfoOutputFile.Delete();
                    }
                    //OPEN NEWLY CREATING FILE FOR WRITTING
                    FileStream streamToOutputFile = fileInfoOutputFile.OpenWrite();
                    //GET THE STREAM TO THE RESOURCES
                    Stream streamToResourceFile =
                                        currentAssembly.GetManifestResourceStream(resourceName);

                    //---------------------------------
                    //SAVE TO DISK OPERATION
                    //---------------------------------
                    const int size = 4096;
                    byte[] bytes = new byte[4096];
                    int numBytes;
                    while ((numBytes = streamToResourceFile.Read(bytes, 0, size)) > 0)
                    {
                        streamToOutputFile.Write(bytes, 0, numBytes);
                    }

                    streamToOutputFile.Close();
                    streamToResourceFile.Close();
                }//end_if

            }//end_foreach
        }//end_mRecreateAllExecutableResources 

        private void DumpBytesToConsole(byte[] bytes)
        {
            string line = "symbol bytes: ";
            foreach (byte b in bytes)
            {
                line += b.ToString("X2") + " ";
            }
            Console.WriteLine(line);
        }
        private void DumpSymbolAddressTable(SymbolCollection symbol_collection)
        {
            // export to table.tmp
            //4538 EB0C8P1C.56O F5DB0074
            //05909C 68F5 04 

            using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\table.tmp", false))
            {

                sw.WriteLine(symbol_collection.Count.ToString() + " " + "VAGFHGFSD" + " 00000000");
                sw.WriteLine("");
                sw.WriteLine("");
                foreach (SymbolHelper sh in symbol_collection)
                {
                    if (sh.Length < 0x1000)
                    {
                        sw.WriteLine(sh.Flash_start_address.ToString("X6") + " " + sh.Length.ToString("X4") + " " + sh.Symbol_type.ToString("X2"));
                    }
                }
            }
        }
        private void AddNamesToSymbolsFromTableTmp(SymbolCollection symbol_collection)
        {
            SymbolTranslator translator = new SymbolTranslator();
            SetProgress("Loading...");
            string[] xtablelines = File.ReadAllLines(Path.Combine(Path.GetTempPath(), "XTABLE.TMP"));
            string line = string.Empty;
            int flashaddress = 0;
            int length = 0;
            char[] sep = new char[1];
            sep.SetValue(' ', 0);

            string addresses = string.Empty;
            string symbol = string.Empty;
            for (int i = 4; i < xtablelines.Length; i++)
            {
                line = xtablelines[i];
                try
                {
                    if (line.Length > 2)
                    {
                        if (!line.Contains(" "))
                        {
                            // dan is het een symbool
                            symbol = line.Trim();
                        }
                        else
                        {
                            addresses = line;
                            string[] addvalues = addresses.Split(sep);
                            if (addvalues.Length >= 3)
                            {
                                flashaddress = Convert.ToInt32((string)addvalues.GetValue(0), 16);
                                length = Convert.ToInt32((string)addvalues.GetValue(1), 16);
                                foreach (SymbolHelper sh in symbol_collection)
                                {
                                    if (sh.Flash_start_address == flashaddress)
                                    {
                                        sh.Varname = symbol;
                                        sh.Length = length;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("Failed to add symbolnames: " + E.Message);
                    Console.WriteLine("Failed to add symbolnames line: " + line);
                }
            }
            SetProgress("Combining...");

            string[] comprlines = File.ReadAllLines(Path.Combine(Path.GetTempPath(), "COMPR.TXT"));
            line = string.Empty;
            flashaddress = 0;
            length = 0;

            addresses = string.Empty;
            symbol = string.Empty;
            int idx = 0;

            for (int i = 1; i < comprlines.Length; i++)
            {
                line = comprlines[i];
                try
                {
                    symbol_collection[idx++].Varname = line;
                }
                catch (Exception E)
                {
                    Console.WriteLine("Failed to add symbolnames: " + E.Message + " line (2): " + line);
                }
            }
            SetProgress("Translating...");

            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in symbol_collection)
            {
                sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat);
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        Console.WriteLine("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
            }
        }

        private void AddNamesToSymbolsFromTableTmpOld(SymbolCollection symbol_collection)
        {
            SymbolTranslator translator = new SymbolTranslator();
            using (StreamReader sr = new StreamReader(Path.Combine(System.Windows.Forms.Application.StartupPath, "XTABLE.TMP")))
            {
                sr.ReadLine();// header
                sr.ReadLine();//
                sr.ReadLine();//
                sr.ReadLine();//15092009
                string line = string.Empty;
                int flashaddress = 0;
                int length = 0;

                string addresses = string.Empty;
                string symbol = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        if (line.Length > 2)
                        {
                            if (!line.Contains(" "))
                            {
                                // dan is het een symbool
                                symbol = line.Trim();

                            }
                            else
                            {
                                addresses = line;
                                char[] sep = new char[1];
                                sep.SetValue(' ', 0);
                                string[] addvalues = addresses.Split(sep);
                                if (addvalues.Length >= 3)
                                {
                                    flashaddress = Convert.ToInt32((string)addvalues.GetValue(0), 16);
                                    length = Convert.ToInt32((string)addvalues.GetValue(1), 16);
                                    foreach (SymbolHelper sh in symbol_collection)
                                    {
                                        if (sh.Flash_start_address == flashaddress)
                                        {
                                            sh.Varname = symbol;
                                            //sh.Flash_start_address = flashaddress;
                                            sh.Length = length;
                                            /*if (symbol.StartsWith("BstKnkCal."))
                                            {
                                                Console.WriteLine("Sym: " + symbol + " addr: " + flashaddress.ToString("X8") + " len: " + length.ToString());
                                            }
                                            if (flashaddress == 0x000ABA38)
                                            {
                                                Console.WriteLine("Address match: " + symbol);
                                            }*/
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to add symbolnames: " + E.Message + " line: " + line);
                    }
                }
            }
            using (StreamReader sr = new StreamReader(Path.Combine(System.Windows.Forms.Application.StartupPath, "COMPR.TXT")))
            {
                string line = string.Empty;
                int flashaddress = 0;
                int length = 0;

                string addresses = string.Empty;
                string symbol = string.Empty;
                int idx = 0;
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        //Console.WriteLine("Current name: " + symbol_collection[idx].Varname);
                        symbol_collection[idx++].Varname = line;

                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to add symbolnames: " + E.Message + " line (2): " + line);
                    }
                }
            }

            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in symbol_collection)
            {
                sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat);
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        Console.WriteLine("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
            }
        }

        int m_currentfile_size = 0x100000;

        private int GetStartOfAddressTableOffset(string filename)
        {
            byte[] searchsequence = new byte[9];
            searchsequence.SetValue((byte)0x00, 0);
            searchsequence.SetValue((byte)0x00, 1);
            searchsequence.SetValue((byte)0x00, 2);
            searchsequence.SetValue((byte)0x00, 3);
            searchsequence.SetValue((byte)0x00, 4);
            searchsequence.SetValue((byte)0x00, 5);
            searchsequence.SetValue((byte)0x00, 6);
            searchsequence.SetValue((byte)0x00, 7);
            searchsequence.SetValue((byte)0x20, 8);
            int symboltableoffset = 0x30000;
            int AddressTableOffset = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < 0x100000) && (AddressTableOffset == 0))
                {
                    byte adrb = br.ReadByte();
                    switch (adr_state)
                    {
                        case 0:
                            if (adrb == (byte)searchsequence.GetValue(0))
                            {

                                adr_state++;
                            }
                            break;
                        case 1:
                            if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                            }
                            break;
                        case 8:
                            /*if (fsread.Position > 0x5f900)
                            {
                                Console.WriteLine("Hola");
                            }
                            */
                            if (adrb == (byte)searchsequence.GetValue(8))
                            {
                                // found it
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                            }
                            break;
                    }

                }
            }
            fsread.Close();
            return AddressTableOffset;
        }


        private int GetEndOfSymbolTable(string filename)
        {
            byte[] searchsequence = new byte[11];
            searchsequence.SetValue((byte)0x73, 0);
            searchsequence.SetValue((byte)0x59, 1);
            searchsequence.SetValue((byte)0x4D, 2);
            searchsequence.SetValue((byte)0x42, 3);
            searchsequence.SetValue((byte)0x4F, 4);
            searchsequence.SetValue((byte)0x4C, 5);
            searchsequence.SetValue((byte)0x74, 6);
            searchsequence.SetValue((byte)0x41, 7);
            searchsequence.SetValue((byte)0x42, 8);
            searchsequence.SetValue((byte)0x4C, 9);
            searchsequence.SetValue((byte)0x45, 10);
            int symboltableoffset = 0;
            int AddressTableOffset = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < 0x100000) && (AddressTableOffset == 0))
                {
                    byte adrb = br.ReadByte();
                    switch (adr_state)
                    {
                        case 0:
                            if (adrb == (byte)searchsequence.GetValue(0))
                            {

                                adr_state++;
                            }
                            break;
                        case 1:
                            if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                            }
                            break;
                        case 8:
                            if (adrb == (byte)searchsequence.GetValue(8)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                            }
                            break;
                        case 9:
                            if (adrb == (byte)searchsequence.GetValue(9)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 9;
                            }
                            break;
                        case 10:
                            if (adrb == (byte)searchsequence.GetValue(10))
                            {
                                // found it
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 10;
                            }
                            break;
                    }

                }
            }
            fsread.Close();
            return AddressTableOffset;
        }
        private int GetFirstNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            int Nq1 = 0;
            int Nq2 = 0;
            int Nq3 = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    Nq1 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    Nq2 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    Nq3 = (int)fs.Position;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            if (Nq3 == 0) retval = Nq2;
            if (retval == 0) retval = Nq1;
            return retval;
        }

        private int GetLastNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            int Nq1 = 0;
            int Nq2 = 0;
            int Nq3 = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    Nq1 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    Nq2 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    Nq3 = (int)fs.Position;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            if (Nq3 == 0) retval = Nq2;
            if (retval == 0) retval = Nq1;
            return retval;
        }

        private int GetNqNqNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return retval;
        }

        private int GetAddressFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        retval = Convert.ToInt32(br.ReadByte()) * 256 * 256 * 256;
                        retval += Convert.ToInt32(br.ReadByte()) * 256 * 256;
                        retval += Convert.ToInt32(br.ReadByte()) * 256;
                        retval += Convert.ToInt32(br.ReadByte());
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to retrieve address from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return retval;
        }


        private int GetLengthFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        retval = Convert.ToInt32(br.ReadByte()) * 256;
                        retval += Convert.ToInt32(br.ReadByte());
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to retrieve length from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return retval;
        }

        public bool UnpackFileUsingDecode(string filename, out int symboltableoffset)
        {
            Int64 UnpackedLength = 0;
            symboltableoffset = 0;
            int len = 0;
            int val = 0;
            //int idx = ReadEndMarker(0x9B);
            try
            {
                //int idx = ReadMarkerAddressContent(filename, 0x9B, out len, out val);
                //if (idx > 0)
                {
                    /* if (val > m_currentfile_size)
                     {
                         // try to find the addresstable offset
                         int addrtaboffset = GetStartOfAddressTableOffset(filename);
                         //TODO: Finish for abused packed files!
                     }*/
                    // FAILSAFE for some files that seem to have protection!
                    /*int addrtaboffset = GetStartOfAddressTableOffset(filename);
                    Console.WriteLine("addrtaboffset: " + addrtaboffset.ToString("X8"));
                    //int NqNqNqOffset = GetNqNqNqStringFromOffset(addrtaboffset - 0x100, filename);
                    int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
                    Console.WriteLine("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));*/

                    //<GS-22032010>
                    int addrtaboffset = GetEndOfSymbolTable(filename);
                    Console.WriteLine("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
                    int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
                    Console.WriteLine("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));
                    //<GS-22032010>

                    int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
                    Console.WriteLine("symbtaboffset: " + symbtaboffset.ToString("X8"));
                    //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
                    //                    symbtaboffset = NqNqNqOffset;
                    int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
                    Console.WriteLine("symbtablength: " + symbtablength.ToString("X8"));
                    if (symbtablength < 0x1000) return false; // NO SYMBOLTABLE IN FILE
                    //symbtaboffset -= 2;
                    if (symbtaboffset > 0 && symbtaboffset < 0xF0000)
                    {
                        val = symbtaboffset;

                    }
                    symboltableoffset = val;
                    // MessageBox.Show("Packed table index: " + idx.ToString("X6") + " " + val.ToString("X6"));
                    int state = 0;
                    FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    using (BinaryReader br = new BinaryReader(fsread))
                    {

                        fsread.Seek(val, SeekOrigin.Begin);

                        UnpackedLength = Convert.ToInt64(br.ReadByte());
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256;
                        Console.WriteLine("UnpackedLength: " + UnpackedLength.ToString("X8"));
                        fsread.Seek(val, SeekOrigin.Begin);

                        FileStream fswrite = new FileStream(System.Windows.Forms.Application.StartupPath + "\\COMPR", FileMode.Create);
                        using (BinaryWriter bw = new BinaryWriter(fswrite))
                        {
                            for (int bc = 0; bc < /*bytesread-10*/ symbtablength; bc++)
                            {
                                bw.Write(br.ReadByte());
                            }
                        }
                        fswrite.Close();
                        fswrite.Dispose();

                    }
                    fsread.Close();
                    fsread.Dispose();
                }
                if (UnpackedLength > 0x00FFFFFF) return false;
                return true;
            }
            catch (Exception E)
            {
                Console.WriteLine("Error 1: " + E.Message);
            }
            return false;
        }

        private int ReadMarkerAddressContent(string filename, int value, out int length, out int val)
        {
            int retval = 0;
            length = 0;
            val = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = m_currentfile_size - 0x90;
                    try
                    {

                        fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                        byte[] inb = br.ReadBytes(0x8F);
                        //int offset = 0;
                        for (int t = 0; t < 0xFF; t++)
                        {
                            if (((byte)inb.GetValue(t) == (byte)value) && ((byte)inb.GetValue(t + 1) < 0x30))
                            {
                                // marker gevonden
                                // lees 6 terug
                                retval = /*0x3FF00*/ fileoffset + t;
                                length = (byte)inb.GetValue(t + 1);
                                break;
                            }
                        }
                        fs.Seek((retval - length), SeekOrigin.Begin);
                        byte[] info = br.ReadBytes(length);
                        for (int bc = info.Length - 1; bc >= 0; bc--)
                        {
                            int temp = Convert.ToInt32(info.GetValue(bc));
                            for (int mt = 0; mt < (3 - bc); mt++)
                            {
                                temp *= 256;
                            }
                            val += temp;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Error 2: " + E.Message);
                        retval = 0;
                    }
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();

                }
            }

            return retval;

        }

        private int ReadMarkerAddress(string filename, int value, out int length, out string val)
        {
            int retval = 0;
            length = 0;
            val = string.Empty;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = m_currentfile_size - 0x100;

                    fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                    byte[] inb = br.ReadBytes(0xFF);
                    //int offset = 0;
                    for (int t = 0; t < 0xFF; t++)
                    {
                        if (((byte)inb.GetValue(t) == (byte)value) && ((byte)inb.GetValue(t + 1) < 0x30))
                        {
                            // marker gevonden
                            // lees 6 terug
                            retval = /*0x3FF00*/ fileoffset + t;
                            length = (byte)inb.GetValue(t + 1);
                            break;
                        }
                    }
                    fs.Seek((retval - length), SeekOrigin.Begin);
                    byte[] info = br.ReadBytes(length);
                    for (int bc = info.Length - 1; bc >= 0; bc--)
                    {
                        val += Convert.ToChar(info.GetValue(bc));
                    }
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();

                }
            }

            return retval;
        }

        private void OpenFile(string filename)
        {
            CloseProject();
            m_currentfile = filename;
            TryToOpenFile(m_currentfile, out m_symbols, m_currentfile_size);

            this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
            barFilenameText.Caption = Path.GetFileName(m_currentfile);
            gridControlSymbols.DataSource = m_symbols;
            m_appSettings.Lastfilename = m_currentfile;
            gridViewSymbols.BestFitColumns();
            UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);

            m_appSettings.LastOpenedType = 0;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "bin";
            ofd.Filter = "Trionic 8 binary files|*.bin;*.s19";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filename = string.Empty;
                if (ofd.FileName.ToUpper().EndsWith("S19"))
                {
                    srec2bin cvt = new srec2bin();

                    cvt.ConvertSrecToBin(ofd.FileName, out filename);
                }
                else
                {
                    filename = ofd.FileName;
                }
                OpenFile(filename);
            }
            //if (m_currentfile != string.Empty) LoadRealtimeTable();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            StartTableViewer();
        }
        private void GetAxisDescriptions(string filename, SymbolCollection curSymbols, string symbolname, out string x, out string y, out string z)
        {
            x = "x-axis";
            y = "y-axis";
            z = "z-axis";
            SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
            string x_axis = string.Empty;
            string y_axis = string.Empty;
            string x_axis_descr = string.Empty;
            string y_axis_descr = string.Empty;
            string z_axis_descr = string.Empty;
            axestrans.GetAxisSymbols(symbolname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
            if (x_axis == "BstKnkCal.OffsetXSP")
            {
                if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
            }

            x = x_axis_descr;
            y = y_axis_descr;
            z = z_axis_descr;
        }

        private int GetSymbolLength(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Length;
                }
            }
            return 0;
        }

        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }

        private int GetSymbolNumber(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Symbol_number;
                }
            }
            return 0;
        }

        private Int64 GetSymbolAddressSRAM(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Start_address;
                }
            }
            return 0;
        }

        private int GetTableMatrixWitdhByName(string filename, SymbolCollection curSymbols, string symbolname, out int columns, out int rows)
        {

            columns = 1;// GetSymbolLength(curSymbols, symbolname) / 2;
            if (symbolname == "MisfCal.m_LoadLevelMAT") columns = 5;
            else if (symbolname == "PedalMapCal.GainFactorMap") columns = 16;
            else if (symbolname == "FrictionLoadCal.Trq_RequestT_EngMAP") columns = 9;
            else if (symbolname == "CatDiagCal.t_Ph3MaxMAT") columns = 7;

            else if (GetSymbolLength(curSymbols, symbolname) == 576) columns = 18;
            else if (GetSymbolLength(curSymbols, symbolname) == 672) columns = 16;
            else if (GetSymbolLength(curSymbols, symbolname) == 512) columns = 16;
            else if (GetSymbolLength(curSymbols, symbolname) == 504) columns = 14;
            else if (GetSymbolLength(curSymbols, symbolname) == 480) columns = 6;
            else if (GetSymbolLength(curSymbols, symbolname) == 416) columns = 16;
            else if (GetSymbolLength(curSymbols, symbolname) == 384) columns = 12;
            else if (GetSymbolLength(curSymbols, symbolname) == 336 && !symbolname.StartsWith("PurgeCal.")) columns = 16;
            else if (GetSymbolLength(curSymbols, symbolname) == 336 && symbolname.StartsWith("PurgeCal.")) columns = 12;
            else if (GetSymbolLength(curSymbols, symbolname) == 320) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 306) columns = 9;
            else if (GetSymbolLength(curSymbols, symbolname) == 288) columns = 18;
            else if (GetSymbolLength(curSymbols, symbolname) == 256) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 224) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 220) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 208) columns = 13;
            else if (GetSymbolLength(curSymbols, symbolname) == 204) columns = 6;
            else if (GetSymbolLength(curSymbols, symbolname) == 200) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 192) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 168) columns = 7;
            else if (GetSymbolLength(curSymbols, symbolname) == 140) columns = 7;
            else if (GetSymbolLength(curSymbols, symbolname) == 130) columns = 1;
            else if (GetSymbolLength(curSymbols, symbolname) == 128) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 112) columns = 14;
            else if (GetSymbolLength(curSymbols, symbolname) == 100) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 98) columns = 7;
            else if (GetSymbolLength(curSymbols, symbolname) == 80) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 60) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 50) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 96) columns = 6;
            else if (GetSymbolLength(curSymbols, symbolname) == 64) columns = 4;
            else if (GetSymbolLength(curSymbols, symbolname) == 160) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 72) columns = 9;
            else if (GetSymbolLength(curSymbols, symbolname) == 42) columns = 7; // testen... 
            if (symbolname.ToUpper().EndsWith("YSP") || symbolname.ToUpper().EndsWith("XSP")) columns = 1;
            rows = GetSymbolLength(curSymbols, symbolname) / columns;
            return columns;
        }

        private double GetMapCorrectionOffset(string symbolname)
        {
            double returnvalue = 0;
            return returnvalue;
        }

        private bool IsDigit(char c, out bool isdot)
        {
            isdot = false;
            if (c >= 0x30 && c <= 0x39) return true;
            if (c == '.')
            {
                isdot = true;
                return true;
            }
            if (c == ',')
            {
                isdot = true;
                return true;
            }
            return false;
        }

        private string ClearToNumber(string value)
        {
            string retval = string.Empty;
            bool dotseen = false;
            for (int t = 0; t < value.Length; t++)
            {
                bool isdot = false;
                if (IsDigit(value[t], out isdot))
                {
                    if (isdot)
                    {
                        if (!dotseen)
                        {
                            retval += value[t];
                        }
                        dotseen = true;
                    }
                    else
                    {
                        retval += value[t];
                    }
                }
                else
                {
                    return retval;
                }
            }
            return retval;
        }

        private double GetMapCorrectionFactor(string symbolname)
        {
            double returnvalue = 1;
            try
            {
                SymbolTranslator st = new SymbolTranslator();
                string helptext = string.Empty;
                XDFCategories cat = XDFCategories.Undocumented;
                XDFSubCategory subcat = XDFSubCategory.Undocumented;
                string text = st.TranslateSymbolToHelpText(symbolname, out helptext, out cat, out subcat);
                if (helptext.Contains("Resolution is"))
                {
                    int idx = helptext.IndexOf("Resolution is");
                    idx += 14;
                    string value = helptext.Substring(idx).Trim();
                    if (value.Contains(" "))
                    {
                        int idx2 = value.IndexOf(" ");
                        value = value.Substring(0, idx2);
                        value = ClearToNumber(value);
                        returnvalue = ConvertToDouble(value);
                    }
                    else
                    {
                        value = ClearToNumber(value);
                        returnvalue = ConvertToDouble(value);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            if (returnvalue == 0) returnvalue = 1;
            if (symbolname == "KnkSoundRedCal.fi_OffsMa") returnvalue = 0.1;
            else if (symbolname == "IgnE85Cal.fi_AbsMap") returnvalue = 0.1;
            else if (symbolname == "MAFCal.cd_ThrottleMap") returnvalue = 0.0009765625;
            else if (symbolname == "TrqMastCal.Trq_NominalMap") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.Trq_MBTMAP") returnvalue = 0.1;
            else if (symbolname == "AfterStCal.StartMAP") returnvalue = 0.0009765625; // 1/1024
            else if (symbolname == "KnkFuelCal.EnrichmentMap") returnvalue = 0.0009765625; // 1/1024
            else if (symbolname == "AfterStCal.HotSoakMAP") returnvalue = 0.0009765625; // 1/1024
            else if (symbolname == "MAFCal.NormAdjustFacMap") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "BFuelCal.LambdaOneFacMap") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "BFuelCal.TempEnrichFacMap") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "BFuelCal.E85TempEnrichFacMap") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "AfterStCal.AmbientMAP") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "FFFuelCal.KnkEnrichmentMAP") returnvalue = 0.0078125; // 1/128
            else if (symbolname == "FFFuelCal.TempEnrichFacMAP") returnvalue = 0.0078125; // 1/128




           /** vanaf hier */
            else if (symbolname == "CoolFanCal.K_Speed_ON_FanSP") returnvalue = 0.1;
            else if (symbolname == "CoolFanCal.K_Speed_OFF_FanSP") returnvalue = 0.1;
            else if (symbolname == "CoolFanCal.K_ECT_FanReqTAB") returnvalue = 0.1;
            else if (symbolname == "CoolFanCal.K_AC_FanReqTAB") returnvalue = 0.1;
            else if (symbolname == "CoolFanCal.K_TempEngOil_FanReqTAB") returnvalue = 0.1;
            else if (symbolname == "CoolFanCal.K_AfterRunTimeTAB") returnvalue = 0.1;
            else if (symbolname == "FFCatDiagCal.X_blendFacTab") returnvalue = 0.0009765625;
            else if (symbolname == "FFCatModCal.X_SSTempInterpolWeightTAB") returnvalue = 0.0078125;
            else if (symbolname == "Lambda.IntegratorBuf") returnvalue = 0.01;
            else if (symbolname == "FCutCal.v_EngineKillSP") returnvalue = 0.1;
            else if (symbolname == "FCutCal.FuelFactorMAP") returnvalue = 0.0009765625;
            else if (symbolname == "FCutCal.A_ThrottleTAB") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.Trq_ManGear") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.Trq_MaxEngineTab1") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.Trq_MaxEngineTab2") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.Trq_OverBoostTab") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.CompressorNoiseXSP") returnvalue = 0.1;
            else if (symbolname == "TrqLimCal.Trq_CompressorNoiseRedLimMAP") returnvalue = 0.1;
            else if (symbolname == "MAFProt.p_AirInletBuffer") returnvalue = 0.1;
            else if (symbolname == "MAFProt.p_AirInletPredBuffer") returnvalue = 0.1;
            else if (symbolname == "FuelAdapAdap.AdapMap") returnvalue = 0.01;
            else if (symbolname == "FuelAdapAdap.AdapTab") returnvalue = 0.01;
            else if (symbolname == "IgnAbsCal.EngEfficiencySP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_etaIgnOffsetTAB") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_IgnMBTMAP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_highOctanMAP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_lowOctanMAP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_NormalMAP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_FuelCutMAP") returnvalue = 0.1;
            else if (symbolname == "IgnAbsCal.fi_StartMAP") returnvalue = 0.1;
            else if (symbolname == "TrqMastProt.Trq_ResBuffer") returnvalue = 0.1;
            else if (symbolname == "obdCore.v_DrvCondFaultVeh") returnvalue = 0.1;
            else if (symbolname == "obdCore.m_DrvCondFaultLoad") returnvalue = 0.01;
            else if (symbolname == "StartEngCal.p_AirAmbientYSP") returnvalue = 0.1;
            else if (symbolname == "StartEngCal.Q_AirStartOffsetAutMAP") returnvalue = 0.01;
            else if (symbolname == "StartEngCal.Q_AirStartOffsetManMAP") returnvalue = 0.01;
            else if (symbolname == "FMastCal.X_FuelConsFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "FMastCal.FuelCompCylMAP1") returnvalue = 0.0009765625;
            else if (symbolname == "FMastCal.FuelCompCylMAP2") returnvalue = 0.0009765625;
            else if (symbolname == "FMastCal.FuelCompCylMAP3") returnvalue = 0.0009765625;
            else if (symbolname == "FMastCal.FuelCompCylMAP4") returnvalue = 0.0009765625;
            else if (symbolname == "BFuelCal.t_JerkDelayTAB") returnvalue = 0.1;
            else if (symbolname == "BFuelCal.FCIMinLoadFuelFacMAP") returnvalue = 0.0078125;
            else if (symbolname == "BFuelCal.LambdaOneFacMap") returnvalue = 0.0078125;
            else if (symbolname == "BFuelCal.TempEnrichFacMap") returnvalue = 0.0078125;
            else if (symbolname == "BFuelCal.JerkEnrichFacTab") returnvalue = 0.0078125;
            else if (symbolname == "InjCorrCal.BattCorrSP") returnvalue = 0.1;
            else if (symbolname == "AirAmbientCal.X_PrRatioMinMAP") returnvalue = 0.0009765625;
            else if (symbolname == "AirAmbientCal.X_PrRatioMaxMAP") returnvalue = 0.0009765625;
            else if (symbolname == "AirAmbientCal.p_ThrRatioSP") returnvalue = 0.001;
            else if (symbolname == "AirAmbientCal.X_PrAdjustEst") returnvalue = 0.0078125;
            else if (symbolname == "TMCCal.Trq_FrictionMAP") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_NeutralMAP") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_DriveMAP") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_ACLoadTAB") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_ACDynRecTAB") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_MinLoadTAB") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_MaxEngineTab") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_MaxEngineLowTab") returnvalue = 0.1;
            else if (symbolname == "TMCCal.Trq_ElLoadTAB") returnvalue = 0.1;
            else if (symbolname == "TMCCal.PWM_ElSP") returnvalue = 0.1;
            else if (symbolname == "AltCtrlCal.K_ColdStartDsblTAB") returnvalue = 0.1;
            else if (symbolname == "SystemActionCal.BefThrOffsetMAP") returnvalue = 0.1;
            else if (symbolname == "SystemActionCal.A_ThrottleXSP") returnvalue = 0.1;
            else if (symbolname == "SystemActionCal.PwrLimFanMAP") returnvalue = 0.1;
            else if (symbolname == "SystemActionCal.v_VehicleXSP") returnvalue = 0.1;
            else if (symbolname == "SystemActionCal.Q_ThrotDfltFlowTAB") returnvalue = 0.01;
            else if (symbolname == "FrictionLoadCal.Trq_RequestT_EngMAP") returnvalue = 0.1;
            else if (symbolname == "FrictionLoadCal.p_AirAmbSP") returnvalue = 0.1;
            else if (symbolname == "FrictionLoadCal.Trq_Request_p_AirAmbTAB") returnvalue = 0.1;
            else if (symbolname == "FrictionLoadCal.Trq_RequestT_AirAmbTAB") returnvalue = 0.1;
            else if (symbolname == "IgnTransCal.p_AirInletXSP") returnvalue = 0.1;
            else if (symbolname == "IgnTransCal.factorMAP") returnvalue = 0.0009765625;
            else if (symbolname == "IgnOffsCal.fi_OffsCyl") returnvalue = 0.1;
            else if (symbolname == "IgnKnkCal.fi_RetardTAB") returnvalue = 0.1;
            else if (symbolname == "IgnKnkCal.fi_TransRetardTAB") returnvalue = 0.1;
            else if (symbolname == "IgnKnkCal.fi_Map1MaxOffset") returnvalue = 0.1;
            else if (symbolname == "IgnKnkCal.RetardIndexMap") returnvalue = 0.01;
            else if (symbolname == "IgnKnkCal.ARetardIndexMap") returnvalue = 0.01;
            else if (symbolname == "IgnKnkCal.IndexMap") returnvalue = 0.01;
            else if (symbolname == "IgnKnkCal.K_RetardIndexTAB") returnvalue = 0.0078125;
            else if (symbolname == "IgnTempCal.fi_OffsetMaxAirInletMap") returnvalue = 0.1;
            else if (symbolname == "IgnTempCal.fi_OffsetMinAirInletMap") returnvalue = 0.1;
            else if (symbolname == "IgnTempCal.fi_OffsetMaxTEngMap") returnvalue = 0.1;
            else if (symbolname == "IgnTempCal.fi_OffsetMinTEngMap") returnvalue = 0.1;
            else if (symbolname == "IgnJerkCal.fi_Tab") returnvalue = 0.1;
            else if (symbolname == "KnkSoundRedCal.fi_OffsMap") returnvalue = 0.1;
            else if (symbolname == "BoostAdapAdap.AdapMAP") returnvalue = 0.0078125;
            else if (symbolname == "LightOffCal.Trq_RequestXSP") returnvalue = 0.1;
            else if (symbolname == "LightOffCal.Trq_ReserveMAP") returnvalue = 0.1;
            else if (symbolname == "FFBoffCal.p_diff6SP") returnvalue = 0.1;
            else if (symbolname == "DNCompCal.Trq_ReqNeutralMAP") returnvalue = 0.1;
            else if (symbolname == "DNCompCal.Trq_ReqDriveMAP") returnvalue = 0.1;
            else if (symbolname == "DNCompCal.n_DeltaTAB") returnvalue = 0.0078125;
            else if (symbolname == "ElCompCal.P_FanCompTAB") returnvalue = 0.1;
            else if (symbolname == "ElCompCal.U_BattDiffSP") returnvalue = 0.1;
            else if (symbolname == "ElCompCal.P_BattDiffTAB") returnvalue = 0.1;
            else if (symbolname == "ElCompCal.Trq_FTermMap") returnvalue = 0.1;
            else if (symbolname == "ElCompCal.PWM_FTermYSP") returnvalue = 0.1;
            else if (symbolname == "PedalMapCal.Trq_RequestMap") returnvalue = 0.1;
            else if (symbolname == "PedalMapCal.Trq_RequestSportMap") returnvalue = 0.1;
            else if (symbolname == "PedalMapCal.GainFactorMap") returnvalue = 0.0078125;
            else if (symbolname == "TOACal.v_vehicleXSP") returnvalue = 0.1;
            else if (symbolname == "TOACal.Trq_P_partMAP") returnvalue = 0.1;
            else if (symbolname == "TOACal.X_D_partFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "TOACal.v_vehicleYSP") returnvalue = 0.1;
            else if (symbolname == "TOACal.Trq_maxTAB") returnvalue = 0.1;
            else if (symbolname == "ExhaustCal.t_mAirTau1Tab") returnvalue = 0.0625;
            else if (symbolname == "ExhaustCal.t_mAirTau2Tab") returnvalue = 0.0625;
            else if (symbolname == "ExhaustCal.fi_IgnSP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.P_PartIgnMAP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.P_PartIgnTakeOffTAB") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.I_PartIgnTAB") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.v_VehicleXSP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.I_PartAirSP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.P_PartAirMAP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.I_PartAirTAB") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlCal.v_VehicleYSP") returnvalue = 0.1;
            else if (symbolname == "FFFDynProt.betaTAB") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynProt.alphaTAB") returnvalue = 0.003921569;
            else if (symbolname == "FMastProt.CylMulFac25ms") returnvalue = 0.0009765625;
            else if (symbolname == "AirCtrlCal.p_ThrRatioSP") returnvalue = 0.001;
            else if (symbolname == "AirCtrlCal.p_diffThrotSP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.Ppart_BoostAirM1TAB") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.p_AirAmbientXSP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.Ppart_BoostMap") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.Ipart_BoostMap") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.Dpart_BoostMap") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.RegMap") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.p_diffReqMAP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.q_AirInletSP") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.PRatioMaxTab") returnvalue = 0.01;
            else if (symbolname == "AirCtrlCal.m_IPartThrLimNegTab") returnvalue = 0.0078125;
            else if (symbolname == "AirCtrlCal.m_IPartThrLimPosTab") returnvalue = 0.0078125;
            else if (symbolname == "AirCtrlCal.p_AfterTurbineXSP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.p_AirAmbientYSP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlCal.BasicPresTempCompTAB") returnvalue = 0.0078125;
            else if (symbolname == "AirCtrlCal.m_IpartThrMaxMAP") returnvalue = 0.0078125;
            else if (symbolname == "AreaCal.Area") returnvalue = 0.1;
            else if (symbolname == "AreaCal.Cd_DischargeMAP") returnvalue = 0.0009765625;
            else if (symbolname == "VenturiCal.p_AirAmbientSP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlProt.v_Buffer") returnvalue = 0.1;
            else if (symbolname == "StartCal.EnrFacManTab") returnvalue = 0.01;
            else if (symbolname == "StartCal.EnrFacManThrFaultTab") returnvalue = 0.01;
            else if (symbolname == "StartCal.EnrFacAutTab") returnvalue = 0.01;
            else if (symbolname == "StartCal.EnrFacAutThrFaultTab") returnvalue = 0.01;
            else if (symbolname == "StartCal.t_RestartXSP") returnvalue = 0.1;
            else if (symbolname == "StartCal.RestartFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "StartCal.EnrFacRecovStep") returnvalue = 0.01;
            else if (symbolname == "StartCal.EnrFacWCFuel") returnvalue = 0.01;
            else if (symbolname == "StartCal.CombFacAutMAP") returnvalue = 0.01;
            else if (symbolname == "StartCal.CombFacManMAP") returnvalue = 0.01;
            else if (symbolname == "StartCal.m_FuelBefStart") returnvalue = 0.01;
            else if (symbolname == "StartCal.m_ReStBefFuel") returnvalue = 0.01;
            else if (symbolname == "StartCal.p_HighAltSP") returnvalue = 0.1;
            else if (symbolname == "StartCal.ScaleFacRpmMap") returnvalue = 0.0078125;
            else if (symbolname == "StartCal.HighAltFacMap") returnvalue = 0.0078125;
            else if (symbolname == "KnkFuelCal.EnrichmentMap") returnvalue = 0.0009765625;
            else if (symbolname == "KnkFuelCal.fi_OffsetEnrichEnable") returnvalue = 0.1;
            else if (symbolname == "KnkFuelCal.fi_MaxOffsetMap") returnvalue = 0.1;
            else if (symbolname == "IgnAbsProt.tempTAB") returnvalue = 0.1;
            else if (symbolname == "FFTrqCal.FFTrq_MaxEngineTab1") returnvalue = 0.1;
            else if (symbolname == "FFTrqCal.FFTrq_MaxEngineTab2") returnvalue = 0.1;
            else if (symbolname == "FFTrqCal.X_torqueTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFTrqCal.M_maxMAP") returnvalue = 0.1;
            else if (symbolname == "FrompAdapAdap.AdapMAP") returnvalue = 0.0078125;
            else if (symbolname == "GearCal.Ratio") returnvalue = 0.01;
            else if (symbolname == "GearCal.Range") returnvalue = 0.01;
            else if (symbolname == "ACCompCal.p_ACXSP") returnvalue = 0.01;
            else if (symbolname == "ACCompCal.Trq_ReqStaticMAP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlCal.v_AccReqSP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlCal.v_ResumeSP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlCal.v_SetSP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlCal.v_DecSP") returnvalue = 0.1;
            else if (symbolname == "CrsCntrlCal.a_AccReqTAB") returnvalue = 0.01;
            else if (symbolname == "CrsCntrlCal.a_ResumeTAB") returnvalue = 0.01;
            else if (symbolname == "CrsCntrlCal.a_SetTAB") returnvalue = 0.01;
            else if (symbolname == "CrsCntrlCal.a_DecTAB") returnvalue = 0.01;
            else if (symbolname == "CrsCntrlCal.X_EfficiencySP") returnvalue = 0.0078125;
            else if (symbolname == "CrsCntrlCal.X_EfficiencyTAB") returnvalue = 0.0078125;
            else if (symbolname == "IgnKnk.fi_Offset") returnvalue = 0.1;
            else if (symbolname == "IgnKnk.fi_OffNorm") returnvalue = 0.1;
            else if (symbolname == "IgnKnk.fi_Diff") returnvalue = 0.1;
            else if (symbolname == "BlockHeatCal.StartFuelFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "BlockHeatCal.t_SoakMinutesSP") returnvalue = 0.1;
            else if (symbolname == "BlockHeatCal.AftStFuelFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "BlockHeatCal.Trq_FricOffsetTAB") returnvalue = 0.1;
            else if (symbolname == "BlockHeatCal.Q_StartOffsetTAB") returnvalue = 0.01;
            else if (symbolname == "BlockHeatCal.t_soakSP") returnvalue = 0.1;
            else if (symbolname == "BlockHeatCal.X_reStartFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "BlockHeatCal.X_AftReStartFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "LambdaCal.rampTempScaleTAB") returnvalue = 0.0078125;
            else if (symbolname == "LambdaCal.IntStepGainTAB") returnvalue = 0.0078125;
            else if (symbolname == "LambdaCal.IntStepGainMisfireTAB") returnvalue = 0.0078125;
            else if (symbolname == "LambdaCal.DecStepMAP") returnvalue = 0.01;
            else if (symbolname == "LambdaCal.DecRampMAP") returnvalue = 0.01;
            else if (symbolname == "LambdaCal.IncStepMAP") returnvalue = 0.01;
            else if (symbolname == "LambdaCal.IncRampMAP") returnvalue = 0.01;
            else if (symbolname == "LambdaCal.p_SwitchPointAmbSP") returnvalue = 0.1;
            else if (symbolname == "OBDAdap.V_FuelLevResult") returnvalue = 0.1;
            else if (symbolname == "BstKnkCal.fi_offsetXSP") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlAdapAdap.Trq_ReqDriveNormTAB") returnvalue = 0.1;
            else if (symbolname == "RpmCtrlAdapAdap.Trq_ReqNeutralNormTAB") returnvalue = 0.1;
            else if (symbolname == "SAICal.m_AirSAIMAP") returnvalue = 0.01;
            else if (symbolname == "AfterStCal.StartMAP") returnvalue = 0.0009765625;
            else if (symbolname == "AfterStCal.AmbientMAP") returnvalue = 0.0009765625;
            else if (symbolname == "AfterStCal.p_AirAmbientYSP") returnvalue = 0.1;
            else if (symbolname == "AfterStCal.RestartAdjustTAB") returnvalue = 0.0078125;
            else if (symbolname == "AfterStCal.HotSoakMAP") returnvalue = 0.0009765625;
            else if (symbolname == "AfterStCal.HotSoakCylFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "AfterStCal.t_soakXSP") returnvalue = 0.1;
            else if (symbolname == "AfterStCal.t_soakXSP2") returnvalue = 0.1;
            else if (symbolname == "AfterStCal.HotSoakAdjustMAP") returnvalue = 0.0078125;
            else if (symbolname == "AfterStCal.WCFMAP") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap1") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap2") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap3") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap4") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap5") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap6") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap7") returnvalue = 0.0009765625;
            else if (symbolname == "TCompCal.FuelFacMap8") returnvalue = 0.0009765625;
            else if (symbolname == "TAirCompCal.FuelFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "InjAnglCal.Map") returnvalue = 0.1;
            else if (symbolname == "KnkDetCal.fi_knkWinOffsMAP") returnvalue = 0.1;
            else if (symbolname == "KnkDetCal.fi_knkWinSizeMAP") returnvalue = 0.1;
            else if (symbolname == "KnkDetCal.X_hystOffsetMAP") returnvalue = 0.0009765625;
            else if (symbolname == "KnkDetCal.X_AHystOffsetMAP") returnvalue = 0.0009765625;
            else if (symbolname == "KnkDetCal.fi_DiffXSP") returnvalue = 0.1;
            else if (symbolname == "KnkDetCal.K1TAB") returnvalue = 0.0078125;
            else if (symbolname == "MisfAveCal.KaTAB") returnvalue = 0.0009765625;
            else if (symbolname == "MisfAveCal.KbTAB") returnvalue = 0.0009765625;
            else if (symbolname == "MisfAveCal.Ka2TAB") returnvalue = 0.0009765625;
            else if (symbolname == "MisfAveCal.Kb2TAB") returnvalue = 0.0009765625;
            else if (symbolname == "PurgeCal.p_Diff16EUXSp") returnvalue = 0.1;
            else if (symbolname == "PurgeCal.p_Diff16USXSp") returnvalue = 0.1;
            else if (symbolname == "PurgeCal.ValveMap16EU") returnvalue = 0.1;
            else if (symbolname == "PurgeCal.ValveMap16US") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.f_FFSensorSP") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.p_AirInletTAB") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.p_AirBefThrottleTAB") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.p_SAITAB") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.V_FuelTankEUTAB") returnvalue = 0.1;
            else if (symbolname == "VIOSSensorCal.V_FuelTankUSTAB") returnvalue = 0.1;
            else if (symbolname == "VIOSMAFCal.Q_AirInletTab") returnvalue = 0.01;
            else if (symbolname == "FFIgnCal.o_octaneTAB") returnvalue = 0.01;
            else if (symbolname == "FFIgnCal.X_hystOffsetMAP") returnvalue = 0.0009765625;
            else if (symbolname == "FFIgnCal.X_ignTAB") returnvalue = 0.0078125;
            else if (symbolname == "CoreDiagCal.p_PosAmbDiffLim") returnvalue = 0.1;
            else if (symbolname == "CoreDiagCal.p_NegAmbDiffLim") returnvalue = 0.1;
            else if (symbolname == "CoreDiagCal.p_AmbDiffLimSP") returnvalue = 0.1;
            else if (symbolname == "CatDiagCal.T_CatMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "CatDiagCal.Q_AirMeanSP") returnvalue = 0.01;
            else if (symbolname == "CatDiagCal.LoadMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "CatDiagCal.Q_AirSP") returnvalue = 0.01;
            else if (symbolname == "EvapDiagCal.V_Test1FuelYSP") returnvalue = 0.1;
            else if (symbolname == "EvapDiagCal.V_Test2FuelYSP") returnvalue = 0.1;
            else if (symbolname == "EvapDiagCal.LeakFacTest1MAT") returnvalue = 0.1;
            else if (symbolname == "EvapDiagCal.LeakFacTest2MAT") returnvalue = 0.1;
            else if (symbolname == "O2SRespDiagCal.CombMulFacMAT") returnvalue = 0.0009765625;
            else if (symbolname == "O2SPreDiagCal.t_O2SStartTAB") returnvalue = 0.1;
            else if (symbolname == "FuelDeterminationCal.V_TankEUTAB") returnvalue = 0.1;
            else if (symbolname == "FuelDeterminationCal.V_TankUSTAB") returnvalue = 0.1;
            else if (symbolname == "FuelDeterminationCal.V_TankEUSP") returnvalue = 0.1;
            else if (symbolname == "FuelDeterminationCal.V_TankUSSP") returnvalue = 0.1;
            else if (symbolname == "MisfCal.Trq_OffsetTAB") returnvalue = 0.1;
            else if (symbolname == "CatModCal.LambdaSP") returnvalue = 0.0009765625;
            else if (symbolname == "CatModCal.Q_LoadFiltCoefSP") returnvalue = 0.01;
            else if (symbolname == "CatModCal.Q_CorrFacSP") returnvalue = 0.01;
            else if (symbolname == "CatModCal.LoadFiltCoefTAB") returnvalue = 0.0001;
            else if (symbolname == "CatModCal.t_SoakMinYSP") returnvalue = 0.1;
            else if (symbolname == "CatModCal.QAccWeightFacSP") returnvalue = 0.01;
            else if (symbolname == "CatModCal.TEngineMulFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "CatModCal.TSoakFacMAP") returnvalue = 0.0078125;
            else if (symbolname == "CatModCal.QAccWeightFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "CatModCal.QCorrFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "TAmbModCal.t_StartWaitTAB") returnvalue = 0.1;
            else if (symbolname == "TAmbModCal.v_VehicleSP") returnvalue = 0.1;
            else if (symbolname == "TAmbModCal.TempFilterCoefTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.U_BattMulFacSP") returnvalue = 0.1;
            else if (symbolname == "SAIDiagCal.UBattMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.TAmbMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.fi_IgnMulFacSP") returnvalue = 0.1;
            else if (symbolname == "SAIDiagCal.fiIgnMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.Q_AirMulFacSP") returnvalue = 0.01;
            else if (symbolname == "SAIDiagCal.QAirMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.pAirAmbientMulFacSP") returnvalue = 0.1;
            else if (symbolname == "SAIDiagCal.pAirAmbientMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "SAIDiagCal.m_FuelMulFacSP") returnvalue = 0.01;
            else if (symbolname == "SAIDiagCal.mFuelMulFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "TComp.FuelFacTab") returnvalue = 0.0009765625;
            else if (symbolname == "EngTipLimCal.X_Koeff") returnvalue = 0.0001;
            else if (symbolname == "EngTipLimCal.Trq_BacklashMap") returnvalue = 0.1;
            else if (symbolname == "EngTipLimCal.Trq_BacklashAutTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimCal.Trq_BacklashFCutAutTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimCal.Trq_RequestSP") returnvalue = 0.1;
            else if (symbolname == "EngTipLimCal.X_PedalRequestSP") returnvalue = 0.1;
            else if (symbolname == "EngTipLimCal.X_BacklashFacTab") returnvalue = 0.0078125;
            else if (symbolname == "EngTipLimNormCal.Trq_BacklashTipOutMap") returnvalue = 0.1;
            else if (symbolname == "EngTipLimNormCal.Trq_TipOutRampTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimNormCal.Trq_SumTipIn") returnvalue = 0.1;
            else if (symbolname == "EngTipLimNormCal.Trq_BacklashTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimNormCal.Trq_BacklashFCutTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimSportCal.Trq_BacklashTipOutMap") returnvalue = 0.1;
            else if (symbolname == "EngTipLimSportCal.Trq_TipOutRampTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimSportCal.Trq_SumTipIn") returnvalue = 0.1;
            else if (symbolname == "EngTipLimSportCal.Trq_BacklashTab") returnvalue = 0.1;
            else if (symbolname == "EngTipLimSportCal.Trq_BacklashFCutTab") returnvalue = 0.1;
            else if (symbolname == "KnkDetProt.X_npfX") returnvalue = 0.0009765625;
            else if (symbolname == "KnkDetProt.X_average") returnvalue = 0.0009765625;
            else if (symbolname == "KnkDetProt.BGN") returnvalue = 0.0009765625;
            else if (symbolname == "FFRfuelCal.V_fuelTankTAB") returnvalue = 0.1;
            else if (symbolname == "FFRfuelCal.V_TankSP6") returnvalue = 0.1;
            else if (symbolname == "FFRfuelCal.V_ErrorTankTAB") returnvalue = 0.1;
            else if (symbolname == "FFBladCal.M_lowLimSysActTab") returnvalue = 0.1;
            else if (symbolname == "pAirSensProt.p_DevMAP_SAI") returnvalue = 0.1;
            else if (symbolname == "pAirSensProt.p_DevSAI_pBef") returnvalue = 0.1;
            else if (symbolname == "pAirSensProt.p_DevMAP_pBef") returnvalue = 0.1;
            else if (symbolname == "obdMAF.bufHiWin") returnvalue = 0.01;
            else if (symbolname == "obdMAF.bufLoWin") returnvalue = 0.01;
            else if (symbolname == "AirMassMastCal.Trq_MBTXSP") returnvalue = 0.1;
            else if (symbolname == "FuelDynCal.WCFbetaSP") returnvalue = 0.0078125;
            else if (symbolname == "FuelDynCal.alphaStartTAB") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.X_negFuelLimStartTAB") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.WCFbetaTAB") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.X_negFuelLimMAP") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.FuelModFacTab") returnvalue = 0.0078125;
            else if (symbolname == "FuelDynCal.betaMAP1") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP2") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP3") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP4") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP5") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP6") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP7") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.betaMAP8") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP1") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP2") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP3") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP4") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP5") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP6") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP7") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaSSMAP8") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP1") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP2") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP3") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP4") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP5") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP6") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP7") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynCal.alphaMAP8") returnvalue = 0.003921569;
            else if (symbolname == "TrqMastCal.Trq_NominalMap") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.X_AccPedalMAP") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.IgnAngleDiffSP") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.TLO_TAB") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.Trq_EngXSP") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.Trq_PedYSP") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.Trq_MaxDerIncMAP") returnvalue = 0.1;
            else if (symbolname == "TrqMastCal.Trq_MaxDerDecMAP") returnvalue = 0.1;
            else if (symbolname == "IgnMastCal.fi_minFCITAB") returnvalue = 0.1;
            else if (symbolname == "IgnMastCal.MinMap") returnvalue = 0.1;
            else if (symbolname == "IgnMastCal.UBattDwellTimeXSP") returnvalue = 0.1;
            else if (symbolname == "AC_ControlCal.K_MinClutchDsblTmeTAB") returnvalue = 0.1;
            else if (symbolname == "obdEvap.DiffR2Result") returnvalue = 0.1;
            else if (symbolname == "obdEvap.p_AirAmbStack") returnvalue = 0.1;
            else if (symbolname == "IgnKnkAdap.fi_OffsetAdap") returnvalue = 0.1;
            else if (symbolname == "FuelDynProt.betaTAB") returnvalue = 0.003921569;
            else if (symbolname == "FuelDynProt.alphaTAB") returnvalue = 0.003921569;
            else if (symbolname == "PurgeProt.m_FuelBuffer") returnvalue = 0.01;
            else if (symbolname == "ElevIdleCal.U_BattSP") returnvalue = 0.1;
            else if (symbolname == "JerkCal.p_AirAmbientTab") returnvalue = 0.0078125;
            else if (symbolname == "JerkCal.p_AirAmbientSP") returnvalue = 0.1;
            else if (symbolname == "JerkCal.n_EngineTab") returnvalue = 0.0078125;
            else if (symbolname == "REPCal.T_AirInletTab") returnvalue = 0.1;
            else if (symbolname == "obdT_Est.TEngTemp") returnvalue = 0.01;
            else if (symbolname == "obdT_Est.TCylTemp") returnvalue = 0.1;
            else if (symbolname == "BoostCal.p_AntiHowlPairSP") returnvalue = 0.1;
            else if (symbolname == "BoostCal.p_AntiHowlTab") returnvalue = 0.1;
            else if (symbolname == "FFFDynCal.X_negFuelLimMAP") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaStartTAB") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP1") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP2") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP3") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP4") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP5") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP6") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP7") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.betaMAP8") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP1") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP2") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP3") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP4") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP5") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP6") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP7") returnvalue = 0.003921569;
            else if (symbolname == "FFFDynCal.alphaMAP8") returnvalue = 0.003921569;
            else if (symbolname == "FFAirCal.fi_offsetXSP") returnvalue = 0.1;
            else if (symbolname == "AirCtrlAdap.m_IpartThrMAP") returnvalue = 0.0078125;
            else if (symbolname == "MAFCal.FrompMulFacTab") returnvalue = 0.01;
            else if (symbolname == "MAFCal.X_pRatioSP") returnvalue = 0.01;
            else if (symbolname == "MAFCal.NormAdjustFacMap") returnvalue = 0.0078125;
            else if (symbolname == "OilTempCal.T_DegreePerRevTAB") returnvalue = 1.525902E-05;
            else if (symbolname == "OilTempCal.T_EngineSpeedIncreaseTAB") returnvalue = 0.0078125;
            else if (symbolname == "OilTempCal.p_AirInletSP") returnvalue = 0.1;
            else if (symbolname == "OilTempCal.T_AirInletIncreaseTAB") returnvalue = 0.0078125;
            else if (symbolname == "OilTempCal.v_VehicleYSP") returnvalue = 0.1;
            else if (symbolname == "OilTempCal.T_SpeedAndLoadDecreaseMap") returnvalue = 0.0078125;
            else if (symbolname == "OilTempCal.X_EstimatedOilTempCoeffTAB") returnvalue = 1.525902E-05;
            else if (symbolname == "KnkFuelProt.FuelAdjust") returnvalue = 0.0009765625;
            else if (symbolname == "FFFuelCal.KnkEnrichmentMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.fi_offsetEnrichEnableMAP") returnvalue = 0.1;
            else if (symbolname == "FFFuelCal.AFRTAB") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.enrFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.EnrFacThrFaultTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.combFacMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.afterStartMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP1") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP2") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP3") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP4") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP5") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP6") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP7") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.TCompMAP8") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.lambdaIntLimTAB") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.InjAngTAB") returnvalue = 0.1;
            else if (symbolname == "FFFuelCal.RestartFacMAP") returnvalue = 0.0009765625;
            else if (symbolname == "FFFuelCal.AfterStRestartFacMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.X_hotSoakAdjustMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.t_JerkDelayTAB") returnvalue = 0.1;
            else if (symbolname == "FFFuelCal.X_LambdaDecRampTAB") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.X_LambdaIncRampTAB") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.LambdaRampTempScaleTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.BlkHeatStartFuelFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "FFFuelCal.BlkHeatAfterStFuelFacTAB") returnvalue = 0.0009765625;
            else if (symbolname == "FFFuelCal.X_DNFuelFacMAP") returnvalue = 0.0009765625;
            else if (symbolname == "FFFuelCal.t_soakSP") returnvalue = 0.1;
            else if (symbolname == "FFFuelCal.BlkHeatReStFuelFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.BlkHeatAftReStFuelFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.Q_LambdaRampSP6") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.LambdaRampTAB") returnvalue = 0.01;
            else if (symbolname == "FFFuelCal.TempEnrichFacMAP") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.X_fuelTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.X_fuelCPTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.X_hotSoakTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.X_fuelBlkHeatTAB") returnvalue = 0.0078125;
            else if (symbolname == "FFFuelCal.BFuelJerkEnrichFacTAB") returnvalue = 0.0078125;
            else if (symbolname == "AirAmbient3.p_AirBefThrBuff") returnvalue = 0.1;
            else if (symbolname == "TankData.V_FuelTankTAB") returnvalue = 0.1;

            /** tot hier **/

            //

            //MAFCal.NormAdjustFacMap (correctionfactor = 1/128)
            //BFuelCal.LambdaOneFacMap (correctionfactor = 1/128)
            //BFuelCal.TempEnrichFacMap (correctionfactor = 1/128)
            //AfterStCal.AmbientMAP (correctionfactor = 1/128)


            return returnvalue;
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

        private bool GetMapUpsideDown(string symbolname)
        {
            return true;
        }

        private bool isSixteenBitTable(string symbolname)
        {
            bool retval = true;
            if (symbolname == "KnkDetCal.RefFactorMap") retval = false;
            else if (symbolname == "BFuelCal.Map") retval = false;
            else if (symbolname == "BFuelCal.StartMap") retval = false;
            else if (symbolname == "TorqueCal.M_IgnInflTorqM") retval = false;
            else if (symbolname == "TCompCal.EnrFacMap") retval = false;
            else if (symbolname == "TCompCal.EnrFacAutMap") retval = false;
            else if (symbolname == "AftSt2ExtraCal.EnrFacMap") retval = false;
            else if (symbolname == "AftSt1ExtraCal.EnrFacMap") retval = false;
            else if (symbolname == "StartCal.HighAltFacMap") retval = false;
            else if (symbolname == "BFuelCal.TempEnrichFacMap") retval = false;
            else if (symbolname == "BFuelCal.E85TempEnrichFacMap") retval = false;
            else if (symbolname == "BFuelCal.LambdaOneFacMap") retval = false;
            else if (symbolname == "MAFCal.NormAdjustFacMap") retval = false;
            else if (symbolname.StartsWith("FuelDynCal.m_FbetaMap")) retval = false;
            else if (symbolname.StartsWith("FuelDynCal.m_FalphaMap")) retval = false;
            else if (symbolname == "CatModCal.TSoakFacMAP") retval = false;
            else if (symbolname == "StartCal.ScaleFacRpmMap") retval = false;
            else if (symbolname == "ECUIDCal.ApplicationFileName") retval = false;
            else if (symbolname == "FFFuelCal.TempEnrichFacMAP") retval = false;
            //else if (symbolname == "PurgeCal.ValveMap16US") retval = false;
            //else if (symbolname == "PurgeCal.ValveMap16EU") retval = false;
            else if (GetSymbolLength(m_symbols, symbolname) % 2 == 1) retval = false;
            else if (GetSymbolLength(m_symbols, symbolname) == 336 && !symbolname.StartsWith("PurgeCal.")) retval = false;
            return retval;
        }

        private byte[] readdatafromfile(string filename, int address, int length)
        {
            if (length <= 0) return new byte[1];
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
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private int[] GetYaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int yaxisaddress = 0;
            int yaxislength = 0;
            bool issixteenbit = true;
            int multiplier = 1;
            if (symbolname == "FuelDynCal.FuelModFacTab") issixteenbit = false;

            if (symbolname == "TrqLimCal.Trq_ManGear")
            {
                retval = new int[8];
                retval.SetValue(0, 0);
                retval.SetValue(1, 1);
                retval.SetValue(2, 2);
                retval.SetValue(3, 3);
                retval.SetValue(4, 4);
                retval.SetValue(5, 5);
                retval.SetValue(6, 6);
                retval.SetValue(-1, 7);
                return retval;
            }

            SymbolAxesTranslator axistranslator = new SymbolAxesTranslator();
            string x_axis = string.Empty;
            string y_axis = string.Empty;
            string x_axis_descr = string.Empty;
            string y_axis_descr = string.Empty;
            string z_axis_descr = string.Empty;
            if (axistranslator.GetAxisSymbols(symbolname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr))
            {
                if (y_axis != "")
                {
                    yaxislength = GetSymbolLength(curSymbols, y_axis);
                    yaxisaddress = (int)GetSymbolAddress(curSymbols, y_axis);
                }
            }

            int number = yaxislength;
            if (yaxislength > 0)
            {
                byte[] axisdata = readdatafromfile(filename, yaxisaddress, yaxislength);
                if (issixteenbit) number /= 2;
                retval = new int[number];
                int offset = 0;
                for (int i = 0; i < yaxislength; i++)
                {

                    if (issixteenbit)
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        byte val2 = (byte)axisdata.GetValue(++i);
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        int value = (ival1 * 256) + ival2;
                        value *= multiplier;
                        if (value > 0x8000)
                        {
                            value = 0x10000 - value;
                            value = -value;
                        }
                        retval.SetValue(value, offset++);
                    }
                    else
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        int ival1 = Convert.ToInt32(val1);
                        ival1 *= multiplier;
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }


        private int[] GetXaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int xaxisaddress = 0;
            int xaxislength = 0;
            bool issixteenbit = true;
            int multiplier = 1;
            if (symbolname == "EvapDiagCal.LeakFacTest2MAT") issixteenbit = false;
            if (symbolname == "EvapDiagCal.LeakFacTest1MAT") issixteenbit = false;
            //      if (symbolname == "FuelDynCal.FuelModFacTab") issixteenbit = false;
            SymbolAxesTranslator axistranslator = new SymbolAxesTranslator();
            string x_axis = string.Empty;
            string y_axis = string.Empty;
            string x_axis_descr = string.Empty;
            string y_axis_descr = string.Empty;
            string z_axis_descr = string.Empty;
            if (axistranslator.GetAxisSymbols(symbolname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr))
            {
                if (x_axis != "")
                {
                    if (x_axis == "BstKnkCal.OffsetXSP")
                    {
                        if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
                    }

                    xaxislength = GetSymbolLength(curSymbols, x_axis);
                    xaxisaddress = (int)GetSymbolAddress(curSymbols, x_axis);
                }
            }


            int number = xaxislength;
            if (xaxislength > 0)
            {
                byte[] axisdata = readdatafromfile(filename, xaxisaddress, xaxislength);
                if (issixteenbit) number /= 2;
                retval = new int[number];
                int offset = 0;
                for (int i = 0; i < xaxislength; i++)
                {

                    if (issixteenbit)
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        byte val2 = (byte)axisdata.GetValue(++i);
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        int value = (ival1 * 256) + ival2;

                        value *= multiplier;
                        retval.SetValue(value, offset++);
                    }
                    else
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        int ival1 = Convert.ToInt32(val1);

                        ival1 *= multiplier;
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }

        private void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry)
        {
            if (address > 0 && address < 0x100000)
            {
                try
                {
                    byte[] beforedata = readdatafromfile(filename, address, length);
                    FileStream fsi1 = File.OpenWrite(filename);
                    BinaryWriter bw1 = new BinaryWriter(fsi1);
                    fsi1.Position = address;
                    for (int i = 0; i < length; i++)
                    {
                        bw1.Write((byte)data.GetValue(i));
                    }
                    fsi1.Flush();
                    bw1.Close();
                    fsi1.Close();
                    fsi1.Dispose();

                    if (m_ProjectTransactionLog != null && DoTransActionEntry)
                    {
                        TransactionEntry tentry = new TransactionEntry(DateTime.Now, address, length, beforedata, data, 0, 0, "");
                        m_ProjectTransactionLog.AddToTransactionLog(tentry);
                        SignalTransactionLogChanged(tentry.SymbolAddress, tentry.Note);
                    }

                }
                catch (Exception E)
                {
                    frmInfoBox info = new frmInfoBox("Failed to write to binary. Is it read-only? Details: " + E.Message);
                }
            }
        }

        void tabdet_onSymbolSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            if (sender is IMapViewer)
            {
                // juiste filename kiezen 
                IMapViewer tabdet = (IMapViewer)sender;

                string note = string.Empty;
                if (m_appSettings.RequestProjectNotes && m_CurrentWorkingProject != "")
                {
                    //request a small note from the user in which he/she can denote a description of the change
                    frmChangeNote changenote = new frmChangeNote();
                    changenote.ShowDialog();
                    note = changenote.Note;
                }

                savedatatobinary(e.SymbolAddress, e.SymbolLength, e.SymbolDate, e.Filename, true, note);
                UpdateChecksum(e.Filename, true);
                //tabdet.Map_content = readdatafromfile(e.Filename, e.SymbolAddress, e.SymbolLength);
            }
        }

        private void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry, string note)
        {
            if (address > 0 && address < 0x100000)
            {
                try
                {
                    byte[] beforedata = readdatafromfile(filename, address, length);
                    FileStream fsi1 = File.OpenWrite(filename);
                    BinaryWriter bw1 = new BinaryWriter(fsi1);
                    fsi1.Position = address;



                    for (int i = 0; i < length; i++)
                    {
                        bw1.Write((byte)data.GetValue(i));
                    }
                    fsi1.Flush();
                    bw1.Close();
                    fsi1.Close();
                    fsi1.Dispose();

                    if (m_ProjectTransactionLog != null && DoTransActionEntry)
                    {
                        TransactionEntry tentry = new TransactionEntry(DateTime.Now, address, length, beforedata, data, 0, 0, note);
                        m_ProjectTransactionLog.AddToTransactionLog(tentry);
                        SignalTransactionLogChanged(tentry.SymbolAddress, tentry.Note);
                    }

                }
                catch (Exception E)
                {
                    frmInfoBox info = new frmInfoBox("Failed to write to binary. Is it read-only? Details: " + E.Message);
                }
            }
        }

        void tabdet_onClose(object sender, EventArgs e)
        {
            // close the corresponding dockpanel
            if (sender is IMapViewer)
            {
                IMapViewer tabdet = (IMapViewer)sender;

                string dockpanelname = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]";
                string dockpanelname2 = "SRAM Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]";
                string dockpanelname3 = "Symbol difference: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]";
                foreach (DevExpress.XtraBars.Docking.DockPanel dp in dockManager1.Panels)
                {
                    if (dp.Text == dockpanelname)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                    else if (dp.Text == dockpanelname2)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                    else if (dp.Text == dockpanelname3)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }

                }
            }
        }

        private void StartTableViewer()
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();

                if (selrows.Length > 0)
                {
                    if (gridViewSymbols.GroupCount > 0)
                    {
                        if (gridViewSymbols.GetRowLevel((int)selrows.GetValue(0)) < gridViewSymbols.GroupCount)
                        {
                            return;
                        }
                    }

                    SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    //DataRowView dr = (DataRowView)gridViewSymbols.GetRow((int)selrows.GetValue(0));


                    if (sh == null) return;

                    string varname = sh.Varname;
                    if (varname.StartsWith("Symbol") && sh.Userdescription != "")
                    {
                        varname = sh.Userdescription;
                    }
                    if (sh.BitMask > 0)
                    {
                        // get all other symbols with the same address
                        StartBitMaskViewer(sh);
                        return;
                    }
                    if (sh.Flash_start_address > m_currentfile_size && !m_connectedToECU)
                    {
                        MessageBox.Show("Symbol outside of flash boundary, probably SRAM only symbol");
                        return;
                    }

                    
                    


                    DevExpress.XtraBars.Docking.DockPanel dockPanel;
                    bool pnlfound = false;
                    try
                    {
                        foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                        {
                            if (pnl.Text == "Symbol: " + varname + " [" + Path.GetFileName(m_currentfile) + "]")
                            {
                                dockPanel = pnl;
                                pnlfound = true;
                                dockPanel.Show();
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    if (!pnlfound)
                    {
                        //dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        dockManager1.BeginUpdate();
                        try
                        {


                            IMapViewer tabdet = new MapViewerEx(); //<GS-03012011>
                            tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                            tabdet.Visible = false;
                            tabdet.Filename = m_currentfile;
                            tabdet.GraphVisible = m_appSettings.ShowGraphs;
                            tabdet.Viewtype = m_appSettings.DefaultViewType;
                            tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                            tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                            tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                            tabdet.Map_name = varname;
                            tabdet.Map_descr = "";//TranslateSymbolName(tabdet.Map_name);
                            tabdet.Map_cat = XDFCategories.Undocumented; //TranslateSymbolNameToCategory(tabdet.Map_name);

                            SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                            string x_axis = string.Empty;
                            string y_axis = string.Empty;
                            string x_axis_descr = string.Empty;
                            string y_axis_descr = string.Empty;
                            string z_axis_descr = string.Empty;
                            axestrans.GetAxisSymbols(varname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                            if (x_axis == "BstKnkCal.OffsetXSP")
                            {
                                if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
                            }

                            tabdet.X_axis_name = x_axis_descr;
                            tabdet.Y_axis_name = y_axis_descr;
                            tabdet.Z_axis_name = z_axis_descr;
                            tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                            tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                            // z, y and z axis to do
                            if (!m_appSettings.NewPanelsFloating)
                            {
                                dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);

                                if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                {
                                    int dw = 650;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 400) dw = 400;
                                    if (dw > 800) dw = 800;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 900);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 500);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                {
                                    int dw = 550;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (dw > 600) dw = 600;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 850);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                {
                                    int dw = 450;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (dw > 400) dw = 400;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 700);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }

                            }
                            else
                            {
                                System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                                dockPanel = dockManager1.AddPanel(floatpoint);
                                if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                {
                                    int dw = 650;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 400) dw = 400;
                                    if (dw > 800) dw = 800;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 900);
                                        tabdet.SetSplitter(0, 0, 280, false, false);

                                    }
                                    else
                                    {

                                        dockPanel.FloatSize = new Size(dw, 500);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                {
                                    int dw = 550;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (dw > 600) dw = 600;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 850);
                                        tabdet.SetSplitter(0, 0, 250, false, false);
                                        //tabdet.SetSurfaceGraphZoom(0.4);
                                    }
                                    else
                                    {

                                        dockPanel.FloatSize = new Size(dw, 450);
                                        //dockPanel.FloatSize = new Size(550, 450);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                {
                                    int dw = 450;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (dw > 400) dw = 400;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 700);
                                        tabdet.SetSplitter(0, 0, 320, false, false);
                                        // tabdet.SetSurfaceGraphZoom(0.5);
                                    }
                                    else
                                    {
                                        // dockPanel.FloatSize = new Size(450, 450);

                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }
                                floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                                while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                                dockPanel.FloatLocation = floatpoint;

                            }
                            dockPanel.Tag = m_currentfile;

                            /*string xdescr = string.Empty;
                            string ydescr = string.Empty;
                            string zdescr = string.Empty;
                            GetAxisDescriptions(m_currentfile, m_symbols, tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                            tabdet.X_axis_name = xdescr;
                            tabdet.Y_axis_name = ydescr;
                            tabdet.Z_axis_name = zdescr;*/
                            int columns = 8;
                            int rows = 8;
                            int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
                            int address = Convert.ToInt32(sh.Flash_start_address);

                            if (tabdet.X_axisvalues.Length > 1) columns = tabdet.X_axisvalues.Length;
                            if (tabdet.Y_axisvalues.Length > 1) rows = tabdet.Y_axisvalues.Length;
                            tablewidth = columns;

                            int sramaddress = 0;// Convert.ToInt32(dr.Row["SRAMADDRESS"].ToString());
                            if (address != 0)
                            {

                                while (address > m_currentfile_size) address -= m_currentfile_size;
                                tabdet.Map_address = address;
                                tabdet.Map_sramaddress = sramaddress;
                                int length = Convert.ToInt32(sh.Length);
                                tabdet.Map_length = length;
                                byte[] mapdata = readdatafromfile(m_currentfile, address, length);
                                tabdet.Map_content = mapdata;
                                tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                                tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                                tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);

                                if (m_connectedToECU)
                                {
                                    tabdet.IsRAMViewer = true;
                                    tabdet.OnlineMode = true;
                                }

                                tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));
                                tabdet.Dock = DockStyle.Fill;
                                tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                                tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                                tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                                tabdet.onSymbolRead += new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);

                                //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                                //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                                //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                                //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                                //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                                //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                                //tabdet.onAxisEditorRequested += new MapViewer.AxisEditorRequested(tabdet_onAxisEditorRequested);
                                bool isDocked = false;
                                dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_currentfile) + "]";
                                if (m_appSettings.AutoDockSameSymbol)
                                {
                                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                    {
                                        if (pnl.Text.StartsWith("Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            isDocked = true;
                                            break;
                                        }
                                    }
                                }
                                if (!isDocked)
                                {
                                    if (m_appSettings.AutoDockSameFile)
                                    {
                                        foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                        {
                                            if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                            {
                                                dockPanel.DockAsTab(pnl, 0);
                                                isDocked = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (!isDocked)
                                {

                                    //dockPanel.DockAsTab(dockPanel1);
                                    /*dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_currentfile) + "]";
                                    int width = 400;
                                    if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                    {
                                        width = 500;
                                    }
                                    if (m_appSettings.AutoSizeNewWindows)
                                    {
                                        //if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                        {
                                            if (tabdet.X_axisvalues.Length > 0)
                                            {
                                                width = 30 + ((tabdet.X_axisvalues.Length + 1) * 40);
                                            }
                                            else
                                            {
                                                width = 30 + ((columns + 1) * 40);
                                                //width = this.Width - dockSymbols.Width - 10;
                                            }
                                            if (width < 500) width = 500;
                                            if (width > 800) width = 800;
                                        }
                                    }
                                    dockPanel.Width = width;*/
                                    int width = 400;
                                    if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                    {
                                        width = 500;
                                    }
                                    if (m_appSettings.AutoSizeNewWindows)
                                    {
                                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                        {
                                            if (tabdet.X_axisvalues.Length > 0)
                                            {
                                                width = 30 + ((tabdet.X_axisvalues.Length + 1) * 40);
                                            }
                                            else
                                            {
                                                //width = this.Width - dockSymbols.Width - 10;
                                            }
                                            if (width < 500) width = 500;
                                            if (width > 800) width = 800;
                                        }
                                        else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                        {
                                            if (tabdet.X_axisvalues.Length > 0)
                                            {
                                                width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                            }
                                            else
                                            {
                                                //width = this.Width - dockSymbols.Width - 10;
                                            }
                                            if (width < 450) width = 450;
                                            if (width > 600) width = 600;

                                        }
                                        else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                        {
                                            if (tabdet.X_axisvalues.Length > 0)
                                            {
                                                width = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                            }
                                            else
                                            {
                                                //width = this.Width - dockSymbols.Width - 10;
                                            }
                                            if (width < 400) width = 400;
                                            if (width > 600) width = 600;


                                        }
                                    }
                                    dockPanel.Width = width;
                                }
                                dockPanel.Controls.Add(tabdet);
                            }
                            tabdet.Visible = true;
                        }
                        catch (Exception newdockE)
                        {
                            Console.WriteLine(newdockE.Message);
                        }
                        dockManager1.EndUpdate();
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
            }

        }

        void tabdet_onSymbolRead(object sender, IMapViewer.ReadSymbolEventArgs e)
        {
            // reload data from the file
            IMapViewer tabdet = (IMapViewer)sender;
            tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
            tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
            Int64 symaddress = GetSymbolAddress(m_symbols, tabdet.Map_name);
            int symlen = GetSymbolLength(m_symbols, tabdet.Map_name);
            int columns = 8;
            int rows = 8;
            int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
            int address = Convert.ToInt32(symaddress);

            if (tabdet.X_axisvalues.Length > 1) columns = tabdet.X_axisvalues.Length;
            if (tabdet.Y_axisvalues.Length > 1) rows = tabdet.Y_axisvalues.Length;
            tablewidth = columns;

            int sramaddress = 0;// Convert.ToInt32(dr.Row["SRAMADDRESS"].ToString());
            if (address != 0)
            {

                while (address > m_currentfile_size) address -= m_currentfile_size;
                tabdet.Map_address = address;
                tabdet.Map_sramaddress = sramaddress;
                int length = Convert.ToInt32(symlen);
                tabdet.Map_length = length;
                byte[] mapdata = readdatafromfile(m_currentfile, address, length);
                tabdet.Map_content = mapdata;
                tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                tabdet.ShowTable(tablewidth, isSixteenBitTable(tabdet.Map_name));
            }

        }

        private void StartBitMaskViewer(SymbolHelper sh)
        {
            // check for SRAM file and/or connected ECU if symbol is outside flash area
            uint data = 0;
            if (sh.Flash_start_address > m_currentfile_size)
            {
                //TODO: Implement
                if (t8can.isOpen())
                {
                    bool success = false;
                    byte[] bdat = t8can.readMemory((int)sh.Flash_start_address, 2, out success);
                    if (success)
                    {
                        data = Convert.ToUInt32(bdat[0]) * 256;
                        data += Convert.ToUInt32(bdat[1]);
                    }
                    else
                    {
                        MessageBox.Show("Symbol outside of flash boundary and failed to read symbol from ECU");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Symbol outside of flash boundary and no connection to ECU available");
                    return;
                }
            }
            else
            {
                byte[] bdat = readdatafromfile(m_currentfile, (int)sh.Flash_start_address, 2);
                data = Convert.ToUInt32(bdat[0]) * 256;
                data += Convert.ToUInt32(bdat[1]);
            }
            // get all other symbols with the same address
            SymbolCollection shview = new SymbolCollection();
            foreach (SymbolHelper shl in m_symbols)
            {
                if (shl.Flash_start_address == sh.Flash_start_address)
                {
                    shview.Add(shl);
                }
            }
            // start a viewer with the collection and the real data 
            frmBitmaskViewer view = new frmBitmaskViewer();
            view.SetInformation(shview, data);
            if (view.ShowDialog() == DialogResult.OK)
            {
                // save the data to the file && update checksum if set to auto
                byte[] bdata = new byte[2];
                byte b1 = Convert.ToByte(view.Data / 256);
                byte b2 = Convert.ToByte(view.Data - (int)b1 * 256);
                bdata.SetValue(b1, 0);
                bdata.SetValue(b2, 1);
                if (sh.Flash_start_address > m_currentfile_size)
                {
                    // save to ECU ... if possible
                }
                else
                {
                    savedatatobinary((int)sh.Flash_start_address, 2, bdata, m_currentfile, true);
                    if (m_appSettings.AutoChecksum) UpdateChecksum(m_currentfile, true);
                }
            }
        }

        void mv_onSurfaceGraphViewChangedEx(object sender, IMapViewer.SurfaceGraphViewChangedEventArgsEx e)
        {
            if (m_appSettings.SynchronizeMapviewers)
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is IMapViewer)
                        {
                            if (c != sender)
                            {
                                IMapViewer vwr = (IMapViewer)c;
                                if (vwr.Map_name == e.Mapname)
                                {
                                    vwr.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is IMapViewer)
                                {
                                    if (c2 != sender)
                                    {
                                        IMapViewer vwr2 = (IMapViewer)c2;
                                        if (vwr2.Map_name == e.Mapname)
                                        {
                                            vwr2.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is IMapViewer)
                                {
                                    if (c3 != sender)
                                    {
                                        IMapViewer vwr3 = (IMapViewer)c3;
                                        if (vwr3.Map_name == e.Mapname)
                                        {
                                            vwr3.SetSurfaceGraphViewEx(e.DepthX, e.DepthY, e.Zoom, e.Rotation, e.Elevation);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        void tabdet_onSelectionChanged(object sender, IMapViewer.CellSelectionChangedEventArgs e)
        {
            //<GS-22042010>
            // sync mapviewers maybe?
            if (m_appSettings.SynchronizeMapviewers)
            {
                // andere cell geselecteerd, doe dat ook bij andere viewers met hetzelfde symbool (mapname)
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is IMapViewer)
                        {
                            if (c != sender)
                            {
                                IMapViewer vwr = (IMapViewer)c;
                                if (vwr.Map_name == e.Mapname)
                                {
                                    vwr.SelectCell(e.Rowhandle, e.Colindex);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is IMapViewer)
                                {
                                    if (c2 != sender)
                                    {
                                        IMapViewer vwr2 = (IMapViewer)c2;
                                        if (vwr2.Map_name == e.Mapname)
                                        {
                                            vwr2.SelectCell(e.Rowhandle, e.Colindex);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is IMapViewer)
                                {
                                    if (c3 != sender)
                                    {
                                        IMapViewer vwr3 = (IMapViewer)c3;
                                        if (vwr3.Map_name == e.Mapname)
                                        {
                                            vwr3.SelectCell(e.Rowhandle, e.Colindex);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void CreateRepositoryItem(string filename)
        {
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(filename));
            dt.Columns.Add("Varname");
            dt.Columns.Add("Description");
            dt.Columns.Add("Flash_start_address", Type.GetType("System.Int32"));
            dt.Columns.Add("Internal_address", Type.GetType("System.Int32"));
            dt.Columns.Add("Length", Type.GetType("System.Int32"));
            dt.Columns.Add("Start_address", Type.GetType("System.Int32"));
            dt.Columns.Add("Symbol_number", Type.GetType("System.Int32"));
            dt.Columns.Add("Symbol_number_ECU", Type.GetType("System.Int32"));
            dt.Columns.Add("Userdescription");

            foreach (SymbolHelper sh in m_symbols)
            {
                dt.Rows.Add(sh.Varname, sh.Description, sh.Flash_start_address, sh.Internal_address, sh.Length, sh.Start_address, sh.Symbol_number, sh.Symbol_number_ECU, sh.Userdescription);
            }
            dt.WriteXml(filename);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ribbonControl1.Minimized = true;
            try
            {
                IMapViewer mv = new MapViewerEx();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            LoadLayoutFiles();
            InitSkins();
            SetFilterMode();
            if (m_startFromCommandLine)
            {
                if (File.Exists(m_commandLineFile))
                {
                    m_currentfile = m_commandLineFile;
                    TryToOpenFile(m_commandLineFile, out m_symbols, m_currentfile_size);

                    this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
                    barFilenameText.Caption = Path.GetFileName(m_currentfile);
                    gridControlSymbols.DataSource = m_symbols;
                    m_appSettings.Lastfilename = m_currentfile;
                    gridViewSymbols.BestFitColumns();
                    UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);


                }
            }
            else if (m_appSettings.AutoLoadLastFile)
            {
                if (m_appSettings.LastOpenedType == 0)
                {
                    if (m_appSettings.Lastfilename != "")
                    {
                        if (File.Exists(m_appSettings.Lastfilename))
                        {
                            m_currentfile = m_appSettings.Lastfilename;
                            TryToOpenFile(m_appSettings.Lastfilename, out m_symbols, m_currentfile_size);

                            this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
                            barFilenameText.Caption = Path.GetFileName(m_currentfile);
                            gridControlSymbols.DataSource = m_symbols;
                            m_appSettings.Lastfilename = m_currentfile;
                            gridViewSymbols.BestFitColumns();
                            UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);


                        }
                    }
                }
                else if (m_appSettings.Lastprojectname != "")
                {
                    OpenProject(m_appSettings.Lastprojectname);
                }
            }
            if (m_appSettings.ShowMenu)
            {
                ribbonControl1.Minimized = false;
            }
            if (m_appSettings.DebugMode)
            {
                ribbonDebug.Visible = true;
            }
            
            LoadMyMaps();
        }

        private void StartTableViewer(string symbolname)
        {
            bool _vwrstarted = false;
            if (GetSymbolAddress(m_symbols, symbolname) > 0)
            {
                gridViewSymbols.ActiveFilter.Clear(); // clear filter

                SymbolCollection sc = (SymbolCollection)gridControlSymbols.DataSource;
                int rtel = 0;
                foreach (SymbolHelper sh in sc)
                {
                    if (sh.Varname == symbolname)
                    {
                        try
                        {
                            int rhandle = gridViewSymbols.GetRowHandle(rtel);
                            gridViewSymbols.OptionsSelection.MultiSelect = true;
                            gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                            gridViewSymbols.ClearSelection();
                            gridViewSymbols.SelectRow(rhandle);
                            //gridViewSymbols.SelectRows(rhandle, rhandle);
                            gridViewSymbols.MakeRowVisible(rhandle, true);
                            gridViewSymbols.FocusedRowHandle = rhandle;
                            //gridViewSymbols.SelectRange(rhandle, rhandle);
                            _vwrstarted = true;
                            StartTableViewer();
                            break;
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show(E.Message);
                        }
                    }

                    rtel++;
                }
                if (!_vwrstarted)
                {
                    rtel = 0;
                    foreach (SymbolHelper sh in sc)
                    {
                        if (sh.Userdescription == symbolname)
                        {
                            try
                            {
                                int rhandle = gridViewSymbols.GetRowHandle(rtel);
                                gridViewSymbols.OptionsSelection.MultiSelect = true;
                                gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                                gridViewSymbols.ClearSelection();
                                gridViewSymbols.SelectRow(rhandle);
                                //gridViewSymbols.SelectRows(rhandle, rhandle);
                                gridViewSymbols.MakeRowVisible(rhandle, true);
                                gridViewSymbols.FocusedRowHandle = rhandle;
                                //gridViewSymbols.SelectRange(rhandle, rhandle);
                                _vwrstarted = true;
                                StartTableViewer();
                                break;
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show(E.Message);
                            }
                        }
                        rtel++;
                    }
                }
            }
            else
            {
                MessageBox.Show("Symbol " + symbolname + " does not exist in this file");
            }
        }

        private string TranslateSymbolName(string symbolname)
        {
            return symbolname;
        }

        private void StartCompareMapViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength, SymbolCollection curSymbols)
        {
            try
            {
                DevExpress.XtraBars.Docking.DockPanel dockPanel;
                bool pnlfound = false;
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {

                    if (pnl.Text == "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]")
                    {
                        dockPanel = pnl;
                        pnlfound = true;
                        dockPanel.Show();
                        // nog data verversen?
                        foreach (Control c in dockPanel.Controls)
                        {
                            /* if (c is MapViewer)
                             {
                                 MapViewer tempviewer = (MapViewer)c;
                                 tempviewer.Map_content
                             }*/
                        }
                    }
                }
                if (!pnlfound)
                {
                    dockManager1.BeginUpdate();
                    try
                    {
                        dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        dockPanel.Tag = Filename;// m_currentfile; changed 24/01/2008
                        IMapViewer tabdet = new MapViewerEx(); //<GS-03012011>
                        tabdet.SetViewSize(m_appSettings.DefaultViewSize);

                        //tabdet.IsHexMode = barViewInHex.Checked;
                        tabdet.Viewtype = m_appSettings.DefaultViewType;
                        tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                        tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                        tabdet.GraphVisible = m_appSettings.ShowGraphs;
                        tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                        tabdet.Filename = Filename;
                        tabdet.Map_name = SymbolName;
                        tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                        tabdet.Map_cat = XDFCategories.Undocumented;
                        tabdet.X_axisvalues = GetXaxisValues(Filename, curSymbols, tabdet.Map_name);
                        tabdet.Y_axisvalues = GetYaxisValues(Filename, curSymbols, tabdet.Map_name);
                        string xdescr = string.Empty;
                        string ydescr = string.Empty;
                        string zdescr = string.Empty;
                        GetAxisDescriptions(Filename, curSymbols, tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                        tabdet.X_axis_name = xdescr;
                        tabdet.Y_axis_name = ydescr;
                        tabdet.Z_axis_name = zdescr;

                        //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                        int columns = 8;
                        int rows = 8;
                        int tablewidth = GetTableMatrixWitdhByName(Filename, curSymbols, tabdet.Map_name, out columns, out rows);
                        int address = Convert.ToInt32(SymbolAddress);
                        if (address != 0)
                        {
                            while (address > m_currentfile_size) address -= m_currentfile_size;
                            tabdet.Map_address = address;
                            int length = SymbolLength;
                            tabdet.Map_length = length;
                            byte[] mapdata = readdatafromfile(Filename, address, length);
                            tabdet.Map_content = mapdata;
                            tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, isSixteenBitTable(SymbolName));
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            //tabdet.onSymbolRead +=new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);
                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]";
                            //dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 40);
                            }
                            else
                            {
                                //dockPanel.Width = this.Width - dockSymbols.Width - 10;

                            }
                            if (dockPanel.Width < 400) dockPanel.Width = 400;
                            //                    dockPanel.Width = 400;

                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        isDocked = true;
                                        break;
                                    }
                                }
                            }
                            if (!isDocked)
                            {
                                if (m_appSettings.AutoDockSameFile)
                                {
                                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            isDocked = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!isDocked)
                            {
                                int width = 400;
                                if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                {
                                    width = 500;
                                }
                                if (m_appSettings.AutoSizeNewWindows)
                                {
                                    if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                    {
                                        if (tabdet.X_axisvalues.Length > 0)
                                        {
                                            width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                        }
                                        else
                                        {
                                            //width = this.Width - dockSymbols.Width - 10;
                                        }
                                        if (width < 500) width = 500;
                                        if (width > 800) width = 800;
                                    }
                                    else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                    {
                                        if (tabdet.X_axisvalues.Length > 0)
                                        {
                                            width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                        }
                                        else
                                        {
                                            //width = this.Width - dockSymbols.Width - 10;
                                        }
                                        if (width < 450) width = 450;
                                        if (width > 600) width = 600;

                                    }
                                    else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                    {
                                        if (tabdet.X_axisvalues.Length > 0)
                                        {
                                            width = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                        }
                                        else
                                        {
                                            //width = this.Width - dockSymbols.Width - 10;
                                        }
                                        if (width < 400) width = 400;
                                        if (width > 600) width = 600;


                                    }
                                }
                                dockPanel.Width = width;
                                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                            }
                            dockPanel.Controls.Add(tabdet);
                            // dock to another panel?
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    dockManager1.EndUpdate();
                }
            }
            catch (Exception startnewcompareE)
            {
                Console.WriteLine(startnewcompareE.Message);
            }

        }

        private void StartCompareDifferenceViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {

                if (pnl.Text == "Symbol difference: " + SymbolName + " [" + Path.GetFileName(Filename) + "]")
                {
                    dockPanel = pnl;
                    pnlfound = true;
                    dockPanel.Show();
                }
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                    dockPanel.Tag = Filename;
                    IMapViewer tabdet = new MapViewerEx();
                    //tabdet.IsHexMode = true; // always in hexmode!
                    tabdet.Viewtype = m_appSettings.DefaultViewType;
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Filename = Filename;
                    tabdet.Map_name = SymbolName;
                    //tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                    //tabdet.Map_cat = TranslateSymbolNameToCategory(tabdet.Map_name);
                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    GetAxisDescriptions(m_currentfile, m_symbols, tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;

                    //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
                    int address = Convert.ToInt32(SymbolAddress);
                    if (address != 0)
                    {
                        while (address > m_currentfile_size) address -= m_currentfile_size;
                        tabdet.Map_address = address;
                        int length = SymbolLength;
                        tabdet.Map_length = length;
                        byte[] mapdata = readdatafromfile(Filename, address, length);
                        byte[] mapdata2 = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, SymbolName), GetSymbolLength(m_symbols, SymbolName));
                        byte[] mapdataorig = readdatafromfile(Filename, address, length);

                        tabdet.Map_original_content = mapdataorig;
                        tabdet.Map_compare_content = mapdata2;

                        if (mapdata.Length == mapdata2.Length)
                        {
                            if (isSixteenBitTable(SymbolName))
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt += 2)
                                {
                                    int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                    int value2 = Convert.ToInt16(mapdata2.GetValue(bt)) * 256 + Convert.ToInt16(mapdata2.GetValue(bt + 1));

                                    value1 = Math.Abs((int)value1 - (int)value2);
                                    byte v1 = (byte)(value1 / 256);
                                    byte v2 = (byte)(value1 - (int)v1 * 256);
                                    mapdata.SetValue(v1, bt);
                                    mapdata.SetValue(v2, bt + 1);
                                }
                            }
                            else
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt++)
                                {
                                    Console.WriteLine("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }
                            tabdet.UseNewCompare = true;
                            tabdet.Map_content = mapdata;
                            tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, isSixteenBitTable(SymbolName));
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            
                            //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                            //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                            //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                            //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                            //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                            //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol difference: " + SymbolName + " [" + Path.GetFileName(Filename) + "]";
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol difference: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        isDocked = true;
                                        break;
                                    }
                                }
                            }
                            if (!isDocked)
                            {
                                if (m_appSettings.AutoDockSameFile)
                                {
                                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            isDocked = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!isDocked)
                            {
                                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                                if (m_appSettings.AutoSizeNewWindows)
                                {
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 40);
                                    }
                                    else
                                    {
                                        //dockPanel.Width = this.Width - dockSymbols.Width - 10;

                                    }
                                }
                                if (dockPanel.Width < 400) dockPanel.Width = 400;

                                //                    dockPanel.Width = 400;
                            }
                            dockPanel.Controls.Add(tabdet);

                        }
                        else
                        {
                            MessageBox.Show("Map lengths don't match...");
                        }
                    }
                }
                catch (Exception E)
                {

                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void tabdet_onSymbolSelect(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            Console.WriteLine(e.SymbolName);
            if (!e.ShowDiffMap)
            {
                StartTableViewer(e.SymbolName);
                StartCompareMapViewer(e.SymbolName, e.Filename, e.SymbolAddress, e.SymbolLength, e.Symbols);
            }
            else
            {
                // show difference map
                StartCompareDifferenceViewer(e.SymbolName, e.Filename, e.SymbolAddress, e.SymbolLength);
            }
        }

        private bool CompareSymbolToCurrentFile(string symbolname, int address, int length, string filename, out double diffperc, out int diffabs, out double diffavg)
        {
            diffperc = 0;
            diffabs = 0;
            diffavg = 0;

            double totalvalue1 = 0;
            double totalvalue2 = 0;
            bool retval = true;

            if (address > 0)
            {
                while (address > m_currentfile_size) address -= m_currentfile_size;
                int curaddress = (int)GetSymbolAddress(m_symbols, symbolname);
                while (curaddress > m_currentfile_size) curaddress -= m_currentfile_size;
                int curlength = GetSymbolLength(m_symbols, symbolname);
                byte[] curdata = readdatafromfile(m_currentfile, curaddress, curlength);
                byte[] compdata = readdatafromfile(filename, address, length);
                if (curdata.Length != compdata.Length)
                {
                    return false;
                }
                for (int offset = 0; offset < curdata.Length; offset++)
                {
                    if ((byte)curdata.GetValue(offset) != (byte)compdata.GetValue(offset))
                    {
                        retval = false;
                        diffabs++;
                    }
                    totalvalue1 += (byte)curdata.GetValue(offset);
                    totalvalue2 += (byte)compdata.GetValue(offset);
                }
                if (curdata.Length > 0)
                {
                    totalvalue1 /= curdata.Length;
                    totalvalue2 /= compdata.Length;
                }
            }
            diffavg = totalvalue1 - totalvalue2;
            if (isSixteenBitTable(symbolname))
            {
                diffabs /= 2;
            }

            diffperc = (diffabs * 100) / length;

            return retval;
        }

        private void CompareToFile(string filename)
        {
            if (m_currentfile != "")
            {
                if (m_symbols.Count > 0)
                {
                    dockManager1.BeginUpdate();
                    try
                    {
                        DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        CompareResults tabdet = new CompareResults();
                        tabdet.Dock = DockStyle.Fill;
                        tabdet.Filename = filename;
                        tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelect);
                        dockPanel.Controls.Add(tabdet);
                        dockPanel.Text = "Compare results: " + Path.GetFileName(filename);
                        dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);

                        dockPanel.Width = 700;


                        SymbolCollection compare_symbols = new SymbolCollection();
                        FileInfo fi = new FileInfo(filename);
                        Console.WriteLine("Opening compare file");
                        TryToOpenFile(filename, out compare_symbols, (int)fi.Length);
                        System.Windows.Forms.Application.DoEvents();
                        Console.WriteLine("Start compare");
                        SetProgress("Start comparing symbols in files");
                        SetProgressPercentage(0);
                        System.Windows.Forms.Application.DoEvents();
                        System.Data.DataTable dt = new System.Data.DataTable();
                        dt.Columns.Add("SYMBOLNAME");
                        dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                        dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                        dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                        dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                        dt.Columns.Add("DESCRIPTION");
                        dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                        dt.Columns.Add("CATEGORY", Type.GetType("System.Int32")); //0
                        dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                        dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                        dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                        dt.Columns.Add("CATEGORYNAME");
                        dt.Columns.Add("SUBCATEGORYNAME");
                        dt.Columns.Add("SymbolNumber1", Type.GetType("System.Int32"));
                        dt.Columns.Add("SymbolNumber2", Type.GetType("System.Int32"));
                        dt.Columns.Add("Userdescription");
                        dt.Columns.Add("MissingInOriFile", Type.GetType("System.Boolean"));
                        dt.Columns.Add("MissingInCompareFile", Type.GetType("System.Boolean"));

                        string category = "";
                        string ht = string.Empty;
                        double diffperc = 0;
                        int diffabs = 0;
                        double diffavg = 0;
                        int percentageDone = 0;
                        int symNumber = 0;

                        XDFCategories cat = XDFCategories.Undocumented;
                        XDFSubCategory subcat = XDFSubCategory.Undocumented;
                        if (compare_symbols.Count > 0)
                        {
                            CompareResults cr = new CompareResults();
                            cr.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                            cr.SetFilterMode(m_appSettings.ShowAddressesInHex);
                            SymbolTranslator st = new SymbolTranslator();
                            int symidx = 0;

                            foreach (SymbolHelper sh_compare in compare_symbols)
                            {
                                try
                                {
                                    symNumber++;
                                    percentageDone = (symNumber * 50) / compare_symbols.Count;
                                    if (Convert.ToInt32(barProgress.EditValue) != percentageDone)
                                    {
                                        barProgress.EditValue = percentageDone;
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }
                                
                                string compareName = sh_compare.Varname;
                                if (compareName.StartsWith("Symbolnumber")) compareName = sh_compare.Userdescription;

                                foreach (SymbolHelper sh_org in m_symbols)
                                {
                                    string originalName = sh_org.Varname;
                                    if (originalName.StartsWith("Symbolnumber")) originalName = sh_org.Userdescription;

                                    if (compareName.Equals(originalName) && compareName != String.Empty)
                                    {
                                        if (sh_compare.Flash_start_address > 0 && sh_compare.Flash_start_address < 0x100000)
                                        {
                                            if (sh_org.Flash_start_address > 0 && sh_org.Flash_start_address < 0x100000)
                                            {
                                                if (!CompareSymbolToCurrentFile(compareName, (int)sh_compare.Flash_start_address, sh_compare.Length, filename, out diffperc, out diffabs, out diffavg))
                                                {
                                                    category = "";
                                                    if (sh_org.Varname.Contains("."))
                                                    {
                                                        try
                                                        {
                                                            category = sh_org.Varname.Substring(0, sh_org.Varname.IndexOf("."));
                                                        }
                                                        catch (Exception cE)
                                                        {
                                                            Console.WriteLine("Failed to assign category to symbol: " + sh_org.Varname + " err: " + cE.Message);
                                                        }
                                                    }
                                                    else if (sh_org.Userdescription.Contains("."))
                                                    {
                                                        try
                                                        {
                                                            category = sh_org.Userdescription.Substring(0, sh_org.Userdescription.IndexOf("."));
                                                        }
                                                        catch (Exception cE)
                                                        {
                                                            Console.WriteLine("Failed to assign category to symbol: " + sh_org.Userdescription + " err: " + cE.Message);
                                                        }
                                                    }

                                                    dt.Rows.Add(originalName, sh_compare.Start_address, sh_compare.Flash_start_address, sh_compare.Length, sh_compare.Length, st.TranslateSymbolToHelpText(compareName, out ht, out cat, out subcat), false, 0, diffperc, diffabs, diffavg, category, "", sh_org.Symbol_number, sh_compare.Symbol_number, sh_org.Userdescription);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                symidx++;
                            }
                            symNumber = 0;
                            string varnameori = string.Empty;
                            string varnamecomp = string.Empty;
                            foreach (SymbolHelper shtest in compare_symbols)
                            {
                                try
                                {
                                    symNumber++;
                                    percentageDone = 50 + (symNumber * 25) / compare_symbols.Count;
                                    if (Convert.ToInt32(barProgress.EditValue) != percentageDone)
                                    {
                                        barProgress.EditValue = percentageDone;
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }
                                bool _foundSymbol = false;
                                varnamecomp = shtest.Varname;
                                if (varnamecomp.StartsWith("Symbolnumber")) varnamecomp = shtest.Userdescription;
                                if (IsSymbolCalibration(varnamecomp))
                                {
                                    foreach (SymbolHelper shoritest in m_symbols)
                                    {
                                        varnameori = shoritest.Varname;
                                        if (varnameori.StartsWith("Symbolnumber")) varnameori = shoritest.Userdescription;

                                        if (varnamecomp == varnameori)
                                        {
                                            _foundSymbol = true;
                                            break;
                                        }
                                    }
                                    if (!_foundSymbol)
                                    {
                                        // add this symbol to the MissingInOriCollection
                                        dt.Rows.Add(varnamecomp, shtest.Start_address, shtest.Flash_start_address, shtest.Length, shtest.Length, st.TranslateSymbolToHelpText(varnamecomp, out ht, out cat, out subcat), false, 0, 0, 0, 0, "Missing in original", "", 0, shtest.Symbol_number, shtest.Userdescription, true, false);
                                    }
                                }
                            }
                            symNumber = 0;
                            foreach (SymbolHelper shtest in m_symbols)
                            {
                                try
                                {
                                    symNumber++;
                                    percentageDone = 75 + (symNumber * 25) / compare_symbols.Count;
                                    if (Convert.ToInt32(barProgress.EditValue) != percentageDone)
                                    {
                                        barProgress.EditValue = percentageDone;
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine(E.Message);
                                }
                                bool _foundSymbol = false;
                                varnamecomp = shtest.Varname;
                                if (varnamecomp.StartsWith("Symbolnumber")) varnamecomp = shtest.Userdescription;
                                if (IsSymbolCalibration(varnamecomp))
                                {
                                    foreach (SymbolHelper shoritest in compare_symbols)
                                    {
                                        varnameori = shoritest.Varname;
                                        if (varnameori.StartsWith("Symbolnumber")) varnameori = shoritest.Userdescription;

                                        if (varnamecomp == varnameori)
                                        {
                                            _foundSymbol = true;
                                            break;
                                        }
                                    }
                                    if (!_foundSymbol)
                                    {
                                        // add this symbol to the MissingInCompCollection
                                        dt.Rows.Add(varnamecomp, shtest.Start_address, shtest.Flash_start_address, shtest.Length, shtest.Length, st.TranslateSymbolToHelpText(varnamecomp, out ht, out cat, out subcat), false, 0, 0, 0, 0, "Missing in compare", "", 0, shtest.Symbol_number, shtest.Userdescription, false, true);
                                    }
                                }
                            }

                            tabdet.OriginalSymbolCollection = m_symbols;
                            tabdet.OriginalFilename = m_currentfile;
                            tabdet.CompareFilename = filename;
                            tabdet.CompareSymbolCollection = compare_symbols;
                            tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
                            tabdet.gridControl1.DataSource = dt.Copy();
                            SetProgressIdle();
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    dockManager1.EndUpdate();
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // compare current files content to another binaries content
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Trionic 8 binaries|*.bin";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CompareToFile(openFileDialog1.FileName);
                
            }
        }

        private bool IsSymbolCalibration(string symbolname)
        {
            //<GS-23022011> maybe rewrite for Trionic 8
            if (symbolname.Contains("Cal.")) return true;
            if (symbolname.Contains("Cal1.")) return true;
            if (symbolname.Contains("Cal2.")) return true;
            if (symbolname.Contains("Cal3.")) return true;
            if (symbolname.Contains("Cal4.")) return true;
            if (symbolname.StartsWith("X_Acc")) return true;
            return false;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Verify checksums
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(m_currentfile))
                {
                    UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);
                }
            }

        }

        private Int64 GetInt64FromFile(string filename, int offset)
        {
            Int64 retval = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(offset, SeekOrigin.Begin);
                retval = Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256 * 256;
                retval += Convert.ToInt64(br.ReadByte()) * 256;
                retval += Convert.ToInt64(br.ReadByte());
            }
            fsread.Close();
            return retval;
        }

        private int GetChecksumAreaOffset(string filename)
        {
            int retval = 0;
            if (filename == "") return retval;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(0x20140, SeekOrigin.Begin);
                retval = (int)br.ReadByte() * 256 * 256 * 256;
                retval += (int)br.ReadByte() * 256 * 256;
                retval += (int)br.ReadByte() * 256;
                retval += (int)br.ReadByte();
            }
            fsread.Close();
            return retval;
        }

        int GetChecksumLong(string filename, int offset)
        {
            int retval = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fsread.Seek(offset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                retval = (int)br.ReadByte() * 256 * 256 * 256;
                retval += (int)br.ReadByte() * 256 * 256;
                retval += (int)br.ReadByte() * 256;
                retval += (int)br.ReadByte();
            }
            fsread.Close();
            return retval;
        }

        int GetChecksumShort(string filename, int offset)
        {
            int retval = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fsread.Seek(offset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                retval = (int)br.ReadByte() * 256;
                retval += (int)br.ReadByte();
            }
            fsread.Close();
            return retval;
        }

        private int CalculateChecksumSmall(string filename, int start, int end)
        {
            int retval = 0;

            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fsread.Seek(start, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                for (int i = start; i <= end; i += 2)
                {
                    if (i == end)
                    {
                        int tempvalue = (int)br.ReadByte();
                        retval += tempvalue;
                    }
                    else
                    {
                        int tempvalue = (int)br.ReadByte() * 256;
                        tempvalue += (int)br.ReadByte();
                        retval += tempvalue;
                    }
                }
            }
            fsread.Close();
            //  Console.WriteLine("Checksum 2 byte from " + start.ToString("X6") + " to " + end.ToString("X6") + " = " + retval.ToString("X4"));
            return retval;
        }

        private int CalculateChecksum(string filename, int start, int end)
        {
            int retval = 0;

            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fsread.Seek(start, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                for (int i = start; i <= end; i += 4)
                {
                    int tempvalue = (int)br.ReadByte() * 256 * 256 * 256;
                    tempvalue += (int)br.ReadByte() * 256 * 256;
                    tempvalue += (int)br.ReadByte() * 256;
                    tempvalue += (int)br.ReadByte();
                    retval += tempvalue;
                }
            }
            fsread.Close();
            // Console.WriteLine("Checksum 4 byte from " + start.ToString("X6") + " to " + end.ToString("X6") + " = " + retval.ToString("X8"));
            return retval;
        }

        private byte[] CalculateLayer1ChecksumMD5(string filename, int OffsetLayer1, bool forceSilent)
        {
            /*
1.	calculate checksum pointer
2.	Checksum is 2 level based on Message Digest 5 algorithm
3.	Pointer = @ 0x20140 and is a 4 byte pointer
4.	Use MD5 to make 16 bytes digest from any string
5.	name checksum pointer CHPTR
6.	checksum area 1 ranges from 20000h to CHPTR – 20000h- 1
7.	Create an MD5 hash from this string (20000h – (CHPTR – 20000h – 1))
    a.	MD5Init(Context)
    b.	MD5Update(Context, buffer, size)
    c.	MD5Final(Context, Md5Seed)
    d.	sMd5Seed = MD5Print(Md5Seed)
    e.	sMd5Seed = 16 bytes hex, so 32 bytes.
    f.	Now crypt sMd5Seed: xor every byte with 21h, then substract D6h (minus)
    g.	These 16 bytes are from CHPTR + 2 in the bin!!!! This is checksum level 1 !!
             * */
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            int len = OffsetLayer1 - 0x20000;//- 1;
            md5.Initialize();
            int end = 0x20000 + len;
            Console.WriteLine("Calculating from 0x20000 upto " + end.ToString("X8"));

            byte[] data = readdatafromfile(filename, 0x20000, len);
            byte[] hash = md5.ComputeHash(data);
            /*            foreach (byte b in hash)
                        {
                            byte bcalc = b;
                            Console.Write(" " + b.ToString("X2"));
                        }
                        Console.WriteLine("");*/
            byte[] finalhash = new byte[hash.Length];

            for (int i = 0; i < hash.Length; i++)
            {
                byte bcalc = hash[i];
                bcalc ^= 0x21;
                bcalc -= 0xD6;
                finalhash[i] = bcalc;
            }

            /*foreach (byte b in finalhash)
            {
                Console.Write(" " + b.ToString("X2"));
            }*/
            return finalhash;

        }

        private Int64 CalculateLayer1Checksum(string filename, int OffsetLayer1)
        {
            Int64 chkLayer1 = 0;
            //20000 to B5058 (?)
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            int byteCount = OffsetLayer1 - 0x20000;
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(0x20000, SeekOrigin.Begin);
                // addition
                Int64 _tempValue = 0;
                for (int tel = 0; tel < byteCount; tel += 8)
                {
                    _tempValue = 0;
                    _tempValue = Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256 * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte()) * 256;
                    _tempValue += Convert.ToInt64(br.ReadByte());
                    chkLayer1 += _tempValue;
                }
            }
            fsread.Close();

            return chkLayer1;
        }

        private bool CompareByteArray(byte[] arr1, byte[] arr2)
        {
            bool retval = true;
            if (arr1.Length != arr2.Length) retval = false;
            else
            {
                for (int t = 0; t < arr1.Length; t++)
                {
                    if (arr1[t] != arr2[t]) retval = false;
                }
            }
            return retval;
        }

        private bool CalculateLayer2Checksum(string filename, int OffsetLayer2, bool forceSilent)
        {
            bool Layer2ChecksumValid = false;
            uint checksum0 = 0;
            uint checksum1 = 0;
            uint sum0 = 0;
            uint matrix_dimension = 0;
            uint partial_address = 0;
            uint x = 0;
            /*
Get 0x100 byte buffer from CHPTR – CHPTR + 0xFF
Because level 1 is in that area level1 must be correct first
Prepare coded_buffer (0x100 buffer from chptr) with loop: coded_buffer(x) = (buffer (x) + 0xD6) xor 0x21 
(add 0xd6 to every byte of buffer, then xor it by 0x21)
[ 1 indexed, not 0 indexed ]
So, 0x101 byte buffer with first byte ignored (convention)
             * */
            byte[] coded_buffer = readdatafromfile(filename, OffsetLayer2, 0x100);

            for (int i = 0; i < coded_buffer.Length; i++)
            {
                byte b = coded_buffer[i];
                b += 0xD6;
                b ^= 0x21;
                coded_buffer[i] = b;
                //Console.Write(b.ToString("X2") + " ");
            }
            //Console.WriteLine("");
            byte[] complete_file = readdatafromfile(filename, 0, m_currentfile_size);
            int index = 0;
            bool chk_found = false;
            while (index < 0x100 && !chk_found)
            {
                if ((coded_buffer[index] == 0xFB) && (coded_buffer[index + 6] == 0xFC) && (coded_buffer[index + 0x0C] == 0xFD))
                {
                    sum0 = ((uint)coded_buffer[index + 1] * 0x01000000 + (uint)coded_buffer[index + 2] * 0x010000 + (uint)coded_buffer[index + 3] * 0x100 + (uint)coded_buffer[index + 4]);
                    matrix_dimension = (uint)coded_buffer[index + 7] * 0x01000000 + (uint)coded_buffer[index + 8] * 0x010000 + (uint)coded_buffer[index + 9] * 0x100 + (uint)coded_buffer[index + 10];
                    partial_address = (uint)coded_buffer[index + 0x0d] * 0x01000000 + (uint)coded_buffer[index + 0x0e] * 0x010000 + (uint)coded_buffer[index + 0x0F] * 0x100 + (uint)coded_buffer[index + 0x10];
                    if (matrix_dimension >= 0x020000)
                    {
                        checksum0 = 0;
                        x = partial_address /*+ 1*/;
                        while (x < (matrix_dimension - 4))
                        {
                            checksum0 = checksum0 + (uint)complete_file[x];
                            x++;
                        }
                        checksum0 = checksum0 + (uint)complete_file[matrix_dimension - 1];
                        checksum1 = 0;
                        x = partial_address /*+ 1*/;
                        while (x < (matrix_dimension - 4))
                        {
                            checksum1 = checksum1 + (uint)complete_file[x] * 0x01000000 + (uint)complete_file[x + 1] * 0x10000 + (uint)complete_file[x + 2] * 0x100 + (uint)complete_file[x + 3];
                            x = x + 4;
                        }
                        if ((checksum0 & 0xFFF00000) != (sum0 & 0xFFF00000))
                        {
                            checksum0 = checksum1;
                        }
                        if (checksum0 != sum0)
                        {
                            //MessageBox.Show("Layer 2 checksum was invalid, should be updated!");
                            if (m_appSettings.AutoChecksum || forceSilent)
                            {
                                byte[] checksum_to_file = new byte[4];
                                checksum_to_file[0] = Convert.ToByte((checksum0 >> 24) & 0x000000FF);
                                checksum_to_file[1] = Convert.ToByte((checksum0 >> 16) & 0x000000FF);
                                checksum_to_file[2] = Convert.ToByte((checksum0 >> 8) & 0x000000FF);
                                checksum_to_file[3] = Convert.ToByte((checksum0) & 0x000000FF);
                                checksum_to_file[0] = Convert.ToByte(((checksum_to_file[0] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                checksum_to_file[1] = Convert.ToByte(((checksum_to_file[1] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                checksum_to_file[2] = Convert.ToByte(((checksum_to_file[2] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                checksum_to_file[3] = Convert.ToByte(((checksum_to_file[3] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                //CreateBinaryBackup();
                                savedatatobinary(index + OffsetLayer2 + 1, 4, checksum_to_file, filename, false);
                                Layer2ChecksumValid = true;
                            }
                            else
                            {
                                frmChecksum frmchecksum = new frmChecksum();
                                frmchecksum.Layer = "Checksum validation Layer 2";
                                string layer2file = sum0.ToString("X8");
                                string layer2calc = checksum0.ToString("X8");
                                frmchecksum.FileChecksum = layer2file;
                                frmchecksum.RealChecksum = layer2calc;
                                if (frmchecksum.ShowDialog() == DialogResult.OK)
                                {
                                    byte[] checksum_to_file = new byte[4];
                                    checksum_to_file[0] = Convert.ToByte((checksum0 >> 24) & 0x000000FF);
                                    checksum_to_file[1] = Convert.ToByte((checksum0 >> 16) & 0x000000FF);
                                    checksum_to_file[2] = Convert.ToByte((checksum0 >> 8) & 0x000000FF);
                                    checksum_to_file[3] = Convert.ToByte((checksum0) & 0x000000FF);
                                    checksum_to_file[0] = Convert.ToByte(((checksum_to_file[0] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                    checksum_to_file[1] = Convert.ToByte(((checksum_to_file[1] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                    checksum_to_file[2] = Convert.ToByte(((checksum_to_file[2] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                    checksum_to_file[3] = Convert.ToByte(((checksum_to_file[3] ^ 0x21) - (byte)0xD6) & 0x000000FF);
                                    //CreateBinaryBackup();
                                    savedatatobinary(index + OffsetLayer2 + 1, 4, checksum_to_file, filename, true);
                                    Layer2ChecksumValid = true;
                                    coded_buffer[index/* + 1*/] = Convert.ToByte((checksum0 >> 24) & 0x000000FF);
                                    coded_buffer[index/* + 2*/] = Convert.ToByte((checksum0 >> 16) & 0x000000FF);
                                    coded_buffer[index/* + 3*/] = Convert.ToByte((checksum0 >> 8) & 0x000000FF);
                                    coded_buffer[index/* + 4*/] = Convert.ToByte((checksum0) & 0x000000FF);

                                    for (int i = 0; i < 4; i++)
                                    {
                                        //TODO: Actually update the file!!!
                                        //complete_file[i + index + OffsetLayer2] = Convert.ToByte((coded_buffer[index + i] ^ 0x21) - 0xD6);
                                    }
                                    // checksum is ready

                                }
                                else
                                {
                                    Layer2ChecksumValid = false;
                                }
                            }

                        }
                        else
                        {
                            Layer2ChecksumValid = true;
                        }
                        chk_found = true;
                    }
                }
                index++;
            }
            if (!chk_found)
            {
                //MessageBox.Show("Layer 2 checksum could not be calculated [ file incompatible ]");
            }
            return Layer2ChecksumValid;
        }

        private int GetEmptySpaceStartFrom(string filename, int offset)
        {
            int retval = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(offset, SeekOrigin.Begin);
                bool found = false;
                while (!found && fsread.Position < offset + 0x1000)
                {
                    retval = (int)fsread.Position;
                    byte b1 = br.ReadByte();
                    if (b1 == 0xFF)
                    {
                        byte b2 = br.ReadByte();
                        if (b2 == 0xFF)
                        {
                            byte b3 = br.ReadByte();
                            if (b3 == 0xFF) found = true;
                        }
                    }
                }
                /*retval = (int)br.ReadByte() * 256 * 256 * 256;
                retval += (int)br.ReadByte() * 256 * 256;
                retval += (int)br.ReadByte() * 256;
                retval += (int)br.ReadByte();*/
            }
            fsread.Close();
            return retval;
        }



        private void UpdateChecksum(string filename, bool forceSilent)
        {

            int m_ChecksumAreaOffset = GetChecksumAreaOffset(filename);
            if (m_ChecksumAreaOffset > m_currentfile_size) return;
            bool do_layer2 = true;
            bool _layer1Valid = false;
            bool _layer2Valid = false;
            barChecksumInfo.Caption = "Checksum: validating...";
            System.Windows.Forms.Application.DoEvents();

            //Console.WriteLine("Checksum area offset: " + m_ChecksumAreaOffset.ToString("X8"));
            byte[] hash = CalculateLayer1ChecksumMD5(filename, m_ChecksumAreaOffset, forceSilent);
            // compare hash to bytes after checksumareaoffset
            byte[] layer1checksuminfile = readdatafromfile(filename, m_ChecksumAreaOffset + 2, 16);
            if (!CompareByteArray(hash, layer1checksuminfile))
            {
                //CreateBinaryBackup();
                //savedatatobinary(m_ChecksumAreaOffset + 2, 16, hash, filename);
                if (m_appSettings.AutoChecksum || forceSilent)
                {
                    savedatatobinary(m_ChecksumAreaOffset + 2, 16, hash, filename, false);
                    _layer1Valid = true;
                    do_layer2 = true;
                }
                else
                {
                    frmChecksum frmchecksum = new frmChecksum();
                    frmchecksum.Layer = "Checksum validation Layer 1";
                    string layer1file = string.Empty;
                    string layer1calc = string.Empty;
                    for (int i = 0; i < layer1checksuminfile.Length; i++)
                    {
                        layer1file += layer1checksuminfile[i].ToString("X2") + " ";
                        layer1calc += hash[i].ToString("X2") + " ";
                    }
                    frmchecksum.FileChecksum = layer1file;
                    frmchecksum.RealChecksum = layer1calc;
                    if (frmchecksum.ShowDialog() == DialogResult.OK)
                    {
                        savedatatobinary(m_ChecksumAreaOffset + 2, 16, hash, filename, false);
                        _layer1Valid = true;
                        do_layer2 = true;
                    }
                    else
                    {
                        do_layer2 = false;
                    }
                }
            }
            else
            {
                _layer1Valid = true;
            }
            if (do_layer2)
            {
                if (CalculateLayer2Checksum(filename, m_ChecksumAreaOffset, forceSilent))
                {
                    _layer2Valid = true;
                }
                else
                {
                    _layer2Valid = false;
                }
            }
            // show result in statusbar
            if (_layer1Valid && _layer2Valid)
            {
                barChecksumInfo.Caption = "Checksum: OK";
            }
            else if (!_layer1Valid)
            {
                barChecksumInfo.Caption = "Checksum: Layer 1 invalid";
            }
            else if (!_layer2Valid)
            {
                barChecksumInfo.Caption = "Checksum: Layer 2 invalid";
            }


        }

        private byte[] ReadChecksumHeader(string filename, int checksumAreaOffset)
        {
            byte[] retval = new byte[1];
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fsread.Seek(checksumAreaOffset, SeekOrigin.Begin);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                retval = br.ReadBytes(0x100);
            }
            fsread.Close();
            return retval;
        }

        private bool ChecksumPresentInChecksumField(string filename, int checksum, int length, int checksumAreaOffset)
        {
            bool retval = false;
            byte[] checksumarea = ReadChecksumHeader(filename, checksumAreaOffset);
            if (length == 2)
            {
                checksum &= 0x0000FFFF;
                for (int i = 0; i < checksumarea.Length; i += 2)
                {
                    byte checksumbyte1 = (byte)((checksum & 0x0000FF00) >> 8);
                    byte checksumbyte2 = (byte)(checksum & 0x000000FF);
                    if (checksumarea[i] == checksumbyte1 && checksumarea[i + 1] == checksumbyte2)
                    {
                        retval = true;
                        break;
                    }
                }

            }
            else if (length == 4)
            {
                for (int i = 0; i < checksumarea.Length - 4; i++)
                {
                    byte checksumbyte1 = (byte)((checksum & 0xFF000000) >> 24);
                    byte checksumbyte2 = (byte)((checksum & 0x00FF0000) >> 16);
                    byte checksumbyte3 = (byte)((checksum & 0x0000FF00) >> 8);
                    byte checksumbyte4 = (byte)(checksum & 0x000000FF);
                    if (checksumarea[i] == checksumbyte1 && checksumarea[i + 1] == checksumbyte2 && checksumarea[i + 2] == checksumbyte3 && checksumarea[i + 4] == checksumbyte4)
                    {
                        retval = true;
                        break;
                    }
                }
            }
            return retval;
        }

        private void DumpFilePart(string filename, int start, int end, string newfilename)
        {
            byte[] bytes;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                fs.Position = start;
                BinaryReader br = new BinaryReader(fs);
                bytes = br.ReadBytes(end - start);
                br.Close();
            }
            using (FileStream fw = new FileStream(newfilename, FileMode.CreateNew))
            {
                BinaryWriter bw = new BinaryWriter(fw);
                bw.Write(bytes);
                bw.Close();
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show firmware information
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(m_currentfile))
                {
                    frmFirmwareInformation frminfo = new frmFirmwareInformation();
                    T8Header t8header = new T8Header();
                    t8header.init(m_currentfile);
                    frminfo.Filename = m_currentfile;
                    frminfo.ProgrammerName = t8header.ProgrammerName;
                    frminfo.ProgrammingDevice = t8header.ProgrammerDevice;
                    frminfo.PartNumber = t8header.PartNumber;
                    frminfo.ReleaseDate = t8header.ReleaseDate;
                    frminfo.SoftwareID = t8header.SoftwareVersion.Trim();
                    // determine engine type / MY by software version
                    // and determine engine type MY by VIN number
                    string engineTypeBySWVersion = string.Empty;
                    string engineTypeByVIN = string.Empty;
                    VINDecoder decoder = new VINDecoder();
                    VINCarInfo carinfo = decoder.DecodeVINNumber(t8header.ChassisID);
                    engineTypeByVIN = carinfo.EngineType + " MY" + carinfo.Makeyear.ToString() + " " + carinfo.GearboxDescription;
                    frminfo.EngineTypeByVIN = engineTypeByVIN;
                    string swversion = t8header.SoftwareVersion.Trim();
                    engineTypeBySWVersion = DetermineCarInfoBySWVersion(swversion);
                    frminfo.ChassisID = t8header.ChassisID;
                    frminfo.EngineTypeBySoftwareVersion = engineTypeBySWVersion;//t8header.CarDescription;
                    frminfo.ImmoID = t8header.ImmobilizerID;
                    frminfo.HardwareID = t8header.HardwareID;
                    frminfo.HardwareType = t8header.DeviceType;
                    frminfo.ECUDescription = t8header.EcuDescription;
                    frminfo.InterfaceDevice = t8header.InterfaceDevice;
                    frminfo.NumberOfFlashBlocks = t8header.NumberOfFlashBlocks.ToString();

                    if (frminfo.ShowDialog() == DialogResult.OK)
                    {
                        t8header.ImmobilizerID = frminfo.ImmoID;
                        t8header.SoftwareVersion = frminfo.SoftwareID;
                        t8header.CarDescription = frminfo.EngineTypeBySoftwareVersion;
                        t8header.ChassisID = frminfo.ChassisID;
                        t8header.ReleaseDate = frminfo.ReleaseDate;
                        t8header.ProgrammerDevice = frminfo.ProgrammingDevice;
                        t8header.ProgrammerName = frminfo.ProgrammerName;
                        t8header.PartNumber = frminfo.PartNumber;
                        t8header.DeviceType = frminfo.HardwareType;
                        t8header.HardwareID = frminfo.HardwareID;
                        t8header.InterfaceDevice = frminfo.InterfaceDevice;
                        t8header.EcuDescription = frminfo.ECUDescription;
                        // only if enabled
                        if (frminfo.ChangeVINAndImmo)
                        {
                            t8header.UpdateVinAndImmoCode();
                        }
                        UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);
                    }
                }
            }
        }

        private string DetermineCarInfoBySWVersion(string swversion)
        {
            string retval = string.Empty;
            try
            {
                char[] sep = new char[1];
                sep.SetValue('_', 0);
                string[] ids = swversion.Split(sep);
                if (ids.Length > 0)
                {
                    string engineID = ids.GetValue(ids.Length - 1).ToString();
                    if (engineID.Length == 3)
                    {
                        int engineTypeID = Convert.ToInt32(engineID.Substring(0, 2));
                        char engineMY = engineID[2];
                        switch (engineTypeID)
                        {
                            case 80:
                                retval = "B207E";
                                switch (engineMY)
                                {
                                    case 'b':
                                        retval += " MY03";
                                        break;
                                    case 'c':
                                    case 'd':
                                    case 'e':
                                        retval += " MY04/05";
                                        break;
                                    case 'f':
                                    case 'h':
                                        retval += " MY07/08";
                                        break;
                                    case 'g':
                                        retval += " MY07/08 Biopower";
                                        break;
                                }
                                break;
                            case 81:
                                retval = "B207L";
                                switch (engineMY)
                                {
                                    case 'b':
                                        retval += " MY03";
                                        break;
                                    case 'c':
                                    case 'd':
                                    case 'e':
                                        retval += " MY04/05";
                                        break;
                                    case 'f':
                                    case 'h':
                                        retval += " MY05";
                                        break;
                                    case 'i':
                                        retval += " MY06";
                                        break;
                                    case 'j':
                                        retval += " MY07";
                                        break;
                                    case 'l':
                                        retval += " MY08 Biopower";
                                        break;
                                    case 'm':
                                        retval += " MY09";
                                        break;
                                }
                                break;
                            case 82:
                                retval = "B207R";
                                switch (engineMY)
                                {
                                    case 'b':
                                        retval += " MY03";
                                        break;
                                    case 'c':
                                    case 'd':
                                    case 'e':
                                        retval += " MY04/05";
                                        break;
                                    case 'f':
                                    case 'h':
                                        retval += " MY05";
                                        break;
                                    case 'n':
                                        retval += " MY06";
                                        break;
                                    case 's':
                                        retval += " MY07";
                                        break;
                                    case 'r':
                                        retval += " MY07/08";
                                        break;
                                    case 'x':
                                    case 'v':
                                        retval += " MY09";
                                        break;
                                    case '8':
                                        retval += " MY10";
                                        break;
                                }
                                break;
                            case 83:
                                retval = "Z20NET";
                                switch (engineMY)
                                {
                                    
                                    case 'e':
                                        retval += " MY03";
                                        break;
                                    case 'f':
                                        retval += " MY04";
                                        break;
                                    case 'g':
                                        retval += " MY05";
                                        break;
                                    case 'h':
                                        retval += " MY06";
                                        break;
                                    case 'i':
                                        retval += " MY08";
                                        break;
                                    
                                }
                                break;
                        }
                        
                    }
                }
            }
            catch (Exception)
            {

            }
            return retval;
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_currentfile != "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Trionic 8 binaries|*.bin";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    bincomp.SetCurrentFilename(m_currentfile);
                    bincomp.SetCompareFilename(openFileDialog1.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("No file is currently opened, you need to open a binary file first to compare it to another one!");
            }
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // start user manual PDF file
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//T8_manual.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//T8_manual.pdf");
                }
                else
                {
                    MessageBox.Show("T8Suite user manual could not be found or opened!");
                }
            }
            catch (Exception E2)
            {
                Console.WriteLine(E2.Message);
            }
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // start Trionic 8 PDF file
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//Trionic 8.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//Trionic 8.pdf");
                }
                else
                {
                    MessageBox.Show("Trionic 8 documentation could not be found or opened!");
                }
            }
            catch (Exception E2)
            {
                Console.WriteLine(E2.Message);
            }
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // check for updates
            try
            {
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.CheckForUpdates("Global", "http://trionic.mobixs.eu/t8suite/", "", "", false);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_currentfile != "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Trionic 8 binaries|*.bin";
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    bincomp.Symbols = m_symbols;
                    bincomp.OutsideSymbolRangeCheck = true;
                    bincomp.SetCurrentFilename(m_currentfile);
                    bincomp.SetCompareFilename(openFileDialog1.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("No file is currently opened, you need to open a binary file first to compare it to another one!");
            }
        }

        private void SetToolstripTheme()
        {
            //Console.WriteLine("Rendermode was: " + ToolStripManager.RenderMode.ToString());
            //Console.WriteLine("Visual styles: " + ToolStripManager.VisualStylesEnabled.ToString());
            //Console.WriteLine("Skinname: " + appSettings.SkinName);
            //Console.WriteLine("Backcolor: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor.ToString());
            //Console.WriteLine("Backcolor2: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor2.ToString());
            try
            {
                Skin currentSkin = CommonSkins.GetSkin(defaultLookAndFeel1.LookAndFeel);
                Color c = currentSkin.TranslateColor(SystemColors.Control);
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
                ProfColorTable profcolortable = new ProfColorTable();
                profcolortable.CustomToolstripGradientBegin = c;
                profcolortable.CustomToolstripGradientMiddle = c;
                profcolortable.CustomToolstripGradientEnd = c;
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(profcolortable);
            }
            catch (Exception)
            {

            }

        }

        void InitSkins()
        {
            /*
            ribbonControl1.ForceInitialize();
            BarButtonItem item;

            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.BonusSkins).Assembly);
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);

            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                item = new BarButtonItem();
                item.Caption = cnt.SkinName;
                //iPaintStyle.AddItem(item);
                ribbonPageGroup13.ItemLinks.Add(item);
                item.ItemClick += new ItemClickEventHandler(OnSkinClick);
            }*/

            ribbonControl1.ForceInitialize();
            BarButtonItem item;
            int skinCount = 0;
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.BonusSkins).Assembly);
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);
            SymbolCollection symcol = new SymbolCollection();
            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                SymbolHelper sh = new SymbolHelper();
                sh.Varname = cnt.SkinName;
                symcol.Add(sh);
            }
            symcol.SortColumn = "Varname";
            symcol.SortingOrder = GenericComparer.SortOrder.Ascending;
            symcol.Sort();
            foreach (SymbolHelper sh in symcol)
            {
                item = new BarButtonItem();
                item.Caption = sh.Varname;
                BarItemLink il = ribbonPageGroup13.ItemLinks.Add(item);
                if ((skinCount++ % 3) == 0) il.BeginGroup = true;

                item.ItemClick += new ItemClickEventHandler(OnSkinClick);
            }
            try
            {
                if (IsChristmasTime())
                {
                    // set chrismas skin
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Xmas 2008 Blue"); // don't save
                }
                else if (IsHalloweenTime())
                {
                    // set Halloween skin
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Pumpkin"); // don't save
                }
                else if (IsValetineTime())
                {
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Valentine"); // don't save
                }
                else
                {
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(m_appSettings.Skinname);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            SetToolstripTheme();
        }

        /// <summary>
        /// OnSkinClick: Als er een skin gekozen wordt door de gebruiker voer deze
        /// dan door in de user interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSkinClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string skinName = e.Item.Caption;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName);
            m_appSettings.Skinname = skinName;
            SetToolstripTheme();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_connectedToECU = false;
            m_prohibitReading = true;
            if (m_CurrentWorkingProject != "")
            {
                CloseProject();
            }
            m_appSettings.ShowMenu = !ribbonControl1.Minimized;
            SaveLayoutFiles();
            SaveRealtimeTable();
            try
            {
                m_mapHelper.Close();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            if (t8can.isOpen()) t8can.Cleanup();
            Environment.Exit(0);
        }

        private void gridViewSymbols_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartTableViewer();
            }

        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd1 = new SaveFileDialog();
            sfd1.Filter = "S19 files|*.S19";
            sfd1.AddExtension = true;
            sfd1.DefaultExt = "S19";
            sfd1.OverwritePrompt = true;
            sfd1.Title = "Export file to motorola S19 format...";
            if (sfd1.ShowDialog() == DialogResult.OK)
            {
                srec2bin srec = new srec2bin();
                srec.ConvertBinToSrec(m_currentfile, sfd1.FileName);
            }
        }

        private void gridControlSymbols_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left && _isMouseDown)
            {
                _isMouseDown = false; // try only once
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = gridViewSymbols.CalcHitInfo(e.Location);
                if (!(hi.IsValid && hi.InRowCell)) return;

                if (gridViewSymbols.SelectedRowsCount > 0)
                {
                    int[] selrows = gridViewSymbols.GetSelectedRows();
                    if (selrows.Length > 0)
                    {
                        int row = (int)selrows.GetValue(0);
                        if (row >= 0)
                        {
                            SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                            Console.WriteLine("Symbol dragging: " + sh.Varname);
                            sh.Currentdata = readdatafromfile(m_currentfile, (int)sh.Flash_start_address, sh.Length);
                            gridControlSymbols.DoDragDrop(sh, DragDropEffects.All);

                        }

                    }

                }
            }
            else
            {
                try
                {
                    GridHitInfo ghi = gridViewSymbols.CalcHitInfo(new System.Drawing.Point(e.X, e.Y));
                    if (ghi != null)
                    {
                        ShowSymbolHitInfo(ghi);
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("Failed to run hitinfo on gridControlSymbols: " + E.Message);
                }
            }
        }

        private void ShowSymbolHitInfo(DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi)
        {
            if (hi == null)
            {
                return;
            }
            try
            {
                if (hi.InRowCell)
                {
                    if (hi.Column.Name == gcSymbolsName.Name)
                    {

                        if (m_appSettings.ShowMapPreviewPopup)
                        {
                            if (hi.RowHandle < 0) return;

                            if (m_currentMapHelperRowHandle != hi.RowHandle)
                            {
                                m_currentMapHelperRowHandle = hi.RowHandle;

                                if (tmrMapHelper.Enabled)
                                {
                                    m_currentMapHelperRowHandle = hi.RowHandle;
                                }
                                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow(hi.RowHandle);

                                // tot hier
                                m_currentMapname = sh.Varname;
                                Console.WriteLine("Hover above: " + sh.Varname);
                                if (m_mapHelper != null)
                                {
                                    m_mapHelper.SuspendLayout();
                                    System.Drawing.Point p = gridControlSymbols.PointToScreen(hi.HitPoint);
                                    m_mapHelper.Location = new System.Drawing.Point(gridControlSymbols.Location.X + gridControlSymbols.Size.Width + 50, p.Y);
                                    m_mapHelper.ResumeLayout();
                                    if (m_mapHelper.Visible)
                                    {
                                        tmrMapHelper_Tick(this, EventArgs.Empty);
                                    }
                                    else
                                    {
                                        tmrMapHelper.Stop();
                                        tmrMapHelper.Start();
                                        m_mapHelper.Reset();
                                    }
                                }

                            }
                            //AddLogItem("Stop: " + hi.RowHandle.ToString());

                        }

                    }
                    else
                    {
                        try
                        {
                            //AddLogItem("Start (1): " );

                            if (m_appSettings.ShowMapPreviewPopup)
                            {
                                //if (m_mapHelper.Visible)
                                {
                                    tmrMapHelper.Stop();
                                    m_mapHelper.Reset();
                                }
                            }
                            //AddLogItem("Stop (1): ");
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Failed to reset mapHelper: " + E.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        // AddLogItem("Start (2): ");

                        if (m_appSettings.ShowMapPreviewPopup)
                        {
                            //if (m_mapHelper.Visible)
                            {
                                tmrMapHelper.Stop();

                                m_mapHelper.Reset();
                            }
                        }
                        //AddLogItem("Start (2): ");

                    }
                    catch (Exception mE)
                    {
                        Console.WriteLine("Failed to reset mapHelper (2): " + mE.Message);
                    }
                }
                //if (hi.Column.Name == gcVehID.Name)
                {
                    //svv = new SingleVehicleViewer(appSettings.MapPath, 
                }
                //AddLogItem("Start (3): ");

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            //AddLogItem("Start (3): ");

        }

        private void tmrMapHelper_Tick(object sender, EventArgs e)
        {
            tmrMapHelper.Enabled = false;
            if (m_mapHelper != null && m_currentMapname != string.Empty)
            {
                //m_mapHelper.Location
                //m_mapHelper.ShowPosition(_helperPositionMercator);
                m_mapHelper.mapViewer1.SetViewSize(m_appSettings.DefaultViewSize);
                m_mapHelper.mapViewer1.Visible = false;
                m_mapHelper.mapViewer1.Filename = m_currentfile;
                //m_mapHelper.mapViewer1.GraphVisible = m_appSettings.ShowGraphs;
                m_mapHelper.mapViewer1.Viewtype = m_appSettings.DefaultViewType;
                m_mapHelper.mapViewer1.DisableColors = m_appSettings.DisableMapviewerColors;
                m_mapHelper.mapViewer1.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                m_mapHelper.mapViewer1.IsRedWhite = m_appSettings.ShowRedWhite;
                m_mapHelper.mapViewer1.Map_name = m_currentMapname;
                m_mapHelper.mapViewer1.Map_descr = "";//TranslateSymbolName(m_mapHelper.mapViewer1.Map_name);
                m_mapHelper.mapViewer1.Map_cat = XDFCategories.Undocumented; //TranslateSymbolNameToCategory(m_mapHelper.mapViewer1.Map_name);

                SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                string x_axis = string.Empty;
                string y_axis = string.Empty;
                string x_axis_descr = string.Empty;
                string y_axis_descr = string.Empty;
                string z_axis_descr = string.Empty;
                axestrans.GetAxisSymbols(m_currentMapname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                if (x_axis == "BstKnkCal.OffsetXSP")
                {
                    if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
                }

                m_mapHelper.mapViewer1.X_axis_name = x_axis_descr;
                m_mapHelper.mapViewer1.Y_axis_name = y_axis_descr;
                m_mapHelper.mapViewer1.Z_axis_name = z_axis_descr;
                m_mapHelper.mapViewer1.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, m_mapHelper.mapViewer1.Map_name);
                m_mapHelper.mapViewer1.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, m_mapHelper.mapViewer1.Map_name);
                int columns = 8;
                int rows = 8;
                int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, m_mapHelper.mapViewer1.Map_name, out columns, out rows);
                int address = Convert.ToInt32(GetSymbolAddress(m_symbols, m_mapHelper.mapViewer1.Map_name));
                int sramaddress = 0;// Convert.ToInt32(dr.Row["SRAMADDRESS"].ToString());
                if (address != 0)
                {

                    while (address > m_currentfile_size) address -= m_currentfile_size;
                    m_mapHelper.mapViewer1.Map_address = address;
                    m_mapHelper.mapViewer1.Map_sramaddress = sramaddress;
                    int length = Convert.ToInt32(GetSymbolLength(m_symbols, m_mapHelper.mapViewer1.Map_name));
                    m_mapHelper.mapViewer1.Map_length = length;
                    byte[] mapdata = readdatafromfile(m_currentfile, address, length);
                    m_mapHelper.mapViewer1.Map_content = mapdata;
                    m_mapHelper.mapViewer1.Correction_factor = GetMapCorrectionFactor(m_mapHelper.mapViewer1.Map_name);
                    m_mapHelper.mapViewer1.Correction_offset = GetMapCorrectionOffset(m_mapHelper.mapViewer1.Map_name);
                    m_mapHelper.mapViewer1.IsUpsideDown = GetMapUpsideDown(m_mapHelper.mapViewer1.Map_name);
                    m_mapHelper.mapViewer1.GraphVisible = false;
                    m_mapHelper.mapViewer1.TableVisible = true;
                    m_mapHelper.mapViewer1.Visible = true;
                    m_mapHelper.mapViewer1.ShowTable(columns, isSixteenBitTable(m_mapHelper.mapViewer1.Map_name));
                    m_mapHelper.mapViewer1.SetSurfaceGraphZoom(0.15);
                    //m_mapHelper.mapViewer1.SetSurfaceGraphView(10, 10, 10, 10, 10, 0.2);
                    m_mapHelper.mapViewer1.Update();
                    Console.WriteLine("Showing: " + m_mapHelper.mapViewer1.Map_name);
                    m_mapHelper.SetSize(new Size(tablewidth * 60, rows * 15));
                    m_mapHelper.ShowPosition();
                }
            }
        }

        private void btnNominalTorqueMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqMastCal.Trq_NominalMap");
        }

        private void brnAirmassTorqueMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqMastCal.m_AirTorqMap");

        }

        private void btnPedalPositionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqMastCal.X_AccPedalMAP");

        }

        private void btnNormalIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_NormalMAP");
        }

        private void btnHighOctaneIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_highOctanMAP");
        }

        private void btnLowOctaneIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_lowOctanMAP");
        }

        private void btnMBTIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_IgnMBTMAP");
        }

        private void btnFuelcutIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_FuelCutMAP");
        }

        private void btnStartupIgnitionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("IgnAbsCal.fi_StartMAP");
        }

        private void btnOverboostMAP_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_OverBoostTab");
        }

        private void btnEGTMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("ExhaustCal.T_Lambda1Map");
        }

        private void btnFuelKnockMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("KnkFuelCal.EnrichmentMap");
        }

        private void btnMaxAirmassMapManual_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("BstKnkCal.MaxAirmass");
        }

        private void btnMaxAirmassMapAuto_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("BstKnkCal.MaxAirmassAu");
        }

        private void btnInjectionEndangleMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("InjAnglCal.Map");
        }

        private void btnRequestedTorqueMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("PedalMapCal.Trq_RequestMap");
        }

        private void btnMBTAirmass_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("AirMassMastCal.m_AirMBTMAP");
        }

        private void btnFuelCorrectionMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("BFuelCal.LambdaOneFacMap");

        }

        private void btnBoostRegulationMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("AirCtrlCal.RegMap");
        }

        private void btnAmbientPressureTorqueLimitMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_CompressorNoiseRedLimMAP");
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_MaxEngineAutTab2");
        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_MaxEngineAutTab1");

        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_MaxEngineManTab2");

        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_MaxEngineManTab1");

        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("TrqLimCal.Trq_ManGear");

        }

        private void btnPofPIDMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("AirCtrlCal.Ppart_BoostMap");
        }

        private void btnIofPIDMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("AirCtrlCal.Ipart_BoostMap");

        }

        private void btnDofPIDMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("AirCtrlCal.Dpart_BoostMap");

        }

        private SymbolCollection GetRealtimeNotificationSymbols()
        {
            SymbolCollection _symbols = new SymbolCollection();
            if (m_symbols != null)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Start_address > 0x0100000)
                    {
                        _symbols.Add(sh);
                    }
                }
            }
            return _symbols;
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmSettings set = new frmSettings();
            m_appSettings = new AppSettings();
            set.AppSettings = m_appSettings;
            set.Symbols = GetRealtimeNotificationSymbols();
            set.CANBusAdapterType = m_appSettings.CANBusAdapterType;

            set.InterpolateLogWorksTimescale = m_appSettings.InterpolateLogWorksTimescale;
            //set.AutoGenerateLogWorksFile = m_appSettings.AutoGenerateLogWorks;
            set.OnlyPBus = m_appSettings.OnlyPBus;
            set.ResetRealtimeSymbolOnTabPageSwitch = m_appSettings.ResetRealtimeSymbolOnTabPageSwitch;

            set.AutoSizeNewWindows = m_appSettings.AutoSizeNewWindows;
            set.AutoSizeColumnsInViewer = m_appSettings.AutoSizeColumnsInWindows;
            set.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            set.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
            set.HideSymbolWindow = m_appSettings.HideSymbolTable;
            set.ShowGraphsInMapViewer = m_appSettings.ShowGraphs;
            set.AutoMapDetectionActive = m_appSettings.MapDetectionActive;
            set.ViewTablesInHex = m_appSettings.Viewinhex;
            set.AutoDockSameFile = m_appSettings.AutoDockSameFile;
            set.AutoDockSameSymbol = m_appSettings.AutoDockSameSymbol;
            set.DisableMapviewerColors = m_appSettings.DisableMapviewerColors;
            set.ShowMapViewersInWindows = m_appSettings.ShowViewerInWindows;
            set.NewPanelsFloating = m_appSettings.NewPanelsFloating;
            set.AutoLoadLastFile = m_appSettings.AutoLoadLastFile;
            set.ShowMapPreviewPopup = m_appSettings.ShowMapPreviewPopup;
            set.DefaultViewType = m_appSettings.DefaultViewType;
            set.DefaultViewSize = m_appSettings.DefaultViewSize;
            set.SynchronizeMapviewers = m_appSettings.SynchronizeMapviewers;
            set.FancyDocking = m_appSettings.FancyDocking;
            set.ShowTablesUpsideDown = m_appSettings.ShowTablesUpsideDown;

            set.RequestProjectNotes = m_appSettings.RequestProjectNotes;
            set.ProjectFolder = m_appSettings.ProjectFolder;

            if (set.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.InterpolateLogWorksTimescale = set.InterpolateLogWorksTimescale;
                m_appSettings.AutoSizeNewWindows = set.AutoSizeNewWindows;
                m_appSettings.AutoSizeColumnsInWindows = set.AutoSizeColumnsInViewer;
                m_appSettings.AutoChecksum = set.AutoUpdateChecksum;
                m_appSettings.ShowAddressesInHex = set.ShowAddressesInHex;
                //m_appSettings.AutoGenerateLogWorks = set.AutoGenerateLogWorksFile;
                m_appSettings.HideSymbolTable = set.HideSymbolWindow;
                m_appSettings.ShowGraphs = set.ShowGraphsInMapViewer;
                m_appSettings.MapDetectionActive = set.AutoMapDetectionActive;
                m_appSettings.Viewinhex = set.ViewTablesInHex;
                m_appSettings.DisableMapviewerColors = set.DisableMapviewerColors;
                m_appSettings.AutoDockSameFile = set.AutoDockSameFile;
                m_appSettings.AutoDockSameSymbol = set.AutoDockSameSymbol;
                m_appSettings.ShowViewerInWindows = set.ShowMapViewersInWindows;
                m_appSettings.NewPanelsFloating = set.NewPanelsFloating;
                m_appSettings.CANBusAdapterType = set.CANBusAdapterType;
                m_appSettings.OnlyPBus = set.OnlyPBus;

                m_appSettings.ResetRealtimeSymbolOnTabPageSwitch = set.ResetRealtimeSymbolOnTabPageSwitch;

                m_appSettings.DefaultViewType = set.DefaultViewType;
                m_appSettings.DefaultViewSize = set.DefaultViewSize;

                m_appSettings.AutoLoadLastFile = set.AutoLoadLastFile;
                m_appSettings.FancyDocking = set.FancyDocking;
                m_appSettings.ShowTablesUpsideDown = set.ShowTablesUpsideDown;
                m_appSettings.ShowMapPreviewPopup = set.ShowMapPreviewPopup;
                m_appSettings.SynchronizeMapviewers = set.SynchronizeMapviewers;
                //UpdateSettingButtons();
                if (!m_appSettings.FancyDocking)
                {
                    dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.Standard;
                }
                else
                {
                    dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.VS2005;
                }

                m_appSettings.RequestProjectNotes = set.RequestProjectNotes;
                m_appSettings.ProjectFolder = set.ProjectFolder;
            }
            SetFilterMode();
        }

        private void SetFilterMode()
        {
            if (m_appSettings.ShowAddressesInHex)
            {
                gcSymbolsAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsAddress.DisplayFormat.FormatString = "X6";
                gcSymbolsAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolsLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsLength.DisplayFormat.FormatString = "X6";
                gcSymbolsLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;

                gcSymbolsBitmask.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsBitmask.DisplayFormat.FormatString = "X6";
                gcSymbolsBitmask.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            else
            {
                gcSymbolsAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsAddress.DisplayFormat.FormatString = "";
                gcSymbolsAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolsLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsLength.DisplayFormat.FormatString = "";
                gcSymbolsLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolsBitmask.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsBitmask.DisplayFormat.FormatString = "";
                gcSymbolsBitmask.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
            }
        }

        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartExcelExport();
        }

        private void barButtonItem20_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportFileInExcelFormat();
        }

        private void StartExcelExport()
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));

                    //                    DataRowView dr = (DataRowView)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    //frmTableDetail tabdet = new frmTableDetail();
                    string Map_name = sh.Varname;
                    if (Map_name.StartsWith("Symbol:") && sh.Userdescription != "") Map_name = sh.Userdescription;
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, Map_name, out columns, out rows);
                    int address = (int)sh.Flash_start_address;
                    if (address != 0)
                    {
                        while (address > m_currentfile_size) address -= m_currentfile_size;
                        int length = sh.Length;
                        byte[] mapdata = readdatafromfile(m_currentfile, address, length);
                        int[] xaxis = GetXaxisValues(m_currentfile, m_symbols, Map_name);
                        int[] yaxis = GetYaxisValues(m_currentfile, m_symbols, Map_name);
                        ExportToExcel(Map_name, address, length, mapdata, columns, rows, isSixteenBitTable(Map_name), xaxis, yaxis);
                    }
                }
            }
            else
            {
                MessageBox.Show("No symbol selected in the primary symbol list");
            }
        }
        private System.Data.DataTable getDataFromXLS(string strFilePath)
        {
            try
            {
                string strConnectionString = string.Empty;
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + @";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                //MessageBox.Show(strConnectionString);
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM [symboldata$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter();
                daCSV.SelectCommand = cmdSelect;
                System.Data.DataTable dtCSV = new System.Data.DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                return null;
            }
            finally { }
        }

        private void ImportExcelSymbol(string symbolname, string filename)
        {
            bool issixteenbit = false;
            System.Data.DataTable dt = getDataFromXLS(openFileDialog2.FileName);
            if (isSixteenBitTable(symbolname)) issixteenbit = true;
            int symbollength = GetSymbolLength(m_symbols, symbolname);
            int datalength = symbollength;
            if (issixteenbit) datalength /= 2;
            int[] buffer = new int[datalength];
            int bcount = 0;
            for (int rtel = 1; rtel < dt.Rows.Count; rtel++)
            {
                try
                {
                    int idx = 0;
                    foreach (object o in dt.Rows[rtel].ItemArray)
                    {
                        if (idx > 0)
                        {
                            if (o != null)
                            {
                                if (o != DBNull.Value)
                                {
                                    if (bcount < buffer.Length)
                                    {
                                        buffer.SetValue(Convert.ToInt32(o), bcount++);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Too much information in file, abort");
                                        return;
                                    }
                                }
                            }
                        }
                        idx++;
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("ImportExcelSymbol: " + E.Message);
                }

            }
            if (bcount >= datalength)
            {
                byte[] data = new byte[symbollength];
                int cellcount = 0;
                if (issixteenbit)
                {
                    for (int dcnt = 0; dcnt < buffer.Length; dcnt++)
                    {
                        string bstr1 = "0";
                        string bstr2 = "0";
                        int cellvalue = Convert.ToInt32(buffer.GetValue(dcnt));
                        bstr1 = cellvalue.ToString("X4").Substring(0, 2);
                        bstr2 = cellvalue.ToString("X4").Substring(2, 2);
                        data.SetValue(Convert.ToByte(bstr1, 16), cellcount++);
                        data.SetValue(Convert.ToByte(bstr2, 16), cellcount++);
                    }
                }
                else
                {
                    for (int dcnt = 0; dcnt < buffer.Length; dcnt++)
                    {
                        int cellvalue = Convert.ToInt32(buffer.GetValue(dcnt));
                        data.SetValue(Convert.ToByte(cellvalue.ToString()), cellcount++);
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, symbolname), symbollength, data, m_currentfile, true);
                //verifychecksum(false);
                UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);
            }

        }

        private void ImportFileInExcelFormat()
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string mapname = string.Empty;
                    int tildeindex = openFileDialog2.FileName.LastIndexOf("~");
                    bool symbolfound = false;
                    if (tildeindex > 0)
                    {
                        tildeindex++;
                        mapname = openFileDialog2.FileName.Substring(tildeindex, openFileDialog2.FileName.Length - tildeindex);
                        mapname = mapname.Replace(".xls", "");
                        mapname = mapname.Replace(".XLS", "");
                        mapname = mapname.Replace(".Xls", "");
                        // look if it is a valid symbolname
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Varname == mapname || sh.Userdescription == mapname)
                            {
                                symbolfound = true;
                                if (MessageBox.Show("Found valid symbol for import: " + mapname + ". Are you sure you want to overwrite the map in the binary?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    // ok, overwrite info in binary
                                }
                                else
                                {
                                    mapname = string.Empty; // do nothing
                                }
                            }
                        }
                        if (!symbolfound)
                        {
                            // ask user for symbol designation
                            frmSymbolSelect frmselect = new frmSymbolSelect(m_symbols);
                            if (frmselect.ShowDialog() == DialogResult.OK)
                            {
                                mapname = frmselect.SelectedSymbol;
                            }
                        }

                    }
                    else
                    {
                        // ask user for symbol designation
                        frmSymbolSelect frmselect = new frmSymbolSelect(m_symbols);
                        if (frmselect.ShowDialog() == DialogResult.OK)
                        {
                            mapname = frmselect.SelectedSymbol;
                        }

                    }
                    if (mapname != string.Empty)
                    {
                        ImportExcelSymbol(mapname, openFileDialog2.FileName);
                    }

                }
                catch (Exception E)
                {
                    MessageBox.Show("Failed to import map from excel: " + E.Message);
                }
            }
        }

        private System.Data.DataTable getDataFromXLSSymbolHelper(string strFilePath)
        {
            try
            {
                string strConnectionString = string.Empty;
                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + @";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM [Symbols$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter();
                daCSV.SelectCommand = cmdSelect;
                System.Data.DataTable dtCSV = new System.Data.DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { }
        }

        private byte[] TurnMapUpsideDown(byte[] mapdata, int numcolumns, int numrows, bool issixteenbit)
        {
            byte[] mapdatanew = new byte[mapdata.Length];
            if (issixteenbit) numcolumns *= 2;
            int internal_rows = mapdata.Length / numcolumns;
            for (int tel = 0; tel < internal_rows; tel++)
            {
                for (int ctel = 0; ctel < numcolumns; ctel++)
                {
                    int orgoffset = (((internal_rows - 1) - tel) * numcolumns) + ctel;
                    mapdatanew.SetValue(mapdata.GetValue(orgoffset), (tel * numcolumns) + ctel);
                }
            }
            return mapdatanew;
        }
        private void ExportToExcel(string mapname, int address, int length, byte[] mapdata, int cols, int rows, bool isSixteenbit, int[] xaxisvalues, int[] yaxisvalues)
        {
            try
            {
                bool isupsidedown = GetMapUpsideDown(mapname);
                if (xla == null)
                {
                    xla = new Microsoft.Office.Interop.Excel.Application();
                }

                // turn mapdata upside down
                if (isupsidedown)
                {
                    mapdata = TurnMapUpsideDown(mapdata, cols, rows, isSixteenbit);
                }

                xla.Visible = true;
                Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;
                ws.Name = "symboldata";

                // Now create the chart.
                ChartObjects chartObjs = (ChartObjects)ws.ChartObjects(Type.Missing);
                ChartObject chartObj = chartObjs.Add(100, 400, 400, 300);
                Chart xlChart = chartObj.Chart;

                int nRows = rows;
                if (isSixteenbit) nRows /= 2;
                int nColumns = cols;
                string upperLeftCell = "B3";
                int endRowNumber = System.Int32.Parse(upperLeftCell.Substring(1)) + nRows - 1;
                char endColumnLetter = System.Convert.ToChar(Convert.ToInt32(upperLeftCell[0]) + nColumns - 1);
                string upperRightCell = System.String.Format("{0}{1}", endColumnLetter, System.Int32.Parse(upperLeftCell.Substring(1)));
                string lowerRightCell = System.String.Format("{0}{1}", endColumnLetter, endRowNumber);
                // Send single dimensional array to Excel:
                Range rg1 = ws.get_Range("B2", "Z2");
                double[] xarray = new double[nColumns];
                double[] yarray = new double[nRows];
                ws.Cells[1, 1] = "Data for " + mapname;
                for (int i = 0; i < xarray.Length; i++)
                {
                    if (xaxisvalues.Length > i)
                    {
                        xarray[i] = (int)xaxisvalues.GetValue(i);
                    }
                    else
                    {
                        xarray[i] = i;
                    }
                    //ws.Cells[i + 3, 1] = xarray[i];
                    ws.Cells[2, 2 + i] = xarray[i];
                }
                for (int i = 0; i < yarray.Length; i++)
                {
                    if (yaxisvalues.Length > i)
                    {
                        if (isupsidedown)
                        {
                            yarray[i] = (int)yaxisvalues.GetValue((yarray.Length - 1) - i);
                        }
                        else
                        {
                            yarray[i] = (int)yaxisvalues.GetValue(i);
                        }
                    }
                    else
                    {
                        yarray[i] = i;
                    }
                    ws.Cells[i + 3, 1] = yarray[i];
                    //ws.Cells[2, 2 + i] = yarray[i];
                }

                string xaxisdescr = "x-axis";
                string yaxisdescr = "y-axis";
                string zaxisdescr = "z-axis";
                GetAxisDescriptions(m_currentfile, m_symbols, mapname, out xaxisdescr, out yaxisdescr, out zaxisdescr);
                Range rg = ws.get_Range(upperLeftCell, lowerRightCell);
                rg.Value2 = AddData(nRows, nColumns, mapdata, isSixteenbit);

                Range chartRange = ws.get_Range("A2", lowerRightCell);
                xlChart.SetSourceData(chartRange, Type.Missing);
                if (yarray.Length > 1)
                {
                    xlChart.ChartType = XlChartType.xlSurface;
                }

                // Customize axes:
                Axis xAxis = (Axis)xlChart.Axes(XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = yaxisdescr;
                try
                {
                    Axis yAxis = (Axis)xlChart.Axes(XlAxisType.xlSeriesAxis,
                        XlAxisGroup.xlPrimary);
                    yAxis.HasTitle = true;
                    yAxis.AxisTitle.Text = xaxisdescr;
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }


                Axis zAxis = (Axis)xlChart.Axes(XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary);
                zAxis.HasTitle = true;
                zAxis.AxisTitle.Text = zaxisdescr;

                // Add title:
                xlChart.HasTitle = true;

                xlChart.ChartTitle.Text = TranslateSymbolName(mapname);

                // Remove legend:
                xlChart.HasLegend = false;
                // add 3d shade
                xlChart.SurfaceGroup.Has3DShading = true;
                /*if (File.Exists(m_currentfile + "~" + mapname + ".xls"))
                {

                }*/
                wb.SaveAs(m_currentfile + "~" + mapname + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, false, null, null, null, null);

                /* This following code is used to create Excel default color indices:
                for (int i = 0; i < 14; i++)
                {
                    string cellString = "A" + (i + 1).ToString();
                    ws.get_Range(cellString, cellString).Interior.ColorIndex = i + 1;
                    ws.get_Range(cellString, cellString).Value2 = i + 1;
                    cellString = "B" + (i + 1).ToString();
                    ws.get_Range(cellString, cellString).Interior.ColorIndex = 14 + i + 1;
                    ws.get_Range(cellString, cellString).Value2 = 14 + i + 1;
                    cellString = "C" + (i + 1).ToString();
                    ws.get_Range(cellString, cellString).Interior.ColorIndex = 2 * 14 + i + 1;
                    ws.get_Range(cellString, cellString).Value2 = 2 * 14 + i + 1;
                    cellString = "D" + (i + 1).ToString();
                    ws.get_Range(cellString, cellString).Interior.ColorIndex = 3 * 14 + i + 1;
                    ws.get_Range(cellString, cellString).Value2 = 3 * 14 + i + 1;
                }*/
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to export to excel: " + E.Message);
            }

        }

        private double[,] AddData(int nRows, int nColumns, byte[] mapdata, bool isSixteenbit)
        {
            double[,] dataArray = new double[nRows, nColumns];
            double[] xarray = new double[nColumns];
            for (int i = 0; i < xarray.Length; i++)
            {
                xarray[i] = -3.0f + i * 0.25f;
            }
            double[] yarray = xarray;

            int mapindex = 0;
            for (int i = 0; i < dataArray.GetLength(0); i++)
            {
                for (int j = 0; j < dataArray.GetLength(1); j++)
                {
                    if (isSixteenbit)
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        byte val2 = (byte)mapdata.GetValue(mapindex++);
                        if (val1 == 0xff)
                        {
                            val1 = 0;
                            val2 = (byte)(0x100 - val2);
                        }
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        double value = (ival1 * 256) + ival2;
                        dataArray[i, j] = value;
                    }
                    else
                    {
                        byte val1 = (byte)mapdata.GetValue(mapindex++);
                        int ival1 = Convert.ToInt32(val1);

                        double value = ival1;
                        dataArray[i, j] = value;
                    }
                }
            }
            return dataArray;
        }

        private void barButtonItem21_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_symbols = new SymbolCollection();
            TryToOpenFile(m_currentfile, out m_symbols, m_currentfile_size);
            m_symbols.SortColumn = "Length";
            m_symbols.SortingOrder = GenericComparer.SortOrder.Descending;
            m_symbols.Sort();
            gridControlSymbols.DataSource = m_symbols;
        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartHexViewer();
            //StartSRAMHexViewer();
        }

        private void StartHexViewer()
        {
            if (m_currentfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DevExpress.XtraBars.Docking.DockPanel dockPanel;
                    //= dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    if (!m_appSettings.NewPanelsFloating)
                    {
                        dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    }
                    else
                    {
                        System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 10));
                        dockPanel = dockManager1.AddPanel(floatpoint);
                    }

                    dockPanel.Text = "Hexviewer: " + Path.GetFileName(m_currentfile);
                    HexViewer hv = new HexViewer();
                    hv.Issramviewer = false;
                    hv.Dock = DockStyle.Fill;
                    dockPanel.Width = 580;
                    hv.LoadDataFromFile(m_currentfile, m_symbols);
                    dockPanel.ClosedPanel += new DevExpress.XtraBars.Docking.DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Controls.Add(hv);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void tabdet_onSymbolSelectForFind(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            StartTableViewer(e.SymbolName);
        }


        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask the user for which value to search and if searching should include symbolnames and/or symbol description
            if (m_currentfile != "")
            {
                SymbolCollection result_Collection = new SymbolCollection();
                frmSearchMaps searchoptions = new frmSearchMaps();
                if (searchoptions.ShowDialog() == DialogResult.OK)
                {

                    SetProgress("Start searching data...");
                    SetProgressPercentage(0);

                    System.Windows.Forms.Application.DoEvents();
                    int cnt = 0;
                    SetProgress("Searching...");
                    foreach (SymbolHelper sh in m_symbols)
                    {

                        SetProgressPercentage((cnt * 100) / m_symbols.Count);
                        bool hit_found = false;
                        if (searchoptions.UseSpecificMapLength)
                        {
                            if (sh.Length != (int)searchoptions.MapLength)
                            {
                                continue;
                            }
                        }
                        if (searchoptions.IncludeSymbolNames)
                        {
                            if (searchoptions.SearchForNumericValues)
                            {
                                if (sh.Varname.Contains(searchoptions.NumericValueToSearchFor.ToString()))
                                {
                                    hit_found = true;
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor != string.Empty)
                                {
                                    if (sh.Varname.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }
                        if (searchoptions.IncludeSymbolDescription)
                        {
                            if (searchoptions.SearchForNumericValues)
                            {
                                if (sh.Description.Contains(searchoptions.NumericValueToSearchFor.ToString()))
                                {
                                    hit_found = true;
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor != string.Empty)
                                {
                                    if (sh.Description.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }
                        // now search the symbol data
                        if (sh.Flash_start_address < m_currentfile_size)
                        {
                            byte[] symboldata = readdatafromfile(m_currentfile, (int)sh.Flash_start_address, sh.Length);
                            if (searchoptions.SearchForNumericValues)
                            {
                                if (isSixteenBitTable(sh.Varname))
                                {
                                    for (int i = 0; i < symboldata.Length / 2; i += 2)
                                    {
                                        float value = Convert.ToInt32(symboldata.GetValue(i)) * 256;
                                        value += Convert.ToInt32(symboldata.GetValue(i + 1));
                                        value *= (float)GetMapCorrectionFactor(sh.Varname);
                                        value += (float)GetMapCorrectionOffset(sh.Varname);
                                        if (value == (float)searchoptions.NumericValueToSearchFor)
                                        {
                                            hit_found = true;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < symboldata.Length; i++)
                                    {
                                        float value = Convert.ToInt32(symboldata.GetValue(i));
                                        value *= (float)GetMapCorrectionFactor(sh.Varname);
                                        value += (float)GetMapCorrectionOffset(sh.Varname);
                                        if (value == (float)searchoptions.NumericValueToSearchFor)
                                        {
                                            hit_found = true;
                                        }
                                    }
                                }
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                if (searchoptions.StringValueToSearchFor.Length > symboldata.Length)
                                {
                                    // possible...
                                    string symboldataasstring = System.Text.Encoding.ASCII.GetString(symboldata);
                                    if (symboldataasstring.Contains(searchoptions.StringValueToSearchFor))
                                    {
                                        hit_found = true;
                                    }
                                }
                            }
                        }

                        if (hit_found)
                        {
                            // add to collection
                            result_Collection.Add(sh);
                        }
                        cnt++;
                    }
                    SetProgressIdle();
                    if (result_Collection.Count == 0)
                    {
                        MessageBox.Show("No results found...");
                    }
                    else
                    {
                        // start result screen
                        dockManager1.BeginUpdate();
                        try
                        {
                            SymbolTranslator st = new SymbolTranslator();
                            DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                            CompareResults tabdet = new CompareResults();
                            tabdet.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                            tabdet.SetFilterMode(m_appSettings.ShowAddressesInHex);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.UseForFind = true;
                            tabdet.Filename = openFileDialog1.FileName;
                            tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelectForFind);
                            dockPanel.Controls.Add(tabdet);
                            string resultText = "Search results: ";
                            if (searchoptions.SearchForNumericValues)
                            {
                                resultText += " number " + searchoptions.NumericValueToSearchFor.ToString();
                            }
                            if (searchoptions.SearchForStringValues)
                            {
                                resultText += " string " + searchoptions.StringValueToSearchFor;
                            }
                            dockPanel.Text = resultText;

                            dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);

                            dockPanel.Width = 700;

                            System.Data.DataTable dt = new System.Data.DataTable();
                            dt.Columns.Add("SYMBOLNAME");
                            dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                            dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                            dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                            dt.Columns.Add("DESCRIPTION");
                            dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                            dt.Columns.Add("CATEGORY"); //0
                            dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                            dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                            dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                            dt.Columns.Add("CATEGORYNAME");
                            dt.Columns.Add("SUBCATEGORYNAME");
                            dt.Columns.Add("SymbolNumber1", Type.GetType("System.Int32"));
                            dt.Columns.Add("SymbolNumber2", Type.GetType("System.Int32"));
                            string category = "";
                            string ht = string.Empty;
                            double diffperc = 0;
                            int diffabs = 0;
                            double diffavg = 0;
                            XDFCategories cat = XDFCategories.Undocumented;
                            XDFSubCategory subcat = XDFSubCategory.Undocumented;
                            foreach (SymbolHelper shfound in result_Collection)
                            {
                                string helptext = st.TranslateSymbolToHelpText(shfound.Varname, out ht, out cat, out subcat);
                                if (shfound.Varname.Contains("."))
                                {
                                    try
                                    {
                                        shfound.Category = shfound.Varname.Substring(0, shfound.Varname.IndexOf("."));
                                    }
                                    catch (Exception cE)
                                    {
                                        Console.WriteLine("Failed to assign category to symbol: " + shfound.Varname + " err: " + cE.Message);
                                    }
                                }
                                dt.Rows.Add(shfound.Varname, shfound.Start_address, shfound.Flash_start_address, shfound.Length, shfound.Length, helptext, false, 0, 0, 0, 0, shfound.Category, "", shfound.Symbol_number, shfound.Symbol_number);
                            }
                            tabdet.CompareSymbolCollection = result_Collection;
                            tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
                            tabdet.gridControl1.DataSource = dt.Copy();

                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                        dockManager1.EndUpdate();

                    }


                }
            }
        }

        private bool IsChristmasTime()
        {
            // test, return true
            if (DateTime.Now.Month == 12 && DateTime.Now.Day >= 20 && DateTime.Now.Day <= 26)
            {
                return true;
            }
            return false;
        }

        private bool IsHalloweenTime()
        {
            // test, return true
            if (DateTime.Now.Month == 10 && DateTime.Now.Day >= 30 && DateTime.Now.Day <= 31)
            {
                return true;
            }
            return false;
        }

        private bool IsValetineTime()
        {
            // test, return true
            if (DateTime.Now.Month == 2 && DateTime.Now.Day >= 13 && DateTime.Now.Day <= 14)
            {
                return true;
            }
            return false;
        }


        private void ShowChristmasWish()
        {
            int newyear = DateTime.Now.Year + 1;
            frmInfoBox info = new frmInfoBox("Merry christmas and a happy " + newyear.ToString("D4") + "\rDilemma");
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                m_msiUpdater = new msiupdater(new Version(System.Windows.Forms.Application.ProductVersion));
                m_msiUpdater.Apppath = System.Windows.Forms.Application.UserAppDataPath;
                m_msiUpdater.onDataPump += new msiupdater.DataPump(m_msiUpdater_onDataPump);
                m_msiUpdater.onUpdateProgressChanged += new msiupdater.UpdateProgressChanged(m_msiUpdater_onUpdateProgressChanged);
                m_msiUpdater.CheckForUpdates("Global", "http://trionic.mobixs.eu/t8suite/", "", "", false);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            if (IsChristmasTime())
            {
                ShowChristmasWish();
            }
        }

        void m_msiUpdater_onUpdateProgressChanged(msiupdater.MSIUpdateProgressEventArgs e)
        {

        }

        private void SetStatusText(string text)
        {
            barUpdateText.Caption = text;
            System.Windows.Forms.Application.DoEvents();
        }

        void m_msiUpdater_onDataPump(msiupdater.MSIUpdaterEventArgs e)
        {
            SetStatusText(e.Data);
            if (e.UpdateAvailable)
            {
                if (e.XMLFile != "" && e.Version.ToString() != "0.0")
                {
                    if (!this.IsDisposed)
                    {
                        try
                        {
                            this.Invoke(m_DelegateStartReleaseNotePanel, e.XMLFile, e.Version.ToString());
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }

                //this.Invoke(m_DelegateShowChangeLog, e.Version);
                frmUpdateAvailable frmUpdate = new frmUpdateAvailable();
                frmUpdate.SetVersionNumber(e.Version.ToString());
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.Blockauto_updates = false;
                }
                if (frmUpdate.ShowDialog() == DialogResult.OK)
                {
                    if (m_msiUpdater != null)
                    {
                        m_msiUpdater.ExecuteUpdate(e.Version);
                        System.Windows.Forms.Application.Exit();
                    }
                }
                else
                {
                    // user choose "NO", don't bug him again!
                    if (m_msiUpdater != null)
                    {
                        m_msiUpdater.Blockauto_updates = false;
                    }
                }
            }
        }

        public void GetRSSFeeds(Version newversion)
        {
            try
            {
                RSS2HTMLScoutLib.RSS2HTMLScout RSS2HTML = new RSS2HTMLScoutLib.RSS2HTMLScout();
                //RSS2HTML.ForceRefresh = true;
                RSS2HTML.ItemsPerFeed = 10; // limit 5 latest items per feed
                RSS2HTML.MainHeader = "<html><head><title>T8Suite changelog</title><!-- CSS source code will be inserted here -->{CSS}<!-- HTML page encoding. please change if needed --><!-- <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"> --></head><body>";
                //RSS2HTML.ChannelHeader = "<div class=\"ChannelHeader\"><table width=\"100%\" border=\"0\"><tr><td valign=\"middle\" align=\"left\"><a href=\"{LINK}\">{IMAGE}</a></td>      <td width=\"99%\" vAlign=middle align=middle><br><h3>{TITLE}</h3></td></tr></table></div>";
                RSS2HTML.ChannelHeader = "<div class=\"ChannelHeader\"><table width=\"100%\" border=\"0\"><tr><td valign=\"middle\" align=\"left\"><a href=\"{LINK}\"> {IMAGE}</a></td>      <td width=\"99%\" vAlign=middle align=middle><br><h2>{TITLE}</h2></td></tr></table>{DESCRIPTION}</div>";
                RSS2HTML.EnclosureTemplate = "<a href=\"{LINK}\">Image: {TITLE} ({LENGTH})</a>";
                RSS2HTML.ErrorMessageTemplate = "<p>Following feeds can not be displayed:<br>{FAILEDFEEDS}<br></p>";
                RSS2HTML.ItemTemplate = "<div class=\"ItemHeader\"><a href=\"{LINK}\">{TITLE}</a></div><div class=\"ItemDescription\">{DESCRIPTION}</div><div class=\"ItemFooter\">{AUTHOR} {DATE} {TIME} <a href=\"{COMMENTS}\">{COMMENTS} {ENCLOSURE}</a></div>";
                RSS2HTML.NewItemTemplate = "<div style=\"font-style: italic; background-color: #ead2d9\" class=\"NewItemHeader\"><a href=\"{LINK}\">{TITLE}</a></div><div class=\"NewItemDescription\">{DESCRIPTION}</div><div class=\"NewItemFooter\">{AUTHOR} {DATE} {TIME} <a href=\"{COMMENTS}\">{COMMENTS} {ENCLOSURE}</a></div>";
                RSS2HTML.MainFooter = "</body></html>";
                RSS2HTML.AddFeed("http://trionic.mobixs.eu/t8suite/" + newversion.ToString() + "/Notes.xml", 180); // ' update every 180 minutes (3 hours)
                RSS2HTML.Execute();
                RSS2HTML.SaveOutputToFile(System.Windows.Forms.Application.UserAppDataPath + "\\T8Suite.html");
            }
            catch (Exception E)
            {
                Console.WriteLine("Error getting RSS feeds: " + E.Message);
            }

        }

        private void ShowChangeLog(Version newversion)
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.UserAppDataPath + "\\T8Suite.html"))
                {
                    File.Delete(System.Windows.Forms.Application.UserAppDataPath + "\\T8Suite.html");
                }
                GetRSSFeeds(newversion);
                if (File.Exists(System.Windows.Forms.Application.UserAppDataPath + "\\T8Suite.html"))
                {
                    DevExpress.XtraBars.Docking.DockPanel panel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    panel.Text = "Change history";
                    WebBrowser wb = new WebBrowser();
                    panel.Width = 600;
                    wb.Dock = DockStyle.Fill;
                    panel.Controls.Add(wb);
                    panel.Show();
                    wb.Navigate(System.Windows.Forms.Application.UserAppDataPath + "\\T8Suite.html");
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

        }

        /// <summary>
        /// Sets or Unsets Full-Screen mode for this form, saving the old state values. 
        /// Note, the order of calls in this function is important.
        /// </summary>
        /// <param name="bFullScreen">set to fullscreen if true, unset if false</param>
        void SetFullScreenMode(bool bFullScreen)
        {
            // enable full screen mode only if we're NOT in fullscreen
            if (bFullScreen && !_isFullScreenEnabled)
            {
                this.SuspendLayout();

                // get current window state
                _oldWindowState = this.WindowState;

                // get the normal window state so we don't lose those values
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;

                // get the normal state values
                _oldClientSize = this.ClientSize;
                _oldDesktopBounds = this.DesktopBounds;

                // jump to full screen
                this.FormBorderStyle = FormBorderStyle.None;
                this.DesktopBounds = Screen.FromControl(this).Bounds;

                _isFullScreenEnabled = true;

                this.ResumeLayout();
            }

            // disable full screen mode only if we're in fullscreen
            if (!bFullScreen && _isFullScreenEnabled)
            {
                this.SuspendLayout();

                // reset the old state
                this.DesktopBounds = _oldDesktopBounds;
                this.ClientSize = _oldClientSize;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = _oldWindowState;

                _isFullScreenEnabled = false;

                this.ResumeLayout();
            }
        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_isFullScreenEnabled)
            {
                SetFullScreenMode(true);
                TopMost = true;
            }
            else
            {
                SetFullScreenMode(false);
                TopMost = false;
            }
        }

        private void CreateBinaryBackup(string filename, bool ShowMessages)
        {
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(filename))
                {
                    if (m_CurrentWorkingProject != "")
                    {
                        if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups");
                        string newfilename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                        File.Copy(GetBinaryForProject(m_CurrentWorkingProject), newfilename);
                    }
                    else
                    {
                        File.Copy(filename, Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup", true);
                        if (ShowMessages)
                        {
                            MessageBox.Show("Backup created: " + Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup");
                        }
                    }

                }
            }

        }

        private void barButtonItem25_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateBinaryBackup(m_currentfile, true);
        }

        private void barButtonItem26_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            sfd.Title = "Save current file as... ";
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.Copy(m_currentfile, sfd.FileName, true);
                if (MessageBox.Show("Do you want to open the newly saved file?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_currentfile = sfd.FileName;//Application.StartupPath + "\\55559437  81f.bin";
                    CloseProject();
                    m_currentfile = sfd.FileName;//Application.StartupPath + "\\55559437  81f.bin";
                    TryToOpenFile(m_currentfile, out m_symbols, m_currentfile_size);


                    this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
                    barFilenameText.Caption = Path.GetFileName(m_currentfile);
                    gridControlSymbols.DataSource = m_symbols;
                    m_appSettings.Lastfilename = m_currentfile;
                    gridViewSymbols.BestFitColumns();
                    UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);

                    m_appSettings.LastOpenedType = 0;
                }
            }
        }
        private void CopySymbol(string symbolname, string fromfilename, int fromflashaddress, int fromlength, string targetfilename, int targetflashaddress, int targetlength)
        {
            if (fromlength != targetlength)
            {
                AddToResumeTable("Unable to transfer symbol " + symbolname + " because source and target lengths don't match!");
            }
            else
            {
                try
                {
                    while (fromflashaddress > m_currentfile_size) fromflashaddress -= m_currentfile_size;
                    FileInfo fi = new FileInfo(targetfilename);
                    while (targetflashaddress > fi.Length) targetflashaddress -= (int)fi.Length;
                    byte[] mapdata = readdatafromfile(fromfilename, fromflashaddress, fromlength);
                    savedatatobinary(targetflashaddress, targetlength, mapdata, targetfilename, true);
                    AddToResumeTable("Transferred symbol " + symbolname + " successfully");
                }
                catch (Exception E)
                {
                    AddToResumeTable("Failed to transfer symbol " + symbolname + ": " + E.Message);
                }
            }
        }

        private bool CanTransfer(SymbolHelper sh)
        {
            // FOR TESTING ONLY
            //return true;
            bool retval = false;
            if (sh.Varname.Contains(".") || sh.Userdescription.Contains(".")) retval = true;
            return retval;
        }

        private void TransferMapsToNewBinary(string filename)
        {
            SymbolCollection curSymbolCollection = new SymbolCollection();
            //AddressLookupCollection curAddressLookupCollection = new AddressLookupCollection();
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");

            if (filename != string.Empty)
            {
                frmTransferSelectionSymbolsSelection frmtransfer = new frmTransferSelectionSymbolsSelection();
                SymbolCollection _onlyFlashSymbols = new SymbolCollection();
                foreach (SymbolHelper shcopy in m_symbols)
                {
                    if (shcopy.Flash_start_address > 0 && GetSymbolAddress(m_symbols, shcopy.Varname) < 1048576 && shcopy.Length > 0 && CanTransfer(shcopy))
                    {
                        _onlyFlashSymbols.Symbols.Add(shcopy);
                    }
                }
                frmtransfer.Symbols = /*m_symbols*/_onlyFlashSymbols;
                if (frmtransfer.ShowDialog() == DialogResult.OK)
                {
                    SetProgress("Initializing");
                    SetProgress("Creating backup file...");
                    File.Copy(filename, Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetransferringmaps.bin", true);
                    AddToResumeTable("Backup file created (" + Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetransferringmaps.bin)");
                    AddToResumeTable("Transferring data from " + Path.GetFileName(m_currentfile) + " to " + filename);



                    bool m_fileparsed = false;

                    //listView1.Items.Clear();
                    SetStatusText("Start symbol parsing");


                    FileInfo fi = new FileInfo(filename);
                    TryToOpenFile(filename, out curSymbolCollection, (int)fi.Length);

                    curSymbolCollection.SortColumn = "Flash_start_address";
                    curSymbolCollection.SortingOrder = GenericComparer.SortOrder.Ascending;
                    curSymbolCollection.Sort();
                    SetProgress("Start transfer");
                    int symidx = 0;
                    foreach (SymbolHelper sh in curSymbolCollection)
                    {
                        if (sh.Flash_start_address > 0 && sh.Flash_start_address < m_currentfile_size && sh.Length < 0x1000)
                        {
                            foreach (SymbolHelper cfsh in m_symbols)
                            {
                                if (CanTransfer(cfsh))
                                {
                                    if (cfsh.Varname == sh.Varname || cfsh.Userdescription == sh.Varname)
                                    {
                                        string symbolname = cfsh.Varname;
                                        if (symbolname.StartsWith("Symbolnumber"))
                                        {
                                            if (!sh.Varname.StartsWith("Symbolnumber")) symbolname = sh.Varname;
                                            else if (sh.Userdescription != "") symbolname = sh.Userdescription;
                                            else if (cfsh.Userdescription != "") symbolname = cfsh.Userdescription;
                                        }
                                        if (SymbolInTransferCollection(frmtransfer.Symbols, symbolname))
                                        {
                                            SetProgress("Transferring: " + sh.Varname);
                                            int percentage = symidx * 100 / curSymbolCollection.Count;
                                            SetProgressPercentage(percentage);
                                            CopySymbol(sh.Varname, m_currentfile, (int)cfsh.Flash_start_address, cfsh.Length, filename, (int)sh.Flash_start_address, sh.Length);
                                        }
                                    }
                                }
                            }
                        }
                        symidx++;
                    }

                    UpdateChecksum(filename, m_appSettings.AutoChecksum);
                    SetStatusText("Idle.");
                    SetProgressIdle();
                }

            }
        }

        private bool SymbolInTransferCollection(SymbolCollection transferCollection, string mapname)
        {
            foreach (SymbolHelper sh_test in transferCollection)
            {
                if (sh_test.Selected && (sh_test.Varname == mapname || sh_test.Userdescription == mapname))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddToResumeTable(string description)
        {
            if (resumeTuning != null)
            {
                if (resumeTuning.Columns.Count == 1)
                {
                    resumeTuning.Rows.Add(description);
                }
            }
        }

        private void barButtonItem27_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask for another bin file
            if (m_currentfile != string.Empty)
            {

                PSTaskDialog.cTaskDialog.ForceEmulationMode = false;
                PSTaskDialog.cTaskDialog.EmulatedFormWidth = 600;
                PSTaskDialog.cTaskDialog.UseToolWindowOnXP = false;
                PSTaskDialog.cTaskDialog.VerificationChecked = true;
                PSTaskDialog.cTaskDialog.ShowTaskDialogBox("Transfer maps to different binary wizard", "This wizard assists you in transferring map contents from the current file to another binary.", "Make sure engine types and such are equal for both binaries!", "Happy driving!!!\nDilemma © 2008", "The author does not take responsibility for any damage done to your car or other objects in any form!", "Show me a summary after transferring data.", "", "Yes, let me select the target binary|No thanks!", eTaskDialogButtons.None, eSysIcons.Information, eSysIcons.Warning);
                switch (PSTaskDialog.cTaskDialog.CommandButtonResult)
                {
                    case 0:
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            TransferMapsToNewBinary(openFileDialog1.FileName);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case 1:
                        // cancel
                        break;
                }
                if (PSTaskDialog.cTaskDialog.VerificationChecked && PSTaskDialog.cTaskDialog.CommandButtonResult != 1)
                {
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Data transfer report";
                    tuningrep.DataSource = resumeTuning;
                    tuningrep.CreateReport();
                    tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);
                }
            }
        }

        private int TorqueToAirmass(int torque, bool E85)
        {
            double airmass = Convert.ToDouble(torque) * 3.1;
            if (E85)
            {
                airmass /= 1.07;
            }
            return Convert.ToInt32(airmass);
        }

        private int PowerToTorque(int power, int rpm, bool doCorrection)
        {
            double torque = (power * 7024) / rpm;
            double correction = 1;
            if (rpm >= 6000) correction = 0.88;
            else if (rpm > 5800) correction = 0.90;
            else if (rpm > 5400) correction = 0.92;
            else if (rpm > 5000) correction = 0.95;
            else if (rpm > 4600) correction = 0.98;
            if (doCorrection)
            {
                torque /= correction;
            }
            return Convert.ToInt32(torque);
        }

        private int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7024;
            return Convert.ToInt32(power);
        }

        private int AirmassToTorque(int airmass, bool E85)
        {
            double tq = Convert.ToDouble(airmass) / 3.1;
            if (E85)
            {
                tq *= 1.07;
            }
            return Convert.ToInt32(tq);
        }

        private byte[] GetPedalMap(int maxairmass, int peakHP, bool E85)
        {
            // PedalMapCal.n_EngineMap is rpm axis
            // 700, 880, 1260, 2020, 2400 etc
            byte[] rpm_axis = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.n_EngineMap"), GetSymbolLength(m_symbols, "PedalMapCal.n_EngineMap"));

            int[] airmasspercentage = new int[16 * 16] {96,  97,  99, 100, 100, 100, 100, 100, 100, 100, 99,  98,  95,  90,  85,  83,
                                                        92,  92,  92,  92,  92,  92,  92,  92,  89,  82, 72,  68,  64,  60,  60,  60,
                                                        85,  85,  85,  85,  85,  85,  85,  81,  71,  64, 59,  55,  53,  50,  49,  48,
                                                        79,  79,  79,  79,  79,  77,  75,  69,  59,  54, 50,  47,  45,  43,  42,  42,
                                                        73,  73,  73,  71,  69,  67,  64,  54,  48,  44, 41,  39,  37,  36,  36,  36,
                                                        67,  67,  65,  63,  59,  56,  48,  42,  38,  36, 34,  32,  30,  30,  29,  29,
                                                        59,  59,  55,  50,  46,  40,  34,  31,  29,  27, 26,  25,  25,  24,  24,  24,
                                                        50,  48,  42,  37,  34,  29,  26,  24,  23,  21, 20,  20,  20,  19,  19,  19,
                                                        36,  33,  29,  27,  24,  22,  19,  18,  17,  16, 15,  15,  15,  15,  15,  15,
                                                        24,  23,  21,  19,  18,  16,  15,  12,  11,  10, 10,  10,  10,  10,  10,  10,
                                                        20,  18,  15,  12,   9,   6,   5,   5,   5,   5,  5,   5,   5,   5,   5,  5,
                                                        16,  13,  10,   7,   5,   4,   3,   3,   3,   2,  2,   2,   2,   2,   2,  2,
                                                        10,   8,   5,   4,   3,   2,   2,   2,   2,   2,  2,   2,   2,   2,   2,  2,
                                                         5,   4,   3,   2,   1,   1,   1,   1,   1,   1,  1,   1,   1,   1,   1,  1,
                                                         2,   1,   1,   0,   0,   0,   0,   0,   0,   0,  0,   0,   0,   0,   0,  0,
                                                         0,   0,   0,   0,   0,   0,   0,   0,   0,   0,  0,   0,   0,   0,   0,  0};

            byte[] returnvalue = new byte[16 * 16 * 2]; // in bytes
            int cell = 0;
            for (int column = 0; column < 16; column++)
            {
                // calculate RPM indication

                for (int row = 0; row < 16; row++)
                {
                    int rpm = Convert.ToInt32(rpm_axis[row * 2]) * 256 + Convert.ToInt32(rpm_axis[(row * 2) + 1]);
                    int valueincell = (maxairmass * airmasspercentage[(row) + (16 * (15 - column))]) / 100;

                    //int torqueincell = AirmassToTorque(valueincell, E85);
                    //if (TorqueToPower(torqueincell, rpm) > peakHP)
                    //{
                    // set to max
                    //int maxtorqueforcell = PowerToTorque(peakHP, rpm, false);
                    //int maxairmassforcell = TorqueToAirmass(maxtorqueforcell, E85);
                    //Console.WriteLine("Setting " + valueincell.ToString() + " to " + maxairmassforcell.ToString() + " at " + rpm.ToString() + " rpm");
                    //valueincell *= 10; // T8 has 10 factor
                    valueincell = valueincell;
                    //}


                    byte b1 = Convert.ToByte(valueincell / 256);
                    byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                    returnvalue[cell * 2] = b1;
                    returnvalue[(cell * 2) + 1] = b2;
                    cell++;
                }
            }
            return returnvalue;
        }

        private byte[] GetAirTorqueCalibration(int maxairmass)
        {
            int[] airtrqcalib = new int[16 * 16] {      6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100,
                                                        6, 9, 12, 20, 25, 32, 39, 45,51, 58, 65, 73, 81, 87, 93, 100};
            byte[] returnvalue = new byte[16 * 16 * 2]; // in bytes
            int cell = 0;
            for (int column = 0; column < 16; column++)
            {
                for (int row = 0; row < 16; row++)
                {
                    int valueincell = (maxairmass * airtrqcalib[(row) + (16 * (15 - column))]) / 100;
                    byte b1 = Convert.ToByte(valueincell / 256);
                    byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                    returnvalue[cell * 2] = b1;
                    returnvalue[(cell * 2) + 1] = b2;
                    cell++;
                }
            }
            return returnvalue;
        }

        private byte[] GetNominalTorqueMap(int maxairmass)
        {
            /*   int[] airtrqcalib = new int[18 * 16] {      0xFFDF, 0xFFEA, 0xFFF2, 0xFFFA, 8, 29, 53, 77, 99,124, 143, 163, 185, 204, 219, 236, 256, 283,
                                                           0xFFE2, 0xFFEC, 0xFFF3, 0xFFFB,10, 32, 56, 77,100,124, 146, 167, 189, 209, 225, 241, 260, 283,
                                                           0xFFE4, 0xFFEE, 0xFFF7, 0xFFFF,12, 32, 58, 78,101,124, 151, 171, 192, 215, 233, 248, 264, 283,
                                                           0xFFE8, 0xFFF2, 0xFFFB,      3,16, 39, 62, 85,107,130, 154, 178, 198, 219, 238, 254, 266, 283,
                                                           0xFFE8, 0xFFF3, 0xFFFE,      5,18, 41, 64, 88,110,132, 154, 179, 203, 223, 242, 258, 272, 288,
                                                           0xFFE8, 0xFFF3, 0xFFFD,      5,18, 42, 66, 92,114,136, 158, 178, 200, 227, 244, 260, 276, 296,
                                                           0xFFE9, 0xFFF3, 0xFFFD,      4,17, 43, 67, 91,116,139, 161, 181, 196, 224, 243, 259, 274, 297,
                                                           0xFFEC, 0xFFF6,      1,      8,23, 45, 69, 93,117,142, 163, 185, 205, 221, 246, 263, 277, 294,
                                                           0xFFEE, 0xFFFB,      4,     11,26, 49, 72, 96,119,143, 167, 187, 206, 225, 247, 263, 278, 294,
                                                           0xFFEE, 0xFFFB,      4,     11,27, 52, 74, 98,121,144, 168, 191, 213, 231, 248, 261, 275, 295,
                                                           0xFFEF, 0xFFFA,      3,     11,27, 53, 74, 98,120,145, 167, 187, 211, 232, 245, 261, 277, 300,
                                                           0xFFEF, 0xFFF9,      2,     10,27, 53, 75, 96,120,143, 166, 185, 203, 226, 244, 257, 269, 290,
                                                           0xFFEF, 0xFFF9,      2,     10,27, 53, 75, 96,120,143, 166, 185, 203, 226, 244, 257, 269, 290,
                                                           0xFFF0, 0xFFFA,      3,     10,27, 51, 75, 97,118,140, 161, 181, 202, 222, 237, 252, 267, 290,
                                                           0xFFEF, 0xFFF7, 0xFFFF,      9,27, 52, 74, 98,120,138, 160, 184, 202, 222, 237, 252, 267, 290,
                                                           0xFFF0, 0xFFF7, 0xFFFF,     11,31, 55, 79,101,122,146, 177, 189, 202, 222, 237, 252, 267, 290};*/
            int[] airtrqcalib = new int[18 * 16] {         0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300,
                                                           0xFFD8, 0xFFFB, 10, 30, 50, 65, 80, 100, 120,140, 160, 180, 195, 210, 230, 250, 275, 300};

            byte[] returnvalue = new byte[18 * 16 * 2]; // in bytes

            int cell = 0;
            for (int column = 0; column < 16; column++)
            {
                for (int row = 0; row < 18; row++)
                {
                    //int valueincell = (airtrqcalib[(row) + (16 * (18 - column) -1)]) / 100;
                    int valueincell = (airtrqcalib[(row) + (18 * (15 - column))]);
                    if (valueincell < 32000)
                    {
                        valueincell *= 10; // T8 has factor 10
                    }
                    byte b1 = Convert.ToByte(valueincell / 256);
                    byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                    returnvalue[cell * 2] = b1;
                    returnvalue[(cell * 2) + 1] = b2;
                    cell++;
                }
            }
            return returnvalue;
        }

        private byte[] GetTorqueCalEngineSupportPoints(int maxairmass)
        {
            // 16 hardcoded values
            int[] supportpoints = new int[16] { 0xFF38, 0, 200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000, 2250, 2500, 2750, 3000 };

            byte[] returnvalue = new byte[32];
            for (int i = 0; i < 16; i++)
            {
                int valueincell = Convert.ToInt32(supportpoints.GetValue(i));
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;
        }

        private byte[] GetTurboSpeedLimiter(int maxairmass, int length)
        {
            byte[] returnvalue = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                int valueincell = (maxairmass);
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;

        }


        private byte[] GetAutAccelPedalSupportPoints(int maxairmass)
        {
            // 6 steps from 0 to maxairmass
            byte[] returnvalue = new byte[12];
            int range = maxairmass;
            int airmassperstep = range / 5;
            for (int i = 0; i < 6; i++)
            {
                int valueincell = (airmassperstep * i);
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;
        }


        private byte[] GetTorqueCalSupportPoints(int maxairmass)
        {
            // 17 steps from 80 to maxairmass
            byte[] returnvalue = new byte[36];
            int range = maxairmass - 80;
            int airmassperstep = range / 17;
            for (int i = 0; i < 18; i++)
            {
                int valueincell = 80 + (airmassperstep * i);
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;
        }

        private byte[] GetBoostMapSupportPoints(int maxairmass)
        {
            // 7 steps from 500 to maxairmass
            byte[] returnvalue = new byte[16];
            int range = maxairmass - 500;
            int airmassperstep = range / 7;
            for (int i = 0; i < 8; i++)
            {
                int valueincell = 500 + (airmassperstep * i);
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;
        }

        private byte[] GetBoostRegMap(int maxairmass)
        {
            int[] airtrqcalib = new int[8 * 16] {      0, 140, 200, 280, 450, 630, 730, 900,
                                                       0, 140, 200, 280, 450, 630, 700, 840,
                                                       0, 140, 200, 270, 400, 500, 640, 780,
                                                       0, 140, 170, 220, 370, 450, 580, 720,
                                                       0, 140, 180, 210, 350, 450, 560, 660,
                                                       0, 150, 180, 230, 390, 480, 550, 600,
                                                       0, 170, 210, 240, 300, 400, 530, 610,
                                                       0, 180, 230, 260, 300, 400, 580, 600,
                                                       0, 200, 270, 290, 300, 400, 600, 650,
                                                       0, 200, 280, 300, 300, 400, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650,
                                                       0, 250, 280, 370, 300, 450, 600, 650};
            byte[] returnvalue = new byte[8 * 16 * 2]; // in bytes
            int cell = 0;
            for (int column = 0; column < 16; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    int valueincell = (airtrqcalib[(row) + (8 * (15 - column))]);
                    byte b1 = Convert.ToByte(valueincell / 256);
                    byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                    returnvalue[cell * 2] = b1;
                    returnvalue[(cell * 2) + 1] = b2;
                    cell++;
                }
            }
            return returnvalue;
        }

        private byte[] GetAirmassLimiter(int maxairmass)
        {
            int[] airmasslimit = new int[16 * 16] { 40, 40, 40, 44, 45, 47, 49, 53, 55,60, 60, 68, 72, 79, 85, 92,
                                                    41, 42, 43, 46, 49, 52, 54, 58, 60,64, 66, 74, 78, 84, 90, 98,
                                                    44, 44, 46, 50, 54, 57, 60, 62, 65,68, 70, 77, 82, 88, 94, 100,
                                                    46, 46, 48, 56, 60, 62, 64, 66, 69,72, 74, 84, 90, 92, 95, 100,
                                                    47, 47, 49, 58, 62, 66, 68, 70, 77,80, 84, 87, 93, 94, 96, 100,
                                                    47, 47, 50, 60, 64, 70, 72, 74, 81,84, 88, 92, 96, 97, 98, 100,
                                                    48, 48, 52, 61, 66, 71, 74, 76, 83,86, 90, 94, 97, 98, 99, 100,
                                                    49, 50, 53, 62, 67, 72, 75, 77, 83,86, 90, 94, 97, 98, 99, 100,
                                                    49, 50, 53, 62, 67, 72, 75, 77, 83,86, 90, 94, 97, 98, 99, 100,
                                                    48, 48, 52, 61, 66, 71, 74, 76, 83,86, 90, 94, 97, 98, 99, 100,
                                                    47, 47, 50, 60, 64, 70, 72, 74, 81,84, 88, 92, 96, 97, 98, 100,
                                                    47, 47, 49, 58, 62, 66, 68, 70, 77,80, 84, 87, 93, 94, 96, 100,
                                                    46, 46, 48, 51, 52, 53, 54, 59, 60,62, 62, 70, 74, 80, 85, 88,
                                                    44, 44, 46, 50, 50, 50, 50, 50, 51,53, 55, 61, 64, 69, 73, 77,
                                                    41, 42, 43, 46, 46, 46, 46, 46, 46,46, 46, 46, 46, 46, 46, 46,
                                                    41, 42, 43, 46, 46, 46, 46, 46, 46,46, 46, 46, 46, 46, 46, 46};
            byte[] returnvalue = new byte[16 * 16 * 2]; // in bytes
            int cell = 0;
            for (int column = 0; column < 16; column++)
            {
                for (int row = 0; row < 16; row++)
                {
                    int valueincell = (maxairmass * airmasslimit[(row) + (16 * (15 - column))]) / 100;
                    byte b1 = Convert.ToByte(valueincell / 256);
                    byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                    returnvalue[cell * 2] = b1;
                    returnvalue[(cell * 2) + 1] = b2;
                    cell++;
                }
            }
            return returnvalue;
        }

        private void barButtonItem28_ItemClick(object sender, ItemClickEventArgs e)
        {
            // tune to stage 1!!!
            if (File.Exists(m_currentfile))
            {
                // read tuned marker to see if binary has been tuned before
                T8Header fh = new T8Header();
                fh.init(m_currentfile);
                PartNumberConverter pnc = new PartNumberConverter();
                ECUInformation ecuinfo = pnc.GetECUInfo(fh.PartNumber, "");


                // start window to ask user for desired horses (depending on E85 usage?)
                frmTuneBinary frmTune = new frmTuneBinary();
                if (frmTune.ShowDialog() == DialogResult.OK)
                {
                    int desiredHP = frmTune.DesiredHP;
                    bool carRunsE85 = frmTune.CarRunsE85;

                    // convert HP to torque and airmass
                    //int torque = PowerToTorque(desiredHP, 6000);
                    //int peaktorque = (110 * torque) / 100;
                    int peaktorque = frmTune.PeakTorque;
                    int maxairmass = TorqueToAirmass(peaktorque, carRunsE85);

                    // give user a warning if airmass/c > 1300 mg/c
                    if (maxairmass > 1300)
                    {
                        MessageBox.Show("Your maximum requested airmass exceeds 1300 mg/c [" + maxairmass.ToString() + " mg/c]. Please make sure all your other maps will support this airflow!", "Warning!", MessageBoxButtons.OK);
                    }

                    string msg = "Partnumber not recognized, tuning will continue anyway, please verify settings afterwards";
                    msg += Environment.NewLine + " desired airmass " + maxairmass.ToString() + " mg/c and peak torque " + peaktorque.ToString() + " Nm";
                    if (ecuinfo.Valid)
                    {
                        msg = "Partnumber " + fh.PartNumber + ", carmodel " + ecuinfo.Carmodel.ToString() + ", engine " + ecuinfo.Enginetype.ToString();
                        msg += Environment.NewLine + " desired airmass " + maxairmass.ToString() + " mg/c and peak torque " + peaktorque.ToString() + " Nm";
                    }
                    PSTaskDialog.cTaskDialog.ForceEmulationMode = false;
                    PSTaskDialog.cTaskDialog.EmulatedFormWidth = 600;
                    PSTaskDialog.cTaskDialog.UseToolWindowOnXP = false;
                    PSTaskDialog.cTaskDialog.VerificationChecked = true;
                    PSTaskDialog.cTaskDialog.ShowTaskDialogBox("Tune me up™ wizard", "This wizard will tune your binary.", "Several maps will be altered" + Environment.NewLine + msg, "Happy driving!!!\nDilemma © 2009", "The author does not take responsibility for any damage done to your car or other objects in any form!", "Show me a summary after tuning", "", "Yes, tune me up!|No thanks!", eTaskDialogButtons.None, eSysIcons.Information, eSysIcons.Warning);
                    switch (PSTaskDialog.cTaskDialog.CommandButtonResult)
                    {
                        case 0:
                            // tune to stage x
                            // must use fixed maps, scaled to be good for the current airmass (e.g. pedalrequest map)
                            //TuneToStageNew(1, maxairmass, peaktorque, desiredHP, ecuinfo.Enginetype, carRunsE85);
                            TuneMeUp(maxairmass, peaktorque, desiredHP, ecuinfo.Enginetype, carRunsE85);
                            break;
                        case 1:
                            // cancel
                            break;
                    }
                    if (PSTaskDialog.cTaskDialog.VerificationChecked && PSTaskDialog.cTaskDialog.CommandButtonResult != 1)
                    {
                        TuningReport tuningrep = new TuningReport();
                        tuningrep.DataSource = resumeTuning;
                        tuningrep.CreateReport();
                        tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);
                    }
                }
            }
        }

        private int GetMaxTorque()
        {
            int retval = 300;
            /*byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(m_symbols, "TorqueCal.M_EngXSP"));
            retval = Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 2)) * 256;
            retval += Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 1));
            Console.WriteLine("Max torque from table = " + retval.ToString());*/
            return retval;
        }

        private void TuneNominalTorqueMap(double maxairmass)
        {
            SetProgress("Tuning TrqMastCal.Trq_NominalMap...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap") > 0)
            {
                byte[] nominalMap = GetNominalTorqueMap(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"), nominalMap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map (TrqMastCal.Trq_NominalMap)");
            }

        }

        private void TuneAirTorqueCalibration(double maxairmass)
        {
            SetProgress("Tuning TrqMastCal.m_AirTorqMap...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap") > 0)
            {
                byte[] AirTorqueCalibration = GetAirTorqueCalibration(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirTorqMap"), AirTorqueCalibration, m_currentfile, true);
                AddToResumeTable("Tuned nominal airmass map (TrqMastCal.m_AirTorqMap)");
            }
        }

        private void TuneAirmassLimiters(double maxairmass)
        {
            //BstKnkCal.MaxAirmass & BstKnkCal.MaxAirmassAu -> MaxAirmassrequest
            SetProgress("Tuning BstKnkCal.MaxAirmass...");
            /********** BstKnkCal.MaxAirmass ***********/
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass") > 0)
            {
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for manual transmission (BstKnkCal.MaxAirmass)");
            }
            SetProgress("Tuning BstKnkCal.MaxAirmassAu...");
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu") > 0)
            {
                /********** BstKnkCal.MaxAirmassAu ***********/
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for automatic transmission (BstKnkCal.MaxAirmassAu)");
            }
        }

        private void SetTableContentTo(string mapname, int value)
        {
            try
            {
                value *= 10;
                SetProgress("Tuning " + mapname);
                byte[] mapdata = new byte[GetSymbolLength(m_symbols, mapname)];
                for (int i = 0; i < mapdata.Length; i += 2)
                {
                    byte b1 = Convert.ToByte(value / 256);
                    byte b2 = Convert.ToByte(value - (int)b1 * 256);
                    mapdata[i] = b1;
                    mapdata[i + 1] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, mapname), mapdata.Length, mapdata, m_currentfile, true);
            }
            catch (Exception)
            {

            }
        }

        private void TuneTorqueLimiter3d(string limitername, int maxtorque)
        {
            SetProgress("Tuning " + limitername);
            if ((int)GetSymbolAddress(m_symbols, limitername) > 0)
            {
                byte[] maxtorqueaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, limitername), GetSymbolLength(m_symbols, limitername));
                int cols = 0;
                int rows = 0;

                rows = maxtorqueaut.Length;

                if (isSixteenBitTable(limitername)) rows /= 2;
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt *  2);
                    int offset2 = (rt *  2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorqueaut[offset1]) * 256 + Convert.ToInt32(maxtorqueaut[offset2]);
                    boostcalvalue = maxtorque * 10;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, limitername), GetSymbolLength(m_symbols, limitername), maxtorqueaut, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter " + limitername);
            }
        }

        private void TuneTorqueLimiter(string limitername, int maxtorque)
        {
            SetProgress("Tuning " + limitername);
            if ((int)GetSymbolAddress(m_symbols, limitername) > 0)
            {
                byte[] maxtorqueaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, limitername), GetSymbolLength(m_symbols, limitername));
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, limitername, out cols, out rows);
                if (isSixteenBitTable(limitername)) rows /= 2;
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorqueaut[offset1]) * 256 + Convert.ToInt32(maxtorqueaut[offset2]);
                    boostcalvalue = maxtorque * 10;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, limitername), GetSymbolLength(m_symbols, limitername), maxtorqueaut, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter " + limitername);
            }
        }

        private void TuneTorqueLimiters(int maxtorque)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname.StartsWith("TrqLimCal.Trq_MaxEngine") || sh.Userdescription.StartsWith("TrqLimCal.Trq_MaxEngine"))
                {
                    string limitername = sh.Varname;
                    if (limitername.StartsWith("Symbol")) limitername = sh.Userdescription;
                    TuneTorqueLimiter(limitername, maxtorque);
                }
                else if (sh.Varname.StartsWith("TMCCal.Trq_MaxEngine") || sh.Userdescription.StartsWith("TMCCal.Trq_MaxEngine"))
                {
                    string limitername = sh.Varname;
                    if (limitername.StartsWith("Symbol")) limitername = sh.Userdescription;
                    TuneTorqueLimiter(limitername, maxtorque);
                }
            }
            TuneTorqueLimiter("TrqLimCal.Trq_ManGear", maxtorque);
            TuneTorqueLimiter("TrqLimCal.TrqOverBoostTab", maxtorque);

            TuneTorqueLimiter3d("FFTrqCal.M_maxMAP", maxtorque);

            SetTableContentTo("TrqLimCal.Trq_CompressorNoiseRedLimMAP", maxtorque);
            TuneTorqueLimiter("TrqLimCal.Trq_AutNoiseRedLim", maxtorque);
            TuneTorqueLimiter("TrqLimCal.Trq_NoiseRedLim", maxtorque);
        }

        private void TuneFuelcutLimiter(double maxairmass)
        {
            /********** FCutCal.m_AirInletLimit ***********/

            SetProgress("Tuning FCutCal.m_AirInletLimit...");
            // write xxx to fuelcut limit
            if ((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                byte[] fuelcutmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"));
                if (fuelcutmap.Length == 2)
                {
                    // airmass fuelcut should be ~20% higher than max request
                    double m_AirmassFuelcut = maxairmass * 1.2;
                    int i_AirmassFuelcut = Convert.ToInt32(m_AirmassFuelcut);
                    //if (i_AirmassFuelcut >= 1800) i_AirmassFuelcut = 1800;
                    //TODO: FUELCUT
                    byte b1 = Convert.ToByte(i_AirmassFuelcut / 256);
                    byte b2 = Convert.ToByte(i_AirmassFuelcut - (int)b1 * 256);
                    fuelcutmap[0] = b1;
                    fuelcutmap[1] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned fuelcut limiter (FCutCal.m_AirInletLimit)");
            }
        }

        private void TuneMeUp(double maxairmass, double maxtorque, double peakHP, EngineType enginetype, bool E85)
        {
            /*

MaxEngSpdCal.m_EngMin -> 6200
TMCCal.Trq_MaxEngineTab -> MaxTrq 320
TrqLimCal.Trq_MAxEngineMan* -> 320
TrqLimCal.Trq_MAxEngineAut* -> 320
TrqLimCal.TrqOverBoostTab -> 330

TrqMastCal.Trq_NominalMap -> 1050 = 340 Nm
TrqMastCal.m_AirTorqMap -> 325 Nm = 1300 mg/c             * */

            SetProgress("Checking current configuration...");
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
            AddToResumeTable("Tuning your binary to : " + maxairmass.ToString() + " mg/c");
            SetProgress("Creating backup file...");
            int imaxairmass = Convert.ToInt32(maxairmass);
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin", true);
            AddToResumeTable("Backup file created (" + Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin" + ")");
            int max_torque = 320;
            TuneNominalTorqueMap(maxairmass);
            TuneAirTorqueCalibration(maxairmass);
            TuneAirmassLimiters(maxairmass);
            TuneTorqueLimiters(max_torque);
            TuneFuelcutLimiter(maxairmass);
            // all done with changes, update the checksum!
            UpdateChecksum(m_currentfile, true);
            SetProgress("Generating report...");
            AddToResumeTable("Updated checksum.");
            SetProgressIdle();

        }

        private void TuneToStageNew(int stage, double maxairmass, double maxtorque, double peakHP, EngineType enginetype, bool E85)
        {
            SetProgress("Checking current configuration...");
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
            AddToResumeTable("Tuning your binary to stage: " + stage.ToString());

            SetProgress("Creating backup file...");
            int imaxairmass = Convert.ToInt32(maxairmass);
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin", true);
            AddToResumeTable("Backup file created (" + Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin" + ")");

            // tune maps
            //TorqueCal.M_EngXSP
            SetProgress("Tuning TrqMastCal.Trq_PedYSP...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_PedYSP") > 0)
            {
                byte[] TorqCal = GetTorqueCalEngineSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_PedYSP"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_PedYSP"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned y axis for torque pedal map (TrqMastCal.Trq_PedYSP)");
            }

            SetProgress("Tuning TrqMastCal.Trq_EngXSP...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_EngXSP") > 0)
            {
                byte[] TorqCal = GetTorqueCalEngineSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_EngXSP"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_EngXSP"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned x axis for torque calibration map (TrqMastCal.Trq_EngXSP)");
            }

            //step 1a) Alter x axis for TorqueCal.M_NominalMap (=TorqueCal.m_AirXSP) so that the airmass 
            // reaches the maximum desired airmass at the last column (e.g. for a stage 4 1400 mg/c)
            SetProgress("Tuning TrqMastCal.m_AirXSP...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP") > 0)
            {
                byte[] TorqCal = GetTorqueCalSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned x axis for nominal torquemap (TrqMastCal.m_AirXSP)");
            }

            /*** TorqueCal.M_NominalMap ***/
            /*** Data-matrix for nominal Torque. Engine speed and airmass are used as support points. 
            The value in the matrix will be the engine output torque when inlet airmass (- friction airmass) 
            is used together with actual engine speed as pointers ***/
            // formula = replace last column with estimated values of max_torque (= last column * 1.3)
            int max_torque = GetMaxTorque();

            /*if ((int)GetSymbolAddress(m_symbols, "AirCtrlCal.RegMap") > 0)
            {
                byte[] BoostMap = GetBoostRegMap(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "AirCtrlCal.RegMap"), GetSymbolLength(m_symbols, "AirCtrlCal.RegMap"), BoostMap, m_currentfile);
                AddToResumeTable("Tuned boost calibration map (AirCtrlCal.RegMap)");
            }*/


            // now also do BoostCal.SetLoadXSP (!!!) mg/c 8 values
            // run from 500 upto maxairmass
            if ((int)GetSymbolAddress(m_symbols, "AirCtrlCal.SetLoadXSP") > 0)
            {
                byte[] BoostMap = GetBoostMapSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "AirCtrlCal.SetLoadXSP"), GetSymbolLength(m_symbols, "AirCtrlCal.SetLoadXSP"), BoostMap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map x axis (AirCtrlCal.SetLoadXSP)");
            }

            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap") > 0)
            {
                byte[] nominalMap = GetNominalTorqueMap(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"), nominalMap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map (TrqMastCal.Trq_NominalMap)");
            }

            // step 1b) Alter TorqueCal.m_AirTorqMap so that the maximum torquecolumn requests the desired airmass.
            /*** TorqueCal.m_AirTorqMap ***/
            /*** Data-matrix for nominal airmass. Engine speed and torque are used as support points. 
            The value in the matrix + friction airmass (idle airmass) will create the pointed torque at the pointed engine speed. 
            Resolution is   1 mg/c. ***/
            SetProgress("Tuning TrqMastCal.m_AirTorqMap...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap") > 0)
            {
                byte[] AirTorqueCalibration = GetAirTorqueCalibration(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirTorqMap"), AirTorqueCalibration, m_currentfile, true);
                AddToResumeTable("Tuned nominal airmass map (TrqMastCal.m_AirTorqMap)");
            }

            /*progress.SetProgress("Tuning TorqueCal.m_PedYSP...");
            //TODO: PedelMap Y axis with fixed data!
            // step 1c) Alter the pedal request Y axis to meet the airmass request. TorqueCal.m_PedYSP
            
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP") > 0)
            {
                byte[] pedalmapysp = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_PedYSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_PedYSP")) rows /= 2;
                // row > 5 and col > 4
                int ioffset1 = ((rows - 4) * cols * 2);
                int ioffset2 = ((rows - 4) * cols * 2) + 1;
                int pos4value = Convert.ToInt32(pedalmapysp[ioffset1]) * 256 + Convert.ToInt32(pedalmapysp[ioffset2]);

                int airmassperstep = (Convert.ToInt32(maxairmass) - pos4value) / 3;
                int step = 1;
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(pedalmapysp[offset1]) * 256 + Convert.ToInt32(pedalmapysp[offset2]);
                    boostcalvalue = pos4value + (airmassperstep * step);
                    step++;
                    if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    pedalmapysp[offset1] = b1;
                    pedalmapysp[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"), pedalmapysp, m_currentfile);
                AddToResumeTable("Tuned airmass pedalmap y axis (TorqueCal.m_PedYSP)");
            }*/

            SetProgress("Tuning PedalMapCal.m_RequestMap...");


            // step 1d) Alter the pedal request map to meet the desired airmass in the top 2 rows (90-100%). PedalMapCal.m_RequestMap
            /********** PedalMapCal.m_RequestMap ***********/
            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.Trq_RequestMap") > 0)
            {
                byte[] ReferencePedalMap = GetPedalMap(Convert.ToInt32(max_torque * 10), Convert.ToInt32(peakHP), E85);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.Trq_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.Trq_RequestMap"), ReferencePedalMap, m_currentfile, true);
                AddToResumeTable("Tuned airmass request map (PedalMapCal.Trq_RequestMap)");
            }
            SetProgress("Tuning BstKnkCal.MaxAirmass...");
            // step 2) Increase the airmass limit table to allow for more airmass in the desired areas. BstKnkCal.MaxAirmass.
            /********** BstKnkCal.MaxAirmass ***********/
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass") > 0)
            {
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for manual transmission (BstKnkCal.MaxAirmass)");
            }
            SetProgress("Tuning BstKnkCal.MaxAirmassAu...");
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu") > 0)
            {
                /********** BstKnkCal.MaxAirmassAu ***********/
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for automatic transmission (BstKnkCal.MaxAirmassAu)");
            }

            SetProgress("Tuning TrqLimCal.Trq_MaxEngineAutTab1...");

            // step 3 – Increasing engine torque limiters. Up the engine limiters so that the limiter 
            // is higher than the maximum torque in the request maps. 
            // (TorqueCal.M_EngMaxTab, TorqueCal.M_ManGearLim, TorqueCal.M_CabGearLim, TorqueCal.M_5GearLimTab)
            /********** TorqueCal.M_EngMaxAutTab ***********/
            if ((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1") > 0)
            {
                byte[] maxtorqueaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxAutTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorqueaut[offset1]) * 256 + Convert.ToInt32(maxtorqueaut[offset2]);
                    boostcalvalue = 3200;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab1"), maxtorqueaut, m_currentfile, true);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab2"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineAutTab2"), maxtorqueaut, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for automatic transmission (TrqLimCal.Trq_MaxEngineAutTab1 & 2)");
            }

            SetProgress("Tuning TrqLimCal.Trq_MaxEngineManTab1...");
            /********** TorqueCal.M_EngMaxTab ***********/
            if ((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineManTab1") > 0)
            {
                byte[] maxtorquetab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineManTab1"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineManTab1"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqLimCal.Trq_MaxEngineManTab1", out cols, out rows);
                if (isSixteenBitTable("TrqLimCal.Trq_MaxEngineManTab1")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquetab[offset1]) * 256 + Convert.ToInt32(maxtorquetab[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 3200;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineManTab1"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineManTab1"), maxtorquetab, m_currentfile, true);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_MaxEngineManTab2"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_MaxEngineManTab2"), maxtorquetab, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission (TrqLimCal.Trq_MaxEngineManTab1 & 2)");
            }


            SetProgress("Tuning TrqLimCal.Trq_ManGear...");
            if ((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_ManGear") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_ManGear"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_ManGear"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqLimCal.Trq_ManGear", out cols, out rows);
                if (isSixteenBitTable("TrqLimCal.Trq_ManGear")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;

                    boostcalvalue = 3200;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqLimCal.Trq_ManGear"), GetSymbolLength(m_symbols, "TrqLimCal.Trq_ManGear"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission gears (TrqLimCal.Trq_ManGear)");
            }

            // step 4 – Adapt fuel delivery.
            // step 4a) You should make sure the fuel supply is good in all ranges by recalibrating BFuelCal.Map. Altering the maximum allowed airmass will also require more fuel. Check this with a wideband O2 sensor.
            // step 4b) If you change injectors you should change the injector constant InjCorrCal.InjectorConst and the battery voltage correction table InjCorrCal.BattCorrTab accordingly.

            //Step 5 – Increase fuel cur level
            // Increase the fuelcut limit to above the airmass desired. E.g. if you target 1350 mg/c the fuelcut limit should be higher e.g. 1450 or 1500. FCutCal.m_AirInletLimit.

            /********** FCutCal.m_AirInletLimit ***********/

            SetProgress("Tuning FCutCal.m_AirInletLimit...");
            // write xxx to fuelcut limit
            if ((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                byte[] fuelcutmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"));
                if (fuelcutmap.Length == 2)
                {
                    // airmass fuelcut should be ~20% higher than max request
                    double m_AirmassFuelcut = maxairmass * 1.2;
                    int i_AirmassFuelcut = Convert.ToInt32(m_AirmassFuelcut);
                    //if (i_AirmassFuelcut >= 1800) i_AirmassFuelcut = 1800;
                    //TODO: FUELCUT
                    byte b1 = Convert.ToByte(i_AirmassFuelcut / 256);
                    byte b2 = Convert.ToByte(i_AirmassFuelcut - (int)b1 * 256);
                    fuelcutmap[0] = b1;
                    fuelcutmap[1] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned fuelcut limiter (FCutCal.m_AirInletLimit)");
            }

            // all done with changes, update the checksum!
            UpdateChecksum(m_currentfile, true);
            SetProgress("Generating report...");
            AddToResumeTable("Updated checksum.");
            SetProgressIdle();
        }

        private void barButtonItem29_ItemClick(object sender, ItemClickEventArgs e)
        {
            // stage II
        }

        private void barButtonItem30_ItemClick(object sender, ItemClickEventArgs e)
        {
            // stage III
        }

        /*private int GetMaxTorque()
        {
            int retval = 300;
            byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_EngXSP"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_EngXSP"));
            retval = Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 2)) * 256;
            retval += Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 1));
            retval /= 10;
            Console.WriteLine("Max torque from table = " + retval.ToString());
            return retval;
        }*/

        private void TuneToStage(int stage, double maxairmass, double maxtorque, EngineType enginetype)
        {
            SetProgress("Checking current configuration...");
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
            AddToResumeTable("Tuning your binary to stage: " + stage.ToString());

            SetProgress("Creating backup file...");
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin", true);
            AddToResumeTable("Backup file created (" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin" + ")");

            // tune maps
            SetProgress("Tuning TrqMastCal.m_AirXSP...");
            int max_airflow_requested = (int)maxairmass;
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP") > 0)
            {
                byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqMastCal.m_AirXSP", out cols, out rows);
                if (isSixteenBitTable("TrqMastCal.m_AirXSP")) rows /= 2;
                int offset1 = (torquenominalx.Length - 2);
                int offset2 = (torquenominalx.Length - 1);
                int boostcalvalue = Convert.ToInt32(torquenominalx[offset1]) * 256 + Convert.ToInt32(torquenominalx[offset2]);
                boostcalvalue = max_airflow_requested;
                byte b1 = Convert.ToByte(boostcalvalue / 256);
                byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                torquenominalx[offset1] = b1;
                torquenominalx[offset2] = b2;
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirXSP"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirXSP"), torquenominalx, m_currentfile, true);
                AddToResumeTable("Tuned x axis for nominal torquemap (TrqMastCal.m_AirXSP)");
            }
            UpdateChecksum(m_currentfile, true);

            /*** TorqueCal.M_NominalMap ***/
            /*** Data-matrix for nominal Torque. Engine speed and airmass are used as support points. 
            The value in the matrix will be the engine output torque when inlet airmass (- friction airmass) 
            is used together with actual engine speed as pointers ***/
            // formula = replace last column with estimated values of max_torque (= last column * 1.3)
            int max_torque = GetMaxTorque();

            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap") > 0)
            {
                byte[] torquemap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"));

                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqMastCal.Trq_NominalMap", out cols, out rows);
                if (isSixteenBitTable("TrqMastCal.Trq_NominalMap")) rows /= 2;

                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = cols - 1; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(torquemap[offset1]) * 256 + Convert.ToInt32(torquemap[offset2]);
                        boostcalvalue *= 11;
                        boostcalvalue /= 10;
                        if (boostcalvalue > max_torque) boostcalvalue = max_torque;

                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        torquemap[offset1] = b1;
                        torquemap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.Trq_NominalMap"), GetSymbolLength(m_symbols, "TrqMastCal.Trq_NominalMap"), torquemap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map (TrqMastCal.Trq_NominalMap)");
            }
            UpdateChecksum(m_currentfile, true);

            // step 1b) Alter TorqueCal.m_AirTorqMap so that the maximum torquecolumn requests the desired airmass.
            /*** TorqueCal.m_AirTorqMap ***/
            /*** Data-matrix for nominal airmass. Engine speed and torque are used as support points. 
            The value in the matrix + friction airmass (idle airmass) will create the pointed torque at the pointed engine speed. 
            Resolution is   1 mg/c. ***/
            SetProgress("Tuning TrqMastCal.m_AirTorqMap...");
            if ((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap") > 0)
            {
                byte[] torquemap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirTorqMap"));

                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TrqMastCal.m_AirTorqMap", out cols, out rows);
                if (isSixteenBitTable("TrqMastCal.m_AirTorqMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = cols - 1; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(torquemap[offset1]) * 256 + Convert.ToInt32(torquemap[offset2]);
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        //boostcalvalue = max_airflow_requested;
                        if (boostcalvalue > max_airflow_requested) boostcalvalue = max_airflow_requested;
                        //if (boostcalvalue > max_torque) max_torque = boostcalvalue;

                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        torquemap[offset1] = b1;
                        torquemap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TrqMastCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TrqMastCal.m_AirTorqMap"), torquemap, m_currentfile, true);
                AddToResumeTable("Tuned nominal airmass map (TrqMastCal.m_AirTorqMap)");
            }
            // update the checksum
            UpdateChecksum(m_currentfile, true);
            SetProgress("Tuning TorqueCal.m_PedYSP...");

            // step 1c) Alter the pedal request Y axis to meet the airmass request. TorqueCal.m_PedYSP
            /********** TorqueCal.m_PedYSP ***********/
            /*if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP") > 0)
            {
                byte[] pedalmapysp = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_PedYSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_PedYSP")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(pedalmapysp[offset1]) * 256 + Convert.ToInt32(pedalmapysp[offset2]);
                    if (rt == rows - 3)
                    {
                        boostcalvalue *= 11;
                        boostcalvalue /= 10;
                    }
                    else if (rt == rows - 2)
                    {
                        boostcalvalue *= 12;
                        boostcalvalue /= 10;
                    }
                    else
                    {
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                    }
                    if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    pedalmapysp[offset1] = b1;
                    pedalmapysp[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"), pedalmapysp, m_currentfile);
                AddToResumeTable("Tuned airmass pedalmap y axis (TorqueCal.m_PedYSP)");
            }
            
             */

            /*
            progress.SetProgress("Tuning PedalMapCal.m_RequestMap...");


            // step 1d) Alter the pedal request map to meet the desired airmass in the top 2 rows (90-100%). PedalMapCal.m_RequestMap
            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap") > 0)
            {
                byte[] pedalmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.m_RequestMap"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "PedalMapCal.m_RequestMap", out cols, out rows);
                if (isSixteenBitTable("PedalMapCal.m_RequestMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(pedalmap[offset1]) * 256 + Convert.ToInt32(pedalmap[offset2]);
                        if (rt == rows - 3)
                        {
                            boostcalvalue *= 11;
                            boostcalvalue /= 10;
                        }
                        else if (rt == rows - 2)
                        {
                            boostcalvalue *= 12;
                            boostcalvalue /= 10;
                        }
                        else
                        {
                            boostcalvalue *= 13;
                            boostcalvalue /= 10;
                        }
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        pedalmap[offset1] = b1;
                        pedalmap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.m_RequestMap"), pedalmap, m_currentfile);
                AddToResumeTable("Tuned airmass request map (PedalMapCal.m_RequestMap)");
            }
            UpdateChecksum(m_currentfile);
             * */
            SetProgress("Tuning BstKnkCal.MaxAirmass...");

            // step 2) Increase the airmass limit table to allow for more airmass in the desired areas. BstKnkCal.MaxAirmass.
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass") > 0)
            {
                byte[] maxairmasstab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"));
                // up the upper right quadrant > 800 mg/c and > 2000 rpm
                // start off with factor 1.3 for stage I
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BstKnkCal.MaxAirmass", out cols, out rows);
                if (isSixteenBitTable("BstKnkCal.MaxAirmass")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(maxairmasstab[offset1]) * 256 + Convert.ToInt32(maxairmasstab[offset2]);
                        // multiply by 1.3
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        maxairmasstab[offset1] = b1;
                        maxairmasstab[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"), maxairmasstab, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for manual transmission (BstKnkCal.MaxAirmass)");
            }
            UpdateChecksum(m_currentfile, true);
            SetProgress("Tuning BstKnkCal.MaxAirmassAu...");
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu") > 0)
            {
                /********** BstKnkCal.MaxAirmassAu ***********/
                byte[] maxairmassaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"));
                // up the upper right quadrant > 800 mg/c and > 2000 rpm
                // start off with factor 1.3 for stage I
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BstKnkCal.MaxAirmassAu", out cols, out rows);
                if (isSixteenBitTable("BstKnkCal.MaxAirmassAu")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(maxairmassaut[offset1]) * 256 + Convert.ToInt32(maxairmassaut[offset2]);
                        // multiply by 1.3
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        maxairmassaut[offset1] = b1;
                        maxairmassaut[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"), maxairmassaut, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for automatic transmission (BstKnkCal.MaxAirmassAu)");
            }
            UpdateChecksum(m_currentfile, true);
            /*
            progress.SetProgress("Tuning TorqueCal.M_EngMaxAutTab...");

            // step 3 – Increasing engine torque limiters. Up the engine limiters so that the limiter 
            // is higher than the maximum torque in the request maps. 
            // (TorqueCal.M_EngMaxTab, TorqueCal.M_ManGearLim, TorqueCal.M_CabGearLim, TorqueCal.M_5GearLimTab)
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab") > 0)
            {
                byte[] maxtorqueaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxAutTab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxAutTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxAutTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorqueaut[offset1]) * 256 + Convert.ToInt32(maxtorqueaut[offset2]);
                    boostcalvalue = 400;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxAutTab"), maxtorqueaut, m_currentfile);
                AddToResumeTable("Tuned torque limiter for automatic transmission (TorqueCal.M_EngMaxAutTab)");
            }
            
            progress.SetProgress("Tuning TorqueCal.M_EngMaxTab...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab") > 0)
            {
                byte[] maxtorquetab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxTab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquetab[offset1]) * 256 + Convert.ToInt32(maxtorquetab[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 400;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxTab"), maxtorquetab, m_currentfile);
                AddToResumeTable("Tuned torque limiter for manual transmission (TorqueCal.M_EngMaxTab)");
            }
            
            progress.SetProgress("Tuning TorqueCal.M_EngMaxE85Tab...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab") > 0)
            {
                byte[] maxtorquetab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxE85Tab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxE85Tab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxE85Tab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquetab[offset1]) * 256 + Convert.ToInt32(maxtorquetab[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 400;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxE85Tab"), maxtorquetab, m_currentfile);
                AddToResumeTable("Tuned torque limiter for manual transmission on E85 fuel (TorqueCal.M_EngMaxE85Tab)");
            }
            
            progress.SetProgress("Tuning TorqueCal.M_ManGearLim...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_ManGearLim"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_ManGearLim", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_ManGearLim")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 400;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_ManGearLim"), maxtorquemangear, m_currentfile);
                AddToResumeTable("Tuned torque limiter for manual transmission gears (TorqueCal.M_ManGearLim)");
            }
            progress.SetProgress("Tuning TorqueCal.M_CabGearLim...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_CabGearLim") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_CabGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_CabGearLim"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_CabGearLim", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_CabGearLim")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 400;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_CabGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_CabGearLim"), maxtorquemangear, m_currentfile);
                AddToResumeTable("Tuned torque limiter for cabrialet cars (TorqueCal.M_CabGearLim)");
            }
            progress.SetProgress("Tuning TorqueCal.M_5GearLimTab...");

            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(m_symbols, "TorqueCal.M_5GearLimTab"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_5GearLimTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_5GearLimTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;
                    boostcalvalue = 400;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(m_symbols, "TorqueCal.M_5GearLimTab"), maxtorquemangear, m_currentfile);
                AddToResumeTable("Tuned torque limiter for manual transmission in 5th gear (TorqueCal.M_5GearLimTab)");
            }
            */
            // step 4 – Adapt fuel delivery.
            // step 4a) You should make sure the fuel supply is good in all ranges by recalibrating BFuelCal.Map. Altering the maximum allowed airmass will also require more fuel. Check this with a wideband O2 sensor.
            // step 4b) If you change injectors you should change the injector constant InjCorrCal.InjectorConst and the battery voltage correction table InjCorrCal.BattCorrTab accordingly.

            //Step 5 – Increase fuel cur level
            // Increase the fuelcut limit to above the airmass desired. E.g. if you target 1350 mg/c the fuelcut limit should be higher e.g. 1450 or 1500. FCutCal.m_AirInletLimit.

            /********** FCutCal.m_AirInletLimit ***********/

            SetProgress("Tuning FCutCal.m_AirInletLimit...");
            // write 1450 to fuelcut limit
            if ((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                byte[] fuelcutmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"));
                if (fuelcutmap.Length == 2)
                {
                    fuelcutmap[0] = 0x05;
                    fuelcutmap[1] = 0xAA;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned fuelcut limiter (FCutCal.m_AirInletLimit)");
            }
            UpdateChecksum(m_currentfile, true);

            // mark binary as tuned to stage I

            //AddToResumeTable("Updated binary description with tuned stage");
            SetProgress("Generating report...");


            AddToResumeTable("Updated checksum.");

            SetProgressIdle();

            // refresh open viewers

        }

        private void browseAxisInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string symbolname = string.Empty;
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper dr = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    symbolname = dr.Varname;
                }
            }
            DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
            AxisBrowser tabdet = new AxisBrowser();
            tabdet.onStartSymbolViewer += new AxisBrowser.StartSymbolViewer(tabdet_onStartSymbolViewer);
            tabdet.Dock = DockStyle.Fill;
            dockPanel.Controls.Add(tabdet);
            tabdet.ShowSymbolCollection(m_symbols);
            tabdet.SetCurrentSymbol(symbolname);
            dockPanel.Text = "Axis browser: " + Path.GetFileName(m_currentfile);
            bool isDocked = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Axis browser: ") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                {
                    dockPanel.DockAsTab(pnl, 0);
                    isDocked = true;
                    break;
                }
            }
            if (!isDocked)
            {
                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);
                dockPanel.Width = 700;
            }
        }

        void tabdet_onStartSymbolViewer(object sender, AxisBrowser.SymbolViewerRequestedEventArgs e)
        {
            StartTableViewer(e.Mapname);
        }

        private void barButtonItem31_ItemClick(object sender, ItemClickEventArgs e)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
            AxisBrowser tabdet = new AxisBrowser();
            tabdet.onStartSymbolViewer += new AxisBrowser.StartSymbolViewer(tabdet_onStartSymbolViewer);
            tabdet.Dock = DockStyle.Fill;
            dockPanel.Controls.Add(tabdet);
            tabdet.ShowSymbolCollection(m_symbols);
            dockPanel.Text = "Axis browser: " + Path.GetFileName(m_currentfile);
            bool isDocked = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Axis browser: ") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                {
                    dockPanel.DockAsTab(pnl, 0);
                    isDocked = true;
                    break;
                }
            }
            if (!isDocked)
            {
                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);
                dockPanel.Width = 700;
            }
        }
        private bool AssemblerViewerActive(bool ShowIfActive, string filename)
        {
            bool retval = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Assembler: " + Path.GetFileName(filename)))
                {
                    retval = true;
                    if (ShowIfActive)
                    {
                        pnl.Show();
                        dockManager1.ActivePanel = pnl;
                    }
                }
                else
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            if (tpnl.Text.StartsWith("Assembler: " + Path.GetFileName(filename)))
                            {
                                retval = true;
                                if (ShowIfActive)
                                {
                                    tpnl.Show();
                                    dockManager1.ActivePanel = tpnl;
                                }
                            }
                        }
                    }
                }
            }
            return retval;

        }

        private void StartAssemblerViewer(string filename)
        {
            if (m_currentfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Text = "Assembler: " + Path.GetFileName(filename);
                    //HexViewer hv = new HexViewer();
                    AsmViewer av = new AsmViewer();
                    av.Dock = DockStyle.Fill;
                    dockPanel.Width = 700;
                    dockPanel.Controls.Add(av);
                    SetProgress("Loading assembler file ...");
                    av.LoadDataFromFile(filename, m_symbols);
                    SetProgress("Finding starting address in file");
                    av.FindStartAddress();
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private long GetStartVectorAddress(string filename, int number)
        {
            // startvector = second 4 byte word in the file
            long retval = 0;
            Int32 start_address = number * 4;
            retval = Convert.ToInt64(readdatafromfile(m_currentfile, start_address, 1)[0]) * 256 * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentfile, start_address + 1, 1)[0]) * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentfile, start_address + 2, 1)[0]) * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentfile, start_address + 3, 1)[0]);
            return retval;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string outputfile = Path.GetDirectoryName(m_currentfile);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(m_currentfile) + ".asm");
            if (!AssemblerViewerActive(true, outputfile))
            {
                SetProgress("Start disassembler");
                if (!File.Exists(outputfile))
                {
                    SetProgress("Disassembler running...");
                    Disassembler dis = new Disassembler();
                    long start_address = 0;

                    // Test
                    start_address = GetStartVectorAddress(m_currentfile, 1);

                    dis.DisassembleFile(true, m_currentfile_size, m_currentfile, outputfile, start_address, m_currentfile_size, m_symbols);
                    SetProgress("Disassembler done...");
                }
                SetProgress("Loading assembler file");
                StartAssemblerViewer(outputfile);
                SetProgressIdle();
            }
        }

        private void barButtonItem32_ItemClick(object sender, ItemClickEventArgs e)
        {
            // save as a byte flipped file
            if (m_currentfile != "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Binary files|*.bin";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine("Flipping file");
                    string filenamenew = sfd.FileName;
                    FileStream fromfile = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
                    FileStream tofile = new FileStream(filenamenew, FileMode.CreateNew);
                    using (BinaryReader brori = new BinaryReader(fromfile))
                    {
                        using (BinaryWriter brnew = new BinaryWriter(tofile))
                        {
                            for (int t = 0; t < 0x100000; t += 2)
                            {
                                byte bfirst = brori.ReadByte();
                                byte bsecond = brori.ReadByte();
                                brnew.Write(bsecond);
                                brnew.Write(bfirst);
                            }
                        }
                    }
                    fromfile.Close();
                    tofile.Close();
                }
            }


        }

        private string GetFileDescriptionFromFile(string file)
        {
            string retval = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    string name = sr.ReadLine();
                    name = name.Trim();
                    name = name.Replace("<", "");
                    name = name.Replace(">", "");
                    //name = name.Replace("x0020", " ");
                    name = name.Replace("_x0020_", " ");
                    for (int i = 0; i <= 9; i++)
                    {
                        name = name.Replace("_x003" + i.ToString() + "_", i.ToString());
                    }
                    retval = name;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private bool TryToLoadAdditionalSymbols(string filename, bool ImportFromRepository, SymbolCollection coll2load)
        {
            bool retval = false;
            SymbolTranslator st = new SymbolTranslator();
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(filename));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            if (ImportFromRepository)
            {
                T8Header fh = new T8Header();
                fh.init(filename);

                string checkstring = fh.PartNumber + fh.SoftwareVersion;
                string xmlfilename = System.Windows.Forms.Application.StartupPath + "\\repository\\" + Path.GetFileNameWithoutExtension(filename) + File.GetCreationTime(filename).ToString("yyyyMMddHHmmss") + checkstring + ".xml";
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\repository"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\repository");
                }
                if (File.Exists(xmlfilename))
                {
                    dt.ReadXml(xmlfilename);
                    retval = true;
                }
            }
            else
            {
                string binname = GetFileDescriptionFromFile(filename);
                if (binname != string.Empty)
                {
                    dt = new System.Data.DataTable(binname);
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    if (File.Exists(filename))
                    {
                        dt.ReadXml(filename);
                        retval = true;
                    }
                }
            }
            foreach (SymbolHelper sh in coll2load)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                        {
                            //if (sh.Symbol_number == Convert.ToInt32(dr["SYMBOLNUMBER"]))
                            {
                                if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                                {
                                    sh.Userdescription = dr["DESCRIPTION"].ToString();
                                    string helptext = string.Empty;
                                    XDFCategories cat = XDFCategories.Undocumented;
                                    XDFSubCategory sub = XDFSubCategory.Undocumented;
                                    sh.Description = st.TranslateSymbolToHelpText(sh.Userdescription, out helptext, out cat, out sub);

                                    //if(sh.Category == 
                                    if (sh.Category == "Undocumented" || sh.Category == "")
                                    {
                                        if (sh.Userdescription.Contains("."))
                                        {
                                            try
                                            {
                                                sh.Category = sh.Userdescription.Substring(0, sh.Userdescription.IndexOf("."));
                                                //Console.WriteLine("Set cat to " + sh.Category + " for " + sh.Userdescription);
                                            }
                                            catch (Exception cE)
                                            {
                                                Console.WriteLine("Failed to assign category to symbol: " + sh.Userdescription + " err: " + cE.Message);
                                            }
                                        }

                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
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

        private bool CheckAllTablesAvailable()
        {
            bool retval = true;
            if (m_currentfile != "")
            {
                if (File.Exists(m_currentfile))
                {
                    if (!SymbolExists("PedalMapCal.Trq_RequestMap")) retval = false;
                    if (!SymbolExists("TrqMastCal.m_AirTorqMap")) retval = false;
                    if (!SymbolExists("TrqMastCal.Trq_NominalMap")) retval = false;
                    if (!SymbolExists("BstKnkCal.MaxAirmass")) retval = false;
                    if (!SymbolExists("FCutCal.m_AirInletLimit")) retval = false;
                    if (!SymbolExists("TrqLimCal.Trq_MaxEngineManTab1") && !SymbolExists("TrqLimCal.Trq_MaxEngineTab1")) retval = false;
                    if (!SymbolExists("TrqLimCal.Trq_MaxEngineAutTab1") && !SymbolExists("TrqLimCal.Trq_MaxEngineTab1")) retval = false;
                    // if (!SymbolExists("TrqLimCal.Trq_MaxEngineManTab2")) retval = false;
                    //if (!SymbolExists("TrqLimCal.Trq_MaxEngineAutTab2")) retval = false;
                    if (!SymbolExists("TrqLimCal.Trq_ManGear")) retval = false;
                }
                else retval = false;
            }
            else retval = false;
            return retval;
        }

        private void btnAirmassResult_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Windows.Forms.Application.DoEvents();
            if (CheckAllTablesAvailable())
            {
                // build a table that shows the maximum allowed airmass depending on the current limiters
                // table show be the same size as the pedalrequest map
                frmAirmassResult airmassresult = new frmAirmassResult();
                airmassresult.onStartTableViewer += new frmAirmassResult.StartTableViewer(airmassresult_onStartTableViewer);
                airmassresult.Currentfile = m_currentfile;
                airmassresult.Symbols = m_symbols;
                airmassresult.Calculate();
                airmassresult.Show(); // not dialog?
            }
        }

        void airmassresult_onStartTableViewer(object sender, frmAirmassResult.StartTableViewerEventArgs e)
        {
            // start the table viewer
            StartTableViewer(e.SymbolName);
        }

        private void barButtonItem33_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask which file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] file = readdatafromfile(ofd.FileName, 0, 0x100000);
                for (int i = 0; i < file.Length; i++)
                {
                    file[i] += 0xD6;
                    file[i] ^= 0x21;
                }

                string outputfilename = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + "-decodedD621.bin");
                FileStream fs = new FileStream(outputfilename, FileMode.OpenOrCreate);
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(file);
                }
                fs.Close();
            }
        }

        private void barButtonItem34_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask which file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] file = readdatafromfile(ofd.FileName, 0, 0x100000);
                for (int i = 0; i < file.Length; i++)
                {
                    file[i] += 0x53;
                    file[i] ^= 0xA4;
                }
                string outputfilename = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + "-decoded53A4.bin");
                FileStream fs = new FileStream(outputfilename, FileMode.OpenOrCreate);
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(file);
                }
                fs.Close();
            }
        }

        private void barButtonItem35_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask which file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] file = readdatafromfile(ofd.FileName, 0, 0x100000);
                for (int i = 0; i < file.Length; i++)
                {
                    file[i] ^= 0x21;
                    file[i] -= 0xD6;
                }

                string outputfilename = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + "-ecodedD621.bin");
                FileStream fs = new FileStream(outputfilename, FileMode.OpenOrCreate);
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(file);
                }
                fs.Close();
            }
        }

        private void barButtonItem36_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask which file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Binary files|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] file = readdatafromfile(ofd.FileName, 0, 0x100000);
                for (int i = 0; i < file.Length; i++)
                {
                    file[i] ^= 0xA4;
                    file[i] -= 0x53;
                }

                string outputfilename = Path.Combine(Path.GetDirectoryName(ofd.FileName), Path.GetFileNameWithoutExtension(ofd.FileName) + "-ecoded53A4.bin");
                FileStream fs = new FileStream(outputfilename, FileMode.OpenOrCreate);
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(file);
                }
                fs.Close();
            }
        }

        /*
        private void AddToSourceFile(string symbolname, string description, string resolution, string interval)
        {
            if (description == "") return;
            if (description.StartsWith("No info text")) return;
            description = description.Replace("\"", "\\\"");
            if (resolution != "") description += " Resolution is " + resolution + ".";
            if (interval != "") description += " Interval is " + interval + ".";
            using (StreamWriter sw = new StreamWriter(@"C:\T8Source2Copy.cs", true, System.Text.Encoding.Default))
            {
                sw.WriteLine("          case \"" + symbolname + "\":");
                sw.WriteLine("              description = helptext = \"" + description + "\";");
                sw.WriteLine("              category = XDFCategories.Undocumented;");
                sw.WriteLine("              subcategory = XDFSubCategory.Undocumented;");
                sw.WriteLine("              break;");
            }
        }*/

        private string GetFullDescription(string[] lines, int fromindex)
        {
            bool endFound = false;
            string retval = string.Empty;
            //retval = lines[fromindex];
            int cnt = 0;
            while (!endFound)
            {
                string line = lines[fromindex + cnt];
                if (line.Contains("\"]"))
                {
                    // laatste regel
                    line = line.Replace("\x0d", " ");
                    line = line.Replace("\x0a", " ");
                    line = line.Replace("\"", " ");
                    line = line.Replace("'", " ");
                    line = line.Replace("Description:[", "");
                    line = line.Replace("DESCRIPTION:", "");
                    while (line.Contains("  ")) line = line.Replace("  ", " ");
                    while (line.Contains("**")) line = line.Replace("**", "*");
                    line.Trim();
                    endFound = true;
                    retval += line;
                }
                else if (line.StartsWith("*") && !line.StartsWith("**"))
                {
                    endFound = true;
                }
                else
                {
                    line = line.Replace("\x0d", " ");
                    line = line.Replace("\x0a", " ");
                    line = line.Replace("\"", " ");
                    line = line.Replace("'", " ");
                    line = line.Replace("Description:[", "");
                    line = line.Replace("DESCRIPTION:", "");
                    while (line.Contains("  ")) line = line.Replace("  ", " ");
                    while (line.Contains("**")) line = line.Replace("**", "*");
                    line.Trim();
                    retval += line;
                }
                if (cnt++ > 15)
                {
                    endFound = true;
                    Console.WriteLine("More than 10 lines!");
                }
            }
            return retval;
        }
        

        private void SaveAdditionalSymbols()
        {
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(m_currentfile));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");

            T8Header fh = new T8Header();
            fh.init(m_currentfile);
            string checkstring = fh.PartNumber + fh.SoftwareVersion;
            string xmlfilename = System.Windows.Forms.Application.StartupPath + "\\repository\\" + Path.GetFileNameWithoutExtension(m_currentfile) + File.GetCreationTime(m_currentfile).ToString("yyyyMMddHHmmss") + checkstring + ".xml";
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\repository"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\repository");
            }
            if (File.Exists(xmlfilename))
            {
                File.Delete(xmlfilename);
            }
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Userdescription != "")
                {
                    dt.Rows.Add(sh.Varname, sh.Symbol_number, sh.Flash_start_address, sh.Userdescription);
                }
            }
            dt.WriteXml(xmlfilename);
        }

        private void btnImportXML_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportXMLDescriptorFile();
        }

       

        private void gridViewSymbols_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Name == gcSymbolsUserDescription.Name)
            {
                // save a new repository item
                SaveAdditionalSymbols();

            }
        }

        public enum VectorType : int
        {
            Reset_initial_stack_pointer,
            Reset_initial_program_counter,
            Bus_error,
            Address_error,
            Illegal_instruction,
            Zero_division,
            CHK_CHK2_instructions,
            TRAPcc_TRAPV_instructions,
            Privilege_violation,
            Trace,
            Line_1010_emulator,
            Line_1111_emulator,
            Hardware_breakpoint,
            Coprocessor_protocol_violation,
            Format_error_and_uninitialized_interrupt_1,
            Format_error_and_uninitialized_interrupt_2,
            Unassigned_reserved_1,
            Unassigned_reserved_2,
            Unassigned_reserved_3,
            Unassigned_reserved_4,
            Unassigned_reserved_5,
            Unassigned_reserved_6,
            Unassigned_reserved_7,
            Unassigned_reserved_8,
            Spurious_interrupt,
            Level_1_interrupt_autovector,
            Level_2_interrupt_autovector,
            Level_3_interrupt_autovector,
            Level_4_interrupt_autovector,
            Level_5_interrupt_autovector,
            Level_6_interrupt_autovector,
            Level_7_interrupt_autovector,
            Trap_instruction_vectors_0,
            Trap_instruction_vectors_1,
            Trap_instruction_vectors_2,
            Trap_instruction_vectors_3,
            Trap_instruction_vectors_4,
            Trap_instruction_vectors_5,
            Trap_instruction_vectors_6,
            Trap_instruction_vectors_7,
            Trap_instruction_vectors_8,
            Trap_instruction_vectors_9,
            Trap_instruction_vectors_10,
            Trap_instruction_vectors_11,
            Trap_instruction_vectors_12,
            Trap_instruction_vectors_13,
            Trap_instruction_vectors_14,
            Trap_instruction_vectors_15,
            Reserved_coprocessor_0,
            Reserved_coprocessor_1,
            Reserved_coprocessor_2,
            Reserved_coprocessor_3,
            Reserved_coprocessor_4,
            Reserved_coprocessor_5,
            Reserved_coprocessor_6,
            Reserved_coprocessor_7,
            Reserved_coprocessor_8,
            Reserved_coprocessor_9,
            Reserved_coprocessor_10,
            Unassigned_reserved_9,
            Unassigned_reserved_10,
            Unassigned_reserved_11,
            Unassigned_reserved_12,
            Unassigned_reserved_13
        }

        private void btnShowVectors_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmVectorlist vectors = new frmVectorlist();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Vector");
            dt.Columns.Add("Address", System.Type.GetType("System.Int64"));
            string description = string.Empty;
            for (int i = 0; i < 256; i++)
            {
                long address = GetStartVectorAddress(m_currentfile, i);
                if (i <= 63)
                {
                    // get description
                    description = ((VectorType)i).ToString().Replace("_", " ");
                }
                else
                {
                    int number = i - 64;
                    description = "User defined vector " + number.ToString();
                }
                dt.Rows.Add(description, address);
            }
            vectors.SetDataTable(dt);
            vectors.ShowDialog();

        }

        private void DumpThisData(byte[] data, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(data);
            }
            fs.Close();
        }

        //
        //        file_data[t] += 0x53;
        //        file_data[t] ^= 0xA4;
        //
        private void btnTestMFSBlocks_ItemClick(object sender, ItemClickEventArgs e)
        {
            // search file for "MFS*" indicator
            if (m_currentfile != string.Empty)
            {
                byte[] entirefile = File.ReadAllBytes(m_currentfile);
                int fileCount = 0;
                for (int i = 0; i < entirefile.Length - 4; i++)
                {
                    if (entirefile[i] == 'M' && entirefile[i + 1] == 'F' && entirefile[i + 2] == 'S' && entirefile[i + 3] == '*')
                    {
                        // get 0x100 bytes to start with
                        Console.WriteLine("Block at " + i.ToString("X8"));
                        byte[] btodump = new byte[0x1000];
                        for (int j = 0; j < 0x1000; j++)
                        {
                            btodump[j] = entirefile[i + j];
                            btodump[j] += 0x53;
                            btodump[j] ^= 0xA4;
                        }
                        //DumpThisData(btodump, @"C:\T8Decode\MFSBlock-A-" + fileCount.ToString() + "-" + i.ToString("X8") + ".mfs");
                        fileCount++;
                        for (int j = 0; j < 0x1000; j++)
                        {
                            btodump[j] = entirefile[i + j];
                            btodump[j] ^= 0xA4;
                            btodump[j] -= 0x53;
                        }
                        //DumpThisData(btodump, @"C:\T8Decode\MFSBlock-B-" + fileCount.ToString() + "-" + i.ToString("X8") + ".mfs");
                        fileCount++;
                        for (int j = 0; j < 0x1000; j++)
                        {
                            btodump[j] = entirefile[i + j];
                            btodump[j] += 0xD6;
                            btodump[j] ^= 0x21;
                        }
                        //DumpThisData(btodump, @"C:\T8Decode\MFSBlock-C-" + fileCount.ToString() + "-" + i.ToString("X8") + ".mfs");
                        fileCount++;
                        for (int j = 0; j < 0x1000; j++)
                        {
                            btodump[j] = entirefile[i + j];
                            btodump[j] ^= 0x21;
                            btodump[j] -= 0xD6;
                        }
                        //DumpThisData(btodump, @"C:\T8Decode\MFSBlock-D-" + fileCount.ToString() + "-" + i.ToString("X8") + ".mfs");
                        fileCount++;
                    }
                }
                // dump piarea but larger
                byte[] bpitodump = new byte[0x1000];
                int jx = 0;
                for (int i = 0xBAF06; i < 0xBBF06; i++)
                {
                    bpitodump[jx] = entirefile[i];
                    bpitodump[jx] += 0xD6;
                    bpitodump[jx] ^= 0x21;
                    jx++;
                }
                //DumpThisData(bpitodump, @"C:\T8Decode\piarea.mfs");

            }
        }

        private void btnConvertOriFiles_ItemClick(object sender, ItemClickEventArgs e)
        {
            // get all files in subfolder Binaries\Ori and convert the filenames
            string[] files = Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\Binaries\\Ori", "*.bin");
            foreach (string file in files)
            {
                Console.WriteLine("Handling: " + Path.GetFileName(file));
                m_currentfile = file;
                TryToOpenFile(m_currentfile, out m_symbols, m_currentfile_size);
                // now get the details
                if (m_currentfile != "")
                {
                    T8Header t8header = new T8Header();

                    t8header.init(m_currentfile);
                    frmAirmassResult res = new frmAirmassResult();
                    res.Currentfile = m_currentfile;
                    res.Symbols = m_symbols;
                    res.Calculate();
                    int peak_airmass = res.PeakAirmass;
                    string newFilename = t8header.PartNumber.Trim() + "_" + t8header.SoftwareVersion.Trim() /*+ "_" + t8header.HardwareID.Trim() + "_" + t8header.DeviceType.Trim() + "_" + peak_airmass.ToString() + "mgc.bin"*/ + ".BIN";

                    if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\Binaries\\" + newFilename))
                    {
                        File.Copy(file, System.Windows.Forms.Application.StartupPath + "\\Binaries\\" + newFilename);

                        // test
                        VINDecoder dec = new VINDecoder();
                        VINCarInfo info = dec.DecodeVINNumber(t8header.ChassisID);
                        AddToPartnumberCollectionFile(t8header.PartNumber.Trim(), info.Makeyear, t8header.SoftwareVersion.Trim());
                        AddToPartnumberConverterFile(t8header.PartNumber.Trim(), info.EngineType, info.CarModel, t8header.SoftwareVersion.Trim());
                        // test

                    }
                    else
                    {
                        Console.WriteLine("File already existed: " + newFilename);
                    }
                    /*frminfo.ProgrammerName = t8header.ProgrammerName;
                    frminfo.ProgrammingDevice = t8header.ProgrammerDevice;
                    frminfo.PartNumber = t8header.PartNumber;
                    frminfo.ReleaseDate = t8header.ReleaseDate;
                    frminfo.SoftwareID = t8header.SoftwareVersion;
                    frminfo.ChassisID = t8header.ChassisID;
                    frminfo.EngineType = t8header.CarDescription;
                    frminfo.ImmoID = t8header.ImmobilizerID;
                    frminfo.HardwareID = t8header.HardwareID;
                    frminfo.HardwareType = t8header.DeviceType;
                    frminfo.ECUDescription = t8header.EcuDescription;
                    frminfo.InterfaceDevice = t8header.InterfaceDevice;
                    frminfo.NumberOfFlashBlocks = t8header.NumberOfFlashBlocks.ToString();*/
                }
            }
            Console.WriteLine("All done");
        }

        private void AddToPartnumberConverterFile(string partnumber, VINEngineType vINEngineType, VINCarModel vINCarModel, string swversion)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\T8PartnumberConverter.cs", true, System.Text.Encoding.Default))
            {
                sw.WriteLine("\t\tcase \"" + partnumber + "_" + swversion + "\":");
                sw.WriteLine("\t\t\treturnvalue.Enginetype = EngineType." + vINEngineType.ToString() + ";");
                sw.WriteLine("\t\t\treturnvalue.Carmodel = CarModel.Saab93;");
                sw.WriteLine("\t\t\treturnvalue.Softwareversion = \"" + swversion.Trim() + "\";");
                sw.WriteLine("\t\t\tbreak;");
                /*
                case "5380480":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                 * */
            }
        }

        private void AddToPartnumberCollectionFile(string partnumber, int my, string swversion)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\T8PartnumberCollection.cs", true, System.Text.Encoding.Default))
            {
                sw.WriteLine("\t\tAddPartNumber(\"" + partnumber + "_" + swversion + "\", \"" + my.ToString() + "\");");
            }
        }

        private void btnVINDecoder_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmDecodeVIN decode = new frmDecodeVIN();
            if (m_currentfile != string.Empty)
            {
                T8Header header = new T8Header();
                header.init(m_currentfile);
                decode.SetVinNumber(header.ChassisID);
            }
            decode.ShowDialog();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                exportAsTuningPackageToolStripMenuItem.Enabled = true;
            }
            else
            {
                exportAsTuningPackageToolStripMenuItem.Enabled = false;
            }
            if (m_currentsramfile != "")
            {
                if (File.Exists(m_currentsramfile))
                {
                    readFromSRAMFileToolStripMenuItem.Enabled = true;
                }
                else
                {
                    readFromSRAMFileToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                readFromSRAMFileToolStripMenuItem.Enabled = false;
            }

        }

        private void exportAsTuningPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // export selected maps as tuning package (name the file t8p)
            // get selected rows
            int[] selectedrows = gridViewSymbols.GetSelectedRows();
            SymbolCollection scToExport = new SymbolCollection();
            if (selectedrows.Length > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Trionic 8 packages|*.t8p";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                    if (grouplevel >= gridViewSymbols.GroupCount)
                    {
                        int[] selrows = gridViewSymbols.GetSelectedRows();
                        if (selrows.Length > 0)
                        {
                            for (int i = 0; i < selrows.Length; i++)
                            {
                                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(i));
                                // make a copy?
                                scToExport.Add(sh);
                            }
                            PackageExporter pe = new PackageExporter();
                            pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
                        }

                    }
                }

            }
        }

        private void ImportTuningPackage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 8 packages|*.t8p";
            ofd.Multiselect = false;
            char[] sep = new char[1];
            sep.SetValue(',', 0);

            SymbolCollection scToImport = new SymbolCollection();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Map");
            dt.Columns.Add("Result");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //TODO: create a list of maps to import .. maybe?
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line = string.Empty;
                    SymbolHelper sh_Import = new SymbolHelper();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("symbol="))
                        {
                            //
                            sh_Import = new SymbolHelper();
                            sh_Import.Varname = line.Replace("symbol=", "");
                        }
                        else if (line.StartsWith("length="))
                        {
                            sh_Import.Length = Convert.ToInt32(line.Replace("length=", ""));
                        }
                        else if (line.StartsWith("data="))
                        {
                            //
                            try
                            {
                                string dataBytes = line.Replace("data=", "");
                                // split using ','
                                string[] bytesInStrings = dataBytes.Split(sep);
                                byte[] dataToInsert = new byte[sh_Import.Length];
                                for (int t = 0; t < sh_Import.Length; t++)
                                {
                                    byte b = Convert.ToByte(bytesInStrings[t], 16);
                                    dataToInsert.SetValue(b, t);
                                }
                                int addressInFile = (int)GetSymbolAddress(m_symbols, sh_Import.Varname);
                                if (addressInFile > 0)
                                {
                                    savedatatobinary(addressInFile, sh_Import.Length, dataToInsert, m_currentfile, true);
                                    // add successful
                                    dt.Rows.Add(sh_Import.Varname, "Success");
                                }
                                else
                                {
                                    // add failure
                                    dt.Rows.Add(sh_Import.Varname, "Fail");
                                }
                            }
                            catch (Exception E)
                            {
                                // add failure
                                dt.Rows.Add(sh_Import.Varname, "Fail");
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }
                UpdateChecksum(m_currentfile, true);
                frmImportResults res = new frmImportResults();
                res.SetDataTable(dt);
                res.ShowDialog();

            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SymbolCollection scToExport = new SymbolCollection();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Trionic 8 packages|*.t8p";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                // add all relevant symbols to the export collection
                //SymbolHelper sh = new SymbolHelper();
                // must contain Varname, FlashAddress, Length, Userdescription

                AddToSymbolCollection(scToExport, "AirCtrlCal.PRatioMaxTab");
                AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmass");
                AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmassAu");
                AddToSymbolCollection(scToExport, "BFuelCal.TempEnrichFacMap");
                AddToSymbolCollection(scToExport, "BFuelCal.E85TempEnrichFacMap");
                AddToSymbolCollection(scToExport, "KnkFuelCal.EnrichmentMap");
                AddToSymbolCollection(scToExport, "KnkFuelCal.fi_OffsetEnrichEnable");
                AddToSymbolCollection(scToExport, "KnkFuelCal.fi_MaxOffsetMap");
                AddToSymbolCollection(scToExport, "IgnAbsCal.fi_highOctanMAP");
                AddToSymbolCollection(scToExport, "IgnAbsCal.fi_lowOctanMAP");
                AddToSymbolCollection(scToExport, "IgnAbsCal.fi_NormalMAP");
                AddToSymbolCollection(scToExport, "IgnAbsCal.fi_StartMAP");
                AddToSymbolCollection(scToExport, "DNCompCal.SlowDriveRelTAB");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_ManGear");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_MaxEngineManTab1");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_MaxEngineAutTab1");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_MaxEngineManTab2");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_MaxEngineAutTab2");
                AddToSymbolCollection(scToExport, "TrqLimCal.Trq_OverBoostTab");
                AddToSymbolCollection(scToExport, "MaxEngSpdCal.n_EngMin");
                AddToSymbolCollection(scToExport, "TrqMastCal.Trq_NominalMap");
                AddToSymbolCollection(scToExport, "TrqMastCal.m_AirTorqMap");
                AddToSymbolCollection(scToExport, "TMCCal.Trq_MaxEngineTab");
                AddToSymbolCollection(scToExport, "TMCCal.Trq_MaxEngineLowTab");
                PackageExporter pe = new PackageExporter();
                pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
            }
        }
        private void AddToSymbolCollection(SymbolCollection scToExport, string symbolName)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolName || sh.Userdescription == symbolName)
                {
                    SymbolHelper shNew = new SymbolHelper();
                    shNew.Start_address = sh.Start_address;
                    shNew.Symbol_number = sh.Symbol_number;
                    shNew.Symbol_number_ECU = sh.Symbol_number_ECU;
                    shNew.Internal_address = sh.Internal_address;
                    shNew.Varname = symbolName;
                    shNew.Flash_start_address = sh.Flash_start_address;
                    shNew.Length = sh.Length;
                    shNew.Userdescription = symbolName;
                    scToExport.Add(shNew);
                    break;
                }
            }
        }

        private void EditTuningPackage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 8 packages|*.t8p";
            ofd.Multiselect = false;
            char[] sep = new char[1];
            sep.SetValue(',', 0);

            SymbolCollection scToImport = new SymbolCollection();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Map");
            dt.Columns.Add("Length");
            dt.Columns.Add("Data");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //TODO: create a list of maps to import .. maybe?
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line = string.Empty;
                    SymbolHelper sh_Import = new SymbolHelper();
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("symbol="))
                        {
                            //
                            sh_Import = new SymbolHelper();
                            sh_Import.Varname = line.Replace("symbol=", "");
                        }
                        else if (line.StartsWith("length="))
                        {
                            sh_Import.Length = Convert.ToInt32(line.Replace("length=", ""));
                        }
                        else if (line.StartsWith("data="))
                        {
                            //
                            try
                            {
                                string dataBytes = line.Replace("data=", "");
                                // split using ','
                                string[] bytesInStrings = dataBytes.Split(sep);
                                byte[] dataToInsert = new byte[sh_Import.Length];
                                for (int t = 0; t < sh_Import.Length; t++)
                                {
                                    byte b = Convert.ToByte(bytesInStrings[t], 16);
                                    dataToInsert.SetValue(b, t);
                                }
                                int addressInFile = (int)GetSymbolAddress(m_symbols, sh_Import.Varname);
                                if (addressInFile > 0)
                                {
                                    //savedatatobinary(addressInFile, sh_Import.Length, dataToInsert, m_currentfile, true);
                                    // add successful
                                    dt.Rows.Add(sh_Import.Varname, sh_Import.Length.ToString(), dataBytes);
                                }
                                else
                                {
                                    // add failure
                                    dt.Rows.Add(sh_Import.Varname, sh_Import.Length.ToString(), dataBytes);
                                }
                            }
                            catch (Exception E)
                            {
                                // add failure
                                dt.Rows.Add(sh_Import.Varname, sh_Import.Length.ToString(), "");
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }
                frmEditTuningPackage edit = new frmEditTuningPackage();
                edit.FormClosed += new FormClosedEventHandler(edit_FormClosed);
                edit.SetDataTable(dt);
                edit.Show();

            }
        }
        void edit_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is frmEditTuningPackage)
            {
                frmEditTuningPackage edit = (frmEditTuningPackage)sender;
                if (edit.WriteData)
                {
                    // save the package again with altered settings probably.
                    //Console.WriteLine("We should write the tuning package here!");
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Trionic 8 packages|*.t8p";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.Data.DataTable dt = edit.GetDataTable();
                        SymbolCollection scToExport = new SymbolCollection();
                        foreach (DataRow dr in dt.Rows)
                        {
                            SymbolHelper sh = new SymbolHelper();
                            sh.Varname = dr["Map"].ToString();
                            sh.Flash_start_address = GetSymbolAddress(m_symbols, sh.Varname);
                            sh.Userdescription = GetUserDescription(m_symbols, sh.Varname);
                            sh.Length = GetSymbolLength(m_symbols, sh.Varname);
                            scToExport.Add(sh);

                        }
                        PackageExporter pe = new PackageExporter();
                        pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
                    }
                }
            }
        }
        private string GetUserDescription(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return sh.Userdescription;
                }
            }
            return symbolname;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            EditTuningPackage();
        }

        private void btnImportTuningPackage_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportTuningPackage();
        }

        private void btnEditTuningPackage_ItemClick(object sender, ItemClickEventArgs e)
        {
            EditTuningPackage();
        }

        private string MakeDirName(string dirname)
        {
            string retval = dirname;
            retval = retval.Replace(@"\", "");
            retval = retval.Replace(@"/", "");
            retval = retval.Replace(@":", "");
            retval = retval.Replace(@"*", "");
            retval = retval.Replace(@"?", "");
            retval = retval.Replace(@">", "");
            retval = retval.Replace(@"<", "");
            retval = retval.Replace(@"|", "");
            return retval;
        }

        private TrionicTransactionLog m_ProjectTransactionLog;

        private void OpenProject(string projectname)
        {
            //TODO: Are there pending changes in the optionally currently opened binary file / project?

            //TODO: open a selected project
            //frmInfoBox info = new frmInfoBox("Opening project: " + projectname);
            if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + projectname))
            {
                m_appSettings.LastOpenedType = 1;
                m_CurrentWorkingProject = projectname;
                m_ProjectLog.OpenProjectLog(m_appSettings.ProjectFolder + "\\" + projectname);
                //Load the binary file that comes with this project
                LoadBinaryForProject(projectname);
                if (m_currentfile != string.Empty)
                {
                    // transaction log <GS-15032010>
                    m_ProjectTransactionLog = new TrionicTransactionLog();
                    if (m_ProjectTransactionLog.OpenTransActionLog(m_appSettings.ProjectFolder, projectname))
                    {
                        m_ProjectTransactionLog.ReadTransactionFile();
                        //m_trionicFile.SetTransactionLog(m_ProjectTransactionLog);
                        if (m_ProjectTransactionLog.TransCollection.Count > 2000)
                        {
                            frmProjectTransactionPurge frmPurge = new frmProjectTransactionPurge();
                            frmPurge.SetNumberOfTransactions(m_ProjectTransactionLog.TransCollection.Count);
                            if (frmPurge.ShowDialog() == DialogResult.OK)
                            {
                                m_ProjectTransactionLog.Purge();
                            }
                        }
                    }
                    // transaction log <GS-15032010>
                    btnCloseProject.Enabled = true;
                    btnAddNoteToProjectLog.Enabled = true;
                    btnEditProject.Enabled = true;
                    btnShowProjectLogbook.Enabled = true;
                    btnProduceLatestBinary.Enabled = true;
                    //btncreateb                    
                    btnRebuildFile.Enabled = true;
                    CreateProjectBackupFile();
                    UpdateRollbackForwardControls();
                    m_appSettings.Lastprojectname = m_CurrentWorkingProject;
                    this.Text = "T8SuitePro [Project: " + projectname + "]";
                }
            }
        }

        private void UpdateRollbackForwardControls()
        {
            btnRollback.Enabled = false;
            btnRollforward.Enabled = false;
            btnShowTransactionLog.Enabled = false;

            for (int t = m_ProjectTransactionLog.TransCollection.Count - 1; t >= 0; t--)
            {
                if (!btnShowTransactionLog.Enabled) btnShowTransactionLog.Enabled = true;
                if (m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                {
                    btnRollforward.Enabled = true;
                }
                else
                {
                    btnRollback.Enabled = true;
                }
            }
        }

        private void CreateProjectBackupFile()
        {
            // create a backup file automatically! <GS-16032010>
            if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups");
            string filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
            File.Copy(GetBinaryForProject(m_CurrentWorkingProject), filename);
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.BackupfileCreated, filename);
            }


        }


        private void LoadBinaryForProject(string projectname)
        {
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                projectprops.Columns.Add("CARMAKE");
                projectprops.Columns.Add("CARMODEL");
                projectprops.Columns.Add("CARMY");
                projectprops.Columns.Add("CARVIN");
                projectprops.Columns.Add("NAME");
                projectprops.Columns.Add("BINFILE");
                projectprops.Columns.Add("VERSION");
                projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml");
                // valid project, add it to the list
                if (projectprops.Rows.Count > 0)
                {
                    m_currentfile = projectprops.Rows[0]["BINFILE"].ToString();//Application.StartupPath + "\\55559437  81f.bin";
                    int symboltableoffset = 0;

                    TryToOpenFile(projectprops.Rows[0]["BINFILE"].ToString(), out m_symbols, m_currentfile_size);

                    this.Text = "T8Suite Professional [" + Path.GetFileName(m_currentfile) + "]";
                    barFilenameText.Caption = Path.GetFileName(m_currentfile);
                    gridControlSymbols.DataSource = m_symbols;
                    m_appSettings.Lastfilename = m_currentfile;
                    gridViewSymbols.BestFitColumns();
                    UpdateChecksum(m_currentfile, m_appSettings.AutoChecksum);

                    //OpenWorkingFile(projectprops.Rows[0]["BINFILE"].ToString());
                }
            }
        }

        private string GetBinaryForProject(string projectname)
        {
            string retval = m_currentfile;
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                projectprops.Columns.Add("CARMAKE");
                projectprops.Columns.Add("CARMODEL");
                projectprops.Columns.Add("CARMY");
                projectprops.Columns.Add("CARVIN");
                projectprops.Columns.Add("NAME");
                projectprops.Columns.Add("BINFILE");
                projectprops.Columns.Add("VERSION");
                projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml");
                // valid project, add it to the list
                if (projectprops.Rows.Count > 0)
                {
                    retval = projectprops.Rows[0]["BINFILE"].ToString();
                }
            }
            return retval;
        }

        private string GetBackupOlderThanDateTime(string project, DateTime mileDT)
        {
            string retval = m_currentfile; // default = current file
            string BackupPath = m_appSettings.ProjectFolder + "\\" + project + "\\Backups";
            DateTime MaxDateTime = DateTime.MinValue;
            string foundBackupfile = string.Empty;
            if (Directory.Exists(BackupPath))
            {
                string[] backupfiles = Directory.GetFiles(BackupPath, "*.bin");
                foreach (string backupfile in backupfiles)
                {
                    FileInfo fi = new FileInfo(backupfile);
                    if (fi.LastAccessTime > MaxDateTime && fi.LastAccessTime <= mileDT)
                    {
                        MaxDateTime = fi.LastAccessTime;
                        foundBackupfile = backupfile;
                    }
                }
            }
            if (foundBackupfile != string.Empty)
            {
                retval = foundBackupfile;
            }
            return retval;
        }

        private void btnCreateNewProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the project properties screen for the user to fill in
            // if a bin file is loaded, ask the user whether this should be the new projects binary file
            // the project XML should contain a reference to this binfile as well as a lot of other stuff
            frmProjectProperties projectprops = new frmProjectProperties();
            if (m_currentfile != string.Empty)
            {
                projectprops.BinaryFile = m_currentfile;
                T8Header fileheader = new T8Header();
                fileheader.init(m_currentfile);
                projectprops.CarModel = fileheader.CarDescription.Trim();

                projectprops.ProjectName = fileheader.PartNumber.Trim() + " " + fileheader.SoftwareVersion.Trim();
            }
            if (projectprops.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
                // create a new folder with these project properties.
                // also copy the binary file into the subfolder for this project
                if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName)))
                {
                    frmInfoBox info = new frmInfoBox("The chosen projectname already exists, please choose another one");
                    //TODO: reshow the dialog
                }
                else
                {
                    // create the project
                    Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName));
                    // copy the selected binary file to this folder
                    string binfilename = m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName) + "\\" + Path.GetFileName(projectprops.BinaryFile);
                    File.Copy(projectprops.BinaryFile, binfilename);
                    // now create the projectproperties.xml in this new folder
                    System.Data.DataTable dtProps = new System.Data.DataTable("T5PROJECT");
                    dtProps.Columns.Add("CARMAKE");
                    dtProps.Columns.Add("CARMODEL");
                    dtProps.Columns.Add("CARMY");
                    dtProps.Columns.Add("CARVIN");
                    dtProps.Columns.Add("NAME");
                    dtProps.Columns.Add("BINFILE");
                    dtProps.Columns.Add("VERSION");
                    dtProps.Rows.Add(projectprops.CarMake, projectprops.CarModel, projectprops.CarMY, projectprops.CarVIN, MakeDirName(projectprops.ProjectName), binfilename, projectprops.Version);
                    dtProps.WriteXml(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName) + "\\projectproperties.xml");
                    OpenProject(projectprops.ProjectName); //?
                }
            }
        }

        private void btnOpenProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO let the user select a project from the Project folder. If none are present, let the user know
            if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
            System.Data.DataTable ValidProjects = new System.Data.DataTable();
            ValidProjects.Columns.Add("Projectname");
            ValidProjects.Columns.Add("NumberBackups");
            ValidProjects.Columns.Add("NumberTransactions");
            ValidProjects.Columns.Add("DateTimeModified");
            ValidProjects.Columns.Add("Version");
            string[] projects = Directory.GetDirectories(m_appSettings.ProjectFolder);
            // filter for folders with a projectproperties.xml file
            foreach (string project in projects)
            {
                string[] projectfiles = Directory.GetFiles(project, "projectproperties.xml");

                if (projectfiles.Length > 0)
                {
                    System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
                    projectprops.Columns.Add("CARMAKE");
                    projectprops.Columns.Add("CARMODEL");
                    projectprops.Columns.Add("CARMY");
                    projectprops.Columns.Add("CARVIN");
                    projectprops.Columns.Add("NAME");
                    projectprops.Columns.Add("BINFILE");
                    projectprops.Columns.Add("VERSION");
                    projectprops.ReadXml((string)projectfiles.GetValue(0));
                    // valid project, add it to the list
                    if (projectprops.Rows.Count > 0)
                    {
                        string projectName = projectprops.Rows[0]["NAME"].ToString();
                        ValidProjects.Rows.Add(projectName, GetNumberOfBackups(projectName), GetNumberOfTransactions(projectName), GetLastAccessTime(projectprops.Rows[0]["BINFILE"].ToString()), projectprops.Rows[0]["VERSION"].ToString());
                    }
                }
            }
            if (ValidProjects.Rows.Count > 0)
            {
                frmProjectSelection projselection = new frmProjectSelection();
                projselection.SetDataSource(ValidProjects);
                if (projselection.ShowDialog() == DialogResult.OK)
                {
                    string selectedproject = projselection.GetProjectName();
                    if (selectedproject != "")
                    {
                        OpenProject(selectedproject);
                    }

                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No projects were found, please create one first!");
            }
        }

        private int GetNumberOfBackups(string project)
        {
            int retval = 0;
            string dirname = m_appSettings.ProjectFolder + "\\" + project + "\\Backups";
            if (!Directory.Exists(dirname)) Directory.CreateDirectory(dirname);
            string[] backupfiles = Directory.GetFiles(dirname, "*.bin");
            retval = backupfiles.Length;
            return retval;
        }

        private int GetNumberOfTransactions(string project)
        {
            int retval = 0;
            string filename = m_appSettings.ProjectFolder + "\\" + project + "\\TransActionLogV2.ttl";
            if (File.Exists(filename))
            {
                TrionicTransactionLog translog = new TrionicTransactionLog();
                translog.OpenTransActionLog(m_appSettings.ProjectFolder, project);
                translog.ReadTransactionFile();
                retval = translog.TransCollection.Count;
            }
            return retval;
        }

        private DateTime GetLastAccessTime(string filename)
        {
            DateTime retval = DateTime.MinValue;
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                retval = fi.LastAccessTime;
            }
            return retval;
        }

        private void CloseProject()
        {
            /*if (_ecuConnection.Opened) StopOnlineMode();// StopECUConnection();
            if (m_CurrentWorkingProject != "")
            {
                if (m_AFRMaps != null)
                {
                    m_AFRMaps.SaveMaps();
                }
            }*/

            m_CurrentWorkingProject = string.Empty;
            // unload the current file
            m_currentfile = string.Empty;
            gridControlSymbols.DataSource = null;
            barFilenameText.Caption = "No file";
            //barButtonItem4.Enabled = false;
            m_appSettings.Lastfilename = string.Empty;
            btnCloseProject.Enabled = false;
            btnShowProjectLogbook.Enabled = false;
            btnProduceLatestBinary.Enabled = false;
            btnAddNoteToProjectLog.Enabled = false;
            btnEditProject.Enabled = false;

            btnRebuildFile.Enabled = false;
            btnRollback.Enabled = false;
            btnRollforward.Enabled = false;
            btnShowTransactionLog.Enabled = false;
            this.Text = "T8SuitePro";
        }

        private void btnCloseProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            CloseProject();
            m_appSettings.Lastprojectname = "";
        }

        private void btnShowTransactionLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show new form
            if (m_CurrentWorkingProject != string.Empty)
            {
                frmTransactionLog translog = new frmTransactionLog();
                translog.onRollBack += new frmTransactionLog.RollBack(translog_onRollBack);
                translog.onRollForward += new frmTransactionLog.RollForward(translog_onRollForward);
                translog.onNoteChanged += new frmTransactionLog.NoteChanged(translog_onNoteChanged);
                foreach (TransactionEntry entry in m_ProjectTransactionLog.TransCollection)
                {
                    entry.SymbolName = GetSymbolNameByAddress(entry.SymbolAddress);

                }
                translog.SetTransactionLog(m_ProjectTransactionLog);
                translog.Show();
            }
        }

        void translog_onNoteChanged(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            m_ProjectTransactionLog.SetEntryNote(e.Entry);
        }

        void translog_onRollForward(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            // alter the log!
            // rollback the transaction
            // now reload the list
            RollForward(e.Entry);
            if (sender is frmTransactionLog)
            {
                frmTransactionLog logfrm = (frmTransactionLog)sender;
                logfrm.SetTransactionLog(m_ProjectTransactionLog);
            }
        }

        private void RollForward(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > m_currentfile_size) addressToWrite -= m_currentfile_size;
            savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataAfter, m_currentfile, false);
            UpdateChecksum(m_currentfile, true);
            //m_trionicFile.WriteDataNoLog(entry.DataAfter, (uint)addressToWrite);
            m_ProjectTransactionLog.SetEntryRolledForward(entry.TransactionNumber);
            if (m_CurrentWorkingProject != string.Empty)
            {

                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledforward, GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }

            UpdateRollbackForwardControls();
        }

        void translog_onRollBack(object sender, frmTransactionLog.RollInformationEventArgs e)
        {
            // alter the log!
            // rollback the transaction
            RollBack(e.Entry);
            // now reload the list
            if (sender is frmTransactionLog)
            {
                frmTransactionLog logfrm = (frmTransactionLog)sender;
                logfrm.SetTransactionLog(m_ProjectTransactionLog);
            }
        }

        private void SignalTransactionLogChanged(int SymbolAddress, string Note)
        {
            UpdateRollbackForwardControls();
            // should contain the new info as well
            // <GS-18032010> insert logbook entry here if project is opened
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionExecuted, GetSymbolNameByAddress(SymbolAddress) + " " + Note);
            }
        }

        private string GetSymbolNameByAddress(Int32 address)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Flash_start_address == address) return sh.Varname;
            }
            return address.ToString();
        }

        private void RollBack(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > m_currentfile_size) addressToWrite -= m_currentfile_size;
            //m_trionicFile.WriteDataNoLog(entry.DataBefore, (uint)addressToWrite);
            savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataBefore, m_currentfile, false);
            UpdateChecksum(m_currentfile, true);
            m_ProjectTransactionLog.SetEntryRolledBack(entry.TransactionNumber);
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledback, GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }

            UpdateRollbackForwardControls();
        }

        private void btnRollback_ItemClick(object sender, ItemClickEventArgs e)
        {
            //roll back last entry in the log that has not been rolled back
            for (int t = m_ProjectTransactionLog.TransCollection.Count - 1; t >= 0; t--)
            {
                if (!m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                {
                    RollBack(m_ProjectTransactionLog.TransCollection[t]);

                    break;
                }
            }
        }

        private void btnRollforward_ItemClick(object sender, ItemClickEventArgs e)
        {
            //roll back last entry in the log that has not been rolled back
            for (int t = 0; t < m_ProjectTransactionLog.TransCollection.Count; t++)
            {
                if (m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                {
                    RollForward(m_ProjectTransactionLog.TransCollection[t]);

                    break;
                }
            }
        }

        private void btnRebuildFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the transactionlog again and ask the user upto what datetime he wants to rebuild the file
            // first ask a datetime
            frmRebuildFileParameters filepar = new frmRebuildFileParameters();
            if (filepar.ShowDialog() == DialogResult.OK)
            {

                // get the last backup that is older than the selected datetime
                string file2Process = GetBackupOlderThanDateTime(m_CurrentWorkingProject, filepar.SelectedDateTime);
                // now rebuild the file
                // first create a copy of this file
                string tempRebuildFile = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "rebuild.bin";
                if (File.Exists(tempRebuildFile))
                {
                    File.Delete(tempRebuildFile);
                }
                // CREATE A BACKUP FILE HERE
                CreateProjectBackupFile();
                File.Copy(file2Process, tempRebuildFile);
                // now do all the transactions newer than this file and older than the selected date time
                //IECUFile m_RebuildFile = new Trionic5File();
                //m_RebuildFile.LibraryPath = Application.StartupPath + "\\Binaries";

                //IECUFileInformation m_RebuildFileInformation = new Trionic5FileInformation();

                //m_RebuildFile.SelectFile(tempRebuildFile);
                //m_RebuildFileInformation = m_RebuildFile.ParseFile();
                FileInfo fi = new FileInfo(file2Process);
                foreach (TransactionEntry te in m_ProjectTransactionLog.TransCollection)
                {
                    if (te.EntryDateTime >= fi.LastAccessTime && te.EntryDateTime <= filepar.SelectedDateTime)
                    {
                        // apply this change
                        RollForwardOnFile(tempRebuildFile, te);
                    }
                }
                // rename/copy file
                if (filepar.UseAsNewProjectFile)
                {
                    // just delete the current file
                    File.Delete(m_currentfile);
                    File.Copy(tempRebuildFile, m_currentfile);
                    File.Delete(tempRebuildFile);
                    // done
                }
                else
                {
                    // ask for destination file
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save rebuild file as...";
                    sfd.Filter = "Binary files|*.bin";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        File.Copy(tempRebuildFile, sfd.FileName);
                        File.Delete(tempRebuildFile);
                    }
                }
                if (m_CurrentWorkingProject != string.Empty)
                {
                    m_ProjectLog.WriteLogbookEntry(LogbookEntryType.ProjectFileRecreated, "Reconstruct upto " + filepar.SelectedDateTime.ToString("dd/MM/yyyy") + " selected file " + file2Process);
                }
                UpdateRollbackForwardControls();
            }
        }

        private void RollForwardOnFile(string file2Rollback, TransactionEntry entry)
        {
            FileInfo fi = new FileInfo(file2Rollback);
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > fi.Length) addressToWrite -= (int)fi.Length;
            savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataAfter, file2Rollback, false);
            UpdateChecksum(m_currentfile, true);
        }

        private void btnEditProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                EditProjectProperties(m_CurrentWorkingProject);
            }
        }

        private void EditProjectProperties(string project)
        {
            // edit current project properties
            System.Data.DataTable projectprops = new System.Data.DataTable("T5PROJECT");
            projectprops.Columns.Add("CARMAKE");
            projectprops.Columns.Add("CARMODEL");
            projectprops.Columns.Add("CARMY");
            projectprops.Columns.Add("CARVIN");
            projectprops.Columns.Add("NAME");
            projectprops.Columns.Add("BINFILE");
            projectprops.Columns.Add("VERSION");
            projectprops.ReadXml(m_appSettings.ProjectFolder + "\\" + project + "\\projectproperties.xml");

            frmProjectProperties projectproperties = new frmProjectProperties();
            projectproperties.Version = projectprops.Rows[0]["VERSION"].ToString();
            projectproperties.ProjectName = projectprops.Rows[0]["NAME"].ToString();
            projectproperties.CarMake = projectprops.Rows[0]["CARMAKE"].ToString();
            projectproperties.CarModel = projectprops.Rows[0]["CARMODEL"].ToString();
            projectproperties.CarVIN = projectprops.Rows[0]["CARVIN"].ToString();
            projectproperties.CarMY = projectprops.Rows[0]["CARMY"].ToString();
            projectproperties.BinaryFile = projectprops.Rows[0]["BINFILE"].ToString();
            bool _reopenProject = false;
            if (projectproperties.ShowDialog() == DialogResult.OK)
            {
                // delete the original XML file
                if (project != projectproperties.ProjectName)
                {
                    Directory.Move(m_appSettings.ProjectFolder + "\\" + project, m_appSettings.ProjectFolder + "\\" + projectproperties.ProjectName);
                    project = projectproperties.ProjectName;
                    m_CurrentWorkingProject = project;
                    // set the working file to the correct folder
                    projectproperties.BinaryFile = Path.Combine(m_appSettings.ProjectFolder + "\\" + project, Path.GetFileName(projectprops.Rows[0]["BINFILE"].ToString()));
                    _reopenProject = true;
                    // open this project

                }

                File.Delete(m_appSettings.ProjectFolder + "\\" + project + "\\projectproperties.xml");
                System.Data.DataTable dtProps = new System.Data.DataTable("T5PROJECT");
                dtProps.Columns.Add("CARMAKE");
                dtProps.Columns.Add("CARMODEL");
                dtProps.Columns.Add("CARMY");
                dtProps.Columns.Add("CARVIN");
                dtProps.Columns.Add("NAME");
                dtProps.Columns.Add("BINFILE");
                dtProps.Columns.Add("VERSION");
                dtProps.Rows.Add(projectproperties.CarMake, projectproperties.CarModel, projectproperties.CarMY, projectproperties.CarVIN, MakeDirName(projectproperties.ProjectName), projectproperties.BinaryFile, projectproperties.Version);
                dtProps.WriteXml(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectproperties.ProjectName) + "\\projectproperties.xml");
                if (_reopenProject)
                {
                    OpenProject(m_CurrentWorkingProject);
                }
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.PropertiesEdited, projectproperties.Version);

            }

        }

        private void btnAddNoteToProjectLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmChangeNote newNote = new frmChangeNote();
            newNote.ShowDialog();
            if (newNote.Note != string.Empty)
            {
                if (m_CurrentWorkingProject != string.Empty)
                {
                    m_ProjectLog.WriteLogbookEntry(LogbookEntryType.Note, newNote.Note);
                }
            }
        }

        private void btnShowProjectLogbook_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                frmProjectLogbook logb = new frmProjectLogbook();

                logb.LoadLogbookForProject(m_appSettings.ProjectFolder, m_CurrentWorkingProject);
                logb.Show();
            }
        }

        private void btnProduceLatestBinary_ItemClick(object sender, ItemClickEventArgs e)
        {
            // save binary as
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // copy the current project file to the selected destination
                File.Copy(m_currentfile, sfd.FileName, true);
            }
        }

        private bool _isMouseDown = false;

        private void gridViewSymbols_DragObjectStart(object sender, DevExpress.XtraGrid.Views.Base.DragObjectStartEventArgs e)
        {
            Console.WriteLine("Start dragging: " + e.DragObject.ToString());
            _isMouseDown = true;
        }

        private void gridControlSymbols_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
        }

        private void gridControlSymbols_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        private void gridControlSymbols_MouseLeave(object sender, EventArgs e)
        {
            _isMouseDown = false;
        }

        private void btnReleaseNotes_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartReleaseNotesViewer(m_msiUpdater.GetReleaseNotes(), System.Windows.Forms.Application.ProductVersion.ToString());
        }

        private string m_swversion = string.Empty;

        private void btnToggleRealtime_ItemClick(object sender, ItemClickEventArgs e)
        {
            // start realtime mode
            if (dockRealtime.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
            {
                dockRealtime.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                tmrRealtime.Enabled = false;
                barConnectedECUName.Caption = string.Empty;

            }
            else
            {
                if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
                {
                    t8can.setCANDevice("LAWICEL");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    t8can.setCANDevice("COMBI");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
                {
                    Console.WriteLine("Setting ELM327 comport: " + m_appSettings.ELM327Port);
                    t8can.ForcedComport = m_appSettings.ELM327Port;
                    t8can.setCANDevice("ELM327");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
                {
                    t8can.setCANDevice("JUST4TRIONIC");
                }
                //t8can.EnableCanLog = true; // for testing only
                if (!t8can.isOpen())
                {
                    t8can.openDevice(true);
                }
                if (t8can.isOpen())
                {
                    // whatever
                    // get the software version from the ECU
                    if (m_swversion == string.Empty) // not yet connected
                    {
                        m_realtimeAddresses = new System.Data.DataTable();
                        m_realtimeAddresses.Columns.Add("SymbolName");
                        m_realtimeAddresses.Columns.Add("SymbolNumber", System.Type.GetType("System.Int32"));
                        m_realtimeAddresses.Columns.Add("VarName");

                        m_swversion = t8can.GetSoftwareVersion();
                        //Console.WriteLine("Version: " + m_swversion);
                        Console.WriteLine("***");
                        for (int i = 0; i < m_swversion.Length; i++)
                        {
                            Console.Write(Convert.ToByte(m_swversion[i]).ToString("X2") + " ");
                        }
                        Console.WriteLine("***");
                        barConnectedECUName.Caption = m_swversion;

                        // fill realtime table
                    }
                    if (m_appSettings.ResetRealtimeSymbolOnTabPageSwitch)
                    {
                        FillRealtimeTable(MonitorType.Dashboard); // default
                    }
                    dockRealtime.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    int width = dockManager1.Form.ClientSize.Width - dockSymbols.Width;
                    int height = dockManager1.Form.ClientSize.Height;
                    if (width > 660) width = 660;

                    dockRealtime.Dock = DockingStyle.Left;
                    dockRealtime.Width = width;
                    // set default skin
                    SwitchRealtimePanelMode(m_appSettings.Panelmode);
                    m_connectedToECU = true;
                    tmrRealtime.Enabled = true;
                }
            }
        }

        private void btnSRAMSnapshot_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Snapshots|*.RAM";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
                {
                    t8can.setCANDevice("LAWICEL");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    t8can.setCANDevice("COMBI");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
                {
                    t8can.ForcedComport = m_appSettings.ELM327Port;
                    t8can.setCANDevice("ELM327");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
                {
                    t8can.setCANDevice("JUST4TRIONIC");
                }
                SetProgress("Starting snapshot download");
                if (!t8can.isOpen())
                {
                    t8can.openDevice(false);
                }
                if (t8can.isOpen())
                {

                    m_prohibitReading = true;
                    byte[] snapshot = t8can.getSRAMSnapshotWithBootloader();
                    File.WriteAllBytes(sfd.FileName, snapshot);
                    frmInfoBox info = new frmInfoBox("Snapshot done");
                    m_prohibitReading = false;
                    SetProgressIdle();
                }
            }
        }

        private string GetLogWorksFromRegistry()
        {
            RegistryKey TempKey = null;
            string foundvalue = string.Empty;
            TempKey = Registry.LocalMachine;

            using (RegistryKey Settings = TempKey.OpenSubKey("SOFTWARE\\Classes\\d32FileHandler\\Shell\\Open\\Command"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            foundvalue = Settings.GetValue(a).ToString();
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }

            if (foundvalue == string.Empty)
            {
                using (RegistryKey Settings = TempKey.OpenSubKey("SOFTWARE\\Classes\\Applications\\LogWorks2.exe\\shell\\Open\\Command"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                foundvalue = Settings.GetValue(a).ToString();
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }
            }
            return foundvalue;
        }

        private void btnExportToLogWorks_ItemClick(object sender, ItemClickEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 8 logfiles|*.t8l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            string logworksstring = GetLogWorksFromRegistry();
            if (logworksstring == string.Empty)
            {
                if (MessageBox.Show("Logworks is not installed on this computer, do you wish to install it now?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start(System.Windows.Forms.Application.StartupPath + "\\LogWorks\\LogWorks3Setup.exe");
                }
            }
            else if (ofd.ShowDialog() == DialogResult.OK)
            {
                //                OpenAndDisplayLogFile(ofd.FileName);
                ConvertFileToDif(ofd.FileName, false); // TEST

            }

        }

        private void ConvertFileToDif(string filename, bool AutoExport)
        {
            System.Windows.Forms.Application.DoEvents();
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            try
            {
                SymbolCollection sc = new SymbolCollection();
                string[] alllines = File.ReadAllLines(filename);
                //using (StreamReader sr = new StreamReader(filename))
                {
                    //string line = string.Empty;
                    char[] sep = new char[1];
                    char[] sep2 = new char[1];
                    //int linecount = 0;
                    sep.SetValue('|', 0);
                    sep2.SetValue('=', 0);
                    //while ((line = sr.ReadLine()) != null)

                    foreach (string line in alllines)
                    {
                        string[] values = line.Split(sep);
                        if (values.Length > 0)
                        {
                            try
                            {
                                //dd/MM/yyyy HH:mm:ss
                                //string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";

                                string dtstring = (string)values.GetValue(0);
                                DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                if (dt > endDate) endDate = dt;
                                if (dt < startDate) startDate = dt;
                                for (int t = 1; t < values.Length; t++)
                                {
                                    string subvalue = (string)values.GetValue(t);
                                    string[] subvals = subvalue.Split(sep2);
                                    if (subvals.Length == 2)
                                    {
                                        string varname = (string)subvals.GetValue(0);
                                        bool sfound = false;
                                        foreach (SymbolHelper sh in sc)
                                        {
                                            if (sh.Varname == varname || sh.Userdescription == varname)
                                            {
                                                sfound = true;
                                            }
                                        }
                                        SymbolHelper nsh = new SymbolHelper();
                                        nsh.Varname = varname;
                                        if (!sfound) sc.Add(nsh);
                                    }
                                }
                            }
                            catch (Exception pE)
                            {
                                Console.WriteLine(pE.Message);
                            }
                        }
                    }
                }

                if (AutoExport)
                {
                    foreach (SymbolHelper sh in sc)
                    {
                        sh.Color = GetColorFromRegistry(sh.Varname);
                    }
                    DifGenerator difgen = new DifGenerator();
                    difgen.AppSettings = m_appSettings;

                    difgen.WidebandSymbol = "";// m_appSettings.WideBandSymbol;
                    difgen.UseWidebandInput = false;
                    difgen.onExportProgress += new DifGenerator.ExportProgress(difgen_onExportProgress);
                    frmProgressLogWorks = new frmProgress();
                    frmProgressLogWorks.SetProgress("Exporting to LogWorks");
                    frmProgressLogWorks.Show();
                    System.Windows.Forms.Application.DoEvents();
                    try
                    {
                        difgen.ConvertFileToDif(filename, sc, startDate, endDate, m_appSettings.InterpolateLogWorksTimescale, m_appSettings.InterpolateLogWorksTimescale);
                    }
                    catch (Exception expE1)
                    {
                        Console.WriteLine(expE1.Message);
                    }
                    frmProgressLogWorks.Close();
                }
                else
                {

                    // show selection screen
                    frmPlotSelection plotsel = new frmPlotSelection();
                    foreach (SymbolHelper sh in sc)
                    {
                        plotsel.AddItemToList(sh.Varname);
                    }
                    plotsel.Startdate = startDate;
                    plotsel.Enddate = endDate;
                    plotsel.SelectAllSymbols();
                    if (plotsel.ShowDialog() == DialogResult.OK)
                    {
                        sc = plotsel.Sc;
                        endDate = plotsel.Enddate;
                        startDate = plotsel.Startdate;
                        DifGenerator difgen = new DifGenerator();
                        LogFilters filterhelper = new LogFilters();
                        difgen.SetFilters(filterhelper.GetFiltersFromRegistry());
                        difgen.AppSettings = m_appSettings;
                        //difgen.LowAFR = m_appSettings.WidebandLowAFR;
                        //difgen.HighAFR = m_appSettings.WidebandHighAFR;
                        //difgen.MaximumVoltageWideband = m_appSettings.WidebandHighVoltage;
                        //difgen.MinimumVoltageWideband = m_appSettings.WidebandLowVoltage;
                        difgen.WidebandSymbol = ""; //m_appSettings.WideBandSymbol;
                        //difgen.UseWidebandInput = m_appSettings.UseWidebandLambdaThroughSymbol;
                        difgen.UseWidebandInput = false;

                        difgen.onExportProgress += new DifGenerator.ExportProgress(difgen_onExportProgress);
                        frmProgressLogWorks = new frmProgress();
                        frmProgressLogWorks.SetProgress("Exporting to LogWorks");
                        frmProgressLogWorks.Show();
                        System.Windows.Forms.Application.DoEvents();
                        try
                        {
                            if (difgen.ConvertFileToDif(filename, sc, startDate, endDate, m_appSettings.InterpolateLogWorksTimescale, m_appSettings.InterpolateLogWorksTimescale))
                            {
                                StartLogWorksWithCurrentFile(Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".dif");
                            }
                            else
                            {
                                frmInfoBox info = new frmInfoBox("No data was found to export!");
                            }
                        }
                        catch (Exception expE2)
                        {
                            Console.WriteLine(expE2.Message);
                        }
                        frmProgressLogWorks.Close();
                    }
                    TimeSpan ts = new TimeSpan(endDate.Ticks - startDate.Ticks);
                    //MessageBox.Show("LogFile should be " + ts.ToString());
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void StartLogWorksWithCurrentFile(string filename)
        {
            try
            {
                string logworksstring = GetLogWorksFromRegistry();
                if (logworksstring != string.Empty)
                {
                    logworksstring = logworksstring.Substring(1, logworksstring.Length - 1);
                    int idx = logworksstring.IndexOf('\"');
                    if (idx > 0)
                    {
                        logworksstring = logworksstring.Substring(0, idx);
                        //string parameterstring = "\"" + Path.GetDirectoryName(m_currentfile) + "\\" + DateTime.Now.ToString("yyyyMMdd") + "-CanTraceExt.dif" + "\"";
                        //                        Console.WriteLine(logworksstring);
                        //Console.WriteLine(parameterstring);

                        System.Diagnostics.Process.Start(logworksstring, "\"" + filename + "\"");
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        void difgen_onExportProgress(object sender, DifGenerator.ProgressEventArgs e)
        {
            frmProgressLogWorks.SetProgressPercentage(e.Percentage);
        }

        private Color GetColorFromRegistry(string symbolname)
        {
            Color c = Color.Black;
            Int32 win32color = GetValueFromRegistry(symbolname);
            c = Color.FromArgb((int)win32color);
            //c = System.Drawing.ColorTranslator.FromWin32(win32color);
            return c;
        }


        private Int32 GetValueFromRegistry(string symbolname)
        {
            RegistryKey TempKey = null;
            Int32 win32color = 0;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T8SuitePro\\SymbolColors"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == symbolname)
                            {
                                string value = Settings.GetValue(a).ToString();
                                win32color = Convert.ToInt32(value);
                            }
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            return win32color;
        }

        private void SaveRegistryColor(string key, int value)
        {
            RegistryKey TempKey = null;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey saveSettings = TempKey.CreateSubKey("T8SuitePro\\SymbolColors"))
            {
                saveSettings.SetValue(key, value.ToString(), RegistryValueKind.String);
            }
        }

        private void CheckDefaultSymbolColors()
        {
            if (GetValueFromRegistry("ActualIn.n_Engine") == 0) SaveRegistryColor("ActualIn.n_Engine", ColorTranslator.ToWin32(Color.LightCyan));
            if (GetValueFromRegistry("ActualIn.T_Engine") == 0) SaveRegistryColor("ActualIn.T_Engine", ColorTranslator.ToWin32(Color.LightSalmon));
            if (GetValueFromRegistry("ActualIn.T_AirInlet") == 0) SaveRegistryColor("ActualIn.T_AirInlet", ColorTranslator.ToWin32(Color.LightBlue));
            if (GetValueFromRegistry("ECMStat.ST_ActiveAirDem") == 0) SaveRegistryColor("ECMStat.ST_ActiveAirDem", ColorTranslator.ToWin32(Color.Lime));
            if (GetValueFromRegistry("Lambda.Status") == 0) SaveRegistryColor("Lambda.Status", ColorTranslator.ToWin32(Color.Azure));
            if (GetValueFromRegistry("IgnMastProt.fi_Offset") == 0) SaveRegistryColor("IgnMastProt.fi_Offset", ColorTranslator.ToWin32(Color.Pink));
            if (GetValueFromRegistry("AirMassMast.m_Request") == 0) SaveRegistryColor("AirMassMast.m_Request", ColorTranslator.ToWin32(Color.Orange));
            if (GetValueFromRegistry("Out.M_EngTrqAct") == 0) SaveRegistryColor("Out.M_EngTrqAct", ColorTranslator.ToWin32(Color.DarkGray));
            if (GetValueFromRegistry("ECMStat.P_Engine") == 0) SaveRegistryColor("ECMStat.P_Engine", ColorTranslator.ToWin32(Color.LightSlateGray));
            if (GetValueFromRegistry("In.p_AirInlet") == 0) SaveRegistryColor("In.p_AirInlet", ColorTranslator.ToWin32(Color.Red));
            if (GetValueFromRegistry("ECMStat.p_Diff") == 0) SaveRegistryColor("ECMStat.p_Diff", ColorTranslator.ToWin32(Color.Red));
            if (GetValueFromRegistry("Out.PWM_BoostCntrl") == 0) SaveRegistryColor("Out.PWM_BoostCntrl", ColorTranslator.ToWin32(Color.Wheat));
            if (GetValueFromRegistry("Out.fi_Ignition") == 0) SaveRegistryColor("Out.fi_Ignition", ColorTranslator.ToWin32(Color.LightGoldenrodYellow));
            if (GetValueFromRegistry("Out.X_AccPos") == 0) SaveRegistryColor("Out.X_AccPos", ColorTranslator.ToWin32(Color.Crimson));
            if (GetValueFromRegistry("MAF.m_AirInlet") == 0) SaveRegistryColor("MAF.m_AirInlet", ColorTranslator.ToWin32(Color.OrangeRed));
            if (GetValueFromRegistry("In.v_Vehicle") == 0) SaveRegistryColor("In.v_Vehicle", ColorTranslator.ToWin32(Color.Green));
            if (GetValueFromRegistry("Exhaust.T_Calc") == 0) SaveRegistryColor("Exhaust.T_Calc", ColorTranslator.ToWin32(Color.Goldenrod));
            if (GetValueFromRegistry("BFuelProt.CurrentFuelCon") == 0) SaveRegistryColor("BFuelProt.CurrentFuelCon", ColorTranslator.ToWin32(Color.White));
        }

        private void SwitchRealtimePanelMode(PanelMode panelMode)
        {
            m_appSettings.Panelmode = panelMode;
            Color backColor = Color.FromArgb(232, 232, 232);
            Color foreColor = Color.Black;
            Color labelColor = Color.Black;
            switch (panelMode)
            {
                case PanelMode.Day:
                    // set colorscheme to day, brighter colors
                    backColor = Color.Transparent;//Color.FromArgb(232, 232, 232);
                    foreColor = Color.Black;
                    labelColor = Color.Black;
                    simpleButton1.Text = "Night";
                    break;
                case PanelMode.Night:
                    // set colorscheme to day, darker colors
                    backColor = Color.Black;
                    foreColor = Color.FromArgb(234, 77, 0);
                    labelColor = Color.FromArgb(0, 192, 0);
                    simpleButton1.Text = "Day";
                    break;
            }
            SetTabControlView(xtraTabControl1, panelMode, backColor, labelColor);
            SwitchButtons(panelMode, backColor, labelColor);

            SetColorForMeasurement(measurementAirmassRequest, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementBoost, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementCalculatedPower, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementCalculatedTorque, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementDutyCycleBCV, backColor, foreColor, labelColor);
            //SetColorForMeasurement(measurementEGT, backColor, foreColor);
            SetColorForMeasurement(measurementIgnitionAdvance, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementIgnitionOffset, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementPedalPosition, backColor, foreColor, labelColor);
            SetColorForMeasurement(measurementSpeed, backColor, foreColor, labelColor);

            foreach (Control c in panel2.Controls)
            {
                if (c is Owf.Controls.DigitalDisplayControl)
                {
                    Owf.Controls.DigitalDisplayControl ctrl = (Owf.Controls.DigitalDisplayControl)c;
                    ctrl.DigitColor = foreColor;
                    ctrl.BackColor = backColor;
                }
                else if (c is DevExpress.XtraEditors.LabelControl)
                {
                    DevExpress.XtraEditors.LabelControl lbl = (DevExpress.XtraEditors.LabelControl)c;
                    lbl.BackColor = backColor;
                    if (panelMode == PanelMode.Night)
                    {
                        lbl.ForeColor = labelColor;
                    }
                    else
                    {
                        if (lbl.Name == "labelControl9")
                        {
                            lbl.ForeColor = Color.DarkGreen;
                        }
                        else if (lbl.Name == "labelControl10")
                        {
                            lbl.ForeColor = Color.DarkBlue;
                        }
                        else if (lbl.Name == "labelControl12")
                        {
                            lbl.ForeColor = Color.Maroon;
                        }
                        else
                        {
                            lbl.ForeColor = labelColor;
                        }
                    }
                }
            }
            foreach (Control c in dockRealtime.Controls)
            {
                c.BackColor = backColor;
                c.ForeColor = foreColor;
            }
            if (backColor == Color.Black)
            {
                toolStrip2.BackColor = backColor;
                toolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            }
            else
            {
                toolStrip2.GripStyle = ToolStripGripStyle.Visible;
                toolStrip2.BackColor = Color.Empty;
            }
            gridRealtime.BackColor = backColor;
            gridRealtime.ForeColor = foreColor;
            tableLayoutPanel1.BackColor = backColor;
            tableLayoutPanel1.ForeColor = foreColor;
            linearGauge1.BackColor = backColor;
            linearGauge1.ForeColor = foreColor;
            linearGauge1.TextColor = foreColor;
            linearGauge1.TickColor = foreColor;
            linearGauge2.BackColor = backColor;
            linearGauge2.ForeColor = foreColor;
            linearGauge2.TextColor = foreColor;
            linearGauge2.TickColor = foreColor;
            if (backColor == Color.Black)
            {
                linearGauge1.BevelLineColor = backColor;
                linearGauge2.BevelLineColor = backColor;
            }
            else
            {
                linearGauge1.BevelLineColor = Color.DimGray;
                linearGauge2.BevelLineColor = Color.DimGray;
            }
            SetViewRealtime(panelMode);
            System.Windows.Forms.Application.DoEvents();
        }

        private void SwitchButtons(PanelMode panelMode, Color backColor, Color labelColor)
        {
            //SetButtonStyle(btnEconomyMode, panelMode, backColor, labelColor);
            //SetButtonStyle(btnNormalMode, panelMode, backColor, labelColor);
            //SetButtonStyle(btnSportMode, panelMode, backColor, labelColor);
            SetButtonStyle(simpleButton1, panelMode, backColor, labelColor);
        }

        private void SetButtonStyle(DevExpress.XtraEditors.SimpleButton btn, PanelMode panelMode, Color backColor, Color labelColor)
        {
            if (panelMode == PanelMode.Day)
            {
                btn.LookAndFeel.UseDefaultLookAndFeel = true;
                btn.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                btn.Appearance.BorderColor = Color.Empty;
                btn.Appearance.BackColor = Color.Empty;
                btn.Appearance.BackColor2 = Color.Empty;
                btn.BackColor = Color.Empty;
                btn.ForeColor = Color.Empty;
            }
            else
            {
                btn.LookAndFeel.UseDefaultLookAndFeel = false;

                btn.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                btn.Appearance.BorderColor = backColor;
                btn.Appearance.BackColor = backColor;
                btn.Appearance.BackColor2 = backColor;
                btn.BackColor = backColor;
                btn.ForeColor = labelColor;

            }
        }

        private void SetTabControlView(DevExpress.XtraTab.XtraTabControl tabcontrol, PanelMode panelMode, Color nightColor, Color labelColor)
        {
            //Console.WriteLine("Switching tab control!");
            if (panelMode == PanelMode.Day)
            {
                tabcontrol.LookAndFeel.UseDefaultLookAndFeel = true;
                tabcontrol.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
                tabcontrol.LookAndFeel.SkinName = defaultLookAndFeel1.LookAndFeel.SkinName;

                tabcontrol.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.True;
                tabcontrol.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                tabcontrol.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                tabcontrol.Appearance.BackColor = Color.Empty;
                tabcontrol.Appearance.BackColor2 = Color.Empty;
                tabcontrol.Appearance.BorderColor = Color.Empty;
                tabcontrol.Appearance.ForeColor = Color.Empty;
                tabcontrol.AppearancePage.Header.BorderColor = Color.Empty;
                tabcontrol.AppearancePage.Header.BackColor = Color.Empty;
                tabcontrol.AppearancePage.Header.BackColor2 = Color.Empty;
                tabcontrol.AppearancePage.Header.ForeColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderActive.BorderColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderActive.BackColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderActive.BackColor2 = Color.Empty;
                tabcontrol.AppearancePage.HeaderActive.ForeColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderDisabled.BorderColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderDisabled.BackColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderDisabled.BackColor2 = Color.Empty;
                tabcontrol.AppearancePage.HeaderDisabled.ForeColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderHotTracked.BorderColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderHotTracked.BackColor = Color.Empty;
                tabcontrol.AppearancePage.HeaderHotTracked.BackColor2 = Color.Empty;
                tabcontrol.AppearancePage.HeaderHotTracked.ForeColor = Color.Empty;
                tabcontrol.AppearancePage.PageClient.BackColor = Color.Empty;
                tabcontrol.AppearancePage.PageClient.BackColor2 = Color.Empty;
                tabcontrol.AppearancePage.PageClient.ForeColor = Color.Empty;

            }
            else
            {
                tabcontrol.LookAndFeel.UseDefaultLookAndFeel = false;
                tabcontrol.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                //tabcontrol.LookAndFeel.SkinName = "Dark Side"; 

                tabcontrol.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.True;
                tabcontrol.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                tabcontrol.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                tabcontrol.Appearance.BackColor = nightColor;
                tabcontrol.Appearance.BackColor2 = nightColor;
                tabcontrol.Appearance.BorderColor = nightColor;
                tabcontrol.BackColor = nightColor;
                tabcontrol.Appearance.ForeColor = labelColor;
                tabcontrol.AppearancePage.Header.BorderColor = Color.OrangeRed;
                tabcontrol.AppearancePage.Header.BackColor = nightColor;
                tabcontrol.AppearancePage.Header.BackColor2 = nightColor;
                tabcontrol.AppearancePage.Header.ForeColor = Color.FromArgb(48, 148, 48);
                tabcontrol.AppearancePage.HeaderActive.BorderColor = Color.OrangeRed;
                tabcontrol.AppearancePage.HeaderActive.BackColor = nightColor;
                tabcontrol.AppearancePage.HeaderActive.BackColor2 = nightColor;
                tabcontrol.AppearancePage.HeaderActive.ForeColor = Color.FromArgb(0, 192, 0);
                tabcontrol.AppearancePage.HeaderDisabled.BorderColor = Color.OrangeRed;
                tabcontrol.AppearancePage.HeaderDisabled.BackColor = nightColor;
                tabcontrol.AppearancePage.HeaderDisabled.BackColor2 = nightColor;
                tabcontrol.AppearancePage.HeaderDisabled.ForeColor = Color.FromArgb(0, 192, 0);
                tabcontrol.AppearancePage.HeaderHotTracked.BorderColor = Color.OrangeRed;
                tabcontrol.AppearancePage.HeaderHotTracked.BackColor = nightColor;
                tabcontrol.AppearancePage.HeaderHotTracked.BackColor2 = nightColor;
                tabcontrol.AppearancePage.HeaderHotTracked.ForeColor = Color.FromArgb(0, 192, 0);

                tabcontrol.AppearancePage.PageClient.BackColor = nightColor;
                tabcontrol.AppearancePage.PageClient.BackColor2 = nightColor;
                tabcontrol.AppearancePage.PageClient.ForeColor = Color.FromArgb(0, 192, 0);
            }
            foreach (DevExpress.XtraTab.XtraTabPage page in tabcontrol.TabPages)
            {
                //Console.WriteLine("Switching tab page: " + page.Name);
                SetTabPageView(page, panelMode, nightColor);
            }


        }

        private void SetTabPageView(DevExpress.XtraTab.XtraTabPage page, PanelMode panelMode, Color nightColor)
        {
            if (panelMode == PanelMode.Day)
            {
                page.BackColor = Color.Empty;
                page.Appearance.Header.BackColor = Color.Empty;
                page.Appearance.Header.BackColor2 = Color.Empty;
                page.Appearance.Header.BorderColor = Color.Empty;
                page.Appearance.PageClient.BackColor = Color.Empty;
                page.Appearance.PageClient.BackColor2 = Color.Empty;
                page.Appearance.PageClient.BorderColor = Color.Empty;
            }
            else
            {
                page.BackColor = nightColor;
                page.Appearance.Header.BackColor = nightColor;
                page.Appearance.Header.BackColor2 = nightColor;
                page.Appearance.Header.BorderColor = nightColor;
                page.Appearance.PageClient.BackColor = nightColor;
                page.Appearance.PageClient.BackColor2 = nightColor;
                page.Appearance.PageClient.BorderColor = nightColor;
            }
        }

        private void SetViewRealtime(PanelMode panelMode)
        {
            if (panelMode == PanelMode.Night)
            {
                ViewRealtime.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                ViewRealtime.Appearance.Empty.BackColor = Color.Black;
                ViewRealtime.Appearance.Empty.BackColor2 = Color.Black;
                ViewRealtime.Appearance.Empty.ForeColor = Color.FromArgb(0, 192, 0);
                ViewRealtime.Appearance.FocusedCell.BackColor = Color.Black;
                ViewRealtime.Appearance.FocusedCell.BackColor2 = Color.Black;
                ViewRealtime.Appearance.FocusedCell.ForeColor = Color.FromArgb(0, 192, 0);
                ViewRealtime.Appearance.FocusedRow.BackColor = Color.Black;
                ViewRealtime.Appearance.FocusedRow.BackColor2 = Color.Black;
                ViewRealtime.Appearance.FocusedRow.ForeColor = Color.FromArgb(0, 192, 0);
                ViewRealtime.Appearance.Row.BackColor = Color.Black;
                ViewRealtime.Appearance.Row.BackColor2 = Color.Black;
                ViewRealtime.Appearance.Row.ForeColor = Color.FromArgb(0, 192, 0);
                ViewRealtime.Appearance.SelectedRow.BackColor = Color.Black;
                ViewRealtime.Appearance.SelectedRow.BackColor2 = Color.Black;
                ViewRealtime.Appearance.SelectedRow.ForeColor = Color.FromArgb(0, 192, 0);
                ViewRealtime.OptionsView.ShowColumnHeaders = false;
                ViewRealtime.OptionsView.ShowHorzLines = false;
                ViewRealtime.OptionsView.ShowVertLines = false;
                ViewRealtime.OptionsBehavior.Editable = false;
            }
            else
            {
                ViewRealtime.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                ViewRealtime.Appearance.Empty.BackColor = Color.Empty;
                ViewRealtime.Appearance.Empty.BackColor2 = Color.Empty;
                ViewRealtime.Appearance.Empty.ForeColor = Color.Empty;
                ViewRealtime.Appearance.FocusedCell.BackColor = Color.Empty;
                ViewRealtime.Appearance.FocusedCell.BackColor2 = Color.Empty;
                ViewRealtime.Appearance.FocusedCell.ForeColor = Color.Empty;
                ViewRealtime.Appearance.FocusedRow.BackColor = Color.Empty;
                ViewRealtime.Appearance.FocusedRow.BackColor2 = Color.Empty;
                ViewRealtime.Appearance.FocusedRow.ForeColor = Color.Empty;
                ViewRealtime.Appearance.Row.BackColor = Color.Empty;
                ViewRealtime.Appearance.Row.BackColor2 = Color.Empty;
                ViewRealtime.Appearance.Row.ForeColor = Color.Empty;
                ViewRealtime.Appearance.SelectedRow.BackColor = Color.Empty;
                ViewRealtime.Appearance.SelectedRow.BackColor2 = Color.Empty;
                ViewRealtime.Appearance.SelectedRow.ForeColor = Color.Empty;
                ViewRealtime.OptionsView.ShowColumnHeaders = true;
                ViewRealtime.OptionsView.ShowHorzLines = true;
                ViewRealtime.OptionsView.ShowVertLines = true;
                ViewRealtime.OptionsBehavior.Editable = false;
            }

        }


        private void SetColorForMeasurement(Measurement measurement, Color backColor, Color foreColor, Color labelColor)
        {
            measurement.LookAndFeel.SkinName = "";
            measurement.SetDigitColor = foreColor;
            measurement.SetBackColor = backColor;
            measurement.SetLabelColor = labelColor;
            if (backColor == Color.Black)
            {
                measurement.BorderStyle = BorderStyle.None;
            }
            else
            {
                measurement.BorderStyle = BorderStyle.Fixed3D;
            }
            //measurement.BackColor = backColor;
            //measurement.ForeColor = foreColor;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (simpleButton1.Text == "Night")
            {
                SwitchRealtimePanelMode(PanelMode.Night);

            }
            else
            {
                SwitchRealtimePanelMode(PanelMode.Day);

            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void SaveRealtimeTable()
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                    //dt.WriteXml(System.Windows.Forms.Application.StartupPath + "\\RealtimeTable.xml", XmlWriteMode.WriteSchema);
                    // save the user defined symbols
                    //<GS-24062010>
                    using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\rtsymbols.txt"))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //if (Convert.ToInt32(dr["UserDefined"]) == 1)
                            {
                                sw.WriteLine(dr["SymbolName"].ToString() + "|" + dr["Symbolnumber"].ToString() + "|" + dr["Minimum"].ToString() + "|" + dr["Maximum"].ToString() + "|" + dr["Offset"].ToString() + "|" + dr["Correction"].ToString() + "|" + dr["ConvertedSymbolnumber"].ToString() + "|" + dr["SRAMAddress"].ToString() + "|" + dr["Length"].ToString() + "|" + dr["Description"].ToString().Replace("|", ""));
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to write realtime datatable: " + E.Message);
            }
        }

        private void SaveRealtimeTable(string filename)
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                    //dt.WriteXml(System.Windows.Forms.Application.StartupPath + "\\RealtimeTable.xml", XmlWriteMode.WriteSchema);
                    // save the user defined symbols
                    //<GS-24062010>
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //if (Convert.ToInt32(dr["UserDefined"]) == 1)
                            {
                                sw.WriteLine(dr["SymbolName"].ToString() + "|" + dr["Symbolnumber"].ToString() + "|" + dr["Minimum"].ToString() + "|" + dr["Maximum"].ToString() + "|" + dr["Offset"].ToString() + "|" + dr["Correction"].ToString() + "|" + dr["ConvertedSymbolnumber"].ToString() + "|" + dr["SRAMAddress"].ToString() + "|" + dr["Length"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to write realtime datatable: " + E.Message);
            }
        }

        private void btnSaveRealtimeLayout_Click(object sender, EventArgs e)
        {
            // save the user defined symbols in the list
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Realtime layout files|*.t8rtl";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveRealtimeTable(sfd.FileName);
            }
        }

        private int GetSymbolNumberFromRealtimeList(int number, string symbolname)
        {
            int retval = number;

            if (m_realtimeAddresses != null)
            {
                foreach (DataRow dr in m_realtimeAddresses.Rows)
                {
                    if (dr["VarName"] != null)
                    {
                        if (dr["VarName"] != DBNull.Value)
                        {
                            if (symbolname == dr["VarName"].ToString())
                            {
                                retval = Convert.ToInt32(dr["SymbolNumber"]);
                                if (number != retval)
                                {
                                    //Console.WriteLine("Fetched (" + symbolname + ") number from realtime list: " + retval.ToString());
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return retval;

        }

        private void AddSymbolToRealTimeList(string symbolname, int symbolnumber, double minvalue, double maxvalue, double offset, double correction, string description, uint sramaddress, bool isUserDefined)
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    int userdef = 0;
                    if (isUserDefined) userdef = 1;
                    System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                    bool symbolfound = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["SymbolName"].ToString() == symbolname)
                        {
                            symbolfound = true;
                            // overwrite other  values
                            dr["Description"] = description;
                            dr["Symbolnumber"] = symbolnumber;
                            dr["Minimum"] = minvalue;
                            dr["Maximum"] = maxvalue;
                            dr["Offset"] = offset;
                            dr["Correction"] = correction;
                            dr["Value"] = 0;
                            dr["Peak"] = minvalue;
                            dr["ConvertedSymbolNumber"] = GetSymbolNumberFromRealtimeList(symbolnumber, symbolname);
                            dr["SRAMAddress"] = sramaddress;
                            dr["Length"] = GetSymbolLength(m_symbols, symbolname);
                            dr["UserDefined"] = userdef;
                            dr["Delay"] = 1;
                            dr["Reload"] = 1;
                        }
                    }
                    if (!symbolfound)
                    {
                        // create new one
                        dt.Rows.Add(symbolname, description, symbolnumber, 0, offset, correction, minvalue, minvalue, maxvalue, GetSymbolNumberFromRealtimeList(symbolnumber, symbolname), sramaddress, GetSymbolLength(m_symbols, symbolname), userdef, 1, 1);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to add symbol to realtime list: " + E.Message);
            }
        }

        private void LoadRealtimeTable()
        {
            try
            {
                // create a tabel from scratch
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.TableName = "RTSymbols";
                dt.Columns.Add("SymbolName");
                dt.Columns.Add("Description");
                dt.Columns.Add("Symbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("Value", Type.GetType("System.Double"));
                dt.Columns.Add("Offset", Type.GetType("System.Double"));
                dt.Columns.Add("Correction", Type.GetType("System.Double"));
                dt.Columns.Add("Peak", Type.GetType("System.Double"));
                dt.Columns.Add("Minimum", Type.GetType("System.Double"));
                dt.Columns.Add("Maximum", Type.GetType("System.Double"));
                dt.Columns.Add("ConvertedSymbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("SRAMAddress", Type.GetType("System.Int32"));
                dt.Columns.Add("Length", Type.GetType("System.Int32"));
                dt.Columns.Add("UserDefined", Type.GetType("System.Int32"));
                dt.Columns.Add("Delay", Type.GetType("System.Int32"));
                dt.Columns.Add("Reload", Type.GetType("System.Int32"));
                gridRealtime.DataSource = dt;

                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\rtsymbols.txt"))
                {
                    using (StreamReader sr = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\rtsymbols.txt"))
                    {
                        string line = string.Empty;
                        char[] sep = new char[1];
                        sep.SetValue('|', 0);
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] values = line.Split(sep);
                            if (values.Length == 9)
                            {
                                string symbolname = (string)values.GetValue(0);
                                AddSymbolToRealTimeList(symbolname, GetSymbolNumber(m_symbols, symbolname), ConvertToDouble((string)values.GetValue(2)), ConvertToDouble((string)values.GetValue(3)), ConvertToDouble((string)values.GetValue(4)), ConvertToDouble((string)values.GetValue(5)), symbolname, Convert.ToUInt32((string)values.GetValue(7)), true);
                            }
                            else if (values.Length == 10) // we added the description, so now there are 10 fields.
                            {
                                string symbolname = (string)values.GetValue(0);
                                string description = (string)values.GetValue(9);
                                AddSymbolToRealTimeList(symbolname, GetSymbolNumber(m_symbols, symbolname), ConvertToDouble((string)values.GetValue(2)), ConvertToDouble((string)values.GetValue(3)), ConvertToDouble((string)values.GetValue(4)), ConvertToDouble((string)values.GetValue(5)), description, Convert.ToUInt32((string)values.GetValue(7)), true);
                            }
                        }
                    }
                }

            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to load realtime symbol table: " + E.Message);
            }
        }

        private void LoadRealtimeTable(string filename)
        {
            try
            {
                // create a tabel from scratch
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.TableName = "RTSymbols";
                dt.Columns.Add("SymbolName");
                dt.Columns.Add("Description");
                dt.Columns.Add("Symbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("Value", Type.GetType("System.Double"));
                dt.Columns.Add("Offset", Type.GetType("System.Double"));
                dt.Columns.Add("Correction", Type.GetType("System.Double"));
                dt.Columns.Add("Peak", Type.GetType("System.Double"));
                dt.Columns.Add("Minimum", Type.GetType("System.Double"));
                dt.Columns.Add("Maximum", Type.GetType("System.Double"));
                dt.Columns.Add("ConvertedSymbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("SRAMAddress", Type.GetType("System.Int32"));
                dt.Columns.Add("Length", Type.GetType("System.Int32"));
                dt.Columns.Add("UserDefined", Type.GetType("System.Int32"));
                dt.Columns.Add("Delay", Type.GetType("System.Int32"));
                dt.Columns.Add("Reload", Type.GetType("System.Int32"));
                gridRealtime.DataSource = dt;

                if (File.Exists(filename))
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        string line = string.Empty;
                        char[] sep = new char[1];
                        sep.SetValue('|', 0);
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] values = line.Split(sep);
                            if (values.Length == 9)
                            {
                                string symbolname = (string)values.GetValue(0);
                                AddSymbolToRealTimeList(symbolname, GetSymbolNumber(m_symbols, symbolname), ConvertToDouble((string)values.GetValue(2)), ConvertToDouble((string)values.GetValue(3)), ConvertToDouble((string)values.GetValue(4)), ConvertToDouble((string)values.GetValue(5)), symbolname, Convert.ToUInt32((string)values.GetValue(7)), true);
                            }
                        }
                    }
                }

            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to load realtime symbol table: " + E.Message);
            }
        }

        private void btnLoadRealtimeLayout_Click(object sender, EventArgs e)
        {
            // load user defined symbols
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Realtime layout files|*.t8rtl";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadRealtimeTable(ofd.FileName);
            }
        }

        private void btnAddSymbol_Click(object sender, EventArgs e)
        {
            // add a user defined symbol
            frmEditRealtimeSymbol frmeditsymbol = new frmEditRealtimeSymbol();
            frmeditsymbol.Symbols = m_symbols;
            if (frmeditsymbol.ShowDialog() == DialogResult.OK)
            {
                AddSymbolToRealTimeList(frmeditsymbol.Varname, frmeditsymbol.Symbolnumber, frmeditsymbol.MinimumValue, frmeditsymbol.MaximumValue, frmeditsymbol.OffsetValue, frmeditsymbol.CorrectionValue, frmeditsymbol.Description, (uint)GetSymbolAddressSRAM(m_symbols, frmeditsymbol.Varname), true);
            }
        }

        private void btnRemoveSymbolFromList_Click(object sender, EventArgs e)
        {
            // delete a symbol
            //TODO: add delete key as well
            int[] selrows = ViewRealtime.GetSelectedRows();
            if (selrows.Length > 0)
            {
                foreach (int rowhandle in selrows)
                {
                    ViewRealtime.DeleteRow(rowhandle);
                }
            }
        }

        private void EditSelectedSymbol()
        {
            //TODO: edit symbol parameters
            int[] selrows = ViewRealtime.GetSelectedRows();
            if (selrows.Length == 1)
            {
                DataRow dr = ViewRealtime.GetDataRow(Convert.ToInt32(selrows.GetValue(0)));
                frmEditRealtimeSymbol frmeditsymbol = new frmEditRealtimeSymbol();
                frmeditsymbol.Symbols = m_symbols;
                frmeditsymbol.Symbolname = dr["SymbolName"].ToString();
                frmeditsymbol.Varname = dr["SymbolName"].ToString();
                frmeditsymbol.Description = dr["Description"].ToString();
                frmeditsymbol.MinimumValue = Convert.ToDouble(dr["Minimum"]);
                frmeditsymbol.MaximumValue = Convert.ToDouble(dr["Maximum"]);
                frmeditsymbol.OffsetValue = Convert.ToDouble(dr["Offset"]);
                frmeditsymbol.CorrectionValue = Convert.ToDouble(dr["Correction"]);

                if (frmeditsymbol.ShowDialog() == DialogResult.OK)
                {
                    // UPDATE the edited symbol
                    AddSymbolToRealTimeList(frmeditsymbol.Varname, frmeditsymbol.Symbolnumber, frmeditsymbol.MinimumValue, frmeditsymbol.MaximumValue, frmeditsymbol.OffsetValue, frmeditsymbol.CorrectionValue, frmeditsymbol.Description, (uint)GetSymbolAddressSRAM(m_symbols, frmeditsymbol.Varname), true);
                }
            }
        }

        private void btnEditSymbol_Click(object sender, EventArgs e)
        {
            EditSelectedSymbol();
        }

        private void xtraTabControl2_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            // fill realtime table with other values to be able to fill the dashboard
            if (e.Page == xtraTabPage1)
            {
                if (m_appSettings.ResetRealtimeSymbolOnTabPageSwitch)
                {
                    FillRealtimeTable(MonitorType.Dashboard);
                }
            }
            else
            {
                // remove the symbols again ??
            }
        }


        private void ViewRealtime_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedSymbol();
        }

        private void ViewRealtime_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == gcRealtimeValue.Name)
            {
                // get maximum and minumum  value
                try
                {

                    DataRow dr = ViewRealtime.GetDataRow(e.RowHandle);
                    if (dr != null && e.DisplayText != null)
                    {
                        /*if (dr["SYMBOLNAME"].ToString() == "Rpm")
                        {
                            Console.WriteLine("break");
                        }*/


                        double maximum = Convert.ToDouble(dr["Maximum"]);
                        double minimum = Convert.ToDouble(dr["Minimum"]);
                        double actualvalue = Convert.ToDouble(dr["Value"]);
                        double range = maximum - minimum;

                        double percentage = (actualvalue - minimum) / range;
                        if (percentage < 0) percentage = 0;
                        if (percentage > 1) percentage = 1;
                        double xwidth = percentage * (double)(e.Bounds.Width - 2);
                        if (xwidth > 0)
                        {
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height);
                            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.LightGreen, Color.OrangeRed, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            e.Graphics.FillRectangle(brush, e.Bounds.X + 1, e.Bounds.Y + 1, (float)xwidth, e.Bounds.Height - 2);
                        }
                        //percentage *= 100;
                        //e.DisplayText = percentage.ToString("F0") + @" %";
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }

        }

        private void ViewRealtime_KeyDown(object sender, KeyEventArgs e)
        {
            // control arrow UP = move UP
            // control arrow DOWN = move down
            if (e.Control)
            {
                if (e.KeyCode == Keys.Up)
                {
                    // move the selected row up
                }
                else if (e.KeyCode == Keys.Down)
                {
                    // move the selected row down
                }
            }
        }

        private void gridRealtime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnRemoveSymbolFromList_Click(this, EventArgs.Empty);
            }
            else if (e.Control)
            {
                if (e.KeyCode == Keys.Up)
                {
                    /***
                dt.Columns.Add("SymbolName");
                dt.Columns.Add("Description");
                dt.Columns.Add("Symbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("Value", Type.GetType("System.Double"));
                dt.Columns.Add("Offset", Type.GetType("System.Double"));
                dt.Columns.Add("Correction", Type.GetType("System.Double"));
                dt.Columns.Add("Peak", Type.GetType("System.Double"));
                dt.Columns.Add("Minimum", Type.GetType("System.Double"));
                dt.Columns.Add("Maximum", Type.GetType("System.Double"));
                dt.Columns.Add("ConvertedSymbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("SRAMAddress", Type.GetType("System.Int32"));
                dt.Columns.Add("Length", Type.GetType("System.Int32"));
                dt.Columns.Add("UserDefined", Type.GetType("System.Int32"));
                dt.Columns.Add("Delay", Type.GetType("System.Int32"));
                dt.Columns.Add("Reload", Type.GetType("System.Int32"));                     * */
                    // move current row up
                    if (ViewRealtime.FocusedRowHandle >= 1)
                    {
                        DataRow dr1 = ViewRealtime.GetDataRow(ViewRealtime.FocusedRowHandle);
                        DataRow dr2 = ViewRealtime.GetDataRow(ViewRealtime.FocusedRowHandle - 1);
                        //DataRow drtemp = ViewRealtime.GetDataRow(ViewRealtime.FocusedRowHandle - 1);
                        string descr = dr2["Description"].ToString();
                        string symbolname = dr2["SymbolName"].ToString();
                        int symbolnumber = Convert.ToInt32(dr2["Symbolnumber"]);
                        double value = Convert.ToDouble(dr2["Value"]);
                        double offset = Convert.ToDouble(dr2["Offset"]);
                        double correction = Convert.ToDouble(dr2["Correction"]);
                        double peak = Convert.ToDouble(dr2["Peak"]);
                        double minimum = Convert.ToDouble(dr2["Minimum"]);
                        double maximum = Convert.ToDouble(dr2["Maximum"]);
                        int convertedsymbolnumber = Convert.ToInt32(dr2["ConvertedSymbolnumber"]);
                        int sramaddress = Convert.ToInt32(dr2["SRAMAddress"]);
                        int Length = Convert.ToInt32(dr2["Length"]);
                        int UserDefined = Convert.ToInt32(dr2["UserDefined"]);
                        int Delay = Convert.ToInt32(dr2["Delay"]);
                        int Reload = Convert.ToInt32(dr2["Reload"]);

                        dr2["SymbolName"] = dr1["SymbolName"];
                        dr2["Description"] = dr1["Description"];
                        dr2["Symbolnumber"] = dr1["Symbolnumber"];
                        dr2["Value"] = dr1["Value"];
                        dr2["Offset"] = dr1["Offset"];
                        dr2["Correction"] = dr1["Correction"];
                        dr2["Peak"] = dr1["Peak"];
                        dr2["Minimum"] = dr1["Minimum"];
                        dr2["Maximum"] = dr1["Maximum"];
                        dr2["ConvertedSymbolnumber"] = dr1["ConvertedSymbolnumber"];
                        dr2["SRAMAddress"] = dr1["SRAMAddress"];
                        dr2["Length"] = dr1["Length"];
                        dr2["UserDefined"] = dr1["UserDefined"];
                        dr2["Delay"] = dr1["Delay"];
                        dr2["Reload"] = dr1["Reload"];

                        dr1["SymbolName"] = symbolname;
                        dr1["Description"] = descr;
                        dr1["Symbolnumber"] = symbolnumber;
                        dr1["Value"] = value;
                        dr1["Offset"] = offset;
                        dr1["Correction"] = correction;
                        dr1["Peak"] = peak;
                        dr1["Minimum"] = minimum;
                        dr1["Maximum"] = maximum;
                        dr1["ConvertedSymbolnumber"] = convertedsymbolnumber;
                        dr1["SRAMAddress"] = sramaddress;
                        dr1["Length"] = Length;
                        dr1["UserDefined"] = UserDefined;
                        dr1["Delay"] = Delay;
                        dr1["Reload"] = Reload;

                    }

                }
                else if (e.KeyCode == Keys.Down)
                {
                    if (ViewRealtime.FocusedRowHandle < ViewRealtime.RowCount - 1)
                    {
                        DataRow dr1 = ViewRealtime.GetDataRow(ViewRealtime.FocusedRowHandle);
                        DataRow dr2 = ViewRealtime.GetDataRow(ViewRealtime.FocusedRowHandle + 1);

                        string descr = dr2["Description"].ToString();
                        string symbolname = dr2["SymbolName"].ToString();
                        int symbolnumber = Convert.ToInt32(dr2["Symbolnumber"]);
                        double value = Convert.ToDouble(dr2["Value"]);
                        double offset = Convert.ToDouble(dr2["Offset"]);
                        double correction = Convert.ToDouble(dr2["Correction"]);
                        double peak = Convert.ToDouble(dr2["Peak"]);
                        double minimum = Convert.ToDouble(dr2["Minimum"]);
                        double maximum = Convert.ToDouble(dr2["Maximum"]);
                        int convertedsymbolnumber = Convert.ToInt32(dr2["ConvertedSymbolnumber"]);
                        int sramaddress = Convert.ToInt32(dr2["SRAMAddress"]);
                        int Length = Convert.ToInt32(dr2["Length"]);
                        int UserDefined = Convert.ToInt32(dr2["UserDefined"]);
                        int Delay = Convert.ToInt32(dr2["Delay"]);
                        int Reload = Convert.ToInt32(dr2["Reload"]);

                        dr2["SymbolName"] = dr1["SymbolName"];
                        dr2["Description"] = dr1["Description"];
                        dr2["Symbolnumber"] = dr1["Symbolnumber"];
                        dr2["Value"] = dr1["Value"];
                        dr2["Offset"] = dr1["Offset"];
                        dr2["Correction"] = dr1["Correction"];
                        dr2["Peak"] = dr1["Peak"];
                        dr2["Minimum"] = dr1["Minimum"];
                        dr2["Maximum"] = dr1["Maximum"];
                        dr2["ConvertedSymbolnumber"] = dr1["ConvertedSymbolnumber"];
                        dr2["SRAMAddress"] = dr1["SRAMAddress"];
                        dr2["Length"] = dr1["Length"];
                        dr2["UserDefined"] = dr1["UserDefined"];
                        dr2["Delay"] = dr1["Delay"];
                        dr2["Reload"] = dr1["Reload"];

                        dr1["SymbolName"] = symbolname;
                        dr1["Description"] = descr;
                        dr1["Symbolnumber"] = symbolnumber;
                        dr1["Value"] = value;
                        dr1["Offset"] = offset;
                        dr1["Correction"] = correction;
                        dr1["Peak"] = peak;
                        dr1["Minimum"] = minimum;
                        dr1["Maximum"] = maximum;
                        dr1["ConvertedSymbolnumber"] = convertedsymbolnumber;
                        dr1["SRAMAddress"] = sramaddress;
                        dr1["Length"] = Length;
                        dr1["UserDefined"] = UserDefined;
                        dr1["Delay"] = Delay;
                        dr1["Reload"] = Reload;

                    }
                }
            }
        }

        private void AddToRealtimeTable(System.Data.DataTable dt, string varname, string description, int symbolnumber, double value, double offset, double correction, double peak, double minimum, double maximum, int convertedSymbolnumber, uint sramaddress, int length, int delay)
        {
            bool fnd = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SymbolName"] != DBNull.Value)
                {
                    if (dr["SymbolName"].ToString() == varname)
                    {
                        fnd = true;
                        dr["Symbolnumber"] = symbolnumber;
                        dr["ConvertedSymbolnumber"] = symbolnumber;
                        dr["Peak"] = peak;
                        dr["SRAMAddress"] = sramaddress;
                        dr["Length"] = length;
                        dr["UserDefined"] = 0;
                        dr["Delay"] = delay;
                        dr["Reload"] = delay;
                        if (symbolnumber == 0 && sramaddress == 0)
                        {
                            dr["Value"] = value;
                            try
                            {
                                if (Convert.ToDouble(dr["Peak"]) < value) dr["Peak"] = value;
                            }
                            catch (Exception peakE)
                            {
                                Console.WriteLine("Failed to set peak: " + peakE.Message);
                            }
                        }
                        break;
                    }
                }
            }
            if (!fnd)
            {
                dt.Rows.Add(varname, description, symbolnumber, value, offset, correction, peak, minimum, maximum, convertedSymbolnumber, sramaddress, length, 0, delay, delay);
            }
        }

        private void FillRealtimeTable(MonitorType type)
        {
            if (gridRealtime.DataSource == null)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.TableName = "RTSymbols";
                dt.Columns.Add("SymbolName");
                dt.Columns.Add("Description");
                dt.Columns.Add("Symbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("Value", Type.GetType("System.Double"));
                dt.Columns.Add("Offset", Type.GetType("System.Double"));
                dt.Columns.Add("Correction", Type.GetType("System.Double"));
                dt.Columns.Add("Peak", Type.GetType("System.Double"));
                dt.Columns.Add("Minimum", Type.GetType("System.Double"));
                dt.Columns.Add("Maximum", Type.GetType("System.Double"));
                dt.Columns.Add("ConvertedSymbolnumber", Type.GetType("System.Int32"));
                dt.Columns.Add("SRAMAddress", Type.GetType("System.Int32"));
                dt.Columns.Add("Length", Type.GetType("System.Int32"));
                dt.Columns.Add("UserDefined", Type.GetType("System.Int32"));
                dt.Columns.Add("Delay", Type.GetType("System.Int32"));
                dt.Columns.Add("Reload", Type.GetType("System.Int32"));
                gridRealtime.DataSource = dt;
            }
            // fill values
            try
            {
                System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;

                if (SymbolExists("ActualIn.U_Battery")) AddToRealtimeTable(dt, "ActualIn.U_Battery", "Battery voltage", GetSymbolNumber(m_symbols, "ActualIn.U_Battery"), 0, 0, 0.1, 0, 0, 16, GetSymbolNumber(m_symbols, "ActualIn.U_Battery"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.U_Battery"), GetSymbolLength(m_symbols, "ActualIn.U_Battery"), 1);


                // rpm should always be present
                if (SymbolExists("ActualIn.n_Engine")) AddToRealtimeTable(dt, "ActualIn.n_Engine", "Engine speed", GetSymbolNumber(m_symbols, "ActualIn.n_Engine"), 0, 0, 1, 0, 0, 8000, GetSymbolNumber(m_symbols, "ActualIn.n_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.n_Engine"), GetSymbolLength(m_symbols, "ActualIn.n_Engine"), 1);
                if (SymbolExists("In.v_Vehicle")) AddToRealtimeTable(dt, "In.v_Vehicle", "Vehicle speed", GetSymbolNumber(m_symbols, "In.v_Vehicle"), 0, 0, 0.1, 0, 0, 300, GetSymbolNumber(m_symbols, "In.v_Vehicle"), (uint)GetSymbolAddressSRAM(m_symbols, "In.v_Vehicle"), GetSymbolLength(m_symbols, "In.v_Vehicle"), 3);
                if (SymbolExists("Out.X_AccPos")) AddToRealtimeTable(dt, "Out.X_AccPos", "TPS %", GetSymbolNumber(m_symbols, "Out.X_AccPos"), 0, 0, 0.1, 0, 0, 100, GetSymbolNumber(m_symbols, "Out.X_AccPos"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.X_AccPos"), GetSymbolLength(m_symbols, "Out.X_AccPos"), 1);
                if (SymbolExists("ActualIn.T_Engine")) AddToRealtimeTable(dt, "ActualIn.T_Engine", "Engine temperature", GetSymbolNumber(m_symbols, "ActualIn.T_Engine"), 0, 0, 1, 0, -20, 120, GetSymbolNumber(m_symbols, "ActualIn.T_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.T_Engine"), GetSymbolLength(m_symbols, "ActualIn.T_Engine"), 5);
                if (SymbolExists("ActualIn.T_AirInlet")) AddToRealtimeTable(dt, "ActualIn.T_AirInlet", "Intake air temperature", GetSymbolNumber(m_symbols, "ActualIn.T_AirInlet"), 0, 0, 1, 0, -20, 120, GetSymbolNumber(m_symbols, "ActualIn.T_AirInlet"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.T_AirInlet"), GetSymbolLength(m_symbols, "ActualIn.T_AirInlet"), 3);
                if (SymbolExists("ECMStat.ST_ActiveAirDem")) AddToRealtimeTable(dt, "ECMStat.ST_ActiveAirDem", "Active air demand map", GetSymbolNumber(m_symbols, "ECMStat.ST_ActiveAirDem"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "ECMStat.ST_ActiveAirDem"), (uint)GetSymbolAddressSRAM(m_symbols, "ECMStat.ST_ActiveAirDem"), GetSymbolLength(m_symbols, "ECMStat.ST_ActiveAirDem"), 1);
                if (SymbolExists("Lambda.Status")) AddToRealtimeTable(dt, "Lambda.Status", "Lambda status", GetSymbolNumber(m_symbols, "Lambda.Status"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "Lambda.Status"), (uint)GetSymbolAddressSRAM(m_symbols, "Lambda.Status"), GetSymbolLength(m_symbols, "Lambda.Status"), 1);
                if (SymbolExists("FCut.CutStatus")) AddToRealtimeTable(dt, "FCut.CutStatus", "Fuelcut status", GetSymbolNumber(m_symbols, "FCut.CutStatus"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "FCut.CutStatus"), (uint)GetSymbolAddressSRAM(m_symbols, "FCut.CutStatus"), GetSymbolLength(m_symbols, "FCut.CutStatus"), 1);
                if (SymbolExists("IgnMastProt.fi_Offset")) AddToRealtimeTable(dt, "IgnMastProt.fi_Offset", "Ignition offset", GetSymbolNumber(m_symbols, "IgnMastProt.fi_Offset"), 0, 0, 0.1, 0, -20, 20, GetSymbolNumber(m_symbols, "IgnMastProt.fi_Offset"), (uint)GetSymbolAddressSRAM(m_symbols, "IgnMastProt.fi_Offset"), GetSymbolLength(m_symbols, "IgnMastProt.fi_Offset"), 1);
                if (SymbolExists("AirMassMast.m_Request")) AddToRealtimeTable(dt, "AirMassMast.m_Request", "Requested airmass", GetSymbolNumber(m_symbols, "AirMassMast.m_Request"), 0, 0, 1, 0, 0, 600, GetSymbolNumber(m_symbols, "AirMassMast.m_Request"), (uint)GetSymbolAddressSRAM(m_symbols, "AirMassMast.m_Request"), GetSymbolLength(m_symbols, "AirMassMast.m_Request"), 1);
                if (SymbolExists("Out.M_EngTrqAct")) AddToRealtimeTable(dt, "Out.M_EngTrqAct", "Calculated torque", GetSymbolNumber(m_symbols, "Out.M_EngTrqAct"), 0, 0, 1, 0, 0, 600, GetSymbolNumber(m_symbols, "Out.M_EngTrqAct"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.M_EngTrqAct"), GetSymbolLength(m_symbols, "Out.M_EngTrqAct"), 1);

                //<GS-05042011> P_Engine is not needed, we can calculate that from torque and RPM
                //if (SymbolExists("ECMStat.P_Engine")) AddToRealtimeTable(dt, "ECMStat.P_Engine", "Calculated power", GetSymbolNumber(m_symbols, "ECMStat.P_Engine"), 0, 0, 1, 0, 0, 600, GetSymbolNumber(m_symbols, "ECMStat.P_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "ECMStat.P_Engine"), GetSymbolLength(m_symbols, "ECMStat.P_Engine"), 1);

                //if (SymbolExists("ECMStat.p_Diff")) AddToRealtimeTable(dt, "ECMStat.p_Diff", "Boost", GetSymbolNumber(m_symbols, "ECMStat.p_Diff"), 0, 0, 0.01, 0, -1, 3, GetSymbolNumber(m_symbols, "ECMStat.p_Diff"), (uint)GetSymbolAddressSRAM(m_symbols, "ECMStat.p_Diff"), GetSymbolLength(m_symbols, "ECMStat.p_Diff"));
                if (SymbolExists("In.p_AirInlet")) AddToRealtimeTable(dt, "In.p_AirInlet", "Boost", GetSymbolNumber(m_symbols, "In.p_AirInlet"), 0, -1, 0.001, 0, -1, 3, GetSymbolNumber(m_symbols, "In.p_AirInlet"), (uint)GetSymbolAddressSRAM(m_symbols, "In.p_AirInlet"), GetSymbolLength(m_symbols, "In.p_AirInlet"), 1);
                if (SymbolExists("Out.PWM_BoostCntrl")) AddToRealtimeTable(dt, "Out.PWM_BoostCntrl", "Duty cycle BCV", GetSymbolNumber(m_symbols, "Out.PWM_BoostCntrl"), 0, 0, 0.1, 0, 0, 100, GetSymbolNumber(m_symbols, "Out.PWM_BoostCntrl"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.PWM_BoostCntrl"), GetSymbolLength(m_symbols, "Out.PWM_BoostCntrl"), 1);
                if (SymbolExists("Out.fi_Ignition")) AddToRealtimeTable(dt, "Out.fi_Ignition", "Ignition advance", GetSymbolNumber(m_symbols, "Out.fi_Ignition"), 0, 0, 0.1, 0, -10, 50, GetSymbolNumber(m_symbols, "Out.fi_Ignition"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.fi_Ignition"), GetSymbolLength(m_symbols, "Out.fi_Ignition"), 1);
                if (SymbolExists("MAF.m_AirInlet")) AddToRealtimeTable(dt, "MAF.m_AirInlet", "Actual airmass", GetSymbolNumber(m_symbols, "MAF.m_AirInlet"), 0, 0, 1, 0, 0, 1600, GetSymbolNumber(m_symbols, "MAF.m_AirInlet"), (uint)GetSymbolAddressSRAM(m_symbols, "MAF.m_AirInlet"), GetSymbolLength(m_symbols, "MAF.m_AirInlet"), 1);

                //<GS-05042011> Only add Exhaust.T_Calc when this function is active in the opened binary file
                if (HasExhaustGasTemperatureCalculation())
                {
                    if (SymbolExists("Exhaust.T_Calc")) AddToRealtimeTable(dt, "Exhaust.T_Calc", "Calculated EGT temperature", GetSymbolNumber(m_symbols, "Exhaust.T_Calc"), 0, 0, 1, 0, 0, 1200, GetSymbolNumber(m_symbols, "Exhaust.T_Calc"), (uint)GetSymbolAddressSRAM(m_symbols, "Exhaust.T_Calc"), GetSymbolLength(m_symbols, "Exhaust.T_Calc"), 2);
                    // enable the realtime symbol for exhaust gas temperature
                    measurementEGT.Enabled = true;
                    labelControl4.Enabled = true;
                }
                else
                {
                    // disable the realtime symbol for exhaust gas temperature
                    measurementEGT.Enabled = false;
                    labelControl4.Enabled = false;
                }
                //TODO: Fix for Trionic 8 
                //if (SymbolExists("BFuelProt.CurrentFuelCon")) AddToRealtimeTable(dt, "BFuelProt.CurrentFuelCon", "Fuel consumption", GetSymbolNumber(m_symbols, "BFuelProt.CurrentFuelCon"), 0, 0, 0.1, 0, 0, 50, GetSymbolNumber(m_symbols, "BFuelProt.CurrentFuelCon"), (uint)GetSymbolAddressSRAM(m_symbols, "BFuelProt.CurrentFuelCon"), GetSymbolLength(m_symbols, "BFuelProt.CurrentFuelCon"), 2);
                // only if use wideband selected
                if (m_appSettings.UseWidebandLambda)
                {

                    if (SymbolExists(m_appSettings.WideBandSymbol))
                    {
                        double corr = 0.1;
                        if (m_appSettings.WideBandSymbol == "DisplProt.AD_Scanner") corr = 1;
                        AddToRealtimeTable(dt, m_appSettings.WideBandSymbol, "Lambda value (wbO2)", GetSymbolNumber(m_symbols, m_appSettings.WideBandSymbol), 0, 0, corr, 0, 10, 20, GetSymbolNumber(m_symbols, m_appSettings.WideBandSymbol), (uint)GetSymbolAddressSRAM(m_symbols, m_appSettings.WideBandSymbol), GetSymbolLength(m_symbols, m_appSettings.WideBandSymbol), 1);
                        // AFR feedback map initialiseren // <GS-19042010>
                        int _width = 18;
                        int _height = 16;
                    }
                }
                else
                {
                    if (SymbolExists("Lambda.LambdaInt")) AddToRealtimeTable(dt, "Lambda.LambdaInt", "Lambda value (nbO2)", GetSymbolNumber(m_symbols, "Lambda.LambdaInt"), 0, 1, 0.0001, 0, 0, 2, GetSymbolNumber(m_symbols, "Lambda.LambdaInt"), (uint)GetSymbolAddressSRAM(m_symbols, "Lambda.LambdaInt"), GetSymbolLength(m_symbols, "Lambda.LambdaInt"), 1);
                }

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private bool HasExhaustGasTemperatureCalculation()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "ExhaustCal.ST_Enable" || sh.Userdescription == "ExhaustCal.ST_Enable")
                {
                    byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) != 0x00)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool m_connectedToECU = false;
        private bool m_prohibitReading = false;
        private bool _soundAllowed = true;
        private System.Media.SoundPlayer sndplayer;



        private void tmrRealtime_Tick(object sender, EventArgs e)
        {
            tmrRealtime.Enabled = false;
            try
            {
                if (m_connectedToECU)
                {
                    if (!m_prohibitReading)
                    {
                        t8can.StallKeepAlive = true;
                        GetSRAMVarsFromTable();
                        // t8can.StallKeepAlive = false;
                        // keep alive every time? ... ?
                    }
                    else
                    {
                        Console.WriteLine("Not reading from SRAM because we're doing something else");
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to run realtime timer code: " + E.Message);
            }
            tmrRealtime.Enabled = true;
        }

        //TODO: Adjust for Trionic 8
        private void GetSRAMVarsFromTable()
        {
            //Console.WriteLine("GetSRAMVarsFromTable started");
            if (gridRealtime.DataSource != null)
            {
                _sw.Reset();
                _sw.Start();
                System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                //Console.WriteLine("Fetch the datatable: " + dt.Rows.Count.ToString() + " rows");
                foreach (DataRow dr in dt.Rows)
                {

                    double value = 0;
                    if (m_prohibitReading) return;
                    try
                    {
                        if (dr["Delay"] != DBNull.Value)
                        {
                            int dly = Convert.ToInt32(dr["Delay"]);
                            dly--;
                            if (dly > 0)
                            {
                                dr["Delay"] = dly;
                                continue;
                            }
                            else
                            {
                                dr["Delay"] = dr["Reload"];
                            }
                        }
                        else
                        {
                            Console.WriteLine("Delay value was null for : " + dr["SymbolName"].ToString());
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to reload: " + E.Message);
                    }
                    //int symbolnumber = Convert.ToInt32(dr["ConvertedSymbolnumber"]);
                    string symbolName = dr["SymbolName"].ToString();
                    //Console.WriteLine("Start reading " + symbolName + " at address: " + Convert.ToInt32(dr["SRAMAddress"]).ToString("X8"));
                    //if (symbolnumber > 0)
                    {
                        byte[] buffer = new byte[1];
                        
                        bool _success = false;
                        //buffer = ReadSymbolFromSRAM((uint)symbolnumber, symbolName, Convert.ToUInt32(dr["SRAMAddress"]), Convert.ToInt32(dr["Length"]), out _success);
                        if (Convert.ToInt32(dr["SRAMAddress"]) > 0)
                        {
                            buffer = t8can.readMemory(Convert.ToInt32(dr["SRAMAddress"]), Convert.ToInt32(dr["Length"]), out _success);
                        }
                        else
                        {
                            Console.WriteLine("Symbol: " + symbolName + " has address 0");
                        }

                        /* string dbg = string.Empty;
                         for (int i = 0; i < buffer.Length; i++)
                         {
                             dbg += buffer[i].ToString("X2") + " ";
                         }
                         Console.WriteLine(symbolName + " = " + dbg + "buflen: " + buffer.Length.ToString() + " " + _success.ToString());
                         */
                        if (_success)
                        {


                            if (buffer.Length == 1)
                            {
                                //Console.WriteLine("Buffer received: " + buffer[0].ToString("X2"));
                                value = Convert.ToInt32(buffer.GetValue(0));
                            }
                            else if (buffer.Length == 2)
                            {
                                value = (Convert.ToInt32(buffer.GetValue(0)) * 256) + Convert.ToInt32(buffer.GetValue(1));
                            }
                            else if (buffer.Length == 4)
                            {
                                value = (Convert.ToInt32(buffer.GetValue(0)) * 256 * 256 * 256) + (Convert.ToInt32(buffer.GetValue(1)) * 256 * 256) + (Convert.ToInt32(buffer.GetValue(2)) * 256) + Convert.ToInt32(buffer.GetValue(3));
                            }

                            //double realvalue = (double)value;
                            if (symbolName == "ActualIn.T_Engine" ||
                                symbolName == "ActualIn.T_AirInlet" ||
                                symbolName == "Out.fi_Ignition" ||
                                symbolName == "Out.M_EngTrqAct" ||
                                symbolName == "ECMStat.P_Engine" ||
                                symbolName == "IgnMastProt.fi_Offset" ||
                                symbolName == "Lambda.LambdaInt" ||
                                symbolName == "MAF.m_AirInlet")
                            {
                                if (value > 32000) value = -(65536 - value); // negatief maken
                            }

                            else if (symbolName == "KnkDet.KnockCyl" || symbolName == "KnkDetAdap.KnkCntCyl")         // 4 length
                            {
                                int knkcountcyl1 = 0;
                                int knkcountcyl2 = 0;
                                int knkcountcyl3 = 0;
                                int knkcountcyl4 = 0;
                                if (buffer.Length == 4)
                                {
                                    knkcountcyl1 = Convert.ToInt32(buffer.GetValue(0));
                                    knkcountcyl2 = Convert.ToInt32(buffer.GetValue(1));
                                    knkcountcyl3 = Convert.ToInt32(buffer.GetValue(2));
                                    knkcountcyl4 = Convert.ToInt32(buffer.GetValue(3));
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl1", (float)knkcountcyl1);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl2", (float)knkcountcyl2);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl3", (float)knkcountcyl3);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl4", (float)knkcountcyl4);
                                }

                            }
                            // 
                            else if (symbolName == "MisfAdap.N_MisfCountCyl")         // 8 length
                            {
                                int miscountcyl1 = 0;
                                int miscountcyl2 = 0;
                                int miscountcyl3 = 0;
                                int miscountcyl4 = 0;
                                if (buffer.Length == 8)
                                {
                                    miscountcyl1 = (Convert.ToInt32(buffer.GetValue(0)) * 256) + Convert.ToInt32(buffer.GetValue(1));
                                    miscountcyl2 = (Convert.ToInt32(buffer.GetValue(2)) * 256) + Convert.ToInt32(buffer.GetValue(3));
                                    miscountcyl3 = (Convert.ToInt32(buffer.GetValue(4)) * 256) + Convert.ToInt32(buffer.GetValue(5));
                                    miscountcyl4 = (Convert.ToInt32(buffer.GetValue(6)) * 256) + Convert.ToInt32(buffer.GetValue(7));
                                    UpdateRealtimeInformationInTable(dt, "MisfCyl1", (float)miscountcyl1);
                                    UpdateRealtimeInformationInTable(dt, "MisfCyl2", (float)miscountcyl2);
                                    UpdateRealtimeInformationInTable(dt, "MisfCyl3", (float)miscountcyl3);
                                    UpdateRealtimeInformationInTable(dt, "MisfCyl4", (float)miscountcyl4);
                                }

                            }
                            value *= Convert.ToDouble(dr["Correction"]);
                            value += Convert.ToDouble(dr["Offset"]);
                            dr["Value"] = value;

                            try
                            {
                                if (Convert.ToDouble(dr["Peak"]) < value) dr["Peak"] = value;
                            }
                            catch (Exception peakE)
                            {
                                Console.WriteLine("Failed to set peak: " + peakE.Message);
                            }
                            // update realtime info
                            UpdateRealtimeInformation(symbolName, (float)value);
                            try
                            {
                                CheckSoundsToPlay(symbolName, value);
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to read SRAM");
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    Thread.Sleep(0);//<GS-11022010>

                }
                // <GS-29072010> if the multiadapter is in use 
                // and the user configured to use ADCs or thermoinput, get the values
                if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    if (m_appSettings.Useadc1)
                    {
                        float adc = t8can.GetADCValue(0);
                        double convertedADvalue = ConvertADCValue(0, adc);
                        string channelName = m_appSettings.Adc1channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 1", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1highvalue, m_appSettings.Adc1lowvalue, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc2)
                    {
                        float adc = t8can.GetADCValue(1);
                        double convertedADvalue = ConvertADCValue(1, adc);
                        string channelName = m_appSettings.Adc2channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 2", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1highvalue, m_appSettings.Adc1lowvalue, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc3)
                    {
                        float adc = t8can.GetADCValue(2);
                        double convertedADvalue = ConvertADCValue(2, adc);
                        string channelName = m_appSettings.Adc3channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 3", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1highvalue, m_appSettings.Adc1lowvalue, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc4)
                    {
                        float adc = t8can.GetADCValue(3);
                        double convertedADvalue = ConvertADCValue(3, adc);
                        string channelName = m_appSettings.Adc4channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 4", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1highvalue, m_appSettings.Adc1lowvalue, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc5)
                    {
                        float adc = t8can.GetADCValue(4);
                        double convertedADvalue = ConvertADCValue(4, adc);
                        string channelName = m_appSettings.Adc5channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 5", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1highvalue, m_appSettings.Adc1lowvalue, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Usethermo)
                    {
                        float temperature = t8can.GetThermoValue();
                        string channelName = m_appSettings.Thermochannelname;
                        AddToRealtimeTable(dt, channelName, "Thermo channel", 0, temperature, 0, 1, 0, 0, 1023, 0, 0, 0, 1);
                    }
                }

                //Console.WriteLine("Updated in " + _sw.ElapsedMilliseconds.ToString() + " ms");
                LogRealTimeInformation(dt);
                //UpdateOpenViewers();
                //<GS-06012011> maybe move the fps counter timer here!
                _sw.Stop();
                // update fps indicator
                float secs = _sw.ElapsedMilliseconds / 1000F;
                secs = 1 / secs;
                if (float.IsInfinity(secs)) secs = 1;
                UpdateRealtimeInformation("FPSCounter", secs);
            }
        }

        private void UpdateOpenViewers()
        {
            UpdateInjectionMap();
            UpdateIgnitionMap();
            UpdateAirmassMap();
        }

        /// <summary>
        /// TODO: Has to check for negative values (FFFFFFFD for exmpl)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="symbolname"></param>
        /// <param name="multiplywith"></param>
        /// <returns></returns>
        private int LookUpIndexAxisRPMMap(double value, string symbolname, double multiplywith)
        {
            int return_index = -1;
            double min_difference = 10000000;

            byte[] axisvalues = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, symbolname), GetSymbolLength(m_symbols, symbolname));
            if (isSixteenBitTable(symbolname))
            {
                for (int t = 0; t < axisvalues.Length; t += 2)
                {
                    int b = (int)(byte)axisvalues.GetValue(t) * 256;
                    b += (int)(byte)axisvalues.GetValue(t + 1);
                    if (b > 32000) b = -(65536 - b); // negatief maken
                    double db = (double)b;
                    db *= multiplywith;
                    double diff = Math.Abs(db - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t / 2;
                        // Console.WriteLine("Difference was: " + diff.ToString() + " at index " + return_index.ToString());

                    }
                }
            }
            else
            {
                for (int t = 0; t < axisvalues.Length; t++)
                {
                    int b = (int)(byte)axisvalues.GetValue(t);
                    double db = (double)b;
                    db *= multiplywith;
                    double diff = Math.Abs(db - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t;
                    }
                }
            }
            return return_index;
        }

        //TODO: adjust for Trionic 8
        private void UpdateAirmassMap()
        {
            int airmassindex = 0;
            int torqueindex = 0;
            int rpmindex = 0;
            int tpsindex = 0;
            //PedalMapCal.m_RequestMap has 
            // X: PedalMapCal.n_EngineMap (rpm)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "PedalMapCal.n_EngineMap", 1);
            // Y: PedalMapCal.X_PedalMap (tps)
            tpsindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentThrottlePosition, "PedalMapCal.X_PedalMap", 0.1);
            UpdateDocksWithName("PedalMapCal.Trq_RequestMap", rpmindex, tpsindex);

            //TorqueCal.m_AirTorqMap has
            // X: TorqueCal.M_EngXSP
            torqueindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentEngineTorque, "TrqMastCal.Trq_EngXSP", 1);
            // Y: TorqueCal.n_EngYSP
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "TrqMastCal.n_EngYSP", 1);
            UpdateDocksWithName("TrqMastCal.m_AirTorqMap", torqueindex, rpmindex);

            //TorqueCal.M_NominalMap has
            //X: TorqueCal.m_AirXSP
            //Y: TorqueCal.n_EngYSP // same, so leave that out
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "TrqMastCal.m_AirXSP", 1);
            UpdateDocksWithName("TrqMastCal.Trq_NominalMap", airmassindex, rpmindex);

            //BoostCal.RegMap has:
            //X: BoostCal.SetLoadXSP (airmass)
            //Y: BoostCal.n_EngSP (rpm)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "AirCrtlCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "AirCrtlCal.SetLoadXSP", 1);
            UpdateDocksWithName("AirCrtlCal.RegMap", airmassindex, rpmindex);

            //BstKnkCal.MaxAirmass
            //BstKnkCal.MaxAirmassAu
            //X: BstKnkCal.OffsetXSP // special because all negative
            //Y: BstKnkCal.n_EngYSP
            string xaxisstr = "BstKnkCal.OffsetXSP";
            if (!SymbolExists(xaxisstr)) xaxisstr = "BstKnkCal.fi_offsetXSP";
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BstKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentIgnitionOffset, xaxisstr, 0.1);
            UpdateDocksWithName("BstKnkCal.MaxAirmass", airmassindex, rpmindex);
            UpdateDocksWithName("BstKnkCal.MaxAirmassAu", airmassindex, rpmindex);
        }

        private void UpdateDocksWithName(string symbolname, int xindex, int yindex)
        {
            try
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is IMapViewer)
                        {
                            IMapViewer vwr = (IMapViewer)c;
                            if (vwr.Map_name.StartsWith(symbolname))
                            {
                                vwr.HighlightCell(xindex, yindex);
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is IMapViewer)
                                {
                                    IMapViewer vwr2 = (IMapViewer)c2;
                                    if (vwr2.Map_name.StartsWith(symbolname))
                                    {
                                        vwr2.HighlightCell(xindex, yindex);
                                    }

                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is IMapViewer)
                                {
                                    IMapViewer vwr3 = (IMapViewer)c3;
                                    if (vwr3.Map_name.StartsWith(symbolname))
                                    {
                                        vwr3.HighlightCell(xindex, yindex);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        //TODO: adjust for Trionic 8
        private void UpdateIgnitionMap()
        {

            int airmassindex = 0;
            int rpmindex = 0;
            // IgnNormCal.Map has
            //X: IgnNormCal.m_AirXSP // airmass
            //Y: IgnNormCal.n_EngYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnAbsCal.n_EngNormYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnAbsCal.m_AirNormXSP", 1);
            UpdateDocksWithName("IgnAbsCal.fi_NormalMAP", airmassindex, rpmindex);
            UpdateDocksWithName("IgnAbsCal.fi_lowOctanMAP", airmassindex, rpmindex);
            UpdateDocksWithName("IgnAbsCal.fi_highOctanMAP", airmassindex, rpmindex);
            //IgnE85Cal.fi_AbsMap has:
            // same, leave it
            //UpdateDocksWithName("IgnE85Cal.fi_AbsMap", airmassindex, rpmindex);
            //KnkFuelCal.fi_MapMaxOff has
            //X: KnkFuelCal.m_AirXSP //airmass
            //Y: BstKnkCal.n_EngYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BstKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "KnkFuelCal.m_AirXSP", 1);
            UpdateDocksWithName("KnkFuelCal.fi_MaxOffsetMap", airmassindex, rpmindex);
            //IgnKnkCal.IndexMap has
            // X: IgnKnkCal.m_AirXSP (airmass)
            // Y: IgnKnkCal.n_EngYSP (engine speed)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnKnkCal.m_AirXSP", 1);
            UpdateDocksWithName("IgnKnkCal.IndexMap", airmassindex, rpmindex);
            //KnkDetCal.RefFactorMap
            // X: KnkDetCal.m_AirXSP (airmass)
            // Y: KnkDetCal.n_EngYSP (engine speed)
            //rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "KnkDetCal.n_EngYSP", 1);
            //airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "KnkDetCal.m_AirXSP", 1);
            //UpdateDocksWithName("KnkDetCal.RefFactorMap", airmassindex, rpmindex);

        }

        //TODO: adjust for Trionic 8
        private void UpdateInjectionMap()
        {
            int airmassindex = 0;
            int rpmindex = 0;

            //BFuelCal.Map has
            //X: BFuelCal.AirXSP (airmass)
            //Y: BFuelCal.RpmYSP (engine speed)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BFuelCal.RpmYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "BFuelCal.AirXSP", 1);
            UpdateDocksWithName("BFuelCal.LambdaOneFacMap", airmassindex, rpmindex);
            //BFuelCal.StartMap has
            //X: BFuelCal.AirXSP (airmass)
            //Y: BFuelCal.RpmYSP (engine speed)
            // uses the same as BFuelCal.Map, so leave it
            //UpdateDocksWithName("BFuelCal.StartMap", airmassindex, rpmindex);
            //KnkFuelCal.EnrichmentMap has
            //X: IgnKnkCal.m_AirXSP // airmass
            //Y: IgnKnkCal.n_EngYSP // engine speed
            //rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnKnkCal.n_EngYSP", 1);
            //airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnKnkCal.m_AirXSP", 1);
            //UpdateDocksWithName("KnkFuelCal.EnrichmentMap", airmassindex, rpmindex);
        }

        private void LogRealTimeInformation(System.Data.DataTable dt)
        {
            if (dt == null) return;
            if (m_currentfile == "") return;
            DateTime datet = DateTime.Now;
            string logline = datet.ToString("dd/MM/yyyy HH:mm:ss") + "." + datet.Millisecond.ToString("D3") + "|";
            //            string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    //if (Convert.ToInt32(dr["Symbolnumber"]) > 0)
                    //{
                    logline += dr["SymbolName"].ToString() + "=" + Convert.ToDouble(dr["Value"]).ToString() + "|";
                    //}
                }
                catch (Exception E)
                {
                    Console.WriteLine("Failed to log to file: " + E.Message);
                }
            }
            if (m_WriteLogMarker)
            {
                m_WriteLogMarker = false;
                logline += "IMPORTANTLINE=1|";
            }
            else
            {
                logline += "IMPORTANTLINE=0|";
            }
            if (!Directory.Exists(Path.GetDirectoryName(m_currentfile) + "\\Logs")) Directory.CreateDirectory(Path.GetDirectoryName(m_currentfile) + "\\Logs");

            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(m_currentfile) + "\\Logs\\" + DateTime.Now.ToString("yyyyMMdd") + "-CanTraceExt.t8l", true))
            {
                sw.WriteLine(logline);
            }
        }

        private void UpdateRealtimeInformation(string symbolname, float value)
        {
            if (!this.IsDisposed)
            {
                try
                {
                    this.Invoke(m_DelegateUpdateRealTimeValue, symbolname, value);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
            }

        }

        private void CheckSoundsToPlay(string symbolName, double value)
        {
            string _sound2Play = string.Empty;
            if (symbolName == m_appSettings.Notification1symbol && m_appSettings.Notification1Active)
            {
                // check bounds
                switch (m_appSettings.Notification1condition)
                {
                    case 0: // equal
                        if (value == m_appSettings.Notification1value) _sound2Play = m_appSettings.Notification1sound;
                        break;
                    case 1: // is greater than
                        if (value > m_appSettings.Notification1value) _sound2Play = m_appSettings.Notification1sound;
                        break;
                    case 2: // is smaller than
                        if (value < m_appSettings.Notification1value) _sound2Play = m_appSettings.Notification1sound;
                        break;
                }
            }
            if (symbolName == m_appSettings.Notification2symbol && m_appSettings.Notification2Active && _sound2Play == "")
            {
                // check bounds
                switch (m_appSettings.Notification2condition)
                {
                    case 0: // equal
                        if (value == m_appSettings.Notification2value) _sound2Play = m_appSettings.Notification2sound;
                        break;
                    case 1: // is greater than
                        if (value > m_appSettings.Notification2value) _sound2Play = m_appSettings.Notification2sound;
                        break;
                    case 2: // is smaller than
                        if (value < m_appSettings.Notification2value) _sound2Play = m_appSettings.Notification2sound;
                        break;
                }
            }
            if (symbolName == m_appSettings.Notification3symbol && m_appSettings.Notification3Active && _sound2Play == "")
            {
                // check bounds
                switch (m_appSettings.Notification3condition)
                {
                    case 0: // equal
                        if (value == m_appSettings.Notification3value) _sound2Play = m_appSettings.Notification3sound;
                        break;
                    case 1: // is greater than
                        if (value > m_appSettings.Notification3value) _sound2Play = m_appSettings.Notification3sound;
                        break;
                    case 2: // is smaller than
                        if (value < m_appSettings.Notification3value) _sound2Play = m_appSettings.Notification3sound;
                        break;
                }
            }
            if (_sound2Play != "")
            {
                // we need to play a sound, but not too many times in a row, we have to wait until a previous sound
                // has ended

                if (File.Exists(_sound2Play))
                {
                    try
                    {
                        if (sndplayer != null)
                        {
                            if (_soundAllowed)
                            {
                                _soundAllowed = false; // no more for 2 seconds (sndTimer)
                                sndplayer.SoundLocation = _sound2Play;
                                sndplayer.Play();
                                m_WriteLogMarker = true; // mark this in the logfile immediately
                                sndTimer.Enabled = true;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
            }

        }

        private double ConvertADCValue(int channel, float value)
        {
            double retval = value;
            double m_HighVoltage = 5;
            double m_LowVoltage = 0;
            double m_HighValue = 1;
            double m_LowValue = 0;
            switch (channel)
            {
                case 0:
                    m_HighVoltage = m_appSettings.Adc1highvoltage;
                    m_LowVoltage = m_appSettings.Adc1lowvoltage;
                    m_LowValue = m_appSettings.Adc1lowvalue;
                    m_HighValue = m_appSettings.Adc1highvalue;
                    break;
                case 1:
                    m_HighVoltage = m_appSettings.Adc2highvoltage;
                    m_LowVoltage = m_appSettings.Adc2lowvoltage;
                    m_LowValue = m_appSettings.Adc2lowvalue;
                    m_HighValue = m_appSettings.Adc2highvalue;
                    break;
                case 2:
                    m_HighVoltage = m_appSettings.Adc3highvoltage;
                    m_LowVoltage = m_appSettings.Adc3lowvoltage;
                    m_LowValue = m_appSettings.Adc3lowvalue;
                    m_HighValue = m_appSettings.Adc3highvalue;
                    break;
                case 3:
                    m_HighVoltage = m_appSettings.Adc4highvoltage;
                    m_LowVoltage = m_appSettings.Adc4lowvoltage;
                    m_LowValue = m_appSettings.Adc4lowvalue;
                    m_HighValue = m_appSettings.Adc4highvalue;
                    break;
                case 4:
                    m_HighVoltage = m_appSettings.Adc5highvoltage;
                    m_LowVoltage = m_appSettings.Adc5lowvoltage;
                    m_LowValue = m_appSettings.Adc5lowvalue;
                    m_HighValue = m_appSettings.Adc5highvalue;
                    break;
                default:
                    break;
            }
            // convert using the known math
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double voltage = value; // <GS-14042011> Combiadapter seems to generate voltage in stead of 0-255 values ((value) / 255) * (m_HighVoltage / 1000 - m_LowVoltage / 1000);
            //Console.WriteLine("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_LowVoltage / 1000) voltage = m_LowVoltage / 1000;
            if (voltage > m_HighVoltage / 1000) voltage = m_HighVoltage / 1000;
            //Console.WriteLine("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_HighValue / 1000) - (m_LowValue / 1000)) / ((m_HighVoltage / 1000) - (m_LowVoltage / 1000));
            //Console.WriteLine("Steepness: " + steepness.ToString());
            retval = (m_LowValue / 1000) + (steepness * (voltage - (m_LowVoltage / 1000)));
            //Console.WriteLine("retval: " + retval.ToString());
            return retval;

        }

        private void UpdateRealtimeInformationInTable(System.Data.DataTable dt, string symbolname, double value)
        {
            if (dt != null)
            {
                bool _symbolFound = false;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["SymbolName"].ToString() == symbolname)
                    {
                        dr["Value"] = value;
                        _symbolFound = true;
                        break;
                    }
                }
                if (!_symbolFound)
                {
                    dt.Rows.Add(symbolname, 0, value, 0, 1, 0, 0, 65535, 0, 0, 0, 1, 1, 1);
                }
            }
        }

        private void btnWriteLogMarker_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_WriteLogMarker = true;
        }

        private void sndTimer_Tick(object sender, EventArgs e)
        {
            sndTimer.Enabled = false;
            _soundAllowed = true; // re-allow the playback of sounds
        }

        private string ConvertFuelcutStatus(int value)
        {
            //Fuel cut status, indicates what caused the fuelcut: 
            //0 - No fuelcut 
            //1 - Ignition key turned off 
            //2 - Acc pedal pressed during start 
            //3 - Engine speed above limit 
            //4 - Throttle block adaption active for the first time 
            //5 - 6 - Airmass guard ( pressure guard ) 
            //7 - Immobilizer code not correct 
            //8 - Current to h-bridge to high during throttle limphome 
            //9 - Torque to high during throttle limphome 
            //10 - Not used 
            //11 - Tampering protection of throttle 
            //12 - Error on all ignition trigg outputs 
            //13 - ECU not correctly programmed 
            //14 - To high rpm in Throttle limp home, pedal poti fault 
            //15 - Torque Master fuel cut request 
            //16 - TCM requests fuelcut to smoothen gear shift 
            //20 - Application conditions for fuel cut: See \"HELP FUEL_CUT\" menu.
            string retval = value.ToString();
            switch (value)
            {
                case 0:
                    retval = "No fuelcut";
                    break;
                case 1:
                    retval = "Ignition key turned off";
                    break;
                case 2:
                    retval = "Accelerator pedal pressed during start";
                    break;
                case 3:
                    retval = "RPM limiter (engine speed guard)";
                    break;
                case 4:
                    retval = "Throttle block adaption active 1st time";
                    break;
                case 5:
                    retval = "Engine position lost";
                    break;
                case 6:
                    retval = "Airmass limit (pressure guard)";
                    break;
                case 7:
                    retval = "Immobilizer code incorrect";
                    break;
                case 8:
                case 9:
                    retval = "Starter control relay circuit short to ground";
                    break;
                case 11:
                    retval = "Tampering protection of throttle";
                    break;
                case 12:
                    retval = "Error on all ignition trigger outputs";
                    break;
                case 13:
                    retval = "ECU not correctly programmed";
                    break;
                case 14:
                    retval = "Forced fuelcut by user";
                    break;
                case 15:
                    retval = "Transmission requests fuelcut";
                    break;
                case 16:
                    retval = "Kill engine, after engine has started, rpm too low";
                    break;
                case 20:
                    retval = "Application conditions for fuel cut -SAAB";
                    break;
                case 21:
                    retval = "Application conditions for fuel cut -OPEL";
                    break;
                case 31:
                    retval = "Power management fuelcut on one cylinder";
                    break;
                case 32:
                    retval = "Power management fuelcut on two cylinders";
                    break;
                case 33:
                    retval = "Power management fuelcut on three cylinders";
                    break;
                case 34:
                    retval = "Power management fuelcut on four cylinders";
                    break;
                case 35:
                    retval = "Power management fuelcut on five cylinders";
                    break;
                case 36:
                    retval = "Power management fuelcut on six cylinders";
                    break;
            }
            return retval;
        }

        private string ConvertLambdaStatus(int value)
        {
            string retval = value.ToString();
            switch (value)
            {
                case 0:
                    retval = "Closed loop activated";
                    break;
                case 1:
                    retval = "Closed loop not activated";
                    break;
                case 2:
                    retval = "Load too low";
                    break;
                case 3:
                    retval = "Fuel enrichment in progress (no knock)";
                    break;
                case 4:
                    retval = "Fuel enrichment in progress (knock)";
                    break;
                case 5:
                    retval = "CW temp too low, closed throttle";
                    break;
                case 6:
                    retval = "CW temp too low, open throttle";
                    break;
                case 7:
                    retval = "Engine speed too low";
                    break;
                case 8:
                    retval = "Negative throttle transient in progress";
                    break;
                case 9:
                    retval = "Positive throttle transient in progress";
                    break;
                case 10:
                    retval = "Fuel cut";
                    break;
                case 11:
                    retval = "Throttle in limp home";
                    break;
                case 12:
                    retval = "Diagnostic failure that affects the lambda control";
                    break;
                case 13:
                    retval = "Engine not started";
                    break;
                case 14:
                    retval = "Waiting number of combustion before hardware check";
                    break;
                case 15:
                    retval = "Waiting until engine probe is warm";
                    break;
                case 16:
                    retval = "Waiting until number of combustions have past after probe is warm";
                    break;
                case 17:
                    retval = "Hot soak in progress";
                    break;
                case 18:
                    retval = "SAI: Number of combustion to start closed loop has not passed";
                    break;
                case 19:
                    retval = "Lambda integrator is frozen to 0 by SAI lean clamp";
                    break;
                case 20:
                    retval = "Catalyst diagnose for V6 controls the fuel";
                    break;
                case 21:
                    retval = "Lambda start not finished";
                    break;
                case 22:
                    retval = "Lambda probe diagnose request open loop";
                    break;
            }
            return retval;
        }

        private string ConvertActiveAirDemand(int value)
        {
            string retval = value.ToString();
            switch (value)
            {
                case 10:
                    retval = "PedalMap";
                    break;
                case 11:
                    retval = "Cruise control";
                    break;
                case 12:
                    retval = "Idle control";
                    break;
                case 20:
                    retval = "Max engine torque";
                    break;
                case 21:
                    retval = "Traction control";
                    break;
                case 22:
                    retval = "Manual gearbox limit";
                    break;
                case 23:
                    retval = "Automatic gearbox limit";
                    break;
                case 24:
                    retval = "Stall limit (Aut)";
                    break;
                case 25:
                    retval = "Special mode";
                    break;
                case 26:
                    retval = "Reverse limit";
                    break;
                case 27:
                    retval = "Max vehicle speed";
                    break;
                case 28:
                    retval = "Brake management";
                    break;
                case 29:
                    retval = "System action";
                    break;
                case 30:
                    retval = "Max engine speed";
                    break;
                case 31:
                    retval = "Max vehicle speed";
                    break;
                case 40:
                    retval = "Min load";
                    break;
                case 41:
                    retval = "Min load";
                    break;
                case 50:
                    retval = "Knock airmass limit";
                    break;
                case 51:
                    retval = "Max engine speed";
                    break;
                case 52:
                    retval = "Max turbo speed";
                    break;
                case 53:
                    retval = "Max turbo speed";
                    break;
                case 54:
                    retval = "Crankcase vent error";
                    break;
                case 55:
                    retval = "Faulty APC";
                    break;
                case 61:
                    retval = "Engine tipin limit";
                    break;
                case 62:
                    retval = "Engine tipout limit";
                    break;
            }
            return retval;
        }

        private void btnGetFlashContent_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
                {
                    t8can.setCANDevice("LAWICEL");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    t8can.setCANDevice("COMBI");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
                {
                    t8can.ForcedComport = m_appSettings.ELM327Port;
                    t8can.setCANDevice("ELM327");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
                {
                    t8can.setCANDevice("JUST4TRIONIC");
                }
                SetProgress("Starting flash download");
                if (!t8can.isOpen())
                {
                    t8can.openDevice(false);
                }
                if (t8can.isOpen())
                {
                    m_prohibitReading = true;
                    //byte[] snapshot = t8can.getFlashContent();
                    byte[] snapshot = t8can.getFlashWithBootloader();
                    File.WriteAllBytes(sfd.FileName, snapshot);
                    frmInfoBox info = new frmInfoBox("Download done");
                    m_prohibitReading = false;
                    SetProgressIdle();
                }
            }
        }

        private void btnFlashECU_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
            {
                frmInfoBox info = new frmInfoBox("Flashing with an ELM327 device is not supported");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
                {
                    t8can.setCANDevice("LAWICEL");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    t8can.setCANDevice("COMBI");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
                {
                    t8can.ForcedComport = m_appSettings.ELM327Port;
                    t8can.setCANDevice("ELM327");
                    frmInfoBox info = new frmInfoBox("Flashing with an ELM327 device is not supported");
                    return;
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
                {
                    t8can.setCANDevice("JUST4TRIONIC");
                }
                SetProgress("Starting flash download");
                if (!t8can.isOpen())
                {
                    t8can.openDevice(false);
                }
                if (t8can.isOpen())
                {
                    m_prohibitReading = true;
                    Thread.Sleep(1000);
                    System.Windows.Forms.Application.DoEvents();
                    if (t8can.UpdateFlash(ofd.FileName))
                    {
                        frmInfoBox info = new frmInfoBox("Flash sequence done");
                    }
                    else
                    {
                        if (t8can.NeedRecovery)
                        {
                            if (MessageBox.Show("Flash was erased but programming failed, do you which to attempt to recover the ECU?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.OK)
                            {
                                t8can.RecoverECU(ofd.FileName);
                            }
                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("Failed to update flash");
                        }
                        //<GS-12052011> if erase was done, we need to give the user an option to recover the ECU

                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Unable to connect to Trionic 8 ECU");
                }
                m_prohibitReading = false;
                SetProgressIdle();
            }
        }

        private void btnImportSRAMFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            //openFileDialog3.FileName = Path.GetFileNameWithoutExtension(m_currentfile) + ".RAM";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Snapshots|*.RAM";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                {
                    OpenSRAMFile(ofd.FileName);
                }
            }
        }

        private void OpenSRAMFile(string filename)
        {
            // opens the sram dump file and displays the maps inside the dump file
            // after verifying that this ramdump matches the selected binary.
            // this can result in a compare between flash maps and the sram map values
            m_currentsramfile = filename;
            barStaticItem1.Caption = "SRAM: " + Path.GetFileNameWithoutExtension(m_currentsramfile);

        }

        private byte[] readdatafromSRAMfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
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
            return retval;
        }

        private void StartSRAMTableViewer(string filename, string symbolname, int length, int flashaddress, int sramaddress)
        {
            if (!File.Exists(filename)) return;
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text == "SRAM Symbol: " + symbolname + " [" + Path.GetFileName(m_currentsramfile) + "]")
                {
                    dockPanel = pnl;
                    pnlfound = true;
                    dockPanel.Show();
                }
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Tag = filename;
                    IMapViewer tabdet;//= new MapViewer();
                    tabdet = new MapViewerEx();
                    tabdet.AutoUpdateIfSRAM = false;// m_appSettings.AutoUpdateSRAMViewers;
                    //tabdet.AutoUpdateInterval = m_appSettings.AutoUpdateInterval;
                    tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Filename = filename;
                    tabdet.Viewtype = m_appSettings.DefaultViewType;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    //tabdet.IsHexMode = barViewInHex.Checked;
                    if (m_appSettings.Viewinhex)
                    {
                        tabdet.Viewtype = ViewType.Hexadecimal;
                    }
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                    tabdet.Map_cat = XDFCategories.Undocumented;
                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);

                    /** new 12/11/2008 **/
                    if (!m_appSettings.NewPanelsFloating)
                    {
                        // dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                        {
                            int dw = 650;
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                            }
                            if (dw < 400) dw = 400;
                            if (m_appSettings.ShowGraphs)
                            {
                                dockPanel.FloatSize = new Size(dw, 900);
                            }
                            else
                            {
                                dockPanel.FloatSize = new Size(dw, 500);
                            }
                        }
                        else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                        {
                            int dw = 550;
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                            }
                            if (dw < 380) dw = 380;
                            if (m_appSettings.ShowGraphs)
                            {
                                dockPanel.FloatSize = new Size(dw, 850);
                            }
                            else
                            {
                                dockPanel.FloatSize = new Size(dw, 450);
                            }
                        }
                        else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                        {
                            int dw = 450;
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                            }
                            if (dw < 380) dw = 380;
                            if (m_appSettings.ShowGraphs)
                            {
                                dockPanel.FloatSize = new Size(dw, 700);
                            }
                            else
                            {
                                dockPanel.FloatSize = new Size(dw, 450);
                            }
                        }
                    }

                    SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                    string x_axis = string.Empty;
                    string y_axis = string.Empty;
                    string x_axis_descr = string.Empty;
                    string y_axis_descr = string.Empty;
                    string z_axis_descr = string.Empty;
                    axestrans.GetAxisSymbols(tabdet.Map_name, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);

                    if (x_axis == "BstKnkCal.OffsetXSP")
                    {
                        if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
                    }

                    tabdet.X_axis_name = x_axis_descr;
                    tabdet.Y_axis_name = y_axis_descr;
                    tabdet.Z_axis_name = z_axis_descr;


                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
                    int address = flashaddress;
                    //int sramaddress = sramaddress;
                    if (sramaddress != 0)
                    {
                        // while (address > m_currentfile_size) address -= m_currentfile_size;
                        tabdet.Map_address = address;
                        tabdet.Map_sramaddress = sramaddress;
                        tabdet.Map_length = length;
                        byte[] mapdata = readdatafromSRAMfile(filename, sramaddress, length);
                        tabdet.Map_content = mapdata;
                        tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                        tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                        tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                        tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));

                        tabdet.IsRAMViewer = true;
                        tabdet.OnlineMode = true;
                        tabdet.Dock = DockStyle.Fill;
                        //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                        tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                        //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                        //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                        //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                        //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                        //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                        //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                        //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                        tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(tabdet_onReadFromSRAM);
                        tabdet.onWriteToSRAM += new IMapViewer.WriteDataToSRAM(tabdet_onWriteToSRAM);
                        tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                        tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                        tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);


                        //dockPanel.DockAsTab(dockPanel1);
                        dockPanel.Text = "SRAM Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(filename) + "]";
                        bool isDocked = false;
                        if (m_appSettings.AutoDockSameSymbol)
                        {
                            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                            {
                                if (pnl.Text.StartsWith("SRAM Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                {
                                    dockPanel.DockAsTab(pnl, 0);
                                    isDocked = true;
                                    break;
                                }
                            }
                        }
                        /*if (!isDocked)
                        {
                            if (m_appSettings.AutoDockSameFile)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if ((string)pnl.Tag == filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        isDocked = true;
                                        break;
                                    }
                                }
                            }
                        }*/

                        if (!isDocked)
                        {
                            dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                            if (m_appSettings.AutoSizeNewWindows)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                }
                                else
                                {
                                    //dockPanel.Width = this.Width - dockSymbols.Width - 10;
                                }
                            }
                            if (dockPanel.Width < 400) dockPanel.Width = 400;

                        }
                        dockPanel.Controls.Add(tabdet);
                    }
                }
                catch (Exception newdockE)
                {
                    Console.WriteLine(newdockE.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void mv_onSurfaceGraphViewChanged(object sender, IMapViewer.SurfaceGraphViewChangedEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers)
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is IMapViewer)
                        {
                            if (c != sender)
                            {
                                IMapViewer vwr = (IMapViewer)c;
                                if (vwr.Map_name == e.Mapname)
                                {
                                    vwr.SetSurfaceGraphView(e.Pov_x, e.Pov_y, e.Pov_z, e.Pan_x, e.Pan_y, e.Pov_d);
                                    vwr.Invalidate();
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is IMapViewer)
                                {
                                    if (c2 != sender)
                                    {
                                        IMapViewer vwr2 = (IMapViewer)c2;
                                        if (vwr2.Map_name == e.Mapname)
                                        {
                                            vwr2.SetSurfaceGraphView(e.Pov_x, e.Pov_y, e.Pov_z, e.Pan_x, e.Pan_y, e.Pov_d);
                                            vwr2.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                        else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                        {
                            DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                            foreach (Control c3 in cntr.Controls)
                            {
                                if (c3 is IMapViewer)
                                {
                                    if (c3 != sender)
                                    {
                                        IMapViewer vwr3 = (IMapViewer)c3;
                                        if (vwr3.Map_name == e.Mapname)
                                        {
                                            vwr3.SetSurfaceGraphView(e.Pov_x, e.Pov_y, e.Pov_z, e.Pan_x, e.Pan_y, e.Pov_d);
                                            vwr3.Invalidate();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private byte[] ReadMapFromSRAM(SymbolHelper sh)
        {
            m_prohibitReading = true;
            bool success = false;
            int blockSize = 0x40;

            // we need security access for this


            Console.WriteLine("reading from address: " + sh.Start_address.ToString("X8") + " len: " + sh.Length.ToString());
            byte[] mapvalues = new byte[sh.Length];
            if (sh.Length < blockSize)
            {
                mapvalues = t8can.readMemory((int)sh.Start_address, sh.Length, out success);
                Console.WriteLine("ReadMapFromSRAM: " + success.ToString());
            }
            else
            {
                // 0x80 bytes at a time
                int nrReads = sh.Length / blockSize;
                if (sh.Length % blockSize > 0) nrReads++;
                int idx = 0;
                for (int i = 0; i < nrReads; i++)
                {
                    int address2read = (int)sh.Start_address + (i * blockSize);
                    int length2read = blockSize;
                    byte[] blockBytes = t8can.readMemory(address2read, length2read, out success);
                    Thread.Sleep(1);
                    Console.WriteLine("ReadMapFromSRAM: " + success.ToString());
                    // copy bytes to complete buffer
                    for (int j = 0; j < length2read; j++)
                    {
                        if (idx < mapvalues.Length)
                        {
                            mapvalues[idx++] = blockBytes[j];
                        }
                    }
                }
            }
            m_prohibitReading = false;
            return mapvalues;
        }

        void tabdet_onReadFromSRAM(object sender, IMapViewer.ReadFromSRAMEventArgs e)
        {
            // read data from SRAM through CAN bus and refresh the viewer with it
            bool writepossible = false;
            try
            {
                m_prohibitReading = true;
                //if (flash != null)
                {
                    if (m_connectedToECU)
                    {
                        writepossible = true;
                        //T5 byte[] resulttemp = tcan.readRAM((ushort)GetSymbolAddressSRAM(m_symbols, e.Mapname), (uint)GetSymbolLength(m_symbols, e.Mapname) + 1);
                        int symbolindex = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, e.Mapname), e.Mapname);
                        if (symbolindex >= 0)
                        {
                            //Console.WriteLine("Reading " + symbolindex.ToString() + " " + e.Mapname);
                            System.Windows.Forms.Application.DoEvents();
                            foreach (SymbolHelper shs in m_symbols)
                            {
                                if (shs.Varname == e.Mapname)
                                {
                                    try
                                    {
                                        byte[] result = ReadMapFromSRAM(shs);
                                        int rows = 0;
                                        int cols = 0;
                                        foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                        {
                                            if (pnl.Text.StartsWith("Symbol: ") || pnl.Text.StartsWith("SRAM"))
                                            {
                                                foreach (Control c in pnl.Controls)
                                                {
                                                    if (c is IMapViewer)
                                                    {
                                                        IMapViewer vwr = (IMapViewer)c;
                                                        if (vwr.Map_name == e.Mapname)
                                                        {
                                                            vwr.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, e.Mapname, out cols, out rows);
                                                            vwr.IsRAMViewer = false;
                                                            vwr.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                            if ((m_connectedToECU) /*|| m_appSettings.DebugMode*/)
                                                            {
                                                                vwr.OnlineMode = true;
                                                                vwr.IsRAMViewer = true;
                                                            }
                                                            else
                                                            {
                                                                vwr.IsRAMViewer = false;
                                                            }
                                                        }
                                                    }
                                                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                                                    {
                                                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                                                        foreach (Control c2 in tpnl.Controls)
                                                        {
                                                            if (c2 is IMapViewer)
                                                            {
                                                                IMapViewer vwr2 = (IMapViewer)c2;
                                                                if (vwr2.Map_name == e.Mapname)
                                                                {
                                                                    vwr2.Map_content = result;
                                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, e.Mapname, out cols, out rows);
                                                                    vwr2.IsRAMViewer = false;
                                                                    vwr2.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                                    if ((m_connectedToECU) /*|| m_appSettings.DebugMode*/)
                                                                    {
                                                                        vwr2.OnlineMode = true;
                                                                        vwr2.IsRAMViewer = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        vwr2.IsRAMViewer = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                                                    {
                                                        DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                                                        foreach (Control c3 in cntr.Controls)
                                                        {
                                                            if (c3 is IMapViewer)
                                                            {
                                                                IMapViewer vwr3 = (IMapViewer)c3;
                                                                if (vwr3.Map_name == e.Mapname)
                                                                {
                                                                    vwr3.Map_content = result;
                                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, e.Mapname, out cols, out rows);
                                                                    vwr3.IsRAMViewer = false;
                                                                    vwr3.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                                    if ((m_connectedToECU) /*|| m_appSettings.DebugMode*/)
                                                                    {
                                                                        vwr3.OnlineMode = true;
                                                                        vwr3.IsRAMViewer = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        vwr3.IsRAMViewer = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine("Refresh viewer with SRAM data error: " + E.Message);
                                    }
                                    break;
                                }
                            }

                        }
                    }
                }
                if (!writepossible)
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to get data from the ECU");
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to read MAP from SRAM: " + E.Message);
            }
            m_prohibitReading = false;
        }

        private void WriteMapToSRAM(string mapname, byte[] data)
        {
            //TODO: Implement SRAM writing for Trionic 8
        }

        void tabdet_onWriteToSRAM(object sender, IMapViewer.WriteToSRAMEventArgs e)
        {
            // write data to SRAM, check for valid connection first
            bool writepossible = false;
            try
            {
                //if (flash != null)
                {
                    if (m_connectedToECU)
                    {
                        writepossible = true;
                        m_prohibitReading = true;
                        try
                        {


                            WriteMapToSRAM(e.Mapname, e.Data);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Failed to write to SRAM: " + E.Message);
                        }
                    }
                }
                if (!writepossible)
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to write data to the ECU");
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to write map to SRAM: " + E.Message);
            }
            m_prohibitReading = false;
        }

        private void StartSRAMTableViewer()
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    DevExpress.XtraBars.Docking.DockPanel dockPanel;
                    bool pnlfound = false;
                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                    {
                        if (pnl.Text == "SRAM Symbol: " + sh.Varname + " [" + Path.GetFileName(m_currentsramfile) + "]")
                        {
                            dockPanel = pnl;
                            pnlfound = true;
                            dockPanel.Show();
                        }
                    }
                    if (!pnlfound)
                    {
                        dockManager1.BeginUpdate();
                        try
                        {
                            dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                            if (m_appSettings.ShowGraphs)
                            {
                                dockPanel.FloatSize = new Size(650, 700);
                            }
                            else
                            {
                                dockPanel.FloatSize = new Size(650, 450);
                            }
                            dockPanel.Tag = m_currentsramfile;
                            IMapViewer tabdet;//= new MapViewer();
                            tabdet = new MapViewerEx();
                            tabdet.AutoUpdateIfSRAM = false;// m_appSettings.AutoUpdateSRAMViewers;
                            //tabdet.AutoUpdateInterval = m_appSettings.AutoUpdateInterval;
                            tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                            tabdet.Filename = m_currentsramfile;
                            tabdet.GraphVisible = m_appSettings.ShowGraphs;
                            //tabdet.IsHexMode = barViewInHex.Checked;
                            tabdet.Viewtype = m_appSettings.DefaultViewType;//ViewType.Easy;
                            if (m_appSettings.Viewinhex)
                            {
                                tabdet.Viewtype = ViewType.Hexadecimal;
                            }
                            tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                            tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                            tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                            tabdet.Map_name = sh.Varname;
                            tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                            tabdet.Map_cat = XDFCategories.Undocumented;
                            tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                            tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);

                            /** NEW 12/11/2008 **/
                            if (!m_appSettings.NewPanelsFloating)
                            {
                                dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                                if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                {
                                    int dw = 650;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 400) dw = 400;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 900);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 500);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                {
                                    int dw = 550;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 850);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                {
                                    int dw = 450;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 700);
                                    }
                                    else
                                    {
                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }
                            }
                            else
                            {
                                System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                                dockPanel = dockManager1.AddPanel(floatpoint);
                                if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                                {
                                    int dw = 650;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 400) dw = 400;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 900);
                                        tabdet.SetSplitter(0, 0, 280, false, false);

                                    }
                                    else
                                    {

                                        dockPanel.FloatSize = new Size(dw, 500);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.SmallView)
                                {
                                    int dw = 550;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 850);
                                        tabdet.SetSplitter(0, 0, 250, false, false);
                                        //tabdet.SetSurfaceGraphZoom(0.4);
                                    }
                                    else
                                    {

                                        dockPanel.FloatSize = new Size(dw, 450);
                                        //dockPanel.FloatSize = new Size(550, 450);
                                    }
                                }
                                else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                                {
                                    int dw = 450;
                                    if (tabdet.X_axisvalues.Length > 0)
                                    {
                                        dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                    }
                                    if (dw < 380) dw = 380;
                                    if (m_appSettings.ShowGraphs)
                                    {
                                        dockPanel.FloatSize = new Size(dw, 700);
                                        tabdet.SetSplitter(0, 0, 320, false, false);
                                        // tabdet.SetSurfaceGraphZoom(0.5);
                                    }
                                    else
                                    {
                                        // dockPanel.FloatSize = new Size(450, 450);

                                        dockPanel.FloatSize = new Size(dw, 450);
                                    }
                                }
                                floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                                while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                                dockPanel.FloatLocation = floatpoint;

                            }
                            /** end NEW 12/11/2008 */


                            // z, y and z axis to do
                            /*string xdescr = string.Empty;
                            string ydescr = string.Empty;
                            string zdescr = string.Empty;
                            GetAxisDescriptions(m_currentfile, m_symbols, tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                            tabdet.X_axis_name = xdescr;
                            tabdet.Y_axis_name = ydescr;
                            tabdet.Z_axis_name = zdescr;*/
                            SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                            string x_axis = string.Empty;
                            string y_axis = string.Empty;
                            string x_axis_descr = string.Empty;
                            string y_axis_descr = string.Empty;
                            string z_axis_descr = string.Empty;
                            axestrans.GetAxisSymbols(tabdet.Map_name, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                            if (x_axis == "BstKnkCal.OffsetXSP")
                            {
                                if (!SymbolExists(x_axis)) x_axis = "BstKnkCal.fi_offsetXSP";
                            }

                            tabdet.X_axis_name = x_axis_descr;
                            tabdet.Y_axis_name = y_axis_descr;
                            tabdet.Z_axis_name = z_axis_descr;


                            int columns = 8;
                            int rows = 8;
                            int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
                            int address = (int)sh.Flash_start_address;
                            int sramaddress = (int)sh.Start_address;
                            if (sramaddress != 0)
                            {
                                //while (address > m_currentfile_size) address -= m_currentfile_size;
                                tabdet.Map_address = address;
                                tabdet.Map_sramaddress = sramaddress;
                                int length = sh.Length;
                                tabdet.Map_length = length;
                                byte[] mapdata = readdatafromSRAMfile(m_currentsramfile, sramaddress, length);
                                tabdet.Map_content = mapdata;
                                tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                                tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                                tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                                tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));
                                //TryToAddOpenLoopTables(tabdet);

                                tabdet.IsRAMViewer = true;
                                tabdet.Dock = DockStyle.Fill;
                                //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                                tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                                //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                                //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                                //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                                //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                                //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                                //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                                tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(tabdet_onReadFromSRAM);
                                tabdet.onWriteToSRAM += new IMapViewer.WriteDataToSRAM(tabdet_onWriteToSRAM);

                                tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                                tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);

                                //dockPanel.DockAsTab(dockPanel1);
                                dockPanel.Text = "SRAM Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_currentsramfile) + "]";
                                bool isDocked = false;
                                if (m_appSettings.AutoDockSameSymbol)
                                {
                                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                    {
                                        if (pnl.Text.StartsWith("SRAM Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            isDocked = true;
                                            break;
                                        }
                                    }
                                }
                                if (!isDocked)
                                {
                                    if (m_appSettings.AutoDockSameFile)
                                    {
                                        foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                        {
                                            if ((string)pnl.Tag == m_currentsramfile && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                            {
                                                dockPanel.DockAsTab(pnl, 0);
                                                isDocked = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (!isDocked)
                                {
                                    dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                                    if (m_appSettings.AutoSizeNewWindows)
                                    {
                                        if (tabdet.X_axisvalues.Length > 0)
                                        {
                                            dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                        }
                                        else
                                        {
                                            //dockPanel.Width = this.Width - dockSymbols.Width - 10;
                                        }
                                    }
                                    if (dockPanel.Width < 400) dockPanel.Width = 400;

                                }
                                dockPanel.Controls.Add(tabdet);
                            }
                        }
                        catch (Exception newdockE)
                        {
                            Console.WriteLine(newdockE.Message);
                        }
                        dockManager1.EndUpdate();
                    }
                }
            }

        }

        private void readFromSRAMFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridViewSymbols.FocusedRowHandle >= 0)
            {
                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow(gridViewSymbols.FocusedRowHandle);
                StartSRAMTableViewer(m_currentsramfile, sh.Varname, sh.Length, (int)sh.Flash_start_address, (int)(sh.Start_address & 0x00FFFF));
            }
        }

        private void btnGetECUInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
            {
                t8can.setCANDevice("LAWICEL");
            }
            else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
            {
                t8can.setCANDevice("COMBI");
            }
            else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
            {
                t8can.ForcedComport = m_appSettings.ELM327Port;
                t8can.setCANDevice("ELM327");
            }
            else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
            {
                t8can.setCANDevice("JUST4TRIONIC");
            }
            SetProgress("Starting ECU info retrieval");
            if (!t8can.isOpen())
            {
                t8can.openDevice(true);
            }
            if (t8can.isOpen())
            {
                m_prohibitReading = true;
                // get info and fill a special ECU info screen with it
                frmECUInformation info = new frmECUInformation();
                info.Show();
                info.SetECUHardwareDescription(t8can.GetECUDescription());
                info.SetECUHardware(t8can.GetECUHardware());
                info.SetECUHardwareType(t8can.RequestECUInfo(0x97, ""));
                info.SetECUHardwareSupplierID(t8can.RequestECUInfo(0x92, ""));
                info.SetECUBuildDate(t8can.GetBuildDate());
                info.SetECUSerialNumber(t8can.GetSerialNumber());
                info.SetECUSAABPartnumber(t8can.GetSaabPartnumber());
                info.SetECUBasemodel(t8can.GetInt64FromID(0xCC));
                info.SetECUEndmodel(t8can.GetInt64FromID(0xCB));
                info.SetCalibrationSet(t8can.GetCalibrationSet());
                info.SetCodefileVersion(t8can.GetCodefileVersion());
                info.SetSoftwareVersion(t8can.GetSoftwareVersion());
                info.SetSoftwareVersionFile(t8can.RequestECUInfo(0x0F, ""));
                info.SetSoftwareID1(t8can.RequestECUInfo(0xC1, ""));
                info.SetSoftwareID2(t8can.RequestECUInfo(0xC2, ""));
                info.SetSoftwareID3(t8can.RequestECUInfo(0xC3, ""));
                info.SetSoftwareID4(t8can.RequestECUInfo(0xC4, ""));
                info.SetSoftwareID5(t8can.RequestECUInfo(0xC5, ""));
                info.SetSoftwareID6(t8can.RequestECUInfo(0xC6, ""));
                info.SetVIN(t8can.GetVehicleVIN());
                info.SetEngineType(t8can.RequestECUInfo(0x0C, ""));
                info.SetSpeedLimit(t8can.GetTopSpeed() + " km/h");
                m_prohibitReading = false;

            }
            SetProgressIdle();
        }

        private void LoadMyMaps()
        {
            try
            {
                DevExpress.XtraBars.Ribbon.RibbonPage page_maps = new DevExpress.XtraBars.Ribbon.RibbonPage("My Maps");
                DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgset = new DevExpress.XtraBars.Ribbon.RibbonPageGroup("myMaps settings");
                rpgset.AllowTextClipping = false;
                DevExpress.XtraBars.BarButtonItem btnset = new BarButtonItem();
                btnset.Caption = "Define myMaps";
                btnset.Tag = "DefineMaps";
                btnset.ItemClick += new ItemClickEventHandler(MyMapDefine_ItemClick);
                rpgset.ItemLinks.Add(btnset);
                page_maps.Groups.Add(rpgset);

                if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\mymaps.xml"))
                {
                    try
                    {
                        System.Xml.XmlDocument mymaps = new System.Xml.XmlDocument();
                        mymaps.Load(System.Windows.Forms.Application.StartupPath + "\\mymaps.xml");




                        foreach (System.Xml.XmlNode category in mymaps.SelectNodes("categories/category"))
                        {
                            DevExpress.XtraBars.Ribbon.RibbonPageGroup rpg = new DevExpress.XtraBars.Ribbon.RibbonPageGroup(category.Attributes["title"].Value);
                            rpg.AllowTextClipping = false;
                            foreach (System.Xml.XmlNode map in category.SelectNodes("map"))
                            {
                                DevExpress.XtraBars.BarButtonItem btn = new BarButtonItem();
                                btn.Caption = map.Attributes["title"].Value;
                                btn.Tag = map.Attributes["symbol"].Value;
                                btn.ItemClick += new ItemClickEventHandler(MyMapItems_ItemClick);
                                rpg.ItemLinks.Add(btn);
                            }

                            page_maps.Groups.Add(rpg);
                        }
                    }
                    catch { }
                    //ribbonControl1.Pages.Add(page_maps);

                }
                ribbonControl1.Pages.Insert(3, page_maps);
            }
            catch (Exception myMapsE)
            {
                Console.WriteLine("Failed to create myMaps menu: " + myMapsE.Message);
            }
        }

        void MyMapDefine_ItemClick(object sender, ItemClickEventArgs e)
        {
            // define myMaps!
            frmDefineMyMaps mymapsdef = new frmDefineMyMaps();
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\mymaps.xml"))
            {
                mymapsdef.Filename = System.Windows.Forms.Application.StartupPath + "\\mymaps.xml";
            }
            else
            {
                mymapsdef.CreateNewFile(System.Windows.Forms.Application.StartupPath + "\\mymaps.xml");
            }
            if (mymapsdef.ShowDialog() == DialogResult.OK)
            {
                // reload my maps
                if (ribbonControl1.Pages[3].Text == "My Maps")
                {
                    ribbonControl1.Pages.RemoveAt(3);
                }
                LoadMyMaps();
            }

        }

        void MyMapItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string map_name = e.Item.Tag.ToString().ToLower().Trim();

                /*if (map_name == "targetafr")
                    barButtonItem94_ItemClick(sender, e); // start 'target afr' mapviewer
                else if (map_name == "feedbackafr")
                    btnAFRFeedbackMap_ItemClick(sender, e); // start 'feedback afr' mapviewer
                else if (map_name == "feedbackvstargetafr")
                    btnAFRErrormap_ItemClick(sender, e); // start 'feedback vs target afr' mapviewer
                else*/
                    StartTableViewer(e.Item.Tag.ToString().Trim());
            }
            catch { }
        }

        private void btnCopyAddressTable_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "T8 binary files|*.bin";
            ofd.Multiselect = false;
            ofd.FileName = "";
            ofd.Title = "Select binary file to transfer the address table to...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                

                int addrtaboffset = GetAddrTableOffsetBySymbolTable(m_currentfile) + 7;
                int addrtaboffset_newfile = GetAddrTableOffsetBySymbolTable(ofd.FileName) + 7; 
                Console.WriteLine("Addresstable offset 1: " + addrtaboffset.ToString());
                Console.WriteLine("Addresstable offset 2: " + addrtaboffset_newfile.ToString());
                bool _allow = false;
                if (addrtaboffset == addrtaboffset_newfile) _allow = true;
                if (!_allow)
                {
                    if (MessageBox.Show("Address table start addresses are not equal, continue anyway?", "Attention!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _allow = true;
                    }
                }

                if (_allow)
                {
                    if (addrtaboffset > 0)
                    {
                        FileStream fsread = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
                        using (BinaryReader br = new BinaryReader(fsread))
                        {

                            FileStream fswrite = new FileStream(ofd.FileName, FileMode.Open);
                            using (BinaryWriter bw = new BinaryWriter(fswrite))
                            {
                                fsread.Seek(addrtaboffset - 17, SeekOrigin.Begin); //was - 17
                                fswrite.Seek(addrtaboffset_newfile - 17, SeekOrigin.Begin);
                                bool endoftable = false;
                                while (!endoftable)
                                {
                                    // steeds 10 karaketers
                                    try
                                    {
                                        byte[] bytes = br.ReadBytes(10);
                                        if (bytes.Length == 10)
                                        {
                                            //DumpBytesToConsole(bytes);
                                            if ((Convert.ToInt32(bytes.GetValue(9)) != 0x00))
                                            {
                                                endoftable = true;
                                            }
                                            else
                                            {
                                                // Write to target file
                                                bw.Write(bytes);
                                            }
                                        }
                                        else
                                        {
                                            endoftable = true;
                                        }
                                    }
                                    catch (Exception E)
                                    {
                                        Console.WriteLine(E.Message);
                                    }

                                }
                            }
                        }
                        UpdateChecksum(ofd.FileName, true);
                        frmInfoBox info = new frmInfoBox("Transfer done");
                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Transfer cancelled");
                }
            }
        }

        private void SetupLogFilters()
        {
            // setup the export filters
            LogFilters filterhelper = new LogFilters();
            frmLogFilters frmfilters = new frmLogFilters();
            LogFilterCollection filters = filterhelper.GetFiltersFromRegistry();
            frmfilters.SetFilters(filters);
            SymbolCollection sc = new SymbolCollection();
            foreach (SymbolHelper sh in m_symbols)
            {
                //if (!sh.Varname.Contains("!")) sc.Add(sh);
                if (!IsSymbolCalibration(sh.Varname)) sc.Add(sh);
            }
            frmfilters.SetSymbols(sc);
            if (frmfilters.ShowDialog() == DialogResult.OK)
            {
                filterhelper.SaveFiltersToRegistry(frmfilters.GetFilters());
            }
        }

        private void btnSetupLogFilters_ItemClick(object sender, ItemClickEventArgs e)
        {
            SetupLogFilters();
        }

        private void OpenAndDisplayLogFile(string filename)
        {
            // create a new dock with a graph view in it
            //dockManager1.BeginUpdate();
            DevExpress.XtraBars.Docking.DockPanel dp = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Left);
            dp.Size = new Size(dockManager1.Form.ClientSize.Width - dockSymbols.Width, dockSymbols.Height);
            dp.Hide();
            //dp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

            //dp.FloatLocation = new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 5, dockSymbols.Location.Y + 5);
            //dp.FloatSize = new Size(this.Bounds.
            //dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
            dp.Text = "CANBus logfile: " + Path.GetFileName(filename);
            RealtimeGraphControl lfv = new RealtimeGraphControl();
            //LogFilters lfhelper = new LogFilters();
            //lfv.SetFilters(lfhelper.GetFiltersFromRegistry());
            dp.Controls.Add(lfv);
            lfv.ImportT5Logfile(filename);
            //dp.Height = 600;
            lfv.Dock = DockStyle.Fill;

            dp.Show();


            //dockManager1.EndUpdate();
        }

        private void btnLoadTrionic8Logfile_ItemClick(object sender, ItemClickEventArgs e)
        {
            // open a logfile from the canlog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 8 logfiles|*.t8l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CheckDefaultSymbolColors();
                OpenAndDisplayLogFile(ofd.FileName);
            }
        }

        System.Data.DataTable avgTable;


        private void btnViewMatrixFromLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            // let the user select x axis, y axis and z axis symbols from the logfile
            //
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 8 logfiles|*.t8l";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                avgTable = null;
                string[] alllines = File.ReadAllLines(ofd.FileName);
                System.Windows.Forms.Application.DoEvents();
                DateTime startDate = DateTime.MaxValue;
                DateTime endDate = DateTime.MinValue;
                SymbolCollection sc = new SymbolCollection();
                try
                {
                    // using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        //string line = string.Empty;

                        char[] sep = new char[1];
                        char[] sep2 = new char[1];
                        //int linecount = 0;
                        sep.SetValue('|', 0);
                        sep2.SetValue('=', 0);
                        //while ((line = sr.ReadLine()) != null)
                        foreach (string line in alllines)
                        {
                            string[] values = line.Split(sep);
                            if (values.Length > 0)
                            {
                                try
                                {
                                    //dd/MM/yyyy HH:mm:ss
                                    //string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";

                                    string dtstring = (string)values.GetValue(0);
                                    DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                    if (dt > endDate) endDate = dt;
                                    if (dt < startDate) startDate = dt;
                                    for (int t = 1; t < values.Length; t++)
                                    {
                                        string subvalue = (string)values.GetValue(t);
                                        string[] subvals = subvalue.Split(sep2);
                                        if (subvals.Length == 2)
                                        {
                                            string varname = (string)subvals.GetValue(0);
                                            bool sfound = false;
                                            foreach (SymbolHelper sh in sc)
                                            {
                                                if (sh.Varname == varname)
                                                {
                                                    sfound = true;
                                                }
                                            }
                                            SymbolHelper nsh = new SymbolHelper();
                                            nsh.Varname = varname;
                                            if (!sfound) sc.Add(nsh);
                                        }
                                    }
                                }
                                catch (Exception pE)
                                {
                                    Console.WriteLine(pE.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                }
                frmMatrixSelection sel = new frmMatrixSelection();
                sel.SetSymbolList(sc);
                sel.SetXSelection(m_appSettings.LastXAxisFromMatrix);
                sel.SetYSelection(m_appSettings.LastYAxisFromMatrix);
                sel.SetZSelection(m_appSettings.LastZAxisFromMatrix);
                if (sel.ShowDialog() == DialogResult.OK)
                {
                    // get selected for x, y and z
                    int type = sel.GetViewType(); // <GS-31032011> 0 = mean values, 1 = minimum values, 2 = maximum values
                    string x = sel.GetXAxisSymbol();
                    string y = sel.GetYAxisSymbol();
                    string z = sel.GetZAxisSymbol();
                    m_appSettings.LastXAxisFromMatrix = x;
                    m_appSettings.LastYAxisFromMatrix = y;
                    m_appSettings.LastZAxisFromMatrix = z;
                    double xmin = Double.MaxValue;
                    double xmax = Double.MinValue;
                    double ymin = Double.MaxValue;
                    double ymax = Double.MinValue;
                    double zmin = Double.MaxValue;
                    double zmax = Double.MinValue;
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        // string line = string.Empty;
                        char[] sep = new char[1];
                        char[] sep2 = new char[1];
                        //int linecount = 0;
                        sep.SetValue('|', 0);
                        sep2.SetValue('=', 0);
                        //while ((line = sr.ReadLine()) != null)

                        foreach (string line in alllines)
                        {
                            string[] values = line.Split(sep);
                            if (values.Length > 0)
                            {
                                try
                                {
                                    //dd/MM/yyyy HH:mm:ss
                                    //string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";

                                    string dtstring = (string)values.GetValue(0);
                                    DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                    if (dt > endDate) endDate = dt;
                                    if (dt < startDate) startDate = dt;
                                    for (int t = 1; t < values.Length; t++)
                                    {
                                        string subvalue = (string)values.GetValue(t);
                                        string[] subvals = subvalue.Split(sep2);
                                        if (subvals.Length == 2)
                                        {
                                            string varname = (string)subvals.GetValue(0);
                                            double value = ConvertToDouble((string)subvals.GetValue(1));

                                            if (value > 65535) value = 0;
                                            if (value < -65535) value = 0;
                                            if (varname == x)
                                            {
                                                // get max and min

                                                if (value > xmax) xmax = value;
                                                if (value < xmin) xmin = value;
                                            }
                                            else if (varname == y)
                                            {
                                                // get max and min
                                                if (value > ymax) ymax = value;
                                                if (value < ymin) ymin = value;
                                            }
                                            else if (varname == z)
                                            {
                                                // get max and min
                                                if (value > zmax) zmax = value;
                                                if (value < zmin) zmin = value;
                                            }

                                        }
                                    }
                                }
                                catch (Exception pE)
                                {
                                    Console.WriteLine(pE.Message);
                                }
                            }
                        }
                    }
                    // now we have it all
                    if (xmin == xmax || ymin == ymax)
                    {
                        frmInfoBox info = new frmInfoBox("No data to display ... x or y axis contains no differentiated values");
                        return;
                    }
                    frmMatrixResult result = new frmMatrixResult();
                    result.SetViewType(type); // <GS-31032011> 0 = mean values, 1 = minimum values, 2 = maximum values

                    // parse the file again and add the points
                    System.Data.DataTable dtresult = new System.Data.DataTable();
                    // xmin = -0.8
                    // xmin = 2.01
                    double[] x_values = new double[16];
                    double[] y_values = new double[16];

                    // fill x and y axis
                    for (int i = 0; i < 16; i++)
                    {

                        double xvalue = xmin;
                        if (i > 0) xvalue = xmin + i * ((xmax - xmin) / (15));
                        //Console.WriteLine("Adding: " + xvalue.ToString());
                        try
                        {
                            dtresult.Columns.Add(xvalue.ToString(), Type.GetType("System.Double"));
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Failed to add column: " + E.Message);
                        }
                        x_values.SetValue(xvalue, i); //    test: andersom?
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        //double yvalue = ymin + ((ymax - ymin) / i);
                        double yvalue = ymin;
                        if (i > 0) yvalue = ymin + i * ((ymax - ymin) / (15));
                        y_values.SetValue(yvalue, i); //    test: andersom?
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        try
                        {
                            dtresult.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine("Failed to add empty row: " + E.Message);
                        }
                    }
                    // table filled
                    double _lastX = 0;
                    double _lastY = 0;
                    double _lastZ = 0;
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        //string line = string.Empty;
                        char[] sep = new char[1];
                        char[] sep2 = new char[1];
                        //int linecount = 0;
                        sep.SetValue('|', 0);
                        sep2.SetValue('=', 0);
                        //while ((line = sr.ReadLine()) != null)
                        foreach (string line in alllines)
                        {
                            string[] values = line.Split(sep);
                            if (values.Length > 0)
                            {
                                try
                                {
                                    //dd/MM/yyyy HH:mm:ss
                                    //string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";

                                    string dtstring = (string)values.GetValue(0);
                                    DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                                    if (dt > endDate) endDate = dt;
                                    if (dt < startDate) startDate = dt;
                                    for (int t = 1; t < values.Length; t++)
                                    {
                                        string subvalue = (string)values.GetValue(t);
                                        string[] subvals = subvalue.Split(sep2);
                                        if (subvals.Length == 2)
                                        {
                                            string varname = (string)subvals.GetValue(0);
                                            double value = ConvertToDouble((string)subvals.GetValue(1));
                                            if (varname == x)
                                            {
                                                _lastX = value;
                                            }
                                            else if (varname == y)
                                            {
                                                _lastY = value;
                                            }
                                            else if (varname == z)
                                            {
                                                _lastZ = value;
                                            }

                                        }
                                    }
                                }
                                catch (Exception pE)
                                {
                                    Console.WriteLine(pE.Message);
                                }
                                // add point to the datatable
                                AddPointToDataTable(dtresult, _lastX, _lastY, _lastZ, xmin, xmax, ymin, ymax, zmin, zmax, type);
                            }
                        }
                    }
                    result.MaxValue = zmax;
                    result.MinValue = zmin;
                    result.SetXAxis(x_values);
                    result.SetYAxis(y_values);
                    result.SetXAxisName(x);
                    result.SetYAxisName(y);
                    result.SetZAxisName(z);
                    result.UseNewMapViewer = true;
                    result.SetTable(dtresult);
                    string typedescr = " (Mean values)";
                    if (type == 1) typedescr = " (Minimum values)";
                    else if (type == 1) typedescr = " (Maximum values)";
                    result.Text = "Matrix [" + x + " : " + y + " : " + z + "]" + typedescr;
                    result.ShowDialog();
                }
            }
        }

        private void AddPointToDataTable(System.Data.DataTable dt, double x, double y, double z, double xmin, double xmax, double ymin, double ymax, double zmin, double zmax, int type)
        {
            // table is 16x16
            if (avgTable == null)
            {
                avgTable = new System.Data.DataTable();
                // needs to init
                for (int i = 0; i < 16; i++)
                {
                    //double xvalue = xmin + ((xmax - xmin) / i);
                    double xvalue = xmin;
                    if (i > 0) xvalue = xmin + i * ((xmax - xmin) / (15));

                    avgTable.Columns.Add(xvalue.ToString(), Type.GetType("System.Double"));
                }
                for (int i = 0; i < 16; i++)
                {
                    avgTable.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                }
            }
            // which cell should we update?
            int xindex = 0;
            double xcurrdiff = Double.MaxValue;
            for (int i = 0; i < 16; i++)
            {
                //double xvalue = xmin + ((xmax - xmin) / i);
                double xvalue = xmin;
                if (i > 0) xvalue = xmin + i * ((xmax - xmin) / (15));

                double diffx = Math.Abs(xvalue - x);
                if (diffx < xcurrdiff)
                {
                    xcurrdiff = diffx;
                    xindex = i;
                }
            }
            int yindex = 0;
            double ycurrdiff = Double.MaxValue;
            for (int i = 0; i < 16; i++)
            {
                //double yvalue = ymin + ((ymax - ymin) / i);
                double yvalue = ymin;
                if (i > 0) yvalue = ymin + i * ((ymax - ymin) / (15));

                double diffy = Math.Abs(yvalue - y);
                if (diffy < ycurrdiff)
                {
                    ycurrdiff = diffy;
                    yindex = i;
                }
            }
            yindex = 15 - yindex; // flip the table


            //Console.WriteLine("x = " + x.ToString() + " y = " + y.ToString() + " xindex = " + xindex.ToString() + " yindex = " + yindex.ToString());
            // get the counter from avgTable
            if (type == 0)
            {
                double count = Convert.ToDouble(avgTable.Rows[yindex][xindex]);
                // calculate new average
                double currAvg = Convert.ToDouble(dt.Rows[yindex][xindex]);
                currAvg *= count;
                currAvg += z;
                count++;
                currAvg /= count;
                avgTable.Rows[yindex][xindex] = count;
                dt.Rows[yindex][xindex] = currAvg;
            }
            else if (type == 1)
            {
                // minimum values
                double currAvg = Convert.ToDouble(dt.Rows[yindex][xindex]);
                if (z < currAvg || currAvg == 0) dt.Rows[yindex][xindex] = z;
            }
            else if (type == 2)
            {
                // maximum values
                double currAvg = Convert.ToDouble(dt.Rows[yindex][xindex]);
                if (z > currAvg || currAvg == 0) dt.Rows[yindex][xindex] = z;
            }
        }

        private void btnSetSymbolColors_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmPlotSelection plotsel = new frmPlotSelection();
            // get values from realtime panel
            // LoadRealtimeTable();
            CheckDefaultSymbolColors();
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Start_address > 0)
                {
                    plotsel.AddItemToList(sh.Varname);
                }
            }
            plotsel.ShowDialog();
        }

        private void ShowRealtimeMapFromECU(string symbolname)
        {
            if (m_connectedToECU)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Varname == symbolname)
                    {
                        // convert to realtime symbol
                        // start an SRAM mapviewer for this symbol
                        int symbolnumber = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, sh.Varname), sh.Varname);
                        sh.Symbol_number = symbolnumber;

                        Console.WriteLine("Got symnolnumber: " + symbolnumber.ToString() + " for map: " + sh.Varname);
                        if (symbolnumber >= 0)
                        {
                            //byte[] result = ReadSymbolFromSRAM((uint)symbolnumber);
                            byte[] result = ReadMapFromSRAM(sh);
                            Console.WriteLine("read " + result.Length.ToString() + " bytes from SRAM!");
                            StartTableViewer(sh.Varname);
                            try
                            {
                                int rows = 0;
                                int cols = 0;
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol: ") || pnl.Text.StartsWith("SRAM"))
                                    {
                                        foreach (Control c in pnl.Controls)
                                        {
                                            if (c is IMapViewer)
                                            {
                                                IMapViewer vwr = (IMapViewer)c;
                                                if (vwr.Map_name == sh.Varname)
                                                {
                                                    vwr.Map_content = result;
                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, sh.Varname, out cols, out rows);
                                                    vwr.IsRAMViewer = true;
                                                    vwr.OnlineMode = true;
                                                    vwr.ShowTable(cols, isSixteenBitTable(sh.Varname));
                                                }
                                            }
                                            else if (c is DevExpress.XtraBars.Docking.DockPanel)
                                            {
                                                DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                                                foreach (Control c2 in tpnl.Controls)
                                                {
                                                    if (c2 is IMapViewer)
                                                    {
                                                        IMapViewer vwr2 = (IMapViewer)c2;
                                                        if (vwr2.Map_name == sh.Varname)
                                                        {
                                                            vwr2.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, sh.Varname, out cols, out rows);
                                                            vwr2.IsRAMViewer = true;
                                                            vwr2.OnlineMode = true;
                                                            vwr2.ShowTable(cols, isSixteenBitTable(sh.Varname));
                                                        }
                                                    }
                                                }
                                            }
                                            else if (c is DevExpress.XtraBars.Docking.ControlContainer)
                                            {
                                                DevExpress.XtraBars.Docking.ControlContainer cntr = (DevExpress.XtraBars.Docking.ControlContainer)c;
                                                foreach (Control c3 in cntr.Controls)
                                                {
                                                    if (c3 is IMapViewer)
                                                    {
                                                        IMapViewer vwr3 = (IMapViewer)c3;
                                                        if (vwr3.Map_name == sh.Varname)
                                                        {
                                                            vwr3.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, sh.Varname, out cols, out rows);
                                                            vwr3.IsRAMViewer = true;
                                                            vwr3.OnlineMode = true;
                                                            vwr3.ShowTable(cols, isSixteenBitTable(sh.Varname));
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine("Refresh viewer with SRAM data error: " + E.Message);
                            }
                            break;
                        }
                    }
                }
            }
        }


        private void btnViewKnockCountMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowRealtimeMapFromECU("KnkDetAdap.KnkCntMAP");
        }

        private void btnViewMissfireMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ShowRealtimeMapFromECU("MissfAdap.MissfCntMap");
        }

        private void btnCreateFromTISFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            // we need to decode a file from TIS and generate a .BIN file for that
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ByteCoder bc = new ByteCoder();
            ofd.Filter = "Binary files|*.bin";
            ofd.Title = "Select a binary file to base the new file on";
            try
            {
                ofd.InitialDirectory = Path.Combine(System.Windows.Forms.Application.StartupPath, "Binaries");
            }
            catch (Exception)
            {

            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                if (fi.Length == 0x100000)
                {
                    byte[] newFile = new byte[0x100000];
                    byte[] binFile = File.ReadAllBytes(ofd.FileName);
                    for (int i = 0; i < 0x020000; i++)
                    {
                        newFile[i] = binFile[i];
                    }

                    // open gbf file
                    OpenFileDialog ofd2 = new OpenFileDialog();
                    ofd2.Filter = "TIS T8 files|*.gbf";
                    ofd2.Title = "Choose a TIS T8 file to build the new file with";
                    ofd2.Multiselect = false;
                    if (ofd2.ShowDialog() == DialogResult.OK)
                    {
                        byte[] gbfbytes = File.ReadAllBytes(ofd2.FileName);
                        int idx = 0;
                        for(int gbft = 0; gbft < gbfbytes.Length ; gbft ++)
                        {
                            newFile[0x020000 + gbft] = bc.codeByte(gbfbytes[gbft]);
                            idx++;
                            if (idx == 0xEA)
                            {
                                idx = 0;
                                bc.ResetCounter();
                            }
                        }
                    }
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Choose a filename for the new binary file";
                    sfd.Filter = "Binary files|*.bin";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, newFile);
                        frmInfoBox info = new frmInfoBox("New file created");
                    }

                }
                else
                {
                    frmInfoBox info = new frmInfoBox("You did not choose a valid T8 file as basefile");
                }
            }

        }

        private void exportSymbollistAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // export as CSV
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV files|*.csv";
            sfd.Title = "Select a filename to save the symbollist";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                {
                    foreach (SymbolHelper sh in m_symbols)
                    {
                        sw.WriteLine(sh.Varname.Replace(',', '.') + "," + sh.Flash_start_address.ToString() + "," + sh.Start_address.ToString() + "," + sh.Length + "," + sh.Symbol_number.ToString());
                    }
                }
                frmInfoBox info = new frmInfoBox("Export done");
            }
        }

        private void btnRecoverECU_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
            {
                frmInfoBox info = new frmInfoBox("Flashing with an ELM327 device is not supported");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (m_appSettings.CANBusAdapterType == CANBusAdapter.Lawicel)
                {
                    t8can.setCANDevice("LAWICEL");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.MultiAdapter)
                {
                    t8can.setCANDevice("COMBI");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.ELM327)
                {
                    t8can.ForcedComport = m_appSettings.ELM327Port;
                    t8can.setCANDevice("ELM327");
                }
                else if (m_appSettings.CANBusAdapterType == CANBusAdapter.Just4Trionic)
                {
                    t8can.setCANDevice("JUST4TRIONIC");
                }
                SetProgress("Starting recovery procedure");
                if (!t8can.isOpen())
                {
                    t8can.openDevice(false);
                }
                if (t8can.isOpen())
                {
                    m_prohibitReading = true;
                    Thread.Sleep(1000);
                    System.Windows.Forms.Application.DoEvents();
                    if (t8can.RecoverECU(ofd.FileName))
                    {
                        frmInfoBox info = new frmInfoBox("Recovery done");
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Failed to recover ECU");
                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Unable to connect to Trionic 8 ECU");
                }
                m_prohibitReading = false;
                SetProgressIdle();
            }
        }

        private void btnLookupPartnumber_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmPartnumberLookup lookup = new frmPartnumberLookup();
            lookup.ShowDialog();
            if (lookup.Open_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CloseProject();
                    m_appSettings.Lastprojectname = "";
                    
                    OpenFile(filename);
                    m_appSettings.LastOpenedType = 0;

                }
            }
            else if (lookup.Compare_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CompareToFile(filename);
                }
            }
            //<GS-21062010>
            else if (lookup.CreateNewFile)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CloseProject();
                    m_appSettings.Lastprojectname = "";
                    File.Copy(filename, lookup.FileNameToSave);
                    OpenFile(lookup.FileNameToSave);
                    m_appSettings.LastOpenedType = 0;

                }
            }
        }

        private void ImportXMLDescriptorFile()
        {
            // ask user to point to XML document
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML documents|*.xml";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalSymbols(ofd.FileName, false, m_symbols);
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                SaveAdditionalSymbols();
            }
        }

        private void barButtonItem38_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportXMLDescriptorFile();
        }

        private void ImportCSVDescriptor()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV documents|*.csv";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalCSVSymbols(ofd.FileName);
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                // and save the data to the repository
                SaveAdditionalSymbols();

            }
        }

        private void ImportCSVDescriptor(string filename)
        {
            TryToLoadAdditionalCSVSymbols(filename);
            gridControlSymbols.DataSource = m_symbols;
            SetDefaultFilters();
            gridControlSymbols.RefreshDataSource();
            // and save the data to the repository
            SaveAdditionalSymbols();
        }

        private void TryToLoadAdditionalCSVSymbols(string filename)
        {
            // convert to CSV file format
            // 56;AreaCal.A_MaxAdap;;;
            try
            {
                SymbolTranslator st = new SymbolTranslator();
                char[] sep = new char[1];
                sep.SetValue(';', 0);
                string[] fileContent = File.ReadAllLines(filename);
                foreach (string line in fileContent)
                {
                    string[] values = line.Split(sep);
                    try
                    {
                        string varname = (string)values.GetValue(1);
                        int symbolnumber = Convert.ToInt32(values.GetValue(0));
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Symbol_number == symbolnumber)
                            {
                                sh.Userdescription = varname;
                                string helptext = string.Empty;
                                XDFCategories cat = XDFCategories.Undocumented;
                                XDFSubCategory sub = XDFSubCategory.Undocumented;
                                sh.Description = st.TranslateSymbolToHelpText(sh.Userdescription, out helptext, out cat, out sub);
                                //if(sh.Category == 
                                if (sh.Category == "Undocumented" || sh.Category == "")
                                {
                                    if (sh.Userdescription.Contains("."))
                                    {
                                        try
                                        {
                                            sh.Category = sh.Userdescription.Substring(0, sh.Userdescription.IndexOf("."));
                                            //Console.WriteLine("Set cat to " + sh.Category + " for " + sh.Userdescription);
                                        }
                                        catch (Exception cE)
                                        {
                                            Console.WriteLine("Failed to assign category to symbol: " + sh.Userdescription + " err: " + cE.Message);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception lineE)
                    {
                        Console.WriteLine("Failed to import a symbol from CSV file " + line + ": " + lineE.Message);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to import additional CSV symbols: " + E.Message);
            }
        }

        private void barButtonItem39_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportCSVDescriptor();
        }

        private void barButtonItem40_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportAS2Descriptor();
        }

        private void TryToLoadAdditionalAS2Symbols(string filename)
        {
            // convert to AS2 file format
            
            try
            {
                SymbolTranslator st = new SymbolTranslator();
                char[] sep = new char[1];
                sep.SetValue(';', 0);
                string[] fileContent = File.ReadAllLines(filename);
                int symbolnumber = 0;
                foreach (string line in fileContent)
                {
                    if (line.StartsWith("*"))
                    {
                        symbolnumber++;
                        //AddLog(line);
                        string[] values = line.Split(sep);
                        try
                        {

                            string varname = (string)values.GetValue(0);
                            varname = varname.Substring(1);
                            int idxSymTab = 0;
                            foreach (SymbolHelper sh in m_symbols)
                            {
                                if(sh.Length > 0) idxSymTab++;
                                if(idxSymTab == symbolnumber)
                                //if (sh.Symbol_number == symbolnumber)
                                {
                                    sh.Userdescription = varname;
                                    string helptext = string.Empty;
                                    XDFCategories cat = XDFCategories.Undocumented;
                                    XDFSubCategory sub = XDFSubCategory.Undocumented;
                                    sh.Description = st.TranslateSymbolToHelpText(sh.Userdescription, out helptext, out cat, out sub);
                                    //if(sh.Category == 
                                    if (sh.Category == "Undocumented" || sh.Category == "")
                                    {
                                        if (sh.Userdescription.Contains("."))
                                        {
                                            try
                                            {
                                                sh.Category = sh.Userdescription.Substring(0, sh.Userdescription.IndexOf("."));
                                                //Console.WriteLine("Set cat to " + sh.Category + " for " + sh.Userdescription);
                                            }
                                            catch (Exception cE)
                                            {
                                                Console.WriteLine("Failed to assign category to symbol: " + sh.Userdescription + " err: " + cE.Message);
                                            }
                                        }

                                    }

                                    break;

                                }
                            }
                        }
                        catch (Exception lineE)
                        {
                            Console.WriteLine("Failed to import a symbol from AS2 file " + line + ": " + lineE.Message);
                        }
                        
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to import additional AS2 symbols: " + E.Message);
            }
        }

        private void AddLog(string line)
        {
            using (StreamWriter sw = new StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(line);
            }
        }

        private void ImportAS2Descriptor()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AS2 documents|*.as2";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalAS2Symbols(ofd.FileName);
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                // and save the data to the repository
                SaveAdditionalSymbols();

            }
        }

        private void barButtonItem37_ItemClick(object sender, ItemClickEventArgs e)
        {

            try
            {
                SymbolTranslator st = new SymbolTranslator();
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    char[] sep = new char[1];
                    sep.SetValue(';', 0);
                    char[] sepspace = new char[1];
                    sepspace.SetValue(' ', 0);
                    string[] fileContent = File.ReadAllLines(ofd.FileName);
                    string symbolName = string.Empty;
                    string description = string.Empty;
                    bool readingDescription = false;
                    bool readType = false;
                    int xaxislineCount = -1;
                    int yaxislineCount = -1;
                    int resinfolinecount = -1;
                    string xaxisSymbol = "";
                    string yaxisSymbol = "";
                    float divisor = 1;
                    string type = string.Empty;

                    foreach (string line in fileContent)
                    {
                        if (line.StartsWith("*"))
                        {
                            if (line.StartsWith("*FFFuelCal.KnkEnrichmentMAP"))
                            {
                                Console.WriteLine("hold on");
                            }
                            readType = true;
                            xaxislineCount = -1;
                            yaxislineCount = -1;
                            resinfolinecount = -1;
                            description = description.Replace("Description:[\"", "");
                            description = description.Replace("\"]", "");
                            description = description.Replace("\t", "");
                            description = description.Replace("\"", "");
                            description = description.Trim();
                            while (description.Contains("  ")) description = description.Replace("  ", " ");
                            if (symbolName != string.Empty && description != string.Empty)
                            {
                                string ht = string.Empty;
                                XDFCategories cat = XDFCategories.Undocumented;
                                XDFSubCategory subcat = XDFSubCategory.Undocumented;
                              //  if (st.TranslateSymbolToHelpText(symbolName, out ht, out cat, out subcat) == string.Empty)
                                {
                                    DumpSymbolToSourceFile(symbolName, xaxisSymbol, yaxisSymbol, description, divisor, type);
                                    //Console.WriteLine(symbolName + Environment.NewLine + xaxisSymbol + Environment.NewLine + yaxisSymbol + Environment.NewLine + description);
                                }
                            }
                            divisor = 1;
                            type = string.Empty;
                            readingDescription = false;
                            symbolName = line.Replace("*", "");
                            description = "";
                            xaxisSymbol = "";
                            yaxisSymbol = "";

                        }
                        else if(line.StartsWith("Description:["))
                        {
                            xaxislineCount = -1;
                            yaxislineCount = -1;
                            resinfolinecount = -1;
                            if(!line.Contains("\"]")) readingDescription = true;
                            description = line;
                        }
                        else if (readingDescription)
                        {
                            description += line;
                            if (line.Contains("\"]")) readingDescription = false;
                        }
                        else if (readType)
                        {
                            readType = false;
                            if (line.StartsWith("MAP"))
                            {
                                // we need x and y axis info
                                resinfolinecount = 1;
                                xaxislineCount = 2;
                                yaxislineCount = 4;
                            }
                            else if (line.StartsWith("TABLE"))
                            {
                                resinfolinecount = 1;

                                xaxislineCount = 2;
                            }
                        }
                        else
                        {
                            if(xaxislineCount >= 0) xaxislineCount--;
                            if(yaxislineCount >= 0) yaxislineCount--;
                            if (resinfolinecount >= 0) resinfolinecount--;
                            if (resinfolinecount == 0)
                            {
                                // parse grootheid en factor uit de regel
                                //1 1024 0.000 2048 0 1 3 0 Fac
                                divisor = 1;
                                type = string.Empty;
                                string[] resvalues = line.Split(sepspace);
                                try
                                {
                                    divisor = (float)(1.0F / Convert.ToDouble(resvalues.GetValue(1)));
                                }
                                catch (Exception)
                                {
                                }
                                try
                                {
                                    type = resvalues.GetValue(8).ToString();
                                }
                                catch (Exception)
                                {

                                }
                            }
                            if (xaxislineCount == 0)
                            {
                                xaxislineCount = -1;
                                xaxisSymbol = line;
                            }
                            if (yaxislineCount == 0)
                            {
                                yaxislineCount = -1;
                                yaxisSymbol = line;
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to import additional AS2 symbols: " + E.Message);
            }

            /*StreamReader sr = new StreamReader(@"C:\t7test\T7_EE0C.as2");
            string alltext = sr.ReadToEnd();
            char[] sep = new char[1];
            sep.SetValue((char)0x0A, 0);
            //  sep.SetValue((char)0x0A, 1);
            string[] lines = alltext.Split(sep);



            foreach (SymbolHelper sh in m_symbols)
            {
                //if (sh.Varname != "MAF.Q_AirInletNormHFM") continue;
                if (sh.Description == "" || sh.Description == sh.Varname)
                {
                    // try to find the description in the T7 info file
                    //Console.WriteLine("Find info for " + sh.Varname);
                    for (int i = 0; i < lines.Length; i++)
                    {

                        if (lines[i].StartsWith("*"))
                        {
                            string mapname = lines[i];
                            mapname = mapname.Replace("*", "");
                            mapname = mapname.Replace("\x0d", "");
                            mapname = mapname.Replace("\x0a", "");
                            //if (lines[i].Replace("*", "").StartsWith(sh.Varname))
                            if (mapname == sh.Varname)
                            {
                                // get next info
                                int j = 1;
                                bool foundEnd = false;
                                string description = string.Empty;
                                while (!foundEnd)
                                {
                                    if (lines[i + j].StartsWith("Description:["))
                                    {
                                        description = GetFullDescription(lines, i + j); // lines[i + j];
                                        foundEnd = true;
                                    }
                                    else if (lines[i + j].Trim().Length == 0)
                                    {

                                        foundEnd = true;
                                    }
                                    else if ((i + j) == lines.Length) foundEnd = true;
                                    j++;
                                }
                                description = description.Replace("]", "");
                                description = description.Trim();

                                if (description.Length > 3)
                                {
                                    Console.WriteLine("Found info for " + sh.Varname + ": " + description);
                                    AddToSourceFile(sh.Varname, description, "", "");
                                }
                                else
                                {
                                    //Console.WriteLine("No info found for: " + sh.Varname);
                                }
                            }
                        }
                    }
                }
            }*/
        }

        private void DumpSymbolToSourceFile(string symbolName, string xaxisSymbol, string yaxisSymbol, string description, float divisor, string type)
        {
            if (divisor != 1)
            {
                using (StreamWriter sw = new StreamWriter("c:\\t8fac.cs", true))
                {
                    sw.WriteLine("else if (symbolname == \"" + symbolName + "\") returnvalue = " + divisor.ToString().Replace(",", ".") + ";");
                }
            }
            /*if (description != string.Empty)
            {
                
                using (StreamWriter sw = new StreamWriter("c:\\t8sym.cs", true))
                {
                    sw.WriteLine("case \"" + symbolName + "\":");
                    sw.WriteLine("description = helptext = \"" + description + "\";");
                    sw.WriteLine("category = XDFCategories.Undocumented;");
                    sw.WriteLine("subcategory = XDFSubCategory.Undocumented;");
                    sw.WriteLine("break;");
                }
            }
            if (xaxisSymbol != string.Empty || yaxisSymbol != string.Empty)
            {
                using (StreamWriter sw = new StreamWriter("c:\\t8axis.cs", true))
                {
                    sw.WriteLine("case \"" + symbolName + "\":");
                    sw.WriteLine("x_axis = \"" + xaxisSymbol + "\";");
                    sw.WriteLine("y_axis = \"" + yaxisSymbol + "\";");
                   // sw.WriteLine("x_axis_description = \"\"");
                   // sw.WriteLine("y_axis_description = \"\"");
                   // sw.WriteLine("z_axis_description = \"\"");
                    sw.WriteLine("break;");
                }
            }*/
        }

        private void btnFlexFuelLimiter_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartTableViewer("FFTrqCal.M_maxMAP");
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (gridViewSymbols.FocusedRowHandle >= 0)
            {
                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow(gridViewSymbols.FocusedRowHandle);
                // add to realtime table
                string symName = sh.Varname;
                if (symName.StartsWith("Symbol") && sh.Userdescription != string.Empty)
                {
                    symName = sh.Userdescription;
                }

                switch (symName)
                {
                    case "ActualIn.v_Vehicle2":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "In.v_Vehicle":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "FFTrqProt.Trq_MaxEngineBefComp":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 65535, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "FFTrqProt.Trq_MaxEngine":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 65535, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    default:
                        if (sh.Length == 1)
                        {
                            AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        }
                        else
                        {
                            AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 65535, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        }
                        break;
                }
            }
        }
    }
}