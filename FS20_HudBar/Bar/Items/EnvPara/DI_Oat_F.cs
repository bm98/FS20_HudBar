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
  class DI_Oat_F : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.OAT_F;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "OAT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Outside Air Temp °F";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Oat_F( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      // OAT
      var item = VItem.OAT_F;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Temp_F( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SV.Get<float>( SItem.fG_Env_OutsideTemperature_degC );
        _value1.ItemForeColor = Calculator.IcingCondition ? cTxSubZero : cTxInfo;
      }
    }

  }
}

