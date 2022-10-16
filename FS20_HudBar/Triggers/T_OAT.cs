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
  /// OAT Trigger: a float trigger where OAT <4 => True
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_OAT : TriggerFloat
  {
    // the OAT we start processing (not above)
    private const float c_detectionOAT = 5f;

    private TSmoother _smooth = new TSmoother( );

    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      RegisterObserver_low( SC.SimConnectClient.Instance.HudBarModule, OnDataArrival ); // use generic
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use generic
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!Enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      var ds = SC.SimConnectClient.Instance.HudBarModule;
      _smooth.Add( ds.OutsideTemperature_degC ); // smoothen

      if (_smooth.GetFloat > c_detectionOAT) {
        // when OAT is above 5 retrigger the alert detection
        m_lastTriggered = c_detectionOAT;
      }
      else {
        // in callout range
        if (_smooth.GetFloat < m_lastTriggered) {
          // only if below last callout
          DetectStateChange( _smooth.GetFloat );
        }
      }

    }


    // Implements the means to speak out the OAT Alerts

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_OAT( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "OAT Icing";
      m_test = "Icing Alert";

      m_lastTriggered = 20; // trigger at start
      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 3.0f, 1.0f ), Callback = Say, Text = "Low Air Temperature" } ); // around 3 °C
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( -5.0f, 5.0f ), Callback = Say, Text = "Icing Alert" } );  // capture 0..-10 change
    }

  }

}

