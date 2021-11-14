﻿using System;
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
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

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
    public static string Short = "APT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "ATC Airport and distance nm";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    // A HudBar standard ToolTip for the Metar Display
    private ToolTip_Base _toolTip = new ToolTip_Base();


    public DI_Atc_APT( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      _metar.MetarDataEvent += _metar_MetarDataEvent;
      LabelID = LItem;
      var item = VItem.ATC_APT;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ATC_DIST;
      _value2 = new V_Dist( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );// use the Location tracer
    }

    private void _metar_MetarDataEvent( object sender, MetarLib.MetarTafDataEventArgs e )
    {
      _metar.Update( e.MetarTafData );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        _metar.Clear( );
        if ( AirportMgr.IsAvailable ) {
          if ( AirportMgr.Location != null )
            _metar.PostMETAR_Request( AirportMgr.AirportICAO, AirportMgr.Location ); // station rec with Location
          else
            _metar.PostMETAR_Request( AirportMgr.AirportICAO ); // station rec
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        // ATC Airport
        if ( AirportMgr.HasChanged )
          _value1.Text = AirportMgr.Read( ); // update only when changed

        if ( _metar.HasNewData ) {
          // avoiding redraw and flicker for every cycle
          _toolTip.SetToolTip( this.Label, _metar.Read( ) );
          this.ColorType.ItemBackColor = _metar.ConditionColor;
        }

        // Distance to Destination
        if ( HudBar.AtcFlightPlan.HasFlightPlan ) {
          _value2.Value = HudBar.AtcFlightPlan.RemainingDist_nm(
            SC.SimConnectClient.Instance.GpsModule.WYP_next,
            SC.SimConnectClient.Instance.GpsModule.WYP_dist );
        }
        else {
          // calc straight distance if we don't have an ATC flightplan with waypoints
          var latLon = new LatLon( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon );
          _value2.Value = AirportMgr.Distance_nm( latLon );
        }
      }
    }

  }
}
