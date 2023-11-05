using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Remarks for Speed and Alt e.g. N0174F255
  /// A Speed Remark
  /// 
  ///   K: Kilometers per hour followed by a four digit value.
  ///   N: Knots followed by a four digit value.
  ///   M: Mach followed by a three digit value.The mach value is converted to knots ground speed assuming standard atmosphere conditions at the given flight altitude.
  ///
  ///   A value of 000 will return NaN for the speed
  ///   
  /// e.g. K0750 (750km/h), N0230 (230 kt), M065 (0.65 Mach) 
  /// 
  /// An Altitude Remark
  /// 
  ///     F :Flight level in three digits.
  ///     S: Metric flight level in three digits of tens of meters. ???
  ///     A: Altitude in hundreds of feet in three digits.
  ///     M: Altitude in tens of meter in four digits.
  ///
  /// e.g. F200 (FL 200), S320 (FLm3200 ), A022 (2200ft), M0710 (7100 m)
  /// </summary>
  public struct SpeedAltRemark
  {
    private Regex _rx;
    private float _value_kt; // allways maintained in Knots
    private float _value_ft; // allways maintained in ft or NaN

    // * Mach Kt conversion (TODO should be relocated to dNet module where other Unit convs are)
    // mach tables for conversion
    private double[] c_AltAbove_ft;
    private double[] c_Mach2Kt;

    // Convert from Kt to Mach at Altitude [ft] defaults to 30_000 ft
    private double Mach_From_Kt( double kt, double alt_ft = 30_000 )
    {
      double ret = kt / 660; // default for below 0 alt
      for (int i = 0; i < c_AltAbove_ft.Length; i++) {
        if (alt_ft >= c_AltAbove_ft[i]) {
          ret = kt / c_Mach2Kt[i]; // recalc while going up - well could be done inversely but then...
        }
      }
      return ret;
    }

    // Convert from Mach to Kt at Altitude [ft] defaults to 30_000 ft
    private double Kt_From_Mach( double mach, double alt_ft = 30_000 )
    {
      double ret = mach * 660; // default for below 0 alt
      for (int i = 0; i < c_AltAbove_ft.Length; i++) {
        if (alt_ft >= c_AltAbove_ft[i]) {
          ret = mach * c_Mach2Kt[i]; // recalc while going up - well could be done inversely but then...
        }
      }
      return ret;
    }

    /// <summary>
    /// The Remark as string
    /// </summary>
    public string Remark { get; set; }
    /// <summary>
    /// True if it is a valid entry
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// True if it is a Speed Entry
    /// </summary>
    public bool IsSpeed => Remark.StartsWith( "N" ) || Remark.StartsWith( "K" );
    /// <summary>
    /// True if it is a Mach Entry
    /// </summary>
    public bool IsMach => Remark.StartsWith( "M" );
    /// <summary>
    /// Returns the content as kt (float.NaN if NOT Valid)
    /// Mach entries will be converted to knots ground speed assuming standard atmosphere conditions at the given flight altitude
    /// </summary>
    public float AsKnots => _value_kt;
    /// <summary>
    /// Returns the content as km/h (float.NaN if NOT Valid)
    /// Mach entries will be converted to km/h ground speed assuming standard atmosphere conditions at the given flight altitude
    /// </summary>
    public float AsKmH => (float)dNetBm98.Units.Kmh_From_Kt( _value_kt );
    /// <summary>
    /// Returns the content as Mach (float.NaN if NOT Valid)
    /// Speed entries will be converted from knots ground speed assuming standard atmosphere conditions at the given flight altitude
    /// </summary>
    public float AsMach => (float)Mach_From_Kt( _value_kt, _value_ft );

    /// <summary>
    /// True if it is an Altitude
    /// </summary>
    public bool IsAlt => Remark.StartsWith( "A" ) || Remark.StartsWith( "M" );
    /// <summary>
    /// True if it is a FlightLevel
    /// </summary>
    public bool IsFL => Remark.StartsWith( "F" ) || Remark.StartsWith( "S" );
    /// <summary>
    /// Returns the content as Feet (float.NaN if NOT Valid)
    /// </summary>
    public float AsFeet => _value_ft;
    /// <summary>
    /// Returns the content as FL (float.NaN if NOT Valid)
    /// </summary>
    public float AsFL => IsValid ? (float)Math.Round( _value_ft / 100.0 ) : float.NaN;

    /// <summary>
    /// Returns the content as Meter (float.NaN if NOT Valid)
    /// </summary>
    public float AsMeter => IsValid ? (float)dNetBm98.Units.M_From_Ft( _value_ft ) : float.NaN;


    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="remark">The remark string</param>
    internal SpeedAltRemark( string remark )
    {
      _value_kt = float.NaN;
      _value_ft = float.NaN;
      IsValid = false;
      Remark = remark.Trim( );

      //InitMachTable
      c_AltAbove_ft = new double[] { 0, 10_000, 15_000, 20_000, 25_000, 30_000, 35_000, 40_000 };
      // Kt = Mach * table value if alt is above
      c_Mach2Kt = new double[] { 660.0, 638.0, 627.0, 614.0, 602.0, 590.0, 577.0, 573.0 };

      _rx = new Regex( @"(?<Stag>[KMN])(?<Sval>[0-9]{3,4})(?<Atag>[AFMS])(?<Aval>[0-9]{3,4})", RegexOptions.Compiled | RegexOptions.Singleline );
      Match match = _rx.Match( remark );
      if (match.Success) {
        // Alt entries
        // must kill leading zeors (I think..)
        if (float.TryParse( match.Groups["Aval"].Value.TrimStart( new char[] { '0' } ), out float avalue )) {
          if (match.Groups["Atag"].Value == "A") {
            // from ft
            _value_ft = avalue * 100.0f;
          }
          else if (match.Groups["Atag"].Value == "F") {
            // from FL * 100ft
            _value_ft = avalue * 100.0f;
          }
          else if (match.Groups["Atag"].Value == "M") {
            // from meter *10m
            _value_ft = (float)dNetBm98.Units.Ft_From_M( avalue * 10.0 );
          }
          else if (match.Groups["Atag"].Value == "S") {
            // from FLm *10m
            _value_ft = (float)dNetBm98.Units.Ft_From_M( avalue * 10.0 );
          }
        }
        // Speed entries
        // must kill leading zeors (I think..)
        if (float.TryParse( match.Groups["Sval"].Value.TrimStart( new char[] { '0' } ), out float svalue )) {
          if (match.Groups["Stag"].Value == "K") {
            // from kmh
            _value_kt = (float)dNetBm98.Units.Kt_From_Kmh( svalue );
          }
          else if (match.Groups["Stag"].Value == "M") {
            // from mach
            _value_kt = (float)Kt_From_Mach( svalue, _value_ft );
          }
          else if (match.Groups["Stag"].Value == "N") {
            // from kt
            _value_kt = svalue;
          }
        }
        IsValid = true;
      }
    }

    /// <summary>
    /// Returns an Empty Remark
    /// </summary>
    /// <returns></returns>
    public static SpeedAltRemark Empty => new SpeedAltRemark( "" );

  }
}
