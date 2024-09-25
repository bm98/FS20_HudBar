using System;
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

namespace FS20_HudBar.Bar.Items
{
  class DI_Nav2 : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV2_F;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-2 Stdby Active";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Nav2( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.NAV2_SWAP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Click += _label_Click;

      item = VItem.NAV2_STDBY;
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable=true;
      _value1.Cursor = Cursors.SizeNS;

      item = VItem.NAV2_ACTIVE;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxNav };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 5, OnDataArrival );
    }

    // Inc/Dec Standby Frequ
    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 2/3 - 1/3  dectection for Digits
      var whole = e.Location.X < (_value1.Width / 3 * 2);

      if (e.Delta > 0) {
        SV.Set( SItem.cmS_Nav_2_stdby_step, whole ? FSimClientIF.CmdMode.Inc : FSimClientIF.CmdMode.Inc_Fract );
      }
      else if (e.Delta < 0) {
        SV.Set( SItem.cmS_Nav_2_stdby_step, whole ? FSimClientIF.CmdMode.Dec : FSimClientIF.CmdMode.Dec_Fract );
      }
    }

    // Swap NAV Active - Standby
    private void _label_Click( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.cmS_Nav_2_stdby_step, FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (!SV.Get<bool>( SItem.bG_Nav_2_available )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          _value2.Text = "";
          return;
        }

        // Has NAV2
        _value1.Text = $"{SV.Get<int>( SItem.iGS_Nav_2_stdby_hz ) / 1_000_000f:000.00}";
        _value2.Text = $"{SV.Get<int>( SItem.iG_Nav_2_active_hz ) / 1_000_000f:000.00}";
      }
    }

  }
}

