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
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Est_ALT : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.EST_ALT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "WP-ALT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Estimated ALT @WYP";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Est_ALT( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.EST_ALT;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Alt( valueProto ) { ItemForeColor = cEst };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
          float tgtAlt = SC.SimConnectClient.Instance.GpsModule.WYP_Alt;
          // Estimates use WYP ALT if >0 (there is no distinction if a WYP ALT is given - it is 0 if not)
          ColorType estCol = cEst;
          if ( tgtAlt == 0 ) {
            // use Set Alt if WYP ALT is zero (see comment above)
            tgtAlt = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;
            estCol = cSet;
          }
          _value1.Value = Calculator.AltitudeAtTgt( SC.SimConnectClient.Instance.GpsModule.WYP_Dist );
          _value1.ItemForeColor = estCol;
        }
        else {
          _value1.Value = null; // cannot if we don't have a WYP to aim at
        }
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.GpsModule.RemoveObserver( m_observerID );
    }

  }
}
