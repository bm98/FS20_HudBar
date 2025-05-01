using System.Collections.Generic;
using System.Linq;

using CoordLib;

namespace MetarLib
{
  /// <summary>
  /// A list of METAR / TAF data records
  /// </summary>
  public class MetarTafDataList : List<MetarTafData>
  {

    /// <summary>
    /// Returns true if the list is valid (contains valid first record)
    /// </summary>
    public bool Valid {
      get {
        return ( this.Count > 0 && this.First( ).Valid );
      }
    }


    /// <summary>
    /// Get the station record
    /// </summary>
    /// <param name="station">The ICAO Station name</param>
    /// <returns>The station record</returns>
    public MetarTafData GetStation( string station )
    {
      MetarTafData named = new MetarTafData();
      // find and hold the closest
      foreach ( var md in this ) {
        if ( md.Valid ) {
          if ( md.Data.Station.StationID == station.Trim( ) ) {
            named = md;
            break; // found
          }
        }
      }
      return named;
    }

    /// <summary>
    /// Get the closest record vs. Pos from the list
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <returns>The closest record</returns>
    public MetarTafData GetClosest( double lat, double lon )
    {
      // shortcuts to not process a lot
      if ( this.Count <= 0 ) return new MetarTafData( );
      if ( this.Count == 1 ) {
        this.First( ).Evaluate( lat, lon );
        return this.First( );
      }

      // more than one - we have to measure them
      LatLon pos = new LatLon(lat, lon);
      MetarTafData closest = new MetarTafData();
      double dist = 1e10; // far away
      // find and hold the closest
      foreach ( var md in this ) {
        if ( md.Valid ) {
          md.Evaluate( lat, lon );
          if ( md.Distance_nm < dist ) {
            closest = md;
            dist = md.Distance_nm;
          }
        }
      }
      return closest;
    }

  }
}
