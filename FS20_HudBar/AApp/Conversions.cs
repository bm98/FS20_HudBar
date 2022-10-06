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
    private const double c_nmPm = 1.0 / c_mPNm;

    private const double c_kmhPkt = 1.852;
    private const double c_ktPkmh = 1.0 / c_kmhPkt;

    private const double c_mPFt = 0.3048;
    private const double c_ftPm = 1.0 / c_mPFt;

    private const double c_lbsPkg = 2.204622621848776;
    private const double c_kgPlbs = 1.0 / c_lbsPkg;

    private const double c_degF = 9f / 5f;

    /// <summary>
    /// Kilograms from Pounds
    /// </summary>
    /// <param name="kg">Kilograms</param>
    /// <returns>Pound</returns>
    public static float Lbs_From_Kg( double kg )
    {
      return (float)(kg * c_lbsPkg);
    }

    /// <summary>
    /// Kilograms from Pounds
    /// </summary>
    /// <param name="lbs">Pound</param>
    /// <returns>Kilograms</returns>
    public static float Kg_From_Lbs( double lbs )
    {
      return (float)(lbs * c_kgPlbs);
    }

    /// <summary>
    /// Nautical Miles from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Nautical Miles</returns>
    public static float Nm_From_M( double meter )
    {
      return (float)(meter * c_nmPm);
    }
    /// <summary>
    /// Meters from Nautical Miles
    /// </summary>
    /// <param name="nm">Nautical Miles</param>
    /// <returns>Meter</returns>
    public static float M_From_Nm( double nm )
    {
      return (float)(nm * c_mPNm);
    }

    /// <summary>
    /// Kilometers from Nautical Miles
    /// </summary>
    /// <param name="nm">Nautical Miles</param>
    /// <returns>Kilometer</returns>
    public static float Km_From_Nm( double nm )
    {
      return (float)(nm * c_mPNm / 1000.0);
    }

    /// <summary>
    /// Foot from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Foot</returns>
    public static float Ft_From_M( double meter )
    {
      return (float)(meter * c_ftPm);
    }
    /// <summary>
    /// Meters from Foot
    /// </summary>
    /// <param name="ft">Foot</param>
    /// <returns>Meter</returns>
    public static float M_From_Ft( double ft )
    {
      return (float)(ft * c_mPFt);
    }

    /// <summary>
    /// Returns Meter/Sec from Foot/Min
    /// </summary>
    /// <param name="fpm">A foot/minute value</param>
    /// <returns>The meter/second value</returns>
    public static float Mps_From_Ftpm( float fpm )
    {
      return (float)(fpm * c_mPFt / 60.0);
    }

    /// <summary>
    /// Returns m/s from kt
    /// </summary>
    /// <param name="kt">Knots value</param>
    /// <returns>Meter / second</returns>
    public static float Mps_From_Kt( float kt )
    {
      return (float)(kt * c_mPNm / 3600f);
    }

    /// <summary>
    /// Returns kts from m/sec
    /// </summary>
    /// <param name="mps">Meter per sec value</param>
    /// <returns>Converted Knots value</returns>
    public static float Kt_From_Mps( float mps )
    {
      return (float)(mps * 3600f * c_nmPm);
    }

    /// <summary>
    /// Returns kmh from kt
    /// </summary>
    /// <param name="kt">Knots value</param>
    /// <returns>Kilometer per hour</returns>
    public static float Kmh_From_Kt( float kt )
    {
      return (float)(kt * c_kmhPkt);
    }

    /// <summary>
    /// Returns kt from kmh
    /// </summary>
    /// <param name="kmh">Knots value</param>
    /// <returns>Kilometer per hour</returns>
    public static float Kt_From_Kmh( float kmh )
    {
      return (float)(kmh * c_ktPkmh);
    }

    /// <summary>
    /// Returns feet per Minute from Feet per Second
    /// </summary>
    /// <param name="fps">Feet per Second</param>
    /// <returns>Feet per Minute</returns>
    public static float Fpm_From_Fps( float fps )
    {
      return fps * 60f;
    }

    /// <summary>
    /// Returns DegF from DegC ((DEG°C * 9/5) + 32 = 32°F)
    /// </summary>
    /// <param name="degC">Temp deg C</param>
    /// <returns>Temp in deg F</returns>
    public static float DegF_From_DegC( double degC )
    {
      return (float)((degC * c_degF) + 32.0f);
    }

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
