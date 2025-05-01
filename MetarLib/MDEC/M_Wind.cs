using System.Text.RegularExpressions;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Wind Data
  /// </summary>
  public class M_Wind : Chunk
  {
    /// <summary>
    /// Flag if ABV speed was found
    /// </summary>
    public bool SpeedAbove { get; set; } = false;  // above 99KT, 50MPS
    /// <summary>
    /// Windspeed in KT
    /// </summary>
    public float WindSpeed_kt { get; set; } = float.NaN; // when not above
    /// <summary>
    /// Gust Speed in KT
    /// </summary>
    public float WindGustSpeed_kt { get; set; } = float.NaN; // when not above and Gusts
    /// <summary>
    /// VAR tag found
    /// </summary>
    public bool DirectionVariable { get; set; } = false;  // VAR tag
    /// <summary>
    /// Main Wind Direction 
    /// </summary>
    public float WindDirectionMain_deg { get; set; } = float.NaN; // not variable
    /// <summary>
    /// Variable Wind From part
    /// </summary>
    public float WindVariationFrom_deg { get; set; } = float.NaN; // degVdeg chunk
    /// <summary>
    /// Variable Wind To part
    /// </summary>
    public float WindVariationTo_deg { get; set; } = float.NaN; // degVdeg chunk

    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      MakePretty( );

    private string MakePretty( )
    {
      // Direction (from-to|variable)@ speed (gust)
      string ret = "Wind: ";

      if ( WindDirectionMain_deg == 0 && WindSpeed_kt == 0 ) {
        ret += "calm";
      }
      else {
        if ( DirectionVariable ) {
          ret += "variable";
        }
        else {
          ret += $"{WindDirectionMain_deg:000}°";
          if ( !float.IsNaN( WindVariationFrom_deg ) ) {
            ret += $" (var {WindVariationFrom_deg:000}°..{WindVariationTo_deg:000}°)";
          }
        }

        if ( SpeedAbove ) {
          ret += " @ above 99 kt";
        }
        else {
          ret += $" @ {WindSpeed_kt:##0}kt";
          if ( !float.IsNaN( WindGustSpeed_kt ) ) {
            ret += $" (gusts {WindGustSpeed_kt:##0}kt)";
          }
        }
      }

      return ret;
    }

  }


  // ******* DECODER CLASS

  internal static class M_WindDecoder
  {
    /*
     dddss[Gss]KT.. 
    or
     ABVssKT..
    or
     VRBss[Gss]KT.. 

    or the variable part
     dddVddd
     */
    private static Regex RE_regular  = new Regex(@"^(?<chunk>(?<dir>\d{3})(?<speed>\d{2,3})(G(?<gust>\d{2,3}))?(?<unit>KTS?|MPS|KMH))(?<rest>\s{1}.*)", RegexOptions.Compiled);// 23002KT 23002G23MPS
    private static Regex RE_above    = new Regex(@"^(?<chunk>(ABV)(?<speed>\d{2,3})(?<unit>KTS?|MPS|KMH))(?<rest>\s{1}.*)", RegexOptions.Compiled); //  ABV99KT 

    private static Regex RE_varLow   = new Regex(@"^(?<chunk>(VRB)(?<speed>\d{2,3})(G(?<gust>\d{2,3}))?(?<unit>KTS?|MPS|KMH))(?<rest>\s{1}.*)", RegexOptions.Compiled);// VRB02KT VAR02G26 <6KT

    private static Regex RE_variable = new Regex(@"^(?<chunk>(?<from>\d{3})(V)(?<to>\d{3}))(?<rest>\s{1}.*)", RegexOptions.Compiled); // option variable 350V002


    /// <summary>
    /// Decode a part into mData and return the rest
    /// </summary>
    /// <param name="raw">The raw METAR input string</param>
    /// <param name="wind">The Wind record to fill in</param>
    /// <returns>The reminder of  the input string, (raw minus the processed part)</returns>
    public static string Decode( string raw, M_Wind wind )
    {
      try {
        Match match = RE_regular.Match( raw );
        if ( match.Success ) {
          wind.Chunks += match.Groups["chunk"].Value;
          wind.DirectionVariable = false;
          wind.SpeedAbove = false;
          wind.WindDirectionMain_deg = int.Parse( match.Groups["dir"].Value );
          wind.WindSpeed_kt = UnitC.SpeedAsKT( int.Parse( match.Groups["speed"].Value ), match.Groups["unit"].Value );
          if ( match.Groups["gust"].Success )
            wind.WindGustSpeed_kt = UnitC.SpeedAsKT( int.Parse( match.Groups["gust"].Value ), match.Groups["unit"].Value );
          wind.Valid = true;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_above.Match( raw );
        if ( match.Success ) {
          wind.Chunks += match.Groups["chunk"].Value;
          wind.DirectionVariable = false;
          wind.SpeedAbove = true;
          wind.WindDirectionMain_deg = 0; // undet direction 
          wind.WindSpeed_kt = UnitC.SpeedAsKT( int.Parse( match.Groups["speed"].Value ), match.Groups["unit"].Value );
          wind.Valid = true;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_varLow.Match( raw );
        if ( match.Success ) {
          wind.Chunks += match.Groups["chunk"].Value;
          wind.DirectionVariable = true;
          wind.SpeedAbove = false;
          wind.WindSpeed_kt = UnitC.SpeedAsKT( int.Parse( match.Groups["speed"].Value ), match.Groups["unit"].Value );
          if ( match.Groups["gust"].Success )
            wind.WindGustSpeed_kt = UnitC.SpeedAsKT( int.Parse( match.Groups["gust"].Value ), match.Groups["unit"].Value );
          wind.Valid = true;
          return match.Groups["rest"].Value.TrimStart( );
        }

        match = RE_variable.Match( raw );
        // should have a valid Wind Record here..
        if ( wind.Valid && match.Success ) {
          wind.Chunks += " " + match.Groups["chunk"].Value;
          wind.WindVariationFrom_deg = int.Parse( match.Groups["from"].Value );
          wind.WindVariationTo_deg = int.Parse( match.Groups["to"].Value );
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
