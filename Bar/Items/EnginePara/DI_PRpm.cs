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
  class DI_PRpm : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.PRPM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "P-RPM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Propeller RPM";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_PRpm( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.P1_RPM;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_RPM( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.P2_RPM;
      _value2 = new V_RPM( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.EngineModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.EngineModule.Propeller1_rpm;
        _value2.Value = SC.SimConnectClient.Instance.EngineModule.Propeller2_rpm;
        _value2.Visible = ( SC.SimConnectClient.Instance.EngineModule.NumEngines > 1 );
      }
    }

  }
}