using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FSimClientIF;
using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Xpdr : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.XPDR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "XPDR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Transponder Code/Status";

    private readonly B_Base _label;
    private readonly V_Base _value1; // Code
    private readonly V_Base _value2; // Status

    public DI_Xpdr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.XPDR_CODE;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_ButtonClicked;

      _value1 = new V_ICAO( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.XPDR_STAT;
      _value2 = new V_Text( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 2, OnDataArrival ); // twice per sec
    }

    // Send Ident
    private void _label_ButtonClicked( object sender, GUI.ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set<bool>( SItem.bGS_Com_Transponder_IDENT, true );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        var stat = SV.Get<TransponderStatus>( SItem.tsGS_Com_Transponder_status );
        if (SV.Get<bool>( SItem.bG_Com_Transponder_available )) {
          _value1.Text = $"{SV.Get<int>( SItem.iGS_Com_Transponder_code ):0000}";
          _value1.ItemForeColor = SV.Get<bool>( SItem.bGS_Com_Transponder_IDENT ) ? cTxNav : cTxInfo;

          _value2.Text = $"{stat}";
          if (stat == TransponderStatus.ALT
            || stat == TransponderStatus.TA
            || stat == TransponderStatus.TA_RA
            ) {
            _value2.ItemForeColor = cTxNav;
          }
          else {
            _value2.ItemForeColor = cTxInfo;
          }

        }
        else {
          _value1.Text = null;
          _value2.Text = null;
          _value1.ItemForeColor = cTxInfo;
          _value2.ItemForeColor = cTxInfo;
        }
      }
    }

  }
}

