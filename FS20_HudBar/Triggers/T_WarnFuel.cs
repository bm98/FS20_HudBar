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
      RegisterObserver_low( SV, 5, OnDataArrival );  // update 2/sec 
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
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases

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
      _name = "Fuel Warning";
      _test = "Low Fuel Alert";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { Detector = new BinaryDetector( level: true, autoReset: true ), Callback = Say, Text = "Low Fuel Alert" } );
    }

  }

}

