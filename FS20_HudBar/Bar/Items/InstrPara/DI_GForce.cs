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
  class DI_GForce : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GFORCE;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "G";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "G Force Current";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_GForce( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.GFORCE_Cur;
      _label = new V_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_GForce( valueProto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, (int)(DataArrival_perSecond / 2), OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SV.Get<float>( SItem.fG_Acft_GForce_current_g);
      }
    }

  }
}

