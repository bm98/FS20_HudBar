using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;


namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Spoilers / Speedbrakes Trigger: a float trigger where Down=> 1, Up=> 0
  /// 
  ///  detects a change in the Spoiler position completely Down / Up and levels inbetween 0..1
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_Spoilers : TriggerFloat
  {
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
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases
      if (!SV.Get<bool>( SItem.bG_Flp_HasSpoilers )) return; // no Spoilers

      DetectStateChange( SV.Get<float>( SItem.fGS_Flp_SpoilerHandle_position_prct ) );
    }

    // Implements the means to speak out the Spoiler State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Spoilers( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "Spoiler, Sp-Brake state";
      _test = "Spoilers out";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 0f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers retracted" } );//-5-5
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 17f, 8f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 20" } );//9-25
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 30f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 30" } );//25-35
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 40f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 40" } );//35-45
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 50f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 50" } );//45-55
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 60f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 60" } );//55-65
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 70f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 70" } );//65-75
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 83f, 8f, 0, autoReset: true ), Callback = Say, Text = "Spoilers 80" } );//75-91
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 100f, 5f, 0, autoReset: true ), Callback = Say, Text = "Spoilers out" } );//95-105
    }

  }

}
