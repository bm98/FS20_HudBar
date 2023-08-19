using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using CoordLib;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

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
      _value1 = new V_Dist( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SV.AddObserver( Short, (int)DataArrival_perSecond, OnDataArrival ); // once per sec
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Distance to Destination
        if (SV.Get<float>( SItem.fG_Gps_DEST_dist_nm ) > 0) {
          _value1.Value = SV.Get<float>( SItem.fG_Gps_DEST_dist_nm );
          _value1.ItemForeColor = cTxGps;
        }
        else if (HudBar.AtcFlightPlan.HasFlightPlan) {
          _value1.Value = HudBar.AtcFlightPlan.RemainingDist_nm(
               SV.Get<string>( SItem.sG_Gps_WYP_nextID ),
               SV.Get<float>( SItem.fG_Gps_WYP_dist_nm ) );
          _value1.ItemForeColor = cTxGps;
        }
        else {
          // calc straight distance if we don't have an ATC flightplan with waypoints
          var latLon = new LatLon( SV.Get<double>( SItem.dG_Acft_Lat ), SV.Get<double>( SItem.dG_Acft_Lon ) );
          _value1.Value = AirportMgr.ArrDistance_nm( latLon );
          _value1.ItemForeColor = cTxInfo;
        }
      }
    }

  }
}

