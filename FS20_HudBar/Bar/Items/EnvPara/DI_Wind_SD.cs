using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  class DI_Wind_SD : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.WIND_SD;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "WIND";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Wind dir° @ speed kt";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly A_WindArrow _wind;
    private readonly V_Base _value2;

    public DI_Wind_SD( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      // Wind Direction, Speed
      var item = VItem.WIND_DIR;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Deg( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.WIND_SPEED;
      _value2 = new V_Speed( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.WIND_DIRA;
      _wind = new A_WindArrow( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true, ItemForeColor = cScale0 };
      this.AddItem( _wind ); vCat.AddLbl( item, _wind );

      AddObserver( Short, 2, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Acft_WindDirection_deg );
        _wind.DirectionFrom = (int)SV.Get<float>( SItem.fG_Acft_WindDirection_deg );
        _wind.Heading = (int)SV.Get<float>( SItem.fG_Nav_HDG_true_deg );
        _wind.ItemForeColor = WindColor( SV.Get<float>( SItem.fG_Acft_WindSpeed_kt ) );
        _value2.Value = SV.Get<float>( SItem.fG_Acft_WindSpeed_kt );
      }
    }

    // Get a Color for the Arrow (Beaufort scale based)
    private ColorType WindColor( float knots )
    {
      if (knots <= 1) return cScale0;   // Beaufort <=1
      if (knots <= 7) return cScale1;   // Beaufort >1 <=3
      if (knots <= 16) return cScale2;  // Beaufort >3 <=5
      if (knots <= 28) return cScale3;  // Beaufort >5 <=7
      if (knots <= 41) return cScale4;  // Beaufort >7 <=9
      return cScale5;                     // Beaufort >9
    }
  }
}
