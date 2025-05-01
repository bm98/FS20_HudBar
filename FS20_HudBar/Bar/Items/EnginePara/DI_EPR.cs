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

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// Turbine EPR values
  /// </summary>
  internal class DI_EPR : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.EPR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "EPR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Turbine EPR";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;
    private readonly V_Base _value4;

    public DI_EPR( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Value2x2;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.E1_EPR;
      _value1 = new V_Ratio2( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.E2_EPR;
      _value2 = new V_Ratio2( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      // add 2 more values
      this.TwoRows = true;
      item = VItem.E3_EPR;
      _value3 = new V_Ratio2( value2Proto ) { Visible = false };
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      item = VItem.E4_EPR;
      _value4 = new V_Ratio2( value2Proto ) { Visible = false };
      this.AddItem( _value4 ); vCat.AddLbl( item, _value4 );

      this.IsEngineItem = true;
      AddObserver( Desc, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        int nEngines = SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num );
        var v1 = SV.Get<float>( SItem.fG_Eng_T1_EPR_ratio );
        var v2 = SV.Get<float>( SItem.fG_Eng_T2_EPR_ratio );
        var v3 = SV.Get<float>( SItem.fG_Eng_T3_EPR_ratio );
        var v4 = SV.Get<float>( SItem.fG_Eng_T4_EPR_ratio );
        _label.Text = (v1 + v2 + v3 + v4) > (nEngines * 2.5) ? "THR" : "EPR"; // cheat, if not an EPR ratio its Thrust % EPR is below 2.5 I assume..
        _value1.Value = v1;
        _value2.Value = v2;
        _value3.Value = v3;
        _value4.Value = v4;
      }
    }

  }
}
