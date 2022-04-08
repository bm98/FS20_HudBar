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
using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.Bar.Items
{
  class DI_Cht_F : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.CHT_F;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "CHT";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Engine CHT °F";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;
    private readonly V_Base _value4;

    public DI_Cht_F( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto, bool showUnits )
    {
      LabelID = LItem;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      var item = VItem.E1_CHT_F;
      _value1 = new V_Temp_F( value2Proto, showUnits );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.E2_CHT_F;
      _value2 = new V_Temp_F( value2Proto, showUnits );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      // add 2 more values
      this.TwoRows = true;
      item = VItem.E3_CHT_F;
      _value3 = new V_Temp_F( value2Proto, showUnits ) { Visible = false };
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      item = VItem.E4_CHT_F;
      _value4 = new V_Temp_F( value2Proto, showUnits ) { Visible = false };
      this.AddItem( _value4 ); vCat.AddLbl( item, _value4 );

      this.IsEngineItem = true;
      m_observerID = SC.SimConnectClient.Instance.HudBarModule.AddObserver( Short, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    public void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        _value1.Value = SC.SimConnectClient.Instance.HudBarModule.Engine1_cht_degC;
        _value2.Value = SC.SimConnectClient.Instance.HudBarModule.Engine2_cht_degC;
        _value3.Value = SC.SimConnectClient.Instance.HudBarModule.Engine3_cht_degC;
        _value4.Value = SC.SimConnectClient.Instance.HudBarModule.Engine4_cht_degC;
      }
    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      SC.SimConnectClient.Instance.HudBarModule.RemoveObserver( m_observerID );
    }

  }
}
