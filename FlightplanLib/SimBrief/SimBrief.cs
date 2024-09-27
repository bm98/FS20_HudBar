using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.Flightplan;
using FlightplanLib.SimBrief.Provider;
using FlightplanLib.SimBrief.SBDEC;
using FlightplanLib.Routes;

using bm98_hbFolders;
using FSFData;

namespace FlightplanLib.SimBrief
{
  /// <summary>
  /// Provides Access to SimBrief Data 
  ///   DEBUG version allows for File retrieval
  ///   RELEASE version only supports HTTP retrieval
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
      // USING ROUTE INFORMATION DECODING
      var routeS = FullRouteString( sbPlan );
      var _err = new StringBuilder( );
      var route = new RouteDecoder( routeS, _err );
      var fPlan = route.Route.AsFlightPlan( );

      // replace generic FP properties with captured ones
      fPlan.FlightPlanFile = $"SimBrief download (UID:{sbPlan.Params.UserID} - {sbPlan.Params.Timestamp})";
      fPlan.Source = SourceOfFlightPlan.SimBrief;
      fPlan.CruisingAlt_ft = sbPlan.General.CruiseAlt_ft;
      fPlan.FlightPlanType = TypeOfFlightplan.IFR;
      fPlan.RouteType = (sbPlan.General.CruiseAlt_ft < 18000) ? TypeOfRoute.LowAlt : TypeOfRoute.HighAlt;
      fPlan.StepProfile = sbPlan.General.StepProfile;
      fPlan.FPLayout = sbPlan.Params.OFP_Layout.Replace( " ", "_" ); // cannot use spaces internally
      fPlan.Airline_ICAO = sbPlan.General.Airline_ICAO;
      fPlan.FlightNumber = sbPlan.General.FlightNumber;
      fPlan.AircraftReg = sbPlan.Aircraft.RegID_plain;
      fPlan.AircraftType_ICAO = sbPlan.Aircraft.TypeCode_ICAO;
      fPlan.AircraftTypeName = sbPlan.Aircraft.TypeName;

      // create Plan Doc HTML
      fPlan.HTMLdocument = sbPlan.Text.Plan;
      // create PDF doc link
      fPlan.DocLinks.Add( new FileLink( ) { Name = sbPlan.Plan_Files.Pdf_File.Name, RemoteUrl = sbPlan.Plan_Files.Directory, LinkUrl = sbPlan.Plan_Files.Pdf_File.Link } );

      /* DISABLED _ NOT USED SO FAR
      // create Download Images
      if (sbPlan.Image_Files.Files != null) {
        foreach (var fix in sbPlan.Image_Files.Files) {
          fPlan.ImageLinks.Add( new FileLink( ) { Name = fix.Name, RemoteUrl = sbPlan.Image_Files.Directory, LinkUrl = fix.Link } );
        }
      }

      // create Download Documents
      if (sbPlan.Plan_Files.Files != null) {
        foreach (var fileLink in sbPlan.Plan_Files.Files) {
          fPlan.DocLinks.Add( new FileLink( ) { Name = fileLink.Name, RemoteUrl = sbPlan.Plan_Files.Directory, LinkUrl = fileLink.Link } );
        }
      }
      */
      return fPlan;

      /*
// REGULAR DECODING

      var plan = new FlightPlan {
        Source = SourceOfFlightPlan.SimBrief,       // create gen doc items
        Title = "",
        CruisingAlt_ft = sbPlan.General.CruiseAlt_ft,
        FlightPlanType = TypeOfFlightplan.IFR,
        RouteType = (sbPlan.General.CruiseAlt_ft < 18000) ? TypeOfRoute.LowAlt : TypeOfRoute.HighAlt,
        StepProfile = sbPlan.General.StepProfile
      };
      // create Origin
      var loc = Formatter.ExpandAirport( sbPlan.Departure.AptICAO, sbPlan.Departure.RunwayIdent, LocationTyp.Origin );
      // adding a Null when not found in our DB - then all other DB queries may fail anyway
      plan.Origin = loc;

      // create Destination
      loc = Formatter.ExpandAirport( sbPlan.Destination.AptICAO, sbPlan.Destination.RunwayIdent, LocationTyp.Destination );
      plan.Destination = loc;

      // create Alternate Destination
      loc = Formatter.ExpandAirport( sbPlan.Alternate.AptICAO, sbPlan.Alternate.RunwayIdent, LocationTyp.Alternate );
      plan.Alternate = loc;

      / *
      var sidTrans = lnmPlan.HasSID ? proc.SID.Transition : "";
      var dbSid = (lnmPlan.HasSID ? depRunway?.SIDs.FirstOrDefault( s => s.ProcRef == proc.SID.Name ) : null)
              ?? (lnmPlan.HasSID ? arrAirport?.SIDs( ).FirstOrDefault( s => s.ProcRef == proc.SID.Name ) : null);

      var starTrans = lnmPlan.HasSTAR ? proc.STAR.Transition : "";
      var dbStar = (lnmPlan.HasSTAR ? arrRunway?.STARs.FirstOrDefault( s => s.ProcRef == proc.STAR.Name ) : null)
              ?? (lnmPlan.HasSTAR ? arrAirport?.STARs( ).FirstOrDefault( s => s.ProcRef == proc.STAR.Name ) : null);

      var dbApr = (lnmPlan.HasApproach ? arrRunway?.APRs.FirstOrDefault( a => a.ProcRef == plan.ApproachProcRef ) : null)
              ?? (lnmPlan.HasApproach ? arrAirport?.APRs( ).FirstOrDefault( a => a.ProcRef == plan.ApproachProcRef ) : null);
      * /

      // create waypoints
      var wypList = new List<Waypoint>( );
      Waypoint wyp;
      // to be consistent across plans - add the origin as Waypoint at the beginning
      if (sbPlan.HasDeparture) {
        // create Airport Waypoint
        wyp = new Waypoint( ) {
          WaypointType = WaypointTyp.APT,
          SourceIdent = plan.Origin.Icao_Ident.ICAO,
          Name = plan.Origin.Name,
          LatLonAlt_ft = plan.Origin.LatLonAlt_ft,
          Icao_Ident = plan.Origin.Icao_Ident,
          InboundTrueTrk = 0,
          OutboundTrueTrk = -1, // to be calculated
          Distance_nm = 0, // set start
          Stage = "",
        };
        wypList.Add( wyp );
        if (sbPlan.Departure.HasRunway) {
          // create Runway Waypoint
          wyp = new Waypoint( ) {
            WaypointType = WaypointTyp.RWY,
            SourceIdent = plan.Origin.Runway_Ident,
            Name = plan.Origin.Runway_Ident,
            LatLonAlt_ft = plan.Origin.RunwayLatLonAlt_ft,
            Icao_Ident = new IcaoRec( ) { ICAO = plan.Origin.Runway_Ident, Region = plan.Origin.Icao_Ident.Region, AirportRef = plan.Origin.Icao_Ident.ICAO },
            InboundTrueTrk = 0,
            OutboundTrueTrk = -1, // to be calculated
            Distance_nm = 0, // set start
            Stage = "",
          };
          wypList.Add( wyp );
        }
      }


      bool forSid = true, forStar = false;
      foreach (var fix in sbPlan.NavLog.Waypoints) {
        var icao = new IcaoRec( );// SB are ICAO but also and TOC etc.
        if (fix.WaypointType != WaypointTyp.OTH) {
          icao.ICAO = fix.Ident;
        }
        // select SID or STAR Ident, assuming it starts with SID if at all
        string sidIdent = forSid && fix.IsSidOrStar ? fix.Via_Airway : "";
        string starIdent = forStar && fix.IsSidOrStar ? fix.Via_Airway : "";

        // create Waypoint
        wyp = new Waypoint( ) {
          WaypointType = fix.WaypointType,
          SourceIdent = fix.Ident,
          Name = fix.Name,
          LatLonAlt_ft = new LatLon( fix.Lat, fix.Lon, fix.Altitude_ft ),
          Frequency = fix.Frequency,
          Icao_Ident = icao,
          InboundTrueTrk = fix.TRKt_deg,
          OutboundTrueTrk = -1, // to be calculated
          Distance_nm = fix.Distance_nm,
          Airway_Ident = fix.IsSidOrStar ? "" : fix.Via_Airway, // SB uses this as Airway but also SID, STAR
          SID_Ident = sidIdent,
          STAR_Ident = starIdent,
          Stage = fix.Stage,
        };
        / *
        // Add STAR before Arr Airport
        if (wyp.Ident == sbPlan.Destination.AptICAO) {
          // expand STAR/Approach
          wypList.AddRange( Formatter.ExpandSTAR( dbStar, starTrans ) );
          if (lnmPlan.HasApproach) {
            wypList.AddRange( Formatter.ExpandAPR( dbApr ) );
            // Add Arr Airport WYP after MAPR without Calculations
            wyp.Distance_nm = 0; wyp.InboundTrueTrk = 0; wyp.OutboundTrueTrk = 0;
            wypList.Add( wyp );
          }
          else {
            // Add Arr Airport WYP without STAR and/or Approach
            wypList.Add( wyp );
          }
        }
        else {
          // Add Enroute WYP
          wypList.Add( wyp );
        }

        // Add SID only after Dep Airport
        if (wyp.Ident == depAirport.Ident) {
          // expand SID
          wypList.AddRange( Formatter.ExpandSID( dbSid, sidTrans ) );
        }

        * /







        wypList.Add( wyp );
        if (string.IsNullOrEmpty( sidIdent )) { forSid = false; forStar = true; }// change Proc if SID is exhausted
      }
      plan.Waypoints = wypList;

      // calculate missing items
      plan.RecalcWaypoints( );

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

      return plan;
      */
    }


    // SB has some route strings available
    //  ATC.RouteS / Route with SpeedAlt but without Apts/RW and Ozeanic Wyps
    //  General.RouteNGS / Route with Ozeanic Wyps (56N020W) without SpeedAlt, Apts/RW
    // we need one with Airport/RW + SpeedAlt + Oceanic Wyps...
    private static string FullRouteString( OFP capture )
    {
      /*
       Example: 
      ATC:
      <N0444F340 CPT3F CPT L9 DIDZA N14 OKTAD DCT MEDOG DCT KRAGY DCT PEMOB DCT VATRY DCT ENOKU DCT DEVOL DCT RESNO/M076F360 NATE NEEKO/N0444F380 N478A TOPPS DCT SEAER DCT AJJAY OOSHN5>
      RouteNGS:
      <CPT3F CPT L9 DIDZA N14 OKTAD DCT MEDOG DCT KRAGY DCT PEMOB DCT VATRY DCT ENOKU DCT DEVOL DCT RESNO DCT 56N020W 57N030W 56N040W 54N050W DCT NEEKO N478A TOPPS DCT SEAER DCT AJJAY OOSHN5>
      Desired:
      <EGLL/27R N0444F340 CPT3F CPT L9 DIDZA N14 OKTAD DCT MEDOG DCT KRAGY DCT PEMOB DCT VATRY DCT ENOKU DCT DEVOL DCT RESNO/M076F360 DCT 56N020W 57N030W 56N040W 54N050W DCT NEEKO/N0444F380 N478A TOPPS DCT SEAER DCT AJJAY OOSHN5 KBOS/22L>

      where NATx in ATC is a NorthAtlanic route not in NG database (will be discarded when not found)
       */
      string route = "";
      string endMark = "$$$"; // to not exhaust the lines in indexing below..
      var rNG = (capture.General.RouteNGS + " " + endMark).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
      var rATC = (capture.Atc.RouteS + " " + endMark).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
      // scan both
      int iNG = 0, iATC = 0;
      while (iNG < rNG.Length) {
        if (rNG[iNG] == endMark) break;
        if (rATC[iATC] == endMark) break;

        if (rATC[iATC] == rNG[iNG]) {
          // identical
          route += " " + rNG[iNG]; // use any..
          iNG++; // next NG 
          iATC++; // next ATC 
        }
        else if (rATC[iATC].StartsWith( rNG[iNG] ) && rATC[iATC].Replace( rNG[iNG], "" ).StartsWith( "/" )) {
          // atc has a / extension (SpeedAlt..)
          route += " " + rATC[iATC]; // use ATC
          iNG++; // next NG 
          iATC++; // next ATC 
        }
        else {
          // atc field missing or NG field missing...
          if (rATC[iATC + 1].StartsWith( rNG[iNG] )) {
            // check one further and take one from ATC if they match later
            route += " " + rATC[iATC]; // use ATC
            // don't advance NG until we find a matching one
            iATC++; // next ATC 
          }
          else {
            // take from NG
            route += " " + rNG[iNG]; // use NG
            iNG++; // next NG 
            // don't advance ATC until we find a matching one
          }
        }
      }

      // add Airports and Ruwnays
      var depApt = capture.Departure.AptICAO;
      if (!string.IsNullOrWhiteSpace( capture.Departure.RunwayNumberS )) {
        depApt += "/" + capture.Departure.RunwayNumberS + capture.Departure.RunwayDesignation;
      }
      var arrApt = capture.Arrival.AptICAO;
      if (!string.IsNullOrWhiteSpace( capture.Arrival.RunwayNumberS )) {
        arrApt += "/" + capture.Arrival.RunwayNumberS + capture.Arrival.RunwayDesignation;
      }
      var altApt = capture.Alternate.AptICAO; // can be empty...
      if (!string.IsNullOrWhiteSpace( capture.Alternate.RunwayNumberS )) {
        altApt += "/" + capture.Alternate.RunwayNumberS + capture.Alternate.RunwayDesignation;
      }
      route = $"{depApt} " + route + $" {arrApt} {altApt}";

      return route.Replace( "  ", " " ).ToUpperInvariant( ); // remove double spaces
    }

    #region Request Handling

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



#if DEBUG
    bool USE_HTTP = true;  // CHANGE FOR DEBUGGING
#endif

    /// <summary>
    /// Post a SBrief Json file plan request
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
#if DEBUG
      if (USE_HTTP) {
        GetData( userIDorName, dataFormat );
      }
      else {
        string jFile = Path.Combine( Folders.UserFilePath, "SB_TEST.json" );
        GetData( jFile );
      }
#else
      GetData( userIDorName, dataFormat );
#endif
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
#if DEBUG
      if (USE_HTTP) {
        GetFile( remoteFile, localDocName, destPath ); // retrieve only when using HTTP
      }
#else
      GetFile( remoteFile, localDocName, destPath ); // retrieve only when using HTTP
#endif
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    #endregion

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


    // Provide File Loading

    // Retrieve a File
    private async Task GetData( string fileName )
    {
      string response = await SimBriefRequest.GetDocument( fileName );
      // signal response
      OnSimBriefDataEvent( !string.IsNullOrWhiteSpace( response ), response, SimBriefDataFormat.JSON );
    }


    #endregion

  }
}
