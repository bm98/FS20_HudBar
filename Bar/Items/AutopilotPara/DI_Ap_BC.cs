using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ap_BC : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_BC;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "BC";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP BC";

    private readonly B_Base _label;

    public DI_Ap_BC( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Back Course Navigation\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_BC; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      _label.ButtonClicked += _label_ButtonClicked;
      SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.BChold_active = true; // toggles independent of the set value
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.BChold_active ? cAP : cLabel;
      }
    }

  }
}
