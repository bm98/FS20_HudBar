using CoordLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.RDEC
{
  internal class R_UserWyp : Word
  {
    /// <summary>
    /// The Coordinate (Lat,Lon)
    /// </summary>
    public LatLon Coord { get; set; } = LatLon.Empty;

    /// <summary>
    /// The Wyp Ident
    /// </summary>
    public string Ident { get; set; } = "";
    /// <summary>
    /// A SpeedAlt remark
    /// </summary>
    public SpeedAltRemark SpeedAlt = new SpeedAltRemark( );


    public override bool IsValid => !(string.IsNullOrEmpty( Ident ) || Coord.IsEmpty);

  }

  // ******* DECODER CLASS

  internal static class R_UserWypDecoder
  {
    // decoding a User Waypoint
    // DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W}.[0-9A-Z]1..7 {+ SpeedAlt} 0..89°59'59" N/S 0..180°00'00" E/W
    private static Regex _X_userWyp
      = new Regex( @"^(?<coord>(?<lat>[0-8][0-9](([0-5][0-9])([0-5][0-9])?)?[NS])(?<lon>[0-1][0-9][0-9](([0-5][0-9])([0-5][0-9])?)?[EW]))\.(?<id>[0-9A-Z]{1,7})(\/(?<remark>[KMN][0-9]{3,4}[AFMS][0-9]{3,4}))?$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeUSERWYP( string word, out string lat, out string lon, out string ident, out string speedAltRemark )
    {
      lat = ""; lon = ""; ident = ""; speedAltRemark = "";
      Match match = _X_userWyp.Match( word );
      if (match.Success) {
        lat = match.Groups["lat"].Value;
        lon = match.Groups["lon"].Value;
        ident = match.Groups["id"].Value;
        speedAltRemark = (match.Groups["remark"].Success) ? match.Groups["remark"].Value : "";
        return true;
      }
      return false;
    }

    /// <summary>
    /// Test if the content matches
    /// </summary>
    /// <param name="word">The raw message input string</param>
    /// <returns>True if it matches</returns>
    public static bool IsMatch( string word )
    {
      if (_X_userWyp.Match( word ).Success) return true;

      return false;
    }

    /// <summary>
    /// Decode and return a Coordinate
    /// </summary>
    /// <param name="word">A Route Word</param>
    /// <returns>A Coordinate</returns>
    public static R_UserWyp Decode( string word )
    {
      if (DecodeUSERWYP( word, out string latS, out string lonS, out string ident, out string saRemark )) {
        var ll = Dms.ParseRouteCoord( latS + lonS );
        if (!(ll.IsEmpty || string.IsNullOrEmpty( ident ))) {
          var ret = new R_UserWyp {
            Coord = ll,
            Ident = ident,
            SpeedAlt = new SpeedAltRemark( saRemark )
          };
          return ret;
        }
      }
      return new R_UserWyp( );
    }

  }
}
