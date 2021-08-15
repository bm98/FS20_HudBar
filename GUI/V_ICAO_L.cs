using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// An ICAO label (up to 5 chars padded, LEFT aligned)
  /// </summary>
  class V_ICAO_L : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_ICAO_L( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = "_____";
      Text = UnitString( m_default );
    }

    public override string Text {
      get => base.Text;
      set {
        base.Text = $"{value,-5}";
      }
    }
  }
}

