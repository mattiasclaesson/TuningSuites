using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ctlLEDRadioButton {
	/// <summary>
	/// Summary description for CustomControl1.
	/// </summary>
	/// 		
	public class LEDradioButton : System.Windows.Forms.RadioButton {
		//[Appearance(Button)]
		private Color m_color1 = Color.LightGreen;  //default top color
		private Color m_color2 = Color.DarkBlue;   // default bottom color
		private Color m_ledcolor = Color.Green;    // default LED color
		private int m_width = 10;
		private int m_height = 40;
		private int m_offset = 5; // # of pixels of offset the LED from the edge of the control
		private int m_color1Transparent = 64; // transparency degree 
		// (applies to the 1st color)
		private int m_color2Transparent = 64; // transparency degree 
		private bool onOff = false;
		public LEDradioButton(){
		base.Appearance = Appearance.Button;
	}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]

		public new Appearance Appearance {  
		get{return Appearance.Button; }
			
		}
		[
		Bindable(true),
		Category("Appearance"),
		DefaultValue(10),
		Description("Sets the width of the LED in % of control")
		]
		public int LEDWidth{
			get {return m_width;}
			set {m_width = value; Invalidate();}
		}
		[
		Bindable(true),
		Category("Appearance"),
		DefaultValue(40),
		Description("Sets the height of the LED in % of the control")
		]
		public int LEDHeight{
			get {return m_height;}
			set {m_height = value; Invalidate();}
		}
		[
		Category("Appearance"),
		Description("Sets the Color for the top gradient color"),
		Bindable(true)
		]

		public Color topColor {
			get { return m_color1; }
			set { m_color1 = value; Invalidate(); }
		}
		[
		Category("Appearance"),
		Description("Sets the Color for the bottom gradient color"),
		Bindable(true),
		]
		public Color bottomColor {
			get { return m_color2; }
			set { m_color2 = value; Invalidate(); }
		}
		[
		Category("Appearance"),
		Description("Sets the Transparency value for the top gradient color"),
		Bindable(true)
		]
		public int TopTransparent {
			get { return m_color1Transparent; }
			set { m_color1Transparent = value; Invalidate(); }
		}
		[
		Category("Appearance"),
		Description("Sets the Transparency value for the bottom gradient color"),
		Bindable(true)
		]
		public int BottomTransparent {
			get { return m_color2Transparent; }
			set { m_color2Transparent = value; Invalidate(); }
		}
		[
		Category("Appearance"),
		Description("Sets the color for the LED"),
		Bindable(true)
		]
		public Color LEDColor{
			get{return m_ledcolor;}
			set { m_ledcolor = value; Invalidate();}

		}
		[
		Category("Appearance"),
		Description("Sets the number of pixels to offset the LED from the edge of the control"),
		DefaultValue(5),
		Bindable(true)
		]
		public int LEDOffset{
			get{return m_offset;}
			set{m_offset = value;Invalidate();}
		}
		

		protected override void OnCheckedChanged(System.EventArgs e){
			base.OnCheckedChanged(e);
			onOff = this.Checked;
			Invalidate();

		}
		protected override void OnClick(System.EventArgs e){
			base.OnClick(e);
			Invalidate();
		}
		protected override void OnPaint(PaintEventArgs pe) {
			// TODO: Add custom paint code here

			// Calling the base class OnPaint
			Color c3 = m_ledcolor;
			base.OnPaint(pe);
			// Create two semi-transparent colors
			Color c1 = Color.FromArgb
				(m_color1Transparent , m_color1);
			Color c2 = Color.FromArgb
				(m_color2Transparent , m_color2);
			Brush b = new System.Drawing.Drawing2D.LinearGradientBrush
				(ClientRectangle,c1, c2, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
			if(onOff == true){
				c3 = System.Windows.Forms.ControlPaint.LightLight(m_ledcolor);
			}
			else
				c3 = System.Windows.Forms.ControlPaint.Dark(m_ledcolor);
			SolidBrush sb = new SolidBrush(c3);
			pe.Graphics.FillRectangle (b, ClientRectangle);
			Pen blackPen = new Pen(Color.Black , 2);
			Point point1 = new Point( m_offset,  m_offset);
			Point point2 = new Point(m_offset, triHeight());
			Point point3 = new Point(triWidth(),m_offset);
			Point point4 = new Point(m_offset,m_offset);
			Point[] triPoints = {
				point1,
				point2,
				point3,
				point4,
			};
			// Draw polygon to screen.
			pe.Graphics.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.HighQuality;//Add this line
			pe.Graphics.DrawPolygon(blackPen, triPoints);
			pe.Graphics.FillPolygon(sb,triPoints,System.Drawing.Drawing2D.FillMode.Winding);

			b.Dispose();
		}
		virtual protected int triHeight(){
			float x;
			x = this.Height * ((float)m_height/100);
			return (int)x;
		}
		virtual protected int triWidth(){
			float x;
			x = this.Width * ((float)m_width/100);
			return (int)x;
		}

	}
}

