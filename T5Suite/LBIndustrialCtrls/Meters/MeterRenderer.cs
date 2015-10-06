/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 03/04/2008
 * Ora: 14.34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using LBSoft.IndustrialCtrls.Utils;

namespace LBSoft.IndustrialCtrls.Meters
{
	/// <summary>
	/// Base class for the renderers of the analog meter
	/// </summary>
	public class LBAnalogMeterRenderer
	{
		#region Variables
		/// <summary>
		/// Control to render
		/// </summary>
		private LBAnalogMeter meter = null;
		#endregion
		
		#region Properies
		public LBAnalogMeter AnalogMeter
		{
			set { this.meter = value; }
			get { return this.meter; }
		}
		#endregion
		
		#region Virtual method
		public virtual bool DrawBackground( Graphics gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			Color c = this.AnalogMeter.BackColor;
			SolidBrush br = new SolidBrush ( c );
			Pen pen = new Pen ( c );
			
			Rectangle _rcTmp = new Rectangle(0, 0, this.AnalogMeter.Width, this.AnalogMeter.Height );
			gr.DrawRectangle ( pen, _rcTmp );
			gr.FillRectangle ( br, rc );
			
			br.Dispose();
			pen.Dispose();
			
			return true;
		}
		
		public virtual bool DrawBody( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			Color bodyColor = this.AnalogMeter.BodyColor;
			Color cDark = LBColorManager.StepColor ( bodyColor, 20 );
			
			LinearGradientBrush br1 = new LinearGradientBrush ( rc, 
			                                                   bodyColor,
			                                                   cDark,
			                                                   45 );
			Gr.FillEllipse ( br1, rc );
			
			float drawRatio = this.AnalogMeter.GetDrawRatio();
			
			RectangleF _rc = rc;
			_rc.X += 3 * drawRatio;
			_rc.Y += 3 * drawRatio;
			_rc.Width -= 6 * drawRatio;
			_rc.Height -= 6 * drawRatio;

			LinearGradientBrush br2 = new LinearGradientBrush ( _rc,
			                                                   cDark,
			                                                   bodyColor,
			                                                   45 );
			Gr.FillEllipse ( br2, _rc );
			
			return true;
		}
		
		public virtual bool DrawThresholds( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			float drawRatio = (float)this.AnalogMeter.GetDrawRatio();
			
			RectangleF _rc = rc;
			_rc.Inflate ( -18F * drawRatio, -18F * drawRatio );
			
			double w = _rc.Width;
			double radius = w / 2 - ( w * 0.075);
			
			float startAngle = this.AnalogMeter.GetStartAngle();
			float endAngle = this.AnalogMeter.GetEndAngle();
			float rangeAngle = endAngle - startAngle;
			float minValue = (float)this.AnalogMeter.MinValue;
			float maxValue = (float)this.AnalogMeter.MaxValue;
			
			double stepVal = rangeAngle / ( maxValue - minValue );

			foreach ( LBMeterThreshold sect in this.AnalogMeter.Thresholds )
			{
				
				float startPathAngle	= ( (float)(startAngle + ( stepVal *  sect.StartValue )));
				float endPathAngle		= ( (float)( ( stepVal * ( sect.EndValue - sect.StartValue ))));
					
				GraphicsPath pth = new GraphicsPath();
				pth.AddArc ( _rc, startPathAngle, endPathAngle );
				
				Pen pen = new Pen( sect.Color, 4.5F * drawRatio );
				
				Gr.DrawPath ( pen, pth );
				
				pen.Dispose();
				pth.Dispose();
			}
			
			return false;
		}
		
		public virtual bool DrawDivisions( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			PointF needleCenter = this.AnalogMeter.GetNeedleCenter();
			float startAngle = this.AnalogMeter.GetStartAngle();
			float endAngle = this.AnalogMeter.GetEndAngle();
			float scaleDivisions = this.AnalogMeter.ScaleDivisions;
			float scaleSubDivisions = this.AnalogMeter.ScaleSubDivisions;
			float drawRatio = this.AnalogMeter.GetDrawRatio();
			double minValue = this.AnalogMeter.MinValue;
			double maxValue = this.AnalogMeter.MaxValue;
			Color scaleColor = this.AnalogMeter.ScaleColor;
			
			float cx = needleCenter.X;
			float cy = needleCenter.Y;
			float w = rc.Width;
			float h = rc.Height;

			float incr = LBMath.GetRadian(( endAngle - startAngle ) / (( scaleDivisions - 1 )* (scaleSubDivisions + 1)));
			float currentAngle = LBMath.GetRadian( startAngle );
			float radius = (float)(w / 2 - ( w * 0.08));
			float rulerValue = (float)minValue;

			Pen pen = new Pen ( scaleColor, ( 1 * drawRatio ) );
			SolidBrush br = new SolidBrush ( scaleColor );
			
			PointF ptStart = new PointF(0,0);
			PointF ptEnd = new PointF(0,0);
			int n = 0;
			for( ; n < scaleDivisions; n++ )
			{
					//Draw Thick Line
				ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
				ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));
				ptEnd.X = (float)(cx + (radius - w/20) * Math.Cos(currentAngle));
				ptEnd.Y = (float)(cy + (radius - w/20) * Math.Sin(currentAngle));
				Gr.DrawLine( pen, ptStart, ptEnd );
				
       				//Draw Strings
       			Font font = new Font ( this.AnalogMeter.Font.FontFamily, (float)( 8F * drawRatio ), FontStyle.Bold );
		
				float tx = (float)(cx + (radius - ( 20 * drawRatio )) * Math.Cos(currentAngle));
		        float ty = (float)(cy + (radius - ( 20 * drawRatio )) * Math.Sin(currentAngle));
		        //float val = (float)Math.Round ( rulerValue );
                float val = (float)rulerValue;
		        //String str = String.Format( "{0,0:D}", (int)val );
                String str = val.ToString("F0");
                if (val > -10 & val < 10 && val != 0) str = val.ToString("F1");
                if (val > -1 & val < 1 && val != 0) str = val.ToString("F2");
                //if (val > 10 && val < 100) str = val.ToString("F1");
                //else if (val >= 100) str = val.ToString("F0");
		
				SizeF size = Gr.MeasureString ( str, font );
				Gr.DrawString ( str, 
				                font, 
				                br, 
				                tx - (float)( size.Width * 0.5 ), 
				                ty - (float)( size.Height * 0.5 ) );

				rulerValue += (float)(( maxValue - minValue) / (scaleDivisions - 1));
		
				if ( n == scaleDivisions -1)
				{
					font.Dispose();
					break;
				}
		
				if ( scaleDivisions <= 0 )
					currentAngle += incr;
				else
				{
			        for (int j = 0; j <= scaleSubDivisions; j++)
			        {
						currentAngle += incr;
						ptStart.X = (float)(cx + radius * Math.Cos(currentAngle));
						ptStart.Y = (float)(cy + radius * Math.Sin(currentAngle));
						ptEnd.X = (float)(cx + (radius - w/50) * Math.Cos(currentAngle));
						ptEnd.Y = (float)(cy + (radius - w/50) * Math.Sin(currentAngle));
						Gr.DrawLine( pen, ptStart, ptEnd );
			        }
				}
				
				font.Dispose();
			}
			
			return true;
		}
		
		public virtual bool DrawUM( Graphics gr, RectangleF rc )
		{
			return false;
		}
		
		public virtual bool DrawValue( Graphics gr, RectangleF rc )
		{
			return false;
		}
		
		public virtual bool DrawNeedle( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			float w, h ;		
			w = rc.Width;
			h = rc.Height;
		
			double minValue = this.AnalogMeter.MinValue;
			double maxValue = this.AnalogMeter.MaxValue;
			double currValue = this.AnalogMeter.Value;
			float startAngle = this.AnalogMeter.GetStartAngle();
			float endAngle = this.AnalogMeter.GetEndAngle();
			PointF needleCenter = this.AnalogMeter.GetNeedleCenter();
			
			float radius = (float)(w / 2 - ( w * 0.12));
			float val = (float)(maxValue - minValue);
		
			val = (float)((100 * ( currValue - minValue )) / val);
			val = (( endAngle - startAngle ) * val) / 100;
		    val += startAngle;
			
		    float angle = LBMath.GetRadian ( val );
		    
		    float cx = needleCenter.X;
		    float cy = needleCenter.Y;
		    
		    PointF ptStart = new PointF(0,0);
		    PointF ptEnd = new PointF(0,0);

		    GraphicsPath pth1 = new GraphicsPath();
				    
		    ptStart.X = cx;
		    ptStart.Y = cy;		    
		    angle = LBMath.GetRadian(val + 10);
			ptEnd.X = (float)(cx + (w * .09F) * Math.Cos(angle));
		    ptEnd.Y = (float)(cy + (w * .09F) * Math.Sin(angle));
		    pth1.AddLine ( ptStart, ptEnd );
		    
		    ptStart = ptEnd;
		    angle = LBMath.GetRadian(val);
		    ptEnd.X = (float)(cx + radius * Math.Cos(angle));
		    ptEnd.Y = (float)(cy + radius * Math.Sin(angle));
			pth1.AddLine ( ptStart, ptEnd );

		    ptStart = ptEnd;
		    angle = LBMath.GetRadian(val - 10);
			ptEnd.X = (float)(cx + (w * .09F) * Math.Cos(angle));
		    ptEnd.Y = (float)(cy + (w * .09F) * Math.Sin(angle));
		    pth1.AddLine ( ptStart, ptEnd );
			
		    pth1.CloseFigure();
		    
			SolidBrush br = new SolidBrush( this.AnalogMeter.NeedleColor );
		    Pen pen = new Pen ( this.AnalogMeter.NeedleColor );
			Gr.DrawPath ( pen, pth1 );
			Gr.FillPath ( br, pth1 );
			
			return true;
		}
		
		public virtual bool DrawNeedleCover( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			Color clr = this.AnalogMeter.NeedleColor;
			RectangleF _rc = rc;
			float drawRatio = this.AnalogMeter.GetDrawRatio();
			
			Color clr1 = Color.FromArgb( 70, clr );
			
			_rc.Inflate ( 5 * drawRatio, 5 * drawRatio );
		
			SolidBrush brTransp = new SolidBrush ( clr1 );
			Gr.FillEllipse ( brTransp, _rc );
			
			clr1 = clr;
			Color clr2 = LBColorManager.StepColor ( clr, 75 );
			LinearGradientBrush br1 = new LinearGradientBrush( rc,
			                                                   clr1,
			                                                   clr2,
			                                                   45 );
			Gr.FillEllipse ( br1, rc );
			return true;
		}
		
		public virtual bool DrawGlass( Graphics Gr, RectangleF rc )
		{
			if ( this.AnalogMeter == null )
				return false;
			
			if ( this.AnalogMeter.ViewGlass == false )
				return true;
			
			Color clr1 = Color.FromArgb( 40, 200, 200, 200 );
			
			Color clr2 = Color.FromArgb( 0, 200, 200, 200 );
			LinearGradientBrush br1 = new LinearGradientBrush( rc,
			                                                   clr1,
			                                                   clr2,
			                                                   45 );
			Gr.FillEllipse ( br1, rc );
			
			return true;
		}
		#endregion
	}
}
