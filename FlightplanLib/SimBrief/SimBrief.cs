using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoordLib;
using FlightplanLib.MSFSPln;
using FlightplanLib.SimBrief.Provider;
using FlightplanLib.SimBrief.SBDEC;
using Windows.UI.Xaml.Controls.Primitives;

namespace FlightplanLib.SimBrief
{
  /// <summary>
  /// Provides Access to SimBrief Data 
  /// 
  /// </summary>
  public class SimBrief
  {
    /// <summary>
    /// True for a valid userID
    /// </summary>
    /// <param name="userID">A SimBrief UserID (6 digit string)</param>
    /// <returns>True if the pattern matches</returns>
    public static bool IsSimBriefUserID( string userID ) => SimBriefRequest.IsSimBriefUserID( userID );

    /// <summary>
    /// Returns the generic FlighPlan from a SimBrief OFP
    /// </summary>
    /// <param name="sbPlan">A SimBrief plan</param>
    /// <returns>A generic FlightPlan obj</returns>
    public static FlightPlan AsFlightPlan( OFP sbPlan )
    {
      var plan = new FlightPlan {
        Source = SourceOfFlightPlan.SimBrief,       // create gen doc items
        Title = "",
        CruisingAlt_ft = sbPlan.General.CruiseAlt_ft,
        FlightPlanType = TypeOfFlightplan.IFR,
        RouteType = (sbPlan.General.CruiseAlt_ft < 18000) ? TypeOfRoute.LowAlt : TypeOfRoute.HighAlt,
        StepProfile = sbPlan.General.StepProfile
      };
      // create Origin
      var loc = new Location {
        Icao_Ident = new IcaoRec { ICAO = sbPlan.Departure.AptICAO },
        Iata_Ident = sbPlan.Departure.AptIATA,
        Name = sbPlan.Departure.Name,
        LatLonAlt_ft = sbPlan.Departure.LatLon,
        RunwayNumber_S = sbPlan.Departure.PlannedRunway, // includes designation
        RunwayDesignation = "",
      };
      plan.Origin = loc;
      // create Destination
      loc = new Location( ) {
        Icao_Ident = new IcaoRec { ICAO = sbPlan.Destination.AptICAO },
        Iata_Ident = sbPlan.Destination.AptIATA,
        Name = sbPlan.Destination.Name,
        LatLonAlt_ft = sbPlan.Destination.LatLon,
        RunwayNumber_S = sbPlan.Destination.PlannedRunway, // includes designation
        RunwayDesignation = "",
      };
      plan.Destination = loc;
      // create waypoints
      var wypList = new List<Waypoint>( );
      // to be consistent across plans - add the origin as Waypoint at the beginning
      // create Waypoint
      var wyp = new Waypoint( ) {
        WaypointType = TypeOfWaypoint.Airport,
        ID = plan.Origin.Icao_Ident.ICAO,
        Name = plan.Origin.Name,
        LatLonAlt_ft = plan.Origin.LatLonAlt_ft,
        Airway_Ident = "",
        Frequency = "",
        Icao_Ident = plan.Origin.Icao_Ident,
        InboundTrueTrk = 0,
        OutboundTrueTrk = -1, // to be calculated
        Distance_nm = 0,
        SID_Ident = "",
        STAR_Ident = "",
        Stage = "",
      };
      wypList.Add( wyp );

      foreach (var fix in sbPlan.NavLog.Waypoints) {
        var icao = new IcaoRec( );// SB are ICAO but also and TOC etc.
        if (fix.WaypointType != TypeOfWaypoint.Other) {
          icao.ICAO = fix.Ident;
        }
        // create Waypoint
        wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          ID = fix.Ident,
          Name = fix.Name,
          LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.Altitude_ft ),
          Airway_Ident = fix.Via_Airway, // SB uses this as Airway but also SID, STAR
          Frequency = fix.Frequency,
          Icao_Ident = icao,
          InboundTrueTrk = fix.TRKt_deg,
          OutboundTrueTrk = -1, // to be calculated
          Distance_nm = fix.Distance_nm,
          SID_Ident = fix.IsSidOrStar ? "-1" : "", // to be calculated - from SB it is not clear
          STAR_Ident = fix.IsSidOrStar ? "-1" : "", // to be calculated - from SB it is not clear
          Stage = fix.Stage,
        };
        wypList.Add( wyp );
      }
      plan.Waypoints = wypList;
      // create Plan Doc HTML
      plan.HTMLdocument = sbPlan.Text.Plan;
      // create Download Images
      if (sbPlan.Image_Files.Files != null) {
        foreach (var fix in sbPlan.Image_Files.Files) {
          plan.ImageLinks.Add( new FileLink( ) { Name = fix.Name, RemoteUrl = sbPlan.Image_Files.Directory, LinkUrl = fix.Link } );
        }
      }
      // create Download Documents
      if (sbPlan.Plan_Files.Files != null) {
        foreach (var fix in sbPlan.Plan_Files.Files) {
          plan.DocLinks.Add( new FileLink( ) { Name = fix.Name, RemoteUrl = sbPlan.Plan_Files.Directory, LinkUrl = fix.Link } );
        }
      }
      // calculate missing items
      plan.RecalcWaypoints( );

      return plan;
    }

    /// <summary>
    /// Event Handler for SimBrief data arrival
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void SimBriefDataEventHandler( object sender, SimBriefDataEventArgs e );

    /// <summary>
    /// Event triggered on SimBrief data arrival
    /// </summary>
    public event SimBriefDataEventHandler SimBriefDataEvent;

    // Signal the user that and what data has arrived
    private void OnSimBriefDataEvent( bool success, string data, SimBriefDataFormat dataFormat )
    {
      SimBriefDataEvent?.Invoke( this, new SimBriefDataEventArgs( success, data, dataFormat ) );
    }

    /// <summary>
    /// Event Handler for SimBrief download confirmation
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Event Arguments</param>
    public delegate void SimBriefDownloadEventHandler( object sender, EventArgs e );

    /// <summary>
    /// Event triggered on SimBrief download confirmation
    /// </summary>
    public event SimBriefDownloadEventHandler SimBriefDownloadEvent;

    // Signal the user download confirmation
    private void OnSimBriefDownloadEvent( )
    {
      SimBriefDownloadEvent?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Post a SimBrief request for a user ID
    /// The caller received an SimBrief Event when finished
    /// </summary>
    /// <param name="userIDorName">The SimBrief Pilot ID or Name</param>
    /// <param name="dataFormat">The SimBrief Data Format to retrieve</param>
    public void PostDocument_Request( string userIDorName, SimBriefDataFormat dataFormat )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( userIDorName )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetData( userIDorName, dataFormat );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Post a SimBrief request for a user ID
    /// The caller received an SimBrief Event when finished
    /// </summary>
    /// <param name="remoteFile">The Remote URL</param>
    /// <param name="localDocName">The local document name</param>
    /// <param name="destPath">The local document location</param>
    public void PostDownload_Request( string remoteFile, string localDocName, string destPath )
    {
      // Sanity checks
      if (string.IsNullOrWhiteSpace( remoteFile )) return;
      if (string.IsNullOrWhiteSpace( localDocName )) return;
      if (string.IsNullOrWhiteSpace( destPath )) return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      GetFile( remoteFile, localDocName, destPath );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }


    #region Asynch Request methods

    // Retrieve most current data
    private async Task GetData( string userIDorName, SimBriefDataFormat dataFormat )
    {
      string response = await SimBriefRequest.GetDocument( userIDorName, dataFormat );
      // signal response
      OnSimBriefDataEvent( !string.IsNullOrWhiteSpace( response ), response, dataFormat );
  }

  // download document files
  private async Task GetFile( string remoteFile, string localDocName, string destPath )
  {
    bool response = await SimBriefRequest.DownloadFile( remoteFile, localDocName, destPath );
    if (response) {
      // signal response
      OnSimBriefDownloadEvent( );
    }
  }

  #endregion

}
}
