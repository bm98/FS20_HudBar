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
  /// A Value Item for Text
  /// </summary>
  class V_Text : V_Base
  {
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="proto"></param>
    public V_Text( Label proto )
    : base( proto, false )
    {
      m_unit = "";
      m_default = "_____";
      Text = UnitString( m_default );
    }

  }
}
