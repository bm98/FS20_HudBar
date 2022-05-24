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

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;


    public DI_DepArr( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );

      var item = VItem.DEPARR_DEP;
      _value1 = new V_ICAO_L( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.DEPARR_ARR;
      _value2 = new V_ICAO( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );// use the Location tracer
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // ATC Airport
        if (AirportMgr.HasChanged)
          _value1.Text = AirportMgr.Read( ); // update only when changed

        if (HudBar.AtcFlightPlan.HasFlightPlan) {
          _value1.Text = HudBar.AtcFlightPlan.Departure;
          _value2.Text = HudBar.AtcFlightPlan.Destination;
        }
        else {
          _value1.Text = "...."; // default text
          _value2.Text = "...."; // default text
        }

      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}

