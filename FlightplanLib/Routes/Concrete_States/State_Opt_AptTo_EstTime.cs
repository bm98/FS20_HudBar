using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Opt_AptTo_EstTime : AState
  {
    public State_Opt_AptTo_EstTime( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // optional arrival time 
      if (_token.IsEST_TIME) {
        // handle TO EstTime
        _contextRef.Route.ArrEstTime = _token.Arg1;
        _contextRef.Log.AppendLine( $" - arrival time found" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Exit ); // for now Exit, no Alternate Apts collected
        return;
      }
      else {
        _contextRef.Log.AppendLine( $" - arrival time not present" );
        _contextRef.ChangeToState( DecoderState.Opt_AptTo_Runway ); // can be a Runway
        return;
      }
    }

  }
}
