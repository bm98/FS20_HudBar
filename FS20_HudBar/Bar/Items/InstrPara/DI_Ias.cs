using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ias : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.IAS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "IAS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft IAS kt";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Ias( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.IAS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Speed( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 5, OnDataArrival );
    }


    // Retuns a ColorType for the IAS based on DesignSpeeds
    private ColorType IAScolor( )
    {
      if (SV.Get<bool>( SItem.bG_Sim_OnGround )) return cTxInfo;

      if (SV.Get<bool>( SItem.bG_Warn_Overspeed_warn )) return cTxAlert;
      if (SV.Get<bool>( SItem.bG_Warn_Stall_warn )) return cTxAlert;

      var flapsSpeed = (SV.Get<float>( SItem.fG_Flp_Deployment_prct ) > 0.8)
                        ? SV.Get<float>( SItem.fG_Dsg_SpeedVS0_kt )
                        : SV.Get<float>( SItem.fG_Dsg_SpeedVS1_kt );
      if (SV.Get<float>( SItem.fG_Acft_IAS_kt ) <= flapsSpeed) return cTxAlert;
      if (SV.Get<float>( SItem.fG_Acft_IAS_kt ) <= (flapsSpeed + 5)) return cTxWarn; // within 5kts of Flaps speed

      return cInfo;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Acft_IAS_kt );
        _value1.ItemForeColor = IAScolor( );
      }
    }

  }
}
