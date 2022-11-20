using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// An interface to support dynamic color handling
  /// </summary>
  public interface IColorType
  {
    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    GUI_Colors.ColorType ItemForeColor { get; set; }

    /// <summary>
    /// Get; Set the items Background Color by the type of the Item
    /// </summary>
    GUI_Colors.ColorType ItemBackColor { get; set; }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    void UpdateColor( );

  }
}
