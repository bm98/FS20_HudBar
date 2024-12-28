using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// OAT Trigger: a float trigger where OAT <4 => True
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_OAT : TriggerFloat
  {
    // the OAT we start processing (not above)
    private const float c_upperDetectionOAT = 5f;

    private readonly TSmoother _smooth = new TSmoother( );

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

      _smooth.Add( SV.Get<float>( SItem.fG_Env_OutsideTemperature_degC ) ); // smoothen

      if (_smooth.GetFloat <= c_upperDetectionOAT) {
        // in callout range
        DetectStateChange( _smooth.GetFloat );
      }
      else {
        // only when OAT gets above det. limit, retrigger the alert detection
        this.ResetTrigger( );
      }
    }


    // Implements the means to speak out the OAT Alerts

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_OAT( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "OAT Icing";
      _test = "Icing Alert";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 3.0f, 1.0f, 2.0f, autoReset: false ), Callback = Say, Text = "Low Air Temperature" } ); //4 .. 1
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 0.0f, 0.5f, 7.0f, autoReset: false ), Callback = Say, Text = "Icing Alert" } );  //0.5 .. -7

      // start triggered to avoid callout at start, will reset if above det. limit
      this.SetTrigger( );
    }

  }

}

