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
  class DI_ESIGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ESI_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ESI";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ESI Panel";

    private readonly V_Base _label;
    private readonly A_AHor _scale1;
    private readonly A_FPA _scale2;

    public DI_ESIGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.ATT_ANI;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _scale1 = new A_AHor( ) { Minimum = -6, Maximum = 6, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      item = VItem.FPA_ANI;
      _scale2 = new A_FPA( ) { MinimumVer = -6, MaximumVer = 6, MinimumHor = -6 * 2, MaximumHor = 6 * 2, ItemForeColor = cOK };
      this.AddItem( _scale2 ); vCat.AddLbl( item, _scale2 );

      // using GPS but we use the HudBar Ping to update those as well
      // if we observe GPS as well we get two calls for no better data
      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival ); 
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _scale1.PitchAngle = SC.SimConnectClient.Instance.HudBarModule.Pitch_deg;
        _scale1.BankAngle = SC.SimConnectClient.Instance.HudBarModule.Bank_deg;

        _scale2.VerticalAngle = SC.SimConnectClient.Instance.HudBarModule.FlightPathAngle_deg;
        _scale2.HorizontalAngle = (float)CoordLib.Geo.Wrap180( SC.SimConnectClient.Instance.GpsModule.GTRK_true - SC.SimConnectClient.Instance.GpsModule.GHDG_true ); // +-180°
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}
