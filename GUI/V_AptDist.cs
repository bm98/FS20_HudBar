using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_AptDist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_AptDist( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "nm";
      m_default = DefaultString( "__._↓" );
      Text = UnitString( m_default );
    }

    private string c_from = "↓";
    private string c_to = "↑";
    private string c_flat = " ";

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
          return;
        }
        if ( Math.Abs( (float)value ) >= 100.0f ) {
          this.Text = UnitString( m_default );
          return;
        }

        if ( value < 0 ) {
          this.Text = UnitString( $"{-value,4:#0.0}{c_to}" );
        }
        else if ( value > 0 ) {
          this.Text = UnitString( $"{value,4:#0.0}{c_from}" );
        }
        else {
          this.Text = UnitString( $"{value,4:#0.0}{c_flat}" );
        }
      }
    }

  }
}
