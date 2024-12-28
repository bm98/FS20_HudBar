
using System;

namespace FS20_HudBar.Triggers.Base
{
  class EventProcInteger : EventProc<int>
  {
    protected LevelDetectorBase<int> _band;

    /// <summary>
    /// cTor: empty
    /// </summary>
    public EventProcInteger( )
    {
      _triggerType = TriggerType.LevelChainI;
    }

    /// <summary>
    /// The Int state to trigger the callback
    /// </summary>
    public override ILevelDetectorAPI Detector {
      get => _band; set {
        if (!(value is LevelDetectorBase<int>)) throw new ArgumentException( "Requires a LevelDetectorBase<int> as argument" ); // Program ERROR
        _band = (LevelDetectorBase<int>)value;
      }
    }

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    public override void SetLevel( int level ) => _band.Level = level;
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
