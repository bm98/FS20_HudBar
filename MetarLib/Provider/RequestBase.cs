using System.Threading.Tasks;


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
    AviationWeatherDotGov=0,
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
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( double lat, double lon, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="toICAO">The destination Apt</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( double lat, double lon, string toICAO, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="dLat">Destination Lat</param>
    /// <param name="dLon">Destination Lon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetMetar( double lat, double lon, double dLat, double dLon, int range_StM ) { return new MetarTafDataList( ); }


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
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( double lat, double lon, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a TAF flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="toICAO">The destination Apt</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( double lat, double lon, string toICAO, int range_StM ) { return new MetarTafDataList( ); }

    /// <summary>
    /// Async Retrieve a TAF flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="dLat">Destination Lat</param>
    /// <param name="dLon">Destination Lon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    public static async Task<MetarTafDataList> GetTaf( double lat, double lon, double dLat, double dLon, int range_StM ) { return new MetarTafDataList( ); }


#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
  }


}
