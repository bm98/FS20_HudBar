using FSimClientIF;

using System.Numerics;

namespace FCamControl.State
{
  /// <summary>
  /// Events all States must support
  /// </summary>
  internal interface ICameraRequests
  {

    // Events/Requests from inside

    // GENERIC CAMERA

    /// <summary>
    /// Reset camera is requested
    /// </summary>
    void RequestResetCamera( );

    /// <summary>
    /// Camera Mode and View index is requested
    /// </summary>
    /// <param name="cameraMode">current Cam Mode</param>
    /// <param name="viewIndex">ViewIndex 0..N</param>
    void RequestSwitchCamera( CameraMode cameraMode, int viewIndex );

    /// <summary>
    /// Camera Mode and View index and 6DOF is requested
    /// </summary>
    /// <param name="cameraMode">current Cam Mode</param>
    /// <param name="viewIndex">ViewIndex 0..N</param>
    /// <param name="pos">6DOF Position</param>
    /// <param name="gimbal">6DOF Gimbal</param>
    /// <param name="lockMode">Cam Lock to Acft mode, null to leave it as it is</param>
    void RequestSwitchCamera( CameraMode cameraMode, int viewIndex, Vector3 pos, Vector3 gimbal, bool? lockMode );

    /// <summary>
    /// Camera Mode and View index and ZoomLevel is requested
    /// </summary>
    /// <param name="cameraMode">current Cam Mode</param>
    /// <param name="viewIndex">ViewIndex 0..N</param>
    /// <param name="zoomLevel">ZoomLevel</param>
    void RequestSwitchCamera( CameraMode cameraMode, int viewIndex, int zoomLevel );

    /// <summary>
    /// A Zoomlevel is requested
    /// </summary>
    /// <param name="zoomLevel">Zoomlevel 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    void RequestZoomLevel( int zoomLevel, bool untracked = false );

    // INDEXED CAMERA

    /// <summary>
    /// A ViewIndex is requested
    /// </summary>
    /// <param name="viewIndex">ViewIndex 0..N</param>
    void RequestViewIndex( int viewIndex );

    // SMART TARGETS

    /// <summary>
    /// Smart Target request
    /// </summary>
    /// <param name="index">Target index (-1 to disable smartcam)</param>
    void RequestSmartTarget( int index );

    // DRONE

    /// <summary>
    /// Drone Lock Target is requested
    /// </summary>
    /// <param name="locked">Lock Mode</param>
    void RequestDroneLock( bool locked );

    /// <summary>
    /// Drone Follow Target is requested
    /// </summary>
    /// <param name="follow">Follow mode</param>
    void RequestDroneFollow( bool follow );

    /// <summary>
    /// Drone Move Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    void RequestDroneMoveSpeed( int speed, bool untracked = false );
    /// <summary>
    /// Drone Rotation Speed is requested
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    /// <param name="untracked">True for untracked change (default=false)</param>
    void RequestDroneRotSpeed( int speed, bool untracked = false );

    /// <summary>
    /// Toggle Control Input while in Drone Cam
    /// </summary>
    void RequestDroneToggleControls( );

    // 6DOF CAMERA

    /// <summary>
    /// 6DOF Cam orientation is requested
    /// </summary>
    /// <param name="position">Cam Position</param>
    /// <param name="gimbal">Cam Gimbal Orientation</param>
    void Request6DofPosition( Vector3 position, Vector3 gimbal );

    /// <summary>
    /// 6DOF entries will lock on aircraft or not
    /// </summary>
    /// <param name="locked">Camera lock mode</param>
    void Request6DofCameraLock( bool locked );

  }
}
