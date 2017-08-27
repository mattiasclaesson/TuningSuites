using System;
using System.Collections.Generic;
using System.Text;
using CommonSuite;

namespace Trionic5Tools
{
    /// <summary>
    /// </summary>
    abstract public class IECUFileInformation
    {
        /// <summary>
        /// </summary>

        public abstract int Filelength
        {
            get;
            set;
        }

        public abstract SymbolCollection SymbolCollection
        {
            get;
            set;
        }

        public abstract AddressLookupCollection AddressCollection
        {
            get;
            set;
        }

        public abstract string SRAMfilename
        {
            get;
            set;
        }

        public abstract string Filename
        {
            get;
            set;
        }

        abstract public string GetSymbolNameByAddress(int address);

        abstract public bool Has2DRegKonMat();
        abstract public Int32 GetSymbolAddressSRAM(string symbolname);
        abstract public Int32 GetSymbolAddressFlash(string symbolname);
        abstract public Int32 GetSymbolLength(string symbolname);
        abstract public string GetPressureSymbol();
        abstract public string GetAirTempSymbol();
        abstract public string GetCoolantTempSymbol();
        abstract public string GetProgramModeSymbol();
        abstract public string GetEngineSpeedSymbol();
        abstract public string GetLambdaSymbol();
        abstract public string GetProgramStatusSymbol();
        abstract public string GetInjectorConstant();
        abstract public string GetIgnitionMap();
        abstract public string GetIgnitionKnockMap();
        abstract public string GetIgnitionWarmupMap();
        abstract public string GetBoostRequestMap();
        abstract public string GetBoostBiasMap();
        abstract public string GetFuelcutMap();
        abstract public string GetBatteryCorrectionMap();
        abstract public string GetPFactorsMap();
        abstract public string GetIFactorsMap();
        abstract public string GetDFactorsMap();
        abstract public string GetPFactorsMapAUT();
        abstract public string GetIFactorsMapAUT();
        abstract public string GetDFactorsMapAUT();
        abstract public string GetBoostBiasMapAUT();
        abstract public string GetBoostRequestMapAUT();
        abstract public string GetBoostLimiterFirstGearMapAUT();
        abstract public string GetBoostLimiterFirstGearMap();
        abstract public string GetOpenLoopMap();
        abstract public string GetOpenLoopKnockMap();
        abstract public bool isSixteenBitTable(string symbolname);
        abstract public string GetFirstAfterStartEnrichmentMap();
        abstract public string GetSecondAfterStartEnrichmentMap();

        abstract public string GetIdleTargetRPMMap();
        abstract public string GetIdleIgnition();
        abstract public string GetIdleIgnitionCorrectionMap();
        abstract public string GetIdleFuelMap();

        abstract public string GetBoostLimiterSecondGearMap();
        abstract public string GetKnockLimitMap();
        abstract public string GetBoostKnockMap();
        abstract public string GetKnockSensitivityMap();
        abstract public string GetInjectionDurationSymbol();
        abstract public string GetInjectionMap();
        abstract public string GetInjectionMapLOLA();
        abstract public string GetInjectionKnockMap();
        abstract public string GetEnrichmentForLoadSymbol();
        abstract public string GetEnrichmentForTPSSymbol();
        abstract public string GetEnleanmentForLoadSymbol();
        abstract public string GetEnleanmentForTPSSymbol();
        abstract public string GetIgnitionAdvanceSymbol();
        abstract public string GetKnockOffsetCylinder1Symbol();
        abstract public string GetKnockOffsetCylinder2Symbol();
        abstract public string GetKnockOffsetCylinder3Symbol();
        abstract public string GetKnockOffsetCylinder4Symbol();
        abstract public string GetBoostRequestSymbol();
        abstract public string GetBoostControlOffsetSymbol();
        abstract public string GetBoostTargetSymbol();
        abstract public string GetThrottlePositionSymbol();
        abstract public string GetBoostReductionSymbol();
        abstract public string GetPFactorSymbol();
        abstract public string GetIFactorSymbol();
        abstract public string GetDFactorSymbol();
        abstract public string GetPWMOutputSymbol();
        abstract public string GetKnockCountCylinder1Symbol();
        abstract public string GetKnockCountCylinder2Symbol();
        abstract public string GetKnockCountCylinder3Symbol();
        abstract public string GetKnockCountCylinder4Symbol();
        abstract public string GetKnockLevelSymbol();
        abstract public string GetVehicleSpeedSymbol();
        abstract public string GetTorqueSymbol();
        abstract public string GetKnockOffsetAllCylindersSymbol();
        abstract public XDFCategories GetSymbolCategory(string symbolname);
        abstract public XDFSubCategory GetSymbolSubcategory(string symbolname);
        abstract public string GetSymbolDescription(string symbolname);
    }

}

