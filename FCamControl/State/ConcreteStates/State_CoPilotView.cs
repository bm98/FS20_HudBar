using FSimClientIF;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// Cockpit - CoPilotView
  ///  Fixed View
  /// </summary>
  internal class State_CoPilotView : AState_CockpitView
  {


    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => -1;

    /// <summary>
    /// cTor:
    /// </summary>
    public State_CoPilotView( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    // Establish current
    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );


      _firstInitDone = true;
    }

  }
}
