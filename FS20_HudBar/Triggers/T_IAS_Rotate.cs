using System.Linq;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  class T_IAS_Rotate : TriggerFloat
  {
    private TSmoother _smooth = new TSmoother( );

    private float _curRotSpeed = float.MinValue;

    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SV, 2, OnDataArrival );  // update 5/sec 
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
      if (SV.Get<float>( SItem.fG_Eng_RotorMain_rpm ) > 0) return; // Not with HELI !!

      // Rotate is only triggered while OnGround and accelerating (else it calls on touchdown)
      if (!SV.Get<bool>( SItem.bG_Sim_OnGround )) return; // not on ground
      if (SV.Get<float>( SItem.fG_Acft_Accel_acftZ_fps2 ) < 0.1f) return; // not accelerating (enough to be considered as takeoff..)

      var rotSpeed = SV.Get<float>( SItem.fG_Dsg_SpeedMinRotation_kt );
      if (rotSpeed < 10) rotSpeed = SV.Get<float>( SItem.fG_Dsg_SpeedTakeoff_kt ); // try takeoff speed (some don't have Rot..)
      if (rotSpeed < 10) return; // not properly set or otherwise not meaningful value from SIM

      // having a RotSpeed here
      if (rotSpeed != _curRotSpeed) {
        this.SetLevel( rotSpeed, 0 );// set the new trigger level
        _curRotSpeed = rotSpeed;
      }

      _smooth.Add( SV.Get<float>( SItem.fG_Acft_IAS_kt ) ); // smoothen
      DetectStateChange( _smooth.GetFloat );

      // reset when below 10 kt - Rotate will only trigger if within limits +-2
      if (SV.Get<float>( SItem.fG_Acft_IAS_kt ) < 10) {
        this.ResetTrigger( );
      }
    }

    // Implements the means to speak out the IAS Rotate

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_IAS_Rotate( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "IAS Rotate";
      _test = "Rotate";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { Detector = new ClimbDetector( 100.0f, 10f, autoReset: false ), Callback = Say, Text = "Rotate" } ); // take from design speeds
    }

  }

}

