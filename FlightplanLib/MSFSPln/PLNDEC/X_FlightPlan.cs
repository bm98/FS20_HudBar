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
    [XmlElement( ElementName = "Title" )]
    public string Title { get; set; } = "";

    [XmlElement( ElementName = "Descr" )]
    public string Description { get; set; } = ""; // description 

    [XmlElement( ElementName = "FPType" )]
    public string PlanType_S { get; set; } = ""; // IFR, VFR, VOR ??

    [XmlElement( ElementName = "RouteType" )]
    public string RouteType_S { get; set; } = ""; // HighAlt, LowAlt, VFR, VOR ??

    [XmlElement( ElementName = "CruisingAlt" )]
    public string CruisingAlt_S { get; set; } = ""; // a number


    [XmlElement( ElementName = "DepartureID" )]
    public string DepartureICAO { get; set; } = ""; // an ICAO airport code

    [XmlElement( ElementName = "DepartureLLA" )]
    public string DepartureLLA { get; set; } = ""; // an LLA

    [XmlElement( ElementName = "DepartureName" )]
    public string DepartureName { get; set; } = ""; // an Airport common name

    [XmlElement( ElementName = "DeparturePosition" )]
    public string DeparturePosition { get; set; } = ""; // runway or parking spot e.g. 11, N PARKING 2, 



    [XmlElement( ElementName = "DestinationID" )]
    public string DestinationICAO { get; set; } = ""; // an ICAO airport code

    [XmlElement( ElementName = "DestinationLLA" )]
    public string DestinationLLA { get; set; } = ""; // an LLA

    [XmlElement( ElementName = "DestinationName" )]
    public string DestinationName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// Catalog of Waypoints
    /// </summary>
    [XmlElement( ElementName = "ATCWaypoint" )]
    public List<X_ATCWaypoint> WaypointCat { get; set; } = new List<X_ATCWaypoint>( );


    [XmlElement( ElementName = "AppVersion" )]
    public X_AppVersion AppVersion { get; set; } = new X_AppVersion( );


    // Non XML

    public LatLon DEP_LatLon => Formatter.ToLatLon( DepartureLLA );
    public double DEP_Lat => DEP_LatLon.Lat;
    public double DEP_Lon => DEP_LatLon.Lon;
    public float DEP_Altitude_ft => (float)DEP_LatLon.Altitude;


    public LatLon DST_LatLon => Formatter.ToLatLon( DestinationLLA );
    public double DST_Lat => DST_LatLon.Lat;
    public double DST_Lon => DST_LatLon.Lon;
    public float DST_Altitude_ft => (float)DST_LatLon.Altitude;

    public float CruisingAlt_ft => (float)Formatter.GetValue( CruisingAlt_S );

    public TypeOfFlightplan FlightPlanType => ToTypeOfFP( PlanType_S );
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
