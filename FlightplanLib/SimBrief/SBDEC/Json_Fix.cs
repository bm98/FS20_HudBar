using CoordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// NavLog Fix Record
  /// </summary>
  [DataContract]
  public class Json_Fix
  {
    [DataMember( Name = "ident", IsRequired = true )]
    public string Ident { get; set; } = ""; // ICAO, 'TOC', 'TOD'

    [DataMember( Name = "name", IsRequired = true )]
    public string Name { get; set; } = ""; // common name, 'TOP OF CLIMB', 'TOP OF DESCENT'

    [DataMember( Name = "type", IsRequired = true )]
    public string FixType { get; set; } = ""; // wpt, apt, vor, ndb, ltlg for TOC/TOD, ...???

    [DataMember( Name = "frequency", IsRequired = true )]
    public string Frequency { get; set; } = ""; // 352, 114.35 .. 

    [DataMember( Name = "pos_lat", IsRequired = true )]
    public string Latitude { get; set; } = ""; // dec degree e.g. 35.786228
    [DataMember( Name = "pos_long", IsRequired = true )]
    public string Longitude { get; set; } = ""; // dec degree e.g. 14.503772


    [DataMember( Name = "stage", IsRequired = false )]
    public string Stage { get; set; } = ""; // CLB, CRZ, DSC

    [DataMember( Name = "via_airway", IsRequired = false )]
    public string Via_Airway { get; set; } = "";

    [DataMember( Name = "is_sid_star", IsRequired = false )]
    public string SidOrStar { get; set; } = ""; // 0 or 1

    [DataMember( Name = "distance", IsRequired = false )]
    public string Distance { get; set; } = ""; // a number nm

    // Inbound Track and Heading
    [DataMember( Name = "track_true", IsRequired = false )]
    public string TrueTRK { get; set; } = ""; // a number deg
    [DataMember( Name = "track_mag", IsRequired = false )]
    public string MagTRK { get; set; } = ""; // a number degm

    [DataMember( Name = "heading_true", IsRequired = false )]
    public string TrueHDG { get; set; } = ""; // a number deg
    [DataMember( Name = "heading_mag", IsRequired = false )]
    public string MagHDG { get; set; } = ""; // a number degm

    [DataMember( Name = "altitude_feet", IsRequired = false )]
    public string Altitude { get; set; } = ""; // a number ft


    // Non JSON

    // converted datatypes
    public LatLon LatLon => new LatLon( Lat, Lon, Altitude_ft );
    public double Lat => Formatter.GetValue( Latitude );
    public double Lon => Formatter.GetValue( Longitude );

    /// <summary>
    /// True if the Wyp is part of an Airway
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Via_Airway ) && (!IsSidOrStar);
    public bool IsSidOrStar => SidOrStar == "1";

    public float Distance_nm => (float)Formatter.GetValue( Distance );
    public int TRKt_deg => (int)Formatter.GetValue( TrueTRK );
    public int TRK_degm => (int)Formatter.GetValue( MagTRK );
    public int HDGt_deg => (int)Formatter.GetValue( TrueHDG );
    public int HDG_degm => (int)Formatter.GetValue( MagHDG );
    public float Altitude_ft => (float)Formatter.GetValue( Altitude );
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
    public TypeOfWaypoint WaypointType => ToTypeOfWP( FixType );

    // local only
    private static TypeOfWaypoint ToTypeOfWP( string fpType )
    {
      switch (fpType.ToLowerInvariant( )) {
        case "apt": return TypeOfWaypoint.Airport;
        case "wpt": return TypeOfWaypoint.Waypoint;
        case "ndb": return TypeOfWaypoint.NDB;
        case "rwy": return TypeOfWaypoint.Runway; // ?? exists ??
        case "usr": return TypeOfWaypoint.User; // ?? exists ??
        case "vor": return TypeOfWaypoint.VOR;
        default: return TypeOfWaypoint.Other; // TOC,TOD goes here
      }
    }

  }
}
