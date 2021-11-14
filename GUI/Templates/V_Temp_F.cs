using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Display an Engine Temp in °F (converting °C input to °F output)
  /// </summary>
  class V_Temp_F : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Temp_F( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "°F";
      m_default = DefaultString( "+____ " );
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
          this.Text = UnitString( $"{Tooling.DegCtoF( (float)value ),5:###0} " ); // signed only 4 digits, add a blank for alignment
        }
      }
    }

  }
}

