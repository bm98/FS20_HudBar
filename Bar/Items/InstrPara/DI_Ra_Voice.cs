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
  class DI_Ra_Voice : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.RA_VOICE;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RAv";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft RA ft audible";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Ra_Voice( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.RA_VOICE;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_RAaudio( valueProto, showUnits, HudBar.SpeechLib ) { ItemForeColor = cRA };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.HudBarModule.AltAoG_ft <= 1500 ) {
          _value1.Value = SC.SimConnectClient.Instance.HudBarModule.AltAoG_ft;
        }
        else {
          _value1.Text = " .....";
        }
      }
    }

  }
}
