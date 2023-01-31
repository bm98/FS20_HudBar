using CoordLib;
using FlightplanLib.MS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// FLT Ini Section ATC_ActiveFlightPlan
  /// </summary>
  public class Ini_ATC_ActiveFlightPlan
  {
    [IniFileKey( "title" )]
    public string Title { get; internal set; } = "";

    [IniFileKey( "description" )]
    public string Description { get; internal set; } = "";

    [IniFileKey( "type" )]
    public string PlanType_S { get; internal set; } = ""; // IFR, VFR, VOR ??

    [IniFileKey( "routetype" )]
    public int Routetype_Num { get; internal set; } = 0; // 0=.. ??  LoAlt, HiAlt, VFR...
    [IniFileIgnore]
    public string Routetype => RouteTypeFromNum( Routetype_Num );

    [IniFileKey( "cruising_altitude" )]
    public double CruisingAltitude_ft { get; internal set; } = 0.0;

    [IniFileKey( "departure_id" )]
    public string DepartureID { get; set; } = ""; // ICAO, LLA
    [IniFileIgnore]
    public string DepartureICAO => IcaoFromID( DepartureID );
    [IniFileIgnore]
    public string DepartureLLA => LLAFromID( DepartureID );
    [IniFileKey( "departure_name" )]
    public string DepartureName { get; set; } = ""; // an Airport common name
    [IniFileKey( "departure_position" )]
    public string DeparturePosition { get; set; } = ""; // runway or parking spot e.g. 11, N PARKING 2, 

    [IniFileKey( "destination_id" )]
    public string DestinationID { get; set; } = ""; // ICAO, LLA
    [IniFileIgnore]
    public string DestinationICAO => IcaoFromID( DestinationID );
    [IniFileIgnore]
    public string DestinationLLA => LLAFromID( DestinationID );
    [IniFileKey( "destination_name" )]
    public string DestinationName { get; set; } = ""; // an Airport common name

    [IniFileIgnore]
    public LatLon DEP_LatLon => Formatter.ToLatLon( DepartureLLA );
    [IniFileIgnore]
    public double DEP_Lat => DEP_LatLon.Lat;
    [IniFileIgnore]
    public double DEP_Lon => DEP_LatLon.Lon;
    [IniFileIgnore]
    public float DEP_Altitude_ft => (float)DEP_LatLon.Altitude;


    [IniFileIgnore]
    public LatLon DST_LatLon => Formatter.ToLatLon( DestinationLLA );
    [IniFileIgnore]
    public double DST_Lat => DST_LatLon.Lat;
    [IniFileIgnore]
    public double DST_Lon => DST_LatLon.Lon;
    [IniFileIgnore]
    public float DST_Altitude_ft => (float)DST_LatLon.Altitude;

    [IniFileIgnore]
    public TypeOfFlightplan FlightPlanType => ToTypeOfFP( PlanType_S );
    [IniFileIgnore]
    public TypeOfRoute RouteType => ToTypeOfRoute( Routetype_Num );

    // tools

    private static string LLAFromID( string id )
    {
      int p = id.IndexOf( "," );
      if (p == -1) return id;
      return id.Substring( p + 1 );
    }
    private static string IcaoFromID( string id )
    {
      int p = id.IndexOf( "," );
      if (p == -1) return id;
      return id.Substring( 0, p );
    }

    private static string RouteTypeFromNum( int num )
    {
      switch (num) {
        case 0: return "VFR";
        case 1: return "LowAlt";
        case 2: return "HighAlt";
        default: return "VFR";
      }
    }

    private static TypeOfFlightplan ToTypeOfFP( string fpType )
    {
      switch (fpType.ToUpperInvariant( )) {
        case "IFR": return TypeOfFlightplan.IFR;
        case "VFR": return TypeOfFlightplan.VFR;
        case "VOR": return TypeOfFlightplan.VOR;
        default: return TypeOfFlightplan.VFR;
      }
    }

    private static TypeOfRoute ToTypeOfRoute( int num )
    {
      switch (num) {
        case 0: return TypeOfRoute.VFR;
        case 1: return TypeOfRoute.VOR;
        case 2: return TypeOfRoute.LowAlt;
        case 3: return TypeOfRoute.HighAlt;
        default: return TypeOfRoute.VFR;
      }
    }

  }
}
