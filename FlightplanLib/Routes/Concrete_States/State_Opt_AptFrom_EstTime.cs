using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Opt_AptFrom_EstTime : AState
  {
    public State_Opt_AptFrom_EstTime( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // optional departure time 
      if (_token.IsEST_TIME) {
        // handle FROM EstTime
        _contextRef.Route.DepEstTime = _token.Arg1;
        _contextRef.Log.AppendLine( $" - departure time found" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Opt_Cruise_SpeedAlt ); // can be a Cruise Remark
        return;
      }
      else {
        _contextRef.Log.AppendLine( $" - departure time not present" );
        _contextRef.ChangeToState( DecoderState.Opt_AptFrom_Runway ); // can be a Runway
        return;
      }
    }
  }
}
