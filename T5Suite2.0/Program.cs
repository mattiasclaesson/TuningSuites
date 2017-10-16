using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace T5Suite2._
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.UserSkins.BonusSkins.Register();

            Application.Run(new Form1(args));
        }
    }
}