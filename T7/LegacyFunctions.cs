using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using CommonSuite;

namespace T7
{
    public partial class frmMain : Form
    {
        private int GetSymbolListOffSet(string m_currentfile)
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
                while ((fsread.Position < m_currentfile_size) && retval == 0)
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

       
        private int ReadEndMarker(int value)
        {
            int retval = 0;
            if (m_currentfile != string.Empty)
            {
                if (File.Exists(m_currentfile))
                {
                    // read the file footer
                    //3ff00 - 0x3ffff
                    FileStream fs = new FileStream(m_currentfile, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileoffset = m_currentfile_size - 0x100;
                    fs.Seek(/*0x3FF00*/fileoffset, SeekOrigin.Begin);
                    byte[] inb = br.ReadBytes(0xFF);
                    int offset = 0;
                    for (int t = 0; t < 0xFF; t++)
                    {
                        if ((byte)inb.GetValue(t) == (byte)value)
                        {
                            // marker gevonden
                            // lees 6 terug
                            offset = t;
                            break;
                        }
                    }
                    string hexstr = string.Empty;
                    if (offset > 6)
                    {
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 1));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 2));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 3));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 4));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 5));
                        hexstr += Convert.ToChar((byte)inb.GetValue(offset - 6));
                    }
                    try
                    {
                        retval = Convert.ToInt32(hexstr, 16);
                        if (m_currentfile_size == 0x40000)
                        {
                            retval -= m_currentfile_size;
                        }
                        else
                        {
                            retval -= 0x60000;
                        }
                    }
                    catch (Exception E)
                    {
                        LogHelper.Log(E.Message);
                    }
                    fs.Flush();
                    br.Close();
                    fs.Close();
                    fs.Dispose();
                }
            }
            return retval;
        }

        private void UnpackFile()
        {
            /*int len = 0;
            int val = 0;
            //int idx = ReadEndMarker(0x9B);
            int idx = ReadMarkerAddressContent(m_currentfile, 0x9B, out len, out val);
            if (idx > 0)
            {
                // MessageBox.Show("Packed table index: " + idx.ToString("X6") + " " + val.ToString("X6"));

                FileStream fsread = new FileStream(m_currentfile, FileMode.Open);
                using (BinaryReader br = new BinaryReader(fsread))
                {
                    fsread.Seek(val, SeekOrigin.Begin);
                    br.ReadByte(); // dummy
                    br.ReadByte(); // dummy
                    int unpacked_length = 0;
                    byte b = br.ReadByte();
                    unpacked_length = Convert.ToInt32(b);
                    b = br.ReadByte();
                    unpacked_length += Convert.ToInt32(b) * 256;
                    b = br.ReadByte();
                    unpacked_length += Convert.ToInt32(b) * 256 * 256;
                    b = br.ReadByte();
                    unpacked_length += Convert.ToInt32(b) * 256 * 256 * 256;
                    //MessageBox.Show("Unpacked table size: " + unpacked_length.ToString());
                    // save the bytes to a tempfile

                    // now... how many bytes ???
                    for (int length = 26024; length <= 26048; length++)
                    {
                        fsread.Seek(val, SeekOrigin.Begin);
                        br.ReadByte(); // dummy
                        br.ReadByte(); // dummy

                        br.ReadByte(); // dummy
                        br.ReadByte(); // dummy
                        br.ReadByte(); // dummy
                        br.ReadByte(); // dummy

                        // 26024 - 26048 
                        LZSSDecompressor decompr = new LZSSDecompressor();
                        byte[] decompressed = decompr.Decompress(br.ReadBytes(length), out len);
                        if (len == unpacked_length)
                        {
                            // success!
                            MessageBox.Show("Success at length: " + len.ToString());
                        }
                        else
                        {
                            if (File.Exists(length.ToString() + ".txt")) File.Delete(length.ToString() + ".txt");
                            FileStream teststream = new FileStream(length.ToString() + ".txt", FileMode.CreateNew);
                            BinaryWriter bw = new BinaryWriter(teststream);
                            bw.Write(decompressed);
                            bw.Close();
                            teststream.Close();
                            teststream.Dispose();
                        }
                    }

                }
                fsread.Close();
                fsread.Dispose();
            }*/

        }

        private bool UnpackFileUsingDecode(string filename, out int symboltableoffset)
        {
            symboltableoffset = 0;
            int len = 0;
            int val = 0;
            //int idx = ReadEndMarker(0x9B);
            try
            {
                int idx = ReadMarkerAddressContent(filename, 0x9B, out len, out val);
                if (idx > 0)
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
                LogHelper.Log(E.Message);
            }
            return false;
        }
        private void DumpBytesToConsole(byte[] bytes)
        {
            string line = "symbol bytes: ";
            foreach (byte b in bytes)
            {
                line += b.ToString("X2") + " ";
            }
            LogHelper.Log(line);
        }
        private void DumpSymbolAddressTable(SymbolCollection symbol_collection)
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
                    if (sh.Length < 0x1000)
                    {
                        sw.WriteLine(sh.Flash_start_address.ToString("X6") + " " + sh.Length.ToString("X4") + " " + sh.Symbol_type.ToString("X2"));
                    }
                }
            }
        }
        private void AddNamesToSymbols(SymbolCollection symbol_collection)
        {
            SymbolTranslator translator = new SymbolTranslator();
            using (StreamReader sr = new StreamReader(Path.Combine(Path.GetTempPath(), "COMPR.TXT")))
            {
                //                sr.ReadLine(); // dummy
                //sr.ReadLine(); // dummy
                foreach (SymbolHelper sh in symbol_collection)
                {
                    try
                    {
                        sh.Varname = sr.ReadLine();
                        // LogHelper.Log(sh.Varname);
                        /*if (sh.Varname == "TorqueCal.M_IgnInflTorqMap")
                        {
                            LogHelper.Log("break!");
                        }*/
                    }
                    catch (Exception snaE)
                    {
                        LogHelper.Log("Failed to attach name to symbol: :" + snaE.Message);
                    }
                }
            }
            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in symbol_collection)
            {
                sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, m_appSettings.ApplicationLanguage);
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        LogHelper.Log("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
            }
        }
        private void AddNamesToSymbolsFromTableTmp(SymbolCollection symbol_collection)
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
            LogHelper.Log("Fase 1");
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
                        LogHelper.Log("Failed to add symbolnames: " + E.Message);
                    }
                }
                LogHelper.Log("Fase 2");
                for (int i = 3; i < lines.Length; i++)
                {
                    line = (string)lines[i];
                    try
                    {
                        if (line.Length > 2)
                        {
                            //LogHelper.Log("line: " + line);
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
                                    //LogHelper.Log("Flash address: " + flashaddress.ToString());
                                    length = Convert.ToInt32((string)addvalues.GetValue(1), 16);
                                    foreach (SymbolHelper sh in symbol_collection)
                                    {
                                        if (sh.Symbol_number == symb_count)
                                        {
                                            if (sh.Varname.StartsWith("Symbolnumber"))
                                            {
                                                sh.Varname = symbol;
                                                sh.Length = length;
                                                //LogHelper.Log("Set symbolnumber: " + sh.Symbol_number.ToString() + " to be " + symbol);
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
                        LogHelper.Log("Failed to add symbolnames: " + E.Message);
                    }
                }

            }
            LogHelper.Log("Fase 3");
            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            foreach (SymbolHelper sh in symbol_collection)
            {
                sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, m_appSettings.ApplicationLanguage);
                if (sh.Varname.Contains("."))
                {
                    try
                    {
                        sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                    }
                    catch (Exception cE)
                    {
                        LogHelper.Log("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                    }
                }
            }
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
        private bool TryToExtractPackedBinary(string filename, frmProgress progress, int sym_count, int filename_size, out SymbolCollection symbol_collection)
        {
            // MessageBox.Show("Binary file is packed. Packed files are not (yet) supported, continueing with symbols as numbers!");
            // try to unpack the file!!!
            bool retval = true;
            int symboltableoffset = 0;
            symbol_collection = new SymbolCollection();
            bool compr_created = UnpackFileUsingDecode(filename, out symboltableoffset);
            // UnpackFile();
            /* if (!compr_created)
             {
                 if (progress != null) progress.Close();
                 Application.DoEvents();
                 MessageBox.Show("Unable to open the file, maybe it does not contain a symboltable, or the file is locked.");
                 return;
             }*/
            sym_count = 0;
            progress.SetProgress("Searching address lookup table");

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
                                LogHelper.Log("Hola");
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
                    AddDebugLog("Opening file resulted in AddressTableOffset (packed): " + AddressTableOffset.ToString("X8"));

                    fsread.Seek(/*0x588f0*/ AddressTableOffset - 17, SeekOrigin.Begin);
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
                                    LogHelper.Log("EOT: " + fsread.Position.ToString("X6"));
                                }
                                else
                                {
                                    //DumpBytesToConsole(bytes);

                                    internal_address = Convert.ToInt64(bytes.GetValue(0)) * 256 * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(1)) * 256;
                                    internal_address += Convert.ToInt64(bytes.GetValue(2));

                                    /* if (bytes[1] == 0x7A && bytes[2] == 0xEE)
                                     {
                                         LogHelper.Log("suspicious");

                                         if (internal_address == 0x7AEE)
                                         {
                                             LogHelper.Log("break: " + fsread.Position.ToString("X8"));
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
                                            LogHelper.Log("Corrected symbol with address: " + internal_address.ToString("X8") + " and len: " + symbollength.ToString("X4"));
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
                                    if (symbollength < 0x1000 /*&& symbollength > 0*/)
                                    {
                                        symbol_collection.Add(sh);
                                        if (symb_count % 500 == 0)
                                        {
                                            progress.SetProgress(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));
                                        }
                                    }
                                    else
                                    {
                                        LogHelper.Log("Length > 0x1000: " + sh.Varname);
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
                            LogHelper.Log(E.Message);
                            retval = false;
                        }

                    }
                    if (compr_created)
                    {
                        progress.SetProgress("Dumping addresstable");
                        DumpSymbolAddressTable(symbol_collection);
                        progress.SetProgress("Decoding packed symbol table");
                        // run decode.exe

                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\COMPR.TXT");
                        }
                        if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\T7.decode.exe");
                        }
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
                        conv_proc.WaitForExit(30000); // wait for 30 seconds max
                        if (!conv_proc.HasExited)
                        {
                            conv_proc.Kill();
                            retval = false;
                        }
                        else
                        {
                            // nu door compr.txt lopen
                            progress.SetProgress("Adding names to symbols");

                            if (File.Exists(/*Application.StartupPath*/Path.GetTempPath() + "\\COMPR.TXT"))
                            {
                                //AddNamesToSymbols(symbol_collection);
                                AddNamesToSymbolsFromTableTmp(symbol_collection);
                            }
                            progress.SetProgress("Cleaning up");
                        }
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
                        if (File.Exists(Path.GetTempPath() + "\\COMPR.TXT"))
                        {
                            File.Delete(Path.GetTempPath() + "\\COMPR.TXT");
                        }
                        if (File.Exists(Path.GetTempPath() + "\\XTABLE.TMP"))
                        {
                            File.Delete(Path.GetTempPath() + "\\XTABLE.TMP");
                        }
                    }
                }
                else
                {
                    if (progress != null)
                    {
                        progress.Close();
                        System.Windows.Forms.Application.DoEvents();
                    }
                    MessageBox.Show("Could not find address table!");
                    retval = false;
                }
            }
            return retval;
        }
        private void TryToOpenFile(/*string filename, out SymbolCollection symbol_collection, int filename_size*/)
        {
            /*
            SymbolTranslator translator = new SymbolTranslator();
            string help = string.Empty;
            XDFCategories category = XDFCategories.Undocumented;
            XDFSubCategory subcat = XDFSubCategory.Undocumented;
            FileInfo fi = new FileInfo(filename);
            fi.IsReadOnly = false;

            try
            {
                T7FileHeader t7InfoHeader = new T7FileHeader();
                if (t7InfoHeader.init(filename, m_appSettings.AutoFixFooter))
                {
                    m_current_softwareversion = t7InfoHeader.getSoftwareVersion();
                }
                else
                {
                    m_current_softwareversion = "";
                }
            }
            catch (Exception E2)
            {
                LogHelper.Log(E2.Message);
            }


            symbol_collection = new SymbolCollection();
            try
            {
                int sym_count = 0; // altered
                frmProgress progress = new frmProgress();
                progress.Show();
                progress.SetProgress("Opening file");
                symbol_collection = new SymbolCollection();
                if (filename != string.Empty)
                {
                    if (File.Exists(filename))
                    {
                        AddFileToMRUList(filename);
                        if (!IsBinaryPackedVersion(filename))
                        {
                            progress.SetProgress("Getting symbol list offset");

                            int SymbolListOffSet = GetSymbolListOffSet(filename);
                            if (SymbolListOffSet > 0)
                            {
                                AddDebugLog("Opening file resulted in SymbolListOffset: " + SymbolListOffSet.ToString("X8"));
                                FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
                                using (BinaryReader br = new BinaryReader(fsread))
                                {
                                    fsread.Seek(SymbolListOffSet, SeekOrigin.Begin);     // 0x15F9 <SymbolListOffSet>
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
                                                    
                                                    sh.Description = translator.TranslateSymbolToHelpText(sh.Varname, out help, out category, out subcat, m_appSettings.ApplicationLanguage);
                                                    if (sh.Varname.Contains("."))
                                                    {
                                                        try
                                                        {
                                                            sh.Category = sh.Varname.Substring(0, sh.Varname.IndexOf("."));
                                                        }
                                                        catch (Exception cE)
                                                        {
                                                            LogHelper.Log("Failed to assign category to symbol: " + sh.Varname + " err: " + cE.Message);
                                                        }
                                                    }
                                                    sh.Internal_address = internal_address - 1;
                                                    sh.Symbol_number = sym_count;
                                                    //if (sh.Varname == "IgnNormCal.Map")
                                                    //{
                                                    //    LogHelper.Log("IgnNormCal.Map: " + sh.Internal_address.ToString("X4"));
                                                    //}
                                                    symbol_collection.Add(sh);
                                                    symbolname = "";
                                                    internal_address = 0;
                                                    sym_count++;
                                                    if ((sym_count % 100) == 0)
                                                    {
                                                        progress.SetProgress("Symbol: " + sh.Varname);
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
                                        progress.SetProgress("Searching address lookup table");

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
                                            AddDebugLog("Opening file resulted in AddressTableOffset: " + AddressTableOffset.ToString("X8"));

                                            fsread.Seek(AddressTableOffset-8, SeekOrigin.Begin);        // 0x588f0 <AddressTableOffset-8>
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
                                                            if (symbollength == 1 || symbollength == 2 || symbollength == 4)
                                                            {
                                                                //LogHelper.Log("Symbol address: " + internal_address.ToString("X8") + " sram: " + sramaddress.ToString("X8") + " ind " + indicator.ToString("X2") + " len " + symbollength.ToString());
                                                            }
                                                            
                                                            //if (sramaddress < 0xF00000) LogHelper.Log("Flash Indicator = " + indicator.ToString("X2"));
                                                            //else if (sramaddress >= 0xF00000) LogHelper.Log("SRAM Indicator = " + indicator.ToString("X2"));
                                                            
                                                           
                                                            int realromaddress = 0;
                                                            
                                                            if (sramaddress > 0xF00000) realromaddress = sramaddress - 0xef02f0;


                                                            foreach (SymbolHelper sh in symbol_collection)
                                                            {
                                                                if (sh.Internal_address == internal_address)
                                                                {
                                                                    //DumpToDebugTable(bytes, sh);
                                                                    if (realromaddress > 0)
                                                                    {
                                                                        //LogHelper.Log(sh.Varname + " " + sh.Internal_address.ToString("X6") + " " + sramaddress.ToString("X6") + " " +realromaddress.ToString("X6") + " " + symbollength.ToString("X4"));
                                                                        if (sramaddress > highestsramaddress) highestsramaddress = sramaddress;
                                                                    }

                                                                    if (sramaddress > 0 && sh.Flash_start_address == 0)
                                                                    //if (sh.Symbol_number==sym_count)
                                                                    {
                                                                        if (sh.Varname == "BFuelCal.Map")
                                                                        {
                                                                            LogHelper.Log("BfuelCal.map: " + sh.Symbol_number.ToString() + " index in address lookup: " + sym_count.ToString() + " " + sramaddress.ToString("X8"));
                                                                        }
                                                                        sh.Start_address = sramaddress; // TEST
                                                                        //if (internal_address == 0xE71A)
                                                                        //{
                                                                        //    LogHelper.Log("IgnNormCal.Map: " + sh.Symbol_number.ToString() + " index in address lookup: " + sym_count.ToString());
                                                                        //}
                                                                        sh.Flash_start_address = sramaddress;
                                                                        //sh.Flash_start_address = realromaddress;
                                                                        //TODO: VERIFY TEST WITH OTHER BINS
                                                                        if (realromaddress > 0 && sh.Varname.Contains(".")) sh.Flash_start_address = realromaddress;
                                                                        sh.Length = symbollength;
                                                                        sym_count++;
                                                                        if ((sym_count % 500) == 0)
                                                                        {
                                                                            progress.SetProgress(sh.Varname + " : " + sh.Flash_start_address.ToString("X6"));
                                                                        }
                                                                        break;
                                                                    }
                                                                    //break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception E)
                                                    {
                                                        LogHelper.Log(E.Message);
                                                    }
                                                }
                                                //LogHelper.Log("Highest SRAM: " + highestsramaddress.ToString("X6"));
                                                //foreach (SymbolHelper sh in symbol_collection)
                                                //{
                                                //    LogHelper.Log(sh.Varname + " at " + sh.Flash_start_address.ToString("X6"));
                                                //}
                                            }
                                        }
                                        else
                                        {
                                            //if (progress != null)
                                            //{
                                            //    progress.Close();
                                            //    Application.DoEvents();
                                            //}
                                            fsread.Close();
                                            if (!TryToExtractPackedBinary(filename, progress, sym_count, filename_size,out symbol_collection))
                                            {
                                                MessageBox.Show("Could not find address table!");
                                            }
                                        }
                                    }
                                }
                                fsread.Close();
                                fsread.Dispose();
                            }

                            else
                            {
                                if (progress != null)
                                {
                                    progress.Close();
                                }
                                System.Windows.Forms.Application.DoEvents();
                                MessageBox.Show("Couldn't find symboltable, file is probably packed!");
                            }
                        }
                        else
                        {
                            TryToExtractPackedBinary(filename, progress, sym_count, filename_size, out symbol_collection);
                        }


                        // try to load additional symboltranslations that the user entered
                        TryToLoadAdditionalSymbols(m_currentfile, true);
                    }
                }
                progress.Close();
            }
            catch (Exception E)
            {
                LogHelper.Log("TryOpenFile filed: " + filename + " err: " + E.Message);
                
            }
        */  
        }

        private void TuneToStage_OLD(int stage, double maxairmass, double maxtorque, EngineType enginetype)
        {
            frmProgress progress = new frmProgress();
            progress.Show();
            progress.SetProgress("Checking current configuration...");
            resumeTuning = new System.Data.DataTable();
            resumeTuning.Columns.Add("Description");
            AddToResumeTable("Tuning your binary to stage: " + stage.ToString());

            progress.SetProgress("Creating backup file...");
            File.Copy(m_currentfile, Path.GetDirectoryName(m_currentfile) + "\\" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin", true);
            AddToResumeTable("Backup file created (" + Path.GetFileNameWithoutExtension(m_currentfile) + DateTime.Now.ToString("yyyyMMddHHmmss") + "beforetuningtostage" + stage.ToString() + ".bin" + ")");

            // tune maps

            /********** BoostCal.RegMap ***********/
            if ((int)GetSymbolAddress(m_symbols, "BoostCal.RegMap") > 0)
            {
                byte[] boostcalmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BoostCal.RegMap"), GetSymbolLength(m_symbols, "BoostCal.RegMap"));
                // up the upper right quadrant > 800 mg/c and > 2000 rpm
                // start off with factor 1.3 for stage I
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BoostCal.RegMap", out cols, out rows);
                if (isSixteenBitTable("BoostCal.RegMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = /*5*/0; rt < rows; rt++)
                {
                    for (int ct = /*4*/0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        //LogHelper.Log("Offset 1: " + offset1.ToString() + " offset2: "+ offset2.ToString());
                        //LogHelper.Log("row = " + rt.ToString() + " col = " + ct.ToString() + " value1: " + boostcalmap[offset1].ToString("X2") + " value2: " + boostcalmap[offset2].ToString("X2"));

                        int boostcalvalue = Convert.ToInt32(boostcalmap[offset1]) * 256 + Convert.ToInt32(boostcalmap[offset2]);
                        //boostcalvalue *= 13;
                        //boostcalvalue /= 10;

                        //minimal value = col * 100
                        if (boostcalvalue < ct * 100) boostcalvalue = ct * 100;

                        double correctionrpm = 1;
                        correctionrpm += rt * 0.0375;
                        if (enginetype == EngineType.B205 || enginetype == EngineType.B205E || enginetype == EngineType.B235 || enginetype == EngineType.B235E)
                        {
                            if (correctionrpm > 1.3) correctionrpm = 1.3;
                        }
                        else
                        {
                            if (correctionrpm > 1.1) correctionrpm = 1.1;
                        }
                        correctionrpm *= boostcalvalue;
                        boostcalvalue = Convert.ToInt32(correctionrpm);
                        if (boostcalvalue > 950) boostcalvalue = 950;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        boostcalmap[offset1] = b1;
                        boostcalmap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BoostCal.RegMap"), GetSymbolLength(m_symbols, "BoostCal.RegMap"), boostcalmap, m_currentfile, true);
                AddToResumeTable("Tuned boost calibration map (BoostCal.RegMap)");
            }
            UpdateChecksum(m_currentfile);
            /********** BstKnkCal.MaxAirmass ***********/
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass") > 0)
            {
                byte[] maxairmasstab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"));
                // up the upper right quadrant > 800 mg/c and > 2000 rpm
                // start off with factor 1.3 for stage I
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BstKnkCal.MaxAirmass", out cols, out rows);
                if (isSixteenBitTable("BstKnkCal.MaxAirmass")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(maxairmasstab[offset1]) * 256 + Convert.ToInt32(maxairmasstab[offset2]);
                        // multiply by 1.3
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        maxairmasstab[offset1] = b1;
                        maxairmasstab[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmass"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmass"), maxairmasstab, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for manual transmission (BstKnkCal.MaxAirmass)");
            }
            UpdateChecksum(m_currentfile);
            if ((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu") > 0)
            {
                /********** BstKnkCal.MaxAirmassAu ***********/
                byte[] maxairmassaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"));
                // up the upper right quadrant > 800 mg/c and > 2000 rpm
                // start off with factor 1.3 for stage I
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "BstKnkCal.MaxAirmassAu", out cols, out rows);
                if (isSixteenBitTable("BstKnkCal.MaxAirmassAu")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(maxairmassaut[offset1]) * 256 + Convert.ToInt32(maxairmassaut[offset2]);
                        // multiply by 1.3
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        maxairmassaut[offset1] = b1;
                        maxairmassaut[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "BstKnkCal.MaxAirmassAu"), GetSymbolLength(m_symbols, "BstKnkCal.MaxAirmassAu"), maxairmassaut, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter for automatic transmission (BstKnkCal.MaxAirmassAu)");
            }
            UpdateChecksum(m_currentfile);
            /********** FCutCal.m_AirInletLimit ***********/

            // write 1450 to fuelcut limit
            if ((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit") > 0)
            {
                byte[] fuelcutmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"));
                if (fuelcutmap.Length == 2)
                {
                    fuelcutmap[0] = 0x05;
                    fuelcutmap[1] = 0xAA;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "FCutCal.m_AirInletLimit"), GetSymbolLength(m_symbols, "FCutCal.m_AirInletLimit"), fuelcutmap, m_currentfile, true);
                AddToResumeTable("Tuned fuelcut limiter (FCutCal.m_AirInletLimit)");
            }
            UpdateChecksum(m_currentfile);
            /********** PedalMapCal.m_RequestMap ***********/
            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap") > 0)
            {
                byte[] pedalmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.m_RequestMap"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "PedalMapCal.m_RequestMap", out cols, out rows);
                if (isSixteenBitTable("PedalMapCal.m_RequestMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(pedalmap[offset1]) * 256 + Convert.ToInt32(pedalmap[offset2]);
                        if (rt == rows - 3)
                        {
                            boostcalvalue *= 11;
                            boostcalvalue /= 10;
                        }
                        else if (rt == rows - 2)
                        {
                            boostcalvalue *= 12;
                            boostcalvalue /= 10;
                        }
                        else
                        {
                            boostcalvalue *= 13;
                            boostcalvalue /= 10;
                        }
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        pedalmap[offset1] = b1;
                        pedalmap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.m_RequestMap"), GetSymbolLength(m_symbols, "PedalMapCal.m_RequestMap"), pedalmap, m_currentfile, true);
                AddToResumeTable("Tuned airmass request map (PedalMapCal.m_RequestMap)");
            }
            UpdateChecksum(m_currentfile);
            /********** TorqueCal.m_PedYSP ***********/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP") > 0)
            {
                byte[] pedalmapysp = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"));
                // up the highest three rows with respectively 1.1, 1.2 and 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_PedYSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_PedYSP")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = rows - 3; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(pedalmapysp[offset1]) * 256 + Convert.ToInt32(pedalmapysp[offset2]);
                    if (rt == rows - 3)
                    {
                        boostcalvalue *= 11;
                        boostcalvalue /= 10;
                    }
                    else if (rt == rows - 2)
                    {
                        boostcalvalue *= 12;
                        boostcalvalue /= 10;
                    }
                    else
                    {
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                    }
                    if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    pedalmapysp[offset1] = b1;
                    pedalmapysp[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_PedYSP"), GetSymbolLength(m_symbols, "TorqueCal.m_PedYSP"), pedalmapysp, m_currentfile, true);
                AddToResumeTable("Tuned airmass pedalmap y axis (TorqueCal.m_PedYSP)");
            }
            UpdateChecksum(m_currentfile);
            /********** TorqueCal.M_EngMaxAutTab ***********/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab") > 0)
            {
                byte[] maxtorqueaut = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxAutTab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxAutTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxAutTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorqueaut[offset1]) * 256 + Convert.ToInt32(maxtorqueaut[offset2]);
                    boostcalvalue *= 13;
                    boostcalvalue /= 10;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorqueaut[offset1] = b1;
                    maxtorqueaut[offset2] = b2;

                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxAutTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxAutTab"), maxtorqueaut, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for automatic transmission (TorqueCal.M_EngMaxAutTab)");
            }
            UpdateChecksum(m_currentfile);
            /********** TorqueCal.M_EngMaxTab ***********/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab") > 0)
            {
                byte[] maxtorquetab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxTab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquetab[offset1]) * 256 + Convert.ToInt32(maxtorquetab[offset2]);
                    boostcalvalue *= 13;
                    boostcalvalue /= 10;
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxTab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxTab"), maxtorquetab, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission (TorqueCal.M_EngMaxTab)");
            }
            UpdateChecksum(m_currentfile);
            /********** TorqueCal.M_EndMaxE85Tab ***********/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab") > 0)
            {
                byte[] maxtorquetab = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxE85Tab"));
                // up with 1.3 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngMaxE85Tab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngMaxE85Tab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquetab[offset1]) * 256 + Convert.ToInt32(maxtorquetab[offset2]);
                    boostcalvalue *= 13;
                    boostcalvalue /= 10;
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquetab[offset1] = b1;
                    maxtorquetab[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngMaxE85Tab"), GetSymbolLength(m_symbols, "TorqueCal.M_EngMaxE85Tab"), maxtorquetab, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission on E85 fuel (TorqueCal.M_EngMaxE85Tab)");
            }
            UpdateChecksum(m_currentfile);
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_ManGearLim"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_ManGearLim", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_ManGearLim")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    boostcalvalue *= 14;
                    boostcalvalue /= 10;
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_ManGearLim"), GetSymbolLength(m_symbols, "TorqueCal.M_ManGearLim"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission gears (TorqueCal.M_ManGearLim)");
            }
            UpdateChecksum(m_currentfile);
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab") > 0)
            {
                //TorqueCal.M_ManGearLim
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(m_symbols, "TorqueCal.M_5GearLimTab"));
                // up with 1.4 factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_5GearLimTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_5GearLimTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    boostcalvalue *= 14;
                    boostcalvalue /= 10;
                    if (boostcalvalue > maxtorque) boostcalvalue = (int)maxtorque;

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_5GearLimTab"), GetSymbolLength(m_symbols, "TorqueCal.M_5GearLimTab"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque limiter for manual transmission in 5th gear (TorqueCal.M_5GearLimTab)");
            }
            UpdateChecksum(m_currentfile);
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_OverBoostTab") > 0)
            {
                //TorqueCal.M_OverBoostTab
                byte[] maxtorquemangear = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_OverBoostTab"), GetSymbolLength(m_symbols, "TorqueCal.M_OverBoostTab"));
                // up with 1.3factor
                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_OverBoostTab", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_OverBoostTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(maxtorquemangear[offset1]) * 256 + Convert.ToInt32(maxtorquemangear[offset2]);
                    boostcalvalue *= 13;
                    boostcalvalue /= 10;
                    if (boostcalvalue > (maxtorque * 1.1)) boostcalvalue = (int)(maxtorque * 1.1);

                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    maxtorquemangear[offset1] = b1;
                    maxtorquemangear[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_OverBoostTab"), GetSymbolLength(m_symbols, "TorqueCal.M_OverBoostTab"), maxtorquemangear, m_currentfile, true);
                AddToResumeTable("Tuned torque overboost table (TorqueCal.M_OverBoostTab)");
            }
            UpdateChecksum(m_currentfile);
            //additional maps
            /*** BoostCal.MaxOffAdap ****/
            /*** Limit for how much m_AirInlet allowed to been over over actual set value for high adaption. Resolution is 1 mg/c ***/
            // should be increased by 1.6 factor
            if ((int)GetSymbolAddress(m_symbols, "BoostCal.MaxOffAdap") > 0)
            {
                byte[] maxoffadap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "BoostCal.MaxOffAdap"), GetSymbolLength(m_symbols, "BoostCal.MaxOffAdap"));
                if (maxoffadap.Length == 2)
                {
                    int value = Convert.ToInt32(maxoffadap[0]) * 256;
                    value += Convert.ToInt32(maxoffadap[1]);

                    value *= 16;
                    value /= 10;
                    byte b1 = Convert.ToByte(value / 256);
                    byte b2 = Convert.ToByte(value - (int)b1 * 256);
                    maxoffadap[0] = b1;
                    maxoffadap[1] = b2;
                    savedatatobinary((int)GetSymbolAddress(m_symbols, "BoostCal.MaxOffAdap"), GetSymbolLength(m_symbols, "BoostCal.MaxOffAdap"), maxoffadap, m_currentfile, true);
                    AddToResumeTable("Tuned BoostCal.MaxOffAdap");
                }
            }
            UpdateChecksum(m_currentfile);
            /*** PedalMapCal.X_AutFacTab ***/
            /*** The pedal position pointer in the pedal map will be multiplied with this factor before the interpolation 
            is done to calculate m_Driver. The factor is related to the vehicle speed. 
            Used in normal mode (no sport or economi mode), for automatic gearbox when vehicle speed signal is OK. Resolution is 0.001 ***/
            // increase every step with 80!!! 

            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.X_AutFacTab") > 0)
            {
                byte[] autfacmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.X_AutFacTab"), GetSymbolLength(m_symbols, "PedalMapCal.X_AutFacTab"));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "PedalMapCal.X_AutFacTab", out cols, out rows);
                if (isSixteenBitTable("PedalMapCal.X_AutFacTab")) rows /= 2;
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(autfacmap[offset1]) * 256 + Convert.ToInt32(autfacmap[offset2]);
                    boostcalvalue += (rt * 80);
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    autfacmap[offset1] = b1;
                    autfacmap[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.X_AutFacTab"), GetSymbolLength(m_symbols, "PedalMapCal.X_AutFacTab"), autfacmap, m_currentfile, true);
                AddToResumeTable("Tuned pedal factor map for automatic transmission (PedalMapCal.X_AutFacTab)");
            }
            UpdateChecksum(m_currentfile);
            /*** PedelMapCal.X_ManFacTab ***/
            /*** The pedal position pointer in the pedal map will be multiplied with this factor before the interpolation 
            is done to calculate m_Driver. The factor is related to the vehicle speed. 
            Used in normal mode (no sport or economi mode), for manual gearbox when vehicle speed signal is OK. Resolution is 0.001 ***/
            // increase every step with 80!!! 

            if ((int)GetSymbolAddress(m_symbols, "PedalMapCal.X_ManFacTab") > 0)
            {
                byte[] autfacmap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "PedalMapCal.X_ManFacTab"), GetSymbolLength(m_symbols, "PedalMapCal.X_ManFacTab"));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "PedalMapCal.X_ManFacTab", out cols, out rows);
                if (isSixteenBitTable("PedalMapCal.X_ManFacTab")) rows /= 2;
                for (int rt = 0; rt < rows; rt++)
                {
                    int offset1 = (rt * cols * 2);
                    int offset2 = (rt * cols * 2) + 1;
                    int boostcalvalue = Convert.ToInt32(autfacmap[offset1]) * 256 + Convert.ToInt32(autfacmap[offset2]);
                    boostcalvalue += (rt * 80);
                    byte b1 = Convert.ToByte(boostcalvalue / 256);
                    byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                    autfacmap[offset1] = b1;
                    autfacmap[offset2] = b2;
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "PedalMapCal.X_ManFacTab"), GetSymbolLength(m_symbols, "PedalMapCal.X_ManFacTab"), autfacmap, m_currentfile, true);
                AddToResumeTable("Tuned pedal factor map for manual transmission (PedalMapCal.X_ManFacTab)");
            }
            UpdateChecksum(m_currentfile);
            /*** TorqueCal.M_NominalMap ***/
            /*** Data-matrix for nominal Torque. Engine speed and airmass are used as support points. 
            The value in the matrix will be the engine output torque when inlet airmass (- friction airmass) 
            is used together with actual engine speed as pointers ***/
            // formula = replace last column with estimated values of max_torque (= last column * 1.3)
            int max_torque = 0;

            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap") > 0)
            {
                byte[] torquemap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(m_symbols, "TorqueCal.M_NominalMap"));

                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_NominalMap", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_NominalMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = cols - 1; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(torquemap[offset1]) * 256 + Convert.ToInt32(torquemap[offset2]);
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > max_torque) max_torque = boostcalvalue;

                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        torquemap[offset1] = b1;
                        torquemap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_NominalMap"), GetSymbolLength(m_symbols, "TorqueCal.M_NominalMap"), torquemap, m_currentfile, true);
                AddToResumeTable("Tuned nominal torque map (TorqueCal.M_NominalMap)");
            }
            UpdateChecksum(m_currentfile);
            /*** TorqueCal.m_AirXSP (should run upto maximum requested airflow) ***/
            /*** Air mass supportpoints for Ignition angle limit influenceing torque table, Ignition- angle influence on 
            torque table and Nominal torque table. Resolution is 1 mg/combustion ***/
            // formula: just replace the rightmost column (last value in the table) to the maximum airmass requested)
            int max_airflow_requested = 1300; //TODO: determine max airflow in previous algorithms
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP") > 0)
            {
                byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(m_symbols, "TorqueCal.m_AirXSP"));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_AirXSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_AirXSP")) rows /= 2;
                int offset1 = (torquenominalx.Length - 2);
                int offset2 = (torquenominalx.Length - 1);
                int boostcalvalue = Convert.ToInt32(torquenominalx[offset1]) * 256 + Convert.ToInt32(torquenominalx[offset2]);
                boostcalvalue = max_airflow_requested;
                byte b1 = Convert.ToByte(boostcalvalue / 256);
                byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                torquenominalx[offset1] = b1;
                torquenominalx[offset2] = b2;
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirXSP"), GetSymbolLength(m_symbols, "TorqueCal.m_AirXSP"), torquenominalx, m_currentfile, true);
                AddToResumeTable("Tuned x axis for nominal torquemap (TorqueCal.m_AirXSP)");
            }

            UpdateChecksum(m_currentfile);
            /*** TorqueCal.M_EngXSP (should run upto maximum requested torque in Nm) ***/
            /*** Engine torque supportpoints for nominal airmass table. Resolution is 1 Nm ***/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP") > 0)
            {
                byte[] torquenominalx = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(m_symbols, "TorqueCal.M_EngXSP"));
                int cols = 0;
                int rows = 0;
                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.M_EngXSP", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.M_EngXSP")) rows /= 2;
                int offset1 = (torquenominalx.Length - 2);
                int offset2 = (torquenominalx.Length - 1);
                int boostcalvalue = Convert.ToInt32(torquenominalx[offset1]) * 256 + Convert.ToInt32(torquenominalx[offset2]);
                boostcalvalue = max_torque;
                byte b1 = Convert.ToByte(boostcalvalue / 256);
                byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                torquenominalx[offset1] = b1;
                torquenominalx[offset2] = b2;
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.M_EngXSP"), GetSymbolLength(m_symbols, "TorqueCal.M_EngXSP"), torquenominalx, m_currentfile, true);
                AddToResumeTable("Tuned x axis for nominal airmass map (TorqueCal.M_EngXSP)");
            }
            UpdateChecksum(m_currentfile);
            /*** TorqueCal.m_AirTorqMap ***/
            /*** Data-matrix for nominal airmass. Engine speed and torque are used as support points. 
            The value in the matrix + friction airmass (idle airmass) will create the pointed torque at the pointed engine speed. 
            Resolution is   1 mg/c. ***/
            if ((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap") > 0)
            {
                byte[] torquemap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TorqueCal.m_AirTorqMap"));

                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "TorqueCal.m_AirTorqMap", out cols, out rows);
                if (isSixteenBitTable("TorqueCal.m_AirTorqMap")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = cols - 1; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(torquemap[offset1]) * 256 + Convert.ToInt32(torquemap[offset2]);
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        if (boostcalvalue > max_torque) max_torque = boostcalvalue;

                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        torquemap[offset1] = b1;
                        torquemap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "TorqueCal.m_AirTorqMap"), GetSymbolLength(m_symbols, "TorqueCal.m_AirTorqMap"), torquemap, m_currentfile, true);
                AddToResumeTable("Tuned nominal airmass map (TorqueCal.m_AirTorqMap)");
            }
            // update the checksum
            UpdateChecksum(m_currentfile);
            /*** AirCtrlCal.m_MaxAirTab ***/
            if ((int)GetSymbolAddress(m_symbols, "AirCtrlCal.m_MaxAirTab") > 0)
            {
                byte[] torquemap = readdatafromfile(m_currentfile, (int)GetSymbolAddress(m_symbols, "AirCtrlCal.m_MaxAirTab"), GetSymbolLength(m_symbols, "AirCtrlCal.m_MaxAirTab"));

                int cols = 0;
                int rows = 0;

                GetTableMatrixWitdhByName(m_currentfile, m_symbols, "AirCtrlCal.m_MaxAirTab", out cols, out rows);
                if (isSixteenBitTable("AirCtrlCal.m_MaxAirTab")) rows /= 2;
                // row > 5 and col > 4
                for (int rt = 0; rt < rows; rt++)
                {
                    for (int ct = 0; ct < cols; ct++)
                    {
                        int offset1 = (rt * cols * 2) + (ct * 2);
                        int offset2 = (rt * cols * 2) + (ct * 2) + 1;
                        int boostcalvalue = Convert.ToInt32(torquemap[offset1]) * 256 + Convert.ToInt32(torquemap[offset2]);
                        boostcalvalue *= 13;
                        boostcalvalue /= 10;
                        if (boostcalvalue > maxairmass) boostcalvalue = (int)maxairmass;
                        //if (boostcalvalue > max_torque) max_torque = boostcalvalue;

                        byte b1 = Convert.ToByte(boostcalvalue / 256);
                        byte b2 = Convert.ToByte(boostcalvalue - (int)b1 * 256);
                        torquemap[offset1] = b1;
                        torquemap[offset2] = b2;
                    }
                }
                savedatatobinary((int)GetSymbolAddress(m_symbols, "AirCtrlCal.m_MaxAirTab"), GetSymbolLength(m_symbols, "AirCtrlCal.m_MaxAirTab"), torquemap, m_currentfile, true);
                AddToResumeTable("Tuned airmass limiter (AirCtrlCal.m_MaxAirTab)");
            }
            // update the checksum
            UpdateChecksum(m_currentfile);

            /*** TorqueCal.X_AccPedalMap ***/
            /*** Data-matrix for calculation of approx pedal positions for Out.X_AccPedal. Resolution is 0.1 % ***/


            /*** InjCorrCal.InjectorConst (uncertain, on hold) ***/
            /*** Injector constant for actual injectors. Resolution is 1 g/min ***/

            /*** LimEngCal.TurboSpeedTab (uncertain, on hold) ***/
            /*** Max allowed m_request depending on ambient air. Resolution is 1 mg/c. ***/

            /*** MaxSpdCal.n_EngLimAir (not absolutely needed) ***/
            /*** This table include the maximum engine speed limit. Above this limit will the airmass be reduced if a gear is detected. It is always fuelcut above this limit + 200rpm. Resolution is 1 rpm ***/

            /*** TorqueCal.m_PedYSP (already done) ***/
            /*** Air mass supportpoints for (Calc) X_AccPedalMap. Resolution is 1 mg/combustion ***/


            // mark binary as tuned to stage I

            //AddToResumeTable("Updated binary description with tuned stage");


            AddToResumeTable("Updated checksum.");

            progress.Close();

            // refresh open viewers

        }

    }
}
