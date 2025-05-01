using System.Drawing;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Convert from HSL to RGB colour codes in C#
  /// A function to create a new colour in C# from HSL values:
  /// 
  /// http://james-ramsden.com/convert-from-hsl-to-rgb-colour-codes-in-c/
  /// Thank you...
  /// </summary>
  internal static class ColorScale
  {

    /// <summary>
    /// Converts from HSL to a .Net Colort
    /// </summary>
    /// <param name="h">Hue 0..1</param>
    /// <param name="s">Saturation 0..1</param>
    /// <param name="l">Lightness 0..1</param>
    /// <returns></returns>
    public static Color ColorFromHSL( double h, double s, double l )
    {
      double r = 0, g = 0, b = 0;
      if (l != 0) {
        if (s == 0)
          r = g = b = l;
        else {
          double temp2;
          if (l < 0.5)
            temp2 = l * (1.0 + s);
          else
            temp2 = l + s - (l * s);

          double temp1 = 2.0 * l - temp2;

          r = GetColorComponent( temp1, temp2, h + 1.0 / 3.0 );
          g = GetColorComponent( temp1, temp2, h );
          b = GetColorComponent( temp1, temp2, h - 1.0 / 3.0 );
        }
      }
      return Color.FromArgb( (int)(255 * r), (int)(255 * g), (int)(255 * b) );

    }

    private static double GetColorComponent( double temp1, double temp2, double temp3 )
    {
      if (temp3 < 0.0)
        temp3 += 1.0;
      else if (temp3 > 1.0)
        temp3 -= 1.0;

      if (temp3 < 1.0 / 6.0)
        return temp1 + (temp2 - temp1) * 6.0 * temp3;
      else if (temp3 < 0.5)
        return temp2;
      else if (temp3 < 2.0 / 3.0)
        return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
      else
        return temp1;
    }

    // Plug from ADS-B visualization web site
    // for Alt based coloring of Airplanes

    /// <summary>
    /// Color type based on Acft status
    /// </summary>
    public enum ColorType
    {
      Unknown = 0,
      OnGround = 1,
      InAir = 2,
    }

    // These define altitude-to-hue mappings
    // at particular altitudes; the hue
    // for intermediate altitudes that lie
    // between the provided altitudes is linearly
    // interpolated.
    // assuming the W3 360,100,100 HSL spec as Input values
    private static readonly double[] _unk = new double[3] { 0, 0, 40 };
    private static readonly double[] _gnd = new double[3] { 15.0, 80.0, 20.0 };


    private struct Hpt
    {
      public double Alt;
      public double Val;
      public Hpt( double alt, double val ) { Alt = alt; Val = val; }
    }

    // Mappings must be provided in increasing
    // order of altitude.
    //
    // Altitudes below the first entry use the
    // hue of the first entry; altitudes above
    // the last entry use the hue of the last
    // entry.
    private static readonly Hpt[] _hpoints = new Hpt[3] {
      new Hpt( 2_000.0, 20.0 ),     // orange
      new Hpt( 10_000.0, 140.0 ),   // light green
      new Hpt( 40_000.0, 300.0 ) }; // magenta

    // determines a color based on type and altitude
    // provides JS W3 values (360/100/100)
    public static double[] HSLj_ColorByAlt( ColorType type, double altitude )
    {
      if (type == ColorType.Unknown) return _unk;
      if (type == ColorType.OnGround) return _gnd;
      if (type == ColorType.InAir) {
        // S,L parts are predefined
        var s = 85;
        var l = 50;

        // H part: find the pair of points the current altitude lies between, and interpolate the hue between those points
        var h = _hpoints[0].Val;
        for (var i = _hpoints.Length - 1; i >= 0; --i) {
          if (altitude > _hpoints[i].Alt) {
            if (i == _hpoints.Length - 1) {
              h = _hpoints[i].Val;
            }
            else {
              h = _hpoints[i].Val + (_hpoints[i + 1].Val - _hpoints[i].Val) * (altitude - _hpoints[i].Alt) / (_hpoints[i + 1].Alt - _hpoints[i].Alt);
            }
            break;
          }
        }
        // fix out of range values
        if (h < 0) {
          h = (h % 360) + 360;
        }
        else if (h >= 360) {
          h = h % 360;
        }

        if (s < 5) s = 5;
        else if (s > 95) s = 95;

        if (l < 5) l = 5;
        else if (l > 95) l = 95;
        return new double[] { h, s, l };
      }
      // type cannot be found
      return _unk;
    }

    public static Color RGB_ColorByAlt( ColorType type, double altitude )
    {
      var hsl = HSLj_ColorByAlt( type, altitude );
      return ColorScale.ColorFromHSL( hsl[0] / 360.0, hsl[1] / 100.0, hsl[2] / 100.0 );
    }

    // returns a color based on altitude
    public static Color AltitudeColor( double alt, bool onGround )
    {
      if (onGround) return RGB_ColorByAlt( ColorType.OnGround, 0 );
      if (double.IsNaN( alt )) return RGB_ColorByAlt( ColorType.Unknown, 0 );

      return RGB_ColorByAlt( ColorType.InAir, alt );
    }

  }

}
