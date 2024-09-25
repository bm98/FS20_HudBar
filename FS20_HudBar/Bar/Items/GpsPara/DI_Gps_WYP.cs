using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Gps_WYP : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_WYP;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "≡GPS≡";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "GPS Waypoints";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Gps_WYP( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GPS_PWYP;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO( value2Proto ) { ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.GPS_NWYP;
      _value2 = new V_ICAO_L( value2Proto ) { ItemForeColor = cTxGps };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 1, OnDataArrival ); // once per sec is enough
    }

    // format an empty wyp as null -> ____ readout
    private string WypLabel( string wypName ) => string.IsNullOrWhiteSpace( wypName ) ? null : wypName;


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // using the WPtrackers content
        string pWyp = WPTracker.PrevWP; // SV.Get<string>( SItem.sG_Gps_WYP_prevID );
        string nWyp = WPTracker.NextWP; // SV.Get<string>( SItem.sG_Gps_WYP_nextID );

        _value1.Text = SV.Get<bool>( SItem.bG_Gps_DirectTo_tracking ) ? "Ð→" : WypLabel( pWyp );
        _value2.Text = WypLabel( nWyp );
      }
    }

  }
}
