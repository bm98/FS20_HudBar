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
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// Auto ETrim  - click to toggle on/off, once enabled it remains active for an amount of time
  ///   (currently 20 sec)
  /// </summary>
  class DI_A_ETrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.A_ETRIM;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "A-ETrim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Auto E-Trim";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    private const float c_incPerWheel = 0.001f; // Get 0.1% per mouse inc
    private const int c_aETrim_sec = 20; // sec  AutoETrim active time when clicked
    private DateTime _endTime = DateTime.Now; // to switch AET off when expired

    public DI_A_ETrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      TText = "Click to enable Auto Elevator Trim for 20 seconds.";

      LabelID = LItem;
      var item = VItem.A_ETRIM;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct_999( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;
      _label.MouseWheel += _label_MouseWheel;
      _label.Scrollable= true;
      _label.Cursor = Cursors.SizeNS;

      AddObserver( Short, (int)(DataArrival_perSecond / 5), OnDataArrival );
    }

    private void _label_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      float current = SV.Get<float>( SItem.fGS_Trm_ElevatorTrim_prct100 );

      if (e.Delta > 0) {
        // Wheel Up - nose down
        SV.Set( SItem.fGS_Trm_ElevatorTrim_prct100, current - c_incPerWheel );
      }
      else if (e.Delta < 0) {
        // Wheel Down
        SV.Set( SItem.fGS_Trm_ElevatorTrim_prct100, current + c_incPerWheel );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      /* TODO REMOVED
      if (SC.SimConnectClient.Instance.IsConnected) {
        SC.SimConnectClient.Instance.AutoETrimModule.Enabled = !SC.SimConnectClient.Instance.AutoETrimModule.Enabled; // toggles
        _endTime = DateTime.Now + TimeSpan.FromSeconds( c_aETrim_sec ); // does not matter if it is off, else it starts again
      }
      */
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      /* TODO REMOVED
      if (this.Visible) {
        this.ColorType.ItemBackColor = SC.SimConnectClient.Instance.AutoETrimModule.Enabled ? cLiveBG : cActBG;
        _value1.Value = SC.SimConnectClient.Instance.SV.ElevatorTrim_prct;
      }

      // switch the module off if the end time has passed
      if (SC.SimConnectClient.Instance.AutoETrimModule.Enabled && (DateTime.Now > _endTime)) {
        SC.SimConnectClient.Instance.AutoETrimModule.Enabled = false;
      }
      */
    }

  }
}
