using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// The Visibility part
  /// </summary>
  public class M_Visibility : Chunk
  {
    /// <summary>
    /// True for CAVOK condition
    /// </summary>
    public bool CAVOK { get; set; } = false;

    /// <summary>
    /// Flag if M sign was found
    /// </summary>
    public bool LessThanFlag { get; set; } = false;
    /// <summary>
    /// Flag if P sign was found
    /// </summary>
    public bool MoreThanFlag { get; set; } = false;
    /// <summary>
    /// Distance in SM
    /// </summary>
    public float Distance_SM { get; set; } = float.NaN;
    /// <summary>
    /// A wind direction if found
    /// </summary>
    public string Direction { get; set; } = "";


    private const string CAVOKtext = "Ceiling And Visibility OK";

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      "Visibility: " +
      (CAVOK ? CAVOKtext :
      LessThanFlag ? $"less than {Distance_SM:##0.0} SM ({UnitC.SMtoKM( Distance_SM ):##0.0} km)" :
      MoreThanFlag ? $"more than {Distance_SM:##0.0} SM ({UnitC.SMtoKM( Distance_SM ):##0.0} km)" :
      $"{Distance_SM:##0.0} SM ({UnitC.SMtoKM( Distance_SM ):##0.0} km)")
      + $" {Direction}";

  }

  // ******* DECODER CLASS

  internal static class M_VisibilityDecoder
  {
    /*
    "^(?P<vis> (?P<dist>(M|P)?\d\d\d\d|////) (?P<dir>[NSEW][EW]? | NDV)? | (?P<distu>(M|P)?(\d+|\d\d?/\d\d?|\d+\s+\d/\d) )  (?P<units>SM|KM|M|U) | CAVOK )\s+"
    */
    // [M|P]0012SM
    private static Regex RE_regular = new Regex( @"^(?<chunk>(?<mp>(M|P))?(?<dist>\d{4})(?<dir>[NSEW][EW]?|NDV)?(?<unit>SM|KM|M)?)(?<rest>\s{1}.*)",
      RegexOptions.Compiled );
    // [M|P]1 | 48/11 | 10 1/4  SM ..
    private static Regex RE_parts = new Regex( @"^(?<chunk>(?<mp>(M|P))?(?<distu>\d+|\d\d?/\d\d?|\d+\s+\d/\d)(?<unit>SM|KM|M))(?<rest>\s{1}.*)",
      RegexOptions.Compiled );
    // CAVOK
    private static Regex RE_cavok = new Regex( @"^(?<chunk>(CAVOK))(?<rest>\s{1}.*)",
      RegexOptions.Compiled );

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="visibility">The Visibility record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Visibility visibility )
    {
      try {
        Match match = RE_regular.Match( raw );
        if (match.Success) {
          visibility.Chunks += match.Groups["chunk"].Value;
          visibility.Distance_SM = UnitC.DistAsSM( int.Parse( match.Groups["dist"].Value ), match.Groups["unit"].Value );
          visibility.LessThanFlag = match.Groups["mp"].Success && match.Groups["mp"].Value == "M";
          visibility.MoreThanFlag = (match.Groups["mp"].Success && match.Groups["mp"].Value == "P") || (match.Groups["dist"].Value == "9999");
          visibility.Direction = match.Groups["dir"].Success ? match.Groups["dir"].Value : "";
          visibility.Valid = true;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_parts.Match( raw );
        if (match.Success) {
          visibility.Chunks += match.Groups["chunk"].Value;
          float val = UnitC.FromDistU( match.Groups["distu"].Value );
          visibility.Distance_SM = UnitC.DistAsSM( val, match.Groups["unit"].Value );
          visibility.LessThanFlag = match.Groups["mp"].Success && match.Groups["mp"].Value == "M";
          visibility.MoreThanFlag = match.Groups["mp"].Success && match.Groups["mp"].Value == "P";
          visibility.Direction = match.Groups["dir"].Success ? match.Groups["dir"].Value : "";
          visibility.Valid = true;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_cavok.Match( raw );
        if (match.Success) {
          visibility.Chunks += match.Groups["chunk"].Value;
          visibility.CAVOK = true;
          visibility.Valid = true;
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
