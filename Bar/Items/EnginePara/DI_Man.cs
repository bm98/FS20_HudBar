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
  class DI_Man : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.MAN;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "MAN";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "MAN Pressure inHg";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Man( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      // Wind Direction, Speed
      var item = VItem.E1_MAN;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_PressureInHg( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.E2_MAN;
      _value2 = new V_PressureInHg( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
          _value1.Value = SC.SimConnectClient.Instance.HudBarModule.Engine1MAN_inhg;
          _value2.Value = SC.SimConnectClient.Instance.HudBarModule.Engine2MAN_inhg;
          _value2.Visible = ( SC.SimConnectClient.Instance.HudBarModule.NumEngines > 1 );
      }
    }

  }
}
