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

    private int _obs2;

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

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
      _obs2 = SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
      if (_obs2 > 0) {
        SC.SimConnectClient.Instance.GpsModule.RemoveObserver( _obs2 );
        _obs2 = 0;
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      WPTracker.InitFlight( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
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
