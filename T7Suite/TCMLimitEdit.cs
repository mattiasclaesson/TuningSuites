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
        byte[] m_moddedSequence;
        byte[] m_originalSequence;

        // Methods
        private bool findByteSequence(byte[] sequence, byte[] seq_mask)
        {
            int index = 0;
            int length = sequence.Length;
            FileStream stream = null;
            try
            {
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.Read);
                while (stream.Position < this.m_fileLength)
                {
                    byte num = (byte)stream.ReadByte();
                    if ((num == sequence[index]) || (seq_mask[index] == 0))
                    {
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
            return false;
        }

/* myrtilos @ trionictuning.com 
        Here's what i will do : I will use this symbol as a threshold :
> If TCM is below the limit, the TCM value will remain unchanged, if we are above, then I will replace by 450NM...
In my specific case I will put "331" as I want to keep the limiter working at 330NM on 1st & 2nd gears !

Original code :
[code]0004EA24:	MOVE.W	#FF9C,(ROM_ActualIn.M_TCMLda)								
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
[/code]

Here's what i propose... Only 20 bytes changed to implement the threshold feature explained earlier :
[code]0004EA24:	MOVE.W	#FF9C,(ROM_ActualIn.M_TCMLda)
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
[/code]
        */
        // Original bin = 0C2B00050006660C4A6B00666606D479ZZZZZZZZ
        // Modded bin = 4E714A6B0066660CB479ZZZZZZZZ6F04343C01C2
        // where ZZZZZZZZ is the adress of VISOCal.M_TCMOffset as you have guessed
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

            m_moddedSequence = new byte[] { 0x4E, 0x71, 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x0C, 0xB4, 0x79, (byte)address4, (byte)address3, (byte)address2, (byte)address1, 0x6F, 0x04, 0x34, 0x3C, 0x01, 0xC2 };
            m_originalSequence = new byte[] { 0x0C, 0x2B, 0x00, 0x05, 0x00, 0x06, 0x66, 0x0C, 0x4A, 0x6B, 0x00, 0x66, 0x66, 0x06, 0xD4, 0x79, (byte)address4, (byte)address3, (byte)address2, (byte)address1 };
            byte[] mask = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            
            // look for the original sequence    
            found = this.findByteSequence(m_originalSequence, mask);
            m_modified = !found;

            // look for the modded sequence
            if (!found)
            {
                found = this.findByteSequence(m_moddedSequence, mask);
                m_modified = found;
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
