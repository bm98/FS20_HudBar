using FSimClientIF;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// External - Free View - Index 1
  /// </summary>
  internal class State_ExternalFree : AState_ExternalView
  {
    /// <summary>
    /// True when SmartTarget is available
    /// </summary>
    public override bool CanSmartTarget => true;

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    public override int ViewIndex => -1;

    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => -1;

    /// <summary>
    /// cTor:
    /// </summary>
    public State_ExternalFree( CameraMode mode, Context context )
      : base( mode, context )
    {
    }



  }
}
