﻿using System;
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
    private readonly V_RAaudio _value1;

    public DI_Ra_Voice( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.RA_VOICE;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_RAaudio( valueProto, HudBar.SpeechLib ) { ItemForeColor = cTxRA };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.HudBarModule ); // use the generic one
      // must unregister the callout as well
      _value1.UnregisterDataSource( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (SC.SimConnectClient.Instance.HudBarModule.AltAoG_ft <= 1500) {
          this.ColorType.ItemForeColor = SC.SimConnectClient.Instance.HudBarModule.Sim_OnGround ? cTxActive : cTxLabel;
          _value1.Value = SC.SimConnectClient.Instance.HudBarModule.AltAoG_ft;
        }
        else {
          this.ColorType.ItemForeColor = cTxLabel;
          _value1.Text = " .....";
        }
      }
    }

  }
}
