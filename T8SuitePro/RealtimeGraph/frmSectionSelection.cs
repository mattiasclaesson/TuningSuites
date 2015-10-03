using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace RealtimeGraph
{
    public partial class frmSectionSelection : DevExpress.XtraEditors.XtraForm
    {
        public frmSectionSelection()
        {
            InitializeComponent();
        }

        private LogSectionCollection _logSections;

        public LogSectionCollection LogSections
        {
            get { return _logSections; }
            set
            {
                _logSections = value;
                listBoxControl1.DataSource = _logSections;
            }
        }

        private DateTime _selectedStart;

        public DateTime SelectedStart
        {
            get { return _selectedStart; }
            set { _selectedStart = value; }
        }
        private DateTime _selectedEnd;

        public DateTime SelectedEnd
        {
            get { return _selectedEnd; }
            set { _selectedEnd = value; }
        }

        private bool _valid = false;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        private void CloseAndReturnLogSection()
        {
            if (listBoxControl1.SelectedItem != null)
            {
                if (listBoxControl1.SelectedItem is LogSection)
                {
                    LogSection ls = (LogSection)listBoxControl1.SelectedItem;
                    _selectedStart = ls.StartDateTime;
                    _selectedEnd = ls.EndDateTime;
                    _valid = true;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            CloseAndReturnLogSection();
        }

        private void listBoxControl1_DoubleClick(object sender, EventArgs e)
        {
            CloseAndReturnLogSection();
        }

    }
}