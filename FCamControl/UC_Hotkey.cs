using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// One Hotkey
  /// </summary>
  internal partial class UC_Hotkey : UserControl
  {

    private MSFS_Key _key;
    private FrmHotkey _hkConfig;

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Hotkey( )
    {
      InitializeComponent( );
    }

    private void UC_Hotkey_Load( object sender, EventArgs e )
    {

    }


    /// <summary>
    /// Get;Set: The MSFS Key
    /// </summary>
    public MSFS_Key MSFS_Key {
      get { return _key; }
      set {
        if (value == null) return;

        _key = value;
        lblName.Text = _key.KeyName;
        if (!_key.HKey.isValid) {
          _key.SetHKey( _key.DefaultHKey );
        }
        txEntry.Text = _key.HKey.AsString;
      }
    }



    private void btDefault_Click( object sender, EventArgs e )
    {
      _key.SetHKey( _key.DefaultHKey );
      txEntry.Text = _key.DefaultHKey.AsString;
    }

    private void btConfig_Click( object sender, EventArgs e )
    {
      _hkConfig = new FrmHotkey {
        ProfileName = _key.KeyName,
        Hotkey = _key.HKey
      };

      if (_hkConfig.ShowDialog( this ) == DialogResult.OK) {
        _key.SetHKey( _hkConfig.Hotkey );
      }
      txEntry.Text = _key.HKey.AsString;

      _hkConfig.Dispose( );
    }

  }
}
