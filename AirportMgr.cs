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
    /// The Airport ICAO Code
    /// </summary>
    public static string AirportICAO { get; private set; } = AirportNA_Icao;

    /// <summary>
    /// For valid Airports, the Airport Name
    /// </summary>
    public static string AirportName { get; private set; } = "";

    /// <summary>
    /// For valid Airports, the Location if found, else it is null
    /// </summary>
    public static LatLon Location { get; private set; } = null;

    /// <summary>
    /// True if the Airport has changed 
    /// (Use Read() to commit)
    /// </summary>
    public static bool HasChanged { get; private set; } = false;
    /// <summary>
    /// True if a real Airport is available
    /// </summary>
    public static bool IsAvailable => AirportICAO != AirportNA_Icao;

    /// <summary>
    /// Update from Sim Data Event
    /// Maintain the current Airport
    /// 
    /// </summary>
    /// <param name="icao">The airport ICAO code</param>
    public static void Update( string icao )
    {
      icao = icao.ToUpperInvariant( );
      if ( icao == AirportICAO ) return; // same as before

      // reset before evaluation
      Location = null;
      AirportName = "";

      if ( !string.IsNullOrWhiteSpace( icao ) ) {
        AirportICAO = icao;
        var apt = Airports.FindAirport( icao );
        if ( apt != null ) {
          Location = new LatLon( apt.Lat, apt.Lon, apt.Elevation );
          AirportName = apt.Name;
        }
        else {
          AirportICAO = AirportNA_Icao; // Dummy one if not in DB
        }
      }
      else {
        AirportICAO = AirportNA_Icao; // Dummy one
      }
      HasChanged = true;
    }

    /// <summary>
    /// Reset the Manager
    /// </summary>
    public static void Reset( )
    {
      AirportICAO = AirportNA_Icao; // Dummy one
      HasChanged = true;
    }

    /// <summary>
    /// Commit reading the new airport
    /// </summary>
    /// <returns>The current Airport Code</returns>
    public static string Read( )
    {
      HasChanged = false;
      return AirportICAO;
    }

  }
}
