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
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Oat_C : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.OAT_C;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "OAT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Outside Air Temp °C";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Oat_C( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // OAT
      var item = VItem.OAT_C;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Temp_C( value2Proto );
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
        _value1.Value = SV.Get<float>( SItem.fG_Env_OutsideTemperature_degC );
        _value1.ItemForeColor = Calculator.IcingCondition ? cTxSubZero : cTxInfo;
      }
    }

  }
}

