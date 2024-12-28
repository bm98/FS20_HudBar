using static dNetBm98.Units;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// RA Callout Trigger: a float trigger with 400 ..  10 ft callouts
  ///  should only trigger on the way down and reset if above our initial start RA
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_RAcallout : TriggerFloat
  {
    // This is an average ground offset of the Radar i.e. the RA readout when the airplane stands on the ground
    // for smaller planes it is around 3ft and up to 10 for the 747 (which does its own readout anyway)
    // so we take 5ft as average for the moment (higher hurts less than lower..)
    private const float c_RAgroundOffset = 5f;

    // the RA we start processing (not above)
    private const float c_detectionRA = 440f + c_RAgroundOffset; // must change if the higest det. level changes !!

    // touch down detector
    private TouchDownTrigger _tdTrigger = null;

    /// <summary>
    /// Get;Set; enabled state of this Voice Trigger Element
    /// </summary>
    public override bool Enabled {
      get => _enabled;
      set {
        _enabled = value;
        _tdTrigger.Enabled = value;
      }
    }

    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, 2, OnDataArrival );  // update 5/sec 
      _tdTrigger.RegisterObserver( );
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SV ); // use generic
      _tdTrigger.UnRegisterObserver( );
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases

      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) {
        this.Inhibit( true ); // disable when on ground (until reaching det height)

        // on ground we don't callout
        return;
      }

      if (SV.Get<float>( SItem.fGS_Acft_AltAoG_ft ) < c_detectionRA) {
        // in air and in the callout range
        // detect only when the current RA has not been called yet
        DetectStateChange( SV.Get<float>( SItem.fGS_Acft_AltAoG_ft ) );
      }
      else {
        // enable when in air and above our detection RA
        this.Inhibit( false ); // enable when reaching det. height
      }
    }

    // Implements the means to speak out the RA down count in ft above ground

    /// <summary>
    /// Set Metric height callouts if true, else Imperial
    /// </summary>
    /// <param name="setting">True for Meters</param>
    public void SetMetric( bool setting )
    {
      if (setting)
        SetMetricCallout( );
      else
        SetImperialCallout( );
    }

    // Imperial units (ft)
    private void SetImperialCallout( )
    {
      this.ClearProcs( );

      // the bands must accomodate for VSpeeds up to ~ -1000 fps on the way down and capture each of the levels with some margin
      // bands must never overlap and should not callout way too early or too late
      // 1000fps is ~ 17 ft/sec, having  about 4 reports/sec is a datapoint resolution of about 4ft 
      // the detector band is set to +-10 above 100, +-5 at 50 and less below - it should therefore catch levels all the time
      // The readout takes about 1.2 sec too - we should hit the level only after the readout finishes to percieve it as on spot (1.2 sec with -500 VS = 10ft)
      // In the final flare at 20,10 are with way less VS (else the plane has crashed..) - 

      // VS >500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 410.0f, 80f, autoReset: true ), Callback = Say, Text = "400" } ); // detect 410, reset +-80
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 310.0f, 80f, autoReset: true ), Callback = Say, Text = "300" } ); // detect 310, reset +-80
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 210.0f, 80f, autoReset: true ), Callback = Say, Text = "200" } ); // detect 210, reset +-80
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 110.0f, 50f, autoReset: true ), Callback = Say, Text = "100" } ); // detect 110, reset +-50
      // VS ~500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 58.0f, 20f, autoReset: true ), Callback = Say, Text = "50" } ); // detect 58, reset +-20
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 48.0f, 20f, autoReset: true ), Callback = Say, Text = "40" } ); // detect 48, reset +-20
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 38.0f, 20f, autoReset: true ), Callback = Say, Text = "30" } ); // detect 38, reset +-20
      // VS << 500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 26.0f, 10f, autoReset: true ), Callback = Say, Text = "20" } ); // detect 26, reset +-10
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + 15.0f, 10f, autoReset: true ), Callback = Say, Text = "10" } ); // detect 15, reset +-10
    }

    // Metruc units (m)
    private void SetMetricCallout( )
    {
      this.ClearProcs( );

      // logic see above calls for meters at 120, 90, 60, 30,   15, 12, 9,   6, 3 (input and detector is still feet)

      // VS >500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 123 ), 80f, autoReset: true ), Callback = Say, Text = "120" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 93 ), 80f, autoReset: true ), Callback = Say, Text = "90" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 63 ), 80f, autoReset: true ), Callback = Say, Text = "60" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 33 ), 50f, autoReset: true ), Callback = Say, Text = "30" } );
      // VS ~500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 16.3 ), 20f, autoReset: true ), Callback = Say, Text = "15" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 13.3 ), 20f, autoReset: true ), Callback = Say, Text = "12" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 10.3 ), 20f, autoReset: true ), Callback = Say, Text = "9" } );
      // VS << 500
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 7 ), 10f, autoReset: true ), Callback = Say, Text = "6" } );
      this.AddProc( new EventProcFloat( ) { Detector = new DiveDetector( c_RAgroundOffset + (float)Ft_From_M( 4 ), 10f, autoReset: true ), Callback = Say, Text = "3" } );
    }


    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_RAcallout( GUI.GUI_Speech speaker )
    : base( speaker )
    {
      _name = "RA Callout";
      _test = "100";

      _tdTrigger = new TouchDownTrigger( speaker );

      // need to set this below the lowest callout level, it will be activated only once we are above our detection RA
      SetImperialCallout( );

      // start with disabled, will be reset when above det. height
      this.Inhibit( true );
    }

  }

}
