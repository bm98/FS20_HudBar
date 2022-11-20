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
  class DI_Ap_AprGs : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_APR_GS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "APR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP APR and GS";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Ap_AprGs( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Approach Mode\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_APR; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_GS;
      _value1 = new V_Text( value2Proto ) { ItemForeColor= cTxDim, Text = "►GS◄" };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.AP_G1000Module ); // use the generic one
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.APRhold_active = true; // toggles independent of the set value
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.APRhold_active ? cTxAPActive : cTxLabel;
        _value1.Text = SC.SimConnectClient.Instance.AP_G1000Module.GPS_active ? "►GP◄" : "►GS◄";
        _value1.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GS_active ? cTxAPActive :
                                      ( SC.SimConnectClient.Instance.AP_G1000Module.GShold_active ? cTxInfo : cTxDim);
        
      }
    }

  }
}
