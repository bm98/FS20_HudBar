using System;
using System.Windows.Forms;

using SC = SimConnectClient;

using FSimClientIF;
using static FSimClientIF.Sim;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_SlipBallGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SBALL_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "T_2M";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Turn Coordinator";

    private readonly B_Base _label;
    private readonly A_SlipBall _scale1;

    // Std Turnrate setting
    private int _stdTurnDuration_min = 2; // 2 minutes for 360 degrees
    private float _stdTurnRate_degPSec = 3;
    public DI_SlipBallGraph( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto )
    {
      TText = "Std Turn 2/4 Min\nClick to toggle";

      LabelID = LItem;
      DiLayout = ItemLayout.GraphX1;
      var item = VItem.SBALL_ANI;
      _label = new B_Text( item, value2Proto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_SlipBallGraph_ButtonClicked;
      // min/max: as per SimVar
      _scale1 = new A_SlipBall( ) { MinimumHor = -127, MaximumHor = 127, ItemForeColor = cOK };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      // init value
      _stdTurnRate_degPSec = 360f / (_stdTurnDuration_min * 60);

      AddObserver( Desc, 5, OnDataArrival );
    }

    // Label button click
    private void DI_SlipBallGraph_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        // switch between 2 and 4 Min
        _stdTurnDuration_min = (_stdTurnDuration_min == 4) ? 2 : 4;
        // update value
        _stdTurnRate_degPSec = 360f / (_stdTurnDuration_min * 60);
        _label.Text = $"T_{_stdTurnDuration_min}M";
      }
    }


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        int ball = SV.Get<int>( SItem.iG_Acft_TurnBall_pos128 );
        if (Math.Abs( ball ) > 64) _scale1.ItemForeColor = cAlert;
        else if (Math.Abs( ball ) > 35) _scale1.ItemForeColor = cWarn;
        else _scale1.ItemForeColor = cOK;

        _scale1.IntValue = (int)((Math.Abs( ball ) / 127f * 50f) + 40f); // size 40..90%
        _scale1.Value = ball; // pos

        // current turn rate % of Std TurnRate
        float stdTurn_prct = (SV.Get<float>( SItem.fG_Acft_TurnRate_degPsec ) / _stdTurnRate_degPSec) * 100f;
        _scale1.Turnrate_prct = (int)stdTurn_prct;
      }
    }

  }
}
