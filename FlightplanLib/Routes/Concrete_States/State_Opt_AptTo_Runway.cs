using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Opt_AptTo_Runway : AState
  {
    public State_Opt_AptTo_Runway( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // optional arrival runway 
      if (_token.IsRUNWAY) {
        // handle TO Runway
        _contextRef.Route.ArrRunwayID = $"RW{_token.Arg1}";
        _contextRef.Log.AppendLine( $" - arrival runway found" );
        _contextRef.Tokens.AdvanceToken( );
      }
      else {
        _contextRef.Log.AppendLine( $" - arrival runway not present" );
      }
      _contextRef.ChangeToState( DecoderState.Exit ); // for now Exit, no Alternate Apts collected
      return;
    }

  }
}
