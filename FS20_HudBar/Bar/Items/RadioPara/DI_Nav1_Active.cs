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
  class DI_Nav1_Active : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV1;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-1 Id BRG DME";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Nav1_Active( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.NAV1_ID;
      _label = new L_Text(lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( valueProto ) { ItemForeColor = cInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.NAV1_BRG;
      _value2 = new V_Deg( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.NAV1_DST;
      _value3 = new V_DmeDist( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

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
      if ( this.Visible ) {
        this.Label.Text = SC.SimConnectClient.Instance.NavModule.Nav1_hasLOC ? "LOC 1" : "NAV 1";
        if ( SC.SimConnectClient.Instance.NavModule.Nav1_Ident != "" ) {
          _value1.ItemForeColor = cNav;
          _value1.Text = Calculator.NAV1_ID;

          var brg = (float)Geo.Wrap360( SC.SimConnectClient.Instance.NavModule.Nav1_Radial_degm - 180 ); // direction towards the station 
          brg = SC.SimConnectClient.Instance.NavModule.Nav1_hasLOC ? SC.SimConnectClient.Instance.NavModule.CRS1 : brg; // for LOC use the LOC heading as direction

          if ( SC.SimConnectClient.Instance.NavModule.Nav1_Signal && SC.SimConnectClient.Instance.NavModule.FromToFlag1 != 0 ) {
            _value2.Value = brg;
          }
          else {
            _value2.Value = null;
          }

          if ( SC.SimConnectClient.Instance.NavModule.Nav1_hasDME ) {
            var diversion = Geo.Wrap180(SC.SimConnectClient.Instance.GpsModule.GTRK - brg); // delta < +-90 -> going towards the Station, outside going away from it
            var toFromFlag = (Math.Abs(diversion) <= 90) ? 1 : 2; // same values as the Sim ToFrom Flag (1->To, 2-> From, 0->Off)
            toFromFlag = SC.SimConnectClient.Instance.NavModule.Nav1_Signal ? toFromFlag : 0; // no signal -> direction Off

            _value3.Value =
                  Conversions.DmeDistance( SC.SimConnectClient.Instance.NavModule.Nav1_DMEdist_nm, toFromFlag );
          }
          else {
            _value3.Value = null;
          }
        }
        else {
          _value1.ItemForeColor = cInfo;
          _value1.Text = $"{SC.SimConnectClient.Instance.NavModule.Nav1_active_hz / 1_000_000f:000.00}";
          _value2.Value = null;
          _value3.Value = null;
        }
      }
    }

  }
}

