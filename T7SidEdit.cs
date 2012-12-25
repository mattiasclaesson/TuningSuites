using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace T7
{
    internal class T7SidEdit
    {
        // Fields
        private string[] m_dataArrayAll;
        private string[] m_dataArrayNew;
        private long m_fileLength = 0x80000L;
        private string m_fileName;
        private long[] m_filePos = new long[2];
        private int m_fileType = -1;

        // Methods
        private void createDataArrays()
        {
            FileStream stream = null;
            this.m_dataArrayNew = new string[0xfc];
            this.m_dataArrayAll = new string[0x63];
            byte[] bytes = new byte[4];
            try
            {
                int num;
                int num2;
                int num3;
                int num12;
                int num13;
                int num18;
                int num19;
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.Read);
                switch (this.m_fileType)
                {
                    case 1:
                        stream.Position = this.m_filePos[1];
                        num = 0;
                        num2 = 0;
                        goto Label_013F;

                    case 2:
                        stream.Position = this.m_filePos[0];
                        num = 0;
                        num12 = 0;
                        goto Label_02FD;

                    case 3:
                        stream.Position = this.m_filePos[0];
                        num = 0;
                        num18 = 0;
                        goto Label_0454;

                    default:
                        goto Label_057C;
                }
            Label_006D:
                num3 = 0;
                while (num3 <= 3)
                {
                    bytes[num3] = (byte)stream.ReadByte();
                    num3++;
                }
                this.m_dataArrayNew[num++] = Encoding.Default.GetString(bytes, 0, 4);
                stream.Position += 3L;
                for (int i = 0; i <= 2; i++)
                {
                    bytes[i] = (byte)stream.ReadByte();
                }
                this.m_dataArrayNew[num++] = BitConverter.ToString(bytes, 0, 3).Replace("-", "");
                for (int j = 0; j <= 0; j++)
                {
                    bytes[j] = (byte)stream.ReadByte();
                }
                this.m_dataArrayNew[num++] = BitConverter.ToString(bytes, 0, 1).Replace("-", "");
                stream.Position += 1L;
                num2++;
            Label_013F:
                if (num2 < (this.m_dataArrayNew.Length / 3))
                {
                    goto Label_006D;
                }
                stream.Position = this.m_filePos[0];
                num = 0;
                for (int k = 0; k < (this.m_dataArrayAll.Length / 3); k++)
                {
                    for (int num7 = 0; num7 <= 3; num7++)
                    {
                        bytes[num7] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = Encoding.Default.GetString(bytes, 0, 4);
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position += 2L;
                num = 1;
                for (int m = 0; m < (this.m_dataArrayAll.Length / 3); m++)
                {
                    for (int num9 = 0; num9 <= 2; num9++)
                    {
                        bytes[num9] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 0, 3).Replace("-", "");
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position -= 1L;
                num = 2;
                for (int n = 0; n < (this.m_dataArrayAll.Length / 3); n++)
                {
                    for (int num11 = 0; num11 <= 0; num11++)
                    {
                        bytes[num11] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 0, 1).Replace("-", "");
                    num += 3;
                }
                return;
            Label_02B4:
                num13 = 0;
                while (num13 <= 3)
                {
                    bytes[num13] = (byte)stream.ReadByte();
                    num13++;
                }
                this.m_dataArrayAll[num] = Encoding.Default.GetString(bytes, 0, 4);
                stream.Position += 1L;
                num += 3;
                num12++;
            Label_02FD:
                if (num12 < (this.m_dataArrayAll.Length / 3))
                {
                    goto Label_02B4;
                }
                stream.Position += 2L;
                num = 1;
                for (int num14 = 0; num14 < (this.m_dataArrayAll.Length / 3); num14++)
                {
                    for (int num15 = 0; num15 <= 2; num15++)
                    {
                        bytes[num15] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 0, 3).Replace("-", "");
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position -= 1L;
                num = 2;
                for (int num16 = 0; num16 < (this.m_dataArrayAll.Length / 3); num16++)
                {
                    for (int num17 = 0; num17 <= 0; num17++)
                    {
                        bytes[num17] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 0, 1).Replace("-", "");
                    num += 3;
                }
                this.m_dataArrayNew = null;
                return;
            Label_040B:
                num19 = 0;
                while (num19 <= 3)
                {
                    bytes[num19] = (byte)stream.ReadByte();
                    num19++;
                }
                this.m_dataArrayAll[num] = Encoding.Default.GetString(bytes, 0, 4);
                stream.Position += 1L;
                num += 3;
                num18++;
            Label_0454:
                if (num18 < (this.m_dataArrayAll.Length / 3))
                {
                    goto Label_040B;
                }
                stream.Position += 2L;
                num = 1;
                bytes = new byte[4];
                for (int num20 = 0; num20 < (this.m_dataArrayAll.Length / 3); num20++)
                {
                    for (int num21 = 2; num21 >= 0; num21--)
                    {
                        bytes[num21] = (byte)stream.ReadByte();
                    }
                    uint num22 = BitConverter.ToUInt32(bytes, 0) - 0xef02f0;
                    bytes = BitConverter.GetBytes(num22);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 1, 3).Replace("-", "");
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position -= 1L;
                num = 2;
                for (int num23 = 0; num23 < (this.m_dataArrayAll.Length / 3); num23++)
                {
                    for (int num24 = 0; num24 <= 0; num24++)
                    {
                        bytes[num24] = (byte)stream.ReadByte();
                    }
                    this.m_dataArrayAll[num] = BitConverter.ToString(bytes, 0, 1).Replace("-", "");
                    num += 3;
                }
                this.m_dataArrayNew = null;
                return;
            Label_057C:
                this.m_dataArrayAll = null;
                this.m_dataArrayNew = null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private long findByteSequence(byte[] sequence, byte[] seq_mask)
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
                    return (stream.Position -= length);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return -1L;
        }

        public string[] getDataArrayAll()
        {
            return this.m_dataArrayAll;
        }

        public string[] getDataArrayNew()
        {
            return this.m_dataArrayNew;
        }

        public long[] getFilePos()
        {
            return this.m_filePos;
        }

        public int getFileType()
        {
            return this.m_fileType;
        }

        public string getT7Filename()
        {
            return this.m_fileName;
        }

        public bool init(string a_fileName)
        {
            if (!File.Exists(a_fileName))
            {
                return false;
            }
            this.setT7FileName(a_fileName);
            this.setFTypePos();
            if (this.m_fileType == -1)
            {
                return false;
            }
            this.createDataArrays();
            if (this.m_dataArrayAll == null)
            {
                return false;
            }
            return true;
        }

        private void saveDataArrays()
        {
            FileStream stream = null;
            byte[] array = new byte[5];
            byte[] buffer = new byte[3];
            byte[] buffer3 = new byte[1];
            try
            {
                int num;
                int num2;
                int num8;
                int num9;
                int num13;
                int num14;
                ASCIIEncoding encoding = new ASCIIEncoding();
                stream = new FileStream(this.m_fileName, FileMode.Open, FileAccess.ReadWrite);
                switch (this.m_fileType)
                {
                    case 1:
                        stream.Position = this.m_filePos[1];
                        num2 = 0;
                        goto Label_00CA;

                    case 2:
                        stream.Position = this.m_filePos[0];
                        num = 0;
                        num8 = 0;
                        goto Label_0264;

                    case 3:
                        stream.Position = this.m_filePos[0];
                        num = 0;
                        num13 = 0;
                        goto Label_0385;

                    default:
                        return;
                }
            Label_0061:
                array = encoding.GetBytes(this.m_dataArrayNew[num2++]);
                Console.WriteLine(this.m_dataArrayNew[num2 - 1]);
                Array.Resize<byte>(ref array, 7);
                stream.Write(array, 0, 7);
                buffer = ToByteArray(this.m_dataArrayNew[num2++]);
                Console.WriteLine(this.m_dataArrayNew[num2 - 1]);
                stream.Write(buffer, 0, 3);
                buffer3 = ToByteArray(this.m_dataArrayNew[num2++]);
                Console.WriteLine(this.m_dataArrayNew[num2 - 1]);
                Array.Resize<byte>(ref buffer3, 2);
                stream.Write(buffer3, 0, 2);
                System.Windows.Forms.Application.DoEvents();
            Label_00CA:
                if (num2 < this.m_dataArrayNew.Length)
                {
                    goto Label_0061;
                }
                stream.Position = this.m_filePos[0];
                num = 0;
                for (int i = 0; i < (this.m_dataArrayAll.Length / 3); i++)
                {
                    int length = this.m_dataArrayAll[num].Length;
                    array = encoding.GetBytes(this.m_dataArrayAll[num]);
                    Array.Resize<byte>(ref array, 5);
                    for (int num5 = length; num5 < array.Length; num5++)
                    {
                        array[num5] = 0x20;
                    }
                    stream.Write(array, 0, 5);
                    num += 3;
                }
                stream.Position += 2L;
                num = 1;
                for (int j = 0; j < (this.m_dataArrayAll.Length / 3); j++)
                {
                    buffer = ToByteArray(this.m_dataArrayAll[num]);
                    stream.Write(buffer, 0, 3);
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position -= 1L;
                num = 2;
                for (int k = 0; k < (this.m_dataArrayAll.Length / 3); k++)
                {
                    buffer3 = ToByteArray(this.m_dataArrayAll[num]);
                    stream.Write(buffer3, 0, 1);
                    num += 3;
                }
                return;
            Label_020D:
                num9 = this.m_dataArrayAll[num].Length;
                array = encoding.GetBytes(this.m_dataArrayAll[num]);
                Array.Resize<byte>(ref array, 5);
                for (int m = num9; m < array.Length; m++)
                {
                    array[m] = 0x20;
                }
                stream.Write(array, 0, 5);
                num += 3;
                num8++;
            Label_0264:
                if (num8 < (this.m_dataArrayAll.Length / 3))
                {
                    goto Label_020D;
                }
                stream.Position += 2L;
                num = 1;
                for (int n = 0; n < (this.m_dataArrayAll.Length / 3); n++)
                {
                    buffer = ToByteArray(this.m_dataArrayAll[num]);
                    stream.Write(buffer, 0, 3);
                    stream.Position += 1L;
                    num += 3;
                }
                stream.Position -= 1L;
                num = 2;
                for (int num12 = 0; num12 < (this.m_dataArrayAll.Length / 3); num12++)
                {
                    buffer3 = ToByteArray(this.m_dataArrayAll[num]);
                    stream.Write(buffer3, 0, 1);
                    num += 3;
                }
                return;
            Label_032E:
                num14 = this.m_dataArrayAll[num].Length;
                array = encoding.GetBytes(this.m_dataArrayAll[num]);
                Array.Resize<byte>(ref array, 5);
                for (int num15 = num14; num15 < array.Length; num15++)
                {
                    array[num15] = 0x20;
                }
                stream.Write(array, 0, 5);
                num += 3;
                num13++;
            Label_0385:
                if (num13 < (this.m_dataArrayAll.Length / 3))
                {
                    goto Label_032E;
                }
                stream.Position += 1L;
                num = 1;
                for (int num16 = 0; num16 < (this.m_dataArrayAll.Length / 3); num16++)
                {
                    buffer = ToByteArray(this.m_dataArrayAll[num]);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(buffer);
                    }
                    Array.Resize<byte>(ref buffer, 4);
                    uint num17 = BitConverter.ToUInt32(buffer, 0) + 0xef02f0;
                    buffer = BitConverter.GetBytes(num17);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(buffer);
                    }
                    stream.Write(buffer, 0, 4);
                    num += 3;
                }
                num = 2;
                for (int num18 = 0; num18 < (this.m_dataArrayAll.Length / 3); num18++)
                {
                    buffer3 = ToByteArray(this.m_dataArrayAll[num]);
                    stream.Write(buffer3, 0, 1);
                    num += 3;
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
        }

        public bool saveFile()
        {
            this.saveDataArrays();
            return true;
        }

        public void setDataArrayAll(string[] a_dataArrayAll)
        {
            this.m_dataArrayAll = a_dataArrayAll;
        }

        public void setDataArrayNew(string[] a_dataArrayNew)
        {
            this.m_dataArrayNew = a_dataArrayNew;
        }

        private void setFTypePos()
        {
            byte[] sequence = new byte[] { 0x41, 100, 0x70, 0x4e, 0, 0, 0 };
            byte[] buffer2 = new byte[] { 1, 1, 1, 1, 1, 1, 1 };
            long num = this.findByteSequence(sequence, buffer2);
            if (num != -1L)
            {
                this.m_fileType = 1;
                this.m_filePos[1] = num;
                sequence = new byte[] { 0x52, 0x70, 0x6d, 0x20, 0x20 };
                buffer2 = new byte[] { 1, 1, 1, 1, 1 };
                num = this.findByteSequence(sequence, buffer2);
                if (num != -1L)
                {
                    this.m_filePos[0] = num;
                }
                else
                {
                    this.m_filePos[0] = -1L;
                    this.m_fileType = -1;
                }
            }
            else
            {
                sequence = new byte[] { 0x52, 0x70, 0x6d, 0x20, 0x20 };
                buffer2 = new byte[] { 1, 1, 1, 1, 1 };
                num = this.findByteSequence(sequence, buffer2);
                if (num != -1L)
                {
                    this.m_fileType = 2;
                    this.m_filePos[0] = num;
                    this.m_filePos[1] = -1L;
                    if (num > 0x15f90L)
                    {
                        this.m_fileType = 3;
                    }
                    this.m_filePos[0] = num;
                    this.m_filePos[1] = -1L;
                }
                else
                {
                    this.m_fileType = -1;
                }
            }
        }

        public void setT7FileName(string a_fileName)
        {
            this.m_fileName = a_fileName;
        }

        public static byte[] ToByteArray(string HexString)
        {
            int length = HexString.Length;
            byte[] buffer = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 0x10);
            }
            return buffer;
        }
    }
}
