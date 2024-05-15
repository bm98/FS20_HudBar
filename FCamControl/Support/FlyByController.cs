﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

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
  internal class FlyByController
  {
    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    private const float c_minDist_nm = 0.1f;
    private const float c_maxDist_nm = 2f;

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

    // milliseconds key press per nm at DroneSpeed = 10
    private const float c_msPerNm = 35_000;


    // stage 1: Runway 1nm
    // stage 2: 100 kt GS - 1.7 nm / Min; 0.02 nm / sec 
    // stage 3: 250 kt GS - 4.2 nm / Min; 0.07 nm / sec
    // stage 4: 400 kt GS - 6.7 nm / Min; 0.1 nm / sec
    private static readonly float[] c_nmOut = new float[4] { 1f, 0.35f, 0.875f, 1.4f }; // with calibrated drone speed

    // 0..max
    private WinKbdSender _kbd;
    private CountdownTimer _kbdTimer;
    private bool _preparing = false;

    private JobRunner _jobRunner;

    private MSFS_KeyCat _msfsKeyCatalog;

    /// <summary>
    /// Event fired every 100ms and when FlyBy is ready to fire
    /// </summary>
    public event EventHandler<FlyByControllerEventArgs> FlyByReadyProgress;
    private void OnFlyByReady( int remaining_ms ) => FlyByReadyProgress?.Invoke( this, new FlyByControllerEventArgs( remaining_ms <= 0, remaining_ms ) );

    /// <summary>
    /// cTor:
    /// </summary>
    public FlyByController( MSFS_KeyCat msfsKeyCatalog )
    {
      _msfsKeyCatalog = new MSFS_KeyCat( msfsKeyCatalog ); // maintain a copy of the catalog

      _jobRunner = new JobRunner( );

      _kbd = new WinKbdSender( );

      _kbdTimer = new CountdownTimer( );
      _kbdTimer.Progress += _kbdTimer_Progress;

    }

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
      var vs_mPsec = Units.Mps_From_Ftpm( SV.Get<float>( SItem.fG_Acft_VS_ftPmin ) );
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
    /// <param name="leftSide">True to look from left, else from right side</param>
    /// <param name="above">True to look from above (has prio over below)</param>
    /// <param name="below">True to look from below</param>
    public bool Prepare( int stage, bool leftSide, bool above, bool below )
    {
      return Prepare( DistOfStage_nm( stage ), leftSide, above, below );
    }

    /// <summary>
    /// Prepare for FlyBy Cam
    ///   CAM MUST BE IN DRONE CAM MODE ALREADY - no check 
    /// </summary>
    /// <param name="dist_nm">Cam distance</param>
    /// <param name="leftSide">True to look from left, else from right side</param>
    /// <param name="above">True to look from above (has prio over below)</param>
    /// <param name="below">True to look from below</param>
    public bool Prepare( float dist_nm, bool leftSide, bool above, bool below )
    {
      // sanity
      if (_kbd.IsBusy) return false;

      if (dist_nm < 0.1f) dist_nm = 0.1f;
      if (dist_nm > 2f) dist_nm = 2f;

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
      var goUp = vTime > 50;
      var goDn = vTime < -50;
      vTime = Math.Abs( vTime ); // time is positive for the delay

      var delay = 0;

      _jobRunner.AddJob( new JobObj( ( ) => {
        ResetDroneCam( ); // should be in Follow mode just behind the aircraft, Lock mode is OFF, DroneSpeed is 30
        Thread.Sleep( 2000 ); // Wait until settled
      }, "Reset Cam" ) );
      delay += 2000;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        SetDroneLockMode( true );
        Thread.Sleep( 250 ); // Wait until settled
        SetDroneSpeed( lonSpeed );
        Thread.Sleep( 250 ); // Wait until settled
      }, "Set Drone Lock and Longitudinal Speed" ) );
      delay += 2 * 250;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        var kbd = new WinKbdSender( );
        kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveForward].AsStroke( (int)wTime ) );
        kbd.RunStrokes( c_SimWindowTitle, blocking: true );
        kbd.Dispose( );
        /*
        WinUser.SendKey( Keys.W, true, c_SimWindowTitle );
        Thread.Sleep( (int)wTime);
        WinUser.SendKey( Keys.W, false, c_SimWindowTitle );
        */
      }, "Move Drone Forward" ) );
      delay += (int)wTime;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        SetDroneSpeed( c_droneSpeed_LatVert );
        Thread.Sleep( 200 ); // Wait until settled
      }, "Set Drone Speed Lateral and Vertical" ) );
      delay += 200;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        if (leftSide) {
          var kbd = new WinKbdSender( );
          kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveRight].AsStroke( (int)lTime ) );
          kbd.RunStrokes( c_SimWindowTitle, blocking: true );
          kbd.Dispose( );
          /*
          Thread.Sleep( 50 );
          WinUser.SendKey( Keys.D, true, c_SimWindowTitle );
          Thread.Sleep( (int)lTime );
          WinUser.SendKey( Keys.D, false, c_SimWindowTitle );
          */
        }
        else {
          var kbd = new WinKbdSender( );
          kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveLeft].AsStroke( (int)lTime ) );
          kbd.RunStrokes( c_SimWindowTitle, blocking: true );
          kbd.Dispose( );
          /*
          Thread.Sleep( 50 );
          WinUser.SendKey( Keys.A, true, c_SimWindowTitle );
          Thread.Sleep( (int)lTime );
          WinUser.SendKey( Keys.A, false, c_SimWindowTitle );
          */
        }
      }, "Move Drone lateral" ) );
      delay += lTime;
      Thread.Yield( );

      _jobRunner.AddJob( new JobObj( ( ) => {
        if (vTime > 40) {
          if (goUp) {
            var kbd = new WinKbdSender( );
            kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveUp].AsStroke( (int)vTime ) );
            kbd.RunStrokes( c_SimWindowTitle, blocking: true );
            kbd.Dispose( );
            /*
            Thread.Sleep( 50 );
            WinUser.SendKey( Keys.R, true, c_SimWindowTitle );
            Thread.Sleep( (int)vTime );
            WinUser.SendKey( Keys.R, false, c_SimWindowTitle );
            */
          }
          else if (goDn) {
            var kbd = new WinKbdSender( );
            kbd.AddStroke( _msfsKeyCatalog[FS_Key.DrMoveDown].AsStroke( (int)vTime ) );
            kbd.RunStrokes( c_SimWindowTitle, blocking: true );
            kbd.Dispose( );
            /*
            Thread.Sleep( 50 );
            WinUser.SendKey( Keys.F, true, c_SimWindowTitle );
            Thread.Sleep( (int)vTime );
            WinUser.SendKey( Keys.F, false, c_SimWindowTitle );
            */
          }
        }
      }, "Move Drone vertical" ) );
      delay += (int)vTime;
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
      SetDroneFollowMode( false );
    }


    private void SetDroneSpeed( int dSpeed )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (dSpeed < 10) dSpeed = 10;
      if (dSpeed > 100) dSpeed = 100;

      SV.Set( SItem.fGS_Cam_Drone_movespeed, (float)dSpeed );
    }

    private void SetDroneLockMode( bool mode )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.bGS_Cam_Drone_locked, mode );
    }

    private void SetDroneFollowMode( bool mode )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.bGS_Cam_Drone_follow, mode );
    }

    private void ResetDroneCam( )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.S_Cam_Actual_reset, true );
    }


  }
}
