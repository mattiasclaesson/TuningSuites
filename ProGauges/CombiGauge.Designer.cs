namespace ProGauges
{
    partial class CombiGauge
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
            this.digitalDisplayControl1 = new Owf.Controls.DigitalDisplayControl();
            this.smallLinearGauge1 = new ProGauges.SmallLinearGauge();
            ((System.ComponentModel.ISupportInitialize)(this.smallLinearGauge1)).BeginInit();
            this.SuspendLayout();
            // 
            // digitalDisplayControl1
            // 
            this.digitalDisplayControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.digitalDisplayControl1.BackColor = System.Drawing.Color.Transparent;
            this.digitalDisplayControl1.DigitColor = System.Drawing.Color.DimGray;
            this.digitalDisplayControl1.Location = new System.Drawing.Point(3, 3);
            this.digitalDisplayControl1.Name = "digitalDisplayControl1";
            this.digitalDisplayControl1.Size = new System.Drawing.Size(234, 108);
            this.digitalDisplayControl1.TabIndex = 2;
            // 
            // smallLinearGauge1
            // 
            this.smallLinearGauge1.AlphaForGaugeColors = 69;
            this.smallLinearGauge1.BackGroundColor = System.Drawing.Color.DimGray;
            this.smallLinearGauge1.BevelLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.smallLinearGauge1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.smallLinearGauge1.EndColor = System.Drawing.Color.OrangeRed;
            this.smallLinearGauge1.Font = new System.Drawing.Font("Eurostile", 10F, System.Drawing.FontStyle.Bold);
            this.smallLinearGauge1.GaugeText = "Linear gauge";
            this.smallLinearGauge1.GaugeUnits = "units";
            this.smallLinearGauge1.HighlightOpaqueEnd = ((byte)(30));
            this.smallLinearGauge1.HighlightOpaqueStart = ((byte)(100));
            this.smallLinearGauge1.Location = new System.Drawing.Point(0, 117);
            this.smallLinearGauge1.Name = "smallLinearGauge1";
            this.smallLinearGauge1.NumberOfDecimals = 0;
            this.smallLinearGauge1.RecommendedRangeColor = System.Drawing.Color.LawnGreen;
            this.smallLinearGauge1.Size = new System.Drawing.Size(240, 15);
            this.smallLinearGauge1.StartColor = System.Drawing.Color.GreenYellow;
            this.smallLinearGauge1.TabIndex = 0;
            this.smallLinearGauge1.TextColor = System.Drawing.Color.Silver;
            this.smallLinearGauge1.ThresholdColor = System.Drawing.Color.Firebrick;
            this.smallLinearGauge1.TickColor = System.Drawing.Color.Gray;
            this.smallLinearGauge1.Value = 60F;
            // 
            // CombiGauge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.digitalDisplayControl1);
            this.Controls.Add(this.smallLinearGauge1);
            this.Name = "CombiGauge";
            this.Size = new System.Drawing.Size(240, 132);
            ((System.ComponentModel.ISupportInitialize)(this.smallLinearGauge1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SmallLinearGauge smallLinearGauge1;
        private Owf.Controls.DigitalDisplayControl digitalDisplayControl1;
    }
}
