using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using FSimClientIF.Modules;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// ALT Hold Trigger: a binary trigger where ALT Hold Active => True, else=> False
  /// 
  ///  detects a change in the Glideslope Capture
  ///  triggers one event each time it changed
  ///  
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_AltHold : TriggerBinary
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( m_name, OnDataArrival );
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAP_G1000 object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if ( !Enabled ) return; // not enabled
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity, capture odd cases
      if ( SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround ) return; // not while on ground

      var ds = SC.SimConnectClient.Instance.AP_G1000Module;

      if ( ds.ALT_setting_ft < 8000 ) {
        m_actions.First( ).Value.Text = $"Holding {(int)ds.ALT_setting_ft } feet";
      }
      else {
        m_actions.First( ).Value.Text = $"Holding Flightlevel {(int)( ds.ALT_setting_ft / 100 ) }";
      }
      // only if AP is On
      DetectStateChange( ( ds.AP_mode == FSimClientIF.APMode.On ) && ds.ALT_hold );
      if ( ds.ALT_hold == false )
        m_lastTriggered = false; // RESET if no longer captured
    }

    // Implements the means to speak out the AP - ALT hold State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_AltHold( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "AP ALT Hold";
      m_test = "Holding 5000 feet";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      m_lastTriggered = false;
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Holding" } );
    }

  }

}

