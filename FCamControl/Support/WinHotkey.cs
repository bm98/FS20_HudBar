using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using dNetBm98.Win;

namespace FCamControl
{
  /// <summary>
  /// Represents a MSFS Key or Key combination
  /// </summary>
  internal class WinHotkey : List<Keys>
  {
    #region String serialization

    /// <summary>
    /// Create a Hotkey from a string of keynames (space separated)
    ///  - from WinHotkey.AsString  
    /// </summary>
    /// <param name="hkString">The hotkey string</param>
    /// <returns>A WinHotkey</returns>
    public static WinHotkey FromConfigString( string hkString )
    {
      // MUST match the ToString below
      var ret = new WinHotkey( );
      string[] e = hkString.Split( new char[] { ' ' } );
      foreach (var s in e) {
        if (Enum.TryParse( s, out Keys key )) {
          ret.Add( key );
        }
      }
      return ret;
    }

    /// <summary>
    /// Create a string from a WinHotkey
    /// </summary>
    /// <param name="key">The WinHotkey</param>
    /// <returns>A string of keynames</returns>
    public static string ToConfigString( WinHotkey key )
    {
      // MUST match the FromString above
      string ret = "";
      foreach (var k in key) {
        ret += $"{k} ";
      }
      return ret.TrimEnd( );
    }

    #endregion

    // CLASS

    /// <summary>
    /// cTor: empty
    /// </summary>
    public WinHotkey( ) { }
    /// <summary>
    /// cTor: Copy Constructor
    /// </summary>
    /// <param name="hotkey">A WinHotkey</param>
    public WinHotkey( WinHotkey hotkey )
    {
      this.AddRange( hotkey );
    }
    /// <summary>
    /// cTor: Create from a Hotkey string / serialized 
    /// </summary>
    /// <param name="hotkey">A Hotkey string</param>
    public WinHotkey( string hotkey )
    {
      this.AddRange( FromConfigString( hotkey ) );
    }
    /// <summary>
    /// cTor: Create from a List of Keys
    /// </summary>
    /// <param name="keyArr">Array of Keys</param>
    public WinHotkey( Keys[] keyArr )
    {
      this.AddRange( keyArr );
    }

    /// <summary>
    /// True if we have a valid hotkey
    /// </summary>
    public bool isValid => this.Count > 0;

    /// <summary>
    /// Return the main key of this item
    ///  The last item
    /// </summary>
    public Keys Key {
      get {
        if (this.isValid)
          return this.ElementAt( this.Count - 1 );
        else
          return Keys.None;
      }
    }

    /// <summary>
    /// Returns the Modifiers of this item (can be empty)
    ///  All but the last key..
    /// </summary>
    public List<Keys> Modifier {
      get {
        var ret = new List<Keys>( );
        if (this.Count > 1) {
          ret.AddRange( this.GetRange( 0, this.Count - 1 ) );
        }

        return ret;
      }
    }

    /// <summary>
    /// Get; The Hotkey as Text string
    /// </summary>
    public string AsString {
      get => ToConfigString( this );
      set {
        this.Clear( );
        this.AddRange( FromConfigString( value ) );
      }
    }

    /// <summary>
    /// Return a copy of this item
    /// </summary>
    /// <returns>A copy of this Hotkey</returns>
    public WinHotkey Copy( )
    {
      WinHotkey ret = new WinHotkey( );
      ret.AddRange( this );
      return ret;
    }


  }
}
