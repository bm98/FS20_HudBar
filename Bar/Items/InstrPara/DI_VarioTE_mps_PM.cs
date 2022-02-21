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
  class DI_VarioTE_mps_PM : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.VARIO_MPS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "VARIO";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "TE-Vario + Avg [m/s]";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_VarioTE_mps_PM( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.VARIO_MPS;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Vario_ButtonClicked;
      _value1 = new V_VSpeed_mPsPM( valueProto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.VARIO_MPS_AVG;
      _value2 = new V_VSpeed_mPsPM( value2Proto, showUnits ) { ItemForeColor = cAvg };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    // The Ping Part
    private enum EVolume
    {
      V_Silent=0,
      // audible ones
      V_Plus,
      V_PlusMinus,
      // Can use the audible levels above
      V_LAST,
    }

    private EVolume _volume= EVolume.V_Silent;
    private PingLib.SoundBite _soundBite = new PingLib.SoundBite(HudBar.LoopSound);

    private void DI_Vario_ButtonClicked( object sender, ClickedEventArgs e )
    {
      // rotate through the Volumes
      _volume++;
      _volume = ( _volume == EVolume.V_LAST ) ? EVolume.V_Silent : _volume;

      // Set Note to Silence if not connected (just in case..)
      if ( !SC.SimConnectClient.Instance.IsConnected ) {
        // this will change the Volume if needed and clears the Ping when not connected (else it is taken in the next Update)
        _soundBite.Tone = 0; // Ask for Silence
      }

      // color if enabled, else default BG
      _label.ItemBackColor = ( _volume == EVolume.V_Silent ) ? cActBG : ( _volume == EVolume.V_PlusMinus ) ? cMetB : cLiveBG;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        var rate = SC.SimConnectClient.Instance.HudBarModule.VARIO_te_mps;

        _value1.Value = (float)( Math.Round( rate * 20.0 ) / 20.0 ); // 0.05 increments only
        _value2.Value = (float)( Math.Round( SC.SimConnectClient.Instance.HudBarModule.VARIO_Avg_te_mps * 10.0 ) / 10.0 ); // 0.10 increments only

        // Get the new Value and Change the Player if needed
        if ( ModNote( rate, _soundBite ) ) {
          HudBar.PingLoop.PlayAsync( _soundBite ); // this will change the note if needed
        }
      }
    }

    // Set the value dependent Note in the soundBite
    // Returns true if the note has changed
    private bool ModNote( float value, PingLib.SoundBite soundBite )
    {
      uint note = 0; // default is silent
      if ( ( _volume == EVolume.V_Plus ) && ( value > 0 ) ) {
        note = Calculator.ToneFromVS( value );
      }
      else if ( _volume == EVolume.V_PlusMinus ) {
        note = Calculator.ToneFromVS( value );
      }
      bool changed = soundBite.Tone!= note;
      soundBite.Tone = note;
      return changed;
    }


  }
}
