namespace Trionic5Controls
{
    partial class LedToggler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LedToggler));
            this.pictureOn = new System.Windows.Forms.PictureBox();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.pictureOff = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOff)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureOn
            // 
            this.pictureOn.Image = ((System.Drawing.Image)(resources.GetObject("pictureOn.Image")));
            this.pictureOn.Location = new System.Drawing.Point(1, 1);
            this.pictureOn.Name = "pictureOn";
            this.pictureOn.Size = new System.Drawing.Size(16, 16);
            this.pictureOn.TabIndex = 0;
            this.pictureOn.TabStop = false;
            this.pictureOn.Visible = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(32, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(156, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Text goes here";
            // 
            // pictureOff
            // 
            this.pictureOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureOff.Image = ((System.Drawing.Image)(resources.GetObject("pictureOff.Image")));
            this.pictureOff.Location = new System.Drawing.Point(1, 1);
            this.pictureOff.Name = "pictureOff";
            this.pictureOff.Size = new System.Drawing.Size(16, 16);
            this.pictureOff.TabIndex = 2;
            this.pictureOff.TabStop = false;
            // 
            // LedToggler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureOff);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureOn);
            this.Name = "LedToggler";
            this.Size = new System.Drawing.Size(191, 18);
            ((System.ComponentModel.ISupportInitialize)(this.pictureOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureOff)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureOn;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.PictureBox pictureOff;
    }
}
