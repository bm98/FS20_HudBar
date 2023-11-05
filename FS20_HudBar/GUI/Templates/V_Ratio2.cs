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
  /// Ratio with 2 digits
  /// </summary>
  internal class V_Ratio2 : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Ratio2( Label proto )
    : base( proto )
    {
      m_unit = " "; // unitless
      m_default = DefaultString( "_.__ " );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
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
          this.Text = UnitString( $"{value,5:#.00} " ); // positive 1.2 digits, add a blank for alignment
        }
      }
    }

  }
}

