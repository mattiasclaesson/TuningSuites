using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public class Disassembler
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public enum ProgressType : int
        {
            DisassemblingVectors,
            DisassemblingFunctions,
            TranslatingVectors,
            AddingLabels,
            TranslatingLabels,
            SortingData,
            PassOne
        }

        public class ProgressEventArgs : System.EventArgs
        {
            private ProgressType _type;

            public ProgressType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            private int _percentage;

            public int Percentage
            {
                get { return _percentage; }
                set { _percentage = value; }
            }

            private string _info;

            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            public ProgressEventArgs(string info, int percentage, ProgressType type)
            {
                this._info = info;
                this._percentage = percentage;
                this._type = type;
            }
        }

        public delegate void Progress(object sender, ProgressEventArgs e);
        public event Disassembler.Progress onProgress;

        uint swap = 0;
        //FILE *IN, *OUT, *TST;

        long SRAM_start = 0x1000L;
        long SRAM_end = 0x7FFFL;

        long[] A_reg =  new long[8];// = {0, 0, 0, 0, 0, 0, 0, 0};
        long[] D_reg = new long[8];// = {0, 0, 0, 0, 0, 0, 0, 0};
        long souraddr = 0L;
        long destaddr = 0L;
        long trgdata = 0L;
        private SymbolCollection m_symbols = new SymbolCollection();
        

        public Disassembler()
        {
            for (int t = 0 ; t< 8; t ++) 
            {
                A_reg.SetValue(0, t);
                D_reg.SetValue(0,t);
            }

        }

        uint build_source(out string sour, uint size, byte srcmod, byte srcreg, long addr, long offset, BinaryReader br)
        {

            uint ilen = 0;
            byte ch3, ch4, ch5, ch6, ch7, ch8;
            long trgdata = 0;
            long disp;
            string vari = string.Empty;
            sour = string.Empty;
            switch ((int)srcmod)
            {
                case 0x00:
                    sour = "D" + Convert.ToInt16(srcreg).ToString();
                    break;
                case 0x01:
                    sour = "A" + Convert.ToInt16(srcreg).ToString();
                    break;
                case 0x02:
                    sour = "(A" + Convert.ToInt16(srcreg).ToString() + ")";
                    break;
                case 0x03:
                    sour = "(A" + Convert.ToInt16(srcreg).ToString() + ")+";
                    break;
                case 0x04:
                    sour = "-(A" + Convert.ToInt16(srcreg).ToString() + ")";
                    break;
                case 0x05:
                    ch3 = br.ReadByte();
                    ch4 = br.ReadByte();
                    trgdata = (long)ch3 * 256 + ch4;
                    sour = "(#" + trgdata.ToString("X4") + ",A" + Convert.ToInt16(srcreg).ToString() + ")";
                    ilen++;
                    break;
                case 0x06:
                    ch3 = br.ReadByte();
                    ch4 = br.ReadByte();
                    ilen++;
                    if ((ch3 & 1) == 1) // Base Displacement 
                    {
                        if ((ch4 & 0x30) == 0x30) // BD size = 11, long word 
                        {
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            ch7 = br.ReadByte();
                            ch8 = br.ReadByte();
                            trgdata = (long)ch5 * 16777216L + (long)ch6 * 65536L + (long)ch7 * 256 + ch8;
                            if (find_symbol(out vari, trgdata) == 1)
                            {
                                // DEBUG: printf("\nwithin SRAM!\n");
                                //sprintf(sour,"(%s+%c%u*%u)\0",vari,((ch3&0x80)==0x80?'A':'D'),,; 
                                int itemp1 = ((ch3 & 0x70) >> 4);
                                int itemp2 = 1 << ((ch3 & 6) >> 1);
                                sour = "(" + vari + "+" + ((ch3 & 0x80) == 0x80 ? 'A' : 'D') + itemp1.ToString() + "*" + itemp2.ToString() + ")";
                                ilen += 2;
                                souraddr = trgdata;
                            }
                            else
                            {
                                //sprintf(sour,"(%08lX + ...).L\0",trgdata); 
                                sour = "(" + trgdata.ToString("X8") + " + ...).L";
                                ilen += 2;
                            }
                        }
                        else if ((ch4 & 0x30) == 0x20) // BD size = 10, word 
                        {
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            trgdata = (long)ch5 * 256 + ch6;
                            //sprintf(sour,"(%04lX + ...).W\0",trgdata); 
                            sour = "(" + trgdata.ToString("X4") + " + ...).W";
                            ilen++;
                        }
                        else if ((ch4 & 0x30) == 0x10) // BD size = 01, null 
                        {
                            //sprintf(sour,"(%04lX + ...).W\0",trgdata);
                            sour = "(" + trgdata.ToString("X4") + " + ...).W";
                        }
                    }
                    else // single word extension (d8,An,Xn.SIZE*SCALE) 
                    {
                        //sprintf(sour,"(%02X,A%u,%c%u.%c*%u)\0",ch4,srcreg,((ch3&0x80)==0x80?'A':'D'),((ch3&0x70)>>4),((ch3&0x08)==0x08?'L':'W'),1<<((ch3&6)>>1));
                        int itemp1 = ((ch3 & 0x70) >> 4);
                        int itemp2 = 1 << ((ch3 & 6) >> 1);
                        char byte1 = ((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                        char byte2 = ((ch3 & 0x08) == 0x08 ? 'L' : 'W');
                        sour = "(" + ch4.ToString("X2") + ",A" + Convert.ToInt16(srcreg).ToString() + "," + byte1 + itemp1.ToString() + "." + byte2 + "*" + itemp2.ToString();
                    }
                    break;
                case 0x07:
                    switch (srcreg)
                    {
                        case 0x00: // (xxx).W 
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            trgdata = (long)ch3 * 256 + ch4;
                            //sprintf(sour,"(%04lX).W\0",trgdata); ilen++;
                            sour = "(" + trgdata.ToString("X4") + ").W";
                            break;
                        case 0x01: // (xxx).L 
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            trgdata = (long)ch3 * 16777216L + (long)ch4 * 65536L + (long)ch5 * 256 + ch6;
                            if (find_symbol(out vari, trgdata) == 1)
                            {
                                // DEBUG: printf("\nwithin SRAM!\n"); 
                                //sprintf(sour,"(%s)\0",vari); 
                                sour = "(" + vari + ")";
                                ilen += 2;
                                souraddr = trgdata;
                            }
                            else
                            {
                                //sprintf(sour,"(%08lX).L\0",trgdata); 
                                sour = "(" + trgdata.ToString("X8") + ").L";
                                ilen += 2;
                            }
                            break;
                        case 0x04: // #(data) 
                            switch (size)
                            {
                                case 0x01: // BYTE TYPE 
                                    ch3 = br.ReadByte();
                                    ch4 = br.ReadByte();
                                    //sprintf(sour,"#%02X\0",ch4); 
                                    sour = "#" + ch4.ToString("X2");
                                    ilen++;
                                    break;
                                case 0x03: // WORD TYPE 
                                    ch3 = br.ReadByte();
                                    ch4 = br.ReadByte();
                                    trgdata = (long)ch3 * 256 + ch4;
                                    //sprintf(sour,"#%04lX\0",trgdata); 
                                    sour = "#" + trgdata.ToString("X4");
                                    ilen++;
                                    break;
                                case 0x02: // LONG TYPE 
                                    ch3 = br.ReadByte();
                                    ch4 = br.ReadByte();
                                    ch5 = br.ReadByte();
                                    ch6 = br.ReadByte();
                                    trgdata = (long)ch3 * 16777216L + (long)ch4 * 65536L + (long)ch5 * 256 + ch6;
                                    if (find_symbol(out vari, trgdata) == 1)
                                    {
                                        //sprintf(sour,"#%s\0",vari); 
                                        sour = "#" + vari;
                                        ilen += 2;
                                        souraddr = trgdata;
                                    }
                                    else
                                    {
                                        //sprintf(sour,"#%08lX\0",trgdata); 
                                        sour = "#" + trgdata.ToString("X8");
                                        ilen += 2;
                                    }
                                    break;
                            }
                            break;
                        case 0x02: // (d16,PC) 
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            disp = (long)ch4;
                            disp += (long)(ch3) << 8;
                            if ((disp & 0x8000) == 0) disp = (0x00007FFF & disp);
                            else disp = (0xFFFF8000 | disp);
                            trgdata = addr + disp + 2 + offset;
                            //sprintf(sour,"(%08lX)\0",trgdata); 
                            sour = "(" + trgdata.ToString("X8") + ")";
                            ilen++;
                            break;
                        case 0x03: // (bd,PC,Xn) 
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            ilen++;
                            if ((ch3 & 1) == 1) // Base Displacement 
                            {
                                if ((ch4 & 0xc0) == 0) // Supress BS & IS 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0x70) // Supress Index Operand 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0x80) // Suppress Base Register 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0xc0) // BS & IS active
                                {
                                }
                            }
                            else // (d8,PC,Xn.SIZE*SCALE) 
                            {
                                disp = (long)ch4;
                                if ((disp & 0x80) == 0) disp = (0x0000007F & disp);
                                else disp = (0xFFFFFF80 | disp);
                                trgdata = addr + disp + 2 + offset;
                                int itemp1 = (ch3 & 70) >> 4;
                                int itemp2 = 1 << ((ch3 & 6) >> 1);
                                char byte1 = ((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                                char byte2 = ((ch3 & 0x08) == 0x08 ? 'L' : 'W');
                                //sprintf(sour,"%08lX(PC,%c%u.%c*%u)\0",trgdata,((ch3&0x80)==0x80?'A':'D'),(ch3&70)>>4,((ch3&0x08)==0x08?'L':'W'),1<<((ch3&6)>>1));
                                sour = trgdata.ToString("X8") + "(PC," + byte1 + itemp1.ToString() + "." + byte2 + "*" + itemp2.ToString();
                            }
                            break;
                    }
                    break;
            }
            return (ilen);
        }


        uint build_destination(out string dest, byte size, byte dstmod, byte dstreg, long addr, long offset, BinaryReader br)
        {
            uint ilen = 0;
            byte ch3, ch4, ch5, ch6, ch7, ch8;
            long disp;
            string vari = string.Empty;
            dest = string.Empty;
            trgdata = 0L;
            switch (dstmod)
            {
                case 0x00:
                    //sprintf(dest,"D%u\0",dstreg);
                    dest = "D" + Convert.ToInt16(dstreg).ToString();
                    break;
                case 0x01:
                    //sprintf(dest,"A%u\0",dstreg);
                    //          sprintf(dest,"A%u\0",dstreg);
                    dest = "A" + Convert.ToInt16(dstreg).ToString();
                    break;
                case 0x02:
                    //sprintf(dest,"(A%u)\0",dstreg);
                    dest = "(A" + Convert.ToInt16(dstreg).ToString() + ")";
                    break;
                case 0x03:
                    //sprintf(dest,"(A%u)+\0",dstreg);
                    dest = "(" + Convert.ToInt16(dstreg).ToString() + ")+";
                    break;
                case 0x04:
                    //sprintf(dest,"-(A%u)\0",dstreg);
                    dest = "-(A" + Convert.ToInt16(dstreg).ToString() + ")";
                    break;
                case 0x05:
                    ch3 = br.ReadByte();
                    ch4 = br.ReadByte();
                    trgdata = (long)ch3 * 256 + ch4;
                    if (find_symbol(out vari, trgdata + (long)A_reg.GetValue(dstreg)) == 1)
                    {
                        //   sprintf(dest,"(%s)\0",vari); 
                        dest = "(" + vari + ")";
                        ilen++;
                    }
                    else
                    {
                        //sprintf(dest,"(#%04lX,A%u)\0",trgdata,dstreg); ilen++;
                        dest = "(#" + trgdata.ToString("X4") + ",A" + Convert.ToInt16(dstreg).ToString() + ")";
                        ilen++;
                    }
                    break;
                case 0x06:
                    ch3 = br.ReadByte(); ch4 = br.ReadByte(); ilen++;
                    if ((ch3 & 1) == 1) // Base Displacement 
                    {
                        if ((ch4 & 0x30) == 0x30) // BD size = 11, long word 
                        {
                            ch5 = br.ReadByte(); ch6 = br.ReadByte(); ch7 = br.ReadByte(); ch8 = br.ReadByte();
                            trgdata = (long)ch5 * 16777216L + (long)ch6 * 65536L + (long)ch7 * 256 + ch8;
                            if (find_symbol(out vari, trgdata) == 1)
                            {
                                // DEBUG: printf("\nwithin SRAM!\n");
                                char ichar1 = ((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                                int itemp1 = ((ch3 & 0x70) >> 4);
                                int itemp2 = 1 << ((ch3 & 6) >> 1);
                                //sprintf(dest,"(%s+%c%u*%u)\0",vari,((ch3&0x80)==0x80?'A':'D'),((ch3&0x70)>>4),1<<((ch3&6)>>1)); 
                                dest = "(" + vari + "+" + ichar1 + itemp1.ToString() + "*" + itemp2.ToString();
                                ilen += 2;
                                destaddr = trgdata;
                            }
                            else
                            {
                                //sprintf(dest,"(%08lX + ...).L\0",trgdata); 
                                dest = "(" + trgdata.ToString("X8") + " + ...).L";
                                ilen += 2;
                            }
                        }
                        else if ((ch4 & 0x30) == 0x20) // BD size = 10, word 
                        {
                            ch5 = br.ReadByte(); ch6 = br.ReadByte();
                            trgdata = (long)ch5 * 256 + ch6;
                            //sprintf(dest,"(%04lX + ...).W\0",trgdata); ilen++;
                            dest = "(" + trgdata.ToString("X4") + " + ...).W";
                            ilen++;
                        }
                        else if ((ch4 & 0x30) == 0x10) // BD size = 01, null 
                        {
                            //sprintf(dest,"(%04lX + ...).W\0",trgdata);
                            dest = "(" + trgdata.ToString("X4") + " + ...).W";
                        }
                    }
                    else // single word extension (d8,An,Xn.SIZE*SCALE) 
                    {
                        char ichar1 = ((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                        int itemp1 = ((ch3 & 0x70) >> 4);
                        char ichar2 = ((ch3 & 0x08) == 0x08 ? 'L' : 'W');
                        int itemp2 = 1 << ((ch3 & 6) >> 1);
                        //sprintf(dest,"(%02X,A%u,%c%u.%c*%u)\0",ch4,dstreg,((ch3&0x80)==0x80?'A':'D'),((ch3&0x70)>>4),((ch3&0x08)==0x08?'L':'W'),1<<((ch3&6)>>1));
                        dest = "(" + ch4.ToString("X2") + ",A" + Convert.ToInt16(dstreg).ToString() + "," + ichar1 + itemp1.ToString() + "." + ichar2 + "*" + itemp2.ToString() + ")";
                    }
                    break;
                case 0x07:
                    switch (dstreg)
                    {
                        case 0x00:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte();
                            trgdata = (long)ch3 * 256 + ch4;
                            //sprintf(dest,"(%04lX).W\0",trgdata); 
                            dest = "(" + trgdata.ToString("X4") + ").W";
                            ilen++;
                            break;
                        case 0x01:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte(); ch5 = br.ReadByte(); ch6 = br.ReadByte();
                            trgdata = (long)ch3 * 16777216L + (long)ch4 * 65536L + (long)ch5 * 256 + ch6;
                            if (find_symbol(out vari, trgdata) == 1)
                            {
                                //sprintf(dest,"(%s)\0",vari); 
                                dest = "(" + vari + ")";
                                ilen += 2;
                                destaddr = trgdata;
                            }
                            else
                            {
                                //sprintf(dest,"(%08lX).L\0",trgdata); 
                                dest = "(" + trgdata.ToString("X8") + ").L";
                                ilen += 2;
                            }
                            break;
                        case 0x02:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte();
                            disp = (long)ch4;
                            disp += (long)(ch3) << 8;
                            if ((disp & 0x8000) == 0) disp = (0x00007FFF & disp);
                            else disp = (0xFFFF8000 | disp);
                            trgdata = addr + disp + 2 + offset;
                            //	      sprintf(dest,"(%08lX)\0",trgdata); 
                            dest = "(" + trgdata.ToString("X8") + ")";
                            ilen++;
                            break;
                        case 0x03:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte(); ilen++;
                            if ((ch3 & 1) == 1) // Base Displacement 
                            {
                                if ((ch4 & 0xc0) == 0) // Supress BS & IS 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0x70) // Supress Index Operand 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0x80) // Suppress Base Register 
                                {
                                }
                                else if ((ch4 & 0xc0) == 0xc0) // BS & IS active
                                {
                                }
                            }
                            else // (d8,PC,Xn.SIZE*SCALE) 
                            {
                                disp = (long)ch4;
                                if ((disp & 0x80) == 0) disp = (0x0000007F & disp);
                                else disp = (0xFFFFFF80 | disp);
                                trgdata = addr + disp + 2 + offset;
                                int itemp1 = (ch3 & 70) >> 4;
                                int itemp2 = 1 << ((ch3 & 6) >> 1);
                                char ichar1 = ((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                                char ichar2 = ((ch3 & 0x08) == 0x08 ? 'L' : 'W');
                                //sprintf(dest,"%08lX(PC,%c%u.%c*%u)\0",trgdata,((ch3&0x80)==0x80?'A':'D'),(ch3&70)>>4,((ch3&0x08)==0x08?'L':'W'),1<<((ch3&6)>>1));
                                dest = trgdata.ToString("X8") + "(PC," + ichar1 + itemp1.ToString() + "." + ichar2 + "*" + itemp2.ToString() + ")";
                            }
                            break;
                    }
                    break;
            }
            return (ilen);
        }

        unsafe uint build_displacement(long* trgaddr, long addr, long offset, byte ch2, BinaryReader br)
        {
            uint ilen = 0;
            byte ch3, ch4, ch5, ch6;
            long trgdata = 0;
            long disp = 0;

            if ((ch2 > 0) && (ch2 < 0xff))
            {
                disp = (long)ch2;
                if ((disp & 0x80) == 0) disp = (0x0000007F & disp);
                else disp = (0xFFFFFF80 | disp);
            }
            else if (ch2 == 0)
            {
                ch3 = br.ReadByte(); ch4 = br.ReadByte();
                disp = (long)ch4;
                disp += (long)(ch3) << 8;
                if ((disp & 0x8000) == 0) disp = (0x00007FFF & disp);
                else disp = (0xFFFF8000 | disp);
                ilen++;
            }
            else if (ch2 == 0xff)
            {
                ch3 = br.ReadByte(); ch4 = br.ReadByte(); ch5 = br.ReadByte(); ch6 = br.ReadByte();
                disp = (long)ch6;
                disp += (long)(ch5) << 8;
                disp += (long)(ch4) << 16;
                disp += (long)(ch3) << 24;
                if ((disp & 0x8000000) == 0) disp = (0x7FFFFFFF & disp);
                else disp = (0x80000000 | disp);
                ilen += 2;
            }
            *trgaddr = addr + disp + 2 + offset;
            return (ilen);
        }

        byte mapsize(byte input)
        {
            if (input == 0) return ((byte)1);
            if (input == 1) return ((byte)3);
            if (input == 2) return ((byte)2);
            return 0;
        }

        int find_symbol(out string symbol, long caddr)
        {
            int retval = 0;
            symbol = string.Empty;
            foreach(SymbolHelper sh in m_symbols)
            {
                if (caddr != 0 && sh.Flash_start_address == caddr)
                {
                    symbol = "ROM_" + sh.SmartVarname;
                    retval = 1;
                }
                else if (caddr != 0 && sh.Start_address == caddr)
                {
                    symbol = "RAM_" + sh.SmartVarname;
                    retval = 1;
                }
            }
            if (symbol == string.Empty)
            {
                // http://cache.freescale.com/files/microcontrollers/doc/ref_manual/MC68377RM.pdf
                // APPENDIX A INTERNAL MEMORY MAP
                if (caddr == 0x100000) symbol = "RAM_FSRAM";
                else if (caddr >= 0xFFF000 && caddr < 0xFFF100)
                {
                    long seg_base = caddr - 0xFFF000;
                    symbol += "QADC64.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "QADC64MCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "QADC64TEST";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "QADC64INT";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "PORTQA/PORTQB";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "DDRQA";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "QACR0";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "QACR1";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "QACR2";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "QASR0";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "QASR1";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF100 && caddr < 0xFFF200) symbol = "TouCAN+0x" + (caddr - 0xFFF100).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF300 && caddr < 0xFFF400)
                {
                    long seg_base = caddr - 0xFFF300;
                    symbol += "QADC64.";
                    if (seg_base >= 0x0 && seg_base < 0x7F) symbol += "CCW";
                    else if (seg_base >= 0x80 && seg_base < 0xFE) symbol += "RJURR";
                    else if (seg_base >= 0x0 && seg_base < 0x7E) symbol += "LJSRR";
                    else if (seg_base >= 0x80 && seg_base < 0xFE) symbol += "LJURR";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF400 && caddr < 0xFFF600)
                {
                    long seg_base = caddr - 0xFFF400;
                    symbol = "QSM_B.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "QSMCR_B";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "QTEST_B";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "QILR/QIVR_B";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "SCCR0_B";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "SCCR1_B";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "SCSR_B";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "SCDR_B";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "PORTQS_B";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "PQSPAR_B/DDRQST_B";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "SPCR0_B";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "SPCR1_B";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "SPCR2_B";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "SPCR3_B/SPSR_B";
                    else if (seg_base >= 0x100 && seg_base < 0x11F) symbol += "SCRQ_B";
                    else if (seg_base >= 0x120 && seg_base < 0x13F) symbol += "SCTQ_B";
                    else if (seg_base >= 0x140 && seg_base < 0x14F) symbol += "QSPIRAM_B";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF600 && caddr < 0xFFF610) symbol = "DLCMD+0x" + (caddr - 0xFFF600).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF610 && caddr < 0xFFF660) symbol = "RESERVED_A+0x" + (caddr - 0xFFF610).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF660 && caddr < 0xFFF668) symbol = "SRAM_A+0x" + (caddr - 0xFFF660).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF668 && caddr < 0xFFF670) symbol = "SRAM_B+0x" + (caddr - 0xFFF668).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF670 && caddr < 0xFFF678) symbol = "SRAM_C+0x" + (caddr - 0xFFF670).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF678 && caddr < 0xFFF680) symbol = "SRAM_D+0x" + (caddr - 0xFFF678).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF680 && caddr < 0xFFF6C0)
                {
                    long seg_base = caddr - 0xFFF680;
                    symbol += "DPTRAM.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "DPTMCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "DPTTCR";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "DPTBAR";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "MISRH";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "MISRL";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "MISCNT";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF6C0 && caddr < 0xFFF6E0)
                {
                    long seg_base = caddr - 0xFFF6C0;
                    symbol += "FASRAM.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "FMCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "FTEST";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "FBAR-H";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "FBAR-L";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "FCCR0";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "FCCR1";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "FMATCH";
                    //else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "FSTATUS";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF6E0 && caddr < 0xFFF700) symbol = "RESERVED_B+0x" + (caddr - 0xFFF6E0).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF700 && caddr < 0xFFF800)
                {
                    long seg_base = caddr - 0xFFF700;
                    symbol += "CTM9.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "BIUMCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "BIUTEST";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "BIUTBR";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "CPCR";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "CPTR";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "MCSM2SIC";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "MCSM2CNT";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "MCSM2ML";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "DASM3SIC";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "DASM3A";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "DASM3B";
                    else if (seg_base >= 0x20 && seg_base < 0x22) symbol += "DASM4SIC";
                    else if (seg_base >= 0x22 && seg_base < 0x24) symbol += "DASM4A";
                    else if (seg_base >= 0x24 && seg_base < 0x26) symbol += "DASM4B";
                    else if (seg_base >= 0x28 && seg_base < 0x2A) symbol += "PWM5SIC";
                    else if (seg_base >= 0x2A && seg_base < 0x2C) symbol += "PWM5A";
                    else if (seg_base >= 0x2C && seg_base < 0x2E) symbol += "PWM5B";
                    else if (seg_base >= 0x2E && seg_base < 0x30) symbol += "PWM5C";
                    else if (seg_base >= 0x30 && seg_base < 0x32) symbol += "PWM6SIC";
                    else if (seg_base >= 0x32 && seg_base < 0x34) symbol += "PWM6A";
                    else if (seg_base >= 0x34 && seg_base < 0x36) symbol += "PWM6B";
                    else if (seg_base >= 0x36 && seg_base < 0x38) symbol += "PWM6C";
                    else if (seg_base >= 0x38 && seg_base < 0x3A) symbol += "PWM7SIC";
                    else if (seg_base >= 0x3A && seg_base < 0x3C) symbol += "PWM7A";
                    else if (seg_base >= 0x3C && seg_base < 0x3E) symbol += "PWM7B";
                    else if (seg_base >= 0x3E && seg_base < 0x40) symbol += "PWM7C";
                    else if (seg_base >= 0x40 && seg_base < 0x42) symbol += "PWM8SIC";
                    else if (seg_base >= 0x42 && seg_base < 0x44) symbol += "PWM8A";
                    else if (seg_base >= 0x44 && seg_base < 0x46) symbol += "PWM8B";
                    else if (seg_base >= 0x46 && seg_base < 0x48) symbol += "PWM8C";
                    else if (seg_base >= 0x48 && seg_base < 0x4A) symbol += "DASM9SIC";
                    else if (seg_base >= 0x4A && seg_base < 0x4C) symbol += "DASM9A";
                    else if (seg_base >= 0x4C && seg_base < 0x4E) symbol += "DASM9B";
                    else if (seg_base >= 0x50 && seg_base < 0x52) symbol += "DASM10SIC";
                    else if (seg_base >= 0x52 && seg_base < 0x54) symbol += "DASM10A";
                    else if (seg_base >= 0x54 && seg_base < 0x56) symbol += "DASM10B";
                    else if (seg_base >= 0x58 && seg_base < 0x5A) symbol += "MCSM11SIC";
                    else if (seg_base >= 0x5A && seg_base < 0x5C) symbol += "MCSM11CNT";
                    else if (seg_base >= 0x5C && seg_base < 0x5E) symbol += "MCSM11ML";
                    else if (seg_base >= 0x60 && seg_base < 0x62) symbol += "FCSMSIC";
                    else if (seg_base >= 0x62 && seg_base < 0x64) symbol += "FCSMCNT";
                    else if (seg_base >= 0x70 && seg_base < 0x72) symbol += "S14ICA";
                    else if (seg_base >= 0x72 && seg_base < 0x74) symbol += "S14DATA";
                    else if (seg_base >= 0x74 && seg_base < 0x76) symbol += "S14ICB";
                    else if (seg_base >= 0x76 && seg_base < 0x78) symbol += "S14DATB";
                    else if (seg_base >= 0x80 && seg_base < 0x82) symbol += "S16ICA";
                    else if (seg_base >= 0x82 && seg_base < 0x84) symbol += "S16DATA";
                    else if (seg_base >= 0x84 && seg_base < 0x86) symbol += "S16ICB";
                    else if (seg_base >= 0x86 && seg_base < 0x88) symbol += "S16DATB";
                    else if (seg_base >= 0x90 && seg_base < 0x92) symbol += "S18ICA";
                    else if (seg_base >= 0x92 && seg_base < 0x94) symbol += "S18DATA";
                    else if (seg_base >= 0x94 && seg_base < 0x96) symbol += "S18ICB";
                    else if (seg_base >= 0x96 && seg_base < 0x98) symbol += "S18DATB";
                    else if (seg_base >= 0xA0 && seg_base < 0xA2) symbol += "S20ICA";
                    else if (seg_base >= 0xA2 && seg_base < 0xA4) symbol += "S20DATA";
                    else if (seg_base >= 0xA4 && seg_base < 0xA6) symbol += "S20ICB";
                    else if (seg_base >= 0xA6 && seg_base < 0xA8) symbol += "S20DATB";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF800 && caddr < 0xFFF840)
                {
                    long seg_base = caddr - 0xFFF800;
                    symbol += "TPU3_B.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "TPUMCR_B";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "TCR_B";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "DSCR_B";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "DSSR_B";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "TICR_B";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "CIER_B";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "CFSR0_B";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "CFSR1_B";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "CFSR2_B";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "CFSR3_B";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "HSQR0_B";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "HSQR1_B";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "HSRR0_B";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "HSRR1_B";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "CPR0_B";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "CPR1_B";
                    else if (seg_base >= 0x20 && seg_base < 0x22) symbol += "CISR_B";
                    else if (seg_base >= 0x22 && seg_base < 0x24) symbol += "LR_B";
                    else if (seg_base >= 0x24 && seg_base < 0x26) symbol += "SGLR_B";
                    else if (seg_base >= 0x26 && seg_base < 0x28) symbol += "DCNR_B";
                    else if (seg_base >= 0x28 && seg_base < 0x2A) symbol += "TPUMCR2_B";
                    else if (seg_base >= 0x2A && seg_base < 0x2C) symbol += "TPUMCR3_B";
                    else if (seg_base >= 0x2C && seg_base < 0x2E) symbol += "ISDR_B";
                    else if (seg_base >= 0x2E && seg_base < 0x30) symbol += "ISCR_B";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFF840 && caddr < 0xFFF848) symbol = "SRAM_A+0x" + (caddr - 0xFFF840).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF848 && caddr < 0xFFF850) symbol = "SRAM_B+0x" + (caddr - 0xFFF848).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF850 && caddr < 0xFFF858) symbol = "SRAM_C+0x" + (caddr - 0xFFF850).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF858 && caddr < 0xFFF900) symbol = "SRAM_D+0x" + (caddr - 0xFFF858).ToString("X3"); // unused in T8
                else if (caddr >= 0xFFF900 && caddr < 0xFFFA00)
                {
                    long seg_base = caddr - 0xFFF900;
                    symbol = "TPU3_B.";
                    if (seg_base >= 0x0 && seg_base < 0x10F) symbol += "Ch 0 param registers";
                    else if (seg_base >= 0x10 && seg_base < 0x1F) symbol += "Ch 1 param registers";
                    else if (seg_base >= 0x20 && seg_base < 0x2F) symbol += "Ch 2 param registers";
                    else if (seg_base >= 0x30 && seg_base < 0x3F) symbol += "Ch 3 param registers";
                    else if (seg_base >= 0x40 && seg_base < 0x4F) symbol += "Ch 4 param registers";
                    else if (seg_base >= 0x50 && seg_base < 0x5F) symbol += "Ch 5 param registers";
                    else if (seg_base >= 0x60 && seg_base < 0x6F) symbol += "Ch 6 param registers";
                    else if (seg_base >= 0x70 && seg_base < 0x7F) symbol += "Ch 7 param registers";
                    else if (seg_base >= 0x80 && seg_base < 0x8F) symbol += "Ch 8 param registers";
                    else if (seg_base >= 0x90 && seg_base < 0x9F) symbol += "Ch 9 param registers";
                    else if (seg_base >= 0xA0 && seg_base < 0xAF) symbol += "Ch 10 param registers";
                    else if (seg_base >= 0xB0 && seg_base < 0xBF) symbol += "Ch 11 param registers";
                    else if (seg_base >= 0xC0 && seg_base < 0xCF) symbol += "Ch 12 param registers";
                    else if (seg_base >= 0xD0 && seg_base < 0xDF) symbol += "Ch 13 param registers";
                    else if (seg_base >= 0xE0 && seg_base < 0xEF) symbol += "Ch 14 param registers";
                    else if (seg_base >= 0xF0 && seg_base < 0xFF) symbol += "Ch 15 param registers";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFFA00 && caddr < 0xFFFA80)
                {
                    long seg_base = caddr - 0xFFFA00;
                    symbol = "BIM.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "MCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "MTR";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "BIMTR/MDR";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "RESERVED";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "SYNCR";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "SYNST/RSR";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "RESERVED";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "PCON";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "PORTA/PORTB";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "PORTAP/PORTBP";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "DDRAB";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "RESERVED";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "PORTC/PORTD";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "PORTCP/PORTDP";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "DDRC/DDRD";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "PCPAR/PDPAR";
                    else if (seg_base >= 0x20 && seg_base < 0x22) symbol += "PORTK/PORTE";
                    else if (seg_base >= 0x22 && seg_base < 0x24) symbol += "PORTKP/PORTEP";
                    else if (seg_base >= 0x24 && seg_base < 0x26) symbol += "DDRK/DDRE";
                    else if (seg_base >= 0x26 && seg_base < 0x28) symbol += "PKPAR/PEPAR";
                    else if (seg_base >= 0x28 && seg_base < 0x2A) symbol += "PORTG/PORTH";
                    else if (seg_base >= 0x2A && seg_base < 0x2C) symbol += "PORTGP/PORTHP";
                    else if (seg_base >= 0x2C && seg_base < 0x2E) symbol += "DDRG/DDRH";
                    else if (seg_base >= 0x2E && seg_base < 0x30) symbol += "RESERVED";

                    else if (seg_base >= 0x30 && seg_base < 0x32) symbol += "PORTF";
                    else if (seg_base >= 0x32 && seg_base < 0x34) symbol += "PORTFP";
                    else if (seg_base >= 0x34 && seg_base < 0x36) symbol += "DDRF";
                    else if (seg_base >= 0x36 && seg_base < 0x38) symbol += "PFPAR";
                    else if (seg_base >= 0x38 && seg_base < 0x3A) symbol += "PORTFE";
                    else if (seg_base >= 0x3A && seg_base < 0x3C) symbol += "PFIACK/PFEER";
                    else if (seg_base >= 0x3C && seg_base < 0x3E) symbol += "PFLVR";
                    else if (seg_base >= 0x3E && seg_base < 0x40) symbol += "RESERVED";

                    else if (seg_base >= 0x40 && seg_base < 0x42) symbol += "MSRA";
                    else if (seg_base >= 0x42 && seg_base < 0x44) symbol += "MSRB";
                    else if (seg_base >= 0x44 && seg_base < 0x46) symbol += "SCRA";
                    else if (seg_base >= 0x46 && seg_base < 0x48) symbol += "REPS";
                    else if (seg_base >= 0x48 && seg_base < 0x4A) symbol += "CREG";
                    else if (seg_base >= 0x4A && seg_base < 0x4C) symbol += "DREG";
                    else if (seg_base >= 0x4C && seg_base < 0x4E) symbol += "BCSOR2";
                    else if (seg_base >= 0x4E && seg_base < 0x50) symbol += "RESERVED";

                    else if (seg_base >= 0x50 && seg_base < 0x52) symbol += "SYPCR";
                    else if (seg_base == 0x52) symbol += "TIC";
                    else if (seg_base == 0x53) symbol += "TIV";
                    else if (seg_base == 0x54) symbol += "RESERVED";
                    else if (seg_base == 0x55) symbol += "SWS";
                    else if (seg_base == 0x56) symbol += "PRE";
                    else if (seg_base == 0x58) symbol += "SWI";
                    else if (seg_base == 0x5A) symbol += "RTI";
                    else if (seg_base == 0x5C) symbol += "SWIT";
                    else if (seg_base == 0x5E) symbol += "RTDC";
                    else if (seg_base == 0x5F) symbol += "RTIT";

                    else if (seg_base >= 0x60 && seg_base < 0x62) symbol += "CSBAR1";
                    else if (seg_base >= 0x62 && seg_base < 0x64) symbol += "CSOR1";
                    else if (seg_base >= 0x64 && seg_base < 0x66) symbol += "CSBAR2";
                    else if (seg_base >= 0x66 && seg_base < 0x68) symbol += "CSOR2";
                    else if (seg_base >= 0x68 && seg_base < 0x6A) symbol += "CSBAR3";
                    else if (seg_base >= 0x6A && seg_base < 0x6C) symbol += "CSOR3";
                    else if (seg_base >= 0x6C && seg_base < 0x6E) symbol += "CSBAR4";
                    else if (seg_base >= 0x6E && seg_base < 0x70) symbol += "CSOR4";

                    else if (seg_base >= 0x70 && seg_base < 0x72) symbol += "CSBAR5";
                    else if (seg_base >= 0x72 && seg_base < 0x74) symbol += "CSOR5";
                    else if (seg_base >= 0x74 && seg_base < 0x76) symbol += "CSBAR6";
                    else if (seg_base >= 0x76 && seg_base < 0x78) symbol += "CSOR6";
                    else if (seg_base >= 0x78 && seg_base < 0x7A) symbol += "CSBAR7";
                    else if (seg_base >= 0x7A && seg_base < 0x7C) symbol += "CSOR7";
                    else if (seg_base >= 0x7C && seg_base < 0x7E) symbol += "BCSBAR";
                    else if (seg_base >= 0x7E && seg_base < 0x80) symbol += "BCSOR1";

                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFFA80 && caddr < 0xFFFB00)
                {
                    long seg_base = caddr - 0xFFFA80;
                    symbol = "TouCAN.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "TCNMCR";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "TTR";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "CANICR";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "CANCTRL0/CANCTRL1";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "PRESDIV/CANCTRL2";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "TIMER";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "RXGMASKHI";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "RXGMASKLO";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "RX14MASKHI";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "RX14MASKLO";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "RX15MASKHI";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "RX15MASKLO";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "ESTAT";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "IMASK";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "IFLAG";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "RXECTR/TXECTR";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFFB00 && caddr < 0xFFFC00)
                {
                    long seg_base = caddr - 0xFFFB00;
                    symbol = "SRAM_E.";
                    if (0x0 == seg_base) symbol += "RAMMCR";
                    else if (0x2 == seg_base) symbol += "RAMTST";
                    else if (0x4 == seg_base) symbol += "RAMBAH";
                    else if (0x6 == seg_base) symbol += "RAMBAL";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFFC00 && caddr < 0xFFFE00)
                {
                    long seg_base = caddr - 0xFFFC00;
                    symbol = "QSM_A.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "QSMCR_A";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "QTEST_A";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "QILR/QIVR_A";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "SCCR0_A";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "SCCR1_A";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "SCSR_A";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "SCDR_A";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "PORTQS_A";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "PQSPAR_A/DDRQST_A";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "SPCR0_A";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "SPCR1_A";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "SPCR2_A";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "SPCR3_A/SPSR_A";
                    else if (seg_base >= 0x100 && seg_base < 0x11F) symbol += "SCRQ_A";
                    else if (seg_base >= 0x120 && seg_base < 0x13F) symbol += "SCTQ_A";
                    else if (seg_base >= 0x140 && seg_base < 0x14F) symbol += "QSPIRAM_A";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
                else if (caddr >= 0xFFFE00 && caddr < 0xFFFFFF)
                {
                    long seg_base = caddr - 0xFFFE00;
                    symbol = "TPU3_A.";
                    if (seg_base >= 0x0 && seg_base < 0x2) symbol += "TPUMCR_A";
                    else if (seg_base >= 0x2 && seg_base < 0x4) symbol += "TCR_A";
                    else if (seg_base >= 0x4 && seg_base < 0x6) symbol += "DSCR_A";
                    else if (seg_base >= 0x6 && seg_base < 0x8) symbol += "DSSR_A";
                    else if (seg_base >= 0x8 && seg_base < 0xA) symbol += "TICR_A";
                    else if (seg_base >= 0xA && seg_base < 0xC) symbol += "CIER_A";
                    else if (seg_base >= 0xC && seg_base < 0xE) symbol += "CFSR0_A";
                    else if (seg_base >= 0xE && seg_base < 0x10) symbol += "CFSR1_A";
                    else if (seg_base >= 0x10 && seg_base < 0x12) symbol += "CFSR2_A";
                    else if (seg_base >= 0x12 && seg_base < 0x14) symbol += "CFSR3_A";
                    else if (seg_base >= 0x14 && seg_base < 0x16) symbol += "HSQR0_A";
                    else if (seg_base >= 0x16 && seg_base < 0x18) symbol += "HSQR1_A";
                    else if (seg_base >= 0x18 && seg_base < 0x1A) symbol += "HSRR0_A";
                    else if (seg_base >= 0x1A && seg_base < 0x1C) symbol += "HSRR1_A";
                    else if (seg_base >= 0x1C && seg_base < 0x1E) symbol += "CPR0_A";
                    else if (seg_base >= 0x1E && seg_base < 0x20) symbol += "CPR1_A";
                    else if (seg_base >= 0x20 && seg_base < 0x22) symbol += "CISR_A";
                    else if (seg_base >= 0x22 && seg_base < 0x24) symbol += "LR_A";
                    else if (seg_base >= 0x24 && seg_base < 0x26) symbol += "SGLR_A";
                    else if (seg_base >= 0x26 && seg_base < 0x28) symbol += "DCNR_A";
                    else if (seg_base >= 0x28 && seg_base < 0x2A) symbol += "TPUMCR2_A";
                    else if (seg_base >= 0x2A && seg_base < 0x2C) symbol += "TPUMCR3_A";
                    else if (seg_base >= 0x2C && seg_base < 0x2E) symbol += "ISDR_A";
                    else if (seg_base >= 0x2E && seg_base < 0x30) symbol += "ISCR_A";
                    else if (seg_base >= 0x100 && seg_base < 0x10F) symbol += "Ch 0 param registers";
                    else if (seg_base >= 0x110 && seg_base < 0x11F) symbol += "Ch 1 param registers";
                    else if (seg_base >= 0x120 && seg_base < 0x12F) symbol += "Ch 2 param registers";
                    else if (seg_base >= 0x130 && seg_base < 0x13F) symbol += "Ch 3 param registers";
                    else if (seg_base >= 0x140 && seg_base < 0x14F) symbol += "Ch 4 param registers";
                    else if (seg_base >= 0x150 && seg_base < 0x15F) symbol += "Ch 5 param registers";
                    else if (seg_base >= 0x160 && seg_base < 0x16F) symbol += "Ch 6 param registers";
                    else if (seg_base >= 0x170 && seg_base < 0x17F) symbol += "Ch 7 param registers";
                    else if (seg_base >= 0x180 && seg_base < 0x18F) symbol += "Ch 8 param registers";
                    else if (seg_base >= 0x190 && seg_base < 0x19F) symbol += "Ch 9 param registers";
                    else if (seg_base >= 0x1A0 && seg_base < 0x1AF) symbol += "Ch 10 param registers";
                    else if (seg_base >= 0x1B0 && seg_base < 0x1BF) symbol += "Ch 11 param registers";
                    else if (seg_base >= 0x1C0 && seg_base < 0x1CF) symbol += "Ch 12 param registers";
                    else if (seg_base >= 0x1D0 && seg_base < 0x1DF) symbol += "Ch 13 param registers";
                    else if (seg_base >= 0x1E0 && seg_base < 0x1EF) symbol += "Ch 14 param registers";
                    else if (seg_base >= 0x1F0 && seg_base < 0x1FF) symbol += "Ch 15 param registers";
                    else symbol += "+0x" + seg_base.ToString("X3");
                }
            }
            if (symbol != string.Empty) retval = 1;
            return retval;
        }

        /*
int find_symbol(string symbol, long caddr)  // TEST SEQUENCE FOR READING BINARY WRITTEN FILE 
{

  long addr, saddr, trgaddr;
  unsigned int i,finished, seg;
  byte ch1,str[80];

// DEBUG:  printf("\nCame to symbol search!\n"); 

  finished = 0;
  if((TST = fopen("TABLE.TXT","rb")) == NULL) {
    printf("\nCannot open file.");
    exit(0);
  }
  addr = 0L;
  finished = 0;

  while(!feof(TST)) {
    fseek(TST,addr,0);
    i=0;
    do {
      ch1 = getc(TST);
      str[i++] = ch1;
      addr++;
    } while(ch1 != 0);

    if(!strcmp(str,"$END$\0")) break;

    sscanf(str,"%08lX%s",&saddr,symbol);

    // DEBUG: printf("\n%08lX\t%s\n",saddr,symbol); 
 
    if(saddr == caddr) {
      finished = 1;
      // DEBUG: printf("\n%08lX\t%s\n",saddr,symbol); 
      break;
    } 
  }
  fclose(TST); 
  return(finished);
}
        */

        private unsafe uint disasm(out string str, long addr, byte upperbyte, byte lowerbyte, long offset, BinaryReader br, out bool endsub, out bool issub, out bool isjump)
        {
            endsub = false;
            issub = false;
            isjump = false;
            uint i, ilen;       /* word, inst length */
            byte ch1, ch2;     /* Bytes */
            byte ch3, ch4;     /* Extra bytes */
            byte ch5, ch6;     /* Extra bytes */
            byte ch7, ch8;     /* Extra bytes */
            byte n1, n2, n3, n4; /* Nibbles */
            long trgaddr; /* Target address */
            long trgdata; /* Target data */
            long disp;  /* Displacement */
            string sour;
            string dest;
            trgdata = 0L;
            byte itype, dstreg, dstmod, srcmod, srcreg;
            string[] movesize = { "", "B", "L", "W" };
            string[] opsize = { "B", "W", "L", "" };
            string[] ctregs = { "SFC", "DFC", "USP", "VBR" };
            string[] ccodes = { "T", "F", "HI", "LS", "CC", "CS", "NE", "EQ", "VC", "VS", "PL", "MI", "GE", "LT", "GT", "LE" };
            uint err;
            str = string.Empty;
            err = 0; ilen = 0;
            ch1 = upperbyte;
            ch2 = lowerbyte;
            i = (uint)(ch1 << 8) + ch2;
            n1 = (byte)((ch1 & 0xf0) >> 4);
            n2 = (byte)((ch1 & 15));
            n3 = (byte)((ch2 & 0xf0) >> 4);
            n4 = (byte)((ch2 & 15));
            itype = n1;
            dstreg = (byte)((n2 >> 1) & 0x07);
            dstmod = (byte)((i >> 6) & 0x07);
            srcmod = (byte)((ch2 & 0x38) >> 3);
            srcreg = (byte)(n4 & 0x07);
            switch (itype)
            {
                /* Instruction type 00 */
                case 0x00:
                    if (i == 0x003c)
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        str = "ORI\t" + sour + ",CCR";
                        ilen++;
                    }
                    else if (i == 0x007c)
                    {
                        ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                        str = "ORI\t" + sour + ",SR";
                        ilen++;
                    }
                    else if (dstreg == 0 && dstmod <= 2)
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        str = "ORI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if (dstmod == 3 && dstreg <= 2)
                    {
                        ch3 = br.ReadByte();
                        ch4 = br.ReadByte();
                        ilen++;
                        ilen += build_destination(out sour, 0, srcmod, srcreg, addr, offset, br);
                        err = (uint)((ch3 << 8) + ch4);
                        if ((err & 0x0800) == 0)
                        {
                            int i2 = ((ch3 & 0x70) >> 4);
                            str = "CMP2." + sour + "\t" + (string)opsize.GetValue(dstreg & 3) + ((ch3 & 0x80) == 0x80 ? 'D' : 'A') + i2.ToString();
                            ilen++;
                        }
                        else
                        {
                            int i2 = ((ch3 & 0x70) >> 4);
                            str = "CHK2." + sour + "\t" + (string)opsize.GetValue(dstreg & 3) + ((ch3 & 0x80) == 0x80 ? 'D' : 'A') + i2.ToString();
                            ilen++;
                        }
                        err = 0;
                    }
                    else if (dstmod >= 4 && dstmod <= 7 && srcmod == 1)
                    {
                        ch3 = br.ReadByte();
                        ch4 = br.ReadByte();
                        ilen++;
                        err = (uint)((ch3 << 8) + ch4);
                        if ((dstmod & 2) == 2)
                        {
                            str = "MOVEP." + (string)opsize.GetValue(dstmod & 3) + "\tD" + Convert.ToInt16(dstreg).ToString() + ",(#," + err.ToString("X4") + ",A" + Convert.ToInt16(srcreg).ToString() + ")";
                            ilen++;
                        }
                        else
                        {
                            str = "MOVEP." + (string)opsize.GetValue(dstmod & 3) + "\t(#" + err.ToString("X4") + ",A" + Convert.ToInt16(srcreg).ToString() + "),D" + Convert.ToInt16(dstreg).ToString();
                            //       sprintf(str,"MOVEP.%c\t(#%04X,A%u),D%u\0",opsize[dstmod&3][0],err,srcreg,dstreg); 
                            ilen++;
                        }
                        err = 0;
                    }
                    else if (dstmod == 4)
                    {
                        ilen += build_source(out sour, 1, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"BTST\tD%u,%s\0",dstreg,sour); 
                        str = "BTST\tD" + Convert.ToInt16(dstreg).ToString() + "," + sour;
                        ilen++;
                    }
                    else if (dstmod == 5)
                    {
                        ilen += build_source(out sour, 1, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"BCHG\tD%u,%s\0",dstreg,sour); 
                        str = "BCHG\tD" + Convert.ToInt16(dstreg).ToString() + "," + sour;
                        ilen++;
                    }
                    else if (dstmod == 6)
                    {
                        ilen += build_source(out sour, 1, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"BCLR\tD%u,%s\0",dstreg,sour); 
                        str = "BCLR\tD" + Convert.ToInt16(dstreg).ToString() + "," + sour;
                        ilen++;
                    }
                    else if (dstmod == 7)
                    {
                        ilen += build_source(out sour, 1, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"BSET\tD%u,%s\0",dstreg,sour); 
                        str = "BSET\tD" + Convert.ToInt16(dstreg).ToString() + "," + sour;
                        ilen++;
                    }
                    else if (i == 0x023c)
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        //sprintf(str,"ANDI\t%s,CCR\0",sour); 
                        str = "ANDI\t" + sour + ",CCR";
                        ilen++;
                    }
                    else if (i == 0x027c)
                    {
                        ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                        //sprintf(str,"ANDI\t%s,SR\0",sour); 
                        str = "ANDI\t" + sour + ",SR";
                        ilen++;
                    }
                    else if ((dstreg == 1) && (dstmod <= 2))
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        str = "ANDI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 2) && (dstmod <= 2))
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"SUBI.%c\t%s,%s\0",opsize[dstmod&3][0],sour,dest); 
                        str = "SUBI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 3) && (dstmod <= 2))
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //   sprintf(str, "ADDI.%c\t%s,%s\0", opsize[dstmod & 3][0], sour, dest);
                        str = "ADDI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 4) && (dstmod == 0))
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "BTST\t%s,%s\0", sour, dest); 
                        str = "BTST\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 4) && (dstmod == 1))
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "BCHG\t%s,%s\0", sour, dest); 
                        str = "BCHG\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 4) && (dstmod == 2))
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "BCLR\t%s,%s\0", sour, dest); 
                        str = "BCLR\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 4) && (dstmod == 3))
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //                    sprintf(str, "BSET\t%s,%s\0", sour, dest); 
                        str = "BSET\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if (i == 0x0a3c)
                    {
                        ilen += build_source(out sour, 1, 7, 4, addr, offset, br);
                        //sprintf(str, "EORI\t%s,CCR\0", sour); 
                        str = "EORI\t" + sour + ",CCR";
                        ilen++;
                    }
                    else if (i == 0x0a7c)
                    {
                        ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                        //sprintf(str, "EORI\t%s,SR\0", sour); 
                        str = "EORI\t" + sour + ",SR";
                        ilen++;
                    }
                    else if ((dstreg == 5) && (dstmod <= 2))
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "EORI.%c\t%s,%s\0", opsize[dstmod & 3][0], sour, dest); 
                        str = "EORI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 6) && (dstmod <= 2))
                    {
                        ilen += build_source(out sour, mapsize((byte)(dstmod & 3)), 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "CMPI.%c\t%s,%s\0", opsize[dstmod & 3][0], sour, dest); ilen++;
                        str = "CMPI." + (string)opsize.GetValue(dstmod & 3) + "\t" + sour + "," + dest;
                        ilen++;
                    }
                    else if ((dstreg == 7) && (dstmod <= 2))
                    {
                        ch3 = br.ReadByte(); ch4 = br.ReadByte();
                        ilen++;
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        err = (uint)((ch3 << 8) + ch4);
                        if ((err & 0x0800) == 0x0800)
                        {
                            //((ch3 & 0x80) == 0x80 ? 'A' : 'D');
                            char ichar1 = ((ch3 & 0x80) > 0 ? 'D' : 'A');
                            int itemp1 = ((ch3 & 0x70) >> 4);
                            //sprintf(str, "MOVES.%c\t%c%u,%s\0", opsize[dstreg & 3][0], (ch3 && 0x80 ? 'D' : 'A'), ((ch3 & 0x70) >> 4), &ctregs[err][0]); 
                            str = "MOVES." + (string)opsize.GetValue(dstmod & 3) + "\t" + ichar1 + itemp1.ToString() + "," + (string)ctregs.GetValue(err);
                            ilen++;
                        }
                        else
                        {
                            char ichar1 = ((ch3 & 0x80) > 0 ? 'D' : 'A');
                            int itemp1 = ((ch3 & 0x70) >> 4);

                            ///sprintf(str, "MOVES.%c\t%s,%c%u\0", opsize[dstreg & 3][0], &ctregs[err][0], (ch3 && 0x80 ? 'D' : 'A'), ((ch3 & 0x70) >> 4)); 
                            str = "MOVES." + (string)opsize.GetValue(dstmod & 3) + "\t" + (string)ctregs.GetValue(err) + "," + ichar1 + itemp1.ToString();
                            ilen++;
                        }
                        err = 0;
                    }
                    break;

                /* Instruction types 01 - 03 */

                case 0x01:
                case 0x02:
                case 0x03:
                    souraddr = 0L;
                    ilen += build_source(out sour, itype, srcmod, srcreg, addr, offset, br);
                    ilen += build_destination(out dest, itype, dstmod, dstreg, addr, offset, br);
                    switch (dstmod)
                    {
                        case 0x01:
                            //sprintf(str, "MOVEA.%c\t%s,%s\0", movesize[itype][0], sour, dest);
                            str = "MOVEA." + (string)movesize.GetValue(itype) + "\t" + sour + "," + dest;
                            ilen++;
                            if (souraddr != 0) A_reg.SetValue(souraddr, dstreg);
                            else if (srcmod == 1) A_reg.SetValue((long)A_reg.GetValue(srcreg), dstreg);
                            break;
                        default:
                            ///sprintf(str,"MOVE.%c\t%s,%s\0",movesize[itype][0],sour,dest); 
                            str = "MOVE." + (string)movesize.GetValue(itype) + "\t" + sour + "," + dest;
                            ilen++;
                            break;
                    }
                    break;
                // Instruction type 04 

                case 0x04:
                    switch (i)
                    {
                        case 0x4afa:
                            //sprintf(str,"BGND\0"); 
                            str = "BGND";
                            ilen++;
                            break;
                        case 0x4afc:
                            //sprintf(str,"ILLEGAL\0"); 
                            str = "ILLEGAL";
                            ilen++;
                            break;
                        case 0x4e70:
                            //sprintf(str,"RESET\0"); 
                            str = "RESET";
                            ilen++;
                            break;
                        case 0x4e71:
                            //sprintf(str,"NOP\0"); 
                            str = "NOP";
                            ilen++;
                            break;
                        case 0x4e72:
                            ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                            //sprintf(str,"STOP\t%s",sour); 
                            endsub = true;
                            str = "STOP\t" + sour;
                            ilen++;
                            break;
                        case 0x4e73:
                            //sprintf(str,"RTE\n\n\n\0"); 
                            endsub = true;
                            str = "RTE\n\n\n";
                            ilen++;
                            break;
                        case 0x4e74:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte();
                            ilen++;
                            err = (uint)(ch3 << 8) + ch4;
                            //sprintf(str,"RTD\t#%04X\0",err); 
                            endsub = true;
                            str = "RTD\t#" + err.ToString("X4");
                            ilen++;
                            break;
                        case 0x4e75:
                            //sprintf(str,"RTS\n\n\n\0");
                            endsub = true;
                            str = "RTS\n\n\n";
                            ilen++;
                            break;
                        case 0x4e76:
                            //sprintf(str,"TRAPV\0"); 
                            str = "TRAPV";
                            ilen++;
                            break;
                        case 0x4e77:
                            //sprintf(str,"RTR\0"); 
                            endsub = true;
                            str = "RTR";
                            ilen++;
                            break;
                        case 0x4e7a:
                        case 0x4e7b:
                            ch3 = br.ReadByte(); ch4 = br.ReadByte();
                            ilen++;
                            err = (uint)(ch3 << 8) + ch4;
                            err = err & 0x8FF;
                            if ((err & 0x800) == 0x800) err = err | 2;
                            err = err & 3;
                            if ((i & 1) == 1)
                            {
                                char char1 = ((ch3 & 0x80) > 0 ? 'D' : 'A');
                                int temp1 = ((ch3 & 0x70) >> 4);
                                //sprintf(str,"MOVEC\t%c%u,%s\0",(ch3&&0x80?'D':'A'),((ch3 & 0x70) >> 4),&ctregs[err][0]); 
                                str = "MOVEC\t" + char1 + temp1.ToString() + "," + (string)ctregs.GetValue(err);
                                ilen++;
                            }
                            else
                            {
                                char char1 = ((ch3 & 0x80) > 0 ? 'D' : 'A');
                                int temp1 = ((ch3 & 0x70) >> 4);
                                //sprintf(str,"MOVEC\t%s,%c%u\0",&ctregs[err][0],(ch3&&0x80?'D':'A'),((ch3 & 0x70)>>4)); 
                                str = "MOVEC\t" + (string)ctregs.GetValue(err) + "," + char1 + temp1.ToString();
                                ilen++;
                            }
                            err = 0;
                            break;
                    }
                    if (dstreg == 0 && dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NEGX.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NEGX." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 0 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\tSR,%s\0",dest); 
                        str = "MOVE\tSR," + dest;
                        ilen++;
                    }
                    else if (dstmod == 4 || dstmod == 6)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        char char1 = ((dstmod & 0x20) > 0 ? 'L' : 'W');
                        //sprintf(str,"CHK.%c\t%s,D%u\0",(dstmod&&0x20?'L':'W'),dest,dstreg); 
                        str = "CHK." + char1 + "\t" + dest + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstmod == 7)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"LEA\t%s,A%u\0",dest,dstreg); 
                        str = "LEA\t" + dest + ",A" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstreg == 1 && dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"CLR.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "CLR." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 3 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t%s,SR\0",dest); 
                        str = "MOVE\t" + dest + ",SR";
                        ilen++;
                    }

                    else if (dstreg == 1 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t CCR,%s\0",dest); 
                        str = "MOVE\t CCR," + dest;
                        ilen++;
                    }
                    else if (dstreg == 2 && dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NEG.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NEG." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 2 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t %s,CCR\0",dest);
                        str = "MOVE\t " + dest + ",CCR";
                        ilen++;
                    }
                    else if (dstreg == 3 && dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NOT.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NOT." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 4 && dstmod == 0)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NBCD\t%s\0",dest); 
                        str = "NBCD\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 4 && dstmod == 0 && srcmod == 1)
                    {
                        ilen += build_source(out sour, 2, 7, 4, addr, offset, br);
                        //sprintf(str,"LINK.L\tA%u,#%s\0",srcreg,sour); 
                        str = "LINK.L\tA" + srcreg.ToString() + ",#" + sour;
                        ilen++;
                    }
                    else if (dstreg == 4 && dstmod == 1 && srcmod == 0)
                    {
                        //sprintf(str, "SWAP\tD%u\0", srcreg); 
                        str = "SWAP\tD" + srcreg.ToString();
                        ilen++;
                    }
                    else if (dstreg == 4 && dstmod == 1 && srcmod == 1)
                    {
                        //sprintf(str, "BKPT\t#%X\0", srcreg); 
                        str = "BKPT\t#" + srcreg.ToString("X2");
                        ilen++;
                    }
                    else if (dstreg == 4 && dstmod == 1)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "PEA\t%s\0", dest); 
                        str = "PEA\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 4 && srcmod == 0)
                    {
                        switch (dstmod)
                        {
                            case 0x02:
                                //sprintf(str, "EXT.W\tD%u\0", srcreg); 
                                str = "EXT.W\tD" + srcreg.ToString();
                                ilen++;
                                break;
                            case 0x03:
                                //sprintf(str, "EXT.L\tD%u\0", srcreg);
                                str = "EXT.L\tD" + srcreg.ToString();
                                ilen++;
                                break;
                            case 0x07:
                                //sprintf(str, "EXTB.L\tD%u\0", srcreg); 
                                str = "EXTB.L\tD" + srcreg.ToString();
                                ilen++;
                                break;
                        }
                    }
                    else if ((dstreg == 4 || dstreg == 6) && (dstmod == 2 || dstmod == 3))
                    {
                        ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        if (dstreg == 4)
                        {
                            //sprintf(str, "MOVEM.%c\t%s,%s\0", opsize[(dstmod & 1) + 1][0], sour, dest); 
                            str = "MOVEM." + (string)opsize.GetValue((dstmod & 1) + 1) + "\t" + sour + "," + dest;
                            ilen++;
                        }
                        else
                        {
                            //sprintf(str, "MOVEM.%c\t%s,%s\0", opsize[(dstmod & 1) + 1][0], dest, sour); 
                            str = "MOVEM." + (string)opsize.GetValue((dstmod & 1) + 1) + "\t" + dest + "," + sour;
                            ilen++;
                        }
                    }
                    else if (dstreg == 5 && dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "TST.%c\t%s\0", opsize[dstmod & 7][0], dest); 
                        str = "TST." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 5 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "TAS\t%s\0", dest); 
                        str = "TAS\t" + dest;
                        ilen++;
                    }
                    else if (dstreg == 6 && dstmod == 0)
                    {
                        ch3 = br.ReadByte(); ch4 = br.ReadByte();
                        ilen++;
                        ilen += build_source(out sour, 2, srcmod, srcreg, addr, offset, br);
                        if ((ch3 & 4) == 4)
                        {
                            char char1 = (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S');
                            int temp1 = ch4 & 7;
                            int temp2 = ((ch3 & 0x70) >> 4);

                            //sprintf(str, "MUL%c.L\t%s,D%u:D%u\0", (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S'), sour, ch4 & 7, ((ch3 & 0x70) >> 4)); 
                            str = "MUL" + char1 + ".L\t" + sour + ",D" + temp1.ToString() + ":D" + temp2.ToString();
                            ilen++;
                        }
                        else
                        {
                            char char1 = (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S');
                            int temp2 = ((ch3 & 0x70) >> 4);
                            //sprintf(str, "MUL%c.L\t%s,D%u\0", (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S'), sour, ((ch3 & 0x70) >> 4)); 
                            str = "MUL" + char1 + ".L\t" + sour + ",D" + temp2.ToString();
                            ilen++;
                        }
                    }
                    else if (dstreg == 6 && dstmod == 1)
                    {
                        ch3 = br.ReadByte(); ch4 = br.ReadByte();
                        ilen++;
                        ilen += build_source(out sour, 2, srcmod, srcreg, addr, offset, br);
                        if ((ch3 & 4) == 4)
                        {
                            char char1 = (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S');
                            int temp1 = ch4 & 7;
                            int temp2 = ((ch3 & 0x70) >> 4);

                            //sprintf(str, "DIV%c.L\t%s,D%u:D%u\0", (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S'), sour, ch4 & 7, ((ch3 & 0x70) >> 4)); 
                            str = "DIV" + char1 + ".L\t" + sour + ",D" + temp1.ToString() + ":D" + temp2.ToString();
                            ilen++;
                        }
                        else
                        {

                            char char1 = (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S');
                            int temp2 = ((ch3 & 0x70) >> 4);
                            //sprintf(str, "DIV%c.L\t%s,D%u\0", (((ch3 & 0x8) >> 3) == 0 ? 'U' : 'S'), sour, ((ch3 & 0x70) >> 4)); 
                            str = "DIV" + char1 + ".L\t" + sour + ",D" + temp2.ToString();
                            ilen++;
                        }
                    }
                    else if (dstreg == 7 && dstmod == 1 && (srcmod == 0 || srcmod == 1))
                    {
                        //sprintf(str, "TRAP\t#%X\0", srcreg); 
                        str = "TRAP\t#" + srcreg.ToString("X2");
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 1 && srcmod == 2)
                    {
                        ilen += build_source(out sour, 3, 7, 4, addr, offset, br);
                        //sprintf(str, "LINK.W\tA%u,#%s\0", srcreg, sour); 
                        str = "LINK.W\tA" + srcreg.ToString() + ",#" + sour;
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 1 && srcmod == 3)
                    {
                        //sprintf(str, "UNLK\tA%u\0", srcreg); 
                        str = "UNLK\tA" + srcreg.ToString();
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 1 && srcmod == 4)
                    {
                        //sprintf(str, "MOVE\tA%u,USP\0", srcreg); 
                        str = "MOVE\tA" + srcreg.ToString() + ",USP";
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 1 && srcmod == 5)
                    {
                        //sprintf(str, "MOVE\tUSP,A%u\0", srcreg); 
                        str = "MOVE\tUSP,A" + srcreg.ToString();
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "JSR\t%s\0", dest); 
                        isjump = true;
                        issub = true;

                        /*if (trgdata == 0xFFF2)
                        {
                            logger.Debug("break!");
                        }*/

                        str = "JSR\t" + dest;
                        //logger.Debug(dest);
                        ilen++;
                    }
                    else if (dstreg == 7 && dstmod == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "JMP\t%s\0", dest); 
                        endsub = true; // jump to somewhere else
                        AddLabel(destaddr);
                        isjump = true;
                        //logger.Debug("JUMP SEEND: " + destaddr.ToString("X8"));
                        str = "JMP\t" + dest;
                        ilen++;
                    }
                    break;

                // Instruction type 05 

                case 0x05:
                    if (dstmod <= 2)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        if (dstreg == 0) dstreg = 8;
                        //sprintf(str, "ADDQ.%c\t#%u,%s\0", opsize[dstmod & 3][0], dstreg, dest);
                        str = "ADDQ." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + "," + dest;
                        ilen++;
                    }
                    else if (((ch2 & 0xF8) == 0xc8) && ((dstmod & 3) == 3))
                    {
                        ilen += build_displacement(&trgaddr, addr, offset, 0, br);
                        //sprintf(str, "DB%s\tD%u,%08lX\0", &ccodes[n2][0], srcreg, trgaddr); 
                        str = "DB" + (string)ccodes.GetValue(n2) + "\tD" + srcreg.ToString() + "," + trgaddr.ToString("X8");
                        ilen++;
                    }
                    else if (((ch2 & 0xf8) == 0xf8) && ((dstmod & 3) == 3))
                    {
                        switch (srcreg)
                        {
                            case 0x02:
                                ilen += build_source(out dest, 3, 7, 4, addr, offset, br);
                                //sprintf(str, "TRAP%s.%c \t%s\0", &ccodes[n2][0], opsize[srcreg & 3][0], dest); 
                                str = "TRAP." + (string)ccodes.GetValue(n2) + "." + (string)opsize.GetValue(srcreg & 3) + "\t" + dest;
                                ilen++;
                                break;
                            case 0x03:
                                ilen += build_source(out dest, 2, 7, 4, addr, offset, br);
                                //sprintf(str, "TRAP%s.%c \t%s\0", &ccodes[n2][0], opsize[srcreg & 3][0], dest); 
                                str = "TRAP" + (string)ccodes.GetValue(n2) + "." + (string)opsize.GetValue(srcreg & 3) + "\t" + dest;
                                ilen++;
                                break;
                            case 0x04:
                                //sprintf(str, "TRAP%s\0", &ccodes[n2][0]); 
                                str = "TRAP" + (string)ccodes.GetValue(n2);
                                ilen++;
                                break;
                        }
                    }
                    else if ((dstmod >= 4) && (dstmod <= 6))
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        if (dstreg == 0) dstreg = 8;
                        //sprintf(str, "SUBQ.%c\t#%u,%s\0", opsize[dstmod & 3][0], dstreg, dest); 
                        str = "SUBQ." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + "," + dest;
                        ilen++;
                    }
                    else if ((dstmod & 3) == 3)
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "S%s\t%s\0", &ccodes[n2][0], dest); 
                        str = "S" + (string)ccodes.GetValue(n2) + "\t" + dest;
                        ilen++;
                    }
                    break;

                // Instruction type 06 

                case 0x06:
                    if (n2 == 0)
                    {
                        ilen += build_displacement(&trgaddr, addr, offset, ch2, br);
                        //sprintf(str,"BRA\t%08lX\0",trgaddr); 
                        // add it to the labellist
                        AddLabel(trgaddr);
                        isjump = true;
                        str = "BRA\t" + trgaddr.ToString("X8");
                        ilen++;
                    }
                    else if (n2 == 1)
                    {
                        ilen += build_displacement(&trgaddr, addr, offset, ch2, br);
                        //sprintf(str,"BSR\t%08lX\0",trgaddr); 
                        AddLabel(trgaddr);
                        isjump = true;

                        str = "BSR\t" + trgaddr.ToString("X8");
                        ilen++;
                    }
                    else
                    {
                        ilen += build_displacement(&trgaddr, addr, offset, ch2, br);
                        //sprintf(str,"B%s\t%08lX\0",&ccodes[n2][0],trgaddr); 
                        AddLabel(trgaddr);
                        isjump = true;

                        str = "B" + (string)ccodes.GetValue(n2) + "\t" + trgaddr.ToString("X8");
                        ilen++;
                    }
                    break;

                // Instruction type 07 

                case 0x07:
                    //sprintf(str,"MOVEQ\t#%02X,D%u\0",ch2,dstreg); 
                    str = "MOVEQ\t#" + ch2.ToString("X2") + ",D" + dstreg.ToString();
                    ilen++;
                    break;

                // Instruction type 08 

                case 0x08:
                    if ((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <= 6))
                    {
                        if ((dstmod & 4) == 4)
                        {
                            ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                            //sprintf(str,"OR.%c\tD%u,%s\0",opsize[dstmod&3][0],dstreg,dest);
                            str = "OR." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + "," + dest;
                            ilen++;
                        }
                        else
                        {
                            if (dstmod == 3) err = 3;
                            if (dstmod == 7) err = 2;
                            ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                            if (dstmod == 3) err = 1;
                            if (dstmod == 7) err = 2;
                            //sprintf(str,"OR.%c\t%s,D%u\0",opsize[dstmod&3][0],dest,dstreg); 
                            str = "OR." + (string)opsize.GetValue(dstmod & 3) + "\t" + dest + ",D" + dstreg.ToString();
                            ilen++;
                        }
                    }
                    else if (dstmod == 3)
                    {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"DIVU.W\t%s,D%u\0",sour,dstreg); 
                        str = "DIVU.W\t" + sour + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstmod == 7)
                    {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"DIVS.W\t%s,D%u\0",sour,dstreg); 
                        str = "DIVS.W\t" + sour + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((srcmod == 0 || srcmod == 1) && (dstmod == 4))
                    {
                        if (srcmod == 0)
                        {
                            //sprintf(str,"SBCD.%c\tD%u,D%u\0",opsize[dstmod&3][0],srcreg,dstreg); 
                            str = "SBCD." + (string)opsize.GetValue(dstmod & 3) + "\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                            ilen++;
                        }
                        else
                        {
                            //sprintf(str,"SBCD.%c\t-(A%u),-(A%u)\0",opsize[dstmod&3][0],srcreg,dstreg); 
                            str = "SBCD." + (string)opsize.GetValue(dstmod & 3) + "\t- (A" + srcreg.ToString() + "),-(A" + dstreg.ToString() + ")";
                            ilen++;
                        }
                    }
                    break;

                // Instruction type 09 

                case 0x09:
                    if ((dstmod == 3) || (dstmod == 7))
                    {
                        if (dstmod == 3) err = 3;
                        if (dstmod == 7) err = 2;
                        ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                        if (dstmod == 3) err = 1;
                        if (dstmod == 7) err = 2;
                        //sprintf(str, "SUBA.%c\t%s,A%u\0", opsize[err][0], dest, dstreg);
                        str = "SUBA." + (string)opsize.GetValue(err) + "\t" + dest + ",A" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((srcmod == 0 || srcmod == 1) && (dstmod >= 4 && dstmod <= 6))
                    {
                        if (srcmod == 0)
                        {
                            //sprintf(str, "SUBX.%c\tD%u,D%u\0", opsize[dstmod & 3][0], srcreg, dstreg); 
                            str = "SUBX." + (string)opsize.GetValue(dstmod & 3) + "\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                            ilen++;
                        }
                        else
                        {
                            //sprintf(str, "SUBX.%c\t-(A%u),-(A%u)\0", opsize[dstmod & 3][0], srcreg, dstreg);
                            str = "SUBX." + (string)opsize.GetValue(dstmod & 3) + "\t-(A" + srcreg.ToString() + "),-(A" + dstreg.ToString() + ")";
                            ilen++;
                        }
                    }
                    else if ((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <= 6))
                    {
                        if ((dstmod & 4) == 4)
                        {
                            ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                            //sprintf(str, "SUB.%c\tD%u,%s\0", opsize[dstmod & 3][0], dstreg, dest);
                            str = "SUB." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + "," + dest;
                            ilen++;
                        }
                        else
                        {
                            if (dstmod == 3) err = 3;
                            if (dstmod == 7) err = 2;
                            ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                            if (dstmod == 3) err = 1;
                            if (dstmod == 7) err = 2;
                            //sprintf(str, "SUB.%c\t%s,D%u\0", opsize[dstmod & 3][0], dest, dstreg); 
                            str = "SUB." + (string)opsize.GetValue(dstmod & 3) + "\t" + dest + ",D" + dstreg.ToString();
                            ilen++;
                        }
                    }
                    break;

                // Instruction type 0B 

                case 0x0b:
                    if ((dstmod >= 0) && (dstmod <= 2))
                    {
                        if (dstmod == 0) err = 1;
                        if (dstmod == 1) err = 3;
                        if (dstmod == 2) err = 2;
                        ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "CMP.%c\t%s,D%u\0", opsize[dstmod][0], dest, dstreg); 
                        str = "CMP." + (string)opsize.GetValue(dstmod) + "\t" + dest + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((dstmod == 3) || (dstmod == 7))
                    {
                        if (dstmod == 3) err = 3;
                        if (dstmod == 7) err = 2;
                        ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                        if (dstmod == 3) err = 1;
                        if (dstmod == 7) err = 2;
                        //sprintf(str, "CMPA.%c\t%s,A%u\0", opsize[err][0], dest, dstreg); 
                        str = "CMPA." + (string)opsize.GetValue(err) + "\t" + dest + ",A" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((dstmod >= 4) && (dstmod <= 6))
                    {
                        if (dstmod == 4) err = 1;
                        if (dstmod == 5) err = 3;
                        if (dstmod == 6) err = 2;
                        ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "EOR.%c\t%s,D%u\0", opsize[dstmod][0], dest, dstreg); 
                        str = "EOR." + (string)opsize.GetValue(dstmod) + "\t" + dest + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((srcmod == 1) && (dstmod >= 4 && dstmod <= 6))
                    {
                        //sprintf(str, "CMPM.%c\t(A%u)+,(A%u)+\0", opsize[dstmod & 3][0], srcreg, dstreg); 
                        str = "CMPM." + (string)opsize.GetValue(dstmod & 3) + "\t(A" + srcreg.ToString() + ")+,(A" + dstreg.ToString() + ")+";
                        ilen++;
                    }
                    break;

                // Instruction type 0C 

                case 0x0c:
                    if ((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <= 6))
                    {
                        if ((dstmod & 4) == 4)
                        {
                            ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                            //sprintf(str, "AND.%c\tD%u,%s\0", opsize[dstmod & 3][0], dstreg, dest); 
                            str = "AND." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + "," + dest;
                            ilen++;
                        }
                        else
                        {
                            if (dstmod == 3) err = 3;
                            if (dstmod == 7) err = 2;
                            ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                            if (dstmod == 3) err = 1;
                            if (dstmod == 7) err = 2;
                            //sprintf(str, "AND.%c\t%s,D%u\0", opsize[dstmod & 3][0], dest, dstreg); 
                            str = "AND." + (string)opsize.GetValue(dstmod & 3) + "\t" + dest + ",D" + dstreg.ToString();
                            ilen++;
                        }
                    }
                    else if (dstmod == 3)
                    {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //   sprintf(str, "MULU.W\t%s,D%u\0", sour, dstreg);
                        str = "MULU.W\t" + sour + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstmod == 7)
                    {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //sprintf(str, "MULS.W\t%s,D%u\0", sour, dstreg); 
                        str = "MULS.W\t" + sour + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((srcmod == 0 || srcmod == 1) && (dstmod == 4))
                    {
                        if (srcmod == 0)
                        {
                            //sprintf(str, "ABCD.%c\tD%u,D%u\0", opsize[dstmod & 3][0], srcreg, dstreg);
                            str = "ABCD." + (string)opsize.GetValue(dstmod & 3) + "\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                            ilen++;
                        }
                        else
                        {
                            //sprintf(str, "ABCD.%c\t-(A%u),-(A%u)\0", opsize[dstmod & 3][0], srcreg, dstreg); 
                            str = "ABCD." + (string)opsize.GetValue(dstmod & 3) + "\t-(A" + srcreg.ToString() + "),-(A" + dstreg.ToString() + ")";
                            ilen++;
                        }
                    }
                    else if (dstmod == 5 && srcmod == 0)
                    {
                        //sprintf(str, "EXG\tD%u,D%u\0", srcreg, dstreg); 
                        str = "EXG\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstmod == 5 && srcmod == 1)
                    {
                        //sprintf(str, "EXG\tA%u,A%u\0", srcreg, dstreg); 
                        str = "EXG\tA" + srcreg.ToString() + ",A" + dstreg.ToString();
                        ilen++;
                    }
                    else if (dstmod == 6 && srcmod == 1)
                    {
                        //sprintf(str, "EXG\tA%u,D%u\0", srcreg, dstreg); 
                        str = "EXG\tA" + srcreg.ToString() + ",D" + dstreg.ToString();
                        ilen++;
                    }
                    break;

                // Instruction type 0D 

                case 0x0d:
                    if ((dstmod == 3) || (dstmod == 7))
                    {
                        if (dstmod == 3) err = 3;
                        if (dstmod == 7) err = 2;
                        ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                        if (dstmod == 3) err = 1;
                        if (dstmod == 7) err = 2;
                        //sprintf(str, "ADDA.%c\t%s,A%u\0", opsize[err][0], dest, dstreg); 
                        str = "ADDA." + (string)opsize.GetValue(err) + "\t" + dest + ",A" + dstreg.ToString();
                        ilen++;
                    }
                    else if ((srcmod == 0 || srcmod == 1) && (dstmod >= 4 && dstmod <= 6))
                    {
                        if (srcmod == 0)
                        {
                            //sprintf(str, "ADDX.%c\tD%u,D%u\0", opsize[dstmod & 3][0], srcreg, dstreg); 
                            str = "ADDX." + (string)opsize.GetValue(dstmod & 3) + "\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                            ilen++;
                        }
                        else
                        {
                            //sprintf(str, "ADDX.%c\t-(A%u),-(A%u)\0", opsize[dstmod & 3][0], srcreg, dstreg); 
                            str = "ADDX." + (string)opsize.GetValue(dstmod & 3) + "\t-(A" + srcreg.ToString() + "),-(A" + dstreg.ToString() + ")";
                            ilen++;
                        }
                    }
                    else if ((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <= 6))
                    {
                        if ((dstmod & 4) == 4)
                        {
                            ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                            //sprintf(str, "ADD.%c\tD%u,%s\0", opsize[dstmod & 3][0], dstreg, dest); 
                            str = "ADD." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + "," + dest;
                            ilen++;
                        }
                        else
                        {
                            if (dstmod == 3) err = 3;
                            if (dstmod == 7) err = 2;
                            ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                            if (dstmod == 3) err = 1;
                            if (dstmod == 7) err = 2;
                            //sprintf(str, "ADD.%c\t%s,D%u\0", opsize[dstmod & 3][0], dest, dstreg); 
                            str = "ADD." + (string)opsize.GetValue(dstmod & 3) + "\t" + dest + ",D" + dstreg.ToString();
                            ilen++;
                        }
                    }
                    break;

                // Instruction type 0E 

                case 0x0e:
                    char charx = ((dstmod & 4) > 0 ? 'L' : 'R');
                    if (srcmod == 0)
                    {
                        if (dstreg == 0) dstreg = 8;

                        // sprintf(str,"AS%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "AS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (srcmod == 4)
                    {
                        //sprintf(str,"AS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "AS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (srcmod == 1)
                    {
                        if (dstreg == 0) dstreg = 8;
                        //sprintf(str,"LS%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "LS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + ",D" + srcreg.ToString();

                        ilen++;
                    }
                    else if (srcmod == 5)
                    {
                        //sprintf(str,"LS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "LS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (srcmod == 2)
                    {
                        if (dstreg == 0) dstreg = 8;
                        //sprintf(str,"ROX%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg);
                        str = "ROX" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + ",D" + srcreg.ToString();

                        ilen++;
                    }
                    else if (srcmod == 6)
                    {
                        //sprintf(str,"ROX%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "ROX" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (srcmod == 3)
                    {
                        if (dstreg == 0) dstreg = 8;
                        //sprintf(str,"RO%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "RO" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\t#" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (srcmod == 7)
                    {
                        //sprintf(str,"RO%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                        str = "RO" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D" + srcreg.ToString();
                        ilen++;
                    }
                    else if (dstreg == 0 && ((dstmod & 3) == 3))
                    {
                        //ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"AS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                        //str = "AS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD"
                        str = "FAILURE";
                        ilen++;
                    }
                    else if (dstreg == 1 && ((dstmod & 3) == 3))
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        // sprintf(str,"LS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                        str = "FAILURE2";
                        ilen++;
                    }
                    else if (dstreg == 2 && ((dstmod & 3) == 3))
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"ROX%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                        str = "FAILURE3";
                        ilen++;
                    }
                    else if (dstreg == 3 && ((dstmod & 3) == 3))
                    {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"RO%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                        str = "FAILURE4";
                        ilen++;
                    }
                    break;

                // Instruction type 0F 

                case 0x0f:
                    ch3 = br.ReadByte(); ch4 = br.ReadByte();
                    ilen++;
                    err = (uint)((ch3 << 8) + ch4);
                    if (ch1 == 0xf8 && ch2 == 00 && err == 0x01c0)
                    {
                        ilen += build_source(out dest, 3, 7, 4, addr, offset, br);
                        //sprintf(str,"LPSTOP\t%s\0",dest); 
                        str = "LPSTOP\t" + dest;
                        ilen++;
                    }
                    else if ((ch3 & 1) == 0)
                    {
                        char char1 = ((ch3 & 8) == 8 ? 'U' : 'S');
                        char char2 = ((ch3 & 4) == 4 ? 'N' : ' ');
                        int temp1 = (ch4 & 0xc0) >> 6;
                        int temp2 = ch2 & 7;
                        int temp3 = ch4 & 7;
                        int temp4 = ch3 & 7;
                        //sprintf(str,"TBL%c%c.%c\tD%u:D%u,D%u\0",((ch3&8)==8?'U':'S'),((ch3&4)==4?'N':' '),opsize[(ch4&0xc0)>>6][0],ch2&7,ch4&7,ch3&7); 
                        str = "TBL" + char1 + char2 + "." + (string)opsize.GetValue(temp1) + "\tD" + temp2.ToString() + ":D" + temp3.ToString() + ",D" + temp4.ToString();
                        ilen++;
                    }
                    else if ((ch3 & 1) == 1)
                    {
                        if ((ch3 & 0xc0) == 0) err = 1;
                        if ((ch3 & 0xc0) == 0x80) err = 2;
                        if ((ch3 & 0xc0) == 0x40) err = 3;
                        ilen += build_source(out sour, err, srcmod, srcreg, addr, offset, br);
                        char char1 = ((ch3 & 8) == 8 ? 'U' : 'S');
                        char char2 = ((ch3 & 4) == 4 ? 'N' : ' ');
                        int temp1 = (ch4 & 0xc0) >> 6;
                        int temp2 = ch3 & 7;
                        //sprintf(str,"TBL%c%c.%c\t%s,D%u\0",((ch3&8)==8?'U':'S'),((ch3&4)==4?'N':' '),opsize[(ch4&0xc0)>>6][0],sour,ch3&7); 
                        str = "TBL" + char1 + char2 + "." + (string)opsize.GetValue(temp1) + "\t" + sour + ",D" + temp2.ToString();
                        ilen++;
                    }
                    break;

            } /* END SWITCH STATEMENT */
            if (ilen == 0)
            {
                //sprintf(str, ".word\t%04X\0", i);
                str = ".word\t" + i.ToString("X4");
            }
            return (ilen);
        }

        private bool _passOne = true;

        private void AddLabel(long trgaddr)
        {
            if (trgaddr < 0x100000)
            {
                MNemonicHelper label = new MNemonicHelper();
                label.Address = trgaddr;
                label.Mnemonic = "LBL_" + trgaddr.ToString("X8") + ":";
                if (!_passOne)
                {
                    if (!LabelPresent(trgaddr))
                    {
                        labels.Add(label);
                    }
                }
                else
                {
                    if (!AddressInLabelList(trgaddr))
                    {
                        _labels.Add(label);
                    }
                }
            }
        }

        private bool LabelPresent(long trgaddr)
        {
            foreach (MNemonicHelper label in labels)
            {
                if (label.Address == trgaddr) return true;
            }
            return false;
        }

        private void CastProgressEvent(string info, int percentage, ProgressType type)
        {
            if (onProgress != null)
            {
                onProgress(this, new ProgressEventArgs(info, percentage, type));
            }
        }

        private MNemonicCollection _labels = new MNemonicCollection();

        private MNemonicCollection findLabels(string inputfile)
        {
            _labels = new MNemonicCollection();
            uint i, t, seg, adr;
            long addr, endaddr, adrcntr, trgaddr, trgaddr1, trgaddr2, trgaddr3, offaddr;

            byte ch1, ch2, ch3, ch4, ch5, ch6, ch7, ch8, ch9, ch10;
            //byte n1, n2, n3, n4;
            //uint infile = 0, outfile = 0, 
            uint addoff = 0;
            string inname, outname, offsetval;
            //byte inname[80], outname[80], offsetval[40];
            //byte str[80],cmd[80];
            string str, cmd;
            str = string.Empty;
            for (int temp = 0; temp < 8; temp++)
            {
                A_reg.SetValue(0, temp);
                D_reg.SetValue(0, temp);
            }
            swap = 0;
            addr = offaddr = 0;

            inname = inputfile;
            adrcntr = 0L;
            FileStream fsbr = new FileStream(inname, FileMode.Open, FileAccess.Read);
            if (fsbr == null) return _labels;
            BinaryReader br = new BinaryReader(fsbr);
            if (br == null)
            {
                fsbr.Close();
                return _labels;
            }
            fsbr.Position = addr;
            adrcntr = addr;
            // iterate through all functions
            // first get all the pointers to work from
            func_count = 0;
            FileInfo fi = new FileInfo(inputfile);
            CastProgressEvent("Start analyzing", 0, ProgressType.PassOne);
            long[] adresses = TrionicFile.GetVectorAddresses(inputfile);
            for (int vec = 1; vec < adresses.Length; vec++)
            {
                int percentage = (vec * 100) / adresses.Length;
                CastProgressEvent("Analyzing", percentage, ProgressType.PassOne);
                long vector = adresses[vec];

                if (vector != 0 && vector < fi.Length)
                {
                    try
                    {
                        LoadLabels(vector, fsbr, br, 0);
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to handle vector: " + E.Message);
                    }
                }
            }

            logger.Debug("Found " + _labels.Count.ToString() + " in pass one");
            foreach (MNemonicHelper label in _labels)
            {
                if (label.Address == adresses[1]) logger.Debug("Found start pointer!");
            }
            return _labels;
        }

        public bool DisassembleFile(string inputfile, SymbolCollection symbols)
        {
            // recursive method when jsr was found
            mnemonics = new MNemonicCollection();
            labels = new MNemonicCollection();

            findLabels(inputfile);

            _passOne = false;

            int i, t, seg, adr;
            long addr, endaddr, adrcntr, trgaddr, trgaddr1, trgaddr2, trgaddr3, offaddr;

            byte ch1, ch2, ch3, ch4, ch5, ch6, ch7, ch8, ch9, ch10;
            //byte n1, n2, n3, n4;
            //uint infile = 0, outfile = 0, 
            uint addoff = 0;
            string inname, outname, offsetval;
            //byte inname[80], outname[80], offsetval[40];
            //byte str[80],cmd[80];
            string str, cmd;
            str = string.Empty;
            for (int temp = 0; temp < 8; temp++)
            {
                A_reg.SetValue(0, temp);
                D_reg.SetValue(0, temp);
            }
            m_symbols = symbols;
            swap = 0;
            addr = offaddr = 0;

            inname = inputfile;
            //infile = 1;
            //            outname = outputfile;
            //outfile = 1;
            //addr = startaddress;
            /********************* DISASSEMBLY STARTS HERE *********************/
            /* Read all the preceding words first */
            adrcntr = 0L;
            //StreamWriter sw = new StreamWriter(outname, false);
            FileStream fsbr = new FileStream(inname, FileMode.Open, FileAccess.Read);
            if (fsbr == null) return false;
            BinaryReader br = new BinaryReader(fsbr);
            if (br == null)
            {
                fsbr.Close();
                //sw.Close();
                return false;
            }
            //fsbr.Position = addr;
            adrcntr = addr;
            // iterate through all functions
            // first get all the pointers to work from
            func_count = 0;
            FileInfo fi = new FileInfo(inputfile);
            CastProgressEvent("Starting disassembly", 0, ProgressType.DisassemblingVectors);
            long[] vectors = TrionicFile.GetVectorAddresses(inputfile);
            for (int vec = 0; vec < vectors.Length; vec++)
            {
                int percentage = ((vec + 1) * 100) / vectors.Length;
                CastProgressEvent("Disassembling vectors", percentage, ProgressType.DisassemblingVectors);
                long vector = Convert.ToInt64(vectors.GetValue(vec));

                if (vector != 0 && vector < fi.Length)
                {
                    try
                    {
                        DisassembleFunction(vector, symbols, fsbr, br, 0);
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to handle vector: " + E.Message);
                    }
                }
            }

            CastProgressEvent("Translating vector labels", 0, ProgressType.TranslatingVectors);
            //logger.Debug("Translating vector labels");

            int lblcount = 0;
            string[] names = TrionicFile.GetVectorNames();
            foreach (MNemonicHelper label in labels)
            {
                //logger.Debug("label: " + label.Address.ToString("X8") + " " + label.Mnemonic);
                int percentage = (lblcount++ * 100) / labels.Count;
                CastProgressEvent("Translating vector labels", percentage, ProgressType.TranslatingVectors);
                for (i = 0; i < vectors.Length; i++)
                {
                    if (label.Address == Convert.ToInt64(vectors.GetValue(i)))
                    {
                        label.Mnemonic = names[i].Replace(" ","_").ToUpper() + ":";
                    }
                }
            }
            /*
            logger.Debug("Translating known functions");
            CastProgressEvent("Translating known functions", 0, ProgressType.TranslatingLabels);
            lblcount = 0;
            foreach (MNemonicHelper label in labels)
            {
                int percentage = (lblcount++ * 100) / labels.Count;
                CastProgressEvent("Translating known functions", percentage, ProgressType.TranslatingLabels);
                foreach (MNemonicHelper mnemonic in mnemonics)
                {
                    if (mnemonic.Mnemonic.Contains("JSR") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        //    break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BEQ") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BRA") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BLS") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BNE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BHI") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BCS") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BCC") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BGE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BLT") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BGT") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                    else if (mnemonic.Mnemonic.Contains("BLE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                    }
                }
            }*/
            CastProgressEvent("Adding labels", 0, ProgressType.AddingLabels);
            //logger.Debug("Adding labels");
            lblcount = 0;
            foreach (MNemonicHelper label in labels)
            {
                int percentage = (lblcount++ * 100) / labels.Count;
                CastProgressEvent("Adding labels", percentage, ProgressType.AddingLabels);

                label.Address--; // for sequencing
                mnemonics.Add(label);
            }

            //logger.Debug("Sorting data");
            CastProgressEvent("Sorting mnemonics", 0, ProgressType.SortingData);
            mnemonics.SortColumn = "Address";
            mnemonics.SortingOrder = GenericComparer.SortOrder.Ascending;
            mnemonics.Sort();
            CastProgressEvent("Sorting mnemonics", 100, ProgressType.SortingData);
            return true;

        }
        
        private MNemonicCollection labels;

        public MNemonicCollection Labels
        {
            get { return labels; }
            set { labels = value; }
        }

        private MNemonicCollection mnemonics;

        public MNemonicCollection Mnemonics
        {
            get { return mnemonics; }
            set { mnemonics = value; }
        }

        private int func_count = 0;

        private void TranslateLabels(MNemonicHelper mnemonic)
        {
            if (AddressInLabelList(mnemonic.Mnemonic))
            {
                foreach (MNemonicHelper label in _labels)
                {
                    if (mnemonic.Mnemonic.Contains("JSR") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BEQ") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BRA") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BLS") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BNE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BHI") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BCS") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BCC") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BGE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BLT") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BGT") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                    else if (mnemonic.Mnemonic.Contains("BLE") && mnemonic.Mnemonic.Contains(label.Address.ToString("X8")))
                    {
                        mnemonic.Mnemonic = mnemonic.Mnemonic.Replace(label.Address.ToString("X8"), label.Mnemonic.Replace(":", ""));
                        break;
                    }
                }
            }
        }

        private void DisassembleFunction(long addr, SymbolCollection symbols, FileStream fs, BinaryReader br, long offset)
        {
            CastProgressEvent("Disassembling function: " + addr.ToString("X8"), func_count++, ProgressType.DisassemblingFunctions);
            //logger.Debug("DisassembleFunction: " + addr.ToString("X8"));
            MNemonicCollection functionList = new MNemonicCollection();

            MNemonicHelper label = new MNemonicHelper();
            //long realAddr = fs.Position + offset;
            //label.Mnemonic = "Function_" + realAddr.ToString("X8") + ":";
            label.Mnemonic = "Function_" + addr.ToString("X8") +":";
            label.Address = addr;
            if (AddressInMnemonicList(addr))
            {
                //logger.Debug("Already disassembled: " + addr.ToString("X8"));
                return ;
            }
            labels.Add(label);
            functionList.Add(label);
            long offaddr = 0;
            if (addr == 0) return ;
            if (addr > offset)
            {
                fs.Position = addr - offset;
            }
            else
            {
                fs.Position = addr;
            }

            bool endsub = false;
            bool issub = false;
            bool isjump = false;
            string str;
            while (!endsub)
            {
                byte ch1 = br.ReadByte();
                byte ch2 = br.ReadByte();
                uint i = (uint)((ch1 << 8) + ch2);
                uint seg = (uint)(((addr + offaddr) & 0xffff0000) >> 16);
                uint adr = (uint)(((addr + offaddr) & 0xffff));
                /*if (ch1 == 0x58 && ch2 == 0x8F)
                {
                    logger.Debug("break!");
                }*/
                uint t = disasm(out str, addr, ch1, ch2, offaddr, br, out endsub, out issub, out isjump);
                //logger.Debug(str);
                if (str != "")
                {
                    MNemonicHelper mnhelper = new MNemonicHelper();
                    mnhelper.Mnemonic = str;
                    mnhelper.Address = addr;
                    //realAddr = fs.Position + offset;
                    //mnhelper.Address = realAddr;
                    if (!AddressInMnemonicList(addr))
                    {
                        if (isjump)
                        {
                            TranslateLabels(mnhelper);
                        }
                        mnemonics.Add(mnhelper);
                    }
                    functionList.Add(mnhelper);
                }
                if (t > 5) t = 5;
                //addr += t;
                switch (t)
                {
                    case 0:
                    case 1:
                        addr += 2L;
                        break;
                    case 2:
                        addr += 4L;
                        break;
                    case 3:
                        addr += 6L;
                        break;
                    case 4:
                        addr += 8L;
                        break;
                    case 5:
                        addr += 10L;
                        break;
                }
                if (issub)
                {
                    /*if (trgdata == 0)
                    {
                        logger.Debug("break!");
                    }*/
                    
                    // alleen als die nog niet geweest is
                    if (trgdata != 0)
                    {
                        if (!AddressInMnemonicList(trgdata))
                        {
                            if (trgdata < 0x00100000) // 0x00F00000 == T7
                            {
                                long position = fs.Position;
                                //logger.Debug("recursive: " + trgdata.ToString("X8") + " curr address: " + addr.ToString("X8")); 
                                DisassembleFunction(trgdata, symbols, fs, br, offset);
                                //logger.Debug("After recursion: " + addr.ToString("X8"));
                                fs.Position = position; // reset to previous position
                            }
                        }
                    }
                }
                if (endsub)
                {
                }
            }

            //logger.Debug("Done with function: " + mnemonics.Count.ToString());
        }
    
		private bool AddressInLabelList(long address)
        {
            foreach (MNemonicHelper _label in _labels)
            {
                if (_label.Address == address) return true;
            }
            return false;
        }

        private bool AddressInLabelList(string mnemonic)
        {
            foreach (MNemonicHelper _label in _labels)
            {
                if (mnemonic.Contains(_label.Address.ToString("X8"))) return true;
            }
            return false;
        }

		private void LoadLabels(long addr, FileStream fs, BinaryReader br, long offset)
        {
            //logger.Debug("Load labels from addr: " + addr.ToString("X8"));
            MNemonicHelper label = new MNemonicHelper();
            MNemonicCollection functionList = new MNemonicCollection();
            label.Mnemonic = "Function_" + addr.ToString("X8") + ":";
            label.Address = addr;
            
            if (AddressInLabelList(addr))
            {
                return;
            }
            _labels.Add(label);
            //logger.Debug("Added label: " + label.Mnemonic + " " + label.Address.ToString("X8"));

            long offaddr = 0;
            if (addr == 0) return;
            if (addr > offset)
            {
                fs.Position = addr - offset;
            }
            else
            {
                fs.Position = addr;
            }

            bool endsub = false;
            bool issub = false;
            bool isjump = false;
            string str;
            while (!endsub)
            {
                byte ch1 = br.ReadByte();
                byte ch2 = br.ReadByte();
                uint i = (uint)((ch1 << 8) + ch2);
                uint seg = (uint)(((addr + offaddr) & 0xffff0000) >> 16);
                uint adr = (uint)(((addr + offaddr) & 0xffff));
                uint t = disasm(out str, addr, ch1, ch2, offaddr, br, out endsub, out issub, out isjump);
                if (str != "")
                {
                    MNemonicHelper mnhelper = new MNemonicHelper();
                    mnhelper.Mnemonic = str;
                    mnhelper.Address = addr;
                    //realAddr = fs.Position + offset;
                    //mnhelper.Address = realAddr;
                    functionList.Add(mnhelper);
                }
                if (t > 5) t = 5;
                //addr += t;
                switch (t)
                {
                    case 0:
                    case 1:
                        addr += 2L;
                        break;
                    case 2:
                        addr += 4L;
                        break;
                    case 3:
                        addr += 6L;
                        break;
                    case 4:
                        addr += 8L;
                        break;
                    case 5:
                        addr += 10L;
                        break;
                }
                if (issub)
                {
                    if (trgdata != 0)
                    {
                        if (!AddressInLabelList(trgdata))
                        {
                            if (trgdata < 0x00100000) // 0x00F00000 == T7
                            {
                                long position = fs.Position;
                                LoadLabels(trgdata, fs, br, 0); //offset
                                fs.Position = position; // reset to previous position
                            }
                        }
                    }
                }
                if (endsub)
                {
                }
            }
        }

        private bool AddressInMnemonicList(long trgdata)
        {
            foreach (MNemonicHelper helper in Mnemonics)
            {
                if (helper.Address == trgdata) return true;
            }
            return false;
        }

        public bool DisassembleFileRtf(string inputfile, string outputfile, long endaddress, SymbolCollection symbols)
        {
            uint i, t, seg, adr;
            long addr, endaddr, adrcntr, trgaddr, trgaddr1, trgaddr2, trgaddr3, offaddr;
            
            byte ch1, ch2, ch3, ch4, ch5, ch6, ch7, ch8, ch9, ch10;
            //byte n1, n2, n3, n4;
            //uint infile = 0, outfile = 0,
            string inname, outname;
            //byte inname[80], outname[80], offsetval[40];
            //byte str[80],cmd[80];
            string str, cmd;
            str = string.Empty;
            for (int temp = 0; temp < 8; temp++)
            {
                A_reg.SetValue(0, temp);
                D_reg.SetValue(0, temp);
            }
            m_symbols = symbols;
            swap = 0;
            addr = offaddr = 0;

            inname = inputfile;
            //infile = 1;
            outname = outputfile;
            //outfile = 1;

            endaddr = endaddress;
            addr = 0;
            /********************* DISASSEMBLY STARTS HERE *********************/
            /* Read all the preceding words first */
            adrcntr = 0L;
            StreamWriter sw = new StreamWriter(outname, false);
            FileStream fsbr = new FileStream(inname, FileMode.Open, FileAccess.Read);
            if (fsbr == null) return false;
            BinaryReader br = new BinaryReader(fsbr);
            if (br == null)
            {
                fsbr.Close();
                sw.Close();
                return false;
            }
            fsbr.Position = addr;
            adrcntr = addr;

            // start rtf file.
            sw.Write(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1043{\fonttbl{\f0\fswiss\fcharset0 Courier new;}}{\colortbl ;\red255\green0\blue0;\red0\green128\blue0;\red0\green0\blue255;}{\*\generator Msftedit 5.41.15.1507;}\viewkind4\uc1\pard\lang1033\f0\fs20 ");


            /* Parse starting from address addr */
            bool issub = false;
            bool isjump = false;
            bool endsub = false;
            while (addr <= endaddr)
            {
                try
                {
                    ch1 = br.ReadByte();
                    ch2 = br.ReadByte();
                    i = (uint)((ch1 << 8) + ch2);
                    seg = (uint)(((addr + offaddr) & 0xffff0000) >> 16);
                    adr = (uint)(((addr + offaddr) & 0xffff));
                    t = disasm(out str, addr, ch1, ch2, offaddr, br, out endsub, out issub, out isjump);
                    if (t > 5) t = 5;
                    string myline = string.Empty;
                    switch (t)
                    {
                        case 0:
                        case 1:
                            //sw.WriteLine(seg.ToString("X4")  + adr.ToString("X4") + ": " + i.ToString("X4") + "\t\t\t\t" + str);
                            myline = seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + "\t\t\t\t" + str;
                            addr += 2L;
                            break;
                        case 2:
                            fsbr.Position = addr + 2L;
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            trgaddr = (long)ch3 * 256 + ch4;
                            //sw.WriteLine(seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + "\t\t\t" + str);
                            myline = seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + "\t\t\t" + str;
                            addr += 4L;
                            break;
                        case 3:
                            fsbr.Position = addr + 2L;
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            trgaddr = (long)ch3 * 256 + ch4;
                            trgaddr1 = (long)ch5 * 256 + ch6;
                            //sw.WriteLine(seg.ToString("X4")  + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + "\t\t" + str);
                            myline = seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + "\t\t" + str;
                            addr += 6L;
                            break;
                        case 4:
                            fsbr.Position = addr + 2L;
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            ch7 = br.ReadByte();
                            ch8 = br.ReadByte();
                            trgaddr = (long)ch3 * 256 + ch4;
                            trgaddr1 = (long)ch5 * 256 + ch6;
                            trgaddr2 = (long)ch7 * 256 + ch8;
                            //sw.WriteLine(seg.ToString("X4")  + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + " " + trgaddr2.ToString("X4") + "\t\t" + str);
                            myline = seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + " " + trgaddr2.ToString("X4") + "\t\t" + str;
                            addr += 8L;
                            break;
                        case 5:
                            fsbr.Position = addr + 2L;
                            ch3 = br.ReadByte();
                            ch4 = br.ReadByte();
                            ch5 = br.ReadByte();
                            ch6 = br.ReadByte();
                            ch7 = br.ReadByte();
                            ch8 = br.ReadByte();
                            ch9 = br.ReadByte();
                            ch10 = br.ReadByte();
                            trgaddr = (long)ch3 * 256 + ch4;
                            trgaddr1 = (long)ch5 * 256 + ch6;
                            trgaddr2 = (long)ch7 * 256 + ch8;
                            trgaddr3 = (long)ch9 * 256 + ch10;
                            //sw.WriteLine(seg.ToString("X4")  + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + " " + trgaddr2.ToString("X4") + " " + trgaddr3.ToString("X4") + "\t\t" + str);
                            myline = seg.ToString("X4") + adr.ToString("X4") + ": " + i.ToString("X4") + " " + trgaddr.ToString("X4") + " " + trgaddr1.ToString("X4") + " " + trgaddr2.ToString("X4") + " " + trgaddr3.ToString("X4") + "\t" + str;
                            addr += 10L;
                            break;
                        default:
                            br.Close();
                            fsbr.Close();
                            sw.Close();
                            return false;
                    }
                    sw.WriteLine(myline + @"\par");
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                    addr++;
                }
            }
            sw.Write(@"\lang1043\par}");

            br.Close();
            fsbr.Close();
            sw.Close();
            return true;

        }

        private string HighlightText(string value)
        {
            /*   if (value.Contains("ORI"))
               {
                   int idx = value.IndexOf("ORI");
                   if (idx >= 0)
                   {
                       // insert highlight code
                       value = value.Insert(idx, @"\cf1");
                       value = value.Insert(idx + 7, @"\cf0");
                   }
               }*/
            if (value.Contains("#"))
            {
                int idx = value.IndexOf("#");
                if (idx >= 0)
                {
                    // insert highlight code
                    value = value.Insert(idx, @"\cf3 ");
                    int idx2 = value.IndexOf(",", idx);
                    if (idx2 > 0)
                    {
                        value = value.Insert(idx2, @"\cf0 ");
                    }
                    else
                    {
                        value += @"\cf0 ";
                    }
                }
            }
            return value + @"\par";
        }

    }
}
