using System;
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


    public DI_DepArr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.DEPARR_DEP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Cursor = Cursors.Hand;
      _label.MouseClick += _label_MouseClick;

      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.DEPARR_ARR;
      _value2 = new V_ICAO( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SV.AddObserver( Short, (int)DataArrival_perSecond, OnDataArrival ); // once per sec
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    private void _label_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      var TTX = new Config.frmApt( );
      // load default
      if (FltPlanMgr.FlightPlan.HasFlightPlan) {
        // dest from FPLan
        TTX.DepAptICAO = FltPlanMgr.FlightPlan.Departure;
        TTX.ArrAptICAO = FltPlanMgr.FlightPlan.Destination;
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
          if (FltPlanMgr.FlightPlan.HasFlightPlan) {
            // update with FP destination
            AirportMgr.UpdateDep( FltPlanMgr.FlightPlan.Departure );
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
          if (FltPlanMgr.FlightPlan.HasFlightPlan) {
            // update with FP destination
            AirportMgr.UpdateArr( FltPlanMgr.FlightPlan.Destination );
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

