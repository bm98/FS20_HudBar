using System;
using System.Linq;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A template for Binary Triggers
  ///  triggers the action when the BinaryEventProcs TriggerState is seen
  ///  There can only be one True and one False EventProc
  /// </summary>
  abstract class TriggerBinary : TriggerBase<bool>
  {
    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerBinary( GUI.GUI_Speech speaker )
      : base( speaker )
    {
    }

    /// <summary>
    /// Detect binary state changes and trigger registered callbacks if there are
    /// </summary>
    /// <param name="state"></param>
    protected override bool DetectStateChange( bool level )
    {
      bool callout = false;
      // goes through all registered levels and triggers the first fitting one if not done before
      // will fire/reset the rest if needed
      try {
        EventProc<bool> todo = null;

        foreach (var action in _actions) {
          if ((action.Value.Detector as LevelDetectorBase<bool>).LevelDetected( level )) {
            // capture the first to trigger
            if (!_inhibited) {
              todo = action.Value;
            }
          }
        }
        // call if needed
        if (todo != null) {
          todo.Callback.Invoke( todo.Text );
          callout = true;
        }
      }
      catch {
        // ignore, just don't bail out...
      }
      return callout;
    }

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    /// <param name="itemIndex">the item index</param>
    public override void SetLevel( bool level, int itemIndex )
    {
      // sanity 
      if (itemIndex >= _actions.Count) return;

      (_actions.ElementAt( itemIndex ).Value.Detector as LevelDetectorBase<bool>).Level = level;
    }

    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  There can only be one True and one False EventProc
    ///  Overwrites any existing one for the submitted state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public override void AddProc( EventProc<bool> callback )
    {
      if (!(callback is EventProcBinary)) throw new ArgumentException( "Requires a BinaryEventProc as argument" ); // Program ERROR

      // override existing ones
      _actions.TryRemove( (callback.Detector as LevelDetectorBase<bool>).Level, out _ );
      _actions.TryAdd( (callback.Detector as LevelDetectorBase<bool>).Level, callback );

    }

  }
}
