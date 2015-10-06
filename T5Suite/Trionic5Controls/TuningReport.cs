using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace Trionic5Controls
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

        public void SetDataSource(DataTable dt)
        {
            this.DataSource = dt;
        }

        public void ShowReportPreview(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel)
        {
            ShowPreview(lookAndFeel);
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
