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
  class DI_N1Graph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.N1_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "N1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Turbine N1 Graph %";

    private readonly V_Base _label;
    private readonly A_Scale _scale1;
    private readonly A_TwinScale _scale2;
    private readonly A_Scale _scale3;
    private readonly A_TwinScale _scale4;

    public DI_N1Graph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.GraphX2;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.E1_N1_ANI;
      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      item = VItem.E2_N1_ANI;
      _scale2 = new A_TwinScale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK, ItemForeColor_LScale = cOK };
      this.AddItem( _scale2 ); vCat.AddLbl( item, _scale2 );

      // add 2 more scales
      item = VItem.E3_N1_ANI;
      _scale3 = new A_Scale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK };
      this.AddItem( _scale3 ); vCat.AddLbl( item, _scale3 );

      item = VItem.E4_N1_ANI;
      _scale4 = new A_TwinScale( ) { Visible = false, Minimum = 0, Maximum = 110, AlertValue = 101, ItemForeColor_Alert = cAlert, ItemForeColor = cOK, ItemForeColor_LScale = cOK };
      this.AddItem( _scale4 ); vCat.AddLbl( item, _scale4 );

      AddObserver( Desc, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        int nEng = SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num );
        _scale1.Visible = (nEng == 1); // Single
        _scale2.Visible = (nEng > 1);  // Twin Left
        _scale3.Visible = (nEng == 3); // Twin Left + Single Right
        _scale4.Visible = (nEng > 3);  // Twin Left + Twin Right

        if (_scale1.Visible) {
          _scale1.Value = SV.Get<float>( SItem.fG_Eng_T1_N1_prct ); // 0..100
          _scale1.ItemBackColor = (SV.Get<float>( SItem.fG_Eng_T1_N1_prct ) > 102) ? cWarnBG : cBG;
        }

        if (_scale2.Visible && !(_scale3.Visible || _scale4.Visible)) {
          _scale2.Value = SV.Get<float>( SItem.fG_Eng_T1_N1_prct ); // 0..100
          _scale2.ValueLScale = SV.Get<float>( SItem.fG_Eng_T2_N1_prct ); // 0..100
          _scale2.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_T1_N1_prct ) > 102) ||
                                    (SV.Get<float>( SItem.fG_Eng_T2_N1_prct ) > 102)) ? cWarnBG : cBG;
        }

        if (_scale3.Visible) {
          // reorder for 3 Engines
          _scale2.Value = SV.Get<float>( SItem.fG_Eng_T1_N1_prct ); // 0..100
          _scale2.ValueLScale = SV.Get<float>( SItem.fG_Eng_T3_N1_prct ); // 0..100
          _scale2.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_T1_N1_prct ) > 102) ||
                                    (SV.Get<float>( SItem.fG_Eng_T3_N1_prct ) > 102)) ? cWarnBG : cBG;

          _scale3.Value = SV.Get<float>( SItem.fG_Eng_T2_N1_prct ); // 0..100
          _scale3.ItemBackColor = (SV.Get<float>( SItem.fG_Eng_T2_N1_prct ) > 102) ? cWarnBG : cBG;
        }

        if (_scale4.Visible) {
          // reorder for 4 Engines
          _scale2.Value = SV.Get<float>( SItem.fG_Eng_T1_N1_prct ); // 0..100
          _scale2.ValueLScale = SV.Get<float>( SItem.fG_Eng_T3_N1_prct ); // 0..100
          _scale2.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_T1_N1_prct ) > 102) ||
                                    (SV.Get<float>( SItem.fG_Eng_T3_N1_prct ) > 102)) ? cWarnBG : cBG;

          _scale4.Value = SV.Get<float>( SItem.fG_Eng_T2_N1_prct ); // 0..100
          _scale4.ValueLScale = SV.Get<float>( SItem.fG_Eng_T4_N1_prct ); // 0..100
          _scale4.ItemBackColor = ((SV.Get<float>( SItem.fG_Eng_T2_N1_prct ) > 102) ||
                                    (SV.Get<float>( SItem.fG_Eng_T4_N1_prct ) > 102)) ? cWarnBG : cBG;
        }
      }
    }

  }
}
