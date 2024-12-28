using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using FS20_HudBar.Bar;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Positive Rate Callout Trigger: a float trigger with callout at a positive Netto Vario Avg rate of 120 fpm => 
  ///  should only trigger on the way down and reset if above our initial start RA
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  internal class T_PositiveRate : TriggerBinary
  {
    /// <summary>
    /// Get;Set; enabled state of this Voice Trigger Element
    /// </summary>
    public override bool Enabled {
      get => _enabled;
      set {
        _enabled = value;
      }
    }

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
    /// <param name="dataSource">An IHudBar object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases
      if (SV.Get<float>( SItem.fG_Eng_RotorMain_rpm ) > 0) return; // Not with HELI !!
      if (SV.Get<float>( SItem.fG_Acft_Accel_acftZ_fps2 ) < 0.1f) return; // not accelerating (enough to be considered as takeoff..)


      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) {
        this.ResetTrigger( ); // reset        
        return; // on ground we don't callout
      }

      // in air
      DetectStateChange( Calculator.PositiveRate );
    }


    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_PositiveRate( GUI.GUI_Speech speaker )
    : base( speaker )
    {
      _name = "Positive Rate";
      _test = "Positive Rate";

      // need to set this below the lowest callout level, it will be activated only once we are above our detection RA
      this.AddProc( new EventProcBinary( ) { Detector = new BinaryDetector( level: true, autoReset: false ), Callback = Say, Text = "Positive Rate" } );

      // start with fired, will be reset when on ground, otherwise if created in flight will not trigger
      this.SetTrigger( );
    }

  }

}

