using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WinAPI {
  /// <summary>
  /// Encapsulate necessary functions and structures for simulating user input
  /// </summary>
  /// <remarks>
  /// Documentation gets only brief desctiption about unmanaged function parameters
  /// see Platform SDK for full decscription
  /// </remarks>
  public class MouseInputEmulation {
    #region Unmanaged WinAPI calls and structs
    /// <summary>
    /// Imported unmanaged function. The SendInput function synthesizes
    /// keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="nInputs">Specifies the number of structures in the pInputs array</param>
    /// <param name="pInputs">Pointer to an array of INPUTMOUSE structures.</param>
    /// <param name="cbSize">Specifies the size, in bytes, of an INPUT structure</param>
    /// <returns>The function returns the number of events that it successfully
    /// inserted into the keyboard or mouse input stream. If the function
    /// returns zero, the input was already blocked by another thread.
    ///</returns>
    ///<remarks>
    ///Type of pInputs parameter - the INPUT structure is defined
    ///as union (MOUSEINPUT, KEYBDINPUT, HARDWAREINPUT) in the Platform SDK.
    /// We use overloaded methods with pInputs types MOUSEINPUT, KEYBDINPUT instead
    ///</remarks>
    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, ref INPUTMOUSE pInputs, int cbSize);

    /// <summary>
    /// Structure used to store information for synthesizing mouse input events
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    private struct INPUTMOUSE {
      /// <summary>
      /// Type of INPUT structure
      /// </summary>
      /// <remarks>
      /// must be INPUT_MOUSE for INPUTMOUSE structure
      /// </remarks>
      [FieldOffset(0)]
      public INPUTTYPE type;

      /// <summary>
      /// The structure that contains information about a simulated mouse event
      /// </summary>
      [FieldOffset(4)]   
      public tagMOUSEINPUT mi;
    }

    /// <summary>
    /// Defines type of INPUT structure
    /// </summary>
    private enum INPUTTYPE : uint {
      INPUT_MOUSE = 0,
      INPUT_KEYBOARD = 1,
      INPUT_HARDWARE = 2,
    }

    /// <summary>
    /// The structure contains information about a simulated mouse event
    /// </summary>
    private struct tagMOUSEINPUT {
      /// <summary>
      /// Specifies the absolute position of the mouse,
      /// or the amount of motion since the last mouse event was generated.
      /// </summary>
      public int dx;

      /// <summary>
      /// Specifies the absolute position of the mouse,
      /// or the amount of motion since the last mouse event was generated.
      /// </summary>
      public int dy;

      /// <summary>
      /// If dwFlags contains MOUSEEVENTF_WHEEL,
      /// then mouseData specifies the amount of wheel movement.
      /// </summary>
      public uint mouseData;

      /// <summary>
      /// A set of bit flags that specify various aspects of mouse motion and button clicks.
      /// The bits in this member can be any reasonable combination of the following values. 
      /// </summary>
      public MOUSEEVENTFLAGS dwFlags;

      /// <summary>
      /// Time stamp for the event, in milliseconds.
      /// If this parameter is 0, the system will provide its own time stamp. 
      /// </summary>
      public uint time;

      /// <summary>
      /// Specifies an additional value associated with the mouse event.
      /// </summary>
      public uint dwExtraInfo;
    }

    /// <summary>
    /// Enum defines MOUSEEVENTFLAGS bitfield
    /// </summary>
    [System.Flags]
    private enum MOUSEEVENTFLAGS : uint {
      MOUSEEVENTF_MOVE = 0x0001,        /* mouse move */
      MOUSEEVENTF_LEFTDOWN = 0x0002,    /* left button down */
      MOUSEEVENTF_LEFTUP = 0x0004,      /* left button up */
      MOUSEEVENTF_RIGHTDOWN = 0x0008,   /* right button down */
      MOUSEEVENTF_RIGHTUP = 0x0010,     /* right button up */
      MOUSEEVENTF_MIDDLEDOWN = 0x0020,  /* middle button down */
      MOUSEEVENTF_MIDDLEUP = 0x0040,    /* middle button up */
      MOUSEEVENTF_XDOWN = 0x0080,       /* x button down */
      MOUSEEVENTF_XUP = 0x0100,         /* x button down */
      MOUSEEVENTF_WHEEL = 0x0800,       /* wheel button rolled */
      MOUSEEVENTF_VIRTUALDESK = 0x4000, /* map to entire virtual desktop */
      MOUSEEVENTF_ABSOLUTE = 0x8000     /* absolute move */
    }
    #endregion

    /// <summary>
    /// Synthesizies right mouse button click at the current cursor position
    /// </summary>
    public static void SendRightMouseClick() {
      INPUTMOUSE im = new INPUTMOUSE();
      im.type = INPUTTYPE.INPUT_MOUSE;

      //Sends MOUSEEVENTF_RIGHTDOWN
      im.mi.dwFlags = MOUSEEVENTFLAGS.MOUSEEVENTF_RIGHTDOWN;
      SendInput(( uint )1, ref im, Marshal.SizeOf(im));

      //Sends MOUSEEVENTF_RIGHTUP
      im.mi.dwFlags = MOUSEEVENTFLAGS.MOUSEEVENTF_RIGHTUP;
      SendInput(( uint )1, ref im, Marshal.SizeOf(im));
    }
  }
}
