using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Some general tools are located here..
  /// </summary>
  class Tooling
  {
    private const float c_nmPerM = 5.399568e-4f;

    /// <summary>
    /// Nautical Miles from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Nautical Miles</returns>
    public static float NmFromM(float meter )
    {
      return meter * c_nmPerM;
    }

    /// <summary>
    /// Derives Apt from an ATC Translatable Apt string
    /// </summary>
    /// <param name="atcApt">The ATC Apt string</param>
    /// <returns>The ICAO Apt part</returns>
    public static string AptFromATCApt(string atcApt )
    {
      if ( string.IsNullOrEmpty(atcApt)) return "n.a."; // seen a null here...

      // arrives as TT:AIRPORTLR.ICAO.name
      string[] e = atcApt.Split ( new char[]{'.'});
      if ( e.Length > 1 ) 
        return e[1];

      return "n.a.";    
    }
  }
}
