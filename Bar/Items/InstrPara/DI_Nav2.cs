using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using CoordLib;
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Nav2 : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV2;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static string Short = "NAV 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static string Desc = "NAV-2 Id BRG DME";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;

    public DI_Nav2( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      var item = VItem.NAV2_ID;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( valueProto ) { ItemForeColor = cNav };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.NAV2_BRG;
      _value2 = new V_Deg( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      item = VItem.NAV2_DST;
      _value3 = new V_DmeDist( value2Proto, showUnits );
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      SC.SimConnectClient.Instance.NavModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( )
    {
      if ( this.Visible ) {
        if ( SC.SimConnectClient.Instance.NavModule.Nav2_Ident != "" ) {

          _value1.Text = Calculator.NAV2_ID;

          if ( SC.SimConnectClient.Instance.NavModule.Nav2_Signal && SC.SimConnectClient.Instance.NavModule.FromToFlag2 != 0 ) {
            _value2.Value = (float)Geo.Wrap360( SC.SimConnectClient.Instance.NavModule.Nav2_Radial_degm - 180 );
          }
          else {
            _value2.Value = null;
          }

          if ( SC.SimConnectClient.Instance.NavModule.Nav2_hasDME ) {
            _value3.Value =
                  V_DmeDist.DmeDistance( SC.SimConnectClient.Instance.NavModule.DMEdistNav2_nm, SC.SimConnectClient.Instance.NavModule.FromToFlag2 );
          }
          else {
            _value3.Value = null;
          }
        }
        else {
          _value1.Text = null;
          _value2.Value = null;
          _value3.Value = null;
        }
      }
    }

  }
}

