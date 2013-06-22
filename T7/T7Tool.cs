using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using T7Tool.KWP;
using T7Tool.CAN;
using T7Tool.Flasher;
using T7Tool.Forms;
using System.Threading;

namespace T7Tool
{
    public partial class T7Tool : Form
    {
        private Forms.RealTimeSymbolForm realTimeSymbolForm;
        private T7FileHeader t7InfoHeader = null;
        private CANUSBDevice canUsbDevice = null;
        private ELM327Device m_elm327Device = null;
        private KWPCANDevice kwpCanDevice = null;
        private KWPHandler kwpHandler = null;
        private T7Flasher m_t7Flasher = null;
        private string m_fileName;
        private TimerCallback timerDelegate;
        private System.Threading.Timer stateTimer;
        private bool m_connectedToECU = false;

        public T7Tool()
        {
            InitializeComponent();

            t7InfoHeader = new T7FileHeader();
            canUsbDevice = new CANUSBDevice();
            kwpCanDevice = new KWPCANDevice();

            timerDelegate = new TimerCallback(this.flasherInfo);
            flashDownLoadButton.Select();
            flashStartButton.Enabled = false;
            kwpDeviceComboBox.SelectedItem = "Lawicel CANUSB";
        }



        private void chassisIDTextBox_TextChanged(object sender, EventArgs e)
        {
            t7InfoHeader.setChassisID(chassisIDTextBox.Text);
        }

     

        private void openFileDialog_FileOk_1(object sender, CancelEventArgs e)
        {
            openFile();             
        }

        

        private void getIDButton_Click(object sender, EventArgs e)
        {
            getIDFileDialog.ShowDialog();
        }

  

        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }


        private void imobilizerTextBox_TextChanged(object sender, EventArgs e)
        {
            t7InfoHeader.setImmobilizerID(imobilizerTextBox.Text);
        }

        private void softwareVersionTextBox_TextChanged(object sender, EventArgs e)
        {
            t7InfoHeader.setSoftwareVersion(softwareVersionTextBox.Text);
        }

        private void carDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            t7InfoHeader.setCarDescription(carDescriptionTextBox.Text);
        }

        private void saveFileButton_Click(object sender, EventArgs e)
        {
            t7InfoHeader.save(m_fileName);
        }

        private void getIDFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = getIDFileDialog.FileName;
            T7FileHeader fileHeader = new T7FileHeader();
            fileHeader.init(fileName);
            chassisIDTextBox.Text = fileHeader.getChassisID();
            imobilizerTextBox.Text = fileHeader.getImmobilizerID();

        }


        private void saveFileAsbButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = saveFileDialog.FileName;
            File.Copy(m_fileName, fileName, true);
            t7InfoHeader.save(fileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitApplication();
        }

        private void fixChecksumButton_Click(object sender, EventArgs e)
        {
            ChecksumHandler csHandler = new ChecksumHandler();
            int fwLength = t7InfoHeader.getFWLength();
            int calculatedFWChecksum = csHandler.calculateFWChecksum(m_fileName);
            csHandler.setFWChecksum(m_fileName, calculatedFWChecksum);

            uint calculatedF2Checksum = csHandler.calculateF2Checksum(m_fileName, 0, fwLength);
            int calculatedFBChecksum = csHandler.calculateFBChecksum(m_fileName, 0, fwLength);
            t7InfoHeader.setChecksumF2((int)calculatedF2Checksum);
            t7InfoHeader.setChecksumFB(calculatedFBChecksum);

            t7InfoHeader.save(m_fileName);
            

            openFile();
        }

        private void openFile()
        {
            m_fileName = openFileDialog.FileName;
            fileNameLabel.Text = m_fileName;
            t7InfoHeader.init(m_fileName);
            chassisIDTextBox.Text = t7InfoHeader.getChassisID();
            softwareVersionTextBox.Text = t7InfoHeader.getSoftwareVersion();
            carDescriptionTextBox.Text = t7InfoHeader.getCarDescription();
            imobilizerTextBox.Text = t7InfoHeader.getImmobilizerID();
            if (t7InfoHeader.getChecksumF2() != 0)
            {
                checksumF2TextBox.Enabled = true;
                checksumF2TextBox.Text = "0x" + t7InfoHeader.getChecksumF2().ToString("X");
            }
            else
            {
                checksumF2TextBox.Enabled = false;
                checksumF2TextBox.Text = "";
            }
            checksumFBTextBox.Text = "0x" + t7InfoHeader.getChecksumFB().ToString("X");
            firmwareLengthTextBox.Text = "0x" + t7InfoHeader.getFWLength().ToString("X");

            ChecksumHandler csHandler = new ChecksumHandler();
            int fwChecksum = csHandler.getFWChecksum(m_fileName);
            int fwLength = t7InfoHeader.getFWLength();
            uint f2Checksum = (uint)t7InfoHeader.getChecksumF2();
            int fbChecksum = t7InfoHeader.getChecksumFB();
            checksumFWTextBox.Text = "0x" + fwChecksum.ToString("X");

            int calculatedFWChecksum = csHandler.calculateFWChecksum(m_fileName);
            uint calculatedF2Checksum = csHandler.calculateF2Checksum(m_fileName, 0, fwLength);
            int calculatedFBChecksum = csHandler.calculateFBChecksum(m_fileName, 0, fwLength);

            if (fwChecksum == calculatedFWChecksum)
                checksumFWTextBox.BackColor = Color.LightGreen;
            else
                checksumFWTextBox.BackColor = Color.Salmon;

            if (t7InfoHeader.getChecksumF2() == 0)
                checksumF2TextBox.BackColor = Color.Empty;
            else if (calculatedF2Checksum == f2Checksum)
                checksumF2TextBox.BackColor = Color.LightGreen;
            else
                checksumF2TextBox.BackColor = Color.Salmon;

            if (t7InfoHeader.getChecksumFB() == 0)
                checksumFBTextBox.BackColor = Color.Empty;
            else if (calculatedFBChecksum == fbChecksum)
                checksumFBTextBox.BackColor = Color.LightGreen;
            else
                checksumFBTextBox.BackColor = Color.Salmon;
        }

        private void aboutT7ToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void fileInfoPage_Click(object sender, EventArgs e)
        {


        }
            

        public void flasherInfo(Object stateInfo)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            flashNrOfBytesLabel.Text = "" + m_t7Flasher.getNrOfBytesRead() / 1024;
            switch (m_t7Flasher.getStatus())
            {
                case T7Flasher.FlashStatus.Completed:
                    {
                        flashStartButton.Text = "Start";
                        flashStatusLabel.Text = "Completed";
                        flashFileNameLabel.Text = "";
                        flashStartButton.Enabled = true;
                        break;
                    } 
                case T7Flasher.FlashStatus.Reading:
                    {
                        flashStartButton.Text = "Stop";
                        flashStatusLabel.Text = "Reading";
                        break;
                    }
                case T7Flasher.FlashStatus.Writing:
                    {
                        flashStatusLabel.Text = "Writing";
                        flashStartButton.Enabled = false;
                        break;
                    }
                case T7Flasher.FlashStatus.Eraseing:
                    {
                        flashStatusLabel.Text = "Eraseing";
                        flashStartButton.Enabled = false;
                        break;
                    }
                case T7Flasher.FlashStatus.NoSequrityAccess: flashStatusLabel.Text = "No security access"; break;
                case T7Flasher.FlashStatus.DoinNuthin:
                    {
                        flashStatusLabel.Text = "Not started";
                        flashStartButton.Text = "Start";
                        break;
                    }
                case T7Flasher.FlashStatus.NoSuchFile: flashStatusLabel.Text = "No such file"; break;
                case T7Flasher.FlashStatus.EraseError:
                    {
                        flashStartButton.Enabled = true;
                        flashStatusLabel.Text = "Erase error";
                        break;
                    }
                case T7Flasher.FlashStatus.WriteError:
                    {
                        flashStatusLabel.Text = "Write error";
                        flashStartButton.Enabled = true;
                        break;
                    }
            }
        }

        private void kwpDeviceOpenButton_Click(object sender, EventArgs e)
        {

            kwpDeviceConnectionStatus.Text = "Connecting";
            if (kwpDeviceComboBox.SelectedItem.ToString() == "Lawicel CANUSB")
            {
                kwpCanDevice.setCANDevice(canUsbDevice);
                KWPHandler.setKWPDevice(kwpCanDevice);
                
            }
            else if (kwpDeviceComboBox.SelectedItem.ToString() == "ELM327 1.2")
            {
                if (m_elm327Device == null)
                    m_elm327Device = new ELM327Device();
                KWPHandler.setKWPDevice(m_elm327Device);

            }

            kwpHandler = KWPHandler.getInstance();
            T7Flasher.setKWPHandler(kwpHandler);
            m_t7Flasher = T7Flasher.getInstance();

            if (kwpHandler.openDevice())
                kwpDeviceConnectionStatus.Text = "Open";
            else
            {
                kwpDeviceConnectionStatus.Text = "Unable to open";
                kwpHandler.closeDevice();
                return;
            }
            if (kwpHandler.startSession())
                kwpDeviceConnectionStatus.Text = "Connected";
            else
            {
                kwpDeviceConnectionStatus.Text = "Initialize error";
                kwpHandler.closeDevice();
                return;
            }
            m_connectedToECU = true;
            stateTimer = new System.Threading.Timer(timerDelegate, new Object(), 0, 250);
            populateECUTab();

        }

        private void populateECUTab()
        {
            string vin;
            string immo;
            string engineType;
            string swVersion;
            KWPResult res = kwpHandler.getVIN(out vin);
            if (res == KWPResult.OK)
                ecuVINTextBox.Text = vin;
            else if (res == KWPResult.DeviceNotConnected)
                ecuVINTextBox.Text = "Not connected";
            else
                ecuVINTextBox.Text = "Timeout";

            res = kwpHandler.getImmo(out immo);
            if (res == KWPResult.OK)
                ecuImmoTextBox.Text = immo;
            res = kwpHandler.getEngineType(out engineType);
            if (res == KWPResult.OK)
                ecuCarDescTextBox.Text = engineType;
            res = kwpHandler.getSwVersion(out swVersion);
            if (res == KWPResult.OK)
                ecuSWVerTextBox.Text = swVersion;
            flashStartButton.Enabled = true;
        }


        private void flashStartButton_Click(object sender, EventArgs e)
        {
            if (flashStartButton.Text == "Stop")
            {
                m_t7Flasher.stopFlasher();
                flashFileNameLabel.Text = "";
                flashStartButton.Text = "Start";
            }
            else
            {
                if (flashDownLoadButton.Checked)
                    flashFileDialog.OverwritePrompt = true;
                else
                    flashFileDialog.OverwritePrompt = false;
                flashFileDialog.FileName = ecuSWVerTextBox.Text + ".bin";
                flashFileDialog.ShowDialog();
            }
        }

        private void flashFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (flashDownLoadButton.Checked)
            {
                m_t7Flasher.readFlash(flashFileDialog.FileName);
                flashFileNameLabel.Text = Path.GetFileName(flashFileDialog.FileName);
            }
            else
            {
                m_t7Flasher.writeFlash(flashFileDialog.FileName);
                flashFileNameLabel.Text = Path.GetFileName(flashFileDialog.FileName);
            }

        }

        private void T7Tool_Load(object sender, EventArgs e)
        {

        }


        private void T7Tool_Disposed(object sender, EventArgs e)
        {
            exitApplication();
        }


        private void exitApplication()
        {
            KWPHandler.stopLogging();
            kwpHandler.closeDevice();
            t7InfoHeader = null;
            canUsbDevice = null;
            kwpCanDevice = null;
            kwpHandler = null;
            m_t7Flasher = null;
            m_fileName = null;
            timerDelegate = null;
            stateTimer = null;
            Application.Exit();
            Environment.Exit(0);
        }

        private void DownloadRAMButton_Click(object sender, EventArgs e)
        {

        }


        private void downLoadSymbolTableFileDialog_FileOk(object sender, CancelEventArgs e)
        {

           // m_t7Flasher.readMemory(downLoadSymbolTableFileDialog.FileName, (uint)kwpHandler.getSymbolTableOffset(), );
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void kWPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (kWPToolStripMenuItem.Checked)
                KWPHandler.startLogging();
            else
                KWPHandler.stopLogging();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void fileInfoPage_Click_1(object sender, EventArgs e)
        {

        }

        private void eCUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_connectedToECU)
            {
                InfoFormOK infoForm = new InfoFormOK("Unable to perform. ECU not connected");
                infoForm.Show();
                return;
            }
            if(realTimeSymbolForm == null)
                realTimeSymbolForm = new RealTimeSymbolForm();
            realTimeSymbolForm.Show();
        }

        private void kwpDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}