using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;
using System.Linq;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Flaps Trigger: a int trigger for Flaps Deployment Degree or Index
  /// 
  ///  detects a change in the Flaps deployment degree
  ///  triggers one event each time it changed
  ///  
  /// </summary>
  internal class T_FlapsDeg : TriggerBinary
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

    private bool _indexMode = false;
    // Flaps degree state 
    private float _curFlapsDeg = -10000; // trigger first time


    /// <summary>
    /// Toggle the Flaps Handle Index/Degree Mode
    /// </summary>
    public void ToggleIndexMode( ) => _indexMode = !_indexMode;

    /// <summary>
    /// Set the Flaps Handle Index Mode (else it's degree)
    /// </summary>
    /// <param name="indexMode">True for index mode, false for degree</param>
    public void SetIndexMode( bool indexMode ) => _indexMode = indexMode;


    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataRefName">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases

      int flapsDeg = SV.Get<int>( SItem.iG_Flp_Deployment_deg );
      int flapsHandle = SV.Get<int>( SItem.iGS_Flp_HandleIndex );
      bool hasChanged = false;

      // calls when the flaps have settled
      if (flapsDeg != _curFlapsDeg) {
        // only redo the translation when needed
        _actions.First( ).Value.Text =
          (flapsHandle == 0)
            ? "Flaps up"
            : _indexMode
                ? $"Flaps {flapsHandle}"
                : $"Flaps {flapsDeg}";

        _curFlapsDeg = flapsDeg;
        hasChanged = true;
      }

      // auto reset should trigger next time it changes
      DetectStateChange( hasChanged );
    }

    // Implements the means to speak out the Flaps State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_FlapsDeg( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "Flaps deg or handle";
      _test = "Flaps 30";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { Detector = new BinaryDetector( level: true, autoReset: true ), Callback = Say, Text = "Flaps" } );

      this.ResetTrigger( );
    }

  }

}
