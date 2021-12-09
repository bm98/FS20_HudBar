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
  /// Brakes Trigger: a binary trigger where Applied=> True, Released=> False
  /// 
  ///  detects a change in the Full Brakes 
  ///  triggers one event each time it changed
  ///  
  ///  One need to add BinaryEventProc for True and False
  /// </summary>
  class T_Brakes : TriggerBinary
  {
    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void UpdateStateLow( object dataSource )
    {
      if ( !( dataSource is IHudBar ) ) throw new ArgumentException( "Needs an IHudBar argument" ); // Program ERROR

      var ds = (dataSource as IHudBar);
      DetectStateChange( ds.Parkbrake_on );
    }

    // Implements the means to speak out the Gear State

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Brakes( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Parkingbrake";
      m_test = "Parkingbrake Applied";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcBinary( ) { TriggerState = false, Callback = Say, Text = "Parkingbrake Released" } );
      this.AddProc( new EventProcBinary( ) { TriggerState = true, Callback = Say, Text = "Parkingbrake Set" } );
    }

  }

}

