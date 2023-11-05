using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.Routes.Concrete_States;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Context of the Route Decoder
  /// </summary>
  internal class Context
  {
    // current State
    private AState _state = null;
    private RouteDecoder _tokenizer;
    private RouteCapture _route = new RouteCapture( );

    // log and error handling
    private readonly StringBuilder _log = new StringBuilder( );
    private readonly StringBuilder _err = new StringBuilder( );

    /// <summary>
    /// The Tokenizer
    /// </summary>
    public RouteDecoder Tokens => _tokenizer;
    /// <summary>
    /// The decoded Route
    /// </summary>
    public RouteCapture Route => _route;

    /// <summary>
    /// Logger - used for the decoder Logging
    /// </summary>
    public StringBuilder Log => _log;
    /// <summary>
    /// Error bucket, collects all error information for the user
    /// </summary>
    public StringBuilder Err => _err;

    /// <summary>
    /// cTor:
    /// </summary>
    public Context( DecoderState state )
    {
      Log.AppendLine( $"Context: Init with <{state}>" );
      // init State Handling
      _state = StateFactory.CreateState( state, this );
    }

    /// <summary>
    /// Derive A GenRoute from the route line 
    /// </summary>
    /// <param name="routeString">One line of roue information</param>
    public void DeriveRoute( string routeString )
    {
      _log.Clear( ); _err.Clear( );
      _route = new RouteCapture( );
      _tokenizer = new RouteDecoder( routeString, _err, _log );

      if (_tokenizer.IsValid) {
        ChangeToState( DecoderState.AptFrom ); // start decoding
      }
      else {
        Err.AppendLine( $"ERROR - failed to decompose the routeString" );
      }
    }

    /// <summary>
    /// Transition to new State
    ///  Note: return immediately after calling this one...
    /// </summary>
    /// <param name="state">New State</param>
    public void ChangeToState( DecoderState state )
    {
      AState oldState = _state;
      _state = StateFactory.CreateState( state, this );
      Log.AppendLine( $"Context: StateChanged <{oldState.State}> -> <{_state.State}>" );
      //    oldState?.Dispose( );

      _state.OnInit( );
    }



  }
}
