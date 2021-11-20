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
  class DI_Fuel_LR_Gal : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.Fuel_LR_gal;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "F-LR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Fuel Left/Right Gal";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Fuel_LR_Gal( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.Fuel_Left_gal;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Gallons( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.Fuel_Right_gal;
      _value2 = new V_Gallons( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityLeft_gal;
        _value2.Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityRight_gal;
        // Color when there is a substantial unbalance
        if ( Calculator.HasFuelImbalance ) {
          _value1.ItemForeColor = cWarn;
          _value2.ItemForeColor = cWarn;
        }
        else {
          _value1.ItemForeColor = cInfo;
          _value2.ItemForeColor = cInfo;
        }
      }
    }

  }
}