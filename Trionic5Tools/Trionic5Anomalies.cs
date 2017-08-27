using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using CommonSuite;

namespace Trionic5Tools
{
    public class Trionic5Anomalies
    {
        IECUFileInformation m_fileInformation = new Trionic5FileInformation();
        /// <summary>
        /// Check binary file for anomalies that cannot be right
        /// </summary>
        /// <param name="filename"></param>
        public void CheckBinForAnomalies(string filename, Trionic5Resume resume, bool fixproblems, bool showreport, IECUFileInformation fileInformation)
        {
            m_fileInformation = fileInformation;
            if (showreport)
            {
                resume.AddToResumeTable("Checking file " + Path.GetFileName(filename));
                resume.AddToResumeTable("Checking injection map against fuel knock map");
            }
            CheckInjectionMapAgainstFuelKnockMap(filename, showreport, fixproblems, resume);
            if (showreport)
            {
                resume.AddToResumeTable("Checking injection constant value");
            }
            CheckInjectionConstant(filename, showreport, resume);
            if (showreport)
            {
                resume.AddToResumeTable("Checking boost request maps agains boost limiters");
            }
            CheckBoostRequestMapAgainstBoostLimiters(filename, showreport, resume);

            if (showreport)
            {
                resume.AddToResumeTable("Checking axis against maximum requested boost level");
            }
            try
            {
                CheckBoostRequestAgainstAxisRanges(filename, true, resume);
            }
            catch (Exception E)
            {
                Console.WriteLine("CheckBoostRequestAgainstAxisRanges: " + E.Message);
            }
            if (showreport)
            {
                // seperator for next file... maybe
                resume.AddToResumeTable("");
            }
        }

        private byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            FileStream fsi1 = File.OpenRead(filename);
            while (address > fsi1.Length) address -= (int)fsi1.Length;
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = address;
            string temp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                retval.SetValue(br1.ReadByte(), i);
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();
            return retval;
        }

        private int GetSymbolLength(string symbolname)
        {
            if (symbolname == "Knock_count_cyl1" || symbolname == "Knock_count_cyl2" || symbolname == "Knock_count_cyl3" || symbolname == "Knock_count_cyl4")
            {
                return 2;
            }
            foreach (SymbolHelper sh in m_fileInformation.SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    return sh.Length;
                }
            }
            return 1;
        }

        private int GetSymbolAddress(string symbolname)
        {
            int retval = 0;
            foreach (SymbolHelper sh in m_fileInformation.SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    retval = (int)sh.Flash_start_address - m_fileInformation.Filelength;
                    while (retval > m_fileInformation.Filelength) retval -= m_fileInformation.Filelength;
                }
            }
            return retval;
        }

        private int GetMaxBoostValue(string filename)
        {
            byte[] boost_request_map = readdatafromfile(filename, GetSymbolAddress( "Tryck_mat!"), GetSymbolLength( "Tryck_mat!"));
            byte[] boost_request_map_aut = readdatafromfile(filename, GetSymbolAddress( "Tryck_mat_a!"), GetSymbolLength( "Tryck_mat_a!"));
            int retval = 0;
            foreach (byte b in boost_request_map)
            {
                int i = (int)b;
                if (i > retval) retval = i;
            }
            foreach (byte b in boost_request_map_aut)
            {
                int i = (int)b;
                if (i > retval) retval = i;
            }
            return retval;
        }

        /// <summary>
        /// Check maximum boost request against maximum in fuel_map_x_axis and ign_map_0_x_axis
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="showreport"></param>
        private void CheckBoostRequestAgainstAxisRanges(string filename, bool showreport, Trionic5Resume resume)
        {
            // Detect_map_x_axis
            //Temp_reduce_x_st
            //Ign_map_0_x_axis
            //Ign_map_2_x_axis
            //Ign_map_3_x_axis
            //Ign_map_6_x_axis
            //Ign_map_7_x_axis
            // Misfire_map_x_axis
            //Fuel_knock_xaxis
            //Fuel_map_xaxis
            //Overs_tab_xaxis
            int maxboostvalue = GetMaxBoostValue(filename);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Detect_map_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_0_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_2_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_3_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_6_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_7_x_axis!", resume);
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Misfire_map_x_axis!", resume);
            CheckEigthBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Fuel_knock_xaxis!", resume);
            CheckEigthBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Fuel_map_xaxis!", resume);

        }

        private void CheckEigthBitAxisAgainstBoostPressure(string filename, bool showreport, int maxboostvalue, string symbolname, Trionic5Resume resume)
        {
            byte[] axis = readdatafromfile(filename, GetSymbolAddress( symbolname), GetSymbolLength( symbolname));
            bool found = false;
            foreach (byte b in axis)
            {
                int i = (int)b;
                if (i >= maxboostvalue) found = true;
            }
            if (!found && showreport)
            {
                resume.AddToResumeTable(symbolname + " does not support the maximum boost request value!");
            }
        }

        private void CheckSixteenBitAxisAgainstBoostPressure(string filename, bool showreport, int maxboostvalue, string symbolname, Trionic5Resume resume)
        {
            byte[] axis = readdatafromfile(filename, GetSymbolAddress( symbolname), GetSymbolLength( symbolname));
            if (axis.Length < 2) return;
            bool found = false;
            for (int t = 0; t < axis.Length; t += 2)
            {
                int i = Convert.ToInt32(axis.GetValue(t));
                i *= 256;
                i += Convert.ToInt32(axis.GetValue(t + 1));
                if (i >= maxboostvalue) found = true;
            }
            if (!found && showreport)
            {
                resume.AddToResumeTable(symbolname + " does not support the maximum boost request value!");
            }
        }

        /// <summary>
        /// Check boost request maps (Tryck_mat & Tryck_mat_a) against limiters
        /// </summary>
        /// <param name="filename"></param>
        private void CheckBoostRequestMapAgainstBoostLimiters(string filename, bool showreport, Trionic5Resume resume)
        {
            byte[] boost_request_map = readdatafromfile(filename, GetSymbolAddress( "Tryck_mat!"), GetSymbolLength( "Tryck_mat!"));
            byte[] boost_request_map_aut = readdatafromfile(filename, GetSymbolAddress( "Tryck_mat_a!"), GetSymbolLength( "Tryck_mat_a!"));
            byte[] fuel_cut_map = readdatafromfile(filename, GetSymbolAddress( "Tryck_vakt_tab!"), GetSymbolLength( "Tryck_vakt_tab!"));
            byte[] boost_maps_axis = readdatafromfile(filename, GetSymbolAddress( "Pwm_ind_rpm!"), GetSymbolLength( "Pwm_ind_rpm!"));

            int numberofrows = (boost_maps_axis.Length / 2);
            int numberofcolumns = boost_request_map.Length / numberofrows;

            for (int fct = 0; fct < fuel_cut_map.Length; fct++)
            {
                byte boostlimit = (byte)fuel_cut_map.GetValue(fct);
                for (int ct = 0; ct < numberofcolumns; ct++)
                {
                    // look in tryck_mat & tryck_mat_a
                    byte boost_req_value = (byte)boost_request_map.GetValue((fct * numberofcolumns) + ct);
                    if (boost_req_value >= boostlimit)
                    {
                        if (showreport)
                        {
                            resume.AddToResumeTable("Found anomaly! Boost request value higher than boost limiter (fuel cut value) in Tryck_mat");
                            resume.AddToResumeTable("--> row: " + fct.ToString() + " column: " + ct.ToString());
                        }
                    }
                    boost_req_value = (byte)boost_request_map_aut.GetValue((fct * numberofcolumns) + ct);
                    if (boost_req_value >= boostlimit)
                    {
                        if (showreport)
                        {
                            resume.AddToResumeTable("Found anomaly! Boost request value higher than boost limiter (fuel cut value) in Tryck_mat_a");
                            resume.AddToResumeTable("--> row: " + fct.ToString() + " column: " + ct.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks is fuelknock map values are higher for every load/site than injection map
        /// </summary>
        /// <param name="filename"></param>
        private void CheckInjectionMapAgainstFuelKnockMap(string filename, bool showreport, bool fixproblems, Trionic5Resume resume)
        {
            bool changes_made = false;
            byte[] fuel_injection_map = readdatafromfile(filename, GetSymbolAddress( "Insp_mat!"), GetSymbolLength( "Insp_mat!"));
            byte[] fuel_knock_map = readdatafromfile(filename, GetSymbolAddress( "Fuel_knock_mat!"), GetSymbolLength( "Fuel_knock_mat!"));
            byte[] fuel_injection_xaxis = readdatafromfile(filename, GetSymbolAddress( "Fuel_map_xaxis!"), GetSymbolLength( "Fuel_map_xaxis!"));
            byte[] fuel_knock_xaxis = readdatafromfile(filename, GetSymbolAddress( "Fuel_knock_xaxis!"), GetSymbolLength( "Fuel_knock_xaxis!"));
            byte[] fuel_knock_yaxis = readdatafromfile(filename, GetSymbolAddress( "Fuel_map_yaxis!"), GetSymbolLength( "Fuel_map_yaxis!"));

            // t5.2 insp_mat = 16 * 16
            // t5.2 fuel_knock_mat = 8 * 16

            // t5.5 insp_mat = 16 * 16
            // t5.5 fuel_knock_mat = 12 * 16

            int fuel_numberrows = 16; // always 16 rows
            int fuel_numbercolumns = fuel_injection_map.Length / fuel_numberrows;


            int numberrows = 16; // always 16 rows
            int numbercolumns = fuel_knock_map.Length / numberrows;

            // handle all rows
            for (int rt = 0; rt < numberrows; rt++)
            {
                // and all columns   
                for (int ct = 0; ct < numbercolumns; ct++)
                {
                    byte fuel_knock_byte = (byte)fuel_knock_map.GetValue((rt * numbercolumns) + ct);
                    // fetch pressure & rpm for this cell
                    byte fuel_knock_pressure = (byte)fuel_knock_xaxis.GetValue(ct);

                    // now find the nearest column in fuel_map_xaxis
                    double diff = 255;
                    int idx_found = -1;
                    for (int xt = 0; xt < fuel_injection_xaxis.Length; xt++)
                    {
                        byte fuel_map_pressure_temp = (byte)fuel_injection_xaxis.GetValue(xt);
                        double tempdiff = Math.Abs((double)fuel_knock_pressure - (double)fuel_map_pressure_temp);
                        if (tempdiff < diff)
                        {
                            idx_found = xt;
                            diff = tempdiff;
                        }
                    }
                    if (idx_found >= 0)
                    {
                        // found it, we can compare
                        byte fuel_map_byte = (byte)fuel_injection_map.GetValue((rt * fuel_numbercolumns) + idx_found);
                        if (fuel_map_byte >= fuel_knock_byte)
                        {
                            // ANOMALY!!
                            if (showreport)
                            {
                                resume.AddToResumeTable("Found anomaly! Fuel injection map value larger than or equal to knock map");
                            }
                            if (fixproblems)
                            {
                                if (fuel_knock_byte < 255)
                                {
                                    fuel_knock_byte = fuel_map_byte;
                                    for (int corrt = 0; corrt < 5; corrt++)
                                    {
                                        if (fuel_knock_byte < 255) fuel_knock_byte++;
                                    }
                                    // write to symbol
                                    fuel_knock_map.SetValue(fuel_knock_byte, (rt * numbercolumns) + ct);
                                    resume.AddToResumeTable("Adjusted a value in the knock fuel matrix");
                                    changes_made = true;
                                }
                            }

                            double pressure = (double)fuel_knock_pressure;
                            pressure *= GetMapCorrectionFactor("Tryck_mat!");
                            pressure += GetMapCorrectionOffset("Tryck_mat!");

                            try
                            {
                                int rpm = Convert.ToInt32(fuel_knock_yaxis.GetValue(rt * 2)) * 256;
                                rpm += Convert.ToInt32(fuel_knock_yaxis.GetValue((rt * 2) + 1));
                                rpm *= 10;
                                if (showreport)
                                {
                                    resume.AddToResumeTable("--> pressure = " + pressure.ToString() + " bar, rpm = " + rpm.ToString());
                                }
                            }
                            catch (Exception E)
                            {
                                Console.WriteLine(E.Message);
                            }
                        }
                    }
                }
            }
            if (fixproblems && changes_made)
            {
                savedatatobinary(GetSymbolAddress( "Fuel_knock_mat!"), GetSymbolLength( "Fuel_knock_mat!"), fuel_knock_map, filename);
            }
        }

        private void savedatatobinary(int address, int length, byte[] data, string filename)
        {
            if (address <= 0) return;
            FileStream fsi1 = File.OpenWrite(filename);
            BinaryWriter bw1 = new BinaryWriter(fsi1);
            fsi1.Position = address;

            for (int i = 0; i < length; i++)
            {
                bw1.Write((byte)data.GetValue(i));
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();
        }

        private double GetMapCorrectionOffset(string symbolname)
        {
            double returnvalue = 0;
            if (symbolname.StartsWith("Ign_map_0!")) returnvalue = 0;
            else if (symbolname.StartsWith("Insp_mat!")) returnvalue = 0.5; // 128/256
            else if (symbolname.StartsWith("Idle_fuel_korr!")) returnvalue = 0.5; // 128/256
            //else if (symbolname.StartsWith("Accel_konst!")) returnvalue = 0.75; // 128/256
            else if (symbolname.StartsWith("Fuel_knock_mat!")) returnvalue = 0.5; // 128/256
            else if (symbolname.StartsWith("Adapt_korr!")) returnvalue = 0.75; // 384/512
            else if (symbolname == "Adapt_korr") returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_ref!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_ref")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_injfaktor!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_inj_imat!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_injfaktor_high!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_injfaktor_low!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_korr_high!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Adapt_korr_low!")) returnvalue = 0.75; // 384/512
            else if (symbolname.StartsWith("Cyl_komp!")) returnvalue = 0.75; // 384/512

            else if (symbolname.StartsWith("Lambdaint!")) returnvalue = 0.75; // 1/512
            else if (symbolname.StartsWith("Lacc_konst!")) returnvalue = 1; // 256/256
            else if (symbolname.StartsWith("Lret_konst!")) returnvalue = 1; // 256/256
            else if (symbolname.StartsWith("Accel_konst!")) returnvalue = 1; // 128/256
            else if (symbolname.StartsWith("Retard_konst!")) returnvalue = 1; // 128/256
            else if (symbolname.StartsWith("Hot_start_fak!")) returnvalue = 1; // 128/256
            else if (symbolname.StartsWith("Ret_fuel_fak!")) returnvalue = 1; // 128/256
            else if (symbolname.StartsWith("Ret_fuel_tab!")) returnvalue = 1; // 128/256

            else if (symbolname.StartsWith("Ign_map_4!")) returnvalue = 0;
            //else if (symbolname.StartsWith("Insp_mat!")) returnvalue = 0;
            //else if (symbolname.StartsWith("Accel_konst!")) returnvalue = 0;
            else if (symbolname.StartsWith("Del_mat!")) returnvalue = 0;
            else if (symbolname.StartsWith("Tryck_mat_a!")) returnvalue = -1;
            else if (symbolname.StartsWith("P_Manifold")) returnvalue = -1;
            else if (symbolname.StartsWith("Eftersta_fak!")) returnvalue = 1;
            else if (symbolname.StartsWith("Eftersta_fak2!")) returnvalue = 1;
            else if (symbolname.StartsWith("After_fcut_tab!")) returnvalue = 1;
            else if (symbolname.StartsWith("Hot_tab!")) returnvalue = 1;
            //else if (symbolname.StartsWith("Fload_tab!")) returnvalue = 1;
            else if (symbolname.StartsWith("Fload_tab!")) returnvalue = 1;
            else if (symbolname.StartsWith("Tryck_mat!")) returnvalue = -1;
            else if (symbolname.StartsWith("Knock_press_tab!")) returnvalue = -1;
            else if (symbolname.StartsWith("Knock_press!")) returnvalue = -1;
            else if (symbolname.StartsWith("Limp_tryck_konst!")) returnvalue = -1;
            else if (symbolname.StartsWith("Idle_tryck!")) returnvalue = -1;
            else if (symbolname.StartsWith("Tryck_vakt_tab!")) returnvalue = -1;
            else if (symbolname.StartsWith("Regl_tryck")) returnvalue = -1;
            else if (symbolname.StartsWith("Pressure map scaled for 3 bar mapsensor")) returnvalue = -1;
            else if (symbolname.StartsWith("Pressure map (AUT) scaled for 3 bar mapsensor")) returnvalue = -1;
            else if (symbolname.StartsWith("Knock_press_lim")) returnvalue = -1; // bar
            else if (symbolname.StartsWith("Turbo_knock_press")) returnvalue = -1; // bar

            else if (symbolname.StartsWith("Turbo_knock_tab")) returnvalue = -1;
            else if (symbolname.StartsWith("Open_loop_knock")) returnvalue = -1;
            else if (symbolname.StartsWith("Open_loop")) returnvalue = -1;
            else if (symbolname.StartsWith("Sond_heat_tab")) returnvalue = -1;

            else if (symbolname.StartsWith("Reg_last!")) returnvalue = 0;
            else if (symbolname.StartsWith("Idle_st_last!")) returnvalue = -1;
            //else if (symbolname.StartsWith("Last_temp_st!")) returnvalue = -1;
            else if (symbolname.StartsWith("Lam_minlast!")) returnvalue = -1;
            else if (symbolname.StartsWith("Lam_laststeg!")) returnvalue = -1;
            else if (symbolname.StartsWith("Grund_last!")) returnvalue = -1;
            else if (symbolname.StartsWith("Max_ratio_aut!")) returnvalue = -1;
            else if (symbolname.StartsWith("Diag_speed_load!")) returnvalue = -1;

            return returnvalue;

        }

        private double GetMapCorrectionFactor(string symbolname)
        {
            double returnvalue = 1;
            if (symbolname.StartsWith("Ign_map_0!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Apc_knock_tab!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Knock_lim_tab!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Knock_average")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Knock_lim")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Knock_ang_dec!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Knock_average_tab!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Knock_press_lim")) returnvalue = 0.01; // bar
            else if (symbolname.StartsWith("Turbo_knock_press")) returnvalue = 0.01; // bar
            else if (symbolname.StartsWith("Ign_idle_angle_start")) returnvalue = 0.1;


            else if (symbolname.StartsWith("Batt_korr_tab!")) returnvalue = 0.004; // 1/250
            else if (symbolname.StartsWith("Gear_st!")) returnvalue = 0.1; // 1/ ((256*256) / 260)
            else if (symbolname.StartsWith("Start_insp!")) returnvalue = 0.004; // 1/ ((256*256) / 260)
            //else if (symbolname.StartsWith("AC_wait_on!")) returnvalue = 0.25; // 1/ ((256*256) / 260)
            //else if (symbolname.StartsWith("AC_wait_off!")) returnvalue = 0.25; // 1/ ((256*256) / 260)
            else if (symbolname.StartsWith("Startvev_fak!")) returnvalue = 0.125; // 1/8
            else if (symbolname.StartsWith("After_fcut_tab!")) returnvalue = 0.0009765625; // 1/1024
            else if (symbolname.StartsWith("Hot_tab!")) returnvalue = 0.0009765625; // 1/1024
            //else if (symbolname.StartsWith("Hot_decr!")) returnvalue = 10; // 1/1024
            else if (symbolname.StartsWith("Idle_fuel_korr!")) returnvalue = 0.00390625; // 1/256
            else if (symbolname.StartsWith("Insp_mat!")) returnvalue = 0.00390625; // 1/256
            else if (symbolname.StartsWith("Fuel_knock_mat!")) returnvalue = 0.00390625; // 1/256
            else if (symbolname.StartsWith("Fload_tab!")) returnvalue = 0.001953125; // 1/512

            else if (symbolname == "Adapt_korr") returnvalue = 0.001953125; // 1/512
            else if (symbolname == "Adapt_korr!") returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_ref!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_ref")) returnvalue = 0.001953125; // 1/512

            else if (symbolname.StartsWith("Accel_konst!")) returnvalue = 0.00390625;//returnvalue = 0.0078125; // 1/128
            else if (symbolname.StartsWith("Start_proc!")) returnvalue = 0.0078125; // 1/128
            else if (symbolname.StartsWith("Cyl_komp!")) returnvalue = 0.001953125; // 1/512
            //Cylinder Compensation: (Cyl_komp+384)/512
            else if (symbolname.StartsWith("Retard_konst!")) returnvalue = 0.00390625;//returnvalue = 0.0078125; // 1/128
            else if (symbolname.StartsWith("Lacc_konst!")) returnvalue = 0.00390625; // 1/256 //0.0078125; // 1/128
            else if (symbolname.StartsWith("Lret_konst!")) returnvalue = 0.00390625; // 1/256 //0.0078125; // 1/128
            else if (symbolname.StartsWith("Hot_start_fak!")) returnvalue = 0.0009765625; // 128/256
            else if (symbolname.StartsWith("Ret_fuel_fak!")) returnvalue = 0.0009765625; // 128/256
            else if (symbolname.StartsWith("Ret_fuel_tab!")) returnvalue = 0.0009765625; // 128/256

            else if (symbolname.StartsWith("Adapt_injfaktor!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_inj_imat!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_injfaktor_high!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_injfaktor_low!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_korr_high!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Adapt_korr_low!")) returnvalue = 0.001953125; // 1/512
            else if (symbolname.StartsWith("Eftersta_fak!")) returnvalue = 0.0078125;// 0.01;
            else if (symbolname.StartsWith("Eftersta_fak2!")) returnvalue = 0.0078125;//0.01;
            else if (symbolname.StartsWith("Ign_map_1!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_2!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_3!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_4!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_5!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_6!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_7!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Ign_map_8!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Lamd_tid!")) returnvalue = 10;
            else if (symbolname.StartsWith("Del_mat!")) returnvalue = 3;

            else if (symbolname.StartsWith("Reg_kon_mat"))
            {
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    returnvalue = 1;
                }
                else
                {
                    returnvalue = 0.1;
                }
            }
            //else if (symbolname.StartsWith("Insp_mat!")) returnvalue = 1;
            //else if (symbolname.StartsWith("Del_mat!")) returnvalue = 1;
            else if (symbolname.StartsWith("Tryck_mat_a!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Tryck_mat!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("P_Manifold10")) returnvalue = 0.001;
            else if (symbolname.StartsWith("P_Manifold")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Max_ratio_aut!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Diag_speed_load!")) returnvalue = 0.01;

            else if (symbolname.StartsWith("Reg_last!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Idle_st_last!")) returnvalue = 0.01;
            //else if (symbolname.StartsWith("Last_temp_st!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Lam_minlast!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Lam_laststeg!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Grund_last!")) returnvalue = 0.01;


            else if (symbolname.StartsWith("Turbo_knock_tab")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Open_loop_knock")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Open_loop")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Sond_heat_tab")) returnvalue = 0.01;


            else if (symbolname.StartsWith("Knock_press_tab!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Knock_press!")) returnvalue = 0.01;

            else if (symbolname.StartsWith("Limp_tryck_konst!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Idle_tryck!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Tryck_vakt_tab!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Regl_tryck")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Pressure map scaled for 3 bar mapsensor")) returnvalue = 0.012;
            else if (symbolname.StartsWith("Pressure map (AUT) scaled for 3 bar mapsensor")) returnvalue = 0.012;
            else if (symbolname.StartsWith("Rpm_max!")) returnvalue = 10;
            else if (symbolname.StartsWith("Kadapt_rpm_high!")) returnvalue = 10;
            else if (symbolname.StartsWith("Kadapt_rpm_low!")) returnvalue = 10;
            else if (symbolname.StartsWith("Derivata_grans!")) returnvalue = 10;
            else if (symbolname.StartsWith("Min_rpm_closed_loop!")) returnvalue = 10;
            else if (symbolname.StartsWith("Min_rpm_gadapt!")) returnvalue = 10;
            else if (symbolname.StartsWith("Max_rpm_gadapt!")) returnvalue = 10;
            else if (symbolname.StartsWith("Ign_idle_angle!")) returnvalue = 0.1;
            else if (symbolname.StartsWith("Derivata_fuel_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Ret_delta_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Ret_down_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Ret_up_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Open_all_varv!")) returnvalue = 10;
            else if (symbolname.StartsWith("Open_varv!")) returnvalue = 10;
            else if (symbolname.StartsWith("Start_detekt_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Press_rpm_lim!")) returnvalue = 10;
            else if (symbolname.StartsWith("Rpm_dif!")) returnvalue = 10;
            else if (symbolname.StartsWith("Rpm_perf_max!")) returnvalue = 10;
            else if (symbolname.StartsWith("Rpm_perf_min!")) returnvalue = 10;
            else if (symbolname.StartsWith("Diag_speed_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Dash_rpm_axis!")) returnvalue = 10;
            else if (symbolname.StartsWith("Idle_st_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Idle_rpm_tab!")) returnvalue = 10;
            else if (symbolname.StartsWith("Knock_wind_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("PMCal_RpmIdleNomRefLim!")) returnvalue = 10;
            else if (symbolname.StartsWith("Pwm_ind_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Ap_max_rpm!")) returnvalue = 10;
            else if (symbolname.StartsWith("Lam_rpm_steg!")) returnvalue = 10; // ??
            else if (symbolname.StartsWith("Last_varv_st!")) returnvalue = 10; // ??
            else if (symbolname.StartsWith("Lambdaint!")) returnvalue = 0.001953125; // 1/512
            ///else if (symbolname.StartsWith("Pulses_per_rev!")) returnvalue = 0.1;
            return returnvalue;
        }

        /// <summary>
        /// checks injector constant value: should be over 5 and under 25
        /// </summary>
        /// <param name="filename"></param>
        private void CheckInjectionConstant(string filename, bool showreport, Trionic5Resume resume)
        {
            byte[] injector_constant = readdatafromfile(filename, GetSymbolAddress( "Inj_konst!"), GetSymbolLength( "Inj_konst!"));
            byte b = (byte)injector_constant.GetValue(0);
            if (b <= 5 || b > 25)
            {
                if (showreport)
                {
                    resume.AddToResumeTable("Found anomaly! Injector constant has an invalid value: " + b.ToString());
                }
            }

        }
    }
}
