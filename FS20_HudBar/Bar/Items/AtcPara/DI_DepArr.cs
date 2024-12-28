using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_DepArr : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.DEPARR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RTE";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Departure / Arrival";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    /// <summary>
    /// cTor:
    /// </summary>
    public DI_DepArr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.DEPARR_DEP;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.Cursor = Cursors.Hand;
      _label.MouseClick += _label_MouseClick;

      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.DEPARR_ARR;
      _value2 = new V_ICAO( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Desc, 0.5f, OnDataArrival );
    }

    // User Enty of Airport ICAOs
    private void _label_MouseClick( object sender, MouseEventArgs e )
    {
      // if (!SC.SimConnectClient.Instance.IsConnected) return; // need to set it even if the Sim is not connected

      var TTX = new Config.frmApt( );
      if (TTX.ShowDialog( this ) == DialogResult.OK) {
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Text = AirportMgr.IsDepAvailable ? AirportMgr.DepAirportICAO : "...."; // default text
        _value2.Text = AirportMgr.IsArrAvailable ? AirportMgr.ArrAirportICAO : "...."; // default text
      }
    }

  }
}

