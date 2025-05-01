using System;
using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// TAF Validity Period
  /// </summary>
  public class T_Period : Chunk
  {
    /// <summary>
    /// Observation Time of the record
    /// </summary>
    public DateTime From { get; set; } = DateTime.Now;
    /// <summary>
    /// Observation Time End of the record
    /// </summary>
    public DateTime To { get; set; } = DateTime.Now;

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "NO Period" :
      $"Forecast: {From.TimeOfDay:hh\\:mm}Z to {To.TimeOfDay:hh\\:mm}Z - day {From.Day}";
  }

  // ******* DECODER CLASS

  internal static class T_PeriodDecoder
  {
    /*
     * 2718/2824 CHUNKS
     * DDHH/DDHH ..
     * 
     * or 
     * NIL
     */
    private static Regex RE_regular  = new Regex(@"^(?<chunk>NIL|(?<fDay>[0-3][0-9])(?<fHour>[0-2][0-9])/(?<tDay>[0-3][0-9])(?<tHour>[0-2][0-9]))(?<rest>\s{1}.*)", RegexOptions.Compiled);

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="period">The Period record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, T_Period period )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          period.Chunks += match.Groups["chunk"].Value;
          int fDay = int.Parse( match.Groups["fDay"].Value);
          int fzHour = int.Parse( match.Groups["fHour"].Value);
          period.From = new DateTime( DateTime.Now.Year, DateTime.Now.Month, fDay, fzHour, 0, 0, DateTimeKind.Utc );
          int tDay = int.Parse( match.Groups["tDay"].Value);
          int tzHour = int.Parse( match.Groups["tHour"].Value);
          if ( tzHour == 24 )
            period.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, tDay, 23, 59, 59, 999, DateTimeKind.Utc );
          else
            period.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, tDay, tzHour, 0, 0, DateTimeKind.Utc );
          if ( tDay < fDay )
            period.To.AddMonths( 1 ); // to day is < from day -> next month

          period.Valid = true;
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