using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using CommonSuite;
using NLog;

namespace T8SuitePro
{
    public partial class frmBinCompare : DevExpress.XtraEditors.XtraForm
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private SymbolCollection m_symbols = new SymbolCollection();

        public SymbolCollection Symbols
        {
            get { return m_symbols; }
            set { m_symbols = value; }
        }
        private string _currentfilename = string.Empty;
        private string _comparefilename = string.Empty;

        private bool m_OutsideSymbolRangeCheck = false;

        public bool OutsideSymbolRangeCheck
        {
            get { return m_OutsideSymbolRangeCheck; }
            set { m_OutsideSymbolRangeCheck = value; }
        }

        public frmBinCompare()
        {
            InitializeComponent();
        }

        /// <summary>
        /// checks if address is outside all symbol range addresses
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private bool CheckOutsideSymbolRange(int address)
        {
            if (m_symbols == null) return true;
            foreach (SymbolHelper sh in m_symbols)
            {
                if (address >= sh.Flash_start_address && address < (sh.Flash_start_address + sh.Length))
                {
                    return false;
                }
            }
            return true;
        }

        private bool ByteCompare(Byte[] ib1, Byte[] ib2, int address)
        {
            if (ib1.Length != 16) return false;
            if (ib2.Length != 16) return false;
            for (int t = 0; t < 16; t++)
            {
                if (m_OutsideSymbolRangeCheck)
                {
                    if (CheckOutsideSymbolRange(address + t))
                    {
                        if ((Byte)ib1.GetValue(t) != (Byte)ib2.GetValue(t))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if ((Byte)ib1.GetValue(t) != (Byte)ib2.GetValue(t))
                    {
                        return false;
                    }
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

        public void CompareFiles()
        {
            try
            {
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

                                    if (!ByteCompare(ib1, ib2, (tel * 16)))
                                    {
                                        Int32 addr = tel * 16;
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
                                catch (Exception cE)
                                {
                                    logger.Debug(cE.Message);
                                }
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