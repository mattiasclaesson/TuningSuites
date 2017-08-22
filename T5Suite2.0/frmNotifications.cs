using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;

namespace T5Suite2
{
    public partial class frmNotifications : DevExpress.XtraEditors.XtraForm
    {
        T5AppSettings m_appSettings;

        SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set
            {
                m_symbols = value;
                lookUpEdit1.Properties.DataSource = m_symbols;
                lookUpEdit2.Properties.DataSource = m_symbols;
                lookUpEdit3.Properties.DataSource = m_symbols;
                lookUpEdit1.Properties.DisplayMember = "Varname";
                lookUpEdit1.Properties.ValueMember = "Varname";
                lookUpEdit2.Properties.DisplayMember = "Varname";
                lookUpEdit2.Properties.ValueMember = "Varname";
                lookUpEdit3.Properties.DisplayMember = "Varname";
                lookUpEdit3.Properties.ValueMember = "Varname";
            }
        }

        

        public T5AppSettings AppSettings
        {
            get { return m_appSettings; }
            set
            {
                m_appSettings = value;

                // set values
                checkEdit1.Checked = m_appSettings.Notification1Active;
                checkEdit2.Checked = m_appSettings.Notification2Active;
                checkEdit3.Checked = m_appSettings.Notification3Active;
                lookUpEdit1.EditValue = m_appSettings.Notification1symbol;
                lookUpEdit2.EditValue = m_appSettings.Notification2symbol;
                lookUpEdit3.EditValue = m_appSettings.Notification3symbol;
                comboBoxEdit1.SelectedIndex = m_appSettings.Notification1condition;
                comboBoxEdit2.SelectedIndex = m_appSettings.Notification2condition;
                comboBoxEdit3.SelectedIndex = m_appSettings.Notification3condition;
                textEdit1.Text = m_appSettings.Notification1value.ToString();
                textEdit2.Text = m_appSettings.Notification2value.ToString();
                textEdit3.Text = m_appSettings.Notification3value.ToString();
                buttonEdit1.Text = m_appSettings.Notification1sound;
                buttonEdit2.Text = m_appSettings.Notification2sound;
                buttonEdit3.Text = m_appSettings.Notification3sound;
            }
        }

        public frmNotifications()
        {
            InitializeComponent();
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

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            // set appsettings values
            m_appSettings.Notification1Active = checkEdit1.Checked;
            m_appSettings.Notification2Active = checkEdit2.Checked;
            m_appSettings.Notification3Active = checkEdit3.Checked;
            string sym1 = string.Empty;
            string sym2 = string.Empty;
            string sym3 = string.Empty;
            if (lookUpEdit1.EditValue != null) sym1 = lookUpEdit1.EditValue.ToString();
            if (lookUpEdit2.EditValue != null) sym2 = lookUpEdit2.EditValue.ToString();
            if (lookUpEdit3.EditValue != null) sym3 = lookUpEdit3.EditValue.ToString();
            m_appSettings.Notification1symbol = sym1;
            m_appSettings.Notification2symbol = sym2;
            m_appSettings.Notification3symbol = sym3;
            m_appSettings.Notification1condition = comboBoxEdit1.SelectedIndex;
            m_appSettings.Notification2condition = comboBoxEdit2.SelectedIndex;
            m_appSettings.Notification3condition = comboBoxEdit3.SelectedIndex;
            m_appSettings.Notification1value = ConvertToDouble(textEdit1.Text);
            m_appSettings.Notification2value = ConvertToDouble(textEdit2.Text);
            m_appSettings.Notification3value = ConvertToDouble(textEdit3.Text);
            m_appSettings.Notification1sound = buttonEdit1.Text;
            m_appSettings.Notification2sound = buttonEdit2.Text;
            m_appSettings.Notification3sound = buttonEdit3.Text;
            this.Close();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "wav files|*.wav";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                buttonEdit1.Text = ofd.FileName;
            }
        }

        private void buttonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "wav files|*.wav";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                buttonEdit2.Text = ofd.FileName;
            }
        }

        private void buttonEdit3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "wav files|*.wav";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.None;
                buttonEdit3.Text = ofd.FileName;
            }
        }
    }
}