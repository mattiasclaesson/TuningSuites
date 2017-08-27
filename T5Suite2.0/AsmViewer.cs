using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Trionic5Tools;
using CommonSuite;

namespace T5Suite2
{
    public partial class AsmViewer : DevExpress.XtraEditors.XtraUserControl
    {
        public AsmViewer()
        {
            InitializeComponent();
        }

        internal void LoadDataFromFile(string filename, SymbolCollection m_symbols)
        {
            richTextBox1.LoadFile(filename, RichTextBoxStreamType.RichText);
            
            //richTextBox1.LoadFile(filename, RichTextBoxStreamType.PlainText);
            /*string totaltext = string.Empty;

            int linecount = 0;
            using (StreamReader sr = new StreamReader(filename))
            {
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\temp.asm", false))
                {
                    sw.Write(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1043{\fonttbl{\f0\fswiss\fcharset0 Courier new;}}{\colortbl ;\red255\green0\blue0;\red0\green128\blue0;\red0\green0\blue255;}{\*\generator Msftedit 5.41.15.1507;}\viewkind4\uc1\pard\lang1033\f0\fs20 ");
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(HighlightText(line) + @"\par");
                        linecount++;
                        //if (linecount == 1000) break;
                    }
                    sw.Write(@"\lang1043\par}");
                }
            }
            richTextBox1.LoadFile(Application.StartupPath + "\\temp.asm");*/
            //richTextBox1.Text = totaltext;
        }

        public void FindStartAddress()
        {
            int offset = richTextBox1.Find("#2700");
            if (offset > 0)
            {
                richTextBox1.SelectionStart = offset;
                richTextBox1.SelectionLength = 6;
                richTextBox1.ScrollToCaret();
            }
        }

        string[] ccodes = { "ORI.W", "ORI.B", "HI", "LS", "CC", "CS", "NE", "EQ", "VC", "VS", "PL", "MI", "GE", "LT", "GT", "LE" };

        private string HighlightText(string value)
        {
         /*   if (value.Contains("ORI"))
            {
                int idx = value.IndexOf("ORI");
                if (idx >= 0)
                {
                    // insert highlight code
                    value = value.Insert(idx, @"\cf1");
                    value = value.Insert(idx + 7, @"\cf0");
                }
            }*/
            if (value.Contains("#"))
            {
                int idx = value.IndexOf("#");
                if (idx >= 0)
                {
                    // insert highlight code
                    value = value.Insert(idx, @"\cf3 ");
                    int idx2 = value.IndexOf(",", idx);
                    if (idx2 > 0)
                    {
                        value = value.Insert(idx2, @"\cf0 ");
                    }
                    else
                    {
                        value += @"\cf0 ";
                    }
                }
            }
            return value;
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //richTextBox1.ProcessAllLines();
        }
    }
}
