using System;
using System.Collections.Generic;
using System.IO;
using CommonSuite;
using NLog;
using System.Text;

namespace T8SuitePro
{
    public class srec2bin
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public bool ConvertSrecToBin(string filename, out string newfilename)
        {
            string readline = string.Empty;
            string readhex = string.Empty;
            newfilename = string.Empty;
            string outputfile = string.Empty;
            try
            {
                outputfile = Path.GetDirectoryName(filename);
                outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + ".bin");
                int bytecount = 0;
                ulong currentaddress = 0;
                FileStream fswrite = new FileStream(outputfile, FileMode.Create);
                BinaryWriter binwrite = new BinaryWriter(fswrite);

                using (StreamReader streamread = new StreamReader(filename))
                {
                    while ((readline = streamread.ReadLine()) != null)
                    {
                        // S0210000415050544F4F4C5F435F455F50000002000000000048FFFFFFFF001D00BAAD
                        // 0 Record. The type of record is 'S0' (0x5330). The address field is unused and will be filled with zeros (0x0000). 
                        // The header information within the data field is divided into the following subfields.
                        //  mname is char[20] and is the module name.
                        //  ver is char[2] and is the version number.
                        //  rev is char[2] and is the revision number.
                        //  description is char[0-36] and is a text comment.
                        // Each of the subfields is composed of ASCII bytes whose associated characters, when paired, represent one byte 
                        // hexadecimal values in the case of the version and revision numbers, or represent the hexadecimal values of the
                        // ASCII characters comprising the module name and description.
                        if (readline.StartsWith("S0"))
                        {
                            Int32 count = Int32.Parse(readline.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                            Int32 expectedNumberOfCharacters = 2*count - 2 - 4;
                            string data = readline.Substring(8, expectedNumberOfCharacters);

                            StringBuilder mname = new StringBuilder(20);
                            if (data.Length > 20)
                            {
                                for (int i = 0; i < 20; i += 2)
                                {
                                    string hs = data.Substring(i, 2);
                                    mname.Append(Convert.ToChar(Convert.ToUInt32(hs, 16)));
                                }
                            }

                            Int32 version = 0;
                            if (data.Length > 22)
                            {
                                version = Int32.Parse(readline.Substring(20, 2), System.Globalization.NumberStyles.HexNumber);
                            }

                            Int32 revision = 0;
                            if (data.Length > 24)
                            {
                                revision = Int32.Parse(readline.Substring(22, 2), System.Globalization.NumberStyles.HexNumber);
                            }

                            logger.Debug("Found S0 record count: {0} mname: {1} version: {2} revision: {3}", count, mname, version, revision);
                        }
                        // S319000200006A293624473D6D1877691341396C23ED473258D460
                        else if (readline.StartsWith("S3"))
                        {
                            Int32 count = Int32.Parse(readline.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                            //logger.Debug("Found S3 record count: {0}", count);

                            UInt64 address = Convert.ToUInt64(readline.Substring(4, 8), 16);
                            while (address - currentaddress > 0)
                            {
                                binwrite.Write((byte)0);
                                bytecount++;
                                currentaddress++;
                            }

                            Int32 expectedNumberOfCharacters = 2 * count - 2 - 8;
                            readhex = readline.Substring(12, expectedNumberOfCharacters);
                            for (int t = 0; t < expectedNumberOfCharacters; t += 2)
                            {
                                byte b = Convert.ToByte(readhex.Substring(t, 2), 16);
                                binwrite.Write(b);
                                bytecount++;
                                currentaddress++;
                            }
                        }
                        else if (readline.StartsWith("S2") && readline.Length > 75)
                        {
                            Int32 address = Convert.ToInt32(readline.Substring(4, 6), 16);
                            if (address < 0x100000)
                            {
                                readhex = readline.Substring(10, 64);
                                for (int t = 0; t < 64; t += 2)
                                {
                                    byte b = Convert.ToByte(readhex.Substring(t, 2), 16);
                                    binwrite.Write(b);
                                    bytecount++;
                                }
                            }

                            // logger.Debug("S2: " + bytecount.ToString());
                        }
                        else if (readline.StartsWith("S2") && readline.Length > 43)
                        {
                            Int32 address = Convert.ToInt32(readline.Substring(4, 6), 16);
                            if (address < 0x100000)
                            {
                                readhex = readline.Substring(10, 32);
                                for (int t = 0; t < 32; t += 2)
                                {
                                    byte b = Convert.ToByte(readhex.Substring(t, 2), 16);
                                    binwrite.Write(b);
                                    bytecount++;
                                }
                            }
                            // logger.Debug("S2: " + bytecount.ToString());
                        }
                        else if (readline.StartsWith("S1") && readline.Length > 41 && readline.Length <= 44)
                        {
                            //Int32 address = Convert.ToInt32(readline.Substring(4, 6), 16);
                            //if (address < 0x100000)
                            {
                                readhex = readline.Substring(8, 32);
                                for (int t = 0; t < 32; t += 2)
                                {
                                    byte b = Convert.ToByte(readhex.Substring(t, 2), 16);
                                    binwrite.Write(b);
                                    bytecount++;
                                }
                            }
                            //logger.Debug("S1: " + bytecount.ToString());
                        }

                    }

                    // pad
                    while (currentaddress < 0x100000)
                    {
                        binwrite.Write((byte)0);
                        bytecount++;
                        currentaddress++;
                    }
                }
                logger.Debug("Bytes written: " + bytecount.ToString());
                binwrite.Close();
                fswrite.Close();
                newfilename = outputfile;
                return true;
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return false;
        }

        public bool ConvertBinToSrec(string filename)
        {
            string outputfile = string.Empty;
            outputfile = Path.GetDirectoryName(filename);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + ".s19");
            return ConvertBinToSrec(filename, outputfile);
        }

        public bool ConvertBinToSrec(string filename, string newfilename)
        {
            int record_count = 0;
            int addr_bytes= 3;
            string outputfile = string.Empty;
            try
            {
                //outputfile = Path.GetDirectoryName(filename);
                //outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + ".s19");
                outputfile = newfilename;

                FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryReader binread = new BinaryReader(fsread);
                if (fsread.Length != 0x100000 && fsread.Length != 0x80000 && fsread.Length != 0x40000 && fsread.Length != 0x20000) return false;
                using (StreamWriter streamwriter = new StreamWriter(outputfile, false))
                {
                    // header naar bestand zetten
                    streamwriter.WriteLine("S00600004844521B");
                    byte c;
                    byte checksum = 0;
                    byte byte_count = 0;
                    string outpline = string.Empty;
                    for (int address = 0; address < fsread.Length; address += 32) // line-length = 32 (default)
                    {
                        byte[] inpbytes = binread.ReadBytes(32);
                        byte_count = (byte)(addr_bytes + 1 + inpbytes.Length);
                        outpline = "S2" + byte_count.ToString("X2");
                        checksum = (byte)byte_count;
                        for (int i = addr_bytes - 1; i >= 0; i--)
                        {
                            c = (byte)((address >> (i << 3)) & 0xff);
                            //printf("%02lX", c);
                            outpline += c.ToString("X2");
                            checksum += c;
                        }
                        for (int i = 0; i < inpbytes.Length; i++)
                        {
                            //printf("%02X", inpbytes.[i]);
                            byte b = (byte)inpbytes.GetValue(i);
                            outpline += b.ToString("X2");
                            checksum += b;
                        }
                        checksum = (byte)(255 - checksum);
                        outpline += checksum.ToString("X2");
                        //printf("%02X\n", 255 - checksum);
                        streamwriter.WriteLine(outpline);
                        record_count++;
                    }

                    checksum = (byte)((byte)3 + (byte)(record_count & 0xff) + (byte)((record_count >> 8) & 0xff));

                    checksum = (byte)(255 - checksum);
                    outpline = "S503" + record_count.ToString("X4") + checksum.ToString("X2");
                    streamwriter.WriteLine(outpline);
                    //printf("S503%04X%02X\n", record_count, 255 - checksum);

                    byte_count = (byte)(addr_bytes + 1);
                    int temp = 11 - addr_bytes;
                    outpline = "S" + temp.ToString() + byte_count.ToString("X2");
                    //streamwriter.WriteLine(outpline);
                    //printf("S%d%02X", 11 - addr_bytes, byte_count);

                    checksum = byte_count;
                    //outpline = string.Empty;
                    for (int i = addr_bytes - 1; i >= 0; i--)
                    {
                        c = (byte)((0 >> (i << 3)) & 0xff);
                        outpline += c.ToString("X2");
                        //printf("%02lX", c);
                        checksum += c;
                    }
                    checksum = (byte)(255 - checksum);
                    outpline += checksum.ToString("X2");
                    streamwriter.WriteLine(outpline);
                    //printf("%02X\n", 255 - checksum);
                }
                binread.Close();
                fsread.Close();
                return true;
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return false;
        }

        public bool ConvertBinToSrecS2Format(string filename)
        {
            int record_count = 0;
            int addr_bytes = 3;
            string outputfile = string.Empty;
            try
            {
                outputfile = Path.GetDirectoryName(filename);
                outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + ".s2");

                FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryReader binread = new BinaryReader(fsread);
                if (fsread.Length != 0x80000 && fsread.Length != 0x40000 && fsread.Length != 0x20000) return false;
                using (StreamWriter streamwriter = new StreamWriter(outputfile, false))
                {
                    // header naar bestand zetten
//                    streamwriter.WriteLine("S00600004844521B");
                    byte c;
                    byte checksum = 0;
                    byte byte_count = 0;
                    string outpline = string.Empty;
                    for (int address = (int)fsread.Length; address < (fsread.Length + fsread.Length); address += 32) // line-length = 32 (default)
                    {
                        byte[] inpbytes = binread.ReadBytes(32);
                        byte_count = (byte)(addr_bytes + 1 + inpbytes.Length);
                        outpline = "S2" + byte_count.ToString("X2");
  
                        checksum = (byte)byte_count;
                        for (int i = addr_bytes - 1; i >= 0; i--)
                        {
                            c = (byte)((address >> (i << 3)) & 0xff);
                            //printf("%02lX", c);
                            outpline += c.ToString("X2");
                            checksum += c;
                        }
                        for (int i = 0; i < inpbytes.Length; i++)
                        {
                            //printf("%02X", inpbytes.[i]);
                            byte b = (byte)inpbytes.GetValue(i);
                            outpline += b.ToString("X2");
                            checksum += b;
                        }
                        checksum = (byte)(255 - checksum);
                        outpline += checksum.ToString("X2");
                        //printf("%02X\n", 255 - checksum);
                        streamwriter.WriteLine(outpline);
                        record_count++;
                    }

                    /*checksum = (byte)((byte)3 + (byte)(record_count & 0xff) + (byte)((record_count >> 8) & 0xff));

                    checksum = (byte)(255 - checksum);
                    outpline = "S503" + record_count.ToString("X4") + checksum.ToString("X2");
                    streamwriter.WriteLine(outpline);
                    //printf("S503%04X%02X\n", record_count, 255 - checksum);

                    byte_count = (byte)(addr_bytes + 1);
                    int temp = 11 - addr_bytes;
                    outpline = "S" + temp.ToString() + byte_count.ToString("X2");
                    //streamwriter.WriteLine(outpline);
                    //printf("S%d%02X", 11 - addr_bytes, byte_count);

                    checksum = byte_count;
                    //outpline = string.Empty;
                    for (int i = addr_bytes - 1; i >= 0; i--)
                    {
                        c = (byte)((0 >> (i << 3)) & 0xff);
                        outpline += c.ToString("X2");
                        //printf("%02lX", c);
                        checksum += c;
                    }
                    checksum = (byte)(255 - checksum);
                    outpline += checksum.ToString("X2");
                    streamwriter.WriteLine(outpline);*/
                    //printf("%02X\n", 255 - checksum);
                }
                binread.Close();
                fsread.Close();
                return true;
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return false;
        }
    }
}
