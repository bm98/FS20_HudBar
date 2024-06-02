using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WKeys = System.Windows.Forms.Keys;

namespace FCamControl
{
  /// <summary>
  /// Just a Catalog Type
  /// </summary>
  internal class MSFS_KeyCat : Dictionary<FS_Key, MSFS_Key>
  {

    #region Key Setup

    // supported MSFS Keys
    public readonly static MSFS_KeyCat sMSFS_Keys = new MSFS_KeyCat( ) {
     {FS_Key.DrMoveForward,  new MSFS_Key("TRANSLATE DRONE FORWARD",
       new WinHotkey( new WKeys[] { WKeys.W }), new WinHotkey( new WKeys[] { WKeys.W } ) ) },
     {FS_Key.DrMoveLeft,     new MSFS_Key("TRANSLATE DRONE LEFT",
       new WinHotkey( new WKeys[]{ WKeys.A } ), new WinHotkey( new WKeys[]{ WKeys.A } ) )},
     {FS_Key.DrMoveBackward, new MSFS_Key("TRANSLATE DRONE BACKWARD",
       new WinHotkey(new WKeys[]{ WKeys.S }), new WinHotkey( new WKeys[]{ WKeys.S } ) )},
     {FS_Key.DrMoveRight,    new MSFS_Key("TRANSLATE DRONE RIGHT",
       new WinHotkey(new WKeys[]{ WKeys.D }), new WinHotkey( new WKeys[]{ WKeys.D } ) )},
     {FS_Key.DrMoveUp,       new MSFS_Key("TRANSLATE DRONE UP",
       new WinHotkey(new WKeys[]{ WKeys.R } ), new WinHotkey( new WKeys[]{ WKeys.R } ) )},
     {FS_Key.DrMoveDown,     new MSFS_Key("TRANSLATE DRONE DOWN",
       new WinHotkey( new WKeys[]{ WKeys.F } ), new WinHotkey( new WKeys[]{ WKeys.F } ) )},

     {FS_Key.DrToggleControls,     new MSFS_Key("TOGGLE CONTROLS",
       new WinHotkey( new WKeys[]{ WKeys.C } ), new WinHotkey( new WKeys[]{ WKeys.C } ) )},

     {FS_Key.CustCam1,       new MSFS_Key("LOAD CUSTOM CAMERA 1",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D1 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D1 } ) )},
     {FS_Key.CustCam2,       new MSFS_Key("LOAD CUSTOM CAMERA 2",
       new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D2 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D2 } ) )},
     {FS_Key.CustCam3,       new MSFS_Key("LOAD CUSTOM CAMERA 3",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D3 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D3 } ) )},
     {FS_Key.CustCam4,       new MSFS_Key("LOAD CUSTOM CAMERA 4",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D4 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D4 } ) )},
     {FS_Key.CustCam5,       new MSFS_Key("LOAD CUSTOM CAMERA 5",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D5 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D5 } ) )},
     {FS_Key.CustCam6,       new MSFS_Key("LOAD CUSTOM CAMERA 6",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D6 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D6 } ) )},
     {FS_Key.CustCam7,       new MSFS_Key("LOAD CUSTOM CAMERA 7",
       new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D7 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D7 } ) )},
     {FS_Key.CustCam8,       new MSFS_Key("LOAD CUSTOM CAMERA 8",
       new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D8 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D8 } ) )},
     {FS_Key.CustCam9,       new MSFS_Key("LOAD CUSTOM CAMERA 9",
       new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D9 }), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D9 } ) )},
     {FS_Key.CustCam0,       new MSFS_Key("LOAD CUSTOM CAMERA 0",
       new WinHotkey(new WKeys[]{ WKeys.LMenu, WKeys.D0 } ), new WinHotkey( new WKeys[]{ WKeys.LMenu, WKeys.D0 } ) )},
     };


    #endregion

    /// <summary>
    /// cTor:
    /// </summary>
    public MSFS_KeyCat( ) { }

    /// <summary>
    /// cTor: Copy
    /// </summary>
    public MSFS_KeyCat( MSFS_KeyCat keyCat )
      : base( keyCat )
    { }

    /// <summary>
    /// Loads the default catalog
    /// </summary>
    public void LoadDefaultCat( )
    {
      this.Clear( );
      foreach (var item in sMSFS_Keys) {
        this.Add( item.Key, new MSFS_Key( item.Value ) ); // add as copy
      }
    }

    /// <summary>
    /// Load this Cat from a config string
    /// </summary>
    /// <param name="hkString">A config string</param>
    public void FromConfigString( string hkString )
    {
      LoadDefaultCat( );
      // empty config string
      if (string.IsNullOrEmpty( hkString )) return; // with default

      // take the ones which are valid
      string[] entries = hkString.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      for (int i = 0; i < entries.Length; i++) {
        string[] entry = entries[i].Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
        if (entry.Length > 1) {
          if (Enum.TryParse( entry[0], true, out FS_Key key )) {
            var winKey = new WinHotkey( entry[1] );
            if (winKey.isValid) {
              this[key] = new MSFS_Key( this[key].KeyName, winKey, this[key].DefaultHKey );
            }
          }
        }
      }
    }

    /// <summary>
    /// Return a Config string from this cat
    /// </summary>
    /// <returns>A Config String</returns>
    public string ToConfigString( )
    {
      var sb = new StringBuilder( );

      // Format is:
      // "entry;[entry;]"
      // entry: "FS_Key,hotkey"
      foreach (var item in this) {
        sb.Append( $"{item.Key},{item.Value.HKey.AsString};" );
      }

      return sb.ToString( );
    }

  }
}
