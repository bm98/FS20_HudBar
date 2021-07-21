﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Base class to implement Value labels
  /// </summary>
  abstract class V_Base : Label, IValue
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;

    private const string c_numbers="0123456789";
    private Random random = new Random();

    virtual public float? Value { set => throw new NotImplementedException( ); }
    virtual public int? IntValue { set => throw new NotImplementedException( ); }
    virtual public Steps Step { set => throw new NotImplementedException( ); }

    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    /// <summary>
    /// Add a Unit if ShowUnit is true
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string UnitString( string defaultString )
    {
      return defaultString + ( m_showUnit ? m_unit : "" );
    }

    /// <summary>
    /// Debugging, provide the default string with some numbers
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string DefaultString( string defaultString )
    {
      string ret = defaultString;
#if DEBUG
      ret = "";
      
      for (int i=0;i<defaultString.Length; i++ ) {
        if ( defaultString[i] == '_' ) {
          ret+= c_numbers[ random.Next( 10 )];
        }
        else {
          ret += defaultString[i];
        }
      }
#endif
      return ret;
    }


    /// <summary>
    /// cTor: Create a UserControl..
    /// </summary>
    /// <param name="proto">A label Prototype to derive from</param>
    public V_Base( Label proto, bool showUnit )
    {
      Font = proto.Font;
      ForeColor = proto.ForeColor;
      BackColor = proto.BackColor;
      AutoSize = true;
      TextAlign = proto.TextAlign;
      Anchor = proto.Anchor;
      Margin = proto.Margin;
      m_showUnit = showUnit;
      UseCompatibleTextRendering = true; // make sure the WingDings an other font special chars display properly
      Cursor = Cursors.Default; // avoid the movement cross on the item controls
      Text = m_default;
    }

  }
}