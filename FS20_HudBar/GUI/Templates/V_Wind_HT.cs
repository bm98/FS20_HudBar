using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Head/Tail Wind Formatter
  /// </summary>
  class V_Wind_HT : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Wind_HT( Label proto )
    : base( proto )
    {
      m_unit = "kt";
      m_default = DefaultString( "___↓" ); // H | T NNN kt
      Text = UnitString( m_default );
    }

    private string c_head = "↓";
    private string c_tail = "↑";
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
          float uValue = _distance_metric ? (float)Mps_From_Kt( (float)value ) : (float)value;
          if (uValue < 0 ) {
            this.Text = UnitString( $"{-uValue,3:##0}{c_head}" );
          }
          else if (uValue > 0 ) {
            this.Text = UnitString( $"{uValue,3:##0}{c_tail}" );
          }
          else {
            this.Text = UnitString( $"{uValue,3:##0}{c_flat}" );
          }
        }
      }
    }

  }
}
