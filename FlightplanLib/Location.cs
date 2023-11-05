using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;

using FSimFacilityIF;
using FSFData;


using static FSimFacilityIF.Extensions;

namespace FlightplanLib
{
  /// <summary>
  /// Type of this Location
  /// </summary>
  public enum LocationTyp
  {
    /// <summary>
    /// Unknown Loc
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Departure Airport 
    /// </summary>
    Origin,
    /// <summary>
    /// Arrival Airport
    /// </summary>
    Destination,
    /// <summary>
    /// Alternate Airport
    /// </summary>
    Alternate,
  }
  /// <summary>
  /// Generic Start/End Location, usually an Airport
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// </summary>
  public class Location
  {
    /// <summary>
    /// Type of this Location
    /// </summary>
    public LocationTyp LocationType { get; set; } = LocationTyp.Unknown;

    /// <summary>
    /// Get;Set: The ICAO indent of this location
    /// </summary>
    public IcaoRec Icao_Ident { get; internal set; } = new IcaoRec( );

    /// <summary>
    /// Optional: Get;Set: The IATA Ident of this location
    /// </summary>
    public string Iata_Ident { get; internal set; } = "";

    /// <summary>
    /// Optional: Get;Set: The Common Name of this location
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// Get;Set: The Lat, Lon, Elevation [ft] of this location
    /// </summary>
    public LatLon LatLonAlt_ft { get; internal set; } = LatLon.Empty;

    /// <summary>
    /// Get: The Magnetic Variation at this location
    /// </summary>
    public double MagVar_deg => LatLonAlt_ft.MagVarLookup_deg( );

    /// <summary>
    /// Optional: Get;Set: The Runway number as string
    /// </summary>
    internal string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...
    /// <summary>
    /// Optional: Get;Set: The Runway designation as string as provided by the plan
    /// e.g. R, L, C, RIGHT, LEFT, BOTH??
    /// </summary>
    internal string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    /// <summary>
    /// Get: Returns a Runway ident like RW22 or RW12R etc.
    /// </summary>
    public string Runway_Ident => AsRwIdent( RunwayNumber_S, RunwayDesignation );
    /// <summary>
    /// Coordinate of the Runway Start or empty
    /// </summary>
    public LatLon RunwayLatLonAlt_ft { get; internal set; } = LatLon.Empty;

    /// <summary>
    /// Optional: Get: The SID Proc Ident (PROC[.TRANSITION] GBAS3, FDESO3.KULSI)
    /// Default: empty
    /// </summary>
    public string SIDProcName => DotProc( _sidIdent, _sidTransitionIdent );
    /// <summary>
    /// Optional: Get: The STAR Proc Ident (PROC[.TRANSITION] GBAS3, FDESO3.KULSI)
    /// Default: empty
    /// </summary>
    public string STARProcName => DotProc( _starIdent, _starTransitionIdent );

    /// <summary>
    /// Optional: Get;Set: The Approach Nav Procedure used (ILS, RNAV,...)
    /// Default: None
    /// </summary>
    public string ApproachProc => _aprProcRef.ProcOf( );
    /// <summary>
    /// Optional: Get;Set: The Approach Nav Procedure Suffix (Y,Z..) a single character
    /// Default: empty
    /// </summary>
    public string ApproachSuffix => _aprProcRef.SuffixOf( );
    /// <summary>
    /// Optional: Get: The Approach ProcRef (ILS Z, VOR..)
    /// </summary>
    public string ApproachProcRef => AsProcRef( ApproachProc, ApproachSuffix );

    // Tools

    /// <summary>
    /// True if the Location is valid (has a valid LatLon)
    /// </summary>
    public bool IsValid => !LatLonAlt_ft.IsEmpty;

    private string DotProc( string proc, string transition )
    {
      if (string.IsNullOrEmpty( transition )) return proc;
      return proc + "." + transition;
    }

    /// <summary>
    /// preset the Airport when already collected
    /// </summary>
    /// <param name="airportDBItem">An IAirport obj</param>
    internal void SetAirportDBItem( IAirport airportDBItem ) => _airport = airportDBItem;

    // Procedure detection

    private string _sidIdent = "";
    private string _sidTransitionIdent = "";
    private string _starIdent = "";
    private string _starTransitionIdent = "";
    private string _aprProcRef = "";
    private string _aprTransitionIdent = "";
    private bool _preferRNAV = false;
    private IAirport _airport = null;

    /// <summary>
    /// Set a SID for this Location
    /// </summary>
    /// <param name="sid">A SID procedure name</param>
    /// <param name="transitionIdent">A SID transition or empty</param>
    public void SetSID( string sid, string transitionIdent )
    {
      _sidIdent = sid;
      _sidTransitionIdent = transitionIdent;
    }
    /// <summary>
    /// Set a SID for this Location 
    /// from SID.TRANSITION or SID
    /// </summary>
    /// <param name="sidDotted">A dotted SID procedure name </param>
    public void SetSID( string sidDotted )
    {
      string[] e = sidDotted.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
      if (e.Length > 0) _sidIdent = e[0];
      if (e.Length > 1) _sidTransitionIdent = e[1];
    }

    /// <summary>
    /// Set a STAR for this Location
    /// </summary>
    /// <param name="star">A STAR procedure name</param>
    /// <param name="transitionIdent">A SID transition or empty</param>
    public void SetSTAR( string star, string transitionIdent )
    {
      _starIdent = star;
      _starTransitionIdent = transitionIdent;
    }
    /// <summary>
    /// Set a STAR for this Location 
    /// from STAR.TRANSITION or STAR
    /// </summary>
    /// <param name="starDotted">A dotted STAR procedure name </param>
    public void SetSTAR( string starDotted )
    {
      string[] e = starDotted.Split( new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
      if (e.Length > 0) _starIdent = e[0];
      if (e.Length > 1) _starTransitionIdent = e[1];
    }

    /// <summary>
    /// Set an Approach ProcRef (ILS Z...) for this Location
    /// - will use the RunwayIdent 
    /// </summary>
    /// <param name="aprProcRef">An Approach ProcRef</param>
    /// <param name="transitionIdent">An Approach transition or empty</param>
    public void SetAPR( string aprProcRef, string transitionIdent )
    {
      _aprProcRef = aprProcRef;
      _aprTransitionIdent = transitionIdent;
    }

    /// <summary>
    /// Set an Approach ProcRef (ILS Z...) for this Location
    /// </summary>
    /// <param name="navTyp">An Approach Nav type</param>
    /// <param name="aprSuffix">A Approach Suffix</param>
    /// <param name="transitionIdent">An Approach transition or empty</param>
    public void SetAPR( NavTyp navTyp, string aprSuffix, string transitionIdent )
    {
      _aprProcRef = AsProcRef( navTyp, aprSuffix );
      _aprTransitionIdent = transitionIdent;
    }

    /// <summary>
    /// Return the expanded SID Waypoints to Transition or Common End Wyp
    /// if no SID is defined, it returns an empty list
    /// </summary>
    /// <returns>A list of Waypoints</returns>
    public WaypointList ExpandSID( )
    {
      var wypList = new WaypointList( );
      if (this.LocationType != LocationTyp.Origin) return wypList; // cannot, only Origin can provide a SID

      AquireFacilities( );
      if (_airport == null) return wypList; // need an airport, return an empty list

      if (string.IsNullOrEmpty( _sidIdent )) return wypList;

      // can expand a SID...
      var dbRunway = _airport.Runways.FirstOrDefault( r => r.Ident == Runway_Ident );
      if (dbRunway == null) return wypList; // need a runway, return an empty list

      // try find SID Objects in the Facility Database (defaults to null when not found)
      var dbProc = dbRunway.SIDs.FirstOrDefault( s => s.ProcRef == _sidIdent )
              ?? _airport.SIDs( ).FirstOrDefault( s => s.ProcRef == _sidIdent );
      if (dbProc == null) return wypList; // need a procedure, return an empty list
      _preferRNAV = dbProc.NavType == NavTyp.RNAV;

      // expand SID
      wypList.AddRange( Formatter.ExpandSID( dbProc, _sidTransitionIdent ) );

      return wypList;
    }

    /// <summary>
    /// Return the expanded STAR Waypoints from Transition or Common Entry to last STAR Wyp
    /// </summary>
    /// <param name="firstIdent">An initial STAR Waypoint Ident</param>
    /// <returns>A list of Waypoints</returns>
    public WaypointList ExpandSTAR( string firstIdent = "" )
    {
      var wypList = new WaypointList();
      if (this.LocationType != LocationTyp.Destination) return wypList; // cannot, only Destination can provide a STAR
      if (string.IsNullOrEmpty( _starIdent )) return wypList; // cannot without STAR ident

      AquireFacilities( );
      if (_airport == null) return wypList; // need an airport, return an empty list

      var dbRunway = _airport?.Runways.FirstOrDefault( r => r.Ident == Runway_Ident );
      if (dbRunway == null) return wypList; // need a runway, return an empty list

      // try find STAR Objects in the Facility Database (defaults to null when not found)
      var dbProc = dbRunway.STARs.FirstOrDefault( s => s.ProcRef == _starIdent )
              ?? _airport.STARs( ).FirstOrDefault( s => s.ProcRef == _starIdent );
      if (dbProc == null) return wypList; // need a procedure, return an empty list
      _preferRNAV = dbProc.NavType == NavTyp.RNAV;

      // expand STAR
      if (!string.IsNullOrEmpty( firstIdent )) {
        // if we get a firstIdent, then the transition is not set - so try to find it out...
        if (dbProc.HasTransitionIdent( firstIdent )) {
          SetSTAR( _starIdent, firstIdent );
        }
      }
      wypList.AddRange( Formatter.ExpandSTAR( dbProc, _starTransitionIdent ) );

      return wypList;
    }

    /// <summary>
    /// Return the expanded Approach Waypoints from first Approach Wyp to MAPR
    /// does include a Runway Waypoint, and an Airport WYP at the end
    /// </summary>
    /// <returns>A list of Waypoints</returns>
    public WaypointList ExpandAPR( )
    {
      var wypList = new WaypointList( );
      if (this.LocationType != LocationTyp.Destination) return wypList; // cannot, only Destination can provide an Approach
      if (string.IsNullOrEmpty( _aprProcRef )) return wypList; // cannot, APR is empty

      AquireFacilities( );
      if (_airport == null) return wypList; // cannot, return an empty list

      var dbRunway = _airport?.Runways.FirstOrDefault( r => r.Ident == Runway_Ident );
      if (dbRunway == null) {
        // add at least the Airport
        wypList.AddRange( Formatter.ExpandLocationRwApt( this, false, "" ) );
        wypList.Last( ).Distance_nm = 0; wypList.Last( ).InboundTrueTrk = 0; wypList.Last( ).OutboundTrueTrk = 0;
        return wypList;
      }

      // try find APR Objects in the Facility Database (defaults to null when not found)
      var dbProc = dbRunway.APRs.FirstOrDefault( s => s.ProcRef == _aprProcRef )
              ?? _airport.APRs( ).FirstOrDefault( s => s.ProcRef == _aprProcRef );
      if (dbProc == null) {
        // add at least the RW + Airport
        wypList.AddRange( Formatter.ExpandLocationRwApt( this, true, "" ) );
        wypList.Last( ).Distance_nm = 0; wypList.Last( ).InboundTrueTrk = 0; wypList.Last( ).OutboundTrueTrk = 0;
        return wypList;
      }

      wypList.AddRange( Formatter.ExpandAPR( dbProc, _aprTransitionIdent, _preferRNAV ) ); // will expand the RW Wyp before the MAP leg

      // Add Arr Airport WYP after MAPR without Calculations
      // Add RW + ArrAirport if the APR was empty
      wypList.AddRange( Formatter.ExpandLocationRwApt( this, wypList.Count == 0, _aprProcRef ) );
      wypList.Last( ).Distance_nm = 0; wypList.Last( ).InboundTrueTrk = 0; wypList.Last( ).OutboundTrueTrk = 0;

      return wypList;
    }


    // get the needed data from the DB
    private void AquireFacilities( )
    {
      if (_airport != null) {
        if (!string.IsNullOrEmpty( Icao_Ident.ICAO )) {
          _airport = DbLookup.GetAirport( Icao_Ident.ICAO, bm98_hbFolders.Folders.GenAptDBFile );
        }
      }
    }

    /// <summary>
    /// To release the collected facilities when no longer using Expand Procedures
    /// </summary>
    public void ReleaseFacilities( )
    {
      _airport = null;
    }

  }
}
