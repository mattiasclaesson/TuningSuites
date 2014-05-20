using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Trionic5Tools;


namespace Trionic5Controls
{
    public partial class frmEasyFirmwareInfo : DevExpress.XtraEditors.XtraForm
    {
        private byte[] m_pgm_mod;

        public byte[] Pgm_mod
        {
            get { return m_pgm_mod; }
            set
            {
                m_pgm_mod = value;
                // set all variable according to specs
                if (m_pgm_mod.Length < 6)
                {
                    DisableVSSOptions();
                }
                if (m_pgm_mod.Length < 5)
                {
                    DisableAdvancedControls();
                }
                if ((m_pgm_mod[0] & 0x01) > 0) AfterstartEnrichment = true;
                else AfterstartEnrichment = false;
                if ((m_pgm_mod[0] & 0x02) > 0) WOTEnrichment = true;
                else WOTEnrichment = false;
                if ((m_pgm_mod[0] & 0x08) > 0) TemperatureCompensation = true;
                else TemperatureCompensation = false;
                if ((m_pgm_mod[0] & 0x10) > 0) LambdaControl = true;
                else LambdaControl = false;
                if ((m_pgm_mod[0] & 0x20) > 0) Adaptivity = true;
                else Adaptivity = false;
                if ((m_pgm_mod[0] & 0x40) > 0) IdleControl = true;
                else IdleControl = false;
                if ((m_pgm_mod[0] & 0x80) > 0) EnrichmentDuringStart = true;
                else EnrichmentDuringStart = false;
                if ((m_pgm_mod[1] & 0x01) > 0) ConstantInjectionTime = true;
                else ConstantInjectionTime = false;
                if ((m_pgm_mod[1] & 0x02) > 0) LambdaDuringTransitions = true;
                else LambdaDuringTransitions = false;
                if ((m_pgm_mod[1] & 0x04) > 0) Fuelcut = true;
                else Fuelcut = false;
                if ((m_pgm_mod[1] & 0x08) > 0) ConstantInjectionTimeDuringIdle = true;
                else ConstantInjectionTimeDuringIdle = false;
                if ((m_pgm_mod[1] & 0x10) > 0) AccelEnrichment = true;
                else AccelEnrichment = false;
                if ((m_pgm_mod[1] & 0x20) > 0) DecelEnleanment = true;
                else DecelEnleanment = false;
                if ((m_pgm_mod[1] & 0x80) > 0) AdaptionWithClosedThrottle = true;
                else AdaptionWithClosedThrottle = false;
                if ((m_pgm_mod[2] & 0x01) > 0) FactorToLambdaWhenThrottleOpening = true;
                else FactorToLambdaWhenThrottleOpening = false;
                if ((m_pgm_mod[2] & 0x02) > 0) SeperateInjectionMapForIdle = true;
                else SeperateInjectionMapForIdle = false;
                if ((m_pgm_mod[2] & 0x04) > 0) FactorToLambdaWhenACEngaged = true;
                else FactorToLambdaWhenACEngaged = false;
                if ((m_pgm_mod[2] & 0x10) > 0) FuelAdjustDuringIdle = true;
                else FuelAdjustDuringIdle = false;
                if ((m_pgm_mod[2] & 0x20) > 0) PurgeControl = true;
                else PurgeControl = false;
                if ((m_pgm_mod[2] & 0x40) > 0) AdaptionOfIdleControl = true;
                else AdaptionOfIdleControl = false;
                if ((m_pgm_mod[2] & 0x80) > 0) LambdaControlDuringIdle = true;
                else LambdaControlDuringIdle = false;
                if ((m_pgm_mod[3] & 0x01) > 0) HeatplatesPresent = true;
                else HeatplatesPresent = false;
                if ((m_pgm_mod[3] & 0x02) > 0) AutoGearBox = true;
                else AutoGearBox = false;
                if ((m_pgm_mod[3] & 0x04) > 0) LoadControl = true;
                else LoadControl = false;
                if ((m_pgm_mod[3] & 0x08) > 0) ETS = true;
                else ETS = false;
                if ((m_pgm_mod[3] & 0x10) > 0) APCControl = true;
                else APCControl = false;
                if ((m_pgm_mod[3] & 0x20) > 0) HigherIdleDuringStart = true;
                else HigherIdleDuringStart = false;
                if ((m_pgm_mod[3] & 0x40) > 0) GlobalAdaption = true;
                else GlobalAdaption = false;
                if ((m_pgm_mod[3] & 0x80) > 0) Tempcompwithactivelambdacontrol = true;
                else Tempcompwithactivelambdacontrol = false;
                if (m_pgm_mod.Length > 4)
                {
                    if ((m_pgm_mod[4] & 0x01) > 0) LoadBufferDuringIdle = true;
                    else LoadBufferDuringIdle = false;
                    if ((m_pgm_mod[4] & 0x02) > 0) ConstIdleIgnAngleDuringFirstAndSecondGear = true;
                    else ConstIdleIgnAngleDuringFirstAndSecondGear = false;
                    if ((m_pgm_mod[4] & 0x04) > 0) NoFuelCutR12 = true;
                    else NoFuelCutR12 = false;
                    if ((m_pgm_mod[4] & 0x08) > 0) AirpumpControl = true;
                    else AirpumpControl = false;
                    if ((m_pgm_mod[4] & 0x10) > 0) NormalAsperatedEngine = true;
                    else NormalAsperatedEngine = false;
                    if ((m_pgm_mod[4] & 0x20) > 0) KnockRegulatingDisabled = true;
                    else KnockRegulatingDisabled = false;
                    //if ((m_pgm_mod[4] & 0x40) > 0) Consta = true;
                    //else KnockRegulatingDisabled = false;
                    if ((m_pgm_mod[4] & 0x80) > 0) PurgeValveMY94 = true;
                    else PurgeValveMY94 = false;
                }
                if (m_pgm_mod.Length > 5)
                {
                    if ((m_pgm_mod[5] & 0x80) > 0) VSSEnabled = true;
                    else VSSEnabled = false;
                    if ((m_pgm_mod[5] & 0x10) > 0) TankDiagnosticsEnabled = true;
                    else TankDiagnosticsEnabled = false;
                }
            }
        }

        private byte[] m_pgm_mod2;

        public byte[] Pgm_mod2
        {
            get { return m_pgm_mod2; }
            set
            {
                m_pgm_mod2 = value;
                // set all variable according to specs
                if (m_pgm_mod2.Length < 6)
                {
                    DisableVSSOptions2();
                }
                if (m_pgm_mod2.Length < 5)
                {
                    DisableAdvancedControls2();
                }
                if ((m_pgm_mod2[0] & 0x01) > 0) AfterstartEnrichment2 = true;
                else AfterstartEnrichment2 = false;
                if ((m_pgm_mod2[0] & 0x02) > 0) WOTEnrichment2 = true;
                else WOTEnrichment2 = false;
                if ((m_pgm_mod2[0] & 0x08) > 0) TemperatureCompensation2 = true;
                else TemperatureCompensation2 = false;
                if ((m_pgm_mod2[0] & 0x10) > 0) LambdaControl2 = true;
                else LambdaControl2 = false;
                if ((m_pgm_mod2[0] & 0x20) > 0) Adaptivity2 = true;
                else Adaptivity2 = false;
                if ((m_pgm_mod2[0] & 0x40) > 0) IdleControl2 = true;
                else IdleControl2 = false;
                if ((m_pgm_mod2[0] & 0x80) > 0) EnrichmentDuringStart2 = true;
                else EnrichmentDuringStart2 = false;
                if ((m_pgm_mod2[1] & 0x01) > 0) ConstantInjectionTime2 = true;
                else ConstantInjectionTime2 = false;
                if ((m_pgm_mod2[1] & 0x02) > 0) LambdaDuringTransitions2 = true;
                else LambdaDuringTransitions2 = false;
                if ((m_pgm_mod2[1] & 0x04) > 0) Fuelcut2 = true;
                else Fuelcut2 = false;
                if ((m_pgm_mod2[1] & 0x08) > 0) ConstantInjectionTimeDuringIdle2 = true;
                else ConstantInjectionTimeDuringIdle2 = false;
                if ((m_pgm_mod2[1] & 0x10) > 0) AccelEnrichment2 = true;
                else AccelEnrichment2 = false;
                if ((m_pgm_mod2[1] & 0x20) > 0) DecelEnleanment2 = true;
                else DecelEnleanment2 = false;
                if ((m_pgm_mod2[1] & 0x80) > 0) AdaptionWithClosedThrottle2 = true;
                else AdaptionWithClosedThrottle2 = false;
                if ((m_pgm_mod2[2] & 0x01) > 0) FactorToLambdaWhenThrottleOpening2 = true;
                else FactorToLambdaWhenThrottleOpening2 = false;
                if ((m_pgm_mod2[2] & 0x02) > 0) SeperateInjectionMapForIdle2 = true;
                else SeperateInjectionMapForIdle2 = false;
                if ((m_pgm_mod2[2] & 0x04) > 0) FactorToLambdaWhenACEngaged2 = true;
                else FactorToLambdaWhenACEngaged2 = false;
                if ((m_pgm_mod2[2] & 0x10) > 0) FuelAdjustDuringIdle2 = true;
                else FuelAdjustDuringIdle2 = false;
                if ((m_pgm_mod2[2] & 0x20) > 0) PurgeControl2 = true;
                else PurgeControl2 = false;
                if ((m_pgm_mod2[2] & 0x40) > 0) AdaptionOfIdleControl2 = true;
                else AdaptionOfIdleControl2 = false;
                if ((m_pgm_mod2[2] & 0x80) > 0) LambdaControlDuringIdle2 = true;
                else LambdaControlDuringIdle2 = false;
                if ((m_pgm_mod2[3] & 0x01) > 0) HeatplatesPresent2 = true;
                else HeatplatesPresent2 = false;
                if ((m_pgm_mod2[3] & 0x02) > 0) AutoGearBox2 = true;
                else AutoGearBox2 = false;
                if ((m_pgm_mod2[3] & 0x04) > 0) LoadControl2 = true;
                else LoadControl2 = false;
                if ((m_pgm_mod2[3] & 0x08) > 0) ETS2 = true;
                else ETS2 = false;
                if ((m_pgm_mod2[3] & 0x10) > 0) APCControl2 = true;
                else APCControl2 = false;
                if ((m_pgm_mod2[3] & 0x20) > 0) HigherIdleDuringStart2 = true;
                else HigherIdleDuringStart2 = false;
                if ((m_pgm_mod2[3] & 0x40) > 0) GlobalAdaption2 = true;
                else GlobalAdaption2 = false;
                if ((m_pgm_mod2[3] & 0x80) > 0) Tempcompwithactivelambdacontrol2 = true;
                else Tempcompwithactivelambdacontrol2 = false;
                if (m_pgm_mod2.Length > 4)
                {
                    if ((m_pgm_mod2[4] & 0x01) > 0) LoadBufferDuringIdle2 = true;
                    else LoadBufferDuringIdle2 = false;
                    if ((m_pgm_mod2[4] & 0x02) > 0) ConstIdleIgnAngleDuringFirstAndSecondGear2 = true;
                    else ConstIdleIgnAngleDuringFirstAndSecondGear2 = false;
                    if ((m_pgm_mod2[4] & 0x04) > 0) NoFuelCutR122 = true;
                    else NoFuelCutR122 = false;
                    if ((m_pgm_mod2[4] & 0x08) > 0) AirpumpControl2 = true;
                    else AirpumpControl2 = false;
                    if ((m_pgm_mod2[4] & 0x10) > 0) NormalAsperatedEngine2 = true;
                    else NormalAsperatedEngine2 = false;
                    if ((m_pgm_mod2[4] & 0x20) > 0) KnockRegulatingDisabled2 = true;
                    else KnockRegulatingDisabled2 = false;
                    //if ((m_pgm_mod2[4] & 0x40) > 0) Consta = true;
                    //else KnockRegulatingDisabled = false;
                    if ((m_pgm_mod2[4] & 0x80) > 0) PurgeValveMY942 = true;
                    else PurgeValveMY942 = false;
                }
                if (m_pgm_mod2.Length > 5)
                {
                    if ((m_pgm_mod2[5] & 0x80) > 0) VSSEnabled2 = true;
                    else VSSEnabled2 = false;
                    if ((m_pgm_mod2[5] & 0x10) > 0) TankDiagnosticsEnabled2 = true;
                    else TankDiagnosticsEnabled2 = false;
                }
                HighlighDifferences();
            }
        }

        public frmEasyFirmwareInfo()
        {
            InitializeComponent();
        }

        public bool TankDiagnosticsEnabled
        {
            get
            {
                return chkTankDiagnostics.Checked;
            }
            set
            {
                chkTankDiagnostics.Checked = value;
            }
        }

        public bool TankDiagnosticsEnabled2
        {
            get
            {
                return chkTankDiagnostics2.Checked;
            }
            set
            {
                chkTankDiagnostics2.Checked = value;
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
            chkTankDiagnostics.Enabled = false;
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

        
        /************************ secondary source *************************/
        public bool VSSEnabled2
        {
            get
            {
                return chkVSSCode2.Checked;
            }
            set
            {
                chkVSSCode2.Checked = value;
            }
        }

        public bool SecondO2Enabled2
        {
            get
            {
                return chkEnableSecondLambdaSensor2.Checked;
            }
            set
            {
                chkEnableSecondLambdaSensor2.Checked = value;
            }
        }




        public bool AutoGearBox2
        {
            get
            {
                return chkAutomaticTransmission2.Checked;
            }
            set
            {
                chkAutomaticTransmission2.Checked = value;
            }
        }

        public bool HeatplatesPresent2
        {
            get
            {
                return chkHeatPlates2.Checked;
            }
            set
            {
                chkHeatPlates2.Checked = value;
            }
        }

        public bool AfterstartEnrichment2
        {
            get
            {
                return chkEnrichmentAfterStart2.Checked;
            }
            set
            {
                chkEnrichmentAfterStart2.Checked = value;
            }
        }

        public bool WOTEnrichment2
        {
            get
            {
                return chkWOTEnrichment2.Checked;
            }
            set
            {
                chkWOTEnrichment2.Checked = value;
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

        public bool TemperatureCompensation2
        {
            get
            {
                return chkTemperatureCompensation2.Checked;
            }
            set
            {
                chkTemperatureCompensation2.Checked = value;
            }
        }

        public bool LambdaControl2
        {
            get
            {
                return chkLambdaControl2.Checked;
            }
            set
            {
                chkLambdaControl2.Checked = value;
            }
        }

        public bool Adaptivity2
        {
            get
            {
                return chkAdaptivity2.Checked;
            }
            set
            {
                chkAdaptivity2.Checked = value;
            }
        }

        public bool IdleControl2
        {
            get
            {
                return chkIdleControl2.Checked;
            }
            set
            {
                chkIdleControl2.Checked = value;
            }
        }

        public bool EnrichmentDuringStart2
        {
            get
            {
                return chkEnrichmentDuringStart2.Checked;
            }
            set
            {
                chkEnrichmentDuringStart2.Checked = value;
            }
        }

        public bool LambdaDuringTransitions2
        {
            get
            {
                return chkLambdaControlOnTransients2.Checked;
            }
            set
            {
                chkLambdaControlOnTransients2.Checked = value;
            }
        }

        public bool Fuelcut2
        {
            get
            {
                return chkFuelcutEngineBrake2.Checked;
            }
            set
            {
                chkFuelcutEngineBrake2.Checked = value;
            }
        }
        public bool AccelEnrichment2
        {
            get
            {
                return chkAccelerationEnrichment2.Checked;
            }
            set
            {
                chkAccelerationEnrichment2.Checked = value;
            }
        }
        public bool AirpumpControl2
        {
            get
            {
                return chkAirpumpControl2.Checked;
            }
            set
            {
                chkAirpumpControl2.Checked = value;
            }
        }

        public bool DecelEnleanment2
        {
            get
            {
                return chkDecelerationEnleanment2.Checked;
            }
            set
            {
                chkDecelerationEnleanment2.Checked = value;
            }
        }
        public bool AdaptionWithClosedThrottle2
        {
            get
            {
                return chkAdaptivityWithClosedThrottle2.Checked;
            }
            set
            {
                chkAdaptivityWithClosedThrottle2.Checked = value;
            }
        }

        public bool FactorToLambdaWhenThrottleOpening2
        {
            get
            {
                return chkLambdaCorrectionOnTPSOpening2.Checked;
            }
            set
            {
                chkLambdaCorrectionOnTPSOpening2.Checked = value;
            }
        }

        public bool SeperateInjectionMapForIdle2
        {
            get
            {
                return chkUseIdleInjectionMap2.Checked;
            }
            set
            {
                chkUseIdleInjectionMap2.Checked = value;
            }
        }
        public bool FactorToLambdaWhenACEngaged2
        {
            get
            {
                return chkLambdaCorrectionOnACEngage2.Checked;
            }
            set
            {
                chkLambdaCorrectionOnACEngage2.Checked = value;
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

        public bool FuelAdjustDuringIdle2
        {
            get
            {
                return chkFuelAdjustDuringIdle2.Checked;
            }
            set
            {
                chkFuelAdjustDuringIdle2.Checked = value;
            }
        }

        public bool PurgeControl2
        {
            get
            {
                return chkPurgeControl2.Checked;
            }
            set
            {
                chkPurgeControl2.Checked = value;
            }
        }

        public bool AdaptionOfIdleControl2
        {
            get
            {
                return chkAdaptionOfIdleControl2.Checked;
            }
            set
            {
                chkAdaptionOfIdleControl2.Checked = value;
            }
        }

        public bool LambdaControlDuringIdle2
        {
            get
            {
                return chkLambdaControlDuringIdle2.Checked;
            }
            set
            {
                chkLambdaControlDuringIdle2.Checked = value;
            }
        }

        public bool Tempcompwithactivelambdacontrol2
        {
            get
            {
                return chkTemperatureCorrectionInClosedLoop2.Checked;
            }
            set
            {
                chkTemperatureCorrectionInClosedLoop2.Checked = value;
            }
        }
        public bool NoFuelCutR122
        {
            get
            {
                return chkNoFuelcutR122.Checked;
            }
            set
            {
                chkNoFuelcutR122.Checked = value;
            }
        }

        public bool GlobalAdaption2
        {
            get
            {
                return chkGlobalAdaption2.Checked;
            }
            set
            {
                chkGlobalAdaption2.Checked = value;
            }
        }

        public bool HigherIdleDuringStart2
        {
            get
            {
                return chkHigherIdleOnStart2.Checked;
            }
            set
            {
                chkHigherIdleOnStart2.Checked = value;
            }
        }

        public bool APCControl2
        {
            get
            {
                return chkBoostControl2.Checked;
            }
            set
            {
                chkBoostControl2.Checked = value;
            }
        }

        public bool ETS2
        {
            get
            {
                return chkETS2.Checked;
            }
            set
            {
                chkETS2.Checked = value;
            }
        }

        public bool LoadControl2
        {
            get
            {
                return chkLoadControl2.Checked;
            }
            set
            {
                chkLoadControl2.Checked = value;
            }
        }

        public bool LoadBufferDuringIdle2
        {
            get
            {
                return chkLoadBufferOnIdle2.Checked;
            }
            set
            {
                chkLoadBufferOnIdle2.Checked = value;
            }
        }
        public bool ConstantInjectionTime2
        {
            get
            {
                return chkConstantInjectionE512.Checked;
            }
            set
            {
                chkConstantInjectionE512.Checked = value;
            }
        }

        public bool ConstantInjectionTimeDuringIdle2
        {
            get
            {
                return chkConstantInjectionOnIdle2.Checked;
            }
            set
            {
                chkConstantInjectionOnIdle2.Checked = value;
            }
        }

        public bool PurgeValveMY942
        {
            get
            {
                return chkPurgeValveMY942.Checked;
            }
            set
            {
                chkPurgeValveMY942.Checked = value;
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

        public bool KnockRegulatingDisabled2
        {
            get
            {
                return chkKnockDetectionOff2.Checked;
            }
            set
            {
                chkKnockDetectionOff2.Checked = value;
            }
        }

        public bool NormalAsperatedEngine2
        {
            get
            {
                return chkNormallyAspirated2.Checked;
            }
            set
            {
                chkNormallyAspirated2.Checked = value;
            }
        }

        public bool ConstIdleIgnAngleDuringFirstAndSecondGear2
        {
            get
            {
                return chkIdleIgnitionGear122.Checked;
            }
            set
            {
                chkIdleIgnitionGear122.Checked = value;
            }
        }

        public void DisableVSSOptions2()
        {
            chkVSSCode2.Enabled = false;
            chkTankDiagnostics2.Enabled = false;
        }

        public void DisableAdvancedControls2()
        {
            chkLoadBufferOnIdle2.Enabled = false;
            chkIdleIgnitionGear122.Enabled = false;
            chkNoFuelcutR122.Enabled = false;
            chkAirpumpControl2.Enabled = false;
            chkNormallyAspirated2.Enabled = false;
            chkKnockDetectionOff2.Enabled = false;
            chkPurgeValveMY942.Enabled = false;
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
        
        

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void chkETS_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAirpumpControl_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkBoostControl_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void HighlighDifferences()
        {
            if (groupControl1.Enabled)
            {
                if (chkAccelerationEnrichment.Checked != chkAccelerationEnrichment2.Checked)
                {
                    chkAccelerationEnrichment.BackColor = Color.Orange;
                    chkAccelerationEnrichment2.BackColor = Color.Orange;
                }
                if (chkAdaptionOfIdleControl.Checked != chkAdaptionOfIdleControl2.Checked)
                {
                    chkAdaptionOfIdleControl.BackColor = Color.Orange;
                    chkAdaptionOfIdleControl2.BackColor = Color.Orange;
                }
                if (chkAdaptivity.Checked != chkAdaptivity2.Checked)
                {
                    chkAdaptivity.BackColor = Color.Orange;
                    chkAdaptivity2.BackColor = Color.Orange;
                }
                if (chkAdaptivityWithClosedThrottle.Checked != chkAdaptivityWithClosedThrottle2.Checked)
                {
                    chkAdaptivityWithClosedThrottle.BackColor = Color.Orange;
                    chkAdaptivityWithClosedThrottle2.BackColor = Color.Orange;
                }
                if (chkAirpumpControl.Checked != chkAirpumpControl2.Checked)
                {
                    chkAirpumpControl.BackColor = Color.Orange;
                    chkAirpumpControl2.BackColor = Color.Orange;
                }
                if (chkAutomaticTransmission.Checked != chkAutomaticTransmission2.Checked)
                {
                    chkAutomaticTransmission.BackColor = Color.Orange;
                    chkAutomaticTransmission2.BackColor = Color.Orange;
                }
                if (chkBoostControl.Checked != chkBoostControl2.Checked)
                {
                    chkBoostControl.BackColor = Color.Orange;
                    chkBoostControl2.BackColor = Color.Orange;
                }
                if (chkConstantInjectionE51.Checked != chkConstantInjectionE512.Checked)
                {
                    chkConstantInjectionE51.BackColor = Color.Orange;
                    chkConstantInjectionE512.BackColor = Color.Orange;
                }
                if (chkConstantInjectionOnIdle.Checked != chkConstantInjectionOnIdle2.Checked)
                {
                    chkConstantInjectionOnIdle.BackColor = Color.Orange;
                    chkConstantInjectionOnIdle2.BackColor = Color.Orange;
                }
                if (chkDecelerationEnleanment.Checked != chkDecelerationEnleanment2.Checked)
                {
                    chkDecelerationEnleanment.BackColor = Color.Orange;
                    chkDecelerationEnleanment2.BackColor = Color.Orange;
                }
                if (chkEnableSecondLambdaSensor.Checked != chkEnableSecondLambdaSensor2.Checked)
                {
                    chkEnableSecondLambdaSensor.BackColor = Color.Orange;
                    chkEnableSecondLambdaSensor2.BackColor = Color.Orange;
                }
                if (chkEnrichmentAfterStart.Checked != chkEnrichmentAfterStart2.Checked)
                {
                    chkEnrichmentAfterStart.BackColor = Color.Orange;
                    chkEnrichmentAfterStart2.BackColor = Color.Orange;
                }
                if (chkEnrichmentDuringStart.Checked != chkEnrichmentDuringStart2.Checked)
                {
                    chkEnrichmentDuringStart.BackColor = Color.Orange;
                    chkEnrichmentDuringStart2.BackColor = Color.Orange;
                }
                if (chkETS.Checked != chkETS2.Checked)
                {
                    chkETS.BackColor = Color.Orange;
                    chkETS2.BackColor = Color.Orange;
                }
                if (chkFuelAdjustDuringIdle.Checked != chkFuelAdjustDuringIdle2.Checked)
                {
                    chkFuelAdjustDuringIdle.BackColor = Color.Orange;
                    chkFuelAdjustDuringIdle2.BackColor = Color.Orange;
                }
                if (chkFuelcutEngineBrake.Checked != chkFuelcutEngineBrake2.Checked)
                {
                    chkFuelcutEngineBrake.BackColor = Color.Orange;
                    chkFuelcutEngineBrake2.BackColor = Color.Orange;
                }
                if (chkGlobalAdaption.Checked != chkGlobalAdaption2.Checked)
                {
                    chkGlobalAdaption.BackColor = Color.Orange;
                    chkGlobalAdaption2.BackColor = Color.Orange;
                }
                if (chkHeatPlates.Checked != chkHeatPlates2.Checked)
                {
                    chkHeatPlates.BackColor = Color.Orange;
                    chkHeatPlates2.BackColor = Color.Orange;
                }
                if (chkHigherIdleOnStart.Checked != chkHigherIdleOnStart2.Checked)
                {
                    chkHigherIdleOnStart.BackColor = Color.Orange;
                    chkHigherIdleOnStart2.BackColor = Color.Orange;
                }
                if (chkIdleControl.Checked != chkIdleControl2.Checked)
                {
                    chkIdleControl.BackColor = Color.Orange;
                    chkIdleControl2.BackColor = Color.Orange;
                }
                if (chkIdleIgnitionGear12.Checked != chkIdleIgnitionGear122.Checked)
                {
                    chkIdleIgnitionGear12.BackColor = Color.Orange;
                    chkIdleIgnitionGear122.BackColor = Color.Orange;
                }
                if (chkKnockDetectionOff.Checked != chkKnockDetectionOff2.Checked)
                {
                    chkKnockDetectionOff.BackColor = Color.Orange;
                    chkKnockDetectionOff2.BackColor = Color.Orange;
                }
                if (chkLambdaControl.Checked != chkLambdaControl2.Checked)
                {
                    chkLambdaControl.BackColor = Color.Orange;
                    chkLambdaControl2.BackColor = Color.Orange;
                }
                if (chkLambdaControlDuringIdle.Checked != chkLambdaControlDuringIdle2.Checked)
                {
                    chkLambdaControlDuringIdle.BackColor = Color.Orange;
                    chkLambdaControlDuringIdle2.BackColor = Color.Orange;
                }
                if (chkLambdaControlOnTransients.Checked != chkLambdaControlOnTransients2.Checked)
                {
                    chkLambdaControlOnTransients.BackColor = Color.Orange;
                    chkLambdaControlOnTransients2.BackColor = Color.Orange;
                }
                if (chkLambdaCorrectionOnACEngage.Checked != chkLambdaCorrectionOnACEngage2.Checked)
                {
                    chkLambdaCorrectionOnACEngage.BackColor = Color.Orange;
                    chkLambdaCorrectionOnACEngage2.BackColor = Color.Orange;
                }
                if (chkLambdaCorrectionOnTPSOpening.Checked != chkLambdaCorrectionOnTPSOpening2.Checked)
                {
                    chkLambdaCorrectionOnTPSOpening.BackColor = Color.Orange;
                    chkLambdaCorrectionOnTPSOpening2.BackColor = Color.Orange;
                }
                if (chkLoadBufferOnIdle.Checked != chkLoadBufferOnIdle2.Checked)
                {
                    chkLoadBufferOnIdle.BackColor = Color.Orange;
                    chkLoadBufferOnIdle2.BackColor = Color.Orange;
                }
                if (chkLoadControl.Checked != chkLoadControl2.Checked)
                {
                    chkLoadControl.BackColor = Color.Orange;
                    chkLoadControl2.BackColor = Color.Orange;
                }
                if (chkNoFuelcutR12.Checked != chkNoFuelcutR122.Checked)
                {
                    chkNoFuelcutR12.BackColor = Color.Orange;
                    chkNoFuelcutR122.BackColor = Color.Orange;
                }
                if (chkNormallyAspirated.Checked != chkNormallyAspirated2.Checked)
                {
                    chkNormallyAspirated.BackColor = Color.Orange;
                    chkNormallyAspirated2.BackColor = Color.Orange;
                }
                if (chkPurgeControl.Checked != chkPurgeControl2.Checked)
                {
                    chkPurgeControl.BackColor = Color.Orange;
                    chkPurgeControl2.BackColor = Color.Orange;
                }
                if (chkPurgeValveMY94.Checked != chkPurgeValveMY942.Checked)
                {
                    chkPurgeValveMY94.BackColor = Color.Orange;
                    chkPurgeValveMY942.BackColor = Color.Orange;
                }
                if (chkTemperatureCompensation.Checked != chkTemperatureCompensation2.Checked)
                {
                    chkTemperatureCompensation.BackColor = Color.Orange;
                    chkTemperatureCompensation2.BackColor = Color.Orange;
                }
                if (chkTemperatureCorrectionInClosedLoop.Checked != chkTemperatureCorrectionInClosedLoop2.Checked)
                {
                    chkTemperatureCorrectionInClosedLoop.BackColor = Color.Orange;
                    chkTemperatureCorrectionInClosedLoop2.BackColor = Color.Orange;
                }
                if (chkUseIdleInjectionMap.Checked != chkUseIdleInjectionMap2.Checked)
                {
                    chkUseIdleInjectionMap.BackColor = Color.Orange;
                    chkUseIdleInjectionMap2.BackColor = Color.Orange;
                }
                if (chkVSSCode.Checked != chkVSSCode2.Checked)
                {
                    chkVSSCode.BackColor = Color.Orange;
                    chkVSSCode2.BackColor = Color.Orange;
                }
                if (chkTankDiagnostics.Checked != chkTankDiagnostics2.Checked)
                {
                    chkTankDiagnostics.BackColor = Color.Orange;
                    chkTankDiagnostics2.BackColor = Color.Orange;
                }
                if (chkWOTEnrichment.Checked != chkWOTEnrichment2.Checked)
                {
                    chkWOTEnrichment.BackColor = Color.Orange;
                    chkWOTEnrichment2.BackColor = Color.Orange;
                }
                Invalidate();
                Application.DoEvents();
            }
        }


        public void SetPrimarySourceName(string name)
        {
            groupControl4.Text = "Primary source: " + name;
        }

        public void SetSecondarySourceName(string name)
        {
            groupControl1.Text = "Secondary source: " + name;
        }

        public void DisableSecondarySource()
        {
            groupControl1.Enabled = false;
        }

        private void frmEasyFirmwareInfo_Shown(object sender, EventArgs e)
        {
            
        }


    }
}