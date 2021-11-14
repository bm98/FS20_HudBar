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
  class DI_ETrim : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ETrim;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "E-Trim";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Elevator Trim";

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_ETrim( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // Elevator (plain)
      // All ERA-Trim label get a button to activate the 0 Trim action
      var item = VItem.ETrim;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct( value2Proto );
      this.AddItem( _value1 );
      vCat.AddLbl( item, _value1 );

      _label.ButtonClicked += _label_ButtonClicked;

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct = 0; // Set 0
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      // SimRate
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      }
    }

  }
}
