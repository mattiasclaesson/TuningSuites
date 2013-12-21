/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 26/02/2008
 * Ora: 11.44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LBSoft.IndustrialCtrls.Leds
{
	/// <summary>
	/// Class for the Led control.
	/// </summary>
	public partial class LBLed : UserControl
	{
		#region Enumeratives
		public enum LedState
		{
			Off	= 0,
			On,
			Blink,
		}
		
		public enum LedLabelPosition
		{
			Left = 0,
			Top,
			Right,
			Bottom,
		}
		#endregion
		
		#region Properties variables
		private Color				ledColor;
		private LedState			state;
		private LedLabelPosition	labelPosition;
		private	String				label = "Led";
		private SizeF				ledSize;
		private int					blinkInterval = 500;
		private LBLedRenderer		renderer = null;
		#endregion
		
		#region Class variables
		private RectangleF			drawRect;
		private RectangleF			rectLed;		
		private RectangleF			rectLabel;		
		private	Timer 				tmrBlink;
		private	bool 				blinkIsOn = false;
		private LBLedRenderer		defaultRenderer = null;
		#endregion
		
		#region Constructor
		public LBLed()
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
			
			this.ledColor		= Color.Red;
			this.state 			= LBLed.LedState.Off;
			this.blinkIsOn		= false;
			this.ledSize		= new SizeF ( 10F, 10F );
			this.labelPosition = LedLabelPosition.Top;
			
			this.defaultRenderer = new LBLedRenderer();
			this.defaultRenderer.Led = this;
		}
		#endregion
		
		#region Properties
		[
			Category("Led"),
			Description("Color of the led")
		]
		public Color LedColor
		{
			get { return ledColor; }
			set
			{
				ledColor = value;
				Invalidate();
			}
		}
		
		
		[
			Category("Led"),
			Description("State of the led")
		]
		public LedState State
		{
			get { return state; }
			set
			{
				state = value;
				if ( state == LedState.Blink )
				{
					this.blinkIsOn = true;
					this.tmrBlink.Interval = this.BlinkInterval;
					this.tmrBlink.Start();
				}
				else
				{
					this.blinkIsOn = true;
					this.tmrBlink.Stop();
				}
				
				Invalidate();
			}
		}
		
		
		[
			Category("Led"),
			Description("Size of the led")
		]
		public SizeF LedSize
		{
			get { return this.ledSize; }
			set
			{
				this.ledSize = value;
				this.CalculateDimensions();
				Invalidate();
			}
		}
		
		
		[
			Category("Led"),
			Description("Label of the led")
		]
		public String Label
		{
			get { return this.label; }
			set
			{
				this.label = value;
				Invalidate();
			}
		}
		
				
		[
			Category("Led"),
			Description("Position of the label of the led")
		]
		public LedLabelPosition LabelPosition
		{
			get { return this.labelPosition; }
			set
			{
				this.labelPosition = value;
				this.CalculateDimensions();
				Invalidate();
			}
		}
		
				
		[
			Category("Led"),
			Description("Interval for the blink state of the led")
		]
		public int BlinkInterval
		{
			get { return this.blinkInterval; }
			set { this.blinkInterval = value; }
		}
		
		[Browsable(false)]
		public LBLedRenderer Renderer
		{
			get { return this.renderer; }
			set
			{
				this.renderer = value;
				if ( this.renderer != null )
					renderer.Led = this;
				Invalidate();
			}
		}
		
		[Browsable(false)]
		public bool BlinkIsOn
		{
			get { return this.blinkIsOn; }
		}
		#endregion
		
		#region Events delegates
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnSizeChanged ( EventArgs e )
		{
			base.OnSizeChanged( e );
			this.CalculateDimensions();
			this.Invalidate();
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint ( PaintEventArgs e )
		{
			RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height );
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			
			if ( this.Renderer == null )
			{
				this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				this.defaultRenderer.DrawLed( e.Graphics, this.rectLed );
				this.defaultRenderer.DrawLabel( e.Graphics, this.rectLabel );
				return;
			}
	
			this.Renderer.DrawBackground( e.Graphics, _rc );
			this.Renderer.DrawLed( e.Graphics, this.rectLed );
			this.Renderer.DrawLabel( e.Graphics, this.rectLabel );
		}
		
		void OnBlink(object sender, EventArgs e)
		{
			if ( this.State == LedState.Blink )
			{
				if ( this.blinkIsOn == false )
					this.blinkIsOn = true;
				else
					this.blinkIsOn = false;
				
				this.Invalidate();
			}
		}
		#endregion
		
		#region Virtual functions		
		protected virtual void CalculateDimensions()
		{
				// Dati del rettangolo
			float x, y, w, h;
			x = 0;
			y = 0;
			w = this.Size.Width;
			h = this.Size.Height;
		
				// Rettangolo di disegno
			drawRect.X = x;
			drawRect.Y = y;
			drawRect.Width = w - 2;
			drawRect.Height = h - 2;
		
			this.rectLed = drawRect;
			this.rectLabel = drawRect;
			
			if ( this.LabelPosition == LedLabelPosition.Bottom )
			{
				this.rectLed.X = ( this.rectLed.Width * 0.5F ) - ( this.LedSize.Width * 0.5F );
				this.rectLed.Width = this.LedSize.Width;
				this.rectLed.Height = this.LedSize.Height;
				
				this.rectLabel.Y = this.rectLed.Bottom;
			}
			
			else if ( this.LabelPosition == LedLabelPosition.Top )
			{
				this.rectLed.X = ( this.rectLed.Width * 0.5F ) - ( this.LedSize.Width * 0.5F );
				this.rectLed.Y = this.rectLed.Height - this.LedSize.Height;
				this.rectLed.Width = this.LedSize.Width;
				this.rectLed.Height = this.LedSize.Height;
				
				this.rectLabel.Height = this.rectLed.Top;
			}
			
			else if ( this.LabelPosition == LedLabelPosition.Left )
			{
				this.rectLed.X = this.rectLed.Width - this.LedSize.Width;
				this.rectLed.Width = this.LedSize.Width;
				this.rectLed.Height = this.LedSize.Height;
				
				this.rectLabel.Width = this.rectLabel.Width - this.rectLed.Width;
			}
			
			else if ( this.LabelPosition == LedLabelPosition.Right )
			{
				this.rectLed.Width = this.LedSize.Width;
				this.rectLed.Height = this.LedSize.Height;
				
				this.rectLabel.X = this.rectLed.Right;
			}
		}
		#endregion
	}
}
