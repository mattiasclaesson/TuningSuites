using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PSTaskDialog;
using CommonSuite;

namespace Trionic5Tools
{
    public enum TuningResult : int
    {
        TuningSuccess,
        TuningFailedAlreadyTuned,
        TuningFailedThreebarSensor,
        TuningFailed,
        TuningCancelled
    }

    public class Trionic5Tuner
    {
        private bool m_autoUpdateChecksum = false;

        public bool AutoUpdateChecksum
        {
            get { return m_autoUpdateChecksum; }
            set { m_autoUpdateChecksum = value; }
        }

        private Trionic5Resume m_resume;

        public Trionic5Resume Resume
        {
            get
            {
                if (m_resume == null) m_resume = new Trionic5Resume();
                return m_resume;
            }
            set { m_resume = value; }
        }
        private IECUFileInformation m_fileInformation = new Trionic5FileInformation();

        private string ConvertToStageDescription(int stage)
        {
            string retval = stage.ToString();
            switch (stage)
            {
                case 1:
                    retval = "I";
                    break;
                case 2:
                    retval = "II";
                    break;
                case 3:
                    retval = "III";
                    break;
                case 4:
                    retval = "IV";
                    break;
                case 5:
                    retval = "V";
                    break;
                case 6:
                    retval = "VI";
                    break;
                case 7:
                    retval = "VII";
                    break;
                case 8:
                    retval = "VIII";
                    break;
                case 99:
                    retval = "X";
                    break;
            }
            return retval;
        }

        public TuningResult TuneFileToStage(int stage, string filename, IECUFile m_TrionicFile, IECUFileInformation trionicFileInformation, bool SilentMode)
        {
            //<COPY FROM HERE>
            m_fileInformation = trionicFileInformation;
            m_resume = new Trionic5Resume();
            TuningResult retval = TuningResult.TuningFailed;
            string enginetp = readenginetype(filename);
            string partnumber = readpartnumber(filename);
            PartNumberConverter pnc = new PartNumberConverter();
            ECUInformation ecuinfo = pnc.GetECUInfo(partnumber, enginetp);
            bool isLpt = false;
            if (ReadTunedToStageMarker(filename) > 0 && !SilentMode)
            {
                retval = TuningResult.TuningFailedAlreadyTuned;
            }
            else if (ReadThreeBarConversionMarker(filename) > 0 && !SilentMode)
            {
                retval = TuningResult.TuningFailedThreebarSensor;
            }
            else if (SilentMode)
            {
                Trionic5Properties t5p = m_TrionicFile.GetTrionicProperties();
                if (stage == 1)
                {
                    TuneToStage(filename, stage, ecuinfo.Stage1boost, 0.72, 1.54, 0.62, ecuinfo.Stage1boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                    retval = TuningResult.TuningSuccess;
                }
                else if (stage == 2)
                {
                    TuneToStage(filename, stage, ecuinfo.Stage2boost, 0.72, 1.54, 0.62, ecuinfo.Stage2boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                    retval = TuningResult.TuningSuccess;
                }
                else if (stage == 3)
                {
                    TuneToStage(filename, stage, ecuinfo.Stage3boost, 0.72, 1.54, 0.62, ecuinfo.Stage3boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                    retval = TuningResult.TuningSuccess;
                }
            }
            else
            {
                Trionic5Properties t5p = m_TrionicFile.GetTrionicProperties();
                string msg = string.Empty;
                if (ecuinfo.Valid)
                {
                    msg = "Tuning your: " + ecuinfo.Bhp.ToString() + " bhp ";
                    msg += ecuinfo.Carmodel.ToString() + " (" + ecuinfo.Enginetype.ToString() + ") ";
                    if (ecuinfo.Is2point3liter) msg += " 2.3 liter ";
                    else msg += " 2.0 liter ";
                    if (ecuinfo.Isaero)
                    {
                        t5p.TurboType = TurboType.TD0415T;
                        msg += " Aero binary";
                    }
                    else if (ecuinfo.Isfpt) msg += " Full pressure turbo binary";
                    else if (ecuinfo.Isturbo)
                    {
                        msg += " Low pressure turbo, you'll have to modify hardware (solenoid valve, hoses etc.) to get this working!";
                        isLpt = true;
                    }
                    else msg += " non turbo car to stage, you'll have to modify hardware to get this working!";
                }
                else
                {
                    msg = "Partnumber not recognized, tuning will continue anyway, please verify settings afterwards";
                }
                PSTaskDialog.cTaskDialog.ForceEmulationMode = false;
                PSTaskDialog.cTaskDialog.EmulatedFormWidth = 600;
                PSTaskDialog.cTaskDialog.UseToolWindowOnXP = false;
                PSTaskDialog.cTaskDialog.VerificationChecked = true;
                string stageDescription = ConvertToStageDescription(stage);
                PSTaskDialog.cTaskDialog.ShowTaskDialogBox("Tune me up™ to stage " + stageDescription + " wizard", "This wizard will tune your binary to a stage " + stageDescription + " equivalent.", "Boost request map, fuel injection and ignition tables will be altered" + Environment.NewLine + msg, "Happy driving!!!\nDilemma © 2009", "The author does not take responsibility for any damage done to your car or other objects in any form!", "Show me a summary after tuning", "", "Yes, tune me to stage " + stageDescription + "|No thanks!", PSTaskDialog.eTaskDialogButtons.None, PSTaskDialog.eSysIcons.Information, PSTaskDialog.eSysIcons.Warning);
                switch (PSTaskDialog.cTaskDialog.CommandButtonResult)
                {
                    case 0:
                        // tune to stage 1
                        if (stage == 1)
                        {
                            TuneToStage(filename, stage, ecuinfo.Stage1boost, 0.72, 1.54, 0.62, ecuinfo.Stage1boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                        }
                        else if (stage == 2)
                        {
                            TuneToStage(filename, stage, ecuinfo.Stage2boost, 0.72, 1.54, 0.62, ecuinfo.Stage2boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                        }
                        else if (stage == 3)
                        {
                            TuneToStage(filename, stage, ecuinfo.Stage3boost, 0.72, 1.54, 0.62, ecuinfo.Stage3boost, 90, isLpt, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                        }
                        else if (stage == 99) // stage X
                        {
                            // get parameters from user:
                            // max boost, turbo type, injector type, rpm limit etc etc
                            frmTuningSettings tunset = new frmTuningSettings();
                            tunset.Turbo = t5p.TurboType;
                            tunset.Injectors = t5p.InjectorType;
                            tunset.MapSensor = t5p.MapSensorType;
                            if (t5p.MapSensorType != MapSensorType.MapSensor25)
                            {
                                // set max boost etc
                                //tunset.PeakBoost = 1.75;
                                //tunset.BoostFuelcut = 2.05;
                            }
                            if (tunset.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                // write details to the file

                                if (t5p.MapSensorType != tunset.MapSensor)
                                {
                                    ConvertFileToThreeBarMapSensor(m_fileInformation, t5p.MapSensorType, tunset.MapSensor);
                                }
                                // check injector type
                                if (t5p.InjectorType != tunset.Injectors)
                                {
                                    int inj_konst_diff = DetermineDifferenceInInjectorConstant(t5p.InjectorType, tunset.Injectors);
                                    AddToInjectorConstant(filename, inj_konst_diff);
                                    // roughly set inj_konst
                                    // Stock = 21
                                    // Green giants = 20 (minus 1)
                                    // Siemens 630 = 16 (minus 5)
                                    // Siemens 875 = 13 (minus 8)
                                    // Siemens 1000 = 10 (minus 11)

                                    // set battery correction voltage maps

                                    SetInjectorBatteryCorrectionMap(m_TrionicFile, tunset.Injectors);
                                }
                                t5p.TurboType = tunset.Turbo;
                                t5p.InjectorType = tunset.Injectors;
                                t5p.MapSensorType = tunset.MapSensor;
                                // determine stage??
                                if (tunset.PeakBoost < 1.2) stage = 1;
                                else if (tunset.PeakBoost < 1.3) stage = 2;
                                else if (tunset.PeakBoost < 1.4) stage = 3;
                                else if (tunset.PeakBoost < 1.5) stage = 4;
                                else if (tunset.PeakBoost < 1.6) stage = 5;
                                else if (tunset.PeakBoost < 1.7) stage = 6;
                                else if (tunset.PeakBoost < 1.8) stage = 7;
                                else if (tunset.PeakBoost < 1.9) stage = 8;
                                else stage = 9;
                                if (tunset.MapSensor == MapSensorType.MapSensor30)
                                {
                                    // set correct values

                                    double conversion = CalculateConversionFactor(MapSensorType.MapSensor25, tunset.MapSensor);
                                    tunset.PeakBoost = (((((tunset.PeakBoost + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFirstGear = (((((tunset.BoostFirstGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostSecondGear = (((((tunset.BoostSecondGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFuelcut = (((((tunset.BoostFuelcut + 1) * 100) / conversion) / 100) - 1);
                                }
                                else if (tunset.MapSensor == MapSensorType.MapSensor35)
                                {
                                    // set correct values
                                    double conversion = CalculateConversionFactor(MapSensorType.MapSensor25, tunset.MapSensor);
                                    tunset.PeakBoost = (((((tunset.PeakBoost + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFirstGear = (((((tunset.BoostFirstGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostSecondGear = (((((tunset.BoostSecondGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFuelcut = (((((tunset.BoostFuelcut + 1) * 100) / conversion) / 100) - 1);
                                }
                                else if (tunset.MapSensor == MapSensorType.MapSensor40)
                                {
                                    // set correct values
                                    double conversion = CalculateConversionFactor(MapSensorType.MapSensor25, tunset.MapSensor);
                                    tunset.PeakBoost = (((((tunset.PeakBoost + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFirstGear = (((((tunset.BoostFirstGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostSecondGear = (((((tunset.BoostSecondGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFuelcut = (((((tunset.BoostFuelcut + 1) * 100) / conversion) / 100) - 1);
                                }
                                else if (tunset.MapSensor == MapSensorType.MapSensor50)
                                {
                                    // set correct values
                                    double conversion = CalculateConversionFactor(MapSensorType.MapSensor25, tunset.MapSensor);
                                    tunset.PeakBoost = (((((tunset.PeakBoost + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFirstGear = (((((tunset.BoostFirstGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostSecondGear = (((((tunset.BoostSecondGear + 1) * 100) / conversion) / 100) - 1);
                                    tunset.BoostFuelcut = (((((tunset.BoostFuelcut + 1) * 100) / conversion) / 100) - 1);
                                }
                                m_TrionicFile.SetTrionicOptions(t5p);
                                TuneToStage(filename, stage, tunset.PeakBoost, tunset.BoostFirstGear, tunset.BoostSecondGear, tunset.BoostFirstGear, tunset.BoostFuelcut, 90, /*isLpt*/ true, t5p.TurboType, t5p.InjectorType, t5p.MapSensorType);
                            }
                            else
                            {
                                retval = TuningResult.TuningCancelled;
                                return retval;
                            }

                        }
                        retval = TuningResult.TuningSuccess;
                        break;
                    /*                        case 1:
                                                // tune to stage 2
                                                TuneToStage(2, ecuinfo.Stage2boost, 0.72, 1.54, 0.62, 1.54, 90, isLpt);
                                                break;
                                            case 2:
                                                // tune to stage 3
                                                TuneToStage(3, ecuinfo.Stage3boost, 0.72, 1.54, 0.62, 1.54, 90, isLpt);
                                                break;*/
                    case 1:
                        // cancel
                        retval = TuningResult.TuningCancelled;
                        break;
                }
            }
            return retval;
        }

       /* public byte[] GetBatteryCorrectionMap(float[] correctionFactors)
        {
            byte[] retval = new byte[22];
            int bcount = 0;
            for (int i = 0; i < 11; i++)
            {
                float val = GetBatteryCorrection(i) / 0.004F;
                int ival = Convert.ToInt32(val);
                byte b1 = (byte)(ival / 256);
                byte b2 = (byte)(ival - (int)b1 * 256);
                retval[bcount++] = b1;
                retval[bcount++] = b2;
            }
            return retval;

        }*/

        private void SetInjectorBatteryCorrectionMap(IECUFile m_file, InjectorType injectorType)
        {
            byte[] batt_korr_tab = new byte[22]; // 11 values, first one is for 15 volt
            float tempvalue = 0;
            switch (injectorType)
            {
                case InjectorType.Stock:
                case InjectorType.Siemens875Dekas:
                case InjectorType.Siemens1000cc:
                    batt_korr_tab.SetValue((byte)0x00, 0); // 15 volt = 0.59
                    batt_korr_tab.SetValue((byte)0x93, 1);
                    batt_korr_tab.SetValue((byte)0x00, 2); // 14 volt = 0.77
                    batt_korr_tab.SetValue((byte)0xC0, 3);
                    batt_korr_tab.SetValue((byte)0x00, 4); // 13 volt = 0.78
                    batt_korr_tab.SetValue((byte)0xC3, 5);
                    batt_korr_tab.SetValue((byte)0x00, 6); // 12 volt = 0.94
                    batt_korr_tab.SetValue((byte)0xEB, 7);
                    batt_korr_tab.SetValue((byte)0x01, 8); // 11 volt = 1.28
                    batt_korr_tab.SetValue((byte)0x40, 9);
                    batt_korr_tab.SetValue((byte)0x01, 10); // 10 volt = 1.50
                    batt_korr_tab.SetValue((byte)0x77, 11);
                    batt_korr_tab.SetValue((byte)0x01, 12); // 9 volt = 1.85
                    batt_korr_tab.SetValue((byte)0xCE, 13);
                    batt_korr_tab.SetValue((byte)0x02, 14); // 8 volt = 2.32
                    batt_korr_tab.SetValue((byte)0x44, 15);
                    batt_korr_tab.SetValue((byte)0x03, 16); // 7 volt = 3.73
                    batt_korr_tab.SetValue((byte)0xA4, 17);
                    batt_korr_tab.SetValue((byte)0x03, 18); // 6 volt = 3.73
                    batt_korr_tab.SetValue((byte)0xA4, 19);
                    batt_korr_tab.SetValue((byte)0x03, 20); // 5 volt = 3.73
                    batt_korr_tab.SetValue((byte)0xA4, 21);
                    break;
                case InjectorType.GreenGiants:
                    batt_korr_tab.SetValue((byte)0x00, 0); // 15 volt = 0.894
                    batt_korr_tab.SetValue((byte)0xDF, 1);
                    batt_korr_tab.SetValue((byte)0x00, 2); // 14 volt = 1.003
                    batt_korr_tab.SetValue((byte)0xFA, 3);
                    batt_korr_tab.SetValue((byte)0x01, 4); // 13 volt = 1.15
                    batt_korr_tab.SetValue((byte)0x1F, 5);
                    batt_korr_tab.SetValue((byte)0x01, 6); // 12 volt = 1.308
                    batt_korr_tab.SetValue((byte)0x47, 7);
                    batt_korr_tab.SetValue((byte)0x01, 8); // 11 volt = 1.521
                    batt_korr_tab.SetValue((byte)0x7C, 9);
                    batt_korr_tab.SetValue((byte)0x01, 10); // 10 volt = 1.768
                    batt_korr_tab.SetValue((byte)0xBA, 11);
                    batt_korr_tab.SetValue((byte)0x02, 12); // 9 volt = 2.102
                    batt_korr_tab.SetValue((byte)0x0D, 13);
                    batt_korr_tab.SetValue((byte)0x02, 14); // 8 volt = 2.545
                    batt_korr_tab.SetValue((byte)0x7C, 15);
                    batt_korr_tab.SetValue((byte)0x03, 16); // 7 volt = 3.216
                    batt_korr_tab.SetValue((byte)0x24, 17);
                    batt_korr_tab.SetValue((byte)0x04, 18); // 6 volt = 4.142
                    batt_korr_tab.SetValue((byte)0x0B, 19);
                    batt_korr_tab.SetValue((byte)0x05, 20); // 5 volt = 5.45
                    batt_korr_tab.SetValue((byte)0x52, 21);
                    break;
                case InjectorType.Siemens630Dekas:
                    batt_korr_tab.SetValue((byte)0x00, 0); // 15 volt = 0.33
                    batt_korr_tab.SetValue((byte)0x52, 1);
                    batt_korr_tab.SetValue((byte)0x00, 2); // 14 volt = 0.433
                    batt_korr_tab.SetValue((byte)0x6C, 3);
                    batt_korr_tab.SetValue((byte)0x00, 4); // 13 volt = 0.548
                    batt_korr_tab.SetValue((byte)0x89, 5);
                    batt_korr_tab.SetValue((byte)0x00, 6); // 12 volt = 0.673
                    batt_korr_tab.SetValue((byte)0xA8, 7);
                    batt_korr_tab.SetValue((byte)0x00, 8); // 11 volt = 0.802
                    batt_korr_tab.SetValue((byte)0xC8, 9);
                    batt_korr_tab.SetValue((byte)0x00, 10); // 10 volt = 0.974
                    batt_korr_tab.SetValue((byte)0xF3, 11);
                    batt_korr_tab.SetValue((byte)0x01, 12); // 9 volt = 1.208
                    batt_korr_tab.SetValue((byte)0x2E, 13);
                    batt_korr_tab.SetValue((byte)0x01, 14); // 8 volt = 1.524
                    batt_korr_tab.SetValue((byte)0x7D, 15);
                    batt_korr_tab.SetValue((byte)0x01, 16); // 7 volt = 2.023
                    batt_korr_tab.SetValue((byte)0xF9, 17);
                    batt_korr_tab.SetValue((byte)0x02, 18); // 6 volt = 2.74
                    batt_korr_tab.SetValue((byte)0xAD, 19);
                    batt_korr_tab.SetValue((byte)0x03, 20); // 5 volt = 3.6
                    batt_korr_tab.SetValue((byte)0x84, 21);
                    break;

            }
            // write to batt_korr_tab
            m_file.WriteData(batt_korr_tab, (uint)m_file.GetFileInfo().GetSymbolAddressFlash("Batt_korr_tab!"));
        }

        private void SetInjectorBatteryCorrectionMapOld(InjectorType injectorType)
        {
            // set battery correction voltage maps
            /*
             * Siemens deka 875         Siemens Deka 630      stock
             * Batt_korr_table
                15v = 0.62              15v=0.17ms          0.59
                14v = 0.73              14v=0.28ms          0.77
                13v = 0.85              13v=0.38ms          0.78
                12v = 1.00              12v=0.50ms          0.94
                11v = 1.20              11v=0.64ms          1.28
                10v = 1.46              10v=0.83ms          1.50
            */
        }

        public int DetermineDifferenceInInjectorConstant(InjectorType injectorTypeFrom, InjectorType injectorTypeTo)
        {
            int retval = 0;
            // Stock = 21
            // Green giants = 20 (minus 1)
            // Siemens 630 = 16 (minus 5)
            // Siemens 875 = 13 (minus 8)
            // Siemens 1000 = 11 (minus 10)

            
            switch (injectorTypeFrom)
            {
                case InjectorType.Stock:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.GreenGiants:
                            retval = 1; // from stock to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 5; // from stock to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 8; // from stock to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 10; // from stock to 1000s
                            break;
                    }
                    break;
                case InjectorType.GreenGiants:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = -1; // from gg to stock ????
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 4; // from GG to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 7; // from GG to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 9; // from GG to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens630Dekas:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = -5; // from 630s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = -4; // from 630s to GG
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 3; // from 630s to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 5; // from 630s to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens875Dekas:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = -8; // from 875s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = -7; // from 875s to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = -3; // from 875s to 630s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 2; // from 875s to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens1000cc:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = -10; // from 1000s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = -9; // from 1000s to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = -5; // from 1000s to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = -2; // from 1000s to 875s
                            break;
                    }
                    break;
            }

            return retval;

        }

        public float DetermineDifferenceInInjectorConstantPercentage(InjectorType injectorTypeFrom, InjectorType injectorTypeTo)
        {
            float retval = 1;
            // Stock = 21
            // Green giants = 20 (minus 1)
            // Siemens 630 = 16 (minus 5)
            // Siemens 875 = 13 (minus 8)
            // Siemens 1000 = 11 (minus 10)


            switch (injectorTypeFrom)
            {
                case InjectorType.Stock:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.GreenGiants:
                            retval = 0.84F; // from stock to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 0.55F; // from stock to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 0.40F; // from stock to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 0.35F; // from stock to 1000s
                            break;
                    }
                    break;
                case InjectorType.GreenGiants:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = 1.2F; // from gg to stock ????
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 0.66F; // from GG to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 0.48F; // from GG to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 0.42F; // from GG to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens630Dekas:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = 1.82F; // from 630s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = 1.53F; // from 630s to GG
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 0.72F; // from 630s to 875s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 0.63F; // from 630s to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens875Dekas:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = 2.53F; // from 875s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = 2.12F; // from 875s to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 1.39F; // from 875s to 630s
                            break;
                        case InjectorType.Siemens1000cc:
                            retval = 0.875F; // from 875s to 1000s
                            break;
                    }
                    break;
                case InjectorType.Siemens1000cc:
                    switch (injectorTypeTo)
                    {
                        case InjectorType.Stock:
                            retval = 2.89F; // from 1000s to stock ????
                            break;
                        case InjectorType.GreenGiants:
                            retval = 2.42F; // from 1000s to GG
                            break;
                        case InjectorType.Siemens630Dekas:
                            retval = 1.59F; // from 1000s to 630s
                            break;
                        case InjectorType.Siemens875Dekas:
                            retval = 1.14F; // from 1000s to 875s
                            break;
                    }
                    break;
            }

            return retval;

        }

        private int findBoostAdpationAreaAutomatic(string filename)
        {
            int retval = -1;
            FileInfo _fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[14] {0x33, 0xFC, 0x01, 0x13, 0x00, 0x00, 0xFF, 0xFF, 0x33, 0xFC, 0x01, 0xC2, 0x00, 0x00};
                // fill specific ori automatic rpm values
                
                sequence[2] = (byte)((_ori_rpmLowAut/10) / 256);
                sequence[3] = (byte)((_ori_rpmLowAut / 10) - (256 * sequence[2]));
                sequence[10] = (byte)((_ori_rpmHighAut/10) / 256);
                sequence[11] = (byte)((_ori_rpmHighAut / 10) - (256 * sequence[10]));

                byte[] seq_mask = new byte[14] {1,    1,    1,    1,    1,    1,    0,    0,    1,    1,    1,    1,    1,    1};
                byte data;
                int i, max;
                i = 0;
                max = 0;

                while (a_fileStream.Position < _fi.Length)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || seq_mask[i] == 0)
                    {
                        i++;
                    }
                    else
                    {
                        if (i > max) max = i;
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = ((int)a_fileStream.Position - sequence.Length);
                }
                else
                {
                    retval = -1;
                }
            }
            return retval;
        }

        private int findBoostAdpationAreaManualFirst(string filename)
        {
            int retval = -1;
            FileInfo _fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[14] { 0x33, 0xFC, 0x01 ,0x13,0x00,0x00,0xFF,0xFF, 0x33,0xFC,0x01,0x90,0x00,0x00 };
                sequence[2] = (byte)((_ori_rpmLowManual/10) / 256);
                sequence[3] = (byte)((_ori_rpmLowManual/10) - (256 * sequence[2]));
                sequence[10] = (byte)((_ori_rpmHighManual/10) / 256);
                sequence[11] = (byte)((_ori_rpmHighManual/10) - (256 * sequence[10]));

                byte[] seq_mask = new byte[14] { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 };
                byte data;
                int i, max;
                i = 0;
                max = 0;

                while (a_fileStream.Position < _fi.Length)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || seq_mask[i] == 0)
                    {
                        i++;
                    }
                    else
                    {
                        if (i > max) max = i;
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = ((int)a_fileStream.Position - sequence.Length);
                }
                else
                {
                    retval = -1;
                }
            }
            return retval;
        }

        private int findBoostAdpationAreaManualSecond(string filename)
        {
            int retval = -1;
            FileInfo _fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[18] { 0x0C, 0x79,0x01,0x13,0x00,0x00,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0x0C,0x79,0x01,0x90,0x00,0x00 };
                sequence[2] = (byte)((_ori_rpmLowManual/10) / 256);
                sequence[3] = (byte)((_ori_rpmLowManual/10) - (256 * sequence[2]));
                sequence[14] = (byte)((_ori_rpmHighManual/10) / 256);
                sequence[15] = (byte)((_ori_rpmHighManual/10) - (256 * sequence[14]));

                byte[] seq_mask = new byte[18] { 1,    1,   1,   1,   1,   1,   0,    0,   0,   0,   0,   0,   1,   1,   1,   1,   1,   1 };
                byte data;
                int i, max;
                i = 0;
                max = 0;

                while (a_fileStream.Position < _fi.Length)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || seq_mask[i] == 0)
                    {
                        i++;
                    }
                    else
                    {
                        if (i > max) max = i;
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = ((int)a_fileStream.Position - sequence.Length);
                }
                else
                {
                    retval = -1;
                }
            }
            return retval;
        }

        private int _ori_rpmLowManual = 2750;

        public int Ori_rpmLowManual
        {
            get { return _ori_rpmLowManual; }
            set { _ori_rpmLowManual = value; }
        }
        private int _ori_rpmHighManual = 4000;

        public int Ori_rpmHighManual
        {
            get { return _ori_rpmHighManual; }
            set { _ori_rpmHighManual = value; }
        }
        private int _ori_rpmLowAut = 2750;

        public int Ori_rpmLowAut
        {
            get { return _ori_rpmLowAut; }
            set { _ori_rpmLowAut = value; }
        }
        private int _ori_rpmHighAut = 4250;

        public int Ori_rpmHighAut
        {
            get { return _ori_rpmHighAut; }
            set { _ori_rpmHighAut = value; }
        }
        private int _ori_boostError = 4;

        public int Ori_boostError
        {
            get { return _ori_boostError; }
            set { _ori_boostError = value; }
        }

        public bool SetBoostRegulationDivisor(int divisorToSet, int currentDivisor, IECUFileInformation trionicFileInformation)
        {
            bool retval = true;
            m_fileInformation = trionicFileInformation;
            int DivisorOffset = findBoostRegulationDivisorOffset(m_fileInformation.Filename, currentDivisor);
            if (DivisorOffset != -1)
            {
                Console.WriteLine("DivisorOffset: " + DivisorOffset.ToString("X8"));
                byte[] divisor_data = readdatafromfile(m_fileInformation.Filename, DivisorOffset, 7);
                divisor_data[5] = Convert.ToByte(divisorToSet); // set the divisor value
                savedatatobinary(DivisorOffset, 7, divisor_data, m_fileInformation.Filename);
                // also save to the bin file for later access
                // <GS-19052010> also update the max rp m range for which to check
                // this should be 28 bytes back from DivisorOffset
                int maxRpm = 2500 + (30 * (divisorToSet*10));
                Console.WriteLine("Max rpm to set would be: " + maxRpm.ToString());
                
                int currentRpm = Convert.ToInt32(readbytefromfile(m_fileInformation.Filename, DivisorOffset - 28));
                currentRpm *= 256;
                currentRpm += Convert.ToInt32(readbytefromfile(m_fileInformation.Filename, DivisorOffset - 27));
                currentRpm *= 10;
                Console.WriteLine("Current rpm limit: " + currentRpm.ToString());
                maxRpm /= 10;
                byte b1rpmmax = (byte)(maxRpm / 256);
                byte b2rpmmax = (byte)(maxRpm - (256 * b1rpmmax));
                byte[] bdata = new byte[2];
                bdata.SetValue(b1rpmmax, 0);
                bdata.SetValue(b2rpmmax, 1);
                savedatatobinary(DivisorOffset - 28, 2, bdata, m_fileInformation.Filename);


            }
            else
            {
                retval = false;
            }
            return retval;
        }

        private int findBoostRegulationDivisorOffset(string filename, int currentDivisor)
        {
            int retval = -1;
            FileInfo _fi = new FileInfo(filename);
            using (FileStream a_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] sequence = new byte[7] { 0xFF, 0xFF, 0xFF, 0x06 ,0x70, 0x0A, 0x4C };
                // fill specific value
                sequence[5] = Convert.ToByte(currentDivisor);

                byte[] seq_mask = new byte[7] { 1, 1, 1, 1, 1, 1, 1};
                byte data;
                int i, max;
                i = 0;
                max = 0;

                while (a_fileStream.Position < _fi.Length)
                {
                    data = (byte)a_fileStream.ReadByte();
                    if (data == sequence[i] || seq_mask[i] == 0)
                    {
                        i++;
                    }
                    else
                    {
                        if (i > max) max = i;
                        i = 0;
                    }
                    if (i == sequence.Length) break;
                }
                if (i == sequence.Length)
                {
                    retval = ((int)a_fileStream.Position - sequence.Length);
                }
                else
                {
                    retval = -1;
                }
            }
            return retval;
        }

        public bool SetBoostAdaptionParameters(int rpmLowManual, int rpmHighManual, int rpmLowAut, int rpmHighAut, int boostError, IECUFileInformation trionicFileInformation)
        {
            bool retval = true;
            m_fileInformation = trionicFileInformation;
            rpmLowManual /= 10;
            rpmHighManual /= 10;
            rpmLowAut /= 10;
            rpmHighAut /= 10;
            byte b1rpmlowaut = (byte)(rpmLowAut / 256);
            byte b2rpmlowaut = (byte)(rpmLowAut - (256 * b1rpmlowaut));
            byte b1rpmhighaut = (byte)(rpmHighAut / 256);
            byte b2rpmhighaut = (byte)(rpmHighAut - (256 * b1rpmhighaut));
            byte b1rpmlowman = (byte)(rpmLowManual / 256);
            byte b2rpmlowman = (byte)(rpmLowManual - (256 * b1rpmlowman));
            byte b1rpmhighman = (byte)(rpmHighManual / 256);
            byte b2rpmhighman = (byte)(rpmHighManual - (256 * b1rpmhighman));
            // search the bin file for the known sequence in the boost adaption routine
            //                              HH LL                   HH LL       
            //AUTOMATIC FIRST ENTRY   33 FC 01 13 00 00 XX XX 33 FC 01 C2 00 00 XX XX
            //MANUAL FIRST ENTRY:     33 FC 01 13 00 00 XX XX 33 FC 01 90 00 00 XX XX
            //                              HH LL                               HH LL       
            //MANUAL SECOND ENTRY     0C 79 01 13 00 00 XX XX XX XX XX XX 0C 79 01 90 00 00 XX XX
            int AutomaticFirstOffset = findBoostAdpationAreaAutomatic(m_fileInformation.Filename);
            if (AutomaticFirstOffset != -1)
            {
                Console.WriteLine("AutomaticFirstOffset: " + AutomaticFirstOffset.ToString("X8"));
                byte[] Aut_1_data = readdatafromfile(m_fileInformation.Filename, AutomaticFirstOffset, 14);
                Aut_1_data[2] = b1rpmlowaut;
                Aut_1_data[3] = b2rpmlowaut;
                Aut_1_data[10] = b1rpmhighaut;
                Aut_1_data[11] = b2rpmhighaut;
                savedatatobinary(AutomaticFirstOffset, 14, Aut_1_data, m_fileInformation.Filename);

                // also save to the bin file for later access
            }
            else
            {
                retval = false;
            }
            int ManualFirstOffset = findBoostAdpationAreaManualFirst(m_fileInformation.Filename);
            if (ManualFirstOffset != -1)
            {
                Console.WriteLine("ManualFirstOffset: " + ManualFirstOffset.ToString("X8"));
                byte[] Manual_1_data = readdatafromfile(m_fileInformation.Filename, ManualFirstOffset, 14);
                Manual_1_data[2] = b1rpmlowman;
                Manual_1_data[3] = b2rpmlowman;
                Manual_1_data[10] = b1rpmhighman;
                Manual_1_data[11] = b2rpmhighman;
                savedatatobinary(ManualFirstOffset, 14, Manual_1_data, m_fileInformation.Filename);
                // also save to the bin file for later access
            }
            else
            {
                retval = false;
            }

            int ManualSecondOffset = findBoostAdpationAreaManualSecond(m_fileInformation.Filename);
            if (ManualSecondOffset != -1)
            {
                // read data
                Console.WriteLine("ManualSecondOffset: " + ManualSecondOffset.ToString("X8"));
                byte[] Manual_2_data = readdatafromfile(m_fileInformation.Filename, ManualSecondOffset, 18);
                Manual_2_data[2] = b1rpmlowman;
                Manual_2_data[3] = b2rpmlowman;
                Manual_2_data[14] = b1rpmhighman;
                Manual_2_data[15] = b2rpmhighman;
                savedatatobinary(ManualSecondOffset, 18, Manual_2_data, m_fileInformation.Filename);
                // also save to the bin file for later access
            }
            else
            {
                retval = false;
            }
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
                    retval = sh.Flash_start_address - m_fileInformation.Filelength;
                    while (retval > m_fileInformation.Filelength) retval -= m_fileInformation.Filelength;
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

        private string readenginetype(string filename)
        {
            int length = 0;
            string value = string.Empty;
            FileInfo fi = new FileInfo(filename);
            int offset = ReadMarkerAddress(filename, (int)fi.Length, 0x04, out length, out value);
            return value;
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

        /// <summary>
        /// Read the tuned-to-stage marker in the binary. This is kept at address (length - 0x200)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private int ReadTunedToStageMarker(string filename)
        {
            int stage = 0;
            FileInfo fi = new FileInfo(filename);
            int address = (int)fi.Length -0x200;
            if (address > 0)
            {
                stage = (int)readbytefromfile(filename, address);
                if (stage == 0xFF) stage = 0;
            }
            return stage;
        }

        /// <summary>
        /// Write the threebar conversion marker to the binary. This is kept at address (length - 0x1FF)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        private void WriteThreeBarConversionMarker(string filename, MapSensorType sensorType)
        {
            int address = m_fileInformation.Filelength - 0x1FF;
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
        /// Write the threebar conversion marker to the binary. This is kept at address (length - 0x1FF)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        private void WriteThreeBarConversionMarker(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            int address = (int)fi.Length - 0x1FF;
            if (address > 0)
            {
                writebyteinfile(filename, address, 0x01);
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
            FileInfo fi = new FileInfo(filename);

            int address = (int)fi.Length - 0x1FF;
            if (address > 0)
            {
                stage = (int)readbytefromfile(filename, address);
                if (stage == 0xFF) stage = 0;
            }
            return stage;
        }

        /// <summary>
        /// Write the tuned-to-stage marker to the binary. This is kept at address (length - 0x200)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        private void WriteTunedToStageMarker(string filename, int stage)
        {
            FileInfo fi = new FileInfo(filename);

            int address = (int)fi.Length - 0x200;
            if (address > 0)
            {
                writebyteinfile(filename, address, (byte)stage);
            }
        }

        private byte readbytefromfile(string filename, int address)
        {
            byte retval = 0;
            FileStream fsi1 = File.OpenRead(filename);
            while (address > fsi1.Length) address -= (int)fsi1.Length;
            BinaryReader br1 = new BinaryReader(fsi1);
            fsi1.Position = address;
            retval = br1.ReadByte();
            fsi1.Flush();
            br1.Close();
            fsi1.Close();
            fsi1.Dispose();
            return retval;
        }

        private void writebyteinfile(string filename, int address, byte value)
        {
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

        #region Tuning functions



        private bool CheckBoostRegulationMapEmpty(string m_currentfile)
        {
            bool retval = true;
            
            byte[] reg_kon_mat = readdatafromfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat!"), GetSymbolLength( "Reg_kon_mat!"));
            for (int t = 0; t < reg_kon_mat.Length; t++)
            {
                if ((byte)reg_kon_mat.GetValue(t) != 0) retval = false;
            }
            return retval;
        }

        private bool CheckBoostRegulationAUTMapEmpty(string m_currentfile)
        {
            bool retval = true;
            byte[] reg_kon_mat = readdatafromfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat_a!"), GetSymbolLength( "Reg_kon_mat_a!"));
            for (int t = 0; t < reg_kon_mat.Length; t++)
            {
                if ((byte)reg_kon_mat.GetValue(t) != 0) retval = false;
            }
            return retval;
        }

        private void FillRegulationMapValue(string m_currentfile, int percentage)
        {
            int valuetofill = percentage; //* 10;
            int reg_kon_mat_length = GetSymbolLength( "Reg_kon_mat!"); // 2;
            if (reg_kon_mat_length != 0x80)
            {
                valuetofill *= 10;
                reg_kon_mat_length /= 2; // if not 128 byte length, its 16 bit
                for (int t = 0; t < reg_kon_mat_length; t++)
                {
                    byte b1 = (byte)(valuetofill / 256);
                    byte b2 = (byte)(valuetofill - (256 * b1));
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat!") + (t * 2), b1);
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat!") + (t * 2) + 1, b2);
                }
            }
            else
            {
                for (int t = 0; t < reg_kon_mat_length; t++)
                {
                    byte b1 = (byte)valuetofill;
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat!") + t, b1);
                }
            }
            m_resume.AddToResumeTable("Filled boost regulation map (manual) with " + percentage.ToString() + " percent");
        }

        private void FillRegulationAUTMapValue(string m_currentfile, int percentage)
        {
            int valuetofill = percentage; //* 10;
            int reg_kon_mat_length = GetSymbolLength( "Reg_kon_mat_a!"); // 2;
            if (reg_kon_mat_length != 0x80)
            {
                valuetofill *= 10;
                reg_kon_mat_length /= 2; // if not 128 byte length, its 16 bit
                for (int t = 0; t < reg_kon_mat_length; t++)
                {
                    byte b1 = (byte)(valuetofill / 256);
                    byte b2 = (byte)(valuetofill - (256 * b1));
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat_a!") + (t * 2), b1);
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat_a!") + (t * 2) + 1, b2);
                }
            }
            else
            {
                for (int t = 0; t < reg_kon_mat_length; t++)
                {
                    byte b1 = (byte)valuetofill;
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Reg_kon_mat_a!") + t, b1);
                }
            }
            m_resume.AddToResumeTable("Filled boost regulation map (automatic) with " + percentage.ToString() + " percent");
        }

        private void AddToInjectorConstant(string m_currentfile, int diff)
        {
            byte val = readbytefromfile(m_currentfile, GetSymbolAddress("Inj_konst!"));
            val -= Convert.ToByte(diff);
            writebyteinfile(m_currentfile, GetSymbolAddress("Inj_konst!"), val);

        }

        private void FillDefaultPIDControls(string m_currentfile)
        {
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!"), (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 1, (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 2, (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 3, (byte)255);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 4, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 5, (byte)90);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 6, (byte)120);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 7, (byte)165);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 8, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 9, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 10, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 11, (byte)110);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 12, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 13, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 14, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 15, (byte)100);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 16, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 17, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 18, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 19, (byte)50);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 20, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 21, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 22, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 23, (byte)30);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 24, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 25, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 26, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors!") + 27, (byte)20);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!"), (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 1, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 2, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 3, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 4, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 5, (byte)13);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 6, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 7, (byte)1);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 8, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 9, (byte)22);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 10, (byte)20);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 11, (byte)4);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 12, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 13, (byte)35);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 14, (byte)20);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 15, (byte)8);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 16, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 17, (byte)45);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 18, (byte)30);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 19, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 20, (byte)15);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 21, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 22, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 23, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 24, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 25, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 26, (byte)35);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors!") + 27, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!"), (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 1, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 2, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 3, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 4, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 5, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 6, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 7, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 8, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 9, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 10, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 11, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 12, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 13, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 14, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 15, (byte)150);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 16, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 17, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 18, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 19, (byte)255);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 20, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 21, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 22, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 23, (byte)175);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 24, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 25, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 26, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors!") + 27, (byte)150);
            m_resume.AddToResumeTable("Filled PID controls with default values for FPT (manual)");
        }

        private void FillDefaultPIDAUTControls(string m_currentfile)
        {
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!"), (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 1, (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 2, (byte)255);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 3, (byte)255);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 4, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 5, (byte)90);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 6, (byte)120);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 7, (byte)165);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 8, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 9, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 10, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 11, (byte)110);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 12, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 13, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 14, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 15, (byte)100);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 16, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 17, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 18, (byte)70);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 19, (byte)50);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 20, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 21, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 22, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 23, (byte)30);

            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 24, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 25, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 26, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "P_fors_a!") + 27, (byte)20);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!"), (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 1, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 2, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 3, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 4, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 5, (byte)13);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 6, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 7, (byte)1);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 8, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 9, (byte)22);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 10, (byte)20);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 11, (byte)4);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 12, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 13, (byte)35);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 14, (byte)20);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 15, (byte)8);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 16, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 17, (byte)45);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 18, (byte)30);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 19, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 20, (byte)15);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 21, (byte)60);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 22, (byte)40);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 23, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 24, (byte)10);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 25, (byte)50);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 26, (byte)35);
            writebyteinfile(m_currentfile, GetSymbolAddress( "I_fors_a!") + 27, (byte)13);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!"), (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 1, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 2, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 3, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 4, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 5, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 6, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 7, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 8, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 9, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 10, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 11, (byte)0);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 12, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 13, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 14, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 15, (byte)150);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 16, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 17, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 18, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 19, (byte)255);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 20, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 21, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 22, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 23, (byte)175);

            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 24, (byte)0);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 25, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 26, (byte)150);
            writebyteinfile(m_currentfile, GetSymbolAddress( "D_fors_a!") + 27, (byte)150);
            m_resume.AddToResumeTable("Filled PID controls with default values for FPT (automatic)");
        }

        private bool CheckPIDControlEmpty(string m_currentfile)
        {
            bool retval = true;
            byte[] reg_kon_mat = readdatafromfile(m_currentfile, GetSymbolAddress( "P_fors!"), GetSymbolLength( "P_fors!"));
            for (int t = 0; t < reg_kon_mat.Length; t++)
            {
                if ((byte)reg_kon_mat.GetValue(t) != 0) retval = false;
            }
            byte[] reg_kon_mat2 = readdatafromfile(m_currentfile, GetSymbolAddress( "I_fors!"), GetSymbolLength( "I_fors!"));
            for (int t = 0; t < reg_kon_mat2.Length; t++)
            {
                if ((byte)reg_kon_mat2.GetValue(t) != 0) retval = false;
            }
            byte[] reg_kon_mat3 = readdatafromfile(m_currentfile, GetSymbolAddress( "D_fors!"), GetSymbolLength( "D_fors!"));
            for (int t = 0; t < reg_kon_mat3.Length; t++)
            {
                if ((byte)reg_kon_mat3.GetValue(t) != 0) retval = false;
            }
            return retval;
        }

        private bool CheckPIDControlAUTEmpty(string m_currentfile)
        {
            bool retval = true;
            byte[] reg_kon_mat = readdatafromfile(m_currentfile, GetSymbolAddress( "P_fors_a!"), GetSymbolLength( "P_fors_a!"));
            for (int t = 0; t < reg_kon_mat.Length; t++)
            {
                if ((byte)reg_kon_mat.GetValue(t) != 0) retval = false;
            }
            byte[] reg_kon_mat2 = readdatafromfile(m_currentfile, GetSymbolAddress( "I_fors_a!"), GetSymbolLength( "I_fors_a!"));
            for (int t = 0; t < reg_kon_mat2.Length; t++)
            {
                if ((byte)reg_kon_mat2.GetValue(t) != 0) retval = false;
            }
            byte[] reg_kon_mat3 = readdatafromfile(m_currentfile, GetSymbolAddress( "D_fors_a!"), GetSymbolLength( "D_fors_a!"));
            for (int t = 0; t < reg_kon_mat3.Length; t++)
            {
                if ((byte)reg_kon_mat3.GetValue(t) != 0) retval = false;
            }
            return retval;
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

        private void SetRegKonMatFirstGearManual(string m_currentfile, int percentage)
        {
            if (GetSymbolAddress( "Reg_kon_fgm!") > 0)
            {
                byte[] bytes = readdatafromfile(m_currentfile, GetSymbolAddress( "Reg_kon_fgm!"), GetSymbolLength( "Reg_kon_fgm!"));
                if (bytes.Length == 1)
                {
                    bytes.SetValue((byte)percentage, 0);
                    savedatatobinary(GetSymbolAddress( "Reg_kon_fgm!"), 1, bytes, m_currentfile);
                    m_resume.AddToResumeTable("Set regulation control value for first gear manual to " + percentage.ToString());
                }
            }
        }
        private void SetRegKonMatSecondGearManual(string m_currentfile, int percentage)
        {
            if (GetSymbolAddress( "Reg_kon_sgm!") > 0)
            {
                byte[] bytes = readdatafromfile(m_currentfile, GetSymbolAddress( "Reg_kon_sgm!"), GetSymbolLength( "Reg_kon_sgm!"));
                if (bytes.Length == 1)
                {
                    bytes.SetValue((byte)percentage, 0);
                    savedatatobinary(GetSymbolAddress( "Reg_kon_sgm!"), 1, bytes, m_currentfile);
                    m_resume.AddToResumeTable("Set regulation control value for second gear manual to " + percentage.ToString());
                }
            }
        }
        private void SetRegKonMatFirstGearAutomatic(string m_currentfile, int percentage)
        {
            if (GetSymbolAddress( "Reg_kon_fga!") > 0)
            {
                byte[] bytes = readdatafromfile(m_currentfile, GetSymbolAddress( "Reg_kon_fga!"), GetSymbolLength( "Reg_kon_fga!"));
                if (bytes.Length == 1)
                {
                    bytes.SetValue((byte)percentage, 0);
                    savedatatobinary(GetSymbolAddress( "Reg_kon_fga!"), 1, bytes, m_currentfile);
                    m_resume.AddToResumeTable("Set regulation control value for first gear automatic to " + percentage.ToString());
                }
            }
        }


        private int[] GetXaxisValues(string filename, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int xaxisaddress = 0;
            int xaxislength = 0;
            bool issixteenbit = false;
            int multiplier = 1;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_1_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_1_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_2_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_2_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_3_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_3_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_5_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_5_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_6_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_6_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_7_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_7_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_8_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_8_x_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress( "Pwm_ind_trot!") + 32;
                issixteenbit = false;

                //                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                //                xaxislength = GetSymbolLength("Trans_x_st!");
                //                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                //xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                //xaxislength = GetSymbolLength("Trans_x_st!");
                xaxisaddress = GetSymbolAddress( "Overs_tab_xaxis!");
                xaxislength = GetSymbolLength("Overs_tab_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress( "Pwm_ind_trot!") + 32;
                issixteenbit = false;

                //xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                //xaxislength = GetSymbolLength("Trans_x_st!");
                //issixteenbit = false;
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Adapt_ref"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                xaxisaddress = GetSymbolAddress( "Dash_trot_axis!");
                xaxislength = GetSymbolLength("Dash_trot_axis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Purge_tab!"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                xaxisaddress = GetSymbolAddress( "Idle_st_last!");
                xaxislength = GetSymbolLength("Idle_st_last!");
                issixteenbit = false;
            }
            //retard_konst
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                xaxisaddress = GetSymbolAddress( "Trans_x_st!");
                xaxislength = GetSymbolLength("Trans_x_st!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                xaxisaddress = GetSymbolAddress( "Reg_last!");
                xaxislength = GetSymbolLength("Reg_last!");
                issixteenbit = false;
                //multiplier = 0.01;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                xaxisaddress = GetSymbolAddress( "Reg_last!");
                xaxislength = GetSymbolLength("Reg_last!");
                issixteenbit = false;
                //multiplier = 0.01;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_map_xaxis!");
                xaxislength = GetSymbolLength("Fuel_map_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                xaxisaddress = GetSymbolAddress( "Fuel_knock_xaxis!");
                xaxislength = GetSymbolLength("Fuel_knock_xaxis!");
                issixteenbit = false;
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                xaxisaddress = GetSymbolAddress( "Misfire_map_x_axis!");
                xaxislength = GetSymbolLength("Misfire_map_x_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                xaxisaddress = GetSymbolAddress( "Misfire_map_x_axis!");
                xaxislength = GetSymbolLength("Misfire_map_x_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                xaxisaddress = GetSymbolAddress( "Ign_map_0_x_axis!");
                xaxislength = GetSymbolLength("Ign_map_0_x_axis!");
                // MAP value
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                xaxisaddress = GetSymbolAddress( "Temp_reduce_x_st!");
                xaxislength = GetSymbolLength("Temp_reduce_x_st!");
                issixteenbit = false;
            }
            /*            else if (symbolname.StartsWith("Knock_ref_matrix!"))
                        {
                            xaxisaddress = GetSymbolAddress("Ign_map_0_x_axis!");
                            xaxislength = GetSymbolLength(curSymbols,"Ign_map_0_x_axis!");
                            issixteenbit = true;
                        }*/
            else if (symbolname.StartsWith("Detect_map!"))
            {
                xaxisaddress = GetSymbolAddress( "Detect_map_x_axis!");
                xaxislength = GetSymbolLength("Detect_map_x_axis!");
                issixteenbit = true;
            }
            else if ((symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!")) && GetSymbolLength(symbolname) == 0x80)
            {
                xaxislength = 8;
                xaxisaddress = GetSymbolAddress( "Pwm_ind_trot!") + 32;
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
                GetTableMatrixWitdhByName(filename, symbolname, out cols, out rows);
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

        private int[] GetYaxisValues(string filename, string symbolname)
        {
            int[] retval = new int[0];
            //retval.SetValue(0, 0);
            int yaxisaddress = 0;
            int yaxislength = 0;
            bool issixteenbit = false;
            int multiplier = 1;
            if (symbolname.StartsWith("Ign_map_0!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_1!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_1_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_1_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_2_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_2_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_3_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_3_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_0_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                // 16 bits
                //multiplier = 10; // RPM = 10 times
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_5_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_5_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_6_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_6_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_7_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_7_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Open_loop_adapt!") || symbolname.StartsWith("Open_loop!") || symbolname.StartsWith("Open_loop_knock!"))
            {
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Fload_tab!") || symbolname.StartsWith("Fload_throt_tab!"))
            {
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }


            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                yaxisaddress = GetSymbolAddress( "Ign_map_8_y_axis!");
                yaxislength = GetSymbolLength("Ign_map_8_y_axis!");
                // 16 bits
                issixteenbit = true;
            }
            /*else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                yaxislength = GetSymbolLength("Temp_steg!");
                yaxisaddress = GetSymbolAddress( "Temp_steg!");

                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;
            }*/

            else if (symbolname.StartsWith("Kyltemp_tab!"))
            {
                yaxislength = GetSymbolLength("Kyltemp_steg!");
                yaxisaddress = GetSymbolAddress( "Kyltemp_steg!");
            }
            else if (symbolname.StartsWith("Lufttemp_tab!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress( "Lufttemp_steg!");
            }

            /*else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                yaxislength = GetSymbolLength("Lufttemp_steg!");
                yaxisaddress = GetSymbolAddress( "Lufttemp_steg!");
                // 
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {

                    retval.SetValue(LookupAirTemperature((int)Lufttemp_steg.GetValue(i)), i);
                }
                return retval;
            }*/
            else if (symbolname.StartsWith("Derivata_br_tab_pos!") || symbolname.StartsWith("Derivata_br_tab_neg!"))
            {
                yaxislength = GetSymbolLength("Derivata_br_sp!");
                yaxisaddress = GetSymbolAddress( "Derivata_br_sp!");
            }
            else if (symbolname.StartsWith("I_last_rpm!") || symbolname.StartsWith("Last_reg_ac!"))
            {
                yaxislength = GetSymbolLength("Last_varv_st!");
                yaxisaddress = GetSymbolAddress( "Last_varv_st!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("I_last_temp!"))
            {
                yaxislength = GetSymbolLength("Last_temp_st!");
                yaxisaddress = GetSymbolAddress( "Last_temp_st!");
            }
            else if (symbolname.StartsWith("Iv_start_time_tab!"))
            {
                yaxislength = GetSymbolLength("I_kyl_st!");
                yaxisaddress = GetSymbolAddress( "I_kyl_st!");
            }

            /*else if (symbolname.StartsWith("Idle_ac_tab!"))
            {
                yaxislength = GetSymbolLength("Idle_ac_extra_sp!");
                yaxisaddress = GetSymbolAddress( "Idle_ac_extra_sp!");
            }*/

            else if (symbolname.StartsWith("Idle_start_extra!"))
            {
                yaxislength = GetSymbolLength("Idle_start_extra_sp!");
                yaxisaddress = GetSymbolAddress( "Idle_start_extra_sp!");
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
                yaxisaddress = GetSymbolAddress( "Hp_support_points!");
            }
            else if (symbolname.StartsWith("Lam_minlast!"))
            {
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Lamb_tid!") || symbolname.StartsWith("Lamb_idle!") || symbolname.StartsWith("Lamb_ej!"))
            {
                //TODO: get the corresponding temperatures for these maps.
                // this is done by interpolating the Lamb_kyl values with kyltemp_steg and fetching that index from
                // Kyltemp_tab
                yaxislength = GetSymbolLength("Lamb_kyl!");
                yaxisaddress = GetSymbolAddress( "Lamb_kyl!");
            }
            else if (symbolname.StartsWith("Overstid_tab!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("AC_wait_on!") || symbolname.StartsWith("AC_wait_off!"))
            {
                yaxislength = GetSymbolLength("I_luft_st!");
                yaxisaddress = GetSymbolAddress( "I_luft_st!");
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Idle_temp_off!") || symbolname.StartsWith("Idle_rpm_tab!") || symbolname.StartsWith("Start_tab!"))
            {
                yaxislength = GetSymbolLength("I_kyl_st!");
                yaxisaddress = GetSymbolAddress( "I_kyl_st!");
            }

            else if (symbolname.StartsWith("Overs_tab!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Insp_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Adapt_ggr"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Lambdamatris!") || symbolname.StartsWith("Lambdamatris_diag!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }

            else if (symbolname.StartsWith("Adapt_ref"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Max_regl_temp_"))
            {
                yaxislength = GetSymbolLength("Max_regl_sp!");
                yaxisaddress = GetSymbolAddress( "Max_regl_sp!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_wind_on_tab!") || symbolname.StartsWith("Knock_wind_off_tab!"))
            {
                yaxislength = GetSymbolLength("Knock_wind_rpm!");
                yaxisaddress = GetSymbolAddress( "Knock_wind_rpm!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_ref_tab!"))
            {
                yaxislength = GetSymbolLength("Knock_ref_rpm!");
                yaxisaddress = GetSymbolAddress( "Knock_ref_rpm!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Knock_average_tab!") || symbolname.StartsWith("Turbo_knock_tab!") || symbolname.StartsWith("Knock_press_tab!") || symbolname.StartsWith("Lknock_oref_tab!") || symbolname.StartsWith("Knock_lim_tab!"))
            {
                yaxislength = GetSymbolLength("Wait_count_tab!");
                yaxisaddress = GetSymbolAddress( "Wait_count_tab!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Dash_tab!"))
            {
                yaxislength = GetSymbolLength("Dash_rpm_axis!");
                yaxisaddress = GetSymbolAddress( "Dash_rpm_axis!");
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
                //yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                //issixteenbit = true;
                //multiplier = 10;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                issixteenbit = true;
                multiplier = 10;
            }
            else if (symbolname.StartsWith("Idle_fuel_korr!"))
            {
                //yaxislength = GetSymbolLength("Idle_rpm_tab!");
                //yaxisaddress = GetSymbolAddress( "Idle_rpm_tab!");
                yaxislength = GetSymbolLength("Idle_st_rpm!");
                yaxisaddress = GetSymbolAddress( "Idle_st_rpm!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress( "Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress( "Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress( "Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                yaxislength = GetSymbolLength("Trans_y_st!");
                yaxisaddress = GetSymbolAddress( "Trans_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                yaxislength = GetSymbolLength("Reg_varv!");
                yaxisaddress = GetSymbolAddress( "Reg_varv!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                yaxislength = GetSymbolLength("Reg_varv!");
                yaxisaddress = GetSymbolAddress( "Reg_varv!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab!"))
            {
                yaxislength = GetSymbolLength(/*"Fuel_map_yaxis!"*/"Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( /*"Fuel_map_yaxis!"*/"Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
                //Fuel_map_yaxis!
            }
            else if (symbolname.StartsWith("Regl_tryck_sgm!") || symbolname.StartsWith("Regl_tryck_fgm!") || symbolname.StartsWith("Regl_tryck_fgaut!"))
            {
                yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                multiplier = 10;
                issixteenbit = true;
                //Fuel_map_yaxis!
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                yaxislength = GetSymbolLength("Fuel_map_yaxis!");
                yaxisaddress = GetSymbolAddress( "Fuel_map_yaxis!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Mis1000_map!"))
            {
                yaxislength = GetSymbolLength("Misfire_map_y_axis!");
                yaxisaddress = GetSymbolAddress( "Misfire_map_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Misfire_map!"))
            {
                yaxislength = GetSymbolLength("Misfire_map_y_axis!");
                yaxisaddress = GetSymbolAddress( "Misfire_map_y_axis!");
                issixteenbit = true;
            }
            /*else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_") || symbolname.StartsWith("Tempkomp_konst!") || symbolname.StartsWith("Accel_temp!") || symbolname.StartsWith("Accel_temp2!") || symbolname.StartsWith("Retard_temp!") || symbolname.StartsWith("Throt_after_tab!") || symbolname.StartsWith("Throt_aft_dec_fak!"))
            {
                yaxislength = GetSymbolLength("Temp_steg!");
                yaxisaddress = GetSymbolAddress( "Temp_steg!");
                retval = new int[yaxislength];
                for (int i = 0; i < yaxislength; i++)
                {
                    retval.SetValue(LookupCoolantTemperature((int)Temp_steg.GetValue(i)), i);
                }
                return retval;

            }*/
            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                yaxislength = GetSymbolLength("Temp_reduce_y_st!");
                yaxisaddress = GetSymbolAddress( "Temp_reduce_y_st!");
                multiplier = 10;
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                yaxislength = GetSymbolLength("Ign_map_0_y_axis!");
                yaxisaddress = GetSymbolAddress( "Ign_map_0_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                yaxislength = GetSymbolLength("Detect_map_y_axis!");
                yaxisaddress = GetSymbolAddress( "Detect_map_y_axis!");
                issixteenbit = true;
            }
            else if (symbolname.StartsWith("Reg_kon_mat!") || symbolname.StartsWith("Reg_kon_mat_a!"))
            {
                if (GetSymbolLength(symbolname) == 0x80)
                {
                    yaxislength = GetSymbolLength("Pwm_ind_rpm!");
                    yaxisaddress = GetSymbolAddress( "Pwm_ind_rpm!");
                    multiplier = 10;
                    issixteenbit = true;
                }
                else
                {
                    retval = new int[31];
                    int v = 0;
                    for (int ti = 2500; ti <= 5500; ti += 100)
                    {
                        retval.SetValue(ti, v++);
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

        private void SetBoostRequestMap(string m_currentfile, int rowindex, int colindex, double value, double autoGearBoxPercentage, double capoff)
        {
            if(value > capoff) value = capoff;
            SetBoostRequestMap(m_currentfile, rowindex, colindex, value, autoGearBoxPercentage);
        }


        private void SetBoostRequestMap(string m_currentfile, int rowindex, int colindex, double value, double autoGearBoxPercentage)
        {
            //Tryck_mat!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Tryck_mat!"), GetSymbolLength( "Tryck_mat!"));
            int rows = 0;
            int cols = 0;
            GetTableMatrixWitdhByName(m_currentfile, "Tryck_mat!", out cols, out rows);
            int[] rpmvalues = GetYaxisValues(m_currentfile,  "Tryck_mat!");
            int[] mapvalues = GetXaxisValues(m_currentfile, "Tryck_mat!");
            int xt = colindex;
            int yt = rowindex;
            // gevonden
            //Console.WriteLine("tuning boost request map at x = " + xt.ToString() + " y = " + yt.ToString() + " perc = " + percentage.ToString());
            byte curr_byte = readbytefromfile(m_currentfile, GetSymbolAddress( "Tryck_mat!") + (yt * cols) + xt);
            //double newval = (double)curr_byte * percentage;
            double curr_real_value = (double)curr_byte;
            curr_real_value /= 100;
            curr_real_value -= 1;
            if(m_resume != null) m_resume.AddToResumeTable("Changed boostrequest (MAN) value at x = " + xt.ToString() + ", y = " + yt.ToString() + " from " + curr_real_value.ToString() + " bar to " + value.ToString() + " bar");

            double newval = (value + 1) * 100;
            if (newval > 254) newval = 254;
            // don't adjust DOWN
            
            //if (newval > 236) newval = 236; // block at 1.36 bar... 
            if (curr_byte < Convert.ToByte(newval))
            {
                curr_byte = Convert.ToByte(newval);
                writebyteinfile(m_currentfile, GetSymbolAddress("Tryck_mat!") + (yt * cols) + xt, curr_byte);
            }
            // do for auto transmission also
            SetBoostRequestMapAutoTrans(m_currentfile, rowindex, colindex, value * (autoGearBoxPercentage / 100));
        }

        private void SetBoostRequestMapAutoTrans(string m_currentfile, int rowindex, int colindex, double value)
        {
            //Tryck_mat_a!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Tryck_mat_a!"), GetSymbolLength( "Tryck_mat_a!"));
            int rows = 0;
            int cols = 0;
            GetTableMatrixWitdhByName(m_currentfile, "Tryck_mat_a!", out cols, out rows);
            int[] rpmvalues = GetYaxisValues(m_currentfile, "Tryck_mat_a!");
            int[] mapvalues = GetXaxisValues(m_currentfile, "Tryck_mat_a!");
            int xt = colindex;
            int yt = rowindex;
            // gevonden
            //Console.WriteLine("tuning boost request map at x = " + xt.ToString() + " y = " + yt.ToString() + " perc = " + percentage.ToString());
            byte curr_byte = readbytefromfile(m_currentfile, GetSymbolAddress( "Tryck_mat_a!") + (yt * cols) + xt);
            double curr_real_value = (double)curr_byte;
            curr_real_value /= 100;
            curr_real_value -= 1;
            //double newval = (double)curr_byte * percentage;
            if (m_resume != null) m_resume.AddToResumeTable("Changed boostrequest (AUT) value at x = " + xt.ToString() + ", y = " + yt.ToString() + " from " + curr_real_value.ToString() + " bar to " + value.ToString() + " bar");
            double newval = (value + 1) * 100;
            if (newval > 254) newval = 254;
            //if (newval > 236) newval = 236; // block at 1.36 bar... 
            if (curr_byte < Convert.ToByte(newval))
            {
                curr_byte = Convert.ToByte(newval);
                writebyteinfile(m_currentfile, GetSymbolAddress("Tryck_mat_a!") + (yt * cols) + xt, curr_byte);
            }
        }

        private int GetTableMatrixWitdhByName(string filename, string symbolname, out int columns, out int rows)
        {
            columns = 0;
            rows = 0;
            if (symbolname.StartsWith("Insp_mat!"))
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            if (symbolname.StartsWith("Adapt_ggr"))
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            if (symbolname.StartsWith("Adapt_ref"))
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Idle_der_tab!"))
            {
                columns = 8;
                rows = GetSymbolLength( "Idle_der_tab!") / 8;
                return 8;
            }
            else if (symbolname.StartsWith("Del_mat!"))
            {
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }
            else if (symbolname.StartsWith("Fuel_knock_mat!"))
            {
                columns = GetSymbolLength( "Fuel_knock_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 12;
            }
            else if (symbolname.StartsWith("Iv_min_tab_ac!"))
            {
                columns = 1;
                rows = GetSymbolLength( symbolname) / columns;
                return 2;
            }
            else if (symbolname.StartsWith("Iv_min_tab!"))
            {
                columns = 1;
                rows = GetSymbolLength( symbolname) / columns;
                return 1;
            }
            else if (symbolname.StartsWith("Lambdamatris_diag!"))
            {
                columns = 3;
                rows = GetSymbolLength( symbolname) / (columns * 2);
                return 3;
            }
            else if (symbolname.StartsWith("Lambdamatris!"))
            {
                //columns = 12; // was 3
                columns = 3;
                rows = GetSymbolLength( symbolname) / (columns * 2);
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
                columns = GetSymbolLength( "Idle_st_last!");
                //rows = GetSymbolLength( "Idle_rpm_tab!");//2
                rows = GetSymbolLength( "Idle_st_rpm!");//2 ??? which one is right???
                return 12;
            }
            else if (symbolname.StartsWith("Mis1000_map!") || symbolname.StartsWith("Mis200_map!") || symbolname.StartsWith("Detect_map!") || symbolname.StartsWith("Knock_ref_matrix!"))
            {
                columns = GetSymbolLength( "Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Misfire_map!")) // T5.2 table
            {
                columns = GetSymbolLength( "Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Ign_map_0!"))
            {
                columns = GetSymbolLength( "Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Knock_count_map"))
            {
                columns = GetSymbolLength( "Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_0_y_axis!");//2
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
                columns = GetSymbolLength( "Ign_map_1_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_1_y_axis!");//2
                return 0x1;
            }
            else if (symbolname.StartsWith("Ign_map_2!"))
            {
                columns = GetSymbolLength( "Ign_map_2_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_2_y_axis!");//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_3!"))
            {
                columns = GetSymbolLength( "Ign_map_3_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_3_y_axis!") / 2;
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_4!"))
            {
                columns = GetSymbolLength( "Ign_map_0_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_0_y_axis!");//2
                return 0x12;
            }
            else if (symbolname.StartsWith("Ign_map_5!"))
            {
                columns = GetSymbolLength( "Ign_map_5_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_5_y_axis!");//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_6!"))
            {
                columns = GetSymbolLength( "Ign_map_6_x_axis!") / 2;//2
                rows = GetSymbolLength( "Ign_map_6_y_axis!") / 2;//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_7!"))
            {
                columns = GetSymbolLength( "Ign_map_7_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_7_y_axis!") / 2;//2
                return 0x08;
            }
            else if (symbolname.StartsWith("Ign_map_8!"))
            {
                columns = GetSymbolLength( "Ign_map_8_x_axis!") / 2;
                rows = GetSymbolLength( "Ign_map_8_y_axis!") / 2;//2
                return 0x08;
            }
            
            else if (symbolname.StartsWith("AC_Control!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Tryck_vakt_tab"))
            {
                //Fuel_map_yaxis! langs de y as
                columns = 1;// op aanvraag hessu
                rows = GetSymbolLength( symbolname) / columns;
                return 0x10;
            }
            else if (symbolname.StartsWith("Regl_tryck"))
            {
                columns = 1;// op aanvraag hessu
                rows = GetSymbolLength( symbolname) / columns;
                return 0x10;
            }
            else if (symbolname.StartsWith("Reg_kon_mat"))
            {
                columns = 1;// op aanvraag hessu
                // sometimes this table is 0x80 bytes long, in that case it needs special conversion.
                if (GetSymbolLength( symbolname) == 0x80)
                {
                    //MY94 and earlier reg_kon_mat, has values for each gear and misses boost limiter for 1st and 2nd gear
                    columns = 8;
                }
                rows = GetSymbolLength( symbolname) / columns;
                return 0x10;
            }

            else if (symbolname.StartsWith("Tryck_mat_a!"))
            {
                columns = GetSymbolLength( "Trans_x_st!");
                //columns = GetSymbolLength( "Pwm_ind_trot!");
                //rows = GetSymbolLength( "Trans_y_st!");//2
                rows = GetSymbolLength( "Pwm_ind_rpm!") / 2;
                return 8;
            }
            else if (symbolname.StartsWith("Before_start!") || symbolname.StartsWith("Startvev_fak!") || symbolname.StartsWith("Start_dead_tab!") || symbolname.StartsWith("Ramp_fak!"))
            {
                columns = 1;
                rows = GetSymbolLength( symbolname);
                return 1;
            }
            else if (symbolname.StartsWith("Tryck_mat!") || symbolname.StartsWith("Pressure map"))
            {
                columns = GetSymbolLength( "Trans_x_st!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength( "Pwm_ind_rpm!") / 2;

                return 8;
            }
            else if (symbolname.StartsWith("Idle_tryck!"))
            {
                columns = GetSymbolLength( "Trans_x_st!");
                rows = GetSymbolLength( symbolname) / columns;
                return columns;
            }
            else if (symbolname.StartsWith("Overs_tab!"))
            {
                //columns = GetSymbolLength( "Trans_x_st!");
                columns = GetSymbolLength( "Overs_tab_xaxis!");
                rows = GetSymbolLength( "Pwm_ind_rpm!") / 2;

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
                columns = GetSymbolLength( "Dash_trot_axis!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength( "Dash_rpm_axis!") / 2;
                return 8;
                /*columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;*/
            }
            else if (symbolname.StartsWith("Retard_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Accel_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Eftersta_fak") || symbolname.StartsWith("Eft_dec_") || symbolname.StartsWith("Eft_fak_"))
            {
                //x = "MAP"; //?
                columns = 1;
                rows = GetSymbolLength( "Temp_steg!");
                return 1;
            }

            else if (symbolname.StartsWith("Temp_reduce_mat!") || symbolname.StartsWith("Temp_reduce_mat_2!"))
            {
                columns = GetSymbolLength( "Temp_reduce_x_st!");
                //rows = GetSymbolLength( "Trans_y_st!");
                rows = GetSymbolLength( "Temp_reduce_y_st!") / 2;
                return 8;
                /*columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;*/
            }
            else if (symbolname.StartsWith("Lacc_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Lret_konst!"))
            {
                columns = 8;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Adapt_ind_mat"))
            {
                columns = 0x10;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x10;
            }
            else if (symbolname == "Adapt_korr" || symbolname == "Adapt_korr!")
            {
                columns = 0x10;
                rows = GetSymbolLength( symbolname) / columns;
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
                columns = GetSymbolLength( "Fuel_map_xaxis!");
                rows = GetSymbolLength( "Fuel_map_yaxis!") / 2;
                return 16;
            }

            else if (symbolname.StartsWith("Knock_ref_matrix!"))
            {
                columns = 0x12;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Detect_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Mis200_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Misfire_map!")) // T5.2 table
            {
                columns = 0x12;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("Mis1000_map!"))
            {
                columns = 0x12;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x12;
            }
            else if (symbolname.StartsWith("P_fors!") || symbolname.StartsWith("I_fors!") || symbolname.StartsWith("D_fors!"))
            {
                columns = 0x4;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x4;
            }
            else if (symbolname.StartsWith("P_fors_a!") || symbolname.StartsWith("I_fors_a!") || symbolname.StartsWith("D_fors_a!"))
            {
                columns = 0x4;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x4;
            }
            else if (symbolname.StartsWith("Pwm_ind_trot!"))
            {
                columns = 0x08;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            else if (symbolname.StartsWith("Idle_p_tab!"))
            {
                columns = 0x08;
                rows = GetSymbolLength( symbolname) / columns;
                return 0x08;
            }
            columns = 1;
            rows = GetSymbolLength( symbolname) / columns;
            return 1;
        }

        private void IncreaseInjectionKnockMap(string m_currentfile, int columnindexfromend, int valuetoincrease)
        {
            // Fuel_knock_mat!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Fuel_knock_mat!"), GetSymbolLength("Fuel_knock_mat!"));
            int rows = 0;
            int cols = 0;
            GetTableMatrixWitdhByName(m_currentfile,  "Fuel_knock_mat!", out cols, out rows);
            columnindexfromend = cols - (columnindexfromend + 1);
            //columnindexfromend ++;
            //int[] rpmvalues = GetYaxisValues("Fuel_knock_mat!");
            //int[] mapvalues = GetXaxisValues("Fuel_knock_mat!");
            if (cols > columnindexfromend)
            {
                for (int t = 0; t < rows; t++)
                {
                    //columnindexfromend = cols - columnindexfromend;
                    byte currval = readbytefromfile(m_currentfile, GetSymbolAddress( "Fuel_knock_mat!") + (columnindexfromend) + (cols * t));
                    int inewvalue = (int)currval + (int)valuetoincrease;
                    if (inewvalue > 255) inewvalue = 255;
                    byte bnewvalue = (byte)inewvalue;
                    m_resume.AddToResumeTable("Changed fuel knock map value at x = " + columnindexfromend.ToString() + ", y = " + t.ToString() + " from " + currval.ToString() + " to " + bnewvalue.ToString());
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Fuel_knock_mat!") + (t * cols) + columnindexfromend, bnewvalue);
                }
            }
        }

        private void SetInjectionMap(string m_currentfile, int yt, int xt, int value)
        {
            // Insp_mat!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Insp_mat!"), GetSymbolLength( "Insp_mat!"));
            int rows = 0;
            int cols = 0;
            GetTableMatrixWitdhByName(m_currentfile, "Insp_mat!", out cols, out rows);
            //            int[] rpmvalues = GetYaxisValues("Insp_mat!");
            //            int[] mapvalues = GetXaxisValues("Insp_mat!");
            byte currval = readbytefromfile(m_currentfile, GetSymbolAddress( "Insp_mat!") + (yt * cols) + xt);

            // check percentage of increase
            double percentage = (double)value / (double)currval;
            if (percentage > 1.05) percentage = 1.05;
            if (percentage > 1)
            {
                value = (int)(percentage * (double)currval);
                if (value > 255) value = 255;
                writebyteinfile(m_currentfile, GetSymbolAddress( "Insp_mat!") + (yt * cols) + xt, (byte)value);
                m_resume.AddToResumeTable("Changed injection map value at x = " + xt.ToString() + ", y = " + yt.ToString() + " from " + currval.ToString() + " to " + value.ToString());
            }
        }

        private void SetMaxReglTempValues(string filename, byte value)
        {
            if (GetSymbolAddress("Max_regl_temp_1!") > 0)
            {
                byte[] mapdata = readdatafromfile(filename, GetSymbolAddress("Max_regl_temp_1!"), GetSymbolLength("Max_regl_temp_1!"));
                for (int tel = 0; tel < mapdata.Length; tel += 2)
                {
                    writebyteinfile(filename, GetSymbolAddress("Max_regl_temp_1!") + tel, 0);
                    writebyteinfile(filename, GetSymbolAddress("Max_regl_temp_1!") + tel + 1, (byte)value);
                }
            }
            if (GetSymbolAddress("Max_regl_temp_2!") > 0)
            {
                byte[] mapdata = readdatafromfile(filename, GetSymbolAddress("Max_regl_temp_2!"), GetSymbolLength("Max_regl_temp_2!"));
                for (int tel = 0; tel < mapdata.Length; tel += 2)
                {
                    writebyteinfile(filename, GetSymbolAddress("Max_regl_temp_2!") + tel, 0);
                    writebyteinfile(filename, GetSymbolAddress("Max_regl_temp_2!") + tel + 1, (byte)value);
                }
            }
        }

        private void SetBoostRequestMaps(TurboType turboType, InjectorType injectorType, MapSensorType mapSensorType, string m_currentfile, double maxBoostValue, double AutoGearBoxPercentage, bool isLpt)
        {
            switch (turboType)
            {
                case TurboType.Stock:
                    SetBoostRequestMap(m_currentfile, 0, 6, maxBoostValue * 0.85, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 6, maxBoostValue * 0.87, AutoGearBoxPercentage); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 6, maxBoostValue * 0.89, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 6, maxBoostValue * 0.90, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 6, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 6, maxBoostValue * 0.8, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 6, maxBoostValue * 0.7, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 6, maxBoostValue * 0.65, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 6, maxBoostValue * 0.6, AutoGearBoxPercentage); // high rpm

                    SetBoostRequestMap(m_currentfile, 0, 7, maxBoostValue * 0.85, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 7, maxBoostValue * 0.89, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 2, 7, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 7, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 7, maxBoostValue * 0.8, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 7, maxBoostValue * 0.7, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 7, maxBoostValue * 0.65, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 7, maxBoostValue * 0.6, AutoGearBoxPercentage);
                    
                    break;
                case TurboType.TD0415T: // hold longer than stock
                    SetBoostRequestMap(m_currentfile, 0, 6, maxBoostValue * 0.85, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 6, maxBoostValue * 0.87, AutoGearBoxPercentage); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 6, maxBoostValue * 0.89, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 6, maxBoostValue * 0.90, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 6, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 6, maxBoostValue * 0.80, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 6, maxBoostValue * 0.65, AutoGearBoxPercentage); // high rpm

                    SetBoostRequestMap(m_currentfile, 0, 7, maxBoostValue * 0.85, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 7, maxBoostValue * 0.89, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 2, 7, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 7, maxBoostValue , AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 7, maxBoostValue * 0.90, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 7, maxBoostValue * 0.80, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 7, maxBoostValue * 0.65, AutoGearBoxPercentage);
                    
                    break;
                case TurboType.TD0419T: // hold longer than stock but spool later
                case TurboType.GT28BB: // hold longer than stock but spool later
                case TurboType.GT28RS: // hold longer than stock but spool later
                    SetBoostRequestMap(m_currentfile, 0, 6, maxBoostValue * 0.60, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 6, maxBoostValue * 0.60, AutoGearBoxPercentage); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 6, maxBoostValue * 0.60, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 6, maxBoostValue * 0.65, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 6, maxBoostValue * 0.80, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 6, maxBoostValue * 0.9, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 6, maxBoostValue * 0.85, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 6, maxBoostValue * 0.70, AutoGearBoxPercentage); // high rpm

                    SetBoostRequestMap(m_currentfile, 0, 7, maxBoostValue * 0.60, AutoGearBoxPercentage); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 7, maxBoostValue * 0.60, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 2, 7, maxBoostValue * 0.60, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 3, 7, maxBoostValue * 0.65, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 4, 7, maxBoostValue * 0.80, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 5, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 6, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 7, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 8, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 9, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 7, maxBoostValue * 0.90, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 7, maxBoostValue * 0.85, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 7, maxBoostValue * 0.70, AutoGearBoxPercentage);
                    break;
                case TurboType.GT3071R: // hold longer than stock
                case TurboType.HX35w: // hold longer than stock
                    SetBoostRequestMap(m_currentfile, 0, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 3, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 4, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 5, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 6, 6, maxBoostValue * 0.65, AutoGearBoxPercentage, 0.8);
                    SetBoostRequestMap(m_currentfile, 7, 6, maxBoostValue * 0.75, AutoGearBoxPercentage, 1.0);
                    SetBoostRequestMap(m_currentfile, 8, 6, maxBoostValue * 0.95, AutoGearBoxPercentage, 1.2);
                    SetBoostRequestMap(m_currentfile, 9, 6, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 6, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 6, maxBoostValue * 0.80, AutoGearBoxPercentage); // high rpm

                    SetBoostRequestMap(m_currentfile, 0, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 3, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 4, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 5, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 6, 7, maxBoostValue * 0.65, AutoGearBoxPercentage, 0.8);
                    SetBoostRequestMap(m_currentfile, 7, 7, maxBoostValue * 0.75, AutoGearBoxPercentage, 1.0);
                    SetBoostRequestMap(m_currentfile, 8, 7, maxBoostValue * 0.95, AutoGearBoxPercentage, 1.2);
                    SetBoostRequestMap(m_currentfile, 9, 7, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 7, maxBoostValue * 0.95, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 7, maxBoostValue * 0.80, AutoGearBoxPercentage);
                    break;
                case TurboType.HX40w: // hold longer than stock
                case TurboType.S400SX371:
                    SetBoostRequestMap(m_currentfile, 0, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 3, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 4, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 5, 6, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 6, 6, maxBoostValue * 0.65, AutoGearBoxPercentage, 0.8);
                    SetBoostRequestMap(m_currentfile, 7, 6, maxBoostValue * 0.75, AutoGearBoxPercentage, 1.0);
                    SetBoostRequestMap(m_currentfile, 8, 6, maxBoostValue * 0.95, AutoGearBoxPercentage, 1.2);
                    SetBoostRequestMap(m_currentfile, 9, 6, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 6, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 6, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 6, maxBoostValue * 0.93, AutoGearBoxPercentage); // high rpm

                    SetBoostRequestMap(m_currentfile, 0, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // % of max boost
                    SetBoostRequestMap(m_currentfile, 1, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7); // low rpm
                    SetBoostRequestMap(m_currentfile, 2, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 3, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 4, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 5, 7, maxBoostValue * 0.60, AutoGearBoxPercentage, 0.7);
                    SetBoostRequestMap(m_currentfile, 6, 7, maxBoostValue * 0.65, AutoGearBoxPercentage, 0.8);
                    SetBoostRequestMap(m_currentfile, 7, 7, maxBoostValue * 0.75, AutoGearBoxPercentage, 1.0);
                    SetBoostRequestMap(m_currentfile, 8, 7, maxBoostValue * 0.95, AutoGearBoxPercentage, 1.2);
                    SetBoostRequestMap(m_currentfile, 9, 7, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 10, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 11, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 12, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 13, 7, maxBoostValue, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 14, 7, maxBoostValue * 0.98, AutoGearBoxPercentage);
                    SetBoostRequestMap(m_currentfile, 15, 7, maxBoostValue * 0.93, AutoGearBoxPercentage);
                    break;
            }
            if (isLpt)
            {
                // more columns need adjusting
                SetBoostRequestMap(m_currentfile, 0, 5, maxBoostValue * 0.55, AutoGearBoxPercentage); // % of max boost
                SetBoostRequestMap(m_currentfile, 1, 5, maxBoostValue * 0.55, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 2, 5, maxBoostValue * 0.55, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 3, 5, maxBoostValue * 0.57, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 4, 5, maxBoostValue * 0.58, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 5, 5, maxBoostValue * 0.58, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 6, 5, maxBoostValue * 0.58, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 7, 5, maxBoostValue * 0.57, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 8, 5, maxBoostValue * 0.57, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 9, 5, maxBoostValue * 0.56, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 10, 5, maxBoostValue * 0.53, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 11, 5, maxBoostValue * 0.50, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 12, 5, maxBoostValue * 0.45, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 13, 5, maxBoostValue * 0.40, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 14, 5, maxBoostValue * 0.40, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 15, 5, maxBoostValue * 0.35, AutoGearBoxPercentage);

                SetBoostRequestMap(m_currentfile, 0, 4, maxBoostValue * 0.35, AutoGearBoxPercentage); // 75% of max boost
                SetBoostRequestMap(m_currentfile, 1, 4, maxBoostValue * 0.35, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 2, 4, maxBoostValue * 0.35, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 3, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 4, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 5, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 6, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 7, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 8, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 9, 4, maxBoostValue * 0.30, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 10, 4, maxBoostValue * 0.25, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 11, 4, maxBoostValue * 0.2, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 12, 4, maxBoostValue * 0.18, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 13, 4, maxBoostValue * 0.15, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 14, 4, maxBoostValue * 0.15, AutoGearBoxPercentage);
                SetBoostRequestMap(m_currentfile, 15, 4, maxBoostValue * 0.1, AutoGearBoxPercentage);
            }
        }


        private void TuneToStage(string m_currentfile, int stage, double maxBoostValue, double maxBoostFirstGear, double maxBoostSecondGear, double maxBoostFirstGearAUT, double fuelCutLevel, double AutoGearBoxPercentage, bool isLpt, TurboType turboType, InjectorType injectorType, MapSensorType mapSensorType)
        {
            m_resume = new Trionic5Resume();
            m_resume.AddToResumeTable("Tuning your binary to stage: " + stage.ToString());
            // get the software ID from the bainery
            string enginetp = readenginetype(m_currentfile);
            string partnumber = readpartnumber(m_currentfile);
            // look up parameters for this sw id
            PartNumberConverter pnc = new PartNumberConverter();
            ECUInformation ecuinfo = pnc.GetECUInfo(partnumber, enginetp);
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin", true);
            m_resume.AddToResumeTable("Backup file created (" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin" + ")");


            switch (stage)
            {
                case 1:
                    SetRegKonMatFirstGearManual(m_currentfile, 30);
                    SetRegKonMatSecondGearManual(m_currentfile, 45);
                    SetRegKonMatFirstGearAutomatic(m_currentfile, 30);
                    break;
                case 2:
                    SetRegKonMatFirstGearManual(m_currentfile, 45);
                    SetRegKonMatSecondGearManual(m_currentfile, 45);
                    SetRegKonMatFirstGearAutomatic(m_currentfile, 45);
                    break;
                case 3:
                default:
                    SetRegKonMatFirstGearManual(m_currentfile, 45);
                    SetRegKonMatSecondGearManual(m_currentfile, 45);
                    SetRegKonMatFirstGearAutomatic(m_currentfile, 45);
                    break;
            }

            if (CheckBoostRegulationMapEmpty(m_currentfile))
            {
                // empty reg_kon_mat
                switch (stage)
                {
                    case 1:
                        FillRegulationMapValue(m_currentfile, 45);
                        break;
                    case 2:
                        FillRegulationMapValue(m_currentfile, 45);
                        break;
                    case 3:
                    default:
                        FillRegulationMapValue(m_currentfile, 45);
                        break;
                }
            }
            if (CheckBoostRegulationAUTMapEmpty(m_currentfile))
            {
                switch (stage)
                {
                    case 1:
                        FillRegulationAUTMapValue(m_currentfile, 45);
                        break;
                    case 2:
                        FillRegulationAUTMapValue(m_currentfile, 45);
                        break;
                    case 3:
                    default:
                        FillRegulationAUTMapValue(m_currentfile, 45);
                        break;
                }
            }

            if (CheckPIDControlEmpty(m_currentfile))
            {
                FillDefaultPIDControls(m_currentfile);
            }
            if (CheckPIDControlAUTEmpty(m_currentfile))
            {
                FillDefaultPIDAUTControls(m_currentfile);
            }

            //depending on turbotype!!!

            SetBoostRequestMaps(turboType, injectorType, mapSensorType, m_currentfile, maxBoostValue, AutoGearBoxPercentage, isLpt);

            if (/*!isLpt*/true) // don't if T5.2&& m_currentfile_size > 0x20000
            {
                // should be percentages
               /* SetInjectionMap(m_currentfile,15, 15, 255);
                SetInjectionMap(m_currentfile,14, 15, 253);
                SetInjectionMap(m_currentfile,13, 15, 253);
                SetInjectionMap(m_currentfile,12, 15, 249);
                SetInjectionMap(m_currentfile,11, 15, 248);
                SetInjectionMap(m_currentfile,10, 15, 245);
                SetInjectionMap(m_currentfile,9, 15, 236);

                SetInjectionMap(m_currentfile,15, 14, 255);
                SetInjectionMap(m_currentfile,14, 14, 253);
                SetInjectionMap(m_currentfile,13, 14, 253);
                SetInjectionMap(m_currentfile,12, 14, 235);
                SetInjectionMap(m_currentfile,11, 14, 234);
                SetInjectionMap(m_currentfile,10, 14, 226);
                SetInjectionMap(m_currentfile,9, 14, 225);

                SetInjectionMap(m_currentfile,15, 13, 248);
                SetInjectionMap(m_currentfile,14, 13, 245);
                SetInjectionMap(m_currentfile,13, 13, 245);
                SetInjectionMap(m_currentfile,12, 13, 224);
                SetInjectionMap(m_currentfile,11, 13, 217);
                SetInjectionMap(m_currentfile,10, 13, 205);
                SetInjectionMap(m_currentfile,9, 13, 189);

                SetInjectionMap(m_currentfile,15, 12, 219);
                SetInjectionMap(m_currentfile,14, 12, 215);
                SetInjectionMap(m_currentfile,13, 12, 213);
                SetInjectionMap(m_currentfile,12, 12, 206);
                SetInjectionMap(m_currentfile,11, 12, 205);
                SetInjectionMap(m_currentfile,10, 12, 198);
                SetInjectionMap(m_currentfile,9, 12, 176);

                SetInjectionMap(m_currentfile,15, 11, 198);
                SetInjectionMap(m_currentfile,14, 11, 192);
                SetInjectionMap(m_currentfile,13, 11, 191);
                SetInjectionMap(m_currentfile,12, 11, 190);
                SetInjectionMap(m_currentfile,11, 11, 190);
                SetInjectionMap(m_currentfile,10, 11, 183);
                SetInjectionMap(m_currentfile,9, 11, 163);*/
            }


            IncreaseInjectionKnockMap(m_currentfile, 0, 4);
            IncreaseInjectionKnockMap(m_currentfile, 1, 4);
            IncreaseInjectionKnockMap(m_currentfile, 2, 4);

            //SetIgnitionMap(m_currentfile, 15, 17, 1.5);
            //SetIgnitionMap(m_currentfile, 14, 17, 1.0);
            //SetIgnitionMap(m_currentfile, 13, 17, 0.5);
            //byte fuelcut = (byte)((fuelCutLevel + 1) * 100);
            SetBoostLimitMap(m_currentfile, 254 /* fuelcut */);

            //m_resume.m_resume.AddToResumeTable("Updated fuelcut map to: " + fuelCutLevel.ToString() + " bar");
            byte fglimit = (byte)((maxBoostFirstGear + 1) * 100);
            SetFirstGearLimiter(m_currentfile, fglimit);
            m_resume.AddToResumeTable("Updated first gear limiter (MAN) to: " + maxBoostFirstGear.ToString() + " bar");
            byte fgalimit = (byte)((maxBoostFirstGearAUT + 1) * 100);
            SetFirstGearLimiterAutoTrans(m_currentfile, fgalimit);
            m_resume.AddToResumeTable("Updated first gear limiter (AUT) to: " + maxBoostFirstGearAUT.ToString() + " bar");
            byte sglimit = (byte)((maxBoostSecondGear + 1) * 100);
            SetSecondGearLimiter(m_currentfile, sglimit);
            m_resume.AddToResumeTable("Updated second gear limiter (MAN) to: " + maxBoostSecondGear.ToString() + " bar");
            // <Guido> add Max_regl_temp1 Max_regl_temp2
            SetMaxReglTempValues(m_currentfile, 250);

            try
            {
                Trionic5Anomalies anomalies = new Trionic5Anomalies();
                anomalies.CheckBinForAnomalies(m_currentfile, m_resume, false, true, m_fileInformation );
            }
            catch (Exception E)
            {
                Console.WriteLine("CheckBinForAnomalies: " + E.Message);
            }

            // mark this particular file as tuned to stage X, to prevent running the wizard on this file again!
            //enginetp = enginetp.Substring(0, enginetp.Length - 4);
            //enginetp += "T5S" + stage.ToString();
            //writeenginetype(enginetp);
            WriteTunedToStageMarker(m_currentfile, stage);
            m_resume.AddToResumeTable("Updated binary description with tuned stage");
            Trionic5File file = new Trionic5File();
            file.LibraryPath = Application.StartupPath + "\\Binaries";
            file.SetAutoUpdateChecksum(m_autoUpdateChecksum);
            file.UpdateChecksum(m_currentfile);
        }

        private void SetBoostLimitMap(string m_currentfile, byte value)
        {
            //Tryck_vakt_tab!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress("Tryck_vakt_tab!"), GetSymbolLength("Tryck_vakt_tab!"));
            for (int tel = 0; tel < mapdata.Length; tel++)
            {
                writebyteinfile(m_currentfile, GetSymbolAddress("Tryck_vakt_tab!") + tel, (byte)value);
            }
        }

        private void SetIgnitionMap(string m_currentfile, int yt, int xt, double degreestoadd)
        {
            //Ign_map_0!
            byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Ign_map_0!"), GetSymbolLength( "Ign_map_0!"));
            int rows = 0;
            int cols = 0;
            GetTableMatrixWitdhByName(m_currentfile,  "Ign_map_0!", out cols, out rows);
            //int[] rpmvalues = GetYaxisValues("Ign_map_0!");
            //int[] mapvalues = GetXaxisValues("Ign_map_0!");
            //Console.WriteLine("tuning Ign_map_0 at x = " + xt.ToString() + " y = " + yt.ToString() + " val = " + degreestoadd.ToString());

            double curr_value = readbytefromfile(m_currentfile, GetSymbolAddress( "Ign_map_0!") + (yt * cols * 2) + (xt * 2)) * 256;
            curr_value += readbytefromfile(m_currentfile, GetSymbolAddress( "Ign_map_0!") + (yt * cols * 2) + (xt * 2) + 1);
            //Console.WriteLine("Current value was: :" + curr_value.ToString());
            double curr_real_value = curr_value / 10;
            m_resume.AddToResumeTable("Updated ignition map value at x = " + xt.ToString() + ", y = " + yt.ToString() + " from " + curr_real_value.ToString() + " degrees with " + degreestoadd.ToString() + " degrees");
            curr_value += degreestoadd * 10;
            //Console.WriteLine("New value would be: :" + curr_value.ToString());
            byte b1 = (byte)(curr_value / 256);
            byte b2 = (byte)(curr_value - (double)(b1 * 256));
            //Console.WriteLine("byte 1 would be : " + b1.ToString() + " byte 2 would be : " + b2.ToString());
            writebyteinfile(m_currentfile, GetSymbolAddress( "Ign_map_0!") + (yt * cols * 2) + (xt * 2), b1);
            writebyteinfile(m_currentfile, GetSymbolAddress( "Ign_map_0!") + (yt * cols * 2) + (xt * 2) + 1, b2);
        }

        private void SetFirstGearLimiter(string m_currentfile, byte value)
        {
            //Regl_tryck_fgm!
            if (GetSymbolAddress( "Regl_tryck_fgm!") > 0) // fail safe for T5.2
            {
                byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Regl_tryck_fgm!"), GetSymbolLength( "Regl_tryck_fgm!"));
                for (int tel = 0; tel < mapdata.Length; tel++)
                {
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Regl_tryck_fgm!") + tel, (byte)value);
                }
            }
        }

        private void SetSecondGearLimiter(string m_currentfile, byte value)
        {
            //Regl_tryck_sgm!
            if (GetSymbolAddress( "Regl_tryck_sgm!") > 0) // fail safe for T5.2
            {

                byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Regl_tryck_sgm!"), GetSymbolLength( "Regl_tryck_sgm!"));
                for (int tel = 0; tel < mapdata.Length; tel++)
                {
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Regl_tryck_sgm!") + tel, (byte)value);
                }
            }

        }

        private void SetFirstGearLimiterAutoTrans(string m_currentfile, byte value)
        {
            //Regl_tryck_fgaut!
            if (GetSymbolAddress( "Regl_tryck_fgaut!") > 0) // fail safe for T5.2
            {
                byte[] mapdata = readdatafromfile(m_currentfile, GetSymbolAddress( "Regl_tryck_fgaut!"), GetSymbolLength( "Regl_tryck_fgaut!"));
                for (int tel = 0; tel < mapdata.Length; tel++)
                {
                    writebyteinfile(m_currentfile, GetSymbolAddress( "Regl_tryck_fgaut!") + tel, (byte)value);
                }
            }
        }
        #endregion

        #region ThreeBarConversion
        private void AlterTableForThreeBarSensor(string symbolname, double factor)
        {
            m_resume.AddToResumeTable("Altering table for different mapsensor: " + symbolname);
            if (GetSymbolAddress(symbolname) > 0)
            {
                
                byte[] symboldata = readdatafromfile(m_fileInformation.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));

                if (m_fileInformation.isSixteenBitTable(symbolname))
                {
                    //                AddToResumeTable("Sixteenbit table: " + symbolname);
                    for (int t = 0; t < symboldata.Length; t += 2)
                    {
                        int b = (int)(byte)symboldata.GetValue(t) * 256;
                        b += (int)(byte)symboldata.GetValue(t + 1);
                        double d = (double)b;
                        d /= /*1.2*/ factor;
                        byte b1 = (byte)(d / 256);
                        byte b2 = (byte)(d - (double)(b1 * 256));

                        symboldata.SetValue(b1, t);
                        symboldata.SetValue(b2, t + 1);
                    }

                }
                else
                {
                    for (int t = 0; t < symboldata.Length; t++)
                    {
                        byte b = (byte)symboldata.GetValue(t);
                        double d = (double)b;
                        d /= /*1.2*/ factor;
                        b = (byte)d;
                        symboldata.SetValue(b, t);
                    }
                }

                savedatatobinary(GetSymbolAddress(symbolname), GetSymbolLength(symbolname), symboldata, m_fileInformation.Filename);
            }
            else
            {
                m_resume.AddToResumeTable("Couldn't find symbol: " + symbolname);
            }
        }

        private void AlterXAxisForThreeBarSensor(string symbolname, bool issixteenbit, double factor)
        {
            m_resume.AddToResumeTable("Altering x axis for three bar sensor: " + symbolname);
            if (GetSymbolAddress(symbolname) > 0)
            {
                byte[] symboldata = readdatafromfile(m_fileInformation.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));
                if (issixteenbit)
                {
                    //AddToResumeTable("Sixteenbit axis: " + symbolname);
                    for (int t = 0; t < symboldata.Length; t += 2)
                    {
                        int b = (int)(byte)symboldata.GetValue(t) * 256;
                        b += (int)(byte)symboldata.GetValue(t + 1);
                        double d = (double)b;
                        d /= /*1.2*/ factor;
                        byte b1 = (byte)(d / 256);
                        byte b2 = (byte)(d - (double)(b1 * 256));

                        symboldata.SetValue(b1, t);
                        symboldata.SetValue(b2, t + 1);
                    }

                }
                else
                {
                    for (int t = 0; t < symboldata.Length; t++)
                    {
                        byte b = (byte)symboldata.GetValue(t);
                        double d = (double)b;
                        d /= /*1.2*/ factor;
                        b = (byte)d;
                        symboldata.SetValue(b, t);
                    }
                }
                //if (!m_appSettings.PreventThreeBarRescaling)
                {
                    if (symbolname == "Fuel_map_xaxis!")
                    {
                        // fuel_map_xaxis needs higher values in last 4 columns!
                        // values are : 152, 180, 195, 215
                        if (symboldata.Length == 16)
                        {
                            //symboldata.SetValue((byte)140, 11);
                            //symboldata.SetValue((byte)161, 12);
                            symboldata.SetValue((byte)161, 11);
                            symboldata.SetValue((byte)173, 12);
                            symboldata.SetValue((byte)186, 13);
                            symboldata.SetValue((byte)210, 14);
                            symboldata.SetValue((byte)235, 15); // upto ~1.8 bar
                            /*                        symboldata.SetValue((byte)152, 12);
                                                    symboldata.SetValue((byte)180, 13);
                                                    symboldata.SetValue((byte)195, 14);
                                                    symboldata.SetValue((byte)215, 15);*/
                        }

                    }
                    else if (symbolname == "Fuel_knock_xaxis!")
                    {
                        // fuel_map_xaxis needs higher values in last 4 columns!
                        // values are : 152, 180, 195, 215
                        if (symboldata.Length == 12)
                        {
                            //symboldata.SetValue((byte)140, 7);
                            //symboldata.SetValue((byte)161, 8);
                            symboldata.SetValue((byte)161, 7);
                            symboldata.SetValue((byte)173, 8);
                            symboldata.SetValue((byte)186, 9);
                            symboldata.SetValue((byte)210, 10);
                            symboldata.SetValue((byte)235, 11);
                            /*                        symboldata.SetValue((byte)174, 9);
                                                    symboldata.SetValue((byte)188, 10);
                                                    symboldata.SetValue((byte)205, 11);*/
                        }

                    }
                    else if (symbolname == "Ign_map_0_x_axis!")
                    {
                        if (symboldata.Length == 36) // 18 values, 16 bit
                        {
                            // values are 120, 134, 152, 178, 195, 210, 220, 230
                            symboldata.SetValue((byte)0, 20);
                            symboldata.SetValue((byte)120, 21);
                            symboldata.SetValue((byte)0, 22);
                            symboldata.SetValue((byte)133, 23);
                            symboldata.SetValue((byte)0, 24);
                            symboldata.SetValue((byte)146, 25);
                            symboldata.SetValue((byte)0, 26);
                            symboldata.SetValue((byte)160, 27);
                            symboldata.SetValue((byte)0, 28);
                            symboldata.SetValue((byte)187, 29);
                            symboldata.SetValue((byte)0, 30);
                            symboldata.SetValue((byte)200, 31);
                            symboldata.SetValue((byte)0, 32);
                            symboldata.SetValue((byte)217, 33);
                            symboldata.SetValue((byte)0, 34);
                            symboldata.SetValue((byte)233, 35);
                        }
                    }
                }
                savedatatobinary(GetSymbolAddress(symbolname), GetSymbolLength(symbolname), symboldata, m_fileInformation.Filename);
            }
            else
            {
                m_resume.AddToResumeTable("Couldn't find axis symbol: " + symbolname);
            }

        }

        private void AlterYAxisForThreeBarSensor(string symbolname)
        {
            m_resume.AddToResumeTable("Altering y axis three bar sensor: " + symbolname);

        }

        /// <summary>
        /// TODO: Should tune and smooth out insp_mat and fuel_knock_mat for use with 3 bar mapsensor
        /// </summary>
        /// <param name="symbolname"></param>
        private void TuneAndSmoothTable(string symbolname, int columncount)
        {
            // de fuel_map_xaxis is is altered with higher values in last 4 columns (highest load). 
            // the injection should be altered too
            // difficult because map is already at it's maximum in stock binaries.
            if (GetSymbolAddress(symbolname) > 0)
            {
                // first try: shift last 3 columns 2 to the left and fill out last column
                try
                {
                    byte[] fuel_injection_map = readdatafromfile(m_fileInformation.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));

                    for (int columnindex = columncount - 3; columnindex < columncount; columnindex++)
                    {
                        for (int rowindex = 0; rowindex < fuel_injection_map.Length / columncount; rowindex++)
                        {
                            byte b = (byte)fuel_injection_map.GetValue((columncount * rowindex) + columnindex);
                            fuel_injection_map.SetValue(b, (columncount * rowindex) + columnindex - 2); //was - 1
                        }
                    }
                    // now fill last 2 column s

                    for (int rindex = 0; rindex < fuel_injection_map.Length / columncount; rindex++)
                    {
                        byte b = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 2));
                        byte prev_byte1 = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 3));
                        byte prev_byte2 = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 4));
                        double diff = Math.Abs(prev_byte1 - prev_byte2);
                        if (diff > 10) diff = 10; // nieuw
                        if (diff < -10) diff = -10; // nieuw
                        double result = prev_byte1 + (byte)diff;
                        if (result > 255) result = 255;
                        fuel_injection_map.SetValue((byte)result, (rindex * columncount) + (columncount - 2));

                    }

                    for (int rindex = 0; rindex < fuel_injection_map.Length / columncount; rindex++)
                    {
                        byte b = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 1));
                        byte prev_byte1 = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 2));
                        byte prev_byte2 = (byte)fuel_injection_map.GetValue((rindex * columncount) + (columncount - 3));
                        double diff = Math.Abs(prev_byte1 - prev_byte2);
                        if (diff > 10) diff = 10; // nieuw
                        if (diff < -10) diff = -10; // nieuw
                        double result = prev_byte1 + (byte)diff;
                        if (result > 255) result = 255;
                        fuel_injection_map.SetValue((byte)result, (rindex * columncount) + (columncount - 1));

                    }
                    savedatatobinary(GetSymbolAddress(symbolname), GetSymbolLength(symbolname), fuel_injection_map, m_fileInformation.Filename);
                    m_resume.AddToResumeTable("Altered and smoothed: " + symbolname);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                    m_resume.AddToResumeTable("Failed to alter and smooth: " + symbolname);
                }
            }
            else
            {
                m_resume.AddToResumeTable("Couldn't find symbol to tune and smooth: " + symbolname);
            }
        }


        /// <summary>
        /// TODO: Should tune and smooth out ign_map_0 for use with 3 bar mapsensor
        /// </summary>
        /// <param name="symbolname"></param>
        private void TuneAndSmoothTableSixteen(string symbolname, int columncount)
        {
            if (GetSymbolAddress(symbolname) > 0)
            {
                // first try: shift last 7 columns 1 to the left and fill out last column
                try
                {
                    byte[] ign_map = readdatafromfile(m_fileInformation.Filename, GetSymbolAddress(symbolname), GetSymbolLength(symbolname));

                    for (int columnindex = columncount - 7; columnindex < columncount - 3; columnindex++)
                    {
                        for (int rowindex = 0; rowindex < ign_map.Length / (columncount * 2); rowindex++)
                        {
                            byte b1 = (byte)ign_map.GetValue((columncount * rowindex * 2) + columnindex * 2);
                            ign_map.SetValue(b1, (columncount * rowindex * 2) + (columnindex * 2) - 2);
                            byte b2 = (byte)ign_map.GetValue((columncount * rowindex * 2) + (columnindex * 2) + 1);
                            ign_map.SetValue(b2, (columncount * rowindex * 2) + (columnindex * 2) - 1);
                        }
                    }
                    for (int columnindex = columncount - 2; columnindex < columncount; columnindex++)
                    {
                        for (int rowindex = 0; rowindex < ign_map.Length / (columncount * 2); rowindex++)
                        {
                            byte b1 = (byte)ign_map.GetValue((columncount * rowindex * 2) + columnindex * 2);
                            ign_map.SetValue(b1, (columncount * rowindex * 2) + (columnindex * 2) - 4);
                            byte b2 = (byte)ign_map.GetValue((columncount * rowindex * 2) + (columnindex * 2) + 1);
                            ign_map.SetValue(b2, (columncount * rowindex * 2) + (columnindex * 2) - 3);
                        }
                    }
                    // now fill last 2 columns with the values of the highest original column

                    for (int rindex = 0; rindex < ign_map.Length / (columncount * 2); rindex++)
                    {
                        byte b1 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 6));
                        byte b2 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 5));

                        ign_map.SetValue((byte)b1, (rindex * columncount * 2) + ((columncount * 2) - 4));
                        ign_map.SetValue((byte)b2, (rindex * columncount * 2) + ((columncount * 2) - 3));
                        ign_map.SetValue((byte)b1, (rindex * columncount * 2) + ((columncount * 2) - 2));
                        ign_map.SetValue((byte)b2, (rindex * columncount * 2) + ((columncount * 2) - 1));

                    }


                    /*            INTERPOLATION VERSION        for (int rindex = 0; rindex < ign_map.Length / (columncount * 2); rindex++)
                                        {
                                            byte b1 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 2));
                                            byte b2 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 1));

                                            byte prev_byte_b1 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 4));
                                            byte prev_byte_b2 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 3));

                                            byte prev_byte2_b1 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 6));
                                            byte prev_byte2_b2 = (byte)ign_map.GetValue((rindex * columncount * 2) + ((columncount * 2) - 5));
                                            int prev_byte1 = (prev_byte_b1 * 256) + prev_byte_b2;
                                            int prev_byte2 = (prev_byte2_b1 * 256) + prev_byte2_b2;
                                            double diff = Math.Abs(prev_byte1 - prev_byte2);
                                            double result = prev_byte1 - (byte)diff; // ign_map decreases
                                            //if (result > 180 && result <= 256) result -= 256;    // last column maybe negative but doesn't hold large advances
                                                                                // this is not fool proof, but works in practice.
                                            if (result < 0) result = 0x10000 - Math.Abs(result);
                                            //if (result > 65535) result = 65535;
                                            // save it
                                            //int ires = (int)result;
                                            byte resb1 = (byte) (result / 256);
                                            byte resb2 = (byte)(result - ((double)resb1 * 256));
                                            ign_map.SetValue((byte)resb1, (rindex * columncount * 2) + ((columncount * 2) - 2));
                                            ign_map.SetValue((byte)resb2, (rindex * columncount * 2) + ((columncount * 2) - 1));

                                        }*/
                    savedatatobinary(GetSymbolAddress(symbolname), GetSymbolLength(symbolname), ign_map, m_fileInformation.Filename);
                    m_resume.AddToResumeTable("Altered and smoothed: " + symbolname);
                }
                catch (Exception E)
                {
                    Console.WriteLine(E.Message);
                    m_resume.AddToResumeTable("Failed to alter and smooth: " + symbolname);
                }
            }
            else
            {
                m_resume.AddToResumeTable("Couldn't find symbol to tune and smooth: " + symbolname);
            }
        }

        private double CalculateConversionFactor(MapSensorType fromSensorType, MapSensorType toSensorType)
        {
            double factor = 1.2;
            switch (fromSensorType)
            {
                case MapSensorType.MapSensor25:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 1.0;           // from 2.5 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 1.2;           // from 2.5 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.4;           // from 2.5 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.6;           // from 2.5 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 2.0;           // from 2.5 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor30:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.8333;           // from 3.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 1.0;           // from 3.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.1666;           // from 3.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.3333;           // from 3.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.6667;           // from 3.0 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor35:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.7143;           // from 3.5 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.8571;           // from 3.5 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 1.0;           // from 3.5 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.1429;           // from 3.5 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.42857;           // from 3.5 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor40:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.6250;           // from 4.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.75;           // from 4.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 0.875;           // from 4.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 1.0;           // from 4.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.25;           // from 4.0 to 5.0 mapsensor
                            break;
                    }
                    break;
                case MapSensorType.MapSensor50:
                    switch (toSensorType)
                    {
                        case MapSensorType.MapSensor25:
                            factor = 0.5;           // from 5.0 to 2.5 mapsensor
                            break;
                        case MapSensorType.MapSensor30:
                            factor = 0.6;           // from 5.0 to 3.0 mapsensor
                            break;
                        case MapSensorType.MapSensor35:
                            factor = 0.7;           // from 5.0 to 3.5 mapsensor
                            break;
                        case MapSensorType.MapSensor40:
                            factor = 0.8;           // from 5.0 to 4.0 mapsensor
                            break;
                        case MapSensorType.MapSensor50:
                            factor = 1.0;           // from 5.0 to 5.0 mapsensor
                            break;
                    }
                    break;
            }
            return factor;
        }


        public void ConvertFileToThreeBarMapSensor(IECUFileInformation fileinformation, MapSensorType fromSensorType, MapSensorType toSensorType)
        {
            m_fileInformation = fileinformation;
//            frmProgress progress = new frmProgress();
//            progress.Show();
//            progress.SetProgress("Checking current configuration...");
            m_resume = new Trionic5Resume();
            m_resume.ResumeTuning = new System.Data.DataTable();
            m_resume.ResumeTuning.Columns.Add("Description");
            if (fromSensorType == toSensorType) return;
            string infoStr = "Tuning your binary from ";
            string infoCopyStr = "beforetuningfrom";
            switch (fromSensorType)
            {
                case MapSensorType.MapSensor25:
                    infoStr += " 2.5 bar sensor to ";
                    infoCopyStr += "250kpasensorto";
                    break;
                case MapSensorType.MapSensor30:
                    infoStr += " 3.0 bar sensor to ";
                    infoCopyStr += "300kpasensorto";
                    break;
                case MapSensorType.MapSensor35:
                    infoStr += " 3.5 bar sensor to ";
                    infoCopyStr += "350kpasensorto";
                    break;
                case MapSensorType.MapSensor40:
                    infoStr += " 4.0 bar sensor to ";
                    infoCopyStr += "400kpasensorto";
                    break;
                case MapSensorType.MapSensor50:
                    infoStr += " 5.0 bar sensor to ";
                    infoCopyStr += "500kpasensorto";
                    break;
            }
            switch (toSensorType)
            {
                case MapSensorType.MapSensor25:
                    infoStr += " 2.5 bar sensor";
                    infoCopyStr += "250kpasensor";
                    break;
                case MapSensorType.MapSensor30:
                    infoStr += " 3.0 bar sensor";
                    infoCopyStr += "300kpasensor";
                    break;
                case MapSensorType.MapSensor35:
                    infoStr += " 3.5 bar sensor";
                    infoCopyStr += "350kpasensor";
                    break;
                case MapSensorType.MapSensor40:
                    infoStr += " 4.0 bar sensor";
                    infoCopyStr += "400kpasensor";
                    break;
                case MapSensorType.MapSensor50:
                    infoStr += " 5.0 bar sensor";
                    infoCopyStr += "500kpasensor";
                    break;
            }
            infoStr += ": " + Path.GetFileName(m_fileInformation.Filename);
            m_resume.AddToResumeTable(infoStr);
            // get the software ID from the bainery
            //progress.SetProgress("Creating backup file...");
            File.Copy(m_fileInformation.Filename, Path.GetDirectoryName(m_fileInformation.Filename) + "\\" + Path.GetFileNameWithoutExtension(m_fileInformation.Filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + infoCopyStr + ".bin", true);
            m_resume.AddToResumeTable("Backup file created (" + Path.GetFileNameWithoutExtension(m_fileInformation.Filename) + DateTime.Now.ToString("yyyyMMddHHmmss") + infoCopyStr + ".bin" + ")");
            // symbols with MAP as values

            infoStr = "Altering tables for";
            switch (toSensorType)
            {
                case MapSensorType.MapSensor25:
                    infoStr += " 2.5 bar sensor...";
                    break;
                case MapSensorType.MapSensor30:
                    infoStr += " 3.0 bar sensor...";
                    break;
                case MapSensorType.MapSensor35:
                    infoStr += " 3.5 bar sensor...";
                    break;
                case MapSensorType.MapSensor40:
                    infoStr += " 4.0 bar sensor...";
                    break;
            }

            //progress.SetProgress(infoStr);

            double factor = CalculateConversionFactor(fromSensorType, toSensorType);

            AlterTableForThreeBarSensor("Tryck_mat!", factor);
            AlterTableForThreeBarSensor("Tryck_mat_a!", factor);
            AlterTableForThreeBarSensor("Tryck_vakt_tab!", factor);
            AlterTableForThreeBarSensor("Regl_tryck_fgaut!", factor);
            AlterTableForThreeBarSensor("Regl_tryck_fgm!", factor);
            AlterTableForThreeBarSensor("Regl_tryck_sgm!", factor);
            AlterTableForThreeBarSensor("Limp_tryck_konst!", factor);
            AlterTableForThreeBarSensor("Idle_tryck!", factor);
            AlterTableForThreeBarSensor("Knock_press_tab!", factor);
            AlterTableForThreeBarSensor("Turbo_knock_tab!", factor);
            AlterTableForThreeBarSensor("Iv_min_load!", factor);

            // new symbols 25/03/2008
            AlterTableForThreeBarSensor("Open_loop!", factor);
            AlterTableForThreeBarSensor("Open_loop_knock!", factor);
            AlterTableForThreeBarSensor("Open_loop_adapt!", factor);
            AlterTableForThreeBarSensor("Lacc_clear_tab!", factor);
            AlterTableForThreeBarSensor("Lner_detekt!", factor);
            AlterTableForThreeBarSensor("Lupp_detekt!", factor);
            AlterTableForThreeBarSensor("Sond_heat_tab!", factor);
            AlterTableForThreeBarSensor("Grund_last!", factor);
            AlterTableForThreeBarSensor("Grund_last_max!", factor);
            //end


            /*
Detect_map_x_axis (1.2 factor, normal)
Reg_last (1.2 factor, normal)
Overstid_tab op 0 gezet (bigger turbo???)
Fuel_knock_xaxis factor 1.2 + increasing in last 5 stages
Fuel_map_xaxis factor 1.2 + increasing in last 5 stages
Ign_map_6_x_axis factor 1.2
Ign_map_2_x_axis factor 1.2
Ign_map_0_x_axis factor 1.2 + increasing in last 8 stages
Misfire_map_x_axis factor 1.2
             
  * * */
            //progress.SetProgress("Altering axis for" + infoStr);
            // symbols with MAP in x axis
            // these three need increasing last steps
            AlterXAxisForThreeBarSensor("Fuel_map_xaxis!", false, factor);
            AlterXAxisForThreeBarSensor("Fuel_knock_xaxis!", false, factor);
            AlterXAxisForThreeBarSensor("Ign_map_0_x_axis!", true, factor);

            AlterXAxisForThreeBarSensor("Ign_map_2_x_axis!", true, factor);
            AlterXAxisForThreeBarSensor("Ign_map_3_x_axis!", true, factor);
            //AlterXAxisForThreeBarSensor("Ign_map_4_x_axis!", false, factor);
            AlterXAxisForThreeBarSensor("Ign_map_6_x_axis!", true, factor);
            AlterXAxisForThreeBarSensor("Ign_map_8_x_axis!", true, factor);
            AlterXAxisForThreeBarSensor("Temp_reduce_x_st!", false, factor);
            AlterXAxisForThreeBarSensor("Misfire_map_x_axis!", true, factor);
            //AlterXAxisForThreeBarSensor("Mis200_map!", factor);
            //AlterXAxisForThreeBarSensor("Misfire_map!", factor);
            AlterXAxisForThreeBarSensor("Detect_map_x_axis!", true, factor);
            //AlterXAxisForThreeBarSensor("Knock_ref_matrix!", factor);
            AlterXAxisForThreeBarSensor("Idle_st_last!", false, factor);
            AlterXAxisForThreeBarSensor("Reg_last!", false, factor);
            // new symbols 25/03/2008
            AlterXAxisForThreeBarSensor("Min_load_gadapt!", false, factor);
            AlterXAxisForThreeBarSensor("Max_load_gadapt!", false, factor);
            AlterXAxisForThreeBarSensor("Kadapt_load_low!", false, factor);
            AlterXAxisForThreeBarSensor("Kadapt_load_high!", false, factor);
            AlterXAxisForThreeBarSensor("Last_cyl_komp!", false, factor);
            AlterXAxisForThreeBarSensor("Overs_tab_xaxis!", false, factor);
            // end
            //if (!m_appSettings.PreventThreeBarRescaling)
            {
                TuneAndSmoothTable("Insp_mat!", 16);
                TuneAndSmoothTable("Fuel_knock_mat!", 12);
                TuneAndSmoothTableSixteen("Ign_map_0!", 18);
                TuneAndSmoothTableSixteen("Ign_map_4!", 18);
            }
            // ALTER Grund_last!
            // ALTER Lam_laststeg!

            // symbols with MAP in Y axis
            //AlterYAxisForThreeBarSensor("Lambdamatris!");

            // Also scale inj_konst with factor 1.2 because the measured MAP value will be 1.2 factor lower 
            // and this value is used to determine Grund_tid in injection duration calculation.

            //<GS-30062010> Probably better to do this after all (have a look at ChrisDuram examples)

            ScaleInjectorConstant(factor); //<GS-06072010> ??? should this be done?
            WriteThreeBarConversionMarker(m_fileInformation.Filename, toSensorType);


            //TryVerifyChecksum(m_fileInformation.Filename);
            //CheckBinForAnomalies(m_fileInformation.Filename, true, false);
            //progress.Close();


        }


        public TuningResult FreeTuneBinary(IECUFile m_File, double peakTorque, double peakBoost, bool tuneBasedOnTorque, MapSensorType mapType, TurboType turboType, InjectorType injectorType, BPCType valve, int rpmlimiter, int knockTime)
        {
            Trionic5Resume _localResume = new Trionic5Resume();
            m_fileInformation = m_File.GetFileInfo();
            string filename = m_fileInformation.Filename;
            // first set things right by running the tunetostagex wizard
            // generate a nice x_scale for ignition map (18 long)
            PressureToTorque ptt = new PressureToTorque();
            double peak_boost = ptt.CalculatePressureFromTorque(peakTorque, turboType);
            if (!tuneBasedOnTorque) peak_boost = peakBoost;
            double peak_boost_request = peak_boost;
            double correction = 1.0;
            if (mapType == MapSensorType.MapSensor30) correction = 1.2;
            if (mapType == MapSensorType.MapSensor35) correction = 1.4;
            if (mapType == MapSensorType.MapSensor40) correction = 1.6;
            if (mapType == MapSensorType.MapSensor50) correction = 2.0;
            peak_boost_request *= 100;
            peak_boost_request += 100;
            peak_boost_request /= correction;
            peak_boost_request -= 100;
            peak_boost_request /= 100;

            double min_pressure = -1;
            double max_pressure = peak_boost;

            #region preparation

            /********* start of prepare phase *********/
            string enginetp = readenginetype(filename);
            string partnumber = readpartnumber(filename);
            PartNumberConverter pnc = new PartNumberConverter();
            ECUInformation ecuinfo = pnc.GetECUInfo(partnumber, enginetp);
            bool isLpt = true;
            if (ReadTunedToStageMarker(filename) > 0)
            {
                return TuningResult.TuningFailedAlreadyTuned;
            }
            else if (ReadThreeBarConversionMarker(filename) > 0)
            {
                return TuningResult.TuningFailedThreebarSensor;
            }
            Trionic5Properties t5p = m_File.GetTrionicProperties();
            if (ecuinfo.Valid)
            {
                if (ecuinfo.Isaero || ecuinfo.Isfpt)
                {
                    isLpt = false;
                }
            }

            if (t5p.MapSensorType != mapType)
            {
                ConvertFileToThreeBarMapSensor(m_fileInformation, t5p.MapSensorType, mapType);
            }
            // check injector type
            if (t5p.InjectorType != injectorType)
            {
                int inj_konst_diff = DetermineDifferenceInInjectorConstant(t5p.InjectorType, injectorType);
                AddToInjectorConstant(filename, inj_konst_diff);
                SetInjectorBatteryCorrectionMap(m_File, injectorType); //TODO: check this function for correctness!
            }
            /*if (injectorType == InjectorType.Stock) writebyteinfile(filename, GetSymbolAddress("Inj_konst!"), 19);
            else if (injectorType == InjectorType.GreenGiants) writebyteinfile(filename, GetSymbolAddress("Inj_konst!"), 18);
            else if (injectorType == InjectorType.Siemens630Dekas) writebyteinfile(filename, GetSymbolAddress("Inj_konst!"), 15);
            else if (injectorType == InjectorType.Siemens875Dekas) writebyteinfile(filename, GetSymbolAddress("Inj_konst!"), 13);
            else if (injectorType == InjectorType.Siemens875Dekas) writebyteinfile(filename, GetSymbolAddress("Inj_konst!"), 13);*/
            t5p.TurboType = turboType;
            t5p.InjectorType = injectorType;
            t5p.MapSensorType = mapType;
            // determine stage??
            int stage = 0;
            if (peak_boost < 1.2) stage = 1;
            else if (peak_boost < 1.3) stage = 2;
            else if (peak_boost < 1.4) stage = 3;
            else if (peak_boost < 1.5) stage = 4;
            else if (peak_boost < 1.6) stage = 5;
            else if (peak_boost < 1.7) stage = 6;
            else if (peak_boost < 1.8) stage = 7;
            else if (peak_boost < 1.9) stage = 8;
            else stage = 9;

            m_File.SetTrionicOptions(t5p);
            TuneToStage(filename, stage, peak_boost_request, 0.52, 1.0, 0.52, 1.54, 90, isLpt, turboType, injectorType, mapType);
            _localResume.ResumeTuning = m_resume.ResumeTuning.Copy();
            /*********** end of prepare phase **************/

            // set limiter, bpc valve type and knock time
            SetBPCValveType(filename, valve);
            _localResume.AddToResumeTable("Set BPC driving frequencies");
            SetRPMLimiter(filename, rpmlimiter);
            _localResume.AddToResumeTable("Set RPM limiter");
            SetKnockTime(filename, knockTime);
            _localResume.AddToResumeTable("Set knock time value");

            #endregion

            // if mapsensor != stock and injectors are 630 cc or bigger
            if (mapType != MapSensorType.MapSensor25 && (injectorType == InjectorType.Siemens630Dekas || injectorType == InjectorType.Siemens875Dekas || injectorType == InjectorType.Siemens1000cc))
            {
                // now scale it
                double step = (max_pressure - min_pressure) / 17;
                double[] axisforIgnitionMap = new double[18];
                for (int i = 0; i < 18; i++)
                {
                    axisforIgnitionMap.SetValue(min_pressure + (i * step), i);
                }
                byte[] actualAxis = new byte[36];
                int j = 0;
                for (int i = 0; i < 18; i++)
                {
                    double currValue = Convert.ToDouble(axisforIgnitionMap.GetValue(i));
                    currValue *= 100;
                    currValue += 100;
                    if (mapType == MapSensorType.MapSensor30) currValue /= 1.2;
                    else if (mapType == MapSensorType.MapSensor35) currValue /= 1.4;
                    else if (mapType == MapSensorType.MapSensor40) currValue /= 1.6;
                    else if (mapType == MapSensorType.MapSensor50) currValue /= 2.0;
                    int ival = Convert.ToInt32(currValue);
                    byte v1 = (byte)(ival / 256);
                    byte v2 = (byte)(ival - (int)v1 * 256);

                    actualAxis.SetValue(v1, j++);
                    actualAxis.SetValue(v2, j++);
                }
                m_File.WriteData(actualAxis, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Ign_map_0_x_axis!"));
                _localResume.AddToResumeTable("Generated and saved new ignition map x axis");
                //Generate the ignition map based on the axis values
                GenerateAndSaveNewIgnitionMap(m_File, false);
                _localResume.AddToResumeTable("Generated and saved new ignition map");
                min_pressure = -0.8;
                step = (max_pressure - min_pressure) / 15;
                // now setup x axis for fuel map
                double[] axisforFuelMap = new double[16];
                for (int i = 0; i < 16; i++)
                {
                    axisforFuelMap.SetValue(min_pressure + (i * step), i);
                }
                byte[] actualFuelAxis = new byte[16];
                for (int i = 0; i < 16; i++)
                {
                    double currValue = Convert.ToDouble(axisforFuelMap.GetValue(i));
                    currValue *= 100;
                    currValue += 100;
                    if (mapType == MapSensorType.MapSensor30) currValue /= 1.2;
                    else if (mapType == MapSensorType.MapSensor35) currValue /= 1.4;
                    else if (mapType == MapSensorType.MapSensor40) currValue /= 1.6;
                    else if (mapType == MapSensorType.MapSensor50) currValue /= 2.0;
                    int ival = Convert.ToInt32(currValue);
                    if (ival > 255) ival = 255;
                    actualFuelAxis.SetValue((byte)ival, i);
                }
                m_File.WriteData(actualFuelAxis, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_map_xaxis!"));
                _localResume.AddToResumeTable("Generated and saved new fuel map x axis");

                //Generate the ignition map based on the axis values
                GenerateAndSaveNewFuelMap(m_File);
                _localResume.AddToResumeTable("Generated and saved new fuel map");

                min_pressure = -0.3;
                step = (max_pressure - min_pressure) / 11;
                // now setup x axis for fuel map
                double[] axisforFuelKnockMap = new double[12];
                for (int i = 0; i < 12; i++)
                {
                    axisforFuelKnockMap.SetValue(min_pressure + (i * step), i);
                }
                byte[] actualFuelKnockAxis = new byte[12];
                for (int i = 0; i < 12; i++)
                {
                    double currValue = Convert.ToDouble(axisforFuelKnockMap.GetValue(i));
                    currValue *= 100;
                    currValue += 100;
                    if (mapType == MapSensorType.MapSensor30) currValue /= 1.2;
                    else if (mapType == MapSensorType.MapSensor35) currValue /= 1.4;
                    else if (mapType == MapSensorType.MapSensor40) currValue /= 1.6;
                    else if (mapType == MapSensorType.MapSensor50) currValue /= 2.0;
                    int ival = Convert.ToInt32(currValue);
                    if (ival > 255) ival = 255;
                    actualFuelKnockAxis.SetValue((byte)ival, i);
                }
                m_File.WriteData(actualFuelKnockAxis, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_knock_xaxis!"));
                _localResume.AddToResumeTable("Generated and saved new fuel knock map x axis");

                //Generate the ignition map based on the axis values
                GenerateAndSaveNewFuelKnockMap(m_File);
                _localResume.AddToResumeTable("Generated and saved new fuel knock map");

                // mesh up a boost request map for this.. already possible
                // adjust peak boost to be scaled for the mapsensor type

                SetBoostRequestMaps(turboType, injectorType, mapType, m_File.GetFileInfo().Filename, peak_boost_request, 100, isLpt);
                _localResume.AddToResumeTable("Generated boost request maps");


            }
            m_resume.ResumeTuning = _localResume.ResumeTuning.Copy();
            return TuningResult.TuningSuccess;

        }

        private void SetKnockTime(string filename, int knockTime)
        {
            if (GetSymbolAddress("Knock_matrix_time!") > 0)
            {
                byte[] timeMax = new byte[2];
                int time1 = knockTime / 256;
                int time2 = knockTime - (time1 * 256);
                timeMax.SetValue(Convert.ToByte(time1), 0);
                timeMax.SetValue(Convert.ToByte(time2), 1);
                savedatatobinary(GetSymbolAddress("Knock_matrix_time!"), 2, timeMax, filename);
            }
        }

        public void SetRPMLimiter(string filename, int rpmlimiter)
        {
            if (GetSymbolAddress("Rpm_max!") > 0)
            {
                byte[] rpmMax = new byte[2];
                int rpm1 = rpmlimiter / 256;
                int rpm2 = rpmlimiter - (rpm1 * 256);
                rpmMax.SetValue(Convert.ToByte(rpm1), 0);
                rpmMax.SetValue(Convert.ToByte(rpm2), 1);
                savedatatobinary(GetSymbolAddress("Rpm_max!"), 2, rpmMax, filename);
            }
        }

        private void SetBPCValveType(string filename, BPCType valve)
        {
            if (GetSymbolAddress("Frek_230!") > 0)
            {
                byte[] frek230 = new byte[2];
                byte[] frek250 = new byte[2];
                FileInfo fi = new FileInfo(filename);
                if (fi.Length == 0x20000)
                {
                    // T5.2
                    if (valve == BPCType.Trionic5Valve)
                    {
                        frek230.SetValue((byte)0x02,0);
                        frek230.SetValue((byte)0xD8, 1);
                        //frek230 = 728;
                        frek250.SetValue((byte)0x03, 0);
                        frek250.SetValue((byte)0xA7, 1);
                        //frek250 = 935;
                    }
                    else
                    {
                        frek230.SetValue((byte)0x02, 0);
                        frek230.SetValue((byte)0x76, 1);
                        //frek230 = 630; //TODO: fill right frek
                        //frek250 = 1900; //TODO: fill right frek
                        frek250.SetValue((byte)0x07, 0);
                        frek250.SetValue((byte)0x6C, 1);
                    }
                }
                else
                {
                    // T5.5
                    if (valve == BPCType.Trionic5Valve)
                    {
                        frek230.SetValue((byte)0x00, 0);
                        frek230.SetValue((byte)0x5A, 1);
                        //frek230 = 90;
                        //frek250 = 70;
                        frek250.SetValue((byte)0x00, 0);
                        frek250.SetValue((byte)0x46, 1);
                    }
                    else
                    {
                        frek230.SetValue((byte)0x00, 0);
                        frek230.SetValue((byte)0x32, 1);
                        //frek230 = 50; 
                        //frek250 = 32;
                        frek250.SetValue((byte)0x00, 0);
                        frek250.SetValue((byte)0x20, 1);
                    }
                }
                // write the values
                savedatatobinary(GetSymbolAddress("Frek_230!"), 2, frek230, filename);
                savedatatobinary(GetSymbolAddress("Frek_250!"), 2, frek250, filename);

            }
        }

        private void GenerateAndSaveNewFuelKnockMap(IECUFile m_File)
        {
            TuningReferenceMaps _referenceMaps = new TuningReferenceMaps();
            byte[] pressure_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_knock_xaxis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Fuel_knock_xaxis!"));
            byte[] rpm_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_map_yaxis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Fuel_map_yaxis!"));
            byte[] new_fuel_map = new byte[12 * 16];
            int fuel_map_index = 0;
            for (int rpm_index = rpm_axis.Length - 2; rpm_index >= 0; rpm_index -= 2)
            {
                for (int pressure_index = 0; pressure_index < pressure_axis.Length; pressure_index++)
                {

                    int irpm = Convert.ToInt32(rpm_axis[rpm_index]) * 256;
                    irpm += Convert.ToInt32(rpm_axis[rpm_index + 1]);
                    irpm *= 10;
                    int ipressure = Convert.ToInt32(pressure_axis[pressure_index]);
                    double correctionFactor = 1;
                    double pressure = ipressure;
                    MapSensorType mapsensor = m_File.GetMapSensorType(false);
                    if (mapsensor == MapSensorType.MapSensor30) correctionFactor = 1.2;
                    else if (mapsensor == MapSensorType.MapSensor35) correctionFactor = 1.4;
                    else if (mapsensor == MapSensorType.MapSensor40) correctionFactor = 1.6;
                    else if (mapsensor == MapSensorType.MapSensor50) correctionFactor = 2.0;
                    pressure *= correctionFactor;
                    pressure -= 100;
                    pressure /= 100;
                    double fuel_correction = _referenceMaps.FuelCorrectionForPressureRpm(pressure, (double)irpm);
                    // add 5% 
                    fuel_correction *= 1.05;
                    // write ignition advance into the map
                    fuel_correction -= 0.5;
                    fuel_correction /= 0.00390625;

                    Int32 icorrection = Convert.ToInt32(fuel_correction);
                    if (icorrection > 255) icorrection = 255;
                    new_fuel_map[fuel_map_index++] = (byte)icorrection;
                }
            }
            // now save the map
            m_File.WriteData(new_fuel_map, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_knock_mat!"));
        }

        private void GenerateAndSaveNewFuelMap(IECUFile m_File)
        {
            TuningReferenceMaps _referenceMaps = new TuningReferenceMaps();
            byte[] pressure_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_map_xaxis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Fuel_map_xaxis!"));
            byte[] rpm_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Fuel_map_yaxis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Fuel_map_yaxis!"));
            byte[] new_fuel_map = new byte[16 * 16];
            int fuel_map_index = 0;
            for (int rpm_index = rpm_axis.Length - 2; rpm_index >= 0; rpm_index -= 2)
            {
                for (int pressure_index = 0; pressure_index < pressure_axis.Length; pressure_index ++)
                {

                    int irpm = Convert.ToInt32(rpm_axis[rpm_index]) * 256;
                    irpm += Convert.ToInt32(rpm_axis[rpm_index + 1]);
                    irpm *= 10;
                    int ipressure = Convert.ToInt32(pressure_axis[pressure_index]);
                    double correctionFactor = 1;
                    double pressure = ipressure;
                    MapSensorType mapsensor = m_File.GetMapSensorType(false);
                    if (mapsensor == MapSensorType.MapSensor30) correctionFactor = 1.2;
                    else if (mapsensor == MapSensorType.MapSensor35) correctionFactor = 1.4;
                    else if (mapsensor == MapSensorType.MapSensor40) correctionFactor = 1.6;
                    else if (mapsensor == MapSensorType.MapSensor50) correctionFactor = 2.0;
                    pressure *= correctionFactor;
                    pressure -= 100;
                    pressure /= 100;
                    double fuel_correction = _referenceMaps.FuelCorrectionForPressureRpm(pressure, (double)irpm);
                    // write ignition advance into the map
                    fuel_correction -= 0.5;
                    fuel_correction /= 0.00390625;

                    Int32 icorrection = Convert.ToInt32(fuel_correction);
                    if (icorrection > 255) icorrection = 255;
                    new_fuel_map[fuel_map_index++] = (byte)icorrection;
                }
            }
            // now save the map
            m_File.WriteData(new_fuel_map, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Insp_mat!"));
        }


        public void ConvertToE85(IECUFile m_File)
        {
            m_resume = new Trionic5Resume();

            m_resume.AddToResumeTable("Start tuning for E85");
            GenerateAndSaveNewIgnitionMap(m_File, true);
            m_resume.AddToResumeTable("Generated and saved ignition map for E85 use");
            // now set fuelling enrichment
            IncreaseByteVariableWith140Percent(m_File, "Inj_konst!");
            m_resume.AddToResumeTable("Increased injector constant by 40%");
            IncreaseByteVariableWith140Percent(m_File, "Eftersta_fak!");
            IncreaseByteVariableWith140Percent(m_File, "Eftersta_fak2!");
            m_resume.AddToResumeTable("Increased afterstart factors by 40%");
            GenerateStartVevFak(m_File);
            m_resume.AddToResumeTable("Generated starting enrichment factors");
        }

        private void IncreaseByteVariableWith140Percent(IECUFile m_File, string symbol)
        {
            // read value
            byte[] data = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash(symbol), (uint)m_File.GetFileInfo().GetSymbolLength(symbol));
            for (int t = 0; t < data.Length; t++)
            {
                double dval = Convert.ToDouble(data[t]);
                dval *= 1.4;
                data[t] = Convert.ToByte(dval);
            }
            m_File.WriteData(data, (uint)m_File.GetFileInfo().GetSymbolAddressFlash(symbol));
        }

        private void GenerateStartVevFak(IECUFile m_File)
        {
            byte[] vev_fak = new byte[15] { 8, 12, 16, 20, 23, 32, 36, 45, 60, 104, 128, 168, 208, 254, 255 };
            m_File.WriteData(vev_fak, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Startvev_fak!"));
            
        }

        private void GenerateAndSaveNewIgnitionMap(IECUFile m_File, bool runsE85)
        {
            //TODO: Implement
            // get the axis
            TuningReferenceMaps _referenceMaps = new TuningReferenceMaps();
            byte[] pressure_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Ign_map_0_x_axis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Ign_map_0_x_axis!"));
            byte[] rpm_axis = m_File.ReadData((uint)m_File.GetFileInfo().GetSymbolAddressFlash("Ign_map_0_y_axis!"), (uint)m_File.GetFileInfo().GetSymbolLength("Ign_map_0_y_axis!"));
            byte[] new_ignition_map = new byte[16 * 18 * 2];
            int ign_map_index = 0;
            for (int rpm_index = rpm_axis.Length-2; rpm_index >=0; rpm_index -= 2)
            {
                for (int pressure_index = 0; pressure_index < pressure_axis.Length; pressure_index += 2)
                {
                
                    int irpm = Convert.ToInt32(rpm_axis[rpm_index]) * 256;
                    irpm += Convert.ToInt32(rpm_axis[rpm_index+1]);
                    int ipressure = Convert.ToInt32(pressure_axis[pressure_index]) * 256;
                    ipressure += Convert.ToInt32(pressure_axis[pressure_index + 1]);
                    double correctionFactor = 1;
                    double pressure = ipressure;
                    MapSensorType mapsensor = m_File.GetMapSensorType(false);
                    if (mapsensor == MapSensorType.MapSensor30) correctionFactor = 1.2;
                    else if (mapsensor == MapSensorType.MapSensor35) correctionFactor = 1.4;
                    else if (mapsensor == MapSensorType.MapSensor40) correctionFactor = 1.6;
                    else if (mapsensor == MapSensorType.MapSensor50) correctionFactor = 2.0;
                    pressure *= correctionFactor;
                    pressure -= 100;
                    pressure /= 100;
                    double ignition_advance = _referenceMaps.GetIgnitionAdvanceForPressureRpm(pressure, (double)irpm);
                    if (runsE85) ignition_advance = _referenceMaps.GetIgnitionAdvanceE85ForPressureRpm(pressure, (double)irpm);
                    // write ignition advance into the map
                    ignition_advance *= 10; // correction factor
                    Int32 iAdvance = Convert.ToInt32(ignition_advance);
                    byte v1 = (byte)(iAdvance / 256);
                    byte v2 = (byte)(iAdvance - (int)v1 * 256);
                    //Console.WriteLine("Writing data rpmidx : " + rpm_index.ToString() + " mapidx: " + pressure_index.ToString() + " ignidx: " + ign_map_index.ToString());
                    new_ignition_map[ign_map_index++] = v1;
                    new_ignition_map[ign_map_index++] = v2;
                }
            }
            // now save the map
            m_File.WriteData(new_ignition_map, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Ign_map_0!"));
            m_File.WriteData(new_ignition_map, (uint)m_File.GetFileInfo().GetSymbolAddressFlash("Ign_map_4!")); // save as warmup map as well
        }

        private void ScaleInjectorConstant(double factor)
        {
            // get the current injector constant
            byte[] data = readdatafromfile(m_fileInformation.Filename, GetSymbolAddress("Inj_konst!"), GetSymbolLength("Inj_konst!"));
            int inj_konst = Convert.ToInt32(data.GetValue(0));
            m_resume.AddToResumeTable("Injector constant was: " + inj_konst.ToString());
            double inj_konst_f = (double)inj_konst;
            inj_konst_f *= factor;
            inj_konst = Convert.ToInt32(Math.Floor(inj_konst_f));
            data.SetValue((byte)inj_konst, 0);
            savedatatobinary(GetSymbolAddress("Inj_konst!"), GetSymbolLength("Inj_konst!"), data, m_fileInformation.Filename);
            m_resume.AddToResumeTable("New injector constant is: " + inj_konst.ToString());
        }
        #endregion

        internal void LiftBoostRequestForTurboType(TurboType turboType)
        {
            // read boost request but hold peak boost for longer, depeding on turbo type
        }
    }
}
