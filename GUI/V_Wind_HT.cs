using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
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
    public V_Wind_HT( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "kt";
      m_default = "↓___"; // H | T NNN kt
      Text = UnitString( m_default );
    }

    private string c_head = "↓";
    private string c_tail = "↑";
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
            this.Text = UnitString( $"{-value,3:##0}{c_head}" );
          }
          else if ( value > 0 ) {
            this.Text = UnitString( $"{value,3:##0}{c_tail}" );
          }
          else {
            this.Text = UnitString( $"{value,3:##0}{c_flat}" );
          }
        }
      }
    }

  }
}
