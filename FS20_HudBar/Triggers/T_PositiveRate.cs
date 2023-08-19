using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      get => m_enabled;
      set {
        m_enabled = value;
      }
    }

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
    /// <param name="dataSource">An IHudBar object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      if (SV.Get<float>( SItem.fG_Eng_RotorMain_rpm) > 0 ) return; // Not with HELI !!

      if (SV.Get<bool>( SItem.bG_Sim_OnGround)) {
        // on ground we disable callouts, this lasts on the way up until a positive rate is detected
        m_lastTriggered = false;
      }
      else {
        // in air
        if (m_lastTriggered ?? false) {
          // was already triggered 
          m_lastTriggered = Calculator.PositiveRate; // this would retrigger in case the detector is reset
        }
        else {
          // wait until triggered
          DetectStateChange( Calculator.PositiveRate );
        }
      }
    }


    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_PositiveRate( GUI.GUI_Speech speaker )
    : base( speaker )
    {
      m_name = "Positive Rate";
      m_test = "Positive Rate";

      // need to set this below the lowest callout level, it will be activated only once we are above our detection RA
      m_lastTriggered = false;

      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Positive Rate" } );
    }

  }

}

