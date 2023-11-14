using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Bar;
using FS20_HudBar.Triggers.Base;

namespace FS20_HudBar.Triggers
{
  class T_WarnFuel : TriggerBinary
  {
    // use 10 values
    private TSmoother _smooth = new TSmoother( 10 );

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
    /// <param name="dataSource">not used</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!m_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      _smooth.Add( Calculator.FuelReachAlert );
      DetectStateChange( _smooth.GetBool );
    }

    // Implements the means to speak out the Gear State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_WarnFuel( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Fuel Warning";
      m_test = "Low Fuel Alert";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Low Fuel Alert" } );
    }

  }

}

