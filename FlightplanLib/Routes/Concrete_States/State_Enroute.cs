using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;
using FSimFacilityDataLib.AirportDB;


namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Enroute : AState
  {
    public State_Enroute( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // clean remanents if there are
      _contextRef.Route.AirwayInit = null;

      // Enroute expecting an Enroute or a DCT or a Waypoint or a STAR or an Arrival airport here
      if (_token.IsDCT) {
        _contextRef.Log.AppendLine( $" - not processed here" );
        _contextRef.ChangeToState( DecoderState.Waypoint ); // forward to Wyp without airway
        return;
      }

      if (_token.IsCOORD) {
        _contextRef.Log.AppendLine( $" - not processed here" );
        _contextRef.ChangeToState( DecoderState.Waypoint ); // forward to Wyp without airway
        return;
      }

      if (_token.IsSTAR || _token.IsID_DOTTED) {
        _contextRef.Log.AppendLine( $" - not processed here" );
        _contextRef.ChangeToState( DecoderState.StarTrans ); // forward to STAR without airway
        return;
      }

      // having an ID
      if (_token.IsID) {
        if (DbLookup.HasAirway( _token.Arg1, Folders.GenAptDBFile )) {
          // prep a Waypoint with Enroute
          _contextRef.Route.AirwayInit = new RouteWaypointCapture( ) { AirwayIdent = _token.Arg1 };
          _contextRef.Log.AppendLine( $" - is Airway" );
          _contextRef.Tokens.AdvanceToken( );
          _contextRef.ChangeToState( DecoderState.Waypoint ); // try the exit point
          return;
        }
        else {
          // no such Enroute found
          _contextRef.Log.AppendLine( $" - no such Airway found" );
          _contextRef.ChangeToState( DecoderState.Waypoint ); // may be a Waypoint
          return;
        }
      }

      else {
        // not an ID, DCT or STAR, whom to forward?
        _contextRef.Log.AppendLine( $" - ERROR infeasible token, trying with next one" );
        _contextRef.Err.AppendLine( $"Enroute start: cannot handle token <{_token.TokenS}> , ignored" );
        _contextRef.Tokens.AdvanceToken( );
        _contextRef.ChangeToState( DecoderState.Enroute ); // try with next token
        return;
      }
    }

  }
}
