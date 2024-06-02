using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using FSimClientIF;
using System.Numerics;
using System.Drawing;
using System.Diagnostics;

namespace FCamControl
{
  /// <summary>
  /// A Camera Setting Slot
  /// </summary>
  internal class Slot
  {
    private int _slotFolderNo = -1; // may be removed later- for debug it helps to know where we are...

    private int _slotNo = -1;
    private CameraMode _camMode = CameraMode.NONE;
    private CameraSetting _camSetting = CameraSetting.NONE;
    private int _camIndex = 1;
    private int _zoomLevel = -1; // asIs zoom level or zoom not supported

    private Vector3 _position = new Vector3( );
    private Vector3 _gimbal = new Vector3( );

    private Button _button = null; // V1
    private HandledButton _handledButton = null; // V2

    // get the cam controller to save
    private Camera _cameraRef;  // V2

    // Wether or not the next click is a Slot Save action
    private bool _expectSlotSave = false;
    // Wether or not the next click is a Slot Clear action
    private bool _expectSlotClear = false;

    /// <summary>
    /// Fired when a Slot is saved
    /// </summary>
    public event EventHandler<EventArgs> SlotSaved;
    /// <summary>
    /// Fired when a Slot is cleared
    /// </summary>
    public event EventHandler<EventArgs> SlotCleared;


    /// <summary>
    /// cTor: Create a new Slot from Arguments V2
    /// </summary>
    /// <param name="slotFolderNo">The Slot Folder Number of this slot</param>
    /// <param name="slotNo">The Slot Number of this slot</param>
    /// <param name="button">Button Ref to capture the click event</param>
    /// <param name="camera">Camera</param>
    public Slot( uint slotFolderNo, uint slotNo, HandledButton button, Camera camera )
    {
      _slotFolderNo = (int)slotFolderNo;
      _slotNo = (int)slotNo;
      _handledButton = button;
      _button = _handledButton.Button;
      _cameraRef = camera;
      _button.Click += _button_Click;
    }

    /// <summary>
    /// Wether or not this Slot is Active
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// True if this slot has a valid setting
    /// </summary>
    public bool HasSetting => _camMode != CameraMode.NONE;

    /// <summary>
    /// Apply the setting visual to the button
    /// </summary>
    public void MaintainButtonState( )
    {
      if (_handledButton != null) {
        // V2
        _handledButton.Activate( HasSetting );
      }
      else {
        // V1
        _button.ForeColor = HasSetting ? Color.DarkTurquoise : Color.Teal;
      }
    }

    /// <summary>
    /// To trigger slot saving
    /// </summary>
    public void ExpectSlotSave( )
    {
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
    /// To trigger slot clearing
    /// </summary>
    public void ExpectSlotClear( )
    {
      _expectSlotClear = true;
    }

    /// <summary>
    /// To cancel slot clearing
    /// </summary>
    public void CancelSlotClear( )
    {
      _expectSlotClear = false;
    }

    // handle the click of this button
    private void _button_Click( object sender, EventArgs e )
    {
      if (Enabled == false) return; // handle only enabled buttons (all folder buttons are either enabled or disabled)

      // does not need a camera obj
      if (_expectSlotClear) {
        ClearSlot( );
        MaintainButtonState( );
        SlotCleared?.Invoke( this, new EventArgs( ) );
        return;
      }

      // sanity for switch and save
      if (_cameraRef == null) return;

      if (_expectSlotSave) {
        SaveCamera( );
        MaintainButtonState( );
        SlotSaved?.Invoke( this, new EventArgs( ) );
        return;
      }

      // switch valid buttons
      if (HasSetting) SwitchCamera( );
    }

    // Saves the current state for this slot
    private void SaveCamera( )
    {
      if (!Enabled) return;

      if (_cameraRef != null) {
        _camMode = _cameraRef.CameraAPI.CurrentCamMode;
        _camIndex = _cameraRef.CameraAPI.CamValueAPI.ViewIndex;
        _zoomLevel = _cameraRef.CameraAPI.CamValueAPI.ZoomLevel;
        _position = _cameraRef.CameraAPI.CamValueAPI.Cam6DofPosition;
        _gimbal = _cameraRef.CameraAPI.CamValueAPI.Cam6DofGimbal;
      }
    }

    // reset slot to cleared defaults
    private void ClearSlot( )
    {
      _camMode = CameraMode.NONE;
      _camSetting = CameraSetting.NONE;
      _camIndex = -1;
      _zoomLevel = -1;
      _position = new Vector3( ); ;
      _gimbal = new Vector3( ); ;
    }


    /// <summary>
    /// Switch Cam to the slot data
    /// </summary>
    public void SwitchCamera( )
    {
      // sanity
      if (!Enabled) return;

      if (_cameraRef != null) {
        if (_camMode == CameraMode.DOF6) {
          _cameraRef.CameraAPI.CamRequestAPI.RequestSwitchCamera( _camMode, _camIndex, _position, _gimbal, false ); // don't lock from slots values
        }
        else {
          if (_zoomLevel >= 0) {
            _cameraRef.CameraAPI.CamRequestAPI.RequestSwitchCamera( _camMode, _camIndex, _zoomLevel );
          }
          else {
            _cameraRef.CameraAPI.CamRequestAPI.RequestSwitchCamera( _camMode, _camIndex );
          }
        }
      }
    }

    // AppSettings Tools support V2 format
    // format is per slot:  "[V2_]SlotNo;SettingNo;[6Dof|IndexNo];zoomlvl¦"
    // 6Dof: "xfyfzfpfbfhf"  xyzpbh: floats
    private static Regex c_slotRegex = new Regex( @"(?<v2>V2_)?(?<slot>\d+);(?<setting>\d+);(((?<dof6>([-]?[0-9]*\.?[0-9]+f){6})|(?<index>\d+))(;(?<zoom>\d{1,3}|-1))?)¦"
                                                    , RegexOptions.Compiled );


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
    /// Get; Set: The setting string for this slot
    /// </summary>
    public string SlotSettingString {
      get {
        return SlotSettingStringV2; // Upgrade to V2
        /*
        return (_camSetting == CameraSetting.Cam_6DOF)
          ? $"{_slotNo};{(int)_camSetting};{_position.X:##0.00}f{_position.Y:##0.00}f{_position.Z:##0.00}f{_gimbal.X:##0.00}f{_gimbal.Y:##0.00}f{_gimbal.Z:##0.00}f¦"
          : $"{_slotNo};{(int)_camSetting};{_camIndex}¦";
        */
      }
      set {
        Match match = c_rx.Match( value );
        if (match.Success) {
          bool isV2 = (match.Groups["v2"].Success);

          if (isV2) {
            SlotSettingStringV2 = value;
            return;
          }
          // Continue with V1
          _camSetting = (CameraSetting)int.Parse( match.Groups["setting"].Value );
          _camIndex = 1;
          _zoomLevel = -1;
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

        // Add the CameraMode for V2 use
        UpgradeToV2( );
      }
    }

    // add a camera Mode from cameraSetting - if defined
    private void UpgradeToV2( )
    {
      // sanity
      if (_camSetting == CameraSetting.NONE) return; // nothing to add
      if (_camMode != CameraMode.NONE) return; // shall not change

      switch (_camSetting) {
        case CameraSetting.Cockpit_Pilot:
          if (_camIndex == (int)PilotCamPosition.Pilot) _camMode = CameraMode.PilotView;
          else if (_camIndex == (int)PilotCamPosition.Close) _camMode = CameraMode.CloseView;
          else if (_camIndex == (int)PilotCamPosition.Landing) _camMode = CameraMode.LandingView;
          else if (_camIndex == (int)PilotCamPosition.Copilot) _camMode = CameraMode.CoPilotView;
          else {
            _camMode = CameraMode.PilotView; // default
            // dont handle custom as it was not supported in V1
          }
          break;
        case CameraSetting.Cockpit_Instrument:
          _camMode = CameraMode.InstrumentIndexed;
          break;
        case CameraSetting.Cockpit_Quick:
          _camMode = CameraMode.InstrumentQuick;
          break;
        case CameraSetting.Cockpit_Custom:
          _camMode = CameraMode.CustomCamera;
          break;
        case CameraSetting.Ext_Default:
          _camMode = CameraMode.ExternalFree;
          break;
        case CameraSetting.Ext_Quick:
          _camMode = CameraMode.ExternalQuick;
          break;
        case CameraSetting.ShCase_Drone:
          _camMode = CameraMode.Drone;
          break;
        case CameraSetting.ShCase_Fixed:
          _camMode = CameraMode.ExternalIndexed;
          break;
        case CameraSetting.Cam_6DOF:
          _camMode = CameraMode.DOF6;
          break;

        default:
          // do nothing
          break;
      }
    }

    /// <summary>
    /// Get; Set: The setting string for this slot as V2 type of string
    /// V2 changed to CamereaMode and added the ZoomLevel
    /// </summary>
    private string SlotSettingStringV2 {
      get {
        return (_camMode == CameraMode.DOF6)
          // Added V2 tag
          ? $"V2_{_slotNo};{(int)_camMode};{_position.X:##0.00}f{_position.Y:##0.00}f{_position.Z:##0.00}f{_gimbal.X:##0.00}f{_gimbal.Y:##0.00}f{_gimbal.Z:##0.00}f;{_zoomLevel}¦"
          : $"V2_{_slotNo};{(int)_camMode};{_camIndex};{_zoomLevel}¦";
      }
      set {
        // this should be only called when a V2 string has to be handled
        Match match = c_rx.Match( value );
        if (match.Success) {
          // sanity
          bool isV2 = match.Groups["v2"].Success;
          Debug.Assert( isV2 );

          _camMode = (CameraMode)int.Parse( match.Groups["setting"].Value );
          _camIndex = 1;
          _zoomLevel = -1;
          _position = new Vector3( );
          _gimbal = new Vector3( );

          if ((_camMode == CameraMode.DOF6) && match.Groups["dof6"].Success) {
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
          // get Zoom if set
          if (match.Groups["zoom"].Success) {
            if (int.TryParse( match.Groups["zoom"].Value, out int zoom )) {
              _zoomLevel = dNetBm98.XMath.Clip( zoom, -1, 100 );
            }
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
