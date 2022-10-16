using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using FSimClientIF.Modules;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Gear Trigger: a binary trigger where Down=> True, Up=> False
  /// 
  ///  detects a change in the Gearposition completely Down / Up
  ///  triggers one event each time it changed
  ///  
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_Gear : TriggerBinary
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.HudBarModule, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      var ds = SC.SimConnectClient.Instance.HudBarModule;

      if (ds.IsGearRetractable) {
        // only if we have a retractable gear...
        switch (ds.GearPos) {
          case FSimClientIF.GearPosition.Up: // Binary False state
            DetectStateChange( false );
            break;
          case FSimClientIF.GearPosition.Down:// Binary True state
            DetectStateChange( true );
            break;
          default:
            ; // not a defined binary state
            break;
        }
      }

    }

    // Implements the means to speak out the Gear State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Gear( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Gear state";
      m_test = "Gear Down";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { TriggerState = false, Callback = Say, Text = "Gear Up" } );
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Gear Down" } );
    }

  }

}
