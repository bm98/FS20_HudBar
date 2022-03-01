using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using bm98_hbControls;

namespace FS20_HudBar.GUI.Templates
{
  class A_AHor : UC_AHor, IValue, IColorType, IAlign
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected GUI_Colors.ColorType m_foreColorType = GUI_Colors.ColorType.cInfo;
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;

    // Use the UC Properties for Range and Value setting


    // Generic Value Interfaces for Int/Step and Units are not supported 
    /// <summary>
    /// Set the float Value of the Upper Scale
    /// </summary>
    public float? Value { set => throw new NotImplementedException( ); }
    /// <summary>
    /// Set the integer Value
    /// </summary>
    public int? IntValue { set => throw new NotImplementedException( ); }
    /// <summary>
    /// Set the Step Value
    /// </summary>
    public Steps Step { set => throw new NotImplementedException( ); }
    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    // Implement the IColorType Interface

    /// <summary>
    /// Get; Set the items Foreground Color Upper Scale by the type of the Item
    /// </summary>
    public GUI_Colors.ColorType ItemForeColor {
      get => m_foreColorType;
      set {
        m_foreColorType = value;
        base.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    public GUI_Colors.ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
        base.BackColor = GUI_Colors.ItemColor( m_backColorType );
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    public void UpdateColor( )
    {
      base.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      base.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }


    /// <summary>
    /// cTor: Create a UserControl based on a prototype control
    /// </summary>
    /// <param name="proto">A label Prototype to derive from</param>
    public A_AHor( )
    {
      ItemForeColor = GUI_Colors.ColorType.cInfo;
      ItemBackColor = GUI_Colors.ColorType.cBG; // force our common BG color here
      AutoSize = false;  // MUST REMAIN FALSE !!! else the Sizing does not work properly
      AutoSizeHeight = false;   // The HudBar default behavior is scaling with fixed Height
      AutoSizeWidth = true;     //   and flexible Width
      Cursor = Cursors.Default;                 // avoid the movement cross on the item controls
      Text = ""; // there is no Text supported
      BorderStyle = BorderStyle.FixedSingle; // Else the Graph is 'homeless'

      GUI_Colors.Register( this );

    }

  }
}
