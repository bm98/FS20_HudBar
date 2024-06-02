using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF;
using FSimClientIF.Modules;
using dNetBm98.Job;

namespace FCamControl.State
{
  /// <summary>
  /// State Management
  /// Context Class
  /// 
  ///  To establish a connection instantiate a new object of this Class
  ///  Dispose it to disconnect and clear
  /// </summary>
  internal class Context : IDisposable
  {
    // attach the property module - this does not depend on the connection established or not
    protected readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    private MSFS_KeyCat _msfs_KeyCat = null;
    private int _msfs_KeyCatVersion = 0;

    // Custom Cams
    private CustomCamController _customCamController = null;

    // Sequential Job Executor  (1 thread only)
    private JobRunner _jobRunner = new JobRunner( 1 );

    // current State
    private AState _state = null;
    // may be needed at some point...
#pragma warning disable CS0414 // Remove unread private members
    private bool _updating = false;
#pragma warning restore CS0414 // Remove unread private members

    /// <summary>
    /// Event: Signals a new State i.e. Camera Setting changed
    /// </summary>
    public event EventHandler<StateTransitionEventArgs> StateChanged;
    private void OnStateChanged( CameraMode state ) => StateChanged?.Invoke( this, new StateTransitionEventArgs( state ) );

    /// <summary>
    /// Event: Signals data arrived from the Sim
    /// </summary>
    public event EventHandler<EventArgs> DataArrived;
    private void OnDataArrived( ) => DataArrived?.Invoke( this, EventArgs.Empty );

    // Persist Cam States 
    private Dictionary<CameraMode, AState> _camStateCat = new Dictionary<CameraMode, AState>( );

    /// <summary>
    /// The current Key Catalog
    /// </summary>
    public MSFS_KeyCat MSFS_KeyCatalog => _msfs_KeyCat;
    /// <summary>
    /// The version of the Key Catalog
    /// </summary>
    public int MSFS_KeyCatalogVersion => _msfs_KeyCatVersion;

    /// <summary>
    /// The CustomCamera Controller
    /// </summary>
    public CustomCamController CustomCamController => _customCamController;

    /// <summary>
    /// A JobRunner 
    /// </summary>
    public JobRunner JobRunner => _jobRunner;

    /// <summary>
    /// The Current Camera Mode
    /// </summary>
    public CameraMode CurrentCamMode => _state.State;

    /// <summary>
    /// True if we are in any Cockpit View
    /// </summary>
    public bool IsCockpitView {
      get {
        switch (CurrentCamMode) {
          case CameraMode.PilotView:
          case CameraMode.CloseView:
          case CameraMode.LandingView:
          case CameraMode.CoPilotView:
          case CameraMode.CustomCamera:
          case CameraMode.InstrumentQuick:
          case CameraMode.InstrumentIndexed:
            return true;

          case CameraMode.ExternalQuick:
          case CameraMode.ExternalIndexed:
          case CameraMode.ExternalFree:
          case CameraMode.Drone:
          case CameraMode.DOF6:
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// True if we are in any External View
    /// </summary>
    public bool IsExternalView {
      get {
        switch (CurrentCamMode) {
          case CameraMode.PilotView:
          case CameraMode.CloseView:
          case CameraMode.LandingView:
          case CameraMode.CoPilotView:
          case CameraMode.CustomCamera:
          case CameraMode.InstrumentQuick:
          case CameraMode.InstrumentIndexed:
            return false;

          case CameraMode.ExternalQuick:
          case CameraMode.ExternalIndexed:
          case CameraMode.ExternalFree:
          case CameraMode.Drone:
          case CameraMode.DOF6:
            return true;
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// The Camera API to get Values
    /// </summary>
    public ICameraValues CamValueAPI => _state;
    /// <summary>
    /// The Camera API to make Requests
    /// </summary>
    public ICameraRequests CamRequestAPI => _state;
    /// <summary>
    /// The Camera API to send Events
    /// </summary>
    public ICameraEvents CamEventAPI => _state;

    /// <summary>
    /// SimStopped Flag 
    /// (not a state but a flag)
    /// </summary>
    public bool SimStopped { get; set; } = false;

    /// <summary>
    /// cTor: init the State Handler
    /// </summary>
    /// <param name="mode">State to start with</param>
    /// <param name="msfs_KeyCat">Current KeyCatalog</param>
    public Context( CameraMode mode, MSFS_KeyCat msfs_KeyCat )
    {
      // 1st copy the key catalog as States will need it on init
      _msfs_KeyCat = new MSFS_KeyCat( msfs_KeyCat ); // use a copy
      _msfs_KeyCatVersion++;
      _customCamController = new CustomCamController( _msfs_KeyCat );

      // Create all states
      foreach (CameraMode cs in Enum.GetValues( typeof( CameraMode ) )) {
        _camStateCat.Add( cs, StateFactory.CreateState( cs, this ) );
      }

      // attach events

      // initial state
      _state = _camStateCat[mode];
      _state.OnInit( CameraMode.NONE );

      Debug.WriteLine( $"Context: Init with <{_state.State}>" );

      // init State Handling
      OnStateChanged( _state.State );
    }


    /// <summary>
    /// Transition to new State
    /// </summary>
    /// <param name="mode">New State</param>
    public void ChangeToState( CameraMode mode )
    {
      if (mode == _state.State) return; // already there

      AState oldState = _state;
      _state = _camStateCat[mode];
      Debug.WriteLine( $"Context: StateChanged <{oldState.State}> -> <{_state.State}>" );

      _state.OnInit( oldState.State );

      // signal to observers
      OnStateChanged( _state.State );
    }

    /// <summary>
    /// Update the Key Catalog
    /// </summary>
    /// <param name="msfs_KeyCat"></param>
    public void OnMSFS_KeyCatalog( MSFS_KeyCat msfs_KeyCat )
    {
      _msfs_KeyCat = new MSFS_KeyCat( msfs_KeyCat ); // use a copy
      _msfs_KeyCatVersion++;
      _customCamController.ReloadKeyCatalog( _msfs_KeyCat );
      // trigger immediate loading if needed
      _state.OnInit( _state.State );
    }

    /// <summary>
    /// Handles the data Arrival
    /// </summary>
    public void DataArrival( )
    {
      _updating = true;

      // prio, this may change the state if the cam has changed
      _state.OnCameraMode( SV.Get<CameraMode>( Sim.SItem.cmodGS_Cam_Actual_Mode ) );

      // others should not change the cam anymore

      _state.OnViewIndex( SV.Get<int>( Sim.SItem.iGS_Cam_Viewindex ) );

      // smart targeting
      if (SV.Get<bool>( Sim.SItem.bGS_Cam_Smart_active )) {
        _state.OnSmartTarget( SV.Get<int>( Sim.SItem.iGS_Cam_Smart_targetindex_selected ) );
      }
      else {
        _state.OnSmartTarget( -1 ); // smart target not active
      }

      // Update Drone Values
      if (_state.State == CameraMode.Drone) {
        _state.OnDroneLock( SV.Get<bool>( Sim.SItem.bGS_Cam_Drone_locked ) );
        _state.OnDroneFollow( SV.Get<bool>( Sim.SItem.bGS_Cam_Drone_follow ) );
        _state.OnZoomLevel( (int)SV.Get<float>( Sim.SItem.fGS_Cam_Drone_zoomlevel ) );
        _state.OnDroneMoveSpeed( (int)SV.Get<float>( Sim.SItem.fGS_Cam_Drone_movespeed ) );
        _state.OnDroneRotSpeed( (int)SV.Get<float>( Sim.SItem.fGS_Cam_Drone_rotspeed ) );
      }
      else {
        // Ev. OnZoom for zoomable cams TODO implement as SItem
      }


      _updating = false;

      // signal updates to customers
      OnDataArrived( );
    }


    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)

          // Detach events


          // Dispose all states
          foreach (CameraMode cs in Enum.GetValues( typeof( CameraMode ) )) {
            _camStateCat[cs]?.Dispose( );
          }
          // cleanup - not really required...
          _camStateCat.Clear( );
          _camStateCat = null;
          _state = null;
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Context()
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
