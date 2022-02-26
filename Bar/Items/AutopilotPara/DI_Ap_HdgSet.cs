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
using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.GUI.Templates;

namespace FS20_HudBar.Bar.Items
{
  class DI_Ap_HdgSet : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_HDGs;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "HDG";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP HDG / Set";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Ap_HdgSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      TText = "Heading Hold\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_HDG; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_HDGset;
      _value1 = new V_Deg( value2Proto, showUnits ) { ItemForeColor = cSet };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.MouseWheel += _label_MouseWheel;
      _label.Cursor = Cursors.SizeNS;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void _label_MouseWheel( object sender, MouseEventArgs e )
    {
      if ( e.Delta > 0 ) {
        // Up
        SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting( FSimClientIF.CmdMode.Inc );
      }
      else if ( e.Delta < 0 ) {
        // Down
        SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting( FSimClientIF.CmdMode.Dec );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.HDGhold_active = true; // toggles independent of the set value
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.HDGhold_active ? cAP : cLabel;
        _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting_degm;
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.AP_G1000Module.RemoveObserver( m_observerID );
    }

  }
}
