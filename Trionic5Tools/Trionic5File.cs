using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using CommonSuite;

namespace Trionic5Tools
{

    public enum PGMStatusbit : int
    {
        Afterstartenrichment,               // byte 0, bit 1
        WOTenrichment,                      // byte 0, bit 2
        Interpolationofdelay,               // byte 0, bit 3
        Temperaturecompensation,            // byte 0, bit 4
        Lambdacontrol,                      // byte 0, bit 5
        Adaptivity,                         // byte 0, bit 6
        Idlecontrol,                        // byte 0, bit 7
        Enrichmentduringstart,              // byte 0, bit 8
        ConstantinjectiontimeE51,           // byte 1, bit 1
        Lambdacontrolduringtransients,      // byte 1, bit 2
        Fuelcut,                            // byte 1, bit 3
        Constantinjtimeduringidle,          // byte 1, bit 4
        Accelerationsenrichment,            // byte 1, bit 5
        Decelerationsenleanment,            // byte 1, bit 6
        Car104,                             // byte 1, bit 7
        Adaptivitywithclosedthrottle,       // byte 1, bit 8
        Factortolambdawhenthrottleopening,  // byte 2, bit 1
        Usesseparateinjmapduringidle,       // byte 2, bit 2
        FactortolambdawhenACisengaged,      // byte 2, bit 3
        ThrottleAccRetadjustsimultMY95,     // byte 2, bit 4
        Fueladjustingduringidle,            // byte 2, bit 5
        Purge,                              // byte 2, bit 6
        Adaptionofidlecontrol,              // byte 2, bit 7
        Lambdacontrolduringidle,            // byte 2, bit 8
        Heatedplates,                       // byte 3, bit 1
        AutomaticTransmission,              // byte 3, bit 2
        Loadcontrol,                        // byte 3, bit 3
        ETS,                                // byte 3, bit 4
        APCcontrol,                         // byte 3, bit 5
        Higheridleduringstart,              // byte 3, bit 6
        Globaladaption,                     // byte 3, bit 7
        Tempcompwithactivelambdacontrol,    // byte 3, bit 8
        Loadbufferduringidle,               // byte 4, bit 1
        Constidleignangleduringgearoneandtwo,// byte 4, bit 2
        NofuelcutR12,                       // byte 4, bit 3
        Airpumpcontrol,                     // byte 4, bit 4
        Normalasperatedengine,              // byte 4, bit 5
        Knockregulatingdisabled,            // byte 4, bit 6
        Constantangle,                      // byte 4, bit 7
        PurgevalveMY94                      // byte 4, bit 8
    }

    public enum InjectorType : int
    {
        Stock,
        GreenGiants,
        Siemens630Dekas,
        Siemens875Dekas,
        Siemens1000cc
    }

    public enum BPCType : int
    {
        Trionic5Valve,
        Trionic7Valve
    }

    public enum TuningStage : int
    {
        Stock,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Stage7,
        Stage8,
        StageX
    }

    public enum CPUFrequency : int
    {
        CPUFrequency16MHZ,
        CPUFrequency20Mhz
    }

    public class Trionic5File : IECUFile
    {
        private string _libraryPath = string.Empty;

        public override string LibraryPath
        {
            get { return _libraryPath; }
            set { _libraryPath = value; }
        }
        private System.Data.DataTable indexedSymbolList;
        private Trionic5FileInformation m_fileInfo = new Trionic5FileInformation();
        private TrionicTransactionLog m_transactionLog = null;

        private bool m_autoUpdateChecksum = false;

        public bool AutoUpdateChecksum
        {
            get { return m_autoUpdateChecksum; }
            set { m_autoUpdateChecksum = value; }
        }

        public override void SetTransactionLog(TrionicTransactionLog transactionLog)
        {
            m_transactionLog = transactionLog;
        }

        

        public override void SetAutoUpdateChecksum(bool autoUpdate)
        {
            m_autoUpdateChecksum = autoUpdate;
        }

        private DataTable resumeTuning;
        private string m_currentFile = string.Empty;

        private void AddToResumeTable(string entry)
        {
            resumeTuning.Rows.Add(entry);
        }

        private void CreateNewResume()
        {
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
        }

        /// <summary>
        /// Checks is fuelknock map values are higher for every load/site than injection map
        /// </summary>
        /// <param name="filename"></param>
        private void CheckInjectionMapAgainstFuelKnockMap(string filename, bool showreport, bool fixproblems)
        {
            bool changes_made = false;
            byte[] fuel_injection_map = readdatafromfile(filename, GetSymbolAddress("Insp_mat!"), GetSymbolLength( "Insp_mat!"));
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
                                AddToResumeTable("Found anomaly! Fuel injection map value larger than or equal to knock map");
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
                                    AddToResumeTable("Adjusted a value in the knock fuel matrix");
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
                                    AddToResumeTable("--> pressure = " + pressure.ToString() + " bar, rpm = " + rpm.ToString());
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
                WriteData(fuel_knock_map, (uint)GetSymbolAddress("Fuel_knock_mat!"));
            }
        }

        /// <summary>
        /// checks injector constant value: should be over 5 and under 25
        /// </summary>
        /// <param name="filename"></param>
        private void CheckInjectionConstant(string filename, bool showreport)
        {
            byte[] injector_constant = readdatafromfile(filename, GetSymbolAddress( "Inj_konst!"), GetSymbolLength( "Inj_konst!"));
            byte b = (byte)injector_constant.GetValue(0);
            if (b <= 5 || b > 25)
            {
                if (showreport)
                {
                    AddToResumeTable("Found anomaly! Injector constant has an invalid value: " + b.ToString());
                }
            }

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


        public override int GetMaxInjection()
        {

            byte[] injection_map = readdatafromfile(m_currentFile, GetSymbolAddress(m_fileInfo.GetInjectionMap()), GetSymbolLength(m_fileInfo.GetInjectionMap()));
            int retval = 0;
            foreach (byte b in injection_map)
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
        private void CheckBoostRequestAgainstAxisRanges(string filename, bool showreport)
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
            if (showreport)
            {
                float realBoostMax = maxboostvalue;
                MapSensorType mst = GetMapSensorType(true);
                if (mst == MapSensorType.MapSensor30)
                {
                    realBoostMax *= 1.2F;
                }
                else if (mst == MapSensorType.MapSensor35)
                {
                    realBoostMax *= 1.4F;
                }
                else if (mst == MapSensorType.MapSensor40)
                {
                    realBoostMax *= 1.6F;
                }
                else if (mst == MapSensorType.MapSensor50)
                {
                    realBoostMax *= 2.0F;
                }
                realBoostMax /= 100;
                realBoostMax -= 1;
                
                
                AddToResumeTable("Maximum boost request: " + realBoostMax.ToString("F2") + " bar");

            }
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Detect_map_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_0_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_2_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_3_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_6_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Ign_map_7_x_axis!");
            CheckSixteenBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Misfire_map_x_axis!");
            CheckEigthBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Fuel_knock_xaxis!");
            CheckEigthBitAxisAgainstBoostPressure(filename, showreport, maxboostvalue, "Fuel_map_xaxis!");

        }

        private void CheckEigthBitAxisAgainstBoostPressure(string filename, bool showreport, int maxboostvalue, string symbolname)
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
                AddToResumeTable(symbolname + " does not support the maximum boost request value!");
            }
        }

        private void CheckSixteenBitAxisAgainstBoostPressure(string filename, bool showreport, int maxboostvalue, string symbolname)
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
                AddToResumeTable(symbolname + " does not support the maximum boost request value!");
            }
        }

        /// <summary>
        /// Check boost request maps (Tryck_mat & Tryck_mat_a) against limiters
        /// </summary>
        /// <param name="filename"></param>
        private void CheckBoostRequestMapAgainstBoostLimiters(string filename, bool showreport)
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
                            AddToResumeTable("Found anomaly! Boost request value higher than boost limiter (fuel cut value) in Tryck_mat");
                            AddToResumeTable("--> row: " + fct.ToString() + " column: " + ct.ToString());
                        }
                    }
                    boost_req_value = (byte)boost_request_map_aut.GetValue((fct * numberofcolumns) + ct);
                    if (boost_req_value >= boostlimit)
                    {
                        if (showreport)
                        {
                            AddToResumeTable("Found anomaly! Boost request value higher than boost limiter (fuel cut value) in Tryck_mat_a");
                            AddToResumeTable("--> row: " + fct.ToString() + " column: " + ct.ToString());
                        }
                    }
                }
            }
        }


        public override DataTable CheckForAnomalies()
        {
            // check the file for inconsistancies
            CreateNewResume();
            AddToResumeTable("Checking file " + Path.GetFileName(m_currentFile));
            AddToResumeTable("Checking injection map against fuel knock map");
            CheckInjectionMapAgainstFuelKnockMap(m_currentFile, true, false);
            AddToResumeTable("Checking injection constant value");
            CheckInjectionConstant(m_currentFile, true);
            AddToResumeTable("Checking boost request maps agains boost limiters");
            CheckBoostRequestMapAgainstBoostLimiters(m_currentFile, true);
            AddToResumeTable("Checking axis against maximum requested boost level");
            CheckBoostRequestAgainstAxisRanges(m_currentFile, true);
            // seperator for next file... maybe
            AddToResumeTable("");
            AddToResumeTable("");
            return resumeTuning.Copy();
        }

        public override Trionic5Properties GetTrionicProperties()
        {
            //return a Trionic5Properties class with all relevant info
            Trionic5Properties _props = new Trionic5Properties();
            if (DetermineFrequency(m_fileInfo.Filename) == CPUFrequency.CPUFrequency16MHZ)
            {
                _props.CPUspeed = "16 Mhz";
            }
            else
            {
                _props.CPUspeed = "20 Mhz";
            }

            _props.HardcodedRPMLimit = GetHardcodedRPMLimit(m_currentFile);

            _props.Partnumber = readpartnumber(m_currentFile);
            _props.Carmodel = readcarmodel();
            _props.Enginetype = readenginetype();
            _props.Dataname = readdataname();
            _props.Partnumber = GetPartnumber();
            _props.SoftwareID = GetSoftwareVersion();
            _props.RAMlocked = readramlockedflag();
            _props.SecondO2Enable = readsecondO2sensorenabledflag();
            if (GetSymbolLength("Pgm_mod!") > 5)
            {
                _props.VSSCode = readimmocode();
                _props.VSSactive = GetVSSEnabled();
                _props.Tank_diagnosticsactive = GetTankPressureDiagnosticsEnabled();
                _props.HasVSSOptions = true;
            }
            else
            {
                _props.VSSCode = "-----";
                _props.VSSactive = false;
                _props.Tank_diagnosticsactive = false;
                _props.HasVSSOptions = false;
            }
            if (GetSymbolLength("Pgm_mod!") > 4)
            {
                _props.ExtendedProgramModeOptions = true;
            }
            else
            {
                _props.ExtendedProgramModeOptions = false;
            }
            

            // bits and bytes
            _props.AutomaticTransmission = GetAutoGearbox();
            _props.Heatedplates = GetHeatplates();
            _props.Accelerationsenrichment = GetPgmStatusValue(PGMStatusbit.Accelerationsenrichment);
            _props.Adaptionofidlecontrol = GetPgmStatusValue(PGMStatusbit.Adaptionofidlecontrol);
            _props.Adaptivitywithclosedthrottle = GetPgmStatusValue(PGMStatusbit.Adaptivitywithclosedthrottle);
            _props.Adaptivity = GetPgmStatusValue(PGMStatusbit.Adaptivity);
            _props.Afterstartenrichment = GetPgmStatusValue(PGMStatusbit.Afterstartenrichment);
            _props.APCcontrol = GetPgmStatusValue(PGMStatusbit.APCcontrol);
            _props.ConstantinjectiontimeE51 = GetPgmStatusValue(PGMStatusbit.ConstantinjectiontimeE51);
            _props.Constantinjtimeduringidle = GetPgmStatusValue(PGMStatusbit.Constantinjtimeduringidle);
            _props.Decelerationsenleanment = GetPgmStatusValue(PGMStatusbit.Decelerationsenleanment);
            _props.Enrichmentduringstart = GetPgmStatusValue(PGMStatusbit.Enrichmentduringstart);
            _props.ETS = GetPgmStatusValue(PGMStatusbit.ETS);
            _props.FactortolambdawhenACisengaged = GetPgmStatusValue(PGMStatusbit.FactortolambdawhenACisengaged);
            _props.Factortolambdawhenthrottleopening = GetPgmStatusValue(PGMStatusbit.Factortolambdawhenthrottleopening);
            _props.Fueladjustingduringidle = GetPgmStatusValue(PGMStatusbit.Fueladjustingduringidle);
            _props.Fuelcut = GetPgmStatusValue(PGMStatusbit.Fuelcut);
            _props.Globaladaption = GetPgmStatusValue(PGMStatusbit.Globaladaption);
            _props.Higheridleduringstart = GetPgmStatusValue(PGMStatusbit.Higheridleduringstart);
            _props.Idlecontrol = GetPgmStatusValue(PGMStatusbit.Idlecontrol);
            _props.Interpolationofdelay = GetPgmStatusValue(PGMStatusbit.Interpolationofdelay);
            _props.Lambdacontrol = GetPgmStatusValue(PGMStatusbit.Lambdacontrol);
            _props.Lambdacontrolduringidle = GetPgmStatusValue(PGMStatusbit.Lambdacontrolduringidle);
            _props.Lambdacontrolduringtransients = GetPgmStatusValue(PGMStatusbit.Lambdacontrolduringtransients);
            if (GetSymbolLength("Pgm_mod!") > 4)
            {
                _props.Loadbufferduringidle = GetPgmStatusValue(PGMStatusbit.Loadbufferduringidle);
                _props.Constidleignangleduringgearoneandtwo = GetPgmStatusValue(PGMStatusbit.Constidleignangleduringgearoneandtwo);
                _props.NofuelcutR12 = GetPgmStatusValue(PGMStatusbit.NofuelcutR12);
                _props.Airpumpcontrol = GetPgmStatusValue(PGMStatusbit.Airpumpcontrol);
                _props.Normalasperatedengine = GetPgmStatusValue(PGMStatusbit.Normalasperatedengine);
                _props.Knockregulatingdisabled = GetPgmStatusValue(PGMStatusbit.Knockregulatingdisabled);
                _props.Constantangle = GetPgmStatusValue(PGMStatusbit.Constantangle);
                _props.PurgevalveMY94 = GetPgmStatusValue(PGMStatusbit.PurgevalveMY94);
                _props.Loadcontrol = GetPgmStatusValue(PGMStatusbit.Loadcontrol);
                _props.Purge = GetPgmStatusValue(PGMStatusbit.Purge);
                _props.Tempcompwithactivelambdacontrol = GetPgmStatusValue(PGMStatusbit.Tempcompwithactivelambdacontrol);
                _props.Temperaturecompensation = GetPgmStatusValue(PGMStatusbit.Temperaturecompensation);
                _props.ThrottleAccRetadjustsimultMY95 = GetPgmStatusValue(PGMStatusbit.ThrottleAccRetadjustsimultMY95);
                _props.WOTenrichment = GetPgmStatusValue(PGMStatusbit.WOTenrichment);
                _props.Usesseparateinjmapduringidle = GetPgmStatusValue(PGMStatusbit.Usesseparateinjmapduringidle);
            }
            FileInfo _fi = new FileInfo(m_currentFile);
            if (_fi.Length == 0x40000)
            {
                _props.IsTrionic55 = true;
            }
            else
            {
                _props.IsTrionic55 = false;
            }
            _props.InjectorType = (InjectorType)ReadInjectorTypeMarker();
            _props.TurboType = (TurboType)ReadTurboTypeMarker();
            _props.MapSensorType = (MapSensorType)ReadThreeBarConversionMarker(m_currentFile);
            _props.TuningStage = (TuningStage)ReadTuningStageMarker();
            _props.SyncDateTime = GetMemorySyncDate();

            return _props;

        }

        public override bool HasSymbol(string symbolname)
        {
            bool retval = false;
            if (m_fileInfo.SymbolCollection != null)
            {
                foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
                {
                    if (sh.Varname == symbolname)
                    {
                        retval = true;
                        break;
                    }
                }
            }
            return retval;
        }

        

        public override void SetTrionicOptions(Trionic5Properties properties)
        {
            // save the settings in properties into the binary file
            //TODO: check which entries are changed and which are not.
            // non changed should not be saved
            Trionic5Properties _oriProperties = GetTrionicProperties();
    //        if (_oriProperties.Carmodel != properties.Carmodel) writecarmodel(properties.Carmodel);
            if(_oriProperties.Enginetype != properties.Enginetype) writeenginetype(properties.Enginetype);
            if(_oriProperties.Partnumber != properties.Partnumber) writepartnumber(properties.Partnumber);
            if(_oriProperties.SoftwareID != properties.SoftwareID) writesoftwareid(properties.SoftwareID);
            if(_oriProperties.VSSCode != properties.VSSCode) writeimmocode(properties.VSSCode);
            if(_oriProperties.Dataname != properties.Dataname) writedataname(properties.Dataname);
            if(_oriProperties.VSSactive != properties.VSSactive) SetVSSEnabled(properties.VSSactive);
            if (_oriProperties.Tank_diagnosticsactive != properties.Tank_diagnosticsactive) SetTankDiagnosticsEnabled(properties.Tank_diagnosticsactive);
            if(_oriProperties.AutomaticTransmission != properties.AutomaticTransmission) SetAutoGearboxEnabled(properties.AutomaticTransmission);
            if(_oriProperties.Heatedplates != properties.Heatedplates) SetHeatplatesEnabled(properties.Heatedplates);
            if(_oriProperties.Accelerationsenrichment != properties.Accelerationsenrichment) SetPgmStatusValue(PGMStatusbit.Accelerationsenrichment, properties.Accelerationsenrichment);
            if (_oriProperties.Adaptionofidlecontrol != properties.Adaptionofidlecontrol) SetPgmStatusValue(PGMStatusbit.Adaptionofidlecontrol, properties.Adaptionofidlecontrol);
            if (_oriProperties.Adaptivity != properties.Adaptivity) SetPgmStatusValue(PGMStatusbit.Adaptivity, properties.Adaptivity);
            if (_oriProperties.Adaptivitywithclosedthrottle != properties.Adaptivitywithclosedthrottle) SetPgmStatusValue(PGMStatusbit.Adaptivitywithclosedthrottle, properties.Adaptivitywithclosedthrottle);
            if (_oriProperties.Afterstartenrichment != properties.Afterstartenrichment) SetPgmStatusValue(PGMStatusbit.Afterstartenrichment, properties.Afterstartenrichment);
            if (_oriProperties.APCcontrol != properties.APCcontrol) SetPgmStatusValue(PGMStatusbit.APCcontrol, properties.APCcontrol);
            if (_oriProperties.ConstantinjectiontimeE51 != properties.ConstantinjectiontimeE51) SetPgmStatusValue(PGMStatusbit.ConstantinjectiontimeE51, properties.ConstantinjectiontimeE51);
            if (_oriProperties.Constantinjtimeduringidle != properties.Constantinjtimeduringidle) SetPgmStatusValue(PGMStatusbit.Constantinjtimeduringidle, properties.Constantinjtimeduringidle);
            if (_oriProperties.Decelerationsenleanment != properties.Decelerationsenleanment) SetPgmStatusValue(PGMStatusbit.Decelerationsenleanment, properties.Decelerationsenleanment);
            if (_oriProperties.Enrichmentduringstart != properties.Enrichmentduringstart) SetPgmStatusValue(PGMStatusbit.Enrichmentduringstart, properties.Enrichmentduringstart);
            if (_oriProperties.ETS != properties.ETS) SetPgmStatusValue(PGMStatusbit.ETS, properties.ETS);
            if (_oriProperties.FactortolambdawhenACisengaged != properties.FactortolambdawhenACisengaged) SetPgmStatusValue(PGMStatusbit.FactortolambdawhenACisengaged, properties.FactortolambdawhenACisengaged);
            if (_oriProperties.Factortolambdawhenthrottleopening != properties.Factortolambdawhenthrottleopening) SetPgmStatusValue(PGMStatusbit.Factortolambdawhenthrottleopening, properties.Factortolambdawhenthrottleopening);
            if (_oriProperties.Fueladjustingduringidle != properties.Fueladjustingduringidle) SetPgmStatusValue(PGMStatusbit.Fueladjustingduringidle, properties.Fueladjustingduringidle);
            if (_oriProperties.Fuelcut != properties.Fuelcut) SetPgmStatusValue(PGMStatusbit.Fuelcut, properties.Fuelcut);
            if (_oriProperties.Globaladaption != properties.Globaladaption) SetPgmStatusValue(PGMStatusbit.Globaladaption, properties.Globaladaption);
            if (_oriProperties.Higheridleduringstart != properties.Higheridleduringstart) SetPgmStatusValue(PGMStatusbit.Higheridleduringstart, properties.Higheridleduringstart);
            if (_oriProperties.Idlecontrol != properties.Idlecontrol) SetPgmStatusValue(PGMStatusbit.Idlecontrol, properties.Idlecontrol);
            if (_oriProperties.Interpolationofdelay != properties.Interpolationofdelay) SetPgmStatusValue(PGMStatusbit.Interpolationofdelay, properties.Interpolationofdelay);
            if (_oriProperties.Lambdacontrol != properties.Lambdacontrol) SetPgmStatusValue(PGMStatusbit.Lambdacontrol, properties.Lambdacontrol);
            if (_oriProperties.Lambdacontrolduringidle != properties.Lambdacontrolduringidle) SetPgmStatusValue(PGMStatusbit.Lambdacontrolduringidle, properties.Lambdacontrolduringidle);
            if (_oriProperties.Lambdacontrolduringtransients != properties.Lambdacontrolduringtransients) SetPgmStatusValue(PGMStatusbit.Lambdacontrolduringtransients, properties.Lambdacontrolduringtransients);
            if (_oriProperties.Loadcontrol != properties.Loadcontrol) SetPgmStatusValue(PGMStatusbit.Loadcontrol, properties.Loadcontrol);
            if (_oriProperties.Purge != properties.Purge) SetPgmStatusValue(PGMStatusbit.Purge, properties.Purge);
            if (_oriProperties.Tempcompwithactivelambdacontrol != properties.Tempcompwithactivelambdacontrol) SetPgmStatusValue(PGMStatusbit.Tempcompwithactivelambdacontrol, properties.Tempcompwithactivelambdacontrol);
            if (_oriProperties.Temperaturecompensation != properties.Temperaturecompensation) SetPgmStatusValue(PGMStatusbit.Temperaturecompensation, properties.Temperaturecompensation);
            if (_oriProperties.ThrottleAccRetadjustsimultMY95 != properties.ThrottleAccRetadjustsimultMY95) SetPgmStatusValue(PGMStatusbit.ThrottleAccRetadjustsimultMY95, properties.ThrottleAccRetadjustsimultMY95);
            if (_oriProperties.Usesseparateinjmapduringidle != properties.Usesseparateinjmapduringidle) SetPgmStatusValue(PGMStatusbit.Usesseparateinjmapduringidle, properties.Usesseparateinjmapduringidle);
            if (_oriProperties.WOTenrichment != properties.WOTenrichment) SetPgmStatusValue(PGMStatusbit.WOTenrichment, properties.WOTenrichment);
            if (GetSymbolLength("Pgm_mod!") > 4)
            {
                if (_oriProperties.Loadbufferduringidle != properties.Loadbufferduringidle) SetPgmStatusValue(PGMStatusbit.Loadbufferduringidle, properties.Loadbufferduringidle);
                if (_oriProperties.Constidleignangleduringgearoneandtwo != properties.Constidleignangleduringgearoneandtwo) SetPgmStatusValue(PGMStatusbit.Constidleignangleduringgearoneandtwo, properties.Constidleignangleduringgearoneandtwo);
                if (_oriProperties.NofuelcutR12 != properties.NofuelcutR12) SetPgmStatusValue(PGMStatusbit.NofuelcutR12, properties.NofuelcutR12);
                if (_oriProperties.Airpumpcontrol != properties.Airpumpcontrol) SetPgmStatusValue(PGMStatusbit.Airpumpcontrol, properties.Airpumpcontrol);
                if (_oriProperties.Normalasperatedengine != properties.Normalasperatedengine) SetPgmStatusValue(PGMStatusbit.Normalasperatedengine, properties.Normalasperatedengine);
                if (_oriProperties.Knockregulatingdisabled != properties.Knockregulatingdisabled) SetPgmStatusValue(PGMStatusbit.Knockregulatingdisabled, properties.Knockregulatingdisabled);
                if (_oriProperties.Constantangle != properties.Constantangle) SetPgmStatusValue(PGMStatusbit.Constantangle, properties.Constantangle);
                if (_oriProperties.PurgevalveMY94 != properties.PurgevalveMY94) SetPgmStatusValue(PGMStatusbit.PurgevalveMY94, properties.PurgevalveMY94);
            }
            if(_oriProperties.RAMlocked != properties.RAMlocked) writeramlockedflag(properties.RAMlocked);
            if (_oriProperties.SecondO2Enable != properties.SecondO2Enable) writesecondo2sensorenabled(properties.SecondO2Enable);
            if (_oriProperties.InjectorType != properties.InjectorType)
            {
                //TODO: we need to do something here ... injector indicator was changed.. ask user?
                // the injector type was changed, we need to check injector constant to verify settings (or better yet, injection duration)
                // if the injector duration is off, we need to do our magic and determine new settings based on old->new injector type
                
                WriteInjectorTypeMarker(properties.InjectorType);
            }
            if (_oriProperties.TurboType != properties.TurboType)
            {
                //TODO: we need to do something here ... turbo type changed, ask user? 
                WriteTurboTypeMarker(properties.TurboType);
                Trionic5Tuner tun = new Trionic5Tuner();

                tun.LiftBoostRequestForTurboType(properties.TurboType);
            }
            if (_oriProperties.TuningStage != properties.TuningStage)
            {
                //TODO: we need to do something here ... tuning stage changed, ask user? 
                // well, we might need to run the tuning wizard to set it to stage properties.TuningStage
                /*TuningStage stage = DetermineTuningStage();
                if (stage != properties.TuningStage)
                {
                    Trionic5Tuner tun = new Trionic5Tuner();
                    tun.TuneFileToStage((int)properties.TuningStage, m_currentFile, this, m_fileInfo, true);
                    Console.WriteLine("Silently tuned to stage " + properties.TuningStage.ToString());
                }*/
                WriteTuningStageMarker(properties.TuningStage);
            }
            if (_oriProperties.SyncDateTime != properties.SyncDateTime) SetMemorySyncDate(properties.SyncDateTime);
            if (_oriProperties.MapSensorType != properties.MapSensorType)
            {
                //TODO: we need to do something here ... mapsensor type changed, ask user?
                // we need to check settings in the binary here to verify whether the bin is already set for the chosen mapsensor type
                // if not, we need to do our magic here and update the bin to the new mapsensor type
                /*MapSensorType _detectedType = DetermineMapSensorType();
                if (_detectedType != properties.MapSensorType)
                {
                    // run a wizard in the background to match the selected mapsensor type
                    Trionic5Tuner tun = new Trionic5Tuner();
                    //tun.
                    //Console.WriteLine("Altering mapsensor type, running wizard " + _detectedType.ToString() + " to " + properties.MapSensorType.ToString());
                    tun.ConvertFileToThreeBarMapSensor(m_fileInfo, _detectedType, properties.MapSensorType);
                }*/
                WriteThreeBarConversionMarker(m_currentFile, properties.MapSensorType);
            }
            if (_oriProperties.HardcodedRPMLimit != properties.HardcodedRPMLimit)
            {
                SetHardcodedRPMLimit(m_currentFile, properties.HardcodedRPMLimit);
            }
            updatechecksum(m_currentFile);
        }

        #region Firmware settings

        public override TuningStage DetermineTuningStage(out float max_boostRequest)
        {
            max_boostRequest = 0;
            TuningStage _stage = TuningStage.Stock;
            // get boost request value @ 3000 rpm
             // <= 1 bar = stock
            // <= 1.15 = stage 1
            // <= 1.25 = stage 2
            // <= 1.35 = stage 3
            Int32 request = GetMaxBoostValue(m_currentFile);
            MapSensorType mst = GetMapSensorType(true);
            float realBoostMax = request;
            if (mst == MapSensorType.MapSensor30)
            {
                realBoostMax *= 1.2F;
            }
            else if (mst == MapSensorType.MapSensor35)
            {
                realBoostMax *= 1.4F;
            }
            else if (mst == MapSensorType.MapSensor40)
            {
                realBoostMax *= 1.6F;
            }
            else if (mst == MapSensorType.MapSensor50)
            {
                realBoostMax *= 2.0F;
            }
            realBoostMax /= 100;
            realBoostMax -= 1;
            if (realBoostMax <= 1) _stage = TuningStage.Stock;
            else if (realBoostMax <= 1.16) _stage = TuningStage.Stage1;
            else if (realBoostMax <= 1.26) _stage = TuningStage.Stage2;
            else if (realBoostMax <= 1.36) _stage = TuningStage.Stage3;
            else if (realBoostMax <= 1.5) _stage = TuningStage.Stage4;
            else if (realBoostMax <= 1.72) _stage = TuningStage.Stage5;
            else if (realBoostMax <= 1.81) _stage = TuningStage.Stage6;
            else if (realBoostMax <= 2.0) _stage = TuningStage.Stage7;
            else  _stage = TuningStage.Stage8;
            /*
            Int32 request = 0;
            byte[] tryck_mat = ReadData((uint)m_fileInfo.GetSymbolAddressFlash("Tryck_mat!"), (uint)m_fileInfo.GetSymbolLength("Tryck_mat!"));
            if (tryck_mat.Length >= 0x80)
            {
                request = Convert.ToInt32(tryck_mat.GetValue(0x3F));
                MapSensorType mst = DetermineMapSensorType();
                if (mst == MapSensorType.MapSensor30)
                {
                    // correct
                    request *= 120;
                    request /= 100;
                }
                else if (mst == MapSensorType.MapSensor35)
                {
                    // correct
                    request *= 140;
                    request /= 100;
                }
                if (mst == MapSensorType.MapSensor40)
                {
                    // correct
                    request *= 160;
                    request /= 100;
                }
                if (request <= 200) _stage = TuningStage.Stock;
                else if (request <= 215) _stage = TuningStage.Stage1;
                else if (request <= 225) _stage = TuningStage.Stage2;
                else if (request <= 235) _stage = TuningStage.Stage3;
            }*/
            max_boostRequest = realBoostMax;
            return _stage;
        }

        public override MapSensorType DetermineMapSensorType()
        {
            MapSensorType _type = MapSensorType.MapSensor25;
            // we just have to read the x axis for insp_mat and check the settings
            byte[] axis = ReadData((uint)m_fileInfo.GetSymbolAddressFlash("Fuel_map_xaxis!"), (uint)m_fileInfo.GetSymbolLength("Fuel_map_xaxis!"));
            int maxvalue = Convert.ToInt32(axis.GetValue(axis.Length-1));
            if (maxvalue == 240 || maxvalue == 224)
            {
                // stock mapsensor
            }
            else 
            {
                // 3, 3.5 or 4 bar sensor
                // check boost request map
                byte[] tryck_mat = ReadData((uint)m_fileInfo.GetSymbolAddressFlash("Tryck_mat!"), (uint)m_fileInfo.GetSymbolLength("Tryck_mat!"));
                if (tryck_mat.Length >= 0x80)
                {
                    // get byte in upper left corner (byte number = 0x78)
                    Int32 requestValue = Convert.ToInt32(tryck_mat.GetValue(0x78));
                    if (requestValue == 10)
                    {
                        // 4 bar sensor
                        _type = MapSensorType.MapSensor50;
                    }
                    else if (requestValue == 12)
                    {
                        // 4 bar sensor
                        _type = MapSensorType.MapSensor40;
                    }
                    else if (requestValue == 14)
                    {
                        _type = MapSensorType.MapSensor35;
                    }
                    else if (requestValue == 16)
                    {
                        _type = MapSensorType.MapSensor30;
                    }

                }
            }
            return _type;
        }

        private bool GetAutoGearbox()
        {
            bool retval = false;
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    int fileoffset = (int)sh.Flash_start_address;
                    //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                    byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                    byte sign = Convert.ToByte(vssdata.GetValue(3));
                    if ((sign & 0x02) > 0)
                    {
                        retval = true;
                    }
                }
            }
            return retval;
        }

        private void SetAutoGearboxEnabled(bool enable)
        {
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    int fileoffset = (int)sh.Flash_start_address;
                    //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                    byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                    byte sign = Convert.ToByte(vssdata.GetValue(3));
                    if (enable)
                    {
                        sign |= 0x02; // set autogearbox flag
                    }
                    else
                    {
                        sign &= 0xFD; // clear autogearbox flag
                    }
                    vssdata.SetValue(sign, 3);

                    WriteData(vssdata, (uint)fileoffset);
                }

            }
        }

        private bool GetHeatplates()
        {
            bool retval = false;
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    int fileoffset = (int)sh.Flash_start_address;
                    //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                    byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                    byte sign = Convert.ToByte(vssdata.GetValue(3));
                    if ((sign & 0x01) > 0)
                    {
                        retval = true;
                    }
                }
            }
            return retval;
        }

        private void SetHeatplatesEnabled(bool enable)
        {
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    int fileoffset = (int)sh.Flash_start_address;
                    //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                    byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                    byte sign = Convert.ToByte(vssdata.GetValue(3));
                    if (enable)
                    {
                        sign |= 0x01; // set autogearbox flag
                    }
                    else
                    {
                        sign &= 0xFE; // clear autogearbox flag
                    }
                    vssdata.SetValue(sign, 3);
                    WriteData(vssdata, (uint)fileoffset);
                }

            }
        }

        private bool GetPgmStatusValue(PGMStatusbit statusbit)
        {
            /*int enumvalue = (int)statusbit;
            int byteoffset = enumvalue / 8;*/
            bool retval = false;
            switch (statusbit)
            {
                case PGMStatusbit.Afterstartenrichment:
                    retval = GetPGMMODVar(0, 0x01);
                    // byte 0, bit 1
                    break;

                case PGMStatusbit.WOTenrichment:
                    retval = GetPGMMODVar(0, 0x02);
                    // byte 0, bit 2
                    break;
                case PGMStatusbit.Interpolationofdelay:
                    retval = GetPGMMODVar(0, 0x04);
                    // byte 0, bit 3
                    break;
                case PGMStatusbit.Temperaturecompensation:
                    retval = GetPGMMODVar(0, 0x08);
                    // byte 0, bit 4
                    break;
                case PGMStatusbit.Lambdacontrol:
                    retval = GetPGMMODVar(0, 0x10);
                    // byte 0, bit 5
                    break;
                case PGMStatusbit.Adaptivity:
                    retval = GetPGMMODVar(0, 0x20);
                    // byte 0, bit 6
                    break;
                case PGMStatusbit.Idlecontrol:
                    retval = GetPGMMODVar(0, 0x40);
                    // byte 0, bit 7
                    break;
                case PGMStatusbit.Enrichmentduringstart:
                    retval = GetPGMMODVar(0, 0x40);
                    // byte 0, bit 8
                    break;
                case PGMStatusbit.ConstantinjectiontimeE51:
                    retval = GetPGMMODVar(1, 0x01);
                    // byte 1, bit 1
                    break;
                case PGMStatusbit.Lambdacontrolduringtransients:
                    retval = GetPGMMODVar(1, 0x02);
                    // byte 1, bit 2
                    break;
                case PGMStatusbit.Fuelcut:
                    retval = GetPGMMODVar(1, 0x04);
                    // byte 1, bit 3
                    break;
                case PGMStatusbit.Constantinjtimeduringidle:
                    retval = GetPGMMODVar(1, 0x08);
                    // byte 1, bit 4
                    break;
                case PGMStatusbit.Accelerationsenrichment:
                    retval = GetPGMMODVar(1, 0x10);
                    // byte 1, bit 5
                    break;
                case PGMStatusbit.Decelerationsenleanment:
                    retval = GetPGMMODVar(1, 0x20);
                    // byte 1, bit 6
                    break;
                case PGMStatusbit.Car104:
                    retval = GetPGMMODVar(1, 0x40);
                    // byte 1, bit 7
                    break;
                case PGMStatusbit.Adaptivitywithclosedthrottle:
                    retval = GetPGMMODVar(1, 0x80);
                    // byte 1, bit 8
                    break;
                case PGMStatusbit.Factortolambdawhenthrottleopening:
                    retval = GetPGMMODVar(2, 0x01);
                    // byte 2, bit 1
                    break;
                case PGMStatusbit.Usesseparateinjmapduringidle:
                    retval = GetPGMMODVar(2, 0x02);
                    // byte 2, bit 2
                    break;
                case PGMStatusbit.FactortolambdawhenACisengaged:
                    retval = GetPGMMODVar(2, 0x04);
                    // byte 2, bit 3
                    break;
                case PGMStatusbit.ThrottleAccRetadjustsimultMY95:
                    retval = GetPGMMODVar(2, 0x08);
                    // byte 2, bit 4
                    break;
                case PGMStatusbit.Fueladjustingduringidle:
                    retval = GetPGMMODVar(2, 0x10);
                    // byte 2, bit 5
                    break;
                case PGMStatusbit.Purge:
                    retval = GetPGMMODVar(2, 0x20);
                    // byte 2, bit 6
                    break;
                case PGMStatusbit.Adaptionofidlecontrol:
                    retval = GetPGMMODVar(2, 0x40);
                    // byte 2, bit 7
                    break;
                case PGMStatusbit.Lambdacontrolduringidle:
                    retval = GetPGMMODVar(2, 0x80);
                    // byte 2, bit 8
                    break;
                case PGMStatusbit.Heatedplates:
                    retval = GetPGMMODVar(3, 0x01);
                    // byte 3, bit 1    
                    break;
                case PGMStatusbit.AutomaticTransmission:
                    retval = GetPGMMODVar(3, 0x02);
                    // byte 3, bit 2
                    break;
                case PGMStatusbit.Loadcontrol:
                    retval = GetPGMMODVar(3, 0x04);
                    // byte 3, bit 3
                    break;
                case PGMStatusbit.ETS:
                    retval = GetPGMMODVar(3, 0x08);
                    // byte 3, bit 4
                    break;
                case PGMStatusbit.APCcontrol:
                    retval = GetPGMMODVar(3, 0x10);
                    // byte 3, bit 5
                    break;
                case PGMStatusbit.Higheridleduringstart:
                    retval = GetPGMMODVar(3, 0x20);
                    // byte 3, bit 6
                    break;
                case PGMStatusbit.Globaladaption:
                    retval = GetPGMMODVar(3, 0x40);
                    // byte 3, bit 7
                    break;
                case PGMStatusbit.Tempcompwithactivelambdacontrol:
                    retval = GetPGMMODVar(3, 0x80);
                    // byte 3, bit 8    
                    break;
                case PGMStatusbit.Loadbufferduringidle:
                    retval = GetPGMMODVar(4, 0x01);
                    // byte 4, bit 1
                    break;
                case PGMStatusbit.Constidleignangleduringgearoneandtwo:
                    retval = GetPGMMODVar(4, 0x02);
                    // byte 4, bit 2
                    break;
                case PGMStatusbit.NofuelcutR12:
                    // byte 4, bit 3
                    retval = GetPGMMODVar(4, 0x04);
                    break;
                case PGMStatusbit.Airpumpcontrol:
                    retval = GetPGMMODVar(4, 0x08);
                    // byte 4, bit 4
                    break;
                case PGMStatusbit.Normalasperatedengine:
                    retval = GetPGMMODVar(4, 0x10);
                    // byte 4, bit 5
                    break;
                case PGMStatusbit.Knockregulatingdisabled:
                    retval = GetPGMMODVar(4, 0x20);
                    // byte 4, bit 6
                    break;
                case PGMStatusbit.Constantangle:
                    retval = GetPGMMODVar(4, 0x40);
                    // byte 4, bit 7
                    break;
                case PGMStatusbit.PurgevalveMY94:
                    retval = GetPGMMODVar(4, 0x80);
                    // byte 4, bit 8
                    break;
            }
            return retval;

        }

        private void SetPgmStatusValue(PGMStatusbit statusbit, bool enable)
        {
            /*int enumvalue = (int)statusbit;
            int byteoffset = enumvalue / 8;*/
            switch (statusbit)
            {
                case PGMStatusbit.Afterstartenrichment:
                    SetPGMMODVar(0, 0x01, enable);
                    // byte 0, bit 1
                    break;

                case PGMStatusbit.WOTenrichment:
                    SetPGMMODVar(0, 0x02, enable);
                    // byte 0, bit 2
                    break;
                case PGMStatusbit.Interpolationofdelay:
                    SetPGMMODVar(0, 0x04, enable);
                    // byte 0, bit 3
                    break;
                case PGMStatusbit.Temperaturecompensation:
                    SetPGMMODVar(0, 0x08, enable);
                    // byte 0, bit 4
                    break;
                case PGMStatusbit.Lambdacontrol:
                    SetPGMMODVar(0, 0x10, enable);
                    // byte 0, bit 5
                    break;
                case PGMStatusbit.Adaptivity:
                    SetPGMMODVar(0, 0x20, enable);
                    // byte 0, bit 6
                    break;
                case PGMStatusbit.Idlecontrol:
                    SetPGMMODVar(0, 0x40, enable);
                    // byte 0, bit 7
                    break;
                case PGMStatusbit.Enrichmentduringstart:
                    SetPGMMODVar(0, 0x40, enable);
                    // byte 0, bit 8
                    break;
                case PGMStatusbit.ConstantinjectiontimeE51:
                    SetPGMMODVar(1, 0x01, enable);
                    // byte 1, bit 1
                    break;
                case PGMStatusbit.Lambdacontrolduringtransients:
                    SetPGMMODVar(1, 0x02, enable);
                    // byte 1, bit 2
                    break;
                case PGMStatusbit.Fuelcut:
                    SetPGMMODVar(1, 0x04, enable);
                    // byte 1, bit 3
                    break;
                case PGMStatusbit.Constantinjtimeduringidle:
                    SetPGMMODVar(1, 0x08, enable);
                    // byte 1, bit 4
                    break;
                case PGMStatusbit.Accelerationsenrichment:
                    SetPGMMODVar(1, 0x10, enable);
                    // byte 1, bit 5
                    break;
                case PGMStatusbit.Decelerationsenleanment:
                    SetPGMMODVar(1, 0x20, enable);
                    // byte 1, bit 6
                    break;
                case PGMStatusbit.Car104:
                    SetPGMMODVar(1, 0x40, enable);
                    // byte 1, bit 7
                    break;
                case PGMStatusbit.Adaptivitywithclosedthrottle:
                    SetPGMMODVar(1, 0x80, enable);
                    // byte 1, bit 8
                    break;
                case PGMStatusbit.Factortolambdawhenthrottleopening:
                    SetPGMMODVar(2, 0x01, enable);
                    // byte 2, bit 1
                    break;
                case PGMStatusbit.Usesseparateinjmapduringidle:
                    SetPGMMODVar(2, 0x02, enable);
                    // byte 2, bit 2
                    break;
                case PGMStatusbit.FactortolambdawhenACisengaged:
                    SetPGMMODVar(2, 0x04, enable);
                    // byte 2, bit 3
                    break;
                case PGMStatusbit.ThrottleAccRetadjustsimultMY95:
                    SetPGMMODVar(2, 0x08, enable);
                    // byte 2, bit 4
                    break;
                case PGMStatusbit.Fueladjustingduringidle:
                    SetPGMMODVar(2, 0x10, enable);
                    // byte 2, bit 5
                    break;
                case PGMStatusbit.Purge:
                    SetPGMMODVar(2, 0x20, enable);
                    // byte 2, bit 6
                    break;
                case PGMStatusbit.Adaptionofidlecontrol:
                    SetPGMMODVar(2, 0x40, enable);
                    // byte 2, bit 7
                    break;
                case PGMStatusbit.Lambdacontrolduringidle:
                    SetPGMMODVar(2, 0x80, enable);
                    // byte 2, bit 8
                    break;
                case PGMStatusbit.Heatedplates:
                    SetPGMMODVar(3, 0x01, enable);
                    // byte 3, bit 1    
                    break;
                case PGMStatusbit.AutomaticTransmission:
                    SetPGMMODVar(3, 0x02, enable);
                    // byte 3, bit 2
                    break;
                case PGMStatusbit.Loadcontrol:
                    SetPGMMODVar(3, 0x04, enable);
                    // byte 3, bit 3
                    break;
                case PGMStatusbit.ETS:
                    SetPGMMODVar(3, 0x08, enable);
                    // byte 3, bit 4
                    break;
                case PGMStatusbit.APCcontrol:
                    SetPGMMODVar(3, 0x10, enable);
                    // byte 3, bit 5
                    break;
                case PGMStatusbit.Higheridleduringstart:
                    SetPGMMODVar(3, 0x20, enable);
                    // byte 3, bit 6
                    break;
                case PGMStatusbit.Globaladaption:
                    SetPGMMODVar(3, 0x40, enable);
                    // byte 3, bit 7
                    break;
                case PGMStatusbit.Tempcompwithactivelambdacontrol:
                    SetPGMMODVar(3, 0x80, enable);
                    // byte 3, bit 8    
                    break;
                case PGMStatusbit.Loadbufferduringidle:
                    SetPGMMODVar(4, 0x01, enable);
                    // byte 4, bit 1
                    break;
                case PGMStatusbit.Constidleignangleduringgearoneandtwo:
                    SetPGMMODVar(4, 0x02, enable);
                    // byte 4, bit 2
                    break;
                case PGMStatusbit.NofuelcutR12:
                    // byte 4, bit 3
                    SetPGMMODVar(4, 0x04, enable);
                    break;
                case PGMStatusbit.Airpumpcontrol:
                    SetPGMMODVar(4, 0x08, enable);
                    // byte 4, bit 4
                    break;
                case PGMStatusbit.Normalasperatedengine:
                    SetPGMMODVar(4, 0x10, enable);
                    // byte 4, bit 5
                    break;
                case PGMStatusbit.Knockregulatingdisabled:
                    SetPGMMODVar(4, 0x20, enable);
                    // byte 4, bit 6
                    break;
                case PGMStatusbit.Constantangle:
                    SetPGMMODVar(4, 0x40, enable);
                    // byte 4, bit 7
                    break;
                case PGMStatusbit.PurgevalveMY94:
                    SetPGMMODVar(4, 0x80, enable);
                    // byte 4, bit 8
                    break;
            }
        }

        private bool GetPGMMODVar(int byteoffset, int bitvalue)
        {
            bool retval = false;
            try
            {
                foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
                {
                    if (sh.Varname == "Pgm_mod!")
                    {
                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(byteoffset));
                        if ((sign & bitvalue) > 0)
                        {
                            retval = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private void SetPGMMODVar(int byteoffset, int bitvalue, bool enable)
        {
            try
            {
                foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
                {
                    if (sh.Varname == "Pgm_mod!")
                    {
                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(byteoffset));
                        if (enable)
                        {
                            sign |= (byte)bitvalue; // set flag
                        }
                        else
                        {
                            sign &= ((byte)((byte)bitvalue ^ 0xFF)); // clear flag
                        }
                        vssdata.SetValue(sign, byteoffset);
                        WriteData(vssdata, (uint)fileoffset);
                    }
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private bool GetVSSEnabled()
        {
            bool retval = false;

            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    if (sh.Length == 6)
                    {
                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;
                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(5));
                        if ((sign & 0x80) > 0)
                        {
                            retval = true;
                        }
                    }
                }
            }
            return retval;
        }

        private bool GetTankPressureDiagnosticsEnabled()
        {
            bool retval = false;

            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    if (sh.Length == 6)
                    {
                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;
                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(5));
                        if ((sign & 0x10) > 0)
                        {
                            retval = true;
                        }
                    }
                }
            }
            return retval;
        }

        private void SetTankDiagnosticsEnabled(bool enable)
        {
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    if (sh.Length == 6)
                    {

                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(5));
                        if (enable)
                        {
                            sign |= 0x10; // set vss flag
                        }
                        else
                        {
                            sign &= 0xEF; // clear vss flag
                        }
                        vssdata.SetValue(sign, 5);
                        WriteData(vssdata, (uint)fileoffset);
                    }
                }
            }
        }

        private void SetVSSEnabled(bool enable)
        {
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == "Pgm_mod!")
                {
                    if (sh.Length == 6)
                    {

                        int fileoffset = (int)sh.Flash_start_address;
                        //while (fileoffset > m_fileInfo.Filelength) fileoffset -= m_fileInfo.Filelength;

                        byte[] vssdata = readdatafromfile(m_currentFile, fileoffset, sh.Length);

                        byte sign = Convert.ToByte(vssdata.GetValue(5));
                        if (enable)
                        {
                            sign |= 0x80; // set vss flag
                        }
                        else
                        {
                            sign &= 0x7F; // clear vss flag
                        }
                        vssdata.SetValue(sign, 5);
                        WriteData(vssdata, (uint)fileoffset);
                    }
                }
            }
        }

        private string readimmocode()
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile, m_fileInfo.Filelength, 0x05, out length, out value);
            return value;
        }

        private bool FileInLibrary()
        {
            if (m_currentFile != string.Empty)
            {
                if (Path.GetDirectoryName(m_currentFile) == _libraryPath)
                {
                    return true;
                }
            }
            return false;
        }

        private void writeimmocode(string immocode)
        {
            if (FileInLibrary()) return;
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile,m_fileInfo.Filelength, 0x05, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);

            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                if (immocode.Length > length - (i + 1))
                {
                    bw1.Write((byte)immocode[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();
        }

        private bool readsecondO2sensorenabledflag()
        {
            // diag_mod & 0x80
            byte[] data = readdatafromfile(m_currentFile, GetSymbolAddress("Diag_mod!"), GetSymbolLength("Diag_mod!"));
            if ((data[0] & 0x80) > 0) return true;
            return false;
        }

        private void writesecondo2sensorenabled(bool enabled)
        {
            byte[] data = readdatafromfile(m_currentFile, GetSymbolAddress("Diag_mod!"), GetSymbolLength("Diag_mod!"));
            if (enabled) data[0] |= 0x80;
            else data[0] &= 0x7F;
            WriteData(data, (uint)GetSymbolAddress("Diag_mod!"));
        }
        private string readcarmodel()
        {
            string retval = string.Empty;
            FileStream fsi1 = File.OpenRead(m_currentFile);
            BinaryReader br1 = new BinaryReader(fsi1);

            int fileoffset = m_fileInfo.Filelength - 0x30;
            fsi1.Position = fileoffset;
            string temp = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                temp += (char)br1.ReadByte();
            }
            for (int i2 = 0; i2 < 4; i2++)
            {
                retval += temp[3 - i2];
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();

            return retval;
        }

        private void writecarmodel(string model)
        {
            if (FileInLibrary()) return;
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);
            int fileoffset = m_fileInfo.Filelength - 0x30;
            fsi1.Position =  fileoffset;
            for (int i = 0; i < model.Length; i++)
            {
                // backwards!!!
                bw1.Write((byte)model[model.Length - (i + 1)]);
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();

        }
        
        private string readenginetype()
        {
            
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile,m_fileInfo.Filelength, 0x04, out length, out value);
            return value;
        }

        private void writeenginetype(string model)
        {
            if (FileInLibrary()) return;
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile, m_fileInfo.Filelength, 0x04, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);
            model = model.PadRight(length, ' ');
            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                // backwards!!!
                if (model.Length > length - (i + 1))
                {
                    bw1.Write((byte)model[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();

        }
        
        private string readdataname()
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile, m_fileInfo.Filelength, 0x03, out length, out value);
            return value;
        }
        

        private void writeramlockedflag(bool locked)
        {
            // search for datanamn in flash data (what if datanamn has been changed)?
            // format is .NNA$ and then 00 or 01 in which 01 = locked, and 00 is unlocked
            if (FileInLibrary()) return;
            FileStream fs = new FileStream(m_currentFile, FileMode.Open, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x1b00, SeekOrigin.Begin); // was 0x2000 for T5.5, changed to 0x1b00 to support T5.2
            bool endsearch = false;
            int lstate = 0;
            byte bufbyte1 = 0;
            byte bufbyte2 = 0;
            byte bufbyte3 = 0;
            byte bufbyte4 = 0;
            bool valuefound = false;
            int positionfound = 0;
            byte foundvalue = 0xFF;
            while (!endsearch)
            {
                byte b = br.ReadByte();
                if (fs.Position > 0x10000) endsearch = true;
                switch (lstate)
                {
                    case 0:
                        if (b == '$')
                        {
                            if (IsAlpha(bufbyte4) && IsNumeric(bufbyte3) && IsNumeric(bufbyte2) && bufbyte1 == 0x2e)
                            {
                                lstate++;
                            }
                        }
                        break;
                    case 1:
                        valuefound = true;
                        foundvalue = b;
                        positionfound = (int)fs.Position - 1;
                        endsearch = true;
                        break;

                }
                // roll buffer
                bufbyte1 = bufbyte2;
                bufbyte2 = bufbyte3;
                bufbyte3 = bufbyte4;
                bufbyte4 = b;

            }
            br.Close();
            fs.Close();
            fs = new FileStream(m_currentFile, FileMode.Open, FileAccess.ReadWrite);
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                fs.Seek(positionfound, SeekOrigin.Begin);
                if (valuefound)
                {
                    if (locked)
                    {
                        if (foundvalue != 0x01)
                        {
                            // write 0x01
                            bw.Write((byte)0x01);
                        }
                    }
                    if (!locked)
                    {
                        if (foundvalue != 0x00)
                        {
                            bw.Write((byte)0x00);
                        }
                    }
                }
                bw.Close();
            }
            fs.Close();

            if (m_fileInfo.Filelength == 0x20000)
            {
                // Trionic 5.2 file
                //byte locked = readbytefromfile(m_currentfile, GetSymbolAddress(m_symbols, "Write_protect!"));
                if (!locked)
                {
                    writebyteinfile(m_currentFile, GetSymbolAddress("Write_protect!"), 0x00);
                }
                else
                {
                    writebyteinfile(m_currentFile, GetSymbolAddress("Write_protect!"), 0x01);
                }
            }
            updatechecksum(m_currentFile);
        }

        private bool readramlockedflag()
        {
            bool retval = true;
            FileStream fs = new FileStream(m_currentFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x1b00, SeekOrigin.Begin);
            bool endsearch = false;
            int lstate = 0;
            byte bufbyte1 = 0;
            byte bufbyte2 = 0;
            byte bufbyte3 = 0;
            byte bufbyte4 = 0;
            bool valuefound = false;
            byte foundvalue = 0xFF;
            while (!endsearch)
            {
                byte b = br.ReadByte();
                if (fs.Position > 0x10000) endsearch = true;
                switch (lstate)
                {
                    case 0:
                        if (b == '$')
                        {
                            if (IsAlpha(bufbyte4) && IsNumeric(bufbyte3) && IsNumeric(bufbyte2) && bufbyte1 == 0x2e)
                            {
                                lstate++;
                            }
                        }
                        break;
                    case 1:
                        endsearch = true;
                        valuefound = true;
                        foundvalue = b;
                        break;

                }
                // roll buffer
                bufbyte1 = bufbyte2;
                bufbyte2 = bufbyte3;
                bufbyte3 = bufbyte4;
                bufbyte4 = b;

            }
            br.Close();
            fs.Close();
            if (valuefound)
            {
                if (foundvalue == 0x00) retval = false;
            }

            if (m_fileInfo.Filelength == 0x20000)
            {
                // Trionic 5.2 file
                byte ramlock = readbytefromfile(m_currentFile, GetSymbolAddress("Write_protect!"));
                if (ramlock != 0x00)
                {
                    retval = true;
                }
            }
            return retval;


            // read dataname symbol


            
        }
        
        private void writedataname(string dataname)
        {
            if (FileInLibrary()) return;
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile, m_fileInfo.Filelength ,0x03, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);

            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                if (dataname.Length > length - (i + 1))
                {
                    bw1.Write((byte)dataname[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();
        }

        /*
        private string readimmocode()
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(0x05, out length, out value);
            return value;
            
        }

        private void writeimmocode(string immocode)
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(0x05, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentfile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);

            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                if (immocode.Length > length - (i + 1))
                {
                    bw1.Write((byte)immocode[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();
            

        }

        


        private string readpartnumber()
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(0x01, out length, out value);
            string retval = string.Empty;
            FileStream fsi1 = File.OpenRead(m_currentfile);
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = offset - length;
            string temp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                temp += (char)br1.ReadByte();
            }
            for (int i2 = 0; i2 < length; i2++)
            {
                retval += temp[(length - 1) - i2];
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();

            return retval;
        }
*/
        private void writepartnumber(string boxnumber)
        {
            if (FileInLibrary()) return;
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile,m_fileInfo.Filelength ,0x01, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);

            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                // backwards!!!
                if (boxnumber.Length > length - (i + 1))
                {
                    bw1.Write((byte)boxnumber[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();

        }
        /*
        private string readsoftwareid()
        {
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(0x02, out length, out value);
            string retval = string.Empty;
            FileStream fsi1 = File.OpenRead(m_currentfile);
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = offset - length;
            string temp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                temp += (char)br1.ReadByte();
            }
            for (int i2 = 0; i2 < length; i2++)
            {
                retval += temp[(length - 1) - i2];
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();

            return retval;
        }
        */
        private void writesoftwareid(string softwareid)
        {
            if (FileInLibrary()) return;
            int length = 0;
            string value = string.Empty;
            int offset = ReadMarkerAddress(m_currentFile,m_fileInfo.Filelength, 0x02, out length, out value);
            FileStream fsi1 = File.OpenWrite(m_currentFile);
            BinaryWriter bw1 = new BinaryWriter(fsi1);

            fsi1.Position = offset - length;
            for (int i = 0; i < length; i++)
            {
                // backwards!!!
                if (softwareid.Length > length - (i + 1))
                {
                    bw1.Write((byte)softwareid[length - (i + 1)]);
                }
            }
            fsi1.Flush();
            bw1.Close();
            fsi1.Close();
            fsi1.Dispose();

        }

        /*
        private string readfilecomment()
        {
            string retval = string.Empty;
            FileStream fsi1 = File.OpenRead(m_currentfile);
            BinaryReader br1 = new BinaryReader(fsi1);
            int fileoffset = m_currentfile_size - 0x44;

            fsi1.Position = fileoffset;
            string temp = string.Empty;
            for (int i = 0; i < 30; i++)
            {
                temp += (char)br1.ReadByte();
            }
            for (int i2 = 0; i2 < 30; i2++)
            {
                retval += temp[29 - i2];
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();

            return retval;
        }
        */

        public override void SetHardcodedRPMLimit(string filename, int rpmlimit)
        {
            int indexOne = RPMLimitSequenceOne(filename);
            int indexTwo = RPMLimitSequenceTwo(filename);
            if (indexOne > 0 && indexTwo > 0)
            {
                byte b1 = (byte)(rpmlimit / 256);
                byte b2 = (byte)(rpmlimit - (int)b1 * 256);
                writebyteinfile(filename, indexOne, b1);
                writebyteinfile(filename, indexOne + 1, b2);
                writebyteinfile(filename, indexTwo, b1);
                writebyteinfile(filename, indexTwo + 1, b2);
                //<GS-04042011> write new value at this position
            }
        }

        public override int GetHardcodedRPMLimit(string filename)
        {
            int retval = 0;
            int indexOne = RPMLimitSequenceOne(filename);
            if (indexOne > 0)
            {
                byte[] bytes = readdatafromfile(m_fileInfo.Filename, indexOne, 2);
                if (bytes.Length == 2)
                {
                    retval = Convert.ToInt32(bytes.GetValue(0));
                    retval *= 256;
                    retval += Convert.ToInt32(bytes.GetValue(1));
                }
            }
            return retval;
        }

        public override int GetHardcodedRPMLimitTwo(string filename)
        {
            int retval = 0;
            int indexOne = RPMLimitSequenceTwo(filename);
            if (indexOne > 0)
            {
                byte[] bytes = readdatafromfile(m_fileInfo.Filename, indexOne, 2);
                if (bytes.Length == 2)
                {
                    retval = Convert.ToInt32(bytes.GetValue(0));
                    retval *= 256;
                    retval += Convert.ToInt32(bytes.GetValue(1));
                }
            }
            return retval;
        }

        private int RPMLimitSequenceOne(string filename)
        {
            int retval = 0;
            FileInfo fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[12] {0x33, 0xF9, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x22, 0x11/*, 0xFF, 0xFF, 0x4C, 0x7C,
                                                0x10, 0x01, 0x00, 0x00, 0x00, 0xB4, 0x33, 0xC1, 0x00, 0x00, 0xFF, 0xFF, 0x4A, 0x91, 0x63, 0x00,
                                                0x00, 0x0C, 0x2A, 0x3C, 0x00, 0xEF, 0xFF, 0x38, 0x4C, 0x51, 0x50, 0x05, 0x0C, 0x85, 0x00, 0x00*/};

                byte data;
                int i;
                i = 0;
                while (a_fileStream.Position < fi.Length - 1)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || sequence[i] == 0xFF) // ignore 0xFF (mask out)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = (int)a_fileStream.Position;
                }
            }
            if (readbytefromfile(filename, retval + 4) == 0x10 && readbytefromfile(filename, retval + 5) == 0x01) retval += 36;
            else if (readbytefromfile(filename, retval + 22) == 0x10 && readbytefromfile(filename, retval + 23) == 0x01 && readbytefromfile(filename, retval + 24) == 0x00 && readbytefromfile(filename, retval + 25) == 0x00) retval += 54;
            else retval += 50;

            return retval;
        }

        private int RPMLimitSequenceTwo(string filename)
        {
            int retval = 0;
            FileInfo fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                //48 E7 30 70 3F 3C 00 01  4E B9 00 04 + 26
                //
                byte[] sequence = new byte[12] {0x48, 0xE7, 0x30, 0x70, 0x3F, 0x3C, 0x00, 0x01, 0x4E, 0xB9, 0x00, 0x04/*, 0xFF, 0xFF, 0x54, 0x8F,
                                                0x33, 0xF9, 0x00, 0x00, 0x2F, 0xFF, 0x00, 0x00, 0x2F, 0xFF, 0x33, 0xF9, 0x00, 0x00, 0x2F, 0xFF,
                                                0x00, 0x00, 0x2F, 0xFF, 0x0C, 0x79*/};

                byte data;
                int i;
                i = 0;
                while (a_fileStream.Position < fi.Length - 1)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || sequence[i] == 0xFF) // ignore 0xFF (mask out)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = (int)a_fileStream.Position + 26;
                }
            }
            if (readbytefromfile(filename, retval) == 0 && readbytefromfile(filename, retval + 1) == 0) retval -= 2;
            return retval;
        }

        private bool Find20MhzSequence(string filename)
        {
            bool retval = false;
            FileInfo fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[32] {0x02, 0x39, 0x00, 0xBF, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x80, 0x00, 0xFF, 0xFA, 0x04,
                                            0x02, 0x39, 0x00, 0xC0, 0x00, 0xFF, 0xFA, 0x04,
                                            0x00, 0x39, 0x00, 0x13, 0x00, 0xFF, 0xFA, 0x04};
                
                byte data;
                int i;
                i = 0;
                while (a_fileStream.Position < fi.Length - 1)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i])
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private CPUFrequency DetermineFrequency(string filename)
        {
            /*
16 Mhz binary: 0039 0040 00FF FA04 0239 00C0 00FF FA04 0039 003F 00FF FA04 
20 Mhz binary: 0239 00BF 00FF FA04 0039 0080 00FF FA04 0239 00C0 00FF FA04 0039 0013 00FF FA04 
             * * */
            if (Find20MhzSequence(filename)) return CPUFrequency.CPUFrequency20Mhz;
            return CPUFrequency.CPUFrequency16MHZ;
        }
        #endregion

        public override bool Exists()
        {
            bool retval = false;
            if (m_currentFile != "")
            {
                if (File.Exists(m_currentFile))
                {
                    retval = true;
                }
            }
            return retval;
        }

        public override void BackupFile()
        {
            //TODO: create a binary backupfile
        }

        public override event IECUFile.TransactionLogChanged onTransactionLogChanged;

        private void SignalTransactionLogChanged(TransactionEntry entry)
        {
            if (onTransactionLogChanged != null)
            {
                onTransactionLogChanged(this, new TransactionsEventArgs(entry));
            }
        }

        public override event IECUFile.DecodeProgress onDecodeProgress;

        private void SignalDecodeProgress(int progress)
        {
            if (onDecodeProgress != null)
            {
                onDecodeProgress(this, new DecodeProgressEventArgs(progress));
            }
        }

        public override int[] GetMapXaxisValues(string symbolname)
        {
            return GetXaxisValues(m_currentFile, symbolname);
        }

        public override int[] GetMapYaxisValues(string symbolname)
        {
            return GetYaxisValues(m_currentFile, symbolname);
        }

        public override void GetMapAxisDescriptions(string symbolname, out string x, out string y, out string z)
        {
            x = "X";
            y = "Y";
            z = "Z";
            GetAxisDescriptions(m_currentFile, symbolname, out x, out y, out z);
        }

        public override void GetMapMatrixWitdhByName(string symbolname, out int columns, out int rows)
        {
            columns = 1;
            rows = 1;
            GetTableMatrixWitdhByName(m_currentFile, symbolname, out columns, out rows);
        }


        public override bool IsTableSixteenBits(string symbolname)
        {
            return isSixteenBitTable(symbolname);
        }

        public override double GetCorrectionFactorForMap(string symbolname)
        {
            return GetMapCorrectionFactor(symbolname);
        }
        public override double GetOffsetForMap(string symbolname)
        {
            return GetMapCorrectionOffset(symbolname);
        }

        public override Trionic5FileInformation GetFileInfo()
        {
            return m_fileInfo;
        }

        public override void SelectFile(string filename)
        {
            m_currentFile = filename;
        }

        public override Trionic5FileInformation ParseFile()
        {
            Trionic5FileInformation t5fi = new Trionic5FileInformation();
            if (File.Exists(m_currentFile))
            {
                SignalDecodeProgress(0);
                t5fi = ParseTrionicFile(m_currentFile);
                SignalDecodeProgress(0);
            }
            return t5fi;
        }

        /*public override int GetInjectorType()
        {
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E6, 0);
        }
        public override void SetInjectorType(InjectorType type)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E6, (int)type);
        }*/

        public override int GetRegulationDivisorValue()
        {
            //return ReadBoostBiasDivisor(m_currentFile, m_fileInfo.Filelength
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E4, 10);
        }

        public override int GetManualRpmLow()
        {
            // read marker in file, if no marker -> 2750
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1F0, 2750);
        }
        public override int GetManualRpmHigh()
        {
            // read marker in file, if no marker -> 4000
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EE, 4000);
        }
        public override int GetAutoRpmLow()
        {
            // read marker in file, if no marker -> 2750
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EC, 2750);
        }
        public override int GetAutoRpmHigh()
        {
            // read marker in file, if no marker -> 4500
            return ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EA, 4500);
        }
        public override int GetMaxBoostError()
        {
            int temp = ReadRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E8, 4);
            return temp;
        }

        public override void SetRegulationDivisorValue(int value)
        {
            //return ReadBoostBiasDivisor(m_currentFile, m_fileInfo.Filelength
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E4, value);
        }

        public override void SetAutoRpmHigh(int rpm)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EA, rpm);
        }
        public override void SetAutoRpmLow(int rpm)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EC, rpm);
        }
        public override void SetManualRpmHigh(int rpm)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1EE, rpm);
        }
        public override void SetManualRpmLow(int rpm)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1F0, rpm);
        }
        public override void SetMaxBoostError(int boosterror)
        {
            WriteRpmBoostRange(m_currentFile, m_fileInfo.Filelength - 0x1E8, boosterror);
        }


        private void WriteRpmBoostRange(string filename, int address, int rpm)
        {
            byte[] buf = new byte[2];
            buf[0] = Convert.ToByte((rpm >> 8) & 0x0000000000000000FF);
            buf[1] = Convert.ToByte((rpm) & 0x0000000000000000FF);

            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                if (address > 0)
                {
                    WriteDataNoCounterIncrease(buf, (uint)address);
                }
            }
        }

        public override DateTime GetMemorySyncDate()
        {
            DateTime dt_sync = new DateTime(2000,1,1,0,0,0); // testvalue
            int year = 2000;
            int month = 1;
            int day = 1;
            int hour = 0;
            int minute = 0;
            int second = 0;

            int address = m_fileInfo.Filelength - 0x1E0;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                if (address > 0)
                {
                    year = (Int32)readbytefromfile(m_currentFile, address) * 256;
                    year += (Int32)readbytefromfile(m_currentFile, address + 1);
                    month = (Int32)readbytefromfile(m_currentFile, address + 2);
                    day = (Int32)readbytefromfile(m_currentFile, address + 3);
                    hour = (Int32)readbytefromfile(m_currentFile, address + 4);
                    minute = (Int32)readbytefromfile(m_currentFile, address + 5);
                    second = (Int32)readbytefromfile(m_currentFile, address + 6);
                    if (year != -1)
                    {
                        try
                        {
                            dt_sync = new DateTime(year, month, day, hour, minute, second);
                        }
                        catch (Exception E)
                        {
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }
            Console.WriteLine("BIN: " + dt_sync.ToString("dd/MM/yyyy HH:mm:ss"));
            return dt_sync;
        }

        public override void SetMemorySyncDate(DateTime syncdt)
        {
            byte[] buf = new byte[7];
            buf[0] = Convert.ToByte((syncdt.Year >> 8) & 0x0000000000000000FF);
            buf[1] = Convert.ToByte((syncdt.Year) & 0x0000000000000000FF);
            buf[2] = Convert.ToByte((syncdt.Month) & 0x0000000000000000FF);
            buf[3] = Convert.ToByte((syncdt.Day) & 0x0000000000000000FF);
            buf[4] = Convert.ToByte((syncdt.Hour) & 0x0000000000000000FF);
            buf[5] = Convert.ToByte((syncdt.Minute) & 0x0000000000000000FF);
            buf[6] = Convert.ToByte((syncdt.Second) & 0x0000000000000000FF);
            WriteDataNoCounterIncrease(buf, (uint)(m_fileInfo.Filelength - 0x1E0));
        }

        public override Int64 GetMemorySyncCounter()
        {
            Int64 countervalue = 0;
            int address = m_fileInfo.Filelength - 0x1FC;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                if (address > 0)
                {
                    countervalue = (Int64)readbytefromfile(m_currentFile, address) * 256 * 256 * 256 * 256 * 256 * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 1) * 256 * 256 * 256 * 256 * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 2) * 256 * 256 * 256 * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 3) * 256 * 256 * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 4) * 256 * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 5) * 256 * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 6) * 256;
                    countervalue += (Int64)readbytefromfile(m_currentFile, address + 7);
                    if (countervalue == -1 || countervalue == 0xFFFFFFFF) countervalue = 0; // start @ 0
                }
            }
            return countervalue;
        }

        public override void SetMemorySyncCounter(Int64 countervalue)
        {
            byte[] buf = new byte[8];
            buf[0] = Convert.ToByte((countervalue >> 56) & 0x0000000000000000FF);
            buf[1] = Convert.ToByte((countervalue >> 48) & 0x0000000000000000FF);
            buf[2] = Convert.ToByte((countervalue >> 40) & 0x0000000000000000FF);
            buf[3] = Convert.ToByte((countervalue >> 32) & 0x0000000000000000FF);
            buf[4] = Convert.ToByte((countervalue >> 24) & 0x0000000000000000FF);
            buf[5] = Convert.ToByte((countervalue >> 16) & 0x0000000000000000FF);
            buf[6] = Convert.ToByte((countervalue >> 8) & 0x0000000000000000FF);
            buf[7] = Convert.ToByte((countervalue) & 0x0000000000000000FF);
            WriteDataNoCounterIncrease(buf, (uint)(m_fileInfo.Filelength - 0x1FC));
        }

        private int ReadRpmBoostRange(string filename, int address, int defaultvalue)
        {
            int stage = defaultvalue;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                if (address > 0)
                {
                    stage = (int)readbytefromfile(filename, address) * 256;
                    stage += (int)readbytefromfile(filename, address + 1);
                    if (stage == -1 || stage == 0xFFFF) stage = defaultvalue;
                }
            }
            return stage;
        }

        public override MapSensorType GetMapSensorType(bool autoDetectMapsensorType)
        {
            MapSensorType mst = (MapSensorType)ReadThreeBarConversionMarker(m_currentFile);
            if (autoDetectMapsensorType && mst == MapSensorType.MapSensor25) // only if stock and not overruled
            {
                mst = DetermineMapSensorType();
            }
            return mst;
        }

        private void WriteThreeBarConversionMarker(string filename, MapSensorType sensorType)
        {

            int address = m_fileInfo.Filelength - 0x1FF;
            if (address > 0)
            {
                switch (sensorType)
                {
                    case MapSensorType.MapSensor25:
                        writebyteinfile(filename, address, 0xFF); // original
                        break;
                    case MapSensorType.MapSensor30:
                        writebyteinfile(filename, address, 0x01);
                        break;
                    case MapSensorType.MapSensor35:
                        writebyteinfile(filename, address, 0x02);
                        break;
                    case MapSensorType.MapSensor40:
                        writebyteinfile(filename, address, 0x03);
                        break;
                    case MapSensorType.MapSensor50:
                        writebyteinfile(filename, address, 0x04);
                        break;
                }
            }
        }

        /// <summary>
        /// Read the threebar conversion marker in the binary. This is kept at address (length - 0x1FF)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private int ReadThreeBarConversionMarker(string filename)
        {
            int stage = 0;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                int address = (int)fi.Length - 0x1FF;
                if (address > 0)
                {
                    stage = (int)readbytefromfile(filename, address);
                    if (stage == 0xFF) stage = 0;
                }
            }
            return stage;
        }

        public override void WriteInjectorTypeMarker(InjectorType injectorType)
        {
            int address = m_fileInfo.Filelength - 0x1FE;
            if (address > 0)
            {
                writebyteinfile(m_currentFile, address, Convert.ToByte((int)injectorType)); 
            }
        }

        public override void WriteTurboTypeMarker(TurboType turboType)
        {
            int address = m_fileInfo.Filelength - 0x1FD;
            if (address > 0)
            {
                writebyteinfile(m_currentFile, address, Convert.ToByte((int)turboType)); 
            }
        }

        public override void WriteTuningStageMarker(TuningStage tuningStage)
        {
            int address = m_fileInfo.Filelength - 0x200;
            if (address > 0)
            {
                writebyteinfile(m_currentFile, address, Convert.ToByte((int)tuningStage));
            }
        }

        public override int ReadInjectorTypeMarker()
        {
            int stage = 0;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                int address = (int)fi.Length - 0x1FE;
                if (address > 0)
                {
                    stage = (int)readbytefromfile(m_currentFile, address);
                    if (stage == 0xFF) stage = 0;
                }
            }
            return stage;
        }

        public override int ReadTuningStageMarker()
        {
            int stage = 0;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                int address = (int)fi.Length - 0x200;
                if (address > 0)
                {
                    stage = (int)readbytefromfile(m_currentFile, address);
                    if (stage == 0xFF) stage = 0;
                }
            }
            return stage;
        }

        public override int ReadTurboTypeMarker()
        {
            int stage = 0;
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                int address = (int)fi.Length - 0x1FD;
                if (address > 0)
                {
                    stage = (int)readbytefromfile(m_currentFile, address);
                    if (stage == 0xFF) stage = 0;
                }
            }
            return stage;
        }


        public override ECUFileType DetermineFileType()
        {
            
            if (File.Exists(m_currentFile))
            {
                FileInfo fi = new FileInfo(m_currentFile);
                if (fi.Length == 0x020000) return ECUFileType.Trionic52File;
                if (fi.Length == 0x040000) return ECUFileType.Trionic55File;
                if (fi.Length == 0x080000) return ECUFileType.Trionic7File;
                if (fi.Length == 0x100000) return ECUFileType.Trionic8File;
            }
            return ECUFileType.UnknownFile;
        }

        public override bool ValidateChecksum()
        {
            if (File.Exists(m_currentFile))
            {
                return verifychecksum(m_currentFile);
            }
            return false;
        }

        public override void UpdateChecksum()
        {
            if (File.Exists(m_currentFile))
            {
                updatechecksum(m_currentFile);
            }
        }

        public override byte[] ReadDataFromFile(string filename, uint offset, uint length)
        {
            if (File.Exists(filename))
            {
                return readdatafromfile(filename, (int)offset, (int)length);
            }
            byte[] b = new byte[1];
            b.SetValue((byte)0x00, 0);
            return b;
        }

        

        public override byte[] ReadData(uint offset, uint length)
        {
            if (File.Exists(m_currentFile))
            {
                return readdatafromfile(m_currentFile, (int)offset, (int)length);
            }
            byte[] b = new byte[1];
            b.SetValue((byte)0x00, 0);
            return b;
        }

        // Implement addition to changelog here (write data to binary file)

        public override bool WriteDataNoCounterIncrease(byte[] data, uint offset)
        {
            if (File.Exists(m_currentFile))
            {
                byte[] beforedata = readdatafromfile(m_currentFile, (int)offset, data.Length);

                for (int i = 0; i < data.Length; i++)
                {
                    writebyteinfile(m_currentFile, (int)offset + i, (byte)data.GetValue(i));
                }
                if(m_transactionLog != null)
                {
                    TransactionEntry tentry = new TransactionEntry(DateTime.Now, (int)offset, data.Length, beforedata, data, 0, 0, "");
                    m_transactionLog.AddToTransactionLog(tentry);
                    SignalTransactionLogChanged(tentry);
                    // versiebeheer bijhouden <GS-16032010>
                    //m_fileInfo.getSymbolNameByAddress((int)offset);
                }
                if (m_autoUpdateChecksum) UpdateChecksum();
                return true;
            }
            return false;
        }

        public override bool WriteDataNoLog(byte[] data, uint offset)
        {
            if (File.Exists(m_currentFile))
            {
                //byte[] beforedata = readdatafromfile(m_currentFile, (int)offset, data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    writebyteinfile(m_currentFile, (int)offset + i, (byte)data.GetValue(i));
                }
                //SetMemorySyncCounter(GetMemorySyncCounter() + 1);
                //SetMemorySyncDate(DateTime.Now);
                if (m_autoUpdateChecksum) UpdateChecksum();
                return true;
            }
            return false;
        }

        public override bool WriteData(byte[] data, uint offset, string note)
        {
            if (File.Exists(m_currentFile))
            {
                byte[] beforedata = readdatafromfile(m_currentFile, (int)offset, data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    writebyteinfile(m_currentFile, (int)offset + i, (byte)data.GetValue(i));
                }
                //SetMemorySyncCounter(GetMemorySyncCounter() + 1);
                SetMemorySyncDate(DateTime.Now);
                if (m_transactionLog != null)
                {
                    TransactionEntry tentry = new TransactionEntry(DateTime.Now, (int)offset, data.Length, beforedata, data, 0, 0, note);
                    m_transactionLog.AddToTransactionLog(tentry);
                    SignalTransactionLogChanged(tentry);
                    // versiebeheer bijhouden <GS-16032010>
                }
                if (m_autoUpdateChecksum) UpdateChecksum();
                return true;
            }
            return false;
        }

        public override bool WriteData(byte[] data, uint offset)
        {
            return WriteData(data, offset, "");
        }

        public override string GetSoftwareVersion()
        {
            if (File.Exists(m_currentFile))
            {
                return GetSoftwareVersion(m_currentFile);
            }
            return string.Empty;
        }

        public override string GetPartnumber()
        {
            if (File.Exists(m_currentFile))
            {
                return readpartnumber(m_currentFile);
            }
            return string.Empty;
        }

        public Trionic5FileInformation FileInfo
        {
            get { return m_fileInfo; }
            set { m_fileInfo = value; }
        }


        private string readpartnumber(string m_currentfile)
        {
            int length = 0;
            string value = string.Empty;
            FileInfo fi = new FileInfo(m_currentfile);

            int offset = ReadMarkerAddress(m_currentfile, (int)fi.Length, 0x01, out length, out value);
            string retval = string.Empty;
            FileStream fsi1 = File.OpenRead(m_currentfile);
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = offset - length;
            string temp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                temp += (char)br1.ReadByte();
            }
            for (int i2 = 0; i2 < length; i2++)
            {
                retval += temp[(length - 1) - i2];
            }
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();

            return retval;
        }
        public Trionic5FileInformation ParseTrionicFile(string filename)
        {
            ParseFile(filename, m_fileInfo.SymbolCollection, m_fileInfo.AddressCollection);
            m_fileInfo.Filename = filename;
            FileInfo fi = new FileInfo(filename);
            m_fileInfo.Filelength = (int)fi.Length;
            
            SymbolTranslator st = new SymbolTranslator();
            string helptext = string.Empty;
            XDFCategories cat = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach(SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                st.TranslateSymbolToHelpText(sh.Varname, out helptext, out cat, out subcat);
                sh.Helptext = helptext;
                sh.XdfCategory = cat;
                sh.XdfSubcategory = subcat;
                if (sh.Varname.StartsWith("Knock_count_cyl")) sh.Length = 2;
            }
            TryToLoadTemperatureConversionTables();
            SignalDecodeProgress(95);
            return m_fileInfo;
        }

        private bool IsAlpha(byte b)
        {
            bool retval = false;
            if (b >= 0x30 && b <= 0x39) retval = true;
            else if (b >= 0x41 && b <= 0x5A) retval = true;
            else if (b >= 0x61 && b <= 0x7A) retval = true;
            return retval;
        }

        private bool IsNumeric(byte b)
        {
            bool retval = false;
            if (b >= 0x30 && b <= 0x39) retval = true;
            return retval;
        }

        public string GetSoftwareVersion(string filename)
        {
            string swid = string.Empty;
            // search for datanamn in flash data (what if datanamn has been changed)?
            // format is XXXXXXXX.NNA$ and then 00 or 01 in which 01 = locked, and 00 is unlocked

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x1b00, SeekOrigin.Begin); // was 0x2000 for T5.5, changed to 0x1b00 to support T5.2
            bool endsearch = false;
            int lstate = 0;
            byte bufbyte1 = 0;
            byte bufbyte2 = 0;
            byte bufbyte3 = 0;
            byte bufbyte4 = 0;
            
            int positionfound = 0;
            byte foundvalue = 0xFF;
            while (!endsearch)
            {
                if (fs.Position >= m_fileInfo.Filelength -1) endsearch = true;
                byte b = br.ReadByte();

                //if (fs.Position > m_fileInfo.Filelength) endsearch = true;
                switch (lstate)
                {
                    case 0:
                        if (b == '$')
                        {
                            if (IsAlpha(bufbyte4) && IsNumeric(bufbyte3) && IsNumeric(bufbyte2) && bufbyte1 == 0x2e)
                            {
                                lstate++;
                            }
                        }
                        break;
                    case 1:
                        
                        foundvalue = b;
                        positionfound = (int)fs.Position - 14;
                        endsearch = true;
                        break;

                }
                // roll buffer
                bufbyte1 = bufbyte2;
                bufbyte2 = bufbyte3;
                bufbyte3 = bufbyte4;
                bufbyte4 = b;

            }
            if (positionfound > 0)
            {
                // positionfound
                fs.Seek(positionfound, SeekOrigin.Begin);
                byte[] dn = br.ReadBytes(12);
                swid = System.Text.Encoding.ASCII.GetString(dn);
            }
            br.Close();
            fs.Close();
            Console.WriteLine("SW ID: " + swid);
            return swid;
        }

        public double GetMapCorrectionOffset(string symbolname)
        {
            double returnvalue = 0;
            if (symbolname.StartsWith("Ign_map_0!")) returnvalue = 0;
            else if (symbolname.StartsWith("Insp_mat!")) returnvalue = 0.5; // 128/256
            else if (symbolname.StartsWith("Inj_map_0!")) returnvalue = 0; //LOLA specific
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
            else if (symbolname.StartsWith("Max_regl_temp_1!")) returnvalue = -1;
            else if (symbolname.StartsWith("Max_regl_temp_2!")) returnvalue = -1;

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

            else if (symbolname.StartsWith("Kadapt_load_high!")) returnvalue = -1;
            else if (symbolname.StartsWith("Kadapt_load_low!")) returnvalue = -1;
            else if (symbolname.StartsWith("Iv_min_load!")) returnvalue = -1;
            else if (symbolname.StartsWith("Shift_load!")) returnvalue = -1;
            else if (symbolname.StartsWith("Shift_up_load_hyst!")) returnvalue = -1;
            else if (symbolname.StartsWith("Luft_kompfak!")) returnvalue = 0.75;

            else if (symbolname.StartsWith("Ign_map_0_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Ign_map_2_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Ign_map_6_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Ign_map_7_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Fuel_map_xaxis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Detect_map_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Misfire_map_x_axis!")) returnvalue = -1;
            else if (symbolname.StartsWith("Purge_map_xaxis!")) returnvalue = -1;
            return returnvalue;

        }

        public double GetMapCorrectionFactor(string symbolname)
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
            else if (symbolname.StartsWith("Inj_map_0!")) returnvalue = 1;// 0.00390625; // 1/256 LOLA specific
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

                //<GS-31012011>

            else if (symbolname.StartsWith("Max_regl_temp_1!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Max_regl_temp_2!")) returnvalue = 0.01;

            else if (symbolname.StartsWith("P_Manifold10")) returnvalue = 0.001;
            else if (symbolname.StartsWith("P_Manifold")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Max_ratio_aut!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Diag_speed_load!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Kadapt_load_high!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Kadapt_load_low!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Iv_min_load!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Shift_load!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Shift_up_load_hyst!")) returnvalue = 0.01;


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
            else if (symbolname.StartsWith("Luft_kompfak!")) returnvalue = 0.001953125;
            else if (symbolname.StartsWith("Ign_map_0_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Ign_map_2_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Ign_map_6_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Ign_map_7_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Fuel_map_xaxis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Detect_map_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Misfire_map_x_axis!")) returnvalue = 0.01;
            else if (symbolname.StartsWith("Purge_map_xaxis!")) returnvalue = 0.01;

            return returnvalue;
        }



        public byte readbytefromfile(string filename, int address)
        {
            byte retval = 0;
            try
            {
                FileStream fsi1 = File.OpenRead(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                retval = br1.ReadByte();
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            /*            System.Windows.Forms.Application.DoEvents();
                        System.Threading.Thread.Sleep(5);
                        System.Windows.Forms.Application.DoEvents();*/
            return retval;
        }

        public void writebyteinfile(string filename, int address, byte value)
        {
            try
            {
                if (FileInLibrary()) return;
                if (address <= 0) return;
                FileStream fsi1 = File.OpenWrite(filename);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryWriter br1 = new BinaryWriter(fsi1);
                fsi1.Position = address;
                br1.Write(value);
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            /*System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(5);
            System.Windows.Forms.Application.DoEvents();*/
        }

        private int GetSymbolLength(string symbolname)
        {
            if (symbolname == "Knock_count_cyl1" || symbolname == "Knock_count_cyl2" || symbolname == "Knock_count_cyl3" || symbolname == "Knock_count_cyl4")
            {
                return 2;
            }
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
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
            foreach (SymbolHelper sh in m_fileInfo.SymbolCollection)
            {
                if (sh.Varname == symbolname)
                {
                    retval = (int)sh.Flash_start_address - m_fileInfo.Filelength;
                    while (retval > m_fileInfo.Filelength) retval -= m_fileInfo.Filelength;
                }
            }
            return retval;
        }

        public int GetTableMatrixWitdhByName(string filename, string symbolname, out int columns, out int rows)
        {
            columns = 0;
            rows = 0;
            if (symbolname.StartsWith("Insp_mat!"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                columns = GetSymbolLength("Inj_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Inj_map_0_y_axis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Idle_der_tab!"))
            {
                columns = 8;
                rows = GetSymbolLength("Idle_der_tab!") / 8;
                return 8;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                columns = GetSymbolLength("Fuel_knock_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 12;
            }
            else if (symbolname.StartsWith("Iv_min_tab_ac!"))
            {
                columns = 1;
                rows = GetSymbolLength(symbolname) / columns;
                return 2;
            }
            else if (symbolname.StartsWith("Iv_min_tab!"))
            {
                columns = 1;
                rows = GetSymbolLength(symbolname) / columns;
                return 1;
            }
            else if (symbolname.StartsWith("Lambdamatris_diag!"))
            {
                columns = 3;
                rows = GetSymbolLength(symbolname) / (columns * 2);
                return 3;
            }
            else if (symbolname.StartsWith("Lambdamatris!"))
            {
                //columns = 12; // was 3
                columns = 3;
                rows = GetSymbolLength(symbolname) / (columns * 2);
                return 3; // was 3
            }
            /*else if (symbolname.StartsWith("Lambdamatris_diag!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 8;
            }*/
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                columns = GetSymbolLength("Idle_st_last!");
                //rows = GetSymbolLength( "Idle_rpm_tab!");//2
                rows = GetSymbolLength("Idle_st_rpm!");//2 ??? which one is right???
                return 12;
            }
            else if (symbolname.StartsWith("Mis1000_map!") || symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Detect_map!") || symbolname.StartsWith("Knock_ref_matrix!"))
            {
                columns = GetSymbolLength("Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Misfire_map!")) // T5.2 table
            {
                columns = GetSymbolLength("Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Ign_map_0!"))
            {
                columns = GetSymbolLength("Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                columns = GetSymbolLength("Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                columns = 16;
                rows = 32;
                return 16;
            }

            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                columns = GetSymbolLength("Ign_map_1_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_1_y_axis!");//2
                return 0x1;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                columns = GetSymbolLength("Ign_map_2_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_2_y_axis!");//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                columns = GetSymbolLength("Ign_map_3_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_3_y_axis!") / 2;
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                columns = GetSymbolLength("Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                columns = GetSymbolLength("Ign_map_5_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_5_y_axis!");//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                columns = GetSymbolLength("Ign_map_6_x_axis!") / 2;//2
                rows = GetSymbolLength("Ign_map_6_y_axis!") / 2;//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                columns = GetSymbolLength("Ign_map_7_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_7_y_axis!") / 2;//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                columns = GetSymbolLength("Ign_map_8_x_axis!") / 2;
                rows = GetSymbolLength("Ign_map_8_y_axis!") / 2;//2
                return 0x08;
            }

            else if (symbolname.StartsWith("AC_Control!"))
            {
                columns = 8;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab"))
            {
                //Fuel_map_yaxis! langs de y as
                columns = 1;// op aanvraag hessu
                rows = GetSymbolLength(symbolname) / columns;
                return 0x10;
            }
            else if (symbolname.StartsWith("Regl_tryck"))
            {
                columns = 1;// op aanvraag hessu
                rows = GetSymbolLength(symbolname) / columns;
                return 0x10;
            }
            else if (symbolname.StartsWith("Reg_kon_mat"))
            {
                columns = 1;// op aanvraag hessu
                // sometimes this table is 0x80 bytes long, in that case it needs special conversion.
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    //MY94 and earlier reg_kon_mat, has values for each gear and misses boost limiter for 1st and 2nd gear
                    columns = 8;
                }
                rows = GetSymbolLength(symbolname) / columns;
                return 0x10;
            }

            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                columns = GetSymbolLength("Trans_x_st!");
                //columns = GetSymbolLength( "Pwm_ind_trot!");
                //rows = GetSymbolLength( "Trans_y_st!");//2
                rows = GetSymbolLength("Pwm_ind_rpm!") / 2;
                return 8;
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                columns = 1;
                rows = GetSymbolLength(symbolname);
                return 1;
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                columns = GetSymbolLength("Trans_x_st!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength("Pwm_ind_rpm!") / 2;

                return 8;
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                columns = GetSymbolLength("Trans_x_st!");
                rows = GetSymbolLength(symbolname) / columns;
                return columns;
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                //columns = GetSymbolLength( "Trans_x_st!");
                columns = GetSymbolLength("Overs_tab_xaxis!");
                rows = GetSymbolLength("Pwm_ind_rpm!") / 2;

                return 8;
            }
            /*else if (symbolname.StartsWith("Detect_map!"))
            {
                columns = GetSymbolLength( "Detect_map_x_axis!"); // 2;
                rows = GetSymbolLength( "Detect_map_y_axis!"); // 2;
                return 0x12;
            }*/
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                columns = GetSymbolLength("Dash_trot_axis!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength("Dash_rpm_axis!") / 2;
                return 8;
                /*columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;*/
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_"))
            {
                //x = "MAP"; //?
                columns = 1;
                rows = GetSymbolLength("Temp_steg!");
                return 1;
            }

            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                columns = GetSymbolLength("Temp_reduce_x_st!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength("Temp_reduce_y_st!") / 2;
                return 8;
                /*columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;*/
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                columns = 0x10;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x10;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                columns = 0x10;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x10;
            }
            /*            else if (symbolname.StartsWith("Purge_tab!"))
                        {
                            columns = 0x10;
                            rows = GetSymbolLength( symbolname) / columns;
                            return 0x10;
                        }*/
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                columns = GetSymbolLength("Fuel_map_xaxis!");
                rows = GetSymbolLength("Fuel_map_yaxis!") / 2;
                return 16;
            }

            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                columns = 0x12;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Mis200_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Misfire_map!")) // T5.2 table
            {
                columns = 0x12;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Mis1000_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                columns = 0x4;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x4;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                columns = 0x4;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x4;
            }
            else if (symbolname.StartsWith("Pwm_ind_trot!"))
            {
                columns = 0x08;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Idle_p_tab!"))
            {
                columns = 0x08;
                rows = GetSymbolLength(symbolname) / columns;
                return 0x08;
            }
            columns = 1;
            rows = GetSymbolLength(symbolname) / columns;
            return 1;
        }

        public override int[] GetXaxisValues(string filename, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int xaxisaddress = 0;
            int xaxislength = 0;
            bool issixteenbit = false;
            int multiplier = 1;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_1_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_1_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_2_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_2_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_3_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_3_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_5_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_5_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_6_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_6_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_7_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_7_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_8_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_8_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname == "Lambdamatris!" || symbolname == "Lambdamatris_diag!")
            {
                xaxisaddress = GetSymbolAddress("Lam_laststeg!");
                xaxislength = GetSymbolLength("Lam_laststeg!");
            }

            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress("Pwm_ind_trot!") + 32;
                issixteenbit = false;

                //                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                //                xaxislength = GetSymbolLength("Trans_x_st!");
                //                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                //xaxisaddress = GetSymbolAddress("Trans_x_st!");
                //xaxislength = GetSymbolLength("Trans_x_st!");
                xaxisaddress = GetSymbolAddress("Overs_tab_xaxis!");
                xaxislength = GetSymbolLength("Overs_tab_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress("Pwm_ind_trot!") + 32;
                issixteenbit = false;

                //xaxisaddress = GetSymbolAddress("Trans_x_st!");
                //xaxislength = GetSymbolLength("Trans_x_st!");
                //issixteenbit = false;
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                xaxisaddress = GetSymbolAddress("Inj_map_0_x_axis!");
                xaxislength = GetSymbolLength("Inj_map_0_x_axis!") ;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                xaxisaddress = GetSymbolAddress("Dash_trot_axis!");
                xaxislength = GetSymbolLength("Dash_trot_axis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                xaxisaddress = GetSymbolAddress("Idle_st_last!");
                xaxislength = GetSymbolLength("Idle_st_last!");
                issixteenbit = false;
            }
            //retard_konst
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                xaxisaddress = GetSymbolAddress("Reg_last!");
                xaxislength = GetSymbolLength("Reg_last!");
                issixteenbit = false;
                //multiplier = 0.01;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                xaxisaddress = GetSymbolAddress("Reg_last!");
                xaxislength = GetSymbolLength("Reg_last!");
                issixteenbit = false;
                //multiplier = 0.01;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_knock_xaxis!");
                xaxislength = GetSymbolLength("Fuel_knock_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                xaxisaddress = GetSymbolAddress("Misfire_map_x_axis!");
                xaxislength = GetSymbolLength("Misfire_map_x_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                xaxisaddress = GetSymbolAddress("Misfire_map_x_axis!");
                xaxislength = GetSymbolLength("Misfire_map_x_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                xaxisaddress = GetSymbolAddress("Temp_reduce_x_st!");
                xaxislength = GetSymbolLength("Temp_reduce_x_st!");
                issixteenbit = false;
            }
            /*            else if (symbolname.StartsWith("Knock_ref_matrix!"))
                        {
                            xaxisaddress = GetSymbolAddress(curSymbols,"Ign_map_0_x_axis!");
                            xaxislength = GetSymbolLength(curSymbols,"Ign_map_0_x_axis!");
                            issixteenbit = true;
                        }*/
            else if (symbolname.StartsWith("Detect_map!"))
            {
                xaxisaddress = GetSymbolAddress("Detect_map_x_axis!");
                xaxislength = GetSymbolLength("Detect_map_x_axis!");
                issixteenbit = true;
            }
            else if ((symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!")) && GetSymbolLength(symbolname) == 0x80)
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress("Pwm_ind_trot!") + 32;
                issixteenbit = false;
            }
            /*else if (symbolname.StartsWith("Iv_start_time_tab!"))
            {
                retval = new int[8];
                //65 50 35 20 5 -5 -20 -40
                retval.SetValue(65, 0);
                retval.SetValue(50, 1);
                retval.SetValue(35, 2);
                retval.SetValue(20, 3);
                retval.SetValue(5, 4);
                retval.SetValue(-5, 5);
                retval.SetValue(-20, 6);
                retval.SetValue(-40, 7);
            }*/

            else
            {
                xaxislength = 0;
                int rows = 0;
                int cols = 0;
                GetTableMatrixWitdhByName(filename,  symbolname, out cols, out rows);
                retval = new int[cols];
                for (int t = 0; t < cols; t++)
                {
                    retval.SetValue(t, t);
                }
            }

            int number = xaxislength;
            if (xaxislength > 0)
            {
                byte[] axisdata = readdatafromfile(filename, xaxisaddress, xaxislength);
                if (issixteenbit) number /= 2;
                retval = new int[number];
                int offset = 0;
                for (int i = 0; i < xaxislength; i++)
                {

                    if (issixteenbit)
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        byte val2 = (byte)axisdata.GetValue(++i);
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        int value = (ival1 * 256) + ival2;

                        value *= multiplier;
                        retval.SetValue(value, offset++);
                    }
                    else
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        int ival1 = Convert.ToInt32(val1);

                        ival1 *= multiplier;
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }

        public override int[] GetYaxisValues(string filename, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int yaxisaddress = 0;
            int yaxislength = 0;
            bool issixteenbit = false;
            int multiplier = 1;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_1_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_1_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_2_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_2_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_3_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_3_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_5_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_5_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_6_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_6_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_7_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_7_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Open_loop_adapt!") || symbolname.StartsWith("Open_loop!") || symbolname.StartsWith("Open_loop_knock!"))
            {
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Fload_tab!") || symbolname.StartsWith("Fload_throt_tab!"))
            {
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }


            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                yaxisaddress = GetSymbolAddress("Ign_map_8_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_8_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                yaxislength = GetSymbolLength("Temp_steg!");
                yaxisaddress = GetSymbolAddress("Temp_steg!");

                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;
            }

            else if (symbolname.StartsWith("Kyltemp_tab!"))
            {
                yaxislength = GetSymbolLength("Kyltemp_steg!");
                yaxisaddress = GetSymbolAddress("Kyltemp_steg!");
            }
            else if (symbolname.StartsWith("Lufttemp_tab!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress("Lufttemp_steg!");
            }
           /* else if (symbolname.StartsWith("Luft_kompfak!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress("Lufttemp_steg!");
            }*/
            else if (symbolname.StartsWith("Luft_kompfak!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress("Lufttemp_steg!");
                // 
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {

                    retval.SetValue(LookupAirTemperature((int)Lufttemp_steg.GetValue(i)), i);
                }
                return retval;
            }

            else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress("Lufttemp_steg!");
                // 
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {

                    retval.SetValue(LookupAirTemperature((int)Lufttemp_steg.GetValue(i)), i);
                }
                return retval;
            }
            else if (symbolname.StartsWith("Derivata_br_tab_pos!") || symbolname.StartsWith("Derivata_br_tab_neg!"))
            {
                yaxislength = GetSymbolLength("Derivata_br_sp!");
                yaxisaddress = GetSymbolAddress("Derivata_br_sp!");
            }
            else if (symbolname.StartsWith("I_last_rpm!") || symbolname.StartsWith("Last_reg_ac!"))
            {
                yaxislength = GetSymbolLength("Last_varv_st!");
                yaxisaddress = GetSymbolAddress("Last_varv_st!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("I_last_temp!"))
            {
                yaxislength = GetSymbolLength("Last_temp_st!");
                yaxisaddress = GetSymbolAddress("Last_temp_st!");
            }
            else if (symbolname.StartsWith("Iv_start_time_tab!"))
            {
                yaxislength = GetSymbolLength("I_kyl_st!");
                yaxisaddress = GetSymbolAddress("I_kyl_st!");
            }

            /*else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                yaxislength = GetSymbolLength("Idle_ac_extra_sp!");
                yaxisaddress = GetSymbolAddress("Idle_ac_extra_sp!");
            }*/

            else if (symbolname.StartsWith("Idle_start_extra!"))
            {
                yaxislength = GetSymbolLength("Idle_start_extra_sp!");
                yaxisaddress = GetSymbolAddress("Idle_start_extra_sp!");
            }
            else if (symbolname.StartsWith("Hot_tab!"))
            {
                //7 : 60 70 80 90 100 110 115
                retval = new int[7];
                retval.SetValue(60, 0);
                retval.SetValue(70, 1);
                retval.SetValue(80, 2);
                retval.SetValue(90, 3);
                retval.SetValue(100, 4);
                retval.SetValue(110, 5);
                retval.SetValue(115, 6);
            }
            else if (symbolname.StartsWith("Last_reg_kon!"))
            {
                //4 : 1 2 3 4 
                retval = new int[4];
                retval.SetValue(1, 0);
                retval.SetValue(2, 1);
                retval.SetValue(3, 2);
                retval.SetValue(4, 3);
            }
            else if (symbolname.StartsWith("Kadapt_max_ref!"))
            {
                //11: 1500,2000,2500,3000,3500,4000,4500,5000,5500,6000,6500
                retval = new int[11];
                retval.SetValue(1500, 0);
                retval.SetValue(2000, 1);
                retval.SetValue(2500, 2);
                retval.SetValue(3000, 3);
                retval.SetValue(3500, 4);
                retval.SetValue(4000, 5);
                retval.SetValue(4500, 6);
                retval.SetValue(5000, 7);
                retval.SetValue(5500, 8);
                retval.SetValue(6000, 9);
                retval.SetValue(6500, 10);
            }
            else if (symbolname.StartsWith("Cyl_komp!"))
            {
                retval = new int[4];
                retval.SetValue(1, 0);
                retval.SetValue(2, 1);
                retval.SetValue(3, 2);
                retval.SetValue(4, 3);
            }
            else if (symbolname.StartsWith("Idle_der_tab!"))
            {
                retval = new int[8];
                retval.SetValue(0, 0);
                retval.SetValue(10, 1);
                retval.SetValue(20, 2);
                retval.SetValue(30, 3);
                retval.SetValue(40, 4);
                retval.SetValue(50, 5);
                retval.SetValue(60, 6);
                retval.SetValue(60, 7);
            }
            else if (symbolname.StartsWith("Idle_p_tab!"))
            {
                retval = new int[8];
                retval.SetValue(0, 0);
                retval.SetValue(30, 1);
                retval.SetValue(60, 2);
                retval.SetValue(90, 3);
                retval.SetValue(120, 4);
                retval.SetValue(150, 5);
                retval.SetValue(180, 6);
                retval.SetValue(200, 7);
            }
            else if (symbolname.StartsWith("Br_plus_tab!") || symbolname.StartsWith("Br_minus_tab!"))
            {
                retval = new int[8];
                retval.SetValue(10, 0);
                retval.SetValue(20, 1);
                retval.SetValue(30, 2);
                retval.SetValue(40, 3);
                retval.SetValue(50, 4);
                retval.SetValue(60, 5);
                retval.SetValue(70, 6);
                retval.SetValue(70, 7);
            }
            else if (symbolname.StartsWith("Iv_min_tab!") || symbolname.StartsWith("Iv_min_tab_ac!"))
            {
                retval = new int[54];
                retval.SetValue(120, 0);
                retval.SetValue(245, 1);
                retval.SetValue(370, 2);
                retval.SetValue(495, 3);
                retval.SetValue(620, 4);
                retval.SetValue(745, 5);
                retval.SetValue(870, 6);
                retval.SetValue(995, 7);
                retval.SetValue(1120, 8);
                retval.SetValue(1245, 9);
                retval.SetValue(1370, 10);
                retval.SetValue(1495, 11);
                retval.SetValue(1620, 12);
                retval.SetValue(1745, 13);
                retval.SetValue(1870, 14);
                retval.SetValue(1995, 15);
                retval.SetValue(2120, 16);
                retval.SetValue(2245, 17);
                retval.SetValue(2370, 18);
                retval.SetValue(2495, 19);
                retval.SetValue(2620, 20);
                retval.SetValue(2745, 21);
                retval.SetValue(2870, 22);
                retval.SetValue(2995, 23);
                retval.SetValue(3120, 24);
                retval.SetValue(3245, 25);
                retval.SetValue(3370, 26);
                retval.SetValue(3495, 27);
                retval.SetValue(3620, 28);
                retval.SetValue(3745, 29);
                retval.SetValue(3870, 30);
                retval.SetValue(3995, 31);
                retval.SetValue(4120, 32);
                retval.SetValue(4245, 33);
                retval.SetValue(4370, 34);
                retval.SetValue(4495, 35);
                retval.SetValue(4620, 36);
                retval.SetValue(4745, 37);
                retval.SetValue(4870, 38);
                retval.SetValue(4995, 39);
                retval.SetValue(5120, 40);
                retval.SetValue(5245, 41);
                retval.SetValue(5370, 42);
                retval.SetValue(5495, 43);
                retval.SetValue(5620, 44);
                retval.SetValue(5745, 45);
                retval.SetValue(5870, 46);
                retval.SetValue(5995, 47);
                retval.SetValue(6120, 48);
                retval.SetValue(6245, 49);
                retval.SetValue(6370, 50);
                retval.SetValue(6495, 51);
                retval.SetValue(6745, 52);
                retval.SetValue(6870, 53);
            }
            else if (symbolname.StartsWith("After_fcut_tab!"))
            {
                //10 : 4 8 12 16 20 24 28 32 36 40
                retval = new int[10];
                int v = 0;
                for (int ti = 4; ti <= 40; ti += 4)
                {
                    retval.SetValue(ti, v++);
                }
            }
            else if (symbolname.StartsWith("Batt_korr_tab!"))
            {
                retval = new int[11];
                int v = 0;
                for (int ti = 15; ti >= 5; ti--)
                {
                    retval.SetValue(ti, v++);
                }
            }
            else if (symbolname.StartsWith("Restart_corr_hp!"))
            {
                yaxislength = GetSymbolLength("Hp_support_points!");
                yaxisaddress = GetSymbolAddress("Hp_support_points!");
                //TEST
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;
                //TEST
            }
            else if (symbolname.StartsWith("Lam_minlast!"))
            {
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Lamb_tid!") || symbolname.StartsWith("Lamb_idle!") || symbolname.StartsWith("Lamb_ej!"))
            {

                yaxislength = GetSymbolLength("Lamb_kyl!");
                yaxisaddress = GetSymbolAddress("Lamb_kyl!");
                //TEST
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;
                //TEST
            }
            else if (symbolname.StartsWith("Overstid_tab!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("AC_wait_on!") || symbolname.StartsWith("AC_wait_off!"))
            {
                yaxislength = GetSymbolLength("I_luft_st!");
                yaxisaddress = GetSymbolAddress("I_luft_st!");
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Idle_temp_off!") || symbolname.StartsWith("Idle_rpm_tab!") || symbolname.StartsWith("Start_tab!"))
            {
                yaxislength = GetSymbolLength("I_kyl_st!");
                yaxisaddress = GetSymbolAddress("I_kyl_st!");
            }

            else if (symbolname.StartsWith("Overs_tab!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                yaxislength = GetSymbolLength("Inj_map_0_y_axis!");
                yaxisaddress = GetSymbolAddress("Inj_map_0_y_axis!");
                issixteenbit = true;
                //multiplier = 10;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Lambdamatris!") || symbolname.StartsWith("Lambdamatris_diag!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Adapt_ref"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Max_regl_temp_"))
            {
                yaxislength = GetSymbolLength("Max_regl_sp!");
                yaxisaddress = GetSymbolAddress("Max_regl_sp!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                yaxislength = GetSymbolLength("Knock_wind_rpm!");
                yaxisaddress = GetSymbolAddress("Knock_wind_rpm!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_ref_tab!"))
            {
                yaxislength = GetSymbolLength("Knock_ref_rpm!");
                yaxisaddress = GetSymbolAddress("Knock_ref_rpm!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_press_tab!") || symbolname.StartsWith("Lknock_oref_tab!") || symbolname.StartsWith("Knock_lim_tab!"))
            {
                yaxislength = GetSymbolLength("Wait_count_tab!");
                yaxisaddress = GetSymbolAddress("Wait_count_tab!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                yaxislength = GetSymbolLength("Dash_rpm_axis!");
                yaxisaddress = GetSymbolAddress("Dash_rpm_axis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                retval = new int[16];
                retval.SetValue(500, 0);
                retval.SetValue(830, 1);
                retval.SetValue(1160, 2);
                retval.SetValue(1480, 3);
                retval.SetValue(1810, 4);
                retval.SetValue(2140, 5);
                retval.SetValue(2460, 6);
                retval.SetValue(2790, 7);
                retval.SetValue(3120, 8);
                retval.SetValue(3440, 9);
                retval.SetValue(3770, 10);
                retval.SetValue(4100, 11);
                retval.SetValue(4420, 12);
                retval.SetValue(4750, 13);
                retval.SetValue(5070, 14);
                retval.SetValue(5400, 15);
                //yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                //yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                //issixteenbit = true;
                //multiplier = 10;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                //yaxislength = GetSymbolLength("Idle_rpm_tab!");
                //yaxisaddress = GetSymbolAddress("Idle_rpm_tab!");
                yaxislength = GetSymbolLength("Idle_st_rpm!");
                yaxisaddress = GetSymbolAddress("Idle_st_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                yaxislength = GetSymbolLength("Reg_varv!");
                yaxisaddress = GetSymbolAddress("Reg_varv!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                yaxislength = GetSymbolLength("Reg_varv!");
                yaxisaddress = GetSymbolAddress("Reg_varv!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab!"))
            {
                yaxislength = GetSymbolLength(/*"Fuel_map_yaxis!"*/"Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress(/*"Fuel_map_yaxis!"*/"Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
                //Fuel_map_yaxis!
            }
            else if (symbolname.StartsWith("Regl_tryck_sgm!") || symbolname.StartsWith("Regl_tryck_fgm!") || symbolname.StartsWith("Regl_tryck_fgaut!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
                //Fuel_map_yaxis!
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                yaxislength = GetSymbolLength("Misfire_map_y_axis!");
                yaxisaddress = GetSymbolAddress("Misfire_map_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                yaxislength = GetSymbolLength("Misfire_map_y_axis!");
                yaxisaddress = GetSymbolAddress("Misfire_map_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_") || symbolname.StartsWith("Tempkomp_konst!") || symbolname.StartsWith("Accel_temp!") || symbolname.StartsWith("Accel_temp2!") || symbolname.StartsWith("Retard_temp!") || symbolname.StartsWith("Throt_after_tab!") || symbolname.StartsWith("Throt_aft_dec_fak!"))
            {
                yaxislength = GetSymbolLength("Temp_steg!");
                yaxisaddress = GetSymbolAddress("Temp_steg!");
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                yaxislength = GetSymbolLength("Temp_reduce_y_st!");
                yaxisaddress = GetSymbolAddress("Temp_reduce_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                yaxislength = GetSymbolLength("Detect_map_y_axis!");
                yaxisaddress = GetSymbolAddress("Detect_map_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!"))
            {
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                    yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                    multiplier = 10;
                    issixteenbit = true;
                }
                else
                {
                    retval = new int[31];
                    int v = 0;
                    // get the step value
                    int incrementValue = GetRegulationDivisorValue();
                    int ti = 2500;
                    for (v = 0; v <= 30; v++)
                    {
                        ti = 2500 + (v * incrementValue * 10);
                        retval.SetValue(ti, v);

                    }
                }
            }
            int number = yaxislength;
            if (yaxislength > 0)
            {
                byte[] axisdata = readdatafromfile(filename, yaxisaddress, yaxislength);
                if (issixteenbit) number /= 2;
                retval = new int[number];
                int offset = 0;
                for (int i = 0; i < yaxislength; i++)
                {

                    if (issixteenbit)
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        byte val2 = (byte)axisdata.GetValue(++i);
                        int ival1 = Convert.ToInt32(val1);
                        int ival2 = Convert.ToInt32(val2);
                        int value = (ival1 * 256) + ival2;
                        value *= multiplier;
                        if (value > 0x8000)
                        {
                            value = 0x10000 - value;
                            value = -value;
                        }
                        retval.SetValue(value, offset++);
                    }
                    else
                    {
                        byte val1 = (byte)axisdata.GetValue(i);
                        int ival1 = Convert.ToInt32(val1);
                        ival1 *= multiplier;
                        if (symbolname.StartsWith("Iv_start_time_tab!") || symbolname.StartsWith("Idle_temp_off!") || symbolname.StartsWith("Idle_rpm_tab!") || symbolname.StartsWith("Start_tab!") || symbolname.StartsWith("I_last_temp!"))
                        {
                            if (ival1 > 0x80) ival1 = ival1 - 0x100;
                            //65 50 35 20 5 -5 -20 -40
                        }
                        retval.SetValue(ival1, offset++);
                    }
                }
            }
            return retval;
        }



        public void GetXYAxisAddresses(string symbolname, out int xaxisaddress, out int yaxisaddress, out bool isxaxis16bit, out bool isyaxis16bit)
        {
            xaxisaddress = 0;
            yaxisaddress = 0;
            isxaxis16bit = false;
            isyaxis16bit = false;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_1_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_1_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_2_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_2_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_3_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_3_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_5_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_5_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_6_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_6_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_7_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_7_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_8_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_8_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Restart_corr_hp!"))
            {
                yaxisaddress = GetSymbolAddress("Hp_support_points!");
            }

            else if (symbolname.StartsWith("Tryck_mat") || symbolname.StartsWith("Pressure map"))
            {
                //xaxisaddress = GetSymbolAddress("Trans_x_st!");
                xaxisaddress = GetSymbolAddress("Pwm_ind_trot!") + 32;
                yaxisaddress = GetSymbolAddress("Pwm_ind_rpm!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                xaxisaddress = GetSymbolAddress("Inj_map_0_x_axis!");
                yaxisaddress = GetSymbolAddress("Inj_map_0_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            /*else if (symbolname.StartsWith("Insp_mat!"))
            {
                yaxisaddress = GetSymbolAddress("Max_regl_sp!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }*/
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                //xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Knock_wind_rpm!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_press_tab!") || symbolname.StartsWith("Lknock_oref_tab!") || symbolname.StartsWith("Knock_lim_tab!"))
            {
                yaxisaddress = GetSymbolAddress("Wait_count_tab!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                xaxisaddress = GetSymbolAddress("Dash_trot_axis!");
                yaxisaddress = GetSymbolAddress("Dash_rpm_axis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                xaxisaddress = GetSymbolAddress("Idle_st_last!");
                yaxisaddress = GetSymbolAddress(/*"Idle_rpm_tab!"*/ "Idle_st_rpm!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            //retard_konst
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                xaxisaddress = GetSymbolAddress("Trans_x_st!");
                yaxisaddress = GetSymbolAddress("Trans_y_st!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                xaxisaddress = GetSymbolAddress("Reg_last!");
                yaxisaddress = GetSymbolAddress("Reg_varv!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                xaxisaddress = GetSymbolAddress("Reg_last!");
                yaxisaddress = GetSymbolAddress("Reg_varv!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_map_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                xaxisaddress = GetSymbolAddress("Fuel_knock_xaxis!");
                yaxisaddress = GetSymbolAddress("Fuel_map_yaxis!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                xaxisaddress = GetSymbolAddress("Misfire_map_x_axis!");
                yaxisaddress = GetSymbolAddress("Misfire_map_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                xaxisaddress = GetSymbolAddress("Misfire_map_x_axis!");
                yaxisaddress = GetSymbolAddress("Misfire_map_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_"))
            {
                yaxisaddress = GetSymbolAddress("Temp_steg!");
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                xaxisaddress = GetSymbolAddress("Temp_reduce_x_st!");
                yaxisaddress = GetSymbolAddress("Temp_reduce_y_st!");
                isxaxis16bit = false;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                yaxisaddress = GetSymbolAddress("Ign_map_0_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                xaxisaddress = GetSymbolAddress("Detect_map_x_axis!");
                yaxisaddress = GetSymbolAddress("Detect_map_y_axis!");
                isxaxis16bit = true;
                isyaxis16bit = true;
            }
        }

        public override long GetStartVectorAddress(string filename, int number)
        {
            long retval = 0;
            Int32 start_address = number * 4;
            retval = Convert.ToInt64(readdatafromfile(m_currentFile, start_address, 1)[0]) * 256 * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentFile, start_address + 1, 1)[0]) * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentFile, start_address + 2, 1)[0]) * 256;
            retval += Convert.ToInt64(readdatafromfile(m_currentFile, start_address + 3, 1)[0]);
            return retval;
        }

        public byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            try
            {
                FileStream fsi1 = new FileStream(filename, FileMode.Open, FileAccess.Read);
                //FileStream fsi1 = File.OpenRead(filename);
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
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        internal bool VerifyChecksum(string filename)
        {
            return verifychecksum(filename);
        }

        internal bool UpdateChecksum(string filename)
        {
            return updatechecksum(filename);
        }

        #region private file functions
        private int ReadEndMarker(string filename, int value)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    int filesize = 0x40000;
                    FileInfo finfo = new FileInfo(filename);
                    filesize = (int)finfo.Length;
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = filesize - 0x100;
                    fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                    byte[] inb = br.ReadBytes(0xFF);
                    int offset = 0;
                    for (int t = 0; t < 0xFF; t++)
                    {
                        if ((byte)inb.GetValue(t) == (byte)value)
                        {
                            // marker gevonden
                            // lees 6 terug
                            offset = t;
                            break;
                        }
                    }
                    string hexstr = string.Empty;
                    if (offset > 6)
                    {
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 1));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 2));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 3));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 4));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 5));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 6));
                    }
                    try
                    {
                        retval = Convert.ToInt32(hexstr, 16);
                        if (filesize == 0x40000)
                        {
                            retval -= filesize;
                        }
                        else
                        {
                            retval -= 0x60000;
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                    fs.Flush();
                    br.Close();
                    fs.Close();
                    fs.Dispose();
                }
            }
            return retval;
        }

        private int ReadMarkerAddress(string m_currentfile, int m_currentfile_size, int value, out int length, out string val)
        {
            int retval = 0;
            length = 0;
            val = string.Empty;
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(m_currentfile))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = m_currentfile_size - 0x100;

                    fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                    byte[] inb = br.ReadBytes(0xFF);
                    //int offset = 0;
                    for (int t = 0; t < 0xFF; t++)
                    {
                        if (((byte)inb.GetValue(t) == (byte)value) && ((byte)inb.GetValue(t + 1) < 0x30))
                        {
                            // marker gevonden
                            // lees 6 terug
                            retval = /*0x3FF00*/ fileoffset + t;
                            length = (byte)inb.GetValue(t + 1);
                            break;
                        }
                    }
                    fs.Seek((retval - length), SeekOrigin.Begin);
                    byte[] info = br.ReadBytes(length);
                    for (int bc = info.Length - 1; bc >= 0; bc--)
                    {
                        val += Convert.ToChar(info.GetValue(bc));
                    }
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();

                }
            }

            return retval;
        }

       



        #endregion

        #region private checksum functions

        private bool verifychecksum(string m_currentfile)
        {
            bool retval = false;
            uint checksum = 0;

            FileInfo m_fileInfo = new FileInfo(m_currentfile);
            int m_currentfile_size = (int)m_fileInfo.Length;

            int indexoffirstmarking = ReadEndMarker(m_currentfile, 0xFE) - 3;
            if (indexoffirstmarking > 0)
            {
                FileStream fsi1 = File.OpenRead(m_currentfile);
                BinaryReader br1 = new BinaryReader(fsi1);
                for (int tel = 0; tel < indexoffirstmarking + 4; tel++)
                {
                    Byte ib1 = br1.ReadByte();
                    checksum += ib1;
                }
                int fileoffset = m_currentfile_size - 4;
                fsi1.Position = fileoffset;
                uint readchecksum = (uint)br1.ReadByte() * 0x01000000;
                readchecksum += (uint)br1.ReadByte() * 0x00010000;
                readchecksum += (uint)br1.ReadByte() * 0x00000100;
                readchecksum += (uint)br1.ReadByte();
                if (checksum == readchecksum) retval = true;
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            return retval;
        }

        private bool updatechecksum(string filename)
        {
            if (FileInLibrary()) return true;
            bool retval = false;
            uint checksum = 0;
            int filesize = 0x40000;
            FileInfo finfo = new FileInfo(filename);
            filesize = (int)finfo.Length;
            int indexoffirstmarking = ReadEndMarker(filename, 0xFE) - 3;
            if (indexoffirstmarking > 0)
            {
                FileStream fsi1 = File.OpenRead(filename);
                BinaryReader br1 = new BinaryReader(fsi1);
                for (int tel = 0; tel < indexoffirstmarking + 4; tel++)
                {
                    Byte ib1 = br1.ReadByte();
                    checksum += ib1;
                }
                fsi1.Position = filesize - 0x04; 
                uint readchecksum = (uint)br1.ReadByte() * 0x01000000;
                readchecksum += (uint)br1.ReadByte() * 0x00010000;
                readchecksum += (uint)br1.ReadByte() * 0x00000100;
                readchecksum += (uint)br1.ReadByte();
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
                if (checksum != readchecksum)
                {
                    retval = true;
                    FileStream fsi2 = File.OpenWrite(filename);
                    fsi2.Position = filesize - 0x04; 
                    BinaryWriter bw1 = new BinaryWriter(fsi2);
                    bw1.Write((byte)((checksum & 0xFF000000) >> 24));
                    bw1.Write((byte)((checksum & 0x00FF0000) >> 16));
                    bw1.Write((byte)((checksum & 0x0000FF00) >> 8));
                    bw1.Write((byte)((checksum & 0x000000FF)));
                    fsi2.Flush();
                    bw1.Close();
                    fsi2.Close();
                    fsi2.Dispose();

                }
            }
            return retval;
        }
        #endregion

        private int[] Temp_steg = new int[1];

        public override int[] Temp_steg_array
        {
            get { return Temp_steg; }
            set { Temp_steg = value; }
        }
        private int[] Kyltemp_steg = new int[1];

        public override int[] Kyltemp_steg_array
        {
            get { return Kyltemp_steg; }
            set { Kyltemp_steg = value; }
        }
        private int[] Kyltemp_tab = new int[1];

        public override int[] Kyltemp_tab_array
        {
            get { return Kyltemp_tab; }
            set { Kyltemp_tab = value; }
        }
        private int[] Lamb_kyl = new int[1];

        public override int[] Lamb_kyl_array
        {
            get { return Lamb_kyl; }
            set { Lamb_kyl = value; }
        }
        private int[] Lufttemp_steg = new int[1];

        public override int[] Lufttemp_steg_array
        {
            get { return Lufttemp_steg; }
            set { Lufttemp_steg = value; }
        }
        private int[] Lufttemp_tab = new int[1];

        public override int[] Lufttemp_tab_array
        {
            get { return Lufttemp_tab; }
            set { Lufttemp_tab = value; }
        }
        private int[] Luft_kompfak = new int[1];

        public override int[] Luft_kompfak_array
        {
            get { return Luft_kompfak; }
            set { Luft_kompfak = value; }
        }

        public override int GetSymbolAsInt(string symbolname)
        {
            int retval = 0;
            byte[] bytes = readdatafromfile(m_fileInfo.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));
            if (bytes.Length == 1)
            {
                retval = Convert.ToInt32(bytes.GetValue(0));
            }
            else if (bytes.Length == 2)
            {
                retval = Convert.ToInt32(bytes.GetValue(0));
                retval *= 256;
                retval += Convert.ToInt32(bytes.GetValue(1));
            }
            else if (bytes.Length == 4)
            {
                retval = Convert.ToInt32(bytes.GetValue(0));
                retval *= 256 * 256 * 256;
                int temp = Convert.ToInt32(bytes.GetValue(1));
                temp *= 256 * 256;
                retval += temp;
                temp = Convert.ToInt32(bytes.GetValue(2));
                temp *= 256;
                retval += temp;
                retval += Convert.ToInt32(bytes.GetValue(3));

            }
            return retval;
        }

        public void GetAxisDescriptions(string filename, string symbolname, out string x, out string y, out string z)
        {
            x = "x-axis";
            y = "y-axis";
            z = "z-axis";

            if (symbolname.StartsWith("Ign_map_0!") || symbolname.StartsWith("Ign_map_2!") || symbolname.StartsWith("Ign_map_3!") || symbolname.StartsWith("Ign_map_4!") || symbolname.StartsWith("Ign_map_6!") || symbolname.StartsWith("Ign_map_7!"))
            {
                x = "MAP";
                y = "RPM";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Cyl_komp!"))
            {
                y = "Cyl. number";
                z = "Correction factor";
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                x = "MAP";
                y = "RPM";
                z = "# knocks";
            }
            else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                y = "Air temperature";
            }
            else if (symbolname.StartsWith("Lambdamatris!"))
            {
                x = "MAP";
                y = "RPM";
                //y = "MAP";
            }
            else if (symbolname.StartsWith("Lambdamatris_diag!"))
            {
                y = "RPM";
                x = "MAP";
                //y = "MAP";
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                x = "Ign.correction";
                y = "RPM difference";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                y = "RPM";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                x = "MAP";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Iv_min_tab!"))
            {
                z = "RPM";
            }
            else if (symbolname.StartsWith("Iv_min_tab_ac!"))
            {
                z = "RPM";
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                y = "CW Temperature";
            }
            else if (symbolname.StartsWith("Batt_korr_tab!"))
            {
                y = "Voltage";
                z = "ms";
            }
            else if (symbolname.StartsWith("Restart_corr_hp!"))
            {
                y = "Time";
            }
            else if (symbolname.StartsWith("Lam_minlast!"))
            {
                y = "RPM";
                z = "MAP";
            }
            else if (symbolname.StartsWith("Lamb_tid!") || symbolname.StartsWith("Lamb_idle!") || symbolname.StartsWith("Lamb_ej!"))
            {
                y = "CW Temperature";
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Tryck_mat_a!") || symbolname.StartsWith("Pressure map"))
            {
                x = "Throttle position";
                y = "RPM";
                z = "MAP";
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                x = "Throttle position";
                z = "MAP";
            }
            else if (symbolname.StartsWith("Limp_tryck_konst!"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                //x = "Throttle position";
                x = "Max. pressure";
                y = "RPM";
                //z = "MAP";
            }

            else if (symbolname.StartsWith("Retard_konst!"))
            {
                x = "Throttle position";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                x = "Throttle position";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                x = "MAP";
                y = "RPM";
                //z = "Injection time";
                z = "Correction factor";
            }
            else if (symbolname.StartsWith("Inj_map_0!"))
            {
                x = "TPS";
                y = "RPM";
                //z = "Injection time";
                z = "Correction factor";
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                x = "MAP";
                y = "RPM";
                //z = "Injection time";
                z = "Adaption";
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Max_regl_temp_"))
            {
                y = "RPM";
            }
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                y = "RPM";
            }
            else if (symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_average_tab!"))
            {
                y = "RPM";
                z = "MAP";
            }
            else if (symbolname.StartsWith("Knock_lim_tab!"))
            {
                y = "RPM";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Lknock_oref_tab!"))
            {
                y = "RPM";
            }
            else if (symbolname.StartsWith("Knock_press_tab!"))
            {
                y = "RPM";
                z = "MAP";
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                x = "MAP";
                y = "RPM";
                //                z = "Injection time";
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                x = "MAP";
                y = "RPM";
                z = "Adaption done";
            }

            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                x = "MAP";
                y = "RPM";
                z = "Injection correction";
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                x = "MAP";
                y = "RPM";
                z = "Fuel correction";
            }
            else if (symbolname.StartsWith("Kyltemp_tab!"))
            {
                z = "Degrees celcius +40";
                /*if (m_appSettings.TemperaturesInFahrenheit)
                {
                    z = "Degrees Fahrenheit";
                }*/
                y = "A/D values coolant temp.";
            }
            else if (symbolname.StartsWith("Lufttemp_tab!"))
            {
                z = "Degrees celcius +40";
                /*if (m_appSettings.TemperaturesInFahrenheit)
                {
                    z = "Degrees Fahrenheit";
                }*/
                y = "A/D values air temp.";
            }
            else if (symbolname.StartsWith("P_fors"))
            {
                y = "RPM";
                x = "Pressure error (bar)";
                z = "P factor";

            }
            else if (symbolname.StartsWith("I_fors"))
            {
                y = "RPM";
                //x = "Load";
                x = "Pressure error (bar)";
                z = "I factor";
            }
            else if (symbolname.StartsWith("D_fors"))
            {
                y = "RPM";
                //x = "Load";
                x = "Pressure error (bar)";
                z = "D factor";

            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                //                x = "RPM";
                //                y = "MAP";
                x = "MAP";
                y = "RPM";
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!"))
            {
                x = "MAP"; //?
                y = "RPM";
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_"))
            {
                //x = "MAP"; //?
                y = "CW Temperature";
            }
            else if (symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                x = "MAP"; //??
                y = "RPM";
            }
            else if (symbolname.StartsWith("Mis1000_map!"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Mis200_map!"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Misfire_map!")) // T5.2 table
            {
                x = "MAP";
                y = "RPM";
            }

            else if (symbolname.StartsWith("Detect_map!"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                x = "MAP";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Lacc_konst!") || symbolname.StartsWith("Lret_konst!"))
            {
                x = "Engine load delta";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                x = "Relative throttle value";
                y = "RPM";
            }
            else if (symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!"))
            {
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    x = "Relative throttle position";
                }
                y = "RPM";
                z = "BCV position (% to return hose)";
            }
            else if (symbolname.StartsWith("Apc_knock_tab!"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Knock_lim_tab!") || symbolname.StartsWith("Knock_average") || symbolname.StartsWith("Knock_lim") || symbolname.StartsWith("Knock_ang_dec!") || symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Ign_idle_angle_start"))
            {
                z = "Degrees";
            }
            else if (symbolname.StartsWith("Regl_tryck"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Tryck_vakt"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Grund_last"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Lam_laststeg"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Idle_st_last"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Kadapt_load"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Diag_speed_load"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Iv_min_load"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Shift_load"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Shift_up_load"))
            {
                z = "MAP";
            }
            else if (symbolname.StartsWith("Fload_tab!"))
            {
                z = "MAP";
            }
        }

        private bool isSixteenBitTable(string symbolname)
        {
            return m_fileInfo.isSixteenBitTable(symbolname);
        }

        public override int[] GetSymbolAsIntArray(string symbolname)
        {
            int[] retval = new int[1];
            byte[] bytes = readdatafromfile(m_fileInfo.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));
            if (isSixteenBitTable(symbolname))
            {
                retval = new int[bytes.Length / 2];
                int idx = 0;
                for (int t = 0; t < bytes.Length; t += 2)
                {
                    int v = Convert.ToInt32(bytes.GetValue(t)) * 256;
                    v += Convert.ToInt32(bytes.GetValue(t + 1));
                    retval.SetValue(v, idx++);
                }
            }
            else
            {
                retval = new int[bytes.Length];
                for (int t = 0; t < bytes.Length; t++)
                {
                    retval.SetValue(Convert.ToInt32(bytes.GetValue(t)), t);
                }
            }
            return retval;
        }

        /// <summary>
        /// Tries to load the temperature conversion tables to memory
        /// This is done because several conversions have to be done on temperature axis display
        /// and would need much file access.
        /// </summary>
        private void TryToLoadTemperatureConversionTables()
        {
            /*
For Air and Coolant temp.
Input: AD value
Find index from *steg table.
use that to *temp_tab and substract 40, you have temperature in celcius (this for Kyl_temp_tab! and Luft_temp_tab!)
             * */

            /*About temp_reduce_mat:
Temp_fak_tab value comes from Tempkomp_konst(input AD_kylv) and then it is multiplied with value from temp_reduce_mat 
In math: Temp_fak=(temp_fak_tab*Temp_reduce_mat_value)/255
Temp_fak is one part for injection time calculation.*/
                // COOLANT WATER
                Temp_steg = GetSymbolAsIntArray("Temp_steg!");
                Kyltemp_steg = GetSymbolAsIntArray("Kyltemp_steg!");
                Kyltemp_tab = GetSymbolAsIntArray("Kyltemp_tab!");
                Lamb_kyl = GetSymbolAsIntArray("Lamb_kyl!");
                Lufttemp_steg = GetSymbolAsIntArray("Lufttemp_steg!");
                Lufttemp_tab = GetSymbolAsIntArray("Lufttemp_tab!");
                Luft_kompfak = GetSymbolAsIntArray("Luft_kompfak!");
        }



        private int LookupCoolantTemperature(int axisvalue)
        {
            // find index in Kyltemp_steg
            int index = -1;
            int retval = -1;
            int smallestdiff = 256;
            int idx = 0;
            int secondvalue = -1;
            try
            {
                foreach (int i in Kyltemp_steg)
                {
                    if (Math.Abs(i - axisvalue) < smallestdiff)
                    {
                        index = idx;
                        smallestdiff = (int)Math.Abs(i - axisvalue);
                        if (i < axisvalue)
                        {
                            secondvalue = (int)Kyltemp_steg.GetValue(idx + 1);
                        }
                        else
                        {
                            secondvalue = (int)Kyltemp_steg.GetValue(idx - 1);
                        }
                    }
                    idx++;
                }
                if (index >= 0 && index < Kyltemp_tab.Length)
                {
                    // get value from Kyltemp_tab
                    retval = (int)Kyltemp_tab.GetValue(index);
                    int firstvalue = (int)Kyltemp_steg.GetValue(index);
                    int sval = -1000;
                    int diff = (int)Math.Abs(secondvalue - firstvalue);
                    int diff2 = axisvalue - firstvalue;
                    double percentage = (double)diff2 / (double)diff;
                    if (secondvalue > firstvalue)
                    {
                        // dan moeten we de volgende uit kyltemp_tab ook hebben
                        sval = (int)Kyltemp_tab.GetValue(index + 1);
                        percentage = (double)diff2 / (double)diff;
                    }
                    else
                    {
                        sval = (int)Kyltemp_tab.GetValue(index - 1);
                        percentage = (double)diff2 / (double)diff;
                        percentage = -percentage;

                    }
                    // hoeveel interpoleren?

                    retval += (int)((double)percentage * (double)(sval - retval));
                    retval -= 40;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            // Console.WriteLine("looked for : " + axisvalue.ToString() + " found idx in kyltemp_steg : " + index.ToString() + " value in kyltemp_tab:  " + retval.ToString());

/*            if (m_appSettings.TemperaturesInFahrenheit)
            {
                retval = ConvertToFahrenheit(retval);
            }*/

            return retval;

        }

        private int ConvertToFahrenheit(int celcius)
        {
            //F =  C × 1.8 + 32
            int retval = (celcius * 18) / 10 + 32;
            return retval;
        }
        private double ConvertToFahrenheit(double celcius)
        {
            //F =  C × 1.8 + 32
            double retval = (celcius * 1.8) + 32;
            return retval;
        }


        private int LookupAirTemperature(int axisvalue)
        {
            // find index in Lufttemp_steg
            int index = -1;
            int retval = -1;
            int smallestdiff = 256;
            int idx = 0;
            int secondvalue = -1;
            try
            {
                foreach (int i in Lufttemp_steg)
                {
                    if (Math.Abs(i - axisvalue) < smallestdiff)
                    {
                        index = idx;
                        smallestdiff = (int)Math.Abs(i - axisvalue);
                        try
                        {
                            if (i < axisvalue)
                            {
                                secondvalue = (int)Lufttemp_steg.GetValue(idx + 1);
                            }
                            else
                            {
                                secondvalue = (int)Lufttemp_steg.GetValue(idx - 1);
                            }
                        }
                        catch (Exception sE)
                        {
                            Console.WriteLine(sE.Message);
                        }
                    }
                    idx++;
                }
                if (index >= 0 && index < Lufttemp_tab.Length)
                {
                    // get value from Lufttemp_tab
                    retval = (int)Lufttemp_tab.GetValue(index);
                    int firstvalue = (int)Lufttemp_steg.GetValue(index);
                    int sval = -1000;
                    int diff = (int)Math.Abs(secondvalue - firstvalue);
                    int diff2 = axisvalue - firstvalue;
                    double percentage = (double)diff2 / (double)diff;
                    if (secondvalue >= 0)
                    {
                        if (secondvalue > firstvalue)
                        {
                            // dan moeten we de volgende uit Lufttemp_tab ook hebben
                            sval = (int)Lufttemp_tab.GetValue(index + 1);
                            percentage = (double)diff2 / (double)diff;
                        }
                        else
                        {
                            sval = (int)Lufttemp_tab.GetValue(index - 1);
                            percentage = (double)diff2 / (double)diff;
                            percentage = -percentage;

                        }
                        // hoeveel interpoleren?
                        retval += (int)((double)percentage * (double)(sval - retval));
                    }
                    retval -= 40;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            /*if (m_appSettings.TemperaturesInFahrenheit)
            {
                retval = ConvertToFahrenheit(retval);
            }*/

            return retval;

        }

        #region private functions

        private bool TryToAddSymbolToCollection(string hexstring, SymbolCollection curSymbolCollection)
        {
            //1A 0C 00 05 41 4D 4F 53 5F 73 74 61 74 75 73 00
            //19 3A 00 04 49 6E 74 5F 61 64 72 65 73 73
            //19 74 00 04 44 61 5F 35
            //40 B6 00 01 42 75 69 6C 64 5F 75 70 5F 74 69 6D 65 5F 73 77 69 74 63 68 21 00
            bool retval = false;
            int address = 0x00000;
            int length = 0x00;
            bool symbol_invalid = false;
            int invalid_char_count = 0;
            string name = string.Empty;
            try
            {
                address = Convert.ToInt32(hexstring.Substring(0, 5).Replace(" ", ""), 16);
                length = Convert.ToInt32(hexstring.Substring(6, 5).Replace(" ", ""), 16);
                for (int i = 12; i < hexstring.Length; i += 3)
                {
                    int val = Convert.ToInt16(hexstring.Substring(i, 3).Replace(" ", ""), 16);
                    if (val < 10) invalid_char_count++;
                    if (invalid_char_count > 2) symbol_invalid = true;
                    name += Convert.ToChar(val);
                }

                if (symbol_invalid)
                {
                    //AddLogItem("Found invalid symbol: " + name + " at " + address.ToString("X4") + " len " + length.ToString("X4"));
                }
                else
                {
                    name = name.Replace("\0", "");
                    if (name.Length > 0)
                    {
                        SymbolHelper sh = new SymbolHelper();
                        sh.Varname = name;
                        sh.Start_address = address;
                        sh.Length = length;
                        curSymbolCollection.Add(sh);
                        /*if (name == "Tryck_mat!")
                        {
                            SymbolHelper sh2 = new SymbolHelper();
                            sh2.Varname = "Pressure map scaled for 3 bar mapsensor";
                            sh2.Start_address = address;
                            sh2.Length = length;
                            curSymbolCollection.Add(sh2);
                        }
                        if (name == "Tryck_mat_a!")
                        {
                            SymbolHelper sh2 = new SymbolHelper();
                            sh2.Varname = "Pressure map (AUT) scaled for 3 bar mapsensor";
                            sh2.Start_address = address;
                            sh2.Length = length;
                            curSymbolCollection.Add(sh2);
                        }*/

                        retval = true;
                    }
                    else
                    {
                        retval = false;
                    }
                }

            }
            catch (Exception E)
            {
//                AddLogItem("Failed to convert symbol: " + E.Message);
                Console.WriteLine(E.Message);
            }
            return retval;
        }
        private void ReadAddressLookupTableFromFile(int startofsymboltable, int numberofsymbols,  AddressLookupCollection curAddressLookupCollection, string filename)
        {
            // find the address lookuptable
            // this table starts with 48 79 00 04
            int readstate = 0;
            int readaddress = 0;
            int lookuptablestartaddress = 0x00;

            // test
            readstate = -30;
            // test

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] filebytes = br.ReadBytes((int)fs.Length);
                for (int t = 0; t < filebytes.Length; t++)
                {
                    byte b = filebytes[t];
                /*for (int t = 0; t < fs.Length; t++)
                {
                    byte b = br.ReadByte();*/
                    //4E 75 48 E7 01 30 26 6F 00 16 3E 2F 00 14 24 6F 00 10 60 00 00 0A
                    switch (readstate)
                    {
                        case -30:
                            if (b == 0x4E)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -29:
                            if (b == 0x75)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -28:
                            if (b == 0x48)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -27:
                            if (b == 0xE7)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -26:
                            if (b == 0x01)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -25:
                            if (b == 0x30)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -24:
                            if (b == 0x26)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -23:
                            if (b == 0x6F)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -22:
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -21:
                            if (b == 0x16)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -20:
                            if (b == 0x3E)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -19:
                            if (b == 0x2F)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -18:
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -17:
                            if (b == 0x14)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -16:
                            if (b == 0x24)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -15:
                            if (b == 0x6F)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -14:
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -13:
                            if (b == 0x10)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -12:
                            if (b == 0x60)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -11:
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -10:
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;
                        case -9:
                            if (b == 0x0A)
                            {
                                readstate = 0;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = -30;
                            }
                            break;

                        case 0:
                            // waiting for first recognition char 48
                            if (b == 0x48)
                            {
                                lookuptablestartaddress = t;
                                readstate++;
                            }
                            break;
                        case 1:
                            // waiting for second char 79
                            if (b == 0x79)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = 0;
                            }
                            break;
                        case 2:
                            // waiting for third char 00
                            if (b == 0x00)
                            {
                                readstate++;
                            }
                            else
                            {
                                lookuptablestartaddress = 0x00;
                                readstate = 0;
                            }
                            break;
                        case 3:
                            // waiting for last char 04
                            if (fs.Length == 0x20000)
                            {
                                if (b == 0x06)
                                {
                                    readstate++;
                                }
                                else
                                {
                                    lookuptablestartaddress = 0x00;
                                    readstate = 0;
                                }
                            }
                            else
                            {
                                if (b == 0x04)
                                {
                                    readstate++;
                                }
                                else
                                {
                                    lookuptablestartaddress = 0x00;
                                    readstate = 0;
                                }
                            }
                            break;
                        default:
                            break;

                    }
                }
            }
            //fs.Flush();
            fs.Close();
            fs.Dispose();



            FileStream fs2 = new FileStream(filename, FileMode.Open, FileAccess.Read);
            fs2.Seek(lookuptablestartaddress + 2, SeekOrigin.Begin);
            readaddress = lookuptablestartaddress + 2;
            using (BinaryReader br = new BinaryReader(fs2))
            {
                for (int sc = 0; sc < numberofsymbols; sc++)
                {
                    if (readaddress >= startofsymboltable) break;
                    int sramaddress = 0;
                    int flashaddress = 0;
                    byte b = br.ReadByte();
                    flashaddress = b * 256 * 256 * 256;
                    b = br.ReadByte();
                    flashaddress += (b * 256 * 256);
                    b = br.ReadByte();
                    flashaddress += (b * 256);
                    b = br.ReadByte();
                    flashaddress += (b);
                    // 8 x dummy
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    b = br.ReadByte();
                    // lees sram adres
                    b = br.ReadByte();
                    sramaddress = (b * 256);
                    b = br.ReadByte();
                    sramaddress += (b);
                    AddressLookupHelper alh = new AddressLookupHelper();
                    alh.Flash_address = flashaddress;
                    alh.Sram_adddress = sramaddress;
                    curAddressLookupCollection.Add(alh);
                    /*using (StreamWriter sw = new StreamWriter("lookupcollection.txt", true))
                    {
                        sw.WriteLine("Lookup entry: " + alh.Sram_adddress.ToString("X4") + " : " + alh.Flash_address.ToString("X6"));
                    }*/

                    // lees naar volgende 48 79
                    int tel = 0;
                    bool found = false;
                    int tstate = 0;
                    while (tel++ < 16 && !found)
                    {
                        byte tb = br.ReadByte();
                        switch (tstate)
                        {
                            case 0:
                                if (tb == 0x048) tstate++;
                                break;
                            case 1:
                                if (tb == 0x79) found = true;
                                else tstate = 0;
                                break;
                        }
                    }
                    // als niet gevonden.. ?
                    if (!found)
                    {
                        break;
                    }

                }
                //lees 4 bytes address
                /*
                for (int fa = 0; fa < 4; fa++)
                {

                }*/

            }
            //fs2.Flush();
            fs2.Close();
            fs2.Dispose();
            /*using (StreamWriter sw = new StreamWriter("lookupdump.txt", false))
            {
                foreach (AddressLookupHelper ah in m_addresslookup)
                {
                    sw.WriteLine("Helper: " + ah.Sram_adddress.ToString("X4") + " " + ah.Flash_address.ToString("X8"));
                }
            }*/
        }

        private string FetchSymbolNameFromSequenceList(int idx, string previousssymbolname)
        {
            string symbolname = "";
            try
            {
                if (indexedSymbolList == null)
                {
                    if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\symbolindex.xml"))
                    {
                        indexedSymbolList = new System.Data.DataTable("DEFAULT");
                        indexedSymbolList.Columns.Add("SYMBOLNAME");
                        indexedSymbolList.Columns.Add("SRAMADDRESS", Type.GetType("System.Int32"));
                        indexedSymbolList.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                        indexedSymbolList.Columns.Add("LENGTHBYTES", Type.GetType("System.Int32"));
                        indexedSymbolList.Columns.Add("LENGTHVALUES", Type.GetType("System.Int32"));
                        indexedSymbolList.Columns.Add("DESCRIPTION");
                        indexedSymbolList.Columns.Add("ISCHANGED", Type.GetType("System.Boolean"));
                        indexedSymbolList.Columns.Add("CATEGORY", Type.GetType("System.Int32"));
                        indexedSymbolList.ReadXml(System.Windows.Forms.Application.StartupPath + "\\symbolindex.xml");
                    }
                }
                bool fetchnext = false;
                if (indexedSymbolList != null)
                {
                    foreach (DataRow dr in indexedSymbolList.Rows)
                    {
                        if (dr["SYMBOLNAME"] != DBNull.Value)
                        {
                            string cursymbol = dr["SYMBOLNAME"].ToString();
                            if (cursymbol == previousssymbolname)
                            {
                                fetchnext = true;
                                Console.WriteLine("Previous symbol was: " + previousssymbolname);
                            }
                            else if (fetchnext)
                            {
                                fetchnext = false;
                                symbolname = dr["SYMBOLNAME"].ToString();
                                Console.WriteLine("Fetched symbolname to replace: " + symbolname);
                                break;
                            }
                        }
                    }

                    /*                    if (idx > 0)
                                        {
                                            if (indexedSymbolList.Rows.Count >= idx)
                                            {
                                                if (indexedSymbolList.Rows[idx - 1]["SYMBOLNAME"] != DBNull.Value)
                                                {
                                                    symbolname = indexedSymbolList.Rows[idx - 1]["SYMBOLNAME"].ToString();
                                                }
                                            }
                                        }*/
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("FetchSymbolNameFromSequenceList: " + E.Message);
            }
            return symbolname;
        }

        private void FixMaskedSymbols(SymbolCollection thisSymbolCollection)
        {
            int idx = 0;
            string previoussymbol = string.Empty;
            foreach (SymbolHelper sh in thisSymbolCollection)
            {
                if (sh.Varname.Trim() == "")
                {
                    Console.WriteLine("Fetching symbolname for symbol after: " + previoussymbol);
                    sh.Varname = FetchSymbolNameFromSequenceList(idx, previoussymbol);
                }
                previoussymbol = sh.Varname;
                idx++;
            }
        }

        private void ParseFile(string filename, SymbolCollection curSymbolCollection, AddressLookupCollection curAddressLookupCollection)
        {
            // lezen tot END$ (einde symbol table)
            
            int m_symboltablestartaddress = 0;
            int state = 0;
            int charcount = 0;
            string dbgstring = string.Empty;
            string asciistring = string.Empty;

            // for test
            state = -10;
            // for test
            SignalDecodeProgress(5);

            //listBox1.Items.Clear();
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] filebytes = br.ReadBytes((int)fs.Length);
                    for (int t = 0; t < filebytes.Length; t++)
                    {

                        byte b = filebytes[t];
                    /*for (int t = 0; t < fs.Length; t++)
                    {

                        byte b = br.ReadByte();*/
                        // first find string 00 0A 28 79 00
                        switch (state)
                        {
                            case -10:
                                if (b == 0)
                                {
                                    state++;
                                }
                                break;
                            case -9:
                                if (b == 0x0a)
                                {
                                    state++;
                                }
                                else
                                {
                                    state = -10;
                                }
                                break;
                            case -8:
                                if (b == 0x28)
                                {
                                    state++;
                                }
                                else
                                {
                                    state = -10;
                                }
                                break;
                            case -7:
                                if (b == 0x79)
                                {
                                    state++;
                                }
                                else
                                {
                                    state = -10;
                                }
                                break;
                            case -6:
                                if (b == 0x00)
                                {
                                    state++;
                                }
                                else
                                {
                                    state = -10;
                                }
                                break;
                            case -5:
                                if (b == 0x4E)
                                {
                                    state++;
                                }
                                break;
                            case -4:
                                if (b == 0x75)
                                {
                                    state = 2;
                                }
                                else
                                {
                                    state = -5;
                                }
                                break;

                            case 0:
                                if (b == 0x0d)
                                {
                                    //                                    if (br.PeekChar() == 0x0a)
                                    //                                    {
                                    state++;
                                    //                                    }
                                }
                                break;
                            case 1:
                                if (b == 0x0a)
                                {
                                    state++;
                                    charcount = 0;
                                }
                                else
                                {
                                    state = 0;
                                }
                                break;
                            case 2:
                                if (charcount < 32)
                                {
                                    if (b == 0x0d && /*br.PeekChar()*/ filebytes[t+1] == 0x0a) // start of next symbol
                                    {
                                        state = 1;
                                        int address = t - charcount;
                                        //AddLogItem(address.ToString("X6") + ": " + dbgstring + " - " + asciistring);
                                        TryToAddSymbolToCollection(dbgstring, curSymbolCollection);

                                        if (m_symboltablestartaddress == 0 && t > 0xA000) m_symboltablestartaddress = address;
                                        dbgstring = string.Empty;
                                        asciistring = string.Empty;
                                    }
                                    else
                                    {
                                        charcount++;
                                        dbgstring += b.ToString("X2") + " ";
                                        if (b >= 0x20 && b < 0x80)
                                        {
                                            asciistring += Convert.ToChar(b);
                                        }
                                        else
                                        {
                                            asciistring += ".";
                                        }
                                    }
                                }
                                else
                                {
                                    int address = t - charcount;
                                    // AddLogItem(address.ToString("X6") + ": " + dbgstring + " - " + asciistring);
                                    TryToAddSymbolToCollection(dbgstring, curSymbolCollection);

                                    if (m_symboltablestartaddress == 0 && t > 0xA000) m_symboltablestartaddress = address;
                                    //TryToAddSymbolToCollection(dbgstring);
                                    if (asciistring.StartsWith("END$"))
                                    {
                                        br.Close();
                                        fs.Close();
                                        //button1.Enabled = true;
                                        //AddLogItem("Found end of symbol table, quit");
                                        return;
                                    }
                                    else
                                    {
                                        dbgstring = string.Empty;
                                        asciistring = string.Empty;
                                        state = 0;
                                    }
                                }
                                break;
                        }
                    }
                    br.Close();
                }
                //fs.Flush();
                fs.Close();
                fs.Dispose();
                SignalDecodeProgress(30);

                // now sort the symbols...
            }
            catch (Exception E)
            {
                //MessageBox.Show(E.Message);
                Console.WriteLine(E.Message);
            }
            finally
            {
                //button1.Enabled = true;
                try
                {
                    // fix masked symbolnames...
                    try
                    {
                        FixMaskedSymbols(curSymbolCollection);
                        SignalDecodeProgress(40);

                    }
                    catch (Exception fmsE)
                    {
                        Console.WriteLine("Failed to fix masked symbols: " + fmsE.Message);
                    }
                    try
                    {
                        if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\symbolindex.xml"))
                        {
                            System.Data.DataTable dt = new System.Data.DataTable("DEFAULT");
                            dt.Columns.Add("SYMBOLNAME");
                            foreach (SymbolHelper sh in curSymbolCollection)
                            {
                                dt.Rows.Add(sh.Varname);
                            }
                            dt.WriteXml(System.Windows.Forms.Application.StartupPath + "\\symbolindex.xml");
                        }
                    }
                    catch (Exception wsiE)
                    {
                        Console.WriteLine("Failed to write symbolindex: " + wsiE.Message);
                    }
                    SignalDecodeProgress(50);

                    ReadAddressLookupTableFromFile(m_symboltablestartaddress, curSymbolCollection.Count, curAddressLookupCollection, filename);
                    //Console.WriteLine("Read address lookup table. Symbolcount = " + curSymbolCollection.Count.ToString() + " addresses = " + curAddressLookupCollection.Count.ToString());
                    SignalDecodeProgress(70);

                    foreach (SymbolHelper sh in curSymbolCollection)
                    {
                        foreach (AddressLookupHelper alh in curAddressLookupCollection)
                        {
                            if (sh.Start_address == alh.Sram_adddress)
                            {
                                sh.Flash_start_address = alh.Flash_address;
                                alh.UsedAddress = true;
                                break;
                            }
                        }
                    }
                    SignalDecodeProgress(80);

                    foreach (AddressLookupHelper alh in curAddressLookupCollection)
                    {
                        if (!alh.UsedAddress)
                        {
                            Console.WriteLine("Unused addresshelper: " + alh.Flash_address.ToString("X6") + " " + alh.Sram_adddress.ToString("X4"));
                        }

                    }
                    SignalDecodeProgress(90);

                }
                catch (Exception finE)
                {
                    Console.WriteLine(finE.Message);
                }
            }

        }
        #endregion

        public override long[] GetVectorAddresses(string filename)
        {
            long[] vectors = new long[256];
            for (int i = 0; i < 256; i++)
            {
                vectors.SetValue(GetStartVectorAddress(filename, i), i);
            }
            return vectors;
        }
    }
}
