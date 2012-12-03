using System;
using System.Collections.Generic;
using System.Text;



namespace T8SuitePro
{
    class PartNumberConverter
    {
        public PartNumberConverter()
        {

        }

        public ECUInformation GetECUInfo(string partnumber, string enginetype)
        {
            ECUInformation returnvalue = new ECUInformation();
            returnvalue.Tunedbyt8stostage = 0;
            switch (partnumber)
            {
                #region SAAB93

                case "55567225_FD0G_C_FMEP_90_FIEF_81m":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0G_C_FMEP_90_FIEF_81m";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_72_FIEF_81c":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_72_FIEF_81c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_72_FIEF_81d":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_72_FIEF_81d";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0J_C_FMEP_63_FIEF_82s":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0J_C_FMEP_63_FIEF_82s";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0J_C_FMEP_63_FIEF_81j":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Valid = true;
                    returnvalue.Softwareversion = "FC0J_C_FMEP_63_FIEF_81j";
                    break;
                case "55353231_FA5I_C_FME3_71_FIEF_80d":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME3_71_FIEF_80d";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA56_C_FMEP_37_FIEF_81c":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA56_C_FMEP_37_FIEF_81c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_80c":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_80c";
                    returnvalue.Valid = true;
                    break;
                /*case "55353231_FA5B_C_FMEP_46_FIEF_80c":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_80c";
                    returnvalue.Valid = true;
                    break;*/
                case "55352688_FA4H_C_FME9_28_SAN_PF_81b":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4H_C_FME9_28_SAN_PF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_81c":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_81c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_82c":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_82c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_66_FIEFF_81e":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_66_FIEFF_81e";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_65_FIEF_81f":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_65_FIEF_81f";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_65_FIEF_80d":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_65_FIEF_80d";
                    returnvalue.Valid = true;
                    break;
                /*case "55353231_FA5I_C_FME2_65_FIEF_80d":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_65_FIEF_80d";
                    returnvalue.Valid = true;
                    break;*/
                case "55352688_FA4Y_C_FME2_3Z_FME_PIF_83e":
                    returnvalue.Enginetype = EngineType.Z20NET;
                    returnvalue.Carmodel = CarModel.OpelSignum;
                    returnvalue.Valid = true;
                    returnvalue.Softwareversion = "FA4Y_C_FME2_3Z_FME_PIF_83e";
                    break;
                case "55353231_FA5B_C_FME3_4M_FME_PIF_83f":
                    returnvalue.Enginetype = EngineType.Z20NET;
                    returnvalue.Carmodel = CarModel.OpelVectra;
                    returnvalue.Softwareversion = "FA5B_C_FME3_4M_FME_PIF_83f";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_4R_FMEF_83g":
                    returnvalue.Enginetype = EngineType.Z20NET;
                    returnvalue.Carmodel = CarModel.OpelVectra;
                    returnvalue.Softwareversion = "FA5I_C_FME2_4R_FMEF_83g";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FMEP_4T_FMEF_83h":
                    returnvalue.Enginetype = EngineType.Z20NET;
                    returnvalue.Carmodel = CarModel.OpelVectra;
                    returnvalue.Softwareversion = "FA5L_C_FMEP_4T_FMEF_83h";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0G_C_FMEP_05_JWAFFF_83i":
                    returnvalue.Enginetype = EngineType.Z20NET;
                    returnvalue.Carmodel = CarModel.OpelVectra;
                    returnvalue.Softwareversion = "FC0G_C_FMEP_05_JWAFFF_83i";
                    returnvalue.Valid = true;
                    break;
                case "55352688_FA5B_C_FME4_52_FIEF_81b":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FME4_52_FIEF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_65_FIEF_82h":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_65_FIEF_82h";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_72_FIEF_80c":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_72_FIEF_80c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_73_FIEF_80c":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_73_FIEF_80c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_66_FIEFF_80d":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_66_FIEFF_80d";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0J_C_FMEP_63_FIEF_80f":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0J_C_FMEP_63_FIEF_80f";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA56_C_FME2_37_FIEF_81c":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA56_C_FME2_37_FIEF_81c";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME3_71_FIEF_81e":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME3_71_FIEF_81e";
                    returnvalue.Valid = true;
                    break;
                case "55352688_FA5B_C_FME4_52_FIEF_82b":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FME4_52_FIEF_82b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA4W_C_FME2_90_FMEF_82b":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4W_C_FME2_90_FMEF_82b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_65_FIEF_82i":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_65_FIEF_82i";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME3_71_FIEF_82h":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME3_71_FIEF_82h";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0G_C_FMEP_61_FIEFF_82s":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0G_C_FMEP_61_FIEFF_82s";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FME2_79_FIEF_81i":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.CadillacBTS;
                    returnvalue.Softwareversion = "FA5L_C_FME2_79_FIEF_81i";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FC0U_C_FME1_14_FIEF_828":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0U_C_FME1_14_FIEF_828";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FE09_C_FME5_A0_FIEF_82x":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FE09_C_FME5_A0_FIEF_82x";
                    returnvalue.Valid = true;
                    break;
                case "55352571_FA4H_C_FME6_20_MWO_PF_81b":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4H_C_FME6_20_MWO_PF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_82d":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_82d";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FE09_C_FME4_99_FIEF_82v":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FE09_C_FME4_99_FIEF_82v";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0J_C_FMEP_63_FIEF_82r":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0J_C_FMEP_63_FIEF_82r";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FMEP_77_FIEF_82n":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5L_C_FMEP_77_FIEF_82n";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME4_73_FIEF_80d":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME4_73_FIEF_80d";
                    returnvalue.Valid = true;
                    break;
                /*case "55352688_FA5B_C_FME4_52_FIEF_82b":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FME4_52_FIEF_82b";
                    returnvalue.Valid = true;
                    break;
                case "55352688_FA5B_C_FME4_52_FIEF_82b":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FME4_52_FIEF_82b";
                    returnvalue.Valid = true;
                    break;*/
                case "55565020_FD0D_C_FMEP_16_FIEF_80g":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0D_C_FMEP_16_FIEF_80g";
                    returnvalue.Valid = true;
                    break;
                /*case "55352688_FA4H_C_FME9_28_SAN_PF_81b":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4H_C_FME9_28_SAN_PF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_82d":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_82d";
                    returnvalue.Valid = true;
                    break;*/


                case "55352688_FA4H_C_FME5_07_MWO_PF_81b":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4H_C_FME5_07_MWO_PF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FE1D_C_FMEP_15_FIEF_85d":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FE1D_C_FMEP_15_FIEF_85d";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FD0F_C_FMEP_30_FIEF_81I":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0F_C_FMEP_30_FIEF_81I";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FE18_C_FME2_20_FIEF_826":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FE18_C_FME2_20_FIEF_826";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_66_FIEFF_82h":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_66_FIEFF_82h";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0G_C_FMEP_61_FIEFF_80f":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0G_C_FMEP_61_FIEFF_80f";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FD0F_C_FMEP_30_FIEF_80g":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0F_C_FMEP_30_FIEF_80g";
                    returnvalue.Valid = true;
                    break;
                case "55352571_FA4H_C_FME9_28_SAN_PF_81b":
                    returnvalue.Enginetype = EngineType.Unknown;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA4H_C_FME9_28_SAN_PF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5B_C_FMEP_46_FIEF_82m":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FMEP_46_FIEF_82m";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FMEP_63_FIEF_82h":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FMEP_63_FIEF_82h";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FMEP_77_FIEF_82p":
                    returnvalue.Enginetype = EngineType.Unknown;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5L_C_FMEP_77_FIEF_82p";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FF0C_C_FME1_73_FIEF_82z":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FF0C_C_FME1_73_FIEF_82z";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FD0F_C_FMEP_30_FIEF_81I":
                    returnvalue.Enginetype = EngineType.Unknown;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0F_C_FMEP_30_FIEF_81I";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FD0H_C_FMEP_90_FIEF_81m":
                    returnvalue.Enginetype = EngineType.Unknown;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0H_C_FMEP_90_FIEF_81m";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME2_66_FIEFF_82j":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME2_66_FIEFF_82j";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5I_C_FME3_72_FIEF_81e":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5I_C_FME3_72_FIEF_81e";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FMEP_78_FIEF_80e":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5L_C_FMEP_78_FIEF_80e";
                    returnvalue.Valid = true;
                    break;
                case "55353231_FA5L_C_FMEP_78_FIEF_81i":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5L_C_FMEP_78_FIEF_81i";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FD0M_C_FMEP_14_FIEF_80l":
                    returnvalue.Enginetype = EngineType.B207E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FD0M_C_FMEP_14_FIEF_80l";
                    returnvalue.Valid = true;
                    break;
                case "55567225_FE09_C_FME2_96_FIEF_82v":
                    returnvalue.Enginetype = EngineType.B207R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FE09_C_FME2_96_FIEF_82v";
                    returnvalue.Valid = true;
                    break;
                case "55352571_FA5B_C_FME4_52_FIEF_81b":
                    returnvalue.Enginetype = EngineType.B207L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FA5B_C_FME4_52_FIEF_81b";
                    returnvalue.Valid = true;
                    break;
                case "55565020_FC0J_C_FMEP_01_FMPF_83i":
                    returnvalue.Enginetype = EngineType.Unknown;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "FC0J_C_FMEP_01_FMPF_83i";
                    returnvalue.Valid = true;
                    break;

                #endregion
            }

            if (returnvalue.Carmodel == CarModel.Saab93)
            {

                switch (returnvalue.Enginetype)
                {
                    case EngineType.B207E:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 980
                        returnvalue.Stage1torque = 280;
                        returnvalue.Valid = true;
                        break;
                    case EngineType.B207L:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 980
                        returnvalue.Stage1torque = 300;
                        returnvalue.Valid = true;
                        break;
                    case EngineType.B207R:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 970
                        returnvalue.Stage1torque = 320;
                        returnvalue.Valid = true;
                        break;
                
                }
            }
            return returnvalue;
        }
    }

    enum CarModel : int
    {
        Unknown = 0,
        Saab93 = 1,
        Saab95 = 2,
        OpelVectra,
        OpelSignum,
        CadillacBTS

    }

    enum EngineType : int
    {
        Unknown,
        B204,
        B204E,
        B204L,
        B204R,
        B205,
        B205E,
        B205L,
        B205R,
        B234,
        B234I,
        B234E,
        B234L,
        B234R,
        B235,
        B235E,
        B235L,
        B235R,
        B308E,
        B207E,
        B207L,
        B207R,
        Z20NET

    }

    enum TurboModel : int
    {
        None,
        GarretT25,
        GarretTB2529,
        GarretTB2531,
        MitsuTD04
    }

    class ECUInformation
    {
        double _stage1airmass = 1300;
        double _stage1torque = 350;

        public double Stage1torque
        {
            get { return _stage1torque; }
            set { _stage1torque = value; }
        }


        public double Stage1airmass
        {
            get { return _stage1airmass; }
            set { _stage1airmass = value; }
        }
        double _stage2airmass = 1350;
        double _stage2torque = 400;

        public double Stage2torque
        {
            get { return _stage2torque; }
            set { _stage2torque = value; }
        }

        public double Stage2airmass
        {
            get { return _stage2airmass; }
            set { _stage2airmass = value; }
        }

        double _stage3airmass = 1400;
        double _stage3torque = 450;

        public double Stage3torque
        {
            get { return _stage3torque; }
            set { _stage3torque = value; }
        }

        public double Stage3airmass
        {
            get { return _stage3airmass; }
            set { _stage3airmass = value; }
        }


        private EngineType _enginetype = EngineType.Unknown;

        internal EngineType Enginetype
        {
            get { return _enginetype; }
            set { _enginetype = value; }
        }

        private CarModel _carmodel = CarModel.Unknown;

        internal CarModel Carmodel
        {
            get { return _carmodel; }
            set { _carmodel = value; }
        }

        private TurboModel _turbomodel = TurboModel.None;

        internal TurboModel Turbomodel
        {
            get { return _turbomodel; }
            set { _turbomodel = value; }
        }


        private bool _valid = false;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }
        private bool _isturbo = false;

        public bool Isturbo
        {
            get { return _isturbo; }
            set { _isturbo = value; }
        }
        private bool _isaero = false;

        public bool Isaero
        {
            get { return _isaero; }
            set { _isaero = value; }
        }
        private bool _isfpt = false;

        public bool Isfpt
        {
            get { return _isfpt; }
            set { _isfpt = value; }
        }
        private bool _is2point3liter = false;

        public bool Is2point3liter
        {
            get { return _is2point3liter; }
            set { _is2point3liter = value; }
        }

        private int _bhp = 0;

        public int Bhp
        {
            get { return _bhp; }
            set { _bhp = value; }
        }

        private double _baseboost = 0;

        public double Baseboost
        {
            get { return _baseboost; }
            set { _baseboost = value; }
        }

        private double _max_stock_boost_manual = 0;

        public double Max_stock_boost_manual
        {
            get { return _max_stock_boost_manual; }
            set { _max_stock_boost_manual = value; }
        }

        private double _max_stock_boost_automatic = 0;

        public double Max_stock_boost_automatic
        {
            get { return _max_stock_boost_automatic; }
            set { _max_stock_boost_automatic = value; }
        }

        private int _torque = 0;

        public int Torque
        {
            get { return _torque; }
            set { _torque = value; }
        }

        private int _tunedbyt8stostage = 0;

        public int Tunedbyt8stostage
        {
            get { return _tunedbyt8stostage; }
            set { _tunedbyt8stostage = value; }
        }

        private bool _automatic_gearbox = false;

        public bool Automatic_gearbox
        {
            get { return _automatic_gearbox; }
            set { _automatic_gearbox = value; }
        }

        private string _softwareversion = string.Empty;

        public string Softwareversion
        {
            get { return _softwareversion; }
            set { _softwareversion = value; }
        }
    }
}
