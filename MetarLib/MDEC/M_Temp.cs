using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Temperature Dewpoint
  /// </summary>
  public class M_Temp : Chunk
  {
    /// <summary>
    /// Temperature in °C
    /// </summary>
    public int? Temperature = null;
    /// <summary>
    /// Dewpoint in °C
    /// </summary>
    public int? Dewpoint = null;

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Temp: {Temperature:#0}°C ({UnitC.FfromC( (float)Temperature ):#0}°F)" 
      + ((Dewpoint!=null)? $" - Dewpoint: {Dewpoint}°C ({UnitC.FfromC( (float)Dewpoint ):#0}°F) (RH={UnitC.RHfromTandD((float)Temperature, (float)Dewpoint ):#0}%)" : "");

  }

  // ******* DECODER CLASS

  internal static class M_TempDecoder
  {
    /*
     TEMP_RE = re.compile(
    r"""^(?P<temp>(M|-)?\d+|//|XX|MM)/
        (?P<dewpt>(M|-)?\d+|//|XX|MM)?\s+""",
    re.VERBOSE,*/
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<temp>(M|-)?\d+|//|XX|MM)/(?<dewpt>(M|-)?\d+|//|XX|MM)?)(?<rest>\s{1}.*)", RegexOptions.Compiled);

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="temp">The Temperature record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Temp temp )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          temp.Chunks += match.Groups["chunk"].Value;
          temp.Temperature = UnitC.FromTemp( match.Groups["temp"].Value );
          if ( match.Groups["dewpt"].Success )
            temp.Dewpoint = UnitC.FromTemp( match.Groups["dewpt"].Value );

          temp.Valid = true;
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
