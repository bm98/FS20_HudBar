using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_Torque : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Torque( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "ft/lb";
      m_default = DefaultString( "____" );
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else {
          this.Text = UnitString( $"{value,4:###0}" ); // positive only 4 digits
        }
      }
    }

  }
}

