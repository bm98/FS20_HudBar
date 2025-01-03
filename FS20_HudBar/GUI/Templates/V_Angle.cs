﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Angle : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Angle( Label proto )
    : base( proto )
    {
      m_unit = " "; // Deg always shows °
      m_default = DefaultString( "+__._°" + " " );
      Text = UnitString( RightAlign( m_default ) );
    }

    /// <summary>
    /// Set the value of the Control - formatted as +-NN.N°
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
          this.Text = UnitString( RightAlign( $"{value,5:#0.0}°" + " " ) ); // sign 2.1 digits
        }
      }
    }

  }
}
