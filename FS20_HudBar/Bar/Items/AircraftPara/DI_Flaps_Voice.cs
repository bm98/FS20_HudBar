using System;
using System.Windows.Forms;

using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_Flaps_Voice : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.FLAPS_VOICE;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "Flaps";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Flaps state audible";

    private readonly B_Base _label;
    private readonly V_FlapsAudio _value1;
    private bool _indexMode = false;

    public DI_Flaps_Voice( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.FLAPS_VOICE;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += DI_Flaps_Voice_ButtonClicked;
      _value1 = new V_FlapsAudio( valueProto, HudBar.SpeechLib );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 5, OnDataArrival );
    }

    private void DI_Flaps_Voice_ButtonClicked( object sender, ClickedEventArgs e )
    {
      _indexMode = !_indexMode; // toggle
      _value1.SetIndexMode( _indexMode );
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
      // must unregister the callout as well
      _value1.UnregisterDataSource( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (!SC.SimConnectClient.Instance.IsConnected) _value1.Text = "UP"; // cannot calculate anything

        int flapsHandle = SV.Get<int>( SItem.iGS_Flp_HandleIndex );
        if (flapsHandle == 0) {
          // Flaps UP
          _value1.Value = V_FlapsAudio.FlapsUpValue; // to indicate UP
          _value1.ItemForeColor = GUI_Colors.ColorType.cOK;
        }
        else {
          // Flaps not UP
          if (_indexMode) {
            // handle step
            _value1.Value = flapsHandle;
          }
          else {
            // degree
            _value1.Value = SV.Get<int>( SItem.iG_Flp_Deployment_deg );
          }
          _value1.ItemForeColor = GUI_Colors.ColorType.cStep;
        }
      }
    }


  }
}

