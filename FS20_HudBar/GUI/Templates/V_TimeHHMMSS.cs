﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Format Time as HH:MM:SS
  /// </summary>
  class V_TimeHHMMSS : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_TimeHHMMSS( Label proto )
    : base( proto )
    {
      m_unit = " ";
      m_default = DefaultString( $"{"__:__:__",8} " );
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
          this.Text = FmtTimeFromSec( (int)value ); // H:MM:SS
        }
      }
    }


    /// <summary>
    /// Time Format:  00.00..99.99 (leading Zeroes)
    /// </summary>
    public string FmtTimeFromSec( int number )
    {
      try {
        if ( number > 3600 ) {
          // with hours
          return $"{new TimeSpan( 0, 0, 0, number ),8:hh\\:mm\\:ss}";
        }

        else if ( number >= 0 ) {
          return $"{new TimeSpan( 0, 0, 0, number ),8:mm\\:ss} ";
        }
        else {
          return m_default;
        }
      }
      catch {
        return m_default;
      }
    }


  }
}