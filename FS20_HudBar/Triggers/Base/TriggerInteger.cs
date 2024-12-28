using System;
using System.Linq;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A template for Int Level Triggers
  /// </summary>
  abstract class TriggerInteger : TriggerBase<int>
  {
    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerInteger( GUI.GUI_Speech speaker )
      : base( speaker )
    {
    }

    /// <summary>
    /// Detect integer level state changes and trigger registered callbacks if there are
    /// </summary>
    /// <param name="level">The value to evaluate</param>
    protected override bool DetectStateChange( int level )
    {
      bool callout = false;
      // goes through all registered levels and triggers the first fitting one if not done before
      // will fire/reset the rest if needed
      try {
        EventProc<int> todo = null;

        foreach (var action in _actions) {
          if ((action.Value.Detector as LevelDetectorBase<int>).LevelDetected( level )) {
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
    public override void SetLevel( int level, int itemIndex )
    {
      // sanity 
      if (itemIndex >= _actions.Count) return;

      (_actions.ElementAt( itemIndex ).Value.Detector as LevelDetectorBase<int>).Level = level;
    }

    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public override void AddProc( EventProc<int> callback )
    {
      if (!(callback is EventProcInteger)) throw new ArgumentException( "Requires a IntEventProc as argument" ); // Program ERROR

      // override existing ones
      _actions.TryRemove( (callback.Detector as LevelDetectorBase<int>).Level, out _ );
      _actions.TryAdd( (callback.Detector as LevelDetectorBase<int>).Level, callback );
    }

  }
}

