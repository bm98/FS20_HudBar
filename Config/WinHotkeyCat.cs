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
    // must start with enum=0 (not what enums are for - but then, it makes things easier...)
    Profile_1=0, Profile_2, Profile_3, Profile_4, Profile_5,
    Profile_6, Profile_7, Profile_8, Profile_9, Profile_10,
    Show_Hide,
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
    /// Remove it when existing and the string is empty
    /// Replace if it exists
    /// </summary>
    /// <param name="hotkey">The hotkey item</param>
    /// <param name="hkString">The hotkey string</param>
    public void MaintainHotkeyString(Hotkeys hotkey, string hkString )
    {
      // remove if empty string and exists
      if (string.IsNullOrWhiteSpace( hkString )) {
        if (this.ContainsKey( hotkey )) {
          this.Remove( hotkey );
          return;
        }
      }
      // replace, add
      var hk = new WinHotkey(hkString);
      if (hk.isValid) {
        // replace or add if valid
        if (this.ContainsKey( hotkey )) {
          // remove to replace
          this.Remove( hotkey );
        }
        // now add
        this.Add( hotkey, hk );
      }
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
