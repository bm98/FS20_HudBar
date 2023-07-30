using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!m_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      var ds = SV;
      DetectStateChange( SV.Get<float>( SItem.fGS_Flp_SpoilerHandle_position_prct ) );
    }

    // Implements the means to speak out the Flaps State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Spoilers( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Spoiler, Sp-Brake state";
      m_test = "Spoilers out";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0f, 5f ), Callback = Say, Text = "Spoilers retracted" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 20f, 5f ), Callback = Say, Text = "Spoilers 20" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 30f, 5f ), Callback = Say, Text = "Spoilers 30" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 40f, 5f ), Callback = Say, Text = "Spoilers 40" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 50f, 5f ), Callback = Say, Text = "Spoilers 50" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 60f, 5f ), Callback = Say, Text = "Spoilers 60" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 70f, 5f ), Callback = Say, Text = "Spoilers 70" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 80f, 5f ), Callback = Say, Text = "Spoilers 80" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 1.0f, 5f ), Callback = Say, Text = "Spoilers out" } );
    }

  }

}
