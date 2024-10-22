using System;
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

namespace FS20_HudBar.Bar.Items
{
  class DI_Fuel_LR_Gal : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_LR_gal;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "F-LR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fuel Left/Right Gal";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Fuel_LR_Gal( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.FUEL_L_gal;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Gallons( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.FUEL_R_gal;
      _value2 = new V_Gallons( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_left_gal );
        _value2.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_right_gal );
        // Color when there is a substantial unbalance
        if (Calculator.HasFuelImbalance) {
          _value1.ItemForeColor = cTxWarn;
          _value2.ItemForeColor = cTxWarn;
        }
        else {
          _value1.ItemForeColor = cTxInfo;
          _value2.ItemForeColor = cTxInfo;
        }
      }
    }

  }
}
