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
  class DI_Fuel_LR_Lb : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_LR_lb;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "F-LR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fuel Left/Right Lb";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Fuel_LR_Lb( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.FUEL_L_lb;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Pounds( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.FUEL_R_lb;
      _value2 = new V_Pounds( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SV.AddObserver( Short, 5, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_left_lb );
        _value2.Value = SV.Get<float>( SItem.fG_Fuel_Quantity_right_lb );
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
