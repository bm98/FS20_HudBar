﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items
{
  class DI_Com2 : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.COM2;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "COM 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "COM-2 Stdby Active";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Com2( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.COM2_SWAP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_Click;

      item = VItem.COM2_STDBY;
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      item = VItem.COM2_ACTIVE;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxNav, ItemBackColor = cValBG };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );
      _value2.Cursor = Cursors.Hand;
      _value2.MouseClick += _value2_MouseClick;
      AddObserver( Desc, 5, OnDataArrival );
    }

    // Inc/Dec Standby Frequ
    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 1/2 - 1/2  dectection for Digits
      var whole = e.Location.X < (_value1.Width / 2);

      if (e.Delta > 0) {
        SV.Set( SItem.cmS_Com_2_step, whole ? FSimClientIF.CmdMode.Inc : FSimClientIF.CmdMode.Inc_Fract );
      }
      else if (e.Delta < 0) {
        SV.Set( SItem.cmS_Com_2_step, whole ? FSimClientIF.CmdMode.Dec : FSimClientIF.CmdMode.Dec_Fract );
      }
    }

    // Swap COM Active - Standby
    private void _label_Click( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (SV.Get<bool>( SItem.bG_Com_2_available )) {
        SV.Set( SItem.cmS_Com_2_step, FSimClientIF.CmdMode.Toggle );
      }
    }

    // Activate Frequ
    private void _value2_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (SV.Get<bool>( SItem.bG_Com_2_available )) {
        SV.Set( SItem.bGS_Com_2_Tx_active, true );
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SV.Get<bool>( SItem.bG_Com_2_available )) {
          _value1.Text = $"{SV.Get<int>( SItem.iGS_Com_2_stdby_hz ) / 1_000_000f:000.000}";
          _value1.ItemForeColor = cTxInfo;
          _value2.Text = $"{SV.Get<int>( SItem.iG_Com_2_active_hz ) / 1_000_000f:000.000}";
          _value2.ItemForeColor = SV.Get<bool>( SItem.bGS_Com_2_Tx_active ) ? cTxNav : cTxInfo;
        }
        else {
          _value1.Text = "n.a.   ";
          _value1.ItemForeColor = cTxDim;
          _value2.Text = "       ";
          _value2.ItemForeColor = cTxDim;
        }
      }
    }

  }
}

