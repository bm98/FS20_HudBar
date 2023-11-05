using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Opt_AptFrom_Runway : AState
  {
    public State_Opt_AptFrom_Runway( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // optional runway 
      if (_token.IsRUNWAY) {
        // handle FROM Runway
        _contextRef.Route.DepRunwayID = $"RW{_token.Arg1}";
        _contextRef.Log.AppendLine( $" - departure runway found" );
        _contextRef.Tokens.AdvanceToken( );
      }
      else {
        _contextRef.Log.AppendLine( $" - departure runway not present" );
      }
      _contextRef.ChangeToState( DecoderState.Opt_Cruise_SpeedAlt ); // can be a Cruise Remark
      return;
    }

  }
}
