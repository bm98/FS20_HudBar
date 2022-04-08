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
  /// Latitude S90 .. N90
  /// </summary>
  class V_Latitude : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Latitude( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = DefaultString( "N__°__'" );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = UnitString( m_default );
        }
        else {
          string lat = CoordLib.Dms.ToLat( (double)value, "dm", 0 );
          this.Text = $"{lat,7}";
        }
      }
    }
  }
}

