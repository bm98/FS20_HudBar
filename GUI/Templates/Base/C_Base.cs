using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.GUI.Templates.Base
{
  /// <summary>
  /// Base class to implement generic controls
  /// </summary>
  abstract class C_Base : Control, IValue, IColorType
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected GUI_Colors.ColorType m_foreColorType = GUI_Colors.ColorType.cInfo;
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;

    /// <summary>
    /// Set the numeric Value
    /// </summary>
    virtual public float? Value { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Set the integer Value
    /// </summary>
    virtual public int? IntValue { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Set the Step Value
    /// </summary>
    virtual public Steps Step { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public GUI_Colors.ColorType ItemForeColor {
      get => m_foreColorType;
      set {
        m_foreColorType = value;
        this.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public GUI_Colors.ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
        this.BackColor = GUI_Colors.ItemColor( m_backColorType );
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    virtual public void UpdateColor( )
    {
      this.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      this.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }


    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    /// <summary>
    /// cTor: Create a UserControl based on a prototype control
    /// </summary>
    /// <param name="proto">A label Prototype to derive from</param>
    public C_Base( Label proto, bool showUnit )
    {
      Font = proto.Font;
      ItemForeColor = GUI_Colors.ColorType.cInfo;
      ItemBackColor = GUI_Colors.ColorType.cBG; // force our common BG color here
      AutoSize = true;                          // force AutoSize
      Anchor = proto.Anchor;
      Margin = proto.Margin;
      Padding = proto.Padding;
      m_showUnit = showUnit;
      Cursor = Cursors.Default;                 // avoid the movement cross on the item controls
      Text = m_default;

      GUI_Colors.Register( this );
    }

  }
}

