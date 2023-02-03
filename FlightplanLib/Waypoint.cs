using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CoordLib;
using CoordLib.Extensions;

namespace FlightplanLib
{
  /// <summary>
  /// The Type of the Waypoint
  /// </summary>
  public enum TypeOfWaypoint
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Airport = 0,
    Waypoint,
    VOR,
    NDB,
    User,
    ATC,
    Runway,
    Other,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Altitude Limits for a Waypoint
  /// </summary>
  public enum AltLimitType
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    NoLimit = 0,
    At,
    Above,
    Below,
    Between,
    Runway,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }


  /// <summary>
  /// Generic Waypoint
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// </summary>
  public class Waypoint
  {
    /// <summary>
    /// Original Waypoint ID from source
    /// </summary>
    public string ID { get; internal set; } = "";

    /// <summary>
    /// Clean ID of the Waypoint - removed all known decorations
    /// </summary>
    public string Wyp_Ident => CleanB21SoaringName( ID );
    /// <summary>
    /// Clean ID of the Waypoint - removed all known decorations
    /// shortened to 7 chars
    /// </summary>
    public string Wyp_Ident7 {
      get {
        var w = Wyp_Ident;
        if (w.Length > 7) return w.Substring( 0, 7 );
        return w;
      }
    }
    /// <summary>
    /// Get the Decoration for soaring waypoints
    /// </summary>
    public string Wyp_Deco => GetB21SoaringDecoration( ID );
    /// <summary>
    /// True if the Wyp as DECO information e.g. soaring Wyp 
    /// </summary>
    public bool IsDecorated => !string.IsNullOrWhiteSpace( Wyp_Deco );


    /// <summary>
    /// Optional:
    /// The ICAO ident of this Waypoint
    /// </summary>
    public IcaoRec Icao_Ident { get; internal set; } = new IcaoRec( );

    /// <summary>
    /// Optional:
    /// The common name of this Waypoint
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// Type of this Waypoint
    /// </summary>
    public TypeOfWaypoint WaypointType { get; internal set; } = TypeOfWaypoint.Other;

    /// <summary>
    /// The Lat, Lon, Elevation [ft] of this location
    /// </summary>
    public LatLon LatLonAlt_ft { get; internal set; } = LatLon.Empty;

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public float AltitudeRounded_ft =>
      (WaypointType == TypeOfWaypoint.Airport || WaypointType == TypeOfWaypoint.Runway)
        ? (float)LatLonAlt_ft.Altitude
        : (float)(Math.Round( LatLonAlt_ft.Altitude / 100.0 ) * 100.0);

    /// <summary>
    /// The Magnetic Variation at this location
    /// </summary>
    public double MagVar_deg => LatLonAlt_ft.MagVarLookup_deg( );

    /// <summary>
    /// Distance to this Waypoint
    /// </summary>
    public double Distance_nm { get; internal set; } = -1;
    /// <summary>
    /// Optional:
    /// An Airway id this Wyp belongs to
    /// </summary>
    public string Airway_Ident { get; internal set; } = "";
    /// <summary>
    /// Optional:
    /// An SID id this Wyp belongs to
    /// </summary>
    public string SID_Ident { get; internal set; } = "";
    /// <summary>
    /// Optional:
    /// An STAR id this Wyp belongs to
    /// </summary>
    public string STAR_Ident { get; internal set; } = "";

    /// <summary>
    /// Optional:
    /// An Approach Type
    /// </summary>
    public string ApproachType { get; internal set; } = ""; // RNAV, ILS, LOCALIZER, ?? others ??
    /// <summary>
    /// Optional:
    /// An Approach Suffix
    /// </summary>
    public string ApproachSuffix { get; internal set; } = ""; // empty or X,Y,Z etc. from approach ILS 22 Y
    /// <summary>
    /// Full Approach   ILS X RWY 22C  RNAV X RWY 22C
    /// </summary>
    public string Approach_Ident => $"{ApproachType} {ApproachSuffix} RWY {Runway_Ident}";


    /// <summary>
    /// Optional:
    /// The Runway number as string
    /// </summary>
    internal string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...
    /// <summary>
    /// Optional:
    /// The Runway designation as string as provided by the plan
    /// </summary>
    internal string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    /// <summary>
    /// Returns a Runway ident like 22 or 12R etc.
    /// </summary>
    public string Runway_Ident => FlightPlan.ToRunwayID( RunwayNumber_S, RunwayDesignation );


    /// <summary>
    /// True if the Wyp is part of SID
    /// </summary>
    public bool IsSID => !string.IsNullOrWhiteSpace( SID_Ident );
    /// <summary>
    /// True if the Wyp is part of an Airway
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Airway_Ident ) && !IsSIDorSTAR;
    /// <summary>
    /// True if the Wyp is part of STAR
    /// </summary>
    public bool IsSTAR => !string.IsNullOrWhiteSpace( STAR_Ident );
    /// <summary>
    /// True is this Wyp belongs to a SID OR to a STAR
    /// </summary>
    public bool IsSIDorSTAR => IsSID || IsSTAR;

    /// <summary>
    /// True if there is an Approach
    /// </summary>
    public bool IsAPR => !string.IsNullOrEmpty( ApproachType );



    /// <summary>
    /// Inbound True Track [deg] from last Wyp
    /// </summary>
    public int InboundTrueTrk { get; internal set; } = -1;
    /// <summary>
    /// Inbound Mag Track [degm] from last Wyp
    /// </summary>
    public int InboundMagTrk => (int)CoordLib.WMM.MagVarEx.MagFromTrueBearing( InboundTrueTrk, LatLonAlt_ft, true );

    /// <summary>
    /// Outbound True Track [deg] from last Wyp
    /// </summary>
    public int OutboundTrueTrk { get; internal set; } = -1;
    /// <summary>
    /// Outbound Mag Track [degm] from last Wyp
    /// </summary>
    public int OutboundMagTrk => (int)CoordLib.WMM.MagVarEx.MagFromTrueBearing( OutboundTrueTrk, LatLonAlt_ft, true );

    /// <summary>
    /// Optional:
    /// An associated frequency
    /// </summary>
    public string Frequency { get; internal set; } = "";

    /// <summary>
    /// Optional:
    /// A stage string (source dependent e.g. CLB, CRZ ...)
    /// </summary>
    public string Stage { get; internal set; } = "";


    // Tools
    /// <summary>
    /// The B21 Soaring engine uses tagged names for the Task Management
    ///  [*]Name+Elev[|MaxAlt[/MinAlt]][xRadius]
    ///  1st * - Start of Task
    ///  2nd * - End of Task
    ///  Name  - the WP name
    ///  +     - Separator
    ///  Elev  - Waypoint Elevation  [ft}
    ///  |MaxAlt - Max alt of the gate [ft}
    ///  /MinAlt - Min alt of the gate [ft}
    ///  xRadius - Radius of the gate [meters]
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    private static string CleanB21SoaringName( string b21name )
    {
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        if (match.Groups["name"].Success) {
          return match.Groups["name"].Value;
        }
      }
      // seems a regular name
      return b21name;
    }
    private readonly static Regex c_wpB21 =
      new Regex( @"^(?<start_end>\*)?(?<name>([^\+])*)(?<elevation>(\+|-)\d{1,5})(?<maxAlt>\|\d{1,5})?(?<minAlt>\/\d{1,5})?(?<radius>x\d{1,5})?" );

    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    private static string GetB21SoaringDecoration( string b21name )
    {
      // decoration see above
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        var deco = "";
        if (match.Groups["start_end"].Success) { deco += "* "; }
        if (match.Groups["elevation"].Success) { deco += $"{match.Groups["elevation"].Value} ft "; }
        if (match.Groups["minAlt"].Success) { deco += $"(min {match.Groups["minAlt"].Value}) "; }
        if (match.Groups["maxAlt"].Success) { deco += $"(max {match.Groups["maxAlt"].Value}) "; }
        if (match.Groups["radius"].Success) { deco += $"(rad {match.Groups["radius"].Value} m)"; }
        return deco;
      }
      // seems a regular name
      return "";
    }

  }
}
