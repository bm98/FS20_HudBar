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
      var item = VItem.FLAPS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Steps( signProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Returns the GUI State for the Flaps
    /// </summary>
    private static Steps FlapsState {
      get {
        if ( !SC.SimConnectClient.Instance.IsConnected ) return Steps.Up; // cannot calculate anything

        if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 0 ) {
          return Steps.Up;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == SC.SimConnectClient.Instance.HudBarModule.NumberOfFlapsPositions ) {
          return Steps.PEnd;
        }

        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 1 ) {
          return Steps.P1;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 2 ) {
          return Steps.P2;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 3 ) {
          return Steps.P3;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 4 ) {
          return Steps.P4;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 5 ) {
          return Steps.P5;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 6 ) {
          return Steps.P6;
        }
        else if ( SC.SimConnectClient.Instance.HudBarModule.FlapsHandleIndex == 7 ) {
          return Steps.P7;
        }
        return Steps.Up;
      }
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Step = FlapsState;
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}

