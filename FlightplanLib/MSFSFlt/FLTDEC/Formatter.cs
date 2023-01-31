using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

using CoordLib;
using FlightplanLib.MS;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// Generic De-Serializer helpers
  /// </summary>
  internal class Formatter
  {
    #region STATIC TOOLS

    /// <summary>
    /// Returns the value in the string or NaN
    /// </summary>
    /// <param name="valueS">A value String</param>
    /// <returns>The value or NaN</returns>
    internal static double GetValue( string valueS )
    {
      if (double.TryParse( valueS, out double result )) return result;
      return double.NaN;
    }

    /// <summary>
    /// Convert a WorldPosition (LLA) to a LatLon
    /// </summary>
    /// <param name="worldPosition">An LLA string</param>
    /// <returns>A LatLon</returns>
    internal static LatLon ToLatLon( string worldPosition )
    {
      LatLon ll = LatLon.Empty;
      if (LLA.TryParseLLA( worldPosition, out var lat, out var lon, out var alt )) {
        ll = new LatLon( lat, lon, alt );
      }
      else {
        Console.WriteLine( $"Ini_File ERROR ToLatLon failed - pattern does not match\n{worldPosition}" );
      }
      return ll;
    }

    /// <summary>
    /// The B21 Soaring engine uses tagged names for the Task Management
    ///  [*]Name+Elev[|MaxAlt[/MinAlt]][xRadius]
    ///  1st * - Start of Task
    ///  2nd * - End of Task
    ///  Name  - the WP name
    ///  +     - Separator
    ///  Elev  - Waypoint Elevation  [ft}
    ///  |MaxAlt - Max alt of the gate [ft}
    ///  /MinAlt - Min alt of the gate [ft}
    ///  xRadius - Radius of the gate [meters]
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    internal static string CleanB21SoaringName( string b21name )
    {
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        if (match.Groups["name"].Success) {
          return match.Groups["name"].Value;
        }
      }
      // seems a regular name
      return b21name;
    }
    private readonly static Regex c_wpB21 =
      new Regex( @"^(?<start_end>\*)?(?<name>([^\+])*)(?<elevation>(\+|-)\d{1,5})(?<maxAlt>\|\d{1,5})?(?<minAlt>\/\d{1,5})?(?<radius>x\d{1,5})?" );

    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    internal static string GetB21SoaringDecoration( string b21name )
    {
      // decoration see above
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        var deco = "";
        if (match.Groups["start_end"].Success) { deco += "* "; }
        if (match.Groups["elevation"].Success) { deco += $"{match.Groups["elevation"].Value} ft "; }
        if (match.Groups["minAlt"].Success) { deco += $"(min {match.Groups["minAlt"].Value}) "; }
        if (match.Groups["maxAlt"].Success) { deco += $"(max {match.Groups["maxAlt"].Value}) "; }
        if (match.Groups["radius"].Success) { deco += $"(rad {match.Groups["radius"].Value} m)"; }
        return deco;
      }
      // seems a regular name
      return "";
    }


    private static readonly Regex c_latRE = new Regex( @"^(?<ns>N|S)(?<deg>\d{1,2}).\s(?<part>\d{1,2}\.\d{2})'" );
    private static readonly Regex c_lonRE = new Regex( @"^(?<ns>E|W)(?<deg>\d{1,3}).\s(?<part>\d{1,2}\.\d{2})'" );
    private static readonly Regex c_altRE = new Regex( @"^(?<alt>(\+|-)\d{6}\.\d{2})" );

    /// <summary>
    /// Convert from Lat string to double 
    /// N23° 23.23' or S33.12
    /// </summary>
    /// <param name="lat">Lat String</param>
    /// <returns></returns>
    public static double ToLat( string lat )
    {
      Match match = c_latRE.Match( lat );
      if (match.Success) {
        double l = double.Parse( match.Groups["deg"].Value );
        l += double.Parse( match.Groups["part"].Value ) / 60.0;
        l = (match.Groups["ns"].Value == "N") ? l : -l; // North South Deg
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Convert from Lon string to double 
    /// E23° 23.23' or W133.12
    /// </summary>
    /// <param name="lon">Lon String</param>
    /// <returns></returns>
    public static double ToLon( string lon )
    {
      Match match = c_lonRE.Match( lon );
      if (match.Success) {
        double l = double.Parse( match.Groups["deg"].Value );
        l += double.Parse( match.Groups["part"].Value ) / 60.0;
        l = (match.Groups["ns"].Value == "E") ? l : -l; // North South Deg
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Convert from Alt string to double 
    /// +123456.00 or -000006.00
    /// </summary>
    /// <param name="alt">Lon String</param>
    /// <returns></returns>
    public static double ToAlt( string alt )
    {
      Match match = c_altRE.Match( alt );
      if (match.Success) {
        double l = double.Parse( match.Groups["alt"].Value );
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Returns the Bool of the string (checks for True only, else it is False)
    /// </summary>
    /// <param name="bol">A Boolean string</param>
    /// <returns>The bool type</returns>
    public static bool ToBool( string bol )
    {
      return (bol.ToLowerInvariant( ) == "true");
    }

    /*
     
    ********************
    ****** WP FIELDS
    ****** (deducted) Parts of old: http://www.prepar3d.com/SDK/Mission%20Creation%20Kit/FlightFiles.htm
    ********************
    00: RegionCode (ICAO)  OR other 2 letters  '!A' / AA, AB .. for Missions
    01: Ident (ICAO)
    02: Airport (ICAO) if the WP belongs to an Airport and it's Approaches
    03: NameID (ICAO or User or MSFS provided) can be as long as TT:Nevada.Mission.180 (ref to a translated text)
    04: Type (one of: A : airport, I : intersection, V : VOR, N : NDB, U : user, T : ATC)
    05: Lat S33° 45.79' (non unicode chars ° ' )
    06: Lon E18° 33.52' (non unicode chars ° ' )
    07: Altitude  +003000.00  likely -000100.00 ??
    08: Airway ID	 (default = empty) The airway ID, such as J5
    09: Departure ID  (default = empty) ID
    10: Arrival ID (default = empty) ID
    11: Approach Type (default = empty) RNAV, ILS, VOR ..
    13: Rwy (default = empty) 14, 34 ..
    14: Rwy Designation (default = NONE) R, L, C, ..
    15: ApproachVariant (default = 0) (ILS RNAV Variant e.g. X, Y, Z etc.)
    16: Unk6 (default = 0) STILL NOT SEEN OR DECODED
    17: MaxSpeed (default = -1) Max Speed kt or -1 if none
    18: AltLimit1 (default = 0) an Altitude Limit or the lower Altitude Limit if B
    19: AltLimit2 (default = 0) an Altitude Limit or the higher Altitude Limit if B
    20: Alt Limit (blank, +, B, -) blank=NoLimit; +=Above AltLimit1; -=Below AltLimit1; B=Between AltLimit1 and AltLimit2

    */

    private readonly static Regex c_wpRE =
  new Regex( @"^(?<reg>(\!\w|\w{2}))?,\s*(?<ident>\w{2,5})?,\s*(?<apt>\w{2,4})?,\s*(?<name>([^,])*),\s*(?<typ>[A-Z]),\s*(?<lat>(N|S)\d{1,2}.\s\d{1,2}\.\d{2}'),\s*(?<lon>(E|W)\d{1,3}.\s\d{1,2}\.\d{2}'),\s*(?<alt>(\+|-)\d{6}\.\d{2}),\s*(?<awy>\w*)?,\s*(?<depId>\w*)?,\s*(?<arrId>\w*)?,\s*(?<aprT>\w*)?,\s*(?<rwy>\w*)?,\s*(?<rwyD>\w*)?,\s*(?<aprV>-?\w*),\s*(?<u6>-?\w*),\s*(?<speedL>-?\d*),\s*(?<altL1>-?\d*),\s*(?<altL2>-?\d*),\s*(?<altLT>.*)?" );

    // Waypoints to ignore while decoding
    private readonly static List<string> c_ignoreWP = new List<string>( ){
      "TIMECLIMB", "TIMECRUIS", "TIMEDSCNT", "TIMEAPPROACH", "TIMEVERT",
      /*"Apprch", "ClrApprch",*/ "EnRt",
    };

    /*
     There are MSFS:
        (T) Apprch 
        (T) ClrApprch
        (T) Runway => RWYNNx e.g. RW26L
        (U) EnRt
     */

    /// <summary>
    /// Decode one Waypoint Line to a Waypoint Object
    /// </summary>
    /// <param name="wpLine">A Waypoint Line</param>
    /// <returns>A new Waypoint or null on error/ignore</returns>
    public static Ini_Waypoint DecodeWaypoint( string wpLine )
    {
      var ret = new Ini_Waypoint( );
      Match match = c_wpRE.Match( wpLine );
      if (match.Success) {
        ret.Region = match.Groups["reg"].Success ? match.Groups["reg"].Value.Trim( ) : "";
        ret.ID = match.Groups["name"].Value.Trim( );
        ret.Name = CleanB21SoaringName( ret.ID ); // get a native name if it is decorated with B21 stuff

        // exclude MSFS ones that are not to be reported
        if (c_ignoreWP.Contains( ret.Name )) return null; // we don't collect this one

        if (string.IsNullOrEmpty( ret.Ident )) ret.Ident = ret.Name7; // fix missing Idents with the Name (cut)
        ret.Ident = ret.Ident.Trim( ).ToUpperInvariant( ); // case issues ..

        // waypoint type
        string t = match.Groups["typ"].Value.Trim( );
        switch (t) {
          case "A": ret.WaypointType = TypeOfWaypoint.Airport; break;
          case "I": ret.WaypointType = TypeOfWaypoint.Waypoint; break;
          case "V": ret.WaypointType = TypeOfWaypoint.VOR; break;
          case "N": ret.WaypointType = TypeOfWaypoint.NDB; break;
          case "U": ret.WaypointType = TypeOfWaypoint.User; break;
          case "T": ret.WaypointType = TypeOfWaypoint.ATC; break;
          default: ret.WaypointType = TypeOfWaypoint.Other; break;
        }
        // assign Runway an own type (is T from the file)
        if (ret.Name == "Runway") {
          ret.WaypointType = TypeOfWaypoint.Runway;
        }
        // Lat Lon, Alt etc.
        ret.LatLon = new LatLon( ToLat( match.Groups["lat"].Value ), ToLon( match.Groups["lon"].Value ), ToAlt( match.Groups["alt"].Value ) );
        ret.Airport = match.Groups["apt"].Value.Trim( );
        ret.Airway_Ident = match.Groups["awy"].Value.Trim( );
        // Eval the Runway ID as NNx (complete later to RWNNx) usually given in the Dest Airport - but not when the FP changed ?? MS BUG
        ret.RunwayNumber_S = $"{match.Groups["rwy"].Value}".Trim( );
        ret.RunwayDesignation += (match.Groups["rwyD"].Value.Trim( ) != "NONE") ? $"{match.Groups["rwyD"].Value.Trim( )}" : ""; // seems to be NONE if not used...

        ret.SID_Ident = match.Groups["depId"].Value.Trim( ); // and ID
        //if (!string.IsNullOrEmpty( ret.Departure )) ret.Departure += $"-{ret.Runway_Ident}";

        ret.STAR_Ident = match.Groups["arrId"].Value.Trim( ); // and ID
        //if (!string.IsNullOrEmpty( ret.Arrival )) ret.Arrival += $"-{ret.Runway_Ident}";

        ret.ApproachType = match.Groups["aprT"].Value.Trim( ); // RNAV, ILS ..
        ret.Approach_Suffix = (match.Groups["aprV"].Value.Trim( ) == "0") ? "" : match.Groups["aprV"].Value.Trim( ); // X Y Z ..  default is 0

        // Limits
        ret.SpeedLimit_kt = int.Parse( match.Groups["speedL"].Value ); // default -1
        ret.AltLimit1_ft = int.Parse( match.Groups["altL1"].Value );   // default 0
        ret.AltLimit2_ft = int.Parse( match.Groups["altL2"].Value );   // default 0
        // Altitude Limit type
        t = match.Groups["altLT"].Value;
        switch (t) {
          case "+": ret.AltLimit = AltLimitType.Above; break;
          case "-": ret.AltLimit = AltLimitType.Below; break;
          case "B": ret.AltLimit = AltLimitType.Between; break;
          default: ret.AltLimit = AltLimitType.NoLimit; break;
        }

        ret.IsValid = true;
        return ret;
      }
      // ERROR cannot decode
      Console.WriteLine( $"FLT-WYP Decoder - cannot decode:\n{wpLine}" );
      return null;
    }


    #endregion


#pragma warning disable CS0168 // Variable is declared but never used

    /// <summary>
    /// Reads from the open stream one entry
    /// </summary>
    /// <param name="iStream">An open stream at position</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromIniStream<T>( Stream iStream )
    {
      try {
        var iniSerializer = new IniSerializer( typeof( T ) );
        object objResponse = iniSerializer.Deserialize( iStream );
        var iniResults = (T)objResponse;
        iStream.Flush( );
        return iniResults;
      }
      catch (Exception e) {
        return default( T );
      }
    }

    /// <summary>
    /// Reads from the supplied string
    /// </summary>
    /// <param name="iString">A INI formatted string</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromIniString<T>( string iString )
    {
      try {
        T iniResults = default;
        using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( iString ) )) {
          iniResults = FromIniStream<T>( ms );
        }
        return iniResults;
      }
      catch (Exception e) {
        return default;
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// Tries to aquire a shared Read Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="iFilename">The Xml Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromIniFile<T>( string iFilename )
    {
      T retVal = default( T );
      if (!File.Exists( iFilename )) {
        return retVal;
      }

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        byte[] byt;
        try {
          using (var ts = File.Open( iFilename, FileMode.Open, FileAccess.Read, FileShare.Read )) {
            byt = new byte[ts.Length];
            ts.Read( byt, 0, byt.Length );
            var encoder = Encoding.GetEncoding( "iso-8859-1" ); // FLT INI File have this encoding
            var tmp = Encoding.UTF8.GetString( Encoding.Convert( encoder, Encoding.UTF8, byt ) );
            retVal = FromIniString<T>( tmp );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
        finally {
          byt = new byte[0]; // help the GC
        }
      }

      return retVal;
    }

#pragma warning restore CS0168 // Variable is declared but never used
  }
}
