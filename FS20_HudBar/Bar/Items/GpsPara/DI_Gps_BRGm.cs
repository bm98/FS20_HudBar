﻿using System;
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
  class DI_Gps_BRGm : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_BRGm;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "BRG";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "GPS Bearing to WYP (mag)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_BRGm( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.GPS_BRGm;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Deg( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 2, OnDataArrival);
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Gps_FP_tracking )) {
          // Wyp BRG is available from GPS (not in our Tracker)
          _value1.Value = SV.Get<float>( SItem.fG_Gps_WYP_brg_mag_degm );
        }
        else {
          // No SIM GPS - Flightplan active
          _value1.Value = null;
        }
      }
    }

  }
}
