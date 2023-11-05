using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;
using CoordLib.Extensions;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib
{

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
  public class Waypoint : IEquatable<Waypoint>
  {
    /// <summary>
    /// Get;Set: Type of this Waypoint
    /// </summary>
    public WaypointTyp WaypointType { get; internal set; } = WaypointTyp.Unknown;

    /// <summary>
    /// Get;Set: Usage of this Waypoint
    /// </summary>
    public UsageTyp WaypointUsage { get; internal set; } = UsageTyp.Unknown;

    /// <summary>
    /// Get;Set: Original Waypoint Ident from source
    /// </summary>
    public string SourceIdent { get; internal set; } = "";

    /// <summary>
    /// Get: Clean ID of the Waypoint - removed all known decorations
    /// </summary>
    public string Ident => Formatter.CleanB21SoaringName( SourceIdent );
    /// <summary>
    /// Get: Clean ID of the Waypoint - removed all known decorations
    /// shortened to 7 chars max
    /// </summary>
    public string Ident7 {
      get {
        var w = Ident;
        if (w.Length > 7) return w.Substring( 0, 7 );
        return w;
      }
    }
    /// <summary>
    /// Get: The Decoration for soaring waypoints
    /// </summary>
    public string IdentDecoration => Formatter.GetB21SoaringDecoration( SourceIdent );
    /// <summary>
    /// Get: True if the Wyp as DECO information e.g. soaring Wyp 
    /// </summary>
    public bool IsDecorated => !string.IsNullOrWhiteSpace( IdentDecoration );

    /// <summary>
    /// Optional: Get;Set: The ICAO ident of this Waypoint
    /// </summary>
    public IcaoRec Icao_Ident { get; internal set; } = new IcaoRec( );

    /// <summary>
    /// Optional: Get;Set: The common name of this Waypoint
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// The Lat, Lon, Target Altitude [ft] of this Waypoint
    /// </summary>
    public LatLon LatLonAlt_ft { get => _LatLonAlt_ft; internal set => _LatLonAlt_ft = value; }
    private LatLon _LatLonAlt_ft = LatLon.Empty;
    /// <summary>
    /// Returns a Coordinate Name for this item
    /// </summary>
    public string CoordName => Dms.ToRouteCoord( LatLonAlt_ft, "d" );
    /// <summary>
    /// Get: The Magnetic Variation at this location
    /// </summary>
    public double MagVar_deg => LatLonAlt_ft.MagVarLookup_deg( );

    /// <summary>
    /// Set a new Target Altitude for this Waypoint
    /// </summary>
    /// <param name="feet">Alt in feet</param>
    public void SetAltitude_ft( double feet ) => _LatLonAlt_ft.SetAltitude( feet );

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public int AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? dNetBm98.XMath.AsRoundInt( LatLonAlt_ft.Altitude, 1 )
        : dNetBm98.XMath.AsRoundInt( LatLonAlt_ft.Altitude, 100 );

    /// <summary>
    /// Get;Set: Lo Altitude Limit for Wyps 
    /// Use negative or 0 to omit 
    /// </summary>
    public int AltitudeLo_ft { get; internal set; } = 0;
    /// <summary>
    /// Get;Set: Hi Altitude Limit for Wyps 
    /// Use negative or 0 to omit 
    /// </summary>
    public int AltitudeHi_ft { get; internal set; } = 0;
    /// <summary>
    /// Speed Limit for Procedures
    /// </summary>
    public int SpeedLimit_kt { get; set; } = 0;

    /// <summary>
    /// Optional: Get;Set: An Airway ident this Wyp belongs to
    /// Empty when not set
    /// </summary>
    public string Airway_Ident { get; internal set; } = "";
    /// <summary>
    /// Optional: Get;Set: A SID ident this Wyp belongs to
    /// Empty when not set
    /// </summary>
    public string SID_Ident { get; internal set; } = "";
    /// <summary>
    /// Optional: Get;Set: A STAR ident this Wyp belongs to
    /// Empty when not set
    /// </summary>
    public string STAR_Ident { get; internal set; } = "";
    /// <summary>
    /// Optional: Get;Set: An Approach Type String (RNAV, ILS etc as derived from the source)
    /// Empty when not set
    /// </summary>
    public string ApproachTypeS { get; internal set; } = ""; // RNAV, ILS, LOCALIZER, ?? others ??
    /// <summary>
    /// Optional: Get;Set: An Approach Suffix
    /// Empty when not set
    /// </summary>
    public string ApproachSuffix { get; internal set; } = ""; // empty or X,Y,Z etc. from approach ILS 22 Y
    /// <summary>
    /// Approach Reference 'ILS X'  'RNAV X' 'VOR'
    /// </summary>
    public string ApproachProcRef => AsProcRef( ApproachTypeS, ApproachSuffix );
    /// <summary>
    /// Approach full name  'ILS X RWY 22C'  'RNAV X RW22C' 'VOR RW01'
    /// </summary>
    public string ApproachName => $"{ApproachProcRef} {RunwayIdent}";
    /// <summary>
    /// The Waypoint Number of the Approach 
    /// Starts with 1...
    /// </summary>
    public int ApproachSequence { get; internal set; } = 0;


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
    public string RunwayIdent => AsRwIdent( RunwayNumber_S, RunwayDesignation );


    /// <summary>
    /// Get: True if the Wyp is part of an Enroute
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Airway_Ident ) && !IsSIDorSTAR;
    /// <summary>
    /// Get: True if the Wyp is part of SID
    /// </summary>
    public bool IsSID => !string.IsNullOrWhiteSpace( SID_Ident );
    /// <summary>
    /// Get: True if the Wyp is part of STAR
    /// </summary>
    public bool IsSTAR => !string.IsNullOrWhiteSpace( STAR_Ident );
    /// <summary>
    /// Get: True is this Wyp belongs to a SID OR to a STAR
    /// </summary>
    public bool IsSIDorSTAR => IsSID || IsSTAR;
    /// <summary>
    /// Get: True if there is an Approach
    /// </summary>
    public bool IsAPR => !string.IsNullOrEmpty( ApproachTypeS );
    /// <summary>
    /// Get: True is this Wyp belongs to a SID OR to a STAR OR to an Approach
    /// </summary>
    public bool IsProc => IsSID || IsSTAR || IsAPR;

    /// <summary>
    /// Get;Set: Distance TO this Waypoint
    /// Use negative when not known
    /// </summary>
    public double Distance_nm { get; internal set; } = -1;

    /// <summary>
    /// Get;Set: Inbound True Track [deg] from last Wyp (-1 if not set)
    /// </summary>
    public int InboundTrueTrk { get; internal set; } = -1;
    /// <summary>
    /// Get: Inbound Mag Track [degm] from last Wyp
    /// </summary>
    public int InboundMagTrk => (InboundTrueTrk < 0) ? -1
                              : (int)CoordLib.WMM.MagVarEx.MagFromTrueBearing( InboundTrueTrk, LatLonAlt_ft, true );

    /// <summary>
    /// Get; Set: Outbound True Track [deg] from last Wyp (-1 if not set)
    /// </summary>
    public int OutboundTrueTrk { get; internal set; } = -1;
    /// <summary>
    /// Get: Outbound Mag Track [degm] from last Wyp
    /// </summary>
    public int OutboundMagTrk => (OutboundTrueTrk < 0) ? -1
                              : (int)CoordLib.WMM.MagVarEx.MagFromTrueBearing( OutboundTrueTrk, LatLonAlt_ft, true );

    /// <summary>
    /// Optional: Get;Set: An associated frequency as string
    /// </summary>
    public string Frequency { get; internal set; } = "";

    /// <summary>
    /// Optional: Get;Set: A stage string (source dependent e.g. CLB, CRZ ...)
    /// </summary>
    public string Stage { get; internal set; } = "";


    /// <summary>
    /// Equatable Implementation
    /// </summary>
    public bool Equals( Waypoint other ) => Equals( this, other );

    /// <summary>
    /// Returns true when equal
    /// </summary>
    public static bool Equals( Waypoint x, Waypoint y )
    {
      //Check whether the compared objects reference the same data.
      if (Object.ReferenceEquals( x, y )) return true;
      //Check whether any of the compared objects is null.
      if (x is null || y is null) return false;

      return
        x.Ident.Equals( y.Ident )
        && x.WaypointType.Equals( y.WaypointType );
    }


  }
}
