/****
 TODO: tasklist T5Suite 2.0
 * DONE: Canbus wakeup on start connection
 * DONE: Map highlights automatically in all maps with known axis (just pass rpm/map/temp etc into mapviewer and mapviewer should sort it out!
 * DONE: Implement CPUFrequency (enum already in Trionic5File)
 * DONE PARTIALLY: Implement adjustment wizard for boost adaption RPM window and boost error values before adaption takes place (option: boost adaption OFF) (I_fak_max?)
 * DONE: Project based development (subfolders in Projects folder with properties XML file)
 * DONE: Resize/rethink the progressbar gauge (smaller resizing possible etc)
 * DONE: Check fuel consumption values (injector type)
 * DONE: Ability to specify a "temp" folder so the program doesn't drop a bunch of other files in the folder with your bins. No needed, project based
 * DONE: Project based: If a project is opened, logs should be saved in the project folder in a subfolder "Logs"
 * DONE: Project based: If a project is opened, sram snapshots should be saved in the project folder in a subfolder "SRAM snapshots"
 * DONE: Autotune: reset afr-feedbackmap when starting autotune (delete)
 * DONE: Comparing two maps in files results in one always being in hex mode view
 * DONE: Add symbolinfo optionally (below symbollist)
 * DONE: Add option to auto select the started symbol/map
 * DONE: Extended tuning wizard (torque calculated based on boost level?)
 * DONE: Reopen last project in stead of last file if a project as opened last time!
 * DONE: Treat library bins as readonly!!!!
 * DONE: Scalable dashboard (?), look at the carpc dashboard for rescaling options
 * DONE: Tuning wizard should account for 2.0 vs 2.3 engine differences including marker for turbo type
 * DONE: Proper knock indicator support
 * DONE: Autotune: reset counter on cell update (DONE?)
 * DONE: Autotune: reset counter if value received that deviates more than the set parameter!!!
 * DONE: Autotune: try..catch to prevent crashes
 * DONE: Autotune: implement fuel correction with adaption map on starting autotune
 * DONE: Mark current location in LOG
 * DONE: Autotune: closed loop seems to switch to ON after tuning (sometimes?)
 * DONE: Autotune: disable Purge control on start and 're-enable' afterwards
 * DONE: Project based: changelog with rollback functions 
 * DONE: If no project is opened, logfile should be written to the binfile folder
 * DONE: Project based: If a project is opened/closed, a binary backup should be created (if file is altered) in a subfolder "Backups"
 * DONE: AFR target/feedback + auto tune (only if wb02 is present)
 * DONE: Check for anomalies
 * DONE: Adapt_korr for T5.2 vs. Insp_mat
 * DONE: Check all realtime values  
 * DONE: Fix bug in hexviewer (autoclose on window close?)
 * DONE: Tune to larger injectors (presets, including marker in memory)
 * DONE: Tune to E85 including marker in memory
 * DONE: Option to disable (not show) graphs in mapviewer
 * DONE: Option to user-select the viewtype (and so, no autodetect)
 * DONE: Disable the progressbar when it resets to 0%
 * DONE: Open .S19 files
 * DONE: Check easy tune to stage 1/2/3 for injector constant adjust (should NOT be done)
 * DONE: SMAP: Partnumber list shows incorrect info B204 engine, stage 1,2&3 boost checkup for all bins
 * DONE: SMAP: 4301933 - B204S, 4300331 - B204S, 4661146 - B204S, A - Automatic
 * DONE: SMAP: Remove T7 binary codes from partnumber list
 * DONE: SMAP: Check examination report on smaps bin (should be GG in stead of 630s)
 * DONE: SONETT7: Minimizing the realtime panel causes exception (Value 0 is not valid for emSize)
 * DONE: Connecting to ECU with no ECU present leads to 5 minute wait time (timeout)
 * DONE: Binary examinor (determine mapsensor type, injector type, turbo type, stage and make a report/overview screen)
 * DONE: Hex editor now makes changes in a copy of the bin which renders the edit function useless.
 * DONE: TORBOKONGEN: Issue with PGM_MOD (tank diagnostics). Maybe show in pgm_mod difference viewer & partnumber list
 * DONE: SMAP: Read & clear error codes
 * DONE: SONETT7: AFR feedback maps don't live update values
 * DONE: Realtime panel: Option to alter symbol properties (offset, correction)
 * DONE: Realtime panel: Option to save and reload created symbol lists (including user settings in previous point)
 * DONE: Realtime panel: Implement engine status tab
 * DONE: Logging: Configuration option for 1 log overall or 1 log per screen (tab)
 * DONE: Logging: Configuration option for creating a new log for every session (start - stop) or appending to the same log based on a date (one log per day)
 * DONE with transactionlogging: Keep track of modified maps (repository needed, so only with project based development)
 * DONE: SUPERSONETT: Injector conversion wizard issue
 *      Latest t5 II sets 12 in injector wizard and also drops bat corr alot .... runs lean 
        Latest t5 II sets 15 in tune stage x wizard and do not change bat corr (have not tested in car jet)
 *      Inj_konst should be 15!!!
 * DONE: Implement T7 valve wizard
 * DONE: Accepting proposal after autotune session takes minutes!!!
 * DONE SONETT7: Sound notification after a cell update has been done in autotune (ping, optional)
 * DONE: LogWorks 3.0 in the deployment and ask for installation when LogWorks not detected when clicking export to logworks
 * DONE: Filter options in log viewing (logworks & internal)
 * DONE: User library of binary files (XML files) with stage, enginetype etc
 * DONE: bug in mapviewer(s): 2d graph does not get updated initially
 * DONE: Knock map compare function! <GS-14022011>
 * DONE: Bugreport about restarting compressormap viewer hanging (mechel)
 * DONE: Implement proper T5.2 support (adapt_korr etc)
 * DONE: Autotune ignition: Implement
 * OBSOLETE: AFR file compatibility 1.x vs 2.x
 * OBSOLETE: RAWILL: Import/export to excel with detection of office installation
 * OBSOLETE: Reg_kon_mat differences (2d vs 3d)
 * OBSOLETE: Build an engine emulator to simulate and test autotune function
 * OBSOLETE: Option to view changelogs for all Suites and a RSS feed for 'feature requests/planning'
 * TODO: More "for dummies" features... like disabling the "save to SRAM" buttons or popping up a message when the lock bit in the binary is set.
 * TODO: Close all open map / data windows when a new bin file is loaded (ask question first!!!) Why?
 * TODO: Automatically refreshing SRAM viewers (how to do when user and/or ECU is editing)
 * TODO: Automatically refreshing and highlighting changed values would be about as cool as it gets (highlight1 for ECU edits, highlight2 for user edit?).
 * TODO: New control with value, label and small progressbar gauge (see above)
 * TODO: Autotune PID: Implement
 * TODO: Autotune startup enrichment: Implement (start_insp, Startvev_fak, Eftersta_fak, Eftersta_fak2 etc)
 * TODO: New feature that shows the changes for the new version if a version is started for the first time (keep version in registry)
 * TODO: 900T5R: Autotune on narrowband lambda sensor
 * TODO: Realtime settings location (point for discussion)
 * TODO: Cleanup log content (2 AFRs, maybe more issues?)
 * TODO: Option to set the realtime panel fullscreen?
 * TODO: Preset color schemes for logviewing
 * TODO: Nightpanel option in realtime panel
 * TODO: Maybe add a "ABSOLUTE MAX IGNITION MAP" for additional safety when using the autotune ignition function?
 * TODO: Mapviewers, keep CLOSE button always visible?
 * TODO: Option to disable canbus data when using the combiadapter. This would enable users to have EGT and 5 analogue input without a canbus connection.
 * TODO: Visual option to set the interval for realtime symbols by user (drag a trackbar/progressbar control or something like that). Alternatively set symbols high, normal and low prio?
 * TODO: Different approach for knock detection (small knock is not detected in Knock_status and/or Pgm_status) use knock counters instead
 *       First try: Knock_offset1234, length 2 only T5.5
 * DONE: Config/logging issue with combiadapter: changed the input range from 0-255 to 0-5 volt
 * TODO: Auto copy logs options like in the dashboard application
 * TODO: AFR target (and error) displayed when autotuning fuel
 * **/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Trionic5Tools;
using Trionic5Controls;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.IO;
using System.Reflection;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System.Threading;
using MouseGestures;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RealtimeGraph;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using System.Media;
using DevExpress.Skins;
using NLog;

namespace T5Suite2
{
    #region enumerations
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
    public enum SyncOption : int
    {
        ToECU,
        ToFile
    }

    public enum ecu_t : int
    {
        Trionic52,
        Trionic55,
        Trionic5529,
        Trionic7,
        None
    }

    public enum OperationMode : int
    {
        ModeOnline,
        ModeOffline
    }

    

    #endregion

    public delegate void FIOCallback(int value);
    internal delegate void FIOInvokeDelegate();
    public delegate void DelegateUpdateRealtimeValue(string symbolname, double value);
    public delegate void DelegateStartReleaseNotePanel(string filename, string version);
    public delegate void DelegateUpdateMapViewer(IMapViewer viewer, int tabwidth, bool sixteenbits);
    public delegate void DelegateFeedInfoToAFRMaps();
    public delegate void DelegateUpdateBDMProgress(uint bytes);

    public partial class Form1 : Form
    {
        IECUFile m_trionicFile;
        Trionic5Properties props = new Trionic5Properties();
        bool m_startFromCommandLine = false;
        string m_commandLineFile = string.Empty;
        private CombiAdapterMonitor _adapterMonitor; 
        private FTDIAdapterMonitor _ftdiAdapterMonitor;
        private IECUFileInformation m_trionicFileInformation = new Trionic5FileInformation();
        ECUConnection _ecuConnection = new ECUConnection();
        OperationMode _APPmode = OperationMode.ModeOffline; // always start in offline mode
        OperationMode _ECUmode = OperationMode.ModeOffline; // always start in offline mode
        AppSettings m_appSettings;
        private bool _overruleTPS = false;
        private bool _syncAskedForECUConnect = false;
        public DelegateUpdateRealtimeValue m_DelegateUpdateRealtimeValue;
        public DelegateStartReleaseNotePanel m_DelegateStartReleaseNotePanel;
        public DelegateFeedInfoToAFRMaps m_DelegateFeedInfoToAFRMaps;
        frmSplash _splash = new frmSplash();
        private bool _immoValid = false;
        private msiupdater m_msiUpdater;
        private bool _connectionWasOpenedBefore = false;
        private string m_CurrentWorkingProject = string.Empty;
        private TrionicProjectLog m_ProjectLog = new TrionicProjectLog();
        SoundPlayer sndplayer;
        /*private float[] targetmap;
        private float[] AFRMapInMemory;
        private int[] AFRMapCounterInMemory;*/
        public DelegateUpdateMapViewer m_DelegateUpdateMapViewer;
        public DelegateUpdateBDMProgress m_DelegateUpdateBDMProgress;
        private ecu_t _globalECUType = ecu_t.None;
        private bool _globalBDMOpened = false;
        private ushort BDMversion = 0;
        private ProgramModeSettings m_ProgramModeSettings = new ProgramModeSettings();
        private AFRMaps m_AFRMaps;
        private IgnitionMaps m_IgnitionMaps;

        private string logworksstring = CommonSuite.LogWorks.GetLogWorksPathFromRegistry();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Form1(string[] args)
        {
            _splash.Show();
            Application.DoEvents();
            _splash.SetProgressText("Initializing...");

            /*HWID _id = new HWID();
            string computerID = _id.GetUniqueIdentifier(Application.StartupPath);
            //_splash.SetProgressText("ID: " + computerID);
            Trionic5Immo immo = new Trionic5Immo();
            //logger.Debug("ID: " + computerID);
            if (immo.ImmoValid(computerID))
            {
                _immoValid = true;
            }
            else
            {
                // show license input window
//                _splash.SetProgressText("License is NOT valid");
                // only for testing purposes: always on
                //_immoValid = true;
            }*/
            _immoValid = true; 

            _splash.SetProgressText("Loading components...");
            InitializeComponent();
            try
            {
                m_DelegateUpdateRealtimeValue = new DelegateUpdateRealtimeValue(this.UpdateRealtimeValue);
                m_DelegateFeedInfoToAFRMaps = new DelegateFeedInfoToAFRMaps(this.FeedInfoIntoAFRMaps);
                m_DelegateUpdateMapViewer = new DelegateUpdateMapViewer(this.UpdateMapViewer);
                m_DelegateUpdateBDMProgress = new DelegateUpdateBDMProgress(this.ReportBDMProgress);
                m_DelegateStartReleaseNotePanel = new DelegateStartReleaseNotePanel(this.StartReleaseNotesViewer);

            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            _splash.SetProgressText("Loading settings...");

            m_appSettings = new AppSettings();
            try
            {
                sndplayer = new SoundPlayer();
            }
            catch (Exception sndE)
            {
                logger.Debug(sndE.Message);
            }
            // reload last file?
            _splash.SetProgressText("Starting desktop...");

            _ecuConnection = new ECUConnection();
            _ecuConnection.CanusbDevice = m_appSettings.CanDevice;
            _ecuConnection.SetWidebandvalues(m_appSettings.WidebandLowVoltage, m_appSettings.WidebandHighVoltage, m_appSettings.WidebandLowAFR, m_appSettings.WidebandHighAFR);
            _ecuConnection.onSymbolDataReceived += new ECUConnection.SymbolDataReceived(_ecuConnection_onSymbolDataReceived);
            _ecuConnection.onCycleCompleted += new ECUConnection.CycleCompleted(_ecuConnection_onCycleCompleted);
            _ecuConnection.onWriteDataToECU += new ECUConnection.WriteDataToECU(_ecuConnection_onWriteDataToECU);
            _ecuConnection.onCanBusInfo += new ECUConnection.CanBusInfo(_ecuConnection_onCanBusInfo);
            _ecuConnection.onReadDataFromECU += new ECUConnection.ReadDataFromECU(_ecuConnection_onReadDataFromECU);
            //_ecuConnection.RunInEmulationMode = m_appSettings.DebugMode; // <GS-17032011> if debugging, run emulator in stead of real connection

            CommonSuite.SystemFileAssociation.Create(@"SystemFileAssociations\.bin\shell\Edit in T5Suite 2.0\command");
            
            try
            {
                // should be done only once!
                this.fio_callback = new FIOCallback(this.on_fio);
                BdmAdapter_SetFIOCallback(this.fio_callback);
                logger.Debug("BDM adapter callback set!");
                // should be done only once!

            }
            catch (Exception BDMException)
            {
                logger.Debug("BDM init failed: " + BDMException.Message);
            }
            if (args.Length > 0)
            {
                if (args[0].ToString().ToUpper().EndsWith(".BIN"))
                {
                    // open the bin file 
                    m_startFromCommandLine = true;
                    m_commandLineFile = args[0].ToUpper();
                }
            }
            try
            {
                _adapterMonitor = new CombiAdapterMonitor();
                _adapterMonitor.onAdapterPresent += new CombiAdapterMonitor.AdapterPresent(_adapterMonitor_onAdapterPresent);
                _adapterMonitor.Start();
                _ftdiAdapterMonitor = new FTDIAdapterMonitor();
                _ftdiAdapterMonitor.onAdapterPresent += new FTDIAdapterMonitor.AdapterPresent(_ftdiAdapterMonitor_onAdapterPresent);
                _ftdiAdapterMonitor.Start();
            }
            catch (Exception E)
            {
                logger.Debug("Failed to instantiate the combiadapter monitor: " + E.Message);
            }
        }

        void _ftdiAdapterMonitor_onAdapterPresent(object sender, FTDIAdapterMonitor.AdapterEventArgs e)
        {
            if (e.Present)
            {
                //logger.Debug(e.Type.ToString() + " connected");
                barItemCombiAdapter.Caption = e.Type.ToString() + " connected";
                if (e.Type == AdapterType.LAWICEL && m_appSettings.CanDevice != "Lawicel")
                {
                    frmInfoBox info = new frmInfoBox("You have connected a Lawicel CANUSB adapter but the settings show that this is not the currently selected canusb device!");
                }
            }
            else
            {
                barItemCombiAdapter.Caption = "";
            }
            Application.DoEvents();
        }

        void _adapterMonitor_onAdapterPresent(object sender, CombiAdapterMonitor.AdapterEventArgs e)
        {
            if (e.Present)
            {
                barItemCombiAdapter.Caption = "CombiAdapter connected";
                if (m_appSettings.CanDevice != "Multiadapter" && m_appSettings.CanDevice != "CombiAdapter")
                {
                    frmInfoBox info = new frmInfoBox("You have connected a CombiAdapter but the settings show that this is not the currently selected canusb device!");
                }
            }
            else
            {
                barItemCombiAdapter.Caption = "";
            }
            Application.DoEvents();
        }


        private void CreateECUConnection()
        {
            try
            {

                
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void UpdateMapViewer(IMapViewer viewer, int tabwidth, bool sixteenbits)
        {
            viewer.ShowTable(tabwidth, sixteenbits);
        }

        void _ecuConnection_onReadDataFromECU(object sender, ECUConnection.ProgressEventArgs e)
        {
            // signal progress!
            SetTaskProgress(e.Percentage, true);
            if (e.Percentage == 100) SetTaskProgress(0, false);
        }

        void _ecuConnection_onCanBusInfo(object sender, ECUConnection.CanInfoEventArgs e)
        {
            SetStatusText(e.Info);
            logger.Debug(DateTime.Now.ToString("HH:mm:ss:fff") + " " + e.Info);
        }

        void _ecuConnection_onWriteDataToECU(object sender, ECUConnection.ProgressEventArgs e)
        {
            // show progress bar!
            SetTaskProgress(e.Percentage, true);
        }

        void _ecuConnection_onSymbolDataReceived(object sender, ECUConnection.RealtimeDataEventArgs e)
        {
            // process received symboldata into the application
            // invoke method because data is coming from another thread!
            if (!this.IsDisposed)
            {
                try
                {
                    this.Invoke(m_DelegateUpdateRealtimeValue, e.Symbol, e.Value);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }

           
        }

        double _currentEngineSpeed = 0;
        double _currentBoostLevel = 0;
        double _currentBoostTarget = 0;
        double _currentThrottlePosition = 0;
        double _currentAFR = 0;
        double _currentIgnitionAdvance = 0;
        bool _autoTuneAllowed = false;
        bool _idleMapActive = false;
        bool _ignitionIdleActive = false;
        bool _currentKnockStatus = false;

        private bool CheckAutoTuneParameters(double dpgmstatus)
        {
            bool retval = true;
            UInt64 lval = Convert.ToUInt64(dpgmstatus);
            // fetch fuelcut, warmup and knock indication
            // byte bitmask meaning
            // 0    0x10    Engine is warm
            // 0    0x20    Pressure guard triggered (fuel cut)
            // 1    0x02    Knock fuel map activated (knock detected)
            // 1    0x04    Throttle closed
            // 1    0x10    Fuel off cylinder 4
            // 1    0x20    Fuel off cylinder 3
            // 1    0x40    Fuel off cylinder 2
            // 1    0x80    Fuel off cylinder 1
            // 3    0x02    Active lambda control
            // 3    0x04    Afterstart enrichment completed
            // 3    0x10    Cooling water enrichment finished
            // 3    0x20    Purge control active (engine running in closed loop)
            // 3    0x40    Running in idle map
            // 4    0x20    Enrichment after fuelcut
            // 4    0x40    Fulload enrichment (allowed for autotune???)

            //TODO: Add check for TPS enrich/enlean factors! If TPS changes, we should not include the measurement

            if ((lval & 0x0000002000000000) > 0)
            {
                retval = false;                 // Enrichment after fuelcut active
            }
            // ALSO NOT ALLOWED IN IDLE MAP RUNNING!!! 3 0x40 Separate fuel map during idle
            if ((lval & 0x0000000040000000) > 0)
            {
                //<GS-28102010> We should be able to autotune the idle map as well... this bit could indicate WHEN 
                if (m_appSettings.AllowIdleAutoTune)
                {
                    _idleMapActive = true;
                    _ignitionIdleActive = true;
                }
                else
                {
                    retval = false;                 // Idle map, not allowed to tune
                }
            }
            else
            {
                _idleMapActive = false;
                _ignitionIdleActive = false;
            }
            if ((lval & 0x0000000020000000) > 0)
            {
                retval = false;                 // Purge control active
            }
            if ((lval & 0x0000000010000000) == 0)
            {
                retval = false;                 // Cooling water enrichment not yet finished
            }
            if ((lval & 0x0000000004000000) == 0)
            {
                retval = false;                 // Afterstart enrichment is not yet completed
            }
            if ((lval & 0x0000000002000000) > 0)
            {
                retval = false;                 // Lambda control is on
            }
            if ((lval & 0x0000000000008000) > 0)
            {
                if (m_appSettings.DiscardFuelcutMeasurements)
                {
                    retval = false;                 // Cylinder 1 fuel cut off
                }
            }
            if ((lval & 0x0000000000004000) > 0)
            {
                if (m_appSettings.DiscardFuelcutMeasurements)
                {
                    retval = false;                 // Cylinder 2 fuel cut off
                }
            }
            if ((lval & 0x0000000000002000) > 0)
            {
                if (m_appSettings.DiscardFuelcutMeasurements)
                {
                    retval = false;                 // Cylinder 3 fuel cut off
                }
            }
            if ((lval & 0x0000000000001000) > 0)
            {
                if (m_appSettings.DiscardFuelcutMeasurements)
                {
                    retval = false;                 // Cylinder 4 fuel cut off
                }
            }
            if ((lval & 0x0000000000000400) > 0)
            {
                if (m_appSettings.DiscardClosedThrottleMeasurements)
                {
                    retval = false;                 // Throttle is closed, only if settings tell us to discard this
                }
            }
            if ((lval & 0x0000000000000010) == 0) 
            {
                retval = false;                 // ENGINE IS NOT WARMED UP YET
            }
            if ((lval & 0x0000000000000020) > 0)
            {
                if (m_appSettings.DiscardFuelcutMeasurements)
                {
                    retval = false;                 // FUELCUT IS ACTIVE
                }
            }
            if ((lval & 0x0000000000000200) > 0)
            {
                retval = false;                 // Fuel knock map is active
                //_currentKnockStatus = true;
                //_currentKnockStatus = true;
            }
            else
            {
                //_currentKnockStatus = false;
                //_currentKnockStatus = false;
                /*if (m_appSettings.DebugMode)
                {
                    // knock is indicated by using a debug thingie
                    if (this.WindowState == FormWindowState.Maximized) _currentKnockStatus = true;
                }*/
            }
            //if (m_appSettings.DebugMode) retval = true; // <GS-29032010>
            return retval;
        }

        private void UpdateRealtimeValue(string symbol, double value)
        {
            // update the appropriate control
            ctrlRealtime1.SetValue(symbol, value);

            if (symbol == m_trionicFileInformation.GetProgramStatusSymbol())
            {
                _autoTuneAllowed = CheckAutoTuneParameters(value);
                //<GS-03032010> also check latest enrich/enleanment factors.. none of them is allowed to be higher than x
                if (ctrlRealtime1.IsEnrichmentActive()) _autoTuneAllowed = false;

            }
            else if (symbol == "Idle_status")
            {
                // 0, 0x20 mask = Idle mode is active
                UInt64 lval = Convert.ToUInt64(value);
                if ((lval & 0x0000000000000020) > 0)
                {
                    _ignitionIdleActive = true;
                }
                else
                {
                    _ignitionIdleActive = false;
                }
            }
            else if (symbol == "Knock_offset1234")
            {
                // 0, 0x20 mask = Idle mode is active
                UInt64 lval = Convert.ToUInt64(value);
                if ((lval) > 0)
                {
                    _currentKnockStatus = true;
                }
                else
                {
                    _currentKnockStatus = false;
                }
            }
            
            /*else if (symbol == "Knock_status")
            {
                // 0, 0x20 mask = Idle mode is active
                UInt64 lval = Convert.ToUInt64(value);
                if ((lval & 0x0000000000000001) > 0)
                {
                    _currentKnockStatus = true;
                }
                else
                {
                    _currentKnockStatus = false;
                }
            }*/
            else if (symbol == m_trionicFileInformation.GetEngineSpeedSymbol())
            {
                _currentEngineSpeed = value;
            }
            else if (symbol == m_trionicFileInformation.GetBoostTargetSymbol())
            {
                _currentBoostTarget = value;
            }
            else if (symbol == m_trionicFileInformation.GetPressureSymbol())
            {
                _currentBoostLevel = value;
            }
            else if (symbol == m_trionicFileInformation.GetIgnitionAdvanceSymbol())
            {
                _currentIgnitionAdvance = value;
            }
            else if (symbol == m_trionicFileInformation.GetThrottlePositionSymbol())
            {
                //<GS-25032010>
                // check whether the throttle was release quickly
                if (_currentThrottlePosition > value + 10 && _currentThrottlePosition != 9999) // value has decreased significantly
                {
                    // in that case, we should hold autotune functions for at least x ms
                    // to prevent measurement of false AFRs in the wrong cell
                    // during wot mixture is rich and the wideband cannot measure that quick,
                    // so low-load cells will be measured very rich as well if we don't 
                    // stop autotuning for a few seconds
                    // so, as long as "overrule TPS" is active, we shall pass a new value for TPS (9999)
                    // to indicate that the has been sudden changes
                    _overruleTPS = true;
                    tmrOverruleTPS.Stop();
                    tmrOverruleTPS.Start(); // restart the timer, it is set to 500 ms
                }

                if (_overruleTPS) value = 9999;
                _currentThrottlePosition = value;
            }
            else if (symbol == m_appSettings.WidebandLambdaSymbol)
            {
                _currentAFR = value;
                if (m_AFRMaps != null)
                {
                    //logger.Debug("boost: " + _currentBoostLevel.ToString());
                    if (ctrlRealtime1.AutoTuning)
                    {
                        if (_autoTuneAllowed && _currentThrottlePosition != 9999 /*|| m_appSettings.DebugMode*/) // <GS-17032010>
                        {
                            m_AFRMaps.LogWidebandAFR(_currentAFR, _currentEngineSpeed, _currentBoostLevel, _idleMapActive);
                        }
                    }
                    else //<GS-27072010> log wideband when we're not autotuning
                    {
                        // TEST
                        //if (_currentEngineSpeed < 800) _currentEngineSpeed = 880;
                        //logger.Debug("AFR: " + _currentAFR.ToString());
                        m_AFRMaps.LogWidebandAFR(_currentAFR, _currentEngineSpeed, _currentBoostLevel, _idleMapActive);
                        UpdateFeedbackMaps();
                    }
                }
            }
            UpdateMapViewers(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel, _currentBoostTarget);
        }

        void _ecuConnection_onCycleCompleted(object sender, EventArgs e)
        {
            // ecu read cycle completed (all symbols read and refreshed
            // must be done though a new delegate
            if (!this.IsDisposed)
            {
                try
                {
                    this.Invoke(m_DelegateFeedInfoToAFRMaps);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
        }

        private void FeedInfoIntoAFRMaps()
        {
            ctrlRealtime1.RefreshOnlineGraph();
            //if (m_appSettings.AutoUpdateFuelMap)
           // {
                FeedInfoToAFRMaps(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel, _currentAFR, _currentIgnitionAdvance, _currentKnockStatus, _idleMapActive);
           // }
            //else
            //{
                // do something new here
            //}
        }


        private void FeedInfoToAFRMaps(double rpm, double tps, double boost, double AFR, double IgnitionAdvance, bool KnockStatus, bool IdleMapActive)
        {
            if (tps == 9999) return; // ignore these readings, tps changed fast

            // failesafe
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            if (m_IgnitionMaps == null)
            {
                m_IgnitionMaps = new IgnitionMaps();
                m_IgnitionMaps.onCellLocked +=new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                m_IgnitionMaps.onIgnitionmapCellChanged +=new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                m_IgnitionMaps.TrionicFile = m_trionicFile;
                m_IgnitionMaps.InitializeMaps();
            }

            if (_autoTuneAllowed  /*|| m_appSettings.DebugMode*/)
            {
                float targetAFR = m_AFRMaps.HandleRealtimeData(rpm, tps, boost, AFR, _idleMapActive);
                ctrlRealtime1.SetValue("TARGETAFR", (double)targetAFR);
            }
            else
            {
                ctrlRealtime1.SetValue("TARGETAFR", (double)m_AFRMaps.GetCurrentTargetAFR(rpm, boost));
            }
            if (!_ignitionIdleActive) // if idle map is active, we DO NOT tune ignition
            {
                m_IgnitionMaps.HandleRealtimeData(rpm, tps, boost, IgnitionAdvance, KnockStatus);
            }
            if (m_AFRMaps.IsAutoMappingActive)
            {
                // in that case, update the realtime control's data
                //TODO: <GS-28102010> afmaken, idle map tonen en updaten indien _idleMapActive true is
                ctrlRealtime1.UpdateMutatedFuelMap(m_AFRMaps.GetCurrentlyMutatedFuelMap(), m_AFRMaps.GetCurrentlyMutatedFuelMapCounter());
                ctrlRealtime1.UpdateFeedbackAFR(m_AFRMaps.GetFeedbackAFRMap(), m_AFRMaps.GetTargetAFRMap(), m_AFRMaps.GetAFRCountermap());
            }
            if (m_IgnitionMaps.IsAutoMappingActive)
            {
                // update ignition maps in UI
                ctrlRealtime1.UpdateMutatedIgnitionMap(m_IgnitionMaps.GetCurrentlyMutatedIgnitionMap(), m_IgnitionMaps.GetCurrentlyMutatedIgnitionMapCounter(), m_IgnitionMaps.GetIgnitionLockedMap());
            }
            ctrlRealtime1.RedoGrids();
        }

        void m_AFRMaps_onIdleFuelmapCellChanged(object sender, AFRMaps.IdleFuelmapChangedEventArgs e)
        {
            //TODO: <GS-28102010> afmaken
            // seems that we need to adjust a value in the current idle fuelmap
            if (_ecuConnection.Opened)
            {
                if (m_appSettings.AutoUpdateFuelMap && m_appSettings.AllowIdleAutoTune)
                {
                    if (m_AFRMaps.IsAutoMappingActive)
                    {
                        // then go!
                        byte[] write = new byte[1];
                        write[0] = e.Cellvalue;
                        _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()) + e.Mapindex, 1, write);
                    }
                }
                else
                {
                    // update the fuelinformation struct
                }
            }
        }

        void m_AFRMaps_onCellLocked(object sender, EventArgs e)
        {
            //<GS-09082010> play a short sound here
            if (sndplayer != null)
            {
                if (m_appSettings.PlayCellProcessedSound)
                {
                    string sound2play = Application.StartupPath + "\\ping.wav";
                    if (File.Exists(sound2play))
                    {
                        sndplayer.SoundLocation = sound2play;
                        sndplayer.Play();
                    }
                }
            }
        }

        private void UpdateMapViewers(double rpm, double tps, double boost, double boostTarget)
        {
            if (tps == 9999) return; // ignore 
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                // if (pnl.Focused)
                {
                    foreach (Control c in pnl.Controls)
                    {
                        if (c is IMapViewer)
                        {
                            IMapViewer vwr = (IMapViewer)c;
                            vwr.Rpm = rpm;
                            vwr.Tps = tps;
                            vwr.Boost = boost;
                            vwr.BoostTarget = boostTarget;
                        }
                        else if (c is DevExpress.XtraBars.Docking.DockPanel)
                        {
                            DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                            foreach (Control c2 in tpnl.Controls)
                            {
                                if (c2 is IMapViewer)
                                {
                                    IMapViewer vwr2 = (IMapViewer)c2;
                                    vwr2.Rpm = rpm;
                                    vwr2.Tps = tps;
                                    vwr2.Boost = boost;
                                    vwr2.BoostTarget = boostTarget;
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
                                    vwr3.Rpm = rpm;
                                    vwr3.Tps = tps;
                                    vwr3.Boost = boost;
                                    vwr3.BoostTarget = boostTarget;
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Application logic functions

        private void OpenWorkingFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Trionic 5 files|*.bin;*.s19";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
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
                OpenWorkingFile(filename);
                m_appSettings.LastOpenedType = 0;
                SetDefaultFilters();
            }
            
        }

        private bool OpenWorkingFile(string filename)
        {
            bool retval = false;
            if (File.Exists(filename))
            {
                m_trionicFile = new Trionic5File();

                m_trionicFile.LibraryPath = Application.StartupPath + "\\Binaries";

                m_trionicFile.SetAutoUpdateChecksum(m_appSettings.AutoChecksum);

                FileInfo fi = new FileInfo(filename); //<GS-07102010> remove read only flag if possible
                try
                {
                    fi.IsReadOnly = false;
                    btnReadOnly.Caption = "File access OK";
                }
                catch (Exception E)
                {
                    logger.Debug("Failed to remove read only flag: " + E.Message);
                    btnReadOnly.Caption = "File is READ ONLY";
                }



                m_trionicFile.onDecodeProgress += new IECUFile.DecodeProgress(m_trionicFile_onDecodeProgress);
                m_trionicFile.onTransactionLogChanged += new IECUFile.TransactionLogChanged(m_trionicFile_onTransactionLogChanged);
                m_trionicFile.SelectFile(filename);
                //m_trionicFileInformation = m_trionicFile.ParseTrionicFile(ofd.FileName);
                m_trionicFileInformation = m_trionicFile.ParseFile();
                props = m_trionicFile.GetTrionicProperties();
                // set indicators and menu items accoring to the file that has been opened
                if (props.IsTrionic55)
                {
                    barECUType.Caption = "T5.5";
                    // enable T5.5 maps
                    EnableT55Maps(true);
                }
                else
                {
                    barECUType.Caption = "T5.2";
                    EnableT55Maps(false);
                    // disable T5.5 maps
                }
                barECUSpeed.Caption = props.CPUspeed;
                if (props.RAMlocked)
                {
                    barECULocked.Caption = "RAM locked";
                }
                else
                {
                    barECULocked.Caption = "RAM unlocked";
                }
                if (CheckFileInLibrary(props.Partnumber + "-" + props.SoftwareID))
                {
                    btnCompareToOriginalFile.Enabled = true;
                }
                else
                {
                    btnCompareToOriginalFile.Enabled = false;
                }
                _ecuConnection.MapSensorType = m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType);
                gridSymbols.DataSource = m_trionicFileInformation.SymbolCollection;
                barStaticItem2.Caption = "File: " + Path.GetFileName(m_trionicFileInformation.Filename);
                this.Text = "T5Suite Professional 2.0 [" + Path.GetFileName(m_trionicFileInformation.Filename) + "]";
                OpenGridViewGroups(gridSymbols, 0);
                // enable buttons
                barButtonItem4.Enabled = true;
                btnConnectECU.Enabled = true;
                btnSwitchMode.Enabled = true;
                //btnSynchronizeMaps.Enabled = true;
                btnTuneForE85Fuel.Enabled = true; //<GS-06042010> todo
                btnTuneToLargerInjectors.Enabled = true; //<GS-06042010> todo
                btnTuneToStage1.Enabled = true;
                btnTuneToStage2.Enabled = true;
                btnTuneToStage3.Enabled = true;
                btnTuneToStageX.Enabled = true;

                //            btnTuneToThreeBarSensor.Enabled = true;
                barConvertMapSensor.Enabled = true;
                btnBoostAdaptionWizard.Enabled = true;
                btnHardcodedRPMLimit.Enabled = true;
                if (m_trionicFileInformation.Has2DRegKonMat())
                {
                    btnChangeRegkonMatRange.Enabled = true;
                }
                else
                {
                    btnChangeRegkonMatRange.Enabled = false;
                }

                m_appSettings.Lastfilename = filename;
                ctrlRealtime1.Currentfile = filename;

                if (m_AFRMaps != null)
                {
                    m_AFRMaps.SaveMaps(); // first save changes that might have been done
                }

                m_AFRMaps = null;
                if (m_AFRMaps == null && m_appSettings.AlwaysCreateAFRMaps)
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }
                // add realtime symbollist to ctrlRealtime1
                Trionic5Tools.SymbolCollection _rtSymbols = new Trionic5Tools.SymbolCollection();
                foreach (Trionic5Tools.SymbolHelper symh in m_trionicFileInformation.SymbolCollection)
                {
                    if (symh.Start_address > 0 && (symh.Length >= 1 && symh.Length <= 4)) // <GS-29072010> was 1 & 2 only
                    {
                        _rtSymbols.Add(symh);
                    }
                }
                ctrlRealtime1.RealtimeSymbolCollection = _rtSymbols;
                try
                {
                    LoadUserDefinedRealtimeSymbols(); // try to load it
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                LoadKnockMaps();
                SetTaskProgress(0, false);
                retval = true;
            }
            return retval;
        }


        private bool CheckFileInLibrary(string partnumber)
        {
            string libPath = Application.StartupPath + "\\Binaries";
            bool retval = false;
            if(Directory.Exists(libPath))
            {
                string[] filesinLibrary = Directory.GetFiles(libPath, "*.bin");
                foreach (string libFile in filesinLibrary)
                {
                    if (Path.GetFileNameWithoutExtension(libFile).ToUpper() == partnumber.ToUpper())
                    {
                        retval = true;
                        break;
                    }
                }
            }
            return retval;
        }

        private void EnableT55Maps(bool enable)
        {
            btnFirstGearLimiterManual.Enabled = enable;
            btnSecondGearLimiterManual.Enabled = enable;
            btnKnockLimitMap.Enabled = enable;
            btnBoostAdaptionWizard.Enabled = enable;
            btnChangeRegkonMatRange.Enabled = enable;
            btnHardcodedRPMLimit.Enabled = enable;
        }
        

        void m_AFRMaps_onFuelmapCellChanged(object sender, AFRMaps.FuelmapChangedEventArgs e)
        {
            // seems that we need to adjust a value in the current fuelmap
            if (_ecuConnection.Opened)
            {
                if (m_appSettings.AutoUpdateFuelMap)
                {
                    if (m_AFRMaps.IsAutoMappingActive)
                    {
                        // then go!
                        byte[] write = new byte[1];
                        write[0] = e.Cellvalue;
                        
                        if (!props.IsTrionic55)
                        {
                            _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr") + e.Mapindex, 1, write);
                        }
                        else
                        {
                            _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()) + e.Mapindex, 1, write);
                        }
                        // now clear that data in the afrfeedback map because the settings for that cell have changed
                        // note: we don't use the feedback map for alteration, just for user feedback
                        // m_AFRMaps.ClearAFRFeedbackMapCell(e.Mapindex); //<GS-23122009>
                    }
                }
                else
                {
                    // update the fuelinformation struct
                }
            }

        }

        private void SetStatusText(string text)
        {
            if (barStaticItem4.Caption != text)
            {
                barStaticItem4.Caption = text;
                Application.DoEvents();
            }
        }

        private void SetTaskProgress(int progress, bool enableBar)
        {
            //DevExpress.XtraEditors.ProgressBarControl progressbar = (DevExpress.XtraEditors.ProgressBarControl)repositoryItemProgressBar1;
            if (Convert.ToInt32(barEditItem1.EditValue) != progress)
            {
                barEditItem1.EditValue = progress;
                Application.DoEvents();
            }
            if (progress > 99)
            {
                barEditItem1.EditValue = 0;
                enableBar = false;
                Application.DoEvents();
            }
            //logger.Debug("Perc: " + progress.ToString() + " enable: " + enableBar.ToString());
            if (barEditItem1.Enabled && !enableBar)
            {
                barEditItem1.Caption = "---";
                barEditItem1.Enabled = false;
                Application.DoEvents();
            }
            if (!barEditItem1.Enabled && enableBar)
            {
                barEditItem1.Caption = "Progress";
                barEditItem1.Enabled = true;
                Application.DoEvents();
            }
            
            //progressbar.EditValue = progress;
            
        }

        void m_trionicFile_onDecodeProgress(object sender, DecodeProgressEventArgs e)
        {
            SetStatusText("Decoding file");
            SetTaskProgress(e.Progress, true);
            if (e.Progress == 0)
            {
                SetStatusText("Idle");
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

        private void StartTableViewerSRAMFile(string symbolname, string sramfile)
        {
            if (m_trionicFile == null)
            {
                frmInfoBox info = new frmInfoBox("You should open a binary file first");
                return;
            }
            if (!m_trionicFile.HasSymbol(symbolname))
            {
                frmInfoBox info = new frmInfoBox(symbolname + " is not present in the current file");
                return;
            }
            // make it 100% self-defining
            // the application shouldn't have any know how about the details
            // it should only define TUNING-FLOW
            // have this work in online mode as well, this should enable the user to work in online mode in the SRAM 
            //itself when it is connected.
            
            if (!SRAMPanelExists(symbolname, sramfile)/* true*/)
            {

                dockManager1.BeginUpdate();
                DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
                dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                dp.Tag = sramfile;
                IMapViewer mv;
                if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                {
                    mv = new MapViewerEx();
                }
                else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                {
                    mv = new MapViewer();
                }
                else
                {
                    mv = new SimpleMapViewer();
                }
                mv.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                mv.GraphVisible = m_appSettings.ShowGraphs;

                mv.SetViewSize((Trionic5Tools.ViewSize)m_appSettings.DefaultViewSize);

                if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                    }
                }
                //mv.LoadSymbol(symbolname, m_trionicFile);
                mv.LoadSymbol(symbolname, m_trionicFile, sramfile);
                //mv.Map_content = m_trionicFile.ReadDataFromFile(sramfile, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                mv.OnlineMode = true;
                int cols = 1;
                int rows = 1;
                m_trionicFile.GetMapMatrixWitdhByName(symbolname, out cols, out rows);
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(symbolname));
                TryToAddOpenLoopTables(mv);
                mv.InitEditValues();

                mv.Dock = DockStyle.Fill;
                mv.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                // what todo on a save action (sram/binary)?
                mv.onSymbolSave += new IMapViewer.NotifySaveSymbol(mv_onSymbolSave);
                mv.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(mv_onReadFromSRAM);
                mv.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequestedSRAM);
                mv.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                mv.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                mv.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                //
                dp.Width = mv.DetermineWidth();
                dp.Text = "SRAM: " + Path.GetFileName(/*m_trionicFile.GetFileInfo().Filename*/ sramfile) + " [" + symbolname + "]"; 
                //if (_ecuConnection.Opened) dp.Text += " Online";
                dp.Controls.Add(mv);
                bool isDocked = false;
                if (m_appSettings.AutoDockSameSymbol)
                {
                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                    {
                        if (pnl.Text.Contains("[" + symbolname + "]") && pnl != dp && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                        {
                            dp.DockAsTab(pnl, 0);
                            //pnl.Options.ShowCloseButton = false;
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
                            if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dp && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                            {
                                dp.DockAsTab(pnl, 0);
                                //pnl.Options.ShowCloseButton = false;
                                isDocked = true;
                                break;
                            }
                        }
                    }
                }
                dockManager1.EndUpdate();
            }
            else
            {
                BringPanelToForeGround(symbolname);
            }
        }

        void mv_onAxisEditorRequestedSRAM(object sender, IMapViewer.AxisEditorRequestedEventArgs e)
        {
            // show axis from selected SRAM file
            string axisToStart = string.Empty;
            SymbolAxesTranslator sat = new SymbolAxesTranslator();
            if (e.Axisident == IMapViewer.AxisIdent.X_Axis)
            {
                axisToStart = sat.GetXaxisSymbol(e.Mapname);
            }
            else if (e.Axisident == IMapViewer.AxisIdent.Y_Axis)
            {
                axisToStart = sat.GetYaxisSymbol(e.Mapname);
            }
            if (axisToStart != string.Empty)
            {
                StartTableViewerSRAMFile(axisToStart, e.Filename);
            }
        }


        private void StartTableViewer(string symbolname)
        {
            if (m_trionicFile == null)
            {
                frmInfoBox info = new frmInfoBox("You should open a binary file first");
                return;
            }
            if(!m_trionicFile.HasSymbol(symbolname))
            {
                frmInfoBox info = new frmInfoBox(symbolname + " is not present in the current file");
                return;
            }
            
            // make it 100% self-defining
            // the application shouldn't have any know how about the details
            // it should only define TUNING-FLOW
            // have this work in online mode as well, this should enable the user to work in online mode in the SRAM 
            //itself when it is connected.
            if (m_appSettings.ShowAdditionalSymbolInformation)
            {
                ShowContextSensitiveHelpOnSymbol(symbolname);
            }

            if (m_appSettings.AutoHighlightSelectedMap)
            {
                try
                {
                    //gridViewSymbols.ActiveFilter.Clear(); // clear filter
                    if (gridSymbols.DataSource is Trionic5Tools.SymbolCollection)
                    {
                        Trionic5Tools.SymbolCollection dt = (Trionic5Tools.SymbolCollection)gridSymbols.DataSource;
                        int rtel = 0;
                        
                        foreach (Trionic5Tools.SymbolHelper dr in dt)
                        {

                            if (dr.Varname == symbolname)
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
                                    Application.DoEvents(); //<GS-15042010>
                                    if (gridViewSymbols.IsRowVisible(rhandle) == RowVisibleState.Hidden)
                                    {
                                        gridViewSymbols.ActiveFilter.Clear(); // clear filter
                                        //Do again!
                                        Application.DoEvents();
                                        dt = (Trionic5Tools.SymbolCollection)gridSymbols.DataSource;
                                        int rtel2 = 0;

                                        foreach (Trionic5Tools.SymbolHelper dr2 in dt)
                                        {

                                            if (dr2.Varname == symbolname)
                                            {
                                                rhandle = gridViewSymbols.GetRowHandle(rtel2);
                                                gridViewSymbols.OptionsSelection.MultiSelect = true;
                                                gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                                                gridViewSymbols.ClearSelection();
                                                gridViewSymbols.SelectRow(rhandle);
                                                gridViewSymbols.MakeRowVisible(rhandle, true);
                                                gridViewSymbols.FocusedRowHandle = rhandle;
                                            }
                                            rtel2++;
                                        }
                                    }
                                    //gridViewSymbols.SelectRange(rhandle, rhandle);
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
                catch (Exception highlightE)
                {
                    logger.Debug(highlightE.Message);
                }
            }

            if (symbolname == m_trionicFileInformation.GetProgramModeSymbol())
            {
                //<GS-20042010> make a special viewer for this!
                frmEasyFirmwareInfo info = new frmEasyFirmwareInfo();
                if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
                {
                    info.Pgm_mod = _ecuConnection.ReadSymbolData(symbolname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    info.SetPrimarySourceName("ECU " + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename));
                }
                else
                {
                    info.Pgm_mod = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetProgramModeSymbol()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol()));
                    info.SetPrimarySourceName("BIN " + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename));
                }
                if (m_trionicFileInformation.SRAMfilename != "")
                {
                    info.Pgm_mod2 = m_trionicFile.ReadDataFromFile(m_trionicFileInformation.SRAMfilename, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    info.SetSecondarySourceName("SRAM " + Path.GetFileNameWithoutExtension(m_trionicFileInformation.SRAMfilename));
                }
                else
                {
                    info.DisableSecondarySource();
                }
                info.ShowDialog();
            }
            else if (symbolname == m_trionicFileInformation.GetProgramStatusSymbol())
            {
                //TODO: <GS-20042010> make a special viewer for this!
            }
            else if (!PanelExists(symbolname))
            {
                dockManager1.BeginUpdate();
                DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
                dp.Tag = m_trionicFileInformation.Filename;
                dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                //IMapViewer mv = new MapViewerEx();
                IMapViewer mv;
                if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                {
                    mv = new MapViewerEx();
                }
                else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                {
                    mv = new MapViewer();
                }
                else
                {
                    mv = new SimpleMapViewer();
                } 
                mv.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                mv.GraphVisible = m_appSettings.ShowGraphs;
                // set viewsize
                mv.SetViewSize((Trionic5Tools.ViewSize)m_appSettings.DefaultViewSize);
                if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                    }
                }
                mv.LoadSymbol(symbolname, m_trionicFile);
                if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
                {
                    mv.Map_content = _ecuConnection.ReadSymbolData(symbolname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    mv.OnlineMode = true;
                }
                else if (m_trionicFileInformation.SRAMfilename != "" && !symbolname.Contains("!"))
                {
                    mv.Map_content = m_trionicFile.ReadDataFromFile(m_trionicFileInformation.SRAMfilename, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    mv.OnlineMode = true;
                }
                int cols = 1;
                int rows = 1;
                m_trionicFile.GetMapMatrixWitdhByName(symbolname, out cols, out rows);
                TryToAddOpenLoopTables(mv);
                mv.InitEditValues();

                mv.Dock = DockStyle.Fill;
                mv.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                // what todo on a save action (sram/binary)?
                mv.onSymbolSave += new IMapViewer.NotifySaveSymbol(mv_onSymbolSave);
                mv.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(mv_onReadFromSRAM);
                mv.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);
                mv.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                mv.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                mv.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);
                mv.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);
                int width = mv.DetermineWidth();
                if (dp.Width != width) dp.Width = width;
                dp.Text = Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + " [" + symbolname + "]"; 
                //if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline) dp.Text += " Online";
                dp.Controls.Add(mv);

                bool isDocked = false;
                if (m_appSettings.AutoDockSameSymbol)
                {
                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                    {
                        if (pnl.Text.Contains("[" + symbolname + "]") && pnl != dp && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                        {
                            dp.DockAsTab(pnl, 0);
                            //pnl.Options.ShowCloseButton = false;
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
                            if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dp && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                            {
                                dp.DockAsTab(pnl, 0);
                                //pnl.Options.ShowCloseButton = false;
                                isDocked = true;
                                break;
                            }
                        }
                    }
                }

                dockManager1.EndUpdate();

            }
            else
            {
                BringPanelToForeGround(symbolname);
            }

            // update all mapviewers with a dummy value for test
            // UpdateMapViewers(3000, 100, 1.6);

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

        void mv_onAxisEditorRequested(object sender, IMapViewer.AxisEditorRequestedEventArgs e)
        {
            // start Table viewer with selected axis
            string axisToStart = string.Empty;
            SymbolAxesTranslator sat = new SymbolAxesTranslator();
            if (e.Axisident == IMapViewer.AxisIdent.X_Axis)
            {
                axisToStart = sat.GetXaxisSymbol(e.Mapname);
            }
            else if (e.Axisident == IMapViewer.AxisIdent.Y_Axis)
            {
                axisToStart = sat.GetYaxisSymbol(e.Mapname);
            }
            if (axisToStart != string.Empty)
            {
                StartTableViewer(axisToStart);
            }
        }

        private void StartTableViewerFloating(string symbolname) // center screen?
        {
            bool _isAdjustmentMap = false;
            if (symbolname == "FuelAdjustmentMap")
            {
                symbolname = m_trionicFileInformation.GetInjectionMap();
                props = m_trionicFile.GetTrionicProperties();
                if (!props.IsTrionic55) symbolname = "Adapt_korr";
                _isAdjustmentMap = true;
                if (m_AFRMaps == null)
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }
                if (!m_AFRMaps.HasValidFuelmap)
                {
                    // insert the fuel map from SRAM now
                    byte[] fuelmap = _ecuConnection.ReadSymbolData(symbolname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    m_AFRMaps.SetCurrentFuelMap(fuelmap);
                }
                
            }
            if (m_trionicFile == null)
            {
                frmInfoBox info = new frmInfoBox("You should open a binary file first");
                return;
            }
            if (!m_trionicFile.HasSymbol(symbolname))
            {
                frmInfoBox info = new frmInfoBox(symbolname + " is not present in the current file");
                return;
            }
            // make it 100% self-defining
            // the application shouldn't have any know how about the details
            // it should only define TUNING-FLOW
            // have this work in online mode as well, this should enable the user to work in online mode in the SRAM 
            //itself when it is connected.
            if (!PanelExists(symbolname))
            {

                dockManager1.BeginUpdate();
                /*Rectangle r = Screen.GetWorkingArea(ctrlRealtime1);
                Point p = new Point(r.X + r.Width/2, r.Y + r.Height/2);
                System.Drawing.Point floatpoint = this.PointToScreen(p);*/
                System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                DockPanel dp = dockManager1.AddPanel(floatpoint);
                dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                dp.Tag = m_trionicFileInformation.Filename;
                //IMapViewer mv = new MapViewerEx();
                IMapViewer mv;
                if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                {
                    mv = new MapViewerEx();
                }
                else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                {
                    mv = new MapViewer();
                }
                else
                {
                    mv = new SimpleMapViewer();
                }
                mv.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                mv.GraphVisible = m_appSettings.ShowGraphs;
                mv.SetViewSize((Trionic5Tools.ViewSize)m_appSettings.DefaultViewSize);
                if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                    }
                }
                else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                {
                    if (m_appSettings.DefaultViewType == ViewType.Decimal)
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                    }
                    else
                    {
                        mv.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                    }
                }
                mv.LoadSymbol(symbolname, m_trionicFile);

                if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
                {
                    if (_isAdjustmentMap)
                    {
                        mv.Map_content = m_AFRMaps.GetCurrentlyMutatedFuelMap();
                    }
                    else
                    {
                        mv.Map_content = _ecuConnection.ReadSymbolData(symbolname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(symbolname), (uint)m_trionicFileInformation.GetSymbolLength(symbolname));
                    }

                    mv.OnlineMode = true;
                }
                int cols = 1;
                int rows = 1;
                m_trionicFile.GetMapMatrixWitdhByName(symbolname, out cols, out rows);
               /* if (m_trionicFile.GetMapSensorType() == MapSensorType.MapSensor30)
                {
                    mv.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                }*/
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(symbolname));
                mv.Dock = DockStyle.Fill;
                mv.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                // what todo on a save action (sram/binary)
                mv.onSymbolSave += new IMapViewer.NotifySaveSymbol(mv_onSymbolSave);
                mv.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(mv_onReadFromSRAM);
                mv.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);
                mv.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                mv.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                mv.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                TryToAddOpenLoopTables(mv);
                mv.InitEditValues();


                //
                dp.Width = mv.DetermineWidth();
                dp.Text = Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + " [" + symbolname + "]";
                //if (_ecuConnection.Opened && _APPmode == OperationMode.ModeOnline) dp.Text += " Online";
                dp.Controls.Add(mv);

                
                mv.GraphVisible = false;
                int dw = 650;
                if (mv.X_axisvalues.Length > 0)
                {
                    dw = 30 + ((mv.X_axisvalues.Length + 1) * 35);
                }
                if (dw < 400) dw = 400;
                dp.FloatSize = new Size(dw, 500);
 
                // now set it in the center of the screen!
                //int x = Screen.PrimaryScreen.WorkingArea.Width / 2 - dp.FloatSize.Width / 2;
                //int y = Screen.PrimaryScreen.WorkingArea.Height / 2 - dp.FloatSize.Height / 2;
                int x = this.Left + this.Width / 2 - dp.FloatSize.Width / 2;
                int y = this.Top + this.Height / 2 - dp.FloatSize.Height / 2;
                System.Drawing.Point realfloatpoint = new Point(x, y);//this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));
                dp.FloatLocation = realfloatpoint;

                dockManager1.EndUpdate();
            }
            else
            {
                BringPanelToForeGround(symbolname);
                //TODO: Fuel adjustment map should be data-updated!
                /*if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
                {
                    if (_isAdjustmentMap)
                    {
                        mv.Map_content = m_AFRMaps.GetCurrentlyMutatedFuelMap();
                    }
                }*/
            }
            // update all mapviewers with a dummy value for test
            // UpdateMapViewers(3000, 100, 1.6);

        }

        void mv_onReadFromSRAM(object sender, MapViewer.ReadFromSRAMEventArgs e)
        {
                // if the tag is a SRAM file, refresh from that
            // refresh the data in the mapviewer with data from the file/ecu
            byte[] _ecudata = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(e.Mapname), (uint)m_trionicFileInformation.GetSymbolLength(e.Mapname));

            if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
            {
                _ecuConnection.ProhibitRead = true;
                Thread.Sleep(100);
                _ecudata = _ecuConnection.ReadSymbolDataNoProhibitRead(e.Mapname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.Mapname), (uint)m_trionicFileInformation.GetSymbolLength(e.Mapname));
                _ecuConnection.ProhibitRead = false;
            }
            // reload the view with the appropriate data
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                int cols = 1;
                int rows = 1;
                if (mv.Filename.ToUpper().EndsWith("RAM"))
                {
                    _ecudata = m_trionicFile.ReadDataFromFile(mv.Filename, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.Mapname), (uint)m_trionicFileInformation.GetSymbolLength(e.Mapname));
                }

                mv.Map_content = _ecudata;// _ecuConnection.ReadSymbolData(e.SymbolName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), (uint)m_trionicFileInformation.GetSymbolLength(e.SymbolName));
                m_trionicFile.GetMapMatrixWitdhByName(e.Mapname, out cols, out rows);
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(e.Mapname));
                mv.InitEditValues();
            }
        }
        /*void mv_onReadFromSRAM(object sender, IMapViewer.ReadFromSRAMEventArgs e)
        {
            // refresh the data in the mapviewer with data from the file/ecu
            byte[] _ecudata = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(e.Mapname), (uint)m_trionicFileInformation.GetSymbolLength(e.Mapname));

            if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
            {
                _ecuConnection.ProhibitRead = true;
                Thread.Sleep(100);
                _ecudata = _ecuConnection.ReadSymbolData(e.Mapname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.Mapname), (uint)m_trionicFileInformation.GetSymbolLength(e.Mapname));
                _ecuConnection.ProhibitRead = false;
            }
            // reload the view with the appropriate data
            if (sender is IMapViewer)
            {
                IMapViewer mv = (MapViewerEx)sender;
                int cols = 1;
                int rows = 1;
                mv.Map_content = _ecudata;// _ecuConnection.ReadSymbolData(e.SymbolName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), (uint)m_trionicFileInformation.GetSymbolLength(e.SymbolName));
                m_trionicFile.GetMapMatrixWitdhByName(e.Mapname, out cols, out rows);
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(e.Mapname));
                mv.InitEditValues();
            }
        }*/

        void mv_onSymbolSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {

            // save data from a mapviewer... save to binary if available and to ECU if available
            string note = string.Empty;
            if (m_appSettings.RequestProjectNotes && m_CurrentWorkingProject != "")
            {
                //request a small note from the user in which he/she can denote a description of the change
                frmChangeNote changenote = new frmChangeNote();
                changenote.ShowDialog();
                note = changenote.Note;
            }

            if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
            {
                _ecuConnection.WriteSymbolData(m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), m_trionicFileInformation.GetSymbolLength(e.SymbolName), e.SymbolDate);
            }
            m_trionicFile.WriteData(e.SymbolDate, (uint)m_trionicFileInformation.GetSymbolAddressFlash(e.SymbolName), note);
            // reload the view with the appropriate data
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                int cols = 1;
                int rows = 1;
                mv.Map_content = e.SymbolDate;// _ecuConnection.ReadSymbolData(e.SymbolName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), (uint)m_trionicFileInformation.GetSymbolLength(e.SymbolName));
                m_trionicFile.GetMapMatrixWitdhByName(e.SymbolName, out cols, out rows);
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(e.SymbolName));
            }


        }
        /*void mv_onSymbolSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            // save data from a mapviewer... save to binary if available and to ECU if available
            if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline)
            {
                _ecuConnection.WriteSymbolData(m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), m_trionicFileInformation.GetSymbolLength(e.SymbolName), e.SymbolDate);
            }
            m_trionicFile.WriteData(e.SymbolDate, (uint)m_trionicFileInformation.GetSymbolAddressFlash(e.SymbolName));
            // reload the view with the appropriate data
            if (sender is MapViewer)
            {
                MapViewer mv = (MapViewer)sender;
                int cols = 1;
                int rows = 1;
                mv.Map_content = e.SymbolDate;// _ecuConnection.ReadSymbolData(e.SymbolName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), (uint)m_trionicFileInformation.GetSymbolLength(e.SymbolName));
                m_trionicFile.GetMapMatrixWitdhByName(e.SymbolName, out cols, out rows);
                mv.ShowTable(cols, m_trionicFile.IsTableSixteenBits(e.SymbolName));
            }
        }
        */

        #endregion

        #region Event handlers

        private void btnOpenFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //CloseProject();
            OpenWorkingFile();
        }

        private void gridSymbols_DoubleClick(object sender, EventArgs e)
        {

        }

        void OnCloseMapViewer(object sender, EventArgs e)
        {
            if (sender is IMapViewer)
            {
                IMapViewer tabdet = (IMapViewer)sender;
                //close the parent if it is a dockpanel
                if (tabdet.Parent.Parent is DockPanel)
                {
                    DockPanel dp = (DockPanel)tabdet.Parent.Parent;
                    dockManager1.RemovePanel(dp);
                }
                else
                {
                    string dockpanelname = Path.GetFileName(tabdet.Filename) + " [" + tabdet.Map_name + "]"; 
                    foreach (DevExpress.XtraBars.Docking.DockPanel dp in dockManager1.Panels)
                    {
                        if (dp.Text == dockpanelname)
                        {
                            dockManager1.RemovePanel(dp);
                            break;
                        }
                        else if (dp.Text == "Symbol difference: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]")
                        {
                            dockManager1.RemovePanel(dp);
                            break;
                        }
                        else if (dp.Text == "Symbol: " + tabdet.Map_name + " [" + tabdet.Filename + "]")
                        {
                            dockManager1.RemovePanel(dp);
                            break;
                        }
                    }
                }
            }
        }

        private void SetDefaultFilters()
        {
            if (m_appSettings.ShowAddressesInHex)
            {
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(@"([Flash_start_address] <> '000000')", "Only symbols within binary");
                gridViewSymbols.ActiveFilter.Clear();
                gridViewSymbols.ActiveFilter.Add(gcSymbolFlash, fltr);
                gridViewSymbols.ActiveFilterEnabled = true;
            }
            else
            {
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(@"([Flash_start_address] > 0)", "Only symbols within binary");
                gridViewSymbols.ActiveFilter.Clear();
                gridViewSymbols.ActiveFilter.Add(gcSymbolFlash, fltr);
                gridViewSymbols.ActiveFilterEnabled = true;
            }
        }

        private void gridViewSymbols_DoubleClick(object sender, EventArgs e)
        {
            // test if we hit a datarow
            Point p = gridSymbols.PointToClient(Cursor.Position);
            GridHitInfo hitinfo = gridViewSymbols.CalcHitInfo(p);
            int[] selectedrows = gridViewSymbols.GetSelectedRows();
            if (hitinfo.InRow)
            {
                int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridViewSymbols.GroupCount)
                {
                    //logger.Debug("In row");
                    if (gridViewSymbols.GetFocusedRow() is Trionic5Tools.SymbolHelper)
                    {
                        Trionic5Tools.SymbolHelper sh = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetFocusedRow();
                        //logger.Debug("Symbol:" + sh.Varname);
                        if (!_ecuConnection.Opened && sh.Flash_start_address == 0/*!sh.Varname.Contains("!")*/ && m_trionicFileInformation.SRAMfilename == "")
                        {
                                frmInfoBox info = new frmInfoBox("Symbol resides in SRAM and you are in offline mode. T5Suite is unable to fetch this symboldata in offline mode");

                        }
                        else
                        {
                            StartTableViewer(sh.Varname);
                        }
                    }
                }
            }

        }


        private bool StartECUConnection()
        {
            bool retval = false;
            // user wants to connect to an ECU
            // while connected, disable this button
            //_ecuConnection = new ECUConnection();
            
            /*if (m_appSettings.DebugMode && Environment.MachineName == "PC-GUIDO") //<GS-29032010>
            {
                // set from binary sram dump in stead of live data
                _ecuConnection.SramDumpFile = Application.StartupPath + "\\workbin.RAM";
                logger.Debug("Loaded working.RAM");
            }*/
            btnSwitchMode.Caption = "Connecting...";
            Application.DoEvents();
            if (m_appSettings.CanDevice == "Lawicel")
            {
                CheckCanwakeup();
            }
            _ecuConnection.AppSettings = m_appSettings;
            _ecuConnection.OpenECUConnection();
            if (_ecuConnection.Opened)
            {
                if (m_trionicFileInformation != null)
                {
                    if (m_trionicFileInformation.Filelength == 0x20000)
                    {
                        _ecuConnection.IsT52 = true;
                    }
                    else
                    {
                        _ecuConnection.IsT52 = false;
                    }
                }

                retval = true;
                _ECUmode = OperationMode.ModeOnline;
                SetOnlineButtons(true);

                string swversion = _ecuConnection.GetSoftwareVersion();
                btnSwitchMode.Caption = "Connected: " + swversion;
                Application.DoEvents();
                logger.Debug("SW version: " + swversion);
                if (swversion == "")
                {
                    // special case, no ECU connected
                    retval = false;
                    btnSwitchMode.Caption = "Not connected";
                    _ecuConnection.CloseECUConnection(true);
                    _ECUmode = OperationMode.ModeOffline;
                    _APPmode = OperationMode.ModeOffline;
                    SetOnlineButtons(false);
                    Application.DoEvents();
                    return retval;
                }
                if (m_trionicFile != null)
                {
                    if (m_trionicFile.GetSoftwareVersion() != swversion)
                    {
                        SetStatusText("Getting symboltable...");
                        DataTable _symboltable = _ecuConnection.GetSymbolTable();
                        //_symboltable.WriteXml(Application.StartupPath + "\\" + _ecuConnection.Swversion + ".xml");

                        FillRealtimePool(RealtimeMonitoringType.Fuel, true);
                        _ecuConnection.StopECUMonitoring();
                        Thread.Sleep(50);
                        FillRealtimePool(RealtimeMonitoringType.Fuel, true);

                        _ecuConnection.StartECUMonitoring();// start monitoring, it's for free!
                        SetStatusText("Monitoring...");

                    }
                }
                else
                {
                    SetStatusText("Getting symboltable...");
                    DataTable _symboltable = _ecuConnection.GetSymbolTable();
                    //_symboltable.WriteXml(Application.StartupPath + "\\" + _ecuConnection.Swversion + ".xml");
                    FillRealtimePool(RealtimeMonitoringType.Fuel, true);
                    _ecuConnection.StopECUMonitoring();
                    Thread.Sleep(50);
                    SetStatusText("Idle");
                }
                barStaticItem1.Caption = " ECU: " + swversion;
                barStaticItem1.Enabled = true;
                // clear default filters to allow access to sram symbols as well.
                gridViewSymbols.ActiveFilterEnabled = false;
                // check for counter values
                Thread.Sleep(100);
                DateTime dt_ecu = _ecuConnection.GetMemorySyncDate();
                Thread.Sleep(100);
                DateTime dt_file = m_trionicFile.GetMemorySyncDate();
                DateTime now = DateTime.Now;
                if (dt_ecu.Year == 2000 && dt_ecu.Month == 1 && dt_ecu.Day == 1 && dt_ecu.Hour == 0 && dt_ecu.Minute == 0 && dt_ecu.Second == 0)
                {
                    dt_ecu = now;
                    _ecuConnection.SetMemorySyncDate(dt_ecu);
                }
                if (dt_file.Year == 2000 && dt_file.Month == 1 && dt_file.Day == 1 && dt_file.Hour == 0 && dt_file.Minute == 0 && dt_file.Second == 0)
                {
                    dt_file = now;
                    //_ecuConnection.SetMemorySyncDate(dt_ecu);
                    m_trionicFile.SetMemorySyncDate(dt_file);
                }
                if (dt_ecu > dt_file)
                {
                    logger.Debug("Sync when start ECU connection: ECU to bin");
                    frmSyncFileECU syncdlg = new frmSyncFileECU();
                    syncdlg.SetTimeStamps(dt_file, dt_ecu);
                    syncdlg.SetInformation("Proposed sync: ECU to binary");
                    DialogResult dr = syncdlg.ShowDialog();
                    _syncAskedForECUConnect = true;

                    if (dr == DialogResult.OK)
                    {
                        // sync ecu to file without changing sync dates
                        SyncMaps(SyncOption.ToFile);
                        m_trionicFile.SetMemorySyncDate(dt_ecu);
                    }
                    else if (dr == DialogResult.Retry)
                    {
                        SyncMaps(SyncOption.ToECU);
                        _ecuConnection.SetMemorySyncDate(dt_file);
                    }
                }
                else if (dt_file > dt_ecu)
                {
                    logger.Debug("Sync when start ECU connection: bin to ECU");
                    frmSyncFileECU syncdlg = new frmSyncFileECU();
                    syncdlg.SetTimeStamps(dt_file, dt_ecu);
                    syncdlg.SetInformation("Proposed sync: binary to ECU");
                    DialogResult dr = syncdlg.ShowDialog();
                    _syncAskedForECUConnect = true;
                    if (dr == DialogResult.OK)
                    {
                        // sync file to ecu without changing sync dates
                        SyncMaps(SyncOption.ToECU);
                        _ecuConnection.SetMemorySyncDate(dt_file);
                    }
                    else if (dr == DialogResult.Retry)
                    {
                        SyncMaps(SyncOption.ToFile);
                        m_trionicFile.SetMemorySyncDate(dt_ecu);
                    }
                }
            }
            return retval;
        }

        private void StopECUConnection()
        {
            // close ecu connection, so also revert back to offline mode if we were in online mode
            if (_ECUmode == OperationMode.ModeOnline)
            {
                _ECUmode = OperationMode.ModeOffline; // we're offline
                UpdateOnlineOffLineTexts();
                SetOnlineButtons(false);
                // remove the fullscreen panel with the online stuff 
                // and return to normal mode
                dockRealtime.Visibility = DockVisibility.Hidden;
                
//                dockSymbols.Visibility = DockVisibility.Visible;
                SetModeAndFilters();
                // never show symbollist in online mode?
                _ecuConnection.StopECUMonitoring();
                Thread.Sleep(50);
            }
            _ecuConnection.StopECUMonitoring();
            Thread.Sleep(50);
            _ecuConnection.CloseECUConnection(false);
            _ecuConnection.SramDumpFile = string.Empty;
            barStaticItem1.Caption = " ECU: not connected";
            barStaticItem1.Enabled = false;
            //SetDefaultFilters();
        }

        private void btnConnectECU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_ecuConnection.Opened)
            {
                StartECUConnection();
                
                btnConnectECU.Caption = "Disconnect ECU";
                if (!_ecuConnection.Opened)
                {
                    btnConnectECU.Caption = "Connect ECU";
                    //btnSwitchMode.Caption = "Go online";
                    //barStaticItem5.Caption = "Mode: offline";
                    _ECUmode = OperationMode.ModeOffline;
                    SetOnlineButtons(false);
                    UpdateOnlineOffLineTexts();
                    Application.DoEvents();
                    frmInfoBox info = new frmInfoBox("Failed to open canbus connection!");
                }
            }
            else
            {
                //<GS-25032010>
                if (btnConnectECU.Caption == "Connect ECU")
                {
                    //StartECUConnection();
                    btnConnectECU.Caption = "Disonnect ECU";
                    _ECUmode = OperationMode.ModeOnline;
                    SetOnlineButtons(true);

                    UpdateOnlineOffLineTexts();
                    // switch mode
                }
                else
                {
                    //StopECUConnection();
                    StopOnlineMode();
                    _ECUmode = OperationMode.ModeOffline;
                    SetOnlineButtons(false);
                    UpdateOnlineOffLineTexts();
                    btnConnectECU.Caption = "Connect ECU";
                }
            }
        }

        private void CheckCanwakeup()
        {
            if (!_connectionWasOpenedBefore)
            {
                string Exename = Path.Combine(Application.StartupPath, "WakeupCANbus.exe");
                // see if we can spawn the exe called WakeupCANbus.exe
                if (File.Exists(Exename))
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
                    startinfo.CreateNoWindow = true;
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //startinfo.UseShellExecute = false;
                    startinfo.WorkingDirectory = Application.StartupPath;
                    logger.Debug("Spawning WakeupCANbus.exe");
                    System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                    conv_proc.WaitForExit(10000); // wait for 30 seconds max
                    if (!conv_proc.HasExited)
                    {
                        logger.Debug("Killing WakeupCANbus.exe");
                        conv_proc.Kill();
                    }
                    _connectionWasOpenedBefore = true;
                }
            }
        }

        private void StartOnlineMode()
        {
            if (!_ecuConnection.Opened)
            {
                StartECUConnection();
            }
            /*if (Environment.MachineName == "PC-GUIDO" && _ecuConnection.Opened)
            {
                _ecuConnection.SetMemorySyncDate(DateTime.Now);
            }*/
            if (_ecuConnection.Opened && _APPmode == OperationMode.ModeOffline)
            {
                ctrlRealtime1.AppSettings = m_appSettings;
                ctrlRealtime1.WideBandAFRSymbol = m_appSettings.WidebandLambdaSymbol;
                ctrlRealtime1.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
                ctrlRealtime1.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
                ctrlRealtime1.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                ctrlRealtime1.CellStableTime_ms = m_appSettings.CellStableTime_ms;
                ctrlRealtime1.CorrectionPercentage = m_appSettings.CorrectionPercentage;
                ctrlRealtime1.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
                ctrlRealtime1.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
                ctrlRealtime1.EnrichmentFilter = m_appSettings.EnrichmentFilter;
                ctrlRealtime1.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
                ctrlRealtime1.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
                ctrlRealtime1.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
                ctrlRealtime1.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
                ctrlRealtime1.AutoLoggingEnabled = m_appSettings.AutoLoggingEnabled;
                ctrlRealtime1.AutoLogStartSign = m_appSettings.AutoLogStartSign;
                ctrlRealtime1.AutoLogStartValue = m_appSettings.AutoLogStartValue;
                ctrlRealtime1.AutoLogStopSign = m_appSettings.AutoLogStopSign;
                ctrlRealtime1.AutoLogStopValue = m_appSettings.AutoLogStopValue;
                ctrlRealtime1.AutoLogTriggerStartSymbol = m_appSettings.AutoLogTriggerStartSymbol;
                ctrlRealtime1.AutoLogTriggerStopSymbol = m_appSettings.AutoLogTriggerStopSymbol;

                ctrlRealtime1.MapSensor = m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType);
                ctrlRealtime1.Fuelxaxis = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetInjectionMap());
                ctrlRealtime1.Fuelyaxis = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetInjectionMap());
                ctrlRealtime1.Ignitionxaxis = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetIgnitionMap());
                ctrlRealtime1.Ignitionyaxis = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetIgnitionMap());
                _APPmode = OperationMode.ModeOnline; // we're in online mode
                UpdateOnlineOffLineTexts();
                SetOnlineButtons(true);
                // start a new panel that fills everything with tabs like the dashboard which enables users to 
                // monitor, edit and save data and logs (sessions) until they decide to go offline again
                dockSymbols.Visibility = DockVisibility.AutoHide;
                dockSymbols.HideImmediately();
                
                // never show symbollist in online mode?
                // also minimize the menu in this mode
                ribbonControl1.Minimized = true;
                //TODO: <GS-06042010> set realtime panel floating if screen resolution is very high to make 
                // the gauge more readable
                /*if (IsHighResolutionScreen())
                {
                    // make the panel floating!
                }*/
                ctrlRealtime1.EnableAdvancedMode = m_appSettings.EnableAdvancedMode;
                dockRealtime.Visibility = DockVisibility.Visible;

                ctrlRealtime1.UpdateProgramModeButtons(_ecuConnection.ReadSymbolData("Pgm_mod!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Pgm_mod!"), (uint)m_trionicFileInformation.GetSymbolLength("Pgm_mod!")));
                dockRealtime.Width = dockManager1.Form.ClientSize.Width - 20; //?
                // and set correct tabpage

                FillRealtimePool(RealtimeMonitoringType.Fuel, true);
                Thread.Sleep(100);
                DateTime dt_ecu = _ecuConnection.GetMemorySyncDate();
                Thread.Sleep(100);
                DateTime dt_file = m_trionicFile.GetMemorySyncDate();
                DateTime now = DateTime.Now;
                if (dt_ecu.Year == 2000 && dt_ecu.Month == 1 && dt_ecu.Day == 1 && dt_ecu.Hour == 0 && dt_ecu.Minute == 0 && dt_ecu.Second == 0)
                {
                    dt_ecu = now;
                    _ecuConnection.SetMemorySyncDate(dt_ecu);
                }
                if (dt_file.Year == 2000 && dt_file.Month == 1 && dt_file.Day == 1 && dt_file.Hour == 0 && dt_file.Minute == 0 && dt_file.Second == 0)
                {
                    dt_file = now;
                    //_ecuConnection.SetMemorySyncDate(dt_ecu);
                    m_trionicFile.SetMemorySyncDate(dt_file);
                }
                if (!_syncAskedForECUConnect)
                {
                    if (dt_ecu > dt_file)
                    {
                        logger.Debug("Sync when start online mode: ECU to bin");
                        frmSyncFileECU syncdlg = new frmSyncFileECU();
                        syncdlg.SetTimeStamps(dt_file, dt_ecu);
                        syncdlg.SetInformation("Proposed sync: ECU to binary");
                        DialogResult dr = syncdlg.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            // sync ecu to file without changing sync dates
                            SyncMaps(SyncOption.ToFile);
                            m_trionicFile.SetMemorySyncDate(dt_ecu);
                        }
                        else if (dr == DialogResult.Retry)
                        {
                            SyncMaps(SyncOption.ToECU);
                            _ecuConnection.SetMemorySyncDate(dt_file);
                        }
                    }
                    else if (dt_file > dt_ecu)
                    {
                        logger.Debug("Sync when start online mode: bin to ECU");
                        frmSyncFileECU syncdlg = new frmSyncFileECU();
                        syncdlg.SetTimeStamps(dt_file, dt_ecu);
                        syncdlg.SetInformation("Proposed sync: binary to ECU");
                        DialogResult dr = syncdlg.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            // sync file to ecu without changing sync dates
                            SyncMaps(SyncOption.ToECU);
                            _ecuConnection.SetMemorySyncDate(dt_file);
                        }
                        else if (dr == DialogResult.Retry)
                        {
                            SyncMaps(SyncOption.ToFile);
                            m_trionicFile.SetMemorySyncDate(dt_ecu);
                        }
                    }
                }
                _syncAskedForECUConnect = false;

                // realtime always starts in fuel mode
                _ecuConnection.StartECUMonitoring();
            }
            else if (_ecuConnection.Opened && _APPmode == OperationMode.ModeOnline)
            {
                // dan?
                StopOnlineMode();
                _APPmode = OperationMode.ModeOffline;
                barStaticItem5.Caption = "Mode: online";
                btnSwitchMode.Caption = "Go offline";
                UpdateOnlineOffLineTexts();
                SetOnlineButtons(true);
            }
            else if(!_ecuConnection.Opened)
            {
                btnSwitchMode.Caption = "Go online";
                barStaticItem5.Caption = "Mode: offline";
                _APPmode = OperationMode.ModeOffline;
                _ECUmode = OperationMode.ModeOffline;
                SetOnlineButtons(false);
                UpdateOnlineOffLineTexts();
                Application.DoEvents();
                frmInfoBox info = new frmInfoBox("Failed to open canbus connection!");
            }
        }

        private void SetOnlineButtons(bool enabled)
        {
            btnSynchronizeMaps.Enabled = enabled;
            btnWriteLogMarker.Enabled = enabled;
            btnClearKnockCounters.Enabled = enabled;
            btnDownloadFlash.Enabled = enabled;
            btnUploadFlash.Enabled = enabled;
            barButtonItem5.Enabled = enabled;
            btnUploadSRAMToECU.Enabled = enabled;
            btnCompareECUWithBinary.Enabled = enabled;
            btnReadCANUSB.Enabled = enabled;
            btnWriteCANUSB.Enabled = enabled;
            btnSRAMSnapshotCANUSB.Enabled = enabled;
            btnErrorCodes.Enabled = enabled;
        }

        private void UpdateOnlineOffLineTexts()
        {
            if (_APPmode == OperationMode.ModeOffline)
            {
                barStaticItem5.Caption = "Mode: offline";
                btnSwitchMode.Caption = "Go online";
            }
            else
            {
                if (_ECUmode == OperationMode.ModeOffline)
                {
                    // dan zou alles offline moeten
                    if (_ecuConnection.Opened)
                    {
                        _ECUmode = OperationMode.ModeOnline;
                        SetOnlineButtons(true);
                    }
                }
                barStaticItem5.Caption = "Mode: online";
                btnSwitchMode.Caption = "Go offline";
            }
            if (_ECUmode == OperationMode.ModeOffline)
            {
                btnConnectECU.Caption = "Connect ECU";
            }
            else
            {
                btnConnectECU.Caption = "Disconnect";
            }
                
        }

        private void StopOnlineMode()
        {
            //switch off autotune if it was active
            ctrlRealtime1.SwitchOffAutoTune();
            ctrlRealtime1.SwitchOffAutoTuneIgnition();

            _APPmode = OperationMode.ModeOffline; // we're offline
            UpdateOnlineOffLineTexts();
            SetOnlineButtons(true);
            // remove the fullscreen panel with the online stuff 
            // and return to normal mode
            dockRealtime.Visibility = DockVisibility.Hidden;
            //dockSymbols.Visibility = DockVisibility.Visible;
            SetModeAndFilters();
            // never show symbollist in online mode?
            ribbonControl1.Minimized = false;
            _ecuConnection.StopECUMonitoring();
            Application.DoEvents();
            SetStatusText("Getting knock counter snapshot");
            //<GS-09022011> get a snapshot of the knock counter map if that is required
            if (m_appSettings.KnockCounterSnapshot && props.IsTrionic55) // only for T5.5
            {
                try
                {
                    uint knockmapAddress = (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Knock_count_map");
                    if (knockmapAddress > 0)
                    {
                        byte[] knockcountmap = _ecuConnection.ReadSymbolData("Knock_count_map", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Knock_count_map"), (uint)m_trionicFileInformation.GetSymbolLength("Knock_count_map"));
                        // write the data into the bin folder in snapshots
                        if (knockcountmap.Length == 576) // only if it has a valid length
                        {
                            string filename = string.Empty;
                            if (m_CurrentWorkingProject != "")
                            {
                                if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots");
                                filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots\\Knockmap" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".KNK";

                            }
                            else
                            {
                                string folder = Path.Combine(Path.GetDirectoryName(m_trionicFileInformation.Filename), "Snapshots");
                                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                                filename = folder + "\\Knockmap" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".KNK";
                            }
                            if (filename != string.Empty)
                            {
                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }
                                using (StreamWriter sw = new StreamWriter(filename, true))
                                {
                                    foreach (byte b in knockcountmap)
                                    {
                                        sw.Write(b.ToString("X2"));
                                    }
                                }
                            }
                        }
                        LoadKnockMaps();
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
            SetStatusText("Idle");
            Thread.Sleep(50);
        }

        private void LoadKnockMaps()
        {
            string folder = string.Empty;
            if (m_CurrentWorkingProject != "")
            {
                folder = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots";
            }
            else
            {
                folder = Path.Combine(Path.GetDirectoryName(m_trionicFileInformation.Filename), "Snapshots");
            }
            if (Directory.Exists(folder))
            {
                string[] knockmaps = Directory.GetFiles(folder, "*.KNK");
                if (knockmaps.Length > 0) btnShowKnockCounterMaps.Enabled = true;
                else btnShowKnockCounterMaps.Enabled = false;
            }
            else
            {
                btnShowKnockCounterMaps.Enabled = false;
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_APPmode == OperationMode.ModeOffline)
            {
                StartOnlineMode();
               // _APPmode = OperationMode.ModeOnline;
               // UpdateOnlineOffLineTexts();
            }
            else if (_APPmode == OperationMode.ModeOnline)
            {
                StopOnlineMode();
                //_APPmode = OperationMode.ModeOffline;
                //UpdateOnlineOffLineTexts();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // preload of mapviewer to speed up loading later on
                //MapViewer mv = new MapViewer();
                //mv.Dispose();
                IMapViewer mvex = new MapViewerEx();
                mvex.Dispose();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            InitSkins();
            SetAdditionalHelpPanelSize();
            SetOnlineButtons(false);
            LoadMyMaps();
        }

        private void SetAdditionalHelpPanelSize()
        {
            try
            {
                if (!m_appSettings.ShowAdditionalSymbolInformation)
                {
                    splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
                    //splitContainerControl1.SplitterPosition = 0;
                    //splitContainerControl1.Panel2.Height = 0;
                }
                else
                {
                    splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
                    //splitContainerControl1.SplitterPosition = 110;
                    //splitContainerControl1.Panel2.Height = 110;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void UploadFlashCANUSB()
        {
            if (_ecuConnection.Opened)
            {
                //if (!_ecuConnection.EngineRunning)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Flash files|*.bin";
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        frmECUTypeSelection typesel = new frmECUTypeSelection();
                        if (typesel.ShowDialog() == DialogResult.OK)
                        {
                            _ecuConnection.ProgramFlash(ofd.FileName, typesel.GetECUType());
                        }
                        
                    }
                }
                /*else
                {
                    frmInfoBox info = new frmInfoBox("You cannot flash the ECU while the engine is running!");
                }*/
            }
        }

        private void btnUploadFlash_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UploadFlashCANUSB();
        }

        #endregion

        #region Program settings screen

        private void SetFilterMode()
        {
            if (m_appSettings.ShowAddressesInHex)
            {

                gcSymbolFlash.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolFlash.DisplayFormat.FormatString = "X6";
                gcSymbolFlash.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolSRAM.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolSRAM.DisplayFormat.FormatString = "X6";
                gcSymbolSRAM.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            else
            {
                gcSymbolFlash.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolFlash.DisplayFormat.FormatString = "";
                gcSymbolFlash.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolSRAM.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolSRAM.DisplayFormat.FormatString = "";
                gcSymbolSRAM.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
            }
            SetDefaultFilters();
        }

        private void btnSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show the options screen
            // should only hold the most relevant options from T5Suite 1.xx

            frmSettings set = new frmSettings();
            set.AppSettings = m_appSettings;
            set.OneLogForAllTypes = m_appSettings.OneLogForAllTypes;
            set.OneLogPerTypePerDay = m_appSettings.OneLogPerTypePerDay;
            set.InterpolateLogWorksTimescale = m_appSettings.InterpolateLogWorksTimescale;
            set.AutoHighlightSelectedMap = m_appSettings.AutoHighlightSelectedMap;
            set.ShowAdditionalSymbolInformation = m_appSettings.ShowAdditionalSymbolInformation;
            set.RequestProjectNotes = m_appSettings.RequestProjectNotes;
            //set.UseNewMapViewer = m_appSettings.UseNewMapViewer;
            set.MapViewerType = m_appSettings.MapViewerType;
            set.DirectSRAMWriteOnSymbolChange = m_appSettings.DirectSRAMWriteOnSymbolChange;
            set.PlayKnockSound = m_appSettings.PlayKnockSound;
            set.AutoOpenLogFile = m_appSettings.AutoOpenLogFile;
            set.AutoSizeNewWindows = m_appSettings.AutoSizeNewWindows;
            set.AutoSizeColumnsInViewer = m_appSettings.AutoSizeColumnsInWindows;
            set.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            set.TemperatureInFahrenheit = m_appSettings.TemperaturesInFahrenheit;
            set.AutoGenerateLogWorksFile = m_appSettings.AutoGenerateLogWorks;
            set.HideSymbolWindow = m_appSettings.HideSymbolTable;
            set.ShowGraphsInMapViewer = m_appSettings.ShowGraphs;
            set.UseRedAndWhiteMaps = m_appSettings.ShowRedWhite;
            set.ViewTablesInHex = m_appSettings.Viewinhex;
            set.AutoDockSameFile = m_appSettings.AutoDockSameFile;
            set.AutoDockSameSymbol = m_appSettings.AutoDockSameSymbol;
            set.DisableMapviewerColors = m_appSettings.DisableMapviewerColors;
            set.NewPanelsFloating = m_appSettings.NewPanelsFloating;
            set.AutoLoadLastFile = m_appSettings.AutoLoadLastFile;
            set.DefaultViewType = m_appSettings.DefaultViewType;
            set.DefaultViewSize = m_appSettings.DefaultViewSize;
            set.SynchronizeMapviewers = m_appSettings.SynchronizeMapviewers;
            set.FancyDocking = m_appSettings.FancyDocking;
            set.UseWidebandLambdaThroughSymbol = m_appSettings.UseWidebandLambdaThroughSymbol;
            set.WideBandLambdaSymbol = m_appSettings.WidebandLambdaSymbol;
            set.WidebandLambdaLowVoltage = m_appSettings.WidebandLowVoltage;
            set.WidebandLambdaHighVoltage = m_appSettings.WidebandHighVoltage;
            set.WidebandLambdaLowAFR = m_appSettings.WidebandLowAFR;
            set.WidebandLambdaHighAFR = m_appSettings.WidebandHighAFR;
            set.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
            set.EnableCanLogging = m_appSettings.EnableCanLogging;
            set.EnableAdvancedMode = m_appSettings.EnableAdvancedMode;
            set.CanUSBDevice = m_appSettings.CanDevice;
            set.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
            set.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
            set.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
            set.CellStableTime_ms = m_appSettings.CellStableTime_ms;
            set.IgnitionCellStableTime_ms = m_appSettings.IgnitionCellStableTime_ms;
            set.MinimumEngineSpeedForIgnitionTuning = m_appSettings.MinimumEngineSpeedForIgnitionTuning;
            set.MaximumIgnitionAdvancePerSession = m_appSettings.MaximumIgnitionAdvancePerSession;

            set.IgnitionAdvancePerCycle = m_appSettings.IgnitionAdvancePerCycle;
            set.IgnitionRetardFirstKnock = m_appSettings.IgnitionRetardFirstKnock;
            set.IgnitionRetardFurtherKnocks = m_appSettings.IgnitionRetardFurtherKnocks;
            set.GlobalMaximumIgnitionAdvance = m_appSettings.GlobalMaximumIgnitionAdvance;

            set.CorrectionPercentage = m_appSettings.CorrectionPercentage;
            set.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
            set.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
            set.DisableClosedLoopOnStartAutotune = m_appSettings.DisableClosedLoopOnStartAutotune;
            set.PlayCellProcessedSound = m_appSettings.PlayCellProcessedSound;
            set.ResetFuelTrims = m_appSettings.ResetFuelTrims;
            set.AllowIdleAutoTune = m_appSettings.AllowIdleAutoTune;
            set.CapIgnitionMap = m_appSettings.CapIgnitionMap;
            set.EnrichmentFilter = m_appSettings.EnrichmentFilter;
            set.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
            set.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
            set.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
            set.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
            set.UseEasyTrionicOptions = m_appSettings.UseEasyTrionicOptions;
            set.AutoDetectMapsensorType = m_appSettings.AutoDetectMapsensorType;
            set.AlwaysCreateAFRMaps = m_appSettings.AlwaysCreateAFRMaps;
            set.KnockCounterSnapshot = m_appSettings.KnockCounterSnapshot;
            set.AutoLoggingEnabled = m_appSettings.AutoLoggingEnabled;
            set.AutoLogStartSign = m_appSettings.AutoLogStartSign;
            set.AutoLogStartValue = m_appSettings.AutoLogStartValue;
            set.AutoLogStopSign = m_appSettings.AutoLogStopSign;
            set.AutoLogStopValue = m_appSettings.AutoLogStopValue;
            set.AutoLogTriggerStartSymbol = m_appSettings.AutoLogTriggerStartSymbol;
            set.AutoLogTriggerStopSymbol = m_appSettings.AutoLogTriggerStopSymbol;
            if (m_trionicFileInformation != null)
            {
                set.Symbols = m_trionicFileInformation.SymbolCollection;
            }
            set.ProjectFolder = m_appSettings.ProjectFolder;
                
            if (set.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.InterpolateLogWorksTimescale = set.InterpolateLogWorksTimescale;
                m_appSettings.AutoOpenLogFile = set.AutoOpenLogFile;
                m_appSettings.PlayKnockSound = set.PlayKnockSound;
                m_appSettings.OneLogForAllTypes = set.OneLogForAllTypes;
                m_appSettings.OneLogPerTypePerDay = set.OneLogPerTypePerDay;
                m_appSettings.CapIgnitionMap = set.CapIgnitionMap;
                m_appSettings.AllowIdleAutoTune = set.AllowIdleAutoTune;
                m_appSettings.ResetFuelTrims = set.ResetFuelTrims;
                m_appSettings.PlayCellProcessedSound = set.PlayCellProcessedSound;
                m_appSettings.DirectSRAMWriteOnSymbolChange = set.DirectSRAMWriteOnSymbolChange;
                m_appSettings.AutoSizeNewWindows = set.AutoSizeNewWindows;
                m_appSettings.AutoSizeColumnsInWindows = set.AutoSizeColumnsInViewer;
                m_appSettings.TemperaturesInFahrenheit = set.TemperatureInFahrenheit;
                m_appSettings.AutoChecksum = set.AutoUpdateChecksum;
                m_appSettings.AutoGenerateLogWorks = set.AutoGenerateLogWorksFile;
                m_appSettings.HideSymbolTable = set.HideSymbolWindow;
                m_appSettings.ShowGraphs = set.ShowGraphsInMapViewer;
                m_appSettings.ShowRedWhite = set.UseRedAndWhiteMaps;
                m_appSettings.Viewinhex = set.ViewTablesInHex;
                m_appSettings.DisableMapviewerColors = set.DisableMapviewerColors;
                m_appSettings.AutoDockSameFile = set.AutoDockSameFile;
                m_appSettings.AutoDockSameSymbol = set.AutoDockSameSymbol;
                m_appSettings.NewPanelsFloating = set.NewPanelsFloating;
                m_appSettings.EnableAdvancedMode = set.EnableAdvancedMode;

                ctrlRealtime1.EnableAdvancedMode = set.EnableAdvancedMode;

                m_appSettings.DefaultViewType = set.DefaultViewType;
                m_appSettings.DefaultViewSize = set.DefaultViewSize;
                //m_appSettings.UseNewMapViewer = set.UseNewMapViewer;
                m_appSettings.MapViewerType = set.MapViewerType;
                m_appSettings.RequestProjectNotes = set.RequestProjectNotes;
                m_appSettings.ShowAdditionalSymbolInformation = set.ShowAdditionalSymbolInformation;
                m_appSettings.AutoHighlightSelectedMap = set.AutoHighlightSelectedMap;
                m_appSettings.AutoLoadLastFile = set.AutoLoadLastFile;
                m_appSettings.FancyDocking = set.FancyDocking;
                m_appSettings.SynchronizeMapviewers = set.SynchronizeMapviewers;
                m_appSettings.UseWidebandLambdaThroughSymbol = set.UseWidebandLambdaThroughSymbol;
                m_appSettings.WidebandLambdaSymbol = set.WideBandLambdaSymbol;
                m_appSettings.WidebandLowVoltage = set.WidebandLambdaLowVoltage;
                m_appSettings.WidebandHighVoltage = set.WidebandLambdaHighVoltage;
                m_appSettings.WidebandLowAFR = set.WidebandLambdaLowAFR;
                m_appSettings.WidebandHighAFR = set.WidebandLambdaHighAFR;
                m_appSettings.ShowAddressesInHex = set.ShowAddressesInHex;
                m_appSettings.EnableCanLogging = set.EnableCanLogging;
                m_appSettings.AcceptableTargetErrorPercentage = set.AcceptableTargetErrorPercentage;
                m_appSettings.AreaCorrectionPercentage = set.AreaCorrectionPercentage;
                m_appSettings.AutoUpdateFuelMap = set.AutoUpdateFuelMap;
                m_appSettings.MaximumIgnitionAdvancePerSession = set.MaximumIgnitionAdvancePerSession;

                m_appSettings.IgnitionAdvancePerCycle = set.IgnitionAdvancePerCycle;
                m_appSettings.IgnitionRetardFirstKnock = set.IgnitionRetardFirstKnock;
                m_appSettings.IgnitionRetardFurtherKnocks = set.IgnitionRetardFurtherKnocks;
                m_appSettings.GlobalMaximumIgnitionAdvance = set.GlobalMaximumIgnitionAdvance;

                m_appSettings.CellStableTime_ms = set.CellStableTime_ms;

                m_appSettings.IgnitionCellStableTime_ms = set.IgnitionCellStableTime_ms;
                m_appSettings.MinimumEngineSpeedForIgnitionTuning = set.MinimumEngineSpeedForIgnitionTuning;


                m_appSettings.CorrectionPercentage = set.CorrectionPercentage;
                m_appSettings.DiscardClosedThrottleMeasurements = set.DiscardClosedThrottleMeasurements;
                m_appSettings.DiscardFuelcutMeasurements = set.DiscardFuelcutMeasurements;
                m_appSettings.EnrichmentFilter = set.EnrichmentFilter;
                m_appSettings.FuelCutDecayTime_ms = set.FuelCutDecayTime_ms;
                m_appSettings.MaximumAdjustmentPerCyclePercentage = set.MaximumAdjustmentPerCyclePercentage;
                m_appSettings.MaximumAFRDeviance = set.MaximumAFRDeviance;
                m_appSettings.MinimumAFRMeasurements = set.MinimumAFRMeasurements;
                m_appSettings.UseEasyTrionicOptions = set.UseEasyTrionicOptions;
                m_appSettings.AutoDetectMapsensorType = set.AutoDetectMapsensorType;
                m_appSettings.DisableClosedLoopOnStartAutotune = set.DisableClosedLoopOnStartAutotune;
                m_appSettings.AutoLoggingEnabled = set.AutoLoggingEnabled;
                m_appSettings.AutoLogStartSign = set.AutoLogStartSign;
                m_appSettings.AutoLogStartValue = set.AutoLogStartValue;
                m_appSettings.AutoLogStopSign = set.AutoLogStopSign;
                m_appSettings.AutoLogStopValue = set.AutoLogStopValue;
                m_appSettings.AutoLogTriggerStartSymbol = set.AutoLogTriggerStartSymbol;
                m_appSettings.AutoLogTriggerStopSymbol = set.AutoLogTriggerStopSymbol;
                m_appSettings.KnockCounterSnapshot = set.KnockCounterSnapshot;
                m_appSettings.AlwaysCreateAFRMaps = set.AlwaysCreateAFRMaps;
                m_appSettings.ProjectFolder = set.ProjectFolder;

                try
                {

                    if (_ecuConnection.Opened)
                    {
                        if (m_appSettings.EnableCanLogging)
                        {
                            _ecuConnection.EnableLogging(System.Windows.Forms.Application.StartupPath);
                        }
                        else
                        {
                            _ecuConnection.DisableLogging();
                        }
                    }
                }
                catch (Exception canE)
                {
                    logger.Debug("Failed to change logging settings in canbus driver: " + canE.Message);
                }
                m_appSettings.CanDevice = set.CanUSBDevice;
                if (_ecuConnection != null) _ecuConnection.CanusbDevice = m_appSettings.CanDevice;
                //logger.Debug("selected canusb device: " + _ecuConnection.CanusbDevice);
                if (!m_appSettings.FancyDocking)
                {
                    dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.Standard;
                }
                else
                {
                    dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.VS2005;
                }
                if (_ecuConnection != null)
                {
                    _ecuConnection.SetWidebandvalues(m_appSettings.WidebandLowVoltage, m_appSettings.WidebandHighVoltage, m_appSettings.WidebandLowAFR, m_appSettings.WidebandHighAFR);
                }

                ctrlRealtime1.AppSettings = m_appSettings;
                _ecuConnection.AppSettings = m_appSettings;

                //update

                ctrlRealtime1.WideBandAFRSymbol = m_appSettings.WidebandLambdaSymbol;
                ctrlRealtime1.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
                ctrlRealtime1.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
                ctrlRealtime1.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                ctrlRealtime1.CellStableTime_ms = m_appSettings.CellStableTime_ms;
                ctrlRealtime1.CorrectionPercentage = m_appSettings.CorrectionPercentage;
                ctrlRealtime1.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
                ctrlRealtime1.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
                ctrlRealtime1.EnrichmentFilter = m_appSettings.EnrichmentFilter;
                ctrlRealtime1.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
                ctrlRealtime1.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
                ctrlRealtime1.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
                ctrlRealtime1.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
                //Set autologging parameters in realtime panel, that's the one that should use it
                ctrlRealtime1.AutoLoggingEnabled = m_appSettings.AutoLoggingEnabled;
                ctrlRealtime1.AutoLogStartSign = m_appSettings.AutoLogStartSign;
                ctrlRealtime1.AutoLogStartValue = m_appSettings.AutoLogStartValue;
                ctrlRealtime1.AutoLogStopSign = m_appSettings.AutoLogStopSign;
                ctrlRealtime1.AutoLogStopValue = m_appSettings.AutoLogStopValue;
                ctrlRealtime1.AutoLogTriggerStartSymbol = m_appSettings.AutoLogTriggerStartSymbol;
                ctrlRealtime1.AutoLogTriggerStopSymbol = m_appSettings.AutoLogTriggerStopSymbol;

            }
            SetModeAndFilters();
            SetAdditionalHelpPanelSize();
        }

        private void TryShowHelpForSymbol()
        {
            try
            {
                //groupControl1.Text = "";
                labelControl1.Text = "";
                object o = gridViewSymbols.GetFocusedRowCellValue(gcSymbolname);
                if (o != null)
                {
                    if (o != DBNull.Value)
                    {
                        //logger.Debug("Showing help for symbol: " + o.ToString());
                        if (o is Trionic5Tools.SymbolHelper)
                        {
                            Trionic5Tools.SymbolHelper sh = (Trionic5Tools.SymbolHelper)o;
                            ShowContextSensitiveHelpOnSymbol(sh.Varname);
                        }
                        else
                        {
                            ShowContextSensitiveHelpOnSymbol(o.ToString());
                        }

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("TryShowHelpForSymbol: " + E.Message);
            }
        }

        private void ShowContextSensitiveHelpOnSymbol(string symbolname)
        {

            string helptext = string.Empty;
/*            SymbolTranslator st = new SymbolTranslator();
            XDFCategories category;
            XDFSubCategory subcategory;
            st.TranslateSymbolToHelpText(symbolname, out helptext, out category, out subcategory, GlobalLanguageID);*/

            //groupControl1.Text = symbolname;
            helptext = m_trionicFile.GetFileInfo().GetSymbolDescription(symbolname);
            if (helptext == "") helptext = "No additional help available";
            labelControl1.Text = helptext;
        }

        #endregion

        #region User interface helper functions

        /// <summary>
        /// Checks if a certain information panel already is active (instance exists)
        /// </summary>
        /// <param name="symbolname"></param>
        /// <returns></returns>
        private bool PanelExists(string symbolname)
        {
            bool retval = false;
            foreach (DockPanel dp in dockManager1.Panels)
            {
                if (dp.Text == Path.GetFileName(m_trionicFileInformation.Filename) + " [" + symbolname + "]") 
                {
                    retval = true;
                }
            }
            return retval;
        }

        private bool SRAMPanelExists(string symbolname, string sramfilename)
        {
            bool retval = false;
            foreach (DockPanel dp in dockManager1.Panels)
            {
                if ((string)dp.Tag == sramfilename)
                {
                    if (dp.Text.StartsWith("SRAM") && dp.Text.Contains("[" + symbolname + "]"))
                    {
                        retval = true;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Brings a panel containing the given symbol to the foreground
        /// </summary>
        /// <param name="symbolname"></param>
        private void BringPanelToForeGround(string symbolname)
        {
            foreach (DockPanel dp in dockManager1.Panels)
            {
                if (dp.Text == Path.GetFileName(m_trionicFileInformation.Filename) + " [" + symbolname + "]") 
                {
                    dp.Show();
                    dp.BringToFront();
                }
            }
        }

        /// <summary>
        /// Opens the grouping level indicated for the given gridcontrol
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="groupleveltoexpand"></param>
        private void OpenGridViewGroups(GridControl ctrl, int groupleveltoexpand)
        {
            // open grouplevel 0 (if available)
            ctrl.BeginUpdate();
            try
            {
                GridView view = (GridView)ctrl.DefaultView;
                //view.ExpandAllGroups();
                view.MoveFirst();
                while (!view.IsLastRow)
                {
                    int rowhandle = view.FocusedRowHandle;
                    if (view.IsGroupRow(rowhandle))
                    {
                        int grouplevel = view.GetRowLevel(rowhandle);
                        if (grouplevel == groupleveltoexpand)
                        {
                            view.ExpandGroupRow(rowhandle);
                        }
                    }
                    view.MoveNext();
                }
                //view.MoveFirst();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            ctrl.EndUpdate();
        }

        /// <summary>
        /// Sets the application filtermodes according to the selected usermode
        /// </summary>
        private void SetModeAndFilters()
        {
            if (_immoValid)
            {
                if (m_appSettings.EnableAdvancedMode)
                {
                    // show symbollist and advanced tuning menu
                    if (_APPmode == OperationMode.ModeOffline) //<GS-14042010> not if it was changed in online mode
                    {
                        //dockSymbols.Restore();
                        dockSymbols.Visibility = DockVisibility.Visible;

                    }
                    ribbonAdvancedTuning.Visible = true;
                    ribbonPageGroup7.Visible = true;
                    ribbonPageGroup8.Visible = true;
                }
                else
                {
                    // hide symbollist and advanced tuning menu
//                    dockSymbols.Visibility = DockVisibility.Hidden;
                    dockSymbols.Visibility = DockVisibility.AutoHide;
                    dockSymbols.HideImmediately();
                    ribbonAdvancedTuning.Visible = true;
                    ribbonPageGroup7.Visible = true;
                    ribbonPageGroup8.Visible = false;
                }
            }
            SetFilterMode();
            //SetDefaultFilters();
        }

        #endregion

        #region TableMappings
        // TrionicFileInformation should be Interface or contain T5, T7 and T8 information
        private void btnInjectionMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetInjectionMap());
        }

        private void btnFuelKnockMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetInjectionKnockMap());
        }

        private void btnInjectorScaling_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetInjectorConstant());
        }

        private void btnIgnitionMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIgnitionMap());
        }

        private void btnIgnitionMapKnock_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIgnitionKnockMap());
        }

        private void btnIgnitionMapWarmup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIgnitionWarmupMap());
        }

        private void btnBoostRequestManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostRequestMap());
        }

        private void btnBoostBiasManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostBiasMap());
        }

        private void btnFuelcutManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetFuelcutMap());
        }

        private void btnPFactorsManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetPFactorsMap());
        }

        private void btnIFactorsManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIFactorsMap());
        }

        private void btnDFactorsManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetDFactorsMap());
        }

        private void btnFirstGearLimiterManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostLimiterFirstGearMap());
        }

        private void btnSecondGearLimiterManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostLimiterSecondGearMap());
        }

        #endregion

        private void ctrlRealtime1_onMonitorTypeChanged(object sender, ctrlRealtime.RealtimeMonitoringEventArgs e)
        {
            // the user selected a different type of realtime monitoring by changing the tabpage index.
            // now we need to reload the realtime monitoring list in the _ecuConnection object
            if (e.Type != RealtimeMonitoringType.UserMaps) // user maps should not alter the settings
            {
                FillRealtimePool(e.Type, false);
            }
        }

        private void FillRealtimePool(RealtimeMonitoringType _type, bool _startRealtime)
        {

            // first remove all the symbol from the list
            _ecuConnection.StopECUMonitoring();
            Thread.Sleep(100);
            _ecuConnection.RemoveAllSymbolsFromWatchlist();

            if (_type == RealtimeMonitoringType.AutotuneIgnition)
            {
                //<GS-06042011> needs less symbols to operate and we need fast comms for autotuning, so as little symbols as possible
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPressureSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPressureSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPressureSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEngineSpeedSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEngineSpeedSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEngineSpeedSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist("Knock_offset1234", m_trionicFileInformation.GetSymbolAddressSRAM("Knock_offset1234"), m_trionicFileInformation.GetSymbolLength("Knock_offset1234"), true);
            }
            else if (_type == RealtimeMonitoringType.AutotuneFuel)
            {
                //<GS-06042011> needs less symbols to operate and we need fast comms for autotuning, so as little symbols as possible
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPressureSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPressureSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPressureSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetCoolantTempSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetCoolantTempSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetCoolantTempSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEngineSpeedSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEngineSpeedSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEngineSpeedSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetThrottlePositionSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetThrottlePositionSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetThrottlePositionSymbol()), true);

            }
            else
            {
                // now add all the default values to the realtime pool!
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPressureSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPressureSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPressureSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetAirTempSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetAirTempSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetAirTempSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetCoolantTempSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetCoolantTempSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetCoolantTempSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEngineSpeedSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEngineSpeedSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEngineSpeedSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetThrottlePositionSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetThrottlePositionSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetThrottlePositionSymbol()), true);
                //<GS-28102010> default gemaakt (BoostTargetSymbol) vanwege update PID maps in live map view
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetBoostTargetSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetBoostTargetSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostTargetSymbol()), true);
                // add program status monitoring
            }
            _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetProgramStatusSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetProgramStatusSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramStatusSymbol()), true);
            //<GS-06042011> removed again _ecuConnection.AddSymbolToWatchlist("Idle_status", m_trionicFileInformation.GetSymbolAddressSRAM("Idle_status"), m_trionicFileInformation.GetSymbolLength("Idle_status"), true);
            //_ecuConnection.AddSymbolToWatchlist("Knock_status", m_trionicFileInformation.GetSymbolAddressSRAM("Knock_status"), m_trionicFileInformation.GetSymbolLength("Knock_status"), true);

            if (ctrlRealtime1.AutoTuning && _type != RealtimeMonitoringType.Fuel)
            {
                // make sure that the correct parameters are being watched
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnrichmentForLoadSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnrichmentForLoadSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnrichmentForLoadSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnrichmentForTPSSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnrichmentForTPSSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnrichmentForTPSSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnleanmentForLoadSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnleanmentForLoadSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnleanmentForLoadSymbol()), true);
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnleanmentForTPSSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnleanmentForTPSSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnleanmentForTPSSymbol()), true);
            }
            // depening on narrowband or wideband O2 sensor
            if (m_appSettings.UseWidebandLambdaThroughSymbol)
            {
                _ecuConnection.AddSymbolToWatchlist(m_appSettings.WidebandLambdaSymbol, m_trionicFileInformation.GetSymbolAddressSRAM(m_appSettings.WidebandLambdaSymbol), m_trionicFileInformation.GetSymbolLength(m_appSettings.WidebandLambdaSymbol), true);
            }
            else
            {
                _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetLambdaSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetLambdaSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetLambdaSymbol()), true);
            }
            
            if (m_appSettings.AutoLoggingEnabled)
            {
                // always add the symbols we need to monitor to be able to start/stop the logging
                _ecuConnection.AddSymbolToWatchlist(m_appSettings.AutoLogTriggerStartSymbol, m_trionicFileInformation.GetSymbolAddressSRAM(m_appSettings.AutoLogTriggerStartSymbol), m_trionicFileInformation.GetSymbolLength(m_appSettings.AutoLogTriggerStartSymbol), true);
                _ecuConnection.AddSymbolToWatchlist(m_appSettings.AutoLogTriggerStopSymbol, m_trionicFileInformation.GetSymbolAddressSRAM(m_appSettings.AutoLogTriggerStopSymbol), m_trionicFileInformation.GetSymbolLength(m_appSettings.AutoLogTriggerStopSymbol), true);
            }
            switch (_type)
            {
                case RealtimeMonitoringType.Fuel:
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetInjectionDurationSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionDurationSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionDurationSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnrichmentForLoadSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnrichmentForLoadSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnrichmentForLoadSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnrichmentForTPSSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnrichmentForTPSSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnrichmentForTPSSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnleanmentForLoadSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnleanmentForLoadSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnleanmentForLoadSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetEnleanmentForTPSSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetEnleanmentForTPSSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetEnleanmentForTPSSymbol()), true);
                    break;
                case RealtimeMonitoringType.Ignition:
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetIgnitionAdvanceSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol()), true);
                    // ignition trim
                    // ignition adapt
                    // ignition knock retard
                    // ignition counter
                    break;
                case RealtimeMonitoringType.Boost:
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetBoostRequestSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetBoostRequestSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostRequestSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetBoostReductionSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetBoostReductionSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostReductionSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPFactorSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPFactorSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPFactorSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetIFactorSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIFactorSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIFactorSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetDFactorSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetDFactorSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetDFactorSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPWMOutputSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPWMOutputSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPWMOutputSymbol()), true);
                    // reg_offset
                    break;
                case RealtimeMonitoringType.Knock:
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockCountCylinder1Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder1Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockCountCylinder1Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockCountCylinder2Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder2Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockCountCylinder2Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockCountCylinder3Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder3Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockCountCylinder3Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockCountCylinder4Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder4Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockCountCylinder4Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder1Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder2Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder3Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetCylinder4Symbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockOffsetAllCylindersSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockOffsetAllCylindersSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockOffsetAllCylindersSymbol()), true);
                    // ignition retard (Knock_offset1234)
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetBoostReductionSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetBoostReductionSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostReductionSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetKnockLevelSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockLevelSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetKnockLevelSymbol()), true);
                    // knock_level (for all cylinders???)
                    break;
                case RealtimeMonitoringType.Dashboard:
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetVehicleSpeedSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetVehicleSpeedSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetVehicleSpeedSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetTorqueSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetTorqueSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetTorqueSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetInjectionDurationSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionDurationSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionDurationSymbol()), true);
                    // inspuitduur (berekening verbruik)
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetBoostReductionSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetBoostReductionSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetBoostReductionSymbol()), true);
                    // apc decrease
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetIgnitionAdvanceSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), true);
                    // ignition advance
                    break;
                case RealtimeMonitoringType.Settings:
                    break;
                case RealtimeMonitoringType.Userdefined:
                    //TODO: <GS-22042010> get the user defined symbols and add them to the list as well
                    // 
                    foreach (Trionic5Tools.SymbolHelper sh in m_RealtimeUserSymbols)
                    {
                        //_ecuConnection.AddSymbolToWatchlist(sh.Varname, sh.Start_address, sh.Length, false);
                        _ecuConnection.AddSymbolToWatchlist(sh, false);
                    }
                    
                    //_ecuConnection.AddSymbolToWatchlist("Iv_position", m_trionicFileInformation.GetSymbolAddressSRAM("Iv_position"), m_trionicFileInformation.GetSymbolLength("Iv_position"), false);
                    break;
                case RealtimeMonitoringType.GraphDashboard:
                    break;
                case RealtimeMonitoringType.OnlineGraph:
                    //_ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetVehicleSpeedSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetVehicleSpeedSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetVehicleSpeedSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetTorqueSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetTorqueSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetTorqueSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetInjectionDurationSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionDurationSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionDurationSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetIgnitionAdvanceSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionAdvanceSymbol()), true);
                    _ecuConnection.AddSymbolToWatchlist(m_trionicFileInformation.GetPWMOutputSymbol(), m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetPWMOutputSymbol()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetPWMOutputSymbol()), true);

                    break;
            }
            // if we're just starting a realtime session, be sure the user defined symbols are started as well.

            if (_startRealtime)
            {
                foreach (Trionic5Tools.SymbolHelper sh in m_RealtimeUserSymbols)
                {
                    //_ecuConnection.AddSymbolToWatchlist(sh.Varname, sh.Start_address, sh.Length, false);
                    _ecuConnection.AddSymbolToWatchlist(sh, false);
                }
            }
            //props = m_trionicFile.GetTrionicProperties();
            switch (props.InjectorType)
            {
                case InjectorType.Stock:
                    ctrlRealtime1.InjectorCC = 365;
                    break;
                case InjectorType.GreenGiants:
                    ctrlRealtime1.InjectorCC = 413;
                    break;
                case InjectorType.Siemens630Dekas:
                    ctrlRealtime1.InjectorCC = 630;
                    break;
                case InjectorType.Siemens875Dekas:
                    ctrlRealtime1.InjectorCC = 875;
                    break;
                case InjectorType.Siemens1000cc:
                    ctrlRealtime1.InjectorCC = 1000;
                    break;
            }
            // restart the monitoring function
            _ecuConnection.StartECUMonitoring();
            ctrlRealtime1.SetNumberOfSymbolsToWatch(_ecuConnection.SymbolsToMonitor.Count);
        }

        private void DownloadFlashCANUSB()
        {
            // download the flashfile (using the external T5CanFlasher?)
            if (_ecuConnection.Opened)
            {
                //if (!_ecuConnection.EngineRunning)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Flash files|*.bin";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        //Ask user to choose an ECU type!!!
                        //frmECUTypeSelection typesel = new frmECUTypeSelection();
                        //if (typesel.ShowDialog() == DialogResult.OK)
                        //{
                            _ecuConnection.ReadFlash(sfd.FileName);
                        //}
                    }
                }
                /*else
                {
                    frmInfoBox info = new frmInfoBox("You cannot download the flash content while the engine is running!");
                }*/
            }
        }

        private void btnDownloadFlash_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DownloadFlashCANUSB();
        }

        private void DownloadSRAMCANUSB()
        {
            //Download sram snapshot from the ECU
            if (_ecuConnection.Opened)
            {
                if (m_CurrentWorkingProject != "")
                {
                    if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots");
                    string filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots\\Snapshot" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".RAM";
                    _ecuConnection.DumpSRAM(filename);
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "SRAM snapshots|*.ram";
                    // set the default filename and directory
                    sfd.InitialDirectory = Path.GetDirectoryName(m_trionicFileInformation.Filename);
                    sfd.FileName = "Snapshot-" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + "-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".RAM";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        _ecuConnection.DumpSRAM(sfd.FileName);
                    }
                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("A canbus connection is needed to create a SRAM snapshot");
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DownloadSRAMCANUSB();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // fetch the XML file that contains the information about available tunes on trionic.mobixs.eu
            // and show it in a browser with custom filters (carmodel, enginetype, OBDI/IBDII, mapsensor, injectors, stage)
            frmBrowseTunes browse = new frmBrowseTunes();
            browse.ShowDialog();
        }

        private void btnExportToLogWorks_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 5 logfiles|*.t5l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            if (logworksstring == string.Empty)
            {
                frmInfoBox info = new frmInfoBox("Logworks is not installed on this computer, download from http://www.innovatemotorsports.com/");
            }
            else if (ofd.ShowDialog() == DialogResult.OK)
            {
                ConvertFileToDif(ofd.FileName, false);
            }
        }

        #region LogWorks interface

        private void ConvertFileToDif(string filename, bool AutoExport)
        {
            System.Windows.Forms.Application.DoEvents();
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            try
            {
                Trionic5Tools.SymbolCollection sc = new Trionic5Tools.SymbolCollection();
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
                    foreach(string line in alllines)
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
                                        foreach (Trionic5Tools.SymbolHelper sh in sc)
                                        {
                                            if (sh.Varname == varname)
                                            {
                                                sfound = true;
                                            }
                                        }
                                        Trionic5Tools.SymbolHelper nsh = new Trionic5Tools.SymbolHelper();
                                        nsh.Varname = varname;
                                        if (!sfound) sc.Add(nsh);
                                    }
                                }
                            }
                            catch (Exception pE)
                            {
                                logger.Debug(pE.Message);
                            }
                        }
                    }
                }

                if (AutoExport)
                {
                    foreach (Trionic5Tools.SymbolHelper sh in sc)
                    {
                        sh.Color = GetColorFromRegistry(sh.Varname);
                    }
                    DifGenerator difgen = new DifGenerator();

                    difgen.LowAFR = m_appSettings.WidebandLowAFR / 1000;
                    difgen.HighAFR = m_appSettings.WidebandHighAFR / 1000;
                    difgen.MaximumVoltageWideband = m_appSettings.WidebandHighVoltage / 1000;
                    difgen.MinimumVoltageWideband = m_appSettings.WidebandLowVoltage / 1000;
                    difgen.WidebandSymbol = m_appSettings.WidebandLambdaSymbol;
                    difgen.UseWidebandInput = m_appSettings.UseWidebandLambdaThroughSymbol;

                    difgen.onExportProgress += new DifGenerator.ExportProgress(difgen_onExportProgress);
                    System.Windows.Forms.Application.DoEvents();
                    try
                    {
                        difgen.ConvertFileToDif(filename, sc, startDate, endDate, true, true);
                    }
                    catch (Exception expE1)
                    {
                        logger.Debug(expE1.Message);
                    }
                }
                else
                {

                    // show selection screen
                    frmPlotSelection plotsel = new frmPlotSelection();
                    foreach (Trionic5Tools.SymbolHelper sh in sc)
                    {
                        if (sh.Varname != "Pgm_status")
                        {
                            plotsel.AddItemToList(sh.Varname);
                        }
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
                        difgen.LowAFR = m_appSettings.WidebandLowAFR / 1000;
                        difgen.HighAFR = m_appSettings.WidebandHighAFR / 1000;
                        difgen.MaximumVoltageWideband = m_appSettings.WidebandHighVoltage / 1000;
                        difgen.MinimumVoltageWideband = m_appSettings.WidebandLowVoltage / 1000;
                        difgen.WidebandSymbol = m_appSettings.WidebandLambdaSymbol;
                        difgen.UseWidebandInput = m_appSettings.UseWidebandLambdaThroughSymbol;
                        difgen.onExportProgress += new DifGenerator.ExportProgress(difgen_onExportProgress);
                        try
                        {
                            if (difgen.ConvertFileToDif(filename, sc, startDate, endDate, m_appSettings.InterpolateLogWorksTimescale, true))
                            {
                                //difgen.ConvertFileToDif(filename, sc, startDate, endDate, false, false);
                                StartLogWorksWithCurrentFile(Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".dif");
                            }
                            else
                            {
                                frmInfoBox info = new frmInfoBox("No data was found to export!");
                            }
                        }
                        catch (Exception expE2)
                        {
                            logger.Debug(expE2.Message);
                        }
                    }
                    TimeSpan ts = new TimeSpan(endDate.Ticks - startDate.Ticks);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        void difgen_onExportProgress(object sender, DifGenerator.ProgressEventArgs e)
        {
            //frmProgressLogWorks.SetProgressPercentage(e.Percentage);
            SetTaskProgress(e.Percentage, true);
            if (e.Percentage == 100)
            {
                SetStatusText("Export done");
                SetTaskProgress(0, false);
            }
        }

        private Int32 GetValueFromRegistry(string symbolname)
        {
            RegistryKey TempKey = null;
            Int32 win32color = 0;
            TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey Settings = TempKey.CreateSubKey("T5SuitePro\\SymbolColors"))
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
                            logger.Debug(E.Message);
                        }
                    }
                }
            }
            return win32color;
        }

        private Color GetColorFromRegistry(string symbolname)
        {
            Color c = Color.Black;
            Int32 win32color = GetValueFromRegistry(symbolname);
            c = Color.FromArgb((int)win32color);
            //c = System.Drawing.ColorTranslator.FromWin32(win32color);
            return c;
        }

        private void StartLogWorksWithCurrentFile(string filename)
        {
            try
            {
                if (logworksstring != string.Empty)
                {
                    System.Diagnostics.Process.Start(logworksstring, "\"" + filename + "\"");
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }
        #endregion

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!_immoValid)
            {
                ribbonFile.Visible = false;
                ribbonActions.Visible = false;
                ribbonAdvancedTuning.Visible = false;
                ribbonManualTuning.Visible = false;
                ribbonOnline.Visible = false;
                rbnPageLogging.Visible = false;
                ribbonProgramming.Visible = false;
                //ribbonProgramming.Visible = false;

                ribbonControl1.SelectedPage = ribbonHelp;
                btnSwitchMode.Enabled = false;
                Application.DoEvents();
            }
            else
            {
                ribbonControl1.SelectedPage = ribbonFile;
                Application.DoEvents();
            }
            SetModeAndFilters();
            _splash.Hide();
            
            if (m_startFromCommandLine)
            {
                if (File.Exists(m_commandLineFile))
                {
                    OpenWorkingFile(m_commandLineFile);
                }
            }
            else if (m_appSettings.AutoLoadLastFile)
            {
                //check if last opened was a project or a seperate file
                if (m_appSettings.LastOpenedType == 0)
                {
                    if (m_appSettings.Lastfilename != "")
                    {
                        if (File.Exists(m_appSettings.Lastfilename))
                        {
                            if (_immoValid)
                            {
                                OpenWorkingFile(m_appSettings.Lastfilename);
                            }
                        }
                    }
                }
                else
                {
                    if (m_appSettings.Lastprojectname != "")
                    {
                        if (_immoValid)
                        {
                            OpenProject(m_appSettings.Lastprojectname);
                        }
                    }
                }
            }
           // InitSkins();
            try
            {
                m_msiUpdater = new msiupdater(new Version(System.Windows.Forms.Application.ProductVersion));
                m_msiUpdater.Apppath = System.Windows.Forms.Application.UserAppDataPath;
                m_msiUpdater.onDataPump += new msiupdater.DataPump(m_msiUpdater_onDataPump);
                m_msiUpdater.onUpdateProgressChanged += new msiupdater.UpdateProgressChanged(m_msiUpdater_onUpdateProgressChanged);
                m_msiUpdater.CheckForUpdates("Global", "http://develop.trionictuning.com/T5Suite2/", "", "", false);

            }
            catch (Exception E)
            {
                logger.Debug("Failed to get initial update: " + E.Message);
            }
            if (IsChristmasTime())
            {
                ShowChristmasWish();
            }
        }

        void m_msiUpdater_onUpdateProgressChanged(msiupdater.MSIUpdateProgressEventArgs e)
        {
            logger.Debug("m_msiUpdater_onUpdateProgressChanged: " + e.PercentageDone.ToString());
        }

        void m_msiUpdater_onDataPump(msiupdater.MSIUpdaterEventArgs e)
        {
            logger.Debug("m_msiUpdater_onDataPump: " + e.Data + " " + e.XMLFile);
            SetStatusText(e.Data);

            


            if (e.UpdateAvailable)
            {
                //                barUpdatestatus.ImageIndex = 28;
                //ShowChangeLog(e.Version);
                //this.Invoke(m_DelegateShowChangeLog, e.Version);

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
                            logger.Debug(E.Message);
                        }
                    }
                }

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
                    // gebruiker heeft nee gekozen, niet meer lastig vallen
                    if (m_msiUpdater != null)
                    {
                        m_msiUpdater.Blockauto_updates = false;
                    }
                }
            }
            // test
            //frmUpdateAvailable frmUpdatetest = new frmUpdateAvailable();
            //frmUpdatetest.ShowDialog();
            // test
        }

        private void btnLicense_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show license input screen including the HWID
            frmLicense lic = new frmLicense();
            HWID _id = new HWID();
            string computerID = _id.GetUniqueIdentifier(Application.StartupPath);
            lic.SetHWID(computerID);
            lic.ImmoValid = _immoValid;

            if (lic.ShowDialog() == DialogResult.OK)
            {
                // controleer geldigheid
                if (!_immoValid)
                {
                    Trionic5Immo immo = new Trionic5Immo();

                    immo.SaveLicense(lic.GetLicenseCode());

                    if (immo.ImmoValid(computerID))
                    {
                        // reshow menu items
                        ribbonFile.Visible = true;
                        ribbonActions.Visible = true;
                        ribbonAdvancedTuning.Visible = true;
                        ribbonManualTuning.Visible = true;
                        ribbonOnline.Visible = true;
                        rbnPageLogging.Visible = true;
                        ribbonControl1.SelectedPage = ribbonFile;
                        btnSwitchMode.Enabled = true;
                        Application.DoEvents();
                    }
                    else
                    {

                    }
                }
            }

            
        }

        private void btnBoostRequestAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostRequestMapAUT());
        }

        private void btnBoostBiasAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostBiasMapAUT());
        }

        private void btnFirstGearLimiterAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostLimiterFirstGearMapAUT());
        }

        private void btnPFactorsAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetPFactorsMapAUT());
        }

        private void btnIFactorsAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIFactorsMapAUT());
        }

        private void btnDFactorsAUT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetDFactorsMapAUT());
        }

        private void btnKnockSensitivityMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetKnockSensitivityMap());
        }

        private void btnBatteryCorrectionMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBatteryCorrectionMap());
        }


        private void btnKnockLimitMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetKnockLimitMap());
        }

        private void btnBoostReductionMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetBoostKnockMap());
        }

        private void btnVerifyChecksum_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // verify the files checksum (if opened)
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    if (m_trionicFile.ValidateChecksum())
                    {
                        frmInfoBox info = new frmInfoBox("Checksum is valid");
                    }
                    else
                    {
                        frmChecksumWarning warning = new frmChecksumWarning();
                        if (warning.ShowDialog() == DialogResult.Yes)
                        {
                            m_trionicFile.UpdateChecksum();
                        }
                    }
                }
            }
            
        }

        private void btnCheckForUpdates_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.CheckForUpdates("Global", "http://develop.trionictuning.com/T5Suite2/", "", "", false);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

        }

        private void btnStartManual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // start the user manual!
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//T5Suite2 User Manual.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//T5Suite2 User Manual.pdf");
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("User manual could not be found or opened!");
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void btnStartT5Manual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // start the T5 documentation
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//Trionic 5.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//Trionic 5.pdf");
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Trionic 5 documentation could not be found or opened!");
                }
            }
            catch (Exception E2)
            {
                logger.Debug(E2.Message);
            }
        }

        private void TryToAddOpenLoopTables(MapViewer mv)
        {
            try
            {
                if (mv.Map_name == m_trionicFileInformation.GetInjectionMap() || mv.Map_name == m_trionicFileInformation.GetInjectionKnockMap() || mv.Map_name == "TargetAFR" || mv.Map_name == "FeedbackAFR" || mv.Map_name == "FeedbackvsTargetAFR" || mv.Map_name == "Adapt_korr" || mv.Map_name == "Adapt_ref"/*|| mv.Map_name == "Ign_map_0!"|| mv.Map_name == "Ign_map_4!"|| mv.Map_name == "Ign_map_2!"|| mv.Map_name == "Tryck_mat!"|| mv.Map_name == "Tryck_mat_a!"*/)
                {
                    byte[] open_loop = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetOpenLoopMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetOpenLoopMap()));
                    mv.Open_loop = open_loop;
                    byte[] open_loop_knock = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetOpenLoopKnockMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetOpenLoopKnockMap()));
                    mv.Open_loop_knock = open_loop_knock;
                    //TODO: add counters as well.
                    //mv.Afr_counter = AFRMapCounterInMemory;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void TryToAddOpenLoopTables(IMapViewer mv)
        {
            try
            {
                if (mv.Map_name == m_trionicFileInformation.GetIgnitionMap() || mv.Map_name == m_trionicFileInformation.GetInjectionMap() || mv.Map_name == m_trionicFileInformation.GetInjectionKnockMap() || mv.Map_name == "TargetAFR" || mv.Map_name == "FeedbackAFR" || mv.Map_name == "FeedbackvsTargetAFR" || mv.Map_name == "Adapt_korr" || mv.Map_name == "Adapt_ref" || mv.Map_name == "Knock_count_map"/*|| mv.Map_name == "Ign_map_0!"|| mv.Map_name == "Ign_map_4!"|| mv.Map_name == "Ign_map_2!"|| mv.Map_name == "Tryck_mat!"|| mv.Map_name == "Tryck_mat_a!"*/)
                {
                    Trionic5Properties props = m_trionicFile.GetTrionicProperties();
                    if (props.Lambdacontrol)
                    {
                        byte[] open_loop = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetOpenLoopMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetOpenLoopMap()));
                        mv.Open_loop = open_loop;
                        byte[] open_loop_knock = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetOpenLoopKnockMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetOpenLoopKnockMap()));
                        mv.Open_loop_knock = open_loop_knock;
                    }
                    // add Turbo_press_tab!
                    if (props.IsTrionic55)
                    {
                        byte[] turbo_press_tab = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Knock_press_tab!"), (uint)m_trionicFileInformation.GetSymbolLength("Knock_press_tab!"));
                        mv.Turbo_press_tab = ByteArrayToIntArray(turbo_press_tab); ;
                    }
                    else
                    {
                        byte[] knock_press_value = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Knock_press!"), (uint)m_trionicFileInformation.GetSymbolLength("Knock_press!"));
                        int[] dummyKnockPress = new int[16];
                        for (int i = 0; i < dummyKnockPress.Length; i++)
                        {
                            dummyKnockPress[i] = Convert.ToInt32(knock_press_value[0]);
                        }
                        mv.Turbo_press_tab = dummyKnockPress;
                    }
                    // add the lock map
                    if (m_IgnitionMaps == null)
                    {
                        //<GS-29032011> always add this stuff.. we just have to
                        m_IgnitionMaps = new IgnitionMaps();
                        m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                        m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                        m_IgnitionMaps.TrionicFile = m_trionicFile;
                        m_IgnitionMaps.InitializeMaps();
}
                    if (m_IgnitionMaps != null)
                    {
                        mv.Ignition_lock_map = m_IgnitionMaps.GetIgnitionLockedMap();
                    }
                    if (m_AFRMaps != null)
                    {
                        mv.AFR_lock_map = m_AFRMaps.GetAFRLockedMap();
                    }


                    // add knock adaption area
                    // add boost adaption area
                    mv.KnockAdaptionLoadFrom = m_trionicFile.GetSymbolAsInt("Kadapt_load_low!");
                    mv.KnockAdaptionLoadUpto = m_trionicFile.GetSymbolAsInt("Kadapt_load_high!");
                    mv.KnockAdaptionRpmFrom = m_trionicFile.GetSymbolAsInt("Kadapt_rpm_low!") * 10;
                    mv.KnockAdaptionRpmUpto = m_trionicFile.GetSymbolAsInt("Kadapt_rpm_high!") * 10;
                    if (props.AutomaticTransmission)
                    {
                        mv.BoostAdaptionRpmFrom = m_trionicFile.GetAutoRpmLow();
                        mv.BoostAdaptionRpmUpto = m_trionicFile.GetAutoRpmHigh();
                    }
                    else
                    {
                        mv.BoostAdaptionRpmFrom = m_trionicFile.GetManualRpmLow();
                        mv.BoostAdaptionRpmUpto = m_trionicFile.GetManualRpmHigh();
                    }
                    //TODO: add counters as well.
                    //mv.Afr_counter = AFRMapCounterInMemory;
                }
                else if (mv.Map_name == m_trionicFileInformation.GetIdleFuelMap() || mv.Map_name == "IdleTargetAFR" || mv.Map_name == "IdleFeedbackAFR" || mv.Map_name == "IdleFeedbackvsTargetAFR")
                {
                    if (m_AFRMaps != null)
                    {
                        mv.IdleAFR_lock_map = m_AFRMaps.GetIdleAFRLockedMap();
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }


        private void btnMoveSymbolsToAnotherBinary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //TODO: have the  user select which symbols to move and which not to move (only the different symbols should be listed)
            // ask for another bin file
            Trionic5Resume resume = new Trionic5Resume();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (m_trionicFileInformation.Filename != string.Empty)
            {
                frmTransferDataWizard dataWiz = new frmTransferDataWizard();

                if (dataWiz.ShowDialog() == DialogResult.OK)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        TransferMapsToNewBinary(ofd.FileName, resume);
                        TuningReport tuningrep = new TuningReport();
                        tuningrep.ReportTitle = "Data transfer report";
                        tuningrep.DataSource = resume.ResumeTuning;
                        tuningrep.CreateReport();
                        tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);
                        frmInfoBox info = new frmInfoBox("Data was transferred to the target binary");
                    }
                    else
                    {
                        return;
                    }
                }
                
            }
        }

        private void TransferMapsToNewBinary(string filename, Trionic5Resume resume)
        {
            IECUFile m_FileToTransferTo = new Trionic5File();
            m_FileToTransferTo.LibraryPath = Application.StartupPath + "\\Binaries";
            m_FileToTransferTo.SetAutoUpdateChecksum(m_appSettings.AutoChecksum);
            
            m_FileToTransferTo.SelectFile(filename);
            Trionic5FileInformation m_FileInfoToTransferTo = new Trionic5FileInformation();

            if (filename != string.Empty)
            {
                File.Copy(filename, Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetransferringmaps.bin", true);
                resume.AddToResumeTable("Backup file created");
                //bool m_fileparsed = false;
                SetStatusText("Start symbol parsing");
                m_FileInfoToTransferTo = m_FileToTransferTo.ParseFile();
                //m_fileparsed = true;

                foreach (Trionic5Tools.SymbolHelper sh in m_FileInfoToTransferTo.SymbolCollection)
                {
                    if (sh.Flash_start_address > 0)
                    {
                        foreach (Trionic5Tools.SymbolHelper cfsh in m_trionicFileInformation.SymbolCollection)
                        {
                            if (cfsh.Varname == sh.Varname)
                            {
                                //progress.SetProgress("Transferring: " + sh.Varname);
                                CopySymbol(sh.Varname, m_trionicFileInformation.Filename, m_FileToTransferTo, cfsh.Flash_start_address, cfsh.Length, filename, sh.Flash_start_address, sh.Length, resume);
                            }
                        }
                    }
                }
                SetStatusText("Idle.");

            }
        }

        private void CopySymbol(string symbolname, string fromfilename, IECUFile FileToTransferTo, int fromflashaddress, int fromlength, string targetfilename, int targetflashaddress, int targetlength, Trionic5Resume resume)
        {
            if (fromlength != targetlength)
            {
                resume.AddToResumeTable("Unable to transfer symbol " + symbolname + " because source and target lengths don't match!");
            }
            else
            {
                try
                {
                    while (fromflashaddress > m_trionicFileInformation.Filelength) fromflashaddress -= m_trionicFileInformation.Filelength;
                    FileInfo fi = new FileInfo(targetfilename);
                    while (targetflashaddress > fi.Length) targetflashaddress -= (int)fi.Length;
                    byte[] mapdata = m_trionicFile.ReadDataFromFile(fromfilename, (uint)fromflashaddress, (uint)fromlength);
                    FileToTransferTo.WriteData(mapdata, (uint)targetflashaddress);
                    resume.AddToResumeTable("Transferred symbol " + symbolname + " successfully");
                }
                catch (Exception E)
                {
                    resume.AddToResumeTable("Failed to transfer symbol " + symbolname + ": " + E.Message);
                }
            }
        }

        private void btnVINDecoder_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDecodeVIN decode = new frmDecodeVIN();
            decode.ShowDialog();
        }

        private void btnLookupPartnumber_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPartnumberLookup lookup = new frmPartnumberLookup();
            lookup.DisableOpenButtons();
            lookup.ShowDialog();
            if (lookup.Open_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    //TryToLoadFile(filename);
                }
            }
            else if (lookup.Compare_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    //CompareToFile(filename);
                }
            }
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_AFRMaps != null)
            {
                m_AFRMaps.SaveMaps();
                m_AFRMaps.SaveIdleMaps(); // <GS-28102010>
            }

            try
            {
                if (_globalBDMOpened)
                {
                    BdmAdapter_Close();
                }
            }
            catch (Exception E)
            {
                //BdmAdapter_Close();
                logger.Debug("Failed to close BDM: " + E.Message);
            }
            if (m_CurrentWorkingProject != "")
            {
                CloseProject();
            }
            if (_ECUmode == OperationMode.ModeOnline)
            {
                // go offline first
                if (_ecuConnection.Opened)
                {
                    _ecuConnection.CloseECUConnection(true);
                    Thread.Sleep(1000); // wait a second for the thread to exit
                }
            }
            SaveUserDefinedRealtimeSymbols();
            if (_adapterMonitor != null) _adapterMonitor.Stop();
            if (_ftdiAdapterMonitor != null) _ftdiAdapterMonitor.Stop();
            // close all open viewers
            bool pnlavailable = true;
            while (pnlavailable)
            {
                pnlavailable = false;
                foreach (DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Name != dockSymbols.Name)
                    {
                        dockManager1.RemovePanel(pnl);
                        pnlavailable = true;
                        Thread.Sleep(100);
                        break;
                    }
                }
            }
            Environment.Exit(0);
        }

        private void LoadUserDefinedRealtimeSymbols()
        {
            string savePath = Path.Combine(Application.LocalUserAppDataPath, "rtsymbols.txt");
            if (File.Exists(savePath))
            {
                m_RealtimeUserSymbols = new Trionic5Tools.SymbolCollection(); // first empty
                char[] sep = new char[1];
                sep.SetValue(';', 0);
                using (StreamReader sr = new StreamReader(savePath))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(sep);


                        foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                        {
                            if (sh.Varname == values.GetValue(0).ToString())
                            {
                                Trionic5Tools.SymbolHelper shnew = new Trionic5Tools.SymbolHelper();
                                shnew.Varname = sh.Varname;
                                shnew.Flash_start_address = sh.Flash_start_address;
                                shnew.Length = sh.Length;
                                shnew.Start_address = sh.Start_address;
                                shnew.Color = sh.Color;
                                shnew.UserCorrectionFactor = ConvertToDouble(values.GetValue(1).ToString());
                                shnew.UserCorrectionOffset = ConvertToDouble(values.GetValue(2).ToString());
                                shnew.UseUserCorrection = true;
                                m_RealtimeUserSymbols.Add(shnew);
                            }
                        }
                    }
                }
                ctrlRealtime1.SetRealtimeSymbollist(m_RealtimeUserSymbols);
            }
        }

        private void SaveUserDefinedRealtimeSymbols()
        {
            //Assembly currentAssembly = Assembly.GetExecutingAssembly();

            string savePath = Path.Combine(Application.LocalUserAppDataPath, "rtsymbols.txt");
            using (StreamWriter sw = new StreamWriter(savePath, false))
            {
                foreach (Trionic5Tools.SymbolHelper sh in m_RealtimeUserSymbols)
                {
                    sw.WriteLine(sh.Varname + ";" + sh.UserCorrectionFactor.ToString() + ";" + sh.UserCorrectionOffset + ";");
                }
            }
        }

        private bool HexViewerActive(bool ShowIfActive)
        {
            bool retval = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Hexviewer: " + Path.GetFileName(m_trionicFileInformation.Filename)))
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
                            if (tpnl.Text.StartsWith("Hexviewer: " + Path.GetFileName(m_trionicFileInformation.Filename)))
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

        private bool SRAMHexViewerActive(bool ShowIfActive)
        {
            bool retval = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("SRAM Hexviewer: " + Path.GetFileName(m_trionicFileInformation.SRAMfilename)))
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
                            if (tpnl.Text.StartsWith("Hexviewer: " + Path.GetFileName(m_trionicFileInformation.SRAMfilename)))
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

        private void StartHexViewer()
        {
            if (m_trionicFileInformation.Filename != "")
            {
                if (MessageBox.Show("Opening a hexviewer will require full access to the binary file. No other data can be read from or written to the file while the hexviewer is open! Do you wish to continue?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                        dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                        dockPanel.Tag = m_trionicFileInformation.Filename;
                        dockPanel.Text = "Hexviewer: " + Path.GetFileName(m_trionicFileInformation.Filename);
                        //dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                        HexViewer hv = new HexViewer();
                        hv.onClose += new HexViewer.ViewerClose(hv_onClose);
                        hv.Issramviewer = false;
                        hv.Dock = DockStyle.Fill;
                        dockPanel.Width = 580;
                        //if (File.Exists(m_trionicFileInformation.Filename + ".hexview")) File.Delete(m_trionicFileInformation.Filename + ".hexview");
                        //File.Copy(m_trionicFileInformation.Filename, m_trionicFileInformation.Filename + ".hexview");
                        hv.LoadDataFromFile(m_trionicFileInformation.Filename /*+ ".hexview"*/, m_trionicFileInformation.SymbolCollection);
                        dockPanel.Controls.Add(hv);
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                    dockManager1.EndUpdate();
                }
            }
        }

        void dockPanel_ClosedPanel(object sender, DockPanelEventArgs e)
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
                    }
                    else if (c is ctrlCompressorMapEx)
                    {
                        ctrlCompressorMapEx plot = (ctrlCompressorMapEx)c;
                        plot.ReleaseResources();
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
                            }
                            else if (c2 is ctrlCompressorMapEx)
                            {
                                ctrlCompressorMapEx plot = (ctrlCompressorMapEx)c2;
                                plot.ReleaseResources();
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
                            }
                            else if (c3 is ctrlCompressorMapEx)
                            {
                                ctrlCompressorMapEx plot = (ctrlCompressorMapEx)c3;
                                plot.ReleaseResources();
                            }

                        }
                    }
                }

                // remove the panel from the dockmanager
                dockManager1.RemovePanel(pnl);
            }
            
        }

        void hv_onClose(object sender, EventArgs e)
        {
            if (sender is HexViewer)
            {
                HexViewer hv = (HexViewer)sender;
                logger.Debug("Closed file: " + hv.LastFilename);
                // revert to previous file in that case?
            }
            // close the corresponding dockpanel
            /*if (sender is HexViewer)
            {
                HexViewer hv = (HexViewer)sender;
                logger.Debug("Hexviewer parent: " + hv.Parent.GetType().ToString());
                if (hv.Parent is DevExpress.XtraBars.Docking.ControlContainer)
                {
                    DevExpress.XtraBars.Docking.ControlContainer cc = (DevExpress.XtraBars.Docking.ControlContainer)hv.Parent;
                    logger.Debug("cc parent: " + cc.Parent.GetType().ToString());
                }
            }*/
        }

        private void StartSRAMHexViewer()
        {
            if (m_trionicFileInformation.SRAMfilename != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Text = "SRAM Hexviewer: " + Path.GetFileName(m_trionicFileInformation.SRAMfilename);
                    dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Tag = m_trionicFileInformation.SRAMfilename;
                    HexViewer hv = new HexViewer();
                    hv.onClose +=new HexViewer.ViewerClose(hv_onClose);
                    hv.Issramviewer = true;
                    hv.Dock = DockStyle.Fill;
                    dockPanel.Width = 580;
                    if (File.Exists(m_trionicFileInformation.SRAMfilename + ".hexview")) File.Delete(m_trionicFileInformation.SRAMfilename + ".hexview");
                    File.Copy(m_trionicFileInformation.SRAMfilename, m_trionicFileInformation.SRAMfilename + ".hexview");

                    hv.LoadDataFromFile(m_trionicFileInformation.SRAMfilename + ".hexview", m_trionicFileInformation.SymbolCollection);
                    dockPanel.Controls.Add(hv);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void SelectSymbolInHexViewer(string symbolname, int fileoffset, int length)
        {
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is HexViewer)
                    {
                        HexViewer vwr = (HexViewer)c;
                        if (!vwr.Issramviewer)
                        {
                            if (vwr.FileName == m_trionicFileInformation.Filename)
                            {
                                vwr.SelectText(symbolname, fileoffset, length);
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is HexViewer)
                            {
                                HexViewer vwr2 = (HexViewer)c2;
                                if (!vwr2.Issramviewer)
                                {
                                    if (vwr2.FileName == m_trionicFileInformation.Filename)
                                    {
                                        vwr2.SelectText(symbolname, fileoffset, length);
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
                            if (c3 is HexViewer)
                            {
                                HexViewer vwr3 = (HexViewer)c3;
                                if (!vwr3.Issramviewer)
                                {
                                    if (vwr3.FileName == m_trionicFileInformation.Filename)
                                    {
                                        vwr3.SelectText(symbolname, fileoffset, length);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private void SelectSymbolInSRAMHexViewer(string symbolname, int fileoffset, int length)
        {
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is HexViewer)
                    {
                        HexViewer vwr = (HexViewer)c;
                        if (vwr.Issramviewer)
                        {
                            if (vwr.FileName == m_trionicFileInformation.SRAMfilename)
                            {
                                vwr.SelectText(symbolname, fileoffset, length);
                            }
                        }
                    }
                    else if (c is DevExpress.XtraBars.Docking.DockPanel)
                    {
                        DevExpress.XtraBars.Docking.DockPanel tpnl = (DevExpress.XtraBars.Docking.DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is HexViewer)
                            {
                                HexViewer vwr2 = (HexViewer)c2;
                                if (vwr2.Issramviewer)
                                {
                                    if (vwr2.FileName == m_trionicFileInformation.SRAMfilename)
                                    {
                                        vwr2.SelectText(symbolname, fileoffset, length);
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
                            if (c3 is HexViewer)
                            {
                                HexViewer vwr3 = (HexViewer)c3;
                                if (vwr3.Issramviewer)
                                {
                                    if (vwr3.FileName == m_trionicFileInformation.SRAMfilename)
                                    {
                                        vwr3.SelectText(symbolname, fileoffset, length);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Show the current file in hexadecimal mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowFileInHex_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    if (!HexViewerActive(true))
                    {
                        // nieuwe hexviewer starten
                        StartHexViewer();
                    }
                    if (!SRAMHexViewerActive(true))
                    {
                        StartSRAMHexViewer();
                    }
                    // select symbol in viewer & scroll to it
                    Trionic5Tools.SymbolHelper dr = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    string symbolname = dr.Varname;
                    int flashaddress = dr.Flash_start_address;
                    int sramaddress = dr.Start_address;
                    int symbollength = dr.Length;
                    //if (flashaddress > m_trionicFile.GetFileInfo().Filename_size) flashaddress -= m_trionicFile.GetFileInfo().Filename_size;
                    int fileoffset = flashaddress;
                    while (fileoffset > m_trionicFileInformation.Filelength) fileoffset -= m_trionicFileInformation.Filelength;
                    
                    SelectSymbolInHexViewer(symbolname, fileoffset, symbollength);
                    SelectSymbolInSRAMHexViewer(symbolname, sramaddress, symbollength);
                }
            }
        }

        private void btnOpenSRAMSnapshot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 
            OpenFileDialog ofdsram = new OpenFileDialog();
            ofdsram.Filter = "SRAM snapshots|*.RAM";
            ofdsram.Multiselect = false;
            ofdsram.Title = "Select a SRAM snapshot";
            if (ofdsram.ShowDialog() == DialogResult.OK)
            {
                m_trionicFileInformation.SRAMfilename = ofdsram.FileName;
                // indicate that is has been loaded in the statusbar!
                barStaticItem3.Caption = "Snapshot: " + Path.GetFileName(m_trionicFileInformation.SRAMfilename);
                gridViewSymbols.ActiveFilterEnabled = false;
            }
        }

        private void ctrlRealtime1_onMapDisplayRequested(object sender, ctrlRealtime.MapDisplayRequestEventArgs e)
        {
            // start a mapviewer (special one) that overlaps the realtime panel!
            // <GS-29112010> if the file is a Trionic 5.2 file, select the proper maps.
            string mapname = e.MapName;
            if (!props.IsTrionic55)
            {
                if (mapname == "Adapt_korr!") mapname = "Adapt_korr";
            }
            StartTableViewerFloating(mapname);
        }

        private void mouseGestures1_GestureUpRight(object sender, MouseGestureEventArgs e)
        {

        }

        private void mouseGestures1_Gesture(object sender, MouseGestureEventArgs e)
        {
            /*if (e.Gesture == MouseGesture.UpRight)
            {
                // start realtime if not started yet
                if (!_ecuConnection.Opened)
                {
                    StartECUConnection();
                }
                if (_ecuConnection.Opened)
                {
                    btnConnectECU.Caption = "Disconnect ECU";
                }
                if (_APPmode == OperationMode.ModeOffline)
                {
                    StartOnlineMode();
                }
            }
            else if (e.Gesture == MouseGesture.DownLeft)
            {
                // stop realtime if not started yet
                if (_APPmode == OperationMode.ModeOnline)
                {
                    StopOnlineMode();
                    if (_ecuConnection.Opened)
                    {
                        StopECUConnection();
                    }
                }
            }*/
        }

        private void btnSwitchMode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_APPmode == OperationMode.ModeOffline)
            {
                if (!_ecuConnection.Opened)
                {
                    StartECUConnection();
                    StartOnlineMode();
                }
                else
                {
                    StartOnlineMode();
                }
            }
            else
            {
                StopOnlineMode();
                /*if (_ecuConnection.Opened)
                {
                    StopECUConnection();
                }*/
            }
        }

        private void gridViewSymbols_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // start the selected row
                try
                {
                    int[] selectedrows = gridViewSymbols.GetSelectedRows();
                    int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                    if (grouplevel >= gridViewSymbols.GroupCount)
                    {
                        if (gridViewSymbols.GetFocusedRow() is Trionic5Tools.SymbolHelper)
                        {
                            Trionic5Tools.SymbolHelper sh = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetFocusedRow();
                            logger.Debug("Symbol:" + sh.Varname);
                            if (!_ecuConnection.Opened && !sh.Varname.Contains("!") && m_trionicFileInformation.SRAMfilename == "")
                            {
                                frmInfoBox info = new frmInfoBox("Symbol resides in SRAM and you are in offline mode. T5Suite is unable to fetch this symboldata in offline mode");
                            }
                            else
                            {
                                StartTableViewer(sh.Varname);
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }

            }
        }

        private void btnCompareToBinary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // compare to other binary file(s)!
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 5 binary files|*.bin";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileNames.Length > 1)
                {
                    // bekijk het verschil voor alle geselecteerde bestanden en laat dit zien in een
                    // symbolcompareselector
                    DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                    CompareResultSelector tabdet = new CompareResultSelector();
                    tabdet.onFileSelect += new CompareResultSelector.NotifySelectFile(tabdet_onFileSelect);
                    tabdet.Dock = DockStyle.Fill;
                    dockPanel.Controls.Add(tabdet);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("FILENAME");
                    dt.Columns.Add("PARTNUMBER");
                    dt.Columns.Add("SOFTWAREID");
                    dt.Columns.Add("NUMBEROFSYMBOLS", Type.GetType("System.Int32"));
                    dt.Columns.Add("FULLFILENAME");
                    int numberofsymbols = 0;

                    foreach (string filename in ofd.FileNames)
                    {
                        Trionic5Tools.SymbolCollection compSymbols = new Trionic5Tools.SymbolCollection();
                        AddressLookupCollection compAddressLookup = new AddressLookupCollection();

                        CompareSymbolTableToFile(filename, compSymbols, compAddressLookup, out numberofsymbols);
                        dt.Rows.Add(Path.GetFileName(filename), "", "", numberofsymbols, filename);
                    }
                    tabdet.SetData(dt);

                    dockPanel.Text = "Compare list: " + Path.GetFileName(m_trionicFileInformation.Filename);
                    bool isDocked = false;
                    foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                    {
                        if (pnl.Text.StartsWith("Compare list: ") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                        {
                            dockPanel.DockAsTab(pnl, 0);
                            //pnl.Options.ShowCloseButton = false;
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
                else if (ofd.FileNames.Length == 1)
                {
                    CompareToFile((string)ofd.FileNames.GetValue(0));
                }
            }
        }

        private bool CompareSymbolToCurrentFile(string symbolname, int address, int length, string filename, out float diffperc, out int diffabs, out float diffavg)
        {
            diffavg = 0;
            diffabs = 0;
            diffperc = 0;
            double totalvalue1 = 0;
            double totalvalue2 = 0;
            bool retval = true;
            if (address > 0)
            {
                while (address > m_trionicFileInformation.Filelength) address -= m_trionicFileInformation.Filelength;

                int curaddress = m_trionicFileInformation.GetSymbolAddressFlash(symbolname);
                int curlength = m_trionicFileInformation.GetSymbolLength(symbolname);

                byte[] curdata = m_trionicFile.ReadData((uint)curaddress, (uint)curlength);
                byte[] compdata = m_trionicFile.ReadDataFromFile(filename, (uint)address, (uint)length);
                if (curdata.Length != compdata.Length) return false;
                for (int offset = 0; offset < curdata.Length; offset++)
                {
                    if ((byte)curdata.GetValue(offset) != (byte)compdata.GetValue(offset))
                    {
                        diffabs++;
                        retval = false;
                    }
                    totalvalue1 += (byte)curdata.GetValue(offset);
                    totalvalue2 += (byte)compdata.GetValue(offset);
                }
                if (curdata.Length > 0)
                {
                    totalvalue1 /= curdata.Length;
                    totalvalue2 /= compdata.Length;
                    diffavg = (float)Math.Abs(totalvalue1 - totalvalue2);
                    diffperc = (diffabs * 100) / curdata.Length;
                }
            }

            return retval;
        }


        private Trionic5File CompareSymbolTable(string filename, Trionic5Tools.SymbolCollection curSymbolCollection, AddressLookupCollection curAddressLookupCollection, DevExpress.XtraGrid.GridControl curGridControl, out Trionic5FileInformation m_CompareInfo)
        {
            //bool m_fileparsed = false;
            //listView1.Items.Clear();
            SetStatusText("Start symbol parsing");
            SetTaskProgress(0, true);
            Trionic5File m_CompareToFile = new Trionic5File();
            m_CompareToFile.LibraryPath = Application.StartupPath + "\\Binaries";
            m_CompareToFile.SetAutoUpdateChecksum(m_appSettings.AutoChecksum);

            m_CompareToFile.SelectFile(filename);
            m_CompareToFile.onDecodeProgress += new IECUFile.DecodeProgress(m_CompareToFile_onDecodeProgress);
            m_CompareInfo = m_CompareToFile.ParseTrionicFile(filename);
            // available in repository?
            curSymbolCollection = m_CompareInfo.SymbolCollection;
            curAddressLookupCollection = m_CompareInfo.AddressCollection;
            //                ParseFile(progress, filename, curTrionic5Tools.SymbolCollection, curAddressLookupCollection);
           // m_fileparsed = true;
            // AddLogItem("Start symbol export...");
            curSymbolCollection.SortColumn = "Start_address";
            curSymbolCollection.SortingOrder = Trionic5Tools.GenericComparer.SortOrder.Ascending;
            curSymbolCollection.Sort();
            // progress.SetProgress("Filling list");
            SetStatusText("Filling list");
            //listView1.SuspendLayout();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
            dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
            dt.Columns.Add("CATEGORY", Type.GetType("System.Int32"));
            dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
            dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
            dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
            dt.Columns.Add("CATEGORYNAME");
            dt.Columns.Add("SUBCATEGORYNAME");

            foreach (Trionic5Tools.SymbolHelper sh in curSymbolCollection)
            {
                float diffperc = 0;
                int diffabs = 0;
                float diffavg = 0;
                if (!CompareSymbolToCurrentFile(sh.Varname, sh.Flash_start_address, sh.Length, filename, out diffperc, out diffabs, out diffavg))
                {
                    dt.Rows.Add(sh.Varname, sh.Start_address, sh.Flash_start_address, sh.Length, sh.Length, m_trionicFileInformation.GetSymbolDescription(sh.Varname), false, (int)m_trionicFileInformation.GetSymbolCategory(sh.Varname), diffperc, diffabs, diffavg, m_trionicFileInformation.GetSymbolCategory(sh.Varname).ToString().Replace("_", " "), m_trionicFileInformation.GetSymbolSubcategory(sh.Varname).ToString().Replace("_", " "));
                }
            }
            curGridControl.DataSource = dt;
            /*if (m_fileparsed)
            {
                CreateRepositoryItem(filename, curTrionic5Tools.SymbolCollection);
            }*/
            //listView1.ResumeLayout();
            SetStatusText("Idle");
            SetTaskProgress(0, false);
            return m_CompareToFile;
            //barButtonItem14.Enabled = true;
        }

        void m_CompareToFile_onDecodeProgress(object sender, DecodeProgressEventArgs e)
        {
            SetTaskProgress(e.Progress, true);
            if (e.Progress >= 99)
            {
                SetStatusText("File decode done");
                SetTaskProgress(0, false);
            }
        }

        private void CompareSymbolTableToFile(string filename, Trionic5Tools.SymbolCollection curSymbolCollection, AddressLookupCollection curAddressLookupCollection, out int numberofsymboldifferent)
        {
            numberofsymboldifferent = 0;
            if (filename != string.Empty)
            {
                //bool m_fileparsed = false;
                SetTaskProgress(0, true);
                SetStatusText("Start symbol parsing");
                // available in repository?
                //ParseFile(progress, filename, curTrionic5Tools.SymbolCollection, curAddressLookupCollection);
                Trionic5File m_CompareToFile = new Trionic5File();
                m_CompareToFile.LibraryPath = Application.StartupPath + "\\Binaries";
                m_CompareToFile.SetAutoUpdateChecksum(m_appSettings.AutoChecksum);

                m_CompareToFile.onDecodeProgress += new IECUFile.DecodeProgress(m_CompareToFile_onDecodeProgress);
                Trionic5FileInformation m_CompareInfo = m_CompareToFile.ParseTrionicFile(filename);
                // available in repository?
                curSymbolCollection = m_CompareInfo.SymbolCollection;
                curAddressLookupCollection = m_CompareInfo.AddressCollection;
                //m_fileparsed = true;
                // AddLogItem("Start symbol export...");
                curSymbolCollection.SortColumn = "Start_address";
                curSymbolCollection.SortingOrder = Trionic5Tools.GenericComparer.SortOrder.Ascending;
                curSymbolCollection.Sort();
                //listView1.SuspendLayout();

                foreach (Trionic5Tools.SymbolHelper sh in curSymbolCollection)
                {
                    float diffperc = 0;
                    int diffabs = 0;
                    float diffavg = 0;
                    if (!CompareSymbolToCurrentFile(sh.Varname, sh.Flash_start_address, sh.Length, filename, out diffperc, out diffabs, out diffavg))
                    {
                        numberofsymboldifferent++;
                    }
                }
                /*if (m_fileparsed)
                {
                    CreateRepositoryItem(filename, curTrionic5Tools.SymbolCollection);
                }*/
                //listView1.ResumeLayout();
                SetTaskProgress(0, false);
                SetStatusText("Idle");
                //barButtonItem14.Enabled = true;
                //progress.Close();

            }
            else
            {
                frmInfoBox info = new frmInfoBox("No file selected, please select one first");
            }

        }

        void tabdet_onSymbolSelect(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            // <GS-22042010> should be compare viewer for Pgm_mod / Pgm_status as well!
            if (e.SymbolName == m_trionicFileInformation.GetProgramModeSymbol())
            {
                frmEasyFirmwareInfo info = new frmEasyFirmwareInfo();
                info.Pgm_mod = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetProgramModeSymbol()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol()));
                info.SetPrimarySourceName(Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename));
                info.Pgm_mod2 = m_trionicFile.ReadDataFromFile(e.Filename, (uint)e.SymbolAddress, (uint)e.SymbolLength);
                info.SetSecondarySourceName(Path.GetFileNameWithoutExtension(e.Filename));
                info.ShowDialog();
            }
            else if (!e.ShowDiffMap)
            {
                StartTableViewer(e.SymbolName);
                StartCompareMapViewer(e.SymbolName, e.Filename, e.SymbolAddress, e.SymbolLength, e.Symbols, e.Addresses, e.CompTrionic5File, e.CompTrionic5FileInformation);
            }
            else
            {
                StartCompareDifferenceViewer(e.SymbolName, e.Filename, e.SymbolAddress, e.SymbolLength, e.CompTrionic5File, e.CompTrionic5FileInformation);
            }
            TryShowHelpForSymbol();
        }

        private void StartCompareDifferenceViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength, Trionic5File curFile, Trionic5FileInformation curInfo)
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
                    dockPanel.Tag = Filename;
                    //IMapViewer tabdet = new MapViewerEx();
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    } 
                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsCompareViewer = true;
                    //TryTpShowTouchScreenInput();

                    tabdet.DirectSRAMWriteOnSymbolChange = false;
                    //tabdet.IsHexMode = true; // always in hexmode!
                    if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }

                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Filename = Filename;
                    tabdet.Map_name = SymbolName;

                    tabdet.Map_descr = m_trionicFileInformation.GetSymbolDescription(tabdet.Map_name);
                    tabdet.Map_cat = m_trionicFileInformation.GetSymbolCategory(tabdet.Map_name);
                    tabdet.X_axisvalues = curFile.GetMapXaxisValues(tabdet.Map_name);
                    tabdet.Y_axisvalues = curFile.GetMapYaxisValues(tabdet.Map_name);
                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    // van compare file halen?
                    curFile.GetMapAxisDescriptions(tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;

                    //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = curFile.GetTableMatrixWitdhByName(Filename, tabdet.Map_name, out columns, out rows);
                    int address = Convert.ToInt32(SymbolAddress);
                    if (address != 0)
                    {
                        while (address > curInfo.Filelength) address -= curInfo.Filelength;
                        tabdet.Map_address = address;
                        int length = SymbolLength;
                        tabdet.Map_length = length;
                        byte[] mapdata = curFile.ReadData((uint)address, (uint)length);
                        byte[] mapdataorig = curFile.ReadData((uint)address, (uint)length);
                        byte[] mapdata2 = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(SymbolName), (uint)m_trionicFileInformation.GetSymbolLength(SymbolName));

                        tabdet.Map_original_content = mapdataorig;
                        tabdet.Map_compare_content = mapdata2;

                        if (mapdata.Length == mapdata2.Length)
                        {

                            if (curFile.IsTableSixteenBits(SymbolName))
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt += 2)
                                {
                                    int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                    int value2 = Convert.ToInt16(mapdata2.GetValue(bt)) * 256 + Convert.ToInt16(mapdata2.GetValue(bt + 1));

                                    value1 = (int)Math.Abs(value1 - value2);
                                    //value1 = (int)(value1 - value2);
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
                                    //logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                    //mapdata.SetValue((byte)(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }

                            tabdet.Map_content = mapdata;
                            tabdet.UseNewCompare = true;

                            tabdet.Correction_factor = curFile.GetCorrectionFactorForMap(tabdet.Map_name);
                            tabdet.Correction_offset = curFile.GetOffsetForMap(tabdet.Map_name);
                            tabdet.IsUpsideDown = true;//GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, curFile.GetFileInfo().isSixteenBitTable(SymbolName));
                            tabdet.Dock = DockStyle.Fill;
                            //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                            //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                            //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                            //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                            //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                            //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                            //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                            tabdet.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);
                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol difference: " + SymbolName + " [" + Path.GetFileName(Filename) + "]";
                            /*bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol difference: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        //pnl.Options.ShowCloseButton = false;
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
                                        if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            //pnl.Options.ShowCloseButton = false;
                                            isDocked = true;
                                            break;
                                        }
                                    }
                                }
                            }*/
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.Contains("[" + SymbolName + "]") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        //pnl.Options.ShowCloseButton = false;
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
                                        if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            //pnl.Options.ShowCloseButton = false;
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

                                //                    dockPanel.Width = 400;
                            }
                            tabdet.InitEditValues();
                            dockPanel.Controls.Add(tabdet);

                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("Map lengths don't match...");
                        }
                    }
                }
                catch (Exception E)
                {

                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
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

        private void CompareToFile(string filename)
        {
            Trionic5Tools.SymbolCollection compSymbols = new Trionic5Tools.SymbolCollection();
            AddressLookupCollection compAddressLookup = new AddressLookupCollection();
            dockManager1.BeginUpdate();
            try
            {
                DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                CompareResults tabdet = new CompareResults();
                tabdet.Dock = DockStyle.Fill;
                tabdet.Filename = filename;

                tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelect);
                dockPanel.Controls.Add(tabdet);
                //dockPanel.DockAsTab(dockPanel1);
                dockPanel.Text = "Compare results: " + Path.GetFileName(filename);
                bool isDocked = false;
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text.StartsWith("Compare results: ") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                    {
                        dockPanel.DockAsTab(pnl, 0);

                        isDocked = true;
                        break;
                    }
                }
                if (!isDocked)
                {
                    dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);
                    dockPanel.Width = 400;
                }
                Trionic5FileInformation m_FileInformation = new Trionic5FileInformation();
                Trionic5File m_CompareToFile = CompareSymbolTable(filename, compSymbols, compAddressLookup, tabdet.gridControl1, out m_FileInformation);
                m_CompareToFile.SetAutoUpdateChecksum(m_appSettings.AutoChecksum);
                tabdet.ComparedTrionic5File = m_CompareToFile;
                tabdet.ComparedTrionic5FileInformation = m_FileInformation;
                tabdet.CompareSymbolCollection = compSymbols;
                tabdet.CompareAddressLookupCollection = compAddressLookup;
                tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            dockManager1.EndUpdate();
        }

        void tabdet_onFileSelect(object sender, CompareResultSelector.SelectFileEventArgs e)
        {
            CompareToFile(e.Filename);
        }

        private void StartCompareMapViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength, Trionic5Tools.SymbolCollection curSymbols, AddressLookupCollection curAddresses, Trionic5File curFile, Trionic5FileInformation curFileInfo)
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
                    }
                }
                if (!pnlfound)
                {
                    dockManager1.BeginUpdate();
                    try
                    {
                        dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        dockPanel.Tag = Filename;// m_trionicFile.GetFileInfo().Filename; changed 24/01/2008
                        //IMapViewer tabdet = new MapViewerEx();
                        IMapViewer tabdet;
                        
                        if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                        {
                            tabdet = new MapViewerEx();
                        }
                        else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                        {
                            tabdet = new MapViewer();
                        }
                        else
                        {
                            tabdet = new SimpleMapViewer();
                        }
                        tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                        tabdet.GraphVisible = m_appSettings.ShowGraphs;
                       // TryTpShowTouchScreenInput();

                        //tabdet.DirectSRAMWriteOnSymbolChange = m_appSettings.DirectSRAMWriteOnSymbolChange;
                       // tabdet.SetViewSize(m_appSettings.DefaultViewSize);

                        //tabdet.IsHexMode = barViewInHex.Checked;
                        tabdet.Viewtype = (Trionic5Tools.ViewType)m_appSettings.DefaultViewType;
                        if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                        {
                            if (m_appSettings.DefaultViewType == ViewType.Decimal)
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                            }
                            else
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                            }
                        }
                        else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                        {
                            if (m_appSettings.DefaultViewType == ViewType.Decimal)
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                            }
                            else
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                            }
                        }
                        else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                        {
                            if (m_appSettings.DefaultViewType == ViewType.Decimal)
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                            }
                            else
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                            }
                        }
                        else if (curFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                        {
                            if (m_appSettings.DefaultViewType == ViewType.Decimal)
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                            }
                            else
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                            }
                        }
                        else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                        {
                            if (m_appSettings.DefaultViewType == ViewType.Decimal)
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                            }
                            else
                            {
                                tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                            }
                        }
                        //tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                        //tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                        //tabdet.GraphVisible = m_appSettings.ShowGraphs;
                        //tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                        tabdet.Filename = Filename;
                        tabdet.Map_name = SymbolName;

                        tabdet.Map_descr = m_trionicFileInformation.GetSymbolDescription(tabdet.Map_name);
                        tabdet.Map_cat = m_trionicFileInformation.GetSymbolCategory(tabdet.Map_name);
                        tabdet.X_axisvalues = curFile.GetMapXaxisValues(tabdet.Map_name);
                        tabdet.Y_axisvalues = curFile.GetMapYaxisValues(tabdet.Map_name);
                        string xdescr = string.Empty;
                        string ydescr = string.Empty;
                        string zdescr = string.Empty;
                        // van compare file halen?
                        curFile.GetMapAxisDescriptions(tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                        tabdet.X_axis_name = xdescr;
                        tabdet.Y_axis_name = ydescr;
                        tabdet.Z_axis_name = zdescr;

                        //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                        int columns = 8;
                        int rows = 8;
                        
                        int tablewidth = curFile.GetTableMatrixWitdhByName(Filename, tabdet.Map_name, out columns, out rows);
                        int address = Convert.ToInt32(SymbolAddress);
                        if (address != 0)
                        {
                            while (address > curFileInfo.Filelength) address -= curFileInfo.Filelength;
                            tabdet.Map_address = address;
                            int length = SymbolLength;
                            tabdet.Map_length = length;
                            byte[] mapdata = curFile.ReadData((uint)address, (uint)length);
                            tabdet.Map_content = mapdata;
                            tabdet.Correction_factor = curFile.GetCorrectionFactorForMap(tabdet.Map_name);
                            tabdet.Correction_offset = curFile.GetOffsetForMap(tabdet.Map_name);
                            tabdet.IsUpsideDown = true;
                            tabdet.ShowTable(columns, curFile.GetFileInfo().isSixteenBitTable(SymbolName));
                            tabdet.Dock = DockStyle.Fill;
                            //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                            tabdet.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);
                            //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                            //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                            //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                            //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                            //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                            //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                            
                            tabdet.SetViewSize((Trionic5Tools.ViewSize)m_appSettings.DefaultViewSize);
                            //dockPanel.DockAsTab(dockPanel1);
                            //dockPanel.Text = "Symbol: " + SymbolName + " [" + Filename + "]";
                            dockPanel.Text = Path.GetFileName(Filename) + " [" + SymbolName + "]"; 

                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.Contains("[" + SymbolName +"]") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        //pnl.Options.ShowCloseButton = false;
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
                                        if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            //pnl.Options.ShowCloseButton = false;
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
                                //                    dockPanel.Width = 400;
                            }
                            tabdet.InitEditValues();
                            dockPanel.Controls.Add(tabdet);

                        }
                        
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                    dockManager1.EndUpdate();
                }



            }
            catch (Exception startnewcompareE)
            {
                logger.Debug(startnewcompareE.Message);
            }

        }

        private void gridViewSymbols_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == gcSymbolHelptext.Name)//gcSymbolsName.Name)
            {
                object o = gridViewSymbols.GetRowCellValue(e.RowHandle, "Category");
                Color c = Color.White;
                if (o != DBNull.Value)
                {
                    if (Convert.ToInt32(o) == (int)XDFCategories.Fuel)
                    {
                        c = Color.LightSteelBlue;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Ignition)
                    {
                        c = Color.LightGreen;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Boost_control)
                    {
                        c = Color.OrangeRed;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Misc)
                    {
                        c = Color.LightGray;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Sensor)
                    {
                        c = Color.Yellow;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Correction)
                    {
                        c = Color.LightPink;
                    }
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Idle)
                    {
                        c = Color.BurlyWood;
                    }
                }
                if (c != Color.White)
                {
                    /*System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, c, Color.White, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(gb, e.Bounds);*/
                    System.Drawing.SolidBrush sb = new SolidBrush(c);
                    e.Graphics.FillRectangle(sb, e.Bounds);
                    sb.Dispose();

                }

            }
        }

        private void btnBrowseLibrary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPartnumberLookup lookup = new frmPartnumberLookup();
            lookup.ShowDialog();
            if (lookup.Open_File)
            {
                string filename = lookup.GetFileToOpen();
                if (filename != string.Empty)
                {
                    CloseProject();
                    OpenWorkingFile(filename);
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


        }

        private void EditTrionicSettingsEasyStyle()
        {

            frmEasyFirmwareSettings info = new frmEasyFirmwareSettings();

            
            info.CPUFrequency = props.CPUspeed;
            info.CarModel = props.Carmodel;
            info.EngineType = props.Enginetype;
            info.Dataname = props.Dataname;
            info.PartNumber = props.Partnumber;
            info.SoftwareID = props.SoftwareID;
            info.RAMLocked = props.RAMlocked;
            info.SecondO2Enabled = props.SecondO2Enable;
            info.Turbo = props.TurboType;
            info.TuningStage = props.TuningStage;
            info.MapSensor = props.MapSensorType;
            info.Injectors = props.InjectorType;
            info.SynchDateTime = props.SyncDateTime;
            info.IsTrionic55 = props.IsTrionic55;
            if(props.HasVSSOptions)
            {
                info.VSSCode = props.VSSCode;
                info.VSSEnabled = props.VSSactive;
                info.TankPressureDiagnosticsEnabled = props.Tank_diagnosticsactive;
            }
            else
            {
                info.DisableVSSOptions();
            }
            
            info.AutoGearBox = props.AutomaticTransmission;
            info.HeatplatesPresent = props.Heatedplates;
            info.AccelEnrichment = props.Accelerationsenrichment;
            info.AdaptionOfIdleControl = props.Adaptionofidlecontrol;
            info.AdaptionWithClosedThrottle = props.Adaptivitywithclosedthrottle;
            info.Adaptivity = props.Adaptivity;
            info.AfterstartEnrichment = props.Afterstartenrichment;
            info.APCControl = props.APCcontrol;
            info.ConstantInjectionTime = props.ConstantinjectiontimeE51;
            info.ConstantInjectionTimeDuringIdle = props.Constantinjtimeduringidle;
            info.DecelEnleanment = props.Decelerationsenleanment;
            info.EnrichmentDuringStart = props.Enrichmentduringstart;
            info.ETS = props.ETS;
            info.FactorToLambdaWhenACEngaged = props.FactortolambdawhenACisengaged;
            info.FactorToLambdaWhenThrottleOpening = props.Factortolambdawhenthrottleopening;
            info.FuelAdjustDuringIdle = props.Fueladjustingduringidle;
            info.Fuelcut = props.Fuelcut;
            info.GlobalAdaption = props.Globaladaption;
            info.HigherIdleDuringStart = props.Higheridleduringstart;
            info.IdleControl = props.Idlecontrol;
            //info.InterpolationOfDelay = GetPgmStatusValue(PGMStatusbit.Interpolationofdelay);
            info.LambdaControl = props.Lambdacontrol;
            info.LambdaControlDuringIdle = props.Lambdacontrolduringidle;
            info.LambdaDuringTransitions = props.Lambdacontrolduringtransients;
            if (props.ExtendedProgramModeOptions)
            {
                info.LoadBufferDuringIdle = props.Loadbufferduringidle;
                info.ConstIdleIgnAngleDuringFirstAndSecondGear = props.Constidleignangleduringgearoneandtwo;
                info.NoFuelCutR12 = props.NofuelcutR12;
                info.AirpumpControl = props.Airpumpcontrol;
                info.NormalAsperatedEngine = props.Normalasperatedengine;
                info.KnockRegulatingDisabled = props.Knockregulatingdisabled;
                //info.ConstantAngle = GetPgmStatusValue(PGMStatusbit.Constantangle);
                info.PurgeValveMY94 = props.PurgevalveMY94;
            }
            else
            {
                info.DisableAdvancedControls();
            }
            info.LoadControl = props.Loadcontrol;
            info.PurgeControl = props.Purge;
            info.Tempcompwithactivelambdacontrol = props.Tempcompwithactivelambdacontrol;
            info.TemperatureCompensation = props.Temperaturecompensation;
            //info.ThrottleAccRetAdjustSimult = GetPgmStatusValue(PGMStatusbit.ThrottleAccRetadjustsimultMY95);
            info.WOTEnrichment = props.WOTenrichment;
            info.SeperateInjectionMapForIdle = props.Usesseparateinjmapduringidle;

            if (info.ShowDialog() == DialogResult.OK)
            {
                // alles wegschrijven
                //props.Carmodel = info.CarModel;
                //props.Enginetype = info.EngineType;
                props.Partnumber = info.PartNumber;
                //props.SoftwareID = info.SoftwareID;
                props.VSSCode = info.VSSCode;
                
                //props.Dataname = info.Dataname;
                props.VSSactive = info.VSSEnabled;
                props.Tank_diagnosticsactive = info.TankPressureDiagnosticsEnabled;
                props.AutomaticTransmission = info.AutoGearBox;
                props.Heatedplates = info.HeatplatesPresent;
                props.Accelerationsenrichment = info.AccelEnrichment;
                props.Adaptionofidlecontrol = info.AdaptionOfIdleControl;
                props.Adaptivity = info.Adaptivity;
                props.Adaptivitywithclosedthrottle = info.AdaptionWithClosedThrottle;
                props.Afterstartenrichment = info.AfterstartEnrichment;
                props.APCcontrol = info.APCControl;
                props.ConstantinjectiontimeE51 = info.ConstantInjectionTime;
                props.Constantinjtimeduringidle = info.ConstantInjectionTimeDuringIdle;
                props.Decelerationsenleanment = info.DecelEnleanment;
                props.Enrichmentduringstart = info.EnrichmentDuringStart;
                props.ETS = info.ETS;
                props.FactortolambdawhenACisengaged = info.FactorToLambdaWhenACEngaged;
                props.Factortolambdawhenthrottleopening = info.FactorToLambdaWhenThrottleOpening;
                props.Fueladjustingduringidle = info.FuelAdjustDuringIdle;
                props.Fuelcut = info.Fuelcut;
                props.Globaladaption = info.GlobalAdaption;
                props.Higheridleduringstart = info.HigherIdleDuringStart;
                props.Idlecontrol = info.IdleControl;
                props.Lambdacontrol = info.LambdaControl;
                props.Lambdacontrolduringidle = info.LambdaControlDuringIdle;
                props.Lambdacontrolduringtransients = info.LambdaDuringTransitions;
                props.Loadcontrol = info.LoadControl;
                props.Purge = info.PurgeControl;
                props.Tempcompwithactivelambdacontrol = info.Tempcompwithactivelambdacontrol;
                props.Temperaturecompensation = info.TemperatureCompensation;
                props.Usesseparateinjmapduringidle = info.SeperateInjectionMapForIdle;
                props.WOTenrichment = info.WOTEnrichment;
                
                
                if (props.ExtendedProgramModeOptions)
                {
                    props.Loadbufferduringidle = info.LoadBufferDuringIdle;
                    props.Constidleignangleduringgearoneandtwo = info.ConstIdleIgnAngleDuringFirstAndSecondGear;
                    props.NofuelcutR12 = info.NoFuelCutR12;
                    props.Airpumpcontrol = info.AirpumpControl;
                    props.Normalasperatedengine = info.NormalAsperatedEngine;
                    props.Knockregulatingdisabled = info.KnockRegulatingDisabled;
                    props.PurgevalveMY94 = info.PurgeValveMY94;
                }
                props.RAMlocked = info.RAMLocked;
                props.SecondO2Enable = info.SecondO2Enabled;
                if (props.HasVSSOptions)
                {
                    props.VSSCode = info.VSSCode;
                    props.VSSactive = info.VSSEnabled;
                    props.Tank_diagnosticsactive = info.TankPressureDiagnosticsEnabled;
                }
                // update checksum
                props.TurboType = info.Turbo;
                props.InjectorType = info.Injectors;
                props.MapSensorType = info.MapSensor;
                props.TuningStage = info.TuningStage;
                m_trionicFile.SetTrionicOptions(props);
                //m_trionicFile.UpdateChecksum();
            }
            else if (info.DialogResult == DialogResult.Abort)
            {
                if (info.Open_file)
                {
                    string filename = info.FiletoOpen;
                    if (filename != string.Empty)
                    {
                        OpenWorkingFile(filename);
                    }
                }
                else if (info.Compare_file)
                {
                    string filename = info.FiletoOpen;
                    if (filename != string.Empty)
                    {
                        CompareToFile(filename);
                    }

                }

            }
        }

        private void btnFirmwareOptions_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_trionicFile.Exists())
            {
                props = m_trionicFile.GetTrionicProperties();
                if (m_appSettings.UseEasyTrionicOptions)
                {
                    EditTrionicSettingsEasyStyle();
                }
                else
                {
                    frmFirmwareSettings _settings = new frmFirmwareSettings();
                    props = m_trionicFile.GetTrionicProperties();
                    _settings.ObjectinView = props;
                    if (_settings.ShowDialog() == DialogResult.OK)
                    {
                        // save the settings into the binary file
                        if (_settings.ObjectinView is Trionic5Properties)
                        {
                            Trionic5Properties _changedprops = (Trionic5Properties)_settings.ObjectinView;
                            m_trionicFile.SetTrionicOptions(_changedprops);
                            props = m_trionicFile.GetTrionicProperties();
                        }
                    }
                }
                props = m_trionicFile.GetTrionicProperties();
            }
        }

        private void btnBinaryCompare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (m_trionicFile.GetFileInfo().Filename != "")
            {
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    bincomp.SetCurrentFilename(m_trionicFile.GetFileInfo().Filename);
                    bincomp.SetCompareFilename(ofd.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No file is currently opened, you need to open a binary file first to compare it to another one!");
            }
        }

        private void btnCompareSRAMSnapshots_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // ask user to point out 2 SRAM files.
            OpenFileDialog ofd1 = new OpenFileDialog();
            
            ofd1.Title = "First SRAM dump...";
            ofd1.Filter = "SRAM dumps|*.ram";
            ofd1.Multiselect = false;
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                string filename_1 = ofd1.FileName;
                ofd1.Title = "Second SRAM dump...";
                if (ofd1.ShowDialog() == DialogResult.OK)
                {
                    string filename_2 = ofd1.FileName;
                    // now compare
                   // frmProgress progress = new frmProgress();
                   // progress.SetProgress("Analyzing SRAM dumps... stand by");
                  //  progress.Show();
                  //  System.Windows.Forms.Application.DoEvents();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                    dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                    dt.Columns.Add("CATEGORY", Type.GetType("System.Int32"));
                    dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                    dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                    dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                    dt.Columns.Add("CATEGORYNAME");
                    dt.Columns.Add("SUBCATEGORYNAME");
                    int cnt = 0;
                    foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                    {
                        int percentage = cnt * 100 / m_trionicFileInformation.SymbolCollection.Count;
                        cnt++;
                        SetTaskProgress(percentage, true);
                        byte[] data_1 = m_trionicFile.ReadDataFromFile(filename_1, (uint)sh.Start_address, (uint)sh.Length);
                        byte[] data_2 = m_trionicFile.ReadDataFromFile(filename_2, (uint)sh.Start_address, (uint)sh.Length);
                        if (data_1.Length != data_2.Length)
                        {
                            frmInfoBox info = new frmInfoBox("Sram data structure invalid... " + sh.Varname);
                            return;
                        }
                        else
                        {
                            double diffperc = 0;
                            int diffabs = 0;
                            double diffavg = 0;
                            bool isdifferent = false;
                            if (m_trionicFileInformation.isSixteenBitTable(sh.Varname))
                            {
                                for (int i = 0; i < data_1.Length; i += 2)
                                {
                                    try
                                    {
                                        int value1 = (int)(byte)data_1.GetValue(i) * 256;
                                        value1 += (int)(byte)data_1.GetValue(i + 1);
                                        int value2 = (int)(byte)data_2.GetValue(i) * 256;
                                        value2 += (int)(byte)data_2.GetValue(i + 1);
                                        if (value1 != value2)
                                        {
                                            isdifferent = true;
                                            diffabs++;
                                        }
                                    }
                                    catch (Exception E)
                                    {
                                        logger.Debug(E.Message);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < data_1.Length; i++)
                                {
                                    if ((byte)data_1.GetValue(i) != (byte)data_2.GetValue(i))
                                    {
                                        isdifferent = true;
                                        diffabs++;
                                    }
                                }
                            }
                            if (isdifferent)
                            {
                                int lengthvalues = sh.Length;
                                if (m_trionicFileInformation.isSixteenBitTable(sh.Varname)) lengthvalues /= 2;
                                diffperc = (diffabs * 100) / lengthvalues;
                                dt.Rows.Add(sh.Varname, sh.Start_address, sh.Flash_start_address, sh.Length, lengthvalues, sh.Helptext, false, (int)sh.Category, diffperc, diffabs, diffavg, sh.Category.ToString().Replace("_", " "), sh.Subcategory.ToString().Replace("_", " "));
                            }
                        }

                    }
                    Trionic5Tools.SymbolCollection compSymbols = new Trionic5Tools.SymbolCollection();
                    AddressLookupCollection compAddressLookup = new AddressLookupCollection();
                    dockManager1.BeginUpdate();
                    try
                    {
                        DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        SRAMCompareResults tabdet = new SRAMCompareResults();
                        tabdet.Dock = DockStyle.Fill;
                        tabdet.Filename1 = filename_1;
                        tabdet.Filename2 = filename_2;
                        tabdet.onSRAMSymbolSelect += new SRAMCompareResults.NotifySRAMSelectSymbol(tabdet_onSRAMSymbolSelect);
                        dockPanel.Controls.Add(tabdet);
                        //dockPanel.DockAsTab(dockPanel1);
                        dockPanel.Text = "SRAM compare results: " + Path.GetFileName(filename_1) + " " + Path.GetFileName(filename_2); 
                        bool isDocked = false;
                        foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                        {
                            if (pnl.Text.StartsWith("SRAM compare results: ") && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
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
                        //CompareSymbolTable(filename, compSymbols, compAddressLookup, tabdet.gridControl1);
                        tabdet.gridControl1.DataSource = dt;
                        //tabdet.CompareTrionic5Tools.SymbolCollection = compSymbols;
                        //tabdet.CompareAddressLookupCollection = compAddressLookup;
                        tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                    dockManager1.EndUpdate();
                    SetStatusText("SRAM compare done");
                    SetTaskProgress(0, false);

                }
            }
        }
        void tabdet_onSRAMSymbolSelect(object sender, SRAMCompareResults.SelectSRAMSymbolEventArgs e)
        {
            
            if (!e.ShowDiffMap)
            {
                StartTableViewer(e.SymbolName);
                StartTableViewerSRAMFile(e.SymbolName, e.Filename1);
                StartTableViewerSRAMFile(e.SymbolName, e.Filename2);
                //StartSRAMTableViewer(e.Filename1, e.SymbolName, e.SymbolLength, e.SymbolAddress, GetSymbolAddressSRAM(m_symbols, e.SymbolName));
                //StartSRAMTableViewer(e.Filename2, e.SymbolName, e.SymbolLength, e.SymbolAddress, GetSymbolAddressSRAM(m_symbols, e.SymbolName));
            }
            else
            {
                //Implement this viewer type in 2.0
                StartSRAMCompareDifferenceViewer(e.SymbolName, e.Filename1, e.Filename2, e.SymbolLength, m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName));
            }
        }

        private void StartSRAMCompareDifferenceViewer(string symbolname, string filename1, string filename2, int length, int sramaddress)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {

                if (pnl.Text == "SRAM symbol difference: " + symbolname + " [" + Path.GetFileName(filename1) + " vs " + Path.GetFileName(filename2) + "]")
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
                    dockPanel.Tag = filename1;

                    //IMapViewer tabdet = new MapViewerEx();
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    }
                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsCompareViewer = true;
                    tabdet.DirectSRAMWriteOnSymbolChange = false;

                    //tabdet.Viewtype = Trionic5Tools.ViewType.Hexadecimal;
                    if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Filename = filename1;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = m_trionicFileInformation.GetSymbolDescription(tabdet.Map_name);
                    tabdet.Map_cat = m_trionicFileInformation.GetSymbolCategory(tabdet.Map_name);
                    tabdet.X_axisvalues = m_trionicFile.GetMapXaxisValues(tabdet.Map_name);
                    tabdet.Y_axisvalues = m_trionicFile.GetMapYaxisValues(tabdet.Map_name);

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    m_trionicFile.GetMapAxisDescriptions(tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;

                    //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    m_trionicFile.GetMapMatrixWitdhByName(tabdet.Map_name, out columns, out rows);
                    int tablewidth = columns;
                    int address = sramaddress;
                    if (address != 0)
                    {
                        while (address > m_trionicFileInformation.Filelength) address -= m_trionicFileInformation.Filelength;
                        tabdet.Map_address = address;
                        tabdet.Map_length = length;
                        byte[] mapdata = m_trionicFile.ReadDataFromFile(filename1, (uint)address, (uint)length);
                        byte[] mapdata2 = m_trionicFile.ReadDataFromFile(filename2, (uint)address, (uint)length);
                        if (mapdata.Length == mapdata2.Length)
                        {
                            if (m_trionicFileInformation.isSixteenBitTable(symbolname))
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt += 2)
                                {
                                    int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                    int value2 = Convert.ToInt16(mapdata2.GetValue(bt)) * 256 + Convert.ToInt16(mapdata2.GetValue(bt + 1));

                                    value1 = (int)Math.Abs(value1 - value2);
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
                                    //logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }

                            tabdet.Map_content = mapdata;
                            tabdet.Correction_factor = m_trionicFile.GetCorrectionFactorForMap(tabdet.Map_name);
                            tabdet.Correction_offset = m_trionicFile.GetOffsetForMap(tabdet.Map_name);
                            tabdet.IsUpsideDown = true;//GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, m_trionicFile.GetFileInfo().isSixteenBitTable(tabdet.Map_name));

                            //tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            //tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            //tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                            //tabdet.ShowTable(columns, isSixteenBitTable(symbolname));
                            tabdet.Dock = DockStyle.Fill;
                            //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);

                            tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                            //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                            //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                            //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                            //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                            //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                            //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                            tabdet.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "SRAM symbol difference: " + symbolname + " [" + Path.GetFileName(filename1) + " vs " + Path.GetFileName(filename2) + "]";
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("SRAM symbol difference: " + symbolname) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        pnl.Options.ShowCloseButton = false;
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
                                        if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            pnl.Options.ShowCloseButton = false;
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

                                //                    dockPanel.Width = 400;
                            }
                            dockPanel.Controls.Add(tabdet);

                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("Map lengths don't match...");
                        }
                        if (dockPanel != null)
                        {
                            dockPanel.Options.ShowCloseButton = false;
                        }
                    }
                }
                catch (Exception E)
                {

                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();


            }
        }

        private void btnImportSRAMSnapshot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Import an SRAM snapshots data into the binfile.
            //First find out what maps / vars are different and prompt the user to make a selection from these.
            // also remember the last set of selected symbols here
            // frmInfoBox info = new frmInfoBox("Still needs to be implemented");
            // let the user select a SRAM file first
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Title = "Select SRAM file...";
            ofd1.Filter = "SRAM dumps|*.ram";
            ofd1.Multiselect = false;
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                // 
                frmMergeAdaptionData frmMergeData = new frmMergeAdaptionData();
                if (frmMergeData.ShowDialog() == DialogResult.OK)
                {
                    //resumeTuning = new System.Data.DataTable();
                    //resumeTuning.Columns.Add("Description");
                    //AddToResumeTable("Importing and merging SRAM file into binary");
                    // well then, open the binary file and extract the relevant data from it
                    File.Copy(m_trionicFileInformation.Filename, Path.GetDirectoryName(m_trionicFileInformation.Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforemergingadaptiondata.bin", true);
                    //AddToResumeTable("Backup file created: " + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforemergingadaptiondata.bin");
                    //AddToResumeTable("Reading adaption data from SRAM file... ");
                    byte[] adaptkorr = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Adapt_korr!"), (uint)m_trionicFileInformation.GetSymbolLength( "Adapt_korr!"));
                    byte[] adaptggr = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Adapt_ggr"), (uint)m_trionicFileInformation.GetSymbolLength( "Adapt_ggr"));
                    byte[] knockcountmap = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Knock_count_map"), (uint)m_trionicFileInformation.GetSymbolLength( "Knock_count_map"));
                    byte[] knockcountcyl1 = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Knock_count_cyl1"), 2);
                    byte[] knockcountcyl2 = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Knock_count_cyl2"), 2);
                    byte[] knockcountcyl3 = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Knock_count_cyl3"), 2);
                    byte[] knockcountcyl4 = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM( "Knock_count_cyl4"), 2);
                    if (frmMergeData.AllowCylinderCorrectionBasedOnKnockInformation)
                    {
                        //AddToResumeTable("Checking individual cylinder knock values... ");

                        int iknockcyl1 = 0;
                        int iknockcyl2 = 0;
                        int iknockcyl3 = 0;
                        int iknockcyl4 = 0;
                        try
                        {
                            iknockcyl1 = Convert.ToInt32(knockcountcyl1[0]) * 256 + Convert.ToInt32(knockcountcyl1[1]);
                            iknockcyl2 = Convert.ToInt32(knockcountcyl2[0]) * 256 + Convert.ToInt32(knockcountcyl2[1]);
                            iknockcyl3 = Convert.ToInt32(knockcountcyl3[0]) * 256 + Convert.ToInt32(knockcountcyl3[1]);
                            iknockcyl4 = Convert.ToInt32(knockcountcyl4[0]) * 256 + Convert.ToInt32(knockcountcyl4[1]);
                        }
                        catch (Exception E)
                        {
                            logger.Debug("Failed to determine knock counter for cylinders: " + E.Message);
                        }
                        // average knock count for cylinders
                        int iknockaverage = (iknockcyl1 + iknockcyl2 + iknockcyl3 + iknockcyl4) / 4;
                        //AddToResumeTable("Average knock count over all cyinders: " + iknockaverage.ToString());

                        byte[] cylindercompensation = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Cyl_komp!"), (uint)m_trionicFileInformation.GetSymbolLength("Cyl_komp!"));
                        if (iknockcyl1 > (iknockaverage * 3))
                        {
                            // add fuel to this cylinder
                            cylindercompensation[0] += 5;
                            //AddToResumeTable("Cylinder 1 had severely more knock than average, increasing fuel compensation slightly");

                        }
                        if (iknockcyl2 > (iknockaverage * 3))
                        {
                            // add fuel to this cylinder
                            cylindercompensation[1] += 5;
                            //AddToResumeTable("Cylinder 2 had severely more knock than average, increasing fuel compensation slightly");
                        }
                        if (iknockcyl3 > (iknockaverage * 3))
                        {
                            // add fuel to this cylinder
                            cylindercompensation[2] += 5;
                            //AddToResumeTable("Cylinder 3 had severely more knock than average, increasing fuel compensation slightly");
                        }
                        if (iknockcyl4 > (iknockaverage * 3))
                        {
                            // add fuel to this cylinder
                            cylindercompensation[3] += 5;
                            //AddToResumeTable("Cylinder 4 had severely more knock than average, increasing fuel compensation slightly");
                        }
                        m_trionicFile.WriteData(cylindercompensation, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Cyl_komp!"));
                    }
                    //<GS-31032010> not allowed in T5.2, should overwite data in insp_mat instead
                    if (frmMergeData.AllowSpotAdaption)
                    {
                        //AddToResumeTable("Checking spot adaption table values...");
                        int cols = 0;
                        int rows = 0;
                        m_trionicFile.GetMapMatrixWitdhByName("Adapt_korr!", out cols, out rows);
                        // read the fuel map as well
                        byte[] insp_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                        // now handle adaptkorr
                        for (int rt = 0; rt < rows; rt++)
                        {
                            for (int ct = 0; ct < cols; ct++)
                            {
                                //if (adaptggr[rt * cols + ct] > 0)
                                {
                                    byte correctionbyte = adaptkorr[rt * cols + ct];
                                    // offset = 0.75
                                    // correction = 0.001953125; // 1/512
                                    double correctionvalue = (Convert.ToDouble(correctionbyte) * 0.001953125) + 0.75;
                                    // now save it into insp_mat at the correct location

                                    byte valuefrominspmat = insp_mat[rt * cols + ct];//readbytefromfile(m_currentfile, GetSymbolAddress(m_symbols, "Insp_mat!") + (rt * cols) + ct);
                                    correctionvalue *= valuefrominspmat;
                                    byte bcorrvalue = Convert.ToByte(correctionvalue);
                                    if (bcorrvalue != valuefrominspmat)
                                    {
                                        //AddToResumeTable("Updating injection table at row " + rt.ToString() + " and column " + ct.ToString() + " seting value: " + correctionvalue.ToString());
                                        insp_mat[(rt * cols) + ct] = bcorrvalue;
                                        //writebyteinfile(m_currentfile, GetSymbolAddress(m_symbols, "Insp_mat!") + (rt * cols) + ct, Convert.ToByte(correctionvalue));
                                    }
                                }
                            }
                        }
                        m_trionicFile.WriteData(insp_mat, (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap()));
                    }
                    if (frmMergeData.AllowLongTermFuelTrimAdaption)
                    {
                        // adjust long term fuel trim
                        //AddToResumeTable("Overwriting long term fuel trim in binary");

                        byte[] longtermfueltrim = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_injfaktor!"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_injfaktor!"));
                        m_trionicFile.WriteData(longtermfueltrim, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Adapt_injfaktor!"));
                    }
                    if (frmMergeData.AllowIdleFuelTrimAdaption)
                    {
                        // adjust long term fuel trim
                        //AddToResumeTable("Overwriting idle fuel trim in binary");
                        byte[] idlefueltrim = m_trionicFile.ReadDataFromFile(ofd1.FileName, (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_inj_imat!"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_inj_imat!"));
                        m_trionicFile.WriteData(idlefueltrim, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Adapt_inj_imat!"));
                    }
                    if (frmMergeData.AllowIgnitionAdaptionBasedOnKnockInformation)
                    {
                        //AddToResumeTable("Checking knock count map for excessive knock and retarding ignition timing");

                        // read knock map values and adjust timing accordingly
                        int cols = 0;
                        int rows = 0;
                        if (m_trionicFileInformation.isSixteenBitTable("Knock_count_map")) rows /= 2;
                        byte[] ign_map_0 = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Ign_map_0!"), (uint)m_trionicFileInformation.GetSymbolLength("Ign_map_0!"));
                        for (int rt = 0; rt < rows; rt++)
                        {
                            for (int ct = 0; ct < cols; ct++)
                            {
                                int iknockcount = ((int)knockcountmap[(rt * cols * 2) + (ct * 2)] * 256) + (int)knockcountmap[(rt * cols * 2) + (ct * 2) + 1];
                                if (iknockcount > 5) // at least 5 knocks
                                {
                                    // retard ignition at this point

                                    int ign_map_0_value = Convert.ToInt32(ign_map_0[(rt * cols * 2) + (ct * 2)]) * 256 + Convert.ToInt32(ign_map_0[(rt * cols * 2) + (ct * 2) + 1]);
                                    //logger.Debug("Ignition advance at knocking position is " + ign_map_0_value.ToString());
                                    if (iknockcount < 50)
                                    {
                                        ign_map_0_value -= 5; // decrease by .5 degree
                                        //AddToResumeTable("Decreased ignition timing by 0.5 degrees at row " + rt.ToString() + " and column " + ct.ToString() + ", result = " + ign_map_0_value.ToString());
                                    }
                                    else
                                    {
                                        ign_map_0_value -= 10; // decrease by 1 degree
                                        //AddToResumeTable("Decreased ignition timing by 1.0 degrees at row " + rt.ToString() + " and column " + ct.ToString() + ", result = " + ign_map_0_value.ToString());
                                    }
                                    byte b1 = (byte)(ign_map_0_value / 256);
                                    byte b2 = (byte)(ign_map_0_value - ((int)b1 * 256));
                                    ign_map_0[(rt * cols * 2) + (ct * 2)] = b1;
                                    ign_map_0[(rt * cols * 2) + (ct * 2) + 1] = b2;
                                }
                            }
                        }
                        m_trionicFile.WriteData(ign_map_0, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Ign_map_0!"));
                    }
                    //AddToResumeTable("SRAM merge done.");
                    //TuningReport tuningrep = new TuningReport();
                    //tuningrep.DataSource = resumeTuning;
                    //tuningrep.CreateReport();
                    //tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);
                    frmInfoBox info = new frmInfoBox("Data was imported");
                }
            }
        }

        private void btnCompareSRAMtoBinary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Compare the data from an sram snapshot to the binary file
            //frmInfoBox info = new frmInfoBox("Still needs to be implemented");
            // read all symbols from list and compare to sram equivalent!
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Title = "Select SRAM file to compare...";
            ofd1.Filter = "SRAM dumps|*.ram";
            ofd1.Multiselect = false;
            
            
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                StartCompareToSRAMFile(ofd1.FileName);
                
            }
        }

        private void StartCompareToSRAMFile(string sramfilename)
        {
            Trionic5Tools.SymbolCollection scdiff = new Trionic5Tools.SymbolCollection();
            foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
            {
                if (sh.Flash_start_address > 0 && sh.Start_address > 0)
                {
                    // get sram content and binfile content for this symbol

                    byte[] sramsymbol = m_trionicFile.ReadDataFromFile(sramfilename, (uint)sh.Start_address, (uint)sh.Length);
                    byte[] flashsymbol = m_trionicFile.ReadData((uint)sh.Flash_start_address, (uint)sh.Length);
                    int bdifferent = 0;
                    for (int btel = 0; btel < sh.Length; btel++)
                    {
                        if (sramsymbol[btel] != flashsymbol[btel])
                        {
                            bdifferent++;
                        }
                    }
                    if (bdifferent > 0)
                    {
                        // symbol is not equal!
                        scdiff.Add(sh);
                    }
                }
            }
            dockManager1.BeginUpdate();
            try
            {
                DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                CompareResults tabdet = new CompareResults();
                tabdet.Dock = DockStyle.Fill;
                tabdet.Filename = sramfilename;

                tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelectRAM);
                dockPanel.Controls.Add(tabdet);
                //dockPanel.DockAsTab(dockPanel1);
                dockPanel.Text = "SRAM <> BIN Compare results: " + Path.GetFileName(sramfilename);
                dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Left, 1);
                dockPanel.Width = 700;
                //CompareSymbolTable(filename, compSymbols, compAddressLookup, tabdet.gridControl1);
                tabdet.CompareSymbolCollection = scdiff;

                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("SYMBOLNAME");
                dt.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                dt.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                dt.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                dt.Columns.Add("DESCRIPTION");
                dt.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                dt.Columns.Add("CATEGORY", Type.GetType("System.Int32"));
                dt.Columns.Add("DIFFPERCENTAGE", Type.GetType("System.Double"));
                dt.Columns.Add("DIFFABSOLUTE", Type.GetType("System.Int32"));
                dt.Columns.Add("DIFFAVERAGE", Type.GetType("System.Double"));
                dt.Columns.Add("CATEGORYNAME");
                dt.Columns.Add("SUBCATEGORYNAME");

                foreach (Trionic5Tools.SymbolHelper sh in scdiff)
                {
                    float diffperc = 0;
                    int diffabs = 0;
                    float diffavg = 0;
                    dt.Rows.Add(sh.Varname, sh.Start_address, sh.Flash_start_address, sh.Length, sh.Length, sh.Helptext, false, (int)sh.Category, diffperc, diffabs, diffavg, sh.Category.ToString().Replace("_", " "), sh.Subcategory.ToString().Replace("_", " "));
                }
                tabdet.gridControl1.DataSource = dt;

                //tabdet.CompareAddressLookupCollection = compAddressLookup;
                tabdet.OpenGridViewGroups(tabdet.gridControl1, 1);
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            dockManager1.EndUpdate();
        }

        void tabdet_onSymbolSelectRAM(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            //<GS-22042010> should be compare viewer for Pgm_mod / Pgm_status as well
            if (e.SymbolName == m_trionicFileInformation.GetProgramModeSymbol())
            {
                frmEasyFirmwareInfo info = new frmEasyFirmwareInfo();
                info.Pgm_mod = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetProgramModeSymbol()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol()));
                info.SetPrimarySourceName(Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename));
                info.Pgm_mod2 = m_trionicFile.ReadDataFromFile(e.Filename, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), (uint)e.SymbolLength);
                info.SetSecondarySourceName("SRAM " + Path.GetFileNameWithoutExtension(e.Filename));
                info.ShowDialog();
            }
            else if (!e.ShowDiffMap)
            {
                StartTableViewer(e.SymbolName); // normal viewer
                StartTableViewerSRAMFile(e.SymbolName, e.Filename);
            }
            else
            {
                //show difference between SRAM and binary file
                StartSRAMToFlashCompareDifferenceViewer(e.SymbolName, e.Filename, m_trionicFileInformation.Filename, e.SymbolLength, m_trionicFileInformation.GetSymbolAddressSRAM(e.SymbolName), m_trionicFileInformation.GetSymbolAddressFlash(e.SymbolName));
                

            }
        }

        private void StartSRAMToFlashCompareDifferenceViewer(string symbolname, string sramfilename, string flashfilename, int length, int sramaddress, int flashaddress)
        {
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
            {

                if (pnl.Text == "SRAM-Flash symbol difference: " + symbolname + " [" + Path.GetFileName(sramfilename) + " vs " + Path.GetFileName(flashfilename) + "]")
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
                    dockPanel.Tag = sramfilename;

                    //IMapViewer mv = new MapViewerEx();
                    IMapViewer mv;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        mv = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        mv = new MapViewer();
                    }
                    else
                    {
                        mv = new SimpleMapViewer();
                    }
                    mv.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    mv.IsCompareViewer = true;
                    mv.GraphVisible = m_appSettings.ShowGraphs;
                    //TryTpShowTouchScreenInput();

                    mv.DirectSRAMWriteOnSymbolChange = false;

                    mv.Viewtype = Trionic5Tools.ViewType.Decimal;
                    mv.DisableColors = m_appSettings.DisableMapviewerColors;
                    mv.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    mv.GraphVisible = m_appSettings.ShowGraphs;
                    mv.IsRedWhite = m_appSettings.ShowRedWhite;
                    mv.Filename = sramfilename;
                    mv.Map_name = symbolname;
                    mv.Map_descr = m_trionicFileInformation.GetSymbolDescription(mv.Map_name);
                    mv.Map_cat = m_trionicFileInformation.GetSymbolCategory(mv.Map_name);
                    mv.X_axisvalues = m_trionicFile.GetMapXaxisValues(mv.Map_name);
                    mv.Y_axisvalues = m_trionicFile.GetMapYaxisValues(mv.Map_name);
                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    m_trionicFile.GetMapAxisDescriptions(mv.Map_name, out xdescr, out ydescr, out zdescr);
                    mv.X_axis_name = xdescr;
                    mv.Y_axis_name = ydescr;
                    mv.Z_axis_name = zdescr;

                    //mv.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    m_trionicFile.GetMapMatrixWitdhByName(mv.Map_name, out columns, out rows);
                    int tablewidth = columns;
                    //int tablewidth = m_trionicFileInformation.GetTab.GetTableGetTableMatrixWitdhByName(m_trionicFileInformation.Filename, mv.Map_name, out columns, out rows);
                    //int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, mv.Map_name, out columns, out rows);
                    int address = sramaddress;
                    if (address != 0)
                    {
                        while (address > m_trionicFileInformation.Filelength) address -= m_trionicFileInformation.Filelength;
                        mv.Map_address = address;
                        mv.Map_length = length;
                        byte[] mapdata = m_trionicFile.ReadDataFromFile(sramfilename, (uint)sramaddress, (uint)length);
                        byte[] mapdata2 = m_trionicFile.ReadDataFromFile(flashfilename, (uint)flashaddress, (uint)length);
                        byte[] mapdataOri = m_trionicFile.ReadDataFromFile(sramfilename, (uint)sramaddress, (uint)length);
                        mv.Map_original_content = mapdata;
                        mv.Map_compare_content = mapdata2;

                        if (mapdata.Length == mapdata2.Length)
                        {
                            if (m_trionicFile.IsTableSixteenBits(symbolname))
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt += 2)
                                {
                                    int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                    int value2 = Convert.ToInt16(mapdata2.GetValue(bt)) * 256 + Convert.ToInt16(mapdata2.GetValue(bt + 1));

                                    value1 = (int)Math.Abs(value1 - value2);
                                    byte v1 = (byte)(value1 / 256);
                                    byte v2 = (byte)(value1 - (int)v1 * 256);
                                    mapdataOri.SetValue(v1, bt);
                                    mapdataOri.SetValue(v2, bt + 1);
                                }
                            }
                            else
                            {
                                for (int bt = 0; bt < mapdata2.Length; bt++)
                                {
                                    //logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdataOri.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }

                            mv.Map_content = mapdataOri;
                            TryToAddOpenLoopTables(mv);
                            // ori and changed

                            mv.Correction_factor = m_trionicFile.GetCorrectionFactorForMap(mv.Map_name);
                            mv.Correction_offset = m_trionicFile.GetOffsetForMap(mv.Map_name);
                            mv.IsUpsideDown = true;// GetMapUpsideDown(mv.Map_name);
                            mv.ShowTable(columns, m_trionicFile.IsTableSixteenBits(symbolname));
                            mv.Dock = DockStyle.Fill;
                            //mv.onSymbolSave += new MapViewer.NotifySaveSymbol(mv_onSymbolSave);
                            mv.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                            mv.onAxisEditorRequested += new IMapViewer.AxisEditorRequested(mv_onAxisEditorRequested);
                            //mv.onAxisLock += new MapViewer.NotifyAxisLock(mv_onAxisLock);
                            //mv.onSliderMove += new MapViewer.NotifySliderMove(mv_onSliderMove);
                            mv.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            mv.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            mv.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                            //mv.onSplitterMoved += new MapViewer.SplitterMoved(mv_onSplitterMoved);
                            //mv.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);
                            //mv.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(mv_onGraphSelectionChanged);
                            //mv.onViewTypeChanged += new MapViewer.ViewTypeChanged(mv_onViewTypeChanged);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "SRAM-Flash symbol difference: " + symbolname + " [" + Path.GetFileName(sramfilename) + " vs " + Path.GetFileName(flashfilename) + "]";
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("SRAM-Flash symbol difference: " + symbolname) && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                    {
                                        dockPanel.DockAsTab(pnl, 0);
                                        pnl.Options.ShowCloseButton = false;
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
                                        if ((string)pnl.Tag == m_trionicFileInformation.Filename && pnl != dockPanel && (pnl.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Visible))
                                        {
                                            dockPanel.DockAsTab(pnl, 0);
                                            pnl.Options.ShowCloseButton = false;
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
                                    if (mv.X_axisvalues.Length > 0)
                                    {
                                        dockPanel.Width = 30 + ((mv.X_axisvalues.Length + 1) * 35);
                                    }
                                    else
                                    {
                                        //dockPanel.Width = this.Width - dockSymbols.Width - 10;

                                    }
                                }
                                if (dockPanel.Width < 400) dockPanel.Width = 400;

                                //                    dockPanel.Width = 400;
                            }
                            dockPanel.Controls.Add(mv);

                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("Map lengths don't match...");
                        }
                    }
                }
                catch (Exception E)
                {

                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void btnBinaryCompareSRAMsnapshots_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // do a binary compare of sram dumps to see what's different
            OpenFileDialog ofdram1 = new OpenFileDialog();
            OpenFileDialog ofdram2 = new OpenFileDialog();
            ofdram1.Filter = "SRAM dumps|*.ram";
            ofdram2.Filter = "SRAM dumps|*.ram";
            ofdram1.Multiselect = false;
            ofdram2.Multiselect = false;

            if (ofdram1.ShowDialog() == DialogResult.OK)
            {
                if (ofdram2.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    bincomp.SetCurrentFilename(ofdram1.FileName);
                    bincomp.SetCompareFilename(ofdram2.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
        }

        private void btnMergeBinaryFiles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBinmerger frmmerger = new frmBinmerger();
            frmmerger.ShowDialog();

        }

        private void btnSplitBinaryFiles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_trionicFile.GetFileInfo().Filename != "")
            {
                if (File.Exists(m_trionicFile.GetFileInfo().Filename))
                {
                    string path = Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename);
                    FileInfo fi = new FileInfo(m_trionicFile.GetFileInfo().Filename);
                    FileStream fs = File.Create(path + "\\chip2.bin");
                    BinaryWriter bw = new BinaryWriter(fs);
                    FileStream fs2 = File.Create(path + "\\chip1.bin");
                    BinaryWriter bw2 = new BinaryWriter(fs2);
                    FileStream fsi1 = File.OpenRead(m_trionicFile.GetFileInfo().Filename);
                    BinaryReader br1 = new BinaryReader(fsi1);
                    bool toggle = false;
                    for (int tel = 0; tel < fi.Length; tel++)
                    {
                        Byte ib1 = br1.ReadByte();
                        if (!toggle)
                        {
                            toggle = true;
                            bw.Write(ib1);
                        }
                        else
                        {
                            toggle = false;
                            bw2.Write(ib1);
                        }
                    }

                    bw.Close();
                    bw2.Close();
                    fs.Close();
                    fs2.Close();
                    fsi1.Close();
                    br1.Close();
                    frmInfoBox info = new frmInfoBox("File split to chip1.bin and chip2.bin");
                }
            }
        }

        private void RunTuningWizard()
        {
            props = m_trionicFile.GetTrionicProperties();
            if ((int)props.TuningStage > 3)
            {
                frmInfoBox infofail = new frmInfoBox("This file has already been tuned to a higher stage, the tuning wizard will not be started");
                return;
            }
            
            frmTuningWizard tunWiz = new frmTuningWizard();
            int _currStage = (int)props.TuningStage;
            if (_currStage == 0) _currStage++;
            tunWiz.TuningStage = _currStage;
            tunWiz.SetMapSensorType(props.MapSensorType);
            tunWiz.SetInjectorType(props.InjectorType);
            tunWiz.SetTurboType(props.TurboType);
            ECUFileType fileType = m_trionicFile.DetermineFileType();
            int frek230 = m_trionicFile.GetSymbolAsInt("Frek_230!");
            int frek250 = m_trionicFile.GetSymbolAsInt("Frek_250!");

            int knockTime = m_trionicFile.GetSymbolAsInt("Knock_matrix_time!");
            tunWiz.SetKnockTime(knockTime);
            int rpmLimit = m_trionicFile.GetSymbolAsInt("Rpm_max!");
            tunWiz.SetRPMLimiter(rpmLimit);

            if (fileType == ECUFileType.Trionic52File)
            {
                if (frek230 == 728 || frek250 == 935)
                {
                    //dtReport.Rows.Add("APC valve type: Trionic 5");
                    tunWiz.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunWiz.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            else if (fileType == ECUFileType.Trionic55File)
            {
                if (frek230 == 90 || frek250 == 70)
                {
                    tunWiz.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunWiz.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            if (tunWiz.ShowDialog() == DialogResult.OK)
            {
                Application.DoEvents();
                Trionic5Tuner _tuner = new Trionic5Tuner();
                _tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                frmInfoBox info;
                TuningResult result = TuningResult.TuningFailed;
                if (tunWiz.TuningStage == 4)
                {
                    result = _tuner.FreeTuneBinary(m_trionicFile, tunWiz.GetPeakTorque(), tunWiz.GetPeakBoost(), tunWiz.IsTorqueBased, tunWiz.GetMapSensorType(), tunWiz.GetTurboType(), tunWiz.GetInjectorType(), tunWiz.GetBCVType(), tunWiz.GetRPMLimiter(), tunWiz.GetKnockTime());
                }
                else
                {
                    result = _tuner.TuneFileToStage(tunWiz.TuningStage, m_trionicFile.GetFileInfo().Filename, m_trionicFile, m_trionicFileInformation, true);

                }
                switch (result)
                {
                    case TuningResult.TuningFailed:
                        info = new frmInfoBox("Tuning of the binary file failed!");
                        break;
                    case TuningResult.TuningFailedAlreadyTuned:
                        info = new frmInfoBox("Your binary file was already tuned!");
                        break;
                    case TuningResult.TuningFailedThreebarSensor:
                        info = new frmInfoBox("Your binary file was already tuned (3 bar sensor)!");
                        break;
                    case TuningResult.TuningSuccess:
                        // show report
                        props = m_trionicFile.GetTrionicProperties();
                        TuningReport tuningrep = new TuningReport();
                        tuningrep.ReportTitle = "Tuning report (stage " + tunWiz.TuningStage.ToString() + ")";
                        tuningrep.SetDataSource(_tuner.Resume.ResumeTuning);
                        tuningrep.CreateReport();
                        tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);
                        //info = new frmInfoBox("Your binary file was succesfully tuned to stage 1");
                        break;
                    case TuningResult.TuningCancelled:
                        // show report
                        info = new frmInfoBox("Tuning process cancelled by user");
                        break;
                }
            }
        }

        private void btnTuneToStage1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RunTuningWizard();
        }

        private void btnTuneToStage2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Trionic5Tuner _tuner = new Trionic5Tuner();
            _tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            frmInfoBox info;
            TuningResult result = _tuner.TuneFileToStage(2, m_trionicFile.GetFileInfo().Filename, m_trionicFile, m_trionicFileInformation, false);
            switch (result)
            {
                case TuningResult.TuningFailed:
                    info = new frmInfoBox("Tuning of the binary file failed!");
                    break;
                case TuningResult.TuningFailedAlreadyTuned:
                    info = new frmInfoBox("Your binary file was already tuned!");
                    break;
                case TuningResult.TuningFailedThreebarSensor:
                    info = new frmInfoBox("Your binary file was already tuned (3 bar sensor)!");
                    break;
                case TuningResult.TuningSuccess:
                    // show report
                    props = m_trionicFile.GetTrionicProperties();
                    //info = new frmInfoBox("Your binary file was succesfully tuned to stage 2");
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Tuning report (stage II)";
                    tuningrep.SetDataSource(_tuner.Resume.ResumeTuning);
                    tuningrep.CreateReport();
                    tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);
                    break;
                case TuningResult.TuningCancelled:
                    // show report
                    info = new frmInfoBox("Tuning process cancelled by user");
                    break;

            }
        }

        private void btnTuneToStage3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Trionic5Tuner _tuner = new Trionic5Tuner();
            _tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            frmInfoBox info;
            TuningResult result = _tuner.TuneFileToStage(3, m_trionicFile.GetFileInfo().Filename, m_trionicFile, m_trionicFileInformation, false);
            switch (result)
            {
                case TuningResult.TuningFailed:
                    info = new frmInfoBox("Tuning of the binary file failed!");
                    break;
                case TuningResult.TuningFailedAlreadyTuned:
                    info = new frmInfoBox("Your binary file was already tuned!");
                    break;
                case TuningResult.TuningFailedThreebarSensor:
                    info = new frmInfoBox("Your binary file was already tuned (3 bar sensor)!");
                    break;
                case TuningResult.TuningSuccess:
                    // show report
                    props = m_trionicFile.GetTrionicProperties();
                    //info = new frmInfoBox("Your binary file was succesfully tuned to stage 3");
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Tuning report (stage III)";
                    tuningrep.SetDataSource(_tuner.Resume.ResumeTuning);
                    tuningrep.CreateReport();
                    tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);
                    break;
                case TuningResult.TuningCancelled:
                    // show report
                    info = new frmInfoBox("Tuning process cancelled by user");
                    break;

            }
        }

        private void btnTuneToThreeBarSensor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void btnTuneToLargerInjectors_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //TODO: implement a wizard for usage with larger/different injectors.
            // it should contain a list with known injectors
            // and adjust not only inj_const but also battery correction maps.
            //frmInfoBox info = new frmInfoBox("Still needs to be implemented");
            // guide the user along all relevant maps
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    frmInjectorWizard injWiz = new frmInjectorWizard();
                    injWiz.InjectorType = props.InjectorType;
                    byte[] data = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectorConstant()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectorConstant()));
                    int inj_konst = Convert.ToInt32(data.GetValue(0));
                    injWiz.Batt_korr_map = m_trionicFile.GetSymbolAsIntArray(m_trionicFileInformation.GetBatteryCorrectionMap());
                    injWiz.InjectorConstant = inj_konst;
                    float startInsp = (float)m_trionicFile.GetSymbolAsInt("Start_insp!");
                    injWiz.Crankfactor = startInsp * 0.004F;
                    if (injWiz.ShowDialog() == DialogResult.OK)
                    {
                        Application.DoEvents();
                        // save data
                        // save battery correction map
                        byte[] batt_korr_new = injWiz.GetBatteryCorrectionMap();
                        m_trionicFile.WriteDataNoLog(batt_korr_new, (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetBatteryCorrectionMap()));
                        // save injector constant
                        byte[] binj_konst = new byte[1];
                        binj_konst.SetValue(Convert.ToByte(injWiz.InjectorConstant), 0);
                        m_trionicFile.WriteDataNoLog(binj_konst, (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectorConstant()));
                        // save crank factor
                        byte[] crank_factor_new = injWiz.GetCrankFactor();
                        m_trionicFile.WriteDataNoLog(crank_factor_new, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Start_insp!"));
                        props.InjectorType = injWiz.InjectorType;
                        m_trionicFile.SetTrionicOptions(props);

                    }
                }
            }
        }

        private void btnTuneForE85Fuel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_trionicFile.Exists())
            {
                frmE85Wizard e85Wiz = new frmE85Wizard();
                if (e85Wiz.ShowDialog() == DialogResult.OK)
                {
                    Application.DoEvents();
                    if (m_CurrentWorkingProject != "")
                    {
                        if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups");
                        string filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                        File.Copy(GetBinaryForProject(m_CurrentWorkingProject), filename);
                    }
                    else
                    {
                        // create a backup file in the current working folder
                        string filename = Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                        File.Copy(m_trionicFile.GetFileInfo().Filename, filename);
                    }

                    // do the tune!
                    Trionic5Tuner _tuner = new Trionic5Tuner();
                    Trionic5Properties properties = m_trionicFile.GetTrionicProperties();
                    _tuner.ConvertToE85(m_trionicFile);
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Convert to E85 report";
                    tuningrep.SetDataSource(_tuner.Resume.ResumeTuning);
                    tuningrep.CreateReport();
                    tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);
                    //frmInfoBox info = new frmInfoBox("Conversion to E85 fuel done");
                }
            }
        }

        private void SyncMaps(SyncOption which_direction)
        {
            //synchronize maps with the ECU
            //frmInfoBox info = new frmInfoBox("Still needs to be implemented");
            // sync ECU with binary file
            int cnt = 0;
            _ecuConnection.ProhibitRead = true;
            Thread.Sleep(100);
            if (_ecuConnection.Opened)
            {
                if (m_CurrentWorkingProject != string.Empty)
                {
                    m_ProjectLog.WriteLogbookEntry(LogbookEntryType.SynchronizationStarted, which_direction.ToString());
                }
                foreach (Trionic5Tools.SymbolHelper sh in m_trionicFile.GetFileInfo().SymbolCollection)
                {
                    if (sh.Flash_start_address > 0)
                    {
                        cnt++;
                    }
                }
                int currcnt = 0;
                //frmProgress progress = new frmProgress();
                //progress.SetProgress("Synchronizing ECU with binary");
                //progress.SetProgressPercentage(0);
                //progress.Show();
                //preventUpdatePaint = true;
                foreach (Trionic5Tools.SymbolHelper sh in m_trionicFile.GetFileInfo().SymbolCollection)
                {
                    if (sh.Flash_start_address > 0)
                    {
                        currcnt++;
                        int percentage = (currcnt * 100) / cnt;
                        //progress.SetProgressPercentage(percentage);
                        SetStatusText("Sync: " + percentage.ToString() + "%");
                        SetTaskProgress(percentage, true);
                        // <GS-08022011> if the user switch offline in the meantime
                        // stop sync action to prevent damage to data
                        if (_ECUmode == OperationMode.ModeOffline) break;
                        //if (_APPmode == OperationMode.ModeOffline) break;
                        // read ECU map
                        if (sh.Start_address > 0 && sh.Length > 0)
                        {
                            //progress.SetProgress("Verifying " + sh.Varname);
                            logger.Debug("map: " + sh.Varname);
                            Thread.Sleep(50);
                            byte[] symboldataECU = _ecuConnection.ReadSymbolDataNoProhibitRead(sh.Varname, (uint)sh.Start_address, (uint)sh.Length);

                            byte[] symboldataBIN = m_trionicFile.ReadData((uint)sh.Flash_start_address, (uint)sh.Length);
                            bool data_matches = true;
                            if (symboldataBIN.Length == symboldataECU.Length)
                            {
                                for (int t = 0; t < symboldataBIN.Length; t++)
                                {
                                    if ((byte)symboldataBIN.GetValue(t) != (byte)symboldataECU.GetValue(t))
                                    {
                                        data_matches = false;
                                        break;
                                    }
                                }
                                if (!data_matches)
                                {
                                    if (which_direction == SyncOption.ToECU)
                                    {
                                        //progress.SetProgress("Writing " + sh.Varname + " to ECU");
                                        // <GS-08022011> if the user switch offline in the meantime
                                        // stop sync action to prevent damage to data
                                        if (_ECUmode == OperationMode.ModeOffline) break;
                                        //if (_APPmode == OperationMode.ModeOffline) break;
                                        _ecuConnection.WriteSymbolDataForced(sh.Start_address, sh.Length, symboldataBIN);
                                    }
                                    else
                                    {
                                        // <GS-08022011> if the user switch offline in the meantime
                                        // stop sync action to prevent damage to data
                                        if (_ECUmode == OperationMode.ModeOffline) break;
                                        //if (_APPmode == OperationMode.ModeOffline) break;
                                        m_trionicFile.WriteDataNoCounterIncrease(symboldataECU, (uint)sh.Flash_start_address);
                                    }
                                }
                            }
                        }
                    }
                }
                SetStatusText("Synchronized");
                SetTaskProgress(0, false);
                //              progress.Close();
                //                preventUpdatePaint = false;
            }
            _ecuConnection.ProhibitRead = false;
        }

        private void btnSynchronizeMaps_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_ecuConnection.Opened)
            {
                DateTime dt_ecu = _ecuConnection.GetMemorySyncDate();
                DateTime dt_file = m_trionicFile.GetMemorySyncDate();
                if (dt_ecu > dt_file)
                {
                    // sync ecu to file without changing sync dates
                    SyncMaps(SyncOption.ToFile);
                    m_trionicFile.SetMemorySyncDate(dt_ecu);

                }
                else if (dt_file > dt_ecu)
                {
                    // sync file to ecu without changing sync dates
                    SyncMaps(SyncOption.ToECU);
                    _ecuConnection.SetMemorySyncDate(dt_file);
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Synchronization not needed");
                }
            }
            else 
            {
                    frmInfoBox info = new frmInfoBox("No connection to ECU available");
            }
        }

        private void btnBoostAdaptionWizard_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // <GS-10012011> create new wizard here

            Trionic5Tuner tuner = new Trionic5Tuner();
            tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            //Make this stuff user selectable!
            frmBoostAdaptionWizard boostAdaptWiz = new frmBoostAdaptionWizard();
            boostAdaptWiz.ManualRPMLow = m_trionicFile.GetManualRpmLow();
            boostAdaptWiz.ManualRPMHigh = m_trionicFile.GetManualRpmHigh();
            boostAdaptWiz.AutomaticRPMLow = m_trionicFile.GetAutoRpmLow();
            boostAdaptWiz.AutomaticRPMHigh = m_trionicFile.GetAutoRpmHigh();
            boostAdaptWiz.BoostError = m_trionicFile.GetMaxBoostError();
            if (boostAdaptWiz.ShowDialog() == DialogResult.OK)
            {
                tuner.Ori_boostError = m_trionicFile.GetMaxBoostError();
                tuner.Ori_rpmHighAut = m_trionicFile.GetAutoRpmHigh();
                tuner.Ori_rpmHighManual = m_trionicFile.GetManualRpmHigh();
                tuner.Ori_rpmLowAut = m_trionicFile.GetAutoRpmLow();
                tuner.Ori_rpmLowManual = m_trionicFile.GetManualRpmLow();
                if (tuner.SetBoostAdaptionParameters(boostAdaptWiz.ManualRPMLow, boostAdaptWiz.ManualRPMHigh, boostAdaptWiz.AutomaticRPMLow, boostAdaptWiz.AutomaticRPMHigh, boostAdaptWiz.BoostError, m_trionicFileInformation))
                {
                    // save
                    m_trionicFile.SetAutoRpmHigh(boostAdaptWiz.AutomaticRPMHigh);
                    m_trionicFile.SetAutoRpmLow(boostAdaptWiz.AutomaticRPMLow);
                    m_trionicFile.SetManualRpmHigh(boostAdaptWiz.ManualRPMHigh);
                    m_trionicFile.SetManualRpmLow(boostAdaptWiz.ManualRPMLow);
                    m_trionicFile.SetMaxBoostError(boostAdaptWiz.BoostError);
                    frmInfoBox info = new frmInfoBox("Boost adaption ranges were set");
                }
                m_trionicFile.UpdateChecksum(); //<GS-28112009>
                
            }
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

        private void btnCreateProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show the project properties screen for the user to fill in
            // if a bin file is loaded, ask the user whether this should be the new projects binary file
            // the project XML should contain a reference to this binfile as well as a lot of other stuff
            frmProjectProperties projectprops = new frmProjectProperties();
            if (m_trionicFile != null)
            {
                projectprops.BinaryFile = m_trionicFileInformation.Filename;
                projectprops.CarModel = m_trionicFile.GetTrionicProperties().Carmodel.Trim();
                projectprops.ProjectName = m_trionicFile.GetTrionicProperties().Enginetype.Trim() + " " + m_trionicFile.GetTrionicProperties().Partnumber.Trim() + " " + m_trionicFile.GetTrionicProperties().SoftwareID.Trim();
            }
            if (projectprops.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
                // create a new folder with these project properties.
                // also copy the binary file into the subfolder for this project
                if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName)))
                {
                    frmInfoBox info = new frmInfoBox("The chosen projectname already exists, please choose another one");
                    //TODO: reshow the dialog if the project name already exists
                }
                else
                {
                    // create the project
                    Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName));
                    // copy the selected binary file to this folder
                    string binfilename = m_appSettings.ProjectFolder + "\\" + MakeDirName(projectprops.ProjectName) + "\\" + Path.GetFileName(projectprops.BinaryFile);
                    File.Copy(projectprops.BinaryFile, binfilename);
                    // now create the projectproperties.xml in this new folder
                    DataTable dtProps = new DataTable("T5PROJECT");
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

        private TrionicTransactionLog m_ProjectTransactionLog;

        private bool OpenProject(string projectname)
        {
            //TODO: Are there pending changes in the optionally currently opened binary file / project?
            // open a selected project
            bool retval = false;
            if (Directory.Exists(m_appSettings.ProjectFolder + "\\" + projectname))
            {
                m_appSettings.LastOpenedType = 1;
                m_CurrentWorkingProject = projectname;
                m_ProjectLog.OpenProjectLog(m_appSettings.ProjectFolder + "\\" + projectname);
                //Load the binary file that comes with this project
                retval = LoadBinaryForProject(projectname);
                if (retval)
                {
                    LoadAFRMapsForProject(projectname);
                    if (m_trionicFile != null)
                    {
                        // transaction log <GS-15032010>
                        m_ProjectTransactionLog = new TrionicTransactionLog();
                        if (m_ProjectTransactionLog.OpenTransActionLog(m_appSettings.ProjectFolder, projectname))
                        {
                            m_ProjectTransactionLog.ReadTransactionFile();
                            m_trionicFile.SetTransactionLog(m_ProjectTransactionLog);
                            // if the number of transactions is > 2000 offer the user to purge the transaction log
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
                        btnProjectNote.Enabled = true;
                        btnEditProject.Enabled = true;
                        btnShowProjectLogbook.Enabled = true;
                        btnProduceBinaryFromProject.Enabled = true;
                        btnRecreateFile.Enabled = true;
                        CreateProjectBackupFile();
                        UpdateRollbackForwardControls();
                        m_appSettings.Lastprojectname = m_CurrentWorkingProject;
                        this.Text = "T5Suite Professional 2.0 [Project: " + projectname + "]";
                    }
                }
            }
            return retval;
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

        private void LoadAFRMapsForProject(string projectname)
        {
            string m_currentfile = string.Empty;
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                DataTable projectprops = new DataTable("T5PROJECT");
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
                    m_currentfile = projectprops.Rows[0]["BINFILE"].ToString();
                }
            }
            if (m_currentfile != "" && m_appSettings.AlwaysCreateAFRMaps)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
                //TODO: initialize & reload if data available.
            }
        }

        private bool LoadBinaryForProject(string projectname)
        {
            bool retval = false;
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                DataTable projectprops = new DataTable("T5PROJECT");
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
                    retval = OpenWorkingFile(projectprops.Rows[0]["BINFILE"].ToString());
                }
            }
            return retval;
        }

        private string GetBinaryForProject(string projectname)
        {
            string retval = retval = m_trionicFile.GetFileInfo().Filename;
            if (File.Exists(m_appSettings.ProjectFolder + "\\" + projectname + "\\projectproperties.xml"))
            {
                DataTable projectprops = new DataTable("T5PROJECT");
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




        private void btnOpenProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //let the user select a project from the Project folder. If none are present, let the user know
            if (!Directory.Exists(m_appSettings.ProjectFolder)) Directory.CreateDirectory(m_appSettings.ProjectFolder);
            DataTable ValidProjects = new DataTable();
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
                    DataTable projectprops = new DataTable("T5PROJECT");
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

        #region AFR maps
        //Save maps in project folder
       

        private void UpdateViewer(IMapViewer viewer, int tabwidth, bool sixteenbits)
        {
            try
            {
                this.Invoke(m_DelegateUpdateMapViewer, viewer, tabwidth, sixteenbits);
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void UpdateFeedbackMaps()
        {
            try
            {
                // convert feedback map in memory to byte[] in stead of float[]
                if (m_AFRMaps == null)
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }

                byte[] current_map = m_AFRMaps.GetFeedBackmapInBytes();
                byte[] idlecurrent_map = m_AFRMaps.GetIdleFeedbackAFRMapinBytes();

                int rows = 0;
                int cols = 0;
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text.StartsWith("Symbol: "))
                    {
                        foreach (Control c in pnl.Controls)
                        {
                            if (c is IMapViewer)
                            {
                                IMapViewer vwr = (IMapViewer)c;
                                if (vwr.Map_name == "FeedbackAFR")
                                {
                                    vwr.Map_content = current_map;
                                    vwr.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                    m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                    UpdateViewer(vwr, cols, true);
                                    //vwr.ShowTable(cols, true);
                                }
                                else if (vwr.Map_name == "FeedbackvsTargetAFR")
                                {
                                    vwr.Map_content = m_AFRMaps.GetDifferenceMapinBytes();
                                    vwr.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                    m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                    UpdateViewer(vwr, cols, true);
                                }
                                if (vwr.Map_name == "IdleFeedbackAFR")
                                {
                                    vwr.Map_content = idlecurrent_map;
                                    vwr.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                    UpdateViewer(vwr, 12, true);
                                }
                                else if (vwr.Map_name == "IdleFeedbackvsTargetAFR")
                                {
                                    vwr.Map_content = m_AFRMaps.GetIdleDifferenceMapinBytes();
                                    vwr.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                    UpdateViewer(vwr, 12, true);
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
                                        if (vwr2.Map_name == "FeedbackAFR")
                                        {
                                            vwr2.Map_content = current_map;
                                            vwr2.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                            m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                            UpdateViewer(vwr2, cols, true);
                                            //vwr.ShowTable(cols, true);
                                        }
                                        else if (vwr2.Map_name == "FeedbackvsTargetAFR")
                                        {
                                            vwr2.Map_content = m_AFRMaps.GetDifferenceMapinBytes();
                                            vwr2.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                            m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                            UpdateViewer(vwr2, cols, true);
                                        }
                                        if (vwr2.Map_name == "IdleFeedbackAFR")
                                        {
                                            vwr2.Map_content = idlecurrent_map;
                                            vwr2.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                            UpdateViewer(vwr2, 12, true);
                                        }
                                        else if (vwr2.Map_name == "IdleFeedbackvsTargetAFR")
                                        {
                                            vwr2.Map_content = m_AFRMaps.GetIdleDifferenceMapinBytes();
                                            vwr2.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                            UpdateViewer(vwr2, 12, true);
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
                                        if (vwr3.Map_name == "FeedbackAFR")
                                        {
                                            vwr3.Map_content = current_map;
                                            vwr3.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                            m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                            UpdateViewer(vwr3, cols, true);
                                            //vwr.ShowTable(cols, true);
                                        }
                                        else if (vwr3.Map_name == "FeedbackvsTargetAFR")
                                        {
                                            vwr3.Map_content = m_AFRMaps.GetDifferenceMapinBytes();
                                            vwr3.Afr_counter = m_AFRMaps.GetAFRCountermap();
                                            m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out cols, out rows);
                                            UpdateViewer(vwr3, cols, true);
                                        }
                                        if (vwr3.Map_name == "IdleFeedbackAFR")
                                        {
                                            vwr3.Map_content = idlecurrent_map;
                                            vwr3.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                            UpdateViewer(vwr3, 12, true);
                                        }
                                        else if (vwr3.Map_name == "IdleFeedbackvsTargetAFR")
                                        {
                                            vwr3.Map_content = m_AFRMaps.GetIdleDifferenceMapinBytes();
                                            vwr3.Afr_counter = m_AFRMaps.GetIdleAFRCountermap();
                                            UpdateViewer(vwr3, 12, true);
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
                logger.Debug("Refresh viewer with AFR data error: " + E.Message);
            }
        }

        private void ShowAfrMAP(string mapname, byte[] _data)
        {
            // show seperate mapviewer for AFR target map
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            string symbolname = mapname;
            try
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text == "Symbol: " + symbolname + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]")
                    {
                        dockPanel = pnl;
                        pnlfound = true;
                        dockPanel.Show();
                        // nog data verversen?

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    }

                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    //TryTpShowTouchScreenInput();

                    tabdet.DirectSRAMWriteOnSymbolChange = false;
                    //tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Visible = false;
                    tabdet.Filename = m_trionicFile.GetFileInfo().Filename;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }
                   /* if (barViewInHex.Checked)
                    {
                        tabdet.Viewtype = Trionic5Tools.ViewType.Hexadecimal;
                    }*/
                    /*else
                    {
                        tabdet.Viewtype = m_appSettings.DefaultViewType;//ViewType.Easy;
                    }*/
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = "Target AFR map for use with wideband lambda sensor";
                    if (mapname == "FeedbackAFR") tabdet.Map_descr = "Feedback AFR map from wideband lambda sensor";
                    if (mapname == "FeedbackvsTargetAFR") tabdet.Map_descr = "Feedback AFR minus target AFR map from wideband lambda sensor";
                    tabdet.Map_cat = XDFCategories.Sensor;

                    tabdet.X_axisvalues = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetInjectionMap());
                    tabdet.Y_axisvalues = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetInjectionMap());
                    // z, y and z axis to do

                    //<GS-24032010> dock it anyway, so it will fit the screen

                    System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                    //dockPanel = dockManager1.AddPanel(floatpoint);
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Width = 800; // TEST

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
                    else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                    {
                        int dw = 550;
                        if (tabdet.X_axisvalues.Length > 0)
                        {
                            dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
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
                        dockPanel.Width = dw;
                    }
                    floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                    while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                    dockPanel.FloatLocation = floatpoint;

                    dockPanel.Tag = m_trionicFile.GetFileInfo().Filename;

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    
                    m_trionicFile.GetMapAxisDescriptions(m_trionicFileInformation.GetInjectionMap(), out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;
                    int columns = 8;
                    int rows = 8;
                    m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetInjectionMap(), out columns, out rows);
                    
                    tabdet.Map_address = 0;
                    tabdet.Map_sramaddress = 0;
                    int length = m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap());
                    tabdet.Map_length = length * 2;
                    byte[] mapdata = _data;// m_AFRMaps.LoadTargetAFRMapInBytes(filename);//TODO: ???
                    tabdet.Map_content = mapdata;

                    tabdet.Correction_factor = 0.1;
                    tabdet.Correction_offset = 0;
                    tabdet.IsUpsideDown = true;
                    
                    tabdet.ShowTable(columns, true);
                    
                    tabdet.Dock = DockStyle.Fill;
                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(onTargetAFRMapSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                    tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(onAFRRefresh);
                    tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]";
                    bool isDocked = false;
                    if (!isDocked)
                    {
                        int width = 600;
                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                        {
                            width = 600;
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

                            }
                            else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                }
                                if (width < 400) width = 400;

                            }
                        }
                        dockPanel.Width = width;
                    }
                    if (dockPanel.Height < 700) tabdet.GraphVisible = false; //<GS-24032010>

                    dockPanel.Controls.Add(tabdet);
                    TryToAddOpenLoopTables(tabdet);
                    tabdet.Visible = true;
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();

            }
            UpdateFeedbackMaps();

            System.Windows.Forms.Application.DoEvents();

        }

        private void ShowIdleAfrMAP(string mapname, byte[] _data)
        {
            // show seperate mapviewer for AFR target map
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            string symbolname = mapname;
            try
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text == "Symbol: " + symbolname + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]")
                    {
                        dockPanel = pnl;
                        pnlfound = true;
                        dockPanel.Show();
                        // nog data verversen?

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    }

                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    //TryTpShowTouchScreenInput();

                    tabdet.DirectSRAMWriteOnSymbolChange = false;
                    //tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Visible = false;
                    tabdet.Filename = m_trionicFile.GetFileInfo().Filename;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }
                    /* if (barViewInHex.Checked)
                     {
                         tabdet.Viewtype = Trionic5Tools.ViewType.Hexadecimal;
                     }*/
                    /*else
                    {
                        tabdet.Viewtype = m_appSettings.DefaultViewType;//ViewType.Easy;
                    }*/
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = "Target Idle AFR map for use with wideband lambda sensor";
                    if (mapname == "IdleFeedbackAFR") tabdet.Map_descr = "Feedback Idle AFR map from wideband lambda sensor";
                    if (mapname == "IdleFeedbackvsTargetAFR") tabdet.Map_descr = "Feedback Idle AFR minus target Idle AFR map from wideband lambda sensor";
                    tabdet.Map_cat = XDFCategories.Sensor;

                    tabdet.X_axisvalues = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetIdleFuelMap());
                    tabdet.Y_axisvalues = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetIdleFuelMap());
                    // z, y and z axis to do

                    //<GS-24032010> dock it anyway, so it will fit the screen

                    System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                    //dockPanel = dockManager1.AddPanel(floatpoint);
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Width = 800; // TEST

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
                    else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                    {
                        int dw = 550;
                        if (tabdet.X_axisvalues.Length > 0)
                        {
                            dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
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
                        dockPanel.Width = dw;
                    }
                    floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                    while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                    dockPanel.FloatLocation = floatpoint;

                    dockPanel.Tag = m_trionicFile.GetFileInfo().Filename;

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;

                    m_trionicFile.GetMapAxisDescriptions(m_trionicFileInformation.GetIdleFuelMap(), out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;
                    int columns = 8;
                    int rows = 8;
                    m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetIdleFuelMap(), out columns, out rows);

                    tabdet.Map_address = 0;
                    tabdet.Map_sramaddress = 0;
                    int length = m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap());
                    tabdet.Map_length = length * 2;
                    byte[] mapdata = _data;// m_AFRMaps.LoadTargetAFRMapInBytes(filename);//TODO: ???
                    tabdet.Map_content = mapdata;

                    tabdet.Correction_factor = 0.1;
                    tabdet.Correction_offset = 0;
                    tabdet.IsUpsideDown = true;

                    tabdet.ShowTable(columns, true);

                    tabdet.Dock = DockStyle.Fill;
                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(onTargetAFRMapSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                    tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(onAFRRefresh);
                    tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);
                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]";
                    bool isDocked = false;
                    if (!isDocked)
                    {
                        int width = 600;
                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                        {
                            width = 600;
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

                            }
                            else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                }
                                if (width < 400) width = 400;

                            }
                        }
                        dockPanel.Width = width;
                    }
                    if (dockPanel.Height < 700) tabdet.GraphVisible = false; //<GS-24032010>

                    dockPanel.Controls.Add(tabdet);
                    TryToAddOpenLoopTables(tabdet);
                    tabdet.Visible = true;
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();

            }
            UpdateFeedbackMaps();

            System.Windows.Forms.Application.DoEvents();

        }

        void onAFRRefresh(object sender, MapViewer.ReadFromSRAMEventArgs e)
        {
            //logger.Debug("Refresh: " + e.Mapname);
            // reload target afr
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                if (m_AFRMaps == null )
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }

                if (e.Mapname == "TargetAFR")
                {
                    mv.Map_content = m_AFRMaps.GetTargetAFRMapinBytes();
                }
                else if (e.Mapname == "FeedbackAFR" )
                {
                    mv.Map_content = m_AFRMaps.GetFeedbackAFRMapinBytes();
                }
                else if (e.Mapname == "FeedbackvsTargetAFR")
                {
                    mv.Map_content = m_AFRMaps.GetDifferenceMapinBytes();
                }
                else if (e.Mapname == "IdleTargetAFR")
                {
                    mv.Map_content = m_AFRMaps.GetIdleTargetAFRMapinBytes();
                }
                else if (e.Mapname == "IdleFeedbackAFR")
                {
                    mv.Map_content = m_AFRMaps.GetIdleFeedbackAFRMapinBytes();
                }
                else if (e.Mapname == "IdleFeedbackvsTargetAFR")
                {
                    mv.Map_content = m_AFRMaps.GetIdleDifferenceMapinBytes();
                }
                mv.ShowTable(mv.X_axisvalues.Length, true);
            }
        }
        

        void onTargetAFRMapSave(object sender, MapViewer.SaveSymbolEventArgs e)
        {
            if (sender is IMapViewer)
            {
                IMapViewer tabdet = (IMapViewer)sender;
                // get data from mapviewer
                if (m_AFRMaps == null )
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }

                if (e.SymbolName == "TargetAFR")
                {
                    m_AFRMaps.SetTargetAFRMapInBytes(e.SymbolDate);
                    m_AFRMaps.SaveMaps();
                    tabdet.Map_content = m_AFRMaps.LoadTargetAFRMapInBytes();
                }
                else if (tabdet.Map_name == "FeedbackAFR" || tabdet.Map_name == "FeedbackvsTargetAFR")
                {
                    if (tabdet.ClearData)
                    {
                        tabdet.ClearData = false;
                        // delete stuff
                        string foldername = Path.Combine(Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename), "AFRMaps");

                        if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-AFRFeedbackmap.afr")))
                        {
                            File.Delete(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-AFRFeedbackmap.afr"));
                        }
                        if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-AFRFeedbackCountermap.afr")))
                        {
                            File.Delete(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-AFRFeedbackCountermap.afr"));
                        }
                        m_AFRMaps.InitializeMaps();
                        // reshow

                        if (tabdet.Map_name == "FeedbackAFR")
                        {
                            tabdet.Map_content = m_AFRMaps.GetFeedbackAFRMapinBytes();
                        }
                        else if (tabdet.Map_name == "FeedbackvsTargetAFR")
                        {
                            tabdet.Map_content = m_AFRMaps.GetDifferenceMapinBytes();
                        }
                        tabdet.ShowTable(tabdet.X_axisvalues.Length, true);

                    }
                    else
                    {
                        // only done on clear?
                        m_AFRMaps.SaveMaps();
                    }
                    // reload data
                    UpdateMapViewers(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel, _currentBoostTarget);
                }

                if (e.SymbolName == "IdleTargetAFR")
                {
                    m_AFRMaps.SetIdleTargetAFRMapInBytes(e.SymbolDate);
                    m_AFRMaps.SaveIdleMaps();
                    tabdet.Map_content = m_AFRMaps.LoadIdleTargetAFRMapInBytes();
                }
                else if (tabdet.Map_name == "IdleFeedbackAFR" || tabdet.Map_name == "IdleFeedbackvsTargetAFR")
                {
                    if (tabdet.ClearData)
                    {
                        tabdet.ClearData = false;
                        // delete stuff
                        string foldername = Path.Combine(Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename), "AFRMaps");
                        if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackmap.afr")))
                        {
                            File.Delete(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackmap.afr"));
                        }
                        if (File.Exists(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackCountermap.afr")))
                        {
                            File.Delete(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-idleAFRFeedbackCountermap.afr"));
                        }
                        m_AFRMaps.InitializeMaps();

                        if (tabdet.Map_name == "IdleFeedbackAFR")
                        {
                            tabdet.Map_content = m_AFRMaps.GetIdleFeedbackAFRMapinBytes();
                        }
                        else if (tabdet.Map_name == "IdleFeedbackvsTargetAFR")
                        {
                            tabdet.Map_content = m_AFRMaps.GetIdleDifferenceMapinBytes();
                        }
                        tabdet.ShowTable(tabdet.X_axisvalues.Length, true);
                    }
                    else
                    {
                        // only done on clear?
                        m_AFRMaps.SaveIdleMaps();
                    }
                    // reload data
                    UpdateMapViewers(_currentEngineSpeed, _currentThrottlePosition, _currentBoostLevel, _currentBoostTarget);
                }
            }
        }

        

        private float[] ConvertMapToFloat(byte[] map)
        {
            float[] mapinfloat = new float[map.Length / 2];
            for (int t = 0; t < map.Length; t += 2)
            {
                int value = Convert.ToInt32(map[t]) * 256;
                value += Convert.ToInt32(map[t + 1]);
                mapinfloat[t / 2] = (float)value / 10;
            }
            return mapinfloat;
        }

        #endregion


        private void CloseProject()
        {
            if (_ecuConnection.Opened) StopOnlineMode();// StopECUConnection();
            if (m_CurrentWorkingProject != "")
            {
                if (m_AFRMaps != null)
                {
                    m_AFRMaps.SaveMaps();
                    m_AFRMaps.SaveIdleMaps();
                }
            }
            m_CurrentWorkingProject = string.Empty;
            // unload the current file
            m_trionicFile = null;
            m_trionicFileInformation = null;
            barECUType.Caption = "--";
            barECUSpeed.Caption = "--";
            barECULocked.Caption = "--";
            gridSymbols.DataSource = null;
            barStaticItem2.Caption = "No file";
            barButtonItem4.Enabled = false;
            btnConnectECU.Enabled = false;
            btnSynchronizeMaps.Enabled = false;
            btnWriteLogMarker.Enabled = false;
            btnClearKnockCounters.Enabled = false;
            btnErrorCodes.Enabled = false;
            btnSwitchMode.Enabled = false;
            btnTuneForE85Fuel.Enabled = false;
            btnTuneToLargerInjectors.Enabled = false;
            btnTuneToStage1.Enabled = false;
            btnTuneToStage2.Enabled = false;
            btnTuneToStage3.Enabled = false;
            btnTuneToStageX.Enabled = false;

//            btnTuneToThreeBarSensor.Enabled = false;
            barConvertMapSensor.Enabled = false;

            btnHardcodedRPMLimit.Enabled = false;
            btnBoostAdaptionWizard.Enabled = false;
            btnChangeRegkonMatRange.Enabled = false;
            m_appSettings.Lastfilename = string.Empty;
            btnCloseProject.Enabled = false;
            btnShowProjectLogbook.Enabled = false;
            btnProduceBinaryFromProject.Enabled = false;
            btnProjectNote.Enabled = false;
            btnEditProject.Enabled = false;
            btnRecreateFile.Enabled = false;
            btnRollback.Enabled = false;
            btnRollForward.Enabled = false;
            btnShowTransactionLog.Enabled = false;
            this.Text = "T5Suite Professional 2.0";
        }

        private void btnCloseProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CloseProject();
        }

        private void btnAFRTargetmap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // als er nog geen aanwezig is, een nieuwe aanmaken
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            m_AFRMaps.CheckTargetAFRMap();
            //float[] map = CreateDefaultTargetAFRMap();
            //SaveTargetAFRMap(Path.Combine(Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename), Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-targetafr.afr"), map);
            ShowAfrMAP("TargetAFR", m_AFRMaps.LoadTargetAFRMapInBytes());

        }

        private void btnAFRFeedbackmap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show feedback afr map if available
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            //SaveTargetAFRMap(Path.Combine(Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename), Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-feedbackafrtab.afr"), AFRMapInMemory);
            ShowAfrMAP("FeedbackAFR", m_AFRMaps.GetFeedbackAFRMapinBytes());
        }

        private void btnAFRErrormap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }    
                //SaveTargetAFRMap(Path.Combine(Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename), Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-feedbackvstargetafrtab.afr"), tempmap);*/
            ShowAfrMAP("FeedbackvsTargetAFR", m_AFRMaps.GetDifferenceMapinBytes());
        }

        private void btnBinaryBackup_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // create a backupfile (in the working folder for this binfile)
            // if a project is opened, create it in a Backups folder for this project

            if (m_trionicFile.Exists())
            {
                if (m_CurrentWorkingProject != "")
                {
                    if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups");
                    string filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                    File.Copy(GetBinaryForProject(m_CurrentWorkingProject), filename);
                }
                else
                {
                    // create a backup file in the current working folder
                    string filename = Path.GetDirectoryName(m_trionicFile.GetFileInfo().Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFile.GetFileInfo().Filename) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                    File.Copy(m_trionicFile.GetFileInfo().Filename, filename);
                }
            }
        }

        private void on_fio(int bytes)
        {

            this.fio_bytes += (uint)bytes;
            //logger.Debug("on_fio: " + this.fio_bytes.ToString());
            if (!this.IsDisposed)
            {
                try
                {
                    this.Invoke(m_DelegateUpdateBDMProgress, this.fio_bytes);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
            //this.fio_invoke();
        }

        private void ReportBDMProgress(uint bytes)
        {
            try
            {
                string str;
                int percentage = 0;
                int max_bytes = 0x40000;
                switch (_globalECUType)
                {
                    case ecu_t.Trionic52:
                        max_bytes = 0x20000;
                        break;
                    case ecu_t.Trionic55:
                        max_bytes = 0x40000;
                        break;
                    case ecu_t.Trionic5529:
                        max_bytes = 0x40000;
                        break;
                    case ecu_t.Trionic7:
                        max_bytes = 0x80000;
                        break;
                }
                percentage = ((int)this.fio_bytes * 100) / max_bytes;
                if (Convert.ToInt32(barEditItem1.EditValue) != percentage)
                {
                    //logger.Debug("Updating with : " + this.fio_bytes.ToString());
                    // need to calculate the percentage

                    barEditItem1.EditValue = percentage;
                    System.Windows.Forms.Application.DoEvents();
                    //this.tspg_progress.Value = (int)this.fio_bytes;
                    /*if (this.fio_bytes < 0x400)
                    {
                        str = string.Format("{1} B", this.fio_bytes);
                    }
                    else
                    {
                        str = string.Format("{1:F} kB", (float)(((double)this.fio_bytes) / 1024.0));
                    }
                    barEditItem2.Caption = str;*/
                }

            }
            catch (Exception E)
            {
                logger.Debug("fio_invoke: " + E.Message);
            }
        }

        private void btnUSBBDMReadECU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // read the ECU through USB BDM
            bool _continue = true;
            try
            {
                if (!_globalBDMOpened)
                {
                    if (!BdmAdapter_Open())
                    {
                        frmInfoBox info = new frmInfoBox("Could not connect to the BDM adapter");
                        _continue = false;
                    }
                }
                if (_continue)
                {
                    _globalBDMOpened = true;
                    if (BDMversion == 0)
                    {
                        if (!BdmAdapter_GetVersion(ref BDMversion))
                        {
                            frmInfoBox info = new frmInfoBox("BDM adapter is not compatible");
                            _continue = false;
                        }
                    }
                    if (_continue)
                    {
                        // adapter opened and version is compatible
                        //BdmAdapter_GetVerifyFlash();
                        // read ECU through USB BDM
                        frmECUBDMTypeSelection typesel = new frmECUBDMTypeSelection();
                        if (typesel.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = "Binary files|*.bin";
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {

                                mRecreateAllScriptResources(Path.GetDirectoryName(sfd.FileName));
                                barEditItem1.EditValue = 0;
                                SetStatusText("Dumping ECU");
                                System.Windows.Forms.Application.DoEvents();
                                _globalECUType = typesel.GetECUType();
                                fio_bytes = 0;
                                if (!BdmAdapter_DumpECU(sfd.FileName, typesel.GetECUType()))
                                {
                                    frmInfoBox info = new frmInfoBox("Failed to dump ECU");
                                }
                                DeleteScripts(Path.GetDirectoryName(sfd.FileName));
                            }
                        }
                    }
                }
                SetStatusText("Idle");
                barEditItem1.EditValue = 0;
                System.Windows.Forms.Application.DoEvents();

            }
            catch (Exception BDMException)
            {
                logger.Debug("Failed to dump ECU: " + BDMException.Message);
                frmInfoBox info = new frmInfoBox("Failed to download firmware from ECU: " + BDMException.Message);
            }
        }

        private uint fio_bytes;
        private FIOCallback fio_callback = null;

        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_Break();
        [DllImport("usb_bdm.dll")]
        public static extern void BdmAdapter_Close();
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_DumpECU(string file_name, ecu_t ecu);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_EraseECU(ecu_t ecu);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_FlashECU(string file_name, ecu_t ecu);
        [DllImport("usb_bdm.dll")]
        public static extern string BdmAdapter_GetLastErrorStr();
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_GetVerifyFlash();
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_GetVersion(ref ushort version);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_Open();
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_Reset();
        [DllImport("usb_bdm.dll")]
        public static extern void BdmAdapter_SetFIOCallback(FIOCallback func);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_SetVerifyFlash(bool verify);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_Stop();
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_UpdateFirmware(string file_name);
        [DllImport("usb_bdm.dll")]
        public static extern bool BdmAdapter_ReadSRAM(string file_name, ecu_t ecu);
 

 


        //======================================================
        //Recreate all executable resources
        //======================================================
        private void mRecreateAllScriptResources(string path)
        {
            // Get Current Assembly refrence
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all imbedded resources
            string[] arrResources = currentAssembly.GetManifestResourceNames();

            foreach (string resourceName in arrResources)
            {
                if (resourceName.EndsWith(".do") || resourceName.EndsWith(".msg") || resourceName.EndsWith(".d32"))
                { //or other extension desired
                    //Name of the file saved on disk
                    string saveAsName = resourceName;
                    saveAsName = saveAsName.Replace("T5Suite2._0.scripts.", "");
                    if (!Directory.Exists(Path.Combine(path, "scripts")))
                    {
                        Directory.CreateDirectory(Path.Combine(path, "scripts"));
                    }
                    FileInfo fileInfoOutputFile = new FileInfo(path + "\\scripts\\" + saveAsName);
                    //CHECK IF FILE EXISTS AND DO SOMETHING DEPENDING ON YOUR NEEDS
                    logger.Debug("Extracting: " + fileInfoOutputFile.FullName);
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

        private void DeleteScripts(string path)
        {
            // remove the entire folder
            path += "\\scripts";
            logger.Debug("Deleting scripts folder: " + path);
            Directory.Delete(path, true);
        }

        private void btnUSBBDMWriteECU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // program ECU through USB BDM
            bool _continue = true;
            try
            {
                if (!_globalBDMOpened)
                {
                    if (!BdmAdapter_Open())
                    {
                        frmInfoBox info = new frmInfoBox("Could not connect to the BDM adapter");
                        _continue = false;
                    }
                }
                if (_continue)
                {
                    _globalBDMOpened = true;
                    if (BDMversion == 0)
                    {
                        if (!BdmAdapter_GetVersion(ref BDMversion))
                        {
                            frmInfoBox info1 = new frmInfoBox("BDM adapter is not compatible");
                            _continue = false;
                        }
                    }
                    if (_continue)
                    {
                        // adapter opened and version is compatible
                        BdmAdapter_GetVerifyFlash();
                        // program ECU through USB BDM
                        frmECUBDMTypeSelection typesel = new frmECUBDMTypeSelection();
                        if (typesel.ShowDialog() == DialogResult.OK)
                        {
                            OpenFileDialog ofd = new OpenFileDialog();
                            ofd.Filter = "Binary files|*.bin";
                            ofd.Multiselect = false;
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                mRecreateAllScriptResources(Path.GetDirectoryName(ofd.FileName));
                                fio_bytes = 0;
                                SetStatusText("Erasing ECU");
                                _globalECUType = typesel.GetECUType();
                                BdmAdapter_EraseECU(typesel.GetECUType());
                                SetStatusText("Flashing ECU");
                                Thread.Sleep(100);
                                BdmAdapter_FlashECU(ofd.FileName, typesel.GetECUType());
                                SetStatusText("Resetting ECU");
                                Thread.Sleep(100);
                                DeleteScripts(Path.GetDirectoryName(ofd.FileName));
                            }
                        }
                    }
                }
                SetStatusText("Idle");
                barEditItem1.EditValue = 0;
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception BDMException)
            {
                logger.Debug("Failed to program ECU: " + BDMException.Message);
                frmInfoBox info= new frmInfoBox("Failed to program ECU: " + BDMException.Message);
            }
        }

        private void ctrlRealtime1_onSwitchClosedLoopOnOff(object sender, ctrlRealtime.ClosedLoopOnOffEventArgs e)
        {
            // toggle closed loop on /off based on e.SwitchOn
            //Fastest way to detect cell stability and such is in ECUConnection but we don't want that logic in there.
            //Design decision is to feed the AFRMaps object with the data and have that figure out what to do.
            _ecuConnection.StopECUMonitoring();
            Thread.Sleep(10);
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }


            byte[] pgm_mod = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetProgramModeSymbol(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetProgramModeSymbol()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol()));
            Thread.Sleep(50);
            // Pgm_mod! byte 0 bit 4  0x10 Lambda control on/off 
            if (pgm_mod.Length > 3)
            {
                if (e.SwitchOn)
                {

                    // re-enable the enrichment factors
                    if (m_ProgramModeSettings.Lambdacontrol) pgm_mod[0] |= 0x10; // switch closed loop back on
                    //if (m_ProgramModeSettings.PurgeControl) pgm_mod[2] |= 0x20; // switch purge control back on
                    //if(m_ProgramModeSettings.AcclerationEnrichment) pgm_mod[1] |= 0x10; // acceleration enrichment back on
                    //if(m_ProgramModeSettings.DecelerationEnleanment) pgm_mod[1] |= 0x20; // deceleration enleanment back on
                    //if(m_ProgramModeSettings.WOTEnrichment) pgm_mod[0] |= 0x02; // WOT enrichment back on
                    //if(m_ProgramModeSettings.Fuelcut) pgm_mod[1] |= 0x04; // Fuelcut back on
                    //if (m_ProgramModeSettings.UseSeperateInjectionMapForIdle) pgm_mod[2] |= 0x02; // Idle map back on
                    //if(m_ProgramModeSettings.LoadControl) pgm_mod[3] |= 0x04; // load control back on
                    //if (pgm_mod.Length >= 5)
                    //{
                    //    if (m_ProgramModeSettings.NoFuelcutR12) pgm_mod[4] |= 0x04; // no fuelcut in R12 back on
                    //    if (m_ProgramModeSettings.ConstIdleIgnitionAngleGearOneAndTwo) pgm_mod[4] |= 0x02; // const ign angle back on
                    //}
                    // write Pgm_mod into ECU immediately!
                    if(m_appSettings.DisableClosedLoopOnStartAutotune) _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetProgramModeSymbol()), 1, pgm_mod); // <GS-12042010> set to length = 1 to prevent other changes
                    Thread.Sleep(100);

                    if (m_appSettings.AutoUpdateFuelMap)
                    {
                        //TODO: ask the user whether he wants to merge the altered fuelmap into ECU memory!
                        // if he replies NO: revert to the previous fuel map (we still need to preserve a copy!)
                        if (MessageBox.Show("Keep adjusted fuel map?", "Question", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            // save the original map back to the ECU
                            if (!props.IsTrionic55)
                            {
                                _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"), (int)m_trionicFileInformation.GetSymbolLength("Adapt_korr"), m_AFRMaps.GetOriginalFuelmap());
                            }
                            else
                            {
                                _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (int)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()), m_AFRMaps.GetOriginalFuelmap());
                            }
                            // <GS-14032011> this should only be done when idlefuel map autotune is active as well
                            //
                            if (m_appSettings.AllowIdleAutoTune)
                            {
                                _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()), (int)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()), m_AFRMaps.GetIdleOriginalFuelmap());
                            }
                            // and write to the bin as well

                        }
                        else
                        {
                            // save the altered map into the binary
                            //TODO: <GS-03062010> this alters the syncdatetime in the file!!!
                            DateTime ecudt = _ecuConnection.GetMemorySyncDate();
                            DateTime filedt = m_trionicFile.GetMemorySyncDate();
                            bool _updateSync = false;
                            if (ecudt == filedt) _updateSync = true;
                            m_trionicFile.WriteData(_ecuConnection.ReadSymbolData(m_trionicFileInformation.GetInjectionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap())), (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap()));
                            if (m_appSettings.AllowIdleAutoTune)
                            {
                                m_trionicFile.WriteData(_ecuConnection.ReadSymbolData(m_trionicFileInformation.GetIdleFuelMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap())), (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIdleFuelMap()));
                            }
                            if (_updateSync)
                            {
                                DateTime dtnow = DateTime.Now;
                                _ecuConnection.SetMemorySyncDate(dtnow);
                                m_trionicFile.SetMemorySyncDate(dtnow);
                                // now we should be in sync again
                            }
                        }


                        // init the afrmaps values
                        m_AFRMaps.InitAutoTuneVars(e.SwitchOn);
                    }
                    else
                    {
                        // <GS-02032010> Create a new form for this with multiselect grid!
                        //TODO: in that case, we've maintained the changes in the m_AFRMaps.FuelMapInformation struct
                        // we should now show the proposed changed (in percentages) to the user and let him/her
                        // decide which cells should be updated and which ones should be discarded
                        double[] diffinperc = m_AFRMaps.GetPercentualDifferences();

                        DataTable dt = new DataTable();
                        for (int i = 0; i < 16; i++)
                        {
                            dt.Columns.Add(i.ToString(), Type.GetType("System.Double"));
                        }

                        for (int i = 15; i >= 0; i--)
                        {
                            object[] arr = new object[16];

                            for (int j = 0; j < 16; j++)
                            {
                                arr.SetValue(diffinperc[(i * 16) + j], j);
                            }
                            dt.Rows.Add(arr);
                        }
                        frmFuelMapAccept acceptMap = new frmFuelMapAccept();
                        acceptMap.onUpdateFuelMap += new frmFuelMapAccept.UpdateFuelMap(acceptMap_onUpdateFuelMap);
                        acceptMap.onSyncDates += new frmFuelMapAccept.SyncDates(acceptMap_onSyncDates);
                        acceptMap.SetDataTable(dt);
                        acceptMap.ShowDialog();
                        DialogResult = DialogResult.None;
                        //TODO: <GS-28102010> Hoe hier ook te vragen voor Idle fuel map???
                        // voor nu maar even gewoon domweg schrijven
                        if (m_appSettings.AllowIdleAutoTune)
                        {
                            _ecuConnection.WriteSymbolData(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()), m_AFRMaps.GetIdleCurrentlyMutatedFuelMap());
                        }
                        Application.DoEvents();

                    }
                    ctrlRealtime1.SetAutoTuneButtonText("Autotune fuel");
                    SetStatusText("Idle");
                }
                else
                {

                    //TODO: disable the enrichment factors
                    // init the afrmaps values
                    SetStatusText("Starting autotune...");
                    System.Windows.Forms.Application.DoEvents();
                    if ((pgm_mod[0] & 0x10) > 0) m_ProgramModeSettings.Lambdacontrol = true;
                    else m_ProgramModeSettings.Lambdacontrol = false;
                    //if ((pgm_mod[2] & 0x20) > 0) m_ProgramModeSettings.PurgeControl = true;
                    //else m_ProgramModeSettings.PurgeControl = false;
                    // Purge control disable
                    //if ((pgm_mod[1] & 0x10) > 0) m_ProgramModeSettings.AcclerationEnrichment = true;
                    //else m_ProgramModeSettings.AcclerationEnrichment = false;
                    //if ((pgm_mod[1] & 0x20) > 0) m_ProgramModeSettings.DecelerationEnleanment = true;
                    //else m_ProgramModeSettings.DecelerationEnleanment = false;
                    //if ((pgm_mod[0] & 0x02) > 0) m_ProgramModeSettings.WOTEnrichment = true;
                    //else m_ProgramModeSettings.WOTEnrichment = false;
                    //if ((pgm_mod[1] & 0x04) > 0) m_ProgramModeSettings.Fuelcut = true;
                    //else m_ProgramModeSettings.Fuelcut = false;
                    //if ((pgm_mod[2] & 0x02) > 0) m_ProgramModeSettings.UseSeperateInjectionMapForIdle = true;
                    //else m_ProgramModeSettings.UseSeperateInjectionMapForIdle = false;
                    //if ((pgm_mod[3] & 0x04) > 0) m_ProgramModeSettings.LoadControl = true;
                    //else m_ProgramModeSettings.LoadControl = false;
                    //if (pgm_mod.Length >= 5)
                    //{
                    //    if ((pgm_mod[4] & 0x04) > 0) m_ProgramModeSettings.NoFuelcutR12 = true;
                    //    else m_ProgramModeSettings.NoFuelcutR12 = false;
                    //    if ((pgm_mod[4] & 0x02) > 0) m_ProgramModeSettings.ConstIdleIgnitionAngleGearOneAndTwo = true;
                    //    else m_ProgramModeSettings.ConstIdleIgnitionAngleGearOneAndTwo = false;
                    //}
                    // we have to restore this after coming out of autotune!
                    // now set to the needed settings for autotuning
                    pgm_mod[0] &= 0xEF; // switch closed loop off
                    //pgm_mod[2] &= 0xDF; // switch purge control off
                    //pgm_mod[1] &= 0xEF; // acceleration enrichment OFF
                    //pgm_mod[1] &= 0xDF; // deceleration enleanment OFF
                    //pgm_mod[0] &= 0xFD; // WOT enrichment OFF ??? Don't: because it will be on after autotune
                    //pgm_mod[2] &= 0xFD; // Idle map usage OFF (so always in main fuel map)
                    //pgm_mod[1] &= 0xFB; // turn fuelcut function in engine brake OFF
                    //pgm_mod[3] &= 0xFB; // turn load control OFF ??? Don't: because it will be on after autotune
                    //if (pgm_mod.Length >= 5)
                    //{
                    //    pgm_mod[4] |= 0x04; // turn off fuelcut in reverse, 1st and second gear (INVERTED)
                    //    pgm_mod[4] &= 0xFD; // turn off constant ignition angle at idle in 1st and second gear
                    //}

                    //Write Pgm_mod into ECU immediately
                    if (m_appSettings.DisableClosedLoopOnStartAutotune) _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetProgramModeSymbol()), 1/*m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol())*/, pgm_mod); // <GS-12042010> set to 1 to prevent other changes
                    Thread.Sleep(100);

                    m_AFRMaps.InitAutoTuneVars(e.SwitchOn); // this also clears the afr feedback map
                    // and automatically show the autotune window (feedbackafr/fuel adjustment map)
                    // read ECU data for fuel correction!
                    //<GS-31032010> should be different for T5.2 
                    //adaption data should NOT be merged and Adapt_korr should be used as the fuel map
                    byte[] fuelmap;
                    if (!props.IsTrionic55)
                    {
                        fuelmap = _ecuConnection.ReadSymbolData("Adapt_korr", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_korr"));
                        Thread.Sleep(50);
                    }
                    else
                    {

                        byte[] fuelcorrectiondata = _ecuConnection.ReadSymbolData("Adapt_korr", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_korr"));
                        Thread.Sleep(50);
                        // AND read the long term fuel trim
                        //byte[] longtermtrim = _ecuConnection.ReadSymbolData("Adapt_injfaktor!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_injfaktor!"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_injfaktor!"));
                        // and merge it into the main fuel map
                        // get the main fuel map
                        fuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetInjectionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                        // if all adaption data is 128, don't update to save time
                        Thread.Sleep(50);
                        bool _doUpdateFuelMap = false;
                        for (int t = 0; t < fuelcorrectiondata.Length; t++)
                        {
                            if (Convert.ToInt32(fuelcorrectiondata[t]) != 0x80)
                            {
                                _doUpdateFuelMap = true;
                            }
                        }
                        //int itrim = Convert.ToInt32(longtermtrim[0]);
                        if (_doUpdateFuelMap)
                        {
                            SetStatusText("Updating fuelmaps...");
                            for (int t = 0; t < fuelcorrectiondata.Length; t++)
                            {
                                int corrval = Convert.ToInt32(fuelcorrectiondata[t]);
                                // ranges from 77 - 160?
                                int fuelmapvalue = Convert.ToInt32(fuelmap[t]);
                                fuelmapvalue *= corrval;
                                fuelmapvalue /= 128;
                                //fuelmapvalue *= itrim;
                                //fuelmapvalue /= 128;
                                // check boundaries
                                if (fuelmapvalue < 1) fuelmapvalue = 1;
                                if (fuelmapvalue > 254) fuelmapvalue = 254;
                                fuelmap[t] = Convert.ToByte(fuelmapvalue);
                            }

                            // save fuel map
                            _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (int)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()), fuelmap);
                            Thread.Sleep(50);
                            for (int t = 0; t < fuelcorrectiondata.Length; t++)
                            {
                                fuelcorrectiondata[t] = 128; // reinit
                            }
                            _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"), (int)m_trionicFileInformation.GetSymbolLength("Adapt_korr"), fuelcorrectiondata);
                            Thread.Sleep(50);
                        }
                    }
                    if (m_appSettings.ResetFuelTrims)
                    {
                        // and write 128 to trim
                        byte[] longtermtrim = new byte[1];
                        longtermtrim[0] = 128;
                        _ecuConnection.WriteSymbolData((int)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_injfaktor!"), (int)m_trionicFileInformation.GetSymbolLength("Adapt_injfaktor!"), longtermtrim);
                        if (props.IsTrionic55)
                        {
                            // clear idle adaption (trim) as well
                            if(m_appSettings.AllowIdleAutoTune)
                            {
                                _ecuConnection.WriteSymbolData((int)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_inj_imat!"), (int)m_trionicFileInformation.GetSymbolLength("Adapt_inj_imat!"), longtermtrim);
                            }
                        }
                    }
                    // all done, adaptions have been made
                    m_AFRMaps.SetCurrentFuelMap(fuelmap);
                    m_AFRMaps.SetOriginalFuelMap(fuelmap);
                    byte[] idlefuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetIdleFuelMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()));
                    m_AFRMaps.SetIdleCurrentFuelMap(idlefuelmap);
                    m_AFRMaps.SetIdleOriginalFuelMap(idlefuelmap); //<GS-14032011> bugfix for erratic idle behaviour after autotune, this was filled with fuelmap in stead of idlefuelmap
                    m_AFRMaps.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                    ctrlRealtime1.SetAutoTuneButtonText("Tuning...");
                    SetStatusText("Autotune fuel running...");
                }
                //TODO: If the afrmaps object does not contain a working fuelmap yet, create it
                if (!m_AFRMaps.HasValidFuelmap) // do always because map has been altered
                {
                    // insert the fuel map from SRAM now
                    byte[] fuelmap;
                    if (!props.IsTrionic55)
                    {
                        fuelmap = _ecuConnection.ReadSymbolData("Adapt_korr", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"), (uint)m_trionicFileInformation.GetSymbolLength("Adapt_korr"));
                    }
                    else
                    {
                        fuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetInjectionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                    }
                    m_AFRMaps.SetCurrentFuelMap(fuelmap);
                }
                if (!m_AFRMaps.HasValidIdleFuelmap) // do always because map has been altered
                {
                    // insert the fuel map from SRAM now
                    byte[] idlefuelmap;
                    idlefuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetIdleFuelMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIdleFuelMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()));
                    m_AFRMaps.SetIdleCurrentFuelMap(idlefuelmap);
                }

                //the object should consider all constraints given in the autotune settings screen
                //so it should remember the ORIGINAL fuel map it started with to determine max-correction etc
                //also it should maintain a counter map for this to be able to monitor how many changes have been made
                //to the map
                //The original map should be 'updateable' (=able to delete and re-create from current sram) so that 
                //the user will be able to force more correction onto the map
                m_AFRMaps.WideBandAFRSymbol = m_appSettings.WidebandLambdaSymbol;
                m_AFRMaps.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
                m_AFRMaps.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
                m_AFRMaps.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                m_AFRMaps.CellStableTime_ms = m_appSettings.CellStableTime_ms;
                m_AFRMaps.CorrectionPercentage = m_appSettings.CorrectionPercentage;
                m_AFRMaps.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
                m_AFRMaps.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
                m_AFRMaps.EnrichmentFilter = m_appSettings.EnrichmentFilter;
                m_AFRMaps.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
                m_AFRMaps.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
                m_AFRMaps.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
                m_AFRMaps.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
                m_AFRMaps.IsAutoMappingActive = !e.SwitchOn; // closed loop OFF means automapping ON
                //ctrlRealtime1.SetAutoTuneButtonText("Tuning...");
            }
            else
            {
                // could not read pgm_mod...wtf?
            }
            _ecuConnection.StartECUMonitoring();

            
        }

        void acceptMap_onSyncDates(object sender, EventArgs e)
        {
            logger.Debug("Timestamp sync after fuelmap accept");
            if (_ecuConnection.Opened)
            {
                DateTime ecudt = _ecuConnection.GetMemorySyncDate();
                DateTime filedt = m_trionicFile.GetMemorySyncDate();
                bool _updateSync = false;
                if (ecudt != filedt)
                {
                    _updateSync = true;
                }
                if (_updateSync)
                {
                    DateTime dtnow = DateTime.Now;
                    m_trionicFile.SetMemorySyncDate(dtnow);
                    _ecuConnection.SetMemorySyncDate(dtnow);
                    // now we are in sync again
                }
            }
        }

        void acceptMap_onUpdateFuelMap(object sender, frmFuelMapAccept.UpdateFuelMapEventArgs e)
        {
            // write to ECU if possible
            //logger.Debug("Update fuel after accept");
            if (_ecuConnection.Opened)
            {
                if (e.Value == 0) return; // test for value 0... nothing todo

                //logger.Debug("x: " + e.X.ToString() + " y: " + e.Y.ToString() + " val: " + e.Value.ToString() + " syn: " + e.doSync.ToString());


                if (m_AFRMaps == null)
                {
                    m_AFRMaps = new AFRMaps();
                    m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                    m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                    m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                    m_AFRMaps.TrionicFile = m_trionicFile;
                    m_AFRMaps.InitializeMaps();
                }

                int y = 15 - e.Y;
                // first get the original map
                //_ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()) + e.Mapindex, 1, write);
                //byte[] fuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetInjectionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                byte[] fuelmap = m_AFRMaps.FuelMapInformation.GetOriginalFuelMap();
                double originalbyte = Convert.ToDouble(fuelmap[(y * 16) + e.X]);
                originalbyte *= (100 + e.Value) / 100;
                byte newFuelMapByte = Convert.ToByte(originalbyte);
                byte[] data2Write = new byte[1];
                data2Write[0] = newFuelMapByte;

                int fuelMapAddress = m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap());
                if (!props.IsTrionic55)
                {
                    fuelMapAddress = m_trionicFileInformation.GetSymbolAddressSRAM("Adapt_korr"); // write to adapt_korr in stead of insp_mat
                }
                fuelMapAddress += (y * 16) + e.X;
                //logger.Debug("Writing data: " + data2Write[0].ToString("D3") + " to address: " + fuelMapAddress.ToString("X6") + " ori was: " + fuelmap[(y * 16) + e.X].ToString("D3"));
                //_ecuConnection.WriteData(data2Write, (uint)fuelMapAddress);
                _ecuConnection.WriteSymbolDataForced(fuelMapAddress, 1, data2Write);
                // and write to the current binary file as well
                //<GS-22032010> create a backup file first
                int fuelMapAddressFlash = m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap());
                fuelMapAddressFlash += (y * 16) + e.X;
                // only on the last update.
                m_trionicFile.WriteData(data2Write, (uint)fuelMapAddressFlash);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // check if symbol selected and which one
            Point p = gridSymbols.PointToClient(Cursor.Position);
            GridHitInfo hitinfo = gridViewSymbols.CalcHitInfo(p);
            int[] selectedrows = gridViewSymbols.GetSelectedRows();
            if (hitinfo.InRow)
            {
                if (selectedrows.Length > 0)
                {
                    int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                    if (grouplevel >= gridViewSymbols.GroupCount)
                    {
                        //logger.Debug("In row");
                        if (gridViewSymbols.GetFocusedRow() is Trionic5Tools.SymbolHelper)
                        {
                            Trionic5Tools.SymbolHelper sh = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetFocusedRow();
                            //logger.Debug("Symbol:" + sh.Varname);
                            if (m_trionicFileInformation.SRAMfilename != "")
                            {
                                // show menu option
                                viewFromSRAMFileToolStripMenuItem.Enabled = true;
                            }
                            else
                            {
                                viewFromSRAMFileToolStripMenuItem.Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void viewFromSRAMFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point p = gridSymbols.PointToClient(Cursor.Position);
            GridHitInfo hitinfo = gridViewSymbols.CalcHitInfo(p);
            int[] selectedrows = gridViewSymbols.GetSelectedRows();
            if (hitinfo.InRow)
            {
                int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridViewSymbols.GroupCount)
                {
                    //logger.Debug("In row");
                    if (gridViewSymbols.GetFocusedRow() is Trionic5Tools.SymbolHelper)
                    {
                        Trionic5Tools.SymbolHelper sh = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetFocusedRow();
                        //StartTableViewer(sh.Varname, true);
                        StartTableViewerSRAMFile(sh.Varname, m_trionicFileInformation.SRAMfilename);
                    }
                }
            }
        }

        private void btnReadSRAMBDM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // read the ECU through USB BDM
            bool _continue = true;
            try
            {
                if (!_globalBDMOpened)
                {
                    if (!BdmAdapter_Open())
                    {
                        frmInfoBox info = new frmInfoBox("Could not connect to the BDM adapter");
                        _continue = false;
                    }
                }
                if (_continue)
                {
                    _globalBDMOpened = true;
                    if (BDMversion == 0)
                    {
                        if (!BdmAdapter_GetVersion(ref BDMversion))
                        {
                            frmInfoBox info = new frmInfoBox("BDM adapter is not compatible");
                            _continue = false;
                        }
                    }
                    if (_continue)
                    {
                        // adapter opened and version is compatible
                        //BdmAdapter_GetVerifyFlash();
                        // read ECU through USB BDM
                        frmECUBDMTypeSelection typesel = new frmECUBDMTypeSelection();
                        if (typesel.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileDialog sfd = new SaveFileDialog();
                            sfd.Filter = "SRAM snapshots files|*.ram";
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                mRecreateAllScriptResources(Path.GetDirectoryName(sfd.FileName));
                                barEditItem1.EditValue = 0;
                                SetStatusText("Dumping ECU");
                                System.Windows.Forms.Application.DoEvents();
                                _globalECUType = typesel.GetECUType();
                                fio_bytes = 0;
                                if (!BdmAdapter_ReadSRAM(sfd.FileName, typesel.GetECUType()))
                                {
                                    frmInfoBox info = new frmInfoBox("Failed to dump ECU");
                                }
                                DeleteScripts(Path.GetDirectoryName(sfd.FileName));
                            }
                        }
                    }
                }
                SetStatusText("Idle");
                barEditItem1.EditValue = 0;
                System.Windows.Forms.Application.DoEvents();

            }
            catch (Exception BDMException)
            {
                logger.Debug("Failed to dump ECU: " + BDMException.Message);
                frmInfoBox info = new frmInfoBox("Failed to download firmware from ECU: " + BDMException.Message);
            }
        }

        private void btnTuneToStageX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // tune with user entered parameters, based in injector type, turbo type, fuel type (to come) etc
            // create a backup!
            // start the parameter screen (user can enter turbo, injector, mapsensor, peak torque and peak power)
            // start the freetune wizard

            frmFreeTuneSettings tunset = new frmFreeTuneSettings();
            props = m_trionicFile.GetTrionicProperties();
            tunset.SetMapSensorType(props.MapSensorType);
            tunset.SetInjectorType(props.InjectorType);
            tunset.SetTurboType(props.TurboType);
            ECUFileType fileType = m_trionicFile.DetermineFileType();
            int frek230 = m_trionicFile.GetSymbolAsInt("Frek_230!");
            int frek250 = m_trionicFile.GetSymbolAsInt("Frek_250!");

            int knockTime = m_trionicFile.GetSymbolAsInt("Knock_matrix_time!");
            tunset.SetKnockTime(knockTime);
            int rpmLimit = m_trionicFile.GetSymbolAsInt("Rpm_max!");
            tunset.SetRPMLimiter(rpmLimit);

            if (fileType == ECUFileType.Trionic52File)
            {
                if (frek230 == 728 || frek250 == 935)
                {
                    //dtReport.Rows.Add("APC valve type: Trionic 5");
                    tunset.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunset.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            else if (fileType == ECUFileType.Trionic55File)
            {
                if (frek230 == 90 || frek250 == 70)
                {
                    tunset.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunset.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            if (tunset.ShowDialog() == DialogResult.OK)
            {
                Trionic5Tuner _tuner = new Trionic5Tuner();
                _tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;

                TuningResult res = _tuner.FreeTuneBinary(m_trionicFile, tunset.GetPeakTorque(), tunset.GetPeakBoost(), tunset.IsTorqueBased, tunset.GetMapSensorType(), tunset.GetTurboType(), tunset.GetInjectorType(), tunset.GetBCVType(), tunset.GetRPMLimiter(), tunset.GetKnockTime());
                string text = "Tuning process completed!";
                if (res == TuningResult.TuningFailedAlreadyTuned)
                {
                    text = "Tuning process aborted, file is already tuned!";
                }
                else if (res == TuningResult.TuningFailedThreebarSensor)
                {
                    text = "Tuning process aborted, file was converted to another mapsensor type before!";
                }

                props = m_trionicFile.GetTrionicProperties();
                if (_tuner.Resume == null)
                {

                }
                _tuner.Resume.AddToResumeTable(text);
                TuningReport tuningrep = new TuningReport();
                tuningrep.ReportTitle = "Tuning report.. stage " + props.TuningStage.ToString();
                tuningrep.SetDataSource(_tuner.Resume.ResumeTuning);
                tuningrep.CreateReport();
                tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);
               //frmInfoBox info = new frmInfoBox(text);
            }
            

        }

        private void OpenAndDisplayLogFile(string filename)
        {
            // create a new dock with a graph view in it
            DevExpress.XtraBars.Docking.DockPanel dp = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Left);

            dp.Size = new Size(dockManager1.Form.ClientSize.Width - dockSymbols.Width, dockSymbols.Height);
            dp.Hide();
            dp.Text = "CANBus logfile: " + Path.GetFileName(filename);
            RealtimeGraphControl lfv = new RealtimeGraphControl();
            LogFilters lfhelper = new LogFilters();
            lfv.SetFilters(lfhelper.GetFiltersFromRegistry());
            dp.Controls.Add(lfv);
            lfv.ImportT5Logfile(filename);
            lfv.Dock = DockStyle.Fill;
            dp.Show();
        }

        private void btnViewLogFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 5 logfiles|*.t5l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenAndDisplayLogFile(ofd.FileName);
            }
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

        private void btnViewMatrixFromLog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // let the user select x axis, y axis and z axis symbols from the logfile
            //
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 5 logfiles|*.t5l";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] alllines = File.ReadAllLines(ofd.FileName);
                avgTable = null;
                System.Windows.Forms.Application.DoEvents();
                DateTime startDate = DateTime.MaxValue;
                DateTime endDate = DateTime.MinValue;
                Trionic5Tools.SymbolCollection sc = new Trionic5Tools.SymbolCollection();
                try
                {
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    
                    {
                        //string line = string.Empty;
                        char[] sep = new char[1];
                        char[] sep2 = new char[1];
                        //int linecount = 0;
                        sep.SetValue('|', 0);
                        sep2.SetValue('=', 0);
                        //while ((line = sr.ReadLine()) != null)
                        foreach(string line in alllines)
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
                                            foreach (Trionic5Tools.SymbolHelper sh in sc)
                                            {
                                                if (sh.Varname == varname)
                                                {
                                                    sfound = true;
                                                }
                                            }
                                            Trionic5Tools.SymbolHelper nsh = new Trionic5Tools.SymbolHelper();
                                            nsh.Varname = varname;
                                            if (!sfound) sc.Add(nsh);
                                        }
                                    }
                                }
                                catch (Exception pE)
                                {
                                    logger.Debug(pE.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
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
                        //string line = string.Empty;
                        char[] sep = new char[1];
                        char[] sep2 = new char[1];
                        //int linecount = 0;
                        sep.SetValue('|', 0);
                        sep2.SetValue('=', 0);
                        //while ((line = sr.ReadLine()) != null)
                        foreach(string line in alllines)
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
                                    logger.Debug(pE.Message);
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
                    DataTable dtresult = new DataTable();
                    // xmin = -0.8
                    // xmin = 2.01
                    double[] x_values = new double[16];
                    double[] y_values = new double[16];

                    // fill x and y axis
                    for (int i = 0; i < 16; i++)
                    {

                        double xvalue = xmin;
                        if(i>0) xvalue = xmin + i * ((xmax - xmin) / (15));
                        //logger.Debug("Adding: " + xvalue.ToString());
                        dtresult.Columns.Add(xvalue.ToString(), Type.GetType("System.Double"));
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
                        dtresult.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
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
                        foreach(string line in alllines)
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
                                    logger.Debug(pE.Message);
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
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy) result.UseNewMapViewer = true;
                    result.SetTable(dtresult);
                    string typedescr = " (Mean values)";
                    if (type == 1) typedescr = " (Minimum values)";
                    else if (type == 1) typedescr = " (Maximum values)";
                    result.Text = "Matrix [" + x + " : " + y + " : " + z + "]" + typedescr;
                    //result.ShowDialog();
                    byte[] m_map_content = new byte[16 * 16 * 2];
                    int idx = 0;
                    for (int i = 15; i >= 0; i--)
                    {
                        foreach (object o in dtresult.Rows[i].ItemArray)
                        {
                            double value = Convert.ToDouble(o);
                            // now convert to int * 100
                            Int32 ivalue = 0;
                            try
                            {
                                ivalue = Convert.ToInt32(value * 100);
                            }
                            catch (Exception E)
                            {
                                logger.Debug("Failed to convert to integer value: " + value.ToString());
                            }
                            byte b1 = 0;
                            byte b2 = 0;
                            try
                            {
                                b1 = Convert.ToByte(ivalue / 256);
                                b2 = Convert.ToByte(ivalue - (int)b1 * 256);
                            }
                            catch (Exception E)
                            {
                                logger.Debug("Failed to convert to byte value + " + ivalue.ToString());
                            }

                            m_map_content[idx++] = b1;
                            m_map_content[idx++] = b2;
                        }
                    }
                    //<GS-28102010> for test, start a mapviewer as well
                    result.ShowDialog();
                    //StartResultViewerForMatrix(x_values, y_values, x, y, z, dtresult, m_map_content);
                }
            }
        }

        private void StartResultViewerForMatrix(double[] xaxis, double[]yaxis, string xname, string yname, string zname, DataTable dtdata, byte[] data)
        {
            dockManager1.BeginUpdate();
            DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
            dp.Tag = m_trionicFileInformation.Filename;
            dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
            //IMapViewer mv = new MapViewerEx();
            IMapViewer mv;
            if (m_appSettings.MapViewerType == MapviewerType.Fancy)
            {
                mv = new MapViewerEx();
            }
            else if (m_appSettings.MapViewerType == MapviewerType.Normal)
            {
                mv = new MapViewer();
            }
            else
            {
                mv = new SimpleMapViewer();
            }
            mv.AutoUpdateChecksum = false;
            mv.GraphVisible = m_appSettings.ShowGraphs;
            // set viewsize
            mv.SetViewSize((Trionic5Tools.ViewSize)m_appSettings.DefaultViewSize);
            mv.Viewtype = Trionic5Tools.ViewType.Easy;
            mv.Map_content = data;
            int[] ixaxis = new int[xaxis.Length];
            int[] iyaxis = new int[yaxis.Length];
            for(int i = 0; i < xaxis.Length; i ++)
            {
                ixaxis.SetValue(Convert.ToInt32(xaxis.GetValue(i)), i);
            }
            for (int i = 0; i < yaxis.Length; i++)
            {
                iyaxis.SetValue(Convert.ToInt32(yaxis.GetValue(i)), i);
            }
            mv.X_axisvalues = ixaxis;
            mv.Y_axisvalues = iyaxis;
            mv.Map_name = "Matrix [" + xname + " : " + yname + " : " + zname + "]";
            
            mv.ShowTable(16, true);
            mv.SetDataTable(dtdata);
            
            int cols = 16;
            int rows = 16;
            TryToAddOpenLoopTables(mv);
            mv.InitEditValues();

            mv.Dock = DockStyle.Fill;
            mv.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
            // what todo on a save action (sram/binary)?
            int width = mv.DetermineWidth();
            if (dp.Width != width) dp.Width = width;
            dp.Text = "Matrix [" + xname + " : " + yname + " : " + zname + "]";
            //if (_ecuConnection.Opened && _ECUmode == OperationMode.ModeOnline) dp.Text += " Online";
            dp.Controls.Add(mv);

            bool isDocked = false;
            
            dockManager1.EndUpdate();
        }

        DataTable avgTable;

        private void AddPointToDataTable(DataTable dt, double x, double y, double z, double xmin, double xmax, double ymin, double ymax, double zmin, double zmax, int type)
        {
            // table is 16x16
            if (avgTable == null)
            {
                avgTable = new DataTable();
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

        

        private void btnShowVectors_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    frmVectorlist vectors = new frmVectorlist();
                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("Vector");
                    dt.Columns.Add("Address", System.Type.GetType("System.Int64"));
                    string description = string.Empty;
                    for (int i = 0; i < 256; i++)
                    {

                        long address = m_trionicFile.GetStartVectorAddress(m_trionicFileInformation.Filename, i);
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
            }
        }

        private void btnDisassembler_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // run disassembler
            if (m_trionicFile != null)
            {
                string outputfile = Path.GetDirectoryName(m_trionicFileInformation.Filename);
                outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + ".asm");

                //if (props.IsTrionic55)
                {
                    Application.DoEvents();
                    //logger.Debug("Starting with " + outputfile);
                    //Process.Start(Application.StartupPath + "\\TextEditor.exe", "\"" + outputfile + "\"");
                    DockPanel panel = dockManager1.AddPanel(DockingStyle.Right);
                    panel.Width = this.ClientSize.Width - dockSymbols.Width;
                    ctrlDisassembler disasmcontrol = new ctrlDisassembler();
                    disasmcontrol.TrionicFile = m_trionicFile;
                    disasmcontrol.Dock = DockStyle.Fill;
                    panel.Controls.Add(disasmcontrol);
                    panel.Text = "T5Suite 2.0 Disassembler";
                    Application.DoEvents();
                    disasmcontrol.DisassembleFile(outputfile);
                }

               /* else
                {
                    //T5.2
                    if (!AssemblerViewerActive(true, outputfile))
                    {
                        if (!File.Exists(outputfile))
                        {
                            Disassembler dis = new Disassembler();
                            dis.DisassembleFile(true, (long)m_trionicFileInformation.Filelength + 0x40000, m_trionicFileInformation.Filename, outputfile, (long)0, (long)m_trionicFileInformation.Filelength, m_trionicFileInformation.SymbolCollection);
                        }
                        StartAssemblerViewer(outputfile);
                    }
                }*/
            }
        }


        void disasm_onProgress(object sender, Disassembler.ProgressEventArgs e)
        {
           
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
            if (m_trionicFileInformation.Filename != "")
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
                    
                    av.LoadDataFromFile(filename, m_trionicFileInformation.SymbolCollection);
                    av.FindStartAddress();
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // show the about screen
            frmAbout about = new frmAbout();
            about.SetVersion(Application.ProductVersion.ToString());
            about.ShowDialog();
        }

        private void btnAfterStartEnrichment1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetFirstAfterStartEnrichmentMap());
        }

        private void btnAfterStartEnrichment2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetSecondAfterStartEnrichmentMap());
        }

        private void btnIdleTargetRPM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIdleTargetRPMMap());
        }

        private void btnIdleIgnition_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIdleIgnition());
        }

        private void btnIdleIgnitionCorrection_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIdleIgnitionCorrectionMap());
        }

        private void btnIdleFuelMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartTableViewer(m_trionicFileInformation.GetIdleFuelMap());
        }

        private void btnReadCANUSB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DownloadFlashCANUSB();
        }

        private void btnWriteCANUSB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UploadFlashCANUSB();
        }

        private void btnSRAMSnapshotCANUSB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DownloadSRAMCANUSB();
        }

        private void btnPeMicroSettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPeMicroParameters frmpe = new frmPeMicroParameters();
            frmpe.ECUReadFile = m_appSettings.Read_ecubatchfile;

            frmpe.ECUWriteAMDFile = m_appSettings.Write_ecuAMDbatchfile;
            frmpe.ECUWriteAtmelFile = m_appSettings.Write_ecuAtmelbatchfile;
            frmpe.ECUWriteIntelFile = m_appSettings.Write_ecuIntelbatchfile;
            frmpe.ECUBruteforceEraseFile = m_appSettings.Erasebruteforcebatchfile;

            frmpe.TargetECUReadFile = m_appSettings.TargetECUReadFile;

            if (frmpe.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Read_ecubatchfile = frmpe.ECUReadFile;
                m_appSettings.Write_ecuAMDbatchfile = frmpe.ECUWriteAMDFile;
                m_appSettings.Write_ecuAtmelbatchfile = frmpe.ECUWriteAtmelFile;
                m_appSettings.Write_ecuIntelbatchfile = frmpe.ECUWriteIntelFile;
                m_appSettings.Erasebruteforcebatchfile = frmpe.ECUBruteforceEraseFile;
                m_appSettings.TargetECUReadFile = frmpe.TargetECUReadFile;
            }
        }

        private void btnPeMicroRead_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_appSettings.Read_ecubatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Read_ecubatchfile))
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo = new System.Diagnostics.ProcessStartInfo(m_appSettings.Read_ecubatchfile);
                        process.Start();
                        process.WaitForExit();
                        // now, import the resulting S19 file
                        string fromfile = Path.GetDirectoryName(m_appSettings.Read_ecubatchfile) + "\\FROM_ECU.S19";
                        string destfile = Path.GetDirectoryName(m_trionicFileInformation.Filename) + "\\FROM_ECU" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".S19";
                        File.Copy(fromfile, destfile, true);
                        if (m_appSettings.TargetECUReadFile != string.Empty)
                        {
                            File.Copy(fromfile, m_appSettings.TargetECUReadFile, true);
                            OpenWorkingFile(m_appSettings.TargetECUReadFile);
                        }
                        else
                        {
                            OpenWorkingFile(destfile);
                        }
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Batch file not found. Check parameters");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }

        private void btnPeMicroBruteforceErase_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // brute force erase batch file
            if (m_appSettings.Erasebruteforcebatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Write_ecuIntelbatchfile))
                    {
                        System.Diagnostics.Process.Start(m_appSettings.Erasebruteforcebatchfile);
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Batch file not found. Check parameters");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }

        private void btnPeMicroProgramAMD_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // write the required file for flashing the ECU
            // this is the current file, exported to S19 format in the directory that contains
            // the selected batchfile
            if (m_appSettings.Write_ecuAMDbatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Write_ecuAMDbatchfile))
                    {
                        m_trionicFile.UpdateChecksum();

                        srec2bin sr = new srec2bin();
                        sr.ConvertBinToSrec(m_trionicFileInformation.Filename);
                        // and copy it to the target directory
                        string fromfile = Path.GetDirectoryName(m_trionicFileInformation.Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + ".S19";
                        string destfile = Path.GetDirectoryName(m_appSettings.Write_ecuAMDbatchfile) + "\\TO_ECU.S19";
                        File.Copy(fromfile, destfile, true);
                        System.Diagnostics.Process.Start(m_appSettings.Write_ecuAMDbatchfile);
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Batch file not found. Check parameters");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }

        private void btnPeMicroProgramIntel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // write the required file for flashing the ECU (INTEL)
            // this is the current file, exported to S19 format in the directory that contains
            // the selected batchfile
            if (m_appSettings.Write_ecuIntelbatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Write_ecuIntelbatchfile))
                    {
                        m_trionicFile.UpdateChecksum();

                        srec2bin sr = new srec2bin();
                        sr.ConvertBinToSrec(m_trionicFileInformation.Filename);
                        // and copy it to the target directory
                        string fromfile = Path.GetDirectoryName(m_trionicFileInformation.Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + ".S19";
                        string destfile = Path.GetDirectoryName(m_appSettings.Write_ecuIntelbatchfile) + "\\TO_ECU.S19";
                        File.Copy(fromfile, destfile, true);
                        System.Diagnostics.Process.Start(m_appSettings.Write_ecuIntelbatchfile);
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Batch file not found. Check parameters");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }

        private void btnPeMicroProgramAtmel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // write the required file for flashing the ECU (Atmel)
            // this is the current file, exported to S19 format in the directory that contains
            // the selected batchfile
            if (m_appSettings.Write_ecuAtmelbatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Write_ecuAtmelbatchfile))
                    {
                        m_trionicFile.UpdateChecksum();

                        srec2bin sr = new srec2bin();
                        sr.ConvertBinToSrec(m_trionicFileInformation.Filename);
                        // and copy it to the target directory
                        string fromfile = Path.GetDirectoryName(m_trionicFileInformation.Filename) + "\\" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + ".S19";
                        string destfile = Path.GetDirectoryName(m_appSettings.Write_ecuAtmelbatchfile) + "\\TO_ECU.S19";
                        File.Copy(fromfile, destfile, true);
                        System.Diagnostics.Process.Start(m_appSettings.Write_ecuAtmelbatchfile);
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Batch file not found. Check parameters");
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
        }

        private void showAxisInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string symbolname = string.Empty;
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    Trionic5Tools.SymbolHelper dr = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    if (dr.Varname != "")
                    {
                        symbolname = dr.Varname;
                    }
                }
            }
            DevExpress.XtraBars.Docking.DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
            AxisBrowser tabdet = new AxisBrowser();
            //tabdet.TheMainForm = this;
            tabdet.onStartSymbolViewer += new AxisBrowser.StartSymbolViewer(tabdet_onStartSymbolViewer);
            tabdet.Dock = DockStyle.Fill;
            dockPanel.Controls.Add(tabdet);
            tabdet.ShowSymbolCollection(m_trionicFileInformation.SymbolCollection);
            tabdet.SetCurrentSymbol(symbolname);
            dockPanel.Text = "Axis browser: " + Path.GetFileName(m_trionicFileInformation.Filename);
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

        private void btnCompareToOriginalFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // only possible when ori file is present
            // get the partnumber from the file
            CompareToFile(Application.StartupPath + "\\Binaries\\" + props.Partnumber + ".bin");
        }

        private void gridViewSymbols_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // show help
            TryShowHelpForSymbol();
        }

        private void btnChangeRegkonMatRange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Trionic5Tuner tuner = new Trionic5Tuner();
            tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            //Make this stuff user selectable!
            frmBoostBiasWizard boostrangeWiz = new frmBoostBiasWizard();
            int originalStep = m_trionicFile.GetRegulationDivisorValue();
            boostrangeWiz.RangeStep = originalStep;
            //boostrangeWiz.HardcodedRPMLimit = m_trionicFile.GetHardcodedRPMLimit(m_trionicFileInformation.Filename);
            if (boostrangeWiz.ShowDialog() == DialogResult.OK)
            {
                if (tuner.SetBoostRegulationDivisor(boostrangeWiz.RangeStep, originalStep, m_trionicFileInformation))
                {
                    // save
                    m_trionicFile.SetRegulationDivisorValue(boostrangeWiz.RangeStep);
                    //m_trionicFile.SetHardcodedRPMLimit(m_trionicFileInformation.Filename, boostrangeWiz.HardcodedRPMLimit);
                    frmInfoBox info = new frmInfoBox("Boost bias range has been changed");
                }
                m_trionicFile.UpdateChecksum(); //<GS-28112009>

            }
        }

        private void btnShowTransactionLog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    entry.SymbolName = m_trionicFileInformation.GetSymbolNameByAddress(entry.SymbolAddress);

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
            while (addressToWrite > m_trionicFileInformation.Filelength) addressToWrite -= m_trionicFileInformation.Filelength;
            m_trionicFile.WriteDataNoLog(entry.DataAfter, (uint)addressToWrite);
            m_ProjectTransactionLog.SetEntryRolledForward(entry.TransactionNumber);
            if (m_CurrentWorkingProject != string.Empty)
            {
                
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledforward, m_trionicFileInformation.GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
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

        private void RollBack(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > m_trionicFileInformation.Filelength) addressToWrite -= m_trionicFileInformation.Filelength;
            m_trionicFile.WriteDataNoLog(entry.DataBefore, (uint)addressToWrite);
            m_ProjectTransactionLog.SetEntryRolledBack(entry.TransactionNumber);
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledback, m_trionicFileInformation.GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }

            UpdateRollbackForwardControls();
        }

        private void btnRollback_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnRollForward_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        void m_trionicFile_onTransactionLogChanged(object sender, TransactionsEventArgs e)
        {
            UpdateRollbackForwardControls();
            // should contain the new info as well
            // <GS-18032010> insert logbook entry here if project is opened
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionExecuted, m_trionicFileInformation.GetSymbolNameByAddress(e.Entry.SymbolAddress) + " " + e.Entry.Note);
            }
        }


        private void UpdateRollbackForwardControls()
        {
            btnRollback.Enabled = false;
            btnRollForward.Enabled = false;
            btnShowTransactionLog.Enabled = false;

            for (int t = m_ProjectTransactionLog.TransCollection.Count - 1; t >= 0; t--)
            {
                if (!btnShowTransactionLog.Enabled) btnShowTransactionLog.Enabled = true;
                if (m_ProjectTransactionLog.TransCollection[t].IsRolledBack)
                {
                    btnRollForward.Enabled = true;
                }
                else
                {
                    btnRollback.Enabled = true;
                }
            }
        }

        private string GetBackupOlderThanDateTime(string project, DateTime mileDT)
        {
            string retval = m_trionicFileInformation.Filename; // default = current file
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


        private void btnRecreateFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                IECUFile m_RebuildFile = new Trionic5File();
                m_RebuildFile.LibraryPath = Application.StartupPath + "\\Binaries";

                IECUFileInformation m_RebuildFileInformation = new Trionic5FileInformation();
                
                m_RebuildFile.SelectFile(tempRebuildFile);
                m_RebuildFileInformation = m_RebuildFile.ParseFile();
                FileInfo fi = new FileInfo(file2Process);
                foreach (TransactionEntry te in m_ProjectTransactionLog.TransCollection)
                {
                    if (te.EntryDateTime >= fi.LastAccessTime && te.EntryDateTime <= filepar.SelectedDateTime)
                    {
                        // apply this change
                        RollForwardOnFile(m_RebuildFile, te);
                    }
                }
                // rename/copy file
                if (filepar.UseAsNewProjectFile)
                {
                    // just delete the current file
                    File.Delete(m_trionicFileInformation.Filename);
                    File.Copy(tempRebuildFile, m_trionicFileInformation.Filename);
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

        private void RollForwardOnFile(IECUFile file2Rollback, TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > file2Rollback.GetFileInfo().Filelength) addressToWrite -= file2Rollback.GetFileInfo().Filelength;
            file2Rollback.WriteDataNoLog(entry.DataAfter, (uint)addressToWrite);
        }

        private void btnEditProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                EditProjectProperties(m_CurrentWorkingProject);
            }

            //let the user select a project from the Project folder. If none are present, let the user know
            
            
        }

        private void EditProjectProperties(string project)
        {
            // edit current project properties
            DataTable projectprops = new DataTable("T5PROJECT");
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
                // delete the original XML file
                File.Delete(m_appSettings.ProjectFolder + "\\" + project + "\\projectproperties.xml");
                DataTable dtProps = new DataTable("T5PROJECT");
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

        private void btnProjectNote_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void ctrlRealtime1_onLoggingStarted(object sender, ctrlRealtime.LoggingEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.LogfileStarted, e.File);
            }
        }

        private void btnShowProjectLogbook_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                frmProjectLogbook logb = new frmProjectLogbook();

                logb.LoadLogbookForProject(m_appSettings.ProjectFolder, m_CurrentWorkingProject);
                logb.Show();
            }
        }

        private void btnProduceBinaryFromProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // save binary as
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // copy the current project file to the selected destination
                File.Copy(m_trionicFileInformation.Filename, sfd.FileName, true);
            }
        }

        private void btnShowDynoGraph_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                frmDynoChart chart = new frmDynoChart();
                chart.TrionicFile = m_trionicFile;
                chart.AppSettings = m_appSettings;
                chart.BuildGraph();
                chart.Show();
            }
        }

        private void btnNewAFRTargetMap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // create a new map based on the injectors used
            //if (m_AFRMaps != null)
            {
                if (m_trionicFile != null)
                {
                    if (m_AFRMaps == null)
                    {
                        m_AFRMaps = new AFRMaps();
                        m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                        m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                        m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                        m_AFRMaps.TrionicFile = m_trionicFile;
                        m_AFRMaps.InitializeMaps();
                    }
                    m_AFRMaps.CreateTargetMap(props.InjectorType);
                    ShowAfrMAP("TargetAFR", m_AFRMaps.LoadTargetAFRMapInBytes());
                }
            }
        }

        private void btnFreeTune_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // create a backup!
            // start the parameter screen (user can enter turbo, injector, mapsensor, peak torque and peak power)
            // start the freetune wizard
            frmFreeTuneSettings tunset = new frmFreeTuneSettings();
            Trionic5Properties props = m_trionicFile.GetTrionicProperties();
            tunset.SetMapSensorType(props.MapSensorType);
            tunset.SetInjectorType(props.InjectorType);
            tunset.SetTurboType(props.TurboType);
            ECUFileType fileType = m_trionicFile.DetermineFileType();
            int frek230 = m_trionicFile.GetSymbolAsInt("Frek_230!");
            int frek250 = m_trionicFile.GetSymbolAsInt("Frek_250!");

            int knockTime = m_trionicFile.GetSymbolAsInt("Knock_matrix_time!");
            tunset.SetKnockTime(knockTime);
            int rpmLimit = m_trionicFile.GetSymbolAsInt("Rpm_max!");
            tunset.SetRPMLimiter(rpmLimit);

            if (fileType == ECUFileType.Trionic52File)
            {
                if (frek230 == 728 || frek250 == 935)
                {
                    //dtReport.Rows.Add("APC valve type: Trionic 5");
                    tunset.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunset.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            else if (fileType == ECUFileType.Trionic55File)
            {
                if (frek230 == 90 || frek250 == 70)
                {
                    tunset.SetBPCType(BPCType.Trionic5Valve);
                }
                else
                {
                    tunset.SetBPCType(BPCType.Trionic7Valve);
                }
            }
            if (tunset.ShowDialog() == DialogResult.OK)
            {
                Trionic5Tuner _tuner = new Trionic5Tuner();
                TuningResult res = _tuner.FreeTuneBinary(m_trionicFile, tunset.GetPeakTorque(), tunset.GetPeakBoost(), tunset.IsTorqueBased, tunset.GetMapSensorType(), tunset.GetTurboType(), tunset.GetInjectorType(), tunset.GetBCVType(), tunset.GetRPMLimiter(), tunset.GetKnockTime());
                string text = "Tuning process completed!";
                if (res == TuningResult.TuningFailedAlreadyTuned)
                {
                    text = "Tuning process aborted, file is already tuned!";
                }
                else if (res == TuningResult.TuningFailedThreebarSensor)
                {
                    text = "Tuning process aborted, file was converted to another mapsensor type before!";
                }
                frmInfoBox info = new frmInfoBox(text);
            }
        }

        private void tmrOverruleTPS_Tick(object sender, EventArgs e)
        {
            _overruleTPS = false;
        }

        Trionic5Tools.SymbolCollection m_RealtimeUserSymbols = new Trionic5Tools.SymbolCollection();

        private void ctrlRealtime1_onAddSymbolToMonitorList(object sender, ctrlRealtime.MapDisplayRequestEventArgs e)
        {
            
            // User added a symbol... maybe we need to keep track of these symbol ourselves and store it as a usercollection
            bool _fnd = false;
            foreach (Trionic5Tools.SymbolHelper sh in m_RealtimeUserSymbols)
            {
                if (sh.Varname == e.MapName)
                {
                    _fnd = true;
                }
            }
            if (!_fnd)
            {
                Trionic5Tools.SymbolHelper shnew = new Trionic5Tools.SymbolHelper();
                shnew.Varname = e.MapName;
                shnew.Flash_start_address = m_trionicFileInformation.GetSymbolAddressFlash(e.MapName);
                shnew.Length = m_trionicFileInformation.GetSymbolLength(e.MapName);
                shnew.Start_address = m_trionicFileInformation.GetSymbolAddressSRAM(e.MapName);
                shnew.UserCorrectionFactor = e.CorrectionFactor;
                shnew.UserCorrectionOffset = e.CorrectionOffset; 
                shnew.UseUserCorrection = e.UseUserCorrection;
                m_RealtimeUserSymbols.Add(shnew);
                _ecuConnection.AddSymbolToWatchlist(shnew, false);
                ctrlRealtime1.SetRealtimeSymbollist(m_RealtimeUserSymbols);
            }
        }

        private void ctrlRealtime1_onRemoveSymbolFromMonitorList(object sender, ctrlRealtime.MapDisplayRequestEventArgs e)
        {
            _ecuConnection.RemoveSymbolFromWatchlist(e.MapName);
            // remove it from the usercollection as well (if present)
            foreach (Trionic5Tools.SymbolHelper sh in m_RealtimeUserSymbols)
            {
                if (sh.Varname == e.MapName)
                {
                    m_RealtimeUserSymbols.Remove(sh);
                    break;
                }
            }
            ctrlRealtime1.SetRealtimeSymbollist(m_RealtimeUserSymbols);
        }

        private void btnWriteLogMarker_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // set logmarker NOW in realtime panel
            ctrlRealtime1.WriteLogMarker();
        }

        #region Skins

        private void SetToolstripTheme()
        {
          /*  logger.Debug("Rendermode was: " + ToolStripManager.RenderMode.ToString());
            logger.Debug("Visual styles: " + ToolStripManager.VisualStylesEnabled.ToString());
            if (DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName.Contains("Black") || DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName.Contains("black") || DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName.Contains("Dark") || DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName.Contains("dark") || DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName.Contains("Pumpkin"))
            {
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
                ProfColorTable profcolortable = new ProfColorTable();
                profcolortable.CustomToolstripGradientBegin = Color.LightGray;
                profcolortable.CustomToolstripGradientMiddle = Color.Gray;
                profcolortable.CustomToolstripGradientEnd = Color.Black;
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(profcolortable);
            }
            else
            {
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer();
            }*/
            //logger.Debug("Rendermode was: " + ToolStripManager.RenderMode.ToString());
            //logger.Debug("Visual styles: " + ToolStripManager.VisualStylesEnabled.ToString());
            //logger.Debug("Skinname: " + appSettings.SkinName);
            //logger.Debug("Backcolor: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor.ToString());
            //logger.Debug("Backcolor2: " + defaultLookAndFeel1.LookAndFeel.Painter.Button.DefaultAppearance.BackColor2.ToString());
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

        //string skinMask = "Skin: ";
        /// <summary>
        /// InitSkins: Initialiseer de skin engine om het voor de gebruiker mogelijk te maken
        /// om een skin te kiezen voor de user interface
        /// </summary>
        void InitSkins()
        {
            ribbonControl1.ForceInitialize();
            BarButtonItem item;
            int skinCount = 0;
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.BonusSkins).Assembly);
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);
            Trionic5Tools.SymbolCollection symcol = new Trionic5Tools.SymbolCollection();
            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                Trionic5Tools.SymbolHelper sh = new Trionic5Tools.SymbolHelper();
                sh.Varname = cnt.SkinName;
                symcol.Add(sh);
            }
            symcol.SortColumn = "Varname";
            symcol.SortingOrder = Trionic5Tools.GenericComparer.SortOrder.Ascending;
            symcol.Sort();
            foreach(Trionic5Tools.SymbolHelper sh in symcol)
            {
                item = new BarButtonItem();
                item.Caption = sh.Varname;
                BarItemLink il = rbnPageSkins.ItemLinks.Add(item);
                if ((skinCount++ % 3) == 0) il.BeginGroup = true;
                
                item.ItemClick += new ItemClickEventHandler(OnSkinClick);
            }
            try
            {
                if (m_appSettings.Skinname != string.Empty)
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
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            SetToolstripTheme();
        }

        private void ShowChristmasWish()
        {
            int newyear = DateTime.Now.Year + 1;
            frmInfoBox info = new frmInfoBox("Merry christmas and a happy " + newyear.ToString("D4") + "\rDilemma");
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

        #endregion

        private void RunMapSensorWizard(MapSensorType targetMapSensorType)
        {
            if (m_trionicFile.Exists())
            {
                Trionic5Tuner _tuner = new Trionic5Tuner();
                string targetMapSensorString = "3 bar mapsensor";
                switch (targetMapSensorType)
                {
                    case MapSensorType.MapSensor25:
                        targetMapSensorString = "2.5 bar mapsensor";
                        break;
                    case MapSensorType.MapSensor30:
                        targetMapSensorString = "3.0 bar mapsensor";
                        break;
                    case MapSensorType.MapSensor35:
                        targetMapSensorString = "3.5 bar mapsensor";
                        break;
                    case MapSensorType.MapSensor40:
                        targetMapSensorString = "4.0 bar mapsensor";
                        break;
                    case MapSensorType.MapSensor50:
                        targetMapSensorString = "5.0 bar mapsensor";
                        break;
                }
                MapSensorType fromMapSensortype = m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType);
                frmMapSensorWizard mapSensorWiz = new frmMapSensorWizard();
                mapSensorWiz.SetMapSensorTypes(fromMapSensortype, targetMapSensorType);
                if (mapSensorWiz.ShowDialog() == DialogResult.OK)
                {
                    _tuner.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    _tuner.ConvertFileToThreeBarMapSensor(m_trionicFileInformation, fromMapSensortype, MapSensorType.MapSensor30);
                    props = m_trionicFile.GetTrionicProperties();
                    m_trionicFile.UpdateChecksum();
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = targetMapSensorString + " report";
                    tuningrep.DataSource = _tuner.Resume.ResumeTuning;
                    tuningrep.CreateReport();
                    tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);

                }
            }
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            // convert to 3.0 bar sensor
            // <GS-10012011> create new wizard here
            RunMapSensorWizard(MapSensorType.MapSensor30);
            
        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            // convert to 3.5 bar sensor
            RunMapSensorWizard(MapSensorType.MapSensor35);
        }

        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            // <GS-10012011> create new wizard here
            RunMapSensorWizard(MapSensorType.MapSensor40);
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            // <GS-10012011> create new wizard here
            RunMapSensorWizard(MapSensorType.MapSensor50);

        }


        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            // <GS-10012011> create new wizard here
            RunMapSensorWizard(MapSensorType.MapSensor25);
        }

        private void btnInjectionTiming_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    frmInjectionTiming injectiontiming = new frmInjectionTiming();
                    injectiontiming.Insp_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                    injectiontiming.Fuel_knock_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectionKnockMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionKnockMap()));
                    injectiontiming.Idle_fuel_map = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIdleFuelMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIdleFuelMap()));
                    injectiontiming.Batt_korr_tab = m_trionicFile.GetSymbolAsIntArray(m_trionicFileInformation.GetBatteryCorrectionMap());
                    injectiontiming.Min_tid = m_trionicFile.GetSymbolAsInt("Min_tid!");

                    injectiontiming.Fuel_map_x_axis = m_trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, m_trionicFileInformation.GetInjectionMap());
                    injectiontiming.Fuel_map_y_axis = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, m_trionicFileInformation.GetInjectionMap());
                    injectiontiming.Fuel_knock_map_x_axis = m_trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, "Fuel_knock_mat!");
                    injectiontiming.Fuel_knock_map_y_axis = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, "Fuel_knock_mat!");
                    injectiontiming.Idle_fuel_x_axis = m_trionicFile.GetXaxisValues(m_trionicFileInformation.Filename, "Idle_fuel_korr!");
                    injectiontiming.Idle_fuel_y_axis = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, "Idle_fuel_korr!");

                    injectiontiming.Luft_kompfak = m_trionicFile.Luft_kompfak_array;
                    injectiontiming.Temp_steg = m_trionicFile.Temp_steg_array;
                    injectiontiming.Kyltemp_steg = m_trionicFile.Kyltemp_steg_array;
                    injectiontiming.Kyltemp_tab = m_trionicFile.Kyltemp_tab_array;
                    injectiontiming.Lufttemp_steg = m_trionicFile.Lufttemp_steg_array;
                    injectiontiming.Lufttemp_tab = m_trionicFile.Lufttemp_tab_array;
                    byte[] data = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetInjectorConstant()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectorConstant()));
                    int inj_konst = Convert.ToInt32(data.GetValue(0));
                    injectiontiming.Inj_konst = inj_konst;
                    injectiontiming.MapSensor = m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType);
                    injectiontiming.CalculateInjectionTiming(InjectionType.Normal);
                    injectiontiming.ShowDialog();
                }
            }
        }

        private void btnAnomaliesChecker_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    DataTable dtReport = m_trionicFile.CheckForAnomalies();
                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Anomaly report";
                    tuningrep.SetDataSource(dtReport);
                    tuningrep.CreateReport();
                    tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);

                }
            }
        }

        private void btnUploadSRAMToECU_ItemClick(object sender, ItemClickEventArgs e)
        {
            // have the user select an sram file and upload all maps into sram
            // this will restore the ECU to that state
            
            if (m_trionicFile != null && _ecuConnection.Opened)
            {
                if (m_trionicFile.Exists())
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "SRAM snapshots|*.ram";
                    if (m_CurrentWorkingProject != "")
                    {
                        if (!Directory.Exists(m_appSettings.ProjectFolder +"\\" + m_CurrentWorkingProject + "\\Snapshots")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots");
                        ofd.InitialDirectory = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots";
                    }
                    else
                    {
                        ofd.InitialDirectory = Path.GetDirectoryName(m_trionicFileInformation.Filename);
                    }
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        // now save all the maps back to the ECU
                        DialogResult dr = MessageBox.Show("Do you want to write to the current binary file as well?", "Question", MessageBoxButtons.YesNo);
                        int percentage = 0;
                        int symcount = 0;
                        SetStatusText("Restoring ECU state...");
                        foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                        {
                            percentage = (symcount * 100) / m_trionicFileInformation.SymbolCollection.Count;
                            SetTaskProgress(percentage, true);

                            if (sh.Start_address > 0 && sh.Flash_start_address > 0)
                            {
                                // write the data into the ECU and notify the progress to the user
                                // if the user wants to sync into the bin as well, write to the bin also
                                byte[] data = m_trionicFile.ReadDataFromFile(ofd.FileName, (uint)sh.Start_address, (uint)sh.Length);
                                _ecuConnection.WriteSymbolDataForced(sh.Start_address, sh.Length, data);
                                if (dr == DialogResult.Yes)
                                {
                                    // also write into the binfile
                                    m_trionicFile.WriteDataNoLog(data, (uint)sh.Flash_start_address);
                                }
                            }
                            symcount++;
                        }
                        DateTime sync = DateTime.Now;
                        _ecuConnection.SetMemorySyncDate(sync);
                        if (dr == DialogResult.Yes)
                        {
                            m_trionicFile.SetMemorySyncDate(sync);
                        }
                        SetStatusText("Idle");
                        SetTaskProgress(0, false);
                    }
                }
            }
        }

        private void btnCompareECUWithBinary_ItemClick(object sender, ItemClickEventArgs e)
        {
            // compare the ECU's content to the binary file
            // for this, we need to fetch the data from the ECU and compare the data from there?
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    if (_ecuConnection.Opened)
                    {
                        string FileName = "Snapshot-" + Path.GetFileNameWithoutExtension(m_trionicFileInformation.Filename) + "-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".RAM";
                        _ecuConnection.DumpSRAM(FileName);
                        //Compare
                        StartCompareToSRAMFile(FileName);

                    }
                }
            }
        }

        private void ctrlRealtime1_onProgramModeChange(object sender, ctrlRealtime.ProgramModeEventArgs e)
        {
            // we have to do something with the Pgm_mod stuff
            _ecuConnection.StopECUMonitoring();
            Thread.Sleep(100);
            byte[] pgm_mod = _ecuConnection.ReadSymbolData("Pgm_mod!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Pgm_mod!"), (uint)m_trionicFileInformation.GetSymbolLength("Pgm_mod!"));
            // Pgm_mod! BYTE bytenumber and mask 
            if (pgm_mod.Length > e.ByteNumber)
            {
                if (e.Enable)
                {
                    pgm_mod[e.ByteNumber] |= e.Mask;
                    _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM("Pgm_mod!"), m_trionicFileInformation.GetSymbolLength("Pgm_mod!"), pgm_mod);
                    Thread.Sleep(50);
                }
                else
                {
                    pgm_mod[e.ByteNumber] &= (byte)(0xFF ^ e.Mask);
                    _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM("Pgm_mod!"), m_trionicFileInformation.GetSymbolLength("Pgm_mod!"), pgm_mod); Thread.Sleep(50);
                }
            }
            ctrlRealtime1.UpdateProgramModeButtons(_ecuConnection.ReadSymbolData("Pgm_mod!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Pgm_mod!"), (uint)m_trionicFileInformation.GetSymbolLength("Pgm_mod!")));
            _ecuConnection.StartECUMonitoring();
        }

        private void HandleMenuItem(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
                if (tsmi.Tag != null)
                {
                    try
                    {
                        string mapName = (string)tsmi.Tag;
                        StartTableViewer(mapName);
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                }
            }
        }

        private void automaticGearboxToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (props.IsTrionic55)
            {
                boostLimitIn1stGearToolStripMenuItem.Tag = "Regl_tryck_fgaut!"; // for T5.5
            }
            else
            {
                boostLimitIn1stGearToolStripMenuItem.Tag = "Regl_tryck_fga!"; // for T5.2
            }
        }

        private void manualGearboxToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (props.IsTrionic55)
            {
                boostLimitIn1stGearToolStripMenuItem1.Enabled = true;
                boostLimitIn2ndGearToolStripMenuItem.Enabled = true;
            }
            else
            {
                boostLimitIn1stGearToolStripMenuItem1.Enabled = false;
                boostLimitIn2ndGearToolStripMenuItem.Enabled = false;
            }
        }

        private void knockControlToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (props.IsTrionic55)
            {
                knockSensitivityMapToolStripMenuItem.Tag = "Knock_ref_matrix!";
                knockSensitivityMapToolStripMenuItem.Enabled = true;
                boostReductionMapToolStripMenuItem.Tag = "Apc_knock_tab!";
                boostReductionMapToolStripMenuItem.Enabled = true;
                ignitionRetardLimitToolStripMenuItem.Tag = "Knock_lim_tab!";
                ignitionRetardLimitToolStripMenuItem.Enabled = true;
                knockIndicationLoadLimitToolStripMenuItem.Tag = "Knock_press_tab!";
                knockIndicationLoadLimitToolStripMenuItem.Enabled = true;
                knockRegulationLoadLimitToolStripMenuItem.Tag = "Turbo_knock_tab!";
                knockRegulationLoadLimitToolStripMenuItem.Enabled = true;
                timeToStayInKnockModeAfterKnockDetectionToolStripMenuItem.Tag = "Knock_matrix_time!";
                timeToStayInKnockModeAfterKnockDetectionToolStripMenuItem.Enabled = true;
            }
            else
            {
                knockSensitivityMapToolStripMenuItem.Tag = "Knock_ref_tab!";
                knockSensitivityMapToolStripMenuItem.Enabled = true;
                boostReductionMapToolStripMenuItem.Tag = "Apc_knock_tab!";
                boostReductionMapToolStripMenuItem.Enabled = true;
                ignitionRetardLimitToolStripMenuItem.Tag = "Knock_lim_tab!";
                ignitionRetardLimitToolStripMenuItem.Enabled = false;
                knockIndicationLoadLimitToolStripMenuItem.Tag = "Knock_press_tab!";
                knockIndicationLoadLimitToolStripMenuItem.Enabled = false;
                knockRegulationLoadLimitToolStripMenuItem.Tag = "Knock_press!";
                knockRegulationLoadLimitToolStripMenuItem.Enabled = true;
                timeToStayInKnockModeAfterKnockDetectionToolStripMenuItem.Tag = "Knock_matrix_time!";
                timeToStayInKnockModeAfterKnockDetectionToolStripMenuItem.Enabled = true;
            }
        }

        private void barStaticItem2_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            // for test only
            if (dockSymbols.Visibility == DockVisibility.Visible)
            {
                dockSymbols.Visibility = DockVisibility.AutoHide;
                dockSymbols.HideImmediately();
            }
            else
            {
                dockSymbols.Visibility = DockVisibility.Visible;
            }

        }

        private void btnClearKnockCounters_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_ECUmode == OperationMode.ModeOnline)
            {
                Application.DoEvents();

                // write zeros to knock_count_cylx and knock_count_map
                byte[] cleardata = new byte[m_trionicFileInformation.GetSymbolLength("Knock_count_map")];
                cleardata.Initialize();
                _ecuConnection.WriteSymbolData(m_trionicFileInformation.GetSymbolAddressSRAM("Knock_count_map"), m_trionicFileInformation.GetSymbolLength("Knock_count_map"), cleardata);
                byte[] cleardatacyl = new byte[2];
                cleardatacyl.Initialize();
                _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder1Symbol()), 2, cleardatacyl);
                _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder2Symbol()), 2, cleardatacyl);
                _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder3Symbol()), 2, cleardatacyl);
                _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetKnockCountCylinder4Symbol()), 2, cleardatacyl);
            }
        }

        private void btnOpenReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            //DevExpress.XtraPrinting.Preview.Prev
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Reports|*.prnx";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PrintingSystem print = new PrintingSystem();
                print.LoadDocument(ofd.FileName);
                // Create an instance of the preview dialog.
                PrintPreviewFormEx preview = new PrintPreviewFormEx();

                // Load the report document into it.
                preview.PrintingSystem = print;

                // Show the preview dialog.
                preview.ShowDialog();

            }
        }

        private void ShowFloatingLog(string filename)
        {
            Application.DoEvents();
            System.Drawing.Point p = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 10, dockSymbols.Location.Y + 55));
            DevExpress.XtraBars.Docking.DockPanel dp = dockManager1.AddPanel(p);
            dp.MakeFloat(p);
            dp.FloatSize = new Size(dockManager1.Form.ClientSize.Width - dockSymbols.Width - 20, dockSymbols.Height - 10);
            dp.Hide();
            dp.Text = "CANBus logfile: " + Path.GetFileName(filename);
            RealtimeGraphControl lfv = new RealtimeGraphControl();
            dp.Controls.Add(lfv);
            lfv.ImportT5Logfile(filename);
            lfv.Dock = DockStyle.Fill;
            dp.Show();
        }

        private void ctrlRealtime1_onOpenLogFileRequest(object sender, ctrlRealtime.OpenLogFileRequestEventArgs e)
        {
            // the realtime panel requests to open a log file
            // check whether the user has set this option to "ON"
            if (m_appSettings.AutoOpenLogFile)
            {
                // create a new logfile window and make it floating over the realtime panel
                // load the designated file
                ShowFloatingLog(e.Filename);
                
            }
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            //ShowFloatingLog(@"C:\Documents and Settings\Guido.MOBICOACH\Desktop\Guido\Log MacExpert\20100313-CanTraceExtenhanced regkon.t5l");
        }

        private void btnBinExaminor_ItemClick(object sender, ItemClickEventArgs e)
        {
            // well, get the details on the binary and build a report
            // determine mapsensor type, injector type, turbo type, stage and make a report
            
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    MapSensorType mapsensor = m_trionicFile.GetMapSensorType(true);
                    DataTable dtReport = new DataTable();//m_trionicFile.CheckForAnomalies();
                    dtReport.Columns.Add("Description");

                    dtReport.Rows.Add("");
                    dtReport.Rows.Add("Report for file: " + Path.GetFileName(m_trionicFileInformation.Filename));
                    dtReport.Rows.Add("");
                    
                    ECUFileType fileType = m_trionicFile.DetermineFileType();
                    if (fileType == ECUFileType.Trionic52File)
                    {
                        dtReport.Rows.Add("File type: Trionic 5.2");
                    }
                    else if (fileType == ECUFileType.Trionic55File)
                    {
                        dtReport.Rows.Add("File type: Trionic 5.5");
                    }
                    Trionic5Properties t5p = m_trionicFile.GetTrionicProperties();
                    //m_trionicFileInformation.deter
                    dtReport.Rows.Add("CPU speed: " + t5p.CPUspeed);
                    dtReport.Rows.Add("Data name: " + t5p.Dataname);
                    dtReport.Rows.Add("Engine type: " + t5p.Enginetype);
                    dtReport.Rows.Add("Partnumber: " + t5p.Partnumber);
                    dtReport.Rows.Add("Software ID: " + t5p.SoftwareID);
                    if (t5p.RAMlocked) dtReport.Rows.Add("SRAM is locked");
                    else dtReport.Rows.Add("SRAM is unlocked");
                    float m_maxBoost = 0;
                    TuningStage _stage = m_trionicFile.DetermineTuningStage(out m_maxBoost);
                    switch (_stage)
                    {
                        case TuningStage.Stock:
                            dtReport.Rows.Add("Stage: stock");
                            break;
                        case TuningStage.Stage1:
                            dtReport.Rows.Add("Stage: 1");
                            break;
                        case TuningStage.Stage2:
                            dtReport.Rows.Add("Stage: 2");
                            break;
                        case TuningStage.Stage3:
                            dtReport.Rows.Add("Stage: 3");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: BCPR7ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.Stage4:
                            dtReport.Rows.Add("Stage: 4");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: BCPR7ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.Stage5:
                            dtReport.Rows.Add("Stage: 5");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: upgraded pressureplate");
                            dtReport.Rows.Add("\tRequires: GT28 turbo or better");
                            dtReport.Rows.Add("\tRequires: BCPR7ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.Stage6:
                            dtReport.Rows.Add("Stage: 6");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: upgraded pressureplate");
                            dtReport.Rows.Add("\tRequires: upgraded fuelpump");
                            dtReport.Rows.Add("\tRequires: wideband lambda");
                            dtReport.Rows.Add("\tRequires: EGT gauge");
                            dtReport.Rows.Add("\tRequires: GT3071r .64 turbo or better");
                            dtReport.Rows.Add("\tRequires: BCPR8ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.Stage7:
                            dtReport.Rows.Add("Stage: 7");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: upgraded pressureplate");
                            dtReport.Rows.Add("\tRequires: upgraded fuelpump");
                            dtReport.Rows.Add("\tRequires: wideband lambda");
                            dtReport.Rows.Add("\tRequires: EGT gauge");
                            dtReport.Rows.Add("\tRequires: GT3071r .86 turbo or better");
                            dtReport.Rows.Add("\tRequires: BCPR8ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.Stage8:
                            dtReport.Rows.Add("Stage: 8");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: upgraded pressureplate");
                            dtReport.Rows.Add("\tRequires: upgraded fuelpump");
                            dtReport.Rows.Add("\tRequires: tubular exhaust manifold");
                            dtReport.Rows.Add("\tRequires: wideband lambda");
                            dtReport.Rows.Add("\tRequires: EGT gauge");
                            dtReport.Rows.Add("\tRequires: HX40 hybrid (super) turbo or better");
                            dtReport.Rows.Add("\tRequires: BCPR8ES plugs");
                            dtReport.Rows.Add("");
                            break;
                        case TuningStage.StageX:
                            dtReport.Rows.Add("Stage: X");
                            dtReport.Rows.Add("\tRequires: 3'' turboback exhaust");
                            dtReport.Rows.Add("\tRequires: upgraded intercooler");
                            dtReport.Rows.Add("\tRequires: upgraded pressureplate");
                            dtReport.Rows.Add("\tRequires: upgraded fuelpump");
                            dtReport.Rows.Add("\tRequires: tubular exhaust manifold");
                            dtReport.Rows.Add("\tRequires: wideband lambda");
                            dtReport.Rows.Add("\tRequires: EGT gauge");
                            dtReport.Rows.Add("\tRequires: HX40 hybrid (super) turbo or better");
                            dtReport.Rows.Add("");
                            break;
                    }
                    dtReport.Rows.Add("Boost request peak: " + m_maxBoost.ToString("F2") + " bar");

                    if (mapsensor == MapSensorType.MapSensor25)
                    {
                        dtReport.Rows.Add("Mapsensor type: stock 2.5 bar sensor");
                    }
                    else if (mapsensor == MapSensorType.MapSensor30)
                    {
                        dtReport.Rows.Add("Mapsensor type: 3.0 bar sensor");
                    }
                    else if (mapsensor == MapSensorType.MapSensor35)
                    {
                        dtReport.Rows.Add("Mapsensor type: 3.5 bar sensor");
                    }
                    else if (mapsensor == MapSensorType.MapSensor40)
                    {
                        dtReport.Rows.Add("Mapsensor type: 4.0 bar sensor");
                    }
                    else if (mapsensor == MapSensorType.MapSensor50)
                    {
                        dtReport.Rows.Add("Mapsensor type: 5.0 bar sensor");
                    }
                    // determine injector type from file... ?
                    int injkonst = m_trionicFile.GetSymbolAsInt("Inj_konst!");
                    bool m_E85 = false;
                    int max_injection = m_trionicFile.GetMaxInjection();
                    max_injection *= injkonst;
                    // de maximale waarde uit fuel_map_x_axis! aub
                    byte[] fuelxaxis = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Fuel_map_xaxis!"), (uint)m_trionicFileInformation.GetSymbolLength("Fuel_map_xaxis!"));
                    int max_value_x_axis = Convert.ToInt32(fuelxaxis.GetValue(fuelxaxis.Length - 1));
                    if (mapsensor == MapSensorType.MapSensor30)
                    {
                        max_value_x_axis *= 120;
                        max_value_x_axis /= 100;
                    }
                    else if (mapsensor == MapSensorType.MapSensor35)
                    {
                        max_value_x_axis *= 140;
                        max_value_x_axis /= 100;
                    }
                    else if (mapsensor == MapSensorType.MapSensor40)
                    {
                        max_value_x_axis *= 160;
                        max_value_x_axis /= 100;
                    }
                    else if (mapsensor == MapSensorType.MapSensor50)
                    {
                        max_value_x_axis *= 200;
                        max_value_x_axis /= 100;
                    }
                    //logger.Debug("max x: " + max_value_x_axis.ToString());
                    float max_support_boost = max_value_x_axis;
                    max_support_boost /= 100;
                    max_support_boost -= 1;
                    float corr_inj = 1.4F / max_support_boost;
                    corr_inj *= 100;
                    //logger.Debug("corr_inj = " + corr_inj.ToString());
                    max_injection *= (int)corr_inj;
                    max_injection /= 100;

                   // dtReport.Rows.Add("Max injection: "+ max_injection.ToString());
                    if (max_injection > 7500) m_E85 = true;
                    if (injkonst > 26)
                    {
                        m_E85 = true;
                    }
                    //TODO: nog extra controleren of er andere indicatoren zijn of er E85 gebruikt wordt
                    // we kunnen dit aan de start verrijkingen zien en aan de ontstekingstijdstippen bij 
                    // vollast (ontsteking scherper), let op want dit laatste is bij W/M injectie ook zo.
                    // de een na laatste waarde uit Eftersta_fak! geeft een duidelijke indicatie
                    byte[] eftstafak = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash("Eftersta_fak!"), (uint)m_trionicFileInformation.GetSymbolLength("Eftersta_fak!"));
                    if (eftstafak.Length == 15)
                    {
                        int eftstafakvalue = Convert.ToInt32(eftstafak.GetValue(13));
                        if (eftstafakvalue > 170) m_E85 = true;
                    }
                    if (m_E85)
                    {
                        max_injection *= 10;
                        max_injection /= 14;
                        dtReport.Rows.Add("Probable fuel: E85");
                    }
                    if (!m_E85)
                    {
                        if (m_maxBoost > 1.1)
                        {
                            dtReport.Rows.Add("Probable fuel: Premium quality petrol");
                        }
                        else
                        {
                            dtReport.Rows.Add("Probable fuel: Petrol");
                        }
                    }
                    // get peak from insp_mat and multiply by injector constant
                    InjectorType inj_type = InjectorType.Stock;
                    if (max_injection > 5000) inj_type = InjectorType.Stock;
                    else if (max_injection > 3500) inj_type = InjectorType.GreenGiants;
                    else if (max_injection > 2000) inj_type = InjectorType.Siemens630Dekas;
                    else if (max_injection > 1565) inj_type = InjectorType.Siemens875Dekas;
                    else inj_type = InjectorType.Siemens1000cc;
                    switch (inj_type)
                    {
                        case InjectorType.Stock:
                            dtReport.Rows.Add("Injectors: stock");
                            break;
                        case InjectorType.GreenGiants:
                            dtReport.Rows.Add("Injectors: Green giants (413 cc/min)");
                            break;
                        case InjectorType.Siemens630Dekas:
                            dtReport.Rows.Add("Injectors: Siemens deka 630 cc/min");
                            break;
                        case InjectorType.Siemens875Dekas:
                            dtReport.Rows.Add("Injectors: Siemens deka 875 cc/min");
                            break;
                        case InjectorType.Siemens1000cc:
                            dtReport.Rows.Add("Injectors: Siemens deka 1000 cc/min");
                            break;
                    }
                        


                    // Add info about T5/T7 valve
                    int frek230 = m_trionicFile.GetSymbolAsInt("Frek_230!");
                    int frek250 = m_trionicFile.GetSymbolAsInt("Frek_250!");
                    if (fileType == ECUFileType.Trionic52File)
                    {
                        if (frek230 == 728 || frek250 == 935)
                        {
                            dtReport.Rows.Add("APC valve type: Trionic 5");
                        }
                        else
                        {
                            dtReport.Rows.Add("APC valve type: Trionic 7");
                        }
                    }
                    else if (fileType == ECUFileType.Trionic55File)
                    {
                        if (frek230 == 90 || frek250 == 70)
                        {
                            dtReport.Rows.Add("APC valve type: Trionic 5");
                        }
                        else
                        {
                            dtReport.Rows.Add("APC valve type: Trionic 7");
                        }
                    }



                    TuningReport tuningrep = new TuningReport();
                    tuningrep.ReportTitle = "Examination report";
                    tuningrep.SetDataSource(dtReport);
                    tuningrep.CreateReport();
                    tuningrep.ShowPreview(defaultLookAndFeel1.LookAndFeel);
                    //tuningrep.ShowReportPreview(defaultLookAndFeel1.LookAndFeel);

                }
            }
           
            
        }

        private void btnConfigureRealtimePanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (dockRealtime.Visibility == DockVisibility.Hidden)
            {
                //<GS-27072010> hier nog de realtime symbolen wel toevoegen
                ctrlRealtime1.AppSettings = m_appSettings;
                ctrlRealtime1.WideBandAFRSymbol = m_appSettings.WidebandLambdaSymbol;
                ctrlRealtime1.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
                ctrlRealtime1.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
                ctrlRealtime1.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                ctrlRealtime1.CellStableTime_ms = m_appSettings.CellStableTime_ms;
                ctrlRealtime1.CorrectionPercentage = m_appSettings.CorrectionPercentage;
                ctrlRealtime1.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
                ctrlRealtime1.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
                ctrlRealtime1.EnrichmentFilter = m_appSettings.EnrichmentFilter;
                ctrlRealtime1.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
                ctrlRealtime1.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
                ctrlRealtime1.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
                ctrlRealtime1.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
                ctrlRealtime1.AutoLoggingEnabled = m_appSettings.AutoLoggingEnabled;
                ctrlRealtime1.AutoLogStartSign = m_appSettings.AutoLogStartSign;
                ctrlRealtime1.AutoLogStartValue = m_appSettings.AutoLogStartValue;
                ctrlRealtime1.AutoLogStopSign = m_appSettings.AutoLogStopSign;
                ctrlRealtime1.AutoLogStopValue = m_appSettings.AutoLogStopValue;
                ctrlRealtime1.AutoLogTriggerStartSymbol = m_appSettings.AutoLogTriggerStartSymbol;
                ctrlRealtime1.AutoLogTriggerStopSymbol = m_appSettings.AutoLogTriggerStopSymbol;
                if (m_trionicFile != null)
                {
                    ctrlRealtime1.MapSensor = m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType);
                    ctrlRealtime1.Fuelxaxis = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetInjectionMap());
                    ctrlRealtime1.Fuelyaxis = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetInjectionMap());
                    ctrlRealtime1.Ignitionxaxis = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetIgnitionMap());
                    ctrlRealtime1.Ignitionyaxis = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetIgnitionMap());
                }

                ctrlRealtime1.EnableAdvancedMode = m_appSettings.EnableAdvancedMode;
                dockRealtime.Visibility = DockVisibility.Visible;
            }
        }

        private void btnErrorCodes_ItemClick(object sender, ItemClickEventArgs e)
        {
            //_error
            if (m_trionicFile != null && _ecuConnection.Opened)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Symbol");
                dt.Columns.Add("Value", Type.GetType("System.Int32"));
                foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                {
                    if ((sh.Varname.Contains("_error") || sh.Varname.Contains("_fel")) && sh.Length == 1)
                    {
                        // read code
                        byte[] data = _ecuConnection.ReadSymbolData(sh.Varname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(sh.Varname), (uint)m_trionicFileInformation.GetSymbolLength(sh.Varname));
                        if (data.Length == 1)
                        {
                            Int32 _counter = Convert.ToInt32(data.GetValue(0));
                            if (_counter > 0)
                            {
                                // add it to the list
                                dt.Rows.Add(sh.Varname, _counter);
                            }
                        }
                    }
                }
                frmDTCCodes codes = new frmDTCCodes();
                codes.onClearErrorCodes += new frmDTCCodes.ClearErrorCodes(codes_onClearErrorCodes);
                codes.SetDataSet(dt);
                codes.Show();
            }
            // get the error code list from the ECU
        }

        void codes_onClearErrorCodes(object sender, EventArgs e)
        {
            // clear all DTC codes
            Application.DoEvents();
            byte[] zerobyte = new byte[1];
            zerobyte.SetValue((byte)0x00, 0);
            if (m_trionicFile != null)
            {
                foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                {
                    try
                    {
                        if (sh.Varname.EndsWith("_error"))
                        {
                            if (sh.Length == 1 && sh.Start_address > 0)
                            {
                                _ecuConnection.WriteSymbolDataForced(sh.Start_address, sh.Length, zerobyte);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Failed to clear errorcounter: " + sh.Varname + ". " + E.Message);
                    }
                }
            }
            if (sender is frmDTCCodes)
            {
                frmDTCCodes codes = (frmDTCCodes)sender;
                if (m_trionicFile != null && _ecuConnection.Opened)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Symbol");
                    dt.Columns.Add("Value", Type.GetType("System.Int32"));
                    foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
                    {
                        if (sh.Varname.Contains("_error") && sh.Length == 1)
                        {
                            // read code
                            byte[] data = _ecuConnection.ReadSymbolData(sh.Varname, (uint)m_trionicFileInformation.GetSymbolAddressSRAM(sh.Varname), (uint)m_trionicFileInformation.GetSymbolLength(sh.Varname));
                            if (data.Length == 1)
                            {
                                Int32 _counter = Convert.ToInt32(data.GetValue(0));
                                if (_counter > 0)
                                {
                                    // add it to the list
                                    dt.Rows.Add(sh.Varname, _counter);
                                }
                            }
                        }
                    }
                    codes.SetDataSet(dt);
                }
            }
            // and refill the control
        }

        private void btnSaveAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.CheckFileExists = false;
                    sfd.CheckPathExists = true;
                    sfd.Filter = "Binary file|*.bin|Motorola S record format|*.S19";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (!m_trionicFile.ValidateChecksum()) m_trionicFile.UpdateChecksum();
                        if (sfd.FileName.ToUpper().EndsWith("S19"))
                        {
                            srec2bin convert = new srec2bin();
                            if (!convert.ConvertBinToSrec(m_trionicFileInformation.Filename, sfd.FileName))
                            {
                                frmInfoBox info = new frmInfoBox("Failed to convert file to S19 format");
                            }
                        }
                        else
                        {
                            if (m_trionicFileInformation.Filename != sfd.FileName)
                            {
                                File.Copy(m_trionicFileInformation.Filename, sfd.FileName, true);
                            }

                        }
                    }
                }
            }
        }

        private void addToRealtimeUserMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string symbolname = string.Empty;
            string descr = string.Empty;
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    Trionic5Tools.SymbolHelper dr = (Trionic5Tools.SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    if (dr.Varname != "")
                    {
                        symbolname = dr.Varname;
                        descr = dr.Helptext;
                        ctrlRealtime1.AddToRealtimeUserMaps(symbolname, descr);
                        
                    }
                }
            }
        }

        private void btnToggleAutoTune_ItemClick(object sender, ItemClickEventArgs e)
        {
            // if autotune running, switch off
            ctrlRealtime1.ToggleAutoTune();
        }

        private void ctrlRealtime1_onAutoTuneStateChanged(object sender, ctrlRealtime.AutoTuneEventArgs e)
        {
            btnToggleAutoTune.Enabled = e.Ready;
        }

        private void SetupLogFilters()
        {
            // setup the export filters
            LogFilters filterhelper = new LogFilters();
            frmLogFilters frmfilters = new frmLogFilters();
            LogFilterCollection filters = filterhelper.GetFiltersFromRegistry();
            frmfilters.SetFilters(filters);
            Trionic5Tools.SymbolCollection sc = new Trionic5Tools.SymbolCollection();
            foreach (Trionic5Tools.SymbolHelper sh in m_trionicFileInformation.SymbolCollection)
            {
                if (!sh.Varname.Contains("!")) sc.Add(sh);
            }
            frmfilters.SetSymbols(sc);
            if (frmfilters.ShowDialog() == DialogResult.OK)
            {
                filterhelper.SaveFiltersToRegistry(frmfilters.GetFilters());
            }
        }

        private void btnSetupLogFiltering_ItemClick(object sender, ItemClickEventArgs e)
        {
            SetupLogFilters();
        }

        private void btnScanForBinaries_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the user library screen
            frmUserLibrary library = new frmUserLibrary();
            if (m_trionicFile != null)
            {
                library.EnableCompareButton();
            }
            library.ShowDialog();
            if (library.Open_File != "")
            {
                // open the file
                OpenWorkingFile(library.Open_File);
            }
            else if (library.Compare_File != "")
            {
                CompareToFile(library.Compare_File);
                // compare to this file
            }
        }

        private void btnIdleTargetAFRMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            // als er nog geen aanwezig is, een nieuwe aanmaken
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            m_AFRMaps.CheckIdleTargetAFRMap();
            ShowIdleAfrMAP("IdleTargetAFR", m_AFRMaps.LoadIdleTargetAFRMapInBytes());
        }

        private void btnIdleAFRFeedbackMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show feedback afr map if available
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            ShowIdleAfrMAP("IdleFeedbackAFR", m_AFRMaps.GetIdleFeedbackAFRMapinBytes());
        }

        private void btnIdleAFRErrorMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_AFRMaps == null)
            {
                m_AFRMaps = new AFRMaps();
                m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                m_AFRMaps.TrionicFile = m_trionicFile;
                m_AFRMaps.InitializeMaps();
            }
            ShowIdleAfrMAP("IdleFeedbackvsTargetAFR", m_AFRMaps.GetIdleDifferenceMapinBytes());
        }

        private void btnShowCompressorMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            // add a new dock with a compressor map control
            //props.TurboType
            if (m_trionicFile != null)
            {
                MapSensorType mst = m_trionicFile.GetMapSensorType(true);
                if (m_trionicFile.Exists())
                {
                    string mapName = m_trionicFileInformation.GetBoostRequestMap();
                    if (props.AutomaticTransmission)
                    {
                        mapName = m_trionicFileInformation.GetBoostRequestMapAUT();
                    }
                    int cols = 8;
                    int rows = 16;
                    m_trionicFile.GetMapMatrixWitdhByName(mapName, out cols, out rows);
                    if (rows != 16)
                    {
                        return;
                    }
                    dockManager1.BeginUpdate();
                    DockPanel dp = dockManager1.AddPanel(DockingStyle.Left);
                    dp.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                    ctrlCompressorMapEx cm = new ctrlCompressorMapEx();
                    cm.onRefreshData += new ctrlCompressorMapEx.RefreshData(cm_onRefreshData);
                    cm.Dock = DockStyle.Fill;
                    // set boost map, rpm range and turbo type
                    double[] boost_req = new double[16];
                    byte[] tryck_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(mapName), (uint)m_trionicFileInformation.GetSymbolLength(mapName));
                    // now get the doubles from it
                    for (int i = 0; i < 16; i++)
                    {
                        double val = Convert.ToDouble(tryck_mat[i * 8 + 7]);
                        if (mst == MapSensorType.MapSensor30)
                        {
                            val *= 1.2;
                        }
                        else if (mst == MapSensorType.MapSensor35)
                        {
                            val *= 1.4;
                        }
                        else if (mst == MapSensorType.MapSensor40)
                        {
                            val *= 1.6;
                        }
                        else if (mst == MapSensorType.MapSensor50)
                        {
                            val *= 2.0;
                        }
                        val /= 100;
                        val -= 1;

                        boost_req.SetValue(val, i);
                    }

                    cm.Boost_request = boost_req;
                    // set rpm range
                    cm.Rpm_points = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, mapName);
                    PartNumberConverter pnc = new PartNumberConverter();
                    ECUInformation ecuinfo = pnc.GetECUInfo(props.Partnumber, props.Enginetype);
                    if (ecuinfo.Is2point3liter) cm.Current_engineType = ctrlCompressorMapEx.EngineType.Liter23;
                    else cm.Current_engineType = ctrlCompressorMapEx.EngineType.Liter2; 

                    switch (props.TurboType)
                    {
                        case TurboType.GT28BB:
                        case TurboType.GT28RS:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.GT28RS);
                            break;
                        case TurboType.Stock:
                            // if aero?
                            if (ecuinfo.Isaero)
                            {
                                cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.TD04);
                            }
                            else
                            {
                                if (ecuinfo.Carmodel == CarModel.Saab900 || ecuinfo.Carmodel == CarModel.Saab900SE || ecuinfo.Carmodel == CarModel.Saab93)
                                {
                                    cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.T25_NG900);
                                }
                                else
                                {
                                    cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.T25_60);
                                }
                            }
                            break;
                        case TurboType.TD0415T:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.TD04);
                            break;
                        case TurboType.GT3071R:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.GT3071R86);
                            break;
                        case TurboType.HX40w:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.HX40W);
                            break;
                        case TurboType.TD0419T:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.TD0419T);
                            break;
                        case TurboType.S400SX371:
                            cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.S400SX371);
                            break;
                        default:
                            if (ecuinfo.Carmodel == CarModel.Saab900 || ecuinfo.Carmodel == CarModel.Saab900SE || ecuinfo.Carmodel == CarModel.Saab93)
                            {
                                cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.T25_NG900);
                            }
                            else
                            {
                                cm.SetCompressorType(Trionic5Controls.ctrlCompressorMapEx.CompressorMap.T25_60);
                            }
                            break;
                            
                    }
                        
                    dp.Width = 600;
                    dp.Text = "Compressor map plotter";
                    dp.Controls.Add(cm);
                    dockManager1.EndUpdate();
                }
            }
        }

        void cm_onRefreshData(object sender, EventArgs e)
        {
            if (sender is ctrlCompressorMapEx)
            {
                ctrlCompressorMapEx cm = (ctrlCompressorMapEx)sender;
                string mapName = m_trionicFileInformation.GetBoostRequestMap();
                if (props.AutomaticTransmission)
                {
                    mapName = m_trionicFileInformation.GetBoostRequestMapAUT();
                }
                double[] boost_req = new double[16];
                byte[] tryck_mat = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(mapName), (uint)m_trionicFileInformation.GetSymbolLength(mapName));
                // now get the doubles from it
                for (int i = 0; i < 16; i++)
                {
                    double val = Convert.ToDouble(tryck_mat[i * 8 + 7]);
                    if (props.MapSensorType == MapSensorType.MapSensor30)
                    {
                        val *= 1.2;
                    }
                    else if (props.MapSensorType == MapSensorType.MapSensor35)
                    {
                        val *= 1.4;
                    }
                    else if (props.MapSensorType == MapSensorType.MapSensor40)
                    {
                        val *= 1.6;
                    }
                    else if (props.MapSensorType == MapSensorType.MapSensor50)
                    {
                        val *= 2.0;
                    }
                    val /= 100;
                    val -= 1;

                    boost_req.SetValue(val, i);
                }

                cm.Boost_request = boost_req;
                // set rpm range
                cm.Rpm_points = m_trionicFile.GetYaxisValues(m_trionicFileInformation.Filename, mapName);
                cm.Redraw();
            }
        }

        private void btnShowKnockCounterMaps_ItemClick(object sender, ItemClickEventArgs e)
        {
            string folder = string.Empty;
            if (m_CurrentWorkingProject != "")
            {
                folder = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots";
            }
            else
            {
                folder = Path.Combine(Path.GetDirectoryName(m_trionicFileInformation.Filename), "Snapshots");
            }
            if (Directory.Exists(folder))
            {
                string[] knockmaps = Directory.GetFiles(folder, "*.KNK");
                KnockMapInfoCollection kmic = new KnockMapInfoCollection();
                foreach (string knockmap in knockmaps)
                {
                    KnockMapInfo kmi = new KnockMapInfo();
                    kmi.FileName = knockmap;
                    kmi.FileNameNoPath = Path.GetFileName(knockmap);
                    string _content = File.ReadAllText(knockmap);
                    if (_content.Length == 1152)
                    {
                        kmi.Content = _content;
                        FileInfo fi = new FileInfo(knockmap);
                        kmi.FileDateTime = fi.LastWriteTime;
                        int[] knockCounters = AnalyzeKnockString(_content);
                        int totalKnocks = 0;
                        foreach (int kc in knockCounters)
                        {
                            totalKnocks += kc;
                        }
                        kmi.NumberOfKnocks = totalKnocks;
                        kmic.Add(kmi);// add to collection
                    }
                }
                frmKnockCounterMapSelect mapSelect = new frmKnockCounterMapSelect();
                mapSelect.SetDataSource(kmic);
                DialogResult dr = mapSelect.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string fileToShow = mapSelect.GetKnockMapFilename();
                    if (fileToShow != string.Empty)
                    {
                        if (File.Exists(fileToShow))
                        {
                            ShowKnockMap(fileToShow, AnalyzeKnockStringToBytes(File.ReadAllText(fileToShow)), string.Empty, null);
                        }
                    }

                }
                else if (dr == DialogResult.Yes)
                {
                    // compare two knock maps
                    string fileToShow = mapSelect.GetKnockMapFilename();
                    string fileToCompare = mapSelect.GetKnockMapFilenameCompare();
                    if (fileToShow != string.Empty && fileToCompare != string.Empty)
                    {
                        if (File.Exists(fileToShow) && File.Exists(fileToCompare))
                        {
                            ShowKnockMap(fileToShow, AnalyzeKnockStringToBytes(File.ReadAllText(fileToShow)), fileToCompare, AnalyzeKnockStringToBytes(File.ReadAllText(fileToCompare)));
                        }
                    }
                }
            }
            else
            {
                btnShowKnockCounterMaps.Enabled = false;
            }
        }

        private int[] AnalyzeKnockString(string _content)
        {
            int[] retval = new int[_content.Length / 4];
            for (int i = 0; i < retval.Length; i++)
            {
                int value = 0;
                byte b1 = Convert.ToByte(_content.Substring(i * 4, 2), 16);
                byte b2 = Convert.ToByte(_content.Substring(i * 4 + 2, 2), 16);
                value = Convert.ToInt32(b1) * 256 + Convert.ToInt32(b2);
                retval.SetValue(value, i);
            }
            return retval;
        }

        private byte[] AnalyzeKnockStringToBytes(string _content)
        {
            byte[] retval = new byte[_content.Length / 2];
            for (int i = 0; i < retval.Length; i++)
            {
                byte b1 = Convert.ToByte(_content.Substring(i * 2, 2), 16);
                retval.SetValue(b1, i);
            }
            return retval;
        }


        /// <summary>
        /// shows a mapviewer with the data from a knock counter snapshot
        /// </summary>
        /// <param name="fileToShow"></param>
        private void ShowKnockMap(string fileToShow, byte[] _data, string fileToCompare, byte[] _dataToCompare)
        {
            // show seperate mapviewer for AFR target map
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            string symbolname = "Knock_count_map";
            
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    }
                    
                    if (fileToCompare != string.Empty) tabdet.IsCompareViewer = true;

                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    //TryTpShowTouchScreenInput();

                    tabdet.DirectSRAMWriteOnSymbolChange = false;
                    //tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Visible = false;
                    tabdet.Filename = m_trionicFile.GetFileInfo().Filename;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = "Knock counter snapshot";
                    tabdet.Map_cat = XDFCategories.Sensor;
                    tabdet.X_axisvalues = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetIgnitionMap());
                    tabdet.Y_axisvalues = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetIgnitionMap());
                    // z, y and z axis to do

                    //<GS-24032010> dock it anyway, so it will fit the screen

                    System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));

                    //dockPanel = dockManager1.AddPanel(floatpoint);
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Width = 800; // TEST

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
                    else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                    {
                        int dw = 550;
                        if (tabdet.X_axisvalues.Length > 0)
                        {
                            dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
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
                        dockPanel.Width = dw;
                    }
                    floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                    while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                    dockPanel.FloatLocation = floatpoint;

                    dockPanel.Tag = m_trionicFile.GetFileInfo().Filename;

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;

                    m_trionicFile.GetMapAxisDescriptions(m_trionicFileInformation.GetIgnitionMap(), out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;
                    int columns = 8;
                    int rows = 8;
                    m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetIgnitionMap(), out columns, out rows);

                    tabdet.Map_address = 0;
                    tabdet.Map_sramaddress = 0;
                    int length = m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap());
                    tabdet.Map_length = length;
                    byte[] mapdata = _data;// m_AFRMaps.LoadTargetAFRMapInBytes(filename);//TODO: ???
                    tabdet.Map_content = mapdata;

                    if (fileToCompare != string.Empty)
                    {
                        tabdet.Map_original_content = _data;
                        tabdet.Map_compare_content = _dataToCompare;
                        if (mapdata.Length == _dataToCompare.Length)
                        {

                            if (m_trionicFile.IsTableSixteenBits(symbolname))
                            {
                                for (int bt = 0; bt < _dataToCompare.Length; bt += 2)
                                {
                                    int value1 = Convert.ToInt16(mapdata.GetValue(bt)) * 256 + Convert.ToInt16(mapdata.GetValue(bt + 1));
                                    int value2 = Convert.ToInt16(_dataToCompare.GetValue(bt)) * 256 + Convert.ToInt16(_dataToCompare.GetValue(bt + 1));

                                    value1 = (int)Math.Abs(value1 - value2);
                                    //value1 = (int)(value1 - value2);
                                    byte v1 = (byte)(value1 / 256);
                                    byte v2 = (byte)(value1 - (int)v1 * 256);
                                    mapdata.SetValue(v1, bt);
                                    mapdata.SetValue(v2, bt + 1);
                                }
                            }
                            else
                            {
                                for (int bt = 0; bt < _dataToCompare.Length; bt++)
                                {
                                    //logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)_dataToCompare.GetValue(bt))), bt);
                                    //mapdata.SetValue((byte)(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }
                        }
                    }
                    

                    tabdet.Correction_factor = 1;
                    tabdet.Correction_offset = 0;
                    tabdet.IsUpsideDown = true;

                    tabdet.ShowTable(columns, true);

                    tabdet.Dock = DockStyle.Fill;
                    //tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(onTargetAFRMapSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                    tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);

                    //tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(onAFRRefresh);
                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]";
                    bool isDocked = false;
                    if (!isDocked)
                    {
                        int width = 600;
                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                        {
                            width = 600;
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

                            }
                            else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                }
                                if (width < 400) width = 400;

                            }
                        }
                        dockPanel.Width = width;
                    }
                    if (dockPanel.Height < 700) tabdet.GraphVisible = false; //<GS-24032010>

                    dockPanel.Controls.Add(tabdet);
                    TryToAddOpenLoopTables(tabdet);
                    tabdet.Visible = true;
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();

            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void ctrlRealtime1_onSwitchIgnitionTuningOnOff(object sender, ctrlRealtime.ClosedLoopOnOffEventArgs e)
        {
            // toggle closed loop on /off based on e.SwitchOn
            //Fastest way to detect cell stability and such is in ECUConnection but we don't want that logic in there.
            //Design decision is to feed the AFRMaps object with the data and have that figure out what to do.
            // DO NOT support T5.2 anymore, because of lack of proper knock detection
            if (!props.IsTrionic55)
            {
                frmInfoBox info = new frmInfoBox("T5.2 is currently not supported for Autotuning Ignition");
                return;
            }

            _ecuConnection.StopECUMonitoring();
            Thread.Sleep(10);
            if (m_IgnitionMaps == null)
            {
                m_IgnitionMaps = new IgnitionMaps();
                m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                m_IgnitionMaps.TrionicFile = m_trionicFile;
                m_IgnitionMaps.InitializeMaps();
            }

            byte[] pgm_mod = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetProgramModeSymbol(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetProgramModeSymbol()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetProgramModeSymbol()));
            Thread.Sleep(50);
            // we have to check whether knock detection is turned on, otherwise we DO NOT start ignition autotune

            if (pgm_mod.Length > 3)
            {
                if (!e.SwitchOn)
                {
                    Thread.Sleep(100);
                    // user ended an ignition tuning session.. what todo ?
                    //TODO: ask the user whether he wants to merge the altered fuelmap into ECU memory!
                    // if he replies NO: revert to the previous ignition map (we still need to preserve a copy!)
                    if (MessageBox.Show("Keep adjusted ignition map?", "Question", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        _ecuConnection.WriteSymbolDataForced((int)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionMap()), (int)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap()), IntArrayToByteArray(m_IgnitionMaps.GetOriginalIgnitionmap()));
                    }
                    else
                    {
                        // user selected to keep this map, sync ignition map to the opened binary file! <GS-11042011>
                        byte[] bIgnMap = IntArrayToByteArray(m_IgnitionMaps.GetCurrentlyMutatedIgnitionMap());
                        m_trionicFile.WriteDataNoCounterIncrease(bIgnMap, (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIgnitionMap()));
                    }
                    // init the ignitionmaps values
                    m_IgnitionMaps.InitAutoTuneVars(e.SwitchOn);
                    ctrlRealtime1.SetAutoTuneIgnitionButtonText("Autotune ignition");
                    SetStatusText("Idle");
                }
                else
                {

                    // init the afrmaps values
                    SetStatusText("Starting ignition autotune...");
                    System.Windows.Forms.Application.DoEvents();
                    m_IgnitionMaps.InitAutoTuneVars(e.SwitchOn);
                    byte[] ignitionmap;
                    ignitionmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetIgnitionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap()));
                    Thread.Sleep(50);

                    // TODO: Check whether there are cells that have more advance than the setting allows
                    if (m_appSettings.CapIgnitionMap) // <GS-05042011> added new option to cap off ignition map before commencing autotune
                    {
                        bool _writeMapToECU = false;
                        for (int i = 0; i < ignitionmap.Length; i += 2)
                        {
                            // get the value from the map
                            int advance = Convert.ToInt32(ignitionmap[i]) * 256 + Convert.ToInt32(ignitionmap[i + 1]);
                            if (advance > 32000) advance = -(65535 - advance);
                            //check against m_appSettings.GlobalMaximumIgnitionAdvance
                            if (advance > m_appSettings.GlobalMaximumIgnitionAdvance * 10)
                            {
                                advance = Convert.ToInt32(m_appSettings.GlobalMaximumIgnitionAdvance * 10);
                                // write into the map and indicate we have to update the map in the ECU
                                byte b1 = (byte)(advance / 256);
                                byte b2 = (byte)(advance - (int)(advance * 256));
                                ignitionmap[i] = b1;
                                ignitionmap[i + 1] = b2;
                                _writeMapToECU = true;
                            }
                        }
                        if (_writeMapToECU)
                        {
                            _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionMap()), m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap()), ignitionmap);
                            Thread.Sleep(50);
                        }
                    }

                    byte[] knockpressuremap;
                    if (props.IsTrionic55)
                    {
                        knockpressuremap = _ecuConnection.ReadSymbolData("Knock_press_tab!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Knock_press_tab!"), (uint)m_trionicFileInformation.GetSymbolLength("Knock_press_tab!"));
                    }
                    else
                    {
                        byte[] knock_press_value = _ecuConnection.ReadSymbolData("Knock_press!", (uint)m_trionicFileInformation.GetSymbolAddressSRAM("Knock_press!"), (uint)m_trionicFileInformation.GetSymbolLength("Knock_press!"));
                        knockpressuremap = new byte[32];
                        for (int i = 0; i < knockpressuremap.Length; i += 2)
                        {
                            knockpressuremap[i] = 0;
                            knockpressuremap[i + 1] = knock_press_value[0];
                        }
                    }
                    Thread.Sleep(50);
                    m_IgnitionMaps.SetKnockPressTab(ByteArrayToIntArray(knockpressuremap));
                    m_IgnitionMaps.SetOriginalIgnitionMap(ByteArrayToIntArray(ignitionmap)); // for reverting back if the user chooses so after the session
                    m_IgnitionMaps.SetCurrentIgnitionMap(ByteArrayToIntArray(ignitionmap)); // for editing & display by autotune functions
                    ctrlRealtime1.SetAutoTuneIgnitionButtonText("Tuning...");
                    SetStatusText("Autotune ignition running...");
                }
                m_IgnitionMaps.CellStableTime_ms = m_appSettings.IgnitionCellStableTime_ms;
                m_IgnitionMaps.MinimumEngineSpeedForIgnitionTuning = m_appSettings.MinimumEngineSpeedForIgnitionTuning;
                m_IgnitionMaps.MaxumimIgnitionAdvancePerSession = m_appSettings.MaximumIgnitionAdvancePerSession;
                m_IgnitionMaps.IgnitionAdvancePerCycle = m_appSettings.IgnitionAdvancePerCycle;
                m_IgnitionMaps.IgnitionRetardFirstKnock = m_appSettings.IgnitionRetardFirstKnock;
                m_IgnitionMaps.IgnitionRetardFurtherKnocks = m_appSettings.IgnitionRetardFurtherKnocks;
                m_IgnitionMaps.GlobalMaximumIgnitionAdvance = m_appSettings.GlobalMaximumIgnitionAdvance;
                m_IgnitionMaps.IsAutoMappingActive = e.SwitchOn; 
            }
            else
            {
                // could not read pgm_mod...wtf?
            }
            _ecuConnection.StartECUMonitoring();

        }

        private int[] ByteArrayToIntArray(byte[] ignitionmap)
        {
            // convert byte values from bin/ecu to int array for ignition advance map 
            int[] retval = new int[ignitionmap.Length / 2];
            int j = 0;
            for (int i = 0; i < ignitionmap.Length; i += 2)
            {
                int ignvalue = Convert.ToInt32(ignitionmap[i]) * 256;
                ignvalue += Convert.ToInt32(ignitionmap[i + 1]);
                retval.SetValue(ignvalue, j++);
            }
            return retval;

        }

        private byte[] IntArrayToByteArray(int[] ignmap)
        {
            byte[] retval = new byte[ignmap.Length * 2];
            int j = 0;
            for (int i = 0; i < ignmap.Length; i++)
            {
                int curr_value = ignmap[i];
                byte b1 = (byte)(curr_value / 256);
                byte b2 = (byte)(curr_value - (double)(b1 * 256));
                retval.SetValue(b1, j++);
                retval.SetValue(b2, j++);
            }
            return retval;
        }

        void m_IgnitionMaps_onCellLocked(object sender, IgnitionMaps.IgnitionmapChangedEventArgs e)
        {
            // a cell is locked... 
            // set locked in the lock-ignition map
            
            if (sndplayer != null)
            {
                if (m_appSettings.PlayCellProcessedSound)
                {
                    string sound2play = Application.StartupPath + "\\ping.wav";
                    if (File.Exists(sound2play))
                    {
                        sndplayer.SoundLocation = sound2play;
                        sndplayer.Play();
                    }
                }
            }
            
        }

        void m_IgnitionMaps_onIgnitionmapCellChanged(object sender, IgnitionMaps.IgnitionmapChangedEventArgs e)
        {
            // a value in the main ignition map was "autotuned"
            // seems that we need to adjust a value in the current fuelmap
            if (_ecuConnection.Opened)
            {
                if (m_IgnitionMaps.IsAutoMappingActive)
                {
                    // then go!
                    
                    byte[] write = new byte[2];

                    byte b1 = (byte)(e.Cellvalue / 256);
                    byte b2 = (byte)(e.Cellvalue - (double)(b1 * 256));

                    write[0] = b1;
                    write[1] = b2;
                    //logger.Debug("Updating cellindex: " + e.Mapindex.ToString());
                    _ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetIgnitionMap()) + e.Mapindex * 2, 2, write);
                }


            }
        }

        private void btnClearIgnitionLockedCells_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClearIgnitionLockMap();
        }

        private void ClearIgnitionLockMap()
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    if (m_IgnitionMaps == null)
                    {
                        m_IgnitionMaps = new IgnitionMaps();
                        m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                        m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                        m_IgnitionMaps.TrionicFile = m_trionicFile;
                        m_IgnitionMaps.InitializeMaps();
                    }
                    m_IgnitionMaps.ClearIgnitionLockedMap();
                }
            }
        }

        private void btnIgnitionLockedMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    if (m_IgnitionMaps == null)
                    {
                        m_IgnitionMaps = new IgnitionMaps();
                        m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                        m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                        m_IgnitionMaps.TrionicFile = m_trionicFile;
                        m_IgnitionMaps.InitializeMaps();
                    }
                    StartTableViewer("Ign_map_0!"); // adds lock map automatically.

                    //ShowIgnitionMAP("IgnitionLockMap", m_IgnitionMaps.GetIgnitionLockedMap());
                }
            }
            
        }

        private void ShowIgnitionMAP(string mapname, int[] lockmap)
        {
            // show seperate mapviewer for AFR target map
            byte[] _data = new byte[lockmap.Length * 2];
            _data = IntArrayToByteArray(lockmap);
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            bool pnlfound = false;
            string symbolname = mapname;
            try
            {
                foreach (DevExpress.XtraBars.Docking.DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text == "Symbol: " + symbolname + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]")
                    {
                        dockPanel = pnl;
                        pnlfound = true;
                        dockPanel.Show();
                        // nog data verversen?

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (!pnlfound)
            {
                dockManager1.BeginUpdate();
                try
                {
                    IMapViewer tabdet;
                    if (m_appSettings.MapViewerType == MapviewerType.Fancy)
                    {
                        tabdet = new MapViewerEx();
                    }
                    else if (m_appSettings.MapViewerType == MapviewerType.Normal)
                    {
                        tabdet = new MapViewer();
                    }
                    else
                    {
                        tabdet = new SimpleMapViewer();
                    }

                    tabdet.AutoUpdateChecksum = m_appSettings.AutoChecksum;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    //TryTpShowTouchScreenInput();

                    tabdet.DirectSRAMWriteOnSymbolChange = false;
                    //tabdet.SetViewSize(m_appSettings.DefaultViewSize);
                    tabdet.Visible = false;
                    tabdet.Filename = m_trionicFile.GetFileInfo().Filename;
                    tabdet.GraphVisible = m_appSettings.ShowGraphs;
                    if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor25)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor30)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal3Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy3Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor35)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal35Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy35Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor40)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal4Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy4Bar;
                        }
                    }
                    else if (m_trionicFile.GetMapSensorType(m_appSettings.AutoDetectMapsensorType) == MapSensorType.MapSensor50)
                    {
                        if (m_appSettings.DefaultViewType == ViewType.Decimal)
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Decimal5Bar;
                        }
                        else
                        {
                            tabdet.Viewtype = Trionic5Tools.ViewType.Easy5Bar;
                        }
                    }
                    tabdet.DisableColors = m_appSettings.DisableMapviewerColors;
                    tabdet.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                    tabdet.IsRedWhite = m_appSettings.ShowRedWhite;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = "Ignition locked map";
                    tabdet.Map_cat = XDFCategories.Sensor;
                    tabdet.X_axisvalues = m_trionicFile.GetMapXaxisValues(m_trionicFileInformation.GetIgnitionMap());
                    tabdet.Y_axisvalues = m_trionicFile.GetMapYaxisValues(m_trionicFileInformation.GetIgnitionMap());
                    System.Drawing.Point floatpoint = this.PointToClient(new System.Drawing.Point(dockSymbols.Location.X + dockSymbols.Width + 30, dockSymbols.Location.Y + 30));
                    dockPanel = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    dockPanel.Width = 800; // TEST

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
                    else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                    {
                        int dw = 550;
                        if (tabdet.X_axisvalues.Length > 0)
                        {
                            dw = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
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
                        dockPanel.Width = dw;
                    }
                    floatpoint = new System.Drawing.Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 2 - dockPanel.FloatSize.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2 - dockPanel.FloatSize.Height / 2);
                    while ((floatpoint.X < (dockSymbols.Width + 20)) && ((floatpoint.X + dockSymbols.Width) < Screen.PrimaryScreen.WorkingArea.Size.Width)) floatpoint.X++;
                    dockPanel.FloatLocation = floatpoint;

                    dockPanel.Tag = m_trionicFile.GetFileInfo().Filename;

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;

                    m_trionicFile.GetMapAxisDescriptions(m_trionicFileInformation.GetIgnitionMap(), out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;
                    int columns = 18;
                    int rows = 16;
                    //m_trionicFile.GetMapMatrixWitdhByName(m_trionicFileInformation.GetIgnitionMap(), out columns, out rows);

                    tabdet.Map_address = 0;
                    tabdet.Map_sramaddress = 0;
                    int length = m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap());
                    tabdet.Map_length = length ;
                    byte[] mapdata = _data;// m_AFRMaps.LoadTargetAFRMapInBytes(filename);//TODO: ???
                    tabdet.Map_content = mapdata;

                    tabdet.Correction_factor = 1;
                    tabdet.Correction_offset = 0;
                    tabdet.IsUpsideDown = true;
                    tabdet.ShowTable(columns, true);
                    tabdet.Dock = DockStyle.Fill;
                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(ignitionMapClear);
                    //tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(onTargetAFRMapSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(OnCloseMapViewer);
                    tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(ignitionMapRefresh);
                    tabdet.onCellLocked += new IMapViewer.CellLocked(mv_onCellLocked);


                    //tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(onIgnitionLockRefresh);
                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_trionicFile.GetFileInfo().Filename) + "]";
                    bool isDocked = false;
                    if (!isDocked)
                    {
                        int width = 600;
                        if (m_appSettings.DefaultViewSize == ViewSize.NormalView)
                        {
                            width = 600;
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

                            }
                            else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView || m_appSettings.DefaultViewSize == ViewSize.TouchscreenView)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                                }
                                if (width < 400) width = 400;

                            }
                        }
                        dockPanel.Width = width;
                    }
                    if (dockPanel.Height < 700) tabdet.GraphVisible = false; //<GS-24032010>

                    dockPanel.Controls.Add(tabdet);
                    TryToAddOpenLoopTables(tabdet);
                    tabdet.Visible = true;
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();

            }
            //UpdateFeedbackMaps();

            System.Windows.Forms.Application.DoEvents();
        }

        void ignitionMapRefresh(object sender, IMapViewer.ReadFromSRAMEventArgs e)
        {
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                if (m_trionicFile != null)
                {
                    if (m_trionicFile.Exists())
                    {
                        if (m_IgnitionMaps == null)
                        {
                            m_IgnitionMaps = new IgnitionMaps();
                            m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                            m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                            m_IgnitionMaps.TrionicFile = m_trionicFile;
                            m_IgnitionMaps.InitializeMaps();
                        }
                        mv.Map_content = IntArrayToByteArray(m_IgnitionMaps.GetIgnitionLockedMap());
                        mv.ShowTable(mv.X_axisvalues.Length, true);
                    }
                }
                
            }
        }

        void ignitionMapClear(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            if (sender is IMapViewer)
            {
                IMapViewer tabdet = (IMapViewer)sender;
                if (tabdet.Map_name == "IgnitionLockMap")
                {
                    if (tabdet.ClearData)
                    {
                        ClearIgnitionLockMap(); // and refresh the data in the viewer
                        tabdet.Map_content = IntArrayToByteArray(m_IgnitionMaps.GetIgnitionLockedMap());
                        tabdet.ShowTable(tabdet.X_axisvalues.Length, true);
                    }
                }
            }
        }

        void mv_onCellLocked(object sender, IMapViewer.CellLockedEventArgs e)
        {
            //TODO: if it is a map that can lock cells, do it
            //<GS-29032011>
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                if (mv.Map_name == "Ign_map_0!" || mv.Map_name == "Knock_count_map")
                {
                    // lock/unlock ignition lock stuff
                    if (m_IgnitionMaps == null)
                    {
                        m_IgnitionMaps = new IgnitionMaps();
                        m_IgnitionMaps.onCellLocked += new IgnitionMaps.CellLocked(m_IgnitionMaps_onCellLocked);
                        m_IgnitionMaps.onIgnitionmapCellChanged += new IgnitionMaps.IgnitionmapCellChanged(m_IgnitionMaps_onIgnitionmapCellChanged);
                        m_IgnitionMaps.TrionicFile = m_trionicFile;
                        m_IgnitionMaps.InitializeMaps();
                    }
                    if (e.Locked) m_IgnitionMaps.LockCell(e.Columnindex, e.Rowindex);
                    else m_IgnitionMaps.UnlockCell(e.Columnindex, e.Rowindex);
                    mv.Ignition_lock_map = m_IgnitionMaps.GetIgnitionLockedMap();
                    mv.ShowTable(mv.X_axisvalues.Length, true);
                }
                else if (mv.Map_name == "FeedbackAFR" || mv.Map_name == "FeedbackvsTargetAFR" || mv.Map_name == "TargetAFR" || mv.Map_name == "Insp_mat!" || mv.Map_name == "Inj_map_0!")
                {
                    // lock/unlock AFR lock stuff
                    if (m_AFRMaps == null)
                    {
                        m_AFRMaps = new AFRMaps();
                        m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                        m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                        m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                        m_AFRMaps.TrionicFile = m_trionicFile;
                        m_AFRMaps.InitializeMaps();
                    }
                    if (e.Locked) m_AFRMaps.LockCell(e.Columnindex, e.Rowindex);
                    else m_AFRMaps.UnlockCell(e.Columnindex, e.Rowindex);
                    mv.AFR_lock_map = m_AFRMaps.GetAFRLockedMap();
                    if (mv.Map_name == "FeedbackAFR" || mv.Map_name == "FeedbackvsTargetAFR" || mv.Map_name == "TargetAFR")
                    {
                        mv.ShowTable(mv.X_axisvalues.Length, true);
                    }
                    else
                    {
                        mv.ShowTable(mv.X_axisvalues.Length, false);
                    }
                }
                else if (mv.Map_name == "IdleFeedbackAFR" || mv.Map_name == "IdleFeedbackvsTargetAFR" || mv.Map_name == "IdleTargetAFR" || mv.Map_name == m_trionicFileInformation.GetIdleFuelMap())
                {
                    // lock/unlock AFR lock stuff
                    if (m_AFRMaps == null)
                    {
                        m_AFRMaps = new AFRMaps();
                        m_AFRMaps.onFuelmapCellChanged += new AFRMaps.FuelmapCellChanged(m_AFRMaps_onFuelmapCellChanged);
                        m_AFRMaps.onIdleFuelmapCellChanged += new AFRMaps.IdleFuelmapCellChanged(m_AFRMaps_onIdleFuelmapCellChanged);
                        m_AFRMaps.onCellLocked += new AFRMaps.CellLocked(m_AFRMaps_onCellLocked);
                        m_AFRMaps.TrionicFile = m_trionicFile;
                        m_AFRMaps.InitializeMaps();
                    }
                    if (e.Locked) m_AFRMaps.IdleLockCell(e.Columnindex, e.Rowindex);
                    else m_AFRMaps.IdleUnlockCell(e.Columnindex, e.Rowindex);
                    mv.IdleAFR_lock_map = m_AFRMaps.GetIdleAFRLockedMap();
                    if (mv.Map_name == "IdleFeedbackAFR" || mv.Map_name == "IdleFeedbackvsTargetAFR" || mv.Map_name == "IdleTargetAFR")
                    {
                        mv.ShowTable(mv.X_axisvalues.Length, true);
                    }
                    else
                    {
                        mv.ShowTable(mv.X_axisvalues.Length, false);
                    }
                }
            }
        }

        private void btnReleaseNotes_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartReleaseNotesViewer(m_msiUpdater.GetReleaseNotes(), Application.ProductVersion.ToString());
        }

        private void btnHardcodedRPMLimit_ItemClick(object sender, ItemClickEventArgs e)
        {

            /*bool _writeMapToECU = false;
            byte[] ignitionmap;
            ignitionmap = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIgnitionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetIgnitionMap()));


            for (int i = 0; i < ignitionmap.Length; i += 2)
            {
                // get the value from the map
                int advance = Convert.ToInt32(ignitionmap[i]) * 256 + Convert.ToInt32(ignitionmap[i + 1]);
                if (advance > 32000) advance = -(65535 - advance);
                if (advance > m_appSettings.GlobalMaximumIgnitionAdvance * 10)
                {
                    logger.Debug("We need to cap: " + advance.ToString());
                    advance = Convert.ToInt32(m_appSettings.GlobalMaximumIgnitionAdvance * 10);

                    // write into the map and indicate we have to update the map in the ECU
                    byte b1 = (byte)(advance / 256);
                    byte b2 = (byte)(advance - (int)(advance * 256));
                    ignitionmap[i] = b1;
                    ignitionmap[i + 1] = b2;
                    _writeMapToECU = true;
                }
            }
            if (_writeMapToECU)
            {
                // test dump to console
                m_trionicFile.WriteData(ignitionmap, (uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetIgnitionMap()));
            }*/

            // get all files from the library, open them and check hardcoded RPM limit detection algorithm
            /*string libPath = Application.StartupPath + "\\Binaries";
            
            if (Directory.Exists(libPath))
            {
                string[] filesinLibrary = Directory.GetFiles(libPath, "*.bin");
                foreach (string libFile in filesinLibrary)
                {
                    OpenWorkingFile(libFile);
                    // get hard & soft rpm limit
                    int hardRPMLimit = m_trionicFile.GetHardcodedRPMLimit(m_trionicFileInformation.Filename);
                    int hardRPMLimitTwo = m_trionicFile.GetHardcodedRPMLimitTwo(m_trionicFileInformation.Filename);
                    int softRPMLimit = m_trionicFile.GetSymbolAsInt("Rpm_max!") * 10;
                    logger.Debug(Path.GetFileName(libFile) + " softrpm: " + softRPMLimit.ToString() + " hardrpm: " + hardRPMLimit.ToString() + " hardrpm2: " + hardRPMLimitTwo.ToString() + " " + "T55: " + props.IsTrionic55.ToString());
                }
            }*/
            


            
            frmRpmLimiterWizard rpmWiz = new frmRpmLimiterWizard();
            if (m_trionicFile != null)
            {
                if (m_trionicFile.Exists())
                {
                    if (props.IsTrionic55)
                    {
                        int hardRPMLimit = m_trionicFile.GetHardcodedRPMLimit(m_trionicFileInformation.Filename);
                        if (hardRPMLimit > 0)
                        {
                            rpmWiz.HardcodedRPMLimit = hardRPMLimit;
                            rpmWiz.SoftwareRPMLimit = m_trionicFile.GetSymbolAsInt("Rpm_max!") * 10;
                            if (rpmWiz.ShowDialog() == DialogResult.OK)
                            {
                                if (rpmWiz.HardcodedRPMLimit > 7500)
                                {
                                    //TODO: <GS-04042011> We should disable diagnostics services when rpm goes as high as this!
                                }
                                m_trionicFile.SetHardcodedRPMLimit(m_trionicFileInformation.Filename, rpmWiz.HardcodedRPMLimit);
                                if (m_trionicFileInformation.GetSymbolAddressFlash("Rpm_max!") > 0)
                                {
                                    byte[] rpmMax = new byte[2];
                                    int rpmLimit = rpmWiz.SoftwareRPMLimit / 10;
                                    int rpm1 = rpmLimit / 256;
                                    int rpm2 = rpmLimit - (rpm1 * 256);
                                    rpmMax.SetValue(Convert.ToByte(rpm1), 0);
                                    rpmMax.SetValue(Convert.ToByte(rpm2), 1);
                                    m_trionicFile.WriteData(rpmMax, (uint)m_trionicFileInformation.GetSymbolAddressFlash("Rpm_max!"));
                                }

                                m_trionicFile.UpdateChecksum();

                                frmInfoBox info = new frmInfoBox("RPM limiters have been changed");

                            }
                        }
                        else
                        {
                            frmInfoBox info = new frmInfoBox("This file is not supported by this wizard");
                        }
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("Trionic 5.2 files are not supported by this wizard");
                    }
                }
            }
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
                logger.Debug("Failed to create myMaps menu: " + myMapsE.Message);
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

                if (map_name == "targetafr")
                    btnAFRTargetmap_ItemClick(sender, e); // start 'target afr' mapviewer
                else if (map_name == "feedbackafr")
                    btnAFRFeedbackmap_ItemClick(sender, e); // start 'feedback afr' mapviewer
                else if (map_name == "feedbackvstargetafr")
                    btnAFRErrormap_ItemClick(sender, e); // start 'feedback vs target afr' mapviewer
                else
                    StartTableViewer(e.Item.Tag.ToString().Trim());
            }
            catch { }
        }



    }
}