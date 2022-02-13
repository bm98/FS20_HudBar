using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib
{
  /// <summary>
  /// The METAR / TAF Data Record
  /// </summary>
  public class MetarTafData
  {
    /// <summary>
    /// Valid Data Flag
    /// </summary>
    public bool Valid { get; set; } = false;

    /// <summary>
    /// Error String (ignore if Valid)
    /// </summary>
    public string Error { get; set; } = "";

    /// <summary>
    /// RAW METAR/TAF String
    /// </summary>
    public string RAW { get; set; } = "";

    /// <summary>
    /// The Decoded METAR/TAF Data
    /// </summary>
    public MDEC.MTData Data { get; internal set; } = new MDEC.MTData( );

    /// <summary>
    /// If provided - Latitude of the Station
    /// </summary>
    public double Lat { get; internal set; } = double.NaN;
    /// <summary>
    /// If provided - Longitude of the Station
    /// </summary>
    public double Lon { get; internal set; } = double.NaN;
    /// <summary>
    /// If provided - Elevation of the Station
    /// </summary>
    public float Elevation_m { get; internal set; } = float.NaN;

    /// <summary>
    /// If provided - Distance to the Station
    /// will be evaluated on the fly
    /// </summary>
    public float Distance_nm { get; internal set; } = float.NaN;
    /// <summary>
    /// If provided - Bearing to the Station
    /// will be evaluated on the fly
    /// </summary>
    public float Bearing_deg { get; internal set; } = float.NaN;

    /// <summary>
    /// Returns a pretty string of the data (with line breaks)
    /// </summary>
    public string Pretty =>
      ( float.IsNaN( Distance_nm ) ? "" : $"Station: {Distance_nm:##0.0} nm @ {Bearing_deg:000}°\n" )
       + Data.Pretty;


    /// <summary>
    /// Evaluate Distance and Bearing from the given location
    /// </summary>
    /// <param name="lat">Lat of current location</param>
    /// <param name="lon">Lon of current location</param>
    public void Evaluate( double lat, double lon )
    {
      // reset validity
      Distance_nm = float.NaN;
      Bearing_deg = float.NaN;

      // Sanity checks
      if ( !Valid ) return; // cannot calc
      if ( double.IsNaN( Lat ) ) return; // cannot calc
      if ( double.IsNaN( Lon ) ) return; // cannot calc

      Distance_nm = (float)CoordLib.Geo.DistanceTo( lat, lon, Lat, Lon, CoordLib.ConvConsts.EarthRadiusNm );
      Bearing_deg = (float)CoordLib.Geo.BearingTo( lat, lon, Lat, Lon );
    }

    /// <summary>
    /// Evaluate Distance and Bearing from the given location
    /// </summary>
    /// <param name="latLon">LatLon of current location</param>
    public void Evaluate( CoordLib.LatLon latLon )
    {
      Evaluate( latLon.Lat, latLon.Lon ); // use the built in
    }


  }
}
