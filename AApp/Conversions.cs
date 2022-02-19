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
    private const double c_mPNm = 1852.0;
    private const double c_nmPm = 1.0/c_mPNm;

    private const double c_mPFt = 0.3048;
    private const double c_ftPm = 1.0 / c_mPFt;

    private const double c_degF = 9f/5f;

    /// <summary>
    /// Nautical Miles from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Nautical Miles</returns>
    public static float Nm_From_M( double meter )
    {
      return (float)( meter * c_nmPm );
    }
    /// <summary>
    /// Meters from Nautical Miles
    /// </summary>
    /// <param name="meter">Nautical Miles</param>
    /// <returns>Meter</returns>
    public static float M_From_Nm( double nm )
    {
      return (float)( nm * c_mPNm );
    }

    /// <summary>
    /// Foot from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Foot</returns>
    public static float Ft_From_M( double meter )
    {
      return (float)( meter * c_ftPm );
    }
    /// <summary>
    /// Meters from Foot
    /// </summary>
    /// <param name="ft">Foot</param>
    /// <returns><Meter/returns>
    public static float M_From_Ft( double ft )
    {
      return (float)( ft * c_mPFt );
    }

    /// <summary>
    /// Returns Meter/Sec from Foot/Min
    /// </summary>
    /// <param name="fpm">A foot/minute value</param>
    /// <returns>The meter/second value</returns>
    public static float Mps_From_Ftpm( float fpm )
    {
      return (float)( fpm * c_mPFt / 60.0 );
    }

    /// <summary>
    /// Returns kts from m/sec
    /// </summary>
    /// <param name="mps">Meter per sec value</param>
    /// <returns>Converted Knots value</returns>
    public static float Kt_From_Mps( float mps )
    {
      return (float)( mps * 3600f * c_nmPm );
    }

    /// <summary>
    /// Returns DegF from DegC ((DEG°C * 9/5) + 32 = 32°F)
    /// </summary>
    /// <param name="degC">Temp deg C</param>
    /// <returns>Temp in deg F</returns>
    public static float DegF_From_DegC( double degC )
    {
      return (float)( ( degC * c_degF ) + 32.0f );
    }

    /// <summary>
    /// Derives Apt from an ATC Translatable Apt string
    /// </summary>
    /// <param name="atcApt">The ATC Apt string</param>
    /// <returns>The ICAO Apt part</returns>
    public static string AptFromATCApt( string atcApt )
    {
      if ( string.IsNullOrEmpty( atcApt ) ) return AirportMgr.AirportNA_Icao; // seen a null here...

      // arrives as TT:AIRPORTLR.ICAO.name
      string[] e = atcApt.Split ( new char[]{'.'});
      if ( e.Length > 1 )
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
      return (float)( Math.Round( number / quant ) * quant );
    }



  }
}
