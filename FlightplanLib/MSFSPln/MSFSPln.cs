using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using FSimFacilityIF;
using FlightplanLib.MSFSPln.PLNDEC;
using static FSimFacilityIF.Extensions;
using FSFData;

namespace FlightplanLib.MSFSPln
{
  /// <summary>
  /// Get and decode MSFS PLN files
  ///  will take care of decorated B21 Soaring Plans
  /// </summary>
  public class MSFSPln
  {
    // not the smartest way to carry the filename into the FlightPlan....
    private static string LastFileRequest = "";

    /// <summary>
    /// Returns the generic FlighPlan from a MSFS PLN file
    /// </summary>
    /// <param name="msfsPlan">An MSFS PLN plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( PLN msfsPlan )
    {
      // sanity
      if (msfsPlan == null) return new FlightPlan( );
      if (!msfsPlan.IsValid) return new FlightPlan( ); // triggers PostProc if not already done

      // create gen doc items
      var plan = new FlightPlan {
        FlightPlanFile = LastFileRequest,
        Source = SourceOfFlightPlan.MS_Pln,
        Title = msfsPlan.FlightPlan.Title,
        CruisingAlt_ft = msfsPlan.FlightPlan.CruisingAlt_ft,
        FlightPlanType = msfsPlan.FlightPlan.FlightPlanType,
        RouteType = msfsPlan.FlightPlan.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };

      // Most of format specific processing has been done in X_FlightPlan as PostProc()
      var loc = Formatter.ExpandAirport( msfsPlan.FlightPlan.DepartureICAO, msfsPlan.FlightPlan.DEP_RwIdent, LocationTyp.Origin );
      plan.Origin = loc;
      plan.Origin.SetSID( msfsPlan.FlightPlan.DEP_SID_Ident );

      // create Destination (for the runway assuming the Last Waypoint is the Airport)
      loc = Formatter.ExpandAirport( msfsPlan.FlightPlan.ArrivalCAO, msfsPlan.FlightPlan.DST_RwIdent, LocationTyp.Destination );
      plan.Destination = loc;
      plan.Destination.SetSTAR( msfsPlan.FlightPlan.DST_STAR_Ident );
      plan.Destination.SetAPR( msfsPlan.FlightPlan.DST_APR_ProcRef, "" );

      // create waypoints 
      // DepApt and RW is usually provided - don't create new ones
      var wypList = new WaypointList( );

      // create the initial WypList from the PLN
      foreach (var fix in msfsPlan.FlightPlan.WaypointCat) {
        // create Waypoint, omit invalid ones
        if (!fix.IsValid) continue;
        // check if we have it already before inserting a new one
        if (wypList.Any( w => w.Ident == fix.Wyp_Ident )) continue; // have it already

        // When having an Approach 
        if (plan.HasApproach && fix.IsAPR
              && fix.WaypointType == WaypointTyp.APT) continue; // ignore Dest Apt- will be inserted by Approach Extension later
        if (plan.HasApproach && fix.RunwayIdent == plan.Destination.Runway_Ident
              && fix.WaypointType == WaypointTyp.RWY) continue; // ignore Dest Runway- will be inserted by Approach Extension later

        var wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          WaypointUsage = fix.UsageType,
          SourceIdent = fix.ID,
          Name = fix.Wyp_Ident,
          LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.AltitudeRounded_ft ),
          Airway_Ident = fix.Airway_Ident,
          Icao_Ident = new IcaoRec( ) { ICAO = fix.IcaoRec.ICAO_Ident, Region = fix.IcaoRec.Region, AirportRef = fix.IcaoRec.AirportCode, },
          Stage = "", // TODO not avail, need to calculate this
        };
        wypList.Add( wyp );
      }
      // merge with DB records where applicable
      if (plan.HasSID) wypList = wypList.Merge( plan.Origin.ExpandSID( ) );
      if (plan.HasSTAR) wypList = wypList.Merge( plan.Destination.ExpandSTAR( ) );
      if (plan.HasApproach) wypList.AddRangeNoPairs( plan.Destination.ExpandAPR( ) );

      plan.Waypoints = wypList;
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
    /// Event Handler for MSFS GPX data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void MSFSPlnDataEventHandler( object sender, MSFSPlnDataEventArgs e );

    /// <summary>
    /// Event triggered on MSFS GPX data arrival
    /// </summary>
    public event MSFSPlnDataEventHandler MSFSPlnDataEvent;

    // Signal the user that and what data has arrived
    private void OnMSFSPlnDataEvent( string data ) => MSFSPlnDataEvent?.Invoke( this, new MSFSPlnDataEventArgs( data ) );


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
      string response = await Provider.PlnRequest.GetDocument( fileName );
      if (!string.IsNullOrWhiteSpace( response )) {
        LastFileRequest = Path.GetFileName( fileName );

        // signal response
        OnMSFSPlnDataEvent( response );
      }
    }

    #endregion

  }
}
