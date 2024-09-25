using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Fps : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FPS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "FPS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Frames per second";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Fps( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.FPS;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Fps( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 2, OnDataArrival ); // twice per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Sim_FrameRate_fps );
      }
    }

  }
}
