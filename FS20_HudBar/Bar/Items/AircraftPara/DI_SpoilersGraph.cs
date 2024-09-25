using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_SpoilersGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SPOILER_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Sp-B";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Spoiler/SBrake Graph";

    private readonly B_Base _label;
    private readonly A_Scale _scale1;

    public DI_SpoilersGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.SPOILER_ANI;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_ButtonClicked;

      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 100, AlertEnabled = false, ItemForeColor = cStep };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      AddObserver( Short, 5, OnDataArrival );
    }


    private void _label_ButtonClicked( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (!SV.Get<bool>( SItem.bG_Flp_HasSpoilers )) return;

      SV.Set( SItem.cmS_Flp_Spoilers_arm, FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Flp_HasSpoilers )) {
          _label.ItemForeColor = SV.Get<bool>( SItem.bG_Flp_Spoilers_armed ) ? cTxLabelArmed : cTxLabel;

          _scale1.Value = SV.Get<float>( SItem.fGS_Flp_SpoilerHandle_position_prct ) * 100; // 0..100
          _scale1.ItemForeColor = (SV.Get<float>( SItem.fGS_Flp_SpoilerHandle_position_prct ) < 0.05) ? cOK : cStep;
        }
        else {
          _scale1.Value = null;
        }
      }
    }

  }
}
