using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Torque : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Torque( Label proto )
    : base( proto )
    {
      m_unit = "ftlb";
      m_default = DefaultString( "____ " );
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
          if (value < 100)
            this.Text = UnitString( $"{value,4:#0.0} " );    // 0..99.9 positive only 2+1 digits, add a blank for alignment
          else this.Text = UnitString( $"{value,4:###0} " ); // 1000..9999 positive only 4 digits, add a blank for alignment

        }
      }
    }

  }
}

