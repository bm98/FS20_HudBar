using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items
{
  class DI_Nav1_Name : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV1_NAME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "NAV 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-1 Name (Apt)";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Nav1_Name( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.NAV1_NAME;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 1, OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        if (!SV.Get<bool>( SItem.bG_Nav_1_available )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          return;
        }

        // Has NAV1
        this.Label.Text = SV.Get<bool>( SItem.bG_Nav_1_hasLOC) ? "LOC 1" : "NAV 1";
        if (SV.Get<string>( SItem.sG_Nav_1_Name) != "" ) {
          string dd = SV.Get<string>( SItem.sG_Nav_1_Name );
          if ( !string.IsNullOrEmpty(SV.Get<string>( SItem.sG_Nav_1_APT_icao) ) ) {
            dd += $" ({SV.Get<string>( SItem.sG_Nav_1_APT_icao )})";
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


