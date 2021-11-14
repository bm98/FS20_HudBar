using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Lights : V_Base
  {
    public enum Lights
    {
      Beacon=0x01,
      Nav=0x02,
      Strobe=0x04,
      Taxi=0x08,
      Landing=0x10,
    }

    /// <summary>
    /// cTor: Inherits from Label Control
    /// </summary>
    /// <param name="proto"></param>
    public V_Lights( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = "_ _ _  _ _"; // note 2 Spaces after to separate T & L _ is Off
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control - formatted as NN0kt
    /// </summary>
    override public int? IntValue {
      set {
        string retVal = m_default;
        if ( value == null ) {
          Text = retVal;
          return;
        }
        retVal = "";
        if ( ( value & (int)Lights.Beacon ) == (int)Lights.Beacon ) { retVal += "B "; } else { retVal += "_ "; }
        if ( ( value & (int)Lights.Nav ) == (int)Lights.Nav ) { retVal += "N "; } else { retVal += "_ "; }
        if ( ( value & (int)Lights.Strobe ) == (int)Lights.Strobe ) { retVal += "S  "; } else { retVal += "_  "; } // note 2 Spaces after to separate T & L
        if ( ( value & (int)Lights.Taxi ) == (int)Lights.Taxi ) { retVal += "T "; } else { retVal += "_ "; }
        if ( ( value & (int)Lights.Landing ) == (int)Lights.Landing ) { retVal += "L "; } else { retVal += "_ "; }
        Text = retVal;
      }
    }

  }
}

