using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using FSimFacilityIF;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// NavLog Fix Record
  /// </summary>
  [DataContract]
  public class Json_Fix
  {
    /// <summary>
    /// The ident field
    /// </summary>
    [DataMember( Name = "ident", IsRequired = true )]
    public string Ident { get; set; } = ""; // ICAO, 'TOC', 'TOD'

    /// <summary>
    /// The name field
    /// </summary>
    [DataMember( Name = "name", IsRequired = true )]
    public string Name { get; set; } = ""; // common name, 'TOP OF CLIMB', 'TOP OF DESCENT'

    /// <summary>
    /// The type field
    /// </summary>
    [DataMember( Name = "type", IsRequired = true )]
    public string FixType { get; set; } = ""; // wpt, apt, vor, ndb, ltlg for TOC/TOD, ...???

    /// <summary>
    /// The frequency field
    /// </summary>
    [DataMember( Name = "frequency", IsRequired = true )]
    public string Frequency { get; set; } = ""; // 352, 114.35 .. 

    /// <summary>
    /// The pos_lat field
    /// </summary>
    [DataMember( Name = "pos_lat", IsRequired = true )]
    public string Latitude { get; set; } = ""; // dec degree e.g. 35.786228
    /// <summary>
    /// The pos_long field
    /// </summary>
    [DataMember( Name = "pos_long", IsRequired = true )]
    public string Longitude { get; set; } = ""; // dec degree e.g. 14.503772


    /// <summary>
    /// The stage field
    /// </summary>
    [DataMember( Name = "stage", IsRequired = false )]
    public string Stage { get; set; } = ""; // CLB, CRZ, DSC

    /// <summary>
    /// The via_airway field
    /// </summary>
    [DataMember( Name = "via_airway", IsRequired = false )]
    public string Via_Airway { get; set; } = "";

    /// <summary>
    /// The is_sid_star field
    /// </summary>
    [DataMember( Name = "is_sid_star", IsRequired = false )]
    public string SidOrStar { get; set; } = ""; // 0 or 1

    /// <summary>
    /// The distance field
    /// </summary>
    [DataMember( Name = "distance", IsRequired = false )]
    public string Distance { get; set; } = ""; // a number nm

    // Inbound Track and Heading
    /// <summary>
    /// The track_true field
    /// </summary>
    [DataMember( Name = "track_true", IsRequired = false )]
    public string TrueTRK { get; set; } = ""; // a number deg
    /// <summary>
    /// The track_mag field
    /// </summary>
    [DataMember( Name = "track_mag", IsRequired = false )]
    public string MagTRK { get; set; } = ""; // a number degm

    /// <summary>
    /// The heading_true field
    /// </summary>
    [DataMember( Name = "heading_true", IsRequired = false )]
    public string TrueHDG { get; set; } = ""; // a number deg
    /// <summary>
    /// The heading_mag field
    /// </summary>
    [DataMember( Name = "heading_mag", IsRequired = false )]
    public string MagHDG { get; set; } = ""; // a number degm

    /// <summary>
    /// The altitude_feet field
    /// </summary>
    [DataMember( Name = "altitude_feet", IsRequired = false )]
    public string Altitude { get; set; } = ""; // a number ft


    // Non JSON

    // converted datatypes
    /// <summary>
    /// The LatLonAlt coordinate of the Wyp (Alt in ft)
    /// </summary>
    public LatLon LatLon => new LatLon( Lat, Lon, Altitude_ft );
    /// <summary>
    /// The Latitude of the Wyp
    /// </summary>
    public double Lat => Formatter.GetValue( Latitude );
    /// <summary>
    /// The Longitude of the Wyp
    /// </summary>
    public double Lon => Formatter.GetValue( Longitude );
    /// <summary>
    /// The Altitude ft of the Wyp
    /// </summary>
    public float Altitude_ft => (float)Formatter.GetValue( Altitude );

    /// <summary>
    /// True if the Wyp is part of an Enroute
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Via_Airway ) && (!IsSidOrStar);
    /// <summary>
    /// True if the sidOrStar field is set
    /// </summary>
    public bool IsSidOrStar => SidOrStar == "1";
    /// <summary>
    /// Distance nm as number (derived field)
    /// </summary>
    public float Distance_nm => (float)Formatter.GetValue( Distance );
    /// <summary>
    /// True TRK as number (derived field)
    /// </summary>
    public int TRKt_deg => (int)Formatter.GetValue( TrueTRK );
    /// <summary>
    /// Mag TRK as number (derived field)
    /// </summary>
    public int TRK_degm => (int)Formatter.GetValue( MagTRK );
    /// <summary>
    /// True HDG as number (derived field)
    /// </summary>
    public int HDGt_deg => (int)Formatter.GetValue( TrueHDG );
    /// <summary>
    /// Mag HDG as number (derived field)
    /// </summary>
    public int HDG_degm => (int)Formatter.GetValue( MagHDG );
    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public float AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? Altitude_ft
        : (float)(Math.Round( Altitude_ft / 100.0 ) * 100.0);

    /// <summary>
    /// The type of the Waypoint as enum
    /// </summary>
    public WaypointTyp WaypointType => ToWaypointTyp( FixType );

    // local only
    private static WaypointTyp ToWaypointTyp( string fpType )
    {
      switch (fpType.ToLowerInvariant( )) {
        case "apt": return WaypointTyp.APT;
        case "wpt": return WaypointTyp.WYP;
        case "ndb": return WaypointTyp.NDB;
        case "rwy": return WaypointTyp.RWY; // ?? exists ??
        case "usr": return WaypointTyp.USR; // ?? exists ??
        case "vor": return WaypointTyp.VOR;
        default: return WaypointTyp.OTH; // TOC,TOD goes here
      }
    }

  }
}
