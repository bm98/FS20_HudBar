using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static dNetBm98.Units;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// MacCready Speed and Setting [kt] [kt]
  /// </summary>
  class DI_VarioMCS_kt : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.MCRAD_KT;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "MCRAD";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "MacCready Speed+Set [kt]";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_VarioMCS_kt( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.MCRAD_KT_SPD;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Speed( valueProto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.MCRAD_KT_SET;
      _value2 = new V_VSpeed_ktPM( value2Proto ) { ItemForeColor = cTxSet };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Short, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = (float)Kt_From_Kmh( SV.Get<float>( SItem.fG_Acft_MacCreadySpeedToFly_kmh ) );
        _value2.Value = (float)Kt_From_Mps( SV.Get<float>( SItem.fG_Acft_MacCreadySetting_mps ) );
      }
    }

  }
}

