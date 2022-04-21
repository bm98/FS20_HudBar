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
  class DI_SpoilersGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.SPOILER_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Sp-B";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Spoiler/SBrake Graph";

    private readonly B_Base _label;
    private readonly A_Scale _scale1;

    public DI_SpoilersGraph( ValueItemCat vCat, Label lblProto )
    {
      LabelID = LItem;
      var item = VItem.SPOILER_ANI;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_ButtonClicked;

      _scale1 = new A_Scale( ) { Minimum = 0, Maximum = 100, AlertEnabled = false, ItemForeColor = cStep };
      this.AddItem( _scale1 ); vCat.AddLbl( item, _scale1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }


    private void _label_ButtonClicked( object sender, EventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;
      if ( !SC.SimConnectClient.Instance.HudBarModule.HasSpoiler ) return;

      SC.SimConnectClient.Instance.HudBarModule.Spoilers_Arm( FSimClientIF.CmdMode.Toggle );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.HudBarModule.HasSpoiler ) {
          _label.ItemForeColor = ( SC.SimConnectClient.Instance.HudBarModule.HasSpoiler && SC.SimConnectClient.Instance.HudBarModule.Spoilers_armed ) ? cArmed : cLabel;

          _scale1.Value = SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct * 100; // 0..100
          _scale1.ItemForeColor = ( SC.SimConnectClient.Instance.HudBarModule.SpoilerHandlePosition_prct < 0.05 ) ? cOK : cStep;
        }
        else {
          _scale1.Value = null;
        }
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}
