using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{
  /// <summary>
  /// One TAF Forecast record 
  /// Used for the main and any further records (FM... ... ...)
  /// </summary>
  public class T_Forecast : Chunk
  {
    /// <summary>
    /// Forecast time
    /// </summary>
    public DateTime From { get; set; } = DateTime.Now;
    /// <summary>
    /// Forecast ending time
    /// </summary>
    public DateTime To { get; set; } = DateTime.Now;
    /// <summary>
    /// If the From To Time Span is used
    /// </summary>
    public bool IsTimeSpan { get; set; } = false;
    /// <summary>
    /// The Probability if given
    /// </summary>
    public float Probability { get; set; } = float.NaN;

    /// <summary>
    /// TEMPO flag
    /// </summary>
    public bool Tempo { get; set; } = false;
    /// <summary>
    /// BECMG flag
    /// </summary>
    public bool Becoming { get; set; } = false;

    // SAME AS METAR

    /// <summary>
    /// Wind Forecast Record
    /// </summary>
    public M_Wind Wind { get; set; } = new M_Wind( );
    /// <summary>
    /// Visibility Forecast record
    /// </summary>
    public M_Visibility Visibility { get; set; } = new M_Visibility( );
    /// <summary>
    /// Weather Forecast records
    /// </summary>
    public List<M_Weather> Weather { get; set; } = new List<M_Weather>( );
    /// <summary>
    /// Sky Condition records
    /// </summary>
    public List<M_SkyCondition> SkyConditions { get; set; } = new List<M_SkyCondition>( );
    /// <summary>
    /// The Flight Category
    /// </summary>
    public M_Category FlightCategory { get; set; } = new M_Category( );


    // TAF SPECIFIC 

    /// <summary>
    /// Temp Min and Max Forecast records
    /// </summary>
    public List<T_TempMinMax> TempMinMax { get; set; } = new List<T_TempMinMax>( );

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"\n"
      + ( !float.IsNaN( Probability ) ? $"{Probability:#0}% - " : "" )
      + ( Tempo ? "Temporary " : "" )
      + ( Becoming ? "Change " : "" )
      + ( IsTimeSpan ? $"From {From:dd}. {From:hh\\:mm}Z to {To:dd}. {To:hh\\:mm}Z" :
          $"From {From:dd}. {From:hh\\:mm}Z onwards" )
      + $"{FromTempMinMax( )}"
      ;

    private string FromTempMinMax( )
    {
      string ret = "";
      foreach ( var tt in TempMinMax ) {
        ret += $"\n{tt.Pretty}";
      }

      return ret;
    }
  }




  // ******* DECODER CLASS

  internal static class T_ForecastDecoder
  {
    /*
     FMddhhmm CHUNKS
    or
     PROBpp ddhh/ddhh CHUNKS
    or
     PROBpp TEMPO ddhh/ddhh CHUNKS
    or
     PROBpp BECMG ddhh/ddhh CHUNKS
    or 
     TEMPO ddhh/ddhh CHUNKS
    or 
     BECMG ddhh/ddhh CHUNKS
     */
    private static Regex RE_regular
      = new Regex(@"^(?<chunk>(?<fcast>FM(?<day>[0-3][0-9])(?<hour>[0-2][0-9])(?<min>[0-6][0-9])))(?<rest>\s{1}.*)", RegexOptions.Compiled);
    private static Regex RE_prob
      = new Regex(@"^(?<chunk>(?<fcast>PROB(?<prob>\d{2})\s((?<opt>TEMPO|BECMG)\s)?(?<fDay>[0-3][0-9])(?<fHour>[0-2][0-9])/(?<tDay>[0-3][0-9])(?<tHour>[0-2][0-9])))(?<rest>\s{1}.*)", RegexOptions.Compiled);
    private static Regex RE_tempo
      = new Regex(@"^(?<chunk>(TEMPO\s(?<fDay>[0-3][0-9])(?<fHour>[0-2][0-9])/(?<tDay>[0-3][0-9])(?<tHour>[0-2][0-9])))(?<rest>\s{1}.*)", RegexOptions.Compiled);
    private static Regex RE_becmg
      = new Regex(@"^(?<chunk>(BECMG\s(?<fDay>[0-3][0-9])(?<fHour>[0-2][0-9])/(?<tDay>[0-3][0-9])(?<tHour>[0-2][0-9])))(?<rest>\s{1}.*)", RegexOptions.Compiled);

    /// <summary>
    /// Test if the content matches
    /// </summary>
    /// <param name="raw">The raw message input string</param>
    /// <returns>True if it matches</returns>
    public static bool IsMatch( string raw )
    {
      if ( RE_regular.Match( raw ).Success ) return true;
      if ( RE_prob.Match( raw ).Success ) return true;
      if ( RE_tempo.Match( raw ).Success ) return true;
      if ( RE_becmg.Match( raw ).Success ) return true;

      return false;
    }


    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="forecasts">The Forecast List to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, List<T_Forecast> forecasts )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          var fc = new T_Forecast();
          fc.Chunks += match.Groups["chunk"].Value;
          int day = int.Parse(match.Groups["day"].Value);
          int zHour = int.Parse(match.Groups["hour"].Value);
          int zMin = int.Parse(match.Groups["min"].Value);
          fc.From = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, zMin, 0, DateTimeKind.Utc );
          fc.IsTimeSpan = false;
          fc.Valid = true;
          forecasts.Add( fc );
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_prob.Match( raw );
        if ( match.Success ) {
          var fc = new T_Forecast();
          fc.Chunks += match.Groups["chunk"].Value;
          fc.Probability = int.Parse( match.Groups["prob"].Value );
          fc.Tempo = match.Groups["opt"].Success && match.Groups["opt"].Value == "TEMPO";
          fc.Becoming = match.Groups["opt"].Success && match.Groups["opt"].Value == "BECMG";
          int day = int.Parse(match.Groups["fDay"].Value);
          int zHour = int.Parse(match.Groups["fHour"].Value);
          fc.From = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          day = int.Parse( match.Groups["tDay"].Value );
          zHour = int.Parse( match.Groups["tHour"].Value );
          if ( zHour == 24 )
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, 23, 59, 59, 999, DateTimeKind.Utc );
          else
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          fc.IsTimeSpan = true;
          fc.Valid = true;
          forecasts.Add( fc );
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_tempo.Match( raw );
        if ( match.Success ) {
          var fc = new T_Forecast();
          fc.Chunks += match.Groups["chunk"].Value;
          fc.Tempo = true;
          int day = int.Parse(match.Groups["fDay"].Value);
          int zHour = int.Parse(match.Groups["fHour"].Value);
          fc.From = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          day = int.Parse( match.Groups["tDay"].Value );
          zHour = int.Parse( match.Groups["tHour"].Value );
          if ( zHour == 24 )
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, 23, 59, 59, 999, DateTimeKind.Utc );
          else
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          fc.IsTimeSpan = true;
          fc.Valid = true;
          forecasts.Add( fc );
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_becmg.Match( raw );
        if ( match.Success ) {
          var fc = new T_Forecast();
          fc.Chunks += match.Groups["chunk"].Value;
          fc.Becoming = true;
          int day = int.Parse(match.Groups["fDay"].Value);
          int zHour = int.Parse(match.Groups["fHour"].Value);
          fc.From = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          day = int.Parse( match.Groups["tDay"].Value );
          zHour = int.Parse( match.Groups["tHour"].Value );
          if ( zHour == 24 )
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, 23, 59, 59, 999, DateTimeKind.Utc );
          else
            fc.To = new DateTime( DateTime.Now.Year, DateTime.Now.Month, day, zHour, 0, 0, DateTimeKind.Utc );
          fc.IsTimeSpan = true;
          fc.Valid = true;
          forecasts.Add( fc );
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
