using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib.GPX
{
  /// <summary>
  /// Get and decode GPX Export GPX files
  /// </summary>
  public class GPXpln
  {
    // not the smartest way to carry the filename into the FlightPlan....
    private static string LastFileRequest = "";

    /// <summary>
    /// Returns the generic FlighPlan from a GPX file
    /// </summary>
    /// <param name="gpxPlan">An GPX plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( GPXDEC.GPX gpxPlan )
    {
      // sanity - return empty ones
      if (gpxPlan == null) return new FlightPlan( );
      if (!gpxPlan.IsValid) return new FlightPlan( );
      if (!gpxPlan.Route.IsValid) return new FlightPlan( );

      // create gen doc items
      var plan = new FlightPlan {
        FlightPlanFile = LastFileRequest,
        Source = SourceOfFlightPlan.LNM_Gpx,
        Title = gpxPlan.Route.Title,
        CruisingAlt_ft = gpxPlan.Route.CruisingAlt_ft,
        FlightPlanType = gpxPlan.Route.FlightPlanType,
        RouteType = gpxPlan.Route.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };

      // create Origin
      var loc = Formatter.ExpandAirport( gpxPlan.Route.DepartureAirport.ICAO, gpxPlan.Route.DepartureRw.RunwayIdent, LocationTyp.Origin );
      plan.Origin = loc;

      // create Destination
      loc = Formatter.ExpandAirport( gpxPlan.Route.ArrivalAirport.ICAO, gpxPlan.Route.ArrivalRw.RunwayIdent, LocationTyp.Destination );
      plan.Destination = loc;

      // create waypoints
      var wypList = new List<Waypoint>( );
      foreach (var fix in gpxPlan.Route.WaypointCat) {
        // create Waypoint, omit invalid ones
        if (!fix.IsValid) continue;
        // need to insert RW in Procedures (mainly for the Approach)
        string rwIdent = !string.IsNullOrEmpty( fix.SID_Ident ) ? plan.Origin.Runway_Ident
          : !string.IsNullOrEmpty( fix.STAR_Ident ) ? plan.Destination.Runway_Ident
          : !string.IsNullOrEmpty( fix.ApproachProcRef ) ? plan.Destination.Runway_Ident
          : (fix.WaypointType == FSimFacilityIF.WaypointTyp.RWY) ? fix.RunwayIdent
          : "";

        var wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          SourceIdent = string.IsNullOrEmpty( fix.Ident ) ? fix.CoordName : fix.Ident,
          Name = fix.ICAO,
          LatLonAlt_ft = fix.LatLonAlt_ft,
          Airway_Ident = "", // not available
          Frequency = "", // GPX has no Frequ
          Icao_Ident = new IcaoRec( ) { ICAO = string.IsNullOrEmpty( fix.ICAO ) ? fix.CoordName : fix.ICAO, Region = "", AirportRef = "", }, // have no region
          InboundTrueTrk = -1, // need to calculate this
          OutboundTrueTrk = -1, // need to calculate this
          Distance_nm = -1, // need to calculate this
          SID_Ident = fix.SID_Ident,
          STAR_Ident = fix.STAR_Ident,
          ApproachTypeS = fix.ApproachProcRef, // e.g. RNAV, ILS
          ApproachSuffix = fix.ApproachSuffix,
          ApproachSequence = fix.ApproachSequ,
          RunwayNumber_S = rwIdent.RwNumberOf( ),
          RunwayDesignation = rwIdent.RwDesignationOf( ),
          AltitudeLo_ft = fix.AltLo_ft,
          AltitudeHi_ft = fix.AltHi_ft,
          WaypointUsage = fix.UsageType,
          Stage = "", // TODO not avail, need to calculate this
        };
        wypList.Add( wyp );
      }
      plan.Waypoints = wypList;
      // create Plan Doc HTML
      //  NA
      // create Download Images
      //  NA
      // create Download Documents
      //  NA

      // calculate missing items
      plan.PostProcess( );

      return plan;
    }

    #region Request Handling

    /// <summary>
    /// Event Handler for MSFS GPX data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void GPXplnDataEventHandler( object sender, GPXplnDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS GPX data arrival
    /// </summary>
    public event GPXplnDataEventHandler GPXplnDataEvent;

    // Signal the user that and what data has arrived
    private void OnGPXplnDataEvent( string data ) => GPXplnDataEvent?.Invoke( this, new GPXplnDataEventArgs( data ) );


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
      string response = await GPX.Provider.GPXplnRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        LastFileRequest = Path.GetFileName( fileName );

        // signal response
        OnGPXplnDataEvent( response );
      }
    }

    #endregion

  }
}
