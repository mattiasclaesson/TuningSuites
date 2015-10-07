/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 07/04/2008
 * Ora: 14.42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using LBSoft.IndustrialCtrls.Utils;

namespace LBSoft.IndustrialCtrls.Knobs
{
	/// <summary>
	/// Base class for the renderers of the knob
	/// </summary>
	public class LBKnobRenderer
	{
		#region Variables
		/// <summary>
		/// Control to render
		/// </summary>
		private LBKnob knob = null;
		#endregion
		
		#region Properies
		public LBKnob Knob
		{
			set { this.knob = value; }
			get { return this.knob; }
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
			if ( this.Knob == null )
				return false;
			
			Color c = this.Knob.BackColor;
			SolidBrush br = new SolidBrush ( c );
			Pen pen = new Pen ( c );
			
			Rectangle _rcTmp = new Rectangle(0, 0, this.Knob.Width, this.Knob.Height );
			Gr.DrawRectangle ( pen, _rcTmp );
			Gr.FillRectangle ( br, rc );
			
			br.Dispose();
			pen.Dispose();
			
			return true;
		}

		/// <summary>
		/// Draw the scale of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawScale( Graphics Gr, RectangleF rc )
		{
			if ( this.Knob == null )
				return false;
			
			Color cKnob = this.Knob.ScaleColor;
			Color cKnobDark = LBColorManager.StepColor ( cKnob, 60 );
			
			LinearGradientBrush br = new LinearGradientBrush ( rc, cKnobDark, cKnob, 45 );
				
			Gr.FillEllipse ( br, rc );
			
			br.Dispose();

			return true;
		}

		/// <summary>
		/// Draw the knob of the control
		/// </summary>
		/// <param name="Gr"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		public virtual bool DrawKnob( Graphics Gr, RectangleF rc )
		{
			if ( this.Knob == null )
				return false;
			
			Color cKnob = this.Knob.KnobColor;
			Color cKnobDark = LBColorManager.StepColor ( cKnob, 60 );
			
			LinearGradientBrush br = new LinearGradientBrush ( rc, cKnob, cKnobDark, 45 );
				
			Gr.FillEllipse ( br, rc );
			
			br.Dispose();

			return true;
		}
		
		public virtual bool DrawKnobIndicator( Graphics Gr, RectangleF rc, PointF pos )
		{
			if ( this.Knob == null )
				return false;
			
			RectangleF _rc = rc;
			_rc.X = pos.X - 4;
			_rc.Y = pos.Y - 4;
			_rc.Width = 8;
			_rc.Height = 8;
			
			Color cKnob = this.Knob.IndicatorColor;
			Color cKnobDark = LBColorManager.StepColor ( cKnob, 60 );
			
			LinearGradientBrush br = new LinearGradientBrush ( _rc, cKnobDark, cKnob, 45 );
				
			Gr.FillEllipse ( br, _rc );
			
			br.Dispose();

			return true;
		}
		#endregion
	}
}
