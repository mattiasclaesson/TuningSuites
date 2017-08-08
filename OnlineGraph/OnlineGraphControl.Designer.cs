namespace OnlineGraph
{
    partial class OnlineGraphControl
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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(0, 337);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(670, 13);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // OnlineGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "OnlineGraphControl";
            this.Size = new System.Drawing.Size(670, 350);
            this.MouseLeave += new System.EventHandler(this.OnlineGraphControl_MouseLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnlineGraphControl_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnlineGraphControl_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnlineGraphControl_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnlineGraphControl_MouseDown);
            this.Resize += new System.EventHandler(this.OnlineGraphControl_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnlineGraphControl_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;



    }
}
