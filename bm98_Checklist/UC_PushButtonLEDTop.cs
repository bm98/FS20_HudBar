﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using bm98_Checklist.Properties;

namespace bm98_Checklist
{
  /// <summary>
  /// A Rectangular Pushbutton with LED illumination
  /// Triggers a SwitchChange event without state
  /// Use OnState Property to illuminate the LED
  /// </summary>
  public partial class UC_PushButtonLEDTop : UserControl
  {
    private bool m_track = false;
    private bool m_state = false;

    private SwitchColor m_colorON = SwitchColor.Red;
    private SwitchColor m_colorOFF = SwitchColor.Dark;
    private Image m_imageON = Resources.button_Rect_LEDstripe_red;
    private Image m_imageOFF = Resources.button_Rect_LEDstripe_off;

    /// <summary>
    /// Event triggered when the push button was clicked
    /// </summary>
    [Description( "Switch has changed its state" ), Category( "Action" )]
    public event EventHandler<MouseEventArgs> PushbuttonPressed;
    private void OnPushbuttonPressed( MouseEventArgs e )
    {
      PushbuttonPressed?.Invoke( this, e );
    }

    /// <summary>
    /// Event triggered when the mouse wheel changed 
    /// </summary>
    [Description( "Mouse Wheel action was detected" ), Category( "Action" )]
    public event EventHandler<MouseWheelActionArgs> MouseWheelChanged;
    private void OnMouseWheelChanged( MouseWheelAction mouseWheelAction )
    {
      MouseWheelChanged?.Invoke( this, new MouseWheelActionArgs( mouseWheelAction ) );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_PushButtonLEDTop( )
    {
      InitializeComponent( );
      lblText.Dock = DockStyle.Fill;
      lblText.Padding = Helper.BtPadding;
      lblText.Text = "";
    }

    private void lblText_Click( object sender, EventArgs e )
    {
      ; // empty - only to capture mouse events
    }

    private void lblText_MouseEnter( object sender, EventArgs e )
    {
      if (m_track) {
        lblText.BackColor = Helper.FogColor;
      }
    }

    private void lblText_MouseLeave( object sender, EventArgs e )
    {
      lblText.BackColor = Helper.ClearColor;
    }
    private void lblText_MouseDown( object sender, MouseEventArgs e )
    {
      OnPushbuttonPressed( e ); // Triggers the PButton Event
    }

    private void lblText_MouseUp( object sender, MouseEventArgs e )
    {
    }

    /// <summary>
    /// The LED color of the ON state
    /// </summary>
    [Description( "Pushbutton ON Color" ), Category( "Appearance" )]
    public SwitchColor PushOnColor {
      get => m_colorON;
      set {
        m_colorON = value;
        switch (m_colorON) {
          case SwitchColor.Red: m_imageON = Resources.button_Rect_LEDstripe_red; break;
          case SwitchColor.Green: m_imageON = Resources.button_Rect_LEDstripe_green; break;
          case SwitchColor.Blue: m_imageON = Resources.button_Rect_LEDstripe_lightblue; break;
          case SwitchColor.Yellow: m_imageON = Resources.button_Rect_LEDstripe_yellow; break;
          case SwitchColor.White: m_imageON = Resources.button_Rect_LEDstripe_white; break;
          case SwitchColor.Amber: m_imageON = Resources.button_Rect_LEDstripe_amber; break;
          case SwitchColor.Dark: m_imageON = Resources.button_Rect_LEDstripe_off; break;
          default: m_imageON = Resources.button_Rect_LEDstripe_red; break;
        }
        this.BackgroundImage = (m_state) ? m_imageON : m_imageOFF;
      }
    }

    /// <summary>
    /// The LED color of the OFF state
    /// </summary>
    [Description( "Pushbutton OFF Color" ), Category( "Appearance" )]
    public SwitchColor PushOffColor {
      get => m_colorOFF;
      set {
        m_colorOFF = value;
        switch (m_colorOFF) {
          case SwitchColor.Red: m_imageOFF = Resources.button_Rect_LEDstripe_red; break;
          case SwitchColor.Green: m_imageOFF = Resources.button_Rect_LEDstripe_green; break;
          case SwitchColor.Blue: m_imageOFF = Resources.button_Rect_LEDstripe_lightblue; break;
          case SwitchColor.Yellow: m_imageOFF = Resources.button_Rect_LEDstripe_yellow; break;
          case SwitchColor.White: m_imageOFF = Resources.button_Rect_LEDstripe_white; break;
          case SwitchColor.Amber: m_imageOFF = Resources.button_Rect_LEDstripe_amber; break;
          case SwitchColor.Dark: m_imageOFF = Resources.button_Rect_LEDstripe_off; break;
          default: m_imageOFF = Resources.button_Rect_LEDstripe_off; break;
        }
        this.BackgroundImage = (m_state) ? m_imageON : m_imageOFF;
      }
    }

    /// <summary>
    /// The text shown ontop of the button
    /// </summary>
    [Description( "The text associated with the control" ), Category( "Appearance" )]
    public string ButtonText {
      get => lblText.Text;
      set {
        lblText.Text = value;
      }
    }

    /// <summary>
    /// Indicates the state of LED (on, Off)
    /// </summary>
    [Description( "Illumination state (True=ON)" ), Category( "Behavior" )]
    public bool OnState {
      get => m_state;
      set {
        m_state = value;
        this.BackgroundImage = (m_state) ? m_imageON : m_imageOFF;
      }
    }

    /// <summary>
    /// Indicates wether the control gives feedback when the mouse is moved over it
    /// </summary>
    [Description( "Indicates wether the control gives feedback when the mouse is moved over it" ), Category( "Behavior" )]
    public bool HotTracking {
      get => m_track;
      set {
        m_track = value;
      }
    }

    private void lblText_ClientSizeChanged( object sender, EventArgs e )
    {
#if DEBUG
      Debug.WriteLine( $"BT {this.Name} - Label Size: {lblText.Size} - ClientSize: {lblText.ClientSize - lblText.Padding.Size}" );
#endif
    }
  }
}
