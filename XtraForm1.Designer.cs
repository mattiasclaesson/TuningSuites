namespace T7
{
    partial class XtraForm1
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
            this.mapViewer1 = new T7.MapViewer();
            this.SuspendLayout();
            // 
            // mapViewer1
            // 
            this.mapViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapViewer1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mapViewer1.Appearance.Options.UseFont = true;
            this.mapViewer1.Correction_factor = 1;
            this.mapViewer1.Correction_offset = 0;
            this.mapViewer1.DatasourceMutated = false;
            this.mapViewer1.DisableColors = false;
            this.mapViewer1.Filename = null;
            this.mapViewer1.GraphVisible = true;
            this.mapViewer1.IsRAMViewer = false;
            this.mapViewer1.IsRedWhite = false;
            this.mapViewer1.IsUpsideDown = false;
            this.mapViewer1.Location = new System.Drawing.Point(6, 6);
            this.mapViewer1.LockMode = 0;
            this.mapViewer1.Map_address = 0;
            this.mapViewer1.Map_cat = T7.XDFCategories.Undocumented;
            this.mapViewer1.Map_content = null;
            this.mapViewer1.Map_descr = "";
            this.mapViewer1.Map_length = 0;
            this.mapViewer1.Map_name = "";
            this.mapViewer1.Map_sramaddress = 0;
            this.mapViewer1.Max_y_axis_value = 1;
            this.mapViewer1.MaxValueInTable = 0;
            this.mapViewer1.Name = "mapViewer1";
            this.mapViewer1.SaveChanges = false;
            this.mapViewer1.Size = new System.Drawing.Size(876, 664);
            this.mapViewer1.TabIndex = 0;
            this.mapViewer1.TableVisible = false;
            this.mapViewer1.Viewtype = T7.ViewType.Hexadecimal;
            this.mapViewer1.X_axis_name = "";
            this.mapViewer1.X_axisvalues = null;
            this.mapViewer1.Y_axis_name = "";
            this.mapViewer1.Y_axisvalues = null;
            this.mapViewer1.Z_axis_name = "";
            // 
            // XtraForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 674);
            this.Controls.Add(this.mapViewer1);
            this.Name = "XtraForm1";
            this.Text = "XtraForm1";
            this.ResumeLayout(false);

        }

        #endregion

        public T7.MapViewer mapViewer1;

    }
}