using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CommonSuite;

namespace T8SuitePro
{

    class IdaProIdcFile
    {
        protected internal static void create(string filename, SymbolCollection symbols)
        {
            string outputfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "-autogen.idc");
            using (StreamWriter sw = new StreamWriter(outputfile))
            {
                sw.WriteLine("//                                           ");
                sw.WriteLine("//      This file for Trionic 8 symboltable  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// History:                                  ");
                sw.WriteLine("//                                           ");
                sw.WriteLine("// 23.12.12 19:00 Started by Dmitry Khoroshev");
                sw.WriteLine("// 21-01-2015 21:04 Autogen by Mattias Claesson");
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
                sw.WriteLine("   SegCreate(0X0,0X100000,0X0,1,1,2);        ");
                sw.WriteLine("   SegRename(0X0,\"ROM\");                   ");
                sw.WriteLine("   SegClass (0X0,\"CODE\");                  ");
                sw.WriteLine("   SetSegmentType(0X0,2);                    ");
                sw.WriteLine("   SegCreate(0X100000,0X108000,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0X100000,\"RAM_FSRAM\");        ");
                sw.WriteLine("   SegClass (0X100000,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF000,0XFFF400,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF000,\"QADC64\");           ");
                sw.WriteLine("   SegClass (0XFFF000,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF400,0XFFF600,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF400,\"QSM_B\");            ");
                sw.WriteLine("   SegClass (0XFFF400,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF600,0XFFF610,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF600,\"DLCMD\");            ");
                sw.WriteLine("   SegClass (0XFFF600,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF610,0XFFF660,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF610,\"RESERVED_A\");       ");
                sw.WriteLine("   SegClass (0XFFF610,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF660,0XFFF668,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF660,\"SRAM_A\");           ");
                sw.WriteLine("   SegClass (0XFFF660,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF668,0XFFF670,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF668,\"SRAM_B\");           ");
                sw.WriteLine("   SegClass (0XFFF668,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF670,0XFFF678,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF670,\"SRAM_C\");           ");
                sw.WriteLine("   SegClass (0XFFF670,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF678,0XFFF680,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF678,\"SRAM_D\");           ");
                sw.WriteLine("   SegClass (0XFFF678,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF680,0XFFF6C0,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF680,\"DPTRAM\");           ");
                sw.WriteLine("   SegClass (0XFFF680,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF6C0,0XFFF6E0,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF6C0,\"FASRAM\");           ");
                sw.WriteLine("   SegClass (0XFFF6C0,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF6E0,0XFFF700,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF6E0,\"RESERVED_B\");       ");
                sw.WriteLine("   SegClass (0XFFF6E0,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF700,0XFFF800,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF700,\"CTM9\");             ");
                sw.WriteLine("   SegClass (0XFFF700,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFF800,0XFFFA00,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFF800,\"TPU3_B\");           ");
                sw.WriteLine("   SegClass (0XFFF800,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFA00,0XFFFA80,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFFA00,\"BIM\");              ");
                sw.WriteLine("   SegClass (0XFFFA00,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFA80,0XFFFC00,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFFA80,\"TouCAN\");           ");
                sw.WriteLine("   SegClass (0XFFFA80,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFC00,0XFFFE00,0X0,0,0,0);   ");
                sw.WriteLine("   SegRename(0XFFFC00,\"QSM_A\");            ");
                sw.WriteLine("   SegClass (0XFFFC00,\"\");                 ");
                sw.WriteLine("   SegCreate(0XFFFE00,0X1000000,0X0,0,0,0);  ");
                sw.WriteLine("   SegRename(0XFFFE00,\"TPU3_A\");           ");
                sw.WriteLine("   SegClass (0XFFFE00,\"\");                 ");
                sw.WriteLine("   LowVoids(0x0);                            ");
                sw.WriteLine("   HighVoids(0x1000000);                     ");
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
                    string name = sh.SmartVarname;
                    if (sh.Flash_start_address > 0x100000)
                    {
                        sw.WriteLine(String.Format("   namevar(\"RAM_{0}\", 0x{1}, 0x{2});", name.Replace(" ", "_"), sh.Start_address.ToString("X"), sh.Length.ToString("X")));
                    }
                    else
                    {
                        sw.WriteLine(String.Format("   namevar(\"ROM_{0}\", 0x{1}, 0x{2});", name.Replace(" ", "_"), sh.Flash_start_address.ToString("X"), sh.Length.ToString("X")));
                    }
                }

                //sw.WriteLine("   namevar(\"Internal_RAMBAR\", 0xFFFB04, 0x2);");
            
                // End function
                sw.WriteLine("}");
            }

        }
    }
}
