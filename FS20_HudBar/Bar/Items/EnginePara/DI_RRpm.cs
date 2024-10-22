using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_RRpm : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.R_RPM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "R-RPM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Rotor RPM";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_RRpm( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.HMAIN_RPM;
      _value1 = new V_RPM( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.HTAIL_RPM;
      _value2 = new V_RPM( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Eng_RotorMain_rpm );
        _value2.Value = SV.Get<float>( SItem.fG_Eng_RotorTail_rpm );
      }
    }

  }
}
