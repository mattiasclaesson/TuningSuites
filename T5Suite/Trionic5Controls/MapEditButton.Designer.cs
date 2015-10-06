namespace Trionic5Controls
{
    partial class MapEditButton
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
            this.btnMapChooser = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnMapChooser
            // 
            this.btnMapChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMapChooser.Location = new System.Drawing.Point(0, 0);
            this.btnMapChooser.Name = "btnMapChooser";
            this.btnMapChooser.Size = new System.Drawing.Size(140, 82);
            this.btnMapChooser.TabIndex = 0;
            this.btnMapChooser.Text = "Edit maps";
            this.btnMapChooser.Click += new System.EventHandler(this.btnMapChooser_Click);
            // 
            // MapEditButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMapChooser);
            this.Name = "MapEditButton";
            this.Size = new System.Drawing.Size(140, 82);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnMapChooser;
    }
}
