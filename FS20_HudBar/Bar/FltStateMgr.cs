using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// Handles the Flight State and changes of it
  /// </summary>
  internal static class FltStateMgr
  {

    private static FlightState _flightState = FlightState.Init;
    private static bool _inFlight  = false;

    /// <summary>
    /// True when in a flight
    /// </summary>
    public static bool IsInFlight => _inFlight;


    /// <summary>
    /// Event triggered when a new Flight has started
    /// </summary>
    public static event EventHandler<EventArgs> FlightStarted;
    private static void OnFlightStarted( )
    {
      _inFlight = true;
      FlightStarted?.Invoke( null, new EventArgs( ) );
    }

    /// <summary>
    /// Event triggered when a Flight has ended or is interrupted (Menu etc)
    /// </summary>
    public static event EventHandler<EventArgs> FlightEnded;
    private static void OnFlightEnded( )
    {
      _inFlight = false;
      FlightEnded?.Invoke( null, new EventArgs( ) );
    }

    /// <summary>
    /// True if the FlightState has changed 
    /// (Use Read() to commit)
    /// </summary>
    public static bool HasChanged { get; private set; } = false;

    /// <summary>
    /// Reset the Manager to be read again
    /// </summary>
    public static void Reset( )
    {
      HasChanged = true;
    }

    /// <summary>
    /// Commit reading the new FlightPlan
    /// </summary>
    /// <returns>The current FlightPlan</returns>
    public static FlightState Read( )
    {
      HasChanged = false;
      return _flightState;
    }


    /// <summary>
    /// static cTor:
    /// </summary>
    static FltStateMgr( )
    {
      SC.SimConnectClient.Instance.FltChange += Instance_FltChange;
    }

    // called when the flight state changes
    private static void Instance_FltChange( object sender, SC.State.FltChangeEventArgs e )
    {
      if ( e.FlightState == _flightState ) return; // no change from known state

      if ( !_inFlight ) {
        // can start
        if ( e.FlightState == FlightState.NormalFlight ) OnFlightStarted( );
        else if ( e.FlightState == FlightState.AutoSaveFlight ) OnFlightStarted( );
      }
      else {
        // can end
        if ( e.FlightState == FlightState.Init ) OnFlightEnded( );
        else if ( e.FlightState == FlightState.FlightPending ) OnFlightEnded( );
        else if ( e.FlightState == FlightState.Stopped ) OnFlightEnded( );
      }
      // indicate for any change in state
      HasChanged = true;
      _flightState = e.FlightState;
    }


  }
}
