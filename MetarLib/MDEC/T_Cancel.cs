using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{

  // ******* DECODER CLASS

  internal static class T_CancelDecoder
  {
    /*
     CNL CHUNKS
    
     */
    private static Regex RE_regular  = new Regex(@"^(?<cancel>CNL)(?<rest>\s{1}.*)", RegexOptions.Compiled);

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
    /// <param name="mData">The MData record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, MTData mData )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          mData.CancelFlag = ( match.Groups["rest"].Value == "CNL" );
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
