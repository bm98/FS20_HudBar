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
  /// Vertical Speed Formatter
  /// </summary>
  class V_VSpeed : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeed( Label proto, bool showUnit, int width = 0 )
    : base( proto, showUnit, width )
    {
      m_unit = "fpm";
      m_default = DefaultString( "____↑" + " " ); // NNNN_Direction
      Text = UnitString( RightAlign( m_default ) );
    }

    private string c_up = "↑";
    private string c_do = "↓";
    private string c_flat = " ";

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
          if (value <= -5) {
            this.Text = UnitString( RightAlign( $"{-value,4:###0}{c_do}{_cManaged}" ) );
          }
          else if (value >= 5) {
            this.Text = UnitString( RightAlign( $"{value,4:###0}{c_up}{_cManaged}" ) );
          }
          else {
            this.Text = UnitString( RightAlign( $"{0,4:###0}{c_flat}{_cManaged}" ) );
          }
        }
      }
    }

  }
}
