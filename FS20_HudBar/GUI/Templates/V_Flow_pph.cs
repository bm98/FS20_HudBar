using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Flow_pph : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Flow_pph( Label proto )
    : base( proto )
    {
      m_unit = "pph";
      m_default = DefaultString( "___._ " );
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
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = UnitString( m_default );
        }
        else {
          if ( value > 1000 )
            this.Text = UnitString( $"{value,5:###0} " );  // positive only 4 digits, add a blank to aling better
          else
            this.Text = UnitString( $"{value,5:##0.0} " );  // positive only 3.1 digits, add a blank to aling better

        }
      }
    }

  }
}

