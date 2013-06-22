using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace T7
{
    public partial class frmBinCompare : DevExpress.XtraEditors.XtraForm
    {
        private string _currentfilename = string.Empty;
        private string _comparefilename = string.Empty;

        public frmBinCompare()
        {
            InitializeComponent();
        }

        private bool m_OutsideSymbolBoundary = false;

        public bool OutsideSymbolBoundary
        {
            get { return m_OutsideSymbolBoundary; }
            set { m_OutsideSymbolBoundary = value; }
        }

        private SymbolCollection m_symbols;

        public void SetSymbolCollection(SymbolCollection _symbols)
        {
            m_symbols = _symbols;
        }

        private bool ByteCompare(Byte[] ib1, Byte[] ib2)
        {
            if (ib1.Length != 16) return false;
            if (ib2.Length != 16) return false;
            for (int t = 0; t < 16; t++)
            {
                if ((Byte)ib1.GetValue(t) != (Byte)ib2.GetValue(t))
                {
                    return false;
                }
            }
            return true;
        }

        public void SetCurrentFilename(string filename)
        {
            label1.Text = Path.GetFileName(filename);
            _currentfilename = filename;
        }

        public void SetCompareFilename(string filename)
        {
            label2.Text = Path.GetFileName(filename);
            _comparefilename = filename;
        }

        private bool InSymbolCollection(Int32 address)
        {
            bool retval = false;

            foreach (SymbolHelper sh in m_symbols)
            {
                if (address >= sh.Flash_start_address && address <= (sh.Flash_start_address + sh.Length))
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public void CompareFiles()
        {
            try
            {
                int bytecount = 0;
                if (File.Exists(_currentfilename))
                {
                    if (File.Exists(_comparefilename))
                    {
                        FileInfo fi = new FileInfo(_currentfilename);
                        FileInfo fi2 = new FileInfo(_comparefilename);
                        if (true)// (fi.Length == fi2.Length)
                        {
                            FileStream fsi1 = File.OpenRead(_currentfilename);
                            BinaryReader br1 = new BinaryReader(fsi1);

                            FileStream fsi2 = File.OpenRead(_comparefilename);
                            BinaryReader br2 = new BinaryReader(fsi2);



                            for (int tel = 0; tel < (fi.Length / 16); tel++)
                            {
                                try
                                {
                                    Byte[] ib1 = br1.ReadBytes(16);
                                    Byte[] ib2 = br2.ReadBytes(16);
                                    Int32 addr = tel * 16;
                                    if (m_OutsideSymbolBoundary)
                                    {
                                        if (InSymbolCollection(bytecount))
                                        {
                                            if (!ByteCompare(ib1, ib2))
                                            {
                                                string s1 = addr.ToString("X6") + ": ";
                                                string s2 = s1;
                                                for (int t = 0; t < 16; t++)
                                                {
                                                    Byte b1 = (Byte)ib1.GetValue(t);
                                                    Byte b2 = (Byte)ib2.GetValue(t);
                                                    s1 += b1.ToString("X2") + " ";
                                                    s2 += b2.ToString("X2") + " ";
                                                }
                                                listBox1.Items.Add(s1);
                                                listBox2.Items.Add(s2);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (!ByteCompare(ib1, ib2))
                                        {
                                            string s1 = addr.ToString("X6") + ": ";
                                            string s2 = s1;
                                            for (int t = 0; t < 16; t++)
                                            {
                                                Byte b1 = (Byte)ib1.GetValue(t);
                                                Byte b2 = (Byte)ib2.GetValue(t);
                                                s1 += b1.ToString("X2") + " ";
                                                s2 += b2.ToString("X2") + " ";
                                            }
                                            listBox1.Items.Add(s1);
                                            listBox2.Items.Add(s2);
                                        }
                                    }
                                }
                                catch (Exception cE)
                                {
                                    Console.WriteLine(cE.Message);
                                }
                                bytecount++;
                            }
                            fsi1.Close();
                            br1.Close();
                            fsi2.Close();
                            br2.Close();
                        }
                    }

                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}