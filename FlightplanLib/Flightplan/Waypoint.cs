using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

using dNetBm98;

using CoordLib;
using CoordLib.Extensions;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;

using static FlightplanLib.Formatter;

namespace FlightplanLib.Flightplan
{

  /// <summary>
  /// Altitude Limits for a Waypoint
  ///  AltitudeLimit used first, for Between use also AltitudeHi
  /// </summary>
  public enum AltLimitType
  {
    /// <summary>
    /// No altitude limit applies
    /// </summary>
    NoLimit = 0,
    /// <summary>
    /// AT AltitudeLimit
    /// </summary>
    At,
    /// <summary>
    /// ABOVE AltitudeLimit
    /// </summary>
    Above,
    /// <summary>
    /// BELOW AltitudeLimit
    /// </summary>
    Below,
    /// <summary>
    /// BETWEEN AltitudeLimit and AltitudeHi
    /// </summary>
    Between,
    /// <summary>
    /// for WYP=RUNWAY; AltitudeLimit is the Runway elevation
    /// </summary>
    Runway,

  }


  /// <summary>
  /// The Type of Route Point
  /// </summary>
  public enum RoutePointType
  {
    /// <summary>
    /// An Empty (Invalid) RoutePoint
    /// </summary>
    Empty = 0,

    /// <summary>
    /// A Waypoint
    /// </summary>
    Wyp,
    /// <summary>
    /// A VOR
    /// </summary>
    Vor,
    /// <summary>
    /// An NDB
    /// </summary>
    Ndb,

    /// <summary>
    /// Top of Climb or Descend mark or other user entered waypoint
    /// </summary>
    User,

    /// <summary>
    /// An Airport or Runway (start)
    /// </summary>
    AptDeparture,
    /// <summary>
    /// An Airport or Runway (landing)
    /// </summary>
    AptArrival,

    /// <summary>
    /// Not one of the above
    /// </summary>
    Other,
  }

  /// <summary>
  /// Generic Waypoint, serves also as RoutePoint 
  /// 
  ///   Assigning data is limited to class internal methods
  ///   
  /// </summary>
  public class Waypoint : IEquatable<Waypoint>
  {
    /// <summary>
    /// Empty Waypoint (not valid...)
    /// </summary>
    public static Waypoint Empty = new Waypoint( );

    /// <summary>
    /// Translate from Facility WaypointTyp to internal RoutePointType
    /// </summary>
    /// <param name="waypointTyp">A WaypointTyp</param>
    /// <param name="onDeparture">Departure flag</param>
    /// <returns>A RoutePointType</returns>
    private static RoutePointType FromWaypointTyp( WaypointTyp waypointTyp, bool onDeparture )
    {
      switch (waypointTyp) {
        case WaypointTyp.Unknown: return RoutePointType.Empty; // invalid
        case WaypointTyp.WYP: return RoutePointType.Wyp;
        case WaypointTyp.VOR: return RoutePointType.Vor;
        case WaypointTyp.NDB: return RoutePointType.Ndb;
        case WaypointTyp.APT: return onDeparture ? RoutePointType.AptDeparture : RoutePointType.AptArrival;  // treat as Apt
        case WaypointTyp.RWY: return onDeparture ? RoutePointType.AptDeparture : RoutePointType.AptArrival;  // treat as Apt
        case WaypointTyp.USR: return RoutePointType.User;
        case WaypointTyp.ATC:
        case WaypointTyp.LOC:  // localizer
        case WaypointTyp.COR:  // coordinate
        case WaypointTyp.OTH:
        default:
          return RoutePointType.Other;
      }
    }

    /// <summary>
    /// True when the Wyp is valid
    /// </summary>
    public static bool IsValidWaypoint( Waypoint waypoint )
    {
      // make sure it contains mandatory items
      if (waypoint.WaypointType == WaypointTyp.Unknown) return false; // cannot be unknown
      if (string.IsNullOrEmpty( waypoint.SourceIdent )) return false;
      if (waypoint.LatLonAlt_ft.IsEmpty) return false;

      // Usage may be unknown ?!
      return true;
    }


    /// <summary>
    /// True when the Wyp is valid
    /// </summary>
    public bool IsValid => IsValidWaypoint( this );

    /// <summary>
    /// The Waypoint Index of this item 0..N
    /// </summary>
    public uint Index { get; internal set; } = uint.MaxValue;

    /// <summary>
    /// True when derived from the Flightplan
    /// False if added later (Usually as Approach)
    /// </summary>
    public bool OriginalPoint { get; internal set; } = true;

    /// <summary>
    /// Mandatory: Get;Set: Type of this Waypoint (from Source)
    /// </summary>
    public WaypointTyp WaypointType { get; internal set; } = WaypointTyp.Unknown;

    /// <summary>
    /// Mandatory: Get;Set: Usage of this Waypoint
    /// </summary>
    public UsageTyp WaypointUsage { get; internal set; } = UsageTyp.Unknown;

    /// <summary>
    /// Get: A RoutepointType from this Waypoint
    /// </summary>
    public RoutePointType RoutePointType => FromWaypointTyp( WaypointType, OnDeparture );

    /// <summary>
    /// Mandatory: Get;Set: The Lat, Lon, Target Altitude [ft] of this Waypoint
    /// Note: Set Altitude 0 or negative if no target alt is assigned
    /// </summary>
    public LatLon LatLonAlt_ft { get => _LatLonAlt_ft; internal set => _LatLonAlt_ft = value; }
    private LatLon _LatLonAlt_ft = LatLon.Empty;

    /// <summary>
    /// Set a new Target Altitude for this Waypoint
    /// </summary>
    /// <param name="feet">Alt in feet</param>
    public void SetAltitude_ft( double feet )
    {
      _LatLonAlt_ft = new LatLon( _LatLonAlt_ft.Lat, _LatLonAlt_ft.Lon, feet );
    }

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    public int AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? XMath.AsRoundInt( LatLonAlt_ft.Altitude, 1 )
        : XMath.AsRoundInt( LatLonAlt_ft.Altitude, 100 );


    /// <summary>
    /// Mandatory: Get;Set: Original Waypoint Ident from source
    /// </summary>
    public string SourceIdent { get; internal set; } = "";
    /// <summary>
    /// Get: Clean ID of the Waypoint - removed all known decorations
    /// </summary>
    public string Ident => Formatter.CleanName( SourceIdent );
    /// <summary>
    /// Get: Clean ID of the Waypoint - removed all known decorations
    /// shortened to 7 chars max
    /// </summary>
    public string Ident7 => XString.LeftStringOf( Ident, 7 );
    /// <summary>
    /// Get: The Decoration for soaring waypoints
    /// </summary>
    public string IdentDecoration => Formatter.GetDecoration( SourceIdent );
    /// <summary>
    /// Get: True if the Wyp as DECO information e.g. soaring Wyp 
    /// </summary>
    public bool IsDecorated => !string.IsNullOrWhiteSpace( IdentDecoration );

    /// <summary>
    /// Optional: Get;Set: The ICAO ident of this Waypoint
    /// </summary>
    public IcaoRec Icao_Ident { get; internal set; } = new IcaoRec( );

    /// <summary>
    /// Optional: Get;Set: The common name of this Waypoint (or Ident if not set)
    /// </summary>
    public string CommonName {
      get => string.IsNullOrEmpty( _commonName ) ? Ident : _commonName;
      internal set => _commonName = value;
    }
    private string _commonName = "";

    /// <summary>
    /// Returns a Coordinate Name for this item (e.g. "20N185E")
    /// </summary>
    public string CoordName => Dms.ToRouteCoord( LatLonAlt_ft, "d" );
    /// <summary>
    /// Get: The Magnetic Variation at this location
    /// </summary>
    public double MagVar_deg => LatLonAlt_ft.MagVarLookup_deg( );

    /// <summary>
    /// Get;Set: Lower Altitude Limit
    ///   Use negative or 0 to omit
    /// </summary>
    public int AltitudeLimitLo_ft { get; internal set; } = 0;
    /// <summary>
    /// Get;Set: Higher Altitude Limit
    ///   Use negative or 0 to omit
    /// </summary>
    public int AltitudeLimitHi_ft { get; internal set; } = 0;

    /// <summary>
    /// Get: The Altitude Limit Type that applies
    /// </summary>
    public AltLimitType AltitudeLimitType => EvalAltLimit( );

    /// <summary>
    /// Get: The Altitude Limit String that applies
    /// </summary>
    public string AltitudeLimitS => EvalAltLimitS( );

    /// <summary>
    /// Get: This Waypoints target Altitude
    /// either from Limits or when no limits apply from original flightplan waypoint altitude
    ///   negative or 0 if not used or set
    /// </summary>
    public int TargetAltitude_ft => EvalTargetAltitude( );

    /// <summary>
    /// Speed Limit for Procedures
    /// </summary>
    public int SpeedLimit_kt { get; internal set; } = 0;

    /// <summary>
    /// Get;Set: Mandatory: True when this Waypoint is on the departure part of the plan
    /// </summary>
    public bool OnDeparture { get; internal set; } = false;

    /// <summary>
    /// Optional: Get;Set: A SID ident this Wyp belongs to
    /// Empty when not set
    /// </summary>
    public string SID_Ident { get; internal set; } = "";

    /// <summary>
    /// Optional: Get;Set: An Airway ident this Wyp belongs to
    /// Empty when not set
    /// </summary>
    public string Airway_Ident { get; internal set; } = "";

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
    ///   for library internal use only
    /// </summary>
    internal string RunwayNumber_S { get; set; } = ""; // leave it as string - don't know what could be in here...
    /// <summary>
    /// Optional: Get;Set: The Runway designation as string as provided by the plan
    /// e.g. R, L, C, RIGHT, LEFT, BOTH??
    ///   for library internal use only
    /// </summary>
    internal string RunwayDesignation { get; set; } = ""; // RIGHT, LEFT, CENTER, ?? others ??

    /// <summary>
    /// Get: Returns a Runway ident like RW22 or RW12R etc.
    /// </summary>
    public string RunwayIdent => AsRwIdent( RunwayNumber_S, RunwayDesignation );

    /// <summary>
    /// Get: Set: True when hidden for the Map Display (default is false)
    /// </summary>
    public bool HiddenInMap { get; internal set; } = false;

    /// <summary>
    /// Get: True if the Wyp is part of an Enroute
    ///  Note: will return false if the waypoint is also part of SID or STAR
    /// </summary>
    public bool IsAirway => !string.IsNullOrWhiteSpace( Airway_Ident );// && !IsSIDorSTAR;
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
    /// Get: True if the Wyp is part of an Approach
    /// </summary>
    public bool IsAPR => !string.IsNullOrEmpty( ApproachProcRef );
    /// <summary>
    /// Get: True is this Wyp belongs to a SID OR to a STAR OR to an Approach
    /// </summary>
    public bool IsProc => IsSID || IsSTAR || IsAPR;

    /// <summary>
    /// Optional: Get;Set: An associated frequency as string
    /// </summary>
    public string Frequency { get; internal set; } = "";

    /// <summary>
    /// Optional: Get;Set: A stage string (source dependent e.g. CLB, CRZ ...)
    /// </summary>
    public string Stage { get; internal set; } = "";


    #region INBOUND Features

    /// <summary>
    /// Get;Set: Distance TO this Waypoint (from the previous one)
    /// Use negative when not known
    /// </summary>
    public double InboundDistance_nm { get; internal set; } = -1;

    /// <summary>
    /// Get;Set: Inbound True Track [deg] from last Wyp (-1 if not set)
    /// </summary>
    public int InboundTrueTrk { get; internal set; } = -1;
    /// <summary>
    /// Get: Inbound Mag Track [degm] from last Wyp
    /// </summary>
    public int InboundMagTrk => (InboundTrueTrk < 0) ? -1
                              : (int)CoordLib.WMM.MagVarEx.MagFromTrueBearing( InboundTrueTrk, LatLonAlt_ft, true );

    #endregion

    #region OUTBOUND Features

    /// <summary>
    /// Coordinate of the outbound point (next Wyp)
    /// </summary>
    public LatLon OutboundLatLonAlt { get; internal set; } = LatLon.Empty;

    /// <summary>
    /// Get;Set: Distance FROM this Waypoint (to the nex one)
    /// Use negative when not known
    /// </summary>
    public double OutboundDistance_nm { get; internal set; } = -1;

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
    /// True if the Outbound route is part of an Airway
    /// </summary>
    public bool OutboundIsAirway { get; internal set; } = false;
    /// <summary>
    /// True if the Outbound route is part of the SID
    /// </summary>
    public bool OutboundIsSID { get; internal set; } = false;
    /// <summary>
    /// True if the Outbound route is part of the STAR
    /// </summary>
    public bool OutboundIsSTAR { get; internal set; } = false;
    /// <summary>
    /// True if the Outbound route is part of the Approach
    /// </summary>
    public bool OutboundIsApproach { get; internal set; } = false;
    /// <summary>
    /// True if the Outbound route goes to an Airport/Runway
    /// </summary>
    public bool OutboundIsApt { get; internal set; } = false;

    #endregion

    /// <summary>
    /// ctor: 
    /// 
    ///   Restricted for library internal use
    /// </summary>
    internal Waypoint( ) { }

    /// <summary>
    /// Merge this with the other waypoint where this is considered as Master
    /// and the other will contribute unknown items
    /// 
    /// Note: if the two are not equal in the first place it will not merge from the other
    /// 
    ///   Restricted for library internal use
    /// </summary>
    /// <param name="other">Contributor to the Merge</param>
    /// <returns>The merged one (this)</returns>
    internal Waypoint Merge( Waypoint other )
    {
      // sanity
      if (!this.Equals( other )) return this; // will catch nulls as well

      // Ident and WaypointType are the same already

      // add unknowns and comment on not merged properties
      // Index - will be recalculated
      this.OriginalPoint |= other.OriginalPoint; // any will set
      if (this.WaypointUsage == UsageTyp.Unknown) this.WaypointUsage = other.WaypointUsage; // if not set only
      if (this.LatLonAlt_ft.IsEmpty) this.LatLonAlt_ft = other.LatLonAlt_ft; // use if not set
      if (double.IsNaN( this.LatLonAlt_ft.Altitude )) this.LatLonAlt_ft.SetAltitude( other.LatLonAlt_ft.Altitude ); // add Alt if not set
      if (this.LatLonAlt_ft.Altitude <= 0) this.LatLonAlt_ft.SetAltitude( other.LatLonAlt_ft.Altitude ); // add Alt if <=0
      if (!this.Icao_Ident.IsValid) this.Icao_Ident = other.Icao_Ident; // copy if not set
      if (string.IsNullOrEmpty( this.CommonName )) CommonName = other.CommonName; // if not set only
      if (this.AltitudeLimitLo_ft <= 0) this.AltitudeLimitLo_ft = other.AltitudeLimitLo_ft;
      if (this.AltitudeLimitHi_ft <= 0) this.AltitudeLimitHi_ft = other.AltitudeLimitHi_ft;
      if (this.SpeedLimit_kt <= 0) this.SpeedLimit_kt = other.SpeedLimit_kt;
      this.OnDeparture |= other.OnDeparture; // any will set
      if (string.IsNullOrEmpty( this.SID_Ident )) SID_Ident = other.SID_Ident;
      if (string.IsNullOrEmpty( this.Airway_Ident )) Airway_Ident = other.Airway_Ident;
      if (string.IsNullOrEmpty( this.STAR_Ident )) STAR_Ident = other.STAR_Ident;
      if (string.IsNullOrEmpty( this.ApproachTypeS )) ApproachTypeS = other.ApproachTypeS;
      if (string.IsNullOrEmpty( this.ApproachSuffix )) ApproachSuffix = other.ApproachSuffix;
      // don't merge approach sequence
      if (string.IsNullOrEmpty( this.RunwayNumber_S )) RunwayNumber_S = other.RunwayNumber_S;
      if (string.IsNullOrEmpty( this.RunwayDesignation )) RunwayDesignation = other.RunwayDesignation;
      this.HiddenInMap &= other.HiddenInMap; // any will clear
      if (string.IsNullOrEmpty( this.Frequency )) Frequency = other.Frequency;
      if (string.IsNullOrEmpty( this.Stage )) Stage = other.Stage;

      // inbound/outbound items will be recalculated by PostProcess (which must be done after adding to the plan)

      return this;
    }

    #region Equality Comparer (IEquatable)

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

    #endregion

    /// <inheritdoc/>
    public override string ToString( )
    {
      return $"RP: {Ident,8} <{RoutePointType}> (SID: {IsSID}, STAR: {IsSTAR}, APR: {IsAPR}, AWY: {IsAirway}) Limits: {AltitudeLimitS}";
    }

    // *** TOOLS

    // return the Waypoints target altitude
    private int EvalTargetAltitude( )
    {
      // if Limit return limit
      switch (AltitudeLimitType) {
        case AltLimitType.At:
        case AltLimitType.Above:
        case AltLimitType.Runway: return AltitudeLimitLo_ft;

        case AltLimitType.Below: return AltitudeLimitHi_ft;

        case AltLimitType.Between: return (int)((AltitudeLimitHi_ft + AltitudeLimitLo_ft) / 2.0);

        case AltLimitType.NoLimit:
        default: return double.IsNaN( LatLonAlt_ft.Altitude ) ? -1 : (int)LatLonAlt_ft.Altitude;
      }
    }

    // return the Altitude limit type
    private AltLimitType EvalAltLimit( )
    {
      if (WaypointType == WaypointTyp.RWY) {
        return AltLimitType.Runway;
      }

      if (InvalidAlt( AltitudeLimitLo_ft ) && InvalidAlt( AltitudeLimitHi_ft )) {
        return AltLimitType.NoLimit;
      }
      else if (ValidAlt( AltitudeLimitLo_ft ) && ValidAlt( AltitudeLimitHi_ft )) {
        // both defined At or Between
        return XMath.AboutEqual( AltitudeLimitLo_ft, AltitudeLimitHi_ft, 400 ) ? AltLimitType.At : AltLimitType.Between;
      }
      else if (ValidAlt( AltitudeLimitLo_ft )) {
        // Low defined
        return AltLimitType.Above;
      }
      else if (ValidAlt( AltitudeLimitHi_ft )) {
        // Hight defined
        return AltLimitType.Below;
      }

      return AltLimitType.NoLimit;
    }


    // return the Altitude limit string
    private string EvalAltLimitS( )
    {
      switch (AltitudeLimitType) {
        case AltLimitType.NoLimit: return ""; // ""
        case AltLimitType.Runway: return $"{this.LatLonAlt_ft.Altitude:##,##0}"; // 150
        case AltLimitType.At: return $"{this.AltitudeLimitLo_ft:##,##0}"; // 15'000
        case AltLimitType.Above: return $"/{this.AltitudeLimitLo_ft:##,##0}"; // /8'000
        case AltLimitType.Below: return $"{this.AltitudeLimitHi_ft:##,##0}/"; // 15'000/
        case AltLimitType.Between: return $"{this.AltitudeLimitHi_ft:##,##0}/{this.AltitudeLimitLo_ft:##,##0}"; // 15'000/8'000
        default:
          return "";
      }
    }

  }
}
