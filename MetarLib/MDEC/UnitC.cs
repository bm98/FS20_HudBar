using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib.MDEC
{
  internal class UnitC
  {

    /// <summary>
    /// Converts from value unit to Knots
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="unit">The unit to convert from</param>
    /// <returns>The converted value in kt</returns>
    public static float SpeedAsKT( float value, string unit )
    {
      unit = unit.ToUpperInvariant( );

      if ( unit == "KT" )
        return (float)value;
      else if ( unit == "KTS" )
        return (float)value;
      else if ( unit == "MPS" )
        return (float)( value * 1.943844f ); // mps to kt
      else if ( unit == "KMH" )
        return (float)( value * 0.539957f ); // kmh to kt
      else
        return float.NaN; // unknown unit
    }

    /// <summary>
    /// Converts from value unit to Statute Miles
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="unit">The unit to convert from</param>
    /// <returns>The converted value in SM</returns>
    public static float DistAsSM( float value, string unit )
    {
      unit = unit.ToUpperInvariant( );

      if ( unit == "SM" )
        return (float)value;
      else if ( unit == "KM" )
        return (float)( value * 0.621371f ); // km to SM
      else if ( unit == "M" )
        return (float)( value * 0.000621371f ); // m to SM
      else if ( unit == "" ) // Meter
        return (float)(value * 0.000621371f); // m to SM
      else
        return float.NaN; // unknown unit
    }

    /// <summary>
    /// Convert SM to KM
    /// </summary>
    /// <param name="sm">SM value</param>
    /// <returns>Km result</returns>
    public static float SMtoKM(float sm)
    {
      return sm / 0.621371f;
    }

    /// <summary>
    /// Converts from value unit to Feet
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="unit">The unit to convert from</param>
    /// <returns>The converted value in ft</returns>
    public static float DistAsFT( float value, string unit )
    {
      unit = unit.ToUpperInvariant( );

      if ( unit == "FT" )
        return (float)value;
      else if ( unit == "" ) // no unit seems M
        return (float)Math.Round(value * 3.28084 / 100d) * 100f;
      else
        return float.NaN; // unknown unit
    }

    public static float PressureAsInHg( float value, string unit, string unit2 = "" )
    {
      unit = unit.ToUpperInvariant( );

      if ( unit == "A" )
        return (float)value / 100f;
      else if ( unit == "Q" )
        return (float)( value / 33.863886667f ); // hPa to inHg
      else if ( unit == "QNH" && unit2 == "INS" )
        return (float)value / 100f;
      else
        return float.NaN; // unknown unit
    }

    public static float PressureAsHPa( float value, string unit, string unit2 = "" )
    {
      unit = unit.ToUpperInvariant( );

      if ( unit == "A" )
        return (float)value * 33.863886667f / 100f;
      else if ( unit == "Q" )
        return (float)value; 
      else if ( unit == "QNH" && unit2 == "INS" )
        return (float)value * 33.863886667f / 100f;
      else
        return float.NaN; // unknown unit
    }

    /// <summary>
    /// Find the float from a part string
    ///  12/9 or 1 4/8 or the like..
    /// </summary>
    /// <param name="valS"></param>
    /// <returns></returns>
    public static float FromDistU( string valS )
    {
      if ( string.IsNullOrWhiteSpace( valS ) ) return 0f;

      valS = valS.Trim( );
      float ret = 0;

      if ( valS.Contains( " " ) ) {
        //  N M/K
        string[] e = valS.Split(new char[]{ ' ' } );
        if ( int.TryParse( e[0], out int val ) ) {
          ret = (float)val;
          ret += FromDistU( e[1] );
        }
      }

      else if ( valS.Contains( "/" ) ) {
        //  M/K
        string[] e = valS.Split(new char[]{ '/' } );
        if ( int.TryParse( e[0], out int val1 ) && int.TryParse( e[1], out int val2 ) ) {
          ret = (float)val1 / (float)val2;
        }
      }

      else {
        // N
        if ( int.TryParse( valS, out int val3 ) ) {
          ret = (float)val3;
        }
      }
      return ret;
    }

    /// <summary>
    /// Return an int temp from the string
    /// can be -02 or M02 or 44
    /// </summary>
    /// <param name="tempS"></param>
    /// <returns>A temp value (failure returns 0..)</returns>
    public static int FromTemp(string tempS )
    {
      if ( string.IsNullOrWhiteSpace( tempS ) ) return 0;

      tempS = tempS.Trim( );
      int ret = 0;

      if ( tempS.StartsWith( "M" ) )
        tempS = tempS.Replace( "M", "-" );

      if ( int.TryParse(tempS, out int val ) ) {
        ret = val;
      }

      return ret;
    }

    /// <summary>
    /// Rel Humidity
    /// </summary>
    /// <param name="temp">Temperature </param>
    /// <param name="dewp">Dewpoint</param>
    /// <returns>Rel. Humidity</returns>
    public static float RHfromTandD(float temp, float dewp )
    {
      return (float)(100.0 * ( Math.Exp( ( 17.625 * dewp ) / ( 243.04 + dewp ) ) / Math.Exp( ( 17.625 * temp ) / ( 243.04 + temp ) ) ));
    }

    /// <summary>
    /// Convert from Celsius to Farenheit
    /// </summary>
    /// <param name="c"></param>
    /// <returns>Farenheit</returns>
    public static float FfromC(float c )
    {
      return  c * 1.8f + 32f;
    }
  }
}
