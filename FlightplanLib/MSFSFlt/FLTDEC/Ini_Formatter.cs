using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DbgLib;
using CoordLib;
using FSimFacilityIF;

using FlightplanLib.Flightplan;


namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// Generic De-Serializer helpers
  /// </summary>
  internal class Ini_Formatter
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #region STATIC TOOLS


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
    08: Enroute ID	 (default = empty) The airway ID, such as J5
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

    // WaypointCat to ignore while decoding
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
        ret.SourceIdent = match.Groups["name"].Value.Trim( );
        ret.Name = Formatter.CleanName( ret.SourceIdent ); // get a native name if it is decorated with B21 stuff

        // exclude MSFS ones that are not to be reported
        if (c_ignoreWP.Contains( ret.Name )) return null; // we don't collect this one

        // waypoint type
        string t = match.Groups["typ"].Value.Trim( );
        switch (t) {
          case "A": ret.WaypointType = WaypointTyp.APT; break;
          case "I": ret.WaypointType = WaypointTyp.WYP; break;
          case "V": ret.WaypointType = WaypointTyp.VOR; break;
          case "N": ret.WaypointType = WaypointTyp.NDB; break;
          case "U": ret.WaypointType =  WaypointTyp.USR; break;
          case "T": ret.WaypointType = (ret.Name.ToUpperInvariant( ) == "RUNWAY") ? WaypointTyp.RWY :WaypointTyp.ATC; break;
          default: ret.WaypointType = WaypointTyp.OTH; break;
        }
        // Lat Lon, Alt etc.
        ret.LatLon = new LatLon( ToLat( match.Groups["lat"].Value ), ToLon( match.Groups["lon"].Value ), ToAlt( match.Groups["alt"].Value ) );
        ret.Airport = match.Groups["apt"].Value.Trim( );
        ret.Airway_Ident = match.Groups["awy"].Value.Trim( );
        // Eval the Runway ID as NNx (complete later to RWNNx) usually given in the Dest Airport - but not when the FP changed ?? MS BUG
        ret.RwNumber_S = $"{match.Groups["rwy"].Value}".Trim( );
        ret.RwDesignation += (match.Groups["rwyD"].Value.Trim( ) != "NONE") ? $"{match.Groups["rwyD"].Value.Trim( )}" : ""; // seems to be NONE if not used...

        ret.SID_Ident = match.Groups["depId"].Value.Trim( ); // and ID
        //if (!string.IsNullOrEmpty( ret.Departure )) ret.Departure += $"-{ret.Runway_Ident}";

        ret.STAR_Ident = match.Groups["arrId"].Value.Trim( ); // and ID
        //if (!string.IsNullOrEmpty( ret.Arrival )) ret.Arrival += $"-{ret.Runway_Ident}";

        ret.ApproachType = match.Groups["aprT"].Value.Trim( ); // RNAV, ILS ..
        ret.ApproachSuffix = (match.Groups["aprV"].Value.Trim( ) == "0") ? "" : match.Groups["aprV"].Value.Trim( ); // X Y Z ..  default is 0

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

        // There are 'unknown' which derive from Navaids not in the MS database (outdated ones)
        // also they are set to N90 W180 (but alt remains..)
        ret.IsValid = !(ret.SourceIdent == "unknown" || (ret.LatLon.Lat == 90.0 && ret.LatLon.Lon == -180.0));
        ;
        return ret;
      }
      // ERROR cannot decode
      LOG.LogError( "DecodeWaypoint", $"Cannot decode:\n{wpLine}" );
      return null;
    }

    #endregion

  }
}
