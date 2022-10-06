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
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Adf1_Active : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ADF1;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ADF 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ADF-1 Id Bearing/Needle";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly A_WindArrow _brg;

    public DI_Adf1_Active( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.ADF1_ID;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( valueProto ) { ItemForeColor = cInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ADF1_BRG;
      _value2 = new V_Deg( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ADF1_ANI;
      _brg = new A_WindArrow( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true, ItemForeColor = cScale3 };
      this.AddItem( _brg ); vCat.AddLbl( item, _brg );

      m_observerID = SC.SimConnectClient.Instance.NavModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.NavModule.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SC.SimConnectClient.Instance.NavModule.Adf1_Ident != "") {
          _value1.ItemForeColor = cNav;
          _value1.Text = SC.SimConnectClient.Instance.NavModule.Adf1_Ident;
        }
        else {
          _value1.ItemForeColor = cLabel;
          _value1.Text = $"{SC.SimConnectClient.Instance.NavModule.Adf1_active_hz / 1_000f:#000.0}";
        }

        if (SC.SimConnectClient.Instance.NavModule.Adf1_hasSignal) {
          _value2.Value = SC.SimConnectClient.Instance.NavModule.Adf1_Radial_deg+SC.SimConnectClient.Instance.NavModule.HDG_mag_degm;// desired heading to the NDB
          _brg.DirectionFrom = (int)CoordLib.Geo.Wrap360( (int)SC.SimConnectClient.Instance.NavModule.Adf1_Radial_deg ); // Deviation from actual course
          _brg.ItemForeColor = cScale3;
          _brg.Heading = 180; // Wind indicator is used - so inverse direction
        }
        else {
          _value2.Value = null;
          _brg.ItemForeColor = cLabel; // dimm
          _brg.DirectionFrom = 0;
          _brg.Heading = 180; // Wind indicator is used - so inverse direction
        }
      }
    }

  }
}

