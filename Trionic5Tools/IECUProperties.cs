using System;
using System.Collections.Generic;
using System.Text;
using CommonSuite;

namespace Trionic5Tools
{
    /// <summary>
    /// </summary>
    abstract public class IECUProperties
    {
        /// <summary>
        /// </summary>
        public abstract bool Afterstartenrichment
        {
            get;
            set;
        }


        public abstract bool HasVSSOptions
        {
            get;
            set;
        }

        public abstract bool ExtendedProgramModeOptions
        {
            get;
            set;
        }
        

        public abstract InjectorType InjectorType
        {
            get;
            set;
        }

        public abstract MapSensorType MapSensorType
        {
            get;
            set;
        }

        public abstract TurboType TurboType
        {
            get;
            set;
        }

        public abstract TuningStage TuningStage
        {
            get;
            set;
        }

        public abstract DateTime SyncDateTime
        {
            get;
            set;
        }

        public abstract bool WOTenrichment
        {
            get;
            set;
        }

        public abstract bool Interpolationofdelay
        {
            get;
            set;
        }

        public abstract bool Temperaturecompensation
        {
            get;
            set;
        }

        public abstract bool Lambdacontrol
        {
            get;
            set;
        }
        public abstract bool Adaptivity
        {
            get;
            set;
        }
        public abstract bool Idlecontrol
        {
            get;
            set;
        }
        public abstract bool Enrichmentduringstart
        {
            get;
            set;
        }
        public abstract bool ConstantinjectiontimeE51
        {
            get;
            set;
        }
        public abstract bool Lambdacontrolduringtransients
        {
            get;
            set;
        }
        public abstract bool Fuelcut
        {
            get;
            set;
        }
        public abstract bool Constantinjtimeduringidle
        {
            get;
            set;
        }
        public abstract bool Accelerationsenrichment
        {
            get;
            set;
        }
        public abstract bool Decelerationsenleanment
        {
            get;
            set;
        }
        public abstract bool Car104
        {
            get;
            set;
        }
        public abstract bool Adaptivitywithclosedthrottle
        {
            get;
            set;
        }
        public abstract bool Factortolambdawhenthrottleopening
        {
            get;
            set;
        }
        public abstract bool Usesseparateinjmapduringidle
        {
            get;
            set;
        }
        public abstract bool FactortolambdawhenACisengaged
        {
            get;
            set;
        }
        public abstract bool ThrottleAccRetadjustsimultMY95
        {
            get;
            set;
        }
        public abstract bool Fueladjustingduringidle
        {
            get;
            set;
        }
        public abstract bool Purge
        {
            get;
            set;
        }
        public abstract bool Adaptionofidlecontrol
        {
            get;
            set;
        }
        public abstract bool Lambdacontrolduringidle
        {
            get;
            set;
        }
        public abstract bool Heatedplates
        {
            get;
            set;
        }
        public abstract bool AutomaticTransmission
        {
            get;
            set;
        }
        public abstract bool Loadcontrol
        {
            get;
            set;
        }
        public abstract bool ETS
        {
            get;
            set;
        }
        public abstract bool APCcontrol
        {
            get;
            set;
        }
        public abstract bool Higheridleduringstart
        {
            get;
            set;
        }
        public abstract bool Globaladaption
        {
            get;
            set;
        }
        public abstract bool Tempcompwithactivelambdacontrol
        {
            get;
            set;
        }
        public abstract bool Loadbufferduringidle
        {
            get;
            set;
        }
        public abstract bool Constidleignangleduringgearoneandtwo
        {
            get;
            set;
        }
        public abstract bool NofuelcutR12
        {
            get;
            set;
        }
        public abstract bool Airpumpcontrol
        {
            get;
            set;
        }
        public abstract bool Normalasperatedengine
        {
            get;
            set;
        }
        public abstract bool Knockregulatingdisabled
        {
            get;
            set;
        }
        public abstract bool Constantangle
        {
            get;
            set;
        }
        public abstract bool PurgevalveMY94
        {
            get;
            set;
        }
        public abstract string VSSCode
        {
            get;
            set;
        }
        public abstract bool VSSactive
        {
            get;
            set;
        }
        public abstract string Carmodel
        {
            get;
            set;
        }
        public abstract string Enginetype
        {
            get;
            set;
        }
        public abstract string Partnumber
        {
            get;
            set;
        }
        public abstract string SoftwareID
        {
            get;
            set;
        }
        public abstract string Dataname
        {
            get;
            set;
        }
        public abstract string CPUspeed
        {
            get;
            set;
        }
        public abstract bool RAMlocked
        {
            get;
            set;
        }
        public abstract bool IsTrionic55
        {
            get;
            set;
        }
        public abstract bool SecondO2Enable
        {
            get;
            set;
        }
        
        


    }
}

