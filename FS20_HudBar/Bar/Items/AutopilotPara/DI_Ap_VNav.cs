﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;


namespace FS20_HudBar.Bar.Items
{
  internal class DI_Ap_VNav : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_VNAV;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "VNAV";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP VNAV";

    private readonly B_Base _label;

    public DI_Ap_VNav( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "VNAV Hold\nClick to toggle";

      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.AP_VNAV; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      _label.ButtonClicked += _label_ButtonClicked;

      AddObserver( Desc, 5, OnDataArrival);
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        SV.Set( SItem.bGS_Ap_VNAV_active, true ); // toggle
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        this.ColorType.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_VNAV_active ) ? cTxAPActive : cTxLabel;
      }
    }

  }
}
