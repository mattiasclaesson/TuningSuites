/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 06/04/2008
 * Ora: 14.47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using LBSoft.IndustrialCtrls.Utils;

namespace LBSoft.IndustrialCtrls.Buttons	
{
	/// <summary>
	/// Base class for the renderers of the button
	/// </summary>
	public class LBButtonRenderer
	{
		#region Variables
		/// <summary>
		/// Control to render
		/// </summary>
		private LBButton button = null;
		#endregion
		
		#region Properies
		public LBButton Button
		{
			set { this.button = value; }
			get { return this.button; }
		}
		#endregion
		
		#region Virtual method
		/// <summary>
		/// Draw the background of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawBackground( Graphics Gr, RectangleF rc )
		{
			if ( this.Button == null )
				return false;
			
			Color c = this.Button.BackColor;
			SolidBrush br = new SolidBrush ( c );
			Pen pen = new Pen ( c );
			
			Rectangle _rcTmp = new Rectangle(0, 0, this.Button.Width, this.Button.Height );
			Gr.DrawRectangle ( pen, _rcTmp );
			Gr.FillRectangle ( br, rc );
			
			br.Dispose();
			pen.Dispose();
			
			return true;
		}
		
		/// <summary>
		/// Draw the body of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawBody( Graphics Gr, RectangleF rc )
		{
			if ( this.Button == null )
				return false;
			
			Color bodyColor = this.Button.ButtonColor;
			Color cDark = LBColorManager.StepColor ( bodyColor, 20 );
			
			LinearGradientBrush br1 = new LinearGradientBrush ( rc, 
			                                                   bodyColor,
			                                                   cDark,
			                                                   45 );
			Gr.FillEllipse ( br1, rc );
			
			br1.Dispose();
			
			if ( this.Button.State == LBButton.ButtonState.Pressed )
			{			
				float drawRatio = this.Button.GetDrawRatio();
				
				RectangleF _rc = rc;
				_rc.Inflate ( -15F * drawRatio, -15F * drawRatio );
				LinearGradientBrush br2 = new LinearGradientBrush ( _rc, 
				                                                   cDark,
				                                                   bodyColor,
				                                                   45 );
				Gr.FillEllipse ( br2, _rc );
				
				br2.Dispose();
			}
			
			return true;
		}
		
		/// <summary>
		/// Draw the text of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawText( Graphics Gr, RectangleF rc )
		{
			if ( this.Button == null )
				return false;
			
			float drawRatio = this.Button.GetDrawRatio();
			
			//Draw Strings
       		Font font = new Font ( this.Button.Font.FontFamily, this.Button.Font.Size * drawRatio );

	        String str = this.Button.Label;
	
	        Color bodyColor = this.Button.ButtonColor;
			Color cDark = LBColorManager.StepColor ( bodyColor, 20 );

			SizeF size = Gr.MeasureString ( str, font );
			
			SolidBrush br1 = new SolidBrush ( bodyColor );
			SolidBrush br2 = new SolidBrush ( cDark );
			
			Gr.DrawString ( str, 
			                font, 
			                br1, 
			                rc.Left + ( ( rc.Width * 0.5F ) - (float)( size.Width * 0.5F ) ) + ( 1 * drawRatio ),
			                rc.Top + ( ( rc.Height * 0.5F ) - (float)( size.Height * 0.5 ) ) + ( 1 * drawRatio ) );
			
			Gr.DrawString ( str, 
			                font, 
			                br2, 
			                rc.Left + ( ( rc.Width * 0.5F ) - (float)( size.Width * 0.5F ) ),
			                rc.Top + ( ( rc.Height * 0.5F ) - (float)( size.Height * 0.5 ) ) );
			
			br1.Dispose();
			br2.Dispose();
			font.Dispose();
			
			return false;
		}
		#endregion
	}
}
