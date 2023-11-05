using FSimFacilityDataLib.AirportDB;
using FSimFacilityIF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Waypoint : AState
  {
    public State_Waypoint( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // expecting an ID or COORD either standalone or belonging to an Enroute 
      if (_token.IsDCT) {
        _contextRef.Log.AppendLine( $" - DCT keyword found" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Waypoint ); // expect a Waypoint
        return;
      }

      if (_token.IsSTAR || _token.IsID_DOTTED) {
        _contextRef.Log.AppendLine( $" - not processed here" );
        _contextRef.ChangeToState( DecoderState.StarTrans );
        return;
      }

      // ID of an Enroute entry or a standalone one
      if (_token.IsID) {
        if (_contextRef.Route.HasAirwayInit) {
          // Enroute present - so we _should_ find an Exit
          if (AddWypToAirway( _token.Arg1 )) {
            _contextRef.Log.AppendLine( $" - add waypoint to airway and commit routepoint" );
          }
          else {
            // no match
            _contextRef.Log.AppendLine( $" - ERROR could not match airway and waypoint" );
            _contextRef.Err.AppendLine( $"Enroute: could not match airway and waypoint <{_contextRef.Route.AirwayInit.AirwayIdent},{_token.Arg1}>" );
          }
          // for all above
          _contextRef.Route.AirwayInit = null; // clear 
          _contextRef.Tokens.AdvanceToken( );
          _contextRef.ChangeToState( DecoderState.Opt_Wyp_SpeedAlt ); // can be a remark
          return;
        }
        else {
          // standalone or STAR or ArrAirport..
          if (AddWyp( _token.Arg1 )) {
            _contextRef.Log.AppendLine( $" - add waypoint and commit routepoint" );
            _contextRef.Tokens.AdvanceToken( );
            _contextRef.ChangeToState( DecoderState.Opt_Wyp_SpeedAlt ); // can be a remark
            return;
          }
          else {
            // not a waypoint 
            _contextRef.ChangeToState( DecoderState.StarTrans ); // may be already STAR
            return;
          }
        }
      }

      else if (_token.IsCOORD) {
        // standalone coord Arg1=lat, Arg2=lon 
        if (double.TryParse( _token.Arg1, out double lat ) && double.TryParse( _token.Arg2, out double lon )) {
          _contextRef.Route.Waypoints.Add( new RouteWaypointCapture( ) { Coord = new CoordLib.LatLon( lat, lon ) } );
        }
        else {
          _contextRef.Log.AppendLine( $" - ERROR cannot create LatLon from coord" );
          _contextRef.Err.AppendLine( $"Waypoint: coordinate conversion failed lat/lon <{_token.Arg1},{_token.Arg2}>" );
        }
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Opt_Wyp_SpeedAlt ); // may be followed by a remark
        return;
      }

      else {
        // _token error (neither ID nor COORD)
        _contextRef.Log.AppendLine( $" - _token cannot be handled <{_token}>" );
        _contextRef.Tokens.AdvanceToken(); // ignore and proceed
        _contextRef.ChangeToState( DecoderState.Enroute ); // go to enroute processing with this _token
        return;
      }
    }


    private bool AddWyp( string wypIdent )
    {
      if (DbLookup.HasNavaid( wypIdent, Folders.GenAptDBFile )) {
        // exists..
        _contextRef.Route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointIdent = wypIdent } );
        return true;
      }
      return false;
    }

    private bool AddWypToAirway( string wypIdent )
    {
      // get Enroute with Ident and see if any has the Wyp in the list 
      // we only want to make sure that an airwas with this waypoint exists... (not if it is the correct airway)
      var airways = DbLookup.GetAirways( _contextRef.Route.AirwayInit.AirwayIdent, Folders.GenAptDBFile );
      foreach (var airway in airways) {
        if (airway.WaypointIDs.Contains( wypIdent )) {
          _contextRef.Route.AirwayInit.Airway = airway;
          _contextRef.Route.AirwayInit.WaypointIdent = wypIdent;
          _contextRef.Route.Waypoints.Add( _contextRef.Route.AirwayInit );
          return true;
        }
      }
      return false;
    }



  }
}
