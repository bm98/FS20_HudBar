using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{

  // ******* DECODER CLASS

  internal static class M_MsgTypeDecoder
  {
    /*
     METAR CHUNKS

     TAF CHUNKS
     TAF AMD CHUNKS
     TAF COR CHUNKS
     
     */
    private static Regex RE_metar  = new Regex(@"^(?<mtype>METAR)(\s(?<opt>COR))?(?<rest>\s{1}.*)", RegexOptions.Compiled);
    private static Regex RE_speci  = new Regex(@"^(?<mtype>SPECI)(\s(?<opt>COR))?(?<rest>\s{1}.*)", RegexOptions.Compiled);
    private static Regex RE_taf  = new Regex(@"^(?<mtype>TAF)(\s(?<opt>AMD|COR))?(?<rest>\s{1}.*)", RegexOptions.Compiled);


    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="mData">The MData record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, MTData mData )
    {
      try {
        Match match = RE_metar.Match( raw );
        if ( match.Success ) {
          mData.MsgType = MsgType.METAR;
          if ( match.Groups["opt"].Success && match.Groups["opt"].Value == "COR" )
            mData.MsgModifier = MsgModifier.COR;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_speci.Match( raw );
        if ( match.Success ) {
          mData.MsgType = MsgType.SPECI;
          if ( match.Groups["opt"].Success && match.Groups["opt"].Value == "COR" )
            mData.MsgModifier = MsgModifier.COR;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_taf.Match( raw );
        if ( match.Success ) {
          mData.MsgType = MsgType.TAF;
          if ( match.Groups["opt"].Success && match.Groups["opt"].Value == "AMD" )
            mData.MsgModifier = MsgModifier.AMD;
          else if ( match.Groups["opt"].Success && match.Groups["opt"].Value == "COR" )
            mData.MsgModifier = MsgModifier.COR;
          else
            mData.MsgModifier = MsgModifier.NONE;
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
