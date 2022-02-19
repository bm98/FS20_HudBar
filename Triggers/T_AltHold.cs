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
  ///  ALT hold can be the Set Alt or the current Alt dependin whether it is
  ///   triggered by the ALT capture or manually hitting the ALT hold.
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

    // Returns either feet or flightlevel
    // Transition is not known or fixed - we use 8000 feet...
    private string AltText( float alt )
    {
      if ( alt < 8000 ) {
        return $"Holding {( (int)( alt / 100 ) ) * 100} feet"; // get it to hundreds else it is talking way to long
      }
      else {
        return $"Holding Flightlevel {(int)( alt / 100 ) }";
      }
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

      // if capturing ALT
      if ( ds.ALThold_active && ( ds.AP_mode == FSimClientIF.APMode.On) ) {
        // find out what ALT we are holding - if at all
        // ALT hold gets active while approaching the target Alt in ALTS mode
        // or holds the current ALT if the pilot hits the ALT hold button.
        // If the Setting is more than 500ft away from the current ALT we assume the pilot pushed the ALT hold button (best guess...)
        float altHolding = ds.ALT_setting_ft; // target ALT
        if ( altHolding > ( SC.SimConnectClient.Instance.HudBarModule.Altimeter_ft + 500f ) ) {
          // seems ALT button was pressed on the way UP to SET ALT
          altHolding = (int)Math.Ceiling( SC.SimConnectClient.Instance.HudBarModule.Altimeter_ft / 100f ) * 100; // round current ALT UP 
        }
        else if ( altHolding < ( SC.SimConnectClient.Instance.HudBarModule.Altimeter_ft - 500f ) ) {
          // seems ALT button was pressed on the way DOWN to SET ALT
          altHolding = (int)Math.Floor( SC.SimConnectClient.Instance.HudBarModule.Altimeter_ft / 100f ) * 100; // round current ALT DOWN
        }
        m_actions.First( ).Value.Text = AltText( altHolding );
      }

      // trigger Once and only if AP and ALT Hold is On
      DetectStateChange( ( ds.AP_mode == FSimClientIF.APMode.On ) && ds.ALThold_active );
      if ( ds.ALThold_active == false )
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

