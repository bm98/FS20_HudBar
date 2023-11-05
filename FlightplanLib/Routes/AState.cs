using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes
{

  internal enum DecoderState
  {
    Idle=0,

    AptFrom,
    Opt_AptTo_Runway,
    Opt_AptTo_EstTime,
    Opt_Cruise_SpeedAlt,

    SidTrans,
    SidWypExit,

    Enroute,
    Waypoint,
    Opt_Wyp_SpeedAlt,

    StarWypEntry,
    StarTrans,

    AptTo,
    Opt_AptFrom_Runway,
    Opt_AptFrom_EstTime,

    Exit,
  }

  /// <summary>
  /// Abstraction of one decoding state
  /// </summary>
  internal abstract class AState
  {
    // CLASS IMPLEMENTATION

    // the Context obj ref (set in constructor)
    protected readonly Context _contextRef;
    // State of the implemented Class (set in constructor)
    protected readonly DecoderState _state;

    protected Token _token;

    /// <summary>
    /// Returns the State Enum
    /// </summary>
    public DecoderState State => _state;

    /// <summary>
    /// cTor:
    ///  Note for concrete state classes: 
    ///    Use base() in constructor to complete the base setup
    /// </summary>
    /// <param name="state">Implemented State</param>
    /// <param name="context">Context obj</param>
    public AState( DecoderState state, Context context )
    {
      _state = state;
      _contextRef = context;
    }

    // proto events

    public virtual void OnInit( )
    {
      _token = _contextRef.Tokens.ThisToken; // shortcut
      _contextRef.Log.Append( $"{_state}: token {_token}" );

    }

  }
}
