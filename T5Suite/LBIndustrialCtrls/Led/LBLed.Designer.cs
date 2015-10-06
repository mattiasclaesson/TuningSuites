/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 26/02/2008
 * Ora: 11.44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace LBSoft.IndustrialCtrls.Leds
{
	public partial class LBLed
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmrBlink = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// tmrBlink
			// 
			this.tmrBlink.Interval = 500;
			this.tmrBlink.Tick += new System.EventHandler(this.OnBlink);
			// 
			// LBLed
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "LBLed";
			this.ResumeLayout(false);
		}
	}
}
