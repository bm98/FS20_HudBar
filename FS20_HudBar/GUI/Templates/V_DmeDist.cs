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
  /// Distance indicator with a directional arrow up to 999.9 shown
  /// </summary>
  class V_DmeDist : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_DmeDist( Label proto )
    : base( proto )
    {
      m_unit = "nm";
      m_default = DefaultString( "___._ " );
      Text = UnitString( m_default );
    }

    private string c_from = "↓";
    private string c_to = "↑";
    private string c_flat = " ";

    protected override void SetDistance_Metric( )
    {
      m_unit = _distance_metric ? "km" : "nm";
    }

    /// <summary>
    /// Set the value of the Control
    ///  pos. values indicate dist towards the target
    ///  neg. values indicate dist from the target
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
          float uValue = _distance_metric ? Conversions.Km_From_Nm( (float)value ) : (float)value;
          if (Math.Abs( (float)uValue ) >= 1000.0f) {
            this.Text = UnitString( "> 999 " );
          }
          else {
            if (value > 0) {
              this.Text = UnitString( $"{uValue,5:##0.0}{c_to}" );
            }
            else if (value < 0) {
              this.Text = UnitString( $"{-uValue,5:##0.0}{c_from}" );
            }
            else {
              this.Text = UnitString( $"{uValue,5:##0.0}{c_flat}" );
            }
          }
        }
      }
    }


  }
}
