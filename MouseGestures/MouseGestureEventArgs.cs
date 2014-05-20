using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace MouseGestures {
  /// <summary>
  /// Provides data for MouseGesture events
  /// </summary>
  public class MouseGestureEventArgs:EventArgs {
    /// <summary>
    /// Initializes new instance of MouseGestureEventArgs
    /// </summary>
    /// <param name="gesture">The type of the gesture performed.</param>
    /// <param name="beginning">The point, where user started the gesture.</param>
    public MouseGestureEventArgs(MouseGesture gesture, Point beginning):base() {
      Gesture = gesture;
      Beginning = beginning;
  }
    /// <summary>
  /// The type of the gesture performed.
    /// </summary>
    public MouseGesture Gesture;

    /// <summary>
    /// The point, where user started the gesture.
    /// </summary>
    public Point Beginning;
  }
}
