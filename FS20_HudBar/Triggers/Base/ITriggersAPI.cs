
namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// An API interface to be implemented by Triggers for use by HudBar
  /// </summary>
  internal interface ITriggersAPI
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
    /// Set Inhibit state (will detect but not callout while true)
    /// </summary>
    void Inhibit( bool inhibited );

    /// <summary>
    /// Speak a test sentence
    /// </summary>
    void Test( GUI.GUI_Speech speech );

    /// <summary>
    /// Calls to register for dataupdates
    ///   To be implemented in the derived class
    /// </summary>
    void RegisterObserver( );
    /// <summary>
    /// Calls to un register for dataupdates
    ///   To be implemented in the derived class
    /// </summary>
    void UnRegisterObserver( );

  }


  /// <summary>
  /// A typed interface to be implemented by Triggers
  /// </summary>
  internal interface ITriggers<T>
  {
    /// <summary>
    /// Add one Callback (a parameterless void method) for a distinct state
    ///  Overwrites any existing one for the new state
    /// </summary>
    /// <param name="callback">A Callback EventProc</param>
    void AddProc( EventProc<T> callback );

    /// <summary>
    /// Clears the Event Proc Stack
    /// </summary>
    void ClearProcs( );

    /// <summary>
    /// Set a new Level for an item
    /// </summary>
    /// <param name="level">The new Level</param>
    /// <param name="itemIndex">the item index</param>
    void SetLevel( T level, int itemIndex );

    /// <summary>
    /// Set the trigger to fired, will not callout until reset
    /// </summary>
    void SetTrigger( );
    /// <summary>
    /// Reset the trigger to callout the current state on the next update
    /// </summary>
    void ResetTrigger( );
  }

}
