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
  internal class DI_RRpmGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.R_RPM_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "R-RPM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Rotor RPM Graph";

    private readonly V_Base _label;
    private readonly A_TwinScale _scale1;

    public DI_RRpmGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.HROT_RPM_ANI;
      _scale1 = new A_TwinScale( ) { Minimum = 0, Maximum = 120, AlertValue = 110, ItemForeColor_Alert = cAlert, ItemForeColor = cOK, ItemForeColor_LScale = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      AddObserver( Short, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        int nEng = SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num );
        _scale1.Value = SV.Get<float>( SItem.fG_Eng_RotorMain_rpm_prct ); // 0..100
        _scale1.ValueLScale = SV.Get<float>( SItem.fG_Eng_RotorTail_rpm_prct ); // 0..100
        _scale1.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_RotorMain_rpm_prct ) > 109) ||
                                  (SV.Get<float>( SItem.fG_Eng_RotorTail_rpm_prct ) > 102)) ? cWarnBG : cBG;
        _scale1.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_RotorMain_rpm_prct ) < 85) ||
                                  (SV.Get<float>( SItem.fG_Eng_RotorTail_rpm_prct ) < 98)) ? cWarnBG : cBG;

      }
    }

  }
}



