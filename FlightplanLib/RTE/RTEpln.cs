using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.Flightplan;
using FlightplanLib.Routes;

namespace FlightplanLib.RTE
{
  /// <summary>
  /// Get and decode LNM RTE Export RTE files
  /// </summary>
  public class RTEpln
  {
    // not the smartest way to carry the filename into the FlightPlan....
    private static string LastFileRequest = "";

    /// <summary>
    /// Returns the generic FlighPlan from a RTE string or file
    /// </summary>
    /// <param name="rtePlan">An MSFS RTE plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( RouteCapture rtePlan )
    {
      var fp = rtePlan.AsFlightPlan( );
      fp.Source = SourceOfFlightPlan.GEN_Rte;
      fp.FlightPlanFile = LastFileRequest;
      return fp;
    }

    #region Request Handling

    /// <summary>
    /// Event Handler for  RTE data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void RTEplnDataEventHandler( object sender, RTEplnDataEventArgs e );

    /// <summary>
    /// Event triggered on RTE data arrival
    /// </summary>
    public event RTEplnDataEventHandler RTEplnDataEvent;

    // Signal the user that and what data has arrived
    private void OnRTEplnDataEvent( string data ) => RTEplnDataEvent?.Invoke( this, new RTEplnDataEventArgs( data ) );


    /// <summary>
    /// Post a LNM Plan request
    /// </summary>
    /// <param name="fileName">The fully qualified filename</param>
    public void PostDocument_Request( string fileName )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( fileName )) return;
      if (!File.Exists( fileName )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( fileName );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    #endregion

    #region Asynch Request methods

    // Retrieve most current data
    private async Task GetData( string fileName )
    {
      string response = await RTE.Provider.RTEplnRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        LastFileRequest = Path.GetFileName( fileName );

        // signal response
        OnRTEplnDataEvent( response );
      }
    }

    #endregion

  }
}
