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
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

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

    private readonly B_Base _label;
    private readonly V_Base _value1;

    public DI_Spoilers( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.SPOLIER;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_ButtonClicked;

      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 5, OnDataArrival );
    }

    private void _label_ButtonClicked( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (!SV.Get<bool>( SItem.bG_Flp_HasSpoilers )) return;

      SV.Set( SItem.cmS_Flp_Spoilers_arm, FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Returns the GUI State for the Flaps
    /// </summary>
    private Steps SpoilerState {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return Steps.OffOK; // cannot calculate anything

        if (!SV.Get<bool>( SItem.bG_Flp_HasSpoilers )) return Steps.OffOK;

        float sp = SV.Get<float>( SItem.fGS_Flp_SpoilerHandle_position_prct );

        if (sp < 0.05) {
          return Steps.UpOK;
        }
        else if (sp < 0.15) {
          return Steps.P1;
        }
        else if (sp < 0.25) {
          return Steps.P2;
        }
        else if (sp < 0.35) {
          return Steps.P3;
        }
        else if (sp < 0.45) {
          return Steps.P4;
        }
        else if (sp < 0.55) {
          return Steps.P5;
        }
        else if (sp < 0.65) {
          return Steps.P6;
        }
        else if (sp < 0.75) {
          return Steps.P7;
        }
        return Steps.PEnd;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _label.ItemForeColor = (SV.Get<bool>( SItem.bG_Flp_HasSpoilers )
                             && SV.Get<bool>( SItem.bG_Flp_Spoilers_armed )) ? cTxLabelArmed : cTxLabel;
        _value1.Step = SpoilerState;
      }
    }

  }
}

