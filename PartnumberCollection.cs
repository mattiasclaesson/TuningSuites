using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace T8SuitePro
{
    class PartnumberCollection
    {
        DataTable dt;
        private void AddPartNumber(string part, string my)
        {
            PartNumberConverter pnc = new PartNumberConverter();
            ECUInformation ecuinfo = new ECUInformation();
            ecuinfo = pnc.GetECUInfo(part, "");
            dt.Rows.Add(ecuinfo.Carmodel.ToString(), ecuinfo.Enginetype.ToString(), part/*, ecuinfo.Turbomodel.ToString(), ecuinfo.Isturbo.ToString(), ecuinfo.Bhp.ToString(), ecuinfo.Torque.ToString()*/, ecuinfo.Softwareversion, my);

        }

        public DataTable GeneratePartNumberCollection()
        {
            dt = new DataTable();
            dt.Columns.Add("Carmodel");
            dt.Columns.Add("Enginetype");
            dt.Columns.Add("Partnumber");
            //dt.Columns.Add("Turbomodel");
            //dt.Columns.Add("Turbo");
            //dt.Columns.Add("Power");
            //dt.Columns.Add("Torque");
            dt.Columns.Add("SoftwareVersion");
            dt.Columns.Add("Makeyear");

            AddPartNumber("55567225_FD0G_C_FMEP_90_FIEF_81m", "2009");
            AddPartNumber("55353231_FA5I_C_FME2_72_FIEF_81c", "2007");
            AddPartNumber("55353231_FA5I_C_FME2_72_FIEF_81d", "2004");
            AddPartNumber("55565020_FC0J_C_FMEP_63_FIEF_82s", "2007");
            AddPartNumber("55565020_FC0J_C_FMEP_63_FIEF_81j", "2007");
            AddPartNumber("55353231_FA5I_C_FME3_71_FIEF_80d", "2005");
            AddPartNumber("55353231_FA56_C_FMEP_37_FIEF_81c", "2004");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_80c", "2004");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_80c", "2004");
            AddPartNumber("55352688_FA4H_C_FME9_28_SAN_PF_81b", "2003");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_81c", "2004");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_82c", "2004");
            AddPartNumber("55353231_FA5I_C_FME2_66_FIEFF_81e", "2005");
            AddPartNumber("55353231_FA5I_C_FME2_65_FIEF_81f", "2005");
            AddPartNumber("55353231_FA5I_C_FME2_65_FIEF_80d", "2005");
            AddPartNumber("55353231_FA5I_C_FME2_65_FIEF_80d", "2005");
            AddPartNumber("55352688_FA4Y_C_FME2_3Z_FME_PIF_83e", "2003"); // opel signum
            AddPartNumber("55353231_FA5B_C_FME3_4M_FME_PIF_83f", "2004"); // opel vectra
            AddPartNumber("55353231_FA5I_C_FME2_4R_FMEF_83g", "2005"); // opel vectra
            AddPartNumber("55353231_FA5L_C_FMEP_4T_FMEF_83h", "2006"); // opel vectra
            AddPartNumber("55565020_FC0G_C_FMEP_05_JWAFFF_83i", "2008"); // opel vectra
            AddPartNumber("55352688_FA5B_C_FME4_52_FIEF_81b", "2003");
            AddPartNumber("55353231_FA5I_C_FME2_65_FIEF_82h", "2005");
            AddPartNumber("55353231_FA5I_C_FME2_72_FIEF_80c", "2004");
            AddPartNumber("55353231_FA5I_C_FME2_73_FIEF_80c", "2004");
            AddPartNumber("55353231_FA5I_C_FME2_66_FIEFF_80d", "2004-2005");
            AddPartNumber("55565020_FC0J_C_FMEP_63_FIEF_80f", "2007");
            AddPartNumber("55353231_FA56_C_FME2_37_FIEF_81c", "2004-2005");
            AddPartNumber("55353231_FA5I_C_FME3_71_FIEF_81e", "2004-2005");
            AddPartNumber("55352688_FA5B_C_FME4_52_FIEF_82b", "2003");
            AddPartNumber("55353231_FA4W_C_FME2_90_FMEF_82b", "2003");
            AddPartNumber("55353231_FA5I_C_FME2_65_FIEF_82i", "2005");
            AddPartNumber("55353231_FA5I_C_FME3_71_FIEF_82h", "2005");
            AddPartNumber("55565020_FC0G_C_FMEP_61_FIEFF_82s", "2007");
            AddPartNumber("55353231_FA5L_C_FME2_79_FIEF_81i", "2006");
            AddPartNumber("55567225_FC0U_C_FME1_14_FIEF_828", "2010");
            AddPartNumber("55567225_FE09_C_FME5_A0_FIEF_82x", "2009");
            AddPartNumber("55352571_FA4H_C_FME6_20_MWO_PF_81b", "2003");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_82d", "2004");
            AddPartNumber("55567225_FE09_C_FME4_99_FIEF_82v", "2009");
            AddPartNumber("55565020_FC0J_C_FMEP_63_FIEF_82r", "2008");
            AddPartNumber("55353231_FA5L_C_FMEP_77_FIEF_82n", "2006");
            AddPartNumber("55353231_FA5I_C_FME4_73_FIEF_80d", "2005");
            AddPartNumber("55352688_FA5B_C_FME4_52_FIEF_82b", "2003");
            AddPartNumber("55352688_FA5B_C_FME4_52_FIEF_82b", "2003");
            AddPartNumber("55565020_FD0D_C_FMEP_16_FIEF_80g", "2007-2008");
            AddPartNumber("55352688_FA4H_C_FME9_28_SAN_PF_81b", "2003");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_82d", "2004");
            AddPartNumber("55352688_FA4H_C_FME5_07_MWO_PF_81b", "2003");
            AddPartNumber("55567225_FE1D_C_FMEP_15_FIEF_85d", "2011");
            AddPartNumber("55565020_FD0F_C_FMEP_30_FIEF_81I", "2008");
            AddPartNumber("55567225_FE18_C_FME2_20_FIEF_826", "2010");
            AddPartNumber("55353231_FA5I_C_FME2_66_FIEFF_82h", "2005");
            AddPartNumber("55565020_FC0G_C_FMEP_61_FIEFF_80f", "2007");
            AddPartNumber("55565020_FD0F_C_FMEP_30_FIEF_80g", "2008");
            AddPartNumber("55352571_FA4H_C_FME9_28_SAN_PF_81b", "1999");
            AddPartNumber("55353231_FA5B_C_FMEP_46_FIEF_82m", "2004");
            AddPartNumber("55353231_FA5I_C_FMEP_63_FIEF_82h", "2005");
            AddPartNumber("55353231_FA5L_C_FMEP_77_FIEF_82p", "0");
            AddPartNumber("55567225_FF0C_C_FME1_73_FIEF_82z", "2010");
            AddPartNumber("55567225_FD0F_C_FMEP_30_FIEF_81I", "0");
            AddPartNumber("55567225_FD0H_C_FMEP_90_FIEF_81m", "0");
            AddPartNumber("55353231_FA5I_C_FME2_66_FIEFF_82j", "2005");
            AddPartNumber("55353231_FA5I_C_FME3_72_FIEF_81e", "2005");
            AddPartNumber("55353231_FA5L_C_FMEP_78_FIEF_80e", "2006");
            AddPartNumber("55353231_FA5L_C_FMEP_78_FIEF_81i", "2006");
            AddPartNumber("55567225_FD0M_C_FMEP_14_FIEF_80l", "2010");
            AddPartNumber("55567225_FE09_C_FME2_96_FIEF_82v", "2009");
            AddPartNumber("55352571_FA5B_C_FME4_52_FIEF_81b", "2003");
            AddPartNumber("55565020_FC0J_C_FMEP_01_FMPF_83i", "0");

           
            
            

            return dt;
        }
    }
}
