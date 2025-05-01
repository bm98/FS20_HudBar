using FSimClientIF;

using static FSimClientIF.Sim;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// Showcase - FixedView - Index 1..N
  /// No Zoom
  /// </summary>
  internal class State_ExternalIndexed : AState
  {

    /// <summary>
    /// cTor:
    /// </summary>
    public State_ExternalIndexed( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );

      _firstInitDone = true;
    }


    // support ViewIndex

    // Switch 0..29
    public override void RequestViewIndex( int index )
    {
      base.RequestViewIndex( index );

      // sanity
      if ((index < 0) || (index > MaxViewIndex)) return;

      SV.Set<int>( SItem.iGS_Cam_Viewindex, index );
    }


  }
}
