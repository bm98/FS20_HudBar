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

namespace FS20_HudBar.Bar.Items
{
  internal class DI_Adf2_Name : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ADF2_NAME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ADF 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ADF-2 Name";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Adf2_Name( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.ADF2_NAME;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, (int)(DataArrival_perSecond / 1), OnDataArrival ); // once per sec
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (!SV.Get<bool>( SItem.bG_Nav_hasADF2 )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          return;
        }

        // Has ADF 2
        if (SV.Get<string>( SItem.sG_Nav_ADF2_Name ) != "") {
          string dd = SV.Get<string>( SItem.sG_Nav_ADF2_Name );
          _value1.Text = dd;
        }
        else {
          _value1.Text = null;
        }
      }
    }

  }
}


