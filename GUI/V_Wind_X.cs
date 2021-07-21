using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
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
    public V_Wind_X( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "kt";
      m_default = DefaultString( "___→" ); // L | R NNN kt
      Text = UnitString( m_default );
    }

    private string c_fromLeft = "→";
    private string c_fromRight = "←";
    private string c_flat = " ";

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else {
          if ( value < 0 ) {
            this.Text = UnitString( $"{-value,3:##0}{c_fromRight}" );
          }
          else if ( value > 0 ) {
            this.Text = UnitString( $"{value,3:##0}{c_fromLeft}" );
          }
          else {
            this.Text = UnitString( $"{value,3:##0}{c_flat}" );
          }
        }
      }
    }

  }
}
