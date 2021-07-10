using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Button String Formatter
  /// </summary>
  class B_ICAO : B_Base
  {
    /// <summary>
    /// cTor: Create a UserControl..
    /// </summary>
    /// <param name="item">The GITem ID of this one</param>
    /// <param name="proto">A label Prototype to derive from</param>
    public B_ICAO( VItem item, Label proto )
      : base( item, proto )
    {
      m_unit = "";
      m_default = "_____";
      Text = UnitString( m_default );
    }

  }
}

