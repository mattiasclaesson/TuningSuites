using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using WinAPI;

namespace MouseGestures {
  /// <summary>
  /// Captures mouse events necessary for mouse gesture recognition
  /// </summary>
  /// <remarks>
  /// MouseMessageFilter implements <see cref="IMessageFilter">IMessageFilter</see> interface, it provides functionality
  /// for the whole application.
  ///</remarks>
  class ManagedMouseFilter: IMessageFilter,IMouseMessageFilter {

    #region Consturctors

    public ManagedMouseFilter() {
      Application.AddMessageFilter(this);
    }

    #endregion

    #region Destructors

    ~ManagedMouseFilter() {
      Application.RemoveMessageFilter(this);
    }

    #endregion

    #region Enabled property
    /// <summary>
    /// Gets or sets property indicating whether MouseMessageFilter is active and filtering messages
    /// </summary>
    public bool Enabled {
      get {
        return enabled;
      }
      set {
        enabled = value;
      }
    }
    private bool enabled;
    #endregion

    #region IMessageFilter Members

    /// <summary>
    /// Implemets <see cref="IMessageFilter">IMessageFilter</see> interface
    /// </summary>
    /// <remarks>
    /// This function filters WM_RBUTTONDOWN and WM_RBUTTONUP messages
    /// before they are delivered to application. WM_MOUSEMOVE message is recorded
    /// only. When these messages appear the appropriate events of MouseMessageFilter are
    /// rised. 
    /// </remarks>
    public bool PreFilterMessage(ref Message m) {
      if ( enabled ) {

        if ( m.Msg == (int)MessageCodes.WM_MOUSEMOVE ) {
          RaiseRightButtonMoveEvent();

          //pass WM_MOUSEMOVE message to the application
          return false;
        }
        else if ( m.Msg == (int)MessageCodes.WM_RBUTTONDOWN ) {
          RaiseRightButtonDownEvent();

          return true;
        }
        else if ( m.Msg == (int)MessageCodes.WM_RBUTTONUP ) {
          RaiseRightButtonUpEvent();

          return true;
        }
      }
      return false;
    }
    #endregion

    #region MessageFilterEvents

    /// <summary>
    /// Occures when right mouse button is pressed
    /// </summary>
    public event MouseFilterEventHandler RightButtonDown;
    private void RaiseRightButtonDownEvent() {
      MouseFilterEventHandler temp = RightButtonDown;
      if ( temp != null ) {
        temp(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Occures when right mouse button is relesed
    /// </summary>
    public event MouseFilterEventHandler RightButtonUp;
    private void RaiseRightButtonUpEvent() {
      MouseFilterEventHandler temp = RightButtonUp;
      if ( temp != null ) {
        temp(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Occures when mouse is moved
    /// </summary>
    public event MouseFilterEventHandler MouseMove;
    private void RaiseRightButtonMoveEvent() {
      MouseFilterEventHandler temp = MouseMove;
      if ( temp != null ) {
        temp(this, EventArgs.Empty);
      }
    }

    #endregion
  }
}
