using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace bm98_Checklist
{
  /// <summary>
  /// A Rectangular Pushbutton
  /// Triggers a SwitchChange event without state
  /// </summary>  
  public partial class UC_PushButton : UserControl
  {
    private bool m_track = false;

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
    public UC_PushButton( )
    {
      InitializeComponent( );
      lblText.Dock = DockStyle.Fill;
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
