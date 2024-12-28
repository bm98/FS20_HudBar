using System;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// The Trigger types supported
  /// </summary>
  enum TriggerType
  {
    /// <summary>
    /// Undef Initial Value
    /// </summary>
    Undef = -1,
    /// <summary>
    /// A binary trigger
    /// </summary>
    Binary = 0,
    /// <summary>
    /// A level based trigger (float)
    ///  Requires a TriggerBand to be defined for each level
    /// </summary>
    LevelChainF,
    /// <summary>
    /// A level based trigger (int)
    /// </summary>
    LevelChainI,

  }

  /// <summary>
  /// Defines an event with it's callback
  /// </summary>
  abstract class EventProc<T>
  {
    /// <summary>
    /// The stored TriggerType
    /// </summary>
    protected TriggerType _triggerType = TriggerType.Undef;

    /// <summary>
    /// The TriggerBand detecting InBand values
    /// </summary>
    public abstract ILevelDetectorAPI Detector { get; set; }

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    public abstract void SetLevel( T level );
    /// <summary>
    /// Set triggers of this Proc
    /// </summary>
    public abstract void SetTrigger( );
    /// <summary>
    /// Reset triggers of this Proc
    /// </summary>
    public abstract void ResetTrigger( );

    /// <summary>
    /// Defines the action with one string parameter to execute
    /// </summary>
    public Action<string> Callback { get; set; }
    /// <summary>
    /// The Text to say for this Event
    /// </summary>
    public string Text { get; set; }
  }


}
