using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Temperature Min Max
  /// </summary>
  public class T_TempMinMax : Chunk
  {
    /// <summary>
    /// The Min or Max Temp
    /// </summary>
    public int? Temperature { get; set; } = null;
    /// <summary>
    /// Minimum Flag
    /// </summary>
    public bool IsMin { get; set; } = false;
    /// <summary>
    /// Maximum Flag
    /// </summary>
    public bool IsMax { get; set; } = false;
    /// <summary>
    /// The Day and Time of the forecast
    /// </summary>
    public DateTime At { get; set; } = DateTime.Now;

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
        !Valid ? "" :
        $"T"
        + ( IsMax ? $"-Maximum {Temperature:#0}°C ({UnitC.FfromC( (float)Temperature ):#0}°F) at {At:hh\\:mm}" : "" )
        + ( IsMin ? $"-Minimum {Temperature:#0}°C ({UnitC.FfromC( (float)Temperature ):#0}°F) at {At:hh\\:mm}" : "" );


  }


  // ******* DECODER CLASS

  internal static class T_TempMinMaxDecoder
  {
    /*
     TX02/2316Z CHUNKS
    or
    TNM16/1401Z CHUNKS

     Tk[M]tt/ddhhZ (tt must be 2 digits with preceeding 0 if needed)
     */
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<flag>TX|TN)(?<temp>(M|-)?\d{2})/(?<day>[0-3][0-9])(?<hour>[0-2][0-9])Z)(?<rest>\s{1}.*)", RegexOptions.Compiled);


    /// <summary>
    /// Test if the content matches
    /// </summary>
    /// <param name="raw">The raw message input string</param>
    /// <returns>True if it matches</returns>
    public static bool IsMatch( string raw )
    {
      if ( RE_regular.Match( raw ).Success ) return true;

      return false;
    }


    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="minMax">The TempMinMax List to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, List<T_TempMinMax> minMax )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          var tmm = new T_TempMinMax();
          tmm.Chunks += match.Groups["chunk"].Value;
          tmm.Temperature = UnitC.FromTemp( match.Groups["temp"].Value );
          int day = int.Parse( match.Groups["day"].Value);
          int zHour = int.Parse( match.Groups["hour"].Value);
          tmm.At = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc);
          tmm.IsMax = match.Groups["flag"].Value == "TX";
          tmm.IsMin = match.Groups["flag"].Value == "TN";
          tmm.Valid = true;
          minMax.Add( tmm );
          return match.Groups["rest"].Value.TrimStart( );
        }
      }
      catch {
        ; // DEBUG STOP
      }

      return raw;
    }

  }

}

