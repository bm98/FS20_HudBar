using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DbgLib;

namespace FlightplanLib.SimBrief.Provider
{
  /// <summary>
  /// Form and issue a FlightPlan Request from SimBrief
  /// 
  /// https://forum.navigraph.com/t/fetching-a-users-latest-ofp-data/5297
  /// </summary>
  internal class SimBriefRequest
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );


    /*
      To download the XML data for a SimBrief user’s latest flight plan, please use the following endpoint:

      https://www.simbrief.com/api/xml.fetcher.php?userid=999999

      Where “userid” is the Pilot ID of the requested user. Your SimBrief Pilot ID can be found either by visiting the Account Settings 15 page, or by opening a New Flight 6, scrolling to the Optional Entries section, and checking the default value of the “Pilot ID” option.

      To retrieve flight data using a user’s username instead, use the following endpoint:

      https://www.simbrief.com/api/xml.fetcher.php?username={username}

      On success, these requests will return HTTP status code 200 and the XML data for the user’s most recent flight.

      If an invalid user is specified, or an error occurs, it will return HTTP status code 400 (Bad Request) and a small XML-formatted error message.

      Update: A JSON object can now be returned instead by appending &json=1 to the above URLs. For example:

      https://www.simbrief.com/api/xml.fetcher.php?username={username}&json=1
     

      SimBrief adds or modifies the available aircraft types and OFP layouts from time to time. Therefore, developers might want a way to automatically get an updated list of these options when integrating with SimBrief.

      The following URL returns a JSON object listing the currently supported aircraft types and OFP layouts on SimBrief:

      http://www.simbrief.com/api/inputs.list.json

      This file is updated every 5 minutes.

      An XML version of this data is now also available and can be found here:

      http://www.simbrief.com/api/inputs.list.xml

      Typical usage would be to download the file, parse the JSON/XML data, and use the resulting object to create your “type” and “planformat” options. This can also be done automatically every time your integration is loaded if need be.

     */
    private static readonly HttpClient httpClient = new HttpClient( );
    public static string ResponseRaw { get; set; } = "";
    public static DateTime ResponseTime { get; set; } = DateTime.Now;

    private const string c_serverURL = "https://www.simbrief.com/api/xml.fetcher.php";

    //    https://www.simbrief.com/api/xml.fetcher.php?userid=642100
    /// <summary>
    /// cTor: Init request
    /// </summary>
    static SimBriefRequest( )
    {
      httpClient.Timeout = new TimeSpan( 0, 0, 20 );
    }

    // 6 digit number string allow 2..7 as there is no spec about it (read also '5 or 6'...)
    private static Regex _sbUid = new Regex( @"^([0-9]{2,7})$", RegexOptions.Compiled );

    // true for a valid userID
    public static bool IsSimBriefUserID( string userID ) => _sbUid.Match( userID ).Success;


    /// <summary>
    /// Async Retrieve a SimBrief XML record
    /// </summary>
    /// <param name="userIDorName">The SimBrief Pilot ID or Name</param>
    /// <param name="dataFormat">SB data format to retrieve</param>
    /// <returns>An XML SimBriefDocument</returns>
    public static async Task<string> GetDocument( string userIDorName, SimBriefDataFormat dataFormat )
    {
      Uri uri;
      if (IsSimBriefUserID( userIDorName )) {
        uri = new Uri( $"{c_serverURL}?userid={userIDorName}" + ((dataFormat == SimBriefDataFormat.JSON) ? "&json=1" : "") );
      }
      else {
        // if it is not an ID, it might be a name...
        uri = new Uri( $"{c_serverURL}?username={userIDorName}" + ((dataFormat == SimBriefDataFormat.JSON) ? "&json=1" : "") );
      }

      //GET
      try {
        ResponseRaw = await httpClient.GetStringAsync( uri );
        ResponseTime = DateTime.Now;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception ex) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return ResponseRaw;
    }

    /// <summary>
    /// Downloads a file to a location
    /// </summary>
    /// <param name="remoteFile">The Remote URL</param>
    /// <param name="localDocName">The local document name</param>
    /// <param name="destPath">The local destination folder</param>
    /// <returns>True when successfull</returns>
    public static async Task<bool> DownloadFile( string remoteFile, string localDocName, string destPath )
    {
      Uri uri = new Uri( remoteFile );
      bool success = false;
      //GET
      try {
        var response = await httpClient.GetAsync( uri );
        using (var fs = new FileStream( Path.Combine( destPath, localDocName ), FileMode.CreateNew )) {
          await response.Content.CopyToAsync( fs );
        }
        success = true;
      }
      catch (Exception ex) {
        // dest may be locked when viewing
        LOG.LogException( "DownloadFile", ex, "Saving to file failed" );
        ; // DEBUG STOP
      }

      return success;
    }


#if DEBUG

    // Provide File Loading for Debug

    /// <summary>
    /// Async Retrieve a SB Json record as File
    /// </summary>
    /// <param name="fileName">The fully qualified filename</param>
    /// <returns>An JSon Document</returns>
    public static async Task<string> GetDocument( string fileName )
    {
      //GET
      try {
        using (var sr = new StreamReader( fileName )) {
          ResponseRaw = await sr.ReadToEndAsync( );
        }
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception ex) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return ResponseRaw;
    }

#endif
  }
}
