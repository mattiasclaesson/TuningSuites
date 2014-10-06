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
    public partial class frmMultiAdapterConfig : DevExpress.XtraEditors.XtraForm
    {
        private AppSettings m_appSettings;

        public AppSettings AppSettings
        {
            get { return m_appSettings; }
            set
            {
                m_appSettings = value;
                buttonEdit1.Text = m_appSettings.Adc1channelname;
                buttonEdit2.Text = m_appSettings.Adc2channelname;
                buttonEdit3.Text = m_appSettings.Adc3channelname;
                buttonEdit4.Text = m_appSettings.Adc4channelname;
                buttonEdit5.Text = m_appSettings.Adc5channelname;
                textEdit1.Text = m_appSettings.Thermochannelname;
                chkUseADC1.Checked = m_appSettings.Useadc1;
                chkUseADC2.Checked = m_appSettings.Useadc2;
                chkUseADC3.Checked = m_appSettings.Useadc3;
                chkUseADC4.Checked = m_appSettings.Useadc4;
                chkUseADC5.Checked = m_appSettings.Useadc5;
                chkUseThermoInput.Checked = m_appSettings.Usethermo;
            }
        }

        public frmMultiAdapterConfig()
        {
            InitializeComponent();
        }

        private void chkUseADC1_CheckedChanged(object sender, EventArgs e)
        {
            buttonEdit1.Enabled = chkUseADC1.Checked;
            m_appSettings.Useadc1 = chkUseADC1.Checked;
        }

        private void chkUseADC2_CheckedChanged(object sender, EventArgs e)
        {
            buttonEdit2.Enabled = chkUseADC2.Checked;
            m_appSettings.Useadc2 = chkUseADC2.Checked;
        }

        private void chkUseADC3_CheckedChanged(object sender, EventArgs e)
        {
            buttonEdit3.Enabled = chkUseADC3.Checked;
            m_appSettings.Useadc3 = chkUseADC3.Checked;
        }

        private void chkUseADC4_CheckedChanged(object sender, EventArgs e)
        {
            buttonEdit4.Enabled = chkUseADC4.Checked;
            m_appSettings.Useadc4 = chkUseADC4.Checked;
        }

        private void chkUseADC5_CheckedChanged(object sender, EventArgs e)
        {
            buttonEdit5.Enabled = chkUseADC5.Checked;
            m_appSettings.Useadc5 = chkUseADC5.Checked;
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // start the config screen for this ADC input
            frmADCInputConfig config = new frmADCInputConfig();
            config.ChannelName = m_appSettings.Adc1channelname;
            config.LowVoltage = m_appSettings.Adc1lowvoltage;
            config.HighVoltage = m_appSettings.Adc1highvoltage;
            config.LowValue = m_appSettings.Adc1lowvalue;
            config.HighValue = m_appSettings.Adc1highvalue;
            config.ShowDialog();
            m_appSettings.Adc1channelname = config.ChannelName;
            m_appSettings.Adc1lowvoltage = config.LowVoltage;
            m_appSettings.Adc1highvoltage = config.HighVoltage;
            m_appSettings.Adc1lowvalue = config.LowValue;
            m_appSettings.Adc1highvalue = config.HighValue;
            buttonEdit1.Text = config.ChannelName;
        }

        private void chkUseThermoInput_CheckedChanged(object sender, EventArgs e)
        {
            textEdit1.Enabled = chkUseThermoInput.Checked;
            m_appSettings.Usethermo = chkUseThermoInput.Checked;

        }

        private void buttonEdit2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // start the config screen for this ADC input
            frmADCInputConfig config = new frmADCInputConfig();
            config.ChannelName = m_appSettings.Adc2channelname;
            config.LowVoltage = m_appSettings.Adc2lowvoltage;
            config.HighVoltage = m_appSettings.Adc2highvoltage;
            config.LowValue = m_appSettings.Adc2lowvalue;
            config.HighValue = m_appSettings.Adc2highvalue;
            config.ShowDialog();
            m_appSettings.Adc2channelname = config.ChannelName;
            m_appSettings.Adc2lowvoltage = config.LowVoltage;
            m_appSettings.Adc2highvoltage = config.HighVoltage;
            m_appSettings.Adc2lowvalue = config.LowValue;
            m_appSettings.Adc2highvalue = config.HighValue;
            buttonEdit2.Text = config.ChannelName;
        }

        private void buttonEdit3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // start the config screen for this ADC input
            frmADCInputConfig config = new frmADCInputConfig();
            config.ChannelName = m_appSettings.Adc3channelname;
            config.LowVoltage = m_appSettings.Adc3lowvoltage;
            config.HighVoltage = m_appSettings.Adc3highvoltage;
            config.LowValue = m_appSettings.Adc3lowvalue;
            config.HighValue = m_appSettings.Adc3highvalue;
            config.ShowDialog();
            m_appSettings.Adc3channelname = config.ChannelName;
            m_appSettings.Adc3lowvoltage = config.LowVoltage;
            m_appSettings.Adc3highvoltage = config.HighVoltage;
            m_appSettings.Adc3lowvalue = config.LowValue;
            m_appSettings.Adc3highvalue = config.HighValue;
            buttonEdit3.Text = config.ChannelName;
        }

        private void buttonEdit4_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // start the config screen for this ADC input
            frmADCInputConfig config = new frmADCInputConfig();
            config.ChannelName = m_appSettings.Adc4channelname;
            config.LowVoltage = m_appSettings.Adc4lowvoltage;
            config.HighVoltage = m_appSettings.Adc4highvoltage;
            config.LowValue = m_appSettings.Adc4lowvalue;
            config.HighValue = m_appSettings.Adc4highvalue;
            config.ShowDialog();
            m_appSettings.Adc4channelname = config.ChannelName;
            m_appSettings.Adc4lowvoltage = config.LowVoltage;
            m_appSettings.Adc4highvoltage = config.HighVoltage;
            m_appSettings.Adc4lowvalue = config.LowValue;
            m_appSettings.Adc4highvalue = config.HighValue;
            buttonEdit4.Text = config.ChannelName;
        }

        private void buttonEdit5_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            // start the config screen for this ADC input
            frmADCInputConfig config = new frmADCInputConfig();
            config.ChannelName = m_appSettings.Adc5channelname;
            config.LowVoltage = m_appSettings.Adc5lowvoltage;
            config.HighVoltage = m_appSettings.Adc5highvoltage;
            config.LowValue = m_appSettings.Adc5lowvalue;
            config.HighValue = m_appSettings.Adc5highvalue;
            config.ShowDialog();
            m_appSettings.Adc5channelname = config.ChannelName;
            m_appSettings.Adc5lowvoltage = config.LowVoltage;
            m_appSettings.Adc5highvoltage = config.HighVoltage;
            m_appSettings.Adc5lowvalue = config.LowValue;
            m_appSettings.Adc5highvalue = config.HighValue;
            buttonEdit5.Text = config.ChannelName;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            m_appSettings.Thermochannelname = textEdit1.Text;
            this.Close();
        }
    }
}