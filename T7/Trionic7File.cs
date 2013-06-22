using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T7;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Data;

namespace T7
{
    public class Trionic7File
    {

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
        public event Trionic7File.Progress onProgress;

        private int ReadMarkerAddress(string filename, int value, int filelength, out int length, out string val)
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

        public long GetStartVectorAddress(string filename, int number)
        {
            long retval = 0;
            Int32 start_address = number * 4;
            retval = Convert.ToInt64(readdatafromfile(filename, start_address, 1)[0]) * 256 * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(filename, start_address + 1, 1)[0]) * 256 * 256;
            retval += Convert.ToInt64(readdatafromfile(filename, start_address + 2, 1)[0]) * 256;
            retval += Convert.ToInt64(readdatafromfile(filename, start_address + 3, 1)[0]);
            return retval;
        }

        public long[] GetVectorAddresses(string filename)
        {
            long[] vectors = new long[256];
            for (int i = 0; i < 256; i++)
            {
                vectors.SetValue(GetStartVectorAddress(filename, i), i);
            }
            return vectors;
        }

        public byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            try
            {
                FileStream fsi1 = new FileStream(filename, FileMode.Open, FileAccess.Read);
                //FileStream fsi1 = File.OpenRead(filename);
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

        private int ReadMarkerAddressContent(string filename, int value, int filelength, out int length, out int val)
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
        private bool IsBinaryPackedVersion(string m_currentfile, int filelength)
        {
            int len = 0;
            string val = "";
            int ival = 0;
            int value = ReadMarkerAddress(m_currentfile, 0x9B, filelength, out len, out val);
            value = ReadMarkerAddressContent(m_currentfile, 0x9B, filelength, out len, out ival);
            if (value > 0 && ival < filelength && ival > 0) return true;
            return false;
        }

        private int GetSymbolListOffSet(string m_currentfile, int filelength)
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
                Console.WriteLine("Failed to clear read-only flag: " + E.Message);
            }
            symbol_collection = new SymbolCollection();
            try
            {
                int sym_count = 0; // altered
                //frmProgress progress = new frmProgress();
                //progress.Show();
                //progress.SetProgress("Opening file");
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
                                //progress.SetProgress("Getting symbol list offset");
                                CastProgressEvent("Getting symbol list offset", 5);


                                int SymbolListOffSet = GetSymbolListOffSet(filename, (int)fi.Length);
                                if (SymbolListOffSet > 0)
                                {
                                    FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                                    using (BinaryReader br = new BinaryReader(fsread))
                                    {
                                        fsread.Seek(/*0x15F9*/ SymbolListOffSet, SeekOrigin.Begin);
                                        bool endoftable = false;
                                        int state = 0;
                                        int internal_address = 0;
                                        string symbolname = "";
                                        while (!endoftable)
                                        {
                                            byte b = br.ReadByte();
                                            switch (state)
                                            {
                                                case 0:
                                                    if (b == 0x00)
                                                    {
                                                        symbolname = "";
                                                        internal_address = 0;
                                                        state++;
                                                    }
                                                    break;
                                                case 1:
                                                    if (b == 0xff) ;

                                                    else if (b == 0x02)
                                                    {
                                                        endoftable = true;
                                                    }
                                                    else if (b == 0x00)
                                                    {
                                                        SymbolHelper sh = new SymbolHelper();
                                                        sh.Varname = symbolname;

                                                        sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, languageID);
                                                        if (sh.Varname.Contains("."))
                                                        {
                                                            try
                                                            {
                                                                sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                                                            }
                                                            catch (Exception cE)
                                                            {
                                                                Console.WriteLine("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                                                            }
                                                        }
                                                        sh.Internal_address = internal_address - 1;
                                                        sh.Symbol_number = sym_count;
                                                        symbol_collection.Add(sh);
                                                        symbolname = "";
                                                        internal_address = 0;
                                                        sym_count++;
                                                        if ((sym_count % 500) == 0)
                                                        {
                                                            //progress.SetProgress("Symbol: " + sh.Varname);
                                                            CastProgressEvent("Symbol: " + sh.Varname, 10);

                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (internal_address == 0) internal_address = (int)fsread.Position;
                                                        symbolname += Convert.ToChar(b);
                                                    }
                                                    break;
                                            }
                                        }
                                        // now, try to get the addresses
                                        // 20 00 00 00 15 FA 00 F0 75 3E 00 01 00 00 
                                        // 24 00 00 00 16 04 00 F0 75 3F 00 01 00 00 
                                        // 24 00 00 00 16 10 00 F0 75 40 00 01 00 00 
                                        // 24 00 00 00 16 1E 00 F0 75 42 00 02 00 00 
                                        // 20 00 00 00 16 2C 00 F0 75 44 00 02 00 00 
                                        if (symbol_collection.Count > 0)
                                        {
                                            sym_count = 0;
                                            //progress.SetProgress("Searching address lookup table");
                                            CastProgressEvent("Searching address lookup table", 15);

                                            byte[] searchsequence = new byte[8];
                                            byte firstaddr_high = (byte)(symbol_collection[0].Internal_address / 256);
                                            byte firstaddr_low = (byte)(symbol_collection[0].Internal_address - (firstaddr_high * 256));
                                            searchsequence.SetValue((byte)0x20, 0);
                                            searchsequence.SetValue((byte)0x00, 1);
                                            searchsequence.SetValue((byte)0x00, 2);
                                            searchsequence.SetValue((byte)0x00, 3);
                                            searchsequence.SetValue(firstaddr_high, 4);
                                            searchsequence.SetValue(firstaddr_low, 5);
                                            searchsequence.SetValue((byte)0x00, 6);
                                            searchsequence.SetValue((byte)0xF0, 7);
                                            int AddressTableOffset = 0;//GetAddressTableOffset(searchsequence);
                                            fsread.Seek(0, SeekOrigin.Begin);
                                            int adr_state = 0;
                                            byte[] filebytes = br.ReadBytes((int)fsread.Length);
                                            for (int t = 0; t < filebytes.Length; t++)
                                            {
                                                if (AddressTableOffset != 0) break;

                                                //while ((fsread.Position < filename_size) && (AddressTableOffset == 0))
                                                //{
                                                byte adrb = filebytes[t];
                                                //byte adrb = br.ReadByte();
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
                                                        else adr_state = 0;
                                                        break;
                                                    case 2:
                                                        if (adrb == (byte)searchsequence.GetValue(2)) adr_state++;
                                                        else adr_state = 0;
                                                        break;
                                                    case 3:
                                                        if (adrb == (byte)searchsequence.GetValue(3)) adr_state++;
                                                        else adr_state = 0;
                                                        break;
                                                    case 4:
                                                        if (adrb == (byte)searchsequence.GetValue(4)) adr_state++;
                                                        else adr_state = 0;
                                                        break;
                                                    case 5:
                                                        if (adrb == (byte)searchsequence.GetValue(5)) adr_state++;
                                                        else adr_state = 0;
                                                        break;
                                                    case 6:
                                                        if (adrb == (byte)searchsequence.GetValue(6)) adr_state++;
                                                        else adr_state = 0;
                                                        break;
                                                    case 7:
                                                        if (adrb == (byte)searchsequence.GetValue(7))
                                                        {
                                                            // found it
                                                            AddressTableOffset = t - 7;
                                                            //AddressTableOffset = (int)fsread.Position - 8;
                                                        }
                                                        else adr_state = 0;
                                                        break;
                                                }

                                            }
                                            if (AddressTableOffset > 0)
                                            {
                                                fsread.Seek(AddressTableOffset - 8, SeekOrigin.Begin);
                                                endoftable = false;
                                                internal_address = 0;
                                                int sramaddress = 0;
                                                int symbollength = 0;
                                                int highestsramaddress = 0;
                                                // while (!endoftable)
                                                {
                                                    // steeds 14 karaketers

                                                    for (int t = 0; t < symbol_collection.Count; t++)
                                                    {
                                                        try
                                                        {
                                                            byte[] bytes = br.ReadBytes(14);
                                                            if (bytes.Length == 14)
                                                            {
                                                                internal_address = Convert.ToInt32(bytes.GetValue(11)) * 256 * 256;
                                                                internal_address += Convert.ToInt32(bytes.GetValue(12)) * 256;
                                                                internal_address += Convert.ToInt32(bytes.GetValue(13));
                                                                symbollength = Convert.ToInt32(bytes.GetValue(4)) * 256;
                                                                symbollength += Convert.ToInt32(bytes.GetValue(5));
                                                                sramaddress = Convert.ToInt32(bytes.GetValue(1)) * 256 * 256;
                                                                sramaddress += Convert.ToInt32(bytes.GetValue(2)) * 256;
                                                                sramaddress += Convert.ToInt32(bytes.GetValue(3));
                                                                int indicator = Convert.ToInt32(bytes.GetValue(8));
                                                                int realromaddress = 0;
                                                                if (sramaddress > 0xF00000) realromaddress = sramaddress - 0xef02f0;

                                                                //DumpBytesToConsole(bytes);

                                                                foreach (SymbolHelper sh in symbol_collection)
                                                                {
                                                                    if (sh.Internal_address == internal_address)
                                                                    {
                                                                        /*if (sh.Varname == "BFuelCal.Map")
                                                                        {
                                                                            Console.WriteLine("Break for fuel map: " + internal_address.ToString("X6"));

                                                                        }*/
                                                                        if (realromaddress > 0)
                                                                        {
                                                                            if (sramaddress > highestsramaddress) highestsramaddress = sramaddress;
                                                                        }

                                                                        if (sramaddress > 0 && sh.Flash_start_address == 0)
                                                                            sh.Start_address = sramaddress; // TEST
                                                                        sh.Flash_start_address = sramaddress;
                                                                        if (realromaddress > 0 && sh.Varname.Contains(".")) sh.Flash_start_address = realromaddress;
                                                                        sh.Length = symbollength;
                                                                        sym_count++;
                                                                        if ((sym_count % 500) == 0)
                                                                        {
                                                                           // progress.SetProgress(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));
                                                                            CastProgressEvent(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"), 20);
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception E)
                                                        {
                                                            Console.WriteLine(E.Message);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                fsread.Close();
                                                if (!TryToExtractPackedBinary(filename, sym_count, (int)fi.Length, out symbol_collection, languageID, m_current_softwareversion))
                                                {
                                                    Console.WriteLine("Failed to extract packed binary!");//<GS-23022010>
                                                }
                                            }
                                        }
                                    }
                                    fsread.Close();
                                    fsread.Dispose();
                                }

                                else
                                {
                                    //if (progress != null)
                                    //{
                                        //progress.Close();
                                    //}
                                    System.Windows.Forms.Application.DoEvents();
                                    Console.WriteLine("Couldn't find symboltable, file is probably packed!");//<GS-23022010>
                                }
                            }
                            else
                            {
                                TryToExtractPackedBinary(filename, sym_count, (int)fi.Length, out symbol_collection, languageID, m_current_softwareversion);
                            }
                            // try to load additional symboltranslations that the user entered
                            TryToLoadAdditionalSymbols(filename, true, symbol_collection, languageID);
                        }
                    }
                }
                catch (Exception eBin)
                {
                    Console.WriteLine("Failed to open binfile: " + eBin.Message);
                }
                CastProgressEvent("Decoding done", 60);
            }
            catch (Exception E)
            {
                Console.WriteLine("TryOpenFile filed: " + filename + " err: " + E.Message);
                
            }
            
            return symbol_collection;
        }

        private void DumpBytesToConsole(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.Write(b.ToString("X2") + " ");
            }
            Console.WriteLine();
        }

        private string GetFileDescriptionFromFile(string file)
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

        private void TryToLoadAdditionalSymbols(string filename, bool ImportFromRepository, SymbolCollection m_symbols, int LanguageID)
        {
            bool _faultyAxisForAccPedalDetected = false;
            SymbolTranslator st = new SymbolTranslator();
            System.Data.DataTable dt = new System.Data.DataTable(Path.GetFileNameWithoutExtension(filename));
            dt.Columns.Add("SYMBOLNAME");
            dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
            dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
            dt.Columns.Add("DESCRIPTION");
            if (ImportFromRepository)
            {
                T7FileHeader fh = new T7FileHeader();
                fh.init(filename, false);
                string checkstring = fh.getPartNumber() + fh.getSoftwareVersion();
                string xmlfilename = System.Windows.Forms.Application.StartupPath + "\\repository\\" + Path.GetFileNameWithoutExtension(filename) + File.GetCreationTime(filename).ToString("yyyyMMddHHmmss") + checkstring + ".xml";
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\repository"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\repository");
                }
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
                            Console.WriteLine("Read: " + dt.Rows.Count.ToString() + " symbols from " + xmlfile);
                            break;
                        }
                    }
                }
            }
            else
            {
                string binname = GetFileDescriptionFromFile(filename);
                if (binname != string.Empty)
                {
                    dt = new System.Data.DataTable(binname);
                    dt.Columns.Add("SYMBOLNAME");
                    dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                    dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                    dt.Columns.Add("DESCRIPTION");
                    if (File.Exists(filename))
                    {
                        dt.ReadXml(filename);
                    }
                }
            }
            foreach (SymbolHelper sh in m_symbols)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["SYMBOLNAME"].ToString() == sh.Varname)
                        {
                            if (sh.Symbol_number == Convert.ToInt32(dr["SYMBOLNUMBER"]))
                            {
                                if (sh.Flash_start_address == Convert.ToInt32(dr["FLASHADDRESS"]))
                                {
                                    sh.Userdescription = dr["DESCRIPTION"].ToString();
                                    string helptext = string.Empty;
                                    XDFCategories cat = XDFCategories.Undocumented;
                                    XDFSubCategory sub = XDFSubCategory.Undocumented;
                                    sh.Description = st.TranslateSymbolToHelpText(sh.Userdescription, out helptext, out cat, out sub, LanguageID);
                                    //if(sh.Category == 
                                    if (sh.Category == "Undocumented" || sh.Category == "")
                                    {
                                        if (sh.Userdescription.Contains("."))
                                        {
                                            try
                                            {
                                                sh.Category = sh.Userdescription.Substring(0, sh.Userdescription.IndexOf("."));
                                                //Console.WriteLine("Set cat to " + sh.Category + " for " + sh.Userdescription);
                                            }
                                            catch (Exception cE)
                                            {
                                                Console.WriteLine("Failed to assign category to symbol: " + sh.Userdescription + " err: " + cE.Message);
                                            }
                                        }

                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine(E.Message);
                    }
                }
                if (sh.Varname == "X_AccPedalManSP" || sh.Varname == "X_AccPedalAutTAB" || sh.Varname == "X_AccPedalAutSP" || sh.Varname == "X_AccPedalManTAB" || sh.Userdescription == "X_AccPedalManSP" || sh.Userdescription == "X_AccPedalAutTAB" || sh.Userdescription == "X_AccPedalAutSP" || sh.Userdescription == "X_AccPedalManTAB")
                {
                    if (sh.Length == 4) _faultyAxisForAccPedalDetected = true;
                    //sh.Flash_start_address -= 0x0C;
                    //sh.Length = 0x0C;
                }
            }
            if (_faultyAxisForAccPedalDetected)
            {
                foreach (SymbolHelper sh in m_symbols)
                {
                    if (sh.Varname == "X_AccPedalManSP" || sh.Varname == "X_AccPedalAutTAB" || sh.Varname == "X_AccPedalAutSP" || sh.Varname == "X_AccPedalManTAB" || sh.Userdescription == "X_AccPedalManSP" || sh.Userdescription == "X_AccPedalAutTAB" || sh.Userdescription == "X_AccPedalAutSP" || sh.Userdescription == "X_AccPedalManTAB")
                    {
                        //if (sh.Length == 4) _faultyAxisForAccPedalDetected = true;
                        sh.Flash_start_address -= 0x0C;
                        sh.Length = 0x0C;
                    }
                }
            }
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

        private bool TryToExtractPackedBinary(string filename, int sym_count, int filename_size, out SymbolCollection symbol_collection, int languageID, string m_current_softwareversion)
        {
            // MessageBox.Show("Binary file is packed. Packed files are not (yet) supported, continueing with symbols as numbers!");
            // try to unpack the file!!!
            bool retval = true;
            int symboltableoffset = 0;
            symbol_collection = new SymbolCollection();
            bool compr_created = UnpackFileUsingDecode(filename, filename_size, out symboltableoffset);
            Console.WriteLine("Compr_created: " + compr_created.ToString());
            // UnpackFile();
            /* if (!compr_created)
             {
                 if (progress != null) progress.Close();
                 Application.DoEvents();
                 MessageBox.Show("Unable to open the file, maybe it does not contain a symboltable, or the file is locked.");
                 return;
             }*/
            sym_count = 0;
            //progress.SetProgress("Searching address lookup table");
            CastProgressEvent("Searching address lookup table", 30);

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
                            /*if (fsread.Position > 0x5f900)
                            {
                                Console.WriteLine("Hola");
                            }
                            */
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
                // search next 0xF0
                /*int bcount = 0;
                bool notfound = false;
                while (br.ReadByte() != 0xF0 && bcount < 0x10)
                {
                    bcount++;
                    AddressTableOffset++;
                    if (bcount == 0x0F)
                    {
                        notfound = true;
                        MessageBox.Show("Start of addresstable could not be located");
                    }
                }*/

                if (AddressTableOffset > 0 /*&& !notfound*/)
                {
                    Console.WriteLine("SOT: " + AddressTableOffset.ToString("X6"));
                    fsread.Seek(AddressTableOffset - 17, SeekOrigin.Begin);
                    bool endoftable = false;
                    Int64 internal_address = 0;
                    int sramaddress = 0;
                    int symbollength = 0;
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
                                //DumpBytesToConsole(bytes);
                                if ((Convert.ToInt32(bytes.GetValue(8)) != 0x00) || (Convert.ToInt32(bytes.GetValue(9)) != 0x00))
                                {
                                    endoftable = true;
                                    //MessageBox.Show("EOT: " + fsread.Position.ToString("X6"));
                                    Console.WriteLine("EOT: " + fsread.Position.ToString("X6"));
                                }
                                else
                                {
                                    //DumpBytesToConsole(bytes);

                                    internal_address = Convert.ToInt64(bytes.GetValue(0)) * 256 * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(1)) * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(2));

                                    /* if (bytes[1] == 0x7A && bytes[2] == 0xEE)
                                     {
                                         Console.WriteLine("suspicious");

                                         if (internal_address == 0x7AEE)
                                         {
                                             Console.WriteLine("break: " + fsread.Position.ToString("X8"));
                                         }
                                     }*/
                                    symbollength = Convert.ToInt32(bytes.GetValue(3)) * 256;
                                    symbollength += Convert.ToInt32(bytes.GetValue(4));
                                    if (symbollength > 0 && internal_address == 0x00)
                                    {
                                        // might be damaged addresstable by MapTun.. correct it automatically
                                        if (symbol_collection.Count > 0)
                                        {
                                            internal_address = symbol_collection[symbol_collection.Count - 1].Start_address + symbol_collection[symbol_collection.Count - 1].Length;
                                            if (symbollength == 0x240 && (internal_address % 2) > 0) internal_address++;
                                            Console.WriteLine("Corrected symbol with address: " + internal_address.ToString("X8") + " and len: " + symbollength.ToString("X4"));
                                        }
                                    }
                                    //                                                sramaddress = Convert.ToInt32(bytes.GetValue(7)) * 256 * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(8)) * 256;
                                    //                                                sramaddress += Convert.ToInt32(bytes.GetValue(9));
                                    SymbolHelper sh = new SymbolHelper();
                                    sh.Symbol_number = symb_count;
                                    sh.Symbol_type = Convert.ToInt32(bytes.GetValue(7));
                                    sh.Varname = "Symbolnumber " + symbol_collection.Count.ToString();
                                    sh.Flash_start_address = internal_address;
                                    sh.Start_address = internal_address;
                                    sh.Length = symbollength;
                                    symb_count++;
                                    if (symbollength <= 10000)
                                    {
                                        symbol_collection.Add(sh);
                                        if (symb_count % 500 == 0)
                                        {
                                           // progress.SetProgress(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));
                                            CastProgressEvent(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"), 35);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Length > 10000: " + sh.Varname + " " + sh.Length);
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
                            Console.WriteLine(E.Message);
                            retval = false;
                        }

                    }
                    if (compr_created)
                    {
                        Console.WriteLine("Dumping addresstable", 0);
                        //progress.SetProgress("Dumping addresstable");
                        CastProgressEvent("Dumping addresstable", 40);
                        DumpSymbolAddressTable(symbol_collection, m_current_softwareversion);
                        //progress.SetProgress("Decoding packed symbol table");
                        CastProgressEvent("Decoding packed symbol table", 45);
                        //Console.WriteLine("Decoding packed symbol table");
                        // run decode.exe

                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe");
                        }
                        Console.WriteLine("using: " + Path.GetTempPath());
                        if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR.TXT");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\COMPR"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\table.tmp"))
                        {
                            File.Delete(Path.GetTempPath() + "\\table.tmp");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\T7.decode.exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\T7.decode.exe");
                        }

                        // <GS-18062012> support for x64
                        bool x64 = Detectx64Architecture();
                        if (x64)
                        {
                            mRecreateAllExecutableResources();

                            // rename T7.decode.exe to decode.exe
                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\decode.exe"))
                            {
                                File.Delete(System.Windows.Forms.Application.StartupPath + "\\decode.exe");
                            }
                            File.Move(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe", System.Windows.Forms.Application.StartupPath + "\\decode.exe");

                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\decode.exe"))
                            {
                                if (File.Exists(Path.GetTempPath() + "\\decode.exe"))
                                {
                                    File.Delete(Path.GetTempPath() + "\\decode.exe");
                                }
                                File.Copy(System.Windows.Forms.Application.StartupPath + "\\decode.exe", Path.GetTempPath() + "\\decode.exe");
                            }
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\COMPR", Path.GetTempPath() + "\\COMPR");
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\table.tmp", Path.GetTempPath() + "\\table.tmp");
                            string Exename = Path.Combine(Path.GetTempPath(), "decode.exe");



                            Exename = Path.Combine(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "DOSBox-0.74"), "DOSBox.exe");
                            // write a dosbox.conf first
                            string confFile = Path.Combine(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "DOSBox-0.74"), "t7dosb.conf");
                            if (!File.Exists(confFile))
                            {
                                using (StreamWriter sw = new StreamWriter(confFile, false))
                                {
                                    sw.WriteLine("[autoexec]");
                                    sw.WriteLine("cycles=max");
                                    sw.WriteLine("mount c \"" + Path.GetTempPath() + "\"");
                                    sw.WriteLine("c:");
                                    sw.WriteLine("decode.exe");
                                    sw.WriteLine("exit");
                                }
                            }

                            string argument = "-noconsole -conf t7dosb.conf";
                            ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
                            startinfo.CreateNoWindow = true; // TRUE
                            startinfo.WindowStyle = ProcessWindowStyle.Hidden; // hidden
                            startinfo.Arguments = argument;
                            startinfo.WorkingDirectory = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "DOSBox-0.74");
                            System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                            conv_proc.WaitForExit(20000); 
                            
                            if (!conv_proc.HasExited)
                            {
                                conv_proc.Kill();
                                retval = false;
                            }
                            else
                            {
                                // nu door compr.txt lopen
                                //progress.SetProgress("Adding names to symbols");
                                CastProgressEvent("Adding names to symbols", 50);

                                if (File.Exists(/*Application.StartupPath*/Path.GetTempPath() + "\\COMPR.TXT"))
                                {
                                    //AddNamesToSymbols(symbol_collection);
                                    AddNamesToSymbolsFromTableTmp(symbol_collection, languageID);
                                }
                                //progress.SetProgress("Cleaning up");
                                CastProgressEvent("Cleaning up", 55);
                            }
                        }
                        else
                        {

                            mRecreateAllExecutableResources();
                            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe"))
                            {
                                File.Copy(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe", Path.GetTempPath() + "\\T7.decode.exe");
                                //File.Delete(Application.StartupPath + "\\T7.decode.exe");
                            }
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\COMPR", Path.GetTempPath() + "\\COMPR");
                            File.Copy(System.Windows.Forms.Application.StartupPath + "\\table.tmp", Path.GetTempPath() + "\\table.tmp");
                            string Exename = Path.Combine(Path.GetTempPath(), "T7.decode.exe");

                            //                        string Exename = Path.Combine(Application.StartupPath, "T7.decode.exe");

                            ProcessStartInfo startinfo = new ProcessStartInfo(Exename);
                            startinfo.CreateNoWindow = true;
                            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                            //startinfo.UseShellExecute = false;
                            startinfo.WorkingDirectory = /*Application.StartupPath*/Path.GetTempPath();
                            System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                            conv_proc.WaitForExit(10000); // wait for 10 seconds max
                            if (!conv_proc.HasExited)
                            {
                                conv_proc.Kill();
                                retval = false;
                            }
                            else
                            {
                                // nu door compr.txt lopen
                                //progress.SetProgress("Adding names to symbols");
                                CastProgressEvent("Adding names to symbols", 50);

                                if (File.Exists(/*Application.StartupPath*/Path.GetTempPath() + "\\COMPR.TXT"))
                                {
                                    //AddNamesToSymbols(symbol_collection);
                                    AddNamesToSymbolsFromTableTmp(symbol_collection, languageID);
                                }
                                //progress.SetProgress("Cleaning up");
                                CastProgressEvent("Cleaning up", 55);
                            }
                        }
                        
                        // read the compr.txt file
                        string[] allSymbolNames = File.ReadAllLines(Path.GetTempPath() + "\\COMPR.TXT");
                        foreach (SymbolHelper sh in symbol_collection)
                        {
                            foreach (string symName in allSymbolNames)
                            {
                                if(symName.Length >= 24)
                                {
                                    if (sh.Varname == symName.Substring(0, 24) && sh.Varname != symName)
                                    {
                                        
                                        // only do this if the symbol does not exist yet
                                        if (!CollectionContains(symbol_collection, symName))
                                        {
                                            //Console.WriteLine(sh.Varname + " " + symName);
                                            sh.Varname = symName;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        /*
                        if (File.Exists(Path.GetTempPath() + "\\T7.decode.exe"))
                        {
                            File.Delete(Path.GetTempPath() + "\\T7.decode.exe");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\XTABLE.TMP");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\TABLE.TMP"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\TABLE.TMP");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR.TXT");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\XTABLE.TMP"))
                        {
                            File.Delete(Path.GetTempPath() + "\\XTABLE.TMP");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\TABLE.TMP"))
                        {
                            File.Delete(Path.GetTempPath() + "\\TABLE.TMP");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\COMPR"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR");
                        }*/
                    }
                }
                else
                {
                    /*if (progress != null)
                    {
                        progress.Close();
                        System.Windows.Forms.Application.DoEvents();
                    }*/
                    Console.WriteLine("Could not find address table!"); //<GS-23022010>
                    retval = false;
                }
            }
            return retval;
        }

       

        private bool Detectx64Architecture()
        {
            try
            {
                // if x64, install dosbox if not already done
                if (T7.Wow.Is64BitOperatingSystem)
                {
                    // 64 bit detected... 
                    AppSettings m_appSettings = new AppSettings();
                    if (!m_appSettings.DosBoxInstalled)
                    {
                        //DOSBox0.74-win32-installer.exe /S
                        string fileName = Path.Combine(System.Windows.Forms.Application.StartupPath + "\\Dosbox", "DOSBox0.74-win32-installer.exe");
                        ProcessStartInfo startinfo = new ProcessStartInfo(fileName);
                        startinfo.CreateNoWindow = true;
                        startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startinfo.Arguments = "/S";
                        startinfo.WorkingDirectory = /*Application.StartupPath*/Path.GetTempPath();
                        System.Diagnostics.Process conv_proc = System.Diagnostics.Process.Start(startinfo);
                        conv_proc.WaitForExit();
                        m_appSettings.DosBoxInstalled = true;
                    }
                    return true;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("Failed to install dosbox: " + E.Message);
            }
            return false;
        }

        private bool CollectionContains(SymbolCollection symbol_collection, string symName)
        {
            foreach (SymbolHelper sh in symbol_collection)
            {
                if (sh.Varname == symName) return true;
            }
            return false;
        }

        private void AddNamesToSymbolsFromTableTmp(SymbolCollection symbol_collection, int LanguageID)
        {
            SymbolTranslator translator = new SymbolTranslator();
            string line = string.Empty;
            int flashaddress = 0;
            int length = 0;
            int symb_count = 0;
            char[] sep = new char[1];
            sep.SetValue(' ', 0);

            string addresses = string.Empty;
            string symbol = string.Empty;
            //Console.WriteLine("Fase 1");
            using (StreamReader sr = new StreamReader(Path.Combine(/*Application.StartupPath*/Path.GetTempPath(), "XTABLE.TMP")))
            {
                string totallines = sr.ReadToEnd();
                char[] split = new char[2];
                split.SetValue((byte)0x0D, 0);
                split.SetValue((byte)0x0A, 1);
                string[] lines = totallines.Split(split);
                for (int i = 3; i < lines.Length; i++)
                {

                    line = (string)lines[i];

                    

                    try
                    {
                        if (line.Length > 2)
                        {
                            if (!line.Contains(" "))
                            {
                                // dan is het een symbool
                                symbol = line.Trim();
                                

                            }
                            else
                            {
                                addresses = line;
                                string[] addvalues = addresses.Split(sep);
                                if (addvalues.Length >= 3)
                                {
                                    flashaddress = Convert.ToInt32((string)addvalues.GetValue(0), 16);
                                    length = Convert.ToInt32((string)addvalues.GetValue(1), 16);
                                    foreach (SymbolHelper sh in symbol_collection)
                                    {
                                        if (sh.Flash_start_address == flashaddress && flashaddress != 0 && sh.Varname.StartsWith("Symbolnumber"))
                                        {
                                            sh.Varname = symbol;
                                            //sh.Flash_start_address = flashaddress;
                                            sh.Length = length;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to add symbolnames: " + E.Message);
                    }
                }
                
                
                //Console.WriteLine("Fase 2");
                symb_count = 0; // TEST
                for (int i = 3; i < lines.Length; i++)
                {
                    line = (string)lines[i];
                    
                    try
                    {
                        if (line.Length > 2)
                        {
                            //Console.WriteLine("line: " + line);
                            if (!line.Contains(" "))
                            {
                                // dan is het een symbool
                                symbol = line.Trim();
                                symb_count++;
                                
                            }
                            else
                            {
                                addresses = line;
                                string[] addvalues = addresses.Split(sep);
                                if (addvalues.Length >= 3)
                                {
                                    flashaddress = Convert.ToInt32((string)addvalues.GetValue(0), 16);
                                    //Console.WriteLine("Flash address: " + flashaddress.ToString());
                                    length = Convert.ToInt32((string)addvalues.GetValue(1), 16);
                                    foreach (SymbolHelper sh in symbol_collection)
                                    {
                                        if (sh.Symbol_number == symb_count -1)
                                        {
                                            if (sh.Varname.StartsWith("Symbolnumber"))
                                            {
                                                sh.Varname = symbol;
                                                sh.Length = length;
                                                if (sh.Flash_start_address == 0)
                                                {
                                                    sh.Flash_start_address = flashaddress;
                                                    sh.Start_address = flashaddress;
                                                }
                                                //Console.WriteLine("Set symbolnumber: " + sh.Symbol_number.ToString() + " to be " + symbol);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Failed to add symbolnames: " + E.Message);
                    }
                }

            }
            
            //Console.WriteLine("Fase 3");
            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in symbol_collection)
            {
                sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, LanguageID);
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        Console.WriteLine("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
            }
            // remove Type identifiers from the symbolcollection
            /*bool TypeFound = true;
            while (TypeFound)
            {
                foreach (SymbolHelper sh in symbol_collection)
                {
                    TypeFound = false;
                    if (sh.Flash_start_address == 0)
                    {
                        symbol_collection.Remove(sh);
                        TypeFound = true;
                        break;
                    }

                }
            }*/
        }

        //======================================================
        //Recreate all executable resources
        //======================================================
        private void mRecreateAllExecutableResources()
        {
            // Get Current Assembly refrence
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all imbedded resources
            string[] arrResources = currentAssembly.GetManifestResourceNames();

            foreach (string resourceName in arrResources)
            {
                if (resourceName.EndsWith(".exe"))
                { //or other extension desired
                    //Name of the file saved on disk
                    string saveAsName = resourceName;
                    FileInfo fileInfoOutputFile = new FileInfo(System.Windows.Forms.Application.StartupPath + "\\" + saveAsName);
                    //CHECK IF FILE EXISTS AND DO SOMETHING DEPENDING ON YOUR NEEDS
                    if (fileInfoOutputFile.Exists)
                    {
                        //overwrite if desired  (depending on your needs)
                        //fileInfoOutputFile.Delete();
                    }
                    //OPEN NEWLY CREATING FILE FOR WRITTING
                    FileStream streamToOutputFile = fileInfoOutputFile.OpenWrite();
                    //GET THE STREAM TO THE RESOURCES
                    Stream streamToResourceFile =
                                        currentAssembly.GetManifestResourceStream(resourceName);

                    //---------------------------------
                    //SAVE TO DISK OPERATION
                    //---------------------------------
                    const int size = 4096;
                    byte[] bytes = new byte[4096];
                    int numBytes;
                    while ((numBytes = streamToResourceFile.Read(bytes, 0, size)) > 0)
                    {
                        streamToOutputFile.Write(bytes, 0, numBytes);
                    }

                    streamToOutputFile.Close();
                    streamToResourceFile.Close();
                }//end_if

            }//end_foreach
        }//end_mRecreateAllExecutableResources 

        private void DumpSymbolAddressTable(SymbolCollection symbol_collection, string m_current_softwareversion)
        {
            // export to table.tmp
            //4538 EB0C8P1C.56O F5DB0074
            //05909C 68F5 04 

            using (StreamWriter sw = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\table.tmp", false))
            {

                sw.WriteLine(symbol_collection.Count.ToString() + " " + m_current_softwareversion + " 00000000");
                sw.WriteLine("");
                sw.WriteLine("");
                foreach (SymbolHelper sh in symbol_collection)
                {
                    if (sh.Length <= 10000)
                    {
                        sw.WriteLine(sh.Flash_start_address.ToString("X6") + " " + sh.Length.ToString("X4") + " " + sh.Symbol_type.ToString("X2"));
                    }
                    else
                    {
                        Console.WriteLine("Length > 10000: " + sh.Varname + " " + sh.Length);
                    }
                }
            }
        }
    }
}
