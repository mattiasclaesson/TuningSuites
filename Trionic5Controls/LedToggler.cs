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
    public partial class LedToggler : DevExpress.XtraEditors.XtraUserControl
    {
        bool _checked = false;
        int _bytenumber = 0;

        public int Bytenumber
        {
            get { return _bytenumber; }
            set { _bytenumber = value; }
        }
        int _bitnumber = 0;

        public int Bitnumber
        {
            get { return _bitnumber; }
            set { _bitnumber = value; }
        }


        public LedToggler()
        {
            InitializeComponent();
        }

        public string Description
        {
            get
            {
                return labelControl1.Text;
            }
            set
            {
                labelControl1.Text = value;
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    if (_checked)
                    {
                        pictureOn.Visible = true;
                        pictureOff.Visible = false;
                    }
                    else
                    {
                        pictureOff.Visible = true;
                        pictureOn.Visible = false;
                    }
                }
            }
        }
    }
}
