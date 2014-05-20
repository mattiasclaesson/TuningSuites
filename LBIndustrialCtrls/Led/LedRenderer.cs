/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 07/04/2008
 * Ora: 13.05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using LBSoft.IndustrialCtrls.Utils;

namespace LBSoft.IndustrialCtrls.Leds
{
	/// <summary>
	/// Base class for the renderers of the led
	/// </summary>
	public class LBLedRenderer
	{
		#region Variables
		/// <summary>
		/// Control to render
		/// </summary>
		private LBLed led = null;
		#endregion
		
		#region Properies
		public LBLed Led
		{
			set { this.led = value; }
			get { return this.led; }
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
			if ( this.Led == null )
				return false;
	
			Color c = this.Led.BackColor;
			SolidBrush br = new SolidBrush ( c );
			Pen pen = new Pen ( c );
			
			Rectangle _rcTmp = new Rectangle(0, 0, this.Led.Width, this.Led.Height );
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
		public virtual bool DrawLed( Graphics Gr, RectangleF rc )
		{
			if ( this.Led == null )
				return false;
	
			Color cDarkOff = LBColorManager.StepColor ( this.Led.LedColor, 20 );
			Color cDarkOn = LBColorManager.StepColor ( this.Led.LedColor, 90 );
			
			LinearGradientBrush brOff = new LinearGradientBrush ( rc, 
			                                                   	  this.Led.LedColor,
			                                                   	  cDarkOff,
			                                                	  45 );
			
			LinearGradientBrush brOn = new LinearGradientBrush ( rc, 
			                                                  	 cDarkOn,
			                                                  	 this.Led.LedColor,
			                                                  	 45 );
			if ( this.Led.State == LBLed.LedState.Blink )
			{
				if ( this.Led.BlinkIsOn == false )
					Gr.FillEllipse ( brOff, rc );
				else
					Gr.FillEllipse ( brOn, rc );					
			}
			else
			{
				if ( this.Led.State == LBLed.LedState.Off )
					Gr.FillEllipse ( brOff, rc );
				else
					Gr.FillEllipse ( brOn, rc );
			}
			
			brOff.Dispose();
			brOn.Dispose();
			
			return true;
		}
		
		/// <summary>
		/// Draw the text of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawLabel( Graphics Gr, RectangleF rc )
		{
			if ( this.Led == null )
				return false;
	
			if ( this.Led.Label == String.Empty )
				return false;
						
			SizeF size = Gr.MeasureString (  this.Led.Label, this.Led.Font );
			
			SolidBrush br1 = new SolidBrush ( this.Led.ForeColor );

			float hPos = 0;
			float vPos = 0;
			switch ( this.Led.LabelPosition )
			{
				case LBLed.LedLabelPosition.Top:
					hPos = (float)(rc.Width*0.5F)-(float)(size.Width*0.5F);
					vPos = rc.Bottom - size.Height;
					break;
					
				case LBLed.LedLabelPosition.Bottom:
					hPos = (float)(rc.Width*0.5F)-(float)(size.Width*0.5F);
					break;
					
				case LBLed.LedLabelPosition.Left:
					hPos = rc.Width - size.Width;
					vPos = (float)(rc.Height*0.5F)-(float)(size.Height*0.5F);
					break;
					
				case LBLed.LedLabelPosition.Right:
					vPos = (float)(rc.Height*0.5F)-(float)(size.Height*0.5F);
					break;
			}
			
			Gr.DrawString ( this.Led.Label, 
			                this.Led.Font, 
			                br1, 
			                rc.Left + hPos,
			                rc.Top + vPos );	
			
			return true;
		}
		#endregion
	}
}
