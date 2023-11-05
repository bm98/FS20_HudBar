using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// One Waypoint from the Ini file
  /// </summary>
  public class Ini_Waypoint
  {
    /// <summary>
    /// True if it is a valid Waypoint
    /// </summary>
    public bool IsValid { get; internal set; } = false;

    /// <summary>
    /// The Type of the Waypoint 
    /// </summary>
    public WaypointTyp WaypointType { get; internal set; } = WaypointTyp.Unknown;

    /// <summary>
    /// Waypoint Usage derived from content
    /// </summary>
    public UsageTyp UsageType {
      get {
        if (IsSID) return UsageTyp.SID;
        if (IsSTAR) return UsageTyp.STAR;
        if (HasAPR) return UsageTyp.APR;
        return UsageTyp.Unknown;
      }
    }

    /// <summary>
    /// Native Ident of the Wyp (can have decorations or is Empty - use Ident instead)
    /// </summary>
    public string SourceIdent { get; internal set; } = "";
    /// <summary>
    /// Cleaned Ident of the Waypoint - removed all known decorations
    /// If the Ident is not provided by the FLT file it returns the Name7 Field as Ident
    /// </summary>
    public string Ident => string.IsNullOrEmpty( SourceIdent ) ? Name7 : Formatter.CleanB21SoaringName( SourceIdent ).ToUpperInvariant( );
    /// <summary>
    /// Get the Decoration for soaring waypoints
    /// </summary>
    public string Decoration => Formatter.GetB21SoaringDecoration( SourceIdent );
    /// <summary>
    /// True if the Wyp as DECO information
    /// </summary>
    public bool IsDecorated => !string.IsNullOrWhiteSpace( Decoration );

    /// <summary>
    /// The Waypoint desctriptive name, can be an official one or an MSFS internal one
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// The Waypoint name (max 7 chars UCase), can be an official one or an MSFS internal one
    /// </summary>
    public string Name7 => Name.Length > 7 ? Name.Substring( 0, 7 ).ToUpperInvariant( ) : Name.ToUpperInvariant( );

    /// <summary>
    /// The ICAO Region (where given, else it is an empty string)
    /// </summary>
    public string Region { get; internal set; } = "";

    /// <summary>
    /// The Latitude, Longitude, Altitude ft of the Waypoint (NaN if not given or decoded)
    /// </summary>
    public LatLon LatLon { get; internal set; } = LatLon.Empty;
    /// <summary>
    /// The Latitude of the Waypoint (NaN if not given or decoded)
    /// </summary>
    public double Lat => LatLon.Lat;
    /// <summary>
    /// The Longitude of the Waypoint (NaN if not given or decoded)
    /// </summary>
    public double Lon => LatLon.Lon;
    /// <summary>
    /// The Altitude ft of the Waypoint (NaN if not given or decoded)
    /// </summary>
    public float Altitude_ft => (float)LatLon.Altitude;

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public float AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? Altitude_ft
        : (float)(Math.Round( Altitude_ft / 100.0 ) * 100.0);

    /// <summary>
    /// The given Enroute ID
    /// </summary>
    public string Airway_Ident { get; internal set; } = "";
    /// <summary>
    /// The given Departure ID
    /// </summary>
    public string SID_Ident { get; internal set; } = "";
    /// <summary>
    /// The given ArrivalID
    /// </summary>
    public string STAR_Ident { get; internal set; } = "";

    /// <summary>
    /// The given Approach ProcRef
    /// </summary>
    public string ApproachProcRef => string.IsNullOrWhiteSpace( ApproachType ) ? "" : AsProcRef( ApproachType, ApproachSuffix ); // from approach ILS Y ..
    /// <summary>
    /// The given Approach Type (nav type ILS, RNAV,..)
    /// </summary>
    public string ApproachType { get; internal set; } = "";
    /// <summary> 
    /// The given Approach Suffix (X,Y or empty)
    /// </summary>
    public string ApproachSuffix { get; internal set; } = ""; // empty or X,Y,Z etc. from approach ILS Y 22

    /// <summary>
    /// Returns a Runway Ident like RW22 or RW12R etc.
    /// </summary>
    public string RwIdent => AsRwIdent( RwNumber_S, RwDesignation );
    /// <summary>
    /// The given Runway
    /// </summary>
    public string RwNumber_S { get; internal set; } = "";
    /// <summary>
    /// runway designation RIGHT, LEFT, CENTER, ?? others ??
    /// </summary>
    public string RwDesignation { get; set; } = "";


    /// <summary>
    /// The given Airport for the Runway
    /// </summary>
    public string Airport { get; internal set; } = "";

    /// <summary>
    /// A Speed Limit if given (else it is -1)
    /// </summary>
    public int SpeedLimit_kt { get; internal set; } = -1;

    /// <summary>
    /// The Altitude Limit type
    /// </summary>
    public AltLimitType AltLimit { get; internal set; } = AltLimitType.NoLimit;
    /// <summary>
    /// A first Alt Limit for +;- or B Top
    /// </summary>
    public int AltLimit1_ft { get; internal set; } = 0;
    /// <summary>
    /// A second Alt Limit for B Bottom
    /// </summary>
    public int AltLimit2_ft { get; internal set; } = 0;

    /// <summary>
    /// True if the Wyp is part of an Enroute
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Airway_Ident );
    /// <summary>
    /// True if the Wyp as APR information
    /// </summary>
    public bool HasAPR => !string.IsNullOrWhiteSpace( ApproachType );
    /// <summary>
    /// True if the Wyp is part of SID
    /// </summary>
    public bool IsSID => !string.IsNullOrWhiteSpace( SID_Ident );
    /// <summary>
    /// True if the Wyp is part of STAR
    /// </summary>
    public bool IsSTAR => !string.IsNullOrWhiteSpace( STAR_Ident );
    /// <summary>
    /// True if the Wyp is part of SID or STAR
    /// </summary>
    public bool IsSidOrStar => IsSID || IsSTAR;
    /// <summary>
    /// True if the Wyp is part of SID or STAR or APR
    /// </summary>
    public bool IsProc => IsSID || IsSTAR || HasAPR;


    /// <summary>
    /// cTor: empty
    /// </summary>
    public Ini_Waypoint( ) { }

    /// <summary>
    /// cTor: from a waypoint string (FLT file string)
    /// </summary>
    /// <param name="wypString"></param>
    public static Ini_Waypoint GetWaypoint( string wypString )
    {
      return Ini_Formatter.DecodeWaypoint( wypString );

    }

    // tools

  }
}
