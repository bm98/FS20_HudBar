using CoordLib;
using FlightplanLib.MSFSPln.PLNDEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace FlightplanLib.MSFSPln
{
  /// <summary>
  /// Get and decode MSFS PLN files
  ///  will take care of decorated B21 Soaring Plans
  /// </summary>
  public class MSFSPln
  {
    /// <summary>
    /// Returns the generic FlighPlan from a SimBrief OFP
    /// </summary>
    /// <param name="plan">A SimBrief plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( PLN msfsPlan )
    {
      // create gen doc items
      var plan = new FlightPlan {
        Source = SourceOfFlightPlan.MS_Pln,
        Title = msfsPlan.FlightPlan.Title,
        CruisingAlt_ft = msfsPlan.FlightPlan.CruisingAlt_ft,
        FlightPlanType= msfsPlan.FlightPlan.FlightPlanType,
        RouteType= msfsPlan.FlightPlan.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };
      // create Origin (for the runway assuming the First Waypoint is the Airport)
      var loc = new Location {
        Icao_Ident = new IcaoRec { ICAO = msfsPlan.FlightPlan.DepartureICAO },
        Iata_Ident = "", // not available
        Name = msfsPlan.FlightPlan.DepartureName,
        LatLonAlt_ft = msfsPlan.FlightPlan.DEP_LatLon,
        RunwayNumber_S = msfsPlan.FlightPlan.WaypointCat.First( ).RunwayNumber_S,
        RunwayDesignation = msfsPlan.FlightPlan.WaypointCat.First( ).RunwayDesignation,
      };
      plan.Origin = loc;
      // create Destination (for the runway assuming the Last Waypoint is the Airport)
      loc = new Location( ) {
        Icao_Ident = new IcaoRec { ICAO = msfsPlan.FlightPlan.DestinationICAO },
        Iata_Ident = "", // not available
        Name = msfsPlan.FlightPlan.DestinationName,
        LatLonAlt_ft = msfsPlan.FlightPlan.DST_LatLon,
        RunwayNumber_S = msfsPlan.FlightPlan.WaypointCat.Last( ).RunwayNumber_S,
        RunwayDesignation = msfsPlan.FlightPlan.WaypointCat.Last( ).RunwayDesignation,
      };
      plan.Destination = loc;
      // create waypoints
      var wypList = new List<Waypoint>( );
      foreach (var fix in msfsPlan.FlightPlan.WaypointCat) {
        // create Waypoint
        var wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          ID = fix.ID,
          Name = fix.Wyp_Ident,
          LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.Altitude_ft ),
          Airway_Ident = fix.Airway_Ident,
          Frequency = "", // PLN has no Frequ
          Icao_Ident = new IcaoRec( ) { ICAO = fix.IcaoRec.ICAO_Ident, Region = fix.IcaoRec.Region, AirportRef = fix.IcaoRec.AirportCode, },
          InboundTrueTrk = -1, // need to calculate this
          OutboundTrueTrk = -1, // need to calculate this
          Distance_nm = -1, // need to calculate this
          SID_Ident = fix.SID_Ident,
          STAR_Ident = fix.STAR_Ident,
          ApproachType = fix.ApproachType,
          ApproachSuffix = fix.Approach_Suffix,
          RunwayNumber_S = fix.RunwayNumber_S,
          RunwayDesignation = fix.RunwayDesignation,
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
      plan.RecalcWaypoints( );

      return plan;
    }


    /// <summary>
    /// Event Handler for MSFS PLN data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void MSFSPlnDataEventHandler( object sender, MSFSPlnDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS PLN data arrival
    /// </summary>
    public event MSFSPlnDataEventHandler MSFSPlnDataEvent;

    // Signal the user that and what data has arrived
    private void OnMSFSPlnDataEvent( string data )
    {
      MSFSPlnDataEvent?.Invoke( this, new MSFSPlnDataEventArgs( data ) );
    }

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

    #region Asynch Request methods

    // Retrieve most current data
    private async Task GetData( string fileName )
    {
      string response = await Provider.PlnRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        // signal response
        OnMSFSPlnDataEvent( response );
      }
    }

    #endregion

  }
}
