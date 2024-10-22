using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_Fuel_C_Kg : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_C_kg;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "F-C";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fuel Center kg";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Fuel_C_Kg( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.FUEL_C_kg;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Kilograms( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = (float)UnitConv.Lbs_to_Kg( SV.Get<float>( SItem.fG_Fuel_Quantity_center_lb ) );
      }
    }

  }
}

