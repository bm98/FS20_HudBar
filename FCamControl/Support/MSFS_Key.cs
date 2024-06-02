using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Win.WinKbdSender;

namespace FCamControl
{
  /// <summary>
  /// Currently used Keys to send to MSFS
  /// </summary>
  internal enum FS_Key
  {
    DrMoveForward = 0,
    DrMoveLeft,
    DrMoveBackward,
    DrMoveRight,
    DrMoveUp,
    DrMoveDown,
    DrToggleControls,

    CustCam1, CustCam2, CustCam3, CustCam4, CustCam5, CustCam6, CustCam7, CustCam8, CustCam9, CustCam0
    // ADD NEW ONES AT THE END !!!!
  }


  /// <summary>
  /// Key used in MSFS
  /// </summary>
  internal class MSFS_Key
  {
    /// <summary>
    /// Window to send Keystrokes
    /// </summary>
    public const string c_SimWindowTitle = "Microsoft Flight Simulator";
    /// <summary>
    /// Regular Key Press time
    /// </summary>
    public const int c_KeyDelay = 50; // ms

    /// <summary>
    /// The descriptive name
    /// </summary>
    public string KeyName { get; private set; } = "";
    /// <summary>
    /// The WinHotKey
    /// </summary>
    public WinHotkey HKey { get; private set; } = new WinHotkey( );
    /// <summary>
    /// The MSFS Default WinHotKey
    /// </summary>
    public WinHotkey DefaultHKey { get; private set; } = new WinHotkey( );

    /// <summary>
    /// cTor: Copy
    /// </summary>
    public MSFS_Key( MSFS_Key other )
    {
      this.KeyName = other.KeyName;
      this.HKey = other.HKey.Copy( );
      this.DefaultHKey = other.DefaultHKey.Copy( );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public MSFS_Key( string name, WinHotkey key, WinHotkey @default )
    {
      KeyName = name;
      HKey = key;
      DefaultHKey = @default;
    }

    /// <summary>
    /// Set the HKey
    /// </summary>
    /// <param name="hKey">A WinHotKey</param>
    public void SetHKey( WinHotkey hKey )
    {
      HKey = hKey.Copy( );
    }

    /// <summary>
    /// Return the HotKey As KbdStroke with given delay
    /// </summary>
    /// <param name="delay_ms">Press, Release Delay in ms</param>
    /// <returns>A KbdStroke</returns>
    public KbdStroke AsStroke( int delay_ms )
    {
      return new KbdStroke( HKey.Key, delay_ms, HKey.Modifier.ToArray( ) );
    }

  }
}
