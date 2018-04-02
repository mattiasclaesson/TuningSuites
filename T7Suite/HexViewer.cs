using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;
using CommonSuite;

namespace T7
{
    public partial class HexViewer : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void SelectionChanged(object sender, SelectionChangedEventArgs e);
        public event HexViewer.SelectionChanged onSelectionChanged;

        private string _fileName = string.Empty;
        private string _lastFilename = string.Empty;

        public string LastFilename
        {
            get { return _lastFilename; }
            set { _lastFilename = value; }
        }

        private int m_currentfile_size = 0x80000;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        readonly frmFind _formFind = new frmFind();
        frmFindCancel _formFindCancel;
        FindOptions findOptions;
        SymbolCollection m_symbolcollection= new SymbolCollection();

        private bool m_issramviewer = false;

        public bool Issramviewer
        {
            get { return m_issramviewer; }
            set { m_issramviewer = value; }
        }

        public HexViewer()
        {
            InitializeComponent();   
        }

        public void SelectText(string symbolname, int fileoffset, int length)
        {
            hexBox1.SelectionStart = fileoffset;
            hexBox1.SelectionLength = length;
            hexBox1.ScrollByteIntoView(fileoffset + length + 64); // scroll 4 lines extra for viewing purposes
            toolStripButton1.Text = symbolname;
        }

        public DialogResult CloseFile()
        {
            if (hexBox1.ByteProvider == null)
                return DialogResult.OK;

            try
            {
                if (hexBox1.ByteProvider != null && hexBox1.ByteProvider.HasChanges())
                {
                    DialogResult res = MessageBox.Show("Do you want to save changes?",
                        "T7Suite",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        SaveFile();
                        CleanUp();
                    }
                    else if (res == DialogResult.No)
                    {
                        CleanUp();
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        return res;
                    }

                    return res;
                }
                else
                {
                    CleanUp();
                    return DialogResult.OK;
                }
            }
            finally
            {
                ManageAbility();
            }
        }

        /// <summary>
        /// Saves the current file.
        /// </summary>
        void SaveFile()
        {
            if (hexBox1.ByteProvider == null)
                return;

            try
            {
                if (File.Exists(_fileName))
                {
                    File.Copy(_fileName, _fileName + "-backup" + DateTime.Now.Ticks.ToString());
                    FileByteProvider fileByteProvider = hexBox1.ByteProvider as FileByteProvider;
                    DynamicByteProvider dynamicByteProvider = hexBox1.ByteProvider as DynamicByteProvider;
                    DynamicFileByteProvider dynamicFileByteProvider = hexBox1.ByteProvider as DynamicFileByteProvider;
                    if (fileByteProvider != null)
                    {
                        fileByteProvider.ApplyChanges();
                    }
                    else if (dynamicFileByteProvider != null)
                    {
                        dynamicFileByteProvider.ApplyChanges();
                    }
                    else if (dynamicByteProvider != null)
                    {
                        byte[] data = dynamicByteProvider.Bytes.ToArray();
                        using (FileStream stream = File.Open(_fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            stream.Write(data, 0, data.Length);
                        }
                        dynamicByteProvider.ApplyChanges();
                    }
                }
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "T7Suite", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                ManageAbility();
            }
        }


        void CleanUp()
        {
            if (hexBox1.ByteProvider != null)
            {
                IDisposable byteProvider = hexBox1.ByteProvider as IDisposable;
                if (byteProvider != null)
                    byteProvider.Dispose();
                hexBox1.ByteProvider = null;
            }
            _fileName = null;
        }


        void OpenFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File does not exist!");
                return;
            }

            if (hexBox1.ByteProvider != null)
            {
                if (CloseFile() == DialogResult.Cancel)
                    return;
            }

            try
            {
                FileByteProvider fileByteProvider = new FileByteProvider(fileName);
                fileByteProvider.Changed += byteProvider_Changed;
                hexBox1.ByteProvider = fileByteProvider;
                _fileName = fileName;
                _lastFilename = _fileName;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "HexEditor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
               ManageAbility();
            }
        }

        void byteProvider_Changed(object sender, EventArgs e)
        {
            ManageAbility();
        }

        void ManageAbility()
        {
            if (hexBox1.ByteProvider == null)
            {
                miClose.Enabled = false;
                miSave.Enabled =  false;

                miFind.Enabled = false;
                miFindNext.Enabled = false;
            }
            else
            {
                miSave.Enabled = hexBox1.ByteProvider.HasChanges();

                miClose.Enabled = true;

                miFind.Enabled = true;
                miFindNext.Enabled = true;
            }

            ManageAbilityForCopyAndPaste();
        }

        /// <summary>
        /// Manages enabling or disabling of menu items and toolbar buttons for copy and paste
        /// </summary>
        void ManageAbilityForCopyAndPaste()
        {
            miCopy.Enabled = hexBox1.CanCopy();
            miCut.Enabled = hexBox1.CanCut();
            miPaste.Enabled = hexBox1.CanPaste();
        }

        public void LoadDataFromFile(string filename, SymbolCollection symbols)
        {
            _fileName = filename;
            _lastFilename = _fileName;
            m_symbolcollection = symbols;
            FileInfo fi = new FileInfo(filename);
            m_currentfile_size = (int)fi.Length;
            OpenFile(filename);
        }

        void Find()
        {
            if (_formFind.ShowDialog() == DialogResult.OK)
            {
                findOptions = _formFind.GetFindOptions();
                FindNext();
            }
        }

        void FindNext()
        {
            if (findOptions == null)
            {
                Find();
                return;
            }

            // show cancel dialog
            _formFindCancel = new frmFindCancel();
            _formFindCancel.SetHexBox(hexBox1);
            _formFindCancel.Closed += FormFindCancel_Closed;
            _formFindCancel.Show();

            // start find process
            long res = hexBox1.Find(findOptions);

            _formFindCancel.Dispose();

            if (res == -1) // -1 = no match
            {
                MessageBox.Show("Find reached end of file", "T7Suite",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (res == -2) // -2 = find was aborted
            {
                return;
            }
            else // something was found
            {
                if (!hexBox1.Focused)
                    hexBox1.Focus();
            }

            ManageAbility();
        }

        void FormFindCancel_Closed(object sender, EventArgs e)
        {
            hexBox1.AbortFind();
        }


        private void miFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void miCut_Click(object sender, EventArgs e)
        {
            hexBox1.Cut();
        }

        private void miCopy_Click(object sender, EventArgs e)
        {
            hexBox1.Copy();
        }

        private void miPaste_Click(object sender, EventArgs e)
        {
            hexBox1.Paste();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            //print document
        }

        private void miSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void miClose_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private string GetSymbolNameOffSetAndLength(long index, out int offset, out int length)
        {
            offset = 0;
            length = 0;
            string retval = "No symbol";
            foreach (SymbolHelper sh in m_symbolcollection)
            {
                int address = (int)sh.Flash_start_address;
                if (m_issramviewer)
                {
                    address = (int)sh.Start_address;
                }
                int internal_length = sh.Length;
                if (address > 0)
                {
                    while (address > m_currentfile_size) address -= m_currentfile_size;
                    if (index >= address && index < (address + internal_length) && !sh.Varname.StartsWith("Pressure map"))
                    {
                        retval = sh.Varname;
                        if (m_issramviewer)
                        {
                            offset = (int)sh.Start_address;
                        }
                        else
                        {
                            offset = (int)sh.Flash_start_address;
                            while (offset > m_currentfile_size) offset -= m_currentfile_size;
                        }
                        length = sh.Length;
                        break;
                    }
                }
            }
            return retval;
        }

        private string GetSymbolName(long index)
        {
            string retval = "No symbol";
            foreach (SymbolHelper sh in m_symbolcollection)
            {
                int address = (int)sh.Flash_start_address;
                if (m_issramviewer)
                {
                    address = (int)sh.Start_address;
                }
                int length = sh.Length;
                if (address > 0)
                {
                    while (address > m_currentfile_size) address -= m_currentfile_size;
                    if (index >= address && index < (address + length) && !sh.Varname.StartsWith("Pressure map"))
                    {
                        retval = sh.Varname;
                        break;
                    }
                }
            }
            return retval;

        }

        private void hexBox1_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            toolStripLabel1.Text = string.Format("Ln {0}    Col {1}", hexBox1.CurrentLine, hexBox1.CurrentPositionInLine);
            toolStripButton1.Text = GetSymbolName((hexBox1.CurrentLine - 1) * 16 + (hexBox1.CurrentPositionInLine - 1));
        }

        private void hexBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            object oFileNames = e.Data.GetData(DataFormats.FileDrop);
            string[] fileNames = (string[])oFileNames;
            if (fileNames.Length == 1)
            {
                OpenFile(fileNames[0]);
            }
        }

        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            ManageAbilityForCopyAndPaste();
        }

        private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
        {
            ManageAbilityForCopyAndPaste();
        }

        private void hexBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

       
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Text != "" && toolStripButton1.Text != "No symbol")
            {
                // find in symbol collection
                foreach (SymbolHelper sh in m_symbolcollection)
                {
                    if (sh.Varname == toolStripButton1.Text)
                    {
                        int address = (int)sh.Flash_start_address;
                        if (m_issramviewer)
                        {
                            address = (int)sh.Start_address;
                        }

                        int length = sh.Length;
                        while (address > m_currentfile_size) address -= m_currentfile_size;
                        SelectText(sh.Varname, address, length);
                        break;
                    }
                }
            }
        }

        private void miFindNext_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void CastSelectionChanged(int offset, int length)
        {
            if (onSelectionChanged != null && offset != 0)
            {
                onSelectionChanged(this, new SelectionChangedEventArgs(offset, length));
            }
        }

        private void hexBox1_DoubleClick(object sender, EventArgs e)
        {
            int offset = 0;
            int length = 0;
            string symbol = GetSymbolNameOffSetAndLength((hexBox1.CurrentLine - 1) * 16 + (hexBox1.CurrentPositionInLine - 1), out offset, out length);
            CastSelectionChanged(Convert.ToInt32((hexBox1.CurrentLine - 1) * 16 + (hexBox1.CurrentPositionInLine - 1)), length);
            if (symbol != "No symbol")
            {
                SelectText(symbol, offset, length);
            }

        }
    }

    public class SelectionChangedEventArgs : System.EventArgs
    {
        public int Length { get; set; }

        public int Offset { get; set; }

        public SelectionChangedEventArgs(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
