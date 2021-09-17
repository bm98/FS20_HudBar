using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// An ICAO label (up to 6 chars padded, RIGHT aligned)
  /// </summary>
  class V_ICAO : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_ICAO( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = "______";
      Text = UnitString( m_default );
    }

    public override string Text {
      get => base.Text;
      set {
        if ( value.Length > 6 ) {
          base.Text = $"{value.Substring(0,6)}";
        }
        else {
          base.Text = $"{value,6}";
        }
      }
    }
  }
}

