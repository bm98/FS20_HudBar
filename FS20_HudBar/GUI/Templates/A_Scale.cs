using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using bm98_hbControls;

namespace FS20_HudBar.GUI.Templates
{
  class A_Scale : UC_Scale, IValue, IColorType, IAlign
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected GUI_Colors.ColorType m_foreColorType = GUI_Colors.ColorType.cInfo;
    protected GUI_Colors.ColorType m_foreColorAlertType = GUI_Colors.ColorType.cWarn;
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;

    // Use the UC Properties for Range and Alert setting

    /// <summary>
    /// Set the float Value
    /// </summary>
    public new float? Value {
      set {
        if (value == null) {
          base.Enabled = false;
        }
        else {
          base.Enabled = true;
          base.Value = (float)value;
        }
      }
    }

    // Generic Value Interfaces for Int/Step and Units are not supported 

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
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    public GUI_Colors.ColorType ItemForeColor {
      get => m_foreColorType;
      set {
        m_foreColorType = value;
        base.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color for the Alert by the type of the Item
    /// </summary>
    public GUI_Colors.ColorType ItemForeColor_Alert {
      get => m_foreColorAlertType;
      set {
        m_foreColorAlertType = value;
        base.ForeColor_Alert = GUI_Colors.ItemColor( m_foreColorAlertType );
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
      base.ForeColor_Alert = GUI_Colors.ItemColor( ItemForeColor_Alert );
      base.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }

    // Implement IValue Metric IF (but it is not used)
    public bool Altitude_metric { get => false; set => _ = value; }
    public bool Speed_metric { get => false; set => _ = value; }
    public bool Distance_metric { get => false; set => _ = value; }


    /// <summary>
    /// cTor: Create a UserControl based on a prototype control
    /// </summary>
    /// <param name="proto">A label Prototype to derive from</param>
    public A_Scale( )
    {
      ItemForeColor = GUI_Colors.ColorType.cInfo;
      ItemForeColor_Alert = GUI_Colors.ColorType.cWarn;
      ItemBackColor = GUI_Colors.ColorType.cBG; // force our common BG color here
      AutoSize = false;  // MUST REMAIN FALSE !!! else the Sizing does not work properly
      AutoSizeHeight = false;   // The HudBar default behavior is scaling with fixed Height
      AutoSizeWidth = true;     //   and flexible Width
      Cursor = Cursors.Default;                 // avoid the movement cross on the item controls
      Text = ""; // there is no Text supported
      BorderStyle = BorderStyle.FixedSingle; // Else the Graph is 'homeless'
      Margin = new Padding( 0, 0, 3, 0 ); // right only

      GUI_Colors.Register( this );

    }

  }
}
