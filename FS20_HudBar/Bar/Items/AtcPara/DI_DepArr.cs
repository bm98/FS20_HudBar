﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_DepArr : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.DEPARR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RTE";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Departure / Arrival";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    /// <summary>
    /// cTor:
    /// </summary>
    public DI_DepArr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.DEPARR_DEP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Cursor = Cursors.Hand;
      _label.MouseClick += _label_MouseClick;

      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.DEPARR_ARR;
      _value2 = new V_ICAO( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 0.5f, OnDataArrival );
    }

    // User Enty of Airport ICAOs
    private void _label_MouseClick( object sender, MouseEventArgs e )
    {
      // if (!SC.SimConnectClient.Instance.IsConnected) return; // need to set it even if the Sim is not connected

      var TTX = new Config.frmApt( );
      // load default
      if (HudBar.FlightPlanRef.IsValid) {
        // dest from FPLan
        TTX.DepAptICAO = HudBar.FlightPlanRef.Origin.Icao_Ident;
        TTX.ArrAptICAO = HudBar.FlightPlanRef.Destination.Icao_Ident;
      }
      else {
        // no Flightplan
        if (AirportMgr.IsDepAvailable) {
          // departure from Mgr (prev entry)
          TTX.DepAptICAO = AirportMgr.DepAirportICAO;
        }
        else {
          // no preset
          TTX.DepAptICAO = "";
        }

        if (AirportMgr.IsArrAvailable) {
          // destination from Mgr (prev entry)
          TTX.ArrAptICAO = AirportMgr.ArrAirportICAO;
        }
        else {
          // no preset
          TTX.ArrAptICAO = "";
        }
      }

      if (TTX.ShowDialog( this ) == DialogResult.OK) {
        // Update DEP
        if (string.IsNullOrWhiteSpace( TTX.DepAptICAO )) {
          // empty entry to clear
          if (HudBar.FlightPlanRef.IsValid) {
            // update with FP origin
            AirportMgr.UpdateDep( HudBar.FlightPlanRef.Origin.Icao_Ident );
          }
          else {
            // clear with N.A. airport
            AirportMgr.UpdateDep( AirportMgr.AirportNA_Icao );
          }
        }
        else {
          // user entry - will be checked in the Mgr
          AirportMgr.UpdateDep( TTX.DepAptICAO );
        }
        // Update ARR
        if (string.IsNullOrWhiteSpace( TTX.ArrAptICAO )) {
          // empty entry to clear
          if (HudBar.FlightPlanRef.IsValid) {
            // update with FP destination
            AirportMgr.UpdateArr( HudBar.FlightPlanRef.Destination.Icao_Ident );
          }
          else {
            // clear with N.A. airport
            AirportMgr.UpdateArr( AirportMgr.AirportNA_Icao );
          }
        }
        else {
          // user entry - will be checked in the Mgr
          AirportMgr.UpdateArr( TTX.ArrAptICAO );
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Text = AirportMgr.IsDepAvailable ? AirportMgr.DepAirportICAO : "...."; // default text
        _value2.Text = AirportMgr.IsArrAvailable ? AirportMgr.ArrAirportICAO : "...."; // default text
      }
    }

  }
}

