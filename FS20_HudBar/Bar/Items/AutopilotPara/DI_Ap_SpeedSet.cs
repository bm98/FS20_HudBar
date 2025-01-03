﻿using System;
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

    public DI_Ap_SpeedSet( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "IAS Hold\nClick to toggle";

      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.AP_SPD; // Button Handler
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      item = VItem.AP_SPDset;
      _value1 = new V_Speed( value2Proto, m_alignWidth ) { ItemForeColor = cTxSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.AP_SPDset_man;
      _value2 = new V_Speed( value2Proto, m_alignWidth ) { ItemForeColor = cTxInfo, Visible = false };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.Cursor = Cursors.Hand;

      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      AddObserver( Desc, 10, OnDataArrival );
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 1/2 - 1/2  dectection for Digits
      var largeChange = e.Location.X < (_value1.Width / 2);

      if (e.Delta > 0) {
        // Up
        if (SV.Get<bool>( SItem.bGS_Ap_MACH_dial )) {
          SV.Set<CmdMode>( SItem.cmS_Ap_SPD_setting_step, CmdMode.Inc );
        }
        else {
          if (largeChange) {
            float value = SV.Get<float>( SItem.fGS_Ap_IAS_setting_kt ) + 10;
            SV.Set<float>( SItem.fGS_Ap_IAS_setting_kt, value );
          }
          else {
            SV.Set<CmdMode>( SItem.cmS_Ap_SPD_setting_step, CmdMode.Inc );
          }
        }
      }
      else if (e.Delta < 0) {
        // Down
        if (SV.Get<bool>( SItem.bGS_Ap_MACH_dial )) {
          SV.Set<CmdMode>( SItem.cmS_Ap_SPD_setting_step, CmdMode.Dec );
        }
        else {
          if (largeChange) {
            float value = SV.Get<float>( SItem.fGS_Ap_IAS_setting_kt ) - 10;
            value = (value < 0) ? 0 : value; // cannot supply neg values
            SV.Set<float>( SItem.fGS_Ap_IAS_setting_kt, value );
          }
          else {
            SV.Set<CmdMode>( SItem.cmS_Ap_SPD_setting_step, CmdMode.Dec );
          }
        }
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      //      SV.SPDhold_active = true; // toggles independent of the set value
      SV.Set( SItem.cmS_Ap_SPD_hold_current, CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // set MACH mode
        (_value1 as V_Speed).MachMode = SV.Get<bool>( SItem.bGS_Ap_MACH_dial );
        // Set Value and color
        if (SV.Get<bool>( SItem.bGS_Ap_MACH_dial )) {
          Label.Text = Short + "m";
          this.ColorType.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_SPD_active ) ? cTxAPActive : cTxLabel;
          _value1.Value = SV.Get<float>( SItem.fG_Ap_MACH_setting_mach );

          // Managed Mode
          _value2.Managed = SV.Get<bool>( SItem.bG_Ap_SPD_managed );
          _value2.Value = SV.Get<float>( SItem.fG_Ap_MACH_setting_mach ); // TODO get Managed MACH here
          _value2.Visible = SV.Get<bool>( SItem.bG_Ap_SPD_managed );
        }
        else {
          // IAS managed
          Label.Text = Short;
          this.ColorType.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_SPD_active ) ? cTxAPActive : cTxLabel;
          _value1.Value = SV.Get<float>( SItem.fGS_Ap_IAS_setting_kt );

          // Managed Mode
          _value2.Managed = SV.Get<bool>( SItem.bG_Ap_SPD_managed );
          _value2.Value = SV.Get<float>( SItem.fGS_Ap_IAS_setting_kt );
          _value2.Visible = SV.Get<bool>( SItem.bG_Ap_SPD_managed );
        }
      }
    }

  }
}
