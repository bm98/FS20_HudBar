using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using RawInputLib;

namespace FS20_HudBar.Win
{

  /// <summary>
  /// The available Key Modifiers used for registering Hotkeys
  ///  Can be ORed (|) together for multiple modifiers
  /// </summary>
  [Flags]
  public enum KeyModifiers
  {
    None   =  0x00,
    LAlt   =  0x01,
    RAlt   =  0x02,
    LCtrl  =  0x04,
    RCtrl  =  0x08,
    LShift =  0x10,
    RShift =  0x20,
  }

  /// <summary>
  /// Implements a Global Keyboard Hook Controller
  /// NOTE:  RMENU aka Right ALT triggers LCTRL as well (Windows..)
  /// </summary>
  internal class HotkeyController : IDisposable
  {
    private bool _kbdAction = false;

    // the Hook Handler
    private readonly GlobalKbdHook _kbdHook;
    //private readonly RawInput _rawinput;

    // Catalog of registered Hotkeys
    private HotkeyItemCat _cat;

    // current modifier pattern
    private KeyModifiers _modifiers;

    public HotkeyController( IntPtr handle )
    {
      _cat = new HotkeyItemCat( );

      /*
      // RawInputDLL Version
      _rawinput = new RawInput( handle, false );
      _rawinput.AddMessageFilter( );   // Adding a message filter will cause keypresses to be handled
      _rawinput.KeyPressed += OnKeyPressed;
      */

      // GlobalKbdHook Version
      try {
        _kbdHook = new GlobalKbdHook( );
        _kbdHook.KeyboardPressed += _kbdHook_KeyboardPressed;
      }
      catch {
        _kbdHook = null;
        Console.WriteLine( "HotkeyController: Cannot establish the Keyboard Hook" );
      }

    }

    /// <summary>
    /// Remove a Hotkey from the registered List
    /// </summary>
    /// <param name="tag">The Hotkey ID to remove</param>
    public void RemoveKey( string tag )
    {
      if ( _cat.ContainsKey( tag ) ) {
        _cat.Remove( tag );
      }
    }

    /// <summary>
    /// Remover all Hotkeys
    /// </summary>
    public void RemoveAllKeys( )
    {
      _cat.Clear( );
    }

    /// <summary>
    /// Add a Hotkey to the list (no checks for potentially problematic cases here..)
    ///  NOTE: The Action may only last some milliseconds, else the KeyHook will be unhooked by Windows (see doc)
    /// </summary>
    /// <param name="key">A virtual Key</param>
    /// <param name="modifierPattern">The Key Modifier pattern</param>
    /// <param name="tag">A unique ID</param>
    /// <param name="onKey">An Action(string) which is exec when the Hotkey is pressed</param>
    public void AddKey( Keys key, KeyModifiers modifierPattern, string tag, Action<string> onKey )
    {
      RemoveKey( tag );
      var item = new HotkeyItem(key, modifierPattern, tag, onKey);
      _cat.Add( tag, item );
    }


    /// <summary>
    /// Enable/Disable KeyHandling
    /// </summary>
    /// <param name="enabled">Mode</param>
    public void KeyHandling( bool enabled )
    {
      // a bit silly but we cannot change the _cat when kbd handling is enabled
      // and also not block as it is the same thread..
      _kbdAction = enabled;
    }

    // Asynch Call from the GlobalKbdHook
    private void _kbdHook_KeyboardPressed( object sender, GlobalKbdHookEventArgs e )
    {
      if ( !_kbdAction ) return; // not allowed at this time...

      // maintain the currently held down modifiers
      if ( e.KeyDown ) {
        // pressed
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.LMenu ) ? KeyModifiers.LAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.RMenu ) ? KeyModifiers.RAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.LControlKey ) ? KeyModifiers.LCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.RControlKey ) ? KeyModifiers.RCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.LShiftKey ) ? KeyModifiers.LShift : KeyModifiers.None;
        _modifiers |= ( e.KeyboardData.VirtualCode == (int)Keys.RShiftKey ) ? KeyModifiers.RShift : KeyModifiers.None;
      }
      else {
        // released
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.LMenu ) ? ~KeyModifiers.LAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.RMenu ) ? ~KeyModifiers.RAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.LControlKey ) ? ~KeyModifiers.LCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.RControlKey ) ? ~KeyModifiers.RCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.LShiftKey ) ? ~KeyModifiers.LShift : ~KeyModifiers.None;
        _modifiers &= ( e.KeyboardData.VirtualCode == (int)Keys.RShiftKey ) ? ~KeyModifiers.RShift : ~KeyModifiers.None;
      }

      //Console.WriteLine( $"{e.KeyDown,6} {e.KeyboardData.VirtualCode} {(Keys)e.KeyboardData.VirtualCode}  {e.KeyboardData.Flags}  {e.KeyboardData.AdditionalInformation}" );

      if ( e.KeyDown ) {
        // proc keys only if enabled 
        for ( int i = 0; i < _cat.Count; i++ ) {
          var hi = _cat.ElementAt(i);
          hi.Value.TestAndCall( e.KeyboardData.VirtualCode, _modifiers ); // will exec if the key and mod matches
        }
        // Eat Input if true, else it will go to all other subscribers too 
        //e.Handled = true;
      }
    }

    /*
    // Asynch Call from the RawInputLib DLL
    private void OnKeyPressed( object sender, RawInputEventArg e )
    {
      if ( !_kbdAction ) return; // not allowed at this time...

      // maintain the current modifiers
      if ( e.KeyPressEvent.KeyDown ) {
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.LMenu ) ? KeyModifiers.LAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.RMenu ) ? KeyModifiers.RAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.LControlKey ) ? KeyModifiers.LCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.RControlKey ) ? KeyModifiers.RCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.LShiftKey ) ? KeyModifiers.LShift : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)Keys.RShiftKey ) ? KeyModifiers.RShift : KeyModifiers.None;
      }
      else {
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.LMenu ) ? ~KeyModifiers.LAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.RMenu ) ? ~KeyModifiers.RAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.LControlKey ) ? ~KeyModifiers.LCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.RControlKey ) ? ~KeyModifiers.RCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.LShiftKey ) ? ~KeyModifiers.LShift : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)Keys.RShiftKey ) ? ~KeyModifiers.RShift : ~KeyModifiers.None;
      }

      //Console.WriteLine( $"{e.KeyPressEvent.Message} {e.KeyPressEvent.VKeyName}  {e.KeyPressEvent.VKey}" );

      if ( e.KeyPressEvent.KeyDown ) {
        // proc keys only if enabled 
        for ( int i = 0; i < _cat.Count; i++ ) {
          var hi = _cat.ElementAt(i);
          hi.Value.TestAndCall( e.KeyPressEvent.VKey, _modifiers ); // will exec if the key and mod matches
        }
      }
    }
    */
    /// <summary>
    /// Dispose this and depending objects
    /// </summary>
    public void Dispose( ) => ( (IDisposable)_kbdHook )?.Dispose( );


  }
}