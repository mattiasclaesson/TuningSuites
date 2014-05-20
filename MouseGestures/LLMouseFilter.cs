using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using WinAPI.Hooks;
using WinAPI;
using System.Windows.Forms;

namespace MouseGestures {
  /// <summary>
  /// Captures mouse events necessary for mouse gesture recognition
  /// </summary>
  /// <remarks>
  /// LLMouseFilter uses Windows Hooks to capture mouse events
  ///</remarks>
  class LLMouseFilter : WindowsHook, IDisposable, IMouseMessageFilter {

    #region IMouseMaessageFilter Properties
    
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

    #region IMouseMessageFilter Events
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

    #region IDisposable implementation

    protected void Dispose(bool disposing) {
      if ( Installed )
        Uninstall();

      if ( disposing )
        GC.SuppressFinalize(this);
    }

    public void Dispose() {
      Dispose(true);
    }

    #endregion

    #region Constructors
    
    /// <summary>
    /// Initializes new instance of LLMouseFilter
    /// </summary>
    public LLMouseFilter()
      : base(HookType.WH_MOUSE)
		{
			// we provide our own callback function
			callbackFunc = new HookCallbackFunction(this.MouseHookCallback);

      Install();
    }

    #endregion

    #region Destructors

    ~LLMouseFilter()
		{
			Dispose( false );
		}

		#endregion

    /// <summary>
    /// MouseHook callback function. It recives notification about mouse events from the system and
    /// filter messages to be passed to the application.
    /// </summary>
    /// <param name="code">Message code.</param>
    /// <param name="wParam">Pointer to the wParam structure.</param>
    /// <param name="lParam">Pointer to the lParam structure.</param>
    /// <returns></returns>
    protected int MouseHookCallback(int code, IntPtr wParam, IntPtr lParam) {
      // according to the Platform SDK a code < 0 means skip
      if ( code < 0 )
        return CallNextHookEx(hhook, code, wParam, lParam);

      // if enabled fillter messages passed to the application
      if ( (code == (int)MessageCodes.HC_ACTION) && enabled) {
        int messageCode = wParam.ToInt32();
        
        switch ( messageCode ) {
          case (int)MessageCodes.WM_RBUTTONDOWN:
            RaiseRightButtonDownEvent();

            //dont let applicaton to recive WM_RBUTTONDOWN message
            return 1;
          case ( int )MessageCodes.WM_MOUSEMOVE:
            RaiseRightButtonMoveEvent();

            //let applicaton to recive WM_MOUSEMOVE message
            break;
          case ( int )MessageCodes.WM_RBUTTONUP:
            RaiseRightButtonUpEvent();

            //dont let applicaton to recive WM_RBUTTONUP message
            return 1;
        }
      }
        
      // Call the next hook in the chain
      return CallNextHookEx(hhook, code, wParam, lParam);
    }
  }
}
