using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_GForce : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_GForce( Label proto )
    : base( proto )
    {
      m_unit = "g";
      m_default = DefaultString( "+_._ " );
      Text = UnitString( m_default );
    }

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
          this.Text = UnitString( $"{value,4:+0.0;-0.0} " ); // sign 1.1 digits no sign expected, add a blank to aling better
        }
      }
    }

  }
}
