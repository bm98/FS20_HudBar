using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal static class StateFactory
  {
    /// <summary>
    /// Create and return a new State of DecoderState type
    /// </summary>
    /// <param name="state">State to be created</param>
    /// <param name="context">The StateHandler Context</param>
    /// <returns>A State object</returns>
    public static AState CreateState( DecoderState state, Context context )
    {
      switch (state) {
        case DecoderState.AptFrom: return new State_AptFrom( state, context );
        case DecoderState.Opt_AptFrom_EstTime: return new State_Opt_AptFrom_EstTime( state, context );
        case DecoderState.Opt_AptFrom_Runway: return new State_Opt_AptFrom_Runway( state, context );
        case DecoderState.Opt_Cruise_SpeedAlt: return new State_Opt_Cruise_SpeedAlt( state, context );
        case DecoderState.SidTrans: return new State_SidTrans( state, context );
        case DecoderState.SidWypExit: return new State_SidWypExit( state, context );
        case DecoderState.Enroute: return new State_Enroute( state, context );
        case DecoderState.Waypoint: return new State_Waypoint( state, context );
        case DecoderState.Opt_Wyp_SpeedAlt: return new State_Opt_Wyp_SpeedAlt( state, context );
        case DecoderState.StarWypEntry: return new State_StarWypEntry( state, context );
        case DecoderState.StarTrans: return new State_StarTrans( state, context );
        case DecoderState.AptTo: return new State_AptTo( state, context );
        case DecoderState.Opt_AptTo_EstTime: return new State_Opt_AptTo_EstTime( state, context );
        case DecoderState.Opt_AptTo_Runway: return new State_Opt_AptTo_Runway( state, context );
        case DecoderState.Exit: return new State_Exit( state, context );
        default:
          throw new ArgumentException( $"StateFactory: Invalid State requested: {state}" );
      }
    }

  }
}
