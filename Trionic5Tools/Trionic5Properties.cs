using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CommonSuite;

namespace Trionic5Tools
{
    public class Trionic5Properties : IECUProperties
    {
        // contains all firmware options for Trionic 5!

        private DateTime m_syncDateTime = DateTime.MinValue;

        [Category("Car"), Description("Determines when the last synchronization between file and ECU was done")]
        public override DateTime SyncDateTime
        {
            get
            {
                return m_syncDateTime;
            }
            set
            {
                m_syncDateTime = value;
            }
        }

        private int _hardcodedRPMLimit = 7500;

        public int HardcodedRPMLimit
        {
            get { return _hardcodedRPMLimit; }
            set { _hardcodedRPMLimit = value; }
        }


        private bool m_Afterstartenrichment = false;               // byte 0 - bit 1

        /// <summary>
        /// Determines whether or not enrichment after starting should be enabled
        /// </summary>
        [Category("Enrichment"), Description("Determines whether or not enrichment after starting should be enabled")]
        public override bool Afterstartenrichment
        {
            get { return m_Afterstartenrichment; }
            set { m_Afterstartenrichment = value; }
        }
        private bool m_WOTenrichment = false;                      // byte 0 - bit 2

        [Category("Enrichment"), Description("Determines whether or not enrichment should occur at wide open throttle")]
        public override bool WOTenrichment
        {
            get { return m_WOTenrichment; }
            set { m_WOTenrichment = value; }
        }
        private bool m_Interpolationofdelay = false;               // byte 0 - bit 3

        [Category("Misc"), Description("Interpolation of delay parameter")]
        public override bool Interpolationofdelay
        {
            get { return m_Interpolationofdelay; }
            set { m_Interpolationofdelay = value; }
        }
        private bool m_Temperaturecompensation = false;            // byte 0 - bit 4

        [Category("Temperature"), Description("Determines whether or not the system should correct for temperatures")]
        public override bool Temperaturecompensation
        {
            get { return m_Temperaturecompensation; }
            set { m_Temperaturecompensation = value; }
        }

        private bool m_Lambdacontrol = false;                      // byte 0 - bit 5

        [Category("Lambda control"), Description("Turns lambda control on/off")]
        public override bool Lambdacontrol
        {
            get { return m_Lambdacontrol; }
            set { m_Lambdacontrol = value; }
        }
        private bool m_Adaptivity = false;                         // byte 0 - bit 6

        [Category("Adaption"), Description("Turns fuel adaption on/off")]
        public override bool Adaptivity
        {
            get { return m_Adaptivity; }
            set { m_Adaptivity = value; }
        }
        private bool m_Idlecontrol = false;                        // byte 0 - bit 7

        [Category("Idle"), Description("Turns specific idle control on/off")]
        public override bool Idlecontrol
        {
            get { return m_Idlecontrol; }
            set { m_Idlecontrol = value; }
        }
        private bool m_Enrichmentduringstart = false;              // byte 0 - bit 8

        [Category("Enrichment"), Description("Turns enrichment during start (cranking) on/off")]
        public override bool Enrichmentduringstart
        {
            get { return m_Enrichmentduringstart; }
            set { m_Enrichmentduringstart = value; }
        }
        private bool m_ConstantinjectiontimeE51 = false;           // byte 1 - bit 1

        [Category("Fuelling"), Description("Turns a constant injection time on/off")]
        public override bool ConstantinjectiontimeE51
        {
            get { return m_ConstantinjectiontimeE51; }
            set { m_ConstantinjectiontimeE51 = value; }
        }
        private bool m_Lambdacontrolduringtransients = false;      // byte 1 - bit 2

        [Category("Lambda control"), Description("Turns closed loop control during transients on/off")]
        public override bool Lambdacontrolduringtransients
        {
            get { return m_Lambdacontrolduringtransients; }
            set { m_Lambdacontrolduringtransients = value; }
        }
        private bool m_Fuelcut = false;                            // byte 1 - bit 3

        [Category("Fuelling"), Description("Turns fuel cut during engine braking on/off")]
        public override bool Fuelcut
        {
            get { return m_Fuelcut; }
            set { m_Fuelcut = value; }
        }
        private bool m_Constantinjtimeduringidle = false;          // byte 1 - bit 4

        [Category("Idle"), Description("Turns the constant injection time during idle on/off")]
        public override bool Constantinjtimeduringidle
        {
            get { return m_Constantinjtimeduringidle; }
            set { m_Constantinjtimeduringidle = value; }
        }
        private bool m_Accelerationsenrichment = false;            // byte 1 - bit 5

        [Category("Enrichment"), Description("Turns enrichment during acceleration on/off")]
        public override bool Accelerationsenrichment
        {
            get { return m_Accelerationsenrichment; }
            set { m_Accelerationsenrichment = value; }
        }

        private bool m_Decelerationsenleanment = false;            // byte 1 - bit 6

        [Category("Enrichment"), Description("Turns enleanment during deceleration on/off")]
        public override bool Decelerationsenleanment
        {
            get { return m_Decelerationsenleanment; }
            set { m_Decelerationsenleanment = value; }
        }
        private bool m_Car104 = false;                             // byte 1 - bit 7

        [Category("Misc"), Description("Car104")]
        public override bool Car104
        {
            get { return m_Car104; }
            set { m_Car104 = value; }
        }
        private bool m_Adaptivitywithclosedthrottle = false;       // byte 1 - bit 8

        [Category("Adaption"), Description("Turns adaption during closed throttle plate on/off")]
        public override bool Adaptivitywithclosedthrottle
        {
            get { return m_Adaptivitywithclosedthrottle; }
            set { m_Adaptivitywithclosedthrottle = value; }
        }
        private bool m_Factortolambdawhenthrottleopening = false;  // byte 2 - bit 1

        [Category("Lambda control"), Description("Turns an allowed correction factor during the opening of the throttle plate to the lambda controller on/off")]
        public override bool Factortolambdawhenthrottleopening
        {
            get { return m_Factortolambdawhenthrottleopening; }
            set { m_Factortolambdawhenthrottleopening = value; }
        }
        private bool m_Usesseparateinjmapduringidle = false;       // byte 2 - bit 2

        [Category("Idle"), Description("Turns the usage of the idle fuel map on/off")]
        public override bool Usesseparateinjmapduringidle
        {
            get { return m_Usesseparateinjmapduringidle; }
            set { m_Usesseparateinjmapduringidle = value; }
        }
        private bool m_FactortolambdawhenACisengaged = false;      // byte 2 - bit 3

        [Category("Lambda control"), Description("Turns an allowed correction factor whilst the airco is engaged to the lambda controller on/off")]
        public override bool FactortolambdawhenACisengaged
        {
            get { return m_FactortolambdawhenACisengaged; }
            set { m_FactortolambdawhenACisengaged = value; }
        }

        private bool m_ThrottleAccRetadjustsimultMY95 = false;     // byte 2 - bit 4

        [Category("Misc"), Description("Unknown test option")]
        public override bool ThrottleAccRetadjustsimultMY95
        {
            get { return m_ThrottleAccRetadjustsimultMY95; }
            set { m_ThrottleAccRetadjustsimultMY95 = value; }
        }

        private bool m_Fueladjustingduringidle = false;            // byte 2 - bit 5

        [Category("Adaption"), Description("Turns fuel adaption during idle on/off")]
        public override bool Fueladjustingduringidle
        {
            get { return m_Fueladjustingduringidle; }
            set { m_Fueladjustingduringidle = value; }
        }
        private bool m_Purge = false;                              // byte 2 - bit 6

        [Category("Purge"), Description("Turns the purge option on/off")]
        public override bool Purge
        {
            get { return m_Purge; }
            set { m_Purge = value; }
        }

        private bool m_Adaptionofidlecontrol = false;              // byte 2 - bit 7

        [Category("Adaption"), Description("Turns adaptive behaviour for idle control on/off")]
        public override bool Adaptionofidlecontrol
        {
            get { return m_Adaptionofidlecontrol; }
            set { m_Adaptionofidlecontrol = value; }
        }
        private bool m_Lambdacontrolduringidle = false;            // byte 2 - bit 8

        [Category("Lambda control"), Description("Turns closed loop control during idle on/off")]
        public override bool Lambdacontrolduringidle
        {
            get { return m_Lambdacontrolduringidle; }
            set { m_Lambdacontrolduringidle = value; }
        }
        private bool m_Heatedplates = false;                       // byte 3 - bit 1

        [Category("Car"), Description("Turns correction factor for cars with heatplates on/off")]
        public override bool Heatedplates
        {
            get { return m_Heatedplates; }
            set { m_Heatedplates = value; }
        }
        private bool m_AutomaticTransmission = false;              // byte 3 - bit 2

        [Category("Car"), Description("Indicates whether the car has an automatic gearbox or not")]
        public override bool AutomaticTransmission
        {
            get { return m_AutomaticTransmission; }
            set { m_AutomaticTransmission = value; }
        }
        private bool m_Loadcontrol = false;                        // byte 3 - bit 3

        [Category("Fuelling"), Description("Turns correction factor for load changes on/off")]
        public override bool Loadcontrol
        {
            get { return m_Loadcontrol; }
            set { m_Loadcontrol = value; }
        }
        private bool m_ETS = false;                                // byte 3 - bit 4

        [Category("Car"), Description("Turns ETS (traction control) on/off")]
        public override bool ETS
        {
            get { return m_ETS; }
            set { m_ETS = value; }
        }
        private bool m_APCcontrol = false;                         // byte 3 - bit 5

        [Category("Boost"), Description("Turns boost control on/off")]
        public override bool APCcontrol
        {
            get { return m_APCcontrol; }
            set { m_APCcontrol = value; }
        }
        private bool m_Higheridleduringstart = false;              // byte 3 - bit 6

        [Category("Idle"), Description("Turns a higher engine speed during starting on/off")]
        public override bool Higheridleduringstart
        {
            get { return m_Higheridleduringstart; }
            set { m_Higheridleduringstart = value; }
        }
        private bool m_Globaladaption = false;                     // byte 3 - bit 7

        [Category("Adaption"), Description("Turns global fuel adaption on/off")]
        public override bool Globaladaption
        {
            get { return m_Globaladaption; }
            set { m_Globaladaption = value; }
        }
        private bool m_Tempcompwithactivelambdacontrol = false;    // byte 3 - bit 8

        [Category("Temperature"), Description("Turns temperature correction in closed loop on/off")]
        public override bool Tempcompwithactivelambdacontrol
        {
            get { return m_Tempcompwithactivelambdacontrol; }
            set { m_Tempcompwithactivelambdacontrol = value; }
        }

        private bool m_Loadbufferduringidle = false;               // byte 4 - bit 1

        [Category("Idle"), Description("Turns load buffering during idle on/off")]
        public override bool Loadbufferduringidle
        {
            get { return m_Loadbufferduringidle; }
            set { m_Loadbufferduringidle = value; }
        }
        private bool m_Constidleignangleduringgearoneandtwo = false;// byte 4 - bit 2

        [Category("Ignition"), Description("Turns a constant ignition advance during first and second gear on/off")]
        public override bool Constidleignangleduringgearoneandtwo
        {
            get { return m_Constidleignangleduringgearoneandtwo; }
            set { m_Constidleignangleduringgearoneandtwo = value; }
        }
        private bool m_NofuelcutR12 = false;                       // byte 4 - bit 3

        [Category("Fuelling"), Description("Turns the fuelcut in engine braking function during reverse, first and second gear on/off")]
        public override bool NofuelcutR12
        {
            get { return m_NofuelcutR12; }
            set { m_NofuelcutR12 = value; }
        }
        private bool m_Airpumpcontrol = false;                     // byte 4 - bit 4

        [Category("Airpump"), Description("Turns the airpump control on/off")]
        public override bool Airpumpcontrol
        {
            get { return m_Airpumpcontrol; }
            set { m_Airpumpcontrol = value; }
        }
        private bool m_Normalasperatedengine = false;              // byte 4 - bit 5

        [Category("Car"), Description("Indicates whether the car is turbocharged or N/A")]
        public override bool Normalasperatedengine
        {
            get { return m_Normalasperatedengine; }
            set { m_Normalasperatedengine = value; }
        }
        private bool m_Knockregulatingdisabled = false;            // byte 4 - bit 6

        [Category("Knock"), Description("Turns knock control on/off")]
        public override bool Knockregulatingdisabled
        {
            get { return m_Knockregulatingdisabled; }
            set { m_Knockregulatingdisabled = value; }
        }
        private bool m_Constantangle = false;                      // byte 4 - bit 7

        [Category("Misc"), Description("Constantangle")]
        public override bool Constantangle
        {
            get { return m_Constantangle; }
            set { m_Constantangle = value; }
        }
        private bool m_PurgevalveMY94 = false;                     // byte 4 - bit 8

        [Category("Purge"), Description("Turns the purge specific function for MY94 cars on/off")]
        public override bool PurgevalveMY94
        {
            get { return m_PurgevalveMY94; }
            set { m_PurgevalveMY94 = value; }
        }

        private string m_VSSCode = string.Empty;

        [Category("VSS/Immo"), Description("The VSS (Vehicle Security System) for the car")]
        public override string VSSCode
        {
            get { return m_VSSCode; }
            set { m_VSSCode = value; }
        }

        private bool m_HasVSSProperties = false;

        [Category("Misc"), Description("Binary has VSS options")]
        public override bool HasVSSOptions
        {
            get { return m_HasVSSProperties; }
            set { m_HasVSSProperties = value; }
        }

        private bool m_IsExtendedProperties = false;

        [Category("Misc"), Description("Binary has extended options")]
        public override bool ExtendedProgramModeOptions
        {
            get { return m_IsExtendedProperties; }
            set { m_IsExtendedProperties = value; }
        }

        private bool m_tank_diagnosticsactive = false;

        [Category("Diagnostics"), Description("Turns Fuel tank pressure diagnostics on/off")]
        public bool Tank_diagnosticsactive
        {
            get { return m_tank_diagnosticsactive; }
            set { m_tank_diagnosticsactive = value; }
        }

        private bool m_vssactive = false;

        [Category("VSS/Immo"), Description("Turns VSS (Vehicle Security System) on/off")]
        public override bool VSSactive
        {
            get { return m_vssactive; }
            set { m_vssactive = value; }
        }

        private string m_carmodel = string.Empty;

        [Category("Car"), Description("The description of the car model")]
        public override string Carmodel
        {
            get { return m_carmodel; }
            set { m_carmodel = value; }
        }

        private string m_enginetype = string.Empty;

        [Category("Car"), Description("The description of the engine type")]
        public override string Enginetype
        {
            get { return m_enginetype; }
            set { m_enginetype = value; }
        }

        private InjectorType m_injectorType = InjectorType.Stock;

        [Category("Car"), Description("Injector type")]
        public override InjectorType InjectorType
        {
            get { return m_injectorType; }
            set { m_injectorType = value; }
        }

        private MapSensorType m_mapsensorType = MapSensorType.MapSensor25;

        [Category("Car"), Description("Mapsensor type")]
        public override MapSensorType MapSensorType
        {
            get { return m_mapsensorType; }
            set { m_mapsensorType = value; }
        }

        private TuningStage _tuningStage = TuningStage.Stock;

        [Category("Car"), Description("Tuning stage")]
        public override TuningStage TuningStage
        {
            get { return _tuningStage; }
            set { _tuningStage = value; }
        }

        private TurboType m_turboType = TurboType.Stock;

        [Category("Car"), Description("Turbo type")]
        public override TurboType TurboType
        {
            get { return m_turboType; }
            set { m_turboType = value; }
        }

        private string m_partnumber = string.Empty;

        [Category("ECU"), Description("The partnumber associated with this binary file")]
        public override string Partnumber
        {
            get { return m_partnumber; }
            set { m_partnumber = value; }
        }
        private string m_softwareID = string.Empty;

        [Category("ECU"), Description("The software ID associated with this binary file")]
        public override string SoftwareID
        {
            get { return m_softwareID; }
            set { m_softwareID = value; }
        }
        private string m_dataname = string.Empty;

        [Category("ECU"), Description("The dataname associated with this binary file")]
        public override string Dataname
        {
            get { return m_dataname; }
            set { m_dataname = value; }
        }

        private string m_CPUspeed = string.Empty;

        [Category("ECU"), Description("The processor speed this binary file initializes the ECU at")]
        public override string CPUspeed
        {
            get { return m_CPUspeed; }
            set { m_CPUspeed = value; }
        }
        private bool m_RAMlocked = false;

        [Category("ECU"), Description("Determines whether or not the SRAM content can be altered through the canbus connection")]
        public override bool RAMlocked
        {
            get { return m_RAMlocked; }
            set { m_RAMlocked = value; }
        }

        private bool m_isTrionic55 = false;

        [Category("ECU"), Description("Indicates whether the file is a Trionic 5.2 file or a Trionic 5.5 file")]
        public override bool IsTrionic55
        {
            get { return m_isTrionic55; }
            set { m_isTrionic55 = value; }
        }

        private bool m_SecondO2Enable = false;

        [Category("Lambda control"), Description("Turns the control of the second (post cat) lambda sonde on/off")]
        public override bool SecondO2Enable
        {
            get { return m_SecondO2Enable; }
            set { m_SecondO2Enable = value; }
        }
    }
}
