using FSimClientIF;

namespace FCamControl.State
{
  /// <summary>
  /// Events all States must support
  /// </summary>
  internal interface ICameraEvents
  {
    /// <summary>
    /// Implement Dispose in order to Cleanup State objects
    /// </summary>
    void Dispose( );

    /// <summary>
    /// Trigger State Init
    /// </summary>
    /// <param name="prevMode">The state comming from</param>
    void OnInit( CameraMode prevMode );

    // Events from Outside

    // GENERIC CAMERA

    /// <summary>
    /// Received a Camera Mode
    /// </summary>
    /// <param name="cameraMode">current Cam Mode</param>
    void OnCameraMode( CameraMode cameraMode );

    /// <summary>
    /// Received a Zoomlevel indication
    /// </summary>
    /// <param name="zoomlevel">Zoomlevel 0..100</param>
    void OnZoomLevel( int zoomlevel );

    // INDEXED CAMERA

    /// <summary>
    /// Received a View Index
    /// </summary>
    /// <param name="index">current ViewIndex</param>
    void OnViewIndex( int index );

    // SMART TARGETS

    /// <summary>
    /// Received Smarcam active indication
    /// </summary>
    /// <param name="index">Target index (-1 if not active)</param>
    void OnSmartTarget( int index );

    // DRONE

    /// <summary>
    /// Received a Drone Lock indication
    /// </summary>
    /// <param name="lockmode"></param>
    void OnDroneLock( bool lockmode );

    /// <summary>
    /// Received a Drone Follow indication
    /// </summary>
    /// <param name="followmode"></param>
    void OnDroneFollow( bool followmode );

    /// <summary>
    /// Received a Drone Move Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    void OnDroneMoveSpeed( int speed );

    /// <summary>
    /// Received a Drone Rotation Speed indication
    /// </summary>
    /// <param name="speed">Speed 0..100</param>
    void OnDroneRotSpeed( int speed );

  }
}
