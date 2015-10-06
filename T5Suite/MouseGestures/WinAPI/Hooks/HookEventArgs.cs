using System;
using System.Collections.Generic;
using System.Text;

namespace WinAPI.Hooks {
  /// <summary>
  /// Class containing the information about invoked hook
  /// </summary>
  public class HookEventArgs : EventArgs {
    public int HookCode;
    public IntPtr wParam;
    public IntPtr lParam;
  }
}
