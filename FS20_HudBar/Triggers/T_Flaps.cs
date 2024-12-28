using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Flaps Trigger: a float trigger where Down=> 1, Up=> 0
  /// 
  ///  detects a change in the Flapsposition completely Down / Up and levels inbetween 0..1
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_Flaps : TriggerFloat
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, 5, OnDataArrival ); // update 2/sec 
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
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases

      DetectStateChange( SV.Get<float>( SItem.fG_Flp_Deployment_prct ) );
    }

    // Implements the means to speak out the Flaps State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Flaps( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "Flaps state";
      _test = "Flaps Down";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 00f, 5f,0, autoReset: true ), Callback = Say, Text = "Flaps Up" } );//-5-5
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 17f, 8f, 0, autoReset: true ), Callback = Say, Text = "Flaps 20" } );//9-25
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 30f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps 30" } );//25-35
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 40f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps 40" } );//35-45
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 50f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps 50" } );//45-55
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 60f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps 60" } );//55-65
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 70f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps 70" } );//65-75
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 83f, 8f, 0, autoReset: true ), Callback = Say, Text = "Flaps 80" } );//75-91
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 100f, 5f, 0, autoReset: true ), Callback = Say, Text = "Flaps Down" } );//95-105
    }

  }

}
