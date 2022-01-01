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
  /// Percent with +- 2.1 up to 99.x then +- 3
  /// </summary>
  class V_Prct_999 : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Prct_999( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = DefaultString( "+__._%" );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +N0.0% or +NN0%
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
          if ( Math.Abs( (float)value ) > 99f ) {
            this.Text = UnitString( $"{value,7:##0%}" );  // sign 3.0 digits %
          }
          else {
            this.Text = UnitString( $"{value,7:#0.0%}" );  // sign 2.1 digits %
          }
        }
      }
    }


  }
}
