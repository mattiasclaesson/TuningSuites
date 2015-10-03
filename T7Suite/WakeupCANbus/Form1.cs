using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WakeupCANbus
{
    public partial class Form1 : Form
    {
        private CANUSBDevice canUsbDevice = null;
        private KWPCANDevice kwpCanDevice = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                canUsbDevice = new CANUSBDevice();
                canUsbDevice.UseOnlyPBus = true;
                kwpCanDevice = new KWPCANDevice();
                kwpCanDevice.setCANDevice(canUsbDevice);
                KWPHandler.setKWPDevice(kwpCanDevice);
                if (KWPHandler.getInstance().openDevice())
                {
                    string m_swversion = string.Empty;
                    KWPHandler.getInstance().getSwVersionFromDR51(out m_swversion);
                    Console.WriteLine("SW: " + m_swversion);
                }
                KWPHandler.getInstance().closeDevice();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            Environment.Exit(0);
        }
    }
}
