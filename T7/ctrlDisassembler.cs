using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using System.IO;


namespace T7
{
    public partial class ctrlDisassembler : DevExpress.XtraEditors.XtraUserControl
    {

        private Trionic7File _trionicFile;

        public Trionic7File TrionicFile
        {
            get { return _trionicFile; }
            set { _trionicFile = value; }
        }

        public ctrlDisassembler()
        {
            InitializeComponent();
            HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(Application.StartupPath));
            editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("ASM");
            editor.IsReadOnly = false;
            //editor.ActiveTextAreaControl.TextArea.SelectionManager.SelectionChanged +=
            editor.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
            
            editor.Document.DocumentChanged += new DocumentEventHandler((senderX, eX) => { SetModifiedFlag(editor, true); });
            if (_editorSettings == null)
            {
                _editorSettings = editor.TextEditorProperties;
                OnSettingsChanged();
            }
            else
                editor.TextEditorProperties = _editorSettings;
        }

        void Caret_PositionChanged(object sender, EventArgs e)
        {
            // the caret changed, get the address that belongs to this line
            try
            {
                List<TextWord> words = editor.Document.GetLineSegment(editor.ActiveTextAreaControl.Caret.Line).Words;
                if (words.Count > 0)
                {
                    if (words[0].Word.StartsWith("0x"))
                    {
                        Int32 address = Convert.ToInt32(words[0].Word, 16);
                        Int32 endaddress = address + 4;
                        List<TextWord> wordsnextLine = editor.Document.GetLineSegment(editor.ActiveTextAreaControl.Caret.Line + 1).Words;
                        if (wordsnextLine.Count > 0)
                        {
                            if (wordsnextLine[0].Word.StartsWith("0x"))
                            {
                                endaddress = Convert.ToInt32(wordsnextLine[0].Word, 16);
                            }
                        }
                        if (address > 0)
                        {
                            int offset = 0; //_trionicFile.Filelength;
                            //if (offset == 0x20000) offset = 0x60000;
                            hexViewer1.SelectText("", address - offset, endaddress - address);
                        }
                    }

                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            
            
            
        }

        private void hexViewer1_Load(object sender, EventArgs e)
        {
            
            /*            tab.Enter += new EventHandler((sender, e) => 
                            {
                                var page = ((TabPage)sender);
                                page.BeginInvoke(new Action<TabPage>(p => p.Controls[0].Focus()), page);
                            });
                        tab.Controls.Add(editor);
                        fileTabs.Controls.Add(tab);

                        if (_editorSettings == null)
                        {
                            _editorSettings = editor.TextEditorProperties;
                            OnSettingsChanged();
                        }
                        else
                            editor.TextEditorProperties = _editorSettings;*/
        }
        /// <summary>Gets whether the file in the specified editor is modified.</summary>
        /// <remarks>TextEditorControl doesn't maintain its own internal modified 
        /// flag, so we use the '*' shown after the file name to represent the 
        /// modified state.</remarks>
        private bool IsModified(TextEditorControl editor)
        {
            // TextEditorControl doesn't seem to contain its own 'modified' flag, so 
            // instead we'll treat the "*" on the filename as the modified flag.
            return editor.Parent.Text.EndsWith("*");
        }
        private void SetModifiedFlag(TextEditorControl editor, bool flag)
        {
            if (IsModified(editor) != flag)
            {
                var p = editor.Parent;
                if (IsModified(editor))
                    p.Text = p.Text.Substring(0, p.Text.Length - 1);
                else
                    p.Text += "*";
            }
        }
        /// <summary>Returns the currently displayed editor, or null if none are open</summary>
        private TextEditorControl ActiveEditor
        {
            get
            {
                return editor;
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (editor != null)
                DoSave(editor);
        }
        private bool DoSaveAs(TextEditorControl editor)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = editor.FileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    editor.SaveFile(saveFileDialog.FileName);
                    editor.Parent.Text = Path.GetFileName(editor.FileName);
                    SetModifiedFlag(editor, false);

                    // The syntax highlighting strategy doesn't change
                    // automatically, so do it manually.
                    editor.Document.HighlightingStrategy =
                        HighlightingStrategyFactory.CreateHighlightingStrategyForFile(editor.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            return false;
        }
        private bool DoSave(TextEditorControl editor)
        {
            if (string.IsNullOrEmpty(editor.FileName))
                return DoSaveAs(editor);
            else
            {
                try
                {
                    editor.SaveFile(editor.FileName);
                    SetModifiedFlag(editor, false);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                    return false;
                }
            }
        }

        private bool HaveSelection()
        {
            return editor != null &&
                editor.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected;
        }

        /// <summary>Performs an action encapsulated in IEditAction.</summary>
        /// <remarks>
        /// There is an implementation of IEditAction for every action that 
        /// the user can invoke using a shortcut key (arrow keys, Ctrl+X, etc.)
        /// The editor control doesn't provide a public funciton to perform one
        /// of these actions directly, so I wrote DoEditAction() based on the
        /// code in TextArea.ExecuteDialogKey(). You can call ExecuteDialogKey
        /// directly, but it is more fragile because it takes a Keys value (e.g.
        /// Keys.Left) instead of the action to perform.
        /// <para/>
        /// Clipboard commands could also be done by calling methods in
        /// editor.ActiveTextAreaControl.TextArea.ClipboardHandler.
        /// </remarks>
        private void DoEditAction(TextEditorControl editor, ICSharpCode.TextEditor.Actions.IEditAction action)
        {
            if (editor != null && action != null)
            {
                var area = editor.ActiveTextAreaControl.TextArea;
                editor.BeginUpdate();
                try
                {
                    lock (editor.Document)
                    {
                        action.Execute(area);
                        if (area.SelectionManager.HasSomethingSelected && area.AutoClearSelection /*&& caretchanged*/)
                        {
                            if (area.Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                            {
                                area.SelectionManager.ClearSelection();
                            }
                        }
                    }
                }
                finally
                {
                    editor.EndUpdate();
                    area.Caret.UpdateCaretPosition();
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Cut());
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Copy());
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Paste());
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (HaveSelection())
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.Delete());
        }

        FindAndReplaceForm _findForm = new FindAndReplaceForm();

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            _findForm.ShowFor(editor, false);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            _findForm.ShowFor(editor, true);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            _findForm.FindNext(true, false,
                string.Format("Search text «{0}» not found.", _findForm.LookFor));
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            _findForm.FindNext(true, true,
                string.Format("Search text «{0}» not found.", _findForm.LookFor));
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            var editor = ActiveEditor;
            if (editor != null)
            {
                DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.ToggleBookmark());
                editor.IsIconBarVisible = editor.Document.BookmarkManager.Marks.Count > 0;
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoNextBookmark
                (bookmark => true));
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            DoEditAction(ActiveEditor, new ICSharpCode.TextEditor.Actions.GotoPrevBookmark
                (bookmark => true));
        }


        /// <summary>
        /// Loads an assembler file into the text editor
        /// </summary>
        /// <param name="filename"></param>
        public void LoadFile(string filename)
        {
            editor.LoadFile(filename);
        }

        /// <summary>
        /// Loads an binary file into the hexviewer
        /// </summary>
        /// <param name="filename"></param>
        public void LoadBinaryFile(string filename, SymbolCollection _symbols)
        {
            hexViewer1.LoadDataFromFile(filename, _symbols);
        }

        private void editor_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void editor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        Dictionary<TextEditorControl, HighlightGroup> _highlightGroups = new Dictionary<TextEditorControl, HighlightGroup>();


        private void highlightSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string LookFor = string.Empty;

            LookFor = editor.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;

            // if there's text selected, highlight all!
            if (!_highlightGroups.ContainsKey(editor))
                _highlightGroups[editor] = new HighlightGroup(editor);
            HighlightGroup group = _highlightGroups[editor];
            TextEditorSearcher _search = new TextEditorSearcher();
            _search.Document = editor.Document;

            group.ClearMarkers();


            _search.LookFor = LookFor;
            _search.MatchCase = true;
            _search.MatchWholeWordOnly = true;

            bool looped = false;
            int offset = 0, count = 0;
            for (; ; )
            {
                TextRange range = _search.FindNext(offset, false, out looped);
                if (range == null || looped)
                    break;
                offset = range.Offset + range.Length;
                count++;

                var m = new TextMarker(range.Offset, range.Length,
                        TextMarkerType.SolidBlock, Color.Orange, Color.Black);
                group.AddMarker(m);
            }
            
            editor.Invalidate();
            editor.Refresh();
        }

        private void ctrlDisassembler_Resize(object sender, EventArgs e)
        {
            // set the splitter
            
        }

        private void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.F)
                {
                    TextEditorControl editor = ActiveEditor;
                    if (editor == null) return;
                    _findForm.ShowFor(editor, false);
                }
                else if (e.KeyCode == Keys.H)
                {
                    TextEditorControl editor = ActiveEditor;
                    if (editor == null) return;
                    _findForm.ShowFor(editor, true);
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                _findForm.FindNext(true, false, string.Format("Search text «{0}» not found.", _findForm.LookFor));
            }
        }

        private void menuSplitTextArea_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.Split();
        }
        ITextEditorProperties _editorSettings;


        private void OnSettingsChanged()
        {
            menuShowSpacesTabs.Checked = _editorSettings.ShowSpaces;
            menuShowNewlines.Checked = _editorSettings.ShowEOLMarker;
            menuHighlightCurrentRow.Checked = _editorSettings.LineViewerStyle == LineViewerStyle.FullRow;
            menuBracketMatchingStyle.Checked = _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After;
            menuEnableVirtualSpace.Checked = _editorSettings.AllowCaretBeyondEOL;
            menuShowLineNumbers.Checked = _editorSettings.ShowLineNumbers;
        }

        private void menuShowSpacesTabs_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowSpaces = editor.ShowTabs = !editor.ShowSpaces;
            OnSettingsChanged();
        }

        private void menuShowNewlines_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowEOLMarkers = !editor.ShowEOLMarkers;
            OnSettingsChanged();
        }

        private void menuShowLineNumbers_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.ShowLineNumbers = !editor.ShowLineNumbers;
            OnSettingsChanged();
        }

        private void menuHighlightCurrentRow_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.LineViewerStyle = editor.LineViewerStyle == LineViewerStyle.None
                ? LineViewerStyle.FullRow : LineViewerStyle.None;
            OnSettingsChanged();
        }

        private void menuBracketMatchingStyle_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.BracketMatchingStyle = editor.BracketMatchingStyle == BracketMatchingStyle.After
                ? BracketMatchingStyle.Before : BracketMatchingStyle.After;
            OnSettingsChanged();
        }

        private void menuEnableVirtualSpace_Click(object sender, EventArgs e)
        {
            TextEditorControl editor = ActiveEditor;
            if (editor == null) return;
            editor.AllowCaretBeyondEOL = !editor.AllowCaretBeyondEOL;
            OnSettingsChanged();
        }

        private void menuSetTabSize_Click(object sender, EventArgs e)
        {
            if (ActiveEditor != null)
            {
                string result = InputBox.Show("Specify the desired tab width.", "Tab size", _editorSettings.TabIndent.ToString());
                int value;
                if (result != null && int.TryParse(result, out value) && value.IsInRange(1, 32))
                {
                    ActiveEditor.TabIndent = value;
                }
            }
        }

        private void menuSetFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            var editor = ActiveEditor;
            if (editor != null)
            {
                fontDialog.Font = editor.Font;
                if (fontDialog.ShowDialog(this) == DialogResult.OK)
                {
                    editor.Font = fontDialog.Font;
                    OnSettingsChanged();
                }
            }
        }

        private void hexViewer1_onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // in that case, move the selection in the ASM file as well
            Console.WriteLine("Selection in hexviewer: " + e.Offset.ToString("X8"));
            int offset = 0;// _trionicFile.Filelength;
            //if (offset == 0x20000) offset = 0x60000;

            int address = e.Offset + offset;
            string searchString = "0x" + address.ToString("X8");
            //_findForm.FindNext(true, false, string.Format("Search text «{0}» not found.", searchString));
            TextEditorSearcher _search = new TextEditorSearcher();
            _search.ClearScanRegion();
            _search.Document = editor.Document;
            _search.LookFor = searchString;
            _search.MatchCase = true;
            _search.MatchWholeWordOnly = true;

            var caret = editor.ActiveTextAreaControl.Caret;
            bool _lastSearchLoopedAround = false;
            int startFrom = 0;
            TextRange range = _search.FindNext(startFrom, false, out _lastSearchLoopedAround);
            if (range != null)
                SelectResult(range);
            editor.Invalidate();
            editor.Refresh();


        }

        private void SelectResult(TextRange range)
        {
            TextLocation p1 = editor.Document.OffsetToPosition(range.Offset);
            TextLocation p2 = editor.Document.OffsetToPosition(range.Offset + range.Length);
            editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            // Also move the caret to the end of the selection, because when the user 
            // presses F3, the caret is where we start searching next time.
            editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(range.Offset + range.Length);
        }

        public void DisassembleFile(string outputfile)
        {
            bool _skipDisassembly = false;

            if (File.Exists(outputfile))
            {
                if (MessageBox.Show("Assemblerfile already exists, do you want to redo the disassembly?", "Question", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    _skipDisassembly = true;
                }
            }
            if (!_skipDisassembly)
            {
                Disassembler disasm = new Disassembler();
                disasm.onProgress += new Disassembler.Progress(disasm_onProgress);
                Console.WriteLine("Starting disassembly");
				disasm.DisassembleFile(_trionicFile, _trionicFile.FileName, outputfile, _trionicFile.Symbol_collection);
                Console.WriteLine("Done disassembling: " + disasm.Mnemonics.Count.ToString());
                using (StreamWriter sw = new StreamWriter(outputfile))
                {
                    foreach (MNemonicHelper helper in disasm.Mnemonics)
                    {
                        if (helper.Mnemonic.Contains(":"))
                        {
                            //listBox1.Items.Add(helper.Mnemonic);
                            if (!helper.Mnemonic.Contains("LBL_"))
                            {
                                sw.WriteLine();
                            }
                            sw.WriteLine(helper.Mnemonic);
                        }
                        else
                        {
                            //listBox1.Items.Add(helper.Address.ToString("X8") + " " + helper.Mnemonic);
                            sw.WriteLine("0x" + helper.Address.ToString("X8") + "\t" + helper.Mnemonic);
                        }
                    }
                }
                // start the external viewer with the file
            }
            LoadFile(outputfile);
            string copyFile = _trionicFile.FileName + DateTime.Now.Ticks.ToString();
            File.Copy(_trionicFile.FileName, copyFile);
            LoadBinaryFile(copyFile, _trionicFile.Symbol_collection);
        }

        void disasm_onProgress(object sender, Disassembler.ProgressEventArgs e)
        {
             switch (e.Type)
            {
                case Disassembler.ProgressType.DisassemblingVectors:
                    toolStripProgressBar1.Value = e.Percentage;
                    break;
                case Disassembler.ProgressType.DisassemblingFunctions:
                    toolStripStatusLabel3.Text = e.Percentage.ToString("D3") + " functions disassembled";
                    break;
                case Disassembler.ProgressType.TranslatingVectors:
                case Disassembler.ProgressType.TranslatingLabels:
                case Disassembler.ProgressType.SortingData:
                    if (e.Percentage <= toolStripProgressBar2.Maximum)
                    {
                        toolStripProgressBar2.Value = e.Percentage;
                    }
                    break;
            }
            Application.DoEvents();
        }
    }
    /// <summary>
    /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    /// </summary>
    public class RegionFoldingStrategy : IFoldingStrategy
    {
        /// <summary>
        /// Generates the foldings for our document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <param name="fileName">The filename of the document.</param>
        /// <param name="parseInformation">Extra parse information, not used in this sample.</param>
        /// <returns>A list of FoldMarkers.</returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            List<FoldMarker> list = new List<FoldMarker>();

            Stack<int> startLines = new Stack<int>();

            // Create foldmarkers for the whole document, enumerate through every line.
            for (int i = 0; i < document.TotalNumberOfLines; i++)
            {
                var seg = document.GetLineSegment(i);
                int offs, end = document.TextLength;
                char c;
                for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                { }
                if (offs == end)
                    break;
                int spaceCount = offs - seg.Offset;

                // now offs points to the first non-whitespace char on the line
                if (document.GetCharAt(offs) == '#')
                {
                    string text = document.GetText(offs, seg.Length - spaceCount);
                    if (text.StartsWith("#region"))
                        startLines.Push(i);
                    if (text.StartsWith("#endregion") && startLines.Count > 0)
                    {
                        // Add a new FoldMarker to the list.
                        int start = startLines.Pop();
                        list.Add(new FoldMarker(document, start,
                            document.GetLineSegment(start).Length,
                            i, spaceCount + "#endregion".Length));
                    }
                }
            }

            return list;
        }
    }

    public class HighlightGroup : IDisposable
    {
        List<TextMarker> _markers = new List<TextMarker>();
        TextEditorControl _editor;
        IDocument _document;
        public HighlightGroup(TextEditorControl editor)
        {
            _editor = editor;
            _document = editor.Document;
        }
        public void AddMarker(TextMarker marker)
        {
            _markers.Add(marker);
            _document.MarkerStrategy.AddMarker(marker);
        }
        public void ClearMarkers()
        {
            foreach (TextMarker m in _markers)
                _document.MarkerStrategy.RemoveMarker(m);
            _markers.Clear();
            _editor.Refresh();
        }
        
        public void Dispose()
        {
            Console.WriteLine("Disposed!");
            ClearMarkers(); 
            GC.SuppressFinalize(this);
        }

        ~HighlightGroup() { Dispose(); }

        public IList<TextMarker> Markers { get { return _markers.AsReadOnly(); } }
    }
}
