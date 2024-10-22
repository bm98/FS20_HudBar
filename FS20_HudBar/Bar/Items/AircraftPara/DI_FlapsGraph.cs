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
  class DI_FlapsGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FLAPS_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Flaps";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Flaps % Graph";

    private readonly V_Base _label;
    private readonly A_Scale _scale1;

    public DI_FlapsGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.GraphX1;
      var item = VItem.FLAPS_ANI;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 100, AlertEnabled = false, ItemForeColor = cStep };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      AddObserver( Short, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _scale1.Value = SV.Get<float>( SItem.fG_Flp_Deployment_prct ); // 0..100
        _scale1.ItemForeColor = (SV.Get<float>( SItem.fG_Flp_Deployment_prct ) < 5) ? cOK : cStep;
      }
    }

  }
}
