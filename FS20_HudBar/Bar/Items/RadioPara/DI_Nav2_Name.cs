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
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Nav2_Name : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV2_NAME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-2 Name (Apt)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Nav2_Name( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.NAV2_NAME;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SC.SimConnectClient.Instance.NavModule.AddObserver( Short, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SC.SimConnectClient.Instance.NavModule ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        this.Label.Text = SC.SimConnectClient.Instance.NavModule.Nav2_hasLOC ? "LOC 2" : "NAV 2";
        if ( SC.SimConnectClient.Instance.NavModule.Nav2_Name != "" ) {
          string dd = SC.SimConnectClient.Instance.NavModule.Nav2_Name;
          if ( !string.IsNullOrEmpty( SC.SimConnectClient.Instance.NavModule.APT2 ) ) {
            dd += $" ({SC.SimConnectClient.Instance.NavModule.APT2})";
          }
          _value1.Text = dd;
        }
        else {
          _value1.Text = null;
        }
      }
    }

  }
}


