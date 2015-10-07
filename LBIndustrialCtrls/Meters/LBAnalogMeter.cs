/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 03/04/2008
 * Ora: 14.34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LBSoft.IndustrialCtrls.Meters
{
	/// <summary>
	/// Class for the analog meter control
	/// </summary>
	public partial class LBAnalogMeter : UserControl
	{		
		#region Enumerator
		public enum AnalogMeterStyle
		{
			Circular	= 0,
		};
		#endregion
		
		#region Properties variables
		private	AnalogMeterStyle 					meterStyle;
		private Color								bodyColor;
		private Color								needleColor;
		private Color								scaleColor;
		private bool								viewGlass;
		private double								currValue;
		private double								minValue;
		private double								maxValue;
		private int									scaleDivisions;
		private int									scaleSubDivisions;
		private LBAnalogMeterRenderer				renderer;
        private LBMeterThresholdCollection listThreshold;

        public LBMeterThresholdCollection ListThreshold
        {
            get { return listThreshold; }
            set { listThreshold = value; }
        }
		#endregion

		#region Class variables
		protected PointF 				needleCenter;
		protected RectangleF			drawRect;		
		protected RectangleF			glossyRect;		
		protected RectangleF			needleCoverRect;
		protected float					startAngle;
		protected float					endAngle;
		protected float					drawRatio;
		protected LBAnalogMeterRenderer	defaultRenderer;
		#endregion
		
		#region Costructors
		public LBAnalogMeter()
		{
			// Initialization
			InitializeComponent();
			
			// Properties initialization
			this.bodyColor = Color.Red;
			this.needleColor = Color.Yellow;
			this.scaleColor = Color.White;
			this.meterStyle = AnalogMeterStyle.Circular;
			this.viewGlass = true;
			this.startAngle = 135;
			this.endAngle = 405;
			this.minValue = 0;
			this.maxValue = 1;
			this.currValue = 0;
			this.scaleDivisions = 11;
			this.scaleSubDivisions = 4;
			this.renderer = null;

			// Create the sector list
			this.listThreshold = new LBMeterThresholdCollection();
			
			// Set the styles for drawing
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				true);

			// Transparent background
			this.BackColor = Color.Transparent;
			
			// Create the default renderer
			this.defaultRenderer = new LBAnalogMeterRenderer();
			this.defaultRenderer.AnalogMeter = this;	
			
			this.CalculateDimensions();
		}
		#endregion
		
		#region Properties
		[
			Category("Analog Meter"),
			Description("Style of the control")
		]
		public AnalogMeterStyle MeterStyle
		{
			get { return meterStyle;}
			set
			{
				meterStyle = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Color of the body of the control")
		]
		public Color BodyColor
		{
			get { return bodyColor; }
			set
			{
				bodyColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Color of the needle")
		]
		public Color NeedleColor
		{
			get { return needleColor; }
			set
			{
				needleColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Show or hide the glass effect")
		]
		public bool ViewGlass
		{
			get { return viewGlass; }
			set
			{
				viewGlass = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Color of the scale of the control")
		]
		public Color ScaleColor
		{
			get { return scaleColor; }
			set
			{
				scaleColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Value of the data")
		]
		public double Value
		{
			get { return currValue; }
			set
			{
				double val = value;
				if ( val > maxValue )
					val = maxValue;
				
				if ( val < minValue )
					val = minValue;
				
				currValue = val;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Minimum value of the data")
		]
		public double MinValue
		{
			get { return minValue; }
			set
			{
				minValue = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Maximum value of the data")
		]
		public double MaxValue
		{
			get { return maxValue; }
			set
			{
				maxValue = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Number of the scale divisions")
		]
		public int ScaleDivisions
		{
			get { return scaleDivisions; }
			set
			{
				scaleDivisions = value;
				CalculateDimensions();
				Invalidate();
			}
		}
		
		
		[
			Category("Analog Meter"),
			Description("Number of the scale subdivisions")
		]
		public int ScaleSubDivisions
		{
			get { return scaleSubDivisions; }
			set
			{
				scaleSubDivisions = value;
				CalculateDimensions();
				Invalidate();
			}
		}

		[Browsable(false)]
		public LBMeterThresholdCollection Thresholds
		{
			get { return this.listThreshold; }
		}
		
		[Browsable(false)]
		public LBAnalogMeterRenderer Renderer
		{
			get { return this.renderer; }
			set
			{
				this.renderer = value;
				if ( this.renderer != null )
					renderer.AnalogMeter = this;
				Invalidate();
			}
		}
		#endregion
		
		#region Public methods
		public float GetDrawRatio()
		{
			return this.drawRatio;
		}
		
		public float GetStartAngle()
		{
			return this.startAngle;
		}
		
		public float GetEndAngle()
		{
			return this.endAngle;
		}
		
		public PointF GetNeedleCenter()
		{
			return this.needleCenter;
		}
		#endregion
		
		#region Events delegates
		protected override void OnSizeChanged ( EventArgs e )
		{
			base.OnSizeChanged( e );
			
			// Calculate dimensions
			CalculateDimensions();
			
			this.Invalidate();
		}
		
		protected override void OnPaint ( PaintEventArgs e )
		{
			RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height );
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			
			if ( this.renderer == null )
			{
				this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				this.defaultRenderer.DrawBody( e.Graphics, drawRect );
				this.defaultRenderer.DrawThresholds( e.Graphics, drawRect );
				this.defaultRenderer.DrawDivisions( e.Graphics, drawRect );
				this.defaultRenderer.DrawUM( e.Graphics, drawRect );
				this.defaultRenderer.DrawValue( e.Graphics, drawRect );
				this.defaultRenderer.DrawNeedle( e.Graphics, drawRect );
				this.defaultRenderer.DrawNeedleCover( e.Graphics, this.needleCoverRect );
				this.defaultRenderer.DrawGlass( e.Graphics, this.glossyRect );
				return;
			}

			this.renderer.DrawBackground( e.Graphics, _rc );
			this.renderer.DrawBody( e.Graphics, drawRect );
			this.renderer.DrawThresholds( e.Graphics, drawRect );
			this.renderer.DrawDivisions( e.Graphics, drawRect );
			this.renderer.DrawUM( e.Graphics, drawRect );
			this.renderer.DrawValue( e.Graphics, drawRect );
			this.renderer.DrawNeedle( e.Graphics, drawRect );
			this.renderer.DrawNeedleCover( e.Graphics, this.needleCoverRect );
			this.renderer.DrawGlass( e.Graphics, this.glossyRect );
		}
		#endregion
		
		#region Virtual functions		
		protected virtual void CalculateDimensions()
		{
			// Rectangle
			float x, y, w, h;
			x = 0;
			y = 0;
			w = this.Size.Width;
			h = this.Size.Height;
			
			// Calculate ratio
			drawRatio = (Math.Min(w,h)) / 200;
			if ( drawRatio == 0.0 )
				drawRatio = 1;
		
			// Draw rectangle
			drawRect.X = x;
			drawRect.Y = y;
			drawRect.Width = w - 2;
			drawRect.Height = h - 2;
		
			if ( w < h )
				drawRect.Height = w;
			else if ( w > h )
				drawRect.Width = h;
			
			if ( drawRect.Width < 10 )
				drawRect.Width = 10;
			if ( drawRect.Height < 10 )
				drawRect.Height = 10;
		
			// Calculate needle center
			needleCenter.X = drawRect.X + ( drawRect.Width / 2 );
			needleCenter.Y = drawRect.Y + ( drawRect.Height / 2 );	
			
			// Needle cover rect
			needleCoverRect.X = needleCenter.X - ( 8 * drawRatio );
			needleCoverRect.Y = needleCenter.Y - ( 8 * drawRatio );
			needleCoverRect.Width = 16 * drawRatio;
			needleCoverRect.Height = 16 * drawRatio;
			
			// Glass effect rect
			glossyRect.X = drawRect.X + ( 20 * drawRatio );
			glossyRect.Y = drawRect.Y + ( 10 * drawRatio );
			glossyRect.Width = drawRect.Width - ( 40 * drawRatio );
			glossyRect.Height = needleCenter.Y + ( 30 * drawRatio );
		}
		#endregion
	}
}
