using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Controls
{
    public partial class Measurement : DevExpress.XtraEditors.XtraUserControl
    {

        private SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }
        private string _symbolToDisplay = string.Empty;

        public string SymbolToDisplay
        {
            get { return _symbolToDisplay; }
            set { _symbolToDisplay = value; }
        }

        private int m_decimals = 0;
        private int m_numberdigits = 3;

        public int Numberdigits
        {
            get { return m_numberdigits; }
            set { m_numberdigits = value; }
        }
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

        public Color DigitColor
        {
            get
            {
                return digitalDisplayControl1.DigitColor;
            }
            set
            {
                if (digitalDisplayControl1.DigitColor != value)
                {
                    digitalDisplayControl1.DigitColor = value;
                    Invalidate();
                }
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

        public void SetColor(Color c)
        {
            if (digitalDisplayControl1.DigitColor != c)
            {
                digitalDisplayControl1.DigitColor = c;
                Invalidate();
            }
        }

        private void SetValue(float value)
        {
            if (m_value != value)
            {
                string text = value.ToString(formatstring);
                if (text.StartsWith("-"))
                {
                    text = text.Replace("-","");
                    if (m_numberdigits > 1)
                    {
                        text = text.PadLeft(m_numberdigits - 1, '0');
                    }
                    text = "-" + text;
                }
                else
                {
                    text = text.PadLeft(m_numberdigits, '0');
                }
                digitalDisplayControl1.DigitText = text;
                m_value = value;
                Invalidate();
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

        private void SetText(string text)
        {
            if (labelControl1.Text != text)
            {
                labelControl1.Text = text;
            }
        }

        public void SizeControl()
        {
            // set digitalDisplay to the middle
            int xpos = (panel1.Bounds.Width - digitalDisplayControl1.Width) / 2;
            digitalDisplayControl1.Left = xpos;
            int ypos = (panel1.Bounds.Height - digitalDisplayControl1.Height) / 2;
            digitalDisplayControl1.Top = ypos;
            // size
            int width = panel1.Bounds.Width - 10;
            if (width > 200) width = 200;
            int height = panel1.Bounds.Height - 10;
            if (height > 150) height = 150;
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;
            digitalDisplayControl1.Width = width;
            digitalDisplayControl1.Height = height;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            SizeControl();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // show all available symbols (from the monitored list)

            chooseSymbolToolStripMenuItem.DropDownItems.Clear();
            foreach (SymbolHelper sh in m_symbols)
            {
                ToolStripMenuItem newitem = new ToolStripMenuItem(sh.Varname);
                newitem.Click += new EventHandler(newitem_Click);
                chooseSymbolToolStripMenuItem.DropDownItems.Add(newitem);
            }

        }

        private void chooseSymbolToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            
        }

        void newitem_Click(object sender, EventArgs e)
        {
            // select this toolstripitemmenus symbol
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                Console.WriteLine("Chosen: " + mi.Text);
                MeasurementText = mi.Text;
            }
        }
    }
}
