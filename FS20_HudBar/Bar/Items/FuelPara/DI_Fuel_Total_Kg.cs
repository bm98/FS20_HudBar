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
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_Fuel_Total_Kg : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FUEL_TOT_kg;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "F-TOT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fuel Total kg";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Fuel_Total_Kg( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.FUEL_TOT_kg;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Kilograms( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.FUEL_REACH_kg;
      _value2 = new V_TimeHHMM( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, (int)(DataArrival_perSecond / 1), OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Fuel Tot & Reach
        _value1.Value = (float)UnitConv.Lbs_to_Kg( SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) );
        _value2.Value = Calculator.FuelReach_sec( );
        _value2.ItemForeColor = Calculator.FuelReachAlert ? cTxAlert : (Calculator.FuelReachWarn ? cTxWarn : cTxInfo);
      }
    }

  }
}


