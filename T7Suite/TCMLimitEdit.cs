using System;
using System.Collections.Generic;
using System.IO;
using CommonSuite;
using NLog;

namespace T7
{
    internal class TCMLimitEdit
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        // Fields
        private readonly long m_fileLength = 0x80000L;
        private string m_fileName;
        private long m_filePos = -1;
        private bool m_modified;
        private bool m_modifiedGearLimit;
        byte[] m_moddedSequence;
        byte[] m_originalSequence;
        byte[] m_moddedGearLimitSelection;

        // Methods
        private bool findByteSequence(byte[] sequence, byte[] seq_mask, out byte[] read)
        {
            int index = 0;
            int length = sequence.Length;
            read = new byte[length];
            FileStream stream = null;
            try
            {
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.Read);
                while (stream.Position < this.m_fileLength)
                {
                    byte num = (byte)stream.ReadByte();
                    if ((num == sequence[index]) || (seq_mask[index] == 0))
                    {
                        read[index] = num;
                        index++;
                    }
                    else
                    {
                        if (index > 0)
                        {
                            stream.Position -= index;
                        }
                        index = 0;
                    }
                    if (index == length)
                    {
                        break;
                    }
                }
                if (index == length)
                {
                    m_filePos = stream.Position - length;
                    return true; 
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            read = new byte[length];
            return false;
        }

/* myrtilos @ trionictuning.com 
        Here's what i will do : I will use this symbol as a threshold :
> If TCM is below the limit, the TCM value will remain unchanged, if we are above, then I will replace by 450NM...
In my specific case I will put "331" as I want to keep the limiter working at 330NM on 1st & 2nd gears !

Original code :
0004EA24:	MOVE.W	#FF9C,(ROM_ActualIn.M_TCMLda)								
0004EA2A:	MOVE.W	#FF9C,(ROM_In.M_TCMLda)								
0004EA30:	MOVE.W	D2,(ROM_ActualIn.M_TCMLimitReq)								
0004EA34:	MOVE.B	D3,(ROM_ActualIn.ST_TCMIntervType)								
0004EA38:	MOVE.B	D3,(ROM_In.ST_TCMIntervType)								
0004EA3C:	CMPI.B	#05,(ROM_In.X_ActualGear)								
0004EA42:	BNE	0004EA50								
0004EA44:	TST.W	(ROM_In.t_TCMTrqLimDuration)								
0004EA48:	BNE	0004EA50								
0004EA4A:	ADD.W	(ROM_VIOSCal.M_TCMOffset),D2								
0004EA50:	BTST	#05,(00F04AD6).L								
0004EA58:	BEQ	0004EA60								
0004EA5A:	MOVE.W	(00F04A30).L,D2								
0004EA60:	MOVE.W	D2,(ROM_In.M_TCMLimitReq)								
0004EA64:	MOVEM.L	(7)+,#1C0C								
0004EA68:	RTS									

Here's what i propose... Only 20 bytes changed to implement the threshold feature explained earlier :
0004EA24:	MOVE.W	#FF9C,(ROM_ActualIn.M_TCMLda)
0004EA2A:	MOVE.W	#FF9C,(ROM_In.M_TCMLda)
0004EA30:	MOVE.W	D2,(ROM_ActualIn.M_TCMLimitReq)
0004EA34:	MOVE.B	D3,(ROM_ActualIn.ST_TCMIntervType)
0004EA38:	MOVE.B	D3,(ROM_In.ST_TCMIntervType)
0004EA3C:	TST.W	(ROM_In.t_TCMTrqLimDuration)
0004EA42:	BNE	0004EA50
0004EA44:	CMP.W	(ROM_VIOSCal.M_TCMOffset), D2 
0004EA48:	BLE	0004EA50
0004EA4A:	MOVE.W	#01C2, D2
0004EA50:	BTST	#05,(00F04AD6).L
0004EA58:	BEQ	0004EA60
0004EA5A:	MOVE.W	(00F04A30).L,D2
0004EA60:	MOVE.W	D2,(ROM_In.M_TCMLimitReq)
0004EA64:	MOVEM.L	(7)+,#1C0C
0004EA68:	RTS	

TCM Mod V3
000511D4:   MOVE.W	#FF9C,(RAM_ActualIn.M_TCMLda)
000511DA:   MOVE.W	#FF9C,(RAM_In.M_TCMLda)
000511E0:   MOVE.W	D2,(RAM_ActualIn.M_TCMLimitReq)
000511E4:   MOVE.B	D3,(RAM_ActualIn.ST_TCMIntervType)
000511E8:   MOVE.B	D3,(RAM_In.ST_TCMIntervType)
000511EC:   TST.W     (RAM_In.t_TCMTrqLimDuration)
000511F0:   BNE	    00051200
000511F2:   MOVE.W	#012C,D2
000511F6:   CMPI.B	#05,(RAM_In.X_ActualGear)
000511FC:   BLS	    00051200
000511FE:   ROL.B	    #4,D2
00051200:   BTST      #05,(00F05E5C).L
00051208:   BEQ	    00051210
0005120A:   MOVE.W	(00F05DB4).L,D2
00051210:   MOVE.W	D2,(RAM_In.M_TCMLimitReq)
00051214:   MOVEM.L	(7)+,#1C0C	;Move Multiple Registers from memory
00051218:   RTS
        */

        // Original bin = 0C2B00050006660C4A6B00666606D479ZZZZZZZZ
        // Modded bin = 4E714A6B0066660CB479ZZZZZZZZ6F04343C01C2
        // where ZZZZZZZZ is the adress of VISOCal.M_TCMOffset as you have guessed
        //
        // TCM Mod V3 = 4A6B0066660E343CXXXX0C2BYYYY00066302E91A
        // where the AAAA and BBBB values should be customizable from the popup screen 
        // XXXX=1st gear torque (nm) value according the the following table
        // 1stgear  value   highest gears target value
        // -------------------------------------------
        // 279      10E     480
        // 287      11F     497
        // 300      12C     450
        // 319      13F     499
        // 334      14E     484
        // 350      15E     485
        // 365      16D     470
        // 382      17E     487
        // 398      18E     488
        //
        // YYYY=0005
        // Limit should be applied up to this gear
        // 
        public bool loadFile(string a_fileName, long tcmOffsetAddress)
        {
            bool found = false;
            if (!File.Exists(a_fileName))
            {
                return false;
            }
            this.m_fileName = a_fileName;

            ulong address4 = (uint)tcmOffsetAddress & 0xFF000000;
            address4 /= 0x1000000;
            ulong address3 = (uint)tcmOffsetAddress & 0x00FF0000;
            address3 /= 0x10000;
            ulong address2 = (uint)tcmOffsetAddress & 0x0000FF00;
            address2 /= 0x100;
            ulong address1 = (uint)tcmOffsetAddress & 0x000000FF;

            m_moddedGearLimitSelection = new byte[] { 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x0E, 0x34, 0x3C, 0xFF, 0xFF, 0x0C, 0x2B, 0xFF, 0xFF, 0x00, 0x06, 0x63, 0x02, 0xE9, 0x1A };
            m_moddedSequence = new byte[] { 0x4E, 0x71, 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x0C, 0xB4, 0x79, (byte)address4, (byte)address3, (byte)address2, (byte)address1, 0x6F, 0x04, 0x34, 0x3C, 0x01, 0xC2 };
            m_originalSequence = new byte[] { 0x0C, 0x2B, 0x00, 0x05, 0x00, 0x06, 0x66, 0x0C, 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x06, 0xD4, 0x79, (byte)address4, (byte)address3, (byte)address2, (byte)address1 };
            byte[] mask = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            byte[] maskGearLimit = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1 };
            byte[] read;
            
            // look for the original sequence    
            found = this.findByteSequence(m_originalSequence, mask, out read);
            m_modified = !found;

            // look for the modded sequence
            if (!found)
            {
                found = this.findByteSequence(m_moddedSequence, mask,  out read);
                m_modified = found;
            }

            // look for the gear limit modded sequence
            if (!found)
            {
                found = this.findByteSequence(m_moddedGearLimitSelection, maskGearLimit, out read);
                m_modifiedGearLimit = found;
                if (found)
                {
                    m_limit = 0x100 * read[8] + read[9];
                    m_gear = read[13];
                }
            }
            
            return found;
        }

        public void setModificationEnabled(bool a_value)
        {
            m_modified = a_value;
        }

        public bool getModificationEnabled()
        {
            return m_modified;
        }

        public void setGearLimitModificationEnabled(bool a_value)
        {
            m_modifiedGearLimit = a_value;
        }

        public bool getGearLimitModificationEnabled()
        {
            return m_modifiedGearLimit;
        }

        private int m_gear;
        public int Gear
        {
            get
            {
                return m_gear;
            }
            set
            {
                m_gear = value;
            }
        }

        private int m_limit;
        public int Limit
        {
            get
            {
                return m_limit;
            }
            set
            {
                m_limit = value;
            }
        }

        private void saveModification()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.ReadWrite);
                stream.Position = this.m_filePos;
                if(m_modified)
                {
                    foreach (byte b in m_moddedSequence)
                    {
                        stream.WriteByte(b);
                    }
                }
                else if (m_modifiedGearLimit)
                {
                    int limit2 = m_limit & 0xFF00;
                    limit2 /= 0x100;
                    int limit1 = m_limit & 0xFF;

                    m_moddedGearLimitSelection = new byte[] { 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x0E, 0x34, 0x3C, (byte)limit2, (byte)limit1, 0x0C, 0x2B, 0x00, (byte)m_gear, 0x00, 0x06, 0x63, 0x02, 0xE9, 0x1A };
                    foreach (byte b in m_moddedGearLimitSelection)
                    {
                        stream.WriteByte(b);
                    }
                }
                else
                {
                    foreach (byte b in m_originalSequence)
                    {
                        stream.WriteByte(b);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public void saveFile()
        {
            this.saveModification();
        }
    }
}
