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
  class DI_Spoilers : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SPOILER;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Sp-B";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Spoiler / Speedbrakes";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Spoilers( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.SPOLIER;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Returns the GUI State for the Flaps
    /// </summary>
    private static Steps SpoilerState {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return Steps.Up; // cannot calculate anything

        if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.05 ) {
          return Steps.Up;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.15 ) {
          return Steps.P1;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.25 ) {
          return Steps.P2;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.35 ) {
          return Steps.P3;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.45 ) {
          return Steps.P4;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.55 ) {
          return Steps.P5;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.65 ) {
          return Steps.P6;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.75 ) {
          return Steps.P7;
        }
        return Steps.PEnd;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Step = SpoilerState;
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}

