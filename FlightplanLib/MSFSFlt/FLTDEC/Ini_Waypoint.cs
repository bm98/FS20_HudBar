using CoordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    /// Native ID of the Wyp (can have decorations - use Wyp_Ident instead)
    /// </summary>
    public string ID { get; internal set; } = "";
    /// <summary>
    /// The Waypoint name, can be an official one or an MSFS internal one
    /// </summary>
    public string Ident { get; internal set; } = "";
    /// <summary>
    /// The Waypoint name, can be an official one or an MSFS internal one
    /// </summary>
    public string Name { get; internal set; } = "";
    /// <summary>
    /// The Waypoint name (max 8 chars), can be an official one or an MSFS internal one
    /// </summary>
    public string Name7 =>
      Name.Length > 7 ? Name.Substring( 0, 7 ) : Name;

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
      (WaypointType == TypeOfWaypoint.Airport || WaypointType == TypeOfWaypoint.Runway)
        ? Altitude_ft
        : (float)(Math.Round( Altitude_ft / 100.0 ) * 100.0);

    /// <summary>
    /// The Type of the Waypoint 
    /// </summary>
    public TypeOfWaypoint WaypointType { get; internal set; } = TypeOfWaypoint.Other;

    /// <summary>
    /// The Waypoint Index starting at 0
    /// </summary>
    public int Index { get; internal set; } = -1;
    /// <summary>
    /// The leg distance to the next WP
    /// (sum of leg distances)
    /// </summary>
    public float LegDist_nm { get; internal set; } = 0;
    /// <summary>
    /// Heading to the next WP
    /// </summary>
    public float HeadingTo_deg { get; internal set; } = 0;
    /// <summary>
    /// The remaining distance to the destination
    /// (sum of leg distances)
    /// </summary>
    public float RemainingDist_nm { get; internal set; } = 0;
    /// <summary>
    /// The given Airway ID
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
    /// The given Approach
    /// </summary>
    public string ApproachType { get; internal set; } = "";
    /// <summary>
    /// The given Approach Suffix
    /// </summary>
    public string Approach_Suffix { get; internal set; } = ""; // empty or X,Y,Z etc. from approach ILS Y 22

    /// <summary>
    /// True if the Wyp as DECO information
    /// </summary>
    public bool IsDecorated => !string.IsNullOrWhiteSpace( Wyp_Deco );

    /// <summary>
    /// Clean ID of the Waypoint - removed all known decorations
    /// </summary>
    public string Wyp_Ident => Formatter.CleanB21SoaringName( ID );
    /// <summary>
    /// Get the Decoration for soaring waypoints
    /// </summary>
    public string Wyp_Deco => Formatter.GetB21SoaringDecoration( ID );

    /// <summary>
    /// The given Runway
    /// </summary>
    public string RunwayNumber_S { get; internal set; } = "";
    /// <summary>
    /// runway designation RIGHT, LEFT, CENTER, ?? others ??
    /// </summary>
    public string RunwayDesignation { get; set; } = "";

    /// <summary>
    /// Returns a Runway ident like 22 or 12R etc.
    /// </summary>
    public string Runway_Ident => ToRunwayID( RunwayNumber_S, RunwayDesignation );

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
    /// True if the Wyp is part of an Airway
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Airway_Ident );
    /// <summary>
    /// True if the Wyp as APR information
    /// </summary>
    public bool IsAPR => !string.IsNullOrWhiteSpace( ApproachType );
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
    /// cTor: empty
    /// </summary>
    public Ini_Waypoint( ) { }

    /// <summary>
    /// cTor: from a waypoint string (FLT file string)
    /// </summary>
    /// <param name="wypString"></param>
    public static Ini_Waypoint GetWaypoint( string wypString )
    {
      return Formatter.DecodeWaypoint( wypString );

    }

    // tools
    private static string ToRunwayID( string rNum, string rDes )
    {
      if (string.IsNullOrWhiteSpace( rDes )) {
        return rNum;
      }
      else {
        return rNum + rDes.Substring( 0, 1 ); // convert from 11 LEFT to 11L
      }
    }

  }
}
