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
  class DI_Flaps : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FLAPS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Flaps";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Flaps";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Flaps( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Symbol;
      var item = VItem.FLAPS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 5, OnDataArrival );
    }

    /// <summary>
    /// Returns the GUI State for the Flaps
    /// </summary>
    private Steps FlapsState {
      get {
        if (!SC.SimConnectClient.Instance.IsConnected) return Steps.UpOK; // cannot calculate anything

        int fi = SV.Get<int>( SItem.iGS_Flp_HandleIndex );
        if (fi == 0) {
          return Steps.UpOK;
        }
        else if (fi == SV.Get<int>( SItem.iG_Flp_FlapsPositions_num )) {
          return Steps.PEnd;
        }

        else if (fi == 1) {
          return Steps.P1;
        }
        else if (fi == 2) {
          return Steps.P2;
        }
        else if (fi == 3) {
          return Steps.P3;
        }
        else if (fi == 4) {
          return Steps.P4;
        }
        else if (fi == 5) {
          return Steps.P5;
        }
        else if (fi == 6) {
          return Steps.P6;
        }
        else if (fi == 7) {
          return Steps.P7;
        }
        return Steps.UpOK;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Step = FlapsState;
      }
    }

  }
}

