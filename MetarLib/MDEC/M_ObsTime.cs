using System;
using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Report Observation 
  /// </summary>
  public class M_ObsTime : Chunk
  {
    /// <summary>
    /// Observation Time of the record
    /// </summary>
    public DateTime ObsTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "No Report date" :
      $"Report Date {ObsTime:dd}. Time {ObsTime.TimeOfDay:hh\\:mm}Z";
  }

  // ******* DECODER CLASS

  internal static class M_ObsTimeDecoder
  {
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<day>[0-3][0-9])(?<hour>[0-2][0-9])(?<min>[0-6][0-9])(Z))(?<rest>\s{1}.*)", RegexOptions.Compiled); // 312315Z

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="obsTime">The ObsTime record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_ObsTime obsTime )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          obsTime.Chunks += match.Groups["chunk"].Value;
          int day = int.Parse( match.Groups["day"].Value);
          int zHour = int.Parse( match.Groups["hour"].Value);
          int zMin = int.Parse( match.Groups["min"].Value);
          DateTime dateTime = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, zMin, 0, DateTimeKind.Utc);
          obsTime.ObsTime = dateTime;
          obsTime.Valid = true;
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
