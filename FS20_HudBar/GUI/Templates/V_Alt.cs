using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Altitude Value Formatter
  ///  responds to Altitude_metric  Flag
  /// </summary>
  class V_Alt : V_Base
  {

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto">The proto Label to derive from</param>
    /// <param name="width">Width in chars (right bound) 0 for no padding</param>
    public V_Alt( Label proto, int width = 0 )
    : base( proto, width )
    {
      m_unit = "ft";
      m_default = DefaultString( "+__'___ " + " " ); // -nn,nnn + blank
      Text = UnitString( RightAlign( m_default ) );
    }

    protected override void SetAltitude_Metric( )
    {
      m_unit = _altitude_metric ? "m" : "ft";
    }


    /// <summary>
    /// Set the value of the Control - formatted as +NN'NN0ft
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
          float uValue = _altitude_metric ? (float)M_From_Ft( (float)value ) : (float)value;
          this.Text = UnitString( RightAlign( $"{uValue,7:##,##0} {_cManaged}" ) ); // 9 chars: sign + 5 digits + 1000 separator, add a blank to aling better with ° values
        }
      }
    }

  }
}