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
  class DI_Fuel_LR_Lb : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_LR_lb;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "F-LR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Fuel Left/Right Lb";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Fuel_LR_Lb( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.FUEL_L_lb;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Pounds( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.FUEL_R_lb;
      _value2 = new V_Pounds( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.FuelQuantityLeft_lb;
        _value2.Value = SC.SimConnectClient.Instance.HudBarModule.FuelQuantityRight_lb;
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
