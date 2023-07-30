using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Brakes Trigger: a binary trigger where Applied=> True, Released=> False
  /// 
  ///  detects a change in the Full Brakes 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_Brakes : TriggerBinary
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SV ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!m_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      DetectStateChange( SV.Get<bool>( SItem.bGS_Gear_Parkbrake_on ) );
    }

    // Implements the means to speak out the Gear State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Brakes( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Parkingbrake";
      m_test = "Parkingbrake Set";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { TriggerState = false, Callback = Say, Text = "Parkingbrake Released" } );
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Parkingbrake Set" } );
    }

  }

}

