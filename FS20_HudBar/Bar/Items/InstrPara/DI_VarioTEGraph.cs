using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;
using static FS20_HudBar.Bar.Calculator;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_VarioTEGraph : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VARIO_ANI;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "VARIO";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "TE-Vario Graph + Avg [m/s]";

    private readonly B_Base _label;
    private readonly A_BiScale _scale;
    private readonly V_Base _value2;

    public DI_VarioTEGraph( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.VARIO_ANI;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Vario_ButtonClicked;
      _scale = new A_BiScale( ) { Minimum = -5, Middle = 0, Maximum = 5, Dampening = 3, ItemForeColor = cOK, ItemForeColor_LScale = cAltOK };
      this.AddItem( _scale ); vCat.AddLbl( item, _scale );

      item = VItem.VARIO_ANI_AVG;
      _value2 = new V_VSpeed_mPsPM( value2Proto ) { ItemForeColor = cTxAvg };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    private EVolume _volume = EVolume.V_Silent;
    private PingLib.SoundBitePitched _soundBite = new PingLib.SoundBitePitched( HudBar.LoopSound );

    private void DI_Vario_ButtonClicked( object sender, ClickedEventArgs e )
    {
      // rotate through the Volumes
      _volume++;
      _volume = (_volume == EVolume.V_LAST) ? EVolume.V_Silent : _volume;

      // Set Note to Silence if not connected (just in case..)
      if (!SC.SimConnectClient.Instance.IsConnected) {
        // this will change the Volume if needed and clears the Ping when not connected (else it is taken in the next Update)
        _soundBite.Tone = 0; // Ask for Silence
      }

      // color if enabled, else default BG
      _label.ItemBackColor = (_volume == EVolume.V_Silent) ? cActBG : (_volume == EVolume.V_PlusMinus) ? cMetB : cLiveBG;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        var rate = SV.Get<float>( SItem.fG_Acft_VARIO_te_mps );

        _scale.Value = rate;
        _value2.Value = (float)(Math.Round( SV.Get<float>( SItem.fG_Acft_VARIO_Avg_te_mps ) * 10.0 ) / 10.0); // 0.10 increments only

        // Get the new Value and Change the Player if needed
        if (ModNote( _volume, rate, _soundBite )) {
          HudBar.PingLoop.PlayAsync( _soundBite ); // this will change the note if needed
        }
      }
    }

  }
}
