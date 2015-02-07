using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonSuite;

namespace T8SuitePro
{
    class SymbolFiller
    {
        public bool CheckAndFillCollection(SymbolCollection sc)
        {
            bool retval = false;
            // first check whether we have a "blank" file
            bool _hasSymbolNumbers = false;
            sc.SortColumn = "Symbol_number";
            sc.SortingOrder = GenericComparer.SortOrder.Ascending;
            sc.Sort();
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Varname.StartsWith("Symbolnumber"))
                {
                    _hasSymbolNumbers = true;
                    break;
                }
            }
            // check known symbol length
            if (_hasSymbolNumbers)
            {
                int MapIndex = 0;

                int KnkFuelCalfi_MaxOffsetMap_Index = 0;
                int TrqMastCalm_AirTorqMap_Index = 0;
                int TrqMastCalX_AccPedalMAP_Index = 0;
                int BstKnkCalMaxAirmass_Index = 0;
                int BstKnkCalMaxAirmassAu_Index = 0;
                int PedalMapCalTrq_RequestMap_Index = 0;
                int PedalMapCalTrq_RequestSportMap_Index = 0;
                int KnkDetCalfi_knkWinOffsMAP_Index = 0;
                int KnkDetCalfi_knkWinSizeMAP_Index = 0;
                int InjAnglCalMap_Index = 0;
                bool _reverse288Maps = false;

                if (SequenceOf512Maps(sc, 1, 2, 1, 2, 2, 1))
                {
                    if (SequenceOf576Maps(sc, 1, 1, 1, 2, 1, 2)) // new file from JZW
                    {
                        //2 = KnkDetCal.fi_KnkWinOffsMap
                        //3 = KnkDetCal.fi_KnkWinSizeMap
                        //7 = TrqMastCal.m_AirTorqMap
                        //8 = TrqMastCal.X_AccPedalMAP
                        
                        KnkFuelCalfi_MaxOffsetMap_Index = 4;
                        KnkDetCalfi_knkWinOffsMAP_Index = 2;
                        KnkDetCalfi_knkWinSizeMAP_Index = 3;

                        InjAnglCalMap_Index = 1;

                        PedalMapCalTrq_RequestMap_Index = 5;
                        PedalMapCalTrq_RequestSportMap_Index = 6;
                        TrqMastCalm_AirTorqMap_Index = 7;
                        TrqMastCalX_AccPedalMAP_Index = 8;
                        BstKnkCalMaxAirmass_Index = 9;
                        _reverse288Maps = true;

                    }
                    else
                    {
                        KnkFuelCalfi_MaxOffsetMap_Index = 1;
                        TrqMastCalm_AirTorqMap_Index = 2;
                        TrqMastCalX_AccPedalMAP_Index = 3;
                        BstKnkCalMaxAirmass_Index = 4;
                        PedalMapCalTrq_RequestMap_Index = 5;
                        PedalMapCalTrq_RequestSportMap_Index = 6;
                        KnkDetCalfi_knkWinOffsMAP_Index = 7;
                        KnkDetCalfi_knkWinSizeMAP_Index = 8;
                        InjAnglCalMap_Index = 9;
                    }
                }
                else if (SequenceOf512Maps(sc, 2, 1, 1, 2, 1, 2))
                {
                    BstKnkCalMaxAirmass_Index = 1;
                    BstKnkCalMaxAirmassAu_Index = 2;
                    InjAnglCalMap_Index = 3;
                    KnkFuelCalfi_MaxOffsetMap_Index = 4;
                    KnkDetCalfi_knkWinOffsMAP_Index = 5;
                    KnkDetCalfi_knkWinSizeMAP_Index = 6;
                    PedalMapCalTrq_RequestMap_Index = 7;
                    TrqMastCalm_AirTorqMap_Index = 8;
                    TrqMastCalX_AccPedalMAP_Index = 9;
                }
                else
                {
                    BstKnkCalMaxAirmass_Index = 1;
                    TrqMastCalm_AirTorqMap_Index = 2;
                    TrqMastCalX_AccPedalMAP_Index = 3;
                    InjAnglCalMap_Index = 4;
                    KnkFuelCalfi_MaxOffsetMap_Index = 5;
                    PedalMapCalTrq_RequestMap_Index = 6;
                    PedalMapCalTrq_RequestSportMap_Index = 7;
                    KnkDetCalfi_knkWinOffsMAP_Index = 8;
                    KnkDetCalfi_knkWinSizeMAP_Index = 9;
                }

                // set the 512 map indexes correctly
                /*
2008 file: (1,2,1,2,2,1)

9*512 map

KnkFuelCal.fi_MaxOffsetMap	1039 	1
TrqMastCal.m_AirTorqMap		2344	2
TrqMastCal.X_AccPedalMAP	2345
BstKnkCal.MaxAirmass		3192	1
PedalMapCal.Trq_RequestMap	5820	2
PedalMapCal.Trq_RequestSportMap	5821
KnkDetCal.fi_knkWinOffsMAP	6749	2
KnkDetCal.fi_knkWinSizeMAP	6750
InjAnglCal.Map			6815	1

2007 file: (2,1,1,2,1,2)

9*512 map
BstKnkCal.MaxAirmass		316
BstKnkCal.MaxAirmassAu		317
InjAnglCal.Map			2489
KnkFuelCal.fi_MaxOffsetMap	2517
KnkDetCal.fi_knkWinOffsMAP	3311
KnkDetCal.fi_knkWinSizeMAP	3312
PedalMapCal.Trq_RequestMap	5962
TrqMastCal.m_AirTorqMap		6254
TrqMastCal.X_AccPedalMAP	6255

Non working file 2007: (1,2,1,1,2,2)

BstKnkCal.MaxAirmass		512		1
TrqMastCal.m_AirTorqMap		2186		2
TrqMastCal.X_AccPedalMAP	2187
InjAnglCal.Map			2981		1
KnkFuelCal.fi_MaxOffsetMap	4496		1
PedalMapCal.Trq_RequestMap	5771		2
PedalMapCal.Trq_RequestSportMap	5772	
KnkDetCal.fi_knkWinOffsMAP	6373		2
KnkDetCal.fi_knkWinSizeMAP	6374


                 * */

                MapIndex = SetMapName(sc, 70, 1, "FCutCal.FuelFactorMAP");
                SetMapNameByIndex(sc, MapIndex - 1, 10, "FCutCal.T_EngineSP");
                SetMapNameByIndex(sc, MapIndex - 2, 14, "FCutCal.nCombSinceFuelCutSP");
                SetMapNameByIndex(sc, MapIndex - 6, 2, "FCutCal.m_AirInletTime");
                SetMapNameByIndex(sc, MapIndex - 7, 2, "FCutCal.m_AirInletLimit");

                MapIndex = SetMapName(sc, 504, 1, "AfterStCal.StartMAP");
                SetMapNameByIndex(sc, MapIndex + 1, 28, "AfterStCal.n_CombXSP");
                SetMapNameByIndex(sc, MapIndex + 2, 36, "AfterStCal.T_EngineYSP");
                SetMapNameByIndex(sc, MapIndex + 3, 112, "AfterStCal.AmbientMAP");
                SetMapNameByIndex(sc, MapIndex + 4, 8, "AfterStCal.p_AirAmbientYSP");



                SetMapName(sc, 480, 1, "EngTipLimCal.X_Koeff");
                SetMapName(sc, 672, 1, "EngTipNormCal.Trq_BacklashTipOutMap");
                SetMapName(sc, 672, 2, "EngTipSportCal.Trq_BacklashTipOutMap");

                // very probable
                SetMapName(sc, 512, /*1*/ KnkFuelCalfi_MaxOffsetMap_Index, "KnkFuelCal.fi_MaxOffsetMap");
                MapIndex = SetMapName(sc, 512, /*2*/ TrqMastCalm_AirTorqMap_Index, "TrqMastCal.m_AirTorqMap");
                SetMapNameByIndex(sc, MapIndex -2, 576, "TrqMastCal.Trq_MBTMAP"); // maybe use search first match from current index
                SetMapNameByIndex(sc, MapIndex -1, 576, "TrqMastCal.Trq_NominalMap");
                SetMapNameByIndex(sc, MapIndex + 1, 512, "TrqMastCal.X_AccPedalMAP");
                SetMapNameByIndex(sc, MapIndex + 2, 72, "TrqMastCal.IgnAngleDiffSP");
                SetMapNameByIndex(sc, MapIndex + 3, 72, "TrqMastCal.TLO_TAB");
                SetMapNameByIndex(sc, MapIndex + 4, 32, "TrqMastCal.n_EngineYSP"); // TrqMastCal.n_EngineYSP/n_EngYSP
                SetMapNameByIndex(sc, MapIndex + 5, 32, "TrqMastCal.n_EngineXSP");
                SetMapNameByIndex(sc, MapIndex + 6, 8, "TrqMastCal.n_MaxDerXSP");
                SetMapNameByIndex(sc, MapIndex + 7, 32, "TrqMastCal.Trq_EngXSP");
                SetMapNameByIndex(sc, MapIndex + 8, 32, "TrqMastCal.Trq_PedYSP");
                SetMapNameByIndex(sc, MapIndex + 9, 32, "TrqMastCal.Trq_MaxDerIncMAP");
                SetMapNameByIndex(sc, MapIndex + 10, 32, "TrqMastCal.Trq_MaxDerDecMAP");
                SetMapNameByIndex(sc, MapIndex + 11, 2, "TrqMastCal.Trq_MaxDerShift");
                SetMapNameByIndex(sc, MapIndex + 12, 36, "TrqMastCal.m_AirXSP");

                SetMapName(sc, 512, /*3*/ TrqMastCalX_AccPedalMAP_Index, "TrqMastCal.X_AccPedalMAP");
                MapIndex = SetMapName(sc, 512, /*4*/ BstKnkCalMaxAirmass_Index, "BstKnkCal.MaxAirmass");
                SetMapNameByIndex(sc, MapIndex - 1, 32, "BstKnkCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 32, "BstKnkCal.OffsetXSP");
                if (BstKnkCalMaxAirmassAu_Index > 0)
                {
                    MapIndex = SetMapName(sc, 512, /*4*/ BstKnkCalMaxAirmassAu_Index, "BstKnkCal.MaxAirmassAu");
                }


                SetMapName(sc, 512, /*5*/ PedalMapCalTrq_RequestMap_Index, "PedalMapCal.Trq_RequestMap");
                if (PedalMapCalTrq_RequestSportMap_Index > 0)
                {
                    SetMapName(sc, 512, /*6*/ PedalMapCalTrq_RequestSportMap_Index, "PedalMapCal.Trq_RequestSportMap");
                }
                SetMapName(sc, 512, /*7*/ KnkDetCalfi_knkWinOffsMAP_Index, "KnkDetCal.fi_knkWinOffsMAP");
                SetMapName(sc, 512, /*8*/ KnkDetCalfi_knkWinSizeMAP_Index, "KnkDetCal.fi_knkWinSizeMAP");
                MapIndex = SetMapName(sc, 512, /*9*/ InjAnglCalMap_Index, "InjAnglCal.Map");
                SetMapNameByIndex(sc, MapIndex - 1, 32, "InjAnglCal.RpmYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 32, "InjAnglCal.AirXSP");

                MapIndex = SetMapName(sc, 384, 1, "AirMassMastCal.m_AirMBTMAP");
                SetMapNameByIndex(sc, MapIndex - 1, 24, "AirMassMastCal.Trq_MBTXSP");
                SetMapNameByIndex(sc, MapIndex - 2, 32, "AirMassMastCal.n_EngMBTYSP");

                MapIndex = SetMapName(sc, 384, 2, "IgnAbsCal.fi_IgnMBTMAP");
                SetMapNameByIndex(sc, MapIndex - 1, 32, "IgnAbsCal.n_EngMBTYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 24, "IgnAbsCal.m_AirMBTXSP");
                SetMapNameByIndex(sc, MapIndex + 1, 576, "IgnAbsCal.fi_highOctanMAP");
                SetMapNameByIndex(sc, MapIndex + 2, 576, "IgnAbsCal.fi_lowOctanMAP");
                SetMapNameByIndex(sc, MapIndex + 3, 576, "IgnAbsCal.fi_NormalMAP");
                SetMapNameByIndex(sc, MapIndex + 4, 36, "IgnAbsCal.m_AirNormXSP");
                SetMapNameByIndex(sc, MapIndex + 5, 32, "IgnAbsCal.n_EngNormYSP");
                


                MapIndex = SetMapName(sc, 320, 1, "IgnAbsCal.fi_FuelCutMAP");
                SetMapNameByIndex(sc, MapIndex - 1, 32, "IgnAbsCal.n_EngFuelCutYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 20, "IgnAbsCal.T_EngFuelCutXSP");
                SetMapNameByIndex(sc, MapIndex + 1, 20, "IgnAbsCal.T_EngStartXSP");
                SetMapNameByIndex(sc, MapIndex + 2, 16, "IgnAbsCal.n_EngStartYSP");
                SetMapNameByIndex(sc, MapIndex + 3, 160, "IgnAbsCal.fi_StartMAP");
                

                MapIndex = SetMapName(sc, 306, 1, "OilTempCal.T_SpeedAndLoadDecreaseMap");
                SetMapNameByIndex(sc, MapIndex - 1, 34, "OilTempCal.v_VehicleYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 18, "OilTempCal.T_AirInletXSP");

                //<GS-29082011>
                /*
                        4 maps length 288 (0x120) means biopower for T8.

                        1st = BFuelCal.LambdaOneFacMap
                        2nd = BFuelCal.TempEnrichFacMap
                        3rd = BFuelCal.E85TempEnrichFacMap
                        4th = MAFCal.NormAdjustFacMap
                 * */
                if (SymbolCountOfLength(sc, 288) == 4)
                {
                    // biopower
                    MapIndex = SetMapName(sc, 288, 1, "BFuelCal.LambdaOneFacMap");
                    SetMapNameByIndex(sc, MapIndex - 5, 32, "BFuelCal.RpmYSP");
                    SetMapNameByIndex(sc, MapIndex - 6, 36, "BFuelCal.AirXSP");
                    SetMapName(sc, 288, 2, "BFuelCal.E85TempEnrichFacMap");
                    SetMapName(sc, 288, 3, "BFuelCal.TempEnrichFacMap");
                    SetMapName(sc, 288, 4, "MAFCal.NormAdjustFacMap");
                }
                else
                {
                    if (!_reverse288Maps)
                    {
                        MapIndex = SetMapName(sc, 288, 1, "BFuelCal.LambdaOneFacMap");
                        SetMapNameByIndex(sc, MapIndex - 5, 32, "BFuelCal.RpmYSP");
                        SetMapNameByIndex(sc, MapIndex - 6, 36, "BFuelCal.AirXSP");
                        SetMapName(sc, 288, 2, "BFuelCal.TempEnrichFacMap");
                        SetMapName(sc, 288, 3, "MAFCal.NormAdjustFacMap");
                    }
                    else
                    {
                        MapIndex = SetMapName(sc, 288, 2, "BFuelCal.LambdaOneFacMap");
                        SetMapNameByIndex(sc, MapIndex - 5, 32, "BFuelCal.RpmYSP");
                        SetMapNameByIndex(sc, MapIndex - 6, 36, "BFuelCal.AirXSP");
                        SetMapName(sc, 288, 3, "BFuelCal.TempEnrichFacMap");
                        SetMapName(sc, 288, 1, "MAFCal.NormAdjustFacMap");
                    }
                }

                SetMapName(sc, 256, 1, "AirCtrlCal.RegMap");
                MapIndex = SetMapName(sc, 256, 2, "PedalMapCal.GainFactorMap");
                SetMapNameByIndex(sc, MapIndex - 1, 16, "PedalMapCal.GainFactorYSP");
                SetMapNameByIndex(sc, MapIndex - 3, 2, "PedalMapCal.T_SportModeEnable");
                SetMapNameByIndex(sc, MapIndex - 10, 2, "PedalMapCal.SportFactor");
                SetMapNameByIndex(sc, MapIndex - 14, 512, "PedalMapCal.Trq_RequestSportMap");
                SetMapNameByIndex(sc, MapIndex - 15, 512, "PedalMapCal.Trq_RequestMap");
                SetMapNameByIndex(sc, MapIndex - 16, 32, "PedalMapCal.X_PedalMap");
                SetMapNameByIndex(sc, MapIndex - 17, 32, "PedalMapCal.n_EngineMap");
                
                
                SetMapName(sc, 224, 1, "MAFCal.WeightConstMap");

                MapIndex = SetMapName(sc, 220, 1, "IgnTempCal.T_AirInletReferenceMap");
                SetMapNameByIndex(sc, MapIndex - 1, 22, "IgnTempCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 20, "IgnTempCal.m_AirXSP");


                SetMapName(sc, 220, 2, "IgnTempCal.fi_OffsetMaxAirInletMap");
                SetMapName(sc, 220, 3, "IgnTempCal.T_MaxAirInletMap");
                SetMapName(sc, 220, 4, "IgnTempCal.fi_OffsetMinAirInletMap");
                SetMapName(sc, 220, 5, "IgnTempCal.T_MinAirInletMap");
                SetMapName(sc, 220, 6, "IgnTempCal.fi_OffsetMaxTEngMap");
                SetMapName(sc, 220, 7, "IgnTempCal.fi_OffsetMinTEngMap");


                MapIndex = SetMapName(sc, 208, 1, "AfterStCal.HotSoakMAP");
                SetMapNameByIndex(sc, MapIndex + 2, 26, "AfterStCal.t_soakXSP");
                SetMapNameByIndex(sc, MapIndex + 4, 16, "AfterStCal.T_EngineYSP2");


                MapIndex = SetMapName(sc, 204, 1, "IgnMastCal.dwellTimeMap");
                SetMapNameByIndex(sc, MapIndex + 1, 34, "IgnMastCal.n_EngDwellTimeYSP");
                SetMapNameByIndex(sc, MapIndex + 2, 12, "IgnMastCal.UBattDwellTimeXSP");


                MapIndex = SetMapName(sc, 200, 1, "ExhaustCal.T_fiIgnMap");
                SetMapNameByIndex(sc, MapIndex - 1, 20, "ExhaustCal.fi_IgnSP");
                SetMapNameByIndex(sc, MapIndex - 4, 20, "ExhaustCal.m_AirSP");
                SetMapNameByIndex(sc, MapIndex + 1, 576, "ExhaustCal.T_Lambda1Map");
                SetMapNameByIndex(sc, MapIndex + 4, 576, "TrqMastCal.Trq_MBTMAP"); // maybe use search first match from current index
                SetMapNameByIndex(sc, MapIndex + 5, 576, "TrqMastCal.Trq_NominalMap");
                SetMapNameByIndex(sc, MapIndex + 6, 512, "TrqMastCal.m_AirTorqMap");
                SetMapNameByIndex(sc, MapIndex + 7, 512, "TrqMastCal.X_AccPedalMAP");
                SetMapNameByIndex(sc, MapIndex + 8, 72, "TrqMastCal.IgnAngleDiffSP");
                SetMapNameByIndex(sc, MapIndex + 9, 72, "TrqMastCal.TLO_TAB");
                SetMapNameByIndex(sc, MapIndex + 10, 32, "TrqMastCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex + 11, 32, "TrqMastCal.n_EngXSP");
                SetMapNameByIndex(sc, MapIndex + 12, 8, "TrqMastCal.n_MaxDerXSP");
                SetMapNameByIndex(sc, MapIndex + 13, 32, "TrqMastCal.Trq_EngXSP");
                SetMapNameByIndex(sc, MapIndex + 14, 32, "TrqMastCal.Trq_PedYSP");
                SetMapNameByIndex(sc, MapIndex + 15, 32, "TrqMastCal.Trq_MaxDerIncMAP");
                SetMapNameByIndex(sc, MapIndex + 16, 32, "TrqMastCal.Trq_MaxDerDecMAP");
                SetMapNameByIndex(sc, MapIndex + 17, 2, "TrqMastCal.Trq_MaxDerShift");
                SetMapNameByIndex(sc, MapIndex + 18, 36, "TrqMastCal.m_AirXSP");
                



                MapIndex = SetMapName(sc, 192, 1, "AirCtrlCal.m_AirBoostHighAltOffsMAP");
                SetMapNameByIndex(sc, MapIndex - 1, 24, "AirCtrlCal.p_AirAmbientYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 16, "AirCtrlCal.p_AfterTurbineXSP");
                MapIndex = SetMapName(sc, 192, 2, "IgnMastCal.MinMap");
                SetMapNameByIndex(sc, MapIndex + 1, 24, "IgnMastCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex + 2, 16, "IgnMastCal.m_AirXSP");

                MapIndex = SetMapName(sc, 168, 1, "IgnKnkCal.RetardIndexMap");
                SetMapNameByIndex(sc, MapIndex - 1, 24, "IgnKnkCal.n_EngIndexYSP");
                SetMapNameByIndex(sc, MapIndex - 2, 14, "IgnKnkCal.m_AirIndexXSP");
                SetMapNameByIndex(sc, MapIndex - 6, 32, "IgnKnkCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex - 7, 36, "IgnKnkCal.m_AirXSP");

                MapIndex = SetMapName(sc, 168, 2, "IgnKnkCal.ARetardIndexMap");
                SetMapNameByIndex(sc, MapIndex + 1, 576, "IgnKnkCal.IndexMap");

                MapIndex = SetMapName(sc, 168, 3, "KnkDetCal.X_hystOffsetMAP");
                MapIndex = SetMapName(sc, 168, 4, "KnkDetCal.X_AHystOffsetMAP");
                

                MapIndex = SetMapName(sc, 160, 1, "MisfCal.m_LoadLevelMAT");
                SetMapNameByIndex(sc, MapIndex + 1, 36, "MisfCal.m_AirXSP");
                SetMapNameByIndex(sc, MapIndex + 4, 576, "MisfCal.CatOverheatFactorMAT");
                SetMapNameByIndex(sc, MapIndex + 7, 32, "MisfCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex + 10, 10, "MisfCal.T_EngXSP");
                if (_reverse288Maps)
                {
                    MapIndex = SetMapName(sc, 576, 12, "KnkFuelCal.EnrichmentMap", 1, 576);
                    SetMapNameByIndex(sc, MapIndex + 1, 576, "KnkFuelCal.fi_OffsetEnrichEnable");
                    SetMapNameByIndex(sc, MapIndex + 2, 576, "KnkFuelCal.fi_MaxOffsetMap");
                    SetMapNameByIndex(sc, MapIndex + 3, 32, "KnkFuelCal.m_AirXSP");

                }
                else
                {
                    MapIndex = SetMapName(sc, 576, 2, "KnkFuelCal.EnrichmentMap", 1, 576);
                    SetMapNameByIndex(sc, MapIndex + 1, 576, "KnkFuelCal.fi_OffsetEnrichEnable");
                    SetMapNameByIndex(sc, MapIndex + 2, 576, "KnkFuelCal.fi_MaxOffsetMap");
                    SetMapNameByIndex(sc, MapIndex + 3, 32, "KnkFuelCal.m_AirXSP");

                }


                SetMapName(sc, 160, 2, "IgnAbsCal.fi_StartMAP");
                SetMapName(sc, 140, 1, "FuelCutInhibitCal.FCIFaultCodeList");
                SetMapName(sc, 130, 1, "VIOSMAFCal.TicksSP");
                SetMapName(sc, 130, 2, "VIOSMAFCal.Q_AirInletTab");
                if (_reverse288Maps)
                {
                    SetMapName(sc, 98, 1, "AirCtrlCal.Ppart_BoostMap");
                    SetMapName(sc, 98, 2, "AirCtrlCal.Ipart_BoostMap");
                    MapIndex = SetMapName(sc, 98, 3, "AirCtrlCal.Dpart_BoostMap");
                }
                else
                {
                    SetMapName(sc, 98, 3, "AirCtrlCal.Ppart_BoostMap");
                    SetMapName(sc, 98, 4, "AirCtrlCal.Ipart_BoostMap");
                    MapIndex = SetMapName(sc, 98, 5, "AirCtrlCal.Dpart_BoostMap");

                }
                SetMapNameByIndex(sc, MapIndex + 1, 14, "AirCtrlCal.PIDXSP");
                SetMapNameByIndex(sc, MapIndex + 2, 14, "AirCtrlCal.PIDYSP");
                SetMapNameByIndex(sc, MapIndex + 3, 2, "AirCtrlCal.IPart_BoostCoAirM1");
                SetMapNameByIndex(sc, MapIndex + 4, 2, "AirCtrlCal.m_IFacMax");
                SetMapNameByIndex(sc, MapIndex + 5, 2, "AirCtrlCal.FilterFactor");
                SetMapNameByIndex(sc, MapIndex + 6, 32, "AirCtrlCal.n_EngYSP");
                SetMapNameByIndex(sc, MapIndex + 7, 2, "AirCtrlCal.Hysteres");
                SetMapNameByIndex(sc, MapIndex + 8, 16, "AirCtrlCal.SetLoadXSP");
                SetMapNameByIndex(sc, MapIndex + 9, 256, "AirCtrlCal.RegMap");

                // find torquelimiters automatically
                int state = 0;
                int symIndex =0;
                int symCounter = 0;
                bool limitersFound = false;
                foreach (SymbolHelper sh in sc)
                {
                    //if (state > 0) LogHelper.Log("State = " + state.ToString() + " symcount: " + symCounter.ToString());
                    switch (state)
                    {
                        case 0:
                            if (sh.Length == 16) state++;
                            break;
                        case 1:
                            
                            if (sh.Length == 2) state++;
                            else state = 0;
                            break;
                        case 2:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 3:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 4:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 5:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 6:
                            if (sh.Length == 2) state++;
                            else state = 0;
                            break;
                        case 7:
                            if (sh.Length == 2) state++;
                            else state = 0;
                            break;
                        case 8:
                            if (sh.Length == 2) state++;
                            else state = 0;
                            break;
                        case 9:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 10:
                            if (sh.Length == 32) state++;
                            else state = 0;
                            break;
                        case 11:
                            symIndex = sh.Symbol_number;
                            break;
                    }
                    symCounter++;
                    if (symIndex > 0) break;
                }
                if (symIndex > 0)
                {
                    LogHelper.Log("(I) Found index: " + symIndex.ToString());
                    limitersFound = true;
                    // - 11 = TrqLimCal.Trq_ManGear
                    SetMapNameByIndex(sc, symIndex - 11, 16, "TrqLimCal.Trq_ManGear");
                    SetMapNameByIndex(sc, symIndex - 9, 32, "TrqLimCal.Trq_MaxEngineManTab1");
                    SetMapNameByIndex(sc, symIndex - 8, 32, "TrqLimCal.Trq_MaxEngineAutTab1");
                    SetMapNameByIndex(sc, symIndex - 7, 32, "TrqLimCal.Trq_MaxEngineManTab2");
                    SetMapNameByIndex(sc, symIndex - 6, 32, "TrqLimCal.Trq_MaxEngineAutTab2");
                    SetMapNameByIndex(sc, symIndex - 2, 32, "TrqLimCal.n_EngYSP");
                    SetMapNameByIndex(sc, symIndex - 1, 32, "TrqLimCal.Trq_OverBoostTab");

                    SetMapNameByIndex(sc, symIndex + 7, 24, "TrqLimCal.CompressorNoiseYSP");
                    SetMapNameByIndex(sc, symIndex + 8, 6, "TrqLimCal.CompressorNoiseXSP");
                    SetMapNameByIndex(sc, symIndex + 9, 72, "TrqLimCal.Trq_CompressorNoiseRedLimMAP");

                }

                if (!limitersFound)
                {
                    state = 0;
                    symIndex = 0;
                    symCounter = 0;
                    foreach (SymbolHelper sh in sc)
                    {
                        //if (state > 0) LogHelper.Log("State = " + state.ToString() + " symcount: " + symCounter.ToString());
                        switch (state)
                        {
                            case 0:
                                if (sh.Length == 16) state++;
                                break;
                            case 1:

                                if (sh.Length == 2) state++;
                                else state = 0;
                                break;
                            case 2:
                                if (sh.Length == 32) state++;
                                else state = 0;
                                break;
                            case 3:
                                if (sh.Length == 32) state++;
                                else state = 0;
                                break;
                            case 4:
                                if (sh.Length == 2) state++;
                                else state = 0;
                                break;
                            case 5:
                                if (sh.Length == 2) state++;
                                else state = 0;
                                break;
                            case 6:
                                if (sh.Length == 2) state++;
                                else state = 0;
                                break;
                            case 7:
                                if (sh.Length == 32) state++;
                                else state = 0;
                                break;
                            case 8:
                                if (sh.Length == 32) state++;
                                else state = 0;
                                break;
                            case 9:
                                symIndex = sh.Symbol_number;
                                state++;
                                break;
                            case 10:
                                break;
                        }
                        symCounter++;
                        if (symIndex > 0) break;
                    }
                }
                if (symIndex > 0)
                {
                    LogHelper.Log("(II) Found index: " + symIndex.ToString());
                    limitersFound = true;
                    // - 9 = TrqLimCal.Trq_ManGear
                    SetMapNameByIndex(sc, symIndex - 9, 16, "TrqLimCal.Trq_ManGear");
                    SetMapNameByIndex(sc, symIndex - 7, 32, "TrqLimCal.Trq_MaxEngineManTab1");
                    SetMapNameByIndex(sc, symIndex - 6, 32, "TrqLimCal.Trq_MaxEngineAutTab1");
                    SetMapNameByIndex(sc, symIndex - 2, 32, "TrqLimCal.n_EngYSP");
                    SetMapNameByIndex(sc, symIndex - 1, 32, "TrqLimCal.Trq_OverBoostTab");
                    SetMapNameByIndex(sc, symIndex + 7, 24, "TrqLimCal.CompressorNoiseYSP");
                    SetMapNameByIndex(sc, symIndex + 8, 6, "TrqLimCal.CompressorNoiseXSP");
                    SetMapNameByIndex(sc, symIndex + 9, 72, "TrqLimCal.Trq_CompressorNoiseRedLimMAP");
                }
                SymbolTranslator st = new SymbolTranslator();
                foreach (SymbolHelper sh in sc)
                {
                    string helptext = string.Empty;
                    XDFCategories cat = XDFCategories.Undocumented;
                    XDFSubCategory sub = XDFSubCategory.Undocumented;
                    sh.Description = st.TranslateSymbolToHelpText(sh.Userdescription, out helptext, out cat, out sub);

                    if (sh.Category == "Undocumented" || sh.Category == "")
                    {
                        sh.createAndUpdateCategory(sh.Userdescription);
                    }
                }
            }
            return retval;
        }

        private bool SequenceOf512Maps(SymbolCollection sc, int seq1, int seq2, int seq3, int seq4, int seq5, int seq6)
        {
            bool retval = true;
            // get the sequence of 512 lengths maps
            sc.SortColumn = "Symbol_number";
            sc.SortingOrder = GenericComparer.SortOrder.Ascending;
            sc.Sort();
            int idx = 0;
            int[] _seq = new int[9];
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Length == 512)
                {
                    _seq[idx] = sh.Symbol_number;
                    idx++;
                }
            }
            // example:
            /*
KnkFuelCal.fi_MaxOffsetMap	1039 	1
TrqMastCal.m_AirTorqMap		2344	2
TrqMastCal.X_AccPedalMAP	2345
BstKnkCal.MaxAirmass		3192	1
PedalMapCal.Trq_RequestMap	5820	2
PedalMapCal.Trq_RequestSportMap	5821
KnkDetCal.fi_knkWinOffsMAP	6749	2
KnkDetCal.fi_knkWinSizeMAP	6750
InjAnglCal.Map			6815	1             * */
            int[] rseq = new int[6];
            for (int i = 0; i < 6; i++) rseq[i] = 1;
            idx = 0;
            int idx2 = 0;
            for (idx = 0; idx < 9; idx++)
            {
                if (idx < _seq.Length - 1)
                {
                    if (Math.Abs(_seq[idx] - _seq[idx + 1]) == 1)
                    {
                        rseq[idx2++] = 2;
                        idx++;
                    }
                    else
                    {
                        rseq[idx2++] = 1;
                    }
                }
            }
            if (rseq[0] != seq1) retval = false;
            if (rseq[1] != seq2) retval = false;
            if (rseq[2] != seq3) retval = false;
            if (rseq[3] != seq4) retval = false;
            if (rseq[4] != seq5) retval = false;
            if (rseq[5] != seq6) retval = false;
            return retval;
            
        }

        private bool SequenceOf576Maps(SymbolCollection sc, int seq1, int seq2, int seq3, int seq4, int seq5, int seq6)
        {
            bool retval = true;
            // get the sequence of 576 lengths maps
            sc.SortColumn = "Symbol_number";
            sc.SortingOrder = GenericComparer.SortOrder.Ascending;
            sc.Sort();
            int idx = 0;
            int[] _seq = new int[32];
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Length == 576)
                {
                    _seq[idx] = sh.Symbol_number;
                    idx++;
                }
            }
            int[] rseq = new int[32];
            for (int i = 0; i < 32; i++) rseq[i] = 1;
            idx = 0;
            int idx2 = 0;
            for (idx = 0; idx < 9; idx++)
            {
                if (idx < _seq.Length - 1)
                {
                    if (Math.Abs(_seq[idx] - _seq[idx + 1]) == 1)
                    {
                        rseq[idx2++] = 2;
                        idx++;
                    }
                    else
                    {
                        rseq[idx2++] = 1;
                    }
                }
            }
            if (rseq[0] != seq1) retval = false;
            if (rseq[1] != seq2) retval = false;
            if (rseq[2] != seq3) retval = false;
            if (rseq[3] != seq4) retval = false;
            if (rseq[4] != seq5) retval = false;
            if (rseq[5] != seq6) retval = false;
            return retval;

        }

        private void SetMapNameByIndex(SymbolCollection sc, int symbolnumber, int symbollength, string mapname)
        {
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Symbol_number == symbolnumber && sh.Flash_start_address < 0x100000)
                {
                    if (sh.Length == symbollength)
                    {
                        if (sh.Userdescription == string.Empty)
                        {
                            sh.Userdescription = mapname;
                            break;
                        }
                    }
                }
            }
        }

        private int SetMapName(SymbolCollection sc, int symbollength, int sequence, string mapname)
        {
            int seq = 0;
            int retval = 0;
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Length == symbollength && sh.Flash_start_address < 0x100000)
                {
                    seq++;
                    if (seq == sequence)
                    {
                        if (sh.Userdescription == string.Empty)
                        {
                            sh.Userdescription = mapname;
                            retval = sh.Symbol_number;
                            break;
                        }
                    }
                }
            }
            return retval;
        }

        private int SymbolCountOfLength(SymbolCollection sc, int symbollength)
        {
            int retval = 0;
            
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Length == symbollength) retval++;
            }
            return retval;
        }

        private int SetMapName(SymbolCollection sc, int symbollength, int sequence, string mapname, int MapIndexToCheck, int MapLengthToCheck)
        {
            int seq = 0;
            int retval = 0;
            int symcount = 0;
            foreach (SymbolHelper sh in sc)
            {
                if (sh.Length == symbollength && sh.Flash_start_address < 0x100000)
                {
                    seq++;
                    if (seq == sequence)
                    {
                        if (sc[symcount + MapIndexToCheck].Length == MapLengthToCheck)
                        {
                            if (sh.Userdescription == string.Empty)
                            {
                                sh.Userdescription = mapname;
                                retval = sh.Symbol_number;
                                break;
                            }
                        }
                    }
                }
                symcount++;
            }
            return retval;
        }
    }
}
