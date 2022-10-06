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
  /// Vertical Speed Formatter with +- signs
  /// </summary>
  class V_VSpeedPM : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeedPM( Label proto )
    : base( proto )
    {
      m_unit = "f/M";
      m_default = DefaultString( "±____ " + " " ); // ±NNNN
      Text = UnitString( RightAlign( m_default ) );
    }

    protected override void SetAltitude_Metric( )
    {
      m_unit = _altitude_metric ? "m/M" : "f/M";
    }

    /// <summary>
    /// Set the value of the Control
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
          float uValue = _altitude_metric ? Conversions.M_From_Ft( (float)value ) : (float)value;
          this.Text = UnitString( RightAlign( $"{uValue,5:+###0;-###0} " + " " ) ); // show + and - signs, add a blank for alignment
        }
      }
    }

  }
}