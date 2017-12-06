using System;
using System.Collections.Generic;
using System.Text;



/***
T5.2 is only found on MY1993 Saab 9000 2,3T (B234L and B234R)
Saab part no:
9136474 = B234L
9136490 = B234L, TCS, ASR 
4300810 = B234R
9136516 = B234R TCS

 * **/
namespace Trionic5Tools
{
    public class PartNumberConverter
    {
        public PartNumberConverter()
        {

        }

        public ECUInformation GetECUInfo(string partnumber, string enginetype)
        {
            //4903 = for high altitude use!
            ECUInformation returnvalue = new ECUInformation();
            returnvalue.Tunedbyt5stostage = 0;
            returnvalue.Ecutype = "T5.5";
            if (enginetype.EndsWith("T5S1")) returnvalue.Tunedbyt5stostage = 1;
            else if (enginetype.EndsWith("T5S2")) returnvalue.Tunedbyt5stostage = 2;
            else if (enginetype.EndsWith("T5S3")) returnvalue.Tunedbyt5stostage = 3;
            switch (partnumber)
            {
                #region partnumbers 9000

                #region 9000 B204E

                case "4660833":
                    returnvalue.SoftwareID = "A5EZK6BL.17A";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2529;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 215;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4660841": // B204E LPT 1997
                    returnvalue.SoftwareID = "A5EZV1JL.17B";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2529;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 215;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301933": // B204S 2.0 LPT
                    returnvalue.SoftwareID = "A53!V04L.12A";
                    returnvalue.Enginetype = EngineType.B204S;
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Stage1boost = 1.05;
                    returnvalue.Stage2boost = 1.15;
                    returnvalue.Stage3boost = 1.25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.59;
                    returnvalue.Max_stock_boost_automatic = 0.59;
                    returnvalue.Torque = 210;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300331": // B204S 1994
                    returnvalue.SoftwareID = "A53OF4LL.12A";
                    returnvalue.Enginetype = EngineType.B204S;
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Stage1boost = 1.05;
                    returnvalue.Stage2boost = 1.15;
                    returnvalue.Stage3boost = 1.25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.59;
                    returnvalue.Max_stock_boost_automatic = 0.59;
                    returnvalue.Torque = 210;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4661146": // B204S auto 1995
                    returnvalue.SoftwareID = "A53!K5DL.12B";
                    returnvalue.Enginetype = EngineType.B204S;
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Stage1boost = 1.05;
                    returnvalue.Stage2boost = 1.15;
                    returnvalue.Stage3boost = 1.25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.59;
                    returnvalue.Max_stock_boost_automatic = 0.59;
                    returnvalue.Torque = 210;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #region 9000 B204L
                case "4300844": // B204L 1994
                    returnvalue.SoftwareID = "A53OF4OL.14C";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.72;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301941": // B204L 1995-1998
                    returnvalue.SoftwareID = "A5DOK5VL.14C";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.72;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4661260": // B204L VSS 1996-1998
                    returnvalue.SoftwareID = "A5EOK6DL.14E";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.72;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #region 9000 B234R

                #region T5.2
                case "4300810": // T5.2 B234R
                    returnvalue.Ecutype = "T5.2";
                    returnvalue.SoftwareID = "A45LT21M.36A";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.02;
                    returnvalue.Max_stock_boost_automatic = 1.02;
                    returnvalue.Torque = 350;
                    returnvalue.MakeYearFrom = 1993;
                    returnvalue.MakeYearUpto = 1993;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "9136516": // T5.2 91 36 516 88 28 196 1993 9000 B234R/TCS 
                    returnvalue.Ecutype = "T5.2";
                    returnvalue.SoftwareID = "A45LT22M.36B";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.02;
                    returnvalue.Max_stock_boost_automatic = 1.02;
                    returnvalue.Torque = 350;
                    returnvalue.MakeYearFrom = 1993;
                    returnvalue.MakeYearUpto = 1993;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #region T5.5
                case "4780268": //47 80 268 47 80 268 1997 9000 B234R 
                    returnvalue.SoftwareID = "A54QL56L.36M";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4903936": //49 03 936 49 03 936 1996-1998 9000 B234R
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;
                case "4611752": //46 11 752 47 81 894 1998 9000 B234R 
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781894": // B234R 1997-1998
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301206": // B234R 1996-1997
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300422": //43 00 422 43 00 422 1996 9000 B234R 
                    returnvalue.SoftwareID = "A54ML43L.36F";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781886":
                    returnvalue.SoftwareID = "A54UK9AL.36O";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08; //0.91; ??? too low for B234R?
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300414": //43 00 414 43 00 414 1994 9000 B234R/TCS 
                    returnvalue.SoftwareID = "A53OF7XL.36C";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08;
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4903928": //49 03 928 49 03 928 1994 1995 9000 B234R
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08;
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;
                case "4660338": //46 60 338 46 60 338 1995 9000 B234R (No TCS) 
                    returnvalue.SoftwareID = "A53OP3IL.36E";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08;
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301974": //43 01 974 43 01 974 1995 9000 B234R/TCS Aero M/T 
                    returnvalue.SoftwareID = "A5COP48L.36C";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08;
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302998":
                    returnvalue.SoftwareID = "A5EZK75L.36L";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.08;
                    returnvalue.Max_stock_boost_automatic = 0.91;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300851": //43 00 851 43 00 851 1994 9000 B234R (No TCS) 
                    returnvalue.SoftwareID = "A53KB6GC.36E";
                    returnvalue.Enginetype = EngineType.B234R;
                    returnvalue.Isaero = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 225;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.MitsuTD04;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1;
                    returnvalue.Max_stock_boost_automatic = 1;
                    returnvalue.Torque = 342;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #endregion

                #region 9000 B234E
                case "4780243": //47 80 243 47 80 243 1997 9000 B234E 
                    returnvalue.SoftwareID = "A54QL54L.33G";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;

                case "4903894": //49 03 894 49 03 894 1996-1998 9000 B234E
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;
                case "4300364": //43 00 364 43 00 364 1996 9000 B234E 
                    returnvalue.SoftwareID = "A54KL36L.33B";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301909": //43 01 909 43 01 909 1995 9000 B234E 
                    returnvalue.SoftwareID = "A5COP46L.33A";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4903886": //49 03 886 49 03 886 1994-1995 9000 B234E
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;

                case "4781845":
                    returnvalue.SoftwareID = "A5BUK96L.33L";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;

                case "4300877":
                    returnvalue.SoftwareID = "A53OT0FL.33A";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302972":
                    returnvalue.SoftwareID = "A5EZK73L.33F";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781456": // B234E 1997
                    returnvalue.SoftwareID = "A5EZP5JL.33H";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781464": // B234E VSS 1997
                    returnvalue.SoftwareID = "A5IZP5JL.33J";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781852": // B234E 1997-1998
                    returnvalue.SoftwareID = "A54UK95L.33K";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302303": // B234E 1996
                    returnvalue.SoftwareID = "A5DZK7AL.33C";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 170;
                    returnvalue.Enginetype = EngineType.B234E;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.4;
                    returnvalue.Torque = 260;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;

                #endregion

                #region 9000 B234L

                #region T5.2
                case "9136474": // T5.2 ECU B234L 1993
                    returnvalue.Ecutype = "T5.2";
                    returnvalue.SoftwareID = "A45XT2CM.35E";
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 200;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.94;
                    returnvalue.Max_stock_boost_automatic = 0.78;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1993;
                    returnvalue.MakeYearUpto = 1993;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "9136490": // T5.2 B234L, TCS, ASR 
                    returnvalue.Ecutype = "T5.2";
                    returnvalue.SoftwareID = "A45JT1QM.35F";
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 200;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.45;
                    returnvalue.Max_stock_boost_manual = 1.02;
                    returnvalue.Max_stock_boost_automatic = 1.02;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1993;
                    returnvalue.MakeYearUpto = 1993;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;

                #endregion

                #region T5.5

                case "4300828": //43 00 828 43 00 828 1994 9000 B234L 
                    returnvalue.SoftwareID = "A53KB6EL.35G";
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 1.00;
                    returnvalue.Max_stock_boost_automatic = 0.81;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4903902": //49 03 902 49 03 902 1994-1995 9000 B234L 
                    returnvalue.SoftwareID = "A5FO903L.35G";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 1.00;
                    returnvalue.Max_stock_boost_automatic = 0.81;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;
                case "4301917": //43 01 917 43 01 917 1995 9000 B234L & Aero A/T 
                    returnvalue.SoftwareID = "A53OF8ZL.35G";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 1.00;
                    returnvalue.Max_stock_boost_automatic = 0.81;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4611737": //46 11 737 47 81 878 1998 9000 B234L 
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 1.00;
                    returnvalue.Max_stock_boost_automatic = 0.81;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4903910": //49 03 910 49 03 910 1996-1998 9000 B234L
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 1.00;
                    returnvalue.Max_stock_boost_automatic = 0.81;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781860": // B234L VSS 1997-1998
                    returnvalue.SoftwareID = "A54UK98L.35S";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302329": // B234L 1996-1997
                    returnvalue.SoftwareID = "A5EZK71L.35L";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4303910": // B234L HIGH ALTITUDE 1996-1998
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US";
                    returnvalue.HighAltitude = true;
                    break;
                case "4781878": // B234L 1997-1998
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300836": //43 00 836 43 00 836 1996 9000 B234L/R A/T 
                    returnvalue.SoftwareID = "A54KL37L.35I";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4780250": //47 80 250 47 80 250 1997 9000 B234L 
                    returnvalue.SoftwareID = "A5AQL56L.35P";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302980":
                    returnvalue.SoftwareID = "A5IZV1LL.35O";
                    returnvalue.Isaero = false;
                    returnvalue.Isfpt = true;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Bhp = 200;
                    returnvalue.Enginetype = EngineType.B234L;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretTB2531;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_manual = 0.9;
                    returnvalue.Max_stock_boost_automatic = 0.7;
                    returnvalue.Torque = 323;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #endregion

                #region 9000 BXXX
                case "4302642":
                    returnvalue.Enginetype = EngineType.B204I; // i, non turbo
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 130;
                    returnvalue.Torque = 177;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;

                case "4301891":
                    returnvalue.SoftwareID = "A5CPK5NL.30H";
                    returnvalue.Enginetype = EngineType.B204I; // i, non turbo
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 130;
                    returnvalue.Torque = 177;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4300349":
                    returnvalue.SoftwareID = "A53PW8XL.10A";
                    returnvalue.Enginetype = EngineType.B204I; // i, non turbo
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 130;
                    returnvalue.Torque = 177;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301925": // B204i 1995
                    returnvalue.SoftwareID = "A5EPK65L.10A";
                    // NON TURBO CAR
                    returnvalue.Enginetype = EngineType.B204I; // i, non turbo
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 130;
                    returnvalue.Torque = 177;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "07B95":
                    // hirsch tuned file, invalid for tuning
                    break;
                //case "4301891":
                case "4300356":
                    returnvalue.SoftwareID = "A53PW8VL.30H";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Torque = 212;
                    returnvalue.Enginetype = EngineType.B234I;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301313":
                    returnvalue.SoftwareID = "A53OW8QL.30K";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Torque = 212;
                    returnvalue.Enginetype = EngineType.B234I;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4301966":
                    //43 00 356 43 00 356 1994 9000 B234I 
                    returnvalue.SoftwareID = "A53PW8YL.30K";
                    returnvalue.Isaero = false;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Isturbo = false;
                    returnvalue.Valid = true;
                    returnvalue.Bhp = 150;
                    returnvalue.Torque = 212;
                    returnvalue.Enginetype = EngineType.B234I;
                    returnvalue.Carmodel = CarModel.Saab9000;
                    returnvalue.Turbomodel = TurboModel.None;
                    returnvalue.Stage1boost = 0;
                    returnvalue.Stage2boost = 0;
                    returnvalue.Stage3boost = 0;
                    returnvalue.Baseboost = 0;
                    returnvalue.Max_stock_boost_manual = 0;
                    returnvalue.Max_stock_boost_automatic = 0;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1994;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #endregion

                #region partnumbers 93

                #region 93 B204E

                case "4782546":
                    returnvalue.SoftwareID = "A5DUX24L.17C";
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5165212":
                    returnvalue.SoftwareID = "A554Y26L.17D";
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "Sweden";
                    returnvalue.HighAltitude = false;
                    break;
                case "5165246":
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "Sweden";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171434":
                    returnvalue.SoftwareID = "A5EU90UL.17C";
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171459":
                    returnvalue.SoftwareID = "A55790VL.17D";
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 2000;
                    returnvalue.MakeYearUpto = 2000;
                    returnvalue.Region = "Sweden";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171475":
                    returnvalue.Bhp = 150;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204E;
                    returnvalue.Isfpt = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.45;
                    returnvalue.Torque = 219;
                    returnvalue.MakeYearFrom = 2000;
                    returnvalue.MakeYearUpto = 2000;
                    returnvalue.Region = "Sweden";
                    returnvalue.HighAltitude = false;
                    break;

                #endregion

                #region 93 B204L
                case "4782280": //51 69 982	51 69 982	2002	9-3 B205L
                    returnvalue.SoftwareID = "A553L60L.15R";
                    returnvalue.Isaero = false;
                    //returnvalue.Isviggen = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171673": // 9-3 B204L A/T
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 2000;
                    returnvalue.MakeYearUpto = 2000;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4611935": //9-3 B204L A/T
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782298": // 9-3 B204L A/T
                    returnvalue.SoftwareID = "A553L61L.15S";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782512":
                    returnvalue.SoftwareID = "A554X24L.15S";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782314":
                    returnvalue.SoftwareID = "A5CUK92L.15Z";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782504":
                    returnvalue.SoftwareID = "A5DUX24L.15Z";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171160":
                    returnvalue.SoftwareID = "A55790QL.15S";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171418":
                    returnvalue.SoftwareID = "A5EU90TL.15Z";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171848":
                    returnvalue.SoftwareID = "A558913L.15S";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 230;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782496": //9-3 B204L M/T
                    returnvalue.SoftwareID = "A554X24L.15R";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782306":// 9-3 B204L M/T
                    returnvalue.SoftwareID = "A5CUK94L.15X";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4611919":// 9-3 B204L M/T
                case "4782488":
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171152":
                    returnvalue.SoftwareID = "A55790PL.15R";
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171392":
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171822":
                    returnvalue.Isaero = false;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.75;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Torque = 263;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;

                #endregion

                #region 93 B204R
                case "5171699":
                    returnvalue.SoftwareID = "A557912L.18B";
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4780656": // 9-3 B204R M/T Stock = 1 bar boost pressure
                    returnvalue.SoftwareID = "A554X24L.18B";
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171178":
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781035": // 9-3 B204R M/T
                    returnvalue.SoftwareID = "A554X24L.18C";
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171491": // 9-3 B204R M/T
                    returnvalue.SoftwareID = "A55790YL.18C";
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5171863":
                    returnvalue.Isaero = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Enginetype = EngineType.B204R;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.Max_stock_boost_automatic = 0.95;
                    returnvalue.Max_stock_boost_manual = 0.95;
                    returnvalue.Torque = 283;
                    returnvalue.MakeYearFrom = 1999;
                    returnvalue.MakeYearUpto = 1999;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion
/*
                #region 93 B205L

                case "4571907": //45 71 907	53 81 108	2000	9-3 B205L
                case "5169883": //51 69 883	51 69 883	2001	9-3 B205L
                case "5169982": //51 69 982	51 69 982	2002	9-3 B205L
                    returnvalue.Isaero = false;
                    //returnvalue.Isviggen = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    break;
                case "4782280": //51 69 982	51 69 982	2002	9-3 B205L
                    returnvalue.SoftwareID = "A553L60L.15R";
                    returnvalue.Isaero = false;
                    //returnvalue.Isviggen = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Enginetype = EngineType.B205L;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 185;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    break;

                #endregion

                #region 93 B205R

                case "4571915": //45 71 915	53 81 140	2000	9-3 B205R
                case "5166822": //51 66 822	51 66 822	2001	9-3 B205R
                case "5169990": //51 69 990	51 69 990	2002	9-3 B205R
                    returnvalue.Isaero = false;
                    //returnvalue.Isviggen = true;
                    returnvalue.Enginetype = EngineType.B205R;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = true;
                    returnvalue.Bhp = 200;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    break;

                #endregion

                #region 93 B235R

                case "4782538": //9-3 B235R Viggen
                case "5166731": //9-3 B235R Viggen
                case "4571923":  //45 71 923	53 81 074	2000	9-3 B235R Viggen
                case "5166855":  //51 66 855	51 66 855	2001	9-3 B235R
                case "5169974":  //51 69 974	51 69 974	2002	9-3 B235R
                    returnvalue.Isaero = true;
                    //returnvalue.Isviggen = true;
                    returnvalue.Enginetype = EngineType.B235R;
                    returnvalue.Isturbo = true;
                    returnvalue.Carmodel = CarModel.Saab93;
                    returnvalue.Is2point3liter = true;
                    returnvalue.Valid = true;
                    returnvalue.Isfpt = false;
                    returnvalue.Bhp = 225;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    break;
                #endregion
*/
                #endregion

                #region partnumbers 900SE

                #region 900SE B204E

                #endregion

                #region 900SE B204L AUT
                case "4781050": //	1998	B204L A/T
                    returnvalue.SoftwareID = "A552L5AL.15S";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;

                case "4239919": //42 39 919	47 80 284	1996	B204L A/T
                    returnvalue.SoftwareID = "A54ML3FL.15I";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4780284": //47 80 284	47 80 284	1997	B204L A/T
                    returnvalue.SoftwareID = "A5BUL53L.15P";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4662177": // 1996 900NG 2.0T B204 A/T
                    // stock = 0.8 bar boost
                    returnvalue.SoftwareID = "A5DZK60L.15G";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781787":
                    returnvalue.SoftwareID = "A5BUK92L.15Z";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781795":
                    // stock = 0.8 bar boost
                    returnvalue.SoftwareID = "A54UK91L.15Y";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4782660": //47 82 660	51 71 848	1998	B204L A/T
                    // stock = 0.8 bar boost
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4239810":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4239828":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302733":
                    returnvalue.SoftwareID = "A5EZK6FL.15K";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4662185":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781209":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781225":
                    // stock = 0.8 bar boost
                    returnvalue.SoftwareID = "A5EZK7FL.15K";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Automatic_gearbox = true; // watch out with tuning algorithm... 
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.45;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #region 900SE B204L Manual
                case "9132671": // 91 32 671	91 32 671	1994	B204L M/T
                    returnvalue.SoftwareID = "A53UF7UL.15A";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.75;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4445318": //44 45 318	44 45 318	1995	B204L M/T
                    returnvalue.SoftwareID = "A5CZK5GL.15A";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "9132689": //91 32 689 1)	91 32 689	1996	B204L M/T
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4662565": //46 62 565 2)	47 80 276	1996	B204L M/T
                    returnvalue.SoftwareID = "A54ML40L.15L";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4780276": //47 80 276	47 80 276	1997	B204L M/T
                    returnvalue.SoftwareID = "A5AQL54L.15O";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781779": //47 81 779	47 81 779	1998	B204L M/T
                    returnvalue.SoftwareID = "A5AUK93L.15V";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4781761": // B204L manual
                    returnvalue.SoftwareID = "A5AUK94L.15X";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1997;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "4239273":
                    returnvalue.SoftwareID = "A53UP22L.15E";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4239281":
                    returnvalue.SoftwareID = "A5DZK5TL.15F";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1995;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = false;
                    break;
                case "4302725":
                    returnvalue.SoftwareID = "A5EZK6EL.15J";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = false;
                    break;
                case "5170576":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1996;
                    returnvalue.Region = "All, except US";
                    returnvalue.HighAltitude = true;
                    break;
                case "5170790":
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1994;
                    returnvalue.MakeYearUpto = 1995;
                    returnvalue.Region = "All";
                    returnvalue.HighAltitude = true;
                    break;
                case "5174412":
                    // stock = 0.72 bar boost
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1998;
                    returnvalue.MakeYearUpto = 1998;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false; 
                    break;
                case "5171657":
                    // stock = 0.72 bar boost
                    returnvalue.SoftwareID = "A557911L.15R";
                    returnvalue.Bhp = 185;
                    returnvalue.Carmodel = CarModel.Saab900SE;
                    returnvalue.Is2point3liter = false;
                    returnvalue.Isaero = false;
                    returnvalue.Enginetype = EngineType.B204L;
                    returnvalue.Isfpt = true;
                    returnvalue.Isturbo = true;
                    returnvalue.Valid = true;
                    returnvalue.Stage1boost = 1.15;
                    returnvalue.Stage2boost = 1.25;
                    returnvalue.Stage3boost = 1.35;
                    returnvalue.Turbomodel = TurboModel.GarretT25;
                    returnvalue.Torque = 263;
                    returnvalue.Max_stock_boost_automatic = 0.62;
                    returnvalue.Max_stock_boost_manual = 0.72;
                    returnvalue.Baseboost = 0.4;
                    returnvalue.MakeYearFrom = 1996;
                    returnvalue.MakeYearUpto = 1997;
                    returnvalue.Region = "US, CA";
                    returnvalue.HighAltitude = false;
                    break;
                #endregion

                #endregion

            }
            return returnvalue;
        }
    }

    public enum CarModel : int
    {
        Unknown = 0,
        Saab900 = 1,
        Saab9000 = 2,
        Saab900SE = 3,
        Saab93 = 4,
        Saab95 = 5
    }

    public enum EngineType : int
    {
        Unknown,
        B204I,
        B204E,
        B204L,
        B204R,
        B204S,
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
        B235R
    }

    enum TurboModel : int
    {
        None,
        GarretT25,
        GarretTB2529,
        GarretTB2531,
        MitsuTD04
    }

    public class ECUInformation
    {
        double _stage1boost = 1.15;

        public double Stage1boost
        {
            get { return _stage1boost; }
            set { _stage1boost = value; }
        }
        double _stage2boost = 1.25;

        public double Stage2boost
        {
            get { return _stage2boost; }
            set { _stage2boost = value; }
        }
        double _stage3boost = 1.35;

        public double Stage3boost
        {
            get { return _stage3boost; }
            set { _stage3boost = value; }
        }

        private EngineType _enginetype = EngineType.Unknown;

        public EngineType Enginetype
        {
            get { return _enginetype; }
            set { _enginetype = value; }
        }

        private CarModel _carmodel = CarModel.Unknown;

        public CarModel Carmodel
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

        private int _tunedbyt5stostage = 0;

        public int Tunedbyt5stostage
        {
            get { return _tunedbyt5stostage; }
            set { _tunedbyt5stostage = value; }
        }

        private bool _automatic_gearbox = false;

        public bool Automatic_gearbox
        {
            get { return _automatic_gearbox; }
            set { _automatic_gearbox = value; }
        }

        private string _ecutype = "T5.5";

        public string Ecutype
        {
            get { return _ecutype; }
            set { _ecutype = value; }
        }


        private string _softwareID = string.Empty;

        public string SoftwareID
        {
            get { return _softwareID; }
            set { _softwareID = value; }
        }

        private int _makeYearFrom = 0;

        public int MakeYearFrom
        {
            get { return _makeYearFrom; }
            set { _makeYearFrom = value; }
        }

        private int _makeYearUpto = 0;

        public int MakeYearUpto
        {
            get { return _makeYearUpto; }
            set { _makeYearUpto = value; }
        }


        private string _region = string.Empty;

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        private bool _HighAltitude = false;

        public bool HighAltitude
        {
            get { return _HighAltitude; }
            set { _HighAltitude = value; }
        }
    }
}
