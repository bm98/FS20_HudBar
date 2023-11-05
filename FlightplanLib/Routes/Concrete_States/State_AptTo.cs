using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_AptTo : AState
  {
    public State_AptTo( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // must be an ID
      if (!_token.IsID) {
        _contextRef.Err.AppendLine( $"Arrival Airport: expecting ID token <{_token}> is not an ID" );
        _contextRef.Log.AppendLine( " - ERROR invalid token - not an ID" );
        _contextRef.ChangeToState( DecoderState.Exit ); // stop without Arr Airport
        return;
      }

      // handle APT TO 
      _contextRef.Route.ArrAirport = FSimFacilityDataLib.AirportDB.DbLookup.GetAirport( _token.Arg1, Folders.GenAptDBFile );
      if (_contextRef.Route.HasArrival) {
        _contextRef.Log.AppendLine( $" - is Airport" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Opt_AptTo_EstTime ); // try Est Time
        return;
      }
      else {
        // failed, not an apt icao or apt not found
        _contextRef.Err.AppendLine( $"Arrival Airport: expecting an Aiport <{_token}>, airport not found" );
        _contextRef.Log.AppendLine( " - ERROR airport not found" );
        _contextRef.ChangeToState( DecoderState.Exit ); // stop without Arr Airport
        return;
      }

    }
  }
}
