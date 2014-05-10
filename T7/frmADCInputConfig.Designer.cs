namespace T7
{
    partial class frmADCInputConfig
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
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtHighValue = new DevExpress.XtraEditors.TextEdit();
            this.txtHighVoltage = new DevExpress.XtraEditors.TextEdit();
            this.txtLowValue = new DevExpress.XtraEditors.TextEdit();
            this.txtLowVoltage = new DevExpress.XtraEditors.TextEdit();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighVoltage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowValue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowVoltage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.labelControl6);
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.txtHighValue);
            this.groupControl1.Controls.Add(this.txtHighVoltage);
            this.groupControl1.Controls.Add(this.txtLowValue);
            this.groupControl1.Controls.Add(this.txtLowVoltage);
            this.groupControl1.Controls.Add(this.textEdit1);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 12);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(399, 125);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "AD channel config";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(194, 94);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(93, 13);
            this.labelControl6.TabIndex = 23;
            this.labelControl6.Text = "equals symbolvalue";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(194, 70);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(93, 13);
            this.labelControl5.TabIndex = 22;
            this.labelControl5.Text = "equals symbolvalue";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(10, 92);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(67, 13);
            this.labelControl4.TabIndex = 21;
            this.labelControl4.Text = "High value (V)";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(10, 70);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(65, 13);
            this.labelControl3.TabIndex = 20;
            this.labelControl3.Text = "Low value (V)";
            // 
            // txtHighValue
            // 
            this.txtHighValue.EditValue = "20,0";
            this.txtHighValue.Location = new System.Drawing.Point(316, 87);
            this.txtHighValue.Name = "txtHighValue";
            this.txtHighValue.Properties.Mask.EditMask = "f";
            this.txtHighValue.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtHighValue.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtHighValue.Size = new System.Drawing.Size(60, 20);
            this.txtHighValue.TabIndex = 19;
            // 
            // txtHighVoltage
            // 
            this.txtHighVoltage.EditValue = "5";
            this.txtHighVoltage.Location = new System.Drawing.Point(119, 87);
            this.txtHighVoltage.Name = "txtHighVoltage";
            this.txtHighVoltage.Properties.Mask.EditMask = "f";
            this.txtHighVoltage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtHighVoltage.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtHighVoltage.Size = new System.Drawing.Size(60, 20);
            this.txtHighVoltage.TabIndex = 18;
            // 
            // txtLowValue
            // 
            this.txtLowValue.EditValue = "10,0";
            this.txtLowValue.Location = new System.Drawing.Point(316, 63);
            this.txtLowValue.Name = "txtLowValue";
            this.txtLowValue.Properties.Mask.EditMask = "f";
            this.txtLowValue.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtLowValue.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtLowValue.Size = new System.Drawing.Size(60, 20);
            this.txtLowValue.TabIndex = 17;
            // 
            // txtLowVoltage
            // 
            this.txtLowVoltage.EditValue = "0";
            this.txtLowVoltage.Location = new System.Drawing.Point(119, 63);
            this.txtLowVoltage.Name = "txtLowVoltage";
            this.txtLowVoltage.Properties.Mask.EditMask = "f";
            this.txtLowVoltage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.txtLowVoltage.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtLowVoltage.Size = new System.Drawing.Size(60, 20);
            this.txtLowVoltage.TabIndex = 16;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(119, 32);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(257, 20);
            this.textEdit1.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(14, 39);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Symbolname";
            // 
            // simpleButton1
            // 
            this.simpleButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleButton1.Location = new System.Drawing.Point(336, 143);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "Ok";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // frmADCInputConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 176);
            this.ControlBox = false;
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.groupControl1);
            this.Name = "frmADCInputConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AD channel configuration";
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighVoltage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowValue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowVoltage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtHighValue;
        private DevExpress.XtraEditors.TextEdit txtHighVoltage;
        private DevExpress.XtraEditors.TextEdit txtLowValue;
        private DevExpress.XtraEditors.TextEdit txtLowVoltage;
    }
}