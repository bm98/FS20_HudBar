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
using FSimClientIF;

namespace FS20_HudBar.Bar.Items
{
  class DI_FlightLog : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.LOG;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Fl-Rec";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Fligh Recorder";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_FlightLog( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Fligh Recorder\nClick to toggle recording on/off";
      LabelID = LItem;
      var item = VItem.LOG;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival ); // get updates with the HudBar pace
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // no SC
      if ( !SC.SimConnectClient.Instance.FlightLogModule.Enabled ) return; // Not enabled in Config

      if ( SC.SimConnectClient.Instance.FlightLogModule.LogMode == FlightLogMode.Off ) {
        // toggle ON
        SC.SimConnectClient.Instance.FlightLogModule.LogMode = FlightLogMode.Kml;
      }
      else {
        // toggle OFF
        SC.SimConnectClient.Instance.FlightLogModule.LogMode = FlightLogMode.Off;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if ( !SC.SimConnectClient.Instance.FlightLogModule.Enabled ) {
          _value1.Text = "disabled";
          _value1.ItemForeColor = cTxDim;
        }
        else {
          _value1.Text = ( SC.SimConnectClient.Instance.FlightLogModule.LogMode == FlightLogMode.Off ) ? "not rec." : "recording";
          _value1.ItemForeColor = ( SC.SimConnectClient.Instance.FlightLogModule.LogMode == FlightLogMode.Off ) ? cTxWarn : cTxActive;
        }
      }
    }

  }
}

