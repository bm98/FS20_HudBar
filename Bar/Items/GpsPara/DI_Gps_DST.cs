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
using CoordLib;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Gps_DST : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_DST;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "D-DST";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Distance to Dest. [nm]";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_DST( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GPS_DST;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Dist( valueProto ) { ItemForeColor = cGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Distance to Destination
        if (HudBar.AtcFlightPlan.HasFlightPlan) {
          _value1.Value = HudBar.AtcFlightPlan.RemainingDist_nm(
            SC.SimConnectClient.Instance.GpsModule.WYP_nextID,
            SC.SimConnectClient.Instance.GpsModule.WYP_Dist );
          _value1.ItemForeColor = cGps;
        }
        else {
          // calc straight distance if we don't have an ATC flightplan with waypoints
          var latLon = new LatLon( SC.SimConnectClient.Instance.HudBarModule.Lat, SC.SimConnectClient.Instance.HudBarModule.Lon );
          _value1.Value = AirportMgr.ArrDistance_nm( latLon );
          _value1.ItemForeColor = cInfo;
        }
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}

