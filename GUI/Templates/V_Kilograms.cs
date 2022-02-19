using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;


namespace FS20_HudBar.GUI.Templates
{
  class V_Kilograms : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Kilograms( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "kg";
      m_default = DefaultString( "_____ " ); //NNNNN
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
          this.Text = UnitString( $"{value,5:####0} " ); // 5 digits positive only, add a blank to aling better
        }
      }
    }

  }
}
