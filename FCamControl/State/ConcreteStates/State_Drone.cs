using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dNetBm98.Win;

using FSimClientIF;

using SC = SimConnectClient;
using static FSimClientIF.Sim;
using static dNetBm98.Win.WinKbdSender;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// Showcase - FreeView - Index 1 Drone Cam
  /// </summary>
  internal class State_Drone : AState
  {
    /// <summary>
    /// True if we are in a View that needs a reset for the Drone Cam
    /// </summary>
    public static bool NeedsExternalViewReset( CameraMode cameraMode )
    {
      // some external views leave the drone reset position in a spot not behind the acft.
      // other don't affect the drone reset position.
      // So reply when the ext view must be reset for the drone flyby to work properly
      switch (cameraMode) {
        case CameraMode.ExternalQuick:
        case CameraMode.ExternalIndexed:
        case CameraMode.ExternalFree:
          return true;

        case CameraMode.PilotView:
        case CameraMode.CloseView:
        case CameraMode.LandingView:
        case CameraMode.CoPilotView:
        case CameraMode.CustomCamera:
        case CameraMode.InstrumentQuick:
        case CameraMode.InstrumentIndexed:
        case CameraMode.Drone:
        case CameraMode.DOF6:
        default:
          return false;
      }
    }


    // Toggle Controls keystroke
    private KbdStroke _keyToggleControls = new KbdStroke( );
    private int _keyCatVersion = -1;

    // Used to issue the Control Toggle command to the Sim (only possible via keystroke)
    private WinKbdSender _kbd = null;

    // local store 
    private float _droneZoom = 0;
    private float _droneMovement = 0;
    private float _droneRotation = 0;

    // temp store while resetting the drone
    private float _storedDroneSpeed = 4;  // initial values as the sim has them
    private float _storedDroneRotation = 50; // initial values as the sim has them
    private float _storedDroneZoom = 50; // initial values as the sim has them

    /// <summary>
    /// True when Zoom is available
    /// </summary>
    public override bool CanZoom => true;

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    public override int ViewIndex => -1;

    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => -1;

    /// <summary>
    /// Zoom Level 0..100 or -1 if no Zoom is supported for this Camera
    /// </summary>
    public override int ZoomLevel => (int)SV.Get<float>( SItem.fGS_Cam_Drone_zoomlevel );

    /// <summary>
    /// Drone Lock mode, false if not supported
    /// </summary>
    public override bool DroneLock => SV.Get<bool>( SItem.bGS_Cam_Drone_locked );

    /// <summary>
    /// Drone Follow mode, false if not supported
    /// </summary>
    public override bool DroneFollow => SV.Get<bool>( SItem.bGS_Cam_Drone_follow );

    /// <summary>
    /// Drone Move Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    public override int DroneMoveSpeed => (int)SV.Get<float>( SItem.fGS_Cam_Drone_movespeed );

    /// <summary>
    /// Drone Rot Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    public override int DroneRotSpeed => (int)SV.Get<float>( SItem.fGS_Cam_Drone_rotspeed );


    /// <summary>
    /// cTor:
    /// </summary>
    public State_Drone( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    /// <summary>
    /// Trigger State Init
    /// </summary>
    /// <param name="prevMode">The state comming from</param>
    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );

      // reload keycatalog if it has changed
      if (_contextRef.MSFS_KeyCatalogVersion > _keyCatVersion) {
        // load from Context
        _keyToggleControls = _contextRef.MSFS_KeyCatalog[FS_Key.DrToggleControls].AsStroke( MSFS_Key.c_KeyDelay ); ;
        _keyCatVersion = _contextRef.MSFS_KeyCatalogVersion;
      }


      _droneZoom = SV.Get<float>( SItem.fGS_Cam_Drone_zoomlevel );
      _droneMovement = SV.Get<float>( SItem.fGS_Cam_Drone_movespeed );
      _droneRotation = SV.Get<float>( SItem.fGS_Cam_Drone_rotspeed );

      if (_firstInitDone) return;

      // first init stuff
      _kbd = new WinKbdSender( );
      _firstInitDone = true;
    }


    // Outside updates


    /// <summary>
    /// Received a Zoomlevel indication
    /// </summary>
    /// <param name="zoomlevel">Zoomlevel 0..100</param>
    public override void OnZoomLevel( int zoomlevel )
    {
      base.OnZoomLevel( zoomlevel );

      _droneZoom = zoomlevel;
    }
    /// <summary>
    /// Received a Drone Move Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    public override void OnDroneMoveSpeed( int speed )
    {
      base.OnDroneMoveSpeed( speed );

      _droneMovement = speed;
    }
    /// <summary>
    /// Received a Drone Rotation Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    public override void OnDroneRotSpeed( int speed )
    {
      base.OnDroneRotSpeed( speed );

      _droneRotation = speed;
    }


    // App Code Requests

    /// <summary>
    /// Requests a Cam Reset call
    /// </summary>
    public override void RequestResetCamera( )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _storedDroneSpeed = SV.Get<float>( SItem.fGS_Cam_Drone_movespeed );
      _storedDroneRotation = SV.Get<float>( SItem.fGS_Cam_Drone_rotspeed );
      _storedDroneZoom = SV.Get<float>( SItem.fGS_Cam_Drone_zoomlevel );

      base.RequestResetCamera( );

      RequestRestoreDroneValues( );
    }

    /// <summary>
    /// Drone Lock Target is requested
    /// </summary>
    /// <param name="locked">Lock Mode</param>
    public override void RequestDroneLock( bool locked )
    {
      base.RequestDroneLock( locked );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set<bool>( SItem.bGS_Cam_Drone_locked, locked );
    }

    /// <summary>
    /// Drone Follow Target is requested
    /// </summary>
    /// <param name="follow">Follow mode</param>
    public override void RequestDroneFollow( bool follow )
    {
      base.RequestDroneFollow( follow );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set<bool>( SItem.bGS_Cam_Drone_follow, follow );
    }

    /// <summary>
    /// A Zoomlevel is requested
    /// </summary>
    /// <param name="zoomLevel">Zoomlevel 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public override void RequestZoomLevel( int zoomLevel, bool untracked = false )
    {
      base.RequestZoomLevel( zoomLevel );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _droneZoom = untracked ? _droneZoom : zoomLevel; // dont track if requested
      SV.Set<float>( SItem.fGS_Cam_Drone_zoomlevel, (float)zoomLevel );
    }

    /// <summary>
    /// Drone Move Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public override void RequestDroneMoveSpeed( int speed, bool untracked = false )
    {
      base.RequestDroneMoveSpeed( speed );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _droneMovement = untracked ? _droneMovement : speed; // dont track if requested
      SV.Set<float>( SItem.fGS_Cam_Drone_movespeed, (float)speed );
    }

    /// <summary>
    /// Drone Rotation Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public override void RequestDroneRotSpeed( int speed, bool untracked = false )
    {
      base.RequestDroneRotSpeed( speed );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _droneRotation = untracked ? _droneRotation : speed; // dont track if requested
      SV.Set<float>( SItem.fGS_Cam_Drone_rotspeed, (float)speed );
    }

    /// <summary>
    /// Toggle Control Input while in Drone Cam
    /// </summary>
    public override void RequestDroneToggleControls( )
    {
      base.RequestDroneToggleControls( );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _kbd.AddStroke( _keyToggleControls );
      _kbd.RunStrokes( MSFS_Key.c_SimWindowTitle, blocking: false );
    }


    // set values to tracked values
    public void RequestRestoreDroneValues( )
    {
      SV.Set<float>( SItem.fGS_Cam_Drone_zoomlevel, _storedDroneZoom );
      SV.Set<float>( SItem.fGS_Cam_Drone_movespeed, _storedDroneSpeed );
      SV.Set<float>( SItem.fGS_Cam_Drone_rotspeed, _storedDroneRotation );
    }

  }
}
