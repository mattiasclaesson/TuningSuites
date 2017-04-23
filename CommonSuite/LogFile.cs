using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using NLog;
using System.IO;

namespace CommonSuite
{
    public class LogFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static SymbolCollection FindSymbols(string filename, ref DateTime startDate, ref DateTime endDate)
        {
            SymbolCollection sc = new SymbolCollection();
            string[] alllines = File.ReadAllLines(filename);
            //using (StreamReader sr = new StreamReader(filename))
            {
                //string line = string.Empty;
                char[] sep = new char[1];
                char[] sep2 = new char[1];
                //int linecount = 0;
                sep.SetValue('|', 0);
                sep2.SetValue('=', 0);
                //while ((line = sr.ReadLine()) != null)

                foreach (string line in alllines)
                {
                    string[] values = line.Split(sep);
                    if (values.Length > 0)
                    {
                        try
                        {
                            //dd/MM/yyyy HH:mm:ss
                            //string logline = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|";

                            string dtstring = (string)values.GetValue(0);
                            DateTime dt = new DateTime(Convert.ToInt32(dtstring.Substring(6, 4)), Convert.ToInt32(dtstring.Substring(3, 2)), Convert.ToInt32(dtstring.Substring(0, 2)), Convert.ToInt32(dtstring.Substring(11, 2)), Convert.ToInt32(dtstring.Substring(14, 2)), Convert.ToInt32(dtstring.Substring(17, 2)));
                            if (dt > endDate) endDate = dt;
                            if (dt < startDate) startDate = dt;
                            for (int t = 1; t < values.Length; t++)
                            {
                                string subvalue = (string)values.GetValue(t);
                                string[] subvals = subvalue.Split(sep2);
                                if (subvals.Length == 2)
                                {
                                    string varname = (string)subvals.GetValue(0);
                                    bool sfound = false;
                                    foreach (SymbolHelper sh in sc)
                                    {
                                        if (sh.Varname == varname || sh.Userdescription == varname)
                                        {
                                            sfound = true;
                                        }
                                    }
                                    SymbolHelper nsh = new SymbolHelper();
                                    nsh.Varname = varname;
                                    if (!sfound) sc.Add(nsh);
                                }
                            }
                        }
                        catch (Exception pE)
                        {
                            logger.Debug(pE.Message);
                        }
                    }
                }
            }
            return sc;
        }
    }
}
