using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

using CoordLib;

namespace MetarLib.Provider.AviationWeatherDotGov
{
  /// <summary>
  /// Form and issue a Metar Request from aviationweather.gov
  /// 
  /// https://aviationweather.gov/dataserver/example?datatype=metar
  /// </summary>
  internal class MetarRequest : RequestBaseMetar
  {
    /*
    Original pre Oct 2023
    Update Oct 2023 for new API..
      
    may convert to JSON or XML later... TODO
      Query for metar KMCI 3hours back
        https://aviationweather.gov/api/data/metar?ids=KMCI&format=xml&hours=3
      Query for metar KMCI 3hours back =date as of now XML
        https://aviationweather.gov/api/data/metar?ids=KMCI&format=xml&hours=3&date=2023-10-27T07%3A20%3A16Z
      Query for metar KMCI 3hours back =date as of now JSON     
        https://aviationweather.gov/api/data/metar?ids=KMCI&format=json&hours=3

      Query for metar KORD with bounding box (40,-90,45,-85) 3hours back =date as of now JSON     
        https://aviationweather.gov/api/data/metar?ids=KORD&format=json&taf=false&hours=3&bbox=40%2C-90%2C45%2C-85


      Query for metar LSZH + TAF 3hours back =date as of now JSON     
        https://aviationweather.gov/api/data/metar?ids=LSZH&format=json&taf=true&hours=3
     
     * NOV 2025 Data API (JSON)
     // airport 
     https://aviationweather.gov/api/data/metar?ids=KMCI&format=json&taf=false&hours=3
     // bounding box Geographic bounding box (lat0, lon0, lat1, lon1) 40,-90,45,-85
     https://aviationweather.gov/api/data/metar?bbox=40%2C-90%2C45%2C-85&format=json&taf=false&hours=3


     */

    private static readonly HttpClient httpClient = new HttpClient( );

    private static readonly string HoursBefore = "3";   // reach of past data
    private static readonly string DataFormat = "csv"; // can be xml too
                                                       // private static readonly string DataFields = "raw_text,latitude,longitude,elevation_m"; 
                                                       // CANNOT USE limited fields - the reply header and datalined don't match
    public static string ResponseRaw { get; set; } = "";
    public static DateTime ResponseTime { get; set; } = DateTime.Now;

    private const string c_serverURLdataserverAPI = "https://aviationweather.gov/api/data/dataserver?requestType=retrieve";
    private const string c_serverURLmetarAPI = "https://aviationweather.gov/api/data/metar";

    /// <summary>
    /// cTor: Init request
    /// </summary>
    public MetarRequest( )
    {
      httpClient.Timeout = new TimeSpan( 0, 0, 10 );
    }

    // returns a bounding box for data
    private static string BBoxString( LatLon latLon, double rad_statM )
    {
      var llTopLeft = latLon.DestinationPoint( rad_statM, 270 + 45, ConvConsts.EarthRadiusSM );
      var llBottomRight = latLon.DestinationPoint( rad_statM, 90 + 45, ConvConsts.EarthRadiusSM );
      // format is:  (lat0, lon0, lat1, lon1)
      string fmt = $"{llTopLeft.Lat:0.0}%2C{llTopLeft.Lon:0.0}%2C{llBottomRight.Lat:0.0}%2C{llBottomRight.Lon:0.0}";
      return fmt;
    }

    /// <summary>
    /// Async Retrieve the most recent but not older than 3 hours METAR station record
    /// </summary>
    /// <param name="station">The ICAO Weather station ID</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( string station )
    {
      // NOV 2025 Data API( JSON)
      // airport 
      // https://aviationweather.gov/api/data/metar?ids=KMCI&format=json&taf=false&hours=3

      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURLmetarAPI}" +
                          $"?ids={station}" +
                          $"&format=json" +
                          $"&taf=false" +
                          $"&hours={HoursBefore}" );
      //GET
      try {
        // FOR NOW PROVIDE A FIXED STRING
        ResponseRaw = await httpClient.GetStringAsync( uri );
        //ResponseRaw = "[{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T16:22:23.499Z\",\"obsTime\":1763050800,\"reportTime\":\"2025-11-13T16:20:00.000Z\",\"temp\":9,\"dewp\":6,\"wdir\":310,\"wspd\":4,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131620Z 31004KT CAVOK 09/06 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"},{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T15:52:30.938Z\",\"obsTime\":1763049000,\"reportTime\":\"2025-11-13T16:00:00.000Z\",\"temp\":11,\"dewp\":8,\"wdir\":320,\"wspd\":3,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131550Z 32003KT CAVOK 11/08 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"},{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T15:22:23.019Z\",\"obsTime\":1763047200,\"reportTime\":\"2025-11-13T15:20:00.000Z\",\"temp\":14,\"dewp\":8,\"wdir\":320,\"wspd\":3,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131520Z 32003KT CAVOK 14/08 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"},{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T14:52:12.903Z\",\"obsTime\":1763045400,\"reportTime\":\"2025-11-13T15:00:00.000Z\",\"temp\":18,\"dewp\":7,\"wdir\":\"VRB\",\"wspd\":2,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131450Z VRB02KT CAVOK 18/07 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"},{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T14:22:25.877Z\",\"obsTime\":1763043600,\"reportTime\":\"2025-11-13T14:20:00.000Z\",\"temp\":18,\"dewp\":6,\"wdir\":\"VRB\",\"wspd\":1,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131420Z VRB01KT CAVOK 18/06 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"},{\"icaoId\":\"LSZH\",\"receiptTime\":\"2025-11-13T13:52:16.366Z\",\"obsTime\":1763041800,\"reportTime\":\"2025-11-13T14:00:00.000Z\",\"temp\":18,\"dewp\":6,\"wdir\":\"VRB\",\"wspd\":2,\"visib\":\"6+\",\"altim\":1017,\"qcField\":16,\"metarType\":\"METAR\",\"rawOb\":\"METAR LSZH 131350Z VRB02KT CAVOK 18/06 Q1017 NOSIG\",\"lat\":47.48,\"lon\":8.536,\"elev\":424,\"name\":\"Zürich Intl Arpt, ZH, CH\",\"cover\":\"CAVOK\",\"clouds\":[],\"fltCat\":\"VFR\"}]";
        ;
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeJSON( ResponseRaw );
    }

    /*
    /// <summary>
    /// Async Retrieve a METAR range record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( LatLon latLon, int range_StM )
    {
      // NOV 2025 Data API( JSON)
      // bounding box Geographic bounding box (lat0, lon0, lat1, lon1) 40,-90,45,-85
      // https://aviationweather.gov/api/data/metar?bbox=40%2C-90%2C45%2C-85&format=json&taf=false&hours=3

      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURLmetarAPI}" +
                          $"?bbox={BBoxString( new CoordLib.LatLon( lat, lon ), range_StM )}" +
                          $"&format=json" +
                          $"&taf=false" +
                          $"&hours={HoursBefore}" );
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }
      return DecodeCSV( ResponseRaw );
    }
    */

    /*
    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="toICAO">The destination Apt</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( LatLon latLon, string toICAO, int range_StM )
    {
      // NOV 2025 Data API( JSON)
      // bounding box Geographic bounding box (lat0, lon0, lat1, lon1) 40,-90,45,-85
      // https://aviationweather.gov/api/data/metar?bbox=40%2C-90%2C45%2C-85&format=json&taf=false&hours=3

      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURLmetarAPI}" +
                          $"?bbox={BBoxString( new CoordLib.LatLon( lat, lon ), range_StM )}" +
                          $"&format=json" +
                          $"&taf=false" +
                          $"&hours={HoursBefore}" );
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }
    */

    /*
    /// <summary>
    /// Async Retrieve the most recent but not older than 3 hours METAR station record
    /// </summary>
    /// <param name="station">The ICAO Weather station ID</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( string station )
    {
      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURLdataserverAPI}" +
                          $"&dataSource=metars" +
                          $"&requestType=retrieve" +
                          $"&format={DataFormat}" +
                          //$"fields={DataFields}&"+
                          $"&hoursBeforeNow={HoursBefore}" +
                          $"&mostRecentForEachStation=constraint" +
                          $"&stationString={station}" );
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }
    */

    /// <summary>
    /// Async Retrieve a METAR range record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( LatLon latLon, int range_StM )
    {
      // this should retrieve one most recent record dating back max 3 hours
      Uri uri = new Uri( $"{c_serverURLdataserverAPI}" +
                          $"&dataSource=metars" +
                          $"&format={DataFormat}" +
                          $"&hoursBeforeNow={HoursBefore}" +
                          $"&mostRecentForEachStation=constraint" +
                          $"&boundingBox={BBoxString( latLon, range_StM )}" );
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }

    /// <summary>
    /// Async Retrieve a METAR flightpath record
    /// </summary>
    /// <param name="latLon">Location LatLon</param>
    /// <param name="dstLatLon">The destination LatLon</param>
    /// <param name="range_StM">Range (Statute Miles)</param>
    /// <returns>A MetarDataList</returns>
    new public static async Task<MetarTafDataList> GetMetar( LatLon latLon, LatLon dstLatLon, int range_StM )
    {
      var brg = latLon.BearingTo( dstLatLon );
      var pathLatLon = latLon.DestinationPoint( range_StM, brg, ConvConsts.EarthRadiusSM );
      // this should retrieve one most recent record dating back max N hours
      Uri uri = new Uri( $"{c_serverURLdataserverAPI}" +
                          $"&dataSource=metars" +
                          $"&format={DataFormat}" +
                          $"&hoursBeforeNow={HoursBefore}" +
                          $"&mostRecentForEachStation=constraint" +
                          $"&boundingBox={BBoxString( pathLatLon, range_StM )}" );
      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return DecodeCSV( ResponseRaw );
    }

    #region JSON Decoding

    /// <summary>
    /// Decodes a METAR message received as JSON
    /// Returns one or more MetarData Records as List
    /// </summary>
    /// <param name="jsonData">The Metar Message</param>
    /// <returns>A list of MetarData</returns>
    internal static MetarTafDataList DecodeJSON( string jsonData )
    {
      var ret = new MetarTafDataList( );

      if (string.IsNullOrEmpty( jsonData )) return ret;

      JMDEC.J_Metar jMetar = Formatter.FromJsonString<JMDEC.J_Metar>( "{\"list\":" + jsonData + "}" ); // patch as named obj
      if (jMetar != null) {
        if (jMetar.MetarList.Length > 0) {
          foreach (var entry in jMetar.MetarList) {
            var rec = new MetarTafData( ); ret.Add( rec );

            rec.RAW = entry.RawString;
            rec.Lat = entry.Lat;
            rec.Lon = entry.Lon;
            rec.Elevation_m = entry.Elev_m;
            rec.Data = MDEC.MTData.Decode( rec.RAW );
            rec.Valid = true;
          }
        }
        else {
          var rec = new MetarTafData( ); ret.Add( rec );
          rec.Error = $"METAR no records received";
          rec.Valid = false;
        }
      }
      else {
        var rec = new MetarTafData( ); ret.Add( rec );
        rec.Error = $"METAR no valid response received";
        rec.Valid = false;
      }

      return ret;
    }

    #endregion

    #region CSV Decoding

    private enum Fields // as of 20210724
    {
      raw_text = 0,
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
      /* NOV 2025
        header line
        value line
     */
      var ret = new MetarTafDataList( );

      if (string.IsNullOrEmpty( csvData )) {
        var rec = new MetarTafData( ); ret.Add( rec );
        rec.Error = $"Empty METAR record received";
        rec.Valid = false;
        return ret;
      }
      using (var sr = new StringReader( csvData )) {
        try {
          string line;
          int nRec = csvData.Count( c => c == '\n' );  // get the N 
          if (nRec <= 0) {
            var rec = new MetarTafData( ); ret.Add( rec );
            rec.Error = $"METAR contains 0 records (not a known station?)";
            rec.Valid = false;
            return ret;
          }

          line = sr.ReadLine( ); // headers
          line = line.Replace( "\"", "" ); // must unquote
          if (!line.StartsWith( $"{Fields.raw_text}," )) {
            var rec = new MetarTafData( ); ret.Add( rec );
            rec.Error = $"Unrecognizable METAR record header format\n{line}";
            rec.Valid = false;
            return ret;
          }
          // Create an item Lookup table
          var hList = line.Split( new char[] { ',' } ).ToList( ); // the list of items from the header
          var lookup = CreateLookup( line ); // a lookup where the value is the item index in the list (and CSV line)
          if (lookup.Count == 0) {
            var rec = new MetarTafData( ); ret.Add( rec );
            rec.Error = $"Field(s) not found in METAR record header format\n{line}";
            rec.Valid = false;
            return ret;
          }

          // get all station reports
          for (int i = 0; i < nRec; i++) {
            line = sr.ReadLine( ); // result line
            line = line.Replace( "\"", "" ); // must unquote
            ret.Add( DecodeCSVLine( line, lookup ) );
          }

        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
          var rec = new MetarTafData( ); ret.Add( rec );
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
      var rec = new MetarTafData( );
      try {
        string[] e = csvLine.Split( new char[] { ',' } );

        rec.RAW = e[lookup[Fields.raw_text]].Trim( );
        rec.Data = MDEC.MTData.Decode( rec.RAW );
        if (double.TryParse( e[lookup[Fields.latitude]], out double dData )) rec.Lat = dData;
        if (double.TryParse( e[lookup[Fields.longitude]], out dData )) rec.Lon = dData;
        if (float.TryParse( e[lookup[Fields.elevation_m]], out float fData )) rec.Elevation_m = fData;
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
      var ret = new Dictionary<Fields, int>( );

      try {
        var hList = header.Split( new char[] { ',' } ).ToList( ); // the list of items from the header
                                                                  // add fields we need
        var f = Fields.raw_text; var l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
        f = Fields.longitude; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
        f = Fields.latitude; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
        f = Fields.elevation_m; l = hList.IndexOf( f.ToString( ) ); ret.Add( f, l );
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
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
