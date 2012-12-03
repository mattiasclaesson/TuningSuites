using System;
using System.Collections.Generic;
using System.Text;

namespace T8SuitePro
{
    class SymbolAxesTranslator
    {
       

        public bool GetAxisSymbols(string symbolname, out string x_axis, out string y_axis, out string x_axis_description, out string y_axis_description, out string z_axis_description)
        {
            bool retval = false;
            x_axis ="";
            y_axis ="";
            x_axis_description ="x-axis";
            y_axis_description ="y-axis";
            z_axis_description ="z-axis";
            switch (symbolname)
            {
                case"AirAmbientCal.F_FrictionMaxTAB":
                    x_axis ="";
                    y_axis ="AirAmbientCal.F_FrictionSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirAmbientCal.F_FrictionTAB":
                    x_axis ="";
                    y_axis ="AirAmbientCal.F_FrictionSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirAmbientCal.p_ThrRatioSP":
                    x_axis ="AirAmbientCal.n_EngineSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirAmbientCal.X_PrAdjustEst":
                    x_axis ="";
                    y_axis ="AirAmbientCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_ReqAdapXSP":
                    x_axis ="AirCtrlCal.n_EngAdapYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.ThrFlowFacTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.p_ThrRatioSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.ThrTempFacTAB":
                    x_axis ="";
                    y_axis ="AirCtrlCal.T_AirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_MaxAirTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.y_axis";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.x_axis":
                    x_axis ="AirCtrlCal.y_axis";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AreaCal.Table":
                    x_axis ="";
                    y_axis ="AreaCal.Area";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.Ppart_BoostLimTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.Ipart_BoostLimTab":
                    x_axis ="";
                    y_axis ="16 : 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.PIDXSP":
                    x_axis ="AirCtrlCal.PIDYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.SetLoadXSP":
                    x_axis ="AirCtrlCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_IPartThrLimNegTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.I_PartThrLimSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_IPartThrLimPosTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.I_PartThrLimSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_AirInletXSP":
                    x_axis ="AirCtrlCal.T_AirInletYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.p_AfterTurbineXSP":
                    x_axis ="AirCtrlCal.p_AirAmbientYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_diffPReqXSP":
                    x_axis ="AirCtrlCal.PIDYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.p_AirAmbientXSP":
                    x_axis ="AirCtrlCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.BasicPresTempCompTAB":
                    x_axis ="";
                    y_axis ="AirCtrlCal.T_AirInletSP2";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.PRatioMaxTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.q_AirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.m_AirCtrlDFacTAB":
                    x_axis ="";
                    y_axis ="AirCtrlCal.m_AirCtrlDFacSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.ThrFlowFacSSTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.p_ThrRatioSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.ThrFlowFacTransTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.p_ThrRatioSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirCtrlCal.TempFacTab":
                    x_axis ="";
                    y_axis ="AirCtrlCal.T_AirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirMassMastCal.Trq_MBTXSP":
                    x_axis ="AirMassMastCal.n_EngMBTYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BoostAdapCal.m_AirXSP":
                    x_axis ="BoostAdapCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BoostCal.p_AntiHowlTab":
                    x_axis ="";
                    y_axis ="BoostCal.p_AntiHowlPairSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BstKnkCal.OffsetXSP":
                    x_axis ="BstKnkCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.n_EngTEngFacSP":
                    x_axis ="MAFCal.p_InlTEngFacSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.m_AirNormAdjXSP":
                    x_axis ="MAFCal.n_EngNormAdjYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.n_EngSP":
                    x_axis ="MAFCal.fi_CamOverlapSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.K_FrompTAirinletTab":
                    x_axis ="";
                    y_axis ="MAFCal.T_AirInlSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.K_FrompTEngineTab":
                    x_axis ="";
                    y_axis ="MAFCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.AreaXSP":
                    x_axis ="MAFCal.PQuoteYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.t_PosTransFreezTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngineXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.t_NegTransFreezTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngineXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.t_PosTransFreezMainTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngineXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.t_NegTransFreezMainTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngineXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.C_PosTransPredictorTab":
                    x_axis ="";
                    y_axis ="MAFCal.X_pRatioSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.C_NegTransPredictorTab":
                    x_axis ="";
                    y_axis ="MAFCal.X_pRatioSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.FrompMulFacTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.FrompAddFacTab":
                    x_axis ="";
                    y_axis ="MAFCal.n_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MAFCal.n_EngineXSP":
                    x_axis ="MAFCal.p_InletGradYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FrompAdapCal.m_AirXSP":
                    x_axis ="FrompAdapCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirMinLimCal.T_EngineSP":
                    x_axis ="AirMinLimCal.n_EngineSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartEngCal.T_EngineXSP":
                    x_axis ="StartEngCal.p_AirAmbientYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartEngCal.n_EngineDiffXSP":
                    x_axis ="StartEngCal.T_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.m_AirXSP":
                    x_axis ="MisfCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TAmbModCal.v_VehicleSP":
                    x_axis ="TAmbModCal.m_AirInletSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatModCal.T_AirInletXSP":
                    x_axis ="CatModCal.t_SoakMinYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatModCal.m_SteadyStateXSP":
                    x_axis ="CatModCal.n_SteadyStateYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatModCal.T_LambdaOffsetTAB":
                    x_axis ="";
                    y_axis ="CatModCal.LambdaSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAIDiagCal.SumLimitTAB":
                    x_axis ="";
                    y_axis ="SAIDiagCal.N_SamplesSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.Trq_OffsetTAB":
                    x_axis ="";
                    y_axis ="MisfCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatDiagCal.LoadTab":
                    x_axis ="CatDiagCal.RpmTab";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SPreDiagCal.t_O2SStartTAB":
                    x_axis ="";
                    y_axis ="O2SPreDiagCal.T_O2SStartSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"EvapDiagCal.Ramp2ResultXSP":
                    x_axis ="EvapDiagCal.V_Test1FuelYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.T_EngXSP":
                    x_axis ="MisfCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.m_Air2XSP":
                    x_axis ="MisfCal.n_Eng2YSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.n_CombFilterTAB":
                    x_axis ="";
                    y_axis ="MisfCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfCal.n_CombFromStartTAB":
                    x_axis ="";
                    y_axis ="MisfCal.T_EngXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatDiagCal.T_CatMulFacTAB":
                    x_axis ="";
                    y_axis ="CatDiagCal.T_CatSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatDiagCal.LoadMulFacTAB":
                    x_axis ="";
                    y_axis ="CatDiagCal.Q_AirMeanSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SRespDiagCal.CombMulFacTAB":
                    x_axis ="";
                    y_axis ="O2SRespDiagCal.n_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelDeterminationCal.V_TankEUTAB":
                    x_axis ="";
                    y_axis ="FuelDeterminationCal.V_TankEUSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelDeterminationCal.V_TankUSTAB":
                    x_axis ="";
                    y_axis ="FuelDeterminationCal.V_TankUSSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CatDiagCal.Q_AirSP":
                    x_axis ="CatDiagCal.T_CatSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal.AirXSP":
                    x_axis ="BFuelCal.RpmYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal.t_JerkDelayTab":
                    x_axis ="";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal.m_AirJerkTab":
                    x_axis ="";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal.JerkEnrichFacTab":
                    x_axis ="";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal.AirFCIXSP":
                    x_axis ="BFuelCal.RpmFCIYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BlockHeatCal.T_DiffBlkHeatStartTAB":
                    x_axis ="";
                    y_axis ="BlockHeatCal.t_SoakMinutesSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BlockHeatCal.StartFuelFacTAB":
                    x_axis ="";
                    y_axis ="BlockHeatCal.T_BlkHeatSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BlockHeatCal.AftStFuelFacTAB":
                    x_axis ="";
                    y_axis ="BlockHeatCal.T_BlkHeatSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BlockHeatCal.Trq_FricOffsetTAB":
                    x_axis ="";
                    y_axis ="BlockHeatCal.T_BlkHeatSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BlockHeatCal.Q_StartOffsetTAB":
                    x_axis ="";
                    y_axis ="BlockHeatCal.T_BlkHeatSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ExhaustCal.t_mAirTau1Tab":
                    x_axis ="";
                    y_axis ="ExhaustCal.m_AirSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ExhaustCal.t_mAirTau2Tab":
                    x_axis ="";
                    y_axis ="ExhaustCal.m_AirSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ExhaustCal.m_AirSP":
                    x_axis ="ExhaustCal.fi_IgnSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_CombSinceFuelCutSP":
                    x_axis ="FCutCal.T_EngineSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.WeightFactorTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_CombInFuelCutSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.FuelFactorCatDiagTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_CombInFuelCutSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_PedalToRpmTAB":
                    x_axis ="";
                    y_axis ="FCutCal.X_AccPedalSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.RpmToFCutTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_EngOffLimTab":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.A_ThrottleTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.t_OpenMaxTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_EngOffLimAutTab":
                    x_axis ="";
                    y_axis ="11 : 0 1 2 3 4 5 6 7 8 9 10 11";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_EngineKillTAB":
                    x_axis ="";
                    y_axis ="FCutCal.v_EngineKillSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FCutCal.n_EngOffOpenTAB":
                    x_axis ="";
                    y_axis ="FCutCal.n_EngOffOpenSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelDynCal.FuelModFacTab":
                    x_axis ="";
                    y_axis ="FuelDynCal.n_EngCycleCntSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelDynCal.n_EngineSP":
                    x_axis ="FuelDynCal.m_AirInletSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;

                case"FuelDynCal.n_Engine1SP":
                    x_axis ="FuelDynCal.m_AirInlet1SP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                
                case"FMastCal.X_FuelConsFacTAB":
                    x_axis ="";
                    y_axis ="FMastCal.m_AirFuelConsSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FMastCal.m_AirInletFuelXSP":
                    x_axis ="FMastCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"InjAnglCal.AirXSP":
                    x_axis ="InjAnglCal.RpmYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                
                case"InjAnglCal.X_InjAnglRampTab":
                    x_axis ="";
                    y_axis ="InjAnglCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"InjCorrCal.BattCorrTab":
                    x_axis ="";
                    y_axis ="InjCorrCal.BattCorrSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.m_AirXSP":
                    x_axis ="IgnKnkCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"KnkFuelCal.m_AirXSP":
                    x_axis ="BstKnkCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"KnkFuelCal.WeightXSP":
                    x_axis ="KnkFuelCal.WeightYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.N_ST15DelayTAB":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.rampTempScaleTAB":
                    x_axis ="";
                    y_axis ="LambdaCal.T_rampTempSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.XSp":
                    x_axis ="LambdaCal.YSp";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                
                case"LambdaCal.CombNrTab2":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.TempCthrTab":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.TempOthrTab":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.N_TransDelay":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.LeanStep":
                    x_axis ="";
                    y_axis ="LambdaCal.U_AdjStepSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.LeanStepMisfire":
                    x_axis ="";
                    y_axis ="LambdaCal.U_AdjStepMisfireSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.RichStep":
                    x_axis ="";
                    y_axis ="LambdaCal.U_AdjStepSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.RichStepMisfire":
                    x_axis ="";
                    y_axis ="LambdaCal.U_AdjStepMisfireSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.N_BeforeHW":
                    x_axis ="";
                    y_axis ="LambdaCal.TempSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SHeatFrontCal.n_DewPresentTab":
                    x_axis ="";
                    y_axis ="O2SHeatFrontCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SHeatRearCal.n_DewPresentTab":
                    x_axis ="";
                    y_axis ="O2SHeatRearCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SHeatFrontCal.m_CutOffTAB":
                    x_axis ="";
                    y_axis ="O2SHeatFrontCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"O2SHeatRearCal.m_CutOffTAB":
                    x_axis ="";
                    y_axis ="O2SHeatRearCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"PurgeCal.p_Diff16XSp":
                    x_axis ="PurgeCal.q_Flow16YSp";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"PurgeCal.PdiffMaxRefFlowEUTab":
                    x_axis ="";
                    y_axis ="PurgeCal.p_Diff16XSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"PurgeCal.PdiffMaxRefFlowUSTab":
                    x_axis ="";
                    y_axis ="PurgeCal.p_Diff16XSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAICal.T_EngineSP":
                    x_axis ="SAICal.p_AltCompSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAICal.N_PumpStart":
                    x_axis ="";
                    y_axis ="SAICal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAICal.N_FuellingStart":
                    x_axis ="";
                    y_axis ="SAICal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAICal.m_AirXSP":
                    x_axis ="SAICal.p_AltCompSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SAICal.N_ClosedLoopDelay":
                    x_axis ="";
                    y_axis ="SAICal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.m_FuelBefStart":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.T_EngineXSP":
                    x_axis ="StartCal.n_CombYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"StartCal.EnrFacManTab":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.EnrFacManThrFaultTab":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.EnrFacAutTab":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.EnrFacAutThrFaultTab":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                case"StartCal.RestartFacTAB":
                    x_axis ="";
                    y_axis ="StartCal.t_RestartXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.T_EngineSP":
                    x_axis ="StartCal.p_HighAltSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCal.m_ReStBefFuel":
                    x_axis ="";
                    y_axis ="StartCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TransFuelCal.p_deltaSP":
                    x_axis ="TransFuelCal.n_EngineSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TransFuelCal.tempFacTAB":
                    x_axis ="";
                    y_axis ="TransFuelCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"AfterStCal.n_CombXSP":
                    x_axis ="AfterStCal.T_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"AfterStCal.t_soakXSP":
                    x_axis ="AfterStCal.T_EngineYSP2";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AfterStCal.n_CombXSP2":
                    x_axis ="AfterStCal.T_EngineYSP2";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AfterStCal.RestartAdjustTAB":
                    x_axis ="";
                    y_axis ="AfterStCal.t_soakXSP2";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TCompCal.m_AirInletSP":
                    x_axis ="TCompCal.n_EngineSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"TComp.FuelFacTab":
                    x_axis ="";
                    y_axis ="TComp.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AfterStCal.HotSoakCylFacTAB":
                    x_axis ="";
                    y_axis ="6: 1 2 3 4 5 6";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnAbsCal.m_AirNormXSP":
                    x_axis ="IgnAbsCal.n_EngNormYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"IgnAbsCal.fi_etaIgnOffsetTAB":
                    x_axis ="";
                    y_axis ="IgnAbsCal.EngEfficiencySP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnAbsCal.m_AirMBTXSP":
                    x_axis ="IgnAbsCal.n_EngMBTYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnAbsCal.T_EngStartXSP":
                    x_axis ="IgnAbsCal.n_EngStartYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnAbsCal.T_EngFuelCutXSP":
                    x_axis ="IgnAbsCal.n_EngFuelCutYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnMastCal.m_AirXSP":
                    x_axis ="IgnMastCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnMastCal.UBattDwellTimeXSP":
                    x_axis ="IgnMastCal.n_EngDwellTimeYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.ReduceTime":
                    x_axis ="";
                    y_axis ="16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.m_AirIndexXSP":
                    x_axis ="IgnKnkCal.n_EngIndexYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                
                case"IgnKnkCal.fi_RetardTAB":
                    x_axis ="";
                    y_axis ="IgnKnkCal.X_KnockSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.fi_TransRetardTAB":
                    x_axis ="";
                    y_axis ="IgnKnkCal.X_KnockSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.fi_Map1MaxOffset":
                    x_axis ="";
                    y_axis ="IgnKnkCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                case"IgnTempCal.m_AirXSP":
                    x_axis ="IgnTempCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                
                case"IgnTransCal.p_AirInletXSP":
                    x_axis ="IgnTransCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"IgnKnkCal.K_RetardIndexTAB":
                    x_axis ="";
                    y_axis ="KnkDetCal.fi_DiffXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"KnkDetCal.m_AirXSP":
                    x_axis ="KnkDetCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                case"KnkDetCal.m_AirNoiseXSP":
                    x_axis ="KnkDetCal.n_EngNoiseYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"KnkDetCal.T_EnableYSP":
                    x_axis ="";
                    y_axis ="KnkDetCal.T_EnableXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"KnkDetCal.K1TAB":
                    x_axis ="";
                    y_axis ="KnkDetCal.fi_DiffXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfAveCal.KaTAB":
                    x_axis ="";
                    y_axis ="MisfAveCal.t_HiMFAve1KaSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfAveCal.KbTAB":
                    x_axis ="";
                    y_axis ="MisfAveCal.t_HiMFAve1KbSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfAveCal.Ka2TAB":
                    x_axis ="";
                    y_axis ="MisfAveCal.t_HiMFAveAKaSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfAveCal.Kb2TAB":
                    x_axis ="";
                    y_axis ="MisfAveCal.t_HiMFAveAKbSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MisfAveCal.m_AirMFIndexXSP":
                    x_axis ="MisfAveCal.n_EngMFIndexYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SyncCal.n_SyncDelayTAB":
                    x_axis ="";
                    y_axis ="SyncCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"GearCal.Ratio":
                    x_axis ="";
                    y_axis ="6 : 1 2 3 4 5 6";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"GearCal.Range":
                    x_axis ="";
                    y_axis ="6 : 1 2 3 4 5 6";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"GearCal.n_SULAtGear":
                    x_axis ="";
                    y_axis ="5 : 1 2 3 4 5";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"GearCal.m_SULAtGear":
                    x_axis ="";
                    y_axis ="5 : 1 2 3 4 5";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"LambdaScanCal.LamScannerTab1":
                    x_axis ="";
                    y_axis ="LambdaScanCal.AD_ScannerSP1";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaScanCal.LamScannerTab2":
                    x_axis ="";
                    y_axis ="LambdaScanCal.AD_ScannerSP2";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaScanCal.LamScannerTab3":
                    x_axis ="";
                    y_axis ="LambdaScanCal.AD_ScannerSP3";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BFuelCal2.AirXSP":
                    x_axis ="BFuelCal2.RpmYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"StartCal.RampFacTab":
                    x_axis ="";
                    y_axis ="15 : 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AirAmbientCal.m_AirInlXSP":
                    x_axis ="AirAmbientCal.n_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                
                case"IgnNormCal.m_AirXSP":
                    x_axis ="IgnNormCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MissfCal.m_AirXSP":
                    x_axis ="MissfCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelConsCal.AirSP":
                    x_axis ="FuelConsCal.RpmSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"KnkDetCal.X_AverageTab":
                    x_axis ="";
                    y_axis ="KnkAdaptCal.fi_OffsetSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DDecCombMap":
                    x_axis ="";
                    y_axis ="LambdaCal.XSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DDecStepMap":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DDecRampMap":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DIncCombMap":
                    x_axis ="";
                    y_axis ="LambdaCal.XSp";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DIncStepMap":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LambdaCal.DIncRampMap":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FuelConsCal.m_AirInlXSP":
                    x_axis ="FuelConsCal.n_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                case"SystemActionCal.A_ThrottleXSP":
                    x_axis ="SystemActionCal.m_AirInletYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"SystemActionCal.v_VehicleXSP":
                    x_axis ="SystemActionCal.T_AirInletYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.p_AirInletTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_pAirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.p_AirBefThrottleTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_pAirBefThrottleSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.p_FuelTankTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_pFuelTankSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.T_ExhaustTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.T_ExhaustSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.p_AirAmbientTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_pAirAmbientSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.V_FuelTankEUTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_VFuelTankEUSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.V_FuelTankUSTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_VFuelTankUSSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.U_BatteryTAB":
                    x_axis ="";
                    y_axis ="VIOSSensorCal.AD_UBatterySP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSSensorCal.AD_UBatteryRangeTAB":
                    x_axis ="";
                    y_axis ="2 : 0 1";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"VIOSMAFCal.Q_AirInletTab":
                    x_axis ="";
                    y_axis ="VIOSMAFCal.TicksSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AC_ControlCal.K_MinClutchDsblTmeTAB":
                    x_axis ="";
                    y_axis ="AC_ControlCal.n_EngineSpeedSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AC_ControlCal.K_LaunchPedPosDsngeTAB":
                    x_axis ="";
                    y_axis ="AC_ControlCal.n_LaunchPedPosEngineSpeedSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AltCtrlCal.K_ColdStartDsblTAB":
                    x_axis ="";
                    y_axis ="AltCtrlCal.K_ColdStartDsblSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AltCtrlCal.K_EngSpdCutoutInParkTAB":
                    x_axis ="";
                    y_axis ="AltCtrlCal.K_EngSpdCutoutSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"AltCtrlCal.K_EngSpdCutoutNotParkTAB":
                    x_axis ="";
                    y_axis ="AltCtrlCal.K_EngSpdCutoutSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"BoostMeterCal.X_FactorTAB":
                    x_axis ="";
                    y_axis ="BoostMeterCal.X_FactorSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_D1_SpeedTAB":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_D2_SpeedTAB":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_D3_SpeedTAB":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.Fan_SpeedONTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.K_Speed_ON_FanSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.Fan_SpeedOFFTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.K_Speed_OFF_FanSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_ECT_FanReqTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_AC_FanReqTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.ACSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_TempEngOil_FanReqTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.T_EngOilSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_TempTransOil_FanReqTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.T_TrnOilSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CoolFanCal.K_AfterRunTimeTAB":
                    x_axis ="";
                    y_axis ="CoolFanCal.T_EngAfterRunSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilLifeCal.X_PenaltyFactorTAB":
                    x_axis ="";
                    y_axis ="OilLifeCal.T_OilSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.N_EngineRevsDelayTAB":
                    x_axis ="";
                    y_axis ="OilTempCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.T_DegreePerRevTAB":
                    x_axis ="";
                    y_axis ="OilTempCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.T_EngineSpeedIncreaseTAB":
                    x_axis ="";
                    y_axis ="OilTempCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.T_AirInletIncreaseTAB":
                    x_axis ="";
                    y_axis ="OilTempCal.p_AirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.X_EstimatedOilTempCoeffTAB":
                    x_axis ="";
                    y_axis ="OilTempCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"OilTempCal.T_AirInletXSP":
                    x_axis ="OilTempCal.v_VehicleYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCtrlCal.K_EngThresh":
                    x_axis ="";
                    y_axis ="StartCtrlCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"StartCtrlCal.K_Cnt_EngRevs":
                    x_axis ="";
                    y_axis ="StartCtrlCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ACCompCal.p_ACXSP":
                    x_axis ="ACCompCal.n_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"ACCompCal.t_ACOnRampTAB":
                    x_axis ="";
                    y_axis ="ACCompCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ACCompCal.t_ACRelayOffDelayTAB":
                    x_axis ="";
                    y_axis ="ACCompCal.n_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.F_FrictionTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.F_FrictionSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.X_EfficiencyTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.X_EfficiencySP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.a_AccReqTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.v_AccReqSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.a_ResumeTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.v_ResumeSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.a_SetTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.v_SetSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"CrsCntrlCal.a_DecTAB":
                    x_axis ="";
                    y_axis ="CrsCntrlCal.v_DecSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.SlowDriveRelTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_TrnXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.SlowDriveActTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_TrnXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.FastDriveActTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_TrnXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.FastDriveRelTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_TrnXSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.T_TrnXSP":
                    x_axis ="DNCompCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"DNCompCal.n_EngNomDriveTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.n_EngNomNeutralTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.n_DeltaTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.n_DeltaSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.n_NomStartRpmFlareTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"DNCompCal.n_StartFlareDecayRateTAB":
                    x_axis ="";
                    y_axis ="DNCompCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ElCompCal.P_BattDiffTAB":
                    x_axis ="";
                    y_axis ="ElCompCal.U_BattDiffSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ElCompCal.P_FanCompTAB":
                    x_axis ="";
                    y_axis ="8: 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ElCompCal.n_FTermXSP":
                    x_axis ="ElCompCal.PWM_FTermYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ElevIdleCal.n_ElevIdleHeaterTAB":
                    x_axis ="";
                    y_axis ="ElevIdleCal.T_OtsAirSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"ElevIdleCal.n_EngElevIdleUBattTAB":
                    x_axis ="";
                    y_axis ="ElevIdleCal.U_BattSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"6: A B C D E F":
                    x_axis ="40: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"EngTipLimNormCal.n_TipSP":
                    x_axis ="21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"EngTipLimSportCal.n_TipSP":
                    x_axis ="21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
             
                case"EngTipLimCal.n_DiffDeltaXSP":
                    x_axis ="EngTipLimCal.n_DiffYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FrictionLoadCal.T_EngXSP":
                    x_axis ="FrictionLoadCal.n_EngNomYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FrictionLoadCal.Trq_Request_p_AirAmbTAB":
                    x_axis ="";
                    y_axis ="FrictionLoadCal.p_AirAmbSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"FrictionLoadCal.Trq_RequestT_AirAmbTAB":
                    x_axis ="";
                    y_axis ="FrictionLoadCal.T_AirAmbSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.Trq_RequestXSP":
                    x_axis ="LightOffCal.n_EngYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.T_EngCombTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.T_AirCombTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.T_AirInletSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.n_EngNeutralTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.n_EngDriveTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.RampStepTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.n_EngRampSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"LightOffCal.n_CombAfterStartRpmTAB":
                    x_axis ="";
                    y_axis ="LightOffCal.T_EngSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"PedalMapCal.n_EngineMap":
                    x_axis ="PedalMapCal.X_PedalMap";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"Trq_ThrDefPosStartTAB":
                    x_axis ="";
                    y_axis ="T_DEG_C T_ThrDefPosStartSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"PedalMapCal.Trq_ThrDefPosStartTAB":
                    x_axis ="";
                    y_axis ="PedalMapCal.T_ThrDefPosStartSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"TOACal.v_vehicleXSP":
                    x_axis ="TOACal.n_EngDiffYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TOACal.X_D_partFacTAB":
                    x_axis ="";
                    y_axis ="TOACal.n_EngDiffYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TOACal.Trq_maxTAB":
                    x_axis ="";
                    y_axis ="TOACal.v_vehicleYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlAdapAdap.Trq_ReqNeutralNormTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlAdapCal.T_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlAdapAdap.Trq_ReqDriveNormTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlAdapCal.T_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.P_PartIgnTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.P_PartIgnSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.P_PartIgnTakeOffTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.P_PartIgnTakeOffSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.I_PartIgnTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.I_PartIgnSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.P_PartAirTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.P_PartAirSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.I_PartAirTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.I_PartAirSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.D_FacIgnTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.P_PartIgnSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.Trq_TakeOffLimHighTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.Trq_TakeOffLimSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.Trq_TakeOffLimLowTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.Trq_TakeOffLimSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"RpmCtrlCal.v_VehicleXSP":
                    x_axis ="RpmCtrlCal.P_PartIgnYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
              
                case"RpmCtrlCal.I_PartAirMaxTAB":
                    x_axis ="";
                    y_axis ="RpmCtrlCal.v_VehicleYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_ManGear":
                    x_axis ="";
                    y_axis ="8 : 0 1 2 3 4 5 6 7";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_MaxEngineManTab1":
                    x_axis ="";
                    y_axis ="TrqLimCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_MaxEngineAutTab1":
                    x_axis ="";
                    y_axis ="TrqLimCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_MaxEngineManTab2":
                    x_axis ="";
                    y_axis ="TrqLimCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_MaxEngineAutTab2":
                    x_axis ="";
                    y_axis ="TrqLimCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.Trq_OverBoostTab":
                    x_axis ="";
                    y_axis ="TrqLimCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"MaxEngSpdCal.n_EngLimTab":
                    x_axis ="";
                    y_axis ="MaxEngSpdCal.T_EngineSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqLimCal.CompressorNoiseXSP":
                    x_axis ="TrqLimCal.CompressorNoiseYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.n_EngineXSP":
                    x_axis ="TrqMastCal.Trq_PedYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.TLO_TAB":
                    x_axis ="";
                    y_axis ="TrqMastCal.IgnAngleDiffSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.m_AirXSP":
                    x_axis ="AirMassMastCal.n_EngMBTYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"TrqMastCal.Trq_EngXSP":
                    x_axis ="TrqMastCal.n_EngineYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.n_MaxCylInFuelCutTAB":
                    x_axis ="";
                    y_axis ="TrqMastCal.T_YSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.Trq_MaxDerTAB":
                    x_axis ="";
                    y_axis ="TrqMastCal.T_MaxDerSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TrqMastCal.n_MaxDerXSP":
                    x_axis ="TrqMastCal.T_MaxDerYSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case "TrqMastCal.Trq_NominalMap":
                    x_axis = "TrqMastCal.m_AirXSP";
                    y_axis = "TrqMastCal.n_EngineYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Torque (Nm * 10)";
                    break;
                case "TrqMastCal.Trq_MBTMAP":
                    x_axis = "TrqMastCal.m_AirXSP";
                    y_axis = "AirMassMastCal.n_EngMBTYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Torque (Nm * 10)";
                    break;
                case "TrqMastCal.m_AirTorqMap":
                    x_axis = "TrqMastCal.Trq_EngXSP";
                    y_axis = "TrqMastCal.n_EngineYSP";
                    x_axis_description = "Torque (Nm * 10)";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case"TMCCal.T_EngSP":
                    x_axis ="TMCCal.n_Engine1YSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TMCCal.T_TrnOilSP":
                    x_axis ="TMCCal.n_Engine2YSP";
                    y_axis ="";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
               
                case"TMCCal.Trq_ACLoadTAB":
                    x_axis ="";
                    y_axis ="TMCCal.p_ACSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TMCCal.Trq_MaxEngineTab":
                    x_axis ="";
                    y_axis ="TMCCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TMCCal.Trq_MaxEngineLowTab":
                    x_axis ="";
                    y_axis ="TMCCal.n_EngYSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TMCCal.Trq_MinLoadTAB":
                    x_axis ="";
                    y_axis ="TMCCal.T_EngMinSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;
                case"TMCCal.Trq_ACDynReqTAB":
                    x_axis ="";
                    y_axis ="TMCCal.t_ACDynReqSP";
                    x_axis_description ="";
                    y_axis_description ="";
                    z_axis_description ="";
                    break;

                case"CruiseCal.a_DecReqMap":
                    x_axis ="CruiseCal.v_DiffXSP";
                    y_axis ="CruiseCal.v_ActualYSP";
                    x_axis_description ="km/h (* 10)";
                    y_axis_description ="km/h (* 10)";
                    z_axis_description ="m/s^2";
                    break;
                case"TempTiCal.map":
                    x_axis ="TempTiCal.x_axis";
                    y_axis ="TempTiCal.y_axis";
                    x_axis_description ="kPa";
                    y_axis_description ="mg/c";
                    z_axis_description ="ms";
                    break;
                case"AirCtrlCal.map":
                    x_axis ="AirCtrlCal.x_axis";
                    y_axis ="AirCtrlCal.y_axis";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="bit";
                    break;
                case"LambdaCal.DecCombMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="comb";
                    break;
                case"LambdaCal.DecStepMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"LambdaCal.DecRampMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"LambdaCal.IncCombMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="comb";
                    break;
                case"LambdaCal.IncStepMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"LambdaCal.IncRampMap":
                    x_axis ="LambdaCal.XSp";
                    y_axis ="LambdaCal.YSp";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"BFuelCal.Map":
                    x_axis ="BFuelCal.AirXSP";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"BFuelCal.StartMap":
                    x_axis ="BFuelCal.AirXSP";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"BFuelCal2.StartMap":
                    x_axis ="BFuelCal2.AirXSP";
                    y_axis ="BFuelCal2.RpmYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"BFuelCal2.Map":
                    x_axis ="BFuelCal2.AirXSP";
                    y_axis ="BFuelCal2.RpmYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"StartCal.ScaleFacRpmMap":
                    x_axis ="StartCal.T_EngineXSP";
                    y_axis ="StartCal.n_EngineYSP";
                    x_axis_description ="øC";
                    y_axis_description ="Rpm";
                    z_axis_description ="Fac";
                    break;
                case"StartCal.HighAltFacMap":
                    x_axis ="StartCal.T_EngineSP";
                    y_axis ="StartCal.p_HighAltSP";
                    x_axis_description ="øC";
                    y_axis_description ="kPa";
                    z_axis_description ="Fac";
                    break;
                case"InjAnglCal.Map":
                    x_axis ="InjAnglCal.AirXSP";
                    y_axis ="InjAnglCal.RpmYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm";
                    z_axis_description ="0 (* 10)";
                    break;
                case"EmLimCal.m_MaxIncrMap":
                    x_axis ="EmLimCal.GearXSP";
                    y_axis ="EmLimCal.n_EngYSP";
                    x_axis_description ="ascii";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="mg/c";
                    break;
                case"EmLimCal.m_AutMaxIncrMap":
                    x_axis ="EmLimCal.AutGearXSP";
                    y_axis ="EmLimCal.n_EngYSP";
                    x_axis_description ="0";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="mg/c";
                    break;
                case"EmLimCal.RampFactorMap":
                    x_axis ="EmLimCal.PedDeltaXSP";
                    y_axis ="EmLimCal.X_PedYSP";
                    x_axis_description ="0";
                    y_axis_description ="%";
                    z_axis_description ="0";
                    break;
                case"MAFCal.WeightConstFuelMa":
                    x_axis ="MAFCal.n_EngineXSP";
                    y_axis ="MAFCal.p_InletGradYSP";
                    x_axis_description ="rpm (* 100)";
                    y_axis_description ="kPa";
                    z_axis_description ="0";
                    break;
                case"MAFCal.m_RedundantAirMap":
                    x_axis ="MAFCal.n_EngXSP";
                    y_axis ="MAFCal.LoadYSP";
                    x_axis_description ="rpm (* 100)";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"MAFCal.cd_ThrottleMap":
                    x_axis ="MAFCal.AreaXSP";
                    y_axis ="MAFCal.PQuoteYSP";
                    x_axis_description ="mmý (* 100)";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MAFCal.corr_AirFromp":
                    x_axis ="MAFCal.Diagn_EngXSP";
                    y_axis ="MAFCal.DiagLoadYSP";
                    x_axis_description ="rpm (* 100)";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MAFCal.corr_Count":
                    x_axis ="MAFCal.Diagn_EngXSP";
                    y_axis ="MAFCal.DiagLoadYSP";
                    x_axis_description ="rpm (* 100)";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MAFCal.Area_Count":
                    x_axis ="MAFCal.AreaXSP";
                    y_axis ="MAFCal.PQuoteYSP";
                    x_axis_description ="0 (* 100)";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"SAICal.m_AirSAIMAP":
                    x_axis ="SAICal.m_AirXSP";
                    y_axis ="SAICal.p_AltCompSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="kPa";
                    z_axis_description ="g/s";
                    break;
                case"SAICal.FuellingMap":
                    x_axis ="SAICal.m_AirXSP";
                    y_axis ="SAICal.n_EngineYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Num";
                    break;
                case"SAICal.FuellingMap_2":
                    x_axis ="SAICal.m_AirXSP";
                    y_axis ="SAICal.n_EngineYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="0";
                    break;
                case"IdleCal.StartRedFacMat":
                    x_axis ="IdleCal.n_EngDiffSP";
                    y_axis ="IdleCal.t_yaxis";
                    x_axis_description ="rpm";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"IgnStartCal.fi_StartMap":
                    x_axis ="IgnStartCal.n_EngXSP";
                    y_axis ="IgnStartCal.T_AirSP";
                    x_axis_description ="0";
                    y_axis_description ="øC";
                    z_axis_description ="ø";
                    break;
                case"IgnNormCal.Map":
                    x_axis ="IgnNormCal.m_AirXSP";
                    y_axis ="IgnNormCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"IgnNormCal2.Map":
                    x_axis ="IgnNormCal.m_AirXSP";
                    y_axis ="IgnNormCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"TorqueCal.X_AccPedalMap":
                    x_axis ="PedalMapCal.n_EngineMap";
                    y_axis ="TorqueCal.m_PedYSP";
                    x_axis_description ="rpm (* 10)";
                    y_axis_description ="mg/c (* 10)";
                    z_axis_description ="% (* 10)";
                    break;
                case"TorqueCal.M_NominalMap":
                    x_axis ="TorqueCal.m_AirXSP";
                    y_axis ="TorqueCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TorqueCal.fi_IgnLimMap":
                    x_axis ="TorqueCal.m_AirXSP";
                    y_axis ="TorqueCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="ø";
                    break;
                case"TorqueCal.M_IgnInflTorqM":
                    x_axis ="TorqueCal.m_AirXSP";
                    y_axis ="TorqueCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm/ø";
                    break;
                case"TorqueCal.m_AirTorqMap":
                    x_axis ="TorqueCal.M_EngXSP";
                    y_axis ="TorqueCal.n_EngYSP";
                    x_axis_description ="Nm (* 5)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="mg/c (* 10)";
                    break;
                case"TorqueCal.fi_IgnMinTab":
                    x_axis ="TorqueCal.n_EngMinSP";
                    y_axis ="TorqueCal.T_EngSP";
                    x_axis_description ="rpm";
                    y_axis_description ="øC";
                    z_axis_description ="ø";
                    break;
                case"BoostCal.PMap":
                    x_axis ="BoostCal.PIDXSP";
                    y_axis ="BoostCal.PIDYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="0";
                    break;
                case"BoostCal.IMap":
                    x_axis ="BoostCal.PIDXSP";
                    y_axis ="BoostCal.PIDYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="0";
                    break;
                case"BoostCal.DMap":
                    x_axis ="BoostCal.PIDXSP";
                    y_axis ="BoostCal.PIDYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="0";
                    break;
                case"BoostCal.RegMap":
                    x_axis ="BoostCal.SetLoadXSP";
                    y_axis ="BoostCal.n_EngSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="%";
                    break;
                case"TransCal.AccFacMap":
                    x_axis ="TransCal.AccSP";
                    y_axis ="TransCal.RpmSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"TransCal.DecFacMap":
                    x_axis ="TransCal.DecSP";
                    y_axis ="TransCal.RpmSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"AirMinLimCal.Q_DashPotMa":
                    x_axis ="AirMinLimCal.m_AirXSP";
                    y_axis ="AirMinLimCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="g/s";
                    break;
                case"LoadCoCal.Q_ACCompAirMap":
                    x_axis ="LoadCoCal.p_ACSP";
                    y_axis ="LoadCoCal.n_EngineSP";
                    x_axis_description ="Bar";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="g/s";
                    break;
                case"LoadCoCal.Q_ACDynamicMap":
                    x_axis ="LoadCoCal.p_ACSP";
                    y_axis ="LoadCoCal.n_EngineSP";
                    x_axis_description ="Bar";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="g/s";
                    break;
                case"IgnLOffCal.fi_MapOffset":
                    x_axis ="IgnLOffCal.m_AirXSP";
                    y_axis ="IgnLOffCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"BoostCal.HighAltMap":
                    x_axis ="BoostCal.SetLoad2XSP";
                    y_axis ="BoostCal.HighAltTabSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="kPa";
                    z_axis_description ="%";
                    break;
                case"TCompCal.EnrFacMap":
                    x_axis ="TCompCal.EnrFacXSP";
                    y_axis ="TCompCal.EnrFacYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"TCompCal.EnrFacAutMap":
                    x_axis ="TCompCal.EnrFacAutXSP";
                    y_axis ="TCompCal.EnrFacAutYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="%";
                    break;
                case"IgnOffsCal.DNCompSlow":
                    x_axis ="IgnOffsCal.Q_ExtraSP";
                    y_axis ="IdleCal.T_EngineSP";
                    x_axis_description ="0";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"IgnOffsCal.DNCompFast":
                    x_axis ="IgnOffsCal.Q_ExtraSP";
                    y_axis ="IdleCal.T_EngineSP";
                    x_axis_description ="0";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"TEngDiagCal.m_AirperDegM":
                    x_axis ="TEngDiagCal.T_AirInletSP";
                    y_axis ="TEngDiagCal.n_CombSP";
                    x_axis_description ="C";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"TEngDiagCal.m_AirperDegA":
                    x_axis ="TEngDiagCal.T_AirInletSP";
                    y_axis ="TEngDiagCal.n_CombSP";
                    x_axis_description ="C";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"CatDiagCal.HiMapOx1Cycle":
                    x_axis ="CatDiagCal.LoadTab";
                    y_axis ="CatDiagCal.RpmTab";
                    x_axis_description ="0";
                    y_axis_description ="0";
                    z_axis_description ="ms";
                    break;
                case"CatDiagCal.LoMapOx1Cycle":
                    x_axis ="CatDiagCal.LoadTab";
                    y_axis ="CatDiagCal.RpmTab";
                    x_axis_description ="0";
                    y_axis_description ="0";
                    z_axis_description ="ms";
                    break;
                case"CatDiagCal.Ox2DevMaxMap":
                    x_axis ="CatDiagCal.LoadTab";
                    y_axis ="CatDiagCal.RpmTab";
                    x_axis_description ="0";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"CatDiagLevCal.ErrorLim":
                    x_axis ="CatDiagLevCal.m_SP";
                    y_axis ="CatDiagLevCal.T_CatSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="C";
                    z_axis_description ="ms";
                    break;
                case"CatModCal.T_SteadyState":
                    x_axis ="CatModCal.m_SP";
                    y_axis ="CatModCal.n_SP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm";
                    z_axis_description ="C";
                    break;
                case"MissfAdap.MissfCntMap":
                    x_axis ="MissfCal.m_AirXSP";
                    y_axis ="MissfCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MissfCal.RPMDiffLimMAP":
                    x_axis ="MissfCal.LoadRPMDiffXSP";
                    y_axis ="MissfCal.n_EngRPMDiffYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MissfCal.DetectLoadLevel":
                    x_axis ="MissfCal.T_EngXSP";
                    y_axis ="MissfCal.n_EngYSP";
                    x_axis_description ="øC";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"MissfCal.CatOverheatFact":
                    x_axis ="MissfCal.m_AirXSP";
                    y_axis ="MissfCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MissfCal.outOfLimDelayMA":
                    x_axis ="MissfCal.p_AirAmbOutOfLi";
                    y_axis ="MissfCal.T_EngOutOfLimSP";
                    x_axis_description ="kPa";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"AirCompCal.PressMap":
                    x_axis ="AirCtrlCal.x_axis";
                    y_axis ="AirCtrlCal.y_axis";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="kPa";
                    break;
                case"IdleCal.Q_IdleOffsMAP":
                    x_axis ="IdleCal.T_EngRPMOffSP";
                    y_axis ="IdleCal.n_RPMOffYSP";
                    x_axis_description ="øC";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="g/s";
                    break;
                case"BoostCal.p_DiffILimMap":
                    x_axis ="BoostCal.p_DiffILimXSP";
                    y_axis ="BoostCal.p_DiffILimYSP";
                    x_axis_description ="kPa";
                    y_axis_description ="mg/c";
                    z_axis_description ="0";
                    break;
                case"IgnTempCal.AirMap":
                    x_axis ="IgnTempCal.n_EngXSP";
                    y_axis ="IgnTempCal.T_AirYSP";
                    x_axis_description ="rpm";
                    y_axis_description ="øC";
                    z_axis_description ="ø";
                    break;
                case"IgnTempCal.EngMap":
                    x_axis ="IgnTempCal.n_EngXSP";
                    y_axis ="IgnTempCal.T_EngYSP";
                    x_axis_description ="rpm";
                    y_axis_description ="øC";
                    z_axis_description ="ø";
                    break;
                case"IgnCal.MinMap":
                    x_axis ="IgnCal.m_AirXSP";
                    y_axis ="IgnCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;
                case"BstKnkCal.MaxAirmass":
                    x_axis = "BstKnkCal.OffsetXSP"; // BstKnkCal.fi_offsetXSP in case of flexfuel!
                    y_axis ="BstKnkCal.n_EngYSP";
                    x_axis_description ="ø";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"BstKnkCal.MaxAirmassAu":
                    x_axis = "BstKnkCal.OffsetXSP"; // BstKnkCal.fi_offsetXSP in case of flexfuel!
                    y_axis ="BstKnkCal.n_EngYSP";
                    x_axis_description ="ø";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"PedalMapCal.m_RequestMap":
                    x_axis ="PedalMapCal.n_EngineMap";
                    y_axis ="PedalMapCal.X_PedalMap";
                    x_axis_description ="rpm (* 10)";
                    y_axis_description ="%";
                    z_axis_description ="mg/c";
                    break;
                case"IgnIdleCal.fi_IdleMap":
                    x_axis ="IgnIdleCal.m_AirXSP";
                    y_axis ="IgnIdleCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"LoadCoCal.Q_ColdFricMap":
                    x_axis ="LoadCoCal.n_EngXSP";
                    y_axis ="LoadCoCal.T_EngYSP";
                    x_axis_description ="0";
                    y_axis_description ="øC";
                    z_axis_description ="g/s";
                    break;
                case"BFuelAdap.V_FuelConsMap":
                    x_axis ="FuelConsCal.m_AirInlXSP";
                    y_axis ="FuelConsCal.n_EngineYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ml";
                    break;
                case"KnkAdaptCal.WeightMap1":
                    x_axis ="KnkAdaptCal.m_AirXSP1";
                    y_axis ="KnkAdaptCal.n_EngYSP1";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"KnkAdaptCal.WeightMap2":
                    x_axis ="KnkAdaptCal.m_AirXSP2";
                    y_axis ="KnkAdaptCal.n_EngYSP2";
                    x_axis_description ="mg/c";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"KnkDetCal.KnockWinOffs":
                    x_axis ="KnkDetCal.m_AirXSP";
                    y_axis ="KnkDetCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="ø";
                    break;
                case"KnkDetCal.RefFactorMap":
                    x_axis ="KnkDetCal.m_AirXSP";
                    y_axis ="KnkDetCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"KnkDetAdap.KnkCntMap":
                    x_axis ="KnkDetCal.m_AirXSP";
                    y_axis ="KnkDetCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"IgnKnkCal.IndexMap":
                    x_axis ="IgnKnkCal.m_AirXSP";
                    y_axis ="IgnKnkCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="const";
                    break;
                case"KnkFuelCal.fi_MapMaxOff":
                    x_axis ="KnkFuelCal.m_AirXSP";
                    y_axis ="BstKnkCal.n_EngYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;
                case"KnkFuelCal.WeightTab":
                    x_axis ="KnkFuelCal.WeightXSP";
                    y_axis ="KnkFuelCal.WeightYSP";
                    x_axis_description ="rpm";
                    y_axis_description ="0";
                    z_axis_description ="Num";
                    break;
                case"KnkFuelCal.EnrichmentMap":
                    x_axis ="IgnKnkCal.m_AirXSP";
                    y_axis ="IgnKnkCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Factor";
                    break;
                case"KnkFuelCal.fi_MapMaxOffs":
                    x_axis ="IgnKnkCal.m_AirXSP";
                    y_axis ="IgnKnkCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="ø";
                    break;
                case"TqToHiCal.Q_AirInletMap":
                    x_axis ="TqToHiCal.T_AirInletXSP";
                    y_axis ="TqToHiCal.X_AccPedYSP";
                    x_axis_description ="C";
                    y_axis_description ="%";
                    z_axis_description ="g/s";
                    break;
                case"SwitchCal.A_SwitchPoint":
                    x_axis ="SwitchCal.T_Engine";
                    y_axis ="SwitchCal.n_Engine";
                    x_axis_description ="øC";
                    y_axis_description ="rpm";
                    z_axis_description ="mm^2";
                    break;
                case"SwitchCal.A_AmbPresMap":
                    x_axis ="SwitchCal.n_EngXSP";
                    y_axis ="SwitchCal.p_AmbientYSP";
                    x_axis_description ="rpm";
                    y_axis_description ="kPa";
                    z_axis_description ="mm2";
                    break;
                case"KnkSoundRedCal.fi_OffsMa":
                    x_axis ="IgnNormCal.m_AirXSP";
                    y_axis ="IgnNormCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"AftSt1ExtraCal.EnrFacMap":
                    x_axis ="AftStCal1.T_EngineSP";
                    y_axis ="AftSt1ExtraCal.p_AirAmbS";
                    x_axis_description ="øC";
                    y_axis_description ="kPa";
                    z_axis_description ="0";
                    break;
                case"AftSt2ExtraCal.EnrFacMap":
                    x_axis ="AftStCal1.T_EngineSP";
                    y_axis ="AftSt2ExtraCal.p_AirAmbS";
                    x_axis_description ="øC";
                    y_axis_description ="kPa";
                    z_axis_description ="0";
                    break;
                case"PurgeCal.ValveMap16":
                    x_axis ="PurgeCal.p_Diff16XSp";
                    y_axis ="PurgeCal.q_Flow16YSp";
                    x_axis_description ="kPa";
                    y_axis_description ="mg/s";
                    z_axis_description ="%";
                    break;
                case"PurgeCal.ValveMap8":
                    x_axis ="PurgeCal.p_Diff8XSp";
                    y_axis ="PurgeCal.q_Flow8YSp";
                    x_axis_description ="kPa";
                    y_axis_description ="mg/s";
                    z_axis_description ="%";
                    break;
                case"ExhaustCal.T_Lambda1Map":
                    x_axis ="BFuelCal.AirXSP";
                    y_axis ="BFuelCal.RpmYSP";
                    x_axis_description ="mg/c";
                    y_axis_description ="rpm";
                    z_axis_description ="øC";
                    break;
                case"ExhaustCal.fi_IgnMap":
                    x_axis ="ExhaustCal.m_AirSP";
                    y_axis ="ExhaustCal.fi_IgnSP";
                    x_axis_description ="0";
                    y_axis_description ="ø";
                    z_axis_description ="øC";
                    break;
                case"IgnNormCal.GasMap":
                    x_axis ="IgnNormCal.m_AirXSP";
                    y_axis ="IgnNormCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="ø";
                    break;
                case"F_KnkDetAdap.RKnkCntMap":
                    x_axis ="KnkDetCal.m_AirXSP";
                    y_axis ="KnkDetCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;
                case"F_KnkDetAdap.FKnkCntMap":
                    x_axis ="KnkDetCal.m_AirXSP";
                    y_axis ="KnkDetCal.n_EngYSP";
                    x_axis_description ="mg/c (* 10)";
                    y_axis_description ="0 (* 10)";
                    z_axis_description ="0";
                    break;

                case"ShiftSupCal.M_HystTab":
                    y_axis ="ShiftSupCal.M_RLoadSP";
                    y_axis_description ="Nm";
                    z_axis_description ="Nm";
                    break;
                case"PwmLimitCal.PwmLimit":
                    y_axis ="PwmLimitCal.U_Batt";
                    y_axis_description ="V";
                    z_axis_description ="%";
                    break;
                case"CruiseComCal.M_RoadTorque":
                    y_axis ="CruiseComCal.v_Actual";
                    y_axis_description ="km/h (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"CruiseCal.v_LimitPos":
                    y_axis ="CruiseCal.v_RequestNorma";
                    y_axis_description ="km/h (* 10)";
                    z_axis_description ="km/h";
                    break;
                case"CruiseCal.v_LimitNeg":
                    y_axis ="CruiseCal.v_RequestNorma";
                    y_axis_description ="km/h (* 10)";
                    z_axis_description ="km/h";
                    break;
                case"CruiseCal.v_LimitNormal":
                    y_axis ="CruiseCal.v_RequestNorma";
                    y_axis_description ="km/h (* 10)";
                    z_axis_description ="km/h";
                    break;
                case"CruiseCal.a_AccRequest":
                    y_axis ="CruiseComCal.v_Delta";
                    y_axis_description ="km/h";
                    z_axis_description ="m/s^2";
                    break;
                case"CruiseCal.a_DecRequest":
                    y_axis ="CruiseComCal.v_Delta";
                    y_axis_description ="km/h";
                    z_axis_description ="m/s^2";
                    break;
                case"CruiseComCal.M_Offset":
                    y_axis ="CruiseComCal.v_Delta";
                    y_axis_description ="km/h";
                    z_axis_description ="%";
                    break;
                case"CruiseCal.M_GradientPos":
                    y_axis ="CruiseCal.M_GradActual";
                    y_axis_description ="Nm";
                    z_axis_description ="Nm";
                    break;
                case"CruiseCal.M_GradientNeg":
                    y_axis ="CruiseCal.M_GradActual";
                    y_axis_description ="Nm";
                    z_axis_description ="Nm";
                    break;
                
                case"TempLimPosCal.Limit":
                    y_axis ="TempLimPosCal.Airmass";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                
               
                case"LambdaCal.MinLoadTab":
                    y_axis ="LambdaCal.RpmSp";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"LambdaCal.MaxLoadTimeTab":
                    y_axis ="LambdaCal.RpmSp";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"LambdaCal.MaxLoadNormTab":
                    y_axis ="LambdaCal.RpmSp";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                
                case"AftStCal1.EnrFacTab":
                    y_axis ="AftStCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"AftStCal1.DecrDelayTab":
                    y_axis ="AftStCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"AftStCal2.EnrFacTab":
                    y_axis ="AftStCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"AftStCal2.DecrDelayTab":
                    y_axis ="AftStCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
              
                case"TCompCal.EnrFacTab":
                    y_axis ="TCompCal.T_EngineAutSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"TCompCal.EnrFacAutTab":
                    y_axis ="TCompCal.T_EngineAutSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"StartCal.CombFacTab":
                    y_axis ="StartCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"StartCal.EnrFacTab":
                    y_axis ="StartCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"StartCal.RestartFacTab":
                    y_axis ="StartCal.t_RestartSP";
                    y_axis_description ="sec";
                    z_axis_description ="0";
                    break;
                case"MAFCal.t_PosTransFreezTa":
                    y_axis ="MAFCal.n_EngineXSP";
                    y_axis_description ="rpm (* 100)";
                    z_axis_description ="ms";
                    break;
                case"MAFCal.t_NegTransFreezTa":
                    y_axis ="MAFCal.n_EngineXSP";
                    y_axis_description ="rpm (* 100)";
                    z_axis_description ="ms";
                    break;
                case"MAFCal.ConstT_AirInlTab":
                    y_axis ="MAFCal.T_EngineSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MAFCal.ConstT_EngineTab":
                    y_axis ="MAFCal.T_EngineSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
              
                case"SAICal.m_AirInjReq":
                    y_axis ="SAICal.T_EngineSP";
                    y_axis = "SAICal.p_AltCompSP";

                    y_axis_description ="øC";
                    z_axis_description ="mg/c";
                    break;
             
                case"SAICal.AltComp":
                    y_axis ="SAICal.p_AltCompSP";
                    y_axis_description ="øC";
                    z_axis_description ="comb";
                    break;
                case"MaxSpdCal.n_EngLimAir":
                    y_axis ="MaxSpdCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="rpm";
                    break;
                case"IdleCal.C_PartDrive":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.C_PartNeutral":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.I_PartTab":
                    y_axis ="IdleCal.P_PartSP";
                    y_axis_description ="rpm";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.P_PartTab":
                    y_axis ="IdleCal.P_PartSP";
                    y_axis_description ="rpm";
                    z_axis_description ="0";
                    break;
                case"IdleCal.Q_StartOffsTab":
                    y_axis ="IdleCal.p_AltStartSP";
                    y_axis_description ="kPa";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.Q_AltStartTAB":
                    y_axis ="IdleCal.p_AltStartSP";
                    y_axis_description ="kPa";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.n_EngNomDrive":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="rpm";
                    break;
                case"IdleCal.n_EngNomNeutral":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="rpm";
                    break;
               /* case"IgnKnkCal.ReduceTime":
                    y_axis ="16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description ="0";
                    z_axis_description ="ø";
                    break;
                case"GearCal.Range":
                    y_axis ="16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;*/
                case"TorqueCal.M_ManGearLim":
                    y_axis ="TorqueCal.n_Eng5GearSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm (* 5)";
                    break;
                case"TorqueCal.M_5GearLimTab":
                    y_axis ="TorqueCal.n_Eng5GearSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TorqueCal.M_ReverseTab":
                    y_axis ="TorqueCal.n_Eng1GearSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TorqueCal.M_1GearTab":
                    y_axis ="TorqueCal.n_Eng1GearSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TorqueCal.M_EngMaxTab":
                    y_axis ="TorqueCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TorqueCal.M_EngMaxAutTab":
                    y_axis ="TorqueCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"BoostCal.P_LimTab":
                    y_axis ="BoostCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"BoostCal.I_LimTab":
                    y_axis ="BoostCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"TorqueCal.M_OverBoostTab":
                    y_axis ="TorqueCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="Nm";
                    break;
                case"TransCal.m_TriggMaxTab":
                    y_axis ="TransCal.DecTriggSP";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"TransCal.AccTriggLim":
                    y_axis ="TransCal.DecTriggSP";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"TransCal.DecTriggLim":
                    y_axis ="TransCal.DecTriggSP";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"TransCal.AccIdleTriggLim":
                    y_axis ="TransCal.DecTriggSP";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"TransCal.DecIdleTriggLim":
                    y_axis ="TransCal.DecTriggSP";
                    y_axis_description ="0";
                    z_axis_description ="mg/c";
                    break;
                case"AirMinLimCal.Q_MinLoadTa":
                    y_axis ="AirMinLimCal.v_VehicleSP";
                    y_axis_description ="km/h";
                    z_axis_description ="g/s";
                    break;
                case"AirMinLimCal.p_AirAmbTab":
                    y_axis ="AirMinLimCal.v_VehicleSP";
                    y_axis_description ="km/h";
                    z_axis_description ="kPa";
                    break;
                case"AirMinLimCal.v_VehicleTa":
                    y_axis ="AirMinLimCal.v_VehicleSP";
                    y_axis_description ="km/h";
                    z_axis_description ="km/h";
                    break;
                case"LoadCoCal.t_ACOnDlyTab":
                    y_axis ="LoadCoCal.n_EngineSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ms";
                    break;
                case"LoadCoCal.t_ACOffDlyTab":
                    y_axis ="LoadCoCal.n_EngineSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ms";
                    break;
                case"LoadCoCal.Q_ElLoadCoTab":
                    y_axis ="LoadCoCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="g/s";
                    break;
                case"LoadCoCal.t_ElLoadRampTa":
                    y_axis ="LoadCoCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ms";
                    break;
                case"TransCal.AccTempFacTab":
                    y_axis ="TransCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0 (* 10)";
                    break;
                case"TransCal.DecTempFacTab":
                    y_axis ="TransCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0 (* 10)";
                    break;
                case"FCutCal.FuelFactor":
                    y_axis ="FCutCal.n_CombSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"BoostCal.PercAdapTab":
                    y_axis ="BoostCal.m_AirAdapSP";
                    y_axis_description ="mg/c";
                    z_axis_description ="%";
                    break;
                case"IgnLOffCal.CombTab":
                    y_axis ="IgnLOffCal.T_AirInletSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"IgnLOffCal.N_AirTab":
                    y_axis ="IgnLOffCal.T_AirInletSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"BoostCal.AntiHowlPresTab":
                    y_axis ="BoostCal.AntiHowlPairSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"BoostCal.PWMTempTab":
                    y_axis ="BoostCal.PWMTempSP";
                    y_axis_description ="øC";
                    z_axis_description ="%";
                    break;
                case"EvapDiagCal.CalcFac":
                    y_axis ="EvapDiagCal.RampSumTab";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"IdleCal.NeutralAirPartTAB":
                    y_axis ="EvapDiagCal.RampSumTab";
                    y_axis_description ="0";
                    z_axis_description ="%";
                    break;
                case"QAirDiagCal.HFMOffSim":
                    y_axis ="QAirDiagCal.HFMOffSimSP";
                    y_axis_description ="g/s";
                    z_axis_description ="0";
                    break;
                case"TEngDiagCal.T_startSP":
                    y_axis ="TEngDiagCal.m_LoadFacSP";
                    y_axis_description ="0";
                    z_axis_description ="C";
                    break;
                case"TEngDiagCal.m_LoadFacTAB":
                    y_axis ="TEngDiagCal.m_LoadFacSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"O2SPreCal.t_O2SFrontTime":
                    y_axis ="O2SPreCal.T_O2SFrontTemp";
                    y_axis_description ="C";
                    z_axis_description ="ms";
                    break;
                case"CatModCal.T_Offset":
                    y_axis ="CatModCal.T_SPCombStart";
                    y_axis_description ="C";
                    z_axis_description ="C";
                    break;
                case"CatModCal.n_CombStart":
                    y_axis ="CatModCal.T_SPCombStart";
                    y_axis_description ="C";
                    z_axis_description ="0";
                    break;
                case"MissfCal.LeanStep":
                    y_axis ="LambdaCal.U_AdjStepSP";
                    y_axis_description ="mV (* 10)";
                    z_axis_description ="%";
                    break;
                case"MissfCal.RichStep":
                    y_axis ="LambdaCal.U_AdjStepSP";
                    y_axis_description ="mV (* 10)";
                    z_axis_description ="%";
                    break;
                case"MissfCal.nrOfTransientFi":
                    y_axis ="MissfCal.EngTempSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MissfCal.startDelayTAB":
                    y_axis ="MissfCal.T_EngSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"MissfCal.M_LimTab":
                    y_axis ="MissfCal.n_EngineSP";
                    y_axis_description ="rpm";
                    z_axis_description ="Nm";
                    break;
                case"AirCompCal.AirTab":
                    y_axis ="AirCompCal.p_PresSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"AirCompCal.PresTab":
                    y_axis ="AirCompCal.p_PresSP";
                    y_axis_description ="0";
                    z_axis_description ="0";
                    break;
                case"IdleCal.DelayDriveAct":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="ms";
                    break;
                case"IdleCal.DelayDriveRel":
                    y_axis ="IdleCal.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="ms";
                    break;
                /*case"GearCal.n_SULAtGear":
                    y_axis ="4 : 1 2 3 4 ";
                    y_axis_description ="ascii";
                    z_axis_description ="rpm";
                    break;
                case"GearCal.m_SULAtGear":
                    y_axis ="4 : 1 2 3 4 ";
                    y_axis_description ="ascii";
                    z_axis_description ="rpm";
                    break;
                case"GearCal.m_SULAtGear":
                    y_axis ="4 : 1 2 3 4 ";
                    y_axis_description ="ascii";
                    z_axis_description ="rpm";
                    break;*/
                case"HotStCal1.EnrFacTab":
                    y_axis ="HotStCal2.t_RestartSP";
                    y_axis_description ="sec";
                    z_axis_description ="0";
                    break;
                case"HotStCal1.DecrDelayTab":
                    y_axis ="HotStCal2.t_RestartSP";
                    y_axis_description ="sec";
                    z_axis_description ="0";
                    break;
                case"HotStCal2.RestartFacTab":
                    y_axis ="HotStCal2.t_RestartSP";
                    y_axis_description ="sec";
                    z_axis_description ="0";
                    break;
                case"TransCal2.EnrFacTab":
                    y_axis ="TransCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"TransCal2.DecrDelayTab":
                    y_axis ="TransCal2.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"SyncCal.n_CombMultiIgnTa":
                    y_axis ="SyncCal.T_EngMultiIgnSP";
                    y_axis_description ="øC";
                    z_axis_description ="comb";
                    break;
                case"IdleCal.LOffTab":
                    y_axis ="IdleCal.LOffSP";
                    y_axis_description ="ø";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.X_mLOffHighAltTa":
                    y_axis ="IdleCal.n_RpmDiffLOffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="0";
                    break;
                case"LambdaCal.HeatLoadLimTab":
                    y_axis ="LambdaCal.HeatRpmSP";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"LambdaCal.t_HeatFront":
                    y_axis ="LambdaCal.HeatRpmSP";
                    y_axis_description ="rpm";
                    z_axis_description ="s";
                    break;
                case"LambdaCal.PWM_HeatO2Fron":
                    y_axis ="LambdaCal.HeatRpmSP";
                    y_axis_description ="rpm";
                    z_axis_description ="%";
                    break;
                case"LambdaCal.t_HeatRear":
                    y_axis ="LambdaCal.HeatRpmSP";
                    y_axis_description ="rpm";
                    z_axis_description ="s";
                    break;
                case"LambdaCal.PWM_HeatO2Rear":
                    y_axis ="LambdaCal.HeatRpmSP";
                    y_axis_description ="rpm";
                    z_axis_description ="%";
                    break;
                case"TransCal3.EnrFacTab":
                    y_axis ="TransCal3.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"TransCal3.DecrDelayTab":
                    y_axis ="TransCal3.T_EngineSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"FuelConsCal.Tab":
                    y_axis ="FuelConsCal.AirSP";
                    y_axis_description ="mg/c";
                    z_axis_description ="0";
                    break;
                case"AirCompCal.AirLimTab":
                    y_axis ="AirCompCal.T_AirLimSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"X_AccPedalAutTAB":
                    y_axis ="X_AccPedalManSP";
                    y_axis_description ="AD";
                    z_axis_description ="%";
                    break;
                case"X_AccPedalManTAB":
                    y_axis ="X_AccPedalManSP";
                    y_axis_description ="AD";
                    z_axis_description ="%";
                    break;
                case"TransCal.AccRampFac":
                    y_axis ="TransCal.RpmSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"TransCal.DecRampFac":
                    y_axis ="TransCal.RpmSP";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"LimEngCal.TurboSpeedTab":
                    y_axis ="LimEngCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="mg/c";
                    break;
                case"LimEngCal.TurboSpeedTab2":
                    y_axis ="LimEngCal.n_EngSP";
                    y_axis_description ="rpm";
                    z_axis_description ="0";
                    break;
                case"KnkAdaptCal.ConstantTab":
                    y_axis ="KnkAdaptCal.fi_OffsetSP";
                    y_axis_description ="ø";
                    z_axis_description ="0";
                    break;
             
                case"IdleCal.n_EngLOffDrive":
                    y_axis ="IdleCal.n_EngLOffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="rpm";
                    break;
                case"IdleCal.n_EngLOffNeutral":
                    y_axis ="IdleCal.n_EngLOffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="rpm";
                    break;
                case"IdleCal.Q_LOffRpmDrive":
                    y_axis ="IdleCal.n_EngLOffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.Q_LOffRpmNeutral":
                    y_axis ="IdleCal.n_EngLOffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.T_AirTab":
                    y_axis ="IdleCal.p_AirAmbSP";
                    y_axis_description ="kPa";
                    z_axis_description ="g/s";
                    break;
                case"IdleCal.Q_AirAmbTab":
                    y_axis ="IdleCal.p_AirAmbSP";
                    y_axis_description ="kPa";
                    z_axis_description ="g/s";
                    break;
                case"PedalMapCal.X_AutFacTab":
                    y_axis ="PedalMapCal.v_VehManSP";
                    y_axis_description ="km/h";
                    z_axis_description ="0";
                    break;
                case"PedalMapCal.X_ManFacTab":
                    y_axis ="PedalMapCal.v_VehManSP";
                    y_axis_description ="km/h";
                    z_axis_description ="0";
                    break;
                case"IgnIdleCal.Tab":
                    //y_axis ="IgnIdleCal.n_EngMinSP";
                    y_axis ="IgnIdleCal.n_EngDiffSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;
                case"IgnIdleCal.fi_MinTab":
                    y_axis ="IgnIdleCal.n_EngMinSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;
                case"IgnJerkCal.n_LimTab":
                    y_axis ="IgnJerkCal.n_DiffXSP";
                    y_axis_description ="rpm";
                    z_axis_description ="0";
                    break;
                case"IgnJerkCal.fi_Tab":
                    y_axis ="IgnJerkCal.n_DiffXSP";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;
                case"DiffPSCal.M_LimitTab":
                    y_axis ="DiffPSCal.M_EngineSP";
                    y_axis_description ="Nm";
                    z_axis_description ="Nm";
                    break;
                case"DiffPSCal.t_LimDelayTab":
                    y_axis ="DiffPSCal.M_EngineSP";
                    y_axis_description ="Nm";
                    z_axis_description ="ms";
                    break;
                case"KnkAdaptCal.MaxRef":
                    y_axis ="KnkAdaptCal.fi_OffsetSP";
                    y_axis_description ="ø";
                    z_axis_description ="0";
                    break;
               /* case"KnkDetCal.KnockWinAngle":
                    y_axis ="14 : 0 500 1000 1500 2000 2500 3000 3500 4000 4500 5000 5500 6000 6500";
                    y_axis_description ="rpm";
                    z_axis_description ="ø";
                    break;*/
                case"IgnKnkCal.fi_Offset":
                    y_axis ="IgnKnkCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="ø";
                    break;
                case"IgnKnkCal.AdpLowLimTab":
                    y_axis ="IgnKnkCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="mg/c";
                    break;
                case"IgnKnkCal.AdpHighLimTab":
                    y_axis ="IgnKnkCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="mg/c";
                    break;
                case"IgnKnkCal.fi_Map1MaxOffs":
                    y_axis ="IgnKnkCal.n_EngYSP";
                    y_axis_description ="rpm (* 10)";
                    z_axis_description ="ø";
                    break;
              /*  case"IgnKnkCal.ReduceTime":
                    y_axis ="16 : 400 800 1200 1600 2000 2400 2800 3200 3600 4000 4400 4800 5200 5600 6000 6400";
                    y_axis_description ="0";
                    z_axis_description ="ø";
                    break;*/
                case"DisplAdap.LamScannerTab1":
                    y_axis ="DisplAdap.AD_ScannerSP";
                    y_axis_description ="AD";
                    z_axis_description ="Lam";
                    break;
                case"DisplAdap.LamScannerTab2":
                    y_axis ="DisplAdap.AD_ScannerSP";
                    y_axis_description ="AD";
                    z_axis_description ="Lam";
                    break;
                case"DisplAdap.LamScannerTab3":
                    y_axis ="DisplAdap.AD_ScannerSP";
                    y_axis_description ="AD";
                    z_axis_description ="Lam";
                    break;
                case"SAIDiagCal.m_AirTab":
                    y_axis ="SAIDiagCal.m_AirSP";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                case"SAIDiagCal.m_WSHAirTab1":
                    y_axis ="SAIDiagCal.m_WSHAirSP";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                case"SAIDiagCal.m_WSHAirTab2":
                    y_axis ="SAIDiagCal.m_WSHAirSP";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                case"AreaCal.Q_VenturiTab":
                    y_axis ="AreaCal.Q_VenturiSP";
                    y_axis_description ="g/s";
                    z_axis_description ="g/s";
                    break;
                case"JerkCal.T_AirInletTab":
                    y_axis ="JerkCal.p_AirAmbientSP";
                    y_axis_description ="kPa";
                    z_axis_description ="0";
                    break;
                case"JerkCal.n_EngineTab":
                    y_axis ="JerkCal.p_AirAmbientSP";
                    y_axis_description ="kPa";
                    z_axis_description ="0";
                    break;
                case"JerkCal.p_AirAmbientTab":
                    y_axis ="JerkCal.p_AirAmbientSP";
                    y_axis_description ="kPa";
                    z_axis_description ="kPa";
                    break;
                case"TorqueCal.a_FreeRollingTab":
                    y_axis ="TorqueCal.v_TorqueCalcSP";
                    y_axis_description ="km/h";
                    z_axis_description ="m/s^2";
                    break;
                case"REPCal.T_AirInletTab":
                    y_axis ="REPCal.T_AirInletSP";
                    y_axis_description ="øC";
                    z_axis_description ="kPa";
                    break;
                case"BlockHeatCal.AftSt2FacTa":
                    y_axis ="BlockHeatCal.T_AirInletS";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"BlockHeatCal.Q_StartFacT":
                    y_axis ="BlockHeatCal.T_AirInletS";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"BlockHeatCal.C_PartNFacT":
                    y_axis ="BlockHeatCal.T_AirInletS";
                    y_axis_description ="øC";
                    z_axis_description ="0";
                    break;
                case"PurgeCal.PdiffMaxRefFlow":
                    y_axis ="PurgeCal.p_Diff16XSp";
                    y_axis_description ="kPa";
                    z_axis_description ="mg/s";
                    break;
                case"PurgeCal.m_AirDerLowTab":
                    y_axis ="PurgeCal.m_AirHighSp";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                case"PurgeCal.m_AirDerHighTab":
                    y_axis ="PurgeCal.m_AirHighSp";
                    y_axis_description ="mg/c";
                    z_axis_description ="mg/c";
                    break;
                case"ExhaustCal.m_AirTau1Tab":
                    y_axis ="ExhaustCal.T_LimitSP";
                    y_axis_description ="øC";
                    z_axis_description ="s";
                    break;
                case"ExhaustCal.m_AirTau2Tab":
                    y_axis ="ExhaustCal.T_LimitSP";
                    y_axis_description ="øC";
                    z_axis_description ="s";
                    break;
                case"ExhaustCal.T_LimitTab":
                    y_axis ="ExhaustCal.T_LimitSP";
                    y_axis_description ="øC";
                    z_axis_description ="øC";
                    break;
                case"ExhaustCal.T_ExhaustTab":
                    y_axis ="ExhaustCal.T_ExhaustSP";
                    y_axis_description ="øC";
                    z_axis_description ="øC";
                    break;
                case"KnkDetCal.T_EngKnkCntrl":
                    y_axis ="KnkDetCal.T_InletAirXSP";
                    y_axis_description ="ø";
                    z_axis_description ="ø";
                    break;
                case "AirAmbientCal.X_PrRatioMinMAP":
                    x_axis = "AirAmbientCal.p_ThrRatioSP";
                    y_axis = "AirAmbientCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AirAmbientCal.X_PrRatioMaxMAP":
                    x_axis = "AirAmbientCal.p_ThrRatioSP";
                    y_axis = "AirAmbientCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AirCtrlAdap.m_IpartThrMAP":
                    x_axis = "AirCtrlCal.m_ReqAdapXSP";
                    y_axis = "AirCtrlCal.n_EngAdapYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "AirCtrlCal.m_AirBasicPressureMAP":
                    x_axis = "AirCtrlCal.p_AirAmbientXSP";
                    y_axis = "AirCtrlCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "AirCtrlCal.Ppart_BoostMap":
                    x_axis = "AirCtrlCal.PIDXSP";
                    y_axis = "AirCtrlCal.PIDYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "constant";
                    break;
                case "AirCtrlCal.Ipart_BoostMap":
                    x_axis = "AirCtrlCal.PIDXSP";
                    y_axis = "AirCtrlCal.PIDYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "constant";
                    break;
                case "AirCtrlCal.Dpart_BoostMap":
                    x_axis = "AirCtrlCal.PIDXSP";
                    y_axis = "AirCtrlCal.PIDYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "constant";
                    break;
                case "AirCtrlCal.RegMap":
                    x_axis = "AirCtrlCal.SetLoadXSP";
                    y_axis = "AirCtrlCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "%";
                    break;
                case "AirCtrlCal.p_diffReqMAP":
                    x_axis = "AirCtrlCal.m_diffPReqXSP";
                    y_axis = "AirCtrlCal.PIDYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "kPa";
                    break;
                case "AirCtrlCal.m_AirBoostTempOffsMAP":
                    x_axis = "AirCtrlCal.m_AirInletXSP";
                    y_axis = "AirCtrlCal.T_AirInletYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "AirCtrlCal.m_AirBoostHighAltOffsMAP":
                    x_axis = "AirCtrlCal.p_AfterTurbineXSP";
                    y_axis = "AirCtrlCal.p_AirAmbientYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "AirCtrlCal.m_IpartThrMaxMAP":
                    x_axis = "AirCtrlCal.m_ReqAdapXSP";
                    y_axis = "AirCtrlCal.n_EngAdapYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "AirMassMastCal.m_AirMBTMAP":
                    x_axis = "AirMassMastCal.Trq_MBTXSP";
                    y_axis = "AirMassMastCal.n_EngMBTYSP";
                    x_axis_description = "Torque (Nm*10)";
                    y_axis_description = "rpm";
                    z_axis_description = "mg/c";
                    break;
                case "BoostAdap.DiagnoseMAP":
                    x_axis = "2: 0 1";
                    y_axis = "4: 0 1 2 3";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "bool";
                    break;
                case "BoostAdapAdap.AdapMAP":
                    x_axis = "BoostAdapCal.m_AirXSP";
                    y_axis = "BoostAdapCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "BoostAdapAdap.ValidMAP":
                    x_axis = "BoostAdapCal.m_AirXSP";
                    y_axis = "BoostAdapCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "count";
                    break;
                case "FrompAdapAdap.AdapMAP":
                    x_axis = "FrompAdapCal.m_AirXSP";
                    y_axis = "FrompAdapCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "MAFCal.WeightConstMap":
                    x_axis = "MAFCal.n_EngineXSP";
                    y_axis = "MAFCal.p_InletGradYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "MAFCal.T_EngineFactorMap":
                    x_axis = "MAFCal.n_EngTEngFacSP";
                    y_axis = "MAFCal.p_InlTEngFacSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "MAFCal.NormAdjustFacMap":
                    x_axis = "MAFCal.m_AirNormAdjXSP";
                    y_axis = "MAFCal.n_EngNormAdjYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fac";
                    break;
                case "AirMinLimCal.m_MinLoadMAP":
                    x_axis = "AirMinLimCal.T_EngineSP";
                    y_axis = "AirMinLimCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "StartEngCal.Q_AirStartOffsetAutMAP":
                    x_axis = "StartEngCal.T_EngineXSP";
                    y_axis = "StartEngCal.p_AirAmbientYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "g/s";
                    break;
                case "StartEngCal.Q_AirStartOffsetManMAP":
                    x_axis = "StartEngCal.T_EngineXSP";
                    y_axis = "StartEngCal.p_AirAmbientYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "g/s";
                    break;
                case "MisfAdap.N_MisfCountMap":
                    x_axis = "MisfCal.m_AirXSP";
                    y_axis = "MisfCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "No";
                    break;
                case "CatDiagCal.t_Ph3MaxMAT":
                    x_axis = "CatDiagCal.Q_AirSP";
                    y_axis = "CatDiagCal.T_CatSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "ms";
                    break;
                case "EvapDiagCal.LeakFacTest1MAT":
                    x_axis = "EvapDiagCal.Ramp2ResultXSP";
                    y_axis = "EvapDiagCal.V_Test1FuelYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Pa/s";
                    break;
                case "EvapDiagCal.LeakFacTest2MAT":
                    x_axis = "EvapDiagCal.Ramp2ResultXSP";
                    y_axis = "EvapDiagCal.V_Test2FuelYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Pa/s";
                    break;
                case "MisfCal.m_LoadLevelMAT":
                    x_axis = "MisfCal.T_EngXSP";
                    y_axis = "MisfCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "MisfCal.CatOverheatFactorMAT":
                    x_axis = "MisfCal.m_AirXSP";
                    y_axis = "MisfCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fac";
                    break;
                case "MisfCal.t_MaxCycleMAT":
                    x_axis = "MisfCal.m_Air2XSP";
                    y_axis = "MisfCal.n_Eng2YSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "us";
                    break;
                case "CatModCal.T_SteadyStateMAP":
                    x_axis = "CatModCal.m_SteadyStateXSP";
                    y_axis = "CatModCal.n_SteadyStateYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°C";
                    break;
                case "CatModCal.TSoakFacMAP":
                    x_axis = "CatModCal.T_AirInletXSP";
                    y_axis = "CatModCal.t_SoakMinYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TAmbModCal.T_OffsetMAP":
                    x_axis = "TAmbModCal.v_VehicleSP";
                    y_axis = "TAmbModCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°C";
                    break;
                case "BFuelCal.FCIMinLoadFuelFacMAP":
                    x_axis = "BFuelCal.AirFCIXSP";
                    y_axis = "BFuelCal.RpmFCIYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "BFuelCal.LambdaOneFacMap":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fac";
                    break;
                case "BFuelCal.TempEnrichFacMap":
                case "BFuelCal.E85TempEnrichFacMap":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "Fac";
                    break;
                case "ExhaustCal.T_fiIgnMap":
                    x_axis = "ExhaustCal.m_AirSP";
                    y_axis = "ExhaustCal.fi_IgnSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°C";
                    break;
                case "FCutCal.FuelFactorMAP":
                    x_axis = "FCutCal.n_CombSinceFuelCutSP";
                    y_axis = "FCutCal.T_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "FuelAdapAdap.AdapMap":
                    x_axis = "FuelAdapCal.m_AirFuelXSP";
                    y_axis = "FuelAdapCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "FuelDynCal.m_AirInletFuelDeltaLim":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "mg/c";
                    break;
                case "FuelDynCal.m_FbetaMap1":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap2":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap3":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap4":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap5":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap6":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap7":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaMap8":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap1":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap2":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap3":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap4":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap5":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap6":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap7":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FbetaSteadyStateMap8":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;


                case "FuelDynCal.m_FalphaMap1":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap2":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap3":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap4":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap5":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap6":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap7":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaMap8":
                    x_axis = "FuelDynCal.n_EngineSP";
                    y_axis = "FuelDynCal.m_AirInletSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap1":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap2":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap3":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap4":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap5":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap6":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap7":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "FuelDynCal.m_FalphaSteadyStateMap8":
                    x_axis = "FuelDynCal.n_Engine1SP";
                    y_axis = "FuelDynCal.m_AirInlet1SP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;

                case "FMastCal.FuelCompCylMAP1":
                    x_axis = "FMastCal.m_AirInletFuelXSP";
                    y_axis = "FMastCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "FMastCal.FuelCompCylMAP2":
                    x_axis = "FMastCal.m_AirInletFuelXSP";
                    y_axis = "FMastCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "FMastCal.FuelCompCylMAP3":
                    x_axis = "FMastCal.m_AirInletFuelXSP";
                    y_axis = "FMastCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "FMastCal.FuelCompCylMAP4":
                    x_axis = "FMastCal.m_AirInletFuelXSP";
                    y_axis = "FMastCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "KnkFuelCal.fi_OffsetEnrichEnable":
                    x_axis = "IgnKnkCal.m_AirXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkFuelCal.fi_MaxOffsetMap":
                    x_axis = "KnkFuelCal.m_AirXSP";
                    y_axis = "BstKnkCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°";
                    break;

                case "PurgeCal.ValveMap16EU":
                    x_axis = "PurgeCal.p_Diff16XSp";
                    y_axis = "PurgeCal.q_Flow16YSp";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "PurgeCal.ValveMap16US":
                    x_axis = "PurgeCal.p_Diff16XSp";
                    y_axis = "PurgeCal.q_Flow16YSp";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "StartCal.CombFacAutMAP":
                    x_axis = "StartCal.T_EngineXSP";
                    y_axis = "StartCal.n_CombYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "StartCal.CombFacManMAP":
                    x_axis = "StartCal.T_EngineXSP";
                    y_axis = "StartCal.n_CombYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "TransFuelCal.rampFacMAP":
                    x_axis = "TransFuelCal.p_deltaSP";
                    y_axis = "TransFuelCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TransFuelCal.facMAP":
                    x_axis = "TransFuelCal.p_deltaSP";
                    y_axis = "TransFuelCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AfterStCal.StartMAP":
                    x_axis = "AfterStCal.n_CombXSP";
                    y_axis = "AfterStCal.T_EngineYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AfterStCal.AmbientMAP":
                    x_axis = "AfterStCal.n_CombXSP";
                    y_axis = "AfterStCal.p_AirAmbientYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AfterStCal.HotSoakMAP":
                    x_axis = "AfterStCal.t_soakXSP";
                    y_axis = "AfterStCal.T_EngineYSP2";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "AfterStCal.HotSoakAdjustMAP":
                    x_axis = "AfterStCal.n_CombXSP2";
                    y_axis = "AfterStCal.T_EngineYSP2";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "%";
                    break;
                case "TCompCal.FuelFacMap1":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap2":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap3":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap4":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap5":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap6":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap7":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TCompCal.FuelFacMap8":
                    x_axis = "TCompCal.m_AirInletSP";
                    y_axis = "TCompCal.n_EngineSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "IgnAbsCal.fi_IgnMBTMAP":
                    x_axis = "IgnAbsCal.m_AirMBTXSP";
                    y_axis = "IgnAbsCal.n_EngMBTYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnAbsCal.fi_highOctanMAP":
                    x_axis = "IgnAbsCal.m_AirNormXSP";
                    y_axis = "IgnAbsCal.n_EngNormYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnAbsCal.fi_lowOctanMAP":
                    x_axis = "IgnAbsCal.m_AirNormXSP";
                    y_axis = "IgnAbsCal.n_EngNormYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnAbsCal.fi_NormalMAP":
                    x_axis = "IgnAbsCal.m_AirNormXSP";
                    y_axis = "IgnAbsCal.n_EngNormYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnAbsCal.fi_FuelCutMAP":
                    x_axis = "IgnAbsCal.T_EngFuelCutXSP";
                    y_axis = "IgnAbsCal.n_EngFuelCutYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°";
                    break;
                case "IgnAbsCal.fi_StartMAP":
                    x_axis = "IgnAbsCal.T_EngStartXSP";
                    y_axis = "IgnAbsCal.n_EngStartYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°";
                    break;
                case "IgnMastCal.MinMap":
                    x_axis = "IgnMastCal.m_AirXSP";
                    y_axis = "IgnMastCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°";
                    break;
                case "IgnMastCal.dwellTimeMap":
                    x_axis = "IgnMastCal.UBattDwellTimeXSP";
                    y_axis = "IgnMastCal.n_EngDwellTimeYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "uS";
                    break;
                case "IgnTransCal.factorMAP":
                    x_axis = "IgnTransCal.p_AirInletXSP";
                    y_axis = "IgnTransCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "IgnKnkCal.RetardIndexMap":
                    x_axis = "IgnKnkCal.m_AirIndexXSP";
                    y_axis = "IgnKnkCal.n_EngIndexYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "constant";
                    break;
                case "IgnKnkCal.ARetardIndexMap":
                    x_axis = "IgnKnkCal.m_AirIndexXSP";
                    y_axis = "IgnKnkCal.n_EngIndexYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "constant";
                    break;
                case "IgnTempCal.T_AirInletReferenceMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°C";
                    break;
                case "IgnTempCal.fi_OffsetMaxAirInletMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°";
                    break;
                case "IgnTempCal.T_MaxAirInletMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°C";
                    break;
                case "IgnTempCal.fi_OffsetMinAirInletMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnTempCal.T_MinAirInletMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°C";
                    break;
                case "IgnTempCal.fi_OffsetMaxTEngMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "IgnTempCal.fi_OffsetMinTEngMap":
                    x_axis = "IgnTempCal.m_AirXSP";
                    y_axis = "IgnTempCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkSoundRedCal.fi_OffsMap":
                    x_axis = "IgnAbsCal.m_AirNormXSP";
                    y_axis = "IgnAbsCal.n_EngNormYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkDetAdap.KnkCntMAP":
                    x_axis = "IgnAbsCal.m_AirNormXSP";
                    y_axis = "IgnAbsCal.n_EngNormYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Num";
                    break;
                case "KnkDetCal.fi_knkWinOffsMAP":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkDetCal.fi_knkWinSizeMAP":
                    x_axis = "KnkDetCal.m_AirXSP";
                    y_axis = "KnkDetCal.n_EngYSP";
                    x_axis_description = "mg/c";
                    y_axis_description = "rpm";
                    z_axis_description = "°";
                    break;
                case "KnkDetCal.X_hystOffsetMAP":
                    x_axis = "KnkDetCal.m_AirNoiseXSP";
                    y_axis = "KnkDetCal.n_EngNoiseYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "count";
                    break;
                case "KnkDetCal.X_AHystOffsetMAP":
                    x_axis = "KnkDetCal.m_AirNoiseXSP";
                    y_axis = "KnkDetCal.n_EngNoiseYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "count";
                    break;
                case "MisfAveCal.MFIndexMAP":
                    x_axis = "MisfAveCal.m_AirMFIndexXSP";
                    y_axis = "MisfAveCal.n_EngMFIndexYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "SystemActionCal.BefThrOffsetMAP":
                    x_axis = "SystemActionCal.A_ThrottleXSP";
                    y_axis = "SystemActionCal.m_AirInletYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "kPa";
                    break;
                case "SystemActionCal.PwrLimFanMAP":
                    x_axis = "SystemActionCal.v_VehicleXSP";
                    y_axis = "SystemActionCal.T_AirInletYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "kW";
                    break;
                case "OilTempCal.T_SpeedAndLoadDecreaseMap":
                    x_axis = "OilTempCal.T_AirInletXSP";
                    y_axis = "OilTempCal.v_VehicleYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "°C";
                    break;
                case "ACCompCal.Trq_ReqStaticMAP":
                    x_axis = "ACCompCal.p_ACXSP";
                    y_axis = "ACCompCal.n_EngineYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "ACCompCal.Trq_ReqDynamicMAP":
                    x_axis = "ACCompCal.p_ACXSP";
                    y_axis = "ACCompCal.n_EngineYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "DNCompCal.Trq_ReqNeutralMAP":
                    x_axis = "DNCompCal.T_TrnXSP";
                    y_axis = "DNCompCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "DNCompCal.Trq_ReqDriveMAP":
                    x_axis = "DNCompCal.T_TrnXSP";
                    y_axis = "DNCompCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "ElCompCal.Trq_FTermMap":
                    x_axis = "ElCompCal.n_FTermXSP";
                    y_axis = "ElCompCal.PWM_FTermYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "EngTipLimCal.X_Koeff":
                    x_axis = "6: A B C D E F";
                    y_axis = "40: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "EngTipLimCal.Trq_BacklashMap":
                    x_axis = "EngTipLimCal.n_DiffDeltaXSP";
                    y_axis = "EngTipLimCal.n_DiffYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "EngTipLimNormCal.Trq_BacklashTipOutMap":
                    x_axis = "EngTipLimNormCal.n_TipSP";
                    y_axis = "EngTipLimNormCal.ST_GearYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "EngTipLimNormCal.N_TipInMap":
                    x_axis = "EngTipLimNormCal.n_TipSP";
                    y_axis = "21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "No";
                    break;
                case "EngTipLimNormCal.N_TipOutMap":
                    x_axis = "EngTipLimNormCal.n_TipSP";
                    y_axis = "21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "No";
                    break;
                case "EngTipLimSportCal.Trq_BacklashTipOutMap":
                    x_axis = "EngTipLimSportCal.n_TipSP";
                    y_axis = "EngTipLimSportCal.ST_GearYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "EngTipLimSportCal.N_TipInMap":
                    x_axis = "EngTipLimSportCal.n_TipSP";
                    y_axis = "21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "No";
                    break;
                case "EngTipLimSportCal.N_TipOutMap":
                    x_axis = "EngTipLimSportCal.n_TipSP";
                    y_axis = "21: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "No";
                    break;
                case "FrictionLoadCal.Trq_RequestT_EngMAP":
                    x_axis = "FrictionLoadCal.T_EngXSP";
                    y_axis = "FrictionLoadCal.n_EngNomYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "LightOffCal.Trq_ReserveMAP":
                    x_axis = "LightOffCal.Trq_RequestXSP";
                    y_axis = "LightOffCal.n_EngYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "PedalMapCal.Trq_RequestMap":
                    x_axis = "PedalMapCal.n_EngineMap";
                    y_axis = "PedalMapCal.X_PedalMap";
                    x_axis_description = "rpm";
                    y_axis_description = "TPS";
                    z_axis_description = "Nm";
                    break;
                case "PedalMapCal.Trq_RequestSportMap":
                    x_axis = "PedalMapCal.n_EngineMap";
                    y_axis = "PedalMapCal.X_PedalMap";
                    x_axis_description = "rpm";
                    y_axis_description = "TPS";
                    z_axis_description = "Nm";
                    break;
                case "PedalMapCal.GainFactorMap":
                    x_axis = "PedalMapCal.n_EngineMap";
                    y_axis = "PedalMapCal.GainFactorYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TOACal.Trq_P_partMAP":
                    x_axis = "TOACal.v_vehicleXSP";
                    y_axis = "TOACal.n_EngDiffYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "RpmCtrlCal.P_PartIgnMAP":
                    x_axis = "RpmCtrlCal.v_VehicleXSP";
                    y_axis = "RpmCtrlCal.P_PartIgnYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "RpmCtrlCal.P_PartAirMAP":
                    x_axis = "RpmCtrlCal.v_VehicleXSP";
                    y_axis = "RpmCtrlCal.P_PartAirYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "RpmCtrlCal.D_FacIgnMAP":
                    x_axis = "RpmCtrlCal.v_VehicleXSP";
                    y_axis = "RpmCtrlCal.P_PartIgnYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Fac";
                    break;
                case "TrqLimCal.Trq_CompressorNoiseRedLimMAP":
                    x_axis = "TrqLimCal.CompressorNoiseXSP";
                    y_axis = "TrqLimCal.CompressorNoiseYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "TrqMastCal.X_AccPedalMAP":
                    x_axis = "TrqMastCal.n_EngineXSP";
                    y_axis = "TrqMastCal.Trq_PedYSP";
                    //x_axis_description = "mg/c";
                    //y_axis_description = "rpm";
                    x_axis_description = "rpm";
                    y_axis_description = "Torque (Nm * 10)";

                    
                    z_axis_description = "%";
                    break;
                case "TrqMastCal.Trq_MaxDerIncMAP":
                    x_axis = "TrqMastCal.n_MaxDerXSP";
                    y_axis = "TrqMastCal.T_MaxDerYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "TrqMastCal.Trq_MaxDerDecMAP":
                    x_axis = "TrqMastCal.n_MaxDerXSP";
                    y_axis = "TrqMastCal.T_MaxDerYSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "TMCCal.Trq_FrictionMAP":
                    x_axis = "TMCCal.T_EngSP";
                    y_axis = "TMCCal.n_Engine1YSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "TMCCal.Trq_NeutralMAP":
                    x_axis = "TMCCal.T_TrnOilSP";
                    y_axis = "TMCCal.n_Engine2YSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;
                case "TMCCal.Trq_DriveMAP":
                    x_axis = "TMCCal.T_TrnOilSP";
                    y_axis = "TMCCal.n_Engine3YSP";
                    x_axis_description = "";
                    y_axis_description = "";
                    z_axis_description = "Nm";
                    break;

                    // van hier

                case "obdLTFT.AdapMap":
                    x_axis = "FuelAdapCal.m_AirFuelXSP";
                    y_axis = "FuelAdapCal.n_EngYSP";
                    break;
                case "obdLTFT.AdapTab":
                    x_axis = "FuelAdapCal.T_EngineSP";
                    y_axis = "";
                    break;
                case "FFCatDiagCal.X_blendFacTab":
                    x_axis = "FFCatDiagCal.X_blendFacTabSP";
                    y_axis = "";
                    break;
                case "FFCatModCal.T_SteadyStateMAP":
                    x_axis = "CatModCal.m_SteadyStateXSP";
                    y_axis = "CatModCal.n_SteadyStateYSP";
                    break;
                case "FFCatModCal.X_SSTempInterpolWeightTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FCutCal.X_combAdjustTAB":
                    x_axis = "FCutCal.n_CombInFuelCutSP";
                    y_axis = "";
                    break;
                case "TrqLimCal.Trq_MaxEngineTab1":
                    x_axis = "";
                    y_axis = "TrqLimCal.n_EngYSP";
                    break;
                case "TrqLimCal.Trq_MaxEngineTab2":
                    x_axis = "";
                    y_axis = "TrqLimCal.n_EngYSP";
                    break;
                case "BFuelCal.t_JerkDelayTAB":
                    x_axis = "BFuelCal.RpmYSP";
                    y_axis = "";
                    break;
                case "O2SHeatFrontCal.m_exhaust_lightOffTAB":
                    x_axis = "O2SHeatFrontCal.T_engLightOffSP";
                    y_axis = "";
                    break;
                case "O2SHeatFrontCal.m_exhaust_normalTAB":
                    x_axis = "O2SHeatFrontCal.T_engNormalSP";
                    y_axis = "";
                    break;
                case "O2SHeatRearCal.m_exhaust_lightOffTAB":
                    x_axis = "O2SHeatRearCal.T_engLightOffSP";
                    y_axis = "";
                    break;
                case "O2SHeatRearCal.m_exhaust_normalTAB":
                    x_axis = "O2SHeatRearCal.T_engNormalSP";
                    y_axis = "";
                    break;
                case "FFBoffCal.Q_fuelConstRateMAP":
                    x_axis = "FFBoffCal.n_eng3SP";
                    y_axis = "FFBoffCal.p_diff6SP";
                    break;
                case "FFFDynProt.betaTAB":
                    x_axis = "T_engSP";
                    y_axis = "";
                    break;
                case "FFFDynProt.alphaTAB":
                    x_axis = "T_engSP";
                    y_axis = "";
                    break;
                case "AirCtrlCal.Ppart_BoostAirM1TAB":
                    x_axis = "AirCtrlCal.p_diffThrotSP";
                    y_axis = "";
                    break;
                case "AreaCal.Cd_DischargeMAP":
                    x_axis = "AreaCal.m_AirInletSP";
                    y_axis = "AreaCal.n_EngineSP";
                    break;
                case "StartCal.N_combFacRampTAB":
                    x_axis = "StartCal.T_EngineXSP";
                    y_axis = "";
                    break;
                case "KnkFuelCal.X_weightMAP":
                    x_axis = "KnkFuelCal.X_blendXSP";
                    y_axis = "KnkFuelCal.X_weightYSP";
                    break;
                case "FFTrqCal.FFTrq_MaxEngineTab1":
                    x_axis = "";
                    y_axis = "TrqLimCal.n_EngYSP";
                    break;
                case "FFTrqCal.FFTrq_MaxEngineTab2":
                    x_axis = "";
                    y_axis = "TrqLimCal.n_EngYSP";
                    break;
                case "FFTrqCal.X_torqueTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFTrqCal.M_maxMAP":
                    x_axis = "FFTrqCal.T_engSP5";
                    y_axis = "FFFuelCal.X_blendSP5";
                    y_axis_description = "Ethanol %";
                    x_axis_description = "Coolant temperature";
                    break;
                case "GearCal.t_HoldNeutral":
                    x_axis = "GearCal.T_TrnXSP";
                    y_axis = "";
                    break;
                case "BlockHeatCal.X_reStartFacTAB":
                    x_axis = "BlockHeatCal.t_soakSP";
                    y_axis = "";
                    break;
                case "BlockHeatCal.X_AftReStartFacTAB":
                    x_axis = "BlockHeatCal.t_soakSP";
                    y_axis = "";
                    break;
                case "LambdaCal.IntStepGainTAB":
                    x_axis = "LambdaCal.U_O2SensRearErrorSP";
                    y_axis = "";
                    break;
                case "LambdaCal.IntStepGainMisfireTAB":
                    x_axis = "LambdaCal.U_O2SensRearErrorSP";
                    y_axis = "";
                    break;
                case "LambdaCal.DecStepMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.DecRampMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.IncStepMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.IncRampMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.U_O2SensSwitchMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.U_O2SensSwitchTAB":
                    x_axis = "LambdaCal.T_SwitchPointSP";
                    y_axis = "";
                    break;
                case "LambdaCal.U_O2SensSwitchAmbTAB":
                    x_axis = "LambdaCal.p_SwitchPointAmbSP";
                    y_axis = "";
                    break;
                case "LambdaCal.U_O2SensRearRefSigMAP":
                    x_axis = "LambdaCal.m_AirInletSP";
                    y_axis = "LambdaCal.n_EngineSP";
                    break;
                case "LambdaCal.n_CombDelayWCFTAB":
                    x_axis = "LambdaCal.T_delayWCFSP";
                    y_axis = "";
                    break;
                case "LambdaPostCal.U_pPartTAB":
                    x_axis = "LambdaPostCal.U_pPartSP";
                    y_axis = "";
                    break;
                case "AfterStCal.WCFMAP":
                    x_axis = "AfterStCal.n_WCFSP";
                    y_axis = "AfterStCal.T_WCFSP";
                    break;
                case "AfterStCal.n_delayWCFTAB":
                    x_axis = "AfterStCal.T_WCFSP";
                    y_axis = "";
                    break;
                case "TAirCompCal.FuelFacTAB":
                    x_axis = "TAirCompCal.T_AirInletSP";
                    y_axis = "";
                    break;
                case "VIOSSensorCal.X_FFSensorTAB":
                    x_axis = "VIOSSensorCal.f_FFSensorSP";
                    y_axis = "";
                    break;
                case "FFIgnCal.o_octaneTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFIgnCal.n_SyncDelayTAB":
                    x_axis = "SyncCal.T_EngSP";
                    y_axis = "";
                    break;
                case "FFIgnCal.X_hystOffsetMAP":
                    x_axis = "KnkDetCal.m_AirNoiseXSP";
                    y_axis = "KnkDetCal.n_EngNoiseYSP";
                    break;
                case "FFIgnCal.X_ignTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "CatDiagCal.t_Ph2MaxMAT":
                    x_axis = "CatDiagCal.Q_AirSP";
                    y_axis = "CatDiagCal.T_CatSP";
                    break;
                case "O2SRespDiagCal.CombMulFacMAT":
                    x_axis = "O2SRespDiagCal.m_AirmassXSP";
                    y_axis = "O2SRespDiagCal.n_EngYSP";
                    break;
                case "SAIDiagCal.UBattMulFacTAB":
                    x_axis = "SAIDiagCal.U_BattMulFacSP";
                    y_axis = "";
                    break;
                case "SAIDiagCal.TAmbMulFacTAB":
                    x_axis = "SAIDiagCal.T_AmbMulFacSP";
                    y_axis = "";
                    break;
                case "SAIDiagCal.fiIgnMulFacTAB":
                    x_axis = "SAIDiagCal.fi_IgnMulFacSP";
                    y_axis = "";
                    break;
                case "SAIDiagCal.QAirMulFacTAB":
                    x_axis = "SAIDiagCal.Q_AirMulFacSP";
                    y_axis = "";
                    break;
                case "SAIDiagCal.pAirAmbientMulFacTAB":
                    x_axis = "SAIDiagCal.pAirAmbientMulFacSP";
                    y_axis = "";
                    break;
                case "SAIDiagCal.mFuelMulFacTAB":
                    x_axis = "SAIDiagCal.m_FuelMulFacSP";
                    y_axis = "";
                    break;
                case "FFRfuelCal.V_fuelTankTAB":
                    x_axis = "FFRfuelCal.AD_fuelTankSP11";
                    y_axis = "";
                    break;
                case "FFRfuelCal.V_ErrorTankTAB":
                    x_axis = "FFRfuelCal.V_TankSP6";
                    y_axis = "";
                    break;
                case "FFBladCal.M_lowLimSysActTab":
                    x_axis = "FFBladCal.n_LimSysActSP7";
                    y_axis = "";
                    break;
                case "FFBladCal.X_HLBlendOffset4TAB":
                    x_axis = "FFBladCal.n_eng4SP";
                    y_axis = "";
                    break;
                case "FuelDynCal.alphaStartTAB":
                    x_axis = "FuelDynCal.T_EngineSP";
                    y_axis = "";
                    break;
                case "FuelDynCal.X_negFuelLimStartTAB":
                    x_axis = "FuelDynCal.T_eng6SP";
                    y_axis = "";
                    break;
                case "FuelDynCal.alphaRampStepTAB":
                    x_axis = "FuelDynCal.T_EngineSP";
                    y_axis = "";
                    break;
                case "FuelDynCal.WCFbetaTAB":
                    x_axis = "FuelDynCal.WCFbetaSP";
                    y_axis = "";
                    break;
                case "FuelDynCal.X_negFuelLimMAP":
                    x_axis = "FuelDynCal.T_eng6SP";
                    y_axis = "FuelDynCal.T_engStarted5SP";
                    break;
                case "FuelDynCal.betaMAP1":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP2":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP3":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP4":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP5":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP6":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP7":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.betaMAP8":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP1":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP2":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP3":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP4":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP5":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP6":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP7":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaSSMAP8":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP1":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP2":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP3":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP4":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP5":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP6":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP7":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynCal.alphaMAP8":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FuelDynProt.betaTAB":
                    x_axis = "FuelDynProt.T_EngineSP";
                    y_axis = "";
                    break;
                case "FuelDynProt.alphaTAB":
                    x_axis = "FuelDynProt.T_EngineSP";
                    y_axis = "";
                    break;
                case "ElevIdleCal.n_EngElevIdleClutchActTAB":
                    x_axis = "DNCompCal.T_EngSP";
                    y_axis = "";
                    break;
                case "ElevIdleCal.n_ElevIdleACTAB":
                    x_axis = "ElevIdleCal.T_ElevIdleACSP";
                    y_axis = "";
                    break;
                case "FFFDynCal.X_negFuelLimMAP":
                    x_axis = "FFFDynCal.T_eng6SP";
                    y_axis = "FFFDynCal.T_engStarted5SP";
                    break;
                case "FFFDynCal.alphaStartTAB":
                    x_axis = "FuelDynCal.T_EngineSP";
                    y_axis = "";
                    break;
                case "FFFDynCal.betaMAP1":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP2":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP3":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP4":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP5":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP6":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP7":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.betaMAP8":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP1":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP2":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP3":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP4":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP5":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP6":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP7":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFFDynCal.alphaMAP8":
                    x_axis = "FuelDynCal.m_AirInletSP";
                    y_axis = "FuelDynCal.n_EngineSP";
                    break;
                case "FFAirCal.m_maxAirmass":
                    x_axis = "FFAirCal.fi_offsetXSP";
                    y_axis = "BstKnkCal.n_EngYSP";

                    /*
                     * FFAirCal.fi_offsetXSP
                        mapPointer
                        BstKnkCal.n_EngYSP
                        In.n_Engine
                     */
                    break;
                case "FFAirCal.m_MinLoadMAP":
                    x_axis = "AirMinLimCal.T_EngineSP";
                    y_axis = "AirMinLimCal.n_EngineSP";
                    break;
                case "FFFuelCal.KnkEnrichmentMAP":
                    x_axis = "FFFuelCal.m_airXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    break;
                case "FFFuelCal.fi_offsetEnrichEnableMAP":
                    x_axis = "FFFuelCal.m_airXSP";
                    y_axis = "IgnKnkCal.n_EngYSP";
                    break;
                case "FFFuelCal.AFRTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.enrFacTAB":
                    x_axis = "FFFuelCal.T_engSP12";
                    y_axis = "";
                    break;
                case "FFFuelCal.EnrFacThrFaultTAB":
                    x_axis = "FFFuelCal.T_engSP12";
                    y_axis = "";
                    break;
                case "FFFuelCal.combFacMAP":
                    x_axis = "FFFuelCal.T_engSP12";
                    y_axis = "FFFuelCal.n_combSP";
                    break;
                case "FFFuelCal.afterStartMAP":
                    x_axis = "FFFuelCal.n_afterStartSP";
                    y_axis = "FFFuelCal.T_engSP14";
                    break;
                case "FFFuelCal.TCompMAP1":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP2":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP3":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP4":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP5":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP6":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP7":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.TCompMAP8":
                    x_axis = "FFFuelCal.m_airInletTCompSP";
                    y_axis = "FFFuelCal.n_engTCompSP";
                    break;
                case "FFFuelCal.lambdaIntLimTAB":
                    x_axis = "FFFuelCal.T_engSP7";
                    y_axis = "";
                    break;
                case "FFFuelCal.InjAngTAB":
                    x_axis = "FFFuelCal.T_engSP4";
                    y_axis = "";
                    break;
                case "FFFuelCal.RestartFacMAP":
                    x_axis = "FFFuelCal.T_engSP4_Restart";
                    y_axis = "StartCal.t_RestartXSP";
                    break;
                case "FFFuelCal.AfterStRestartFacMAP":
                    x_axis = "FFFuelCal.T_engSP4_Restart";
                    y_axis = "StartCal.t_RestartXSP";
                    break;
                case "FFFuelCal.X_hotSoakAdjustMAP":
                    x_axis = "FFFuelCal.n_combSP4";
                    y_axis = "FFFuelCal.T_engSP2";
                    break;
                case "FFFuelCal.m_ExhaustO2SHeatRearLightOffTAB":
                    x_axis = "O2SHeatRearCal.T_engLightOffSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.m_ExhaustO2SHeatRearNormalTAB":
                    x_axis = "O2SHeatRearCal.T_engNormalSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.t_JerkDelayTAB":
                    x_axis = "BFuelCal.RpmYSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.BFuelAirMassJerkTAB":
                    x_axis = "BFuelCal.RpmYSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.U_LambdaO2SensSwitchFacTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_LambdaDecRampTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_LambdaIncRampTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.LambdaRampTempScaleTAB":
                    x_axis = "FFFuelCal.T_engSP5";
                    y_axis = "";
                    break;
                case "FFFuelCal.BlkHeatStartFuelFacTAB":
                    x_axis = "BlockHeatCal.T_BlkHeatSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.BlkHeatAfterStFuelFacTAB":
                    x_axis = "BlockHeatCal.T_BlkHeatSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_DNFuelFacMAP":
                    x_axis = "FFFuelCal.n_DNFuelSP5";
                    y_axis = "FFFuelCal.T_DNFuelSP5";
                    break;
                case "FFFuelCal.BlkHeatReStFuelFacTAB":
                    x_axis = "FFFuelCal.t_soakSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.BlkHeatAftReStFuelFacTAB":
                    x_axis = "FFFuelCal.t_soakSP";
                    y_axis = "";
                    break;
                case "FFFuelCal.U_02SensRearTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.LambdaRampTAB":
                    x_axis = "FFFuelCal.Q_LambdaRampSP6";
                    y_axis = "";
                    break;
                case "FFFuelCal.TempEnrichFacMAP":
                    x_axis = "BFuelCal.AirXSP";
                    y_axis = "BFuelCal.RpmYSP";
                    break;
                case "FFFuelCal.X_fuelTAB":
                    x_axis = "FFFuelCal.X_blendSP12";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_fuelCPTAB":
                    x_axis = "FFFuelCal.X_blendSP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_hotSoakTAB":
                    x_axis = "FFFuelCal.X_blend2SP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.X_fuelBlkHeatTAB":
                    x_axis = "FFFuelCal.X_blend2SP9";
                    y_axis = "";
                    break;
                case "FFFuelCal.BFuelJerkEnrichFacTAB":
                    x_axis = "BFuelCal.RpmYSP";
                    y_axis = "";
                    break;

                    // tot hier


            }
            if(z_axis_description.StartsWith("0")) z_axis_description ="z-axis";
            if(x_axis_description.StartsWith("0")) x_axis_description ="x-axis";
            if(y_axis_description.StartsWith("0")) y_axis_description ="y-axis";
            if (y_axis !="") retval = true;
            return retval;
        }

        public string GetXaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                retval ="Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                retval ="Ign_map_1_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                retval ="Ign_map_2_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                retval ="Ign_map_3_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                retval ="Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                retval ="Ign_map_5_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                retval ="Ign_map_6_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                retval ="Ign_map_7_x_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                retval ="Ign_map_8_x_axis!";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                retval ="Overs_tab_xaxis!";
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                retval ="Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval ="Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                retval ="Dash_trot_axis!";
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                retval ="Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Adapt_korr"))
            {
                retval ="Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                retval ="Idle_st_last!";
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                retval ="Trans_x_st!";
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                retval ="Reg_last!";
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                retval ="Reg_last!";
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                retval ="Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                retval ="Fuel_knock_xaxis!";
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                retval ="Misfire_map_x_axis!";
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                retval ="Misfire_map_x_axis!";
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                retval ="Ign_map_0_x_axis!";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                retval ="Temp_reduce_x_st!";
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                retval ="Detect_map_x_axis!";
            }
            else if ((symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!")))
            {
                retval ="Pwm_ind_trot!";
            }
            return retval;
        }

        public string GetYaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                retval ="Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                retval ="Ign_map_1_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                retval ="Ign_map_2_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                retval ="Ign_map_3_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                retval ="Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                retval ="Ign_map_5_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                retval ="Ign_map_6_y_axis!";
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                retval ="Ign_map_7_y_axis!";
            }
            else if (symbolname.StartsWith("Open_loop_adapt!") || symbolname.StartsWith("Open_loop!") || symbolname.StartsWith("Open_loop_knock!"))
            {
                retval ="Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Fload_tab!") || symbolname.StartsWith("Fload_throt_tab!"))
            {
                retval ="Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                retval ="Ign_map_8_y_axis!";
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                retval ="Temp_steg";
            }
            else if (symbolname.StartsWith("Kyltemp_tab!"))
            {
                retval ="Kyltemp_steg!";
            }
            else if (symbolname.StartsWith("Lufttemp_tab!"))
            {
                retval ="Lufttemp_steg!";
            }
            else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                retval ="Lufttemp_steg!";
            }
            else if (symbolname.StartsWith("Derivata_br_tab_pos!") || symbolname.StartsWith("Derivata_br_tab_neg!"))
            {
                retval ="Derivata_br_sp!";
            }
            else if (symbolname.StartsWith("I_last_rpm!") || symbolname.StartsWith("Last_reg_ac!"))
            {
                retval ="Last_varv_st!";
            }
            else if (symbolname.StartsWith("I_last_temp!"))
            {
                retval ="Last_temp_st!";
            }
            else if (symbolname.StartsWith("Iv_start_time_tab!"))
            {
                retval ="I_kyl_st!";
            }
            else if (symbolname.StartsWith("Idle_start_extra!"))
            {
                retval ="Idle_start_extra_sp!";
            }
            else if (symbolname.StartsWith("Restart_corr_hp!"))
            {
                retval ="Hp_support_points!";
            }
            else if (symbolname.StartsWith("Lam_minlast!"))
            {
                retval ="Fuel_map_yaxis";
            }
            else if (symbolname.StartsWith("Lamb_tid!") || symbolname.StartsWith("Lamb_idle!") || symbolname.StartsWith("Lamb_ej!"))
            {
                retval ="Lamb_kyl!";
            }
            else if (symbolname.StartsWith("Overstid_tab!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("AC_wait_on!") || symbolname.StartsWith("AC_wait_off!"))
            {
                retval ="I_luft_st!";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Idle_temp_off!") || symbolname.StartsWith("Idle_rpm_tab!") || symbolname.StartsWith("Start_tab!"))
            {
                retval ="I_kyl_st!";
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                retval ="Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval ="Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Max_regl_temp_"))
            {
                retval ="Max_regl_sp!";
            }
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                retval ="Knock_wind_rpm!";
            }
            else if (symbolname.StartsWith("Knock_ref_tab!"))
            {
                retval ="Knock_ref_rpm!";
            }
            else if (symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_press_tab!") || symbolname.StartsWith("Lknock_oref_tab!") || symbolname.StartsWith("Knock_lim_tab!"))
            {
                retval ="Wait_count_tab!";
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                retval ="Dash_rpm_axis!";
            }
            else if (symbolname.StartsWith("Adapt_korr"))
            {
                retval ="Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                retval ="Idle_st_rpm!";
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                retval ="Trans_y_st!";
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                retval ="Trans_y_st!";
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                retval ="Trans_y_st!";
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                retval ="Trans_y_st!";
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                retval ="Reg_varv!";
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                retval ="Reg_varv!";
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                retval ="Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Regl_tryck_sgm!") || symbolname.StartsWith("Regl_tryck_fgm!") || symbolname.StartsWith("Regl_tryck_fgaut!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                retval ="Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                retval ="Misfire_map_y_axis!";
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                retval ="Misfire_map_y_axis!";
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_") || symbolname.StartsWith("Tempkomp_konst!") || symbolname.StartsWith("Accel_temp!") || symbolname.StartsWith("Accel_temp2!") || symbolname.StartsWith("Retard_temp!") || symbolname.StartsWith("Throt_after_tab!") || symbolname.StartsWith("Throt_aft_dec_fak!"))
            {
                retval ="Temp_steg!";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                retval ="Temp_reduce_y_st!";
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                retval ="Ign_map_0_y_axis!";
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                retval ="Detect_map_y_axis!";
            }
            else if (symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!"))
            {
                retval ="Pwm_ind_rpm!";
            }
            return retval;
        }

    }
}
