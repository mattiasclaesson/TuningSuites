using System;
using System.Runtime.InteropServices;

namespace WinAPI.Hooks
{
  /// <summary>
  /// Base class for working with Windows hooks
  /// </summary>
	public class WindowsHook {

    #region Class Fields
    
    protected IntPtr hhook = IntPtr.Zero;
		protected HookCallbackFunction callbackFunc = null;
		protected HookType hookType;

    #endregion

    #region Properties

    /// <summary>
    /// Gets property indication whether the hook is installed into the system's hook chain
    /// </summary>
    public bool Installed {
      get {
        return hhook != IntPtr.Zero;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates new hook of specified type with gereral handling function
    /// </summary>
    /// <param name="hook">The type of the hook.</param>
    public WindowsHook(HookType hook) {
      hookType = hook;
      callbackFunc = new HookCallbackFunction(this.CoreHookProc);
    }

    /// <summary>
    /// Creates new hook of specified type with user defined handling function
    /// </summary>
    /// <param name="hook">The type of the hook.</param>
    /// <param name="func">Delegate of the user defined function</param>
    public WindowsHook(HookType hook, HookCallbackFunction func) {
      hookType = hook;
      callbackFunc = func;
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs after the hook has been invoked
    /// </summary>
    public event HookEventHandler HookInvoked;
    protected void RaiseHookInvoked(HookEventArgs e) {
      HookEventHandler temp = HookInvoked;
      if ( temp != null )
        temp(this, e);
    }
    public delegate void HookEventHandler(object sender, HookEventArgs e);

    #endregion 

    /// <summary>
    /// General hook handling function prototype
    /// </summary>
    /// <param name="code">Specifies the hook code passed to the current hook procedure.</param>
    /// <param name="wParam">Specifies the wParam code passed to the current hook procedure.</param>
    /// <param name="lParam">Specifies the lParam code passed to the current hook procedure.</param>
    /// <returns></returns>
    public delegate int HookCallbackFunction(int code, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// General Hook hadling function, only raises event with hook's parameters
    /// </summary>
    /// <param name="code">Specifies the hook code passed to the current hook procedure.</param>
    /// <param name="wParam">Specifies the hook wParam passed to the current hook procedure.</param>
    /// <param name="lParam">Specifies the hook lParam passed to the current hook procedure.</param>
    /// <returns>The value returned by CallNextHookEx function</returns>
    protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
		{
      // According to Platform SDK we must return the value returned by CallNextHookEx
			if (code < 0)
				return CallNextHookEx(hhook, code, wParam, lParam);

			// Raise HookInvoked event
			HookEventArgs e = new HookEventArgs();
			e.HookCode = code;
			e.wParam = wParam;
			e.lParam = lParam;
			RaiseHookInvoked(e);

			// Call the next hook in the chain
			return CallNextHookEx(hhook, code, wParam, lParam);
		}
	
    /// <summary>
    /// Insatalls the hook to the system's hook chain
    /// </summary>
		public void Install()
		{
      hhook = SetWindowsHookEx(hookType, callbackFunc, IntPtr.Zero, ( int )AppDomain.GetCurrentThreadId());
		}
		
    /// <summary>
    /// Uninstalls the hook from the system's hook chain
    /// </summary>
		public void Uninstall()
		{
			UnhookWindowsHookEx(hhook); 
			hhook = IntPtr.Zero;
		}
		
		#region WinAPI Imports
    /// <summary>
    /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
    /// </summary>
    /// <param name="code">Specifies the type of hook procedure to be installed.</param>
    /// <param name="func">Pointer to the hook procedure.</param>
    /// <param name="hInstance">Handle to the DLL containing the hook procedure. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process.</param>
    /// <param name="threadID">Specifies the identifier of the thread with which the hook procedure is to be associated.</param>
    /// <returns>If the function succeeds, the return value is the handle to the hook procedure, otherwise returns NULL.</returns>
		[DllImport("user32.dll")]
		protected static extern IntPtr SetWindowsHookEx(HookType code, 
			HookCallbackFunction func, IntPtr hInstance, int threadID);
		
    /// <summary>
    /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain.
    /// </summary>
    /// <param name="hhook">Handle to the hook to be removed.</param>
    /// <returns>If the function succeeds, the return value is nonzero, otherwise return zero.</returns>
		[DllImport("user32.dll")]
		protected static extern int UnhookWindowsHookEx(IntPtr hhook); 
		
    /// <summary>
    /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
    /// </summary>
    /// <param name="hhook">Ignored.</param>
    /// <param name="code">Specifies the hook code passed to the current hook procedure. </param>
    /// <param name="wParam">Specifies the wParam value passed to the current hook procedure.</param>
    /// <param name="lParam">Specifies the lParam value passed to the current hook procedure.</param>
    /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type.</returns>
		[DllImport("user32.dll")]
		protected static extern int CallNextHookEx(IntPtr hhook, 
			int code, IntPtr wParam, IntPtr lParam);
		
		#endregion
	}
}
