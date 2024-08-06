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
using static FS20_HudBar.GUI.GUI_Fonts;
using static FSimClientIF.Sim;
using FSimClientIF;

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
      _value1 = new V_Alt( value2Proto, m_alignWidth ) { ItemForeColor = cTxSet, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.AP_ALThold;
      _value2 = new V_Alt( value2Proto, m_alignWidth ) { ItemForeColor = cTxInfo, Visible = true }; // always shown
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.Cursor = Cursors.Hand;

      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      AddObserver( Short, (int)(DataArrival_perSecond / 10), OnDataArrival);
    }

    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 1/2 - 1/2  dectection for Digits
      var largeChange = e.Location.X < (_value1.Width / 2);

      if (e.Delta > 0) {
        // Up
        if (largeChange) {
          SV.Set<CmdMode>( SItem.cmS_Ap_ALT_setting_step1000ft, CmdMode.Inc );
        }
        else {
          SV.Set<CmdMode>( SItem.cmS_Ap_ALT_setting_step, CmdMode.Inc );
        }
      }
      else if (e.Delta < 0) {
        // Down
        if (largeChange) {
          SV.Set<CmdMode>( SItem.cmS_Ap_ALT_setting_step1000ft, CmdMode.Dec );
        }
        else {
          SV.Set<CmdMode>( SItem.cmS_Ap_ALT_setting_step, CmdMode.Dec );
        }
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      //SV.ALThold_active = true; // toggles independent of the set value
      SV.Set<CmdMode>( SItem.cmS_Ap_ALT_hold_current, CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _label.Text = SV.Get<bool>( SItem.bGS_Ap_ALT_hold )
          ? $"ALT{c_space}{c_space}"
          : SV.Get<bool>( SItem.bG_Ap_ALT_armed )
              ? (SV.Get<bool>( SItem.bG_Ap_VNAV_useTargetAlt ) ? $"ALTV{c_space}" : $"ALTS{c_space}")
              : $"ALT{c_space}{c_space}";
        this.ColorType.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_ALT_hold ) ? cTxAPActive : cTxLabel;

        _value1.Value = SV.Get<float>( SItem.fGS_Ap_ALT_setting_ft );

        // second item
        if (SV.Get<bool>( SItem.bG_Ap_ALT_managed )) {
          // Managed Mode
          _value2.Managed = true;
          _value2.ItemForeColor = cTxInfo;
          _value2.Value = SV.Get<float>( SItem.fGS_Ap_ALT_setting_ft );
        }
        else {
          // the Alt Ref currently holding (can get NaN)
          _value2.Managed = false;
          _value2.ItemForeColor = SV.Get<bool>( SItem.bGS_Ap_ALT_hold )
            ? cTxAPActive
            : SV.Get<bool>( SItem.bG_Ap_ALT_armed )
                ? cTxInfo
                : cTxDim;
          _value2.Value = SV.Get<float>( SItem.fG_Ap_ALT_holding_ft );
        }

      }
    }

  }
}
