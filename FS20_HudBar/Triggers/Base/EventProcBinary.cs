

using System;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Defines a binary event with it's callback
  /// </summary>
  class EventProcBinary : EventProc<bool>
  {
    protected BinaryDetector _band;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public EventProcBinary( )
    {
      _triggerType = TriggerType.Binary;
    }

    /// <summary>
    /// The Boolean state to trigger the callback
    /// </summary>
    public override ILevelDetectorAPI Detector {
      get => _band; set {
        if (!(value is BinaryDetector)) throw new ArgumentException( "Requires a TriggerBandB as argument" ); // Program ERROR
        _band = (BinaryDetector)value;
      }
    }

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    public override void SetLevel( bool level ) => _band.Level = level;
    /// <summary>
    /// Set triggers of this Proc
    /// </summary>
    public override void SetTrigger( ) => _band.SetTrigger( );
    /// <summary>
    /// Reset triggers of this Proc
    /// </summary>
    public override void ResetTrigger( ) => _band.ResetTrigger( );

  }
}
