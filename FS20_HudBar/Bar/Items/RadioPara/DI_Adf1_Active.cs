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
  class DI_Adf1_Active : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.ADF1;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "ADF 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "ADF-1 Id Bearing/Needle";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly A_WindArrow _brg;

    public DI_Adf1_Active( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.ADF1_ID;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( valueProto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.ADF1_BRG;
      _value2 = new V_Deg( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.ADF1_ANI;
      _brg = new A_WindArrow( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true, ItemForeColor = cScale3 };
      _brg.Heading = 180; // Wind indicator is used - so inverse direction
      this.AddItem( _brg ); vCat.AddLbl( item, _brg );

      AddObserver( Desc, 2, OnDataArrival );
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
          _value2.Value = null;
          _brg.ItemForeColor = cTxDim; // dimm
          _brg.DirectionFrom = 0;
          return;
        }

        // Has ADF 1
        if (SV.Get<string>( SItem.sG_Nav_ADF1_Ident ) != "") {
          _value1.ItemForeColor = cTxNav;
          _value1.Text = SV.Get<string>( SItem.sG_Nav_ADF1_Ident );
        }
        else {
          _value1.ItemForeColor = cTxDim;
          _value1.Text = $"{SV.Get<int>( SItem.iG_Nav_ADF1_active_hz ) / 1_000f:#000.0}";
        }

        if (SV.Get<bool>( SItem.bG_Nav_ADF1_hasSignal )) {
          // desired heading to the NDB
          _value2.Value = (float)CoordLib.Geo.Wrap360( SV.Get<float>( SItem.fG_Nav_ADF1_Radial_mag_degm ) + 180f );
          // Direction of Station
          var dir = (float)CoordLib.Geo.DirectionOf( SV.Get<float>( SItem.fG_Nav_ADF1_Radial_mag_degm ) + 180f,
                                                     SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm ) );

          _brg.DirectionFrom = (int)dir;
          _brg.ItemForeColor = Math.Abs( dir ) <= 3 ? cOK : Math.Abs( dir ) <= 6 ? cWarn : cInfo; // green / orange / white
        }
        else {
          _value2.Value = null;
          _brg.ItemForeColor = cTxDim; // dimm
          _brg.DirectionFrom = 0;
        }
      }
    }

  }
}

