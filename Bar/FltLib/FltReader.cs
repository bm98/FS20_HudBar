using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FS20_HudBar.Bar.FltLib
{
  internal static class FltReader
  {

    /// <summary>
    /// Skips the reader until the partName is found as [part]
    /// </summary>
    /// <param name="partName">The part name</param>
    /// <param name="reader">A text reader</param>
    /// <returns>The reader placed AFTER the part header</returns>
    private static bool SkipToPart( string partName, TextReader reader )
    {
      string line = reader.ReadLine();
      while ( line != null ) {
        if ( line.StartsWith( $"[{partName}]" ) ) {
          return true;
        }
        line = reader.ReadLine( );
      }
      return false;
    }

    /// <summary>
    /// Collects all entries of a single part [part] section
    /// </summary>
    /// <param name="partName">The part name</param>
    /// <param name="fileContent">The file content as string</param>
    /// <returns>The list of lines belonging to the part (can be an empty list)</returns>
    public static List<string> GetPart( string partName, string fileContent )
    {
      var ret = new List<string>();

      using ( var sr = new StringReader( fileContent ) ) {
        if ( SkipToPart( partName, sr ) ) {
          string line = sr.ReadLine();
          while ( line != null ) {
            if ( line.StartsWith( "[" ) ) {
              return ret;
            }
            ret.Add( line );
            line = sr.ReadLine( );
          }
        }
      }
      return ret;
    }

    // item  = content
    private readonly static Regex c_Item = new Regex(@"^(?<item>[\w\.]*)(\s*=)(?<cont>.*)?", RegexOptions.Compiled);

    /// <summary>
    /// Returns the content of an item (item=content) from a list of part lines
    /// </summary>
    /// <param name="part">A list of item lines</param>
    /// <param name="item">The item sought</param>
    /// <returns>The item content (can be empty string)</returns>
    public static string ItemContent( List<string> part, string item )
    {
      item = item.ToUpperInvariant( );
      foreach ( var line in part ) {
        Match match = c_Item.Match( line );
        if ( match.Success && ( match.Groups["item"].Value.ToUpperInvariant( ) == item ) ) {
          return match.Groups["cont"].Value.Trim( );
        }
      }
      return "";
    }

  }
}
