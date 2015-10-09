namespace T7
{
    partial class frmWidebandConfig
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
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtHighAFR = new DevExpress.XtraEditors.TextEdit();
            this.txtHighVoltage = new DevExpress.XtraEditors.TextEdit();
            this.txtLowAFR = new DevExpress.XtraEditors.TextEdit();
            this.txtLowVoltage = new DevExpress.XtraEditors.TextEdit();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighAFR.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighVoltage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowAFR.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowVoltage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(243, 56);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(54, 13);
            this.labelControl6.TabIndex = 23;
            this.labelControl6.Text = "equals AFR";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(243, 32);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(54, 13);
            this.labelControl5.TabIndex = 22;
            this.labelControl5.Text = "equals AFR";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(12, 56);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(89, 13);
            this.labelControl4.TabIndex = 21;
            this.labelControl4.Text = "High voltage (Volt)";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(12, 34);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(87, 13);
            this.labelControl3.TabIndex = 20;
            this.labelControl3.Text = "Low voltage (Volt)";
            // 
            // txtHighAFR
            // 
            this.txtHighAFR.EditValue = "20,0";
            this.txtHighAFR.Location = new System.Drawing.Point(346, 53);
            this.txtHighAFR.Name = "txtHighAFR";
            this.txtHighAFR.Properties.Mask.EditMask = "f";
            this.txtHighAFR.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtHighAFR.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtHighAFR.Size = new System.Drawing.Size(100, 20);
            this.txtHighAFR.TabIndex = 19;
            // 
            // txtHighVoltage
            // 
            this.txtHighVoltage.EditValue = "5,00";
            this.txtHighVoltage.Location = new System.Drawing.Point(134, 53);
            this.txtHighVoltage.Name = "txtHighVoltage";
            this.txtHighVoltage.Properties.Mask.EditMask = "f";
            this.txtHighVoltage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtHighVoltage.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtHighVoltage.Size = new System.Drawing.Size(100, 20);
            this.txtHighVoltage.TabIndex = 18;
            // 
            // txtLowAFR
            // 
            this.txtLowAFR.EditValue = "10,0";
            this.txtLowAFR.Location = new System.Drawing.Point(346, 29);
            this.txtLowAFR.Name = "txtLowAFR";
            this.txtLowAFR.Properties.Mask.EditMask = "f";
            this.txtLowAFR.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtLowAFR.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtLowAFR.Size = new System.Drawing.Size(100, 20);
            this.txtLowAFR.TabIndex = 17;
            // 
            // txtLowVoltage
            // 
            this.txtLowVoltage.EditValue = "0,00";
            this.txtLowVoltage.Location = new System.Drawing.Point(134, 29);
            this.txtLowVoltage.Name = "txtLowVoltage";
            this.txtLowVoltage.Properties.Mask.EditMask = "f";
            this.txtLowVoltage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtLowVoltage.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtLowVoltage.Size = new System.Drawing.Size(100, 20);
            this.txtLowVoltage.TabIndex = 16;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.txtHighVoltage);
            this.groupControl1.Controls.Add(this.labelControl6);
            this.groupControl1.Controls.Add(this.txtLowVoltage);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.txtLowAFR);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.txtHighAFR);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(455, 92);
            this.groupControl1.TabIndex = 24;
            this.groupControl1.Text = "Wideband lambda configuration";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(392, 110);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 25;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(311, 110);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 26;
            this.simpleButton2.Text = "Cancel";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // frmWidebandConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 143);
            this.ControlBox = false;
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmWidebandConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wideband configuration";
            ((System.ComponentModel.ISupportInitialize)(this.txtHighAFR.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighVoltage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowAFR.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowVoltage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtHighAFR;
        private DevExpress.XtraEditors.TextEdit txtHighVoltage;
        private DevExpress.XtraEditors.TextEdit txtLowAFR;
        private DevExpress.XtraEditors.TextEdit txtLowVoltage;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
    }
}