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
using FSimClientIF;

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
      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    private void DI_Ap_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        SV.Set( SItem.cmGS_Ap_AP1, CmdMode.Toggle );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        this.ColorType.ItemForeColor = (SV.Get<CmdMode>( SItem.cmGS_Ap_AP1 ) == CmdMode.On) ? cTxAPActive : cTxLabel;
      }
    }

  }
}

