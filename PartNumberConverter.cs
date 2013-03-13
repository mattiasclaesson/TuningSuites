using System;
using System.Collections.Generic;
using System.Text;



namespace T7
{
    class PartNumberConverter
    {

        public PartNumberConverter()
        {

        }

        public ECUInformation GetECUInfo(string partnumber, string enginetype)
        {
            ECUInformation returnvalue = new ECUInformation();
            returnvalue.Tunedbyt7stostage = 0;
            if (enginetype.EndsWith("T7S1")) returnvalue.Tunedbyt7stostage = 1;
            else if (enginetype.EndsWith("T7S2")) returnvalue.Tunedbyt7stostage = 2;
            else if (enginetype.EndsWith("T7S3")) returnvalue.Tunedbyt7stostage = 3;
            switch (partnumber)
            {
                #region SAAB95

                #region B205E
                case "5381728":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0RP1TC.47O";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5168646":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1WF0LC.47D";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5381249":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB0C.47B";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5380522":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1CC.47O";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5380514":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1DC.47P";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5380506":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1AC.47N";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5380498":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1BC.47Q";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_RW;
                    break;
                case "5380845":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB0C.47D";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5380852":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB0C.47M";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5380779":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB0C.47H";
                    returnvalue.EmissionVariant = EmissionVariant.TUN_EU;
                    break;
                case "5380860":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB0C.47R";
                    returnvalue.Automatic_gearbox = true;
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5384748":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Z922C.47D";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5383294":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1YZA9C.47B";
                    returnvalue.EmissionVariant = EmissionVariant._190HP_EU;
                    break;
                case "55564013":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "ET07F02C.46U";
                    returnvalue.EmissionVariant = EmissionVariant.BIOPOWER_GB;
                    break;
                case "55565639":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EU06F01C.46V";
                    returnvalue.EmissionVariant = EmissionVariant.BIOPOWER_GB;
                    break;
                case "55562425":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "ET02U01C.46S";
                    returnvalue.EmissionVariant = EmissionVariant.BIOPOWER_SE;
                    break;
                case "5386792":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.47S";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5386941":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.471";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387618":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47I";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5388053":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.47O";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5388533":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46C";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5386800":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.477";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GB;
                    break;
                case "5386958":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.475";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GB;
                    break;
                case "5387634":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47Y";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GB;
                    break;
                case "5388541":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46A";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GB;
                    break;
                case "5386818":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.47T";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5386966":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.472";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387626":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47J";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388061":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.47P";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388558":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46D";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5386834":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.478";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_GB;
                    break;
                case "5386974":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.473";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5386982":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.476";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_GB;
                    break;
                case "5386990":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.474";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_RW;
                    break;
                case "5386826":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.47U";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5387782":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47K";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5388079":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.47N";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5388566":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46E";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_EU;
                    break;
                case "5387774":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47Z";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_GB;
                    break;
                case "5388574":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46B";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_GB;
                    break;
                case "5386842":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.47V";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_RW;
                    break;
                case "5387790":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.47L";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_RW;
                    break;
                case "5388582":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5OC.46F";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_TUN_RW;
                    break;
                case "55561754":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6CA.46I";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55564147":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46O";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55561759":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.46G";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_GB;
                    break;
                case "55564145":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46M";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_GB;
                    break;
                case "55560236":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.46J";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "55564148":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46P";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "55564150":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46R";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_RW;
                    break;
                case "55561756":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BYl6AC.46K";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_EU;
                    break;
                case "55564149":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46Q";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_EU;
                    break;
                case "55561761":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.46H";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_GB;
                    break;
                case "55564146":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.46N";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_GB;
                    break;
                case "55560248":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.46L";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_TUN_RW;
                    break;
                case "5383310":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1YZA9C.47M";
                    returnvalue.EmissionVariant = EmissionVariant.RU;
                    break;
                case "5383328":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1YZA9C.47H";
                    returnvalue.EmissionVariant = EmissionVariant.TUN_EU;
                    break;
                #endregion

                #region B235E

                case "5380803":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB1C.53F";
                    returnvalue.EmissionVariant = EmissionVariant._163HP_EU;
                    break;
                case "5381223":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS6UC.53I";
                    returnvalue.EmissionVariant = EmissionVariant._163HP_EU;
                    break;
                case "5380597":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXLBC.53L";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_163HK_EU;
                    break;
                case "5380589":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXJBC.53J";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5380381":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXZBC.53Z";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_M1_EU;
                    break;
                case "5380571":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXMBC.53M";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5380878":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB1C.531";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5381389":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB1C.53D";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5381298":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS6UC.53W";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5381322":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS6UC.53H";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5380563":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXKBC.53K";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5381421":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS6UC.53C";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5381355":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS6UC.53X";
                    returnvalue.EmissionVariant = EmissionVariant.M1_EU;
                    break;
                case "5380829":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "E924FB1C.53E";
                    returnvalue.EmissionVariant = EmissionVariant.OBD2_US;
                    break;
                case "5380811":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFB1C.53G";
                    returnvalue.EmissionVariant = EmissionVariant.ORVR_US;
                    break;
                case "5383336":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Y635C.53F";
                    returnvalue.EmissionVariant = EmissionVariant._163HP_EU;
                    break;
                case "5388301":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.53I";
                    returnvalue.EmissionVariant = EmissionVariant._163HP_EU;
                    break;
                case "55565640":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EU06Z44C.55P";
                    returnvalue.EmissionVariant = EmissionVariant.BIOPOWER_EU;
                    break;
                case "5388095":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34.53L";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_163HK_EU;
                    break;
                case "5386859":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.536";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387006":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.55B";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387642":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.53Q";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55559994":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.55F";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5386867":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.538";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GAS_EU;
                    break;
                case "5388608":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y50C.55H";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_GAS_EU;
                    break;
                case "5388111":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.53Z";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_M1_EU;
                    break;
                case "5386875":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.537";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387659":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.53R";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388129":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.53M";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55559996":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.55G";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388103":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.53J";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387014":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.55C";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55560237":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.55I";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55564151":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.55N";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55560238":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.55J";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "55564152":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.55O";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "5387022":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.55A";
                    returnvalue.EmissionVariant = EmissionVariant.GAS_EU;
                    break;
                case "5387808":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.533";
                    returnvalue.EmissionVariant = EmissionVariant.GAS_EU;
                    break;
                case "5388137":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.532";
                    returnvalue.EmissionVariant = EmissionVariant.GAS_EU;
                    break;
                case "5388319":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.53H";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5388327":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.53W";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_RU;
                    break;
                case "5388335":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.534";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_TUN_US;
                    break;
                case "5385570":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE078F1C.535";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387030":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.55D";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387667":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.53S";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388145":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.53K";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388343":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.53C";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55559995":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.55E";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55560239":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.55L";
                    returnvalue.EmissionVariant = EmissionVariant.LEV2BIN5_US;
                    break;
                case "5388350":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.53X";
                    returnvalue.EmissionVariant = EmissionVariant.M1_EU;
                    break;
                case "5383369":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "E92563C.53E";
                    returnvalue.EmissionVariant = EmissionVariant.OBD2_US;
                    break;
                case "5383377":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Y635C.53G";
                    returnvalue.EmissionVariant = EmissionVariant.ORVR_US;
                    break;
                case "5383351":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Y635C.531";
                    returnvalue.EmissionVariant = EmissionVariant.RU;
                    break;
                #endregion

                #region B235L
                case "5386883":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.57A";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55559999":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.57D";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5386891":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.57B";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55559998":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.57E";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55560240":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.57G";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55564157":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.57M";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55560241":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH09Y66C.57H";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "55564158":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.57N";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "5386909":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.57C";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388525":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.57J";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55559997":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.57F";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55561856":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.57I";
                    returnvalue.EmissionVariant = EmissionVariant.LEV2BIN5_US;
                    break;
                case "55564156":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y01C.57L";
                    returnvalue.EmissionVariant = EmissionVariant.LEV2BIN5_US;
                    break;
                #endregion

                #region B235R

                case "5380555":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXEBC.56E";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5380399":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXMBC.56M";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_M1_EU;
                    break;
                case "5380548":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXGBC.56G";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5381405":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS77C.56D";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5381413":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS77C.56K";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5380530":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PXFBC.56F";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5381397":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS77C.56C";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5381272":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS77C.56L";
                    returnvalue.EmissionVariant = EmissionVariant.M1_EU;
                    break;
                case "5386917":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.56O";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387048":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.56Q";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387675":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.56H";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387709":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.56T";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5388152":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.56E";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55560000":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.56W";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55563630":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EY01C.581";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55563631":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EY01C.582";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55563632":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CY01C.583";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55563633":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CY01C.584";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "55563634":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y01C.585";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5388160":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.56M";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_M1_EU;
                    break;
                case "5386925":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.56P";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387055":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.56R";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387683":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.56I";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387717":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.56U";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388178":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.56G";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55560001":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.56Z";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "55561854":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.567";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55564155":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y02C.56X";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_EU;
                    break;
                case "55561853":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.566";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;           
                    break;
                case "55564154":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y02C.569";
                    returnvalue.EmissionVariant = EmissionVariant.EUR4_RW;
                    break;
                case "5388368":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.56D";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5388376":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.56K";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_RU;
                    break;
                case "5386933":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0C8P1C.56N";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387063":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0CZ1AC.56S";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387691":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.56J";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388186":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.56F";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388384":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.56C";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55560002":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EG03Y5TC.56V";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "55561852":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EH0BY6AC.565";
                    returnvalue.EmissionVariant = EmissionVariant.LEV2BIN5_US;
                    break;
                case "55564153":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EI02Y02C.568";
                    returnvalue.EmissionVariant = EmissionVariant.LEV2BIN5_US;
                    break;
                case "5388392":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.56L";
                    returnvalue.EmissionVariant = EmissionVariant.M1_EU;
                    break;
                #endregion

                #region B308E

                case "5380944":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1HC.CBF";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5380951":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1IC.CBG";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5380787":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFAKC.CBA";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5381363":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS66C.CBE";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_EU;
                    break;
                case "5380969":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0PP1GC.CBH";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5381256":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB2VS66C.CBC";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5380837":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "E924FAKC.CBB";
                    returnvalue.EmissionVariant = EmissionVariant.OBD2_US;
                    break;
                case "5380795":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1XFAKC.CBD";
                    returnvalue.EmissionVariant = EmissionVariant.ORVR_US;
                    break;
                case "5387071":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0D8Q1C.CBO";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387725":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.CBI";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387758":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.CBR";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5388194":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.CBF";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_EU;
                    break;
                case "5387089":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0D8Q1C.CBP";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387733":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.CBJ";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5387766":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.CBS";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5388202":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.CBG";
                    returnvalue.EmissionVariant = EmissionVariant.EC2000_RW;
                    break;
                case "5383385":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Y635C.CBA";
                    returnvalue.EmissionVariant = EmissionVariant.EU;
                    break;
                case "5388400":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.CBE";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387097":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0D8Q1C.CBQ";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5387741":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EE0EE02C.CBK";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388210":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EC0YS34C.CBH";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5388418":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EB31SABC.CBC";
                    returnvalue.EmissionVariant = EmissionVariant.LEV_US;
                    break;
                case "5383393":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "E925635C.CBB";
                    returnvalue.EmissionVariant = EmissionVariant.OBD2_US;
                    break;
                case "5383401":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "EA1Y635C.CBD";
                    returnvalue.EmissionVariant = EmissionVariant.ORVR_US;
                    break;
                #endregion

                #endregion

                #region SAAB93

                #region B205E

                case "5381843":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0RXDBC.48D";
                    break;
                case "5380449":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PXDAC.48D";
                    break;
                case "5380431":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PXEAC.48E";
                    break;
                case "5387923":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48D";
                    break;
                case "5388459":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48H";
                    break;
                case "5387949":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48F";
                    break;
                case "5388467":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48I";
                    break;
                case "5387956":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48G";
                    break;
                case "5387931":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.48E";
                    break;
                #endregion

                #region B205L

                case "5381603":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0RB51C.45G";
                    break;


                case "5380423":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4BC.45H";
                    break;
                case "5380415":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4CC.45I";
                    break;
                case "5381280":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS18C.45B";
                    break;
                case "5381348":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS18C.45F";
                    break;
                case "5381330":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS18C.45C";
                    break;
                case "5387964":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.45H";
                    break;
                case "5387972":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "5387972.45I";
                    break;
                case "5388236":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.45F";
                    break;
                case "5388228":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.45B";
                    break;
                case "5387980":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.45G";
                    break;
                case "5388426":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.45J";
                    break;
                case "8358244":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.45C";
                    break;
                case "5169446":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2RV13C.45C";
                    break;


                #endregion

                #region B205R

                case "5380480":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4DC.50A";
                    break;
                case "5380472":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4FC.50E";
                    break;
                case "5381231":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS94C.50B";
                    break;
                case "5381264":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS94C.50F";
                    break;
                case "5380464":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4EC.50D";
                    break;
                case "5381371":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS94C.50C";
                    break;
                case "5387998":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.50A";
                    break;
                case "5388004":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.50E";
                    break;
                case "5388251":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.50B";
                    break;
                case "5388269":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB13SABC.50F";
                    break;
                case "5381504":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "ED0KC2TC.50H";
                    break;
                case "5388012":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.50D";
                    break;
                case "5388277":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.50C";
                    break;
                case "5388434":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.50G";
                    break;
                #endregion

                #region B235R
                case "5381207":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EA1XFB9C.41A";
                    break;
                case "5381314":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS6SC.41D";
                    break;
                case "5380456":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0PB4GC.41F";
                    break;
                case "5381306":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB2VS6SC.41C";
                    break;
                case "5381215":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EA1XFB9C.41B";
                    break;

                case "6159677":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0NB3MC.41E";
                    break;
                case "6159693":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0NB3OC.41G";
                    break;
                case "5383278":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EA1YY3SC.41A";
                    break;
                case "5388285":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.41D";
                    break;
                case "5388046":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.41F";
                    break;
                case "5388292":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EB31SABC.41C";
                    break;
                case "5388442":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EC0YS3SC.41H";
                    break;
                case "5383286":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "EA1YY3SC.41B";
                    break;
                #endregion

                #endregion

                #region Unknown
                case "5383930":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383617":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383625":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383955":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383633":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383641":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383658":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5381587":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5169719":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383666":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5381595":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383427":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383419":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388244":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383922":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383435":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383674":
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383682":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383690":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383450":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383443":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383948":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383468":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383708":
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383476":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388293":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383914":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383484":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383732":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569107":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55566754":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55566757":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55565638":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55564143":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55564014":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563620":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563379":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55562537":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55562028":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55561307":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569108":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55566755":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55566758":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55564144":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563621":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563378":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563250":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387477":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384946":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386487":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386057":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386214":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385554":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385356":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385190":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384128":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384326":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383740":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387584":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384961":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386503":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385968":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386131":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385661":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385372":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384144":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384342":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383765":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387469":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384920":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386479":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386073":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386230":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385539":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385398":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385232":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384169":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384300":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387576":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384938":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386495":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385984":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386248":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385547":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385406":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384177":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384318":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387485":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384953":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386461":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386040":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386156":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385562":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385364":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385208":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384136":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384334":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383757":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388087":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387592":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384979":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386511":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385976":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386123":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385679":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385380":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384151":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384359":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383773":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383302":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569098":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569100":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563142":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563144":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569099":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569111":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55567768":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563140":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563141":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55567767":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569101":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563145":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569102":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563143":
                    returnvalue.Enginetype = EngineType.B205E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383492":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569109":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55566756":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55564758":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55564142":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563622":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383781":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559497":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388590":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387493":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384995":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386420":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386065":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385588":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385422":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385265":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384193":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385992":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386149":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385414":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384185":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384367":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383807":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386172":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384375":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383799":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387600":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385018":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386529":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383203":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383815":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559499":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388616":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387501":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385000":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386412":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386081":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386206":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385596":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385430":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385273":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384201":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384383":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383823":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384755":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383344":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569103":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563146":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569104":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563147":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383518":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383500":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383526":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559498":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388624":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384987":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386016":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386164":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385448":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385281":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385109":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384219":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384391":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383534":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383831":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383245":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383542":
                    returnvalue.Enginetype = EngineType.B235E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559502":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388632":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387543":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386651":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386552":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559501":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388640":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387550":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386669":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386586":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569115":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563148":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569114":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563149":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559500":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388657":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387568":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386677":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386610":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569113":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563150":
                    returnvalue.Enginetype = EngineType.B235L;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383849":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559503":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388665":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387527":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385034":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386446":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386107":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385752":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385612":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385455":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385299":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384227":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386313":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385695":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384409":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386271":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385711":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384540":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383856":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559504":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388673":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387535":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385042":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386453":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386099":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385745":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385620":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385463":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385307":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384235":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383864":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386297":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385687":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384417":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386289":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385729":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384557":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569106":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563151":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55560243":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569105":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563152":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383559":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383567":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55559505":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5388681":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5387519":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385026":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386438":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386115":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385737":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385604":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385471":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385315":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385083":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384243":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383575":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383872":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386305":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385703":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384425":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383252":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55570653":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569110":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55569112":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55567334":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55565942":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "55563153":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383583":
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386032":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385489":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385323":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384250":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383880":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386222":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384433":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386263":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384565":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386024":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385497":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385331":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384268":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383898":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386180":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384441":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386255":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384573":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383591":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386008":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385505":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385349":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5385091":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384276":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383609":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383906":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5386198":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5384458":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5383260":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5165113":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "4571725":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;
                case "5166368":
                    returnvalue.Enginetype = EngineType.B308E;
                    returnvalue.Carmodel = CarModel.Saab95;
                    returnvalue.Softwareversion = "";
                    break;

                #endregion
            }

            if (returnvalue.Carmodel == CarModel.Saab93)
            {

                switch (returnvalue.Enginetype)
                {
                    case EngineType.B205:
                    case EngineType.B205E:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 980
                        returnvalue.Stage1torque = 280;
                        returnvalue.Valid = true;
                        // stock = 240 Nm
                        break;
                    case EngineType.B205L:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 980
                        returnvalue.Stage1torque = 300;
                        returnvalue.Valid = true;
                        // stock = 263 Nm
                        break;
                    case EngineType.B205R:
                        returnvalue.Stage1airmass = 1200; // stock = ??? 970
                        returnvalue.Stage1torque = 320;
                        returnvalue.Valid = true;
                        // stock = 280 Nm
                        break;
                    case EngineType.B235:
                    case EngineType.B235E:
                        break;
                    case EngineType.B235L:
                        break;
                    case EngineType.B235R:
                        returnvalue.Stage1airmass = 1400; // stock = ??? 1200
                        returnvalue.Stage1torque = 410;
                        returnvalue.Valid = true;
                        // stock = 350 Nm
                        break;
                
                }
            }
            else if (returnvalue.Carmodel == CarModel.Saab95)
            {
                switch (returnvalue.Enginetype)
                {
                    case EngineType.B205:
                    case EngineType.B205E:
                        returnvalue.Stage1airmass = 1200; 
                        returnvalue.Stage1torque = 280;
                        returnvalue.Valid = true;
                        // stock = 1030 mg/c
                        // stock = 240 Nm
                        break;
                    case EngineType.B205L:
                        break;
                    case EngineType.B205R:
                        break;
                    case EngineType.B235:
                    case EngineType.B235E:
                        returnvalue.Stage1airmass = 1300;
                        returnvalue.Stage1torque = 330;
                        returnvalue.Valid = true;
                        // stock = 1126 mg/c
                        // stock = 287 Nm
                        break;
                    case EngineType.B235L:
                        returnvalue.Stage1airmass = 1450;
                        returnvalue.Stage1torque = 350;
                        returnvalue.Valid = true;
                        // stock = 1300 mg/c
                        // stock = 310 Nm
                        break;
                    case EngineType.B235R:
                        returnvalue.Stage1airmass = 1450;
                        returnvalue.Stage1torque = 390;
                        returnvalue.Valid = true;
                        // stock = 1300 mg/c
                        // stock = 350 Nm
                        break;
                    case EngineType.B308E:
                        returnvalue.Stage1airmass = 1000;
                        returnvalue.Stage1torque = 350;
                        returnvalue.Valid = true;
                        // stock = 800 mg/c
                        // stock = 310 Nm
                        break;
                }
            }
            if (returnvalue.Enginetype == EngineType.B204 || returnvalue.Enginetype == EngineType.B204E || returnvalue.Enginetype == EngineType.B204L || returnvalue.Enginetype == EngineType.B204R || returnvalue.Enginetype == EngineType.B205 || returnvalue.Enginetype == EngineType.B205E || returnvalue.Enginetype == EngineType.B205L || returnvalue.Enginetype == EngineType.B205R)
            {
                returnvalue.Is2point3liter = false;
            }
            else returnvalue.Is2point3liter = true;
            if (returnvalue.Enginetype == EngineType.B204R || returnvalue.Enginetype == EngineType.B205R || returnvalue.Enginetype == EngineType.B234R || returnvalue.Enginetype == EngineType.B235R)
            {
                returnvalue.Isaero = true;
                returnvalue.Isfpt = true;
            }
            else if (returnvalue.Enginetype == EngineType.B204L || returnvalue.Enginetype == EngineType.B205L || returnvalue.Enginetype == EngineType.B234L || returnvalue.Enginetype == EngineType.B235L)
            {
                returnvalue.Isfpt = true;
            }
            returnvalue.Isturbo = true;

            return returnvalue;
        }
    }

    enum CarModel : int
    {
        Unknown = 0,
        Saab93 = 1,
        Saab95 = 2
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
        B308E
    }

    enum TurboModel : int
    {
        None,
        GarretT25,
        GarretTB2529,
        GarretTB2531,
        MitsuTD04
    }

    enum EmissionVariant : int
    {
        None,
        BIOPOWER_GB,
        BIOPOWER_SE,
        EUR4_EU,
        EUR4_RW,
        EUR4_GB,
        EUR4_TUN_RW,
        EUR4_TUN_EU,
        EUR4_TUN_GB,
        LEV2BIN5_US,
        EC2000_EU,
        EC2000_RW,
        EC2000_M1_EU,
        EC2000_GB,
        EC2000_TUN_GB,
        EC2000_TUN_EU,
        EC2000_TUN_RW,
        M1_EU,
        LEV_US,
        LEV_RU,
        LEV_EU,
        EU,
        TUN_EU,
        _190HP_EU,
        RU,
        _163HP_EU,
        EC2000_163HK_EU,
        OBD2_US,
        ORVR_US,
        BIOPOWER_EU,
        EC2000_GAS_EU,
        GAS_EU,
        LEV_TUN_US
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

        private int _tunedbyt7stostage = 0;

        public int Tunedbyt7stostage
        {
            get { return _tunedbyt7stostage; }
            set { _tunedbyt7stostage = value; }
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

        private EmissionVariant _emissionVariant = EmissionVariant.None;

        internal EmissionVariant EmissionVariant
        {
            get { return _emissionVariant; }
            set { _emissionVariant = value; }
        }
    }
}
