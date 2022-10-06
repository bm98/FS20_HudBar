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
using static FS20_HudBar.GUI.GUI_Fonts;

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
    public static readonly string Short = "ALT" + c_space + c_space; // need to have one all the time, else the height box changes which is moving the whole column
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP ALT / Set";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Ap_AltSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Altitude Hold\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_ALT; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_ALTset;
      _value1 = new V_Alt( value2Proto, m_alignWidth ) { ItemForeColor = cSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.AP_ALThold;
      _value2 = new V_Alt( value2Proto, m_alignWidth ) { ItemForeColor = cInfo, Visible = true }; // always shown
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.Cursor = Cursors.Hand;

      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Cursor = Cursors.SizeNS;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.AP_G1000Module.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      _value1.ActivateForm( e );

      if (e.Delta > 0) {
        // Up
        SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting( FSimClientIF.CmdMode.Inc );
      }
      else if (e.Delta < 0) {
        // Down
        SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting( FSimClientIF.CmdMode.Dec );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      //SC.SimConnectClient.Instance.AP_G1000Module.ALThold_active = true; // toggles independent of the set value
      SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold_current( FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _label.Text = SC.SimConnectClient.Instance.AP_G1000Module.ALThold_armed ? "ALTS" + c_space : "ALT" + c_space + c_space;
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALThold_active ? cAP : cLabel;

        _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;

        // second item
        if (SC.SimConnectClient.Instance.AP_G1000Module.ALT_managed) {
          // Managed Mode
          _value2.Managed = true;
          _value2.ItemForeColor = cInfo;
          _value2.Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_selSlot_ft;
        }
        else {
          // the Alt Ref currently holding (can get NaN)
          _value2.Managed = false;
          _value2.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALThold_active ? cAP
            : SC.SimConnectClient.Instance.AP_G1000Module.ALThold_armed ? cInfo : cLabel;
          _value2.Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_holding_ft;
        }

      }
    }

  }
}
