using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Bar;
using FS20_HudBar.Triggers.Base;

namespace FS20_HudBar.Triggers
{
  class T_WarnFuel : TriggerBinary
  {
    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">not used</param>
    protected override void UpdateStateLow( object dataSource )
    {
      DetectStateChange( Calculator.FuelReachAlert );
    }

    // Implements the means to speak out the Gear State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_WarnFuel( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Fuel Warning";
      m_test = "Low Fuel Alert";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Low Fuel Alert" } );
    }

  }

}

