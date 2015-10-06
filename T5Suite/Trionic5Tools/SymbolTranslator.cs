using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public class SymbolTranslator
    {
        public string TranslateSymbolToHelpText(string symbolname, out string helptext, out XDFCategories category, out XDFSubCategory subcategory)
        {
            if (symbolname.EndsWith("!")) symbolname = symbolname.Substring(0, symbolname.Length - 1);
            helptext = "";
            category = XDFCategories.Undocumented;
            subcategory = XDFSubCategory.Undocumented;
            string description = "";
            switch (symbolname)
            {

                case "Shift_load":
                case "Shift_max_rpm":
                case "Shift_rpm":
                case "Shift_up_load_hyst":
                case "Shift_up_off_time":
                case "Shift_up_rpm_hyst":
                case "Shift_up_temp":
                case "Shift_up_time":
                    helptext = description = symbolname;
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Shifting;
                    break;
                case "Overs_angle":
                case "Overs_komp_tab":
                case "Regl_temp_max":
                    helptext = description = symbolname;
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_rpm_high":
                    helptext = description = "Upper engine speed boundary for knock adaption" ;
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_rpm_low":
                    helptext = description = "Lower engine speed boundary for knock adaption";
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_counter":
                    helptext = description = "Number of knock adaptions done";
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_load_high":
                    helptext = description = "Upper engine load boundary to allow knock adaption";
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_load_low":
                    helptext = description = "Lower engine load boundary to allow knock adaption";
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kadapt_max_ref":
                case "Kadapt_period":
                case "Kadapt_ref_value":
                case "Kadapt_ref_value_actual":
                case "Load_adapt_time":
                case "Nr_adapt_idle_mat":
                case "Adaption_time":
                case "Adapt_purge_period":
                case "Adapt_duty":
                    helptext = description = symbolname;
                    category = XDFCategories.Adaption;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "Ign_map_request":
                case "Ign_map_value":
                    helptext = description = symbolname;
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "Idle_ac_ext":
                case "Idle_frame":
                case "Idle_update":
                    helptext = description = symbolname;
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_idle_angle_start":
                    helptext = description = "Ignition angle when coming on idle";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "OBD_fuel_status":
                case "Rpm_byte":
                case "Rpm_dif":
                case "S_Kyl_temp":
                case "S_Lufttemp":
                    helptext = description = symbolname;
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "I_fak_max":
                    helptext = description = "Maximum achievable I factor for PID controller";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Grund_last":
                    helptext = description = "Base load in the entire system, used for calculation of injection time a.o.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "Diag_buff":
                case "Diag_f_buff":
                case "Diag_mod":
                case "Diag_p_duty":
                    helptext = description = symbolname;
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Det_ok_counter_cyl1":
                case "Det_ok_counter_cyl2":
                case "Det_ok_counter_cyl3":
                case "Det_ok_counter_cyl4":
                    helptext = description = symbolname;
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "Adapt_error":
                    helptext = description = "Adaption error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Airpump_error":
                    helptext = description = "Airpump control error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Cat_error":
                    helptext = description = "Catalyst error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Dog_error":
                    helptext = description = "Watchdog error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Error_frame":
                    helptext = description = "Complete error frame";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Hast_error":
                    helptext = description = "Speed signal error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Idle_error":
                    helptext = description = "Idle control error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Kyl_error":
                    helptext = description = "Coolant temperature sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Luft_error":
                    helptext = description = "Intake air temperature sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Mis_CVS_error":
                    helptext = description = "Misfire error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Mis_heating_error":
                    helptext = description = "Misfire catalyst overheating error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Purge_error":
                    helptext = description = "Purge valve control error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "R_sond_error":
                    helptext = description = "Rear (second) O2 sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "RAM_error":
                    helptext = description = "SRAM integrity error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "ROM_error":
                    helptext = description = "Flash (ROM) integrity error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Sond_error":
                    helptext = description = "Front (first) O2 sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;

                case "Sond_omsl_error":
                case "Sync_error":
                    helptext = description = symbolname;
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Trott_error":
                    helptext = description = "Throttle position sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Tryck_error":
                    helptext = description = "MAP sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "VSS_error":
                    helptext = description = "VSS (Vehicle Security System) error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Airpump_frame":
                case "Airpump_state":
                case "Airpump_time":
                case "Ap_ign_offset_delay":
                case "Ap_ign_offset_delay2":
                case "Ap_ign_step":
                case "Ap_inj_factor_delay":
                case "Ap_inj_factor_delay2":
                case "Ap_max_on_time2":
                case "Ap_max_startwatertemp2":
                case "Ap_min_stop_time":
                case "Ap_min_watertemp":
                case "Ap_start_delay":
                case "Ap_start_delay2":
                    helptext = description = symbolname;
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Write_protect":
                    helptext = description = "SRAM write protect flag for Trionic 5.2 (1 = locked, 0 = unlocked)";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_ang_dec": 
                    helptext = description = "Current ignition decrease angle because of knocking conditions";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_average": 
                    helptext = description = "Current average knock retard for all cylinders";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_average_limit": 
                    helptext = description = "Current average knock ignition retard limit from Knock_average_tab in degrees";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_average_tab": 
                    helptext = description = "Average allowed knock ignition retard values per RPM site";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_back": 
                    helptext = description = "Ignition decrease step in degrees when knock occurs.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_frame": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_map_lim": 
                    helptext = description = "Current knock limit.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_map_offset": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset1234": 
                    helptext = description = "Current ignition offset for all cylinders because of knock.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_reduce": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Lknock_oref_level": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Turbo_knock_press": 
                    helptext = description = "No additional info.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Min_rpm_closed_loop":
                    helptext = description = "Minimum engine speed to run in closed loop";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lambda_cat_rich":
                    helptext = description = "Lambda sensor rich value";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Knock_matrix_time":
                    helptext = description = "Duration in milliseconds that knocking mode will be active after detecting two knocks";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Lambda_cat_lean":
                    helptext = description = "Lambda sensor lean value";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Temp_reduce_y_st":
                    helptext = description = "Temperature reduction fuel correction y axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Temp_reduce_x_st":
                    helptext = description = "Temperature reduction fuel correction x axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Pwm_ind_rpm":
                    helptext = description = "Rpm axis for several tables";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Overs_tab_xaxis":
                    helptext = description = "Overshoot table x axis";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Misfire_map_y_size":
                    helptext = description = "Misfire maps y size";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Misfire_map_x_size":
                    helptext = description = "Misfire maps x size";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Lam_rpmsteg":
                    helptext = description = "Lambda control table rpm steps";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Lam_laststeg":
                    helptext = description = "Lambda control table load steps";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Knock_wind_rpm":
                    helptext = description = "Knock_wind table rpm axis";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Fuel_map_yaxis":
                    helptext = description = "Fuel correction table y axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Fuel_map_xaxis":
                    helptext = description = "Fuel correction table x axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Inj_map_0_y_axis":
                    helptext = description = "LOLA Fuel correction table y axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Injector_flow":
                    helptext = description = "Injector flow constant, for T5.2 only";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Inj_map_0_x_axis":
                    helptext = description = "LOLA Fuel correction table x axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Fuel_knock_xaxis":
                    helptext = description = "Fuel knock correction table x axis";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Dash_rpm_axis":
                    helptext = description = "Dash pot tabel rpm axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Dash_trot_axis":
                    helptext = description = "Dash pot tabel rpm axis";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Detect_map_y_size":
                    helptext = description = "Detect map y size";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Detect_map_x_size":
                    helptext = description = "Detect map x size";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Dash_act_step":
                    helptext = description = "Dash act step";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Last_reg_kon":
                    helptext = description = "Current reg_kon_mat values, support points are [1] = Reg_kon_mat value, [2] = P-gain value, [3] = I-gain value, [4] = D-gain value";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Last_temp_st":
                    helptext = description = "Nominal load calculation maps support points";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Last_varv_st":
                    helptext = description = "Nominal load calculation maps support points";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Last_reg_ac":
                    helptext = description = "Value for calculation of nominal load. Function of AC.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_last_temp":
                    helptext = description = "Value for calculation of nominal load. Function of coolant water temperature.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_last_rpm":
                    helptext = description = "Value for calculation of nominal load. Function of rpm.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "n_Engine_delta_180":
                    helptext = description = "Rpm delta";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "AC_wait_sp":
                    helptext = description = "AC wait time table support points";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "AC_start_tid":
                    helptext = description = "AC starttime";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_start":
                    helptext = description = "Idle start variable";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_drift_tab":
                    helptext = description = "Drive/Neutral value";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ret_up_rpm":
                    helptext = description = "RPM drop enrichment, upper rpm limit for function";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_down_rpm":
                    helptext = description = "RPM drop enrichment, lower rpm limit for function";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_delta_rpm":
                    helptext = description = "RPM drop enrichment, delta rpm for triggering";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_fuel_tab":
                    helptext = description = "RPM drop enrichment, value for enrichment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_fuel_time":
                    helptext = description = "RPM drop enrichment, constant enrichment time (ms)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_fuel_step":
                    helptext = description = "RPM drop enrichment, amount of decrement/combustion";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_fuel_fak":
                    helptext = description = "RPM drop enrichment, current value";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "After_fcut_tab":
                    helptext = description = "After fuelcut enrichment (function of number of combustions after fuelcut)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;

                case "Idle_ac_tab":
                    helptext = description = "Drive with AC, function of airtemp. Extra step for idle valve position when AC is turned on.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_temp_off":
                    helptext = description = "Additional factor to valveposition. Function of cooling water temperature)";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_tab":
                    helptext = description = "Increment of valve position during start. Function of cooling water temperature)";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_rpm_tab":
                    helptext = description = "Nominal rpm depending on cooling water temperature";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_der_tab":
                    helptext = description = "(D) Above nominal enginespeed when rpm is decreasing";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_p_tab":
                    helptext = description = "(P) Decrement value over nominal rpm";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_cert_tid":
                    helptext = description = symbolname;
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_decrese_asp":
                    helptext = description = symbolname;
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Frek_230":
                    description = "Apc control frequency below 2500 rpm";
                    helptext = "Apc control frequency below 2500 rpm. Default is 90 Hz, for T7 valve this would be 50 Hz.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Frek_250":
                    description = "Apc control frequency above 2500 rpm";
                    helptext = "Apc control frequency above 2500 rpm. Default is 70 Hz, for T7 valve this would be 32 Hz.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_frek":
                    helptext = description = "Apc control frequency";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_fullgas":
                    helptext = description = symbolname;
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_knock_tab":
                    helptext = description = "Step increment to reduce boost when knock occurs. In 0.01 bar resolution.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_konst_tid":
                    helptext = description = symbolname;
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;

                case "P_man_korr_tab":
                    helptext = description = "Pressure correction vs. throttle";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "LhomePManifold":
                    helptext = description = "Limphome pressure";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Limp_home;
                    break;
                case "LhomeMapPman":
                    helptext = description = "Limphome map for pressure in manifold (P_manifold)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Limp_home;
                    break;
                case "LhomeYaxisPman":
                    helptext = description = "RPM values for Limphome pressure map";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "LhomeXaxisPman":
                    helptext = description = "Throttle values for Limphome pressure map";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Comb_w_temp_limbhome":
                    helptext = description = "Number of combustions before increasing limphome value";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Limp_home;
                    break;
                case "Sond_heat_tab":
                    helptext = description = "Load limit for lambda sensor heating";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Temp_max_regl":
                    helptext = description = "Boost temperature limits";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_regl_sp":
                    helptext = description = "Maximum boost pressure limit maps support points";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Max_regl_temp_1":
                    helptext = description = "Maximum boost pressure to set above temperature limit #1";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_regl_temp_2":
                    helptext = description = "Maximum boost pressure to set above temperature limit #2";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Overstid_tab":
                    helptext = description = "Overboost time";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Gear_ratio_delta":
                    helptext = description = "Car speed calculation, gear ratio delta";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Gear_st":
                    helptext = description = "Car speed calculation, gear stages";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Fload_tab":
                    helptext = description = "Full load map for full load enrichment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Fload_throt_tab":
                    helptext = description = "Full load throttle map for full load enrichment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Derivata_fuel_rpm":
                    helptext = description = "Rpm limit for fuel adjustment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Derivata_br_sp":
                    helptext = description = "Fuel adjustment map support points";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Derivata_br_tab_neg":
                    helptext = description = "Fuel adjustment, negative rpm delta";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Derivata_br_tab_pos":
                    helptext = description = "Fuel adjustment, positive rpm delta";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_tryck":
                    helptext = description = "Increase of valve position during start. Function of AD value pressure sensor.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Br_minus_tab":
                    helptext = description = "Decrease of fuel during idle when rpm < rpm nominal. Resolution is uS";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Br_plus_tab":
                    helptext = description = "Increase of fuel during idle when rpm < rpm nominal. Resolution is uS";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "AdjustLamCal":
                    helptext = description = "Lean and rich step adjustment depending on rear lambda sensor";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lam_konst_mager":
                    helptext = description = "Lean lambda constant";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lam_konst_fet":
                    helptext = description = "Rich lambda constant";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Konst_lean_map":
                    helptext = description = "Lean constant lambdaintegrator, combustions";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Konst_rich_map":
                    helptext = description = "Rich constant lambdaintegrator, combustions";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lam_rpm_sp":
                    helptext = description = "Lambda rpm support points";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Lam_load_sp":
                    helptext = description = "Lambda load support points";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Lean_step_map":
                    helptext = description = "Lean step for lambda integrator";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Rich_step_map":
                    helptext = description = "Rich step for lambda integrator";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lam_minlast":
                    helptext = description = "Lambda minimal load";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lamb_ej":
                    helptext = description = "Lambda no, temperature limit for open throttle";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lamb_tid":
                    helptext = description = "Lambda time (number of combustions)";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lamb_idle":
                    helptext = description = "Lambda idle, temperature limit for closed throttle";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lamb_kyl":
                    helptext = description = "Lambda map support points for coolant temperature";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Throt_after_tab":
                    helptext = description = "Acceleration after start factor (MY1995)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Throt_aft_dec_fak":
                    helptext = description = "Acceleration after deceleration factor (MY1995)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Throt_aft_fak":
                    helptext = description = "After acceleration factor (MY1995)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Throt_aft_dec":
                    helptext = description = "Acceleration after deceleration factor (MY1995)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Tempkomp_konst":
                    helptext = description = "Temperature compensation";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Hp_support_points":
                    helptext = description = "Restart factor for heatplates support points";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Restart_corr_hp":
                    helptext = description = "Restart factor for heatplates";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Restart_temp":
                    helptext = description = "Restart temperature";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Restart_tab":
                    helptext = description = "Restart factor (1 - 15, 15 - 30 and 30 - 45 seconds)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "CwIgnOffset":
                case "CwIgnCombCounter":
                case "CwIgnFuncFinished":
                    helptext = description = "Variables for cooling water ignition offset";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "CwIgnCal":
                    helptext = description = "Constants for cooling water ignition offset";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rev_grad_sens_dur_req":
                    helptext = description = "Rev. gradient sensitivity during request";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rev_grad_sens_cont":
                    helptext = description = "Rev. gradient sensitivity continous";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ramp_up_ram":
                    helptext = description = "Ramp up table";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ramp_down_ram":
                    helptext = description = "Ramp down table";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Run_hours":
                    helptext = "Hour counter. Starts at system powerup. Resets at system disconnection.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    description = "Runtime hour counter (system uptime)";
                    break;
                case "Run_minutes":
                    helptext = "Minute counter. Starts at system powerup. Resets at system disconnection.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    description = "Runtime minute counter (system uptime)";
                    break;
                case "Run_seconds":
                    helptext = "Second counter. Starts at system powerup. Resets at system disconnection.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    description = "Runtime minute counter (system uptime)";
                    break;
                case "Battery_time":
                    helptext = "Time when battery voltage last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Last battery ok time";
                    break;
                case "Diag_tid":
                    helptext = "Second counter. Ranges from 0 to 2^32. Starts at ignition key on. Resets at ignition key off.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Second counter during ignition on.";
                    break;
                case "Hast_tid_grans":
                    helptext = "Time when vehicle speed low detection last was ok. Stops counting when vehicle speed has been to low compared to what should be expected according to the load, RPM and brake pedal conditions.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Vehicle low speed detection ok time.";
                    break;
                case "I_front_time":
                    helptext = "Time when preheating of front lambdasensor last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    description = "Front lambda ok time.";
                    break;
                case "I_rear_time":
                    helptext = "Time when preheating of rear lambdasensor last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    description = "Rear lambda ok time.";
                    break;
                case "Ign_counter":
                    helptext = "Counts number of ignitions after start. Increments for each combustion. Resets below 750 rpm.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Ignition counter.";
                    break;
                case "Knock_time_low":
                    helptext = "Time when knock detection reading last was ok. ";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Basic;
                    description = "Last knock detection ok.";
                    break;
                case "Kyl_tid_end":
                    helptext = "Time when coolant water sensor end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Last time water temperature sensing ok";
                    break;
                case "Luft_tid_end":
                    helptext = "Time when air sensor end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Last time air temperature sensing ok";
                    break;
                case "Purge_time":
                    helptext = "Time when purge reading last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    description = "Time when purge reading last was ok";
                    break;
                case "Rsond_tid_end":
                    helptext = "Time when rear lambda sensor end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    description = "Time when rear lambda sensor end position detection last was ok";
                    break;
                case "Sond_tid_end":
                    helptext = "Time when front lambda sensor high end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    description = "Time when front lambda sensor high end position detection last was ok.";
                    break;
                case "Sond_tid_konst":
                    helptext = "Time when front lambda sensor low end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    description = "Time when front lambda sensor low end position detection last was ok.";
                    break;
                case "Tid_kylv_ok":
                    helptext = "Time vehicle speed is above 20km/h for coolingwater sensor low activity detection.";
                    description = "Time vehicle speed is above 20km/h for coolingwater sensor low activity detection.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Luft_tid_konst":
                    helptext = "Time when constant reading from IAT sensor was last ok.";
                    description = "Time when constant reading from IAT sensor was last ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Trott_tid_konst":
                    helptext = "Time when constant reading from throttle position sensor was last ok.";
                    description = "Time when constant reading from throttle position sensor was last ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Trott_tid_end":
                    helptext = "Time when throttle end position detection last was ok.";
                    description = "Time when throttle end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Tryck_tid_end":
                    helptext = "Time when pressure sensor end position detection last was ok.";
                    description = "Time when pressure sensor end position detection last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Tryck_tid_konst":
                    helptext = "Time when constant reading from pressure sensor last was ok.";
                    description = "Time when constant reading from pressure sensor last was ok.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Adapt_injfaktor":
                    helptext = "Long term fuel adaption factor. Using point adaption to calculate a global adaption factor at every system stop.";
                    description = "Long term fuel adaption factor.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rpm":
                    helptext = "Current engine speed. Updates every 180 degrees engine revolution. Resolution is 10 rpm.";
                    description = "Current engine speed.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Trot_min":
                    helptext = "Adaptiv throttle offset. AD_trot-Trot_min => Relative throttle reading.";
                    description = "Adaptiv throttle offset. AD_trot-Trot_min => Relative throttle reading.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Diag_AD_trot":
                    helptext = "A/D reading of throttle sensor.";
                    description = "A/D reading of throttle sensor.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Lufttemp":
                    helptext = "Current inlet manifold air temperature.";
                    description = "Current inlet manifold air temperature.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Kyl_temp":
                    helptext = "Current cooling water temp.";
                    description = "Current cooling water temp.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Start_kylv":
                    helptext = "Stored cooling water temp at start. Used to detect low activity on cooling water sensor.";
                    description = "Stored cooling water temp at start. Used to detect low activity on cooling water sensor.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Start_tryck":
                    helptext = "Absolut airpressure read at start before cranking.";
                    description = "Absolut airpressure read at start before cranking.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Medellast":
                    helptext = "Average calculated load.";
                    description = "Average calculated load.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Lambdaint":
                    helptext = "Current lambdaintegrator value.";
                    description = "Current lambdaintegrator value.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Diag_U_lmbda_eng":
                    helptext = "Analog reading of lambda probe before catalyst in mV.";
                    description = "Analog reading of lambda probe before catalyst in mV.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "I_lambda_Eng":
                    helptext = "Current of front lambda probe preheating in mA.";
                    description = "Current of front lambda probe preheating in mA.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Diag_U_lmbda_cat":
                    helptext = "Analog reading of lambda probe after catalyst in mV.";
                    description = "Analog reading of lambda probe after catalyst in mV.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "I_lambda_Cat":
                    helptext = "Current of rear lambda probe preheating in mA.";
                    description = "Current of rear lambda probe preheating in mA.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Rich_adaption":
                    helptext = "Front lambda sensor switch point rich adaption using rear lambda sensor cloosed loop.";
                    description = "Front lambda sensor switch point rich adaption using rear lambda sensor cloosed loop.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lean_adaption":
                    helptext = "Front lambda sensor switch point lean adaption using rear lambda sensor cloosed loop.";
                    description = "Front lambda sensor switch point lean adaption using rear lambda sensor cloosed loop.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Sond_omsl_cntr":
                    helptext = "Sonde switching point. Counts number of front oxygen sensor transitions during a test cycle.";
                    description = "Counts number of front oxygen sensor transitions during a test cycle.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                /*                case "Sond_ign_counter":
                                    helptext = "Sonde ignition counter. Counts number of ignitions during a calibratible number of front oxygen sensor transitions.";
                                    description = "Sonde ignition counter. Counts number of ignitions during a calibratible number of front oxygen sensor transitions.";
                                    category = XDFCategories.Sensor;
                                    subcategory = XDFSubCategory.Basic;
                                    break;*/
                case "Ox2_act_time":
                    helptext = "Oxygen activity timer. Time counter for oxygen sensor 2 activity check.";
                    description = "Oxygen activity timer. Time counter for oxygen sensor 2 activity check.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "LLS_lage":
                    helptext = "Current idle valve position without temperature compensation.";
                    description = "Current idle valve position without temperature compensation.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Bil_hast":
                    helptext = "Current vehicle speed.";
                    description = "Current vehicle speed.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Old_hast":
                    helptext = "Stores last reading of Bil_hast. Update drequency is 1 second. Used to determine abnormal vehicle speed changes. (Bil_hast - Old_hast = Speed difference).";
                    description = "Stores last reading of Bil_hast. Update drequency is 1 second. Used to determine abnormal vehicle speed changes. (Bil_hast - Old_hast = Speed difference).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "PWM_ut":
                    helptext = "Pulse Witdh Modulation (PWM) signal to turbo boost control valve (in percentages).";
                    description = "Pulse Witdh Modulation (PWM) signal to turbo boost control valve (in percentages).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cat_avg_dev_diff":
                    helptext = "Catalyst monitor evaluation result.";
                    description = "Catalyst monitor evaluation result.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Batt_volt":
                    helptext = "Battery voltage (*10).";
                    description = "Battery voltage (*10).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Knock_diag_level":
                    helptext = "Current knock diagnostic signal.";
                    description = "Current knock diagnostic signal.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Mis200_tot":
                    helptext = "Misfire catalyst overheating counter for all cylinders.";
                    description = "Misfire catalyst overheating counter for all cylinders.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis200_1":
                    helptext = "Misfire catalyst overheating counter for cylinder 1.";
                    description = "Misfire catalyst overheating counter for cylinder 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis200_2":
                    helptext = "Misfire catalyst overheating counter for cylinder 2.";
                    description = "Misfire catalyst overheating counter for cylinder 2.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis200_3":
                    helptext = "Misfire catalyst overheating counter for cylinder 3.";
                    description = "Misfire catalyst overheating counter for cylinder 3.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis200_4":
                    helptext = "Misfire catalyst overheating counter for cylinder 4.";
                    description = "Misfire catalyst overheating counter for cylinder 4.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_tot":
                    helptext = "Misfire emission level degradation counter for all cylinders.";
                    description = "Misfire emission level degradation counter for all cylinders.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_1":
                    helptext = "Misfire emission level degradation counter for cylinder 1.";
                    description = "Misfire emission level degradation counter for cylinder 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_2":
                    helptext = "Misfire emission level degradation counter for cylinder 2.";
                    description = "Misfire emission level degradation counter for cylinder 2.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_3":
                    helptext = "Misfire emission level degradation counter for cylinder 3.";
                    description = "Misfire emission level degradation counter for cylinder 3.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_4":
                    helptext = "Misfire emission level degradation counter for cylinder 4.";
                    description = "Misfire emission level degradation counter for cylinder 4.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "PurgeLambdaDiff":
                    helptext = "Maximal lambda difference during purge flow test.";
                    description = "Maximal lambda difference during purge flow test.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PurgeIdlePosDiff":
                    helptext = "Maximal idle position difference during purge flow test.";
                    description = "Maximal idle position difference during purge flow test.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[0]":
                    helptext = "Current program status flags (byte 1).";
                    description = "Current program status flags (byte 1).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[1]":
                    helptext = "Current program status flags (byte 2).";
                    description = "Current program status flags (byte 2).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[2]":
                    helptext = "Current program status flags (byte 3).";
                    description = "Current program status flags (byte 3).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[3]":
                    helptext = "Current program status flags (byte 4).";
                    description = "Current program status flags (byte 4).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[4]":
                    helptext = "Current program status flags (byte 5).";
                    description = "Current program status flags (byte 5).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pgm_status[5]":
                    helptext = "Current program status flags (byte 6).";
                    description = "Current program status flags (byte 6).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ready":
                    helptext = "Flag register indicating evaluation ready codes for one trip. Holds flag for trip, warm-up-cycle and driving cycle.";
                    description = "Flag register indicating evaluation ready codes for one trip.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "ErrorCode":
                    helptext = "Corresponding error code for the current error (decimal).";
                    description = "Corresponding error code for the current error (decimal).";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Low_fuel":
                    helptext = "Indicates if there was a low fuel level when error was detected.";
                    description = "Indicates if there was a low fuel level when error was detected.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Tryck_vakt_tab":
                    helptext = "Boost limit map (fuel cut). This map contains one value that determines the maximum allowable turbo boost pressure. Any boost pressure above this value will result in a fuel cut (fuel supply will be cut off to prevent engine damage)";
                    description = "Boost limit map (fuel cut)";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ign_offset":
                    description = "Global ignition offset, added to calculated ignition angle.";
                    helptext = "Global ignition offset, added to calculated ignition angle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ign_offset_adapt":
                    description = "General ignition offset adaption.";
                    helptext = "General ignition offset adaption.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_1":
                    description = "Ignition offset for cylinder 1.";
                    helptext = "Ignition offset for cylinder 1.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_2":
                    description = "Ignition offset for cylinder 2.";
                    helptext = "Ignition offset for cylinder 2.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_3":
                    description = "Ignition offset for cylinder 3.";
                    helptext = "Ignition offset for cylinder 3.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_4":
                    description = "Ignition offset for cylinder 4.";
                    helptext = "Ignition offset for cylinder 4.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "ign_idle_angle":
                case "Ign_idle_angle":
                    description = "Target ignition advance for idle state.";
                    helptext = "Target ignition advance for idle state.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Idle;
                    break;
                /*case "Adapt_injfaktor":
                    description = "Injection map global adaption.";
                    helptext = "Injection map global adaption.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;*/
                case "Adapt_inj_imat":
                    description = "Idle Injection map adaption.";
                    helptext = "Idle Injection map adaption.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Min_load_gadapt":
                    description = "Minimum Load global fuel injection adaption.";
                    helptext = "Minimum Load for the area where global fuel injection adaption occurs.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_load_gadapt":
                    description = "Maximum Load global fuel injection adaption.";
                    helptext = "Maximum Load for the area where global fuel injection adaption occurs.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Min_rpm_gadapt":
                    description = "Minimum RPM global fuel injection adaption.";
                    helptext = "Minimum RPM for the area where global fuel injection adaption occurs.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_rpm_gadapt":
                    description = "Maximum RPM global fuel injection adaption.";
                    helptext = "Maximum RPM for the area where global fuel injection adaption occurs.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Tid_konst":
                    description = "When program goes with constant injection time during idle, the actual injection time is taken from this adress.";
                    helptext = "When program goes with constant injection time during idle, the actual injection time is taken from this adress.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "min_tid":
                    description = "Fuel injection minimum time.";
                    helptext = "Fuel injection minimum time. After injection time calculation the final time is compared to this value and this is used if below.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Eft_dec_fak":
                    description = "Number of half enginerev. between a unit of decrease for Eft_fak1. Number of half enginerev is temperature dependent.";
                    helptext = "Number of half enginerev. between a unit of decrease for Eft_fak1. Number of half enginerev is temperature dependent.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_dec_fak2":
                    description = "Number of half enginerev. between a unit of decrease for Eft_fak2. Number of half enginerev is temperature dependent.";
                    helptext = "Number of half enginerev. between a unit of decrease for Eft_fak2. Number of half enginerev is temperature dependent.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Radius_of_roll":
                    description = "Front wheel tyre radius of roll.";
                    helptext = "Front wheel tyre radius of roll in millimeters.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Max_vehicle_speed":
                    description = "Maximum vehicle speed.";
                    helptext = "Maximum vehicle speed in km/h (speed limiter).";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Reg_kon_mat":
                    description = "Boost Regulation Map, BCV Constant (manual).";
                    helptext = "Boost Regulation Map, BCV Constant for manual transmission. This table of constants (per RPM range) indicates what the preset level on the Boost Control Valve will be.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "Reg_kon_mat_a":
                    description = "Boost Regulation Map, BCV Constant (automatic).";
                    helptext = "Boost Regulation Map, BCV Constant for automatic transmission. This table of constants (per RPM range) indicates what the preset level on the Boost Control Valve will be.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Data_namn":
                    description = "Software version";
                    helptext = "Software version. This is a string value that indicates which software version the binary contains.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Inj_konst":
                    description = "Injector scaling (adjust for different injectors)";
                    helptext = "Injector scaling value. If you install larger injectors you need to enter a smaller value for this constant. If the percentage of difference in injectors is larger than ~30% you'll need to consider that larger injectors need longer to open and linear scaling of this value is not enough.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Batt_korr_tab":
                    description = "Correction on injectiontime depending on battery voltage";
                    helptext = "Correction on injectiontime depending on battery voltage. This table is used to correct PWM times for fuel injection.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ad_0":
                case "Ad_1":
                case "Ad_2":
                case "Ad_3":
                case "Ad_4":
                case "Ad_5":
                case "Ad_6":
                case "Ad_7":
                    description = "Analogue to digital channel";
                    helptext = "Analogue to digital channel";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Da_0":
                case "Da_1":
                case "Da_2":
                case "Da_3":
                case "Da_4":
                case "Da_5":
                case "Da_6":
                case "Da_7":
                    description = "Digital to analogue channel";
                    helptext = "Digital to analogue channel";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_tryck":
                    description = "Present value from APC pressure map";
                    helptext = "Present value from APC pressure map";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rpm_max":
                    description = "Maximum Revolutions Per Minute (RPM limiter)";
                    helptext = "Maximum Revolutions Per Minute (RPM limiter). This value determines at what RPM the limiter intervenes. Normally it is set to ~6200 RPM for a 2.3 liter engine and just a bit higher for a 2.0 liter engine.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Limp_tryck_konst":
                    description = "Maximum boost in limp-home mode";
                    helptext = "Maximum boost in limp-home mode. This values indicates what the maximum turbo boost level is when the car is running in 'Limp home mode'. Limp home is a modus that is activated when the Trionic senses there is something significant wrong in the engine and to prevent further damage the car is set back in boost limit to prevent it from making too much power and torque that could further complicate things.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_map_0":
                    description = "Main ignition map for warm engine (0)";
                    helptext = "Ignition map 0. This is the main ignition map for a warm engine. It shows the degrees BTDC that the spark should fire.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ign_map_1":
                    description = "Ignition map for idle correction (1)";
                    helptext = "Ignition map 1. Ignition map for idle correction from target idle ignition advance depending on RPM difference from target idle RPM.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Ign_map_2":
                    description = "Ignition map for knocking correction (2)";
                    helptext = "Ignition map 2. Ignition correction when knocking.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_map_3":
                    description = "Transient ignition map (3)";
                    helptext = "Ignition map 3. Transient ignition map.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_map_4":
                    description = "Ignition map for warmup (cool engine) (4)";
                    helptext = "Ignition map 4. Warmup ignition map (cool engine).";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ign_map_5":
                    description = "Ignition map for stalling engine (5)";
                    helptext = "Ignition map 5. Stall ignition map (stalling engine).";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Ign_map_6":
                    description = "Ignition map for torque reduction on upshift (6)";
                    helptext = "Ignition map 6. Torque reduction on upshift ignition map.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_map_7":
                    description = "Ignition map for torque reduction on downshift (7)";
                    helptext = "Ignition map 7. Torque reduction on downshift ignition map.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_map_8":
                    description = "Ignition map for idle ignition compensation during speed (8)";
                    helptext = "Ignition map 8. Idle ignition compensation during speed ignition map.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Ign_map_0_x_size":
                    description = "Ignition map 0 x-axis size";
                    helptext = "Ignition map 0 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_1_x_size":
                    description = "Ignition map 1 x-axis size";
                    helptext = "Ignition map 1 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_2_x_size":
                    description = "Ignition map 2 x-axis size";
                    helptext = "Ignition map 2 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_3_x_size":
                    description = "Ignition map 3 x-axis size";
                    helptext = "Ignition map 3 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_4_x_size":
                    description = "Ignition map 4 x-axis size";
                    helptext = "Ignition map 4 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_5_x_size":
                    description = "Ignition map 5 x-axis size";
                    helptext = "Ignition map 5 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_6_x_size":
                    description = "Ignition map 6 x-axis size";
                    helptext = "Ignition map 6 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_7_x_size":
                    description = "Ignition map 7 x-axis size";
                    helptext = "Ignition map 7 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_8_x_size":
                    description = "Ignition map 8 x-axis size";
                    helptext = "Ignition map 8 x-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_0_y_size":
                    description = "Ignition map 0 y-axis size";
                    helptext = "Ignition map 0 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_1_y_size":
                    description = "Ignition map 1 y-axis size";
                    helptext = "Ignition map 1 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_2_y_size":
                    description = "Ignition map 2 y-axis size";
                    helptext = "Ignition map 2 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_3_y_size":
                    description = "Ignition map 3 y-axis size";
                    helptext = "Ignition map 3 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_4_y_size":
                    description = "Ignition map 4 y-axis size";
                    helptext = "Ignition map 4 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_5_y_size":
                    description = "Ignition map 5 y-axis size";
                    helptext = "Ignition map 5 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_6_y_size":
                    description = "Ignition map 6 y-axis size";
                    helptext = "Ignition map 6 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_7_y_size":
                    description = "Ignition map 7 y-axis size";
                    helptext = "Ignition map 7 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_8_y_size":
                    description = "Ignition map 8 y-axis size";
                    helptext = "Ignition map 8 y-axis size";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_0_x_axis":
                    description = "Ignition map 0 x-axis, pressure from MAP sensor";
                    helptext = "Ignition map 0 x-axis, pressure from MAP sensor";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_1_x_axis":
                    description = "Ignition map 1 x-axis.";
                    helptext = "Ignition map 1 x-axis.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_2_x_axis":
                    description = "Ignition map 2 x-axis, pressure from MAP sensor";
                    helptext = "Ignition map 2 x-axis, pressure from MAP sensor";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_3_x_axis":
                    description = "Ignition map 3 x-axis, cooling water AD";
                    helptext = "Ignition map 3 x-axis, cooling water AD";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_4_x_axis":
                    description = "Ignition map 4 x-axis";
                    helptext = "Ignition map 4 x-axis";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_5_x_axis":
                    description = "Ignition map 5 x-axis";
                    helptext = "Ignition map 5 x-axis";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_6_x_axis":
                    description = "Ignition map 6 x-axis, pressure from MAP sensor";
                    helptext = "Ignition map 6 x-axis, pressure from MAP sensor";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_7_x_axis":
                    description = "Ignition map 7 x-axis, pressure from MAP sensor";
                    helptext = "Ignition map 7 x-axis, pressure from MAP sensor";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_8_x_axis":
                    description = "Ignition map 8 x-axis";
                    helptext = "Ignition map 8 x-axis";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_0_y_axis":
                    description = "Ignition map 0 y-axis, RPM";
                    helptext = "Ignition map 0 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_1_y_axis":
                    description = "Ignition map 1 y-axis, RPM deviation";
                    helptext = "Ignition map 1 y-axis, RPM deviation";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_2_y_axis":
                    description = "Ignition map 2 y-axis, RPM";
                    helptext = "Ignition map 2 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_3_y_axis":
                    description = "Ignition map 3 y-axis, RPM";
                    helptext = "Ignition map 3 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_4_y_axis":
                    description = "Ignition map 4 y-axis";
                    helptext = "Ignition map 4 y-axis";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_5_y_axis":
                    description = "Ignition map 5 y-axis, RPM";
                    helptext = "Ignition map 5 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_6_y_axis":
                    description = "Ignition map 6 y-axis, RPM";
                    helptext = "Ignition map 6 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_7_y_axis":
                    description = "Ignition map 7 y-axis, RPM";
                    helptext = "Ignition map 7 y-axis, RPM";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Ign_map_8_y_axis":
                    description = "Ignition map 8 y-axis, RPM delta";
                    helptext = "Ignition map 8 y-axis, RPM delta";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Knock_count_cyl1":
                    description = "Number of knocks detected in cylinder 1";
                    helptext = "Number of knocks detected in cylinder 1";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_count_cyl2":
                    description = "Number of knocks detected in cylinder 2";
                    helptext = "Number of knocks detected in cylinder 2";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_count_cyl3":
                    description = "Number of knocks detected in cylinder 3";
                    helptext = "Number of knocks detected in cylinder 3";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_count_cyl4":
                    description = "Number of knocks detected in cylinder 4";
                    helptext = "Number of knocks detected in cylinder 4";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Insp_mat":
                    description = "Volumetric Efficiencey table (RPM x MAP)";
                    helptext = "Volumetric Efficiencey table (RPM x MAP). This table contains engine specific values for correction of the injection duration. When VE is high (high efficiency, so only little air escapes from the combustion chamber) this correction value will be high. When VE is lower (e.g. more air escapes from the chamber, either backwards into the air inlet of forward into the exhaust system, the correction factor will be lower, less fuel has to be injected in this case. Despite its name, there is more to adjusting this map than just reflecting the actual VE of the engine. While an AFR of 14.3-14.7:1 is desirable for maximum fuel efficiency and minimum emissions at idle and normal cruising under low loads, more fuel is needed to keep cylinder temperatures lower and detonation in check under boost and this needs to be reflected in the VE map";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Inj_map_0":
                    description = "Volumetric Efficiencey table (RPM x TPS) for LOLA";
                    helptext = "Volumetric Efficiencey table (RPM x TPS). This table contains engine specific values for correction of the injection duration. When VE is high (high efficiency, so only little air escapes from the combustion chamber) this correction value will be high. When VE is lower (e.g. more air escapes from the chamber, either backwards into the air inlet of forward into the exhaust system, the correction factor will be lower, less fuel has to be injected in this case. Despite its name, there is more to adjusting this map than just reflecting the actual VE of the engine. While an AFR of 14.3-14.7:1 is desirable for maximum fuel efficiency and minimum emissions at idle and normal cruising under low loads, more fuel is needed to keep cylinder temperatures lower and detonation in check under boost and this needs to be reflected in the VE map";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Adapt_ref":
                    description = "Adaption reference table (T5.2)";
                    helptext = "Adaption reference table (T5.2)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Del_mat":
                    description = "Delay map (crankshaft degrees)";
                    helptext = "Delay map (crankshaft degrees)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Tryck_mat_a":
                    description = "Boost table for automatic transmission";
                    helptext = "Boost table for automatic transmission. This table shows the boost REQUEST values that the Trionic tries to reach at a certain RPM and throttle position. Using several other sensors and values trionic aims for this boost pressure. Whether or not and how fast this target boost pressure is reached depends on a lot of factors like hardware (turbo, intercooler, injectors used) and control parameters (Reg_kon_mat, PID control etc).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_automatic;
                    break;
                case "Tryck_mat":
                    description = "Boost table for manual transmission";
                    helptext = "Boost table for manual transmission. This table shows the boost REQUEST values that the Trionic tries to reach at a certain RPM and throttle position. Using several other sensors and values trionic aims for this boost pressure. Whether or not and how fast this target boost pressure is reached depends on a lot of factors like hardware (turbo, intercooler, injectors used) and control parameters (Reg_kon_mat, PID control etc).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_manual;
                    break;
                case "Reg_last":
                    description = "Boost regulation load axis values.";
                    helptext = "Boost regulation load axis values. Represents engine load ranges to select PID values on (x axis values for PID tables).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Reg_varv":
                    description = "Boost regulation RPM axis values.";
                    helptext = "Boost regulation RPM axis values. Represents engine RPM ranges to select PID values on (y axis values for PID tables).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_st_last":
                    description = "Injection on idle load axis values.";
                    helptext = "Injection on idle load axis values. Represents engine load ranges to select idle injection correction on.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Idle_st_rpm":
                    description = "Injection on idle RPM axis values.";
                    helptext = "Injection on idle RPM axis values. Represents engine RPM ranges to select idle injection correction on.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Cyl_komp":
                    description = "Limit for cylinder detection low part Fuel compensation factor due to differences between cylinders.";
                    helptext = "Limit for cylinder detection low part Fuel compensation factor due to differences between cylinders.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Eftersta_fak":
                    description = "Enrichmentfactor for coldstart #1. Factor is depending on coolantwater temperature.";
                    helptext = "Enrichmentfactor for coldstart #1. Factor is depending on coolantwater temperature.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eftersta_fak2":
                    description = "Enrichmentfactor for coldstart #2. Factor is depending on coolantwater temperature.";
                    helptext = "Enrichmentfactor for coldstart #2. Factor is depending on coolantwater temperature.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Before_start":
                    description = "Parallel injection on all cylinders before engine start to aid in starting a cold engine.";
                    helptext = "Parallel injection on all cylinders. Triggered on first detected tooth from crank sensor. The following conditions disables before start. -Last start attempt was not successful. i.e. Motorn_startat flag not set) -Restart after 0 to 45 sec. and cooling water temperature below BS_temp.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Lret_konst":
                    description = "Fuel Injection Map, Deceleration, Load Delta.";
                    helptext = "Fuel Injection Map, Deceleration, Load Delta.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Transient;
                    break;
                case "Lacc_konst":
                    description = "Fuel Injection Map, Acceleration, Load Delta.";
                    helptext = "Fuel Injection Map, Acceleration, Load Delta.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Transient;
                    break;
                case "Dash_tab":
                    description = "Dash pot position, should affect the auxiliary air valve to retard the engines RPM from around 3000 RPM to idle state in about four seconds. .";
                    helptext = "The dash pot is a mechanichal retardation timer in respect of lowering the engines RPM from 3000 RPM to idle (~850 RPM) in 4 seconds ±1 second. At 2500 - 2700 RPM the throttle lever should touch the dash pot's piston and from there prolong the engines RPM retardation. Therefore the X axis is the relative throttle value, how much the throttle is moving. The Y axis is the RPM and the value in the cells should affect the auxiliary air valve to retard the engines RPM from around 3000 RPM to idle state in about four seconds. This applies to the carburated 99 as well.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Accel_konst":
                    description = "Fuel Injection Map, Acceleration, Throttle Position.";
                    helptext = "Fuel Injection Map, Acceleration, Throttle Position.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Transient;
                    break;
                case "Retard_konst":
                    description = "Fuel Injection Map, Deceleration, Throttle Position.";
                    helptext = "Fuel Injection Map, Deceleration, Throttle Position.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Transient;
                    break;
                case "Pwm_ind_trot":
                    description = "Relative throttle value";
                    helptext = "Relative throttle value";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Fuel_knock_mat":
                    description = "Fuel enrichment table when knocking occurs";
                    helptext = "Fuel enrichment table when knocking occurs. When the Trionic detects knocking (very advanced mechanism by sensing ionization between spark plug gap) more fuel is injected to cool down the charges in the combustion chamber. This table holds the values for this knocking enrichment.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Iv_min_tab_ac":
                    description = "Idle valve min table ac.";
                    helptext = "Idle valve min table ac.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Iv_start_time_tab":
                    description = helptext = "Time for higher idle speed after start";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Iv_start_pos_offset":
                    description = helptext = "Idle valve after start position offset";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Iv_start_rpm_offset":
                    description = helptext = "Idle valve after start rpm offset";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Iv_min_tab":
                    description = "Idle valve min table.";
                    helptext = "Min position value for idlevalve when normal idle control not are activated. 1 Byte 0 - 125 rpm, 2 byte 126 - 250 rpm, 3 byte 251 - 375 rpm .... 54 byte 6625 - 6750 rpm.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Lambdamatris_diag":
                    description = "Lambda sonde diagnostics control factors";
                    helptext = "Lambda sonde diagnostics control factors";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lambdamatris":
                    description = "Lambda sonde control factors";
                    helptext = "Lambda sonde control factors";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Idle_fuel_korr":
                    description = "Idle fuel correction map";
                    helptext = "Idle fuel correction map. This table holds the values for keeping the engine running smoothly around idle RPM. The engine needs constant fuel correction when the RPM fluctuates around ~850 RPM.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Open_loop_adapt":
                    description = "Open loop adaption, not used";
                    helptext = "Open loop adaption, not used";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Open_loop_time":
                case "Open_loop_msec":
                    description = helptext = "Open loop time.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Open_loop":
                    description = "Load limit where system switches to open loop. Function of rpm.";
                    helptext = "Load limit where system switches to open loop. Function of rpm.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Open_loop_knock":
                    description = "Load limit where system switches to open loop during knocking conditions. Function of rpm.";
                    helptext = "Load limit where system switches to open loop during knocking conditions. Function of rpm.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "P_fors":
                    description = "Boost Regulation Map, P-Gain factor of PID control";
                    helptext = "Boost Regulation Map, P-Gain factor of PID control. The P value controls how much the PID output instantly changes in response to a difference between the target and the feedback (the error). For example, if your idle speed dropped to 700 rpm and the target was 800 rpm, then the PID output might jump from the neutral point of 40% to a new value of 50%. By doubling the P value, then the jump in PID output would also double (in this example it would jump from a neutral value of 40% to a new value of 60%).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "I_fors":
                    description = "Boost Regulation Map, I-Gain factor of PID control";
                    helptext = "Boost Regulation Map, I-Gain factor of PID table. The I value controls how much the PID output changes in response to a long term error. If in this example the idle speed rose from 700 rpm to 750 rpm as the PID output changed from 40% to 50%, then a long term error would result, because the idle has not reached its target of 800 rpm. For this reason, the PID output will slowly climb until the target is reached. In this example, the PID output would first jump from 40% to 50% due to the P value, and then would rise to 51%, 52%, 53%, etc, until the idle speed reached 800 rpm. The I value controls how quickly this long term adjustment takes place. You might have guessed that by doubling the I value, the rate of adjustment also doubles.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "D_fors":
                    description = "Boost Regulation Map, D-Gain factor of PID control";
                    helptext = "Boost Regulation Map, D-Gain factor of PID control. The D value controls how the PID output changes when the error is rapidly changing. If in this example the idle speed rose rapidly when the PID output changed from 40% to 50%, then the PID output might be reduced a few percent to counteract this rapid rise. The D value has only a small influence compared to the P and I value, and is used mostly to prevent the controlled system oscillating (e.g. a hunting idle). Again, the larger the D value, the more pronounced the anti-oscillation action will be.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "P_fors_a":
                    description = "Boost Regulation Map, P-Gain factor of PID control (automatic)";
                    helptext = "Boost Regulation Map, P-Gain factor of PID control (automatic). The P value controls how much the PID output instantly changes in response to a difference between the target and the feedback (the error). For example, if your idle speed dropped to 700 rpm and the target was 800 rpm, then the PID output might jump from the neutral point of 40% to a new value of 50%. By doubling the P value, then the jump in PID output would also double (in this example it would jump from a neutral value of 40% to a new value of 60%).";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "I_fors_a":
                    description = "Boost Regulation Map, I-Gain factor of PID control (automatic)";
                    helptext = "Boost Regulation Map, I-Gain factor of PID control (automatic). The I value controls how much the PID output changes in response to a long term error. If in this example the idle speed rose from 700 rpm to 750 rpm as the PID output changed from 40% to 50%, then a long term error would result, because the idle has not reached its target of 800 rpm. For this reason, the PID output will slowly climb until the target is reached. In this example, the PID output would first jump from 40% to 50% due to the P value, and then would rise to 51%, 52%, 53%, etc, until the idle speed reached 800 rpm. The I value controls how quickly this long term adjustment takes place. You might have guessed that by doubling the I value, the rate of adjustment also doubles.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "D_fors_a":
                    description = "Boost Regulation Map, D-Gain factor of PID control (automatic)";
                    helptext = "Boost Regulation Map, D-Gain factor of PID control (automatic). The D value controls how the PID output changes when the error is rapidly changing. If in this example the idle speed rose rapidly when the PID output changed from 40% to 50%, then the PID output might be reduced a few percent to counteract this rapid rise. The D value has only a small influence compared to the P and I value, and is used mostly to prevent the controlled system oscillating (e.g. a hunting idle). Again, the larger the D value, the more pronounced the anti-oscillation action will be.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Regl_tryck_fgm":
                    description = "Maximum boost in first and reverse gear (limiter)";
                    helptext = "Maximum boost in first and reverse gear. This value is only valid for manual transmission. It limits the maximum turbo charge pressure to this value to prevent the transmission to get to much torque on it.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_manual;
                    break;
                case "Regl_tryck_sgm":
                    description = "Maximum boost in second gear (limiter)";
                    helptext = "Maximum boost in second gear. This value is only valid for manual transmission. It limits the maximum turbo charge pressure to this value to prevent the transmission to get to much torque on it.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_manual;
                    break;
                case "Regl_tryck_fgaut":
                    description = "Maximum boost in first gear (automatic)";
                    helptext = "Maximum boost in first gear (automatic). This value is only valid for automatic transmission. It limits the maximum turbo charge pressure to this value to prevent the transmission to get to much torque on it. Acts on values below a programmable gear-ratio";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_automatic;
                    break;
                case "Regl_tryck_fga":
                    description = "Maximum boost in first gear (automatic)";
                    helptext = "Maximum boost in first gear (automatic). This value is only valid for automatic transmission. It limits the maximum turbo charge pressure to this value to prevent the transmission to get to much torque on it. Acts on values below a programmable gear-ratio";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Basic_automatic;
                    break;
                case "Temp_reduce_mat":
                    description = "Fuel Injection, Temperature compensation reduction map (open throttle)";
                    helptext = "Fuel Injection, Temperature compensation reduction map (open throttle). When temperature rises less fuel has to be injected because the charge contains less O2. This map corrects for temperature fluctuations.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Temp_reduce_mat_2":
                    description = "Fuel Injection, Temperature compensation reduction map (closed throttle)";
                    helptext = "Fuel Injection, Temperature compensation reduction map (closed throttle). When temperature rises less fuel has to be injected because the charge contains less O2. This map corrects for temperature fluctuations.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Adapt_korr":
                    description = "The adapted injection table (spot adaption base)";
                    helptext = "The adapted injection table (spot adaption base). This value holds the INITIAL values for the fuel adaption map that is actually stored in SRAM. Trionic copies this table to SRAM at cold boot (when no values are present) and starts to adjust these values as the engine runs. As long as the ECU is not disconnected from the battery it holds its values. This way the Trionic can maintain adjustments made even when the car is switched off. Adjustments are made all the time to reach best for for every engine and fuel used.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_korr_high":
                    description = "Maximum value for fuel adaption map";
                    helptext = "Maximum value for fuel adaption map. If an element in short term fuel adaption matrix is above Adapt_korr_high, adaption error is set.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_korr_low":
                    description = "Minimal value for fuel adaption map";
                    helptext = "Minimal value for fuel adaption map. If an element in short term fuel adaption matrix is below Adapt_korr_low, adaption error is set.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Restart_status":
                    description = helptext = "Status variable holding flags for restart status";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_start_extra_value":
                    description = helptext = "Idle start extra current value";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_start_extra_ramp":
                    description = helptext = "Number of 180 degrees pulses for decrement";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_start_extra":
                    description = helptext = "Idle start extra, new value after 5 minutes soak";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_start_extra_sp":
                    description = helptext =  "Idle start extra, new value after 5 minutes soak, map support points";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Rxbuf":
                case "rxbuf":
                    description = "Receive buffer";
                    helptext = "Receive buffer";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "txbuf":
                case "Txbuf":
                    description = "Transmit buffer";
                    helptext = "Transmit buffer";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Immotimer":
                case "ImmoTimer":
                    description = "Immobilizer timer";
                    helptext = "Immobilizer timer";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Acc_fak":
                    description = "Present acceleration enrichment not temperature compensated";
                    helptext = "Present acceleration enrichment not temperature compensated";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Acc_temp":
                    description = "Present acceleration enrichment temperature factor.";
                    helptext = "Present acceleration enrichment temperature factor.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Acc_mangd":
                    description = "Present acceleration enrichment for each cylinder";
                    helptext = "Present acceleration enrichment for each cylinder";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Last_delta":
                    description = "Present load derivative (change)";
                    helptext = "Present load derivative (change)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Lacc_mangd":
                    description = "Present load enrichment for each cylinder";
                    helptext = "Present load enrichment for each cylinder";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Lret_mangd":
                    description = "Present load enleanment for each cylinder";
                    helptext = "Present load enleanment for each cylinder";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ad_bat":
                case "AD_bat":
                    description = "Analogue reading of battery voltage in A/D units";
                    helptext = "Analogue reading of battery voltage in A/D units";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ad_knock":
                case "AD_knock":
                    description = "Analogue reading of knock input A/D units";
                    helptext = "Analogue reading of knock input A/D units";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ad_kylv":
                case "AD_kylv":
                    description = "Analogue reading of cooling water temperature in A/D units";
                    helptext = "Analogue reading of cooling water temperature in A/D units";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Ad_egr":
                case "AD_egr":
                case "AD_EGR":
                    description = "Analogue reading of EGR (exhaust gas return) temperature in A/D units";
                    helptext = "Analogue reading of EGR (exhaust gas return) temperature in A/D units";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Ad_sond":
                case "AD_sond":
                    description = "Analogue reading of lambda probe before catalyst in A/D units. AD_sond = U_lambda_eng/20.";
                    helptext = "Analogue reading of lambda probe before catalyst in A/D units. AD_sond = U_lambda_eng/20.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "AD_lufttemp":
                    description = "Analogue reading of inlet manifold air temperature in A/D units";
                    helptext = "Analogue reading of inlet manifold air temperature in A/D units";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "AD_trot":
                    description = "Analogue reading of throttle position in A/D units";
                    helptext = "Analogue reading of throttle position in A/D units";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "AD_cat":
                    description = "Analogue reading of lambda probe after catalyst in A/D units. AD_sond = U_lambda_cat/20.";
                    helptext = "Analogue reading of lambda probe after catalyst in A/D units. AD_sond = U_lambda_cat/20.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "P_Manifold10":
                    description = "Monitors the inlet manifold pressure. Resultion = 10kPa. Update frequency = 2 milliseconds";
                    helptext = "Monitors the inlet manifold pressure. Resultion = 10kPa. Update frequency = 2 milliseconds";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "P_Manifold":
                    description = "Monitors the inlet manifold pressure. Resultion = 1kPa. Update frequency = 2 milliseconds";
                    helptext = "Monitors the inlet manifold pressure. Resultion = 1kPa. Update frequency = 2 milliseconds";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Antal_steg":
                    description = "Contains number of half enginerotations (180 degrees) from last lambda probe value change";
                    helptext = "Contains number of half enginerotations (180 degrees) from last lambda probe value change";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cyl_nr":
                    description = "Present cylinder on which fuel is calculated";
                    helptext = "Present cylinder on which fuel is calculated";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_tid":
                    description = "Base injection time during start cranking";
                    helptext = "Base injection time during start cranking";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Readiness":
                    description = "Indicates catalyst monitor, purge monitor, both oxygen sensor monitor and both oxygen sensor heater monitor readiness status";
                    helptext = "Indicates catalyst monitor, purge monitor, both oxygen sensor monitor and both oxygen sensor heater monitor readiness status";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cat_status":
                    description = "Indicates that catalyst monitor diagnostic is ready.";
                    helptext = "Indicates that catalyst monitor diagnostic is ready.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt":
                    description = "Purge monitor status field.";
                    helptext = "Purge monitor status field. Contains many bitwise flags.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Diag_status":
                    description = "Indicates that several diagnosticis are ready.";
                    helptext = "Indicates that oxygen sensor 1 transition diagnostic, oxygen sensor 2 activity diagnostic, oxygen sensor 1 heater diagnostic, oxygen sensor 2 heater diagnosticis and cooling water temperature activity diagnostics are ready. (bitwise)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "MError":
                    description = "Indicates several momentary errors.";
                    helptext = "Indicates momentary error on throttle potentiometer, vehicle speed signal, water temperature signal, inlet manifold pressure signal, oxygen sensor 1 signal, oxygen sensor 2 signal, battery voltage, inlet air temperature signal and knock signal. (bitwise)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ox2_change":
                    description = "Variable including additative oxygen sensor 2 signal change.";
                    helptext = "Variable including additative oxygen sensor 2 signal change.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ox2_change_lim":
                    description = "If Ox2_change is above this limit after Ox2_activity_time the oxygen sensor 2 activity test is approved.";
                    helptext = "If Ox2_change is above this limit after Ox2_activity_time the oxygen sensor 2 activity test is approved.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ox2_activity_time":
                    description = "Oxygen sensor 2 activity time counter.";
                    helptext = "Oxygen sensor 2 activity time counter.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ox2_activity_time_lim":
                    description = "When Ox2_activity_time exceeds this limit the oxygen sensor 2 change activity is evaluated.";
                    helptext = "When Ox2_activity_time exceeds this limit the oxygen sensor 2 change activity is evaluated.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ox1DeltaLoadLim":
                    description = "If Cat_delta_load exceeds this limit, oxygen sensor 1 transition test is halted.";
                    helptext = "If Cat_delta_load exceeds this limit, oxygen sensor 1 transition test is halted.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_filt_coef_2":
                    description = "Filter coeficient to second LP-filter on oxygen sensor 2 signal. (used to create Cat_ox2_dev)";
                    helptext = "Filter coeficient to second LP-filter on oxygen sensor 2 signal. (used to create Cat_ox2_dev)";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_fc_start_timer":
                    description = "Increments every second when fuel cut function is operating. Decrements every second when fuel cut function is not operating.";
                    helptext = "Increments every second when fuel cut function is operating. Decrements every second when fuel cut function is not operating.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_fc_start_timer_lim":
                    description = "If Cat_fc_start_timer exceeds this limit, Cat_start_timer is set to zero.";
                    helptext = "If Cat_fc_start_timer exceeds this limit, Cat_start_timer is set to zero.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_vs_start_timer":
                    description = "Increments every second when vehicle speed is zero.";
                    helptext = "Increments every second when vehicle speed is zero.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_vs_start_timer_lim":
                    description = "If Cat_vs_start_timer exceeds this limit, Cat_start_timer is set to zero.";
                    helptext = "If Cat_vs_start_timer exceeds this limit, Cat_start_timer is set to zero.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_monitor_restart":
                    description = "If set to one, the catalyst monitor will restart. (automaticly set to zero directly afterwards)";
                    helptext = "If set to one, the catalyst monitor will restart. (automaticly set to zero directly afterwards)";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_PWM":
                    description = "PWM out to purge during monitor test.";
                    helptext = "PWM out to purge during monitor test.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_LambdaInt":
                    description = "System lambda integrator.";
                    helptext = "System lambda integrator.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_LambdaAvg":
                    description = "Average lambda int during one lambda cycle.";
                    helptext = "Average lambda int during one lambda cycle.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_LambdaAvgRef":
                    description = "Average lambda int during PMCal_LamCntTransLim lambda transitions.";
                    helptext = "Average lambda int during PMCal_LamCntTransLim lambda transitions.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_LambdaAvgSum":
                    description = "Average lambda int sum during PMCal_LamCntTransLim lambda transitions.";
                    helptext = "Average lambda int sum during PMCal_LamCntTransLim lambda transitions.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_LambdaTransCnt":
                    description = "Counts lambda transitions.";
                    helptext = "Counts lambda transitions.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMProt_OpenTime":
                    description = "Current time in purge open fase.";
                    helptext = "Current time in purge open fase.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_IdleRefValue":
                    description = helptext = "Stored reference idle position.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_TestCounter":
                    description = helptext = "Number of completed purge monitor tests done during one trip.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_RpmIdleNomRef":
                    description = helptext = "Reference rpm idle nominal.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "PMProt_IdlePos":
                    description = helptext = "Current idle position (LFR/ETS)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_IdlePosFilt":
                    description = helptext = "Filtered idle position.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_IdlePosIntABS":
                    description = helptext = "Absolute value of integrated idle position";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMProt_RpmIdleNom":
                    description = helptext = "Rpm idle nominal.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "PMCal_StartRamp":
                    description = helptext = "Rampfactor to purge monitor start position (0%).";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_LamTransCntLim":
                    description = helptext = "Number of transitions used for lambda integrator reference calculation.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMCal_OpenTimeTab":
                    description = helptext = "Time axis for purge monitor open phase.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_OpenPWMTab":
                    description = helptext = "PWM axis for purge monitor open phase.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_LambdaAvgLim":
                    description = helptext = "Lambda change limit from reference value for a successful purge diagnostic test.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMCal_IdleValueLim":
                    description = helptext = "Idle position change limit from reference value for a successful purge diagnostic test.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_CloseRamp":
                    description = helptext = "Ramp factor during close phase.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_LambdaAvgHaltLim":
                    description = helptext = "Halt limit on lambda average reference signal.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "PMCal_IdleRefHaltLim":
                    description = helptext = "Halt limit on idle reference position.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_TestCountLim":
                    description = helptext = "Number of tests that trionic will perform before purge test fails.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_RpmIdleNomRefLim":
                    description = helptext = "RPM idle nominal change reference halt limit.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "PMCal_IdlePosFiltCoef":
                    description = helptext = "Idle position filter coeficient. 1.000 -> Min filtering. 0.000 -> Max filtering.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PMCal_IdlePosIntABSLim":
                    description = helptext = "Halt limit for fast idle position changes.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "MisCountMap1":
                case "MisCountMap2":
                case "MisCountMap3":
                case "MisCountMap4":
                    description = helptext = "Counts number of misfires in each map point. Resets only if trionic looses its supply voltage or if Kontrollord differs from ABCD(hex) when system starts up.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "API_throttle_tab":
                    description = helptext = "Table used to determine ignition offset retardation depending on relative throttle angle when DRIVE is engaged.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_dt_offset_tab":
                    description = helptext = "Function to reduce torque when driver switches from NEUTRAL to DRIVE and has activated accelerator pedal.";
                    /*              Criterias to enable function:
                                  -Automatic flag set.
                                  -Engine running.
                                  -Driver switches from NEUTRAL to DRIVE.

                                  The throttle angle is read and ignition is reduced using
                                  API_dt_offset_tab. Retardation is active and offset is 
                                  uppdated continuous during API_dt_delay and ramped back 
                                  during API_dt_ramp.";*/
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API":
                    description = helptext = "API = Automatic Transmission";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_dt_delay":
                    description = helptext = "Delay time for 'drive and throttle' ignition retardation before ramping back.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_dt_ramp":
                    description = helptext = "Ramp time for 'drive and throttle' ignition retardation ramping back.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_ku_offset":
                    description = helptext = "Function to reduce torque when driver kick's down and automatic gearbox shifts up.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                //
                case "API_ku_derivata":
                    description = helptext = "Negative rpm derivata limit (rpm/10ms).";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_ku_delay":
                    description = helptext = "Delay time for 'kickdown and upshift' ignition retardation before ramping back.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_ku_ramp":
                    description = helptext = "Ramp time for 'kickdown and upshift' ignition retardation ramping back.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_rpm_limit":
                    description = helptext = "Below this rpm limit no 'kickdown and upshift' ignition retardation will be enabled.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "API_throttle_limit":
                    description = helptext = "Below this throttle limit no 'kickdown and upshift' ignition retardation will be enabled.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Sond_oms_fkat":
                case "Sond_oms_ekat":
                    description = helptext = "Not used.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "R_sond_lean_ign":
                    description = helptext = "Counts number of ignitions between front lambda sensor lean indication to rear lambda sensor lean indication. Updated every 180 degrees engine rotation";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "R_sond_rich_ign":
                    description = helptext = "Count's number of ignitions between front lambda sensor rich indication to rear lambda sensor rich indication. Updated every 180 degrees engine rotation";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "R_sond_lean_limit":
                    description = helptext = "Threshold for bad lean response detection.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "R_sond_rich_limit":
                    description = helptext = "Threshold for bad rich response detection.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "U_lambda_eng":
                    description = helptext = "Monitors current front lambda signal. Updates every 2 milliseconds";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "U_lambda_cat":
                    description = helptext = "Monitors current rear lambda signal. Updates every 2 milliseconds";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Sond_omsl_limit":
                    description = helptext = "Threshold for transition test.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Sond_ign_limit":
                    description = helptext = "Test condition for front lambda sensor transition test.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Diag_speed_time":
                    description = helptext = "Timelimit for load and rpm to be over their limits for detecting vehicle speed error.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Sond_omsl_counter":
                    description = helptext = "Front oxygen sensor transition counter.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Sond_ign_counter":
                    description = helptext = "Counts ignitions during Sond_omsl_limit - 4 front oxygen sensor transitions. Updates every 10 milliseconds";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Det12":
                    description = helptext = "Monitors detect signal from DI cassette cylinder 1 and 2. 16 samples are made during a detect window. Updates every 180 degrees engine rotation";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Det34":
                    description = helptext = "Monitors detect signal from DI cassette cylinder 3 and 4. 16 samples are made during a detect window. Updates every 180 degrees engine rotation";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Mis1000_limit":
                    description = helptext = "Return value from Mis1000_map";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis200_limit":
                    description = helptext = "Return value from Mis200_map";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Detect_limit":
                    description = "Return value from Detect map. Reference value for misfire detection.";
                    helptext = "Return value from Detect map. Reference value for misfire detection. If (Det12 < Detect_limit) engine misfires on cylinder 1 or 2. If (Det34 < Detect_limit) engine misfires on cylinder 3 or 4";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Cat_load_filt":
                    description = helptext = "Filtrated load signal (Last) using Cat_load_filt_coef as filtercoeficient.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_delta_load":
                    description = helptext = "To determine load transients. Cat_delta_load = ABS( Last - Cat_load_filt )";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_air_flow":
                    description = helptext = "To symbolize the air-flow through the engine. The value has no unit but should have the same tendency as an air-flow meter. Logic : Cat_air_flow = (RPM / 100) * Last";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_start_timer":
                    description = helptext = "Counts seconds to ensure a warm catalyst.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_start_timer_lim":
                    description = helptext = "To enable catalyst monitor. if (Cat_start_timer < Cat_start_timer_lim) Catalyst monitor disabled.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_af_start_timer_min":
                    description = helptext = "If the air flow is below this threshold, the Cat_start_timer halts.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ready_timer":
                    description = "To enable catalyst monitor.";
                    helptext = "To enable catalyst monitor. When warm up conditions are fullfilled (i.e. delta-load, cooling-watertemp,vehicle-speed,air-flow,closed-loop = OK) Cat_ready_timer starts to increment towards Cat_ready_timer_lim. If one of the warm up conditions is not fullfilled Cat_ready_timer starts to decrement disabeling the monitor.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_filt_coef":
                    description = helptext = "Result value from Cat_ox1_filt_coef_map. Used to filtrate oxygen sensor 1 signal.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_filt":
                    description = helptext = "Process the filtrated signal from oxygen sensor 1 using Cat_ox1_filt_coef.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_filt_bias":
                    description = helptext = "Bias compensated oxygen sensor 1 signal using Cat_ox1_bias_lim.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_err":
                    description = helptext = "Absolute value of Cat_ox1_filt_bias. Shows peek voltage around bias voltage. Cat_ox1_err = ABS(Cat_ox1_filt_bias)";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_err_max":
                    description = helptext = "Stored max value of Cat_ox1_filt_bias during a  complete oxygen sensor 1 cycle. Used for cycle evaluation.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_filt_coef":
                    description = helptext = "Result value from Cat_ox2_filt_coef_map. Used to filtrate oxygen sensor 2 signal.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_load_tab":
                    description = helptext = "Load site for calibrating maps using load as input data.  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_rpm_tab":
                    description = helptext = "Rpm site for calibrating maps using engine speed as input data.  Resolution is 10.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev":
                    description = helptext = "Absolute and filtrated oxygen sensor 2 value.  Resolution is 1mV. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_max":
                    description = helptext = "Stored max value of Cat_ox2_dev during a complete oxygen sensor 1 cycle. Used for cycle evaluation.  Resolution is 1mV. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_hi":
                    description = helptext = "Stores oxygen sensor 1 high time during a complete oxygen sensor 1 cycle.  Resolution is 1mS.. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_lo":
                    description = helptext = "Stores oxygen sensor 1 low time during a complete oxygen sensor 1 cycle.  Resolution is 1mS.. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_hi_lim":
                    description = helptext = "Result value from Cat_ox1_per_hi_tab. if (Cat_ox1_per_hi > Cat_ox1_per_hi_lim) Do not evaluate current oxygen sensor 1 cycle.  Resolution is 1mS. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_lo_lim":
                    description = helptext = "Result value from Cat_ox1_per_lo_tab. if (Cat_ox1_per_lo > Cat_ox1_per_lo_lim) Do not evaluate current oxygen sensor 1 cycle.  Resolution is 1mS. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_duty_cycle":
                    description = helptext = "Current oxygen sensor 1 duty cycle. Resolution is 1%. Interval is After each ox1 window cycle..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_max_lim":
                    description = helptext = "Result value from Cat_ox2_dev_max_map. This value should contain the max deviation of a new catalyst depending on Cat_ox1_err_max.  Resolution is 1mV. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_samp":
                    description = helptext = "Oxygen sensor 1 cycle sample counter. Resolution is 1. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_dev_diff":
                    description = helptext = "Deviation difference between front and rear lambda sensors. Value limited to min 0 and max 255. Cat_dev_diff = Cat_ox2_dev_max - Cat_ox2_dev_max_lim if (Cat_dev_diff < 0) Cat_dev_diff = 0 if (Cat_dev_diff > 255) Cat_dev_diff = 255  Resolution is 1. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_sum":
                    description = helptext = "Sum of deviation differences over a number of samples. Cat_ox2_dev_sum = Cat_ox2_dev_sum + Cat_dev_diff  Resolution is 1. Interval is Every 10ms when catalytic test is pending..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Fload_value":
                    description = helptext = "Present full load enrichment.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Purge_duty":
                    description = helptext = "Present purge duty cycle";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Purge_freq":
                    description = helptext = "Present purge freqency";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Purge_status":
                    description = helptext = "Status flags for purge control.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_dead":
                    description = helptext = "Number of half engine revs fuel should be switched of after beforestart.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_count":
                    description = helptext = "Counter for number of half enginerevs from start.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cyl_i_tur":
                    description = helptext = "Next cylinder for which ignition and fuel delay is set up";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "DA_insp":
                    description = helptext = "Scaled injection time in order to fit within one byte. Useful for plotting and D/A output.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "DA_d_trot":
                    description = helptext = "Scaled throttle derivative in order to fit within one byte. Useful for plotting and D/A output.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Duty_C":
                    description = helptext = "Idle valve duty cycle. Range 0 => 624";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Eft_dec":
                    description = helptext = "Number of half enginerev. between a unit of decrease for Eft_fak1. Interpolated value from Eft_dec_fak";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_dec2":
                    description = helptext = "Number of half enginerev. between a unit of decrease for Eft_fak2. Interpolated value from Eft_dec_fak2";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_fak":
                    description = helptext = "Enrichmentfactor for coldstart #1 interpolated value from Eftersta_fak";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_fak2":
                    description = helptext = "Enrichmentfaktor for coldstart #2 interpolated value from Eftersta_fak tab.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_fak_bas":
                    description = helptext = "Base value for afterstart enrichment 1";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Eft_fak_bas2":
                    description = helptext = "Base value for afterstart enrichment 2";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Extern_tid":
                    description = helptext = "This variabel contains the injection time when the injection system is run as a slave under a external computers control.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Fet_steg":
                    description = helptext = "Present lambdacontrol step to the rich side.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Frekv_map":
                    description = helptext = "Lambda ramp combustion delay";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Frekv":
                    description = helptext = "Number of ignitions between every update of lambda control.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Ganger_fet":
                    description = helptext = "Number of ignitions the lambdacontrol has spent on the rich side since the last switch.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Ganger_magr":
                    description = helptext = "Number of ignitions the lambdacontrol has spent on the lean side since the last switch.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Grund_tid":
                    description = helptext = "Base injectiontime in the system";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Batt_korr":
                    description = helptext = "Correction on injectiontime depending on battery voltage";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cyl_fel":
                    description = helptext = "Counter for number of faults on camshaftsensor";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Insptid_ms10":
                    description = helptext = "Present injection time converted to 10 ms units";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Insp_tid":
                    description = helptext = "Present injection time";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Integ_medel":
                    description = helptext = "Average value of the lambda integrator";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Insp_vinkel_komp":
                    description = helptext = "Compensated injection angle with respect to current cylinder. System refers to 0 as TDC at cylinder 1. Cyl3 has offset 180, Cyl4 360 and so on ";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "TQ":
                    description = helptext = "Present TQ value in microseconds units";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Gl_adap":
                    description = helptext = "Global adaption counter.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Int_val_I":
                    description = helptext = "Interpolated value from the injection matrix.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Int_val_D":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Last":
                    description = helptext = "Current engine load. Last = P_Manifold(Air_temp) Resolution is 1.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Mager_steg":
                    description = helptext = "Present lambdacontrol step to the lean side.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Medeltrot":
                    description = helptext = "Average Throttle position";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ret_fak":
                    description = helptext = "Present acceleration enleanment not temperature compensated";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_temp":
                    description = helptext = "Present acceleration enleanment temperature factor.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Ret_mangd":
                    description = helptext = "Present retardation enleanment factor for each cylinder";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Rpm_pol":
                    description = helptext = "Present engine speed, scaled to fit one byte.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Retrigg_count":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Retrigg_times":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Slut_last":
                    description = helptext = "Present or last injection engine load.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_detekt_cnt":
                    description = helptext = "Counter counting number of engine rev above 500 rpm. Used for start detection.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_rot":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Start_varde":
                    description = helptext = "Startposition for LLS while cranking";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Temp_fak":
                    description = helptext = "Present engine temperature compensation factor.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Temp_fak_tab":
                    description = helptext = "Temp compensations factor for injection time";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Trot_delta":
                    description = helptext = "Present throttle derivative.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Throttle_pwm_out":
                    description = helptext = "Throttle position to or from other systems. ";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Int_indent":
                    description = helptext = "Last cause of illegal interrupt";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Int_adress":
                    description = helptext = "Last adress when illegal interrupt.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rst_indent":
                    description = helptext = "Last reset type";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Venta":
                    description = helptext = "Counter of reactivating delay for Lambdacontrol";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Insp_vinkel":
                    description = helptext = "Injection angle";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Motortid":
                    description = helptext = "Time with key on since last memory reset of unit.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Samp_area":
                    description = helptext = "Adress to sample area";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_ggr":
                    description = helptext = "Map containing number of adaptions in certain load/rpm sites";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Fel_status":
                    description = helptext = "Error code flags";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Acc_ind":
                    description = helptext = "Flag variabel containing flags showing which cylinder that has acceleration enrichment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ret_ind":
                    description = helptext = "Flag variabel containing flags showing which cylinder that has acceleration enleanment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Ipol_ign":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "AMOS_text":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "AMOS_status":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "AMOS_var":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "AMOS_button":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Ram_unvalid":
                    description = helptext = "Counter containing number of RAM resets.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Mult_time":
                    description = helptext = "Time between multisparks";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Multispark":
                    description = helptext = "Multispark mode.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset1":
                    description = helptext = "Current knock ignition offset for cylinder 1";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Purge_value":
                    description = helptext = "Purge input pin status. O = ASCII 79 = Ok. L = ASCII 76 = stuck Low. (triggs error) H = ASCII 72 = stuck High.(triggs error) U = ASCII 85 = Undefined. (triggs error) N = ASCII 78 = Not ready.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Airpump_value":
                    description = helptext = "Airpump input pin status. O = ASCII 79 = Ok. L = ASCII 76 = stuck Low. (triggs error) H = ASCII 72 = stuck High.(triggs error)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset2":
                    description = helptext = "Current knock ignition offset for cylinder 2.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset3":
                    description = helptext = "Current knock ignition offset for cylinder 3";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_offset4":
                    description = helptext = "Current knock ignition offset for cylinder 4";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Wait_count":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Wait_count1":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Wait_count2":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Wait_count3":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Wait_count4":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Knock_status":
                    description = helptext = "Knock status";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "F_heat_error":
                    description = helptext = "Error counter preheating front lambdasensor. ";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "R_heat_error":
                    description = helptext = "Error counter preheating rear lambdasensor ";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Knock_error":
                    description = helptext = "Error counter knock detection";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                /*case "Mis_heating_error":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Mis_CVS_error":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;*/
                case "Rpm_out_error":
                    description = helptext = "Error counter rpm output";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Battery_error":
                    description = helptext = "Error counter battery voltage.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Crankshaft_error":
                    description = helptext = "Error counter crankshaft sensor.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Errors;
                    break;
                case "Ign_angle":
                    description = helptext = "Ignition angle";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfire_1":
                    description = helptext = "Misfire counter cylinder 1 ";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfire_2":
                    description = helptext = "Misfire counter cylinder 2";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfire_3":
                    description = helptext = "Misfire counter cylinder 3 ";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfire_4":
                    description = helptext = "Misfire counter cylinder 4 ";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfire_tot":
                    description = helptext = "Misfire counter all cylinders ";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_ref_level":
                    description = helptext = "Knocking reference levels";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_oref_level":
                    description = helptext = "Not used";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Knock_press_limit":
                    description = helptext = "Current pressure limit for knock detection";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_opress_limit":
                    description = helptext = "Not used";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Knock_wind_off_tab":
                    description = helptext = "Closing angle of knock window from matrix";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_wind_on_tab":
                    description = helptext = "Opening angle of knock window from matrix";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_wind_on":
                    description = helptext = "Opening angle of knock window from matrix";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_wind_on_ang":
                    description = helptext = "Opening angle of knock window from matrix and compensated with ignition angle.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_wind_off":
                    description = helptext = "Closing angle of knock window from matrix";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_wind_off_ang":
                    description = helptext = "Closing angle of knock window from matrix and compensated with ignition angle.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Diag_I_front":
                    description = helptext = "Current preheating value from front lambdasensor. Resolution is 1 mA.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Diag_I_rear":
                    description = helptext = "Current preheating value from rear lambdasensor. Resolution is 1 mA.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "F_e_count":
                    description = helptext = "Error counter structure";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Jansen_area":
                    description = helptext = "Application aid symbols";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Time_det12":
                    description = helptext = "Time the detect signal was active in detect window on cylinder 1 & 2";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Time_det34":
                    description = helptext = "Time the detect signal was active in detect window on cylinder 3 & 4";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                /*case "Det_ok_counter_cyl1":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Det_ok_counter_cyl2":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Det_ok_counter_cyl3":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Det_ok_counter_cyl4":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;*/
                case "Knock_level":
                    description = helptext = "Knocklevel from ionization current.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kontrollord":
                    description = helptext = "Checkword which should contain \"ABCD\" at startup to prohibit memory init.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Rpm_idle_nom":
                    description = helptext = "Nominal rpm value for idle speed control";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Br_plus":
                    description = helptext = "Additative value added to injection time in order to compensate for low battery voltage.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Br_minus":
                    description = helptext = "Substractive value substracted from the injection time in order to compensate for big battery voltage.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_medel_rpm":
                    description = helptext = "Average engine speed during idle";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Idle_reknare":
                    description = helptext = "Counter determing if idle speed has been stable long enough to allow update of adaption.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Dashord":
                    description = helptext = "Dashpot status flags";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Sond_status":
                    description = helptext = "Lambda probe status flags";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Idle_status":
                    description = helptext = "Status flag for idle speed control";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Diag_slask":
                    description = helptext = "Not used";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "LLS_tkomp":
                    description = helptext = "Temperature compensated idle speed position";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "LLS_app":
                    description = helptext = "Scaled idle speed valve position to fit into one byte";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "LLS_styr":
                    description = helptext = "LLS status flags";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Dash_tab_value":
                    description = helptext = "Dash pot position interpolated from matrix";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Dash_act_value":
                    description = helptext = "Present dash pot value";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Stab_reknare":
                    description = helptext = "Counter for number of lambda switches before adaption";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Samp_status":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Samp_adr_1":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Samp_adr_2":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Samp_adr_3":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Samp_adr_4":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Samp_pek":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                /*case "Diag_p_duty":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;*/
                case "Service_tid":
                    description = helptext = "Run time timer";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Advanced;
                    break;
               /* case "Diag_f_buff":
                    description = helptext = "Application aid symbol";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;*/
                case "RAM_fel":
                    description = helptext = "RAM error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "ROM_fel":
                    description = helptext = "ROM error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Dog_fel":
                    description = helptext = "Watch dog error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kyl_fel":
                    description = helptext = "Water temp sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Luft_fel":
                    description = helptext = "Air temp sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Tryck_fel":
                    description = helptext = "Pressure sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Trott_fel":
                    description = helptext = "Throttle position sensor error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Sond_fel":
                    description = helptext = "Lambda probe error counter";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Hast_fel":
                    description = helptext = "Vehicle speed sensor counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kam_fel":
                    description = helptext = "Not used";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Idle_fel":
                    description = helptext = "Idle speed control error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Idle;
                    break;
                case "Purgefel":
                    description = helptext = "Purge control error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kat_fel":
                    description = helptext = "Catalythic converter error counter.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_fel":
                    description = helptext = "Adaption error counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kat_tro_fel":
                    description = helptext = "Counter for probable catalythic converter errors.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kat_test":
                    description = helptext = "Number of catalythic converter test done during this trip.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
              /*  case "Diag_buff":
                    description = helptext = "Not used.";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;*/
                case "ISAT_visa":
                    description = helptext = "Not used.";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Tid_overs":
                    description = helptext = "Time for overboost pressure.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Medeltryck":
                    description = helptext = "Average inlet manifold pressure.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_decrese":
                    description = helptext = "In what proportion pressure decreases when indicating knock";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Spik_count":
                    description = helptext = "Counting number of knocks.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "APC_status":
                    description = helptext = "Statusbyte to show which modes are active/inactive";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Apc_adapt":
                    description = helptext = "Apc controler adaption value.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Reg_kon_apc":
                    description = helptext = "APC controler constant.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_fak":
                    description = helptext = "APC controler I-factor.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "D_fak":
                    description = helptext = "APC controler D-factor.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "P_fak":
                    description = helptext = "APC controler P-factor";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Regl_tryck":
                    description = helptext = "Map value on which APC controler aims to, to get correct torque";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "PWM_ut10":
                    description = helptext = "APC-PWM signal from AFM to Solenoidvalve. Unit 1 %.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Gear":
                    description = helptext = "Present gear.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "P_medel":
                    description = helptext = "Average inlet manifold pressure.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "After_fcut":
                    description = helptext = "Extra fuel enrichment after fuel cut";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Hot_start_fak":
                    description = helptext = "Enrichmentfactor from Hot_tab when hotstartenrichment is active";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                /*case "Ret_fuel_fak":
                    description = helptext = "Base factor for rpm dip enrichment";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;*/
                case "Nom_last":
                    description = helptext = "Nominal load value for idle speed control.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Restart_fak":
                    description = helptext = "Restart fuel compensation factor.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Last_reg_fak":
                    description = helptext = "Present constant for idle speed load control.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_time":
                    description = helptext = "Time from first detection of crankweel GAP until rpm > StRPM. Resolution is 0.01 sec.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_time_tot":
                    description = helptext = "Time from first detection of crankweel MOVING until rpm > StRPM. Resolution is 0.01 sec.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_time_tot_max":
                    description = helptext = "Maximum time meassured from first detection of crankweel MOVING until rpm > StRPM. ( Maximum value of \"Start_time_tot\" ). Resolution is 0.01 sec.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_time_rpm_lim":
                    description = helptext = "Rpm limit used for start time meassuring. Resolution is 1 rpm.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Limit_low":
                    description = helptext = "Limit for cylinder detection low part";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Limit_high":
                    description = helptext = "Limit for cylinder detection high part";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_start":
                    description = helptext = "Knock window start time";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Synk_ok_limit":
                    description = helptext = "Syncronisation counter counting number of succesful sync detections.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Press_trans_type":
                    description = helptext = "Saab part no of current pressure transducer.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Pulses_per_rev":
                    description = helptext = "Number of pulses from wheel during 1 rev.";
                    category = XDFCategories.Runtime;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "Purge_temp":
                    description = helptext = "The engine temperature above which Purge is alllowed.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Purge_tab":
                    description = helptext = "The Purge matrix (0-100%)";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ramp_fak":
                    description = helptext = "Correcting injectiontime with a faktor in this map at every halv enginerev when ramping during cranking. ";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_dead_tab":
                    description = helptext = "Number of dead injections.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_max":
                    description = helptext = "Number of halv enginerevs when injectiontime is constant";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_ramp":
                    description = helptext = "Number of halv enginerevs to stop ramping and stabilize injectiontime.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_trottel":
                    description = helptext = "Throttlevalue which is needed to achieve total fuelcut while cranking";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Indent_kvot":
                    description = helptext = "Not used.";
                    category = XDFCategories.Undocumented;
                    subcategory = XDFSubCategory.Undocumented;
                    break;
                case "Halva_sidan":
                    description = helptext = "This constant is used by pointadaption to decide if the riftpoint the engine works in is close enough to the matrixpoint which is closest to allow adaption (0-7 - 7 is easyest to allow adaption) ";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Leave_shut":
                    description = helptext = "Additive factor to lambda control when leaving idlecontrol This results in a enrichment when opening throttle";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Lamb_temp":
                    description = helptext = "Min. Coolingwatertemp to start Lambdacontrol.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Ramp_steg":
                    description = helptext = "Number of steps lambdaintergrator will change while ramping.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Pgm_mod":
                    description = helptext = "Program control flags";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_detekt_rpm":
                    description = helptext = "Engine speed should be over this limit to fulfil the first creterion of \"Motorn_startad\"";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_detekt_nr":
                    description = helptext = "The engine should be this number of halve enginerevs over the Start_detect_rpm to fulfil the second creterion of \"Motorn_startad\"";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Startvev_fak":
                    description = helptext = "Enrichmentfactor on Start_insp depending on cooling water temperature";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Eft_fak_minsk":
                    description = helptext = "Number of steps to decrease Eft_fak2";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Warmup;
                    break;
                case "Expand":
                    description = helptext = "Multiplicative factor to load and engine-speed axles. Introduced to experiment with idlemap.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Temp_steg":
                    description = helptext = "Map-support points used in the most cooling water dependend maps";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Kyltemp_steg":
                    description = helptext = "Map to convert coolingwatertemps A/D value to øC,";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Kyltemp_tab":
                    description = helptext = "Same as Kyltemp_steg, Value in øC +40";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Lufttemp_steg":
                    description = helptext = "Map-support points used in the most airtemp dependend maps";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Lufttemp_tab":
                    description = helptext = "Map to convert airtemps A/D value to øC, map-support points in Lufttemp_steg. Value in øC +40.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Luft_kompfak":
                    description = helptext = "Map over correction factors on injectiontime as a function of airtemp. Map-support points from Lufttemp_steg";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Vinkel_konst":
                    description = helptext = "If the program not interpolates delay-angle from crankshaftpulse to injectionstart in the Delay map, the angle will be taken from this constant.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Min_tid":
                    description = helptext = "Lowest possible injectiontime in the system";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_insp":
                    description = helptext = "Base fuel amount (constant) while cranking";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Cut_ej_under":
                    description = helptext = "Lower engine speed limit to be able to allow fuel cut";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Open_varv":
                    description = helptext = "When enginerev is coming under this limit the fuel will come back successively for all cylinders";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Start_proc":
                    description = helptext = "Procentual adjusting of injection time (after Start_max) before ramping ";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Start_v_vinkel":
                    description = helptext = "Injection angle while cranking";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Build_up_time":
                    description = helptext = "Delay between first detected crank tooth and before start. Compensation for building up fuel system pressure. Active when coolingwater temp is above Build_up_time_switch.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Build_up_time_2":
                    description = helptext = "Delay between first detected crank tooth and before start. Compensation for building up fuel system pressure. Active when coolingwater temp is below Build_up_time_switch.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "Build_up_time_switch":
                    description = helptext = "Temp limit for Build_up_time. if(Kyl_temp > Build_up_time_switch) Use Build_up_time for delay. else Use Build_up_time_2 for delay.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Cranking;
                    break;
                case "BS_temp":
                    description = helptext = "Enables before start at restart after 0 to 45 sec if last start was succsessful and coolingwater temperature is above BS_temp.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Open_all_varv":
                    description = helptext = "When enginerev is coming under this limit the fuel will come back simultanously for all cylinders";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cut_temp":
                    description = helptext = "Minimal coolingwatertemp to allow fuel cut";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Shut_off_time":
                    description = helptext = "Time between engine stop and disactivating main-relay";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_luft_cel":
                    description = helptext = "Last actual airtemp (more text)";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "AC_fak":
                    description = helptext = "Additive (enrichment) offset to lambdacontrol when AC activates.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "AC_const":
                    description = helptext = "Number of half enginerevs AC compensation is active.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "AC_slope":
                    description = helptext = "Number of half enginerevs between every decreament of AC_fak when ramping if lamdacontrol is not activated.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "AC_wait_on":
                    description = helptext = "Delaytime in milliseconds when AC engages. Function of air temperature";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "AC_wait_off":
                    description = helptext = "Delaytime in milliseconds when AC disables. Function of air temperature";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_kyl_st":
                    description = helptext = "Support points for cooling water temperature dependend maps";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "I_luft_st":
                    description = helptext = "Support points for airtemp dependend mas";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Temperature_calculation;
                    break;
                case "Wait_count_tab":
                    description = helptext = "Number of sparks to wait before reducing knock offsets";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Turbo_knock_tab":
                    description = helptext = "Pressure limit for turbo knock regulation. Below these levels, boost will not be reduced.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_press":
                    description = helptext = "T5.2 Pressure limit for knock indication";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_press_tab":
                    description = helptext = "Pressure limit for knock indication";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_lim_tab":
                    description = helptext = "Maximum allowed knock ignition offset. If knock still occurs with this offset, boost reduction will become active.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_average_map":
                    description = helptext = "Average knock reduction limit in ignition degrees retard";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Lknock_oref_tab":
                    description = helptext = "Offset reference table for large knock";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_ref_matrix":
                case "Knock_ref_mat":
                    description = helptext = "Knock reference map. Lower values mean less sensitive knock detection.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_ref_tab":
                    description = helptext = "T5.2 Knock reference map. Higher values mean less sensitive knock detection.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_ref_rpm":
                    description = helptext = "T5.2 Knock reference map support points.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Overs_tab":
                    description = helptext = "Peek map, pressure peek (Overshoot)";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Reg_kon_fga":
                    description = helptext = "Regulation constant (automatic) below a programmable gear-ratio";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Reg_kon_fgm":
                    description = helptext = "Regulation constant for first and reverse gear manual";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "Reg_kon_sgm":
                    description = helptext = "Regulation constant for second gear manual";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_manual;
                    break;
                case "Max_ratio_aut":
                    description = helptext = "Below this limit load pressure reduce is active.";
                    category = XDFCategories.Boost_control;
                    subcategory = XDFSubCategory.Advanced_automatic;
                    break;
                case "Hot_tab":
                    description = helptext = "Enrichmentfactor when hotstartenrichment is active";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Hot_decr":
                    description = helptext = "Decrementtime on Hot_start_fak";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Enrichment;
                    break;
                case "Idle_step_imns":
                    description = helptext = "Decrement value within the inner band";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_step_ioka":
                    description = helptext = "Increment value within the inner band";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Derivata_grans":
                    description = helptext = "Smallest rpm difference for idle control";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "I_medel_varv":
                    description = helptext = "Number of rotations for integrating control. (Number of engine revolutions needed to calculate average enginespeed)";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Min_dead":
                    description = helptext = "Rpm offset for lower integrator limit.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Max_dead":
                    description = helptext = "Rpm offset for higher integrator limit.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Delay":
                    description = helptext = "Number of 131 millisecond intervals to permit adaption. (How many seconds engine should be in the I part to allow adaption)";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Hast_grans":
                    description = helptext = "Lowest vehicle speed with idle control. (Minimal car speed limit to enable idle control)";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_ej_off":
                    description = helptext = "Increment of valveposition first time when not idle.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_rpm_off":
                    description = helptext = "Increment of nominal rpm when Drive/Neutral";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Iv_min_offset":
                    description = helptext = "Adaption offset for Iv_min_tab Adaption window : 2000 - 2500 Rpm, \"Iv_min_load\" kPa.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Iv_min_count":
                    description = helptext = "Idle valve counter";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Iv_min_load":
                    description = helptext = "Pressure for which adaption should been adjust.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Iv_min_change":
                    description = helptext = "Step for how much adaption should been adjust.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_ind_mat":
                    description = helptext = "This map includes flags that indicate where adaption have been made. 0=No adaption, 1=Adaption";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Knock_count_map":
                    description = helptext = "This map includes knockcounters. When a knock have been detected will the counter, at the current drive condition, bee increased.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ap_max_on_time":
                    description = helptext = "Maximum time the airpump will bee activated after the engine are started.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_max_rpm":
                    description = helptext = "Maximum engine speed for the airpump. If the rpm are above this limit will the airpump bee turned off.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_max_start_watertemp":
                    description = helptext = "Maximum start watertemperature for the airpump. If the start watertemperature are above this limit will the airpump never turn on.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_max_start_airtemp":
                    description = helptext = "Maximum start airtemperature for the airpump. If the start airtemperature are above this limit will the airpump never turn on.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_max_on_watertemp":
                    description = helptext = "Maximum watertemperature for the airpump. If the watertemperature are above this limit will the airpump bee turned off.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_min_airtemp":
                    description = helptext = "Minimum start airtemperature for the airpump. If the start airtemperature are below this limit will the airpump never turn on.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ap_lambda_delay":
                    description = helptext = "Delaytime between that the airpump are turned off and that the lamdacontrol are activated.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Airpump_ign_offset":
                    description = helptext = "Ignitionangle offset value when the airpump are activated. A negative value will decrease the ignitionangle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Airpump_inj_factor":
                    description = helptext = "Injectiontime factor when the airpump are activated. A value above 1.00 will increase the injectiontime.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Airpump_status":
                    description = helptext = "Airpump status flag: -bit 128 indicates airpump activated";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Airpump_control;
                    break;
                case "Ign_offset_cyl1":
                    description = helptext = "Individual ignition offset for cylinder 1, added to calculated ignition angle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_cyl2":
                    description = helptext = "Individual ignition offset for cylinder 2, added to calculated ignition angle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_cyl3":
                    description = helptext = "Individual ignition offset for cylinder 3, added to calculated ignition angle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Ign_offset_cyl4":
                    description = helptext = "Individual ignition offset for cylinder 4, added to calculated ignition angle.";
                    category = XDFCategories.Ignition;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Cat_load_filt_coef":
                    description = helptext = "To average the engine load value.(Last). Used to detect load transients. 0.000 => Max filtering. 1.000 => Min filtering. filter_coeficient = 1-exp(-loop_time / time_constant) loop_time = 0.010s  Resolution is 0.001.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_delta_load_lim":
                    description = helptext = "Used to enable catalyst monitor function. if (Cat_delta_load > Cat_delta_load_lim) Catalyst monitor disables.  Resolution is 1 kPa.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_cool_temp_lim":
                    description = helptext = "Used to enable catalyst monitor function. if (Kyl_temp < Cat_cool_temp_lim) Catalyst monitor disables.  Resolution is 1øC.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_vehi_speed_hi":
                    description = helptext = "Used to enable catalyst monitor function. if (Bil_hast > Cat_vehi_speed_hi) Catalyst monitor disables.  Resolution is 1 km/h.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_vehi_speed_lo":
                    description = helptext = "Used to enable catalyst monitor function. if (Bil_hast < Cat_vehi_speed_lo) Catalyst monitor disables.  Resolution is 1 km/h.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_air_flow_hi":
                    description = helptext = "Used to enable catalyst monitor function. if (Cat_air_flow > Cat_air_flow_hi) Catalyst monitor disables.  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_air_flow_lo":
                    description = helptext = "Used to enable catalyst monitor function. if (Cat_air_flow < Cat_air_flow_lo) Catalyst monitor disables.  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ready_timer_lim":
                    description = helptext = "Used to enable catalyst monitor function. if (Cat_ready_timer = Cat_ready_timer_lim) Catalyst monitor enables.  Resolution is 1 sec..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ready_timer_step":
                    description = helptext = "Used to increase recovering period for catalyst after fuel dependent actions. Cat_ready_timer_step is decremented from the ready timer if the warm up data are met and the warm up data breaks due to high delta load or lambda ctrl. deactivation.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_filt_coef_map":
                    description = helptext = "Look-up tabe of filter coefficients as a function of load and rpm. As driving conditions changes, so does the O2 frequency, therefore the amount of filtering will need to change to maintain a fixed amplitude filtered output. This table look-up will provide a varying coefficient to keep the amount of filtering constant at all points.  Resolution is 0.001.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_bias_lim":
                    description = helptext = "The O2 bias is the point where the O2 sensor switches from rich to lean. This value is substracted from the filtrated O2 voltage to get Cat_ox1_err.  Resolution is 1 mV..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_err_lim":
                    description = helptext = "Its purpose is to assure that a stable fuel control condition exists before data can be considered valid for test purpose. if (Cat_ox1_err > Cat_ox1_err_lim) Do not use current cycle data for catalyst monitor.  Resolution is 1 mV..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_filt_coef_map":
                    description = helptext = "Look-up tabe of filter coefficients as a function of load and rpm. As driving conditions changes, so does the O2 frequency, therefore the amount of filtering will need to change to maintain a fixed amplitude filtered output. This table look-up will provide a varying coefficient to keep the amount of filtering constant at all points.  Resolution is 0.001.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_air_flow_tab":
                    description = helptext = "Airflow site for calibrating tables using airflow as input data.  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_hi_tab":
                    description = helptext = "To eliminate catalyst oxygen sensor data whose high period is higher (longer) than expected. This look-up table is a function of engine airflow.  Resolution is 1 mS..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_per_lo_tab":
                    description = helptext = "To eliminate catalyst oxygen sensor data whose low period is higher (longer) than expected. This look-up table is a function of engine airflow.  Resolution is 1 mS..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_duty_hi_lim":
                    description = helptext = "To define the maximum acceptable duty cycle of oxygen sensor 1. if (Cat_ox1_duty_cycle > Cat_ox1_duty_hi_lim) Do not use current cycle data for catalyst monitor.  Resolution is 1%.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_duty_lo_lim":
                    description = helptext = "To define the minimum acceptable duty cycle of oxygen sensor 1. if (Cat_ox1_duty_cycle < Cat_ox1_duty_lo_lim) Do not use current cycle data for catalyst monitor.  Resolution is 1%.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_max_map":
                    description = helptext = "Table look-up as function of load and rpm. This calibration map defines a threshold for the Cat_ox2_dev_max_lim. Cat_dev_diff = Cat_ox2_dev_max - Cat_ox2_dev_max_lim  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox1_err_max_tab":
                    description = helptext = "Table to control Cat_ox2_dev_max_lim. Input variable Cat_ox1_max_err.  Resolution is 1mV..";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_ox2_dev_samp_lim":
                    description = helptext = "To determine if enough samples have been collected to average the post-catalyst oxygen sensor maximum deviation data. if (Cat_ox2_dev_samp >= Cat_ox2_dev_samp_lim) Cat_avg_dev_diff = Cat_ox2_dev_sum / Cat_ox2_dev_samp  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Cat_stage1_threshold":
                    description = helptext = "To determine if the post-catalyst oxygen sensor's average deviation difference is small enough to pass the diagnostic test. if (Cat_avg_dev_diff > Cat_stage1_threshold) catalyst converter test fails. else catalyst converter test passes.  Resolution is 1.";
                    category = XDFCategories.Sensor;
                    subcategory = XDFSubCategory.Lambda_sensor;
                    break;
                case "Detect_map_x_axis":
                    description = helptext = "Control's the current working position in Detect_map depending on inlet manifold pressure. Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Detect_map_y_axis":
                    description = helptext = "Control's the current working position in Detect_map depending on engine speed. Resolution is 10.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Detect_map":
                    description = helptext = "Reference map for misfire detection. Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfire_map_x_axis":
                    description = helptext = "Control's the current working position in misfire map's depending on inlet manifold pressure. Resolution is 1.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Misfire_map_y_axis":
                    description = helptext = "Control's the current working position in misfire map's depending on engine speed. Resolution is 10.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Mis200_map":
                    description = helptext = "Contains maximum allowed misfire over 400 cumbustions for not causing catalyst overheating. Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis1000_map":
                    description = helptext = "Contains maximum allowed misfire over 2000 cumbustions for not causing emission level degradation. Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis_trans_limit":
                    description = helptext = "Disables misfire detection during throttle transients + 5 cumbustions. if (Trans_trott > Mis_trans_limit) Disable misfire detection Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Mis_temp_limit":
                    description = helptext = "Below this cooling water temp limit no misfire are  Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Diag_speed_rpm":
                    description = helptext = "Threshold for detecting missing vehicle speed signal. (RPM > Diag_speed_rpm) Resolution is 10 rpm.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Diag_speed_load":
                    description = helptext = "Threshold for detecting missing vehicle speed signal. (Last > Diag_speed_load) Resolution is 1.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "DI_cassette_U_limit":
                    description = helptext = "Below this voltage, signals from the DI-cassette can not be trusted. Used to disable some diagnostic tests to prevent corruption depending on bad input signals. Resolution is 0.1 V.";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_injfaktor_low":
                    description = helptext = "If the long term fuel adaption (Adapt_injfaktor) is below Adapt_injfaktor_low, adaption error is set. Resolution is 0.008.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Adapt_injfaktor_high":
                    description = helptext = "If the long term fuel adaption (Adapt_injfaktor) is above Adapt_injfaktor_high, adaption error is set. Resolution is 0.008.";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Idle_ac_extra_sp":
                    description = helptext = "Rpm breakpoints for Idle_ac_extra_tab.";
                    category = XDFCategories.Idle;
                    subcategory = XDFSubCategory.Axis;
                    break;
                case "Idle_ac_extra_value":
                    description = helptext = "Actual output value read from Idle_ac_extra_tab.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Pgm_status":
                    description = helptext = "Current program status flags";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Basic;
                    break;
                case "AC_Control":
                    description = helptext = "Data to control the ac-compressor when the brake pedal is pressed. This function must be enabled with a bit in Pgm_mod.";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Kyl_temp_limphome":
                    description = helptext = "Limphome value for water temperature";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Limp_home;
                    break;
                case "Lufttemp_faktor":
                    description = helptext = "Present air temperature compensation factor for the load calculation";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Misfcut_ign_filt":
                    description = helptext = "Misfire fuelcut filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfpres_ign_filt":
                    description = helptext = "Mmisfire pressurecut filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfrpm_ign_filt":
                    description = helptext = "Misfire RPMcut filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfac_ign_filt":
                    description = helptext = "Misfire AC on/off filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfpur_ign_filt":
                    description = helptext = "Misfire purge diag filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misftakeoff_ign_filt":
                    description = helptext = "Misfire takeoff filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misftrans_ign_filt":
                    description = helptext = "Misfire trans filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Misfgear_filt":
                    description = helptext = "Misfire Neutal/Drive shift filter";
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Misfire;
                    break;
                case "Knock_lim":
                    description = helptext = "Knock limit, value from Knock_lim_tab in degrees of ignition retard.";
                    category = XDFCategories.Knocking;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Trans_x_st":
                    description = helptext = "Delta throttle angle";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Trans_y_st":
                    description = helptext = "RPM";
                    category = XDFCategories.Misc;
                    subcategory = XDFSubCategory.Advanced;
                    break;
                case "Accel_temp":
                    description = helptext = "Acceleration temperature compensation";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Accel_temp2":
                    description = helptext = "Acceleration temperature compensation #2";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
                case "Retard_temp":
                    description = helptext = "Deceleration temperature compensation";
                    category = XDFCategories.Fuel;
                    subcategory = XDFSubCategory.Temperature_compensation;
                    break;
            }
            if (description == "" || description == symbolname)
            {
                if (symbolname.Contains("_frame"))
                {
                    if (description == "") description = helptext = symbolname;
                    category = XDFCategories.Diagnostics;
                    subcategory = XDFSubCategory.Advanced;
                }
            }
            return description;
        }
    }
}
