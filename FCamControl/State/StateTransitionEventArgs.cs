using System;

using FSimClientIF;

namespace FCamControl.State
{
  /// <summary>
  /// Event Args when a State Transition occurs
  /// </summary>
  internal class StateTransitionEventArgs : EventArgs
  {
    /// <summary>
    /// Current State
    /// </summary>
    public CameraMode Mode { get; set; }
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="mode">State to submit</param>
    public StateTransitionEventArgs( CameraMode mode )
    {
      Mode = mode;
    }

  }
}
