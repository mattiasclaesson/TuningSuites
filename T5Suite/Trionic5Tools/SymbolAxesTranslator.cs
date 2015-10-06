using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class SymbolAxesTranslator
    {
        public string GetXaxisSymbol(string symbolname)
        {
            string retval = string.Empty;
            if (symbolname.StartsWith("Ign_map_0!") || symbolname == "Knock_count_map")
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
            else if (symbolname == "Lambdamatris!" || symbolname == "Lambdamatris_diag!")
            {
                retval = "Lam_laststeg!";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                //retval = "Trans_x_st!";
                retval = "Pwm_ind_trot!";
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
//                retval = "Trans_x_st!";
                retval = "Pwm_ind_trot!";
            }
            else if (symbolname.StartsWith("Insp_mat!") || symbolname == "Adapt_ggr" || symbolname == "Adapt_ref" || symbolname == "Adapt_ind_mat")
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval = "Fuel_map_xaxis!";
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                retval = "Inj_map_0_x_axis!";
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
            if (symbolname.StartsWith("Ign_map_0!") || symbolname == "Knock_count_map")
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
            else if (symbolname.StartsWith("Insp_mat!") || symbolname == "Lambdamatris!" || symbolname == "Lambdamatris_diag!")
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                retval = "Fuel_map_yaxis!";
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                retval = "Inj_map_0_y_axis!";
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
            else if (symbolname.StartsWith("Adapt_korr") || symbolname == "Adapt_ind_mat!")
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
                //TODO: this is only for 3D reg_kon_mat maps... otherwise it's hardcoded
                retval = "Pwm_ind_rpm!";
                //retval = "";
            }
            else if (symbolname.StartsWith("Luft_kompfak!"))
            {
                retval = "Lufttemp_steg!";
            }

            return retval;
        }

    }
}
