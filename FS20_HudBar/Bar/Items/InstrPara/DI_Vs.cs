using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using dNetBm98;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Vs : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "VS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft VS fpm (arrows)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Vs( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.VS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_VSpeedPerMin( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = XMath.AsRoundInt( SV.Get<float>( SItem.fG_Acft_VS_ftPmin ), 20 ); // steps 20
      }
    }

  }
}
