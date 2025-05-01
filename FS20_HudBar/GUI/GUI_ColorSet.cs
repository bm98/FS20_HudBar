using System.Collections.Generic;
using System.Drawing;

using static FS20_HudBar.GUI.GUI_Colors;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// A Color Set
  /// </summary>
  public class GUI_ColorSet : Dictionary<ColorType, Color>
  {
    /// <summary>
    /// cTor: empty
    /// </summary>
    public GUI_ColorSet( ) { }

    /// <summary>
    /// True when the ColorSet is empty
    /// </summary>
    public bool IsEmpty => this.Count == 0;

    /// <summary>
    /// cTor: Copy
    /// </summary>
    public GUI_ColorSet( GUI_ColorSet other )
    {
      foreach (var item in other) {
        this.Add( item.Key, item.Value );
      }
    }

    /// <summary>
    /// Returns a Clone of this ColorSet
    /// </summary>
    /// <returns></returns>
    public GUI_ColorSet Clone( )
    {
      return new GUI_ColorSet( this );
    }

    /// <summary>
    /// Returns true if this equals the other
    /// </summary>
    /// <param name="other">A ColorSet</param>
    /// <returns>True when equal</returns>
    public bool Equals( GUI_ColorSet other )
    {
      if (this.Count != other.Count) return false;

      foreach (var item in other) {
        if (this.ContainsKey( item.Key )) {
          if (!this[item.Key].Equals( other[item.Key] )) return false;
        }
        else {
          return false;
        }
      }
      return true;
    }


  }
}
