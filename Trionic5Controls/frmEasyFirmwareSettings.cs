using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Controls
{
    public partial class frmEasyFirmwareSettings : DevExpress.XtraEditors.XtraForm
    {

        public frmEasyFirmwareSettings()
        {
            InitializeComponent();
        }

        public string CarModel
        {
            get
            {
                return txtCarModel.Text;
            }
            set
            {
                txtCarModel.Text = value;
                txtCarModel.Properties.MaxLength = txtCarModel.Text.Length;
            }
        }

        public string EngineType
        {
            get
            {
                return txtEngineType.Text;
            }
            set
            {
                txtEngineType.Text = value;
                txtEngineType.Properties.MaxLength = txtEngineType.Text.Length;

            }
        }

        public string PartNumber
        {
            get
            {
                return txtPartnumber.Text;
            }
            set
            {
                txtPartnumber.Text = value;
                txtPartnumber.Properties.MaxLength = txtPartnumber.Text.Length;

            }
        }

        public string SoftwareID
        {
            get
            {
                return txtSoftwareID.Text;
            }
            set
            {
                txtSoftwareID.Text = value;
                txtSoftwareID.Properties.MaxLength = txtSoftwareID.Text.Length;

            }
        }

        public string Dataname
        {
            get
            {
                return txtDataName.Text;
            }
            set
            {
                txtDataName.Text = value;
                txtDataName.Properties.MaxLength = txtDataName.Text.Length;

            }
        }


        public string VSSCode
        {
            get
            {
                return txtVSSCode.Text;
            }
            set
            {
                txtVSSCode.Text = value;
                txtVSSCode.Properties.MaxLength = txtVSSCode.Text.Length;

            }
        }

        public bool VSSEnabled
        {
            get
            {
                return chkVSSCode.Checked;
            }
            set
            {
                chkVSSCode.Checked = value;
            }
        }

        public bool TankPressureDiagnosticsEnabled
        {
            get
            {
                return chkTankPressureDiagnostics.Checked;
            }
            set
            {
                chkTankPressureDiagnostics.Checked = value;
            }
        }

        public bool SecondO2Enabled
        {
            get
            {
                return chkEnableSecondLambdaSensor.Checked;
            }
            set
            {
                chkEnableSecondLambdaSensor.Checked = value;
            }
        }

        public bool RAMLocked
        {
            get
            {
                return chkRAMLocked.Checked;
            }
            set
            {
                chkRAMLocked.Checked = value;
            }
        }


        public bool AutoGearBox
        {
            get
            {
                return chkAutomaticTransmission.Checked;
            }
            set
            {
                chkAutomaticTransmission.Checked = value;
            }
        }

        public bool HeatplatesPresent
        {
            get
            {
                return chkHeatPlates.Checked;
            }
            set
            {
                chkHeatPlates.Checked = value;
            }
        }

        public bool AfterstartEnrichment
        {
            get
            {
                return chkEnrichmentAfterStart.Checked;
            }
            set
            {
                chkEnrichmentAfterStart.Checked = value;
            }
        }

        public bool WOTEnrichment
        {
            get
            {
                return chkWOTEnrichment.Checked;
            }
            set
            {
                chkWOTEnrichment.Checked = value;
            }
        }

        /*public bool InterpolationOfDelay
        {
            get
            {
                return checkEdit5.Checked;
            }
            set
            {
                checkEdit5.Checked = value;
            }
        }*/

        public bool TemperatureCompensation
        {
            get
            {
                return chkTemperatureCompensation.Checked;
            }
            set
            {
                chkTemperatureCompensation.Checked = value;
            }
        }

        public bool LambdaControl
        {
            get
            {
                return chkLambdaControl.Checked;
            }
            set
            {
                chkLambdaControl.Checked = value;
            }
        }

        public bool Adaptivity
        {
            get
            {
                return chkAdaptivity.Checked;
            }
            set
            {
                chkAdaptivity.Checked = value;
            }
        }

        public bool IdleControl
        {
            get
            {
                return chkIdleControl.Checked;
            }
            set
            {
                chkIdleControl.Checked = value;
            }
        }

        public bool EnrichmentDuringStart
        {
            get
            {
                return chkEnrichmentDuringStart.Checked;
            }
            set
            {
                chkEnrichmentDuringStart.Checked = value;
            }
        }

        public bool LambdaDuringTransitions
        {
            get
            {
                return chkLambdaControlOnTransients.Checked;
            }
            set
            {
                chkLambdaControlOnTransients.Checked = value;
            }
        }

        public bool Fuelcut
        {
            get
            {
                return chkFuelcutEngineBrake.Checked;
            }
            set
            {
                chkFuelcutEngineBrake.Checked = value;
            }
        }
        public bool AccelEnrichment
        {
            get
            {
                return chkAccelerationEnrichment.Checked;
            }
            set
            {
                chkAccelerationEnrichment.Checked = value;
            }
        }
        public bool AirpumpControl
        {
            get
            {
                return chkAirpumpControl.Checked;
            }
            set
            {
                chkAirpumpControl.Checked = value;
            }
        }

        public bool DecelEnleanment
        {
            get
            {
                return chkDecelerationEnleanment.Checked;
            }
            set
            {
                chkDecelerationEnleanment.Checked = value;
            }
        }
        public bool AdaptionWithClosedThrottle
        {
            get
            {
                return chkAdaptivityWithClosedThrottle.Checked;
            }
            set
            {
                chkAdaptivityWithClosedThrottle.Checked = value;
            }
        }

        public bool FactorToLambdaWhenThrottleOpening
        {
            get
            {
                return chkLambdaCorrectionOnTPSOpening.Checked;
            }
            set
            {
                chkLambdaCorrectionOnTPSOpening.Checked = value;
            }
        }

        public bool SeperateInjectionMapForIdle
        {
            get
            {
                return chkUseIdleInjectionMap.Checked;
            }
            set
            {
                chkUseIdleInjectionMap.Checked = value;
            }
        }
        public bool FactorToLambdaWhenACEngaged
        {
            get
            {
                return chkLambdaCorrectionOnACEngage.Checked;
            }
            set
            {
                chkLambdaCorrectionOnACEngage.Checked = value;
            }
        }

        /*public bool ThrottleAccRetAdjustSimult
        {
            get
            {
                return checkEdit17.Checked;
            }
            set
            {
                checkEdit17.Checked = value;
            }
        }*/

        public bool FuelAdjustDuringIdle
        {
            get
            {
                return chkFuelAdjustDuringIdle.Checked;
            }
            set
            {
                chkFuelAdjustDuringIdle.Checked = value;
            }
        }

        public bool PurgeControl
        {
            get
            {
                return chkPurgeControl.Checked;
            }
            set
            {
                chkPurgeControl.Checked = value;
            }
        }

        public bool AdaptionOfIdleControl
        {
            get
            {
                return chkAdaptionOfIdleControl.Checked;
            }
            set
            {
                chkAdaptionOfIdleControl.Checked = value;
            }
        }

        public bool LambdaControlDuringIdle
        {
            get
            {
                return chkLambdaControlDuringIdle.Checked;
            }
            set
            {
                chkLambdaControlDuringIdle.Checked = value;
            }
        }

        public bool Tempcompwithactivelambdacontrol
        {
            get
            {
                return chkTemperatureCorrectionInClosedLoop.Checked;
            }
            set
            {
                chkTemperatureCorrectionInClosedLoop.Checked = value;
            }
        }
        public bool NoFuelCutR12
        {
            get
            {
                return chkNoFuelcutR12.Checked;
            }
            set
            {
                chkNoFuelcutR12.Checked = value;
            }
        }

        public bool GlobalAdaption
        {
            get
            {
                return chkGlobalAdaption.Checked;
            }
            set
            {
                chkGlobalAdaption.Checked = value;
            }
        }

        public bool HigherIdleDuringStart
        {
            get
            {
                return chkHigherIdleOnStart.Checked;
            }
            set
            {
                chkHigherIdleOnStart.Checked = value;
            }
        }

        public bool APCControl
        {
            get
            {
                return chkBoostControl.Checked;
            }
            set
            {
                chkBoostControl.Checked = value;
            }
        }

        public bool ETS
        {
            get
            {
                return chkETS.Checked;
            }
            set
            {
                chkETS.Checked = value;
            }
        }

        public bool LoadControl
        {
            get
            {
                return chkLoadControl.Checked;
            }
            set
            {
                chkLoadControl.Checked = value;
            }
        }

        public bool LoadBufferDuringIdle
        {
            get
            {
                return chkLoadBufferOnIdle.Checked;
            }
            set
            {
                chkLoadBufferOnIdle.Checked = value;
            }
        }
        public bool ConstantInjectionTime
        {
            get
            {
                return chkConstantInjectionE51.Checked;
            }
            set
            {
                chkConstantInjectionE51.Checked = value;
            }
        }

        public bool ConstantInjectionTimeDuringIdle
        {
            get
            {
                return chkConstantInjectionOnIdle.Checked;
            }
            set
            {
                chkConstantInjectionOnIdle.Checked = value;
            }
        }

        public bool PurgeValveMY94
        {
            get
            {
                return chkPurgeValveMY94.Checked;
            }
            set
            {
                chkPurgeValveMY94.Checked = value;
            }
        }

        /*public bool ConstantAngle
        {
            get
            {
                return chkIdleIgnitionGear12.Checked;
            }
            set
            {
                chkIdleIgnitionGear12.Checked = value;
            }
        }*/

        public bool KnockRegulatingDisabled
        {
            get
            {
                return chkKnockDetectionOff.Checked;
            }
            set
            {
                chkKnockDetectionOff.Checked = value;
            }
        }

        public bool NormalAsperatedEngine
        {
            get
            {
                return chkNormallyAspirated.Checked;
            }
            set
            {
                chkNormallyAspirated.Checked = value;
            }
        }

        public bool ConstIdleIgnAngleDuringFirstAndSecondGear
        {
            get
            {
                return chkIdleIgnitionGear12.Checked;
            }
            set
            {
                chkIdleIgnitionGear12.Checked = value;
            }
        }

        public void DisableVSSOptions()
        {
            chkVSSCode.Enabled = false;
            txtVSSCode.Enabled = false;
        }

        public void DisableAdvancedControls()
        {
            chkLoadBufferOnIdle.Enabled = false;
            chkIdleIgnitionGear12.Enabled = false;
            chkNoFuelcutR12.Enabled = false;
            chkAirpumpControl.Enabled = false;
            chkNormallyAspirated.Enabled = false;
            chkKnockDetectionOff.Enabled = false;
            chkPurgeValveMY94.Enabled = false;
            /*
Loadbufferduringidle,               // byte 4, bit 1
Constidleignangleduringgearoneandtwo,// byte 4, bit 2
NofuelcutR12,                       // byte 4, bit 3
Airpumpcontrol,                     // byte 4, bit 4
Normalasperatedengine,              // byte 4, bit 5
Knockregulatingdisabled,            // byte 4, bit 6
Constantangle,                      // byte 4, bit 7
PurgevalveMY94                      // byte 4, bit 8
* */


        }

        public bool IsTrionic55
        {
            get
            {
                return chkTrionic55.Checked;
            }
            set
            {
                chkTrionic55.Checked = value;
                if (!chkTrionic55.Checked)
                {
                    chkEnableSecondLambdaSensor.Enabled = false;
                    chkEnableSecondLambdaSensor.Checked = false;
                }
            }
        }

        public InjectorType Injectors
        {
            get
            {
                return (InjectorType)cbxInjectors.SelectedIndex;
            }
            set
            {
                cbxInjectors.SelectedIndex = (int)value;
            }
        }

        public TurboType Turbo
        {
            get
            {
                return (TurboType)cbxTurbo.SelectedIndex;
            }
            set
            {
                cbxTurbo.SelectedIndex = (int)value;
            }
        }

        public MapSensorType MapSensor
        {
            get
            {
                return (MapSensorType)cbxMapsensor.SelectedIndex;
            }
            set
            {
                cbxMapsensor.SelectedIndex = (int)value;
            }
        }

        public TuningStage TuningStage
        {
            get
            {
                return (TuningStage)cbxTuningStage.SelectedIndex;
            }
            set
            {
                cbxTuningStage.SelectedIndex = (int)value;
            }
        }


        public DateTime SynchDateTime
        {
            set
            {
                txtSyncTimestamp.Text = value.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        public string CPUFrequency
        {
            get { return txtCPUSpeed.Text; }
            set
            {
                txtCPUSpeed.Text = value;
            }
        }


        private bool m_compare_file = false;

        public bool Compare_file
        {
            get { return m_compare_file; }
            set { m_compare_file = value; }
        }
        private bool m_open_file = false;
        private string m_filetoOpen = string.Empty;

        public string FiletoOpen
        {
            get { return m_filetoOpen; }
            set { m_filetoOpen = value; }
        }
        public bool Open_file
        {
            get { return m_open_file; }
            set { m_open_file = value; }
        }
        private void txtPartnumber_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            frmPartnumberLookup partnumberlookup = new frmPartnumberLookup();
            partnumberlookup.LookUpPartnumber(txtPartnumber.Text);
            partnumberlookup.ShowDialog();
            if (partnumberlookup.Open_File)
            {
                m_filetoOpen = partnumberlookup.GetFileToOpen();
                m_open_file = true;
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else if (partnumberlookup.Compare_File)
            {
                m_filetoOpen = partnumberlookup.GetFileToOpen();
                m_compare_file = true;
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void chkRAMLocked_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRamLockedColor();
        }

        private void UpdateRamLockedColor()
        {
            if (chkRAMLocked.Checked) chkRAMLocked.ForeColor = Color.Red;
            else chkRAMLocked.ForeColor = Color.Black;
        }

        private void frmEasyFirmwareSettings_Shown(object sender, EventArgs e)
        {
            UpdateRamLockedColor();
        }
    }
}