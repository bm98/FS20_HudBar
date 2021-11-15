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
  class DI_Gear : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.Gear;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "Gear";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Gear";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gear( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.Gear;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.IsGearRetractable ) {
          _value1.Step =
              SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Down ? Steps.Down :
              SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Up ? Steps.Up : Steps.Unk;
        }
        else {
          _value1.Step = Steps.Down;
        }
      }
    }

  }
}

