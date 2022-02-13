using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Report modifiers
  /// </summary>
  public class M_Modifier : Chunk
  {
    /// <summary>
    /// The Modifier String
    /// </summary>
    public string Modifier { get; set; } = "";
    /// <summary>
    /// True if NIL 
    /// </summary>
    public bool IsNil => Modifier == "NIL";
    /// <summary>
    /// True if AUTO
    /// </summary>
    public bool IsAuto => Modifier.Contains( "AUTO" );
    /// <summary>
    /// True if COR
    /// </summary>
    public bool IsCor => Modifier.Contains( "COR" );

  }

  // ******* DECODER CLASS

  internal static class M_ModifierDecoder
  {
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<mod>AUTO|COR AUTO|COR|FINO|NIL|TEST|CORR?|RTD|CC[A-G]))(?<rest>\s{1}.*)", RegexOptions.Compiled); // AUTO

    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="modifier">The Modifier record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Modifier modifier )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          modifier.Chunks += match.Groups["chunk"].Value;
          modifier.Modifier = match.Groups["mod"].Value;
          modifier.Valid = true;
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

