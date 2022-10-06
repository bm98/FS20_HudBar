using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Dist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Dist( Label proto )
    : base( proto )
    {
      m_unit = "nm";
      m_default = DefaultString( "____._ " );
      Text = UnitString( m_default );
    }

    protected override void SetDistance_Metric( )
    {
      m_unit = _distance_metric ? "km" : "nm";
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
          float uValue = _distance_metric ? Conversions.Km_From_Nm( (float)value ) : (float)value;
          this.Text = UnitString( $"{uValue,6:###0.0}" ); // 4.1 digits no sign expected, add a blank to aling better
        }
      }
    }

  }
}
