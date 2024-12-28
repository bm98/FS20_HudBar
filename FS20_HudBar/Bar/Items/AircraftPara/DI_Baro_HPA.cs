using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Baro_HPA : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.BARO_HPA;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "BARO";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Baro Setting hPa";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Baro_HPA( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Barometer reading\nClick to set to adjust to Sim";

      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.BARO_HPA;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_PressureHPA( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      AddObserver( Desc, 2, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        SV.Set( SItem.bS_Acft_Altimeter_set_current, true );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Acft_Altimeter1_setting_hpa );
      }
    }

  }
}

