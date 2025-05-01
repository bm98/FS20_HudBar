using System.Numerics;

using FSimClientIF;

using SC = SimConnectClient;
using static FSimClientIF.Sim;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// 6DOF Camera
  /// </summary>
  internal class State_DOF6 : AState
  {

    private static readonly Vector3 c_cam6dDefaultPos = new Vector3( ) { X = 20f, Y = 20f, Z = 20f };

    // Sim does not provide them, store them here
    private Vector3 _position = new Vector3( );
    private Vector3 _gimbal = new Vector3( );
    private bool _camLocked = true;

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    public override int ViewIndex => -1;

    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => -1;

    /// <summary>
    /// Returns the current 6DOF Position
    /// </summary>
    public override Vector3 Cam6DofPosition => _position;

    /// <summary>
    /// Returns the current 6DOF Gimbal
    /// </summary>
    public override Vector3 Cam6DofGimbal => _gimbal;

    /// <summary>
    /// Returns the current 6DOF Camera LockMode
    /// </summary>
    public override bool Cam6DofLocked => _camLocked;

    /// <summary>
    /// cTor:
    /// </summary>
    public State_DOF6( CameraMode mode, Context context )
      : base( mode, context )
    {
    }


    // Set Camera
    public override void Request6DofPosition( Vector3 position, Vector3 gimbal )
    {
      base.Request6DofPosition( position, gimbal );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // persist, cannot be read from the Sim
      _position = position;
      _gimbal = gimbal;
      SV.Set( SItem.v3GS_Cam_6DOF_xyz, position );
      SV.Set( SItem.v3GS_Cam_6DOF_pbh, gimbal );
      SV.Set( SItem.bS_Cam_6DOF_set, true );
    }

    /// <summary>
    /// 6DOF entries will lock on aircraft or not
    /// </summary>
    /// <param name="locked">Camera lock mode</param>
    public override void Request6DofCameraLock( bool locked )
    {
      base.Request6DofCameraLock( locked );

      _camLocked= locked; // updated during GUI update of the panel
    }


  }
}
