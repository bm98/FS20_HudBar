using System;
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
  class DI_Lights : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.Lights;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "Lights";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Lights BNSTL";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Lights( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.Lights;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Lights( value2Proto );
      this.AddItem( _value1 );
      vCat.AddLbl( item, _value1 as Control );

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        // Consolidated lights (RA colored for Taxi and/or Landing lights on)
        int lightsInt = 0;
        _value1.ItemForeColor = cInfo;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Beacon ) lightsInt |= (int)V_Lights.Lights.Beacon;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Nav ) lightsInt |= (int)V_Lights.Lights.Nav;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Strobe ) lightsInt |= (int)V_Lights.Lights.Strobe;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Taxi ) {
          lightsInt |= (int)V_Lights.Lights.Taxi;
          _value1.ItemForeColor = cWarn;
        }
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Landing ) {
          lightsInt |= (int)V_Lights.Lights.Landing;
          _value1.ItemForeColor = cWarn;
        }
          _value1.IntValue = lightsInt;
      }
    }

  }
}

