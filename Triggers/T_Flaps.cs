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
  /// Flaps Trigger: a float trigger where Down=> 1, Up=> 0
  /// 
  ///  detects a change in the Flapsposition completely Down / Up and levels inbetween 0..1
  ///  triggers one event each time it changed
  ///  
  ///  One need to add FloatEventProc for Levels
  /// </summary>
  class T_Flaps : TriggerFloat
  {
    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void UpdateStateLow( object dataSource )
    {
      if ( !( dataSource is IAircraft ) ) throw new ArgumentException( "Needs an IAircraft argument" ); // Program ERROR

      var ds = (dataSource as IAircraft);
      DetectStateChange( ds.FlapsDeployment_prct );
    }

    // Implements the means to speak out the Flaps State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Flaps( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Flaps state";
      m_test = "Flaps Down";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.0f, 0.05f ), Callback = Say, Text = "Flaps Up" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.2f, 0.05f ), Callback = Say, Text = "Flaps 20" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.3f, 0.05f ), Callback = Say, Text = "Flaps 30" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.4f, 0.05f ), Callback = Say, Text = "Flaps 40" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.5f, 0.05f ), Callback = Say, Text = "Flaps 50" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.6f, 0.05f ), Callback = Say, Text = "Flaps 60" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.7f, 0.05f ), Callback = Say, Text = "Flaps 70" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 0.8f, 0.05f ), Callback = Say, Text = "Flaps 80" } );
      this.AddProc( new EventProcFloat( ) { TriggerStateF = new TriggerBandF( 1.0f, 0.05f ), Callback = Say, Text = "Flaps Down" } );
    }

  }

}
