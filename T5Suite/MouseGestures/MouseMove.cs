using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MouseGestures {
  public class MouseMoveSegment {
    
    /// <summary>
    /// Enum for the direction of the part of the mouse move
    /// </summary>
    public enum SegmentDirection {
      Unknown,
      Left,
      Right,
      Up,
      Down
    };

    /// <summary>
    /// Defines maximum angle error in degrees. If the angle error is greater then
    /// maxAngleError then the direction is not recognized.
    /// </summary>
    /// <remarks>
    /// It must be positive number lesser then 45</remarks>
    //TODO: consider moving the hardcoded value to a propery
    private const double maxAngleError = 30;

    /// <summary>
    /// Gets Direction of the MouseMoveSegment
    /// </summary>
    public SegmentDirection Direction {
      get {
        return direction;
      }
    }
    private SegmentDirection direction;
    
    /// <summary>
    /// Recognizes direction of the MouseMoveSegment
    /// </summary>
    /// <param name="deltaX">Lenght of movement in the horizontal direction</param>
    /// <param name="deltaY">Lenght of movement in the vertical direction</param>
    /// <returns>Segment direction, if fails returns SegmentDirection.Unknown</returns>
    private static SegmentDirection GetDirection(int deltaX, int deltaY) {
      double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

      double sin = deltaX / length;
      double cos = deltaY / length;

      double angle = Math.Asin(Math.Abs(sin)) * 180 / Math.PI;

      if ( (sin >= 0) && (cos < 0) )
        angle = 180 - angle;
      else if ( (sin < 0) && (cos < 0) )
        angle = angle + 180;
      else if( (sin < 0) && (cos >= 0) )
        angle = 360 - angle;

      //direction recognition
      if ( (angle > 360 - maxAngleError) || (angle < 0 + maxAngleError) )
        return SegmentDirection.Up;
      else if ( (angle > 90 - maxAngleError) && (angle < 90 + maxAngleError) )
        return SegmentDirection.Right;
      else if ( (angle > 180 - maxAngleError) && (angle < 180 + maxAngleError) )
        return SegmentDirection.Down;
      else if ( (angle > 270 - maxAngleError) && (angle < 270 + maxAngleError) )
        return SegmentDirection.Left;
      else return SegmentDirection.Unknown;
    }

    /// <summary>
    /// Creates MouseMoveSegment and finds out it's Direction property
    /// </summary>
    /// <param name="p1">Starting point</param>
    /// <param name="p2">Final point</param>
    public MouseMoveSegment(Point p1, Point p2) {
      direction = GetDirection(p2.X - p1.X, p1.Y - p2.Y);
    }
  }
}
