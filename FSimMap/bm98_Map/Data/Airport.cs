using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;
using CoordLib;

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
    /// Get: The Airport Elevation in Feet
    /// </summary>
    public float Elevation_ft => Tools.MeterToFeet( Elevation_m );

    /// <summary>
    /// Create a dummy Airport if none is available
    /// </summary>
    /// <param name="latLon">Center Coords</param>
    /// <returns>An Airport</returns>
    public static Airport DummyAirport( LatLon latLon )
    {
      Airport apt = new Airport {
        ICAO = "@@@@",
        IATA = "",
        Name = "Placeholder Airport",
        Lat = latLon.Lat,
        Lon = latLon.Lon,
        Elevation_m = (float)latLon.Altitude,


        HasRunwaysRelation = false,
        HasNavaidsRelation = false,
        HasCommsRelation = false,
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
