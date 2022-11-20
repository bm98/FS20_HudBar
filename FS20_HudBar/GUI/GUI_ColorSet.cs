using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

  }
}
