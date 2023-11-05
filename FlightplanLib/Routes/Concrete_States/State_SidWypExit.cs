using FSimFacilityDataLib.AirportDB;
using FSimFacilityIF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_SidWypExit : AState
  {
    public State_SidWypExit( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // sanity
      if (!_contextRef.Route.HasSID) {
        // ?? no SID ??
        _contextRef.Log.AppendLine( " - Route has no SID" );
        _contextRef.ChangeToState( DecoderState.Enroute ); // proceed with enroute with this _token
        return;
      }

      // expecting an (optional) SID transition Waypoint
      // having an ID _token SID [TRANSITION]
      if (_token.IsID) {
        if (_contextRef.Route.SID.WaypointIDs.Contains( _token.Arg2 )) {
          // found the Exit in the SID
          AddWypToSID( _token.Arg2 );
        }
        else {
          // SID transition Wyp not found..
          _contextRef.Log.AppendLine( $" - SID Transition Waypoint not in SID waypoints" );
          _contextRef.Route.SID = null; _contextRef.Route.SID_Transition = null;
          // no error this item is optional
        }
        // for all - got to Enroute
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Enroute ); // go to enroute processing
        return;
      }
      else {
        // _token error (neither ID nor DottedID)
        _contextRef.Log.AppendLine( $" - Token cannot be handled <{_token}>" );
        // no error this item is optional
        _contextRef.ChangeToState( DecoderState.Enroute ); // go to enroute processing with this Token
        return;
      }
    }


    private void AddWypToSID( string wypIdent )
    {
      var naids = DbLookup.GetNavaids( wypIdent, Folders.GenAptDBFile );
      // there can be multiple of them... or none
      if (naids.Count( ) == 0) {
        // Wyp not in DB, ignore SID and got to Enroute
        _contextRef.Log.AppendLine( $" SID Transition Waypoint not found in DB" );
        _contextRef.Err.AppendLine( $"Process SID: INFO SID Transition Waypoint not in database - ignoring SID" );
        _contextRef.Route.SID = null; _contextRef.Route.SID_Transition = null;
      }
      else if (naids.Count( ) == 1) {
        // exactly one - use it
        _contextRef.Route.SID_Transition = naids.First( );
      }
      else {
        // more than one - use the closest
        double dist = double.MaxValue;
        INavaid closest = new Navaid( );
        foreach (var naid in naids) {
          double d = _contextRef.Route.DepAirport.Coordinate.DistanceTo( naid.Coordinate );
          closest = (d < dist) ? naid : closest;
          dist = d;
          _contextRef.Route.SID_Transition = closest;
        }
      }
    }

  }
}
