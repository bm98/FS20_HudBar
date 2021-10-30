using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// An interface to be implemented by Triggers
  /// </summary>
  interface ITriggers
  {
    /// <summary>
    /// Returns a descriptive name of this Voice Trigger Element
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Get;Set; enabled state of this Voice Trigger Element
    /// </summary>
    bool Enabled { get; set; }
    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    void AddProc( EventProc callback );

    /// <summary>
    /// Update the internal state of this trigger
    ///   To be implemented in the derived class
    /// </summary>
    /// <param name="dataSource">A datasource</param>
    void UpdateState( object dataSource );

    /// <summary>
    /// Speak a test sentence
    /// </summary>
    void Test( GUI.GUI_Speech speech );

    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    void Reset( );
  }
}
