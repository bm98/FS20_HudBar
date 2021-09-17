﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Vertical Speed Formatter with +- signs
  /// </summary>
  class V_VSpeedPM : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_VSpeedPM( Label proto, bool showUnit )
    : base( proto, showUnit )
    {
      m_unit = "fpm";
      m_default = DefaultString( "±____ " ); // ±NNNN
      Text = UnitString( m_default );
    }

    /// <summary>
    /// Set the value of the Control
    /// </summary>
    override public float? Value {
      set {
        if ( value == null ) {
          this.Text = UnitString( m_default );
        }
        else if ( float.IsNaN( (float)value ) ) {
          this.Text = UnitString( m_default );
        }
        else {
          this.Text = UnitString( $"{value,5:+###0;-###0} " ); // show + and - signs, add a blank for alignment
        }
      }
    }

  }
}