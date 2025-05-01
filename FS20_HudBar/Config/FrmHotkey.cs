using System;
using System.Windows.Forms;

using FS20_HudBar.Win;


namespace FS20_HudBar.Config
{
  internal partial class FrmHotkey : Form
  {
    // the handled item
    private WinHotkey _hotkey = new WinHotkey();

    /// <summary>
    /// Get; Set; The Hotkey description
    /// </summary>
    public string ProfileName {
      get => lblProfile.Text;
      set => lblProfile.Text = value;
    }

    /// <summary>
    /// Get; Set; the Hotkey
    /// </summary>
    public WinHotkey Hotkey {
      get => _hotkey;
      set {
        _hotkey.Clear( );
        _hotkey.AddRange( value );
        txEntry.Text = _hotkey.AsString;
      }
    }




    public FrmHotkey( )
    {
      InitializeComponent( );
    }

    // have it in one place only
    private void EvalCheckedState( )
    {
      if ( chkDisabled.Checked ) {
        txEntry.Text = "DISABLED";
        _hotkey.Clear( );
        txEntry.ReadOnly = true;
        chkDisabled.Select( );
      }
      else {
        txEntry.Text = Hotkey.AsString;
        txEntry.ReadOnly = false;
        txEntry.Select( );
      }
    }

    // Called when the Form is shown
    private void FrmHotkey_Load( object sender, EventArgs e )
    {
      chkDisabled.CheckState = Hotkey.isValid ? CheckState.Unchecked : CheckState.Checked;
      EvalCheckedState( );
      //txEntry.Focus( );
    }

    // enable / disable the entry
    private void chkDisabled_CheckedChanged( object sender, EventArgs e )
    {
      EvalCheckedState( );
      if ( chkDisabled.Checked )
        btAccept.Select( );
      else
        txEntry.Select();
    }

    // Evaluate the keys presed
    private void txEntry_PreviewKeyDown( object sender, PreviewKeyDownEventArgs e )
    {
      EvalKey( e );
    }

    // inhibit printing of any input
    private void txEntry_KeyPress( object sender, KeyPressEventArgs e )
    {
      e.Handled = true;
    }

    // load the textbox with our values
    private void txEntry_KeyUp( object sender, KeyEventArgs e )
    {
      txEntry.Text = _hotkey.AsString;
      e.Handled = true;
    }


    // Evaluate the Keys pressed 
    private void EvalKey( PreviewKeyDownEventArgs e )
    {
      Keys _shift = Keys.None;
      Keys _ctrl = Keys.None;
      Keys _alt = Keys.None;

      // get Left / Right Modifier Keys sorted out
      if ( e.Shift ) {
        if ( GetAsyncKeyState( Keys.LShiftKey ) < 0 ) _shift = Keys.LShiftKey;
        if ( GetAsyncKeyState( Keys.RShiftKey ) < 0 ) _shift = Keys.RShiftKey;
      }
      else {
        _shift = Keys.None;
      }
      if ( e.Control ) {
        if ( GetAsyncKeyState( Keys.LControlKey ) < 0 ) _ctrl = Keys.LControlKey;
        if ( GetAsyncKeyState( Keys.RControlKey ) < 0 ) _ctrl = Keys.RControlKey;
      }
      else {
        _ctrl = Keys.None;
      }
      if ( e.Alt ) {
        if ( GetAsyncKeyState( Keys.LMenu ) < 0 ) _alt = Keys.LMenu;
        if ( GetAsyncKeyState( Keys.RMenu ) < 0 ) _alt = Keys.RMenu;
      }
      else {
        _alt = Keys.None;
      }
      // Eval the current entry, assuming the entry sequence is FIRST Modifiers THEN action key..
      _hotkey.Clear( );

      if ( e.KeyCode == Keys.Return ) return;        // Return and NumPad Enter are the same in e.KeyCode, cannot use them

      if ( !( ( e.KeyCode == Keys.ControlKey )
        || ( e.KeyCode == Keys.ShiftKey )
        || ( e.KeyCode == Keys.Menu ) ) ) {

        // it is not only a modifier - so eval what is pressed alltogether
        if ( e.Shift ) _hotkey.Add( _shift );
        if ( e.Control ) _hotkey.Add( _ctrl );
        if ( e.Alt ) _hotkey.Add( _alt );
        // finally the keysd
        _hotkey.Add( e.KeyCode );
      }
      //txEntry.Text = _hotkey.AsString;
    }

    // Using the Win API here
    [System.Runtime.InteropServices.DllImport( "user32.dll" )]
    private static extern short GetAsyncKeyState( Keys key ); // MSB is set when the Key is Down aka <0 then

    private void btCancel_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.Cancel;
      this.Close( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.OK;
      this.Close( );
    }
  }
}
