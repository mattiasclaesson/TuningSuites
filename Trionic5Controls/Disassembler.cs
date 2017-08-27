using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Trionic5Tools;
using CommonSuite;

namespace Trionic5Controls
{
    public class Disassembler
    {

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
                if (sh.Flash_start_address == caddr)
                {
                    symbol = "ROM_" + sh.Varname  ;
                    retval = 1;
                }
                else if (sh.Start_address == caddr)
                {
                    symbol = "RAM_" + sh.Varname ;
                    retval = 1;
                }
            }
            if (symbol == string.Empty)
            {
                if (caddr == 0xFFFB04) symbol = "RAMBAR";
                else if (caddr == 0xFFFB02) symbol = "RAMTST";
                else if (caddr == 0xFFFB00) symbol = "RAMMCR";
                else if (caddr == 0xFFFA00) symbol = "SIMCR";
                else if (caddr == 0xFFFA02) symbol = "SIMTR";
                else if (caddr == 0xFFFA04) symbol = "SYNCR";
                else if (caddr == 0xFFFA07) symbol = "RSR";
                else if (caddr == 0xFFFA08) symbol = "SIMTRE";
                else if (caddr == 0xFFFA11) symbol = "PORTE0";
                else if (caddr == 0xFFFA13) symbol = "PORTE1";
                else if (caddr == 0xFFFA15) symbol = "DDRE";
                else if (caddr == 0xFFFA17) symbol = "PEPAR";
                else if (caddr == 0xFFFA19) symbol = "PORTF0";
                else if (caddr == 0xFFFA1B) symbol = "PORTF1";
                else if (caddr == 0xFFFA1D) symbol = "DDRF";
                else if (caddr == 0xFFFA1F) symbol = "PFPAR";
                else if (caddr == 0xFFFA21) symbol = "SYPCR";
                else if (caddr == 0xFFFA22) symbol = "PICR";
                else if (caddr == 0xFFFA24) symbol = "PITR";
                else if (caddr == 0xFFFA26) symbol = "SWSR_1";
                else if (caddr == 0xFFFA27) symbol = "SWSR";
                else if (caddr == 0xFFFA30) symbol = "TSTMSRA";
                else if (caddr == 0xFFFA32) symbol = "TSTMSRB";
                else if (caddr == 0xFFFA34) symbol = "TSTSC";
                else if (caddr == 0xFFFA36) symbol = "TSTRC";
                else if (caddr == 0xFFFA38) symbol = "CREG";
                else if (caddr == 0xFFFA3A) symbol = "DREG";
                else if (caddr == 0xFFFA41) symbol = "PORTC";
                else if (caddr == 0xFFFA44) symbol = "CSPAR0";
                else if (caddr == 0xFFFA46) symbol = "CSPAR1";
                else if (caddr == 0xFFFA48) symbol = "CSBARBT";
                else if (caddr == 0xFFFA4A) symbol = "CSORBT";
                else if (caddr == 0xFFFA4C) symbol = "CSBAR0";
                else if (caddr == 0xFFFA4E) symbol = "CSOR0";
                else if (caddr == 0xFFFA50) symbol = "CSBAR1";
                else if (caddr == 0xFFFA52) symbol = "CSOR1";
                else if (caddr == 0xFFFA54) symbol = "CSBAR2";
                else if (caddr == 0xFFFA56) symbol = "CSOR2";
                else if (caddr == 0xFFFA58) symbol = "CSBAR3";
                else if (caddr == 0xFFFA5A) symbol = "CSOR3";
                else if (caddr == 0xFFFA5C) symbol = "CSBAR4";
                else if (caddr == 0xFFFA5E) symbol = "CSOR4";
                else if (caddr == 0xFFFA60) symbol = "CSBAR5";
                else if (caddr == 0xFFFA62) symbol = "CSOR5";
                else if (caddr == 0xFFFA64) symbol = "CSBAR6";
                else if (caddr == 0xFFFA66) symbol = "CSOR6";
                else if (caddr == 0xFFFA68) symbol = "CSBAR7";
                else if (caddr == 0xFFFA6A) symbol = "CSOR7";
                else if (caddr == 0xFFFA6C) symbol = "CSBAR8";
                else if (caddr == 0xFFFA6E) symbol = "CSOR8";
                else if (caddr == 0xFFFA70) symbol = "CSBAR9";
                else if (caddr == 0xFFFA72) symbol = "CSOR9";
                else if (caddr == 0xFFFA74) symbol = "CSBAR10";
                else if (caddr == 0xFFFA76) symbol = "CSOR10";
                else if (caddr == 0xFFFC00) symbol = "QSMCR";
                else if (caddr == 0xFFFC02) symbol = "QTEST";
                else if (caddr == 0xFFFC04) symbol = "QILR";
                else if (caddr == 0xFFFC05) symbol = "QIVR";
                else if (caddr == 0xFFFC08) symbol = "SCCR0";
                else if (caddr == 0xFFFC0A) symbol = "SCCR1";
                else if (caddr == 0xFFFC0C) symbol = "SCSR";
                else if (caddr == 0xFFFC0E) symbol = "SCDR";
                else if (caddr == 0xFFFC15) symbol = "QPDR";
                else if (caddr == 0xFFFC16) symbol = "PQSPAR";
                else if (caddr == 0xFFFC17) symbol = "QDDR";
                else if (caddr == 0xFFFC18) symbol = "SPCR0";
                else if (caddr == 0xFFFC1A) symbol = "SPCR1";
                else if (caddr == 0xFFFC1C) symbol = "SPCR2";
                else if (caddr == 0xFFFC1E) symbol = "SPCR3";
                else if (caddr == 0xFFFC1F) symbol = "SPSR";
                else if (caddr == 0xFFFD00) symbol = "QRXD";
                else if (caddr == 0xFFFD20) symbol = "QTXD";
                else if (caddr == 0xFFFD40) symbol = "QCMD";
                else if (caddr == 0xFFFE00) symbol = "TPUMCR";
                else if (caddr == 0xFFFE02) symbol = "TPUCFG";
                else if (caddr == 0xFFFE04) symbol = "DSCR";
                else if (caddr == 0xFFFE06) symbol = "DSSR";
                else if (caddr == 0xFFFE08) symbol = "TPUICR";
                else if (caddr == 0xFFFE0A) symbol = "TPUIER";
                else if (caddr == 0xFFFE0C) symbol = "CFSR0";
                else if (caddr == 0xFFFE0E) symbol = "CFSR1";
                else if (caddr == 0xFFFE10) symbol = "CFSR2";
                else if (caddr == 0xFFFE12) symbol = "CFSR3";
                else if (caddr == 0xFFFE14) symbol = "HSR0";
                else if (caddr == 0xFFFE16) symbol = "HSR1";
                else if (caddr == 0xFFFE18) symbol = "HSRR0";
                else if (caddr == 0xFFFE1A) symbol = "HSRR1";
                else if (caddr == 0xFFFE1C) symbol = "CPR0";
                else if (caddr == 0xFFFE1E) symbol = "CPR1";
                else if (caddr == 0xFFFE20) symbol = "TPUISR";
                else if (caddr == 0xFFFE22) symbol = "LINK";
                else if (caddr == 0xFFFE24) symbol = "SGLR";
                else if (caddr == 0xFFFE26) symbol = "DCNR";
                else if (caddr == 0xFFFF00) symbol = "TPUPRAM";
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
            string[] ctregs ={ "SFC", "DFC", "USP", "VBR" };
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
//                            sprintf(str, "MOVEA.%c\t%s,%s\0", movesize[itype][0], sour, dest);
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
                      switch(i) {
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
                          err = (uint)(ch3<<8)+ch4;
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
                          if((err & 0x800) == 0x800) err = err | 2;
                          err = err & 3;
                          if((i & 1)==1) 
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
                      if(dstreg == 0 && dstmod <= 2) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset,br);
                        //sprintf(str,"NEGX.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NEGX." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                          ilen++;
                      }
                      else if(dstreg == 0 && dstmod == 3) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset,br);
                        //sprintf(str,"MOVE\tSR,%s\0",dest); 
                        str = "MOVE\tSR," + dest;
                          ilen++;
                      }
                      else if(dstmod == 4 || dstmod == 6) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset,br);
                        char char1 = ((dstmod & 0x20) > 0 ? 'L' : 'W');
                        //sprintf(str,"CHK.%c\t%s,D%u\0",(dstmod&&0x20?'L':'W'),dest,dstreg); 
                        str = "CHK." + char1 + "\t" + dest + ",D" + dstreg.ToString();
                          ilen++;
                      }
                      else if(dstmod == 7) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"LEA\t%s,A%u\0",dest,dstreg); 
                        str = "LEA\t" + dest + ",A" + dstreg.ToString();
                          ilen++;
                      }
                      else if(dstreg == 1 && dstmod <= 2) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"CLR.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "CLR." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                          ilen++;
                      }
                      else if(dstreg == 3 && dstmod == 3) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t%s,SR\0",dest); 
                        str = "MOVE\t" + dest + ",SR";
                          ilen++;
                      }

                      else if(dstreg == 1 && dstmod == 3) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t CCR,%s\0",dest); 
                        str = "MOVE\t CCR," + dest;
                          ilen++;
                      }
                      else if(dstreg == 2 && dstmod <= 2) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NEG.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NEG." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                          ilen++;
                      }
                      else if(dstreg == 2 && dstmod == 3) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"MOVE\t %s,CCR\0",dest);
                        str = "MOVE\t " + dest + ",CCR";
                          ilen++;
                      }
                      else if(dstreg == 3 && dstmod <= 2) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"NOT.%c\t%s\0",opsize[dstmod&7][0],dest); 
                        str = "NOT." + (string)opsize.GetValue(dstmod & 7) + "\t" + dest;
                          ilen++;
                      }
                      else if(dstreg == 4 && dstmod == 0) {
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
                              Console.WriteLine("break!");
                          }*/

                          str = "JSR\t" + dest;
                          //Console.WriteLine(dest);
                          ilen++;
                      }
                      else if (dstreg == 7 && dstmod == 3)
                      {
                          ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                          //sprintf(str, "JMP\t%s\0", dest); 
                          endsub = true; // jump to somewhere else
                          AddLabel(destaddr);
                          isjump = true;
                          //Console.WriteLine("JUMP SEEND: " + destaddr.ToString("X8"));
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
                      if(n2 == 0) {
                        ilen += build_displacement(&trgaddr,addr, offset, ch2, br);
                        //sprintf(str,"BRA\t%08lX\0",trgaddr); 
                          // add it to the labellist
                        AddLabel(trgaddr);
                        isjump = true;
                        str = "BRA\t" + trgaddr.ToString("X8");
                          ilen++;
                      }
                      else if(n2 == 1) {
                        ilen += build_displacement(&trgaddr,addr, offset, ch2, br);
                        //sprintf(str,"BSR\t%08lX\0",trgaddr); 
                        AddLabel(trgaddr);
                        isjump = true;

                        str = "BSR\t" + trgaddr.ToString("X8");
                          ilen++;
                      }
                      else {
                        ilen += build_displacement(&trgaddr,addr, offset, ch2,br);
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
                      if((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <=6)) {
                        if((dstmod & 4) == 4) {
                          ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                          //sprintf(str,"OR.%c\tD%u,%s\0",opsize[dstmod&3][0],dstreg,dest);
                          str = "OR." + (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + "," + dest;
                            ilen++;
                        }
                        else {
                          if(dstmod == 3) err = 3;
                          if(dstmod == 7) err = 2;
                      ilen += build_source(out dest, err, srcmod, srcreg, addr, offset, br);
                          if(dstmod == 3) err = 1;
                          if(dstmod == 7) err = 2;
                             //sprintf(str,"OR.%c\t%s,D%u\0",opsize[dstmod&3][0],dest,dstreg); 
                          str = "OR." + (string)opsize.GetValue(dstmod & 3) + "\t" + dest + ",D" + dstreg.ToString();
                            ilen++;
                        }
                      }
                      else if(dstmod == 3) {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"DIVU.W\t%s,D%u\0",sour,dstreg); 
                        str = "DIVU.W\t" + sour + ",D" + dstreg.ToString();
                          ilen++;
                      }
                      else if(dstmod == 7) {
                        ilen += build_source(out sour, 3, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"DIVS.W\t%s,D%u\0",sour,dstreg); 
                        str = "DIVS.W\t" + sour + ",D" + dstreg.ToString();
                          ilen++;
                      }
                      else if((srcmod == 0 || srcmod == 1) && (dstmod == 4)) {
                        if(srcmod == 0) {
                          //sprintf(str,"SBCD.%c\tD%u,D%u\0",opsize[dstmod&3][0],srcreg,dstreg); 
                            str = "SBCD." + (string)opsize.GetValue(dstmod & 3) + "\tD" + srcreg.ToString() + ",D" + dstreg.ToString();
                            ilen++;
                        }
                        else {
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
                      if((dstmod >= 0 && dstmod <= 2) || (dstmod >= 4 && dstmod <=6)) {
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
                        char charx = ((dstmod&4)>0?'L':'R');
                      if(srcmod == 0) {
                        if(dstreg == 0) dstreg = 8;
                          
                       // sprintf(str,"AS%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "AS" + charx + "."+ (string)opsize.GetValue(dstmod&3) +"\t#"+ dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(srcmod == 4) {
                        //sprintf(str,"AS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "AS"+ charx + "."+ (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(srcmod == 1) {
                        if(dstreg == 0) dstreg = 8;
                        //sprintf(str,"LS%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "LS" + charx + "."+ (string)opsize.GetValue(dstmod&3) +"\t#"+ dstreg.ToString() + ",D"+ srcreg.ToString();

                          ilen++;
                      }
                      else if(srcmod == 5) {
                        //sprintf(str,"LS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "LS"+ charx + "."+ (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(srcmod == 2) {
                        if(dstreg == 0) dstreg = 8;
                        //sprintf(str,"ROX%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg);
                          str = "ROX" + charx + "."+ (string)opsize.GetValue(dstmod&3) +"\t#"+ dstreg.ToString() + ",D"+ srcreg.ToString();

                          ilen++;
                      }
                      else if(srcmod == 6) {
                        //sprintf(str,"ROX%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "ROX"+ charx + "."+ (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(srcmod == 3) {
                        if(dstreg == 0) dstreg = 8;
                        //sprintf(str,"RO%c.%c\t#%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "RO" + charx + "."+ (string)opsize.GetValue(dstmod&3) +"\t#"+ dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(srcmod == 7) {
                        //sprintf(str,"RO%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dstreg,srcreg); 
                          str = "RO"+ charx + "."+ (string)opsize.GetValue(dstmod & 3) + "\tD" + dstreg.ToString() + ",D"+ srcreg.ToString();
                          ilen++;
                      }
                      else if(dstreg == 0 && ((dstmod&3) == 3)) {
                        //ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"AS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                          //str = "AS" + charx + "." + (string)opsize.GetValue(dstmod & 3) + "\tD"
                          str = "FAILURE";
                          ilen++;
                      }
                      else if(dstreg == 1 && ((dstmod&3) == 3)) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                       // sprintf(str,"LS%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                          str = "FAILURE2";
                          ilen++;
                      }
                      else if(dstreg == 2 && ((dstmod&3) == 3)) {
                        ilen += build_destination(out dest, 0, srcmod, srcreg, addr, offset, br);
                        //sprintf(str,"ROX%c.%c\tD%u,D%u\0",(dstmod&4?'L':'R'),opsize[dstmod&3][0],dest); 
                          str = "FAILURE3";
                          ilen++;
                      }
                      else if(dstreg == 3 && ((dstmod&3) == 3)) {
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
                        err= (uint)((ch3<<8) + ch4);
                        if(ch1 == 0xf8 && ch2 == 00 && err == 0x01c0) {
                          ilen += build_source(out dest,3,7,4,addr,offset, br);
                          //sprintf(str,"LPSTOP\t%s\0",dest); 
                            str = "LPSTOP\t"  + dest;
                            ilen++;
                        }
                        else if((ch3&1) == 0) {
                            char char1 = ((ch3&8)==8?'U':'S');
                            char char2 = ((ch3&4)==4?'N':' ');
                            int temp1 = (ch4&0xc0)>>6;
                            int temp2 = ch2&7;
                            int temp3 = ch4&7;
                            int temp4 = ch3&7;
                          //sprintf(str,"TBL%c%c.%c\tD%u:D%u,D%u\0",((ch3&8)==8?'U':'S'),((ch3&4)==4?'N':' '),opsize[(ch4&0xc0)>>6][0],ch2&7,ch4&7,ch3&7); 
                            str = "TBL" + char1 + char2 + "." + (string) opsize.GetValue(temp1) + "\tD" + temp2.ToString() + ":D"+ temp3.ToString() + ",D" + temp4.ToString();
                            ilen++;
                        }
                        else if((ch3&1) == 1) {
                          if((ch3&0xc0) == 0) err = 1;
                          if((ch3&0xc0) == 0x80) err = 2;
                          if((ch3&0xc0) == 0x40) err = 3;
                          ilen+=build_source(out sour,err, srcmod, srcreg, addr, offset, br);
                            char char1 = ((ch3&8)==8?'U':'S');
                            char char2 = ((ch3&4)==4?'N':' ');
                            int temp1 = (ch4&0xc0)>>6;
                            int temp2 = ch3&7;
                          //sprintf(str,"TBL%c%c.%c\t%s,D%u\0",((ch3&8)==8?'U':'S'),((ch3&4)==4?'N':' '),opsize[(ch4&0xc0)>>6][0],sour,ch3&7); 
                            str = "TBL" + char1 + char2 + "." + (string)opsize.GetValue(temp1) + "\t" + sour + ",D"+ temp2.ToString();
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
            if (trgaddr < 0xF00000)
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
            if(onProgress != null)
            {
                onProgress(this, new ProgressEventArgs(info, percentage, type));
            }
        }

        private MNemonicCollection _labels = new MNemonicCollection();

        private MNemonicCollection findLabels(IECUFile m_trionicFile, string inputfile)
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
            for (int vec = 1; vec <= 127; vec++)
            {
                int percentage = (vec * 100) / 127;
                CastProgressEvent("Analyzing", percentage, ProgressType.PassOne);
                long vector = m_trionicFile.GetStartVectorAddress(inputfile, vec);
                long len = fi.Length;
                if (len == 0x20000) len = 0x60000;

                if (vector != 0 && vector < len * 2)
                {
                    try
                    {
                        LoadLabels(vector, fsbr, br, len);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to handle vector: " + E.Message);
                    }
                }
            }
            Console.WriteLine("Found " + _labels.Count.ToString() + " in pass one");
            return _labels;
        }

        public bool DisassembleFile(IECUFile m_trionicFile, string inputfile, string outputfile/*, long startaddress*/, SymbolCollection symbols)
        {
            // recursive method when jsr was found
            mnemonics = new MNemonicCollection();
            labels = new MNemonicCollection();

            /*labels = */findLabels(m_trionicFile, inputfile);

            _passOne = false;

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
            for (int vec = 1; vec <= 127; vec++)
            {
                int percentage = (vec * 100) / 127;
                CastProgressEvent("Disassembling vectors", percentage, ProgressType.DisassemblingVectors);
                long vector = m_trionicFile.GetStartVectorAddress(inputfile, vec);

                long len = fi.Length;
                if (len == 0x20000) len = 0x60000;

                if (vector != 0 && vector < len * 2)
                {
                    //Console.WriteLine("Vector: " + vec.ToString() + " addr: " + vector.ToString("X8"));
                    try
                    {
                        DisassembleFunction(vector, symbols, fsbr, br, len);
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to handle vector: " + E.Message);
                    }
                }
            }

            CastProgressEvent("Translating vector labels", 0, ProgressType.TranslatingVectors);
            //Console.WriteLine("Translating vector labels");
            long[] vectors = m_trionicFile.GetVectorAddresses(m_trionicFile.GetFileInfo().Filename);
            int lblcount = 0;
            foreach (MNemonicHelper label in labels)
            {
                int percentage = (lblcount++ * 100) / labels.Count;
                CastProgressEvent("Translating vector labels", percentage, ProgressType.TranslatingVectors);
                for (i = 0; i < 128; i++)
                {
                    if (label.Address == /*m_trionicFile.GetStartVectorAddress(m_trionicFileInformation.Filename, i)*/ Convert.ToInt64(vectors.GetValue(i)))
                    {
                        switch (i)
                        {
                            case 1:
                                label.Mnemonic = "INIT_PROGRAM:";
                                break;
                            case 2:
                                label.Mnemonic = "BUS_ERROR:";
                                break;
                            case 3:
                                label.Mnemonic = "ADDRESS_ERROR:";
                                break;
                            case 4:
                                label.Mnemonic = "ILLEGAL_INSTRUCTION:";
                                break;
                            case 5:
                                label.Mnemonic = "DIVIDE_BY_ZERO:";
                                break;
                            case 6:
                                label.Mnemonic = "CHK12_INSTR:";
                                break;
                            case 7:
                                label.Mnemonic = "TRAPx_INSTR:";
                                break;
                            case 8:
                                label.Mnemonic = "PRIV_VIOLATION:";
                                break;
                            case 9:
                                label.Mnemonic = "TRACE:";
                                break;
                            case 10:
                                label.Mnemonic = "L1010_EMUL:";
                                break;
                            case 11:
                                label.Mnemonic = "L1111_EMUL:";
                                break;
                            case 12:
                                label.Mnemonic = "HW_BREAKPOINT:";
                                break;
                            case 13:
                                label.Mnemonic = "RESERVED:";
                                break;
                            case 14:
                                label.Mnemonic = "FMT_ERR1:";
                                break;
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                            case 21:
                            case 22:
                                label.Mnemonic = "UNASSIGNED:";
                                break;
                            case 23:
                                label.Mnemonic = "FFFFFFFF:";
                                break;
                            case 24:
                                label.Mnemonic = "SPURIOUS_INTERRUPT:";
                                break;
                            case 25:
                                label.Mnemonic = "LEVEL1_INTERUPT_AUTOVECTOR:";
                                break;
                            case 26:
                                label.Mnemonic = "LEVEL2_INTERUPT_AUTOVECTOR:";
                                break;
                            case 27:
                                label.Mnemonic = "LEVEL3_INTERUPT_AUTOVECTOR:";
                                break;
                            case 28:
                                label.Mnemonic = "LEVEL4_INTERUPT_AUTOVECTOR:";
                                break;
                            case 29:
                                label.Mnemonic = "LEVEL5_INTERUPT_AUTOVECTOR:";
                                break;
                            case 30:
                                label.Mnemonic = "LEVEL6_INTERUPT_AUTOVECTOR:";
                                break;
                            case 31:
                                label.Mnemonic = "LEVEL7_INTERUPT_AUTOVECTOR:";
                                break;
                            case 32:
                                label.Mnemonic = "TAP0_INSTRUCTION_VECTOR:";
                                break;
                            case 33:
                                label.Mnemonic = "TAP1_INSTRUCTION_VECTOR:";
                                break;
                            case 34:
                                label.Mnemonic = "TAP2_INSTRUCTION_VECTOR:";
                                break;
                            case 35:
                                label.Mnemonic = "TAP3_INSTRUCTION_VECTOR:";
                                break;
                            case 36:
                                label.Mnemonic = "TAP4_INSTRUCTION_VECTOR:";
                                break;
                            case 37:
                                label.Mnemonic = "TAP5_INSTRUCTION_VECTOR:";
                                break;
                            case 38:
                                label.Mnemonic = "TAP6_INSTRUCTION_VECTOR:";
                                break;
                            case 39:
                                label.Mnemonic = "TAP7_INSTRUCTION_VECTOR:";
                                break;
                            case 40:
                                label.Mnemonic = "TAP8_INSTRUCTION_VECTOR:";
                                break;
                            case 41:
                                label.Mnemonic = "TAP9_INSTRUCTION_VECTOR:";
                                break;
                            case 42:
                                label.Mnemonic = "TAP10_INSTRUCTION_VECTOR:";
                                break;
                            case 43:
                                label.Mnemonic = "TAP11_INSTRUCTION_VECTOR:";
                                break;
                            case 44:
                                label.Mnemonic = "TAP12_INSTRUCTION_VECTOR:";
                                break;
                            case 45:
                                label.Mnemonic = "TAP13_INSTRUCTION_VECTOR:";
                                break;
                            case 46:
                                label.Mnemonic = "TAP14_INSTRUCTION_VECTOR:";
                                break;
                            case 47:
                                label.Mnemonic = "TAP15_INSTRUCTION_VECTOR:";
                                break;
                            default:
                                label.Mnemonic = "VECTOR_" + i.ToString() + ":";
                                break;
                        }

                        break;
                    }
                }
            }
            /*
            Console.WriteLine("Translating known functions");
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
            //Console.WriteLine("Adding labels");
            lblcount = 0;
            foreach (MNemonicHelper label in labels)
            {
                int percentage = (lblcount++ * 100) / labels.Count;
                CastProgressEvent("Adding labels", percentage, ProgressType.AddingLabels);

                label.Address--; // for sequencing
                mnemonics.Add(label);
            }

            //Console.WriteLine("Sorting data");
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
            //Console.WriteLine("DisassembleFunction: " + addr.ToString("X8"));
            MNemonicCollection functionList = new MNemonicCollection();

            MNemonicHelper label = new MNemonicHelper();
            //long realAddr = fs.Position + offset;
            //label.Mnemonic = "Function_" + realAddr.ToString("X8") + ":";
            label.Mnemonic = "Function_" + addr.ToString("X8") +":";
            label.Address = addr;
            if (AddressInMnemonicList(addr))
            {
                //Console.WriteLine("Already disassembled: " + addr.ToString("X8"));
                return ;
            }
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
                    Console.WriteLine("break!");
                }*/
                uint t = disasm(out str, addr, ch1, ch2, offaddr, br, out endsub, out issub, out isjump);
                //Console.WriteLine(str);
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
                        Console.WriteLine("break!");
                    }*/
                    
                    // alleen als die nog niet geweest is
                    if (trgdata != 0)
                    {
                        if (!AddressInMnemonicList(trgdata))
                        {
                            if (trgdata < 0x00F00000)
                            {
                                long position = fs.Position;
                                //Console.WriteLine("recursive: " + trgdata.ToString("X8") + " curr address: " + addr.ToString("X8")); 
                                DisassembleFunction(trgdata, symbols, fs, br, offset);
                                //Console.WriteLine("After recursion: " + addr.ToString("X8"));
                                fs.Position = position; // reset to previous position
                            }
                        }
                    }
                }
                if (endsub)
                {
                }
            }

            // assign a meaningful name to the function if we can
            // we know what the rom->ram copy routine looks like
            bool _has_Rom_IgnitionMap = false;
            bool _has_Ram_IgnitionMap = false;
            bool _has_Rom_FuelMap = false;
            bool _has_Ram_FuelMap = false;
            bool _has_KontrollOrd = false;
            bool _has_Da_insp = false;
            bool _has_Tq = false;
            bool _has_EB = false;
            bool _has_IdleNeutral = false;
            string _functionName = string.Empty;
            foreach (MNemonicHelper functionHelper in functionList)
            {
                //Console.WriteLine(functionHelper.Address.ToString("X8") + " " + functionHelper.Mnemonic);
                if (functionHelper.Mnemonic.Contains("ROM_Ign_map_0!")) _has_Rom_IgnitionMap = true;
                if (functionHelper.Mnemonic.Contains("RAM_Ign_map_0!")) _has_Ram_IgnitionMap = true;
                if (functionHelper.Mnemonic.Contains("RAM_Insp_mat!")) _has_Ram_FuelMap = true;
                if (functionHelper.Mnemonic.Contains("ROM_Insp_mat!")) _has_Rom_FuelMap = true;
                if (functionHelper.Mnemonic.Contains("#ABCD")) _has_KontrollOrd = true;
                if (functionHelper.Mnemonic.Contains("Da_insp")) _has_Da_insp = true;
                if (functionHelper.Mnemonic.Contains("#EB")) _has_EB = true;
                if (functionHelper.Mnemonic.Contains("Tq")) _has_Tq = true;
                if (functionHelper.Mnemonic.Contains("Idle_rpm_offNeutral")) _has_IdleNeutral = true;
                //Idle_rpm_offNeutral
            }


            if (_has_Ram_IgnitionMap && _has_Rom_IgnitionMap) _functionName = "CopyIgnitionRomToRam:";
            else if (_has_Ram_FuelMap && _has_Rom_FuelMap) _functionName = "CopyFuelRomToRam:";
            else if (_has_Ram_IgnitionMap) _functionName = "CalculateIgnitionAngle:";
            else if (_has_Ram_FuelMap && !_has_Rom_FuelMap) _functionName = "CalculateInjectionDuration:";
            else if (_has_KontrollOrd) _functionName = "CheckSRAMIntegrity:";
            else if (_has_Da_insp) _functionName = "CalcInjectionForCylinder:";
            else if (_has_EB && _has_Tq) _functionName = "CalculateTorque:";
            else if (_has_IdleNeutral) _functionName = "DetermineIdleStatus:";

            if (_functionName != string.Empty)
            {
                label.Mnemonic = _functionName;                
            }
            labels.Add(label);

            //Console.WriteLine("Done with function: " + mnemonics.Count.ToString());
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
            MNemonicHelper label = new MNemonicHelper();
            MNemonicCollection functionList = new MNemonicCollection();
            label.Mnemonic = "Function_" + addr.ToString("X8") + ":";
            label.Address = addr;
            if (AddressInLabelList(addr))
            {
                return;
            }
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
                            if (trgdata < 0x00F00000)
                            {
                                long position = fs.Position;
                                LoadLabels(trgdata,  fs, br, offset);
                                fs.Position = position; // reset to previous position
                            }
                        }
                    }
                }
                if (endsub)
                {
                }
            }

            // assign a meaningful name to the function if we can
            // we know what the rom->ram copy routine looks like
            bool _has_Rom_IgnitionMap = false;
            bool _has_Ram_IgnitionMap = false;
            bool _has_Rom_FuelMap = false;
            bool _has_Ram_FuelMap = false;
            bool _has_KontrollOrd = false;
            bool _has_Da_insp = false;
            bool _has_Tq = false;
            bool _has_EB = false;
            bool _has_IdleNeutral = false;
            string _functionName = string.Empty;
            foreach (MNemonicHelper functionHelper in functionList)
            {
                //Console.WriteLine(functionHelper.Address.ToString("X8") + " " + functionHelper.Mnemonic);
                if (functionHelper.Mnemonic.Contains("ROM_Ign_map_0!")) _has_Rom_IgnitionMap = true;
                if (functionHelper.Mnemonic.Contains("RAM_Ign_map_0!")) _has_Ram_IgnitionMap = true;
                if (functionHelper.Mnemonic.Contains("RAM_Insp_mat!")) _has_Ram_FuelMap = true;
                if (functionHelper.Mnemonic.Contains("ROM_Insp_mat!")) _has_Rom_FuelMap = true;
                if (functionHelper.Mnemonic.Contains("#ABCD")) _has_KontrollOrd = true;
                if (functionHelper.Mnemonic.Contains("Da_insp")) _has_Da_insp = true;
                if (functionHelper.Mnemonic.Contains("#EB")) _has_EB = true;
                if (functionHelper.Mnemonic.Contains("Tq")) _has_Tq = true;
                if (functionHelper.Mnemonic.Contains("Idle_rpm_offNeutral")) _has_IdleNeutral = true;
                //Idle_rpm_offNeutral
            }


            if (_has_Ram_IgnitionMap && _has_Rom_IgnitionMap) _functionName = "CopyIgnitionRomToRam:";
            else if (_has_Ram_FuelMap && _has_Rom_FuelMap) _functionName = "CopyFuelRomToRam:";
            else if (_has_Ram_IgnitionMap) _functionName = "CalculateIgnitionAngle:";
            else if (_has_Ram_FuelMap && !_has_Rom_FuelMap) _functionName = "CalculateInjectionDuration:";
            else if (_has_KontrollOrd) _functionName = "CheckSRAMIntegrity:";
            else if (_has_Da_insp) _functionName = "CalcInjectionForCylinder:";
            else if (_has_EB && _has_Tq) _functionName = "CalculateTorque:";
            else if (_has_IdleNeutral) _functionName = "DetermineIdleStatus:";

            if (_functionName != string.Empty)
            {
                label.Mnemonic = _functionName;
            }
            _labels.Add(label);
        }

        private bool AddressInMnemonicList(long trgdata)
        {
            foreach (MNemonicHelper helper in Mnemonics)
            {
                if (helper.Address == trgdata) return true;
            }
            return false;
        }


        public bool DisassembleFile(bool AddOffset, long OffSetAddress, string inputfile, string outputfile, long startaddress, long endaddress, SymbolCollection symbols)
        {
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
            m_symbols = symbols;
            swap = 0;
            addr = offaddr = 0;

            inname = inputfile;
            //infile = 1;
            outname = outputfile;
            //outfile = 1;
            if (AddOffset)
            {
                addoff = 1;
                offaddr = OffSetAddress;
            }

            endaddr = endaddress;
            addr = startaddress;
            if (addr > endaddress) return false;
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
                    Console.WriteLine(E.Message);
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
