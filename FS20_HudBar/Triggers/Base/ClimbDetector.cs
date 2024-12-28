using dNetBm98;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Detects when a value gets above the Level with reset margin
  ///  it will stay true until a value is outside the reset band for AutoReset
  ///  or until Reset by the user
  ///  
  /// </summary>
  internal class ClimbDetector : LevelDetectorBase<float>
  {
    // hidden cTor:
    private ClimbDetector( ) : base( false ) { }

    // trigger level
    protected float _level;

    // Maximum Reset Value
    protected float _maxReset;
    // Minimum Reset Value
    protected float _minReset;
    // dive detector
    protected SlopeDetector<float> _slopeDetector;

    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="resetBand">Reset band (will adjust to at least trigger band)</param>
    /// <param name="autoReset">When true reset when a value is outside the band</param>
    public ClimbDetector( float level, float resetBand, bool autoReset )
      : base( autoReset )
    {
      ResetBand = resetBand;

      // level last, so it calculated the min/max from Bands
      Level = level;
    }

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public override float Level {
      get => _level;
      set {
        _level = value;
        _slopeDetector = new SlopeDetector<float>( Slope.FromBelow, _level );
        _maxReset = Add( Level, ResetBand );
        _minReset = Sub( Level, ResetBand );
      }
    }

    /// <summary>
    /// Reset Band -+ around Level
    /// </summary>
    public float ResetBand { get; private set; } = default;

    /// <summary>
    /// Returns true if the given value is within the band
    ///  in OneShootMode it will return true once until reset
    /// </summary>
    /// <param name="level">The level value</param>
    /// <returns>True if within the band</returns>
    public override bool LevelDetected( float level )
    {
      // outside checks and AutoReset handling
      if ((level.CompareTo( _maxReset ) > 0) && AutoReset) { _hasFired = false; }
      if ((level.CompareTo( _minReset ) < 0) && AutoReset) { _hasFired = false; }

      // trigger band check
      _slopeDetector.Update( level );
      if (_slopeDetector.Read( )) {
        // dive through detected

        // re-trigger will not fire until reset
        if (_hasFired) { return false; }

        // the first time we are InBand
        _hasFired = true;
        return true;
      }
      return false;
    }


  }
}
