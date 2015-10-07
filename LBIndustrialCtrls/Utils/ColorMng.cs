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

namespace LBSoft.IndustrialCtrls.Utils
{
	/// <summary>
	/// Manager for color
	/// </summary>
	public class LBColorManager : Object
	{
		public static double BlendColour ( double fg, double bg, double alpha )
		{
			double result = bg + (alpha * (fg - bg));
			if (result < 0.0)
				result = 0.0;
			if (result > 255)
				result = 255;
			return result;
		}

		public static Color StepColor ( Color clr, int alpha )
		{
			if ( alpha == 100 )
				return clr;
			
			byte a = clr.A;
			byte r = clr.R;
			byte g = clr.G;
			byte b = clr.B;
			float bg = 0;
				
			int _alpha = Math.Min(alpha, 200);
			_alpha = Math.Max(alpha, 0);
			double ialpha = ((double)(_alpha - 100.0))/100.0;
		    
			if (ialpha > 100)
			{
				// blend with white
				bg = 255.0F;
				ialpha = 1.0F - ialpha;  // 0 = transparent fg; 1 = opaque fg
			}
			else
			{
				// blend with black
				bg = 0.0F;
				ialpha = 1.0F + ialpha;  // 0 = transparent fg; 1 = opaque fg
			}
		    
			r = (byte)(LBColorManager.BlendColour(r, bg, ialpha));
			g = (byte)(LBColorManager.BlendColour(g, bg, ialpha));
			b = (byte)(LBColorManager.BlendColour(b, bg, ialpha));
	    
			return Color.FromArgb ( a, r, g, b );
		}
	};
}
