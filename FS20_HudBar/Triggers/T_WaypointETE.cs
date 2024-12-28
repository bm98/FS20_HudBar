using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// WaypointID ETE Trigger: a float trigger where 60,30,10 seconds to go
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_WaypointETE : TriggerFloat
  {
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
    /// <param name="dataSource">An IGps object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      // sanity
      if (!_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // capture odd cases
      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) return; // not while on ground

      // are we tracking a flightplan i.e. having GPS distance to waypoint
      if (SV.Get<bool>( SItem.bG_Gps_FP_tracking ) || FS20_HudBar.Bar.HudBar.FlightPlanRef.Tracker.IsTracking) {
        var time = (float)SV.Get<double>( SItem.dG_Gps_WYP_ete_sec );

        // start detection only if in viable range and allow for reset
        if ((time < 100) && (time > 15)) {
          DetectStateChange( (float)SV.Get<double>( SItem.dG_Gps_WYP_ete_sec ) );
        }
      }
    }

    // Implements the means to speak out the WaypointID ETE in seconds

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_WaypointETE( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      _name = "GPS Waypoint sec.";
      _test = "Waypoint in 60";

      // add the closest band first
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 30.0f, 3.0f, 5.0f, true ), Callback = Say, Text = "Waypoint in 30" } ); // 33 .. 25
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 60.0f, 3.0f, 6.0f, true ), Callback = Say, Text = "Waypoint in 60" } ); // 63 .. 54
      this.AddProc( new EventProcFloat( ) { Detector = new BandDetector<float>( 90.0f, 3.0f, 8.0f, true ), Callback = Say, Text = "Waypoint in 90" } ); // 93 .. 82
    }

  }

}
