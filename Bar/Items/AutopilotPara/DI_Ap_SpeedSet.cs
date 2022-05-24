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
  class DI_Ap_SpeedSet : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.AP_SPDs;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "SPD";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "AP SPD / Set";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Ap_SpeedSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      TText = "IAS Hold\nClick to toggle";

      LabelID = LItem;
      var item = VItem.AP_SPD; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_SPDset;
      _value1 = new V_Speed( value2Proto, showUnits, m_alignWidth ) { ItemForeColor = cSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.AP_SPDset_man;
      _value2 = new V_Speed( value2Proto, showUnits, m_alignWidth ) { ItemForeColor = cInfo, Visible = false };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.Cursor = Cursors.Hand;

      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Cursor = Cursors.SizeNS;

      m_observerID = SC.SimConnectClient.Instance.AP_G1000Module.AddObserver( Short, OnDataArrival );
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (e.Delta > 0) {
        // Up
        if (SC.SimConnectClient.Instance.AP_G1000Module.MACH_managed) {
          SC.SimConnectClient.Instance.AP_G1000Module.MACH_setting( FSimClientIF.CmdMode.Inc );
        }
        else {
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting( FSimClientIF.CmdMode.Inc );
        }
      }
      else if (e.Delta < 0) {
        // Down
        if (SC.SimConnectClient.Instance.AP_G1000Module.MACH_managed) {
          SC.SimConnectClient.Instance.AP_G1000Module.MACH_setting( FSimClientIF.CmdMode.Dec );
        }
        else {
          SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting( FSimClientIF.CmdMode.Dec );
        }
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      //      SC.SimConnectClient.Instance.AP_G1000Module.SPDhold_active = true; // toggles independent of the set value
      SC.SimConnectClient.Instance.AP_G1000Module.SPD_hold_current( FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // set MACH mode
        (_value1 as V_Speed).MachMode = SC.SimConnectClient.Instance.AP_G1000Module.MACH_managed;
        // Set Value and color
        if (SC.SimConnectClient.Instance.AP_G1000Module.MACH_managed) {
          Label.Text = Short + "m";
          this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.SPDhold_active ? cAP : cLabel;
          _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.MACH_setting_mach;

          // Managed Mode
          _value2.Managed = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
          _value2.Value = SC.SimConnectClient.Instance.AP_G1000Module.MACH_setting_mach; // TODO get Managed MACH here
          _value2.Visible = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
        }
        else {
          // IAS managed
          Label.Text = Short;
          this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.SPDhold_active ? cAP : cLabel;
          _value1.Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt;

          // Managed Mode
          _value2.Managed = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
          _value2.Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_managed_kt;
          _value2.Visible = SC.SimConnectClient.Instance.AP_G1000Module.SPD_managed;
        }
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.AP_G1000Module.RemoveObserver( m_observerID );
    }

  }
}
