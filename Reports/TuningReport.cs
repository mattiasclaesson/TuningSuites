using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace CommonSuite
{
    public partial class TuningReport : DevExpress.XtraReports.UI.XtraReport
    {
        public TuningReport()
        {
            InitializeComponent();
        }

        public string ReportTitle
        {
            set
            {
                xrLabel2.Text = value;
            }
        }

        public void CreateReport()
        {
            if (xrLabel1.DataBindings.Count == 0)
            {
                xrLabel1.DataBindings.Add(new XRBinding("Text", this.DataSource, "Description"));
            }
        }

    }
}
