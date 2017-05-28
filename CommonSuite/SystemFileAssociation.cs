using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using NLog;

namespace CommonSuite
{
    public class SystemFileAssociation
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Create(string command)
        {
            if (Registry.ClassesRoot.OpenSubKey(command) == null)
            {
                // Launch as administrator 
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = "SuiteLauncher.exe";
                proc.Arguments = "\"" + System.Windows.Forms.Application.ExecutablePath + "\"" + " \"" + command + "\"";
                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    logger.Trace("The user refused the elevation"); 
                }
            }
        }
    }
}