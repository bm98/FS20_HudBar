using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A template for Int Level Triggers
  /// </summary>
  abstract class TriggerInteger : TriggerBase
  {
    // the internal state, nullable to provide a distinct reset/default level
    protected int? m_lastTriggered = null;

    // the registered callback list
    protected Dictionary<int, EventProcInteger> m_actions = new Dictionary<int, EventProcInteger>();

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
    protected void DetectStateChange( int level )
    {
      // goes through all registered levels and triggers the first fitting one if not done before
      foreach ( var action in m_actions ) {
        if ( action.Value.TriggerStateI.InBand( level ) ) {
          if ( action.Key != m_lastTriggered ) {
            action.Value.Callback.Invoke( action.Value.Text );
            m_lastTriggered = action.Key; // save triggered level
          }
          break; // hit only the first found
        }
      }

      /* alternative implementation...
        var ac = m_actions.Where( x=> x.Value.TriggerStateI.InBand(level) );
        if ( ac.Count( ) > 0 ) {
          var kv = ac.First();
          if ( kv.Key != m_lastTriggered ) {
            kv.Value.Callback.Invoke( );
            m_lastTriggered = kv.Key; // save triggered level
          }
        }
      */

    }

    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public override void AddProc( EventProc callback )
    {
      if ( !( callback is EventProcInteger ) ) throw new ArgumentException( "Requires a IntEventProc as argument" ); // Program ERROR
      // override existing ones
      if ( m_actions.ContainsKey( callback.TriggerStateI.Level ) )
        m_actions.Remove( callback.TriggerStateI.Level );

      m_actions.Add( callback.TriggerStateI.Level, (EventProcInteger)callback );
    }

    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    public override void Reset( ) => m_lastTriggered = null;


  }
}

