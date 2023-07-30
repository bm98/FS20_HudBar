using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FSimClientIF;
using SC = SimConnectClient;
using System.Numerics;
using System.Drawing;

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
    private int _camIndex = 1;

    private Vector3 _position = new Vector3( );
    private Vector3 _gimbal = new Vector3( );

    // proposed values
    private Vector3 _propPosition = new Vector3( );
    private Vector3 _propGimbal = new Vector3( );

    private Button _button = null;

    private Action<CameraSetting, int, Vector3, Vector3> _switchCam;

    // Wether or not the next click is a Slot Save action
    private bool _expectSlotSave = false;

    // Declare the delegate (if using non-generic pattern).
    public delegate void SlotSavedEventHandler( object sender, EventArgs e );
    public event SlotSavedEventHandler SlotSaved;

    /// <summary>
    /// Wether or not this Slot is Active
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// True if this slot has a valid setting
    /// </summary>
    public bool HasSetting => _camSetting != CameraSetting.NONE;

    /// <summary>
    /// Apply the setting visual to the button
    /// </summary>
    public void MaintainButtonState( )
    {
      _button.ForeColor = HasSetting ? Color.DarkTurquoise : Color.Teal;
    }

    /// <summary>
    /// To trigger slot saving
    /// </summary>
    /// <param name="position">The 6DOF position</param>
    /// <param name="gimbal">The 6DOF gimbal</param>
    public void ExpectSlotSave( Vector3 position, Vector3 gimbal )
    {
      _propPosition = position;
      _propGimbal = gimbal;
      _expectSlotSave = true;
    }

    /// <summary>
    /// To cancel slot saving
    /// </summary>
    public void CancelSlotSave( )
    {
      _expectSlotSave = false;
    }

    /// <summary>
    /// cTor: Create a new Slot from Arguments
    /// </summary>
    /// <param name="slotFolderNo">The Slot Folder Number of this slot</param>
    /// <param name="slotNo">The Slot Number of this slot</param>
    /// <param name="button">Button Ref to capture the click event</param>
    /// <param name="switchCam">Camera Switch Method</param>
    public Slot( uint slotFolderNo, uint slotNo, Button button, Action<CameraSetting, int, Vector3, Vector3> switchCam )
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
      if (_expectSlotSave) {
        SaveCamera( );
        MaintainButtonState( );
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

      _camSetting = SC.SimConnectClient.Instance.SimVarModule.Get<CameraSetting>( Sim.SItem.csetGS_Cam_Actual_setting );
      _camIndex = SC.SimConnectClient.Instance.SimVarModule.Get<int>( Sim.SItem.iGS_Cam_Viewindex );
      _position = _propPosition;
      _gimbal = _propGimbal;
    }

    /// <summary>
    /// Switch Cam to the slot data
    /// </summary>
    public void SwitchCamera( )
    {
      // sanity
      if (!Enabled) return;

      _switchCam( _camSetting, _camIndex, _position, _gimbal ); // use supplied Method
    }

    // AppSettings Tools
    // format is per slot:  "SlotNo;SettingNo;[6Dof|IndexNo]¦"
    // 6Dof: "xfyfzfpfbfhf"  xyzpbh: floats
    private static Regex c_slotRegex = new Regex( @"(?<slot>\d+);(?<setting>\d+);((?<dof6>([-]?[0-9]*\.?[0-9]+f){6})¦|(?<index>\d+)¦)", RegexOptions.Compiled );
    /// <summary>
    /// Regex to get Slots from Setting
    /// </summary>
    internal static MatchCollection SlotMatches( string profileString )
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
        return (_camSetting == CameraSetting.Cam_6DOF)
          ? $"{_slotNo};{(int)_camSetting};{_position.X:##0.00}f{_position.Y:##0.00}f{_position.Z:##0.00}f{_gimbal.X:##0.00}f{_gimbal.Y:##0.00}f{_gimbal.Z:##0.00}f¦"
          : $"{_slotNo};{(int)_camSetting};{_camIndex}¦";
      }
      set {
        Match match = c_rx.Match( value );
        if (match.Success) {
          _camSetting = (CameraSetting)int.Parse( match.Groups["setting"].Value );
          _camIndex = 1;
          _position = new Vector3( );
          _gimbal = new Vector3( );

          if ((_camSetting == CameraSetting.Cam_6DOF) && match.Groups["dof6"].Success) {
            string[] e = match.Groups["dof6"].Value.Split( new char[] { 'f' }, StringSplitOptions.RemoveEmptyEntries );
            try {
              _position = new Vector3( float.Parse( e[0] ), float.Parse( e[1] ), float.Parse( e[2] ) );
              _gimbal = new Vector3( float.Parse( e[3] ), float.Parse( e[4] ), float.Parse( e[5] ) );
            }
            catch (Exception) {
              ;// would be a mismatch of the setting string
            }
          }
          else if (match.Groups["index"].Success) {
            _camIndex = int.Parse( match.Groups["index"].Value );
          }
        }
      }
    }

    /// <summary>
    /// Returns the SlotNo of this AppSetting String
    /// </summary>
    /// <param name="appSetting">A Slot AppSetting</param>
    /// <returns>The SlotNo or -1 </returns>
    public static int SlotNo( string appSetting )
    {
      Match match = c_rx.Match( appSetting );
      if (match.Success) {
        return int.Parse( match.Groups["slot"].Value );
      }
      return -1;
    }

  }
}
