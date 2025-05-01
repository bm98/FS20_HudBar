using System;
using System.Diagnostics;

using SC = SimConnectClient;
using FSimClientIF;
using FSimClientIF.Modules;
using System.Numerics;
using static FSimClientIF.Sim;
using System.Threading;
using dNetBm98.Job;
using FCamControl.State.ConcreteStates;

namespace FCamControl.State
{
  /// <summary>
  /// Abstact class for a Camera State
  /// Implements all Interfaces either with function or dummy as fallback
  /// Switching the Camera is implemented here for all
  /// The cam has severe delays in changing and reporting it's state so we cannot rely 
  /// a certain state after only asking to change the camera - will not work when implemented in a concrete state
  /// </summary>
  internal abstract class AState : ICameraEvents, ICameraRequests, ICameraValues, IDisposable
  {

    // attach the property module - this does not depend on the connection established or not
    protected readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // CLASS IMPLEMENTATION

    // the Context obj ref (set in constructor)
    protected readonly Context _contextRef;
    // State of the implemented Class (set in constructor)
    protected readonly CameraMode _state;

    // previous state (set during base.OnInit()
    protected CameraMode _prevState;

    // a state may need to differentiate...
    protected bool _firstInitDone = false;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="mode">Initial State</param>
    /// <param name="context">Ref to Context obj</param>
    public AState( CameraMode mode, Context context )
    {
      _state = mode;
      _contextRef = context;
    }

    /// <summary>
    /// Returns the State Enum
    /// </summary>
    public CameraMode State => _state;


    #region Implement ICameraValues

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    public virtual int ViewIndex => SV.Get<int>( SItem.iGS_Cam_Viewindex );         // DEFAULT IMPLEMENTATION - some may need to overwrite this


    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public virtual int MaxViewIndex {
      get {
        // DEFAULT IMPLEMENTATION - some may need to overwrite this
        // get the max index for the views available
        var vt = SV.Get<CameraViewType>( SItem.cvtG_Cam_Viewtype );
        switch (vt) {
          case CameraViewType.Unknown_default: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_default );
          case CameraViewType.PilotView: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_pilot );
          case CameraViewType.InstrumentView: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_instrument );
          case CameraViewType.Quickview: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_quick );
          case CameraViewType.Quickview_External: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_external );
          case CameraViewType.OtherView: return SV.Get<int>( SItem.iG_Cam_Viewindex_max_other );
          default:
            return SV.Get<int>( SItem.iG_Cam_Viewindex_max_default );
        }

      }
    }

    /// <summary>
    /// Returns the SmartTarget Index 0..N or -1 if not active
    /// </summary>
    public virtual int SmartTargetIndex {
      get {
        if (SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
          return SV.Get<int>( SItem.iGS_Cam_Smart_targetindex_selected );
        }
        return -1;
      }
    }

    /// <summary>
    /// True when Zoom is available
    /// </summary>
    public virtual bool CanZoom => false; // defaults to false

    /// <summary>
    /// True when SmartTarget is available
    /// </summary>
    public virtual bool CanSmartTarget => false; // defaults to false


    /// <summary>
    /// Zoom Level 0..100 or -1 if no Zoom is supported for this Camera
    /// </summary>
    public virtual int ZoomLevel => -1; // defaults to not avail

    /// <summary>
    /// Drone Lock mode, false if not supported
    /// </summary>
    public virtual bool DroneLock => false;

    /// <summary>
    /// Drone Follow mode, false if not supported
    /// </summary>
    public virtual bool DroneFollow => false;

    /// <summary>
    /// Drone Move Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    public virtual int DroneMoveSpeed => -1;

    /// <summary>
    /// Drone Rot Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    public virtual int DroneRotSpeed => -1;

    /// <summary>
    /// Returns the current 6DOF Position
    /// </summary>
    public virtual Vector3 Cam6DofPosition => new Vector3( );
    /// <summary>
    /// Returns the current 6DOF Gimbal
    /// </summary>
    public virtual Vector3 Cam6DofGimbal => new Vector3( );
    /// <summary>
    /// Returns the current 6DOF Camera LockMode
    /// </summary>
    public virtual bool Cam6DofLocked => false;

    #endregion

    #region Implement IStateEvents

    // Handlers for all events - default to logging for requests
    // Must be implemented by States when applicable

    /// <summary>
    /// Trigger State Init
    /// </summary>
    /// <param name="prevState">The state comming from</param>
    public virtual void OnInit( CameraMode prevState )
    {
      _prevState = prevState;

      Debug.WriteLine( $"Camera State:  {_state}: OnInit from {prevState}" );
    }

    // Events from Outside

    // GENERIC CAMERA

    /// <summary>
    /// Received a Camera Setting
    /// </summary>
    /// <param name="cameraMode">current Cam Setting</param>
    public virtual void OnCameraMode( CameraMode cameraMode )
    {
      //  Debug.WriteLine( $"Camera State:  {_state}: OnCameraSetting <{cameraSetting}>" ); // extensive reporting

      // establish state reported from the outside
      _contextRef.ChangeToState( cameraMode );
    }

    // INDEXED CAMERA

    /// <summary>
    /// Received a View Index
    /// </summary>
    /// <param name="index">current ViewIndex 0..N</param>
    public virtual void OnViewIndex( int index )
    {
      //Debug.WriteLine( $"Camera State:  {_state}: OnViewIndex <{index}>" ); // extensive reporting
    }

    /// <summary>
    /// Received a Zoomlevel indication
    /// </summary>
    /// <param name="zoomlevel">Zoomlevel 0..100</param>
    public virtual void OnZoomLevel( int zoomlevel )
    {
    }

    // SMART TARGETS

    /// <summary>
    /// Received Smarcam active indication
    /// </summary>
    /// <param name="index">Target index (-1 if not active)</param>
    public virtual void OnSmartTarget( int index )
    {
    }

    // DRONE

    /// <summary>
    /// Received a Drone Lock indication
    /// </summary>
    /// <param name="lockmode"></param>
    public virtual void OnDroneLock( bool lockmode )
    {
    }

    /// <summary>
    /// Received a Drone Follow indication
    /// </summary>
    /// <param name="followmode"></param>
    public virtual void OnDroneFollow( bool followmode )
    {
    }

    /// <summary>
    /// Received a Drone Move Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    public virtual void OnDroneMoveSpeed( int speed )
    {
    }

    /// <summary>
    /// Received a Drone Rotation Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    public virtual void OnDroneRotSpeed( int speed )
    {
    }

    // Events from inside

    // GENERIC CAMERA

    #region Switch Camera 

    // Exec for the camera change
    private void SwitchCamera_low( CameraMode cameraMode, int viewIndex )
    {
      Debug.WriteLine( $"SwitchCamera_low - Camera State:  {_state}: RequestSwitchCamera({cameraMode}, {viewIndex})" );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (cameraMode != _state) {
        // need to switch

        // first disable smart if it was active
        RequestSmartTarget( -1 );

        // more fiddling to have the Drone reset behind the aircraft
        if (cameraMode == CameraMode.Drone) {
          if (State_Drone.NeedsExternalViewReset( _state )) {
            // to recover the Drone reset pos, Switch to Ext View if not there 
            if (_state != CameraMode.ExternalFree) {
              SV.Set( SItem.cmodGS_Cam_Actual_Mode, CameraMode.ExternalFree );
            }
            // and Reset this Cam
            SV.Set( SItem.S_Cam_Actual_reset, true );
            // another nasty wait until the Sim has settled the reset
            Thread.Sleep( 250 );
          }
        }

        // Generic Cam Mode Switch request
        SV.Set<CameraMode>( SItem.cmodGS_Cam_Actual_Mode, cameraMode );

        // now for the ViewIndex

        // context has not changed yet - so don't use _state here
        // sanity
        if (viewIndex < 0) return;
        if (cameraMode == CameraMode.DOF6) return; // no viewIndex
        if (cameraMode == CameraMode.Drone) return; // no viewIndex
        if (cameraMode == CameraMode.ExternalFree) return; // no viewIndex

        if (cameraMode == CameraMode.CustomCamera) {
          // just send the Key
          _contextRef.CustomCamController?.SendSlot( viewIndex );
        }
        else {
          // Generic Cam ViewIndex Switch request
          SV.Set<int>( SItem.iGS_Cam_Viewindex, viewIndex );
        }
      }

      // same cam as we have already
      else {
        if (viewIndex != ViewIndex) {
          // need to change the ViewIndex

          // sanity
          if (viewIndex < 0) return;
          if (cameraMode == CameraMode.DOF6) return; // no viewIndex
          if (cameraMode == CameraMode.Drone) return; // no viewIndex
          if (cameraMode == CameraMode.ExternalFree) return; // no viewIndex

          if (cameraMode == CameraMode.CustomCamera) {
            // just send the Key
            _contextRef.CustomCamController?.SendSlot( viewIndex );
          }
          else {
            // Generic Cam ViewIndex Switch request
            SV.Set<int>( SItem.iGS_Cam_Viewindex, viewIndex );
          }
        }
      }
    }

    /// <summary>
    /// Switch camera to CameraMode and ViewIndex
    /// </summary>
    public void RequestSwitchCamera( CameraMode cameraMode, int viewIndex )
    {
      // direct call to switch
      SwitchCamera_low( cameraMode, viewIndex );

      // push two more changes to be executed later, as the cam most of the time will not change to the requested mode/index
      _contextRef.JobRunner.AddJob( new JobObj<CameraMode, int>(
           ( CameraMode cMode_, int vIndex_ ) => {
             // it requires the Sim to have some cycles to adjust after the settings changed above
             Thread.Sleep( 250 );
             SwitchCamera_low( cMode_, vIndex_ );
           }, cameraMode, viewIndex, "SwitchCamera 2" )
         );

      _contextRef.JobRunner.AddJob( new JobObj<CameraMode, int>(
           ( CameraMode cMode_, int vIndex_ ) => {
             // it requires the Sim to have some cycles to adjust after the settings changed above
             Thread.Sleep( 250 );
             SwitchCamera_low( cMode_, vIndex_ );
           }, cameraMode, viewIndex, "SwitchCamera 3" )
         );
    }

    /// <summary>
    /// Switch camera to CameraMode and ViewIndex and 6DOF
    /// </summary>
    public void RequestSwitchCamera( CameraMode cameraMode, int viewIndex, Vector3 pos, Vector3 gimbal, bool? lockMode )
    {
      RequestSwitchCamera( cameraMode, viewIndex );
      // delayed call for the 6DOF arguments - will take place after the row of CamSwitches
      _contextRef.JobRunner.AddJob( new JobObj<Vector3, Vector3, bool?>(
           ( Vector3 pos_, Vector3 gimbal_, bool? lockMode_ ) => {
             Thread.Sleep( 50 ); // also here the change may not be applied when called immediately after a change,
                                 // but the cam behaves different hence only 50ms

             if (lockMode_ != null) {
               // assign cam lock
               _contextRef.CamRequestAPI.Request6DofCameraLock( (bool)lockMode_ );
             }
             _contextRef.CamRequestAPI.Request6DofPosition( pos_, gimbal_ );
           }, pos, gimbal, lockMode, "6DOF arguments" )
         );
    }

    /// <summary>
    /// Switch camera to CameraMode and ViewIndex and Zoom
    /// </summary>
    public void RequestSwitchCamera( CameraMode cameraMode, int viewIndex, int zoomLevel )
    {
      RequestSwitchCamera( cameraMode, viewIndex );
      // delayed call for the ZoomLevel - will take place after the row of CamSwitches
      _contextRef.JobRunner.AddJob( new JobObj<int>(
           ( int zoomLevel_ ) => {
             Thread.Sleep( 250 ); // also here the Zoom change is not applied when called immediately after a change
             _contextRef.CamRequestAPI.RequestZoomLevel( zoomLevel_ );
           }, zoomLevel, "Zoom arguments" )
         );
    }

    #endregion

    /// <summary>
    /// Reset camera is requested
    /// </summary>
    public virtual void RequestResetCamera( )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestResetCamera" );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set<bool>( SItem.S_Cam_Actual_reset, true );
    }


    /// <summary>
    /// A Zoomlevel is requested
    /// </summary>
    /// <param name="zoomLevel">Zoomlevel 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public virtual void RequestZoomLevel( int zoomLevel, bool untracked = false )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestZoomLevel <{zoomLevel}, {untracked}>" );
    }

    // INDEXED CAMERA

    /// <summary>
    /// A ViewIndex for the current camera is requested
    /// </summary>
    /// <param name="viewIndex">Requested View Index</param>
    public virtual void RequestViewIndex( int viewIndex )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestViewIndex <{viewIndex}>" );

      // sanity
      if (viewIndex < 0) return;
      if (viewIndex > MaxViewIndex) return;

      if (_state == CameraMode.DOF6) return;
      if (_state == CameraMode.Drone) return;

      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (_state == CameraMode.CustomCamera) {
        _contextRef.CustomCamController?.SendSlot( viewIndex );
      }
      else {
        // Generic Cam ViewIndex Switch request
        SV.Set<int>( SItem.iGS_Cam_Viewindex, viewIndex );
      }
    }

    // SMART TARGETS

    // are handled in the Base Class
    private int _prevSmartTarget = -1;

    /// <summary>
    /// Smart cam request
    /// </summary>
    /// <param name="index">Target index (-1 to disable smartcam)</param>
    public void RequestSmartTarget( int index )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestSmartCam <{index}>" );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if ((index >= 0) && (index < 10)) {
        // valid slot
        if (!SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
          // enable if needed
          SV.Set( SItem.bGS_Cam_Smart_active, true );
        }
        if (index != SmartTargetIndex) {
          // set a new one
          SV.Set( SItem.iGS_Cam_Smart_targetindex_selected, index );
        }
        _prevSmartTarget = index;
      }
      else {
        // Out of range or disable
        _prevSmartTarget = -1;
        if (SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
          // nasty wait until the Sim has settled the disable
          // otherwise the Cam Reset position is messed up when leaving the SmartMode without settling
          SV.Set( SItem.bGS_Cam_Smart_active, false );
          Thread.Sleep( 250 );
        }
      }
    }

    // DRONE

    /// <summary>
    /// Drone Lock Target is requested
    /// </summary>
    /// <param name="locked">Lock Mode</param>
    public virtual void RequestDroneLock( bool locked )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestDroneLock <{locked}>" );
    }

    /// <summary>
    /// Drone Follow Target is requested
    /// </summary>
    /// <param name="follow">Follow mode</param>
    public virtual void RequestDroneFollow( bool follow )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestDroneFollow <{follow}>" );
    }

    /// <summary>
    /// Drone Move Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public virtual void RequestDroneMoveSpeed( int speed, bool untracked = false )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestDroneMoveSpeed <{speed}, {untracked}>" );
    }
    /// <summary>
    /// Drone Rotation Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    public virtual void RequestDroneRotSpeed( int speed, bool untracked = false )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestDroneRotSpeed <{speed}, {untracked}>" );
    }

    /// <summary>
    /// Toggle Control Input while in Drone Cam
    /// </summary>
    public virtual void RequestDroneToggleControls( )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestDroneToggleControls" );
    }

    // 6DOF CAMERA

    /// <summary>
    /// 6DOF Cam orientation is requested
    /// </summary>
    /// <param name="position">Cam Position</param>
    /// <param name="gimbal">Cam Gimbal Orientation</param>
    public virtual void Request6DofPosition( Vector3 position, Vector3 gimbal )
    {
      Debug.WriteLine( $"Camera State:  {_state}: Request6DofPosition <{position}, {gimbal}>" );
    }

    /// <summary>
    /// 6DOF entries will lock on aircraft or not
    /// </summary>
    /// <param name="locked">Camera lock mode</param>
    public virtual void Request6DofCameraLock( bool locked )
    {
      Debug.WriteLine( $"Camera State:  {_state}: RequestCameraLock <{locked}>" );
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~AState()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}
