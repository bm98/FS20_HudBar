using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_VSpeed : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeed( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "fpm";
      m_default = "_____";
      Text = UnitString( m_default );
    }

    private string c_up="↑";
    private string c_do="↓";

    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else {
          if ( value < 0 ) {
            this.Text = UnitString( $"{c_do}{-value,4:###0}" ); // sign 4 digits
          }
          else {
            this.Text = UnitString( $"{c_up}{value,4:###0}" ); // sign 4 digits
          }
        }
      }
    }

  }
}
