using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// A Label Item for Text
  /// </summary>
  class L_Text : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public L_Text( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = "_____";
      Text = UnitString( m_default );
      ItemForeColor = GUI_Colors.ColorType.cLabel; // default color for Labels
    }

  }
}

