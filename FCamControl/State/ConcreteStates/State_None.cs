using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimClientIF;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// No camera selected - init app
  /// </summary>
  internal class State_None : AState
  {

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
    public State_None( CameraMode mode, Context context )
      : base( mode, context )
    {
    }



  }
}
