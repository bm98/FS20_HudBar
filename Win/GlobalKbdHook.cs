using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FS20_HudBar.Win
{
  /// <summary>
  /// Global Keyboard Hook 
  /// based on https://stackoverflow.com/questions/604410/global-keyboard-capture-in-c-sharp-application

  /// </summary>
  internal class GlobalKbdHook : IDisposable
  {
    // Original Comment:
    //Based on https://gist.github.com/Stasonix

    public enum KeyboardState
    {
      KeyDown = 0x0100,
      KeyUp = 0x0101,
      SysKeyDown = 0x0104,
      SysKeyUp = 0x0105
    }

    /// <summary>
    /// Callback when a KeyPress or Release is captured
    /// </summary>
    public event EventHandler<GlobalKbdHookEventArgs> KeyboardPressed;


    public GlobalKbdHook( )
    {
      _windowsHookHandle = IntPtr.Zero;
      _user32LibraryHandle = IntPtr.Zero;
      _hookProc = LowLevelKeyboardProc; // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.

      _user32LibraryHandle = LoadLibrary( "User32" );
      if ( _user32LibraryHandle == IntPtr.Zero ) {
        int errorCode = Marshal.GetLastWin32Error();
        throw new Win32Exception( errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
      }

      _windowsHookHandle = SetWindowsHookEx( WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0 );
      if ( _windowsHookHandle == IntPtr.Zero ) {
        int errorCode = Marshal.GetLastWin32Error();
        throw new Win32Exception( errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess( ).ProcessName}'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
      }
    }

    // TEST ONLY
    private const int KfAltdown = 0x2000;
    private const int LlkhfAltdown = (KfAltdown >> 8);


    // called by WIN on Keys
    public IntPtr LowLevelKeyboardProc( int nCode, IntPtr wParam, IntPtr lParam )
    {
      bool fEatKeyStroke = false;

      var wparamTyped = wParam.ToInt32();
      if ( Enum.IsDefined( typeof( KeyboardState ), wparamTyped ) ) {
        object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent));
        LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;
        KeyboardState kbdState = (KeyboardState)wparamTyped;
        bool kDown = kbdState== KeyboardState.KeyDown || kbdState== KeyboardState.SysKeyDown;
        var eventArguments = new GlobalKbdHookEventArgs(p, kbdState, kDown);
        EventHandler<GlobalKbdHookEventArgs> handler = KeyboardPressed;
        handler?.Invoke( this, eventArguments );

        fEatKeyStroke = eventArguments.Handled;
      }
      // propagates or closes the event (based on Handled)
      return fEatKeyStroke ? (IntPtr)1 : CallNextHookEx( IntPtr.Zero, nCode, wParam, lParam );
    }

    /*
     * FROM ANOTHER SOURCE, may be not used
    private static int VirtualKeyCorrection( int virtualKey, bool isE0BitSet, int makeCode )
    {
      var correctedVKey = virtualKey;

      if ( _rawBuffer.header.hDevice == IntPtr.Zero ) {
        // When hDevice is 0 and the vkey is VK_CONTROL indicates the ZOOM key
        if ( _rawBuffer.data.keyboard.VKey == Win32.VK_CONTROL ) {
          correctedVKey = Win32.VK_ZOOM;
        }
      }
      else {
        switch ( virtualKey ) {
          // Right-hand CTRL and ALT have their e0 bit set 
          case Win32.VK_CONTROL:
            correctedVKey = isE0BitSet ? Win32.VK_RCONTROL : Win32.VK_LCONTROL;
            break;
          case Win32.VK_MENU:
            correctedVKey = isE0BitSet ? Win32.VK_RMENU : Win32.VK_LMENU;
            break;
          case Win32.VK_SHIFT:
            correctedVKey = makeCode == Win32.SC_SHIFT_R ? Win32.VK_RSHIFT : Win32.VK_LSHIFT;
            break;
          default:
            correctedVKey = virtualKey;
            break;
        }
      }

      return correctedVKey;
    }
    */

    #region DISPOSABLE Impl

    protected virtual void Dispose( bool disposing )
    {
      if ( disposing ) {
        // because we can unhook only in the same thread, not in garbage collector thread
        if ( _windowsHookHandle != IntPtr.Zero ) {
          if ( !UnhookWindowsHookEx( _windowsHookHandle ) ) {
            int errorCode = Marshal.GetLastWin32Error();
            Console.WriteLine( $"GlobalKbdHook: Failed to remove keyboard hooks for '{Process.GetCurrentProcess( ).ProcessName}'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
            // we don't throw in dispose
            // throw new Win32Exception( errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess( ).ProcessName}'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
          }
          _windowsHookHandle = IntPtr.Zero;

          // ReSharper disable once DelegateSubtraction
          _hookProc -= LowLevelKeyboardProc;
        }
      }

      if ( _user32LibraryHandle != IntPtr.Zero ) {
        if ( !FreeLibrary( _user32LibraryHandle ) ) // reduces reference to library by 1.
        {
          int errorCode = Marshal.GetLastWin32Error();
          Console.WriteLine( $"GlobalKbdHook: Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
          // we don't throw in dispose
          //throw new Win32Exception( errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception( Marshal.GetLastWin32Error( ) ).Message}." );
        }
        _user32LibraryHandle = IntPtr.Zero;
      }
    }

    ~GlobalKbdHook( )
    {
      Dispose( false );
    }

    public void Dispose( )
    {
      Dispose( true );
      GC.SuppressFinalize( this );
    }

    #endregion

    #region USER32 API

    // WIN CONST
    private const int WH_KEYBOARD_LL = 13;
    // Handles 
    private IntPtr _windowsHookHandle;
    private IntPtr _user32LibraryHandle;
    private HookProc _hookProc;

    delegate IntPtr HookProc( int nCode, IntPtr wParam, IntPtr lParam );

    [DllImport( "kernel32.dll" )]
    private static extern IntPtr LoadLibrary( string lpFileName );

    [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
    private static extern bool FreeLibrary( IntPtr hModule );

    /// <summary>
    /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
    /// You would install a hook procedure to monitor the system for certain types of events. These events are
    /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
    /// </summary>
    /// <param name="idHook">hook type</param>
    /// <param name="lpfn">hook procedure</param>
    /// <param name="hMod">handle to application instance</param>
    /// <param name="dwThreadId">thread identifier</param>
    /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
    [DllImport( "USER32", SetLastError = true )]
    static extern IntPtr SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId );

    /// <summary>
    /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
    /// </summary>
    /// <param name="hhk">handle to hook procedure</param>
    /// <returns>If the function succeeds, the return value is true.</returns>
    [DllImport( "USER32", SetLastError = true )]
    public static extern bool UnhookWindowsHookEx( IntPtr hHook );

    /// <summary>
    /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
    /// A hook procedure can call this function either before or after processing the hook information.
    /// </summary>
    /// <param name="hHook">handle to current hook</param>
    /// <param name="code">hook code passed to hook procedure</param>
    /// <param name="wParam">value passed to hook procedure</param>
    /// <param name="lParam">value passed to hook procedure</param>
    /// <returns>If the function succeeds, the return value is true.</returns>
    [DllImport( "USER32", SetLastError = true )]
    static extern IntPtr CallNextHookEx( IntPtr hHook, int code, IntPtr wParam, IntPtr lParam );


    // API Transfer Struct
    
    [StructLayout( LayoutKind.Sequential )]
    public struct LowLevelKeyboardInputEvent
    {
      /// <summary>
      /// A virtual-key code. The code must be a value in the range 1 to 254.
      /// </summary>
      public int VirtualCode;

      /// <summary>
      /// A hardware scan code for the key. 
      /// </summary>
      public int HardwareScanCode;

      /// <summary>
      /// The extended-key flag, event-injected Flags, context code, and transition-state flag. 
      /// This member is specified as follows. 
      /// An application can use the following values to test the keystroke Flags. 
      /// Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected. 
      /// If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
      /// </summary>
      public int Flags;

      /// <summary>
      /// The time stamp stamp for this message, equivalent to what GetMessageTime would return for this message.
      /// </summary>
      public int TimeStamp;

      /// <summary>
      /// Additional information associated with the message. 
      /// </summary>
      public IntPtr AdditionalInformation;
    }



    #endregion

  }
}