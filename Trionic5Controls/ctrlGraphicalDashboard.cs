using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Trionic5Controls
{
    public partial class ctrlGraphicalDashboard : DevExpress.XtraEditors.XtraUserControl
    {
        public ctrlGraphicalDashboard()
        {
            InitializeComponent();
        }

        public void SetBatteryIndicator(int state)
        {
            //stateIndicatorComponent1.StateIndex = state;
        }
    }
}
