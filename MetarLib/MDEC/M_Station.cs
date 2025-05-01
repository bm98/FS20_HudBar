using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// The reporting station
  /// </summary>
  public class M_Station : Chunk
  {
    /// <summary>
    /// The Station ID
    /// </summary>
    public string StationID { get; set; } = "";

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      StationID;

  }

  // ******* DECODER CLASS

  internal static class M_StationDecoder
  {
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<station>[A-Z][A-Z0-9]{3}))(?<rest>\s{1}.*)", RegexOptions.Compiled); // LSZH

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="station">The Station record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Station station )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          station.Chunks += match.Groups["chunk"].Value;
          station.StationID = match.Groups["station"].Value;
          station.Valid = true;
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
