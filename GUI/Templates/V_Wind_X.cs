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
  /// Cross Wind Formatter
  /// </summary>
  class V_Wind_X : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Wind_X( Label proto )
    : base( proto )
    {
      m_unit = "kt";
      m_default = DefaultString( "___→" ); // L | R NNN kt
      Text = UnitString( m_default );
    }

    private string c_fromLeft = "→";
    private string c_fromRight = "←";
    private string c_flat = " ";

    protected override void SetDistance_Metric( )
    {
      m_unit = _distance_metric ? "m/s" : "kt";
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
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
          float uValue = _distance_metric ? Conversions.Mps_From_Kt( (float)value ) : (float)value;
          if (uValue < 0 ) {
            this.Text = UnitString( $"{-uValue,3:##0}{c_fromRight}" );
          }
          else if (uValue > 0 ) {
            this.Text = UnitString( $"{uValue,3:##0}{c_fromLeft}" );
          }
          else {
            this.Text = UnitString( $"{uValue,3:##0}{c_flat}" );
          }
        }
      }
    }

  }
}
