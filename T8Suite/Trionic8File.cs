using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using CommonSuite;
using NLog;
using System.Windows.Forms;
using BlowFishCS;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using TrionicCANLib.Firmware;

namespace T8SuitePro
{
    public class Trionic8File
    {
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();

        public enum VectorType : int
        {
            Reset_initial_stack_pointer,
            Reset_initial_program_counter,
            Bus_error,
            Address_error,
            Illegal_instruction,
            Zero_division,
            CHK_CHK2_instructions,
            TRAPcc_TRAPV_instructions,
            Privilege_violation,
            Trace,
            Line_1010_emulator,
            Line_1111_emulator,
            Hardware_breakpoint,
            Coprocessor_protocol_violation,
            Format_error_and_uninitialized_interrupt_1,
            Format_error_and_uninitialized_interrupt_2,
            Unassigned_reserved_1,
            Unassigned_reserved_2,
            Unassigned_reserved_3,
            Unassigned_reserved_4,
            Unassigned_reserved_5,
            Unassigned_reserved_6,
            Unassigned_reserved_7,
            Unassigned_reserved_8,
            Spurious_interrupt,
            Level_1_interrupt_autovector,
            Level_2_interrupt_autovector,
            Level_3_interrupt_autovector,
            Level_4_interrupt_autovector,
            Level_5_interrupt_autovector,
            Level_6_interrupt_autovector,
            Level_7_interrupt_autovector,
            Trap_instruction_vector_0,
            Trap_instruction_vector_1,
            Trap_instruction_vector_2,
            Trap_instruction_vector_3,
            Trap_instruction_vector_4,
            Trap_instruction_vector_5,
            Trap_instruction_vector_6,
            Trap_instruction_vector_7,
            Trap_instruction_vector_8,
            Trap_instruction_vector_9,
            Trap_instruction_vector_10,
            Trap_instruction_vector_11,
            Trap_instruction_vector_12,
            Trap_instruction_vector_13,
            Trap_instruction_vector_14,
            Trap_instruction_vector_15,
            Reserved_coprocessor_0,
            Reserved_coprocessor_1,
            Reserved_coprocessor_2,
            Reserved_coprocessor_3,
            Reserved_coprocessor_4,
            Reserved_coprocessor_5,
            Reserved_coprocessor_6,
            Reserved_coprocessor_7,
            Reserved_coprocessor_8,
            Reserved_coprocessor_9,
            Reserved_coprocessor_10,
            Unassigned_reserved_9,
            Unassigned_reserved_10,
            Unassigned_reserved_11,
            Unassigned_reserved_12,
            Unassigned_reserved_13
        }

        private int m_sramOffsetForOpenFile = 0;

        public int SramOffsetForOpenFile
        {
            get { return m_sramOffsetForOpenFile; }
            set { m_sramOffsetForOpenFile = value; }
        }

        private static void CastProgressEvent(string info, int percentage)
        {
            if (onProgress != null)
            {
                onProgress(typeof(Trionic8File), new ProgressEventArgs(info, percentage));
            }
        }

        public class ProgressEventArgs : System.EventArgs
        {
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

            public ProgressEventArgs(string info, int percentage)
            {
                this._info = info;
                this._percentage = percentage;
            }
        }

        public delegate void Progress(object sender, ProgressEventArgs e);
        public static event Trionic8File.Progress onProgress;

        static private int ReadMarkerAddress(string filename, int value, int filelength, out int length, out string val)
        {
            int retval = 0;
            length = 0;
            val = string.Empty;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = filelength - 0x100;

                    fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                    byte[] inb = br.ReadBytes(0xFF);
                    //int offset = 0;
                    for (int t = 0; t < 0xFF; t++)
                    {
                        if (((byte)inb.GetValue(t) == (byte)value) && ((byte)inb.GetValue(t + 1) < 0x30))
                        {
                            // marker gevonden
                            // lees 6 terug
                            retval = /*0x3FF00*/ fileoffset + t;
                            length = (byte)inb.GetValue(t + 1);
                            break;
                        }
                    }
                    fs.Seek((retval - length), SeekOrigin.Begin);
                    byte[] info = br.ReadBytes(length);
                    for (int bc = info.Length - 1; bc >= 0; bc--)
                    {
                        val += Convert.ToChar(info.GetValue(bc));
                    }
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();

                }
            }

            return retval;
        }

        static public long GetStartVectorAddress(string filename)
        {
            // startvector = second 4 byte word in the file
            byte[] vector_data = readdatafromfile(filename, 4, 4);
            
            long address = Convert.ToInt64(vector_data[0]) * 256 * 256 * 256;
            address += Convert.ToInt64(vector_data[1]) * 256 * 256;
            address += Convert.ToInt64(vector_data[2]) * 256;
            address += Convert.ToInt64(vector_data[3]);
            return address;
        }

        static public long[] GetVectorAddresses(string filename)
        {
            byte[] vector_data = readdatafromfile(filename, 0, 480);
            long[] vector_addresses = new long[120];
            Int32 offset = 0;

            for (int i = 0; i < 120; i++)
            {
                offset = i * 4;
                long address = Convert.ToInt64(vector_data[offset]) * 256 * 256 * 256;
                address += Convert.ToInt64(vector_data[offset + 1]) * 256 * 256;
                address += Convert.ToInt64(vector_data[offset + 2]) * 256;
                address += Convert.ToInt64(vector_data[offset + 3]);
                vector_addresses.SetValue(address, i);
            }
            return vector_addresses;
        }

        static public string[] GetVectorNames()
        {
            string[] vector_names = new string[120];
            for (int i = 0; i < 120; i++)
            {
                if (i <= 63)
                {
                    vector_names[i] = ((Trionic8File.VectorType)i).ToString();
                }
                else
                {
                    int number = i - 64;
                    vector_names[i] = "User defined vector " + number;
                }
            }
            return vector_names;
        }

        static public byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            try
            {
                FileStream fsi1 = new FileStream(filename, FileMode.Open, FileAccess.Read);
                while (address > fsi1.Length) address -= (int)fsi1.Length;
                BinaryReader br1 = new BinaryReader(fsi1);
                fsi1.Position = address;
                string temp = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    retval.SetValue(br1.ReadByte(), i);
                }
                fsi1.Flush();
                br1.Close();
                fsi1.Close();
                fsi1.Dispose();
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return retval;
        }

        static public bool ValidateTrionic8File(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }
            FileInfo fi = new FileInfo(filename);
            if (fi.Length != TrionicCANLib.Firmware.FileT8.Length)
            {
                MessageBox.Show("File has incorrect length: " + Path.GetFileName(filename));
                return false;
            }

            byte[] testdata = readdatafromfile(filename, 0, 0x10);
            if (testdata[0] == 0x00 && (testdata[1] == 0x10 || testdata[1] == 0x00) && testdata[2] == 0x0C && testdata[3] == 0x00)
            {
                return true;
            }
            MessageBox.Show("File does not seem to be a Trionic 8 file: " + Path.GetFileName(filename));

            return false;
        }

        static private int GetStartOfAddressTableOffset(string filename)
        {
            byte[] searchsequence = new byte[9];
            searchsequence.SetValue((byte)0x00, 0);
            searchsequence.SetValue((byte)0x00, 1);
            searchsequence.SetValue((byte)0x00, 2);
            searchsequence.SetValue((byte)0x00, 3);
            searchsequence.SetValue((byte)0x00, 4);
            searchsequence.SetValue((byte)0x00, 5);
            searchsequence.SetValue((byte)0x00, 6);
            searchsequence.SetValue((byte)0x00, 7);
            searchsequence.SetValue((byte)0x20, 8);
            int symboltableoffset = 0x30000;
            int AddressTableOffset = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {

                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < 0x80000) && (AddressTableOffset == 0))
                {
                    byte adrb = br.ReadByte();
                    switch (adr_state)
                    {
                        case 0:
                            if (adrb == (byte)searchsequence.GetValue(0))
                            {

                                adr_state++;
                            }
                            break;
                        case 1:
                            if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                            }
                            break;
                        case 8:
                            if (adrb == (byte)searchsequence.GetValue(8))
                            {
                                // found it
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                            }
                            break;
                    }

                }
            }
            fsread.Close();
            return AddressTableOffset;
        }

        static private int GetAddressFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        retval = Convert.ToInt32(br.ReadByte()) * 256 * 256 * 256;
                        retval += Convert.ToInt32(br.ReadByte()) * 256 * 256;
                        retval += Convert.ToInt32(br.ReadByte()) * 256;
                        retval += Convert.ToInt32(br.ReadByte());
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to retrieve address from: " + offset.ToString("X6") + ": " + E.Message);
                    }
                    fs.Close();
                }
            }
            return retval;
        }


        static private int GetLengthFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        retval = Convert.ToInt32(br.ReadByte()) * 256;
                        retval += Convert.ToInt32(br.ReadByte());
                    }
                    catch (Exception E)
                    {
                        logger.Debug("Failed to retrieve length from: " + offset.ToString("X6") + ": " + E.Message);
                    }
                    fs.Close();
                }
            }
            return retval;
        }

        static private int GetAddrTableOffset(string filename)
        {
            int addrtaboffset = GetStartOfAddressTableOffset(filename);
            logger.Debug("addrtaboffset: " + addrtaboffset.ToString("X8"));
            int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
            logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4, filename);
            int retval = NqNqNqOffset + 21;
            logger.Debug("symbtablength: " + symbtablength.ToString("X8"));
            return retval;
        }

        static private int GetAddrTableOffsetBySymbolTable(string filename)
        {
            int addrtaboffset = GetEndOfSymbolTable(filename);
            logger.Debug("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
            int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
            logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4, filename);
            int retval = NqNqNqOffset + 21;
            logger.Debug("symbtablength: " + symbtablength.ToString("X8"));
            return retval;
        }

        static private int GetLastNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            int Nq1 = 0;
            int Nq2 = 0;
            int Nq3 = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    Nq1 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    Nq2 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    Nq3 = (int)fs.Position;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        logger.Debug("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            if (Nq3 == 0) retval = Nq2;
            if (retval == 0) retval = Nq1;
            return retval;
        }

        static private int GetNqNqNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        logger.Debug("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return retval;
        }

        static private bool extractSymbolTable(string filename, out int symboltableoffset, out string[] allSymbolNames)
        {
            allSymbolNames = null;
            symboltableoffset = 0;

            int val = 0;
            bool retval = false;
            
            try
            {
                int addrtaboffset = GetEndOfSymbolTable(filename);
                logger.Debug("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
                int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
                logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

                int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
                logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
                int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4, filename);
                logger.Debug("symbtablength: " + symbtablength.ToString("X8"));
                if (symbtablength < 0x1000)
                {
                    logger.Error("No symboltable found!");
                    return false;
                }

                if (symbtaboffset > 0 && symbtaboffset < 0xF0000)
                {
                    val = symbtaboffset;
                }
                symboltableoffset = val;
                FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (BinaryReader br = new BinaryReader(fsread))
                {
                    fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                    if (br.ReadByte() == 0xF1 && br.ReadByte() == 0x1A && br.ReadByte() == 0x06 && br.ReadByte() == 0x5B &&
                        br.ReadByte() == 0xA2 && br.ReadByte() == 0x6B && br.ReadByte() == 0xCC && br.ReadByte() == 0x6F)
                    {
                        BlowFish b = new BlowFish(SymbolnamesDictionary.GetHeader()) { NonStandard = true };
                        byte[] header = br.ReadBytes(16);
                        byte[] headerDecrypted = b.Decrypt_ECB(header);
                        string id = System.Text.Encoding.UTF8.GetString(headerDecrypted).Substring(0, 9).TrimEnd('\0');
                        logger.Debug("Header id: " + id);
                            
                        fsread.Seek(symboltableoffset+24, SeekOrigin.Begin);
                        byte[] unencrypted = br.ReadBytes(symbtablength);
                        byte[] value;
                        if (SymbolnamesDictionary.TryGetValue(id, out value))
                        {
                            BlowFish b0 = new BlowFish(value) { NonStandard = true };
                            byte[] decrypted = b0.Decrypt_ECB(unencrypted);

                            Stream stream = new MemoryStream(decrypted);
                            byte[] bytes = UnpackSymbolnames(stream);

                            string complete = System.Text.Encoding.UTF8.GetString(bytes);
                            allSymbolNames = complete.Split(new[] { "\r\n" }, StringSplitOptions.None);
                            retval = true;
                        }
                        else
                        {
                            logger.Error("Failed to find id: " + id);
                        }
                    }
                    else
                    {
                        fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                        Int64 UnpackedLength = Convert.ToInt64(br.ReadByte());
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256;
                        logger.Debug("UnpackedLength: " + UnpackedLength.ToString("X8"));
                        if (UnpackedLength <= 0x00FFFFFF)
                        {
                            // fill the byte array with the compressed symbol table
                            fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                            byte[] bytes = br.ReadBytes(symbtablength);

                            logger.Debug("Decoding packed symbol table");
                            TrionicSymbolDecompressor.ExpandComprStream(bytes, out allSymbolNames);
                            retval = true;
                        }
                    }
                }
                fsread.Close();
                fsread.Dispose();
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }

            return retval;
        }

        private static byte[] UnpackSymbolnames(Stream stream)
        {
            byte[] bytes = null;
            using (ZipInputStream s = new ZipInputStream(stream))
            {
                s.Password = "yii4uXXwser8";
                ZipEntry theEntry;
                int size = 0;
                byte[] buffer = new byte[2048];
                bytes = new byte[0];

                theEntry = s.GetNextEntry();
                if (theEntry != null)
                {
                    if (theEntry.IsFile)
                    {
                        while (true)
                        {
                            size = s.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                byte[] bigger = new byte[bytes.Length + size];
                                System.Buffer.BlockCopy(bytes, 0, bigger, 0, bytes.Length);
                                System.Buffer.BlockCopy(buffer, 0, bigger, bytes.Length, size);
                                bytes = bigger;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return bytes;
        }

        static private int GetEndOfSymbolTable(string filename)
        {
            byte[] searchsequence = new byte[11];
            searchsequence.SetValue((byte)0x73, 0);
            searchsequence.SetValue((byte)0x59, 1);
            searchsequence.SetValue((byte)0x4D, 2);
            searchsequence.SetValue((byte)0x42, 3);
            searchsequence.SetValue((byte)0x4F, 4);
            searchsequence.SetValue((byte)0x4C, 5);
            searchsequence.SetValue((byte)0x74, 6);
            searchsequence.SetValue((byte)0x41, 7);
            searchsequence.SetValue((byte)0x42, 8);
            searchsequence.SetValue((byte)0x4C, 9);
            searchsequence.SetValue((byte)0x45, 10);
            int symboltableoffset = 0;
            int AddressTableOffset = 0;
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < 0x100000) && (AddressTableOffset == 0))
                {
                    byte adrb = br.ReadByte();
                    switch (adr_state)
                    {
                        case 0:
                            if (adrb == (byte)searchsequence.GetValue(0))
                            {

                                adr_state++;
                            }
                            break;
                        case 1:
                            if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                            }
                            break;
                        case 8:
                            if (adrb == (byte)searchsequence.GetValue(8)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                            }
                            break;
                        case 9:
                            if (adrb == (byte)searchsequence.GetValue(9)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 9;
                            }
                            break;
                        case 10:
                            if (adrb == (byte)searchsequence.GetValue(10))
                            {
                                // found it
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 10;
                            }
                            break;
                    }

                }
            }
            fsread.Close();
            return AddressTableOffset;
        }

        static private int GetFirstNqStringFromOffset(int offset, string filename)
        {
            int retval = 0;
            int Nq1 = 0;
            int Nq2 = 0;
            int Nq3 = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        bool found = false;
                        int state = 0;
                        while (!found && fs.Position < (offset + 0x100))
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() == 0x4E) state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    Nq1 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 3:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 4:
                                    Nq2 = (int)fs.Position;
                                    if (br.ReadByte() == 0x4E) state++;
                                    else state = 0;
                                    break;
                                case 5:
                                    if (br.ReadByte() == 0x71) state++;
                                    else state = 0;
                                    break;
                                case 6:
                                    found = true;
                                    Nq3 = (int)fs.Position;
                                    retval = (int)fs.Position;
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        logger.Debug("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            if (Nq3 == 0) retval = Nq2;
            if (retval == 0) retval = Nq1;
            return retval;
        }

        static public bool TryToExtractPackedBinary(string filename, out SymbolCollection symbolCollection)
        {
            symbolCollection = null;
            string[] allSymbolNames;
            int symboltableoffset;

            int realAddressTableOffset = GetAddrTableOffsetBySymbolTable(filename) + 7;
            logger.Debug("Real address table offset: " + realAddressTableOffset.ToString("X8"));

            if (!extractSymbolTable(filename, out symboltableoffset, out allSymbolNames))
            {
                logger.Debug("Could not extract symboltable!");
            }
            
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                logger.Debug("Find address table offset");
                if(!FindAddressTableOffset(symboltableoffset, fsread, br))
                {
                    MessageBox.Show("Could not find address table offset!");
                    return false;
                }

                logger.Debug("Read address table");
                if (!ReadAddressTable(out symbolCollection, realAddressTableOffset, fsread, br))
                {
                    return false;
                }
            }

            AddNamesToSymbols(symbolCollection, allSymbolNames);

            CastProgressEvent("Idle", 0);
            return true;
        }

        private static bool FindAddressTableOffset(int symboltableoffset, FileStream fsread, BinaryReader br)
        {
            int addressTableOffset = 0;
            byte[] searchsequence = new byte[9];
            searchsequence.SetValue((byte)0x00, 0);
            searchsequence.SetValue((byte)0x00, 1);
            searchsequence.SetValue((byte)0x00, 2);
            searchsequence.SetValue((byte)0x00, 3);
            searchsequence.SetValue((byte)0x00, 4);
            searchsequence.SetValue((byte)0x00, 5);
            searchsequence.SetValue((byte)0x00, 6);
            searchsequence.SetValue((byte)0x00, 7);
            searchsequence.SetValue((byte)0x20, 8);

            fsread.Seek(symboltableoffset, SeekOrigin.Begin);
            int adr_state = 0;
            while ((fsread.Position < TrionicCANLib.Firmware.FileT8.Length) && (addressTableOffset == 0))
            {
                byte adrb = br.ReadByte();
                switch (adr_state)
                {
                    case 0:
                        if (adrb == (byte)searchsequence.GetValue(0))
                        {

                            adr_state++;
                        }
                        break;
                    case 1:
                        if (adrb == (byte)searchsequence.GetValue(1)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 1;
                        }
                        break;
                    case 2:
                        if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 2;
                        }
                        break;
                    case 3:
                        if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 3;
                        }
                        break;
                    case 4:
                        if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 4;
                        }
                        break;
                    case 5:
                        if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 5;
                        }
                        break;
                    case 6:
                        if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 6;
                        }
                        break;
                    case 7:
                        if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 7;
                        }
                        break;
                    case 8:
                        if (adrb == (byte)searchsequence.GetValue(8))
                        {
                            // found it
                            addressTableOffset = (int)fsread.Position - 1;

                            if (addressTableOffset > 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            adr_state = 0;
                            fsread.Position -= 8;
                        }
                        break;
                }
            }
            return false;
        }

        private static bool ReadAddressTable(out SymbolCollection symbolCollection, int realAddressTableOffset, FileStream fsread, BinaryReader br)
        {
            bool retval = true;
            bool endoftable = false;
            Int64 internal_address = 0;
            int symbollength = 0;
            int bitmask = 0;
            int symb_count = 0;

            fsread.Seek(realAddressTableOffset - 17, SeekOrigin.Begin);

            symbolCollection = new SymbolCollection();
            while (!endoftable)
            {
                try
                {
                    byte[] bytes = br.ReadBytes(10);
                    if (bytes.Length == 10)
                    {
                        if ((Convert.ToInt32(bytes.GetValue(9)) != 0x00))
                        {
                            endoftable = true;
                            logger.Debug("EOT: " + fsread.Position.ToString("X6"));
                        }
                        else
                        {
                            internal_address = Convert.ToInt64(bytes.GetValue(0)) * 256 * 256;
                            internal_address += Convert.ToInt64(bytes.GetValue(1)) * 256;
                            internal_address += Convert.ToInt64(bytes.GetValue(2));

                            symbollength = Convert.ToInt32(bytes.GetValue(3)) * 256;
                            symbollength += Convert.ToInt32(bytes.GetValue(4));

                            bitmask = Convert.ToInt32(bytes.GetValue(5)) * 256;
                            bitmask += Convert.ToInt32(bytes.GetValue(6));

                            SymbolHelper sh = new SymbolHelper();
                            sh.Symbol_type = Convert.ToInt32(bytes.GetValue(7));
                            sh.Varname = "Symbolnumber " + symbolCollection.Count.ToString();
                            sh.Symbol_number = symbolCollection.Count;
                            sh.Symbol_number_ECU = symbolCollection.Count;
                            sh.Flash_start_address = internal_address;
                            sh.Start_address = internal_address;
                            sh.Length = symbollength;
                            sh.BitMask = bitmask;

                            symbolCollection.Add(sh);
                            symb_count++;
                            if (symb_count % 500 == 0)
                            {
                                CastProgressEvent("Symbol: " + sh.Varname, 5);
                            }
                        }
                    }
                    else
                    {
                        endoftable = true;
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E);
                    retval = false;
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolCollection"></param>
        /// <param name="allSymbolNames"></param>
        private static void AddNamesToSymbols(SymbolCollection symbolCollection, string[] allSymbolNames)
        {
            try
            {
                if (allSymbolNames != null)
                {
                    for (int i = 0; i < allSymbolNames.Length - 1; i++)
                    {
                        if (i % 500 == 0)
                        {
                            CastProgressEvent("Adding symbol names: ", (int)(((float)i / (float)allSymbolNames.Length) * 100));
                        }
                        SymbolHelper sh = symbolCollection[(i)];
                        sh.Varname = allSymbolNames[i + 1].Trim(); // Skip first in array since its "SymbolNames"
                        logger.Debug(String.Format("Set symbolnumber: {0} to be {1}", sh.Symbol_number, sh.Varname));
                        sh.Description = SymbolTranslator.ToDescription(sh.Varname);
                        sh.createAndUpdateCategory(sh.Varname);
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E, "Failed to add symbolnames");
            }
        }

        static private void ImportSymbols(System.Data.DataTable dt, SymbolCollection collection)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CastProgressEvent("Importing symbols", 90);

            try
            {
                foreach (SymbolHelper sh in collection)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                        {
                            if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                            {
                                // Swap varname and userdescription
                                if (sh.Varname.StartsWith("Symbolnumber "))
                                {
                                    sh.Userdescription = sh.Varname;
                                    sh.Varname = dr["DESCRIPTION"].ToString();
                                }
                                else
                                {
                                    sh.Userdescription = dr["DESCRIPTION"].ToString();
                                }

                                sh.Description = SymbolTranslator.ToDescription(sh.Varname);
                                sh.createAndUpdateCategory(sh.Varname);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E);
            }
            sw.Stop();
            logger.Debug("Stopped " + sw.ElapsedMilliseconds);
            CastProgressEvent("Completed", 0);
        }

        static public bool TryToLoadAdditionalXMLSymbols(string filename, SymbolCollection collection)
        {
            if (File.Exists(filename))
            {
                string binname = SymbolXMLFile.GetFileDescriptionFromFile(filename);
                if (binname != string.Empty)
                {
                    System.Data.DataTable dt = new System.Data.DataTable(binname);
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    dt.ReadXml(filename);
                    ImportSymbols(dt, collection);
                    return true;
                }
            }
            return false;
        }

        static public bool TryToLoadAdditionalBinSymbols(string filename, SymbolCollection collection)
        {
            // Look for a complete xml file first
            T8Header fh = new T8Header();
            fh.init(filename);
            string[] symbolFiles = Directory.GetFiles(System.Windows.Forms.Application.StartupPath, "*.xml");
            foreach (string symbolFile in symbolFiles)
            {
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(symbolFile);
                if (fh.SoftwareVersion.Trim().StartsWith(filenameWithoutExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return TryToLoadAdditionalXMLSymbols(symbolFile, collection);
                }
            }

            // Secondly load the .xml file with same path and filename as the .bin file. 
            string xmlfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".xml");
            return TryToLoadAdditionalXMLSymbols(xmlfile, collection);
        }

        public static bool IsSoftwareOpen(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Flash_start_address > FileT8.Length && sh.Length > 0x100 && sh.Length < 0x400)
                {
                    if (sh.SmartVarname == "BFuelCal.LambdaOneFacMap" || sh.SmartVarname == "KnkFuelCal.fi_MaxOffsetMap" || sh.SmartVarname == "AirCtrlCal.RegMap")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
