using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Est_ALT : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.EST_ALT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "WP-ALT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "GPS Estimated ALT @WYP";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Est_ALT( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.ValueRight;
      var item = VItem.EST_ALT;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Alt( valueProto ) { ItemForeColor = cTxEst };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Short, 2, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = Calculator.AltitudeAtTgt( SV.Get<float>( SItem.fG_Gps_WYP_dist_nm ) );
      }
    }

  }
}
