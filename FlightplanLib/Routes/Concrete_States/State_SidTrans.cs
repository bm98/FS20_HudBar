using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;
using FSimFacilityDataLib.AirportDB;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_SidTrans : AState
  {
    public State_SidTrans( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      if (_token.IsDCT) {
        // a DirectTo Waypoint - No SID, go to Waypoint
        _contextRef.Log.AppendLine( $" - DCT keyword found" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Waypoint ); // expect a Waypoint
        return;
      }

      if (_token.IsSID) {
        // a SID Keyword - connects to the enroute part (here it is ignored)
        _contextRef.Log.AppendLine( $" - SID keyword found" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Enroute ); // expect Enroute Parts
        return;
      }

      // test for SID

      // having an ID _token SID [TRANSITION]
      if (_token.IsID) {
        // Handle SID 
        _contextRef.Route.SID = _contextRef.Route.DepAirport.SIDs.FirstOrDefault( x => x.Ident == _token.Arg1 );
        if (_contextRef.Route.HasSID) {
          _contextRef.Log.AppendLine( $" - SID found for this Airport" );
          _contextRef.Tokens.AdvanceToken( );
          _contextRef.ChangeToState( DecoderState.SidWypExit ); // can be followed by the transition exit Wyp
          return;
        }
        else {
          _contextRef.Log.AppendLine( $" - SID not found in DB" );
          _contextRef.ChangeToState( DecoderState.Enroute ); // try Enroute with this _token
          return;
        }
      }
      // having an dotted ID _token SID.TRANSITON
      else if (_token.IsID_DOTTED) {
        // Handle SID 
        _contextRef.Route.SID = _contextRef.Route.DepAirport.SIDs.FirstOrDefault( x => x.Ident == _token.Arg1 );
        if (_contextRef.Route.HasSID) {
          if (_contextRef.Route.SID.WaypointIDs.Contains( _token.Arg2 )) {
            // found the Exit in the SID
            AddWypToSID( _token.Arg2 );
          }
          else {
            // SID transition Wyp not found..
            _contextRef.Log.AppendLine( $" - SID Transition Waypoint not in SID waypoints" );
            _contextRef.Err.AppendLine( $"Process SID: INFO SID Transition Waypoint not in SID waypoints - ignoring SID" );
            _contextRef.Route.SID = null; _contextRef.Route.SID_Transition = null;
          }
        }
        else {
          // SID was not found for this airport
          _contextRef.Log.AppendLine( $" - SID not found in DB" );
          _contextRef.Err.AppendLine( $"Process SID: INFO SID not found for this Airport - ignoring SID" );
          _contextRef.Route.SID = null; _contextRef.Route.SID_Transition = null;
        }
        // for all - got to Enroute
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Enroute ); // go to enroute processing
        return;
      }

      else {
        // _token error (neither ID nor DottedID)
        _contextRef.Log.AppendLine( $" - _token cannot be handled <{_token}>" );
        _contextRef.Tokens.AdvanceToken( ); // ignore and proceed
        _contextRef.ChangeToState( DecoderState.Enroute ); // go to enroute processing with this _token
        return;
      }
    }


    private void AddWypToSID( string wypIdent )
    {
      var naids = DbLookup.GetNavaids( wypIdent, Folders.GenAptDBFile );
      // there can be multiple of them... or none
      if (naids.Count( ) == 0) {
        // Wyp not in DB, ignore SID and got to Enroute
        _contextRef.Log.AppendLine( $" SID Transition Waypoint not found <{wypIdent}>" );
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
