using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
    /// Front end control of the scrollable, zoomable and scalable picture box.
    /// It is a facade and mediator of ScalablePictureBoxImp control and PictureTracker control.
    /// An application should use this control for showing picture
    /// instead of using ScalablePictureBox control directly.
    /// </summary>
    public partial class ScalablePictureBox : UserControl
    {
        /// <summary>
        /// delegate of PictureBox painted event handler
        /// </summary>
        /// <param name="visibleAreaRect">currently visible area of picture</param>
        /// <param name="pictureBoxRect">picture box area</param>
        //public delegate void PictureBoxPaintedEventHandlerEx(Rectangle visibleAreaRect, Rectangle pictureBoxRect, Graphics graphics);

        /// <summary>
        /// PictureBox painted event
        /// </summary>
       // public event ScalablePictureBox.PictureBoxPaintedEventHandlerEx PictureBoxPaintedEvent;

        /// <summary>
        /// indicating mouse dragging mode of picture tracker control
        /// </summary>
        private bool isDraggingPictureTracker = false;

        /// <summary>
        /// last mouse position of mouse dragging
        /// </summary>
        Point lastMousePos;

        /// <summary>
        /// the new area where the picture tracker control to be dragged
        /// </summary>
        Rectangle draggingRectangle;

        /// <summary>
        /// Constructor
        /// </summary>
        public ScalablePictureBox()
        {
            InitializeComponent();

            this.pictureTracker.BringToFront();

            // enable double buffering
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            // register event handler for events from ScalablePictureBox
            this.scalablePictureBoxImp.PictureBoxPaintedEvent += new ScalablePictureBoxImp.PictureBoxPaintedEventHandler(scalablePictureBoxImp_PictureBoxPaintedEvent);
            this.scalablePictureBoxImp.ZoomRateChangedEvent += new ScalablePictureBoxImp.ZoomRateChangedEventHandler(this.scalablePictureBox_ZoomRateChanged);

            // register event handler for events from PictureTracker
            this.pictureTracker.ScrollPictureEvent += new PictureTracker.ScrollPictureEventHandler(this.scalablePictureBoxImp.OnScrollPictureEvent);
            this.pictureTracker.PictureTrackerClosed += new PictureTracker.PictureTrackerClosedHandler(this.pictureTracker_PictureTrackerClosed);
        }

        void scalablePictureBoxImp_PictureBoxPaintedEvent(Rectangle visibleAreaRect, Rectangle pictureBoxRect, Graphics graphics)
        {
            //Console.WriteLine("scalablePictureBoxImp_PictureBoxPaintedEvent");
            this.pictureTracker.OnPictureBoxPainted(visibleAreaRect, pictureBoxRect);
            //Console.WriteLine("this.pictureTracker.OnPictureBoxPainted");
            /*if (PictureBoxPaintedEvent != null)
            {

                Rectangle thisControlClientRect = this.ClientRectangle;
                thisControlClientRect.X -= this.AutoScrollPosition.X;
                thisControlClientRect.Y -= this.AutoScrollPosition.Y;
                PictureBoxPaintedEvent(thisControlClientRect, this.ClientRectangle, graphics);
            }
            Console.WriteLine("PictureBoxPaintedEvent");*/
            
        }

        /// <summary>
        /// Set a picture to show in ScalablePictureBox control 
        /// </summary>
        public Image Picture
        {
            set
            {
                this.scalablePictureBoxImp.Picture = value;
                this.pictureTracker.Picture = value;
                this.scalablePictureBoxImp.ScalePictureBoxToFit();
            }
        }

        /// <summary>
        /// Get picture box control
        /// </summary>
        [Bindable(false)]
        public PictureBox PictureBox
        {
            get { return this.scalablePictureBoxImp.PictureBox; }
        }

        /// <summary>
        /// Notify current scale percentage to PictureTracker control if current picture is
        /// zoomed in, or hide PictureTracker control if current picture is shown fully.
        /// </summary>
        /// <param name="zoomRate">zoom rate of picture</param>
        /// <param name="isWholePictureShown">true if the whole picture is shown</param>
       private void scalablePictureBox_ZoomRateChanged(int zoomRate, bool isWholePictureShown)
        {
            if (isWholePictureShown)
            {
                this.pictureTracker.Visible = false;
                this.pictureTracker.Enabled = false;
            }
            else
            {
                this.pictureTracker.Visible = true;
                this.pictureTracker.Enabled = true;
                this.pictureTracker.ZoomRate = zoomRate;
            }
        }

        /// <summary>
        /// Inform ScalablePictureBox control to show picture fully.
        /// </summary>
        private void pictureTracker_PictureTrackerClosed()
        {
            this.scalablePictureBoxImp.ImageSizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Draw a reversible rectangle
        /// </summary>
        /// <param name="rect">rectangle to be drawn</param>
        private void DrawReversibleRect(Rectangle rect)
        {
            // Convert the location of rectangle to screen coordinates.
            rect.Location = PointToScreen(rect.Location);

            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rect, Color.Navy, FrameStyle.Thick);
        }

        /// <summary>
        /// begin to drag picture tracker control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureTracker_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggingPictureTracker = true;    // Make a note that we are dragging picture tracker control

            // Store the last mouse poit for this rubber-band rectangle.
            lastMousePos.X = e.X;
            lastMousePos.Y = e.Y;

            // draw initial dragging rectangle
            draggingRectangle = this.pictureTracker.Bounds;
            DrawReversibleRect(draggingRectangle);
        }

        /// <summary>
        /// dragging picture tracker control in mouse dragging mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureTracker_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingPictureTracker)
            {
                // caculating next candidate dragging rectangle
                Point newPos = new Point(draggingRectangle.Location.X + e.X - lastMousePos.X,
                                         draggingRectangle.Location.Y + e.Y - lastMousePos.Y);
                Rectangle newPictureTrackerArea = draggingRectangle;
                newPictureTrackerArea.Location = newPos;

                // saving current mouse position to be used for next dragging
                this.lastMousePos = new Point(e.X, e.Y);

                // dragging picture tracker only when the candidate dragging rectangle
                // is within this ScalablePictureBox control
                if (this.ClientRectangle.Contains(newPictureTrackerArea))
                {
                    // removing previous rubber-band frame
                    DrawReversibleRect(draggingRectangle);

                    // updating dragging rectangle
                    draggingRectangle = newPictureTrackerArea;

                    // drawing new rubber-band frame
                    DrawReversibleRect(draggingRectangle);
                }
            }
        }

        /// <summary>
        /// end dragging picture tracker control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureTracker_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDraggingPictureTracker)
            {
                isDraggingPictureTracker = false;

                // erase dragging rectangle
                DrawReversibleRect(draggingRectangle);

                // move the picture tracker control to the new position
                this.pictureTracker.Location = draggingRectangle.Location;
            }
        }

        /// <summary>
        /// relocate picture box at bottom right corner when the control size changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            int x = this.ClientSize.Width - this.pictureTracker.Width - 20;
            int y = this.ClientSize.Height - this.pictureTracker.Height - 20;
            this.pictureTracker.Location = new Point(x, y);
        }
    }
}
