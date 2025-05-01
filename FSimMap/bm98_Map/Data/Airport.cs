using System;

using CoordLib;

using dNetBm98;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;

namespace bm98_Map.Data
{
  /// <summary>
  /// An Airport containing Runways
  ///  Derive from an Airport Template
  ///  Setup the Drawing List for this Airport (and a tracked Aircraft)
  /// </summary>
  internal class Airport : AirportCls
  {
    /// <summary>
    /// The Airport Elevation in [m]
    /// </summary>
    public int Elevation_m => (int)Units.M_From_Ft( Coordinate.Altitude );
    /// <summary>
    /// The Airport Elevation in [ft]
    /// </summary>
    public int Elevation_ft => (int)Coordinate.Altitude;

    /// <summary>
    /// Create a dummy Airport if none is available
    /// </summary>
    /// <param name="latLon">Center Coords</param>
    /// <returns>An Airport</returns>
    public static Airport DummyAirport( LatLon latLon )
    {
      Airport apt = new Airport {
        Ident = "@@@@",
        Region = "XX",
        IATA = "",
        Name = "Placeholder Airport",
        Coordinate = latLon,
      };

      return apt;
    }


    /// <summary>
    /// cTor: 
    /// </summary>
    public Airport( ) { }


    /// <summary>
    /// Clone Interface implementation
    /// </summary>
    public override object Clone( )
    {
      return this.Clone<Airport>( );
    }

  }
}
