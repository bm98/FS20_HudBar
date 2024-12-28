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
    public static readonly string Desc = "GPS Distance to Dest. [nm]";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_DST( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.GPS_DST;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Dist( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Distance to Destination
        if (HudBar.FlightPlanRef.Tracker.PlanDistanceRemaining_nm > 0) {
          // if >0 use the Tracker data (includes GPS merges)
          _value1.Value = (float)HudBar.FlightPlanRef.Tracker.PlanDistanceRemaining_nm;
          _value1.ItemForeColor = cTxGps;
        }
        else {
          // calc straight distance if we don't have a tracked flightplan with waypoints
          var latLon = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) );
          _value1.Value = AirportMgr.ArrDistance_nm( latLon );
          _value1.ItemForeColor = cTxInfo;
        }
      }
    }

  }
}

