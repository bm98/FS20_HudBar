using CoordLib;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// An MSFS PLN FlightPlan Element
  /// </summary>
  [XmlRootAttribute( "FlightPlan.FlightPlan", Namespace = "", IsNullable = false )]
  public class X_FlightPlan
  {
    // Elements
    /// <summary>
    /// The Title element
    /// </summary>
    [XmlElement( ElementName = "Title" )]
    public string Title { get; set; } = "";

    /// <summary>
    /// The Descr element
    /// </summary>
    [XmlElement( ElementName = "Descr" )]
    public string Description { get; set; } = ""; // description 

    /// <summary>
    /// The FPType element
    /// </summary>
    [XmlElement( ElementName = "FPType" )]
    public string PlanType_S { get; set; } = ""; // IFR, VFR, VOR ??

    /// <summary>
    /// The RouteType element
    /// </summary>
    [XmlElement( ElementName = "RouteType" )]
    public string RouteType_S { get; set; } = ""; // HighAlt, LowAlt, VFR, VOR ??

    /// <summary>
    /// The CruisingAlt element
    /// </summary>
    [XmlElement( ElementName = "CruisingAlt" )]
    public string CruisingAlt_S { get; set; } = ""; // a number


    /// <summary>
    /// The DepartureID element
    /// </summary>
    [XmlElement( ElementName = "DepartureID" )]
    public string DepartureICAO { get; set; } = ""; // an ICAO airport code

    /// <summary>
    /// The DepartureLLA element
    /// </summary>
    [XmlElement( ElementName = "DepartureLLA" )]
    public string DepartureLLA { get; set; } = ""; // an LLA

    /// <summary>
    /// The DepartureName element
    /// </summary>
    [XmlElement( ElementName = "DepartureName" )]
    public string DepartureName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// The DeparturePosition element
    /// </summary>
    [XmlElement( ElementName = "DeparturePosition" )]
    public string DeparturePosition { get; set; } = ""; // runway or parking spot e.g. 11, N PARKING 2, 


    /// <summary>
    /// The DestinationID element
    /// </summary>
    [XmlElement( ElementName = "DestinationID" )]
    public string DestinationICAO { get; set; } = ""; // an ICAO airport code

    /// <summary>
    /// The DestinationLLA element
    /// </summary>
    [XmlElement( ElementName = "DestinationLLA" )]
    public string DestinationLLA { get; set; } = ""; // an LLA

    /// <summary>
    /// The DestinationName element
    /// </summary>
    [XmlElement( ElementName = "DestinationName" )]
    public string DestinationName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// Catalog of Waypoints
    /// </summary>
    [XmlElement( ElementName = "ATCWaypoint" )]
    public List<X_ATCWaypoint> WaypointCat { get; set; } = new List<X_ATCWaypoint>( );


    /// <summary>
    /// The AppVersion element
    /// </summary>
    [XmlElement( ElementName = "AppVersion" )]
    public X_AppVersion AppVersion { get; set; } = new X_AppVersion( );


    // Non XML

    /// <summary>
    /// Departure LatLonAlt (derived field)
    /// </summary>
    public LatLon DEP_LatLon => Formatter.ToLatLon( DepartureLLA );
    /// <summary>
    /// Departure Latitude (derived field)
    /// </summary>
    public double DEP_Lat => DEP_LatLon.Lat;
    /// <summary>
    /// Departure Longitude (derived field)
    /// </summary>
    public double DEP_Lon => DEP_LatLon.Lon;
    /// <summary>
    /// Departure Altitude ft (derived field)
    /// </summary>
    public float DEP_Altitude_ft => (float)DEP_LatLon.Altitude;


    /// <summary>
    /// Destination LatLonAlt (derived field)
    /// </summary>
    public LatLon DST_LatLon => Formatter.ToLatLon( DestinationLLA );
    /// <summary>
    /// Destination Latitude (derived field)
    /// </summary>
    public double DST_Lat => DST_LatLon.Lat;
    /// <summary>
    /// Destination Longitude (derived field)
    /// </summary>
    public double DST_Lon => DST_LatLon.Lon;
    /// <summary>
    /// Destination Altitude ft (derived field)
    /// </summary>
    public float DST_Altitude_ft => (float)DST_LatLon.Altitude;

    /// <summary>
    /// Cruising Altitude ft number (derived field)
    /// </summary>
    public float CruisingAlt_ft => (float)Formatter.GetValue( CruisingAlt_S );

    /// <summary>
    /// Flight plan type as enum (derived field)
    /// </summary>
    public TypeOfFlightplan FlightPlanType => ToTypeOfFP( PlanType_S );
    /// <summary>
    /// Route type as enum (derived field)
    /// </summary>
    public TypeOfRoute RouteType => ToTypeOfRoute( RouteType_S );


    private static TypeOfFlightplan ToTypeOfFP( string fpType )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "IFR": return TypeOfFlightplan.IFR;
        case "VFR": return TypeOfFlightplan.VFR;
        case "VOR": return TypeOfFlightplan.VOR;
        default: return TypeOfFlightplan.VFR;
      }
    }

    private static TypeOfRoute ToTypeOfRoute( string rtType )
    {
      switch (rtType.ToUpperInvariant( )) {
        case "LOWALT": return TypeOfRoute.LowAlt;
        case "HIGHALT": return TypeOfRoute.HighAlt;
        case "VOR": return TypeOfRoute.VOR;
        case "VFR": return TypeOfRoute.VFR;
        default: return TypeOfRoute.VFR;
      }
    }

  }
}
