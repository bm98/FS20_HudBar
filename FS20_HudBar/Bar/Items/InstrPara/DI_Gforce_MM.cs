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
  class DI_Gforce_MM : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GFORCE_MM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "G-MM";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "G Force Min, Max";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Gforce_MM( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "G Force min/max\nClick to reset";

      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.GFORCE_Min;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Gforce_MM_ButtonClicked;
      _value1 = new V_GForce( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.GFORCE_Max;
      _value2 = new V_GForce( value2Proto ) { ItemForeColor = cTxDim };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Desc, 2, OnDataArrival );
    }

    private void DI_Gforce_MM_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.bS_Acft_GForce_reset, true );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Acft_GForce_min_g );
        _value2.Value = SV.Get<float>( SItem.fG_Acft_GForce_max_g );
      }
    }

  }
}

