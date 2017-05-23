using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace SuiteLauncher
{
    public enum TrionicFileType : int
    {
        Trionic5File,
        Trionic7File,
        Trionic8File,
        UnknownBinFile
    }

    public partial class LauncherForm : Form
    {
        public LauncherForm(string[] args)
        {
            InitializeComponent();

            Console.WriteLine("blipp");
            if (args.Length == 1)
            {
                Console.WriteLine("Argument: " + args[0].ToString());
                if (args[0].ToString().ToUpper().EndsWith(".BIN"))
                {
                    StartSuite(args[0]);
                }
            }
            else if (args.Length == 2)
            {
                Console.WriteLine("Argument: " + args[0].ToString());
                Console.WriteLine("Argument: " + args[1].ToString());
                if (args[0].ToString().ToUpper().EndsWith(".EXE"))
                {
                    CreateSystemFileAssociations(System.Windows.Forms.Application.ExecutablePath, @"SystemFileAssociations\.bin\shell\Auto detect Trionic file type\command");
                    CreateSystemFileAssociations(args[0], args[1]);
                }
            }
            Environment.Exit(0);
        }

        private void StartSuite(string filename)
        {
            // open the bin file 
            Console.WriteLine("Filename: " + filename);
            if (File.Exists(filename))
            {
                Console.WriteLine("File exists!");
                TrionicFileType fileType = DetermineFileType(filename);
                Console.WriteLine("Filetype: " + fileType.ToString());
                string exeName = string.Empty;
                switch (fileType)
                {
                    case TrionicFileType.Trionic5File:
                        // start T5Suite
                        exeName = getFileAssociation(@"SystemFileAssociations\.bin\shell\Edit in T5Suite 2.0\command");
                        if (exeName == string.Empty)
                        {
                            exeName = getFileAssociation(@"SystemFileAssociations\.bin\shell\Edit in T5 Suite\command");
                        }
                        break;
                    case TrionicFileType.Trionic7File:
                        // start T7Suite
                        exeName = getFileAssociation(@"SystemFileAssociations\.bin\shell\Edit in T7 Suite\command");
                        break;
                    case TrionicFileType.Trionic8File:
                        exeName = getFileAssociation(@"SystemFileAssociations\.bin\shell\Edit in T8 Suite\command");
                        // start T8Suite
                        break;
                }
                if (exeName != string.Empty)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName, "\"" + filename + "\"");
                    Process.Start(startInfo);
                }
            }
        }

        private string getFileAssociation(string key)
        {
            string executableName = string.Empty;
            RegistryKey TempKeyCM = null;
            Console.WriteLine("Opening key: " + key);
            TempKeyCM = Registry.ClassesRoot.OpenSubKey(key);
            if (TempKeyCM != null)
            {
                Console.WriteLine("Key found!");
                object defaultValue = TempKeyCM.GetValue("");
                if (defaultValue != null)
                {
                    Console.WriteLine("default: " + defaultValue.ToString());
                    executableName = defaultValue.ToString();
                    executableName = executableName.Replace("\"%1\"", "");
                }
                TempKeyCM.Close();
            }
            return executableName;
        }

        private TrionicFileType DetermineFileType(string filename)
        {
            TrionicFileType ft = TrionicFileType.UnknownBinFile;
            FileInfo fi = new FileInfo(filename);
            if (fi.Length == 0x20000 || fi.Length == 0x40000)
            {
                if (CheckTrionic5FileType(filename))
                {
                    ft = TrionicFileType.Trionic5File;
                }
            }
            else if (fi.Length == 0x80000)
            {
                if (CheckTrionic7FileType(filename))
                {
                    ft = TrionicFileType.Trionic7File;
                }
            }
            else if (fi.Length == 0x100000)
            {
                if (CheckTrionic8FileType(filename))
                {
                    ft = TrionicFileType.Trionic8File;
                }
            }
            return ft;
        }

        private byte[] readdatafromfile(string filename, int address, int length)
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

        private bool CheckTrionic5FileType(string filename)
        {
            bool retval = false;
            byte[] data = readdatafromfile(filename, 0, 4);
            if (data.Length == 4)
            {
                if (data[0] == 0xFF && data[1] == 0xFF && data[2] == 0xF7 && data[3] == 0xFC) retval = true;
            }
            return retval;

        }

        private bool CheckTrionic7FileType(string filename)
        {
            bool retval = false;
            byte[] data = readdatafromfile(filename, 0, 4);
            if (data.Length == 4)
            {
                if (data[0] == 0xFF && data[1] == 0xFF && data[2] == 0xEF && data[3] == 0xFC) retval = true;
            }
            return retval;
        }

        private bool CheckTrionic8FileType(string filename)
        {
            byte[] testdata = readdatafromfile(filename, 0, 0x10);
            if (testdata[0] == 0x00 && (testdata[1] == 0x10 || testdata[1] == 0x00) && testdata[2] == 0x0C && testdata[3] == 0x00)
            {
                return true;
            }
            if (testdata[0] == 0x10 && (testdata[1] == 0x00 || testdata[1] == 0x10) && testdata[2] == 0x00 && testdata[3] == 0x0C)
            {
                return true;
            }
            return false;
        }

        private static void CreateSystemFileAssociations(string executablePath, string command)
        {
            try
            {
                RegistryKey TempKeyCM = null;
                TempKeyCM = Registry.ClassesRoot.CreateSubKey(command);
                string StartKey = executablePath + " \"%1\"";
                TempKeyCM.SetValue("", StartKey);
                TempKeyCM.Close();
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}
