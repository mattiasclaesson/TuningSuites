/*
 * PREBUILD EVENT: "C:\Program Files\RustemSoft\Skater .NET Obfuscator Light Edition\go.bat"
 * DONE: Save and reload realtime list items added by user (like T5Suite 2.0)
 * DONE: make Realtime panel available offline
 * DONE: Option to prevent creation of AFR maps
 * DONE: Extend partnumber lookup screen with MY for partnumbers (from the SPS list)
 * DONE: Project based development
 * DONE: Use Ioff (IgnProt.fi_Offset) as a knock indicator. positive values should indicate green, negative values red.
 * DONE: LogWorks 3.0 in the deployment and ask for installation when LogWorks not detected when clicking export to logworks
 * DONE: Fix bug in import tuning package (not updating checksum)
 * DONE: Store afr maps in a subfolder (AFRMaps)
 * DONE: Full screen view and shortcut key for Realtime Panel.
 * DONE: Option to move symbols up & down in Realtime Panel (Free logging).
 * DONE: Save as snapshot function and button in map view window.(quick save jpg snapshot of mapviewer)
 * DONE: Export to excel not working with open bin
 * DONE: Feed AFR to AFRMaps only when not in fuelcut situation
 * DONE: Import AFR feedback map into BFuelCal.Map -> Merge/correct action
 * DONE: WakeUpCanbus.exe voor T7
 * DONE: realtimeDock maximaliseren in de beschikbare ruimte
 * DONE: temperaturen negatief weergeven  als > 32000
 * DONE: Settings for lambda indicator in frmSettings
 * DONE: Settings for sound playing thresholds and symbols
 * DONE: Settings in Airmass viewer for graph lines on/off
 * DONE: Implement symbolnumbers from decode file / extract method for SRAM access
 * DONE: Priority reading in realtime panel.
 *  - read all high prio values
 *  - read one low prio value (next run, read the next low prio value)
 *  - Optionally work with -delay parameters in the symbols (e.g. read TEng only once every 5 frames, TAir only once every 3 etc)
 * TODO: allow clone symbols 
 * TODO: Make address display decimal or hex, not both (symbol list and compare list)
 * TODO: Meng is reading from the torquenominal map, Mlow is reading from the airtorq map
 * TODO: Status byte (00 or 01 for words/integers, 04 or 05 for bytes) for the SID does not designate the ability to edit the parameter, it means whether the value is a signed or unsigned number. 00/04 = unsigned, 01/05 = signed. I noticed this when I looked at the Badp and Amul values with status byte = 00 (so I could clear them if needed). At first Badp was 5 then 65531 (= -5).
 * TODO: Need to poll status for canusb adapter once every 2 seconds
 * TODO: The non volatile data is stored in a segment from 0x70000.. we need to figure out what is in there!
 * TODO: Take a look at RAM_PLT.PlottPosition (indicates whether internal ECU logging is active?)
 * TODO: Take a look at MAF: MAFProt.Q_AirInlet
 * TODO: Filter options in log viewing (logworks & internal)
 * TODO: Preset color schemes for logviewing
 * TODO: Enable users to build a OPEN BIN from their working binary file. This means including an open bin in the deployment in a safe location
 * TODO: Ignition autotune function as soon as T5Suite 2.0 variant proves to work correctly and stable
 * TODO: Create a dictionary with XMLfiles based on the partnumber ID of files (for files without symboltables)
 * TODO: Create a dictionary with addresses for symboltable and for addresslookuptable per partnumberID. 
 *       This might help in reconstructing future corrupted files (match on addresses is enough, if both addresses match)
 * TODO: Extend the partnumber lookup screen with details for Trionic 7 cars/engines/ECUs      
 * TODO: Auto fill known maps in files with no symboltable (based on other files, mapsize, offset from known tables with known size etc)
 * TODO: Use additional canbus frames to improve framerate (we don't have to request all symbol when on P-bus like rpm, vehicle speed etc)
 * TODO: Tune-me-up should not alter the left three columns in the nominal torquemap
 * TODO: Reverse decode.exe to be able to support x64 systems and maybe extend to support newer decoding routines.
 * BUG: Airmass calculator freaks out if TorqueCal.M_NominalMap does not reach the same number as other maps
 * BUG: Some of the Tipin/Tipout maps are missing axis, They might be also the function of RPM and Xacc like other tip control maps (ngTipCal.m_TipInAut1BMap, EngTipCal.m_TipInAut5OMap etc)
 * 
Length  Type  Contents   Description 
------------------------------------------------------ 
0x09    0x91     ASCII   Ecuid.vehicleidnr 
0x07    0x94     ASCII   Ecuid.ecuhardwversnr 
0x0C    0x95     ASCII   Ecuid.ecusoftwnr 
0x1E    0x97     ASCII   Ecuid.ecusoftwversnr 
0x04    0x9A     ASCII   Ecuid.softwaredate 
0x04    0x9C       HEX   ? variable name table crc (not really sure) 
0x04    0x9B    OFFSET   Symboltable (packed table with symbol names) 
0x04    0xF2       HEX   * F2 checksum 
0x04    0xFB       HEX   Romchecksum.piareachecksum 
0x04    0xFC    OFFSET   Romchecksum.BottomOffFlash 
0x04    0xFD       HEX   ROMChecksumType 
0x04    0xFE    OFFSET   Romchecksum.TopOffFlash 
0x05    0xFA       HEX   Lastmodifiedby 
0x0F    0x92     ASCII   Ecuid.partnralphacode (IMMO) 
0x07    0x93     ASCII   Ecuid.ecuhardwnr 
0x02    0xF8      ?HEX   ? 
0x02    0xF7      ?HEX   ? 
0x02    0xF6      ?HEX   ? 
0x02    0xF5      ?HEX   ? 
0x11    0x90     ASCII   Ecuid.scaletable (VIN) 
0x06    0x99     ASCII   Ecuid.testerserialnr 
0x0D    0x98     ASCII   Ecuid.enginetype 
0x01    0xF9     ASCII   Romchecksum.Error 
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Data.OleDb;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.Skins;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraGrid;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using PSTaskDialog;
using RealtimeGraph;
using CommonSuite;
using TrionicCANLib.API;
using TrionicCANLib.Checksum;
using WidebandSupport;
using NLog;
using T7.Parser;


namespace T7
{
    public delegate void DelegateStartReleaseNotePanel(string filename, string version);

    public delegate void FIOCallback(int value);
    internal delegate void FIOInvokeDelegate();

    public delegate void DelegateUpdateBDMProgress(uint bytes);
    public delegate void DelegateUpdateRealTimeValue(string symbolname, float value);
    public delegate void DelegateUpdateMapViewer(IMapViewer viewer, int tabwidth, bool sixteenbits);

    public delegate void DelegateUpdateStatus(ITrionic.CanInfoEventArgs e);
    public delegate void DelegateProgressStatus(int percentage);
    public delegate void DelegateCanFrame(ITrionic.CanFrameEventArgs e);

    public enum ecu_t : int
    {
        Trionic52,
        Trionic55,
        Trionic5529,
        Trionic7,
        None
    }

    public partial class frmMain : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string m_filename = string.Empty;
        private string m_swversion = string.Empty;
        private frmProgress frmProgressExportLog;
        public static SymbolCollection m_symbols = new SymbolCollection();
        private SuiteRegistry suiteRegistry = new T7SuiteRegistry();
        AppSettings m_appSettings;
        int m_currentfile_size = 0x80000;
        string m_current_softwareversion = "";
        int m_currentSramOffsett = 0;
        string m_currentfile = string.Empty;
        private AFRMap m_AFRMap;
        public DelegateStartReleaseNotePanel m_DelegateStartReleaseNotePanel;

        private bool m_WriteLogMarker = false;

        public DelegateUpdateMapViewer m_DelegateUpdateMapViewer;

        private bool _soundAllowed = true;
        private System.Media.SoundPlayer sndplayer;

        private string m_currentsramfile = string.Empty;
        private AFRViewType AfrViewMode = AFRViewType.AFRMode;
        private Stopwatch _sw = new Stopwatch();
        private EngineStatus _currentEngineStatus = new EngineStatus();
        frmSplash splash;
        private bool m_RealtimeConnectedToECU = false;
        private bool m_enableRealtimeTimer = false;
        msiupdater m_msiUpdater;
        public DelegateUpdateBDMProgress m_DelegateUpdateBDMProgress;
        public DelegateUpdateRealTimeValue m_DelegateUpdateRealTimeValue;

        private ecu_t _globalECUType = ecu_t.None;
        private bool _globalBDMOpened = false;
        private ushort BDMversion = 0;
        private string m_CurrentWorkingProject = string.Empty;
        private TrionicProjectLog m_ProjectLog = new TrionicProjectLog();


        System.Data.DataTable resumeTuning = new System.Data.DataTable();
        private FormWindowState _oldWindowState;
        private System.Drawing.Rectangle _oldDesktopBounds;
        private System.Drawing.Size _oldClientSize;
        private bool _isFullScreenEnabled = false;
        private Microsoft.Office.Interop.Excel.Application xla;
        private string m_commandLineFile = string.Empty;
        private bool m_startFromCommandLine = false;
        System.Data.DataTable m_realtimeAddresses = null;
        private System.Data.DataTable mrudt = new System.Data.DataTable();
        private bool m_prohibitReading = false;
        private frmEditTuningPackage tunpackeditWindow = null;

        readonly Trionic7 trionic7 = new Trionic7();
        public DelegateUpdateStatus m_DelegateUpdateStatus;
        public DelegateProgressStatus m_DelegateProgressStatus;
        public DelegateCanFrame m_DelegateCanFrame;

        private WidebandFactory wbFactory = null;
        private IWidebandReader wbReader = null;

        private SymbolColors symbolColors;

        private string logworksstring = LogWorks.GetLogWorksPathFromRegistry();
        private DirectoryInfo configurationFilesPath = Directory.GetParent(System.Windows.Forms.Application.UserAppDataPath);

        public ChecksumDelegate.ChecksumUpdate m_ShouldUpdateChecksum;

        public frmMain(string[] args)
        {
            m_appSettings = new AppSettings(suiteRegistry);
            symbolColors = new SymbolColors(suiteRegistry);

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            splash = new frmSplash();
            splash.Show();
            System.Windows.Forms.Application.DoEvents();
            InitializeComponent();
            m_AFRMap = new AFRMap();
            m_AFRMap.onCellLocked += m_AFRMap_onCellLocked;
            m_AFRMap.onFuelmapCellChanged += m_AFRMap_onFuelmapCellChanged;

            try
            {
                sndplayer = new System.Media.SoundPlayer();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (args.Length > 0)
            {
                if (args[0].ToUpper().EndsWith(".BIN"))
                {
                    if(File.Exists(args[0]))
                    {
                        m_commandLineFile = args[0];
                        m_startFromCommandLine = true;
                    }
                }
            }

            m_DelegateUpdateRealTimeValue = UpdateRealtimeInformationValue;
            m_DelegateUpdateBDMProgress = ReportBDMProgress;
            m_DelegateUpdateMapViewer = UpdateMapViewer;

            m_DelegateUpdateStatus = updateStatusInBox;
            m_DelegateProgressStatus = SetProgressPercentage;
            m_DelegateCanFrame = trionic7_onCanFrame;
            
            trionic7.onReadProgress += trionicCan_onReadProgress;
            trionic7.onWriteProgress += trionicCan_onWriteProgress;
            trionic7.onCanInfo += trionicCan_onCanInfo;
            trionic7.onCanFrame += trionicCan_onCanFrame;

            m_ShouldUpdateChecksum = ShouldUpdateChecksum;

            try
            {
                // should be done only once!
                this.fio_callback = this.on_fio;
                BdmAdapter_SetFIOCallback(this.fio_callback);
                logger.Debug("BDM adapter callback set!");
                // should be done only once!

            }
            catch (Exception BDMException)
            {
                logger.Debug("BDM init failed: " + BDMException.Message);
            }

            try
            {
                m_DelegateStartReleaseNotePanel = this.StartReleaseNotesViewer;

            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

            SystemFileAssociation.Create(@"SystemFileAssociations\.bin\shell\Edit in T7 Suite\command");
        }

        void trionicCan_onWriteProgress(object sender, ITrionic.WriteProgressEventArgs e)
        {
            UpdateProgressStatus(e.Percentage);
        }

        void trionicCan_onCanInfo(object sender, ITrionic.CanInfoEventArgs e)
        {
            UpdateFlashStatus(e);
        }

        void trionicCan_onReadProgress(object sender, ITrionic.ReadProgressEventArgs e)
        {
            UpdateProgressStatus(e.Percentage);
        }

        void trionicCan_onCanFrame(object sender, ITrionic.CanFrameEventArgs e)
        {
            logger.Debug("Rx frame: 0x" + e.Message.getID().ToString("X4") + " 0x" + e.Message.getData().ToString("X16"));
            UpdateFrame(e);
        }

        void trionic7_onCanInfo(ITrionic.CanInfoEventArgs e)
        {
            // display progress in the statusbar
            //TODO: For testing only
            SetProgress(e.Info);
        }


        void trionic7_onCanFrame(ITrionic.CanFrameEventArgs e)
        {
            //TODO: handle additional information from the canbus
            // the messages have been filtered already
            // only handle this when the realtime stuff is running... otherwise just ignore
            if (m_appSettings.UseAdditionalCanbusFrames)
            {
                ulong _data = e.Message.getData();
                switch (e.Message.getID())
                {
                    case 0x1A0:         //1A0h - Engine information
                        // rpm is 16 bit value (RPM0 and RPM1 which are byte 1 and 2)
                        int _rpm = Convert.ToInt32(e.Message.getCanData(1)) * 256;
                        _rpm += Convert.ToInt32(e.Message.getCanData(2));
                        UpdateRealtimeInformation("ActualIn.n_Engine", _rpm);
                        int _tps = Convert.ToInt32(e.Message.getCanData(5));
                        UpdateRealtimeInformation("Out.X_AccPedal", _tps);
                        break;
                    case 0x280:         //280h - Pedals, reverse gear
                        // reverse = msg.data(1) & 0x02 
                        // clutch = msg.data(2) & 0x08
                        // brake = msg.data(2) & 0x0A
                        // cruise = msg.data(4) & 0x20

                        break;
                    case 0x290:         //290h - Steering wheel and SID buttons
                        /*
                If (msg.data(2) And &H4) = &H4 Then LabelSteeringWheel.Text = "NXT"
                If (msg.data(2) And &H8) = &H8 Then LabelSteeringWheel.Text = "SEEK-"
                If (msg.data(2) And &H10) = &H10 Then LabelSteeringWheel.Text = "SEEK+"
                If (msg.data(2) And &H20) = &H20 Then LabelSteeringWheel.Text = "SRC"
                If (msg.data(2) And &H40) = &H40 Then LabelSteeringWheel.Text = "VOL+"
                If (msg.data(2) And &H80) = &H80 Then LabelSteeringWheel.Text = "VOL-"

                If (msg.data(3) And &H10) = &H10 Then LabelSID.Text = "+"
                If (msg.data(3) And &H40) = &H40 Then LabelSID.Text = "SET"
                If (msg.data(3) And &H80) = &H80 Then LabelSID.Text = "CLR"                         * */
                        break;
                    case 0x2F0:         //2F0h - Vehicle speed
                        break;
                    case 0x320:         //320h - Doors, central locking and seat belts
                        //msg.data(1) & 0x80 = central locking = unlocked
                        //msg.data(1) & 0x40 = Front left door
                        //msg.data(1) & 0x20 = Front right door
                        //msg.data(1) & 0x10 = Back left door
                        //msg.data(1) & 0x08 = Back right door
                        //msg.data(1) & 0x04 = Hatch door

                        break;
                    case 0x370:         //370h - Mileage
                        break;
                    case 0x3A0:         //3A0h - Vehicle speed
                        int _speed = Convert.ToInt32(e.Message.getCanData(3)) * 256;
                        _speed += Convert.ToInt32(e.Message.getCanData(4));
                        float spd = (float)_speed;
                        spd /= 10F;
                        UpdateRealtimeInformation("In.v_Vehicle", spd);
                        break;
                    case 0x3B0:         //3B0h - Head lights
                        int _lightStatus = Convert.ToInt32(e.Message.getCanData(1));
                        if ((_lightStatus & 0x0001) > 0)
                        {
                            _currentEngineStatus.HeadlightsOn = true;
                        }
                        else
                        {
                            _currentEngineStatus.HeadlightsOn = false;
                        }
                        break;
                    case 0x3E0:         //3E0h - Automatic Gearbox
                        break;
                    case 0x410:         //410h - Light dimmer and light sensor
                        break;
                    case 0x430:         //430h - SID beep request (interesting for Knock indicator?)
                        break;
                    case 0x460:         //460h - Engine rpm and speed
                        _rpm = Convert.ToInt32(e.Message.getCanData(1)) * 256;
                        _rpm += Convert.ToInt32(e.Message.getCanData(2));
                        UpdateRealtimeInformation("ActualIn.n_Engine", _rpm);
                        // rpm = msg.data(1) * 255 + msg.data(2)
                        // speed = (msg.data(3) * 255 + msg.data(4)) / 10
                        _speed = Convert.ToInt32(e.Message.getCanData(3)) * 256;
                        _speed += Convert.ToInt32(e.Message.getCanData(4));
                        spd = (float)_speed;
                        spd /= 10F;
                        UpdateRealtimeInformation("In.v_Vehicle", spd);
                        break;
                    case 0x4A0:         //4A0h - Steering wheel, Vehicle Identification Number
                        //msg.data(2) & 0x40 = left signal indicator
                        //msg.data(2) & 0x20 = right signal indicator
                        break;
                    case 0x520:         //520h - ACC, inside temperature
                        // temperature = msg.data(5) - 40                                        
                        break;
                    case 0x530:         //530h - ACC
                        break;
                    case 0x5C0:         //5C0h - Coolant temperature, air pressure
                        break;
                    case 0x630:         //630h - Fuel usage
                        break;
                    case 0x640:         //640h - Mileage
                        break;
                    case 0x7A0:         //7A0h - Outside temperature
                        break;
                    default:
                        break;
                }
            }
        }

        private void updateStatusInBox(ITrionic.CanInfoEventArgs e)
        {
            SetProgress(e.Info);

            if (e.Type == ActivityType.FinishedFlashing || e.Type == ActivityType.FinishedDownloadingFlash)
            {
                SetCANStatus("");
                trionic7.Cleanup();
                logger.Trace("Connection closed");
                SetProgressIdle();

                if (e.Type == ActivityType.FinishedFlashing)
                {
                    frmInfoBox info = new frmInfoBox("Flash sequence done");
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Download done");
                }
            }
        }

        private void UpdateFrame(ITrionic.CanFrameEventArgs e)
        {
            try
            {
                Invoke(m_DelegateCanFrame, e);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        private void UpdateFlashStatus(ITrionic.CanInfoEventArgs e)
        {
            try
            {
                Invoke(m_DelegateUpdateStatus, e);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
        }

        private void UpdateProgressStatus(int percentage)
        {
            try
            {
                Invoke(m_DelegateProgressStatus, percentage);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
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
                if (perc < 100)
                {
                    if (Convert.ToInt32(barProgress.EditValue) != perc)
                    {
                        barProgress.Visibility = BarItemVisibility.Always;
                        barProgress.EditValue = perc;
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                else
                {
                    barProgress.Caption = "Done";
                    barProgress.Visibility = BarItemVisibility.Never;
                    barProgress.EditValue = 0;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
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

        #region File
        private void File_Open_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                m_appSettings.Lastprojectname = "";
                OpenFile(openFileDialog1.FileName, true);
                m_appSettings.LastOpenedType = 0;

            }
        }

        private void File_SaveAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //review all open editors and save the data still pending
            bool _datasaved = false;
            foreach (DockPanel pnl in dockManager1.Panels)
            {
                foreach (Control c in pnl.Controls)
                {
                    if (c is IMapViewer)
                    {
                        IMapViewer vwr = (IMapViewer)c;
                        if (vwr.SaveData()) _datasaved = true;
                    }
                    else if (c is DockPanel)
                    {
                        DockPanel tpnl = (DockPanel)c;
                        foreach (Control c2 in tpnl.Controls)
                        {
                            if (c2 is IMapViewer)
                            {
                                IMapViewer vwr2 = (IMapViewer)c2;
                                if (vwr2.SaveData()) _datasaved = true;
                            }
                        }
                    }
                    else if (c is ControlContainer)
                    {
                        ControlContainer cntr = (ControlContainer)c;
                        foreach (Control c3 in cntr.Controls)
                        {
                            if (c3 is IMapViewer)
                            {
                                IMapViewer vwr3 = (IMapViewer)c3;
                                if (vwr3.SaveData()) _datasaved = true;
                            }
                        }
                    }
                }
            }
            if (_datasaved)
            {
                frmInfoBox info = new frmInfoBox("All pending changes saved to binary");
            }
            else
            {
                frmInfoBox info = new frmInfoBox("Binary was already up to date!");
            }
        }

        private void File_SaveAs_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
                    m_appSettings.Lastprojectname = "";
                    CloseProject();
                    OpenFile(sfd.FileName, true);
                    m_appSettings.LastOpenedType = 0;
                }
            }
        }

        private void File_ExportToS19(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void File_CreateBackupFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_currentfile != string.Empty)
            {
                VerifyChecksum(false);

                if (File.Exists(m_currentfile))
                {
                    if (m_CurrentWorkingProject != "")
                    {
                        if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups");
                        string filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Backups\\" + Path.GetFileNameWithoutExtension(GetBinaryForProject(m_CurrentWorkingProject)) + "-backup-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".BIN";
                        File.Copy(GetBinaryForProject(m_CurrentWorkingProject), filename);
                    }
                    else
                    {
                        File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup", true);
                        frmInfoBox info = new frmInfoBox("Backup created: " + Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup");
                    }
                }
            }
        }

        private void File_ImportXML_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask user to point to XML document
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML documents|*.xml";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalSymbols(ofd.FileName);
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                // and save the data to the repository
                SaveAdditionalSymbols();
                try
                {
                    _softwareIsOpenDetermined = false;
                    IsSoftwareOpen();
                }
                catch (Exception E3)
                {
                    logger.Debug(E3.Message);
                }
            }
        }

        private void File_ImportCSV_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV documents|*.csv";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalCSVSymbols(ofd.FileName);
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Userdescription != "" && sh.Varname == String.Format("Symbolnumber {0}", sh.Symbol_number))
                    {
                        string temp = sh.Varname;
                        sh.Varname = sh.Userdescription;
                        sh.Userdescription = temp;
                    }
                }
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                // and save the data to the repository
                SaveAdditionalSymbols();
                try
                {
                    _softwareIsOpenDetermined = false;
                    IsSoftwareOpen();
                }
                catch (Exception E3)
                {
                    logger.Debug(E3.Message);
                }
            }
        }

        private void File_ImportAS2_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AS2 documents|*.as2";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TryToLoadAdditionalAS2Symbols(ofd.FileName);
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Userdescription != "" && sh.Varname == String.Format("Symbolnumber {0}", sh.Symbol_number))
                    {
                        string temp = sh.Varname;
                        sh.Varname = sh.Userdescription;
                        sh.Userdescription = temp;
                    }
                }
                gridControlSymbols.DataSource = m_symbols;
                SetDefaultFilters();
                gridControlSymbols.RefreshDataSource();
                // and save the data to the repository
                SaveAdditionalSymbols();
                try
                {
                    _softwareIsOpenDetermined = false;
                    IsSoftwareOpen();
                }
                catch (Exception E3)
                {
                    logger.Debug(E3.Message);
                }
            }
        }

        private void File_ImportTuningPackage_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportTuningPackage();
        }

        private void File_EditTuningPackage(object sender, ItemClickEventArgs e)
        {
            if (tunpackeditWindow != null)
            {
                frmInfoBox info = new frmInfoBox("You have another tuning package edit window open, please close that first");
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 packages|*.t7p";
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
                                logger.Debug(E.Message);
                            }
                        }
                    }
                }
                tunpackeditWindow = new frmEditTuningPackage();
                tunpackeditWindow.FormClosed += new FormClosedEventHandler(edit_FormClosed);
                tunpackeditWindow.onMapSelected += new frmEditTuningPackage.MapSelected(edit_onMapSelected);
                tunpackeditWindow.SetFilename(ofd.FileName);
                tunpackeditWindow.SetDataTable(dt);
                tunpackeditWindow.Show();

            }
        }

        private void File_barEditRecent_EditValueChanged(object sender, EventArgs e)
        {
            if (barEditRecent.EditValue != null)
            {
                if (barEditRecent.EditValue != DBNull.Value)
                {
                    CloseProject();
                    m_appSettings.Lastprojectname = "";
                    OpenFile(barEditRecent.EditValue.ToString(), false);
                    m_appSettings.LastOpenedType = 0;
                }
            }
        }

        private void File_barEditRecent_ShowingEditor(object sender, ItemCancelEventArgs e)
        {
            repositoryItemLookUpEdit1.DataSource = mrudt;
        }

        private void File_CompareToOriginal_ItemClick(object sender, ItemClickEventArgs e)
        {
            T7FileHeader t7header = new T7FileHeader();
            t7header.init(m_currentfile, false);
            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Binaries"))
            {
                string[] files = Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\Binaries", t7header.getPartNumber() + ".bin");
                if (files.Length == 1)
                {
                    CompareToFile((string)files.GetValue(0));
                }
            }
        }

        private void TryToLoadAdditionalAS2Symbols(string filename)
        {
            // convert to AS2 file format

            try
            {
                string[] fileContent = File.ReadAllLines(filename);
                int symbolnumber = 0;
                foreach (string line in fileContent)
                {
                    if (line.StartsWith("*"))
                    {
                        symbolnumber++;
                        try
                        {
                            string varname = line.Substring(1);
                            int idxSymTab = 0;
                            foreach (SymbolHelper sh in m_symbols)
                            {
                                if (sh.Length > 0)
                                {
                                    idxSymTab++;
                                }
                                if (idxSymTab == symbolnumber)
                                {
                                    sh.Userdescription = varname;
                                    sh.Description = SymbolTranslator.ToHelpText(sh.Userdescription, m_appSettings.ApplicationLanguage);
                                    sh.createAndUpdateCategory(sh.Userdescription);
                                    break;
                                }
                            }
                        }
                        catch (Exception lineE)
                        {
                            logger.Debug("Failed to import a symbol from AS2 file " + line + ": " + lineE.Message);
                        }

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to import additional AS2 symbols: " + E.Message);
            }
        }

        #endregion

        #region Options
        private void Options_Settings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmSettings set = new frmSettings();
            set.AppSettings = m_appSettings;
            set.Symbols = GetRealtimeNotificationSymbols();
            set.StandardFill = m_appSettings.StandardFill;
            set.InterpolateLogWorksTimescale = m_appSettings.InterpolateLogWorksTimescale;
            set.AutoSizeNewWindows = m_appSettings.AutoSizeNewWindows;
            set.AutoSizeColumnsInViewer = m_appSettings.AutoSizeColumnsInWindows;
            set.AutoUpdateChecksum = m_appSettings.AutoChecksum;
            set.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
            //set.AutoGenerateLogWorksFile = m_appSettings.AutoGenerateLogWorks;
            set.HideSymbolWindow = m_appSettings.HideSymbolTable;
            set.ShowGraphsInMapViewer = m_appSettings.ShowGraphs;
            set.UseRedAndWhiteMaps = m_appSettings.ShowRedWhite;
            set.AutoDockSameFile = m_appSettings.AutoDockSameFile;
            set.AutoDockSameSymbol = m_appSettings.AutoDockSameSymbol;
            set.DisableMapviewerColors = m_appSettings.DisableMapviewerColors;
            set.ShowMapViewersInWindows = m_appSettings.ShowViewerInWindows;
            set.NewPanelsFloating = m_appSettings.NewPanelsFloating;
            set.AutoLoadLastFile = m_appSettings.AutoLoadLastFile;
            set.AlwaysRecreateRepositoryItems = m_appSettings.AlwaysRecreateRepositoryItems;
            set.DefaultViewType = m_appSettings.DefaultViewType;
            set.DefaultViewSize = m_appSettings.DefaultViewSize;
            set.SynchronizeMapviewers = m_appSettings.SynchronizeMapviewers;
            set.FancyDocking = m_appSettings.FancyDocking;
            set.ShowTablesUpsideDown = m_appSettings.ShowTablesUpsideDown;
            set.WriteTimestampInBinary = m_appSettings.WriteTimestampInBinary;
            set.AutoFixFooter = m_appSettings.AutoFixFooter;
            set.EnableCanLog = m_appSettings.EnableCanLog;
            set.OnlyPBus = m_appSettings.OnlyPBus;
            set.AutoCreateAFRMaps = m_appSettings.AutoCreateAFRMaps;
            set.AutoUpdateSRAMViewers = m_appSettings.AutoUpdateSRAMViewers;
            set.UseAdditionalCanbusFrames = false;// m_appSettings.UseAdditionalCanbusFrames;
            set.ResetRealtimeSymbolOnTabPageSwitch = m_appSettings.ResetRealtimeSymbolOnTabPageSwitch;
            set.UseWidebandLambda = m_appSettings.UseWidebandLambda;
            set.WideBandSymbol = m_appSettings.WideBandSymbol;
            set.AutoUpdateInterval = m_appSettings.AutoUpdateInterval;
            set.MeasureAFRInLambda = m_appSettings.MeasureAFRInLambda;
            set.UseNewMapViewer = m_appSettings.UseNewMapViewer;
            set.ProjectFolder = m_appSettings.ProjectFolder;
            set.RequestProjectNotes = m_appSettings.RequestProjectNotes;

            set.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
            set.AreaCorrectionPercentage = m_appSettings.AreaCorrectionPercentage;
            set.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
            set.CellStableTime_ms = m_appSettings.CellStableTime_ms;
            set.CorrectionPercentage = m_appSettings.CorrectionPercentage;
            set.DiscardClosedThrottleMeasurements = m_appSettings.DiscardClosedThrottleMeasurements;
            set.DiscardFuelcutMeasurements = m_appSettings.DiscardFuelcutMeasurements;
            set.DisableClosedLoopOnStartAutotune = m_appSettings.DisableClosedLoopOnStartAutotune;
            set.PlayCellProcessedSound = m_appSettings.PlayCellProcessedSound;
            set.AllowIdleAutoTune = m_appSettings.AllowIdleAutoTune;
            set.EnrichmentFilter = m_appSettings.EnrichmentFilter;
            set.FuelCutDecayTime_ms = m_appSettings.FuelCutDecayTime_ms;
            set.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;
            set.MaximumAFRDeviance = m_appSettings.MaximumAFRDeviance;
            set.MinimumAFRMeasurements = m_appSettings.MinimumAFRMeasurements;
            set.AutoTuneFuelMap = m_appSettings.AutoTuneFuelMap;

            set.AutoLoggingEnabled = m_appSettings.AutoLoggingEnabled;
            set.AutoLogStartSign = m_appSettings.AutoLogStartSign;
            set.AutoLogStartValue = m_appSettings.AutoLogStartValue;
            set.AutoLogStopSign = m_appSettings.AutoLogStopSign;
            set.AutoLogStopValue = m_appSettings.AutoLogStopValue;
            set.AutoLogTriggerStartSymbol = m_appSettings.AutoLogTriggerStartSymbol;
            set.AutoLogTriggerStopSymbol = m_appSettings.AutoLogTriggerStopSymbol;
            
            set.UseDigitalWidebandLambda = m_appSettings.UseDigitalWidebandLambda;
            set.WidebandDevice = m_appSettings.WidebandDevice;
            set.WidebandComPort = m_appSettings.WbPort;
            set.AdapterType = m_appSettings.AdapterType;
            set.Adapter = m_appSettings.Adapter;

            if (set.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.InterpolateLogWorksTimescale = set.InterpolateLogWorksTimescale;
                m_appSettings.AutoSizeNewWindows = set.AutoSizeNewWindows;
                m_appSettings.AutoSizeColumnsInWindows = set.AutoSizeColumnsInViewer;
                m_appSettings.AutoChecksum = set.AutoUpdateChecksum;
                m_appSettings.ShowAddressesInHex = set.ShowAddressesInHex;
                m_appSettings.StandardFill = set.StandardFill;
                //m_appSettings.AutoGenerateLogWorks = set.AutoGenerateLogWorksFile;
                m_appSettings.HideSymbolTable = set.HideSymbolWindow;
                m_appSettings.ShowGraphs = set.ShowGraphsInMapViewer;
                m_appSettings.ShowRedWhite = set.UseRedAndWhiteMaps;
                m_appSettings.DisableMapviewerColors = set.DisableMapviewerColors;
                m_appSettings.AutoDockSameFile = set.AutoDockSameFile;
                m_appSettings.AutoDockSameSymbol = set.AutoDockSameSymbol;
                m_appSettings.ShowViewerInWindows = set.ShowMapViewersInWindows;
                m_appSettings.NewPanelsFloating = set.NewPanelsFloating;
                m_appSettings.WriteTimestampInBinary = set.WriteTimestampInBinary;

                m_appSettings.DefaultViewType = set.DefaultViewType;
                m_appSettings.DefaultViewSize = set.DefaultViewSize;

                m_appSettings.AutoLoadLastFile = set.AutoLoadLastFile;
                m_appSettings.FancyDocking = set.FancyDocking;
                m_appSettings.ShowTablesUpsideDown = set.ShowTablesUpsideDown;
                m_appSettings.AlwaysRecreateRepositoryItems = set.AlwaysRecreateRepositoryItems;
                m_appSettings.SynchronizeMapviewers = set.SynchronizeMapviewers;
                m_appSettings.AutoFixFooter = set.AutoFixFooter;
                m_appSettings.EnableCanLog = set.EnableCanLog;
                m_appSettings.AutoCreateAFRMaps = set.AutoCreateAFRMaps;
                m_appSettings.OnlyPBus = set.OnlyPBus;
                m_appSettings.AutoUpdateSRAMViewers = set.AutoUpdateSRAMViewers;
                m_appSettings.UseAdditionalCanbusFrames = set.UseAdditionalCanbusFrames;
                m_appSettings.ResetRealtimeSymbolOnTabPageSwitch = set.ResetRealtimeSymbolOnTabPageSwitch;
                m_appSettings.WideBandSymbol = set.WideBandSymbol;
                m_appSettings.UseWidebandLambda = set.UseWidebandLambda;
                m_appSettings.AutoUpdateInterval = set.AutoUpdateInterval;
                m_appSettings.MeasureAFRInLambda = set.MeasureAFRInLambda;
                m_appSettings.UseNewMapViewer = set.UseNewMapViewer;
                m_appSettings.ProjectFolder = set.ProjectFolder;
                m_appSettings.RequestProjectNotes = set.RequestProjectNotes;

                m_appSettings.AcceptableTargetErrorPercentage = set.AcceptableTargetErrorPercentage;
                m_appSettings.AreaCorrectionPercentage = set.AreaCorrectionPercentage;
                m_appSettings.AutoUpdateFuelMap = set.AutoUpdateFuelMap;
                m_appSettings.CellStableTime_ms = set.CellStableTime_ms;
                m_appSettings.CorrectionPercentage = set.CorrectionPercentage;
                m_appSettings.DiscardClosedThrottleMeasurements = set.DiscardClosedThrottleMeasurements;
                m_appSettings.DiscardFuelcutMeasurements = set.DiscardFuelcutMeasurements;
                m_appSettings.EnrichmentFilter = set.EnrichmentFilter;
                m_appSettings.FuelCutDecayTime_ms = set.FuelCutDecayTime_ms;
                m_appSettings.MaximumAdjustmentPerCyclePercentage = set.MaximumAdjustmentPerCyclePercentage;
                m_appSettings.MaximumAFRDeviance = set.MaximumAFRDeviance;
                m_appSettings.MinimumAFRMeasurements = set.MinimumAFRMeasurements;
                m_appSettings.AutoTuneFuelMap = set.AutoTuneFuelMap;

                m_appSettings.AutoLoggingEnabled = set.AutoLoggingEnabled;
                m_appSettings.AutoLogStartSign = set.AutoLogStartSign;
                m_appSettings.AutoLogStartValue = set.AutoLogStartValue;
                m_appSettings.AutoLogStopSign = set.AutoLogStopSign;
                m_appSettings.AutoLogStopValue = set.AutoLogStopValue;
                m_appSettings.AutoLogTriggerStartSymbol = set.AutoLogTriggerStartSymbol;
                m_appSettings.AutoLogTriggerStopSymbol = set.AutoLogTriggerStopSymbol;

                m_appSettings.UseDigitalWidebandLambda = set.UseDigitalWidebandLambda;
                m_appSettings.WidebandDevice = set.WidebandDevice;
                m_appSettings.WbPort = set.WidebandComPort;
                m_appSettings.AdapterType = set.AdapterType;
                m_appSettings.Adapter = set.Adapter;

                SetupMeasureAFRorLambda();
                SetupDocking();
                SetupDisplayOptions();
            }
        }

        private void SetupDocking()
        {
            if (!m_appSettings.FancyDocking)
            {
                dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.Standard;
            }
            else
            {
                dockManager1.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.VS2005;
            }
            if (m_appSettings.HideSymbolTable)
            {
                dockSymbols.Visibility = DockVisibility.AutoHide;
                dockSymbols.HideImmediately();
            }
            else
            {
                dockSymbols.Visibility = DockVisibility.Visible;
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

        private void Options_ToggleFullscreen_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Options_LookupPartnumber_ItemClick(object sender, ItemClickEventArgs e)
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
                    OpenFile(filename, true);
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
                    OpenFile(lookup.FileNameToSave, true);
                    m_appSettings.LastOpenedType = 0;

                }
            }
        }
        #endregion

        #region Projects

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

        private void Projects_btnCreateProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show the project properties screen for the user to fill in
            // if a bin file is loaded, ask the user whether this should be the new projects binary file
            // the project XML should contain a reference to this binfile as well as a lot of other stuff
            frmProjectProperties projectprops = new frmProjectProperties();
            if (m_currentfile != string.Empty)
            {
                projectprops.BinaryFile = m_currentfile;
                T7FileHeader fileheader = new T7FileHeader();
                fileheader.init(m_currentfile, false);
                projectprops.CarModel = fileheader.getCarDescription().Trim();

                projectprops.ProjectName = fileheader.getPartNumber().Trim() + " " + fileheader.getSoftwareVersion().Trim();
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
                //LoadAFRMapsForProject(projectname); // <GS-27072010> TODO: nog bekijken voor T7
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
                    btnAddNoteToProject.Enabled = true;
                    btnEditProject.Enabled = true;
                    btnShowProjectLogbook.Enabled = true;
                    btnProduceLatestBinary.Enabled = true;
                    //btncreateb                    
                    btnRebuildFile.Enabled = true;
                    CreateProjectBackupFile();
                    UpdateRollbackForwardControls();
                    m_appSettings.Lastprojectname = m_CurrentWorkingProject;
                    this.Text = "T7SuitePro [Project: " + projectname + "]";
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
                    OpenFile(projectprops.Rows[0]["BINFILE"].ToString(), false);
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

        private void Projects_btnRebuildFile_ItemClick(object sender, ItemClickEventArgs e)
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
            VerifyChecksum(false);
        }

        private void Projects_btnOpenProject_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Projects_btnCloseProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            CloseProject();
            m_appSettings.Lastprojectname = "";
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
            btnAddNoteToProject.Enabled = false;
            btnEditProject.Enabled = false;

            btnRebuildFile.Enabled = false;
            btnRollback.Enabled = false;
            btnRollforward.Enabled = false;
            btnShowTransactionLog.Enabled = false;
            this.Text = "T7SuitePro";
        }

        private void Projects_btnShowTransactionLog_ItemClick(object sender, ItemClickEventArgs e)
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

        private string GetSymbolNameByAddress(Int32 address)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Flash_start_address == address) return sh.Varname;
            }
            return address.ToString();
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
            VerifyChecksum(false);
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

        private void RollBack(TransactionEntry entry)
        {
            int addressToWrite = entry.SymbolAddress;
            while (addressToWrite > m_currentfile_size) addressToWrite -= m_currentfile_size;
            //m_trionicFile.WriteDataNoLog(entry.DataBefore, (uint)addressToWrite);
            savedatatobinary(addressToWrite, entry.SymbolLength, entry.DataBefore, m_currentfile, false);
            VerifyChecksum(false);
            m_ProjectTransactionLog.SetEntryRolledBack(entry.TransactionNumber);
            if (m_CurrentWorkingProject != string.Empty)
            {
                m_ProjectLog.WriteLogbookEntry(LogbookEntryType.TransactionRolledback, GetSymbolNameByAddress(entry.SymbolAddress) + " " + entry.Note + " " + entry.TransactionNumber.ToString());
            }

            UpdateRollbackForwardControls();
        }

        private void Projects_btnRollback_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Projects_btnRollforward_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Projects_btnEditProject_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Projects_btnAddNoteToProject_ItemClick(object sender, ItemClickEventArgs e)
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

        private void Projects_btnShowProjectLogbook_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_CurrentWorkingProject != string.Empty)
            {
                frmProjectLogbook logb = new frmProjectLogbook();

                logb.LoadLogbookForProject(m_appSettings.ProjectFolder, m_CurrentWorkingProject);
                logb.Show();
            }
        }

        private void Projects_btnProduceLatestBinary_ItemClick(object sender, ItemClickEventArgs e)
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
        #endregion

        #region Actions
        private bool CompareSymbolToCurrentFile(string symbolname, int address, int length, string filename, out double diffperc, out int diffabs, out double diffavg)
        {
            diffperc = 0;
            diffabs = 0;
            diffavg = 0;

            double totalvalue1 = 0;
            double totalvalue2 = 0;
            bool retval = true;

            if (symbolname == "CatOx2Dev")
            {
                logger.Debug("break");
            }

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
                    logger.Debug("Lengths didn't match: " + symbolname);
                    return false;
                }
                for (int offset = 0; offset < curdata.Length; offset++)
                {
                    if ((byte)curdata.GetValue(offset) != (byte)compdata.GetValue(offset))
                    {
                        retval = false;
                        //logger.Debug("Difference detected in: " + symbolname + " offset=" + offset.ToString() + " value1: " + curdata[offset].ToString("X2") + " value2: " + compdata[offset].ToString("X2"));
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


        private void Actions_CompareWithOtherBinary_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (m_currentfile != "")
                {
                    CompareToFile(openFileDialog1.FileName);

                }
            }
        }

        private void CompareToFile(string filename)
        {
            if (m_symbols.Count > 0)
            {
                dockManager1.BeginUpdate();
                try
                {
                    DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                    CompareResults compareResults = new CompareResults();
                    compareResults.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                    compareResults.SetFilterMode(m_appSettings.ShowAddressesInHex);
                    compareResults.Dock = DockStyle.Fill;
                    compareResults.Filename = filename;
                    compareResults.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelect);
                    dockPanel.Controls.Add(compareResults);
                    dockPanel.Text = "Compare results: " + Path.GetFileName(filename);
                    dockPanel.DockTo(DockingStyle.Left, 1);
                    dockPanel.Width = 700;

                    SymbolCollection compare_symbols = new SymbolCollection();
                    Trionic7File compareFile = TryToOpenFileUsingClass(filename, out compare_symbols, false);
                    barProgress.EditValue = 60;
                    barProgress.Caption = "Loading header";
                    System.Windows.Forms.Application.DoEvents();

                    T7FileHeader t7fh = new T7FileHeader();
                    t7fh.init(filename, false);
                    int m_sramOffset = t7fh.getSramOffset();
                    if (m_sramOffset == 0) m_sramOffset = compareFile.SramOffsetForOpenFile;
                    if (m_sramOffset == 0) m_sramOffset = 0xEFFC04;
                    barProgress.EditValue = 90;
                    barProgress.Caption = "Starting compare";
                    System.Windows.Forms.Application.DoEvents();

                    System.Windows.Forms.Application.DoEvents();
                    barProgress.Visibility = BarItemVisibility.Always;
                    barProgress.Caption = "Comparing symbols in files...";
                    barProgress.EditValue = 0;
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
                    double diffperc = 0;
                    int diffabs = 0;
                    double diffavg = 0;
                    int percentageDone = 0;
                    int symNumber = 0;
                    if (compare_symbols.Count > 0)
                    {
                        CompareResults cr = new CompareResults();
                        cr.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                        cr.SetFilterMode(m_appSettings.ShowAddressesInHex);
                        Int64 compareStartAddress = 0;
                        Int64 orgStartAddress = 0;
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
                                logger.Debug(E.Message);
                            }

                            string compareName = sh_compare.Varname;
                            if (compareName.StartsWith("Symbolnumber")) compareName = sh_compare.Userdescription;

                            compareStartAddress = sh_compare.Flash_start_address;
                            if (IsSoftwareOpen(compare_symbols))
                            {
                                // get address
                                if (IsSymbolCalibration(compareName) && sh_compare.Length < 0x400 && sh_compare.Flash_start_address > m_currentfile_size)
                                {
                                    compareStartAddress = sh_compare.Flash_start_address - m_sramOffset;
                                }
                            }

                            foreach (SymbolHelper sh_org in m_symbols)
                            {
                                string originalName = sh_org.Varname;
                                if (originalName.StartsWith("Symbolnumber")) originalName = sh_org.Userdescription;

                                if (compareName.Equals(originalName) && compareName != String.Empty)
                                {
                                    if (compareStartAddress > 0 && compareStartAddress < 0x80000)
                                    {
                                        orgStartAddress = (int)GetSymbolAddress(m_symbols, sh_org.Varname);
                                        if (orgStartAddress > 0 && orgStartAddress < 0x80000)
                                        {
                                            if (!CompareSymbolToCurrentFile(compareName, (int)compareStartAddress, sh_compare.Length, filename, out diffperc, out diffabs, out diffavg))
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
                                                        logger.Debug("Failed to assign category to symbol: " + sh_org.Varname + " err: " + cE.Message);
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
                                                        logger.Debug("Failed to assign category to symbol: " + sh_org.Userdescription + " err: " + cE.Message);
                                                    }
                                                }

                                                dt.Rows.Add(sh_compare.Varname, sh_compare.Start_address, compareStartAddress, sh_compare.Length, sh_compare.Length, SymbolTranslator.ToHelpText(sh_compare.Varname, m_appSettings.ApplicationLanguage), false, 0, diffperc, diffabs, diffavg, category, "", sh_org.Symbol_number, sh_compare.Symbol_number, sh_org.Userdescription, false, false);
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
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
                                logger.Debug(E.Message);
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
                                    dt.Rows.Add(varnamecomp, shtest.Start_address, shtest.Flash_start_address, shtest.Length, shtest.Length, SymbolTranslator.ToHelpText(varnamecomp, m_appSettings.ApplicationLanguage), false, 0, 0, 0, 0, "Missing in original", "", 0, shtest.Symbol_number, shtest.Userdescription, true, false);
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
                                logger.Debug(E.Message);
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
                                    dt.Rows.Add(varnamecomp, shtest.Start_address, shtest.Flash_start_address, shtest.Length, shtest.Length, SymbolTranslator.ToHelpText(varnamecomp, m_appSettings.ApplicationLanguage), false, 0, 0, 0, 0, "Missing in compare", "", 0, shtest.Symbol_number, shtest.Userdescription, false, true);
                                }
                            }
                        }
                        compareResults.CompareSymbolCollection = compare_symbols;
                        compareResults.OriginalSymbolCollection = m_symbols;
                        compareResults.OriginalFilename = m_currentfile;
                        compareResults.CompareFilename = filename;
                        compareResults.OpenGridViewGroups(compareResults.gridControl1, 1);
                        compareResults.gridControl1.DataSource = dt.Copy();
                        barProgress.Visibility = BarItemVisibility.Never;
                        barProgress.Caption = "Done";

                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void StartCompareMapViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength, SymbolCollection curSymbols, int symbolnumber)
        {
            try
            {
                // TEST SYMBOLNUMBERS
                if (symbolnumber > 0 && SymbolName.StartsWith("Symbol"))
                {
                    foreach (SymbolHelper h in curSymbols)
                    {
                        if (h.Symbol_number == symbolnumber)
                        {
                            SymbolName = h.Varname;
                        }
                    }
                }
                DockPanel dockPanel;
                bool pnlfound = false;
                foreach (DockPanel pnl in dockManager1.Panels)
                {

                    if (pnl.Text == "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]")
                    {
                        if (pnl.Tag.ToString() == Filename) // <GS-10052011>
                        {
                            dockPanel = pnl;
                            pnlfound = true;
                            dockPanel.Show();
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
                        IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                        tabdet.IsCompareViewer = true;

                        tabdet.Filename = Filename;
                        tabdet.Map_name = SymbolName;
                        tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                        tabdet.Map_cat = XDFCategories.Undocumented;
                        tabdet.X_axisvalues = GetXaxisValues(Filename, curSymbols, tabdet.Map_name);
                        tabdet.Y_axisvalues = GetYaxisValues(Filename, curSymbols, tabdet.Map_name);

                        SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                        string x_axis = string.Empty;
                        string y_axis = string.Empty;
                        string x_axis_descr = string.Empty;
                        string y_axis_descr = string.Empty;
                        string z_axis_descr = string.Empty;
                        axestrans.GetAxisSymbols(tabdet.Map_name, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                        tabdet.X_axis_name = x_axis_descr;
                        tabdet.Y_axis_name = y_axis_descr;
                        tabdet.Z_axis_name = z_axis_descr;

                        /*string xdescr = string.Empty;
                        string ydescr = string.Empty;
                        string zdescr = string.Empty;
                        GetAxisDescriptions(Filename, curSymbols, tabdet.Map_name, out xdescr, out ydescr, out zdescr);
                        tabdet.X_axis_name = xdescr;
                        tabdet.Y_axis_name = ydescr;
                        tabdet.Z_axis_name = zdescr;*/

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
                            TryToAddOpenLoopTables(tabdet);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onSymbolRead += new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);
                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol: " + SymbolName + " [" + Path.GetFileName(Filename) + "]";

                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    foreach (DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == Filename && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                dockPanel.DockTo(DockingStyle.Right, 0);
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


                            /*dockPanel.DockTo(dockManager1, DevExpress.XtraBars.Docking.DockingStyle.Right, 0);
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                            }
                            else
                            {
                                //dockPanel.Width = this.Width - dockSymbols.Width - 10;

                            }
                            if (dockPanel.Width < 400) dockPanel.Width = 400;
                            //                    dockPanel.Width = 400;
                            dockPanel.Controls.Add(tabdet);*/
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

        private void Actions_btnCompareToSRAMSnapshot_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Compare the data from an sram snapshot to the binary file
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
            SymbolCollection scdiff = new SymbolCollection();
            frmProgress progress = new frmProgress();
            progress.SetProgress("Comparing to SRAM snapshot");
            progress.Show();
            int cnt = 0;
            foreach (SymbolHelper sh in m_symbols)
            {
                int percentage = cnt * 100 / m_symbols.Count;
                cnt++;
                progress.SetProgressPercentage(percentage);

                if (sh.Flash_start_address > 0 && sh.Start_address > 0)
                {
                    // get sram content and binfile content for this symbol
                    string symbolname = sh.Varname;
                    if (symbolname.StartsWith("Symbol:")) symbolname = sh.Userdescription;

                    if (IsSymbolCalibration(symbolname))
                    {
                        int address = (int)sh.Flash_start_address;
                        if (IsSoftwareOpen()/*length > 0x10*/)
                        {
                            address = address - GetOpenFileOffset();// 0xEFFC34; // this should autodetect!!!
                            //tabdet.Map_address = address;
                            //tabdet.IsOpenSoftware = _softwareIsOpen;
                            //mapdata = readdatafromfile(m_currentfile, address, length);
                        }
                        if (address < m_currentfile_size)
                        {
                            byte[] sramsymbol = readdatafromSRAMfile(sramfilename, (int)sh.Start_address, (int)sh.Length);
                            byte[] flashsymbol = readdatafromfile(m_currentfile, address, (int)sh.Length);
                            int bdifferent = 0;
                            if (sh.Varname == "BFuelCal.Map")
                            {
                                logger.Debug("break!");
                            }
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
                }
            }
            progress.Close();
            dockManager1.BeginUpdate();
            try
            {
                DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                CompareResults tabdet = new CompareResults();
                tabdet.HideMissingSymbolIndicators();
                tabdet.Dock = DockStyle.Fill;
                tabdet.Filename = sramfilename;

                tabdet.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelectRAM);
                dockPanel.Controls.Add(tabdet);
                //dockPanel.DockAsTab(dockPanel1);
                dockPanel.Text = "SRAM <> BIN Compare results: " + Path.GetFileName(sramfilename);
                dockPanel.DockTo(DockingStyle.Left, 1);
                dockPanel.Width = 700;
                //CompareSymbolTable(filename, compSymbols, compAddressLookup, tabdet.gridControl1);
                tabdet.CompareSymbolCollection = scdiff;

                System.Data.DataTable dt = new System.Data.DataTable();

                // T7
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


                foreach (SymbolHelper sh in scdiff)
                {
                    float diffperc = 0;
                    int diffabs = 0;
                    float diffavg = 0;
                    dt.Rows.Add(sh.Varname, sh.Start_address, sh.Flash_start_address, sh.Length, sh.Length, sh.Description, false, 0, diffperc, diffabs, diffavg, sh.Category.ToString().Replace("_", " "), sh.Subcategory.ToString().Replace("_", " "), sh.Symbol_number, sh.Symbol_number, sh.Userdescription);
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
            if (!e.ShowDiffMap)
            {
                StartTableViewer(e.SymbolName); // normal viewer
                StartSRAMTableViewer(e.Filename, e.SymbolName, e.SymbolLength, (int)GetSymbolAddress(m_symbols, e.SymbolName), (int)GetSymbolAddressSRAM(m_symbols, e.SymbolName));
            }
            else
            {
                //show difference between SRAM and binary file
                //StartCompareDifferenceViewer(e.SymbolName, e.Filename, e.SymbolAddress, e.SymbolLength);
                StartSRAMToFlashCompareDifferenceViewer(e.SymbolName, e.Filename, m_currentfile, e.SymbolLength, (int)GetSymbolAddressSRAM(m_symbols, e.SymbolName), (int)GetSymbolAddress(m_symbols, e.SymbolName));
            }
        }
        private void StartSRAMToFlashCompareDifferenceViewer(string SymbolName, string SRAMFilename, string FlashFilename, int SymbolLength, int SymbolAddressSRAM, int SymbolAddress)
        {
            DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DockPanel pnl in dockManager1.Panels)
            {

                if (pnl.Text == "Symbol difference: " + SymbolName + " [" + Path.GetFileName(SRAMFilename) + "]")
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
                    dockPanel.Tag = SRAMFilename;
                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.Filename = SRAMFilename;
                    tabdet.Map_name = SymbolName;
                    //tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                    //tabdet.Map_cat = TranslateSymbolNameToCategory(tabdet.Map_name);
                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
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
                    tabdet.X_axis_name = x_axis_descr;
                    tabdet.Y_axis_name = y_axis_descr;
                    tabdet.Z_axis_name = z_axis_descr;


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
                        byte[] mapdata = readdatafromSRAMfile(SRAMFilename, SymbolAddressSRAM, length);
                        byte[] mapdataorig = readdatafromSRAMfile(SRAMFilename, SymbolAddressSRAM, length);
                        byte[] mapdata2 = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, SymbolName), GetSymbolLength(m_symbols, SymbolName));

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
                                    logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }

                            tabdet.Map_content = mapdata;
                            tabdet.UseNewCompare = true;
                            tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, isSixteenBitTable(SymbolName));
                            TryToAddOpenLoopTables(tabdet);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onSymbolRead += new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);
                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);

                            //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                            //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                            //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                            //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                            //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                            //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);


                            //dockPanel.DockAsTab(dockPanel1);
                            dockPanel.Text = "Symbol difference: " + SymbolName + " [" + Path.GetFileName(SRAMFilename) + "]";
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol difference: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    foreach (DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                dockPanel.DockTo(DockingStyle.Right, 0);
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
                    }
                }
                catch (Exception E)
                {

                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void Actions_btnCompareSRAMSnapshots_ItemClick(object sender, ItemClickEventArgs e)
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
                    int cnt = 0;
                    frmProgress progress = new frmProgress();
                    progress.SetProgress("Comparing SRAM snapshots");
                    progress.Show();
                    foreach (SymbolHelper sh in m_symbols)
                    {
                        int percentage = cnt * 100 / m_symbols.Count;
                        cnt++;
                        progress.SetProgressPercentage(percentage);
                        //SetTaskProgress(percentage, true);
                        string symbolname = sh.Varname;
                        if (symbolname.StartsWith("Symbol:")) symbolname = sh.Userdescription;
                        if (IsSymbolCalibration(symbolname))
                        {

                            byte[] data_1 = readdatafromSRAMfile(filename_1, (int)sh.Start_address, (int)sh.Length);
                            byte[] data_2 = readdatafromSRAMfile(filename_2, (int)sh.Start_address, (int)sh.Length);
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
                                if (isSixteenBitTable(sh.Varname))
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
                                    if (isSixteenBitTable(sh.Varname)) lengthvalues /= 2;
                                    diffperc = (diffabs * 100) / lengthvalues;
                                    dt.Rows.Add(sh.Varname, sh.Start_address, sh.Flash_start_address, sh.Length, lengthvalues, sh.Description, false, 0, diffperc, diffabs, diffavg, sh.Category.ToString().Replace("_", " "), sh.Subcategory.ToString().Replace("_", " "), sh.Symbol_number, sh.Symbol_number, sh.Userdescription);
                                }
                            }
                        }

                    }
                    progress.Close();
                    SymbolCollection compSymbols = new SymbolCollection();
                    //AddressLookupCollection compAddressLookup = new AddressLookupCollection();
                    dockManager1.BeginUpdate();
                    try
                    {
                        DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                        SRAMCompareResults sramCompareResults = new SRAMCompareResults();
                        sramCompareResults.Dock = DockStyle.Fill;
                        sramCompareResults.Filename1 = filename_1;
                        sramCompareResults.Filename2 = filename_2;
                        sramCompareResults.onSRAMSymbolSelect += new SRAMCompareResults.NotifySRAMSelectSymbol(tabdet_onSRAMSymbolSelect);
                        dockPanel.Controls.Add(sramCompareResults);
                        //dockPanel.DockAsTab(dockPanel1);
                        dockPanel.Text = "SRAM compare results: " + Path.GetFileName(filename_1) + " " + Path.GetFileName(filename_2);
                        bool isDocked = false;
                        foreach (DockPanel pnl in dockManager1.Panels)
                        {
                            if (pnl.Text.StartsWith("SRAM compare results: ") && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
                            {
                                dockPanel.DockAsTab(pnl, 0);
                                isDocked = true;
                                break;
                            }
                        }
                        if (!isDocked)
                        {
                            dockPanel.DockTo(DockingStyle.Left, 1);
                            dockPanel.Width = 700;
                        }
                        //CompareSymbolTable(filename, compSymbols, compAddressLookup, tabdet.gridControl1);
                        sramCompareResults.gridControl1.DataSource = dt;
                        //tabdet.CompareTrionic5Tools.SymbolCollection = compSymbols;
                        //tabdet.CompareAddressLookupCollection = compAddressLookup;
                        sramCompareResults.OpenGridViewGroups(sramCompareResults.gridControl1, 1);
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                    dockManager1.EndUpdate();
                    SetStatusText("SRAM compare done");
                    //SetTaskProgress(0, false);

                }
            }
        }

        void tabdet_onSRAMSymbolSelect(object sender, SRAMCompareResults.SelectSRAMSymbolEventArgs e)
        {

            if (!e.ShowDiffMap)
            {
                //StartTableViewer(e.SymbolName);
                StartSRAMTableViewer(e.Filename1, e.SymbolName, e.SymbolLength, (int)GetSymbolAddress(m_symbols, e.SymbolName), (int)GetSymbolAddressSRAM(m_symbols, e.SymbolName));
                StartSRAMTableViewer(e.Filename2, e.SymbolName, e.SymbolLength, (int)GetSymbolAddress(m_symbols, e.SymbolName), (int)GetSymbolAddressSRAM(m_symbols, e.SymbolName));
            }
            else
            {
                StartSRAMCompareDifferenceViewer(e.SymbolName, e.Filename1, e.Filename2, e.SymbolLength, (int)GetSymbolAddressSRAM(m_symbols, e.SymbolName), "");
            }
        }

        private void StartSRAMCompareDifferenceViewer(string symbolname, string filename1, string filename2, int length, int sramaddress, string symbolDescription)
        {
            DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DockPanel pnl in dockManager1.Panels)
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

                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.IsCompareViewer = true;
                    tabdet.Filename = filename1;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = symbolDescription;
                    ///tabdet.Map_cat = m_trionicFileInformation.GetSymbolCategory(tabdet.Map_name);

                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, symbolname);
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, symbolname);

                    string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                    string x_axis = string.Empty;
                    string y_axis = string.Empty;
                    axestrans.GetAxisSymbols(symbolname, out x_axis, out y_axis, out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;

                    //tabdet.Map_sramaddress = GetSymbolAddressSRAM(SymbolName);
                    int columns = 8;
                    int rows = 8;
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, symbolname, out columns, out rows);
                    int tablewidth = columns;
                    int address = sramaddress;
                    if (address != 0)
                    {
                        //while (address > m_trionicFileInformation.Filelength) address -= m_trionicFileInformation.Filelength;
                        tabdet.Map_address = address;
                        tabdet.Map_length = length;

                        byte[] mapdata = readdatafromSRAMfile(filename1, sramaddress, length);
                        byte[] mapdata2 = readdatafromSRAMfile(filename2, sramaddress, length);
                        if (mapdata.Length == mapdata2.Length)
                        {
                            if (isSixteenBitTable(symbolname))
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

                            tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.Viewtype = SuiteViewType.Easy;
                            tabdet.IsUpsideDown = true;//GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));
                            tabdet.Dock = DockStyle.Fill;

                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            dockPanel.Text = "SRAM symbol difference: " + symbolname + " [" + Path.GetFileName(filename1) + " vs " + Path.GetFileName(filename2) + "]";
                            bool isDocked = false;
                            if (m_appSettings.AutoDockSameSymbol)
                            {
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("SRAM symbol difference: " + symbolname) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    foreach (DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                dockPanel.DockTo(DockingStyle.Right, 0);
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

        private void CopySymbol(string symbolname, string fromfilename, int fromflashaddress, int fromlength, string targetfilename, int targetflashaddress, int targetlength)
        {
            if (symbolname == "TorqueCal.M_NominalMap")
            {
                logger.Debug("breakme");
            }
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

        private bool ShouldTransferSymbol(SymbolHelper sh)
        {
            // FOR TESTING ONLY
            //return true;

            bool retval = false;
            if (sh.Varname.Contains(".") || sh.Userdescription.Contains(".")) retval = true;
            if (sh.Varname == "MapChkCal.ST_Enable" || sh.Userdescription == "MapChkCal.ST_Enable") retval = false;
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
                    if (shcopy.Flash_start_address > 0 && GetSymbolAddress(m_symbols, shcopy.Varname) < 524288 && shcopy.Length > 0 && ShouldTransferSymbol(shcopy))
                    {
                        _onlyFlashSymbols.Symbols.Add(shcopy);
                    }
                }
                frmtransfer.Symbols = /*m_symbols*/_onlyFlashSymbols;
                if (frmtransfer.ShowDialog() == DialogResult.OK)
                {
                    barProgress.Visibility = BarItemVisibility.Always;
                    barProgress.Caption = "Initializing";
                    barProgress.EditValue = 0;
                    System.Windows.Forms.Application.DoEvents();
                    File.Copy(filename, Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetransferringmaps.bin", true);
                    AddToResumeTable("Backup file created (" + Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetransferringmaps.bin)");
                    AddToResumeTable("Transferring data from " + Path.GetFileName(m_currentfile) + " to " + filename);

                    SetStatusText("Start symbol parsing");

                    Trionic7File transferToFile = TryToOpenFileUsingClass(filename, out curSymbolCollection, false);
                    T7FileHeader t7fh = new T7FileHeader();
                    t7fh.init(filename, false);
                    int m_sramOffset = t7fh.getSramOffset();
                    if (m_sramOffset == 0) m_sramOffset = transferToFile.SramOffsetForOpenFile;
                    if (m_sramOffset == 0) m_sramOffset = 0xEFFC04;
                    curSymbolCollection.SortColumn = "Flash_start_address";
                    curSymbolCollection.SortingOrder = GenericComparer.SortOrder.Ascending;
                    curSymbolCollection.Sort();
                    //progress.SetProgress("Start transfer");
                    barProgress.Caption = "Start transferring";
                    barProgress.EditValue = 1;
                    System.Windows.Forms.Application.DoEvents();

                    Int64 currentFlashAddress = 0;
                    foreach (SymbolHelper sh in curSymbolCollection)
                    {
                        currentFlashAddress = sh.Flash_start_address;
                        //TODO: Keep open bins in mind which have sram addresses in stead of normal addresses
                        if (IsSoftwareOpen(curSymbolCollection))
                        {
                            // get address
                            if (IsSymbolCalibration(sh.Varname) && sh.Length < 0x400 && sh.Flash_start_address > m_currentfile_size)
                            {
                                currentFlashAddress = sh.Flash_start_address - m_sramOffset;
                            }
                        }

                        if (currentFlashAddress > 0 && currentFlashAddress < m_currentfile_size && sh.Length < 0x1000)
                        {
                            foreach (SymbolHelper cfsh in m_symbols)
                            {
                                if (ShouldTransferSymbol(cfsh))
                                {
                                    if (cfsh.Varname == sh.Varname || cfsh.Userdescription == sh.Varname || sh.Userdescription == cfsh.Varname || (cfsh.Userdescription == sh.Userdescription && sh.Userdescription != ""))
                                    {
                                        // set correct symbolname
                                        string symbolname = cfsh.Varname;
                                        if (symbolname.StartsWith("Symbolnumber"))
                                        {
                                            if (!sh.Varname.StartsWith("Symbolnumber")) symbolname = sh.Varname;
                                            else if (sh.Userdescription != "") symbolname = sh.Userdescription;
                                            else if (cfsh.Userdescription != "") symbolname = cfsh.Userdescription;
                                        }
                                        if (SymbolInTransferCollection(frmtransfer.Symbols, symbolname))
                                        {
                                            //progress.SetProgress("Transferring: " + symbolname);
                                            barProgress.Caption = "Transferring: " + symbolname;
                                            barProgress.EditValue = 50;
                                            System.Windows.Forms.Application.DoEvents();

                                            CopySymbol(symbolname, m_currentfile, (int)GetSymbolAddress(m_symbols, cfsh.Varname), cfsh.Length, filename, (int)currentFlashAddress, sh.Length);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //progress.SetProgress("Starting report...");
                    barProgress.Caption = "Starting report...";
                    barProgress.EditValue = 80;
                    System.Windows.Forms.Application.DoEvents();

                    UpdateChecksum(filename);
                    VerifyChecksum(false);
                    SetStatusText("Idle.");
                    barProgress.EditValue = 0;
                    barProgress.Caption = "Done";
                    barProgress.Visibility = BarItemVisibility.Never;
                    //progress.Close();
                }
            }
        }

        private void Actions_TransferMaps_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void Actions_CopyAddressTable_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "T7 binary files|*.bin";
            ofd.Multiselect = false;
            ofd.FileName = "";
            ofd.Title = "Select binary file to transfer the address table to...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int addrtaboffset = GetStartOfAddressTableOffset(m_currentfile);
                int addrtaboffset_newfile = GetStartOfAddressTableOffset(ofd.FileName);
                logger.Debug("Addresstable offset 1: " + addrtaboffset.ToString());
                logger.Debug("Addresstable offset 2: " + addrtaboffset_newfile.ToString());
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
                                fsread.Seek(addrtaboffset - 7, SeekOrigin.Begin); //was - 17
                                fswrite.Seek(addrtaboffset_newfile - 7, SeekOrigin.Begin);
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
                                            if ((Convert.ToInt32(bytes.GetValue(8)) != 0x00) || (Convert.ToInt32(bytes.GetValue(9)) != 0x00))
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
                                        logger.Debug(E.Message);
                                    }

                                }
                            }
                        }
                        frmInfoBox info = new frmInfoBox("Transfer done");
                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Transfer cancelled");
                }
            }
        }

        private void ExportToExcel(string mapname, int address, int length, byte[] mapdata, int cols, int rows, bool isSixteenbit, int[] xaxisvalues, int[] yaxisvalues)
        {
            CultureInfo saved = Thread.CurrentThread.CurrentCulture;
            //en-US
            CultureInfo tci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = tci;

            try
            {
                bool isupsidedown = GetMapUpsideDown(mapname);
                try
                {
                    if (xla == null)
                    {
                        xla = new Microsoft.Office.Interop.Excel.Application();
                    }
                }
                catch (Exception xlaE)
                {
                    frmInfoBox info = new frmInfoBox("Failed to create office application interface");
                    logger.Debug("Failed to create office application interface: " + xlaE.Message);
                }

                // turn mapdata upside down
                if (isupsidedown)
                {
                    mapdata = TurnMapUpsideDown(mapdata, cols, rows, isSixteenbit);
                }

                xla.Visible = true;
                Microsoft.Office.Interop.Excel.Workbook wb = xla.Workbooks.Add(XlSheetType.xlWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xla.ActiveSheet;
                ws.Name = "symboldata";

                // Now create the chart.
                ChartObjects chartObjs = (ChartObjects)ws.ChartObjects(Type.Missing);
                ChartObject chartObj = chartObjs.Add(100, 400, 400, 300);
                Microsoft.Office.Interop.Excel.Chart xlChart = chartObj.Chart;

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
                //GetAxisDescriptions(m_currentfile, m_symbols, mapname, out xaxisdescr, out yaxisdescr, out zaxisdescr);
                SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                string x_axis = string.Empty;
                string y_axis = string.Empty;
                axestrans.GetAxisSymbols(mapname, out x_axis, out y_axis, out xaxisdescr, out yaxisdescr, out zaxisdescr);


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
                    logger.Debug("Failed to set y axis: " + E.Message);
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
                try
                {
                    wb.SaveAs(m_currentfile + "~" + mapname + ".xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, false, null, null, null, null);
                }
                catch (Exception sE)
                {
                    logger.Debug("Failed to save workbook: " + sE.Message);
                }


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
                logger.Debug("Failed to export to excel: " + E.Message);
            }
            Thread.CurrentThread.CurrentCulture = saved;
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
                        bool convertSign = false;
                        if (val1 == 0xff)
                        {
                            val1 = 0;
                            val2 = (byte)(0x100 - val2);
                            convertSign = true;
                        }
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        double value = (ival1 * 256) + ival2;
                        if (convertSign) value = -value;
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
                    string Map_name = sh.SmartVarname;
                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, Map_name, out columns, out rows);
                    int address = (int)GetSymbolAddress(m_symbols, Map_name);// (int)sh.Flash_start_address;
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
                frmInfoBox info = new frmInfoBox("No symbol selected in the primary symbol list");
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
            //            for (int rtel = 1; rtel < dt.Rows.Count; rtel++)
            for (int rtel = dt.Rows.Count; rtel >= 1; rtel--)
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
                                        frmInfoBox info = new frmInfoBox("Too much information in file, abort");
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
                    logger.Debug("ImportExcelSymbol: " + E.Message);
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
                        string svalue = cellvalue.ToString("X4");

                        bstr1 = svalue.Substring(svalue.Length - 4, 2);
                        bstr2 = svalue.Substring(svalue.Length - 2, 2);
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
                VerifyChecksum(false);
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
                    frmInfoBox info = new frmInfoBox("Failed to import map from excel: " + E.Message);
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
                logger.Debug(ex.Message);
                return null;
            }
            finally { }
        }

        private void Actions_ExportMapToExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartExcelExport();
        }

        private void Actions_ImportMapFromExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportFileInExcelFormat();
        }

        private void Actions_OpenSRAMFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            //openFileDialog3.FileName = Path.GetFileNameWithoutExtension(m_currentfile) + ".RAM";
            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog3.FileName))
                {
                    OpenSRAMFile(openFileDialog3.FileName);
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

        private void Actions_ViewFileHex_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartHexViewer();
            StartSRAMHexViewer();
        }

        private void StartHexViewer()
        {
            if (m_currentfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DockPanel dockPanel;
                    //= dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Right);
                    if (!m_appSettings.NewPanelsFloating)
                    {
                        dockPanel = dockManager1.AddPanel(DockingStyle.Right);
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
                    dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Controls.Add(hv);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
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
                        //UpdateChecksum(m_currentfile);
                    }
                    else if (c is DockPanel)
                    {
                        DockPanel tpnl = (DockPanel)c;
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
                    else if (c is ControlContainer)
                    {
                        ControlContainer cntr = (ControlContainer)c;
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

        private void StartSRAMHexViewer()
        {
            if (m_currentsramfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DockPanel dockPanel = dockManager1.AddPanel(DockingStyle.Right);
                    dockPanel.Text = "SRAM Hexviewer: " + Path.GetFileName(m_currentfile);
                    dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                    HexViewer hv = new HexViewer();
                    hv.Issramviewer = true;
                    hv.Dock = DockStyle.Fill;
                    dockPanel.Width = 580;
                    hv.LoadDataFromFile(m_currentsramfile, m_symbols);
                    dockPanel.Controls.Add(hv);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void Actions_SearchMapContent_ItemClick(object sender, ItemClickEventArgs e)
        {
            // ask the user for which value to search and if searching should include symbolnames and/or symbol description
            if (ValidateFile())
            {
                SymbolCollection result_Collection = new SymbolCollection();
                frmSearchMaps searchoptions = new frmSearchMaps();
                if (searchoptions.ShowDialog() == DialogResult.OK)
                {
                    frmProgress progress = new frmProgress();
                    progress.SetProgress("Start searching data...");
                    progress.SetProgressPercentage(0);
                    progress.Show();
                    System.Windows.Forms.Application.DoEvents();
                    int cnt = 0;
                    foreach (SymbolHelper sh in m_symbols)
                    {
                        progress.SetProgress("Searching " + sh.Varname);
                        progress.SetProgressPercentage((cnt * 100) / m_symbols.Count);
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
                    progress.Close();
                    if (result_Collection.Count == 0)
                    {
                        frmInfoBox info = new frmInfoBox("No results found...");
                    }
                    else
                    {
                        // start result screen
                        dockManager1.BeginUpdate();
                        try
                        {
                            DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                            CompareResults compareResults = new CompareResults();
                            compareResults.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;
                            compareResults.SetFilterMode(m_appSettings.ShowAddressesInHex);
                            compareResults.Dock = DockStyle.Fill;
                            compareResults.UseForFind = true;
                            compareResults.Filename = m_currentfile;
                            compareResults.onSymbolSelect += new CompareResults.NotifySelectSymbol(tabdet_onSymbolSelectForFind);
                            dockPanel.Controls.Add(compareResults);
                            dockPanel.Text = "Search results: " + Path.GetFileName(m_currentfile);
                            dockPanel.DockTo(DockingStyle.Left, 1);
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

                            foreach (SymbolHelper shfound in result_Collection)
                            {
                                string helptext = SymbolTranslator.ToHelpText(shfound.Varname, m_appSettings.ApplicationLanguage);
                                shfound.createAndUpdateCategory(shfound.Varname);
                                dt.Rows.Add(shfound.Varname, shfound.Start_address, shfound.Flash_start_address, shfound.Length, shfound.Length, helptext, false, 0, 0, 0, 0, shfound.Category, "", shfound.Symbol_number, shfound.Symbol_number);
                            }
                            compareResults.CompareSymbolCollection = result_Collection;
                            compareResults.OpenGridViewGroups(compareResults.gridControl1, 1);
                            compareResults.gridControl1.DataSource = dt.Copy();

                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                        dockManager1.EndUpdate();

                    }


                }
            }
        }

        /// <summary>
        /// TODO: make this work with manually added symbol (user description in stead of varname
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Actions_SIDInformation_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show SID information table from this bin and give the user the option to alter the settings from this table
            SIDInformationTable sidinfotab = new SIDInformationTable();
            SIDICollection entiresidcollection = sidinfotab.GetSIDInformation();
            SIDICollection currentsidcollection = new SIDICollection();
            SIDTranslator sidtrans = new SIDTranslator();

            // so, now get the current settings from the binary
            T7SidEdit t7SidEdit = new T7SidEdit();
            if (!t7SidEdit.init(m_currentfile))
            {
                frmInfoBox info = new frmInfoBox("File not compatible!");
            }
            else
            {
                bool first = true;
                for (int i = 0; i < t7SidEdit.getDataArrayAll().Length; i += 3)
                {
                    if (i + 2 < t7SidEdit.getDataArrayAll().Length)
                    {
                        SIDIHelper sidh = new SIDIHelper();
                        sidh.Symbol = t7SidEdit.getDataArrayAll()[i];
                        sidh.Symbol = sidh.Symbol.Replace((char)0x00, (char)0x20);
                        sidh.AddressSRAM = Convert.ToInt32(t7SidEdit.getDataArrayAll()[i + 1], 16);
                        //if (sidh.AddressSRAM < 0xF00000) sidh.AddressSRAM += 0xef02f0;
                        sidh.Value = t7SidEdit.getDataArrayAll()[i + 2];
                        sidh.IsReadOnly = first;
                        first = false;
                        sidh.Mode = 99;
                        sidtrans.GetSidDescription(sidh);
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            // try to match it to a symbol in the binary file
                            if (sh.Flash_start_address == sidh.AddressSRAM)
                            {

                                sidh.FoundT7Symbol = sh.Varname;
                                if (sidh.FoundT7Symbol.StartsWith("Symbolnumber") && sh.Userdescription != "")
                                {
                                    sidh.FoundT7Symbol = sh.Userdescription;
                                }
                                break;
                            }
                        }

                        currentsidcollection.Add(sidh);
                    }
                }
                if (t7SidEdit.getFileType() == 1)
                {
                    int mode = 0;
                    int modcount = 0;
                    first = true;
                    for (int j = 0; j < t7SidEdit.getDataArrayNew().Length; j += 3)
                    {
                        if (j + 2 < t7SidEdit.getDataArrayNew().Length)
                        {
                            SIDIHelper sidh = new SIDIHelper();
                            sidh.Symbol = t7SidEdit.getDataArrayNew()[j];
                            sidh.Symbol = sidh.Symbol.Replace((char)0x00, (char)0x20);
                            sidh.AddressSRAM = Convert.ToInt32(t7SidEdit.getDataArrayNew()[j + 1], 16);
                            //if (sidh.AddressSRAM < 0xF00000) sidh.AddressSRAM += 0xef02f0;
                            sidh.Value = t7SidEdit.getDataArrayNew()[j + 2];
                            sidh.IsReadOnly = first;
                            first = false;
                            sidtrans.GetSidDescription(sidh);
                            foreach (SymbolHelper sh in m_symbols)
                            {
                                // try to match it to a symbol in the binary file
                                if (sh.Flash_start_address == sidh.AddressSRAM)
                                {
                                    sidh.FoundT7Symbol = sh.Varname;
                                    if (sidh.FoundT7Symbol.StartsWith("Symbolnumber") && sh.Userdescription != "")
                                    {
                                        sidh.FoundT7Symbol = sh.Userdescription;
                                    }
                                    break;
                                }
                            }

                            sidh.Mode = mode;
                            modcount++;
                            if ((modcount % 12) == 0) mode++;
                            currentsidcollection.Add(sidh);
                        }
                    }
                }
                frmSIDInformation frmsid = new frmSIDInformation();
                frmsid.ApplicationLanguage = m_appSettings.ApplicationLanguage;
                frmsid.ShowAddressesInHex = m_appSettings.ShowAddressesInHex;

                frmsid.Sidcollection = currentsidcollection;
                frmsid.Entiresidcollection = entiresidcollection;
                frmsid.Symbols = m_symbols;
                if (frmsid.ShowDialog() == DialogResult.OK)
                {
                    string[] outputarray = new string[frmsid.Get99ModeCount() * 3];
                    int cnt = 0;
                    foreach (SIDIHelper sh in frmsid.Sidcollection)
                    {
                        if (sh.Mode == 99)
                        {
                            outputarray.SetValue(sh.Symbol, cnt++);
                            outputarray.SetValue(sh.AddressSRAM.ToString("X6"), cnt++);
                            outputarray.SetValue(sh.Value, cnt++);
                        }
                    }
                    t7SidEdit.setDataArrayAll(outputarray);
                    if (t7SidEdit.getFileType() == 1)
                    {
                        outputarray = new string[frmsid.GetNewModeCount() * 3];
                        cnt = 0;
                        foreach (SIDIHelper sh in frmsid.Sidcollection)
                        {
                            if (sh.Mode != 99)
                            {
                                outputarray.SetValue(sh.Symbol, cnt++);
                                outputarray.SetValue(sh.AddressSRAM.ToString("X6"), cnt++);
                                outputarray.SetValue(sh.Value, cnt++);
                            }
                        }
                        t7SidEdit.setDataArrayNew(outputarray);
                    }
                    t7SidEdit.saveFile();
                    UpdateChecksum(m_currentfile);
                }
            }
        }

        private void Actions_LimiterCheck_ItemClick(object sender, ItemClickEventArgs e)
        {
            // start a dockview for this <GS-26012011>
            DockPanel dockPanel;

            if (CheckAllTablesAvailable())
            {
                //dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                dockManager1.BeginUpdate();
                try
                {
                    ctrlAirmassResult airmassResult = new ctrlAirmassResult();
                    airmassResult.Dock = DockStyle.Fill;
                    dockPanel = dockManager1.AddPanel(DockingStyle.Right);
                    dockPanel.Tag = m_currentfile;
                    dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
                    dockPanel.Text = "Airmass result viewer: " + Path.GetFileName(m_currentfile);
                    dockPanel.Width = 800;
                    airmassResult.onStartTableViewer += new ctrlAirmassResult.StartTableViewer(airmassResult_onStartTableViewer);
                    airmassResult.onClose += new ctrlAirmassResult.ViewerClose(airmassResult_onClose);
                    airmassResult.Currentfile = m_currentfile;
                    airmassResult.Symbols = m_symbols;
                    airmassResult.CurrentSramOffsett = m_currentSramOffsett;
                    airmassResult.Currentfile_size = m_currentfile_size;
                    airmassResult.Calculate();
                    dockPanel.Controls.Add(airmassResult);
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void airmassResult_onClose(object sender, EventArgs e)
        {
            // lookup the panel which cast this event
            if (sender is ctrlAirmassResult)
            {
                string dockpanelname = "Airmass result viewer: " + Path.GetFileName(m_currentfile);
                foreach (DockPanel dp in dockManager1.Panels)
                {
                    if (dp.Text == dockpanelname)
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                }
            }
        }

        void airmassResult_onStartTableViewer(object sender, ctrlAirmassResult.StartTableViewerEventArgs e)
        {
            StartAViewer(e.SymbolName);
        }

        private bool CheckAllTablesAvailable()
        {
            bool retval = true;
            if (m_currentfile != "")
            {
                if (File.Exists(m_currentfile))
                {
                    if (!SymbolExists("PedalMapCal.m_RequestMap")) retval = false;
                    if (!SymbolExists("TorqueCal.m_AirTorqMap")) retval = false;
                    if (!SymbolExists("TorqueCal.M_NominalMap")) retval = false;
                    if (!SymbolExists("BstKnkCal.MaxAirmass")) retval = false;
                    //if (!SymbolExists("BstKnkCal.MaxAirmassAu")) retval = false;
                    //if (!SymbolExists("FCutCal.m_AirInletLimit")) retval = false;
                    if (!SymbolExists("TorqueCal.M_EngMaxTab")) retval = false;
                    if (!SymbolExists("TorqueCal.M_EngMaxAutTab")) retval = false;
                    //if (!SymbolExists("TorqueCal.M_ManGearLim")) retval = false;
                    //if (!SymbolExists("TorqueCal.M_5GearLimTab")) retval = false;
                    if (!SymbolExists("TorqueCal.m_AirXSP")) retval = false;
                    if (!SymbolExists("TorqueCal.n_EngYSP")) retval = false;
                    if (!SymbolExists("TorqueCal.M_EngXSP")) retval = false;
                    //if (!SymbolExists("LimEngCal.TurboSpeedTab")) retval = false;
                    if (!SymbolExists("BstKnkCal.OffsetXSP")) retval = false;
                    if (!SymbolExists("BstKnkCal.n_EngYSP")) retval = false;
                    if (!SymbolExists("PedalMapCal.n_EngineMap")) retval = false;
                    if (!SymbolExists("PedalMapCal.X_PedalMap")) retval = false;

                    /*
LimEngCal.n_EngSP (might change into: LimEngCal.p_AirSP see http://forum.ecuproject.com/viewtopic.php?f=51&t=1213&p=25640&hilit=buglist+t7suite#p25640)
                     * */
                }
                else retval = false;
            }
            else retval = false;
            return retval;
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

        private int PowerToTorque(int power, int rpm, bool doCorrection)
        {
            double torque = (power * 7121) / rpm;
            /*double correction = 1;
            if (rpm >= 6000) correction = 0.88;
            else if (rpm > 5800) correction = 0.90;
            else if (rpm > 5400) correction = 0.92;
            else if (rpm > 5000) correction = 0.95;
            else if (rpm > 4600) correction = 0.98;*/
            double correction = GetCorrectionFactorForRpm(rpm);
            if (doCorrection)
            {
                torque /= correction;
            }
            return Convert.ToInt32(torque);
        }

        private int TorqueToPower(int torque, int rpm)
        {
            double power = (torque * rpm) / 7121;
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

        private void gridViewSymbols_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Name == gcSymbolsName.Name)
            {
                if (e.CellValue != null)
                {
                    if (e.CellValue != DBNull.Value)
                    {
                        Color c = Color.White;
                        if (e.CellValue.ToString().StartsWith("TorqueCal."))
                        {
                            c = Color.Orange;
                        }
                        else if (e.CellValue.ToString().StartsWith("BoostCal."))
                        {
                            c = Color.OrangeRed;
                        }
                        else if (e.CellValue.ToString().StartsWith("BFuelCal.") || e.CellValue.ToString().StartsWith("Inj") || e.CellValue.ToString().StartsWith("FCutCal.") || e.CellValue.ToString().StartsWith("FCompCal."))
                        {
                            c = Color.LightSteelBlue;
                        }
                        else if (e.CellValue.ToString().StartsWith("Ign") || e.CellValue.ToString().StartsWith("DI"))
                        {
                            c = Color.LightGreen;
                        }
                        else if (e.CellValue.ToString().StartsWith("BstKnkCal."))
                        {
                            c = Color.LightGray;
                        }
                        else if (e.CellValue.ToString().StartsWith("Knk"))
                        {
                            c = Color.Plum;
                        }
                        else if (e.CellValue.ToString().StartsWith("MAFCal."))
                        {
                            c = Color.Yellow;
                        }
                        else if (e.CellValue.ToString().StartsWith("Cruise"))
                        {
                            c = Color.SandyBrown;
                        }
                        else if (e.CellValue.ToString().StartsWith("Evap"))
                        {
                            c = Color.Orchid;
                        }
                        else if (e.CellValue.ToString().StartsWith("Idle"))
                        {
                            c = Color.BurlyWood;
                        }
                        else if (e.CellValue.ToString().StartsWith("Lambda") || e.CellValue.ToString().StartsWith("O2"))
                        {
                            c = Color.Goldenrod;
                        }
                        else if (e.CellValue.ToString().StartsWith("Missf"))
                        {
                            c = Color.Bisque;
                        }
                        else if (e.CellValue.ToString().StartsWith("Purge"))
                        {
                            c = Color.Khaki;
                        }
                        else if (e.CellValue.ToString().StartsWith("SAI"))
                        {
                            c = Color.GreenYellow;
                        }
                        else if (e.CellValue.ToString().StartsWith("StartCal."))
                        {
                            c = Color.SeaGreen;
                        }
                        /*
                    else if (Convert.ToInt32(o) == (int)XDFCategories.Idle)
                    {
                        c = Color.BurlyWood;
                    }*/
                        if (c != Color.White)
                        {
                            //System.Drawing.Drawing2D.LinearGradientBrush gb = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, c, Color.White, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            System.Drawing.SolidBrush sb = new SolidBrush(c);
                            e.Graphics.FillRectangle(sb, e.Bounds);
                            sb.Dispose();
                        }
                    }
                }
            }
            else if (e.Column.Name == gcSymbolsAddress.Name)
            {
                /*if (m_appSettings.ShowAddressesInHex)
                {
                    if (e.CellValue != null)
                    {
                        if (e.CellValue != DBNull.Value)
                        {
                            try
                            {
                                e.DisplayText = Convert.ToInt32(e.CellValue).ToString("X6");
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                }*/
            }
            else if (e.Column.Name == gcSymbolsLength.Name)
            {
                /* if (m_appSettings.ShowAddressesInHex)
                 {
                     if (e.CellValue != null)
                     {
                         if (e.CellValue != DBNull.Value)
                         {
                             e.DisplayText = Convert.ToInt32(e.CellValue).ToString("X4");
                         }
                     }
                 }*/
            }
        }

        private void Actions_EditESP_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            // show ESP calibration value from this bin and give the user the option to alter the setting

            // so, now get the current settings from the binary
            T7EspEdit t7EspEdit = new T7EspEdit();
            if (!t7EspEdit.loadFile(m_currentfile))
            {
                frmInfoBox info = new frmInfoBox("File not compatible!");
            }
            else
            {
                frmEspSelection frmEsp = new frmEspSelection();
                frmEsp.Esp = t7EspEdit.getEspValue();
                if (frmEsp.ShowDialog() == DialogResult.OK)
                {
                    t7EspEdit.setEspValue(frmEsp.Esp);
                    t7EspEdit.saveFile();
                    UpdateChecksum(m_currentfile);
                }
            }
        }

        private void Actions_EditTCMLimit_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            const string symbolname = "VIOSCal.M_TCMOffset";
            long tcmOffsetAddress = GetSymbolAddress(m_symbols, symbolname);
            if (tcmOffsetAddress == 0)
            {
                frmInfoBox info = new frmInfoBox("File not compatible, symbol VIOSCal.M_TCMOffset missing!");
            }
            TCMLimitEdit tcmLimitEdit = new TCMLimitEdit();
            if (!tcmLimitEdit.loadFile(m_currentfile, tcmOffsetAddress))
            {
                frmInfoBox info = new frmInfoBox("File not compatible!");
            }
            else
            {
                byte[] tcmLimit = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, symbolname), GetSymbolLength(m_symbols, symbolname));
                int initialTorqueLimit = 0;
                if (tcmLimit.Length == 2)
                {
                    initialTorqueLimit = Convert.ToInt32(tcmLimit[0]) * 256;
                    initialTorqueLimit += Convert.ToInt32(tcmLimit[1]);
                }
                frmTcmLimit frmTcm = new frmTcmLimit();
                frmTcm.TorqueLimit = initialTorqueLimit;
                frmTcm.Modify = tcmLimitEdit.getModificationEnabled();
                frmTcm.ModifyLowGearSpecific = tcmLimitEdit.getGearLimitModificationEnabled();
                frmTcm.LowGearModify = tcmLimitEdit.Gear;
                frmTcm.LowGearTorqueLimit = tcmLimitEdit.Limit;

                if (frmTcm.ShowDialog() == DialogResult.OK)
                {
                    if (!frmTcm.ModifyLowGearSpecific.Equals(tcmLimitEdit.getGearLimitModificationEnabled()) ||
                        !frmTcm.LowGearModify.Equals(tcmLimitEdit.Gear) ||
                        !frmTcm.LowGearTorqueLimit.Equals(tcmLimitEdit.Limit) ||
                        !frmTcm.Modify.Equals(tcmLimitEdit.getModificationEnabled()) ||
                        !frmTcm.TorqueLimit.Equals(initialTorqueLimit))
                    {
                        tcmLimitEdit.setGearLimitModificationEnabled(frmTcm.ModifyLowGearSpecific);
                        tcmLimitEdit.setModificationEnabled(frmTcm.Modify);
                        tcmLimitEdit.Gear = frmTcm.LowGearModify;
                        tcmLimitEdit.Limit = frmTcm.LowGearTorqueLimit;
                        tcmLimitEdit.saveFile();

                        // set default value 0 Nm as torquelimit if modifications has been disabled
                        if (!tcmLimitEdit.getModificationEnabled() && !tcmLimitEdit.getGearLimitModificationEnabled())
                        {
                            frmTcm.TorqueLimit = 0;
                        }

                        if (!frmTcm.TorqueLimit.Equals(initialTorqueLimit))
                        {
                            int torque = frmTcm.TorqueLimit;
                            tcmLimit[0] = Convert.ToByte(torque / 256);
                            tcmLimit[1] = Convert.ToByte(torque - (int)tcmLimit[0] * 256);
                            savedatatobinary((int)GetSymbolAddress(m_symbols, symbolname), (int)GetSymbolLength(m_symbols, symbolname), tcmLimit, m_currentfile, true, "TCM Limit modification VIOSCal.M_TCMOffset");
                        }
                        UpdateChecksum(m_currentfile);
                    }
                }
            }
        }

        private void Actions_ImportAFRFeedbackMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                int rows = 0;
                int cols = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                byte[] fuelmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BFuelCal.Map"), (int)GetSymbolLength(m_symbols, "BFuelCal.Map"));
                int[] countermap = m_AFRMap.GetAFRCountermap();
                float[] feedbackMap = m_AFRMap.GetFeedbackMap();
                float[] targetMap = m_AFRMap.GetTargetMap();
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int counter = countermap[i * cols + j];
                        if (counter > 0)
                        {
                            float target = targetMap[i * cols + j];
                            float feedback = feedbackMap[i * cols + j];

                            float _afr_diff_percentage = Math.Abs(((target - feedback) / target) * 100);
                            float afr_diff_to_correct = Math.Abs(_afr_diff_percentage); // so, if lean, negative! 
                            int _fuelcorrectionvalue = (int)fuelmap[(i * cols) + j];
                            if (feedback > target)
                            {
                                // lean
                                float _tempcorrectionvalue = _fuelcorrectionvalue;

                                _tempcorrectionvalue *= 100F + afr_diff_to_correct;
                                _tempcorrectionvalue /= 100F;
                                if (_tempcorrectionvalue > 254) _tempcorrectionvalue = 254;
                                _fuelcorrectionvalue = Convert.ToInt32(Math.Round(_tempcorrectionvalue));
                            }
                            else
                            {
                                // rich
                                float _tempcorrectionvalue = _fuelcorrectionvalue;
                                _tempcorrectionvalue *= 100F - afr_diff_to_correct;
                                _tempcorrectionvalue /= 100F;
                                if (_tempcorrectionvalue < 1) _tempcorrectionvalue = 1;
                                _fuelcorrectionvalue = Convert.ToInt32(Math.Round(_tempcorrectionvalue));
                            }
                            if (fuelmap[(i * cols) + j] != (byte)_fuelcorrectionvalue)
                            {
                                fuelmap[(i * cols) + j] = (byte)_fuelcorrectionvalue;
                            }
                        }
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BFuelCal.Map"), (int)GetSymbolLength(m_symbols, "BFuelCal.Map"), fuelmap, m_currentfile, true, "Imported AFR feedback data");
                UpdateChecksum(m_currentfile);
                ClearAFRFeedbackMap();
                UpdateViewersWithName("BFuelCal.Map");
                //UpdateOpenViewers();  update BFuelCal.Map
            }
        }

        private void UpdateViewersWithName(string symbolname)
        {
            try
            {
                // convert feedback map in memory to byte[] in stead of float[]
                int rows = 0;
                int cols = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, symbolname, out cols, out rows);
                byte[] current_map = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, symbolname), (int)GetSymbolLength(m_symbols, symbolname));
                foreach (DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text.StartsWith("Symbol: "))
                    {
                        foreach (Control c in pnl.Controls)
                        {
                            if (c is IMapViewer)
                            {
                                IMapViewer vwr = (IMapViewer)c;
                                if (vwr.Map_name == symbolname)
                                {
                                    vwr.Map_content = current_map;
                                    UpdateViewer(vwr, cols, isSixteenBitTable(symbolname));
                                }
                            }
                            else if (c is DockPanel)
                            {
                                DockPanel tpnl = (DockPanel)c;
                                foreach (Control c2 in tpnl.Controls)
                                {
                                    if (c2 is IMapViewer)
                                    {
                                        IMapViewer vwr2 = (IMapViewer)c2;
                                        if (vwr2.Map_name == symbolname)
                                        {
                                            vwr2.Map_content = current_map;
                                            UpdateViewer(vwr2, cols, isSixteenBitTable(symbolname));
                                        }
                                    }
                                }
                            }
                            else if (c is ControlContainer)
                            {
                                ControlContainer cntr = (ControlContainer)c;
                                foreach (Control c3 in cntr.Controls)
                                {
                                    if (c3 is IMapViewer)
                                    {
                                        IMapViewer vwr3 = (IMapViewer)c3;
                                        if (vwr3.Map_name == symbolname)
                                        {
                                            vwr3.Map_content = current_map;
                                            UpdateViewer(vwr3, cols, isSixteenBitTable(symbolname));
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
                logger.Debug("Refresh viewer " + symbolname + " error: " + E.Message);
            }
        }

        #endregion

        #region Information
        private void btnVinDecoder_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmDecodeVIN decode = new frmDecodeVIN();
            if (m_currentfile != string.Empty)
            {
                T7FileHeader t7InfoHeader = new T7FileHeader();
                t7InfoHeader.init(m_currentfile, m_appSettings.AutoFixFooter);
                decode.SetVinNumber(t7InfoHeader.getChassisID());
            }
            decode.ShowDialog();
        }

        private void btnShowDisassembly_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (t7file != null)
            {
                string outputfile = Path.GetDirectoryName(t7file.FileName);
                outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(t7file.FileName) + ".asm");
                System.Windows.Forms.Application.DoEvents();
                DockPanel panel = dockManager1.AddPanel(DockingStyle.Right);
                ctrlDisassembler disasmcontrol = new ctrlDisassembler() { TrionicFile = t7file, Dock = DockStyle.Fill };
                panel.Controls.Add(disasmcontrol);
                panel.Text = "T7Suite Disassembler";
                panel.Width = this.ClientSize.Width - dockSymbols.Width;
                System.Windows.Forms.Application.DoEvents();
                disasmcontrol.DisassembleFile(outputfile);
            }
        }

        private void btnShowFullDisassembly_ItemClick(object sender, ItemClickEventArgs e)
        {
            string outputfile = Path.GetDirectoryName(m_currentfile);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(m_currentfile) + "_full.asm");
            if (!AssemblerViewerActive(true, outputfile))
            {
                frmProgress progress = new frmProgress();
                progress.Show();
                progress.SetProgress("Start disassembler");
                if (!File.Exists(outputfile))
                {
                    progress.SetProgress("Disassembler running...");
                    Disassembler dis = new Disassembler();
                    dis.DisassembleFileRtf(m_currentfile, outputfile, m_currentfile_size, m_symbols);
                    progress.SetProgress("Disassembler done...");
                }
                progress.SetProgress("Loading assembler file");
                StartAssemblerViewer(outputfile, progress);
                progress.Close();
            }
        }

        private bool AssemblerViewerActive(bool ShowIfActive, string filename)
        {
            bool retval = false;
            foreach (DockPanel pnl in dockManager1.Panels)
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
                        if (c is DockPanel)
                        {
                            DockPanel tpnl = (DockPanel)c;
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

        private void StartAssemblerViewer(string filename, frmProgress progress)
        {
            if (m_currentfile != "")
            {
                dockManager1.BeginUpdate();
                try
                {
                    DockPanel dockPanel = dockManager1.AddPanel(DockingStyle.Right);
                    dockPanel.Text = "Assembler: " + Path.GetFileName(filename);
                    AsmViewer av = new AsmViewer() { Dock = DockStyle.Fill };
                    dockPanel.Width = 800;
                    dockPanel.Controls.Add(av);
                    progress.SetProgress("Loading assembler file ...");
                    av.LoadDataFromFile(filename, m_symbols);
                    progress.SetProgress("Finding starting address in file");
                    av.FindStartAddress(m_currentfile);
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void Information_firmwareInformation_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show firmware information screen!
            bool _correctFooter = m_appSettings.AutoFixFooter;
            frmFirmwareInformation frminfo = new frmFirmwareInformation();

            if (m_currentfile != null)
            {
                if (File.Exists(m_currentfile))
                {
                    T7FileHeader t7InfoHeader = new T7FileHeader();
                    t7InfoHeader.init(m_currentfile, m_appSettings.AutoFixFooter);
                    string swVersion = t7InfoHeader.getSoftwareVersion();
                    PartNumberConverter pnc = new PartNumberConverter();
                    ECUInformation ecuinfo = pnc.GetECUInfo(t7InfoHeader.getPartNumber().Trim(), "");
                    frminfo.SIDDate = t7InfoHeader.getSIDDate();
                    if (ecuinfo.Valid)
                    {
                        frminfo.OriginalCarType = ecuinfo.Carmodel.ToString();
                        frminfo.OriginalEngineType = ecuinfo.Enginetype.ToString();
                    }

                    if (swVersion.Trim() == "EU0AF01C.55P" || swVersion.Trim() == "EU0AF01C.46T" || swVersion.Trim().StartsWith("ET02U01C") || swVersion.Trim() == "ET03F01C.46S")
                    {
                        // additional requirements for the bytes in that location
                        // http://www.trionictuning.com/forum/viewtopic.php?f=17&t=109&p=8569#p8537



                        // set these options correct
                        if (swVersion.Trim().StartsWith("EU0AF01C") || swVersion.Trim() == "ET03F01C.46S")
                        {
                            if ((CheckBytesInFile(m_currentfile, 0x4968E, 0, 2) || (CheckBytesInFile(m_currentfile, 0x4968E, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x4968F, 0x80, 1))) &&
                                (CheckBytesInFile(m_currentfile, 0x496B4, 0, 2) || (CheckBytesInFile(m_currentfile, 0x496B4, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x496B5, 0x80, 1))) &&
                                (CheckBytesInFile(m_currentfile, 0x49760, 0, 2) || (CheckBytesInFile(m_currentfile, 0x49760, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x49761, 0x80, 1))))
                            {
                                frminfo.EnableSIDAdvancedOptions(true);
                                if (/*CheckBytesInFile(m_currentfile, 0x495FA, 0, 2) &&*/ CheckBytesInFile(m_currentfile, 0x4968E, 0, 2) && CheckBytesInFile(m_currentfile, 0x496B4, 0, 2))
                                {
                                    frminfo.SIDDisableStartScreen = true;
                                }
                                else
                                {
                                    frminfo.SIDDisableStartScreen = false;
                                }
                                if (CheckBytesInFile(m_currentfile, 0x49760, 0, 2)) // should be 0x49760 in stead of 0x4975E
                                {
                                    frminfo.SIDDisableAdaptionMessages = true;
                                }
                                else
                                {
                                    frminfo.SIDDisableAdaptionMessages = false;
                                }
                                /*
                                 *  Remove startup screen:
                                    change to 00 00 instead of 00 80 
                                    000495FA // not needed!!! <GS-11042011>
                                    0004968E 
                                    000496B4                              
                                    Remove adaptation messages:
                                    Change 0x49760 to 00 00 instead of 00 80
                                 */
                            }
                            else
                            {
                                frminfo.EnableSIDAdvancedOptions(false);
                            }
                        }
                        else
                        {
                            if ((CheckBytesInFile(m_currentfile, 0x46F4D, 0, 1) || CheckBytesInFile(m_currentfile, 0x46F4D, 0x80, 1)) &&
                                (CheckBytesInFile(m_currentfile, 0x4701F, 0, 1) || CheckBytesInFile(m_currentfile, 0x4701F, 0x80, 1)))
                            {
                                frminfo.EnableSIDAdvancedOptions(true);

                                //Disable startscreen, change 0x00046F4D to 00 in stead of 80
                                if (CheckBytesInFile(m_currentfile, 0x46F4D, 0, 1))
                                {
                                    frminfo.SIDDisableStartScreen = true;
                                }
                                else
                                {
                                    frminfo.SIDDisableStartScreen = false;
                                }
                                //Remove the adaption messages, change 0x0004701F to 00 in stead of 80
                                if (CheckBytesInFile(m_currentfile, 0x4701F, 0, 1))
                                {
                                    frminfo.SIDDisableAdaptionMessages = true;
                                }
                                else
                                {
                                    frminfo.SIDDisableAdaptionMessages = false;
                                }
                            }
                            else
                            {
                                frminfo.EnableSIDAdvancedOptions(false);
                            }

                        }
                    }
                    else
                    {
                        frminfo.EnableSIDAdvancedOptions(false);
                    }

                    // Pavel Angelov created this modification. 
                    // Disable effect of the emission limitation function.
                    if (swVersion.Trim().StartsWith("EU0AF01C"))
                    {
                        if (CheckBytesInFile(m_currentfile, 0x13837, 0x03, 1))
                        {
                            frminfo.EmissionLimitation = true;
                            frminfo.EnableEmissionLimitation(true);
                        }
                        else if (CheckBytesInFile(m_currentfile, 0x13837, 0x02, 1))
                        {
                            frminfo.EmissionLimitation = false;
                            frminfo.EnableEmissionLimitation(true);
                        }
                        else
                        {
                            frminfo.EnableEmissionLimitation(false);
                        }
                    }
                    else
                    {
                        frminfo.EnableEmissionLimitation(false);
                    }


                    frminfo.SoftwareID = t7InfoHeader.getSoftwareVersion();
                    frminfo.ChassisID = t7InfoHeader.getChassisID();
                    frminfo.EngineType = t7InfoHeader.getCarDescription();
                    frminfo.Partnumber = t7InfoHeader.getPartNumber();
                    frminfo.ImmoID = t7InfoHeader.getImmobilizerID();
                    frminfo.SoftwareIsOpen = IsBinaryFileOpen();
                    frminfo.BioPowerSoftware = IsBinaryBiopower();
                    frminfo.BioPowerEnabled = IsBioPowerEnabled();
                    frminfo.CompressedSymboltable = IsBinaryPackedVersion(m_currentfile);
                    frminfo.MissingSymbolTable = IsBinaryMissingSymbolTable();
                    if (frminfo.MissingSymbolTable) frminfo.BioPowerSoftware = true; // only missing in biopower software
                    frminfo.ChecksumEnabled = HasBinaryChecksumEnabled();
                    frminfo.TorqueLimitersEnabled = HasBinaryTorqueLimiterEnabled();
                    if (!HasBinaryTorqueLimiters()) frminfo.TorqueLimitersPresent = false;
                    //if (!frminfo.MissingSymbolTable)
                    {
                        frminfo.OBDIIPresent = HasBinaryOBDIIMaps();
                        if (!frminfo.OBDIIPresent)
                        {
                            frminfo.OBDIIEnabled = false;
                        }
                        else
                        {
                            frminfo.OBDIIEnabled = HasBinaryOBDIIEnabled();
                        }
                    }
                    if (HasBinaryOBDIIMaps())
                    {
                        frminfo.OBDIIEnabled = HasBinaryOBDIIEnabled();
                    }
                    frminfo.SecondLambdaEnabled = HasBinarySecondLambdaEnabled();

                    if (!HasBinarySecondLambdaMap()) frminfo.SecondLambdaPresent = false;

                    if (!HasBinaryTipInOutParameters()) frminfo.FastThrottleResponsePresent = false;
                    else frminfo.FastThrottleResponsePresent = true;
                    frminfo.FastThrottleReponse = HasBinaryFastThrottleResponse();
                    frminfo.ExtraFastThrottleReponse = HasBinaryExtraFastThrottleResponse();
                    if (!HasBinaryTipInOutParameters())
                    {
                        frminfo.FastThrottleReponse = false;
                        frminfo.ExtraFastThrottleReponse = false;
                    }

                    if (!HasBinaryCatalystLightOffParameters()) frminfo.CatalystLightoffPresent = false;
                    else frminfo.CatalystLightoffPresent = true;
                    frminfo.CatalystLightOff = HasBinaryCatalystLightOffEnabled();

                    if (!IsBinaryBiopower()) frminfo.EthanolSensorPresent = false;
                    else frminfo.EthanolSensorPresent = true;
                    frminfo.EthanolSensor = HasBinaryEthanolSensorEnabled();

                    frminfo.ProgrammingDateTime = GetProgrammingDateTime();
                    if (!m_appSettings.WriteTimestampInBinary)
                    {
                        frminfo.DisableTimeStamping();
                    }
                    if (frminfo.ShowDialog() == DialogResult.OK)
                    {
                        if (t7InfoHeader.IsTISBinary(m_currentfile))
                        {
                            // user is trying to update a TIS file, ask for footer correction.
                            if ((frminfo.ImmoID != t7InfoHeader.getImmobilizerID()) || frminfo.ChassisID != t7InfoHeader.getChassisID())
                            {
                                if (!_correctFooter)
                                {
                                    if (MessageBox.Show("It seems you are trying to update data in a TIS file, would you like T7Suite to correct the footer information?", "TIS file question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                    {
                                        //_correctFooter = true;
                                        // create a backup file at this point
                                        File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".binarybackup", true);
                                        t7InfoHeader.init(m_currentfile, true);
                                    }
                                }
                            }
                        }
                        t7InfoHeader.setImmobilizerID(frminfo.ImmoID);
                        t7InfoHeader.setSoftwareVersion(frminfo.SoftwareID);
                        t7InfoHeader.setCarDescription(frminfo.EngineType);
                        t7InfoHeader.setChassisID(frminfo.ChassisID);
                        t7InfoHeader.setSIDDate(frminfo.SIDDate);
                        if (GetProgrammingDateTime() != frminfo.ProgrammingDateTime)
                        {
                            SetProgrammingDateTime(frminfo.ProgrammingDateTime);
                        }

                        if (frminfo.SoftwareIsOpen)
                        {
                            SetBinaryFileOpen();
                        }
                        else
                        {
                            SetBinaryFileClosed();
                        }
                        if (frminfo.TorqueLimitersEnabled && !HasBinaryTorqueLimiterEnabled() && HasBinaryTorqueLimiters())
                        {
                            SetTorqueLimiterEnabled(true);
                        }
                        else if (!frminfo.TorqueLimitersEnabled && HasBinaryTorqueLimiterEnabled() && HasBinaryTorqueLimiters())
                        {
                            SetTorqueLimiterEnabled(false);
                        }
                        if (frminfo.OBDIIEnabled && !HasBinaryOBDIIEnabled())
                        {
                            SetOBDIIEnabled(true);
                        }
                        else if (!frminfo.OBDIIEnabled && HasBinaryOBDIIEnabled())
                        {
                            SetOBDIIEnabled(false);
                        }
                        if (frminfo.SecondLambdaEnabled && HasBinarySecondLambdaMap()/*&& !HasBinarySecondLambdaEnabled()*/)
                        {
                            SetSecondLambdaEnabled(true);
                        }
                        else if (!frminfo.SecondLambdaEnabled && HasBinarySecondLambdaMap() && HasBinarySecondLambdaEnabled())
                        {
                            SetSecondLambdaEnabled(false);
                        }
                        if (HasBinaryTipInOutParameters())
                        {
                            if (frminfo.FastThrottleReponse && !HasBinaryFastThrottleResponse())
                            {
                                SetFastThrottleResponse(true);
                            }
                            else if (!frminfo.FastThrottleReponse && HasBinaryFastThrottleResponse())
                            {
                                SetFastThrottleResponse(false);
                            }
                            if (frminfo.ExtraFastThrottleReponse && !HasBinaryExtraFastThrottleResponse())
                            {
                                SetExtraFastThrottleResponse(true);
                            }
                            else if (!frminfo.ExtraFastThrottleReponse && !frminfo.FastThrottleReponse && HasBinaryExtraFastThrottleResponse())
                            {
                                SetExtraFastThrottleResponse(false);
                            }
                            else if (!frminfo.ExtraFastThrottleReponse && frminfo.FastThrottleReponse && HasBinaryExtraFastThrottleResponse())
                            {
                                SetActG2(false);
                            }

                        }
                        if (HasBinaryCatalystLightOffParameters())
                        {
                            if (frminfo.CatalystLightOff && !HasBinaryCatalystLightOffEnabled())
                            {
                                SetCatalystLightOff(true);
                            }
                            else if (!frminfo.CatalystLightOff && HasBinaryCatalystLightOffEnabled())
                            {
                                SetCatalystLightOff(false);
                            }

                        }
                        if (IsBinaryBiopower())
                        {
                            if (frminfo.BioPowerEnabled && !IsBioPowerEnabled())
                            {
                                SetBioPowerEnabled(true);
                            }
                            else if (!frminfo.BioPowerEnabled && IsBioPowerEnabled())
                            {
                                SetBioPowerEnabled(false);
                            }

                            if (frminfo.EthanolSensor && !HasBinaryEthanolSensorEnabled())
                            {
                                SetEthanolSensor(true);
                            }
                            else if (!frminfo.EthanolSensor && HasBinaryEthanolSensorEnabled())
                            {
                                SetEthanolSensor(false);
                            }
                        }
                        t7InfoHeader.save(m_currentfile);

                        if (swVersion.Trim() == "EU0AF01C.55P" || swVersion.Trim() == "EU0AF01C.46T" || swVersion.Trim().StartsWith("ET02U01C") || swVersion.Trim() == "ET03F01C.46S")
                        {
                            if (swVersion.Trim().StartsWith("EU0AF01C") || swVersion.Trim() == "ET03F01C.46S")
                            {
                                if ((CheckBytesInFile(m_currentfile, 0x4968E, 0, 2) || (CheckBytesInFile(m_currentfile, 0x4968E, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x4968F, 0x80, 1))) &&
                                    (CheckBytesInFile(m_currentfile, 0x496B4, 0, 2) || (CheckBytesInFile(m_currentfile, 0x496B4, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x496B5, 0x80, 1))) &&
                                    (CheckBytesInFile(m_currentfile, 0x49760, 0, 2) || (CheckBytesInFile(m_currentfile, 0x49760, 0x00, 1) && CheckBytesInFile(m_currentfile, 0x49761, 0x80, 1))))
                                {

                                    if (frminfo.SIDDisableStartScreen)
                                    {
                                        byte[] data2write = new byte[2];
                                        data2write.SetValue((byte)0x00, 0);
                                        data2write.SetValue((byte)0x00, 1);
                                        //savedatatobinary(0x495FA, 2, data2write, m_currentfile, false);
                                        savedatatobinary(0x4968E, 2, data2write, m_currentfile, false);
                                        savedatatobinary(0x496B4, 2, data2write, m_currentfile, false);
                                    }
                                    else
                                    {
                                        byte[] data2write = new byte[2];
                                        data2write.SetValue((byte)0x00, 0);
                                        data2write.SetValue((byte)0x80, 1);
                                        //savedatatobinary(0x495FA, 2, data2write, m_currentfile, false);
                                        savedatatobinary(0x4968E, 2, data2write, m_currentfile, false);
                                        savedatatobinary(0x496B4, 2, data2write, m_currentfile, false);
                                    }
                                    if (frminfo.SIDDisableAdaptionMessages)
                                    {
                                        byte[] data2write = new byte[2];
                                        data2write.SetValue((byte)0x00, 0);
                                        data2write.SetValue((byte)0x00, 1);
                                        savedatatobinary(0x49760, 2, data2write, m_currentfile, false);
                                    }
                                    else
                                    {
                                        byte[] data2write = new byte[2];
                                        data2write.SetValue((byte)0x00, 0);
                                        data2write.SetValue((byte)0x80, 1);
                                        savedatatobinary(0x49760, 2, data2write, m_currentfile, false);
                                    }
                                }
                            }
                            else
                            {
                                if ((CheckBytesInFile(m_currentfile, 0x46F4D, 0, 1) || CheckBytesInFile(m_currentfile, 0x46F4D, 0x80, 1)) &&
                                    (CheckBytesInFile(m_currentfile, 0x4701F, 0, 1) || CheckBytesInFile(m_currentfile, 0x4701F, 0x80, 1)))
                                {

                                    //Disable startscreen, change 0x00046F4D to 00 in stead of 80
                                    //Remove the adaption messages, change 0x0004701F to 00 in stead of 80
                                    if (frminfo.SIDDisableStartScreen)
                                    {
                                        byte[] data2write = new byte[1];
                                        data2write.SetValue((byte)0x00, 0);
                                        savedatatobinary(0x46F4D, 1, data2write, m_currentfile, false);
                                    }
                                    else
                                    {
                                        byte[] data2write = new byte[1];
                                        data2write.SetValue((byte)0x80, 0);
                                        savedatatobinary(0x46F4D, 1, data2write, m_currentfile, false);
                                    }
                                    if (frminfo.SIDDisableAdaptionMessages)
                                    {
                                        byte[] data2write = new byte[1];
                                        data2write.SetValue((byte)0x00, 0);
                                        savedatatobinary(0x4701F, 1, data2write, m_currentfile, false);
                                    }
                                    else
                                    {
                                        byte[] data2write = new byte[1];
                                        data2write.SetValue((byte)0x80, 0);
                                        savedatatobinary(0x4701F, 1, data2write, m_currentfile, false);
                                    }
                                }
                            }
                        }

                        // Disable effect of the emission limitation function.
                        if (swVersion.Trim().StartsWith("EU0AF01C"))
                        {
                            if (frminfo.EmissionLimitation)
                            {
                                byte[] data2write = new byte[1];
                                data2write.SetValue((byte)0x03, 0);
                                savedatatobinary(0x13837, 1, data2write, m_currentfile, false);
                            }
                            else
                            {
                                byte[] data2write = new byte[1];
                                data2write.SetValue((byte)0x02, 0);
                                savedatatobinary(0x13837, 1, data2write, m_currentfile, false);
                            }
                        }

                        UpdateChecksum(m_currentfile);
                    }
                }
            }
        }

        private void Information_browseAxisInformation_ItemClick(object sender, ItemClickEventArgs e)
        {
            DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
            AxisBrowser tabdet = new AxisBrowser();
            tabdet.onStartSymbolViewer += new AxisBrowser.StartSymbolViewer(tabdet_onStartSymbolViewer);
            tabdet.ApplicationLanguage = m_appSettings.ApplicationLanguage;
            tabdet.Dock = DockStyle.Fill;
            dockPanel.Controls.Add(tabdet);
            tabdet.ShowSymbolCollection(m_symbols);
            dockPanel.Text = "Axis browser: " + Path.GetFileName(m_currentfile);
            bool isDocked = false;
            foreach (DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Axis browser: ") && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
                {
                    dockPanel.DockAsTab(pnl, 0);
                    isDocked = true;
                    break;
                }
            }
            if (!isDocked)
            {
                dockPanel.DockTo(DockingStyle.Left, 1);
                dockPanel.Width = 700;
            }
        }
        #endregion

        #region BDM

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
                    saveAsName = saveAsName.Replace("T7.scripts.", "");
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
                //string str;
                int percentage = 0;
                int max_bytes = 0x80000;
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
                if (Convert.ToInt32(barProgress.EditValue) != percentage)
                {
                    // need to calculate the percentage
                    barProgress.EditValue = percentage;
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception E)
            {
                logger.Debug("fio_invoke: " + E.Message);
            }
        }

        private void barButtonItem81_ItemClick(object sender, ItemClickEventArgs e)
        {
            // read ECU
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

                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "Binary files|*.bin";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {

                            mRecreateAllScriptResources(Path.GetDirectoryName(sfd.FileName));
                            barProgress.Visibility = BarItemVisibility.Always;
                            barProgress.EditValue = 0;
                            barProgress.Caption = "Dumping ECU";
                            System.Windows.Forms.Application.DoEvents();

                            _globalECUType = ecu_t.Trionic7;
                            fio_bytes = 0;
                            if (!BdmAdapter_DumpECU(sfd.FileName, ecu_t.Trionic7))
                            {
                                frmInfoBox info = new frmInfoBox("Failed to dump ECU");
                            }
                            DeleteScripts(Path.GetDirectoryName(sfd.FileName));
                        }

                    }
                }
                barProgress.Caption = "Dumping ECU";
                barProgress.EditValue = 0;
                barProgress.Visibility = BarItemVisibility.Never;
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception BDMException)
            {
                logger.Debug("Failed to dump ECU: " + BDMException.Message);
                frmInfoBox info = new frmInfoBox("Failed to download firmware from ECU: " + BDMException.Message);
            }
        }

        private void barButtonItem82_ItemClick(object sender, ItemClickEventArgs e)
        {
            // flash ECU
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
                        BdmAdapter_GetVerifyFlash();
                        // program ECU through USB BDM
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Binary files|*.bin";
                        ofd.Multiselect = false;
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            mRecreateAllScriptResources(Path.GetDirectoryName(ofd.FileName));
                            fio_bytes = 0;
                            barProgress.Visibility = BarItemVisibility.Always;
                            barProgress.Caption = "Erasing ECU";
                            System.Windows.Forms.Application.DoEvents();
                            _globalECUType = ecu_t.Trionic7;
                            BdmAdapter_EraseECU(ecu_t.Trionic7);
                            barProgress.Caption = "Flashing ECU";
                            System.Windows.Forms.Application.DoEvents();
                            Thread.Sleep(100);
                            BdmAdapter_FlashECU(ofd.FileName, ecu_t.Trionic7);
                            barProgress.Caption = "Resetting ECU";
                            System.Windows.Forms.Application.DoEvents();
                            Thread.Sleep(100);
                            DeleteScripts(Path.GetDirectoryName(ofd.FileName));
                        }

                    }
                }
                barProgress.EditValue = 0;
                barProgress.Caption = "Idle";
                barProgress.Visibility = BarItemVisibility.Never;
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception BDMException)
            {
                logger.Debug("Failed to program ECU: " + BDMException.Message);
                frmInfoBox info = new frmInfoBox("Failed to program ECU: " + BDMException.Message);
            }
        }
        #endregion

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
                    if (sh.Varname == "BFuelCal.Map" || sh.Varname == "IgnNormCal.Map" || sh.Varname == "AirCtrlCal.map" ||
                        sh.Userdescription == "BFuelCal.Map" || sh.Userdescription == "IgnNormCal.Map" || sh.Userdescription == "AirCtrlCal.map")
                    {
                        retval = true; // found maps > 0x100 in size in sram
                        _softwareIsOpen = true;
                        //                        logger.Debug("Software is open because of symbol: " + sh.Varname);
                    }
                }
            }
            _softwareIsOpenDetermined = true;
            if (_softwareIsOpen)
            {
                barStaticOpenClosed.Caption = "Open/dev binary";
                btnAutoTune.Visible = true;
            }
            else
            {
                barStaticOpenClosed.Caption = "Normal binary";
                btnAutoTune.Visible = false;
            }
            return retval;
        }

        private bool IsSoftwareOpen(SymbolCollection symbols)
        {
            bool retval = false;
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Flash_start_address > m_currentfile_size && sh.Length > 0x100 && sh.Length < 0x400)
                {
                    if (sh.Varname == "BFuelCal.Map" || sh.Varname == "IgnNormCal.Map" || sh.Varname == "AirCtrlCal.map" ||
                        sh.Userdescription == "BFuelCal.Map" || sh.Userdescription == "IgnNormCal.Map" || sh.Userdescription == "AirCtrlCal.map")
                    {
                        retval = true; // found maps > 0x100 in size in sram
                    }
                }
            }
            return retval;
        }

        private bool ValidateFile()
        {
            bool retval = true;
            if (File.Exists(m_currentfile))
            {
                if (m_currentfile == string.Empty)
                {
                    retval = false;
                }
                else
                {
                    FileInfo fi = new FileInfo(m_currentfile);
                    if (fi.Length != 0x80000)
                    {
                        retval = false;
                    }
                    // check first few bytes should be FF FF EF FC
                    if (!ReadT7FileIdent(m_currentfile))
                    {
                        retval = false;
                    }
                }
            }
            else
            {
                retval = false;
                m_currentfile = string.Empty;
            }
            return retval;
        }

        private bool ReadT7FileIdent(string filename)
        {
            bool retval = true;
            
            FileStream fsread = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                if (br.ReadByte() != 0xFF) retval = false;
                if (br.ReadByte() != 0xFF) retval = false;
                if (br.ReadByte() != 0xEF) retval = false;
                if (br.ReadByte() != 0xFC) retval = false;
            }
            fsread.Close();
            fsread.Dispose();
            return retval;
        }

        private Trionic7File t7file;

        private Trionic7File TryToOpenFileUsingClass(string filename, out SymbolCollection symbol_collection, bool isWorkingFile)
        {
            Trionic7File retval = new Trionic7File();

            retval.onProgress += retval_onProgress;
            _softwareIsOpen = false;
            _softwareIsOpenDetermined = false;
            m_currentsramfile = string.Empty; // geen sramfile erbij
            barStaticItem1.Caption = "";
            barFilenameText.Caption = "";

            FileInfo fi = new FileInfo(filename);
            if (fi.IsReadOnly)
            {
                btnReadOnly.Caption = "File is READ ONLY";
            }
            else
            {
                btnReadOnly.Caption = "File access OK";
            }

            try
            {
                if (isWorkingFile)
                {
                    T7FileHeader t7InfoHeader = new T7FileHeader();
                    if (t7InfoHeader.init(filename, m_appSettings.AutoFixFooter))
                    {
                        m_current_softwareversion = t7InfoHeader.getSoftwareVersion();
                        m_currentSramOffsett = t7InfoHeader.getSramOffset();
                    }
                    else
                    {
                        m_current_softwareversion = "";
                    }
                }
            }
            catch (Exception E2)
            {
                logger.Debug(E2.Message);
            }
            AddFileToMRUList(filename);
            symbol_collection = retval.ExtractFile(filename, m_appSettings.ApplicationLanguage, m_current_softwareversion);
            
            SetProgressPercentage(60);
            SetProgress("Examining file");
            System.Windows.Forms.Application.DoEvents();
            if (isWorkingFile)
            {
                if (m_currentSramOffsett == 0)
                {
                    m_currentSramOffsett = retval.SramOffsetForOpenFile;
                    logger.Debug("Overrules m_currentSramOffsett with value from t7file: " + m_currentSramOffsett.ToString("X8"));
                }

                // <GS-27042010> now we need to check if there is a symbol information XML file present.
                try
                {
                    IsSoftwareOpen();
                    // fill in the rest of the parameters
                    barFilenameText.Caption = Path.GetFileNameWithoutExtension(filename);
                }
                catch (Exception E3)
                {
                    logger.Debug(E3.Message);
                }
            }

            if (IsBinaryBiopower())
            {
                foreach (SymbolHelper sh in symbol_collection)
                {
                    if (sh.Varname == "BFuelCal.StartMap")
                    {
                        sh.Varname = "BFuelCal.E85Map";
                        sh.Description = SymbolTranslator.ToHelpText(sh.Varname, m_appSettings.ApplicationLanguage);
                    }
                    if (sh.Userdescription == "BFuelCal.StartMap")
                    {
                        sh.Userdescription = "BFuelCal.E85Map";
                        sh.Description = SymbolTranslator.ToHelpText(sh.Userdescription, m_appSettings.ApplicationLanguage);
                    }
                }
            }
            return retval;
        }

        void retval_onProgress(object sender, Trionic7File.ProgressEventArgs e)
        {
            if (e.Percentage < 100)
            {
                barProgress.Visibility = BarItemVisibility.Always;
                barProgress.EditValue = e.Percentage;
                barProgress.Caption = e.Info;
            }
            else
            {
                barProgress.Caption = "Done";
                barProgress.Visibility = BarItemVisibility.Never;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private static string GetFileDescriptionFromFile(string file)
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
                        name = name.Replace("_x003"+i.ToString()+"_", i.ToString());
                    }
                    retval = name;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return retval;
        }

        private void TryToLoadAdditionalSymbols(string filename)
        {
            System.Data.DataTable dt;
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
                }
                foreach (SymbolHelper sh in m_symbols)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            //SymbolHelper sh = m_symbols[Convert.ToInt32(dr["SYMBOLNUMBER"])];
                            if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                            {
                                if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                                {
                                    if (sh.Varname == String.Format("Symbolnumber {0}", sh.Symbol_number))
                                    {
                                        sh.Userdescription = sh.Varname;
                                        sh.Varname = dr["DESCRIPTION"].ToString();
                                    }
                                    else
                                    {
                                        sh.Userdescription = dr["DESCRIPTION"].ToString();
                                    }
                                    sh.Description = SymbolTranslator.ToHelpText(sh.Varname, m_appSettings.ApplicationLanguage);
                                    if (sh.Category == "Undocumented" || sh.Category == "")
                                    {
                                        if (sh.Varname.Contains("."))
                                        {
                                            try
                                            {
                                                sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                                                //logger.Debug(String.Format("Set cat to {0} for {1}", sh.Category, sh.Userdescription));
                                            }
                                            catch (Exception cE)
                                            {
                                                logger.Debug(String.Format("Failed to assign category to symbol: {0} err: {1}", sh.Userdescription, cE.Message));
                                            }
                                        }
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
            }
        }

        private void InitMruSystem()
        {
            try
            {
                mrudt = new System.Data.DataTable();
                mrudt.Columns.Add("FILENAME");
                mrudt.Columns.Add("FULLPATH");
                // haal uit het register!
                RegistryKey TempKey = null;
                TempKey = Registry.CurrentUser.CreateSubKey("Software\\T7SuitePro");
                //m_mappath = "";
                using (RegistryKey Settings = TempKey.CreateSubKey("MRUList"))
                {
                    if (Settings != null)
                    {
                        string[] vals = Settings.GetValueNames();
                        foreach (string a in vals)
                        {
                            try
                            {
                                string fullpath = Settings.GetValue(a).ToString();
                                mrudt.Rows.Add(a, fullpath);
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("InitMruSystem: " + E.Message);
            }


        }

        private void AddFileToMRUList(string m_currentfile)
        {
            bool fnd = false;
            foreach (DataRow dr in mrudt.Rows)
            {
                if (dr["FULLPATH"].ToString() == m_currentfile)
                {
                    fnd = true;
                }
            }
            if (!fnd)
            {
                mrudt.Rows.Add(Path.GetFileName(m_currentfile), m_currentfile);
            }
        }

        private void SaveMRUList()
        {
            try
            {
                RegistryKey TempKey = null;
                TempKey = Registry.CurrentUser.CreateSubKey("Software\\T7SuitePro");
                using (RegistryKey saveSettings = TempKey.CreateSubKey("MRUList"))
                {
                    foreach (DataRow dr in mrudt.Rows)
                    {
                        saveSettings.SetValue(dr["FILENAME"].ToString(), dr["FULLPATH"].ToString());
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private static int GetStartOfAddressTableOffset(string filename)
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
            int AddressTableOffset = 0;//GetAddressTableOffset(searchsequence);
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {

                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
               // byte[] filebytes = br.ReadBytes((int)fsread.Length);
                //for (int t = 0; t < filebytes.Length; t++)
                //{
                    //if (AddressTableOffset != 0) break;
                      //  byte adrb = filebytes[t];
                while ((fsread.Position < 0x80000) && (AddressTableOffset == 0))
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
                                //t -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                                //t -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                                //t -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                               // t -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                               // t -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                                //t -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                                //t -= 7;
                            }
                            break;
                        case 8:
                            /*if (fsread.Position > 0x5f900)
                            {
                                logger.Debug("Hola");
                            }
                            */
                            if (adrb == (byte)searchsequence.GetValue(8))
                            {
                                // found it
                                //AddressTableOffset = t;
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                                //t -= 8;
                            }
                            break;
                    }

                }
            }
            fsread.Close();
            return AddressTableOffset;
        }

        private static int GetAddressFromOffset(int offset, string filename)
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
                        retval += Convert.ToInt32(br.ReadByte()) * 256 ;
                        retval += Convert.ToInt32(br.ReadByte());
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to retrieve address from: " + offset.ToString("X6") + ": " + E.Message);
                    }
                    fs.Close();
                }
            }
            return retval;
        }


        private static int GetLengthFromOffset(int offset, string filename)
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
                        logger.Debug("Failed to retrieve length from: " + offset.ToString("X6") + ": " + E.Message);
                    }
                    fs.Close();
                }
            }
            return retval;
        }
        

        private int ReadMarkerAddressContent(string filename, int value, out int length, out int val)
        {
            int retval = 0;
            length = 0;
            val = 0;
            if (m_currentfile != string.Empty)
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
                        logger.Debug(E.Message);
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
            if (m_currentfile != string.Empty)
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


        /// <summary>
        /// Check is identifier 0x9B is present in the footer. If this is the case, the file is packed!
        /// </summary>
        /// <param name="m_currentfile"></param>
        /// <returns></returns>
        private bool IsBinaryPackedVersion(string m_currentfile)
        {
            int len = 0;
            string val = "";
            int ival = 0;
            int value = ReadMarkerAddress(m_currentfile, 0x9B, out len, out val);
            value = ReadMarkerAddressContent(m_currentfile, 0x9B, out len, out ival);
            if(value > 0 && ival < m_currentfile_size && ival > 0) return true;
            return false;
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
                logger.Debug(E.Message);
            }
            return retval;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_globalBDMOpened)
                {
                    BdmAdapter_Close();
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to close BDM: " + E.Message);
            }
            if (m_CurrentWorkingProject != "")
            {
                CloseProject();
            }
            m_appSettings.ShowMenu = !ribbonControl1.Minimized;
            SaveLayoutFiles();
            SaveRealtimeTable(Path.Combine(configurationFilesPath.FullName, "rtsymbols.txt"));
            SaveMRUList();
            SaveAFRAndCounterMaps();

            try
            {
                trionic7.Cleanup();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

            Environment.Exit(0);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {            
            ribbonControl1.Minimized = true;
            ribbonControl2.Minimized = true;
            InitSkins();
            SetupDisplayOptions();
            InitMruSystem();

            splash.Close();

            LoadLayoutFiles();

            if (m_startFromCommandLine)
            {
                if (m_commandLineFile != string.Empty)
                {
                    if (File.Exists(m_commandLineFile))
                    {
//                        CloseProject();
                        m_appSettings.Lastprojectname = ""; 
                        OpenFile(m_commandLineFile, true);
                        m_appSettings.LastOpenedType = 0;

                    }
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
                            OpenFile(m_appSettings.Lastfilename, true);
                        }
                    }
                }
                else if (m_appSettings.Lastprojectname != "")
                {
                    OpenProject(m_appSettings.Lastprojectname);
                }
            }

            if(m_appSettings.DebugMode)
            {
                readSymbolToolStripMenuItem.Enabled = true;
            }
            SetupMeasureAFRorLambda();
            SetupDocking();
            LoadMyMaps();
            DynamicTuningMenu();
        }

        private void SetupMeasureAFRorLambda()
        {
            if (m_appSettings.MeasureAFRInLambda)
            {
                AfrViewMode = AFRViewType.LambdaMode;
                linearGauge2.MaxValue = 1.5F;
                linearGauge2.MinValue = 0.5F;
                linearGauge2.GaugeText = "λ ";
                labelControl11.Text = "λ";
                linearGauge2.NumberOfDecimals = 2;
                linearGauge2.NumberOfDivisions = 10;
                //
                btnAFRFeedbackMap.Caption = "Show lambda feedback map";
                btnClearAFRFeedback.Caption = "Clear lambda feedback map";
            }
            else
            {
                linearGauge2.MaxValue = 20;
                linearGauge2.MinValue = 10;
                linearGauge2.GaugeText = "AFR ";
                labelControl11.Text = "AFR";
                linearGauge2.NumberOfDecimals = 1;
                AfrViewMode = AFRViewType.AFRMode;
                btnAFRFeedbackMap.Caption = "Show AFR feedback map";
                btnClearAFRFeedback.Caption = "Clear AFR feedback map";
            }
        }

        private bool m_fileiss19 = false;

        private void OpenFile(string filename, bool showmessage)
        {
            m_fileiss19 = false;
            m_currentsramfile = string.Empty; // geen sramfile erbij
            barStaticItem1.Caption = "";
            if (filename.ToUpper().EndsWith(".S19"))
            {
                m_fileiss19 = true;
                srec2bin convert = new srec2bin();
                string convertedfile = string.Empty;
                if (convert.ConvertSrecToBin(filename, out convertedfile))
                {
                    filename = convertedfile;
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Failed to convert S19 file to binary");
                }
            }

            m_currentfile = filename;
            m_appSettings.Lastfilename = m_currentfile;
            if (ValidateFile())
            {
                m_symbols = new SymbolCollection();
                t7file = TryToOpenFileUsingClass(m_currentfile, out m_symbols, true);
                SetProgressPercentage(70);
                SetProgress("Sorting data");
                System.Windows.Forms.Application.DoEvents();
                m_symbols.SortColumn = "Length";
                m_symbols.SortingOrder = GenericComparer.SortOrder.Descending;
                m_symbols.Sort();
                SetProgressPercentage(80);
                SetProgress("Loading data into view");
                gridControlSymbols.DataSource = m_symbols;
                //gridViewSymbols.BestFitColumns();
                SetDefaultFilters();
                Text = String.Format("T7SuitePro v{0} [ {1} ]", System.Windows.Forms.Application.ProductVersion, Path.GetFileName(m_currentfile));
                SetProgressPercentage(90);
                SetProgress("Loading realtime info");
                // also rearrange the symbolnumbers in the realtime view
                UpdateRealTimeDataTableWithNewSRAMValues();
                SetProgressPercentage(100);
                System.Windows.Forms.Application.DoEvents();
            }
            else
            {
                m_symbols = new SymbolCollection();
                gridControlSymbols.DataSource = m_symbols;
                Text = String.Format("T7SuitePro v{0} [ none ]", System.Windows.Forms.Application.ProductVersion);
                if (showmessage)
                {
                    frmInfoBox info = new frmInfoBox("File is not a Trionic 7 binary file!");
                }
                m_currentfile = string.Empty;
            }
            logger.Debug("Number of symbols loaded: " + m_symbols.Count);

            try
            {
                int _width = 18;
                int _height = 16;
                if (m_appSettings.AutoCreateAFRMaps)
                {
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out _width, out _height);
                    m_AFRMap.RpmYSP = GetSymbolAsIntArray("BFuelCal.RpmYSP");
                    m_AFRMap.AirXSP = GetSymbolAsIntArray("BFuelCal.AirXSP");
                    m_AFRMap.InitializeMaps(_width * _height, m_currentfile);
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to load AFR maps: " + E.Message);
            }

            try
            {
                T7FileHeader t7header = new T7FileHeader();
                t7header.init(m_currentfile, false);
                if(CheckFileInLibrary(t7header.getPartNumber()))
                {
                    btnCompareToOriginal.Enabled = true;
                }
                else btnCompareToOriginal.Enabled = false;
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

            if (m_currentfile != string.Empty)
            {
                LoadRealtimeTable(Path.Combine(configurationFilesPath.FullName, "rtsymbols.txt"));
            }
            // <GS-07072011> If the opened file is a BioPower file, then BFuelCal.StartMap = the actual fuel E85 map
            if (IsBinaryBiopower())
            {
                barButtonItem67.Caption = "Petrol VE Map";
                barButtonItem69.Caption = "E85 VE Map";
            }
            else
            {
                barButtonItem67.Caption = "VE map";
                barButtonItem69.Caption = "Startup VE map";
            }

            DynamicTuningMenu();

            System.Windows.Forms.Application.DoEvents();
        }

        private static bool CheckFileInLibrary(string partnumber)
        {
            bool retval = false;
            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Binaries"))
            {
                string[] files = Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\Binaries", partnumber + ".bin");
                if (files.Length > 0)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private void UpdateRealTimeDataTableWithNewSRAMValues()
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // get symbol
                        try
                        {
                            string symbolname = dr["SymbolName"].ToString();
                            dr["SymbolNumber"] = GetSymbolNumber(m_symbols, symbolname);//GetSymbolAddressSRAM(m_symbols, symbolname);
                            dr["Value"] = 0;
                            dr["Peak"] = dr["Minimum"];
                            dr["SRAMAddress"] = (uint)GetSymbolAddressSRAM(m_symbols, symbolname);
                            dr["Length"] = GetSymbolLength(m_symbols, symbolname);
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }

                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void SetDefaultFilters()
        {
            gridViewSymbols.ActiveFilter.Clear(); // clear filter
            if (!IsSoftwareOpen())
            {
                /*** set filter ***/
                //DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo(@"([Flash_start_address] > 0 AND [Flash_start_address] < 524288)", "Only symbols within binary");
                DevExpress.XtraGrid.Columns.ColumnFilterInfo fltr = new DevExpress.XtraGrid.Columns.ColumnFilterInfo("[Flash_start_address] LIKE '0_____' AND [Length] <> '000000'", "Only symbols within binary");
                gridViewSymbols.ActiveFilter.Add(gcSymbolsAddress, fltr);
            }
            gridViewSymbols.ActiveFilterEnabled = false;
        }


        private void gridViewSymbols_DoubleClick(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.DoEvents();
            int[] selectedrows = gridViewSymbols.GetSelectedRows();

            if (selectedrows.Length > 0)
            {
                int grouplevel = gridViewSymbols.GetRowLevel((int)selectedrows.GetValue(0));
                if (grouplevel >= gridViewSymbols.GroupCount)
                {
                    int[] selrows = gridViewSymbols.GetSelectedRows();
                    if (selrows.Length > 0)
                    {
                        SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                        if (sh == null)
                            return;

                        if (IsSoftwareOpen()) // <GS-09082010> if it is open software, get data from flash instead of sram
                        {   
                            // Should we start a viewer in realtime mode or in offline mode?
                            if (m_RealtimeConnectedToECU)
                            {
                                ShowRealtimeMapFromECU(sh.SmartVarname);
                            }
                            else
                            {
                                StartTableViewer(ECUMode.Offline);
                            }
                        }
                        else
                        {
                            if (sh.Flash_start_address > m_currentfile_size)
                            {
                                logger.Debug("Retrieving stuff from SRAM at address: " + sh.Flash_start_address.ToString("X6"));
                                if (RealtimeCheckAndConnect())
                                {
                                    ShowRealtimeMapFromECU(sh.SmartVarname);
                                }
                            }
                            else
                            {
                                StartTableViewer(ECUMode.Auto);
                            }
                        }
                    }
                }
            }
            if (m_appSettings.HideSymbolTable)
            {
                // hide the symbollist again
                dockSymbols.HideImmediately();
            }
            System.Windows.Forms.Application.DoEvents();
            logger.Debug("Double click seen");
        }

        private void WriteMapToSRAM(string symbolname, byte[] completedata, bool showProgress)
        {
            // TODO: <GS-23052011> needs to update a statusbar to the user can see the progress of writing to SRAM
            if (showProgress)
            {
                barProgress.EditValue = 0;
                barProgress.Caption = "Writing map to SRAM";
                barProgress.Visibility = BarItemVisibility.Always;
                System.Windows.Forms.Application.DoEvents();
            }

            uint sramAddress = (uint)GetSymbolAddressSRAM(m_symbols, symbolname);
            //if (sramAddress < 0xF00000) sramAddress = sramAddress + 0xef02f0;
            int symbolindex = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, symbolname), symbolname);
            if (symbolindex >= 0)
            {
                if (sramAddress < 0xF00000)
                {
                    // cannot use sram address in files that don't have these.. use symbolnumbers instead for trial
                    trionic7.WriteSymbolToSRAM((uint)symbolindex, completedata);
                }
                else
                {
                    trionic7.WriteMapToSRAM(symbolname, completedata, showProgress, sramAddress, symbolindex);
                }
            }
            if (showProgress)
            {
                barProgress.EditValue = 0;
                barProgress.Caption = "Idle";
                barProgress.Visibility = BarItemVisibility.Never;
                System.Windows.Forms.Application.DoEvents();
            }

        }

        private byte[] ReadMapFromSRAM(SymbolHelper sh, bool showProgress)
        {
            m_prohibitReading = true;
            if (showProgress)
            {
                barProgress.EditValue = 0;
                barProgress.Caption = "Reading map from SRAM";
                barProgress.Visibility = BarItemVisibility.Always;
                System.Windows.Forms.Application.DoEvents();
            }            

            byte[] completedata = trionic7.ReadMapfromSRAM(sh.Start_address,sh.Length, showProgress);

            m_prohibitReading = false;
            if (showProgress)
            {
                barProgress.EditValue = 0;
                barProgress.Caption = "Idle";
                barProgress.Visibility = BarItemVisibility.Never;
                System.Windows.Forms.Application.DoEvents();
            }

            return completedata;
        }

        private string TranslateSymbolName(string symbolname)
        {
            return symbolname;
        }

        private int[] GetYaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int[] retval = new int[GetSymbolLength(curSymbols, symbolname)];
            //retval.SetValue(0, 0);
            int yaxisaddress = 0;
            int yaxislength = 0;
            bool issixteenbit = true;
            double multiplier = 1;

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
            multiplier = GetMapCorrectionFactor(y_axis);
            if (symbolname == "TorqueCal.M_ManGearLim" || symbolname == "TorqueCal.M_CabGearLim")
            {
                retval = new int[6];
                retval.SetValue(-1, 0);
                retval.SetValue(1, 1);
                retval.SetValue(2, 2);
                retval.SetValue(3, 3);
                retval.SetValue(4, 4);
                retval.SetValue(5, 5);
                return retval;
            }
            if (symbolname == "GearCal.Ratio" || symbolname == "GearCal")
            {
                retval = new int[5];
                retval.SetValue(1, 0);
                retval.SetValue(2, 1);
                retval.SetValue(3, 2);
                retval.SetValue(4, 3);
                retval.SetValue(5, 4);
                
                return retval;

            }
            if (symbolname == "BstMetCal.BoostMeter")
            {
                retval = new int[6];
                retval.SetValue(0, 0);
                retval.SetValue(1, 1);
                retval.SetValue(2, 2);
                retval.SetValue(3, 3);
                retval.SetValue(4, 4);
                retval.SetValue(5, 5);
                return retval;

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
                        double valtot = (double)value * multiplier;
                        value = (int)valtot;
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
                        double valtot = (double)ival1 * multiplier;
                        ival1 = (int)valtot;
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }


        private int[] GetXaxisValues(string filename, SymbolCollection curSymbols, string symbolname)
        {
            int[] retval = new int[1];
            retval.SetValue(0, 0);
            int xaxisaddress = 0;
            int xaxislength = 0;
            bool issixteenbit = true;
            double multiplier = 1;

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
                    xaxislength = GetSymbolLength(curSymbols, x_axis);
                    xaxisaddress = (int)GetSymbolAddress(curSymbols, x_axis);
                }
            }
            multiplier = GetMapCorrectionFactor(x_axis);

            int number = xaxislength;
            if (xaxislength > 0)
            {
                byte[] axisdata = readdatafromfile(filename,xaxisaddress, xaxislength);
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
                        if (symbolname == "BstKnkCal.MaxAirmass" || symbolname == "BstKnkCal.MaxAirmassAu")
                        {
                            if (value > 32000) value = -(65536 - value); // negatief maken
                        }
                        double valtot = (double)value * multiplier;
                        value = (int)valtot;
                        retval.SetValue(value, offset++);
                    }
                    else
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        int ival1 = Convert.ToInt32(val1);
                        double valtot = (double)ival1 * multiplier;
                        ival1 = (int)valtot;
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }

        private void GetAxisDescriptions(string filename, SymbolCollection curSymbols, string symbolname, out string x, out string y, out string z)
        {
            x = "x-axis";
            y = "y-axis";
            z = "z-axis";
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

        private Int64 GetSymbolAddress(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    if (IsSoftwareOpen() && IsSymbolCalibration(symbolname) /*&& sh.Length > 0x02*/ && sh.Length < 0x400 && sh.Flash_start_address > m_currentfile_size) // <GS-09082010>
                    {
                        return sh.Flash_start_address - GetOpenFileOffset();
                    }
                    /*if (sh.Varname == "X_AccPedalManSP" || sh.Varname == "X_AccPedalAutTAB" || sh.Varname == "X_AccPedalAutSP" || sh.Varname == "X_AccPedalManTAB")
                    {
                        return sh.Flash_start_address - 0x0C;
                    }*/
                    return sh.Flash_start_address;
                }
            }
            return 0;
        }

        private int GetSymbolNumber(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    return sh.Symbol_number;
                }
            }
            return 0;
        }

        private string GetUserDescription(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    return sh.Userdescription;
                }
            }
            return symbolname;
        }

        private Int64 GetSymbolAddressSRAM(SymbolCollection curSymbolCollection, string symbolname)
        {
            foreach (SymbolHelper sh in curSymbolCollection)
            {
                if (sh.SmartVarname == symbolname)
                {
                    return sh.Start_address;
                }
            }
            return 0;
        }

        private bool IsSymbolCalibration(string symbolname)
        {
            //logger.Debug("Iscalibration: " + symbolname);
            if (symbolname.Contains("Cal.")) return true;
            if (symbolname.Contains("Cal1.")) return true;
            if (symbolname.Contains("Cal2.")) return true;
            if (symbolname.Contains("Cal3.")) return true;
            if (symbolname.Contains("Cal4.")) return true;
            if (symbolname.StartsWith("X_Acc")) return true;
            return false;
        }

        private int GetTableMatrixWitdhByName(string filename, SymbolCollection curSymbols, string symbolname, out int columns, out int rows)
        {

            columns = 1;// GetSymbolLength(curSymbols, symbolname) / 2;
            if (symbolname == "TorqueCal.M_NominalMap") columns = 18;
            else if (symbolname == "TorqueCal.M_IgnInflTorqMap") columns = 18; //<GS-31052011>
            //else if (symbolname == "TorqueCal.M_EngXSP") columns = 18;
            else if (symbolname == "TorqueCal.X_AccPedalMap") columns = 16;
            else if (symbolname == "TorqueCal.fi_IgnLimMap") columns = 18; //??
            else if (symbolname == "IgnKnkCal.AdapTimer") columns = 18;
            else if (symbolname == "IgnNormCal2Type") columns = 18;
            else if (symbolname == "KnkSoundRedCalType") columns = 18;
            //else if (symbolname == "IgnLOffCal.n_EngYSP") columns = 18;
            else if (symbolname == "MissfCal.DetLevLowLim") columns = 18;
            else if (symbolname == "MissfCal.DetectLevel") columns = 18;
            else if (symbolname == "KnkFuelCal.EnrichmentMap") columns = 18;
            else if (symbolname == "KnkFuelCalType") columns = 18;
            // else if (symbolname == "IgnNormCal.ST_Enable") columns = 18;
            else if (symbolname == "KnkFuelCal.fi_MapMaxOff") columns = 16;
            else if (symbolname == "AirComp.LimPresComp") columns = 8;
            else if (symbolname == "IgnTempCal.AirMap") columns = 5;
            else if (symbolname == "IgnTempCal.EngMap") columns = 5;
            else if (symbolname == "TorqueCal.fi_IgnMinTab") columns = 4;
            else if (symbolname == "MissfCal.DetectLoadLevel") columns = 5;
            else if (symbolname == "KnkDetCal.RefFactorMap") columns = 16;
            else if (symbolname == "BFuelCal.Map") columns = 18;
            else if (symbolname == "BFuelCal.StartMap") columns = 18;
            else if (symbolname == "BFuelCal.E85Map") columns = 18;
            else if (symbolname == "BFuelCal2.Map") columns = 18;
            else if (symbolname == "BFuelCal2.StartMap") columns = 18;
            //else if (symbolname == "TorqueCal.M_IgnInflTorqMap") columns = 18;
            else if (symbolname == "TCompCal.EnrFacMap") columns = 8;
            else if (symbolname == "TCompCal.EnrFacE85Map") columns = 8;

            else if (symbolname == "TCompCal.EnrFacAutMap") columns = 8;
            else if (symbolname == "AftSt2ExtraCal.EnrFacMap") columns = 15;
            else if (symbolname == "AftSt2ExtraCal.EnrMapE85") columns = 7;
            else if (symbolname == "AftSt1ExtraCal.EnrFacMap") columns = 15;
            else if (symbolname == "StartCal.HighAltFacMap") columns = 15;
            else if (symbolname == "BoostCal.p_DiffILimMap") columns = 4;
            else if (symbolname == "MissfCal.outOfLimDelayMAT") columns = 4;
            else if (symbolname == "KnkAdaptCal.WeightMap2") columns = 4;
            else if (symbolname == "KnkAdaptCal.MaxRef") columns = 3;
            else if (symbolname == "MissfAdap.MissfCntMap") columns = 18;
            else if (symbolname == "TorqueCal.M_PumpLossMap") columns = 2;
            else if (symbolname == "HotStCal2.RestartMap") columns = 6;
            else if (symbolname == "StartCal.ScaleFacRpmE85Map") columns = 8;
            else if (symbolname == "StartCal.ScaleFacRpmMap") columns = 8;
            else if (symbolname == "SwitchCal.A_AmbPresMap") columns = 2;

            /*
            Maps met lengte 242 hebben 11 hoogte en breedte 22
            Maps met lengte 200 hebben 10 hoogte en breedte 20
            Maps met lengte 198 hebben 11 hoogte en breedte 18
            Maps met lengte 100 hebben 5 hoogte en breedte 20                 * */

/* 16 
TorqueCal.M_NominalMap 18
TorqueCal.M_IgnInflTroqMap 8*/
            else if (GetSymbolLength(curSymbols, symbolname) == 576) columns = 18;
            else if (GetSymbolLength(curSymbols, symbolname) == 512) columns = 16;

            else if (GetSymbolLength(curSymbols, symbolname) == 336) columns = 12;

            else if (GetSymbolLength(curSymbols, symbolname) == 288) columns = 9;
            else if (GetSymbolLength(curSymbols, symbolname) == 256) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 242) columns = 11; // of 11?
            else if (GetSymbolLength(curSymbols, symbolname) == 224) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 200) columns = 10; // 
            else if (GetSymbolLength(curSymbols, symbolname) == 198) columns = 9; // 
            else if (GetSymbolLength(curSymbols, symbolname) == 192) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 128) columns = 8;
            else if (GetSymbolLength(curSymbols, symbolname) == 120) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 100) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 98) columns = 7;
            else if (GetSymbolLength(curSymbols, symbolname) == 80) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 60) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 50) columns = 5;
            else if (GetSymbolLength(curSymbols, symbolname) == 96) columns = 6;
            else if (GetSymbolLength(curSymbols, symbolname) == 64) columns = 4;
            else if (GetSymbolLength(curSymbols, symbolname) == 160) columns = 10;
            else if (GetSymbolLength(curSymbols, symbolname) == 72) columns = 9;
            
            rows = GetSymbolLength(curSymbols, symbolname) / columns;
            return columns;
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
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
                string text = SymbolTranslator.ToHelpText(symbolname, m_appSettings.ApplicationLanguage);
                if (text.Contains("Resolution is"))
                {
                    int idx = text.IndexOf("Resolution is");
                    idx += 14;
                    string value = text.Substring(idx).Trim();
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
                logger.Debug(E.Message);
            }
            if (returnvalue == 0)
            {
                returnvalue = 1;
                if (symbolname == "KnkSoundRedCal.fi_OffsMap") returnvalue = 0.1;
                else if (symbolname == "IgnE85Cal.fi_AbsMap") returnvalue = 0.1;
                else if (symbolname == "BstKnkCal.OffsetXSP") returnvalue = 0.1;
                //else if (symbolname == "IgnNormCal.Map") returnvalue = 0.1;
                else if (symbolname == "MAFCal.cd_ThrottleMap") returnvalue = 0.0009765625;
                else if (symbolname == "HotStCal2.RestartMap") returnvalue = 0.001;
            }
            return returnvalue;
        }

        private bool GetMapUpsideDown(string symbolname)
        {
           // return m_appSettings.ShowTablesUpsideDown;
            return true;
        }

        private bool isSixteenBitTable(string symbolname)
        {
            bool retval = true;
            if (symbolname == "KnkDetCal.RefFactorMap") retval = false;
            else if (symbolname == "BFuelCal.Map") retval = false;
            else if (symbolname == "BFuelCal.StartMap") retval = false;
            else if (symbolname == "BFuelCal.E85Map") retval = false;
            else if (symbolname == "BFuelCal2.Map") retval = false;
            else if (symbolname == "BFuelCal2.StartMap") retval = false;
            else if (symbolname == "TorqueCal.M_IgnInflTorqMap") retval = false;
            else if (symbolname == "TCompCal.EnrFacMap") retval = false;
            else if (symbolname == "TCompCal.EnrFacAutMap") retval = false;
            else if (symbolname == "TCompCal.EnrFacE85Map") retval = false;
            else if (symbolname == "AftSt2ExtraCal.EnrFacMap") retval = false;
            else if (symbolname == "AftSt1ExtraCal.EnrFacMap") retval = false;
            else if (symbolname == "StartCal.HighAltFacMap") retval = false;
            else if (symbolname == "MissfAdap.MissfCntMap") retval = false;
            else if (symbolname == "WriteProtectedECU") retval = false;
            else if (symbolname == "Data_name") retval = false;
            else if (symbolname == "MAFCal.ConstT_EngineTab") retval = false;
            else if (symbolname == "MAFCal.ConstT_AirInlTab") retval = false;
            int symlen = GetSymbolLength(m_symbols, symbolname);
            if ((symlen % 2) == 1) retval = false;
            return retval;
        }

        public enum ECUMode : int
        {
            Offline,
            Auto
        }

        private void StartTableViewer(ECUMode mode)
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    int row = (int)selrows.GetValue(0);
                    if (row >= 0)
                    {
                        SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                        if (sh.Flash_start_address == 0 && sh.Start_address == 0)
                            return;

                        //DataRowView dr = (DataRowView)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                        if (sh == null) return;
                        /*if (sh.Flash_start_address > m_currentfile_size)
                        {
                            MessageBox.Show("Symbol outside of flash boundary, probably SRAM only symbol");
                            return;
                        }*/

                        string varname = sh.SmartVarname;
                        DockPanel dockPanel;
                        //DockPanel sramdockPanel;
                        bool pnlfound = false;
                        //bool srampnlfound = false;

                        try
                        {
                            foreach (DockPanel pnl in dockManager1.Panels)
                            {
                                if (pnl.Text == "Symbol: " + varname + " [" + Path.GetFileName(m_currentfile) + "]")
                                {
                                    if (pnl.Tag.ToString() == m_currentfile) // <GS-11052011>
                                    {
                                        dockPanel = pnl;
                                        pnlfound = true;
                                        dockPanel.Show();
                                    }
                                }
                                /* if (pnl.Text == "SRAM Symbol: " + varname + " [" + Path.GetFileName(m_currentfile) + "]")
                                 {
                                     sramdockPanel = pnl;
                                     srampnlfound = true;
                                     //sramdockPanel.Show();
                                     // nog data verversen?
                                 }*/
                            }
                        }
                        catch (Exception E)
                        {
                            logger.Debug(E.Message);
                        }
                        if (!pnlfound)
                        {
                            //dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                            logger.Debug("Begin update");
                            dockManager1.BeginUpdate();
                            try
                            {
                                IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                                tabdet.Visible = false;
                                tabdet.Filename = m_currentfile;
                                tabdet.Map_name = varname;
                                tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                                tabdet.Map_cat = XDFCategories.Undocumented; //TranslateSymbolNameToCategory(tabdet.Map_name);

                                SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                                string x_axis = string.Empty;
                                string y_axis = string.Empty;
                                string x_axis_descr = string.Empty;
                                string y_axis_descr = string.Empty;
                                string z_axis_descr = string.Empty;
                                axestrans.GetAxisSymbols(varname, out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                                tabdet.X_axis_name = x_axis_descr;
                                tabdet.Y_axis_name = y_axis_descr;
                                tabdet.Z_axis_name = z_axis_descr;
                                tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                                tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);

                                //tabdet.Yaxiscorrectionfactor = GetMapCorrectionFactor(y_axis);
                                //tabdet.Xaxiscorrectionfactor = GetMapCorrectionFactor(x_axis);
                                //tabdet.close
                                // z, y and z axis to do
                                if (!m_appSettings.NewPanelsFloating)
                                {
                                    dockPanel = dockManager1.AddPanel(DockingStyle.Right);
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
                                dockPanel.ClosedPanel += new DockPanelEventHandler(dockPanel_ClosedPanel);
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
                                int sramaddress = 0;// Convert.ToInt32(dr.Row["SRAMADDRESS"].ToString());
                                if (address != 0)
                                {
                                    logger.Debug("address: " + address.ToString("X8"));

                                    //while (address > m_currentfile_size) address -= m_currentfile_size;

                                    tabdet.Map_address = address;
                                    tabdet.Map_sramaddress = sramaddress;
                                    int length = Convert.ToInt32(sh.Length);
                                    tabdet.Map_length = length;
                                    byte[] mapdata = new byte[sh.Length];
                                    mapdata.Initialize();
                                    if (address < 0x0F00000)
                                    {
                                        logger.Debug("read data from file");
                                        mapdata = readdatafromfile(m_currentfile, address, length);
                                    }
                                    else
                                    {
                                        if (IsSoftwareOpen()/*length > 0x10*/)
                                        {
                                            address = address - GetOpenFileOffset();// 0xEFFC34; // this should autodetect!!!
                                            tabdet.Map_address = address;
                                            tabdet.IsOpenSoftware = _softwareIsOpen;
                                            mapdata = readdatafromfile(m_currentfile, address, length);
                                        }
                                    }

                                    logger.Debug("mapdata len: " + mapdata.Length.ToString("X4"));

                                    tabdet.Map_content = mapdata;

                                    tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                                    tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                                    tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);

                                    //                                    m_connectedToECU = true;

                                    //<GS-07102010>
                                    if (mode == ECUMode.Auto)
                                    {
                                        if ((m_RealtimeConnectedToECU))
                                        {
                                            if (m_appSettings.UseNewMapViewer)
                                            {
                                                tabdet.OnlineMode = true;
                                            }
                                            tabdet.IsRAMViewer = true;
                                        }
                                        else
                                        {
                                            tabdet.IsRAMViewer = false;
                                        }
                                    }

                                    tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));

                                    TryToAddOpenLoopTables(tabdet);
                                    tabdet.Dock = DockStyle.Fill;
                                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                                    tabdet.onSymbolRead += new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);
                                    tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                                    //tabdet.onAxisLock += new MapViewer.NotifyAxisLock(tabdet_onAxisLock);
                                    //tabdet.onSliderMove += new MapViewer.NotifySliderMove(tabdet_onSliderMove);
                                    //tabdet.onSelectionChanged += new MapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                    //tabdet.onSplitterMoved += new MapViewer.SplitterMoved(tabdet_onSplitterMoved);
                                    //tabdet.onSurfaceGraphViewChanged += new MapViewer.SurfaceGraphViewChanged(tabdet_onSurfaceGraphViewChanged);
                                    //tabdet.onGraphSelectionChanged += new MapViewer.GraphSelectionChanged(tabdet_onGraphSelectionChanged);
                                    //tabdet.onViewTypeChanged += new MapViewer.ViewTypeChanged(tabdet_onViewTypeChanged);
                                    //tabdet.onAxisEditorRequested += new MapViewer.AxisEditorRequested(tabdet_onAxisEditorRequested);
                                    tabdet.onWriteToSRAM += new IMapViewer.WriteDataToSRAM(tabdet_onWriteToSRAM);
                                    tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(tabdet_onReadFromSRAM);
                                    tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                                    tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                                    tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);

                                    //dockPanel.DockAsTab(dockPanel1);
                                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_currentfile) + "]";
                                    // autodock same symbolname!!!
                                    bool isDocked = false;
                                    if (m_appSettings.AutoDockSameSymbol)
                                    {
                                        foreach (DockPanel pnl in dockManager1.Panels)
                                        {
                                            if (pnl.Text.StartsWith("Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                            foreach (DockPanel pnl in dockManager1.Panels)
                                            {
                                                if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    }
                                    dockPanel.Controls.Add(tabdet);
                                }
                                else
                                {
                                    byte[] mapdata = new byte[sh.Length];
                                    mapdata.Initialize();

                                }
                                tabdet.Visible = true;
                            }
                            catch (Exception newdockE)
                            {
                                logger.Debug(newdockE.Message);
                            }
                            logger.Debug("End update");
                            dockManager1.EndUpdate();
                        }
                        /*if (!srampnlfound)
                        {
                            // ook openen
                            // show sram equivalent for the current symbol
                            if (m_currentsramfile != string.Empty)
                            {

                                try
                                {
                                    int sramaddress = (int)sh.Start_address;
                                    int length = sh.Length;
                                    if (sramaddress != 0)
                                    {
                                        StartSRAMTableViewer();
                                    }
                                }
                                catch (Exception E)
                                {
                                    logger.Debug(E.Message);
                                }
                            }
                        }*/
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
            }

        }

        void mv_onSurfaceGraphViewChanged(object sender, IMapViewer.SurfaceGraphViewChangedEventArgs e)
        {
            if (m_appSettings.SynchronizeMapviewers)
            {
                foreach (DockPanel pnl in dockManager1.Panels)
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
                        else if (c is DockPanel)
                        {
                            DockPanel tpnl = (DockPanel)c;
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
                        else if (c is ControlContainer)
                        {
                            ControlContainer cntr = (ControlContainer)c;
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
                foreach (DockPanel pnl in dockManager1.Panels)
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
                        else if (c is DockPanel)
                        {
                            DockPanel tpnl = (DockPanel)c;
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
                        else if (c is ControlContainer)
                        {
                            ControlContainer cntr = (ControlContainer)c;
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
                foreach (DockPanel pnl in dockManager1.Panels)
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
                        else if (c is DockPanel)
                        {
                            DockPanel tpnl = (DockPanel)c;
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
                        else if (c is ControlContainer)
                        {
                            ControlContainer cntr = (ControlContainer)c;
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


        void dockPanel_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            if (sender is DockPanel)
            {
                DockPanel dockPanel = (DockPanel)sender;
                if (e.KeyCode == Keys.F10)
                {
                    if (dockPanel.FloatForm != null)
                    {
                        dockPanel.Restore();
                    }
                }
                else if (e.KeyCode == Keys.F9)
                {
                    Bitmap bmp = new Bitmap(dockPanel.Width, dockPanel.Height);
                    dockPanel.DrawToBitmap(bmp, new System.Drawing.Rectangle(0,0,dockPanel.Width, dockPanel.Height));
                    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\screenshot.jpg")) File.Delete(System.Windows.Forms.Application.StartupPath + "\\screenshot.jpg");
                    bmp.Save(System.Windows.Forms.Application.StartupPath + "\\screenshot.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        void tabdet_onSymbolRead(object sender, IMapViewer.ReadSymbolEventArgs e)
        {
            //<GS-12102010> refresh data from file
            if (sender is IMapViewer)
            {
                IMapViewer mv = (IMapViewer)sender;
                mv.Map_content = readdatafromfile(e.Filename, (int)GetSymbolAddress(m_symbols, e.SymbolName), GetSymbolLength(m_symbols, e.SymbolName));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(e.Filename, m_symbols, e.SymbolName, out cols, out rows);
                mv.IsRAMViewer = false;
                mv.OnlineMode = false;
                mv.ShowTable(cols, isSixteenBitTable(e.SymbolName));
                mv.IsRAMViewer = false;
                mv.OnlineMode = false;
                System.Windows.Forms.Application.DoEvents();
            }
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

        private void TryToAddOpenLoopTables(IMapViewer mv)
        {
            try
            {
                //if (mv.Map_name == "BFuelCal.Map" || mv.Map_name == "IgnNormCal.Map" || mv.Map_name == "TargetAFR" || mv.Map_name == "FeedbackAFR" || mv.Map_name == "FeedbackvsTargetAFR" )
                //{

                    byte[] open_loop = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "LambdaCal.MaxLoadNormTab"), GetSymbolLength(m_symbols, "LambdaCal.MaxLoadNormTab"));
                    if (mv.Map_name == "IgnE85Cal.fi_AbsMap" || mv.Map_name == "BFuelCal.E85Map")
                    {
                        open_loop = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "LambdaCal.MaxLoadE85Tab"), GetSymbolLength(m_symbols, "LambdaCal.MaxLoadE85Tab"));
                    }
                    mv.Open_loop = open_loop;
                    mv.StandardFill = m_appSettings.StandardFill;
/*                    byte[] open_loop_knock = m_trionicFile.ReadData((uint)m_trionicFileInformation.GetSymbolAddressFlash(m_trionicFileInformation.GetOpenLoopKnockMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetOpenLoopKnockMap()));
                    mv.Open_loop_knock = open_loop_knock;*/
                    //TODO: add counters as well.
                    //mv.Afr_counter = AFRMapCounterInMemory;
                //}
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
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
                    if (RealtimeCheckAndConnect())
                    {
                        writepossible = true;
                        //T5 byte[] resulttemp = tcan.readRAM((ushort)GetSymbolAddressSRAM(m_symbols, e.Mapname), (uint)GetSymbolLength(m_symbols, e.Mapname) + 1);
                        int symbolindex = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, e.Mapname), e.Mapname);
                        if (symbolindex >= 0)
                        {
                            //logger.Debug("Reading " + symbolindex.ToString() + " " + e.Mapname);
                            System.Windows.Forms.Application.DoEvents();
                            foreach (SymbolHelper shs in m_symbols)
                            {
                                if (shs.Varname == e.Mapname || shs.Userdescription == e.Mapname)
                                {
                                    try
                                    {
                                        byte[] result = ReadMapFromSRAM(shs, true);
                                        int rows = 0;
                                        int cols = 0;
                                        foreach (DockPanel pnl in dockManager1.Panels)
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
                                                            if ((m_RealtimeConnectedToECU && !m_appSettings.UseNewMapViewer))
                                                            {
                                                                vwr.IsRAMViewer = true;
                                                            }
                                                            else
                                                            {
                                                                vwr.IsRAMViewer = false;
                                                            }

                                                            vwr.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                            if ((m_RealtimeConnectedToECU && m_appSettings.UseNewMapViewer) /*|| m_appSettings.DebugMode*/)
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
                                                    else if (c is DockPanel)
                                                    {
                                                        DockPanel tpnl = (DockPanel)c;
                                                        foreach (Control c2 in tpnl.Controls)
                                                        {
                                                            if (c2 is IMapViewer)
                                                            {
                                                                IMapViewer vwr2 = (IMapViewer)c2;
                                                                if (vwr2.Map_name == e.Mapname)
                                                                {
                                                                    vwr2.Map_content = result;
                                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, e.Mapname, out cols, out rows);
                                                                    if ((m_RealtimeConnectedToECU && !m_appSettings.UseNewMapViewer))
                                                                    {
                                                                        vwr2.IsRAMViewer = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        vwr2.IsRAMViewer = false;
                                                                    }

                                                                    vwr2.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                                    if ((m_RealtimeConnectedToECU && m_appSettings.UseNewMapViewer) /*|| m_appSettings.DebugMode*/)
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
                                                    else if (c is ControlContainer)
                                                    {
                                                        ControlContainer cntr = (ControlContainer)c;
                                                        foreach (Control c3 in cntr.Controls)
                                                        {
                                                            if (c3 is IMapViewer)
                                                            {
                                                                IMapViewer vwr3 = (IMapViewer)c3;
                                                                if (vwr3.Map_name == e.Mapname)
                                                                {
                                                                    vwr3.Map_content = result;
                                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, e.Mapname, out cols, out rows);
                                                                    if ((m_RealtimeConnectedToECU && !m_appSettings.UseNewMapViewer))
                                                                    {
                                                                        vwr3.IsRAMViewer = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        vwr3.IsRAMViewer = false;
                                                                    }

                                                                    vwr3.ShowTable(cols, isSixteenBitTable(e.Mapname));
                                                                    if ((m_RealtimeConnectedToECU && m_appSettings.UseNewMapViewer) /*|| m_appSettings.DebugMode*/)
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
                                        logger.Debug("Refresh viewer with SRAM data error: " + E.Message);
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
                logger.Debug("Failed to read MAP from SRAM: " + E.Message);
            }
            m_prohibitReading = false;
        }

        void tabdet_onWriteToSRAM(object sender, IMapViewer.WriteToSRAMEventArgs e)
        {
            // write data to SRAM, check for valid connection first
            bool writepossible = false;
            try
            {
                //if (flash != null)
                {
                    if (RealtimeCheckAndConnect())
                    {
                        writepossible = true;
                        m_prohibitReading = true;
                        try
                        {
                            WriteMapToSRAM(e.Mapname, e.Data, true);
                        }
                        catch (Exception E)
                        {
                            logger.Debug("Failed to write to SRAM: " + E.Message);
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
                logger.Debug("Failed to write map to SRAM: " + E.Message);
            }
            m_prohibitReading = false;
        }

        void tabdet_onSymbolSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            if (sender is IMapViewer)
            {
                // juiste filename kiezen 
                IMapViewer tabdet = (IMapViewer)sender;
               /* if (e.Filename == m_currentfile)
                {
                    MarkSymbolChanged(e.SymbolName);
                }*/

                string note = string.Empty;
                if (m_appSettings.RequestProjectNotes && m_CurrentWorkingProject != "")
                {
                    //request a small note from the user in which he/she can denote a description of the change
                    frmChangeNote changenote = new frmChangeNote();
                    changenote.ShowDialog();
                    note = changenote.Note;
                }

                savedatatobinary(e.SymbolAddress, e.SymbolLength, e.SymbolDate, e.Filename, true, note);
                UpdateChecksum(e.Filename);
                if (!tabdet.IsRAMViewer && m_RealtimeConnectedToECU) // <GS-12102010> don't refresh from binary when in online mode
                {
                    tabdet.Map_content = readdatafromfile(e.Filename, e.SymbolAddress, e.SymbolLength);
                }
            }
        }

        private void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry)
        {
            if (address > 0 && address < 0x80000)
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

        private void savedatatobinary(int address, int length, byte[] data, string filename, bool DoTransActionEntry, string note)
        {
            if (address > 0 && address < 0x80000)
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
                string dockpanelname4 = "SRAM symbol difference: " + tabdet.Map_name;
                string dockpanelname5 = "Tuning package symbol: " + tabdet.Map_name;
                //string dockpanelname4 = "SRAM <> BIN Compare results: " + Path.GetFileName(tabdet.Filename);
                //else if (dp.Text == "Symbol difference: " + tabdet.Map_name + " [" + Path.GetFileName(tabdet.Filename) + "]")

                foreach (DockPanel dp in dockManager1.Panels)
                {
                    logger.Debug(dp.Text);
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
                    else if (dp.Text.StartsWith(dockpanelname4))
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                    else if (dp.Text.StartsWith(dockpanelname5))
                    {
                        dockManager1.RemovePanel(dp);
                        break;
                    }
                }
            }
        }

        private bool ChecksumMapsPresent()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "ROMChecksum.BottomOffFlash" || sh.Varname == "ROMChecksum.TopOffFlash" || sh.Varname == "RomChecksum.ActualChecksum")
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateChecksum(string m_fileName)
        {
            ChecksumT7.UpdateChecksum(m_fileName, m_appSettings.AutoFixFooter);
        }

        private void RefreshTableViewers()
        {
            for (int i = 0; i < dockManager1.Panels.Count; i++)
            {
                // dockPanel = pnl.Controls.
                if (dockManager1.Panels[i].Text != string.Empty)
                    if ((dockManager1.Panels[i].Text.Substring(0, 7) == "Symbol:"))
                    {
                        bool isVisible = false;
                        if (dockManager1.Panels[i].Visibility == DockVisibility.Visible)
                            isVisible = true;
                        string symName = dockManager1.Panels[i].Text.Split(' ')[1].Trim();
                        dockManager1.Panels[i].Dispose();
                        if (isVisible)
                            StartTableViewer(symName);
                    }
            }

        }

        private void StartTableViewer(string symbolname)
        {
            if (GetSymbolAddress(m_symbols, symbolname) > 0)
            {
                gridViewSymbols.ActiveFilter.Clear(); // clear filter
                gridViewSymbols.ApplyFindFilter("");

                SymbolCollection sc = (SymbolCollection)gridControlSymbols.DataSource;
                int rtel = 0;
                foreach (SymbolHelper sh in sc)
                {
                    if (sh.SmartVarname == symbolname)
                    {
                        try
                        {
                            int rhandle = gridViewSymbols.GetRowHandle(rtel);
                            gridViewSymbols.OptionsSelection.MultiSelect = true;
                            gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                            gridViewSymbols.ClearSelection();
                            gridViewSymbols.SelectRow(rhandle);
                            gridViewSymbols.MakeRowVisible(rhandle, true);
                            gridViewSymbols.FocusedRowHandle = rhandle;

                            if (IsSoftwareOpen()) // <GS-09082010> if it is open software, get data from flash instead of sram
                            {
                                if (m_RealtimeConnectedToECU)
                                {
                                    sh.Symbol_number = GetSymbolNumberFromRealtimeList(sh.Symbol_number, sh.Varname);
                                }
                            }

                            StartTableViewer(ECUMode.Auto);
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
            else
            {
                MessageBox.Show("Symbol " + symbolname + " does not exist in this file");
            }
        }

        private void StartTableViewer(string symbolname, int symbolnumber)
        {
            // TEST SYMBOLNUMBERS
            if (symbolname.StartsWith("Symbol") && symbolnumber > 0)
            {
                foreach (SymbolHelper h in m_symbols)
                {
                    if (h.Symbol_number == symbolnumber)
                    {
                        symbolname = h.Varname;
                    }
                }
            }
            if (GetSymbolAddress(m_symbols, symbolname) > 0)
            {
                gridViewSymbols.ActiveFilter.Clear(); // clear filter

                SymbolCollection sc = (SymbolCollection)gridControlSymbols.DataSource;
                int rtel = 0;
                foreach (SymbolHelper sh in sc)
                {
                    if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                    {
                        try
                        {
                            int rhandle = gridViewSymbols.GetRowHandle(rtel);
                            gridViewSymbols.OptionsSelection.MultiSelect = true;
                            gridViewSymbols.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                            gridViewSymbols.ClearSelection();
                            gridViewSymbols.SelectRow(rhandle);
                            gridViewSymbols.MakeRowVisible(rhandle, true);
                            gridViewSymbols.FocusedRowHandle = rhandle;
                            StartTableViewer(ECUMode.Auto);
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
            else
            {
                frmInfoBox info = new frmInfoBox("Symbol " + symbolname + " does not exist in this file");
            }
        }


        void tabdet_onSymbolSelect(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            if (!e.ShowDiffMap)
            {
                if (SymbolExists(e.SymbolName))
                {
                    StartTableViewer(e.SymbolName, e.Symbolnumber1);
                }
                foreach(SymbolHelper sh in e.Symbols)
                {
                    if (sh.Varname == e.SymbolName || sh.Userdescription == e.SymbolName)
                    {
                        string symName = e.SymbolName;
                        if (e.SymbolName.StartsWith("Symbol") && sh.Userdescription != string.Empty) symName = sh.Userdescription;
                        StartCompareMapViewer(symName, e.Filename, e.SymbolAddress, e.SymbolLength, e.Symbols, e.Symbolnumber2);
                        break;
                    }
                }
            }
            else
            {
                // show difference map
                //TODO: op symbolnumbers doen
                string symName = e.SymbolName;
                if (e.SymbolName.StartsWith("Symbol"))
                {
                    foreach (SymbolHelper sh in e.Symbols)
                    {
                        if (sh.Varname == e.SymbolName)
                        {
                            if (sh.Userdescription != string.Empty)
                            {
                                symName = sh.Userdescription;
                                break;
                            }
                        }
                    }
                }
                StartCompareDifferenceViewer(symName, e.Filename, e.SymbolAddress, e.SymbolLength);
            }
        }

        void tabdet_onSymbolSelectForFind(object sender, CompareResults.SelectSymbolEventArgs e)
        {
            StartTableViewer(e.SymbolName, e.Symbolnumber1);
        }


        private void StartCompareDifferenceViewer(string SymbolName, string Filename, int SymbolAddress, int SymbolLength)
        {
            DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DockPanel pnl in dockManager1.Panels)
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
                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.IsCompareViewer = true;

                    tabdet.Filename = Filename;
                    tabdet.Map_name = SymbolName;
                    //tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                    //tabdet.Map_cat = TranslateSymbolNameToCategory(tabdet.Map_name);
                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
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
                    tabdet.X_axis_name = x_axis_descr;
                    tabdet.Y_axis_name = y_axis_descr;
                    tabdet.Z_axis_name = z_axis_descr;


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
                        byte[] mapdataorig = readdatafromfile(Filename, address, length);
                        byte[] mapdata2 = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, SymbolName), GetSymbolLength(m_symbols, SymbolName));

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
                                    logger.Debug("Byte diff: " + mapdata.GetValue(bt).ToString() + " - " + mapdata2.GetValue(bt).ToString() + " = " + (byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))));
                                    mapdata.SetValue((byte)Math.Abs(((byte)mapdata.GetValue(bt) - (byte)mapdata2.GetValue(bt))), bt);
                                }
                            }

                            tabdet.Map_content = mapdata;
                            tabdet.UseNewCompare = true;
                            tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                            tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                            tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                            tabdet.ShowTable(columns, isSixteenBitTable(SymbolName));
                            TryToAddOpenLoopTables(tabdet);
                            tabdet.Dock = DockStyle.Fill;
                            tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                            tabdet.onSymbolRead += new IMapViewer.NotifyReadSymbol(tabdet_onSymbolRead);
                            tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                            tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                            tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                            tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);

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
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol difference: " + SymbolName) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    foreach (DockPanel pnl in dockManager1.Panels)
                                    {
                                        if ((string)pnl.Tag == m_currentfile && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                dockPanel.DockTo(DockingStyle.Right, 0);
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
                    }
                }
                catch (Exception E)
                {

                    logger.Debug(E.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            VerifyChecksum(true);
        }

        private bool VerifyChecksum(bool showinterface)
        {
            ChecksumResult result = ChecksumT7.VerifyChecksum(m_currentfile, m_appSettings.AutoChecksum, m_appSettings.AutoFixFooter, m_ShouldUpdateChecksum);
            if (result == ChecksumResult.Ok)
            {
                if (showinterface)
                {
                    MessageBox.Show("Checksums verified and all matched!");
                }
                return true;
            }
            else
            {
                if (showinterface)
                {
                    MessageBox.Show("Checksums did not verify ok!");
                }
                return false;
            }
        }

        private bool ShouldUpdateChecksum(string layer, string filechecksum, string realchecksum)
        {
            if (MessageBox.Show("Checksums did not verify ok, do you want to recalculate and update the checksums?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsBinaryBiopower()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "TorqueCal.M_EngMaxE85Tab" || sh.Userdescription == "TorqueCal.M_EngMaxE85Tab")
                {
                    logger.Debug(Path.GetFileNameWithoutExtension(m_currentfile) + " is BioPower");
                    return true;

                }
            }
            return false;
        }

        private bool IsBioPowerEnabled()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "E85Cal.ST_Enable" || sh.Userdescription == "E85Cal.ST_Enable")
                {
                    byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) == 0x00)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsBinaryB308TrionicV6()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "IgnNormCal2.Map" || sh.Userdescription == "IgnNormCal2.Map")
                {
                    return true;
                }
            }
            return false;
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

        private bool HasBinaryTorqueLimiterEnabled()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "TorqueCal.ST_Loop" || sh.Userdescription == "TorqueCal.ST_Loop")
                {
                    byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
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


        private bool HasBinaryTorqueLimiters()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "TorqueCal.ST_Loop" || sh.Userdescription == "TorqueCal.ST_Loop")
                {
                    return true;
                }
            }
            return false;
        }


        private void SetTorqueLimiterEnabled(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "TorqueCal.ST_Loop" || sh.Userdescription == "TorqueCal.ST_Loop")
                {
                    byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)torquelimdata.GetValue(0) == 0x00)
                        {
                            if (enabled)
                            {
                                torquelimdata.SetValue((byte)0x02, 0);
                                savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                UpdateChecksum(m_currentfile);
                            }
                        }
                        else if ((byte)torquelimdata.GetValue(0) != 0x00)
                        {
                            if (!enabled)
                            {
                                torquelimdata.SetValue((byte)0x00, 0);
                                savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                UpdateChecksum(m_currentfile);
                            }
                        }
                    }
                }
            }
        }


        private bool HasBinarySecondLambdaEnabled()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "LambdaCal.ST_AdapEnable" || sh.Userdescription == "LambdaCal.ST_AdapEnable")
                {
                    byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) == 0x00)
                        {
                            return false;
                        }
                    }
                }
                if (sh.Varname == "O2HeatPostCal.I_LowLim" || sh.Userdescription == "O2HeatPostCal.I_LowLim")
                {
                    byte[] ilowlim = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 2)
                    {
                        if ((byte)ilowlim.GetValue(0) == 0x00 && (byte)ilowlim.GetValue(1) == 0x00)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool HasBinarySecondLambdaMap()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "LambdaCal.ST_AdapEnable" || sh.Userdescription == "LambdaCal.ST_AdapEnable")
                {
                    return true;
                }
            }
            return false;
        }

        private void SetSecondLambdaEnabled(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "LambdaCal.ST_AdapEnable" || sh.Userdescription == "LambdaCal.ST_AdapEnable")
                {
                    byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)torquelimdata.GetValue(0) == 0x00)
                        {
                            if (enabled)
                            {
                                torquelimdata.SetValue((byte)0x01, 0);
                                savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                UpdateChecksum(m_currentfile);
                            }
                        }
                        else if ((byte)torquelimdata.GetValue(0) != 0x00)
                        {
                            if (!enabled)
                            {
                                torquelimdata.SetValue((byte)0x00, 0);
                                savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                UpdateChecksum(m_currentfile);
                            }
                        }
                    }
                }
                else if (sh.Varname == "O2HeatPostCal.I_LowLim" || sh.Userdescription == "O2HeatPostCal.I_LowLim")
                {
                    // length = 2
                    byte[] Ilowlim = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 2)
                    {
                        if (enabled)
                        {
                            Ilowlim.SetValue((byte)0, 0);
                            Ilowlim.SetValue((byte)230, 1);
                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, Ilowlim, m_currentfile, true);
                            UpdateChecksum(m_currentfile);
                        }
                        else
                        {
                            Ilowlim.SetValue((byte)0, 0);
                            Ilowlim.SetValue((byte)0, 1);
                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, Ilowlim, m_currentfile, true);
                            UpdateChecksum(m_currentfile);
                        }

                    }
                }
            }
        }

        private bool HasBinaryTipInOutParameters()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname.StartsWith("EngTipCal.") || sh.Userdescription.StartsWith("EngTipCal."))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasBinaryCatalystLightOffParameters()
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname.StartsWith("IgnLOffCal.") || sh.Userdescription.StartsWith("IgnLOffCal."))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckValueIsZero(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) != 0x00)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool HasBinaryCatalystLightOffEnabled()
        {
            if (CheckValueIsZero("IdleCal.ST_EnableLOffRpm") && CheckValueIsZero("IgnLOffCal.ST_Enable"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool HasBinaryEthanolSensorEnabled()
        {
            if (CheckValueIsZero("E85Cal.ST_EthanolSensor") && CheckValueIsZero("E85Cal.ST_EthanolSensor"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SetEthanolSensor(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "E85Cal.ST_EthanolSensor" || sh.Userdescription == "E85Cal.ST_EthanolSensor")
                {
                    if (!enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
            }
        }

        private void SetBioPowerEnabled(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "E85Cal.ST_Enable" || sh.Userdescription == "E85Cal.ST_Enable")
                {
                    if (!enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
            }
        }

        private void SetCatalystLightOff(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "IdleCal.ST_EnableLOffRpm" || sh.Userdescription == "IdleCal.ST_EnableLOffRpm")
                {
                    if (!enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                else if (sh.Varname == "IgnLOffCal.ST_Enable" || sh.Userdescription == "IgnLOffCal.ST_Enable")
                {
                    if (!enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                
            }
        }


        private bool HasBinaryFastThrottleResponse()
        {
            if (CheckValueIsZero("EngTipCal.ST_EnableTipin") && CheckValueIsZero("EngTipCal.ST_EnableTipou"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasBinaryExtraFastThrottleResponse()
        {
            if (CheckValueIsZero("EngTipCal.ST_EnableTipin") && CheckValueIsZero("EngTipCal.ST_EnableActG2") && CheckValueIsZero("EngTipCal.ST_EnableTipou"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetFastThrottleResponse(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "EngTipCal.ST_EnableTipin" || sh.Userdescription == "EngTipCal.ST_EnableTipin")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                else if (sh.Varname == "EngTipCal.ST_EnableActG2" || sh.Userdescription == "EngTipCal.ST_EnableActG2")
                {
                    // always enable
                    byte[] data = new byte[1];
                    data.SetValue((byte)0x01, 0);
                    savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                    UpdateChecksum(m_currentfile);
                }
                else if (sh.Varname == "EngTipCal.ST_EnableTipou" || sh.Userdescription == "EngTipCal.ST_EnableTipou")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                /*else if (sh.Varname == "AngleLimCal.AD_GradLow" || sh.Userdescription == "AngleLimCal.AD_GradLow")
                {
                    if (enabled)
                    {
                        //0x01C2
                        byte[] data = new byte[2];
                        data.SetValue((byte)0x01, 0);
                        data.SetValue((byte)0xC2, 1);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        // 0x02A8
                        byte[] data = new byte[2];
                        data.SetValue((byte)0x02, 0);
                        data.SetValue((byte)0xA8, 1);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }*/
            }
        }


        private void SetActG2(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "EngTipCal.ST_EnableActG2" || sh.Userdescription == "EngTipCal.ST_EnableActG2")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
            }
        }

        private void SetExtraFastThrottleResponse(bool enabled)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "EngTipCal.ST_EnableTipin" || sh.Userdescription == "EngTipCal.ST_EnableTipin")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                else if (sh.Varname == "EngTipCal.ST_EnableActG2" || sh.Userdescription == "EngTipCal.ST_EnableActG2")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                else if (sh.Varname == "EngTipCal.ST_EnableTipou" || sh.Userdescription == "EngTipCal.ST_EnableTipou")
                {
                    if (enabled)
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x00, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        byte[] data = new byte[1];
                        data.SetValue((byte)0x01, 0);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }
                /*else if (sh.Varname == "AngleLimCal.AD_GradLow" || sh.Userdescription == "AngleLimCal.AD_GradLow")
                {
                    if (enabled)
                    {
                        //0x01C2
                        byte[] data = new byte[2];
                        data.SetValue((byte)0x01, 0);
                        data.SetValue((byte)0xC2, 1);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        // 0x02A8
                        byte[] data = new byte[2];
                        data.SetValue((byte)0x02, 0);
                        data.SetValue((byte)0xA8, 1);
                        savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, data, m_currentfile, true);
                        UpdateChecksum(m_currentfile);
                    }
                }*/
            }
        }

        private bool HasBinaryOBDIIMaps()
        {
            bool retval = false;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "OBDCal.OBD2Enabled" || sh.Userdescription == "OBDCal.OBD2Enabled" || sh.Varname == "OBDCal.EOBDEnabled" || sh.Userdescription == "OBDCal.EOBDEnabled" || sh.Varname == "OBDCal.LOBDEnabled" || sh.Userdescription == "OBDCal.LOBDEnabled")
                {
                    /*byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)sh.Flash_start_address, sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)toqruelimdata.GetValue(0) == 0x00)
                        {
                            return false;
                        }
                    }*/
                    retval = true;
                }
            }
            return retval;
        }

        private bool HasBinaryOBDIIEnabled()
        {
            // first figure out whether it is OBD2 or EOBD/LOBD that we're looking for
            bool _lookOBD2 = false;
            bool retval = false;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "OBDCal.OBD2Enabled" || sh.Userdescription == "OBDCal.OBD2Enabled")
                {
                    _lookOBD2 = true;
                }
            }
            if (_lookOBD2)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Varname == "OBDCal.OBD2Enabled" || sh.Userdescription == "OBDCal.OBD2Enabled")
                    {
                        byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                        if (sh.Length == 1)
                        {
                            if ((byte)toqruelimdata.GetValue(0) != 0x00)
                            {
                                retval = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Varname == "OBDCal.EOBDEnabled" || sh.Userdescription == "OBDCal.EOBDEnabled")
                    {
                        byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                        if (sh.Length == 1)
                        {
                            if ((byte)toqruelimdata.GetValue(0) != 0x00)
                            {
                                retval =  true;
                            }
                        }
                    }
                    else if (sh.Varname == "OBDCal.LOBDEnabled" || sh.Userdescription == "OBDCal.LOBDEnabled")
                    {
                        byte[] toqruelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                        if (sh.Length == 1)
                        {
                            if ((byte)toqruelimdata.GetValue(0) != 0x00)
                            {
                                retval = true;
                            }
                        }
                    }
                }
            }
            return retval;
        }


        private void SetOBDIIEnabled(bool enabled)
        {
            bool _lookOBD2 = false;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "OBDCal.OBD2Enabled" || sh.Userdescription == "OBDCal.OBD2Enabled")
                {
                    _lookOBD2 = true;
                }
            }
            if (_lookOBD2)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Varname == "OBDCal.OBD2Enabled" || sh.Userdescription == "OBDCal.OBD2Enabled")
                    {
                        byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                        if (sh.Length == 1)
                        {
                            if ((byte)torquelimdata.GetValue(0) == 0x00)
                            {
                                if (enabled)
                                {
                                    torquelimdata.SetValue((byte)0x01, 0);
                                    savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                    UpdateChecksum(m_currentfile);
                                }
                            }
                            else if ((byte)torquelimdata.GetValue(0) != 0x00)
                            {
                                if (!enabled)
                                {
                                    torquelimdata.SetValue((byte)0x00, 0);
                                    savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                    UpdateChecksum(m_currentfile);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // TODO: ask user for confirmation on which type to set
                System.Windows.Forms.Application.DoEvents();
                bool _setEOBD = false;
                if (enabled)
                {
                    if (MessageBox.Show("Do you want to set the European OBD2 tests active? If you choose No, the generic 'Rest of the world' setting will be set", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _setEOBD = true;
                    }
                    if (_setEOBD)
                    {
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Varname == "OBDCal.EOBDEnabled" || sh.Userdescription == "OBDCal.EOBDEnabled")
                            {
                                byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                                if (sh.Length == 1)
                                {
                                    if ((byte)torquelimdata.GetValue(0) == 0x00)
                                    {
                                        if (enabled)
                                        {
                                            torquelimdata.SetValue((byte)0x01, 0);
                                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                            UpdateChecksum(m_currentfile);
                                        }
                                    }
                                    else if ((byte)torquelimdata.GetValue(0) != 0x00)
                                    {
                                        if (!enabled)
                                        {
                                            torquelimdata.SetValue((byte)0x00, 0);
                                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                            UpdateChecksum(m_currentfile);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Varname == "OBDCal.LOBDEnabled" || sh.Userdescription == "OBDCal.LOBDEnabled")
                            {
                                byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                                if (sh.Length == 1)
                                {
                                    if ((byte)torquelimdata.GetValue(0) == 0x00)
                                    {
                                        if (enabled)
                                        {
                                            torquelimdata.SetValue((byte)0x01, 0);
                                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                            UpdateChecksum(m_currentfile);
                                        }
                                    }
                                    else if ((byte)torquelimdata.GetValue(0) != 0x00)
                                    {
                                        if (!enabled)
                                        {
                                            torquelimdata.SetValue((byte)0x00, 0);
                                            savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                            UpdateChecksum(m_currentfile);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (SymbolHelper sh in m_symbols)
                    {
                        if (sh.Varname == "OBDCal.EOBDEnabled" || sh.Userdescription == "OBDCal.EOBDEnabled" || sh.Varname == "OBDCal.LOBDEnabled" || sh.Userdescription == "OBDCal.LOBDEnabled")
                        {
                            byte[] torquelimdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                            if (sh.Length == 1)
                            {
                                if ((byte)torquelimdata.GetValue(0) != 0x00)
                                {
                                    torquelimdata.SetValue((byte)0x00, 0);
                                    savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), (int)sh.Length, torquelimdata, m_currentfile, true);
                                    UpdateChecksum(m_currentfile);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsBinaryMissingSymbolTable()
        {
            int symcount = 0;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname.StartsWith("Symbolnumber "))
                {
                    symcount++;
                    if (symcount > 10) return true;
                }
            }
            return false; 
        }

        private bool HasBinaryChecksumEnabled()
        {
            // check checksum values
            bool retval = true;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "MapChkCal.ST_Enable" || sh.Varname == "ROM339ChksmCal.ST_Enable" || sh.Varname == "MapChk.ST_Enable")
                {
                    // read data
                    byte[] checksummapdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length);
                    if (sh.Length == 1)
                    {
                        if ((byte)checksummapdata.GetValue(0) == 0x00)
                        {
                            retval = false;
                        }
                    }
                }
                
            }
            if (ChecksumMapsPresent())
            {
                // extra map verification
                // check if maps are equal
                byte[] data1 = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "ROMChecksum.BottomOffFlash"), GetSymbolLength(m_symbols, "ROMChecksum.BottomOffFlash"));
                byte[] data2 = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "ROMChecksum.TopOffFlash"), GetSymbolLength(m_symbols, "ROMChecksum.TopOffFlash"));
                if (data1.Length == data2.Length)
                {
                    bool match = true;
                    for (int i = 0; i < data1.Length; i++)
                    {
                        if ((byte)data1.GetValue(i) != (byte)data2.GetValue(i))
                        {
                            match = false;
                        }
                    }
                    if (match)
                    {
                        retval = false;
                    }
                }
            }
            return retval;
        }

        private bool CheckBytesInFile(string filename, int address, byte value, int length)
        {
            byte[] data2check = readdatafromfile(filename, address, length);
            foreach (byte b2check in data2check)
            {
                if (b2check != value) return false;
            }
            return true;
        }

        private void SetProgrammingDateTime(DateTime dateTime)
        {
            // write date time info into a spare area in the T7 file
            //0x7Fd00
            //readdatafromfile(m_currentfile, 0x7FD00, 1);
            // DAY MONTH YEAR HOUR MIN SEC
            if (!m_appSettings.WriteTimestampInBinary) return; // if option = off, don't write anything!
            byte[] data = new byte[6];
            data.SetValue((byte)dateTime.Day, 0);
            data.SetValue((byte)dateTime.Month, 1);
            data.SetValue((byte)(dateTime.Year - 2000), 2);
            data.SetValue((byte)dateTime.Hour, 3);
            data.SetValue((byte)dateTime.Minute, 4);
            data.SetValue((byte)dateTime.Second, 5);
            savedatatobinary(0x7FD00, 6, data, m_currentfile, true);


        }

        private DateTime GetProgrammingDateTime()
        {
            // read date time info from a spare area in the T7 file
            //0x7Fd00
            byte[] data = readdatafromfile(m_currentfile, 0x7FD00, 6);
            if (data[0] == 0xFF) return DateTime.Now;
            else
            {
                try
                {
                    DateTime retval = new DateTime((int)data[2] + 2000, (int)data[1], (int)data[0], (int)data[3], (int)data[4], (int)data[5]);
                    return retval;
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
            return DateTime.Now;
        }

        private bool IsSymbolInBinary(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname)
                {
                    return true;

                }
            }
            return false;
        }

        private void gridViewSymbols_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gridViewSymbols_DoubleClick(this, EventArgs.Empty);
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(gridViewSymbols.GetFocusedDisplayText());
                e.Handled = true;
            }
        }

        private bool RealtimeCheckAndConnect()
        {
            if (m_RealtimeConnectedToECU)
            {
                return true;
            }
            else
            {
                SetCANStatus("Initializing CANbus interface");
            }

            SetupCanAdapter(Latency.Low);
            if (trionic7.openDevice())
            {
                SetCANStatus("Connected");
                btnConnectDisconnect.Caption = "Disconnect ECU";
                m_RealtimeConnectedToECU = true;
            }
            else
            {
                SetCANStatus("Failed to start KWP session");
                trionic7.Cleanup();
                return false;
            }
            return m_RealtimeConnectedToECU;
        }

        private void SetupCanAdapter(Latency latency)
        {
            trionic7.OnlyPBus = m_appSettings.OnlyPBus;
            trionic7.Latency = latency;

            if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.LAWICEL))
            {
                trionic7.setCANDevice(CANBusAdapter.LAWICEL);
            }
            else if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.COMBI))
            {
                trionic7.setCANDevice(CANBusAdapter.COMBI);
            }
            else if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.ELM327))
            {
                trionic7.ForcedBaudrate = m_appSettings.Baudrate;
                trionic7.setCANDevice(CANBusAdapter.ELM327);
            }
            else if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.JUST4TRIONIC))
            {
                trionic7.ForcedBaudrate = m_appSettings.Baudrate;
                trionic7.setCANDevice(CANBusAdapter.JUST4TRIONIC);
            }
            else if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.KVASER))
            {
                trionic7.setCANDevice(CANBusAdapter.KVASER);
            }
            else if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.J2534))
            {
                trionic7.setCANDevice(CANBusAdapter.J2534);
            }

            if (m_appSettings.Adapter != string.Empty)
            {
                trionic7.SetSelectedAdapter(m_appSettings.Adapter);
            }
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
                case 6:
                    retval = "Airmass limit (pressure guard)";
                    break;
                case 7:
                    retval = "Immobilizer code incorrect";
                    break;
                case 8:
                    retval = "Current to h-bridge to high during throttle limphome";
                    break;
                case 9:
                    retval = "Torque to high during throttle limphome";
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
                    retval = "To high rpm in throttle limp home, pedal potentiometer fault";
                    break;
                case 15:
                    retval = "Torque master fuel cut request";
                    break;
                case 16:
                    retval = "TCM requests fuelcut to smoothen gear shift";
                    break;
                case 20:
                    retval = "Application conditions for fuel cut";
                    break;
            }
            return retval;
        }

        private string ConvertLambdaStatus(int value)
        {
            string retval = value.ToString();
            switch(value)
            {
                case 0:
                    retval = "Closed loop activated";
                    break;
                case 1:
                    retval = "Load too high during a specific time";
                    break;
                case 2:
                    retval = "Load too low";
                    break;
                case 3:
                    retval = "Load too high, no knock";
                    break;
                case 4:
                    retval = "Load too high, knocking";
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
                    retval = "Throttle transient in progress";
                    break;
                case 9:
                    retval = "Throttle transient in progress and low temperature";
                    break;
                case 10:
                    retval = "Fuel cut";
                    break;
                case 11:
                    retval = "Load to high and exhaust temperature algorithm decides it is time to enrich";
                    break;
                case 12:
                    retval = "Diagnostic failure that affects the lambda control";
                    break;
                case 13:
                    retval = "Cloosed loop not enabled";
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
                    retval = "SAI request open loop";
                    break;
                case 18:
                    retval = "Number of combustion to start closed loop has not passed";
                    break;
                case 19:
                    retval = "Lambda integrator is frozen to 0 by SAI lean clamp";
                    break;
                case 20:
                    retval = "Catalyst diagnose for V6 controls the fuel";
                    break;
                case 21:
                    retval = "Gas hybrid active, T7 lambdacontrol stopped";
                    break;
                case 22:
                    retval = "Lambda integrator may not decrease below 0 during start.";
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
                    retval = "Misfire diagnose limit";
                    break;
                case 28:
                    retval = "Brake management";
                    break;
                case 29:
                    retval = "Diff protection (Aut)";
                    break;
                case 31:
                    retval = "Max vehicle speed";
                    break;
                case 40:
                    retval = "LDA request";
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
                    retval = "Max air for lambda 1";
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

        private void LoadLayoutFiles()
        {
            try
            {
                string filename = Path.Combine(configurationFilesPath.FullName, "SymbolViewLayout.xml");

                if (File.Exists(filename))
                {
                    gridViewSymbols.RestoreLayoutFromXml(filename);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void SaveLayoutFiles()
        {
            try
            {
                gridViewSymbols.SaveLayoutToXml(Path.Combine(configurationFilesPath.FullName, "SymbolViewLayout.xml"));
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void LoadRealtimeTable(string filename)
        {
            try
            {
                // create a table from scratch
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
                logger.Debug("Failed to load realtime symbol table: " + E.Message);
            }
        }

        private void SaveRealtimeTable(string filename)
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                    // save the user defined symbols
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (Convert.ToInt32(dr["UserDefined"]) == 1)
                            {
                                sw.WriteLine(dr["SymbolName"].ToString() + "|" + dr["Symbolnumber"].ToString() + "|" + dr["Minimum"].ToString() + "|" + dr["Maximum"].ToString() + "|" + dr["Offset"].ToString() + "|" + dr["Correction"].ToString() + "|" + dr["ConvertedSymbolnumber"].ToString() + "|" + dr["SRAMAddress"].ToString() + "|" + dr["Length"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to write realtime datatable: " + E.Message);
            }
        }


        private void SaveAFRAndCounterMaps()
        {
            try
            {
                if (m_currentfile != string.Empty && m_appSettings.AutoCreateAFRMaps)
                {
                    int cols = 18;
                    int rows = 16;
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                    if (File.Exists(m_currentfile))
                    {
                        m_AFRMap.SaveMap(m_currentfile, cols, rows);
                    }
                }
            }
            catch (Exception stargetE)
            {
                logger.Debug(stargetE.Message);
            }
        }

        private SymbolCollection GetRealtimeNotificationSymbols()
        {
            SymbolCollection _symbols = new SymbolCollection();
            if (m_symbols != null)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Start_address > 0xF00000)
                    {
                        _symbols.Add(sh);
                    }
                }
            }
            return _symbols;
        }

        private void SetupDisplayOptions()
        {
            try
            {
                SetRealtimeListFont(m_appSettings.RealtimeFont);
            }
            catch (Exception fontE)
            {
                logger.Debug(fontE.Message);
            }

            if (m_appSettings.ShowMenu)
            {
                ribbonControl1.Minimized = false;
            }
            if (m_appSettings.ShowAddressesInHex)
            {
                gcSymbolsAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsAddress.DisplayFormat.FormatString = "X6";
                gcSymbolsAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolSRAMAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolSRAMAddress.DisplayFormat.FormatString = "X6";
                gcSymbolSRAMAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
                gcSymbolsLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsLength.DisplayFormat.FormatString = "X6";
                gcSymbolsLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            else
            {
                gcSymbolsAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsAddress.DisplayFormat.FormatString = "";
                gcSymbolsAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolSRAMAddress.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolSRAMAddress.DisplayFormat.FormatString = "";
                gcSymbolSRAMAddress.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
                gcSymbolsLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                gcSymbolsLength.DisplayFormat.FormatString = "";
                gcSymbolsLength.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.Value;
            }
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmAbout about = new frmAbout();
            about.SetInformation("T7Suite v" + System.Windows.Forms.Application.ProductVersion.ToString());
            about.ShowDialog();
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (m_msiUpdater != null)
                {
                    m_msiUpdater.CheckForUpdates("http://develop.trionictuning.com/T7Suite/", "t7suitepro", "T7Suite.msi");
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //T7_manual.pdf with user manual content
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//T7_manual.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//T7_manual.pdf");
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("T7Suite user manual could not be found or opened!");
                }
            }
            catch (Exception E2)
            {
                logger.Debug(E2.Message);
            }
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (File.Exists(System.Windows.Forms.Application.StartupPath + "//Trionic 7.pdf"))
                {
                    System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "//Trionic 7.pdf");
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Trionic 7 documentation could not be found or opened!");
                }
            }
            catch (Exception E2)
            {
                logger.Debug(E2.Message);
            }

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

        private void frmMain_Shown(object sender, EventArgs e)
        {
            try
            {
                m_msiUpdater = new msiupdater(new Version(System.Windows.Forms.Application.ProductVersion));
                m_msiUpdater.Apppath = System.Windows.Forms.Application.UserAppDataPath;
                m_msiUpdater.onDataPump += new msiupdater.DataPump(m_msiUpdater_onDataPump);
                m_msiUpdater.onUpdateProgressChanged += new msiupdater.UpdateProgressChanged(m_msiUpdater_onUpdateProgressChanged);
                m_msiUpdater.CheckForUpdates("http://develop.trionictuning.com/T7Suite/", "t7suitepro", "T7Suite.msi");
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (IsChristmasTime())
            {
                ShowChristmasWish();
            }
            if (m_appSettings.HideSymbolTable)
            {
                dockSymbols.Visibility = DockVisibility.AutoHide;
                dockSymbols.HideImmediately();
            }
        }

        void m_msiUpdater_onUpdateProgressChanged(msiupdater.MSIUpdateProgressEventArgs e)
        {

        }

        private void SetCANStatus(string text)
        {
            barECUNameText.Caption = text;
            System.Windows.Forms.Application.DoEvents();
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
                        trionic7.Cleanup();
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

        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridViewSymbols.ColumnsCustomization();
        }

        private void barButtonItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridViewSymbols.BestFitColumns();
        }

        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel files|*.xls";
            sfd.Title = "Save symbolview to Excel...";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                gridViewSymbols.ExportToXls(sfd.FileName);
            }
        }

        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF files|*.pdf";
            sfd.Title = "Save symbolview to PDF...";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                gridViewSymbols.ExportToPdf(sfd.FileName);
            }
        }

        private byte[] ReadSymbolFromSRAM(uint symbolnumber, string symbolname, uint sramaddress, int length, out bool success)
        {
            byte[] data;
            data = new byte[length];
            data[0] = (byte)0xFF;
            success = false;
            logger.Debug("Getting symbolnumber: " + symbolnumber.ToString() + " symbolname: " + symbolname);
            if (m_RealtimeConnectedToECU)
            {
                if (length <= 4)
                {
                    byte[] retdata = trionic7.ReadValueFromSRAM(sramaddress, length, out success);

                    if (length == 1)
                    {
                        data[0] = retdata[1];
                    }
                    else if (length == 2)
                    {
                        data[0] = retdata[1];
                        data[1] = retdata[2];
                    }
                    else if (length == 3)
                    {
                        data[0] = retdata[1];
                        data[1] = retdata[2];
                        data[2] = retdata[3];
                    }
                    else if (length == 4)
                    {
                        data[0] = retdata[1];
                        data[1] = retdata[2];
                        data[2] = retdata[3];
                        data[3] = retdata[4];
                    }
                }
                else
                {
                    data = trionic7.ReadSymbolNumber(symbolnumber, out success);
                    if (!success)
                    {
                        logger.Debug("Failed getting symbolnumber: " + symbolnumber.ToString() + " symbolname: " + symbolname);
                    }
                }
            }

            return data;
        }

        private void readSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridViewSymbols.FocusedRowHandle >= 0)
            {

                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow(gridViewSymbols.FocusedRowHandle);
                // convert to realtime symbol
                // start an SRAM mapviewer for this symbol
                if (!m_RealtimeConnectedToECU)
                {
                    if (RealtimeCheckAndConnect())
                    {
                        string symName = sh.SmartVarname;
                        int symbolnumber = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, symName), symName);
                        sh.Symbol_number = symbolnumber;

                        logger.Debug("Got symbolnumber: " + symbolnumber.ToString() + " for map: " + symName);
                        if (symbolnumber >= 0)
                        {
                            byte[] result = ReadMapFromSRAM(sh, true);
                            logger.Debug("read " + result.Length.ToString() + " bytes from SRAM!");

                            StartTableViewer(symName);
                            try
                            {
                                int rows = 0;
                                int cols = 0;
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol: ") || pnl.Text.StartsWith("SRAM"))
                                    {
                                        foreach (Control c in pnl.Controls)
                                        {
                                            if (c is IMapViewer)
                                            {
                                                IMapViewer vwr = (IMapViewer)c;
                                                if (vwr.Map_name == symName)
                                                {
                                                    vwr.Map_content = result;
                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, symName, out cols, out rows);
                                                    vwr.ShowTable(cols, isSixteenBitTable(symName));
                                                }
                                            }
                                            else if (c is DockPanel)
                                            {
                                                DockPanel tpnl = (DockPanel)c;
                                                foreach (Control c2 in tpnl.Controls)
                                                {
                                                    if (c2 is IMapViewer)
                                                    {
                                                        IMapViewer vwr2 = (IMapViewer)c2;
                                                        if (vwr2.Map_name == symName)
                                                        {
                                                            vwr2.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, symName, out cols, out rows);
                                                            vwr2.ShowTable(cols, isSixteenBitTable(symName));
                                                        }
                                                    }
                                                }
                                            }
                                            else if (c is ControlContainer)
                                            {
                                                ControlContainer cntr = (ControlContainer)c;
                                                foreach (Control c3 in cntr.Controls)
                                                {
                                                    if (c3 is IMapViewer)
                                                    {
                                                        IMapViewer vwr3 = (IMapViewer)c3;
                                                        if (vwr3.Map_name == symName)
                                                        {
                                                            vwr3.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, symName, out cols, out rows);
                                                            vwr3.ShowTable(cols, isSixteenBitTable(symName));
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
                                logger.Debug("Refresh viewer with SRAM data error: " + E.Message);
                            }

                        }
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to get data from the ECU");
                    }
                }
                else
                {
                    // read symbol from file
                    StartTableViewer(ECUMode.Offline);
                    // update the right viewer with filedata
                }
            }
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

        private void StartAViewer(string symbolname)
        {
            if (m_RealtimeConnectedToECU)
            {
                ShowRealtimeMapFromECU(symbolname);
            }
            else
            {
                StartTableViewer(symbolname);
            }
        }

        private void barButtonItem37_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // E85
            StartAViewer("TorqueCal.M_EngMaxE85Tab");
        }

        private void barButtonItem38_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_ManGearLim");
        }

        private void barButtonItem39_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_5GearLimTab");
        }

        private void barButtonItem40_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_OverBoostTab");
        }

        private void btnTorqueCalM_EngXSP_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_EngXSP");
        }

        private void barButtonItem31_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("BoostCal.RegMap");
        }

        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("BstKnkCal.MaxAirmass");
        }

        private void barButtonItem32_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("FCutCal.m_AirInletLimit");
        }

        private void barButtonItem33_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("MaxSpdCal.n_EngLimAir");
        } 

        private void barButtonItem34_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("MaxVehicCal.v_MaxSpeed");
        }

        private void barButtonItem30_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("PedalMapCal.m_RequestMap");
        }

        private void barButtonItem35_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.m_PedYSP");
        }

        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_EngMaxTab");
        }

        private void barButtonItem36_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_EngMaxAutTab");
        }

        private void barButtonItem95_ItemClick(object sender, ItemClickEventArgs e)
        {
            // E85 and Automatic
            StartAViewer("TorqueCal.M_EngMaxE85TabAut");
        }

        private void barButtonItem53_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("BstKnkCal.MaxAirmassAu");
        }

        private void barButtonItem54_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_CabGearLim");
        }

        private void barButtonItem55_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_NominalMap");
        }

        private void barButtonItem56_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.m_AirTorqMap");
        }

        private void barButtonItem57_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.m_AirXSP");
        }

        private void barButtonItem64_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("IgnNormCal.Map");
        }

        private void barButtonItem27_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("IgnNormCal2.Map");
        }

        private void barButtonItem66_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("IgnKnkCal.IndexMap");
        }

        private void barButtonItem67_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("BFuelCal.Map");
        }

        private void barButtonItem68_ItemClick(object sender, ItemClickEventArgs e)
        {
            // injector constant
            StartAViewer("InjCorrCal.InjectorConst");
        }

        private void barButtonItem69_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (IsBinaryBiopower())
            {
                StartAViewer("BFuelCal.E85Map");
            }
            else
            {
                StartAViewer("BFuelCal.StartMap");
            }
        }

        private void barButtonItem70_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("KnkFuelCal.EnrichmentMap");
        }

        private void barButtonItem71_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("KnkFuelCal.fi_MapMaxOff");
        }

        private void barButtonItem72_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("KnkDetCal.RefFactorMap");
        }

        private void barButtonItem73_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("BoostCal.RegMap");
        }

        private void barButtonItem74_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("TorqueCal.M_OverBoostTab");
        }

        private void barButtonItem75_ItemClick(object sender, ItemClickEventArgs e)
        {
            // p factors
            StartAViewer("BoostCal.PMap");
        }

        private void barButtonItem76_ItemClick(object sender, ItemClickEventArgs e)
        {
            // I factors
            StartAViewer("BoostCal.IMap");
        }

        private void barButtonItem77_ItemClick(object sender, ItemClickEventArgs e)
        {
            // D factors
            StartAViewer("BoostCal.DMap");
        }

        private void barButtonItem65_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("IgnE85Cal.fi_AbsMap");
        }
        
        private int GetClosedIndicatorOffset(out bool IsOpen)
        {
            bool indicatorfound = false;
            int retval = 0;
            IsOpen = false;
            int readstate = 0;
            FileStream fsread = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                int bytecount = 0;
                while (!indicatorfound && (bytecount < m_currentfile_size))
                {
                    byte b = br.ReadByte();
                    switch (readstate)
                    {
                        case 0:
                            if (b == 'Y') readstate = 10;
                            else if (b == 'N') readstate++;
                            break;
                        case 1:
                            if (b == 'o') readstate++;
                            else readstate = 0;
                            break;
                        case 2:
                            if (b == '.' || b == 0x00)
                            {
                                readstate++;
                                indicatorfound = true;
                                IsOpen = true;
                                retval = bytecount - 2;
                            }
                            else
                            {
                                readstate = 0;
                            }
                            break;
                        case 3:
                            readstate = 0;
                            break;
                        case 10:
                            if (b == 'e') readstate++;
                            else readstate = 0;
                            break;
                        case 11:
                            if (b == 's')
                            {
                                readstate++;
                                indicatorfound = true;
                                retval = bytecount - 2;
                                IsOpen = false;
                            }
                            else
                            {
                                readstate = 0;
                            }
                            break;
                    }
                    bytecount++;
                }
            }
            fsread.Close();
            return retval;

        }

        private bool IsBinaryFileOpen()
        {
            // search entire binary for "Yes" or "No."
            // Yes means closed, No. means open
            bool retval = false;
            int offset = GetClosedIndicatorOffset(out retval);
            return retval;

        }

        private void SetBinaryFileOpen()
        {
            bool retval = false;
            int offset = GetClosedIndicatorOffset(out retval);
            if (!retval)
            {
                byte[] nodata = new byte[3];
                if (offset > 0)
                {
                    nodata.SetValue((byte)'N', 0);
                    nodata.SetValue((byte)'o', 1);
                    nodata.SetValue((byte)'.', 2);
                    savedatatobinary(offset, 3, nodata, m_currentfile, true);
                    UpdateChecksum(m_currentfile);
                }
            }
        }

        private void SetBinaryFileClosed()
        {
            bool retval = false;
            int offset = GetClosedIndicatorOffset(out retval);
            if (retval)
            {
                if (offset > 0)
                {
                    byte[] yesdata = new byte[3];
                    yesdata.SetValue((byte)'Y', 0);
                    yesdata.SetValue((byte)'e', 1);
                    yesdata.SetValue((byte)'s', 2);

                    savedatatobinary(offset, 3, yesdata, m_currentfile, true);
                    UpdateChecksum(m_currentfile);
                }
            }
        }

        /*
         * 
Required steps

Step 1 – Increasing airmass request
1a) Alter x axis for TorqueCal.M_NominalMap (=TorqueCal.m_AirXSP) so that the airmass reaches the maximum desired airmass at the last column (e.g. for a stage 4 1400 mg/c)
1b) Alter TorqueCal.m_AirTorqMap so that the maximum torquecolumn requests the desired airmass.
1c) Alter the pedal request Y axis to meet the airmass request. TorqueCal.m_PedYSP
1d) Alter the pedal request map to meet the desired airmass in the top 2 rows (90-100%). PedalMapCal.m_RequestMap

Step 2 – Increase airmass limiter
Increase the airmass limit table to allow for more airmass in the desired areas. BstKnkCal.MaxAirmass.

Step 3 – Increasing engine torque limiters
Up the engine limiters so that the limiter is higher than the maximum torque in the request maps. (TorqueCal.M_EngMaxTab, TorqueCal.M_ManGearLim, TorqueCal.M_CabGearLim, TorqueCal.M_5GearLimTab)

Step 4 – Adapt fuel delivery
4a) You should make sure the fuel supply is good in all ranges by recalibrating BFuelCal.Map. Altering the maximum allowed airmass will also require more fuel. Check this with a wideband O2 sensor.
4b) If you change injectors you should change the injector constant InjCorrCal.InjectorConst and the battery voltage correction table InjCorrCal.BattCorrTab accordingly.

Step 5 – Increase fuel cur level
Increase the fuelcut limit to above the airmass desired. E.g. if you target 1350 mg/c the fuelcut limit should be higher e.g. 1450 or 1500. FCutCal.m_AirInletLimit.

Optional steps

Brake limiter
Remove the brake limiter if desired. TorqueCal.M_BrakeLimit. You can set this to the same values as the engine limiter.

Disable read lambda sensor
If you don’t want the rear O2 sensor to be tested you can alter the values in 
O2SensPostCal.T_EngLim (minimum engine temp to allow test) and O2SensPostCal.CoolTempLi (maximum enginetemp to allow test) to an impossible value e.g. 130.
If you do this you should also disable adaption based on rear O2 sensor readings by setting LambdaCal.ST_AdapEnable to 0.

Disable misfire detection
Optionally you can disable the misfire detection algorithm by setting MissfCal.DetectLoadLevel to an airmass beyond the requested airmass (e.g. 1700 mg/c) in the entire table.

Increase vehicle speed limiter
You can increase the maximum vehicle speed limiter to better suit your needs.
Normally MaxVehicCal.v_MaxSpeed is set to 2400 (240km/h) and you can change it to e.g. 2900 (290 km/h).

Increase engine speed limiter (RPM limiter)
You can increase the RPM limiter if you please. MaxSpdCal.n_EngLimAir will tell you at what rpm the torquelimiter will act. You can increase each cell to e.g. 6300 rpm.

Disable checksum verification
Optionally you can disable the checksum routine on the calibration data. In this case you must set MapChkCal.ST_Enable to 0 in stead of 1.

Boost regulation error supression
If boost regulation reports errors you can increase the difference between boost pressure and requested pressure above which a fault report is generated. BoostDiagCal.m_FaultDiff. Set to 200 in stead of 100 for example.         
         * * */

        private int GetMaxTorque()
        {
            int retval = 300;
            /*byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(m_symbols, "TorqueCal.M_EngXSP"));
            retval = Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 2)) * 256;
            retval += Convert.ToInt32(torquenominalx.GetValue(torquenominalx.Length - 1));
            logger.Debug("Max torque from table = " + retval.ToString());*/
            return retval;
        }

        private void TuneToStageNew(int stage, double maxairmass, double maxtorque, double peakHP, EngineType enginetype, bool E85)
        {
            frmProgress progress = new frmProgress();
            progress.Show();
            progress.SetProgress("Checking current configuration...");
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
            AddToResumeTable("Tuning your binary to stage: " + stage.ToString());

            progress.SetProgress("Creating backup file...");
            int imaxairmass = Convert.ToInt32(maxairmass);
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin", true);
            AddToResumeTable("Backup file created (" + Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-beforetuningto-" + imaxairmass.ToString() + "-mg.bin" + ")");

            // tune maps
            //TorqueCal.M_EngXSP
            progress.SetProgress("Tuning TorqueCal.M_EngXSP...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP") > 0)
            {
                byte[] TorqCal = GetTorqueCalEngineSupportPoints(Convert.ToInt32(maxairmass));

                //<GS-11042011> leave the left three columns out

                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP") + 6, GetSymbolLength(m_symbols, "TorqueCal.M_EngXSP") - 6, TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned x axis for torque calibration map (TorqueCal.M_EngXSP)");
            }
            UpdateChecksum(m_currentfile);

            /* maximum allowable airmass per rpm site to protect turbine (overrev protection) */
            progress.SetProgress("Tuning LimEngCal.TurboSpeedTab...");
            if ((int)GetSymbolAddress(m_symbols, "LimEngCal.TurboSpeedTab") > 0)
            {
                // fill the entire table with maxairmass * 1.1
                double max_airmassTurboSpeed = maxairmass * 1.1;
                byte[] TorqCal = GetTurboSpeedLimiter(Convert.ToInt32(max_airmassTurboSpeed), GetSymbolLength(m_symbols, "LimEngCal.TurboSpeedTab")/2);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "LimEngCal.TurboSpeedTab"), GetSymbolLength(m_symbols, "LimEngCal.TurboSpeedTab"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned turbo speed limiter (LimEngCal.TurboSpeedTab)");
            }
            UpdateChecksum(m_currentfile);

            progress.SetProgress("Tuning LimEngCal.TurboSpeedTab2...");
            if ((int)GetSymbolAddress(m_symbols, "LimEngCal.TurboSpeedTab2") > 0)
            {
                // fill the entire table with 1000 (= correction factor 1)
                byte[] TorqCal = GetTurboSpeedLimiter(1000, GetSymbolLength(m_symbols, "LimEngCal.TurboSpeedTab2") / 2);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "LimEngCal.TurboSpeedTab2"), GetSymbolLength(m_symbols, "LimEngCal.TurboSpeedTab2"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned turbo speed limiter correction factors (LimEngCal.TurboSpeedTab2)");
            }
            UpdateChecksum(m_currentfile);

            /* APC error trigger airmass maximum */
            progress.SetProgress("Tuning BoosDiagCal.ErrMaxMReq...");
            if ((int)GetSymbolAddress(m_symbols, "BoosDiagCal.ErrMaxMReq") > 0)
            {
                // fill the entire table with maxairmass * 1.1
                double max_airmassTurboSpeed = maxairmass;
                byte[] TorqCal = GetTurboSpeedLimiter(Convert.ToInt32(max_airmassTurboSpeed), GetSymbolLength(m_symbols, "BoosDiagCal.ErrMaxMReq") / 2);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BoosDiagCal.ErrMaxMReq"), GetSymbolLength(m_symbols, "BoosDiagCal.ErrMaxMReq"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned APC error triggered maximum airmass (BoosDiagCal.ErrMaxMReq)");
            }
            UpdateChecksum(m_currentfile);

            //BoosDiagCal.p_BefLimit = 2.00 bar (absolute pressure) [ 2000 ]
            //BoosDiagCal.p_BefOkLimit = 1.60bar (absolute pressure) [ 1600 ]
            // set to 3000 both (2.0 bar boost)???

            /* maximum airmass per that lets I factor grow */
            progress.SetProgress("Tuning AirCtrlCal.m_MaxAirTab...");
            if ((int)GetSymbolAddress(m_symbols, "AirCtrlCal.m_MaxAirTab") > 0)
            {
                // fill the entire table with maxairmass * 1.1
                double max_airmassTurboSpeed = maxairmass * 1.1;
                byte[] TorqCal = GetTurboSpeedLimiter(Convert.ToInt32(max_airmassTurboSpeed), GetSymbolLength(m_symbols, "AirCtrlCal.m_MaxAirTab") / 2);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "AirCtrlCal.m_MaxAirTab"), GetSymbolLength(m_symbols, "AirCtrlCal.m_MaxAirTab"), TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned I factor airmass limiter (AirCtrlCal.m_MaxAirTab)");
            }
            UpdateChecksum(m_currentfile);

            /*
            progress.SetProgress("Tuning X_AccPedalAutSP...");
            if ((int)GetSymbolAddress(m_symbols, "X_AccPedalAutSP") > 0)
            {
                byte[] TorqCal = GetAutAccelPedalSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "X_AccPedalAutSP"), GetSymbolLength(m_symbols, "X_AccPedalAutSP"), TorqCal, m_currentfile);
                AddToResumeTable("Tuned x axis for accelerator pedal map (AUT) (X_AccPedalAutSP)");
            }
            UpdateChecksum(m_currentfile);
            */

            //step 1a) Alter x axis for TorqueCal.M_NominalMap (=TorqueCal.m_AirXSP) so that the airmass 
            // reaches the maximum desired airmass at the last column (e.g. for a stage 4 1400 mg/c)
            progress.SetProgress("Tuning TorqueCal.m_AirXSP...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP") > 0)
            {
                byte[] TorqCal = GetTorqueCalSupportPoints(Convert.ToInt32(maxairmass));

                //<GS-11042011> leave the left three columns out

                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP") + 6, GetSymbolLength(m_symbols, "TorqueCal.m_AirXSP") - 6, TorqCal, m_currentfile, true);
                AddToResumeTable("Tuned x axis for nominal torquemap (TorqueCal.m_AirXSP)");
            }
            UpdateChecksum(m_currentfile);

            /*** TorqueCal.M_NominalMap ***/
            /*** Data-matrix for nominal Torque. Engine speed and airmass are used as support points. 
            The value in the matrix will be the engine output torque when inlet airmass (- friction airmass) 
            is used together with actual engine speed as pointers ***/
            // formula = replace last column with estimated values of max_torque (= last column * 1.3)
            int max_torque = GetMaxTorque();
            
            if ((int)GetSymbolAddress(m_symbols, "BoostCal.RegMap") > 0)
            {
                byte[] BoostMap = GetBoostRegMap(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BoostCal.RegMap"), GetSymbolLength(m_symbols, "BoostCal.RegMap"), BoostMap, m_currentfile, true);
                AddToResumeTable("Tuned boost calibration map (BoostCal.RegMap)");
            }
            UpdateChecksum(m_currentfile);
            // now also do BoostCal.SetLoadXSP (!!!) mg/c 8 values
            // run from 500 upto maxairmass
            if ((int)GetSymbolAddress(m_symbols, "BoostCal.SetLoadXSP") > 0)
            {
                byte[] BoostMap = GetBoostMapSupportPoints(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BoostCal.SetLoadXSP"), GetSymbolLength(m_symbols, "BoostCal.SetLoadXSP"), BoostMap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map x axis (BoostCal.SetLoadXSP)");
            }
            UpdateChecksum(m_currentfile);

            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap") > 0)
            {
                byte[] nominalMap = GetNominalTorqueMap(Convert.ToInt32(maxairmass));

                //<GS-11042011> leave the left three columns out
                int baseAddress = (int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap");
                for (int iRow = 0; iRow < 16; iRow++)
                {
                    byte[] nominalMapPart = new byte[30];
                    for (int iColumn = 0; iColumn < 30; iColumn++)
                    {
                        nominalMapPart[iColumn] = nominalMap[(iRow * 36) + 6 + iColumn];
                    }
                    savedatatobinary(baseAddress + (iRow * 36) + 6, 30, nominalMapPart, m_currentfile, true);
                }
                //savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(m_symbols, "TorqueCal.M_NominalMap"), nominalMap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map (TorqueCal.M_NominalMap)");
            }
            UpdateChecksum(m_currentfile);

            // step 1b) Alter TorqueCal.m_AirTorqMap so that the maximum torquecolumn requests the desired airmass.
            /*** TorqueCal.m_AirTorqMap ***/
            /*** Data-matrix for nominal airmass. Engine speed and torque are used as support points. 
            The value in the matrix + friction airmass (idle airmass) will create the pointed torque at the pointed engine speed. 
            Resolution is   1 mg/c. ***/
            progress.SetProgress("Tuning TorqueCal.m_AirTorqMap...");
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap") > 0)
            {
                byte[] AirTorqueCalibration = GetAirTorqueCalibration(Convert.ToInt32(maxairmass));

                //<GS-11042011> leave the left three columns out
                int baseAddress = (int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap");
                for (int iRow = 0; iRow < 16; iRow++)
                {
                    byte[] nominalMapPart = new byte[26];
                    for (int iColumn = 0; iColumn < 26; iColumn++)
                    {
                        nominalMapPart[iColumn] = AirTorqueCalibration[(iRow * 32) + 6 + iColumn];
                    }
                    savedatatobinary(baseAddress + (iRow * 32) + 6, 26, nominalMapPart, m_currentfile, true);
                }
                //savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TorqueCal.m_AirTorqMap"), AirTorqueCalibration, m_currentfile, true);
                AddToResumeTable("Tuned nominal airmass map (TorqueCal.m_AirTorqMap)");
            }
            // update the checksum
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning TorqueCal.m_PedYSP...");
            //TODO: PedelMap Y axis with fixed data!
            // step 1c) Alter the pedal request Y axis to meet the airmass request. TorqueCal.m_PedYSP
            /********** TorqueCal.m_PedYSP ***********/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP") > 0)
            {
                byte[] pedalmapysp = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_PedYSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_PedYSP")) rows /= 2;
               // row > 5 and col > 4
                int ioffset1 = ((rows-4) * cols * 2);
                int ioffset2 = ((rows-4) * cols * 2) + 1;
                int pos4value = Convert.ToInt32(pedalmapysp[ioffset1]) * 256 + Convert.ToInt32(pedalmapysp[ioffset2]);

                int airmassperstep = (Convert.ToInt32(maxairmass) - pos4value)/3;
                int step = 1;
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(pedalmapysp[offset1]) * 256 + Convert.ToInt32(pedalmapysp[offset2]);
                    /*if (rt == rows - 3)
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
                    }*/
                    boostcalvalue = pos4value + (airmassperstep * step);
                    step++;
                    if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    pedalmapysp[offset1] = b1;
                    pedalmapysp[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"), pedalmapysp, m_currentfile, true);
                AddToResumeTable("Tuned airmass pedalmap y axis (TorqueCal.m_PedYSP)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning PedalMapCal.m_RequestMap...");


            // step 1d) Alter the pedal request map to meet the desired airmass in the top 2 rows (90-100%). PedalMapCal.m_RequestMap
            /********** PedalMapCal.m_RequestMap ***********/
            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap") > 0)
            {
                byte[] ReferencePedalMap = GetPedalMap(Convert.ToInt32(maxairmass), Convert.ToInt32(peakHP), E85);
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.m_RequestMap"), ReferencePedalMap, m_currentfile, true);
                AddToResumeTable("Tuned airmass request map (PedalMapCal.m_RequestMap)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning BstKnkCal.MaxAirmass...");

            // step 2) Increase the airmass limit table to allow for more airmass in the desired areas. BstKnkCal.MaxAirmass.
            /********** BstKnkCal.MaxAirmass ***********/
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass") > 0)
            {
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for manual transmission (BstKnkCal.MaxAirmass)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning BstKnkCal.MaxAirmassAu...");
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu") > 0)
            {
                /********** BstKnkCal.MaxAirmassAu ***********/
                byte[] AirmassLimiter = GetAirmassLimiter(Convert.ToInt32(maxairmass));
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"), AirmassLimiter, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for automatic transmission (BstKnkCal.MaxAirmassAu)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning TorqueCal.M_EngMaxAutTab...");

            // step 3 – Increasing engine torque limiters. Up the engine limiters so that the limiter 
            // is higher than the maximum torque in the request maps. 
            // (TorqueCal.M_EngMaxTab, TorqueCal.M_ManGearLim, TorqueCal.M_CabGearLim, TorqueCal.M_5GearLimTab)
            /********** TorqueCal.M_EngMaxAutTab ***********/
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
                    boostcalvalue = 320;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxAutTab"), maxtorqueaut, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for automatic transmission (TorqueCal.M_EngMaxAutTab)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning TorqueCal.M_EngMaxTab...");
            /********** TorqueCal.M_EngMaxTab ***********/
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
                    boostcalvalue = 320;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxTab"), maxtorquetab, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission (TorqueCal.M_EngMaxTab)");
            }
            UpdateChecksum(m_currentfile);
            progress.SetProgress("Tuning TorqueCal.M_EngMaxE85Tab...");
            /********** TorqueCal.M_EndMaxE85Tab ***********/
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
                    boostcalvalue = 320;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxE85Tab"), maxtorquetab, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission on E85 fuel (TorqueCal.M_EngMaxE85Tab)");
            }
            UpdateChecksum(m_currentfile);
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
                    if (rt <= 1)
                    {
                        boostcalvalue = 200;
                    }
                    else
                    {
                        boostcalvalue = 320;
                    }

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_ManGearLim"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission gears (TorqueCal.M_ManGearLim)");
            }
            UpdateChecksum(m_currentfile);
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
                    if (rt <= 1)
                    {
                        boostcalvalue = 200;
                    }
                    else
                    {
                        boostcalvalue = 320;
                    }

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_CabGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_CabGearLim"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for cabrialet cars (TorqueCal.M_CabGearLim)");
            }
            UpdateChecksum(m_currentfile);
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
                    boostcalvalue = 320;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(m_symbols, "TorqueCal.M_5GearLimTab"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission in 5th gear (TorqueCal.M_5GearLimTab)");
            }
            UpdateChecksum(m_currentfile);

            // step 4 – Adapt fuel delivery.
            // step 4a) You should make sure the fuel supply is good in all ranges by recalibrating BFuelCal.Map. Altering the maximum allowed airmass will also require more fuel. Check this with a wideband O2 sensor.
            // step 4b) If you change injectors you should change the injector constant InjCorrCal.InjectorConst and the battery voltage correction table InjCorrCal.BattCorrTab accordingly.

            //Step 5 – Increase fuel cur level
            // Increase the fuelcut limit to above the airmass desired. E.g. if you target 1350 mg/c the fuelcut limit should be higher e.g. 1450 or 1500. FCutCal.m_AirInletLimit.

            /********** FCutCal.m_AirInletLimit ***********/

            progress.SetProgress("Tuning FCutCal.m_AirInletLimit...");
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
                    byte b1 = Convert.ToByte(i_AirmassFuelcut / 256);
                    byte b2 = Convert.ToByte(i_AirmassFuelcut - (int)b1 * 256);
                    fuelcutmap[0] = b1;
                    fuelcutmap[1] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned fuelcut limiter (FCutCal.m_AirInletLimit)");
            }
            UpdateChecksum(m_currentfile);

            //Boost regulation error supression
            //If boost regulation reports errors you can increase the difference between boost pressure and requested pressure above which a fault report is generated. BoostDiagCal.m_FaultDiff. Set to 200 in stead of 100 for example.         
            progress.SetProgress("Tuning BoosDiagCal.m_FaultDiff...");

            if ((int)GetSymbolAddress(m_symbols, "BoosDiagCal.m_FaultDiff") > 0)
            {
                byte[] fuelcutmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BoosDiagCal.m_FaultDiff"), GetSymbolLength(m_symbols, "BoosDiagCal.m_FaultDiff"));
                if (fuelcutmap.Length == 2)
                {
                    fuelcutmap[0] = 0x00;
                    fuelcutmap[1] = 0xC8;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BoosDiagCal.m_FaultDiff"), GetSymbolLength(m_symbols, "BoosDiagCal.m_FaultDiff"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned boost fault indication level (BoosDiagCal.m_FaultDiff)");
            }
            UpdateChecksum(m_currentfile);

            // mark binary as tuned to stage I

            //AddToResumeTable("Updated binary description with tuned stage");
            progress.SetProgress("Generating report...");


            AddToResumeTable("Updated checksum.");

            progress.Close();

            // refresh open viewers

        }

        private void barButtonItem41_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (m_currentfile != "")
            {
                openFileDialog1.Multiselect = false;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    frmBinCompare bincomp = new frmBinCompare();
                    //bincomp.SetSymbolCollection(m_symbols);
                    //bincomp.OutsideSymbolBoundary = true;
                    bincomp.SetCurrentFilename(m_currentfile);
                    bincomp.SetCompareFilename(openFileDialog1.FileName);
                    bincomp.CompareFiles();
                    bincomp.ShowDialog();
                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No file is currently opened, you need to open a binary file first to compare it to another one!");
            }
        }

        private void SetToolstripTheme()
        {
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
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

        }

        void InitSkins()
        {
            ribbonControl1.ForceInitialize();
            //barManager1.ForceInitialize();
            BarButtonItem item;

            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.BonusSkins).Assembly);
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.OfficeSkins).Assembly);

            foreach (DevExpress.Skins.SkinContainer cnt in DevExpress.Skins.SkinManager.Default.Skins)
            {
                item = new BarButtonItem();
                item.Caption = cnt.SkinName;
                //iPaintStyle.AddItem(item);
                ribbonPageGroup16.ItemLinks.Add(item);
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
                logger.Debug(E.Message);
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
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

                switch (sh.Varname)
                {
                    case "Torque.M_MaxEngAndGear":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "BFuelProt.t_InjActual":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 0.001, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "ActualIn.v_Vehicle2":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "In.v_Vehicle":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 255, 0, 0.1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "ActualIn.U_LambdaCat":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 1200, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "ActualIn.U_LambdaEng":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 1200, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "TorqueProt.m_AirTMasLim":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 1800, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
                    case "AirctlData.Actual":
                        AddSymbolToRealTimeList(symName, sh.Symbol_number, 0, 1800, 0, 1, sh.Description, (uint)GetSymbolAddressSRAM(m_symbols, symName), true);
                        break;
/*                    case "KnkDet.KnockCyl":         // 8 length
                        break;
                    case "KnkDetAdap.KnkCntCyl":    // 8 length
                        break;
                    case "MissfAdap.MissfCntCyl": // 12 length
                        break;
                    //TODO: add misfire per cylinder 
                        //TODO: add knock per cylinder
 * */

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

        private void AddSymbolToRealTimeList(string symbolname, int symbolnumber, double minvalue, double maxvalue, double offset, double correction, string description, uint sramaddress, bool isUserDefined)
        {
            try
            {
                if (gridRealtime.DataSource != null)
                {
                    int userdef = 0;
                    if(isUserDefined) userdef = 1;
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
                logger.Debug("Failed to add symbol to realtime list: "+ E.Message);
            }
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmEditRealtimeSymbol frmeditsymbol = new frmEditRealtimeSymbol();
            frmeditsymbol.Symbols = m_symbols;
            if (frmeditsymbol.ShowDialog() == DialogResult.OK)
            {
                AddSymbolToRealTimeList(frmeditsymbol.Varname, frmeditsymbol.Symbolnumber, frmeditsymbol.MinimumValue, frmeditsymbol.MaximumValue, frmeditsymbol.OffsetValue, frmeditsymbol.CorrectionValue, frmeditsymbol.Description, (uint)GetSymbolAddressSRAM(m_symbols, frmeditsymbol.Varname), true);
            }
        }

        private void btnToggleRealtimePanel_ItemClick(object sender, ItemClickEventArgs e)
        {
            ToggleRealtimePanel();
        }

        private void ToggleRealtimePanel()
        {
            if (dockRealtime.Visibility == DockVisibility.Visible)
            {
                dockRealtime.Visibility = DockVisibility.Hidden;
                tmrRealtime.Enabled = false;
                m_enableRealtimeTimer = false;
                trionic7.ResumeAlivePolling();

                if (m_appSettings.UseDigitalWidebandLambda)
                {
                    if (wbReader != null)
                    {
                        wbReader.Stop();
                    }
                    wbFactory = null;
                    wbReader = null;
                }
            }
            else
            {
                trionic7.SuspendAlivePolling();
                m_enableRealtimeTimer = true;

                if (m_appSettings.ResetRealtimeSymbolOnTabPageSwitch)
                {
                    FillRealtimeTable(MonitorType.Dashboard); // default
                }
                // check for Performance.Mode presence
                if (PerformanceModePresent())
                {
                    btnEconomyMode.Visible = true;
                    btnNormalMode.Visible = true;
                    btnSportMode.Visible = true;
                }
                else
                {
                    btnEconomyMode.Visible = false;
                    btnNormalMode.Visible = false;
                    btnSportMode.Visible = false;

                }
                if (RealtimeCheckAndConnect())
                {
                    tmrRealtime.Enabled = m_enableRealtimeTimer;
                }
                dockRealtime.Visibility = DockVisibility.Visible;
                int width = dockManager1.Form.ClientSize.Width - dockSymbols.Width;
                int height = dockManager1.Form.ClientSize.Height;
                if (width > 660) width = 660;

                dockRealtime.Dock = DockingStyle.Left;
                dockRealtime.Width = width;

                if(m_appSettings.UseDigitalWidebandLambda)
                {
                    try
                    {
                        wbFactory = new WidebandFactory(m_appSettings.WidebandDevice, m_appSettings.WbPort, false);
                        wbReader = wbFactory.CreateInstance();
                        wbReader.Start();
                    }
                    catch (Exception ex) 
                    {
                        wbFactory = null;
                        wbReader = null;
                        MessageBox.Show(ex.Message, "Wideband error", MessageBoxButtons.OK);
                    }
                }

                // set default skin
                SwitchRealtimePanelMode(m_appSettings.Panelmode);
            }
        }

        private void SetPerformanceMode(int mode)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "Performance.Mode" || sh.Userdescription == "Performance.Mode")
                {
                    if (sh.Start_address > 0)
                    {
                        //TODO: write it to sram
                        byte[] dataToSend = new byte[sh.Length];
                        dataToSend.Initialize();
                        dataToSend.SetValue((byte)mode, sh.Length - 1);
                        //WriteMapToSRAM("Performance.Mode", data);
                        uint addresstowrite = (uint)sh.Start_address;
                        if (!trionic7.WriteMapToSRAM(addresstowrite, dataToSend))
                        {
                            logger.Debug("Failed to write data to the ECU");
                        }
                        break;
                    }
                }
            }
        }

        private bool PerformanceModePresent()
        {
            bool retval = false;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "Performance.Mode" || sh.Userdescription == "Performance.Mode")
                {
                    if (sh.Start_address > 0)
                    {
                        retval = true;
                    }

                }
            }
            return retval;
        }

        public void readSymbolTable(string a_fileName)
        {
            SymbolMapParser m_parser = new SymbolMapParser();
            m_parser.parse(a_fileName);
            List<Symbol> symbolList = m_parser.getSymbolList();
            
            int idx = 0;
            if (!m_parser.TableContainsStrings)
            {
                foreach (Symbol symbol in symbolList)
                {
                    if (m_realtimeAddresses != null)
                    {
                        m_realtimeAddresses.Rows.Add(symbol.getSymbolName(), idx++);
                    }
                    uint sramaddress = symbol.getRAMAddress();
                    uint flashaddress = symbol.getROMAddress();
                    if (sramaddress > 0)
                    {
                        bool sramfnd = false;
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Start_address == sramaddress)
                            {
                                sramfnd = true;
                                sh.Symbol_number_ECU = idx-1;
                                m_realtimeAddresses.Rows[m_realtimeAddresses.Rows.Count - 1]["VarName"] = sh.Varname;
                                //logger.Debug("SRAM Symbol: " + symbol.getSymbolName() + " / " + sh.Varname + "[" + sh.Symbol_number.ToString() + "]" + " addr: " + symbol.getROMAddress().ToString("X8") + " sram: " + symbol.getRAMAddress().ToString("X8") + " len: " + symbol.getDataLength().ToString() + " index: " + idx.ToString());
                                break;
                            }
                        }
                        if (!sramfnd)
                        {
                            //logger.Debug("************ NOT FOUND SRAM Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString("X8") + " sram: " + symbol.getRAMAddress().ToString("X8") + " len: " + symbol.getDataLength().ToString() + " index: " + idx.ToString());
                        }
                    }
                    else if (flashaddress > 0)
                    {
                        bool flashfnd = false;
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Flash_start_address == flashaddress)
                            {
                                flashfnd = true;
                                sh.Symbol_number_ECU = idx-1;
                                m_realtimeAddresses.Rows[m_realtimeAddresses.Rows.Count - 1]["VarName"] = sh.Varname;
                                //logger.Debug("ROM Symbol: " + symbol.getSymbolName() + " / " + sh.Varname + "[" + sh.Symbol_number.ToString() + "]" + " addr: " + symbol.getROMAddress().ToString("X8") + " sram: " + symbol.getRAMAddress().ToString("X8") + " len: " + symbol.getDataLength().ToString() + " index: " + idx.ToString());
                                break;
                            }
                        }
                        if (!flashfnd)
                        {
                            //logger.Debug("************ NOT FOUND ROM Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString("X8") + " sram: " + symbol.getRAMAddress().ToString("X8") + " len: " + symbol.getDataLength().ToString() + " index: " + idx.ToString());
                        }
                    }
                    //logger.Debug("Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString() + " sram: " + symbol.getRAMAddress().ToString());
                }
            }
            else
            {
                foreach (Symbol symbol in symbolList)
                {
                    if (m_realtimeAddresses != null)
                    {
                        m_realtimeAddresses.Rows.Add(symbol.getSymbolName(), idx++);
                        /*if (symbol.getSymbolName() == "BFuelCal.Map")
                        {
                            logger.Debug("Seen BFuelCal.Map");
                            logger.Debug("Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString() + " sram: " + symbol.getRAMAddress().ToString() + " idx: " + idx.ToString());
                        }
                        if (symbol.getSymbolName() == "ActualIn.U_Batt")
                        {
                            logger.Debug("Seen ActualIn.U_Batt");
                            logger.Debug("Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString() + " sram: " + symbol.getRAMAddress().ToString() + " idx: " + idx.ToString());
                        }*/

                    }
                    uint sramaddress = symbol.getRAMAddress();
                    uint flashaddress = symbol.getROMAddress();
                    if (sramaddress > 0)
                    {
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Varname == symbol.getSymbolName())
                            {

                                // match!
                                m_realtimeAddresses.Rows[m_realtimeAddresses.Rows.Count - 1]["VarName"] = sh.Varname;
                                sh.Start_address = sramaddress;
                                sh.Symbol_number_ECU = idx-1;
                                if (symbol.getSymbolName() == "ActualIn.U_Batt")
                                {
                                    logger.Debug("Seen ActualIn.U_Batt");
                                    logger.Debug("Symbol: " + sh.Varname + " addr: " + sh.Flash_start_address.ToString() + " sram: " + sh.Start_address.ToString() + " idx: " + idx.ToString());
                                }

                                break;
                            }
                        }
                    }
                    else if (flashaddress > 0)
                    {
                        logger.Debug("Symbol: " + symbol.getSymbolName() + " addr: " + symbol.getROMAddress().ToString() + " sram: " + symbol.getRAMAddress().ToString());
                    }
                }
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
                                    //logger.Debug("Fetched (" + symbolname + ") number from realtime list: " + retval.ToString());
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return retval;

        }

        private void GetSRAMVarsFromTable()
        {
            if (gridRealtime.DataSource != null)
            {
                _sw.Reset();
                _sw.Start();
                DateTime datet = DateTime.Now;
                System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                

                // run the Realtimelist and update the values
                foreach (DataRow dr in dt.Rows)
                {
                    
                    double value = 0;
                    string symbolName = dr["SymbolName"].ToString();
                    uint symbolnumber = Convert.ToUInt32(dr["ConvertedSymbolnumber"]);
                    uint sRAMAddress = Convert.ToUInt32(dr["SRAMAddress"]);
                    int varLength = Convert.ToInt32(dr["Length"]);                    
                    
                    if (m_prohibitReading)
                    {
                        logger.Debug("prohibitreading");
                        return;
                    }
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
                            logger.Debug("Delay value was null for : " + dr["SymbolName"].ToString());
                        }
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to reload: " + E.Message);
                    }
                    
                    if (symbolnumber > 0)
                    {
                        byte[] buffer;

                        bool _success = false;
                        
                        buffer = ReadSymbolFromSRAM(symbolnumber, symbolName, sRAMAddress, varLength, out _success);
                        
                        if (_success)
                        {
                            if (buffer.Length == 1)
                            {
                                //logger.Debug("Buffer received: " + buffer[0].ToString("X2"));
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
                                symbolName == "Out.M_Engine" ||
                                symbolName == "ECMStat.P_Engine" ||
                                symbolName == "IgnProt.fi_Offset" ||
                                symbolName == "Lambda.LambdaInt" ||
                                symbolName == "MAF.m_AirInlet" ||
                                symbolName == "AdpFuelProt.MulFuelAdapt" ||
                                symbolName == "ECMStat.p_Diff" ||
                                symbolName == "BoostProt.PFac" ||
                                symbolName == "BoostProt.IFac" ||
                                symbolName == "BoostProt.LoadDiff" ||
                                symbolName == "IgnKnk.fi_MeanKnock" ||
                                symbolName == "Ign.fi_OtherOff" ||
                                symbolName == "IgnJerkProt.fi_Offset")
                            {
                                if (value > 32000) value = -(65536 - value); // negatief maken
                            }

                            else if (symbolName == "KnkDet.KnockCyl" || symbolName == "KnkDetAdap.KnkCntCyl")         // 8 length
                            {
                                int knkcountcyl1 = 0;
                                int knkcountcyl2 = 0;
                                int knkcountcyl3 = 0;
                                int knkcountcyl4 = 0;
                                if (buffer.Length == 8)
                                {
                                    knkcountcyl1 = (Convert.ToInt32(buffer.GetValue(0)) * 256) + Convert.ToInt32(buffer.GetValue(1));
                                    knkcountcyl2 = (Convert.ToInt32(buffer.GetValue(2)) * 256) + Convert.ToInt32(buffer.GetValue(3));
                                    knkcountcyl3 = (Convert.ToInt32(buffer.GetValue(4)) * 256) + Convert.ToInt32(buffer.GetValue(5));
                                    knkcountcyl4 = (Convert.ToInt32(buffer.GetValue(6)) * 256) + Convert.ToInt32(buffer.GetValue(7));
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl1", (float)knkcountcyl1);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl2", (float)knkcountcyl2);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl3", (float)knkcountcyl3);
                                    UpdateRealtimeInformationInTable(dt, "KnockCyl4", (float)knkcountcyl4);
                                }

                            }
                            else if (symbolName == "MissfAdap.MissfCntCyl")         // 12 length
                            {
                                int miscountcyl1 = 0;
                                int miscountcyl2 = 0;
                                int miscountcyl3 = 0;
                                int miscountcyl4 = 0;
                                if (buffer.Length == 12)
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
                                logger.Debug("Failed to set peak: " + peakE.Message);
                            }
                            // update realtime info
                            UpdateRealtimeInformation(symbolName, (float)value);
                            try
                            {
                                CheckSoundsToPlay(symbolName, value);
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                            }
                        }
                    }
                    Thread.Sleep(0);//<GS-11022010>

                }

                // <GS-29072010> if the combiadapter is in use 
                // and the user configured to use ADCs or thermoinput, get the values
                if (m_appSettings.AdapterType == EnumHelper.GetDescription(CANBusAdapter.COMBI))
                {
                    if (m_appSettings.Useadc1)
                    {
                        float adc = trionic7.GetADCValue(0);
                        double convertedADvalue = Math.Round(ConvertADCValue(0, adc),2);
                        string channelName = m_appSettings.Adc1channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 1", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc1lowvalue/1000, m_appSettings.Adc1highvalue/1000, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc2)
                    {
                        float adc = trionic7.GetADCValue(1);
                        double convertedADvalue = Math.Round(ConvertADCValue(1, adc),2);
                        string channelName = m_appSettings.Adc2channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 2", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc2lowvalue/1000, m_appSettings.Adc2highvalue/1000, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc3)
                    {
                        float adc = trionic7.GetADCValue(2);
                        double convertedADvalue = Math.Round(ConvertADCValue(2, adc),2);
                        string channelName = m_appSettings.Adc3channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 3", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc3lowvalue/1000, m_appSettings.Adc3highvalue/1000, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc4)
                    {
                        float adc = trionic7.GetADCValue(3);
                        double convertedADvalue = Math.Round(ConvertADCValue(3, adc),2);
                        string channelName = m_appSettings.Adc4channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 4", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc4lowvalue/1000, m_appSettings.Adc4highvalue/1000, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Useadc5)
                    {
                        float adc = trionic7.GetADCValue(4);
                        double convertedADvalue = Math.Round(ConvertADCValue(4, adc),2);
                        string channelName = m_appSettings.Adc5channelname;
                        AddToRealtimeTable(dt, channelName, "ADC channel 5", 0, convertedADvalue, 0, 1, 0, m_appSettings.Adc5lowvalue/1000, m_appSettings.Adc5highvalue/1000, 0, 0, 0, 1);
                    }
                    if (m_appSettings.Usethermo)
                    {
                        float temperature = trionic7.GetThermoValue();
                        string channelName = m_appSettings.Thermochannelname;
                        AddToRealtimeTable(dt, channelName, "Thermo channel", 0, temperature, 0, 1, 0, 0, 1023, 0, 0, 0, 1);
                    }
                }

                // read afr from wideband on serial port
                if (m_appSettings.UseDigitalWidebandLambda && wbReader != null)
                {
                    float afr = (float)wbReader.LatestReading;
                    float lambda = afr / 14.7F;
                    digitalDisplayControl6.DigitText = afr.ToString("F1");
                    if (AfrViewMode == AFRViewType.AFRMode)
                    {
                        linearGauge2.Value = afr;
                    }
                    else
                    {
                        linearGauge2.Value = lambda;
                    }
                    if (m_appSettings.MeasureAFRInLambda)
                    {
                        LogWidebandAFR((float)lambda, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        AddToRealtimeTable(dt, "Wideband", "Lambda value (wbO2)", 0, Math.Round(lambda, 2), 0, 1, 0.0001, 0, 0, 0, 0, 0, 1);
                    }
                    else
                    {
                        LogWidebandAFR((float)afr, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        AddToRealtimeTable(dt, "Wideband", "AFR value (wbO2)", 0, Math.Round(afr, 2), 0, 1, 0, 10, 20, 0, 0, 0, 1);
                    }
                    ProcessAutoTuning((float)afr, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                }
              
                logger.Debug("All updated in " + _sw.ElapsedMilliseconds.ToString() + " ms");
                LogRealTimeInformation(dt, datet);
                UpdateOpenViewers();
                //<GS-06012011> maybe move the fps counter timer here!
                _sw.Stop();
                // update fps indicator
                float secs = _sw.ElapsedMilliseconds / 1000F;
                secs = 1 / secs;
                if (float.IsInfinity(secs)) secs = 1;
                UpdateRealtimeInformation("FPSCounter", secs);
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
                        logger.Debug(E.Message);
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
            //logger.Debug("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_LowVoltage / 1000) voltage = m_LowVoltage / 1000;
            if (voltage > m_HighVoltage / 1000) voltage = m_HighVoltage / 1000;
            //logger.Debug("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_HighValue / 1000) - (m_LowValue / 1000)) / ((m_HighVoltage / 1000) - (m_LowVoltage / 1000));
            //logger.Debug("Steepness: " + steepness.ToString());
            retval = (m_LowValue / 1000) + (steepness * (voltage - (m_LowVoltage / 1000)));
            //logger.Debug("retval: " + retval.ToString());
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
                    // add it to the list then
                    /*
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
                dt.Columns.Add("UserDefined", Type.GetType("System.Int32"));                     * */
                    dt.Rows.Add(symbolname, 0, value, 0, 1, 0, 0, 65535, 0, 0, 0, 1, 1, 1);
                }
            }
        }

        private void UpdateOpenViewers()
        {
            UpdateInjectionMap();
            UpdateIgnitionMap();
            UpdateAirmassMap();
        }

        /*private int LookUpIndexAxisTPSMap(double value, string symbolname, int multiplywith)
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
                    b *= multiplywith;
                    double diff = Math.Abs((double)b - value);
                    if (min_difference > diff)
                    {
                        min_difference = diff;
                        // this is our index
                        return_index = t / 2;
                        // logger.Debug("Difference was: " + diff.ToString() + " at index " + return_index.ToString());

                    }
                }
            }
            else
            {
                for (int t = 0; t < axisvalues.Length; t++)
                {
                    int b = (int)(byte)axisvalues.GetValue(t);
                    b *= multiplywith;
                    double diff = Math.Abs((double)b - value);
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
        */

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
                        // logger.Debug("Difference was: " + diff.ToString() + " at index " + return_index.ToString());

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
            UpdateDocksWithName("PedalMapCal.m_RequestMap", rpmindex, tpsindex);

            //TorqueCal.m_AirTorqMap has
            // X: TorqueCal.M_EngXSP
            torqueindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentEngineTorque, "TorqueCal.M_EngXSP", 1);
            // Y: TorqueCal.n_EngYSP
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "TorqueCal.n_EngYSP", 1);
            UpdateDocksWithName("TorqueCal.m_AirTorqMap", torqueindex, rpmindex);

            //TorqueCal.M_NominalMap has
            //X: TorqueCal.m_AirXSP
            //Y: TorqueCal.n_EngYSP // same, so leave that out
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "TorqueCal.m_AirXSP", 1);
            UpdateDocksWithName("TorqueCal.M_NominalMap", airmassindex, rpmindex);

            //BoostCal.RegMap has:
            //X: BoostCal.SetLoadXSP (airmass)
            //Y: BoostCal.n_EngSP (rpm)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BoostCal.n_EngSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "BoostCal.SetLoadXSP", 1);
            UpdateDocksWithName("BoostCal.RegMap", airmassindex, rpmindex);

            //BstKnkCal.MaxAirmass
            //BstKnkCal.MaxAirmassAu
            //X: BstKnkCal.OffsetXSP // special because all negative
            //Y: BstKnkCal.n_EngYSP
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BstKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentIgnitionOffset, "BstKnkCal.OffsetXSP", 0.1);
            UpdateDocksWithName("BstKnkCal.MaxAirmass", airmassindex, rpmindex);
            UpdateDocksWithName("BstKnkCal.MaxAirmassAu", airmassindex, rpmindex);
        }


        private void UpdateInjectionMap()
        {
            int airmassindex = 0;
            int rpmindex = 0;

            //BFuelCal.Map has
            //X: BFuelCal.AirXSP (airmass)
            //Y: BFuelCal.RpmYSP (engine speed)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BFuelCal.RpmYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "BFuelCal.AirXSP", 1);
            UpdateDocksWithName("BFuelCal.Map", airmassindex, rpmindex);
            //BFuelCal.StartMap has
            //X: BFuelCal.AirXSP (airmass)
            //Y: BFuelCal.RpmYSP (engine speed)
            // uses the same as BFuelCal.Map, so leave it
            UpdateDocksWithName("BFuelCal.StartMap", airmassindex, rpmindex);
            UpdateDocksWithName("BFuelCal.E85Map", airmassindex, rpmindex);
            //KnkFuelCal.EnrichmentMap has
            //X: IgnKnkCal.m_AirXSP // airmass
            //Y: IgnKnkCal.n_EngYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnKnkCal.m_AirXSP", 1);
            UpdateDocksWithName("KnkFuelCal.EnrichmentMap", airmassindex, rpmindex);
            //InjAnglCal.Map has
            //X: InjAnglCal.AirXSP // airmass
            //Y: InjAnglCal.RpmYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "InjAnglCal.RpmYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "InjAnglCal.AirXSP", 1);
            UpdateDocksWithName("InjAnglCal.Map", airmassindex, rpmindex);
        }

        private void UpdateIgnitionMap()
        {

            int airmassindex = 0;
            int rpmindex = 0;
            // IgnNormCal.Map has
            //X: IgnNormCal.m_AirXSP // airmass
            //Y: IgnNormCal.n_EngYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnNormCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnNormCal.m_AirXSP", 1);
            UpdateDocksWithName("IgnNormCal.Map", airmassindex, rpmindex);

            //IgnE85Cal.fi_AbsMap has:
            // same, leave it
            UpdateDocksWithName("IgnE85Cal.fi_AbsMap", airmassindex, rpmindex);
            //KnkFuelCal.fi_MapMaxOff has
            //X: KnkFuelCal.m_AirXSP //airmass
            //Y: BstKnkCal.n_EngYSP // engine speed
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "BstKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "KnkFuelCal.m_AirXSP", 1);
            UpdateDocksWithName("KnkFuelCal.fi_MapMaxOff", airmassindex, rpmindex);
            //IgnKnkCal.IndexMap has
            // X: IgnKnkCal.m_AirXSP (airmass)
            // Y: IgnKnkCal.n_EngYSP (engine speed)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "IgnKnkCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "IgnKnkCal.m_AirXSP", 1);
            UpdateDocksWithName("IgnKnkCal.IndexMap", airmassindex, rpmindex);
            //KnkDetCal.RefFactorMap
            // X: KnkDetCal.m_AirXSP (airmass)
            // Y: KnkDetCal.n_EngYSP (engine speed)
            rpmindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentRPM, "KnkDetCal.n_EngYSP", 1);
            airmassindex = LookUpIndexAxisRPMMap(_currentEngineStatus.CurrentAirmassPerCombustion, "KnkDetCal.m_AirXSP", 1);
            UpdateDocksWithName("KnkDetCal.RefFactorMap", airmassindex, rpmindex);

        }

        private void UpdateDocksWithName(string symbolname, int xindex, int yindex)
        {
            try
            {
                foreach (DockPanel pnl in dockManager1.Panels)
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
                        else if (c is DockPanel)
                        {
                            DockPanel tpnl = (DockPanel)c;
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
                        else if (c is ControlContainer)
                        {
                            ControlContainer cntr = (ControlContainer)c;
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
                logger.Debug(E.Message);
            }
        }

        private void UpdateRealtimeInformationValue(string symbolname, float value)
        {
            switch (symbolname)
            {
                case "ActualIn.n_Engine": // rpm
                    digitalDisplayControl1.DigitText = value.ToString("F0");
                    _currentEngineStatus.CurrentRPM = value;
                    break;
                case "ActualIn.T_Engine": // engine temp
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
                    labelControl9.Text = ConvertActiveAirDemand(Convert.ToInt32(value));
                    _currentEngineStatus.CurrentAirmassLimiterID = Convert.ToInt32(value);
                    break;
                case "Lambda.Status": // Lambda status
                    //digitalDisplayControl5.DigitText = value.ToString("F0");
                    labelControl10.Text = ConvertLambdaStatus(Convert.ToInt32(value));
                    _currentEngineStatus.CurrentLambdaStatus = Convert.ToInt32(value);
                    break;
                case "FCut.CutStatus": // Fuelcut status
                    labelControl12.Text = ConvertFuelcutStatus(Convert.ToInt32(value));
                    _currentEngineStatus.CurrentFuelcutStatus = Convert.ToInt32(value);
                    break;
                case "IgnProt.fi_Offset": // Ioff
                    measurementIgnitionOffset.Value = value;
                    _currentEngineStatus.CurrentIgnitionOffset = value;
                    //TODO: <GS-18102010> ignition offset als knock indicator gebruiken!?
                    UpdateKnockIndicator(value);

                    break;
                case "m_Request": // drivers airmass request
                    measurementAirmassRequest.Value = value;
                    _currentEngineStatus.CurrentAirmassRequest = value;
                    break;
                case "Out.M_Engine": // calc. torque
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
                //case "ECMStat.p_Diff": // pressure diff = boost
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
                case "Out.X_AccPedal": // TPS
                    measurementPedalPosition.Value = value;
                    _currentEngineStatus.CurrentThrottlePosition = value;
                    break;
                case "MAF.m_AirInlet": // actual airmass
                    linearGauge1.Value = value;
                    _currentEngineStatus.CurrentAirmassPerCombustion = value;
                    break;
                case "DisplProt.LambdaScanner": // AFR through wideband?
                case "DisplProt.AD_Scanner": // AFR through wideband?
                    if (m_appSettings.UseWidebandLambda)
                    {
                        if (symbolname == "DisplProt.AD_Scanner")
                        {
                            value = (float)ConvertToWidebandAFR(value);
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
                            LogWidebandAFR(value / 14.7F, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        }
                        else
                        {
                            LogWidebandAFR(value, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                        }
                        ProcessAutoTuning(value, _currentEngineStatus.CurrentRPM, _currentEngineStatus.CurrentAirmassPerCombustion);
                    }
                    //_currentEngineStatus.CurrentAFR = value;
                    break;
                case "Lambda.LambdaInt": // AFR through narrowband?
                    if (!m_appSettings.UseWidebandLambda && !m_appSettings.UseDigitalWidebandLambda)
                    {
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
                logger.Debug(E.Message);
            }

        }

        private double ConvertToWidebandAFR(float value)
        {
            // convert to AFR value using wideband lambda sensor settings
            // ranges 0 - 255 will be default for 0-5 volt
            double retval = 0;
            double voltage = ((value) / 1023) * (m_appSettings.WidebandHighVoltage / 1000 - m_appSettings.WidebandLowVoltage / 1000);
            //logger.Debug("Wideband voltage: " + voltage.ToString());
            // now convert to AFR using user settings
            if (voltage < m_appSettings.WidebandLowVoltage / 1000) voltage = m_appSettings.WidebandLowVoltage / 1000;
            if (voltage > m_appSettings.WidebandHighVoltage / 1000) voltage = m_appSettings.WidebandHighVoltage / 1000;
            //logger.Debug("Wideband voltage (after clipping): " + voltage.ToString());
            double steepness = ((m_appSettings.WidebandHighAFR / 1000) - (m_appSettings.WidebandLowAFR / 1000)) / ((m_appSettings.WidebandHighVoltage / 1000) - (m_appSettings.WidebandLowVoltage / 1000));
            //logger.Debug("Steepness: " + steepness.ToString());
            retval = (m_appSettings.WidebandLowAFR / 1000) + (steepness * (voltage - (m_appSettings.WidebandLowVoltage / 1000)));
            //logger.Debug("retval: " + retval.ToString());
            return retval;

        }

        private void LogWidebandAFR(float afr, float _lastRPM, float _lastLoad)
        {
            
            if (!m_appSettings.AutoCreateAFRMaps) return;
            try
            {
                if (_lastLoad != -1 && _lastRPM > 600 && afr >= 0 && afr < 25)
                {
                    int columns = 0;
                    int rows = 0;
                    
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out columns, out rows);
                    //TODO: calculate averages on the fly so we can display the AFR map live as well.
                    if (columns != 0)
                    {
                        if (m_AFRMap == null )
                        {
                            m_AFRMap = new AFRMap();
                            m_AFRMap.RpmYSP = GetSymbolAsIntArray("BFuelCal.RpmYSP");
                            m_AFRMap.AirXSP = GetSymbolAsIntArray("BFuelCal.AirXSP");
                            m_AFRMap.InitializeMaps(columns * rows, m_currentfile);
                        }

                        int rpmindex = LookUpIndexAxisRPMMap(_lastRPM, "BFuelCal.RpmYSP", 1);

                        int mapindex = LookUpIndexAxisRPMMap(_lastLoad, "BFuelCal.AirXSP", 1);
                        // get current counter
                        if (_currentEngineStatus.CurrentFuelcutStatus == 0) //Only do AddMeasurement if No FuelCut Active.
                        {
                            m_AFRMap.AddMeasurement(afr, rpmindex, mapindex, columns, rows);
                            UpdateFeedbackMaps();
                            System.Windows.Forms.Application.DoEvents();
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("LogWidebandAFR: " + E.Message);
            }
        }

        private void UpdateMapViewer(IMapViewer viewer, int tabwidth, bool sixteenbits)
        {
            viewer.ShowTable(tabwidth, sixteenbits);
        }

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
            if (!m_appSettings.AutoCreateAFRMaps) return;

            try
            {
                // convert feedback map in memory to byte[] in stead of float[]
                int rows = 0;
                int cols = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                byte[] current_map = m_AFRMap.GetFeedbackMapInBytes(cols * rows);
                foreach (DockPanel pnl in dockManager1.Panels)
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
                                    vwr.Afr_counter = m_AFRMap.GetAFRCountermap();
                                    UpdateViewer(vwr, cols, true);
                                }
                            }
                            else if (c is DockPanel)
                            {
                                DockPanel tpnl = (DockPanel)c;
                                foreach (Control c2 in tpnl.Controls)
                                {
                                    if (c2 is IMapViewer)
                                    {
                                        IMapViewer vwr2 = (IMapViewer)c2;
                                        if (vwr2.Map_name == "FeedbackAFR")
                                        {
                                            vwr2.Map_content = current_map;
                                            vwr2.Afr_counter = m_AFRMap.GetAFRCountermap();
                                            UpdateViewer(vwr2, cols, true);
                                        }
                                    }
                                }
                            }
                            else if (c is ControlContainer)
                            {
                                ControlContainer cntr = (ControlContainer)c;
                                foreach (Control c3 in cntr.Controls)
                                {
                                    if (c3 is IMapViewer)
                                    {
                                        IMapViewer vwr3 = (IMapViewer)c3;
                                        if (vwr3.Map_name == "FeedbackAFR")
                                        {
                                            vwr3.Map_content = current_map;
                                            vwr3.Afr_counter = m_AFRMap.GetAFRCountermap();
                                            UpdateViewer(vwr3, cols, true);

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
                    logger.Debug(E.Message);
                }
            }

        }

        private void LogRealTimeInformation(System.Data.DataTable dt, DateTime timestamp)
        {
            if (dt == null) return;
            if (m_currentfile == "") return;
            string logline = timestamp.ToString("dd/MM/yyyy HH:mm:ss") + "." + timestamp.Millisecond.ToString("D3") + "|";
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    logline += dr["SymbolName"].ToString() + "=" + Convert.ToDouble(dr["Value"]).ToString() + "|";
                }
                catch (Exception E)
                {
                    logger.Debug("Failed to log to file: " + E.Message);
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

            string outputfile = Path.GetDirectoryName(m_currentfile);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMdd") + "-CanTraceExt.t7l");
            using (StreamWriter sw = new StreamWriter(outputfile, true))
            {
                sw.WriteLine(logline);
            }
        }

        private bool GenerateLogWorksFile(string filename)
        {
            bool retval = false;
            if (File.Exists(filename))
            {
                ConvertFileToDif(filename, true);
                retval = true;
            }
            return retval;
        }

        private void ConvertFileToDif(string filename, bool AutoExport)
        {
            System.Windows.Forms.Application.DoEvents();
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            try
            {
                SymbolCollection sc = LogFile.FindSymbols(filename, ref startDate, ref endDate);

                if (AutoExport)
                {
                    symbolColors.AddColorsFromRegistry(sc);
                    InitExportDif(filename, startDate, endDate, sc);
                }
                else
                {
                    // show selection screen
                    frmPlotSelection plotsel = new frmPlotSelection(suiteRegistry);
                    foreach (SymbolHelper sh in sc)
                    {
                        plotsel.AddItemToList(sh.SmartVarname);
                    }
                    plotsel.Startdate = startDate;
                    plotsel.Enddate = endDate;
                    plotsel.SelectAllSymbols();
                    if (plotsel.ShowDialog() == DialogResult.OK)
                    {
                        sc = plotsel.Sc;
                        endDate = plotsel.Enddate;
                        startDate = plotsel.Startdate;
                        InitExportDif(filename, startDate, endDate, sc);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void InitExportDif(string filename, DateTime startDate, DateTime endDate, SymbolCollection sc)
        {
            DifGenerator difgen = new DifGenerator();
            LogFilters filterhelper = new LogFilters(suiteRegistry);
            difgen.SetFilters(filterhelper.GetFiltersFromRegistry());
            difgen.AppSettings = m_appSettings;
            //difgen.LowAFR = m_appSettings.WidebandLowAFR;
            //difgen.HighAFR = m_appSettings.WidebandHighAFR;
            //difgen.MaximumVoltageWideband = m_appSettings.WidebandHighVoltage;
            //difgen.MinimumVoltageWideband = m_appSettings.WidebandLowVoltage;
            difgen.WidebandSymbol = m_appSettings.WideBandSymbol;
            //difgen.UseWidebandInput = m_appSettings.UseWidebandLambdaThroughSymbol;
            difgen.UseWidebandInput = false;

            difgen.onExportProgress += new DifGenerator.ExportProgress(difgen_onExportProgress);
            frmProgressExportLog = new frmProgress();
            frmProgressExportLog.SetProgress("Exporting to LogWorks");
            frmProgressExportLog.Show();
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
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            frmProgressExportLog.Close();
        }


        private Int32 GetValueFromRegistry(string symbolname)
        {
            Int32 win32color = 0;
            RegistryKey SoftwareKey = Registry.CurrentUser.CreateSubKey("Software");
            RegistryKey ManufacturerKey = SoftwareKey.CreateSubKey("MattiasC");
            RegistryKey SuiteKey = ManufacturerKey.CreateSubKey("T7SuitePro");

            using (RegistryKey Settings = SuiteKey.CreateSubKey("SymbolColors"))
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

        void difgen_onExportProgress(object sender, DifGenerator.ProgressEventArgs e)
        {
            frmProgressExportLog.SetProgressPercentage(e.Percentage);
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

        private int _delayUpdateCounter = 0;

        private void tmrRealtime_Tick(object sender, EventArgs e)
        {
            tmrRealtime.Enabled = false;
            try
            {
                if (m_RealtimeConnectedToECU)
                {
                    if (!m_prohibitReading)
                    {
                        GetSRAMVarsFromTable();
                        _delayUpdateCounter++;
                        if (_delayUpdateCounter > 20)
                        {
                            _delayUpdateCounter = 0;
                            try
                            {
                                UpdatePerformanceMode(); //
                            }
                            catch (Exception pE)
                            {
                                logger.Debug("Failed to update performance mode: " + pE.Message);
                            }
                        }
                        //<GS-23052011> change to engine temp > 0 in stead of > 70
                        if (/*_currentEngineStatus.CurrentEngineTemp >= 0 &&*/ m_appSettings.UseWidebandLambda || m_appSettings.UseDigitalWidebandLambda)
                        {
                            // autotune enabled
                            if (btnAutoTune.Text != "Wait...") // if we are waiting... don't enable because of engine temperature
                            {
                                btnAutoTune.Enabled = true;
                            }
                        }
                        /*else if (m_appSettings.DebugMode)
                        {
                            btnAutoTune.Enabled = true;
                        }*/
                        else
                        {
                            btnAutoTune.Enabled = false;
                        }
                    }
                    else
                    {
                        logger.Debug("Not reading from SRAM because we're doing something else");
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to run realtime timer code: " + E.Message);
            }
            tmrRealtime.Enabled = m_enableRealtimeTimer;
        }

        private void UpdatePerformanceMode()
        {
            //TODO: refresh the performance mode indicator
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "Performance.Mode" || sh.Userdescription == "Performance.Mode")
                {
                    if (sh.Start_address > 0)
                    {
                        bool _success = false;
                        //logger.Debug("Start read pm!");
                        byte[] performancemodeData = ReadSymbolFromSRAM((uint)sh.Symbol_number_ECU, "Performance.Mode", (uint)sh.Start_address, (int)sh.Length, out _success);
                        //logger.Debug("PM len: " + performancemodeData.Length.ToString() + " shlen: " + sh.Length.ToString());
                        //logger.Debug("PM data: " + performancemodeData[0].ToString());
                        //logger.Debug("success: " + _success.ToString());
                        if (_success)
                        {
                            if (performancemodeData[performancemodeData.Length - 1] == 0x00 || performancemodeData[performancemodeData.Length - 1] == 0x45)
                            {
                                // ECO mode
                                //if (btnEconomyMode.ForeColor != Color.Black)
                                {
                                    btnEconomyMode.ForeColor = Color.Black;
                                    btnNormalMode.ForeColor = Color.DimGray;
                                    btnSportMode.ForeColor = Color.DimGray;
                                }
                            }
                            else if (performancemodeData[performancemodeData.Length - 1] == 0x01 || performancemodeData[performancemodeData.Length - 1] == 0x4E)
                            {
                                // Normal mode
                                //if (btnNormalMode.ForeColor != Color.Black)
                                {
                                    btnEconomyMode.ForeColor = Color.DimGray;
                                    btnNormalMode.ForeColor = Color.Black;
                                    btnSportMode.ForeColor = Color.DimGray;
                                }
                            }
                            else if (performancemodeData[performancemodeData.Length - 1] == 0x02 || performancemodeData[performancemodeData.Length - 1] == 0x53)
                            {
                                // Sport mode
                               // if (btnSportMode.ForeColor != Color.Black)
                                {
                                    btnEconomyMode.ForeColor = Color.DimGray;
                                    btnNormalMode.ForeColor = Color.DimGray;
                                    btnSportMode.ForeColor = Color.Black;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void barButtonItem44_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 logfiles|*.t7l";
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
                            logger.Debug("break");
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
                    logger.Debug(E.Message);
                }
            }

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ViewRealtime.ColumnsCustomization();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ViewRealtime.BestFitColumns();

        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            if (gridRealtime.DataSource != null)
            {
                System.Data.DataTable dt = (System.Data.DataTable)gridRealtime.DataSource;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        dr["Peak"] = dr["Minimum"];
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }

                }
            }
            // reset peak values
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = gridRealtime.Font;
            fd.FontMustExist = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                SetRealtimeListFont(fd.Font);
            }
            
        }

        private void SetRealtimeListFont(System.Drawing.Font f)
        {
            ViewRealtime.Appearance.ColumnFilterButton.Font = f;
            ViewRealtime.Appearance.ColumnFilterButtonActive.Font = f;
            ViewRealtime.Appearance.DetailTip.Font = f;
            ViewRealtime.Appearance.Empty.Font = f;
            ViewRealtime.Appearance.EvenRow.Font = f;
            ViewRealtime.Appearance.FilterCloseButton.Font = f;
            ViewRealtime.Appearance.FilterPanel.Font = f;
            ViewRealtime.Appearance.FixedLine.Font = f;
            ViewRealtime.Appearance.FocusedCell.Font = f;
            ViewRealtime.Appearance.FocusedRow.Font = f;
            ViewRealtime.Appearance.FooterPanel.Font = f;
            ViewRealtime.Appearance.GroupButton.Font = f;
            ViewRealtime.Appearance.GroupFooter.Font = f;
            ViewRealtime.Appearance.GroupPanel.Font = f;
            ViewRealtime.Appearance.GroupRow.Font = f;
            ViewRealtime.Appearance.HeaderPanel.Font = f;
            ViewRealtime.Appearance.HideSelectionRow.Font = f;
            ViewRealtime.Appearance.HorzLine.Font = f;
            ViewRealtime.Appearance.OddRow.Font = f;
            ViewRealtime.Appearance.Preview.Font = f;
            ViewRealtime.Appearance.Row.Font = f;
            ViewRealtime.Appearance.RowSeparator.Font = f;
            ViewRealtime.Appearance.SelectedRow.Font = f;
            ViewRealtime.Appearance.TopNewRow.Font = f;
            ViewRealtime.Appearance.VertLine.Font = f;
            gridRealtime.Font = f;
            ViewRealtime.GroupRowHeight = (int)(f.Size * 1.5F);
            ViewRealtime.RowHeight = (int)(f.Size * 1.5F);
            gridRealtime.Invalidate();
            m_appSettings.RealtimeFont = f;
        }


        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                ViewRealtime.DeleteSelectedRows();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                int[] records = ViewRealtime.GetSelectedRows();
                if (records.Length == 1)
                {
                    DataRowView drv = (DataRowView)ViewRealtime.GetRow((int)records.GetValue(0));
                    frmEditRealtimeSymbol frmeditsymbol = new frmEditRealtimeSymbol();
                    frmeditsymbol.Symbols = m_symbols;
                    frmeditsymbol.Symbolname = drv.Row["SymbolName"].ToString();
                    frmeditsymbol.MinimumValue = Convert.ToDouble(drv.Row["Minimum"]);
                    frmeditsymbol.MaximumValue = Convert.ToDouble(drv.Row["Maximum"]);
                    frmeditsymbol.Description = drv.Row["Description"].ToString();
                    frmeditsymbol.OffsetValue = Convert.ToDouble(drv.Row["Offset"]);
                    frmeditsymbol.CorrectionValue = Convert.ToDouble(drv.Row["Correction"]);
                    if (frmeditsymbol.ShowDialog() == DialogResult.OK)
                    {
                        drv.Row["SymbolName"] = frmeditsymbol.Symbolname;
                        drv.Row["Minimum"] = frmeditsymbol.MinimumValue;
                        drv.Row["Maximum"] = frmeditsymbol.MaximumValue;
                        drv.Row["Description"] = frmeditsymbol.Description;
                        drv.Row["Offset"] = frmeditsymbol.OffsetValue;
                        drv.Row["Correction"] = frmeditsymbol.CorrectionValue;
                    }

                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
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

        private void StartSRAMTableViewer()
        {
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    DockPanel dockPanel;
                    bool pnlfound = false;
                    foreach (DockPanel pnl in dockManager1.Panels)
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
                            IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                            tabdet.Filename = m_currentsramfile;
                            tabdet.Map_name = sh.Varname;
                            tabdet.Map_descr = TranslateSymbolName(tabdet.Map_name);
                            tabdet.Map_cat = XDFCategories.Undocumented;
                            tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, tabdet.Map_name);
                            tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, tabdet.Map_name);

                            if (!m_appSettings.NewPanelsFloating)
                            {
                                dockPanel = dockManager1.AddPanel(DockingStyle.Right);
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
                                TryToAddOpenLoopTables(tabdet);

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
                                    foreach (DockPanel pnl in dockManager1.Panels)
                                    {
                                        if (pnl.Text.StartsWith("SRAM Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                        foreach (DockPanel pnl in dockManager1.Panels)
                                        {
                                            if ((string)pnl.Tag == m_currentsramfile && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                                    dockPanel.DockTo(DockingStyle.Right, 0);
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
                            logger.Debug(newdockE.Message);
                        }
                        dockManager1.EndUpdate();
                    }
                }
            }

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
            DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DockPanel pnl in dockManager1.Panels)
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
                    dockPanel = dockManager1.AddPanel(DockingStyle.Right);
                    dockPanel.Tag = filename;
                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.Filename = filename;
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

                    /** end NEW 12/11/2008 **/

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
                        TryToAddOpenLoopTables(tabdet);

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
                            foreach (DockPanel pnl in dockManager1.Panels)
                            {
                                if (pnl.Text.StartsWith("SRAM Symbol: " + tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
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
                            dockPanel.DockTo(DockingStyle.Right, 0);
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
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();
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

        private void browseAxisInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string symbolname = string.Empty;
            if (gridViewSymbols.SelectedRowsCount > 0)
            {
                int[] selrows = gridViewSymbols.GetSelectedRows();
                if (selrows.Length > 0)
                {
                    SymbolHelper dr = (SymbolHelper)gridViewSymbols.GetRow((int)selrows.GetValue(0));
                    if (dr.Userdescription != "" && dr.Userdescription != String.Format("Symbolnumber {0}", dr.Symbol_number))
                    {
                        symbolname = dr.Userdescription;
                    }
                    else
                    {
                        symbolname = dr.Varname;
                    }
                }
            }
            DockPanel dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
            AxisBrowser tabdet = new AxisBrowser();
            tabdet.onStartSymbolViewer += new AxisBrowser.StartSymbolViewer(tabdet_onStartSymbolViewer);
            tabdet.ApplicationLanguage = m_appSettings.ApplicationLanguage;
            tabdet.Dock = DockStyle.Fill;
            dockPanel.Controls.Add(tabdet);
            tabdet.ShowSymbolCollection(m_symbols);
            tabdet.SetCurrentSymbol(symbolname);
            dockPanel.Text = "Axis browser: " + Path.GetFileName(m_currentfile);
            bool isDocked = false;
            foreach (DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text.StartsWith("Axis browser: ") && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
                {
                    dockPanel.DockAsTab(pnl, 0);
                    isDocked = true;
                    break;
                }
            }
            if (!isDocked)
            {
                dockPanel.DockTo(DockingStyle.Left, 1);
                dockPanel.Width = 700;
            }
        }

        void tabdet_onStartSymbolViewer(object sender, AxisBrowser.SymbolViewerRequestedEventArgs e)
        {
            StartAViewer(e.Mapname);
        } 

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (m_RealtimeConnectedToECU)
            {
                readSymbolToolStripMenuItem.Enabled = true;
                readSymbolToolStripMenuItem.Text = "Read symbol from binary file";
                //writeSymbolToSRAMToolStripMenuItem.Enabled = true;
            }
            else
            {
                //readSymbolToolStripMenuItem.Enabled = false;
                readSymbolToolStripMenuItem.Enabled = true;
                readSymbolToolStripMenuItem.Text = "Read symbol from ECU";
                //writeSymbolToSRAMToolStripMenuItem.Enabled = false;
            }
            // if at least one symbol selected
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

        private void barButtonItem61_ItemClick(object sender, ItemClickEventArgs e)
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
                        string destfile = Path.GetDirectoryName(m_currentfile) + "\\FROM_ECU" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".S19";
                        File.Copy(fromfile, destfile, true);
                        if (m_appSettings.TargetECUReadFile != string.Empty)
                        {
                            CloseProject();
                            m_appSettings.Lastprojectname = "";
                            File.Copy(fromfile, m_appSettings.TargetECUReadFile, true);
                            OpenFile(m_appSettings.TargetECUReadFile, false);
                            m_appSettings.LastOpenedType = 0;
                        }
                        else
                        {
                            CloseProject();
                            m_appSettings.Lastprojectname = "";
                            OpenFile(destfile, false);
                            m_appSettings.LastOpenedType = 0;

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

        private void barButtonItem62_ItemClick(object sender, ItemClickEventArgs e)
        {
            // write the required file for flashing the ECU
            // this is the current file, exported to S19 format in the directory that contains
            // the selected batchfile
            if (m_appSettings.Write_ecubatchfile != string.Empty)
            {
                try
                {
                    if (File.Exists(m_appSettings.Write_ecubatchfile))
                    {

                        if (!VerifyChecksum(false))
                        {
                            frmChecksumIncorrect check = new frmChecksumIncorrect();
                            if (m_appSettings.AutoChecksum)
                            {
                                UpdateChecksum(m_currentfile);
                                if (m_fileiss19)
                                {
                                    // automatisch terugschrijven
                                    srec2bin cnvrt = new srec2bin();
                                    cnvrt.ConvertBinToSrec(m_currentfile);
                                }

                            }
                            //else if (MessageBox.Show("Checksum invalid, auto correct?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            else if (check.ShowDialog() == DialogResult.Yes)
                            {
                                UpdateChecksum(m_currentfile);
                                if (m_fileiss19)
                                {
                                    // automatisch terugschrijven
                                    srec2bin cnvrt = new srec2bin();
                                    cnvrt.ConvertBinToSrec(m_currentfile);
                                }
                            }
                        }

                        srec2bin sr = new srec2bin();
                        sr.ConvertBinToSrec(m_currentfile);
                        // and copy it to the target directory
                        string fromfile = Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + ".S19";
                        string destfile = Path.GetDirectoryName(m_appSettings.Write_ecubatchfile) + "\\TO_ECU.S19";
                        File.Copy(fromfile, destfile, true);
                        System.Diagnostics.Process.Start(m_appSettings.Write_ecubatchfile);
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

        private void barButtonItem60_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            frmPeMicroParameters frmpe = new frmPeMicroParameters();
            frmpe.ECUReadFile = m_appSettings.Read_ecubatchfile;

            frmpe.ECUWriteAMDFile = m_appSettings.Write_ecubatchfile;

            frmpe.TargetECUReadFile = m_appSettings.TargetECUReadFile;

            if (frmpe.ShowDialog() == DialogResult.OK)
            {
                m_appSettings.Read_ecubatchfile = frmpe.ECUReadFile;
                m_appSettings.Write_ecubatchfile = frmpe.ECUWriteAMDFile;
                m_appSettings.TargetECUReadFile = frmpe.TargetECUReadFile;
            }
        }

        private void btnGetFlashContent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RealtimeDisconnectAndHide();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (FlasherConnect())
                {
                    SetProgress("Starting flash download");
                    trionic7.ReadFlash(sfd.FileName);
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to read flash");
                }
            }
        }

        private void btnFlashECU_ItemClick(object sender, ItemClickEventArgs e)
        {
            RealtimeDisconnectAndHide();

            if (File.Exists(m_currentfile))
            {
                if (FlasherConnect())
                {
                    SetProgress("Starting flash upload");
                    trionic7.WriteFlash(m_currentfile);
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to write flash");
                }
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No file has been loaded");
            }
        }

        private void btnGetSRAMSnapshot_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RealtimeDisconnectAndHide();

            if (FlasherConnect())
            {
                SetProgress("Starting snapshot download");
                string filename = Path.GetDirectoryName(m_currentfile) + "\\SRAM" + DateTime.Now.Year.ToString("D4") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2") + DateTime.Now.Millisecond.ToString("D3") + ".RAM";
                if (m_CurrentWorkingProject != "")
                {
                    if (!Directory.Exists(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots")) Directory.CreateDirectory(m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots");
                    filename = m_appSettings.ProjectFolder + "\\" + m_CurrentWorkingProject + "\\Snapshots\\Snapshot" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".RAM";
                }
                if (trionic7.GetSRAMSnapshot(filename))
                {
                    frmInfoBox info = new frmInfoBox("Snapshot downloaded and saved to: " + filename);
                }
                trionic7.Cleanup();
                SetCANStatus("");
                SetProgressIdle();
            }
            else
            {
                frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to read a sram snapshot");
            }
        }

        private bool FlasherConnect()
        {
            if (trionic7.isOpen())
            {
                trionic7.Cleanup();
            }
            SetCANStatus("Initializing CANbus interface");

            SetupCanAdapter(Latency.Default);
            if (trionic7.openDevice())
            {
                SetCANStatus("Connected");
                return true;
            }
            else
            {
                SetCANStatus("Failed to start KWP session");
                trionic7.Cleanup();
                return false;
            }
        }

        private void gridViewSymbols_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Name == gcSymbolsUserDescription.Name)
            {
                // save a new repository item
                SaveAdditionalSymbols();
            }
        }

        private void SaveAdditionalSymbols()
        {
            using (System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(m_currentfile)))
            {
                dt.Columns.Add("SYMBOLNAME");
                dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                dt.Columns.Add("DESCRIPTION");
                T7FileHeader fh = new T7FileHeader();
                fh.init(m_currentfile, false);
                string checkstring = fh.getPartNumber() + fh.getSoftwareVersion();
                string xmlfilename = String.Format("{0}\\repository\\{1}{2:yyyyMMddHHmmss}{3}.xml", System.Windows.Forms.Application.StartupPath, Path.GetFileNameWithoutExtension(m_currentfile), File.GetCreationTime(m_currentfile), checkstring);
                if (Directory.Exists(String.Format("{0}\\repository", System.Windows.Forms.Application.StartupPath)))
                {
                    if (File.Exists(xmlfilename))
                    {
                        File.Delete(xmlfilename);
                    }
                }
                else
                {
                    Directory.CreateDirectory(String.Format("{0}\\repository", System.Windows.Forms.Application.StartupPath));
                }
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Userdescription != "")
                    {
                        if (sh.Userdescription == String.Format("Symbolnumber {0}", sh.Symbol_number))
                        {
                            dt.Rows.Add(sh.Userdescription, sh.Symbol_number, sh.Flash_start_address, sh.Varname);
                        }
                        else
                        {
                            dt.Rows.Add(sh.Varname, sh.Symbol_number, sh.Flash_start_address, sh.Userdescription);
                        }
                    }
                }
                dt.WriteXml(xmlfilename);
            }
        }

        private void StartReleaseNotesViewer(string xmlfilename, string version)
        {
            dockManager1.BeginUpdate();
            DockPanel dp = dockManager1.AddPanel(DockingStyle.Right);
            dp.ClosedPanel += dockPanel_ClosedPanel;
            dp.Tag = xmlfilename;
            ctrlReleaseNotes mv = new ctrlReleaseNotes();
            mv.LoadXML(xmlfilename);
            mv.Dock = DockStyle.Fill;
            dp.Width = 500;
            dp.Text = "Release notes: " + version;
            dp.Controls.Add(mv);
            dockManager1.EndUpdate();
        }

        private bool SymbolExists(string symbolname)
        {
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == symbolname || sh.Userdescription == symbolname) return true;
            }
            return false;
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

                    int torqueincell = AirmassToTorque(valueincell, E85);
                    if (TorqueToPower(torqueincell, rpm) > peakHP)
                    {
                        // set to max
                        int maxtorqueforcell = PowerToTorque(peakHP, rpm, false);
                        int maxairmassforcell = TorqueToAirmass(maxtorqueforcell, E85);
                        logger.Debug("Setting " + valueincell.ToString() + " to " + maxairmassforcell.ToString() + " at " + rpm.ToString() + " rpm");
                        valueincell = maxairmassforcell;
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
            //int[] supportpoints = new int[16] {0xFFD8, 0xFFEC, 0, 20, 40, 60, 80, 100, 125, 150, 175, 200, 225, 250, 275, 300};
            int[] supportpoints = new int[13] { /*0xFFD8, 0xFFEC, 0,*/ 20, 40, 60, 80, 100, 125, 150, 175, 200, 225, 250, 275, 300 };

            byte[] returnvalue = new byte[26];
            for (int i = 0; i < 13; i++)
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

            byte[] returnvalue = new byte[30];
            int range = maxairmass - 140;
            int airmassperstep = range / 14;
            for (int i = 0; i < 15; i++)
            {
                int valueincell = 140 + (airmassperstep * i);
                byte b1 = Convert.ToByte(valueincell / 256);
                byte b2 = Convert.ToByte(valueincell - (int)b1 * 256);
                returnvalue[i * 2] = b1;
                returnvalue[(i * 2) + 1] = b2;
            }
            return returnvalue;


            /*            byte[] returnvalue = new byte[36];
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
                        return returnvalue;*/
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
                        dr["SRAMAddress"] = sramaddress;
                        dr["Length"] = length;
                        dr["UserDefined"] = 0;
                        dr["Delay"] = delay;
                        dr["Reload"] = delay;
                        if (symbolnumber == 0 && sramaddress == 0)
                        {
                            // This is used to update the peak value for combiadapter channels+EGT
                            dr["Value"] = value;
                            try
                            {
                                if (Convert.ToDouble(dr["Peak"]) < value) dr["Peak"] = value;
                            }
                            catch (Exception peakE)
                            {
                                logger.Debug("Failed to set peak: " + peakE.Message);
                            }
                        }
                        else
                        {
                            // Cannot overwrite the last peakvalue until we know its not needed for combiadapter channels+EGT
                            dr["Peak"] = peak;
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
                // rpm should always be present
                if (!m_appSettings.UseAdditionalCanbusFrames) // <GS-05042011> otherwise, get it from the data that is already on the bus
                {
                    if (SymbolExists("ActualIn.n_Engine")) AddToRealtimeTable(dt, "ActualIn.n_Engine", "Engine speed", GetSymbolNumber(m_symbols, "ActualIn.n_Engine"), 0, 0, 1, 0, 0, 8000, GetSymbolNumber(m_symbols, "ActualIn.n_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.n_Engine"), GetSymbolLength(m_symbols, "ActualIn.n_Engine"), 1);
                    if (SymbolExists("In.v_Vehicle")) AddToRealtimeTable(dt, "In.v_Vehicle", "Vehicle speed", GetSymbolNumber(m_symbols, "In.v_Vehicle"), 0, 0, 0.1, 0, 0, 300, GetSymbolNumber(m_symbols, "In.v_Vehicle"), (uint)GetSymbolAddressSRAM(m_symbols, "In.v_Vehicle"), GetSymbolLength(m_symbols, "In.v_Vehicle"), 3);
                    if (SymbolExists("Out.X_AccPedal")) AddToRealtimeTable(dt, "Out.X_AccPedal", "TPS %", GetSymbolNumber(m_symbols, "Out.X_AccPedal"), 0, 0, 0.1, 0, 0, 100, GetSymbolNumber(m_symbols, "Out.X_AccPedal"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.X_AccPedal"), GetSymbolLength(m_symbols, "Out.X_AccPedal"), 1);
                }
                if (SymbolExists("ActualIn.T_Engine")) AddToRealtimeTable(dt, "ActualIn.T_Engine", "Engine temperature", GetSymbolNumber(m_symbols, "ActualIn.T_Engine"), 0, 0, 1, 0, -20, 120, GetSymbolNumber(m_symbols, "ActualIn.T_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.T_Engine"), GetSymbolLength(m_symbols, "ActualIn.T_Engine"), 5);
                if (SymbolExists("ActualIn.T_AirInlet")) AddToRealtimeTable(dt, "ActualIn.T_AirInlet", "Intake air temperature", GetSymbolNumber(m_symbols, "ActualIn.T_AirInlet"), 0, 0, 1, 0, -20, 120, GetSymbolNumber(m_symbols, "ActualIn.T_AirInlet"), (uint)GetSymbolAddressSRAM(m_symbols, "ActualIn.T_AirInlet"), GetSymbolLength(m_symbols, "ActualIn.T_AirInlet"), 3);
                if (SymbolExists("ECMStat.ST_ActiveAirDem")) AddToRealtimeTable(dt, "ECMStat.ST_ActiveAirDem", "Active air demand map", GetSymbolNumber(m_symbols, "ECMStat.ST_ActiveAirDem"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "ECMStat.ST_ActiveAirDem"), (uint)GetSymbolAddressSRAM(m_symbols, "ECMStat.ST_ActiveAirDem"), GetSymbolLength(m_symbols, "ECMStat.ST_ActiveAirDem"), 1);
                if (SymbolExists("Lambda.Status")) AddToRealtimeTable(dt, "Lambda.Status", "Lambda status", GetSymbolNumber(m_symbols, "Lambda.Status"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "Lambda.Status"), (uint)GetSymbolAddressSRAM(m_symbols, "Lambda.Status"), GetSymbolLength(m_symbols, "Lambda.Status"), 1);
                if (SymbolExists("FCut.CutStatus")) AddToRealtimeTable(dt, "FCut.CutStatus", "Fuelcut status", GetSymbolNumber(m_symbols, "FCut.CutStatus"), 0, 0, 1, 0, 0, 255, GetSymbolNumber(m_symbols, "FCut.CutStatus"), (uint)GetSymbolAddressSRAM(m_symbols, "FCut.CutStatus"), GetSymbolLength(m_symbols, "FCut.CutStatus"), 1);
                if (SymbolExists("IgnProt.fi_Offset")) AddToRealtimeTable(dt, "IgnProt.fi_Offset", "Ignition offset", GetSymbolNumber(m_symbols, "IgnProt.fi_Offset"), 0, 0, 0.1, 0, -20, 20, GetSymbolNumber(m_symbols, "IgnProt.fi_Offset"), (uint)GetSymbolAddressSRAM(m_symbols, "IgnProt.fi_Offset"), GetSymbolLength(m_symbols, "IgnProt.fi_Offset"), 1);
                if (SymbolExists("m_Request")) AddToRealtimeTable(dt, "m_Request", "Requested airmass", GetSymbolNumber(m_symbols, "m_Request"), 0, 0, 1, 0, 0, 600, GetSymbolNumber(m_symbols, "m_Request"), (uint)GetSymbolAddressSRAM(m_symbols, "m_Request"), GetSymbolLength(m_symbols, "m_Request"), 1);
                if (SymbolExists("Out.M_Engine")) AddToRealtimeTable(dt, "Out.M_Engine", "Calculated torque", GetSymbolNumber(m_symbols, "Out.M_Engine"), 0, 0, 1, 0, 0, 600, GetSymbolNumber(m_symbols, "Out.M_Engine"), (uint)GetSymbolAddressSRAM(m_symbols, "Out.M_Engine"), GetSymbolLength(m_symbols, "Out.M_Engine"), 1);

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
                if (SymbolExists("BFuelProt.CurrentFuelCon")) AddToRealtimeTable(dt, "BFuelProt.CurrentFuelCon", "Fuel consumption", GetSymbolNumber(m_symbols, "BFuelProt.CurrentFuelCon"), 0, 0, 0.1, 0, 0, 50, GetSymbolNumber(m_symbols, "BFuelProt.CurrentFuelCon"), (uint)GetSymbolAddressSRAM(m_symbols, "BFuelProt.CurrentFuelCon"), GetSymbolLength(m_symbols, "BFuelProt.CurrentFuelCon"), 2);
                
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
                        if (m_appSettings.AutoCreateAFRMaps)
                        {
                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out _width, out _height);
                            m_AFRMap.RpmYSP = GetSymbolAsIntArray("BFuelCal.RpmYSP");
                            m_AFRMap.AirXSP = GetSymbolAsIntArray("BFuelCal.AirXSP");
                            m_AFRMap.InitializeMaps(_width * _height, m_currentfile);
                        }
                    }
                }
                else
                {
                    if (SymbolExists("Lambda.LambdaInt")) AddToRealtimeTable(dt, "Lambda.LambdaInt", "Lambda value (nbO2)", GetSymbolNumber(m_symbols, "Lambda.LambdaInt"), 0, 1, 0.0001, 0, 0, 2, GetSymbolNumber(m_symbols, "Lambda.LambdaInt"), (uint)GetSymbolAddressSRAM(m_symbols, "Lambda.LambdaInt"), GetSymbolLength(m_symbols, "Lambda.LambdaInt"), 1);
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
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

            panelBottom.Visible = (e.Page != xtraTabPageEmpty);
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void OpenAndDisplayLogFile(string filename)
        {
            // create a new dock with a graph view in it
            DockPanel dp = dockManager1.AddPanel(DockingStyle.Left);
            dp.Size = new Size(dockManager1.Form.ClientSize.Width - dockSymbols.Width, dockSymbols.Height);
            dp.Hide();
            dp.Text = "CANBus logfile: " + Path.GetFileName(filename);
            RealtimeGraphControl lfv = new RealtimeGraphControl(suiteRegistry);
            LogFilters lfhelper = new LogFilters(suiteRegistry);
            lfv.SetFilters(lfhelper.GetFiltersFromRegistry());
            dp.Controls.Add(lfv);
            lfv.ImportT5Logfile(filename);
            lfv.Dock = DockStyle.Fill;
            dp.Show();
        }

        private void btnViewLogFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            // open a logfile from the canlog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 logfiles|*.t7l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenAndDisplayLogFile(ofd.FileName);
            }
        }

        private void btnSetSymbolColors_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmPlotSelection plotsel = new frmPlotSelection(suiteRegistry);
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Start_address > 0)
                {
                    plotsel.AddItemToList(sh.SmartVarname);
                }
            }
            plotsel.ShowDialog();
        }

        private void btnReadFaultCodes_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (RealtimeCheckAndConnect())
            {
                // 16 25 16 22 00 00 00 00 00 00 00 00 
                frmFaultcodes frmfaults = new frmFaultcodes();
                frmfaults.onClearCurrentDTC += new frmFaultcodes.onClearDTC(frmfaults_onClearCurrentDTC);
                m_prohibitReading = true;
                bool _success = false;
                string faultCodes = string.Empty;
                int symbolnumber = GetSymbolNumber(m_symbols, "obdFaults");
                if (symbolnumber == 0)
                {
                    // not connected to ECU
                    frmInfoBox info = new frmInfoBox("Cannot find symbolnumber for symbol obdFaults, ECU binary must be loaded");
                }
                byte[] buffer = ReadSymbolFromSRAM((uint)symbolnumber, "obdFaults", (uint)GetSymbolAddressSRAM(m_symbols, "obdFaults"), GetSymbolLength(m_symbols, "obdFaults"), out _success);
                if (_success)
                {
                    for (int t = 0; t < buffer.Length; t += 2)
                    {
                        if (buffer[t] == 0x00 && buffer[t + 1] == 0x00)
                        {
                            break;
                        }
                        else
                        {
                            //faultCodes += "P" + buffer[t].ToString("X2") + buffer[t + 1].ToString("X2") + Environment.NewLine;
                            frmfaults.addFault("P" + buffer[t].ToString("X2") + buffer[t + 1].ToString("X2"));
                        }
                    }
                }

                frmfaults.Show();
                m_prohibitReading = false;

            }
            else
            {
                // not connected to ECU
                frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to read faultcodes");
            }
        }

        void frmfaults_onClearCurrentDTC(object sender, frmFaultcodes.ClearDTCEventArgs e)
        {
            // clear the currently selected DTC code from the ECU
            if (e.DTCCode.StartsWith("P"))
            {
                m_prohibitReading = true;
                try
                {
                    int DTCCode = Convert.ToInt32(e.DTCCode.Substring(1, e.DTCCode.Length - 1), 16);
                    if (RealtimeCheckAndConnect())
                    {
                        trionic7.ClearDTCCode(DTCCode);
                    }
                    if (sender is frmFaultcodes)
                    {
                        frmFaultcodes frmfaults = (frmFaultcodes)sender;
                        bool _success = false;
                        frmfaults.Init();
                        int symbolnumber = GetSymbolNumber(m_symbols, "obdFaults");
                        if (symbolnumber == 0)
                        {
                            // not connected to ECU
                            frmInfoBox info = new frmInfoBox("Cannot find symbolnumber for symbol obdFaults, ECU binary must be loaded");
                        }
                        byte[] buffer = ReadSymbolFromSRAM((uint)symbolnumber, "obdFaults", (uint)GetSymbolAddressSRAM(m_symbols, "obdFaults"), GetSymbolLength(m_symbols, "obdFaults"), out _success);
                        if (_success)
                        {
                            for (int t = 0; t < buffer.Length; t += 2)
                            {
                                if (buffer[t] == 0x00 && buffer[t + 1] == 0x00)
                                {
                                    break;
                                }
                                else
                                {
                                    //faultCodes += "P" + buffer[t].ToString("X2") + buffer[t + 1].ToString("X2") + Environment.NewLine;
                                    frmfaults.addFault("P" + buffer[t].ToString("X2") + buffer[t + 1].ToString("X2"));
                                }
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
                m_prohibitReading = false;
            }
        }

        private void btnClearDTCs_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (RealtimeCheckAndConnect())
            {
                m_prohibitReading = true;
                trionic7.ReadDTC();
                trionic7.ClearDTCCodes();
                m_prohibitReading = false;
            }
        }

        private void ShowRealtimeMapFromECU(string symbolname)
        {
            if (RealtimeCheckAndConnect())
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.SmartVarname == symbolname)
                    {
                        // convert to realtime symbol
                        // start an SRAM mapviewer for this symbol
                        int symbolnumber = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, symbolname), symbolname);
                        sh.Symbol_number = symbolnumber;

                        logger.Debug("Got symbolnumber: " + symbolnumber.ToString() + " for map: " + symbolname);
                        if (symbolnumber >= 0)
                        {
                            byte[] result = ReadMapFromSRAM(sh, true);
                            logger.Debug("read " + result.Length.ToString() + " bytes from SRAM!");
                            StartTableViewer(symbolname);
                            try
                            {
                                int rows = 0;
                                int cols = 0;
                                foreach (DockPanel pnl in dockManager1.Panels)
                                {
                                    if (pnl.Text.StartsWith("Symbol: ") || pnl.Text.StartsWith("SRAM"))
                                    {
                                        foreach (Control c in pnl.Controls)
                                        {
                                            if (c is IMapViewer)
                                            {
                                                IMapViewer vwr = (IMapViewer)c;
                                                if (vwr.Map_name == symbolname)
                                                {
                                                    vwr.Map_content = result;
                                                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, symbolname, out cols, out rows);
                                                    vwr.ShowTable(cols, isSixteenBitTable(symbolname));
                                                }
                                            }
                                            else if (c is DockPanel)
                                            {
                                                DockPanel tpnl = (DockPanel)c;
                                                foreach (Control c2 in tpnl.Controls)
                                                {
                                                    if (c2 is IMapViewer)
                                                    {
                                                        IMapViewer vwr2 = (IMapViewer)c2;
                                                        if (vwr2.Map_name == symbolname)
                                                        {
                                                            vwr2.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, symbolname, out cols, out rows);
                                                            vwr2.ShowTable(cols, isSixteenBitTable(symbolname));
                                                        }
                                                    }
                                                }
                                            }
                                            else if (c is ControlContainer)
                                            {
                                                ControlContainer cntr = (ControlContainer)c;
                                                foreach (Control c3 in cntr.Controls)
                                                {
                                                    if (c3 is IMapViewer)
                                                    {
                                                        IMapViewer vwr3 = (IMapViewer)c3;
                                                        if (vwr3.Map_name == symbolname)
                                                        {
                                                            vwr3.Map_content = result;
                                                            GetTableMatrixWitdhByName(m_currentfile, m_symbols, symbolname, out cols, out rows);
                                                            vwr3.ShowTable(cols, isSixteenBitTable(symbolname));
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
                                logger.Debug("Refresh viewer with SRAM data error: " + E.Message);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void btnViewKnockCountMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowRealtimeMapFromECU("KnkDetAdap.KnkCntMap");
        }

        private void writeSymbolToSRAMToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem86_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowRealtimeMapFromECU("MissfAdap.MissfCntMap");
        }

        private void barButtonItem88_ItemClick(object sender, ItemClickEventArgs e)
        {
            // real knock
            ShowRealtimeMapFromECU("F_KnkDetAdap.RKnkCntMap");
        }

        private void barButtonItem87_ItemClick(object sender, ItemClickEventArgs e)
        {
            // false knock
            ShowRealtimeMapFromECU("F_KnkDetAdap.FKnkCntMap");
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // 
            //e.Graphics.DrawString("Test", this.Font, Brushes.Black, new PointF(100, 100));
            //_cu
            _currentEngineStatus.HeadlightsOn = true;
            if (_currentEngineStatus.HeadlightsOn)
            {
                // draw light beams on the image
                //point1 = 68, 52
                //point1 = 0, 31
                //point1 = 24, 107
                //point1 = 0, 113
                PointF[] pnts = new PointF[4];
                pnts[0] = new PointF(68, 52);
                pnts[1] = new PointF(0, 31);
                pnts[2] = new PointF(0, 113);
                pnts[3] = new PointF(24, 107);
                e.Graphics.FillPolygon(Brushes.Yellow, pnts);
            }

        }

        private void barUpdateText_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            // TEST
            //System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://develop.trionictuning.com/T7Suite/Notes.xml" /*Application.UserAppDataPath + "\\T7Suite.html"*/);
            System.Diagnostics.Process.Start("IEXPLORE.EXE", "http://develop.trionictuning.com/T7Suite/Notes.xml");
            // END TEST

        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }

        private void SwitchAFRMode()
        {
            // switch mode AFR/Lambda
            if (AfrViewMode == AFRViewType.AFRMode)
            {
                // rescale the gauge
                linearGauge2.MaxValue = 1.5F;
                linearGauge2.MinValue = 0.5F;
                linearGauge2.GaugeText = "λ ";
                labelControl11.Text = "λ";
                linearGauge2.NumberOfDecimals = 2;
                linearGauge2.NumberOfDivisions = 10;
                AfrViewMode = AFRViewType.LambdaMode;
                btnAFRFeedbackMap.Caption = "Show lambda feedback map";
                btnClearAFRFeedback.Caption = "Clear lambda feedback map";
            }
            else
            {
                linearGauge2.MaxValue = 20;
                linearGauge2.MinValue = 10;
                linearGauge2.GaugeText = "AFR ";
                labelControl11.Text = "AFR";
                linearGauge2.NumberOfDecimals = 1;
                AfrViewMode = AFRViewType.AFRMode;
                btnAFRFeedbackMap.Caption = "Show AFR feedback map";
                btnClearAFRFeedback.Caption = "Clear AFR feedback map";
            }
        }

        private void linearGauge2_Click(object sender, EventArgs e)
        {
            SwitchAFRMode();
        }

        // Once every x seconds, read the Performance.Mode to show the active mode in the realtime panel

        private void btnEconomyMode_Click(object sender, EventArgs e)
        {
            SetPerformanceMode(0);
        }

        private void btnNormalMode_Click(object sender, EventArgs e)
        {
            SetPerformanceMode(1);
        }

        private void btnSportMode_Click(object sender, EventArgs e)
        {
            SetPerformanceMode(2);
        }

        private void exportAsTuningPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // export selected maps as tuning package (name the file t7p)
            // get selected rows
            int[] selectedrows = gridViewSymbols.GetSelectedRows();
            SymbolCollection scToExport = new SymbolCollection();
            if (selectedrows.Length > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Trionic 7 packages|*.t7p";
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
                            if (IsSoftwareOpen())
                            {
                                pe.AddressOffset = GetOpenFileOffset();
                            }
                            pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
                        }

                    }
                }

            }
        }

        public enum FileTuningPackType
        {
            None = 0,
            SymbolTp,
            ApplyBin,
            SearchAndReplace
        };

        public class FileTuningPackage
        {
            public FileTuningPackType type;
            public bool succesful;
            public FileTuningPackage() { }
            public string result;
            public bool hasResult = false;

            public virtual string GetNameTPAction()
            {
                return "";
            }

        }

        public class SymbolFileTuningPackage : FileTuningPackage
        {
            public SymbolHelper sh_Import;
            public int addressInFile;
            public byte[] dataToInsert;

            public SymbolFileTuningPackage(SymbolHelper _sh_Import, int _addressInFile, byte[] _dataToInsert, bool _successful)
            {
                type = FileTuningPackType.SymbolTp;
                succesful = _successful;
                sh_Import = _sh_Import;
                addressInFile = _addressInFile;
                dataToInsert = _dataToInsert;
            }

            public override string GetNameTPAction()
            {
                return sh_Import.Varname;
            }
        }

        public class SearchReplaceTuningPackage : FileTuningPackage
        {
            string _name;
            public SearchReplacePattern srp; // byte[] _SearchPattern, byte[] _ReplaceWith, byte[][][] _CheckHeadAndTail
            private static void replaceSymbolsWithBytes(ref string[] workString) //works through the referenced string[] replacing symbols with corresponding bytes
            {
                List<string> newsearchString = new List<string>();
                foreach (string searchpart in workString)
                {
                    //Check to see if the part of the  statement is a symbol (begins  with *)
                    if (searchpart[0] == '*')
                    {
                        string searchedSymbol = searchpart.Substring(1);
                        foreach (SymbolHelper cfsh in m_symbols)
                        {
                            if (cfsh.SmartVarname == searchedSymbol)
                            {
                                byte[] bSymSearch = { };
                                bSymSearch = BitConverter.GetBytes((int)cfsh.Flash_start_address);
                                Array.Reverse(bSymSearch);
                                //break the address up in bytes, add each byte to newsearchString.
                                string tempBytes = BitConverter.ToString(bSymSearch);
                                string[] tempindBytes = tempBytes.Substring(0, tempBytes.Length).Split('-');
                                foreach (string bajt in tempindBytes)
                                {
                                    newsearchString.Add("0x" + bajt);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        //if not a a symbol, add it verbatim to the new string
                        newsearchString.Add(searchpart);
                    }
                }
                //converts newreplacestring to an array and replaces referenced string[] with it
                workString = newsearchString.ToArray();
            }
            public SearchReplaceTuningPackage(string searchReplace)
            {
                type = FileTuningPackType.SearchAndReplace;

                //
                // Parses a string and transform it to byte arrays
                // 'Name',{SEARCH},{REPLACE},{{{HEAD_1},{TAIL_1}},{{HEAD_N},{TAIL__N}}}
                // E.g.
                // 'NEW',{0x3D,0x7C,0x0C,0x4E},{0x3D,0x7C,0x0F,0xA0},{{{0x01, 0xAA},{0xFF}},{{},{0xFE}}}
                // 'NEW',{0x3D,0x7C},{0x0F,0xA0},{{{0x01, 0xAA},{0xFF}}}
                //
                int foundS1 = 0;
                int foundS2 = 0;
                byte[] bSearch = new byte[] { };
                byte[] bReplace = new byte[] { };
                byte[][][] myCheckHeadAndTail = new byte[][][] { };
                List<byte[][]> headTailList = new List<byte[][]>();

                // Validate input
                int count_in = 0;
                int count_out = 0;
                int count_in_br = 0;
                int count_out_br = 0;
                int count_st = 0;
                int count_cm = 0;
                foreach (char c in searchReplace)
                {
                    if (c == '{') count_in++;
                    if (c == '}') count_out++;
                    if (c == '[') count_in_br++;
                    if (c == ']') count_out_br++;
                    if (c == '\'') count_st++;
                    if (c == ',') count_cm++;
                }
                if (count_in_br > 0)
                {
                    if ((count_in != 2 || count_in != count_out || count_in_br != 1 || count_in_br != count_out_br || count_st != 2 || count_cm < 2))
                    {
                        _name = "FAIL IN REP VALIDATION: " + searchReplace.Substring(0, 6) + "...";
                        srp = new SearchReplacePattern(bSearch, bReplace, myCheckHeadAndTail);
                        return;
                    }
                }
                else
                {
                    if ((count_in < 6 || count_in != count_out || count_st != 2 || count_cm < 4))
                    {
                        _name = "FAIL IN SnR VALIDATION: " + searchReplace.Substring(0, 6) + "...";
                        srp = new SearchReplacePattern(bSearch, bReplace, myCheckHeadAndTail);
                        return;
                    }
                }

                // Create a string to work with
                string inputStr = searchReplace.Trim();

                // Extract the name
                foundS1 = inputStr.IndexOf('\'') + 1;
                foundS2 = inputStr.IndexOf('\'', foundS1);
                _name = inputStr.Substring(foundS1, foundS2 - foundS1);
                inputStr = inputStr.Remove(0, foundS2 + 2);

                // Remove all whitespace
                inputStr = Regex.Replace(inputStr, @"\s+", "");

                // Extract search pattern
                foundS1 = inputStr.IndexOf('{') + 1;
                foundS2 = inputStr.IndexOf('}');

                // Simple replace
                int foundS1B = 0;
                int foundS2B = 0;
                string adress_string = string.Empty;
                int address = -1;

                if ((foundS1B = inputStr.IndexOf('[')) >= 0)
                {
                    foundS1B++;
                    foundS2B = inputStr.IndexOf(']');
                    adress_string = inputStr.Substring(foundS1B, foundS2B - foundS1B);
                    adress_string = adress_string.Replace("0x", "");
                    address = Int32.Parse(adress_string, System.Globalization.NumberStyles.HexNumber);
                    if (address == -1)
                    {
                        bSearch = new byte[] { };
                        bReplace = new byte[] { };
                        _name = _name + " failing for unknown reason";
                        srp = new SearchReplacePattern(bSearch, bReplace, myCheckHeadAndTail);
                        return;
                    }
                }
                else
                {
                    string[] searchString = inputStr.Substring(foundS1, foundS2 - foundS1).Split(',');
                    //creates a temporary collection list for the search strings.
                    replaceSymbolsWithBytes(ref searchString);
                    bSearch = new byte[searchString.Length];
                    for (int i = 0; i < searchString.Length; i++)
                    {
                        bSearch[i] = Convert.ToByte(searchString[i].Trim(), 16);
                    }
                }
                inputStr = inputStr.Remove(0, foundS2 + 2);

                // Extract replace pattern
                foundS1 = inputStr.IndexOf('{') + 1;
                foundS2 = inputStr.IndexOf('}');
                string[] replaceString = inputStr.Substring(foundS1, foundS2 - foundS1).Split(',');
                //creates a temporary collection list for the replace strings.
                replaceSymbolsWithBytes(ref replaceString);
                bReplace = new byte[replaceString.Length];
                for (int i = 0; i < replaceString.Length; i++)
                {
                    bReplace[i] = Convert.ToByte(replaceString[i].Trim(), 16);
                }

                // Check that the search and replace match in length
                if ((address == -1) && (bSearch.Length != bReplace.Length))
                {
                    bSearch = new byte[] { };
                    bReplace = new byte[] { };
                    _name = _name + " failing due to mismatch in length";
                    srp = new SearchReplacePattern(bSearch, bReplace, myCheckHeadAndTail);
                    return;
                }

                if (address == -1) // No head/tail in a replace
                {
                    // Parse the head/tail arrays, e.g. {{{0xAA,0xBB},{0xCC,0xDD}},{{0xEE,0xFF},{0x11, 0x22}}}
                    // Remove leading "{{{"
                    inputStr = inputStr.Remove(0, foundS2 + 2);
                    for (int i = 0; i < 3; i++)
                        inputStr = inputStr.Remove(0, inputStr.IndexOf('{') + 1);

                    // Split the multiple headtails found by "}},{{". Whitespace already removed
                    string[] headTails = Regex.Split(inputStr, @"}},{{");

                    // Parse through the results, always start with a number or }
                    foreach (string headTail in headTails)
                    {
                        byte[] bHead = { };
                        byte[] bTail = { };
                        string ht = headTail;

                        // Extract the head
                        foundS1 = 0;
                        foundS2 = headTail.IndexOf('}');
                        string head = headTail.Substring(0, foundS2);

                        // Find the start of tail
                        foundS1 = ht.IndexOf('{');
                        ht = ht.Remove(0, foundS1 + 1);

                        // Find the end of the tail
                        foundS1 = 0;
                        foundS2 = ht.IndexOf('}');
                        string tail;
                        if (foundS2 <= 0) //All but last string lacks a }
                            tail = ht;
                        else
                            tail = ht.Substring(0, foundS2);

                        // Convert head to byte array
                        if (head.Length > 0)
                        {
                            if (head[0] == '*')
                            {
                                // This is as symbol, find it's flash address (XXXTBDXXX)
                                string searchSymbol = head.Substring(1);
                                foreach (SymbolHelper cfsh in m_symbols)
                                {
                                    if (cfsh.SmartVarname == searchSymbol)
                                    {
                                        bHead = BitConverter.GetBytes((int)cfsh.Flash_start_address);
                                        Array.Reverse(bHead);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                string[] lHead = head.Split(',');
                                bHead = new byte[lHead.Length];
                                for (int i = 0; i < lHead.Length; i++)
                                {
                                    if (lHead[i].Length > 0)
                                    {
                                        bHead[i] = Convert.ToByte(lHead[i].Trim(), 16);
                                    }
                                    else
                                    {
                                        bHead = new byte[] { };
                                        break;
                                    }
                                }
                            }
                        }

                        // Convert tail to byte array
                        if (tail.Length > 0)
                        {
                            if (tail[0] == '*')
                            {
                                // This is as symbol, find it's flash address (XXXTBDXXX)
                                string searchSymbol = tail.Substring(1);
                                foreach (SymbolHelper cfsh in m_symbols)
                                {
                                    if (cfsh.SmartVarname == searchSymbol)
                                    {
                                        bTail = BitConverter.GetBytes((int)cfsh.Flash_start_address);
                                        Array.Reverse(bTail);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                string[] lTail = tail.Split(',');
                                bTail = new byte[lTail.Length];
                                for (int i = 0; i < lTail.Length; i++)
                                {
                                    if (lTail[i].Length > 0)
                                    {
                                        bTail[i] = Convert.ToByte(lTail[i].Trim(), 16);
                                    }
                                    else
                                    {
                                        bTail = new byte[] { };
                                        break;
                                    }
                                }
                            }
                        }

                        // Add headtail to list
                        byte[][] bHeadTail = { bHead, bTail };
                        headTailList.Add(bHeadTail);
                    }

                    // Store headtail for output
                    myCheckHeadAndTail = new byte[headTailList.Count][][];
                    for (int i = 0; i < headTailList.Count; i++)
                    {
                        myCheckHeadAndTail[i] = headTailList[i];
                    }
                    srp = new SearchReplacePattern(bSearch, bReplace, myCheckHeadAndTail);
                }
                else
                {
                    srp = new SearchReplacePattern(address, bReplace);
                }

            }

            public override string GetNameTPAction()
            {
                if (!hasResult)
                    return _name;
                else
                    return result;
            }
        }

        public class BinFileTuningPackage : FileTuningPackage
        {
            string _binAction;

            public BinFileTuningPackage(string binAction)
            {
                type = FileTuningPackType.ApplyBin;
                _binAction = binAction;
            }

            public override string GetNameTPAction()
            {
                if (!hasResult)
                    return _binAction;
                else
                    return result;
            }
        }

        private void ImportTuningPackage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 packages|*.t7p";
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
                                    if (_softwareIsOpen && sh_Import.Varname == "MapChkCal.ST_Enable")
                                    {
                                        dt.Rows.Add(sh_Import.Varname, "Skipped");
                                    }
                                    else
                                    {
                                        savedatatobinary(addressInFile, sh_Import.Length, dataToInsert, m_currentfile, true);
                                        // add successful
                                        dt.Rows.Add(sh_Import.Varname, "Success");
                                    }
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
                                logger.Debug(E.Message);
                            }
                        }
                    }
                }
                UpdateChecksum(m_currentfile);
                frmImportResults res = new frmImportResults();
                res.SetDataTable(dt);
                res.ShowDialog();
            }
        }

        private List<FileTuningPackage> ReadTuningPackageFile(bool encoded, string tpFile, out string binSwType, out string whitelist, out string blacklist, out string code)
        {
            char[] sep = new char[1];
            sep.SetValue(',', 0);
            binSwType = string.Empty;
            whitelist = string.Empty;
            blacklist = string.Empty;
            code = string.Empty;
            List<FileTuningPackage> lstTp = new List<FileTuningPackage>();

            using (StreamReader sr = new StreamReader(tpFile))
            {
                string line = string.Empty;
                string in_line = string.Empty;
                SymbolHelper sh_Import = new SymbolHelper();

                if (encoded)
                {
                    // Read signature
                    string signature = string.Empty;
                    string s = sr.ReadLine();
                    if (s != null && s.StartsWith("<SIGNATURE>"))
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (s.StartsWith("</SIGNATURE>"))
                                break;
                            signature += s;
                        }
                    }
                }
                while ((in_line = sr.ReadLine()) != null)
                {
                    if (encoded)
                        line = Crypto.DecodeAES(in_line).Trim();
                    else
                        line = in_line.Trim();

                    if (line.StartsWith("packname="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("bintype="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("whitelist="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("blacklist="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("code="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("author="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("msg="))
                    {
                        // Do nothing
                    }
                    else if (line.StartsWith("binaction="))
                    {
                        string inS = line.Replace("binaction=", "");
                        FileTuningPackage binTP = new BinFileTuningPackage(inS);
                        lstTp.Add(binTP);
                    }
                    else if (line.StartsWith("searchreplace="))
                    {
                        string inS = line.Replace("searchreplace=", "");
                        FileTuningPackage srTP = new SearchReplaceTuningPackage(inS);
                        lstTp.Add(srTP);
                    }
                    else if (line.StartsWith("symbol="))
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
                            if (addressInFile != 0)
                            {
                                FileTuningPackage fileTP = new SymbolFileTuningPackage(sh_Import, addressInFile, dataToInsert, true);
                                lstTp.Add(fileTP);
                            }
                            else
                            {
                                FileTuningPackage fileTP = new SymbolFileTuningPackage(sh_Import, addressInFile, dataToInsert, false);
                                lstTp.Add(fileTP);
                            }
                        }
                        catch (Exception)
                        {
                            // add failure
                            byte[] dataToInsert = new byte[0];
                            FileTuningPackage fileTP = new SymbolFileTuningPackage(sh_Import, 0, dataToInsert, false);
                            lstTp.Add(fileTP);
                        }
                    }
                }
            }
            return lstTp;
        }

        private void ApplyTuningPackage(List<FileTuningPackage> fileTP)
        {
            string log_file = Path.GetFileNameWithoutExtension(m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-WIZARD.log";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            log_file = r.Replace(log_file, "");
            log_file = Path.GetDirectoryName(m_currentfile) + "\\" + log_file;

            using (StreamWriter log = new StreamWriter(log_file))
            {
                foreach (FileTuningPackage fTP in fileTP)
                {
                    if (fTP.type == FileTuningPackType.SymbolTp)
                    {
                        SymbolFileTuningPackage sTP = (SymbolFileTuningPackage)fTP;
                        if (sTP.addressInFile > 0)
                        {
                            log.WriteLine("Updating symbol: " + sTP.GetNameTPAction());
                            savedatatobinary(sTP.addressInFile, sTP.sh_Import.Length, sTP.dataToInsert, m_currentfile, true);
                        }
                        if (!sTP.succesful)
                        {
                            log.WriteLine("Updating symbol: " + sTP.GetNameTPAction() + " FAILED");
                        }
                    }
                    else if (fTP.type == FileTuningPackType.ApplyBin)
                    {
                        string result = "";
                        fTP.succesful = performBinAction(fTP.GetNameTPAction(), out result);
                        if (result != "")
                        {
                            fTP.hasResult = true;
                            fTP.result = result;
                        }
                        log.WriteLine("Applied binaction: " + fTP.GetNameTPAction());

                    }
                    else if (fTP.type == FileTuningPackType.SearchAndReplace)
                    {
                        int num = 0;
                        SearchReplaceTuningPackage srtp = (SearchReplaceTuningPackage)fTP;
                        num = performSearchAndReplace(srtp.srp);
                        srtp.result = srtp.GetNameTPAction() + ": " + num.ToString() + " replacements";
                        srtp.hasResult = true;
                        if (num > 0)
                            srtp.succesful = true;

                        log.WriteLine("Applied searchandreplace: " + fTP.GetNameTPAction());
                    }
                }
                UpdateChecksum(m_currentfile);
            }
        }

        public bool performBinAction(string binAction, out string result)
        {
            bool retval = false;
            result = "";
            if (binAction == "") // Name your binaction
            {
                // Apply your binaction
                result = "Failed undefined binaction";
                retval = false;
            }
            else
            {
                result = "Failed with binaction=" + binAction;
                retval = false;
            }
            return retval;
        }

        public class SearchReplacePattern
        {
            public int ReplaceAddress;
            public byte[] SearchPattern;
            public byte[] ReplaceWith;
            public byte[][][] CheckHeadAndTail; //(Head XXYY and Tail AABB) or (Head ZZWW and Tail CCDD) or ...
            public SearchReplacePattern(byte[] _SearchPattern, byte[] _ReplaceWith, byte[][][] _CheckHeadAndTail)
            {
                ReplaceAddress = 0;
                SearchPattern = _SearchPattern;
                ReplaceWith = _ReplaceWith;
                CheckHeadAndTail = _CheckHeadAndTail;
            }

            public SearchReplacePattern(int _ReplaceAddress, byte[] _ReplaceWith)
            {
                ReplaceAddress = _ReplaceAddress;
                ReplaceWith = _ReplaceWith;
            }

            public bool isDirectReplace()
            {
                if (ReplaceAddress == 0)
                    return false;
                else
                    return true;
            }
        }

        public void ReplaceBytePattern(byte[] data, SearchReplacePattern srp)
        {
            for (int j = 0; j < srp.ReplaceWith.Length; j++)
            {
                data[srp.ReplaceAddress + j] = srp.ReplaceWith[j];
            }
        }

        public int SearchReplaceBytePattern(byte[] data, SearchReplacePattern srp)
        {
            int matches = 0;

            // Will browse the file multiple times
            // Can be made more effective to read once and match
            // all SearchReplacePattern at each position
            for (int i = 0; i < data.Length - srp.SearchPattern.Length; i++)
            {
                bool match = true;
                for (int k = 0; k < srp.SearchPattern.Length; k++)
                {
                    if (data[i + k] != srp.SearchPattern[k])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    // Check if the Head AND Tail is matching
                    foreach (byte[][] htArray in srp.CheckHeadAndTail)
                    {
                        // Head in htArray[0]
                        bool headmatch = true;
                        for (int j = 0; j < htArray[0].Length; j++)
                        {
                            if (data[i - htArray[0].Length + j] == htArray[0][j])
                            {
                                headmatch = true;
                            }
                            else
                            {
                                headmatch = false;
                                break;
                            }
                        }

                        // Tail in htArray[1]
                        bool tailmatch = true;
                        for (int j = 0; j < htArray[1].Length; j++)
                        {
                            if (data[i + srp.SearchPattern.Length + j] == htArray[1][j])
                            {
                                tailmatch = true;
                            }
                            else
                            {
                                tailmatch = false;
                                break;
                            }
                        }

                        if (match && headmatch && tailmatch)
                        {
                            // Do the actual replacement
                            matches++;
                            for (int j = 0; j < srp.SearchPattern.Length; j++)
                            {
                                data[i + j] = srp.ReplaceWith[j];

                            }
                            break;
                        }
                    }
                }
            }
            return matches;
        }

        public int performSearchAndReplace(SearchReplacePattern sp)
        {
            int num_replacements = 0;

            // Read the complete file into memory since we will scan it over and over again
            if (File.Exists(m_currentfile))
            {
                byte[] buff = File.ReadAllBytes(m_currentfile);

                // Search and replace
                //int num_replacements = ReplaceBytePattern(buff, find_p, replace_p, 0xFF, 0xFE);
                if (sp.isDirectReplace())
                {
                    num_replacements = 1;
                    ReplaceBytePattern(buff, sp);
                }
                else
                {
                    num_replacements = SearchReplaceBytePattern(buff, sp);
                }

                // Store the file
                File.WriteAllBytes(m_currentfile, buff);
            }
            return num_replacements;
        }

        private void ShowAfrMAP(string mapname, string filename)
        {
            // show seperate mapviewer for AFR target map
            DockPanel dockPanel;
            bool pnlfound = false;
            string symbolname = mapname;
            try
            {
                foreach (DockPanel pnl in dockManager1.Panels)
                {
                    if (pnl.Text == "Symbol: " + symbolname + " [" + Path.GetFileName(m_currentfile) + "]")
                    {
                        dockPanel = pnl;
                        pnlfound = true;
                        dockPanel.Show();
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            if (!pnlfound)
            {
                //dockPanel = dockManager1.AddPanel(new System.Drawing.Point(-500, -500));
                dockManager1.BeginUpdate();
                try
                {
                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.Visible = false;
                    tabdet.Filename = m_currentfile;
                    tabdet.Map_name = symbolname;
                    tabdet.Map_descr = "Target AFR map for use with wideband lambda sensor";
                    if (mapname == "FeedbackAFR") tabdet.Map_descr = "Feedback AFR map from wideband lambda sensor";
                    //if (mapname == "FeedbackvsTargetAFR") tabdet.Map_descr = "Feedback AFR minus target AFR map from wideband lambda sensor";
                    tabdet.Map_cat = XDFCategories.Sensor;
                    tabdet.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, "BFuelCal.Map");
                    tabdet.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, "BFuelCal.Map");
                    // z, y and z axis to do

                    if (!m_appSettings.NewPanelsFloating)
                    {
                        dockPanel = dockManager1.AddPanel(DockingStyle.Right);
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
                    // OUD

                    /*
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
                    dockPanel.FloatLocation = floatpoint;*/

                    dockPanel.Tag = m_currentfile;

                    /*string xdescr = string.Empty;
                    string ydescr = string.Empty;
                    string zdescr = string.Empty;
                    GetAxisDescriptions(m_currentfile, m_symbols, "BFuelCal.Map", out xdescr, out ydescr, out zdescr);
                    tabdet.X_axis_name = xdescr;
                    tabdet.Y_axis_name = ydescr;
                    tabdet.Z_axis_name = zdescr;*/
                    SymbolAxesTranslator axestrans = new SymbolAxesTranslator();
                    string x_axis = string.Empty;
                    string y_axis = string.Empty;
                    string x_axis_descr = string.Empty;
                    string y_axis_descr = string.Empty;
                    string z_axis_descr = string.Empty;
                    axestrans.GetAxisSymbols("BFuelCal.Map", out x_axis, out y_axis, out x_axis_descr, out y_axis_descr, out z_axis_descr);
                    tabdet.X_axis_name = x_axis_descr;
                    tabdet.Y_axis_name = y_axis_descr;
                    tabdet.Z_axis_name = z_axis_descr;

                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out columns, out rows);

                    tabdet.Map_address = 0;
                    tabdet.Map_sramaddress = 0;
                    int length = GetSymbolLength(m_symbols, "BFuelCal.Map");
                    tabdet.Map_length = length * 2;

                    if (mapname == "FeedbackAFR")
                    {
                        byte[] mapdata = m_AFRMap.GetFeedbackMapInBytes(length);//LoadTargetAFRMapInBytes(filename);//TODO: ???
                        tabdet.Map_content = mapdata;
                        tabdet.Afr_counter = m_AFRMap.GetAFRCountermap();
                        tabdet.Correction_factor = 0.1;
                        tabdet.Correction_offset = 0;


                    }
                    if (mapname == "FeedbackCounter")
                    {
                        byte[] mapdata = m_AFRMap.GetFeedbackCounterMapInBytes(length);//LoadTargetAFRMapInBytes(filename);//TODO: ???
                        tabdet.Map_content = mapdata;
                        tabdet.Correction_factor = 1;
                        tabdet.Correction_offset = 0;
                    }
                    if (mapname == "TargetAFR")
                    {
                        byte[] mapdata = m_AFRMap.GetTargetAFRMapinBytes(length, m_currentfile);//LoadTargetAFRMapInBytes(filename);//TODO: ???
                        tabdet.Map_content = mapdata;
                        tabdet.Correction_factor = 0.1;
                        tabdet.Correction_offset = 0;
                    }
                    tabdet.IsUpsideDown = true;
                    tabdet.ShowTable(columns, true);
                    TryToAddOpenLoopTables(tabdet);


                    tabdet.Dock = DockStyle.Fill;
                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(onTargetAFRMapSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                    dockPanel.Text = "Symbol: " + tabdet.Map_name + " [" + Path.GetFileName(m_currentfile) + "]";
                    bool isDocked = false;
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
                            else if (m_appSettings.DefaultViewSize == ViewSize.ExtraSmallView)
                            {
                                if (tabdet.X_axisvalues.Length > 0)
                                {
                                    width = 30 + ((tabdet.X_axisvalues.Length + 1) * 30);
                                }
                                if (width < 400) width = 400;

                            }
                        }
                        dockPanel.Width = width;
                    }

                    dockPanel.Controls.Add(tabdet);
                    tabdet.Visible = true;
                    //tabdet.MaxValueInTable = 350;
                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();

            }
            System.Windows.Forms.Application.DoEvents();

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

        void onTargetAFRMapSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            if (sender is IMapViewer)
            {
                IMapViewer tabdet = (IMapViewer)sender;
                // get data from mapviewer

                /*
if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                int cols = 18;
                int rows = 16;
                string foldername = Path.Combine(Path.GetDirectoryName(m_currentfile), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                m_AFRMap.SaveMap(m_currentfile, cols, rows);
                ShowAfrMAP("TargetAFR", Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-targetafr.afr"));
            }                 * */
                if (m_AFRMap != null && m_currentfile != string.Empty)
                {
                    int cols = 18;
                    int rows = 16;
                    string foldername = Path.Combine(Path.GetDirectoryName(m_currentfile), "AFRMaps");
                    if (!Directory.Exists(foldername))
                    {
                        Directory.CreateDirectory(foldername);
                    }
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);

                    if (e.SymbolName == "TargetAFR")
                    {
                        // set the new map data
                        m_AFRMap.SaveTargetAFRMap(Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-targetafr.afr"), ConvertMapToFloat(e.SymbolDate), cols, rows);
                        m_AFRMap.InitializeMaps(cols * rows, m_currentfile);
                        // reload ... into m_AFRMap structure
                        tabdet.Map_content = m_AFRMap.GetTargetAFRMapinBytes(cols * rows, Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-targetafr.afr"));
                    }
                }
                /*
                if (tabdet.Map_name == "FeedbackAFR" || tabdet.Map_name == "FeedbackvsTargetAFR")
                {
                    // only done on clear?
                    if (tabdet.ClearData)
                    {
                        tabdet.ClearData = false;
                        // now clear the entire map
                        string filename = Path.Combine(Path.GetDirectoryName(m_currentfile), Path.GetFileNameWithoutExtension(m_currentfile) + "-AFRFeedbackmap.afr");
                        string filenamecount = Path.Combine(Path.GetDirectoryName(m_currentfile), Path.GetFileNameWithoutExtension(m_currentfile) + "-AFRFeedbackCountermap.afr");
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                        if (File.Exists(filenamecount))
                        {
                            File.Delete(filenamecount);
                        }
                    }
                    // reload data
                    int columns = 0;
                    int rows = 0;
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, "Insp_mat!", out columns, out rows);
                    AFRMapInMemory = new float[rows * columns];
                    AFRMapCounterInMemory = new int[rows * columns];
                    SaveAFRAndCounterMaps();
                    UpdateFeedbackMaps();
                }*/
            }
        }

        private void btnAFRFeedbackMap_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show feedback afr map if available

            if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                int cols = 18;
                int rows = 16;
                string foldername = Path.Combine(Path.GetDirectoryName(m_currentfile), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                m_AFRMap.SaveMap(m_currentfile, cols, rows);
                ShowAfrMAP("FeedbackAFR", Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-feedbackafrtab.afr"));
            }

        }

        private void ClearAFRFeedbackMap()
        {
            if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                int columns = 8;
                int rows = 8;
                int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out columns, out rows);
                m_AFRMap.ClearMaps(columns, rows, m_currentfile);
                UpdateFeedbackMaps();
            }

        }

        private void btnClearAFRFeedback_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!m_appSettings.AutoCreateAFRMaps) return;
            ClearAFRFeedbackMap();
        }

        private void digitalDisplayControl6_Click(object sender, EventArgs e)
        {
            SwitchAFRMode();

        }

        private void labelControl11_Click(object sender, EventArgs e)
        {
            SwitchAFRMode();
        }

        private void exportFixedTuningPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SymbolCollection scToExport = new SymbolCollection();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Trionic 7 packages|*.t7p";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                // add all relevant symbols to the export collection
                //SymbolHelper sh = new SymbolHelper();
                // must contain Varname, FlashAddress, Length, Userdescription
                AddToSymbolCollection(scToExport, "LimEngCal.TurboSpeedTab"); // add axis
                AddToSymbolCollection(scToExport, "LimEngCal.p_AirSP");
                AddToSymbolCollection(scToExport, "AirCtrlCal.m_MaxAirTab"); // add axis
                AddToSymbolCollection(scToExport, "TempLimPosCal.Airmass");
                //AddToSymbolCollection(scToExport, "X_AccPedalAutSP");
                AddToSymbolCollection(scToExport, "BoostCal.RegMap");   // add axis
                AddToSymbolCollection(scToExport, "BoostCal.SetLoadXSP");
                AddToSymbolCollection(scToExport, "BoostCal.n_EngSP");
                AddToSymbolCollection(scToExport, "PedalMapCal.m_RequestMap");// add axis
                AddToSymbolCollection(scToExport, "PedalMapCal.n_EngineMap");
                AddToSymbolCollection(scToExport, "PedalMapCal.X_PedalMap");
                AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmass");  // add axis
                AddToSymbolCollection(scToExport, "BstKnkCal.OffsetXSP");
                AddToSymbolCollection(scToExport, "BstKnkCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmassAu"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxAutTab"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxTab"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxE85Tab"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_ManGearLim"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_CabGearLim"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.n_Eng5GearSP");
                AddToSymbolCollection(scToExport, "TorqueCal.M_5GearLimTab"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_NominalMap"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.m_AirXSP");
                AddToSymbolCollection(scToExport, "TorqueCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "TorqueCal.m_AirTorqMap"); // add axis
                AddToSymbolCollection(scToExport, "TorqueCal.M_EngXSP");
                AddToSymbolCollection(scToExport, "TorqueCal.m_PedYSP");
                AddToSymbolCollection(scToExport, "FCutCal.m_AirInletLimit");
                AddToSymbolCollection(scToExport, "BoosDiagCal.m_FaultDiff");
                AddToSymbolCollection(scToExport, "BoosDiagCal.ErrMaxMReq");
                AddToSymbolCollection(scToExport, "BFuelCal.Map");
                AddToSymbolCollection(scToExport, "BFuelCal.StartMap");
                AddToSymbolCollection(scToExport, "BFuelCal.E85Map");
                AddToSymbolCollection(scToExport, "BFuelCal.AirXSP");
                AddToSymbolCollection(scToExport, "BFuelCal.RpmYSP");
                AddToSymbolCollection(scToExport, "InjCorrCal.BattCorrSP");
                AddToSymbolCollection(scToExport, "InjCorrCal.BattCorrTab");
                AddToSymbolCollection(scToExport, "InjCorrCal.InjectorConst");
                AddToSymbolCollection(scToExport, "IgnNormCal.Map");
                AddToSymbolCollection(scToExport, "IgnE85Cal.fi_AbsMap");
                AddToSymbolCollection(scToExport, "IgnNormCal.m_AirXSP");
                AddToSymbolCollection(scToExport, "IgnNormCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "IgnKnkCal.IndexMap");
                AddToSymbolCollection(scToExport, "KnkFuelCal.fi_MapMaxOff");
                AddToSymbolCollection(scToExport, "KnkFuelCal.m_AirXSP");
                AddToSymbolCollection(scToExport, "BoostCal.PMap");
                AddToSymbolCollection(scToExport, "BoostCal.IMap");
                AddToSymbolCollection(scToExport, "BoostCal.DMap");
                AddToSymbolCollection(scToExport, "BoostCal.PIDXSP");
                AddToSymbolCollection(scToExport, "BoostCal.PIDYSP");
                AddToSymbolCollection(scToExport, "TorqueCal.M_OverBoostTab");
                AddToSymbolCollection(scToExport, "TorqueCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "KnkFuelCal.EnrichmentMap");
                AddToSymbolCollection(scToExport, "IgnKnkCal.m_AirXSP");
                AddToSymbolCollection(scToExport, "IgnKnkCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "KnkDetCal.RefFactorMap");
                AddToSymbolCollection(scToExport, "KnkDetCal.m_AirXSP");
                AddToSymbolCollection(scToExport, "KnkDetCal.n_EngYSP");
                AddToSymbolCollection(scToExport, "MaxSpdCal.T_EngineSP");
                AddToSymbolCollection(scToExport, "MaxSpdCal.n_EngLimAir");
                AddToSymbolCollection(scToExport, "MaxVehicCal.v_MaxSpeed");
                PackageExporter pe = new PackageExporter();
                if (IsSoftwareOpen())
                {
                    pe.AddressOffset = GetOpenFileOffset();
                }
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

        private void btnSaveRealtimeLayout_Click(object sender, EventArgs e)
        {
            // save the user defined symbols in the list
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Realtime layout files|*.t7rtl";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveRealtimeTable(sfd.FileName);
            }

        }

        private void btnLoadRealtimeLayout_Click(object sender, EventArgs e)
        {
            // load user defined symbols
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Realtime layout files|*.t7rtl";
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

        private void btnVectors_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmVectorlist vectorlist = new frmVectorlist())
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("Vector");
                dt.Columns.Add("Address", System.Type.GetType("System.Int64"));

                long[] adresses = Trionic7File.GetVectorAddresses(m_currentfile);
                string[] names = Trionic7File.GetVectorNames();
                for (int i = 0; i < 256; i++)
                {
                    dt.Rows.Add(names[i].Replace("_", " "), Convert.ToInt64(adresses.GetValue(i)));
                }
                vectorlist.SetDataTable(dt);
                vectorlist.ShowDialog();
            }
        }

        void edit_onMapSelected(object sender, frmEditTuningPackage.MapSelectedEventArgs e)
        {
            // user double clicked on a symbol in the edit tuning packages window...
            // start two mapviewers
            // ONE SHOULD BE ABLE TO EDIT THE DATA AND ALTER THE CONTENT OF THE TUNING PACKAGE
            StartTableViewer(e.Mapname);
            byte[] data = ConvertTuningPackageDataToByteArray(e.Data);
            StartTableViewerFromTuningPackage(e.Mapname, data, e.Filename);
        }

        private byte[] ConvertTuningPackageDataToByteArray(string tpdata)
        {
            if (tpdata.EndsWith(",")) tpdata = tpdata.Substring(0, tpdata.Length - 1);
            char[] sep = new char[1];
            sep.SetValue(',', 0);
            string[] hexdata = tpdata.Split(sep);
            byte[] retval = new byte[hexdata.Length];
            int i = 0;
            foreach (string hs in hexdata)
            {
                retval.SetValue(Convert.ToByte(hs, 16), i++);
            }
            return retval;
        }

        private void StartTableViewerFromTuningPackage(string symbolname, byte[] data, string filename)
        {
            if (!File.Exists(filename)) return;
            DockPanel dockPanel;
            bool pnlfound = false;
            foreach (DockPanel pnl in dockManager1.Panels)
            {
                if (pnl.Text == "Tuning package symbol: " + symbolname + " [" + Path.GetFileName(filename) + "]")
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
                    dockPanel = dockManager1.AddPanel(DockingStyle.Right);
                    dockPanel.Tag = filename;
                    IMapViewer tabdet = MapViewerFactory.Get(m_appSettings);
                    tabdet.Filename = filename;
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
                    tabdet.X_axis_name = x_axis_descr;
                    tabdet.Y_axis_name = y_axis_descr;
                    tabdet.Z_axis_name = z_axis_descr;


                    int columns = 8;
                    int rows = 8;
                    int tablewidth = GetTableMatrixWitdhByName(m_currentfile, m_symbols, tabdet.Map_name, out columns, out rows);
                    int address = (int)GetSymbolAddress(m_symbols, symbolname);
                    //int sramaddress = sramaddress;

                    // while (address > m_currentfile_size) address -= m_currentfile_size;
                    tabdet.Map_address = address;
                    tabdet.Map_sramaddress = (int)GetSymbolAddressSRAM(m_symbols, symbolname);
                    tabdet.Map_length = data.Length;
                    byte[] mapdata = data;
                    tabdet.Map_content = mapdata;
                    tabdet.Correction_factor = GetMapCorrectionFactor(tabdet.Map_name);
                    tabdet.Correction_offset = GetMapCorrectionOffset(tabdet.Map_name);
                    tabdet.IsUpsideDown = GetMapUpsideDown(tabdet.Map_name);
                    tabdet.ShowTable(columns, isSixteenBitTable(tabdet.Map_name));
                    TryToAddOpenLoopTables(tabdet);

                    tabdet.IsRAMViewer = true;
                    tabdet.OnlineMode = true;
                    tabdet.Dock = DockStyle.Fill;
                    //tabdet.onSymbolSave += new MapViewer.NotifySaveSymbol(tabdet_onSymbolSave);
                    tabdet.onSymbolSave += new IMapViewer.NotifySaveSymbol(tuningpackage_onSymbolSave);
                    tabdet.onClose += new IMapViewer.ViewerClose(tabdet_onClose);
                    //tabdet.onReadFromSRAM += new IMapViewer.ReadDataFromSRAM(tabdet_onReadFromSRAM);
                    //tabdet.onWriteToSRAM += new IMapViewer.WriteDataToSRAM(tabdet_onWriteToSRAM);
                    tabdet.onSelectionChanged += new IMapViewer.SelectionChanged(tabdet_onSelectionChanged);
                    tabdet.onSurfaceGraphViewChangedEx += new IMapViewer.SurfaceGraphViewChangedEx(mv_onSurfaceGraphViewChangedEx);
                    tabdet.onSurfaceGraphViewChanged += new IMapViewer.SurfaceGraphViewChanged(mv_onSurfaceGraphViewChanged);
                    //dockPanel.DockAsTab(dockPanel1);
                    dockPanel.Text = "Tuning package symbol: " + tabdet.Map_name + " [" + Path.GetFileName(filename) + "]";
                    bool isDocked = false;
                    if (m_appSettings.AutoDockSameSymbol)
                    {
                        foreach (DockPanel pnl in dockManager1.Panels)
                        {
                            if (pnl.Text.Contains(tabdet.Map_name) && pnl != dockPanel && (pnl.Visibility == DockVisibility.Visible))
                            {
                                dockPanel.DockAsTab(pnl, 0);
                                isDocked = true;
                                break;
                            }
                        }
                    }
                    if (!isDocked)
                    {
                        dockPanel.DockTo(DockingStyle.Right, 0);
                        if (m_appSettings.AutoSizeNewWindows)
                        {
                            if (tabdet.X_axisvalues.Length > 0)
                            {
                                dockPanel.Width = 30 + ((tabdet.X_axisvalues.Length + 1) * 35);
                            }
                        }
                        if (dockPanel.Width < 400) dockPanel.Width = 400;

                    }
                    dockPanel.Controls.Add(tabdet);

                }
                catch (Exception newdockE)
                {
                    logger.Debug(newdockE.Message);
                }
                dockManager1.EndUpdate();
            }
        }

        void tuningpackage_onSymbolSave(object sender, IMapViewer.SaveSymbolEventArgs e)
        {
            // mapviewer requested to save data into a tuning package
            // how on earth are we going to refresh the data in the tuning package edit window?
            if (tunpackeditWindow != null)
            {
                // refresh the data in the window to reflect the changes made in the mapviewer
                if (e.Filename == tunpackeditWindow.TuningPackageFilename)
                {
                    string symbolData = string.Empty;
                    foreach (byte b in e.SymbolDate)
                    {
                        symbolData += b.ToString("X2") + ",";
                    }
                    tunpackeditWindow.SetDataForSymbol(e.SymbolName, symbolData);
                }
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
                    logger.Debug("We should write the tuning package here!");
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Trionic 7 packages|*.t7p";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.Data.DataTable dt = edit.GetDataTable();
                        SymbolCollection scToExport = new SymbolCollection();
                        foreach (DataRow dr in dt.Rows)
                        {
                            SymbolHelper sh = new SymbolHelper();
                            sh.Varname = dr["Map"].ToString();
                            sh.Currentdata = ConvertTuningPackageDataToByteArray(dr["Data"].ToString());
                            sh.Flash_start_address = GetSymbolAddress(m_symbols, sh.Varname);
                            sh.Userdescription = GetUserDescription(m_symbols, sh.Varname);
                            sh.Length = GetSymbolLength(m_symbols, sh.Varname);
                            scToExport.Add(sh);

                        }
                        PackageExporter pe = new PackageExporter();
                        if (IsSoftwareOpen())
                        {
                            pe.AddressOffset = GetOpenFileOffset();
                        }
                        pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
                    }
                }
            }
            tunpackeditWindow = null;
        }

        private void gridViewSymbols_DragObjectStart(object sender, DevExpress.XtraGrid.Views.Base.DragObjectStartEventArgs e)
        {
            logger.Debug("Start dragging: " + e.DragObject.ToString());
            _isMouseDown = true;
            //e.DragObject
        }

        private bool _isMouseDown = false;

        private void gridControlSymbols_MouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            // get selected symbolname
            //if (e.Button == MouseButtons.Left)
            //{

            //}
        }

        private void gridControlSymbols_MouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        private void btnWriteLogMarker_ItemClick(object sender, ItemClickEventArgs e)
        {
            //
            m_WriteLogMarker = true;
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
                            logger.Debug("Symbol dragging: " + sh.Varname);
                            sh.Currentdata = readdatafromfile(m_currentfile, (int)sh.Flash_start_address, sh.Length);
                            if (IsSoftwareOpen())
                            {
                                sh.Currentdata = readdatafromfile(m_currentfile, (int)sh.Flash_start_address - GetOpenFileOffset(), sh.Length);
                            }
                            gridControlSymbols.DoDragDrop(sh, DragDropEffects.All);

                        }

                    }

                }
            }
        }

        private void sndTimer_Tick(object sender, EventArgs e)
        {
            sndTimer.Enabled = false;
            _soundAllowed = true; // re-allow the playback of sounds
        }

        private void btnViewMatrixFromLog_ItemClick(object sender, ItemClickEventArgs e)
        {
            // let the user select x axis, y axis and z axis symbols from the logfile
            //
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 logfiles|*.t7l";
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
                        //logger.Debug("Adding: " + xvalue.ToString());
                        try
                        {
                            dtresult.Columns.Add(xvalue.ToString(), Type.GetType("System.Double"));
                        }
                        catch (Exception E)
                        {
                            logger.Debug("Failed to add column: " + E.Message);
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
                            logger.Debug("Failed to add empty row: " + E.Message);
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
                    result.UseNewMapViewer = m_appSettings.UseNewMapViewer;
                    result.SetTable(dtresult);
                    string typedescr = " (Mean values)";
                    if (type == 1) typedescr = " (Minimum values)";
                    else if (type == 2) typedescr = " (Maximum values)";
                    result.Text = "Matrix [" + x + " : " + y + " : " + z + "]" + typedescr;
                    result.Show();
                }
            }
        }

        System.Data.DataTable avgTable;

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


            //logger.Debug("x = " + x.ToString() + " y = " + y.ToString() + " xindex = " + xindex.ToString() + " yindex = " + yindex.ToString());
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

        private void gridControlSymbols_MouseLeave(object sender, EventArgs e)
        {
            _isMouseDown = false;
        }

        private void btnFeedbackCounter_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show feedback afr map if available

            if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                string foldername = Path.Combine(Path.GetDirectoryName(m_currentfile), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                int cols = 18;
                int rows = 16;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BFuelCal.Map", out cols, out rows);
                m_AFRMap.SaveMap(m_currentfile, cols, rows);
                ShowAfrMAP("FeedbackCounter", Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-AFRFeedbackCountermap.afr"));
            }
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

            foreach (Control c in panelBottom.Controls)
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
            SetButtonStyle(btnEconomyMode, panelMode, backColor, labelColor);
            SetButtonStyle(btnNormalMode, panelMode, backColor, labelColor);
            SetButtonStyle(btnSportMode, panelMode, backColor, labelColor);
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
            //logger.Debug("Switching tab control!");
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
                //logger.Debug("Switching tab page: " + page.Name);
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
                ViewRealtime.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
                ViewRealtime.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
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
                //ViewRealtime.OptionsView.ShowHorzLines = true;
                ViewRealtime.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
                //ViewRealtime.OptionsView.ShowVertLines = true;
                ViewRealtime.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
                ViewRealtime.OptionsBehavior.Editable = false;
            }

        }

        private void SetColorForMeasurement(ctrlMeasurement measurement, Color backColor, Color foreColor, Color labelColor)
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

        private void TryToLoadAdditionalCSVSymbols(string filename)
        {
            // convert to CSV file format
            // 56;AreaCal.A_MaxAdap;;;
            try
            {
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
                                sh.Description = SymbolTranslator.ToHelpText(sh.Userdescription, m_appSettings.ApplicationLanguage);
                                sh.createAndUpdateCategory(sh.Userdescription);
                            }
                        }
                    }
                    catch (Exception lineE)
                    {
                        logger.Debug("Failed to import a symbol from CSV file " + line + ": " + lineE.Message);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug("Failed to import additional CSV symbols: " + E.Message);
            }
        }

        private void barSyncToBinary_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_currentfile != "")
            {
                if (MessageBox.Show("This will overwrite data in your binary file. Are you sure you want to proceed?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (RealtimeCheckAndConnect())
                    {
                        frmProgress progress = new frmProgress();
                        progress.Show();
                        progress.SetProgress("Start synchronization from ECU to binary");
                        System.Windows.Forms.Application.DoEvents();
                        int percentage = 0;
                        int symCnt = 0;
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            // read from ECU, write to binary

                            if (sh.Start_address > 0x80000)
                            {
                                //if (sh.Varname.EndsWith("Map")) //TODO: <GS-28012011> REMOVE AFTER DEBUGGING
                                {
                                    string symbolname = sh.Varname;
                                    if (IsSymbolCalibration(symbolname))
                                    {
                                        if (symbolname.StartsWith("Symbolnumber") && sh.Userdescription != "") symbolname = sh.Userdescription;
                                        percentage = symCnt * 100 / m_symbols.Count;
                                        if (symCnt % 5 == 0)
                                        {
                                            progress.SetProgress("Sync from ECU: " + symbolname);
                                            progress.SetProgressPercentage(percentage);
                                        }
                                        byte[] mapdata = ReadMapFromSRAM(sh, false);
                                        savedatatobinary((int)GetSymbolAddress(m_symbols, symbolname), sh.Length, mapdata, m_currentfile, false);
                                        Thread.Sleep(1);
                                    }
                                }
                            }
                            symCnt++;
                        }
                        if (progress != null) progress.Close();
                        UpdateChecksum(m_currentfile);
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to get data from the ECU");
                    }
                }
            }
        }

        private void barSyncToECU_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_currentfile != "")
            {
                if (MessageBox.Show("This will overwrite data in your ECU. Are you sure you want to proceed?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (RealtimeCheckAndConnect())
                    {
                        frmProgress progress = new frmProgress();
                        progress.Show();
                        progress.SetProgress("Start synchronization from binary to ECU");
                        System.Windows.Forms.Application.DoEvents();
                        int percentage = 0;
                        int symCnt = 0;
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            // read from ECU, write to binary
                            if (sh.Start_address > 0x80000)
                            {
                                //if (sh.Varname.EndsWith("Map")) //TODO: <GS-28012011> REMOVE AFTER DEBUGGING
                                {
                                    string symbolname = sh.Varname;
                                    if (IsSymbolCalibration(symbolname))
                                    {
                                        if (symbolname.StartsWith("Symbolnumber") && sh.Userdescription != "") symbolname = sh.Userdescription;
                                        percentage = symCnt * 100 / m_symbols.Count;
                                        if (symCnt % 5 == 0)
                                        {
                                            progress.SetProgress("Sync to ECU: " + symbolname);
                                            progress.SetProgressPercentage(percentage);
                                        }
                                        byte[] mapdata = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, symbolname), sh.Length);
                                        // store in the ECU
                                        WriteMapToSRAM(symbolname, mapdata, false);
                                        Thread.Sleep(1);
                                    }
                                }

                            }
                            symCnt++;

                        }
                        if (progress != null) progress.Close();
                    }
                    else
                    {
                        frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to get data from the ECU");
                    }
                }
            }
        }

        private void btnUploadTuningPackage_ItemClick(object sender, ItemClickEventArgs e)
        {
            // upload a t7p file to the ECU
            if (m_currentfile != "")
            {
                if (RealtimeCheckAndConnect())
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Trionic 7 packages|*.t7p";
                    ofd.Multiselect = false;
                    char[] sep = new char[1];
                    sep.SetValue(',', 0);
                    SymbolCollection scToImport = new SymbolCollection();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        //TODO: create a list of maps to import .. maybe?
                        frmProgress progress = new frmProgress();
                        progress.SetProgress("Uploading tuning package...");
                        progress.Show();
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
                                            if (sh_Import.Varname != "MapChkCal.ST_Enable")
                                            {
                                                progress.SetProgress("Uploading: " + sh_Import.Varname);
                                                WriteMapToSRAM(sh_Import.Varname, dataToInsert, false);
                                                System.Windows.Forms.Application.DoEvents();
                                                Thread.Sleep(1);
                                            }
                                        }
                                    }
                                    catch (Exception E)
                                    {
                                        // add failure
                                        logger.Debug(E.Message);
                                    }
                                }
                            }
                        }
                        if (progress != null) progress.Close();
                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to upload a tuning package");
                }
            }
        }

        private void btnGenerateTuningPackage_ItemClick(object sender, ItemClickEventArgs e)
        {
            // generate the default maps as a tuning package from the ECUs SRAM
            if (m_currentfile != "")
            {
                if (RealtimeCheckAndConnect())
                {
                    frmProgress progress = new frmProgress();
                    progress.SetProgress("Downloading tuning package...");
                    progress.Show();
                    SymbolCollection scToExport = new SymbolCollection();
                    PackageExporter pe = new PackageExporter();
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Trionic 7 packages|*.t7p";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        AddToSymbolCollection(scToExport, "LimEngCal.TurboSpeedTab"); // add axis
                        AddToSymbolCollection(scToExport, "LimEngCal.p_AirSP");
                        AddToSymbolCollection(scToExport, "AirCtrlCal.m_MaxAirTab"); // add axis
                        AddToSymbolCollection(scToExport, "TempLimPosCal.Airmass");
                        //AddToSymbolCollection(scToExport, "X_AccPedalAutSP");
                        AddToSymbolCollection(scToExport, "BoostCal.RegMap");   // add axis
                        AddToSymbolCollection(scToExport, "BoostCal.SetLoadXSP");
                        AddToSymbolCollection(scToExport, "BoostCal.n_EngSP");
                        AddToSymbolCollection(scToExport, "PedalMapCal.m_RequestMap");// add axis
                        AddToSymbolCollection(scToExport, "PedalMapCal.n_EngineMap");
                        AddToSymbolCollection(scToExport, "PedalMapCal.X_PedalMap");
                        AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmass");  // add axis
                        AddToSymbolCollection(scToExport, "BstKnkCal.OffsetXSP");
                        AddToSymbolCollection(scToExport, "BstKnkCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "BstKnkCal.MaxAirmassAu"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxAutTab"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxTab"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_EngMaxE85Tab"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_ManGearLim"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_CabGearLim"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.n_Eng5GearSP");
                        AddToSymbolCollection(scToExport, "TorqueCal.M_5GearLimTab"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_NominalMap"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.m_AirXSP");
                        AddToSymbolCollection(scToExport, "TorqueCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "TorqueCal.m_AirTorqMap"); // add axis
                        AddToSymbolCollection(scToExport, "TorqueCal.M_EngXSP");
                        AddToSymbolCollection(scToExport, "TorqueCal.m_PedYSP");
                        AddToSymbolCollection(scToExport, "FCutCal.m_AirInletLimit");
                        AddToSymbolCollection(scToExport, "BoosDiagCal.m_FaultDiff");
                        AddToSymbolCollection(scToExport, "BoosDiagCal.ErrMaxMReq");
                        AddToSymbolCollection(scToExport, "BFuelCal.Map");
                        AddToSymbolCollection(scToExport, "BFuelCal.StartMap");
                        AddToSymbolCollection(scToExport, "BFuelCal.E85Map");
                        AddToSymbolCollection(scToExport, "BFuelCal.AirXSP");
                        AddToSymbolCollection(scToExport, "BFuelCal.RpmYSP");
                        AddToSymbolCollection(scToExport, "InjCorrCal.InjectorConst");
                        AddToSymbolCollection(scToExport, "IgnNormCal.Map");
                        AddToSymbolCollection(scToExport, "IgnE85Cal.fi_AbsMap");
                        AddToSymbolCollection(scToExport, "IgnNormCal.m_AirXSP");
                        AddToSymbolCollection(scToExport, "IgnNormCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "IgnKnkCal.IndexMap");
                        AddToSymbolCollection(scToExport, "KnkFuelCal.fi_MapMaxOff");
                        AddToSymbolCollection(scToExport, "KnkFuelCal.m_AirXSP");
                        AddToSymbolCollection(scToExport, "BoostCal.PMap");
                        AddToSymbolCollection(scToExport, "BoostCal.IMap");
                        AddToSymbolCollection(scToExport, "BoostCal.DMap");
                        AddToSymbolCollection(scToExport, "BoostCal.PIDXSP");
                        AddToSymbolCollection(scToExport, "BoostCal.PIDYSP");
                        AddToSymbolCollection(scToExport, "TorqueCal.M_OverBoostTab");
                        AddToSymbolCollection(scToExport, "TorqueCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "KnkFuelCal.EnrichmentMap");
                        AddToSymbolCollection(scToExport, "IgnKnkCal.m_AirXSP");
                        AddToSymbolCollection(scToExport, "IgnKnkCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "KnkDetCal.RefFactorMap");
                        AddToSymbolCollection(scToExport, "KnkDetCal.m_AirXSP");
                        AddToSymbolCollection(scToExport, "KnkDetCal.n_EngYSP");
                        AddToSymbolCollection(scToExport, "MaxSpdCal.T_EngineSP");
                        AddToSymbolCollection(scToExport, "MaxSpdCal.n_EngLimAir");
                        AddToSymbolCollection(scToExport, "MaxVehicCal.v_MaxSpeed");
                        //pe.ExportPackage(scToExport, m_currentfile, sfd.FileName);
                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        foreach (SymbolHelper sh in scToExport)
                        {
                            //<GS-28012011>

                            progress.SetProgress("Downloading: " + sh.Varname);
                            System.Windows.Forms.Application.DoEvents();
                            byte[] data = ReadMapFromSRAM(sh, false);
                            pe.ExportMap(sfd.FileName, sh.Varname, sh.Userdescription, sh.Length, data);
                            Thread.Sleep(1);
                        }
                    }
                    if (progress != null) progress.Close();
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("An active CAN bus connection is needed to download a tuning package");
                }
            }
        }

        private void SetupLogFilters()
        {
            // setup the export filters
            LogFilters filterhelper = new LogFilters(suiteRegistry);
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

        private void btnLogFilters_ItemClick(object sender, ItemClickEventArgs e)
        {
            SetupLogFilters();
        }

        private void btnPanelFullscreen_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (dockManager1.ActivePanel != null)
                {
                    /*if (dockManager1.ActivePanel == dockRealtime)
                    {
                        // misschien wel terugzetten?
                        if (dockManager1.ActivePanel.FloatForm != null)
                        {
                            logger.Debug("Restoring: " + dockManager1.ActivePanel.Text);
                            if (dockManager1.ActivePanel.FloatForm.Width < this.Width - 20)
                            {
                                // maximaliseren
                                dockManager1.ActivePanel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                                dockManager1.ActivePanel.FloatLocation = new System.Drawing.Point(1, 1);
                                dockManager1.ActivePanel.MakeFloat();
                            }
                            else
                            {
                                //<GS-09112010>
                                dockRealtime.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                                int width = dockManager1.Form.ClientSize.Width - dockSymbols.Width;
                                int height = dockManager1.Form.ClientSize.Height;
                                if (width > 660 && Height > 580)
                                {
                                    dockRealtime.MakeFloat();
                                    // maximize
                                    
                                    dockRealtime.FloatSize = new Size(660, 580);
                                    //dockRealtime.FloatLocation = 
                                }
                                else
                                {
                                    dockRealtime.Dock = DockingStyle.Left;
                                    dockRealtime.Width = width;
                                }
                                dockManager1.ActivePanel.Restore();
                            }
                        }
                        else
                        {
                            logger.Debug("Maximizing: " + dockManager1.ActivePanel.Text);
                            dockManager1.ActivePanel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                            dockManager1.ActivePanel.FloatLocation = new System.Drawing.Point(1, 1);
                            dockManager1.ActivePanel.MakeFloat();
                        }
                    }
                    else*/
                    {
                        // misschien wel terugzetten?
                        if (dockManager1.ActivePanel.FloatForm != null)
                        {
                            logger.Debug("Restoring: " + dockManager1.ActivePanel.Text);
                            dockManager1.ActivePanel.Restore();
                        }
                        else
                        {
                            logger.Debug("Maximizing: " + dockManager1.ActivePanel.Text);
                            dockManager1.ActivePanel.FloatSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                            dockManager1.ActivePanel.FloatLocation = new System.Drawing.Point(1, 1);
                            dockManager1.ActivePanel.MakeFloat();
                        }
                    }

                }
            }
            catch (Exception E)
            {
                logger.Debug("btnPanelFullscreen_ItemClick: " + E.Message);
            }
        }

        private void btnCaptureScreenshot_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPG images|*.jpg";
            bool _written = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (dockManager1.ActivePanel != null)
                {
                    if (dockManager1.ActivePanel != dockSymbols)
                    {
                        dockManager1.BeginUpdate();
                        Bitmap bmp = new Bitmap(dockManager1.ActivePanel.Width, dockManager1.ActivePanel.Height);
                        Graphics g = dockManager1.ActivePanel.CreateGraphics();
                        dockManager1.ActivePanel.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, dockManager1.ActivePanel.Width, dockManager1.ActivePanel.Height));

                        // if there is a gridcontrol in the form, we need to copy the gridcontrol directly
                        // and paste it over the bitmap programatically, this is a bug/non-feature in DevExpress components
                        // see: http://www.devexpress.com/Support/Center/p/Q102575.aspx for more information
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 10, 10);
                        GridControl gctrl = GetGridControlFromPanel(dockManager1.ActivePanel, out rect);
                        if (gctrl != null)
                        {
                            // copy & paste directly from the gridcontrol component
                            gctrl.DrawToBitmap(bmp, rect);
                        }

                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        dockManager1.EndUpdate();
                        _written = true;
                    }
                }
                if (!_written)
                {
                    Bitmap bmp = new Bitmap(this.Width, this.Height);
                    this.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, this.Width, this.Height));

                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 10, 10);
                    GridControl gctrl = GetGridControlFromPanel(dockSymbols, out rect);
                    if (gctrl != null)
                    {
                        System.Drawing.Rectangle rect2 = new System.Drawing.Rectangle(rect.X + dockSymbols.Left, rect.Y + dockSymbols.Top, rect.Width, rect.Height);
                        // copy & paste directly from the gridcontrol component
                        gctrl.DrawToBitmap(bmp, rect2);
                    }

                    if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                    bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private GridControl GetGridControlFromPanel(DockPanel dockPanel, out System.Drawing.Rectangle rect)
        {
            GridControl retval = null;
            rect = new System.Drawing.Rectangle(0, 0, 10, 10);
            foreach (Control c in dockPanel.Controls)
            {
                if (c is ControlContainer)
                {
                    ControlContainer cc = (ControlContainer)c;
                    foreach (Control c2 in cc.Controls)
                    {
                        if (c2 is T7.MapViewerEx)
                        {
                            MapViewerEx mapex = (MapViewerEx)c2;
                            foreach (Control c3 in mapex.Controls)
                            {
                                if (c3 is DevExpress.XtraEditors.GroupControl)
                                {
                                    DevExpress.XtraEditors.GroupControl gc = (DevExpress.XtraEditors.GroupControl)c3;
                                    foreach (Control c4 in gc.Controls)
                                    {
                                        if (c4 is System.Windows.Forms.SplitContainer)
                                        {
                                            System.Windows.Forms.SplitContainer sc = (System.Windows.Forms.SplitContainer)c4;
                                            foreach (Control c5 in sc.Panel1.Controls)
                                            {
                                                if (c5 is System.Windows.Forms.Panel)
                                                {
                                                    System.Windows.Forms.Panel pnl = (System.Windows.Forms.Panel)c5;
                                                    foreach (Control c6 in pnl.Controls)
                                                    {
                                                        if (c6 is DevExpress.XtraGrid.GridControl)
                                                        {
                                                            retval = (DevExpress.XtraGrid.GridControl)c6;
                                                            rect = new System.Drawing.Rectangle(retval.Left + cc.Left + mapex.Left + gc.Left + sc.Left + pnl.Left, retval.Top + cc.Top + mapex.Top + gc.Top + sc.Top + pnl.Top, retval.Width, retval.Height);
                                                        }
                                                    }
                                                }

                                            }
                                            foreach (Control c5 in sc.Panel2.Controls)
                                            {
                                                if (c5 is System.Windows.Forms.Panel)
                                                {
                                                    System.Windows.Forms.Panel pnl = (System.Windows.Forms.Panel)c5;
                                                    foreach (Control c6 in pnl.Controls)
                                                    {
                                                        logger.Debug("MapViewerEx:GroupControl:SplitContainer.Panel2.Panel:" + c6.GetType().ToString());
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        else if (c2 is T7.MapViewer)
                        {
                            MapViewer map = (MapViewer)c2;
                            foreach (Control c3 in map.Controls)
                            {
                                DevExpress.XtraEditors.GroupControl gc = (DevExpress.XtraEditors.GroupControl)c3;
                                foreach (Control c4 in gc.Controls)
                                {
                                    if (c4 is System.Windows.Forms.SplitContainer)
                                    {
                                        System.Windows.Forms.SplitContainer sc = (System.Windows.Forms.SplitContainer)c4;
                                        foreach (Control c5 in sc.Panel1.Controls)
                                        {
                                            if (c5 is System.Windows.Forms.Panel)
                                            {
                                                System.Windows.Forms.Panel pnl = (System.Windows.Forms.Panel)c5;
                                                foreach (Control c6 in pnl.Controls)
                                                {
                                                    if (c6 is DevExpress.XtraGrid.GridControl)
                                                    {
                                                        retval = (DevExpress.XtraGrid.GridControl)c6;
                                                        rect = new System.Drawing.Rectangle(retval.Left + cc.Left + map.Left + gc.Left + sc.Left + pnl.Left, retval.Top + cc.Top + map.Top + gc.Top + sc.Top + pnl.Top, retval.Width, retval.Height);
                                                    }
                                                }
                                            }

                                        }
                                        foreach (Control c5 in sc.Panel2.Controls)
                                        {
                                            if (c5 is System.Windows.Forms.Panel)
                                            {
                                                System.Windows.Forms.Panel pnl = (System.Windows.Forms.Panel)c5;
                                                foreach (Control c6 in pnl.Controls)
                                                {
                                                    logger.Debug("MapViewer:GroupControl:SplitContainer.Panel2.Panel:" + c6.GetType().ToString());
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                        }
                        else if (c2 is T7.ctrlAirmassResult)
                        {
                            ctrlAirmassResult airmass = (ctrlAirmassResult)c2;
                            foreach (Control c3 in airmass.Controls)
                            {
                                if (c3 is DevExpress.XtraTab.XtraTabControl)
                                {
                                    DevExpress.XtraTab.XtraTabControl tabctrl = (DevExpress.XtraTab.XtraTabControl)c3;
                                    foreach (Control c4 in tabctrl.SelectedTabPage.Controls)
                                    {
                                        //DevExpress.XtraGrid.GridControl
                                        if (c4 is DevExpress.XtraGrid.GridControl)
                                        {
                                            retval = (DevExpress.XtraGrid.GridControl)c4;
                                            rect = new System.Drawing.Rectangle(retval.Left + cc.Left + airmass.Left + tabctrl.Left + tabctrl.SelectedTabPage.Left, retval.Top + cc.Top + airmass.Top + tabctrl.Top + tabctrl.SelectedTabPage.Top, retval.Width, retval.Height);

                                        }
                                        else
                                        {
                                            logger.Debug("ctrlAirmassResult:XtraTab:" + c4.GetType().ToString());
                                        }
                                    }
                                }

                            }
                        }
                        else if (c2 is DevExpress.XtraGrid.GridControl)
                        {
                            retval = (DevExpress.XtraGrid.GridControl)c2;
                            rect = new System.Drawing.Rectangle(retval.Left + cc.Left, retval.Top + cc.Top, retval.Width, retval.Height);

                        }
                        else
                        {
                            logger.Debug("Unsupported screenshot control: " + c2.GetType().ToString());
                        }
                    }

                }
            }
            return retval;
        }

        private void barButtonItem92_ItemClick(object sender, ItemClickEventArgs e)
        {
            // set E85.X_EthAct_Tech2 
            if (SymbolExists("E85.X_EthAct_Tech2"))
            {
                ShowRealtimeMapFromECU("E85.X_EthAct_Tech2");
            }
            else
            {
                frmInfoBox info = new frmInfoBox("No E85 adaption symbol in this binary file");
            }
        }


        byte _lambdacalSTEnableBackup = 0;

        private bool SetLambdaControl(bool enable)
        {
            // write to LambdaCal.ST_Enable
            // first make a backup of the value
            string symbolName = "LambdaCal.ST_Enable";
            int symbolnumber = GetSymbolNumber(m_symbols, symbolName);
            UInt32 sramaddress = (uint)GetSymbolAddressSRAM(m_symbols, symbolName);
            int length = GetSymbolLength(m_symbols, symbolName);
            bool _success = false;
            byte[] buffer = ReadSymbolFromSRAM((uint)symbolnumber, symbolName, sramaddress, length, out _success);
            if (_success)
            {
                if (!enable)
                {
                    _lambdacalSTEnableBackup = buffer[0];
                    buffer[0] = 0;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
                else // re-enable
                {
                    buffer[0] = _lambdacalSTEnableBackup;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
            }
            return _success;
        }

        byte _e85calSTEnableBackup = 0;

        private bool SetE85Cal(bool enable)
        {
            // write to E85Cal.ST_Enable
            // first make a backup of the value
            string symbolName = "E85Cal.ST_Enable";
            int symbolnumber = GetSymbolNumber(m_symbols, symbolName);
            UInt32 sramaddress = (uint)GetSymbolAddressSRAM(m_symbols, symbolName);
            int length = GetSymbolLength(m_symbols, symbolName);
            bool _success = false;
            byte[] buffer = ReadSymbolFromSRAM((uint)symbolnumber, symbolName, sramaddress, length, out _success);
            if (_success)
            {
                if (!enable)
                {
                    _e85calSTEnableBackup = buffer[0];
                    buffer[0] = 0;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
                else // re-enable
                {
                    buffer[0] = _e85calSTEnableBackup;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
            }
            return _success;
        }

        byte _FCutCalSTEnableBackup = 0;

        private bool SetFCutCal(bool enable)
        {
            // write to FCutCal.ST_Enable
            // first make a backup of the value
            string symbolName = "FCutCal.ST_Enable";
            int symbolnumber = GetSymbolNumber(m_symbols, symbolName);
            UInt32 sramaddress = (uint)GetSymbolAddressSRAM(m_symbols, symbolName);
            int length = GetSymbolLength(m_symbols, symbolName);
            bool _success = false;
            byte[] buffer = ReadSymbolFromSRAM((uint)symbolnumber, symbolName, sramaddress, length, out _success);
            if (_success)
            {
                if (!enable)
                {
                    _FCutCalSTEnableBackup = buffer[0];
                    buffer[0] = 0;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
                else // re-enable
                {
                    buffer[0] = _FCutCalSTEnableBackup;
                    WriteMapToSRAM(symbolName, buffer, false);
                }
            }
            return _success;
        }

        public int[] GetSymbolAsIntArray(string symbolname)
        {
            int[] retval = new int[1];
            byte[] bytes = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, symbolname), (int)GetSymbolLength(m_symbols, symbolname));
            if (isSixteenBitTable(symbolname))
            {
                retval = new int[bytes.Length / 2];
                int idx = 0;
                for (int t = 0; t < bytes.Length; t += 2)
                {
                    int v = Convert.ToInt32(bytes.GetValue(t)) * 256;
                    v += Convert.ToInt32(bytes.GetValue(t + 1));
                    retval.SetValue(v, idx++);
                }
            }
            else
            {
                retval = new int[bytes.Length];
                for (int t = 0; t < bytes.Length; t++)
                {
                    retval.SetValue(Convert.ToInt32(bytes.GetValue(t)), t);
                }
            }
            return retval;
        }

        private void btnAutoTune_Click(object sender, EventArgs e)
        {
            //<GS-31012011> disable it immediately to prevent the user from multi-clicking the button and hence getting the system confused

            if (_currentEngineStatus.CurrentEngineTemp < 70)
            {
                frmInfoBox info = new frmInfoBox("Engine temperature of 70 degrees C not reached...");
                return;
            }

            btnAutoTune.Enabled = false;
            btnAutoTune.Text = "Wait...";
            System.Windows.Forms.Application.DoEvents();

            if (_autoTuning)
            {
                // switch OFF autotune
                //1. write data if needed?

                //2. Clear data that has been measured

                if (m_appSettings.DisableClosedLoopOnStartAutotune)
                {
                    SetLambdaControl(true);
                    if (IsBinaryBiopower())
                    {
                        SetE85Cal(true);
                    }
                    SetFCutCal(true);
                }

                SetStatusText("Autotune stopped.");
                btnAutoTune.ForeColor = Color.Empty;
                btnAutoTune.Text = "AutoTune";
                _autoTuning = false;
                ToggleRealtimePanel();

                if (m_appSettings.AutoUpdateFuelMap)
                {
                    //TODO: ask the user whether he wants to merge the altered fuelmap into ECU memory!
                    // if he replies NO: revert to the previous fuel map (we still need to preserve a copy!)
                    if (MessageBox.Show("Keep adjusted fuel map?", "Question", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        // save the original map back to the ECU
                        WriteMapToSRAM(m_appSettings.AutoTuneFuelMap, m_AFRMap.GetOriginalFuelmap(), true);
                    }
                    else
                    {
                        // save the altered map into the binary
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            if (sh.Varname == m_appSettings.AutoTuneFuelMap || sh.Userdescription == m_appSettings.AutoTuneFuelMap)
                            {
                                //<GS-28012011>
                                if (IsSoftwareOpen())
                                {
                                    int symbolnumber = GetSymbolNumberFromRealtimeList(GetSymbolNumber(m_symbols, sh.Varname), sh.Varname);
                                    sh.Symbol_number = symbolnumber;
                                }

                                byte[] mapdata = ReadMapFromSRAM(sh, true);
                                savedatatobinary((int)GetSymbolAddress(m_symbols, sh.Varname), sh.Length, mapdata, m_currentfile, false);
                                UpdateChecksum(m_currentfile);
                            }
                        }
                    }
                    // init the afrmaps values
                    m_AFRMap.InitAutoTuneVars(true, 18, 16);
                }
                else
                {
                    //TODO: in that case, we've maintained the changes in the m_AFRMaps.FuelMapInformation struct
                    // we should now show the proposed changed (in percentages) to the user and let him/her
                    // decide which cells should be updated and which ones should be discarded
                    try
                    {
                        logger.Debug("Getting differences in percentages");
                        double[] diffinperc = m_AFRMap.GetPercentualDifferences();

                        System.Data.DataTable dt = new System.Data.DataTable();
                        for (int i = 0; i < 18; i++)
                        {
                            dt.Columns.Add(i.ToString(), Type.GetType("System.Double"));
                        }
                        for (int i = 15; i >= 0; i--)
                        {
                            object[] arr = new object[18];

                            for (int j = 0; j < 18; j++)
                            {
                                arr.SetValue(diffinperc[(i * 18) + j], j);
                            }
                            dt.Rows.Add(arr);
                        }
                        frmFuelMapAccept acceptMap = new frmFuelMapAccept();
                        acceptMap.Text = "Select percent mutations to accept for map " + m_appSettings.AutoTuneFuelMap;
                        acceptMap.onUpdateFuelMap += new frmFuelMapAccept.UpdateFuelMap(acceptMap_onUpdateFuelMap);
                        acceptMap.X_axisvalues = GetXaxisValues(m_currentfile, m_symbols, m_appSettings.AutoTuneFuelMap);
                        acceptMap.Y_axisvalues = GetYaxisValues(m_currentfile, m_symbols, m_appSettings.AutoTuneFuelMap);
                        acceptMap.AutoSizeColumns = m_appSettings.AutoSizeColumnsInWindows;
                        acceptMap.SetDataTable(dt);
                        acceptMap.ShowDialog();
                        System.Windows.Forms.Application.DoEvents();
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to stop autotune: " + E.Message);
                    }
                }
            }
            else
            {
                // switch ON autotune
                if (_softwareIsOpen)
                {
                    SetStatusText("Starting autotune...");
                    int _width = 18;
                    int _height = 16;
                    GetTableMatrixWitdhByName(m_currentfile, m_symbols, m_appSettings.AutoTuneFuelMap, out _width, out _height);
                    if (m_AFRMap == null)
                    {
                        m_AFRMap = new AFRMap();
                        m_AFRMap.RpmYSP = GetSymbolAsIntArray("BFuelCal.RpmYSP");
                        m_AFRMap.AirXSP = GetSymbolAsIntArray("BFuelCal.AirXSP");
                        m_AFRMap.InitializeMaps(_width * _height, m_currentfile);
                    }

                    // fill BFuelCal.Map axis as integer values
                    m_AFRMap.RpmYSP = GetSymbolAsIntArray("BFuelCal.RpmYSP");
                    m_AFRMap.AirXSP = GetSymbolAsIntArray("BFuelCal.AirXSP");
                    m_AFRMap.InitAutoTuneVars(false, _width, _height); // this also clears the afr feedback map
                    // disable closed loop operation
                    if (m_appSettings.DisableClosedLoopOnStartAutotune)
                    {
                        // LambdaCal.ST_Enable?
                        SetLambdaControl(false);
                        if (IsBinaryBiopower())
                        {
                            SetE85Cal(false);
                        }
                        SetFCutCal(false);
                    }
                    // what's next?
                    // TODO: read the current fuel map into memory
                    byte[] fuelmap = new byte[18 * 16];
                    // is there something like spot adaption in T7?
                    bool _initOk = false;
                    foreach (SymbolHelper sh in m_symbols)
                    {
                        if (sh.Varname == m_appSettings.AutoTuneFuelMap || sh.Userdescription == m_appSettings.AutoTuneFuelMap)
                        {
                            fuelmap = ReadMapFromSRAM(sh, true);
                            //TODO: Fill AFRMaps with this?
                            m_AFRMap.SetCurrentFuelMap(fuelmap);
                            m_AFRMap.SetOriginalFuelMap(fuelmap);
                            m_AFRMap.AutoUpdateFuelMap = m_appSettings.AutoUpdateFuelMap;
                            m_AFRMap.CorrectionPercentage = m_appSettings.CorrectionPercentage;
                            m_AFRMap.AcceptableTargetErrorPercentage = m_appSettings.AcceptableTargetErrorPercentage;
                            m_AFRMap.CellStableTime_ms = m_appSettings.CellStableTime_ms;
                            m_AFRMap.MaximumAdjustmentPerCyclePercentage = m_appSettings.MaximumAdjustmentPerCyclePercentage;

                            _initOk = true;
                        }
                    }
                    if (_initOk)
                    {
                        _autoTuning = true;
                        SetStatusText("Autotune running...");
                        btnAutoTune.ForeColor = Color.Red;
                        btnAutoTune.Text = "Tuning...";
                    }
                    else
                    {
                        SetStatusText("Autotune init failed.");
                        // revert to closed loop
                        if (m_appSettings.DisableClosedLoopOnStartAutotune)
                        {
                            SetLambdaControl(true);
                            if (IsBinaryBiopower())
                            {
                                SetE85Cal(true);
                            }
                            SetFCutCal(true);
                        }
                    }
                }
                else
                {
                    frmInfoBox info = new frmInfoBox("Autotune is only available for OPEN binaries");
                }
            }
            btnAutoTune.Enabled = true;
            System.Windows.Forms.Application.DoEvents();
        }

        void acceptMap_onUpdateFuelMap(object sender, frmFuelMapAccept.UpdateFuelMapEventArgs e)
        {
            // write to ECU if possible
            if (m_RealtimeConnectedToECU)
            {
                if (e.Value == 0) return; // test for value 0... nothing todo
                if (m_AFRMap == null)
                {
                    m_AFRMap = new AFRMap();
                    m_AFRMap.onFuelmapCellChanged += new AFRMap.FuelmapCellChanged(m_AFRMap_onFuelmapCellChanged);
                    m_AFRMap.onCellLocked += new AFRMap.CellLocked(m_AFRMap_onCellLocked);
                    m_AFRMap.InitializeMaps(18 * 16, m_currentfile);
                }

                int y = 15 - e.Y;
                // first get the original map
                //_ecuConnection.WriteSymbolDataForced(m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()) + e.Mapindex, 1, write);
                //byte[] fuelmap = _ecuConnection.ReadSymbolData(m_trionicFileInformation.GetInjectionMap(), (uint)m_trionicFileInformation.GetSymbolAddressSRAM(m_trionicFileInformation.GetInjectionMap()), (uint)m_trionicFileInformation.GetSymbolLength(m_trionicFileInformation.GetInjectionMap()));
                byte[] fuelmap = m_AFRMap.FuelMapInformation.GetOriginalFuelMap();
                double originalbyte = Convert.ToDouble(fuelmap[(y * 18) + e.X]);
                originalbyte *= (100 + e.Value) / 100;
                byte newFuelMapByte = Convert.ToByte(originalbyte);
                byte[] data2Write = new byte[1];
                data2Write[0] = newFuelMapByte;

                logger.Debug("Writing fuelmap");
                uint addresstowrite = (uint)GetSymbolAddressSRAM(m_symbols, m_appSettings.AutoTuneFuelMap) + (uint)(y * 18) + (uint)e.X; ;
                byte[] dataToSend = new byte[1];
                //dataToSend[0] = e.Cellvalue;
                if (!trionic7.WriteMapToSRAM(addresstowrite, data2Write))
                {
                    logger.Debug("Failed to write data to the ECU");
                }
                int fuelMapAddressFlash = (int)GetSymbolAddress(m_symbols, m_appSettings.AutoTuneFuelMap);
                fuelMapAddressFlash += (y * 18) + e.X;
                savedatatobinary(fuelMapAddressFlash, 1, data2Write, m_currentfile, false);
                UpdateChecksum(m_currentfile);
            }
        }

        void m_AFRMap_onFuelmapCellChanged(object sender, AFRMap.FuelmapChangedEventArgs e)
        {
            // seems that we need to adjust a value in the current fuelmap
            if (m_RealtimeConnectedToECU)
            {
                if (m_appSettings.AutoUpdateFuelMap)
                {
                    if (_autoTuning)
                    {
                        logger.Debug("Writing fuelmap");
                        uint addresstowrite = (uint)GetSymbolAddressSRAM(m_symbols, m_appSettings.AutoTuneFuelMap) + (uint)e.Mapindex;
                        byte[] dataToSend = new byte[1];
                        dataToSend[0] = e.Cellvalue;
                        if (!trionic7.WriteMapToSRAM(addresstowrite, dataToSend))
                        {
                            logger.Debug("Failed to write data to the ECU");
                        }
                    }
                }
                else
                {
                    // update the fuelinformation struct
                }
            }
        }

        void m_AFRMap_onCellLocked(object sender, EventArgs e)
        {
            logger.Debug("AFR maps: cell locked");
            if (sndplayer != null)
            {
                if (m_appSettings.PlayCellProcessedSound)
                {
                    string sound2play = System.Windows.Forms.Application.StartupPath + "\\ping.wav";
                    if (File.Exists(sound2play))
                    {
                        sndplayer.SoundLocation = sound2play;
                        sndplayer.Play();
                    }
                }
            }
        }


        private bool _autoTuning = false;

        private void ProcessAutoTuning(float afr, float rpm, float airmass)
        {
            //<GS-17012011> implement autotuning functions here
            if (_autoTuning)
            {
                //logger.Debug("Autotuning: " + afr.ToString("F2") + " " + rpm.ToString("F0") + " " + airmass.ToString("F1"));
                m_AFRMap.HandleRealtimeData(afr, rpm, airmass);
                // measure current AFR agains targeted AFR and hold for cell stable time.
            }
        }

        private void barButtonItem94_ItemClick(object sender, ItemClickEventArgs e)
        {
            // show AFR target map
            // show feedback afr map if available

            if (m_AFRMap != null && m_currentfile != string.Empty)
            {
                int cols = 18;
                int rows = 16;
                string foldername = Path.Combine(Path.GetDirectoryName(m_currentfile), "AFRMaps");
                if (!Directory.Exists(foldername))
                {
                    Directory.CreateDirectory(foldername);
                }
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, m_appSettings.AutoTuneFuelMap, out cols, out rows);
                m_AFRMap.SaveMap(m_currentfile, cols, rows);
                ShowAfrMAP("TargetAFR", Path.Combine(foldername, Path.GetFileNameWithoutExtension(m_currentfile) + "-targetafr.afr"));
            }
        }

        private void RealtimeDisconnectAndHide()
        {
            if (dockRealtime.Visibility == DockVisibility.Visible)
            {
                dockRealtime.Visibility = DockVisibility.Hidden;
                tmrRealtime.Enabled = false;
            }
            RealtimeDisconnect();
        }

        private void RealtimeDisconnect()
        {
            m_RealtimeConnectedToECU = false;
            trionic7.Cleanup();
            SetCANStatus("");
            btnConnectDisconnect.Caption = "Connect ECU";
        }

        private void SpawnSaabOpenTech(string arguments, bool showWindow)
        {
            RealtimeDisconnectAndHide();

            if (showWindow)
            {
                frmInfoBox info = new frmInfoBox("Lawicel only: Please note that you will have to be connected to the cars I-bus for this to work!");
            }
            System.Windows.Forms.Application.DoEvents();
            string Exename = Path.Combine(System.Windows.Forms.Application.StartupPath, "SaabOpenTech.exe");
            ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = false;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.RedirectStandardOutput = true;
            startinfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
            // set parameters
            startinfo.Arguments = arguments;
            frmProgress progress = new frmProgress();
            progress.SetProgress("Communicating with the car, please wait. This can take upto 30 seconds");
            progress.Show();
            System.Windows.Forms.Application.DoEvents();
            System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
            string o = conv_proc.StandardOutput.ReadToEnd();
            conv_proc.WaitForExit(30000); // wait for 10 seconds max
            if (!conv_proc.HasExited)
            {
                progress.Close();
                System.Windows.Forms.Application.DoEvents();
                conv_proc.Kill();
                frmInfoBox info2 = new frmInfoBox("Maintainance tool was termined, something went wrong.");
            }
            else
            {
                progress.Close();
                System.Windows.Forms.Application.DoEvents();
                frmMaintainanceResult result = new frmMaintainanceResult();
                result.commReport = o;
                result.ShowDialog();
            }
        }

        private void btnSeatbeltPing_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("S 1", true); // seatbelt ping on
        }

        private void btnSetSeatbeltPingOff_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("S 0", true); // seatbelt ping off
        }

        private void btnDoubleUnlockOn_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("U 1", true);// double unlocking on
        }

        private void btnDoubleUnlockOff_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("U 0", true);// double unlocking off
        }

        private void btnTrionicInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("I", false);// show info
            /*System.Windows.Forms.Application.DoEvents();
            logger.Debug("Checking can connection");
            if (CheckCANConnectivityDirectAccess())
            {
                // init 
                canUsbDevice.EnableCanLog = true;
                logger.Debug("Sending session request to trionic");
                //KWPHandler.getInstance().
                System.Windows.Forms.Application.DoEvents();
                canUsbDevice.Flush();
                if (canUsbDevice.sendSessionRequest(0x11)) // 0x11 = Trionic
                {
                    logger.Debug("Query data from trionic");
                    System.Windows.Forms.Application.DoEvents();
                    byte[] buf = new byte[8];
                    int i = canUsbDevice.query_data(0xA1, 0xAC, out buf); // A1 = 11 = trionic
                    logger.Debug("Rx length: " + i.ToString());
                    if (i == 8)
                    {
                        foreach (byte b in buf)
                        {
                            Console.Write(b.ToString("X2") + " ");
                        }
                        logger.Debug();
                    }

                }
                else
                {
                    logger.Debug("Failed to init the session with Trionic");
                }
            }*/
        }

        private void btnSIDDisplayTest_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("T", true);
        }

        private void btnReadEngineData_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("M", false);
        }

        private void btnGetParkAssistanceInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpawnSaabOpenTech("P", false);
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

        private void ViewRealtime_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedSymbol();
        }

        private void btnReleaseNotes_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartReleaseNotesViewer(m_msiUpdater.GetReleaseNotes(), System.Windows.Forms.Application.ProductVersion.ToString());
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
                string filename = Path.Combine(configurationFilesPath.FullName, "mymaps.xml");

                if (File.Exists(filename))
                {
                    try
                    {
                        System.Xml.XmlDocument mymaps = new System.Xml.XmlDocument();
                        mymaps.Load(filename);

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
            catch (Exception E)
            {
                logger.Debug(E, "Failed to create myMaps menu");
            }
        }

        void MyMapDefine_ItemClick(object sender, ItemClickEventArgs e)
        {
            // define myMaps!
            frmDefineMyMaps mymapsdef = new frmDefineMyMaps();
            string filename = Path.Combine(configurationFilesPath.FullName, "mymaps.xml");
            if (File.Exists(filename))
            {
                mymapsdef.Filename = filename;
            }
            else
            {
                mymapsdef.CreateNewFile(filename);
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
                    barButtonItem94_ItemClick(sender, e); // start 'target afr' mapviewer
                else if (map_name == "feedbackafr")
                    btnAFRFeedbackMap_ItemClick(sender, e); // start 'feedback afr' mapviewer
                /*else if (map_name == "feedbackvstargetafr")
                    btnAFRErrormap_ItemClick(sender, e); // start 'feedback vs target afr' mapviewer*/
                else
                    StartTableViewer(e.Item.Tag.ToString().Trim());
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }
        }

        private void btnConnectDisconnect_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (m_RealtimeConnectedToECU)
            {
                trionic7.SuspendAlivePolling();
                RealtimeDisconnect();
            }
            else
            {
                if (RealtimeCheckAndConnect())
                {
                    trionic7.ResumeAlivePolling();
                    btnConnectDisconnect.Caption = "Disconnect ECU";
                }
            }
        }

        private void addToMyMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridViewSymbols.FocusedRowHandle >= 0)
            {
                SymbolHelper sh = (SymbolHelper)gridViewSymbols.GetRow(gridViewSymbols.FocusedRowHandle);
                SymbolCollection scmymaps = new SymbolCollection();
                SymbolHelper shnewmymap = new SymbolHelper();
                shnewmymap.Varname = sh.Varname;
                shnewmymap.Description = sh.Varname;
                shnewmymap.Category = "Directly added";
                scmymaps.Add(shnewmymap);
                string filename = Path.Combine(configurationFilesPath.FullName, "mymaps.xml");

                if (File.Exists(filename))
                {
                    try
                    {
                        System.Xml.XmlDocument mymaps = new System.Xml.XmlDocument();
                        mymaps.Load(filename);
                        foreach (System.Xml.XmlNode category in mymaps.SelectNodes("categories/category"))
                        {
                            foreach (System.Xml.XmlNode map in category.SelectNodes("map"))
                            {
                                SymbolHelper shmap = new SymbolHelper();
                                shmap.Varname = map.Attributes["symbol"].Value;
                                shmap.Category = category.Attributes["title"].Value;
                                shmap.Description = map.Attributes["title"].Value;
                                scmymaps.Add(shmap);
                            }
                        }
                    }
                    catch { }
                }
                // now save a new file
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                XmlDocument doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
                doc.AppendChild(dec);// Create the root element
                XmlElement root = doc.CreateElement("categories");
                doc.AppendChild(root);

                scmymaps.SortColumn = "Category";
                scmymaps.SortingOrder = GenericComparer.SortOrder.Ascending;
                scmymaps.Sort();

                string previouscat = "";
                XmlElement title = doc.CreateElement("category");
                foreach (SymbolHelper shmm in scmymaps)
                {
                    if (shmm.Category != previouscat)
                    {
                        previouscat = shmm.Category;
                        title = doc.CreateElement("category");
                        title.SetAttribute("title", previouscat);
                        root.AppendChild(title);
                    }
                    XmlElement map = doc.CreateElement("map");
                    map.SetAttribute("symbol", shmm.Varname);
                    map.SetAttribute("title", shmm.Description);
                    title.AppendChild(map);
                }
                doc.Save(filename);
                if (ribbonControl1.Pages[3].Text == "My Maps")
                {
                    ribbonControl1.Pages.RemoveAt(3);
                }
                LoadMyMaps();
            }
        }

        private void exportSymbollistAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // export as CSV
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV files|*.csv", Title = "Select a filename to save the symbollist" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                    {
                        foreach (SymbolHelper sh in m_symbols)
                        {
                            sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}", sh.Varname.Replace(',', '.'), sh.Flash_start_address, sh.Start_address, sh.Length, sh.Symbol_number, sh.Symbol_type, sh.Userdescription));
                        }
                    }
                    frmInfoBox info = new frmInfoBox("Export done");
                }
            }
        }

        private void barIdcGenerate_ItemClick(object sender, ItemClickEventArgs e)
        {
            IdaProIdcFile.create(m_currentfile, m_symbols);
        }

        private void DynamicTuningMenu()
        {
            //
            // Show Tuning menu shortcuts depening on which file that was loaded.
            //
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(m_currentfile))
                {
                    if (IsBinaryB308TrionicV6())
                    {
                        barButtonItem2.Visibility = BarItemVisibility.Always;
                        barButtonItem3.Visibility = BarItemVisibility.Always;
                        barButtonItem27.Visibility = BarItemVisibility.Always;
                    }
                    else
                    {
                        barButtonItem2.Visibility = BarItemVisibility.Never;
                        barButtonItem3.Visibility = BarItemVisibility.Never;
                        barButtonItem27.Visibility = BarItemVisibility.Never;
                    }
                    
                    if (IsSymbolInBinary("BoostCal.RegMap"))
                    {
                        ribbonPageGroup22.Visible = true;
                    }
                    else
                    {
                        ribbonPageGroup22.Visible = false;
                    }

                    if (IsBinaryBiopower())
                    {
                        barButtonItem37.Visibility = BarItemVisibility.Always;
                        barButtonItem65.Visibility = BarItemVisibility.Always;
                        if (IsSymbolInBinary("TorqueCal.M_EngMaxE85TabAut"))
                        {
                            barButtonItem95.Visibility = BarItemVisibility.Always;
                        }
                        else
                        {
                            barButtonItem95.Visibility = BarItemVisibility.Never;
                        }
                    }
                    else
                    {
                        barButtonItem95.Visibility = BarItemVisibility.Never;
                        barButtonItem37.Visibility = BarItemVisibility.Never;
                        barButtonItem65.Visibility = BarItemVisibility.Never;
                    }

                    if (IsSymbolInBinary("TorqueCal.M_CabGearLim"))
                    {
                        barButtonItem54.Visibility =  BarItemVisibility.Always;
                    }
                    else
                    {
                        barButtonItem54.Visibility =  BarItemVisibility.Never;
                    }
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("BFuelCal2.Map");
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartAViewer("BFuelCal2.StartMap");
        }

        private void barButtonExportLogCsv_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Trionic 7 logfiles|*.t7l";
            ofd.Title = "Open CAN bus logfile";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ConvertFileToCSV(ofd.FileName, false);
            }
        }

        private void ConvertFileToCSV(string filename, bool AutoExport)
        {
            System.Windows.Forms.Application.DoEvents();
            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            try
            {
                SymbolCollection sc = LogFile.FindSymbols(filename, ref startDate, ref endDate);

                if (AutoExport)
                {
                    InitExportCSV(filename, startDate, endDate);
                }
                else
                {
                    // show selection screen
                    frmPlotSelection plotsel = new frmPlotSelection(suiteRegistry);
                    foreach (SymbolHelper sh in sc)
                    {
                        plotsel.AddItemToList(sh.SmartVarname);
                    }
                    plotsel.Startdate = startDate;
                    plotsel.Enddate = endDate;
                    plotsel.SelectAllSymbols();
                    if (plotsel.ShowDialog() == DialogResult.OK)
                    {
                        sc = plotsel.Sc;
                        endDate = plotsel.Enddate;
                        startDate = plotsel.Startdate;
                        InitExportCSV(filename, startDate, endDate);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
        }

        private void InitExportCSV(string filename, DateTime startDate, DateTime endDate)
        {
            CSVGenerator csvgen = new CSVGenerator();
            LogFilters filterhelper = new LogFilters(suiteRegistry);
            csvgen.SetFilters(filterhelper.GetFiltersFromRegistry());
            csvgen.AppSettings = m_appSettings;
            //csvgen.LowAFR = m_appSettings.WidebandLowAFR;
            //csvgen.HighAFR = m_appSettings.WidebandHighAFR;
            //csvgen.MaximumVoltageWideband = m_appSettings.WidebandHighVoltage;
            //csvgen.MinimumVoltageWideband = m_appSettings.WidebandLowVoltage;
            csvgen.WidebandSymbol = m_appSettings.WideBandSymbol;
            //csvgen.UseWidebandInput = m_appSettings.UseWidebandLambdaThroughSymbol;
            csvgen.UseWidebandInput = false;

            csvgen.onExportProgress += new CSVGenerator.ExportProgress(difgen_onExportProgress);
            frmProgressExportLog = new frmProgress();
            frmProgressExportLog.SetProgress("Exporting to CSV");
            frmProgressExportLog.Show();
            System.Windows.Forms.Application.DoEvents();
            try
            {
                if (!csvgen.ConvertFileToCSV(filename, startDate, endDate))
                {
                    frmInfoBox info = new frmInfoBox("No data was found to export!");
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            frmProgressExportLog.Close();
        }

        void difgen_onExportProgress(object sender, CSVGenerator.ProgressEventArgs e)
        {
            frmProgressExportLog.SetProgressPercentage(e.Percentage);
        }

        public enum TuneWizardType
        {
            None = 0,       // Should never happen
            Embedded,       // Hard coded routines
            TuningFile      // Future: Tuning Packages when installing solution.
        };

        public enum BinaryType
        {
            None = 0,       // Should never happen
            OldBin,         // BIN Older than FC01
            NewBin,         // BIN Newer than FC01
            BothBin
        };

        public class TuningAction
        {
            public string WizName;
            public string WizIdOrFilename;
            public TuneWizardType WizType;
            public BinaryType WizBinType;
            public string[] WizWhitelist;
            public string[] WizBlacklist;
            public string WizCode;
            public string WizAuth;
            public string WizMsg;

            public TuningAction()
            {
                WizType = TuneWizardType.None;
                WizName = string.Empty;
                WizIdOrFilename = string.Empty;
                WizBinType = BinaryType.None;
                WizWhitelist = new string[] { };
                WizBlacklist = new string[] { };
                WizCode = string.Empty;
                WizAuth = string.Empty;
            }

            public override string ToString()
            {
                if (WizCode != string.Empty)
                    return WizName + " [Protected]";

                return WizName;
            }

            public string GetWizardIdOrFilename()
            {
                return WizIdOrFilename;
            }

            public TuneWizardType GetWizardType()
            {
                return WizType;
            }

            public BinaryType GetWizBinaryType()
            {
                return WizBinType;
            }

            public bool compatibelSoftware(string software)
            {
                bool inWhiteList = true; // If no whitelist exist, it is ok
                if (WizWhitelist.Length > 0)
                {
                    inWhiteList = false; // When whitelist exist, make sure it's in there
                    foreach (string white in WizWhitelist)
                    {
                        if (white.Length <= 0)
                            continue;

                        int ast = white.IndexOf('*');
                        string strComp = string.Empty;
                        if (ast != -1)
                            strComp = white.Substring(0, ast);
                        else
                            strComp = white;

                        if (software.StartsWith(strComp))
                        {
                            // We have a white list match, we are now done
                            inWhiteList = true;
                            break;
                        }
                    }
                }

                bool inBlackList = false; // Assume not in blacklist
                foreach (string black in WizBlacklist)
                {
                    if (black.Length <= 0)
                        continue;

                    int ast = black.IndexOf('*');
                    string strComp = string.Empty;
                    if (ast != -1)
                        strComp = black.Substring(0, ast);
                    else
                        strComp = black;

                    if (software.StartsWith(strComp))
                    {
                        // We have a white list match, we are now done
                        inBlackList = true;
                        break;
                    }
                }

                if (inWhiteList && !inBlackList)
                    return true;

                return false;
            }

            public virtual int performTuningAction(frmMain p, string software, out List<string> out_mod_symbols)
            {
                // NOTE: To avoid error "Cannot access a non-static member of outer type  via nested type"
                //       we need to call frmMain functions though the instance of it
                out_mod_symbols = new List<string>();
                return 0;
            }
        }
        public class FileTuningAction : TuningAction
        {
            public FileTuningAction(string name, string filename, BinaryType type, string[] whitelist, string[] blacklist, string code, string author, string msg)
            {
                WizName = name;
                WizIdOrFilename = filename;
                WizType = TuneWizardType.TuningFile;
                WizBinType = type;
                WizWhitelist = whitelist;
                WizBlacklist = blacklist;
                WizCode = code;
                WizAuth = author;
                WizMsg = msg;
            }

            public override int performTuningAction(frmMain p, string software, out List<string> out_mod_symbols)
            {
                out_mod_symbols = new List<string>();
                List<FileTuningPackage> tuningPackages;
                string binType = string.Empty;
                string whitelist = string.Empty;  // Not used in t8p
                string blacklist = string.Empty;  // Not used in t8p
                string code = string.Empty;       // Not used in t8p

                tuningPackages = p.ReadTuningPackageFile(true, WizIdOrFilename, out binType, out whitelist, out blacklist, out code);

                // Here we need the software version in memory
                if (compatibelSoftware(software))
                {
                    // Save a copy
                    string backup_file = Path.GetFileNameWithoutExtension(p.m_currentfile) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-BACKUP-BEFORE-WIZARD-" + WizName + ".bin";
                    string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                    Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                    backup_file = r.Replace(backup_file, "");

                    backup_file = Path.GetDirectoryName(p.m_currentfile) + "\\" + backup_file;
                    File.Copy(p.m_currentfile, backup_file, true);

                    p.ApplyTuningPackage(tuningPackages);
                    foreach (FileTuningPackage tp in tuningPackages)
                        if (tp.hasResult)
                        {
                            out_mod_symbols.Add(tp.result);
                        }
                        else
                        {
                            if (tp.succesful)
                                out_mod_symbols.Add("OK: " + tp.GetNameTPAction());
                            else
                                out_mod_symbols.Add("Fail: " + tp.GetNameTPAction());
                        }
                    p.RefreshTableViewers();
                }
                return 0;
            }
        }

        public class DateAndName : TuningAction
        {
            public DateAndName()
            {
                WizType = TuneWizardType.Embedded;
                WizBinType = BinaryType.None;
                WizName = "Update footer";
                WizIdOrFilename = "ap_dateName";
            }

            public override int performTuningAction(frmMain p, string software, out List<string> out_mod_symbols)
            {
                // NOTE: To avoid error "Cannot access a non-static member of outer type  via nested type"
                //       we need to call frmMain functions though the instance of it
                out_mod_symbols = new List<string>();

                T7FileHeader t7header = new T7FileHeader();
                t7header.init(p.m_currentfile, false);
                t7header.setSIDDate(DateTime.Now.ToString("yyMMdd"));
                return 0;
            }
        }

        public static List<TuningAction> installedTunings = new List<TuningAction>
        {
            new DateAndName() // This SHOULD BE IN POSITION #1
        };

        public void addWizTuneFilePacks()
        {
            // List all files in start-up directory/TuningPacks
            string path = Path.Combine(System.Windows.Forms.Application.StartupPath, "TuningPacks");
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.t7x");
                foreach (string file in files)
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string file_for_md5 = string.Empty;
                        string line = string.Empty;
                        string enc_line = string.Empty;
                        string packname = string.Empty;
                        string sPacktype = string.Empty;
                        string signature = string.Empty;
                        string code = string.Empty;
                        string msg = string.Empty;
                        string author = string.Empty;
                        string[] whitelist = new string[] { };
                        string[] blacklist = new string[] { };
                        frmMain.BinaryType packtype = frmMain.BinaryType.None;

                        // Read signature
                        string s = sr.ReadLine();
                        if (s != null && s.StartsWith("<SIGNATURE>"))
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                if (s.StartsWith("</SIGNATURE>"))
                                    break;
                                signature += s;
                            }
                        }

                        // Read and decrypt the tuning package 
                        while ((enc_line = sr.ReadLine()) != null)
                        {
                            line = Crypto.DecodeAES(enc_line).Trim();
                            file_for_md5 += line + "\x0d\x0a";
                            if (line.StartsWith("packname="))
                            {
                                packname = line.Replace("packname=", "");
                            }
                            else if (line.StartsWith("bintype="))
                            {
                                sPacktype = line.Replace("bintype=", "");
                                if (sPacktype == "OLD")
                                {
                                    packtype = frmMain.BinaryType.OldBin;
                                }
                                else if (sPacktype == "NEW")
                                {
                                    packtype = BinaryType.NewBin;
                                }
                                else if (sPacktype == "BOTH")
                                {
                                    packtype = BinaryType.BothBin;
                                }
                            }
                            else if (line.StartsWith("whitelist="))
                            {
                                line = Regex.Replace(line, @"\s+", "");
                                whitelist = line.Replace("whitelist=", "").Split(',');
                            }
                            else if (line.StartsWith("blacklist="))
                            {
                                line = Regex.Replace(line, @"\s+", "");
                                blacklist = line.Replace("blacklist=", "").Split(',');
                            }
                            else if (line.StartsWith("code="))
                            {
                                line = Regex.Replace(line, @"\s+", "");
                                code = line.Replace("code=", "");
                            }
                            else if (line.StartsWith("msg="))
                            {
                                //line = Regex.Replace(line, @"\s+", "");
                                msg = line.Replace("msg=", "");
                            }
                            else if (line.StartsWith("author="))
                            {
                                //line = Regex.Replace(line, @"\s+", "");
                                author = line.Replace("author=", "");
                            }
                        }

                        // Calculate MD5 of content and verify it against signature
                        if (Crypto.VerifyRSASignature(Crypto.CalculateMD5Hash(file_for_md5), signature))
                        {
                            FileTuningAction tp = new frmMain.FileTuningAction(packname, file, packtype, whitelist, blacklist, code, author, msg);
                            installedTunings.Add(tp);
                        }
                        else
                        {
                            logger.Debug("Signature check failed for file: " + file);
                        }
                    }
                }
            }
        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmTuningWizard frmTunWiz = new frmTuningWizard(this, m_currentfile);
            frmTunWiz.ShowDialog();
        }
    }
}