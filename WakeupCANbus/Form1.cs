using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WakeupCANbus
{
    public partial class Form1 : Form
    {
        private T5CANLib.T5CAN _tcan = null;
        private T5CANLib.CAN.CANUSBDevice _usbcandevice = null;
        private string _swversion = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void AddToLog(string item)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\wakeup.log", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " " + item);
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void DeleteFile()
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\wakeup.log")) File.Delete(Application.StartupPath + "\\wakeup.log");
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //DeleteFile();
            //AddToLog("Wakeup started");
            _tcan = new T5CANLib.T5CAN();
            //AddToLog("T5CAN created");
            _usbcandevice = new T5CANLib.CAN.CANUSBDevice();
            //AddToLog("CANUSBDevice created");
            _tcan.setCANDevice(_usbcandevice);
            //AddToLog("CANUSBDevice set");
            _tcan.openDevice(out _swversion);
            //AddToLog("CANUSBDevice open device called, exiting");
            Environment.Exit(0);
        }
    }
}