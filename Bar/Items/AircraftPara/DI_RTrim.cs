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
  class DI_RTrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.RTrim;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "R-Trim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Rudder Trim";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_RTrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // Rudder
      // All ERA-Trim label get a button to activate the 0 Trim action
      var item = VItem.RTrim;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct( value2Proto );
      this.AddItem( _value1 );
      vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct = 0; // Set 0
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct;
      }
    }

  }
}

