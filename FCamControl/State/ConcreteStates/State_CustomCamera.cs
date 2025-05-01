using FSimClientIF;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// Custom Camera Views 0..9 seems to be Pilot-Index=7
  /// </summary>
  internal class State_CustomCamera : AState_CockpitView
  {
    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => 10;

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    public override int ViewIndex => _contextRef.CustomCamController.LastSlotIndex;


    /// <summary>
    /// cTor:
    /// </summary>
    public State_CustomCamera( CameraMode mode, Context context )
      : base( mode, context )
    {
    }


    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );

      _firstInitDone = true;
    }


    // support ViewIndex

    // Switch 0..9
    public override void RequestViewIndex( int index )
    {
      base.RequestViewIndex( index );

      // sanity
      if ((index < 0) || (index > MaxViewIndex)) return;

      _contextRef.CustomCamController.SendSlot( index );
    }


  }
}
