
using System;

namespace FS20_HudBar.Triggers.Base
{
  class EventProcFloat : EventProc<float>
  {
    protected LevelDetectorBase<float> _band;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public EventProcFloat( )
    {
      _triggerType = TriggerType.LevelChainF;
    }


    /// <summary>
    /// The Float state to trigger the callback
    /// </summary>
    public override ILevelDetectorAPI Detector {
      get => _band; set {
        if (!(value is LevelDetectorBase<float>)) throw new ArgumentException( "Requires a LevelDetectorBase<float> as argument" ); // Program ERROR
        _band = (LevelDetectorBase<float>)value;
      }
    }

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    public override void SetLevel( float level ) => _band.Level = level;
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
