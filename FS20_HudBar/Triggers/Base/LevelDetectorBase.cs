using System;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// LevelDetector base class
  ///  supporting Fired and Inhibit means
  /// </summary>
  internal abstract class LevelDetectorBase<T> : ILevelDetectorAPI where T : IComparable<T>
  {
    // fired flag
    protected bool _hasFired = false;

    /// <summary>
    /// cTor:
    /// </summary>
    protected LevelDetectorBase( bool autoReset )
    {
      AutoReset = autoReset;
    }

    /// <summary>
    /// Wether the trigger resets when outside of the band
    /// </summary>
    public bool AutoReset { get; set; } = false;

    /// <summary>
    /// Set the trigger memory
    /// </summary>
    public virtual void SetTrigger( ) => _hasFired = true;

    /// <summary>
    /// Reset the trigger memory
    /// </summary>
    public virtual void ResetTrigger( ) => _hasFired = false;

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public virtual T Level { get; set; } = default;

    /// <summary>
    /// True when the detector condition is met
    /// </summary>
    /// <param name="level">Value to test</param>
    /// <returns>True when detected</returns>
    public abstract bool LevelDetected( T level );

    /// <summary>
    /// Addition helper, will add if types allow, else returns default
    /// </summary>
    /// <typeparam name="V">A value type</typeparam>
    /// <param name="v1">Value</param>
    /// <param name="v2">Value to add</param>
    /// <returns>The sum or default</returns>
    protected static V Add<V>( V v1, V v2 ) where V : IComparable<V>
    {
      try {
        dynamic a = v1;
        dynamic b = v2;
        return a + b;
      }
      catch { return default; }
    }
    /// <summary>
    /// Subtraction helper, will subtract v2 from v1 if types allow, else returns default
    /// </summary>
    /// <typeparam name="V">A value type</typeparam>
    /// <param name="v1">Value</param>
    /// <param name="v2">Value to subtract</param>
    /// <returns>The difference or default</returns>
    protected static V Sub<V>( V v1, V v2 ) where V : IComparable<V>
    {
      try {
        dynamic a = v1;
        dynamic b = v2;
        return a - b;
      }
      catch { return default; }
    }

    protected static float AsFloat( object v )
    {
      try {
        return (float)v;
      }
      catch { }
      return float.NaN;
    }
    protected static int AsInt( object v )
    {
      try {
        return (int)v;
      }
      catch { }
      return int.MinValue;
    }
  }
}
