using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using CommonSuite;

namespace T8SuitePro
{
    public class TrionicFile
    {
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

        private void CastProgressEvent(string info, int percentage)
        {
            if (onProgress != null)
            {
                onProgress(this, new ProgressEventArgs(info, percentage));
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
        public event TrionicFile.Progress onProgress;

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
                    vector_names[i] = ((TrionicFile.VectorType)i).ToString();
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
                Console.WriteLine(E.Message);
            }
            return retval;
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
                        Console.WriteLine(E.Message);
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
                Console.WriteLine(E.Message);
            }
            return retval;
        }

        private int GetStartOfAddressTableOffset(string filename)
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
                                Console.WriteLine("Hola");
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

        private int GetAddressFromOffset(int offset, string filename)
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
                        Console.WriteLine("Failed to retrieve address from: " + offset.ToString("X6") + ": " + E.Message);
                    }
                    fs.Close();
                }
            }
            return retval;
        }


        private int GetLengthFromOffset(int offset, string filename)
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
                        Console.WriteLine("Failed to retrieve length from: " + offset.ToString("X6") + ": " + E.Message);
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
                Console.WriteLine(E.Message);
            }
            return false;
        }
    }
}
