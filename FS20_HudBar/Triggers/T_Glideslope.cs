using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Glideslope Trigger: a binary trigger where Glideslope Hold Active => True, else=> False
  /// 
  ///  detects a change in the Glideslope Capture
  ///  triggers one event each time it changed
  ///  
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_Glideslope : TriggerBinary
  {
    private const string _slope = "Glideslope";
    private const string _path = "Glidepath";
    private string _text = _slope;

    // flag GP mode
    private bool _curPathMode = false;

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
    /// <param name="dataSource">An IAP_G1000 object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases
      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) return; // not while on ground

      // assign GS or GP text if changed
      var pathMode = SV.Get<bool>( SItem.bG_Ap_GP_tracking );
      if (pathMode != _curPathMode) {
        this._actions[true].Text = pathMode ? _path : _slope; // GS may be active even on GP (then both are..)
        _curPathMode = pathMode;
      }

      DetectStateChange( SV.Get<bool>( SItem.bG_Ap_GS_tracking ) || SV.Get<bool>( SItem.bG_Ap_GP_tracking ) );

      // reset condition ?
      if ((SV.Get<bool>( SItem.bG_Ap_GS_tracking ) == false)
        && (SV.Get<bool>( SItem.bG_Ap_GP_tracking ) == false)) {
        this.ResetTrigger( );// RESET if no longer captured
      }
    }

    // Implements the means to speak out the AP Glideslope Active State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Glideslope( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "AP GS Capture";
      _test = _slope;

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { Detector = new BinaryDetector( level: true, autoReset: false ), Callback = Say, Text = _text } );
      this.ResetTrigger( );
    }

  }

}


