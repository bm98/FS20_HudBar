using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RawInputLib;

namespace FS20_HudBar.Win
{

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


  internal class KeyboardHookController
  {
    private bool _kbdAction = false;

    private readonly RawInput _rawinput;
    private KeyboardHookItemCat _cat;

    private KeyModifiers _modifiers;

    public KeyboardHookController( IntPtr handle )
    {
      _cat = new KeyboardHookItemCat( );

      _rawinput = new RawInput( handle, false );
      _rawinput.AddMessageFilter( );   // Adding a message filter will cause keypresses to be handled
      _rawinput.KeyPressed += OnKeyPressed;
    }

    private Stack<string> _toRelease = new Stack<string>();
    private Stack<KeyboardHookItem> _toAdd = new Stack<KeyboardHookItem>();
    private bool disposedValue;

    public void RemoveKey( string tag )
    {
      if ( _cat.ContainsKey( tag ) ) {
        _cat.Remove( tag );
      }
    }

    public void RemoveAllKeys( )
    {
      _cat.Clear( );
    }

    public void AddKey( VirtualKey key, KeyModifiers modifiers, string tag, Action<string> onKey )
    {
      RemoveKey( tag );
      var item = new KeyboardHookItem(key, modifiers, tag, onKey);
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

    // Asynch Call from the RawInputLib DLL
    private void OnKeyPressed( object sender, RawInputEventArg e )
    {
      if ( !_kbdAction ) return; // not allowed at this time...

      // maintain the current modifiers
      if ( e.KeyPressEvent.KeyDown ) {
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.LMENU ) ? KeyModifiers.LAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.RMENU ) ? KeyModifiers.RAlt : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.LCONTROL ) ? KeyModifiers.LCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.RCONTROL ) ? KeyModifiers.RCtrl : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.LSHIFT ) ? KeyModifiers.LShift : KeyModifiers.None;
        _modifiers |= ( e.KeyPressEvent.VKey == (int)VirtualKey.RSHIFT ) ? KeyModifiers.RShift : KeyModifiers.None;
      }
      else {
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.LMENU ) ? ~KeyModifiers.LAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.RMENU ) ? ~KeyModifiers.RAlt : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.LCONTROL ) ? ~KeyModifiers.LCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.RCONTROL ) ? ~KeyModifiers.RCtrl : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.LSHIFT ) ? ~KeyModifiers.LShift : ~KeyModifiers.None;
        _modifiers &= ( e.KeyPressEvent.VKey == (int)VirtualKey.RSHIFT ) ? ~KeyModifiers.RShift : ~KeyModifiers.None;
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

  }
}