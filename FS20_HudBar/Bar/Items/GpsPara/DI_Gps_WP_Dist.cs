using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Gps_WP_Dist : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_WP_DIST;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "DIST";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "WYP Distance nm";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_WP_Dist( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GPS_WP_DIST;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Dist( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SV.AddObserver( Short, 5, OnDataArrival );
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
        if (SV.Get<bool>( SItem.bG_Gps_FP_active )) {
          _value1.Value = SV.Get<float>( SItem.fG_Gps_WYP_dist_nm );
        }
        else {
          // No SIM GPS - Flightplan active
          _value1.Value = null;
        }
      }
    }

  }
}
