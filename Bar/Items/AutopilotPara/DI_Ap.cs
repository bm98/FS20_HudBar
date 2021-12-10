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
  class DI_Ap : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "≡AP≡";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Autopilot Master";

    private readonly B_Base _label;

    public DI_Ap( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Autopilot Master\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP;
      _label = new B_Text( item, value2Proto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Ap_ButtonClicked;
      SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void DI_Ap_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.Master_toggle( );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.AP_mode == FSimClientIF.APMode.On ? cAP : cLabel;
      }
    }

  }
}

