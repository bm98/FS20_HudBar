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
  class DI_Atc_RWY : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ATC_RWY;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RWY";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ATC Rwy (Dist, Track, Alt)";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Atc_RWY( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.ATC_RWY_LON;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_AptDist( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ATC_RWY_LAT;
      _value2 = new V_LatDist( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ATC_RWY_ALT;
      _value3 = new V_Alt( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );// use the Location tracer
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        // ATC Runway
        if ( SC.SimConnectClient.Instance.HudBarModule.AtcRunwaySelected ) {
          _value1.Value = SC.SimConnectClient.Instance.HudBarModule.AtcRunway_Distance_nm;

          float f = SC.SimConnectClient.Instance.HudBarModule.AtcRunway_Displacement_ft;
          _value2.Value = f;
          _value2.ItemForeColor = ( Math.Abs( f ) <= 3 ) ? cOK : cInfo; // green if within +- 3ft

          f = SC.SimConnectClient.Instance.HudBarModule.AtcRunway_HeightAbove_ft;
          _value3.Value = f;
          _value3.ItemForeColor = ( f <= 500 ) ? cRA : cInfo;  // RA yellow if below 500ft
        }
        else {
          _value1.Value = null;
          _value2.Value = null;
          _value3.Value = null;
        }
      }
    }

  }
}
