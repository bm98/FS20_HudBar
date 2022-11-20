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

    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.AP_G1000Module, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.AP_G1000Module ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAP_G1000 object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if ( !m_enabled ) return; // not enabled
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity, capture odd cases
      if ( SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround ) return; // not while on ground

      var ds = SC.SimConnectClient.Instance.AP_G1000Module;
      this.m_actions[true].Text = ds.GPS_active ? _path : _slope; // change according to NAV mode
      DetectStateChange( ds.GS_active );
      if ( ds.GS_active == false )
        m_lastTriggered = false; // RESET if no longer captured
    }

    // Implements the means to speak out the AP Glideslope Active State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Glideslope( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "AP GS Capture";
      m_test = _slope;

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      m_lastTriggered = false;
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = _text } );
    }

  }

}


