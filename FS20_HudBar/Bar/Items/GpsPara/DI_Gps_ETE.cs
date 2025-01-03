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
  class DI_Gps_ETE : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_ETE;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "D-ETE";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "GPS Destination ETE";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_ETE( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.GPS_ETE;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_TimeHHMMSS( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 2, OnDataArrival ); // twice per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (HudBar.FlightPlanRef.Tracker.EteDestination_sec > 0) {
          // if >0 use the Tracker data (includes GPS merges)
          _value1.Value = (float)HudBar.FlightPlanRef.Tracker.EteDestination_sec;
          _value1.ItemForeColor = cTxGps;
        }
        else {
          // No Flightplan active or arrived
          _value1.Value = null;
        }
      }
    }

  }
}
