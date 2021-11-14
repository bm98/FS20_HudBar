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
  class DI_Vis : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VIS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "VIS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "Visibility nm";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Vis( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      // VIS
      var item = VItem.VIS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Dist( value2Proto, showUnits );
      this.AddItem( _value1 );
      vCat.AddLbl( item, _value1 as Control );

      SC.SimConnectClient.Instance.AircraftModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        _value1.Value = Tooling.NmFromM( SC.SimConnectClient.Instance.AircraftModule.Visibility_m );
      }
    }

  }
}
