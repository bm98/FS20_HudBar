using System;
using System.Linq;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A template for Float Level Triggers
  /// </summary>
  abstract class TriggerFloat : TriggerBase<float>
  {
    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerFloat( GUI.GUI_Speech speaker )
      : base( speaker )
    {
    }

    /// <summary>
    /// Detect float level state changes and trigger registered callbacks if there are
    /// </summary>
    /// <param name="level">The value to evaluate</param>
    protected override bool DetectStateChange( float level )
    {
      bool callout = false;
      // goes through all registered levels and triggers the first fitting one if not done before
      // will fire/reset the rest if needed
      try {
        EventProc<float> todo = null;

        foreach (var action in _actions) {
          if ((action.Value.Detector as LevelDetectorBase<float>).LevelDetected( level )) {
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
    public override void SetLevel( float level, int itemIndex )
    {
      // sanity 
      if (itemIndex >= _actions.Count) return;

      (_actions.ElementAt( itemIndex ).Value.Detector as LevelDetectorBase<float>).Level = level;
    }

    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public override void AddProc( EventProc<float> callback )
    {
      if (!(callback is EventProcFloat)) throw new ArgumentException( "Requires a FloatEventProc as argument" ); // Program ERROR

      // override existing ones(action.Value.TriggerBand as TriggerBandB)
      _actions.TryRemove( (callback.Detector as LevelDetectorBase<float>).Level, out _ );
      _actions.TryAdd( (callback.Detector as LevelDetectorBase<float>).Level, callback );
    }

  }
}
