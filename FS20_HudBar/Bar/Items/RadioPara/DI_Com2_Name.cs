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
  class DI_Com2_Name : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.COM2_NAME;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "COM 2";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "COM-2 Type ID";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Com2_Name( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.COM2_TYPE;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO_L( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.COM2_ID;
      _value2 = new V_Text( value2Proto ) { ItemForeColor = cTxInfo };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      m_observerID = SV.AddObserver( Short, (int)DataArrival_perSecond, OnDataArrival ); // once per sec
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if ( this.Visible ) {
        // seems that if no station is tuned in the reply is Type= ACTIVE, Id= COM
        _value1.Text = SV.Get<string>( SItem.sG_Com_2_type );
        _value2.Text = (SV.Get<string>( SItem.sG_Com_2_id ) == "COM") ? "..." : SV.Get<string>( SItem.sG_Com_2_id );
      }
    }

  }
}

