using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Sky Condition
  /// </summary>
  public class M_SkyCondition : Chunk
  {
    /// <summary>
    /// Cover type
    /// </summary>
    public string Cover { get; set; } = "";
    /// <summary>
    /// Base height ft
    /// </summary>
    public int Height_ft { get; set; } = int.MaxValue;
    /// <summary>
    /// Cloud type
    /// </summary>
    public string Cloud { get; set; } = "";

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Clouds:{FromCover( )}{FromHeight( )}{FromCloud( )}";

    // Content decoders

    private Dictionary<string,string> DCover = new Dictionary<string, string>(){
      {"SKC", "clear"},
      {"CLR", "clear"},
      {"NSC", "clear"},
      {"NCD", "clear"},
      {"FEW", "few"},
      {"SCT", "scattered"},
      {"BKN", "broken"},
      {"OVC", "overcast"},
      {"///", ""},
      {"VV", "sky obscured" },
    };
    private string FromCover( )
    {
      string ret = "";
      string tag = Cover;
      try {
        ret += $" {DCover[tag]}";
      }
      catch { }
      return ret;
    }

    private Dictionary<string,string> DCloud = new Dictionary<string, string>(){
      {"AC", "altocumulus"},
      {"ACC", "altocumulus castellanus"},
      {"ACSL", "standing lenticular altocumulus"},
      {"AS", "altostratus"},
      {"CB", "cumulonimbus"},
      {"CBMAM", "cumulonimbus mammatus"},
      {"CCSL", "standing lenticular cirrocumulus"},
      {"CC", "cirrocumulus"},
      {"CI", "cirrus"},
      {"CS", "cirrostratus"},
      {"CU", "cumulus"},
      {"NS", "nimbostratus"},
      {"SC", "stratocumulus"},
      {"ST", "stratus"},
      {"SCSL", "standing lenticular stratocumulus"},
      {"TCU", "towering cumulus" },
    };
    private string FromCloud( )
    {
      string ret = "";
      string tag = Cloud;
      try {
        ret += $" {DCloud[tag]}";
      }
      catch { }
      return ret;
    }

    private string FromHeight( )
    {
      string ret = "";
      if ( Height_ft < int.MaxValue ) {
        if ( Cover != "VV" )
          ret += $" at {Height_ft:##,##0} ft (AGL)";
        else
          ret += $" vert. visibility {Height_ft:##,##0} ft";
      }
      return ret;
    }



  }

  // ******* DECODER CLASS

  internal static class M_SkyConditionDecoder
  {
    /*
"^(?P<cover>VV|CLR|SKC|SCK|NSC|NCD|BKN|SCT|FEW|[O0]VC|///)
        (?P<height>[\dO]{2,4}|///)?
        (?P<cloud>([A-Z][A-Z]+|///))?\s+"
     */
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<cover>VV|CLR|SKC|SCK|NSC|NCD|BKN|SCT|FEW|[O0]VC|///)(?<height>\d{3}|///)?(?<cloud>([A-Z][A-Z]+|///))?)(?<rest>\s{1}.*)", RegexOptions.Compiled);

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
    /// <param name="skyConditions">The SkyConditions List to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, List<M_SkyCondition> skyConditions )
    {
      try {
        if ( IsMatch( raw ) ) {
          Match match = RE_regular.Match( raw );
          var sc = new M_SkyCondition();
          sc.Chunks += match.Groups["chunk"].Value;
          sc.Cover = match.Groups["cover"].Value;
          if ( match.Groups["height"].Success ) {
            if ( match.Groups["height"].Value != "///" )
              sc.Height_ft = int.Parse( match.Groups["height"].Value ) * 100;
          }
          if ( match.Groups["cloud"].Success )
            sc.Cloud = match.Groups["cloud"].Value;

          sc.Valid = true;
          skyConditions.Add( sc );
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
