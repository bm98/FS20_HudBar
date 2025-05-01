using System;

namespace bm98_VProfile
{
  /// <summary>
  /// Does all kind of conversions from Input to Display items
  /// Methods should be static
  /// </summary>
  internal static class Conversions
  {
    /**
 * Constrain degrees to range -180..+180 (for longitude); e.g. -181 => 179, 181 => -179.
 *
 * @private
 * @param {number} degrees
 * @returns degrees within range -180..+180.
 */
    public static double Wrap180( double degrees )
    {
      if ( -180 <= degrees && degrees <= 180 ) return degrees; // avoid rounding due to arithmetic ops if within range

      // longitude wrapping requires a sawtooth wave function; a general sawtooth wave is
      //     f(x) = (2ax/p - p/2) % p - a
      // where a = amplitude, p = period, % = modulo; however, JavaScript '%' is a remainder operator (same in C#)
      // not a modulo operator - for modulo, replace 'x%n' with '((x%n)+n)%n'
      double x = degrees, a = 180, p = 360;
      return ( ( ( 2 * a * x / p - p / 2 ) % p ) + p ) % p - a;
    }

    /**
 * Constrain degrees to range 0..360 (for bearings); e.g. -1 => 359, 361 => 1.
 *
 * @private
 * @param {number} degrees
 * @returns degrees within range 0..360.
 */
    public static double Wrap360( double degrees )
    {
      if ( 0 <= degrees && degrees < 360 ) return degrees; // avoid rounding due to arithmetic ops if within range

      // bearing wrapping requires a sawtooth wave function with a vertical offset equal to the
      // amplitude and a corresponding phase shift; this changes the general sawtooth wave function from
      //     f(x) = (2ax/p - p/2) % p - a
      // to
      //     f(x) = (2ax/p) % p
      // where a = amplitude, p = period, % = modulo; however, JavaScript '%' is a remainder operator (same in C#)
      // not a modulo operator - for modulo, replace 'x%n' with '((x%n)+n)%n'
      double x = degrees, a = 180, p = 360;
      return ( ( ( 2 * a * x / p ) % p ) + p ) % p;
    }

    /// <summary>
    /// Restricts to 360 deg and returns 360° for 0.0°
    /// </summary>
    /// <param name="deg">Degree Input</param>
    /// <returns>Degree wraped to >0..360</returns>
    public static float NavDeg( float deg )
    {
      var ret = Wrap360(deg);
      return (float)( ( ret == 0 ) ? 360.0 : ret );
    }

    /// <summary>
    /// Restricts to 360 deg and returns 360° for 0°
    /// </summary>
    /// <param name="deg">Degree Input</param>
    /// <returns>Degree wraped to >0..360 as INTEGER</returns>
    public static int NavDegInt( float deg )
    {
      var ret = (int)Wrap360(deg);
      return ( ret == 0 ) ? 360 : ret;
    }

    /// <summary>
    /// Returns a rounded float to 20 steps
    /// </summary>
    /// <param name="fIn">A float input</param>
    /// <returns>Quantized integer</returns>
    public static int Quant20(float fIn )
    {
      if ( float.IsNaN( fIn ) ) return 0;
      return (int)Math.Round( fIn / 20) * 20;
    }

  }
}
