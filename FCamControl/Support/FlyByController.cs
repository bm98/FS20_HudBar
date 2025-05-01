using System;
using System.Threading;

using dNetBm98;
using dNetBm98.Win;
using dNetBm98.Job;

using FCamControl;

using FSimClientIF.Modules;

using static FSimClientIF.Sim;

using SC = SimConnectClient;

namespace FS20_CamControl
{

  /// <summary>
  /// Manages the FlyBy Cam
  /// </summary>
  internal sealed class FlyByController
  {
    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // support to calculate the FlyBy key press directions and times
    // NOTE for now the calibration is off by 2 (i.e. 1nm here is ~0.5 in the Sim)
    //  TODO recalibrate and update the numbers here
    private const float c_minDist_nm = 0.1f;
    private const float c_maxDist_nm = 1f;

    // Window to send Keystrokes
    private const string c_SimWindowTitle = "Microsoft Flight Simulator";
    private const int c_droneSpeedCalibrated = 12; // 0..100 speed value of the calibration = 30 in MSFS GUI

    private const int c_droneSpeed_Lon = 100;
    private const int c_droneSpeed_LatVert = 30;

    // vertical move for the used dronespeed
    private const int c_vertTime_ms = 300;
    // lateral move left or right
    private const int c_lateralTime_ms = 400;

    // Keypress scaling
    //  Base values are defined for DroneSpeed = 12 (0..100)
    // keyDuration_ms = dist_nm * c_msPerNm / (DroneSpeed/12)

    // milliseconds key press per nm at DroneSpeed = 12 (Sim 30)
    private const float c_msPerNm = 60_000;


    // stage 1: Runway 0.5nm
    // stage 2: 100 kt GS - 1.7 nm / Min; 0.02 nm / sec 
    // stage 3: 250 kt GS - 4.2 nm / Min; 0.07 nm / sec
    // stage 4: 400 kt GS - 6.7 nm / Min; 0.1 nm / sec
    private static readonly float[] c_nmOut = new float[4] { 0.5f, 0.15f, 0.4f, 0.7f }; // with calibrated drone speed

    // key mapping, will be loaded from Settings
    private MSFS_KeyCat _msfsKeyCatalog;
    private Camera _camera = null;

    // Runs the FlyBy Prep
    private JobRunner _jobRunner;
    private bool _preparing = false;
    // handles the FireButton visibility
    private CountdownTimer _kbdTimer;


    /// <summary>
    /// Event fired every 100ms and when FlyBy is ready to fire
    /// </summary>
    public event EventHandler<FlyByControllerEventArgs> FlyByReadyProgress;
    private void OnFlyByReady( int remaining_ms ) => FlyByReadyProgress?.Invoke( this, new FlyByControllerEventArgs( remaining_ms <= 0, remaining_ms ) );

    /// <summary>
    /// cTor: V2
    /// </summary>
    public FlyByController( MSFS_KeyCat msfsKeyCatalog, Camera camera )
    {
      _msfsKeyCatalog = new MSFS_KeyCat( msfsKeyCatalog ); // maintain a copy of the catalog
      _camera = camera;

      _jobRunner = new JobRunner( );

      _kbdTimer = new CountdownTimer( );
      _kbdTimer.Progress += _kbdTimer_Progress;

    }

    /// <summary>
    /// Reload the KeyCatalog after changes
    /// </summary>
    /// <param name="msfsKeyCatalog">The Catalog to load</param>
    public void ReloadKeyCatalog( MSFS_KeyCat msfsKeyCatalog )
    {
      _msfsKeyCatalog = new MSFS_KeyCat( msfsKeyCatalog ); // maintain a copy of the catalog
    }


    // handle the timer events
    // will report per ms and when finished
    private void _kbdTimer_Progress( object sender, EventArgs e )
    {
      if (_kbdTimer.Remaining_ms <= 0) {
        _preparing = false;
      }
      OnFlyByReady( _kbdTimer.Remaining_ms );
    }


    /// <summary>
    /// Scales from 0..100 % to min, max distance
    /// </summary>
    public static float DistFromPercent( int percent ) => (c_maxDist_nm - c_minDist_nm) / 100f * percent + c_minDist_nm;
    /// <summary>
    /// Scales from distance to  0..100 %
    /// </summary>
    public static int DistToPercent( float dist ) => XMath.Clip( (int)((dist - c_minDist_nm) * 100 / (c_maxDist_nm - c_minDist_nm)), 0, 100 );


    /// <summary>
    /// Distance value of a stage
    /// </summary>
    /// <param name="stage">Stage 0..Max</param>
    /// <returns>Distance nm</returns>
    public static float DistOfStage_nm( int stage )
    {
      // sanity
      if (stage >= c_nmOut.Length) stage = 0;
      return c_nmOut[stage];
    }

    // calc the vertical deviation for the given distance
    // using the current VS and GS
    private double VertDistance( double distance_nm )
    {
      var vs_mPsec = Units.Mps_From_Ftpm( SV.Get<float>( SItem.fG_Acft_VS_Avg_ftPmin ) );
      var gs_mPsec = Units.Mps_From_Kt( SV.Get<float>( SItem.fG_Acft_GS_kt ) );
      if (gs_mPsec < 0.01) return 0;

      var dLon_m = Units.M_From_Nm( distance_nm );
      var t_sec = dLon_m / gs_mPsec;
      var dVert_m = t_sec * vs_mPsec;
      return Units.Nm_From_M( dVert_m );
    }

    /// <summary>
    /// Prepare for FlyBy Cam
    ///   CAM MUST BE IN DRONE CAM MODE ALREADY - no check 
    /// </summary>
    /// <param name="stage">Fly Stage 0..Max</param>
    /// <param name="rightSide">True to look from right side</param>
    /// <param name="leftSide">True to look from left, else from right side</param>
    /// <param name="above">True to look from above (has prio over below)</param>
    /// <param name="below">True to look from below</param>
    public bool Prepare( int stage, bool rightSide, bool leftSide, bool above, bool below )
    {
      return Prepare( DistOfStage_nm( stage ), rightSide, leftSide, above, below );
    }

    /// <summary>
    /// Prepare for FlyBy Cam
    ///   CAM MUST BE IN DRONE CAM MODE ALREADY - no check 
    /// </summary>
    /// <param name="dist_nm">Cam distance</param>
    /// <param name="rightSide">True to look from right side</param>
    /// <param name="leftSide">True to look from left side</param>
    /// <param name="above">True to look from above (has prio over below)</param>
    /// <param name="below">True to look from below</param>
    public bool Prepare( float dist_nm, bool rightSide, bool leftSide, bool above, bool below )
    {
      // sanity
      if (_camera == null) return false;
      if (_preparing) return false;

      if (dist_nm < 0.01f) dist_nm = 0.01f;
      if (dist_nm > 1f) dist_nm = 1f;

      _preparing = true;

      // Reset DroneCam
      // Set Drone Speed
      // Issue KeyStrokes
      // Setup: Press W for n seconds
      // Setup: Press R (above) or F (below) for m seconds if needed
      // start Fire Timer with total time+

      var lonSpeed = c_droneSpeed_Lon;
      var wTime = (float)dist_nm * c_msPerNm / ((float)lonSpeed / (float)c_droneSpeedCalibrated);
      if (wTime < 800) {
        // too short for reliable keypress, use a slower move speed
        lonSpeed = c_droneSpeed_LatVert;
        wTime = (float)dist_nm * c_msPerNm / ((float)lonSpeed / (float)c_droneSpeedCalibrated);
      }
      var lTime = c_lateralTime_ms;

      // compensate for current vSpeed
      var vTime = VertDistance( dist_nm ) * c_msPerNm / ((float)c_droneSpeed_LatVert / (float)c_droneSpeedCalibrated);
      vTime += above ? c_vertTime_ms : below ? -c_vertTime_ms : 0;
      // move direction, ignore when <50ms key pulse
      if ((vTime < 0) && SV.Get<bool>( SItem.bG_Sim_OnGround )) {
        vTime = 0; // only above
      }
      var goUp = vTime > 50;
      var goDn = vTime < -50;
      vTime = Math.Abs( vTime ); // time is positive for the delay

      // try to preserve values after Jobs
      int dMoveSpeed = _camera.CameraAPI.CamValueAPI.DroneMoveSpeed;
      int dRotSpeed = _camera.CameraAPI.CamValueAPI.DroneRotSpeed;
      int dZoom =  _camera.CameraAPI.CamValueAPI.ZoomLevel;

      // jobs need to add dynamic arguments with the job,
      // static ones can be sourced from the module as the job execution is very local in time
      // no changes of the static args are expected during running the job queue

      var delay = 0; // summarize the runtime on the fly

      _jobRunner.AddJob( new JobObj( ( ) => {
        ResetDroneCam( ); // should be in Follow mode just behind the aircraft, Lock mode is OFF, DroneSpeed is 30
        Thread.Sleep( 2000 ); // Wait until settled
      }, "Reset Cam" ) );
      delay += 2000;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj<int>( ( int speedArg ) => {
        SetDroneLockMode( true );
        Thread.Sleep( 250 ); // Wait until settled
        SetDroneMoveSpeed( speedArg );
        Thread.Sleep( 250 ); // Wait until settled
      }, lonSpeed, "Set Drone Lock and Longitudinal Speed" ) );
      delay += 2 * 250;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj<int>( ( int timeArg ) => {
        var kbd = new WinKbdSender( );
        kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveForward].AsStroke( timeArg ) ); // assume key const while running the job
        kbd.RunStrokes( c_SimWindowTitle, blocking: true ); // const
        kbd.Dispose( );
      }, (int)wTime, "Move Drone Forward" ) );
      delay += (int)wTime;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        SetDroneMoveSpeed( c_droneSpeed_LatVert ); // const
        Thread.Sleep( 200 ); // Wait until settled
      }, "Set Drone Speed Lateral and Vertical" ) );
      delay += 200;
      Thread.Yield( );

      // as we look towards the aircraft now the Left and Right side are inversed 
      // hence moving left to view it from the right side and vica versa
      _jobRunner.AddJob( new JobObj<bool, bool, int>( ( bool leftArg, bool rightArg, int timeArg ) => {
        if (leftArg) {
          // takes prio over right
          var kbd = new WinKbdSender( );
          kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveRight].AsStroke( timeArg ) ); // assume key const while running the job
          kbd.RunStrokes( c_SimWindowTitle, blocking: true ); // const
          kbd.Dispose( );
        }
        else if (rightArg) {
          var kbd = new WinKbdSender( );
          kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveLeft].AsStroke( timeArg ) ); // assume key const while running the job
          kbd.RunStrokes( c_SimWindowTitle, blocking: true ); // const
          kbd.Dispose( );
        }
      }, leftSide, rightSide, lTime, "Move Drone lateral" ) );
      delay += (leftSide || rightSide) ? lTime : 0;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj<bool, bool, int>( ( upArg, dnArg, timeArg ) => {
        if (timeArg > 40) {
          if (upArg) {
            // takes prio over down
            var kbd = new WinKbdSender( );
            kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveUp].AsStroke( timeArg ) ); // assume key const while running the job
            kbd.RunStrokes( c_SimWindowTitle, blocking: true ); // const
            kbd.Dispose( );
          }
          else if (dnArg) {
            var kbd = new WinKbdSender( );
            kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveDown].AsStroke( timeArg ) ); // assume key const while running the job
            kbd.RunStrokes( c_SimWindowTitle, blocking: true ); // const
            kbd.Dispose( );
          }
        }
      }, goUp, goDn, (int)vTime, "Move Drone vertical" ) );
      delay += (goUp || goDn) ? (int)vTime : 0;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj<int, int, int>( ( int moveArg, int rotArg, int zoomArg ) => {

        SetDroneMoveSpeed( moveArg );
        SetDroneRotSpeed( rotArg );
        SetDroneZoom( zoomArg );
      }, dMoveSpeed, dRotSpeed, dZoom, "Recover Drone Speeds" ) );
      Thread.Yield( );

      _kbdTimer.Duration_ms = delay + 200; // 200 ms overhead
      _kbdTimer.Start( );

      return true;
    }

    /// <summary>
    /// Triggers the FlyBy by releasing the Follow mode
    /// </summary>
    public void Fire( )
    {
      // sanity
      if (_preparing) return;

      // release Follow Mode
      _camera?.CameraAPI.CamRequestAPI.RequestDroneFollow( false );
    }


    private void SetDroneMoveSpeed( int dSpeed )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestDroneMoveSpeed( dSpeed, untracked: true );
    }
    private void SetDroneRotSpeed( int dSpeed )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestDroneRotSpeed( dSpeed, untracked: true );
    }
    private void SetDroneZoom( int dZoom )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestZoomLevel( dZoom, untracked: true );
    }

    private void SetDroneLockMode( bool mode )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestDroneLock( mode );
    }

    private void SetDroneFollowMode( bool mode )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestDroneFollow( mode );
    }

    private void ResetDroneCam( )
    {
      _camera?.CameraAPI.CamRequestAPI.RequestResetCamera( );
    }


  }
}
