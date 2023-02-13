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
  class Conversions
  {
    /// <summary>
    /// Derives Apt from an ATC Translatable Apt string
    /// </summary>
    /// <param name="atcApt">The ATC Apt string</param>
    /// <returns>The ICAO Apt part</returns>
    public static string AptFromATCApt( string atcApt )
    {
      if (string.IsNullOrEmpty( atcApt )) return AirportMgr.AirportNA_Icao; // seen a null here...

      // arrives as TT:AIRPORTLR.ICAO.name
      string[] e = atcApt.Split( new char[] { '.' } );
      if (e.Length > 1)
        return e[1];

      return AirportMgr.AirportNA_Icao;
    }

    /// <summary>
    /// Round the number in Quants given
    /// </summary>
    /// <param name="number">The number</param>
    /// <param name="quant">Quantities to round to</param>
    /// <returns>The rounded number</returns>
    public static float Round( float number, int quant )
    {
      return (float)(Math.Round( number / quant ) * quant);
    }

    #region STATIC DME Dist Sign

    /// <summary>
    /// Returns a signed distance for the DME readout Control V_DistDme 
    /// flag==1 => To   + signed
    /// flag==2 => From - signed
    /// flag==0 => Off  NaN
    /// 
    /// </summary>
    /// <param name="absValue">DME Input from Sim</param>
    /// <param name="fromToFlag">FromTo Flag from Sim</param>
    /// <returns></returns>
    public static float DmeDistance( float absValue, int fromToFlag )
    {
      return (fromToFlag == 0) ? float.NaN : ((fromToFlag == 1) ? absValue : -absValue);
    }
    #endregion



  }
}
