using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF.Modules;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Very basic class to derive Triggers from
  /// </summary>
  abstract class TriggerBase : ITriggers
  {
    protected readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // the Voice output ref
    protected GUI.GUI_Speech speakerRef;

    protected int m_observerID = 0; // obs IDs start with 1..
    protected string m_name = "";
    protected string m_test = "";
    protected bool m_enabled = false;

    /// <summary>
    /// Returns a descriptive name of this Voice Trigger Element
    /// </summary>
    public string Name => m_name;

    /// <summary>
    /// Get;Set; enabled state of this Voice Trigger Element
    /// </summary>
    public virtual bool Enabled { get => m_enabled; set => m_enabled = value; }

    /// <summary>
    /// Speak a test sentence
    /// </summary>
    public virtual void Test( GUI.GUI_Speech speech )
    {
      speech.Say( m_test );
    }

    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerBase( GUI.GUI_Speech speaker )
    {
      speakerRef = speaker;
    }

    /// <summary>
    /// Generic Talk routine with one item to say
    /// </summary>
    /// <param name="text">The text to speak out</param>
    protected virtual void Say( string text )
    {
      speakerRef?.Say( text );
    }

    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public abstract void AddProc( EventProc callback );

    /// <summary>
    /// Clears the Event Proc Stack
    /// </summary>
    public abstract void ClearProcs( );

    /// <summary>
    /// Calls to register for dataupdates
    ///   To be implemented in the derived class
    /// </summary>
    public abstract void RegisterObserver( );

    /// <summary>
    /// Calls to un-register for dataupdates
    ///   To be implemented in the derived class
    /// </summary>
    public abstract void UnRegisterObserver( );

    /// <summary>
    /// Update the internal state of this trigger
    ///   To be implemented in the derived class
    /// </summary>
    /// <param name="dataRefName">A dataRefName</param>
    protected abstract void OnDataArrival( string dataRefName );

    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    public abstract void Reset( );

    // generic register method
    protected void RegisterObserver_low( IModule module, Action<string> callback )
    {
      // not registered
      if (m_observerID < 1) {
        var obsID = module.AddObserver( m_name, 2, callback, null );
        m_observerID = (obsID > 0) ? obsID : m_observerID;
      }
    }

    // generic unregister method
    protected void UnregisterObserver_low( IModule module )
    {
      if (m_observerID > 0) {
        module.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }


  }
}
