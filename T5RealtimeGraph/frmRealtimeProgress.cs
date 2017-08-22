using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RealtimeGraph
{
    public partial class frmRealtimeProgress : Form
    {
        public frmRealtimeProgress()
        {
            InitializeComponent();
        }
        public void SetProgressPercentage(int percentage)
        {
            progressBarControl1.Position = percentage;
            Application.DoEvents();
        }
    }
}