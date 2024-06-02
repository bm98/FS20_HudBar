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
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_Lights : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.Lights;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Lights";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Lights BNSTL";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Lights( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.LIGHTS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Lights( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, (int)(DataArrival_perSecond / 2), OnDataArrival ); // twice per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Consolidated lights (RA colored for Taxi and/or Landing lights on)
        int lightsInt = 0;
        _value1.ItemForeColor = cTxInfo;
        if (SV.Get<CmdMode>( SItem.cmGS_Lit_Beacon ) == CmdMode.On) lightsInt |= (int)V_Lights.Lights.Beacon;
        if (SV.Get<CmdMode>( SItem.cmGS_Lit_Nav ) == CmdMode.On) lightsInt |= (int)V_Lights.Lights.Nav;
        if (SV.Get<CmdMode>( SItem.cmGS_Lit_Strobe ) == CmdMode.On) lightsInt |= (int)V_Lights.Lights.Strobe;
        if (SV.Get<CmdMode>( SItem.cmGS_Lit_Taxi ) == CmdMode.On) {
          lightsInt |= (int)V_Lights.Lights.Taxi;
          _value1.ItemForeColor = cTxWarn;
        }
        if (SV.Get<CmdMode>( SItem.cmGS_Lit_Landing ) == CmdMode.On) {
          lightsInt |= (int)V_Lights.Lights.Landing;
          _value1.ItemForeColor = cTxWarn;
        }
        _value1.IntValue = lightsInt;
      }
    }

  }
}

