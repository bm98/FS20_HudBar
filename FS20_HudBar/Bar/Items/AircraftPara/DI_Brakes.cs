using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Brakes : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.BRAKES;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Brakes";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Brakes";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Brakes( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.BRAKES;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto ) { ItemForeColor = cTxWarn };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, (int)(DataArrival_perSecond / 2), OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Step = SV.Get<bool>( SItem.bGS_Gear_Parkbrake_on ) ? Steps.OnWarn : Steps.OffOK;
      }
    }

  }
}

