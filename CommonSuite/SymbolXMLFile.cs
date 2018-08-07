using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using System.IO;
using System.Data;

namespace CommonSuite
{
    public class SymbolXMLFile
    {
        private readonly static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetFileDescriptionFromFile(string file)
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
                logger.Debug(E);
            }
            return retval;
        }

        public static void SaveAdditionalSymbols(string filename, SymbolCollection collection)
        {
            using (DataTable dt = new DataTable(Path.GetFileNameWithoutExtension(filename)))
            {
                dt.Columns.Add("SYMBOLNAME");
                dt.Columns.Add("SYMBOLNUMBER", Type.GetType("System.Int32"));
                dt.Columns.Add("FLASHADDRESS", Type.GetType("System.Int32"));
                dt.Columns.Add("DESCRIPTION");

                string xmlfilename = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".xml");
                if (File.Exists(xmlfilename))
                {
                    File.Delete(xmlfilename);
                }
                foreach (SymbolHelper sh in collection)
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
    }
}
