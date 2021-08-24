using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Vertical Speed Formatter
  /// </summary>
  class V_VSpeed : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeed( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "fpm";
      m_default = DefaultString( "____↑" ); // NNNN_Direction
      Text = UnitString( m_default );
    }

    private string c_up = "↑";
    private string c_do = "↓";
    private string c_flat = " ";

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
          if ( value <= -10 ) {
            this.Text = UnitString( $"{-value,4:###0}{c_do}" );
          }
          else if ( value >= 10 ) {
            this.Text = UnitString( $"{value,4:###0}{c_up}" ); 
          }
          else {
            this.Text = UnitString( $"{value,4:###0}{c_flat}" );
          }
        }
      }
    }

  }
}
