using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

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

      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        this.Label.Text = SV.Get<bool>( SItem.bG_Nav_2_hasLOC ) ? "LOC 2" : "NAV 2";
        if (SV.Get<string>( SItem.sG_Nav_2_Name ) != "") {
          string dd = SV.Get<string>( SItem.sG_Nav_2_Name );
          if (!string.IsNullOrEmpty( SV.Get<string>( SItem.sG_Nav_2_APT_icao ) )) {
            dd += $" ({SV.Get<string>( SItem.sG_Nav_2_APT_icao )})";
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


