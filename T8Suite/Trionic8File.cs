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

namespace T8SuitePro
{
    public class Trionic8File
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            byte[] testdata = readdatafromfile(filename, 0, 0x10);
            if (testdata[0] == 0x00 && (testdata[1] == 0x10 || testdata[1] == 0x00) && testdata[2] == 0x0C && testdata[3] == 0x00)
            {
                return true;
            }
            return false;
        }

        static private int ReadMarkerAddressContent(string filename, int value, int filelength, out int length, out int val)
        {
            int retval = 0;
            length = 0;
            val = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = filelength - 0x90;
                    try
                    {

                        fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                        byte[] inb = br.ReadBytes(0x8F);
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
                            int temp = Convert.ToInt32(info.GetValue(bc));
                            for (int mt = 0; mt < (3 - bc); mt++)
                            {
                                temp *= 256;
                            }
                            val += temp;
                        }
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                        retval = 0;
                    }
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();

                }
            }

            return retval;

        }
        /// <summary>
        /// Check is identifier 0x9B is present in the footer. If this is the case, the file is packed!
        /// </summary>
        /// <param name="m_currentfile"></param>
        /// <returns></returns>
        static private bool IsBinaryPackedVersion(string m_currentfile, int filelength)
        {
            int len = 0;
            string val = "";
            int ival = 0;
            int value = ReadMarkerAddress(m_currentfile, 0x9B, filelength, out len, out val);
            value = ReadMarkerAddressContent(m_currentfile, 0x9B, filelength, out len, out ival);
            if (value > 0 && ival < filelength && ival > 0) return true;
            return false;
        }

        static private int GetSymbolListOffSet(string m_currentfile, int filelength)
        {
            int retval = 0;
            FileStream fsread = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                //byte[] filebytes = br.ReadBytes((int)fsread.Length);

                int state = 0;
                int zerocount = 0;
                //for (int t = 0; t < filebytes.Length; t++)
                //{
                //if (retval != 0) break;
                while ((fsread.Position < filelength) && retval == 0)
                {
                    byte b = br.ReadByte();
                    //byte b = filebytes[t];
                    switch (state)
                    {
                        case 0:
                            if (b == 0x00)
                            {
                                zerocount = 0;
                                state++;
                            }
                            break;
                        case 1:
                            if (b == 0x00)
                            {
                                zerocount++;
                                if (zerocount >= 15) state++;
                            }
                            else
                            {
                                state = 0;
                            }
                            break;
                        case 2:
                            if (b != 0x00)
                            {
                                retval = (int)fsread.Position;
                                //retval = t+1;

                            }
                            break;
                    }
                }
            }
            fsread.Close();
            fsread.Dispose();
            retval -= 2;
            return retval;
        }

        private int m_Filelength = 0x80000; // T7

        public int Filelength
        {
            get { return m_Filelength; }
            set { m_Filelength = value; }
        }

        private string m_fileName = string.Empty;

        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }

        private SymbolCollection symbol_collection;

        public SymbolCollection Symbol_collection
        {
            get { return symbol_collection; }
            set { symbol_collection = value; }
        }

        static private void DumpBytesToConsole(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
        }

        static private string GetFileDescriptionFromFile(string file)
        {
            string retval = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    string name = sr.ReadLine();
                    name = name.Trim();
                    name = name.Replace("<", "");
                    name = name.Replace(">", "");
                    //name = name.Replace("x0020", " ");
                    name = name.Replace("_x0020_", " ");
                    for (int i = 0; i <= 9; i++)
                    {
                        name = name.Replace("_x003" + i.ToString() + "_", i.ToString());
                    }
                    retval = name;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return retval;
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
            int AddressTableOffset = 0;//GetAddressTableOffset(searchsequence);
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {

                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                // byte[] filebytes = br.ReadBytes((int)fsread.Length);
                //for (int t = 0; t < filebytes.Length; t++)
                //{
                //if (AddressTableOffset != 0) break;
                //  byte adrb = filebytes[t];
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
                                //t -= 1;
                            }
                            break;
                        case 2:
                            if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 2;
                                //t -= 2;
                            }
                            break;
                        case 3:
                            if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 3;
                                //t -= 3;
                            }
                            break;
                        case 4:
                            if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 4;
                                // t -= 4;
                            }
                            break;
                        case 5:
                            if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 5;
                                // t -= 5;
                            }
                            break;
                        case 6:
                            if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 6;
                                //t -= 6;
                            }
                            break;
                        case 7:
                            if (adrb == (byte)searchsequence.GetValue(7)) adr_state++;
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 7;
                                //t -= 7;
                            }
                            break;
                        case 8:
                            /*if (fsread.Position > 0x5f900)
                            {
                                logger.Debug("Hola");
                            }
                            */
                            if (adrb == (byte)searchsequence.GetValue(8))
                            {
                                // found it
                                //AddressTableOffset = t;
                                AddressTableOffset = (int)fsread.Position - 1;
                            }
                            else
                            {
                                adr_state = 0;
                                fsread.Position -= 8;
                                //t -= 8;
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

        private bool UnpackFileUsingDecode(string filename, int filelength, out int symboltableoffset)
        {
            symboltableoffset = 0;
            int len = 0;
            int val = 0;
            //int idx = ReadEndMarker(0x9B);
            try
            {
                int idx = ReadMarkerAddressContent(filename, 0x9B, filelength, out len, out val);

               /* if (idx == 0)
                {
                    // try to fetch the packed table another way
                    int temp_addressTableOffset = GetStartOfAddressTableOffset(filename);
                    int temp_packTableStart = temp_addressTableOffset - 18; 

                }*/


                //if (idx > 0)
                {
                    /* if (val > m_currentfile_size)
                     {
                         // try to find the addresstable offset
                         int addrtaboffset = GetStartOfAddressTableOffset(filename);
                         //TODO: Finish for abused packed files!
                     }*/
                    // FAILSAFE for some files that seem to have protection!
                    int addrtaboffset = GetStartOfAddressTableOffset(filename);
                    int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
                    m_sramOffsetForOpenFile = GetAddressFromOffset(addrtaboffset - 0x18, filename);
                    int symbtablength = GetLengthFromOffset(addrtaboffset - 0x0E, filename);
                    if (symbtablength < 0x1000) return false; // NO SYMBOLTABLE IN FILE
                    symbtaboffset -= 2;
                    if (symbtaboffset > 0 && symbtaboffset < 0x70000)
                    {
                        val = symbtaboffset;

                    }
                    symboltableoffset = val;
                    // MessageBox.Show("Packed table index: " + idx.ToString("X6") + " " + val.ToString("X6"));
                    int state = 0;
                    FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    using (BinaryReader br = new BinaryReader(fsread))
                    {

                        //byte[] filebytes = br.ReadBytes((int)fsread.Length);
                        fsread.Seek(val, SeekOrigin.Begin);
                        //int t = val + 2;
                        //if (filebytes[t - 2] == 0xFF) return false;
                        //for (t = val + 2; t < filebytes.Length; t++)
                        //{

                        //  if (state >= 5) break;
                        //byte b = filebytes[t];
                        //if(filebytes[t] == 
                        if (br.ReadByte() == 0xFF) return false;
                        br.ReadByte(); // dummy

                        // how many bytes are the compressed part in the binary?
                        // assume it ends with 00 00 04 00 00 
                        int bytesread = 0;
                        while (state < 5 && bytesread < 0x20000)
                        {
                            byte testbyte = br.ReadByte();
                            //byte testbyte = filebytes[t];
                            bytesread++;
                            switch (state)
                            {
                                case 0:
                                    if (testbyte == 0x00) state++;
                                    break;
                                case 1:
                                    if (testbyte == 0x00) state++;
                                    else state = 0;
                                    break;
                                case 2:
                                    if (testbyte == 0x04) state++;
                                    else
                                    {
                                        state = 0;
                                        // set filepointer back
                                    }
                                    break;
                                case 3:
                                    if (testbyte == 0x00) state++;
                                    else
                                    {
                                        state = 0;
                                        // set filepointer back
                                    }
                                    break;
                                case 4:
                                    if (testbyte == 0x00) state++;
                                    else
                                    {
                                        state = 0;
                                        // set filepointer back
                                    }
                                    break;

                            }
                        }
                        if (state == 5)
                        {
                            // now read the length of the compressed table
                            fsread.Seek(-7, SeekOrigin.Current);
                            //t -= 6;
                            byte b1 = br.ReadByte();
                            //byte b1 = filebytes[t++];
                            byte b2 = br.ReadByte();
                            //byte b2 = filebytes[t++];
                            int symboltablelength = (b1 * 256) + b2;
                            bool nodummies = false;
                            fsread.Seek(val + 2, SeekOrigin.Begin);
                            //t = val + 2;
                            if (br.ReadByte() == 0x01 && br.ReadByte() == 0x00) nodummies = true;
                            //if (filebytes[t++] == 0x01 && filebytes[t++] == 0x00) nodummies = true;
                            // if the thrid byte = 01 and the fourth = 00 then no dummy byte should be read

                            //t = val;
                            fsread.Seek(val, SeekOrigin.Begin);
                            if (!nodummies)
                            {
                                br.ReadByte(); // dummy
                                br.ReadByte(); // dummy
                                //t += 2;
                            }
                            FileStream fswrite = new FileStream(System.Windows.Forms.Application.StartupPath + "\\COMPR", FileMode.Create);
                            using (BinaryWriter bw = new BinaryWriter(fswrite))
                            {
                                for (int bc = 0; bc < /*bytesread-10*/ symboltablelength; bc++)
                                {
                                    bw.Write(br.ReadByte()/* filebytes[t++]*/);
                                }
                            }
                            fswrite.Close();
                            fswrite.Dispose();
                        }
                    }
                    fsread.Close();
                    fsread.Dispose();
                }
                return true;
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return false;
        }

        static private int GetAddrTableOffset(string filename)
        {
            int addrtaboffset = GetStartOfAddressTableOffset(filename);
            logger.Debug("addrtaboffset: " + addrtaboffset.ToString("X8"));
            //int NqNqNqOffset = GetNqNqNqStringFromOffset(addrtaboffset - 0x100, filename);
            int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
            logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
            //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
            //                    symbtaboffset = NqNqNqOffset;
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
            int retval = NqNqNqOffset + 21;
            logger.Debug("symbtablength: " + symbtablength.ToString("X8"));
            return retval;
        }

        static private int GetAddrTableOffsetBySymbolTable(string filename)
        {
            int addrtaboffset = GetEndOfSymbolTable(filename);
            logger.Debug("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
            //int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
            int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
            logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));

            int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
            logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
            //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
            //                    symbtaboffset = NqNqNqOffset;
            int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
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
                    catch (Exception E)
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
                    catch (Exception E)
                    {
                        logger.Debug("Failed to retrieve NqNqNq from: " + offset.ToString("X6"));
                    }
                    fs.Close();
                }
            }
            return retval;
        }

        static private bool extractCompressedSymbolTable(string filename, out int symboltableoffset, out byte[] bytes)
        {
            bytes = null;
            Int64 UnpackedLength = 0;
            symboltableoffset = 0;
            int len = 0;
            int val = 0;
            //int idx = ReadEndMarker(0x9B);
            try
            {
                //int idx = ReadMarkerAddressContent(filename, 0x9B, out len, out val);
                //if (idx > 0)
                {
                    /* if (val > m_currentfile_size)
                     {
                         // try to find the addresstable offset
                         int addrtaboffset = GetStartOfAddressTableOffset(filename);
                         //TODO: Finish for abused packed files!
                     }*/
                    // FAILSAFE for some files that seem to have protection!
                    /*int addrtaboffset = GetStartOfAddressTableOffset(filename);
                    logger.Debug("addrtaboffset: " + addrtaboffset.ToString("X8"));
                    //int NqNqNqOffset = GetNqNqNqStringFromOffset(addrtaboffset - 0x100, filename);
                    int NqNqNqOffset = GetLastNqStringFromOffset(addrtaboffset - 0x100, filename);
                    logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));*/

                    //<GS-22032010>
                    int addrtaboffset = GetEndOfSymbolTable(filename);
                    logger.Debug("EndOfSymbolTable: " + addrtaboffset.ToString("X8"));
                    int NqNqNqOffset = GetFirstNqStringFromOffset(addrtaboffset, filename);
                    logger.Debug("NqNqNqOffset: " + NqNqNqOffset.ToString("X8"));
                    //<GS-22032010>

                    int symbtaboffset = GetAddressFromOffset(NqNqNqOffset, filename);
                    logger.Debug("symbtaboffset: " + symbtaboffset.ToString("X8"));
                    //int symbtaboffset = GetAddressFromOffset(addrtaboffset - 0x12, filename);
                    //                    symbtaboffset = NqNqNqOffset;
                    int symbtablength = GetLengthFromOffset(NqNqNqOffset + 4 /*addrtaboffset - 0x0E*/, filename);
                    logger.Debug("symbtablength: " + symbtablength.ToString("X8"));
                    if (symbtablength < 0x1000) return false; // NO SYMBOLTABLE IN FILE
                    //symbtaboffset -= 2;
                    if (symbtaboffset > 0 && symbtaboffset < 0xF0000)
                    {
                        val = symbtaboffset;

                    }
                    symboltableoffset = val;
                    // MessageBox.Show("Packed table index: " + idx.ToString("X6") + " " + val.ToString("X6"));
                    int state = 0;
                    FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    using (BinaryReader br = new BinaryReader(fsread))
                    {

                        fsread.Seek(val, SeekOrigin.Begin);

                        UnpackedLength = Convert.ToInt64(br.ReadByte());
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256;
                        UnpackedLength += Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256;
                        logger.Debug("UnpackedLength: " + UnpackedLength.ToString("X8"));
                        fsread.Seek(val, SeekOrigin.Begin);

                        // fill the byte array with the compressed symbol table
                        fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                        bytes = br.ReadBytes(symbtablength);
                    }
                    fsread.Close();
                    fsread.Dispose();
                }
                if (UnpackedLength > 0x00FFFFFF) return false;
                return true;
            }
            catch (Exception E)
            {
                logger.Debug("Error 1: " + E.Message);
            }
            return false;
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
                    catch (Exception E)
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

        static public bool TryToExtractPackedBinary(string filename, int filename_size, out SymbolCollection symbol_collection)
        {
            bool retval = true;
            byte[] compressedSymbolTable;
            //Test 15092009
            //int RealAddressTableOffset = GetAddrTableOffset(filename) + 7; // was 17
            int RealAddressTableOffset = GetAddrTableOffsetBySymbolTable(filename) + 7; // was 17 // <GS-22032010>

            //Test 15092009
            logger.Debug("Real symboltable offset: " + RealAddressTableOffset.ToString("X8"));

            int symboltableoffset = 0;
            symbol_collection = new SymbolCollection();
            CastProgressEvent("Unpacking file... ",5);

            bool compr_created = extractCompressedSymbolTable(filename, out symboltableoffset, out compressedSymbolTable);
            CastProgressEvent("Finding address table... ",15);

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
            int AddressTableOffset = 0;//GetAddressTableOffset(searchsequence);

            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fsread))
            {
                fsread.Seek(symboltableoffset, SeekOrigin.Begin);
                int adr_state = 0;
                while ((fsread.Position < filename_size) && (AddressTableOffset == 0))
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

                if (AddressTableOffset > 0)
                {
                    AddressTableOffset = RealAddressTableOffset; // TEST 15092009
                    CastProgressEvent("Reading address table... ", 25);

                    fsread.Seek(AddressTableOffset - 17, SeekOrigin.Begin);
                    bool endoftable = false;
                    Int64 internal_address = 0;
                    int sramaddress = 0;
                    int symbollength = 0;
                    int bitmask = 0;
                    int symb_count = 0;
                    symbol_collection = new SymbolCollection();
                    while (!endoftable)
                    {
                        // steeds 10 karaketers
                        try
                        {
                            byte[] bytes = br.ReadBytes(10);
                            if (bytes.Length == 10)
                            {
                                // DumpBytesToConsole(bytes);
                                if ((Convert.ToInt32(bytes.GetValue(9)) != 0x00) /*|| (Convert.ToInt32(bytes.GetValue(6)) != 0x00)*/)
                                {
                                    endoftable = true;
                                    //MessageBox.Show("EOT: " + fsread.Position.ToString("X6"));
                                    logger.Debug("EOT: " + fsread.Position.ToString("X6"));
                                }
                                else
                                {
                                    //DumpBytesToConsole(bytes);

                                    internal_address = Convert.ToInt64(bytes.GetValue(0)) * 256 * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(1)) * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(2));

                                    /* if (bytes[1] == 0x7A && bytes[2] == 0xEE)
                                     {
                                         logger.Debug("suspicious");

                                         if (internal_address == 0x7AEE)
                                         {
                                             logger.Debug("break: " + fsread.Position.ToString("X8"));
                                         }
                                     }*/
                                    symbollength = Convert.ToInt32(bytes.GetValue(3)) * 256;
                                    symbollength += Convert.ToInt32(bytes.GetValue(4));

                                    bitmask = Convert.ToInt32(bytes.GetValue(5)) * 256;
                                    bitmask += Convert.ToInt32(bytes.GetValue(6));


                                    //                                                sramaddress = Convert.ToInt32(bytes.GetValue(7)) * 256 * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(8)) * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(9));
                                    SymbolHelper sh = new SymbolHelper();
                                    sh.Symbol_type = Convert.ToInt32(bytes.GetValue(7));
                                    sh.Varname = "Symbolnumber " + symbol_collection.Count.ToString();
                                    sh.Symbol_number = symbol_collection.Count;
                                    sh.Symbol_number_ECU = symbol_collection.Count;
                                    sh.Flash_start_address = internal_address;
                                    sh.Start_address = internal_address;
                                    sh.Length = symbollength;
                                    sh.BitMask = bitmask;
                                    /*if (internal_address == 0x0AE956)
                                    {
                                        DumpBytesToConsole(bytes);
                                    }*/
                                    //DumpToSymbolFile(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));

                                    symbol_collection.Add(sh);
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
                            logger.Debug(E.Message);
                            retval = false;
                        }

                    }
                    if (compr_created)
                    {
                        CastProgressEvent("Decoding packed symbol table",30);

                        string[] allSymbolNames;
                        // Decompress the symbol table
                        TrionicSymbolDecompressor.ExpandComprStream(compressedSymbolTable, out allSymbolNames);
                        AddNamesToSymbols(symbol_collection, allSymbolNames);
                        CastProgressEvent("Idle", 0);
                    }
                }
                else
                {
                    MessageBox.Show("Could not find address table!");
                    retval = false;
                }
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol_collection"></param>
        /// <param name="allSymbolNames"></param>
        private static void AddNamesToSymbols(SymbolCollection symbol_collection, string[] allSymbolNames)
        {
            for (int i = 0; i < allSymbolNames.Length - 1; i++)
            {
                try
                {
                    CastProgressEvent("Adding symbol names: ", (int)(((float)i / (float)allSymbolNames.Length) * 100));
                    SymbolHelper sh = symbol_collection[(i)];
                    sh.Varname = allSymbolNames[i + 1].Trim(); // Skip first in array since its "SymbolNames"
                    logger.Debug(String.Format("Set symbolnumber: {0} to be {1}", sh.Symbol_number, sh.Varname));
                    SymbolTranslator translator = new SymbolTranslator();
                    string help = string.Empty;
                    XDFCategories category = XDFCategories.Undocumented;
                    XDFSubCategory subcat = XDFSubCategory.Undocumented;
                    sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat);
                    if (sh.Varname.Contains("."))
                    {
                        try
                        {
                            sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                        }
                        catch (Exception cE)
                        {
                            logger.Debug(String.Format("Failed to assign category to symbol: {0} err: {1}", sh.Varname, cE.Message));
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug("Failed to add symbolnames: " + E.Message);
                }
            }
        }

        static private void ImportSymbols(System.Data.DataTable dt, SymbolCollection coll2load)
        {
            SymbolTranslator st = new SymbolTranslator();
            int numSym = coll2load.Count;
            int cnt = 0;
            foreach (SymbolHelper sh in coll2load)
            {
                cnt = cnt + 1;
                CastProgressEvent("Importing symbols: ", (int)(((float)cnt / (float)numSym) * 100));
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                        {
                            if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                            {
                                // Swap varname and userdescription
                                if (sh.Varname == String.Format("Symbolnumber {0}", sh.Symbol_number))
                                {
                                    sh.Userdescription = sh.Varname;
                                    sh.Varname = dr["DESCRIPTION"].ToString();
                                }
                                else
                                {
                                    sh.Userdescription = dr["DESCRIPTION"].ToString();
                                }
                                string helptext = string.Empty;
                                XDFCategories cat = XDFCategories.Undocumented;
                                XDFSubCategory sub = XDFSubCategory.Undocumented;
                                sh.Description = st.TranslateSymbolToHelpText(sh.Varname, out helptext, out cat, out sub);

                                if (sh.Category == "Undocumented" || sh.Category == "")
                                {
                                    sh.createAndUpdateCategory(sh.Varname);
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        logger.Debug(E.Message);
                    }
                }
            }
            CastProgressEvent("Completed", 0);
        }

        static public bool TryToLoadAdditionalXMLSymbols(string filename, SymbolCollection coll2load)
        {
            if (File.Exists(filename))
            {
                string binname = GetFileDescriptionFromFile(filename);
                if (binname != string.Empty)
                {
                    System.Data.DataTable dt = new System.Data.DataTable(binname);
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    dt.ReadXml(filename);
                    ImportSymbols(dt, coll2load);
                    return true;
                }
            }
            return false;
        }

        static public bool TryToLoadAdditionalBinSymbols(string filename, SymbolCollection coll2load)
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
                    return TryToLoadAdditionalXMLSymbols(symbolFile, coll2load);
                }
            }

            // Secondly load the .xml file with same path and filename as the .bin file. 
            string xmlfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".xml");
            return TryToLoadAdditionalXMLSymbols(xmlfile, coll2load);
        }
    }
}
