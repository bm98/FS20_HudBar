using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A Detector for binary value changes
  ///   The detector will fire only once, the first time InBand is detected
  ///   AutoReset mode will reset the trigger if a value is not InBand
  ///   Non AutoReset mode needs a ResetTrigger() call to enable again
  /// </summary>
  class BinaryDetector : LevelDetectorBase<bool>
  {
    // hidden cTor:
    private BinaryDetector( ) : base( false ) { }

    /// <summary>
    /// cTor: init object
    /// </summary>
    /// <param name="level">The trigger level</param>
    /// <param name="autoReset">When true reset when a value is outside the band</param>
    public BinaryDetector( bool level, bool autoReset )
      : base( autoReset )
    {
      Level = level;
    }

    /// <summary>
    /// A Level to trigger
    /// </summary>
    public override bool Level { get; set; } = false;

    /// <summary>
    /// Returns true if the given value is within the band
    ///  in OneShootMode it will return true once until reset
    /// </summary>
    /// <param name="level">The level value</param>
    /// <returns>True if within the band</returns>
    public override bool LevelDetected( bool level )
    {
      // AutoReset handling
      if (level != Level) { if (AutoReset) _hasFired = false; return false; }

      // in Band now

      // re-trigger will not fire until reset
      if (_hasFired) { return false; }

      // the first time we are InBand
      _hasFired = true;
      return true;
    }

  }
}
