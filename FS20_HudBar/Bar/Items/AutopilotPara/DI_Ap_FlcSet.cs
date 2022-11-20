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
  class DI_Ap_FlcSet : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_FLCs;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "FLC";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP FLC / Set";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2; // dummy for alignment only

    public DI_Ap_FlcSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Flight Level Change\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_FLC; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_FLCset;
      _value1 = new V_Speed( value2Proto, m_alignWidth ) { ItemForeColor = cTxSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.AP_FLCset_man;
      _value2 = new V_Speed( value2Proto, m_alignWidth ) { ItemForeColor = cTxInfo, Visible = false };
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
      UnregisterObserver_low( SC.SimConnectClient.Instance.AP_G1000Module ); // use the generic one
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // activate the form if the HudBar is not active so at least the most scroll goes only to the HudBar
      _value1.ActivateForm( e );

      // 1/2 - 1/2  dectection for Digits
      var largeChange = e.Location.X < (_value1.Width / 2);

      if (e.Delta > 0) {
        // Up
        if (largeChange) {
          int value = (int)SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt + 10;
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt = value;
        }
        else {
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting( FSimClientIF.CmdMode.Inc );
        }
      }
      else if (e.Delta < 0) {
        // Down
        if (largeChange) {
          int value = (int)SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt - 10;
          value = (value < 0) ? 0 : value; // cannot supply neg values
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt = value;
        }
        else {
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting( FSimClientIF.CmdMode.Dec );
        }
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      //      SC.SimConnectClient.Instance.AP_G1000Module.FLChold_active = true; // toggles independent of the set value
      if (!SC.SimConnectClient.Instance.AP_G1000Module.FLChold_active) {
        SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt = SC.SimConnectClient.Instance.HudBarModule.IAS_kt;
      }
      SC.SimConnectClient.Instance.AP_G1000Module.FLC_hold( FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.FLChold_active ? cTxAPActive : cTxLabel;
        _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt;

        // Managed Mode NOT USED SO FAR
        /*
        _value2.Managed = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
        _value2.Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_managed_kt;
        _value2.Visible = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
        */
      }
    }

  }
}
