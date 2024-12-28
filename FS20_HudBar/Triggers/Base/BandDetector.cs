using System;

using static dNetBm98.XMath;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A Detector using a Band around the trigger level
  ///   The detector will fire only once, the first time InBand is detected
  ///   AutoReset mode will reset the trigger if a value is not InBand
  ///   Non AutoReset mode needs a ResetTrigger() call to enable again
  /// </summary>
  internal class BandDetector<T> : LevelDetectorBase<T> where T : IComparable<T>
  {
    // hidden cTor:
    private BandDetector( ) : base( false ) { }

    // trigger level
    protected T _level;

    // Maximum Trigger Value
    protected T _maxTrigger;
    // Minimum Trigger Value
    protected T _minTrigger;
    // Maximum Reset Value
    protected T _maxReset;
    // Minimum Reset Value
    protected T _minReset;


    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="symBand">A symmetric band</param>
    /// <param name="resetBand">Reset band (will adjust to at least trigger band)</param>
    /// <param name="autoReset">When true reset when a value is outside the band</param>
    public BandDetector( T level, T symBand, T resetBand, bool autoReset )
      : base( autoReset )
    {
      AboveBand = symBand;
      BelowBand = symBand;
      // sanity - the reset band must at least cover the triggerband
      ResetBand = Max( resetBand, symBand );

      // level last, so it calculated the min/max from Bands
      Level = level;
    }

    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="above">Above Level band limit</param>
    /// <param name="below">Below Level band limit</param>
    /// <param name="resetBand">Reset band (will adjust to at least trigger band)</param>
    /// <param name="autoReset">When true reset when a value is outside the band</param>
    public BandDetector( T level, T above, T below, T resetBand, bool autoReset )
      : base( autoReset )
    {
      AboveBand = above;
      BelowBand = below;
      // sanity - the reset band must at least cover the triggerband
      ResetBand = Max( resetBand, Max( above, below ) );

      // level last, so it calculated the min/max from Bands
      Level = level;
    }

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public override T Level {
      get => _level;
      set {
        _level = value;
        _maxTrigger = Add( Level, AboveBand );
        _minTrigger = Sub( Level, BelowBand );
        _maxReset = Add( Level, ResetBand );
        _minReset = Sub( Level, ResetBand );
      }
    }
    /// <summary>
    /// A Band around the Level (+Above -Below) where 
    /// the trigger is detected once it enters the band and 
    /// not retriggered until it has left the band
    /// NOTE: it is not recommended to set this to 0f as it 
    /// would require a match of float numbers which leads to an unpredictable behavior
    /// </summary>
    public T AboveBand { get; private set; } = default;
    /// <summary>
    /// A Band around the Level (+Above -Below) where 
    /// the trigger is detected once it enters the band and 
    /// not retriggered until it has left the band
    /// NOTE: it is not recommended to set this to 0f as it 
    /// would require a match of float numbers which leads to an unpredictable behavior
    /// </summary>
    public T BelowBand { get; private set; } = default;

    /// <summary>
    /// Reset Band -+ around Level
    /// </summary>
    public T ResetBand { get; private set; } = default;


    /// <summary>
    /// Returns true if the given value is within the band
    ///  in OneShootMode it will return true once until reset
    /// </summary>
    /// <param name="level">The level value</param>
    /// <returns>True if within the band</returns>
    public override bool LevelDetected( T level )
    {
      // outside checks and AutoReset handling
      if ((level.CompareTo( _maxReset ) > 0) && AutoReset) { _hasFired = false; }
      if ((level.CompareTo( _minReset ) < 0) && AutoReset) { _hasFired = false; }

      // trigger band check
      if (level.CompareTo( _maxTrigger ) > 0) { return false; }
      if (level.CompareTo( _minTrigger ) < 0) { return false; }

      // in Band now

      // re-trigger will not fire until reset
      if (_hasFired) { return false; }

      // the first time we are InBand
      _hasFired = true;
      return true;
    }

  }
}
