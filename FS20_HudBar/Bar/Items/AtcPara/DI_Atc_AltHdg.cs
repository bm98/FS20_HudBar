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
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Atc_AltHdg : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ATC_ALT_HDG;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ATC";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ATC assigned Alt/Hdg";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Atc_AltHdg( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.ATC_ALT;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Alt( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ATC_HDG;
      _value2 = new V_Deg( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ATC_WYP;
      _value3 = new V_ICAO( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      m_observerID = SV.AddObserver( Short, 5, OnDataArrival );// use the Location tracer
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        // if we have an ATC FlightPlan show ATC assignments
        if ( HudBar.AtcFlightPlan.HasFlightPlan ) {
          // ATC Alt Hdg NextWYP
            _value1.Value = HudBar.AtcFlightPlan.AssignedAlt;
            _value2.Value = HudBar.AtcFlightPlan.AssignedHdg;
            _value3.Text = HudBar.AtcFlightPlan.NextWypIdent;
        }
        else {
            _value1.Value = null;
            _value2.Value = null;
            _value3.Text = "";
        }

      }
    }

  }
}
