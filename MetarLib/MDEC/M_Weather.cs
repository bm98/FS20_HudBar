using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Weather Part
  /// </summary>
  public class M_Weather : Chunk
  {
    /// <summary>
    /// Intensity Sign
    /// </summary>
    public string Intensity { get; set; } = "";
    /// <summary>
    /// Descriptor tag
    /// </summary>
    public string Descriptor { get; set; } = "";
    /// <summary>
    /// Precipitation tags
    /// </summary>
    public string Precipitation { get; set; } = "";
    /// <summary>
    /// Obscuration tags
    /// </summary>
    public string Obscuration { get; set; } = "";
    /// <summary>
    /// Other tags
    /// </summary>
    public string Other { get; set; } = "";

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Weather:{FromIntensity( )}{FromDescriptor( )}{FromPrecipitation( )}{FromObscuration( )}{FromOther( )}";

    // Content decoders

    private Dictionary<string, string> DIntensity = new Dictionary<string, string>( ){
      {"-", "Light" },
      {"+", "Heavy" },
      {"=", "Moderate" }, // internal tag for METAR NO TAG = Moderate
      {"?", "Chance of" }, // added since observed (20231105)
      {"VC", "Nearby" },
      {"-VC", "Nearby light" },
      {"+VC", "Nearby heavy" },
    };
    private string FromIntensity( )
    {
      string ret = "";
      string tag = Intensity;
      try {
        ret += $" {DIntensity[tag]}";
      }
      catch { }
      return ret;
    }


    private Dictionary<string, string> DDescriptor = new Dictionary<string, string>( ){
      {"MI", "shallow"},
      {"PR", "partial"},
      {"BC", "patches of"},
      {"DR", "low drifting"},
      {"BL", "blowing"},
      {"SH", "showers"},
      {"TS", "thunderstorm"},
      {"FZ", "freezing" },
    };
    private string FromDescriptor( )
    {
      string ret = "";
      string desc = Descriptor;
      while (desc.Length > 1) {
        string tag = desc.Substring( 0, 2 );
        try {
          ret += $" {DDescriptor[tag]}";
        }
        catch { }
        if (desc.Length > 2)
          desc = desc.Substring( 2 );
        else
          desc = "";
      }
      return ret;
    }

    private Dictionary<string, string> DPrecipitation = new Dictionary<string, string>( ){
      {"DZ", "drizzle"},
      {"RA", "rain"},
      {"SN", "snow"},
      {"SG", "snow grains"},
      {"IC", "ice crystals"},
      {"PL", "ice pellets"},
      {"GR", "hail"},
      {"GS", "snow pellets"},
      {"UP", "unknown precipitation" },
      { "//", "" },
    };
    private string FromPrecipitation( )
    {
      string ret = "";
      string desc = Precipitation;
      while (desc.Length > 1) {
        string tag = desc.Substring( 0, 2 );
        try {
          ret += $" {DPrecipitation[tag]}";
        }
        catch { }
        if (desc.Length > 2)
          desc = desc.Substring( 2 );
        else
          desc = "";
      }
      return ret;
    }

    private Dictionary<string, string> DObscuration = new Dictionary<string, string>( ){
      {"BR", "mist"},
      {"FG", "fog"},
      {"FU", "smoke"},
      {"VA", "volcanic ash"},
      {"DU", "dust"},
      {"SA", "sand"},
      {"HZ", "haze"},
      {"PY", "spray" },
    };
    private string FromObscuration( )
    {
      string ret = "";
      string desc = Obscuration;
      while (desc.Length > 1) {
        string tag = desc.Substring( 0, 2 );
        try {
          ret += $" {DObscuration[tag]}";
        }
        catch { }
        if (desc.Length > 2)
          desc = desc.Substring( 2 );
        else
          desc = "";
      }
      return ret;
    }

    private Dictionary<string, string> DOther = new Dictionary<string, string>( ){
      {"PO", "sand whirls"},
      {"SQ", "squalls"},
      {"FC", "funnel cloud"},
      {"SS", "sandstorm"},
      {"DS", "dust storm" },
    };
    private string FromOther( )
    {
      string ret = "";
      string desc = Other;
      while (desc.Length > 1) {
        string tag = desc.Substring( 0, 2 );
        try {
          ret += $" {DOther[tag]}";
        }
        catch { }
        if (desc.Length > 2)
          desc = desc.Substring( 2 );
        else
          desc = "";
      }
      return ret;
    }

  }



  // ******* DECODER CLASS

  internal static class M_WeatherDecoder
  {
    /*
      "^(?P<int>(-|\+|VC\?)*)
        (?P<desc>(MI|PR|BC|DR|BL|SH|TS|FZ)+)?
        (?P<prec>(DZ|RA|SN|SG|IC|PL|GR|GS|UP|/)*)
        (?P<obsc>BR|FG|FU|VA|DU|SA|HZ|PY)?
        (?P<other>PO|SQ|FC|SS|DS|NSW|/+)?
        (?P<int2>[-+])?\s+"
     */

    private static Regex RE_regular = new Regex(
       @"^(?<chunk>(?<int>(-|\+|VC|\?)*)"           // optional (if not it's medium) (succeeds if 0 captured !!) can be ? (added 20231105)
      + @"(?<desc>(MI|PR|BC|DR|BL|SH|TS|FZ)+)?"     // optional (BR) 
      + @"(?<prec>(DZ|RA|SN|SG|IC|PL|GR|GS|UP|/)*)"  // up to 3 per manual (succeeds if 0 captured !!)
      + @"(?<obsc>BR|FG|FU|VA|DU|SA|HZ|PY)?"         // optional
      + @"(?<other>PO|SQ|FC|SS|DS|NSW|/+)?"
      + @"(?<int2>[-+])?)"
      + @"(?<rest>\s{1}.*)",
       RegexOptions.Compiled );

    /// <summary>
    /// Test if the content matches
    /// </summary>
    /// <param name="raw">The raw message input string</param>
    /// <returns>True if it matches</returns>
    public static bool IsMatch( string raw )
    {
      // darn METAR protocol allows any strange combinations...
      if (RE_regular.Match( raw ).Groups["desc"].Success) return true;
      if (RE_regular.Match( raw ).Groups["prec"].Success && !string.IsNullOrEmpty( RE_regular.Match( raw ).Groups["prec"].Value )) return true;
      if (RE_regular.Match( raw ).Groups["obsc"].Success) return true;
      if (RE_regular.Match( raw ).Groups["other"].Success) return true;

      return false;
    }


    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="weather">The Weather List</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, List<M_Weather> weather )
    {
      try {
        Match match = RE_regular.Match( raw );
        if (IsMatch( raw )) {
          var w = new M_Weather( );
          w.Chunks += match.Groups["chunk"].Value;
          if (!string.IsNullOrEmpty( match.Groups["int"].Value )) {// succeeds everytime (0 captures wins)
            w.Intensity = match.Groups["int"].Value;
          }
          else {
            w.Intensity = "="; // our moderate tag
          }
          if (match.Groups["desc"].Success) {
            w.Descriptor = match.Groups["desc"].Value;
          }
          if (match.Groups["prec"].Success) { // succeeds everytime (0 captures wins)
            w.Precipitation = match.Groups["prec"].Value; //value is empty - OK to capture
          }
          if (match.Groups["obsc"].Success) {
            w.Obscuration = match.Groups["obsc"].Value;
          }
          if (match.Groups["other"].Success) {
            w.Other = match.Groups["other"].Value;
          }

          w.Valid = true;
          weather.Add( w );
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
