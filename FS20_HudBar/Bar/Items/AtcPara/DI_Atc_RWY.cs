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
  class DI_Atc_RWY : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ATC_RWY;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RWY";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ATC Rwy (Dist, Track, Alt)";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Atc_RWY( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.ATC_RWY_LON;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_AptDist( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ATC_RWY_LAT;
      _value2 = new V_LatDist( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ATC_RWY_ALT;
      _value3 = new V_Alt( value2Proto );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      AddObserver( Short, 0.5f, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // ATC Runway
        if (SV.Get<bool>( SItem.bG_Atc_RunwaySelected )) {
          _value1.Value = SV.Get<float>( SItem.fG_Atc_Runway_Distance_nm );

          float f = SV.Get<float>( SItem.fG_Atc_Runway_Displacement_ft );
          _value2.Value = f;
          _value2.ItemForeColor = (Math.Abs( f ) <= 3) ? cTxActive : cTxInfo; // green if within +- 3ft

          f = SV.Get<float>( SItem.fG_Atc_Runway_HeightAbove_ft );
          _value3.Value = f;
          _value3.ItemForeColor = (f <= 1500) ? cTxRA : cTxInfo;  // yellow if below 1500ft
        }
        else {
          _value1.Value = null;
          _value2.Value = null;
          _value3.Value = null;
        }
      }
    }

  }
}
