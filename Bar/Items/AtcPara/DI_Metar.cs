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
    public static string Short = "METAR";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "METAR Close to location";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    // A HudBar standard ToolTip for the Metar Display
    private ToolTip_Base _toolTip = new ToolTip_Base();

    public DI_Metar( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      _metar.MetarDataEvent += _metar_MetarDataEvent;
      LabelID = LItem;
      var item = VItem.METAR;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );// use the Location tracer
    }

    private void _metar_MetarDataEvent( object sender, MetarLib.MetarTafDataEventArgs e )
    {
      _metar.Update( e.MetarTafData );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        _metar.Clear( );
        _metar.PostMETAR_Request( SC.SimConnectClient.Instance.HudBarModule.Lat,
                                    SC.SimConnectClient.Instance.HudBarModule.Lon,
                                    SC.SimConnectClient.Instance.GpsModule.GTRK ); // from current pos along the current track
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        // Location METAR
        if ( _metar.HasNewData ) {
          // avoiding redraw for every cycle
          _value1.Text = _metar.StationText;
          _toolTip.SetToolTip( this.Label, _metar.Read( ) );
          this.ColorType.ItemBackColor = _metar.ConditionColor;
        }
      }
    }

  }
}
