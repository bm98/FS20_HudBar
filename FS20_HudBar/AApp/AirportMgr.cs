using CoordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_AptLib;

namespace FS20_HudBar
{
  /// <summary>
  /// Maintains the current airport from the Sim
  /// </summary>
  internal static class AirportMgr
  {
    /// <summary>
    /// The Airport "Not Available" name 
    /// </summary>
    public const string AirportNA_Icao = "n.a.";

    /// <summary>
    /// The Arrival Airport ICAO Code
    /// </summary>
    public static string ArrAirportICAO { get; private set; } = AirportNA_Icao;

    /// <summary>
    /// For valid Airports, the Arrival Airport Name
    /// </summary>
    public static string ArrAirportName { get; private set; } = "";

    /// <summary>
    /// For valid Airports, the Arrival Location if found, else it is null
    /// </summary>
    public static LatLon ArrLocation { get; private set; } = LatLon.Empty;

    /// <summary>
    /// True if a real Arrival Airport is available
    /// </summary>
    public static bool IsArrAvailable => ArrAirportICAO != AirportNA_Icao;

    /// <summary>
    /// The Departure Airport ICAO Code
    /// </summary>
    public static string DepAirportICAO { get; private set; } = AirportNA_Icao;

    /// <summary>
    /// For valid Airports, the Departure Airport Name
    /// </summary>
    public static string DepAirportName { get; private set; } = "";
    /// <summary>
    /// For valid Airports, the Departure Location if found, else it is null
    /// </summary>
    public static LatLon DepLocation { get; private set; } = LatLon.Empty;
    /// <summary>
    /// True if a real Departure Airport is available
    /// </summary>
    public static bool IsDepAvailable => DepAirportICAO != AirportNA_Icao;


    /// <summary>
    /// True if any Airport has changed 
    /// (Use Read() to commit)
    /// </summary>
    public static bool HasChanged { get; private set; } = false;

    /// <summary>
    /// Update from Sim Data Event
    /// Maintain the current Departure Airport - if the ICAO cannot be found it will get N.A.
    /// 
    /// </summary>
    /// <param name="icao">The airport ICAO code or an empty string</param>
    public static void UpdateDep( string icao )
    {
      icao = icao.ToUpperInvariant( );
      if (icao == DepAirportICAO) return; // same as before


      // then set if a string was given, else we leave it as it was before
      if (!string.IsNullOrWhiteSpace( icao )) {
        // first clear all
        DepAirportICAO = AirportNA_Icao;
        DepLocation = LatLon.Empty;
        DepAirportName = "";

        var apt = Airports.FindAirport( icao );
        if (apt != null) {
          // Apt found in DB
          DepLocation = new LatLon( apt.Lat, apt.Lon, apt.Elevation );
          DepAirportName = apt.Name;
          DepAirportICAO = icao;
        }

        HasChanged = true;
      }
    }

    /// <summary>
    /// Update from Sim Data Event
    /// Maintain the current Arrival Airport - if the ICAO cannot be found it will get N.A.
    /// 
    /// </summary>
    /// <param name="icao">The airport ICAO code or an empty string</param>
    public static void UpdateArr( string icao )
    {
      icao = icao.ToUpperInvariant( );
      if (icao == ArrAirportICAO) return; // same as before


      // then set if a string was given, else we leave it as it was before
      if (!string.IsNullOrWhiteSpace( icao )) {
        // first clear all
        ArrAirportICAO = AirportNA_Icao;
        ArrLocation = LatLon.Empty;
        ArrAirportName = "";

        var apt = Airports.FindAirport( icao );
        if (apt != null) {
          // Apt found in DB
          ArrLocation = new LatLon( apt.Lat, apt.Lon, apt.Elevation );
          ArrAirportName = apt.Name;
          ArrAirportICAO = icao;
        }

        HasChanged = true;
      }
    }

    /// <summary>
    /// Update from Sim Data Event
    /// Maintain the current Departure and Arrival Airport - if the ICAO cannot be found it will get N.A.
    /// 
    /// </summary>
    /// <param name="depIcao">The departure airport ICAO code or an empty string</param>
    /// <param name="arrIcao">The arrival airport ICAO code or an empty string</param>
    public static void Update( string depIcao , string arrIcao )
    {
      UpdateDep( depIcao );
      UpdateArr( arrIcao );
    }

    /// <summary>
    /// Reset the Manager
    /// </summary>
    public static void Reset( )
    {
      //AirportICAO = AirportNA_Icao; // don't kill the current entry
      HasChanged = true;
    }

    /// <summary>
    /// Commit reading the new airport
    /// </summary>
    /// <returns>The current Arrival Airport Code</returns>
    public static string Read( )
    {
      HasChanged = false;
      return ArrAirportICAO;
    }

    /// <summary>
    /// Calculated the straight distance from current to the Arrival Airport
    /// returns NaN if the Airport or current location is not valid
    /// </summary>
    /// <param name="currentLocation">LatLon of the current location</param>
    /// <returns>A distance [nm] or NaN</returns>
    public static float ArrDistance_nm( LatLon currentLocation )
    {
      // Sanity
      if ( currentLocation.IsEmpty ) return float.NaN;
      if ( ArrLocation.IsEmpty ) return float.NaN;

      return (float)currentLocation.DistanceTo( ArrLocation, CoordLib.ConvConsts.EarthRadiusNm );
    }

  }
}
