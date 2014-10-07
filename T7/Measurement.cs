using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CommonSuite
{
    public partial class Measurement : DevExpress.XtraEditors.XtraUserControl
    {
        private int m_decimals = 0;
        private float m_value = 0;
        string formatstring = "F0";

        public Measurement()
        {
            InitializeComponent();
        }

        public void SetNumberOfDecimals(int decimals)
        {
            if (decimals != m_decimals)
            {
                m_decimals = decimals;
                formatstring = "F" + m_decimals.ToString();
            }
        }

        public int NumberOfDecimals
        {
            get
            {
                return m_decimals;
            }
            set
            {
                SetNumberOfDecimals(value);
            }
        }

        public float Value
        {
            get
            {
                return m_value;
            }
            set
            {
                SetValue(value);
            }
        }

        private void SetValue(float value)
        {
            if (m_value != value)
            {
                digitalDisplayControl1.DigitText = value.ToString(formatstring);
                m_value = value;
            }
        }

        public string MeasurementText
        {
            get
            {
                return labelControl1.Text;
            }
            set
            {
                SetText(value);
            }
        }

        public Color SetDigitColor
        {
            get
            {
                return digitalDisplayControl1.DigitColor;
            }
            set
            {
                digitalDisplayControl1.DigitColor = value;
            }
        }

        public Color SetLabelColor
        {
            get
            {
                return labelControl1.ForeColor;
            }
            set
            {
                labelControl1.ForeColor = value;
            }
        }

        public Color SetBackColor
        {
            get
            {
                return digitalDisplayControl1.BackColor;
            }
            set
            {
                digitalDisplayControl1.BackColor = value;
                labelControl1.BackColor = value;
                this.BackColor = value;
                base.BackColor = value;
                //this.Appearance.BackColor = value;
                //this.Appearance.BackColor2 = value;
                //this.Appearance.BorderColor = value;
                //this.Appearance.Options.UseBackColor = true;
            }
        }

        private void SetText(string text)
        {
            if (labelControl1.Text != text)
            {
                labelControl1.Text = text;
            }
        }
    }
}
