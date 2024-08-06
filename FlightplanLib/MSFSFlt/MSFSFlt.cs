using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CoordLib;

using static FSimFacilityIF.Extensions;
using FSimFacilityIF;
using FSFData;

using FlightplanLib.Flightplan;
using FlightplanLib.MSFSFlt.FLTDEC;

namespace FlightplanLib.MSFSFlt
{
  /// <summary>
  /// Get and decode MSFS FLT files
  ///  will take care of decorated B21 Soaring Plans
  /// </summary>
  public class MSFSFlt
  {
    // not the smartest way to carry the filename into the FlightPlan....
    private static string LastFileRequest = "";

    /// <summary>
    /// Returns the generic FlighPlan from a FLT File
    /// </summary>
    /// <param name="msfsPlan">A FLT plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( FLT msfsPlan )
    {
      // a lot may be missing depending on the FLT state
      if (msfsPlan == null) return new FlightPlan( ); // sanity
      if (msfsPlan.Used_Waypoints.Count < 1) return new FlightPlan( ); // cannot without Wyps

      // create gen doc items
      var plan = new FlightPlan {
        FlightPlanFile = LastFileRequest,
        Source = SourceOfFlightPlan.MS_Pln,
        Title = msfsPlan.Main.Title,
        CruisingAlt_ft = msfsPlan.Used_ATC_FlightPlan.CruisingAltitude_ft,
        FlightPlanType = msfsPlan.Used_ATC_FlightPlan.FlightPlanType,
        RouteType = msfsPlan.Used_ATC_FlightPlan.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };

      string rwy = "";
      // create Origin (for the runway assuming the First Waypoint is the Airport)
      // We may have no waypoints at this point
      if (msfsPlan.Used_Waypoints.Count > 0) {
        rwy = msfsPlan.Waypoint( msfsPlan.Used_Waypoints.First( ).Key ).RwNumber_S
                  + msfsPlan.Waypoint( msfsPlan.Used_Waypoints.First( ).Key ).RwDesignation;

        rwy = AsRwIdent( rwy ); // fix as Ident
      }
      var loc = Formatter.ExpandAirport( msfsPlan.Used_ATC_FlightPlan.DepartureICAO, rwy, LocationTyp.Origin );
      plan.Origin = loc;
      if (loc == null) return new FlightPlan( ); // cannot proceed without Origin, return an empty plan


      // We may have no waypoints at this point
      if (msfsPlan.Used_Waypoints.Count > 0) {
        // create Destination (for the runway assuming the Last Waypoint is the Airport)
        rwy = msfsPlan.Waypoint( msfsPlan.Used_Waypoints.Last( ).Key ).RwNumber_S
                    + msfsPlan.Waypoint( msfsPlan.Used_Waypoints.Last( ).Key ).RwDesignation;

        rwy = AsRwIdent( rwy ); // fix as Ident
      }
      loc = Formatter.ExpandAirport( msfsPlan.Used_ATC_FlightPlan.DestinationICAO, rwy, LocationTyp.Destination );
      plan.Destination = loc;
      if (loc == null) return new FlightPlan( ); // cannot proceed without Destination, return an empty plan

      // create waypoints
      Flightplan.Waypoint prevWyp = new Flightplan.Waypoint( );

      var wypList = new List<Flightplan.Waypoint>( );
      foreach (var fixKey in msfsPlan.Used_Waypoints.Keys) {
        var fix = msfsPlan.Waypoint( fixKey );
        // create Waypoint, omit invalid ones
        if (!fix.IsValid) continue; // there are FLT WaypointCat inserted by MS which are not decoded and return an empty Wyp
        if (fix.WaypointType == WaypointTyp.RWY) continue; // RUNWAY has no valuable information

        var altLo = 0;
        var altHi = 0;
        if (fix.AltLimit == AltLimitType.At) {
          altLo = fix.AltLimit1_ft;
          altHi = fix.AltLimit1_ft;
        }
        else if (fix.AltLimit == AltLimitType.Above) {
          altLo = fix.AltLimit1_ft;
        }
        else if (fix.AltLimit == AltLimitType.Below) {
          altHi = fix.AltLimit1_ft;
        }
        else if (fix.AltLimit == AltLimitType.Between) {
          altLo = fix.AltLimit1_ft;
          altHi = fix.AltLimit2_ft;
        }

        // add an end runway Wyp before the Airport
        // add a start runway Wyp
        if (fix.WaypointType == WaypointTyp.APT && fix.Ident == plan.Origin.Icao_Ident.ICAO) {
          wypList.AddRange( Formatter.ExpandLocationAptRw( plan.Origin, true, onDeparture: true ) );
        }
        else if (fix.WaypointType == WaypointTyp.APT && fix.Ident == plan.Destination.Icao_Ident.ICAO) {
          wypList.AddRange( Formatter.ExpandLocationRwApt( plan.Destination, true, fix.ApproachProcRef, onDeparture: false ) );
        }
        else {
          // add regular Wyp
          var wyp = new Flightplan.Waypoint( ) {
            WaypointType = fix.WaypointType,
            SourceIdent = fix.SourceIdent,
            CommonName = fix.Ident,
            LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.Altitude_ft ),
            AltitudeLimitLo_ft = altLo,
            AltitudeLimitHi_ft = altHi,
            Icao_Ident = new IcaoRec( ) { ICAO = fix.Ident, Region = fix.Region, AirportRef = fix.Airport, },
            Airway_Ident = fix.Airway_Ident,
            SID_Ident = fix.SID_Ident,
            STAR_Ident = fix.STAR_Ident,
            ApproachTypeS = fix.ApproachType,
            ApproachSuffix = fix.ApproachSuffix,
            RunwayNumber_S = fix.RwNumber_S,
            RunwayDesignation = fix.RwDesignation,
          };

          // same as before ? ... 
          if (wyp.Equals( prevWyp )) {
            wyp.Merge( prevWyp );
            wypList.Remove( prevWyp ); // remove the previous one
          }
          wypList.Add( wyp );
          // carry on
          prevWyp = wyp;
        }
      }
      plan.AddWaypointRange( wypList );

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
    public delegate void MSFSFltDataEventHandler( object sender, MSFSFltDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS GPX data arrival
    /// </summary>
    public event MSFSFltDataEventHandler MSFSFltDataEvent;

    // Signal the user that and what data has arrived
    private void OnMSFSFltDataEvent( string data ) => MSFSFltDataEvent?.Invoke( this, new MSFSFltDataEventArgs( data ) );


    /// <summary>
    /// Returns the path for the CustomFlight.pln file in the current installation
    ///   or an empty string if not found
    /// </summary>
    public static string CustomFlightPlan_filename => MS.MsFolders.GetCustomFlight_Plan( );

    /// <summary>
    /// Post a SimBrief request for a user ID
    /// The caller received an SimBrief Event when finished
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
      string response = await Provider.FltRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        // signal response
        LastFileRequest = Path.GetFileName( fileName );

        OnMSFSFltDataEvent( response );
      }
    }

    #endregion

  }
}
