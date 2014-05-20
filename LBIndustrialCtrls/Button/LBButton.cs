/*
 * Creato da SharpDevelop.
 * Utente: lucabonotto
 * Data: 05/04/2008
 * Ora: 13.36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LBSoft.IndustrialCtrls.Buttons
{
	/// <summary>
	/// Description of LBButton.
	/// </summary>
	public partial class LBButton : UserControl
	{
		#region Enumeratives
		/// <summary>
		/// Button styles
		/// </summary>
		public enum ButtonStyle
		{
			Circular = 0,
		}
		
		/// <summary>
		/// Button states
		/// </summary>
		public enum ButtonState
		{
			Normal = 0,
			Pressed,
		}
		#endregion
		
		#region Properties variables
		private ButtonStyle					buttonStyle = ButtonStyle.Circular;
		private ButtonState					buttonState = ButtonState.Normal;
		private Color						buttonColor = Color.Red;
		private	LBButtonRenderer			renderer = null;
		private string						label = String.Empty;
		#endregion
		
		#region Class variables
		private RectangleF			drawRect;
		protected float				drawRatio = 1.0F;
		protected LBButtonRenderer	defaultRenderer = null;
		#endregion
		
		#region Constructor
		public LBButton()
		{
			// Initialization
			InitializeComponent();
			
			// Properties initialization
			this.buttonColor = Color.Red;
			
			// Set the styles for drawing
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				true);

			// Transparent background
			this.BackColor = Color.Transparent;
						
			// Create the default renderer
			this.defaultRenderer = new LBButtonRenderer();
			this.defaultRenderer.Button = this;			
			this.renderer = null;
			
			// Calculate the initial dimensions
			this.CalculateDimensions();
		}
		#endregion
		
		#region Properties
		[
			Category("Button"),
			Description("Style of the button")
		]
		public ButtonStyle Style
		{
			set 
			{ 
				this.buttonStyle = value; 
				this.Invalidate();
			}
			get { return this.buttonStyle; }
		}
		
		[
			Category("Button"),
			Description("Color of the body of the button")
		]
		public Color ButtonColor
		{
			get { return buttonColor; }
			set
			{
				buttonColor = value;
				Invalidate();
			}
		}
		
		[
			Category("Button"),
			Description("Label of the button"),
			Browsable(true)
		]
		public string Label
		{
			get { return this.label; }
			set
			{
				this.label = value;
				Invalidate();
			}
		}
		
		[
			Category("Button"),
			Description("State of the button")
		]
		public ButtonState State
		{
			set 
			{ 
				this.buttonState = value; 
				this.Invalidate();
			}
			get { return this.buttonState; }
		}
		
		[Browsable(false)]
		public LBButtonRenderer Renderer
		{
			get { return this.renderer; }
			set
			{
				this.renderer = value;
				if ( this.renderer != null )
					renderer.Button = this;
				Invalidate();
			}
		}
		#endregion
		
		#region Public methods
		public float GetDrawRatio()
		{
			return this.drawRatio;
		}
		#endregion

		#region Events delegates
		
		/// <summary>
		/// Font change event
		/// </summary>
		/// <param name="e"></param>
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnFontChanged(EventArgs e)
		{
			// Calculate dimensions
			CalculateDimensions();
			
			// Redraw the control
			this.Invalidate();
		}
		
		/// <summary>
		/// Size change event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged ( EventArgs e )
		{
			base.OnSizeChanged( e );
			
			// Calculate dimensions
			CalculateDimensions();
			
			// Redraw the control
			this.Invalidate();
		}
		
		/// <summary>
		/// Paint event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint ( PaintEventArgs e )
		{
			// Control rectangle
			RectangleF _rc = new RectangleF(0, 0, this.Width, this.Height );
			
			// Set the drawing mode
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			
			// Draw with default renderer ?
			if ( this.renderer == null )
			{
				this.defaultRenderer.DrawBackground( e.Graphics, _rc );
				this.defaultRenderer.DrawBody( e.Graphics, drawRect );
				this.defaultRenderer.DrawText( e.Graphics, drawRect );
				return;
			}

			this.renderer.DrawBackground( e.Graphics, _rc );
			this.renderer.DrawBody( e.Graphics, drawRect );
			this.renderer.DrawText( e.Graphics, drawRect );
		}
		
		/// <summary>
		/// Mouse down event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMouseDown(object sender, MouseEventArgs e)
		{
			// Change the state
			this.State = ButtonState.Pressed;
			this.Invalidate();
			
			// Call the delagates
			LBButtonEventArgs ev = new LBButtonEventArgs();
			ev.State = this.State;
			this.OnButtonChangeState ( ev );
		}
		
		/// <summary>
		/// Mouse up event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMuoseUp(object sender, MouseEventArgs e)
		{
			// Change the state
			this.State = ButtonState.Normal;
			this.Invalidate();
			
			// Call the delagates
			LBButtonEventArgs ev = new LBButtonEventArgs();
			ev.State = this.State;
			this.OnButtonChangeState ( ev );
		}

		#endregion

		#region Virtual functions	
		/// <summary>
		/// Calculate the dimensions of the drawing rectangles
		/// </summary>
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
		}
		#endregion
		
		#region Fire events
		/// <summary>
		/// Event for the state changed
		/// </summary>
		public event ButtonChangeState ButtonChangeState;
		
		/// <summary>
		/// Method for call the delagetes
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnButtonChangeState( LBButtonEventArgs e )
	    {
	        if( this.ButtonChangeState != null )
	            this.ButtonChangeState( this, e );
	    }		
		#endregion
	}

	#region Classes for event and event delagates args
	
	#region Event args class
	/// <summary>
	/// Class for events delegates
	/// </summary>
	public class LBButtonEventArgs : EventArgs
	{
		private LBButton.ButtonState state;
			
		public LBButtonEventArgs()
		{			
		}
	
		public LBButton.ButtonState State
		{
			get { return this.state; }
			set { this.state = value; }
		}
	}
	#endregion
	
	#region Delegates
	public delegate void ButtonChangeState ( object sender, LBButtonEventArgs e );
	#endregion
	
	#endregion
}
