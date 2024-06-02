using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimClientIF;
using FCamControl.State.ConcreteStates;

namespace FCamControl.State
{
  internal static class StateFactory
  {
    /// <summary>
    /// Create and return a new State of MSFS_State type
    /// </summary>
    /// <param name="mode">State to be created</param>
    /// <param name="context">The StateHandler Context</param>
    /// <returns>A State object</returns>
    public static AState CreateState( CameraMode mode, Context context )
    {
      switch (mode) {
        case CameraMode.PilotView: return new State_PilotView( mode, context );
        case CameraMode.CloseView: return new State_CloseView( mode, context );
        case CameraMode.LandingView: return new State_LandingView( mode, context );
        case CameraMode.CoPilotView: return new State_CoPilotView( mode, context );

        case CameraMode.InstrumentIndexed: return new State_InstrumentIndexed( mode, context );
        case CameraMode.InstrumentQuick: return new State_CockpitQuick( mode, context );
        case CameraMode.CustomCamera: return new State_CustomCamera( mode, context );
        case CameraMode.ExternalFree: return new State_ExternalFree( mode, context );
        case CameraMode.ExternalQuick: return new State_ExternalQuick( mode, context );
        case CameraMode.ExternalIndexed: return new State_ExternalIndexed( mode, context );
        case CameraMode.Drone: return new State_Drone( mode, context );
        case CameraMode.DOF6: return new State_DOF6( mode, context );

        case CameraMode.NONE: return new State_None( mode, context );
        default:
          throw new ArgumentException( $"StateFactory: Invalid State requested: {mode}" );
      }
    }

  }
}
