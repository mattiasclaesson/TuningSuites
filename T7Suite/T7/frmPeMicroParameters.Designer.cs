namespace T7
{
    partial class frmPeMicroParameters
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
            this.mruEdit1 = new DevExpress.XtraEditors.MRUEdit();
            this.mruEdit2 = new DevExpress.XtraEditors.MRUEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.mruEdit3 = new DevExpress.XtraEditors.MRUEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit3.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // mruEdit1
            // 
            this.mruEdit1.Location = new System.Drawing.Point(256, 43);
            this.mruEdit1.Name = "mruEdit1";
            this.mruEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.mruEdit1.Size = new System.Drawing.Size(329, 20);
            this.mruEdit1.TabIndex = 0;
            this.mruEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.mruEdit1_ButtonClick);
            // 
            // mruEdit2
            // 
            this.mruEdit2.Location = new System.Drawing.Point(256, 69);
            this.mruEdit2.Name = "mruEdit2";
            this.mruEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.mruEdit2.Size = new System.Drawing.Size(329, 20);
            this.mruEdit2.TabIndex = 1;
            this.mruEdit2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.mruEdit2_ButtonClick);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(15, 46);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(123, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Batch file for reading ECU";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(15, 72);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(149, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Batch file for programming ECU";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.mruEdit3);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.mruEdit2);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.mruEdit1);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(601, 141);
            this.groupControl1.TabIndex = 5;
            this.groupControl1.Text = "Parameters for reading and programming...";
            // 
            // mruEdit3
            // 
            this.mruEdit3.Location = new System.Drawing.Point(256, 95);
            this.mruEdit3.Name = "mruEdit3";
            this.mruEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.mruEdit3.Size = new System.Drawing.Size(329, 20);
            this.mruEdit3.TabIndex = 6;
            this.mruEdit3.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.mruEdit3_ButtonClick);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(15, 102);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(138, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Result file when reading ECU";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(538, 159);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 6;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Batch files|*.bat";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "FROM_ECU_NOW.S19";
            this.openFileDialog2.Filter = "S19 files|*.S19";
            // 
            // frmPeMicroParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 190);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPeMicroParameters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set PE Micro interface parameters";
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mruEdit3.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.MRUEdit mruEdit1;
        private DevExpress.XtraEditors.MRUEdit mruEdit2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.MRUEdit mruEdit3;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}