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
  internal class DI_AccelGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ACCEL_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ACCEL";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Accelerations";

    private readonly V_Base _label;
    private readonly A_Accel _scale1;

    public DI_AccelGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.GraphX1;
      var item = VItem.ACCEL_ANI;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      // min/max: 1 g ~ 32 ft/sec2 - we use +- 1/4 g for now..
      _scale1 = new A_Accel( ) { MinimumVer = -16, MaximumVer = 16, MinimumLon = -1.689f, MaximumLon = 1.689f, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      AddObserver( Short, 5, OnDataArrival);
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _scale1.VerticalAccel = SV.Get<float>( SItem.fG_Acft_Accel_worldY_fps2 );
        _scale1.LongitudinalAccel = SV.Get<float>( SItem.fG_Acft_Accel_acftZ_fps2 );
      }
    }

  }
}
