using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CommonSuite;

namespace T7
{

    class IdaProIdcFile
    {
        protected internal static void create(string filename, SymbolCollection symbols)
        {
            string outputfile = Path.GetDirectoryName(filename);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + "-autogen.idc");
            using (StreamWriter sw = new StreamWriter(outputfile))
            {
                sw.WriteLine("//                                           ");
                sw.WriteLine("//      This file for Trionic 7 symboltable  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// History:                                  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// 23.12.12 19:00 Started by Dmitry Khoroshev");
                sw.WriteLine("// 18-11-2014 21:04 Autogen by Mattias Claesson");
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
                sw.WriteLine("        DeleteAll();    // purge database    ");
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
                sw.WriteLine("}                                            ");
                sw.WriteLine("                                             ");
                sw.WriteLine("//------------------------------------------------------------------------");
                sw.WriteLine("// Information about segmentation            ");
                sw.WriteLine("                                             ");
                sw.WriteLine("static Segments(void) {                      ");
                sw.WriteLine("   SetSelector(0X1,0X0);                     ");
                sw.WriteLine("   ;                                         ");
                sw.WriteLine("   SegCreate(0X0,0X80000,0X0,1,1,2);         ");
                sw.WriteLine("   SegRename(0X0,\"ROM\");                   ");
                sw.WriteLine("   SegClass (0X0,\"CODE\");                  ");
                sw.WriteLine("   SetSegmentType(0X0,2);                    ");
                sw.WriteLine("   SegCreate(0XF00000,0XF0FFFF,0X0,1,1,2);   ");
                sw.WriteLine("   SegRename(0XF00000,\"RAM\");              ");
                sw.WriteLine("   SegClass (0XF00000,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFA00, 0XFFFF00, 0X0, 1, 1, 2);");
                sw.WriteLine("   SegRename(0XFFFA00, \"Internal\");        ");
                sw.WriteLine("   SegClass(0XFFFA00, \"\");                 ");
                sw.WriteLine("   LowVoids(0x20);                           ");
                sw.WriteLine("   HighVoids(0xF0FFFF);                      ");
                sw.WriteLine("}                                            ");
                sw.WriteLine("                                             ");
                sw.WriteLine("static main(void) {                          ");
                sw.WriteLine("   GenInfo();                                ");
                sw.WriteLine("   Segments();                               ");

                // Disabled because it was missleading, its bad enough with off_10
                //string[] vectors = Trionic7File.GetVectorNames();
                // Do this because namevar() skip addr=0, that seem to bemostly Type symbols, that we want to ignore. 
                //sw.WriteLine("   MakeDword(0);");
                //sw.WriteLine(String.Format("   MakeName(0, \"{0}\");", vectors[0].Replace(" ", "_")));
                //for (int i = 1; i < 256; i++)
                //{
                //    sw.WriteLine(String.Format("   namevar(\"{0}\", {1}, 4);", vectors[i].Replace(" ", "_"), i*4));
                //}
                for (int i = 0; i < 256; i++)
                {
                    sw.WriteLine(String.Format("   MakeDword(\"{0}\");", i*4));
                    sw.WriteLine(String.Format("   MakeName(\"{0}\", \"\");", i*4));
                }

                foreach (SymbolHelper sh in symbols)
                {
                    string name = sh.Varname;
                    if (sh.Userdescription != "" && sh.Varname == String.Format("Symbolnumber {0}", sh.Symbol_number))
                    {
                        name = sh.Userdescription;
                    }
                    if (sh.Flash_start_address > 0x80000)
                    {
                        sw.WriteLine(String.Format("   namevar(\"RAM_{0}\", 0x{1}, 0x{2});", name.Replace(" ", "_"), sh.Start_address.ToString("X"), sh.Length.ToString("X")));
                    }
                    else
                    {
                        sw.WriteLine(String.Format("   namevar(\"ROM_{0}\", 0x{1}, 0x{2});", name.Replace(" ", "_"), sh.Flash_start_address.ToString("X"), sh.Length.ToString("X")));
                    }
                }

                sw.WriteLine("   namevar(\"Internal_RAMBAR\", 0xFFFB04, 0x2);");
                sw.WriteLine("   namevar(\"Internal_RAMTST\", 0xFFFB02, 0x2);");
                sw.WriteLine("   namevar(\"Internal_RAMMCR\", 0xFFFB00, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SIMCR\", 0xFFFA00, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SIMTR\", 0xFFFA02, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SYNCR\", 0xFFFA04, 0x2);");
                sw.WriteLine("   namevar(\"Internal_RSR\", 0xFFFA07, 0x1);");
                sw.WriteLine("   namevar(\"Internal_SIMTRE\", 0xFFFA08, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PORTE0\", 0xFFFA11, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PORTE1\", 0xFFFA13, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DDRE\", 0xFFFA15, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PEPAR\", 0xFFFA17, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PORTF0\", 0xFFFA19, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PORTF1\", 0xFFFA1B, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DDRF\", 0xFFFA1D, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PFPAR\", 0xFFFA1F, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SYPCR\", 0xFFFA21, 0x1);");
                sw.WriteLine("   namevar(\"Internal_PICR\", 0xFFFA22, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PITR\", 0xFFFA24, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SWSR_1\", 0xFFFA26, 0x1);");
                sw.WriteLine("   namevar(\"Internal_SWSR\", 0xFFFA27, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TSTMSRA\", 0xFFFA30, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TSTMSRB\", 0xFFFA32, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TSTSC\", 0xFFFA34, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TSTRC\", 0xFFFA36, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CREG\", 0xFFFA38, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DREG\", 0xFFFA3A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_PORTC\", 0xFFFA41, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSPAR0\", 0xFFFA44, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSPAR1\", 0xFFFA46, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBARBT\", 0xFFFA48, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSORBT\", 0xFFFA4A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR0\", 0xFFFA4C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR0\", 0xFFFA4E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR1\", 0xFFFA50, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR1\", 0xFFFA52, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR2\", 0xFFFA54, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR2\", 0xFFFA56, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR3\", 0xFFFA58, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR3\", 0xFFFA5A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR4\", 0xFFFA5C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR4\", 0xFFFA5E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR5\", 0xFFFA60, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR5\", 0xFFFA62, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR6\", 0xFFFA64, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR6\", 0xFFFA66, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR7\", 0xFFFA68, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR7\", 0xFFFA6A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR8\", 0xFFFA6C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR8\", 0xFFFA6E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR9\", 0xFFFA70, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR9\", 0xFFFA72, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSBAR10\", 0xFFFA74, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CSOR10\", 0xFFFA76, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QSMCR\", 0xFFFC00, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QTEST\", 0xFFFC02, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QILR\", 0xFFFC04, 0x1);");
                sw.WriteLine("   namevar(\"Internal_QIVR\", 0xFFFC05, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SCCR0\", 0xFFFC08, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SCCR1\", 0xFFFC0A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SCSR\", 0xFFFC0C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SCDR\", 0xFFFC0E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QPDR\", 0xFFFC15, 0x1);");
                sw.WriteLine("   namevar(\"Internal_PQSPAR\", 0xFFFC16, 0x1);");
                sw.WriteLine("   namevar(\"Internal_QDDR\", 0xFFFC17, 0x1);");
                sw.WriteLine("   namevar(\"Internal_SPCR0\", 0xFFFC18, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SPCR1\", 0xFFFC1A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SPCR2\", 0xFFFC1C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SPCR3\", 0xFFFC1E, 0x1);");
                sw.WriteLine("   namevar(\"Internal_SPSR\", 0xFFFC1F, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QRXD\", 0xFFFD00, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QTXD\", 0xFFFD20, 0x2);");
                sw.WriteLine("   namevar(\"Internal_QCMD\", 0xFFFD40, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUMCR\", 0xFFFE00, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUCFG\", 0xFFFE02, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DSCR\", 0xFFFE04, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DSSR\", 0xFFFE06, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUICR\", 0xFFFE08, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUIER\", 0xFFFE0A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CFSR0\", 0xFFFE0C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CFSR1\", 0xFFFE0E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CFSR2\", 0xFFFE10, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CFSR3\", 0xFFFE12, 0x2);");
                sw.WriteLine("   namevar(\"Internal_HSR0\", 0xFFFE14, 0x2);");
                sw.WriteLine("   namevar(\"Internal_HSR1\", 0xFFFE16, 0x2);");
                sw.WriteLine("   namevar(\"Internal_HSRR0\", 0xFFFE18, 0x2);");
                sw.WriteLine("   namevar(\"Internal_HSRR1\", 0xFFFE1A, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CPR0\", 0xFFFE1C, 0x2);");
                sw.WriteLine("   namevar(\"Internal_CPR1\", 0xFFFE1E, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUISR\", 0xFFFE20, 0x2);");
                sw.WriteLine("   namevar(\"Internal_LINK\", 0xFFFE22, 0x2);");
                sw.WriteLine("   namevar(\"Internal_SGLR\", 0xFFFE24, 0x2);");
                sw.WriteLine("   namevar(\"Internal_DCNR\", 0xFFFE26, 0x2);");
                sw.WriteLine("   namevar(\"Internal_TPUPRAM\", 0xFFFF00, 0x2);");

                // End function
                sw.WriteLine("}");
            }

        }
    }
}
