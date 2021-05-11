using System;
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

    virtual public float? Value { set => throw new NotImplementedException( ); }
    virtual public Steps Step { set => throw new NotImplementedException( ); }

    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    /// <summary>
    /// Add a Unit if ShowUnit is true
    /// </summary>
    /// <param name="valueString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string UnitString( string valueString )
    {
      return valueString + ( m_showUnit ? m_unit : "" );
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
      UseCompatibleTextRendering = true;
      Text = m_default;
    }

  }
}