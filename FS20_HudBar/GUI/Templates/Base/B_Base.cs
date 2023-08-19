using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.GUI.Templates.Base
{
  /// <summary>
  /// Base class to implement Clickable Buttons with labels
  /// Mostly derived from the V_Base Class
  /// </summary>
  abstract class B_Base : Button, IValue, IColorType
  {
    protected VItem m_mID = VItem.Ad;
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected bool m_scrollable = false;

    protected GUI_Colors.ColorType m_foreColorType = GUI_Colors.ColorType.cTxLabel;
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;

    private const string c_numbers = "0123456789";
    private Random random = new Random( );

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
        base.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public GUI_Colors.ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
        base.BackColor = GUI_Colors.ItemColor( m_backColorType );
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    virtual public void UpdateColor( )
    {
      base.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      base.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }

    /// <summary>
    /// True if the control captures the focus for scrolling support
    /// </summary>
    public bool Scrollable { get => m_scrollable; set => m_scrollable = value; }


    // Implement IValue Metric IF (but it is not used)
    public bool Altitude_metric { get => false; set => _ = value; }
    public bool Speed_metric { get => false; set => _ = value; }
    public bool Distance_metric { get => false; set => _ = value; }

    /// <summary>
    /// Event triggered when the push button was clicked
    /// Sends the VItem ID set in the cTor
    /// </summary>
    public event EventHandler<ClickedEventArgs> ButtonClicked;
    private void OnButtonClicked( )
    {
      ButtonClicked?.Invoke( this, new ClickedEventArgs( m_mID ) );
    }

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
      return valueString + (m_showUnit ? m_unit : "");
    }

    /// <summary>
    /// Debugging support, provide the default string with some numbers replacing _ (placeholder) chars
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string DefaultString( string defaultString )
    {
      string ret = defaultString;
#if DEBUG
      ret = "";

      for (int i = 0; i < defaultString.Length; i++) {
        if (defaultString[i] == '_') {
          ret += c_numbers[random.Next( 10 )];
        }
        else {
          ret += defaultString[i];
        }
      }
#endif
      return ret;
    }


    /* USING the improved method from dNetBm98.WinUser
    /// <summary>
    /// Activates the Main Form in order to prevent that further (Mouse) events are sent to the prev. Active Application
    /// This is used to switch to the HudBar for Mouse Wheel scrolling.
    /// MSFS however will still capture the first scroll event as it receives the Event in parallel to the Mouse hovered control of the HudBar
    /// But at least then it will stop getting further scroll events and usually Zoom in/out like crazy...
    ///     basically one would be able to send a reverse scroll to Windows before this but it seems rather complicated to do so....
    /// </summary>
    /// <param name="e">The MouseEvents</param>
    internal void ActivateForm( MouseEventArgs e )
    {
      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      //  NOTE: this will not prevent a single scroll event captured by DirectInput i.e. the Sim however...
      if (Form.ActiveForm == null) {
        this.FindForm( ).Activate( );
      }
      (e as HandledMouseEventArgs).Handled = true; // don't bubble up the scroll wheel
    }
    */

    /// <summary>
    /// cTor: Create a UserControl..
    /// </summary>
    /// <param name="item">The VITem ID of this new Control</param>
    /// <param name="proto">A label Prototype to derive from</param>
    public B_Base( VItem item, Label proto )
    {
      m_mID = item;
      // Label props
      Font = proto.Font;
      ItemForeColor = GUI_Colors.ColorType.cTxLabel; // forced BUTTONS get a cTxLabel Foreground
      ItemBackColor = GUI_Colors.ColorType.cActBG; // forced BUTTONS get a c_ActBG Background
      AutoSize = true;                // force AutoSize
      TextAlign = proto.TextAlign;
      Anchor = proto.Anchor;
      Dock = proto.Dock;
      Margin = proto.Margin;
      Padding = proto.Padding;
      Text = m_default;
      UseCompatibleTextRendering = true; // make sure the WingDings an other font special chars display properly
      // Button props
      AutoSizeMode = AutoSizeMode.GrowAndShrink;
      FlatStyle = FlatStyle.Flat;
      FlatAppearance.BorderSize = 0;
      FlatAppearance.BorderColor = BackColor;
      FlatAppearance.MouseDownBackColor = GUI_Colors.c_ActPressed;  // forced 
      Cursor = Cursors.Hand; // forced: actionable
      TabStop = false; // forced, no TabStop

      GUI_Colors.Register( this );

      // capture Mouse Click Event
      base.MouseClick += Control_Click;

      // capture mouse entering to switch the active window
      base.MouseEnter += Control_MouseEnter;
      base.MouseLeave += Control_MouseLeave;
    }

    private void Control_MouseEnter( object sender, EventArgs e )
    {
      if (m_scrollable) dNetBm98.WinUser.PushAndSetForeground( this );
    }

    private void Control_MouseLeave( object sender, EventArgs e )
    {
      if (m_scrollable) dNetBm98.WinUser.PopForeground( );
    }


    // subst with our own handler that submits our ID
    private void Control_Click( object sender, EventArgs e )
    {
      OnButtonClicked( );
    }


  }
}
