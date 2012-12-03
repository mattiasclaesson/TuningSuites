namespace T8SuitePro
{
    partial class HexViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexViewer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.miClose = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.miFind = new System.Windows.Forms.ToolStripButton();
            this.miFindNext = new System.Windows.Forms.ToolStripButton();
            this.miSave = new System.Windows.Forms.ToolStripButton();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.miCut = new System.Windows.Forms.ToolStripButton();
            this.miCopy = new System.Windows.Forms.ToolStripButton();
            this.miPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miClose,
            this.openToolStripButton,
            this.miFind,
            this.miFindNext,
            this.miSave,
            this.printToolStripButton,
            this.toolStripSeparator,
            this.miCut,
            this.miCopy,
            this.miPaste,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.toolStripButton1,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(622, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // miClose
            // 
            this.miClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miClose.Image = ((System.Drawing.Image)(resources.GetObject("miClose.Image")));
            this.miClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miClose.Name = "miClose";
            this.miClose.Size = new System.Drawing.Size(23, 22);
            this.miClose.Text = "&Close";
            this.miClose.Click += new System.EventHandler(this.miClose_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // miFind
            // 
            this.miFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miFind.Image = ((System.Drawing.Image)(resources.GetObject("miFind.Image")));
            this.miFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miFind.Name = "miFind";
            this.miFind.Size = new System.Drawing.Size(23, 22);
            this.miFind.Text = "Find";
            this.miFind.Click += new System.EventHandler(this.miFind_Click);
            // 
            // miFindNext
            // 
            this.miFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miFindNext.Image = ((System.Drawing.Image)(resources.GetObject("miFindNext.Image")));
            this.miFindNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miFindNext.Name = "miFindNext";
            this.miFindNext.Size = new System.Drawing.Size(23, 22);
            this.miFindNext.Text = "Find Next";
            this.miFindNext.Click += new System.EventHandler(this.miFindNext_Click);
            // 
            // miSave
            // 
            this.miSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miSave.Image = ((System.Drawing.Image)(resources.GetObject("miSave.Image")));
            this.miSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miSave.Name = "miSave";
            this.miSave.Size = new System.Drawing.Size(23, 22);
            this.miSave.Text = "&Save";
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // printToolStripButton
            // 
            this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Enabled = false;
            this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
            this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.printToolStripButton.Name = "printToolStripButton";
            this.printToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.printToolStripButton.Text = "&Print";
            this.printToolStripButton.Click += new System.EventHandler(this.printToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // miCut
            // 
            this.miCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miCut.Image = ((System.Drawing.Image)(resources.GetObject("miCut.Image")));
            this.miCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miCut.Name = "miCut";
            this.miCut.Size = new System.Drawing.Size(23, 22);
            this.miCut.Text = "C&ut";
            this.miCut.Click += new System.EventHandler(this.miCut_Click);
            // 
            // miCopy
            // 
            this.miCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miCopy.Image = ((System.Drawing.Image)(resources.GetObject("miCopy.Image")));
            this.miCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miCopy.Name = "miCopy";
            this.miCopy.Size = new System.Drawing.Size(23, 22);
            this.miCopy.Text = "&Copy";
            this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miPaste
            // 
            this.miPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.miPaste.Image = ((System.Drawing.Image)(resources.GetObject("miPaste.Image")));
            this.miPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPaste.Name = "miPaste";
            this.miPaste.Size = new System.Drawing.Size(23, 22);
            this.miPaste.Text = "&Paste";
            this.miPaste.Click += new System.EventHandler(this.miPaste_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.ForeColor = System.Drawing.Color.SeaGreen;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton1.Text = "No symbol";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.hexBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(622, 444);
            this.panel1.TabIndex = 2;
            // 
            // hexBox1
            // 
            this.hexBox1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.ForeColor = System.Drawing.Color.Black;
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.DarkGray;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 0);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(622, 444);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 1;
            this.hexBox1.UseFixedBytesPerLine = true;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.hexBox1_DragEnter);
            this.hexBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.hexBox1_DragDrop);
            this.hexBox1.SelectionStartChanged += new System.EventHandler(this.hexBox1_SelectionStartChanged);
            this.hexBox1.CurrentPositionInLineChanged += new System.EventHandler(this.hexBox1_CurrentPositionInLineChanged);
            this.hexBox1.DoubleClick += new System.EventHandler(this.hexBox1_DoubleClick);
            this.hexBox1.SelectionLengthChanged += new System.EventHandler(this.hexBox1_SelectionLengthChanged);
            this.hexBox1.CurrentLineChanged += new System.EventHandler(this.hexBox1_CurrentPositionInLineChanged);
            // 
            // HexViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "HexViewer";
            this.Size = new System.Drawing.Size(622, 469);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton miClose;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton miSave;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton miCut;
        private System.Windows.Forms.ToolStripButton miCopy;
        private System.Windows.Forms.ToolStripButton miPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.ToolStripButton miFind;
        private System.Windows.Forms.ToolStripButton miFindNext;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}
