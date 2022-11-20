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
  /// Vertical Speed Formatter ft/sec (m/sec)
  /// </summary>
  class V_VSpeedPerSec : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeedPerSec( Label proto, int width = 0 )
    : base( proto, width )
    {
      m_unit = "f/s";
      m_default = DefaultString( "____↑" + " " ); // NNNN_Direction
      Text = UnitString( RightAlign( m_default ) );
    }

    private string c_up = "↑";
    private string c_do = "↓";
    private string c_flat = " ";

    protected override void SetAltitude_Metric( )
    {
      m_unit = _altitude_metric ? "m/s" : "f/s";
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
          if (value <= -5) {
            this.Text = UnitString( RightAlign( $"{-uValue,4:###0}{c_do}{_cManaged}" ) );
          }
          else if (value >= 5) {
            this.Text = UnitString( RightAlign( $"{uValue,4:###0}{c_up}{_cManaged}" ) );
          }
          else {
            this.Text = UnitString( RightAlign( $"{0,4:###0}{c_flat}{_cManaged}" ) );
          }
        }
      }
    }

  }
}

