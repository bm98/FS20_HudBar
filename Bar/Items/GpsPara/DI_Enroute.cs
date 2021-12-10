using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Enroute : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ENROUTE;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Enroute";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Enroute Times (WP/Tot)";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Enroute( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Time since last Waypoint - Time since restart\nClick to restart the Enroute timers";

      LabelID = LItem;
      var item = VItem.ENR_WP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_TimeHHMMSS( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ENR_TOTAL;
      _value2 = new V_TimeHHMMSS( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
      SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      WPTracker.InitFlight( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      // Maintain the Waypoint Tracker here to support the GPS Flightplan 
      if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
        // WP Enroute Tracker
        WPTracker.Track(
          SC.SimConnectClient.Instance.GpsModule.WYP_prev,
          SC.SimConnectClient.Instance.GpsModule.WYP_next,
          SC.SimConnectClient.Instance.HudBarModule.SimTime_loc_sec,
          SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround
        );
        // Update Estimate Calculation with Acf data
        Calculator.UpdateValues(
          SC.SimConnectClient.Instance.HudBarModule.Groundspeed_kt,
          SC.SimConnectClient.Instance.HudBarModule.AltMsl_ft,
          SC.SimConnectClient.Instance.HudBarModule.VS_ftPmin
        );
      }

      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
          _value1.Value = WPTracker.WPTimeEnroute_sec;
          _value2.Value = WPTracker.TimeEnroute_sec;
        }
        else {
          // No SIM GPS - Flightplan active
          _value1.Value = null;
          _value2.Value = null;
        }
      }
    }

  }
}
