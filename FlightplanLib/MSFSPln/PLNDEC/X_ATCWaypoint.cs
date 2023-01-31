using CoordLib;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// An MSFS PLN ATCWaypoint Element
  /// </summary>
  [XmlRootAttribute( "ATCWaypoint", Namespace = "", IsNullable = false )]
  public class X_ATCWaypoint
  {
    // Attributes
    /// <summary>
    /// Native ID of the Wyp (can have decorations - use Wyp_Ident instead)
    /// </summary>
    [XmlAttribute( AttributeName = "id" )]
    public string ID { get; set; } // the Waypoint ID (usually an ICAO code, can be User_WP, WPn or whatever, also decoration from B21 soaring planner)


    // Elements
    [XmlElement( ElementName = "ATCWaypointType" )]
    public string WypType_S { get; set; } = ""; // Airport, User, Intersection, VOR, NDB, ATC, Runway ?? others ??

    [XmlElement( ElementName = "WorldPosition" )]
    public string CoordLLA { get; set; } = ""; // N29° 15' 18.00",E91° 45' 54.00",+023630.00

    [XmlElement( ElementName = "ICAO" )]
    public X_Icao IcaoRec { get; set; } = new X_Icao( ); // Waypoint ICAO (code and opt. Region and Apt if related)

    [XmlElement( ElementName = "DepartureFP", IsNullable = false )]
    public string SID_Ident { get; set; } = ""; // SID Name

    [XmlElement( ElementName = "RunwayNumberFP", IsNullable = false )]
    public string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...

    [XmlElement( ElementName = "RunwayDesignatorFP", IsNullable = false )]
    public string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    [XmlElement( ElementName = "ATCAirway", IsNullable = false )]
    public string Airway_Ident { get; set; } = "";

    [XmlElement( ElementName = "ArrivalFP", IsNullable = false )]
    public string STAR_Ident { get; set; } = ""; // STAR name

    [XmlElement( ElementName = "ApproachTypeFP", IsNullable = false )]
    public string ApproachType { get; set; } = ""; // RNAV, ILS, LOCALIZER, ?? others ??

    [XmlElement( ElementName = "SpeedMaxFP", IsNullable = false )]
    public string MaxSpeed_S { get; set; } = ""; // -1 or a kt speed number

    [XmlElement( ElementName = "SuffixFP", IsNullable = false )]
    public string Approach_Suffix { get; set; } = ""; // empty or X,Y,Z etc. from approach ILS 22 Y

    // Non XML

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
    /// Returns a Runway ident like 22 or 12R etc.
    /// </summary>
    public string Runway_Ident => ToRunwayID( RunwayNumber_S, RunwayDesignation );

    /// <summary>
    /// Full Approach   ILS X RWY 22C  RNAV X RWY 22C
    /// </summary>
    public string Approach_Ident => $"{ApproachType} {Approach_Suffix} RWY {Runway_Ident}";
    /// <summary>
    /// The LatLonAlt coordinate of the Wyp (Alt in ft)
    /// </summary>
    public LatLon LatLon => Formatter.ToLatLon( CoordLLA );
    public double Lat => LatLon.Lat;
    public double Lon => LatLon.Lon;
    public float Altitude_ft => (float)LatLon.Altitude;

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public float AltitudeRounded_ft =>
      (WaypointType == TypeOfWaypoint.Airport || WaypointType == TypeOfWaypoint.Runway)
        ? Altitude_ft
        : (float)(Math.Round( Altitude_ft / 100.0 ) * 100.0);

    /// <summary>
    /// The type of the Waypoint as enum
    /// </summary>
    public TypeOfWaypoint WaypointType => ToTypeOfWP( WypType_S );


    // local only
    private static TypeOfWaypoint ToTypeOfWP( string fpType )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "AIRPORT": return TypeOfWaypoint.Airport;
        case "ATC": return TypeOfWaypoint.ATC;
        case "INTERSECTION": return TypeOfWaypoint.Waypoint;
        case "NDB": return TypeOfWaypoint.NDB;
        case "RUNWAY": return TypeOfWaypoint.Runway;
        case "USER": return TypeOfWaypoint.User;
        case "VOR": return TypeOfWaypoint.VOR;
        default: return TypeOfWaypoint.Other;
      }
    }

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
