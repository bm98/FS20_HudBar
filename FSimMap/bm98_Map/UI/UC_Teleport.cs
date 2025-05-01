using System;
using System.Windows.Forms;

namespace bm98_Map.UI
{
  /// <summary>
  /// UC supporting Teleporting
  /// </summary>
  public partial class UC_Teleport : UserControl
  {
    private bool _altMsl = true;
    private int _altitude_ft = 0;


    /// <summary>
    /// Fired when Teleport Button was pressed
    /// </summary>
    public event EventHandler<EventArgs> TeleportPressed;
    private void OnTeleportPressed( ) => TeleportPressed?.Invoke( this, EventArgs.Empty );

    // format the Alt field
    private void FormatAlt( )
    {
      txAlt.Text = $"{_altitude_ft:#####0}";
    }

    // capture the Alt field
    private bool CaptureAlt( )
    {
      _altMsl = rbMSL.Checked;

      if (int.TryParse( txAlt.Text, out int value )) {
        if (value < 0) {
          ; // bing - less than zero... so don't change
          FormatAlt( ); // show 
          return false;
        }
        // seems legit
        _altitude_ft = value;
        FormatAlt( ); // show 
        return true;
      }
      else {
        ; // bing - not a number... so don't change
        FormatAlt( ); // show 
        return false;
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Teleport( )
    {
      InitializeComponent( );

    }

    /// <summary>
    /// True for AltMSL indicator, else AOG
    /// </summary>
    public bool AltMSL {
      get => _altMsl;
      set {
        _altMsl = value;
        rbAOG.Checked = !_altMsl;
        rbMSL.Checked = _altMsl;
      }
    }

    /// <summary>
    /// The Altitude Value
    /// </summary>
    public int Altitude_ft {
      get => _altitude_ft;
      set {
        _altitude_ft = value;
        FormatAlt( );
      }
    }

    private void btTeleport_Click( object sender, EventArgs e )
    {
      if (CaptureAlt( )) {
        OnTeleportPressed( );
      }
      else {
        // not a valid entry 
      }
    }
  }
}
