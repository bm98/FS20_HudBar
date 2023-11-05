using CoordLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.Data.Text;

namespace FlightplanLib.Routes.RDEC
{
  internal class R_SpeedAlt : Word
  {
    public SpeedAltRemark SpeedAlt = new SpeedAltRemark( );

    public override bool IsValid => SpeedAlt.IsValid;

  }

  // ******* DECODER CLASS

  internal static class R_SpeedAltDecoder
  {
    // decoding a SpeedAlt sequence
    // {K|N|M}DDD[D]{A|F|M|S}DDD[D]
    private static Regex _X_spdAlt = new Regex( @"^(?<remark>(?<speed>[KMN][0-9]{3,4})(?<alt>[AFMS][0-9]{3,4}))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeSpeedAlt( string word, out string speed, out string alt )
    {
      speed = ""; alt = "";
      Match match = _X_spdAlt.Match( word );
      if (match.Success) {
        speed = match.Groups["speed"].Value;
        alt = match.Groups["alt"].Value;
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
      if (_X_spdAlt.Match( word ).Success) return true;

      return false;
    }

    /// <summary>
    /// Decode and return a SpeedAltRemark
    /// </summary>
    /// <param name="word">A Route Word</param>
    /// <returns>A SpeedAltRemark</returns>
    public static R_SpeedAlt Decode( string word )
    {
      if (IsMatch( word )) {
        return new R_SpeedAlt( ) { SpeedAlt = new SpeedAltRemark( word ) };
      }
      return new R_SpeedAlt( );
    }
  }

}
