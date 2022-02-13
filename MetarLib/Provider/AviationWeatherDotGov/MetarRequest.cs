using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;

namespace MetarLib.Provider.AviationWeatherDotGov
{
  /// <summary>
  /// Form and issue a Metar Request from aviationweather.gov
  /// 
  /// https://aviationweather.gov/dataserver/example?datatype=metar
  /// </summary>
  internal class MetarRequest : RequestBaseMetar
  {
    // https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&hoursBeforeNow=3&mostRecent=true&stationString=LSZH

    private static readonly HttpClient httpClient = new HttpClient();

    private static readonly string HoursBefore = "3";   // reach of past data
    private static readonly string DataFormat = "csv"; // can be xml too
   // private static readonly string DataFields = "raw_text,latitude,longitude,elevation_m"; // CANNOT USE limited fields - the reply header and datalined don't match
    public static string ResponseRaw { get; set; } = "";
    public static DateTime ResponseTime { get; set; } = DateTime.Now;

    private const string c_serverURL = "https://aviationweather.gov/adds/dataserver_current/httpparam";

    /// <summary>
    /// cTor: Init request
    /// </summary>
    public MetarRequest( )
    {
      httpClient.Timeout = new TimeSpan( 0, 0, 10 );
    }


    /// <summary>
    /// Async Retrieve the most recent but not older than 3 hours METAR station record
    /// </summary>
    /// <param name="station">The ICAO Weather station ID</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( string station )
    {
      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURL}?"+
                          $"dataSource=metars&"+
                          $"requestType=retrieve&"+
                          $"format={DataFormat}&"+
                          //$"fields={DataFields}&"+
                          $"hoursBeforeNow={HoursBefore}&"+
                          $"mostRecentForEachStation=constraint&"+
                          $"stationString={station}");
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }

    /// <summary>
    /// Async Retrieve a METAR range record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( double lat, double lon, int range_StM )
    {
      // https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&radialDistance=20;-104.65,39.83&hoursBeforeNow=3

      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURL}?"+
                          $"dataSource=metars&"+
                          $"requestType=retrieve&"+
                          $"format={DataFormat}&"+
                          //$"fields={DataFields}&"+
                          $"hoursBeforeNow={HoursBefore}&"+
                          $"mostRecentForEachStation=constraint&"+
                          $"radialDistance={range_StM};{lon:##0.0000},{lat:##0.0000}");
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }

    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="toICAO">The destination Apt</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( double lat, double lon, string toICAO, int range_StM )
    {
      // https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&flightPath=100;-97.5,27.77;RJAA&hoursBeforeNow=3

      int degDist = range_StM / 10; // retrieve at 1/10 range scale
      degDist = ( degDist < 1 ) ? 1 : ( degDist > 89 ) ? 89 : degDist; // 0<d<90
      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURL}?"+
                          $"dataSource=metars&"+
                          $"requestType=retrieve&"+
                          $"format={DataFormat}&"+
                          //$"fields={DataFields}&"+
                          $"hoursBeforeNow={HoursBefore}&"+
                          $"mostRecentForEachStation=constraint&"+
                          $"minDegreeDistance={degDist}&"+
                          $"flightPath={range_StM};{lon:##0.0000},{lat:##0.0000};{toICAO}");
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }


    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="lat">Latitude</param>
    /// <param name="lon">Longitude</param>
    /// <param name="dLat">Destination Lat</param>
    /// <param name="dLon">Destination Lon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( double lat, double lon, double dLat, double dLon, int range_StM )
    {
      // https://aviationweather.gov/adds/dataserver_current/httpparam?dataSource=metars&requestType=retrieve&format=xml&flightPath=100;-97.5,27.77;RJAA&hoursBeforeNow=3

      int degDist = range_StM / 10; // retrieve at 1/10 range scale
      degDist = ( degDist < 1 ) ? 1 : ( degDist > 89 ) ? 89 : degDist; // 0<d<90
      // this should retrieve one most recent record dating back max N hours
      Uri uri = new Uri( $"{c_serverURL}?"+
                          $"dataSource=metars&"+
                          $"requestType=retrieve&"+
                          $"format={DataFormat}&"+
                          //$"fields={DataFields}&"+
                          $"hoursBeforeNow={HoursBefore}&"+
                          $"mostRecentForEachStation=constraint&"+
                          $"minDegreeDistance={degDist}&"+
                          $"flightPath={range_StM};{lon:##0.0000},{lat:##0.0000};{dLon:##0.0000},{dLat:##0.0000}"); // format is range;lon,lat;lon,lat;..
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }



    #region CSV Decoding

    private enum Fields // as of 20210724
    {
      raw_text=0,
      latitude,
      longitude,
      elevation_m,
    }


    /// <summary>
    /// Decodes a METAR message received as CSV
    /// Returns one or more MetarData Records as List
    /// </summary>
    /// <param name="csvData">The Metar Message</param>
    /// <returns>A list of MetarData</returns>
    internal static MetarTafDataList DecodeCSV( string csvData )
    {
      // arrives as 
      /*
        No errors
        No warnings
        3 ms
        data source=metars
        N results
        header line
        value line
     */
      var ret = new MetarTafDataList();

      if ( string.IsNullOrEmpty( csvData ) ) {
        var rec = new MetarTafData(); ret.Add( rec );
        rec.Error = $"Empty METAR record received";
        rec.Valid = false;
        return ret;
      }
      using ( var sr = new StringReader( csvData ) ) {
        try {
          var line = sr.ReadLine(); // errors
          if ( !line.StartsWith( "No errors" ) ) {
            var rec = new MetarTafData(); ret.Add( rec );
            rec.Error = line;
            rec.Valid = false;
            return ret;
          }
          line = sr.ReadLine( ); // warnings
          line = sr.ReadLine( ); // response time
          line = sr.ReadLine( ); // data sources
          line = sr.ReadLine( ); // N results
          string[] e = line.Split(new char[]{ ' ' } ); // get the N 
          if ( !int.TryParse( e[0], out int nRec ) ) {
            var rec = new MetarTafData(); ret.Add( rec );
            rec.Error = $"METAR cannot derive number of records\n{line}";
            rec.Valid = false;
            return ret;
          }
          if ( nRec <= 0 ) {
            var rec = new MetarTafData(); ret.Add( rec );
            rec.Error = $"METAR contains 0 records (not a known station?)\n{line}";
            rec.Valid = false;
            return ret;
          }

          line = sr.ReadLine( ); // headers
          if ( !line.StartsWith( $"{Fields.raw_text}," ) ) {
            var rec = new MetarTafData(); ret.Add( rec );
            rec.Error = $"Unrecognizable METAR record header format\n{line}";
            rec.Valid = false;
            return ret;
          }
          // Create an item Lookup table
          var hList = line.Split(new char[]{ ','} ).ToList(); // the list of items from the header
          var lookup = CreateLookup(line); // a lookup where the value is the item index in the list (and CSV line)
          if ( lookup.Count == 0 ) {
            var rec = new MetarTafData(); ret.Add( rec );
            rec.Error = $"Field(s) not found in METAR record header format\n{line}";
            rec.Valid = false;
            return ret;
          }

          // get all station reports
          for ( int i = 0; i < nRec; i++ ) {
            line = sr.ReadLine( ); // result line
            ret.Add( DecodeCSVLine( line, lookup ) );
          }

        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
          var rec = new MetarTafData(); ret.Add( rec );
          rec.Error = $"Unrecognizable METAR record\n{csvData}";
          rec.Valid = false;
        }

        return ret;
      }
    }

    /// <summary>
    /// Decode one report line
    /// </summary>
    /// <param name="csvLine">The Station report line</param>
    /// <param name="lookup">An item lookup table</param>
    /// <returns>A MetarData record</returns>
    private static MetarTafData DecodeCSVLine( string csvLine, IDictionary<Fields, int> lookup )
    {
      var rec = new MetarTafData();
      try {
        string[] e = csvLine.Split(new char[]{ ','} );

        rec.RAW = e[lookup[Fields.raw_text]].Trim( );
        rec.Data = MDEC.MTData.Decode( rec.RAW );
        if ( double.TryParse( e[lookup[Fields.latitude]], out double dData ) ) rec.Lat = dData;
        if ( double.TryParse( e[lookup[Fields.longitude]], out dData ) ) rec.Lon = dData;
        if ( float.TryParse( e[lookup[Fields.elevation_m]], out float fData ) ) rec.Elevation_m = fData;
        rec.Valid = true;

      }
      catch {
        rec.Error = $"Unrecognizable METAR record\n{csvLine}";
        rec.Valid = false;
      }
      return rec;
    }

    /// <summary>
    /// Create a Lookup Dict for the fields we are interested in
    /// </summary>
    /// <param name="header">The header Line</param>
    /// <returns>A Lookup Table</returns>
    private static Dictionary<Fields, int> CreateLookup( string header )
    {
      var ret = new Dictionary<Fields,int>();

      try {
        var hList = header.Split(new char[]{ ','} ).ToList(); // the list of items from the header
        // add fields we need
        var f = Fields.raw_text; var l = hList.IndexOf(f.ToString()); ret.Add( f, l );
        f = Fields.longitude; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
        f = Fields.latitude; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
        f = Fields.elevation_m; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        // most likely the IndexOf Failed if the field was not found..
        // return an empty Lookup
        ret = new Dictionary<Fields, int>( );
      }

      return ret;
    }



    #endregion



  }
}
