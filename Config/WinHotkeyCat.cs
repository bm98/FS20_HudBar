using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Win;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// The supported Hotkeys
  /// </summary>
  public enum Hotkeys
  {
    Show_Hide=0,
    Profile_1, Profile_2, Profile_3, Profile_4, Profile_5,
    FlightBag,
    Camera
  }

  /// <summary>
  /// A Hotkey Catalog
  /// </summary>
  internal class WinHotkeyCat : Dictionary<Hotkeys, WinHotkey>
  {

    /// <summary>
    /// Return a Copy of this item
    /// </summary>
    /// <returns>A copy of this catalog</returns>
    public WinHotkeyCat Copy( )
    {
      var ret = new WinHotkeyCat();
      foreach(var hk in this ) {
        ret.Add( hk.Key, hk.Value.Copy( ) );
      }
      return ret;
    }

    /// <summary>
    /// Add a hotkey from a string
    /// </summary>
    /// <param name="hotkey">The hotkey item</param>
    /// <param name="hkString">The hotkey string</param>
    public void AddHotkeyString(Hotkeys hotkey, string hkString )
    {
      if ( string.IsNullOrWhiteSpace( hkString ) ) return; // cannot add an empty one

      var hk = new WinHotkey(hkString);
      if ( hk.isValid ) this.Add( hotkey, hk );
    }

    /// <summary>
    /// Return a hotkey string for a hotkey item
    /// </summary>
    /// <param name="hotkey">The hotkey item</param>
    /// <returns>The hotkey string</returns>
    public string HotkeyString( Hotkeys hotkey )
    {
      if ( this.ContainsKey( hotkey ) )
        return this[hotkey].AsString;
      else
        return "";
    }


  }

}
