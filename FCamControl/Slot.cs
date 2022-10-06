using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FSimClientIF;
using SC = SimConnectClient;

namespace FCamControl
{
  /// <summary>
  /// A Camera Setting Slot
  /// </summary>
  internal class Slot
  {
    private int _slotFolderNo = -1; // may be removed later- for debug it helps to know where we are...

    private int _slotNo = -1;
    private CameraSetting _camSetting = CameraSetting.NONE;
    private uint _camIndex = 1;
    private Button _button = null;

    private Action<CameraSetting, uint> _switchCam;

    // Declare the delegate (if using non-generic pattern).
    public delegate void SlotSavedEventHandler( object sender, EventArgs e );
    public event SlotSavedEventHandler SlotSaved;

    /// <summary>
    /// Wether or not this Slot is Active
    /// </summary>
    public bool Enabled { get; set; } = false;
    /// <summary>
    /// Wether or not the next click is a Slot Save action
    /// </summary>
    public bool ExpectSlotSave { get; set; } = false;

    /// <summary>
    /// cTor: Create a new Slot from Arguments
    /// </summary>
    /// <param name="slotFolderNo">The Slot Folder Number of this slot</param>
    /// <param name="slotNo">The Slot Number of this slot</param>
    /// <param name="switchCam">Camera Switch Method</param>
    public Slot( uint slotFolderNo, uint slotNo, Button button, Action<CameraSetting,uint> switchCam )
    {
      _slotFolderNo = (int)slotFolderNo;
      _slotNo = (int)slotNo;
      _button = button;
      _button.Click += _button_Click;
      _switchCam = switchCam;
    }

    // handle the click of this button
    private void _button_Click( object sender, EventArgs e )
    {
      if (ExpectSlotSave) {
        SaveCamera( );
        SlotSaved?.Invoke( this, new EventArgs( ) );
      }
      else {
        SwitchCamera( );
      }
    }

    /// <summary>
    /// Saves the current state for this slot
    /// </summary>
    public void SaveCamera( )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return; // cannot
      if (!Enabled) return;

      _camSetting = SC.SimConnectClient.Instance.CameraModule.CameraSetting;
      _camIndex = SC.SimConnectClient.Instance.CameraModule.ActCamView_index;
    }

    /// <summary>
    /// Switch Cam to the slot data
    /// </summary>
    public void SwitchCamera( )
    {
      // sanity
      if (!Enabled) return;

      _switchCam( _camSetting, _camIndex ); // use supplied Method
    }

    // AppSettings Tools
    // format is per slot:  "SlotNo;SettingNo;IndexNo¦"

    private static Regex c_slotRegex = new Regex( @"(?<slot>\d+);(?<setting>\d+);(?<index>\d+)¦?", RegexOptions.Compiled );
    /// <summary>
    /// Regex to get Slots from Setting
    /// </summary>
    internal static MatchCollection SlotMatches (string profileString )
    {
      return c_slotRegex.Matches( profileString );
    }

    // a Slot with starting requirement ^
    private static Regex c_rx = new Regex( $"^{c_slotRegex}", RegexOptions.Compiled );

    /// <summary>
    /// Get; Set: The AppSetting string for this Item
    /// </summary>
    public string AppSettingString {
      get {
        return $"{_slotNo};{(int)_camSetting};{_camIndex}";
      }
      set {
        Match match = c_rx.Match( value );
        if (match.Success) {
          _camSetting = (CameraSetting)int.Parse( match.Groups["setting"].Value );
          _camIndex = uint.Parse( match.Groups["index"].Value );
        }
      }
    }

    /// <summary>
    /// Returns the SlotNo of this AppSetting String
    /// </summary>
    /// <param name="appSetting">A Slot AppSetting</param>
    /// <returns>The SlotNo or -1 </returns>
    public static int SlotNo(string appSetting )
    {
      Match match = c_rx.Match( appSetting );
      if (match.Success) {
        return int.Parse( match.Groups["slot"].Value );
      }
      return -1;
    }

  }
}
