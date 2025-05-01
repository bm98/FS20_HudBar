using System.Collections.Generic;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Defines the Unit Mode for a particular item 
  /// There is no global setting as some properties may need to be in SI other in Imp at the same time
  /// 
  /// There is a fixed set of conversions
  /// SI           Imperial
  /// meters    => feet
  /// kilometer => nm
  /// °C        => °F
  /// hPa       => inHg
  /// kg        => lb (lbs)
  /// liter     => gallons
  /// 
  /// </summary>
  public enum UnitMode
  {
    Metric=0,
    Imperial,
  }

  /// <summary>
  /// SI Units
  /// </summary>
  public enum Unit
  {
    Meter=0,
    Kilometer,
    Kilogram,
    Liter,
    HPA,
    DegC,

    // Corresponding IMP Units - must match the order above and 100 must be the first value
    Foot=100,
    NauticalMile,
    LPound,
    Gallon,
    InHg,
    DegF,
  }


  /// <summary>
  /// A Class supporting Value handling with SI and IMP Unit support
  /// </summary>
  class UnitValue
  {

    /// <summary>
    /// The SI Unit of this Value
    /// </summary>
    public Unit SI { get; private set; } = Unit.DegC;
    /// <summary>
    /// The SI Unit Name of this Value
    /// </summary>
    public string SIname => Units.UnitName( SI );

    /// <summary>
    /// The IMP Unit of this Value
    /// </summary>
    public Unit IMP { get; private set; } = Unit.DegF;
    /// <summary>
    /// The IMP Unit Name of this Value
    /// </summary>
    public string IMPname => Units.UnitName( IMP );

    // The base Value in SI
    private float _value_SI = 0;

    /// <summary>
    /// cTor:  
    /// </summary>
    /// <param name="si">The SI Unit, the IMP unit is implicitely assumed</param>
    public UnitValue( Unit si )
    {
      SI = si;
      IMP = Units.IMP( SI );
    }

    /// <summary>
    /// Get; Set; The SI value
    /// </summary>
    public float SIvalue {
      get => _value_SI;
      set => _value_SI = value;
    }

    /// <summary>
    /// Get; Set; The IMP value
    /// </summary>
    public float IMPvalue {
      get => Units.FromUnit( SI, _value_SI );
      set => _value_SI = Units.ToUnit( SI, value );
    }
  }

  /// <summary>
  /// Unit Converter
  /// </summary>
  static class Units
  {
    private const float c_ftPerM = 1f;
    private const float c_nmPerkm = 1f;
    private const float c_FPerC = 1f;
    private const float c_inHgPerHPA = 1f;
    private const float c_lbsPerKg = 1f;
    private const float c_galPerL = 1f;

    private static Dictionary<Unit, string> _unitNames = new Dictionary<Unit, string>(){
      {Unit.Meter,"m" }, {Unit.Foot,"ft" },
      {Unit.Kilometer,"km" }, {Unit.NauticalMile,"nm" },
      {Unit.Kilogram,"kg" }, {Unit.LPound,"lb" },
      {Unit.Liter,"l" }, {Unit.Gallon,"gal" },
      {Unit.HPA,"hPa" }, {Unit.InHg,"inHg" },
      {Unit.DegC,"°C" }, {Unit.DegF,"°F" },
    };

    /// <summary>
    /// Return the Name for a Unit
    /// </summary>
    /// <param name="unit">The Unit</param>
    /// <returns>The name as string</returns>
    public static string UnitName( Unit unit )
    {
      try {
        return _unitNames[unit];
      }
      catch {
        return "UUU"; // Unit failure, Program Error ...
      }
    }

    /// <summary>
    /// Convert a value FROM a unit
    /// </summary>
    /// <param name="unit">The Source Unit</param>
    /// <param name="value">The value</param>
    /// <returns>The converted Value</returns>
    public static float FromUnit( Unit unit, float value )
    {
      switch ( unit ) {
        case Unit.Meter: return value * c_ftPerM;
        case Unit.Kilometer: return value * c_nmPerkm;
        case Unit.DegC: return value * c_FPerC;
        case Unit.HPA: return value * c_inHgPerHPA;
        case Unit.Kilogram: return value * c_lbsPerKg;
        case Unit.Liter: return value * c_galPerL;

        case Unit.Foot: return value / c_ftPerM;
        case Unit.NauticalMile: return value / c_nmPerkm;
        case Unit.DegF: return value / c_FPerC;
        case Unit.InHg: return value / c_inHgPerHPA;
        case Unit.LPound: return value / c_lbsPerKg;
        case Unit.Gallon: return value / c_galPerL;

        default: return float.NaN;
      }
    }

    /// <summary>
    /// Convert a value TO a unit
    /// </summary>
    /// <param name="unit">The Target Unit</param>
    /// <param name="value">The value</param>
    /// <returns>The converted Value</returns>
    public static float ToUnit( Unit unit, float value )
    {
      switch ( unit ) {
        case Unit.Foot: return value * c_ftPerM;
        case Unit.NauticalMile: return value * c_nmPerkm;
        case Unit.DegF: return value * c_FPerC;
        case Unit.InHg: return value * c_inHgPerHPA;
        case Unit.LPound: return value * c_lbsPerKg;
        case Unit.Gallon: return value * c_galPerL;

        case Unit.Meter: return value / c_ftPerM;
        case Unit.Kilometer: return value / c_nmPerkm;
        case Unit.DegC: return value / c_FPerC;
        case Unit.HPA: return value / c_inHgPerHPA;
        case Unit.Kilogram: return value / c_lbsPerKg;
        case Unit.Liter: return value / c_galPerL;
        default: return float.NaN;
      }
    }

    /// <summary>
    /// Returns the IMP unit for a SI unit assumed by our implementation
    /// </summary>
    /// <param name="si">The SI unit</param>
    /// <returns>The IMP unit</returns>
    public static Unit IMP(Unit si )
    {
      return (Unit)( (int)si + 100 ); // now this is ugly.. - but then it serves the trivial purpose
    }
  }
}
