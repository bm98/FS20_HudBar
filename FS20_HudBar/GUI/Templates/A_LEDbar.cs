using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using bm98_hbControls;

namespace FS20_HudBar.GUI.Templates
{
  public partial class A_LEDbar : UC_LedBar, IValue, IColorType, IAlign
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected GUI_Colors.ColorType m_foreColor = GUI_Colors.ColorType.cInfo; // default and LED OFF
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;
    protected readonly Color c_OffColor = Color.MidnightBlue;

    // Generic Value Interfaces are not supported - the Value Interface is needed to register them properly in the Bar
    // The UserControl provides it's own Data Properties that must be user
    /// <summary>
    /// Not used
    /// </summary>
    public float? Value { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Set the integer Value
    /// </summary>
    public int? IntValue {
      set {
        if (value == null) {
          base.Enabled = false;
          base.OnState = false;
        }
        else {
          base.Enabled = true;
          base.OnState = true;
          base.BarPrct = (int)value;
        }
      }
    }

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
    /// Get; Set the items Foreground Color UpWind
    /// cInfo Switches the LED OFF
    /// </summary>
    public GUI_Colors.ColorType ItemForeColor {
      get => m_foreColor;
      set {
        m_foreColor = value;
      }
    }
    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    public GUI_Colors.ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    public void UpdateColor( )
    {
      this.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }

    // Implement IValue Metric IF (but it is not used)
    public bool Altitude_metric { get => false; set => _ = value; }
    public bool Speed_metric { get => false; set => _ = value; }
    public bool Distance_metric { get => false; set => _ = value; }


    /// <summary>
    /// cTor: Create a UserControl based on a prototype control
    /// </summary>
    public A_LEDbar( )
    {
      base.BarElements = 11;
      base.HoldMax = true;
      base.OnState = true; // always ON for 'off' color

      ItemForeColor = GUI_Colors.ColorType.cInfo; // off
      ItemBackColor = GUI_Colors.ColorType.cBG; // force our common BG color here
      UpdateColor( );

      AutoSize = false;  // MUST REMAIN FALSE !!! else the Sizing does not work properly
      AutoSizeHeight = false;   // The HudBar default behavior is scaling with fixed Height
      AutoSizeWidth = true;     //   and flexible Width
      Cursor = Cursors.Default;                 // avoid the movement cross on the item controls
      Text = ""; // there is no Text supported
      BorderStyle = BorderStyle.None;
      Margin = new Padding( 0, 0, 3, 0 ); // right only

      GUI_Colors.Register( this );
    }

  }
}
