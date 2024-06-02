using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FSimClientIF;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// Cockpit - CloseView
  ///  Fixed View
  /// </summary>
  internal class State_CloseView : AState_CockpitView
  {
    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    public override int MaxViewIndex => -1;

    /// <summary>
    /// cTor:
    /// </summary>
    public State_CloseView( CameraMode mode, Context context )
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
