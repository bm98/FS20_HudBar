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
  class DI_Ra : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.RA;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "RA";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Aircraft RA ft";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_Ra( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.RA;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Alt( valueProto ) { ItemForeColor = cTxRA };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
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
      if (this.Visible) {
        
        if (SV.Get<float>( SItem.fG_Acft_AltAoG_ft ) <= Calculator.RA_Limit_ft) {
          this.ColorType.ItemForeColor = SV.Get<bool>( SItem.bG_Sim_OnGround ) ? cTxActive : cTxLabel;
          _value1.Value = SV.Get<float>( SItem.fG_Acft_AltAoG_ft );
        }
        else {
          this.ColorType.ItemForeColor = cTxLabel;
          _value1.Text = " .....";
        }
      }
    }

  }
}
