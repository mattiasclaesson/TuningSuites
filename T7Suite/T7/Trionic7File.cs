using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Data;
using CommonSuite;
using NLog;

namespace T7
{
    public class Trionic7File
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

        private int m_sramOffsetForOpenFile;

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

        public class ProgressEventArgs : EventArgs
        {

            public int Percentage { get; set; }

            public string Info { get; set; }

            public ProgressEventArgs(string info, int percentage)
            {
                Info = info;
                Percentage = percentage;
            }
        }

        public delegate void Progress(object sender, ProgressEventArgs e);
        public event Trionic7File.Progress onProgress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public long GetStartVectorAddress(string filename)
        {
            // startvector = second 4 byte word in the file
            byte[] vector_data = readdatafromfile(filename, 4, 4);

            long address = 0;
            for (int i = 0; i < 4; i++)
            {
                address <<= 8;
                address |= Convert.ToInt64(vector_data[i]);
            }
            return address;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public long[] GetVectorAddresses(string filename)
        {
            byte[] vector_data = readdatafromfile(filename, 0, 1024);
            long[] vector_addresses = new long[256];

            for (int i = 0; i < 256; i++)
            {
                long address = 0;
                for (int j = 0; j < 4; j++)
                {
                    address <<= 8;
                    address |= Convert.ToInt64(vector_data[(i * 4) + j]);
                }
                vector_addresses.SetValue(address, i);
            }
            return vector_addresses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public string[] GetVectorNames()
        {
            string[] vector_names = new string[256];
            for (int i = 0; i < 256; i++)
            {
                if (i <= 63)
                {
                    vector_names[i] = ((Trionic7File.VectorType)i).ToString();
                }
                else
                {
                    int number = i - 64;
                    vector_names[i] = "User defined vector " + number;
                }
            }
            return vector_names;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static public byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            try
            {
                using (FileStream fsi1 = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    while (address > fsi1.Length)
                    {
                        address -= (int)fsi1.Length;
                    }
                    using (BinaryReader br1 = new BinaryReader(fsi1))
                    {
                        fsi1.Position = address;
                        for (int i = 0; i < length; i++)
                        {
                            retval.SetValue(br1.ReadByte(), i);
                        }
                    }
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="value"></param>
        /// <param name="filelength"></param>
        /// <param name="length"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static int ReadMarkerAddressContent(string filename, int value, int filelength, out int length, out int val)
        {
            int retval = 0;
            length = 0;
            val = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    // read the file footer - expect this to be 0x07FF00 - 0x07FFFF for T7 files
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            int fileoffset = filelength - 0x90;
                            try
                            {
                                fs.Seek(fileoffset, SeekOrigin.Begin);      // 0x07FF70
                                byte[] inb = br.ReadBytes(0x90);
                                for (int t = 0; t < 0x90; t++)
                                {
                                    if (((byte)inb.GetValue(t) == (byte)value) && ((byte)inb.GetValue(t + 1) < 0x30))
                                    {
                                        // marker gevonden
                                        // lees 6 terug
                                        retval = fileoffset + t;            // 0x07FF70 + t
                                        length = (byte)inb.GetValue(t + 1);
                                        break;
                                    }
                                }
                                fs.Seek((retval - length), SeekOrigin.Begin);
                                byte[] info = br.ReadBytes(length);
                                for (int bc = 0; bc < (info.Length); bc++)
                                {
                                    val <<= 8;
                                    val |= Convert.ToInt32(info.GetValue(bc));
                                }
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                                retval = 0;
                            }
                        }
                    }
                }
            }

            return retval;

        }

        /// <summary>
        /// Check is identifier 0x9B is present in the footer. If this is the case, the file is packed!
        /// </summary>
        /// <param name="m_currentfile"></param>
        /// <returns></returns>
        private static bool IsBinaryPackedVersion(string m_currentfile, int filelength)
        {
            int len = 0;
            int ival = 0;
            int value = ReadMarkerAddressContent(m_currentfile, 0x9B, filelength, out len, out ival);
            if (value > 0 && ival < filelength && ival > 0) return true;
            return false;
        }

        /// <summary>
        /// Search for the start of the Symbol list in an unpacked file
        /// 
        /// There is a region of the BIN file that is filled with 0x00 values
        /// immediately before the start of the Symbol table 
        /// 
        /// This function searches for a region of at least 16 0x00 bytes
        /// and returns the address of the first non-0x00 byte which is
        /// assumed to be the Symbol Table.
        /// 
        /// This function return 0x00 if the Symbol table could not be found
        /// </summary>
        /// <param name="m_currentfile"></param>
        /// <param name="filelength"></param>
        /// <returns>
        /// The location of the start of the Symbol Table or -1 if not found
        /// </returns>
        private static int GetSymbolListOffSet(string m_currentfile, int filelength)
        {
            int retval = 0;
            using (FileStream fsread = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fsread))
                {
                    int zerocount = 0;
                    while ((fsread.Position < filelength) && retval == 0)
                    {
                        byte b = br.ReadByte();
                        if (b == 0x00)
                        {
                            zerocount++;
                        }
                        else
                        {
                            if (zerocount < 15)
                            {
                                zerocount = 0;
                            }
                            else
                            {
                                // NOTE fsread.Position actually points to the address of the next character which we fix before the return
                                retval = (int)fsread.Position;
                            }
                        }
                    }
                }
            }
            retval--;           // return -1 if not found or position of first character in the symbol table
            return retval;
        }

        private int m_Filelength = 0x80000;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="languageID"></param>
        /// <param name="m_current_softwareversion"></param>
        /// <returns></returns>
        public SymbolCollection ExtractFile(string filename, int languageID, string m_current_softwareversion)
        {
            m_fileName = filename;
            FileInfo fi = new FileInfo(filename);
            m_Filelength = (int)fi.Length;
            SymbolTranslator translator = new SymbolTranslator();
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            string help = string.Empty;
            try
            {
                fi.IsReadOnly = false;
            }
            catch (Exception E)
            {
                logger.Debug(String.Format("Failed to clear read-only flag: {0}", E.Message));
            }
            symbol_collection = new SymbolCollection();
            try
            {
                int sym_count = 0; // altered
                CastProgressEvent("Opening file", 0);
                try
                {
                    symbol_collection = new SymbolCollection();
                    if (filename != string.Empty)
                    {
                        if (File.Exists(filename))
                        {
                            if (!IsBinaryPackedVersion(filename, (int)fi.Length))
                            {
                                CastProgressEvent("Getting symbol list offset", 5);
                                int SymbolListOffSet = GetSymbolListOffSet(filename, (int)fi.Length);
                                if (SymbolListOffSet > 0)
                                {
                                    using (FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read))
                                    {
                                        using (BinaryReader br = new BinaryReader(fsread))
                                        {
                                            fsread.Seek(SymbolListOffSet, SeekOrigin.Begin);   // 0x15FA in 5168646.BIN
                                            string symbolname = "";
                                            bool endoftable = false;
                                            while (!endoftable)
                                            {
                                                byte b = br.ReadByte();
                                                switch (b)
                                                {
                                                    case 0xFF:              // 0xFF used to keep the start of each string 'word' aligned
                                                        break;
                                                    case 0x02:
                                                        endoftable = true;
                                                        break;
                                                    case 0x00:              // 0x00 end of Symbol name string
                                                        SymbolHelper sh = new SymbolHelper() { Varname = symbolname, Description = translator.TranslateSymbolToHelpText(symbolname, out help, out category, out subcat, languageID) };
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
                                                        sh.Internal_address = (int)fsread.Position - symbolname.Length - 1;
                                                        sh.Symbol_number = sym_count;
                                                        symbol_collection.Add(sh);
                                                        symbolname = "";
                                                        sym_count++;
                                                        if ((sym_count % 500) == 0)
                                                        {
                                                            CastProgressEvent(String.Format("Symbol: {0}", sh.Varname), 10);
                                                        }
                                                        break;
                                                    default:                // Another character in the Symbol name
                                                        symbolname += Convert.ToChar(b);
                                                        break;
                                                }
                                            }
                                            // now, try to get the addresses
                                            // 00 00 00 00 00 00 00 00 20 00 00 00 15 FA
                                            // 00 F0 75 3E 00 01 00 00 24 00 00 00 16 04
                                            // 00 F0 75 3F 00 01 00 00 24 00 00 00 16 10
                                            // 00 F0 75 40 00 01 00 00 24 00 00 00 16 1E
                                            // 00 F0 75 42 00 02 00 00 20 00 00 00 16 2C
                                            // 00 F0 75 44 00 02 00 00 ...
                                            if (symbol_collection.Count > 0)
                                            {
                                                sym_count = 0;
                                                CastProgressEvent("Searching address lookup table", 15);
                                                byte firstaddr_high = (byte)(symbol_collection[0].Internal_address >> 8);
                                                byte firstaddr_low = (byte)(symbol_collection[0].Internal_address);
                                                byte[] searchPattern = { 
                                                                           0x00, 0x00, 0x00, 0x00,
                                                                           0x00, 0x00, 0x00, 0x00,
                                                                           0x20, 0x00, 0x00, 0x00,
                                                                           0x00, 0x00, 0x00, 0xF0
                                                                       };
                                                searchPattern.SetValue(firstaddr_high, 12);
                                                searchPattern.SetValue(firstaddr_low, 13);
                                                int addressTableOffset = bytePatternSearch(filename, searchPattern, 0);
                                                if (addressTableOffset != -1)
                                                {
                                                    fsread.Seek(addressTableOffset, SeekOrigin.Begin);
                                                    // steeds 14 karaketers
                                                    for (int t = 0; t < symbol_collection.Count; t++)
                                                    {
                                                        try
                                                        {
                                                            byte[] bytes = br.ReadBytes(14);
                                                            if (bytes.Length == 14)
                                                            {
                                                                int internal_address = 0;
                                                                for (int i = 0; i < 4; i++)
                                                                {
                                                                    internal_address <<= 8;
                                                                    internal_address |= Convert.ToInt32(bytes.GetValue(i + 10));
                                                                }
                                                                int symbollength = 0;
                                                                for (int i = 0; i < 2; i++)
                                                                {
                                                                    symbollength <<= 8;
                                                                    symbollength |= Convert.ToInt32(bytes.GetValue(i + 4));
                                                                }
                                                                int sramaddress = 0;
                                                                for (int i = 0; i < 4; i++)
                                                                {
                                                                    sramaddress <<= 8;
                                                                    sramaddress |= Convert.ToInt32(bytes.GetValue(i));
                                                                }
                                                                int realromaddress;
                                                                if (sramaddress > 0xF00000)
                                                                    realromaddress = sramaddress - 0xef02f0;
                                                                else
                                                                    realromaddress = 0;
                                                                foreach (SymbolHelper sh in symbol_collection)
                                                                {
                                                                    if (sh.Internal_address == internal_address)
                                                                    {
                                                                        /*if (sh.Varname == "BFuelCal.Map")
                                                                        {
                                                                            logger.Debug("Break for fuel map: " + internal_address.ToString("X6"));

                                                                        }*/
                                                                        if (sramaddress > 0 && sh.Flash_start_address == 0)
                                                                            sh.Start_address = sramaddress; // TEST
                                                                        if (realromaddress > 0 && sh.Varname.Contains("."))
                                                                            sh.Flash_start_address = realromaddress;
                                                                        else
                                                                            sh.Flash_start_address = sramaddress;
                                                                        sh.Length = symbollength;
                                                                        sym_count++;
                                                                        if ((sym_count % 500) == 0)
                                                                        {
                                                                            CastProgressEvent(String.Format("{0} : {1:X6}", sh.Varname, sh.Flash_start_address), 20);
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception E)
                                                        {
                                                            logger.Debug(E.Message);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!tryToDecodePackedBinary(filename, out symbol_collection, languageID))
                                                    {
                                                        logger.Debug("Failed to extract packed binary!"); //<GS-23022010>
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Application.DoEvents();
                                    logger.Debug("Couldn't find symboltable, file is probably packed!");//<GS-23022010>
                                }
                            }
                            else
                            {
                                tryToDecodePackedBinary(filename, out symbol_collection, languageID);
                            }
                            // try to load additional symboltranslations that the user entered
                            TryToLoadAdditionalSymbols(filename, symbol_collection, languageID);
                        }
                    }
                }
                catch (Exception eBin)
                {
                    logger.Debug("Failed to open binfile: " + eBin.Message);
                }
                CastProgressEvent("Decoding done", 55);
            }
            catch (Exception E)
            {
                logger.Debug(String.Format("TryOpenFile filed: {0} err: {1}", filename, E.Message));

            }
            return symbol_collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetFileDescriptionFromFile(string file)
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
                        name = name.Replace(String.Format("_x003{0}_", i), i.ToString());
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="m_symbols"></param>
        /// <param name="LanguageID"></param>
        private static void TryToLoadAdditionalSymbols(string filename, SymbolCollection m_symbols, int LanguageID)
        {
            DataTable dt = new DataTable(Path.GetFileNameWithoutExtension(filename));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            T7FileHeader fh = new T7FileHeader();
            fh.init(filename, false);
            string checkstring = fh.getPartNumber() + fh.getSoftwareVersion();
            string xmlfilename = String.Format("{0}\\repository\\{1}{2:yyyyMMddHHmmss}{3}.xml", Application.StartupPath, Path.GetFileNameWithoutExtension(filename), File.GetCreationTime(filename), checkstring);
            if (File.Exists(xmlfilename))
            {
                dt.ReadXml(xmlfilename);
            }
            else
            {
                // check the file folder
                string[] xmlfiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*.xml");
                foreach (string xmlfile in xmlfiles)
                {
                    if (Path.GetFileName(xmlfile).StartsWith(Path.GetFileNameWithoutExtension(filename)))
                    {
                        dt.ReadXml(xmlfile);
                        logger.Debug(String.Format("Read: {0} symbols from {1}", dt.Rows.Count, xmlfile));
                        break;
                    }
                }
            }
            // auto add symbols for 55P / 46T files only if no other sources of additional symbols can be found
            bool createRepositoryFile = false;
            if (dt.Rows.Count == 0)
            {
                if (fh.getSoftwareVersion().Trim().StartsWith("EU0AF01C", StringComparison.OrdinalIgnoreCase) ||
                    fh.getSoftwareVersion().Trim().StartsWith("EU0BF01C", StringComparison.OrdinalIgnoreCase) ||
                    fh.getSoftwareVersion().Trim().StartsWith("EU0CF01C", StringComparison.OrdinalIgnoreCase))
                {
                    if (MessageBox.Show("Do you want to load the known symbollist for EU0AF01C/EU0BF01C/EU0CF01C files now?", "Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string BioPowerXmlFile = String.Format("{0}\\EU0AF01C.xml", Application.StartupPath);
                        if (File.Exists(BioPowerXmlFile))
                        {
                            string binname = GetFileDescriptionFromFile(BioPowerXmlFile);
                            if (binname != string.Empty)
                            {
                                dt = new DataTable(binname);
                                dt.Columns.Add("SYMBOLNAME");
                                dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                                dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                                dt.Columns.Add("DESCRIPTION");
                                dt.ReadXml(BioPowerXmlFile);
                                createRepositoryFile = true;
                            }
                        }
                    }
                }
            }
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    SymbolHelper sh = m_symbols[Convert.ToInt32(dr["SYMBOLNUMBER"])];
                    if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                    {
                        if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                        {
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
                            SymbolTranslator st = new SymbolTranslator();
                            sh.Description = st.TranslateSymbolToHelpText(sh.Varname, out helptext, out cat, out sub, LanguageID);
                            if (sh.Category == "Undocumented" || sh.Category == "")
                            {
                                if (sh.Varname.Contains("."))
                                {
                                    try
                                    {
                                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                                        //logger.Debug(String.Format("Set cat to {0} for {1}", sh.Category, sh.Userdescription));
                                    }
                                    catch (Exception cE)
                                    {
                                        logger.Debug(String.Format("Failed to assign category to symbol: {0} err: {1}", sh.Userdescription, cE.Message));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception E)
                {
                    logger.Debug(E.Message);
                }
            }
            if (createRepositoryFile)
            {
                SaveAdditionalSymbols(filename, m_symbols);
            }
            foreach (SymbolHelper sh in m_symbols)
            {
                if (sh.Varname == "X_AccPedalManSP" || sh.Varname == "X_AccPedalAutTAB" || sh.Varname == "X_AccPedalAutSP" || sh.Varname == "X_AccPedalManTAB" || sh.Userdescription == "X_AccPedalManSP" || sh.Userdescription == "X_AccPedalAutTAB" || sh.Userdescription == "X_AccPedalAutSP" || sh.Userdescription == "X_AccPedalManTAB")
                {
                    if (sh.Length == 4)
                    {
                        sh.Flash_start_address -= 0x0C;
                        sh.Length = 0x0C;
                    }
                }
            }
            dt.Dispose();
        }

        private static void SaveAdditionalSymbols(string filename, SymbolCollection m_symbols)
        {
            using (DataTable dt = new DataTable(Path.GetFileNameWithoutExtension(filename)))
            {
                dt.Columns.Add("SYMBOLNAME");
                dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                dt.Columns.Add("DESCRIPTION");
                T7FileHeader fh = new T7FileHeader();
                fh.init(filename, false);
                string checkstring = fh.getPartNumber() + fh.getSoftwareVersion();
                string xmlfilename = String.Format("{0}\\repository\\{1}{2:yyyyMMddHHmmss}{3}.xml", Application.StartupPath, Path.GetFileNameWithoutExtension(filename), File.GetCreationTime(filename), checkstring);
                if (Directory.Exists(String.Format("{0}\\repository", Application.StartupPath)))
                {
                    if (File.Exists(xmlfilename))
                    {
                        File.Delete(xmlfilename);
                    }
                }
                else
                {
                    Directory.CreateDirectory(String.Format("{0}\\repository", Application.StartupPath));
                }
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Userdescription != "")
                    {
                        if (sh.Userdescription == String.Format("Symbolnumber {0}", sh.Symbol_number))
                        {
                            dt.Rows.Add(sh.Userdescription, sh.Symbol_number, sh.Flash_start_address, sh.Varname);
                        }
                        else
                        {
                            dt.Rows.Add(sh.Varname, sh.Symbol_number, sh.Flash_start_address, sh.Userdescription);
                        }
                    }
                }
                dt.WriteXml(xmlfilename);
            }
        }

        /// <summary>
        /// Search a file for an arbitrary pattern of bytes
        /// specifying an 0ffset from the start of the file
        /// to search from.
        /// 
        /// Returns the location of the first byte in the
        /// search pattern, or -1 if the pattern wasn't
        /// found.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="searchBytes"></param>
        /// <param name="startOffset"></param>
        /// <returns></returns>
        private static int bytePatternSearch(string filename, byte[] searchBytes, int startOffset)
        {
            int found = -1;
            bool matched = false;
            FileInfo fi = new FileInfo(filename);
            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            //only look at this if we have a populated file and search bytes with a sensible start
            if (fi.Length > 0 && searchBytes.Length > 0 && startOffset <= (fi.Length - searchBytes.Length) && fi.Length >= searchBytes.Length)
            {
                using (BinaryReader br = new BinaryReader(fsread))
                {
                    fsread.Seek(startOffset, SeekOrigin.Begin);
                    //iterate through the file to be searched
                    for (int i = startOffset; i <= fi.Length - searchBytes.Length; i++)
                    {
                        //if the start bytes match we will start comparing all other bytes
                        if (br.ReadByte() == searchBytes[0])
                        {
                            if (fi.Length > 1)
                            {
                                //multiple bytes to be searched we have to compare byte by byte
                                matched = true;
                                for (int y = 1; y <= searchBytes.Length - 1; y++)
                                {
                                    if (br.ReadByte() != searchBytes[y])
                                    {
                                        matched = false;
                                        fsread.Position -= y;
                                        break;
                                    }
                                }
                                //everything matched up
                                if (matched)
                                {
                                    found = i;
                                    break;
                                }
                            }
                            else
                            {
                                //search pattern is only one byte nothing else to do
                                found = i;
                                break; //stop the loop
                            }
                        }
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static int GetAddressFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            try
                            {
                                fs.Seek(offset, SeekOrigin.Begin);
                                for (int i = 3; i >= 0; i--)
                                {
                                    retval |= Convert.ToInt32(br.ReadByte()) << (8 * i);
                                }
                            }
                            catch (Exception E)
                            {
                                logger.Debug(String.Format("Failed to retrieve address from: {0:X6}: {1}", offset, E.Message));
                            }
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static int GetLengthFromOffset(int offset, string filename)
        {
            int retval = 0;
            if (filename != string.Empty)
            {
                if (File.Exists(filename))
                {
                    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            try
                            {
                                fs.Seek(offset, SeekOrigin.Begin);
                                for (int i = 1; i >= 0; i--)
                                {
                                    retval |= Convert.ToInt32(br.ReadByte()) << (8 * i);
                                }
                            }
                            catch (Exception E)
                            {
                                logger.Debug(String.Format("Failed to retrieve length from: {0:X6}: {1}", offset, E.Message));
                            }
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Attempts to locate and extract a compressed symbol
        /// table from the specified file. If succesful the
        /// symbol table is presented as a byte[] array and
        /// the address of the first symbol reference is given
        /// 
        /// The symbol table is missing from some (BioPower)
        /// BIN files. For these files FALSE is returned
        /// and the address for the missing symbol table is
        /// given as a workaround for BioPower XML files...
        /// </summary>
        /// <param name="filename">Filename of BIN file to extract compressed symbol table from</param>
        /// <param name="addressTableOffset">Address of fist symbol (or missing symbol table if BioPower)</param>
        /// <param name="bytes">The compressed symbol table as a byte[] array </param>
        /// <returns>TRUE: symbol table present, FALSE: no symbol table in file</returns>
        private bool extractCompressedSymbolTable(string filename, out int addressTableOffset, out byte[] bytes)
        {
            bytes = null;
            addressTableOffset = -1;
            try
            {
                // try to find the addresstable offset
                // TODO: Finish for abused packed files!
                //       Add a FAILSAFE for some files that seem to have protection!
                byte[] searchPattern = {
                                           0x00, 0x00, 0x04, 0x00,
                                           0x00, 0x00, 0x00, 0x00,
                                           0x00, 0x00, 0x00, 0x00,
                                           0x20, 0x00
                                       };
                addressTableOffset = bytePatternSearch(filename, searchPattern, 0x30000) - 0x06;
                // !!! m_sramOffsetForOpenFile is a PUBLIC variable !!!
                m_sramOffsetForOpenFile = GetAddressFromOffset(addressTableOffset - 0x06, filename);
                int symbolTableOffset = GetAddressFromOffset(addressTableOffset, filename);
                int symbolTableLength = GetLengthFromOffset(addressTableOffset + 0x04, filename);
                if (symbolTableLength > 0x1000 && symbolTableOffset > 0 && symbolTableOffset < 0x70000)
                {
                    using (FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fsread))
                        {
                            // fill the byte array with the compressed symbol table
                            fsread.Seek(symbolTableOffset, SeekOrigin.Begin);
                            bytes = br.ReadBytes(symbolTableLength);
                        }
                    }
                    return true;
                }
            }
            catch (Exception E)
            {
                logger.Debug(E.Message);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="symbol_collection"></param>
        /// <param name="languageID"></param>
        /// <returns></returns>
        private bool tryToDecodePackedBinary(string filename, out SymbolCollection symbol_collection, int languageID)
        {
            bool retval = true;
            int addressTableOffset;
            byte[] compressedSymbolTable;
            bool compr_created = extractCompressedSymbolTable(filename, out addressTableOffset, out compressedSymbolTable);
            logger.Debug(String.Format("Compr_created: ", compr_created));
            CastProgressEvent("Searching address lookup table", 30);

            symbol_collection = new SymbolCollection();
            if (addressTableOffset != -1)
            {
                symbol_collection = new SymbolCollection();
                logger.Debug(String.Format("SOT: {0:X6}", addressTableOffset));
                using (FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fsread))
                    {
                        fsread.Seek(addressTableOffset, SeekOrigin.Begin);
                        int symb_count = 0;
                        bool endoftable = false;
                        while (!endoftable)
                        {
                            // steeds 10 karaketers - each address table entry is 10 bytes in size
                            try
                            {
                                byte[] bytes = br.ReadBytes(10);
                                if (bytes.Length == 10 && (Convert.ToInt32(bytes.GetValue(0)) != 0x53) && (Convert.ToInt32(bytes.GetValue(1)) != 0x43))     // "SC"
                                {
                                    Int64 internal_address = 0;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        internal_address <<= 8;
                                        internal_address |= Convert.ToInt64(bytes.GetValue(i));
                                    }
                                    int symbollength = 0;
                                    if (symb_count == 0)
                                    {
                                        symbollength = 0x08;            // report only a few bytes of the compressed symbol
                                    }
                                    else
                                    {
                                        for (int i = 4; i < 6; i++)
                                        {
                                            symbollength <<= 8;
                                            symbollength |= Convert.ToInt32(bytes.GetValue(i));
                                        }
                                    }
                                    // might be damaged addresstable by MapTun.. correct it automatically
                                    if (internal_address == 0x00 && symbollength > 0 && symbol_collection.Count > 0)
                                    {
                                        internal_address = symbol_collection[symbol_collection.Count - 1].Start_address + symbol_collection[symbol_collection.Count - 1].Length;
                                        if (symbollength == 0x240 && (internal_address % 2) > 0)
                                            internal_address++;
                                        logger.Debug(String.Format("Corrected symbol with address: {0:X8} and len {1:X4}", internal_address, symbollength));
                                    }
                                    SymbolHelper sh = new SymbolHelper()
                                    {
                                        Symbol_number = symb_count,
                                        Symbol_type = Convert.ToInt32(bytes.GetValue(8)),
                                        Varname = String.Format("Symbolnumber {0}", symbol_collection.Count),
                                        Flash_start_address = internal_address,
                                        Start_address = internal_address,
                                        Length = symbollength
                                    };
                                    symbol_collection.Add(sh);
                                    if (symb_count % 500 == 0)
                                    {
                                        CastProgressEvent(String.Format("{0} : {1:X6}", sh.Varname, sh.Flash_start_address), 35);
                                    }
                                    symb_count++;
                                }
                                else
                                {
                                    //MessageBox.Show("EOT: " + fsread.Position.ToString("X6"));
                                    logger.Debug(String.Format("EOT: {0:X6}", fsread.Position));
                                    endoftable = true;
                                }
                            }
                            catch (Exception E)
                            {
                                logger.Debug(E.Message);
                                retval = false;
                            }
                        }
                    }
                }
                if (compr_created)
                {
                    //logger.Debug("Decoding packed symbol table");
                    CastProgressEvent("Decoding packed symbol table", 40);
                    string[] allSymbolNames;
                    TrionicSymbolDecompressor.ExpandComprStream(compressedSymbolTable, out allSymbolNames);
                    //logger.Debug("Adding names to symbols");
                    CastProgressEvent("Adding names to symbols", 45);
                    AddNamesToSymbols(symbol_collection, allSymbolNames, languageID);
                    CastProgressEvent("Cleaning up", 50);
                }
            }
            else
            {
                logger.Debug("Could not find address table!"); //<GS-23022010>
                retval = false;
            }
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol_collection"></param>
        /// <param name="allSymbolNames"></param>
        /// <param name="LanguageID"></param>
        private static void AddNamesToSymbols(SymbolCollection symbol_collection, string[] allSymbolNames, int LanguageID)
        {
            for (int i = 0; i < allSymbolNames.Length; i++)
            {
                try
                {
                    SymbolHelper sh = symbol_collection[(i)];
                    sh.Varname = allSymbolNames[i].Trim();
                    //logger.Debug(String.Format("Set symbolnumber: {0} to be {1}", sh.Symbol_number, symbol));
                    SymbolTranslator translator = new SymbolTranslator();
                    string help = string.Empty;
                    XDFCategories category = XDFCategories.Undocumented;
                    XDFSubCategory subcat = XDFSubCategory.Undocumented;
                    sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, LanguageID);

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
    }
}
