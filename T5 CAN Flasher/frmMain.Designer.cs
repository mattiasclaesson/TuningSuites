namespace T5CanFlasher
{
    partial class frmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnFLASH = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.cboxEnLog = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDUMP = new System.Windows.Forms.Button();
            this.comboInterface = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.statusStripFLASH = new System.Windows.Forms.StatusStrip();
            this.statusFLASH = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusFLASHMake = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusFLASHType = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusFLASHSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusActivity = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.btnAbout = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.statusAdapter = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusECU = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSWVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusChecksum = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripECU = new System.Windows.Forms.StatusStrip();
            this.statusStripFLASH.SuspendLayout();
            this.statusStripECU.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 9);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(380, 264);
            this.listBox1.TabIndex = 0;
            // 
            // btnFLASH
            // 
            this.btnFLASH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFLASH.Location = new System.Drawing.Point(398, 92);
            this.btnFLASH.Name = "btnFLASH";
            this.btnFLASH.Size = new System.Drawing.Size(107, 50);
            this.btnFLASH.TabIndex = 1;
            this.btnFLASH.Text = "Update ECU";
            this.btnFLASH.UseVisualStyleBackColor = true;
            this.btnFLASH.Click += new System.EventHandler(this.btnFLASH_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(516, 223);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(107, 50);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cboxEnLog
            // 
            this.cboxEnLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxEnLog.AutoSize = true;
            this.cboxEnLog.Location = new System.Drawing.Point(398, 196);
            this.cboxEnLog.Name = "cboxEnLog";
            this.cboxEnLog.Size = new System.Drawing.Size(96, 17);
            this.cboxEnLog.TabIndex = 4;
            this.cboxEnLog.Text = "Enable logging";
            this.cboxEnLog.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(444, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Transferred: 0 Bytes";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(444, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Transmission speed: 0 B/s";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(444, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Remaining time: 00:00";
            // 
            // btnDUMP
            // 
            this.btnDUMP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDUMP.Location = new System.Drawing.Point(516, 92);
            this.btnDUMP.Name = "btnDUMP";
            this.btnDUMP.Size = new System.Drawing.Size(107, 50);
            this.btnDUMP.TabIndex = 10;
            this.btnDUMP.Text = "Read ECU";
            this.btnDUMP.UseVisualStyleBackColor = true;
            this.btnDUMP.Click += new System.EventHandler(this.btnDUMP_Click);
            // 
            // comboInterface
            // 
            this.comboInterface.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboInterface.FormattingEnabled = true;
            this.comboInterface.Items.AddRange(new object[] {
            "Lawicel CANUSB",
            "combiAdapter",
            "Mictronics DIY",
            "Just4Trionic",
            "Kvaser"});
            this.comboInterface.Location = new System.Drawing.Point(470, 9);
            this.comboInterface.Name = "comboInterface";
            this.comboInterface.Size = new System.Drawing.Size(150, 21);
            this.comboInterface.TabIndex = 15;
            this.comboInterface.SelectedIndexChanged += new System.EventHandler(this.comboInterface_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(398, 36);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(225, 50);
            this.btnConnect.TabIndex = 16;
            this.btnConnect.Text = "Connect to ECU";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // statusStripFLASH
            // 
            this.statusStripFLASH.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusFLASH,
            this.statusFLASHMake,
            this.statusFLASHType,
            this.statusFLASHSize,
            this.statusActivity,
            this.progressBar1});
            this.statusStripFLASH.Location = new System.Drawing.Point(0, 302);
            this.statusStripFLASH.Name = "statusStripFLASH";
            this.statusStripFLASH.Size = new System.Drawing.Size(632, 24);
            this.statusStripFLASH.TabIndex = 17;
            this.statusStripFLASH.Text = "statusStrip1";
            // 
            // statusFLASH
            // 
            this.statusFLASH.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusFLASH.Name = "statusFLASH";
            this.statusFLASH.Size = new System.Drawing.Size(77, 19);
            this.statusFLASH.Text = "FLASH chips";
            // 
            // statusFLASHMake
            // 
            this.statusFLASHMake.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusFLASHMake.Name = "statusFLASHMake";
            this.statusFLASHMake.Size = new System.Drawing.Size(116, 19);
            this.statusFLASHMake.Text = "Make: Not Detected";
            this.statusFLASHMake.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusFLASHType
            // 
            this.statusFLASHType.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusFLASHType.Name = "statusFLASHType";
            this.statusFLASHType.Size = new System.Drawing.Size(113, 19);
            this.statusFLASHType.Text = "Type: Not Detected";
            // 
            // statusFLASHSize
            // 
            this.statusFLASHSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusFLASHSize.Name = "statusFLASHSize";
            this.statusFLASHSize.Size = new System.Drawing.Size(183, 19);
            this.statusFLASHSize.Spring = true;
            this.statusFLASHSize.Text = "Size: Not Detected";
            this.statusFLASHSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusActivity
            // 
            this.statusActivity.Name = "statusActivity";
            this.statusActivity.Size = new System.Drawing.Size(26, 19);
            this.statusActivity.Text = "Idle";
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 18);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.Location = new System.Drawing.Point(398, 223);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(107, 50);
            this.btnAbout.TabIndex = 18;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(395, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "CAN Device:";
            // 
            // statusAdapter
            // 
            this.statusAdapter.Name = "statusAdapter";
            this.statusAdapter.Size = new System.Drawing.Size(136, 19);
            this.statusAdapter.Text = "Adapter: Not Connected";
            this.statusAdapter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusECU
            // 
            this.statusECU.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusECU.Name = "statusECU";
            this.statusECU.Size = new System.Drawing.Size(120, 19);
            this.statusECU.Text = "ECU: Not Connected";
            this.statusECU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusSWVersion
            // 
            this.statusSWVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusSWVersion.Name = "statusSWVersion";
            this.statusSWVersion.Size = new System.Drawing.Size(146, 19);
            this.statusSWVersion.Text = "SW Version: Not Detected";
            // 
            // statusChecksum
            // 
            this.statusChecksum.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusChecksum.Name = "statusChecksum";
            this.statusChecksum.Size = new System.Drawing.Size(215, 19);
            this.statusChecksum.Spring = true;
            this.statusChecksum.Text = "Checksum: Not Detected";
            this.statusChecksum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStripECU
            // 
            this.statusStripECU.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusECU,
            this.statusSWVersion,
            this.statusChecksum,
            this.statusAdapter});
            this.statusStripECU.Location = new System.Drawing.Point(0, 278);
            this.statusStripECU.Name = "statusStripECU";
            this.statusStripECU.Size = new System.Drawing.Size(632, 24);
            this.statusStripECU.SizingGrip = false;
            this.statusStripECU.TabIndex = 20;
            this.statusStripECU.Text = "statusStrip2";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 326);
            this.Controls.Add(this.statusStripECU);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.statusStripFLASH);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.comboInterface);
            this.Controls.Add(this.btnDUMP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboxEnLog);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnFLASH);
            this.Controls.Add(this.listBox1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trionic 5 CAN flasher";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStripFLASH.ResumeLayout(false);
            this.statusStripFLASH.PerformLayout();
            this.statusStripECU.ResumeLayout(false);
            this.statusStripECU.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnFLASH;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox cboxEnLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDUMP;
        private System.Windows.Forms.ComboBox comboInterface;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.StatusStrip statusStripFLASH;
        private System.Windows.Forms.ToolStripStatusLabel statusFLASHMake;
        private System.Windows.Forms.ToolStripStatusLabel statusFLASHType;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripStatusLabel statusFLASH;
        private System.Windows.Forms.ToolStripStatusLabel statusFLASHSize;
        private System.Windows.Forms.ToolStripStatusLabel statusAdapter;
        private System.Windows.Forms.ToolStripStatusLabel statusECU;
        private System.Windows.Forms.ToolStripStatusLabel statusSWVersion;
        private System.Windows.Forms.ToolStripStatusLabel statusChecksum;
        private System.Windows.Forms.StatusStrip statusStripECU;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripStatusLabel statusActivity;
    }
}

