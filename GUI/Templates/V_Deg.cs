using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// A Label showing Degrees
  /// </summary>
  class V_Deg : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Deg( Label proto, bool showUnit, int width = 0 )
    : base( proto, showUnit, width )
    {
      m_unit = " "; // Deg always shows °
      m_default = DefaultString( "___°" + " " );
      Text = UnitString( RightAlign( m_default ) );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if (value == null) {
          this.Text = UnitString( RightAlign( m_default ) );
        }
        else if (float.IsNaN( (float)value )) {
          this.Text = UnitString( RightAlign( m_default ) );
        }
        else {
          this.Text = UnitString( RightAlign( $"{value,3:000}°{_cManaged}" ) ); // positive 3 digits, leading zeroes
        }
      }
    }

  }
}
