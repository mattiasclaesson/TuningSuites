using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NLog;

namespace CommonSuite
{
    public class Srecord
    {
        private const Int32 RECORD_TYPE_LENGTH = 2;
        private const Int32 COUNT_LENGTH = 2;
        private const Int32 S0_PAD_LENGTH = 4;
        private const Int32 S1_ADDRESS_LENGTH = 4;
        private const Int32 S2_ADDRESS_LENGTH = 6;
        private const Int32 S3_ADDRESS_LENGTH = 8;
        private const Int32 CHECKSUM_LENGTH = 2;
        private const Int32 FROM_BASE_16 = 16;
        private const byte ZERO = 0;

        static private Logger logger = LogManager.GetCurrentClassLogger();

        public bool ConvertSrecToBin(string filename, ulong size, out string newfilename, bool pad)
        {
            string readline = string.Empty;
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
                            Int32 count = Int32.Parse(readline.Substring(2, COUNT_LENGTH), System.Globalization.NumberStyles.HexNumber);
                            Int32 expectedNumberOfCharacters = 2 * count - CHECKSUM_LENGTH - RECORD_TYPE_LENGTH - COUNT_LENGTH;
                            string data = readline.Substring(RECORD_TYPE_LENGTH + COUNT_LENGTH + S0_PAD_LENGTH, expectedNumberOfCharacters);

                            StringBuilder mname = new StringBuilder(20);
                            if (data.Length > 20)
                            {
                                for (int i = 0; i < 20; i += 2)
                                {
                                    string hs = data.Substring(i, 2);
                                    mname.Append(Convert.ToChar(Convert.ToUInt32(hs, FROM_BASE_16)));
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

                            VerifyCheckSum(readline, count);
                        }
                        // S11F00007C0802A6900100049421FFF07C6C1B787C8C23783C6000003863000026
                        else if (readline.StartsWith("S1"))
                        {
                            DecodeDataField(readline, ref bytecount, ref currentaddress, binwrite, S1_ADDRESS_LENGTH, pad);
                        }
                        // S214000000FFFFEFFC0004A478000418420004185810
                        else if (readline.StartsWith("S2"))
                        {
                            DecodeDataField(readline, ref bytecount, ref currentaddress, binwrite, S2_ADDRESS_LENGTH, pad);
                        }
                        // S319000200006A293624473D6D1877691341396C23ED473258D460
                        else if (readline.StartsWith("S3"))
                        {
                            DecodeDataField(readline, ref bytecount, ref currentaddress, binwrite, S3_ADDRESS_LENGTH, pad);
                        }
                    }

                    if (pad)
                    {
                        while (currentaddress < size)
                        {
                            binwrite.Write(ZERO);
                            bytecount++;
                            currentaddress++;
                        }
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
                logger.Error(E);
            }
            return false;
        }

        private static void VerifyCheckSum(string readline, Int32 count)
        {
            byte calculatedCheckSum = 0;
            Int32 allCheckSumCharacters = 2 * count + COUNT_LENGTH - CHECKSUM_LENGTH;
            string checkData = readline.Substring(2, allCheckSumCharacters);
            for (int i = 0; i < allCheckSumCharacters; i += 2)
            {
                string hs = checkData.Substring(i, 2);
                calculatedCheckSum += Convert.ToByte(hs, FROM_BASE_16);
            }
            calculatedCheckSum = (byte)(255 - calculatedCheckSum);

            Int32 expectedCheckSum = Int32.Parse(readline.Substring(RECORD_TYPE_LENGTH + allCheckSumCharacters, CHECKSUM_LENGTH), System.Globalization.NumberStyles.HexNumber);
            if (calculatedCheckSum != expectedCheckSum)
            {
                string error = string.Format("Line {0} Calculated checksum {1}, expected checksum {2}", readline, calculatedCheckSum, expectedCheckSum);
                logger.Error(error);
                throw new Exception(error);
            }
        }

        private static void DecodeDataField(string readline, ref int bytecount, ref ulong currentaddress, BinaryWriter binwrite, Int32 addressLength, bool pad)
        {
            Int32 count = Int32.Parse(readline.Substring(2, COUNT_LENGTH), System.Globalization.NumberStyles.HexNumber);

            UInt64 address = Convert.ToUInt64(readline.Substring(RECORD_TYPE_LENGTH + COUNT_LENGTH, addressLength), FROM_BASE_16);
            if (pad)
            {
                while (address - currentaddress > 0)
                {
                    binwrite.Write(ZERO);
                    bytecount++;
                    currentaddress++;
                }
            }

            Int32 expectedNumberOfCharacters = 2 * count - CHECKSUM_LENGTH - addressLength;
            string readhex = readline.Substring(RECORD_TYPE_LENGTH + COUNT_LENGTH + addressLength, expectedNumberOfCharacters);
            for (int t = 0; t < expectedNumberOfCharacters; t += 2)
            {
                byte b = Convert.ToByte(readhex.Substring(t, 2), FROM_BASE_16);
                binwrite.Write(b);
                bytecount++;
                currentaddress++;
            }

            VerifyCheckSum(readline, count);
        }

        public bool ConvertBinToSrec(string filename, ulong size)
        {
            string outputfile = string.Empty;
            outputfile = Path.GetDirectoryName(filename);
            outputfile = Path.Combine(outputfile, Path.GetFileNameWithoutExtension(filename) + ".s19");
            return ConvertBinToSrec(filename, size, outputfile);
        }

        public bool ConvertBinToSrec(string filename, ulong size, string newfilename)
        {
            int record_count = 0;
            int addr_bytes= 3;
            string outputfile = string.Empty;
            try
            {
                outputfile = newfilename;

                FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryReader binread = new BinaryReader(fsread);
                if (fsread.Length != (long)size) return false;
                using (StreamWriter streamwriter = new StreamWriter(outputfile, false))
                {
                    // put header to file
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
                logger.Error(E);
            }
            return false;
        }
    }
}
