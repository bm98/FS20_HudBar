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
  /// Lateral Distance in ft
  /// </summary>
  class V_LatDist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_LatDist( Label proto )
    : base( proto )
    {
      m_unit = "ft";
      m_default = DefaultString( "___►" );
      Text = UnitString( m_default );
    }

    private string c_left = "◄";
    private string c_right = "►";
    private string c_flat = " ";

    protected override void SetDistance_Metric( )
    {
      m_unit = _distance_metric ? "m" : "ft";
    }

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if (value == null) {
          this.Text = UnitString( m_default );
        }
        else if (float.IsNaN( (float)value )) {
          this.Text = UnitString( m_default );
        }
        else {
          float uValue = _distance_metric ? Conversions.M_From_Ft( (float)value ) : (float)value;
          if (Math.Abs( uValue ) >= 1000.0f) {
            this.Text = UnitString( m_default );
          }
          else {
            if (value <= -0.01) {
              this.Text = UnitString( $"{-uValue,3:##0}{c_right}" );
            }
            else if (value >= 0.01) {
              this.Text = UnitString( $"{uValue,3:##0}{c_left}" );
            }
            else {
              this.Text = UnitString( $"{uValue,3:##0}{c_flat}" );
            }
          }
        }
      }
    }

  }
}