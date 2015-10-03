/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 05/04/2008
 * Ora: 13.35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LBSoft.IndustrialCtrls.Knobs
{
	/// <summary>
	/// Description of LBKnob.
	/// </summary>
	public partial class LBKnob : UserControl
	{
		#region Enumerators
		public enum KnobStyle
		{
			Circular = 0,
		}
		#endregion
		
		#region Properties variables
		private float			minValue = 0.0F;
		private float			maxValue = 1.0F;
		private float			stepValue = 0.1F;
		private float			currValue = 0.0F;
		private KnobStyle		style = KnobStyle.Circular;
		private LBKnobRenderer	renderer = null;
		private Color			scaleColor = Color.Green;
		private Color			knobColor = Color.Black ;
		private Color			indicatorColor = Color.Red;
		private float			indicatorOffset = 10F;
		#endregion
		
		#region Class variables
		private RectangleF		drawRect;
		private RectangleF		rectScale;
		private RectangleF		rectKnob;
		private float			drawRatio;
		private LBKnobRenderer	defaultRenderer = null;
		private bool			isKnobRotating = false;
		private PointF			knobCenter;
		private PointF			knobIndicatorPos;
		#endregion		
		
		#region Constructor
		public LBKnob()
		{
			InitializeComponent();
			
			// Set the styles for drawing
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				true);

			// Transparent background
			this.BackColor = Color.Transparent;
			
			this.defaultRenderer = new LBKnobRenderer();
			this.defaultRenderer.Knob = this;
			
			this.CalculateDimensions();
		}
		#endregion
		
		#region Properties
		[
			Category("Knob"),
			Description("Minimum value of the knob")
		]
		public float MinValue
		{
			set 
			{ 
				this.minValue = value;
				this.Invalidate();
			}
			get { return this.minValue; }
		}

		[
			Category("Knob"),
			Description("Maximum value of the knob")
		]
		public float MaxValue
		{
			set 
			{ 
				this.maxValue = value;
				this.Invalidate();
			}
			get { return this.maxValue; }
		}

		[
			Category("Knob"),
			Description("Step value of the knob")
		]
		public float StepValue
		{
			set 
			{ 
				this.stepValue = value;
				this.Invalidate();
			}
			get { return this.stepValue; }
		}
		
		[
			Category("Knob"),
			Description("Current value of the knob")
		]
		public float Value
		{
			set 
			{ 
				if ( value != this.currValue )
				{
					this.currValue = value;
					this.knobIndicatorPos = this.GetPositionFromValue ( this.currValue );
					this.Invalidate();
					
					LBKnobEventArgs e = new LBKnobEventArgs();
					e.Value = this.currValue;
					this.OnKnobChangeValue( e );
				}
			}
			get { return this.currValue; }
		}
				
		[
			Category("Knob"),
			Description("Style of the knob")
		]
		public KnobStyle Style
		{
			set 
			{ 
				this.style = value;
				this.Invalidate();
			}
			get { return this.style; }
		}		
				
		[
			Category("Knob"),
			Description("Color of the knob")
		]
		public Color KnobColor
		{
			set 
			{ 
				this.knobColor = value;
				this.Invalidate();
			}
			get { return this.knobColor; }
		}		
				
		[
			Category("Knob"),
			Description("Color of the scale")
		]
		public Color ScaleColor
		{
			set 
			{ 
				this.scaleColor = value;
				this.Invalidate();
			}
			get { return this.scaleColor; }
		}
		
		[
			Category("Knob"),
			Description("Color of the indicator")
		]
		public Color IndicatorColor
		{
			set 
			{ 
				this.indicatorColor = value;
				this.Invalidate();
			}
			get { return this.indicatorColor; }
		}
		
		[
			Category("Knob"),
			Description("Offset of the indicator from the kob border")
		]
		public float IndicatorOffset
		{
			set 
			{ 
				this.indicatorOffset = value;
				this.CalculateDimensions();
				this.Invalidate();
			}
			get { return this.indicatorOffset; }
		}
		
		[Browsable(false)]
		public LBKnobRenderer Renderer
		{
			get { return this.renderer; }
			set
			{
				this.renderer = value;
				if ( this.renderer != null )
					renderer.Knob = this;
				Invalidate();
			}
		}
		
		[Browsable(false)]
		public PointF KnobCenter
		{
			get { return this.knobCenter; }
		}
		#endregion

		#region Events delegates
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			bool blResult = true;

			/// <summary>
			/// Specified WM_KEYDOWN enumeration value.
			/// </summary>
			const int WM_KEYDOWN = 0x0100;

			/// <summary>
			/// Specified WM_SYSKEYDOWN enumeration value.
			/// </summary>
			const int WM_SYSKEYDOWN = 0x0104;

			float val = this.Value;
			
			if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
			{
				switch(keyData)
				{
					case Keys.Up:
						val += this.StepValue;
						if ( val <= this.MaxValue )
							this.Value = val;
						break;

					case Keys.Down:
						val -= this.StepValue;
						if ( val >= this.MinValue )
							this.Value = val;
						break;
						
					case Keys.PageUp:
						if ( val <  this.MaxValue )
						{
							val += ( this.StepValue * 10 );
							this.Value = val;
						}
						break;
						
					case Keys.PageDown:
						if ( val > this.MinValue )
						{
							val -= ( this.StepValue * 10 );
							this.Value = val;
						}
						break;

					case Keys.Home:
						this.Value = this.MinValue;
						break;
						
					case Keys.End:
						this.Value = this.MaxValue;
						break;

					default:
						blResult = base.ProcessCmdKey(ref msg,keyData);
						break;
				}
			}

			return blResult;
		}
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnClick(EventArgs e)
		{
			this.Focus();
			this.Invalidate();
			base.OnClick(e);
		}
		
		void OnMouseUp(object sender, MouseEventArgs e)
		{
			this.isKnobRotating = false;
			
			if ( this.rectKnob.Contains ( e.Location ) == false )
				return;
			
			float val = this.GetValueFromPosition ( e.Location );
			if ( val != this.Value )
			{
				this.Value = val;
				this.Invalidate();
			}
		}
		
		void OnMouseDown(object sender, MouseEventArgs e)
		{
			if ( this.rectKnob.Contains ( e.Location ) == false )
				return;
			
			this.isKnobRotating = true;
			
			this.Focus();
		}
		
		void OnMouseMove(object sender, MouseEventArgs e)
		{
			if ( this.isKnobRotating == false )
				return;
			
			float val = this.GetValueFromPosition ( e.Location );
			if ( val != this.Value )
			{
				this.Value = val;
				this.Invalidate ();
			}
		}
		
		void OnKeyDown(object sender, KeyEventArgs e)
		{
			float val = this.Value;

			switch ( e.KeyCode )
			{
				case Keys.Up:
					val = this.Value + this.StepValue;
					break;

				case Keys.Down:
					val = this.Value - this.StepValue;
					break;
			}

			if ( val < this.MinValue )
				val = this.MinValue;
			   
			if ( val > this.MaxValue )
				val = this.MaxValue;
			
			this.Value = val;
		}

		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			
			this.CalculateDimensions();
			
			this.Invalidate();
		}
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(PaintEventArgs e)
		{
			RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height );
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			
			if ( this.Renderer == null )
			{
				this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				this.defaultRenderer.DrawScale( e.Graphics, this.rectScale );
				this.defaultRenderer.DrawKnob( e.Graphics, this.rectKnob );
				this.defaultRenderer.DrawKnobIndicator( e.Graphics, this.rectKnob, this.knobIndicatorPos );
				return;
			}
		
			this.Renderer.DrawBackground( e.Graphics, _rc );
			this.Renderer.DrawScale( e.Graphics, this.rectScale );
			this.Renderer.DrawKnob( e.Graphics, this.rectKnob );
			this.Renderer.DrawKnobIndicator( e.Graphics, this.rectKnob, this.knobIndicatorPos );
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
			
			this.rectScale = this.drawRect;
			this.rectKnob = this.drawRect;
			this.rectKnob.Inflate ( -20 * this.drawRatio, -20 * this.drawRatio );
			
			this.knobCenter.X = this.rectKnob.Left + ( this.rectKnob.Width * 0.5F );
			this.knobCenter.Y = this.rectKnob.Top + ( this.rectKnob.Height * 0.5F );	
			
			this.knobIndicatorPos = this.GetPositionFromValue ( this.Value );
		}
		
		public virtual float GetValueFromPosition ( PointF position )
		{
			float degree = 0.0F;
			float v = 0.0F;
		
			PointF center = this.KnobCenter;
			
			if ( position.X <= center.X )
			{
				degree  = (center.Y - position.Y ) /  (center.X - position.X );
				degree = (float)Math.Atan(degree);
				degree = (float)((degree) * (180F / Math.PI) + 45F);
				v = (degree * ( this.MaxValue - this.MinValue )/ 270F);
			}
			else
			{
				if ( position.X > center.X )
				{
					degree  = (position.Y - center.Y ) /  (position.X - center.X );
					degree = (float)Math.Atan(degree);
					degree = (float)(225F + (degree) * (180F / Math.PI));
					v = (degree * ( this.MaxValue - this.MinValue ) / 270F);
				}
			}
		
			if ( v > this.MaxValue )
				v = this.MaxValue;
		
			if (v < this.MinValue )
				v = this.MinValue;
		
			return v;					
		}
		
		public virtual PointF GetPositionFromValue ( float val )
		{
			PointF pos = new PointF( 0.0F, 0.0F );
				
				// Elimina la divisione per 0
			if ( ( this.MaxValue - this.MinValue ) == 0 )	
				return pos;
		
			float _indicatorOffset = this.IndicatorOffset * this.drawRatio;
			
			float degree = 270F * val / ( this.MaxValue - this.MinValue );
			degree = (degree + 135F) * (float)Math.PI / 180F;
		
			pos.X = (int)(Math.Cos(degree) * ((this.rectKnob.Width * 0.5F)- indicatorOffset ) + this.rectKnob.X + ( this.rectKnob.Width * 0.5F));
			pos.Y = (int)(Math.Sin(degree) * ((this.rectKnob.Width * 0.5F)- indicatorOffset ) + this.rectKnob.Y + ( this.rectKnob.Height * 0.5F));
		
			return pos;
		}
		#endregion

		#region Fire events
		public event KnobChangeValue KnobChangeValue;
		protected virtual void OnKnobChangeValue( LBKnobEventArgs e )
	    {
	        if( this.KnobChangeValue != null )
	            this.KnobChangeValue( this, e );
	    }		
		#endregion
	}

	#region Classes for event and event delagates args
	
	#region Event args class
	/// <summary>
	/// Class for events delegates
	/// </summary>
	public class LBKnobEventArgs : EventArgs
	{
		private float val;
			
		public LBKnobEventArgs()
		{			
		}
	
		public float Value
		{
			get { return this.val; }
			set { this.val = value; }
		}
	}
	#endregion
	
	#region Delegates
	public delegate void KnobChangeValue ( object sender, LBKnobEventArgs e );
	#endregion
	
	#endregion
}
