using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CommonSuite;

namespace T7
{
    public enum AutoStartSign : int
    {
        Equals,
        IsGreaterThan,
        IsSmallerThan
    }

    public partial class frmAutoLoggingSettings : DevExpress.XtraEditors.XtraForm
    {
        private SymbolCollection m_symbols;

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set
            {
                m_symbols = value;
                lookUpEdit1.Properties.DataSource = m_symbols;
                lookUpEdit1.Properties.ValueMember = "Varname";
                lookUpEdit1.Properties.DisplayMember = "Varname";
                lookUpEdit2.Properties.DataSource = m_symbols;
                lookUpEdit2.Properties.ValueMember = "Varname";
                lookUpEdit2.Properties.DisplayMember = "Varname";
            }
        }

        public frmAutoLoggingSettings()
        {
            InitializeComponent();
            comboBoxEdit1.SelectedIndex = 1;
            comboBoxEdit2.SelectedIndex = 2;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public bool AutoLoggingEnabled
        {
            get
            {
                return checkEdit1.Checked;
            }
            set
            {
                checkEdit1.Checked = value;
            }
        }

        public string TriggerStartSymbol
        {
            get
            {
                return lookUpEdit1.EditValue.ToString();
            }
            set
            {
                lookUpEdit1.EditValue = value;
            }
        }

        public string TriggerStopSymbol
        {
            get
            {
                return lookUpEdit2.EditValue.ToString();
            }
            set
            {
                lookUpEdit2.EditValue = value;
            }
        }



        public AutoStartSign TriggerStartSign
        {
            get
            {
                return (AutoStartSign)comboBoxEdit1.SelectedIndex;
            }
            set
            {
                comboBoxEdit1.SelectedIndex = (int)value;
            }
        }

        public AutoStartSign TriggerStopSign
        {
            get
            {
                return (AutoStartSign)comboBoxEdit2.SelectedIndex;
            }
            set
            {
                comboBoxEdit2.SelectedIndex = (int)value;
            }
        }

        private double ConvertToDouble(string v)
        {
            double d = 0;
            if (v == "") return d;
            string vs = "";
            vs = v.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator, System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Double.TryParse(vs, out d);
            return d;
        }

        public double TriggerStartValue
        {
            get
            {
                return ConvertToDouble(textEdit1.Text);
            }
            set
            {
                textEdit1.Text = value.ToString();
            }
        }

        public double TriggerStopValue
        {
            get
            {
                return ConvertToDouble(textEdit2.Text);
            }
            set
            {
                textEdit2.Text = value.ToString();
            }
        }

    }
}