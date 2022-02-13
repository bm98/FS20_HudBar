using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Runway visual range
  /// </summary>
  public class M_RunwayVR : Chunk
  {
    /// <summary>
    /// A Runway ID
    /// </summary>
    public string RunwayID { get; set; } = "";
    /// <summary>
    /// Flag if M sign was found for Low part
    /// </summary>
    public bool LoLessThanFlag { get; set; } = false;
    /// <summary>
    /// Flag if P sign was found for Low part
    /// </summary>
    public bool LoMoreThanFlag { get; set; } = false;
    /// <summary>
    /// Distance of the Low part
    /// </summary>
    public float LoDistance_ft { get; set; } = float.NaN;

    /// <summary>
    /// Flag if M sign was found for High part
    /// </summary>
    public bool HiLessThanFlag { get; set; } = false;
    /// <summary>
    /// Flag if P sign was found for High part
    /// </summary>
    public bool HiMoreThanFlag { get; set; } = false;
    /// <summary>
    /// Distance of the High part
    /// </summary>
    public float HiDistance_ft { get; set; } = float.NaN;

    /// <summary>
    /// Trend signal
    /// </summary>
    public string Trend { get; set; } = ""; // N, D, U

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Runway: {RunwayID}"
      + $"{FromLo()}{FromHi( )}{FromTrend()}"
      ;

    private string FromLo( )
    {
      if ( float.IsNaN( LoDistance_ft ) ) return "";
      string tag = float.IsNaN(HiDistance_ft)? "" : " low"; // low if there is high..
      return
        LoLessThanFlag ? $" vis{tag} < {LoDistance_ft:#,##0}ft" :
        LoMoreThanFlag ? $" vis{tag} > {LoDistance_ft:#,##0}ft" :
        $" vis{tag} {LoDistance_ft:#,##0}ft";
    }
    private string FromHi( )
    {
      if ( float.IsNaN( HiDistance_ft ) ) return "";
      return
        HiLessThanFlag ? $" high < {HiDistance_ft:#,##0}ft" :
        HiMoreThanFlag ? $" high > {HiDistance_ft:#,##0}ft" :
        $" high {HiDistance_ft:#,##0}ft";
    }

    private string FromTrend( )
    {
      if ( Trend == "U" ) return " trending up";
      if ( Trend == "D" ) return " trending down";
      if ( Trend == "N" ) return " no trend";
      return "";
    }


  }

  // ******* DECODER CLASS

  internal static class M_RunwayVRDecoder
  {
    /*
    "^(RVRNO |
        R(?P<name>\d\d(RR?|LL?|C)?)/
        (?P<low>(M|P)?(\d\d\d\d|/{4}))
        (V(?P<high>(M|P)?\d\d\d\d))?
        (?P<unit>FT)?[/NDU]*)\s+"
     */

    private static Regex RE_regular = new Regex(@"^(?<chunk>(?<rwy>R\d{2}(R|L|C)?)/(?<mpl>M|P)?(?<lo>\d{4})(V(?<mph>M|P)?(?<hi>\d{4}))?(?<unit>FT)?(?<trend>N|D|U)?)(?<rest>\s{1}.*)", RegexOptions.Compiled);

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
    /// <param name="runwayVRs">The RunwayVR List to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, List<M_RunwayVR> runwayVRs )
    {
      try {
        if ( IsMatch( raw ) ) {
          Match match = RE_regular.Match( raw );
          var rw= new M_RunwayVR();
          rw.Chunks += match.Groups["chunk"].Value;
          rw.RunwayID = match.Groups["rwy"].Value;
          if ( match.Groups["lo"].Success ) {
            rw.LoDistance_ft = UnitC.DistAsFT( int.Parse( match.Groups["lo"].Value ), match.Groups["unit"].Value );
            rw.LoLessThanFlag = match.Groups["mpl"].Success && match.Groups["mpl"].Value == "M";
            rw.LoMoreThanFlag = match.Groups["mpl"].Success && match.Groups["mpl"].Value == "P";
          }

          if ( match.Groups["hi"].Success ) {
            rw.HiDistance_ft = UnitC.DistAsFT( int.Parse( match.Groups["hi"].Value ), match.Groups["unit"].Value );
            rw.HiLessThanFlag = match.Groups["mpl"].Success && match.Groups["mpl"].Value == "M";
            rw.HiMoreThanFlag = match.Groups["mpl"].Success && match.Groups["mpl"].Value == "P";
          }

          if ( match.Groups["trend"].Success ) {
            rw.Trend = match.Groups["trend"].Value;
          }

          rw.Valid = true;
          runwayVRs.Add( rw );
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
