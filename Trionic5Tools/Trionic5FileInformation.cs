using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CommonSuite;

namespace Trionic5Tools
{
    public class Trionic5FileInformation : IECUFileInformation
    {
        public enum TrionicFileLength : int
        {
            Trionic52Length = 0x20000,
            Trionic55Length = 0x40000,
            Trionic7Length = 0x80000,
            Trionic8Length = 0x100000
        }
        public Trionic5FileInformation()
        {
            m_SymbolCollection = new SymbolCollection();
            m_AddressCollection = new AddressLookupCollection();
        }

        private int m_filelength = 0x40000;

        public override int Filelength
        {
            get { return m_filelength; }
            set { m_filelength = value; }
        }

        private string m_SRAMfilename = string.Empty;

        public override string SRAMfilename
        {
            get { return m_SRAMfilename; }
            set { m_SRAMfilename = value; }
        }

        private string m_filename = string.Empty;

        public override bool Has2DRegKonMat()
        {
            if (GetSymbolLength("Reg_kon_mat!") == 0x80) return false;
            return true;
        }

        public override string GetSymbolNameByAddress(int address)
        {
            if (address == m_filelength - 0x1E0) return "Sync timestamp";
            if (m_SymbolCollection == null) return "";
            
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Flash_start_address == address) return sh.Varname;
            }
            return "";
        }

        public override string Filename
        {
            get { return m_filename; }
            set
            {
                m_filename = value;
                FileInfo fi = new FileInfo(m_filename);
                m_filelength = (int)fi.Length;
            }
        }

        public override Int32 GetSymbolAddressSRAM(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname) return (int)sh.Start_address;
            }
            return 0;
        }

        public override Int32 GetSymbolAddressFlash(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname) return (int)sh.Flash_start_address;
            }
            return 0;
        }

        public override Int32 GetSymbolLength(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname) return sh.Length;
            }
            return 0;
        }


        private SymbolCollection m_SymbolCollection;

        public override SymbolCollection SymbolCollection
        {
            get { return m_SymbolCollection; }
            set { m_SymbolCollection = value; }
        }

        private AddressLookupCollection m_AddressCollection;

        public override AddressLookupCollection AddressCollection
        {
            get { return m_AddressCollection; }
            set { m_AddressCollection = value; }
        }

        public override string GetPressureSymbol()
        {
            // trionic 5 pressure realtime symbol = P_medel
            return "P_medel";
        }

        public override string GetAirTempSymbol()
        {
            return "Lufttemp";
        }

        public override string GetCoolantTempSymbol()
        {
            return "Kyl_temp";
        }

        public override string GetEngineSpeedSymbol()
        {
            return "Rpm";
        }

        public override string GetLambdaSymbol()
        {
            return "AD_sond";
        }

        public override string GetProgramModeSymbol()
        {
            return "Pgm_mod!";
        }


        public override string GetProgramStatusSymbol()
        {
            return "Pgm_status";
        }

        public override string GetInjectorConstant()
        {
            return "Inj_konst!";
        }
        public override string GetIgnitionMap()
        {
            return "Ign_map_0!";
        }
        public override string GetIgnitionKnockMap()
        {
            return "Ign_map_2!";
        }
        public override string GetIgnitionWarmupMap()
        {
            return "Ign_map_4!";
        }

        public override string GetBoostRequestMap()
        {
            return "Tryck_mat!";
        }

        public override string GetBoostBiasMap()
        {
            return "Reg_kon_mat!";
        }
        public override string GetFuelcutMap()
        {
            return "Tryck_vakt_tab!";
        }

        public override string GetPFactorsMap()
        {
            return "P_fors!";
        }
        public override string GetIFactorsMap()
        {
            return "I_fors!";
        }
        public override string GetDFactorsMap()
        {
            return "D_fors!";
        }

        public override string GetPFactorsMapAUT()
        {
            return "P_fors_a!";
        }
        public override string GetIFactorsMapAUT()
        {
            return "I_fors_a!";
        }
        public override string GetDFactorsMapAUT()
        {
            return "D_fors_a!";
        }
        public override string GetBoostBiasMapAUT()
        {
            return "Reg_kon_mat_a!";
        }

        public override string GetBoostRequestMapAUT()
        {
            return "Tryck_mat_a!";
        }

        public override string GetBoostLimiterFirstGearMapAUT()
        {
            if (m_filelength == (int)TrionicFileLength.Trionic52Length)
            {
                return "Regl_tryck_fga!";
            }
            else
            {
                return "Regl_tryck_fgaut!";
            }
        }

        public override string GetBoostLimiterFirstGearMap()
        {
            return "Regl_tryck_fgm!";
        }

        public override string GetOpenLoopMap()
        {
            return "Open_loop!";
        }
        public override string GetOpenLoopKnockMap()
        {
            return "Open_loop_knock!";
        }

        public override bool isSixteenBitTable(string symbolname)
        {
            if (symbolname.StartsWith("Ign_map_0")) return true;
            else if (symbolname.StartsWith("Ign_map_1")) return true;
            else if (symbolname.StartsWith("Ign_map_2")) return true;
            else if (symbolname.StartsWith("Ign_map_3")) return true;
            else if (symbolname.StartsWith("Ign_map_4")) return true;
            else if (symbolname.StartsWith("Ign_map_5")) return true;
            else if (symbolname.StartsWith("Ign_map_6")) return true;
            else if (symbolname.StartsWith("Ign_map_7")) return true;
            else if (symbolname.StartsWith("Ign_map_8")) return true;
            else if (symbolname.StartsWith("Inj_map_0")) return true;
            else if (symbolname.StartsWith("Idle_step_ioka!")) return true;
            else if (symbolname.StartsWith("Idle_step_imns!")) return true;
            else if (symbolname.StartsWith("Derivata_grans!")) return true; // * 10

            else if (symbolname.StartsWith("Lambdamatris!")) return true; // * 10
            else if (symbolname.StartsWith("Lambdamatris_diag!")) return true; // * 10

            else if (symbolname.StartsWith("Iv_min_tab!")) return true;
            else if (symbolname.StartsWith("Iv_min_tab_ac!")) return true;
            else if (symbolname.StartsWith("Misfire_map_x_axis!")) return true;
            else if (symbolname.StartsWith("Max_ratio_aut!")) return true;
            else if (symbolname.StartsWith("Mis200_map!")) return true;
            else if (symbolname.StartsWith("Misfire_map!")) return true;
            else if (symbolname.StartsWith("Start_insp!")) return true;
            else if (symbolname.StartsWith("Start_max!")) return true;
            else if (symbolname.StartsWith("Start_ramp!")) return true;
            else if (symbolname.StartsWith("Reg_kon_mat"))
            {
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (symbolname.StartsWith("Max_regl_temp_1")) return true;
            else if (symbolname.StartsWith("Max_regl_temp_2")) return true;
            else if (symbolname.StartsWith("Mis1000_map!")) return true;
            else if (symbolname.StartsWith("Knock_ref_matrix!")) return true;
            else if (symbolname.StartsWith("Detect_map!")) return true; //??
            else if (symbolname.StartsWith("Knock_wind_on_tab!")) return true;
            else if (symbolname.StartsWith("Lknock_oref_tab!")) return true;
            else if (symbolname.StartsWith("Knock_lim_tab!")) return true;
            else if (symbolname.StartsWith("Knock_wind_off_tab!")) return true;
            else if (symbolname.StartsWith("Turbo_knock_tab!")) return true;
            else if (symbolname.StartsWith("Knock_press_tab!")) return true;
            else if (symbolname.StartsWith("Idle_drift_tab!")) return true;
            else if (symbolname.StartsWith("Last_reg_kon!")) return true;
            else if (symbolname.StartsWith("Shift_rpm! (automaat?)")) return true;
            else if (symbolname.StartsWith("Ign_map_6_x_axis!")) return true;
            else if (symbolname.StartsWith("Ign_map_6_y_axis!")) return true;
            else if (symbolname.StartsWith("Gear_st!")) return true;
            else if (symbolname.StartsWith("Reg_varv!")) return true;
            else if (symbolname.StartsWith("Trans_y_st!")) return true;
            else if (symbolname.StartsWith("Idle_st_rpm!")) return true;
            else if (symbolname.StartsWith("Dash_rpm_axis!")) return true;
            else if (symbolname.StartsWith("Ramp_down_ram!")) return true;
            else if (symbolname.StartsWith("Ramp_up_ram!")) return true;
            else if (symbolname.StartsWith("Idle_start_extra!")) return true;
            else if (symbolname.StartsWith("Idle_start_extra_ramp!")) return true;
            else if (symbolname.StartsWith("Idle_temp_off!")) return true;
            else if (symbolname.StartsWith("Idle_rpm_tab!")) return true;
            else if (symbolname.StartsWith("Derivata_br_sp!")) return true;
            else if (symbolname.StartsWith("Derivata_br_tab_pos!")) return true;
            else if (symbolname.StartsWith("Derivata_br_tab_neg!")) return true;
            else if (symbolname.StartsWith("Br_plus_tab!")) return true;
            else if (symbolname.StartsWith("Br_minus_tab!")) return true;
            else if (symbolname.StartsWith("Start_tab!")) return true;
            else if (symbolname.StartsWith("Temp_reduce_y_st!")) return true;
            else if (symbolname.StartsWith("Idle_tryck!")) return true;
            else if (symbolname.StartsWith("Iv_start_time_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_filt_coef_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox2_filt_coef_tab!")) return true;
            else if (symbolname.StartsWith("Cat_air_flow_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_per_hi_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_per_lo_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_err_max_tab!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_dev_max_tab!")) return true;
            else if (symbolname.StartsWith("Batt_korr_tab!")) return true;
            else if (symbolname.StartsWith("Kadapt_max_ref!")) return true;
            else if (symbolname.StartsWith("Ign_map_3_x_axis!")) return true;
            else if (symbolname.StartsWith("Last_varv_st!")) return true;
            else if (symbolname.StartsWith("Fuel_map_yaxis!")) return true;
            else if (symbolname.StartsWith("Ign_map_3_y_axis!")) return true;
            else if (symbolname.StartsWith("Pwm_ind_rpm!")) return true;
            else if (symbolname.StartsWith("Max_regl_sp!")) return true;
            else if (symbolname.StartsWith("Idle_ac_tab!")) return true;
            else if (symbolname.StartsWith("Wait_count_tab!")) return true;
            else if (symbolname.StartsWith("Knock_average_tab!")) return true;
            else if (symbolname.StartsWith("Knock_wind_rpm!")) return true;
            else if (symbolname.StartsWith("Detect_map_y_axis!")) return true;
            else if (symbolname.StartsWith("Misfire_map_y_axis!")) return true;
            else if (symbolname.StartsWith("Detect_map_x_axis!")) return true;
            else if (symbolname.StartsWith("Rpm_max!")) return true;
            else if (symbolname.StartsWith("Tid_Konst!")) return true;
            else if (symbolname.StartsWith("Min_tid!")) return true;
            else if (symbolname.StartsWith("Kadapt_rpm_high!")) return true; // * 10
            else if (symbolname.StartsWith("Kadapt_rpm_low!")) return true; // * 10
            else if (symbolname.StartsWith("Knock_start!")) return true;
            else if (symbolname.StartsWith("Min_rpm_closed_loop!")) return true;// * 10
            else if (symbolname.StartsWith("Min_rpm_gadapt!")) return true; // * 10 
            else if (symbolname.StartsWith("Ign_offset!")) return true;
            else if (symbolname.StartsWith("Ign_offset_cyl1!")) return true;
            else if (symbolname.StartsWith("Ign_offset_cyl2!")) return true;
            else if (symbolname.StartsWith("Ign_offset_cyl3!")) return true;
            else if (symbolname.StartsWith("Ign_offset_cyl4!")) return true;
            else if (symbolname.StartsWith("Ign_offset_adapt!")) return true;
            else if (symbolname.StartsWith("Max_rpm_gadapt!")) return true; // * 10
            else if (symbolname.StartsWith("Temp_ramp_value!")) return true;
            else if (symbolname.StartsWith("Global_adapt_nr!")) return true;
            else if (symbolname.StartsWith("Ign_idle_angle!")) return true; // /10
            else if (symbolname.StartsWith("Knock_ang_dec")) return true;
            else if (symbolname.StartsWith("Knock_wind_rpm")) return true;
            else if (symbolname.StartsWith("Knock_matrix_time")) return true;
            else if (symbolname.StartsWith("Radius_of_roll!")) return true;
            else if (symbolname.StartsWith("Knock_ref_tab!")) return true;
            else if (symbolname.StartsWith("Frek_230!")) return true;
            else if (symbolname.StartsWith("Frek_250!")) return true;
            else if (symbolname.StartsWith("Min_tid!")) return true;
            else if (symbolname.StartsWith("Mis_trans_limit!")) return true;
            else if (symbolname.StartsWith("Derivata_fuel_rpm!")) return true; // * 10
            else if (symbolname.StartsWith("Tid_konst!")) return true;
            else if (symbolname.StartsWith("Ret_delta_rpm!")) return true; // * 10 
            else if (symbolname.StartsWith("Ret_down_rpm!")) return true; // * 10
            else if (symbolname.StartsWith("Ret_up_rpm!")) return true; // * 10
            else if (symbolname.StartsWith("Start_time_rpm_lim!")) return true;
            else if (symbolname.StartsWith("Start_v_vinkel!")) return true;
            else if (symbolname.StartsWith("Cut_ej_under!")) return true;
            else if (symbolname.StartsWith("Open_all_varv!")) return true; // * 10
            else if (symbolname.StartsWith("Open_varv!")) return true; // * 10
            else if (symbolname.StartsWith("Vinkel_konst!")) return true;
            else if (symbolname.StartsWith("API_ku_delay!")) return true;
            else if (symbolname.StartsWith("API_ku_derivata!")) return true;
            else if (symbolname.StartsWith("API_ku_offset!")) return true;
            else if (symbolname.StartsWith("API_ku_ramp!")) return true;
            else if (symbolname.StartsWith("Airpump_ign_offset!")) return true;
            else if (symbolname.StartsWith("Rev_grad_sens_cont!")) return true;
            else if (symbolname.StartsWith("PMCal_RpmIdleNomRefLim!")) return true;
            else if (symbolname.StartsWith("Ap_max_on_time!")) return true;
            else if (symbolname.StartsWith("Ap_max_rpm!")) return true;
            else if (symbolname.StartsWith("API_dt_delay!")) return true;
            else if (symbolname.StartsWith("API_dt_ramp!")) return true;
            else if (symbolname.StartsWith("API_rpm_limit!")) return true;
            else if (symbolname.StartsWith("Diag_speed_time!")) return true;
            else if (symbolname.StartsWith("Ox2_activity_time_lim!")) return true;
            else if (symbolname.StartsWith("Ox2_change_lim!")) return true;
            else if (symbolname.StartsWith("PMCal_CloseRamp!")) return true;
            else if (symbolname.StartsWith("PMCal_IdlePosFiltCoef!")) return true;
            else if (symbolname.StartsWith("PMCal_IdlePosIntABSLim!")) return true;
            else if (symbolname.StartsWith("PMCal_IdleRefHaltLim!")) return true;
            else if (symbolname.StartsWith("PMCal_IdleValueLim!")) return true;
            else if (symbolname.StartsWith("PMCal_StartRamp!")) return true;
            else if (symbolname.StartsWith("Shut_off_time!")) return true;
            else if (symbolname.StartsWith("Sond_omsl_lim!")) return true;
            else if (symbolname.StartsWith("Start_detekt_nr!")) return true;
            else if (symbolname.StartsWith("Start_detekt_rpm!")) return true; // * 10
            else if (symbolname.StartsWith("Synk_ok_lim!")) return true;
            else if (symbolname.StartsWith("Ap_lambda_delay!")) return true;
            else if (symbolname.StartsWith("Cat_af_start_timer_min!")) return true;
            else if (symbolname.StartsWith("Cat_air_flow_hi!")) return true;
            else if (symbolname.StartsWith("Cat_air_flow_lo!")) return true;
            else if (symbolname.StartsWith("Cat_load_filt_coef!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_bias_lim!")) return true;
            else if (symbolname.StartsWith("Cat_ox1_err_lim!")) return true;
            else if (symbolname.StartsWith("Cat_ox2_filt_coef_2!")) return true;
            else if (symbolname.StartsWith("Cat_stage1_threshold!")) return true;
            else if (symbolname.StartsWith("Cat_start_timer_lim!")) return true;
            else if (symbolname.StartsWith("Lambda_cat_lean!")) return true;
            else if (symbolname.StartsWith("Lambda_cat_rich!")) return true;
            else if (symbolname.StartsWith("PMCal_LambdaAvgHaltLim!")) return true;
            else if (symbolname.StartsWith("PMCal_LambdaAvgLim!")) return true;
            else if (symbolname.StartsWith("Sond_ign_limit!")) return true;
            else if (symbolname.StartsWith("AC_delay!")) return true;
            else if (symbolname.StartsWith("Adapt_purge_period!")) return true;
            else if (symbolname.StartsWith("Ap_ign_offset_delay!")) return true;
            else if (symbolname.StartsWith("Ap_ign_offset_delay2!")) return true;
            else if (symbolname.StartsWith("Ap_ign_step!")) return true;
            else if (symbolname.StartsWith("Ap_inj_factor_delay!")) return true;
            else if (symbolname.StartsWith("Ap_inj_factor_delay2!")) return true;
            else if (symbolname.StartsWith("Ap_max_on_time2!")) return true;
            else if (symbolname.StartsWith("Ap_min_stop_time!")) return true;
            else if (symbolname.StartsWith("Ap_start_delay!")) return true;
            else if (symbolname.StartsWith("Ap_start_delay2!")) return true;
            else if (symbolname.StartsWith("Cat_fc_timer_lim!")) return true;
            else if (symbolname.StartsWith("Cat_hl_air_flow_min!")) return true;
            else if (symbolname.StartsWith("Cat_hl_timer_lim!")) return true;
            else if (symbolname.StartsWith("Cat_vs_timer_lim!")) return true;
            else if (symbolname.StartsWith("Cl_timer1_lim!")) return true;
            else if (symbolname.StartsWith("Cl_timer2_lim!")) return true;
            else if (symbolname.StartsWith("Comb_w_temp_limbhome!")) return true;
            else if (symbolname.StartsWith("I_fak_max!")) return true;
            else if (symbolname.StartsWith("Ign_idle_angle_start!")) return true;
            else if (symbolname.StartsWith("Jd_lastvar!")) return true;
            else if (symbolname.StartsWith("Kadapt_period!")) return true;
            else if (symbolname.StartsWith("Knock_back!")) return true;
            else if (symbolname.StartsWith("Knock_offset!")) return true;
            else if (symbolname.StartsWith("Knock_reduce!")) return true;
            else if (symbolname.StartsWith("LLS_min!")) return true;
            else if (symbolname.StartsWith("Mainrly_off!")) return true;
            else if (symbolname.StartsWith("Max_neg_der!")) return true;
            else if (symbolname.StartsWith("Max_pos_der!")) return true;
            else if (symbolname.StartsWith("Max_um_time!")) return true;
            else if (symbolname.StartsWith("Nr_adapt_idle_mat!")) return true;
            else if (symbolname.StartsWith("Misf_rpm_diff!")) return true;
            else if (symbolname.StartsWith("Misf_rpm_diff_start!")) return true;
            else if (symbolname.StartsWith("P_tank_filt_fak!")) return true;
            else if (symbolname.StartsWith("Pdec_min!")) return true;
            else if (symbolname.StartsWith("Pinc_min!")) return true;
            else if (symbolname.StartsWith("Pperf_min!")) return true;
            else if (symbolname.StartsWith("Press_perf_max!")) return true;
            else if (symbolname.StartsWith("Press_perf_min!")) return true;
            else if (symbolname.StartsWith("Press_rpm_lim!")) return true; // * 10
            else if (symbolname.StartsWith("Rev_grad_sens_dur_req!")) return true;
            else if (symbolname.StartsWith("Rpm_dif!")) return true; // * 10
            else if (symbolname.StartsWith("Rpm_perf_max!")) return true; // * 10
            else if (symbolname.StartsWith("Rpm_perf_min!")) return true; // * 10
            else if (symbolname.StartsWith("Shift_max_rpm!")) return true; // * 10??
            else if (symbolname.StartsWith("Shift_up_off_time!")) return true;
            else if (symbolname.StartsWith("Shift_up_rpm_hyst!")) return true; // * 10 ??
            else if (symbolname.StartsWith("Shift_up_time!")) return true;
            else if (symbolname.StartsWith("Sync_counter!")) return true;
            else if (symbolname.StartsWith("Sync_rpm!")) return true; // * 10 ??
            else if (symbolname.StartsWith("Trans_offset!")) return true;
            else if (symbolname.StartsWith("Trans_trott_limit!")) return true;
            else if (symbolname.StartsWith("Diag_speed_rpm!")) return true;
            else if (symbolname.StartsWith("I_last_rpm!")) return true;
            else if (symbolname.StartsWith("Cat_rpm_tab!")) return true;
            else if (symbolname.StartsWith("Lam_rpm_sp!")) return true;
            else if (symbolname.StartsWith("Shift_rpm!")) return true;
            else if (symbolname.StartsWith("Pulses_per_rev!")) return true;
            else if (symbolname.StartsWith("Apc_adapt")) return true;
            else if (symbolname.StartsWith("Knock_count_cyl")) return true;
            else if (symbolname.StartsWith("Knock_count_map")) return true;
            else if (symbolname.StartsWith("Adapt_ggr")) return true;

            else if (symbolname.StartsWith("Knock_average")) return true;
            else if (symbolname.StartsWith("Knock_average_limit")) return true;
            else if (symbolname.StartsWith("Knock_map_lim")) return true;
            else if (symbolname.StartsWith("Knock_map_offset")) return true;
            else if (symbolname.StartsWith("Knock_offset1234")) return true;
            else if (symbolname.StartsWith("Lknock_oref_level")) return true;
            else if (symbolname.StartsWith("Turbo_knock_press")) return true;

            else if (symbolname.StartsWith("Knock_offset1")) return true;
            else if (symbolname.StartsWith("Knock_offset2")) return true;
            else if (symbolname.StartsWith("Knock_offset3")) return true;
            else if (symbolname.StartsWith("Knock_offset4")) return true;
            else if (symbolname.StartsWith("Knock_press_limit")) return true;
            else if (symbolname.StartsWith("Knock_diag_level")) return true;
            else if (symbolname.StartsWith("Knock_level")) return true;
            else if (symbolname.StartsWith("Knock_lim")) return true;
            else if (symbolname.StartsWith("Knock_ref_level")) return true;
            else if (symbolname.StartsWith("Knock_wind_off")) return true;
            else if (symbolname.StartsWith("Knock_wind_on")) return true;
            else if (symbolname.StartsWith("Knock_wind_off_ang")) return true;
            else if (symbolname.StartsWith("Knock_wind_on_ang")) return true;
            else if (symbolname.StartsWith("Ign_angle_byte")) return false;
            else if (symbolname.StartsWith("Ign_angle")) return true;
            else if (symbolname.StartsWith("Reg_kon_apc")) return true;
            return false;
        }

        public override string GetBoostLimiterSecondGearMap()
        {
            return "Regl_tryck_sgm!";
        }

        public override string GetKnockLimitMap()
        {
            return "Knock_lim_tab!";
        }

        public override string GetBoostControlOffsetSymbol()
        {
            return "Reg_kon_apc";
        }

        public override string GetBoostKnockMap()
        {
            return "Apc_knock_tab!";
        }

        public override string GetBatteryCorrectionMap()
        {
            return "Batt_korr_tab!";
        }

        

        public override string GetKnockSensitivityMap()
        {
            if (m_filelength == (int)TrionicFileLength.Trionic52Length)
            {
                return "Knock_ref_tab!";
            }
            else
            {
                return "Knock_ref_matrix!";
            }
        }

        public override string GetIdleFuelMap()
        {
            return "Idle_fuel_korr!";
        }

        public override string GetIdleTargetRPMMap()
        {
            return "Idle_rpm_tab!";
        }

        public override string GetIdleIgnition()
        {
            return "Ign_idle_angle!";
        }

        public override string GetIdleIgnitionCorrectionMap()
        {
            return "Ign_map_1!";
        }

        public override string GetFirstAfterStartEnrichmentMap()
        {
            return "Eftersta_fak!";
        }

        public override string GetSecondAfterStartEnrichmentMap()
        {
            return "Eftersta_fak2!";
        }

        public override string GetInjectionDurationSymbol()
        {
            return "Insptid_ms10";
        }

        public override string GetInjectionMap()
        {
            if (GetSymbolAddressFlash("Inj_map_0!") > 0) return "Inj_map_0!";
            return "Insp_mat!";
        }

        public override string GetInjectionMapLOLA()
        {
            return "Inj_map_0!";
        }

        public override string GetInjectionKnockMap()
        {
            return "Fuel_knock_mat!";
        }

        public override string GetEnrichmentForLoadSymbol()
        {
            return "Lacc_mangd";
        }

        public override string GetEnrichmentForTPSSymbol()
        {
            return "Acc_mangd";
        }

        public override string GetEnleanmentForLoadSymbol()
        {
            return "Lret_mangd";
        }

        public override string GetEnleanmentForTPSSymbol()
        {
            return "Ret_mangd";
        }

        public override string GetIgnitionAdvanceSymbol()
        {
            return "Ign_angle";
        }

        public override string GetKnockOffsetCylinder1Symbol()
        {
            return "Knock_offset1";
        }

        public override string GetKnockOffsetCylinder2Symbol()
        {
            return "Knock_offset2";
        }

        public override string GetKnockOffsetCylinder3Symbol()
        {
            return "Knock_offset3";
        }

        public override string GetKnockOffsetCylinder4Symbol()
        {
            return "Knock_offset4";
        }

        public override string GetBoostRequestSymbol()
        {
            return "Max_tryck";
        }

        public override string GetBoostTargetSymbol()
        {
            return "Regl_tryck";
        }

        public override string GetThrottlePositionSymbol()
        {
            return "Medeltrot"; // Trot_min?
        }

        public override string GetBoostReductionSymbol()
        {
            return "Apc_decrese";
        }

        public override string GetPFactorSymbol()
        {
            return "P_fak";
        }

        public override string GetIFactorSymbol()
        {
            return "I_fak";
        }

        public override string GetDFactorSymbol()
        {
            return "D_fak";
        }

        public override string GetPWMOutputSymbol()
        {
            return "PWM_ut10";
        }

        public override string GetKnockCountCylinder1Symbol()
        {
            return "Knock_count_cyl1";
        }

        public override string GetKnockCountCylinder2Symbol()
        {
            return "Knock_count_cyl2";
        }

        public override string GetKnockCountCylinder3Symbol()
        {
            return "Knock_count_cyl3";
        }

        public override string GetKnockCountCylinder4Symbol()
        {
            return "Knock_count_cyl4";
        }

        public override string GetKnockLevelSymbol()
        {
            return "Knock_diag_level";
        }

        public override string GetVehicleSpeedSymbol()
        {
            return "Bil_hast";
        }

        public override string GetTorqueSymbol()
        {
            return "TQ";
        }

        public override string GetKnockOffsetAllCylindersSymbol()
        {
            return "Knock_offset1234"; // not in T5.2
        }

        public override XDFCategories GetSymbolCategory(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    return sh.XdfCategory;
                }
            }
            return XDFCategories.Undocumented;
        }

        public override XDFSubCategory GetSymbolSubcategory(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    return sh.XdfSubcategory;
                }
            }
            return XDFSubCategory.Undocumented;
        }

        public override string GetSymbolDescription(string symbolname)
        {
            foreach (SymbolHelper sh in m_SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    return sh.Helptext;
                }
            }
            return symbolname;
        }
    }
}
