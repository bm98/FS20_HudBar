using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;
using CoordLib.MercatorTiles;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

using FSFData;

using FlightplanLib.Flightplan;

using bm98_hbFolders;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Item to capture all aspects of a route while decoding
  /// </summary>
  public class RouteCapture
  {

    /// <summary>
    /// True when valid
    /// </summary>
    public bool IsValid { get; internal set; } = false;

    /// <summary>
    /// Title of the Route
    /// </summary>
    public string Title { get; internal set; } = "";

    /// <summary>
    /// Type of the Plan captured
    /// </summary>
    public TypeOfFlightplan FlightPlanType { get; internal set; } = TypeOfFlightplan.IFR;
    /// <summary>
    /// Type of the Route captured
    /// </summary>
    public TypeOfRoute RouteType { get; internal set; } = TypeOfRoute.LowAlt;
    /// <summary>
    /// Departure Airport
    /// </summary>
    public IAirport DepAirport { get; internal set; } = null;
    /// <summary>
    /// True if the Route contains a Departure Airport
    /// </summary>
    public bool HasDeparture => DepAirport != null;
    /// <summary>
    /// Departure Airport Runway ID (e.g. RW02C) if available
    /// </summary>
    public string DepRwIdent { get; internal set; } = "";
    /// <summary>
    /// Departure Airport Est Time 
    /// </summary>
    public string DepEstTime { get; internal set; } = "";
    /// <summary>
    /// Departure Runway if a Departure Apt exists and a known RunwayID is given else null
    /// </summary>
    public IRunway DepRunway {
      get {
        if (string.IsNullOrEmpty( DepRwIdent ) || DepAirport == null) return null;
        return DepAirport.Runways.FirstOrDefault( x => x.Ident == DepRwIdent );
      }
    }

    /// <summary>
    /// Initial Cruise Speed and Altitude (IsValid = false if not set)
    /// </summary>
    public SpeedAltRemark CruiseSpeedAlt { get; internal set; } = SpeedAltRemark.Empty;

    /// <summary>
    /// SID Ident if used
    /// </summary>
    public IProcedure SID { get; internal set; } = null;
    /// <summary>
    /// True if the Route contains a SID
    /// </summary>
    public bool HasSID => SID != null;
    /// <summary>
    /// The SID Transition Endpoint if SID is given, else null
    /// </summary>
    public RouteWaypointCapture SID_Transition { get; internal set; } = null;

    /// <summary>
    /// Waypoints of the route
    /// </summary>
    public List<RouteWaypointCapture> Waypoints { get; internal set; } = new List<RouteWaypointCapture>( );
    /// <summary>
    /// Temporary Item to capture Airways with Waypoints
    /// </summary>
    public RouteWaypointCapture AirwayInit { get; internal set; } = null;
    /// <summary>
    /// True if a AirwayInit is present
    /// </summary>
    public bool HasAirwayInit => AirwayInit != null;

    /// <summary>
    /// STAR Ident if used
    /// </summary>
    public IProcedure STAR { get; internal set; } = null;
    /// <summary>
    /// True if the Route contains a STAR
    /// </summary>
    public bool HasSTAR => STAR != null;
    /// <summary>
    /// The STAR Transition Startpoint if STAR is given, else null
    /// </summary>
    public RouteWaypointCapture STAR_Transition { get; internal set; } = null;

    /// <summary>
    /// Arrival Airport
    /// </summary>
    public IAirport ArrAirport { get; internal set; } = null;
    /// <summary>
    /// True if the Route contains a Arrival Airport
    /// </summary>
    public bool HasArrival => ArrAirport != null;
    /// <summary>
    /// Arrival Airport Runway ID (e.g. RW02C) if available
    /// </summary>
    public string ArrRwIdent { get; internal set; } = "";
    /// <summary>
    /// Arrival Runway if an Arrival Apt exists and a known RunwayID is given else null
    /// </summary>
    public IRunway ArrRunway {
      get {
        if (string.IsNullOrEmpty( ArrRwIdent ) || ArrAirport == null) return null;
        return ArrAirport.Runways.FirstOrDefault( x => x.Ident == ArrRwIdent );
      }
    }

    /// <summary>
    /// The Approach Ident (e.g. ILS) if available 
    /// </summary>
    public string ApproachIdent { get; internal set; } = "";
    /// <summary>
    /// The Approach Suffix (e.g. Z) if available 
    /// </summary>
    public string ApproachSuffix { get; internal set; } = "";
    /// <summary>
    /// True if the Route contains an Approach Ident
    /// </summary>
    public bool HasApproach => !string.IsNullOrEmpty( ApproachIdent );

    /// <summary>
    /// Arrival Airport Est Time 
    /// </summary>
    public string ArrEstTime { get; internal set; } = "";

    /// <summary>
    /// List of alternate airports
    /// </summary>
    public List<string> AltAirports { get; internal set; } = new List<string>( );


    /// <summary>
    /// Returns the Capture as regular FlightPlan
    /// </summary>
    /// <returns>A FlightPlan</returns>
    public FlightPlan AsFlightPlan( ) => AsFlightPlan( this );


    /// <summary>
    /// Returns the Capture as regular FlightPlan
    /// </summary>
    /// <param name="rteCap"></param>
    /// <returns>A FlightPlan</returns>
    public static FlightPlan AsFlightPlan( RouteCapture rteCap )
    {
      // sanity - return empty ones
      if (rteCap == null) return new FlightPlan( );
      if (!rteCap.IsValid) return new FlightPlan( );

      // create gen doc items
      var plan = new FlightPlan {
        Source = SourceOfFlightPlan.GEN_Rte,
        Title = rteCap.Title,
        CruisingAlt_ft = rteCap.CruiseSpeedAlt.AsFeet,
        FlightPlanType = rteCap.FlightPlanType,
        RouteType = rteCap.RouteType,
        StepProfile = "" // dont have one or need to calculate it
      };

      Location loc;
      var wypList = new List<Flightplan.Waypoint>( );
      SpeedAltRemark currentSA = new SpeedAltRemark( ); // track profile

      Flightplan.Waypoint wyp;
      // create Origin
      if (rteCap.HasDeparture) {
        // if Runway not given, try if there is a SID and a Runway (could be ALL)
        if (string.IsNullOrEmpty( rteCap.DepRwIdent ) && rteCap.HasSID) {
          rteCap.DepRwIdent = rteCap.SID.RunwayIdent;
        }
        loc = Formatter.ExpandAirport( rteCap.DepAirport, rteCap.DepRwIdent, LocationTyp.Origin );
        // adding a Null when not found in our DB - then all other DB queries may fail anyway
        wypList.AddRange( Formatter.ExpandLocationAptRw( loc, true, onDeparture: true ) ); // add Apt + RWY
      }
      else {
        // no Departure Apt, use the first Waypoint as Origin
        var orig = rteCap.Waypoints.FirstOrDefault( );
        if (orig != default) {
          loc = new Location( ) {
            Icao_Ident = new IcaoRec( ) { ICAO = orig.WaypointIdent },
            Name = orig.WaypointIdent,
            LatLonAlt_ft = orig.Coord
          };
        }
        else {
          // that would be an empty capture
          return new FlightPlan( );
        }
      }
      plan.Origin = loc;


      // create waypoints
      Quad lastQuad6 = Quad.Empty;
      LatLon ll;
      WaypointTyp wypType;
      string prevWypIdent = ""; // need to track 

      // SID
      if (rteCap.HasSID) {
        // must have an airport then...
        lastQuad6 = rteCap.DepAirport.Quad6;
        var transitionIdent = rteCap.SID_Transition == null ? "" : rteCap.SID_Transition.WaypointIdent;
        var sidFixes = rteCap.SID.ExpandFixes( transitionIdent );
        var wyps = DbLookup.WaypointList( sidFixes, Folders.GenAptDBFile );
        foreach (var fix in sidFixes) {
          var w = wyps.Find( wayp => wayp.KEY == fix.WYP_FKey );
          // try get the 'real' waypoint used as there are many with the same name
          // create a Waypoint
          wyp = new Flightplan.Waypoint( ) {
            WaypointType = w.WaypointType,
            SourceIdent = w.Ident,
            CommonName = w.Ident,
            LatLonAlt_ft = w.Coordinate, // has no valid Altitude, the Fix carries it
            Icao_Ident = new IcaoRec( ) { ICAO = w.Ident, Region = w.Region, AirportRef = "", }, // have no region
            SID_Ident = rteCap.SID.Ident,

            Stage = "", // TODO not avail, need to calculate this
          };
          wyp.SetAltitude_ft( fix.AltitudeLo_ft );
          wyp.AltitudeLimitHi_ft = fix.AltitudeHi_ft;
          if (wyp.Ident != prevWypIdent) wypList.Add( wyp );
          prevWypIdent = wyp.Ident;
        }
      }

      // enroute
      {
        //   string prevWyp = "";
        foreach (var routeWyp in rteCap.Waypoints) {
          currentSA = routeWyp.SpeedAlt; // track where we know it (waypoints if in Route profile)
          if (!string.IsNullOrEmpty( routeWyp.AirwayIdent )) {
            // uses Airway from..to
            var awyFixes = DbLookup.ExpandAirwayFixes( routeWyp.AirwayIdent, prevWypIdent, routeWyp.WaypointIdent, Folders.GenAptDBFile );
            foreach (var fix in awyFixes) {
              if (fix.WYP == default) continue; // No Wyp Loaded ??? does not exist in DB - ignore
              // create a Waypoint
              wyp = new Flightplan.Waypoint( ) {
                WaypointType = fix.WYP.WaypointType,
                SourceIdent = fix.WYP.Ident,
                CommonName = fix.WYP.Ident,
                LatLonAlt_ft = fix.WYP.Coordinate,
                Airway_Ident = routeWyp.AirwayIdent,
                Icao_Ident = new IcaoRec( ) { ICAO = fix.WYP.Ident, Region = fix.WYP.Region, AirportRef = "", }, // have no region
                Stage = "", // TODO not avail, need to calculate this
              };
              wyp.SetAltitude_ft( currentSA.IsValid ? currentSA.AsFeet : double.NaN );
              if (wyp.Ident != prevWypIdent) wypList.Add( wyp );
              prevWypIdent = wyp.Ident;
            }
          }
          else {
            // standalone Wyp
            // create a Waypoint
            WaypointLookup( routeWyp.WaypointIdent, currentSA, out ll, out wypType, ref lastQuad6 );
            if (ll.IsEmpty && !routeWyp.Coord.IsEmpty) {
              // a USER or COORD waypoint with coordinates
              lastQuad6 = routeWyp.Coord.AsQuad( 6 );
              if (routeWyp.SpeedAlt.IsValid) currentSA = routeWyp.SpeedAlt;
              wypType = routeWyp.WaypointType;
              ll = new LatLon( routeWyp.Coord.Lat, routeWyp.Coord.Lon, currentSA.AsFeet );
            }
            if (!ll.IsEmpty) {
              // valid Coordinate
              wyp = new Flightplan.Waypoint( ) {
                WaypointType = wypType,
                SourceIdent = routeWyp.WaypointIdent,
                CommonName = routeWyp.WaypointIdent,
                LatLonAlt_ft = ll,
                Icao_Ident = new IcaoRec( ) { ICAO = routeWyp.AirwayIdent, Region = "", AirportRef = "", }, // have no region
                Stage = "", // TODO not avail, need to calculate this
              };
              wyp.SetAltitude_ft( currentSA.IsValid ? currentSA.AsFeet : double.NaN );
              if (wyp.Ident != prevWypIdent) wypList.Add( wyp );
              prevWypIdent = wyp.Ident;
            }
            // carry last Wyp
          }
          prevWypIdent = routeWyp.WaypointIdent;
        }
      }

      //STAR
      if (rteCap.HasSTAR) {
        currentSA = new SpeedAltRemark( ); // invalidate - we don't know the profile of the STAR
        var transitionIdent = rteCap.STAR_Transition == null ? "" : rteCap.STAR_Transition.WaypointIdent;
        var starFixes = rteCap.STAR.ExpandFixes( transitionIdent );
        var wyps = DbLookup.WaypointList( starFixes, Folders.GenAptDBFile );
        foreach (var fix in starFixes) {
          var w = wyps.Find( wayp => wayp.KEY == fix.WYP_FKey );
          // try get the 'real' waypoint used as there are many with the same name
          // create a Waypoint
          wyp = new Flightplan.Waypoint( ) {
            WaypointType = w.WaypointType,
            SourceIdent = w.Ident,
            CommonName = w.Ident,
            LatLonAlt_ft = w.Coordinate,
            Icao_Ident = new IcaoRec( ) { ICAO = w.Ident, Region = w.Region, AirportRef = "", }, // have no region
            STAR_Ident = rteCap.STAR.Ident,
            Stage = "", // TODO not avail, need to calculate this
          };
          wyp.SetAltitude_ft( fix.AltitudeLo_ft );
          wyp.AltitudeLimitHi_ft = fix.AltitudeHi_ft;
          if (wyp.Ident != prevWypIdent) wypList.Add( wyp );
          prevWypIdent = wyp.Ident;
        }
      }
      // APPROACH / must have an Arrival Apt
      if (rteCap.HasApproach && rteCap.HasArrival) {
        // approach name is like 'ILS Z' (no dash..)
        string approachName = rteCap.ApproachIdent + (string.IsNullOrEmpty( rteCap.ApproachSuffix ) ? "" : $" {rteCap.ApproachSuffix}");
        approachName += string.IsNullOrEmpty( rteCap.ArrRwIdent ) ? "" : $" {rteCap.ArrRwIdent}";
        // resolves to e.g. ILS Z RW03C

        string appKey = AsKEY( AsKEY( rteCap.ApproachIdent, rteCap.ApproachSuffix ), rteCap.ArrRwIdent );
        var apr = rteCap.ArrAirport.APRs( ).FirstOrDefault( proc => proc.ItemKey == appKey );
        if (apr != default) {
          var aprFixes = apr.CommonFixes;
          var wyps = DbLookup.WaypointList( aprFixes, Folders.GenAptDBFile );
          foreach (var fix in aprFixes) {
            var w = wyps.Find( wayp => wayp.KEY == fix.WYP_FKey );
            wyp = new Flightplan.Waypoint( ) {
              WaypointType = w.WaypointType,
              SourceIdent = w.Ident,
              CommonName = w.Ident,
              LatLonAlt_ft = w.Coordinate,
              Icao_Ident = new IcaoRec( ) { ICAO = w.Ident, Region = w.Region },
              ApproachTypeS = rteCap.ApproachIdent,
              ApproachSuffix = rteCap.ApproachSuffix,
              RunwayNumber_S = apr.RunwayIdent.RwNumberOf( ),
              RunwayDesignation = apr.RunwayIdent.RwDesignationOf( ),
              Stage = "", // TODO not avail, need to calculate this
            };
            wyp.SetAltitude_ft( fix.AltitudeLo_ft );
            if (wyp.Ident != prevWypIdent) wypList.Add( wyp );
            prevWypIdent = wyp.Ident;
          }
        }
      }

      // Create the Destination
      loc = new Location( );
      // create Destination
      if (rteCap.HasArrival) {
        // if Runway not given, try if there is a STAR and a Runway (could be ALL)
        if (string.IsNullOrEmpty( rteCap.ArrRwIdent ) && rteCap.HasSTAR) {
          rteCap.ArrRwIdent = rteCap.STAR.RunwayIdent;
        }
        loc = Formatter.ExpandAirport( rteCap.ArrAirport, rteCap.ArrRwIdent, LocationTyp.Destination );
        // add a Runway if we don't have an Approach, and an APT (don't use Approach Proc)
        wypList.AddRange( Formatter.ExpandLocationRwApt( loc, !rteCap.HasApproach, "", onDeparture: false ) );
      }
      else {
        // No Arrival Apt, use the last Waypoint as Destination
        var dst = rteCap.Waypoints.LastOrDefault( );
        if (dst != default) {
          loc = new Location( ) {
            Icao_Ident = new IcaoRec( ) { ICAO = dst.WaypointIdent },
            Name = dst.WaypointIdent,
            LatLonAlt_ft = dst.Coord
          };
        }
      }
      plan.Destination = loc;

      // finally
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

    // attempts to find a Wyp
    private static void WaypointLookup( string wypIdent, SpeedAltRemark speedAlt, out LatLon ll, out WaypointTyp wypType, ref Quad lastQuad6 )
    {
      ll = LatLon.Empty;
      wypType = WaypointTyp.Unknown;

      // see if there are many
      var rWyps = DbLookup.WaypointList( wypIdent, Folders.GenAptDBFile );
      if (rWyps.Count( ) == 1) {
        // only one in list, return it independent of its location
        lastQuad6 = rWyps.First( ).Quad6;
        ll = rWyps.First( ).Coordinate;
        wypType = rWyps.First( ).WaypointType;
      }

      else if (rWyps.Count( ) > 1) {
        IWaypoint wyp;

        // get the the Quads around
        var qList = Quad.Around9( lastQuad6 );
        wyp = qList.SelectMany( x => rWyps.Where( y => x.Includes( y.Quad6 ) ) ).FirstOrDefault( );
        if (wyp == default) {

          // try zoom out
          qList = Quad.Around9( lastQuad6.AtZoom( 5 ) );
          wyp = qList.SelectMany( x => rWyps.Where( y => x.Includes( y.Quad6 ) ) ).FirstOrDefault( );
          Console.WriteLine( $"@@@@@ ZOOMED OUT to 5 for <{wypIdent}>" );

          // try zoom out once more...
          if (wyp == default) {
            // try zoom out
            qList = Quad.Around9( lastQuad6.AtZoom( 4 ) );
            wyp = qList.SelectMany( x => rWyps.Where( y => x.Includes( y.Quad6 ) ) ).FirstOrDefault( );
            Console.WriteLine( $"@@@@@ ZOOMED OUT to 4 for <{wypIdent}>" );
          }

          if (wyp == default) {
            // still nothing found
            Console.WriteLine( $"@@@@@ DID NOT FIND AT ZOOM 4 <{wypIdent}>" );
            ll = LatLon.Empty;
          }
        }
        lastQuad6 = wyp.Quad6;
        ll = wyp.Coordinate;
        wypType = wyp.WaypointType;
      }
      else {
        // nothing found
        Console.WriteLine( $"@@@@@ DID NOT FIND <{wypIdent}> IN LOOKUP" );
        ll = LatLon.Empty;
      }

      // fix Altitude if possible
      if (!ll.IsEmpty) { ll.Altitude = speedAlt.IsValid ? speedAlt.AsFeet : double.NaN; }
    }


  }
}
