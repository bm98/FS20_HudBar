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
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAP_G1000 object from the FSim library</param>
    protected override void UpdateStateLow( object dataSource )
    {
      if ( !( dataSource is IAP_G1000 ) ) throw new ArgumentException( "Needs an IAP_G1000 argument" ); // Program ERROR

      var ds = (dataSource as IAP_G1000);

      if ( ds.ALT_setting_ft < 8000 ) {
        m_actions.First( ).Value.Text = $"Holding {(int)ds.ALT_setting_ft } feet";
      }
      else {
        m_actions.First( ).Value.Text = $"Holding Flightlevel {(int)( ds.ALT_setting_ft / 100 ) }";
      }
      DetectStateChange( ds.ALT_hold );
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

