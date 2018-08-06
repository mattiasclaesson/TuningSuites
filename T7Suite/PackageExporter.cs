using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T7;
using System.IO;
using System.Windows.Forms;
using CommonSuite;
using NLog;

namespace T7
{
    class PackageExporter
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private byte[] readdatafromfile(string filename, int address, int length)
        {
            byte[] retval = new byte[length];
            try
            {
                FileStream fsi1 = File.OpenRead(filename);
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

        private int m_AddressOffset = 0;

        public int AddressOffset
        {
            get { return m_AddressOffset; }
            set { m_AddressOffset = value; }
        }

        public void ExportMap(string packagefilename, string symbolname, string userdescription, int length, byte[] mapcontent)
        {
            using (StreamWriter sw = new StreamWriter(packagefilename, true))
            {
                if (symbolname.StartsWith("Symbol") && userdescription != "")
                {
                    sw.WriteLine("symbol=" + userdescription);
                }
                else
                {
                    sw.WriteLine("symbol=" + symbolname);
                }
                sw.WriteLine("length=" + length.ToString());
                sw.Write("data=");
                foreach (byte b in mapcontent)
                {
                    sw.Write(b.ToString("X2") + ",");
                }
                sw.WriteLine();

            }
        }

        public void ExportPackage(SymbolCollection sc, string filename, string packagefilename)
        {
            if (File.Exists(packagefilename)) File.Delete(packagefilename);

            foreach (SymbolHelper sh in sc)
            {
                if (sh.Flash_start_address - m_AddressOffset > 0 && sh.Flash_start_address - m_AddressOffset < 0x80000)
                {
                    byte[] data = readdatafromfile(filename, (int)sh.Flash_start_address - m_AddressOffset, sh.Length);
                    // if the symbol contains data... use that
                    if (sh.Currentdata != null)
                    {
                        if (sh.Length > 0) data = sh.Currentdata;
                    }
                    ExportMap(packagefilename, sh.Varname, sh.Userdescription, sh.Length, data);
                }
                else if (sh.Flash_start_address  > 0 && sh.Flash_start_address  < 0x80000)
                {
                    byte[] data = readdatafromfile(filename, (int)sh.Flash_start_address , sh.Length);
                    // if the symbol contains data... use that
                    if (sh.Currentdata != null)
                    {
                        if (sh.Length > 0) data = sh.Currentdata;
                    }
                    ExportMap(packagefilename, sh.Varname, sh.Userdescription, sh.Length, data);
                }
            }
        }
    }
}
