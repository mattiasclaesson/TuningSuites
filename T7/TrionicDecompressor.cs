using System;
using System.Collections.Generic;
using System.IO;
using CommonSuite;

namespace T7
{
    public class TrionicDecompressor
    {
        private byte[]  byte_69A02C = new byte[1024];
        private byte[]  byte_69A12C = new byte[1024];
        private uint[] dword_704098 = new uint[1024];
        private uint[] dword_702818 = new uint[1024];
        private uint   dword_7031E0 = 0;
        private uint   dword_703BAC = 0;
        private uint[] dword_703BB0 = new uint[1024];
        private uint[] dword_70409C = new uint[1024];
        private uint[] dword_7031E8 = new uint[1024];
        private uint[] dword_7031E4 = new uint[1024];
        private uint   dword_704A64 = 0;
        private byte[]  byte_710E7C = new byte[1024];
        private byte    byte_711ED0 = 0;
        private uint   dword_711EC8 = 0;
        private uint   dword_711ECC = 0;


        private uint SubRoutine1()
        {
            uint result=0; // eax@3
            uint v1; // [sp+4h] [bp-4h]@1
            uint v2; // [sp+0h] [bp-8h]@4

            v1 = 0;
            while (v1 < 314)
            {
                dword_704098[v1] = 1;
                dword_702818[v1] = v1 + 627;
                dword_703BB0[v1] = v1;
                result = v1++ + 1;
            }
            v1 = 0;
            v2 = 314;
            while (v2 <= 626)
            {
                dword_704098[v2] = dword_70409C[v1] + dword_704098[v1];
                dword_702818[v2] = v1;
                dword_7031E8[v1] = v2;
                result = v2;
                dword_7031E4[v1] = v2;
                v1 += 2;
                ++v2;
            }
            dword_70409C[2] = 65535; // dword_704A64
            dword_703BAC = 0;
            return result;
        }

        private uint SubRoutine3(uint a1, BinaryReader br)
        {
           uint v2=0; // eax@3
           uint v3=0; // [sp+0h] [bp-4h]@1
            v3 = a1;
            while ( (int)(byte)byte_711ED0 <= 8 )
            {
                v2 = Convert.ToUInt32(br.ReadByte());
                v3 = v2;
                if ( v2 < 0 )
                v3 = 0;
                dword_711ECC |= v3 << (8 - (byte)byte_711ED0);
                byte_711ED0 += 8;
            }
            v3 = dword_711ECC;
            dword_711ECC *= 2;
            --byte_711ED0;
            return (v3 & 0x8000) >> 15;
        }

        private uint SubRoutine2(BinaryReader br)
        {
            uint a1=0;
            uint v2; // eax@3
            uint v3; // [sp+0h] [bp-4h]@1
            v3 = dword_7031E0;
            while ( (uint)v3 < 0x273 )
            {
                v2 = SubRoutine3(a1, br);
                a1 = v2 + v3;
                v3 += v2;
                v3 = (uint)dword_702818[v3];
            }
            v3 -= 627;
            sub_4AB7A9(v3);
            return v3;
        }

        private uint sub_4AB8EC()
        {
            uint result=0; // eax@5
            uint v1; // [sp+8h] [bp-Ch]@1
            uint v2; // [sp+Ch] [bp-8h]@1
            uint v3; // [sp+4h] [bp-10h]@8
            uint v4; // [sp+10h] [bp-4h]@8
            /*size_t*/ uint Size; // [sp+0h] [bp-14h]@11

            v1 = 0;
            v2 = 0;
            while ( v2 < 627 )
            {
              if ( dword_702818[v2] >= 627 )
              {
                dword_704098[v1] = (uint)(dword_704098[v2] + 1) >> 1;
                dword_702818[v1++] = dword_702818[v2];
              }
              result = v2++ + 1;
            }
            v2 = 0;
            v1 = 314;
            while ( v1 < 627 )
            {
              v3 = v2 + 1;
              dword_704098[v1] = dword_704098[v2 + 1] + dword_704098[v2];
              v4 = dword_704098[v1];
              v3 = v1 - 1;
              while ( v4 < dword_704098[v3] )
                --v3;
              ++v3;
              Size = 2 * (v1 - v3);
              for (int i = 0; i < Size; i++)
              {
                  dword_70409C[4 * v3 + i] = dword_70409C[4 * v3 - 4 + i];
              }
              //memmove((void *)(4 * v3 + 7356572), (const void *)(4 * v3 + 7356568), 2 * (v1 - v3));
              dword_704098[v3] = v4;
              for (int i = 0; i < Size; i++)
              {
                  dword_702818[4 * v3 + i + 4] = dword_702818[4 * v3 + i];
              }
              //memmove((void *)(4 * v3 + 7350300), (const void *)(4 * v3 + 7350296), Size);
              dword_702818[v3] = v2;
              result = v2 + 2;
              v2 += 2;
              ++v1;
            }
            v2 = 0;
            while ( v2 < 627 )
            {
              v3 = dword_702818[v2];
              if ( v3 < 627 )
              {
                dword_7031E8[v3] = v2;
                result = v3;
                dword_7031E4[v3] = v2;
              }
              else
              {
                result = v2;
                dword_7031E4[v3] = v2;
              }
              ++v2;
            }
            return result;
        }

        private uint sub_4AB7A9(uint a1)
        {
            uint result; // eax@12
            uint v2; // [sp+4h] [bp-Ch]@4
            uint v3; // [sp+0h] [bp-10h]@4
            uint v4; // [sp+Ch] [bp-4h]@7
            uint v5; // [sp+8h] [bp-8h]@9

            if (dword_70409C[1] == 32768)   //dword_704A60
                sub_4AB8EC();
            a1 = dword_703BB0[a1];
            do
            {
                ++dword_704098[a1];
                v2 = dword_704098[a1];
                v3 = a1 + 1;
                if (v2 > dword_704098[a1 + 1])
                {
                    do
                        ++v3;
                    while (v2 > dword_704098[v3]);
                    --v3;
                    dword_704098[a1] = dword_704098[v3];
                    dword_704098[v3] = v2;
                    v4 = dword_702818[a1];
                    dword_7031E4[v4] = v3;
                    if (v4 < 627)
                        dword_7031E8[v4] = v3;
                    v5 = dword_702818[v3];
                    dword_702818[v3] = v4;
                    dword_7031E4[v5] = a1;
                    if (v5 < 627)
                        dword_7031E8[v5] = a1;
                    dword_702818[a1] = v5;
                    a1 = v3;
                }
                result = a1;
                a1 = dword_7031E4[a1];
            }
            while (a1>0);
            return result;
        }

        private uint sub_4ABB22(uint a1, BinaryReader br)
        {
            uint v2; // eax@3
            uint v3; // [sp+0h] [bp-4h]@1
            v3 = a1;
            while ((uint)(byte)byte_711ED0 <= 8)
            {
                v2 = Convert.ToUInt32(br.ReadByte());//getc(CompessedFilePointer);
                v3 = v2;
                if (v2 < 0)
                    v3 = 0;
                dword_711ECC |= v3 << (8 - (byte)byte_711ED0);
                byte_711ED0 += 8;
            }
            v3 = dword_711ECC;
            dword_711ECC <<= 8;
            byte_711ED0 -= 8;
            return (v3 & 0xFF00u) >> 8;
        }

        private uint sub_4AB712(uint a1, BinaryReader br)
        {
            uint v2; // eax@3
            uint v3; // [sp+0h] [bp-4h]@1

            v3 = a1;
            while ((uint)(byte)byte_711ED0 <= 8)
            {
                v2 = Convert.ToUInt32(br.ReadByte());//readgetc(CompessedFilePointer);
                v3 = v2;
                if (v2 < 0)
                    v3 = 0;
                dword_711ECC |= v3 << (8 - (byte)byte_711ED0);
                byte_711ED0 += 8;
            }
            v3 = dword_711ECC;
            dword_711ECC *= 2;
            --byte_711ED0;
            return (v3 & 0x8000u) >> 15;
        }



        private uint sub_4ABABD(BinaryReader br)
        {
            uint v0; // ecx@1
            uint v2; // eax@1
            uint v3; // eax@1
            uint v4; // edx@2
            uint v5; // eax@3
            uint v6; // [sp+4h] [bp-8h]@1
            uint v7; // [sp+8h] [bp-4h]@1
            uint v8; // [sp+0h] [bp-Ch]@1

            v2 = sub_4ABB22(0, br);
            v6 = v2;
            v7 = Convert.ToUInt32(byte_69A02C[v2]) << 6;
            v3 = byte_69A12C[v2];
            v0 = v3 - 2;
            v8 = v3 - 2;
            while (true)
            {
                v4 = v8--;
                if (v4 == 0)
                    break;
                v5 = sub_4AB712(v0, br);
                v0 = v6;
                v6 = v5 + 2 * v6;
            }
            return v7 | v6 & 0x3F;
        }

        public string DecompressFile(string filename)
        {
            string decompressedString = string.Empty;
            uint result; 
            uint v1; 
            uint v2; 
            uint v3; 
            uint v4;
            uint v5; 
            uint v6; 
            uint v7;
            uint DecompressedLength = 0;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    DecompressedLength = Convert.ToUInt32(br.ReadByte());
                    DecompressedLength |= Convert.ToUInt32(br.ReadByte()) << 8;
                    DecompressedLength |= Convert.ToUInt32(br.ReadByte()) << 16;
                    DecompressedLength |= Convert.ToUInt32(br.ReadByte()) << 24;
                    if (DecompressedLength > 0)
                    {
                        SubRoutine1();
                        v2 = 0;
                        while (v2 < 4036)
                            byte_710E7C[v2++] = 32;
                        v3 = 4036;
                        v4 = 0;
                        while (true)
                        {
                            result = v4;
                            if (v4 >= DecompressedLength) 
                                break;
                            v1 = SubRoutine2(br);
                            v5 = v1;
                            if (v1 >= 256)
                            {
                                v2 = (v3 - sub_4ABABD(br) - 1) & 0xFFF;
                                v6 = v5 - 253;
                                v7 = 0;
                                while (v7 < v6)
                                {
                                    v5 = byte_710E7C[((int)v7 + (int)v2) & 0xFFF];

                                    /*result = putc(v5, File);
                                    if (result == -1)
                                        return result;*/
                                    LogHelper.Log(v5.ToString("X4"));
                                    byte_710E7C[v3++] = Convert.ToByte(v5);
                                    v3 &= 0xFFFu;
                                    ++v4;
                                    ++v7;
                                }
                            }
                            else
                            {
                                /*result = putc(v5, File);
                                if (result == -1)
                                    return result;*/
                                LogHelper.Log(v5.ToString("X4"));
                                byte_710E7C[v3++] = Convert.ToByte(v5);
                                v3 &= 0xFFFu;
                                ++v4;
                            }
                            if (v4 > dword_711EC8)
                                dword_711EC8 += 1024;
                        }
                    }
                }
            }
            return decompressedString;
        }
    }
}
