using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Atc_APT : DispItem
  {
    // METAR instance lives throughout the application 
    private static readonly HudMetar _metar = new HudMetar( ); // For the Airport

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ATC_APT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "APT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ATC Airport, Dist. and Alt";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    // A HudBar standard ToolTip for the Metar Display
    private ToolTip_Base _toolTip = new ToolTip_Base( );


    public DI_Atc_APT( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      _metar.MetarDataEvent += _metar_MetarDataEvent;
      LabelID = LItem;
      var item = VItem.ATC_APT;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ATC_APT_DIST;
      _value2 = new V_Dist( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ATC_APT_ALT;
      _value3 = new V_Alt( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      _label.ButtonClicked += _label_ButtonClicked;

      m_observerID = SV.AddObserver( Short, 5, OnDataArrival );// use the Location tracer
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }


    private void _metar_MetarDataEvent( object sender, MetarLib.MetarTafDataEventArgs e )
    {
      _metar.Update( e.MetarTafData );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        _metar.Clear( );
        if (AirportMgr.IsArrAvailable) {
          if (AirportMgr.ArrLocation.IsEmpty)
            _metar.PostMETAR_Request( AirportMgr.ArrAirportICAO ); // station rec
          else
            _metar.PostMETAR_Request( AirportMgr.ArrAirportICAO, AirportMgr.ArrLocation ); // station rec with Location
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // ATC Arrival Airport
        _value1.Text = AirportMgr.ArrAirportICAO;

        // Distance to Destination
        if (HudBar.AtcFlightPlan.HasFlightPlan) {
          _value2.Value = HudBar.AtcFlightPlan.RemainingDist_nm(
              SV.Get<string>( SItem.sG_Gps_WYP_nextID ),
              SV.Get<float>( SItem.fG_Gps_WYP_dist_nm ) );
          _value2.ItemForeColor = cTxGps;
        }
        else {
          // calc straight distance if we don't have an ATC flightplan with waypoints
          var latLon = new LatLon( SV.Get<double>( SItem.dG_Acft_Lat ), SV.Get<double>( SItem.dG_Acft_Lon ) );
          _value2.Value = AirportMgr.ArrDistance_nm( latLon );
          _value2.ItemForeColor = cTxInfo;
        }

        _value3.Value = (AirportMgr.IsArrAvailable && (!AirportMgr.ArrLocation.IsEmpty)) ? (float)Ft_From_M( AirportMgr.ArrLocation.Altitude ) : float.NaN;

        // METAR ToolTip Text and Button Color
        if (_metar.HasNewData) {
          // avoiding redraw and flicker for every cycle
          _toolTip.SetToolTip( this.Label, _metar.Read( ) );
          this.ColorType.ItemBackColor = _metar.ConditionColor;
        }

      }
    }

  }
}
