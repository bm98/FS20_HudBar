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
  class DI_Ap_AltSet : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_ALTs;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ALT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP ALT / Set";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Ap_AltSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      TText = "Altitude Hold\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_ALT; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_ALTset;
      _value1 = new V_Alt( value2Proto, showUnits ) { ItemForeColor = cSet };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.MouseWheel += _label_MouseWheel;
      _label.Cursor = Cursors.SizeNS;

      SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void _label_MouseWheel( object sender, MouseEventArgs e )
    {
      if ( e.Delta > 0 ) {
        // Up
        SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting( FSimClientIF.CmdMode.Inc );
      }
      else if ( e.Delta < 0 ) {
        // Down
        SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting( FSimClientIF.CmdMode.Dec );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold = true; // toggles independent of the set value
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold ? cAP : cLabel;
        _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;
      }
    }

  }
}
