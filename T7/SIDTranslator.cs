using System;
using System.Collections.Generic;
using System.Text;

namespace T7
{

    class SIDTranslator
    {
        /*
Rpm In.n_Engine  Engine speed UNIT : rpm MAX : 8000 MIN : 25 (set to 10 when engine starts to move) TRANS : V = P. Resolution is 1. Interval is Every combustion / 5 ms when engine is still.  
Lamb  Lambda.LambdaInt  Global closed loop integrator. Update : every combustion. V6: Bank 1. Resolution is ° 0.01 %.  
Igna  Out.fi_Ignition  Actual ignition angle. A positive value is before TDC and a negative value is after TDC. Resolution is 0.1 °. Interval is Every combustion.  
Teng  In.T_Engine  Engine coolant temperature UNIT : (C MAX : 150 MIN : -40 TRANS : V = P. Resolution is 1. Interval is 1000 ms.  
STAd  E85Adap.ST_Adap   
Tair  In.T_AirInlet  Inlet air temperature UNIT : (C MAX : 140 MIN : -40 TRANS : V = P. Resolution is 1. Interval is 1000 ms.  
Ioff  IgnProt.fi_Offset  Shows ignition angle output from offset functions. Resolution is 0.1 °.  
Meng  Out.M_Engine  Engine torque UNIT : Nm MAX : 400 MIN : -100 TRANS : V = (P+. Resolution is 1. Interval is 10ms.  
Mlow  TorqueProt.M_LowLim  By the Torque Master selected lowest torque limit request, corrected with adaption value made at idle.  
nErr  obdNoOfFaults  Number of errors stored  
Pbef  In.p_AirBefThrottle  Engine inlet air pressure UNIT : kPa MAX : 300 MIN : 0 TRANS : V = P * 10. Resolution is 0.1. Interval is Every combustion.  
Pinl  In.p_AirInlet  Engine inlet air pressure UNIT : kPa MAX : 300 MIN : 0 TRANS : V = P * 10. Resolution is 0.1. Interval is Every combustion.  
Pair  In.p_AirAmbient  Barometric air pressure UNIT : kPa MAX : 120 MIN : 50 TRANS : V = P * 10. Resolution is 0.1. Interval is 250 ms.  
mReq  m_Request  Requested airmass  
mAIR  MAF.m_AirInlet  Airmass in milligram per combustion. This airmass is the actual load value in the ECM. (Unfiltered) Calculated from ActualIn.Q_AirInlet. Resolution is 1 mg/c. Interval is every combustion.  
Miss  Missf.nrOfCountedMisfire  Counts the nr of misfire that has not been filtered or rpm diff filtered.  
Pfac  BoostProt.PFac  Calculate P part for regulator. load diff * P const P = 100. Update : every 10 msec. Resolution is 0.1 %.  
Ifac  BoostProt.IFac  Calculated I part for regulator. load diff * I const I = I + 1000. Update : every 10 msec. Resolution is 0.1 %.  
PWM  Out.PWM_BoostCntrl  Duty-cycle for boost pressure valve. Resolution is 0.1 %. Interval is every 20 ms.  
tSta  ECMStat.t_StartTime  Engine start time, measured by measuring the time from that the battery volatage decreases 1.0V to the time engine speed reached 1000 rpm.  
LIMP  OBDAdap.ThrLimpHomeNr  Last reported throttle limphome number.  
Mode  SID.ST_Mode  Mode settings to see different "values"  
Me85  In.X_EthanolSensor   
Ad85  E85.X_EthanolActual   
Ca85  E85Prot.X_EthanolActual   
Amul  AdpFuelProt.MulFuelAdapt  Multicative fueladaption value. Resolution is ° 0.01 %.  
FFac  Purge.FuelFac  The fuelfactor from the purge function. Resolution is 0.01 %.  
ReFu  E85Adap.ST_ReFuel   
Crnk  CrnkCas.ST_Fuel   
MxLo  LambdaProt.MaxLoadNorm  Max load (airmass) for closed loop during normal conditions. Update : every combustion. Resolution is 1 mg/c.  
SFuL  E85Adap.V_SavedFuelLevel   
VFue  In.V_FuelTank  Fuel level UNIT : l (litre) MAX : 100 MIN : 0 TRANS : V = P * 10. Resolution is 0.1. Interval is 1000 ms.  
Aadd  AdpFuelProt.AddFuelAdapt+2  Additative fueladaption value. Resolution is 0.01 mg/c.  
Aadp  AreaAdap.A_Throttle  Adaption of throttle area. Interval is 250ms.  
AdpD  IdleAdap.Q_AirDrive  Adaption value for idlespeed regulation (drive activated). This value is added to the PID and Constant part of the regulator. If the I-part is limited will the adaption stop. Resolution is 0.01 g/s.  
AdpN  IdleAdap.Q_AirNeutral  Adaption value for idlespeed regulation (drive not activated). This value is added to the PID and Constant part of the regulator. If the I-part is limited will the adaption stop. Resolution is 0.01 g/s.  
Akw1  KnkAdaptAdap.RefValueWind   
Akw2  KnkAdaptAdap.RefValueWind+2   
AMR  CanIn.ST_EngineInterv  Engine intervention is requested from ESP (AMR)  
Apur  Purge.HCCont  The content of HC in the purge air. Resolution is 0.1 %. 
ay CanIn.a_Lateral Lateral acceleration, only implemented on cars with ESP. Resolution is 0.5 m/s2. 
 
Badp  BoostAdap.Adaption  Adaption value for boost control. Interval is Every 100ms.  
BMR  CanIn.ST_BrakeInterv  Brake intervention is requested from ESP (BMR)  
CLUi  Out.CMD_CoastLUInhibit  Inhibit coast slip lock up  
Cmem  EngTip.ST_Active  Status flag showing if tipin is active 0 = Not active 1 = Tip-in active 2 = Tip-out active  
CSLU  In.ST_TCMCSLU  Coast Lock up slip state 0 = No request 1 = Fuel cut inhibit 2 = Fuel cut allowed  
DTI  Out.M_DTI  Drivers Torque Intention. The torque that the driver requests converted from air to torque. Limitations from all functions excluding TCM and TCS are included in the signal UNIT : Nm MAX : 400 MIN : -100  
Fcod  obdFaults  codes for errors stored  
FFAd  Purge.m_FuelPrg  Fuel flow from purge. Resolution is 0.01 mg/c.  
Flow  Purge.Flow  The actual purge flow. Resolution is 1 mg/s.  
FMXF  PurgeProt.FuelFacMaxFlow  Maximum allowed purge flow in respect to maximum allowed fuel factor at actual load. Resolution is 1 mg/s.  
Frez  PurgeProt.AdpFreeze  Adaption freeze status.  
Fuel  BFuelProt.CurrentFuelCons   
Gear  In.X_ActualGear  Actual gear on automatic gearbox. 2 - Reverse 3 - Neutral 5 - Gear 1 6 - Gear 2 7 - Gear 3 8 - Gear 4 11 - Gear 3, lock up 12 - Gear 4, lock up Interval is every 50 ms.  
GSI  Out.CMD_GearShiftInhibit  Prevent TCM from shifting.  
HCnt  Purge.HCCont  The content of HC in the purge air. Resolution is 0.1 %.  
In.X  In.X_AccPedal  Pedal position UNIT : % MAX : 130 MIN : 0 TRANS : V = P * 10. Resolution is 0.1. Interval is 20 ms.  
Iput  ActualIn.n_GearBoxIn  Transmission input rpm (turbine speed) Used to detect when the load is changed for the engine when gear is engaged. Resolution is 1 rpm. Interval is every 50 ms.  
JeLi  JerkProt.JerkFactor  Threshold value for changing shift pattern to "no lockup"  
Jerk  ECMStat.JerkFactor  This factor describes the jerking of the engine. The formula for calculating this is abs(ECMStat.n_EngineDelta2) * factor. The factor is for scaling it so it will be possible to filter it. The calibratable value used for filtering is nEngCal.FilterFactor. Since the jerk factor is based on every combustion, it is not possible to compare the numbers for 6 cylinder engines and 4 cylinder.  
Kph1  ActualIn.v_Vehicle  Left front wheel speed UNIT : km/h MAX : 300 MIN : 0 (detection of min. 1.0 km/h) TRANS : V = P * 10. Resolution is 0.1. Interval is 100 ms.  
Kph2  ActualIn.v_Vehicle2  Vehicle speed, measured on the rear wheel, sent from MIU. UNIT : km/h MAX : 300 MIN : 0 TRANS : V = P * 10. Resolution is 0.1. Interval is 100 ms. 
LwsI CanIn.fi_SteeringAngle Stearing angle (LwsIn), only implemented on cars with ESP . Resolution is 3 °. 
 
mAir  MAF.m_AirInlet  Airmass in milligram per combustion. This airmass is the actual load value in the ECM. (Unfiltered) Calculated from ActualIn.Q_AirInlet. Resolution is 1 mg/c. Interval is every combustion.  
Mair  In.M_TCSTorqueReq  Maximum torque request from TCS system via CAN. Resolution is 1 Nm. Interval is Every 20 ms.  
MiFi  Missf.nrOfFilteredMisfire  Number of missfires occurred  
Mnom  Torque.M_Nominal  Nominal engine output torque at a certain enginespeed and inlet airmass. Read from matrix.  
MTCM  ActualIn.M_TCMLimitReq  Maximum engine torque request from TCM UNIT : Nm MAX : 400 MIN : -100 TRANS : V = P. Resolution is 1. Interval is 10 ms.  
Mtot  In.M_TCSTotalReq '  Total torque request from ESP equiped cars. The differance in torque between In.M_TCSTorqueReq and In.M_TCSTotalReq is taken with ignition retardation. Resolution is 1 Nm. Interval is Every 20 ms.  
NoIg  Out.ST_NoIgnitionRetard  Ignition retardation is not allowed due to overheating the catalytic converter 
Oput DiffPSProt.v_GearBoxOut TCM gearbox output speed converted to vehicle speed. Resolution is 0.1 km/h. Interval is Every 100ms. 
 
Pdif  ECMStat.p_Diff  Differance between inlet manifold air pressure and external air pressure. Resolution is 0.1 kPa.  
Peng  ECMStat.P_Engine  Calculated engine power. Measured in horsepower.  
Perc  PurgeProt.PurgePercent  Purge flow/Air mass flow ratio. Resolution is 0.01 %.  
PMXF  PurgeProt.PdiffMaxFlow  Maximum flow allowed by the diff. pressure. Resolution is 1 mg/s.  
Ppwm  Purge.Valve  Purge valve PWM. Resolution is 0.1 %.  
PrSt  Purge.Status  Status of the purge function.  
         * */
        public void GetSidDescription(SIDIHelper sidhelper)
        {
            /*SIDIHelper sidhelper = new SIDIHelper();
            sidhelper.AddressSRAM = SIDSymbol.AddressSRAM;
            sidhelper.Info = SIDSymbol.Info;
            sidhelper.Mode = SIDSymbol.Mode;
            sidhelper.Symbol = SIDSymbol.Symbol;
            sidhelper.T7Symbol = SIDSymbol.T7Symbol;
            sidhelper.Value = SIDSymbol.Value;*/
            switch(sidhelper.Symbol)
            {
                case "Rpm":
                case "Rpm ":
                    sidhelper.Info = "Engine speed";
                    sidhelper.T7Symbol = "In.n_Engine";
                    break;
                case "Lamb":
                    sidhelper.Info = "Global closed loop integrator";
                    sidhelper.T7Symbol = "Lambda.LambdaInt";
                    break;
                case "Igna":
                    sidhelper.T7Symbol = "Out.fi_Ignition";
                    sidhelper.Info = "Actual ignition angle";
                    break;
                case "Teng":
                    sidhelper.T7Symbol = "In.T_Engine";
                    sidhelper.Info = "Engine coolant temperature";
                    break;
                case "STAd":
                    sidhelper.T7Symbol = "E85Adap.ST_Adap";
                    sidhelper.Info = "E85Adap.ST_Adap";
                    break;
                case "Tair":
                    sidhelper.Info = "Inlet air temperature";
                    sidhelper.T7Symbol = "In.T_AirInlet";
                    break;
                case "Ioff":
                    sidhelper.Info = "Shows ignition angle output from offset functions";
                    sidhelper.T7Symbol = "IgnProt.fi_Offset";
                    break;
                case "Meng":
                    sidhelper.Info = "Engine torque";
                    sidhelper.T7Symbol = "Out.M_Engine";
                    break;
                case "Mlow":
                    sidhelper.Info = "By the Torque Master selected lowest torque limit request, corrected with adaption value made at idle";
                    sidhelper.T7Symbol = "TorqueProt.M_LowLim";
                    break;
                case "nErr":
                    sidhelper.Info = "Number of errors stored";
                    sidhelper.T7Symbol = "obdNoOfFaults";
                    break;
                case "Pbef":
                    sidhelper.Info = "Engine inlet air pressure";
                    sidhelper.T7Symbol = "In.p_AirBefThrottle";
                    break;
                case "Pinl":
                    sidhelper.Info = "Engine inlet air pressure";
                    sidhelper.T7Symbol = "In.p_AirInlet";
                    break;
                case "Pair":
                    sidhelper.Info = "Barometric air pressure";
                    sidhelper.T7Symbol = "In.p_AirAmbient";
                    break;
                case "mReq":
                    sidhelper.Info = "Requested airmass";
                    sidhelper.T7Symbol = "m_Request";
                    break;
                case "mAIR":
                    sidhelper.Info = "Airmass in milligram per combustion";
                    sidhelper.T7Symbol = "MAF.m_AirInlet";
                    break;
                case "Miss":
                    sidhelper.Info = "Counts the nr of misfire that has not been filtered or rpm diff filtered";
                    sidhelper.T7Symbol = "Missf.nrOfCountedMisfire";
                    break;
                case "Pfac":
                    sidhelper.Info = "Calculated P part for regulator";
                    sidhelper.T7Symbol = "BoostProt.PFac";
                    break;
                case "Ifac":
                    sidhelper.Info = "Calculated I part for regulator";
                    sidhelper.T7Symbol = "BoostProt.IFac";
                    break;
                case "PWM":
                case "PWM ":
                    sidhelper.Info = "Duty-cycle for boost pressure valve";
                    sidhelper.T7Symbol = "Out.PWM_BoostCntrl";
                    break;
                case "tSta":
                    sidhelper.Info = "Engine start time, measured by measuring the time from that the battery volatage decreases 1.0V to the time engine speed reached 1000 rpm";
                    sidhelper.T7Symbol = "ECMStat.t_StartTime";
                    break;
                case "LIMP":
                    sidhelper.Info = "Last reported throttle limphome number";
                    sidhelper.T7Symbol = "OBDAdap.ThrLimpHomeNr";
                    break;
                case "Mode":
                    sidhelper.Info = "Mode settings to see different values";
                    sidhelper.T7Symbol = "SID.ST_Mode";
                    break;
                case "Me85":
                    sidhelper.Info = "In.X_EthanolSensor";
                    sidhelper.T7Symbol = "In.X_EthanolSensor";
                    break;
                case "Ad85":
                    sidhelper.Info = "E85.X_EthanolActual";
                    sidhelper.T7Symbol = "E85.X_EthanolActual";
                    break;
                case "Ca85":
                    sidhelper.Info = "E85Prot.X_EthanolActual";
                    sidhelper.T7Symbol = "E85Prot.X_EthanolActual";
                    break;
                case "Amul":
                    sidhelper.Info = "Multicative fueladaption value";
                    sidhelper.T7Symbol = "AdpFuelProt.MulFuelAdapt";
                    break;
                case "FFac":
                    sidhelper.Info = "The fuelfactor from the purge function";
                    sidhelper.T7Symbol = "Purge.FuelFac";
                    break;
                case "ReFu":
                    sidhelper.Info = "E85Adap.ST_ReFuel";
                    sidhelper.T7Symbol = "E85Adap.ST_ReFuel";
                    break;
                case "Crnk":
                    sidhelper.Info = "CrnkCas.ST_Fuel";
                    sidhelper.T7Symbol = "CrnkCas.ST_Fuel";
                    break;
                case "MxLo":
                    sidhelper.Info = "Max load (airmass) for closed loop during normal conditions";
                    sidhelper.T7Symbol = "LambdaProt.MaxLoadNorm";
                    break;
                case "SFuL":
                    sidhelper.Info = "E85Adap.V_SavedFuelLevel";
                    sidhelper.T7Symbol = "E85Adap.V_SavedFuelLevel";
                    break;
                case "VFue":
                    sidhelper.Info = "Fuel level";
                    sidhelper.T7Symbol = "In.V_FuelTank";
                    break;
                case "Aadd":
                    sidhelper.Info = "Additative fueladaption value";
                    sidhelper.T7Symbol = "AdpFuelProt.AddFuelAdapt+2";
                    break;
                case "Aadp":
                    sidhelper.Info = "Adaption of throttle area";
                    sidhelper.T7Symbol = "AreaAdap.A_Throttle";
                    break;
                case "AdpD":
                    sidhelper.Info = "Adaption value for idlespeed regulation (drive activated)";
                    sidhelper.T7Symbol = "IdleAdap.Q_AirDrive";
                    break;
                case "AdpN":
                    sidhelper.Info = "Adaption value for idlespeed regulation (drive not activated)";
                    sidhelper.T7Symbol = "IdleAdap.Q_AirNeutral";
                    break;
                case "Akw1":
                    sidhelper.Info = "KnkAdaptAdap.RefValueWind";
                    sidhelper.T7Symbol = "KnkAdaptAdap.RefValueWind";
                    break;
                case "Akw2":
                    sidhelper.Info = "KnkAdaptAdap.RefValueWind+2";
                    sidhelper.T7Symbol = "KnkAdaptAdap.RefValueWind+2";
                    break;
                case "AMR":
                case "AMR ":
                    sidhelper.Info = "Engine intervention is requested from ESP (Airmass reduction)";
                    sidhelper.T7Symbol = "CanIn.ST_EngineInterv";
                    break;
                case "Apur":
                    sidhelper.Info = "The content of HC in the purge air";
                    sidhelper.T7Symbol = "Purge.HCCont";
                    break;
                case "ay":
                case "ay  ":
                    sidhelper.Info = "Lateral acceleration, only implemented on cars with ESP";
                    sidhelper.T7Symbol = "CanIn.a_Lateral";
                    break;
                case "Badp":
                    sidhelper.Info = "Adaption value for boost control";
                    sidhelper.T7Symbol = "BoostAdap.Adaption";
                    break;
                case "BMR":
                case "BMR ":
                    sidhelper.Info = "Brake intervention is requested from ESP (BMR)";
                    sidhelper.T7Symbol = "CanIn.ST_BrakeInterv";
                    break;
                case "CLUi":
                    sidhelper.Info = "Inhibit coast slip lock up";
                    sidhelper.T7Symbol = "Out.CMD_CoastLUInhibit";
                    break;
                case "Cmem":
                    sidhelper.Info = "Status flag showing if tipin is active 0 = Not active 1 = Tip-in active 2 = Tip-out active";
                    sidhelper.T7Symbol = "EngTip.ST_Active";
                    break;
                case "CSLU":
                    sidhelper.Info = "Coast Lock up slip state 0 = No request 1 = Fuel cut inhibit 2 = Fuel cut allowed";
                    sidhelper.T7Symbol = "In.ST_TCMCSLU";
                    break;
                case "DTI":
                case "DTI ":
                    sidhelper.Info = "Drivers Torque Intention. The torque that the driver requests converted from air to torque. Limitations from all functions excluding TCM and TCS are included in the signal";
                    sidhelper.T7Symbol = "Out.M_DTI";
                    break;
                case "Fcod":
                    sidhelper.Info = "Codes for errors stored";
                    sidhelper.T7Symbol = "obdFaults";
                    break;
                case "FFAd":
                    sidhelper.Info = "Fuel flow from purge";
                    sidhelper.T7Symbol = "Purge.m_FuelPrg";
                    break;
                case "Flow":
                    sidhelper.Info = "The actual purge flow";
                    sidhelper.T7Symbol = "Purge.Flow";
                    break;
                case "FMXF":
                    sidhelper.Info = "Maximum allowed purge flow in respect to maximum allowed fuel factor at actual load";
                    sidhelper.T7Symbol = "PurgeProt.FuelFacMaxFlow";
                    break;
                case "Frez":
                    sidhelper.Info = "Adaption freeze status";
                    sidhelper.T7Symbol = "PurgeProt.AdpFreeze";
                    break;
                case "Fuel":
                    sidhelper.Info = "Current fuel consumption";
                    sidhelper.T7Symbol = "BFuelProt.CurrentFuelCons";
                    break;
                case "Gear":
                    sidhelper.Info = "Actual gear on automatic gearbox. 2 - Reverse 3 - Neutral 5 - Gear 1 6 - Gear 2 7 - Gear 3 8 - Gear 4 11 - Gear 3, lock up 12 - Gear 4, lock up";
                    sidhelper.T7Symbol = "In.X_ActualGear";
                    break;
                case "GSI":
                case "GSI ":
                    sidhelper.Info = "Prevent TCM from shifting";
                    sidhelper.T7Symbol = "Out.CMD_GearShiftInhibit";
                    break;
                case "HCnt":
                    sidhelper.Info = "The content of HC in the purge air";
                    sidhelper.T7Symbol = "Purge.HCCont";
                    break;
                case "In.X":
                    sidhelper.Info = "Pedal position";
                    sidhelper.T7Symbol = "In.X_AccPedal";
                    break;
                case "Iput":
                    sidhelper.Info = "Transmission input rpm (turbine speed) Used to detect when the load is changed for the engine when gear is engaged";
                    sidhelper.T7Symbol = "ActualIn.n_GearBoxIn";
                    break;
                case "JeLi":
                    sidhelper.Info = "Threshold value for changing shift pattern to no lockup";
                    sidhelper.T7Symbol = "JerkProt.JerkFactor";
                    break;
                case "Jerk":
                    sidhelper.Info = "This factor describes the jerking of the engine";
                    sidhelper.T7Symbol = "ECMStat.JerkFactor";
                    break;
                case "Kph1":
                    sidhelper.Info = "Left front wheel speed";
                    sidhelper.T7Symbol = "ActualIn.v_Vehicle";
                    break;
                case "Kph2":
                    sidhelper.Info = "Vehicle speed, measured on the rear wheel, sent from MIU";
                    sidhelper.T7Symbol = "ActualIn.v_Vehicle2";
                    break;
                case "LwsI":
                    sidhelper.Info = "Stearing angle, only implemented on cars with ESP";
                    sidhelper.T7Symbol = "CanIn.fi_SteeringAngle";
                    break;
                case "mAir":
                    sidhelper.Info = "Airmass in milligram per combustion. This airmass is the actual load value in the ECM";
                    sidhelper.T7Symbol = "MAF.m_AirInlet";
                    break;
                case "Mair":
                    sidhelper.Info = "Maximum torque request from TCS system via CAN";
                    sidhelper.T7Symbol = "In.M_TCSTorqueReq";
                    break;
                case "MiFi":
                    sidhelper.Info = "Number of missfires occurred";
                    sidhelper.T7Symbol = "Missf.nrOfFilteredMisfire";
                    break;
                case "Mnom":
                    sidhelper.Info = "Nominal engine output torque at a certain enginespeed and inlet airmass";
                    sidhelper.T7Symbol = "Torque.M_Nominal";
                    break;
                case "MTCM":
                    sidhelper.Info = "Maximum engine torque request from TCM";
                    sidhelper.T7Symbol = "ActualIn.M_TCMLimitReq";
                    break;
                case "Mtot":
                    sidhelper.Info = "Total torque request from ESP equipped cars";
                    sidhelper.T7Symbol = "In.M_TCSTotalReq";
                    break;
                case "NoIg":
                    sidhelper.Info = "Ignition retardation is not allowed due to overheating the catalytic converter";
                    sidhelper.T7Symbol = "Out.ST_NoIgnitionRetard";
                    break;
                case "Oput":
                    sidhelper.Info = "TCM gearbox output speed converted to vehicle speed";
                    sidhelper.T7Symbol = "DiffPSProt.v_GearBoxOut";
                    break;
                case "Pdif":
                    sidhelper.Info = "Difference between inlet manifold air pressure and external air pressure";
                    sidhelper.T7Symbol = "ECMStat.p_Diff";
                    break;
                case "Peng":
                    sidhelper.Info = "Calculated engine power";
                    sidhelper.T7Symbol = "ECMStat.P_Engine";
                    break;
                case "Perc":
                    sidhelper.Info = "Purge flow/Air mass flow ratio";
                    sidhelper.T7Symbol = "PurgeProt.PurgePercent";
                    break;
                case "PMXF":
                    sidhelper.Info = "Maximum flow allowed by the diff. pressure";
                    sidhelper.T7Symbol = "PurgeProt.PdiffMaxFlow";
                    break;
                case "Ppwm":
                    sidhelper.Info = "Purge valve PWM";
                    sidhelper.T7Symbol = "Purge.Valve";
                    break;
                case "PrSt":
                    sidhelper.Info = "Status of the purge function";
                    sidhelper.T7Symbol = "Purge.Status";
                    break;
                case "ReqF":
                    sidhelper.Info = "Requested purge flow";
                    sidhelper.T7Symbol = "PurgeProt.ReqFlow";
                    break;
                case "ShPn":
                    sidhelper.Info = "Active TCM shift pattern 0 = ECO 1 = Pwr 2 = Wusp 3 = Wnt 4 = US1 5 = US2 6 = Hot1 7 = Hot2 8 = Jerk 9 = Rep 10 = DS 11 = Tap U/D";
                    sidhelper.T7Symbol = "In.ST_TCMShiftPattern";
                    break;
                case "Tign":
                    sidhelper.Info = "No retardation of ignition above this exhaust temperature";
                    sidhelper.T7Symbol = "TorqueCal.T_NoIgnRet";
                    break;
                case "TngA":
                    sidhelper.Info = "Engine coolant temperature";
                    sidhelper.T7Symbol = "ActualIn.T_Engine";
                    break;
                case "TTCM":
                    sidhelper.Info = "Oil temperature in automatic gearbox";
                    sidhelper.T7Symbol = "In.T_TCMOil";
                    break;
                case "tTCM":
                    sidhelper.Info = "Maximum engine torque duration";
                    sidhelper.T7Symbol = "In.t_TCMTrqLimDuration";
                    break;
                case "vGiF":
                    sidhelper.Info = "Yaw velocity";
                    sidhelper.T7Symbol = "CanIn.fi_YawVelocity";
                    break;
                case "vVLF":
                    sidhelper.Info = "Left front wheel speed";
                    sidhelper.T7Symbol = "In.v_Vehicle";
                    break;
                case "vVRF":
                    sidhelper.Info = "Right front wheel speed";
                    sidhelper.T7Symbol = "In.v_Vehicle3";
                    break;
                case "Xacc":
                    sidhelper.Info = "Pedal position";
                    sidhelper.T7Symbol = "Out.X_AccPedal";
                    break;
                case "Texh":
                    sidhelper.Info = "Exhaust Gas temperature between exhaust manifold and turbine inlet (T7 pin 40)";
                    sidhelper.T7Symbol = "ExhaustProt.T_Exhaust";
                    break;
                case "Tmap":
                    sidhelper.Info = "Sum of load/rpm and ignition dependent steady state temperatures";
                    sidhelper.T7Symbol = "ExhaustProt.T_TotMapVal";
                    break;
                case "Tlim":
                    sidhelper.Info = "Exhaust temperature limit for lambda 1";
                    sidhelper.T7Symbol = "ExhaustProt.T_Limit";
                    break;
                case "Tclc":
                    sidhelper.Info = "Delayed value of Exhaust.T_Calc";
                    sidhelper.T7Symbol = "ExhaustProt.T_CalcDly";
                    break;
                case "Eenb":
                    sidhelper.Info = "Is this flag is set and In.T_Engine is above its limit, the exhaust temperature algo becomes active";
                    sidhelper.T7Symbol = "ExhaustCal.ST_Enable";
                    break;
                case "ExST":
                    sidhelper.Info = "Status variable for exhaust temp calculation";
                    sidhelper.T7Symbol = "Exhaust.Status";
                    break;
                case "LmST":
                    sidhelper.Info = "Status byte for closed loop intergrator";
                    sidhelper.T7Symbol = "Lambda.Status";
                    break;
                case "Pgsi":
                    sidhelper.Info = "State of gear shift inhibitor";
                    sidhelper.T7Symbol = "REPProt.ST_GSIState";
                    break;
                case "GSIs":
                    sidhelper.Info = "Enable REP shift pattern on altitude below this pressure";
                    sidhelper.T7Symbol = "REPProt.p_AirAmbient";
                    break;
                case "Mclc":
                    sidhelper.Info = "Engine Torque measured by acceleration of vehicle";
                    sidhelper.T7Symbol = "TorqueProt.M_EngineByAcc";
                    break;
                case "Tlmp":
                    sidhelper.Info = "";
                    sidhelper.T7Symbol = "LimpIn.T_Engine";
                    break;
                case "Aft1":
                    sidhelper.Info = "";
                    sidhelper.T7Symbol = "AftStProt1.EnrFac";
                    break;
                case "IpN1":
                    sidhelper.Info = "";
                    sidhelper.T7Symbol = "AirctlData.iPartN1";
                    break;
                case "TFac":
                    sidhelper.Info = "";
                    sidhelper.T7Symbol = "TCompProt.EnrFac";
                    break;
                case "Area":
                    sidhelper.Info = "";
                    sidhelper.T7Symbol = "In.A_Throttle";
                    break;


            }
            //return retval;

              /*
               * 
ReqF  PurgeProt.ReqFlow  Requested purge flow. Resolution is 1 mg/s.  
ShPn  In.ST_TCMShiftPattern  Active TCM shift pattern 0 = ECO 1 = Pwr 2 = Wusp 3 = Wnt 4 = US1 5 = US2 6 = Hot1 7 = Hot2 8 = Jerk 9 = Rep 10 = DS 11 = Tap U/D  
Tign  TorqueCal.T_NoIgnRet  No retardation of ignition above this exhaust temperature  
Tlmp  unknown   
TngA  ActualIn.T_Engine  Engine coolant temperature UNIT : (C MAX : 150 MIN : -40 TRANS : V = P. Resolution is 1. Interval is 1000 ms.  
TTCM  In.T_TCMOil  Oil temperature in automatic gearbox  
tTCM  In.t_TCMTrqLimDuration  Maximum engine torque duration UNIT : ms MAX : 2500 MIN : 0 TRANS : V = P. Resolution is 1. Interval is 10 ms.  
vGiF  CanIn.fi_YawVelocity  Yaw velocity (vGiF), only implemented on cars with ESP. Resolution is 0.02 °.  
vVLF  In.v_Vehicle  Left front wheel speed UNIT : km/h MAX : 300 MIN : 0 (detection of min. 1.0 km/h) TRANS : V = P * 10. Resolution is 0.1. Interval is 100 ms.  
vVRF  In.v_Vehicle3  Right front wheel speed UNIT : km/h MAX : 300 MIN : 0 (detection of min. 1.0 km/h) TRANS : V = P * 10. Resolution is 0.1. Interval is 100 ms.  
Xacc  Out.X_AccPedal  Pedal position UNIT : % MAX : 100 MIN : 0 TRANS : V = P*1. Resolution is 0.1. Interval is 20ms.  
            */
        }
    }
}
