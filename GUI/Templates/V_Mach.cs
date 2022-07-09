using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Mach : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Mach( Label proto )
    : base( proto )
    {
      m_unit = "M";
      m_default = DefaultString( "_.__ " + " " ); // N.NN 
      Text = UnitString( RightAlign( m_default ) );
    }

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
          this.Text = UnitString( RightAlign( $"{value,4:#.##} " + " " ) ); //1.2 digits positive only, add a blank for alignment
        }
      }
    }

  }
}

