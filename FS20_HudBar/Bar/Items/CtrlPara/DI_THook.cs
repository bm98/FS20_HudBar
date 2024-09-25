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
  internal class DI_THook : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.THOOK;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "THook";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Tailhook /Arrester";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_THook( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.THOOK;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 2, OnDataArrival ); // twice per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        bool handleUp = !SV.Get<bool>( SItem.bG_Thr_THook_Handle_engaged );
        float extPrct = SV.Get<float>( SItem.fG_Thr_THook_Handle_position_prct100 ); //0..1 - on ground at about 0.89 extendend

        _value1.Step =
          handleUp ? Steps.UpInfo // white up
          : (extPrct < 0.85f) ? Steps.PEnd : Steps.DownOK; // down arrow remains blue until >85% then gets green
      }
    }

  }
}

