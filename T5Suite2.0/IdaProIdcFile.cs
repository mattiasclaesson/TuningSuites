using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CommonSuite;
using Trionic5Tools;

namespace T5Suite2
{
    class IdaProIdcFile
    {
        protected internal static int symHasXdef(string symName, IECUFileInformation t5T, IECUFile fileData)
        {
            SymbolCollection symbols = t5T.SymbolCollection;

            // Some symbols borrow size and axes from other maps (Far from complete list)
            if (symName == "Ign_map_4")
            {
                symName = "Ign_map_0";
            }
            else if (symName == "Mis200_map" || symName == "Mis1000_map")
            {
                symName = "Misfire_map";
            }

            foreach (SymbolHelper sh in symbols)
            {
                if (sh.SmartVarname == symName + "_x_size!" && sh.Flash_start_address > 0x7FFF && sh.Length == 2)
                {
                    byte[] data = fileData.ReadData((uint)sh.Flash_start_address, 2);
                    return (data[0] << 8 | data[1]);
                }
            }

            return 0;
        }

        protected internal static void create(string filename, IECUFileInformation t5T, IECUFile fileData)
        {
            string outputfile = Path.GetDirectoryName(filename);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + "-autogen.idc");
            
            int romStart = 0x80000 - t5T.Filelength;

            using (StreamWriter sw = new StreamWriter(outputfile))
            {
                sw.WriteLine("//                                           ");
                sw.WriteLine("//      This file for Trionic 5 symboltable  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// History:                                  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// 23.12.12 19:00 Started by Dmitry Khoroshev");
                sw.WriteLine("// 18-11-2014 21:04 Autogen by Mattias Claesson");
                sw.WriteLine("// 14-03-2018 19:05 T5 by Christian Ivarsson ");
                sw.WriteLine("                                             ");
                sw.WriteLine("#include <idc.idc>                           ");
                sw.WriteLine("                                             ");
                sw.WriteLine("static namevar(name,addr,size) {             ");
                sw.WriteLine("   auto i;                                   ");
                sw.WriteLine("                                             ");
                sw.WriteLine("   if (addr != 0) {                          ");
                sw.WriteLine("      Message(name);                         ");
                sw.WriteLine("      Message(\"\\n\");                      ");
                sw.WriteLine("      MakeByte(addr);                        ");
                sw.WriteLine("      if(size==4) {                          ");
                sw.WriteLine("         MakeDword(addr);                    ");
                sw.WriteLine("      }                                      ");
                sw.WriteLine("      else if(size>1) {                      ");
                sw.WriteLine("         MakeArray(addr,size);               ");
                sw.WriteLine("      }                                      ");
                sw.WriteLine("      MakeName(addr,name);                   ");
                sw.WriteLine("   }                                         ");
                sw.WriteLine("}                                            ");
                sw.WriteLine("//------------------------------------------------------------------------");
                sw.WriteLine("// General information                       ");
                sw.WriteLine("                                             ");
                sw.WriteLine("static GenInfo(void) {                       ");
                sw.WriteLine("                                             ");
                sw.WriteLine("  DeleteAll();    // purge database          ");
                sw.WriteLine("	SetPrcsr(\"68330\");                       ");
                sw.WriteLine("	StringStp(0xA);                            ");
                sw.WriteLine("	Tabs(1);                                   ");
                sw.WriteLine("	Comments(1);                               ");
                sw.WriteLine("	Voids(0);                                  ");
                sw.WriteLine("	XrefShow(2);                               ");
                sw.WriteLine("	AutoShow(1);                               ");
                sw.WriteLine("	Indent(16);                                ");
                sw.WriteLine("	CmtIndent(40);                             ");
                sw.WriteLine("	TailDepth(0x10);                           ");
                sw.WriteLine("                                             ");
                sw.WriteLine("	auto aOpt;                                 "); // Change some of the default settings
                sw.WriteLine("	aOpt = GetLongPrm(INF_AF);                 ");
                sw.WriteLine("	aOpt = aOpt & ~AF_FINAL;                   "); // Untick Final pass
                sw.WriteLine("	aOpt = aOpt & ~AF_UNK;                     "); // Untick Delete ins w/out xref (IDA is bad with jump tables and _WILL_ butcher functions if this is ticked)
                sw.WriteLine("	SetLongPrm(INF_AF, aOpt);                  ");
                sw.WriteLine("	aOpt = GetLongPrm(INF_AF2);                ");
                sw.WriteLine("	aOpt = aOpt | AF2_VERSP;                   "); // Tick Full stack analysis
                sw.WriteLine("	SetLongPrm(INF_AF2, aOpt);                 ");
                sw.WriteLine("}                                            ");
                sw.WriteLine("                                             ");
                sw.WriteLine("//------------------------------------------------------------------------");
                sw.WriteLine("// Information about segmentation            ");
                sw.WriteLine("                                             ");
                sw.WriteLine("static Segments(void) {                      ");
                sw.WriteLine("   SetSelector(0X1,0X0);                     ");
                sw.WriteLine(String.Format("   SegCreate(0x{0},0X80000,0X0,1,1,2);", romStart.ToString("X")));
                sw.WriteLine(String.Format("   SegRename(0x{0},\"ROM\");          ", romStart.ToString("X")));
                sw.WriteLine(String.Format("   SegClass (0x{0},\"CODE\");         ", romStart.ToString("X")));
                sw.WriteLine(String.Format("   SetSegmentType(0x{0},2);           ", romStart.ToString("X")));
                sw.WriteLine("   SegCreate(0X0,0X8000,0X0,1,1,2);          ");
                sw.WriteLine("   SegRename(0X0,\"RAM\");                   ");
                sw.WriteLine("   SegClass (0X0,\"\");                      ");
                sw.WriteLine("   SegCreate(0XF007F0, 0XF00810, 0X0, 1,1,2);");
                sw.WriteLine("   SegRename(0XF007F0,\"CAN\");              ");
                sw.WriteLine("   SegClass (0XF007F0,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF000, 0XFFF800, 0X0, 1,1,2);");
                sw.WriteLine("   SegRename(0XFFF000, \"TPURAM\");          ");
                sw.WriteLine("   SegClass(0XFFF000, \"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFA00, 0XFFFEFF, 0X0, 1,1,2);");
                sw.WriteLine("   SegRename(0XFFFA00, \"Internal\");        ");
                sw.WriteLine("   SegClass(0XFFFA00, \"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFF00, 0XFFFFFF, 0X0, 1,1,2);");
                sw.WriteLine("   SegRename(0XFFFF00, \"TPUPRAM\");         ");
                sw.WriteLine("   SegClass(0XFFFF00, \"\");                 ");
                sw.WriteLine("   LowVoids(0x000000);                       ");
                sw.WriteLine("   HighVoids(0x080000);                      ");
                sw.WriteLine("}                                            ");
                sw.WriteLine("                                             ");

                // Handle generation of entries in the interrupt table since Trionic 5 is inconsistent
                sw.WriteLine("static GenTable(void) {                      ");
                sw.WriteLine(String.Format("   auto currAddress = 0x{0};   ", (romStart + 4).ToString("X")));
                sw.WriteLine("   auto Step = 1;                            ");
                sw.WriteLine(String.Format("   MakeDword(0x{0});           ", romStart.ToString("X")));
                sw.WriteLine(String.Format("   MakeName(0x{0},\"INIT_SP\");", romStart.ToString("X")));
                sw.WriteLine("   MakeName(Dword(currAddress),\"INIT_Entry\");");
                sw.WriteLine("   while (Step < 256) {                      ");
                sw.WriteLine("      if (Dword(currAddress) != 0xFFFFFFFF && GetFunctionName(Dword(currAddress)) == 0) {");
                sw.WriteLine("         if (MakeFunction(Dword(currAddress), BADADDR) == 0) { return; }");
                sw.WriteLine("      }                                      ");
                sw.WriteLine("      MakeDword(currAddress);                ");
                sw.WriteLine("      currAddress = currAddress + 4;         ");
                sw.WriteLine("      Step ++;                               ");
                sw.WriteLine("}}                                           ");
                sw.WriteLine("                                             ");

                sw.WriteLine("static PrintFooter(void) {                   "); // Print out info in the footer area
                sw.WriteLine("   auto Location = 0x80000 - 6;              ");
                sw.WriteLine("   auto Len, Type;                           ");
                sw.WriteLine("   while (Byte(Location) != 0xFF &&          ");
                sw.WriteLine("          Byte(Location + 1) != 0xFF) {      ");
                sw.WriteLine("      Len  = Byte(Location + 1);             ");
                sw.WriteLine("      Type = Byte(Location);                 ");
                sw.WriteLine("      Location = Location - Len;             ");
                sw.WriteLine("      if        (Type == 0x01) {             ");
                sw.WriteLine("         MakeName(Location,\"Partnumber_1\");");
                sw.WriteLine("      } else if (Type == 0x02) {             ");
                sw.WriteLine("         MakeName(Location,\"Partnumber_2\");");
                sw.WriteLine("      } else if (Type == 0x03) {             ");
                sw.WriteLine("         MakeName(Location, \"Software_ID\");");
                sw.WriteLine("      } else if (Type == 0x04) {             ");
                sw.WriteLine("         MakeName(Location, \"Eng_type\");   ");
                sw.WriteLine("      } else if (Type == 0x05) {             ");
                sw.WriteLine("         MakeName(Location, \"VSS_Code\");   ");
                sw.WriteLine("      } else if (Type == 0x06) {             ");
                sw.WriteLine("         MakeName(Location, \"Type\");       ");
                sw.WriteLine("      } else if (Type == 0xFC) {             ");
                sw.WriteLine("         MakeName(Location, \"Flash_End\");  ");
                sw.WriteLine("      } else if (Type == 0xFD) {             ");
                sw.WriteLine("         MakeName(Location, \"Flash_Start\");");
                sw.WriteLine("      } else if (Type == 0xFE) {             ");
                sw.WriteLine("         MakeName(Location, \"Binary_End\"); ");
                sw.WriteLine("      }                                      ");
                sw.WriteLine("      MakeStr(Location, Location + Len);     "); // Turn text into a string
                sw.WriteLine("      MakeArray(Location + Len, 2);          "); // Neat up info-bytes (type and length)
                sw.WriteLine("      Location = Location - 2;               "); // Step down to next pair of info-bytes
                sw.WriteLine("}}                                           ");
                sw.WriteLine("                                             ");

                sw.WriteLine("static main(void) {                          "); // Main script
                sw.WriteLine("   GenInfo();                                ");
                sw.WriteLine("   Segments();                               ");
                sw.WriteLine("   GenTable();                               ");

                SymbolCollection symbols = t5T.SymbolCollection;

                if (symbols.Count > 0)
                {
                    foreach (SymbolHelper sh in symbols)
                    {
                        string name = sh.SmartVarname.Replace(" ", "_").Replace("!", "");
                        int Len = sh.Length;

                        if (Len > 0)
                        {
                            long Address = sh.Flash_start_address < 0x8000 ? sh.Start_address : sh.Flash_start_address;
                            if (Address < 0x8000)
                            {
                                sw.WriteLine(String.Format("   namevar(\"r{0}\", 0x{1}, 0x{2});", name, Address.ToString("X"), Len.ToString("X")));
                            }
                            else
                            {
                                sw.WriteLine(String.Format("   namevar(\"f{0}\", 0x{1}, 0x{2});", name, Address.ToString("X"), Len.ToString("X")));
                            }

                            // Special case for known string(s)
                            if (name == "Data_namn")
                            {
                                sw.WriteLine(String.Format("   MakeStr(0x{0}, 0x{1});", Address.ToString("X"), (Address + Len).ToString("X")));
                            }

                            // Turn larger symbols into arrays (Must be named beforehand)
                            else if (Len > 4)
                            {
                                byte Divider = 1;

                                // Check size, alignment and width. If all pass, change it into a 16-bit symbol
                                if ((Len & 1) == 0 && (Address & 1) == 0 && t5T.isSixteenBitTable(sh.SmartVarname) == true)
                                {
                                    Divider = 2;
                                    sw.WriteLine(String.Format("   MakeWord(0x{0});", Address.ToString("X")));
                                }

                                // Turn it into an array
                                sw.WriteLine(String.Format("   MakeArray(0x{0}, 0x{1});", Address.ToString("X"), (Len / Divider).ToString("X")));

                                int  symX = symHasXdef(name, t5T, fileData); // Look for corresponding X size
                                int  arrX = symX == 0 ? 16 : symX;           // Default do width of 16 if "sym_x_Size!" could not be found
                                int  arrayXsize  = Len >= arrX ? arrX : Len; // Shorten width in case it's larger than total symbol length
                                int  arrayAlign  = Divider == 2 ? 6 : 4;     // Do not space bytes too wide apart or words too tight together
                                byte arrayFormat = 0x14;                     // Display index in hex (0x10), Enable index (0x04)

                                // Check for symbols that are signed (Not complete list)
                                if (name == "Ign_map_0" || name == "Ign_map_4")
                                {
                                    arrayFormat |= 2; // Signed data
                                }

                                // Compact arrays in SRAM
                                if (Address < 0x8000)
                                {
                                    arrayFormat |= 1; // "dup" construct
                                }

                                // Configure signedness, width and how to align contents
                                sw.WriteLine(String.Format("   SetArrayFormat(0x{0}, 0x{1}, 0x{2}, 0x{3});", Address.ToString("X"), arrayFormat.ToString("X"), arrayXsize.ToString("X"), arrayAlign.ToString("X")));
                            }
                        }
                    }
                }

                sw.WriteLine("   namevar(\"CAN_LONGACC\", 0xF007FE,0x1);"); // CAN
                sw.WriteLine("   namevar(\"CAN_CMDPORT\", 0xF007FF,0x1);");
                sw.WriteLine("   namevar(\"CAN_DATAPORT\",0xF00800,0x1);");

                sw.WriteLine("   namevar(\"SIM_SIMCR\" , 0xFFFA00, 0x2);"); // SIM
                sw.WriteLine("   namevar(\"SIM_SIMTR\" , 0xFFFA02, 0x2);");
                sw.WriteLine("   namevar(\"SIM_SYNCR\" , 0xFFFA04, 0x2);");
                sw.WriteLine("   namevar(\"SIM_RSR\"   , 0xFFFA07, 0x1);");
                sw.WriteLine("   namevar(\"SIM_SIMTRE\", 0xFFFA08, 0x2);");
                sw.WriteLine("   namevar(\"SIM_PORTE0\", 0xFFFA11, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PORTE1\", 0xFFFA13, 0x1);");
                sw.WriteLine("   namevar(\"SIM_DDRE\"  , 0xFFFA15, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PEPAR\" , 0xFFFA17, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PORTF0\", 0xFFFA19, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PORTF1\", 0xFFFA1B, 0x1);");
                sw.WriteLine("   namevar(\"SIM_DDRF\"  , 0xFFFA1D, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PFPAR\" , 0xFFFA1F, 0x2);");
                sw.WriteLine("   namevar(\"SIM_SYPCR\" , 0xFFFA21, 0x1);");
                sw.WriteLine("   namevar(\"SIM_PICR\"  , 0xFFFA22, 0x2);");
                sw.WriteLine("   namevar(\"SIM_PITR\"  , 0xFFFA24, 0x2);");
                sw.WriteLine("   namevar(\"SIM_SWSRw\" , 0xFFFA26, 0x1);");
                sw.WriteLine("   namevar(\"SIM_SWSR\"  , 0xFFFA27, 0x1);");
                sw.WriteLine("   namevar(\"SIM_TSTMSRA\",0xFFFA30, 0x2);");
                sw.WriteLine("   namevar(\"SIM_TSTMSRB\",0xFFFA32, 0x2);");
                sw.WriteLine("   namevar(\"SIM_TSTSC\" , 0xFFFA34, 0x2);");
                sw.WriteLine("   namevar(\"SIM_TSTRC\" , 0xFFFA36, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CREG\"  , 0xFFFA38, 0x2);");
                sw.WriteLine("   namevar(\"SIM_DREG\"  , 0xFFFA3A, 0x2);");
                sw.WriteLine("   namevar(\"SIM_PORTC\" , 0xFFFA41, 0x1);");
                sw.WriteLine("   namevar(\"SIM_CSPAR0\", 0xFFFA44, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSPAR1\", 0xFFFA46, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBARBT\",0xFFFA48, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSORBT\", 0xFFFA4A, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR0\", 0xFFFA4C, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR0\" , 0xFFFA4E, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR1\", 0xFFFA50, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR1\" , 0xFFFA52, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR2\", 0xFFFA54, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR2\" , 0xFFFA56, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR3\", 0xFFFA58, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR3\" , 0xFFFA5A, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR4\", 0xFFFA5C, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR4\" , 0xFFFA5E, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR5\", 0xFFFA60, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR5\" , 0xFFFA62, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR6\", 0xFFFA64, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR6\" , 0xFFFA66, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR7\", 0xFFFA68, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR7\" , 0xFFFA6A, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR8\", 0xFFFA6C, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR8\" , 0xFFFA6E, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR9\", 0xFFFA70, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR9\" , 0xFFFA72, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSBAR10\",0xFFFA74, 0x2);");
                sw.WriteLine("   namevar(\"SIM_CSOR10\", 0xFFFA76, 0x2);");

                sw.WriteLine("   namevar(\"TPU_TRAMMCR\",0xFFFB00, 0x2);"); // TPURAM control
                sw.WriteLine("   namevar(\"TPU_TRAMTST\",0xFFFB02, 0x2);");
                sw.WriteLine("   namevar(\"TPU_TRAMBAR\",0xFFFB04, 0x2);");

                sw.WriteLine("   namevar(\"QSM_QSMCR\" , 0xFFFC00, 0x2);"); // QSM
                sw.WriteLine("   namevar(\"QSM_QTEST\" , 0xFFFC02, 0x2);");
                sw.WriteLine("   namevar(\"QSM_QILR\"  , 0xFFFC04, 0x1);");
                sw.WriteLine("   namevar(\"QSM_QIVR\"  , 0xFFFC05, 0x1);");
                sw.WriteLine("   namevar(\"QSM_SCCR0\" , 0xFFFC08, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SCCR1\" , 0xFFFC0A, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SCSR\"  , 0xFFFC0C, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SCDR\"  , 0xFFFC0E, 0x2);");
                sw.WriteLine("   namevar(\"QSM_PORTQSw\",0xFFFC14, 0x1);"); // They access it as word sometimes
                sw.WriteLine("   namevar(\"QSM_PORTQS\", 0xFFFC15, 0x1);");
                sw.WriteLine("   namevar(\"QSM_PQSPAR\", 0xFFFC16, 0x1);");
                sw.WriteLine("   namevar(\"QSM_DDRQS\" , 0xFFFC17, 0x1);");
                sw.WriteLine("   namevar(\"QSM_SPCR0\" , 0xFFFC18, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SPCR1\" , 0xFFFC1A, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SPCR2\" , 0xFFFC1C, 0x2);");
                sw.WriteLine("   namevar(\"QSM_SPCR3\" , 0xFFFC1E, 0x1);");
                sw.WriteLine("   namevar(\"QSM_SPSR\"  , 0xFFFC1F, 0x1);");
                sw.WriteLine("   namevar(\"QSM_QRXD\"  , 0xFFFD00, 0x2);"); // First two are word arrays, last one is byte
                sw.WriteLine("   MakeWord(0xFFFD00);                    ");
                sw.WriteLine("   MakeArray(0xFFFD00, 16);               ");
                sw.WriteLine("   namevar(\"QSM_QTXD\"  , 0xFFFD20, 0x2);");
                sw.WriteLine("   MakeWord(0xFFFD20);                    ");
                sw.WriteLine("   MakeArray(0xFFFD20, 16);               ");
                sw.WriteLine("   namevar(\"QSM_QCMD\"  , 0xFFFD40, 0x1);");
                sw.WriteLine("   MakeArray(0xFFFD40, 16);               ");

                sw.WriteLine("   namevar(\"TPU_TPUMCR\", 0xFFFE00, 0x2);"); // TPU
                sw.WriteLine("   namevar(\"TPU_TCR\"   , 0xFFFE02, 0x2);");
                sw.WriteLine("   namevar(\"TPU_DSCR\"  , 0xFFFE04, 0x2);");
                sw.WriteLine("   namevar(\"TPU_DSSR\"  , 0xFFFE06, 0x2);");
                sw.WriteLine("   namevar(\"TPU_TICR\"  , 0xFFFE08, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CIER\"  , 0xFFFE0A, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CFSR0\" , 0xFFFE0C, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CFSR1\" , 0xFFFE0E, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CFSR2\" , 0xFFFE10, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CFSR3\" , 0xFFFE12, 0x2);");
                sw.WriteLine("   namevar(\"TPU_HSQR0\" , 0xFFFE14, 0x2);");
                sw.WriteLine("   namevar(\"TPU_HSQR1\" , 0xFFFE16, 0x2);");
                sw.WriteLine("   namevar(\"TPU_HSRR0\" , 0xFFFE18, 0x2);");
                sw.WriteLine("   namevar(\"TPU_HSRR1\" , 0xFFFE1A, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CPR0\"  , 0xFFFE1C, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CPR1\"  , 0xFFFE1E, 0x2);");
                sw.WriteLine("   namevar(\"TPU_CISR\"  , 0xFFFE20, 0x2);");
                sw.WriteLine("   namevar(\"TPU_LR\"    , 0xFFFE22, 0x2);");
                sw.WriteLine("   namevar(\"TPU_SGLR\"  , 0xFFFE24, 0x2);");
                sw.WriteLine("   namevar(\"TPU_DCNR\"  , 0xFFFE26, 0x2);");

                sw.WriteLine("   MakeDword(0x07FFFC);                   "); // Mark checksum-bytes
                sw.WriteLine("   MakeName(0x07FFFC, \"CHECKSUM\");      ");
                sw.WriteLine("   PrintFooter();                         ");
                sw.WriteLine("}                                         "); // End function
            }
        }
    }
}
