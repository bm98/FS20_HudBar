using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// An MSFS GPX ATCWaypoint Element
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
    /// <summary>
    /// The ATCWaypointType element
    /// </summary>
    [XmlElement( ElementName = "ATCWaypointType" )]
    public string WypType_S { get; set; } = ""; // Airport, User, Intersection, VOR, NDB, ATC, Runway ?? others ??

    /// <summary>
    /// The WorldPosition element
    /// </summary>
    [XmlElement( ElementName = "WorldPosition" )]
    public string CoordLLA { get; set; } = ""; // N29° 15' 18.00",E91° 45' 54.00",+023630.00

    /// <summary>
    /// The ICAO element
    /// </summary>
    [XmlElement( ElementName = "ICAO" )]
    public X_Icao IcaoRec { get; set; } = new X_Icao( ); // Waypoint ICAO (code and opt. Region and Apt if related)

    /// <summary>
    /// The DepartureFP element
    /// </summary>
    [XmlElement( ElementName = "DepartureFP", IsNullable = false )]
    public string SID_Ident { get; set; } = ""; // SID Name

    /// <summary>
    /// The RunwayNumberFP element
    /// </summary>
    [XmlElement( ElementName = "RunwayNumberFP", IsNullable = false )]
    public string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...

    /// <summary>
    /// The RunwayDesignatorFP element
    /// </summary>
    [XmlElement( ElementName = "RunwayDesignatorFP", IsNullable = false )]
    public string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    /// <summary>
    /// The ATCAirway element
    /// </summary>
    [XmlElement( ElementName = "ATCAirway", IsNullable = false )]
    public string Airway_Ident { get; set; } = "";

    /// <summary>
    /// The ArrivalFP element
    /// </summary>
    [XmlElement( ElementName = "ArrivalFP", IsNullable = false )]
    public string STAR_Ident { get; set; } = ""; // STAR name

    /// <summary>
    /// The ApproachTypeFP element
    /// </summary>
    [XmlElement( ElementName = "ApproachTypeFP", IsNullable = false )]
    public string ApproachTypeS { get; set; } = ""; // RNAV, ILS, LOCALIZER, ?? others ??

    /// <summary>
    /// The SpeedMaxFP element
    /// </summary>
    [XmlElement( ElementName = "SpeedMaxFP", IsNullable = false )]
    public string MaxSpeed_S { get; set; } = ""; // -1 or a kt speed number

    /// <summary>
    /// The SuffixFP element
    /// </summary>
    [XmlElement( ElementName = "SuffixFP", IsNullable = false )]
    public string Approach_Suffix { get; set; } = ""; // empty or X,Y,Z etc. from approach ILS 22 Y

    // Non XML

    /// <summary>
    /// True if valid 
    /// There are 'unknown' which derive from Navaids not in the MS database (outdated ones)
    /// also they are set to N90 W180 (but alt remains..)
    /// </summary>
    [XmlIgnore]
    public bool IsValid => !(ID == "unknown" || (LatLonElev_ft.Lat == 90.0 && LatLonElev_ft.Lon == -180.0));

    // true when a proc must be reported (proc appears in Runway and User Wyps as well, where it may cause issues...)
    [XmlIgnore]
    private bool ReportProc => !(WaypointType == WaypointTyp.RWY || WaypointType == WaypointTyp.USR);

    /// <summary>
    /// True if the Wyp is part of an Enroute
    /// </summary>
    [XmlIgnore]
    public bool IsAirway => ReportProc && !string.IsNullOrWhiteSpace( Airway_Ident );
    /// <summary>
    /// True if the Wyp as APR information
    /// </summary>
    [XmlIgnore]
    public bool IsAPR => ReportProc && !string.IsNullOrWhiteSpace( ApproachTypeS );
    /// <summary>
    /// True if the Wyp is part of SID
    /// </summary>
    [XmlIgnore]
    public bool IsSID => ReportProc && !string.IsNullOrWhiteSpace( SID_Ident );
    /// <summary>
    /// True if the Wyp is part of STAR
    /// </summary>
    [XmlIgnore]
    public bool IsSTAR => ReportProc && !string.IsNullOrWhiteSpace( STAR_Ident );
    /// <summary>
    /// True if the Wyp is part of SID or STAR
    /// </summary>
    [XmlIgnore]
    public bool IsSidOrStar => IsSID || IsSTAR;

    /// <summary>
    /// True if the Wyp as DECO information
    /// </summary>
    [XmlIgnore]
    public bool IsDecorated => !string.IsNullOrWhiteSpace( Wyp_Deco );

    /// <summary>
    /// Clean ID of the Waypoint - removed all known decorations
    /// </summary>
    [XmlIgnore]
    public string Wyp_Ident => Formatter.CleanB21SoaringName( ID );
    /// <summary>
    /// Get the Decoration for soaring waypoints
    /// </summary>
    [XmlIgnore]
    public string Wyp_Deco => Formatter.GetB21SoaringDecoration( ID );

    /// <summary>
    /// Returns a Runway ident like RW22 or RW12R etc. RW00 if not provided from the source
    /// </summary>
    [XmlIgnore]
    public string RunwayIdent => AsRwIdent( RunwayNumber_S, RunwayDesignation );


    /// <summary>
    /// APPROACH Proc Type if it applies
    /// </summary>
    [XmlIgnore]
    public string ApproachProcRef { get; set; } = "";
    /// <summary>
    /// APPROACH Proc Type if it applies
    /// </summary>
    [XmlIgnore]
    public string ApproachProc => ApproachProcRef.ProcOf( );
    /// <summary>
    /// APPROACH Suffix if it applies
    /// </summary>
    [XmlIgnore]
    public string ApproachSuffix => ApproachProcRef.SuffixOf( );

    /// <summary>
    /// Approach Waypoint Sequence (1..)
    /// </summary>
    [XmlIgnore]
    public int ApproachSequ { get; set; } = 0;


    /// <summary>
    /// The LatLonAlt coordinate of the Wyp (Alt in ft)
    /// </summary>
    [XmlIgnore]
    public LatLon LatLonElev_ft => Formatter.ToLatLon( CoordLLA );
    /// <summary>
    /// Returns a Coordinate Name for this item
    /// </summary>
    [XmlIgnore]
    public string CoordName => Dms.ToRouteCoord( LatLonElev_ft, "d" );

    /// <summary>
    /// The Latitude of the Wyp
    /// </summary>
    [XmlIgnore]
    public double Lat => LatLonElev_ft.Lat;
    /// <summary>
    /// The Longitude of the Wyp
    /// </summary>
    [XmlIgnore]
    public double Lon => LatLonElev_ft.Lon;
    /// <summary>
    /// The Altitude ft of the Wyp
    /// </summary>
    [XmlIgnore]
    public float Altitude_ft => (float)LatLonElev_ft.Altitude;

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    [XmlIgnore]
    public float AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? Altitude_ft
        : (float)(Math.Round( Altitude_ft / 100.0 ) * 100.0);


    /// <summary>
    /// Alt Lo Limit for Procedures
    /// </summary>
    [XmlIgnore]
    public int AltLo_ft { get; set; } = 0;
    /// <summary>
    /// Alt Hi Limit for Procedures
    /// </summary>
    [XmlIgnore]
    public int AltHi_ft { get; set; } = 0;
    /// <summary>
    /// Speed Limit for Procedures
    /// </summary>
    [XmlIgnore]
    public int SpeedLimit_kt { get; set; } = 0;

    /// <summary>
    /// Waypoint Usage derived from content
    /// </summary>
    [XmlIgnore]
    public UsageTyp UsageType {
      get {
        if (IsSID) return UsageTyp.SID;
        if (IsSTAR) return UsageTyp.STAR;
        if (IsAPR) return UsageTyp.APR;
        return UsageTyp.Unknown;
      }
    }

    /// <summary>
    /// The type of the Waypoint as enum
    /// </summary>
    [XmlIgnore]
    public WaypointTyp WaypointType => IsValid ? ToWaypointTyp( WypType_S ) : WaypointTyp.Unknown;


    // local only
    private WaypointTyp ToWaypointTyp( string fpType )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "AIRPORT": return WaypointTyp.APT;
        case "ATC": return WaypointTyp.ATC;
        case "INTERSECTION": return WaypointTyp.WYP;
        case "RUNWAY": return WaypointTyp.RWY;
        case "USER":
          if (Wyp_Ident.StartsWith( "RW" ) && !string.IsNullOrEmpty( RunwayNumber_S )) return WaypointTyp.RWY; // RWxy is tagged as User Waypoint ??!!
          return WaypointTyp.USR;
        case "VOR": return WaypointTyp.VOR;
        case "NDB": return WaypointTyp.NDB;
        default: return WaypointTyp.OTH;
      }
    }

   
  }
}
