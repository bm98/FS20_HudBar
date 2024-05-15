using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;

using FS20_HudBar.Triggers.Base;
using FS20_HudBar.GUI.Templates;

namespace FS20_HudBar.Triggers
{
  /// <summary>
  /// Alert Trigger: an integer trigger  to send Alert Callouts
  /// 
  /// </summary>
  class T_Alert : TriggerInteger
  {
    /// <summary>
    /// Calls to register for dataupdates
    /// </summary>
    public override void RegisterObserver( )
    {
      // not used
    }
    /// <summary>
    /// Calls to un-register for dataupdates
    /// </summary>
    public override void UnRegisterObserver( )
    {
      // not used
    }

    /// <summary>
    /// Issue an Callout for the Alert Type
    /// </summary>
    /// <param name="alertType"></param>
    public void IssueState( AlertType alertType )
    {
      if (!m_enabled) return; // not enabled

      DetectStateChange( (int)alertType );
      Reset( ); // immediately reset and accept new ones
    }

    /// <summary>
    /// Update the internal state from the datasource
    /// </summary>
    /// <param name="dataSource">An IAircraft object from the FSim library</param>
    protected override void OnDataArrival( string dataRefName )
    {
      if (!m_enabled) return; // not enabled
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity, capture odd cases

      // not used
    }

    // Implements the means to speak out the Alert

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="speaker">A valid Speech obj to speak from</param>
    public T_Alert( GUI.GUI_Speech speaker )
      : base( speaker )
    {
      m_name = "Alerts";
      m_test = "Alert";

      // add the proc most likely to be hit as the first - saves some computing time on the long run
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.ALT, 0 ), Callback = Say, Text = "Altitude" } );
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.AOG, 0 ), Callback = Say, Text = "Ground" } );
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.VS, 0 ), Callback = Say, Text = "Vertical Rate" } );
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.SPD, 0 ), Callback = Say, Text = "Airspeed" } );
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.DIST, 0 ), Callback = Say, Text = "Distance" } );
      this.AddProc( new EventProcInteger( ) { TriggerStateI = new TriggerBandI( (int)AlertType.TIME, 0 ), Callback = Say, Text = "Time" } );
    }
  }
}
