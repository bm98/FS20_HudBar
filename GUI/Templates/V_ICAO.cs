using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// An ICAO label (up to 6 chars padded, RIGHT aligned)
  /// </summary>
  class V_ICAO : V_Base
  {
    /// <summary>
    /// Max Length of an ICAO ID (Waypoints etc.)
    /// </summary>
    protected const int MaxLen = 6;


    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_ICAO( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = new string( '_', MaxLen );
      // don't use UnitString here as we don't have units
      Text = m_default;
    }

    /// <summary>
    /// Label Text property (overwritten)
    /// </summary>
    public override string Text {
      get => base.Text;
      set {
        if ( value == null ) {
          base.Text = m_default;
        }
        else {
          base.Text = ( value.Length > MaxLen ) ? $"{value.Substring( 0, MaxLen ),MaxLen}" : base.Text = $"{value,MaxLen}";
        }
      }
    }

  }
}

