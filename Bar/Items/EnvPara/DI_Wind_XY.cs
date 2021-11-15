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
  class DI_Wind_XY : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.WIND_XY;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "WIND";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Wind cross / head kt";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Wind_XY( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      // Wind XY
      var item = VItem.WIND_LAT;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Wind_X( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.WIND_LON;
      _value2 = new V_Wind_HT( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftX_kt;
        _value2.Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftZ_kt;
      }
    }

  }
}