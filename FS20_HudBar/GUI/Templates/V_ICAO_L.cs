using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// An IlsID label (LEFT aligned)
  /// </summary>
  class V_ICAO_L : V_ICAO
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_ICAO_L( Label proto )
    : base( proto )
    {
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
          base.Text = ( value.Length > MaxLen ) ? $"{value.Substring( 0, MaxLen ),MaxLen}" : base.Text = $"{value,-MaxLen}"; // Left aligned
        }
      }
    }

  }
}

