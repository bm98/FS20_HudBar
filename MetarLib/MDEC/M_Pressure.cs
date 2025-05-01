using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// The Altimeter setting
  /// </summary>
  public class M_Pressure : Chunk
  {
    /// <summary>
    /// The Altimeter setting in InHg
    /// </summary>
    public float AltPressure_inHg { get; set; } = 0;
    /// <summary>
    /// The Altimeter setting in hPa
    /// </summary>
    public float AltPressure_hPa { get; set; } = 0;

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Altimeter: {AltPressure_inHg:#0.00} inHg - {AltPressure_hPa:###0} hPa";

  }

  // ******* DECODER CLASS

  internal static class M_PressureDecoder
  {
    /*
    "^(?P<unit>A|Q|QNH)?(?P<press>[\dO]{3,4}|////)(?P<unit2>INS)?\s+"
    */

    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<unit>A|Q|QNH)(?<press>\d{4})(?<unit2>INS)?)(?<rest>\s{1}.*)", RegexOptions.Compiled); // A2929 Q1003 QNH2992INS

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
    /// <param name="raw">The raw message input string</param>
    /// <param name="pressure">The Altimeter record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Pressure pressure )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          pressure.Chunks += match.Groups["chunk"].Value;
          pressure.AltPressure_inHg = UnitC.PressureAsInHg( int.Parse(match.Groups["press"].Value), 
                                                        match.Groups["unit"].Value, 
                                                        ( match.Groups["unit"].Success )? match.Groups["unit"].Value:"");
          pressure.AltPressure_hPa = UnitC.PressureAsHPa( int.Parse( match.Groups["press"].Value ),
                                                        match.Groups["unit"].Value,
                                                        ( match.Groups["unit"].Success ) ? match.Groups["unit"].Value : "" );
          pressure.Valid = true;
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