using System.Threading.Tasks;
using CoordLib;

namespace MetarLib.Provider
{
  /// <summary>
  /// The implemented Message Providers
  /// </summary>
  public enum Providers
  {
    /// <summary>
    /// METAR @ aviationweather.gov
    /// </summary>
    AviationWeatherDotGov = 0,
  }

  // *******************************

  /// <summary>
  /// The Base for METAR provider
  /// </summary>
  internal abstract class RequestBaseMetar
  {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    /// <summary>
    /// Async Retrieve a METAR station record
    /// </summary>
    /// <param name="station">The ICAO Weather station ID</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( string station ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a METAR range record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( LatLon latLon, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a METAR flightpath record 
    /// - forward facing detection window
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="dstLatLon">The destination LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( LatLon latLon, LatLon dstLatLon, int range_StM ) { return new MetarTafDataList( ); }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
  }



  /// <summary>
  /// The Base for TAF provider
  /// </summary>
  internal abstract class RequestBaseTaf
  {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    /// <summary>
    /// Async Retrieve a TAF station record
    /// </summary>
    /// <param name="station">The ICAO Weather station ID</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( string station ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a TAF range record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( LatLon latLon, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a TAF flightpath record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="dstLatLon">The destination LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( LatLon latLon, LatLon dstLatLon, int range_StM ) { return new MetarTafDataList( ); }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
  }


}
