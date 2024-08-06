using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;
using FSFData;

using FlightplanLib.Flightplan;
using FlightplanLib.Routes;
using FlightplanLib.LNM.LNMDEC;

using bm98_hbFolders;

namespace FlightplanLib.LNM
{
  /// <summary>
  /// Get and decode LNM native Plan files
  /// </summary>

  public class LNMpln
  {
    // not the smartest way to carry the filename into the FlightPlan....
    private static string LastFileRequest = "";

    /// <summary>
    /// Returns the generic FlighPlan from a LNM file
    /// USING ROUTE CAPTURING 
    /// </summary>
    /// <param name="rtePlan">An LNM plan served as RouteCapture</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( RouteCapture rtePlan )
    {
      var fp = rtePlan.AsFlightPlan( );
      fp.Source = SourceOfFlightPlan.LNM_Pln;
      return fp;
    }


    /// <summary>
    /// Returns the generic FlighPlan from a LNM file
    /// USING LNM plan decoding
    /// </summary>
    /// <param name="lnmPlan">An LNM plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( LNMDEC.LNM lnmPlan )
    {
      X_Flightplan fp = lnmPlan.Flightplan;
      X_Procedures proc = lnmPlan.Flightplan.Procedures;
      List<X_Waypoint> wyps = lnmPlan.Flightplan.WaypointCat.Waypoints;

      var plan = new FlightPlan {
        FlightPlanFile = LastFileRequest,
        Source = SourceOfFlightPlan.LNM_Pln,       // create gen doc items
        Title = "",
        CruisingAlt_ft = fp.Header.CruiseAlt_ft,
        FlightPlanType = fp.Header.FlightplanType,
        RouteType = (fp.Header.CruiseAlt_ft < 18000) ? TypeOfRoute.LowAlt : TypeOfRoute.HighAlt,
        StepProfile = "", // to be evaluated TODO
      };

      // create Origin
      // Departure Apt / Runway
      // Apt is the 1st Wyp with Type AIRPORT
      var aptWyp = wyps.FirstOrDefault( x => x.WaypointType == WaypointTyp.APT );
      var depAirport = DbLookup.GetAirport( aptWyp.Ident, Folders.GenAptDBFile );
      // Rwy is in the SID or in Departure
      string rwy = "";
      if (lnmPlan.HasSID) {
        rwy = proc.SID.Runway;
      }
      else if (lnmPlan.HasDeparture && lnmPlan.Flightplan.Departure.StartType == StartTypeE.Runway) {
        rwy = fp.Departure.Start;
      }
      rwy = AsRwIdent( rwy ); // fix as Ident
      var depRunway = depAirport?.Runways.FirstOrDefault( r => r.Ident == rwy );
      var loc = Formatter.ExpandAirport( depAirport, depRunway, LocationTyp.Origin );
      // adding a Null when not found in our DB - then all other DB queries may fail anyway
      plan.Origin = loc;

      if (lnmPlan.HasSID) {
        plan.Origin.SetSID( fp.Procedures.SID.Name, proc.SID.Transition );
      }

      // create Destination
      // Arrival Apt / Runway
      // Apt is the last Wyp with Type AIRPORT ?? check how Alternates are handled
      aptWyp = wyps.LastOrDefault( x => x.WaypointType == WaypointTyp.APT );
      var arrAirport = DbLookup.GetAirport( aptWyp.Ident, Folders.GenAptDBFile );
      // Rwy is in the SID or in Departure
      rwy = "";
      if (lnmPlan.HasApproach) {
        rwy = proc.Approach.Runway;
      }
      else if (lnmPlan.HasSTAR) {
        rwy = proc.STAR.Runway;
      }
      rwy = AsRwIdent( rwy ); // fix as Ident
      var arrRunway = arrAirport?.Runways.FirstOrDefault( r => r.Ident == rwy );
      loc = Formatter.ExpandAirport( arrAirport, arrRunway, LocationTyp.Destination );
      // adding a Null when not found in our DB - then all other DB queries may fail anyway
      plan.Destination = loc;

      if (lnmPlan.HasSTAR) {
        plan.Destination.SetSTAR( proc.STAR.Name, proc.STAR.Transition );
      }
      if (lnmPlan.HasApproach) {
        plan.Destination.SetAPR( proc.Approach.Proc, proc.Approach.Suffix, proc.Approach.Transition );
      }

      /*
      // create Alternate Destination  TODO
      */

      // create waypoints
      var wypList = new WaypointList( );

      // adds the Apt, RW
      wypList.AddRange( Formatter.ExpandLocationAptRw( plan.Origin, true, onDeparture: true ) );
      // adds SID if given
      wypList.AddRange( plan.Origin.ExpandSID( ) );
      // track item
      Flightplan.Waypoint prevWyp = (wypList.Count > 0) ? wypList.Last( ) : new Flightplan.Waypoint( );

      foreach (var fix in wyps) {
        if (fix.WaypointType == WaypointTyp.APT) continue; // have it 
        if (fix.WaypointType == WaypointTyp.RWY) continue; // have it 

        var icao = new IcaoRec( );// SB are ICAO but also and TOC etc.
        if (fix.WaypointType != WaypointTyp.OTH) { icao.ICAO = fix.Ident; }

        Flightplan.Waypoint wyp = null;
        bool addWyp = true;
        if (fix.Equals( prevWyp )) {
          // same as before... 
          if (fix.Pos.Alt > 0) {
            // prefer the one with Altitude Information
            wypList.Remove( prevWyp ); // remove the previous one
          }
          else {
            addWyp = false; // omit this one
            wyp = prevWyp; // must have one
          }
        }
        if (addWyp) {
          // create Waypoint
          wyp = new Flightplan.Waypoint( ) {
            WaypointType = fix.WaypointType,
            SourceIdent = fix.Ident,
            OnDeparture = false,
            CommonName = fix.Name,
            LatLonAlt_ft = fix.Pos.Coord,
            Icao_Ident = icao,
            Airway_Ident = fix.Airway,
            Stage = "", // done in Post Proc
          };
          wypList.Add( wyp );
        }
        // next round
        prevWyp = wypList.Last( );
      }
      // Add/Merge STAR and Approach
      //wypList.AddRange(  plan.Destination.ExpandSTAR( ));
      wypList.AddRange( plan.Destination.ExpandSTAR( ).AppendWithMerge( plan.Destination.ExpandAPR( ) ) );

      plan.AddWaypointRange( wypList );

      // create Plan Doc HTML
      //  NA
      // create Download Images
      //  NA
      // create Download Documents
      //  NA

      // calculate missing items
      plan.PostProcess( );

      // release DB records before leaving..
      plan.Origin.ReleaseFacilities( );
      plan.Destination.ReleaseFacilities( );
      plan.Alternate.ReleaseFacilities( );

      return plan;
    }

    #region Request Handling

    /// <summary>
    /// Event Handler for LNM data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void LNMplnDataEventHandler( object sender, LNMplnDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS GPX data arrival
    /// </summary>
    public event LNMplnDataEventHandler LNMplnDataEvent;

    // Signal the user that and what data has arrived
    private void OnLNMplnDataEvent( string data ) => LNMplnDataEvent?.Invoke( this, new LNMplnDataEventArgs( data ) );


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
      string response = await LNM.Provider.LNMplnRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        LastFileRequest = Path.GetFileName( fileName );

        // signal response
        OnLNMplnDataEvent( response );
      }
    }

    #endregion

  }
}
