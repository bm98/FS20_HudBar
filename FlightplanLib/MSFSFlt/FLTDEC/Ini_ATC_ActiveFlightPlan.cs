using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

using dNetBm98.IniLib;

using FlightplanLib.Flightplan;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// FLT Ini Section ATC_ActiveFlightPlan
  /// </summary>
  public class Ini_ATC_ActiveFlightPlan
  {
    /// <summary>
    /// Title field
    /// </summary>
    [IniFileKey( "title" )]
    public string Title { get; internal set; } = "";

    /// <summary>
    /// Description field
    /// </summary>
    [IniFileKey( "description" )]
    public string Description { get; internal set; } = "";

    /// <summary>
    /// Type field
    /// </summary>
    [IniFileKey( "type" )]
    public string PlanType_S { get; internal set; } = ""; // IFR, VFR, VOR ??

    /// <summary>
    /// Routetype field, a number
    /// </summary>
    [IniFileKey( "routetype" )]
    public int Routetype_Num { get; internal set; } = 0; // 0=.. ??  LoAlt, HiAlt, VFR...

    /// <summary>
    /// Cruising_altitude field
    /// </summary>
    [IniFileKey( "cruising_altitude" )]
    public double CruisingAltitude_ft { get; internal set; } = 0.0;

    /// <summary>
    /// Departure_id field
    /// </summary>
    [IniFileKey( "departure_id" )]
    public string DepartureID { get; set; } = ""; // ICAO, LLA
    /// <summary>
    /// Departure ICAO (derived value)
    /// </summary>
    [IniFileIgnore]
    public string DepartureICAO => IcaoFromID( DepartureID );
    /// <summary>
    /// Departure LLA (derived value)
    /// </summary>
    [IniFileIgnore]
    public string DepartureLLA => LLAFromID( DepartureID );
    /// <summary>
    /// Departure_name field
    /// </summary>
    [IniFileKey( "departure_name" )]
    public string DepartureName { get; set; } = ""; // an Airport common name
    /// <summary>
    /// Departure_position field
    /// </summary>
    [IniFileKey( "departure_position" )]
    public string DeparturePosition { get; set; } = ""; // runway or parking spot e.g. 11, N PARKING 2, 

    /// <summary>
    /// Destination_id field
    /// </summary>
    [IniFileKey( "destination_id" )]
    public string DestinationID { get; set; } = ""; // ICAO, LLA
    /// <summary>
    /// Destination ICAO (derived field)
    /// </summary>
    [IniFileIgnore]
    public string DestinationICAO => IcaoFromID( DestinationID );
    /// <summary>
    /// Destination LLA (derived field)
    /// </summary>
    [IniFileIgnore]
    public string DestinationLLA => LLAFromID( DestinationID );
    /// <summary>
    /// Destination_name field
    /// </summary>
    [IniFileKey( "destination_name" )]
    public string DestinationName { get; set; } = ""; // an Airport common name

    /// <summary>
    /// Waypoint. field for enumeration, find Waypoint.N (0.. max)
    /// </summary>
    [IniFileKey( "waypoint." )] // waypoint.0 .. .N
    public Dictionary<string, string> Waypoints { get; internal set; } = new Dictionary<string, string>( );

    /// <summary>
    /// Departure LatLonAlt (derived field)
    /// </summary>
    [IniFileIgnore]
    public LatLon DEP_LatLon => Formatter.ToLatLon( DepartureLLA );
    /// <summary>
    /// Departure Latitude (derived field)
    /// </summary>
    [IniFileIgnore]
    public double DEP_Lat => DEP_LatLon.Lat;
    /// <summary>
    /// Departure Longitude (derived field)
    /// </summary>
    [IniFileIgnore]
    public double DEP_Lon => DEP_LatLon.Lon;
    /// <summary>
    /// Departure Altitude ft (derived field)
    /// </summary>
    [IniFileIgnore]
    public float DEP_Altitude_ft => (float)DEP_LatLon.Altitude;

    /// <summary>
    /// Destination LatLonAlt (derived field)
    /// </summary>
    [IniFileIgnore]
    public LatLon DST_LatLon => Formatter.ToLatLon( DestinationLLA );
    /// <summary>
    /// Destination Latitude (derived field)
    /// </summary>
    [IniFileIgnore]
    public double DST_Lat => DST_LatLon.Lat;
    /// <summary>
    /// Destination Longitude (derived field)
    /// </summary>
    [IniFileIgnore]
    public double DST_Lon => DST_LatLon.Lon;
    /// <summary>
    /// Destination Altitude ft (derived field)
    /// </summary>
    [IniFileIgnore]
    public float DST_Altitude_ft => (float)DST_LatLon.Altitude;

    /// <summary>
    /// Flight plan type as enum (derived field)
    /// </summary>
    [IniFileIgnore]
    public TypeOfFlightplan FlightPlanType => ToTypeOfFP( PlanType_S );
    /// <summary>
    /// Route type as enum (derived field)
    /// </summary>
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
