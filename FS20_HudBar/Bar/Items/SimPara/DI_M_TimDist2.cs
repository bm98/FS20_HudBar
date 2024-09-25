using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_M_TimDist2 : DispItem
  {
    // Checkpoint Meters live throughout the application 
    private static readonly CPointMeter _cpMeter = new CPointMeter();

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.M_TIM_DIST2;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "CP 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Checkpoint 2";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_M_TimDist2( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Click to enable Meter 2\nClick twice within 2 sec. to switch it off";

      LabelID = LItem;
      var item = VItem.M_Elapsed2;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_TimeHHMMSS( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.M_Dist2;
      _value2 = new V_Dist( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      _label.ButtonClicked += _label_ButtonClicked;

      AddObserver( Short, 2, OnDataArrival ); // twice per sec
    }
    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        if ( _cpMeter.Started && _cpMeter.Duration <= 2 ) {
          // if stopped within 2 sec, Stop it
          _cpMeter.Stop( );
        }
        else {
          _cpMeter.Start( new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) ),
                      SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
        }
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        var latLon = new LatLon( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) );
        _cpMeter.Lapse( latLon, SV.Get<double>( SItem.dG_Env_Time_zulu_sec ) );
        _value1.Value = _cpMeter.Duration;
        _value2.Value = (float)_cpMeter.Distance;
        this.ColorType.ItemBackColor = _cpMeter.Started ? cLiveBG : cActBG;
      }
    }

  }
}
