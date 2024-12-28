using System;

using SC = SimConnectClient;
using FSimClientIF.Modules;
using System.Collections.Concurrent;

namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// Very basic class to derive Triggers from
  /// </summary>
  abstract class TriggerBase<T> : ITriggersAPI, ITriggers<T>
  {
    protected readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // the Voice output ref
    protected GUI.GUI_Speech _speakerRef;

    protected int _observerID = 0; // obs IDs start with 1..
    protected string _name = "";
    protected string _test = "";
    protected bool _enabled = false;
    protected bool _inhibited = false;

    // the registered callback list
    protected ConcurrentDictionary<T, EventProc<T>> _actions = new ConcurrentDictionary<T, EventProc<T>>( );

    /// <summary>
    /// Returns a descriptive name of this Voice Trigger Element
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Get;Set; enabled state of this Voice Trigger Element
    /// </summary>
    public virtual bool Enabled { get => _enabled; set => _enabled = value; }

    /// <summary>
    /// Speak a test sentence
    /// </summary>
    public virtual void Test( GUI.GUI_Speech speech )
    {
      speech.Say( _test );
    }

    /// <summary>
    /// cTor: get the speaker 
    /// </summary>
    /// <param name="speaker">A GUI_Speech object to talk from</param>
    public TriggerBase( GUI.GUI_Speech speaker )
    {
      _speakerRef = speaker;
    }

    /// <summary>
    /// Generic Talk routine with one item to say
    /// </summary>
    /// <param name="text">The text to speak out</param>
    protected virtual void Say( string text )
    {
      _speakerRef?.Say( text );
    }

    /// <summary>
    /// Detect level state changes and trigger registered callbacks if there are
    /// </summary>
    /// <param name="level">The value to evaluate</param>
    /// <returns>true if a callout was made</returns>
    protected abstract bool DetectStateChange( T level );
    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    /// <param name="itemIndex">the item index</param>
    public abstract void SetLevel( T level, int itemIndex );


    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    public abstract void AddProc( EventProc<T> callback );

    /// <summary>
    /// Clears the Event Proc Stack
    /// </summary>
    public virtual void ClearProcs( ) => _actions.Clear( );

    /// <summary>
    /// Update the internal state of this trigger
    ///   To be implemented in the derived class
    /// </summary>
    /// <param name="dataRefName">A dataRefName</param>
    protected abstract void OnDataArrival( string dataRefName );


    /// <summary>
    /// Set the trigger to fired, will not callout
    /// </summary>
    public virtual void SetTrigger( )
    {
      foreach (var action in _actions) {
        action.Value.SetTrigger( );
      }
    }
    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    public virtual void ResetTrigger( )
    {
      foreach (var action in _actions) {
        action.Value.ResetTrigger( );
      }
    }
    /// <summary>
    /// Set Inhibit state (will detect but not callout while true)
    /// </summary>
    public virtual void Inhibit( bool inhibited ) => _inhibited = inhibited;

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
    /// Generic register method
    /// </summary>
    /// <param name="module">Module to register with</param>
    /// <param name="divider">Update pace divider (base is 10/sec)</param>
    /// <param name="callback">Data update Callback method</param>
    protected void RegisterObserver_low( IModule module, int divider, Action<string> callback )
    {
      // sanity
      if (divider < 1) divider = 1;
      if (divider > 100) divider = 100; // each 10 sec would be very slow...

      // not registered
      if (_observerID < 1) {
        var obsID = module.AddObserver( _name + "$TRIG", divider, callback, null );
        _observerID = (obsID > 0) ? obsID : _observerID;
      }
    }

    // generic unregister method
    protected void UnregisterObserver_low( IModule module )
    {
      if (_observerID > 0) {
        module.RemoveObserver( _observerID );
        _observerID = 0;
      }
    }


  }
}
