using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
    /// Picture tracker which is used to scrolling original picture in the ScalablePictureBox control.
    /// It is internal class of the scalable picture box control
    /// </summary>
    internal partial class PictureTracker : UserControl
    {
        /// <summary>
        /// image thumbnail for tracking image.
        /// We make thumbnail of original picture for performance consideration
        /// instead of using original picture for tracking.
        /// </summary>
        private Image thumbnail = null;

        /// <summary>
        /// hand cursor for dragging highlighted picture area
        /// </summary>
        private Cursor handCursor = null;

        /// <summary>
        /// tracker cursor for dragging picture tracker
        /// </summary>
        private Cursor trackerCursor = null;

        /// <summary>
        /// current zoom rate
        /// </summary>
        private int currentZoomRate = 0;

        /// <summary>
        /// a transperent brush for shadowing invisible part of picture.
        /// It uses sliver color for shadowing picture
        /// </summary>
        private Brush tranparentBrush = new SolidBrush(Color.FromArgb(180, 0xc0, 0xc0, 0xc0));

        /// <summary>
        /// rectangle area where to draw the thumbnail picture
        /// </summary>
        private Rectangle pictureDestRect;

        /// <summary>
        /// rectangle area where to draw part of visible picture
        /// </summary>
        private Rectangle highlightingRect;

        /// <summary>
        /// indicate if is the highlight rectangle dragging
        /// </summary>
        private bool isDragging = false;

        /// <summary>
        /// last mouse position in dragging highlight rectangle
        /// </summary>
        private Point lastMousePosOfDragging = new Point(0, 0);

        /// <summary>
        /// delegate of PictureTracker control closed event handler
        /// </summary>
        public delegate void PictureTrackerClosedHandler();

        /// <summary>
        /// PictureTracker control closed event
        /// </summary>
        public event PictureTrackerClosedHandler PictureTrackerClosed;

        /// <summary>
        /// Scroll picture event handler
        /// </summary>
        /// <param name="xMovementRate">horizontal scroll movement rate which may be nagtive value</param>
        /// <param name="yMovementRate">vertical scroll movement rate which may be nagtive value</param>
        public delegate void ScrollPictureEventHandler(float xMovementRate, float yMovementRate);

        /// <summary>
        /// Scroll picture event to ask ScalablePictureBox to scroll picture
        /// </summary>
        public event ScrollPictureEventHandler ScrollPictureEvent;

        /// <summary>
        /// Font for drawing zoom rate text
        /// </summary>
        Font zoomRateFont = new System.Drawing.Font("Times New Roman", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        /// <summary>
        /// constructor
        /// </summary>
        public PictureTracker()
        {
            InitializeComponent();

            handCursor = Util.CreateCursorFromFile("Trionic5Controls.Hand.cur");
            trackerCursor = Util.CreateCursorFromFile("Trionic5Controls.Tracker.cur");
        }

        /// <summary>
        /// image for tracking
        /// </summary>
        public Image Picture
        {
            set
            {
                if (this.thumbnail != null)
                {
                    // dispose previous thumbnail image
                    this.thumbnail.Dispose();
                }

                if (value != null)
                {
                    // adjust destination rectangle area to show thumbnail picture
                    Rectangle srcRect = this.picturePanel.ClientRectangle;
                    srcRect.X += 1;
                    srcRect.Y += 1;
                    srcRect.Width -= 2;
                    srcRect.Height -= 2;

                    thumbnail = Util.CreateThumbnail(value, srcRect.Height);

                    pictureDestRect = Util.ScaleToFit(this.thumbnail, srcRect, false);
                    highlightingRect = new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    this.thumbnail = null;
                }
            }
        }

        /// <summary>
        /// zoom rate of current image
        /// </summary>
        public int ZoomRate
        {
            get { return this.currentZoomRate; }
            set
            {
                currentZoomRate = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The original picture is repainted. So we redraw picture tracker to
        /// reflect current highlight picture area.
        /// This method has been rewritten by Jakub.
        /// </summary>
        /// <param name="showingRect">currently visible area of original picture</param>
        /// <param name="pictureBoxRect">picture box area of original picture is shown</param>
        public void OnPictureBoxPainted(Rectangle showingRect, Rectangle pictureBoxRect)
        {
            Region regionToInvalidate;
            if (highlightingRect.IsEmpty)
            {
                //After start or picture change redraw the entire thumbnail.
                regionToInvalidate = new Region(picturePanel.ClientRectangle);
            }
            else
            {
                // Redraw the thumbnail part covered till now.
                regionToInvalidate = new Region(highlightingRect);
            }
            float widthScale = (float)showingRect.Width / (float)pictureBoxRect.Width;
            float xPosScale = (float)showingRect.X / (float)pictureBoxRect.Width;
            float heightScale = (float)showingRect.Height / (float)pictureBoxRect.Height;
            float yPosScale = (float)showingRect.Y / (float)pictureBoxRect.Height;
            highlightingRect = new Rectangle((int)(this.pictureDestRect.X + this.pictureDestRect.Width * xPosScale),
            (int)(this.pictureDestRect.Y + this.pictureDestRect.Height * yPosScale),
            (int)(this.pictureDestRect.Width * widthScale),
            (int)(this.pictureDestRect.Height * heightScale));

            regionToInvalidate.Union(highlightingRect); // Also redraw the part now highlighted.

            // Redraw only old and new highlighted rectangles
            picturePanel.Invalidate(regionToInvalidate); 
        }

        /// <summary>
        /// override OnPaint method to draw border of the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            // draw control border
            Rectangle borderRect = this.ClientRectangle;
            borderRect.Width -= 1;
            borderRect.Height -= 1;
            e.Graphics.DrawRectangle(Pens.Navy, borderRect);

            // draw zoom rate text
            e.Graphics.DrawString("Zoom rate:" + ZoomRate + "%", zoomRateFont, Brushes.Navy, 3, 3);
        }

        /// <summary>
        /// Paint thumbnail image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturePanel_Paint(object sender, PaintEventArgs e)
        {
            if (this.thumbnail == null)
            {
                // do nothing if thumbnail image is null
                return;
            }

            // draw thumbnail image
            e.Graphics.DrawImage(this.thumbnail, this.pictureDestRect);

            // adjust highlighting region of visible picture area
            Region highlightRegion = new Region(this.pictureDestRect);
            if (highlightingRect.Width > 0 && highlightingRect.Height > 0)
            {
                highlightRegion.Exclude(highlightingRect);
            }
            e.Graphics.FillRegion(tranparentBrush, highlightRegion);
        }

        /// <summary>
        /// close the PictureTracker control when close button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Enabled = false;
            if (PictureTrackerClosed != null)
            {
                PictureTrackerClosed();
            }
        }

        /// <summary>
        /// begin to drag highlight rectangle if mouse is down within the highlight rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.highlightingRect.Contains(e.X, e.Y))
            {
                isDragging = true;
                lastMousePosOfDragging = new Point(e.X, e.Y);
            }
        }

        /// <summary>
        /// fire scroll picture event when highlight rectangle is dragged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollPictureEvent != null && isDragging &&
                (lastMousePosOfDragging.X != e.X || lastMousePosOfDragging.Y != e.Y))
            {
                int offsetX = e.X - lastMousePosOfDragging.X;
                int offsetY = e.Y - lastMousePosOfDragging.Y;
                lastMousePosOfDragging = new Point(e.X, e.Y);

                // 1.Calculate horizontal and vertical mouse movement rates relative to the pictureDestRect
                //   the mouse movement rates may be nagtive value if mouse moved to left or up
                // 2.Raise ScrollPictureEvent to scroll actual picture in the ScalablePictureBox
                float xMovementRate = (float)offsetX / (float)pictureDestRect.Width;
                float yMovementRate = (float)offsetY / (float)pictureDestRect.Height;
                ScrollPictureEvent(xMovementRate, yMovementRate);
            }

            // use hand dragging cursor if mouse mode is dragging mouse or
            // mouse is within highlighting rectangle
            if (isDragging || this.highlightingRect.Contains(e.X, e.Y))
            {
                this.Cursor = this.handCursor;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// end dragging highlight rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturePanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        /// <summary>
        /// use tracker cursor if mouse is within this control.
        /// Notice: We always use hand cursor in dragging mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureTracker_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Cursor = this.handCursor;
            }
            else
            {
                this.Cursor = this.trackerCursor;
            }
        }

        /// <summary>
        /// use default cursor if mouse is within close button area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// adjust size of picturePanel when this control size is changed,
        /// because the size of picturePanel could not resized properly on different
        /// language version of OSs(e.g. Windows XP English and Windows XP Japanese)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Rectangle borderRect = this.ClientRectangle;

            const int MSG_HEIGHT = 18;
            const int OFFSET = 5;
            this.picturePanel.Location = new Point(OFFSET, MSG_HEIGHT);
            this.picturePanel.Width = this.ClientRectangle.Width - OFFSET * 2;
            this.picturePanel.Height = this.ClientRectangle.Height - (MSG_HEIGHT + OFFSET);
        }
    }
}
