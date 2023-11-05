using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.Data.Text;

using CoordLib;

namespace FlightplanLib.Routes.RDEC
{
  /// <summary>
  /// A COORD entry
  /// </summary>
  internal class R_Coord : Word
  {
    /// <summary>
    /// The Coordinate (Lat,Lon)
    /// </summary>
    public LatLon Coord { get; set; } = LatLon.Empty;

    public override bool IsValid => !Coord.IsEmpty;

  }

  // ******* DECODER CLASS

  internal static class R_CoordDecoder
  {
    // decoding a Coordinate
    // DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W} 0..89°59'59" N/S 0..180°00'00" E/W
    private static Regex _X_coord = new Regex( @"^(?<coord>(?<lat>[0-8][0-9](([0-5][0-9])([0-5][0-9])?)?[NS])(?<lon>[0-1][0-9][0-9](([0-5][0-9])([0-5][0-9])?)?[EW]))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeCOORD( string word, out string lat, out string lon )
    {
      lat = ""; lon = "";
      Match match = _X_coord.Match( word );
      if (match.Success) {
        lat = match.Groups["lat"].Value;
        lon = match.Groups["lon"].Value;
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
      if (_X_coord.Match( word ).Success) return true;

      return false;
    }

    /// <summary>
    /// Decode and return a Coordinate
    /// </summary>
    /// <param name="word">A Route Word</param>
    /// <returns>A Coordinate</returns>
    public static R_Coord Decode( string word )
    {
      if (DecodeCOORD( word, out string latS, out string lonS )) {
        // a plain COORD entry
        var ll = Dms.ParseRouteCoord( latS + lonS );
        if (!ll.IsEmpty) {
          return new R_Coord( ) { Coord = ll };
        }
      }
      return new R_Coord( ) { Coord = LatLon.Empty };
    }

  }
}
