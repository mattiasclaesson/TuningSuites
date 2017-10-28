using System;
using System.Collections.Generic;
using System.Windows.Forms;
using T5CANLib;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;


namespace T5CanFlasher
{
    public partial class frmMain : Form
    {
        T5CAN t5can;
        T5CANLib.CAN.ICANDevice device;
        readonly Stopwatch stopwatch = new Stopwatch();
        ECUType ECU_type = ECUType.Unknown;
        readonly string commlogFilename = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\commlog.txt";
        private readonly T5CanFlasher.Properties.Settings set = new T5CanFlasher.Properties.Settings();

        public frmMain(string[] argv)
        {
            InitializeComponent();
            manageControls(programMode.notconnected);
            switch (set.AdapterType)
            {
                case "LAWICEL":
                    comboInterface.SelectedIndex = 0;
                    break;
                case "COMBI":
                    comboInterface.SelectedIndex = 1;
                    break;
                case "DIY":
                    comboInterface.SelectedIndex = 2;
                    break;
                case "J4T":
                    comboInterface.SelectedIndex = 3;
                    break;
                case "Kvaser":
                    comboInterface.SelectedIndex = 4;
                    break;
                default:
                    comboInterface.SelectedIndex = 0;
                    break;
            }
            if (argv.Length > 0)
            {
                foreach (string arg in argv)
                {
                    switch (arg)
                    {
                        case "LAWICEL":
                            comboInterface.SelectedIndex = 0;
                            break;
                        case "COMBI":
                            comboInterface.SelectedIndex = 1;
                            break;
                        case "DIY":
                            comboInterface.SelectedIndex = 2;
                            break;
                        case "J4T":
                            comboInterface.SelectedIndex = 3;
                            break;
                        case "Kvaser":
                            comboInterface.SelectedIndex = 4;
                            break;
                        default:
                            comboInterface.SelectedIndex = 0;
                            break;
                    }
                }
            }
            else
            {
                LoadRegistrySettings();
            }
        }

        // Load the Main form
        private void frmMain_Load(object sender, EventArgs e)
        {
            //device.EnableLogging(Application.StartupPath);
            manageControls(programMode.notconnected);
            AddToLog("T5 CAN Flasher version: " + Application.ProductVersion);
            t5can = new T5CAN();
            t5can.onWriteProgress += new T5CAN.WriteProgress(t5can_onWriteProgress);
            t5can.onCanInfo += new T5CAN.CanInfo(t5can_onCanInfo);
            t5can.onBytesTransmitted += new T5CAN.BytesTransmitted(t5can_onBytesTransmitted);
            //device.DisableLogging();
        }

        private void LoadRegistrySettings()
        {
            RegistryKey TempKey = Registry.CurrentUser.CreateSubKey("Software");

            using (RegistryKey Settings = TempKey.CreateSubKey("T5CANFlasher"))
            {
                if (Settings != null)
                {
                    string[] vals = Settings.GetValueNames();
                    foreach (string a in vals)
                    {
                        try
                        {
                            if (a == "AdapterType")
                            {
                                comboInterface.SelectedItem = Settings.GetValue(a).ToString();
                            }
                            else if (a == "EnableLogging")
                            {
                                cboxEnLog.Checked = Convert.ToBoolean(Settings.GetValue(a).ToString());
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        private static void SaveRegistrySetting(string key, string value)
        {
            RegistryKey TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey saveSettings = TempKey.CreateSubKey("T5CANFlasher"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        private static void SaveRegistrySetting(string key, bool value)
        {
            RegistryKey TempKey = Registry.CurrentUser.CreateSubKey("Software");
            using (RegistryKey saveSettings = TempKey.CreateSubKey("T5CANFlasher"))
            {
                saveSettings.SetValue(key, value);
            }
        }

        private void AddToLog(string item)
        {
            DateTime dt = DateTime.Now;
            string line = dt.ToString("HH:mm:ss") + "." + dt.Millisecond.ToString("D3") + " - " + item;
            listBox1.Items.Add(line);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            using (StreamWriter sw = new StreamWriter(commlogFilename, true))
            {
                sw.WriteLine(line);
            }
            //while (listBox1.Items.Count > 5000) listBox1.Items.RemoveAt(0);
            Application.DoEvents();
        }

        int last_progress = 0;
        int delaycount = 0;

        void t5can_onBytesTransmitted(object sender, WriteProgressEventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > 0)
            {
                int bytespersecond = (1000 * e.Percentage) / (int)stopwatch.ElapsedMilliseconds;
                label2.Text = "Transmission speed: " + bytespersecond.ToString() + " B/s";
                label1.Text = "Transferred: " + e.Percentage.ToString() + " Bytes";
                // calculate ETA
                // total amount to transfer = 100/percentage * bytetransferred
                if (progressBar1.Value > 0)
                {
                    if (progressBar1.Value != last_progress)
                    {
                        int totalbytes = (int)((float)((float)100 / (float)progressBar1.Value) * (float)e.Percentage);
                        // time remaining = (totalbytes - bytestranferred)/bytespersecond
                        int secondsleft = (totalbytes - e.Percentage) / bytespersecond;
                        int minutes = secondsleft / 60;
                        int seconds = secondsleft - (minutes * 60);
                        label3.Text = "Remaining time: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");
                        last_progress = progressBar1.Value;
                        //Console.WriteLine("total bytes: " + totalbytes.ToString() + " seconds left: " + secondsleft.ToString() + " bytestransferred: :" + e.Percentage.ToString() + " bps: " + bytespersecond.ToString());
                        //if ((delaycount++ % 10) == 0)
                        {
                            Application.DoEvents();
                        }
                    }
                    else if ((delaycount++ % 10) == 0)
                    {
                        int totalbytes = (int)((float)((float)100 / (float)progressBar1.Value) * (float)e.Percentage);
                        // time remaining = (totalbytes - bytestranferred)/bytespersecond
                        int secondsleft = (totalbytes - e.Percentage) / bytespersecond;
                        int minutes = secondsleft / 60;
                        int seconds = secondsleft - (minutes * 60);
                        label3.Text = "Remaining time: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");
                        last_progress = progressBar1.Value;
                        //Console.WriteLine("total bytes: " + totalbytes.ToString() + " seconds left: " + secondsleft.ToString() + " bytestransferred: :" + e.Percentage.ToString() + " bps: " + bytespersecond.ToString());
                        Application.DoEvents();
                    }
                }
            }
        }

        void t5can_onCanInfo(object sender, CanInfoEventArgs e)
        {
            if (e.Type == ActivityType.StartUploadingBootloader || e.Type == ActivityType.StartFlashing || e.Type == ActivityType.StartDownloadingFlash || e.Type == ActivityType.StartDownloadingFooter)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
            else if (e.Type == ActivityType.FinishedUploadingBootloader || e.Type == ActivityType.FinishedDownloadingFlash || e.Type == ActivityType.FinishedDownloadingFooter || e.Type == ActivityType.FinishedFlashing)
            {
                progressBar1.Value = 0;
            }
            AddToLog(e.Info);
        }

        void t5can_onWriteProgress(object sender, WriteProgressEventArgs e)
        {
            if (progressBar1.Value != e.Percentage)
            {
                progressBar1.Value = e.Percentage;
                if (e.Percentage > 99)
                {
                    progressBar1.Value = 0;
                }
                Application.DoEvents();
            }
        }

        // EXIT
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveRegistrySetting("AdapterType", comboInterface.SelectedItem.ToString());
                SaveRegistrySetting("EnableLogging", cboxEnLog.Checked);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            try
            {
                t5can.Cleanup();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            Environment.Exit(0);
        }

        // FLASH
        private void btnFLASH_Click(object sender, EventArgs e)
        {
            manageControls(programMode.active);
            if (cboxEnLog.Checked)
            {
                device.EnableLogging(Application.StartupPath);
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary files|*.bin";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.FileName);
                bool OkToUpgrade = true;
                switch (ECU_type)
                {
                    case ECUType.T52ECU:
                        if (fi.Length != 0x20000)
                        {
                            MessageBox.Show("Not a Trionic 5.2 BIN File",
                                "ERROR",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
                            OkToUpgrade = false;
                        }
                        break;
                    case ECUType.T55ECU:
                        switch (fi.Length)
                        {
                            case 0x20000:
                                DialogResult result = MessageBox.Show("Do you want to upload a T5.2 BIN file to your T5.5 ECU",
                                    "ECU Conversion Question",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2);
                                OkToUpgrade = (result == DialogResult.Yes) ? true : false;
                                break;
                            case 0x40000:
                                break;
                            default:
                                MessageBox.Show("Not a Trionic 5.5 BIN File",
                                    "ERROR",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                                OkToUpgrade = false;
                                break;
                        }
                        break;
                    case ECUType.T55AST52:
                        switch (fi.Length)
                        {
                            case 0x20000:
                                break;
                            case 0x40000:
                                DialogResult result = MessageBox.Show("Do you want to upload a T5.5 BIN file to your ECU that has been used as a T5.2?",
                                    "ECU Conversion Question",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2);
                                OkToUpgrade = (result == DialogResult.Yes) ? true : false;
                                break;
                            default:
                                MessageBox.Show("Not a Trionic BIN File",
                                    "ERROR",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                                OkToUpgrade = false;
                                break;
                        }
                        break;
                }

                if (OkToUpgrade)
                {
                    AddToLog("Starting FLASH update session...");
                    statusActivity.Text = "Updating FLASH";
                    T5CANLib.UpgradeResult result = t5can.UpgradeECU(ofd.FileName, ECU_type);
                    statusActivity.Text = "IDLE";
                    switch (result)
                    {
                        case T5CANLib.UpgradeResult.Success:
                            AddToLog("!!! SUCCESS !!!");
                            getECUinfo();
                            AddToLog("Your ECU is ready to use your new BIN file :-)");
                            break;
                        case T5CANLib.UpgradeResult.InvalidFile:
                            AddToLog("!!! ERROR !!! Invalid file for the selected ECU type :-o");
                            break;
                        case T5CANLib.UpgradeResult.InvalidECUType:
                            AddToLog("!!! ERROR !!! Invalid ECU type selected :-o");
                            break;
                        case T5CANLib.UpgradeResult.ProgrammingFailed:
                            AddToLog("!!! FAILURE !!! Could not program the FLASH in your ECU :-(");
                            break;
                        case T5CANLib.UpgradeResult.EraseFailed:
                            AddToLog("!!! FAILURE !!! Could not erase the FLASH in your ECU :-(");
                            break;
                        case T5CANLib.UpgradeResult.ChecksumFailed:
                            AddToLog("!!! FAILURE !!! Checksums don't match after FLASHing :-(");
                            break;
                        default:
                            AddToLog("!!! ERROR!!! There was a problem I haven't catered for ???");
                            break;
                    }
                    switch (result)
                    {
                        case T5CANLib.UpgradeResult.ProgrammingFailed:
                        case T5CANLib.UpgradeResult.EraseFailed:
                        case T5CANLib.UpgradeResult.ChecksumFailed:
                            AddToLog("There are many reasons why you saw this error message,");
                            AddToLog("e.g. a problem with your power supply voltage or you may");
                            AddToLog("be unlucky enough to have 'Bad FLASH chips' :-(");
                            AddToLog("You should retry FLASHing your BIN file but if it fails");
                            AddToLog("again your only option is to try to recover your ECU");
                            AddToLog("using a BDM interface !!!");
                            break;
                        default:
                            break;
                    }
                }
            }
            device.DisableLogging();
            manageControls(programMode.connected);
        }

        // DUMP
        private void btnDUMP_Click(object sender, EventArgs e)
        {
            manageControls(programMode.active);
            if (cboxEnLog.Checked)
            {
                device.EnableLogging(Application.StartupPath);
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "bin";
            sfd.Filter = "Binary files|*.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (ECU_type)
                {
                    case ECUType.T52ECU:
                        AddToLog("Reading a 128 kB T5.2 BIN file from your Trionic 5.2 ECU...");
                        break;
                    case ECUType.T55AST52:
                        AddToLog("Reading a 128 kB T5.2 BIN file from your Trionic 5.5 ECU...");
                        break;
                    case ECUType.T55ECU:
                        AddToLog("Reading a 256 kB T5.5 BIN file from your Trionic 5.5 ECU...");
                        break;
                    default:
                        break;
                }
                statusActivity.Text = "Reading FLASH";
                t5can.DumpECU(sfd.FileName, ECU_type);
                statusActivity.Text = "IDLE";
            }
            device.DisableLogging();
            manageControls(programMode.connected);
        }


        // Try to connect to an ECU
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cboxEnLog.Checked)
            {
                device.EnableLogging(Application.StartupPath);
            }
            manageControls(programMode.active);
            if (device != null)
            {
                t5can.setCANDevice(device);
                if (t5can.openDevice())
                {
                    statusAdapter.Text = "Adapter: Connected";
                    //AddToLog("Table: " + t5can.getSymbolTable());
                    // send bootloader
                    Console.WriteLine("Uploading bootloader");
                    statusActivity.Text = "Sending Bootloader";
                    if (t5can.UploadBootLoader())
                    {
                        Console.WriteLine("Bootloader uploaded");
                        getECUinfo();
                        // enable flash and dump buttons
                        manageControls(programMode.connected);
                    }
                    else
                    {
                        AddToLog("Couldn't connect to your T5 ECU");
                        manageControls(programMode.notconnected);
                    }
                    statusActivity.Text = "IDLE";
                }
                else
                {
                    AddToLog("Couldn't open CAN connection");
                    statusAdapter.Text = "Adapter: Not Connected";
                    manageControls(programMode.notconnected);
                }
                device.DisableLogging();
            }
        }

        private void getECUinfo()
        {
            statusActivity.Text = "getting Footer";
            byte[] footer = t5can.getECUFooter();
            byte[] chiptypes = t5can.GetChipTypes();
            statusActivity.Text = "IDLE";
            string swversion = t5can.getIdentifierFromFooter(footer, ECUIdentifier.Dataname);
            string romoffset = t5can.getIdentifierFromFooter(footer, ECUIdentifier.ROMoffset);
            string checksum = t5can.ReturnChecksum();
            statusSWVersion.Text = "SW Version: " + swversion;
            statusChecksum.Text = "Checksum: " + checksum;
            // identify ECU
            string flashzize = "256 kB";
            switch (chiptypes[0])
            {
                case 0xB8:      // Intel/CSI/OnSemi 28F512
                case 0x25:      // AMD 28F512
                    statusFLASHType.Text = "Type: 28F512";
                    flashzize = "128 kB";
                    break;
                case 0x5D:      // Atmel 29C512
                    statusFLASHType.Text = "Type: 29C512";
                    flashzize = "128 kB";
                    break;
                case 0xB4:      // Intel/CSI/OnSemi 28F010
                case 0xA7:      // AMD 28F010
                    statusFLASHType.Text = "Type: 28F010";
                    break;
                case 0x20:      // AMD/ST 29F010
                case 0xA4:      // AMIC 29F010
                    statusFLASHType.Text = "Type: 29F010";
                    break;
                case 0xD5:      // Atmel 29C010
                    statusFLASHType.Text = "Type: 29C010";
                    break;
                case 0xB5:      // SST 39F010
                    statusFLASHType.Text = "Type: 39F010";
                    break;
                default:
                    statusFLASHType.Text = "Type: 0x" + chiptypes[0].ToString("X2") + " - Unknown";
                    flashzize = "Unknown";
                    break;
            }
            statusFLASHSize.Text = "Size: " + flashzize;
            switch (chiptypes[1])
            {
                case 0x89:      // Intel
                    statusFLASHMake.Text = "Make: Intel";
                    break;
                case 0x01:      // AMD
                    statusFLASHMake.Text = "Make: AMD/Spansion";
                    break;
                case 0x31:      // CSI/OnSemi
                    statusFLASHMake.Text = "Make: CSI";
                    break;
                case 0x1F:      // Atmel
                    statusFLASHMake.Text = "Make: Atmel";
                    break;
                case 0xBF:      // SST/Microchip
                    statusFLASHMake.Text = "Make: SST/Microchip";
                    break;
                case 0x20:      // ST
                    statusFLASHMake.Text = "Make: ST Microelectronics";
                    break;
                case 0x37:      // AMIC
                    statusFLASHMake.Text = "Make: AMIC";
                    break;
                default:
                    statusFLASHMake.Text = "Make: 0x" + chiptypes[1].ToString("X2") + " - Unknown";
                    break;
            }
            switch (flashzize)
            {
                case "128 kB":
                    switch (romoffset)
                    {
                        case "060000":
                            ECU_type = ECUType.T52ECU;
                            statusECU.Text = "ECU Type: Trionic 5.2";
                            AddToLog("This is a Trionic 5.2 ECU with 128 kB of FLASH");
                            break;
                        default:
                            ECU_type = ECUType.Unknown;
                            statusECU.Text = "ECU Type: Unknown";
                            AddToLog("!!! ERROR !!! This type of ECU is unknown");
                            break;
                    }
                    break;
                case "256 kB":
                    switch (romoffset)
                    {
                        case "040000":
                            ECU_type = ECUType.T55ECU;
                            statusECU.Text = "ECU Type: Trionic 5.5";
                            AddToLog("This is a Trionic 5.5 ECU with 256 kB of FLASH");
                            break;
                        case "060000":
                            ECU_type = ECUType.T55AST52;
                            statusECU.Text = "ECU Type: T5.5 as T5.2";
                            AddToLog("This is a Trionic 5.5 ECU with a T5.2 BIN");
                            break;
                        default:
                            ECU_type = ECUType.Unknown;
                            statusECU.Text = "ECU Type: Unknown";
                            AddToLog("!!! ERROR !!! This type of ECU is unknown");
                            break;
                    }
                    break;
                default:
                    ECU_type = ECUType.Unknown;
                    statusECU.Text = "ECU Type: Unknown";
                    AddToLog("!!! ERROR !!! This type of ECU is unknown");
                    break;
            }
            AddToLog("Part Number: " + t5can.getIdentifierFromFooter(footer, ECUIdentifier.Partnumber));
            AddToLog("Software ID: " + t5can.getIdentifierFromFooter(footer, ECUIdentifier.SoftwareID));
            AddToLog("SW Version: " + swversion);
            AddToLog("Engine Type: " + t5can.getIdentifierFromFooter(footer, ECUIdentifier.EngineType));
            AddToLog("IMMO Code: " + t5can.getIdentifierFromFooter(footer, ECUIdentifier.ImmoCode));
            AddToLog("Other Info: " + t5can.getIdentifierFromFooter(footer, ECUIdentifier.Unknown));
            AddToLog("ROM Start: 0x" + romoffset);
            AddToLog("Code End: 0x" + t5can.getIdentifierFromFooter(footer, ECUIdentifier.CodeEnd));
            AddToLog("ROM End: 0x" + t5can.getIdentifierFromFooter(footer, ECUIdentifier.ROMend));
            AddToLog("Checksum: " + checksum);
        }

        private void comboInterface_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboInterface.SelectedIndex)
            {
                case 0:     // useLawicel
                    device = new T5CANLib.CAN.CANUSBDevice();
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [Lawicel Adapter]";
                    break;
                case 1:     // useCombiadapter
                    device = new T5CANLib.CAN.LPCCANDevice_T5();
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [Combi Adapter]";
                    break;
                case 2:     // useDIYadapter
                    device = new T5CANLib.CAN.MctCanDevice();
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [DIY Adapter]";
                    break;
                case 3:     // useJust4Trionic
                    device = new T5CANLib.CAN.Just4TrionicDevice();
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [Just4Trionic Adapter]";
                    break;
                case 4:     // useKvaser
                    device = new T5CANLib.CAN.KvaserCANDevice();
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [Kvaser Adapter]";
                    break;
                default:
                    device = null;
                    this.Text = "Trionic 5 CAN Flasher v" + Application.ProductVersion + " [!!! No Adapter Selected !!!]";
                    break;
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            // show the about screen
            using (frmAbout about = new frmAbout())
            {
                about.SetVersion(Application.ProductVersion);
                about.ShowDialog();
            }
        }

        private void manageControls(programMode mode)
        {
            switch (mode)
            {
                case programMode.notconnected:
                    comboInterface.Enabled = true;
                    btnConnect.Enabled = true;
                    btnDUMP.Enabled = false;
                    btnFLASH.Enabled = false;
                    btnAbout.Enabled = true;
                    btnExit.Enabled = true;
                    cboxEnLog.Enabled = true;
                    break;
                case programMode.connected:
                    comboInterface.Enabled = false;
                    btnConnect.Enabled = false;
                    btnDUMP.Enabled = true;
                    btnFLASH.Enabled = true;
                    btnAbout.Enabled = true;
                    btnExit.Enabled = true;
                    cboxEnLog.Enabled = true;
                    break;
                case programMode.active:
                    comboInterface.Enabled = false;
                    btnConnect.Enabled = false;
                    btnDUMP.Enabled = false;
                    btnFLASH.Enabled = false;
                    btnAbout.Enabled = false;
                    btnExit.Enabled = false;
                    cboxEnLog.Enabled = false;
                    break;
                default:    // only allow the user to exit in case we get here
                    comboInterface.Enabled = false;
                    btnConnect.Enabled = false;
                    btnDUMP.Enabled = false;
                    btnFLASH.Enabled = false;
                    btnAbout.Enabled = false;
                    btnExit.Enabled = true;
                    cboxEnLog.Enabled = false;
                    break;
            }
        }

        private enum programMode : int
        {
            notconnected,
            connected,
            active
        }
    }
}