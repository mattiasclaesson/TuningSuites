namespace CommonSuite
{
    partial class frmTuneBinary
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
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.spinEdit2 = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.spinEdit2);
            this.groupControl1.Controls.Add(this.checkEdit1);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.spinEdit1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(320, 158);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "Enter tuning parameters";
            // 
            // checkEdit1
            // 
            this.checkEdit1.Location = new System.Drawing.Point(167, 124);
            this.checkEdit1.Name = "checkEdit1";
            this.checkEdit1.Properties.Caption = "Car runs E85";
            this.checkEdit1.Size = new System.Drawing.Size(137, 19);
            this.checkEdit1.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(28, 96);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(112, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Maximum engine power";
            // 
            // spinEdit1
            // 
            this.spinEdit1.EditValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.spinEdit1.Location = new System.Drawing.Point(169, 89);
            this.spinEdit1.Name = "spinEdit1";
            this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit1.Properties.IsFloatValue = false;
            this.spinEdit1.Properties.Mask.EditMask = "N00";
            this.spinEdit1.Properties.MaxValue = new decimal(new int[] {
            450,
            0,
            0,
            0});
            this.spinEdit1.Properties.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEdit1.Size = new System.Drawing.Size(70, 20);
            this.spinEdit1.TabIndex = 0;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(257, 176);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(176, 176);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "Cancel";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(28, 54);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(79, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Maximum torque";
            // 
            // spinEdit2
            // 
            this.spinEdit2.EditValue = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.spinEdit2.Location = new System.Drawing.Point(169, 47);
            this.spinEdit2.Name = "spinEdit2";
            this.spinEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spinEdit2.Properties.IsFloatValue = false;
            this.spinEdit2.Properties.Mask.EditMask = "N00";
            this.spinEdit2.Properties.MaxValue = new decimal(new int[] {
            800,
            0,
            0,
            0});
            this.spinEdit2.Properties.MinValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEdit2.Size = new System.Drawing.Size(70, 20);
            this.spinEdit2.TabIndex = 3;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(255, 54);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(15, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Nm";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(255, 96);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(13, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "HP";
            // 
            // frmTuneBinary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 211);
            this.ControlBox = false;
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmTuneBinary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tuning process settings";
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEdit2.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.CheckEdit checkEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SpinEdit spinEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SpinEdit spinEdit2;
    }
}