using System;
using System.Collections.Generic;
using System.Text;

namespace MouseGestures {

  /// <summary>
  /// Enum defining gesture types.
  /// </summary>
  /// <remarks>
  /// Complex gestures are composed by 2 simple gestures (Up, Right, Down, Left).
  /// Eg.: UpRight = Up + 2*Right
  /// </remarks>
  [FlagsAttribute]
  public enum MouseGesture {
    Unknown = 0,

    Up = 1,
    //Up2nd = 2         up is second 
    
    Right = 4,
    //Right2nd = 8
    
    Down = 16,
    //Down2nd = 32
    
    Left = 64,
    //Left2nd = 128

    UpRight = 9,
    UpDown = 33,
    UpLeft = 129,

    RightUp = 6,
    RightDown = 36,
    RightLeft = 132,

    DownUp = 18,
    DownRight = 24,
    DownLeft = 144,

    LeftUp = 66,
    LeftRight = 72,
    LeftDown = 96
  }
}
