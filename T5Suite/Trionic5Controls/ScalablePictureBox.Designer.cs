/// <summary>
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// Trionic5Controls.NET makes use of this control to display pictures.
/// Please visit <a href="http://www.Trionic5Controls.net/en/">http://www.Trionic5Controls.net/en/</a>
/// </summary>
namespace Trionic5Controls
{
    public partial class ScalablePictureBox
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
            this.pictureTracker = new Trionic5Controls.PictureTracker();
            this.scalablePictureBoxImp = new Trionic5Controls.ScalablePictureBoxImp();
            this.SuspendLayout();
            // 
            // pictureTracker
            // 
            this.pictureTracker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureTracker.BackColor = System.Drawing.Color.Lavender;
            this.pictureTracker.Location = new System.Drawing.Point(251, 150);
            this.pictureTracker.Name = "pictureTracker";
            this.pictureTracker.Size = new System.Drawing.Size(137, 102);
            this.pictureTracker.TabIndex = 1;
            this.pictureTracker.ZoomRate = 0;
            this.pictureTracker.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureTracker_MouseMove);
            this.pictureTracker.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureTracker_MouseDown);
            this.pictureTracker.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureTracker_MouseUp);
            // 
            // scalablePictureBoxImp
            // 
            this.scalablePictureBoxImp.BackColor = System.Drawing.Color.Gray;
            this.scalablePictureBoxImp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scalablePictureBoxImp.Location = new System.Drawing.Point(0, 0);
            this.scalablePictureBoxImp.Name = "scalablePictureBoxImp";
            this.scalablePictureBoxImp.Picture = null;
            this.scalablePictureBoxImp.Size = new System.Drawing.Size(391, 255);
            this.scalablePictureBoxImp.TabIndex = 0;
            // 
            // ScalablePictureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureTracker);
            this.Controls.Add(this.scalablePictureBoxImp);
            this.Name = "ScalablePictureBox";
            this.Size = new System.Drawing.Size(391, 255);
            this.ResumeLayout(false);

        }

        #endregion

        private ScalablePictureBoxImp scalablePictureBoxImp;
        private PictureTracker pictureTracker;
    }
}
