using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MouseGestures {
  /// <summary>
  /// Defines interface that provides necessary mouse events capturing functionality for
  /// MouseGestures component
  /// </summary>
  public interface IMouseMessageFilter {
    /// <summary>
    /// Gets or set whether IMouseMessageFilter is enabled;
    /// </summary>
    bool Enabled {
      get;
      set;
    }

    event MouseFilterEventHandler RightButtonDown;
    event MouseFilterEventHandler RightButtonUp;
    event MouseFilterEventHandler MouseMove;
  }

  public delegate void MouseFilterEventHandler(object sender, EventArgs e);
}
