using System;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// Form to config the MSFS Keys used
  /// </summary>
  internal partial class frmKeyConfig : Form
  {

    private MSFS_KeyCat _keys = new MSFS_KeyCat( );

    /// <summary>
    /// The actual Catalog of Keys
    /// </summary>
    public MSFS_KeyCat MSFS_Keys => _keys;


    /// <summary>
    /// Load a Catalog of Keys into the Dialog
    /// </summary>
    /// <param name="keyCat"></param>
    public void LoadKeyDict( MSFS_KeyCat keyCat )
    {
      _keys.Clear( );
      _keys = new MSFS_KeyCat( keyCat );

      flp.SuspendLayout( );

      // clean the Panel
      while (flp.Controls.Count > 0) {
        var x = flp.Controls[0];
        flp.Controls.RemoveAt( 0 );
        x.Dispose( );
      }

      foreach (var ke in _keys) {
        var ct = new UC_Hotkey( );
        ct.Name = ke.Key.ToString( );
        ct.MSFS_Key = ke.Value;
        flp.Controls.Add( ct );
      }

      flp.ResumeLayout( );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public frmKeyConfig( )
    {
      InitializeComponent( );

      // load key List
      LoadKeyDict( MSFS_KeyCat.sMSFS_Keys ); // defaults
    }

    private void frmKeyConfig_Load( object sender, EventArgs e )
    {
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.OK;
      this.Hide( );
    }

    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Hide( );
    }
  }
}
