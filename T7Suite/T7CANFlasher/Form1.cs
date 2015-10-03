using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using T7;
using T7.KWP;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace T7CANFlasher
{
    public delegate void DelegateUpdateStatus(string logitem);

    public partial class Form1 : Form
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint MM_BeginPeriod(uint uMilliseconds);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint MM_EndPeriod(uint uMilliseconds);
        public DelegateUpdateStatus m_DelegateUpdateStatus;

        private T7CANFlasher.Properties.Settings set = new T7CANFlasher.Properties.Settings();
        //<GS-08122010> private T7.Flasher.T7Flasher flash = null;
        //<GS-08122010> private CANUSBDevice canUsbDevice = null;
        private T7.CAN.ICANDevice canUsbDevice = null;  // common base class
        private T7.Flasher.IFlasher flash = null;       // common base class
        bool useCombiAdapter = false;
        bool autoRead = false;
        bool autoWrite = false;
        string fileToProcess = string.Empty;
        private KWPCANDevice kwpCanDevice = null;
        private KWPHandler kwpHandler = null;
        private bool m_connectedToECU = false;

        public Form1(string[] args)
        {
            InitializeComponent();
            m_DelegateUpdateStatus = new DelegateUpdateStatus(updateStatusInBox);

            try
            {
                MM_BeginPeriod(1);
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to set thread high prio");
            }
            useCombiAdapter = set.UseCombiAdapter;

            if (args.Length > 0)
            {
                if (Convert.ToInt32(args.GetValue(0)) == 1) useCombiAdapter = true;
                else useCombiAdapter = false;
                if (args.Length > 1)
                {
                    if (Convert.ToInt32(args.GetValue(1)) == 1) autoRead = true;
                    else autoRead = false;
                    if (Convert.ToInt32(args.GetValue(1)) == 2) autoWrite = true;
                    else autoWrite = false;
                }
                if (args.Length > 2)
                {
                    fileToProcess = (string)args.GetValue(2);
                }
            }


            //TODO: Expand for ELM327 support
            // and EasySync support
            if (useCombiAdapter)
            {
                // init combi adapter library
                canUsbDevice = new LPCCANDevice_T7();
                button3.Enabled = false;
            }
            else
            {
                canUsbDevice = new CANUSBDevice();
                kwpCanDevice = new KWPCANDevice();
            }
            

        }

        private void updateStatusInBox(string logItem)
        {
            AddToLog(logItem);
        }

        private void UpdateFlashStatus(string logItem)
        {
            try
            {
                this.Invoke(m_DelegateUpdateStatus,logItem);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }


        private void AddToCanTrace(string line)
        {
            //TODO: Implement
            if (checkBox1.Checked)
            {
                DateTime dtnow = DateTime.Now;
                using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\CanTrace.txt", true))
                {
                    sw.WriteLine(dtnow.ToString("dd/MM/yyyy HH:mm:ss") + " - " + line);
                }
            }
        }

        private void AddToLog(string line)
        {
            AddToCanTrace(line);
            try
            {
                listBox1.Items.Add(line);
                while (listBox1.Items.Count > 3000) listBox1.Items.RemoveAt(0);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                Application.DoEvents();
            }
            catch (Exception E)
            {

            }
        }

        private bool CheckCANConnectivity()
        {
            // write log information to "CanTrace.txt"
            if (flash == null)
            {
                AddToCanTrace("Initializing CANbus interface");
                System.Windows.Forms.Application.DoEvents();
                AddToLog("Initializing CAN interface, please stand by...");
                AddToCanTrace("Creating new connection to CANUSB device!");

                if (useCombiAdapter)
                {
                    // connect to adapter
                    LPCCANDevice_T7 lpc = (LPCCANDevice_T7)this.canUsbDevice;
                    if (!lpc.connect()) return false;
                    //<GS-09122010>
                   // kwpCanDevice.setCANDevice(canUsbDevice);
                   // KWPHandler.setKWPDevice(kwpCanDevice);
                    //<GS-09122010>
                    // get flasher object
                    Console.WriteLine("Created combiadpater flasher object");
                    this.flash = lpc.createFlasher();
                    flash.EnableCanLog = checkBox1.Checked;

                    AddToCanTrace("T7CombiFlasher object created");
                    AddToLog("CombiAdapter ready");
                }
                else
                {

                    //flash = new T7.Flasher.T7Flasher();
                    //AddToCanTrace("Object T7.Flasher.T7Flasher created");
                    kwpCanDevice.setCANDevice(canUsbDevice);
                    kwpCanDevice.EnableCanLog = checkBox1.Checked;
                    KWPHandler.setKWPDevice(kwpCanDevice);
                    if (checkBox1.Checked)
                    {
                        KWPHandler.startLogging();
                    }
                    kwpHandler = KWPHandler.getInstance();
                    try
                    {
                        T7.Flasher.T7Flasher.setKWPHandler(kwpHandler);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                        AddToCanTrace("Failed to set flasher object to KWPHandler");

                    }
                    flash = T7.Flasher.T7Flasher.getInstance();
                    flash.onStatusChanged += new T7.Flasher.IFlasher.StatusChanged(flash_onStatusChanged);
                    //AddToLog("Set callback event on flasher");
                    flash.EnableCanLog = checkBox1.Checked;

                    if (kwpHandler.openDevice())
                    {
                        AddToLog("Canbus channel opened");
                    }
                    else
                    {
                        AddToLog("Unable to open canbus channel");
                        kwpHandler.closeDevice();
                        return false;
                    }
                    if (kwpHandler.startSession())
                    {
                        AddToLog("Session started");
                    }
                    else
                    {
                        AddToLog("Unable to start session");
                        kwpHandler.closeDevice();
                        return false;
                    }
                }

            }
            m_connectedToECU = true;
            return m_connectedToECU;
        }

        void flash_onStatusChanged(object sender, T7.Flasher.IFlasher.StatusEventArgs e)
        {
            UpdateFlashStatus(e.Info);
        }

        private bool CheckFlashStatus()
        {
            AddToCanTrace("Start CheckFlashStatus");
            T7.Flasher.T7Flasher.FlashStatus stat = flash.getStatus();
            AddToCanTrace("Status retrieved");
            switch (stat)
            {
                case T7.Flasher.T7Flasher.FlashStatus.Completed:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.Completed");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.DoinNuthin:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.DoinNuthin");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.EraseError:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.EraseError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Eraseing:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.Eraseing");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.NoSequrityAccess:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.NoSequrityAccess");
                    flash.stopFlasher();
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.NoSuchFile:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.NoSuchFile");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.ReadError:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.ReadError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Reading:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.Reading");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.WriteError:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.WriteError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Writing:
                    AddToCanTrace("Status = T7.Flasher.T7Flasher.FlashStatus.Writing");
                    break;
                default:
                    AddToCanTrace("Status = " + stat.ToString());
                    break;
            }
            bool retval = true;
            if (stat == T7.Flasher.T7Flasher.FlashStatus.Eraseing || stat == T7.Flasher.T7Flasher.FlashStatus.Reading || stat == T7.Flasher.T7Flasher.FlashStatus.Writing)
            {
                retval = false;
            }
            return retval;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_connectedToECU)
            {
                if (checkBox1.Checked)
                {
                    KWPHandler.startLogging();
                }
                if (!tmrReadProcessChecker.Enabled)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Bin files|*.bin";
                    ofd.Title = "Select binary file to flash...";
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fi = new FileInfo(ofd.FileName);
                        if (fi.Length == 512 * 1024)
                        {
                            // check reading status periodically
                            AddToCanTrace("Starting flash procedure, checking flashing process status");
                            if (CheckFlashStatus())
                            {
                                //frmFlasherProgress = new frmProgress();
                                //frmFlasherProgress.onCancelOperation += new frmProgress.CancelEvent(frmFlasherProgress_onCancelOperation);
                                //frmFlasherProgress.Show();
                                //frmFlasherProgress.SetProgress("Starting ECU flash...");
                                tmrWriteProcessChecker.Enabled = true;
                                AddToLog("Flashing: " + ofd.FileName);
                                AddToCanTrace("Calling flash.writeFlash with filename: " + ofd.FileName);
                                flash.writeFlash(ofd.FileName);
                                //DisableCANInteractionButtons();
                            }
                        }
                        else
                        {
                            MessageBox.Show("File has incorrect length, should be 512 kB exactly!");
                        }
                    }
                }

            }
            else
            {
                AddToLog("Connection to ECU not established, please restart the application");
            }
        }

        private void tmrReadProcessChecker_Tick(object sender, EventArgs e)
        {
            float numberkb = (float)flash.getNrOfBytesRead() / 1024F;
            //frmFlasherProgress.SetProgress("Downloaded " + numberkb.ToString("F1") + " of 512 kB");
            label1.Text = "Downloaded " + numberkb.ToString("F1") + " of 512 kB";
            int percentage = ((int)numberkb * 100) / 512;
            progressBar1.Value = percentage;
            //frmFlasherProgress.SetProgressPercentage(percentage);
            if (flash.getStatus() == T7.Flasher.T7Flasher.FlashStatus.Completed)
            {
                flash.stopFlasher();
                tmrReadProcessChecker.Enabled = false;
                //EnableCANInteractionButtons();
                AddToLog("Finished download of flash");
                if (autoRead) this.Close();
            }
        }

        private void tmrWriteProcessChecker_Tick(object sender, EventArgs e)
        {
            float numberkb = (float)flash.getNrOfBytesRead() / 1024F;
            label1.Text = "Flashed " + numberkb.ToString("F1") + " of 512 kB";
            int percentage = ((int)numberkb * 100) / 512;
            //frmFlasherProgress.SetProgressPercentage(percentage);
            progressBar1.Value = percentage;
            //AddToCanTrace("Flashed " + numberkb.ToString("F1") + " of 512 kB");
            T7.Flasher.T7Flasher.FlashStatus stat = flash.getStatus();
            switch (stat)
            {
                case T7.Flasher.T7Flasher.FlashStatus.Completed:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: Completed flashing procedure");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.DoinNuthin:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: DoinNuthin");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.EraseError:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: EraseError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Eraseing:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: Eraseing");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.NoSequrityAccess:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: NoSequrityAccess");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.NoSuchFile:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: NoSuchFile");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.ReadError:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: ReadError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Reading:
                    //AddToCanTrace("tmrWriteProcessChecker_Tick: Reading");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.WriteError:
                    AddToCanTrace("tmrWriteProcessChecker_Tick: WriteError");
                    break;
                case T7.Flasher.T7Flasher.FlashStatus.Writing:
                    //AddToCanTrace("tmrWriteProcessChecker_Tick: Writing");
                    break;
            }

            if (stat == T7.Flasher.T7Flasher.FlashStatus.Completed)
            {
                flash.stopFlasher();
                tmrWriteProcessChecker.Enabled = false;
                //EnableCANInteractionButtons();
                //MessageBox.Show("Done downloading binary file!");
                AddToLog("Finished flash session");
                //KWPHandler.getInstance().ResetECU();
                //AddToLog("ECU is reset");
                if (autoWrite)
                {
                    Thread.Sleep(100);
                    this.Close();
                }
            }
            else if (stat == T7.Flasher.T7Flasher.FlashStatus.NoSequrityAccess)
            {
                flash.stopFlasher();
                tmrWriteProcessChecker.Enabled = false;
                AddToLog("No security access granted");
                if (autoWrite) this.Close();
            }
            else if (stat == T7.Flasher.T7Flasher.FlashStatus.EraseError)
            {
                flash.stopFlasher();
                tmrWriteProcessChecker.Enabled = false;
                AddToLog("An erase error occured");
                if (autoWrite) this.Close();
            }
            else if (stat == T7.Flasher.T7Flasher.FlashStatus.NoSuchFile)
            {
                flash.stopFlasher();
                tmrWriteProcessChecker.Enabled = false;
                AddToLog("File not found");
                if (autoWrite) this.Close();
            }
            else if (stat == T7.Flasher.T7Flasher.FlashStatus.WriteError)
            {
                flash.stopFlasher();
                tmrWriteProcessChecker.Enabled = false;
                AddToLog("A write error occured, please retry to flash without cutting power to the ECU");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (m_connectedToECU)
            {
                if (checkBox1.Checked)
                {
                    KWPHandler.startLogging();
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Bin files|*.bin";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // check reading status periodically
                    if (sfd.FileName != string.Empty)
                    {
                        if (Path.GetFileName(sfd.FileName) != string.Empty)
                        {
                            if (CheckFlashStatus())
                            {
                               // frmFlasherProgress.SetProgress("Starting flash download...");
                                AddToLog("Starting download of flash");
                                tmrReadProcessChecker.Enabled = true;
                                Console.WriteLine("Start reading flash...");
                                flash.readFlash(sfd.FileName);
                                //DisableCANInteractionButtons();
                            }
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MM_EndPeriod(1);
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to set thread high prio");
            }
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string vin;
            string immo;
            string engineType;
            string swVersion;
            if (m_connectedToECU)
            {
                if (useCombiAdapter)
                {
                    // begin comms session
                    T7.Flasher.T7CombiFlasher cf =
                        (T7.Flasher.T7CombiFlasher)this.flash;
                    cf.beginSession();

                    // NB: does not work, but it's a firmware problem
                    //vin = cf.getHeaderString(T7.Flasher.T7HeaderField.vin);
                    //engineType = cf.getHeaderString(T7.Flasher.T7HeaderField.engtype);
                    //swVersion = cf.getHeaderString(T7.Flasher.T7HeaderField.swversion);

                    // end session
                    cf.endSession();
                }
                else
                {

                    if (checkBox1.Checked)
                    {
                        KWPHandler.startLogging();
                    }
                    KWPResult res = kwpHandler.getVIN(out vin);
                    if (res == KWPResult.OK)
                        AddToLog("VIN: " + vin);
                    else if (res == KWPResult.DeviceNotConnected)
                        AddToLog("VIN: not connected");
                    else
                        AddToLog("VIN: timeout");
                    res = kwpHandler.getImmo(out immo);
                    if (res == KWPResult.OK)
                        AddToLog("Immo: " + immo);
                    res = kwpHandler.getEngineType(out engineType);
                    if (res == KWPResult.OK)
                        AddToLog("Engine type: :" + engineType);
                    res = kwpHandler.getSwVersion(out swVersion);
                    if (res == KWPResult.OK)
                        AddToLog("Software version: " + swVersion);
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            AddToLog("Application version: " + Application.ProductVersion.ToString());
            Thread.Sleep(100);
            if (!CheckCANConnectivity())
            {
                AddToLog("Please restart the application");
            }
            
            Thread.Sleep(100);
            button3_Click(this, EventArgs.Empty);
            Thread.Sleep(100);
            Application.DoEvents();

            if (m_connectedToECU)
            {
                if (autoRead)
                {
                    AddToLog("Waiting ... ");
                    Thread.Sleep(3000);
                    if (CheckFlashStatus())
                    {
                        AddToLog("Starting download of flash");
                        tmrReadProcessChecker.Enabled = true;
                        flash.readFlash(fileToProcess);
                    }
                }
                else if (autoWrite)
                {
                    AddToLog("Waiting ... ");
                    Thread.Sleep(3000);

                    FileInfo fi = new FileInfo(fileToProcess);
                    if (fi.Length == 512 * 1024)
                    {
                        // check reading status periodically
                        AddToCanTrace("Starting flash procedure, checking flashing process status");
                        if (CheckFlashStatus())
                        {
                            tmrWriteProcessChecker.Enabled = true;
                            AddToLog("Flashing: " + fileToProcess);
                            AddToCanTrace("Calling flash.writeFlash with filename: " + fileToProcess);
                            flash.writeFlash(fileToProcess);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File has incorrect length, should be 512 kB exactly!");
                    }
                }
            }
        }
    }
}