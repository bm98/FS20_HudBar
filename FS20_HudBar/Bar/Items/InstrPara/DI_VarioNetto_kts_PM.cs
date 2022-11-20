﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;
using static FS20_HudBar.Bar.Calculator;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_VarioNetto_kts_PM : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NETTO_KT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NETTO";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Netto-Vario + Avg [kts]";

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_VarioNetto_kts_PM( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.NETTO_KTS;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Vario_ButtonClicked;
      _value1 = new V_VSpeed_ktPM( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.NETTO_KTS_AVG;
      _value2 = new V_VSpeed_ktPM( value2Proto ) { ItemForeColor = cTxAvg };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
    }


    private EVolume _volume = EVolume.V_Silent;
    private PingLib.SoundBite _soundBite = new PingLib.SoundBite( HudBar.LoopSound );

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
        var rate = SC.SimConnectClient.Instance.HudBarModule.VARIO_netto_mps;
        _value1.Value = (float)(Math.Round( Conversions.Kt_From_Mps( rate ) * 20.0 ) / 20.0); // 0.05 increments only
        _value2.Value = (float)(Math.Round( Conversions.Kt_From_Mps( SC.SimConnectClient.Instance.HudBarModule.VARIO_Avg_netto_mps ) * 10.0 ) / 10.0); // 0.10 increments only

        // Get the new Value and Change the Player if needed
        if (Calculator.ModNote( _volume, rate, _soundBite )) {
          HudBar.PingLoop.PlayAsync( _soundBite ); // this will change the note if needed
        }
      }
    }

  }
}

