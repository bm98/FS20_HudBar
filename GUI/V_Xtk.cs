using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Cross Track Distance Formatter
  /// </summary>
  class V_Xtk : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Xtk( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "nm";
      m_default = DefaultString( "__.__◄" ); // direction sign NN.NN   3.2 format
      Text = UnitString( m_default );
    }

    private string c_left="◄";
    private string c_right="►";
    private string c_flat = " ";

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else {
          if ( value <= -0.01 ) {
            this.Text = UnitString( $"{-value,5:#0.00}{c_left}" );
          }
          else if ( value >= 0.01 ) {
            this.Text = UnitString( $"{value,5:#0.00}{c_right}" );
          }
          else {
            this.Text = UnitString( $"{value,5:#0.00}{c_flat}" );
          }
        }
      }
    }

  }
}
