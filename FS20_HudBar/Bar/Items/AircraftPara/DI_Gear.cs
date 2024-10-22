using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using FSimClientIF;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Gear : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GEAR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Gear";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Gear";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Gear( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Symbol;
      var item = VItem.GEAR;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Gear_Retractable )) {
          _value1.Step =
              (SV.Get<GearPosition>( SItem.gpGS_Gear_Position ) == GearPosition.Down)
              ? Steps.DownOK
              : ((SV.Get<GearPosition>( SItem.gpGS_Gear_Position ) == GearPosition.Up)
                  ? Steps.UpOK
                  : Steps.Unk);
        }
        else {
          _value1.Step = Steps.DownOK;
        }
      }
    }

  }
}

