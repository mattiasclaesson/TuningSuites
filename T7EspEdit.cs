using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace T7
{
    internal class T7EspEdit
    {
        // Fields
        private readonly long m_fileLength = 0x80000L;
        private string m_fileName;
        private long m_filePos = -1;
        private byte m_espValue;

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
                    m_filePos = stream.Position;
                    m_espValue = (byte)stream.ReadByte();
                    return true; 
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
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

        public bool loadFile(string a_fileName)
        {
            if (!File.Exists(a_fileName))
            {
                return false;
            }
            this.m_fileName = a_fileName; 
            byte[] sequence = new byte[] { 0xF0, 0x03, 0x34, 0x4e, 0x75 };
            byte[] buffer2 = new byte[] { 1, 1, 1, 1, 1 };
            return this.findByteSequence(sequence, buffer2); ;
        }

        public void setEspValue(byte a_value)
        {
            m_espValue = a_value;
        }

        public byte getEspValue()
        {
            return m_espValue;
        }

        private void saveEspByte()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.ReadWrite);
                stream.Position = this.m_filePos;
                stream.WriteByte(m_espValue);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
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
            this.saveEspByte();
        }
    }
}
