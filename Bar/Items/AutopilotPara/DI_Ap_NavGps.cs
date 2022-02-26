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
  class DI_Ap_NavGps : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_NAVg;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP NAV and Source";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly int _obs2;

    public DI_Ap_NavGps( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "NAV Hold\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_NAV; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_NAVgps;
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
      _obs2 = SC.SimConnectClient.Instance.NavModule.AddObserver( Short, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AP_G1000Module.NAVhold_active = true; // toggles independent of the set value
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.NAVhold_active ? cAP : cLabel;
        _value1.Text = SC.SimConnectClient.Instance.AP_G1000Module.IsGPS_active ? "GPS" :
            ( SC.SimConnectClient.Instance.NavModule.NavSource_current == FSimClientIF.NavSource.NAV1 ? "NAV1" : "NAV2" );
        _value1.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.IsGPS_active ? cGps : cNav;
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.AP_G1000Module.RemoveObserver( m_observerID );
      SC.SimConnectClient.Instance.NavModule.RemoveObserver( _obs2 );
    }

  }
}
