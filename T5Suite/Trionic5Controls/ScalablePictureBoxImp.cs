using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

/// <summary>
/// This is public domain software - that is, you can do whatever you want
/// with it, and include it software that is licensed under the GNU or the
/// BSD license, or whatever other licence you choose, including proprietary
/// closed source licenses.  I do ask that you leave this lcHeader in tact.
///
/// Trionic5Controls.NET makes use of this control to display pictures.
/// Please visit <a href="http://www.Trionic5Controls.net/en/">http://www.Trionic5Controls.net/en/</a>
/// </summary>
namespace Trionic5Controls
{
	/// <summary>
    /// A scrollable, zoomable and scalable picture box.
    /// It is data aware, and creates zoom rate context menu dynamically.
    /// However, clients of this control should use this control indirectly by using PictureBoxMediator
    /// So we declare this control as internal class
    /// </summary>
    internal partial class ScalablePictureBoxImp : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// The name of fit width ToolStripMenuItem
        /// </summary>
        const String FIT_WIDTH_MENU_ITEM_NAME = "fitWidthScaleToolStripMenuItem";

        /// <summary>
        /// The name of show whole ToolStripMenuItem
        /// </summary>
        const String SHOW_WHOLE_MENU_ITEM_NAME = "showWholeToolStripMenuItem";

        /// <summary>
        /// Maximum scale percent(100%)
        /// </summary>
        const int MAX_SCALE_PERCENT = 300;

        /// <summary>
        /// Zoom in cursor
        /// </summary>
        private static Cursor zoomInCursor = null;

        /// <summary>
        /// Zoom out cursor
        /// </summary>
        private static Cursor zoomOutCursor = null;

        /// <summary>
        /// Picture size mode
        /// </summary>
        private PictureBoxSizeMode pictureBoxSizeMode = PictureBoxSizeMode.Normal;

        /// <summary>
        /// Need dispose image when new image is set
        /// </summary>
        private bool needDisposeImage = true;

        /// <summary>
        /// Scale percentage of picture box in zoom mode
        /// </summary>
        private int currentScalePercent = MAX_SCALE_PERCENT;

        /// <summary>
        /// Last selected menu item name
        /// </summary>
        private String lastSelectedMenuItemName = SHOW_WHOLE_MENU_ITEM_NAME;

        /// <summary>
        /// Fit width image
        /// </summary>
        Image fitWidthImage;

        /// <summary>
        /// Show whole image
        /// </summary>
        Image showWholeImage;

        /// <summary>
        /// Show actual image
        /// </summary>
        Image showActualSizeImage;

        /// <summary>
        /// delegate of zoom rate changed handler
        /// </summary>
        /// <param name="zoomRate">current zoom rate</param>
        /// <param name="isFullPictureShown">true if the whole picture is shown</param>
        public delegate void ZoomRateChangedEventHandler(int zoomRate, bool isFullPictureShown);

        /// <summary>
        /// zoom rate changed event
        /// </summary>
        public event ZoomRateChangedEventHandler ZoomRateChangedEvent;

        /// <summary>
        /// delegate of PictureBox painted event handler
        /// </summary>
        /// <param name="visibleAreaRect">currently visible area of picture</param>
        /// <param name="pictureBoxRect">picture box area</param>
        public delegate void PictureBoxPaintedEventHandler(Rectangle visibleAreaRect, Rectangle pictureBoxRect, Graphics graphics);

        /// <summary>
        /// PictureBox painted event
        /// </summary>
        public event PictureBoxPaintedEventHandler PictureBoxPaintedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
		public ScalablePictureBoxImp()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            // Read icon images
            fitWidthImage = Util.GetImageFromScalablePictureBoxEmbeddedResource("Trionic5Controls.showFitWidth.gif");
            showWholeImage = Util.GetImageFromScalablePictureBoxEmbeddedResource("Trionic5Controls.showWhole.gif");
            showActualSizeImage = Util.GetImageFromScalablePictureBoxEmbeddedResource("Trionic5Controls.showActualSize.gif");

            // read cursors
            zoomInCursor = Util.CreateCursorFromFile("Trionic5Controls.ZoomIn32.cur");
            zoomOutCursor = Util.CreateCursorFromFile("Trionic5Controls.ZoomOut32.cur");

            // set size mode of picture box to zoom mode
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // Enable auto scroll of this control
            this.AutoScroll = true;
        }

        /// <summary>
        /// Get picture box control
        /// </summary>
        [Bindable(false)]
        public PictureBox PictureBox
        {
            get { return this.pictureBox; }
        }

        /// <summary>
        /// Need dispose image when new image is set
        /// </summary>
        [Bindable(true), DefaultValue(true)]
        public bool NeedDisposeImage
        {
            get { return this.needDisposeImage; }
            set { this.needDisposeImage = value; }
        }

		/// <summary>
		/// Image in picture box
		/// </summary>
        [Bindable(true)]
        public Image Picture
		{
            get { return this.pictureBox.Image; }
			set
			{
                if (this.pictureBox.Image != null && this.NeedDisposeImage)
                {
                    this.pictureBox.Image.Dispose();
                }
                this.pictureBox.Image = value;
                ScalePictureBoxToFit();
                RefreshContextMenuStrip();
                this.CurrentScalePercent = GetFitWidthScalePercent();
                this.Invalidate();
            }
		}

        /// <summary>
        /// Image size mode
        /// </summary>
        [Bindable(true), DefaultValue(PictureBoxSizeMode.Zoom)]
        public PictureBoxSizeMode ImageSizeMode
		{
            get { return this.pictureBoxSizeMode; }
			set
			{
                this.pictureBoxSizeMode = value;
                this.ScalePictureBoxToFit();
			}
		}

        /// <summary>
        /// scroll picture programmatically by the event from PictureTracker
        /// </summary>
        /// <param name="xMovementRate">horizontal scroll movement rate which may be nagtive value</param>
        /// <param name="yMovementRate">vertical scroll movement rate which may be nagtive value</param>
        public void OnScrollPictureEvent(float xMovementRate, float yMovementRate)
        {
            // NOTICE : usage of Math.Abs(this.AutoScrollPosition.X) and Math.Abs(this.AutoScrollPosition.Y)
            // The get method of the Panel.AutoScrollPosition.X property and
            // the get method of the Panel.AutoScrollPosition.Y property return negative values.
            // However, positive values are required.
            // You can use the Math.Abs function to obtain a positive value from the Panel.AutoScrollPosition.X property and
            // the Panel.AutoScrollPosition.Y property
            int X = (int)(Math.Abs(this.AutoScrollPosition.X) + this.pictureBox.ClientRectangle.Width * xMovementRate);
            int Y = (int)(Math.Abs(this.AutoScrollPosition.Y) + this.pictureBox.ClientRectangle.Height * yMovementRate);
            this.AutoScrollPosition = new Point(X, Y);
        }

        /// <summary>
        /// Scale percentage for the picture box
        /// </summary>
        private int CurrentScalePercent
        {
            get { return this.currentScalePercent; }
            set { this.currentScalePercent = value; }
        }

        /// <summary>
        /// Scale picture box to fit to current control size and image size
        /// </summary>
		public void ScalePictureBoxToFit()
		{
            //return;
            if (this.Picture == null)
            {
                this.pictureBox.Width = this.ClientSize.Width;
                this.pictureBox.Height = this.ClientSize.Height;
                this.pictureBox.Left = 0;
                this.pictureBox.Top = 0;
                this.AutoScroll = false;
                this.CurrentScalePercent = GetMinScalePercent();
                //this.pictureBoxSizeMode = PictureBoxSizeMode.Zoom;
            }
            /*else if (this.pictureBoxSizeMode == PictureBoxSizeMode.Zoom ||
                    (this.Picture.Width <= this.ClientSize.Width && this.Picture.Height <= this.ClientSize.Height))
            {
                this.pictureBox.Width = Math.Min(this.ClientSize.Width, this.Picture.Width);
                this.pictureBox.Height = Math.Min(this.ClientSize.Height, this.Picture.Height);
                this.pictureBox.Top = (this.ClientSize.Height - this.pictureBox.Height) / 2;
                this.pictureBox.Left = (this.ClientSize.Width - this.pictureBox.Width) / 2;
                this.AutoScroll = false;
                this.CurrentScalePercent = GetMinScalePercent();
                this.pictureBoxSizeMode = PictureBoxSizeMode.Zoom;
            }*/
            else
            {
                this.pictureBox.Width = Math.Max(this.Picture.Width * this.CurrentScalePercent / 100, this.ClientSize.Width);
                this.pictureBox.Height = Math.Max(this.Picture.Height * this.CurrentScalePercent / 100, this.ClientSize.Height);

                // Centering picture box control
                int top = (this.ClientSize.Height - this.pictureBox.Height) / 2;
                int left = (this.ClientSize.Width - this.pictureBox.Width) / 2;

                if (top < 0)
                {
                    top = this.AutoScrollPosition.Y;
                }
                if (left < 0)
                {
                    left = this.AutoScrollPosition.X;
                }
                this.pictureBox.Left = left;
                this.pictureBox.Top = top;
                this.AutoScroll = true;
            }

            // set cursor for picture box
            SetCursor4PictureBox();
            this.pictureBox.Invalidate();

            // Raise zoom rate changed event
            if (ZoomRateChangedEvent != null)
            {
                bool isFullPictureShown = this.pictureBox.Width <= this.ClientSize.Width &&
                                          this.pictureBox.Height <= this.ClientSize.Height;
                ZoomRateChangedEvent(this.CurrentScalePercent, isFullPictureShown);
            }
        }

        /// <summary>
        /// Set cursor for the picture box
        ///     DefaultCursor:if need not scale picture,
        ///     ZoomOutCursor:if can zoom out picture,
        ///     ZoomInCursor:if can zoom in picture
        /// </summary>
        private void SetCursor4PictureBox()
        {
            if (this.Picture == null || this.ContextMenuStrip == null)
            {
                // returen default cursor
                this.pictureBox.Cursor = Cursors.Default;
            }
            else
            {
                if (this.pictureBoxSizeMode == PictureBoxSizeMode.Zoom)
                {
                    // return zoom in cursor if the picture is zoomed out
                    this.pictureBox.Cursor = zoomInCursor;
                }
                else
                {
                    // return zoom out cursor if the picture is zoomed in
                    this.pictureBox.Cursor = zoomOutCursor;
                }
            }
        }

        /// <summary>
        /// Resize picture box on resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnResize(object sender, System.EventArgs e)
		{
            ScalePictureBoxToFit();
            RefreshContextMenuStrip();
        }

        /// <summary>
        /// Scale current picture if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (this.Picture == null /*||
                (this.Picture.Width <= this.ClientSize.Width && this.Picture.Height <= this.ClientSize.Height)*/)
            {
                // do nothing if it is not needed to scale the picture
                return;
            }

            if (this.ImageSizeMode == PictureBoxSizeMode.Zoom)
            {
                this.ImageSizeMode = PictureBoxSizeMode.Normal;
                this.lastSelectedMenuItemName = MAX_SCALE_PERCENT.ToString();
                this.CurrentScalePercent = MAX_SCALE_PERCENT;
            }
            else
            {
                this.ImageSizeMode = PictureBoxSizeMode.Zoom;
                this.lastSelectedMenuItemName = SHOW_WHOLE_MENU_ITEM_NAME;
                this.CurrentScalePercent = GetMinScalePercent();
            }

            ScalePictureBoxToFit();

            // check last selected menu item
            CheckLastSelectedMenuItem();
        }

        /// <summary>
        /// Repaint picture box when its location changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_LocationChanged(object sender, EventArgs e)
        {
            this.pictureBox.Invalidate();
        }

        /// <summary>
        /// Get minimum scale percent of current image
        /// </summary>
        /// <returns>minimum scale percent</returns>
        private int GetMinScalePercent()
        {
            if ((this.Picture == null) ||
                (this.Picture.Width <= this.ClientSize.Width) && (this.Picture.Height <= this.ClientSize.Height))
            {
                return 100;// MAX_SCALE_PERCENT;
            }

            float minScalePercent = Math.Min((float)this.ClientSize.Width / (float)this.Picture.Width,
                                             (float)this.ClientSize.Height / (float)this.Picture.Height);
            //float minScalePercent = 0.1F; //<GS-31012011>
            return (int)(minScalePercent * 100.0f);
        }

        /// <summary>
        /// Get fit width scale percent of current image
        /// </summary>
        /// <returns>fit width scale percent which is bigger than minimum scale percent</returns>
        private int GetFitWidthScalePercent()
        {
            if (this.Picture == null)
            {
                return 100;
            }

            int fitWidthScalePercent = Math.Min(this.ClientSize.Width * 100 / this.Picture.Width, 100);
            return Math.Max(fitWidthScalePercent, GetMinScalePercent());
        }

        /// <summary>
        /// Refresh context menu strip according to current image
        /// </summary>
        private void RefreshContextMenuStrip()
        {
            int minScalePercent = GetMinScalePercent();
            /*if (minScalePercent == MAX_SCALE_PERCENT)
            {
                // no need popup context menu
                this.ContextMenuStrip = null;
            }
            else*/ // <GS-31012011>
            {
                this.pictureBoxContextMenuStrip.SuspendLayout();
                this.pictureBoxContextMenuStrip.Items.Clear();

                // add show whole menu item
                ToolStripMenuItem showWholeScaleMenuItem = CreateToolStripMenuItem(minScalePercent);
                showWholeScaleMenuItem.Name = SHOW_WHOLE_MENU_ITEM_NAME;
                showWholeScaleMenuItem.Text = "Show whole(" + minScalePercent + "%)";
                showWholeScaleMenuItem.Image = this.showWholeImage;
                showWholeScaleMenuItem.Checked = this.pictureBoxSizeMode == PictureBoxSizeMode.Zoom;
                this.pictureBoxContextMenuStrip.Items.Add(showWholeScaleMenuItem);

                // add scale to fit width menu item
                int fitWidthScalePercent = GetFitWidthScalePercent();
                ToolStripMenuItem fitWidthScaleMenuItem = CreateToolStripMenuItem(fitWidthScalePercent);
                fitWidthScaleMenuItem.Name = FIT_WIDTH_MENU_ITEM_NAME;
                fitWidthScaleMenuItem.Text = "Fit width(" + fitWidthScalePercent + "%)";
                fitWidthScaleMenuItem.Image = this.fitWidthImage;
                this.pictureBoxContextMenuStrip.Items.Add(fitWidthScaleMenuItem);

                // add other scale menu items
                for (int scale = minScalePercent / 10 * 10 + 10; scale <= MAX_SCALE_PERCENT; scale += 10)
                {
                    ToolStripMenuItem menuItem = CreateToolStripMenuItem(scale);
                    if (scale == 100)
                    {
                        menuItem.Image = this.showActualSizeImage;
                    }
                    this.pictureBoxContextMenuStrip.Items.Add(menuItem);
                }
                this.pictureBoxContextMenuStrip.ResumeLayout();
                this.ContextMenuStrip = this.pictureBoxContextMenuStrip;

                // check last selected menu item
                CheckLastSelectedMenuItem();
            }
            SetCursor4PictureBox();
        }

        /// <summary>
        /// Create a tool strip menu item with given scale percent
        /// </summary>
        /// <param name="scalePercent">the percentage to scale picture</param>
        /// <returns>a tool strip menu item</returns>
        private ToolStripMenuItem CreateToolStripMenuItem(int scalePercent)
        {
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Name = scalePercent.ToString();
            toolStripMenuItem.Text = scalePercent.ToString() + "%";
            toolStripMenuItem.Tag = scalePercent;
            toolStripMenuItem.Click += new EventHandler(toolStripMenuItem_Click);
            return toolStripMenuItem;
        }

        /// <summary>
        /// check last selected menu item
        /// </summary>
        private void CheckLastSelectedMenuItem()
        {
            // check the selected menu item
            foreach (ToolStripMenuItem menuItem in this.pictureBoxContextMenuStrip.Items)
            {
                menuItem.Checked = this.lastSelectedMenuItemName == menuItem.Name;
            }
        }

        /// <summary>
        /// Scale the picture box size with the scale percentage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedMenuItem = sender as ToolStripMenuItem;
            this.CurrentScalePercent = (int)selectedMenuItem.Tag;
            //this.pictureBoxSizeMode = PictureBoxSizeMode.Normal;
            //ImageSizeMode = PictureBoxSizeMode.Normal;

            ImageSizeMode = selectedMenuItem.Name == SHOW_WHOLE_MENU_ITEM_NAME ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.Normal;

            // Adjust picture box size again for fit picture box width to the scalable control width
            // because the client size may be changed.
            int currentFitWidthScalePercent = GetFitWidthScalePercent();
            if (selectedMenuItem.Name == FIT_WIDTH_MENU_ITEM_NAME && currentFitWidthScalePercent != this.CurrentScalePercent)
            {
                this.CurrentScalePercent = GetFitWidthScalePercent();
                ImageSizeMode = selectedMenuItem.Name == SHOW_WHOLE_MENU_ITEM_NAME ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.Normal;
            }

            this.lastSelectedMenuItemName = selectedMenuItem.Name;

            // check last selected menu item
            CheckLastSelectedMenuItem();
        }

        /// <summary>
        /// Raise pictureBox painted event for adjusting picture tracking control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (PictureBoxPaintedEvent != null)
            {
                Rectangle thisControlClientRect = this.ClientRectangle;
                thisControlClientRect.X -= this.AutoScrollPosition.X;
                thisControlClientRect.Y -= this.AutoScrollPosition.Y;
                PictureBoxPaintedEvent(thisControlClientRect, this.pictureBox.ClientRectangle, e.Graphics);
                
            }
            
            
        }
    }
}
