using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
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
      m_default = "_____";
      Text = UnitString( m_default );
    }

  }
}

