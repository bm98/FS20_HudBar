using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FS20_HudBar.Bar.FltLib
{
  /// <summary>
  /// Static general Methods
  /// </summary>
  internal class Tooling
  {

    private static readonly Regex c_latRE = new Regex(@"^(?<ns>N|S)(?<deg>\d{1,2}).\s(?<part>\d{1,2}\.\d{2})'");
    private static readonly Regex c_lonRE = new Regex(@"^(?<ns>E|W)(?<deg>\d{1,3}).\s(?<part>\d{1,2}\.\d{2})'");
    private static readonly Regex c_altRE = new Regex(@"^(?<alt>(\+|-)\d{6}\.\d{2})");

    /// <summary>
    /// Convert from Lat string to double 
    /// N23° 23.23' or S33.12
    /// </summary>
    /// <param name="lat">Lat String</param>
    /// <returns></returns>
    public static double ToLat( string lat )
    {
      Match match = c_latRE.Match(lat);
      if ( match.Success ) {
        double l = double.Parse(match.Groups["deg"].Value);
        l += double.Parse( match.Groups["part"].Value ) / 60.0;
        l = ( match.Groups["ns"].Value == "N" ) ? l : -l; // North South Deg
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Convert from Lon string to double 
    /// E23° 23.23' or W133.12
    /// </summary>
    /// <param name="lon">Lon String</param>
    /// <returns></returns>
    public static double ToLon( string lon )
    {
      Match match = c_lonRE.Match(lon);
      if ( match.Success ) {
        double l = double.Parse(match.Groups["deg"].Value);
        l += double.Parse( match.Groups["part"].Value ) / 60.0;
        l = ( match.Groups["ns"].Value == "E" ) ? l : -l; // North South Deg
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Convert from Alt string to double 
    /// +123456.00 or -000006.00
    /// </summary>
    /// <param name="alt">Lon String</param>
    /// <returns></returns>
    public static double ToAlt( string alt )
    {
      Match match = c_altRE.Match(alt);
      if ( match.Success ) {
        double l = double.Parse(match.Groups["alt"].Value);
        return l;
      }
      return double.NaN;
    }

    /// <summary>
    /// Returns the Bool of the string (checks for True only, else it is False)
    /// </summary>
    /// <param name="bol">A Boolean string</param>
    /// <returns>The bool type</returns>
    public static bool ToBool(string bol )
    {
      return ( bol.ToLowerInvariant( ) == "true" );
    }

  }
}
