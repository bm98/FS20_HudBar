using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Metar : DispItem
  {
    // METAR instance lives throughout the application 
    private static readonly HudMetar _metar = new HudMetar( ); // For the Airport


    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.METAR;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "METAR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "METAR Close to location";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    // A HudBar standard ToolTip for the Metar Display
    private ToolTip_Base _toolTip = new ToolTip_Base( );

    public DI_Metar( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      _metar.MetarDataEvent += _metar_MetarDataEvent;
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.METAR;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      AddObserver( Desc, 0.5f, OnDataArrival );
    }
    // Disconnect from updates

    private void _metar_MetarDataEvent( object sender, MetarLib.MetarTafDataEventArgs e )
    {
      _metar.Update( e.MetarTafData );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        _metar.Clear( );
        _metar.PostMETAR_Request( SV.Get<double>( SItem.dGS_Acft_Lat ),
                                  SV.Get<double>( SItem.dGS_Acft_Lon ),
                                  SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm ) ); // from current pos along the current track
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // Location METAR
        if (_metar.HasNewData) {
          // avoiding redraw for every cycle
          _value1.Text = _metar.StationText;
          _toolTip.SetToolTip( this.Label, _metar.Read( ) );
          this.ColorType.ItemBackColor = _metar.ConditionColor;
        }
      }
    }

  }
}
