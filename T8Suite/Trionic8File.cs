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

        private static int CountNq(string filename, int offset)
        {
            int cnt = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        int state = 0;
                        while (fs.Position > (offset - 8) && fs.Position > 0 && cnt < 3)
                        {
                            switch (state)
                            {
                                case 0:
                                    if (br.ReadByte() != 0x4E) return cnt;
                                    state++;
                                    break;
                                case 1:
                                    if (br.ReadByte() != 0x71) return cnt;
                                    state = 0;
                                    cnt++;
                                    fs.Position -= 4;
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        logger.Debug("Failed to retrieve Nq count from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return cnt;
        }

        static private int GetAddrTableOffsetBySymbolTable(string filename)
        {
            int addrtaboffset = GetEndOfSymbolTable(filename);
            logger.Debug("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
            int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
            logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));
            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));

            int nqCount = CountNq(filename, NqNqNqOffset - 2);
            logger.Debug("Nq count: " + nqCount.ToString("X8"));
            m_addressoffset = GetAddressFromOffset(NqNqNqOffset - ((nqCount * 2) + 6), filename);
            logger.Debug("AddressOffset: " + m_addressoffset.ToString("X8"));

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

        private static bool DetermineOpen_FromSymbolNames(SymbolCollection symbols)
        {
            foreach (SymbolHelper sh in symbols)
            {
                if (sh.Internal_address >= FileT8.Length &&
                    sh.Length > 0x100 && sh.Length <= 0x400)
                {
                    if (sh.SmartVarname == "BFuelCal.LambdaOneFacMap" ||
                        sh.SmartVarname == "KnkFuelCal.fi_MaxOffsetMap" ||
                        sh.SmartVarname == "AirCtrlCal.RegMap")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool DetermineOpen_FromSymbolAddress(SymbolCollection symbols)
        {
            try
            {
                if (symbols != null)
                {
                    for (int i = 0; i < symbols.Count; i++)
                    {
                        SymbolHelper sh = symbols[(i)];
                        if (sh.Internal_address >= (0x100000 + 32768))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E, "Failed to detect binary type");
            }
            return false;
        }

        private static bool DetermineOpen_FromData(byte[] data)
        {
            byte[] addrPat = { 0x20,0x3C,0x00, 0x14, 0x00, 0x00 };
            byte[] addrMsk = { 0xf1,0xbf,0xff, 0xff, 0xff, 0x00 };
            uint pos = 0x20000;

            while ((pos + addrMsk.Length) <= data.Length)
            {
                if (MatchPattern(data, pos, addrPat, addrMsk) == true)
                {
                    return true;
                }

                pos += 2;
            }
            return false;
        }

        static private void DetermineBinaryOpenness(SymbolCollection symbols, byte[] data)
        {
            const int MinRequiredLevel = 2;
            int level = 0;
            

            // Determine open/closed by looking at symbol names
            if (DetermineOpen_FromSymbolNames(symbols) == true)
            {
                level++;
            }

            // Determine open/closed by looking at symbol address
            if (DetermineOpen_FromSymbolAddress(symbols) == true)
            {
                level++;
            }

            // This one has extra weight since the address should be present in a LOT of places
            if (DetermineOpen_FromData(data) == false)
            {
                level--;
            }
            else
            {
                level++;
            }

            logger.Debug("Binary openness level: " + level.ToString());
            m_openbin = (level >= MinRequiredLevel);
        }

        private static bool m_openbin = false;
        public static bool IsSoftwareOpen
        {   get { return m_openbin; } }

        // This symbol has different meaning and usage depending on if the binary is open or closed!!
        // As such, it's better to translate everything in one place and only work on translated symbols
        private static int m_addressoffset = 0;

        /// <summary>
        /// Translate a symbol's internal address to corresponding Flash and SRAM address
        /// </summary>
        /// <param name="symbols">Symbol list</param>
        /// <param name="pOffset">Primary offset</param>
        /// <param name="sOffset">Secondary offset</param>
        static private void TranslateAddressOffsets(SymbolCollection symbols, int PriOffset, int SecOffset)
        {
            logger.Debug("Offset: " + PriOffset.ToString("X6"));
            try
            {
                if (symbols != null)
                {
                    for (int i = 0; i < symbols.Count; i++)
                    {
                        SymbolHelper sh = symbols[(i)];

                        // Make this GO AWAY!
                        // Only symbols with VALID flash data should have an address in flash
                        sh.Flash_start_address = sh.Internal_address;

                        /*
                        // Flash only symbol
                        if ((sh.Internal_address + sh.Length) <= 0x100000)
                        {
                            sh.Flash_start_address = sh.Internal_address;
                        }
                        // Sram and maybe flash
                        else */if (sh.Internal_address >= 0x100000)
                        {
                            // SRAM address
                            sh.Start_address = sh.Internal_address;

                            // NVDM symbols are tagged with 0xff.
                            // Adaption symbols are sometimes tagged as being cal (which is definitely not true) so these must be masked off
                            if (sh.Symbol_type != 0xff && (sh.Symbol_type & 0x22) == 0x02)
                            {
                                int actAddress = 0;

                                // Normal binary, use the primary offset
                                if (m_openbin == false)
                                {
                                    if ((sh.Internal_address + sh.Length) <= (0x100000 + 32768) &&
                                        sh.Internal_address >= PriOffset)
                                    {
                                        actAddress = sh.Internal_address - PriOffset;
                                    }
                                }
                                // Open binary, offsets are switched up
                                else
                                {
                                    // Internal SRAM, use the secondary offset
                                    if ((sh.Internal_address + sh.Length) <= (0x100000 + 32768) &&
                                        sh.Internal_address >= SecOffset)
                                    {
                                        actAddress = sh.Internal_address - SecOffset;
                                    }

                                    // External SRAM, use the primary offset
                                    else if (sh.Internal_address >= (0x100000 + 32768) &&
                                             sh.Internal_address >= PriOffset)
                                    {
                                        actAddress = sh.Internal_address - PriOffset;
                                    }
                                }

                                // Real address must be within range
                                if ((actAddress + sh.Length) <= 0x100000 && actAddress > 0)
                                {
                                    sh.Flash_start_address = actAddress;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E, "Failed to translate addresses");
            }
        }

        // This method is _NOT_ range safe. Use caution
        private static uint readU32(byte[] data, uint pos)
        {
            uint value = (uint)data[pos] * 256 * 256 * 256;
            value += (uint)data[pos + 1] * 256 * 256;
            value += (uint)data[pos + 2] * 256;
            value += (uint)data[pos + 3];
            return value;
        }

        private static bool MatchPattern(byte[] data, uint pos, byte[] pat, byte[] msk)
        {
            bool found = false;

            if ((pos + msk.Length) <= data.Length)
            {
                found = true;
                for (int i = 0; i < msk.Length; i++)
                {
                    if ((data[pos + i] & msk[i]) != (pat[i] & msk[i]))
                    {
                        return false;
                    }
                }
            }

            return found;
        }

        private static bool ReadAddressPair(byte[] data, uint pos, out uint addr1, out uint addr2)
        {
            byte[] addrPat = { 0x20,0x3C,0x00 };
            byte[] addrMsk = { 0xf1,0xbf,0xff };

            addr1 = 0;
            addr2 = 0;

            if (MatchPattern(data, pos, addrPat, addrMsk) == true &&
                MatchPattern(data, pos + 6, addrPat, addrMsk) == true &&
                (pos + 12) <= data.Length)
            {
                addr1 = readU32(data, pos + 2);
                addr2 = readU32(data, pos + 8);
                return true;
            }

            return false;
        }

        private static int DecodeDataCpy(byte[] data, uint pos)
        {
            uint addr1, addr2;

            // Skip link and register backup
            pos += 8;

            if (ReadAddressPair(data, pos, out addr1, out addr2) == true)
            {
                // Most open bins:
                if (addr1 >= 0x100000 && addr1 < 0x108000 && // Must be within regular sram
                    addr2 < 0x100000) // Must be within flash
                {
                    return (int)(addr1 - addr2);
                }

                // Very early open bins are organised in another way
                // Fix...
                // Or not? Further disassembling seem to indicate they're not even copying symbols to normal sram
            }

            return 0;
        }

        private static int DetermineSecondaryOffset(byte[] data)
        {
            uint initFunc = readU32(data, 0x20004);

            // Read direct address
            if (initFunc >= 0x20008 &&
                initFunc <= (0x100000 - 6) &&
                (initFunc & 1) == 0)
            {
                // in "INIT"
                // Expect jsr xxxx
                if (data[initFunc] == 0x4e &&
                    data[initFunc + 1] == 0xb9 &&
                    data[initFunc + 2] == 0x00)
                {
                    // Make sure that jsr is sane
                    uint nextJump = readU32(data, initFunc + 2);
                    if (nextJump >= 0x20008 &&
                        nextJump <= (0x100000 - 6) &&
                        (nextJump & 1) == 0)
                    {
                        // in "_init"
                        // Expect jsr xxxx
                        if (data[nextJump] == 0x4e &&
                            data[nextJump + 1] == 0xb9 &&
                            data[nextJump + 2] == 0x00)
                        {
                            // Make sure that jsr is sane
                            nextJump = readU32(data, nextJump + 2);

                            if (nextJump >= 0x20008 &&
                                (nextJump & 1) == 0)
                            {
                                // In data init??
                                // (This method is range safe)
                                return DecodeDataCpy(data, nextJump);
                            }
                        }
                    }
                }
            }
            return 0;
        }

        private static bool PrimaryPidTableIsHealthy(byte[] data, int pos, int count)
        {
            byte[] pidPat =
            {
                0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00
            };
            byte[] PidMsk =
            {
                0x00, 0x00,
                0xff, 0xff, 0x00, 0x00,
                0x00, 0xff
            };

            for (int i = 0; i < count; i++)
            {
                if (MatchPattern(data, (uint)pos, pidPat, PidMsk) == false)
                {
                    logger.Debug("PID at " + pos.ToString("X6") + " is not healthy");
                    return false;
                }
                pos += 8;
            }
            return true;
        }

        private static void LoadPrimaryPidTable(byte[] data, int pos, int count, out PidCollection pidCollection)
        {
            pidCollection = new PidCollection();
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                PidHelper ph = new PidHelper();
                ph.FileAddress = pos;
                ph.Index = index++;
                ph.PID = (data[pos] << 8 | data[pos + 1]).ToString("X4");
                ph.SymbolIndex = (data[pos + 4] << 8 | data[pos + 5]);
                ph.PackedFlags = data[pos + 6];
                pidCollection.Add(ph);
                pos += 8;
            }
        }

        private static void LocatePrimaryPidTable_Fallback(byte[] data, out PidCollection pidCollection)
        {
            pidCollection = null;
            int pos = 0x20144;

            byte[] pidPat =
            {
                0x20, 0x6F, 0x00, 0x10,             // 28 6F 00 10          movea.l $10(sp),a4
                0x4E, 0xB9, 0x00, 0x00, 0x00, 0x00, // 4E B9 00 0B 79 74    jsr     sub_B7974
                0x20, 0x7C, 0x00, 0x00, 0x00, 0x00, // 2A 7C 00 03 33 74    movea.l #$33374,a5
                0x48, 0x78, 0x00, 0x00,             // 48 78 06 4B          pea     ($64B).w
                0x48, 0x78, 0x00, 0x00,             // 48 78 02 6C          pea     (620).w
                0x42, 0xa7,                         // 42 A7                clr.l   -(sp)
                0x2F, 0x08,                         // 2F 0D                move.l  a5,-(sp)
                0x4E, 0xB9, 0x00, 0x00, 0x00, 0x00, // 4E B9 00 05 76 58    jsr     sub_57658
                0x20, 0x00,                         // 22 00                move.l  d0,d1
                0x4F, 0xEF, 0x00, 0x10,             // 4F EF 00 10          lea     $10(sp),sp
                0x70, 0xFF,                         // 70 FF                moveq   #$FFFFFFFF,d0
                0xB0, 0x80,                         // B0 81                cmp.l   d1,d0
                0x67, 0x00                          // 67 14                beq.s   loc_4F3B0
            };
            byte[] PidMsk =
            {
                0xf1, 0xff, 0xff, 0xff,             // 28 6F 00 10          movea.l $10(sp),a4
                0xff, 0xff, 0xff, 0xf0, 0x00, 0x01, // 4E B9 00 05 76 58    jsr     000xxxxx // Must be even address
                0xf1, 0xff, 0xff, 0xf0, 0x00, 0x01, // 2A 7C 00 03 33 74    movea.l #$00 0x xx xx,a5 // Must be even address
                0xff, 0xff, 0x00, 0x00,             // 48 78 06 4B          pea     (xxxx).w
                0xff, 0xff, 0x00, 0x00,             // 48 78 02 6C          pea     (xxxx).w
                0xff, 0xff,                         // 42 A7                clr.l   -(sp)
                0xff, 0xf8,                         // 2F 0D                move.l  a5,-(sp)
                0xff, 0xff, 0xff, 0xf0, 0x00, 0x01, // 4E B9 00 05 76 58    jsr     000xxxxx // Must be even address
                0xf1, 0xf8,                         // 22 00                move.l  d*,d*
                0xff, 0xff, 0xff, 0xff,             // 4F EF 00 10          lea     $10(sp),sp
                0xf1, 0xFF,                         // 70 FF                moveq   #$FFFFFFFF,d*
                0xf1, 0xf8,                         // B0 81                cmp.l   d*,d*
                0xff, 0x00                          // 67 14                beq.x   xx
            };

            logger.Debug("Attempting search for primary pid table; Secondary method");

            while (pos < data.Length)
            {
                if (MatchPattern(data, (uint)pos, pidPat, PidMsk))
                {
                    int count = (data[pos + 22] << 8 | data[pos + 23]);
                    int table = (int)readU32(data, (uint)pos + 12);
                    int fromReg = (data[pos + 10] >> 1) & 7;
                    int toReg = data[pos + 27] & 7;

                    logger.Debug("Secondary match at " + pos.ToString("X6"));
                    // logger.Debug("From reg: " + fromReg.ToString());
                    // logger.Debug("To reg: " + toReg.ToString());

                    if (table >= 0x20144 && (table + (count * 8) <= 0x100000) &&
                        count > 0 && count < 10000 && fromReg == toReg &&
                        PrimaryPidTableIsHealthy(data, table, count))
                    {
                        LoadPrimaryPidTable(data, table, count, out pidCollection);
                        return;
                    }
                    else
                    {
                        logger.Debug("Pid table missmatch at " + pos.ToString("X6"));
                        pos += pidPat.Length;
                    }
                }
                else
                {
                    pos += 2;
                }
            }

            logger.Debug("PID search failed");
        }

        private static void LocatePrimaryPidTable(byte[] data, out PidCollection pidCollection)
        {
            pidCollection = null;
            int pos = 0x20144;

            byte[] pidPat =
            {
                0x4a, 0x00,                         // 4a 07                tst.b   d7
                0x67, 0x00, 0x00, 0x00,             // 67 00 00 BC          beq.w   loc_4F434
                0x48, 0x78, 0x00, 0x00,             // 48 78 06 4B          pea     ($64B).w
                0x48, 0x78, 0x00, 0x00,             // 48 78 02 6C          pea     (620).w
                0x42, 0xa7,                         // 42 A7                clr.l   -(sp)
                0x2F, 0x3C, 0x00, 0x00, 0x00, 0x00, // 2F 3C 00 09 50 74    move.l  #$95074,-(sp)
                0x4E, 0xB9, 0x00, 0x00, 0x00, 0x00, // 4E B9 00 05 76 58    jsr     sub_57658
                0x20, 0x00,                         // 22 00                move.l  d0,d1
                0x4F, 0xEF, 0x00, 0x10,             // 4F EF 00 10          lea     $10(sp),sp
                0x70, 0xFF,                         // 70 FF                moveq   #$FFFFFFFF,d0
                0xB0, 0x80,                         // B0 81                cmp.l   d1,d0
                0x67, 0x00                          // 67 14                beq.s   loc_4F3B0
            };
            byte[] PidMsk =
            {       
                0xff, 0xf8,                         // 4a 07                tst.b   d*
                0xff, 0xff, 0x00, 0x00,             // 67 00 00 BC          beq.w   xxxx
                0xff, 0xff, 0x00, 0x00,             // 48 78 06 4B          pea     (xxxx).w
                0xff, 0xff, 0x00, 0x00,             // 48 78 02 6C          pea     (xxxx).w
                0xff, 0xff,                         // 42 A7                clr.l   -(sp)
                0xff, 0xff, 0xff, 0xf0, 0x00, 0x01, // 2F 3C 00 09 50 74    move.l  #xxxxxxxx,-(sp)
                0xff, 0xff, 0xff, 0xf0, 0x00, 0x01, // 4E B9 00 05 76 58    jsr     xxxxxxxx
                0xf1, 0xf8,                         // 22 00                move.l  d*,d*
                0xff, 0xff, 0xff, 0xff,             // 4F EF 00 10          lea     $10(sp),sp
                0xf1, 0xFF,                         // 70 FF                moveq   #$FFFFFFFF,d*
                0xf1, 0xf8,                         // B0 81                cmp.l   d*,d*
                0xff, 0x00                          // 67 14                beq.x   xx
            };

            logger.Debug("Attempting search for primary pid table; Primary method");

            while (pos < data.Length)
            {
                if (MatchPattern(data, (uint)pos, pidPat, PidMsk))
                {
                    int count = (data[pos + 12] << 8 | data[pos + 13]);
                    int table = (int)readU32(data, (uint)pos + 18);

                    if (table >= 0x20144 && (table + (count * 8) <= 0x100000) &&
                        count > 0 && count < 10000 &&
                        PrimaryPidTableIsHealthy(data, table, count))
                    {
                        LoadPrimaryPidTable(data, table, count, out pidCollection);
                        return;
                    }
                    else
                    {
                        logger.Debug("Pid table missmatch at " + pos.ToString("X6"));
                        pos += pidPat.Length;
                    }
                }
                else
                {
                    pos += 2;
                }
            }

            logger.Debug("Primary method failed");
            LocatePrimaryPidTable_Fallback(data, out pidCollection);
        }

        private static bool OkayTemCharacter(byte ch)
        {
            // It's very likely that TEM won't allow more than A - Z, a - z, numeric values, space and maybe one or two special characters.
            // The limits are currently not known so only a lazy boundary check is performed.
            if ((ch == 0 || ch >= 0x20) && ch < 0x7f)
                return true;
            return false;
        }

        private static void LocateTemTable(byte[] data, out PidCollection temCollection)
        {
            temCollection = null;
            int pos = 0x20144;
            // The ECU may or may not care about what the symbol is so perhaps it's better to perform a pattern scan of the code and retrieve tables that way?
            // 4E (71)  00 00   4F 46 46 00  (0000 "OFF" 0)
            // 4E (75)  00 00   4F 46 46 00
            byte[] TemPat =
            {
                0x4e, 0x71,
                0x00, 0x00, 0x4f, 0x46, 0x46, 0x00 
            };
            byte[] TemMsk =
            {
                0xff, 0xfb, // Allow 71 or 75
                0xff, 0xff, 0xdf, 0xdf, 0xdf, 0xdf // Allow either combo of upper/lower case and 00 or 20 as terminator
            };

            while (pos < data.Length)
            {
                if (MatchPattern(data, (uint)pos, TemPat, TemMsk))
                {
                    pos += 2;
                    int count = 0;
                    int tpos = pos;

                    logger.Debug("Found TEM table at " + pos.ToString("X6"));

                    while ((tpos + 6) <= data.Length)
                    {
                        // Abort due to symbol index. 16383 seem like a reasonable limit.
                        if ((data[tpos] & 0xc0) > 0)
                        {
                            // logger.Debug("Abort due to index " + tpos.ToString("X6"));
                            break;
                        }

                        // Check for usable chars
                        if (OkayTemCharacter(data[tpos + 2]) == false ||
                            OkayTemCharacter(data[tpos + 3]) == false ||
                            OkayTemCharacter(data[tpos + 4]) == false ||
                            OkayTemCharacter(data[tpos + 5]) == false)
                        {
                            // logger.Debug("Abort due to char " + tpos.ToString("X6"));
                            break;
                        }
                        tpos += 6;
                        count++;
                    }

                    logger.Debug("TEM count " + count.ToString());

                    if (count > 4)
                    {
                        temCollection = new PidCollection();
                        int index = 0;
                        for (int i = 0; i < count; i++)
                        {
                            PidHelper ph = new PidHelper();
                            ph.FileAddress = pos;
                            ph.SymbolIndex = (data[pos] << 8 | data[pos + 1]);
                            ph.PID = "";
                            ph.Index = index++;

                            // A range check has already been performed but let's be a little chicken shit about it
                            try
                            {
                                for (int c = 0; c < 4; c++)
                                {
                                    if (data[pos + 2 + c] < 0x20) break;
                                    ph.PID += (char)data[pos + 2 + c];
                                }
                            }
                            catch (Exception E)
                            {
                                logger.Debug("TEM collect parse string exception: " + E);
                            }

                            temCollection.Add(ph);
                            pos += 6;
                        }
                        // Protect the first symbol
                        temCollection[0].IsProtected = true;
                        return;
                    }
                    else
                    {
                        logger.Debug("Count too small");
                        pos += (TemPat.Length - 2);
                    }
                }
                else
                {
                    pos += 2;
                }
            }

            logger.Debug("No Usable TEM table in this binary");
        }

        private static void LoadPidTables(byte[] data, out PidCollection pidCollection, out PidCollection temCollection)
        {
            LocatePrimaryPidTable(data, out pidCollection);
            // -:Implement secondary table:-
            LocateTemTable(data, out temCollection);
        }

        static public bool TryToExtractPackedBinary(string filename, out SymbolCollection symbolCollection, out PidCollection pidCollection, out PidCollection temCollection)
        {
            symbolCollection = null;
            pidCollection = null;
            temCollection = null;
            string[] allSymbolNames;
            int symboltableoffset;
            int SecondaryOffset = 0;

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

            byte[] data = readdatafromfile(filename, 0, 0x100000);

            DetermineBinaryOpenness(symbolCollection, data);

            if (m_openbin == true)
            {
                SecondaryOffset = DetermineSecondaryOffset(data);
                logger.Debug("Open bin secondary offset: " + SecondaryOffset.ToString("X6"));
            }

            TranslateAddressOffsets(symbolCollection, m_addressoffset, SecondaryOffset);

            LoadPidTables(data, out pidCollection, out temCollection);

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
                            sh.Symbol_extendedtype = Convert.ToInt32(bytes.GetValue(8));
                            sh.Varname = "Symbolnumber " + symbolCollection.Count.ToString();
                            // The name table symbol is skipped but we need the true, actual count for some stuff to function correctly (pid, tem, read symbol by index)
                            sh.Symbol_number = symbolCollection.Count + 1;
                            sh.Symbol_number_ECU = symbolCollection.Count + 1;
                            sh.Internal_address = (int)internal_address;
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
    }
}
