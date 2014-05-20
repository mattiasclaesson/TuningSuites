using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class ProgramModeSettings
    {
        private bool _PurgeControl = true;

        public bool PurgeControl
        {
            get { return _PurgeControl; }
            set { _PurgeControl = value; }
        }

        private bool _Lambdacontrol = true;

        public bool Lambdacontrol
        {
            get { return _Lambdacontrol; }
            set { _Lambdacontrol = value; }
        }

        private bool _AcclerationEnrichment = true;

        public bool AcclerationEnrichment
        {
            get { return _AcclerationEnrichment; }
            set { _AcclerationEnrichment = value; }
        }
        private bool _DecelerationEnleanment = true;

        public bool DecelerationEnleanment
        {
            get { return _DecelerationEnleanment; }
            set { _DecelerationEnleanment = value; }
        }
        private bool _WOTEnrichment = true;

        public bool WOTEnrichment
        {
            get { return _WOTEnrichment; }
            set { _WOTEnrichment = value; }
        }
        private bool _Fuelcut = true;

        public bool Fuelcut
        {
            get { return _Fuelcut; }
            set { _Fuelcut = value; }
        }
        private bool _LoadControl = true;

        public bool LoadControl
        {
            get { return _LoadControl; }
            set { _LoadControl = value; }
        }
        private bool _NoFuelcutR12 = true;

        public bool NoFuelcutR12
        {
            get { return _NoFuelcutR12; }
            set { _NoFuelcutR12 = value; }
        }
        private bool _ConstIdleIgnitionAngleGearOneAndTwo = true;

        public bool ConstIdleIgnitionAngleGearOneAndTwo
        {
            get { return _ConstIdleIgnitionAngleGearOneAndTwo; }
            set { _ConstIdleIgnitionAngleGearOneAndTwo = value; }
        }

        private bool _UseSeperateInjectionMapForIdle = true;

        public bool UseSeperateInjectionMapForIdle
        {
            get { return _UseSeperateInjectionMapForIdle; }
            set { _UseSeperateInjectionMapForIdle = value; }
        }

        /*
Acclerationsenrichment OFF				                BYTE 1 MASK 0x10
Decelerationsenleanment OFF				                BYTE 1 MASK 0x20
WOTenrichment OFF					                    BYTE 0 MASK 0x02
Fuelcut OFF						                        BYTE 1 MASK 0x04
Loadcontrol OFF						                    BYTE 3 MASK 0x04
NoFuelcutR12 ON						                    BYTE 4 MASK 0x04
Constidleignangleduringgearoneandtwo OFF (permanent)	BYTE 4 MASK 0x02
 * */

    }
}
