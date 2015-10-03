using System;
using System.Collections.Generic;

namespace T7
{
    class SymbolAxesTranslator
    {
        // AirCompCal.AirTab same y axis as AirCompCal.AirLimTab

        public bool GetAxisSymbols(string symbolname, out string x_axis, out string y_axis, out string x_axis_description, out string y_axis_description, out string z_axis_description)
        {
            bool retval = false;
            x_axis = "";
            y_axis = "";
            x_axis_description = "x-axis";
            y_axis_description = "y-axis";
            z_axis_description = "z-axis";
            
            switch (symbolname)
            {
                case "TorqueCal.M_EngTempE85Ta":
                case "TorqueCal.M_EngTempTab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;


                case "MAFCal.WeightConstFuelMap":
                    x_axis = "MAFCal.n_EngineXSP";
                    y_axis = "MAFCal.p_InletGradYSP";
                    x_axis_description = "Engine speed (%)";
                    y_axis_description = "Inlet pressure (kPa)";
                    break;
                case "EngTipCal.X_TipInRFM1Tab":
                case "EngTipCal.X_TipInRFM2Tab":
                case "EngTipCal.X_TipInRFM3Tab":
                case "EngTipCal.X_TipInRFM4Tab":
                case "EngTipCal.X_TipInRFM5Tab":
                    y_axis = "EngTipCal.X_IndAccXSP";
                    break;
                case "EngTipCal.m_TipInMan1Map":
                case "EngTipCal.m_TipInMan2Map":
                case "EngTipCal.m_TipInMan3Map":
                case "EngTipCal.m_TipInMan4Map":
                case "EngTipCal.m_TipInMan5Map":
                case "EngTipCal.m_TipOutMan1Map":
                case "EngTipCal.m_TipOutMan2Map":
                case "EngTipCal.m_TipOutMan3Map":
                case "EngTipCal.m_TipOutMan4Map":
                case "EngTipCal.m_TipOutMan5Map":
                    x_axis = "EngTipCal.m_TipXSP";
                    y_axis = "EngTipCal.n_EngineYSP";
                    y_axis_description = "rpm";
                    break;
                case "EngTipCal.X_TipInRFA1OTab":
                case "EngTipCal.X_TipInRFA2OTab":
                case "EngTipCal.X_TipInRFA3OTab":
                case "EngTipCal.X_TipInRFA4OTab":
                case "EngTipCal.X_TipInRFA5OTab":
                case "EngTipCal.X_TipInRFA3Tab":
                case "EngTipCal.X_TipInRFA4Tab":
                case "EngTipCal.X_TipInRFA5Tab":
                    y_axis = "EngTipCal.X_IndAccXSP";
                    break;

                case "EngTipCal.m_TipInAut1BMap":
                case "EngTipCal.m_TipInAut2BMap":
                case "EngTipCal.m_TipInAut3Map":
                case "EngTipCal.m_TipInAut4Map":
                case "EngTipCal.m_TipInAut5Map":
                case "EngTipCal.m_TipInAut1OMap":
                case "EngTipCal.m_TipInAut2OMap":
                case "EngTipCal.m_TipInAut3OMap":
                case "EngTipCal.m_TipInAut4OMap":
                case "EngTipCal.m_TipInAut5OMap":

                case "EngTipCal.m_TipOutAut3Map":
                case "EngTipCal.m_TipOutAut4Map":
                case "EngTipCal.m_TipOutAut5Map":
                case "EngTipCal.m_TipOutAut1OM":
                case "EngTipCal.m_TipOutAut2OM":
                case "EngTipCal.m_TipOutAut3OM":
                case "EngTipCal.m_TipOutAut4OM":
                case "EngTipCal.m_TipOutAut5OM":
                case "EngTipCal.m_TipOutAut1OMap":
                case "EngTipCal.m_TipOutAut2OMap":
                case "EngTipCal.m_TipOutAut3OMap":
                case "EngTipCal.m_TipOutAut4OMap":
                case "EngTipCal.m_TipOutAut5OMap":

                    x_axis = "EngTipCal.n_EngDiffXSP";
                    y_axis = "EngTipCal.n_GearBoxYSP";
                    break;
                case "IgnE85Cal.fi_AbsMap":
                    x_axis = "IgnNormCal.m_AirXSP";
                    y_axis = "IgnNormCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;

                case "CruiseCal.a_DecReqMap":
                    x_axis = "CruiseCal.v_DiffXSP";
                    y_axis = "CruiseCal.v_ActualYSP";
                    x_axis_description = "km/h";
                    y_axis_description = "km/h";
                    z_axis_description = "m/s^2";
                    break;
                case "TempTiCal.map":
                    x_axis = "TempTiCal.x_axis";
                    y_axis = "TempTiCal.y_axis";
                    x_axis_description = "kPa";
                    y_axis_description = "mg/c";
                    z_axis_description = "ms";
                    break;
                case "AirCtrlCal.map":
                    x_axis = "AirCtrlCal.x_axis";
                    y_axis = "AirCtrlCal.y_axis";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "bit";
                    break;
                case "LambdaCal.MaxLoadE85Tab":
                    y_axis = "LambdaCal.RpmSp";
                    y_axis_description = "rpm";
                    break;
                case "LambdaCal.DecCombMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.DecStepMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "LambdaCal.DecRampMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "LambdaCal.IncCombMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.IncStepMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "LambdaCal.IncRampMap":
                    x_axis = "LambdaCal.XSp";
                    y_axis = "LambdaCal.YSp";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "BFuelCal.Map":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuel correction factor";
                    break;
                case "BFuelCal.StartMap":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuel correction factor";
                    break;
                case "BFuelCal.E85Map":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuel correction factor";
                    break;

                case "BFuelCal2.StartMap":
                    x_axis = "BFuelCal2.AirXSP";
                    y_axis = "BFuelCal2.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuel correction factor";
                    break;
                case "BFuelCal2.Map":
                    x_axis = "BFuelCal2.AirXSP";
                    y_axis = "BFuelCal2.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuel correction factor";
                    break;
                case "StartCal.ScaleFacRpmMap":
                    x_axis = "StartCal.T_EngineXSP";
                    y_axis = "StartCal.n_EngineYSP";
                    x_axis_description = "°C";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuelfactor";
                    break;
                case "StartCal.ScaleFacRpmE85Map":
                    x_axis = "StartCal.T_EngineXSP";
                    y_axis = "StartCal.n_EngineYSP";
                    x_axis_description = "Temp °C";
                    y_axis_description = "rpm";
                    z_axis_description = "Fuelfactor";
                    break;
                case "StartCal.HighAltFacMap":
                    x_axis = "StartCal.T_EngineSP";
                    y_axis = "StartCal.p_HighAltSP";
                    x_axis_description = "°C";
                    y_axis_description = "kPa";
                    z_axis_description = "0";
                    break;
                case "InjAnglCal.Map":
                    x_axis = "InjAnglCal.AirXSP";
                    y_axis = "InjAnglCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "Rpm";
                    z_axis_description = "°";
                    break;
                case "EmLimCal.m_MaxIncrMap":
                    x_axis = "EmLimCal.GearXSP";
                    y_axis = "EmLimCal.n_EngYSP";
                    x_axis_description = "ascii";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "EmLimCal.m_AutMaxIncrMap":
                    x_axis = "EmLimCal.AutGearXSP";
                    y_axis = "EmLimCal.n_EngYSP";
                    x_axis_description = "0";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "EmLimCal.RampFactorMap":
                    x_axis = "EmLimCal.PedDeltaXSP";
                    y_axis = "EmLimCal.X_PedYSP";
                    x_axis_description = "0";
                    y_axis_description = "%";
                    z_axis_description = "0";
                    break;
                case "MAFCal.m_RedundantAirMap":
                    x_axis = "MAFCal.n_EngXSP";
                    y_axis = "MAFCal.LoadYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "MAFCal.cd_ThrottleMap":
                    x_axis = "MAFCal.AreaXSP";
                    y_axis = "MAFCal.PQuoteYSP";
                    x_axis_description = "mm°";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MAFCal.corr_AirFromp":
                    x_axis = "MAFCal.Diagn_EngXSP";
                    y_axis = "MAFCal.DiagLoadYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MAFCal.corr_Count":
                    x_axis = "MAFCal.Diagn_EngXSP";
                    y_axis = "MAFCal.DiagLoadYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MAFCal.Area_Count":
                    x_axis = "MAFCal.AreaXSP";
                    y_axis = "MAFCal.PQuoteYSP";
                    x_axis_description = "0";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "SAICal.m_AirSAIMAP":
                    x_axis = "SAICal.m_AirXSP";
                    y_axis = "SAICal.p_AltCompSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "kPa";
                    z_axis_description = "0";
                    break;
                case "SAICal.FuellingMap":
                    x_axis = "SAICal.m_AirXSP";
                    y_axis = "SAICal.n_EngineYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "SAICal.FuellingMap_2":
                    x_axis = "SAICal.m_AirXSP";
                    y_axis = "SAICal.n_EngineYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "IdleCal.StartRedFacMat":
                    x_axis = "IdleCal.n_EngDiffSP";
                    y_axis = "IdleCal.t_yaxis";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "IgnStartCal.fi_StartMap":
                    x_axis = "IgnStartCal.n_EngXSP";
                    y_axis = "IgnStartCal.T_AirSP";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "°";
                    break;
                case "IgnNormCal.Map":
                    x_axis = "IgnNormCal.m_AirXSP";
                    y_axis = "IgnNormCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;
                case "IgnNormCal2.Map":
                    x_axis = "IgnNormCal.m_AirXSP";
                    y_axis = "IgnNormCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;
                case "TorqueCal.X_AccPedalMap":
                    x_axis = "PedalMapCal.n_EngineMap";
                    y_axis = "TorqueCal.m_PedYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "mg/c";
                    z_axis_description = "%";
                    break;
                case "TorqueCal.M_NominalMap":
                    x_axis = "TorqueCal.m_AirXSP";
                    y_axis = "TorqueCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.fi_IgnLimMap":
                    x_axis = "TorqueCal.m_AirXSP";
                    y_axis = "TorqueCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "˚";
                    break;
                case "TorqueCal.M_IgnInflTorqMap":
                    x_axis = "TorqueCal.m_AirXSP";
                    y_axis = "TorqueCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm/°";
                    break;
                case "TorqueCal.m_AirTorqMap":
                    x_axis = "TorqueCal.M_EngXSP";
                    y_axis = "TorqueCal.n_EngYSP";
                    x_axis_description = "Nm";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "TorqueCal.fi_IgnMinTab":
                    x_axis = "TorqueCal.n_EngMinSP";
                    y_axis = "TorqueCal.T_EngSP";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "°";
                    break;
                case "BoostCal.PMap":
                    x_axis = "BoostCal.PIDXSP";
                    y_axis = "BoostCal.PIDYSP";
                    x_axis_description = "mg/c error";
                    y_axis_description = "rpm";
                    z_axis_description = "P factor";
                    break;
                case "BoostCal.IMap":
                    x_axis = "BoostCal.PIDXSP";
                    y_axis = "BoostCal.PIDYSP";
                    x_axis_description = "mg/c error";
                    y_axis_description = "rpm";
                    z_axis_description = "I factor";
                    break;
                case "BoostCal.DMap":
                    x_axis = "BoostCal.PIDXSP";
                    y_axis = "BoostCal.PIDYSP";
                    x_axis_description = "mg/c error";
                    y_axis_description = "rpm";
                    z_axis_description = "D factor";
                    break;
                case "BoostCal.RegMap":
                    x_axis = "BoostCal.SetLoadXSP";
                    y_axis = "BoostCal.n_EngSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "% PWM";
                    break;
                case "TransCal.AccFacMap":
                    x_axis = "TransCal.AccSP";
                    y_axis = "TransCal.RpmSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "TransCal.DecFacMap":
                    x_axis = "TransCal.DecSP";
                    y_axis = "TransCal.RpmSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "AirMinLimCal.Q_DashPotMap":
                    x_axis = "AirMinLimCal.m_AirXSP";
                    y_axis = "AirMinLimCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "LoadCoCal.Q_ACCompAirMap":
                    x_axis = "LoadCoCal.p_ACSP";
                    y_axis = "LoadCoCal.n_EngineSP";
                    x_axis_description = "Bar";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "LoadCoCal.Q_ACDynamicMap":
                    x_axis = "LoadCoCal.p_ACSP";
                    y_axis = "LoadCoCal.n_EngineSP";
                    x_axis_description = "Bar";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "IgnLOffCal.fi_MapOffset":
                    x_axis = "IgnLOffCal.m_AirXSP";
                    y_axis = "IgnLOffCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "BoostCal.HighAltMap":
                    x_axis = "BoostCal.SetLoad2XSP";
                    y_axis = "BoostCal.HighAltTabSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "kPa";
                    z_axis_description = "%";
                    break;
                case "TCompCal.EnrFacMap":
                case "TCompCal.EnrFacE85Map":
                    x_axis = "TCompCal.EnrFacXSP";
                    y_axis = "TCompCal.EnrFacYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "TCompCal.EnrFacAutMap":
                    x_axis = "TCompCal.EnrFacAutXSP";
                    y_axis = "TCompCal.EnrFacAutYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "IgnOffsCal.DNCompSlow":
                    x_axis = "IgnOffsCal.Q_ExtraSP";
                    y_axis = "IdleCal.T_EngineSP";
                    x_axis_description = "0";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "IgnOffsCal.DNCompFast":
                    x_axis = "IgnOffsCal.Q_ExtraSP";
                    y_axis = "IdleCal.T_EngineSP";
                    x_axis_description = "0";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "TEngDiagCal.m_AirperDegM":
                    x_axis = "TEngDiagCal.T_AirInletSP";
//                    y_axis = "TEngDiagCal.n_CombSP";
                    y_axis = "TEngDiagCal.n_CombSP";
                    x_axis_description = "C";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "TorqueCal.M_EngMaxE85Tab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "TorqueCal.M_EngMaxE85TabAut":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "TorqueCal.M_EngTempE85Tab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm"; 
                    break;
                case "TorqueCal.X_EngTempTab":
                    y_axis = "TorqueCal.T_EngTempSP";
                    y_axis_description = "°C";
                    z_axis_description = "%";
                    break;
                case "TorqueCal.M_PumpLossMap":
                    x_axis = "TorqueCal.m_AirPumpXSP";
                    y_axis = "TorqueCal.n_EngPumpYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TEngDiagCal.m_AirperDegA":
                    x_axis = "TEngDiagCal.T_AirInletSP";
                    y_axis = "TEngDiagCal.n_CombSP";
                    x_axis_description = "C";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "CatDiagCal.HiMapOx1CycleTime":
                    x_axis = "CatDiagCal.LoadTab";
                    y_axis = "CatDiagCal.RpmTab";
                    x_axis_description = "0";
                    y_axis_description = "0";
                    z_axis_description = "ms";
                    break;
                case "CatDiagCal.LoMapOx1CycleTime":
                    x_axis = "CatDiagCal.LoadTab";
                    y_axis = "CatDiagCal.RpmTab";
                    x_axis_description = "0";
                    y_axis_description = "0";
                    z_axis_description = "ms";
                    break;
                case "CatDiagCal.Ox2DevMaxMap":
                    x_axis = "CatDiagCal.LoadTab";
                    y_axis = "CatDiagCal.RpmTab";
                    x_axis_description = "0";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "CatDiagLevCal.ErrorLim":
                    x_axis = "CatDiagLevCal.m_SP";
                    y_axis = "CatDiagLevCal.T_CatSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "C";
                    z_axis_description = "ms";
                    break;
                case "CatModCal.T_SteadyState":
                    x_axis = "CatModCal.m_SP";
                    y_axis = "CatModCal.n_SP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "C";
                    break;
                case "MissfAdap.MissfCntMap":
                    x_axis = "MissfCal.m_AirXSP";
                    y_axis = "MissfCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "# misfires";
                    break;
                case "MissfCal.RPMDiffLimMAP":
                    x_axis = "MissfCal.LoadRPMDiffXSP";
                    y_axis = "MissfCal.n_EngRPMDiffYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "MissfCal.DetectLoadLevel":
                    x_axis = "MissfCal.T_EngXSP";
                    y_axis = "MissfCal.n_EngYSP";
                    x_axis_description = "°C";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "MissfCal.CatOverheatFactor":
                    x_axis = "MissfCal.m_AirXSP";
                    y_axis = "MissfCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "MissfCal.outOfLimDelayMAT":
                    x_axis = "MissfCal.p_AirAmbOutOfLimSP";
                    y_axis = "MissfCal.T_EngOutOfLimSP";
                    x_axis_description = "kPa";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "AirCompCal.PressMap":
                    x_axis = "AirCtrlCal.x_axis";
                    y_axis = "AirCtrlCal.y_axis";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "kPa";
                    break;
                case "IdleCal.Q_IdleOffsMAP":
                    x_axis = "IdleCal.T_EngRPMOffSP";
                    y_axis = "IdleCal.n_RPMOffYSP";
                    x_axis_description = "°C";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "BoostCal.p_DiffILimMap":
                    x_axis = "BoostCal.p_DiffILimXSP";
                    y_axis = "BoostCal.p_DiffILimYSP";
                    x_axis_description = "kPa";
                    y_axis_description = "mg/c";
                    z_axis_description = "0";
                    break;
                case "IgnTempCal.AirMap":
                    x_axis = "IgnTempCal.n_EngXSP";
                    y_axis = "IgnTempCal.T_AirYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "°";
                    break;
                case "IgnTempCal.EngMap":
                    x_axis = "IgnTempCal.n_EngXSP";
                    y_axis = "IgnTempCal.T_EngYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "°";
                    break;
                case "IgnCal.MinMap":
                    x_axis = "IgnCal.m_AirXSP";
                    y_axis = "IgnCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "BstKnkCal.MaxAirmass":
                    x_axis = "BstKnkCal.OffsetXSP";
                    y_axis = "BstKnkCal.n_EngYSP";
                    x_axis_description = "° ignition retard (Ioff)";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "BstKnkCal.MaxAirmassAu":
                    x_axis = "BstKnkCal.OffsetXSP";
                    y_axis = "BstKnkCal.n_EngYSP";
                    x_axis_description = "° ignition retard (Ioff)";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "PedalMapCal.m_RequestMap":
                    x_axis = "PedalMapCal.n_EngineMap";
                    y_axis = "PedalMapCal.X_PedalMap";
                    x_axis_description = "rpm";
                    y_axis_description = "% tps";
                    z_axis_description = "mg/c";
                    break;
                case "IgnIdleCal.fi_IdleMap":
                    x_axis = "IgnIdleCal.m_AirXSP";
                    y_axis = "IgnIdleCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;
                case "LoadCoCal.Q_ColdFricMap":
                    x_axis = "LoadCoCal.n_EngXSP";
                    y_axis = "LoadCoCal.T_EngYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "°C";
                    z_axis_description = "g/s";
                    break;
                case "BFuelAdap.V_FuelConsMap":
                    x_axis = "FuelConsCal.m_AirInlXSP";
                    y_axis = "FuelConsCal.n_EngineYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "ml";
                    break;
                case "KnkAdaptCal.WeightMap1":
                    x_axis = "KnkAdaptCal.m_AirXSP1";
                    y_axis = "KnkAdaptCal.n_EngYSP1";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "KnkAdaptCal.WeightMap2":
                    x_axis = "KnkAdaptCal.m_AirXSP2";
                    y_axis = "KnkAdaptCal.n_EngYSP2";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "KnkDetCal.KnockWinOffs":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkDetCal.RefFactorMap":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Knock sensitivity";
                    break;
                case "KnkDetAdap.KnkCntMap":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "# knocks";
                    break;
                case "IgnKnkCal.IndexMap":
                    x_axis = "IgnKnkCal.m_AirXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° pull";
                    break;
                case "KnkFuelCal.fi_MapMaxOff":
                    x_axis = "KnkFuelCal.m_AirXSP";
                    y_axis = "BstKnkCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkFuelCal.WeightTab":
                    x_axis = "KnkFuelCal.WeightXSP";
                    y_axis = "KnkFuelCal.WeightYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "KnkFuelCal.EnrichmentMap":
                    x_axis = "IgnKnkCal.m_AirXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Enrichment factor";
                    break;
                case "KnkFuelCal.fi_MapMaxOffset":
                    x_axis = "IgnKnkCal.m_AirXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "TqToHiCal.Q_AirInletMap":
                    x_axis = "TqToHiCal.T_AirInletXSP";
                    y_axis = "TqToHiCal.X_AccPedYSP";
                    x_axis_description = "C";
                    y_axis_description = "%";
                    z_axis_description = "g/s";
                    break;
                case "SwitchCal.A_SwitchPoint":
                    x_axis = "SwitchCal.T_Engine";
                    y_axis = "SwitchCal.n_Engine";
                    x_axis_description = "°C";
                    y_axis_description = "rpm";
                    z_axis_description = "mm^2";
                    break;
                case "SwitchCal.A_AmbPresMap":
                    x_axis = "SwitchCal.n_EngXSP";
                    y_axis = "SwitchCal.p_AmbientYSP";
                    x_axis_description = "rpm";
                    y_axis_description = "kPa";
                    z_axis_description = "mm2";
                    break;
                case "KnkSoundRedCal.fi_OffsMap":
                    x_axis = "IgnNormCal.m_AirXSP";
                    y_axis = "IgnNormCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "AftSt1ExtraCal.EnrFacMap":
                    x_axis = "AftStCal1.T_EngineSP";
                    y_axis = "AftSt1ExtraCal.p_AirAmbSP";
                    x_axis_description = "Temp °C";
                    y_axis_description = "Pressure kPa";
                    z_axis_description = "";
                    break;
                case "AftSt2ExtraCal.EnrFacMap":
                    x_axis = "AftStCal2.T_EngineSP";
                    y_axis = "AftSt2ExtraCal.p_AirAmbSP";
                    x_axis_description = "Temp °C";
                    y_axis_description = "Pressure kPa";
                    z_axis_description = "";
                    break;
                case "AftSt2ExtraCal.EnrMapE85":
                    x_axis = "AftSt2ExtraCal.ActualE85SP";
                    y_axis = "AftStCal2.T_EngineSP";
                    x_axis_description = "E85 percentage";
                    y_axis_description = "Temp °C";
                    z_axis_description = "";
                    break;
                case "PurgeCal.ValveMap16":
                    x_axis = "PurgeCal.p_Diff16XSp";
                    y_axis = "PurgeCal.q_Flow16YSp";
                    x_axis_description = "kPa";
                    y_axis_description = "mg/s";
                    z_axis_description = "%";
                    break;
                case "PurgeCal.ValveMap8":
                    x_axis = "PurgeCal.p_Diff8XSp";
                    y_axis = "PurgeCal.q_Flow8YSp";
                    x_axis_description = "kPa";
                    y_axis_description = "mg/s";
                    z_axis_description = "%";
                    break;
                case "ExhaustCal.T_Lambda1Map":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°C";
                    break;
                case "ExhaustCal.fi_IgnMap":
                    x_axis = "ExhaustCal.m_AirSP";
                    y_axis = "ExhaustCal.fi_IgnSP";
                    x_axis_description = "0";
                    y_axis_description = "°";
                    z_axis_description = "°C";
                    break;
                case "IgnNormCal.GasMap":
                    x_axis = "IgnNormCal.m_AirXSP";
                    y_axis = "IgnNormCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;
                case "F_KnkDetAdap.RKnkCntMap":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "# knocks";
                    break;
                case "F_KnkDetAdap.FKnkCntMap":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "# knocks";
                    break;

                case "ShiftSupCal.M_HystTab":
                    y_axis = "ShiftSupCal.M_RLoadSP";
                    y_axis_description = "Nm";
                    z_axis_description = "Nm";
                    break;
                case "PwmLimitCal.PwmLimit":
                    y_axis = "PwmLimitCal.U_Batt";
                    y_axis_description = "V";
                    z_axis_description = "%";
                    break;
                case "CruiseComCal.M_RoadTorque":
                    y_axis = "CruiseComCal.v_Actual";
                    y_axis_description = "km/h";
                    z_axis_description = "Nm";
                    break;
                case "CruiseCal.v_LimitPos":
                    y_axis = "CruiseCal.v_RequestNormal";
                    y_axis_description = "km/h";
                    z_axis_description = "km/h";
                    break;
                case "CruiseCal.v_LimitNeg":
                    y_axis = "CruiseCal.v_RequestNormal";
                    y_axis_description = "km/h";
                    z_axis_description = "km/h";
                    break;
                case "CruiseCal.v_LimitNormal":
                    y_axis = "CruiseCal.v_RequestNormal";
                    y_axis_description = "km/h";
                    z_axis_description = "km/h";
                    break;
                case "CruiseCal.a_AccRequest":
                    y_axis = "CruiseComCal.v_Delta";
                    y_axis_description = "km/h";
                    z_axis_description = "m/s^2";
                    break;
                case "CruiseCal.a_DecRequest":
                    y_axis = "CruiseComCal.v_Delta";
                    y_axis_description = "km/h";
                    z_axis_description = "m/s^2";
                    break;
                case "CruiseComCal.M_Offset":
                    y_axis = "CruiseComCal.v_Delta";
                    y_axis_description = "km/h";
                    z_axis_description = "%";
                    break;
                case "CruiseCal.M_GradientPos":
                    y_axis = "CruiseCal.M_GradActual";
                    y_axis_description = "Nm";
                    z_axis_description = "Nm";
                    break;
                case "CruiseCal.M_GradientNeg":
                    y_axis = "CruiseCal.M_GradActual";
                    y_axis_description = "Nm";
                    z_axis_description = "Nm";
                    break;
                case "AirCtrlCal.m_MaxAirTab":
                case "AirCtrlCal.m_MaxAirE85Ta":
                case "AirCtrlCal.m_MaxAirPetTa":
                    y_axis = "AirCtrlCal.y_axis";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "TempLimPosCal.Limit":
                    y_axis = "TempLimPosCal.Airmass";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "AreaCal.Table":
                    y_axis = "AreaCal.Area";
                    y_axis_description = "mm^2";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.N_BeforeHW":
                    y_axis = "LambdaCal.TempSp";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.CombNrTab2":
                    y_axis = "LambdaCal.TempSp";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.TempCthrTab":
                    y_axis = "LambdaCal.TempSp";
                    y_axis_description = "°C";
                    z_axis_description = "°C";
                    break;
                case "LambdaCal.TempOthrTab":
                    y_axis = "LambdaCal.TempSp";
                    y_axis_description = "°C";
                    z_axis_description = "°C";
                    break;
                case "LambdaCal.N_TransDelay":
                    y_axis = "LambdaCal.TempSp";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.MinLoadTab":
                    y_axis = "LambdaCal.RpmSp";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "LambdaCal.MaxLoadTimeTab":
                    y_axis = "LambdaCal.RpmSp";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "LambdaCal.MaxLoadNormTab":
                    y_axis = "LambdaCal.RpmSp";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "LambdaCal.LeanStep":
                    y_axis = "LambdaCal.U_AdjStepSP";
                    y_axis_description = "mV";
                    z_axis_description = "%";
                    break;
                case "LambdaCal.RichStep":
                    y_axis = "LambdaCal.U_AdjStepSP";
                    y_axis_description = "mV";
                    z_axis_description = "%";
                    break;
                case "InjCorrCal.BattCorrTab":
                    y_axis = "InjCorrCal.BattCorrSP";
                    y_axis_description = "V";
                    z_axis_description = "0";
                    break;
                case "AftStCal1.EnrFacTab":
                case "AftStCal1.EnrFacE85Tab":
                    y_axis = "AftStCal2.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "AftStCal1.DecrDelayTab":
                case "AftStCal1.DecrDelayE85Tab":
                    y_axis = "AftStCal1.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "AftStCal2.EnrFacTab":
                case "AftStCal2.EnrFacE85Tab":
                    y_axis = "AftStCal2.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "AftStCal2.DecrDelayTab":
                    y_axis = "AftStCal2.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "StartCal.m_FuelBefStart":
                    y_axis = "StartCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "mg";
                    break;
                case "StartCal.m_ReStBefFuel":
                    y_axis = "StartCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "mg";
                    break;
                case "TCompCal.EnrFacTab":
                case "TCompCal.EnrFacE85Tab":
                    y_axis = "TCompCal.T_EngineAutSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "TCompCal.EnrFacAutTab":
                    y_axis = "TCompCal.T_EngineAutSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "StartCal.CombFacTab":
                    y_axis = "StartCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "StartCal.EnrFacTab":
                case "StartCal.EnrFacE85Tab":
                    y_axis = "StartCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "StartCal.RestartFacTab":
                    y_axis = "StartCal.t_RestartSP";
                    y_axis_description = "sec";
                    z_axis_description = "0";
                    break;
                case "MAFCal.t_PosTransFreezTab":
                    y_axis = "MAFCal.n_EngineXSP";
                    y_axis_description = "rpm";
                    z_axis_description = "ms";
                    break;
                case "MAFCal.t_NegTransFreezTab":
                    y_axis = "MAFCal.n_EngineXSP";
                    y_axis_description = "rpm";
                    z_axis_description = "ms";
                    break;
                case "MAFCal.ConstT_AirInlTab":
                    y_axis = "MAFCal.T_EngineSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MAFCal.ConstT_EngineTab":
                    y_axis = "MAFCal.T_EngineSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "SAICal.N_ClosedLoopDelay":
                    y_axis = "SAICal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "comb";
                    break;
                case "SAICal.N_PumpStart":
                    y_axis = "SAICal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "comb";
                    break;
                case "SAICal.m_AirInjReq":
                    y_axis = "SAICal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "mg/c";
                    break;
                case "SAICal.N_FuellingStart":
                    y_axis = "SAICal.p_AltCompSP";
                    y_axis_description = "°C";
                    z_axis_description = "comb";
                    break;
                case "SAICal.AltComp":
                    y_axis = "SAICal.p_AltCompSP";
                    y_axis_description = "°C";
                    z_axis_description = "comb";
                    break;
                case "MaxSpdCal.n_EngLimAir":
                    y_axis = "MaxSpdCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "rpm";
                    break;
                case "IdleCal.C_PartDrive":
                    y_axis = "IdleCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.C_PartNeutral":
                    y_axis = "IdleCal.T_EngSP";
                    y_axis_description = "°C";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.I_PartTab":
                    y_axis = "IdleCal.P_PartSP";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.P_PartTab":
                    y_axis = "IdleCal.P_PartSP";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "IdleCal.Q_StartOffsTab":
                    y_axis = "IdleCal.p_AltStartSP";
                    y_axis_description = "kPa";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.Q_AltStartTAB":
                    y_axis = "IdleCal.p_AltStartSP";
                    y_axis_description = "kPa";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.n_EngNomDrive":
                    y_axis = "IdleCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "rpm";
                    break;
                case "IdleCal.n_EngNomNeutral":
                    y_axis = "IdleCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "rpm";
                    break;
               /* case "IgnKnkCal.ReduceTime":
                    y_axis = "16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description = "0";
                    z_axis_description = "°";
                    break;
                case "GearCal.Range":
                    y_axis = "16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;*/
                case "TorqueCal.M_ManGearLim":
                    //y_axis = "TorqueCal.n_Eng5GearSP";
                    y_axis_description = "gear";
                    //y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_CabGearLim":
                    y_axis = "TorqueCal.n_Eng5GearSP";
                    //y_axis_description = "rpm";
                    y_axis_description = "gear";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_5GearLimTab":
                    y_axis = "TorqueCal.n_Eng5GearSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_ReverseTab":
                    //y_axis = "TorqueCal.n_Eng1GearSP";
                    y_axis = "TorqueCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_1GearTab":
                    y_axis = "TorqueCal.n_Eng1GearSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_EngMaxTab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TorqueCal.M_EngMaxAutTab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "BoostCal.P_LimTab":
                    y_axis = "BoostCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "BoostCal.I_LimTab":
                    y_axis = "BoostCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "TorqueCal.M_OverBoostTab":
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                case "TransCal.m_TriggMaxTab":
                    y_axis = "TransCal.DecTriggSP";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "TransCal.AccTriggLim":
                    y_axis = "TransCal.DecTriggSP";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "TransCal.DecTriggLim":
                    y_axis = "TransCal.DecTriggSP";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "TransCal.AccIdleTriggLim":
                    y_axis = "TransCal.DecTriggSP";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "TransCal.DecIdleTriggLim":
                    y_axis = "TransCal.DecTriggSP";
                    y_axis_description = "0";
                    z_axis_description = "mg/c";
                    break;
                case "AirMinLimCal.Q_MinLoadTab":
                    y_axis = "AirMinLimCal.v_VehicleSP";
                    y_axis_description = "km/h";
                    z_axis_description = "g/s";
                    break;
                case "AirMinLimCal.p_AirAmbTab":
                    y_axis = "AirMinLimCal.v_VehicleSP";
                    y_axis_description = "km/h";
                    z_axis_description = "kPa";
                    break;
                case "AirMinLimCal.v_VehicleTab":
                    y_axis = "AirMinLimCal.v_VehicleSP";
                    y_axis_description = "km/h";
                    z_axis_description = "km/h";
                    break;
                case "LoadCoCal.t_ACOnDlyTab":
                    y_axis = "LoadCoCal.n_EngineSP";
                    y_axis_description = "rpm";
                    z_axis_description = "ms";
                    break;
                case "LoadCoCal.t_ACOffDlyTab":
                    y_axis = "LoadCoCal.n_EngineSP";
                    y_axis_description = "rpm";
                    z_axis_description = "ms";
                    break;
                case "LoadCoCal.Q_ElLoadCoTab":
                    y_axis = "LoadCoCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "LoadCoCal.t_ElLoadRampTab":
                    y_axis = "LoadCoCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "ms";
                    break;
                case "TransCal.AccTempFacTab":
                    y_axis = "TransCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "TransCal.DecTempFacTab":
                    y_axis = "TransCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "FCutCal.FuelFactor":
                    y_axis = "FCutCal.n_CombSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "BoostCal.PercAdapTab":
                    y_axis = "BoostCal.m_AirAdapSP";
                    y_axis_description = "mg/c";
                    z_axis_description = "%";
                    break;
                case "IgnLOffCal.CombTab":
                    y_axis = "IgnLOffCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "IgnLOffCal.N_AirTab":
                    y_axis = "IgnLOffCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "BoostCal.AntiHowlPresTab":
                    y_axis = "BoostCal.AntiHowlPairSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "BoostCal.PWMTempTab":
                    y_axis = "BoostCal.PWMTempSP";
                    y_axis_description = "°C";
                    z_axis_description = "%";
                    break;
                case "EvapDiagCal.CalcFac":
                    y_axis = "EvapDiagCal.RampSumTab";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "EvapDiagCal.LeakFacTest1MAT":
                    x_axis = "EvapDiagCal.Test1Ramp2ResSP";
                    y_axis = "EvapDiagCal.V_Test1FuelSP";
                    x_axis_description = "";
                    y_axis_description = "Volume (milliLiter)";
                    z_axis_description = "mg/c";
                    break;
                case "EvapDiagCal.LeakFacTest2MAT":
                    x_axis = "EvapDiagCal.Test2Ramp2ResSP";
                    y_axis = "EvapDiagCal.V_Test2FuelSP";
                    x_axis_description = "";
                    y_axis_description = "Volume (milliLiter)";
                    z_axis_description = "mg/c";
                    break;
                case "IdleCal.NeutralAirPartTAB":
                    y_axis = "EvapDiagCal.RampSumTab";
                    y_axis_description = "0";
                    z_axis_description = "%";
                    break;
                case "QAirDiagCal.HFMOffSim":
                    y_axis = "QAirDiagCal.HFMOffSimSP";
                    y_axis_description = "g/s";
                    z_axis_description = "0";
                    break;
                case "TEngDiagCal.T_startSP":
                    y_axis = "TEngDiagCal.m_LoadFacSP";
                    y_axis_description = "0";
                    z_axis_description = "C";
                    break;
                case "TEngDiagCal.m_LoadFacTAB":
                    y_axis = "TEngDiagCal.m_LoadFacSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "O2SPreCal.t_O2SFrontTime":
                    y_axis = "O2SPreCal.T_O2SFrontTemp";
                    y_axis_description = "C";
                    z_axis_description = "ms";
                    break;
                case "CatModCal.T_Offset":
                    y_axis = "CatModCal.T_SPCombStart";
                    y_axis_description = "C";
                    z_axis_description = "C";
                    break;
                case "CatModCal.n_CombStart":
                    y_axis = "CatModCal.T_SPCombStart";
                    y_axis_description = "C";
                    z_axis_description = "0";
                    break;
                case "MissfCal.LeanStep":
                    y_axis = "LambdaCal.U_AdjStepSP";
                    y_axis_description = "mV";
                    z_axis_description = "%";
                    break;
                case "MissfCal.RichStep":
                    y_axis = "LambdaCal.U_AdjStepSP";
                    y_axis_description = "mV";
                    z_axis_description = "%";
                    break;
                case "MissfCal.nrOfTransientFilterCombust":
                    y_axis = "MissfCal.EngTempSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MissfCal.startDelayTAB":
                    y_axis = "MissfCal.T_EngSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "MissfCal.M_LimTab":
                    y_axis = "MissfCal.n_EngineSP";
                    y_axis_description = "rpm";
                    z_axis_description = "Nm";
                    break;
                /*case "AirCompCal.AirTab":
                    y_axis = "AirCompCal.p_PresSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;*/
                case "AirCompCal.PresTab":
                    y_axis = "AirCompCal.p_PresSP";
                    y_axis_description = "0";
                    z_axis_description = "0";
                    break;
                case "IdleCal.DelayDriveAct":
                    y_axis = "IdleCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "ms";
                    break;
                case "IdleCal.DelayDriveRel":
                    y_axis = "IdleCal.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "ms";
                    break;
                /*case "GearCal.n_SULAtGear":
                    y_axis = "4 : 1 2 3 4  ";
                    y_axis_description = "ascii";
                    z_axis_description = "rpm";
                    break;
                case "GearCal.m_SULAtGear":
                    y_axis = "4 : 1 2 3 4  ";
                    y_axis_description = "ascii";
                    z_axis_description = "rpm";
                    break;
                case "GearCal.m_SULAtGear":
                    y_axis = "4 : 1 2 3 4  ";
                    y_axis_description = "ascii";
                    z_axis_description = "rpm";
                    break;*/
                case "HotStCal1.EnrFacTab":
                case "HotStCal1.EnrFacE85Tab":
                case "HotStCal1.DecrDelayTab":
                    y_axis = "HotStCal1.T_EngineSP";
                    y_axis_description = "Temp °C";
                    z_axis_description = "0";
                    break;
                case "HotStCal2.RestartFacTab":
                    y_axis = "HotStCal2.t_RestartSP";
                    y_axis_description = "sec";
                    z_axis_description = "0";
                    break;
                case "TransCal2.EnrFacTab":
                case "TransCal2.EnrFacE85Tab":
                    y_axis = "TransCal2.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "TransCal2.DecrDelayTab":
                    y_axis = "TransCal2.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "SyncCal.n_CombMultiIgnTab":
                    y_axis = "SyncCal.T_EngMultiIgnSP";
                    y_axis_description = "°C";
                    z_axis_description = "comb";
                    break;
                case "IdleCal.LOffTab":
                    y_axis = "IdleCal.LOffSP";
                    y_axis_description = "°";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.X_mLOffHighAltTab":
                    y_axis = "IdleCal.n_RpmDiffLOffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "LambdaCal.HeatLoadLimTab":
                    y_axis = "LambdaCal.HeatRpmSP";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "LambdaCal.t_HeatFront":
                    y_axis = "LambdaCal.HeatRpmSP";
                    y_axis_description = "rpm";
                    z_axis_description = "s";
                    break;
                case "LambdaCal.PWM_HeatO2FrontSens":
                    y_axis = "LambdaCal.HeatRpmSP";
                    y_axis_description = "rpm";
                    z_axis_description = "%";
                    break;
                case "LambdaCal.t_HeatRear":
                    y_axis = "LambdaCal.HeatRpmSP";
                    y_axis_description = "rpm";
                    z_axis_description = "s";
                    break;
                case "LambdaCal.PWM_HeatO2RearSens":
                    y_axis = "LambdaCal.HeatRpmSP";
                    y_axis_description = "rpm";
                    z_axis_description = "%";
                    break;
                case "TransCal3.EnrFacTab":
                case "TransCal3.EnrFacE85Tab":
                    y_axis = "TransCal3.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "TransCal3.DecrDelayTab":
                    y_axis = "TransCal3.T_EngineSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "FuelConsCal.Tab":
                    y_axis = "FuelConsCal.AirSP";
                    y_axis_description = "mg/c";
                    z_axis_description = "0";
                    break;
                case "AirCompCal.AirLimTab":
                case "AirCompCal.AirTab":
                    y_axis = "AirCompCal.T_AirLimSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "X_AccPedalAutTAB":
                    //y_axis = "X_AccPedalManSP";
                    y_axis = "X_AccPedalAutSP";
                    y_axis_description = "AD";
                    z_axis_description = "%";
                    break;
                case "X_AccPedalManTAB":
                    y_axis = "X_AccPedalManSP";
                    y_axis_description = "AD";
                    z_axis_description = "%";
                    break;
                case "TransCal.AccRampFac":
                    y_axis = "TransCal.RpmSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "TransCal.DecRampFac":
                    y_axis = "TransCal.RpmSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "LimEngCal.TurboSpeedTab":
                    //y_axis = "LimEngCal.n_EngSP";
                    y_axis = "LimEngCal.p_AirSP";
                    y_axis_description = "Ambient air pressure (kPa)";
                    z_axis_description = "mg/c";
                    break;
                case "LimEngCal.TurboSpeedTab2":
                    y_axis = "LimEngCal.n_EngSP";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "KnkAdaptCal.ConstantTab":
                    y_axis = "KnkAdaptCal.fi_OffsetSP";
                    y_axis_description = "°";
                    z_axis_description = "0";
                    break;
                case "KnkDetCal.X_AverageTab":
                    y_axis = "KnkAdaptCal.fi_OffsetSP";
                    y_axis_description = "°";
                    z_axis_description = "0";
                    break;
                case "IdleCal.n_EngLOffDrive":
                    y_axis = "IdleCal.n_EngLOffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "rpm";
                    break;
                case "IdleCal.n_EngLOffNeutral":
                    y_axis = "IdleCal.n_EngLOffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "rpm";
                    break;
                case "IdleCal.Q_LOffRpmDrive":
                    y_axis = "IdleCal.n_EngLOffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.Q_LOffRpmNeutral":
                    y_axis = "IdleCal.n_EngLOffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.T_AirTab":
                    y_axis = "IdleCal.p_AirAmbSP";
                    y_axis_description = "kPa";
                    z_axis_description = "g/s";
                    break;
                case "IdleCal.Q_AirAmbTab":
                    y_axis = "IdleCal.p_AirAmbSP";
                    y_axis_description = "kPa";
                    z_axis_description = "g/s";
                    break;
                case "PedalMapCal.X_AutFacTab":
                    y_axis = "PedalMapCal.v_VehManSP";
                    y_axis_description = "km/h";
                    z_axis_description = "0";
                    break;
                case "PedalMapCal.X_ManFacTab":
                    y_axis = "PedalMapCal.v_VehManSP";
                    y_axis_description = "km/h";
                    z_axis_description = "0";
                    break;
                case "PedalMapCal.X_CabFacTab":
                    y_axis = "PedalMapCal.v_VehManSP";
                    y_axis_description = "km/h";
                    z_axis_description = "0";
                    break;
                case "IgnIdleCal.Tab":
                    //y_axis = "IgnIdleCal.n_EngMinSP";
                    y_axis = "IgnIdleCal.n_EngDiffSP";
                    y_axis_description = "rpm";
                    z_axis_description = "° BDTC";
                    break;
                case "IgnIdleCal.fi_MinTab":
                    y_axis = "IgnIdleCal.n_EngMinSP";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnJerkCal.n_LimTab":
                    y_axis = "IgnJerkCal.n_LimTabSP";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "IgnJerkCal.fi_Tab":
                    y_axis = "IgnJerkCal.n_DiffXSP";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "DiffPSCal.M_LimitTab":
                    //y_axis = "DiffPSCal.M_EngineSP";
                    y_axis = "DiffPSCal.v_DiffSP";
                    //y_axis_description = "Nm";
                    z_axis_description = "0.1 km/h";
                    break;
                case "DiffPSCal.t_LimDelayTab":
                    y_axis = "DiffPSCal.M_EngineSP";
                    y_axis_description = "Nm";
                    z_axis_description = "ms";
                    break;
                case "KnkAdaptCal.MaxRef":
                    y_axis = "KnkAdaptCal.fi_OffsetSP";
                    y_axis_description = "°";
                    z_axis_description = "0";
                    break;
               /* case "KnkDetCal.KnockWinAngle":
                    y_axis = "14 : 0 500 1000 1500 2000 2500 3000 3500 4000 4500 5000 5500 6000 6500";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;*/
                case "IgnKnkCal.fi_Offset":
                    y_axis = "IgnKnkCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnKnkCal.AdpLowLimTab":
                    y_axis = "IgnKnkCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "IgnKnkCal.AdpHighLimTab":
                    y_axis = "IgnKnkCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "IgnKnkCal.fi_Map1MaxOffset":
                    y_axis = "IgnKnkCal.n_EngYSP";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
              /*  case "IgnKnkCal.ReduceTime":
                    y_axis = "16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description = "0";
                    z_axis_description = "°";
                    break;*/
                case "DisplAdap.LamScannerTab1":
                    y_axis = "DisplAdap.AD_ScannerSP";
                    y_axis_description = "AD";
                    z_axis_description = "Lam";
                    break;
                case "DisplAdap.LamScannerTab2":
                    y_axis = "DisplAdap.AD_ScannerSP";
                    y_axis_description = "AD";
                    z_axis_description = "Lam";
                    break;
                case "DisplAdap.LamScannerTab3":
                    y_axis = "DisplAdap.AD_ScannerSP";
                    y_axis_description = "AD";
                    z_axis_description = "Lam";
                    break;
                case "SAIDiagCal.m_AirTab":
                    y_axis = "SAIDiagCal.m_AirSP";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "SAIDiagCal.m_WSHAirTab1":
                    y_axis = "SAIDiagCal.m_WSHAirSP";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "SAIDiagCal.m_WSHAirTab2":
                    y_axis = "SAIDiagCal.m_WSHAirSP";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "AreaCal.Q_VenturiTab":
                    y_axis = "AreaCal.Q_VenturiSP";
                    y_axis_description = "g/s";
                    z_axis_description = "g/s";
                    break;
                case "JerkCal.T_AirInletTab":
                    y_axis = "JerkCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "JerkCal.n_EngineTab":
                    y_axis = "JerkCal.n_EngineSP";
                    y_axis_description = "rpm";
                    z_axis_description = "0";
                    break;
                case "JerkCal.p_AirAmbientTab":
                    y_axis = "JerkCal.p_AirAmbientSP";
                    y_axis_description = "kPa";
                    z_axis_description = "kPa";
                    break;
                case "TorqueCal.a_FreeRollingTab":
                    y_axis = "TorqueCal.v_TorqueCalcSP";
                    y_axis_description = "km/h";
                    z_axis_description = "m/s^2";
                    break;
                case "REPCal.T_AirInletTab":
                    y_axis = "REPCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "kPa";
                    break;
                case "BlockHeatCal.AftSt2FacTab":
                    y_axis = "BlockHeatCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "BlockHeatCal.Q_StartFacTab":
                    y_axis = "BlockHeatCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "BlockHeatCal.C_PartNFacTab":
                    y_axis = "BlockHeatCal.T_AirInletSP";
                    y_axis_description = "°C";
                    z_axis_description = "0";
                    break;
                case "PurgeCal.PdiffMaxRefFlow":
                    y_axis = "PurgeCal.p_Diff16XSp";
                    y_axis_description = "kPa";
                    z_axis_description = "mg/s";
                    break;
                case "PurgeCal.m_AirDerLowTab":
                    y_axis = "PurgeCal.m_AirHighSp";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "PurgeCal.m_AirDerHighTab":
                    y_axis = "PurgeCal.m_AirHighSp";
                    y_axis_description = "mg/c";
                    z_axis_description = "mg/c";
                    break;
                case "ExhaustCal.m_AirTau1Tab":
                    y_axis = "ExhaustCal.m_AirSP";
                    y_axis_description = "°C";
                    z_axis_description = "s";
                    break;
                case "ExhaustCal.m_AirTau2Tab":
                    y_axis = "ExhaustCal.m_AirSP";
                    y_axis_description = "°C";
                    z_axis_description = "s";
                    break;
                case "ExhaustCal.T_LimitTab":
                    y_axis = "ExhaustCal.T_LimitSP";
                    y_axis_description = "°C";
                    z_axis_description = "°C";
                    break;
                case "ExhaustCal.T_ExhaustTab":
                    y_axis = "ExhaustCal.T_ExhaustSP";
                    y_axis_description = "°C";
                    z_axis_description = "°C";
                    break;
                case "KnkDetCal.T_EngKnkCntrl":
                    y_axis = "KnkDetCal.T_InletAirXSP";
                    y_axis_description = "°";
                    z_axis_description = "°";
                    break;
                case "VIOSMAFCal.Q_AirInletTab":
                    y_axis = "VIOSMAFCal.TicsSP";
                    y_axis_description = "CPUTics: frequency = 13107800/VIOSMAFCal.TicsSP";
                    z_axis_description = "g/s";
                    break;
                case "E85FSCal.V_IncTrigLim":
                    y_axis = "E85FSCal.V_IncTrigSP";
                    y_axis_description = "DL";
                    z_axis_description = "Volume DL";
                    break;
                case "E85FSCal.V_IncStaLimTab":
                    y_axis = "E85FSCal.V_IncStableSP";
                    y_axis_description = "DL";
                    z_axis_description = "Volume DL";
                    break;
                case "HotStCal2.RestartMap":
                    x_axis = "HotStCal2.t_RestartYSP";
                    y_axis = "HotStCal2.RestartE85XSP";
                    x_axis_description = "Time/Seconds";
                    y_axis_description = "SoakTime";
                    z_axis_description = "EnrFactor";
                    break;
                case "LoadCoCal.Q_DynElLoadTab":
                    y_axis = "LoadCoCal.I_LoadSP";
                    y_axis_description = "Current Amp";
                    z_axis_description = "Electric load";
                    break;
                case "IgnLOffCal.n_CombAftSt":
                    y_axis = "IgnLOffCal.T_CombAftStSP";
                    y_axis_description = "temp °C /10";
                    z_axis_description = "combustions";
                    break;
                case "TorqueTab.m_01": // KurtMW
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "TorqueTab.m_02": // KurtMW
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "TorqueTab.m_03": // KurtMW
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "TorqueTab.m_Actual": // KurtMW
                    y_axis = "TorqueCal.n_EngYSP";
                    y_axis_description = "rpm";
                    break;
                case "E85CldStCal.EnrMap":
                    x_axis = "E85CldStCal.T_EngineSP";
                    y_axis = "E85CldStCal.n_CombustionTab";
                    x_axis_description = "Temp °C";
                    y_axis_description = "Number of combustions";
                    z_axis_description = "";
                    break;
                case "TorqueCal.X_IgnEfficTab":
                    y_axis = "TorqueCal.fi_MBTOffsYSP";
                    y_axis_description = "";
                    break;
                case "BoostCal.pwm_E85adjTab":
                    y_axis = "BoostCal.PIDYSP";
                    y_axis_description = "";
                    break;
                case "IdleCal.X_FirstDrvFuelTAB":
                    y_axis = "IdleCal.T_EngFirstDrvSP";
                    y_axis_description = "Temp °C";
                    break;
                case "IdleCal.X_FirstDrvE85FuelTAB":
                    y_axis = "IdleCal.T_EngFirstDrvSP";
                    y_axis_description = "Temp °C";
                    break;
                case "IdleCal.n_DeltaTAB":
                    y_axis = "IdleCal.n_DeltaSP";
                    y_axis_description = "";
                    break;
                case "E85Cal.X_EthanolActEnrFacMap":
                    x_axis = "E85Cal.X_EthanolActualXSP";
                    y_axis = "E85Cal.T_EngTempYSP";
                    x_axis_description = "E85 percentage";
                    y_axis_description = "Temp °C";
                    z_axis_description = "";
                    break;
                case "BFuelCal.WWPressCompBetaTab":
                    y_axis = "BFuelCal.p_WWPressCompSP";
                    y_axis_description = "";
                    break;
                case "BFuelCal.WWCompRampTab":
                    y_axis = "BFuelCal.T_WWCompSP";
                    y_axis_description = "Temp °C";
                    break;
                case "BFuelCal.WWTempCompBetaTab":
                    y_axis = "BFuelCal.T_WWCompSP";
                    y_axis_description = "Temp °C";
                    break;
                case "BFuelCal.WWCompAlphaTab":
                    y_axis = "BFuelCal.T_WWCompSP";
                    y_axis_description = "Temp °C";
                    break;
            }
            if(z_axis_description.StartsWith("0")) z_axis_description = "z-axis";
            if(x_axis_description.StartsWith("0")) x_axis_description = "x-axis";
            if(y_axis_description.StartsWith("0")) y_axis_description = "y-axis";
            if (y_axis != "") retval = true;
            return retval;
        }

        //TODO: Translate to Trionic 7
        public string GetXaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                retval = "Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                retval = "Ign_map_1_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                retval = "Ign_map_2_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                retval = "Ign_map_3_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                retval = "Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                retval = "Ign_map_5_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                retval = "Ign_map_6_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                retval = "Ign_map_7_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                retval = "Ign_map_8_x_axis!";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                retval = "Overs_tab_xaxis!";
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                retval = "Dash_trot_axis!";
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Adapt_korr"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                retval = "Idle_st_last!";
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                retval = "Trans_x_st!";
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                retval = "Reg_last!";
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                retval = "Reg_last!";
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                retval = "Fuel_knock_xaxis!";
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                retval = "Misfire_map_x_axis!";
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                retval = "Misfire_map_x_axis!";
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                retval = "Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                retval = "Temp_reduce_x_st!";
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                retval = "Detect_map_x_axis!";
            }
            else if ((symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!")))
            {
                retval = "Pwm_ind_trot!";
            }
            return retval;
        }

        public string GetYaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                retval = "Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                retval = "Ign_map_1_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                retval = "Ign_map_2_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                retval = "Ign_map_3_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                retval = "Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                retval = "Ign_map_5_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                retval = "Ign_map_6_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                retval = "Ign_map_7_y_axis!";
            }
            else if (symbolname.StartsWith("Open_loop_adapt!") || symbolname.StartsWith("Open_loop!") || symbolname.StartsWith("Open_loop_knock!"))
            {
                retval = "Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Fload_tab!") || symbolname.StartsWith("Fload_throt_tab!"))
            {
                retval = "Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                retval = "Ign_map_8_y_axis!";
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                retval = "Temp_steg";
            }
            else if (symbolname.StartsWith("Kyltemp_tab!"))
            {
                retval = "Kyltemp_steg!";
            }
            else if (symbolname.StartsWith("Lufttemp_tab!"))
            {
                retval = "Lufttemp_steg!";
            }
            else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                retval = "Lufttemp_steg!";
            }
            else if (symbolname.StartsWith("Derivata_br_tab_pos!") || symbolname.StartsWith("Derivata_br_tab_neg!"))
            {
                retval = "Derivata_br_sp!";
            }
            else if (symbolname.StartsWith("I_last_rpm!") || symbolname.StartsWith("Last_reg_ac!"))
            {
                retval = "Last_varv_st!";
            }
            else if (symbolname.StartsWith("I_last_temp!"))
            {
                retval = "Last_temp_st!";
            }
            else if (symbolname.StartsWith("Iv_start_time_tab!"))
            {
                retval = "I_kyl_st!";
            }
            else if (symbolname.StartsWith("Idle_start_extra!"))
            {
                retval = "Idle_start_extra_sp!";
            }
            else if (symbolname.StartsWith("Restart_corr_hp!"))
            {
                retval = "Hp_support_points!";
            }
            else if (symbolname.StartsWith("Lam_minlast!"))
            {
                retval = "Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Lamb_tid!") || symbolname.StartsWith("Lamb_idle!") || symbolname.StartsWith("Lamb_ej!"))
            {
                retval = "Lamb_kyl!";
            }
            else if (symbolname.StartsWith("Overstid_tab!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("AC_wait_on!") || symbolname.StartsWith("AC_wait_off!"))
            {
                retval = "I_luft_st!";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Idle_temp_off!") || symbolname.StartsWith("Idle_rpm_tab!") || symbolname.StartsWith("Start_tab!"))
            {
                retval = "I_kyl_st!";
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Max_regl_temp_"))
            {
                retval = "Max_regl_sp!";
            }
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                retval = "Knock_wind_rpm!";
            }
            else if (symbolname.StartsWith("Knock_ref_tab!"))
            {
                retval = "Knock_ref_rpm!";
            }
            else if (symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_press_tab!") || symbolname.StartsWith("Lknock_oref_tab!") || symbolname.StartsWith("Knock_lim_tab!"))
            {
                retval = "Wait_count_tab!";
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                retval = "Dash_rpm_axis!";
            }
            else if (symbolname.StartsWith("Adapt_korr"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                retval = "Idle_st_rpm!";
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                retval = "Trans_y_st!";
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                retval = "Trans_y_st!";
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                retval = "Trans_y_st!";
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                retval = "Trans_y_st!";
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                retval = "Reg_varv!";
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                retval = "Reg_varv!";
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Regl_tryck_sgm!") || symbolname.StartsWith("Regl_tryck_fgm!") || symbolname.StartsWith("Regl_tryck_fgaut!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                retval = "Misfire_map_y_axis!";
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                retval = "Misfire_map_y_axis!";
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_") || symbolname.StartsWith("Tempkomp_konst!") || symbolname.StartsWith("Accel_temp!") || symbolname.StartsWith("Accel_temp2!") || symbolname.StartsWith("Retard_temp!") || symbolname.StartsWith("Throt_after_tab!") || symbolname.StartsWith("Throt_aft_dec_fak!"))
            {
                retval = "Temp_steg!";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                retval = "Temp_reduce_y_st!";
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                retval = "Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                retval = "Detect_map_y_axis!";
            }
            else if (symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!"))
            {
                retval = "Pwm_ind_rpm!";
            }
            return retval;
        }

    }
}
