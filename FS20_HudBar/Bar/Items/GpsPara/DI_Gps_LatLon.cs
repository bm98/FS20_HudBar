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
  class DI_Gps_LatLon : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_LAT_LON;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "POS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft Lat/Lon";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Gps_LatLon( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GPS_LAT;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Latitude( value2Proto ) { ItemForeColor = cGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.GPS_LON;
      _value2 = new V_Longitude( value2Proto ) { ItemForeColor = cGps };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.GpsModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      if (m_observerID > 0) {
        SC.SimConnectClient.Instance.GpsModule.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = (float)SC.SimConnectClient.Instance.GpsModule.LAT;
        _value2.Value = (float)SC.SimConnectClient.Instance.GpsModule.LON;
      }
    }

  }
}
