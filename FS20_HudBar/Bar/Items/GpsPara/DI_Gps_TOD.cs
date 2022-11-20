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
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_Gps_TOD : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_TOD;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "TOD";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Dist to TOD";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gps_TOD( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GPS_TOD;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Dist( valueProto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.GpsModule ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = Conversions.Nm_From_M( SC.SimConnectClient.Instance.GpsModule.TOD_dist);
      }
    }

  }
}