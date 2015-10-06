using System;
using System.Collections.Generic;
using System.Text;

namespace WinAPI {
  /// <summary>
  /// Defines Window Messages codes
  /// </summary>
  /// <remarks>
  /// Window Messages codes are defined in the WinUser.h file (Platform SDK)
  /// </remarks>
  public enum MessageCodes:int {
    /// <summary>
    /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
    /// </summary>
    WM_RBUTTONDOWN = 0x0204,
    /// <summary>
    /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
    /// </summary>
    WM_RBUTTONUP = 0x0205,

    /// <summary>
    /// The WM_MOUSEMOVE message is posted to a window when the cursor moves.
    /// </summary> 
    WM_MOUSEMOVE = 0x0200,

    HC_ACTION = 0
  }
}
