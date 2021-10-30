using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using FS20_HudBar.Triggers.Base;
using FSimClientIF.Modules;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// RA Callout Trigger: a float trigger with 400 ..  10 ft callouts
  ///  should only trigger on the way down and reset if above our initial start RA
  /// 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_RAcallout : TriggerFloat
  {
    // This is an average ground offset of the Radar i.e. the RA readout when the airplane stands on the ground
    // for smaller planes it is around 3ft and up to 10 for the 747 (which does its own readout anyway)
    // so we take 5ft as average for the moment (higher hurts less than lower..)
    private const float c_RAgroundOffset = 5f;

    // the RA we start processing (not above)
    private const float c_detectionRA = 440f + c_RAgroundOffset; // must change if the higest det. level changes !!


    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void UpdateStateLow( object dataSource )
    {
      if ( !( dataSource is IAircraft ) ) throw new ArgumentException( "Needs an IAircraft argument" ); // Program ERROR

      var ds = (dataSource as IAircraft);
      if ( ds.Sim_OnGround ) {
        // on ground we disable callouts, this lasts on the way up until we are above our highest RA level
        m_lastTriggered = -1;
      }
      else if ( ds.AltAoG_ft >= c_detectionRA ) {
        // in air and above our detection RA - reset callout sequence
        m_lastTriggered = c_detectionRA; // set this above the initial callout level to start callouts if we get lower later
      }
      else {
        // in air and in the callout range
        if ( ds.AltAoG_ft < m_lastTriggered ) {
          // detect only when the current RA is lower than the last called out one
          DetectStateChange( ds.AltAoG_ft );
        }
      }
    }

    // Implements the means to speak out the RA down count in ft above ground

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_RAcallout( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "RA Callout";
      m_test = "100";

      // need to set this below the lowest callout level, it will be activated only once we are above our detection RA
      m_lastTriggered = -1;

      // the bands must accomodate for VSpeeds up to ~ -1000 fps on the way down and capture each of the levels with some margin
      // bands must never overlap and should not callout way too early or too late
      // 1000fps is ~ 17 ft/sec, having  about 4 reports/sec is a datapoint resolution of about 4ft 
      // the detector band is set to +-10 above 100, +-5 at 50 and less below - it should therefore catch levels all the time
      // The readout takes about 1.2 sec too - we should hit the level only after the readout finishes to percieve it as on spot (1.2 sec with -500 VS = 10ft)
      // In the final flare at 20,10 are with way less VS (else the plane has crashed..) - 

      // VS >500
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 400.0f, 10.0f ), Callback = Say, Text = "400" } ); // detect in 410..390
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 300.0f, 10.0f ), Callback = Say, Text = "300" } ); // detect in 310..290
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 200.0f, 10.0f ), Callback = Say, Text = "200" } ); // detect in 210..190
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 100.0f, 10.0f ), Callback = Say, Text = "100" } ); // detect in 110.. 90
      // VS ~500
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 54.0f, 4.0f ), Callback = Say, Text = "50" } ); // detect in 58..50
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 44.0f, 4.0f ), Callback = Say, Text = "40" } ); // detect in 48..40
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 34.0f, 4.0f ), Callback = Say, Text = "30" } ); // detect in 38..30
      // VS << 500
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 23.0f, 3.0f ), Callback = Say, Text = "20" } ); // detect in 26..20
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( c_RAgroundOffset + 13.0f, 2.0f ), Callback = Say, Text = "10" } ); // detect in 15..11
    }

  }

}
