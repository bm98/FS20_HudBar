using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;

using FSimFacilityIF;

using static FSimFacilityIF.Extensions;

namespace FlightplanLib.GPX.GPXDEC
{
  /// <summary>
  /// An GPX X_RtePt Waypoint Element
  /// </summary>
  [XmlRoot( ElementName = "rtept", IsNullable = true )]
  public class X_RtePt
  {
    #region EXAMPLE

    /*
     * EXAMPLE:
     * 
    <rtept lon="139.781113" lat="35.553333">
      <ele>8.8392</ele>
      <name>RJTT</name>
      <desc>Airport</desc>
    </rtept>

    <rtept lon="-118.401947" lat="33.952103">
      <ele>9753</ele>
      <name>RW24R</name>
      <desc>User</desc>
    </rtept>
    <rtept lon="-118.502701" lat="33.946671">
      <ele>9753</ele>
      <name></name>
      <desc>User</desc>
    </rtept>
    <rtept lon="-118.861359" lat="34.804028">
      <ele>9753</ele>
      <name>GMN</name>
      <desc>VOR</desc>
    </rtept>
    
    <rtept lon="-118.616371" lat="35.012428">
      <ele>9753</ele>
      <name>LANDO</name>
      <desc>Intersection</desc>
    </rtept>
    ....
    <rtept lon="-60.447449" lat="53.304085">
      <ele>9753</ele>
      <name>RW08</name>
      <desc>User</desc>
    </rtept>
    <rtept lon="-60.425793" lat="53.319237">
      <ele>48.768</ele>
      <name>CYYR</name>
      <desc>Airport</desc>
    </rtept>
    */

    #endregion

    // Attributes
    /// <summary>
    /// The Latitude
    /// </summary>
    [XmlAttribute( AttributeName = "lat" )]
    public double Lat { get; set; } = double.NaN;
    /// <summary>
    /// The Longitude
    /// </summary>
    [XmlAttribute( AttributeName = "lon" )]
    public double Lon { get; set; } = double.NaN;


    // Elements
    /// <summary>
    /// Altitude String [m]
    /// </summary>
    [XmlElement( ElementName = "ele" )]
    public string AltitudeS_m { get; set; } = "";

    /// <summary>
    /// The Name/ID/ICAO
    /// </summary>
    [XmlElement( ElementName = "name" )]
    public string Ident { get; set; } = "";
    /// <summary>
    /// The Descriptor (Airport, )
    /// </summary>
    [XmlElement( ElementName = "desc" )]
    public string Descriptor { get; set; } = "";

    // Content

    // non XML

    /// <summary>
    /// True if the element has content
    /// </summary>
    [XmlIgnore]
    public bool IsValid => !LatLonAlt_ft.IsEmpty;

    /// <summary>
    /// A Name for the Waypoint (Airports may get a real name)
    /// </summary>
    [XmlIgnore]
    public string Name { get; set; } = "";

    /// <summary>
    /// Returns the Ident
    /// </summary>
    [XmlIgnore]
    public string ICAO => Ident;

    /// <summary>
    /// ALtitude in ft
    /// </summary>
    [XmlIgnore]
    public float Altitude_f => (float)dNetBm98.Units.Ft_From_M( Formatter.GetValue( AltitudeS_m ) );

    /// <summary>
    /// LatLonAlt_ft  element
    /// </summary>
    [XmlIgnore]
    public LatLon LatLonAlt_ft => new LatLon( Lat, Lon, AltitudeRounded_ft );
    /// <summary>
    /// Returns a Coordinate Name for this item
    /// </summary>
    [XmlIgnore]
    public string CoordName => Dms.ToRouteCoord( LatLonAlt_ft, "d" );

    /// <summary>
    /// A rounded Altitude to 100ft except for Airports and Runways
    /// </summary>
    [XmlIgnore]
    public int AltitudeRounded_ft =>
      (WaypointType == WaypointTyp.APT || WaypointType == WaypointTyp.RWY)
        ? dNetBm98.XMath.RoundInt( Altitude_f, 1 )
        : dNetBm98.XMath.RoundInt( Altitude_f, 100 );

    /// <summary>
    /// A Runway Ident if the element is a Runway
    /// else an empty string
    /// </summary>
    [XmlIgnore]
    public string RunwayIdent {
      get {
        if (WaypointType == WaypointTyp.RWY) return AsRwIdent( Ident );
        return string.Empty;
      }
    }
    /// <summary>
    /// A Runway Number as string if the element is a Runway
    /// else an empty string
    /// </summary>
    [XmlIgnore]
    public string RunwayNumber_S => RunwayIdent.RwNumberOf( );
    /// <summary>
    /// A Runway Designation as string if the element is a Runway
    /// else an empty string
    /// </summary>
    [XmlIgnore]
    public string RunwayDesignation => RunwayIdent.RwDesignationOf( );


    /// <summary>
    /// SID Proc Name if it applies
    /// </summary>
    [XmlIgnore]
    public string SID_Ident { get; set; } = "";

    /// <summary>
    /// STAR Proc Name if it applies
    /// </summary>
    [XmlIgnore]
    public string STAR_Ident { get; set; } = "";

    /// <summary>
    /// APPROACH Proc Type if it applies 'ILS X'  'RNAV X' 'VOR'
    /// </summary>
    [XmlIgnore]
    public string ApproachProcRef { get; set; } = "";
    /// <summary>
    /// APPROACH Proc Type if it applies 
    /// </summary>
    [XmlIgnore]
    public string ApproachTypeS => ApproachProcRef.ProcOf( );
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
    /// Waypoint Usage derived from content
    /// </summary>
    public UsageTyp UsageType {
      get {
        if (!string.IsNullOrEmpty( SID_Ident )) return UsageTyp.SID;
        if (!string.IsNullOrEmpty( STAR_Ident )) return UsageTyp.STAR;
        if (!string.IsNullOrEmpty( ApproachProcRef )) return UsageTyp.APR;
        return UsageTyp.Unknown;
      }
    }

    /// <summary>
    /// The type of the Waypoint as enum
    /// </summary>
    [XmlIgnore]
    public WaypointTyp WaypointType => ToWaypointTyp( Descriptor, Ident );

    // local only
    private static WaypointTyp ToWaypointTyp( string fpType, string id )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "AIRPORT": return WaypointTyp.APT;
        case "INTERSECTION": return WaypointTyp.WYP;
        case "WAYPOINT": return WaypointTyp.WYP;
        case "NDB": return WaypointTyp.NDB;
        case "RUNWAY": return WaypointTyp.RWY;
        case "USER":
          var idUcase = id.ToUpperInvariant( );
          if (string.IsNullOrEmpty( idUcase )) return WaypointTyp.COR;
          if (idUcase.StartsWith( "RW" ) && !idUcase.Contains( "+" )) return WaypointTyp.RWY; // seen RW22+3
          return WaypointTyp.USR;
        case "VOR": return WaypointTyp.VOR;
        default: return WaypointTyp.OTH;
      }
    }

  }
}
