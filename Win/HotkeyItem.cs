using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Win
{
  /// <summary>
  /// A reportable item
  /// </summary>
  internal class HotkeyItem
  {
    private int _virtualKey;
    private KeyModifiers _keyModifiers;
    public string Tag { get; private set; }
    private Action<string> _OnKey;

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="key">A virtual Key</param>
    /// <param name="modifiers">A Modifier Map</param>
    /// <param name="tag">A unique tag</param>
    /// <param name="onKey">A method to call (arg will be the tag when called)</param>
    public HotkeyItem( Keys key, KeyModifiers modifiers, string tag, Action<string> onKey )
    {
      _virtualKey = (int)key;
      _keyModifiers = modifiers;
      Tag = tag;
      _OnKey = onKey;
    }

    /// <summary>
    /// Call the registered Action
    /// </summary>
    public void OnKey( )
    {
      _OnKey.Invoke( Tag );
    }

    /// <summary>
    /// Test if the key and mod matches the registered ones
    /// </summary>
    /// <param name="key">A virtual Key</param>
    /// <param name="modMap">A Modifier Map</param>
    /// <returns>True for a Match</returns>
    public bool HitTest( int key, KeyModifiers modMap )
    {
      //Console.WriteLine( $"HitTest: {key}-{modMap}  > {_virtualKey}-{_keyModifiers}" );

      return ( ( _keyModifiers == modMap ) && ( _virtualKey == key ) );
    }

    /// <summary>
    /// All in one method to Test and Call when it matches
    /// </summary>
    /// <param name="key">A virtual Key</param>
    /// <param name="modMap">A Modifier Map</param>
    public void TestAndCall( int key, KeyModifiers modMap )
    {
      if ( HitTest( key, modMap ) )
        OnKey( );
    }


  }
}
