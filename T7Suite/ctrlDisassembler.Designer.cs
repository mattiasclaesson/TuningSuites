namespace T7
{
    partial class ctrlDisassembler
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlDisassembler));
            this.editor = new ICSharpCode.TextEditor.TextEditorControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.highlightSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuEditFind = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFindAgain = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFindAgainReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuToggleBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGoToNextBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGoToPrevBookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSplitTextArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuShowSpacesTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowNewlines = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHighlightCurrentRow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBracketMatchingStyle = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEnableVirtualSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSetTabSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetFont = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton11 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton12 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.hexViewer1 = new T7.HexViewer();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // editor
            // 
            this.editor.ContextMenuStrip = this.contextMenuStrip1;
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.IsIconBarVisible = true;
            this.editor.IsReadOnly = false;
            this.editor.Location = new System.Drawing.Point(0, 49);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(256, 387);
            this.editor.TabIndent = 8;
            this.editor.TabIndex = 0;
            this.editor.Text = "...";
            this.editor.DoubleClick += new System.EventHandler(this.editor_DoubleClick);
            this.editor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.editor_MouseDoubleClick);
            this.editor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.editor_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highlightSelectionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(172, 26);
            // 
            // highlightSelectionToolStripMenuItem
            // 
            this.highlightSelectionToolStripMenuItem.Name = "highlightSelectionToolStripMenuItem";
            this.highlightSelectionToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.highlightSelectionToolStripMenuItem.Text = "Highlight selection";
            this.highlightSelectionToolStripMenuItem.Click += new System.EventHandler(this.highlightSelectionToolStripMenuItem_Click);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.editor);
            this.splitContainerControl1.Panel1.Controls.Add(this.menuStrip1);
            this.splitContainerControl1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.hexViewer1);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(841, 436);
            this.splitContainerControl1.SplitterPosition = 573;
            this.splitContainerControl1.TabIndex = 1;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 25);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(256, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditCut,
            this.menuEditCopy,
            this.menuEditPaste,
            this.menuEditDelete,
            this.toolStripSeparator4,
            this.menuEditFind,
            this.menuEditReplace,
            this.menuFindAgain,
            this.menuFindAgainReverse,
            this.toolStripSeparator5,
            this.menuToggleBookmark,
            this.menuGoToNextBookmark,
            this.menuGoToPrevBookmark});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // menuEditCut
            // 
            this.menuEditCut.Name = "menuEditCut";
            this.menuEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuEditCut.Size = new System.Drawing.Size(253, 22);
            this.menuEditCut.Text = "Cu&t";
            this.menuEditCut.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // menuEditCopy
            // 
            this.menuEditCopy.Name = "menuEditCopy";
            this.menuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuEditCopy.Size = new System.Drawing.Size(253, 22);
            this.menuEditCopy.Text = "&Copy";
            this.menuEditCopy.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // menuEditPaste
            // 
            this.menuEditPaste.Name = "menuEditPaste";
            this.menuEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuEditPaste.Size = new System.Drawing.Size(253, 22);
            this.menuEditPaste.Text = "&Paste";
            this.menuEditPaste.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // menuEditDelete
            // 
            this.menuEditDelete.Name = "menuEditDelete";
            this.menuEditDelete.Size = new System.Drawing.Size(253, 22);
            this.menuEditDelete.Text = "&Delete";
            this.menuEditDelete.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
            // 
            // menuEditFind
            // 
            this.menuEditFind.Name = "menuEditFind";
            this.menuEditFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.menuEditFind.Size = new System.Drawing.Size(253, 22);
            this.menuEditFind.Text = "&Find...";
            this.menuEditFind.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // menuEditReplace
            // 
            this.menuEditReplace.Name = "menuEditReplace";
            this.menuEditReplace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuEditReplace.Size = new System.Drawing.Size(253, 22);
            this.menuEditReplace.Text = "Find and &replace...";
            this.menuEditReplace.Click += new System.EventHandler(this.toolStripButton7_Click);
            // 
            // menuFindAgain
            // 
            this.menuFindAgain.Name = "menuFindAgain";
            this.menuFindAgain.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.menuFindAgain.Size = new System.Drawing.Size(253, 22);
            this.menuFindAgain.Text = "Find &again";
            this.menuFindAgain.Click += new System.EventHandler(this.toolStripButton8_Click);
            // 
            // menuFindAgainReverse
            // 
            this.menuFindAgainReverse.Name = "menuFindAgainReverse";
            this.menuFindAgainReverse.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.menuFindAgainReverse.Size = new System.Drawing.Size(253, 22);
            this.menuFindAgainReverse.Text = "Find again (&reverse)";
            this.menuFindAgainReverse.Click += new System.EventHandler(this.toolStripButton9_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(250, 6);
            // 
            // menuToggleBookmark
            // 
            this.menuToggleBookmark.Name = "menuToggleBookmark";
            this.menuToggleBookmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.menuToggleBookmark.Size = new System.Drawing.Size(253, 22);
            this.menuToggleBookmark.Text = "Toggle bookmark";
            this.menuToggleBookmark.Click += new System.EventHandler(this.toolStripButton10_Click);
            // 
            // menuGoToNextBookmark
            // 
            this.menuGoToNextBookmark.Name = "menuGoToNextBookmark";
            this.menuGoToNextBookmark.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.menuGoToNextBookmark.Size = new System.Drawing.Size(253, 22);
            this.menuGoToNextBookmark.Text = "Go to next bookmark";
            this.menuGoToNextBookmark.Click += new System.EventHandler(this.toolStripButton11_Click);
            // 
            // menuGoToPrevBookmark
            // 
            this.menuGoToPrevBookmark.Name = "menuGoToPrevBookmark";
            this.menuGoToPrevBookmark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
            this.menuGoToPrevBookmark.Size = new System.Drawing.Size(253, 22);
            this.menuGoToPrevBookmark.Text = "Go to previous bookmark";
            this.menuGoToPrevBookmark.Click += new System.EventHandler(this.toolStripButton12_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSplitTextArea,
            this.toolStripSeparator6,
            this.menuShowSpacesTabs,
            this.menuShowNewlines,
            this.menuShowLineNumbers,
            this.menuHighlightCurrentRow,
            this.menuBracketMatchingStyle,
            this.menuEnableVirtualSpace,
            this.toolStripSeparator7,
            this.menuSetTabSize,
            this.menuSetFont});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // menuSplitTextArea
            // 
            this.menuSplitTextArea.Name = "menuSplitTextArea";
            this.menuSplitTextArea.Size = new System.Drawing.Size(315, 22);
            this.menuSplitTextArea.Text = "Split text area";
            this.menuSplitTextArea.Click += new System.EventHandler(this.menuSplitTextArea_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(312, 6);
            // 
            // menuShowSpacesTabs
            // 
            this.menuShowSpacesTabs.Name = "menuShowSpacesTabs";
            this.menuShowSpacesTabs.Size = new System.Drawing.Size(315, 22);
            this.menuShowSpacesTabs.Text = "Show spaces && tabs";
            this.menuShowSpacesTabs.Click += new System.EventHandler(this.menuShowSpacesTabs_Click);
            // 
            // menuShowNewlines
            // 
            this.menuShowNewlines.Name = "menuShowNewlines";
            this.menuShowNewlines.Size = new System.Drawing.Size(315, 22);
            this.menuShowNewlines.Text = "Show newlines";
            this.menuShowNewlines.Click += new System.EventHandler(this.menuShowNewlines_Click);
            // 
            // menuShowLineNumbers
            // 
            this.menuShowLineNumbers.Name = "menuShowLineNumbers";
            this.menuShowLineNumbers.Size = new System.Drawing.Size(315, 22);
            this.menuShowLineNumbers.Text = "Show line numbers";
            this.menuShowLineNumbers.Click += new System.EventHandler(this.menuShowLineNumbers_Click);
            // 
            // menuHighlightCurrentRow
            // 
            this.menuHighlightCurrentRow.Name = "menuHighlightCurrentRow";
            this.menuHighlightCurrentRow.Size = new System.Drawing.Size(315, 22);
            this.menuHighlightCurrentRow.Text = "Highlight current row";
            this.menuHighlightCurrentRow.Click += new System.EventHandler(this.menuHighlightCurrentRow_Click);
            // 
            // menuBracketMatchingStyle
            // 
            this.menuBracketMatchingStyle.Name = "menuBracketMatchingStyle";
            this.menuBracketMatchingStyle.Size = new System.Drawing.Size(315, 22);
            this.menuBracketMatchingStyle.Text = "Highlight matching brackets when cursor is after";
            this.menuBracketMatchingStyle.Click += new System.EventHandler(this.menuBracketMatchingStyle_Click);
            // 
            // menuEnableVirtualSpace
            // 
            this.menuEnableVirtualSpace.Name = "menuEnableVirtualSpace";
            this.menuEnableVirtualSpace.Size = new System.Drawing.Size(315, 22);
            this.menuEnableVirtualSpace.Text = "Allow cursor past end-of-line";
            this.menuEnableVirtualSpace.Click += new System.EventHandler(this.menuEnableVirtualSpace_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(312, 6);
            // 
            // menuSetTabSize
            // 
            this.menuSetTabSize.Name = "menuSetTabSize";
            this.menuSetTabSize.Size = new System.Drawing.Size(315, 22);
            this.menuSetTabSize.Text = "Set tab size...";
            this.menuSetTabSize.Click += new System.EventHandler(this.menuSetTabSize_Click);
            // 
            // menuSetFont
            // 
            this.menuSetFont.Name = "menuSetFont";
            this.menuSetFont.Size = new System.Drawing.Size(315, 22);
            this.menuSetFont.Text = "Set font...";
            this.menuSetFont.Click += new System.EventHandler(this.menuSetFont_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripSeparator2,
            this.toolStripButton6,
            this.toolStripButton7,
            this.toolStripButton8,
            this.toolStripButton9,
            this.toolStripSeparator1,
            this.toolStripButton10,
            this.toolStripButton11,
            this.toolStripButton12});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(256, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Save";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Cut";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "Copy";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "Paste";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "Delete";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton6.Text = "Find";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton7.Text = "Find and replace";
            this.toolStripButton7.Click += new System.EventHandler(this.toolStripButton7_Click);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton8.Text = "Search next";
            this.toolStripButton8.Click += new System.EventHandler(this.toolStripButton8_Click);
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton9.Text = "Search previous";
            this.toolStripButton9.Click += new System.EventHandler(this.toolStripButton9_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton10
            // 
            this.toolStripButton10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
            this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton10.Name = "toolStripButton10";
            this.toolStripButton10.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton10.Text = "Toggle bookmark";
            this.toolStripButton10.Click += new System.EventHandler(this.toolStripButton10_Click);
            // 
            // toolStripButton11
            // 
            this.toolStripButton11.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton11.Image")));
            this.toolStripButton11.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton11.Name = "toolStripButton11";
            this.toolStripButton11.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton11.Text = "Next bookmark";
            this.toolStripButton11.Click += new System.EventHandler(this.toolStripButton11_Click);
            // 
            // toolStripButton12
            // 
            this.toolStripButton12.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton12.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton12.Image")));
            this.toolStripButton12.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton12.Name = "toolStripButton12";
            this.toolStripButton12.Size = new System.Drawing.Size(23, 20);
            this.toolStripButton12.Text = "Previous bookmark";
            this.toolStripButton12.Click += new System.EventHandler(this.toolStripButton12_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar2,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 436);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(841, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(65, 17);
            this.toolStripStatusLabel1.Text = "Disassembly";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(61, 17);
            this.toolStripStatusLabel2.Text = "Conversion";
            // 
            // toolStripProgressBar2
            // 
            this.toolStripProgressBar2.Name = "toolStripProgressBar2";
            this.toolStripProgressBar2.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(19, 17);
            this.toolStripStatusLabel3.Text = "...";
            // 
            // hexViewer1
            // 
            this.hexViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexViewer1.FileName = "";
            this.hexViewer1.Issramviewer = false;
            this.hexViewer1.LastFilename = "";
            this.hexViewer1.Location = new System.Drawing.Point(0, 0);
            this.hexViewer1.Name = "hexViewer1";
            this.hexViewer1.Size = new System.Drawing.Size(573, 436);
            this.hexViewer1.TabIndex = 0;
            this.hexViewer1.Load += new System.EventHandler(this.hexViewer1_Load);
            this.hexViewer1.onSelectionChanged += new T7.HexViewer.SelectionChanged(this.hexViewer1_onSelectionChanged);
            // 
            // ctrlDisassembler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ctrlDisassembler";
            this.Size = new System.Drawing.Size(841, 458);
            this.Resize += new System.EventHandler(this.ctrlDisassembler_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl editor;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private HexViewer hexViewer1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripButton toolStripButton10;
        private System.Windows.Forms.ToolStripButton toolStripButton11;
        private System.Windows.Forms.ToolStripButton toolStripButton12;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem highlightSelectionToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuEditCut;
        private System.Windows.Forms.ToolStripMenuItem menuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem menuEditPaste;
        private System.Windows.Forms.ToolStripMenuItem menuEditDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuEditFind;
        private System.Windows.Forms.ToolStripMenuItem menuEditReplace;
        private System.Windows.Forms.ToolStripMenuItem menuFindAgain;
        private System.Windows.Forms.ToolStripMenuItem menuFindAgainReverse;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuToggleBookmark;
        private System.Windows.Forms.ToolStripMenuItem menuGoToNextBookmark;
        private System.Windows.Forms.ToolStripMenuItem menuGoToPrevBookmark;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSplitTextArea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem menuShowSpacesTabs;
        private System.Windows.Forms.ToolStripMenuItem menuShowNewlines;
        private System.Windows.Forms.ToolStripMenuItem menuShowLineNumbers;
        private System.Windows.Forms.ToolStripMenuItem menuHighlightCurrentRow;
        private System.Windows.Forms.ToolStripMenuItem menuBracketMatchingStyle;
        private System.Windows.Forms.ToolStripMenuItem menuEnableVirtualSpace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem menuSetTabSize;
        private System.Windows.Forms.ToolStripMenuItem menuSetFont;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
    }
}
