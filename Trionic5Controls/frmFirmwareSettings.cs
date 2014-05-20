using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Trionic5Controls
{
    public partial class frmFirmwareSettings : DevExpress.XtraEditors.XtraForm
    {
        public frmFirmwareSettings()
        {
            InitializeComponent();
        }

        private object _objectinView;

        public object ObjectinView
        {
            get { return propertyGrid1.SelectedObject; }
            set
            {
                _objectinView = value;
                propertyGrid1.SelectedObject = _objectinView;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}