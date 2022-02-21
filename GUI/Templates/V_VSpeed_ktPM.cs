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
  class V_VSpeed_ktPM : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeed_ktPM( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "kts";
      m_default = DefaultString( "±__.__↑" ); // ±NN.NN
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
          if ( value <= -0.02 ) {
            this.Text = UnitString( $"{value,6:+#0.00;-#0.00}{c_do}" ); // show + and - signs
          }
          else if ( value >= 0.02 ) {
            this.Text = UnitString( $"{value,6:+#0.00;-#0.00}{c_up}" ); // show + and - signs
          }
          else {
            this.Text = UnitString( $"{value,6:+#0.00;-#0.00}{c_flat}" ); // show + and - signs
          }
        }
      }
    }

  }
}

