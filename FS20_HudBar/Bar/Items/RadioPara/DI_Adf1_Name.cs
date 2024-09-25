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
  class DI_Adf1_Name : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ADF1_NAME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ADF 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ADF-1 Name";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Adf1_Name( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.ADF1_NAME;
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
      if (this.Visible) {
        if (!SV.Get<bool>( SItem.bG_Nav_hasADF1 )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          return;
        }

        // Has ADF 1
        if (SV.Get<string>( SItem.sG_Nav_ADF1_Name ) != "") {
          string dd = SV.Get<string>( SItem.sG_Nav_ADF1_Name );
          _value1.Text = dd;
        }
        else {
          _value1.Text = null;
        }
      }
    }

  }
}

