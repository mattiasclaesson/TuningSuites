using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProGauges
{
    public partial class CombiGauge : UserControl
    {
        public CombiGauge()
        {
            InitializeComponent();
        }

        private int m_NumberOfDecimals = 0;

        public int NumberOfDecimals
        {
            get { return m_NumberOfDecimals; }
            set { m_NumberOfDecimals = value; }
        }

        private float _value = 0;

        public float Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    smallLinearGauge1.Value = _value;
                    string strText = _value.ToString("F" + m_NumberOfDecimals.ToString()); 
                    digitalDisplayControl1.DigitText = strText;
                }
            }
        }
    }
}
